using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Payment;

namespace Apzon.Services.Repository.Payment;

public class OutgoingPaymentsRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IOutgoingPayments
{
	private string Temp_Line = "#APZ_VPM1";

	private string Temp_Doc = "#APZ_OVPM";

	public DataTable GetDoc()
	{
		try
		{
			return ExecuteDataTable("APZ_OutgoingPayments_GetDoc", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDoc  FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable Search(DataTable dtSearch)
	{
		try
		{
			if (dtSearch.IsNotNull())
			{
				DateTime dateTime = Function.ParseDateTimes(dtSearch.Rows[0]["fromDate"]);
				DateTime dateTime2 = Function.ParseDateTimes(dtSearch.Rows[0]["toDate"]);
				string value = Function.ToString(dtSearch.Rows[0]["docStatus"]);
				string value2 = Function.ToString(dtSearch.Rows[0]["docNum"]);
				List<SqlParameter> list = new List<SqlParameter>();
				if (!string.IsNullOrEmpty(Function.ToString(dtSearch.Rows[0]["fromDate"])))
				{
					list.Add(new SqlParameter("@DocDate", dateTime));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dtSearch.Rows[0]["toDate"])))
				{
					list.Add(new SqlParameter("@EndDate", dateTime2));
				}
				if (!string.IsNullOrEmpty(value))
				{
					list.Add(new SqlParameter("@DocStatus", value));
				}
				list.Add(new SqlParameter("@DocNum", value2));
				return ExecuteDataTable("APZ_OutgoingPayments_Search", CommandType.StoredProcedure, list.ToArray());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Search  FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetLines(int docentry)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@DocEntry", docentry);
			return ExecuteDataTable("APZ_OutgoingPayments_GetLine", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetLines  FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public string Save(DataSet ds)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.U;
			return Process(ApzObjectTypes.oAdvancedInPayment, -1, transtype, ds).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string Remove(int docNo)
	{
		try
		{
			return Process(ApzObjectTypes.oAdvancedInPayment, -1, Enums.TransactionType.D, new DataSet(), Function.ToString(docNo)).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
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
		case Enums.TransactionType.D:
			ProcessDelete(ref ret, ref objId, ref objNum);
			break;
		}
		return ret;
	}

	private void ProcessSave(ref Enums.TransactionType transtype, DataSet dtS, ref string objId, ref string objNum, ref HttpResult ret)
	{
		try
		{
			if (!dtS.IsNotNull())
			{
				ret.Message = "No data to save";
			}
			else
			{
				_ = transtype;
				DataTable dt = dtS.Tables["dtLine"];
				CreateTempTable(dt, Temp_Line);
				BulkCopy(dt, Temp_Line);
				CreateTempTable(dtS.Tables["dtDoc"], Temp_Doc);
				BulkCopy(dtS.Tables["dtDoc"], Temp_Doc);
				DataTable dataTable = ExecuteDataTable("APZ_OutgoingPayments_Save", CommandType.StoredProcedure, (SqlParameter[])null);
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					ret.Message = Function.ToString(dataTable.Rows[0]["ErrorMessage"]);
					string text = Function.ToString(dataTable.Rows[0]["ErrorNumber"]);
					ret.Message = text + " - " + ret.Message;
				}
				else
				{
					ret.Message = "";
				}
			}
			if (!string.IsNullOrEmpty(ret.Message))
			{
				objId = "";
				objNum = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}

	private void ProcessDelete(ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@DocNum", Function.ParseInt(objId)));
			int num = ExecuteNonQuery("apz_OutgoingPayments_Remove", CommandType.StoredProcedure, list.ToArray());
			ret.Message = ((num > 0) ? "" : "Delete error! ");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}
}
