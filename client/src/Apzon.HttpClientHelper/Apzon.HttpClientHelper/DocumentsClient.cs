using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Global;

namespace Apzon.HttpClientHelper;

internal class DocumentsClient : IDocumentsClient
{
	public ApzObjectTypes ObjectTypes { get; set; }

	public string FormController { get; set; }

	public DocumentsClient()
	{
	}

	public DocumentsClient(ApzObjectTypes type)
	{
		ObjectTypes = type;
	}

	public DataTable GetPaymentTerms(int docEntry, ApzObjectTypes type)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, GetFormController());
			return httpClientHelper.Get("GetPayment", new
			{
				docEntry = docEntry,
				objType = (int)type
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public void SetObject(ApzObjectTypes type)
	{
		ObjectTypes = type;
	}

	public async Task<DataTable> LoadBatchNumberStruct()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Sales");
			return await helper.GetAsync("LoadBatchNumberStruct", new
			{
				objType = ((int)ObjectTypes).ToString()
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetBatchSerialByCode(string itemcode, string whscode, int lineNum, string barcode)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, GetFormController());
			return await helper.GetAsync("GetBatchSerialByCode", new
			{
				itemcode = itemcode,
				whscode = whscode,
				lineNum = lineNum,
				barcode = barcode,
				objType = ((int)ObjectTypes).ToString()
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetBatchSerialByDocEntry(int docEntry)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, GetFormController());
			return httpClientHelper.Get("GetBatchSerialByDocEntry", new { docEntry });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable CopyBatchSerial(string fromentry, ApzObjectTypes fromtype)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, GetFormController());
			int num = (int)fromtype;
			return httpClientHelper.Get("CopyBatchSerial", new
			{
				fromentry = fromentry,
				fromtype = num.ToString()
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DocumentTable> CopyHeader(ApzObjectTypes fromType, ApzObjectTypes toType, string docEntry)
	{
		try
		{
			using HttpClientHelper<DocumentTable> client = new HttpClientHelper<DocumentTable>(Configuration.ApiServer, GetFormController());
			int num = (int)fromType;
			return await client.GetAsync("CopyFrom", new
			{
				type = num.ToString(),
				entry = docEntry,
				bplId = GlobalData.DefaultBranch,
				username = GlobalData.UserName
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DocumentLineTable> Copy(ApzObjectTypes fromType, ApzObjectTypes toType, string docEntry)
	{
		try
		{
			using HttpClientHelper<DocumentLineTable> client = new HttpClientHelper<DocumentLineTable>(Configuration.ApiServer, "Sales");
			int num = (int)fromType;
			string type = num.ToString();
			num = (int)toType;
			return await client.GetAsync("CopyFromLine", new
			{
				type = type,
				entry = docEntry,
				totype = num.ToString()
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> LoadSeries(string usercode)
	{
		try
		{
			using HttpClientHelper<DataTable> client = new HttpClientHelper<DataTable>(Configuration.ApiServer, GetFormController());
			return await client.GetAsync("LoadSeries", new { usercode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<int> GetDocumentNumber(int series)
	{
		try
		{
			using HttpClientHelper<DataTable> client = new HttpClientHelper<DataTable>(Configuration.ApiServer, GetFormController());
			return await client.GetIntAsync("GetDocumentNumber", new { series });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public string GetFormController()
	{
		try
		{
			switch (ObjectTypes)
			{
			case ApzObjectTypes.oOrders:
			case ApzObjectTypes.oShipping:
				return "Sales";
			case ApzObjectTypes.oDeliveryNotes:
				return "Delivery";
			case ApzObjectTypes.oQuotations:
				return "SalesQuotationForBatch";
			case ApzObjectTypes.oReturnRequest:
				return "ReturnRequest";
			case ApzObjectTypes.oReturns:
				return "Return";
			case ApzObjectTypes.oInvoices:
			case ApzObjectTypes.oReserveInvoices:
				return "ARInvoice";
			case ApzObjectTypes.oCreditNotes:
				return "ARCreditMemo";
			case ApzObjectTypes.oPurchaseRequest:
				return "PurchaseRequest";
			case ApzObjectTypes.oPurchaseQuotations:
				return "PPurchaseQuotation";
			case ApzObjectTypes.oPurchaseDeliveryNotes:
				return "GoodReceiptPo";
			case ApzObjectTypes.oGoodsReturnRequest:
				return "GoodsReturnRequest";
			case ApzObjectTypes.oPurchaseReturns:
				return "GoodsReturn";
			case ApzObjectTypes.oPurchaseInvoices:
				return "APInvoice";
			case ApzObjectTypes.oAPReserveInvoice:
				return "APReserveInvoice";
			case ApzObjectTypes.oPurchaseCreditNotes:
				return "APCreditMemo";
			case ApzObjectTypes.oPurchaseOrders:
				return "PurchaseOder";
			case ApzObjectTypes.oDrafts:
				return "DocumentDraft";
			case ApzObjectTypes.oInventoryGenEntry:
				return "GoodReceiptIt";
			case ApzObjectTypes.oInventoryGenExit:
				return "GoodsIssueIt";
			case ApzObjectTypes.oDownPayments:
				return "DownPayment";
			case ApzObjectTypes.oPurchaseDownPayments:
				return "PurchaseDownPayment";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
		return "";
	}

	public async Task<DocumentLineTable> Duplicate(int docentry)
	{
		try
		{
			using HttpClientHelper<DocumentLineTable> client = new HttpClientHelper<DocumentLineTable>(Configuration.ApiServer, GetFormController());
			return await client.GetAsync("Duplicate", new { docentry });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetExpnsByCode(string expnsCode)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, GetFormController());
			return httpClientHelper.Get("GetExpnsByCode", new { expnsCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetFreightCode()
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "PurchaseOder");
		return await helper.GetAsync("GetFreightCode");
	}

	public async Task<HttpResult> UpdateDataByService(DocumentsImpl doc)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, GetFormController());
		return await helper.PostAsync("UpdateDataByService", doc);
	}

	public async Task<HttpResult> SaveDraft(DataSet ds)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "DocumentDraft");
		return await helper.PostAsync("SaveDraft", ds);
	}

	public async Task<HttpResult> Remove(int docEntry)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, GetFormController());
		return await helper.PostAsync("Remove", docEntry);
	}

	public async Task<HttpResult> Close(int docEntry)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, GetFormController());
		return await helper.PostAsync("Close", docEntry);
	}

	public async Task<HttpResult> Cancel(int docEntry)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, GetFormController());
		return await helper.PostAsync("Cancel", docEntry);
	}

	public async Task<HttpResult> SaveDoc(DocumentsImpl doc)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, GetFormController());
		return await helper.PostAsync("SaveDoc", doc);
	}

	public async Task<HttpResult> SaveAsDraft(DocumentsImpl doc)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, GetFormController());
		return await helper.PostAsync("SaveAsDraft", doc);
	}

	public async Task<DataTable> GetLineShipTo(string cardCode)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Sales");
		return await helper.GetAsync("GetLineShipTo", new { cardCode });
	}

	public int GetDocNumByDocEntryType(int baseKey, int baseType)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Sales");
		return httpClientHelper.GetInt("GetDocNumByDocEntryType", new { baseKey, baseType });
	}

	public DataTable GetOwnerByBpUser(string cardCode, int userID)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Sales");
		return httpClientHelper.Get("GetOwnerByBpUser", new { cardCode, userID });
	}

	public async Task<DocumentTable> Search(DocumentsImpl value)
	{
		try
		{
			using HttpClientHelper<DocumentTable> helper = new HttpClientHelper<DocumentTable>(Configuration.ApiServer, GetFormController());
			return await helper.PostAsync("Search", value);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DocumentLineTable> GetLine(int id, string FilterLine = "")
	{
		try
		{
			using HttpClientHelper<DocumentLineTable> helper = new HttpClientHelper<DocumentLineTable>(Configuration.ApiServer, GetFormController());
			return await helper.GetAsync("GetLine", new { id, FilterLine });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public int GetDocTypeByDraftKey(int draftKey)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, GetFormController());
			return httpClientHelper.GetInt("GetDocTypeByDraftKey", new { draftKey });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<HttpResult> SaveDocument(DocumentsImpl doc)
	{
		using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, GetFormController());
		return await helper.PostAsync("SaveDocument", doc);
	}

	public async Task<DataTable> GetDownPaymentToDraw(ApzObjectTypes objType, string cardCode, string docCur)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Sales");
		return await helper.GetAsync("GetDownPaymentToDraw", new { objType, cardCode, docCur });
	}

	public async Task<DataTable> GetDownPaymentDrawed(ApzObjectTypes objType, int docEntry)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Sales");
		return await helper.GetAsync("GetDownPaymentDrawed", new { objType, docEntry });
	}

	public DataTable GetReferencedDocument(int docEntry)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, GetFormController());
		return httpClientHelper.Get("GetReferencedDocument", new { docEntry });
	}

	public async Task<DataTable> ListChooseDocuments(string from, string to)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, GetFormController());
		return await helper.GetAsync("ListChooseDocuments", new { from, to });
	}

	public async Task<DataTable> GetReferencedDocumentBy(int docEntry)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, GetFormController());
		return await helper.GetAsync("GetReferencedDocumentBy", new { docEntry });
	}
}
