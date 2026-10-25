using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.DataServices;
using Apzon.Entities.Models.MasterData;

namespace Apzon.Api.Controllers.Inventory;

public class STDPriceListsController : BaseApiController
{
	[HttpGet]
	public DataTable LoadData()
	{
		return UnitOfWork.STDPriceLists.LoadData();
	}

	[HttpPost]
	public string SaveDoc([FromBody] DataTable dt)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.STDPriceLists.Save(dt, SessionUserInfo.SessionDiServer);
			}
			return "Invalid session";
		}
		return UnitOfWork.STDPriceLists.SaveDoc(dt);
	}

	[HttpPost]
	public string SaveDocSelection([FromBody] DataSet ds)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.STDPriceLists.Save(ds.Tables[0], SessionUserInfo.SessionDiServer);
			}
			return "Invalid session";
		}
		return UnitOfWork.STDPriceLists.SaveDocSelection(ds);
	}

	[HttpGet]
	public DataTable GetGroupNumPriceList()
	{
		return UnitOfWork.STDPriceLists.GetGroupNumPriceList();
	}

	[HttpPost]
	public string DeletePriceList([FromBody] int listNum)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.STDPriceLists.Delete(listNum, SessionUserInfo.SessionDiServer);
			}
			return "Invalid session";
		}
		return UnitOfWork.STDPriceLists.DeletePriceList(listNum);
	}

	[HttpGet]
	public string CorrectPriceCurrency()
	{
		return UnitOfWork.STDPriceLists.CorrectPriceCurrency();
	}

	[HttpGet]
	public DataTable DetailsLoadData(int listNum)
	{
		return UnitOfWork.STDPriceLists.DetailsLoadData(listNum);
	}

	[HttpPost]
	public string DetailsSaveDoc([FromBody] DataSet ds)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.STDPriceLists.SaveItemPrice(ds.Tables[0], SessionUserInfo.SessionDiServer);
			}
			return "Invalid session";
		}
		return UnitOfWork.STDPriceLists.DetailsSaveDoc(ds);
	}

	[HttpGet]
	public int GetUgpEntry(string itemCode)
	{
		return UnitOfWork.STDPriceLists.GetUgpEntry(itemCode);
	}

	[HttpGet]
	public DataTable GetBasePrice(int priceList, string itemCode)
	{
		return UnitOfWork.STDPriceLists.GetPriceCurrency(priceList, itemCode);
	}

	[HttpGet]
	public DataTable DetailsLoadIsGross(int listNum)
	{
		return UnitOfWork.STDPriceLists.DetailsLoadIsGross(listNum);
	}

	[HttpPost]
	public DataTable GetPriceForListItem([FromBody] Tuple<DataTable, int> model)
	{
		return UnitOfWork.STDPriceLists.GetPriceForListItem(model.Item1, model.Item2);
	}

	[HttpGet]
	public PriceListCurrencyImpl GetDataCurrencyByPriceList(int priceList)
	{
		return UnitOfWork.STDPriceLists.GetDataCurrencyByPriceList(priceList);
	}

	[HttpGet]
	public DataTable GetDataSystemPriceListDetail(int priceList)
	{
		return UnitOfWork.STDPriceLists.GetDataSystemPriceListDetail(priceList);
	}
}
