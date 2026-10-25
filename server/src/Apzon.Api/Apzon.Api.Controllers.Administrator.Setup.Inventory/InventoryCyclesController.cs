using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.Setup.Inventory;

public class InventoryCyclesController : BaseApiController
{
	[HttpGet]
	public DataTable FindNextLastFrist(int docId, int type)
	{
		return UnitOfWork.InventoryCycles.FindNextLastFrist(docId, type);
	}

	[HttpPost]
	public HttpResult Saves([FromBody] DataTable dt)
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
				return unitOfWork.InventoryCycles.Add(dt, SessionUserInfo.SessionDiServer);
			}
		}
		return UnitOfWork.InventoryCycles.Save(dt, "A");
	}

	[HttpPost]
	public HttpResult Update([FromBody] DataTable dt)
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
				return unitOfWork.InventoryCycles.Update(dt, SessionUserInfo.SessionDiServer);
			}
		}
		return UnitOfWork.InventoryCycles.Save(dt, "U");
	}

	[HttpPost]
	public HttpResult Delete([FromBody] DataTable dt)
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
				return unitOfWork.InventoryCycles.Delete(dt, SessionUserInfo.SessionDiServer);
			}
		}
		return UnitOfWork.InventoryCycles.Save(dt, "D");
	}

	[HttpGet]
	public int GetMaxId()
	{
		return UnitOfWork.InventoryCycles.GetMaxId();
	}
}
