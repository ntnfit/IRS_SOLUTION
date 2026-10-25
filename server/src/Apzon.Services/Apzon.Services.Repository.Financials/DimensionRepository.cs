using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Financials;

namespace Apzon.Services.Repository.Financials;

public class DimensionRepository : BaseService<IDimension>, IDimension
{
	public DimensionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetData()
	{
		try
		{
			string query = "select * from ODIM order by \"DimCode\";";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetData  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDisableDim()
	{
		try
		{
			string query = "APZ_Dimension_GetDisableDim";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string Save(DataTable dt)
	{
		try
		{
			if (dt == null || dt.Rows.Count != 5)
			{
				return "Dimension data is null (Datatable is null or not have 5 rows)";
			}
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string text = "";
				for (int i = 0; i < 5; i++)
				{
					text = SaveADim(dt.Rows[i]);
					if (!string.IsNullOrEmpty(text))
					{
						break;
					}
				}
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
				dbTransaction.Commit();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return string.Empty;
	}

	private string SaveADim(DataRow row)
	{
		try
		{
			string query = "UPDATE ODIM SET \r\n                                \"DimActive\"=@DimActive\r\n                                , \"DimDesc\"=@DimDesc \r\n                            WHERE \"DimCode\"=@DimCode;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@DimActive", Function.ToString(row["DimActive"])));
			list.Add(new SqlParameter("@DimDesc", Function.ToString(row["DimDesc"])));
			list.Add(new SqlParameter("@DimCode", Function.ParseInt(row["DimCode"])));
			if (ExecuteNonQuery(query, CommandType.Text, list.ToArray()) == 0)
			{
				return "SaveADim: No rows updated";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "";
	}
}
