using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.DataServices;
using Apzon.Entities.Models.Items;

namespace Apzon.Api.Controllers.Items;

public class SpecialBp_LineSPP1Controller : BaseApiController
{
	[HttpGet]
	public DataTable SearchLineSPP1(string ItemCode, string CardCode)
	{
		return UnitOfWork.SpecialBP_LineSPP1.SearchLineSPP1(ItemCode, CardCode);
	}

	[HttpPost]
	public string Delete([FromBody] List<Tuple<string, string>> obj)
	{
		return UnitOfWork.SpecialBP_LineSPP1.DeleteLineSPP1(obj);
	}

	[HttpGet]
	public DataTable GetMaxLineNum(string _itemCode, string _cardCode)
	{
		return UnitOfWork.SpecialBP_LineSPP1.GetMaxLineNum(_itemCode, _cardCode);
	}

	[HttpPost]
	public string SaveDoc([FromBody] List<SpecialBP_LineSPP1Impl> doc)
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
				return unitOfWork.SpecialPrices.SaveDataAreas(doc, SessionUserInfo.SessionDiServer);
			}
			return "Invalid session";
		}
		return UnitOfWork.SpecialBP_LineSPP1.SaveDoc(doc);
	}
}
