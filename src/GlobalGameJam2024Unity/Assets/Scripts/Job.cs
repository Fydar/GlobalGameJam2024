using UnityEngine;

namespace GlobalGameJam2024
{
	[CreateAssetMenu(menuName = "Data/Job")]
	public class Job : ScriptableObject
	{
		[SerializeField] public string displayName;
		[SerializeField] public Sprite icon;
		[SerializeField] public Color accent;
		[SerializeField] public Ability primaryAbility;
		[SerializeField] public GameObject characterPrefab;
	}
}
