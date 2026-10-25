using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.KPIs;

namespace Apzon.Services.Repository.KPIs;

public class ListPointKPIsRepository : BaseService<DataTable>, IListPointKPIs
{
	private const string tempDoc = "#APZ_D_OKPR";

	private const string tempLine = "#APZ_D_KPR1";

	public ListPointKPIsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetListPoint(string setCode)
	{
		try
		{
			new DataTable();
			string query = "APZ_D_KPIs_GetListPoint";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter
				{
					ParameterName = "@code",
					Value = Function.ToString(setCode)
				}
			};
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	public DataTable GetDetailScoring(int absEntry, string setCode, DateTime period)
	{
		try
		{
			new DataTable();
			string query = "APZ_D_KPIs_GetDetailScoring";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter
				{
					ParameterName = "@setCode",
					Value = setCode
				},
				new SqlParameter
				{
					ParameterName = "@period",
					Value = period.ToString("yyyy-MM-dd")
				},
				new SqlParameter
				{
					ParameterName = "@absEntry",
					Value = absEntry
				}
			};
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	public HttpResult SaveData(DataTable data)
	{
		try
		{
			return Process(ApzObjectTypes.oKPIsListPoint, -1, Enums.TransactionType.U, data);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult
			{
				Message = ex.ToString()
			};
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable data, ref string objId, ref string objNum)
	{
		HttpResult httpResult = new HttpResult();
		string message = "";
		switch (transtype)
		{
		case Enums.TransactionType.U:
			message = Upadate(objtype, bplId, ref transtype, data, ref objId, ref objNum).Message;
			break;
		}
		httpResult.Message = message;
		return httpResult;
	}

	public HttpResult Upadate(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable data, ref string objId, ref string objNum)
	{
		try
		{
			new HttpResult();
			CreateTempTable(data, "#APZ_D_KPR1");
			BulkCopy(data, "#APZ_D_KPR1");
			string query = "APZ_D_KPIs_SaveScoringDetail";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure);
			string message = Function.ToString(dataTable.Rows[0]["Message"]);
			objId = "";
			objId = Function.ToString(dataTable.Rows[0]["AbsEntry"]);
			return new HttpResult(Enums.MessageCode.Success, message);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
