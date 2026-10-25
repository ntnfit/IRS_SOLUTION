using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public class ApprovalTemplatesClient : IApprovalTemplates
{
	private const string _controller = "ApprovalTemplates";

	public async Task<DataTable> GetData()
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ApprovalTemplates");
		return await helper.GetAsync("GetData");
	}

	public async Task<DataSet> GetDataLines(int templateCode)
	{
		using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ApprovalTemplates");
		return await helper.GetAsync("GetDataLines", new { templateCode });
	}

	public async Task<DataTable> SearchData(string templateName, string templateRemarks)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ApprovalTemplates");
		return await helper.GetAsync("SearchData", new { templateName, templateRemarks });
	}

	public int GetNewCode()
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ApprovalTemplates");
		return httpClientHelper.GetInt("GetNewCode");
	}

	public async Task<HttpResult> Save(Tuple<DataSet, string> tuple)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ApprovalTemplates");
		return await helper.PostAsync("Save", tuple);
	}

	public async Task<DataTable> GetUserInfo(string userName)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ApprovalTemplates");
		return await helper.GetAsync("GetUserInfo", new { userName });
	}

	public async Task<HttpResult> Delete(Tuple<int> tuple)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ApprovalTemplates");
		return await helper.PostAsync("Delete", tuple);
	}

	public DataTable GetRecord(string position, string templateCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ApprovalTemplates");
		return httpClientHelper.Get("GetRecord", new { position, templateCode });
	}

	public async Task<DataTable> GetApprovalStageInfo(string templateName)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ApprovalTemplates");
		return await helper.GetAsync("GetApprovalStageInfo", new { templateName });
	}

	public int GetQueryId(string queryName)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ApprovalTemplates");
		return httpClientHelper.GetInt("GetQueryId", new { queryName });
	}
}
