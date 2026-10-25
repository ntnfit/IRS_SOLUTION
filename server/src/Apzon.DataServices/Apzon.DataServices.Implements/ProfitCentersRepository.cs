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

public class ProfitCentersRepository : Repository<DataTable>, IProfitCenters, IRepository<DataTable>, IDisposable
{
	public ProfitCentersRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public virtual string Save(string sessionId, DataTable dt)
	{
		try
		{
			string text = "";
			return string.IsNullOrEmpty(GetByParams(Function.ToString(dt.Rows[0]["PrcCode"]), sessionId)) ? Process(dt.Rows[0], sessionId, "AddProfitCenter") : Process(dt.Rows[0], sessionId, "UpdateProfitCenter");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	private string Process(DataRow row, string sessionId, string command)
	{
		try
		{
			string text = Mapping(row, sessionId);
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ProfitCentersService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <" + command + " xmlns=\"ProfitCentersService\">" + text + "</" + command + ">\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
			if (!(DataService.Process(xmlObject).Content is XmlElement xmlElement))
			{
				return "";
			}
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					throw new Exception(xElement2.Value);
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public virtual string Mapping(DataRow row, string sessionId)
	{
		string text = "";
		try
		{
			text += "<ProfitCenter>";
			text = text + "<CenterCode>" + Function.RawText(row["PrcCode"]) + "</CenterCode>";
			text = text + "<CenterName>" + Function.RawText(row["PrcName"]) + "</CenterName>";
			text = text + "<InWhichDimension>" + Function.ParseDouble(row["DimCode"]) + "</InWhichDimension>";
			text = text + "<EffectiveFrom>" + Function.ParseDateTime(row["ValidFrom"]).Value.ToString("yyyyMMdd") + "</EffectiveFrom>";
			text = ((!(Function.ToString(row["Active"]) == "Y")) ? (text + "<Active>tNO</Active>") : (text + "<Active>tYES</Active>"));
			text += DataService.MappingUdf(row);
			text += "</ProfitCenter>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	public virtual string Remove(string sessionId, string prcCode)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ProfitCentersService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <DeleteProfitCenter xmlns=\"ProfitCentersService\">\r\n                                        <ProfitCenterParams>\r\n                                            <CenterCode>" + prcCode + "</CenterCode>\r\n                                        </ProfitCenterParams>>\r\n                                    </DeleteProfitCenter>\r\n                                </soap:Body>  \r\n                            </soap:Envelope>";
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
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string GetByParams(string prcCode, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ProfitCentersService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <GetProfitCenter xmlns=\"ItemsService\">\r\n                                     <ProfitCenterParams><CenterCode>" + prcCode + "</CenterCode></ProfitCenterParams>\r\n                                    </GetProfitCenter>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
			if (!(DataService.Process(xmlObject).Content is XmlElement xmlElement))
			{
				return "";
			}
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault") && xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text")) != null)
			{
				return "";
			}
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("ProfitCenter"))
			{
				return xmlElement.OuterXml;
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}
}
