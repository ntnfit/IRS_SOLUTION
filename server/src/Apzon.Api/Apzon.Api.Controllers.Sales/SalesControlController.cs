using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Sales;

public class SalesControlController : BaseApiController
{
	[HttpGet]
	public DataSet LoadSalesControl(string userCode, string ownerName, string salesEmp, DateTime fromDate, DateTime toDate, string docStatus, string bpCode, string saleType, string userName, int branch)
	{
		try
		{
			return UnitOfWork.SalesControl.LoadSalesControl(userCode, ownerName, salesEmp, fromDate, toDate, docStatus, bpCode, saleType, userName, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpPost]
	public DataSet LoadSalesControl([FromBody] DataTable dt)
	{
		try
		{
			return UnitOfWork.SalesControl.LoadSalesControl(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public string UpdateDeltimeConfirm(int docentry, int lineid, DateTime deltime, string type, string remark, string username)
	{
		try
		{
			return UnitOfWork.SalesControl.UpdateDeltimeConfirm(docentry, lineid, deltime, type, remark, username);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpGet]
	public DataSetResult GetOrderByBarcode(string barcode)
	{
		try
		{
			return UnitOfWork.SalesControl.GetOrderByBarcode(barcode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSetResult(ex.Message);
		}
	}
}
