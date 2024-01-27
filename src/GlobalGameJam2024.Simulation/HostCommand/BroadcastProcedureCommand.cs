namespace GlobalGameJam2024.Simulation.Commands;

/// <summary>
/// Sends a procedure to all connected clients.
/// </summary>
public class BroadcastProcedureCommand : HostCommand
{
	public ClientProcedure Procedure { get; set; }
}
