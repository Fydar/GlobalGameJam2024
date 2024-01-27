using GlobalGameJam2024.Simulation.Commands;
using GlobalGameJam2024.Simulation.Services.Network;
using System;
using System.Collections;
using UnityEngine;

namespace GlobalGameJam2024
{
	public class GameController : MonoBehaviour
	{
		[SerializeField] private string url;
		[SerializeField] private CanvasGroup blackFade;

		private ClientService clientService;

		public IEnumerator Start()
		{
			clientService = new ClientService(url);

			while (!clientService.connectionTask.Task.IsCompletedSuccessfully)
			{
				yield return null;
			}

			foreach (float time in new TimedLoop(1.0f))
			{
				blackFade.alpha = 1.0f - time;
			}

			yield return null;

			while (true)
			{
				_ = clientService.SendCommandAsync(new MoveClientCommand()
				{
					MoveTo = new System.Numerics.Vector2(50.0f, 25.0f)
				});

				yield return new WaitForSeconds(1.0f);
			}
		}

		private void Update()
		{
			if (clientService?.webSocketChannel?.InboundMessages == null)
			{
				return;
			}
			while (clientService.webSocketChannel.InboundMessages.TryRead(out var networkEvent))
			{
				switch (networkEvent)
				{
					case WebSocketBinaryMessageEvent message:
					{
						try
						{
							using var stream = message.MessageContent.ReadStream();
							// var procedure = DeserializeProcedure(message.MessageContent.ReadStream());
							// procedure.ApplyToView(View);
							// OnProcedureApplied?.Invoke(procedure);
						}
						catch (Exception exception)
						{
							Debug.LogError("Error whilst applying procedure to view");
							Debug.LogException(exception);
						}
						break;
					}
					case WebSocketExceptionDisconnectEvent disconnectEvent:
					{
						Debug.LogError("Client connection encountered an exception");
						Debug.LogException(disconnectEvent.InnerException);
						break;
					}
					case WebSocketDisconnectEvent:
					{
						Debug.Log("Connection with client closed successfully");
						break;
					}
				}
			}
		}
	}
}
