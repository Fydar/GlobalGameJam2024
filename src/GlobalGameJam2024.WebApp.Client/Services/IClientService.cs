using GlobalGameJam2024.Simulation.Commands;

namespace GlobalGameJam2024.WebApp.Client.Services;
public interface IClientService
{
	Task SendCommandAsync(ClientCommand command, CancellationToken cancellationToken = default);
}