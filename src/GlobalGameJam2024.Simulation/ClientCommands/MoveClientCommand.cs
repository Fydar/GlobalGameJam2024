using System.Numerics;

namespace GlobalGameJam2024.Simulation.Commands;

public class MoveClientCommand : ClientCommand
{
	public Vector2 MoveTo { get; set; }
}
