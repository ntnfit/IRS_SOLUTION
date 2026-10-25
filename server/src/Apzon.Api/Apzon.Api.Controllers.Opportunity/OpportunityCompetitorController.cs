using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Opportunity;

public class OpportunityCompetitorController : BaseApiController
{
	[HttpGet]
	public int GetNewId()
	{
		return UnitOfWork.OpportunityCompetitor.GetNewId();
	}

	[HttpGet]
	public int GetNewNum()
	{
		return UnitOfWork.OpportunityCompetitor.GetNewNum();
	}

	[HttpGet]
	public DataTable GetDocs()
	{
		return UnitOfWork.OpportunityCompetitor.GetDocs();
	}

	[HttpGet]
	public DataTable GetDoc(int StepId)
	{
		return UnitOfWork.OpportunityCompetitor.GetDoc(StepId);
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
			return BrokerService.GetUnitOfWork(SessionId).Competitor.Save(dt, SessionUserInfo.SessionDiServer).Message;
		}
		return UnitOfWork.OpportunityCompetitor.SaveDoc(dt);
	}

	[HttpPost]
	public string DeleteDocs(List<int> lst)
	{
		return UnitOfWork.OpportunityCompetitor.DeleteDocs(lst);
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
			return BrokerService.GetUnitOfWork(SessionId).Competitor.Delete(id, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.OpportunityCompetitor.DeleteDoc(id);
	}
}
