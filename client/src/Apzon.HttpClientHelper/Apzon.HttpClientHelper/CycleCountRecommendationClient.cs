using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;

namespace Apzon.HttpClientHelper;

internal class CycleCountRecommendationClient : ICycleCountRecommendationClient
{
	private const string FormController = "CycleCountRecommendation";

	public async Task<DataTable> GetListCycleCode()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CycleCountRecommendation");
			return await helper.GetAsync("GetListCycleCode");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetListItemCode()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CycleCountRecommendation");
			return await helper.GetAsync("GetListItemCode");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetWarehouse()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CycleCountRecommendation");
		return await http.GetAsync("GetWarehouse");
	}

	public async Task<DataTable> GetListDataLine(string whs, int fromCycleCode, int toCycleCode, string fromItemCode, string toItemCode)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CycleCountRecommendation");
			return await helper.GetAsync("GetListDataLine", new { whs, fromCycleCode, toCycleCode, fromItemCode, toItemCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}
}
