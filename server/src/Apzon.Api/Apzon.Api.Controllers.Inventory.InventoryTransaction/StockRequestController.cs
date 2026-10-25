using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;

namespace Apzon.Api.Controllers.Inventory.InventoryTransaction;

public class StockRequestController : BaseApiController
{
	[HttpPost]
	public DataSet GetAll([FromBody] DataTable dt)
	{
		try
		{
			return UnitOfWork.StockRequest.GetAll(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet GetDetail(int docentry)
	{
		try
		{
			return UnitOfWork.StockRequest.GetDetail(docentry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataTable CheckRequest(int docForBatch, string username, int branch)
	{
		try
		{
			return UnitOfWork.StockRequest.CheckRequest(docForBatch, username, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public string UpdatePrintCount(string docentry, string table, int count, string userName = "")
	{
		try
		{
			return UnitOfWork.StockRequest.UpdatePrintCount(docentry, table, count, userName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
