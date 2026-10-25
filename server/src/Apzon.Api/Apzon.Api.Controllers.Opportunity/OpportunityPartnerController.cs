using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Opportunity;

public class OpportunityPartnerController : BaseApiController
{
	[HttpGet]
	public int GetNewId()
	{
		return UnitOfWork.OpportunityPartner.GetNewId();
	}

	[HttpGet]
	public int GetNewNum()
	{
		return UnitOfWork.OpportunityPartner.GetNewNum();
	}

	[HttpGet]
	public DataTable GetDocs()
	{
		return UnitOfWork.OpportunityPartner.GetDocs();
	}

	[HttpGet]
	public DataTable GetDoc(int StepId)
	{
		return UnitOfWork.OpportunityPartner.GetDoc(StepId);
	}

	[HttpPost]
	public string SaveDoc(DataTable dt)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).OpportunityPartners.Save(dt, SessionUserInfo.SessionDiServer).Message;
		}
		return UnitOfWork.OpportunityPartner.SaveDoc(dt);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteDocs(List<int> lst)
	{
		return UnitOfWork.OpportunityPartner.DeleteDocs(lst);
	}

	[HttpPost]
	public string DeleteDoc([FromBody] int id)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).OpportunityPartners.Delete(id, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.OpportunityPartner.DeleteDoc(id);
	}
}
