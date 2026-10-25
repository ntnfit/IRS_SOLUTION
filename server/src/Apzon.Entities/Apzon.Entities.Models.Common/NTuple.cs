using System;
using System.Collections.Generic;
using System.Linq;

namespace Apzon.Entities.Models.Common;

public class NTuple<T> : IEquatable<NTuple<T>> where T : class
{
	public readonly T[] Values;

	public NTuple(IEnumerable<T> values)
	{
		Values = values.ToArray();
	}

	public override bool Equals(object obj)
	{
		if (this == obj)
		{
			return true;
		}
		if (obj == null)
		{
			return false;
		}
		return Equals(obj as NTuple<T>);
	}

	public bool Equals(NTuple<T> other)
	{
		if (this == other)
		{
			return true;
		}
		if (other == null)
		{
			return false;
		}
		int num = Values.Length;
		if (num != other.Values.Length)
		{
			return false;
		}
		for (int i = 0; i < num; i++)
		{
			if (!object.Equals(Values[i], other.Values[i]))
			{
				return false;
			}
		}
		return true;
	}

	public override int GetHashCode()
	{
		int num = 17;
		T[] values = Values;
		for (int i = 0; i < values.Length; i++)
		{
			num = num * 37 + (values[i]?.GetHashCode() ?? 0);
		}
		return num;
	}
}
