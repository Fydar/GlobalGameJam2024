using System.Collections;
using UnityEngine;

namespace GlobalGameJam2024
{
	public abstract class Ability : ScriptableObject
	{
		[SerializeField] private string displayName;

		public abstract IEnumerator PlayAbility(PlayerCharacter character);
	}
}
