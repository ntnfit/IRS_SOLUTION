using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.MasterData;

namespace Apzon.Api.Controllers.Administrator.Setup.General;

public class PriceListController : BaseApiController
{
	[HttpGet]
	public List<PriceListImpl> Cfl()
	{
		return UnitOfWork.PriceLists.Cfl();
	}

	[HttpGet]
	public DataTable GetGroupNumPriceList()
	{
		return UnitOfWork.PriceLists.GetGroupNumPriceList();
	}

	[HttpGet]
	public DataTable ShowPriceList()
	{
		return UnitOfWork.PriceLists.ShowPriceList();
	}

	[HttpGet]
	public int GetMaxCode()
	{
		return UnitOfWork.PriceLists.GetMaxCode();
	}

	[HttpPost]
	public DataTable Search([FromBody] PriceListImpl doc)
	{
		return UnitOfWork.PriceLists.Search(doc);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteDoc([FromBody] int idCode)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			BrokerService.GetUnitOfWork(SessionId);
			return "Chức năng đang được xây dựng!";
		}
		return UnitOfWork.PriceLists.DeleteDoc(idCode);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveDoc([FromBody] PriceListImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			BrokerService.GetUnitOfWork(SessionId);
			return "Chức năng đang được xây dựng!";
		}
		return UnitOfWork.PriceLists.SaveDoc(doc);
	}

	[HttpPost]
	public DataTable GetPriceList([FromBody] string itmCode)
	{
		return UnitOfWork.PriceLists.GetPriceList(itmCode);
	}
}
