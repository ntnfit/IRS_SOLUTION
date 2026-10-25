using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;
using Apzon.Services;
using Apzon.Services.Interfaces.Commons;
using SAPbobsCOM;

namespace Apzon.DataServices.Implements;

public class StockTransferRepository : Repository<DocumentsImpl>, Apzon.DataServices.Interfaces.IStockTransfer, IRepository<DocumentsImpl>, IDisposable
{
	public readonly IUnitOfWork _unitOfWork;

	public StockTransferRepository(Apzon.Services.IUnitOfWork serUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	protected virtual HttpResult SaveAttachment(DocumentsImpl doc, string sessionId, bool addNew)
	{
		IFile files = ServiceUnitOfWork.Files;
		List<AttachmentFile_Impl> attachmentFiles = doc.AttachmentFiles;
		Dictionary<string, object> code = new Dictionary<string, object> { 
		{
			"DocEntry,int",
			(doc.DocEntry > 0) ? doc.DocEntry : 0
		} };
		string table;
		if (doc.ObjType == 67)
		{
			table = "OWTR";
		}
		else
		{
			table = ((doc.ObjType == 1250000001) ? "OWTQ" : "ODRF");
		}
		return files.SaveAttachmentFiles(attachmentFiles, code, table);
	}

	public HttpResult Save(DocumentsImpl doc, string sessionId, bool isDraft = false)
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
			if (doc.DocEntry <= 0)
			{
				if (doc.Document_ApprovalRequests != null && doc.Document_ApprovalRequests.Count > 0)
				{
					return _unitOfWork.Documents.AddApprovalRequest(doc, sessionId, ApzObjectTypes.oStockTransfer).Result;
				}
				if (doc.IsDraft && !isDraft)
				{
					doc.DocObjectCode = ApzObjectTypes.oStockTransfer;
					return SaveDraftToDocument(doc, sessionId);
				}
				return Add(doc, sessionId, isDraft);
			}
			return Update(doc, sessionId, isDraft);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual HttpResult Add(DocumentsImpl doc, string sessionId, bool isDraft)
	{
		string text = MappingXml(ApzObjectTypes.oStockTransfer, doc, isDraft);
		Logging.Write("WATCH", "Start Add Transfer...");
		XmlElement xmlElement = DataService.AddObject(ApzObjectTypes.oStockTransfer, sessionId, text).Content as XmlElement;
		XElement xElement = XElement.Parse(xmlElement.OuterXml);
		if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
		{
			XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
			if (xElement2 != null)
			{
				Logging.Write("ERROR", "Add Transfer Error", xElement2.Value);
				Logging.Write("WATCH", text);
				return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
			}
		}
		else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
		{
			XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
			if (xElement3 != null)
			{
				int num = Function.ParseInt(xElement3.Value);
				Logging.Write("WATCH", "Add Transfer success \t" + num);
				return new HttpResult(Enums.MessageCode.Success, "", num);
			}
		}
		return new HttpResult();
	}

	protected virtual HttpResult Update(DocumentsImpl doc, string sessionId, bool isDraft)
	{
		try
		{
			Logging.Write("WATCH", "Start update transfer");
			string text = MappingXml(ApzObjectTypes.oStockTransfer, doc, isDraft, EnumCommandType.Update);
			XmlElement xmlElement = DataService.UpdateObject(ApzObjectTypes.oStockTransfer, sessionId, text).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", "Update Transfer Error", xElement2.Value);
					Logging.Write("WATCH", text);
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", "Update Transfer \t" + num);
					return new HttpResult(Enums.MessageCode.Success, "", num);
				}
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual string MappingXml(ApzObjectTypes objectType, DocumentsImpl model, bool isDraft, EnumCommandType type = EnumCommandType.Add)
	{
		string text = "";
		string text2 = "";
		try
		{
			DataTable allGeneralSetting = ServiceUnitOfWork.GeneralSetting.GetAllGeneralSetting();
			if (allGeneralSetting.IsNotNull())
			{
				text = Function.ToString(allGeneralSetting.Rows[0]["MainCurncy"]);
				text2 = Function.ToString(allGeneralSetting.Rows[0]["SysCurrncy"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		string arg;
		if (objectType != ApzObjectTypes.oStockTransfer)
		{
			arg = (isDraft ? "oStockTransferDraft" : "oInventoryTransferRequest");
		}
		else
		{
			arg = (isDraft ? "oStockTransferDraft" : "oStockTransfer");
		}
		string text3 = $"<BOM><BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n              <AdmInfo>\r\n                <Object>{arg}</Object>\r\n              </AdmInfo>";
		if (type == EnumCommandType.Update)
		{
			text3 += " <QueryParams>";
			if (Function.ParseInt(model.DocEntry) != 0)
			{
				text3 = text3 + "<DocEntry>" + Function.ParseInt(model.DocEntry) + "</DocEntry>";
			}
			text3 += "</QueryParams>";
		}
		text3 += "<StockTransfer>\r\n            <row>";
		if (isDraft)
		{
			text3 = text3 + "<DocObjectCode>" + model.ObjType + "</DocObjectCode>";
		}
		if (model.DocDate > new DateTime(1900, 1, 1))
		{
			text3 = text3 + "<DocDate>" + model.DocDate.ToString("yyyyMMdd") + "</DocDate>";
		}
		if (model.DocDueDate > new DateTime(1900, 1, 1))
		{
			text3 = text3 + "<DueDate>" + model.DocDueDate.ToString("yyyyMMdd") + "</DueDate>";
		}
		if (!string.IsNullOrEmpty(model.CardCode))
		{
			text3 = text3 + "<CardCode>" + Function.RawText(model.CardCode) + "</CardCode>";
		}
		if (!string.IsNullOrEmpty(model.CardName))
		{
			text3 = text3 + "<CardName>" + Function.RawText(model.CardName) + "</CardName>";
		}
		if (model.Series != 0)
		{
			text3 = text3 + "<Series>" + model.Series + "</Series>";
		}
		if (!string.IsNullOrEmpty(model.Address))
		{
			text3 = text3 + "<Address>" + Function.RawText(model.Address) + "</Address>";
		}
		if (!string.IsNullOrEmpty(model.Ref1))
		{
			text3 = text3 + "<Reference1>" + Function.RawText(model.Ref1) + "</Reference1>";
		}
		if (!string.IsNullOrEmpty(model.Ref2))
		{
			text3 = text3 + "<Reference2>" + Function.RawText(model.Ref2) + "</Reference2>";
		}
		if (!string.IsNullOrEmpty(model.Comments))
		{
			text3 = text3 + "<Comments>" + Function.RawText(model.Comments) + "</Comments>";
		}
		if (!string.IsNullOrEmpty(model.JrnlMemo))
		{
			text3 = text3 + "<JournalMemo>" + Function.RawText(model.JrnlMemo) + "</JournalMemo>";
		}
		if (Function.ParseInt(model.GroupNum) != 0)
		{
			text3 = text3 + "<PriceList>" + model.GroupNum + "</PriceList>";
		}
		if (!model.SlpCode.Equals(0))
		{
			text3 = text3 + "<SalesPersonCode>" + model.SlpCode + "</SalesPersonCode>";
		}
		if (!string.IsNullOrEmpty(model.Filler))
		{
			text3 = text3 + "<FromWarehouse>" + Function.RawText(model.Filler) + "</FromWarehouse>";
		}
		if (!string.IsNullOrEmpty(model.ToWhsCode))
		{
			text3 = text3 + "<ToWarehouse>" + Function.RawText(model.ToWhsCode) + "</ToWarehouse>";
		}
		if (model.TaxDate > new DateTime(1900, 1, 1))
		{
			text3 = text3 + "<TaxDate>" + model.TaxDate.ToString("yyyyMMdd") + "</TaxDate>";
		}
		text3 += DataService.MappingUdf(model.UserFields);
		text3 += " </row>\r\n                            </StockTransfer>";
		if (model.Lines != null && model.Lines.Count > 0)
		{
			text3 += "<StockTransfer_Lines>";
			for (int i = 0; i < model.Lines.Count; i++)
			{
				text3 += "<row>";
				Document_LinesImpl document_LinesImpl = model.Lines[i];
				if (objectType == ApzObjectTypes.oInventoryTransferRequest || type == EnumCommandType.Add)
				{
					if (document_LinesImpl.LineNum >= 0)
					{
						text3 = text3 + "<LineNum>" + document_LinesImpl.LineNum + "</LineNum>";
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.ItemCode))
					{
						text3 = text3 + "<ItemCode>" + Function.RawText(document_LinesImpl.ItemCode) + "</ItemCode>";
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.Dscription))
					{
						text3 = text3 + "<ItemDescription>" + Function.RawText(document_LinesImpl.Dscription) + "</ItemDescription>";
					}
					if (!document_LinesImpl.Quantity.Equals(0.0))
					{
						text3 = text3 + "<Quantity>" + Function.ParseDecimal(document_LinesImpl.Quantity) + "</Quantity>";
					}
					if (!document_LinesImpl.InvQty.Equals(0.0))
					{
						text3 = text3 + "<InventoryQuantity>" + Function.ParseDecimal(document_LinesImpl.InvQty) + "</InventoryQuantity>";
					}
					if (!document_LinesImpl.Price.Equals(0.0))
					{
						text3 = text3 + "<Price>" + Function.ParseDecimal(document_LinesImpl.Price) + "</Price>";
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.Currency))
					{
						text3 = text3 + "<Currency>" + document_LinesImpl.Currency + "</Currency>";
					}
					if ((!string.IsNullOrEmpty(document_LinesImpl.Currency) && document_LinesImpl.Currency == text) || string.IsNullOrEmpty(document_LinesImpl.Currency))
					{
						text3 += "<Rate>0</Rate>";
					}
					else if (!document_LinesImpl.Rate.Equals(0.0) && document_LinesImpl.Currency != text2)
					{
						text3 = text3 + "<Rate>" + Function.ParseDecimal(document_LinesImpl.Rate) + "</Rate>";
					}
					if (!document_LinesImpl.DiscPrcnt.Equals(0.0))
					{
						text3 = text3 + "<DiscountPercent>" + Function.ParseDecimal(document_LinesImpl.DiscPrcnt) + "</DiscountPercent>";
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.WhsCode))
					{
						text3 = text3 + "<WarehouseCode>" + Function.RawText(document_LinesImpl.WhsCode) + "</WarehouseCode>";
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.FromWhsCod))
					{
						text3 = text3 + "<FromWarehouseCode>" + Function.RawText(document_LinesImpl.FromWhsCod) + "</FromWarehouseCode>";
					}
					if (!string.IsNullOrEmpty(document_LinesImpl.Project))
					{
						text3 = text3 + "<ProjectCode>" + Function.RawText(document_LinesImpl.Project) + "</ProjectCode>";
					}
					if (!document_LinesImpl.Factor1.Equals(0.0))
					{
						text3 = text3 + "<Factor>" + Function.ParseDecimal(document_LinesImpl.Factor1) + "</Factor>";
					}
					if (!document_LinesImpl.Factor2.Equals(0.0))
					{
						text3 = text3 + "<Factor2>" + Function.ParseDecimal(document_LinesImpl.Factor2) + "</Factor2>";
					}
					if (!document_LinesImpl.Factor3.Equals(0.0))
					{
						text3 = text3 + "<Factor3>" + Function.ParseDecimal(document_LinesImpl.Factor3) + "</Factor3>";
					}
					if (!document_LinesImpl.Factor4.Equals(0.0))
					{
						text3 = text3 + "<Factor4>" + Function.ParseDecimal(document_LinesImpl.Factor4) + "</Factor4>";
					}
					if (document_LinesImpl.BaseType > 0)
					{
						text3 = text3 + "<BaseType>" + Function.ParseInt(document_LinesImpl.BaseType) + "</BaseType>";
						if (document_LinesImpl.BaseEntry > 0)
						{
							text3 = text3 + "<BaseEntry>" + Function.ParseInt(document_LinesImpl.BaseEntry) + "</BaseEntry>";
						}
						if (document_LinesImpl.BaseLine >= 0)
						{
							text3 = text3 + "<BaseLine>" + Function.ParseInt(document_LinesImpl.BaseLine) + "</BaseLine>";
						}
					}
					text3 = text3 + "<DistributionRule>" + document_LinesImpl.OcrCode + "</DistributionRule>";
					text3 = text3 + "<DistributionRule2>" + document_LinesImpl.OcrCode2 + "</DistributionRule2>";
					text3 = text3 + "<DistributionRule3>" + document_LinesImpl.OcrCode3 + "</DistributionRule3>";
					text3 = text3 + "<DistributionRule4>" + document_LinesImpl.OcrCode4 + "</DistributionRule4>";
					text3 = text3 + "<DistributionRule5>" + document_LinesImpl.OcrCode5 + "</DistributionRule5>";
					if (!string.IsNullOrEmpty(document_LinesImpl.UseBaseUn))
					{
						text3 = ((!document_LinesImpl.UseBaseUn.Equals("Y")) ? (text3 + "<UseBaseUnits>tNO</UseBaseUnits>") : (text3 + "<UseBaseUnits>tYES</UseBaseUnits>"));
					}
					if (!document_LinesImpl.NumPerMsr.Equals(0.0))
					{
						text3 = text3 + "<UnitsOfMeasurment>" + Function.RawText(document_LinesImpl.NumPerMsr) + "</UnitsOfMeasurment>";
					}
					if (!document_LinesImpl.UomEntry.Equals(0))
					{
						text3 = text3 + "<UoMEntry>" + Function.ParseInt(document_LinesImpl.UomEntry) + "</UoMEntry>";
					}
					text3 += DataService.MappingUdf(document_LinesImpl.UserFields);
				}
				text3 += "</row>";
			}
			text3 += "</StockTransfer_Lines>";
			text3 += "<SerialNumbers>";
			for (int j = 0; j < model.Lines.Count; j++)
			{
				if (model.Lines[j].SerialNumbers == null || model.Lines[j].SerialNumbers.Count <= 0)
				{
					continue;
				}
				for (int k = 0; k < model.Lines[j].SerialNumbers.Count; k++)
				{
					text3 += "<row>";
					SerialNumbersImpl serialNumbersImpl = model.Lines[j].SerialNumbers[k];
					if (!string.IsNullOrEmpty(serialNumbersImpl.BatchID))
					{
						text3 = text3 + "<BatchID>" + Function.RawText(serialNumbersImpl.BatchID) + "</BatchID>";
					}
					if (serialNumbersImpl.SystemSerialNumber >= 0)
					{
						text3 = text3 + "<SystemSerialNumber>" + Function.ParseInt(serialNumbersImpl.SystemSerialNumber) + "</SystemSerialNumber>";
					}
					if (!string.IsNullOrEmpty(serialNumbersImpl.ManufacturerSerialNumber))
					{
						text3 = text3 + "<ManufacturerSerialNumber>" + Function.RawText(serialNumbersImpl.ManufacturerSerialNumber) + "</ManufacturerSerialNumber>";
					}
					if (!string.IsNullOrEmpty(serialNumbersImpl.InternalSerialNumber))
					{
						text3 = text3 + "<InternalSerialNumber>" + Function.RawText(serialNumbersImpl.InternalSerialNumber) + "</InternalSerialNumber>";
					}
					if (serialNumbersImpl.ExpiryDate.HasValue && serialNumbersImpl.ExpiryDate > new DateTime(1900, 1, 1))
					{
						text3 = text3 + "<ExpiryDate>" + Convert.ToDateTime(serialNumbersImpl.ExpiryDate).ToString("yyyyMMdd") + "</ExpiryDate>";
					}
					if (serialNumbersImpl.ManufactureDate.HasValue && serialNumbersImpl.ManufactureDate > new DateTime(1900, 1, 1))
					{
						text3 = text3 + "<ManufacturingDate>" + Convert.ToDateTime(serialNumbersImpl.ManufactureDate).ToString("yyyyMMdd") + "</ManufacturingDate>";
					}
					if (serialNumbersImpl.ReceptionDate.HasValue && serialNumbersImpl.ReceptionDate > new DateTime(1900, 1, 1))
					{
						text3 = text3 + "<ReceptionDate>" + Convert.ToDateTime(serialNumbersImpl.ReceptionDate).ToString("yyyyMMdd") + "</ReceptionDate>";
					}
					if (!string.IsNullOrEmpty(serialNumbersImpl.Location))
					{
						text3 = text3 + "<Location>" + Function.RawText(serialNumbersImpl.Location) + "</Location>";
					}
					if (!string.IsNullOrEmpty(serialNumbersImpl.Notes))
					{
						text3 = text3 + "<Notes>" + Function.RawText(serialNumbersImpl.Notes) + "</Notes>";
					}
					if (type == EnumCommandType.Update || serialNumbersImpl.Quantity > 0.0)
					{
						text3 = text3 + "<Quantity>" + Function.ParseDecimal(serialNumbersImpl.Quantity) + "</Quantity>";
					}
					if (serialNumbersImpl.BaseLineNumber >= 0)
					{
						text3 = text3 + "<BaseLineNumber>" + Function.ParseInt(serialNumbersImpl.BaseLineNumber) + "</BaseLineNumber>";
					}
					if (0 < serialNumbersImpl.TrackingNote)
					{
						text3 = text3 + "<TrackingNote>" + Function.RawText(serialNumbersImpl.TrackingNote) + "</TrackingNote>";
					}
					if (0 < serialNumbersImpl.TrackingNoteLine)
					{
						text3 = text3 + "<TrackingNoteLine>" + Function.RawText(serialNumbersImpl.TrackingNoteLine) + "</TrackingNoteLine>";
					}
					text3 += DataService.MappingUdf(serialNumbersImpl.UserFields);
					text3 += "</row>";
				}
			}
			text3 += "</SerialNumbers>";
			text3 += "<BatchNumbers>";
			for (int l = 0; l < model.Lines.Count; l++)
			{
				if (model.Lines[l].BatchNumbers == null || model.Lines[l].BatchNumbers.Count <= 0)
				{
					continue;
				}
				for (int m = 0; m < model.Lines[l].BatchNumbers.Count; m++)
				{
					text3 += "<row>";
					BatchNumbersImpl batchNumbersImpl = model.Lines[l].BatchNumbers[m];
					if (!string.IsNullOrEmpty(batchNumbersImpl.BatchNumber))
					{
						text3 = text3 + "<BatchNumber>" + Function.ToString(batchNumbersImpl.BatchNumber) + "</BatchNumber>";
					}
					if (!string.IsNullOrEmpty(batchNumbersImpl.ManufacturerSerialNumber))
					{
						text3 = text3 + "<ManufacturerSerialNumber>" + Function.RawText(batchNumbersImpl.ManufacturerSerialNumber) + "</ManufacturerSerialNumber>";
					}
					if (!string.IsNullOrEmpty(batchNumbersImpl.InternalSerialNumber))
					{
						text3 = text3 + "<InternalSerialNumber>" + Function.RawText(batchNumbersImpl.InternalSerialNumber) + "</InternalSerialNumber>";
					}
					if (batchNumbersImpl.ExpiryDate.HasValue && batchNumbersImpl.ExpiryDate > new DateTime(1900, 1, 1))
					{
						text3 = text3 + "<ExpiryDate>" + Convert.ToDateTime(batchNumbersImpl.ExpiryDate).ToString("yyyyMMdd") + "</ExpiryDate>";
					}
					if (batchNumbersImpl.ManufacturingDate.HasValue && batchNumbersImpl.ManufacturingDate > new DateTime(1900, 1, 1))
					{
						text3 = text3 + "<ManufacturingDate>" + Convert.ToDateTime(batchNumbersImpl.ManufacturingDate).ToString("yyyyMMdd") + "</ManufacturingDate>";
					}
					if (batchNumbersImpl.AddmisionDate.HasValue && batchNumbersImpl.AddmisionDate > new DateTime(1900, 1, 1))
					{
						text3 = text3 + "<AddmisionDate>" + Convert.ToDateTime(batchNumbersImpl.AddmisionDate).ToString("yyyyMMdd") + "</AddmisionDate>";
					}
					if (!string.IsNullOrEmpty(batchNumbersImpl.Location))
					{
						text3 = text3 + "<Location>" + Function.RawText(batchNumbersImpl.Location) + "</Location>";
					}
					if (!string.IsNullOrEmpty(batchNumbersImpl.Notes))
					{
						text3 = text3 + "<Notes>" + Function.RawText(batchNumbersImpl.Notes) + "</Notes>";
					}
					if (type == EnumCommandType.Update || batchNumbersImpl.Quantity > 0.0)
					{
						text3 = text3 + "<Quantity>" + Function.ParseDecimal(batchNumbersImpl.Quantity) + "</Quantity>";
					}
					if (batchNumbersImpl.BaseLineNumber >= 0)
					{
						text3 = text3 + "<BaseLineNumber>" + Function.ParseInt(batchNumbersImpl.BaseLineNumber) + "</BaseLineNumber>";
					}
					if (0 < batchNumbersImpl.TrackingNote)
					{
						text3 = text3 + "<TrackingNote>" + Function.RawText(batchNumbersImpl.TrackingNote) + "</TrackingNote>";
					}
					if (0 < batchNumbersImpl.TrackingNoteLine)
					{
						text3 = text3 + "<TrackingNoteLine>" + Function.RawText(batchNumbersImpl.TrackingNoteLine) + "</TrackingNoteLine>";
					}
					text3 += DataService.MappingUdf(batchNumbersImpl.UserFields);
					text3 += "</row>";
				}
			}
			text3 += "</BatchNumbers>";
			if (model.Lines != null && model.Lines.Count > 0)
			{
				text3 += "<StockTransferLinesBinAllocations>";
				for (int n = 0; n < model.Lines.Count; n++)
				{
					if (model.Lines[n].BinAllocations == null || model.Lines[n].BinAllocations.Count <= 0)
					{
						continue;
					}
					for (int num = 0; num < model.Lines[n].BinAllocations.Count; num++)
					{
						text3 += "<row>";
						DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = model.Lines[n].BinAllocations[num];
						if (!documentLinesBinAllocationsImpl.BinAbsEntry.Equals(0))
						{
							text3 = text3 + "<BinAbsEntry>" + Function.ParseInt(documentLinesBinAllocationsImpl.BinAbsEntry) + "</BinAbsEntry>";
						}
						if (!documentLinesBinAllocationsImpl.Quantity.Equals(0.0))
						{
							text3 = text3 + "<Quantity>" + Function.ParseDouble(documentLinesBinAllocationsImpl.Quantity) + "</Quantity>";
						}
						if (documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine.HasValue && documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine >= 0)
						{
							text3 = text3 + "<SerialAndBatchNumbersBaseLine>" + Function.ParseInt(documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine) + "</SerialAndBatchNumbersBaseLine>";
						}
						if (documentLinesBinAllocationsImpl.BaseLineNumber >= 0)
						{
							text3 = text3 + "<BaseLineNumber>" + Function.ParseInt(documentLinesBinAllocationsImpl.BaseLineNumber) + "</BaseLineNumber>";
						}
						switch (documentLinesBinAllocationsImpl.BinActionType)
						{
						case BinActionTypeEnum.batFromWarehouse:
							text3 += "<BinActionType>2</BinActionType>";
							break;
						case BinActionTypeEnum.batToWarehouse:
							text3 += "<BinActionType>1</BinActionType>";
							break;
						}
						text3 += "</row>";
					}
				}
				text3 += "</StockTransferLinesBinAllocations>";
			}
		}
		return text3 + "</BO></BOM>";
	}

	public virtual HttpResult SaveDraftToDocument(int docEntry, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>StockTransferDraftService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <SaveDraftToDocument xmlns=\"StockTransferDraftService\"><StockTransfer><DocEntry>" + docEntry + "</DocEntry></StockTransfer></SaveDraftToDocument>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					throw new Exception(xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					return new HttpResult(Enums.MessageCode.Success, "", Function.ParseInt(xElement3.Value));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Success);
	}

	public virtual HttpResult SaveDraftToDocument(DocumentsImpl doc, string sessionId)
	{
		try
		{
			string text = MappingXml(doc.DocObjectCode, doc, isDraft: false);
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text + "</dis:AddObject></env:Body></env:Envelopes>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"AddObject document - {doc.DocObjectCode} - DocEntry - {num}");
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
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
		return new HttpResult();
	}

	public void Dispose()
	{
	}

	public async Task<HttpResult> SaveTransferBin(Tuple<DocumentsImpl, DataSet> model, string sessionId)
	{
		try
		{
			Logging.Write("WATCH", "Start Transfer bin");
			DocumentsImpl item = model.Item1;
			string text = "";
			string text2 = "";
			if (item.Lines.Count > 0)
			{
				text = "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + MappingXml(ApzObjectTypes.oStockTransfer, item, isDraft: false) + "</dis:AddObject>";
			}
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + text2 + "</env:Body></env:Envelopes>";
			XmlElement xmlElement = (await DataService.ProcessAsync(xmlObject)).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", "Transfer Bin Error", xElement2.Value);
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", "Save Transfer Bin " + num);
					return new HttpResult(Enums.MessageCode.Success, "", num);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("DocEntry"))
			{
				XElement xElement4 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("DocEntry"));
				if (xElement4 != null)
				{
					int num2 = Function.ParseInt(xElement4.Value);
					Logging.Write("WATCH", "Save Transfer Bin " + num2);
					return new HttpResult(Enums.MessageCode.Success, "", num2);
				}
			}
			return new HttpResult(Enums.MessageCode.Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult Posting(Tuple<DocumentsImpl, DocumentsImpl, DocumentsImpl> model, string sessionId)
	{
		try
		{
			string text = string.Empty;
			if (model.Item2.Lines.Count > 0)
			{
				text = text + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(model.Item2, EnumCommandType.Add, ApzObjectTypes.oInventoryGenEntry) + "</dis:AddObject>";
			}
			if (model.Item3.Lines.Count > 0)
			{
				text = text + " <dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(model.Item3, EnumCommandType.Add, ApzObjectTypes.oInventoryGenExit) + "</dis:AddObject>";
			}
			if (model.Item1.Lines.Count > 0)
			{
				text = text + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + MappingXml(ApzObjectTypes.oStockTransfer, model.Item1, isDraft: false) + "</dis:AddObject>";
			}
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "</env:Body></env:Envelopes>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	public virtual string Close(int docEntry, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                                <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                 <env:Header>\r\n                                  <SessionID>" + sessionId + "</SessionID>\r\n                                 </env:Header>\r\n                                 <env:Body>\r\n                                  <dis:CloseObject  xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                  <BOM>\r\n                                   <BO>\r\n                                    <AdmInfo>\r\n                                     <Object>" + Function.ToString(ApzObjectTypes.oStockTransfer) + "</Object>\r\n                                    </AdmInfo>\r\n                                    <QueryParams>\r\n                                     <DocEntry>" + Function.ToString(docEntry) + "</DocEntry>\r\n                                    </QueryParams>\r\n                                   </BO>\r\n                                  </BOM>\r\n                                  </dis:CloseObject >\r\n                                 </env:Body>\r\n                                </env:Envelope>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", $"Close document - {ApzObjectTypes.oStockTransfer} - {xElement2.Value}");
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Close document - {ApzObjectTypes.oStockTransfer} - DocEntry - {num}");
					return "";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "";
	}

	public virtual string Cancel(int docEntry, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                                <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                 <env:Header>\r\n                                  <SessionID>" + sessionId + "</SessionID>\r\n                                 </env:Header>\r\n                                 <env:Body>\r\n                                  <dis:CancelObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                  <BOM>\r\n                                   <BO>\r\n                                    <AdmInfo>\r\n                                     <Object>" + Function.ToString(ApzObjectTypes.oStockTransfer) + "</Object>\r\n                                    </AdmInfo>\r\n                                    <QueryParams>\r\n                                     <DocEntry>" + Function.ToString(docEntry) + "</DocEntry>\r\n                                    </QueryParams>\r\n                                   </BO>\r\n                                  </BOM>\r\n                                  </dis:CancelObject>\r\n                                 </env:Body>\r\n                                </env:Envelope>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", $"cancel document - {ApzObjectTypes.oStockTransfer} - {xElement2.Value}");
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"cancel document - {ApzObjectTypes.oStockTransfer} - DocEntry - {num}");
					return "";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "Unknown Error";
	}

	public bool Exists(int docEntry, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n\r\n                            <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n\r\n                             <env:Header>\r\n\r\n                              <SessionID>" + sessionId + "</SessionID> \r\n\r\n                             </env:Header>\r\n\r\n                             <env:Body>\r\n\r\n                              <dis:GetByKey xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n\r\n                               <Object>" + ApzObjectTypes.oStockTransfer.ToString() + "</Object> \r\n\r\n                               <DocEntry>" + docEntry + "</DocEntry> \r\n\r\n                              </dis:GetByKey>\r\n\r\n                             </env:Body>\r\n\r\n                            </env:Envelope>";
			XmlElement xmlElement = DataService.GetByKey(sessionId, xmlObject).Content as XmlElement;
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				return false;
			}
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("DocEntry"))
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return !docEntry.Equals(0);
		}
		return false;
	}

	public HttpResult AddTransferMobileApp(DataTable data, string sessionId)
	{
		try
		{
			if (!data.IsNotNull())
			{
				return new HttpResult(Enums.MessageCode.Error, "Data is Empty!");
			}
			DataTable dataTable = data.Clone();
			List<string> list = new List<string>();
			foreach (DataRow row in data.Rows)
			{
				int num = Function.ParseInt(row["DocEntry"]);
				if (num == 0)
				{
					DataRow dataRow2 = dataTable.NewRow();
					dataRow2.ItemArray = row.ItemArray;
					dataTable.Rows.Add(dataRow2);
					string item = Function.ToString(row["TempKey"]);
					list.Add(item);
					continue;
				}
				string text = Function.ToString(row["BoxCode"]);
				string value = Function.ToString(row["Whs"]);
				string value2 = Function.ToString(row["ToWhs"]);
				List<SqlParameter> list2 = new List<SqlParameter>
				{
					new SqlParameter("@barcode", text),
					new SqlParameter("@whs", value2),
					new SqlParameter("@fromWhs", value)
				};
				DataSet dataSet = ExecuteData("Apz_InventoryTransfer_GetDataByBarcode", CommandType.StoredProcedure, list2.ToArray());
				if (dataSet.IsHasStructure() && dataSet.Tables[1].IsNotNull())
				{
					foreach (DataRow row2 in dataSet.Tables[1].Rows)
					{
						DataRow dataRow4 = dataTable.NewRow();
						dataRow4.ItemArray = row2.ItemArray;
						dataTable.Rows.Add(dataRow4);
					}
					list.Add(num.ToString());
					continue;
				}
				return new HttpResult(Enums.MessageCode.Error, "Không lấy được thông tin Box: " + text);
			}
			if (!dataTable.IsNotNull())
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Không lấy được thông tin các Box");
				return new HttpResult(Enums.MessageCode.Error, "Không lấy được thông tin các Box!");
			}
			DocumentsImpl documentsImpl = new DocumentsImpl();
			documentsImpl.DocDate = DateTime.Today;
			documentsImpl.TaxDate = DateTime.Today;
			documentsImpl.DocDueDate = DateTime.Today;
			documentsImpl.Comments = string.Join(",", list.ToArray());
			if (documentsImpl.Comments.Length > 250)
			{
				documentsImpl.Comments = documentsImpl.Comments.Substring(0, 250);
			}
			documentsImpl.Lines = new List<Document_LinesImpl>();
			documentsImpl.UserFields = new List<UserFieldsImpl>();
			var enumerable = from t in dataTable.AsEnumerable()
				group t by new
				{
					ItemCode = Function.ToString(t["ItemCode"]),
					Whs = Function.ToString(t["Whs"]),
					ToWhs = Function.ToString(t["ToWhs"])
				} into g
				select new
				{
					Key = g.Key,
					Items = g
				};
			int num2 = 0;
			foreach (var item2 in enumerable)
			{
				Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
				document_LinesImpl.ItemCode = Function.ToString(item2.Key.ItemCode);
				document_LinesImpl.FromWhsCod = Function.ToString(item2.Key.Whs).ToUpper();
				if (Function.ParseInt(item2.Items.First()["BaseEntry"]) > 0)
				{
					document_LinesImpl.BaseType = 1250000001;
					document_LinesImpl.BaseEntry = Function.ParseInt(item2.Items.First()["BaseEntry"]);
					document_LinesImpl.BaseLine = Function.ParseInt(item2.Items.First()["BaseLine"]);
				}
				document_LinesImpl.Quantity = 0.0;
				if (string.IsNullOrEmpty(documentsImpl.Filler))
				{
					documentsImpl.Filler = document_LinesImpl.FromWhsCod;
				}
				document_LinesImpl.WhsCode = Function.ToString(item2.Key.ToWhs).ToUpper();
				if (string.IsNullOrEmpty(documentsImpl.ToWhsCode))
				{
					documentsImpl.ToWhsCode = document_LinesImpl.WhsCode;
				}
				int num3 = 0;
				document_LinesImpl.BatchNumbers = new List<BatchNumbersImpl>();
				document_LinesImpl.SerialNumbers = new List<SerialNumbersImpl>();
				document_LinesImpl.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
				document_LinesImpl.UserFields = new List<UserFieldsImpl>();
				foreach (var item3 in from g in item2.Items
					group g by new
					{
						DistNumber = Function.ToString(g["DistNumber"]),
						ObjType = Function.ToString(g["ObjType"]),
						SystemNumber = Function.ParseInt(g["SysNumber"]),
						BinId = Function.ParseInt(g["BinId"]),
						ToBinId = Function.ParseInt(g["ToBinId"])
					} into g
					select new
					{
						Key = g.Key,
						Quantity = g.Sum((DataRow dat) => Function.ParseDouble(dat["Quantity"])),
						Bins = g
					})
				{
					int binId = item3.Key.BinId;
					int toBinId = item3.Key.ToBinId;
					if (binId == 0 || toBinId == 0 || !binId.Equals(toBinId))
					{
						double quantity = item3.Quantity;
						document_LinesImpl.Quantity += quantity;
						string text2 = "10000044";
						text2 = item3.Key.ObjType;
						if (text2.Equals("10000044"))
						{
							BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl();
							batchNumbersImpl.BatchNumber = item3.Key.DistNumber;
							batchNumbersImpl.Quantity = quantity;
							batchNumbersImpl.BaseLineNumber = num2;
							document_LinesImpl.BatchNumbers.Add(batchNumbersImpl);
						}
						else if (text2.Equals("10000045"))
						{
							SerialNumbersImpl serialNumbersImpl = new SerialNumbersImpl();
							serialNumbersImpl.BatchID = item3.Key.DistNumber;
							serialNumbersImpl.SystemSerialNumber = item3.Key.SystemNumber;
							serialNumbersImpl.BaseLineNumber = document_LinesImpl.LineNum;
							serialNumbersImpl.Quantity = quantity;
							document_LinesImpl.SerialNumbers.Add(serialNumbersImpl);
						}
						if (binId > 0)
						{
							DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = new DocumentLinesBinAllocationsImpl();
							documentLinesBinAllocationsImpl.BinAbsEntry = binId;
							documentLinesBinAllocationsImpl.BaseLineNumber = num2;
							documentLinesBinAllocationsImpl.BinActionType = BinActionTypeEnum.batFromWarehouse;
							documentLinesBinAllocationsImpl.Quantity = quantity;
							documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine = num3;
							document_LinesImpl.BinAllocations.Add(documentLinesBinAllocationsImpl);
						}
						if (toBinId > 0)
						{
							DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl2 = new DocumentLinesBinAllocationsImpl();
							documentLinesBinAllocationsImpl2.BinAbsEntry = toBinId;
							documentLinesBinAllocationsImpl2.BaseLineNumber = num2;
							documentLinesBinAllocationsImpl2.BinActionType = BinActionTypeEnum.batToWarehouse;
							documentLinesBinAllocationsImpl2.Quantity = quantity;
							documentLinesBinAllocationsImpl2.SerialAndBatchNumbersBaseLine = num3;
							document_LinesImpl.BinAllocations.Add(documentLinesBinAllocationsImpl2);
						}
						num3++;
					}
				}
				document_LinesImpl.LineNum = num2;
				documentsImpl.Lines.Add(document_LinesImpl);
				num2++;
			}
			if (documentsImpl.Lines.Count == 0)
			{
				Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Không có dữ liệu chuyển BIN");
				return new HttpResult();
			}
			HttpResult httpResult = Add(documentsImpl, sessionId, isDraft: false);
			if (string.IsNullOrEmpty(httpResult.Message) && Function.ParseInt(httpResult.Content) > 0)
			{
				try
				{
					int num4 = Function.ParseInt(httpResult.Content);
					string query = $"Apz_InventoryTransfer_LogTransferBox";
					List<SqlParameter> list3 = new List<SqlParameter>
					{
						new SqlParameter("@lstBox", string.Join(",", list.ToArray())),
						new SqlParameter("@docTransfer", num4)
					};
					DataTable dataTable2 = ExecuteDataTable(query, CommandType.StoredProcedure, list3.ToArray());
					if (dataTable2.IsNotNull() && Function.ParseInt(dataTable2.Rows[0][0]) != 0)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), Function.ToString(dataTable2.Rows[0][1]));
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
				}
			}
			return httpResult;
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
	}
}
