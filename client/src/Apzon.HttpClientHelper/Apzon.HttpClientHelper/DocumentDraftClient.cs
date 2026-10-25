using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;

namespace Apzon.HttpClientHelper;

internal class DocumentDraftClient : IDocumentDraftClient
{
	private const string FormController = "DocumentDraft";

	public async Task<string> CheckIsIns(int docEntry)
	{
		try
		{
			using HttpClientHelper<string> heper = new HttpClientHelper<string>(Configuration.ApiServer, "DocumentDraft");
			return await heper.GetAsync("CheckIsIns", new { docEntry });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}
}
