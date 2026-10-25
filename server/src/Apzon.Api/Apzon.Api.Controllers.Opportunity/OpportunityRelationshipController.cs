using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Opportunity;

public class OpportunityRelationshipController : BaseApiController
{
	[HttpGet]
	public int GetNewId()
	{
		return UnitOfWork.OpportunityRelationship.GetNewId();
	}

	[HttpGet]
	public int GetNewNum()
	{
		return UnitOfWork.OpportunityRelationship.GetNewNum();
	}

	[HttpGet]
	public DataTable GetDocs()
	{
		return UnitOfWork.OpportunityRelationship.GetDocs();
	}

	[HttpGet]
	public DataTable GetDoc(int StepId)
	{
		return UnitOfWork.OpportunityRelationship.GetDoc(StepId);
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
			return BrokerService.GetUnitOfWork(SessionId).Relationship.Save(dt, SessionUserInfo.SessionDiServer).Message;
		}
		return UnitOfWork.OpportunityRelationship.SaveDoc(dt);
	}

	[HttpPost]
	public string DeleteDocs(List<int> lst)
	{
		return UnitOfWork.OpportunityRelationship.DeleteDocs(lst);
	}

	[HttpPost]
	public string DeleteDoc([FromBody] int code)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).Relationship.Delete(code, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.OpportunityRelationship.DeleteDoc(code);
	}
}
