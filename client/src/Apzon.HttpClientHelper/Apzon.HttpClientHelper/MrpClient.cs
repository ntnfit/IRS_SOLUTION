using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

internal class MrpClient : IMrpClient
{
	private const string Controller = "Mrps";

	public async Task<HttpResult> RunMrp(DataTable dt, int requestTimeout)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Mrps", requestTimeout);
		return await helper.PostAsync("Run", dt);
	}

	public async Task<DataTable> GetWarehouse(int branch)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Mrps");
		return await http.GetAsync("GetWarehouse", new { branch });
	}

	public async Task<DataTable> Search(string mrpCode, DateTime? fdate, DateTime? tdate, string status)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Mrps");
		return await helper.GetAsync("Search", new { mrpCode, fdate, tdate, status });
	}

	public async Task<string> GetEndateForecast(string forecastCode)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "Mrps");
		return await http.GetAsync("GetEndateForecast", new { forecastCode });
	}

	public async Task<DataTable> GetDataLine(int id)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Mrps");
		return await helper.GetAsync("GetById", new { id });
	}

	public async Task<HttpResult> Delete(int id)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Mrps");
		return await http.PostAsync("Delete", id);
	}

	public async Task<HttpResult> Update(DataTable dt)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Mrps");
		return await http.PostAsync("Update", dt);
	}

	public async Task<HttpResult> ValidateScheduleForMrp(string lstPsCode)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Mrps");
		return await http.GetAsync("ValidateScheduleForMrp", new { lstPsCode });
	}

	public async Task<DataTable> GetDataDoc(int id, Enums.NavigateType type)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Mrps");
		return await http.GetAsync("GetDataDoc", new { id, type });
	}

	public async Task<HttpResult> GetNewIdMrp()
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Mrps");
		return await http.GetAsync("GetNewIdMrp");
	}

	public async Task<DataTable> GetSourceCellDataDocumentType()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Mrps");
		return await http.GetAsync("GetSourceCellDataDocumentType");
	}

	public async Task<DataTable> LoadDataSourceCell(string demandType, int demandEntry, int demandLine)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Mrps");
		return await http.GetAsync("LoadDataSourceCell", new { demandType, demandEntry, demandLine });
	}

	public async Task<int> GetRequestTimeout()
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "Mrps");
		return await http.GetIntAsync("GetRequestTimeout");
	}
}
