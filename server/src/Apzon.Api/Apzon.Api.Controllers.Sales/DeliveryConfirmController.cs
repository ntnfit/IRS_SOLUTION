using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Commons;

namespace Apzon.Api.Controllers.Sales;

[ValidateLoginSapExceptionHandling]
public class DeliveryConfirmController : BaseApiController
{
	[HttpGet]
	public DataSet GetDeliveryOrder(int docEntry)
	{
		try
		{
			return UnitOfWork.DeliveryConfirm.GetDeliveryOrder(docEntry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataTable GetBatchDetail(string batch)
	{
		try
		{
			return UnitOfWork.DeliveryConfirm.GetBatchDetail(batch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpPost]
	public string ConfirmBatch(DataSet ds)
	{
		try
		{
			return UnitOfWork.DeliveryConfirm.ConfirmBatch(ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
