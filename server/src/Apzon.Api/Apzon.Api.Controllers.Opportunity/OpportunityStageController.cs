using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.Opportunity;

public class OpportunityStageController : BaseApiController
{
	[HttpGet]
	public int GetNewId()
	{
		return UnitOfWork.OpportunityStage.GetNewId();
	}

	[HttpGet]
	public int GetNewNum()
	{
		return UnitOfWork.OpportunityStage.GetNewNum();
	}

	[HttpGet]
	public DataTable GetDocs()
	{
		return UnitOfWork.OpportunityStage.GetDocs();
	}

	[HttpGet]
	public DataTable GetDoc(int StepId)
	{
		return UnitOfWork.OpportunityStage.GetDoc(StepId);
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
			return BrokerService.GetUnitOfWork(SessionId).OpportunityStage.Save(dt, SessionUserInfo.SessionDiServer).Message;
		}
		return UnitOfWork.OpportunityStage.SaveDoc(dt);
	}

	[HttpPost]
	public string DeleteDocs(List<int> lst)
	{
		return UnitOfWork.OpportunityStage.DeleteDocs(lst);
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
			return BrokerService.GetUnitOfWork(SessionId).OpportunityStage.Delete(num, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.OpportunityStage.DeleteDoc(num);
	}
}
