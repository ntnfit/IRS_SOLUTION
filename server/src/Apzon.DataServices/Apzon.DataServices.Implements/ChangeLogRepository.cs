using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class ChangeLogRepository : Repository<ChangeLogDifferancesParams>, IChangeLog, IRepository<ChangeLogDifferancesParams>
{
	private readonly IUnitOfWork _unitOfWork;

	public ChangeLogRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public virtual DataTable GetChangeLog(GetChangeLogParams pr, string sessionId)
	{
		string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ChangeLogsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <GetChangeLog xmlns=\"ChangeLogsService\">\r\n                                <GetChangeLogParams>\r\n                                    <PrimaryKey>" + pr.PrimaryKey + "</PrimaryKey>\r\n                                    <UDOObjectCode>" + pr.UDOObjectCode + "</UDOObjectCode>\r\n                                    <Object>" + pr.Object.ToString() + "</Object>\r\n                                </GetChangeLogParams>\r\n                              </GetChangeLog>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
		if (DataService.Process(xmlObject).Content is XmlElement xmlElement)
		{
			XElement xElement = XElement.Parse(xmlElement.InnerXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", xElement2.Value);
				}
			}
			else
			{
				DataSet dataSet = new DataSet();
				using (StringReader reader = new StringReader(Function.ToString(xElement)))
				{
					dataSet.ReadXml(reader);
				}
				if (dataSet.Tables.Contains("ChangeLogParams"))
				{
					return dataSet.Tables["ChangeLogParams"];
				}
			}
		}
		return new DataTable();
	}

	public virtual DataTable GetChangeLogDifferences(GetChangeLogParams pr, string sessionId)
	{
		string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ChangeLogsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <GetChangeLogDifferences xmlns=\"ChangeLogsService\">\r\n                                <ShowDifferenceParams>\r\n                                    <PrimaryKey>" + pr.PrimaryKey + "</PrimaryKey>\r\n                                    <UDOObjectCode>" + pr.UDOObjectCode + "</UDOObjectCode>\r\n                                    <Object>" + pr.Object.ToString() + "</Object>\r\n                                    <LogInstance2>" + pr.LogInstance2 + "</LogInstance2>\r\n                                    <LogInstance>" + pr.LogInstance + "</LogInstance>\r\n                                </ShowDifferenceParams>\r\n                              </GetChangeLogDifferences>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
		if (DataService.Process(xmlObject).Content is XmlElement xmlElement)
		{
			XElement xElement = XElement.Parse(xmlElement.InnerXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", xElement2.Value);
				}
			}
			else
			{
				DataSet dataSet = new DataSet();
				using (StringReader reader = new StringReader(Function.ToString(xElement)))
				{
					dataSet.ReadXml(reader);
				}
				if (dataSet.Tables.Contains("ChangeLogDifferenceParams"))
				{
					return dataSet.Tables["ChangeLogDifferenceParams"];
				}
			}
		}
		return new DataTable();
	}
}
