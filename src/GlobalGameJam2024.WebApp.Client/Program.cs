using GlobalGameJam2024.WebApp.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace GlobalGameJam2024.WebApp.Client;

internal class Program
{
	private static async Task Main(string[] args)
	{
		var builder = WebAssemblyHostBuilder.CreateDefault(args);

		builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

		builder.Services.AddSingleton<IClientService, ClientService>();

		await builder.Build().RunAsync();
	}
}
