using GlobalGameJam2024.Simulation.ClientCommands;
using GlobalGameJam2024.WebApp.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using System.ComponentModel.DataAnnotations;

namespace GlobalGameJam2024.WebApp.Client.Pages;

public class User
{
	[MinLength(3)]
	[MaxLength(32)]
	public string DisplayName { get; set; } = RandomName.Generate();
}

public partial class Home : ComponentBase, IDisposable
{
	public User User { get; set; } = new User();

	[Inject] protected IClientService? ClientService { get; set; }
	[Inject] protected NavigationManager? NavigationManager { get; set; }
	[Inject] protected IJSRuntime? JsRuntime { get; set; }

	protected override async Task OnInitializedAsync()
	{

	}

	public void Dispose()
	{

	}

	public void ValidFormSubmitted(EditContext editContext)
	{
		ClientService.SendCommandAsync(new JoinLobbyClientCommand()
		{
			DisplayName = User.DisplayName
		});

		NavigationManager.NavigateTo("/controls");
	}

	public void InvalidFormSubmitted(EditContext editContext)
	{
	}
}
