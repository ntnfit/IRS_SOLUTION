using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class UnitOfMeasureGroupsRepository : Repository<DataTable>, IUnitOfMeasureGroups
{
	public UnitOfMeasureGroupsRepository(Apzon.Services.IUnitOfWork serviceUOW, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUOW, dataService)
	{
	}

	private string Mapping(DataRow row, DataTable dt)
	{
		try
		{
			string text = "";
			text += "<UnitOfMeasurementGroup>";
			if (row != null && dt.IsNotNull())
			{
				text = text + "<AbsEntry>" + Function.RawText(row["UgpEntry"]) + "</AbsEntry>";
				text = text + "<Code>" + Function.RawText(row["UgpCode"]) + "</Code>";
				text = text + "<Name>" + Function.RawText(row["UgpName"]) + "</Name>";
				text = text + "<BaseUoM>" + Function.RawText(row["BaseUom"]) + "</BaseUoM>";
				text += "<UoMGroupDefinitionCollection>";
				foreach (DataRow row2 in dt.Rows)
				{
					text += "<UoMGroupDefinition>";
					text = text + "<AlternateUoM>" + Function.RawText(row2["UomEntry"]) + "</AlternateUoM>";
					text = text + "<AlternateQuantity>" + Function.RawText(row2["AltQty"]) + "</AlternateQuantity>";
					text = text + "<BaseQuantity>" + Function.RawText(row2["BaseQty"]) + "</BaseQuantity>";
					text = text + "<WeightFactor>" + Function.RawText(row2["WghtFactor"]) + "</WeightFactor>";
					text = text + "<UdfFactor>" + Function.RawText(row2["UdfFactor"]) + "</UdfFactor>";
					text += "</UoMGroupDefinition>";
				}
				text += "</UoMGroupDefinitionCollection>";
				text += "</UnitOfMeasurementGroup>";
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	public virtual HttpResult SaveData(Tuple<DataTable, DataTable, Enums.HttpAction> tupData, string sessionId)
	{
		try
		{
			string empty = string.Empty;
			if (tupData.Item3 == Enums.HttpAction.Delete)
			{
				empty = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>UnitOfMeasurementGroupsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Delete xmlns=\"UnitOfMeasurementGroupsService\">\r\n                                     <UnitOfMeasurementGroup>   \r\n                                        <AbsEntry>" + Function.ParseInt(tupData.Item1.Rows[0]["UgpEntry"]) + "</AbsEntry>\r\n                                     </UnitOfMeasurementGroup>\r\n                                    </Delete>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>";
			}
			else
			{
				string text = ((tupData.Item3 == Enums.HttpAction.AddNew) ? "Add" : "Update");
				empty = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>UnitOfMeasurementGroupsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <" + text + " xmlns=\"UnitOfMeasurementGroupsService\">" + Mapping(tupData.Item1.Rows[0], tupData.Item2) + "</" + text + ">\r\n                                </soap:Body>  \r\n                            </soap:Envelope>";
			}
			XmlElement xmlElement = DataService.Process(empty).Content as XmlElement;
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
					return new HttpResult(Enums.MessageCode.Success, "", Function.ParseInt(xElement3.Value));
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
}
