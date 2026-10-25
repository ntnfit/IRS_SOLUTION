using System;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Payments;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaPaymentsRepository : PaymentsRepository
{
	public HanaPaymentsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	protected override HttpResult UpdateData(ApzObjectTypes objType, IncomingPaymentsImpl doc, string sessionId, bool isDraft)
	{
		try
		{
			string xmlObject = Mapping(objType, doc, EnumCommandType.Update, isDraft);
			if (isDraft)
			{
				objType = ApzObjectTypes.oPaymentsDrafts;
			}
			return DataService.UpdateObject(objType, sessionId, xmlObject, doc.DocEntry);
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

	protected override HttpResult AddNew(ApzObjectTypes objType, IncomingPaymentsImpl doc, string sessionId, bool isDraft)
	{
		try
		{
			string xmlObject = Mapping(objType, doc, EnumCommandType.Add, isDraft);
			if (isDraft)
			{
				objType = ApzObjectTypes.oPaymentsDrafts;
			}
			return DataService.AddObject(objType, sessionId, xmlObject);
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

	protected override string Mapping(ApzObjectTypes objType, IncomingPaymentsImpl doc, EnumCommandType enumCommandType, bool isDraft)
	{
		try
		{
			string mainCurrency = ServiceUnitOfWork.GeneralSetting.GetMainCurrency();
			string text = "{";
			string docType = doc.DocType;
			if (docType == "A")
			{
				text += Function.GetJsonString("DocType", "rAccount");
			}
			else
			{
				text = ((!(docType == "S")) ? (text + Function.GetJsonString("DocType", "rCustomer")) : (text + Function.GetJsonString("DocType", "rSupplier")));
			}
			switch (objType)
			{
			case ApzObjectTypes.oIncomingPayment:
				text += Function.GetJsonString("DocObjectCode", "bopot_IncomingPayments");
				break;
			case ApzObjectTypes.oOutgoingPayment:
				text += Function.GetJsonString("DocObjectCode", "bopot_OutgoingPayments");
				break;
			}
			text += Function.GetJsonString("CardCode", Function.ToString(doc.CardCode));
			text += Function.GetJsonString("CardName", Function.ToString(doc.CardName));
			if (!string.IsNullOrEmpty(doc.PayToCode))
			{
				text += Function.GetJsonString("PayToCode", Function.ToString(doc.PayToCode));
			}
			text += Function.GetJsonString("Address", Function.ToString(doc.Address));
			if (doc.ContactPersonCode > 0)
			{
				text += Function.GetJsonString("ContactPersonCode", Function.ParseInt(doc.ContactPersonCode));
			}
			if (string.IsNullOrEmpty(doc.DocCurr))
			{
				doc.DocCurr = mainCurrency;
				doc.DocRate = 1m;
			}
			text += Function.GetJsonString("DocCurrency", Function.ToString(doc.DocCurr));
			if (!string.IsNullOrEmpty(doc.DiffCurr))
			{
				if (doc.DiffCurr == "Y")
				{
					text += Function.GetJsonString("LocalCurrency", "tYES");
				}
				else if (doc.DiffCurr == "N")
				{
					text += Function.GetJsonString("LocalCurrency", "tNO");
				}
			}
			if (doc.DocRate > 0m)
			{
				text += Function.GetJsonString("DocRate", Function.ParseDouble(doc.DocRate));
			}
			if (!string.IsNullOrEmpty(doc.PrjCode))
			{
				text += Function.GetJsonString("ProjectCode", Function.ToString(doc.PrjCode));
			}
			if (doc.DocNum > 0)
			{
				text += Function.GetJsonString("DocNum", Function.ParseInt(doc.DocNum));
			}
			if (doc.DocDate > new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("DocDate", Function.ParseDateTimes(doc.DocDate).ToString("yyyyMMdd"));
			}
			if (doc.DueDate > new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("DueDate", Function.ParseDateTimes(doc.DueDate).ToString("yyyyMMdd"));
			}
			if (doc.TaxDate > new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("TaxDate", Function.ParseDateTimes(doc.TaxDate).ToString("yyyyMMdd"));
			}
			if (!string.IsNullOrEmpty(doc.CounterReference))
			{
				text += Function.GetJsonString("CounterReference", Function.ToString(doc.DocEntry));
			}
			text += Function.GetJsonString("Remarks", Function.ToString(doc.Comments));
			text += Function.GetJsonString("JournalRemarks", Function.ToString(doc.JrnlMemo));
			if (!string.IsNullOrEmpty(doc.BpAct))
			{
				text += Function.GetJsonString("ControlAccount", Function.ToString(doc.BpAct));
			}
			text = ((!(doc.IsPaytoBnk == "Y")) ? (text + Function.GetJsonString("IsPayToBank", "tNO")) : (text + Function.GetJsonString("IsPayToBank", "tYES")));
			if (!string.IsNullOrEmpty(doc.CashAcct))
			{
				text += Function.GetJsonString("CashAccount", Function.ToString(doc.CashAcct));
			}
			if (doc.DocCurr.Equals(mainCurrency))
			{
				if (doc.CashSum > 0m)
				{
					text += Function.GetJsonString("CashSum", Function.ParseDouble(doc.CashSum));
				}
			}
			else if (doc.CashSumFC > 0m)
			{
				text += Function.GetJsonString("CashSumFC", Function.ParseDouble(doc.CashSumFC));
			}
			if (!string.IsNullOrEmpty(doc.CheckAcct))
			{
				text += Function.GetJsonString("CheckAccount", Function.ToString(doc.CheckAcct));
			}
			if (!string.IsNullOrEmpty(doc.TrsfrAcct))
			{
				text += Function.GetJsonString("TransferAccount", Function.ToString(doc.TrsfrAcct));
			}
			if (doc.DocCurr.Equals(mainCurrency))
			{
				if (doc.TrsfrSum > 0m)
				{
					text += Function.GetJsonString("TransferSum", Function.ParseDouble(doc.TrsfrSum));
				}
			}
			else if (doc.TrsfrSumFC > 0m)
			{
				text += Function.GetJsonString("TransferSum", Function.ParseDouble(doc.TrsfrSumFC));
			}
			if (doc.TrsfrDate > new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("TransferDate", Function.ParseDateTimes(doc.TrsfrDate).ToString("yyyyMMdd"));
			}
			if (!string.IsNullOrEmpty(doc.TrsfrRef))
			{
				text += Function.GetJsonString("TransferReference", Function.ToString(doc.TrsfrRef));
			}
			if (doc.BcgSum != 0.0)
			{
				text += Function.GetJsonString("BankChargeAmount", Function.ParseDouble(doc.BcgSum));
			}
			if (doc.BcgSumFC != 0.0)
			{
				text += Function.GetJsonString("BankChargeAmountInFC", Function.ParseDouble(doc.BcgSumFC));
			}
			if (!string.IsNullOrEmpty(doc.Reference1))
			{
				text += Function.GetJsonString("Reference1", Function.ToString(doc.Reference1));
			}
			if (!string.IsNullOrEmpty(doc.Reference2))
			{
				text += Function.GetJsonString("Reference2", Function.ToString(doc.Reference2));
			}
			if (!string.IsNullOrEmpty(doc.CounterReference))
			{
				text += Function.GetJsonString("CounterReference", Function.ToString(doc.CounterReference));
			}
			if (doc.Series != 0)
			{
				text += Function.GetJsonString("Series", Function.ParseInt(doc.Series));
			}
			if (doc.BPLId != 0)
			{
				text += Function.GetJsonString("BPLID", Function.ParseInt(doc.BPLId));
			}
			text += DataService.MappingUdf(doc.UserFields);
			if ((doc.DocType.Equals("C") || doc.DocType.Equals("S")) && doc.Invoices != null && doc.Invoices.Count > 0)
			{
				text += "\"PaymentInvoices\":[";
				for (int i = 0; i < doc.Invoices.Count; i++)
				{
					Payments_InvoicesImpl payments_InvoicesImpl = doc.Invoices[i];
					if (payments_InvoicesImpl.DocEntry > 0)
					{
						text += "{";
						if (payments_InvoicesImpl.DocEntry > 0)
						{
							text += Function.GetJsonString("DocEntry", Function.ParseInt(payments_InvoicesImpl.DocEntry));
						}
						if (payments_InvoicesImpl.DocLine >= 0)
						{
							text += Function.GetJsonString("DocLine", payments_InvoicesImpl.DocLine);
						}
						if (payments_InvoicesImpl.PaidSum != 0.0)
						{
							text += Function.GetJsonString("PaidSum", Function.ParseDouble(payments_InvoicesImpl.PaidSum));
						}
						if (payments_InvoicesImpl.SumApplied != 0.0)
						{
							text += Function.GetJsonString("SumApplied", Function.ParseDouble(payments_InvoicesImpl.SumApplied));
						}
						if (payments_InvoicesImpl.AppliedFC != 0.0)
						{
							text += Function.GetJsonString("AppliedFC", Function.ParseDouble(payments_InvoicesImpl.AppliedFC));
						}
						if (!string.IsNullOrEmpty(payments_InvoicesImpl.InvType))
						{
							text += Function.GetJsonString("InvoiceType", Function.ToString(payments_InvoicesImpl.InvType));
						}
						text += DataService.MappingUdf(payments_InvoicesImpl.UserFields);
						text += "},";
					}
				}
				text += "],";
			}
			if (doc.DocType.Equals("A") && doc._listPurpose != null && doc._listPurpose.Count > 0)
			{
				text += "\"PaymentAccounts\":[";
				for (int j = 0; j < doc._listPurpose.Count; j++)
				{
					Payments_AccountsImpl payments_AccountsImpl = doc._listPurpose[j];
					text += "{";
					if (!string.IsNullOrEmpty(payments_AccountsImpl.AcctCode))
					{
						text += Function.GetJsonString("AccountCode", Function.ToString(payments_AccountsImpl.AcctCode));
					}
					if (!string.IsNullOrEmpty(payments_AccountsImpl.AcctName))
					{
						text += Function.GetJsonString("AccountName", Function.ToString(payments_AccountsImpl.AcctName));
					}
					if (!string.IsNullOrEmpty(payments_AccountsImpl.Descrip))
					{
						text += Function.GetJsonString("Decription", Function.ToString(payments_AccountsImpl.Descrip));
					}
					if (payments_AccountsImpl.GrssAmntFC != 0.0)
					{
						text += Function.GetJsonString("GrossAmount", Function.ParseDouble(payments_AccountsImpl.GrssAmntFC));
					}
					else if (payments_AccountsImpl.GrossAmnt != 0.0)
					{
						text += Function.GetJsonString("GrossAmount", Function.ParseDouble(payments_AccountsImpl.GrossAmnt));
					}
					if (!string.IsNullOrEmpty(payments_AccountsImpl.OcrCode))
					{
						text += Function.GetJsonString("ProfitCenter", Function.ToString(payments_AccountsImpl.OcrCode));
					}
					if (!string.IsNullOrEmpty(payments_AccountsImpl.OcrCode2))
					{
						text += Function.GetJsonString("ProfitCenter2", Function.ToString(payments_AccountsImpl.OcrCode2));
					}
					if (!string.IsNullOrEmpty(payments_AccountsImpl.OcrCode3))
					{
						text += Function.GetJsonString("ProfitCenter3", Function.ToString(payments_AccountsImpl.OcrCode3));
					}
					if (!string.IsNullOrEmpty(payments_AccountsImpl.OcrCode4))
					{
						text += Function.GetJsonString("ProfitCenter4", Function.ToString(payments_AccountsImpl.OcrCode4));
					}
					if (!string.IsNullOrEmpty(payments_AccountsImpl.OcrCode5))
					{
						text += Function.GetJsonString("ProfitCenter5", Function.ToString(payments_AccountsImpl.OcrCode5));
					}
					if (!string.IsNullOrEmpty(payments_AccountsImpl.Project))
					{
						text += Function.GetJsonString("ProjectCode", Function.ToString(payments_AccountsImpl.Project));
					}
					if (payments_AccountsImpl.AppliedFC != 0.0)
					{
						text += Function.GetJsonString("SumPaid", Function.ParseDecimal(payments_AccountsImpl.AppliedFC));
					}
					else if (payments_AccountsImpl.SumApplied != 0.0)
					{
						text += Function.GetJsonString("SumPaid", Function.ParseDecimal(payments_AccountsImpl.SumApplied));
					}
					if (payments_AccountsImpl.VatAmnt != 0m)
					{
						text += Function.GetJsonString("VatAmount", Function.ParseDouble(payments_AccountsImpl.VatAmnt));
					}
					if (!string.IsNullOrEmpty(payments_AccountsImpl.VatGroup))
					{
						text += Function.GetJsonString("VatGroup", Function.ToString(payments_AccountsImpl.VatGroup));
					}
					text += DataService.MappingUdf(payments_AccountsImpl.UserFields);
					text += "},";
				}
				text += "],";
			}
			if ((doc.DocType.Equals("C") || doc.DocType.Equals("V")) && doc._listCheck != null && doc._listCheck.Count > 0)
			{
				text += "\"PaymentChecks\":[";
				for (int k = 0; k < doc._listCheck.Count; k++)
				{
					Payments_ChecksImpl payments_ChecksImpl = doc._listCheck[k];
					if (!string.IsNullOrWhiteSpace(doc.CheckAcct) && payments_ChecksImpl.CheckSum != 0.0 && !(payments_ChecksImpl.DueDate < new DateTime(1900, 1, 1)))
					{
						text += "{";
						if (payments_ChecksImpl.DueDate > new DateTime(1900, 1, 1))
						{
							text += Function.GetJsonString("DueDate", Function.ParseDateTimes(payments_ChecksImpl.DueDate).ToString("yyyyMMdd"));
						}
						if (payments_ChecksImpl.CheckNumber != 0)
						{
							text += Function.GetJsonString("CheckNumber", Function.ParseDouble(payments_ChecksImpl.CheckNumber));
						}
						if (!string.IsNullOrEmpty(payments_ChecksImpl.BankCode))
						{
							text += Function.GetJsonString("BankCode", Function.ToString(payments_ChecksImpl.BankCode));
						}
						if (!string.IsNullOrEmpty(payments_ChecksImpl.Branch))
						{
							text += Function.GetJsonString("Branch", Function.ToString(payments_ChecksImpl.Branch));
						}
						if (!string.IsNullOrEmpty(payments_ChecksImpl.AcctNum))
						{
							text += Function.GetJsonString("AccounttNum", Function.ToString(payments_ChecksImpl.AcctNum));
						}
						text = ((!Function.ParseBool(payments_ChecksImpl.Trnsfrable)) ? (text + Function.GetJsonString("Trnsfrable", "tNO")) : (text + Function.GetJsonString("Trnsfrable", "tYES")));
						if (payments_ChecksImpl.CheckSum != 0.0)
						{
							text += Function.GetJsonString("CheckSum", Function.ParseDouble(payments_ChecksImpl.CheckSum));
						}
						if (!string.IsNullOrEmpty(payments_ChecksImpl.CountryCode))
						{
							text += Function.GetJsonString("CountryCode", Function.ToString(payments_ChecksImpl.CountryCode));
						}
						if (payments_ChecksImpl.CheckAbsEntry != 0)
						{
							text += Function.GetJsonString("CheckAbsEntry", Function.ParseInt(payments_ChecksImpl.CheckAbsEntry));
						}
						if (!string.IsNullOrEmpty(payments_ChecksImpl.CheckAccount))
						{
							text += Function.GetJsonString("CheckAccount", Function.ToString(payments_ChecksImpl.CheckAccount));
						}
						text = ((!Function.ParseBool(payments_ChecksImpl.ManualCheck)) ? (text + Function.GetJsonString("ManualCheck", "tNO")) : (text + Function.GetJsonString("ManualCheck", "tYES")));
						text += DataService.MappingUdf(payments_ChecksImpl.UserFields);
						text += "},";
					}
				}
				text += "],";
			}
			return text + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public override HttpResult SaveDraftToDocument(int docEntry, string sessionId)
	{
		try
		{
			HttpResult httpResult = DataService.ProcessJson(ApzObjectTypes.oPaymentsDrafts, sessionId, null, Enums.HttpAction.SaveDraftToDocument, docEntry);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult;
			}
			return new HttpResult(Enums.MessageCode.Success, "", Function.ParseInt(httpResult.Content));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
