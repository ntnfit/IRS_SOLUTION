using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

internal class BarcodeInventoryCountingClient : IBarcodeInventoryCountingClient
{
	private const string FormController = "BarcodeInventoryCounting";

	public async Task<DataSet> GetDataStructure()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "BarcodeInventoryCounting");
		return await http.GetAsync("GetDataStructure");
	}

	public async Task<DataTable> SearchDoc(DataTable dtSearch)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BarcodeInventoryCounting");
		return await http.PostAsync("SearchDoc", dtSearch);
	}

	public async Task<DataTable> GetDataLineByDocEntry(int docEntry)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BarcodeInventoryCounting");
		return await http.GetAsync("GetDataLineByDocEntry", new { docEntry });
	}

	public async Task<int> GetNewDocEntry()
	{
		using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "BarcodeInventoryCounting");
		return httpClientHelper.GetInt("GetNewDocEntry");
	}

	public async Task<DataTable> GetNextLastData(Enums.NavigateType type, int docEntry)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BarcodeInventoryCounting");
		return await http.GetAsync("GetNextLastData", new { type, docEntry });
	}

	public DataTable GetDataByBarcode(string barcodeString, string whsCode, string binCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BarcodeInventoryCounting");
		return httpClientHelper.Get("GetDataByBarcode", new { barcodeString, whsCode, binCode });
	}

	public async Task<HttpResult> AddNew(DataSet dsSave)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "BarcodeInventoryCounting");
		return await http.PostAsync("AddNew", dsSave);
	}

	public async Task<HttpResult> UpdateData(DataSet dsSave)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "BarcodeInventoryCounting");
		return await http.PostAsync("UpdateData", dsSave);
	}

	public async Task<string> Delete(int docEntry)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "BarcodeInventoryCounting");
		return await http.PostAsync("Delete", docEntry);
	}
}
