using GlobalGameJam2024.Simulation;
using System;
using System.Collections.Generic;
using UnityEngine;

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
		public LocalId PlayerID;
		public string DisplayName;
		public Job Job;

		[SerializeField] private StatStack walkSpeed = new(10);

		[SerializeField] private Ability primaryAbility;

		public Vector3 targetPosition;

		public StatStack WalkSpeed => walkSpeed;

		private void Update()
		{
			if (Input.GetKey(KeyCode.W))
			{
				targetPosition += new Vector3(0, 0, walkSpeed.Value * Time.deltaTime);
			}
			if (Input.GetKey(KeyCode.S))
			{
				targetPosition += new Vector3(0, 0, -walkSpeed.Value * Time.deltaTime);
			}
			if (Input.GetKey(KeyCode.A))
			{
				targetPosition += new Vector3(-walkSpeed.Value * Time.deltaTime, 0, 0);
			}
			if (Input.GetKey(KeyCode.D))
			{
				targetPosition += new Vector3(walkSpeed.Value * Time.deltaTime, 0, 0);
			}
			if (Input.GetKeyDown(KeyCode.Space))
			{
				StartCoroutine(primaryAbility.PlayAbility(this));
			}

			transform.position = Vector3.MoveTowards(transform.position, targetPosition, walkSpeed.Value * Time.deltaTime);
		}
	}
}
