using System.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Numerics;

namespace GlobalGameJam2024.Simulation;

/// <summary>
/// A structure representing an identifier for a resource locally.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public readonly struct LocalId :
	IEquatable<LocalId>,
	IEquatable<ulong>,
	IComparable,
	IComparable<LocalId>,
	IComparable<ulong>
{
	public class Comparer :
		IComparer,
		IComparer<LocalId>
	{
		public static Comparer Instance { get; } = new();

		private Comparer()
		{
		}

		/// <summary>
		/// <para>Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.</para>
		/// </summary>
		/// <param name="x">The first <see cref="LocalId"/> to compare.</param>
		/// <param name="y">The second <see cref="LocalId"/> to compare.</param>
		/// <returns>A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Compare(LocalId x, LocalId y)
		{
			return x.id.CompareTo(y.id);
		}

		/// <summary>
		/// <para>Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.</para>
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns>A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>.</returns>
		/// <exception cref="ArgumentException">When <paramref name="x"/> is not of a type that is comparable to <see cref="LocalId"/>.</exception>
		/// <exception cref="ArgumentException">When <paramref name="y"/> is not of a type that is comparable to <see cref="LocalId"/>.</exception>
		public int Compare(object x, object y)
		{
			var leftValue = x switch
			{
				LocalId value => value,
				ulong value => new LocalId(value),
				_ => throw new ArgumentException("Type is not comparable to LocalId.", nameof(x)),
			};
			var rightValue = x switch
			{
				LocalId value => value,
				ulong value => new LocalId(value),
				_ => throw new ArgumentException("Type is not comparable to LocalId.", nameof(y)),
			};

			return leftValue.id.CompareTo(rightValue.id);
		}
	}

	public class EqualityComparer :
		IEqualityComparer,
		IEqualityComparer<LocalId>
	{
		public static EqualityComparer Instance { get; } = new();

		private EqualityComparer()
		{
		}

		/// <summary>
		/// <para>Determines whether the specified <see cref="LocalId"/> numerics are equal.</para>
		/// </summary>
		/// <param name="x">The first <see cref="LocalId"/> to compare.</param>
		/// <param name="y">The second <see cref="LocalId"/> to compare.</param>
		/// <returns><c>true</c> if the specified objects are equal; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(LocalId x, LocalId y)
		{
			return x.id == y.id;
		}

		/// <summary>
		/// <para>Determines whether the specified objects are equal.</para>
		/// </summary>
		/// <param name="x">The first <see cref="object"/> to compare.</param>
		/// <param name="y">The second <see cref="object"/> to compare.</param>
		/// <returns><c>true</c> if the specified objects are equal; otherwise, <c>false</c>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public new bool Equals(object x, object y)
		{
			if (x == null)
			{
				return y == null;
			}

			return x.Equals(y);
		}

		/// <summary>
		/// <para>Returns a hash code for the specified object.</para>
		/// </summary>
		/// <param name="obj">The <see cref="LocalId"/> for which a hash code is to be returned.</param>
		/// <returns>A hash code for the specified object.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetHashCode(LocalId obj)
		{
			return obj.GetHashCode();
		}

		/// <summary>
		/// <para>Returns a hash code for the specified object.</para>
		/// </summary>
		/// <param name="obj">The <see cref="object"/> for which a hash code is to be returned.</param>
		/// <returns>A hash code for the specified object.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetHashCode(object obj)
		{
			return obj.GetHashCode();
		}
	}


	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private static readonly Random random = new();

	/// <summary>
	/// A blank identifier. Equivalent to <c>'0x000000000'</c>.
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public static LocalId None { get; } = new(0);

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly ulong id;

	public LocalId(ReadOnlySpan<char> id)
	{
		if (id.IsEmpty || id.IsWhiteSpace())
		{
			this.id = 0;
			return;
		}

		if (id.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
		{
			id = id[2..];
		}
		else if (id.StartsWith("x", StringComparison.OrdinalIgnoreCase))
		{
			id = id[1..];
		}
		this.id = ulong.Parse(id, NumberStyles.HexNumber);
	}

	public LocalId(ulong id)
	{
		this.id = id;
	}

	/// <summary>
	/// <para>Returns a value indicating whether this instance is equal to a specified object.</para>
	/// </summary>
	/// <param name="obj">An object to compare with this instance.</param>
	/// <returns><c>true</c> if <paramref name="obj"/> is a comparable type and equals the value of this instance; otherwise, <c>false</c>.</returns>
	public override readonly bool Equals(object obj)
	{
		return obj switch
		{
			LocalId fixedValue => id == fixedValue.id,
			ulong longValue => this == longValue,
			_ => false,
		};
	}

	/// <summary>
	/// <para>Returns a value indicating whether this instance is equal to a specified <see cref="LocalId"/>.</para>
	/// </summary>
	/// <param name="other">A value to compare to this instance.</param>
	/// <returns><c>true</c> if <paramref name="other"/> has the same value as this instance; otherwise, <c>false</c>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(LocalId other)
	{
		return id == other.id;
	}

	/// <summary>
	/// <para>Returns a value indicating whether this instance is equal to a specified <see cref="ulong"/>.</para>
	/// </summary>
	/// <param name="other">A value to compare to this instance.</param>
	/// <returns><c>true</c> if <paramref name="other"/> has the same value as this instance; otherwise, <c>false</c>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(ulong other)
	{
		return id == other;
	}


	/// <summary>
	/// <para>Returns the hash code for this instance.</para>
	/// </summary>
	/// <returns>The hash code.</returns>
	public override int GetHashCode()
	{
		return 2108858624 + id.GetHashCode();
	}

	/// <summary>
	/// <para>Converts the numeric value of this instance to its equivalent string representation using the specified format.</para>
	/// </summary>
	/// <param name="format">A numeric format string.</param>
	/// <returns>The string representation of the value of this instance as specified by format.</returns>
	public override string ToString()
	{
		return "0x" + id.ToString("x8");
	}

	/// <summary>
	/// <para>Compares this instance to a specified object and returns an indication of their relative values.</para>
	/// </summary>
	/// <param name="other">An <see cref="object"/> to compare, or <c>null</c>.</param>
	/// <returns>A signed number indicating the relative values of this instance and value.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly int CompareTo(object other)
	{
		var otherValue = other switch
		{
			LocalId value => value,
			ulong value => new LocalId(value),
			_ => throw new ArgumentException("Type is not comparable to Fixed.", nameof(other)),
		};

		return id.CompareTo(otherValue.id);
	}

	/// <summary>
	/// <para>Compares this instance to a specified <see cref="LocalId"/> and returns an indication of their relative values.</para>
	/// </summary>
	/// <param name="other">A value to compare.</param>
	/// <returns>A signed number indicating the relative values of this instance and value.</returns>
	public int CompareTo(LocalId other)
	{
		return id.CompareTo(other.id);
	}

	/// <summary>
	/// <para>Compares this instance to a specified <see cref="ulong"/> and returns an indication of their relative values.</para>
	/// </summary>
	/// <param name="other">A value to compare.</param>
	/// <returns>A signed number indicating the relative values of this instance and value.</returns>
	public int CompareTo(ulong other)
	{
		return id.CompareTo(other);
	}

	public static LocalId NewId()
	{
		ulong id = 0;
		var idSpan = MemoryMarshal.CreateSpan(ref id, 1);
		var idBytesSpan = MemoryMarshal.AsBytes(idSpan);
		random.NextBytes(idBytesSpan);
		return new LocalId(id);
	}

	public static LocalId NewShortId()
	{
		ulong id = 0;
		var idSpan = MemoryMarshal.CreateSpan(ref id, 1);
		var idBytesSpan = MemoryMarshal.AsBytes(idSpan);
		random.NextBytes(idBytesSpan[..4]);
		return new LocalId(id);
	}

	public static bool operator ==(LocalId left, LocalId right)
	{
		return left.id == right.id;
	}

	public static bool operator !=(LocalId left, LocalId right)
	{
		return left.id != right.id;
	}

	public static bool operator ==(ulong left, LocalId right)
	{
		return left == right.id;
	}

	public static bool operator !=(ulong left, LocalId right)
	{
		return left != right.id;
	}

	public static bool operator ==(LocalId left, ulong right)
	{
		return left.id == right;
	}

	public static bool operator !=(LocalId left, ulong right)
	{
		return left.id != right;
	}

	public static implicit operator LocalId(ulong source)
	{
		return new LocalId(source);
	}

	public static implicit operator ulong(LocalId source)
	{
		return source.id;
	}
}


public class Lobby
{
	public Dictionary<LocalId, Player> Players { get; set; } = [];

	public Simulation? Simulation { get; set; }
}

public class Simulation
{
	public Dictionary<LocalId, IEntity> Entities { get; set; }
	
}

public interface IEntity
{
	public LocalId Identifier { get; set; }
}

public class Player
{
	public LocalId Identifier { get; set; }
}
