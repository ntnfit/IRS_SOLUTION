using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.HttpClientHelper;

internal class ReceiptIssueBinClient : IReceiptIssueBin
{
	private const string FormController = "ReceiptIssueBin";

	public async Task<DataSet> GetData(string batch, string bin, ApzObjectTypes objtype)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ReceiptIssueBin");
			return await helper.GetAsync("GetData", new
			{
				batchNo = batch,
				bin = bin,
				objtype = objtype
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<HttpResult> SaveDoc(DocumentsImpl doc, ApzObjectTypes formObjectType)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ReceiptIssueBin");
			return await helper.PostAsync("SaveDoc", new Tuple<DocumentsImpl, ApzObjectTypes>(doc, formObjectType));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}
}
