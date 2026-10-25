using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using System.Web.Mvc;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Administrator.Setup.Inventory;

public class WeightController : BaseApiController
{
	[System.Web.Mvc.HttpGet]
	public int GetMax()
	{
		return UnitOfWork.Weight.GetMax();
	}

	[System.Web.Mvc.HttpGet]
	public DataTable GetData()
	{
		return UnitOfWork.Weight.GetData();
	}

	[System.Web.Mvc.HttpPost]
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
				return unitOfWork.Weight.Save(dtObj, SessionUserInfo.SessionDiServer);
			}
			return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
		}
		return UnitOfWork.Weight.Save(dtObj);
	}

	[System.Web.Mvc.HttpGet]
	public bool GetCheckData(string key)
	{
		return UnitOfWork.Weight.Check(key);
	}

	[System.Web.Mvc.HttpPost]
	public string RemoveWeights([FromBody] List<string> lstWeight)
	{
		return UnitOfWork.Weight.RemoveWeights(lstWeight);
	}

	[System.Web.Mvc.HttpGet]
	public DataTable GetWeightByUnitDisply(string key)
	{
		return UnitOfWork.Weight.CheckUnitDisply(key);
	}

	[System.Web.Mvc.HttpPost]
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
				return unitOfWork.Weight.Remove(dt, SessionUserInfo.SessionDiServer);
			}
			return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
		}
		return new HttpResult();
	}
}
