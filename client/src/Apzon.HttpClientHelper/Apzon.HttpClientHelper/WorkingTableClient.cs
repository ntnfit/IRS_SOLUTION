using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.HttpClientHelper;

internal class WorkingTableClient : IWorkingTableClient
{
	private const string Controller = "WorkingTable";

	public async Task<HttpResult> GetDraftApprovalStatus(int draftKey)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "WorkingTable");
		return await helper.GetAsync("GetDraftApprovalStatus", new { draftKey });
	}

	public async Task<HttpResult> SaveDraftToDocument(DocumentsImpl doc)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "WorkingTable");
		return await helper.PostAsync("SaveDraftToDocument", doc);
	}

	public async Task<string> SetStatus(DataTable dt)
	{
		try
		{
			using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, "WorkingTable");
			return await hp.PostAsync("SetStatus", dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<string> CancelRequest(int wddCode)
	{
		try
		{
			using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, "WorkingTable");
			return await hp.PostAsync("CancelApprovalRequest", wddCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}
}
