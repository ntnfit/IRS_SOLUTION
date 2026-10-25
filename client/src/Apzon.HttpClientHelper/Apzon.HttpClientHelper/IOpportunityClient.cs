using System;
using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IOpportunityClient
{
	Task<string> DeleteDoc(string ids);

	Task<string> SaveDoc(Tuple<string, DataSet> tuple);

	Task<DataTable> SearchDoc(DataTable dt);

	Task<int> GetNewOpprId();

	Task<DataTable> GetDoc(int OpprId, string userCode);

	Task<DataTable> GetStage(int OpprId);

	Task<DataTable> GetCompetitor(int OpprId);

	Task<DataTable> GetPartner(int OpprId);

	Task<DataTable> GetReason(int OpprId);

	Task<DataTable> GetItemDetail(int OpprId);

	Task<DataTable> GetListStage();

	Task<DataTable> GetListCompetitor();

	Task<DataTable> GetListPartner();

	Task<DataTable> GetListPartnerRelation();

	Task<DataTable> GetListReason();

	Task<DataTable> GetLinkedActivities(int oprId);

	Task<DataTable> GetOwnerForStage();

	DataTable GetBPData(string cardCode);

	DataTable SearchBPData(string cardCode);

	DataTable GetLinkedDocuments(DataTable dTDocNumAndType);

	double GetDocumentAmountAndProfit(int docType, int docNum);

	DataTable GetBPChanelContact(string cardCode);

	DataTable GetComboInterest();

	DataTable GetComboInterestLevel();

	Task<DataTable> GetInterestRange(int opprId);

	Task<DataTable> GetRecord(string position, int opprId, string userCode, int branch);
}
