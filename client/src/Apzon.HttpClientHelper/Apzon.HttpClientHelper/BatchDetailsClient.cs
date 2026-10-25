using System;
using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

internal class BatchDetailsClient : IBatchDetails
{
	private const string FormController = "BatchDetail";

	public DataTable GetByAbsEntry(int absEntry)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BatchDetail");
		return httpClientHelper.Get("GetByAbsEntry", new { absEntry });
	}

	public async Task<DataTable> GetDataBatchWhs(string itemCode, string code, string whs)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BatchDetail");
		return await helper.GetAsync("GetDataBatchWhs", new { itemCode, code, whs });
	}

	public async Task<DataTable> GetDataAutoSelect(string type, string isAllocateBin, string itemCode, string whsCode, double genQty, DataTable dtExistBatch)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BatchDetail");
		return await helper.PostAsync("GetDataAutoSelect", new Tuple<string, string, string, string, double, DataTable>(type, isAllocateBin, itemCode, whsCode, genQty, dtExistBatch));
	}

	public DataTable GetDataAllowcateBin(string itemCode, string whsCode, decimal qty)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BatchDetail");
		return httpClientHelper.Get("GetDataAllowcateBin", new { itemCode, whsCode, qty });
	}

	public async Task<DataSet> LoadDataAutoSelectConfig()
	{
		using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "BatchDetail");
		return await helper.GetAsync("LoadDataAutoSelectConfig");
	}

	public async Task<string> SaveConfig(DataTable dt)
	{
		using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "BatchDetail");
		return await helper.PostAsync("SaveConfig", dt);
	}

	public async Task<int> GetPage(int currentId, int type, string username, int branch)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BatchDetail");
		return await helper.GetIntAsync("GetPage", new { currentId, type, username, branch });
	}

	public async Task<DataSet> GetBatchDetail(int absEntry, string itemCode, string status, string distNumber, string mnfSerial, string lotNumber)
	{
		using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "BatchDetail");
		return await helper.GetAsync("GetBatchDetail", new { absEntry, itemCode, status, distNumber, mnfSerial, lotNumber });
	}

	public async Task<string> Save(DataSet ds)
	{
		using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "BatchDetail");
		return await helper.PostAsync("Save", ds);
	}

	public DataTable GetDataBatchSerial(string serialNo, string mnfSerial, string lotNumber, string itemCode, string tranType)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BatchDetail");
		return httpClientHelper.Get("GetDataBatchSerial", new { serialNo, mnfSerial, lotNumber, itemCode, tranType });
	}
}
