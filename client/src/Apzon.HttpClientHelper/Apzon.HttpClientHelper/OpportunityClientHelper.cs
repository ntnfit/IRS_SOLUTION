using System;
using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

internal class OpportunityClientHelper : IOpportunityClient
{
	private const string FormController = "Opportunity";

	public async Task<string> DeleteDoc(string opprid)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "Opportunity");
		return await http.PostAsync("DeleteDoc", opprid);
	}

	public async Task<string> SaveDoc(Tuple<string, DataSet> tuple)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "Opportunity");
		return await http.PostAsync("SaveDoc", tuple);
	}

	public async Task<DataTable> SearchDoc(DataTable dt)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity");
		return await http.PostAsync("SearchDoc", dt);
	}

	public async Task<int> GetNewOpprId()
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "Opportunity");
		return await http.GetIntAsync("GetNewOpprId");
	}

	public async Task<DataTable> GetDoc(int OpprId, string userCode)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity");
		return await http.GetAsync("GetDoc", new { OpprId, userCode });
	}

	public async Task<DataTable> GetStage(int OpprId)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity");
		return await http.GetAsync("GetStage", new { OpprId });
	}

	public async Task<DataTable> GetCompetitor(int OpprId)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity");
		return await http.GetAsync("GetCompetitor", new { OpprId });
	}

	public async Task<DataTable> GetPartner(int OpprId)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity");
		return await http.GetAsync("GetPartner", new { OpprId });
	}

	public async Task<DataTable> GetReason(int OpprId)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity");
		return await http.GetAsync("GetReason", new { OpprId });
	}

	public async Task<DataTable> GetItemDetail(int OpprId)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity");
		return await http.GetAsync("GetItemDetail", new { OpprId });
	}

	public async Task<DataTable> GetListStage()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity");
		return await http.GetAsync("GetListStage");
	}

	public async Task<DataTable> GetListCompetitor()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity");
		return await http.GetAsync("GetListCompetitor");
	}

	public async Task<DataTable> GetListPartner()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity");
		return await http.GetAsync("GetListPartner");
	}

	public async Task<DataTable> GetListPartnerRelation()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity");
		return await http.GetAsync("GetListPartnerRelation");
	}

	public async Task<DataTable> GetListReason()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity");
		return await http.GetAsync("GetListReason");
	}

	public async Task<DataTable> GetLinkedActivities(int oprId)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity");
		return await http.GetAsync("GetLinkedActivities", new { oprId });
	}

	public async Task<DataTable> GetOwnerForStage()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity");
		return await http.GetAsync("GetOwnerForStage");
	}

	public DataTable GetBPData(string cardCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity");
		return httpClientHelper.Get("GetBPData", new { cardCode });
	}

	public DataTable SearchBPData(string cardCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity");
		return httpClientHelper.Get("SearchBPData", new { cardCode });
	}

	public DataTable GetLinkedDocuments(DataTable dTDocNumAndType)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity");
		return httpClientHelper.Post("GetLinkedDocuments", dTDocNumAndType);
	}

	public double GetDocumentAmountAndProfit(int docType, int docNum)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity");
		return httpClientHelper.GetDouble("GetDocumentAmountAndProfit", new { docType, docNum });
	}

	public DataTable GetBPChanelContact(string cardCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity");
		return httpClientHelper.Get("GetBPChanelContact", new { cardCode });
	}

	public DataTable GetComboInterest()
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity");
		return httpClientHelper.Get("GetComboInterest");
	}

	public DataTable GetComboInterestLevel()
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity");
		return httpClientHelper.Get("GetComboInterestLevel");
	}

	public async Task<DataTable> GetInterestRange(int opprId)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity");
		return await http.GetAsync("GetInterestRange", new { opprId });
	}

	public async Task<DataTable> GetRecord(string position, int opprId, string userCode, int branch)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity");
		return await http.GetAsync("GetRecord", new { position, opprId, userCode, branch });
	}
}
