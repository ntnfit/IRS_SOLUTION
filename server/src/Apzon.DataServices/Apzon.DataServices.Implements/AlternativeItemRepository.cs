using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.DataServices.Interfaces;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class AlternativeItemRepository : Repository<DataSet>, IAlternativeItem, IDisposable
{
	public AlternativeItemRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public void Dispose()
	{
		Dispose();
	}

	public async Task<string> Save(string sessionId, Tuple<string, DataTable> tuple)
	{
		_ = 1;
		try
		{
			DataTable item = tuple.Item2;
			string item2 = tuple.Item1;
			if (item == null || item.Rows.Count == 0)
			{
				return "No data to save";
			}
			if (Exist(sessionId, tuple.Item1))
			{
				return await UpdateData(sessionId, item, item2);
			}
			return await AddNew(sessionId, item, item2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public virtual async Task<string> DeleteData(string sessionId, string itemNo)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>AlternativeItemsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <DeleteItem xmlns=\"AlternativeItemsService\">\r\n                                     <OriginalItemParams>   \r\n                                        <ItemCode>" + Function.RawText(itemNo) + "</ItemCode>\r\n                                     </OriginalItemParams>\r\n                                    </DeleteItem>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
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

	public virtual async Task<string> AddNew(string sessionId, DataTable dt, string itemNo)
	{
		try
		{
			string text = Mapping(itemNo, dt);
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>AlternativeItemsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <AddItem xmlns=\"AlternativeItemsService\">" + text + "</AddItem>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
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

	public virtual async Task<string> UpdateData(string sessionId, DataTable dt, string itemNo)
	{
		try
		{
			string text = Mapping(itemNo, dt);
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>AlternativeItemsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <UpdateItem xmlns=\"AlternativeItemsService\">" + text + "</UpdateItem>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
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

	protected bool Exist(string sessionId, string itemNo)
	{
		try
		{
			string text = ExecuteScarlar("Select top 1 1 from OALI where \"OrigItem\" = @itemCode;", CommandType.Text, new SqlParameter("@itemCode", itemNo));
			if (!string.IsNullOrEmpty(text) && text.Equals("1"))
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	protected virtual string Mapping(string itemNo, DataTable dt)
	{
		string text = "";
		try
		{
			text += "<OriginalItem>";
			text = text + "<ItemCode>" + Function.RawText(itemNo) + "</ItemCode>";
			text += "<AlternativeItems>";
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				text += "<AlternativeItem>";
				text = text + "<AlternativeItemCode>" + Function.RawText(dt.Rows[i]["AltItem"]) + "</AlternativeItemCode>";
				text = text + "<MatchFactor>" + Function.ParseDecimal(dt.Rows[i]["Match"]) + "</MatchFactor>";
				if (!string.IsNullOrWhiteSpace(Function.ToString(dt.Rows[i]["Remarks"])))
				{
					text = text + "<Remarks>" + Function.RawText(dt.Rows[i]["Remarks"]) + "</Remarks>";
				}
				text += "</AlternativeItem>";
			}
			text += "</AlternativeItems>";
			return text + "</OriginalItem>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
