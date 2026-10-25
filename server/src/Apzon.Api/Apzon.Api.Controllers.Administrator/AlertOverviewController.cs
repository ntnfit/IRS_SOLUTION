using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator;

public class AlertOverviewController : BaseApiController
{
	[HttpGet]
	public DataTable GetAlert(string username, int mailtype)
	{
		try
		{
			return UnitOfWork.AlertOverview.GetAlert(username, mailtype);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable GetAlertUnread(string username, int mailtype)
	{
		try
		{
			return UnitOfWork.AlertOverview.GetAlertUnread(username, mailtype);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataSet GetAlertData(string code)
	{
		try
		{
			string userName = SessionUserInfo.UserName;
			return UnitOfWork.AlertOverview.GetAlertData(code, userName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpPost]
	public HttpResult SaveReadMessageChanged([FromBody] Tuple<int, string, string> model)
	{
		try
		{
			int item = model.Item1;
			string item2 = model.Item2;
			string item3 = model.Item3;
			return UnitOfWork.AlertOverview.SaveReadMessageChanged(item, item2, item3);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpGet]
	public int CheckCountAlertOverview(string username)
	{
		try
		{
			return UnitOfWork.AlertOverview.CheckCountAlertOverview(username);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}
}
