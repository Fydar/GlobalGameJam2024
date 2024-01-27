using System;
using System.Net.WebSockets;

namespace GlobalGameJam2024.Simulation.Services.Network;

public sealed class WebSocketExceptionDisconnectEvent : WebSocketDisconnectEvent
{
	public Exception InnerException { get; }

	internal WebSocketExceptionDisconnectEvent(
		WebSocketReceiveWorker channel,
		DateTimeOffset startTime,
		DateTimeOffset endTime,
		WebSocketCloseStatus closeStatus,
		Exception innerException)
		: base(channel, startTime, endTime, closeStatus)
	{
		InnerException = innerException;
	}
}
