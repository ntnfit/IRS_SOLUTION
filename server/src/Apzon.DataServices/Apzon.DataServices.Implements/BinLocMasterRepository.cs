using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.DataServices.Interfaces;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class BinLocMasterRepository : Repository<DataTable>, IBinLocMaster, IDisposable
{
	public BinLocMasterRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public virtual string Save(string sessionId, DataTable doc, string action)
	{
		try
		{
			EnumCommandType type = ((action == "A") ? EnumCommandType.Add : EnumCommandType.Update);
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>BinLocationsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>" + Mapping(sessionId, doc.Rows[0], type) + "</soap:Body>  \r\n                            </soap:Envelope>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Warehouse Sublevel - {num}");
					return Function.ToString(num);
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

	public virtual string Delete(string sessionId, int absEntry)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>BinLocationsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Delete xmlsn = \"BinLocationsService\"><BinLocation><AbsEntry>" + absEntry + "</AbsEntry></BinLocation></Delete>\r\n                                </soap:Body>  \r\n                            </soap:Envelope>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Warehouse Sublevel - {num}");
					return Function.ToString(num);
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

	public virtual string Mapping(string sessionId, DataRow row, EnumCommandType type)
	{
		string text = "";
		try
		{
			string text2 = ((type == EnumCommandType.Add) ? "Add" : "Update");
			text = text + "<" + text2 + " xmlsn = \"BinLocationsService\"><BinLocation>";
			if (text2 == "Update")
			{
				text = text + "<AbsEntry>" + Function.ParseInt(row["AbsEntry"]) + "</AbsEntry>";
			}
			text = text + "<AlternativeSortCode>" + Function.RawText(row["AltSortCod"]) + "</AlternativeSortCode>";
			text = text + "<Attribute1>" + Function.RawText(row["Attr1Val"]) + "</Attribute1>";
			text = text + "<Attribute2>" + Function.RawText(row["Attr2Val"]) + "</Attribute2>";
			text = text + "<Attribute3>" + Function.RawText(row["Attr3Val"]) + "</Attribute3>";
			text = text + "<Attribute4>" + Function.RawText(row["Attr4Val"]) + "</Attribute4>";
			text = text + "<Attribute5>" + Function.RawText(row["Attr5Val"]) + "</Attribute5>";
			text = text + "<Attribute6>" + Function.RawText(row["Attr6Val"]) + "</Attribute6>";
			text = text + "<Attribute7>" + Function.RawText(row["Attr7Val"]) + "</Attribute7>";
			text = text + "<Attribute8>" + Function.RawText(row["Attr8Val"]) + "</Attribute8>";
			text = text + "<Attribute9>" + Function.RawText(row["Attr9Val"]) + "</Attribute9>";
			text = text + "<Attribute10>" + Function.RawText(row["Attr10Val"]) + "</Attribute10>";
			text = text + "<BarCode>" + Function.RawText(row["BarCode"]) + "</BarCode>";
			text = ((!Function.ToString(row["SngBatch"]).Equals("Y")) ? (text + "<BatchRestrictions>brbNoRestrictions</BatchRestrictions>") : (text + "<BatchRestrictions>brbSingleBatch</BatchRestrictions>"));
			text = text + "<BinCode>" + Function.RawText(row["BinCode"]) + "</BinCode>";
			if (!string.IsNullOrEmpty(Function.ToString(Function.ToString(row["RtrictDate"]))))
			{
				text = text + "<DateRestrictionChanged>" + Convert.ToDateTime(Function.ToString(row["RtrictDate"])).ToString("yyyyMMdd") + "</DateRestrictionChanged>";
			}
			text = text + "<Description>" + Function.RawText(row["Descr"]) + "</Description>";
			text = ((!Function.ToString(row["NoAutoAllc"]).Equals("Y")) ? (text + "<ExcludeAutoAllocOnIssue>tNO</ExcludeAutoAllocOnIssue>") : (text + "<ExcludeAutoAllocOnIssue>tYES</ExcludeAutoAllocOnIssue>"));
			text = ((!Function.ToString(row["Disabled"]).Equals("Y")) ? (text + "<Inactive>tNO</Inactive>") : (text + "<Inactive>tYES</Inactive>"));
			text = ((!Function.ToString(row["SysBin"]).Equals("Y")) ? (text + "<IsSystemBin>tNO</IsSystemBin>") : (text + "<IsSystemBin>tYES</IsSystemBin>"));
			text = text + "<MaximumQty>" + Function.ParseDecimal(row["MaxLevel"]) + "</MaximumQty>";
			text = text + "<MaximumWeight>" + Function.ParseDecimal(row["MaxWeight1"]) + "</MaximumWeight>";
			text = text + "<MaximumWeight1>" + Function.ParseDecimal(row["MaxWeight2"]) + "</MaximumWeight1>";
			text = text + "<MaximumWeightUnit>" + Function.RawText(row["Wght1Unit"]) + "</MaximumWeightUnit>";
			text = text + "<MaximumWeightUnit1>" + Function.RawText(row["Wght2Unit"]) + "</MaximumWeightUnit1>";
			text = text + "<MinimumQty>" + Function.ParseDecimal(row["MinLevel"]) + "</MinimumQty>";
			text = ((!Function.ToString(row["ReceiveBin"]).Equals("Y")) ? (text + "<ReceivingBinLocation>tNO</ReceivingBinLocation>") : (text + "<ReceivingBinLocation>tYES</ReceivingBinLocation>"));
			switch (Function.ParseInt(row["ItmRtrictT"]))
			{
			case 0:
				text += "<RestrictedItemType>briNone</RestrictedItemType>";
				break;
			case 1:
				text += "<RestrictedItemType>briSpecificItem</RestrictedItemType>";
				break;
			case 2:
				text += "<RestrictedItemType>briSingleItemOnly</RestrictedItemType>";
				break;
			case 3:
				text += "<RestrictedItemType>briSpecificItemGroup</RestrictedItemType>";
				break;
			case 4:
				text += "<RestrictedItemType>briSpecificItemGroupOnly</RestrictedItemType>";
				break;
			}
			switch (Function.ParseInt(row["UoMRtrict"]))
			{
			case 0:
				text += "<RestrictedUoMType>bruNone</RestrictedUoMType>";
				break;
			case 1:
				text += "<RestrictedUoMType>bruSpecificUoM</RestrictedUoMType>";
				break;
			case 2:
				text += "<RestrictedUoMType>bruSingleUoMOnly</RestrictedUoMType>";
				break;
			case 3:
				text += "<RestrictedUoMType>bruSpecificUoMGroup</RestrictedUoMType>";
				break;
			case 4:
				text += "<RestrictedUoMType>bruSpecificUoMGroupOnly</RestrictedUoMType>";
				break;
			}
			switch (Function.ParseInt(row["RtrictType"]))
			{
			case 0:
				text += "<RestrictedTransType>brtNoRestrictions</RestrictedTransType>";
				break;
			case 1:
				text += "<RestrictedTransType>brtAllTrans</RestrictedTransType>";
				break;
			case 2:
				text += "<RestrictedTransType>brtInboundTrans</RestrictedTransType>";
				break;
			case 3:
				text += "<RestrictedTransType>brtOutboundTrans</RestrictedTransType>";
				break;
			case 4:
				text += "<RestrictedTransType>brtAllExceptInventoryTrans</RestrictedTransType>";
				break;
			}
			text = text + "<RestrictionReason>" + Function.RawText(row["RtrictResn"]) + "</RestrictionReason>";
			text = text + "<SpecificItem>" + Function.RawText(row["SpcItmCode"]) + "</SpecificItem>";
			text = text + "<SpecificUoM>" + Function.RawText(row["SpcUoMCode"]) + "</SpecificUoM>";
			text = text + "<SpecificItemGroup>" + Function.RawText(row["SpcItmGrpC"]) + "</SpecificItemGroup>";
			text = text + "<SpecificUoMGroup>" + Function.RawText(row["SpcUgpCode"]) + "</SpecificUoMGroup>";
			text = text + "<Sublevel1>" + Function.RawText(row["SL1Code"]) + "</Sublevel1>";
			text = text + "<Sublevel2>" + Function.RawText(row["SL2Code"]) + "</Sublevel2>";
			text = text + "<Sublevel3>" + Function.RawText(row["SL3Code"]) + "</Sublevel3>";
			text = text + "<Sublevel4>" + Function.RawText(row["SL4Code"]) + "</Sublevel4>";
			text = text + "<Warehouse>" + Function.RawText(row["WhsCode"]) + "</Warehouse>";
			text = text + "</BinLocation></" + text2 + ">";
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
