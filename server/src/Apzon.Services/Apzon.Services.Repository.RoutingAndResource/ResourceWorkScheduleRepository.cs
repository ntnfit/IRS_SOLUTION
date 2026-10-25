using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.RoutingAndResource;

namespace Apzon.Services.Repository.RoutingAndResource;

public class ResourceWorkScheduleRepository : BaseService<DataSet>, IResourceWorkSchedule
{
	public ResourceWorkScheduleRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable SearchData(string code, string status, Enums.NavigateType type)
	{
		try
		{
			return ExecuteDataTable("APZ_ResourceWorkSchedule_SearchData", CommandType.StoredProcedure, new SqlParameter("@code", code), new SqlParameter("@status", status), new SqlParameter("@type", (int)type));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	public DataTable GetDataLine(string code, int year)
	{
		try
		{
			return ExecuteDataTable("APZ_ResourceWorkSchedule_GetDataLine", CommandType.StoredProcedure, new SqlParameter("@code", code), new SqlParameter("@year", year));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	public HttpResult Save(string code, DataSet dtSave, Enums.TransactionType type)
	{
		try
		{
			return Process(ApzObjectTypes.oResourceWorkSchedule, -1, type, dtSave, code);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			return SaveData(document, transtype);
		case Enums.TransactionType.D:
			return DeleteData(objId);
		default:
			return new HttpResult(Enums.MessageCode.Error, "Function not support");
		}
	}

	private HttpResult DeleteData(string code)
	{
		DataTable dataTable = ExecuteDataTable("APZ_ResourceWorkSchedule_DeleteData", CommandType.StoredProcedure, new SqlParameter("@code", code));
		if (!dataTable.IsNotNull())
		{
			return new HttpResult(Enums.MessageCode.Error, $"Can not delete Resource Work Schedule \"{code}\"");
		}
		if (Function.ParseInt(dataTable.Rows[0][0]) > 0)
		{
			return new HttpResult(Enums.MessageCode.Error, $"{Function.ToString(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
		}
		return new HttpResult();
	}

	private HttpResult SaveData(DataSet dtSave, Enums.TransactionType type)
	{
		if (dtSave == null || dtSave.Tables.Count != 2 || !dtSave.Tables[0].IsNotNull())
		{
			return new HttpResult(Enums.MessageCode.Error, "Data could not null");
		}
		DataTable tableStruct = GetTableStruct("APZ_ORWS");
		if (!tableStruct.IsNotNull())
		{
			throw new Exception("Could not get data structure Doc");
		}
		IEnumerable<string> source = from DataColumn t in tableStruct.Columns
			select t.ColumnName;
		DataTable dt = dtSave.Tables[0].DefaultView.ToTable(distinct: false, source.ToArray());
		DataTable tableStruct2 = GetTableStruct("APZ_RWS1");
		if (!tableStruct2.IsNotNull())
		{
			throw new Exception("Could not get data structure Line");
		}
		source = from DataColumn t in tableStruct2.Columns
			select t.ColumnName;
		DataTable dt2 = dtSave.Tables[1].DefaultView.ToTable(distinct: false, source.ToArray());
		string tblName = "#APZ_ORWS_TEMP";
		string tblName2 = "#APZ_RWS1_TEMP";
		CreateTempTable(dt, tblName);
		CreateTempTable(dt2, tblName2);
		BulkCopy(dt, tblName);
		BulkCopy(dt2, tblName2);
		DataTable dataTable = ExecuteDataTable("APZ_ResourceWorkSchedule_Save", CommandType.StoredProcedure, new SqlParameter("@tranType", Function.ToString(type)));
		if (!dataTable.IsNotNull())
		{
			return new HttpResult(Enums.MessageCode.Error, "Can not save data");
		}
		if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][1])))
		{
			return new HttpResult(Enums.MessageCode.Error, $"{Function.ToString(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
		}
		return new HttpResult(Enums.MessageCode.Success, "", Function.ToString(dataTable.Rows[0][0]));
	}
}
