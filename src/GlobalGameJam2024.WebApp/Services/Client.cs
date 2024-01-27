using GlobalGameJam2024.Simulation;
using GlobalGameJam2024.Simulation.Services.Network;

namespace GlobalGameJam2024.WebApp.Client.Services;

public class Client
{
	public LocalId PlayerId { get; }
	public WebSocketReceiveWorker WebSocketReceiveWorker { get; set; }
}
