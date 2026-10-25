using System;
using System.Collections.Generic;
using Apzon.Entities.Models.Financials;
using Apzon.Entities.Models.UserFields;
using SAPbobsCOM;

namespace Apzon.Entities.Models.Payments;

public class PaymentsImpl
{
	public List<Payments_AccountsImpl> AccountPayments { get; set; }

	public string Address { get; set; }

	public bool ApplyVAT { get; set; }

	public string AuthorizationStatus { get; set; }

	public int BPLID { get; set; }

	public string BPLName { get; set; }

	public string BankAccount { get; set; }

	public double BankChargeAmount { get; set; }

	public double BankChargeAmountInFC { get; set; }

	public double BankChargeAmountInSC { get; set; }

	public string BankCode { get; set; }

	public BillOfExchangeImpl BillOfExchange { get; set; }

	public string BillOfExchangeAgent { get; set; }

	public double BillOfExchangeAmount { get; set; }

	public double BillOfExchangeAmountFC { get; set; }

	public double BillOfExchangeAmountSC { get; set; }

	public string BillofExchangeStatus { get; set; }

	public string BoeAccount { get; set; }

	public bool Cancelled { get; set; }

	public string CardCode { get; set; }

	public string CardName { get; set; }

	public string CashAccount { get; set; }

	public double CashSum { get; set; }

	public double CashSumFC { get; set; }

	public double CashSumSys { get; set; }

	public string CheckAccount { get; set; }

	public List<Payments_ChecksImpl> Checks { get; set; }

	public int ContactPersonCode { get; set; }

	public string ControlAccount { get; set; }

	public string CounterReference { get; set; }

	public Payments_CreditCards CreditCards { get; set; }

	public double DeductionPercent { get; set; }

	public double DeductionSum { get; set; }

	public string DocCurrency { get; set; }

	public DateTime DocDate { get; set; }

	public int DocEntry { get; set; }

	public int DocNum { get; set; }

	public string DocObjectCode { get; set; }

	public double DocRate { get; set; }

	public string DocType { get; set; }

	public string DocTypte { get; set; }

	public DateTime DueDate { get; set; }

	public bool HandWritten { get; set; }

	public List<Payments_InvoicesImpl> Invoices { get; set; }

	public bool IsPayToBank { get; set; }

	public string JournalRemarks { get; set; }

	public bool LocalCurrency { get; set; }

	public int LocationCode { get; set; }

	public string PayToBankAccountNo { get; set; }

	public string PayToBankBranch { get; set; }

	public string PayToBankCode { get; set; }

	public string PayToBankCountry { get; set; }

	public string PayToCode { get; set; }

	public string PaymentPriority { get; set; }

	public string PaymentType { get; set; }

	public List<Payments_ApprovalRequestsImpl> Payments_ApprovalRequests { get; set; }

	public List<CashFlowAssignmentsImpl> PrimaryFormItems { get; set; }

	public bool Printed { get; set; }

	public bool Proforma { get; set; }

	public string ProjectCode { get; set; }

	public string Reference1 { get; set; }

	public string Reference2 { get; set; }

	public string Remarks { get; set; }

	public int Series { get; set; }

	public DateTime TaxDate { get; set; }

	public string TaxGroup { get; set; }

	public string TransactionCode { get; set; }

	public string TransferAccount { get; set; }

	public DateTime TransferDate { get; set; }

	public double TransferRealAmount { get; set; }

	public string TransferReference { get; set; }

	public double TransferSum { get; set; }

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
}
