using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Opportunity;

public class InterestLevelController : BaseApiController
{
	[HttpGet]
	public DataTable GetDocs()
	{
		return UnitOfWork.InterestLevel.GetDocs();
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveDoc(DataTable dt)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).InterestLevel.Save(dt, SessionUserInfo.SessionDiServer).Message;
		}
		return UnitOfWork.InterestLevel.SaveDoc(dt);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteDoc([FromBody] int id)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).InterestLevel.Delete(id, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.InterestLevel.DeleteDoc(id);
	}
}
