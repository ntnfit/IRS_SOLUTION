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

public class BatchNumberDetailsRepository : Repository<DataSet>, IBatchNumberDetails, IDisposable
{
	protected readonly IUnitOfWork _unitOfWork;

	public BatchNumberDetailsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public void Dispose()
	{
	}

	public virtual string Save(DataSet ds, string sessionId)
	{
		try
		{
			if (ds.Tables.Count > 0 && ds.Tables[0].IsNotNull())
			{
				if (Function.ParseInt(ds.Tables[0].Rows[0]["AbsEntry"]) <= 0)
				{
					return "No matching record found";
				}
				string text = Mapping(ds.Tables[0].Rows[0]);
				string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>BatchNumberDetailsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Update xmlns=\"BatchNumberDetailsService\">" + text + "</Update>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
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
			string text = "<BatchNumberDetail>";
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
				text = text + "<Batch>" + Function.ToString(dr["DistNumber"]) + "</Batch>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dr["MnfSerial"])))
			{
				text = text + "<BatchAttribute1>" + Function.ToString(dr["MnfSerial"]) + "</BatchAttribute1>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dr["LotNumber"])))
			{
				text = text + "<BatchAttribute2>" + Function.ToString(dr["LotNumber"]) + "</BatchAttribute2>";
			}
			if (Function.ParseInt(dr["Status"]) >= 0)
			{
				text = Function.ParseInt(dr["Status"]) switch
				{
					1 => text + "<Status>bdsStatus_NotAccessible</Status>", 
					2 => text + "<Status>bdsStatus_Locked</Status>", 
					_ => text + "<Status>bdsStatus_Released</Status>", 
				};
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
			if (!string.IsNullOrEmpty(Function.ToString(dr["Notes"])))
			{
				text = text + "<Details>" + Function.ToString(dr["Notes"]) + "</Details>";
			}
			if (Function.ParseInt(dr["SysNumber"]) > 0)
			{
				text = text + "<SystemNumber>" + Function.ParseInt(dr["SysNumber"]) + "</SystemNumber>";
			}
			text += DataService.MappingUdf(dr);
			return text + "</BatchNumberDetail>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public string Get(int absEntry, string sessionId)
	{
		string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                                    <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>BatchNumberDetailsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Get xmlns=\"BlanketAgreementsService\">\r\n                                     <BatchNumberDetailParams>   <DocEntry>" + absEntry + "</DocEntry></BatchNumberDetailParams>\r\n                                    </Get>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
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
