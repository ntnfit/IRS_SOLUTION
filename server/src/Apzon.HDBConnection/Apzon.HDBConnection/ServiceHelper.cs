using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Npgsql;
using Sap.Data.Hana;

namespace Apzon.HDBConnection;

public static class ServiceHelper
{
	public static void TryAddParameters(this SqlCommand command, params SqlParameter[] parameters)
	{
		if (command == null)
		{
			throw new ArgumentNullException("command");
		}
		if (parameters != null && parameters.Length != 0)
		{
			command.Parameters.AddRange(parameters);
		}
	}

	public static void TryAddParameters(this HanaCommand command, params HanaParameter[] parameters)
	{
		if (command == null)
		{
			throw new ArgumentNullException("command");
		}
		if (parameters != null && parameters.Length != 0)
		{
			command.Parameters.AddRange(parameters);
		}
	}

	public static void TryAddParameters(this NpgsqlCommand command, params NpgsqlParameter[] parameters)
	{
		if (command == null)
		{
			throw new ArgumentNullException("command");
		}
		if (parameters != null && parameters.Length != 0)
		{
			command.Parameters.AddRange(parameters);
		}
	}

	public static List<TEntity> Select<TEntity>(this IDataReader reader, Func<IDataRecord, TEntity> generator)
	{
		List<TEntity> list = new List<TEntity>();
		while (reader.Read())
		{
			list.Add(generator(reader));
		}
		return list;
	}

	public static IEnumerable<TEntity> GetEnumerator<TEntity>(this IDataReader reader, Func<IDataRecord, TEntity> generator)
	{
		while (reader.Read())
		{
			yield return generator(reader);
		}
	}

	internal static double ParseDouble(this object obj)
	{
		try
		{
			if (obj == null)
			{
				return 0.0;
			}
			if (double.TryParse(obj.ToString(), out var result) && !double.IsInfinity(result) && !double.IsNaN(result))
			{
				return result;
			}
			return 0.0;
		}
		catch (Exception)
		{
			return 0.0;
		}
	}

	internal static int ParseInt(this object obj)
	{
		try
		{
			if (obj == null)
			{
				return 0;
			}
			if (int.TryParse(obj.ToString(), out var result))
			{
				return result;
			}
			return 0;
		}
		catch (Exception)
		{
			return 0;
		}
	}

	internal static string To_String(this object obj)
	{
		try
		{
			if (obj == null)
			{
				return string.Empty;
			}
			return obj.ToString().Trim();
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	internal static decimal ParseDecimal(this object obj)
	{
		try
		{
			if (obj == null)
			{
				return 0m;
			}
			string s = obj.ToString();
			if (decimal.TryParse(s, out var result))
			{
				return result;
			}
			if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			return 0m;
		}
		catch (Exception)
		{
			return 0m;
		}
	}

	internal static DateTime? ParseDateTime(this object obj)
	{
		try
		{
			if (obj == null)
			{
				return null;
			}
			if (DateTime.TryParse(obj.ToString(), out var result))
			{
				return result;
			}
			return null;
		}
		catch (Exception)
		{
			return null;
		}
	}

	internal static double ParseDouble(this object obj, NumberFormatInfo provider)
	{
		try
		{
			if (obj == null)
			{
				return 0.0;
			}
			if (double.TryParse(obj.ToString(), NumberStyles.Any, provider, out var result) && !double.IsInfinity(result) && !double.IsNaN(result))
			{
				return result;
			}
			return 0.0;
		}
		catch (Exception)
		{
			return 0.0;
		}
	}

	internal static bool ParseBool(this object obj)
	{
		try
		{
			if (obj == null)
			{
				return false;
			}
			if (obj.GetType() != typeof(bool))
			{
				return Convert.ToBoolean(obj);
			}
			if (bool.TryParse(obj.ToString(), out var result))
			{
				return result;
			}
			return false;
		}
		catch (Exception)
		{
			return false;
		}
	}
}
