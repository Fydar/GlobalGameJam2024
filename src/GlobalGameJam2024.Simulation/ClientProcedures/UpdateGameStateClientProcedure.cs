using GlobalGameJam2024.Simulation.Commands;

namespace GlobalGameJam2024.Simulation.Procedures;

public class UpdateGameStateClientProcedure : ClientProcedure
{
	public bool IsPlaying { get; set; } = false;
	public int BossHealth { get; set; } = 10000;
	public GameStatePlayer[] Players { get; set; } = [];
}

public class GameStatePlayer
{
	public LocalId PlayerID { get; set; }
	public string DisplayName { get; set; } = "Name";
	public string Job { get; set; } = "archer";
	public int Health { get; set; } = 100;
	public float Cooldown { get; set; } = 1.0f;
	public float PositionX { get; set; } = 0.5f;
	public float PositionY { get; set; } = 0.5f;

	public bool HasAbility => Cooldown > 0.9825f;
	public bool IsDead => Health <= 0;
}
