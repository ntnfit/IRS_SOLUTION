using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Sales;

namespace Apzon.Services.Repository.Sales;

public class SalesControlRepository : BaseService<DataTable>, ISalesControl
{
	public SalesControlRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet LoadSalesControl(string userCode, string ownerName, string salesEmp, DateTime fromDate, DateTime toDate, string docStatus, string bpCode, string saleType, string userName, int branch)
	{
		try
		{
			string query = "Apz_SalesControl_LoadSalesQuotation";
			switch (saleType)
			{
			case "sq":
				query = "Apz_SalesControl_LoadSalesQuotation";
				break;
			case "so":
				query = "Apz_SalesControl_LoadSalesOrder";
				break;
			case "sr":
				query = "Apz_SalesControl_LoadSalesReturn";
				break;
			case "sc":
				query = "Apz_SalesControl_LoadSalesContract";
				break;
			case "sp":
				query = "Apz_SalesControl_LoadSalesProduction";
				break;
			}
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(userCode))
			{
				list.Add(new SqlParameter("@userCode", userCode));
			}
			if (!string.IsNullOrEmpty(ownerName))
			{
				list.Add(new SqlParameter("@ownerName", ownerName));
			}
			if (!string.IsNullOrEmpty(salesEmp))
			{
				list.Add(new SqlParameter("@salesEmp", salesEmp));
			}
			list.Add(new SqlParameter("@fromDate", fromDate));
			list.Add(new SqlParameter("@toDate", toDate));
			if (!string.IsNullOrEmpty(docStatus))
			{
				list.Add(new SqlParameter("@docStatus", docStatus));
			}
			if (!string.IsNullOrEmpty(bpCode))
			{
				list.Add(new SqlParameter("@bpCode", bpCode));
			}
			if (!string.IsNullOrEmpty(userName))
			{
				list.Add(new SqlParameter("@userName", userName));
			}
			if (branch != -1)
			{
				list.Add(new SqlParameter("@branch", branch));
			}
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet LoadSalesControl(DataTable dt)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (dt == null || dt.Rows.Count == 0)
			{
				return new DataSet();
			}
			list.Add(new SqlParameter("@userCode", dt.Rows[0]["userCode"]));
			list.Add(new SqlParameter("@ownerName", dt.Rows[0]["ownerName"]));
			list.Add(new SqlParameter("@salesEmp", dt.Rows[0]["salesEmp"]));
			list.Add(new SqlParameter("@fromDate", dt.Rows[0]["fromDate"]));
			list.Add(new SqlParameter("@toDate", dt.Rows[0]["toDate"]));
			list.Add(new SqlParameter("@docStatus", dt.Rows[0]["docStatus"]));
			list.Add(new SqlParameter("@bpCode", dt.Rows[0]["bpCode"]));
			list.Add(new SqlParameter("@userName", dt.Rows[0]["userName"]));
			list.Add(new SqlParameter("@branch", dt.Rows[0]["branch"]));
			string query = "Apz_SalesControl_LoadSalesQuotation";
			switch (Function.ToString(dt.Rows[0]["saleType"]))
			{
			case "sq":
				query = "Apz_SalesControl_LoadSalesQuotation";
				break;
			case "so":
				query = "Apz_SalesControl_LoadSalesOrder";
				break;
			case "sr":
				query = "Apz_SalesControl_LoadSalesReturn";
				break;
			case "sc":
				query = "Apz_SalesControl_LoadSalesContract";
				break;
			case "sp":
				query = "Apz_SalesControl_LoadSalesProduction";
				break;
			}
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public string UpdateDeltimeConfirm(int docentry, int lineid, DateTime deltime, string type, string remark, string username)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@DocEntry", docentry));
			list.Add(new SqlParameter("@LineNum", lineid));
			list.Add(new SqlParameter("@DelTime", deltime));
			list.Add(new SqlParameter("@Type", type));
			list.Add(new SqlParameter("@Remark", remark));
			list.Add(new SqlParameter("@UserName", username));
			if (ExecuteDataTable("Apz_SalesControl_UpdateDeltimeConfirm", CommandType.StoredProcedure, list.ToArray()).IsNotNull())
			{
				return "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "Update False!";
	}

	public DataSetResult GetOrderByBarcode(string barcode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@barcode", barcode)
			};
			return new DataSetResult("", ExecuteData("Apz_SalesOrder_GetOrderByBarcode", CommandType.StoredProcedure, list.ToArray()));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSetResult(ex.Message);
		}
	}
}
