using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;

namespace Apzon.HttpClientHelper;

internal class SalaryInfoAdvanceClient : ISalaryInfoAdvanceClient
{
	private const string FormController = "SalaryInfoAdvance";

	public async Task<string> SaveDoc(DataTable data)
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalaryInfoAdvance");
			return await _help.PostAsJsonAsync("SaveDoc", data);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> SearchByPeriod(DataTable dtSearch)
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalaryInfoAdvance");
			return await _help.PostAsync("SearchByPeriod", dtSearch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetAdvancedInfo(int empId)
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalaryInfoAdvance");
			return await hp.GetAsync("GetAdvancedInfo", new { empId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetWorkPeriodsByBranch(int branch)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalaryInfoAdvance");
			return httpClientHelper.Get("GetWorkPeriodsByBranch", new { branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> DeleteDoc(int lineNum, int empId)
	{
		try
		{
			using HttpClientHelper<string> _help = new HttpClientHelper<string>(Configuration.ApiServer, "SalaryInfoAdvance");
			return await _help.GetAsync("DeleteDoc", new { lineNum, empId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}
}
