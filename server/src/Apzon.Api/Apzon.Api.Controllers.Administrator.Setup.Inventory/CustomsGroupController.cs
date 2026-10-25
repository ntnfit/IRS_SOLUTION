using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.Setup.Inventory;

public class CustomsGroupController : BaseApiController
{
	[HttpGet]
	public DataTable LoadDoc()
	{
		return UnitOfWork.CustomsGroup.LoadDoc();
	}

	[HttpGet]
	public DataTable GetMaxCstGrpCode()
	{
		return UnitOfWork.CustomsGroup.GetMaxCstGrpCode();
	}

	[HttpGet]
	public DataTable CheckExistCstGrpCode([FromBody] int cstGrpCode)
	{
		return UnitOfWork.CustomsGroup.CheckExistCstGrpCode(cstGrpCode);
	}

	[HttpPost]
	public HttpResult SaveOARG([FromBody] DataTable dt)
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
				return unitOfWork.CustomGroups.Save(dt, SessionUserInfo.SessionDiServer);
			}
			return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
		}
		return UnitOfWork.CustomsGroup.SaveOARG(dt);
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
				return unitOfWork.CustomGroups.Remove(dt, SessionUserInfo.SessionDiServer);
			}
			return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}
}
