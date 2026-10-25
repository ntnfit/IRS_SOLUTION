using System;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.PlanningProduction;

public class MrpPlanController : BaseApiController
{
	[HttpGet]
	public DataSet RunMrpPlan(string bpCode, string timeType)
	{
		try
		{
			return UnitOfWork.MrpPlan.RunMrpPlan(bpCode, timeType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataSet ds)
	{
		try
		{
			return UnitOfWork.MrpPlan.SaveMrpPlan(ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	[HttpGet]
	public int GetPage(int current, int type, string username, int branch)
	{
		try
		{
			return UnitOfWork.MrpPlan.GetPage(current, type, username, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	[HttpGet]
	public DataTable GetHeader(int id)
	{
		try
		{
			return UnitOfWork.MrpPlan.GetHeader(id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataSet GetDetail(int id)
	{
		try
		{
			return UnitOfWork.MrpPlan.GetDetail(id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}
}
