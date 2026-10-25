using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.DataServices;

namespace Apzon.Api.Controllers.Administrator.Setup;

public class FreightController : BaseApiController
{
	[HttpGet]
	public DataTable GetFreight()
	{
		return UnitOfWork.Freight.GetFreight();
	}

	[HttpPost]
	public string DeleteFreight([FromBody] int code)
	{
		return UnitOfWork.Freight.DeleteFreight(code);
	}

	[HttpPost]
	public string UpdateFreight(DataTable dt)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (Global.DatabaseSystemType == Enums.DatabaseSystemType.HANA)
			{
				return unitOfWork.FreightSetup.Save(dt, SessionUserInfo.SessionDiServer);
			}
			return UnitOfWork.Freight.UpdateFreight(dt);
		}
		return UnitOfWork.Freight.UpdateFreight(dt);
	}

	[HttpGet]
	public DataTable GetMaxExpnsCode()
	{
		return UnitOfWork.Freight.GetMaxExpnsCode();
	}

	[HttpGet]
	public DataTable GetOutputTaxGroup()
	{
		return UnitOfWork.Freight.GetOutputTaxGroup();
	}

	[HttpGet]
	public DataTable GetVatGroupI()
	{
		return UnitOfWork.Freight.GetVatGroupI();
	}
}
