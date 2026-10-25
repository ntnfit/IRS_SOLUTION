using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaStockTransferRequestRepository : HanaStockTransferRepository, IStockTransferRequest, IRepository<DocumentsImpl>
{
	public HanaStockTransferRequestRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	public async Task<HttpResult> Save(DocumentsImpl doc, bool isDraft, string sessionId)
	{
		try
		{
			if (isDraft && doc.draftKey > 0)
			{
				doc.DocEntry = doc.draftKey;
			}
			if (doc.AttachmentFiles != null && doc.AttachmentFiles.Count != 0)
			{
				HttpResult httpResult = SaveAttachment(doc, sessionId, doc.DocEntry <= 0);
				if (httpResult.MessageCode != Enums.MessageCode.Success)
				{
					return httpResult;
				}
				doc.AtcEntry = Function.ParseInt(httpResult.Content);
			}
			if (doc.DocEntry == 0)
			{
				if (doc.Document_ApprovalRequests != null && doc.Document_ApprovalRequests.Count > 0)
				{
					return await _unitOfWork.Documents.AddApprovalRequest(doc, sessionId, ApzObjectTypes.oInventoryTransferRequest);
				}
				if (doc.IsDraft && !isDraft)
				{
					doc.DocObjectCode = ApzObjectTypes.oInventoryTransferRequest;
					return SaveDraftToDocument(doc, sessionId);
				}
				return Add(doc, isDraft, sessionId);
			}
			return Update(doc, isDraft, sessionId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected HttpResult Update(DocumentsImpl doc, bool isDraft, string sessionId)
	{
		try
		{
			Logging.Write("WATCH", "Start update transfer request");
			string xmlObject = MappingXml(ApzObjectTypes.oInventoryTransferRequest, doc, isDraft, EnumCommandType.Update);
			HttpResult httpResult = DataService.UpdateObject(isDraft ? ApzObjectTypes.oStockTransferDraft : ApzObjectTypes.oInventoryTransferRequest, sessionId, xmlObject, doc.DocEntry);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", "Update Transfer Request Error", httpResult.Message);
				return new HttpResult(Enums.MessageCode.None, httpResult.Message);
			}
			int num = Function.ParseInt(httpResult.Content);
			Logging.Write("WATCH", "Update Transfer Request \t" + num, doc.DocEntry.ToString());
			return new HttpResult(Enums.MessageCode.Success, "", num);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected HttpResult Add(DocumentsImpl doc, bool isDraft, string sessionId)
	{
		try
		{
			Logging.Write("WATCH", "Start add transfer request");
			string xmlObject = MappingXml(ApzObjectTypes.oInventoryTransferRequest, doc, isDraft);
			HttpResult httpResult = DataService.AddObject(isDraft ? ApzObjectTypes.oStockTransferDraft : ApzObjectTypes.oInventoryTransferRequest, sessionId, xmlObject);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", "Add Transfer Error", httpResult.Message);
				return new HttpResult(Enums.MessageCode.None, httpResult.Message);
			}
			int num = Function.ParseInt(httpResult.Content);
			Logging.Write("WATCH", "Add Transfer success \t" + num);
			return new HttpResult(Enums.MessageCode.Success, "", num);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override string Close(int docEntry, string sessionId)
	{
		try
		{
			HttpResult httpResult = DataService.CloseObject(ApzObjectTypes.oInventoryTransferRequest, sessionId, docEntry);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "";
	}
}
