using System;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Financials;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class JournalEntriesRepository : Repository<JournalEntriesImpl>, IJournalEntries, IRepository<JournalEntriesImpl>, IDisposable
{
	private readonly IUnitOfWork _unitOfWork;

	public JournalEntriesRepository(Apzon.Services.IUnitOfWork serUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}

	public virtual HttpResult SaveData(JournalEntriesImpl doc, string sessionId)
	{
		try
		{
			if (!Exist(doc, sessionId))
			{
				return AddNew(doc, sessionId);
			}
			return UpdateData(doc, sessionId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual HttpResult UpdateData(JournalEntriesImpl doc, string sessionId)
	{
		try
		{
			string text = Mapping(doc, EnumCommandType.Update);
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text + "</dis:UpdateObject></env:Body></env:Envelopes>";
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
					Logging.Write("WATCH", $"update Journal Entry - DocEntry - {num}");
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

	public virtual string Mapping(JournalEntriesImpl model, EnumCommandType commandType)
	{
		string text = string.Empty;
		try
		{
			text += "<BOM>\r\n                        <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                          <AdmInfo>\r\n                            <Object>oJournalEntries</Object>\r\n                          </AdmInfo>";
			if (commandType == EnumCommandType.Update)
			{
				text = text + "<QueryParams><JdtNum>" + Function.ParseInt(model.TransId) + "</JdtNum></QueryParams>";
			}
			text += "<JournalEntries>\r\n                         <row>";
			if (model.RefDate > new DateTime(1900, 1, 1))
			{
				text = text + "<ReferenceDate>" + Convert.ToDateTime(model.RefDate).ToString("yyyyMMdd") + "</ReferenceDate>";
			}
			if (!string.IsNullOrEmpty(model.Memo))
			{
				text = text + "<Memo>" + Function.RawText(model.Memo) + "</Memo>";
			}
			if (!string.IsNullOrEmpty(model.Ref1))
			{
				text = text + "<Reference>" + Function.RawText(model.Ref1) + "</Reference>";
			}
			if (!string.IsNullOrEmpty(model.Ref2))
			{
				text = text + "<Reference2>" + Function.RawText(model.Ref2) + "</Reference2>";
			}
			if (!string.IsNullOrEmpty(model.TransCode))
			{
				text = text + "<TransactionCode>" + Function.RawText(model.TransCode) + "</TransactionCode>";
			}
			if (!string.IsNullOrEmpty(model.Project))
			{
				text = text + "<ProjectCode>" + Function.RawText(model.Project) + "</ProjectCode>";
			}
			if (model.TaxDate > new DateTime(1900, 1, 1))
			{
				text = text + "<TaxDate>" + Convert.ToDateTime(model.TaxDate).ToString("yyyyMMdd") + "</TaxDate>";
			}
			if (!string.IsNullOrEmpty(model.Indicator))
			{
				text = text + "<Indicator>" + Function.RawText(model.Indicator) + "</Indicator>";
			}
			text = text + "<UseAutoStorno>" + model.AutoStorno.ToString() + "</UseAutoStorno>";
			if (model.StornoDate > new DateTime(1900, 1, 1))
			{
				text = text + "<StornoDate>" + Convert.ToDateTime(model.StornoDate).ToString("yyyyMMdd") + "</StornoDate>";
			}
			if (model.VatDate > new DateTime(1900, 1, 1))
			{
				text = text + "<VatDate>" + Convert.ToDateTime(model.VatDate).ToString("yyyyMMdd") + "</VatDate>";
			}
			if (!Function.ParseInt(model.Series).Equals(0))
			{
				text = text + "<Series>" + Function.ParseInt(model.Series) + "</Series>";
			}
			if (model.DueDate > new DateTime(1900, 1, 1))
			{
				text = text + "<DueDate>" + Convert.ToDateTime(model.DueDate).ToString("yyyyMMdd") + "</DueDate>";
			}
			if (!Function.ParseInt(model.Location).Equals(0))
			{
				text = text + "<LocationCode>" + Function.ParseInt(model.Location) + "</LocationCode>";
			}
			if (!string.IsNullOrEmpty(model.Ref3))
			{
				text = text + "<Reference3>" + Function.RawText(model.Ref3) + "</Reference3>";
			}
			if (!string.IsNullOrEmpty(model.DocType))
			{
				text = text + "<DocumentType>" + Function.RawText(model.DocType) + "</DocumentType>";
			}
			text += DataService.MappingUdf(model.UserFields);
			text += "</row></JournalEntries>";
			if (model.Lines != null && model.Lines.Count > 0)
			{
				text += "<JournalEntries_Lines>";
				for (int i = 0; i < model.Lines.Count; i++)
				{
					text += "<row>";
					JournalEntries_LinesImpl journalEntries_LinesImpl = model.Lines[i];
					if (!Function.ParseInt(journalEntries_LinesImpl.Line_ID).Equals(0))
					{
						text = text + "<Line_ID>" + Function.ParseInt(journalEntries_LinesImpl.Line_ID) + "</Line_ID>";
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.Account))
					{
						text = text + "<AccountCode>" + Function.RawText(journalEntries_LinesImpl.Account) + "</AccountCode>";
					}
					if (!Function.ParseDouble(journalEntries_LinesImpl.Debit).Equals(0.0))
					{
						text = text + "<Debit>" + Function.ParseDouble(journalEntries_LinesImpl.Debit) + "</Debit>";
					}
					if (!Function.ParseDouble(journalEntries_LinesImpl.Credit).Equals(0.0))
					{
						text = text + "<Credit>" + Function.ParseDouble(journalEntries_LinesImpl.Credit) + "</Credit>";
					}
					if (!Function.ParseDouble(journalEntries_LinesImpl.FCDebit).Equals(0.0))
					{
						text = text + "<FCDebit>" + Function.ParseDouble(journalEntries_LinesImpl.FCDebit) + "</FCDebit>";
					}
					if (!Function.ParseDouble(journalEntries_LinesImpl.FCCredit).Equals(0.0))
					{
						text = text + "<FCCredit>" + Function.ParseDouble(journalEntries_LinesImpl.FCCredit) + "</FCCredit>";
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.FCCurrency))
					{
						text = text + "<FCCurrency>" + Function.RawText(journalEntries_LinesImpl.FCCurrency) + "</FCCurrency>";
					}
					if (journalEntries_LinesImpl.DueDate > new DateTime(1900, 1, 1))
					{
						text = text + "<DueDate>" + Convert.ToDateTime(journalEntries_LinesImpl.DueDate).ToString("yyyyMMdd") + "</DueDate>";
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.ShortName))
					{
						text = text + "<ShortName>" + Function.RawText(journalEntries_LinesImpl.ShortName) + "</ShortName>";
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.ContraAct))
					{
						text = text + "<ContraAccount>" + Function.RawText(journalEntries_LinesImpl.ContraAct) + "</ContraAccount>";
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.LineMemo))
					{
						text = text + "<LineMemo>" + Function.RawText(journalEntries_LinesImpl.LineMemo) + "</LineMemo>";
					}
					if (journalEntries_LinesImpl.RefDate > new DateTime(1900, 1, 1))
					{
						text = text + "<ReferenceDate1>" + Convert.ToDateTime(journalEntries_LinesImpl.RefDate).ToString("yyyyMMdd") + "</ReferenceDate1>";
					}
					if (journalEntries_LinesImpl.Ref2Date > new DateTime(1900, 1, 1))
					{
						text = text + "<ReferenceDate2>" + Convert.ToDateTime(journalEntries_LinesImpl.Ref2Date).ToString("yyyyMMdd") + "</ReferenceDate2>";
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.Ref1))
					{
						text = text + "<Reference1>" + Function.RawText(journalEntries_LinesImpl.Ref1) + "</Reference1>";
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.Ref2))
					{
						text = text + "<Reference2>" + Function.RawText(journalEntries_LinesImpl.Ref2) + "</Reference2>";
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.Project))
					{
						text = text + "<ProjectCode>" + Function.RawText(journalEntries_LinesImpl.Project) + "</ProjectCode>";
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.ProfitCode))
					{
						text = text + "<CostingCode>" + Function.RawText(journalEntries_LinesImpl.ProfitCode) + "</CostingCode>";
					}
					if (journalEntries_LinesImpl.TaxDate > new DateTime(1900, 1, 1))
					{
						text = text + "<TaxDate>" + Convert.ToDateTime(journalEntries_LinesImpl.TaxDate).ToString("yyyyMMdd") + "</TaxDate>";
					}
					if (!Function.ParseDouble(journalEntries_LinesImpl.BaseSum).Equals(0.0))
					{
						text = text + "<BaseSum>" + Function.ParseDouble(journalEntries_LinesImpl.BaseSum) + "</BaseSum>";
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.VatGroup))
					{
						text = text + "<TaxGroup>" + Function.RawText(journalEntries_LinesImpl.VatGroup) + "</TaxGroup>";
					}
					if (!Function.ParseDouble(journalEntries_LinesImpl.SYSDeb).Equals(0.0))
					{
						text = text + "<DebitSys>" + Function.ParseDouble(journalEntries_LinesImpl.SYSDeb) + "</DebitSys>";
					}
					if (!Function.ParseDouble(journalEntries_LinesImpl.SYSCred).Equals(0.0))
					{
						text = text + "<CreditSys>" + Function.ParseDouble(journalEntries_LinesImpl.SYSCred) + "</CreditSys>";
					}
					if (journalEntries_LinesImpl.VatDate > new DateTime(1900, 1, 1))
					{
						text = text + "<VatDate>" + Convert.ToDateTime(journalEntries_LinesImpl.VatDate).ToString("yyyyMMdd") + "</VatDate>";
					}
					if (!Function.ParseDouble(journalEntries_LinesImpl.SYSBaseSum).Equals(0.0))
					{
						text = text + "<SystemBaseAmount>" + Function.ParseDouble(journalEntries_LinesImpl.SYSBaseSum) + "</SystemBaseAmount>";
					}
					if (!Function.ParseDouble(journalEntries_LinesImpl.VatAmount).Equals(0.0))
					{
						text = text + "<VatAmount>" + Function.ParseDouble(journalEntries_LinesImpl.VatAmount) + "</VatAmount>";
					}
					if (!Function.ParseDouble(journalEntries_LinesImpl.SYSVatSum).Equals(0.0))
					{
						text = text + "<SystemVatAmount>" + Function.ParseDouble(journalEntries_LinesImpl.SYSVatSum) + "</SystemVatAmount>";
					}
					if (!Function.ParseDouble(journalEntries_LinesImpl.GrossValue).Equals(0.0))
					{
						text = text + "<GrossValue>" + Function.ParseDouble(journalEntries_LinesImpl.GrossValue) + "</GrossValue>";
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.Ref3Line))
					{
						text = text + "<AdditionalReference>" + Function.RawText(journalEntries_LinesImpl.Ref3Line) + "</AdditionalReference>";
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.OcrCode2))
					{
						text = text + "<CostingCode2>" + Function.RawText(journalEntries_LinesImpl.OcrCode2) + "</CostingCode2>";
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.OcrCode3))
					{
						text = text + "<CostingCode3>" + Function.RawText(journalEntries_LinesImpl.OcrCode3) + "</CostingCode3>";
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.OcrCode4))
					{
						text = text + "<CostingCode4>" + Function.RawText(journalEntries_LinesImpl.OcrCode4) + "</CostingCode4>";
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.OcrCode5))
					{
						text = text + "<CostingCode5>" + Function.RawText(journalEntries_LinesImpl.OcrCode5) + "</CostingCode5>";
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.TaxCode))
					{
						text = text + "<TaxCode>" + Function.RawText(journalEntries_LinesImpl.TaxCode) + "</TaxCode>";
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.ControlAccount))
					{
						text = text + "<ControlAccount>" + Function.RawText(journalEntries_LinesImpl.ControlAccount) + "</ControlAccount>";
					}
					if (!Function.ParseInt(journalEntries_LinesImpl.BPLId).Equals(0))
					{
						text = text + "<BPLID>" + Function.ParseInt(journalEntries_LinesImpl.BPLId) + "</BPLID>";
					}
					text += "</row>";
				}
				text += "</JournalEntries_Lines>";
			}
			text += "</BO></BOM>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	public virtual HttpResult AddNew(JournalEntriesImpl doc, string sessionId)
	{
		try
		{
			string text = Mapping(doc, EnumCommandType.Add);
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text + "</dis:AddObject></env:Body></env:Envelopes>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("WATCH", $"Add new journal entry - failed - {xElement2.Value}");
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Add new journal entry - DocEntry - {num}");
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

	public virtual bool Exist(JournalEntriesImpl doc, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n\r\n                            <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n\r\n                             <env:Header>\r\n\r\n                              <SessionID>" + sessionId + "</SessionID> \r\n\r\n                             </env:Header>\r\n\r\n                             <env:Body>\r\n\r\n                              <dis:GetByKey xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n\r\n                               <Object>oJournalEntries</Object> \r\n\r\n                               <DocEntry>" + doc.TransId + "</DocEntry> \r\n\r\n                              </dis:GetByKey>\r\n\r\n                             </env:Body>\r\n\r\n                            </env:Envelope>";
			XmlElement xmlElement = DataService.GetByKey(sessionId, xmlObject).Content as XmlElement;
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				return false;
			}
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("DocEntry"))
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

	public virtual HttpResult CreateJournalVoucher(JournalEntriesImpl lstJE, string sessionId)
	{
		throw new NotImplementedException();
	}

	public virtual HttpResult CancelJE(int transId, string sessionId)
	{
		throw new NotImplementedException();
	}
}
