using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Inventory;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class PickListsRepository : Repository<PickListsImpl>, IPickLists, IRepository<PickListsImpl>, IDisposable
{
	protected readonly IUnitOfWork _dataserviceUnitOfWork;

	public PickListsRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService, IUnitOfWork serviceUnitOfWork)
		: base(unitOfWork, dataService)
	{
		_dataserviceUnitOfWork = serviceUnitOfWork;
	}

	public async Task<HttpResult> Save(PickListsImpl doc, string sessionId)
	{
		_ = 1;
		try
		{
			if (doc.AbsEntry <= 0)
			{
				return await AddNew(doc, sessionId);
			}
			return await UpdateData(doc, sessionId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual async Task<HttpResult> AddNew(PickListsImpl doc, string sessionId)
	{
		try
		{
			string empty = string.Empty;
			string text = Mapping(doc, EnumCommandType.Add);
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text + "</dis:AddObject>" + empty + "</env:Body></env:Envelopes>";
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
					Logging.Write("WATCH", $"Add new PickLists\t{num}");
					return new HttpResult(Enums.MessageCode.Success, "", num);
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

	protected virtual async Task<HttpResult> UpdateData(PickListsImpl doc, string sessionId)
	{
		try
		{
			string empty = string.Empty;
			string text = Mapping(doc, EnumCommandType.Update);
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text + "</dis:UpdateObject>" + empty + "</env:Body></env:Envelopes>";
			XmlElement xmlElement = (await DataService.ProcessAsync(xmlObject)).Content as XmlElement;
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
					Logging.Write("WATCH", $"Update PickLists\t{num}");
					return new HttpResult(Enums.MessageCode.Success, "", num);
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

	private string Mapping(PickListsImpl doc, EnumCommandType commandType)
	{
		string text = "<BOM>\r\n                          <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                          <AdmInfo>\r\n                            <Object>" + Function.ToString(ApzObjectTypes.oPickLists) + "</Object>\r\n                          </AdmInfo>";
		try
		{
			if (commandType == EnumCommandType.Update)
			{
				text = text + "<QueryParams><Absoluteentry>" + Function.ParseInt(doc.AbsEntry) + "</Absoluteentry></QueryParams>";
			}
			text += " <PickLists>\r\n                            <row>";
			if (!string.IsNullOrEmpty(doc.Name))
			{
				text = text + "<Name>" + Function.RawText(doc.Name) + "</Name>";
			}
			if (!string.IsNullOrEmpty(doc.OwnerCode))
			{
				text = text + "<OwnerCode>" + Function.RawText(doc.OwnerCode) + "</OwnerCode>";
			}
			if (doc.PickDate > new DateTime(1900, 1, 1))
			{
				text = text + "<PickDate>" + Convert.ToDateTime(doc.PickDate).ToString("yyyyMMdd") + "</PickDate>";
			}
			if (!string.IsNullOrEmpty(doc.Remarks))
			{
				text = text + "<Remarks>" + Function.RawText(doc.Remarks) + "</Remarks>";
			}
			if (!string.IsNullOrEmpty(doc.UseBaseUn))
			{
				text = ((!(doc.UseBaseUn == "Y")) ? (text + "<UseBaseUn>tNO</UseBaseUn>") : (text + "<UseBaseUn>tYES</UseBaseUn>"));
			}
			text += DataService.MappingUdf(doc.UserFields, commandType);
			text += "</row>\r\n                        </PickLists>";
			if (doc.Lines != null && doc.Lines.Count > 0)
			{
				text += "<PickLists_Lines>";
				for (int i = 0; i < doc.Lines.Count; i++)
				{
					PickLists_LineImpl pickLists_LineImpl = doc.Lines[i];
					text += "<row>";
					text += "<AbsoluteEntry>5</AbsoluteEntry>";
					text = text + "<OrderEntry>" + Function.ParseInt(pickLists_LineImpl.OrderEntry) + "</OrderEntry>";
					text = text + "<OrderRowID>" + Function.ParseInt(pickLists_LineImpl.OrderLine) + "</OrderRowID>";
					text = text + "<PickedQuantity>" + Function.ParseDecimal(pickLists_LineImpl.PickQtty) + "</PickedQuantity>";
					text = text + "<ReleasedQuantity>" + Function.ParseDecimal(pickLists_LineImpl.RelQtty) + "</ReleasedQuantity>";
					if (pickLists_LineImpl.BaseObject != 0)
					{
						text = text + "<BaseObjectType>" + Function.ParseInt(pickLists_LineImpl.BaseObject) + "</BaseObjectType>";
					}
					text += DataService.MappingUdf(pickLists_LineImpl.UserFields, commandType);
					text += "</row>";
				}
				text += "</PickLists_Lines>";
				if (doc.Lines != null && doc.Lines.Count > 0)
				{
					text += "<BatchNumbers>";
					for (int j = 0; j < doc.Lines.Count; j++)
					{
						if (doc.Lines[j].BatchNumbers == null || doc.Lines[j].BatchNumbers.Count <= 0)
						{
							continue;
						}
						for (int k = 0; k < doc.Lines[j].BatchNumbers.Count; k++)
						{
							text += "<row>";
							BatchNumbersImpl batchNumbersImpl = doc.Lines[j].BatchNumbers[k];
							if (!string.IsNullOrEmpty(batchNumbersImpl.BatchNumber))
							{
								text = text + "<BatchNumber>" + Function.ToString(batchNumbersImpl.BatchNumber) + "</BatchNumber>";
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.ManufacturerSerialNumber))
							{
								text = text + "<ManufacturerSerialNumber>" + Function.RawText(batchNumbersImpl.ManufacturerSerialNumber) + "</ManufacturerSerialNumber>";
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.InternalSerialNumber))
							{
								text = text + "<InternalSerialNumber>" + Function.RawText(batchNumbersImpl.InternalSerialNumber) + "</InternalSerialNumber>";
							}
							if (batchNumbersImpl.ExpiryDate.HasValue && batchNumbersImpl.ExpiryDate > new DateTime(1900, 1, 1))
							{
								text = text + "<ExpiryDate>" + Convert.ToDateTime(batchNumbersImpl.ExpiryDate).ToString("yyyyMMdd") + "</ExpiryDate>";
							}
							if (batchNumbersImpl.ManufacturingDate.HasValue && batchNumbersImpl.ManufacturingDate > new DateTime(1900, 1, 1))
							{
								text = text + "<ManufacturingDate>" + Convert.ToDateTime(batchNumbersImpl.ManufacturingDate).ToString("yyyyMMdd") + "</ManufacturingDate>";
							}
							if (batchNumbersImpl.AddmisionDate.HasValue && batchNumbersImpl.AddmisionDate > new DateTime(1900, 1, 1))
							{
								text = text + "<AddmisionDate>" + Convert.ToDateTime(batchNumbersImpl.AddmisionDate).ToString("yyyyMMdd") + "</AddmisionDate>";
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.Location))
							{
								text = text + "<Location>" + Function.RawText(batchNumbersImpl.Location) + "</Location>";
							}
							if (!string.IsNullOrEmpty(batchNumbersImpl.Notes))
							{
								text = text + "<Notes>" + Function.RawText(batchNumbersImpl.Notes) + "</Notes>";
							}
							if (!batchNumbersImpl.Quantity.Equals(0.0))
							{
								text = text + "<Quantity>" + Function.ParseDouble(batchNumbersImpl.Quantity) + "</Quantity>";
							}
							if (batchNumbersImpl.BaseLineNumber >= 0)
							{
								text = text + "<BaseLineNumber>" + Function.ParseInt(batchNumbersImpl.BaseLineNumber) + "</BaseLineNumber>";
							}
							if (0 < batchNumbersImpl.TrackingNote)
							{
								text = text + "<TrackingNote>" + Function.RawText(batchNumbersImpl.TrackingNote) + "</TrackingNote>";
							}
							if (0 < batchNumbersImpl.TrackingNoteLine)
							{
								text = text + "<TrackingNoteLine>" + Function.RawText(batchNumbersImpl.TrackingNoteLine) + "</TrackingNoteLine>";
							}
							text += DataService.MappingUdf(batchNumbersImpl.UserFields);
							text += "</row>";
						}
					}
					text += "</BatchNumbers>";
				}
				if (doc.Lines != null && doc.Lines.Count > 0)
				{
					text += "<SerialNumbers>";
					for (int l = 0; l < doc.Lines.Count; l++)
					{
						if (doc.Lines[l].SerialNumbers == null || doc.Lines[l].SerialNumbers.Count <= 0)
						{
							continue;
						}
						for (int m = 0; m < doc.Lines[l].SerialNumbers.Count; m++)
						{
							text += "<row>";
							SerialNumbersImpl serialNumbersImpl = doc.Lines[l].SerialNumbers[m];
							if (!string.IsNullOrEmpty(serialNumbersImpl.BatchID))
							{
								text = text + "<BatchID>" + Function.ToString(serialNumbersImpl.BatchID) + "</BatchID>";
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.ManufacturerSerialNumber))
							{
								text = text + "<ManufacturerSerialNumber>" + Function.RawText(serialNumbersImpl.ManufacturerSerialNumber) + "</ManufacturerSerialNumber>";
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.InternalSerialNumber))
							{
								text = text + "<InternalSerialNumber>" + Function.RawText(serialNumbersImpl.InternalSerialNumber) + "</InternalSerialNumber>";
							}
							if (serialNumbersImpl.ExpiryDate.HasValue && serialNumbersImpl.ExpiryDate > new DateTime(1900, 1, 1))
							{
								text = text + "<ExpiryDate>" + Convert.ToDateTime(serialNumbersImpl.ExpiryDate).ToString("yyyyMMdd") + "</ExpiryDate>";
							}
							if (serialNumbersImpl.ManufactureDate.HasValue && serialNumbersImpl.ManufactureDate > new DateTime(1900, 1, 1))
							{
								text = text + "<ManufacturingDate>" + Convert.ToDateTime(serialNumbersImpl.ManufactureDate).ToString("yyyyMMdd") + "</ManufacturingDate>";
							}
							if (serialNumbersImpl.ReceptionDate.HasValue && serialNumbersImpl.ReceptionDate > new DateTime(1900, 1, 1))
							{
								text = text + "<ReceptionDate>" + Convert.ToDateTime(serialNumbersImpl.ReceptionDate).ToString("yyyyMMdd") + "</ReceptionDate>";
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.Location))
							{
								text = text + "<Location>" + Function.RawText(serialNumbersImpl.Location) + "</Location>";
							}
							if (!string.IsNullOrEmpty(serialNumbersImpl.Notes))
							{
								text = text + "<Notes>" + Function.RawText(serialNumbersImpl.Notes) + "</Notes>";
							}
							if (!serialNumbersImpl.Quantity.Equals(0.0))
							{
								text = text + "<Quantity>" + Function.ParseDouble(serialNumbersImpl.Quantity) + "</Quantity>";
							}
							if (serialNumbersImpl.BaseLineNumber >= 0)
							{
								text = text + "<BaseLineNumber>" + Function.ParseInt(serialNumbersImpl.BaseLineNumber) + "</BaseLineNumber>";
							}
							if (0 < serialNumbersImpl.TrackingNote)
							{
								text = text + "<TrackingNote>" + Function.RawText(serialNumbersImpl.TrackingNote) + "</TrackingNote>";
							}
							if (0 < serialNumbersImpl.TrackingNoteLine)
							{
								text = text + "<TrackingNoteLine>" + Function.RawText(serialNumbersImpl.TrackingNoteLine) + "</TrackingNoteLine>";
							}
							text += DataService.MappingUdf(serialNumbersImpl.UserFields);
							text += "</row>";
						}
					}
					text += "</SerialNumbers>";
				}
				if (doc.Lines != null && doc.Lines.Count > 0)
				{
					text += "<DocumentLinesBinAllocations>";
					for (int n = 0; n < doc.Lines.Count; n++)
					{
						if (doc.Lines[n].BinAllocations == null || doc.Lines[n].BinAllocations.Count <= 0)
						{
							continue;
						}
						for (int num = 0; num < doc.Lines[n].BinAllocations.Count; num++)
						{
							text += "<row>";
							DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = doc.Lines[n].BinAllocations[num];
							if (!documentLinesBinAllocationsImpl.BinAbsEntry.Equals(0))
							{
								text = text + "<BinAbsEntry>" + Function.ParseInt(documentLinesBinAllocationsImpl.BinAbsEntry) + "</BinAbsEntry>";
							}
							if (!documentLinesBinAllocationsImpl.Quantity.Equals(0.0))
							{
								text = text + "<Quantity>" + Function.ParseDouble(documentLinesBinAllocationsImpl.Quantity) + "</Quantity>";
							}
							if (documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine >= 0)
							{
								text = text + "<SerialAndBatchNumbersBaseLine>" + Function.ParseInt(documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine) + "</SerialAndBatchNumbersBaseLine>";
							}
							if (documentLinesBinAllocationsImpl.BaseLineNumber >= 0)
							{
								text = text + "<BaseLineNumber>" + Function.ParseInt(documentLinesBinAllocationsImpl.BaseLineNumber) + "</BaseLineNumber>";
							}
							text += "</row>";
						}
					}
					text += "</DocumentLinesBinAllocations>";
				}
			}
			text += "</BO></BOM>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	public void Dispose()
	{
	}
}
