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

public class PaymentRequestRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IPaymentRequest
{
	private string Temp_Doc = "#MKS_OPMR";

	public DataTable GetDoc()
	{
		try
		{
			return ExecuteDataTable("MKS_PaymentRequest_GetDoc", CommandType.StoredProcedure);
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
				string value3 = Function.ToString(dtSearch.Rows[0]["cardCode"]);
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
				if (!string.IsNullOrEmpty(value3))
				{
					list.Add(new SqlParameter("@CardCode", value3));
				}
				list.Add(new SqlParameter("@DocNum", value2));
				return ExecuteDataTable("MKS_PaymentRequest_Search", CommandType.StoredProcedure, list.ToArray());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Search  FAILED ", ex.Message);
		}
		return new DataTable();
	}

	public string Approval(DataTable dt)
	{
		int num = Function.ParseInt(dt.Rows[0]["DocEntry"]);
		string text = Function.ToString(dt.Rows[0]["WddStatus"]);
		string text2 = Function.ToString(dt.Rows[0]["Note"]);
		if (text2.Length > 300)
		{
			text2 = text2.Substring(0, 300);
		}
		if (num == 0)
		{
			return "Invalid request Id";
		}
		if (string.IsNullOrEmpty(text) || text == "-")
		{
			return "Invalid approval status";
		}
		string query = "UPDATE OPDF Set WddStatus=@Status,U_BaseReqNote=@Note WHERE U_BaseReqEntry=@DocEntry";
		ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@Status", text), new SqlParameter("@DocEntry", num), new SqlParameter("@Note", text2 ?? ""));
		return "";
	}

	public string Save(DataSet ds)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.A;
			return Process(ApzObjectTypes.oPaymentRequest, -1, transtype, ds).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string UpdateStatus(DataSet ds)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.U;
			return Process(ApzObjectTypes.oPaymentRequest, -1, transtype, ds).Message;
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
			return Process(ApzObjectTypes.oPaymentRequest, -1, Enums.TransactionType.D, new DataSet(), Function.ToString(docNo)).Message;
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
				CreateTempTable(dtS.Tables[0], Temp_Doc);
				BulkCopy(dtS.Tables[0], Temp_Doc);
				string text = "";
				text = ((transtype != Enums.TransactionType.A) ? "MKS_PaymentRequest_UpdateStatus" : "MKS_PaymentRequest_Save");
				DataTable dataTable = ExecuteDataTable(text, CommandType.StoredProcedure, (SqlParameter[])null);
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					ret.Message = Function.ToString(dataTable.Rows[0]["ErrorMessage"]);
					string text2 = Function.ToString(dataTable.Rows[0]["ErrorNumber"]);
					ret.Message = text2 + " - " + ret.Message;
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
			int num = ExecuteNonQuery("apz_PaymentRequest_Remove", CommandType.StoredProcedure, list.ToArray());
			ret.Message = ((num > 0) ? "" : "Delete error! ");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}
}
