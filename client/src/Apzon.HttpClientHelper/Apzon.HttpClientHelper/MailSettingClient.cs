using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;

namespace Apzon.HttpClientHelper;

internal class MailSettingClient : IMailSettingClient
{
	private const string FormController = "MailSetting";

	public async Task<DataTable> GetDoc()
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "MailSetting");
			return await hp.GetAsync("GetDoc");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> SaveDoc(DataSet ds)
	{
		try
		{
			using HttpClientHelper<DataSet> _help = new HttpClientHelper<DataSet>(Configuration.ApiServer, "MailSetting");
			return await _help.PostAsJsonAsync("SaveDoc", ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> DeleteDoc(string code)
	{
		try
		{
			using HttpClientHelper<string> _help = new HttpClientHelper<string>(Configuration.ApiServer, "MailSetting");
			return await _help.GetAsync("DeleteDoc", new { code });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}
}
