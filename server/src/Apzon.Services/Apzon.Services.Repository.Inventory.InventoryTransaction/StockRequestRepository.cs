using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Documents;
using Apzon.Services.Interfaces.Inventory.InventoryTransaction;

namespace Apzon.Services.Repository.Inventory.InventoryTransaction;

public class StockRequestRepository : BaseService<DocumentsImpl>, IStockRequest, IService<DocumentsImpl>
{
	public StockRequestRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet GetAll(DataTable dt)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (dt != null && dt.Rows.Count > 0)
			{
				list.Add(new SqlParameter("@UserName", dt.Rows[0]["username"]));
				list.Add(new SqlParameter("@Branch", dt.Rows[0]["branch"]));
				list.Add(new SqlParameter("@FromDate", dt.Rows[0]["fromdate"]));
			}
			return ExecuteData("apz_stockrequest_all", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public string Insert(DocumentsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(DocumentsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(DocumentsImpl entity)
	{
		throw new NotImplementedException();
	}

	public DocumentsImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public DataSet GetDetail(int docentry)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@DocEntry", docentry);
			return ExecuteData("apz_stockrequest_all_detail", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataTable CheckRequest(int docForBatch, string username, int branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@docForBatch", docForBatch));
			if (!string.IsNullOrEmpty(username))
			{
				list.Add(new SqlParameter("@UserName", username));
			}
			if (branch != -1)
			{
				list.Add(new SqlParameter("@Branch", branch));
			}
			return ExecuteDataTable("Apz_Stockrequest_CheckRequest", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string UpdatePrintCount(string docentry, string table, int count, string userName = "")
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@DocEntry", docentry));
			list.Add(new SqlParameter("@Table", table));
			list.Add(new SqlParameter("@Count", count));
			list.Add(new SqlParameter("@UserName", userName));
			if (ExecuteDataTable("Apz_StockRequest_UpdatePrintCount", CommandType.StoredProcedure, list.ToArray()).IsNotNull())
			{
				return "";
			}
			return "Update print count error!";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
