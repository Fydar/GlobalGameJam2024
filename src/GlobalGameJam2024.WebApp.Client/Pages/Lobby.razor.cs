using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace GlobalGameJam2024.WebApp.Client.Pages;

public partial class Lobby : ComponentBase, IDisposable
{
	[Inject] protected NavigationManager? NavigationManager { get; set; }
	[Inject] protected IJSRuntime? JsRuntime { get; set; }

	protected override async Task OnInitializedAsync()
	{
	}

	public void DirectJoinLobbyButton(MouseEventArgs mouseEventArgs)
	{
	}

	public void Dispose()
	{
	}

	// public void OnProcedureApplied(NetworkedViewProcedure? networkedViewProcedure)
	// {
	// 	_ = InvokeAsync(StateHasChanged);
	// 
	// 	NavigationManager.NavigateTo("/lobby");
	// }
}
