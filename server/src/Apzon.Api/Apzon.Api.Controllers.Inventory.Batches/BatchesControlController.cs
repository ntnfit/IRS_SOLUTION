using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;

namespace Apzon.Api.Controllers.Inventory.Batches;

public class BatchesControlController : BaseApiController
{
	[HttpGet]
	public DataSet LoadBatch(string option, string sb, string so, string bp, string userName, int branch)
	{
		try
		{
			return UnitOfWork.BatchesControl.LoadBatch(option, sb, so, bp, userName, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public string UpdateBatchSalesBlanket(string batches, int argNo)
	{
		try
		{
			return UnitOfWork.BatchesControl.UpdateBatchSalesBlanket(batches, argNo);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpPost]
	public string UpdateBatchForSales([FromBody] DataTable dtBatch)
	{
		try
		{
			return UnitOfWork.BatchesControl.UpdateBatchForSales(dtBatch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpPost]
	public string UpdateBatchDetails([FromBody] DataTable dtBatch)
	{
		try
		{
			return UnitOfWork.BatchesControl.UpdateBatchDetails(dtBatch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
