using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public class WorkingProcessClient : IWorkingProcessClient
{
	public string FormController => "WorkingProcess";

	public DataSet GetWorkingProcessSetting(int userId)
	{
		using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, FormController);
		return httpClientHelper.Get("GetWorkingProcessSetting", new { userId });
	}

	public DataSet GetSetting(int userId)
	{
		using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, FormController);
		return httpClientHelper.Get("GetSetting", new { userId });
	}

	public async Task<HttpResult> SaveWorkingProcessSetting(Tuple<DataSet> tupData)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController);
		return await helper.PostAsync("SaveWorkingProcessSetting", tupData);
	}

	public DataTable GetTemplateData(int userId, string status, string requestType)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return httpClientHelper.Get("GetTemplateData", new { userId, status, requestType });
	}

	public async Task<DataTable> GetTemplateDetails(int wddCode)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return await helper.GetAsync("GetTemplateDetails", new { wddCode });
	}

	public DataTable GetTooltipData(int wddCode, int wtmCode, int docEntry, int userId)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return httpClientHelper.Get("GetTooltipData", new { wddCode, wtmCode, docEntry, userId });
	}

	public DataTable GetTileViewSetting(int userId)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return httpClientHelper.Get("GetTileViewSetting", new { userId });
	}

	public bool CheckDocumentCompleteApproval(int wddCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return httpClientHelper.GetBool("CheckDocumentCompleteApproval", new { wddCode });
	}

	public DataTable FilterMenu(Tuple<DataSet> tupFilter)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return httpClientHelper.Post("FilterMenu", tupFilter);
	}

	public HttpResult SaveMenu(Tuple<int, DataTable> tup)
	{
		using HttpClientHelper<HttpResult> httpClientHelper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController);
		return httpClientHelper.Post("SaveMenu", tup);
	}

	public async Task<bool> AllowLastGroupUserCreatedDoc(int wddCode, int userId)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return await helper.GetBoolAsync("AllowLastGroupUserCreatedDoc", new { wddCode, userId });
	}

	public async Task<bool> IsUserApproved(int wddCode, int userId)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return await helper.GetBoolAsync("IsUserApproved", new { wddCode, userId });
	}
}
