using System.Collections;
using UnityEngine;

namespace GlobalGameJam2024
{
	[CreateAssetMenu(menuName = "Data/Abilities/Whirlwind")]
	public class AbilityWhirlwind : Ability
	{
		[SerializeField] private int damage = 25;
		[SerializeField] private float radius = 1.0f;

		public override IEnumerator PlayAbility(PlayerCharacter character)
		{
			yield return null;
		}
	}
}
