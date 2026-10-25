using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Mrps;

namespace Apzon.HttpClientHelper;

internal class AdvanceForecastClient : IAdvanceForecastClient
{
	public readonly string Controller = "AdvanceForecast";

	public async Task<HttpResult> Save(DataSet dsSave)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, Controller);
		return await http.PostAsync("Save", dsSave);
	}

	public async Task<HttpResult> Update(DataSet dsSave)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, Controller);
		return await http.PostAsync("Update", dsSave);
	}

	public async Task<HttpResult> Delete(int code)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, Controller);
		return await http.PostAsync("DeleteForecast", code);
	}

	public async Task<DataTable> GetSettingSap()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, Controller);
		return await http.GetAsync("GetSettingSap");
	}

	public DataTable GetDataDoc(string forecastCode, string view, Enums.NavigateType type)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, Controller);
		return httpClientHelper.Get("GetDataDoc", new { forecastCode, view, type });
	}

	public DataSet GetDataLine(int absId)
	{
		using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, Controller);
		return httpClientHelper.Get("GetDataLine", new { absId });
	}

	public DataTable GetColumnLine()
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, Controller);
		return httpClientHelper.Get("GetColumnLine");
	}

	public async Task<DataTable> GetColumnDoc()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, Controller);
		return await http.GetAsync("GetColumnDoc");
	}

	public DataTable GetDataItemCode(string itemCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, Controller);
		return httpClientHelper.Get("GetDataItemCode", new { itemCode });
	}

	public DataTable GetDataPrice(int priceList)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, Controller);
		return httpClientHelper.Get("GetDataPrice", new { priceList });
	}

	public async Task<HttpResult> GetNewIdForecast()
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, Controller);
		return await http.GetAsync("GetNewIdForecast");
	}

	public async Task<DataSet> GenerateForecast(InputGenerateForecastImpl inputGen)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, Controller);
		return await http.PostAsync("GenerateForecast", inputGen);
	}
}
