using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.DataServices.Interfaces;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class SerialNumberDetailsRepository : Repository<DataSet>, ISerialNumberDetails, IDisposable
{
	protected readonly IUnitOfWork _unitOfWork;

	public SerialNumberDetailsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public void Dispose()
	{
	}

	public virtual string Save(DataTable dt, string sessionId)
	{
		try
		{
			if (dt.IsNotNull())
			{
				if (Function.ParseInt(dt.Rows[0]["AbsEntry"]) <= 0)
				{
					return "No matching record found";
				}
				string text = Mapping(dt.Rows[0]);
				string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>SerialNumberDetailsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Update xmlns=\"SerialNumberDetailsService\">" + text + "</Update>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
				XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
				XElement xElement = XElement.Parse(xmlElement.OuterXml);
				if (string.IsNullOrEmpty(xmlElement.OuterXml) || !xmlElement.OuterXml.Contains("Fault"))
				{
					return "";
				}
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return xElement2.Value;
				}
			}
			return "Data is empty";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string Mapping(DataRow dr)
	{
		try
		{
			string text = "<SerialNumberDetail>";
			if (Function.ParseInt(dr["AbsEntry"]) > 0)
			{
				text = text + "<DocEntry>" + Function.ParseInt(dr["AbsEntry"]) + "</DocEntry>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dr["ItemCode"])))
			{
				text = text + "<ItemCode>" + Function.ToString(dr["ItemCode"]) + "</ItemCode>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dr["itemName"])))
			{
				text = text + "<ItemDescription>" + Function.ParseInt(dr["itemName"]) + "</ItemDescription>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dr["DistNumber"])))
			{
				text = text + "<SerialNumber>" + Function.ToString(dr["DistNumber"]) + "</SerialNumber>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dr["MnfSerial"])))
			{
				text = text + "<MfrSerialNo>" + Function.ToString(dr["MnfSerial"]) + "</MfrSerialNo>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dr["LotNumber"])))
			{
				text = text + "<LotNumber>" + Function.ToString(dr["LotNumber"]) + "</LotNumber>";
			}
			if (Function.ParseDateTime(dr["InDate"]).HasValue)
			{
				text = text + "<AdmissionDate>" + Convert.ToDateTime(dr["InDate"]).ToString("yyyy-MM-dd") + "</AdmissionDate>";
			}
			if (Function.ParseDateTime(dr["MnfDate"]).HasValue)
			{
				text = text + "<ManufacturingDate>" + Convert.ToDateTime(dr["MnfDate"]).ToString("yyyy-MM-dd") + "</ManufacturingDate>";
			}
			if (Function.ParseDateTime(dr["ExpDate"]).HasValue)
			{
				text = text + "<ExpirationDate>" + Convert.ToDateTime(dr["ExpDate"]).ToString("yyyy-MM-dd") + "</ExpirationDate>";
			}
			if (Function.ParseDateTime(dr["GrntStart"]).HasValue)
			{
				text = text + "<MfrWarrantyStart>" + Convert.ToDateTime(dr["GrntStart"]).ToString("yyyy-MM-dd") + "</MfrWarrantyStart>";
			}
			if (Function.ParseDateTime(dr["GrntExp"]).HasValue)
			{
				text = text + "<MFrWarrantyEnd>" + Convert.ToDateTime(dr["GrntExp"]).ToString("yyyy-MM-dd") + "</MFrWarrantyEnd>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dr["Location"])))
			{
				text = text + "<Location>" + Function.ToString(dr["Location"]) + "</Location>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dr["Notes"])))
			{
				text = text + "<Details>" + Function.ToString(dr["Notes"]) + "</Details>";
			}
			if (Function.ParseInt(dr["SysNumber"]) > 0)
			{
				text = text + "<SystemNumber>" + Function.ParseInt(dr["SysNumber"]) + "</SystemNumber>";
			}
			text += DataService.MappingUdf(dr);
			return text + "</SerialNumberDetail>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public string Get(int absEntry, string sessionId)
	{
		string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                                    <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>SerialNumberDetailsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Get xmlns=\"SerialNumberDetailsService\">\r\n                                     <SerialNumberDetailParams>   <DocEntry>" + absEntry + "</DocEntry></SerialNumberDetailParams>\r\n                                    </Get>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
		XmlElement xmlElement = DataService.GetByKey(sessionId, xmlObject).Content as XmlElement;
		if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
		{
			return string.Empty;
		}
		if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("DocEntry"))
		{
			return xmlElement.OuterXml;
		}
		return string.Empty;
	}
}
