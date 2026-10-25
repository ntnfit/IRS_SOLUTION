using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public class ApprovalStagesClient : IApprovalStages
{
	private const string _controller = "ApprovalStages";

	public async Task<DataTable> GetData()
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ApprovalStages");
		return await helper.GetAsync("GetData");
	}

	public async Task<DataTable> GetDataLines(int stageCode)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ApprovalStages");
		return await helper.GetAsync("GetDataLines", new { stageCode });
	}

	public async Task<DataTable> SearchData(int stageCode, string stageName)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ApprovalStages");
		return await helper.GetAsync("SearchData", new { stageCode, stageName });
	}

	public int GetNewCode()
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ApprovalStages");
		return httpClientHelper.GetInt("GetNewCode");
	}

	public async Task<HttpResult> Save(Tuple<DataTable, DataTable, string> tuple)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ApprovalStages");
		return await helper.PostAsync("Save", tuple);
	}

	public async Task<DataTable> GetUserInfo(string userName)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ApprovalStages");
		return await helper.GetAsync("GetUserInfo", new { userName });
	}

	public async Task<HttpResult> Delete(Tuple<int> tuple)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ApprovalStages");
		return await helper.PostAsync("Delete", tuple);
	}

	public DataTable GetRecord(string position, string stageCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ApprovalStages");
		return httpClientHelper.Get("GetRecord", new { position, stageCode });
	}
}
