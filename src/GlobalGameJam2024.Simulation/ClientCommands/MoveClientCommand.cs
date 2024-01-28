using System.Numerics;

namespace GlobalGameJam2024.Simulation.Commands;

public class MoveClientCommand : ClientCommand
{
	public float MoveToX { get; set; }
	public float MoveToY { get; set; }
}
