using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Payments;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class PaymentsDraftsRepository : Repository<IncomingPaymentsImpl>, IPaymentsDrafts, IRepository<IncomingPaymentsImpl>
{
	public PaymentsDraftsRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public HttpResult Save(IncomingPaymentsImpl doc, string sessionId)
	{
		try
		{
			if (!Exist(doc))
			{
				return AddNew(doc, sessionId);
			}
			return UpdateData(doc, sessionId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult
			{
				Content = null,
				Message = ex.Message
			};
		}
	}

	protected virtual HttpResult UpdateData(IncomingPaymentsImpl doc, string sessionId)
	{
		try
		{
			string xmlObject = Mapping(doc, EnumCommandType.Update);
			XmlElement xmlElement = DataService.UpdateObject(ApzObjectTypes.oPaymentsDrafts, sessionId, xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return new HttpResult
					{
						Content = 0,
						Message = xElement2.Value
					};
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"update payment draft {num}");
					return new HttpResult
					{
						Content = num,
						Message = ""
					};
				}
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult
			{
				Content = null,
				Message = ex.Message
			};
		}
	}

	protected virtual HttpResult AddNew(IncomingPaymentsImpl doc, string sessionId)
	{
		try
		{
			string xmlObject = Mapping(doc, EnumCommandType.Add);
			XmlElement xmlElement = DataService.AddObject(ApzObjectTypes.oPaymentsDrafts, sessionId, xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return new HttpResult
					{
						Content = 0,
						Message = xElement2.Value
					};
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Add new payment draft {num}");
					return new HttpResult
					{
						Content = num,
						Message = ""
					};
				}
			}
			return new HttpResult
			{
				Content = 0,
				Message = ""
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult
			{
				Content = null,
				Message = ex.Message
			};
		}
	}

	private string Mapping(IncomingPaymentsImpl model, EnumCommandType enumCommandType)
	{
		try
		{
			string text = "<BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                  <AdmInfo>\r\n                                    <Object>oPaymentsDrafts</Object>\r\n                                  </AdmInfo>\r\n                                  <QueryParams>\r\n                                    <DocEntry />\r\n                                  </QueryParams>";
			if (enumCommandType == EnumCommandType.Update)
			{
				text = text + "<QueryParams><DocEntry>" + Function.ParseInt(model.DocEntry) + "</DocEntry></QueryParams>";
			}
			string docType = model.DocType;
			if (docType == "A")
			{
				text += "<DocType>rAccount</DocType>";
			}
			else
			{
				text = ((!(docType == "S")) ? (text + "<DocType>rCustomer</DocType>") : (text + "<DocType>rSupplier</DocType>"));
			}
			text = text + "<CardCode>" + Function.RawText(model.CardCode) + "</CardCode>";
			text = text + "<CardName>" + Function.RawText(model.CardName) + "</CardName>";
			if (!string.IsNullOrEmpty(model.PayToCode))
			{
				text = text + "<PayToCode>" + Function.RawText(model.PayToCode) + "</PayToCode>";
			}
			text = text + "<Address>" + Function.RawText(model.Address) + "</Address>";
			if (model.ContactPersonCode > 0)
			{
				text = text + "<ContactPersonCode>" + Function.ParseInt(model.ContactPersonCode) + "</ContactPersonCode>";
			}
			if (!string.IsNullOrEmpty(model.DocCurrency))
			{
				text = text + "<DocCurrency>" + Function.RawText(model.DocCurrency) + "</DocCurrency>";
			}
			if (model.DocRate > 0m)
			{
				text = text + "<DocRate>" + Function.ParseDouble(model.DocRate) + "</DocRate>";
			}
			if (!string.IsNullOrEmpty(model.PrjCode))
			{
				text = text + "<ProjectCode>" + Function.RawText(model.PrjCode) + "</ProjectCode>";
			}
			if (model.DocNum > 0)
			{
				text = text + "<DocNum>" + Function.ParseInt(model.DocNum) + "</DocNum>";
			}
			if (model.DocDate > new DateTime(1900, 1, 1))
			{
				text = text + "<DocDate>" + Convert.ToDateTime(model.DocDate).ToString("yyyyMMdd") + "</DocDate>";
			}
			if (model.DueDate > new DateTime(1900, 1, 1))
			{
				text = text + "<DueDate>" + Convert.ToDateTime(model.DueDate).ToString("yyyyMMdd") + "</DueDate>";
			}
			if (model.TaxDate > new DateTime(1900, 1, 1))
			{
				text = text + "<TaxDate>" + Convert.ToDateTime(model.TaxDate).ToString("yyyyMMdd") + "</TaxDate>";
			}
			if (!string.IsNullOrEmpty(model.CounterReference))
			{
				text = text + "<CounterReference>" + Function.RawText(model.DocEntry) + "</CounterReference>";
			}
			text = text + "<Remarks>" + Function.RawText(model.Comments) + "</Remarks>";
			text = text + "<JournalRemarks>" + Function.RawText(model.JrnlMemo) + "</JournalRemarks>";
			if (!string.IsNullOrEmpty(model.ControlAccount))
			{
				text = text + "<ControlAccount>" + Function.RawText(model.ControlAccount) + "</ControlAccount>";
			}
			if (model.IsPaytoBnk == "Y")
			{
				text += "<IsPayToBank>tYES</IsPayToBank>";
			}
			else
			{
				text += "<IsPayToBank>tNO</IsPayToBank>";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	private bool Exist(IncomingPaymentsImpl doc)
	{
		try
		{
			if (Function.ParseInt(ExecuteScarlar("select top 1 1 from OPDF where \"DocEntry\" = @docEntry;", CommandType.Text, new SqlParameter("@docEntry", doc.DocEntry))) < 1)
			{
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}
}
