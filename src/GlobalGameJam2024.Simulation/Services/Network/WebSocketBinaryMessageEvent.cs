using System;

namespace GlobalGameJam2024.Simulation.Services.Network;

public sealed class WebSocketBinaryMessageEvent : IWebSocketEvent
{
	public WebSocketReceiveWorker Channel { get; }
	public DateTimeOffset StartTime { get; }
	public DateTimeOffset EndTime { get; }
	public WebSocketMessageContent MessageContent { get; }

	public TimeSpan Elapsed => EndTime - StartTime;

	internal WebSocketBinaryMessageEvent(
		WebSocketReceiveWorker channel,
		DateTimeOffset startTime,
		DateTimeOffset endTime,
		WebSocketMessageContent messageContent)
	{
		Channel = channel;
		StartTime = startTime;
		EndTime = endTime;
		MessageContent = messageContent;
	}
}
