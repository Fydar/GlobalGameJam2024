using System;

namespace GlobalGameJam2024.Simulation.Services.Network;

/// <summary>
/// A simple structure that formats for bytes.
/// </summary>
public struct MemorySize : IEquatable<MemorySize>, IComparable<MemorySize>, IFormattable
{
	private const ulong denomination = 1000;

	private const ulong kilobyteSize = denomination;
	private const ulong megabyteSize = kilobyteSize * denomination;
	private const ulong gigabyteSize = megabyteSize * denomination;
	private const ulong terrabyteSize = gigabyteSize * denomination;

	private const string byteSuffix = "bytes";
	private const string kilobyteSuffix = "kB";
	private const string megabyteSuffix = "MB";
	private const string gigabyteSuffix = "GB";

	public ulong Bytes { get; set; }

	public double Kilobytes
	{
		get => (double)Bytes / kilobyteSize;
		set => Bytes = (ulong)(value * kilobyteSize);
	}

	public double Megabytes
	{
		get => (double)Bytes / megabyteSize;
		set => Bytes = (ulong)(value * megabyteSize);
	}

	public double Gigabytes
	{
		get => (double)Bytes / gigabyteSize;
		set => Bytes = (ulong)(value * gigabyteSize);
	}

	public double Terrabytes
	{
		get => (double)Bytes / terrabyteSize;
		set => Bytes = (ulong)(value * terrabyteSize);
	}

	public MemorySize(long bytes)
	{
		Bytes = (ulong)bytes;
	}

	public MemorySize(ulong bytes)
	{
		Bytes = bytes;
	}

	public static MemorySize FromBytes(ulong bytes)
	{
		return new MemorySize(bytes);
	}

	public static MemorySize FromKilobytes(double kilobytes)
	{
		return new MemorySize((ulong)(kilobytes * kilobyteSize));
	}

	public static MemorySize FromMegabytes(double megabytes)
	{
		return new MemorySize((ulong)(megabytes * megabyteSize));
	}

	public static MemorySize FromGigabytes(double gigabytes)
	{
		return new MemorySize((ulong)(gigabytes * gigabyteSize));
	}

	public static MemorySize FromTerrabytes(double terrabytes)
	{
		return new MemorySize((ulong)(terrabytes * terrabyteSize));
	}

	public override string ToString()
	{
		if (Gigabytes >= 1.0f)
		{
			return $"{Gigabytes} {gigabyteSuffix}";
		}
		if (Megabytes >= 1.0f)
		{
			return $"{Megabytes} {megabyteSuffix}";
		}
		if (Kilobytes >= 1.0f)
		{
			return $"{Kilobytes} {kilobyteSuffix}";
		}
		return $"{Bytes} {byteSuffix}";
	}

	public string ToString(string format, IFormatProvider formatProvider)
	{
		if (Gigabytes >= 1.0f)
		{
			return $"{Gigabytes.ToString(format, formatProvider)} {gigabyteSuffix}";
		}
		if (Megabytes >= 1.0f)
		{
			return $"{Megabytes.ToString(format, formatProvider)} {megabyteSuffix}";
		}
		if (Kilobytes >= 1.0f)
		{
			return $"{Kilobytes.ToString(format, formatProvider)} {kilobyteSuffix}";
		}
		return $"{Bytes.ToString(format, formatProvider)} {byteSuffix}";
	}

	public override bool Equals(object obj)
	{
		return obj is MemorySize size && Equals(size);
	}

	public bool Equals(MemorySize other)
	{
		return Bytes == other.Bytes;
	}

	public override int GetHashCode()
	{
		return 1182642244 + Bytes.GetHashCode();
	}

	public int CompareTo(MemorySize other)
	{
		return Bytes.CompareTo(other.Bytes);
	}

	public static bool operator ==(MemorySize left, MemorySize right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(MemorySize left, MemorySize right)
	{
		return !(left == right);
	}

	public static MemorySize operator +(MemorySize left, MemorySize right)
	{
		return new MemorySize(left.Bytes + right.Bytes);
	}

	public static MemorySize operator -(MemorySize left, MemorySize right)
	{
		return new MemorySize(left.Bytes + right.Bytes);
	}

	public static MemorySize operator *(MemorySize left, double right)
	{
		return new MemorySize(Convert.ToUInt64(left.Bytes * right));
	}

	public static MemorySize operator /(MemorySize left, double right)
	{
		return new MemorySize(Convert.ToUInt64(left.Bytes / right));
	}
}
