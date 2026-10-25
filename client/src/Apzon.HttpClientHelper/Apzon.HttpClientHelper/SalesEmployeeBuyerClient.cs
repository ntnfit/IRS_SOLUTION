using System;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;

namespace Apzon.HttpClientHelper;

internal class SalesEmployeeBuyerClient : ISalesEmployeeBuyerClient
{
	private const string FormController = "SalesEmployeeBuyer";

	public DataTable GetListEmployee()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesEmployeeBuyer");
			return httpClientHelper.Get("GetListEmployee");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public string Save(DataTable dt)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "SalesEmployeeBuyer");
			return httpClientHelper.Post("Save", dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetOCOG()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesEmployeeBuyer");
			return httpClientHelper.Get("GetOCOG");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetOSLP()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesEmployeeBuyer");
			return httpClientHelper.Get("GetOSLP");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public string DeleteRow(int slpCode)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "SalesEmployeeBuyer");
			return httpClientHelper.Post("DeleteRow", slpCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public string SetDefaultCode(int slpCode)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "SalesEmployeeBuyer");
			return httpClientHelper.Post("SetDefaultCode", slpCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public int GetDefaultCode()
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "SalesEmployeeBuyer");
			return httpClientHelper.GetInt("GetDefaultCode");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public bool IsExistsSlpCode(int slpCode)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesEmployeeBuyer");
			return httpClientHelper.GetBool("IsExistsSlpCode", new { slpCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetStructure()
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesEmployeeBuyer");
		return httpClientHelper.Get("GetStructure");
	}
}
