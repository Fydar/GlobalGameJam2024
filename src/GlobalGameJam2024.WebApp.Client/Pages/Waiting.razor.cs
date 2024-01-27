using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace GlobalGameJam2024.WebApp.Client.Pages;

public partial class Waiting : ComponentBase, IDisposable
{
	[Inject] protected NavigationManager? NavigationManager { get; set; }
	[Inject] protected IJSRuntime? JsRuntime { get; set; }

	protected override async Task OnInitializedAsync()
	{

	}

	public void Dispose()
	{

	}
}
