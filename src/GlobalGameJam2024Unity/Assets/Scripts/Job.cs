using UnityEngine;

namespace GlobalGameJam2024
{
	[CreateAssetMenu(menuName = "Data/Job")]
	public class Job : ScriptableObject
	{
		[SerializeField] private string displayName;
	}
}
