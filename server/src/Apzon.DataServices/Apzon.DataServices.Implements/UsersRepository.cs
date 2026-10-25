using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Administrator;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class UsersRepository : Repository<UsersImpl>, IUsers, IRepository<UsersImpl>
{
	protected IUnitOfWork UnitOfWork;

	public UsersRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService, IUnitOfWork dataUnitOfWork)
		: base(unitOfWork, dataService)
	{
		UnitOfWork = dataUnitOfWork;
	}

	public string Save(UsersImpl doc, string sessionId)
	{
		try
		{
			if (!Exists(doc.USERID, sessionId))
			{
				return Process(doc, sessionId, EnumCommandType.Add);
			}
			return Process(doc, sessionId, EnumCommandType.Update);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private bool Exists(int userID, string sessionId)
	{
		try
		{
			string text = ExecuteScarlar("select 1 from OUSR where USERID = @userId;", CommandType.Text, new SqlParameter("@userId", userID));
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

	protected virtual string Process(UsersImpl doc, string sessionId, EnumCommandType commandType)
	{
		try
		{
			string text = Mapping(doc, commandType, sessionId);
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>UsersService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>" + text + "\r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				string result = "";
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					result = xElement2.Value;
					switch (commandType)
					{
					case EnumCommandType.Add:
						Logging.Write("ERROR", "Add new user error: " + xElement2.Value);
						break;
					case EnumCommandType.Update:
						Logging.Write("ERROR", "Update user error: " + xElement2.Value);
						break;
					}
				}
				return result;
			}
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					string text2 = Function.ToString(xElement3.Value);
					if (!string.IsNullOrWhiteSpace(text2))
					{
						switch (commandType)
						{
						case EnumCommandType.Add:
							Logging.Write("ERROR", $"Add user -[{text2}] success.");
							break;
						case EnumCommandType.Update:
							Logging.Write("ERROR", $"Update user -[{text2}] success.");
							break;
						}
					}
				}
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}

	protected virtual string Mapping(UsersImpl doc, EnumCommandType commandType, string sessionId)
	{
		string text = "";
		try
		{
			text = ((commandType != EnumCommandType.Update) ? (text + "<Add xmlns=\"UsersService\">") : (text + "<Update xmlns=\"UsersService\">"));
			text += "<User>";
			if (commandType == EnumCommandType.Update)
			{
				text = text + "<InternalKey>" + doc.USERID + "</InternalKey>";
			}
			if (commandType == EnumCommandType.Add || doc.IsChangePassword)
			{
				text = text + "<UserPassword>" + Function.RawText(doc.PASSWORD) + "</UserPassword>";
			}
			text = text + "<UserCode>" + Function.RawText(doc.USER_CODE) + "</UserCode>";
			text = text + "<UserName>" + Function.RawText(doc.U_NAME) + "</UserName>";
			text = ((!(Function.ToString(doc.SUPERUSER).ToLower() == "y")) ? (text + "<Superuser>tNO</Superuser>") : (text + "<Superuser>tYES</Superuser>"));
			text = text + "<eMail>" + Function.RawText(doc.E_Mail) + "</eMail>";
			text = text + "<MobilePhoneNumber>" + Function.RawText(doc.PortNum) + "</MobilePhoneNumber>";
			text = text + "<FaxNumber>" + Function.RawText(doc.Fax) + "</FaxNumber>";
			text = ((doc.Branch <= 0) ? (text + "<Branch>-2</Branch>") : (text + "<Branch>" + doc.Branch + "</Branch>"));
			text = ((doc.Department <= 0) ? (text + "<Department>-2</Department>") : (text + "<Department>" + doc.Department + "</Department>"));
			text = ((!(Function.ToString(doc.Locked).ToLower() == "y")) ? (text + "<Locked>tNO</Locked>") : (text + "<Locked>tYES</Locked>"));
			text += "<Group>ug_Regular</Group>";
			text = text + "<Defaults>" + Function.RawText(doc.DfltsGroup) + "</Defaults>";
			text += DataService.MappingUdf(doc.UserFields);
			if (doc.UserBranchAssignment != null)
			{
				text += "<UserBranchAssignment>";
				for (int i = 0; i < doc.UserBranchAssignment.Count; i++)
				{
					text += "<UserBranchAssignmentItem>";
					text = text + "<BPLID>" + doc.UserBranchAssignment[i].BPLId + "</BPLID>";
					text += "</UserBranchAssignmentItem>";
				}
				text += "</UserBranchAssignment>";
			}
			text += "</User>";
			text = ((commandType != EnumCommandType.Update) ? (text + "</Add>") : (text + "</Update>"));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return text;
	}

	public string GetByParams(string sessionId, int userId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>UsersService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <GetByParams xmlns=\"UsersService\">\r\n                                     <UserParams>   <InternalKey>" + userId + "</InternalKey></UserParams>\r\n                                    </GetByParams>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
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
			return xmlElement.OuterXml;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return string.Empty;
	}

	public void Dispose()
	{
	}

	public string Delete(string userCode, string sessionId)
	{
		throw new NotImplementedException();
	}

	public virtual string DeleteDoc(int code, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>UsersService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Remove xmlns=\"UsersService\">\r\n                                     <UserParams><InternalKey>" + code + "</InternalKey></UserParams>\r\n                                    </Remove>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
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
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
	}
}
