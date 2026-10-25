using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;

namespace Apzon.HttpClientHelper;

internal class ExternalRoutingClient : IExternalRoutingClient
{
	private const string FormController = "ExternalRouting";

	public async Task<DataTable> GetAllRoutings()
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ExternalRouting");
			return await hp.GetAsync("GetAllRoutings");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataSet> GetLines(int absEntry)
	{
		try
		{
			using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ExternalRouting");
			return await hp.GetAsync("GetLines", new { absEntry });
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
			using HttpClientHelper<DataSet> _help = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ExternalRouting");
			return await _help.PostAsJsonAsync("SaveDoc", ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> DeleteDoc(int absEntry)
	{
		try
		{
			using HttpClientHelper<string> _help = new HttpClientHelper<string>(Configuration.ApiServer, "ExternalRouting");
			return await _help.GetAsync("DeleteDoc", new { absEntry });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}
}
