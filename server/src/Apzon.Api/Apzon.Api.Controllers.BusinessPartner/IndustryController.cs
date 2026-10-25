using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.BusinessPartner;

public class IndustryController : BaseApiController
{
	[HttpGet]
	public DataTable LoadData()
	{
		return UnitOfWork.Industry.LoadData();
	}

	[HttpGet]
	public bool ExistsIndCode(int indCode)
	{
		return UnitOfWork.Industry.ExistsIndCode(indCode);
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
			return BrokerService.GetUnitOfWork(SessionId).Industry.Save(dt, SessionUserInfo.SessionDiServer).Message;
		}
		return UnitOfWork.Industry.SaveDoc(dt);
	}

	[HttpPost]
	public string DeleteDoc([FromBody] int indCode)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).Industry.Delete(indCode, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.Industry.DeleteDoc(indCode);
	}
}
