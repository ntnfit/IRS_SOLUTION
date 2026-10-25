using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Payments;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class PaymentsRepository : Repository<IncomingPaymentsImpl>, IPayments, IRepository<IncomingPaymentsImpl>
{
	public PaymentsRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public HttpResult Save(ApzObjectTypes objType, IncomingPaymentsImpl doc, string sessionId, bool isDraft)
	{
		try
		{
			if (doc.DocEntry <= 0)
			{
				return AddNew(objType, doc, sessionId, isDraft);
			}
			return UpdateData(objType, doc, sessionId, isDraft);
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

	protected virtual HttpResult UpdateData(ApzObjectTypes objType, IncomingPaymentsImpl doc, string sessionId, bool isDraft)
	{
		try
		{
			string text = Mapping(objType, doc, EnumCommandType.Add, isDraft);
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:UpdateData xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text + "</dis:UpdateData></env:Body></env:Envelopes>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
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
					Logging.Write("WATCH", $"update payment {num}");
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

	protected virtual HttpResult AddNew(ApzObjectTypes objType, IncomingPaymentsImpl doc, string sessionId, bool isDraft)
	{
		try
		{
			string text = Mapping(objType, doc, EnumCommandType.Add, isDraft);
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text + "</dis:AddObject></env:Body></env:Envelopes>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
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
					Logging.Write("WATCH", $"Add new payment {num}");
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

	protected virtual string Mapping(ApzObjectTypes objType, IncomingPaymentsImpl doc, EnumCommandType enumCommandType, bool isDraft)
	{
		try
		{
			string mainCurrency = ServiceUnitOfWork.GeneralSetting.GetMainCurrency();
			string arg;
			if (isDraft)
			{
				arg = "PaymentDrafts";
			}
			else
			{
				arg = ((objType == ApzObjectTypes.oIncomingPayment) ? "oIncomingPayments" : "VendorPayments");
			}
			string text = $"<BOM><BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                  <AdmInfo>\r\n                                    <Object>{arg}</Object>\r\n                                  </AdmInfo>";
			if (enumCommandType == EnumCommandType.Update)
			{
				text = text + "<QueryParams><DocEntry>" + Function.ParseInt(doc.DocEntry) + "</DocEntry></QueryParams>";
			}
			text += " <Payments>\r\n                             <row>";
			string docType = doc.DocType;
			if (docType == "A")
			{
				text += "<DocType>rAccount</DocType>";
			}
			else
			{
				text = ((!(docType == "S")) ? (text + "<DocType>rCustomer</DocType>") : (text + "<DocType>rSupplier</DocType>"));
			}
			switch (objType)
			{
			case ApzObjectTypes.oIncomingPayment:
				text += "<DocObjectCode>bopot_IncomingPayments</DocObjectCode>";
				break;
			case ApzObjectTypes.oOutgoingPayment:
				text += "<DocObjectCode>bopot_OutgoingPayments</DocObjectCode>";
				break;
			}
			text = text + "<CardCode>" + Function.RawText(doc.CardCode) + "</CardCode>";
			text = text + "<CardName>" + Function.RawText(doc.CardName) + "</CardName>";
			if (!string.IsNullOrEmpty(doc.PayToCode))
			{
				text = text + "<PayToCode>" + Function.RawText(doc.PayToCode) + "</PayToCode>";
			}
			text = text + "<Address>" + Function.RawText(doc.Address) + "</Address>";
			if (doc.ContactPersonCode > 0)
			{
				text = text + "<ContactPersonCode>" + Function.ParseInt(doc.ContactPersonCode) + "</ContactPersonCode>";
			}
			if (string.IsNullOrEmpty(doc.DocCurr))
			{
				doc.DocCurr = mainCurrency;
				doc.DocRate = 1m;
			}
			text = text + "<DocCurrency>" + Function.RawText(doc.DocCurr) + "</DocCurrency>";
			if (!string.IsNullOrEmpty(doc.DiffCurr))
			{
				if (doc.DiffCurr == "Y")
				{
					text += "<LocalCurrency>tYES</LocalCurrency>";
				}
				else if (doc.DiffCurr == "N")
				{
					text += "<LocalCurrency>tNO</LocalCurrency>";
				}
			}
			if (doc.DocRate > 0m)
			{
				text = text + "<DocRate>" + Function.ParseDouble(doc.DocRate) + "</DocRate>";
			}
			if (!string.IsNullOrEmpty(doc.PrjCode))
			{
				text = text + "<ProjectCode>" + Function.RawText(doc.PrjCode) + "</ProjectCode>";
			}
			if (doc.DocNum > 0)
			{
				text = text + "<DocNum>" + Function.ParseInt(doc.DocNum) + "</DocNum>";
			}
			if (doc.DocDate > new DateTime(1900, 1, 1))
			{
				text = text + "<DocDate>" + Convert.ToDateTime(doc.DocDate).ToString("yyyyMMdd") + "</DocDate>";
			}
			if (doc.DueDate > new DateTime(1900, 1, 1))
			{
				text = text + "<DueDate>" + Convert.ToDateTime(doc.DueDate).ToString("yyyyMMdd") + "</DueDate>";
			}
			if (doc.TaxDate > new DateTime(1900, 1, 1))
			{
				text = text + "<TaxDate>" + Convert.ToDateTime(doc.TaxDate).ToString("yyyyMMdd") + "</TaxDate>";
			}
			if (!string.IsNullOrEmpty(doc.CounterReference))
			{
				text = text + "<CounterReference>" + Function.RawText(doc.DocEntry) + "</CounterReference>";
			}
			text = text + "<Remarks>" + Function.RawText(doc.Comments) + "</Remarks>";
			text = text + "<JournalRemarks>" + Function.RawText(doc.JrnlMemo) + "</JournalRemarks>";
			if (!string.IsNullOrEmpty(doc.BpAct))
			{
				text = text + "<ControlAccount>" + Function.RawText(doc.BpAct) + "</ControlAccount>";
			}
			text = ((!(doc.IsPaytoBnk == "Y")) ? (text + "<IsPayToBank>tNO</IsPayToBank>") : (text + "<IsPayToBank>tYES</IsPayToBank>"));
			if (!string.IsNullOrEmpty(doc.CashAcct))
			{
				text = text + "<CashAccount>" + Function.RawText(doc.CashAcct) + "</CashAccount>";
			}
			if (doc.DocCurr.Equals(mainCurrency))
			{
				if (doc.CashSum > 0m)
				{
					text = text + "<CashSum>" + Function.ParseDouble(doc.CashSum) + "</CashSum>";
				}
			}
			else if (doc.CashSumFC > 0m)
			{
				text = text + "<CashSumFC>" + Function.ParseDouble(doc.CashSumFC) + "</CashSumFC>";
			}
			if (!string.IsNullOrEmpty(doc.CheckAcct))
			{
				text = text + "<CheckAccount>" + Function.RawText(doc.CheckAcct) + "</CheckAccount>";
			}
			if (!string.IsNullOrEmpty(doc.TrsfrAcct))
			{
				text = text + "<TransferAccount>" + Function.RawText(doc.TrsfrAcct) + "</TransferAccount>";
			}
			if (doc.DocCurr.Equals(mainCurrency))
			{
				if (doc.TrsfrSum > 0m)
				{
					text = text + "<TransferSum>" + Function.ParseDouble(doc.TrsfrSum) + "</TransferSum>";
				}
			}
			else if (doc.TrsfrSumFC > 0m)
			{
				text = text + "<TransferSum>" + Function.ParseDouble(doc.TrsfrSumFC) + "</TransferSum>";
			}
			if (doc.TrsfrDate > new DateTime(1900, 1, 1))
			{
				text = text + "<TransferDate>" + Convert.ToDateTime(doc.TrsfrDate).ToString("yyyyMMdd") + "</TransferDate>";
			}
			if (!string.IsNullOrEmpty(doc.TrsfrRef))
			{
				text = text + "<TransferReference>" + Function.RawText(doc.TrsfrRef) + "</TransferReference>";
			}
			if (doc.BcgSum != 0.0)
			{
				text = text + "<BankChargeAmount>" + Function.ParseDecimal(doc.BcgSum) + "</BankChargeAmount>";
			}
			if (doc.BcgSumFC != 0.0)
			{
				text += (text = text + "<BankChargeAmountInFC>" + Function.ParseDecimal(doc.BcgSumFC) + "</BankChargeAmountInFC>");
			}
			if (!string.IsNullOrEmpty(doc.Reference1))
			{
				text = text + "<Reference1>" + Function.RawText(doc.Reference1) + "</Reference1>";
			}
			if (!string.IsNullOrEmpty(doc.Reference2))
			{
				text = text + "<Reference2>" + Function.RawText(doc.Reference2) + "</Reference2>";
			}
			if (!string.IsNullOrEmpty(doc.CounterReference))
			{
				text = text + "<CounterReference>" + Function.RawText(doc.CounterReference) + "</CounterReference>";
			}
			if (doc.Series != 0)
			{
				text = text + "<Series>" + Function.ParseInt(doc.Series) + "</Series>";
			}
			if (doc.BPLId != 0)
			{
				text = text + "<BPLID>" + Function.ParseInt(doc.BPLId) + "</BPLID>";
			}
			text += "</row>\r\n                        </Payments>";
			if ((doc.DocType.Equals("C") || doc.DocType.Equals("S")) && doc.Invoices != null && doc.Invoices.Count > 0)
			{
				text += "<Payments_Invoices>";
				for (int i = 0; i < doc.Invoices.Count; i++)
				{
					Payments_InvoicesImpl payments_InvoicesImpl = doc.Invoices[i];
					if (payments_InvoicesImpl.DocEntry > 0)
					{
						text += "<row>";
						if (payments_InvoicesImpl.DocEntry > 0)
						{
							text = text + "<DocEntry>" + Function.ParseInt(payments_InvoicesImpl.DocEntry) + "</DocEntry>";
						}
						if (payments_InvoicesImpl.DocLine >= 0)
						{
							text = text + "<DocLine>" + payments_InvoicesImpl.DocLine + "</DocLine>";
						}
						if (payments_InvoicesImpl.PaidSum != 0.0)
						{
							text = text + "<PaidSum>" + Function.ParseDouble(payments_InvoicesImpl.PaidSum) + "</PaidSum>";
						}
						if (payments_InvoicesImpl.SumApplied != 0.0)
						{
							text = text + "<SumApplied>" + Function.ParseDouble(payments_InvoicesImpl.SumApplied) + "</SumApplied>";
						}
						if (payments_InvoicesImpl.AppliedFC != 0.0)
						{
							text = text + "<AppliedFC>" + Function.ParseDouble(payments_InvoicesImpl.AppliedFC) + "</AppliedFC>";
						}
						if (!string.IsNullOrEmpty(payments_InvoicesImpl.InvType))
						{
							text = text + "<InvoiceType>" + Function.RawText(payments_InvoicesImpl.InvType) + "</InvoiceType>";
						}
						text += "</row>";
					}
				}
				text += "</Payments_Invoices>";
			}
			if (doc.DocType.Equals("A") && doc._listPurpose != null && doc._listPurpose.Count > 0)
			{
				text += "<AccountPayments>";
				for (int j = 0; j < doc._listPurpose.Count; j++)
				{
					Payments_AccountsImpl payments_AccountsImpl = doc._listPurpose[j];
					text += "<row>";
					if (!string.IsNullOrEmpty(payments_AccountsImpl.AcctCode))
					{
						text = text + "<AccountCode>" + Function.RawText(payments_AccountsImpl.AcctCode) + "</AccountCode>";
					}
					if (!string.IsNullOrEmpty(payments_AccountsImpl.AcctName))
					{
						text = text + "<AccountName>" + Function.RawText(payments_AccountsImpl.AcctName) + "</AccountName>";
					}
					if (!string.IsNullOrEmpty(payments_AccountsImpl.Descrip))
					{
						text = text + "<Decription>" + Function.RawText(payments_AccountsImpl.Descrip) + "</Decription>";
					}
					if (payments_AccountsImpl.GrossAmnt != 0.0)
					{
						text = text + "<GrossAmount>" + Function.ParseDecimal(payments_AccountsImpl.GrossAmnt) + "</GrossAmount>";
					}
					if (!string.IsNullOrEmpty(payments_AccountsImpl.OcrCode))
					{
						text = text + "<ProfitCenter>" + Function.RawText(payments_AccountsImpl.OcrCode) + "</ProfitCenter>";
					}
					if (!string.IsNullOrEmpty(payments_AccountsImpl.OcrCode2))
					{
						text = text + "<ProfitCenter2>" + Function.RawText(payments_AccountsImpl.OcrCode2) + "</ProfitCenter2>";
					}
					if (!string.IsNullOrEmpty(payments_AccountsImpl.OcrCode3))
					{
						text = text + "<ProfitCenter3>" + Function.RawText(payments_AccountsImpl.OcrCode3) + "</ProfitCenter3>";
					}
					if (!string.IsNullOrEmpty(payments_AccountsImpl.OcrCode4))
					{
						text = text + "<ProfitCenter4>" + Function.RawText(payments_AccountsImpl.OcrCode4) + "</ProfitCenter4>";
					}
					if (!string.IsNullOrEmpty(payments_AccountsImpl.OcrCode5))
					{
						text = text + "<ProfitCenter5>" + Function.RawText(payments_AccountsImpl.OcrCode5) + "</ProfitCenter5>";
					}
					if (!string.IsNullOrEmpty(payments_AccountsImpl.Project))
					{
						text = text + "<ProjectCode>" + Function.RawText(payments_AccountsImpl.Project) + "</ProjectCode>";
					}
					if (payments_AccountsImpl.SumApplied != 0.0)
					{
						text = text + "<SumPaid>" + Function.ParseDouble(payments_AccountsImpl.SumApplied) + "</SumPaid>";
					}
					if (payments_AccountsImpl.VatAmnt != 0m)
					{
						text = text + "<VatAmount>" + Function.ParseDouble(payments_AccountsImpl.VatAmnt) + "</VatAmount>";
					}
					if (!string.IsNullOrEmpty(payments_AccountsImpl.VatGroup))
					{
						text = text + "<VatGroup>" + Function.RawText(payments_AccountsImpl.VatGroup) + "</VatGroup>";
					}
					text += "</row>";
				}
				text += "</AccountPayments>";
			}
			return text + "</BO></BOM>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	private bool Exist(IncomingPaymentsImpl doc, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                            <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                             <env:Header>\r\n                              <SessionID>" + sessionId + "</SessionID> \r\n                             </env:Header>\r\n                             <env:Body>\r\n                              <dis:GetByKey xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                               <Object>oPayments</Object> \r\n                               <DocEntry>" + doc.DocEntry + "</DocEntry> \r\n                              </dis:GetByKey>\r\n                             </env:Body>\r\n                            </env:Envelope>";
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

	public HttpResult SaveListDoc(ApzObjectTypes objType, List<IncomingPaymentsImpl> docs, string sessionId)
	{
		try
		{
			string text = "";
			foreach (IncomingPaymentsImpl doc in docs)
			{
				text = text + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + Mapping(objType, doc, EnumCommandType.Add, isDraft: false) + "</dis:AddObject>";
			}
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "\r\n                        </env:Body></env:Envelopes>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", xElement2.Value);
					return new HttpResult
					{
						Content = 0,
						Message = xElement2.Value
					};
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				List<XElement> list = (from t in xElement.Descendants()
					where t.Name.LocalName.Equals("RetKey")
					select t).ToList();
				if (list.Count > 0)
				{
					List<string> list2 = new List<string>();
					list2.AddRange(list.Select((XElement t) => Function.ToString(t.Value)));
					if (list2.Count > 0)
					{
						Logging.Write("WATCH", "Add incomming payment " + string.Join(",", list2.ToArray()));
						return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list2.ToArray()));
					}
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

	public virtual HttpResult SaveDraftToDocument(int docEntry, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>PaymentDraftsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <SaveDraftToDocument xmlns=\"PaymentDraftsService\"><Payment><DocEntry>" + docEntry + "</DocEntry></Payment></SaveDraftToDocument>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
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
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					return new HttpResult(Enums.MessageCode.Success, "", Function.ParseInt(xElement3.Value));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Success);
	}
}
