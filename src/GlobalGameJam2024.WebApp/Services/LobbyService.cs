using GlobalGameJam2024.Simulation;
using GlobalGameJam2024.Simulation.Commands;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;

namespace GlobalGameJam2024.WebApp.Client.Services;

public class LobbyService
{
	public Client HostClient { get; set; }
	public ConcurrentDictionary<LocalId, Client> Clients { get; } = new ConcurrentDictionary<LocalId, Client>();

	public LobbyService()
	{
	}

	internal async Task SendToAllClients(ClientProcedure procedure, CancellationToken cancellationToken = default)
	{
		byte[] serializedProcedure = JsonSerializer.SerializeToUtf8Bytes(procedure);

		var tasks = new List<Task>();
		foreach (var client in Clients)
		{
			tasks.Add(client.Value.WebSocketReceiveWorker.WebSocket.SendAsync(serializedProcedure, WebSocketMessageType.Binary, true, cancellationToken));
		}
		await Task.WhenAll(tasks);
	}

	internal Task SendToHostClient(HostProcedure procedure, CancellationToken cancellationToken = default)
	{
		if (HostClient == null)
		{
			return Task.CompletedTask;
		}

		byte[] serializedProcedure = JsonSerializer.SerializeToUtf8Bytes(procedure);
		return HostClient.WebSocketReceiveWorker.WebSocket.SendAsync(serializedProcedure, WebSocketMessageType.Binary, true, cancellationToken);
	}
}
