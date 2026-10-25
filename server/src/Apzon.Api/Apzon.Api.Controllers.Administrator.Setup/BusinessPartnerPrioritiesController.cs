using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.Setup;

public class BusinessPartnerPrioritiesController : BaseApiController
{
	[HttpGet]
	public DataTable LoadDoc()
	{
		return UnitOfWork.BusinessPrioritiesSetup.LoadDoc();
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataTable dtDoc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (Global.DatabaseSystemType == Enums.DatabaseSystemType.HANA)
			{
				return unitOfWork.BusinessPartners.SaveBPPriorities(dtDoc, SessionUserInfo.SessionDiServer);
			}
			return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
		}
		return UnitOfWork.BusinessPrioritiesSetup.Save(dtDoc);
	}

	[HttpGet]
	public DataTable CheckExistBPP(int prioCode)
	{
		return UnitOfWork.BusinessPrioritiesSetup.CheckExistBPP(prioCode);
	}

	[HttpPost]
	public HttpResult Remove([FromBody] DataTable dt)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (Global.DatabaseSystemType == Enums.DatabaseSystemType.HANA)
			{
				return unitOfWork.BusinessPartners.RemoveBPPriorities(dt, SessionUserInfo.SessionDiServer);
			}
			return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
		}
		return UnitOfWork.BusinessPrioritiesSetup.Remove(dt);
	}
}
