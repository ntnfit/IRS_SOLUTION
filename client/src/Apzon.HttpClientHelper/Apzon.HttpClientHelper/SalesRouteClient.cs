using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public class SalesRouteClient : ISalesRouteClient
{
	private const string _controller = "SalesRoute";

	public async Task<DataTable> GetDataDoc(string routeCode, string routeName, string status)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesRoute");
		return await helper.GetAsync("GetDataDoc", new { routeCode, routeName, status });
	}

	public DataTable GetDataDetails(string routeCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesRoute");
		return httpClientHelper.Get("GetDataDetails", new { routeCode });
	}

	public async Task<DataTable> GetDataOutlets(int id, string routeCode, int empId)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesRoute");
		return await helper.GetAsync("GetDataOutlets", new { id, routeCode, empId });
	}

	public DataTable GetRecord(string position, string routeCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesRoute");
		return httpClientHelper.Get("GetRecord", new { position, routeCode });
	}

	public async Task<HttpResult> SaveData(Tuple<DataSet, string> tupData)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "SalesRoute");
		return await helper.PostAsync("SaveData", tupData);
	}

	public async Task<HttpResult> Remove(Tuple<string> tupData)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "SalesRoute");
		return await helper.PostAsync("Remove", tupData);
	}

	public async Task<DataTable> GetListDistributors(int id, string routeCode, int empId)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesRoute");
		return await helper.GetAsync("GetListDistributors", new { id, routeCode, empId });
	}

	public DataTable GetBpMasterData(string type, string cardCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesRoute");
		return httpClientHelper.Get("GetBpMasterData", new { type, cardCode });
	}

	public DataTable GetEmployees(string routeCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesRoute");
		return httpClientHelper.Get("GetEmployees", new { routeCode });
	}

	public DataTable GetOutlets(string routeCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesRoute");
		return httpClientHelper.Get("GetOutlets", new { routeCode });
	}

	public DataTable GetDistributors(string routeCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesRoute");
		return httpClientHelper.Get("GetDistributors", new { routeCode });
	}
}
