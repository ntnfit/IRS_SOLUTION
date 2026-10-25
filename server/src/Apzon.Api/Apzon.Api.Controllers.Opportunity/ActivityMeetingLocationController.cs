using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Opportunity;

public class ActivityMeetingLocationController : BaseApiController
{
	[HttpGet]
	public DataTable GetAll()
	{
		return UnitOfWork.ActivityMeetingLocation.GetAll();
	}

	[HttpGet]
	public DataTable GetData()
	{
		return UnitOfWork.ActivityMeetingLocation.ShowData();
	}

	[HttpGet]
	public DataTable GetUserSignByUserCodeOUSR(string madk)
	{
		return UnitOfWork.ActivityMeetingLocation.GetUserSignByUserCode_tableOUSR(madk);
	}

	[HttpPost]
	public string SaveDoc([FromBody] DataTable dt)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).ActivitiesMeetingLocation.Save(dt, SessionUserInfo.SessionDiServer).Message;
		}
		return UnitOfWork.ActivityMeetingLocation.SaveDoc(dt);
	}

	[HttpPost]
	public string DeleteDoc([FromBody] int code)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).ActivitiesMeetingLocation.Delete(code, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.ActivityMeetingLocation.DeleteDoc(code);
	}

	[HttpGet]
	public int GetMaxActivityMeetingLocation()
	{
		return UnitOfWork.ActivityMeetingLocation.GetMaxActivityMeetingLocation();
	}
}
