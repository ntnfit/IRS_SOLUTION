using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;

namespace Apzon.HttpClientHelper;

internal class ActivityCalendarClient : IActivityCalendarClient
{
	private const string FormController = "ActivityCalendar";

	public async Task<DataTable> GetDoc()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ActivityCalendar");
			return await helper.GetDataTableAsync("GetAll");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetUsers()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ActivityCalendar");
			return await helper.GetDataTableAsync("GetUsers");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetDocByUser(int attendUser)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ActivityCalendar");
			return await helper.GetDataTableAsync("GetDocByUser", new { attendUser });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetDocByEmpl(int attendEmpl)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ActivityCalendar");
			return await helper.GetDataTableAsync("GetDocByEmpl", new { attendEmpl });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetActivityByCode(int clgCode)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ActivityCalendar");
			return httpClientHelper.GetDataTable("GetByCode", new { clgCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public string SaveActivityInterval(DataTable dt)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "ActivityCalendar");
			return httpClientHelper.Post("SaveActivityInterval", dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public string SaveActivityStatus(Tuple<int, int> tpl)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "ActivityCalendar");
			return httpClientHelper.Post("SaveActivityStatus", tpl);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public string SaveActivityAction(Tuple<int, string> tpl)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "ActivityCalendar");
			return httpClientHelper.Post("SaveActivityAction", tpl);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetUnlockedUsers()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ActivityCalendar");
			return await helper.GetDataTableAsync("GetUnlockedUsers");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetActiveEmployees()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ActivityCalendar");
			return await helper.GetDataTableAsync("GetActiveEmployees");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}
}
