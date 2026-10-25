using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Administrator.Setup.Inventory;

public class BinLocFieldActivationController : BaseApiController
{
	[HttpGet]
	public DataTable GetObfc()
	{
		return UnitOfWork.BinFieldActivation.GetObfcData();
	}

	[HttpGet]
	public string IsFieldExist(int absEntry, string fldType)
	{
		return UnitOfWork.BinFieldActivation.IsFieldExist(absEntry, fldType);
	}

	[HttpPost]
	public string Save([FromBody] DataTable doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).BinLocationField.Save(SessionUserInfo.SessionDiServer, doc);
		}
		return "Funtion not support";
	}
}
