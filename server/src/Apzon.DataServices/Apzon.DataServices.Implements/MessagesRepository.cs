using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class MessagesRepository : Repository<string>, IMessage
{
	public MessagesRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public MessageHeaderImpl SendMessage(string sessionId, string msg)
	{
		string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>\r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>MessagesService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <SendMessage xmlns=\"MessagesService\">" + msg + "</SendMessage> \r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
		XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
		XElement xElement = XElement.Parse(xmlElement.OuterXml);
		if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
		{
			XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
			if (xElement2 != null)
			{
				throw new Exception(xElement2.Value);
			}
			throw new Exception("[DI-SERVER]Send message - Unknown Error");
		}
		if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Code"))
		{
			XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Code"));
			XElement xElement4 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("SendDate"));
			XElement xElement5 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("SendTime"));
			if (xElement3 != null && xElement4 != null && xElement5 != null)
			{
				return new MessageHeaderImpl
				{
					Code = Function.ParseInt(xElement3.Value),
					SentDate = (Function.ParseDateTime(xElement4) ?? DateTime.Today)
				};
			}
		}
		return new MessageHeaderImpl();
	}

	public virtual string GetMessage(string subject, string content, string firstColumn, string firstLine, string objType, string objKey, string repName)
	{
		return "<Message>\r\n                    <Subject>" + subject + "</Subject>\r\n                    <Text>" + content + "</Text>\r\n                    <MessageDataColumns>\r\n                        <MessageDataColumn>\r\n                                <ColumnName>Request for Document Approval</ColumnName>\r\n                                <Link>tYES</Link>\r\n                                <MessageDataLines>\r\n                                    <MessageDataLine>\r\n                                        <Value>" + firstLine + "</Value>\r\n                                        <Object>" + objType + "</Object>\r\n                                        <ObjectKey>" + objKey + "</ObjectKey>\r\n                                    </MessageDataLine>\r\n                                </MessageDataLines>\r\n                        </MessageDataColumn>\r\n                    </MessageDataColumns>\r\n                        <RecipientCollection>\r\n                            <Recipient>\r\n                                <UserCode>" + repName + "</UserCode>\r\n                                <SendInternal>tYES</SendInternal>\r\n                            </Recipient>\r\n                        </RecipientCollection>\r\n                    </Message>";
	}
}
