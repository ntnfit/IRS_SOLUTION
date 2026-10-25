using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

internal class ChangeLogClient : IChangeLogClient
{
	private const string Controller = "ChangeLog";

	public async Task<DataTable> GetChangeLog(GetChangeLogParams pr)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ChangeLog");
			return await helper.PostAsync("GetChangeLog", pr);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<DataTable> GetChangeLogDifferences(GetChangeLogParams pr)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ChangeLog");
			return await helper.PostAsync("GetChangeLogDifferences", pr);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}
}
