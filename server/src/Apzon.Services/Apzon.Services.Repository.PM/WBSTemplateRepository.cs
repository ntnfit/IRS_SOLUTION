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
using Apzon.Services.Interfaces.PM;

namespace Apzon.Services.Repository.PM;

public class WBSTemplateRepository : BaseService<DataSet>, IWBSTemplate
{
	private const string docTemp = "#APZ_OWBS";

	private const string taskTemp = "#APZ_TASK";

	private const string wbsdetailTemp = "#APZ_WBS1";

	public WBSTemplateRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable LoadData(string code, string fromDate, string toDate)
	{
		try
		{
			DateTime? dateTime = Function.ParseDateTime(fromDate, "yyyyMMdd");
			DateTime? dateTime2 = Function.ParseDateTime(toDate, "yyyyMMdd");
			string query = "APZ_WBSTemplate_LoadData";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@code", code), new SqlParameter("@fromDate", dateTime), new SqlParameter("@toDate", dateTime2));
			SetDefaultValueToTable("APZ_OWBS", dataTable);
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetDataById(string code)
	{
		try
		{
			return ExecuteData("APZ_WBSTemplate_GetLineById", CommandType.StoredProcedure, new SqlParameter("@code", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public HttpResult Save(DataSet ds, string action)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.A;
			switch (action)
			{
			case "A":
				transtype = Enums.TransactionType.A;
				break;
			case "U":
				transtype = Enums.TransactionType.U;
				break;
			case "D":
				transtype = Enums.TransactionType.D;
				break;
			}
			return Process(ApzObjectTypes.oWBSTemplate, -1, transtype, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum)
	{
		HttpResult ret = new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			return SaveData(dsSave, transtype, ref objId, ref objNum);
		case Enums.TransactionType.D:
			return DeleteData(ref ret, ref objId, ref objNum);
		default:
			return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
		}
	}

	public HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			if (ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "Data must not null!");
			}
			DataTable tableStruct = GetTableStruct("APZ_OWBS");
			DataTable dataTable = ds.Tables[0];
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			dataTable = dataTable.DefaultView.ToTable(distinct: false, source.ToArray());
			CreateTempTable(dataTable, "#APZ_OWBS");
			BulkCopy(dataTable, "#APZ_OWBS");
			DataTable tableStruct2 = GetTableStruct("APZ_TASK");
			DataTable dataTable2 = ds.Tables[1];
			if (!tableStruct2.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source2 = from DataColumn t in tableStruct2.Columns
				select t.ColumnName;
			dataTable2 = dataTable2.DefaultView.ToTable(distinct: false, source2.ToArray());
			CreateTempTable(dataTable2, "#APZ_TASK");
			BulkCopy(dataTable2, "#APZ_TASK");
			DataTable tableStruct3 = GetTableStruct("APZ_WBS1");
			DataTable dataTable3 = ds.Tables[2];
			if (!tableStruct3.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source3 = from DataColumn t in tableStruct3.Columns
				select t.ColumnName;
			dataTable3 = dataTable3.DefaultView.ToTable(distinct: false, source3.ToArray());
			CreateTempTable(dataTable3, "#APZ_WBS1");
			BulkCopy(dataTable3, "#APZ_WBS1");
			string value = Function.ToString(tranType);
			SqlParameter sqlParameter = new SqlParameter("@type", value);
			DataTable dataTable4 = ExecuteDataTable("APZ_WBSTemplate_Save", CommandType.StoredProcedure, sqlParameter);
			if (dataTable4 != null && dataTable4.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable4.Rows[0][1])))
				{
					objId = Function.ToString(dataTable4.Rows[0][0]);
					objNum = objId;
					return new HttpResult(Enums.MessageCode.Success, "", objId);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable4.Rows[0][0])}-{Function.ToString(dataTable4.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, (tranType == Enums.TransactionType.A) ? "Data is not valid" : "Data is not valid");
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
			return Process(ApzObjectTypes.oWBSTemplate, -1, Enums.TransactionType.D, new DataSet(), code).Message;
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
			SqlParameter sqlParameter = new SqlParameter("@code", objId);
			DataTable dataTable = ExecuteDataTable("APZ_WBSTemplate_Remove", CommandType.StoredProcedure, sqlParameter);
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

	public DataTable FindNextLastFrist(int id, int type)
	{
		try
		{
			return ExecuteDataTable("APZ_WBSTemplate_FindNextLastFrist", CommandType.StoredProcedure, new SqlParameter("@id", id), new SqlParameter("@type", type));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}
}
