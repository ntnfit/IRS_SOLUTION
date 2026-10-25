using System;
using SAPbobsCOM;

namespace Apzon.Entities.Models.BusinessPartners;

public class BusinessPartnersImpl
{
	public BPAccountReceivablePayble AccountRecivablePayables { get; set; }

	public bool AccrualCriteria { get; set; }

	public string AdditionalID { get; set; }

	public string Address { get; set; }

	public bool Affiliate { get; set; }

	public string AliasName { get; set; }

	public AutomaticPostingEnum AutomaticPosting { get; set; }

	public int AvarageLate { get; set; }

	public bool BackOrder { get; set; }

	public string BankCountry { get; set; }

	public string BankChargesAllocationCode { get; set; }

	public string BillofExchangeonCollection { get; set; }

	public string BillToBuildingFloorRoom { get; set; }

	public string BilltoDefault { get; set; }

	public string BillToState { get; set; }

	public string Block { get; set; }

	public bool BlockDunning { get; set; }

	public BoYesNoEnum BookkeepingCertified { get; set; }

	public string Box1099 { get; set; }

	public BPBankAccounts BPBankAccounts { get; set; }

	public BPBranchAssignment BPBranchAssignment { get; set; }

	public BPPaymentDates BPPaymentDates { get; set; }

	public BPPaymentMethods BPPaymentMethods { get; set; }

	public BPWithholdingTax BPWithholdingTax { get; set; }

	public DataBrowser Browser { get; set; }

	public string BusinessType { get; set; }

	public int CampaignNumber { get; set; }

	public string CardCode { get; set; }

	public string CardForeignName { get; set; }

	public string CardName { get; set; }

	public BoCardTypes BoCardType { get; set; }

	public string CardType { get; set; }

	public string Cellular { get; set; }

	public string CertificateNumber { get; set; }

	public string City { get; set; }

	public int ClosingDateProcedureNumber { get; set; }

	public BoYesNoEnum CollectionAuthorization { get; set; }

	public int CommissionGroupCode { get; set; }

	public double CommissionPercent { get; set; }

	public BoCardCompanyTypes BoCompanyPrivate { get; set; }

	public string CompanyPrivate { get; set; }

	public string CompanyRegistrationNumber { get; set; }

	public string ContactPerson { get; set; }

	public int? CntctCode { get; set; }

	public string Name { get; set; }

	public string Position { get; set; }

	public string Tel1 { get; set; }

	public string County { get; set; }

	public string Country { get; set; }

	public int CreditCardCode { get; set; }

	public DateTime? CreditCardExpiration { get; set; }

	public string CreditCardNum { get; set; }

	public double CreditLimit { get; set; }

	public string Currency { get; set; }

	public double CurrentAccountBalance { get; set; }

	public double CurrentAccountBalanceFC { get; set; }

	public double CurrentAccountBalanceSC { get; set; }

	public string CustomerBillofExchangDisc { get; set; }

	public string CustomerBillofExchangPres { get; set; }

	public string ChannelBP { get; set; }

	public int DatevAccount { get; set; }

	public BoYesNoEnum DatevFirstDataEntry { get; set; }

	public string DebitorAccount { get; set; }

	public string DebitorAccountName { get; set; }

	public BoYesNoEnum DeductibleAtSource { get; set; }

	public string DeductionOffice { get; set; }

	public double DeductionPercent { get; set; }

	public DateTime? DeductionValidUntil { get; set; }

	public string DefaultAccount { get; set; }

	public string DefaultBankCode { get; set; }

	public string DefaultBankName { get; set; }

	public string DefaultAccountName { get; set; }

	public int DefaultBlanketAgreementNumber { get; set; }

	public string DefaultBranch { get; set; }

	public int DefaultTechnician { get; set; }

	public BoYesNoEnum DeferredTax { get; set; }

	public DiscountGroupBaseObjectEnum DiscountBaseObject { get; set; }

	public DiscountGroups DiscountGroups { get; set; }

	public double DiscountPercent { get; set; }

	public string DiscountRelations { get; set; }

	public string DME { get; set; }

	public string DownPaymentClearAct { get; set; }

	public string DownPaymentClearActName { get; set; }

	public string DownPaymentInterimAccount { get; set; }

	public string DownPaymentInterimAccountName { get; set; }

	public DateTime? DunningDate { get; set; }

	public int DunningLevel { get; set; }

	public string DunningTerm { get; set; }

	public string EDIRecipientID { get; set; }

	public string EDISenderID { get; set; }

	public int EffectiveDiscount { get; set; }

	public string EmailAddress { get; set; }

	public BoYesNoEnum Equalization { get; set; }

	public int ETaxWebSite { get; set; }

	public DateTime? ExemptionValidityDateFrom { get; set; }

	public DateTime? ExemptionValidityDateTo { get; set; }

	public string ExemptNum { get; set; }

	public DateTime? ExpirationDate { get; set; }

	public string ExportCode { get; set; }

	public string FatherCard { get; set; }

	public string FatherCardName { get; set; }

	public bool FatherType { get; set; }

	public string Fax { get; set; }

	public string FederalTaxID { get; set; }

	public string FeeAccount { get; set; }

	public BPFiscalTaxID FiscalTaxID { get; set; }

	public int FormCode1099 { get; set; }

	public string FreeText { get; set; }

	public BoYesNoEnum Frozen { get; set; }

	public bool FrozenFor { get; set; }

	public DateTime? FrozenFrom { get; set; }

	public string FrozenRemarks { get; set; }

	public DateTime? FrozenTo { get; set; }

	public string GlobalLocationNumber { get; set; }

	public int GroupCode { get; set; }

	public string GTSBankAccountNo { get; set; }

	public string GTSBillingAddrTel { get; set; }

	public string GTSRegNo { get; set; }

	public BoYesNoEnum HierarchicalDeduction { get; set; }

	public string HouseBank { get; set; }

	public string HouseBankAccount { get; set; }

	public string HouseBankBranch { get; set; }

	public string HouseBankCountry { get; set; }

	public string HouseBankIBAN { get; set; }

	public string IBAN { get; set; }

	public string Indicator { get; set; }

	public int Industry { get; set; }

	public string IndustryType { get; set; }

	public string InstructionKey { get; set; }

	public BoYesNoEnum InsuranceOperation347 { get; set; }

	public string InterestAccount { get; set; }

	public BPIntrastatExtension IntrastatExtension { get; set; }

	public double IntrestRatePercent { get; set; }

	public string ISRBillerID { get; set; }

	public int LanguageCode { get; set; }

	public int LastMultiReconciliationNum { get; set; }

	public string LinkedBusinessPartner { get; set; }

	public string MailAddress { get; set; }

	public string MailCity { get; set; }

	public string MailCounty { get; set; }

	public string MailCountry { get; set; }

	public string MailZipCode { get; set; }

	public double MaxAmountOfExemption { get; set; }

	public double MaxCommitment { get; set; }

	public double MinIntrest { get; set; }

	public string NationalInsuranceNum { get; set; }

	public bool NoDiscounts { get; set; }

	public string Notes { get; set; }

	public double OpenDeliveryNotesBalance { get; set; }

	public double OpenDeliveryNotesBalanceFC { get; set; }

	public double OpenDeliveryNotesBalanceSC { get; set; }

	public int OpenOpportunities { get; set; }

	public double OpenOrdersBalance { get; set; }

	public double OpenOrdersBalanceFC { get; set; }

	public double OpenOrdersBalanceSC { get; set; }

	public OperationCode347Enum OperationCode347 { get; set; }

	public string OtherReceivablePayable { get; set; }

	public string OwnerIDNumber { get; set; }

	public string Pager { get; set; }

	public bool PartialDelivery { get; set; }

	public string Password { get; set; }

	public BoYesNoEnum PaymentBlock { get; set; }

	public int PaymentBlockDescription { get; set; }

	public int PayTermsGrpCode { get; set; }

	public string PeymentMethodCode { get; set; }

	public string Picture { get; set; }

	public string PlanningGroup { get; set; }

	public int PriceListNum { get; set; }

	public int Priority { get; set; }

	public string Profession { get; set; }

	public string ProjectCode { get; set; }

	public string Phone1 { get; set; }

	public string Phone2 { get; set; }

	public string RateDiffAccount { get; set; }

	public string ReferenceDetails { get; set; }

	public string RepresentativeName { get; set; }

	public ResidenceNumberTypeEnum ResidenNumber { get; set; }

	public int SalesPersonCode { get; set; }

	public int Series { get; set; }

	public ShaamGroupEnum ShaamGroup { get; set; }

	public int ShippingType { get; set; }

	public string ShipToBuildingFloorRoom { get; set; }

	public string ShipToDefault { get; set; }

	public BoYesNoEnum SinglePayment { get; set; }

	public BoYesNoEnum SubjectToWithholdingTax { get; set; }

	public BoYesNoEnum SurchargeOverlook { get; set; }

	public string TaxExemptionLetterNum { get; set; }

	public BoTaxRoundingRuleTypes TaxRoundingRule { get; set; }

	public int Territory { get; set; }

	public AssesseeTypeEnum TypeReport { get; set; }

	public BoYesNoEnum ThresholdOverlook { get; set; }

	public string UnpaidBillofExchange { get; set; }

	public BoYesNoEnum Valid { get; set; }

	public DateTime? ValidFrom { get; set; }

	public string ValidRemarks { get; set; }

	public DateTime? ValidTo { get; set; }

	public string VatGroup { get; set; }

	public string VatGroupName { get; set; }

	public string VatGroupLatinAmerica { get; set; }

	public string VatIDNum { get; set; }

	public string VatLiable { get; set; }

	public string VATRegistrationNumber { get; set; }

	public string VerificationNumber { get; set; }

	public string Website { get; set; }

	public BoYesNoEnum WithholdingTaxCertified { get; set; }

	public int WithholdingTaxDeductionGroup { get; set; }

	public string WTCode { get; set; }

	public string ZipCode { get; set; }

	public bool ValidFor { get; set; }

	public string ContactPersonCode { get; set; }

	public string Requester { get; set; }

	public string ReqName { get; set; }

	public string AcctCode { get; set; }

	public string AcctName { get; set; }

	public int ListNum { get; set; }

	public string ListName { get; set; }
}
