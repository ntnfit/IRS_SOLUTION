using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.DataServices.Interfaces;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class DimensionRepository : Repository<DataTable>, IDimension, IRepository<DataTable>, IDisposable
{
	public DimensionRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public async Task<string> Save(string sessionId, DataTable dt)
	{
		try
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				return "No data to save - DataTable is null or does not have 5 rows";
			}
			string text = "";
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				text = await UpdateData(sessionId, dt.Rows[i]);
				if (!string.IsNullOrEmpty(text))
				{
					break;
				}
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public virtual async Task<string> UpdateData(string sessionId, DataRow row)
	{
		try
		{
			string text = Mapping(row, EnumCommandType.Update);
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>DimensionsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <UpdateDimension xmlns=\"DimensionsService\">" + Function.ToString(text) + "</UpdateDimension>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
			XmlElement xmlElement = (await DataService.ProcessAsync(xmlObject)).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return xElement2.Value;
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

	public virtual string Mapping(DataRow row, EnumCommandType commandType)
	{
		string text = "";
		try
		{
			text += "<Dimension>";
			text = text + "<DimensionCode>" + Function.ToString(row["DimCode"]) + "</DimensionCode>";
			text = text + "<DimensionName>" + Function.RawText(row["DimName"]) + "</DimensionName>";
			text = ((!(Function.ToString(row["DimActive"]) == "Y")) ? (text + "<IsActive>tNO</IsActive>") : (text + "<IsActive>tYES</IsActive>"));
			text = text + "<DimensionDescription>" + Function.RawText(row["DimDesc"]) + "</DimensionDescription>";
			text += "</Dimension>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	public bool GetByParamaters(string sessionId, string dimCode)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>DimensionsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <GetDimension xmlns=\"DimensionsService\">\r\n                                     <Dimension><DimensionCode>" + Function.RawText(dimCode) + "</DimensionCode></Dimension>\r\n                                    </GetDimension>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault") && xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text")) != null)
			{
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	public void Dispose()
	{
		Dispose();
	}
}
