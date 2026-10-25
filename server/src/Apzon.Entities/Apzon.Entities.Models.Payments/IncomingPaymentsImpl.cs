using System;
using System.Collections.Generic;
using Apzon.Entities.Models.ARInvoice;
using Apzon.Entities.Models.UserFields;
using SAPbobsCOM;

namespace Apzon.Entities.Models.Payments;

public class IncomingPaymentsImpl
{
	public int DocEntry { get; set; }

	public int DocNum { get; set; }

	public string DocType { get; set; }

	public string Canceled { get; set; }

	public string Printed { get; set; }

	public DateTime DocDate { get; set; }

	public string DocDueDate { get; set; }

	public string CardCode { get; set; }

	public string CardName { get; set; }

	public string Address { get; set; }

	public string CashAcct { get; set; }

	public decimal CashSum { get; set; }

	public decimal CreditSum { get; set; }

	public decimal CouponSum { get; set; }

	public string TrsfrAcct { get; set; }

	public string BpAct { get; set; }

	public decimal TrsfrSum { get; set; }

	public DateTime TrsfrDate { get; set; }

	public string TrsfrRef { get; set; }

	public string DocCurr { get; set; }

	public decimal DocRate { get; set; }

	public decimal DocTotal { get; set; }

	public string JrnlMemo { get; set; }

	public int TransId { get; set; }

	public TimeSpan DocTime { get; set; }

	public string ObjType { get; set; }

	public DateTime UpdateDate { get; set; }

	public DateTime CreateDate { get; set; }

	public int UserSign { get; set; }

	public string VatGroup { get; set; }

	public decimal VatSum { get; set; }

	public decimal Dcount { get; set; }

	public decimal DcntSum { get; set; }

	public decimal CashSumFC { get; set; }

	public decimal CreditSumFC { get; set; }

	public decimal TrsfrSumFC { get; set; }

	public decimal CashSumSy { get; set; }

	public decimal CreditSumSy { get; set; }

	public decimal TrsfrSumSy { get; set; }

	public decimal VatSumFC { get; set; }

	public decimal VatSumSy { get; set; }

	public decimal DcntSumFC { get; set; }

	public decimal DocTotalFC { get; set; }

	public decimal DocTotalSy { get; set; }

	public Payments_Accounts AccountPayments { get; set; }

	public bool ApplyVAT { get; set; }

	public PaymentsAuthorizationStatusEnum AuthorizationStatus { get; set; }

	public int BPLId { get; set; }

	public string BPLName { get; set; }

	public string BankAcct { get; set; }

	public double BcgSum { get; set; }

	public double BcgSumFC { get; set; }

	public double BankChargeAmountInSC { get; set; }

	public string BankCode { get; set; }

	public BillOfExchange BillOfExchange { get; set; }

	public string BillOfExchangeAgent { get; set; }

	public double BillOfExchangeAmount { get; set; }

	public double BillOfExchangeAmountFC { get; set; }

	public double BillOfExchangeAmountSC { get; set; }

	public BoBoeStatus BillofExchangeStatus { get; set; }

	public string BoeAccount { get; set; }

	public DataBrowser Browser { get; set; }

	public bool Cancelled { get; set; }

	public string CashAccount { get; set; }

	public double CashSumSys { get; set; }

	public string CheckAcct { get; set; }

	public Payments_Checks Checks { get; set; }

	public int ContactPersonCode { get; set; }

	public string ControlAccount { get; set; }

	public string CounterReference { get; set; }

	public Payments_CreditCards CreditCards { get; set; }

	public double DeductionPercent { get; set; }

	public double DeductionSum { get; set; }

	public string DocCurrency { get; set; }

	public BoPaymentsObjectType DocObjectCode { get; set; }

	public BoRcptTypes DocTypte { get; set; }

	public DateTime DueDate { get; set; }

	public bool HandWritten { get; set; }

	public List<Payments_InvoicesImpl> Invoices { get; set; }

	public string IsPaytoBnk { get; set; }

	public bool LocalCurrency { get; set; }

	public string DiffCurr { get; set; }

	public int LocationCode { get; set; }

	public string PayToBankAccountNo { get; set; }

	public string PayToBankBranch { get; set; }

	public string PayToBankCode { get; set; }

	public string PayToBankCountry { get; set; }

	public string PayToCode { get; set; }

	public BoPaymentPriorities PaymentPriority { get; set; }

	public BoORCTPaymentTypeEnum PaymentType { get; set; }

	public Payments_ApprovalRequests Payments_ApprovalRequests { get; set; }

	public CashFlowAssignments PrimaryFormItems { get; set; }

	public bool Proforma { get; set; }

	public string PrjCode { get; set; }

	public string Reference1 { get; set; }

	public string Reference2 { get; set; }

	public string Comments { get; set; }

	public int Series { get; set; }

	public DateTime TaxDate { get; set; }

	public string TaxGroup { get; set; }

	public string TransactionCode { get; set; }

	public double TransferRealAmount { get; set; }

	public double UnderOverpaymentdiffFC { get; set; }

	public double UnderOverpaymentdiffSC { get; set; }

	public double UnderOverpaymentdifference { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public string VATRegNum { get; set; }

	public DateTime VatDate { get; set; }

	public string WTAccount { get; set; }

	public double WTAmount { get; set; }

	public double WTAmountFC { get; set; }

	public double WTAmountSC { get; set; }

	public string WTCode { get; set; }

	public double WTTaxableAmount { get; set; }

	public double WtBaseSum { get; set; }

	public double WtBaseSumFC { get; set; }

	public double WtBaseSumSC { get; set; }

	public List<IncomingPaymentInvoicesImpl> _listInvoice { get; set; }

	public List<Payments_AccountsImpl> _listPurpose { get; set; }

	public List<Payments_ChecksImpl> _listCheck { get; set; }

	public IncomingPaymentCreditCardsImpl _objCreditCard { get; set; }

	public IncomingPaymentCouponsImpl _objCoupons { get; set; }

	public ARInvoicesImpl _objARInvoice { get; set; }

	public ARInvoicesImpl _objARInvoiceDetail { get; set; }

	public ARInvoicesImpl _objAPInvoice { get; set; }

	public ARInvoicesImpl _objAPInvoiceDetail { get; set; }

	public bool _isCustomerARInvoice { get; set; }

	public bool _isVendorAPInvoice { get; set; }

	public bool _isPurpose { get; set; }
}
