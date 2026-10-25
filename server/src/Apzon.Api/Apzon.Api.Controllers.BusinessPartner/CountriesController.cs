using System.Data;
using System.Web.Http;
using System.Web.Mvc;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.BusinessPartner;

public class CountriesController : BaseApiController
{
	[System.Web.Mvc.HttpGet]
	public DataTable GetOADF()
	{
		return UnitOfWork.Countries.LoadOADF();
	}

	[System.Web.Mvc.HttpGet]
	public DataTable GetOCRY()
	{
		return UnitOfWork.Countries.GetOCRY();
	}

	[System.Web.Mvc.HttpPost]
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
				return unitOfWork.Countries.Save(dtDoc, SessionUserInfo.SessionDiServer);
			}
			return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
		}
		return UnitOfWork.Countries.Save(dtDoc);
	}

	[System.Web.Mvc.HttpPost]
	public HttpResult Remove([FromBody] string code)
	{
		HttpResult httpResult = new HttpResult();
		if (string.IsNullOrEmpty(SessionId))
		{
			return new HttpResult(Enums.MessageCode.Error, "Invalid session");
		}
		IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
		if (Global.DatabaseSystemType == Enums.DatabaseSystemType.HANA)
		{
			return unitOfWork.Countries.Remove(code, SessionUserInfo.SessionDiServer);
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[System.Web.Mvc.HttpGet]
	public DataTable CheckExistCode([FromBody] string code)
	{
		return UnitOfWork.Countries.CheckExistCode(code);
	}
}
