using System;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;

namespace Apzon.HttpClientHelper;

internal class CostCentersClient : ICostCenters
{
	private const string FormController = "CostCenters";

	public DataTable Search(DataTable dtSearch)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CostCenters");
			return httpClientHelper.Post("Search", dtSearch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public string DeleteCostCenters(string prcCode)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "CostCenters");
			return httpClientHelper.Post("DeleteCostCenters", prcCode);
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
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "CostCenters");
			return httpClientHelper.Post("Save", dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataSet LoadComboBox()
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CostCenters");
			return httpClientHelper.Get("LoadComboBox");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public bool IsExistsCostCenters(string prcCode)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CostCenters");
			return httpClientHelper.GetBool("IsExistsCostCenters", new { prcCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}
}
