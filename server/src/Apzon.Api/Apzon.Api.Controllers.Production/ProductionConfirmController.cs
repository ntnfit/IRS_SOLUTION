using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;

namespace Apzon.Api.Controllers.Production;

public class ProductionConfirmController : BaseApiController
{
	[HttpPost]
	public DataSet GetProductionControl([FromBody] DataTable dt)
	{
		try
		{
			return UnitOfWork.ProductionConfirm.GetProductionControl(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet GetProductionDetail(string docEntry, string username, string branch)
	{
		try
		{
			return UnitOfWork.ProductionConfirm.GetProductionDetail(docEntry, username, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataTable GetReceiptBatch(string receiptBatch)
	{
		try
		{
			return UnitOfWork.ProductionConfirm.GetReceiptBatch(receiptBatch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public string isExistBatch(string batchNo)
	{
		try
		{
			return UnitOfWork.ProductionConfirm.isExistBatch(batchNo);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpGet]
	public string GetWorkCenter(string batchNo)
	{
		try
		{
			return UnitOfWork.ProductionConfirm.GetWorkCenter(batchNo);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
