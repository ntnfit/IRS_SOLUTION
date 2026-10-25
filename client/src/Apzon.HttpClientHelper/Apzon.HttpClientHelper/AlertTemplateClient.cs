using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public class AlertTemplateClient : IAlertTemplate
{
	private const string _controller = "AlertTemplate";

	public async Task<DataTable> GetAllTemplate()
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "AlertTemplate");
		return await helper.GetAsync("GetAllTemplate");
	}

	public async Task<HttpResult> SaveData(Tuple<DataTable, string> tupData)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "AlertTemplate");
		return await helper.PostAsync("SaveData", tupData);
	}

	public async Task<HttpResult> Remove(Tuple<string> tupData)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "AlertTemplate");
		return await helper.PostAsync("Remove", tupData);
	}
}
