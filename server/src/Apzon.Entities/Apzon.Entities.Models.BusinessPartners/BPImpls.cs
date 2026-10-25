using System;
using System.Collections.Generic;
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.BusinessPartners;

public class BPImpls
{
	public string CardCode { get; set; }

	public string CardName { get; set; }

	public string CardType { get; set; }

	public int GroupCode { get; set; }

	public string CmpPrivate { get; set; }

	public string Address { get; set; }

	public string ZipCode { get; set; }

	public string MailAddres { get; set; }

	public string MailZipCod { get; set; }

	public string Phone1 { get; set; }

	public string Phone2 { get; set; }

	public string Fax { get; set; }

	public string CntctPrsn { get; set; }

	public string Notes { get; set; }

	public double Balance { get; set; }

	public double ChecksBal { get; set; }

	public double DNotesBal { get; set; }

	public double OrdersBal { get; set; }

	public int GroupNum { get; set; }

	public double CreditLine { get; set; }

	public double DebtLine { get; set; }

	public double Discount { get; set; }

	public string VatStatus { get; set; }

	public string LicTradNum { get; set; }

	public string DdctStatus { get; set; }

	public double DdctPrcnt { get; set; }

	public DateTime? ValidUntil { get; set; }

	public int Chrctrstcs { get; set; }

	public int ExMatchNum { get; set; }

	public int InMatchNum { get; set; }

	public int ListNum { get; set; }

	public double DNoteBalFC { get; set; }

	public double OrderBalFC { get; set; }

	public double DNoteBalSy { get; set; }

	public double OrderBalSy { get; set; }

	public string Transfered { get; set; }

	public string BalTrnsfrd { get; set; }

	public double IntrstRate { get; set; }

	public double Commission { get; set; }

	public int CommGrCode { get; set; }

	public string Free_Text { get; set; }

	public int SlpCode { get; set; }

	public string PrevYearAc { get; set; }

	public string Currency { get; set; }

	public string RateDifAct { get; set; }

	public double BalanceSys { get; set; }

	public double BalanceFC { get; set; }

	public string Protected { get; set; }

	public string Cellular { get; set; }

	public int AvrageLate { get; set; }

	public string City { get; set; }

	public string County { get; set; }

	public string Country { get; set; }

	public string MailCity { get; set; }

	public string MailCounty { get; set; }

	public string MailCountr { get; set; }

	public string E_Mail { get; set; }

	public string Picture { get; set; }

	public string DflAccount { get; set; }

	public string DflBranch { get; set; }

	public string BankCode { get; set; }

	public string AddID { get; set; }

	public string Pager { get; set; }

	public string FatherCard { get; set; }

	public string CardFName { get; set; }

	public string FatherType { get; set; }

	public string QryGroup1 { get; set; }

	public string QryGroup2 { get; set; }

	public string QryGroup3 { get; set; }

	public string QryGroup4 { get; set; }

	public string QryGroup5 { get; set; }

	public string QryGroup6 { get; set; }

	public string QryGroup7 { get; set; }

	public string QryGroup8 { get; set; }

	public string QryGroup9 { get; set; }

	public string QryGroup10 { get; set; }

	public string QryGroup11 { get; set; }

	public string QryGroup12 { get; set; }

	public string QryGroup13 { get; set; }

	public string QryGroup14 { get; set; }

	public string QryGroup15 { get; set; }

	public string QryGroup16 { get; set; }

	public string QryGroup17 { get; set; }

	public string QryGroup18 { get; set; }

	public string QryGroup19 { get; set; }

	public string QryGroup20 { get; set; }

	public string QryGroup21 { get; set; }

	public string QryGroup22 { get; set; }

	public string QryGroup23 { get; set; }

	public string QryGroup24 { get; set; }

	public string QryGroup25 { get; set; }

	public string QryGroup26 { get; set; }

	public string QryGroup27 { get; set; }

	public string QryGroup28 { get; set; }

	public string QryGroup29 { get; set; }

	public string QryGroup30 { get; set; }

	public string QryGroup31 { get; set; }

	public string QryGroup32 { get; set; }

	public string QryGroup33 { get; set; }

	public string QryGroup34 { get; set; }

	public string QryGroup35 { get; set; }

	public string QryGroup36 { get; set; }

	public string QryGroup37 { get; set; }

	public string QryGroup38 { get; set; }

	public string QryGroup39 { get; set; }

	public string QryGroup40 { get; set; }

	public string QryGroup41 { get; set; }

	public string QryGroup42 { get; set; }

	public string QryGroup43 { get; set; }

	public string QryGroup44 { get; set; }

	public string QryGroup45 { get; set; }

	public string QryGroup46 { get; set; }

	public string QryGroup47 { get; set; }

	public string QryGroup48 { get; set; }

	public string QryGroup49 { get; set; }

	public string QryGroup50 { get; set; }

	public string QryGroup51 { get; set; }

	public string QryGroup52 { get; set; }

	public string QryGroup53 { get; set; }

	public string QryGroup54 { get; set; }

	public string QryGroup55 { get; set; }

	public string QryGroup56 { get; set; }

	public string QryGroup57 { get; set; }

	public string QryGroup58 { get; set; }

	public string QryGroup59 { get; set; }

	public string QryGroup60 { get; set; }

	public string QryGroup61 { get; set; }

	public string QryGroup62 { get; set; }

	public string QryGroup63 { get; set; }

	public string QryGroup64 { get; set; }

	public string DdctOffice { get; set; }

	public DateTime? CreateDate { get; set; }

	public DateTime? UpdateDate { get; set; }

	public string ExportCode { get; set; }

	public int DscntObjct { get; set; }

	public string DscntRel { get; set; }

	public int SPGCounter { get; set; }

	public int SPPCounter { get; set; }

	public string DdctFileNo { get; set; }

	public int SCNCounter { get; set; }

	public double MinIntrst { get; set; }

	public string DataSource { get; set; }

	public int OprCount { get; set; }

	public string ExemptNo { get; set; }

	public int Priority { get; set; }

	public int CreditCard { get; set; }

	public string CrCardNum { get; set; }

	public DateTime? CardValid { get; set; }

	public int UserSign { get; set; }

	public string LocMth { get; set; }

	public string validFor { get; set; }

	public DateTime? validFrom { get; set; }

	public DateTime? validTo { get; set; }

	public string frozenFor { get; set; }

	public DateTime? frozenFrom { get; set; }

	public DateTime? frozenTo { get; set; }

	public string sEmployed { get; set; }

	public int MTHCounter { get; set; }

	public int BNKCounter { get; set; }

	public int DdgKey { get; set; }

	public int DdtKey { get; set; }

	public string ValidComm { get; set; }

	public string FrozenComm { get; set; }

	public string chainStore { get; set; }

	public string DiscInRet { get; set; }

	public string State1 { get; set; }

	public string State2 { get; set; }

	public string VatGroup { get; set; }

	public int LogInstanc { get; set; }

	public string ObjType { get; set; }

	public string Indicator { get; set; }

	public int ShipType { get; set; }

	public string DebPayAcct { get; set; }

	public string ShipToDef { get; set; }

	public string Block { get; set; }

	public string MailBlock { get; set; }

	public string Password { get; set; }

	public string ECVatGroup { get; set; }

	public string Deleted { get; set; }

	public string IBAN { get; set; }

	public int DocEntry { get; set; }

	public int FormCode { get; set; }

	public string Box1099 { get; set; }

	public string PymCode { get; set; }

	public string BackOrder { get; set; }

	public string PartDelivr { get; set; }

	public int DunnLevel { get; set; }

	public DateTime? DunnDate { get; set; }

	public string BlockDunn { get; set; }

	public string BankCountr { get; set; }

	public string CollecAuth { get; set; }

	public string DME { get; set; }

	public string InstrucKey { get; set; }

	public string SinglePaym { get; set; }

	public string ISRBillId { get; set; }

	public string PaymBlock { get; set; }

	public string RefDetails { get; set; }

	public string HouseBank { get; set; }

	public string OwnerIdNum { get; set; }

	public int PyBlckDesc { get; set; }

	public string HousBnkCry { get; set; }

	public string HousBnkAct { get; set; }

	public string HousBnkBrn { get; set; }

	public string ProjectCod { get; set; }

	public int SysMatchNo { get; set; }

	public string VatIdUnCmp { get; set; }

	public string AgentCode { get; set; }

	public int TolrncDays { get; set; }

	public string SelfInvoic { get; set; }

	public string DeferrTax { get; set; }

	public string LetterNum { get; set; }

	public double MaxAmount { get; set; }

	public DateTime? FromDate { get; set; }

	public DateTime? ToDate { get; set; }

	public string WTLiable { get; set; }

	public string CrtfcateNO { get; set; }

	public DateTime? ExpireDate { get; set; }

	public string NINum { get; set; }

	public string AccCritria { get; set; }

	public string WTCode { get; set; }

	public string Equ { get; set; }

	public string HldCode { get; set; }

	public string ConnBP { get; set; }

	public int MltMthNum { get; set; }

	public string TypWTReprt { get; set; }

	public string VATRegNum { get; set; }

	public string RepName { get; set; }

	public string Industry { get; set; }

	public string Business { get; set; }

	public string WTTaxCat { get; set; }

	public string IsDomestic { get; set; }

	public string IsResident { get; set; }

	public string AutoCalBCG { get; set; }

	public string OtrCtlAcct { get; set; }

	public string AliasName { get; set; }

	public string Building { get; set; }

	public string MailBuildi { get; set; }

	public string BoEPrsnt { get; set; }

	public string BoEDiscnt { get; set; }

	public string BoEOnClct { get; set; }

	public string UnpaidBoE { get; set; }

	public string ITWTCode { get; set; }

	public string DunTerm { get; set; }

	public string ChannlBP { get; set; }

	public int DfTcnician { get; set; }

	public int Territory { get; set; }

	public string BillToDef { get; set; }

	public string DpmClear { get; set; }

	public string IntrntSite { get; set; }

	public int LangCode { get; set; }

	public int HousActKey { get; set; }

	public string Profession { get; set; }

	public int CDPNum { get; set; }

	public int DflBankKey { get; set; }

	public string BCACode { get; set; }

	public string UseShpdGd { get; set; }

	public string RegNum { get; set; }

	public string VerifNum { get; set; }

	public string BankCtlKey { get; set; }

	public string HousCtlKey { get; set; }

	public string AddrType { get; set; }

	public string InsurOp347 { get; set; }

	public string MailAddrTy { get; set; }

	public string StreetNo { get; set; }

	public string MailStrNo { get; set; }

	public string TaxRndRule { get; set; }

	public int VendTID { get; set; }

	public string ThreshOver { get; set; }

	public string SurOver { get; set; }

	public string VendorOcup { get; set; }

	public string OpCode347 { get; set; }

	public string DpmIntAct { get; set; }

	public string ResidenNum { get; set; }

	public int UserSign2 { get; set; }

	public string PlngGroup { get; set; }

	public string VatIDNum { get; set; }

	public string Affiliate { get; set; }

	public string MivzExpSts { get; set; }

	public string HierchDdct { get; set; }

	public string CertWHT { get; set; }

	public string CertBKeep { get; set; }

	public string WHShaamGrp { get; set; }

	public int IndustryC { get; set; }

	public int DatevAcct { get; set; }

	public string DatevFirst { get; set; }

	public string GTSRegNum { get; set; }

	public string GTSBankAct { get; set; }

	public string GTSBilAddr { get; set; }

	public string HsBnkSwift { get; set; }

	public string HsBnkIBAN { get; set; }

	public string DflSwift { get; set; }

	public string AutoPost { get; set; }

	public string IntrAcc { get; set; }

	public string FeeAcc { get; set; }

	public int CpnNo { get; set; }

	public int NTSWebSite { get; set; }

	public string DflIBAN { get; set; }

	public int Series { get; set; }

	public int Number { get; set; }

	public int EDocExpFrm { get; set; }

	public string TaxIdIdent { get; set; }

	public string Attachment { get; set; }

	public int AtcEntry { get; set; }

	public string DiscRel { get; set; }

	public string NoDiscount { get; set; }

	public string SCAdjust { get; set; }

	public int DflAgrmnt { get; set; }

	public string GlblLocNum { get; set; }

	public string SenderID { get; set; }

	public string RcpntID { get; set; }

	public int MainUsage { get; set; }

	public string SefazCheck { get; set; }

	public string free312 { get; set; }

	public string free313 { get; set; }

	public DateTime? DateFrom { get; set; }

	public DateTime? DateTill { get; set; }

	public string RelCode { get; set; }

	public string OKATO { get; set; }

	public string OKTMO { get; set; }

	public string KBKCode { get; set; }

	public string TypeOfOp { get; set; }

	public int OwnerCode { get; set; }

	public string MandateID { get; set; }

	public DateTime? SignDate { get; set; }

	public int Remark1 { get; set; }

	public string ConCerti { get; set; }

	public int TpCusPres { get; set; }

	public string RoleTypCod { get; set; }

	public string BlockComm { get; set; }

	public string EmplymntCt { get; set; }

	public string ExcptnlEvt { get; set; }

	public double ExpnPrfFnd { get; set; }

	public string EdrsFromBP { get; set; }

	public string EdrsToBP { get; set; }

	public string EDocGenTyp { get; set; }

	public string OldCardCode { get; set; }

	public string BankName { get; set; }

	public string AcctName { get; set; }

	public List<string> ItemProperties { get; set; }

	public List<AddressesImpl> Addresses { get; set; }

	public List<UserFieldsImpl> ListUserFields { get; set; }

	public List<ContactPersonImpl> ListContactPerson { get; set; }

	public List<AttachmentFile_Impl> AttachmentFiles { get; set; }

	public List<BankAccountsImpl> BPBankAccounts { get; set; }

	public List<PaymentDatesImpl> BPPaymentDates { get; set; }

	public List<BPAccountReceivablePaybleImpl> AccountRecivablePayables { get; set; }

	public List<BPBranchAssignmentImpls> BPBranchAssignment { get; set; }
}
