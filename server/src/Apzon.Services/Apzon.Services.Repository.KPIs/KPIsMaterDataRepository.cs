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
using Apzon.Services.Interfaces.KPIs;

namespace Apzon.Services.Repository.KPIs;

public class KPIsMaterDataRepository : BaseService<DataTable>, IKPIsMaterData
{
	private const string Doctemp = "#APZ_D_OKPI";

	public KPIsMaterDataRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable LoadData(string code, string name, string status)
	{
		try
		{
			string query = "APZ_D_KpisMasterData_LoadData";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@code", code), new SqlParameter("@name", name), new SqlParameter("@status", status));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadData", ex.Message);
			return new DataTable();
		}
	}

	public HttpResult Saves(DataTable dt)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.A;
			return Process(ApzObjectTypes.oKPIsMasterData, -1, transtype, dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dtSave, ref string objId, ref string objNum)
	{
		HttpResult ret = new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			return SaveData(dtSave, transtype, ref objId, ref objNum);
		case Enums.TransactionType.D:
			return DeleteData(ref ret, ref objId, ref objNum);
		default:
			return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
		}
	}

	public HttpResult SaveData(DataTable dt, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "Data must not null!");
			}
			DataTable tableStruct = GetTableStruct("APZ_D_OKPI");
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			dt = dt.DefaultView.ToTable(distinct: false, source.ToArray());
			CreateTempTable(dt, "#APZ_D_OKPI");
			BulkCopy(dt, "#APZ_D_OKPI");
			DataTable dataTable = ExecuteDataTable("APZ_D_KpisMasterData_Saves", CommandType.StoredProcedure);
			if (dataTable.IsNotNull())
			{
				if (Function.ParseInt(dataTable.Rows[0][0]) == 0)
				{
					return new HttpResult(Enums.MessageCode.Success);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, (tranType == Enums.TransactionType.A) ? "Save error!" : "Update error");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string DeleteDocs(string code)
	{
		try
		{
			return Process(ApzObjectTypes.oKPIsMasterData, -1, Enums.TransactionType.D, new DataTable(), code).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public HttpResult DeleteData(ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			string query = "APZ_D_KpisMasterData_Remove";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@code", objId));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][1])))
				{
					objId = Function.ToString(dataTable.Rows[0][0]);
					objNum = objId;
					return new HttpResult(Enums.MessageCode.Success, "", objId);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable LoadDataFormulaCode()
	{
		try
		{
			string query = "Select * from APZ_D_KPIT";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadData", ex.Message);
			return new DataTable();
		}
	}
}
