using Cinemachine;
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
		[SerializeField] private CinemachineVirtualCamera cameraCamp;
		[SerializeField] private CinemachineVirtualCamera cameraPanAcross;
		[SerializeField] private CinemachineVirtualCamera cameraPullBackStart;
		[SerializeField] private CinemachineVirtualCamera cameraPullBackMiddle;
		[SerializeField] private CinemachineVirtualCamera cameraPullBackEnd;
		[SerializeField] private CinemachineVirtualCamera gameplayCamera;


		private ClientService clientService;

		private IEnumerator FadeToBlack(float duration = 1.0f)
		{
			foreach (float time in new TimedLoop(duration))
			{
				blackFade.alpha = time;
				yield return null;
			}
		}

		private IEnumerator FadeFromBlack(float duration = 1.0f)
		{
			foreach (float time in new TimedLoop(duration))
			{
				blackFade.alpha = 1.0f - time;
				yield return null;
			}
		}

		public IEnumerator Start()
		{
			blackFade.alpha = 1.0f;

			cameraCamp.gameObject.SetActive(true);
			cameraPanAcross.gameObject.SetActive(false);
			cameraPullBackStart.gameObject.SetActive(false);
			cameraPullBackMiddle.gameObject.SetActive(false);
			cameraPullBackEnd.gameObject.SetActive(false);
			gameplayCamera.gameObject.SetActive(false);

			// Connect to server...
			clientService = new ClientService(url);

			while (!clientService.connectionTask.Task.IsCompletedSuccessfully)
			{
				yield return null;
			}

			cameraCamp.gameObject.SetActive(true);
			yield return StartCoroutine(FadeFromBlack());

			// Lobby
			yield return new WaitForSeconds(3.0f);

			yield return StartCoroutine(FadeToBlack(1.0f));

			// Start introduction cinematic by panning across the troops rearranged.
			cameraCamp.gameObject.SetActive(false);
			cameraPanAcross.gameObject.SetActive(true);

			yield return StartCoroutine(FadeFromBlack(1.0f));

			yield return new WaitForSeconds(5.0f);

			yield return StartCoroutine(FadeToBlack(2.0f));

			// Now bring the camera behind the soldiers, focused on them.
			cameraPanAcross.gameObject.SetActive(false);
			cameraPullBackStart.gameObject.SetActive(true);

			yield return StartCoroutine(FadeFromBlack(1.0f));

			yield return new WaitForSeconds(4.0f);


			// Now focus on McFunkypants as he completes his rise.
			cameraPullBackStart.gameObject.SetActive(false);
			cameraPullBackMiddle.gameObject.SetActive(true);

			yield return new WaitForSeconds(4.0f);

			// Now bring the camera behind the soldiers, focused on them, with McFunkypants in the background,
			// as the soldiers ready themselves for battle.
			cameraPullBackMiddle.gameObject.SetActive(false);
			cameraPullBackEnd.gameObject.SetActive(true);

			yield return new WaitForSeconds(4.0f);

			// Now transition to the normal gameplay camera.
			cameraPullBackEnd.gameObject.SetActive(false);
			gameplayCamera.gameObject.SetActive(true);

			yield return new WaitForSeconds(2.0f);
			yield return StartCoroutine(FadeToBlack(2.0f));
			yield return StartCoroutine(FadeFromBlack(2.0f));

			yield return new WaitForSeconds(5.0f);

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
