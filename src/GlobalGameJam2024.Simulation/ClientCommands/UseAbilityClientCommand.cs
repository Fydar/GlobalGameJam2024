using System.Numerics;

namespace GlobalGameJam2024.Simulation.Commands;

public class UseAbilityClientCommand : ClientCommand
{
	public Vector2 TargetLocation { get; set; }
}
