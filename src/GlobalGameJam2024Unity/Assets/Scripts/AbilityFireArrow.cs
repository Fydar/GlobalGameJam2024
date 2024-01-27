using System.Collections;
using UnityEngine;

namespace GlobalGameJam2024
{
	[CreateAssetMenu(menuName = "Data/Abilities/Fire Arrow")]
	public class AbilityFireArrow : Ability
	{
		[SerializeField] private int damage = 10;
		[SerializeField] private float projectileSpeed = 5.0f;
		[SerializeField] private GameObject projectilePrefab;

		public int Damage => damage;
		public float ProjectileSpeed => projectileSpeed;

		public override IEnumerator PlayAbility(PlayerCharacter character)
		{
			var walkSpeedModifier = new StatMultiplier();
			character.WalkSpeed.Multipliers.Add(walkSpeedModifier);

			foreach (float time in new TimedLoop(0.2f))
			{
				walkSpeedModifier.Factor = Mathf.Lerp(0.8f, 0.2f, time);
			}

			yield return new WaitForSeconds(0.05f);

			var projectileGameObject = Instantiate(projectilePrefab, character.transform.position, Quaternion.identity);
			var projectile = projectileGameObject.GetComponent<Projectile>();
			projectile.Setup(character, this);

			foreach (float time in new TimedLoop(0.1f))
			{
				walkSpeedModifier.Factor = Mathf.Lerp(0.2f, 1f, time);
			}
			character.WalkSpeed.Multipliers.Remove(walkSpeedModifier);
		}
	}
}
