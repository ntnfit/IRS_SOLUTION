using System;
using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;
using SAPbobsCOM;

namespace Apzon.Entities.Models.Financials;

public class JournalEntriesImpl
{
	public BoYesNoEnum AutoVAT { get; set; }

	public BoYesNoEnum AutomaticWT { get; set; }

	public string BaseReference { get; set; }

	public int BlanketAgreementNumber { get; set; }

	public BoYesNoEnum BlockDunningLetter { get; set; }

	public DataBrowser Browser { get; set; }

	public string CertificationNumber { get; set; }

	public BoYesNoEnum Corisptivi { get; set; }

	public int Count { get; set; }

	public BoYesNoEnum DeferredTax { get; set; }

	public string DocType { get; set; }

	public DateTime DueDate { get; set; }

	public ECDPostingTypeEnum ECDPostingType { get; set; }

	public int FolioNumber { get; set; }

	public string FolioPrefixString { get; set; }

	public string Indicator { get; set; }

	public int TransId { get; set; }

	public List<JournalEntries_LinesImpl> Lines { get; set; }

	public int Location { get; set; }

	public string Memo { get; set; }

	public int Number { get; set; }

	public OperationCodeTypeEnum OperationCode { get; set; }

	public int Original { get; set; }

	public TransTypesEnum OriginalJournal { get; set; }

	public PrintStatusEnum Printed { get; set; }

	public int PrivateKeyVersion { get; set; }

	public string Project { get; set; }

	public string Ref1 { get; set; }

	public string Ref2 { get; set; }

	public string Ref3 { get; set; }

	public DateTime RefDate { get; set; }

	public BoYesNoEnum Report347 { get; set; }

	public BoYesNoEnum ReportEU { get; set; }

	public ResidenceNumberTypeEnum ResidenceNumberType { get; set; }

	public int Series { get; set; }

	public string SignatureDigest { get; set; }

	public string SignatureInputMessage { get; set; }

	public BoYesNoEnum StampTax { get; set; }

	public DateTime StornoDate { get; set; }

	public DateTime TaxDate { get; set; }

	public string TransCode { get; set; }

	public BoYesNoEnum AutoStorno { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public DateTime VatDate { get; set; }

	public double WTSum { get; set; }

	public double WTSumFC { get; set; }

	public double WTSumSC { get; set; }

	public WithholdingTaxData WithholdingTaxData { get; set; }

	public string U_PeriodCogs { get; set; }

	public string U_PeriodProd { get; set; }
}
