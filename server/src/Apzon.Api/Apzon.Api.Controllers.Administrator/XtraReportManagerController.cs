using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator;

public class XtraReportManagerController : BaseApiController
{
	[HttpGet]
	public DataSet LoadReport(string userName, int branch)
	{
		try
		{
			return UnitOfWork.XtraReportManager.LoadReport(userName, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public DataSet LoadLayout(string userName, int branch)
	{
		try
		{
			return UnitOfWork.XtraReportManager.LoadLayout(userName, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataTable dt)
	{
		try
		{
			return UnitOfWork.XtraReportManager.SaveReport(dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpGet]
	public string GetDataReportLayoutById(int id)
	{
		return UnitOfWork.XtraReportManager.GetDataReportLayoutById(id);
	}

	[HttpGet]
	public string RemoveReport(int id)
	{
		return UnitOfWork.XtraReportManager.RemoveReport(id);
	}
}
