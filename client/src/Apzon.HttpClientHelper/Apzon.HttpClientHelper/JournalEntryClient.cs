using System;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Financials;

namespace Apzon.HttpClientHelper;

internal class JournalEntryClient : IJournalEntryClient
{
	private const string Controller = "JournalEntries";

	public async Task<DataTable> GetDoc(int transId)
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "JournalEntries");
			return await http.GetAsync("GetDoc", new { transId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<DataTable> GetLines(int transId)
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "JournalEntries");
			return await http.GetAsync("GetLines", new { transId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<HttpResult> SaveDoc(JournalEntriesImpl doc)
	{
		try
		{
			using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "JournalEntries");
			return await http.PostAsync("SaveDoc", doc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<HttpResult> SaveDoc(JournalEntriesImpl doc, string sessionId)
	{
		try
		{
			using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "JournalEntries");
			HttpResult httpResult = await http.PostAsync("SaveDoc", doc, sessionId);
			Logging.Write($"Save Journal Entry - {httpResult.Content} - {httpResult.Message}");
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<HttpResult> CancelJE(int transId)
	{
		using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "JournalEntries");
		return await http.PostAsync("CancelJE", transId);
	}
}
