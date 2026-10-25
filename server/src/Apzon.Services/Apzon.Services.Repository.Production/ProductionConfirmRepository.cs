using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Production;

namespace Apzon.Services.Repository.Production;

public class ProductionConfirmRepository : BaseService<DataTable>, IProductionConfirm
{
	public ProductionConfirmRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet GetProductionControl(DataTable dt)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (dt != null && dt.Rows.Count > 0)
			{
				list.Add(new SqlParameter("@status", dt.Rows[0]["status"]));
				list.Add(new SqlParameter("@date", dt.Rows[0]["fromDate"]));
				list.Add(new SqlParameter("@toDate", dt.Rows[0]["toDate"]));
				list.Add(new SqlParameter("@username", dt.Rows[0]["username"]));
				list.Add(new SqlParameter("@branch", dt.Rows[0]["branch"]));
			}
			return ExecuteData("Apz_ProductionConfirm_GetproductionControl", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetProductionDetail(string docEntry, string username, string branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(branch))
			{
				list.Add(new SqlParameter("@docEntry", docEntry));
			}
			if (!string.IsNullOrEmpty(username))
			{
				list.Add(new SqlParameter("@username", username));
			}
			if (!string.IsNullOrEmpty(branch))
			{
				list.Add(new SqlParameter("@branch", branch));
			}
			return ExecuteData("Apz_ProductionConfirm_GetProductionDetail", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message, docEntry, username, branch);
		}
		return new DataSet();
	}

	public DataTable GetReceiptBatch(string receiptBatch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(receiptBatch))
			{
				list.Add(new SqlParameter("@receiptBatch", receiptBatch));
			}
			return ExecuteDataTable("Apz_ProductionConfirm_GetReceiptBatch", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string isExistBatch(string batchNo)
	{
		try
		{
			string query = $"SELECT DistNumber FROM APZ_OITW_LOCATION WHERE DistNumber = N'{batchNo}'";
			return Function.ToString(ExecuteScalar(query, CommandType.Text));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string GetWorkCenter(string batchNo)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(batchNo))
			{
				list.Add(new SqlParameter("@batchNo", batchNo));
			}
			return Function.ToString(ExecuteScalar("Apz_ProductionConfirm_GetWorkCenter", CommandType.StoredProcedure, list.ToArray()));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
