using System.Collections;
using UnityEngine;

namespace GlobalGameJam2024
{
	public class Projectile : MonoBehaviour
	{
		private int damage = 10;
		private float projectileSpeed = 5.0f;
		private PlayerCharacter owner;
		private bool hasCollided = false;

		private IEnumerator Start()
		{
			while (!hasCollided)
			{
				yield return null;

				transform.position += new Vector3(0.0f, 0.0f, projectileSpeed * Time.deltaTime);
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (collision.rigidbody != null)
			{
				if (collision.rigidbody.TryGetComponent<PlayerCharacter>(out var collidedPlayerCharacter))
				{
					if (collidedPlayerCharacter == owner)
					{
						return;
					}
					else
					{
						// Deal damage
					}
				}
			}
			hasCollided = true;
		}

		public void Setup(PlayerCharacter owner, AbilityFireArrow abilityFireArrow)
		{
			this.owner = owner;
			damage = abilityFireArrow.Damage;
			projectileSpeed = abilityFireArrow.ProjectileSpeed;
		}
	}
}
