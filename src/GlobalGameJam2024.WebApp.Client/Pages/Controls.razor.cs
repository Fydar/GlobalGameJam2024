using GlobalGameJam2024.Simulation.Commands;
using GlobalGameJam2024.WebApp.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Numerics;

namespace GlobalGameJam2024.WebApp.Client.Pages;

public class BoundingClientRect
{
	public float X { get; set; }
	public float Y { get; set; }
	public float Width { get; set; }
	public float Height { get; set; }
	public float Top { get; set; }
	public float Right { get; set; }
	public float Bottom { get; set; }
	public float Left { get; set; }
}

public partial class Controls : ComponentBase, IDisposable
{
	[Inject] protected IClientService? ClientService { get; set; }
	[Inject] protected NavigationManager? NavigationManager { get; set; }
	[Inject] protected IJSRuntime? JsRuntime { get; set; }
	public ElementReference TrackpadElementReference { get; set; }

	protected override async Task OnInitializedAsync()
	{

	}
	public async Task TrackpadInput(MouseEventArgs mouseEventArgs)
	{
		var result = await JsRuntime.InvokeAsync<BoundingClientRect>("MyDOMGetBoundingClientRect", TrackpadElementReference);

		var command = new MoveClientCommand()
		{
			MoveToX = InverseLerp(result.Left, result.Right, (float)mouseEventArgs.ClientX),
			MoveToY = InverseLerp(result.Top, result.Bottom, (float)mouseEventArgs.ClientY),
		};

		_ = ClientService.SendCommandAsync(command);
	}

	public float InverseLerp(float a, float b, float time)
	{
		return (time - a) / (b - a);
	}

	public Vector2 InverseLerp(Vector2 a, Vector2 b, Vector2 time)
	{
		return (time - a) / (b - a);
	}

	public void Dispose()
	{
	}
}
