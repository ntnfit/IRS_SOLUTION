using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public class UnitOfMeasureGroupsClient : IUnitOfMeasureGroupsClient
{
	private const string Controller = "UnitOfMeasureGroups";

	public DataTable GetDataUomInGroup(int ugpEntry, int uomEntry)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UnitOfMeasureGroups");
		return httpClientHelper.Get("GetDataUomInGroup", new { ugpEntry, uomEntry });
	}

	public async Task<DataTable> GetUoMGroupDefinition(int ugpEntry)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UnitOfMeasureGroups");
		return await helper.GetAsync("LoadDefinition", new { ugpEntry });
	}

	public bool CheckUoMGroupExists(int ugpEntry)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UnitOfMeasureGroups");
		return httpClientHelper.GetBool("CheckUoMGroupExists", new { ugpEntry });
	}

	public async Task<HttpResult> SaveSap(Tuple<DataTable, DataTable, Enums.HttpAction> tupData)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "UnitOfMeasureGroups");
		return await helper.PostAsync("SaveSap", tupData);
	}

	public async Task<HttpResult> Remove(string ugpCode)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "UnitOfMeasureGroups");
		return await helper.PostAsync("Remove", new { ugpCode });
	}

	public DataTable GetUomItem(string itemCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UnitOfMeasureGroups");
		return httpClientHelper.Get("GetUomItem", new { itemCode });
	}
}
