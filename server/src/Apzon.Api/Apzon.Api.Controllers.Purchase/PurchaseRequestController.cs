using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Services.Interfaces.Sales;

namespace Apzon.Api.Controllers.Purchase;

public class PurchaseRequestController : BaseApiController
{
	public ApzObjectTypes FormObjectTypes = ApzObjectTypes.oPurchaseRequest;

	public DataTable GetLine(int id, string FilterLine = "")
	{
		return UnitOfWork.Documents.TableLine(id, FormObjectTypes, FilterLine);
	}

	public DataTable Search([FromBody] DocumentsImpl value)
	{
		try
		{
			return UnitOfWork.Documents.Search(value, FormObjectTypes);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	[HttpPost]
	[QueueModifiedAction]
	public HttpResult SaveDataSet([FromBody] DataSet ds)
	{
		if (!Global.LoginSap)
		{
			return UnitOfWork.Documents.SaveDataSet(ds, FormObjectTypes);
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpPost]
	[QueueModifiedAction]
	public async Task<HttpResult> SaveDoc([FromBody] DocumentsImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return await unitOfWork.Documents.Save(doc, SessionUserInfo.SessionDiServer, FormObjectTypes);
			}
			return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpPost]
	[QueueModifiedAction]
	public async Task<HttpResult> SaveAsDraft([FromBody] DocumentsImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return await unitOfWork.Documents.Save(doc, SessionUserInfo.SessionDiServer, ApzObjectTypes.oDrafts, FormObjectTypes);
			}
			return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	public int GetMaxDocEntry()
	{
		return UnitOfWork.Documents.GetMaxDocEntry(FormObjectTypes);
	}

	public int GetMaxDocNum()
	{
		return UnitOfWork.Documents.GetMaxDocNum(FormObjectTypes);
	}

	public int GetMinDocEntry()
	{
		return UnitOfWork.Documents.GetMinDocEntry(FormObjectTypes);
	}

	[HttpPost]
	[QueueModifiedAction]
	public HttpResult Cancel([FromBody] int DocEntry)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			return BrokerService.GetUnitOfWork(SessionId).Documents.CancelDoc(DocEntry, SessionUserInfo.SessionDiServer, FormObjectTypes);
		}
		return UnitOfWork.Documents.CancelDoc(DocEntry, FormObjectTypes);
	}

	[HttpPost]
	[QueueModifiedAction]
	public HttpResult Close([FromBody] int DocEntry)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			return BrokerService.GetUnitOfWork(SessionId).Documents.CloseDoc(DocEntry, SessionUserInfo.SessionDiServer, FormObjectTypes);
		}
		return UnitOfWork.Documents.CloseDoc(DocEntry, FormObjectTypes);
	}

	[HttpPost]
	[QueueModifiedAction]
	public HttpResult Delete([FromBody] int DocEntry)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			return new HttpResult(Enums.MessageCode.Error, unitOfWork.Documents.RemoveDoc(DocEntry, SessionUserInfo.SessionDiServer, FormObjectTypes));
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpPost]
	public DataTable GetDocLinesByListDocEntry([FromBody] List<int> DocEntry)
	{
		return UnitOfWork.Documents.GetDocLinesByListDocEntry(DocEntry, FormObjectTypes);
	}

	[HttpPost]
	[QueueModifiedAction]
	public DataTable GetApprovalTemplates([FromBody] Tuple<DocumentsImpl, string, string> item)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new DataTable();
			}
			return BrokerService.GetUnitOfWork(SessionId).Documents.GetApprovalTemplate(item.Item1, SessionUserInfo.SessionDiServer, item.Item2, item.Item3, FormObjectTypes);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable Duplicate(int docEntry)
	{
		return UnitOfWork.Documents.Duplicate(docEntry, FormObjectTypes);
	}

	[HttpGet]
	public DataTable GetPayment(int docEntry, int objType)
	{
		return UnitOfWork.Documents.GetPayment(docEntry, FormObjectTypes);
	}

	[HttpGet]
	public DataTable CopyFrom(string type, string entry, int bplId, string username)
	{
		IDocument documents = UnitOfWork.Documents;
		int formObjectTypes = (int)FormObjectTypes;
		return documents.CopyFrom(type, entry, formObjectTypes.ToString(), bplId, username);
	}

	[HttpGet]
	public DataTable CopyFromLine(string type, string entry)
	{
		return UnitOfWork.Documents.CopyFromLine(type, entry, 1470000113.ToString());
	}

	[HttpGet]
	public DataTable GetListOrcCode(string p, DateTime? date = null)
	{
		return UnitOfWork.Commons.GetListOrcCode(p, date);
	}

	[HttpGet]
	public DataTable GetDimensionByCode(string code)
	{
		return UnitOfWork.Commons.GetDimensionByCode(code);
	}

	[HttpGet]
	public DataTable GetDimensions()
	{
		return UnitOfWork.Commons.GetDimensions();
	}

	[HttpGet]
	public DataTable GetListBPCatalog()
	{
		return UnitOfWork.Documents.GetListBpCatalog(FormObjectTypes);
	}

	[HttpGet]
	public DataTable LoadBatchNumberStruct()
	{
		return UnitOfWork.Documents.LoadBatchNumberStruct(FormObjectTypes);
	}

	[HttpGet]
	public DataTable GetBatchSerialByCode(string itemcode, string whscode, int lineNum, string barcode, string objType)
	{
		return UnitOfWork.Documents.GetBatchSerialByCode(itemcode, whscode, lineNum, barcode, objType);
	}

	[HttpGet]
	public DataTable GetBatchSerialByDocEntry(int docEntry)
	{
		return UnitOfWork.Documents.GetBatchSerialByDocEntry(docEntry, FormObjectTypes);
	}

	[HttpGet]
	public DataTable CopyBatchSerial(string fromentry, string fromtype)
	{
		return UnitOfWork.Documents.CopyBatchSerial(fromentry, fromtype);
	}

	[HttpGet]
	public DataTable LoadSeries(string usercode)
	{
		return UnitOfWork.Documents.LoadSeries(usercode, FormObjectTypes);
	}

	[HttpGet]
	public int GetDocumentNumber(int series)
	{
		return UnitOfWork.Documents.GetDocumentNumber(series, FormObjectTypes);
	}

	[HttpGet]
	public DataTable GetFreight(int docEntry)
	{
		return UnitOfWork.Documents.GetFreight(docEntry, FormObjectTypes);
	}

	[HttpGet]
	public DataTable GetFreightDefault()
	{
		return UnitOfWork.Documents.GetFreightDefault(FormObjectTypes);
	}

	[HttpGet]
	public DataTable GetTaxGroup()
	{
		return UnitOfWork.Documents.GetTaxGroup(FormObjectTypes);
	}

	[HttpGet]
	public DataTable GetFreightCode()
	{
		return UnitOfWork.Documents.GetFreightCode(FormObjectTypes);
	}

	[HttpGet]
	public DataTable GetRateByVatGroup()
	{
		return UnitOfWork.Documents.GetRateByVatGroup(FormObjectTypes);
	}

	[HttpGet]
	public DataTable GetListDepartment()
	{
		return UnitOfWork.PurchaseRequest.GetListDepartment();
	}

	[HttpGet]
	public DataTable GetListBranch()
	{
		return UnitOfWork.PurchaseRequest.GetListBranch();
	}

	[HttpGet]
	public DataTable GetListPurchaseRequest()
	{
		return UnitOfWork.PurchaseRequest.GetListPurchaseRequest();
	}

	[HttpGet]
	public DataTable GetExpnsByCode(string expnsCode)
	{
		return UnitOfWork.Documents.GetExpnsByCode(FormObjectTypes, expnsCode);
	}

	[HttpGet]
	public DataTable GetReferencedDocument(int docEntry)
	{
		return UnitOfWork.Documents.GetReferencedDocument(FormObjectTypes, docEntry);
	}

	[HttpGet]
	public DataTable ListChooseDocuments(string from, string to)
	{
		return UnitOfWork.Documents.ListChooseDocuments(FormObjectTypes, from, to);
	}

	public DataTable GetReferencedDocumentBy(int docEntry)
	{
		return UnitOfWork.Documents.GetReferencedDocumentBy(FormObjectTypes, docEntry);
	}
}
