using GlobalGameJam2024.Simulation.Commands;
using GlobalGameJam2024.Simulation.Procedures;
using GlobalGameJam2024.Simulation.Services.Network;
using Microsoft.AspNetCore.Components;
using System.Text;
using System.Text.Json;

namespace GlobalGameJam2024.WebApp.Client.Services;

public class ClientService : IClientService
{
	private readonly ILogger logger;
	private WebSocketReceiveWorker? webSocketChannel;
	private TaskCompletionSource<WebSocketReceiveWorker> connectionTask;

	public ClientService(
		ILogger<ClientService> logger,
		NavigationManager navigationManager)
	{
		this.logger = logger;

		string url = navigationManager.BaseUri
			.Replace("http://", "ws://")
			.Replace("https://", "wss://")
			.TrimEnd('/');

		connectionTask = new TaskCompletionSource<WebSocketReceiveWorker>();
		_ = RunAsync($"{url}/api/game");
	}

	public async Task SendCommandAsync(ClientCommand clientCommand, CancellationToken cancellationToken = default)
	{
		if (webSocketChannel == null)
		{
			throw new InvalidOperationException($"Cannot perform '{nameof(SendCommandAsync)}' without first calling '{nameof(RunAsync)}'.");
		}

		byte[] data = JsonSerializer.SerializeToUtf8Bytes(clientCommand);

		await connectionTask.Task;

		await webSocketChannel.WebSocket.SendAsync(data, System.Net.WebSockets.WebSocketMessageType.Binary, true, cancellationToken);
	}

	private async Task RunAsync(string serverUrl, CancellationToken cancellationToken = default)
	{
		var target = new Uri(serverUrl);

		// Client
		logger.LogInformation($"Connecting to {target}");

		try
		{
			webSocketChannel = await WebSocketReceiveWorker.ConnectAsync(target, cancellationToken);
		}
		catch (Exception exception)
		{
			logger.LogError(exception, "Error whilst connecting");
			return;
		}

		connectionTask.SetResult(webSocketChannel);

		await foreach (var networkEvent in webSocketChannel.InboundMessages.ReadAllAsync(cancellationToken))
		{
			switch (networkEvent)
			{
				case WebSocketBinaryMessageEvent message:
				{
					try
					{
						var clientProcedure = await JsonSerializer.DeserializeAsync<ClientProcedure>(message.MessageContent.ReadStream(), cancellationToken: cancellationToken);

						logger.LogInformation(Encoding.UTF8.GetString(message.MessageContent.Body));

						switch (clientProcedure)
						{
							case UpdateGameStateClientProcedure updateGameStateClientProcedure:
							{

								break;
							}
						}
					}
					catch (Exception exception)
					{
						logger.LogError(exception, "Error whilst applying procedure to view");
					}
					break;
				}
				case WebSocketExceptionDisconnectEvent disconnectEvent:
				{
					logger.LogError(disconnectEvent.InnerException, "Client connection encountered an exception)");
					break;
				}
				case WebSocketDisconnectEvent:
				{
					logger.LogInformation("Connection with client closed successfully");
					break;
				}
			}
		}
	}
}
