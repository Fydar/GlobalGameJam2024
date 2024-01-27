using GlobalGameJam2024.Simulation.Commands;
using GlobalGameJam2024.WebApp.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Numerics;

namespace GlobalGameJam2024.WebApp.Client.Pages;

public partial class Controls : ComponentBase, IDisposable
{
	[Inject] protected IClientService? ClientService { get; set; }
	[Inject] protected NavigationManager? NavigationManager { get; set; }
	[Inject] protected IJSRuntime? JsRuntime { get; set; }

	protected override async Task OnInitializedAsync()
	{

	}
	public void TrackpadInput(MouseEventArgs mouseEventArgs)
	{
		var command = new MoveClientCommand()
		{
			MoveTo = new Vector2(10, 5)
		};

		_ = ClientService.SendCommandAsync(command);
	}

	public void Dispose()
	{
	}
}
