using GlobalGameJam2024.Simulation.Commands;

namespace GlobalGameJam2024.Simulation.ClientCommands;

public class JoinLobbyClientCommand : ClientCommand
{
	public string DisplayName { get; set; } = string.Empty;
}
