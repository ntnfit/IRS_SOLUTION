using System;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.HanaImplements;

public class HanaStockTransferRepository : StockTransferRepository
{
	public HanaStockTransferRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	protected override HttpResult SaveAttachment(DocumentsImpl doc, string sessionId, bool addNew)
	{
		if (addNew || doc.AtcEntry <= 0)
		{
			return _unitOfWork.Attachment2.Add(doc.AttachmentFiles, sessionId);
		}
		return _unitOfWork.Attachment2.Update(doc.AtcEntry, doc.AttachmentFiles, sessionId);
	}

	protected override HttpResult Add(DocumentsImpl doc, string sessionId, bool isDraft)
	{
		try
		{
			string text = MappingXml(ApzObjectTypes.oStockTransfer, doc, isDraft);
			Logging.Write("WATCH", "Start Add Transfer...");
			HttpResult httpResult = DataService.ProcessJson(isDraft ? ApzObjectTypes.oStockTransferDraft : ApzObjectTypes.oStockTransfer, sessionId, text);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", "Add Transfer Error", httpResult.Message);
				Logging.Write("WATCH", text);
				return new HttpResult(Enums.MessageCode.Error, httpResult.Message);
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

	protected override HttpResult Update(DocumentsImpl doc, string sessionId, bool isDraft)
	{
		try
		{
			string text = MappingXml(ApzObjectTypes.oStockTransfer, doc, isDraft, EnumCommandType.Update);
			Logging.Write("WATCH", "Start update transfer");
			HttpResult httpResult = DataService.ProcessJson(isDraft ? ApzObjectTypes.oStockTransferDraft : ApzObjectTypes.oStockTransfer, sessionId, text, Enums.HttpAction.Update, doc.DocEntry);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", "Update Transfer Error", httpResult.Message);
				Logging.Write("WATCH", text);
				return new HttpResult(Enums.MessageCode.Error, httpResult.Message);
			}
			int num = Function.ParseInt(httpResult.Content);
			Logging.Write("WATCH", "Update Transfer \t" + num);
			return new HttpResult(Enums.MessageCode.Success, "", num);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult SaveDraftToDocument(int docEntry, string sessionId)
	{
		try
		{
			HttpResult httpResult = DataService.ProcessJson(ApzObjectTypes.oStockTransferDraft, sessionId, null, Enums.HttpAction.SaveDraftToDocument, docEntry);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult;
			}
			return new HttpResult(Enums.MessageCode.Success, "", Function.ParseInt(httpResult.Content));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult SaveDraftToDocument(DocumentsImpl doc, string sessionId)
	{
		try
		{
			string xmlObject = MappingXml(doc.DocObjectCode, doc, isDraft: false);
			HttpResult httpResult = DataService.AddObject(doc.DocObjectCode, sessionId, xmlObject);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult;
			}
			int num = Function.ParseInt(httpResult.Content);
			if (doc.DocEntry > 0)
			{
				string tableByApzObjectType = Function.GetTableByApzObjectType(doc.DocObjectCode);
				Logging.Write("WATCH", $"Remove document from approval process- docEntry {doc.DocEntry} - object type {doc.DocObjectCode}");
				ExecuteNonQuery("UPDATE " + tableByApzObjectType + " SET \"WddStatus\"='-' WHERE \"DocEntry\" =" + doc.DocEntry, CommandType.Text);
			}
			Logging.Write("WATCH", $"Add new document - {doc.DocObjectCode} - DocEntry - {num}");
			try
			{
				if (!ServiceUnitOfWork.Documents.SaveDraftToDocument(doc.DocObjectCode, num, doc.draftKey))
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "", "");
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			}
			return new HttpResult(Enums.MessageCode.Success, "", num);
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
	}

	public override string Cancel(int docEntry, string sessionId)
	{
		try
		{
			return DataService.ProcessJson(ApzObjectTypes.oStockTransfer, sessionId, "", Enums.HttpAction.Cancel, docEntry).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override string MappingXml(ApzObjectTypes objectType, DocumentsImpl model, bool isDraft, EnumCommandType type = EnumCommandType.Add)
	{
		string text = "{ ";
		if (isDraft)
		{
			text += Function.GetJsonString("DocObjectCode", model.ObjType);
		}
		if (model.DocDate > new DateTime(1900, 1, 1))
		{
			text += Function.GetJsonString("DocDate", model.DocDate.ToString("yyyyMMdd"));
		}
		if (model.DocDueDate > new DateTime(1900, 1, 1))
		{
			text += Function.GetJsonString("DueDate", model.DocDueDate.ToString("yyyyMMdd"));
		}
		if (!string.IsNullOrEmpty(model.CardCode))
		{
			text += Function.GetJsonString("CardCode", Function.ToString(model.CardCode));
		}
		if (!string.IsNullOrEmpty(model.CardName))
		{
			text += Function.GetJsonString("CardName", Function.ToString(model.CardName));
		}
		if (!string.IsNullOrEmpty(model.Address))
		{
			text += Function.GetJsonString("Address", Function.ToString(model.Address));
		}
		if (!string.IsNullOrEmpty(model.Ref1))
		{
			text += Function.GetJsonString("Reference1", Function.ToString(model.Ref1));
		}
		if (!string.IsNullOrEmpty(model.Ref2))
		{
			text += Function.GetJsonString("Reference2", Function.ToString(model.Ref2));
		}
		if (!string.IsNullOrEmpty(model.Comments))
		{
			text += Function.GetJsonString("Comments", Function.ToString(model.Comments));
		}
		if (!string.IsNullOrEmpty(model.JrnlMemo))
		{
			text += Function.GetJsonString("JournalMemo", Function.ToString(model.JrnlMemo));
		}
		if (Function.ParseInt(model.GroupNum) != 0)
		{
			text += Function.GetJsonString("PriceList", model.GroupNum);
		}
		if (!model.SlpCode.Equals(0))
		{
			text += Function.GetJsonString("SalesPersonCode", model.SlpCode);
		}
		if (!string.IsNullOrEmpty(model.Filler))
		{
			text += Function.GetJsonString("FromWarehouse", Function.ToString(model.Filler));
		}
		if (!string.IsNullOrEmpty(model.ToWhsCode))
		{
			text += Function.GetJsonString("ToWarehouse", Function.ToString(model.ToWhsCode));
		}
		if (model.TaxDate > new DateTime(1900, 1, 1))
		{
			text += Function.GetJsonString("TaxDate", model.TaxDate.ToString("yyyyMMdd"));
		}
		text = ((model.AtcEntry == 0 || model.AttachmentFiles == null || model.AttachmentFiles.Count <= 0) ? (text + Function.GetJsonString("AttachmentEntry", null)) : (text + Function.GetJsonString("AttachmentEntry", Function.ParseInt(model.AtcEntry))));
		text += DataService.MappingUdf(model.UserFields);
		if (model.Lines != null && model.Lines.Count > 0)
		{
			text += "\"StockTransferLines\":[ ";
			for (int i = 0; i < model.Lines.Count; i++)
			{
				text += "{ ";
				Document_LinesImpl document_LinesImpl = model.Lines[i];
				if (document_LinesImpl.LineNum >= 0)
				{
					text += Function.GetJsonString("LineNum", document_LinesImpl.LineNum);
				}
				if (!string.IsNullOrEmpty(document_LinesImpl.ItemCode))
				{
					text += Function.GetJsonString("ItemCode", Function.ToString(document_LinesImpl.ItemCode));
				}
				text += Function.GetJsonString("Quantity", document_LinesImpl.Quantity);
				if (!document_LinesImpl.Price.Equals(0.0) && type == EnumCommandType.Add)
				{
					text += Function.GetJsonString("Price", document_LinesImpl.Price);
				}
				if (!document_LinesImpl.PriceBefDi.Equals(0.0) || type == EnumCommandType.Update)
				{
					text += Function.GetJsonString("UnitPrice", document_LinesImpl.PriceBefDi);
				}
				if (!string.IsNullOrEmpty(document_LinesImpl.Currency))
				{
					text += Function.GetJsonString("Currency", document_LinesImpl.Currency);
				}
				if (!document_LinesImpl.Rate.Equals(0.0))
				{
					text += Function.GetJsonString("Rate", document_LinesImpl.Rate);
				}
				if (!document_LinesImpl.DiscPrcnt.Equals(0.0) || type == EnumCommandType.Update)
				{
					text += Function.GetJsonString("DiscountPercent", document_LinesImpl.DiscPrcnt);
				}
				if (!string.IsNullOrEmpty(document_LinesImpl.WhsCode))
				{
					text += Function.GetJsonString("WarehouseCode", Function.ToString(document_LinesImpl.WhsCode));
				}
				if (!string.IsNullOrEmpty(document_LinesImpl.FromWhsCod))
				{
					text += Function.GetJsonString("FromWarehouseCode", Function.ToString(document_LinesImpl.FromWhsCod));
				}
				if (!string.IsNullOrEmpty(document_LinesImpl.Project))
				{
					text += Function.GetJsonString("ProjectCode", Function.ToString(document_LinesImpl.Project));
				}
				if (!document_LinesImpl.Factor1.Equals(0.0))
				{
					text += Function.GetJsonString("Factor", document_LinesImpl.Factor1);
				}
				if (!document_LinesImpl.Factor2.Equals(0.0))
				{
					text += Function.GetJsonString("Factor2", document_LinesImpl.Factor2);
				}
				if (!document_LinesImpl.Factor3.Equals(0.0))
				{
					text += Function.GetJsonString("Factor3", document_LinesImpl.Factor3);
				}
				if (!document_LinesImpl.Factor4.Equals(0.0))
				{
					text += Function.GetJsonString("Factor4", document_LinesImpl.Factor1);
				}
				if (document_LinesImpl.BaseType > 0)
				{
					text += Function.GetJsonString("BaseType", Function.ParseInt(document_LinesImpl.BaseType));
					if (document_LinesImpl.BaseEntry > 0)
					{
						text += Function.GetJsonString("BaseEntry", Function.ParseInt(document_LinesImpl.BaseEntry));
					}
					if (document_LinesImpl.BaseLine >= 0)
					{
						text += Function.GetJsonString("BaseLine", Function.ParseInt(document_LinesImpl.BaseLine));
					}
				}
				text += Function.GetJsonString("DistributionRule", Function.ToString(document_LinesImpl.OcrCode));
				text += Function.GetJsonString("DistributionRule2", Function.ToString(document_LinesImpl.OcrCode2));
				text += Function.GetJsonString("DistributionRule3", Function.ToString(document_LinesImpl.OcrCode3));
				text += Function.GetJsonString("DistributionRule4", Function.ToString(document_LinesImpl.OcrCode4));
				text += Function.GetJsonString("DistributionRule5", Function.ToString(document_LinesImpl.OcrCode5));
				if (!string.IsNullOrEmpty(document_LinesImpl.UseBaseUn))
				{
					text = ((!document_LinesImpl.UseBaseUn.Equals("Y")) ? (text + Function.GetJsonString("UseBaseUnits", "tNO")) : (text + Function.GetJsonString("UseBaseUnits", "tYES")));
				}
				if (!document_LinesImpl.UomEntry.Equals(0))
				{
					text += Function.GetJsonString("UoMEntry", Function.ParseInt(document_LinesImpl.UomEntry));
				}
				if (!document_LinesImpl.NumPerMsr.Equals(0.0))
				{
					text += Function.GetJsonString("UnitsOfMeasurment", Function.ToString(document_LinesImpl.NumPerMsr));
				}
				text += DataService.MappingUdf(document_LinesImpl.UserFields);
				if (model.Lines[i].SerialNumbers != null && model.Lines[i].SerialNumbers.Count > 0)
				{
					text += "\"SerialNumbers\":[ ";
					for (int j = 0; j < model.Lines[i].SerialNumbers.Count; j++)
					{
						text += "{ ";
						SerialNumbersImpl serialNumbersImpl = model.Lines[i].SerialNumbers[j];
						if (!string.IsNullOrEmpty(serialNumbersImpl.BatchID))
						{
							text += Function.GetJsonString("BatchID", Function.ToString(serialNumbersImpl.BatchID));
						}
						if (serialNumbersImpl.SystemSerialNumber > 0)
						{
							text += Function.GetJsonString("SystemSerialNumber", serialNumbersImpl.SystemSerialNumber);
						}
						if (!string.IsNullOrEmpty(serialNumbersImpl.ManufacturerSerialNumber))
						{
							text += Function.GetJsonString("ManufacturerSerialNumber", Function.ToString(serialNumbersImpl.ManufacturerSerialNumber));
						}
						if (!string.IsNullOrEmpty(serialNumbersImpl.InternalSerialNumber))
						{
							text += Function.GetJsonString("InternalSerialNumber", Function.ToString(serialNumbersImpl.InternalSerialNumber));
						}
						if (!string.IsNullOrEmpty(serialNumbersImpl.Location))
						{
							text += Function.GetJsonString("Location", Function.ToString(serialNumbersImpl.Location));
						}
						if (!string.IsNullOrEmpty(serialNumbersImpl.Notes))
						{
							text += Function.GetJsonString("Notes", Function.ToString(serialNumbersImpl.Notes));
						}
						if (type == EnumCommandType.Update || serialNumbersImpl.Quantity > 0.0)
						{
							text += Function.GetJsonString("Quantity", Function.ParseDecimal(serialNumbersImpl.Quantity));
						}
						if (serialNumbersImpl.BaseLineNumber >= 0)
						{
							text += Function.GetJsonString("BaseLineNumber", Function.ParseInt(serialNumbersImpl.BaseLineNumber));
						}
						if (0 < serialNumbersImpl.TrackingNote)
						{
							text += Function.GetJsonString("TrackingNote", Function.ToString(serialNumbersImpl.TrackingNote));
						}
						if (0 < serialNumbersImpl.TrackingNoteLine)
						{
							text += Function.GetJsonString("TrackingNoteLine", Function.ToString(serialNumbersImpl.TrackingNoteLine));
						}
						text += DataService.MappingUdf(serialNumbersImpl.UserFields);
						text += "},";
					}
					text += "],";
				}
				if (model.Lines[i].BatchNumbers != null && model.Lines[i].BatchNumbers.Count > 0)
				{
					text += "\"BatchNumbers\":[ ";
					for (int k = 0; k < model.Lines[i].BatchNumbers.Count; k++)
					{
						text += "{ ";
						BatchNumbersImpl batchNumbersImpl = model.Lines[i].BatchNumbers[k];
						if (!string.IsNullOrEmpty(batchNumbersImpl.BatchNumber))
						{
							text += Function.GetJsonString("BatchNumber", Function.ToString(batchNumbersImpl.BatchNumber));
						}
						if (!string.IsNullOrEmpty(batchNumbersImpl.ManufacturerSerialNumber))
						{
							text += Function.GetJsonString("ManufacturerSerialNumber", Function.ToString(batchNumbersImpl.ManufacturerSerialNumber));
						}
						if (!string.IsNullOrEmpty(batchNumbersImpl.InternalSerialNumber))
						{
							text += Function.GetJsonString("InternalSerialNumber", Function.ToString(batchNumbersImpl.InternalSerialNumber));
						}
						if (!string.IsNullOrEmpty(batchNumbersImpl.Location))
						{
							text += Function.GetJsonString("Location", Function.ToString(batchNumbersImpl.Location));
						}
						if (!string.IsNullOrEmpty(batchNumbersImpl.Notes))
						{
							text += Function.GetJsonString("Notes", Function.ToString(batchNumbersImpl.Notes));
						}
						if (type == EnumCommandType.Update || batchNumbersImpl.Quantity > 0.0)
						{
							text += Function.GetJsonString("Quantity", Function.ParseDecimal(batchNumbersImpl.Quantity));
						}
						if (batchNumbersImpl.BaseLineNumber >= 0)
						{
							text += Function.GetJsonString("BaseLineNumber", Function.ParseInt(batchNumbersImpl.BaseLineNumber));
						}
						if (0 < batchNumbersImpl.TrackingNote)
						{
							text += Function.GetJsonString("TrackingNote", Function.ToString(batchNumbersImpl.TrackingNote));
						}
						if (0 < batchNumbersImpl.TrackingNoteLine)
						{
							text += Function.GetJsonString("TrackingNoteLine", Function.ToString(batchNumbersImpl.TrackingNoteLine));
						}
						text += DataService.MappingUdf(batchNumbersImpl.UserFields);
						text += "},";
					}
					text += "],";
				}
				if (model.Lines[i].BinAllocations != null && model.Lines[i].BinAllocations.Count > 0)
				{
					text += "\"StockTransferLinesBinAllocations\":[ ";
					for (int l = 0; l < model.Lines[i].BinAllocations.Count; l++)
					{
						text += "{ ";
						DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = model.Lines[i].BinAllocations[l];
						if (!documentLinesBinAllocationsImpl.BinAbsEntry.Equals(0))
						{
							text += Function.GetJsonString("BinAbsEntry", Function.ParseInt(documentLinesBinAllocationsImpl.BinAbsEntry));
						}
						if (!documentLinesBinAllocationsImpl.Quantity.Equals(0.0))
						{
							text += Function.GetJsonString("Quantity", Function.ParseDecimal(documentLinesBinAllocationsImpl.Quantity));
						}
						if (documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine.HasValue && documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine >= 0)
						{
							text += Function.GetJsonString("SerialAndBatchNumbersBaseLine", Function.ParseInt(documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine));
						}
						if (documentLinesBinAllocationsImpl.BaseLineNumber >= 0)
						{
							text += Function.GetJsonString("BaseLineNumber", Function.ParseInt(documentLinesBinAllocationsImpl.BaseLineNumber));
						}
						switch (documentLinesBinAllocationsImpl.BinActionType)
						{
						case BinActionTypeEnum.batFromWarehouse:
							text += Function.GetJsonString("BinActionType", 2);
							break;
						case BinActionTypeEnum.batToWarehouse:
							text += Function.GetJsonString("BinActionType", 1);
							break;
						}
						text += "},";
					}
					text += "],";
				}
				text += "},";
			}
			text += "],";
		}
		if (model.DocumentRef != null && model.DocumentRef.Count > 0)
		{
			text += "\"DocumentReferences\":[ ";
			for (int m = 0; m < model.DocumentRef.Count; m++)
			{
				text += "{ ";
				Document_ReferencedImpl document_ReferencedImpl = model.DocumentRef[m];
				if (document_ReferencedImpl.RefDocEntr != 0)
				{
					text += Function.GetJsonString("RefDocEntr", Function.ParseInt(document_ReferencedImpl.RefDocEntr));
				}
				if (document_ReferencedImpl.RefDocNum != 0)
				{
					text += Function.GetJsonString("RefDocNum", Function.ParseInt(document_ReferencedImpl.RefDocNum));
				}
				if (document_ReferencedImpl.RefObjType != 0)
				{
					text += Function.GetJsonString("RefObjType", Function.ParseInt(document_ReferencedImpl.RefObjType));
				}
				if (!string.IsNullOrEmpty(Function.ToString(document_ReferencedImpl.RefType)))
				{
					text += Function.GetJsonString("RefType", Function.ToString(document_ReferencedImpl.RefType));
				}
				if (!string.IsNullOrEmpty(Function.ToString(document_ReferencedImpl.Remark)))
				{
					text += Function.GetJsonString("Remark", Function.ToString(document_ReferencedImpl.Remark));
				}
				text += "},";
			}
			text += "],";
		}
		return text + "}";
	}
}
