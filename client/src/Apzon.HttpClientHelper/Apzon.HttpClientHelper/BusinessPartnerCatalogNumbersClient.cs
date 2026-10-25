using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

internal class BusinessPartnerCatalogNumbersClient : IBusinessPartnerCatalogNumbersClient
{
	private const string Controller = "BusinessPartnerCatalogNumbers";

	public DataTable GetDataItemByBpCatalog(string subCatNum, string cardCode, string itemCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BusinessPartnerCatalogNumbers");
		return httpClientHelper.Get("GetDataItemByBpCatalog", new { subCatNum, cardCode, itemCode });
	}

	public DataTable GetBpByItemCode(string itemcode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BusinessPartnerCatalogNumbers");
		return httpClientHelper.Get("GetBPByItemCode", new { itemcode });
	}

	public DataTable GetItemByCardCode(string cardcode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BusinessPartnerCatalogNumbers");
		return httpClientHelper.Get("GetItemByCardCode", new { cardcode });
	}

	public string Remove(Tuple<string, string, string, string> model)
	{
		using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "BusinessPartnerCatalogNumbers");
		return httpClientHelper.Post("Remove", model);
	}

	public DataTable GetData(Enums.NavigateType type, Enums.BusinessPartnerCatalogNumbersTableType objectType, string par)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BusinessPartnerCatalogNumbers");
		return httpClientHelper.Get("GetData", new { type, objectType, par });
	}

	public string GetNameKey(string code, Enums.BusinessPartnerCatalogNumbersTableType type)
	{
		using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "BusinessPartnerCatalogNumbers");
		return httpClientHelper.Get("GetNameKey", new { code, type });
	}

	public async Task<string> GetCardNameByCode(string cardCode)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "BusinessPartnerCatalogNumbers");
		return await http.GetAsync("GetCardNameByCode", new { cardCode });
	}

	public async Task<string> GetItemNameByCode(string itemCode)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "BusinessPartnerCatalogNumbers");
		return await http.GetAsync("GetItemNameByCode", new { itemCode });
	}

	public async Task<HttpResult> SaveItem(DataTable dt)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "BusinessPartnerCatalogNumbers");
		return await http.PostAsync("SaveItem", dt);
	}

	public async Task<HttpResult> SaveBp(DataTable dt)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "BusinessPartnerCatalogNumbers");
		return await http.PostAsync("SaveBp", dt);
	}

	public string CheckExist(string code, string subtitle, string item, ApzObjectTypes objtype)
	{
		using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "BusinessPartnerCatalogNumbers");
		return httpClientHelper.Get("CheckExist", new { code, subtitle, item, objtype });
	}

	public string GetSubCatNumByItemAndCardCode(string itemCode, string cardCode)
	{
		using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "BusinessPartnerCatalogNumbers");
		return httpClientHelper.Get("GetSubCatNumByItemAndCardCode", new { itemCode, cardCode });
	}
}
