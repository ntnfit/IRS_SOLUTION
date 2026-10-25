using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;

namespace Apzon.HttpClientHelper;

internal class DistributeRuleClient : IDistributeRuleClient
{
	private const string FormController = "DistributeRule";

	public async Task<DataTable> GetStruct()
	{
		try
		{
			using HttpClientHelper<DataTable> help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DistributeRule");
			return await help.GetAsync("GetStruct");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<DataTable> GetStructRow()
	{
		try
		{
			using HttpClientHelper<DataTable> help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DistributeRule");
			return await help.GetAsync("GetStructRow");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}
}
