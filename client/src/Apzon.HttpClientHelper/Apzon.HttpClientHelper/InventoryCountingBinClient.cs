using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.HttpClientHelper;

internal class InventoryCountingBinClient : IInventoryCountingBinClient
{
	private const string FormController = "InventoryCountingBin";

	public async Task<DataSet> TransferGet(string batchNo, string bin)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "InventoryCountingBin");
			return await helper.GetAsync("TransferGet", new { batchNo, bin });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> LoadAllBin(string itemCode, string whsCode, int batchSerialAbs, int tranType, string binCode)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "InventoryCountingBin");
			return await helper.GetAsync("LoadAllBin", new { itemCode, whsCode, batchSerialAbs, tranType, binCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataSet> GetBinInstock(string bin, string period)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "InventoryCountingBin");
			return await helper.GetAsync("GetBinInstock", new { bin, period });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetPeriods()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "InventoryCountingBin");
			return await helper.GetAsync("GetPeriods");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> CheckBatch(string batch, string bin, string period)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "InventoryCountingBin");
			return await helper.GetAsync("CheckBatch", new { batch, bin, period });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataSet> GetById(int id)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "InventoryCountingBin");
			return await helper.GetAsync("GetById", new { id });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataSet> GetCountingReport(string period, string bin)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "InventoryCountingBin");
			return await helper.GetAsync("GetCountingReport", new { period, bin });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<HttpResult> Posting(DocumentsImpl oTransfer, DocumentsImpl oReceipt, DocumentsImpl oIssue)
	{
		try
		{
			Tuple<DocumentsImpl, DocumentsImpl, DocumentsImpl> pr = new Tuple<DocumentsImpl, DocumentsImpl, DocumentsImpl>(oTransfer, oReceipt, oIssue);
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "InventoryCountingBin");
			return await helper.PostAsync("Posting", pr);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task<DataSetResult> GetDataByBarcode(string barcode, string whs, string fromWhs, int requestId)
	{
		try
		{
			using HttpClientHelper<DataSetResult> helper = new HttpClientHelper<DataSetResult>(Configuration.ApiServer, "InventoryCountingBin");
			return await helper.GetAsync("GetDataByBarcode", new { barcode, whs, fromWhs, requestId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSetResult(ex.Message);
		}
	}

	public async Task<DataTableResult> GetWarehouseRequest(string docEntry)
	{
		try
		{
			using HttpClientHelper<DataTableResult> helper = new HttpClientHelper<DataTableResult>(Configuration.ApiServer, "InventoryCountingBin");
			return await helper.GetAsync("GetWarehouseRequest", new { docEntry });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTableResult(ex.Message);
		}
	}
}
