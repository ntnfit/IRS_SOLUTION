using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.Setup;

public class LengthAndWidthController : BaseApiController
{
	[HttpGet]
	public int GetMax()
	{
		return UnitOfWork.LengthAndWidth.GetMax();
	}

	[HttpGet]
	public DataTable GetData()
	{
		return UnitOfWork.LengthAndWidth.GetData();
	}

	[HttpPost]
	public HttpResult Save([FromBody] DataTable dt)
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
				return unitOfWork.LengthAndWidth.Save(dt, SessionUserInfo.SessionDiServer);
			}
			return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
		}
		return UnitOfWork.LengthAndWidth.Save(dt);
	}

	[HttpGet]
	public bool GetCheckData(string key)
	{
		return UnitOfWork.LengthAndWidth.CheckData(key);
	}

	[HttpGet]
	public DataTable CheckExistUnitCode(int unitCode)
	{
		return UnitOfWork.LengthAndWidth.CheckExistUnitCode(unitCode);
	}

	[HttpPost]
	public HttpResult Remove(DataTable dt)
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
				return unitOfWork.LengthAndWidth.Remove(dt, SessionUserInfo.SessionDiServer);
			}
			return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
		}
		return UnitOfWork.LengthAndWidth.Remove(dt);
	}
}
