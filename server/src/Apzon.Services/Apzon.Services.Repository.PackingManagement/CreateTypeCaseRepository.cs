using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.PackingManagement;

namespace Apzon.Services.Repository.PackingManagement;

public class CreateTypeCaseRepository : BaseService<DataSet>, ICreateTypeCase
{
	private const string TableTempDoc = "#DMTCT";

	public CreateTypeCaseRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetTypeCase()
	{
		try
		{
			return ExecuteDataTable("APZ_PackingManagement_GetTypeCase", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string DelTC(string TypeCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@TypeCode", TypeCode));
			DataTable dataTable = ExecuteDataTable("APZ_PackingManagement_DelTC", CommandType.StoredProcedure, list.ToArray());
			if (dataTable.IsNotNull() && !string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][0])))
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

	public DataTable TypeCaseGetDataDoc(Enums.NavigateType type, string TypeCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@type", type));
			list.Add(new SqlParameter("@TypeCode", TypeCode));
			return ExecuteDataTable("APZ_PackingManagement_TypeCaseGetDataDoc", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public HttpResult SaveTCDataSet(DataSet ds, string action)
	{
		try
		{
			Enums.TransactionType transactionType = Enums.TransactionType.A;
			switch (action)
			{
			case "A":
				transactionType = Enums.TransactionType.A;
				break;
			case "U":
				transactionType = Enums.TransactionType.U;
				break;
			case "D":
				transactionType = Enums.TransactionType.D;
				break;
			default:
				return new HttpResult
				{
					Message = "Repository: Wrong action. Should be: 'A', 'U' OR 'D'"
				};
			}
			return Process(ApzObjectTypes.oNone, -1, transactionType, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum)
	{
		HttpResult ret = new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
			ProcessSave(ref transtype, ds, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.U:
			ProcessSave(ref transtype, ds, ref objId, ref objNum, ref ret);
			break;
		}
		return ret;
	}

	private void ProcessSave(ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum, ref HttpResult ret)
	{
		try
		{
			if (ds == null || ds.Tables.Count < 1 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				ret.Message = "No data to save";
				return;
			}
			string value = ((transtype == Enums.TransactionType.A) ? "A" : "U");
			CreateTempTable(ds.Tables["dtCase"], "#DMTCT");
			BulkCopy(ds.Tables["dtCase"], "#DMTCT");
			SqlParameter sqlParameter = new SqlParameter("@Type", value);
			DataTable dataTable = ExecuteDataTable("APZ_PackingManagement_UpdateNewTypeCase", CommandType.StoredProcedure, sqlParameter);
			if (dataTable != null && dataTable.Rows.Count > 0 && (dataTable.Columns.Contains("ErrorMessage") || dataTable.Columns.Contains("ErrorNumber")))
			{
				ret.Message = Function.ToString(dataTable.Rows[0]["ErrorMessage"]);
				string text = Function.ToString(dataTable.Rows[0]["ErrorNumber"]);
				ret.Message = text + " - " + ret.Message;
				objId = "";
				objNum = "";
				ret.Message = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}

	public DataTable Search(string TypeCode, string TypeName, DateTime CreateDate)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@TypeCode", TypeCode));
			list.Add(new SqlParameter("@TypeName", TypeName));
			list.Add(new SqlParameter("@CreateDate", CreateDate));
			return ExecuteDataTable("APZ_PackingManagement_SearchTypeCase", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
