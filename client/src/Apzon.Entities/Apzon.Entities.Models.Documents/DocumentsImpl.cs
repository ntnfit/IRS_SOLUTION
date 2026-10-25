using System;
using System.Collections.Generic;
using Apzon.Commons;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;
using SAPbobsCOM;

namespace Apzon.Entities.Models.Documents;

public class DocumentsImpl
{
	public string ATDocumentType { get; set; }

	public int ObjType { get; set; }

	public string Address { get; set; }

	public string Address2 { get; set; }

	public AddressExtensionImpl AddressExtension { get; set; }

	public string AgentCode { get; set; }

	public int AnnualInvoiceDeclarationReference { get; set; }

	public BoYesNoEnum ApplyCurrentVATRatesForDownPaymentsToDraw { get; set; }

	public BoYesNoEnum ApplyTaxOnFirstInstallment { get; set; }

	public BoYesNoEnum ArchiveNonremovableSalesQuotation { get; set; }

	public DateTime AssetDate { get; set; }

	public int AttachmentEntry { get; set; }

	public string AuthorizationCode { get; set; }

	public DocumentAuthorizationStatusEnum AuthorizationStatus { get; set; }

	public string WddStatus { get; set; }

	public string BPChCode { get; set; }

	public int BPChCntc { get; set; }

	public string BPLName { get; set; }

	public int BPL_IDAssignedToInvoice { get; set; }

	public double BaseAmount { get; set; }

	public double BaseAmountFC { get; set; }

	public double BaseAmountSC { get; set; }

	public BoYesNoEnum BillOfExchangeReserved { get; set; }

	public int BlanketAgreementNumber { get; set; }

	public BoYesNoEnum BlockDunning { get; set; }

	public string Box1099 { get; set; }

	public DataBrowser Browser { get; set; }

	public DateTime CancelDate { get; set; }

	public CancelStatusEnum CancelStatus { get; set; }

	public BoYesNoEnum Cancelled { get; set; }

	public string Canceled { get; set; }

	public string CardCode { get; set; }

	public string CardName { get; set; }

	public int CdcOffset { get; set; }

	public string CntrlBnk { get; set; }

	public string CertificationNumber { get; set; }

	public DateTime ClosingDate { get; set; }

	public ClosingOptionEnum ClosingOption { get; set; }

	public string Footer { get; set; }

	public string Comments { get; set; }

	public string Confirmed { get; set; }

	public int CntctCode { get; set; }

	public string ContactPersonName { get; set; }

	public string CtlAccount { get; set; }

	public BoYesNoEnum CreateOnlineQuotation { get; set; }

	public DateTime CreationDate { get; set; }

	public BoYesNoEnum DeferredTax { get; set; }

	public double DiscPrcnt { get; set; }

	public double DiscSum { get; set; }

	public string DocCur { get; set; }

	public DateTime DocDate { get; set; }

	public DateTime DocDueDate { get; set; }

	public int DocEntry { get; set; }

	public int DocNum { get; set; }

	public ApzObjectTypes DocObjectCode { get; set; }

	public string DocObjectCodeEx { get; set; }

	public double DocRate { get; set; }

	public int DocTime { get; set; }

	public double DocTotal { get; set; }

	public double TotalBeforeDis { get; set; }

	public double DocTotalFc { get; set; }

	public double DocTotalSys { get; set; }

	public string DocType { get; set; }

	public DocumentDeliveryTypeEnum DocumentDelivery { get; set; }

	public string DocStatus { get; set; }

	public BoDocumentSubType DocumentSubType { get; set; }

	public List<Document_ApprovalRequestsImpl> Document_ApprovalRequests { get; set; }

	public int DocumentsOwner { get; set; }

	public int OwnerCode { get; set; }

	public string OwnerName { get; set; }

	public double DownPayment { get; set; }

	public double DPMTotal { get; set; }

	public double DpmAmnt { get; set; }

	public double DpmAmntFC { get; set; }

	public double DpmAmntSC { get; set; }

	public double DpmPrcnt { get; set; }

	public BoSoStatus DownPaymentStatus { get; set; }

	public string U_DownPaymentStatus { get; set; }

	public string Posted { get; set; }

	public List<DownPaymentsToDrawImpl> DownPaymentsToDraw { get; set; }

	public string EDocErrorCode { get; set; }

	public string EDocErrorMessage { get; set; }

	public int EDocExportFormat { get; set; }

	public EDocGenerationTypeEnum EDocGenerationType { get; set; }

	public string EDocNum { get; set; }

	public int EDocSeries { get; set; }

	public string EDocStatus { get; set; }

	public string ETaxNumber { get; set; }

	public int ETaxWebSite { get; set; }

	public string ElecCommMessage { get; set; }

	public ElecCommStatusEnum ElecCommStatus { get; set; }

	public DateTime EndDeliveryDate { get; set; }

	public int EndDeliveryTime { get; set; }

	public DateTime ExemptionValidityDateFrom { get; set; }

	public DateTime ExemptionValidityDateTo { get; set; }

	public List<DocumentsAdditionalExpensesImpl> Expenses { get; set; }

	public List<DocumentFreightImpl> Freight { get; set; }

	public string ExternalCorrectedDocNum { get; set; }

	public int ExtraDays { get; set; }

	public int ExtraMonth { get; set; }

	public string LicTradNum { get; set; }

	public int FinncPriod { get; set; }

	public string FiscalDocNum { get; set; }

	public int FolioNumber { get; set; }

	public string FolioPrefixString { get; set; }

	public int Form1099 { get; set; }

	public int GTSChecker { get; set; }

	public int GTSPayee { get; set; }

	public BoYesNoEnum GroupHandWritten { get; set; }

	public int GroupNumber { get; set; }

	public int GroupSeries { get; set; }

	public string HandWritten { get; set; }

	public string Handwrtten { get; set; }

	public int ImportFileNum { get; set; }

	public string Indicator { get; set; }

	public List<Document_InstallmentsImpl> Installments { get; set; }

	public BoYesNoEnum InsuranceOperation347 { get; set; }

	public int InternalCorrectedDocNum { get; set; }

	public BoYesNoEnum IsAlteration { get; set; }

	public BoYesNoEnum IsPayToBank { get; set; }

	public string JrnlMemo { get; set; }

	public int LanguageCode { get; set; }

	public List<Document_LinesImpl> Lines { get; set; }

	public string ManualNumber { get; set; }

	public BoYesNoEnum MaximumCashDiscount { get; set; }

	public BoYesNoEnum NTSApproved { get; set; }

	public string NTSApprovedNumber { get; set; }

	public BoYesNoEnum NetProcedure { get; set; }

	public int NextCorrectingDocument { get; set; }

	public string NumAtCard { get; set; }

	public int NumberOfInstallments { get; set; }

	public BoYesNoEnum OpenForLandedCosts { get; set; }

	public string Header { get; set; }

	public int POSCashierNumber { get; set; }

	public int POSDailySummaryNo { get; set; }

	public string POSEquipmentNumber { get; set; }

	public string POSManufacturerSerialNumber { get; set; }

	public int POSReceiptNo { get; set; }

	public List<DocumentPackagesImpl> Packages { get; set; }

	public string PartSupply { get; set; }

	public string PayToBankAccountNo { get; set; }

	public string PayToBankBranch { get; set; }

	public string PayToBankCode { get; set; }

	public string PayToBankCountry { get; set; }

	public string PayToCode { get; set; }

	public BoYesNoEnum PaymentBlock { get; set; }

	public int PaymentBlockEntry { get; set; }

	public int GroupNum { get; set; }

	public string PeyMethod { get; set; }

	public string PaymentReference { get; set; }

	public string PeriodIndicator { get; set; }

	public string Pick { get; set; }

	public string PickRmrk { get; set; }

	public string PickStatus { get; set; }

	public BoYesNoEnum PrintSEPADirect { get; set; }

	public string Printed { get; set; }

	public int PrivateKeyVersion { get; set; }

	public string Project { get; set; }

	public int Receiver { get; set; }

	public string Reference1 { get; set; }

	public string Reference2 { get; set; }

	public int Releaser { get; set; }

	public BoYesNoEnum RelevantToGTS { get; set; }

	public BoYesNoEnum ReopenManuallyClosedOrCanceledDocument { get; set; }

	public BoYesNoEnum ReopenOriginalDocument { get; set; }

	public int ReqType { get; set; }

	public string ReqTypeName { get; set; }

	public string Requester { get; set; }

	public int RequesterBranch { get; set; }

	public string RequesterBranchName { get; set; }

	public int Branch { get; set; }

	public int RequesterDepartment { get; set; }

	public string RequesterDepartmentName { get; set; }

	public int Department { get; set; }

	public string Email { get; set; }

	public string RequesterName { get; set; }

	public string ReqName { get; set; }

	public string Reserve { get; set; }

	public bool ReserveInvoice { get; set; }

	public BoYesNoEnum ReuseDocumentNum { get; set; }

	public BoYesNoEnum ReuseNotaFiscalNum { get; set; }

	public string RevisionPo { get; set; }

	public string Rounding { get; set; }

	public double RoundingDiffAmount { get; set; }

	public double RoundingDiffAmountFC { get; set; }

	public double RoundingDiffAmountSC { get; set; }

	public int SlpCode { get; set; }

	public string SalesPersonName { get; set; }

	public int Segment { get; set; }

	public BoYesNoEnum SendNotification { get; set; }

	public int SequenceCode { get; set; }

	public string SequenceModel { get; set; }

	public int SequenceSerial { get; set; }

	public int Series { get; set; }

	public string SeriesString { get; set; }

	public double ServiceGrossProfitPercent { get; set; }

	public string ShipToCode { get; set; }

	public string ShowSCN { get; set; }

	public string SignatureDigest { get; set; }

	public string SignatureInputMessage { get; set; }

	public List<Document_SpecialLinesImpl> SpecialLines { get; set; }

	public DateTime SpecifiedClosingDate { get; set; }

	public DateTime StartDeliveryDate { get; set; }

	public int StartDeliveryTime { get; set; }

	public BoPayTermDueTypes StartFrom { get; set; }

	public string SubSeriesString { get; set; }

	public string Submitted { get; set; }

	public BoDocSummaryTypes SummeryType { get; set; }

	public string Supplier { get; set; }

	public DateTime TaxDate { get; set; }

	public string TaxExemptionLetterNum { get; set; }

	public TaxExtensionImpl TaxExtension { get; set; }

	public double TotalDiscount { get; set; }

	public double TotalDiscountFC { get; set; }

	public double TotalDiscountSC { get; set; }

	public double TotalEqualizationTax { get; set; }

	public double TotalEqualizationTaxFC { get; set; }

	public double TotalEqualizationTaxSC { get; set; }

	public string TrackingNumber { get; set; }

	public int TransNum { get; set; }

	public int TrnspCode { get; set; }

	public DateTime UpdateDate { get; set; }

	public BoYesNoEnum UseCorrectionVATGroup { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public string VATRegNum { get; set; }

	public DateTime VatDate { get; set; }

	public double VatPercent { get; set; }

	public double VatSum { get; set; }

	public double VatSumFc { get; set; }

	public double VatSumSys { get; set; }

	public string VehiclePlate { get; set; }

	public double WTAmount { get; set; }

	public double WTAmountFC { get; set; }

	public double WTAmountSC { get; set; }

	public double WTApplied { get; set; }

	public double WTAppliedFC { get; set; }

	public double WTAppliedSC { get; set; }

	public double WTExemptedAmount { get; set; }

	public double WTExemptedAmountFC { get; set; }

	public double WTExemptedAmountSC { get; set; }

	public double WTNonSubjectAmount { get; set; }

	public double WTNonSubjectAmountFC { get; set; }

	public double WTNonSubjectAmountSC { get; set; }

	public BoDocWhsUpdateTypes WareHouseUpdateType { get; set; }

	public List<WithholdingTaxDataImpl> WithholdingTaxData { get; set; }

	public string DocStatusName { get; set; }

	public string ShipTypeName { get; set; }

	public ApzObjectTypes TypeTo { get; set; }

	public ApzObjectTypes TypeFrom { get; set; }

	public int ObjTypeFrom { get; set; }

	public int ListNum { get; set; }

	public string ListName { get; set; }

	public int PriceListNo { get; set; }

	public string PriceListName { get; set; }

	public string SeriesName { get; set; }

	public string SuppName { get; set; }

	public string AgentName { get; set; }

	public string AgentNum { get; set; }

	public string Descr { get; set; }

	public string Ref1 { get; set; }

	public string Ref2 { get; set; }

	public decimal OIPFDocRate { get; set; }

	public decimal ExpCustom { get; set; }

	public decimal ActCustom { get; set; }

	public decimal Vat1 { get; set; }

	public decimal Vat2 { get; set; }

	public decimal BeforeVat { get; set; }

	public decimal OIPFDocTotal { get; set; }

	public decimal CostSum { get; set; }

	public TimeSpan OIPFDocTime { get; set; }

	public string OIPFCanceled { get; set; }

	public decimal ExCustomFC { get; set; }

	public decimal AcCustomFC { get; set; }

	public decimal Vat1FC { get; set; }

	public decimal Vat2FC { get; set; }

	public decimal BeforVatFC { get; set; }

	public decimal DocTotalFC { get; set; }

	public decimal CostSumFC { get; set; }

	public DateTime CloseDate { get; set; }

	public decimal Cost_Match { get; set; }

	public decimal C_Match_FC { get; set; }

	public string BillOfLad { get; set; }

	public int OIPFTrnspCode { get; set; }

	public decimal CostFactor { get; set; }

	public DateTime CreateDate { get; set; }

	public int JdtNum { get; set; }

	public string JdtMemo { get; set; }

	public int UserSign { get; set; }

	public string OIPFObjType { get; set; }

	public decimal ExCustomSC { get; set; }

	public decimal ActCustSC { get; set; }

	public decimal TtlCostSC { get; set; }

	public string OpenForLaC { get; set; }

	public string incCustom { get; set; }

	public string isIns { get; set; }

	public string IsRIV { get; set; }

	public decimal Total { get; set; }

	public decimal TaxTotal { get; set; }

	public decimal Discount { get; set; }

	public int BPLId { get; set; }

	public List<DocPayment> Payments { get; set; }

	public DateTime ReqDate { get; set; }

	public string SubCatNum { get; set; }

	public int AtcEntry { get; set; }

	public List<AttachmentFile_Impl> AttachmentFiles { get; set; }

	public string SlpName { get; set; }

	public int draftKey { get; set; }

	public string UserName { get; set; }

	public List<DocumentUdo> Udos { get; set; }

	public bool IsDraft { get; set; }

	public int DeliveryCancelEntry { get; set; }

	public string IsDeliveryCancel { get; set; }

	public string FilterString { get; set; }

	public List<DocumentLinesBinAllocationsImpl> BinAllocations { get; set; }

	public int ImportEnt { get; set; }

	public string OriginType { get; set; }

	public string UseShpdGd { get; set; }

	public double PaidToDate { get; set; }

	public double PaidFC { get; set; }

	public double GrosProfit { get; set; }

	public double GrosProfFC { get; set; }

	public int ReceiptNum { get; set; }

	public double PaidSys { get; set; }

	public string IsICT { get; set; }

	public int UserSign2 { get; set; }

	public double PaidSum { get; set; }

	public double PaidSumFc { get; set; }

	public double PaidSumSc { get; set; }

	public string Filler { get; set; }

	public string CurSource { get; set; }

	public string ToWhsCode { get; set; }

	public List<Document_ReferencedImpl> DocumentRef { get; set; }

	public string PriceMode { get; set; }

	public bool SaveFromForm { get; set; }
}
