using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Administrator.Setup;

public class TerritoryController : BaseApiController
{
	[HttpGet]
	public DataTable Search(string id)
	{
		return UnitOfWork.Territory.Search(id);
	}

	[HttpGet]
	public DataTable GetData()
	{
		return UnitOfWork.Territory.GetData();
	}

	[HttpGet]
	public bool ExistsLinkedObject(int id)
	{
		return UnitOfWork.Territory.ExistsLinkedObject(id);
	}

	[HttpPost]
	public string SaveData([FromBody] DataTable dt)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).Territories.SaveData(dt, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.Territory.SaveData(dt);
	}
}
