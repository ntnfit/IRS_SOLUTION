using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public class SaleTerritoryClient : ISaleTerritoryClient
{
	private string _controller = "SaleTerritory";

	public async Task<DataTable> GetSaleTerritory(string territoryCode, string location, string status)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _controller);
		return await helper.GetAsync("GetSaleTerritory", new { territoryCode, location, status });
	}

	public async Task<DataTable> GetBranchStructure(string territoryCode)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _controller);
		return await helper.GetAsync("GetBranchStructure", new { territoryCode });
	}

	public async Task<DataTable> GetStructureDetails(string territoryCode, string regionCode, string ggpCode)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _controller);
		return await helper.GetAsync("GetStructureDetails", new { territoryCode, regionCode, ggpCode });
	}

	public async Task<DataTable> GetStore(string territoryCode, string code)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _controller);
		return await helper.GetAsync("GetStore", new { territoryCode, code });
	}

	public DataTable CheckExistsGeography(string code, int isCountry)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _controller);
		return httpClientHelper.Get("CheckExistsGeography", new { code, isCountry });
	}

	public async Task<DataTable> GetRecord(string position, string territoryCode)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _controller);
		return await helper.GetAsync("GetRecord", new { position, territoryCode });
	}

	public async Task<HttpResult> SaveData(Tuple<DataSet, string> tuple)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, _controller);
		return await helper.PostAsync("SaveData", tuple);
	}

	public async Task<HttpResult> Remove(Tuple<string> tuple)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, _controller);
		return await helper.PostAsync("Remove", tuple);
	}
}
