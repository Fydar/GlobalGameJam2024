using System;

namespace GlobalGameJam2024.Simulation.Services.Network;

public interface IWebSocketEvent
{
	public WebSocketReceiveWorker Channel { get; }

	public DateTimeOffset StartTime { get; }

	public DateTimeOffset EndTime { get; }

	public TimeSpan Elapsed { get; }
}
