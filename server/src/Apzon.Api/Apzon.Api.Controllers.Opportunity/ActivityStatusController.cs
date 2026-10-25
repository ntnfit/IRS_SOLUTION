using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.DataServices;

namespace Apzon.Api.Controllers.Opportunity;

public class ActivityStatusController : BaseApiController
{
	[HttpGet]
	public DataTable GetAll()
	{
		return UnitOfWork.ActivityStatus.GetAll();
	}

	[HttpGet]
	public DataTable GetData()
	{
		return UnitOfWork.ActivityStatus.ShowData();
	}

	[HttpGet]
	public DataTable GetUserSignByUserCodeOUSR(string madk)
	{
		return UnitOfWork.ActivityStatus.GetUserSignByUserCode_tableOUSR(madk);
	}

	[HttpPost]
	public string SaveDoc([FromBody] DataTable dt)
	{
		if (Global.LoginSap && Global.ServiceType == Enums.ServiceType.ServiceLayer)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.ActivityStatus.Save(dt, SessionUserInfo.SessionDiServer).Message;
			}
			return "Invalid session";
		}
		return UnitOfWork.ActivityStatus.SaveDoc(dt);
	}

	[HttpGet]
	public int GetMaxActivityStatus()
	{
		return UnitOfWork.ActivityStatus.GetMaxActivityStatus();
	}

	[HttpGet]
	public DataTable GetDefaultTable()
	{
		return UnitOfWork.ActivityStatus.GetDefaultTable();
	}

	[HttpPost]
	public string DeleteDoc([FromBody] int id)
	{
		return UnitOfWork.ActivityStatus.Remove(id);
	}
}
