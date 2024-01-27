using GlobalGameJam2024.Simulation;
using GlobalGameJam2024.Simulation.Commands;
using GlobalGameJam2024.Simulation.Services.Network;
using GlobalGameJam2024.WebApp.Client.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace GlobalGameJam2024.WebApp.Controllers;

public class GameController : ControllerBase
{
	private readonly ILogger logger;
	private readonly LobbyService lobbyService;

	public GameController(
		ILogger<GameController> logger,
		LobbyService lobbyService)
	{
		this.logger = logger;
		this.lobbyService = lobbyService;
	}

	[Route("/api/game")]
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

		logger.Log(LogLevel.Information, "Accepted client WebSocket connection");

		var playerID = LocalId.NewId();

		await foreach (var networkEvent in socketChannel.InboundMessages.ReadAllAsync(cancellationToken))
		{
			switch (networkEvent)
			{
				case WebSocketBinaryMessageEvent message:
				{
					logger.LogInformation(Encoding.UTF8.GetString(message.MessageContent.Body));

					var clientCommand = await JsonSerializer.DeserializeAsync<ClientCommand>(message.MessageContent.ReadStream());

					switch (clientCommand)
					{
						case MoveClientCommand moveClientCommand:
						{
							await lobbyService.SendToHostClient(new IntakeClientCommandHostProcedure()
							{
								PlayerID = playerID,
								Command = moveClientCommand
							});
							break;
						}
						case UseAbilityClientCommand useAbilityClientCommand:
						{
							await lobbyService.SendToHostClient(new IntakeClientCommandHostProcedure()
							{
								PlayerID = playerID,
								Command = useAbilityClientCommand
							});
							break;
						}
					}

					message.MessageContent.ReturnToPool();
					break;
				}
				case WebSocketExceptionDisconnectEvent disconnectEvent:
				{
					logger.LogError(disconnectEvent.InnerException, "Client connection encountered an exception");
					break;
				}
				case WebSocketDisconnectEvent disconnectEvent:
				{
					break;
				}
			}
		}

		logger.Log(LogLevel.Information, "Closed client WebSocket connection");
	}
}
