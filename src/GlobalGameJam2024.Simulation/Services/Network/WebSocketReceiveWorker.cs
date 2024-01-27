using System;
using System.Buffers;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace GlobalGameJam2024.Simulation.Services.Network;

public sealed class WebSocketReceiveWorker
{
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly Channel<IWebSocketEvent> inboundMessages;

	public WebSocket WebSocket { get; }
	public ChannelReader<IWebSocketEvent> InboundMessages => inboundMessages.Reader;
	public Task Completion { get; }

	private WebSocketReceiveWorker(
		WebSocket webSocket,
		CancellationToken cancellationToken = default)
	{
		WebSocket = webSocket;
		inboundMessages = Channel.CreateUnbounded<IWebSocketEvent>();

		Completion = ReceiveWorkerAsync(cancellationToken);
	}

	public static async Task<WebSocketReceiveWorker> ConnectAsync(
		Uri uri,
		CancellationToken cancellationToken = default)
	{
		var clientWebSocket = new ClientWebSocket();

		await clientWebSocket.ConnectAsync(uri, cancellationToken);

		return new WebSocketReceiveWorker(clientWebSocket, cancellationToken);
	}

	public static WebSocketReceiveWorker ReceiveFrom(
		WebSocket webSocket,
		CancellationToken cancellationToken = default)
	{
		return new WebSocketReceiveWorker(webSocket, cancellationToken);
	}

	private async Task ReceiveWorkerAsync(
		CancellationToken cancellationToken = default)
	{
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();

			byte[] rentedBuffer = ArrayPool<byte>.Shared.Rent(8192);
			var bufferSegment = new ArraySegment<byte>(rentedBuffer);
			int totalSize = 0;
			DateTimeOffset? startTime = null;
			WebSocketReceiveResult? result = null;
			do
			{
				cancellationToken.ThrowIfCancellationRequested();

				if (WebSocket.State != WebSocketState.Open
					&& WebSocket.State != WebSocketState.Connecting)
				{
					var closeEvent = new WebSocketDisconnectEvent(
						this,
						DateTimeOffset.Now,
						DateTimeOffset.Now,
						WebSocket.CloseStatus ?? WebSocketCloseStatus.Empty);

					await inboundMessages.Writer.WriteAsync(closeEvent);
					inboundMessages.Writer.Complete();
					break;
				}

				Exception? innerException = null;

				try
				{
					result = await WebSocket.ReceiveAsync(bufferSegment, cancellationToken);
					if (result == null)
					{
						throw new InvalidOperationException("Failed to get response from receive.");
					}
				}
				catch (Exception exception)
				{
					innerException = exception;
				}
				startTime ??= DateTimeOffset.UtcNow;
				totalSize += result?.Count ?? 0;

				if (innerException != null)
				{
					var exceptionDisconnectedEvent = new WebSocketExceptionDisconnectEvent(
						this,
						startTime.Value,
						DateTimeOffset.Now,
						result?.CloseStatus ?? WebSocketCloseStatus.Empty,
						innerException);

					await inboundMessages.Writer.WriteAsync(exceptionDisconnectedEvent);
					inboundMessages.Writer.Complete();
					break;
				}

				if (!(result?.EndOfMessage ?? true))
				{
					byte[] newBuffer = ArrayPool<byte>.Shared.Rent(rentedBuffer.Length * 2);
					rentedBuffer.CopyTo(newBuffer, 0);

					bufferSegment = new ArraySegment<byte>(newBuffer, rentedBuffer.Length,
						newBuffer.Length - rentedBuffer.Length);

					ArrayPool<byte>.Shared.Return(rentedBuffer);
					rentedBuffer = newBuffer;
				}
			}
			while (!(result?.EndOfMessage ?? true));

			if (result?.CloseStatus.HasValue ?? true)
			{
				ArrayPool<byte>.Shared.Return(rentedBuffer);

				var disconnectedEvent = new WebSocketDisconnectEvent(
					this,
					startTime ?? DateTimeOffset.UtcNow,
					DateTimeOffset.Now,
					result?.CloseStatus ?? WebSocketCloseStatus.Empty);

				await inboundMessages.Writer.WriteAsync(disconnectedEvent);
				inboundMessages.Writer.Complete();
				break;
			}
			else
			{
				var messagedReceivedEvent = new WebSocketBinaryMessageEvent(
					this,
					startTime ?? DateTimeOffset.UtcNow,
					DateTimeOffset.Now,
					new WebSocketMessageContent(rentedBuffer, 0, totalSize));

				await inboundMessages.Writer.WriteAsync(messagedReceivedEvent);
			}
		}
	}
}
