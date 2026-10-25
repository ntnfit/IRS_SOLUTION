using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public class PromotionSetupClient : IPromotionSetupClient
{
	private string FormController = "PromotionSetup";

	public string CheckUsed(string type, int id)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
			return httpClientHelper.Get("CheckUsed", new { type, id });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public async Task<DataSet> GetData()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, FormController);
		return await http.GetAsync("GetData");
	}

	public async Task<HttpResult> SaveData(DataSet ds)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController);
		return await http.PostAsync("SaveData", ds);
	}
}
