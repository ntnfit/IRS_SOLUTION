using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.DataServices;

namespace Apzon.Api.Controllers.RoutingAndResource;

public class ResourceGroupController : BaseApiController
{
	[HttpGet]
	public DataTable SearchData(string resGroupName)
	{
		return UnitOfWork.ResourceGroup.SearchData(resGroupName);
	}

	[HttpPost]
	public string Save(DataTable dt)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "INVALIDSESSION";
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.ResourceGroup.Save(dt, SessionUserInfo.SessionDiServer);
			}
			return "INVALIDSESSION";
		}
		return "This function is not supported";
	}
}
