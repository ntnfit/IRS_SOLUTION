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

public class CreditPaymentMethodsRepository : Repository<DataTable>, ICreditPaymentMethods, IRepository<DataTable>, IDisposable
{
	public CreditPaymentMethodsRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
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
				return AddNew(data, sessionId, ApzObjectTypes.oCreditPaymentMethods).Message;
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
				text = (Exist(Function.ParseInt(data.Rows[i]["CrTypeCode"])) ? (text + "<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + Mapping(data.Rows[i], sessionId, EnumCommandType.Update, objType) + "</dis:UpdateObject>") : (text + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + Mapping(data.Rows[i], sessionId, EnumCommandType.Add, objType) + "</dis:AddObject>"));
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
				text = text + "<QueryParams><PaymentMethodCode>" + Function.ParseInt(data["CrTypeCode"]) + "</PaymentMethodCode></QueryParams>";
			}
			text += " <CreditPaymentMethods>\r\n                            <row>";
			if (!string.IsNullOrEmpty(Function.ToString(data["CrTypeName"])))
			{
				text = text + "<Name>" + Function.RawText(data["CrTypeName"]) + "</Name>";
			}
			if (Function.ParseInt(data["CreditCard"]) > 0)
			{
				text = text + "<AssignedtoCreditCard>" + Function.RawText(data["CreditCard"]) + "</AssignedtoCreditCard>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(data["DueTerms"])))
			{
				text = text + "<PaymentCode>" + Function.RawText(data["DueTerms"]) + "</PaymentCode>";
			}
			if (Function.ParseDouble(data["MinCredit"]) > 0.0)
			{
				text = text + "<MinimumCreditAmount>" + Function.RawText(data["MinCredit"]) + "</MinimumCreditAmount>";
			}
			if (Function.ParseDouble(data["MinToPay"]) > 0.0)
			{
				text = text + "<MinimumPaymentAmount>" + Function.RawText(data["MinToPay"]) + "</MinimumPaymentAmount>";
			}
			if (Function.ParseDouble(data["MaxValid"]) > 0.0)
			{
				text = text + "<MaxQtyWithoutApproval>" + Function.RawText(data["MaxValid"]) + "</MaxQtyWithoutApproval>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(data["InstalMent"])))
			{
				text = text + "<InstallmentPaymentsPossible>" + Function.RawText(data["InstalMent"]) + "</InstallmentPaymentsPossible>";
			}
			text += "</row></CreditPaymentMethods>";
			text += "</BO></BOM>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	protected bool Exist(int crTypeCode)
	{
		try
		{
			if (Function.ParseInt(ExecuteScarlar("select top 1 1 from OCRP where \"CrTypeCode\" = @crTypeCode;", CommandType.Text, new SqlParameter("@crTypeCode", crTypeCode))) < 1)
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

	public virtual string DeleteLine(int crTypeCode, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:RemoveObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                             <BOM>\r\n                            <BO>\r\n                             <AdmInfo>\r\n                               <Object>oCreditPaymentMethods</Object>\r\n                             </AdmInfo>\r\n                             <QueryParams>\r\n                               <PaymentMethodCode>" + crTypeCode + "</PaymentMethodCode>\r\n                             </QueryParams>\r\n                            </BO>\r\n                           </BOM>\r\n                        </dis:RemoveObject></env:Body></env:Envelopes>";
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
					string arg = Function.ToString(xElement3.Value);
					Logging.Write("WATCH", $"Remove a Solution - {arg}");
					return string.Empty;
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}
}
