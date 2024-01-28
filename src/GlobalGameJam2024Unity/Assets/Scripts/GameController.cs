using Cinemachine;
using GlobalGameJam2024.Simulation;
using GlobalGameJam2024.Simulation.Commands;
using GlobalGameJam2024.Simulation.Services.Network;
using NUnit.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using UnityEngine;

namespace GlobalGameJam2024
{
	public class GameController : MonoBehaviour
	{
		[Header("Configuration")]
		[SerializeField] private string url;

		[Header("Data")]
		[SerializeField] private Job[] jobs;

		[Header("Introduction")]
		[SerializeField] private CanvasGroup blackFade;
		[SerializeField] private CanvasGroup lobbyHUDFade;
		[SerializeField] private CinemachineVirtualCamera cameraCamp;
		[SerializeField] private CinemachineVirtualCamera cameraPanAcross;
		[SerializeField] private CinemachineVirtualCamera cameraPullBackStart;
		[SerializeField] private CinemachineVirtualCamera cameraPullBackMiddle;
		[SerializeField] private CinemachineVirtualCamera cameraPullBackEnd;
		[SerializeField] private CinemachineVirtualCamera gameplayCamera;

		[Header("Health Bar")]
		[SerializeField] private CanvasGroup healthBarFade;
		[SerializeField] private RectTransform healthBarGrow;

		[Header("Spawn")]
		[SerializeField] private Transform[] spawnPoints;

		private Dictionary<LocalId, PlayerCharacter> players = new();
		private Dictionary<LocalId, Transform> playerToSpawnPoint = new();

		private enum GamePhase
		{
			Connecting,
			Lobby,
			Introduction,
			Gameplay,
			Cinematic
		}
		private GamePhase Phase;

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
			healthBarFade.alpha = 0.0f;
			lobbyHUDFade.alpha = 0.0f;

			cameraCamp.gameObject.SetActive(true);
			cameraPanAcross.gameObject.SetActive(false);
			cameraPullBackStart.gameObject.SetActive(false);
			cameraPullBackMiddle.gameObject.SetActive(false);
			cameraPullBackEnd.gameObject.SetActive(false);
			gameplayCamera.gameObject.SetActive(false);

			// Connect to server...
			clientService = new ClientService(url);

			Phase = GamePhase.Connecting;

			while (!clientService.connectionTask.Task.IsCompletedSuccessfully)
			{
				yield return null;
			}

			// Lobby
			Phase = GamePhase.Lobby;
			cameraCamp.gameObject.SetActive(true);
			yield return StartCoroutine(FadeFromBlack());

			foreach (float time in new TimedLoop(1.0f))
			{
				lobbyHUDFade.alpha = time;
				yield return null;
			}

			while (true)
			{
				yield return null;

				foreach (var player in players.Values)
				{
					if (!player.enabled)
					{
						player.enabled = false;
						foreach (var spawnPoint in spawnPoints)
						{
							bool occupied = false;
							foreach (var ocupacy in playerToSpawnPoint)
							{
								if (ocupacy.Value == spawnPoint)
								{
									occupied = true;
									break;
								}
							}
							if (!occupied)
							{
								playerToSpawnPoint[player.PlayerID] = spawnPoint;
								player.transform.position = spawnPoint.transform.position;
								player.transform.rotation = spawnPoint.transform.rotation;
								player.enabled = true;
								player.targetPosition = spawnPoint.transform.position;
								break;
							}
						}
					}
				}

				if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
				{
					break;
				}
			}

			foreach (float time in new TimedLoop(1.0f))
			{
				lobbyHUDFade.alpha = 1.0f - time;
				yield return null;
			}
			yield return StartCoroutine(FadeToBlack(1.0f));

			// Start introduction cinematic by panning across the troops rearranged.
			Phase = GamePhase.Introduction;
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

			yield return new WaitForSeconds(1.0f);

			float orignalSizeDeltaX = healthBarGrow.sizeDelta.x;
			foreach (float time in new TimedLoop(2.0f))
			{
				healthBarFade.alpha = time * 3.0f;
				healthBarGrow.sizeDelta = new Vector2(orignalSizeDeltaX * time, healthBarGrow.sizeDelta.y);
			}

			yield return new WaitForSeconds(1.0f);


			// Now bring the camera behind the soldiers, focused on them, with McFunkypants in the background,
			// as the soldiers ready themselves for battle.
			cameraPullBackMiddle.gameObject.SetActive(false);
			cameraPullBackEnd.gameObject.SetActive(true);

			yield return new WaitForSeconds(4.0f);
			yield return StartCoroutine(FadeToBlack(2.0f));

			// Now transition to the normal gameplay camera.
			cameraPullBackEnd.gameObject.SetActive(false);
			gameplayCamera.gameObject.SetActive(true);

			yield return new WaitForSeconds(0.5f);

			Phase = GamePhase.Gameplay;
			yield return StartCoroutine(FadeFromBlack(2.0f));

			yield return new WaitForSeconds(5.0f);

			while (true)
			{
				// _ = clientService.SendCommandAsync();

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
							Debug.Log(Encoding.UTF8.GetString(message.MessageContent.Body));

							var hostProcedure = JsonSerializer.Deserialize<HostProcedure>(message.MessageContent.ReadStream());

							switch (hostProcedure)
							{
								case PlayerJoinedHostProcedure playerJoinedHostProcedure:
								{
									var playerCharacterGameObject = new GameObject($"{playerJoinedHostProcedure.DisplayName} ({playerJoinedHostProcedure.PlayerID})");
									var playerCharacter = playerCharacterGameObject.AddComponent<PlayerCharacter>();

									playerCharacter.PlayerID = playerJoinedHostProcedure.PlayerID;
									playerCharacter.DisplayName = playerJoinedHostProcedure.DisplayName;
									playerCharacter.Job = jobs[UnityEngine.Random.Range(0, jobs.Length - 1)];
									playerCharacter.Graphics = Instantiate(playerCharacter.Job.characterPrefab, playerCharacter.transform);

									players.Add(playerCharacter.PlayerID, playerCharacter);
									playerCharacter.enabled = false;
									break;
								}
								case PlayerLeftHostProcedure playerLeftHostProcedure:
								{
									if (players.TryGetValue(playerLeftHostProcedure.PlayerID, out var disconnectingPlayer))
									{
										players.Remove(playerLeftHostProcedure.PlayerID);
										Destroy(disconnectingPlayer.gameObject);
									}
									if (playerToSpawnPoint.TryGetValue(playerLeftHostProcedure.PlayerID, out var spawnPoint))
									{
										playerToSpawnPoint.Remove(playerLeftHostProcedure.PlayerID);
									}
									break;
								}
								case IntakeClientCommandHostProcedure intakeClientCommandHostProcedure:
								{
									switch (intakeClientCommandHostProcedure.Command)
									{
										case MoveClientCommand moveClientCommand:
										{
											if (Phase == GamePhase.Gameplay && players.TryGetValue(intakeClientCommandHostProcedure.PlayerID, out var commandingPlayer))
											{
												commandingPlayer.targetPosition = new Vector3(
													Mathf.Lerp(-15.0f, 15.0f, moveClientCommand.MoveToX),
													0.0f,
													Mathf.Lerp(10.0f, -5.0f, moveClientCommand.MoveToY));
											}
											break;
										}
									}
									break;
								}
							}

							message.MessageContent.ReturnToPool();
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
