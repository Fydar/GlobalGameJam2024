namespace GlobalGameJam2024.Simulation.Commands;

/// <summary>
/// Sent to the Host to make them aware of the client commands.
/// </summary>
public class IntakeClientCommandHostProcedure : HostProcedure
{
	public LocalId PlayerID { get; set; }
	public ClientCommand Command { get; set; }
}
