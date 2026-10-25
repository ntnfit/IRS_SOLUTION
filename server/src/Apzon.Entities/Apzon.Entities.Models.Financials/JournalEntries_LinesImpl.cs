using System;
using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;
using SAPbobsCOM;

namespace Apzon.Entities.Models.Financials;

public class JournalEntries_LinesImpl
{
	public string Account { get; set; }

	public string Ref3Line { get; set; }

	public int BPLId { get; set; }

	public string BPLName { get; set; }

	public double BaseSum { get; set; }

	public int BlockReason { get; set; }

	public int CheckAbs { get; set; }

	public string ContraAct { get; set; }

	public string ControlAccount { get; set; }

	public string ProfitCode { get; set; }

	public string OcrCode2 { get; set; }

	public string OcrCode3 { get; set; }

	public string OcrCode4 { get; set; }

	public string OcrCode5 { get; set; }

	public int Count { get; set; }

	public double Credit { get; set; }

	public double SYSCred { get; set; }

	public double Debit { get; set; }

	public double SYSDeb { get; set; }

	public DateTime DueDate { get; set; }

	public double EqualizationTaxAmount { get; set; }

	public double FCCredit { get; set; }

	public string FCCurrency { get; set; }

	public double FCDebit { get; set; }

	public string FederalTaxID { get; set; }

	public double GrossValue { get; set; }

	public string LineMemo { get; set; }

	public int Line_ID { get; set; }

	public int LocationCode { get; set; }

	public BoYesNoEnum PaymentBlock { get; set; }

	public BoYesNoEnum PaymentOrdered { get; set; }

	public List<CashFlowAssignmentsImpl> PrimaryFormItems { get; set; }

	public string Project { get; set; }

	public string Ref1 { get; set; }

	public string Ref2 { get; set; }

	public DateTime RefDate { get; set; }

	public DateTime Ref2Date { get; set; }

	public string ShortName { get; set; }

	public double SYSBaseSum { get; set; }

	public double SystemEqualizationTaxAmount { get; set; }

	public double SystemTotalTax { get; set; }

	public double SYSVatSum { get; set; }

	public string TaxCode { get; set; }

	public DateTime TaxDate { get; set; }

	public string VatGroup { get; set; }

	public BoTaxPostAccEnum TaxPostAccount { get; set; }

	public double TotalTax { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public string VatRegNum { get; set; }

	public double VatAmount { get; set; }

	public DateTime VatDate { get; set; }

	public BoYesNoEnum VatLine { get; set; }

	public BoYesNoEnum WTLiable { get; set; }

	public BoYesNoEnum WTRow { get; set; }
}
