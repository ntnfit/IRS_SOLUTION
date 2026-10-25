using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Production;

namespace Apzon.HttpClientHelper;

internal class ProductionOrdersClient : IProductionOrdersClient
{
	private const string FormController = "ProductionOder";

	public async Task<HttpResult> SaveList(List<ProductionOder_Impl_OWOR> oDocs)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ProductionOder");
			return await helper.PostAsync("SaveList", oDocs);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task<HttpResult> UpdateStatus(List<ProductionOder_Impl_OWOR> oDocs)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ProductionOder");
			return await helper.PostAsync("UpdateStatus", oDocs);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task<HttpResult> ReceiptForBatch(Tuple<int, DocumentsImpl, DocumentsImpl, string, string> model)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ProductionOder");
			return await helper.PostAsync("ReceiptForBatch", model);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task<HttpResult> PackingBatch(Tuple<DocumentsImpl, DocumentsImpl, ProductionOder_Impl_OWOR> model)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ProductionOder");
			return await helper.PostAsync("PackingBatch", model);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task<HttpResult> ConfirmBatch(Tuple<DocumentsImpl, DocumentsImpl, ProductionOder_Impl_OWOR> model)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ProductionOder");
			return await helper.PostAsync("ConfirmBatch", model);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task<HttpResult> SaveProductionPlan(List<ProductionOder_Impl_OWOR> oDocs, DocumentsImpl tran, DataTable dtOperation)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ProductionOder");
			Tuple<List<ProductionOder_Impl_OWOR>, DocumentsImpl, DataTable> pr = new Tuple<List<ProductionOder_Impl_OWOR>, DocumentsImpl, DataTable>(oDocs, tran, dtOperation);
			return await helper.PostAsync("SaveProductionPlan", pr);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<int> GetDocNum(int series)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "ProductionOder");
			return await helper.GetIntAsync("GetDocNum", new { series });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetByBaseEntry(int baseEntry, ApzObjectTypes objectType)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionOder");
			return await helper.GetAsync("GetByBaseEntry", new { baseEntry, objectType });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<HttpResult> SaveDataSet(DataSet ds)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ProductionOder");
		return await helper.PostAsync("SaveDataSet", ds);
	}

	public async Task<DataTable> GetRecord(string position, string docEntry)
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionOder");
		return await hp.GetAsync("GetRecord", new { position, docEntry });
	}

	public async Task<DataTable> FindDataNextLast(int id, int type)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionOder");
			return await helper.GetAsync("FindDataNextLast", new { id, type });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetADoc(int docEntry)
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionOder");
		return await hp.GetAsync("GetADoc", new { docEntry });
	}

	public async Task<DataSet> SearchLine(ProductionOder_LineImpl_WOR1 line)
	{
		using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ProductionOder");
		return await hp.PostAsync("SearchLine", line);
	}

	public DataTable GetItemDetails(string type, string code)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionOder");
		return httpClientHelper.Get("GetItemDetails", new { type, code });
	}

	public async Task<string> CancelDoc(int _currentDocEntry)
	{
		using HttpClientHelper<HttpResult> hp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ProductionOder");
		return (await hp.PostAsync("CancelDoc", _currentDocEntry)).Message;
	}

	public async Task<DataTable> Commons_GetDataBrach(string userCode)
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
		return await hp.GetAsync("GetDataBrach", new { userCode });
	}

	public DataTable Commons_GetSeries(int ObjType)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
		return httpClientHelper.Get("GetSeries", new { ObjType });
	}

	public async Task<DataTable> ShowUsers()
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionOder");
		return await hp.GetAsync("ShowUsers");
	}

	public async Task<DataTable> GetActiveDimensionCount()
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionOder");
		return await hp.GetAsync("GetActiveDimensionCount");
	}

	public async Task<DataTable> GetItem_ITT1(BillOfMasterials_LineImpl_ITT1 line)
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionOder");
		return await hp.PostAsync("GetItem_ITT1", line);
	}

	public DataTable GetOITWQuantity(string code, string whsCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionOder");
		return httpClientHelper.Get("GetOITWQuantity", new { code, whsCode });
	}

	public async Task<DataTable> GetItemNameUoM(string itemCode)
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionOder");
		return await hp.GetAsync("GetItemNameUoM", new { itemCode });
	}

	public DataTable GetSalesOrderEntry(int docNum)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionOder");
		return httpClientHelper.Get("GetSalesOrderEntry", new { docNum });
	}

	public async Task<HttpResult> SaveDoc(ProductionOder_Impl_OWOR doc)
	{
		using HttpClientHelper<HttpResult> hp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ProductionOder");
		return await hp.PostAsync("SaveDoc", doc);
	}

	public async Task<DataTable> Search(DataSet ds)
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionOder");
		return await hp.PostAsync("Search", ds);
	}

	public async Task<DataTable> GetIsIssued(int docEntry)
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionOder");
		return await hp.GetAsync("GetIsIssued", new { docEntry });
	}

	public async Task<DataSet> GetIssueComponents(int docEntry)
	{
		using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ProductionOder");
		return await hp.GetAsync("GetIssueComponents", new { docEntry });
	}

	public async Task<DataSet> GetTransferComponents(int docEntry, double parentQty)
	{
		using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ProductionOder");
		return await hp.GetAsync("GetTransferComponents", new { docEntry, parentQty });
	}

	public async Task<HttpResult> CheckBaseProduction(int entry, ApzObjectTypes objtype)
	{
		using HttpClientHelper<HttpResult> hp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "ProductionOder");
		return await hp.GetAsync("CheckBaseProduction", new { entry, objtype });
	}

	public async Task<DataSetResult> GetDataByBarcode(string barcode, string userName, int branch)
	{
		try
		{
			using HttpClientHelper<DataSetResult> helper = new HttpClientHelper<DataSetResult>(Configuration.ApiServer, "ProductionOder");
			return await helper.GetAsync("GetDataByBarcode", new { barcode, userName, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSetResult(ex.Message);
		}
	}

	public async Task<DataTable> CheckByDocNum(string type, int docNum)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionOder");
		return await http.GetAsync("CheckByDocNum", new { type, docNum });
	}

	public DataTable GetReferencedDocument(string FromDocEntry)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionOder");
		return httpClientHelper.Get("GetReferencedDocument", new { FromDocEntry });
	}
}
