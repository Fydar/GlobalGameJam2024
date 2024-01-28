using GlobalGameJam2024.Simulation.Commands;
using GlobalGameJam2024.Simulation.Services.Network;
using GlobalGameJam2024.WebApp.Client.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace GlobalGameJam2024.WebApp.Controllers;

public class HostController : ControllerBase
{
	private readonly ILogger logger;
	private readonly LobbyService lobbyService;

	public HostController(
		ILogger<HostController> logger,
		LobbyService lobbyService)
	{
		this.logger = logger;
		this.lobbyService = lobbyService;
	}

	[Route("/api/host")]
	[ProducesResponseType(StatusCodes.Status101SwitchingProtocols)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task Get()
	{
		var context = ControllerContext.HttpContext;
		bool isSocketRequest = context.WebSockets.IsWebSocketRequest;
		if (!isSocketRequest)
		{
			context.Response.StatusCode = 400;
			return;
		}

		var cancellationToken = context.RequestAborted;
		using var currentSocket = await context.WebSockets.AcceptWebSocketAsync();

		var socketChannel = WebSocketReceiveWorker.ReceiveFrom(currentSocket);

		logger.Log(LogLevel.Information, "Accepted host client WebSocket connection");

		lobbyService.HostClient = new Client.Services.Client()
		{
			WebSocketReceiveWorker = socketChannel
		};

		await foreach (var networkEvent in socketChannel.InboundMessages.ReadAllAsync(cancellationToken))
		{
			switch (networkEvent)
			{
				case WebSocketBinaryMessageEvent message:
				{
					logger.LogInformation(Encoding.UTF8.GetString(message.MessageContent.Body));

					var hostCommand = await JsonSerializer.DeserializeAsync<HostCommand>(message.MessageContent.ReadStream());

					switch (hostCommand)
					{
						case BroadcastProcedureCommand broadcastProcedureCommand:
						{
							await lobbyService.SendToAllClients(broadcastProcedureCommand.Procedure);
							break;
						}
						case SendProcedureCommand sendProcedureCommand:
						{
							byte[] serializedProcedure = JsonSerializer.SerializeToUtf8Bytes(sendProcedureCommand.Procedure);

							if (lobbyService.Clients.TryGetValue(sendProcedureCommand.PlayerID, out var client))
							{
								await client.WebSocketReceiveWorker.WebSocket.SendAsync(serializedProcedure, WebSocketMessageType.Binary, true, cancellationToken);
							}
							break;
						}
						case LaunchGameHostCommand broadcastProcedureCommand:
						{

							break;
						}
					}

					message.MessageContent.ReturnToPool();
					break;
				}
				case WebSocketExceptionDisconnectEvent disconnectEvent:
				{
					logger.LogError(disconnectEvent.InnerException, "Host client connection encountered an exception");
					break;
				}
				case WebSocketDisconnectEvent disconnectEvent:
				{
					break;
				}
			}
		}

		lobbyService.HostClient = null;

		logger.Log(LogLevel.Information, "Closed host client WebSocket connection");
	}
}
