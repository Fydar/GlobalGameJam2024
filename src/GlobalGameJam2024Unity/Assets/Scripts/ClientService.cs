using GlobalGameJam2024.Simulation.Commands;
using GlobalGameJam2024.Simulation.Services.Network;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GlobalGameJam2024
{
	public class ClientService
	{
		public WebSocketReceiveWorker? webSocketChannel;
		public TaskCompletionSource<WebSocketReceiveWorker> connectionTask;

		public ClientService(string url)
		{
			connectionTask = new TaskCompletionSource<WebSocketReceiveWorker>();
			url = url
				.Replace("http://", "ws://")
				.Replace("https://", "wss://")
				.TrimEnd('/');

			_ = RunAsync($"{url}/api/host");
		}

		public async Task SendCommandAsync(HostCommand command, CancellationToken cancellationToken = default)
		{
			if (webSocketChannel == null)
			{
				throw new InvalidOperationException($"Cannot perform '{nameof(SendCommandAsync)}' without first calling '{nameof(RunAsync)}'.");
			}

			byte[] data = JsonSerializer.SerializeToUtf8Bytes(command);

			await connectionTask.Task;

			await webSocketChannel.WebSocket.SendAsync(data, System.Net.WebSockets.WebSocketMessageType.Binary, true, cancellationToken);
		}

		private async Task RunAsync(string serverUrl, CancellationToken cancellationToken = default)
		{
			var target = new Uri(serverUrl);

			// Client
			Debug.Log($"Connecting to {target}");

			try
			{
				webSocketChannel = await WebSocketReceiveWorker.ConnectAsync(target, cancellationToken);
			}
			catch (Exception exception)
			{
				Debug.LogError("Error whilst connecting");
				Debug.LogException(exception);
				return;
			}

			connectionTask.SetResult(webSocketChannel);
		}


		// private NetworkedViewProcedure DeserializeProcedure(Stream data)
		// {
		// 	using var sr = new StreamReader(data);
		// 	using var jr = new JsonTextReader(sr);
		// 
		// 	var deserialized = serializer.Deserialize<PackagedModel<NetworkedViewProcedure>>(jr);
		// 
		// 	if (deserialized == null)
		// 	{
		// 		throw new InvalidOperationException($"Failed to deserialize {nameof(PackagedModel<NetworkedViewProcedure>)}.");
		// 	}
		// 
		// 	return deserialized.Deserialize();
		// }
	}
}
