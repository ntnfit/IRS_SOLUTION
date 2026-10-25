using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.PlanningProduction;

namespace Apzon.Services.Repository.Production.PlanningProduction;

public class ShiftRepository : BaseService<DataTable>, IShift
{
	private const string TemTable = "#APZTem";

	public ShiftRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetData(int branch)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@branch", branch);
			return ExecuteDataTable("APZ_Shift_GetData", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetColumn()
	{
		try
		{
			return ExecuteDataTable("APZ_Shift_GetColumn", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public HttpResult Save(DataTable dtSave, Enums.TransactionType tranType, string objId)
	{
		try
		{
			return Process(ApzObjectTypes.oShift, -1, tranType, dtSave, objId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dtSave, ref string objId, ref string objNum)
	{
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			objId = "";
			return SaveData(dtSave, transtype);
		case Enums.TransactionType.D:
			return Delete(Function.ParseInt(objId));
		default:
			return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
		}
	}

	public HttpResult SaveData(DataTable dtSave, Enums.TransactionType tranType)
	{
		if (dtSave == null || dtSave.Rows.Count == 0)
		{
			return new HttpResult(Enums.MessageCode.Error, "Data is no procedure");
		}
		SqlParameter sqlParameter = new SqlParameter("@type", tranType.ToString());
		CreateTempTable(dtSave, "#APZTem");
		BulkCopy(dtSave, "#APZTem");
		DataTable dataTable = ExecuteDataTable("APZ_Shift_Save", CommandType.StoredProcedure, sqlParameter);
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

	public HttpResult Delete(int shiftId)
	{
		SqlParameter sqlParameter = new SqlParameter("@shiftCode", shiftId);
		DataTable dataTable = ExecuteDataTable("APZ_Shift_Delete", CommandType.StoredProcedure, sqlParameter);
		if (!dataTable.IsNotNull())
		{
			return new HttpResult(Enums.MessageCode.Error, "Can not delete Shift");
		}
		if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][1])))
		{
			return new HttpResult(Enums.MessageCode.Success);
		}
		return new HttpResult(Enums.MessageCode.Error, $"Delete error. {Function.ToString(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
	}
}
