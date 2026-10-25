using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Opportunity;

namespace Apzon.Services.Repository.Opportunity;

public class InterestRepository : BaseService<DataTable>, IInterest
{
	public InterestRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDocs()
	{
		try
		{
			return ExecuteDataTable("Select * From OOIN;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string SaveDoc(DataTable dt)
	{
		try
		{
			return Process(ApzObjectTypes.oInterestLevel, -1, Enums.TransactionType.A, dt).Message;
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
			httpResult.Message = ProcessChildData(dt, "APZ_Interest_SaveDoc", "#OOIN");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			httpResult.Message = ex.Message;
		}
		return httpResult;
	}

	private string ProcessChildData(DataTable dt, string store, string tempTable)
	{
		try
		{
			CreateTempTable(dt, tempTable);
			BulkCopy(dt, tempTable);
			DataTable dataTable = ExecuteDataTable(store, CommandType.StoredProcedure);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string DeleteDoc(int num)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = "APZ_Interest_DeleteDoc";
				DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@num", num));
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					return Function.ToString(dataTable.Rows[0][0]);
				}
				dbTransaction.Commit();
				return "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
