using GlobalGameJam2024.Simulation;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GlobalGameJam2024
{
	public class StatMultiplier
	{
		public float Factor { get; set; }
	}

	[Serializable]
	public class StatStack
	{
		[NonSerialized]
		public readonly List<StatMultiplier> Multipliers = new();

		[SerializeField]
		public float BaseValue;

		public StatStack(float baseValue)
		{
			BaseValue = baseValue;
		}

		public float Value
		{
			get
			{
				var value = BaseValue;
				foreach (var multiplier in Multipliers)
				{
					value *= multiplier.Factor;
				}
				return value;
			}
		}


	}

	public class PlayerCharacter : MonoBehaviour
	{
		public NavMeshAgent agent;

		public LocalId PlayerID;
		public string DisplayName;
		public Job Job;
		public GameObject Graphics;

		[SerializeField] private StatStack walkSpeed = new(10);

		[SerializeField] private Ability primaryAbility;


		public StatStack WalkSpeed => walkSpeed;

		private void Start()
		{
			agent = GetComponent<NavMeshAgent>();
		}

		private void Update()
		{
			agent.speed = walkSpeed.Value;
		}
	}
}
