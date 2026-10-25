using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.MasterData;

namespace Apzon.HttpClientHelper;

internal class ChartOfAccountClient : IChartOfAccountClient
{
	private const string Controller = "ChartOfAccount";

	public async Task<string> Save(List<ChartOfAccountsImpl> pr)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "ChartOfAccount");
			return await helper.PostAsync("Save", pr);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<DataTable> GetStruct()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ChartOfAccount");
			return await helper.GetAsync("GetStruct");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}
}
