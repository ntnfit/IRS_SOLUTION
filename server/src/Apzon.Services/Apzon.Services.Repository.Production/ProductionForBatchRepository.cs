using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Production;
using Apzon.Services.Interfaces.Production;

namespace Apzon.Services.Repository.Production;

public class ProductionForBatchRepository : BaseService<ProductionOder_Impl_OWOR>, IProductionForBatch
{
	public ProductionForBatchRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet Search(DataTable dt)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (dt != null && dt.Rows.Count > 0)
			{
				list.Add(new SqlParameter("@SoNo", dt.Rows[0]["so"]));
				list.Add(new SqlParameter("@ItemCode", dt.Rows[0]["itemcode"]));
				list.Add(new SqlParameter("@Slp", dt.Rows[0]["slp"]));
				list.Add(new SqlParameter("@CardCode", dt.Rows[0]["cardcode"]));
				list.Add(new SqlParameter("@branch", dt.Rows[0]["branch"]));
				list.Add(new SqlParameter("@FromDate", dt.Rows[0]["fromdate"]));
				list.Add(new SqlParameter("@Todate", dt.Rows[0]["todate"]));
				list.Add(new SqlParameter("@Whs", dt.Rows[0]["whs"]));
			}
			return ExecuteData("apz_productionforbatch_search", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet SearchPlanning(DataTable dt)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (dt != null && dt.Rows.Count > 0)
			{
				list.Add(new SqlParameter("@SoNo", dt.Rows[0]["so"]));
				list.Add(new SqlParameter("@ItemCode", dt.Rows[0]["itemcode"]));
				list.Add(new SqlParameter("@Slp", dt.Rows[0]["slp"]));
				list.Add(new SqlParameter("@CardCode", dt.Rows[0]["cardcode"]));
				list.Add(new SqlParameter("@branch", dt.Rows[0]["branch"]));
				list.Add(new SqlParameter("@FromDate", dt.Rows[0]["fromdate"]));
				list.Add(new SqlParameter("@Todate", dt.Rows[0]["todate"]));
			}
			return ExecuteData("apz_productionforbatch_listPlan", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet SearchProductionOrder(DataTable dt)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (dt != null && dt.Rows.Count > 0)
			{
				list.Add(new SqlParameter("@SoNo", dt.Rows[0]["so"]));
				list.Add(new SqlParameter("@ItemCode", dt.Rows[0]["itemcode"]));
				list.Add(new SqlParameter("@Slp", dt.Rows[0]["slp"]));
				list.Add(new SqlParameter("@CardCode", dt.Rows[0]["cardcode"]));
				list.Add(new SqlParameter("@branch", dt.Rows[0]["branch"]));
				list.Add(new SqlParameter("@FromDate", dt.Rows[0]["fromdate"]));
				list.Add(new SqlParameter("@Todate", dt.Rows[0]["todate"]));
				list.Add(new SqlParameter("@Whs", dt.Rows[0]["whs"]));
			}
			return ExecuteData("Apz_ProductionForBatch_SearchProductionOrder", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetProductionRequest(int id)
	{
		try
		{
			return ExecuteData("apz_productionforbatch_request", CommandType.StoredProcedure, new SqlParameter("@DocEntry", id));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetPlanning(string batch, int docEntry)
	{
		try
		{
			return ExecuteData("apz_productionforbatch_planning", CommandType.StoredProcedure, new SqlParameter("@Batch", batch), new SqlParameter("@DocEntry", docEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public int GetPage(int current, int type, string username, int branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Current", current));
			list.Add(new SqlParameter("@Type", type));
			if (!string.IsNullOrEmpty(username))
			{
				list.Add(new SqlParameter("@Username", username));
			}
			if (branch != -1)
			{
				list.Add(new SqlParameter("@Branch", branch));
			}
			DataTable dataTable = ExecuteDataTable("apz_productionforbatch_navigation", CommandType.StoredProcedure, list.ToArray());
			if (dataTable.IsNotNull())
			{
				return Function.ParseInt(dataTable.Rows[0]["DocEntry"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	public DataSet GetByStatus(string status, DateTime? fromDate, DateTime? toDate, string username, string branch)
	{
		try
		{
			return ExecuteData("apz_productionforbatch_GetByStatus", CommandType.StoredProcedure, new SqlParameter("@UserName", username), new SqlParameter("@Branch", branch), new SqlParameter("@FromDate", fromDate), new SqlParameter("@ToDate", toDate), new SqlParameter("@Status", status));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetBatchSelected(int docEntry)
	{
		try
		{
			return ExecuteData("apz_productionforbatch_map", CommandType.StoredProcedure, new SqlParameter("@DocEntry", docEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetOperation(int docEntry, int fatherId)
	{
		try
		{
			return ExecuteData("apz_productionforbatch_planning_operation", CommandType.StoredProcedure, new SqlParameter("@DocEntry", docEntry), new SqlParameter("@FatherId", fatherId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public string UpdateDelRequestBatch(DataTable dtBatch)
	{
		try
		{
			string text = "";
			if (dtBatch.IsNotNull() && dtBatch.IsExistsColumn("BatchNo"))
			{
				for (int i = 0; i < dtBatch.Rows.Count; i++)
				{
					text = text + "," + Function.ToString(dtBatch.Rows[i]["BatchNo"]);
				}
			}
			if (!string.IsNullOrEmpty(text) && text.StartsWith(","))
			{
				text = text.Substring(1, text.Length - 1);
			}
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Batch", text));
			if (ExecuteDataTable("Apz_Productionforbatch_UpdateDelRequestBatch", CommandType.StoredProcedure, list.ToArray()).IsNotNull())
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

	public DataTable LoadBatchStatusQC()
	{
		try
		{
			return ExecuteDataTable("Apz_Productionforbatch_LoadBatchStatusQC", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable LoadWhs(string userName, int branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@userName", userName));
			list.Add(new SqlParameter("@branch", branch));
			return ExecuteDataTable("Apz_Productionforbatch_LoadWhs", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}
}
