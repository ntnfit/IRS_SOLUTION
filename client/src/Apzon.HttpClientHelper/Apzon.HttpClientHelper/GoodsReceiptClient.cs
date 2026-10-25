using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.HttpClientHelper;

internal class GoodsReceiptClient : IGoodsReceiptClient
{
	private const string FormController = "GoodReceiptIt";

	public async Task<HttpResult> SaveDoc(DocumentsImpl doc)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "GoodReceiptIt");
		return await helper.PostAsync("SaveDoc", doc);
	}

	public async Task<HttpResult> AddNew(DocumentsImpl doc)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "GoodReceiptIt");
			return await helper.PostAsync("AddNew", doc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<HttpResult> SaveDocWithUdo(DocumentsImpl doc, DataSet ds)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "GoodReceiptIt");
			return await helper.PostAsync("SaveDocWithUdo", new Tuple<DocumentsImpl, DataSet>(doc, ds));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
