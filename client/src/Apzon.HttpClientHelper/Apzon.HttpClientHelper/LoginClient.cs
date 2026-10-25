using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Administrator;

namespace Apzon.HttpClientHelper;

internal class LoginClient : ILoginClient
{
	private const string Controller = "LoginIRS";

	public void LogOut(string sessionId)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "LoginIRS");
			httpClientHelper.Get("LogOut", new { sessionId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public async Task<List<string>> GetSessionPool(Tuple<LoginImpl, int> model)
	{
		using HttpClientHelper<List<string>> http = new HttpClientHelper<List<string>>(Configuration.ApiServer, "LoginIRS");
		return await http.PostAsync("GetSessionPool", model);
	}
}
