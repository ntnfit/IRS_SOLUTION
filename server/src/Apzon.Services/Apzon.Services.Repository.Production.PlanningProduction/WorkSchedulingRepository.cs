using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.PlanningProduction;

namespace Apzon.Services.Repository.Production.PlanningProduction;

public class WorkSchedulingRepository : BaseService<DataSet>, IWorkScheduling
{
	private const string TableDoc = "#TempDoc";

	private const string TableLine = "#TempLine";

	public WorkSchedulingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDataDoc(int branch)
	{
		try
		{
			return ExecuteDataTable("APZ_WorkScheduling_GetDataDoc", CommandType.StoredProcedure, new SqlParameter("@branch", branch));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetDataLine(int docId)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@docId", docId);
			return ExecuteDataTable("APZ_WorkScheduling_GetDataLine", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadShiftData()
	{
		try
		{
			return ExecuteDataTable("APZ_WorkScheduling_GetAllShiftData", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult Save(DataSet dsSave, Enums.TransactionType tranType, string objId)
	{
		try
		{
			return Process(ApzObjectTypes.oWorkSchedule, -1, tranType, dsSave, objId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum)
	{
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			return SaveData(dsSave, transtype);
		case Enums.TransactionType.D:
			return Delete(Function.ParseInt(objId));
		default:
			return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
		}
	}

	public HttpResult SaveData(DataSet dsSave, Enums.TransactionType tranType)
	{
		if (dsSave == null || dsSave.Tables.Count < 2 || dsSave.Tables[0] == null || dsSave.Tables[0].Rows.Count == 0 || dsSave.Tables[1] == null || dsSave.Tables[1].Rows.Count == 0)
		{
			return new HttpResult(Enums.MessageCode.Error, "Data could not be null");
		}
		CreateTempTable(dsSave.Tables[0], "#TempDoc");
		BulkCopy(dsSave.Tables[0], "#TempDoc");
		CreateTempTable(dsSave.Tables[1], "#TempLine");
		BulkCopy(dsSave.Tables[1], "#TempLine");
		DataTable dataTable = ExecuteDataTable("APZ_WorkScheduling_Save", CommandType.StoredProcedure, new SqlParameter("@type", tranType.ToString()));
		if (dataTable != null && dataTable.Rows.Count > 0)
		{
			if (Function.ParseInt(dataTable.Rows[0][0]) > 0)
			{
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Success);
		}
		return new HttpResult(Enums.MessageCode.Error, (tranType == Enums.TransactionType.A) ? "Save error" : "Update error");
	}

	public HttpResult Delete(int docId)
	{
		SqlParameter sqlParameter = new SqlParameter("@docId", docId);
		if (ExecuteNonQuery("APZ_WorkScheduling_Delete", CommandType.StoredProcedure, sqlParameter) > 0)
		{
			return new HttpResult();
		}
		return new HttpResult(Enums.MessageCode.Error, "Delete error");
	}

	public DataTable GetColumnDoc()
	{
		try
		{
			return ExecuteDataTable("APZ_WorkScheduling_GetColumnDoc", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetColumnLine()
	{
		try
		{
			return ExecuteDataTable("APZ_WorkScheduling_GetColumnLine", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
