using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public class ThirdPartyTokenClient : IThirdPartyTokenClient
{
	private readonly string FormController = "ThirdPartyToken";

	public async Task<DataTable> LoadTokens(string tokenName, string remarks, string status)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return await helper.GetAsync("GetTokens", new { tokenName, remarks, status });
	}

	public async Task<HttpResult> SaveTokens(DataTable dt, string action)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController);
			return await helper.PostAsync("SaveTokens?action=" + action, dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult
			{
				Content = null,
				MessageCode = Enums.MessageCode.Error,
				Message = ex.Message
			};
		}
	}

	public async Task<HttpResult> DeleteToken(int entry)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController);
			return await helper.PostAsync("DeleteDoc", entry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult
			{
				Content = null,
				MessageCode = Enums.MessageCode.Error,
				Message = ex.Message
			};
		}
	}

	public async Task<HttpResult> GetMaxAbsEntry()
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("GetMaxAbsEntry");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult
			{
				Content = null,
				MessageCode = Enums.MessageCode.Error,
				Message = ex.Message
			};
		}
	}
}
