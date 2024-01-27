using GlobalGameJam2024.Simulation.Commands;

namespace GlobalGameJam2024.WebApp.Client.Services;

public class LocalClientService : IClientService
{
	private readonly ILogger logger;

	public LocalClientService(
		ILogger<LocalClientService> logger)
	{
		this.logger = logger;
	}

	public Task SendCommandAsync(ClientCommand command, CancellationToken cancellationToken = default)
	{
		return Task.CompletedTask;
	}
}
