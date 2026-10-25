using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Opportunity;

public class OpportunityReasonController : BaseApiController
{
	[HttpGet]
	public int GetNewId()
	{
		return UnitOfWork.OpportunityReason.GetNewId();
	}

	[HttpGet]
	public int GetNewNum()
	{
		return UnitOfWork.OpportunityReason.GetNewNum();
	}

	[HttpGet]
	public DataTable GetDocs()
	{
		return UnitOfWork.OpportunityReason.GetDocs();
	}

	[HttpGet]
	public DataTable GetDoc(int StepId)
	{
		return UnitOfWork.OpportunityReason.GetDoc(StepId);
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
			return BrokerService.GetUnitOfWork(SessionId).OpportunityReasons.Save(dt, SessionUserInfo.SessionDiServer).Message;
		}
		return UnitOfWork.OpportunityReason.SaveDoc(dt);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteDocs(List<int> lst)
	{
		return UnitOfWork.OpportunityReason.DeleteDocs(lst);
	}

	[HttpPost]
	public string DeleteDoc([FromBody] int num)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).OpportunityReasons.Delete(num, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.OpportunityReason.DeleteDoc(num);
	}
}
