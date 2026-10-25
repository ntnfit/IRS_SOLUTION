using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.HttpClientHelper;

internal class GoodsReceiptPOByBarcodeClient : IGoodsReceiptPOByBarcodeClient
{
	private const string FormController = "GoodReceiptPo";

	public async Task<HttpResult> AddNew(DocumentsImpl doc)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "GoodReceiptPo");
			return await helper.PostAsync("AddNew", doc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataSetResult> GetDataByBarcode(string barcode, string userName, int branch)
	{
		try
		{
			using HttpClientHelper<DataSetResult> helper = new HttpClientHelper<DataSetResult>(Configuration.ApiServer, "GoodReceiptPo");
			return await helper.GetAsync("GetDataByBarcode", new { barcode, userName, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSetResult(ex.Message);
		}
	}

	public async Task<HttpResult> GetCurrentRate(DataTable dtSearch)
	{
		using HttpClientHelper<HttpResult> hp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "GoodReceiptPo");
		return await hp.PostAsync("GetCurrentRate", dtSearch);
	}
}
