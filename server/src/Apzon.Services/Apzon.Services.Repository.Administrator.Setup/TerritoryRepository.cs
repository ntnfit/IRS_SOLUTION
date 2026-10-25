using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class TerritoryRepository : BaseService<DataTable>, ITerritory
{
	public TerritoryRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetData()
	{
		try
		{
			return ExecuteDataTable("Select * from OTER;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable Search(string id)
	{
		try
		{
			string query = "APZ_Territory_Search";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@id", id));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string SaveData(DataTable dt)
	{
		try
		{
			return Process(ApzObjectTypes.oTerritories, -1, Enums.TransactionType.U, dt).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dt, ref string objId, ref string objNum)
	{
		HttpResult httpResult = new HttpResult();
		try
		{
			string text = "U";
			if (dt != null && dt.Rows.Count > 0)
			{
				CreateTempTable(dt, "#APZ_OTER");
				BulkCopy(dt, "#APZ_OTER");
				text = "U";
			}
			else
			{
				text = "D";
			}
			DataTable dataTable = ExecuteDataTable("APZ_Process_Territory", CommandType.StoredProcedure, new SqlParameter("@action", text));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				httpResult.Message = Function.ToString(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			httpResult.Message = ex.Message;
		}
		return httpResult;
	}

	public bool ExistsLinkedObject(int id)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_Territory_ExistsLinkedObject", CommandType.StoredProcedure, new SqlParameter("@id", id));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}
}
