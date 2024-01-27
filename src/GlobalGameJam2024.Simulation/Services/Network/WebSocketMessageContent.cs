using System;
using System.Buffers;
using System.IO;

namespace GlobalGameJam2024.Simulation.Services.Network;

public struct WebSocketMessageContent
{
	internal byte[]? rentedArray;
	internal int offset;
	internal int count;

	internal WebSocketMessageContent(
		byte[]? rentedArray,
		int offset,
		int count)
	{
		this.rentedArray = rentedArray;
		this.offset = offset;
		this.count = count;
	}

	public ArraySegment<byte> Body
	{
		get
		{
			if (rentedArray == null)
			{
				throw new InvalidOperationException("Cannot read the content of the message as it has been returned to the pool.");
			}
			return new ArraySegment<byte>(rentedArray, offset, count);
		}
	}

	public Stream ReadStream()
	{
		if (rentedArray == null)
		{
			throw new InvalidOperationException("Cannot read the content of the message as it has been returned to the pool.");
		}

		return new MemoryStream(rentedArray, offset, count, false, false);
	}

	public void ReturnToPool()
	{
		if (rentedArray != null)
		{
			ArrayPool<byte>.Shared.Return(rentedArray);
		}
	}
}
