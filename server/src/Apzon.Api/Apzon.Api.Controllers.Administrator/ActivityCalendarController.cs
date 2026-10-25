using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Administrator;

public class ActivityCalendarController : BaseApiController
{
	[HttpGet]
	public DataTable GetDoc()
	{
		return UnitOfWork.ActivityCalendar.GetDoc();
	}

	[HttpGet]
	public DataTable GetUsers()
	{
		return UnitOfWork.ActivityCalendar.GetUsers();
	}

	[HttpGet]
	public DataTable GetDocByUser(int attendUser)
	{
		return UnitOfWork.ActivityCalendar.GetDocByUser(attendUser);
	}

	[HttpGet]
	public DataTable GetDocByEmpl(int attendEmpl)
	{
		return UnitOfWork.ActivityCalendar.GetDocByEmpl(attendEmpl);
	}

	[HttpGet]
	public DataTable GetActivityByCode(int clgCode)
	{
		return UnitOfWork.ActivityCalendar.GetActivityByCode(clgCode);
	}

	[HttpGet]
	public DataTable GetUnlockedUsers()
	{
		return UnitOfWork.ActivityCalendar.GetUnlockedUsers();
	}

	[HttpGet]
	public DataTable GetActiveEmployees()
	{
		return UnitOfWork.ActivityCalendar.GetActiveEmployees();
	}

	[HttpPost]
	public string SaveActivityInterval([FromBody] DataTable dt)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).Activities.SaveActivityInterval(SessionUserInfo.SessionDiServer, dt);
		}
		return UnitOfWork.ActivityCalendar.SaveActivityInterval(dt);
	}

	[HttpPost]
	public string SaveActivityStatus([FromBody] Tuple<int, int> tpl)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).Activities.SaveActivityStatus(SessionUserInfo.SessionDiServer, tpl.Item1, tpl.Item2);
		}
		return UnitOfWork.ActivityCalendar.SaveActivityStatus(tpl.Item1, tpl.Item2);
	}

	[HttpPost]
	public string SaveActivityAction([FromBody] Tuple<int, string> tpl)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).Activities.SaveActivityAction(SessionUserInfo.SessionDiServer, tpl.Item1, tpl.Item2);
		}
		return UnitOfWork.ActivityCalendar.SaveActivityAction(tpl.Item1, tpl.Item2);
	}
}
