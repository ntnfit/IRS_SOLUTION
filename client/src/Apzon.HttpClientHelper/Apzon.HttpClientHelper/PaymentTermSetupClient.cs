using System;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;

namespace Apzon.HttpClientHelper;

internal class PaymentTermSetupClient : IPaymentTermSetup
{
	private const string FormController = "PaymentTermSetUp";

	public string SaveDoc(DataTable dt)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "PaymentTermSetUp");
			return httpClientHelper.Post("SaveDoc", dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable LoadDoc()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "PaymentTermSetUp");
			return httpClientHelper.Get("LoadDoc");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public string DeleteDoc(int groupNum)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "PaymentTermSetUp");
			return httpClientHelper.Post("DeleteDoc", groupNum);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetPaymentTermInfo(int groupNum)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "PaymentTermSetUp");
			return httpClientHelper.Get("GetPaymentTermInfo", new { groupNum });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}
}
