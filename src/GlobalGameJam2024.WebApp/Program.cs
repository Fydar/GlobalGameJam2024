using GlobalGameJam2024.WebApp.Client.Services;
using GlobalGameJam2024.WebApp.Components;
using GlobalGameJam2024.WebApp.Utility;
using LettuceEncrypt;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Serilog;
using Serilog.Events;
using System.Net;

namespace GlobalGameJam2024.WebApp;

public class Program
{
	public static int Main(string[] args)
	{
		var loggerConfiguration = new LoggerConfiguration()
			.MinimumLevel.Information()
			.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
			.MinimumLevel.Override("Microsoft.AspNetCore.Server.Kestrel", LogEventLevel.Error)
			.Enrich.FromLogContext()
			.WriteTo.Sink(new ColoredConsoleSink());

		var logger = loggerConfiguration.CreateLogger();
		Log.Logger = logger;

		try
		{
			var host = CreateWebApplication(args);
			host.Start();

			var serverFeatures = host.Services.GetRequiredService<IServer>().Features;
			var addresses = serverFeatures.Get<IServerAddressesFeature>().Addresses;

			Log.Information($"Web host started listening on {string.Join(", ", addresses)}");

			host.WaitForShutdown();

			return 0;
		}
		catch (Exception exception)
		{
			Log.Fatal(exception, "Host terminated unexpectedly");
			return 1;
		}
		finally
		{
			Log.CloseAndFlush();
		}
	}

	public static WebApplication CreateWebApplication(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		builder.Configuration.AddCommandLine(args);
		builder.Configuration.AddEnvironmentVariables("CONFIG_");

		builder.WebHost.UseSetting(WebHostDefaults.SuppressStatusMessagesKey, "True");

		if (builder.WebHost.GetSetting("Environment") != "Development")
		{
			builder.WebHost.UseKestrel(kestral =>
			{
				var appServices = kestral.ApplicationServices;

				kestral.Listen(IPAddress.Any, 8060);

				kestral.Listen(
					IPAddress.Any, 8061,
					listen => listen.UseHttps(adapter =>
					{
						adapter.UseLettuceEncrypt(appServices);
					}));
			});

			string domainName = builder.Configuration.GetValue<string>("DOMAINNAME") ?? string.Empty;

			builder.Services.AddLettuceEncrypt(options =>
			{
				options.AcceptTermsOfService = true;
				options.DomainNames = [domainName];
				options.EmailAddress = "dev.anthonymarmont@gmail.com";
			})
				.PersistDataToDirectory(new DirectoryInfo("lettuce"), null);
		}

		builder.Host.UseSerilog();

		builder.Services.AddSingleton<IClientService, LocalClientService>();
		builder.Services.AddSingleton<LobbyService>();

		builder.Services.AddHealthChecks();
		builder.Services.AddControllers();

		// Add services to the container.
		builder.Services.AddRazorComponents()
			.AddInteractiveWebAssemblyComponents();

		var app = builder.Build();

		app.UseHealthChecks("/api/health");

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
			app.UseWebAssemblyDebugging();
		}
		else
		{
			app.UseExceptionHandler("/Error");
			app.UseHsts();
		}

		app.UseHttpsRedirection();

		app.UseStaticFiles();
		app.UseAntiforgery();
		app.UseWebSockets();
		app.MapControllers();
		app.MapRazorComponents<App>()
			.AddInteractiveWebAssemblyRenderMode()
			.AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

		return app;
	}
}
