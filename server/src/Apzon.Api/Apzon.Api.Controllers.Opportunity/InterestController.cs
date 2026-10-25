using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Opportunity;

public class InterestController : BaseApiController
{
	[HttpGet]
	public DataTable GetDocs()
	{
		return UnitOfWork.Interest.GetDocs();
	}

	[HttpPost]
	public string SaveDoc(DataTable dt)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).Interest.Save(dt, SessionUserInfo.SessionDiServer).Message;
		}
		return UnitOfWork.Interest.SaveDoc(dt);
	}

	[HttpPost]
	public string DeleteDoc([FromBody] int id)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).Interest.Delete(id, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.Interest.DeleteDoc(id);
	}
}
