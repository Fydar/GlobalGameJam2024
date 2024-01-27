using System;
using System.Net.WebSockets;

namespace GlobalGameJam2024.Simulation.Services.Network;

public class WebSocketDisconnectEvent : IWebSocketEvent
{
	public WebSocketReceiveWorker Channel { get; }
	public DateTimeOffset StartTime { get; }
	public DateTimeOffset EndTime { get; }
	public WebSocketCloseStatus CloseStatus { get; }

	public TimeSpan Elapsed => EndTime - StartTime;

	internal WebSocketDisconnectEvent(
		WebSocketReceiveWorker channel,
		DateTimeOffset startTime,
		DateTimeOffset endTime,
		WebSocketCloseStatus closeStatus)
	{
		Channel = channel;
		StartTime = startTime;
		EndTime = endTime;
		CloseStatus = closeStatus;
	}
}
