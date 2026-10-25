using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.DataServices;

namespace Apzon.Api.Controllers.Opportunity;

[ValidateLoginSapExceptionHandling]
public class OpportunityController : BaseApiController
{
	[HttpGet]
	public int GetNewOpprId()
	{
		return UnitOfWork.Opportunity.GetNewOpprId();
	}

	[HttpGet]
	public DataTable GetDoc(int OpprId, string userCode)
	{
		return UnitOfWork.Opportunity.GetDoc(OpprId, userCode);
	}

	[HttpGet]
	public DataTable GetStage(int OpprId)
	{
		return UnitOfWork.Opportunity.GetStage(OpprId);
	}

	[HttpGet]
	public DataTable GetStageDMS(int OpprId)
	{
		return UnitOfWork.Opportunity.GetStageDMS(OpprId);
	}

	[HttpGet]
	public DataTable GetCompetitor(int OpprId)
	{
		return UnitOfWork.Opportunity.GetCompetitor(OpprId);
	}

	[HttpGet]
	public DataTable GetPartner(int OpprId)
	{
		return UnitOfWork.Opportunity.GetPartner(OpprId);
	}

	[HttpGet]
	public DataTable GetPartnerDMS(int OpprId)
	{
		return UnitOfWork.Opportunity.GetPartnerDMS(OpprId);
	}

	[HttpGet]
	public DataTable GetReason(int OpprId)
	{
		return UnitOfWork.Opportunity.GetReason(OpprId);
	}

	[HttpGet]
	public DataTable GetItemDetail(int OpprId)
	{
		return UnitOfWork.Opportunity.GetItemDetail(OpprId);
	}

	[HttpGet]
	public DataTable GetListStage()
	{
		return UnitOfWork.Opportunity.GetListStage();
	}

	[HttpGet]
	public DataTable GetListCompetitor()
	{
		return UnitOfWork.Opportunity.GetListCompetitor();
	}

	[HttpGet]
	public DataTable GetListPartner()
	{
		return UnitOfWork.Opportunity.GetListPartner();
	}

	[HttpGet]
	public DataTable GetListPartnerRelation()
	{
		return UnitOfWork.Opportunity.GetListPartnerRelation();
	}

	[HttpGet]
	public DataTable GetListReason()
	{
		return UnitOfWork.Opportunity.GetListReason();
	}

	[HttpGet]
	public DataTable GetLinkedActivities(int oprId)
	{
		return UnitOfWork.Opportunity.GetLinkedActivities(oprId);
	}

	[HttpGet]
	public DataTable GetUserForListActivity()
	{
		return UnitOfWork.Opportunity.GetUserForListActivity();
	}

	[HttpGet]
	public DataTable GetOwnerForStage()
	{
		return UnitOfWork.Opportunity.GetOwnerForStage();
	}

	[HttpGet]
	public DataTable GetBPData(string cardCode)
	{
		return UnitOfWork.Opportunity.GetBPData(cardCode);
	}

	[HttpGet]
	public DataTable SearchBPData(string cardCode)
	{
		return UnitOfWork.Opportunity.SearchBPData(cardCode);
	}

	[HttpPost]
	public DataTable GetLinkedDocuments([FromBody] DataTable DTNumAndType)
	{
		return UnitOfWork.Opportunity.GetLinkedDocuments(DTNumAndType);
	}

	[HttpGet]
	public double GetDocumentAmountAndProfit(int docType, int docNum)
	{
		return UnitOfWork.Opportunity.GetDocumentAmountAndProfit(docType, docNum);
	}

	[HttpGet]
	public DataTable GetBPChanelContact(string cardCode)
	{
		return UnitOfWork.Opportunity.GetBPChanelContact(cardCode);
	}

	[HttpGet]
	public DataTable GetComboInterest()
	{
		return UnitOfWork.Opportunity.GetComboInterest();
	}

	[HttpGet]
	public DataTable GetComboInterestLevel()
	{
		return UnitOfWork.Opportunity.GetComboInterestLevel();
	}

	[HttpGet]
	public DataTable GetInterestRange(int opprId)
	{
		return UnitOfWork.Opportunity.GetInterestRange(opprId);
	}

	[HttpGet]
	public DataTable GetRecord(string position, int opprId, string userCode, int branch)
	{
		return UnitOfWork.Opportunity.GetRecord(position, opprId, userCode, branch);
	}

	[HttpPost]
	public DataTable SearchDoc(DataTable dt)
	{
		return UnitOfWork.Opportunity.SearchDoc(dt);
	}

	[HttpPost]
	public string SaveDoc([FromBody] Tuple<string, DataSet> tuple)
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
				return unitOfWork.Opportunity.Save(tuple.Item2, SessionId + "," + SessionUserInfo.SessionDiServer);
			}
			return "Invalid session";
		}
		return UnitOfWork.Opportunity.SaveDoc(tuple.Item1, tuple.Item2);
	}

	[HttpPost]
	public string DeleteDoc([FromBody] string opprid)
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
				return unitOfWork.Opportunity.DeleteDocs(opprid, SessionId + "," + SessionUserInfo.SessionDiServer);
			}
			return "Invalid session";
		}
		return UnitOfWork.Opportunity.DeleteDoc(opprid);
	}

	[HttpGet]
	[MappingSap(IsMapping = false)]
	public string DeleteStage(int oId, int ln)
	{
		return UnitOfWork.Opportunity.DeleteStage(oId, ln);
	}
}
