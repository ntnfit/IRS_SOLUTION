using System.Data;
using System.Web.Http;
using System.Web.Mvc;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.MasterData;

namespace Apzon.Api.Controllers.Inventory;

public class OCYCController : BaseApiController
{
	[System.Web.Mvc.HttpGet]
	public DataTable GetData(string key)
	{
		return UnitOfWork.OCYC.Search(key);
	}

	[System.Web.Mvc.HttpGet]
	public DataTable GetList(string name, string frequency)
	{
		return UnitOfWork.OCYC.GetList(name, frequency);
	}

	[System.Web.Mvc.HttpGet]
	public int GetMaxCode()
	{
		return UnitOfWork.OCYC.GetMaxCode();
	}

	[System.Web.Mvc.HttpPost]
	public string Save([FromBody] OCYC_Impl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			BrokerService.GetUnitOfWork(SessionId);
			return "";
		}
		return UnitOfWork.OCYC.Save(doc);
	}
}
