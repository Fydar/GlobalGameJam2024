using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace GlobalGameJam2024
{
	public class EnemyCharacter : MonoBehaviour
	{
		public Dictionary<PlayerCharacter, CharacterAIThreat> Threats = new();

		public void GenerateThreat(PlayerCharacter character, int threat)
		{
			var threatHolder = ModifyThreat(character);
			threatHolder.Threat += threat;
		}

		public CharacterAIThreat ModifyThreat(PlayerCharacter character)
		{
			if (!Threats.TryGetValue(character, out var value))
			{
				value = new CharacterAIThreat();
				Threats[character] = value;
				return value;
			}
			return value;
		}

		public PlayerCharacter GetCurrentTarget()
		{
			PlayerCharacter highestThreatCharacter = null;
			int highestThreat = int.MinValue;

			foreach (var threat in Threats)
			{
				if (threat.Key == null)
				{
					continue;
				}
				if (threat.Value.Threat >= highestThreat)
				{
					highestThreatCharacter = threat.Key;
					highestThreat = threat.Value.Threat;
				}
			}

			return highestThreatCharacter;
		}

		private void FixedUpdate()
		{
			// Detect nearby enemies
			foreach (var target in GameController.Instance.players.Values)
			{
				float distance = Vector3.Distance(transform.position, target.transform.position);
				int distanceThreat = Mathf.Max(1, 50 - (int)(distance * 10));

				var targetForThreat = ModifyThreat(target);

				targetForThreat.Threat = Mathf.Max(distanceThreat, targetForThreat.Threat);
			}

			var newTarget = GetCurrentTarget();
			CurrentTarget = newTarget;
			if (newTarget != null)
			{
				Agent.SetDestination(newTarget.transform.position);
				Agent.stoppingDistance = AttackStoppingDistance;
			}
			else
			{
				Agent.SetDestination(new Vector3(0.0f, 0.0f, 0.0f));
				Agent.stoppingDistance = 0.0f;
			}
		}
		public float AttackRange = 0.2f;
		public float AttackStoppingDistance = 0.2f;
		public float AttackCooldown = 0.2f;
		public int MaxHealth = 100;

		[Header("Behaviour")]
		public float AimSpeed = 15.0f;

		[Header("State")]
		public int Health = 100;

		[Header("Graphics")]
		public Image HealthBar;

		private PlayerCharacter CurrentTarget;
		private NavMeshAgent Agent;

		[Space]
		private float TimeSinceLastAttack;

		private void Awake()
		{
			Agent = GetComponent<NavMeshAgent>();

			if (HealthBar != null)
			{
				HealthBar.fillAmount = 1.0f;
			}
		}

		private void Update()
		{
			TimeSinceLastAttack += Time.deltaTime;

			if (CurrentTarget != null)
			{
				float distance = Vector3.Distance(transform.position, CurrentTarget.transform.position);

				if (distance < AttackRange)
				{
					if (TimeSinceLastAttack > AttackCooldown)
					{
						TimeSinceLastAttack = 0.0f;
						// Do attack
					}
				}
				if (distance < AttackStoppingDistance)
				{
					RotateTowards(CurrentTarget.transform);
				}
			}
		}

		private void RotateTowards(Transform target)
		{
			var direction = (target.position - transform.position).normalized;
			var lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * AimSpeed);
		}

		public void TakeDamage(int damage)
		{
			Health -= damage;

			if (HealthBar != null)
			{
				float fill = (float)Health / MaxHealth;
				HealthBar.fillAmount = fill;
			}

			if (Health <= 0)
			{
				Destroy(gameObject);
			}
		}
	}
}
