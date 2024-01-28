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
			var cast = Physics.BoxCastAll(character.transform.position,
				new Vector3(radius, 10000, radius), Vector3.up, character.transform.rotation);

			foreach (var hit in cast)
			{
				var hitCharacter = hit.transform.GetComponent<EnemyCharacter>();
				if (hitCharacter != null)
				{
					if (hitCharacter == character)
					{
						continue;
					}
					hitCharacter.TakeDamage(damage);
				}
			}
			yield return null;
		}
	}
}
