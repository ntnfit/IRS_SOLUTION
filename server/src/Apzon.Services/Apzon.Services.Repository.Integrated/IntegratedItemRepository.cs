using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Integrated;

namespace Apzon.Services.Repository.Integrated;

public class IntegratedItemRepository : BaseService<DataTable>, IIntegratedItem
{
	public IntegratedItemRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable Search(DataTable doc)
	{
		DataTable result = new DataTable();
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@IsImported", Function.ToString(doc.Rows[0]["IsImported"]))
			};
			if (!string.IsNullOrWhiteSpace(Function.ToString(doc.Rows[0]["FromDate"])))
			{
				list.Add(new SqlParameter("@FromDate", Function.ToString(doc.Rows[0]["FromDate"])));
			}
			if (!string.IsNullOrWhiteSpace(Function.ToString(doc.Rows[0]["ToDate"])))
			{
				list.Add(new SqlParameter("@ToDate", Function.ToString(doc.Rows[0]["ToDate"])));
			}
			result = ExecuteDataTable("APZ_IntegratedItem_Search", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataSet SearchLine(DataTable dt)
	{
		return new DataSet();
	}

	private string GetAsyncDbName()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_IntegratedUdo_GetAsyncDbName", CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public bool CheckExists(string itemCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("select top 1 1 from OITM where \"ItemCode\"=@itemCode;", CommandType.Text, new SqlParameter("@itemCode", itemCode));
			return dataTable != null && dataTable.Rows.Count > 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return true;
	}

	public DataSet GetDocGridStruct()
	{
		DataSet result = new DataSet();
		try
		{
			result = ExecuteData("APZ_IntegratedItem_GetDocGridStruct", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public int GetManualSeries()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("select \"Series\" from nnm1 where \"ObjectCode\"='4' and \"IsManual\"='Y'", CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}
}
