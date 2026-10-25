using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.HttpClientHelper;

public class FacebookManagerClient : IFacebookManagerClient
{
	private const string Controller = "FacebookManager";

	public DataTable GetDataAccessUser(string userCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "FacebookManager");
		return httpClientHelper.Get("GetDataAccessUser", new { userCode });
	}

	public async Task<HttpResult> SaveUserAccessToken(DataTable dtUserToken)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "FacebookManager");
		return await http.PostAsync("SaveUserAccessToken", dtUserToken);
	}

	public async Task<HttpResult> SavePageUserInfo(DataTable dtPageInfo)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "FacebookManager");
		return await http.PostAsync("SavePageUserInfo", dtPageInfo);
	}

	public async Task<DataTable> GetListUserAccessPage(string userCode)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "FacebookManager");
		return await http.GetAsync("GetListUserAccessPage", new { userCode });
	}

	public async Task<DataSet> GetDataConversationPage(string pageId)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "FacebookManager");
		return await http.GetAsync("GetDataConversationPage", new { pageId });
	}

	public DataTable GetDataPageUser(string pageId)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "FacebookManager");
		return httpClientHelper.Get("GetDataPageUser", new { pageId });
	}

	public async Task<HttpResult> SaveDataPageUser(string pageId, DataTable dtConver)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "FacebookManager");
		return await http.PostAsync("SaveDataPageUser", new Tuple<string, DataTable>(pageId, dtConver));
	}

	public DataTable GetDataItem(string itemCode, string bpCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "FacebookManager");
		return httpClientHelper.Get("GetDataItem", new { itemCode, bpCode });
	}

	public async Task<DataTable> GetDataFacebookCustomer(string facebookUserId)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "FacebookManager");
		return await http.GetAsync("GetDataFacebookCustomer", new { facebookUserId });
	}

	public async Task<string> SaveFacebookOrder(DocumentsImpl doc, BPImpls dtBp)
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "FacebookManager");
		return await http.PostAsync("SaveOrder", new Tuple<DocumentsImpl, BPImpls>(doc, dtBp));
	}
}
