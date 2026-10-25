using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Apzon.Commons.Helper;

public static class DataTableExtension
{
	public static bool IsExistsColumn(this DataRow dr, string colName)
	{
		try
		{
			return dr?.Table.Columns.Contains(colName) ?? false;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool IsExistsColumn(this DataTable dt, string colName)
	{
		try
		{
			if (dt != null && dt.Columns.Contains(colName))
			{
				return true;
			}
			return false;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool IsNotNull(this DataTable dt)
	{
		try
		{
			return dt != null && dt.Rows.Count > 0;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool EmptyData(this DataTable dt)
	{
		return !dt.IsNotNull();
	}

	public static bool HasColumn(this DataTable dt)
	{
		try
		{
			if (dt == null)
			{
				return false;
			}
			if (dt.Columns.Count == 0)
			{
				return false;
			}
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool IsNotNull(this DataRow[] dr)
	{
		try
		{
			return dr != null && dr.Length != 0;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool IsNotNull(this DataSet ds)
	{
		try
		{
			return ds != null && ds.Tables.Count > 0;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool IsHasStructure(this DataSet ds)
	{
		try
		{
			return ds != null && ds.Tables.Count > 1;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static bool IsHasValidValue(this DataSet ds)
	{
		try
		{
			return ds != null && ds.Tables.Count > 2;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static void DisposeDataSet(this DataSet ds)
	{
		try
		{
			ds.Tables.Clear();
			ds.Dispose();
		}
		catch (Exception)
		{
		}
	}

	public static bool ExistsColumn(this DataTable dt, string colName)
	{
		try
		{
			return dt?.Columns.Contains(colName) ?? false;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static DataRow FindByKey(this DataTable dt, string[] colKey, params object[] obj)
	{
		if (colKey != null && colKey.Length != 0)
		{
			dt.PrimaryKey = colKey.Select((string t) => dt.Columns[t]).ToArray();
			List<object> list = new List<object>();
			list.AddRange(obj);
			return dt.Rows.Find(list.ToArray());
		}
		return null;
	}

	public static void SetPrimaryKey(this DataTable dt, params string[] colKey)
	{
		try
		{
			dt.PrimaryKey = (from t in colKey
				where dt.Columns.Contains(t)
				select dt.Columns[t]).ToArray();
		}
		catch (Exception)
		{
		}
	}

	public static DataRow[] ToArray(this DataRow dr)
	{
		return new DataRow[1] { dr };
	}

	public static DataColumn[] ToArray(this DataColumn dr)
	{
		return new DataColumn[1] { dr };
	}
}
