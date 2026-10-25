using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public class StoreClient : IStoreClient
{
	private string _controller = "Store";

	public async Task<DataTable> GetStore(string storeCode = "", string cardCode = "", string status = "")
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _controller);
		return await helper.GetAsync("GetStore", new { storeCode, cardCode, status });
	}

	public async Task<DataTable> GetEvents(int clgCode)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _controller);
		return await helper.GetAsync("GetEvents", new { clgCode });
	}

	public async Task<DataTable> GetGeography()
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _controller);
		return await helper.GetAsync("GetGeography");
	}

	public async Task<DataTable> GetStoreEvents(string storeCode)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _controller);
		return await helper.GetAsync("GetStoreEvents", new { storeCode });
	}

	public async Task<DataTable> GetRecord(string position, string storeCode)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _controller);
		return await helper.GetAsync("GetRecord", new { position, storeCode });
	}

	public async Task<DataTable> GetLastEvent(string storeCode)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _controller);
		return await helper.GetAsync("GetLastEvent", new { storeCode });
	}

	public async Task<HttpResult> Save(Tuple<DataSet, string> tuple)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, _controller);
		return await helper.PostAsync("Save", tuple);
	}

	public async Task<HttpResult> CheckRemove(string storeCode)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, _controller);
		return await helper.GetAsync("CheckRemove", new { storeCode });
	}

	public async Task<HttpResult> Remove(Tuple<string> tuple)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, _controller);
		return await helper.PostAsync("Remove", tuple);
	}

	public async Task<DataTable> GetParentsGeography(string ggpCode)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _controller);
		return await helper.GetAsync("GetParentsGeography", new { ggpCode });
	}
}
