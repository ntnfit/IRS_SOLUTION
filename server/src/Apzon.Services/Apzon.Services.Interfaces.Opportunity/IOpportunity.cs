using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Production;

namespace Apzon.Services.Interfaces.Opportunity;

public interface IOpportunity
{
	DataTable GetDoc(int OpprId, string userCode);

	DataTable GetStage(int OpprId);

	DataTable GetCompetitor(int OpprId);

	DataTable GetPartner(int OpprId);

	DataTable GetReason(int OpprId);

	string SaveDoc(string action, DataSet ds);

	DataTable SearchDoc(DataTable dt);

	int GetNewOpprId();

	DataTable GetListStage();

	DataTable GetListCompetitor();

	DataTable GetListPartner();

	DataTable GetListPartnerRelation();

	DataTable GetListReason();

	DataTable GetItemDetail(int OpprId);

	string DeleteDoc(string opprid);

	DataTable GetLinkedActivities(int oprId);

	string DeleteStage(int oId, int ln);

	DataTable GetUserForListActivity();

	DataTable GetOwnerForStage();

	DataTable GetBPData(string cardCode);

	DataTable SearchBPData(string cardCode);

	DataTable GetLinkedDocuments(DataTable dTNumAndType);

	double GetDocumentAmountAndProfit(int docType, int docNum);

	DataTable GetBPChanelContact(string cardCode);

	DataTable GetComboInterest();

	DataTable GetInterestRange(int opprId);

	DataTable GetComboInterestLevel();

	DataTable GetRecord(string position, int opprId, string userCode, int branch);

	string AddFile();

	string SaveAttachmentFiles(List<AttachmentFile_Impl> lstFiles, int docEntry);

	string ProcessDataReasons(DataTable dt, int retKey);

	DataTable GetStageDMS(int OpprId);

	DataTable GetPartnerDMS(int OpprId);

	string GetItemDetailUdfColumn();
}
