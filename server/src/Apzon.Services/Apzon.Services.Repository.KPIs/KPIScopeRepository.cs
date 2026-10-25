using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.KPIs;

namespace Apzon.Services.Repository.KPIs;

internal class KPIScopeRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IKPIScope
{
	private readonly string _connection;

	private string Temp_APZ_OKSE = "#APZ_OKSE";

	private string Temp_APZ_KSE1 = "#APZ_KSE1";

	public DataSet Search(DataTable dt)
	{
		try
		{
			string query = "APZ_KPIScope_Search";
			List<SqlParameter> list = new List<SqlParameter>();
			if (dt != null && dt.Rows.Count > 0)
			{
				list.Add(new SqlParameter("@code", dt.Rows[0]["code"]));
				list.Add(new SqlParameter("@fromDate", dt.Rows[0]["fromDate"]));
				list.Add(new SqlParameter("@toDate", dt.Rows[0]["toDate"]));
				list.Add(new SqlParameter("@active", dt.Rows[0]["active"]));
			}
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetLines(int docentry)
	{
		try
		{
			string query = "APZ_KPIScope_GetLines";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@docEntry", docentry));
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
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
			return Process(ApzObjectTypes.oKPIScope, -1, transtype, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum)
	{
		new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			return SaveData(dsSave, transtype, ref objId, ref objNum);
		case Enums.TransactionType.D:
			return SaveData(dsSave, transtype, ref objId, ref objNum);
		default:
			return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
		}
	}

	public HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			HttpResult httpResult = new HttpResult();
			if (ds == null || (ds.Tables.Count < 2 && tranType != Enums.TransactionType.D) || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				httpResult.Message = "No data to save";
				return httpResult;
			}
			CreateTempTable(ds.Tables["dtDoc"], Temp_APZ_OKSE);
			BulkCopy(ds.Tables["dtDoc"], Temp_APZ_OKSE);
			if (tranType != Enums.TransactionType.D && ds.Tables["dtLine"].IsNotNull())
			{
				CreateTempTable(ds.Tables["dtLine"], Temp_APZ_KSE1);
				BulkCopy(ds.Tables["dtLine"], Temp_APZ_KSE1);
			}
			SqlParameter sqlParameter = new SqlParameter("@type", Function.ToString(tranType));
			DataTable dataTable = ExecuteDataTable("Apz_KPIScope_SaveProcess", CommandType.StoredProcedure, sqlParameter);
			if (dataTable.IsNotNull())
			{
				if (Function.ParseInt(dataTable.Rows[0][0]) == 0)
				{
					return new HttpResult();
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, (tranType == Enums.TransactionType.D) ? "Delete error!" : "Save error");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
