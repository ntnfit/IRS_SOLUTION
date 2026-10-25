using System;
using System.Collections.Generic;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.MasterData;

public class ItemsImpl
{
	public int AtcEntry { get; set; }

	public string OldItemCode { get; set; }

	public string ItemCode { get; set; }

	public string ItemName { get; set; }

	public double LstEvlPric { get; set; }

	public DateTime? LstEvlDate { get; set; }

	public string Canceled { get; set; }

	public int UserSign { get; set; }

	public double LastPurPrc { get; set; }

	public DateTime? LastPurDat { get; set; }

	public DateTime? LstSalDate { get; set; }

	public DateTime? CreateDate { get; set; }

	public DateTime? UpdateDate { get; set; }

	public string ObjType { get; set; }

	public int DocEntry { get; set; }

	public int ShipType { get; set; }

	public double StockValue { get; set; }

	public int UserSign2 { get; set; }

	public string OneBOneRec { get; set; }

	public string IncomeAcct { get; set; }

	public string ExmptIncom { get; set; }

	public string CustomPer { get; set; }

	public string MnufctTime { get; set; }

	public string WholSlsTax { get; set; }

	public string RetilrTax { get; set; }

	public string SpcialDisc { get; set; }

	public string DscountCod { get; set; }

	public string TrackSales { get; set; }

	public string Consig { get; set; }

	public string QueryGroup { get; set; }

	public string Counted { get; set; }

	public string OpenBlnc { get; set; }

	public string FREE { get; set; }

	public string Transfered { get; set; }

	public string BlncTrnsfr { get; set; }

	public string TreeQty { get; set; }

	public string LastPurCur { get; set; }

	public string ExitCur { get; set; }

	public string ExitPrice { get; set; }

	public string ExitWH { get; set; }

	public string WasCounted { get; set; }

	public string FixCurrCms { get; set; }

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

	public string SalFormula { get; set; }

	public string PurFormula { get; set; }

	public string SCNCounter { get; set; }

	public string DataSource { get; set; }

	public string LogInstanc { get; set; }

	public string Deleted { get; set; }

	public string ExpensAcct { get; set; }

	public string FREE1 { get; set; }

	public string PricingPrc { get; set; }

	public string ReorderPnt { get; set; }

	public string ServiceCtg { get; set; }

	public string NotifyASN { get; set; }

	public string ProAssNum { get; set; }

	public string AssblValue { get; set; }

	public string Number { get; set; }

	public string Attachment { get; set; }

	public string Employee { get; set; }

	public string AcqDate { get; set; }

	public string RetDate { get; set; }

	public string AssetRmk1 { get; set; }

	public string AssetRmk2 { get; set; }

	public string AssetAmnt1 { get; set; }

	public string AssetAmnt2 { get; set; }

	public string RuleCode { get; set; }

	public string CompoWH { get; set; }

	public string VirtAstItm { get; set; }

	public string SouVirAsst { get; set; }

	public string InCostRoll { get; set; }

	public string PrdStdCst { get; set; }

	public string EnAstSeri { get; set; }

	public string TaxCodeAP { get; set; }

	public string TaxCodeAR { get; set; }

	public string AssetClass { get; set; }

	public string AssetGroup { get; set; }

	public string AssetItem { get; set; }

	public string AssetSerNo { get; set; }

	public string AsstStatus { get; set; }

	public string CodeBars { get; set; }

	public string BaseUnit { get; set; }

	public int BeverTM { get; set; }

	public string BeverGrpC { get; set; }

	public string BeverTblC { get; set; }

	public DateTime? CapDate { get; set; }

	public string Cession { get; set; }

	public long ChapterID { get; set; }

	public long CommisGrp { get; set; }

	public double CommisPcnt { get; set; }

	public double CommisSum { get; set; }

	public string EvalSystem { get; set; }

	public double NumInCnt { get; set; }

	public int CstGrpCode { get; set; }

	public int DNFEntry { get; set; }

	public string ExportCode { get; set; }

	public string DeacAftUL { get; set; }

	public string CntUnitMsr { get; set; }

	public int INUoMEntry { get; set; }

	public int PUoMEntry { get; set; }

	public int SUoMEntry { get; set; }

	public string DfltWH { get; set; }

	public string DeprGroup { get; set; }

	public double ReorderQty { get; set; }

	public string ECExpAcc { get; set; }

	public string ECInAcct { get; set; }

	public int Technician { get; set; }

	public string Excisable { get; set; }

	public double SalPackUn { get; set; }

	public string BlockOut { get; set; }

	public string FrgnExpAcc { get; set; }

	public string FrgnName { get; set; }

	public string FrgnInAcct { get; set; }

	public string frozenFor { get; set; }

	public DateTime? frozenFrom { get; set; }

	public string FrozenComm { get; set; }

	public DateTime? frozenTo { get; set; }

	public int FuelCode { get; set; }

	public string GLMethod { get; set; }

	public string Spec { get; set; }

	public string TaxCtg { get; set; }

	public int ISvcCode { get; set; }

	public string IndirctTax { get; set; }

	public string InvntItem { get; set; }

	public string InventryNo { get; set; }

	public string InvntryUom { get; set; }

	public int IUoMEntry { get; set; }

	public double IWeight1 { get; set; }

	public double IWeight2 { get; set; }

	public int IWght1Unit { get; set; }

	public int IWght2Unit { get; set; }

	public string Phantom { get; set; }

	public string IssueMthd { get; set; }

	public int IssuePriBy { get; set; }

	public string ItemClass { get; set; }

	public string CountryOrg { get; set; }

	public string ItemType { get; set; }

	public int ItmsGrpCod { get; set; }

	public int LeadTime { get; set; }

	public int Location { get; set; }

	public string CardCode { get; set; }

	public string ManBtchNum { get; set; }

	public string MgrByQty { get; set; }

	public string ManSerNum { get; set; }

	public string ManOutOnly { get; set; }

	public string ByWh { get; set; }

	public int FirmCode { get; set; }

	public int MatGrp { get; set; }

	public string MatType { get; set; }

	public double MaxLevel { get; set; }

	public double MinLevel { get; set; }

	public double MinOrdrQty { get; set; }

	public double AvgPrice { get; set; }

	public int NCMCode { get; set; }

	public string NoDiscount { get; set; }

	public int OrdrIntrvl { get; set; }

	public double OrdrMulti { get; set; }

	public int OSvcCode { get; set; }

	public string PicturName { get; set; }

	public string PlaningSys { get; set; }

	public string PrcrmntMtd { get; set; }

	public int ProductSrc { get; set; }

	public double PurFactor1 { get; set; }

	public double PurFactor2 { get; set; }

	public double PurFactor3 { get; set; }

	public double PurFactor4 { get; set; }

	public int BHght1Unit { get; set; }

	public int BHght2Unit { get; set; }

	public string PrchseItem { get; set; }

	public double NumInBuy { get; set; }

	public int BLen1Unit { get; set; }

	public int BLen2Unit { get; set; }

	public string PurPackMsr { get; set; }

	public double PurPackUn { get; set; }

	public string BuyUnitMsr { get; set; }

	public double BHeight1 { get; set; }

	public double BHeight2 { get; set; }

	public double BLength1 { get; set; }

	public double Blength2 { get; set; }

	public double BVolume { get; set; }

	public double BWeight1 { get; set; }

	public double BWeight2 { get; set; }

	public double BWidth1 { get; set; }

	public double BWidth2 { get; set; }

	public string VatGroupPu { get; set; }

	public int BVolUnit { get; set; }

	public int BWght1Unit { get; set; }

	public int BWght2Unit { get; set; }

	public int BWdth1Unit { get; set; }

	public int BWdth2Unit { get; set; }

	public double OnHand { get; set; }

	public double IsCommited { get; set; }

	public double OnOrder { get; set; }

	public string MngMethod { get; set; }

	public string SWW { get; set; }

	public double SalFactor1 { get; set; }

	public double SalFactor2 { get; set; }

	public double SalFactor3 { get; set; }

	public double SalFactor4 { get; set; }

	public int SHght1Unit { get; set; }

	public int SHght2Unit { get; set; }

	public string SellItem { get; set; }

	public double NumInSale { get; set; }

	public int SLen1Unit { get; set; }

	public int SLen2Unit { get; set; }

	public string SalPackMsr { get; set; }

	public string SalUnitMsr { get; set; }

	public double SHeight1 { get; set; }

	public double SHeight2 { get; set; }

	public double SLength1 { get; set; }

	public double Slength2 { get; set; }

	public double SVolume { get; set; }

	public double SWeight1 { get; set; }

	public double SWeight2 { get; set; }

	public double SWidth1 { get; set; }

	public double SWidth2 { get; set; }

	public string VatGourpSa { get; set; }

	public int SVolUnit { get; set; }

	public int SWght1Unit { get; set; }

	public int SWght2Unit { get; set; }

	public int SWdth1Unit { get; set; }

	public int SWdth2Unit { get; set; }

	public string ScsCode { get; set; }

	public string SerialNum { get; set; }

	public int Series { get; set; }

	public int ServiceGrp { get; set; }

	public string SpProdType { get; set; }

	public string StatAsset { get; set; }

	public string SuppCatNum { get; set; }

	public string TaxType { get; set; }

	public int ToleranDay { get; set; }

	public string TreeType { get; set; }

	public string GLPickMeth { get; set; }

	public int UgpEntry { get; set; }

	public string UserText { get; set; }

	public string validFor { get; set; }

	public DateTime? validFrom { get; set; }

	public string ValidComm { get; set; }

	public DateTime? validTo { get; set; }

	public string VATLiable { get; set; }

	public string WTLiable { get; set; }

	public string WarrntTmpl { get; set; }

	public List<AttachmentFile_Impl> AttachmentFiles { get; set; }

	public List<string> ItemProperties { get; set; }

	public Items_LineImpl Lines { get; set; }

	public List<Items_LineImpl> ListITM1 { get; set; }

	public List<Items_UoMImpl> ListITM12 { get; set; }

	public List<OITW_Impl> OitwLines { get; set; }

	public List<UserFieldsImpl> ListUserFields { get; set; }

	public List<ItemPreferredVendorsImpl> ListItemPreferredVendors { get; set; }

	public string USER_CODE { get; set; }

	public decimal QuantityOnWareHouse { get; set; }

	public string APZ_SAVE_ACTION { get; set; }

	public int BPLId { get; set; }

	public string Currency { get; set; }

	public double Price { get; set; }

	public string WhseCode { get; set; }

	public string UoMCode { get; set; }
}
