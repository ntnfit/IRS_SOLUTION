using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class CreditCardRepository : Repository<DataTable>, ICreditCard, IRepository<DataTable>, IDisposable
{
	public CreditCardRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public void Dispose()
	{
		Dispose();
	}

	public string Save(DataTable data, string sessionId)
	{
		try
		{
			if (data != null && data.Rows.Count > 0)
			{
				return AddNew(data, sessionId, ApzObjectTypes.oCreditCards).Message;
			}
			return "Loi du lieu!";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string UpdateData(DataTable data, string sessionId, ApzObjectTypes objType)
	{
		try
		{
			DataService.GetBusinessObjectTemplate(sessionId, objType.ToString());
			string text = "";
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text + "</dis:UpdateObject></env:Body></env:Envelopes>";
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
					Logging.Write("WATCH", $"update document - {objType} - DocEntry - {num}");
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

	protected virtual HttpResult AddNew(DataTable data, string sessionId, ApzObjectTypes objType)
	{
		try
		{
			DataService.GetBusinessObjectTemplate(sessionId, objType.ToString());
			string text = "";
			for (int i = 0; i < data.Rows.Count; i++)
			{
				text = (Exist(Function.ParseInt(data.Rows[i]["CreditCard"])) ? (text + "<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + Mapping(data.Rows[i], sessionId, EnumCommandType.Update, objType) + "</dis:UpdateObject>") : (text + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + Mapping(data.Rows[i], sessionId, EnumCommandType.Add, objType) + "</dis:AddObject>"));
			}
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "</env:Body></env:Envelopes>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
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
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Add new document - {objType} - DocEntry - {num}");
					return new HttpResult(Enums.MessageCode.Success, "", num);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	private string Mapping(DataRow data, string sessionId, EnumCommandType commandType, ApzObjectTypes objType)
	{
		string text = "<BOM>\r\n                          <BO>\r\n                          <AdmInfo>\r\n                            <Object>" + objType.ToString() + "</Object>\r\n                          </AdmInfo>";
		try
		{
			if (commandType == EnumCommandType.Update)
			{
				text = text + "<QueryParams><CreditCardCode>" + Function.ParseInt(data["CreditCard"]) + "</CreditCardCode></QueryParams>";
			}
			text += " <CreditCards>\r\n                            <row>";
			if (!string.IsNullOrEmpty(Function.ToString(data["CardName"])))
			{
				text = text + "<CreditCardName>" + Function.RawText(data["CardName"]) + "</CreditCardName>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(data["AcctCode"])))
			{
				text = text + "<GLAccount>" + Function.RawText(data["AcctCode"]) + "</GLAccount>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(data["Phone"])))
			{
				text = text + "<Telephone>" + Function.RawText(data["Phone"]) + "</Telephone>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(data["CompanyId"])))
			{
				text = text + "<CompanyID>" + Function.RawText(data["CompanyId"]) + "</CompanyID>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(data["Country"])))
			{
				text = text + "<CountryCode>" + Function.RawText(data["Country"]) + "</CountryCode>";
			}
			text += "</row></CreditCards>";
			text += "</BO></BOM>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	protected bool Exist(int creditCard)
	{
		try
		{
			if (Function.ParseInt(ExecuteScarlar("select top 1 1 from OCRC where \"CreditCard\" = @creditCard;", CommandType.Text, new SqlParameter("@creditCard", creditCard))) < 1)
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
}
