namespace GlobalGameJam2024.Simulation.Commands;

/// <summary>
/// Sends a procedure to a specific connected clients.
/// </summary>
public class SendProcedureCommand : HostCommand
{
	public LocalId PlayerID { get; set; }
	public ClientProcedure Procedure { get; set; }
}
