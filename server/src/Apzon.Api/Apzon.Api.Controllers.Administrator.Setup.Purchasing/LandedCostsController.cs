using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.LandedCosts;

namespace Apzon.Api.Controllers.Administrator.Setup.Purchasing;

public class LandedCostsController : BaseApiController
{
	[HttpGet]
	public DataTable SearchData(string fromDate, string toDate, string vendor, string broker, int docNum)
	{
		return UnitOfWork.LandedCosts.SearchData(fromDate, toDate, vendor, broker, docNum);
	}

	[HttpGet]
	public DataSet GetDetailDocId(int docId)
	{
		return UnitOfWork.LandedCosts.GetDetaiByDocId(docId);
	}

	[HttpGet]
	public DataTable FindDataNextLast(int id, int type)
	{
		return UnitOfWork.LandedCosts.FindDataNextLast(id, type);
	}

	[HttpGet]
	public DataTable GetNo()
	{
		return UnitOfWork.LandedCosts.GetNo();
	}

	[HttpGet]
	public DataTable GetPriceList()
	{
		return UnitOfWork.LandedCosts.GetPriceList();
	}

	[HttpGet]
	public DataTable GetLandedCostName()
	{
		return UnitOfWork.LandedCosts.GetLandedCostName();
	}

	[HttpPost]
	public HttpResult Add([FromBody] DataSet ds)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			BrokerService.GetUnitOfWork(SessionId);
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpPost]
	public HttpResult Save([FromBody] LandedCostImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			return BrokerService.GetUnitOfWork(SessionId).LandedCosts.Save(doc, SessionUserInfo.SessionDiServer);
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpPost]
	public HttpResult Close([FromBody] int docEntry)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			return BrokerService.GetUnitOfWork(SessionId).LandedCosts.Close(docEntry, SessionUserInfo.SessionDiServer);
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpGet]
	public DataTable GetListDetailGLI(int objectType, string ids)
	{
		return UnitOfWork.LandedCosts.GetListDetailGLI(objectType, ids);
	}

	[HttpGet]
	public DataTable GetBusinessPartner(string cardCode, string cardType, string userName, string branch)
	{
		return UnitOfWork.LandedCosts.GetBusinessPartner(cardCode, cardType, userName, branch);
	}

	[HttpGet]
	public int GetMaxDocEntry()
	{
		return UnitOfWork.LandedCosts.GetMaxDocEntry();
	}
}
