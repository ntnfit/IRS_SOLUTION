using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.DataServices;

namespace Apzon.Api.Controllers.Items;

public class SpecialBpController : BaseApiController
{
	[HttpGet]
	public DataSet Search(string cardCode)
	{
		return UnitOfWork.SpecialBP.Search(cardCode);
	}

	[HttpPost]
	public string SaveDoc([FromBody] DataSet doc)
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
				return unitOfWork.SpecialPrices.SaveDoc(doc, SessionUserInfo.SessionDiServer);
			}
			return "Invalid session";
		}
		return UnitOfWork.PeriodAndVolumeDiscounts.SaveDoc(doc);
	}

	[HttpPost]
	public string Delete([FromBody] List<Tuple<string, string>> obj)
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
				return unitOfWork.SpecialPrices.Delete(obj.Find((Tuple<string, string> t) => t.Item1 == "itemCode").Item2, obj.Find((Tuple<string, string> t) => t.Item1 == "cardCode").Item2, SessionUserInfo.SessionDiServer);
			}
			return "Invalid session";
		}
		return UnitOfWork.SpecialBP.Delete(obj);
	}

	[HttpGet]
	public DataTable ShowCardCode(string cardCode)
	{
		return UnitOfWork.SpecialBP.ShowCardCode(cardCode);
	}

	[HttpGet]
	public DataTable GetItemByItemCode(string itemCode, int listNum)
	{
		return UnitOfWork.SpecialBP.GetItemByItemCode(itemCode, listNum);
	}
}
