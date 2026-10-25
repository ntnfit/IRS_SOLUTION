using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

internal class FormFilterSettingClient : IFormFilterSettingClient
{
	private const string Controller = "FormFilterSetting";

	public DataTable GetDataFilterField(ApzObjectTypes objectType)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "FormFilterSetting");
		return httpClientHelper.Get("GetDataFilterField", new { objectType });
	}

	public async Task<DataTable> GetDataFilterSetting(ApzObjectTypes objectType)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "FormFilterSetting");
		return await helper.GetAsync("GetDataFilterSetting", new { objectType });
	}

	public async Task<HttpResult> SaveFilterSetting(Tuple<DataSet, ApzObjectTypes> tuple)
	{
		using HttpClientHelper<HttpResult> hp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "FormFilterSetting");
		return await hp.PostAsync("SaveFilterSetting", tuple);
	}

	public async Task<HttpResult> DeleteFilterSetting(int docEntry)
	{
		using HttpClientHelper<HttpResult> hp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "FormFilterSetting");
		return await hp.PostAsync("DeleteFilterSetting", docEntry);
	}

	public async Task<HttpResult> SaveFilterSettingAddNew(Tuple<DataTable, ApzObjectTypes, DataRow> tuple)
	{
		using HttpClientHelper<HttpResult> hp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "FormFilterSetting");
		return await hp.PostAsync("SaveFilterSettingAddNew", tuple);
	}

	public async Task<HttpResult> ChangeSourceType(DataTable dt)
	{
		using HttpClientHelper<HttpResult> hp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "FormFilterSetting");
		return await hp.PostAsync("ChangeSourceType", dt);
	}

	public async Task<DataTable> GetDataFilterValue(int docEntry)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "FormFilterSetting");
		return await helper.GetAsync("GetDataFilterValue", new { docEntry });
	}

	public async Task<DataTable> GetDataFilterSettingLine(int docEntry)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "FormFilterSetting");
		return await helper.GetAsync("GetDataFilterSettingLine", new { docEntry });
	}

	public async Task<HttpResult> SaveFilterSettingLine(Tuple<DataTable, ApzObjectTypes> tuple)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "FormFilterSetting");
		return await helper.PostAsync("SaveFilterSettingLine", tuple);
	}

	public async Task<HttpResult> SaveFilterSettingLineAddNew(Tuple<DataTable, ApzObjectTypes> tuple)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "FormFilterSetting");
		return await helper.PostAsync("SaveFilterSettingLineAddNew", tuple);
	}

	public async Task<HttpResult> DeleteFilterSettingLine(Tuple<DataTable, ApzObjectTypes> tuple)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "FormFilterSetting");
		return await helper.PostAsync("DeleteFilterSettingLine", tuple);
	}

	public async Task<DataTable> GetDataFilterSettingTable(int docEntry)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "FormFilterSetting");
		return await helper.GetAsync("GetDataFilterSettingTable", new { docEntry });
	}

	public async Task<DataTable> GetDataTableByTableName(string tableName)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "FormFilterSetting");
		return await helper.GetAsync("GetDataTableByTableName", new { tableName });
	}

	public async Task<HttpResult> SaveFilterSettingTable(Tuple<DataTable, ApzObjectTypes> tuple)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "FormFilterSetting");
		return await helper.PostAsync("SaveFilterSettingTable", tuple);
	}

	public async Task<HttpResult> DeleteFilterSettingTable(Tuple<DataTable, ApzObjectTypes> tuple)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "FormFilterSetting");
		return await helper.PostAsync("DeleteFilterSettingTable", tuple);
	}

	public async Task<DataTable> GetFilterStructure()
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "FormFilterSetting");
		return await helper.GetAsync("GetFilterStructure");
	}

	public async Task<DataTable> GetFilterLineStructure()
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "FormFilterSetting");
		return await helper.GetAsync("GetFilterLineStructure");
	}

	public async Task<DataTable> GetFilterTableStructure()
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "FormFilterSetting");
		return await helper.GetAsync("GetFilterTableStructure");
	}
}
