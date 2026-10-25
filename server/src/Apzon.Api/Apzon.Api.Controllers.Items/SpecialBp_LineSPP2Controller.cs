using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.DataServices;
using Apzon.Entities.Models.Items;

namespace Apzon.Api.Controllers.Items;

public class SpecialBp_LineSPP2Controller : BaseApiController
{
	[HttpGet]
	public DataTable SearchLineSPP2(string itemCode, string cardCode, int SPP1LineNum)
	{
		return UnitOfWork.SpecialBP_LineSPP2.SearchLineSPP2(itemCode, cardCode, SPP1LineNum);
	}

	[HttpPost]
	public string Delete([FromBody] List<Tuple<string, string>> obj)
	{
		return UnitOfWork.SpecialBP_LineSPP2.DeleteLineSPP2(obj);
	}

	[HttpGet]
	public DataTable GetMaxSPP2LNum(string _ItemCode, string _CardCode, int _Spp1LNum)
	{
		return UnitOfWork.SpecialBP_LineSPP2.GetMaxSPP2LNum(_ItemCode, _CardCode, _Spp1LNum);
	}

	[HttpPost]
	public string SaveDoc([FromBody] List<SpecialBP_LineSPP2Impl> doc)
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
				return unitOfWork.SpecialPrices.SaveQuantityAreas(doc, SessionUserInfo.SessionDiServer);
			}
			return "Invalid session";
		}
		return UnitOfWork.SpecialBP_LineSPP2.SaveDoc(doc);
	}
}
