using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Financials;

public class CurrencyController : BaseApiController
{
	[HttpGet]
	public DataTable GetData()
	{
		return UnitOfWork.Currencies.GetData();
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataTable dtObj)
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
				return unitOfWork.Currencies.Save(SessionUserInfo.SessionDiServer, dtObj);
			}
			return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
		}
		return UnitOfWork.Currencies.Save(dtObj);
	}

	[HttpGet]
	public string CheckExistGroupCode(string curr)
	{
		return UnitOfWork.Currencies.ExistInDocument(curr);
	}

	[HttpPost]
	public HttpResult Remove([FromBody] string currCode)
	{
		new HttpResult();
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (Global.DatabaseSystemType == Enums.DatabaseSystemType.HANA)
			{
				return unitOfWork.Currencies.Remove(currCode, SessionUserInfo.SessionDiServer);
			}
			return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
		}
		return UnitOfWork.Currencies.Remove(currCode);
	}
}
