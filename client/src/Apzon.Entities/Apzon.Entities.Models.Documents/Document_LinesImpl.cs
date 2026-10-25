using System;
using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;
using SAPbobsCOM;

namespace Apzon.Entities.Models.Documents;

public class Document_LinesImpl
{
	public string SubCatNum { get; set; }

	public string AcctCode { get; set; }

	public int ActBaseEnt { get; set; }

	public int ActualBaseLine { get; set; }

	public DateTime ActDelDate { get; set; }

	public string Address { get; set; }

	public int AgrNo { get; set; }

	public int AgrLnNum { get; set; }

	public double AppliedTax { get; set; }

	public double AppliedTaxFC { get; set; }

	public double AppliedTaxSC { get; set; }

	public bool BackOrder { get; set; }

	public string CodeBars { get; set; }

	public int BaseEntry { get; set; }

	public int BaseLine { get; set; }

	public double BaseOpenQuantity { get; set; }

	public int BaseType { get; set; }

	public string FromWhsCod { get; set; }

	public List<BatchNumbersImpl> BatchNumbers { get; set; }

	public List<DocumentLinesBinAllocationsImpl> BinAllocations { get; set; }

	public string CFOPCode { get; set; }

	public string CogsAcct { get; set; }

	public string CogsOcrCod { get; set; }

	public string CogsOcrCo2 { get; set; }

	public string CogsOcrCo3 { get; set; }

	public string CogsOcrCo4 { get; set; }

	public string CogsOcrCo5 { get; set; }

	public string CSTCode { get; set; }

	public string CSTforCOFINS { get; set; }

	public string CSTforIPI { get; set; }

	public string CSTforPIS { get; set; }

	public string ChangeAssemlyBoMWarehouse { get; set; }

	public double Commission { get; set; }

	public BoYesNoEnum ConsiderQuantity { get; set; }

	public bool ConsumerSalesForecast { get; set; }

	public double CorrInvAmountToDiffAcct { get; set; }

	public double CorrInvAmountToStock { get; set; }

	public BoCorInvItemStatus CorrectionInvoiceItem { get; set; }

	public string CostingCode { get; set; }

	public string CostingCode2 { get; set; }

	public string CostingCode3 { get; set; }

	public string CostingCode4 { get; set; }

	public string CostingCode5 { get; set; }

	public int Count { get; set; }

	public string CountryOrg { get; set; }

	public string CreditOriginCode { get; set; }

	public string Currency { get; set; }

	public double DefectAndBreakup { get; set; }

	public bool DeferredTax { get; set; }

	public double DiscPrcnt { get; set; }

	public double DiscSum { get; set; }

	public bool DistributeExpense { get; set; }

	public int DocEntry { get; set; }

	public BoYesNoEnum EnableReturnCost { get; set; }

	public double EqualizationTaxPercent { get; set; }

	public string ExLineNo { get; set; }

	public double ExciseAmt { get; set; }

	public List<Document_LinesAdditionalExpensesImpl> Expenses { get; set; }

	public ExportProcessesImpl ExportProcesses { get; set; }

	public double Factor1 { get; set; }

	public double Factor2 { get; set; }

	public double Factor3 { get; set; }

	public double Factor4 { get; set; }

	public string FreeText { get; set; }

	public int GrossBase { get; set; }

	public double GrossBuyPrice { get; set; }

	public double GrossProfitTotalBasePrice { get; set; }

	public double Height1 { get; set; }

	public double Height2 { get; set; }

	public int Height2Unit { get; set; }

	public int Hight1Unit { get; set; }

	public ImportProcessesImpl ImportProcesses { get; set; }

	public int Incoterms { get; set; }

	public double InvQty { get; set; }

	public string ItemCode { get; set; }

	public string Dscription { get; set; }

	public string Text { get; set; }

	public int ItemType { get; set; }

	public double LastBuyDistributeSum { get; set; }

	public double LastBuyDistributeSumFc { get; set; }

	public double LastBuyDistributeSumSc { get; set; }

	public double LastBuyInmPrice { get; set; }

	public double Lengh1 { get; set; }

	public int Lengh1Unit { get; set; }

	public double Lengh2 { get; set; }

	public int Lengh2Unit { get; set; }

	public int LineNum { get; set; }

	public string LineStatus { get; set; }

	public double LineTotal { get; set; }

	public double GTotal { get; set; }

	public string LineType { get; set; }

	public string LineVendor { get; set; }

	public int LocCode { get; set; }

	public string unitMsr { get; set; }

	public double NetTaxAmount { get; set; }

	public double NetTaxAmountFC { get; set; }

	public double NetTaxAmountSC { get; set; }

	public double OpenAmount { get; set; }

	public double OpenAmountFC { get; set; }

	public double OpenAmountSC { get; set; }

	public string OriginalItem { get; set; }

	public string POTargetEntry { get; set; }

	public int POTargetNum { get; set; }

	public string POTargetRowNum { get; set; }

	public double PackQty { get; set; }

	public int ParentLineNum { get; set; }

	public BoYesNoEnum PartialRetirement { get; set; }

	public int PickListIdNumber { get; set; }

	public double PickQuantity { get; set; }

	public string PickStatus { get; set; }

	public BoDocumentLinePickStatus PickStatusEx { get; set; }

	public double Price { get; set; }

	public double PriceAfVAT { get; set; }

	public string Project { get; set; }

	public double Quantity { get; set; }

	public double RemainingOpenInventoryQuantity { get; set; }

	public double RemainingOpenQuantity { get; set; }

	public DateTime PQTReqDate { get; set; }

	public double PQTReqQty { get; set; }

	public double RetirementAPC { get; set; }

	public double RetirementQuantity { get; set; }

	public double ReturnCost { get; set; }

	public double RowTotalFC { get; set; }

	public double RowTotalSC { get; set; }

	public string SWW { get; set; }

	public int SlpCode { get; set; }

	public string SerialNum { get; set; }

	public List<SerialNumbersImpl> SerialNumbers { get; set; }

	public DateTime ShipDate { get; set; }

	public string ShipToCode { get; set; }

	public string ShipToDesc { get; set; }

	public int TrnsCode { get; set; }

	public double Shortages { get; set; }

	public double StockDistributesum { get; set; }

	public double StockDistributesumForeign { get; set; }

	public double StockDistributesumSystem { get; set; }

	public double StockInmPrice { get; set; }

	public string SupplierCatNum { get; set; }

	public double Surpluses { get; set; }

	public double TaxBeforeDPM { get; set; }

	public double TaxBeforeDPMFC { get; set; }

	public double TaxBeforeDPMSC { get; set; }

	public string TaxCode { get; set; }

	public List<TaxJurisdictionsImpl> TaxJurisdictions { get; set; }

	public bool TaxLiable { get; set; }

	public string TaxOnly { get; set; }

	public double TaxPerUnit { get; set; }

	public double TaxPercentagePerRow { get; set; }

	public double TaxTotal { get; set; }

	public string TaxType { get; set; }

	public string ThirdParty { get; set; }

	public double TotalEqualizationTax { get; set; }

	public double TotalEqualizationTaxFC { get; set; }

	public double TotalEqualizationTaxSC { get; set; }

	public double TotalInclTax { get; set; }

	public string TranType { get; set; }

	public int TransportMode { get; set; }

	public string TreeType { get; set; }

	public double UnitPrice { get; set; }

	public double NumPerMsr { get; set; }

	public string UomCode { get; set; }

	public int UomEntry { get; set; }

	public string Usage { get; set; }

	public string UseBaseUn { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public string VatGroup { get; set; }

	public double LineVat { get; set; }

	public string VendorNum { get; set; }

	public int VisualOrder { get; set; }

	public double Volume { get; set; }

	public int VolumeUnit { get; set; }

	public string WTLiable { get; set; }

	public string WhsCode { get; set; }

	public double Weight1 { get; set; }

	public int Weight1Unit { get; set; }

	public double Weight2 { get; set; }

	public int Weight2Unit { get; set; }

	public double Width1 { get; set; }

	public int Width1Unit { get; set; }

	public double Width2 { get; set; }

	public int Width2Unit { get; set; }

	public List<WithholdingTaxLinesImpl> WithholdingTaxLines { get; set; }

	public string NoInvtryMv { get; set; }

	public string Remark { get; set; }

	public string FreeTxt { get; set; }

	public string U_Dept { get; set; }

	public string U_Reason { get; set; }

	public string U_DelTime { get; set; }

	public string U_Remark { get; set; }

	public string UoMName { get; set; }

	public double QuantityOnStock { get; set; }

	public string AlcCode { get; set; }

	public string OhType { get; set; }

	public decimal CostSum { get; set; }

	public decimal CostSumFC { get; set; }

	public decimal Factor { get; set; }

	public string CostType { get; set; }

	public string Locked { get; set; }

	public string LaCAllcAcc { get; set; }

	public decimal CostSumSC { get; set; }

	public string InCustCalc { get; set; }

	public string AlcName { get; set; }

	public int UserSign { get; set; }

	public string PDN1LineStatus { get; set; }

	public decimal FobValue { get; set; }

	public string StockEval { get; set; }

	public string CostOH { get; set; }

	public decimal PDN1LineTotal { get; set; }

	public decimal PriceAtWH { get; set; }

	public int OrigLine { get; set; }

	public string PriceFOB { get; set; }

	public int PriceList { get; set; }

	public int UnitCode { get; set; }

	public string UnitCodeString { get; set; }

	public int TrgetEntry { get; set; }

	public int TargetType { get; set; }

	public int BaseRef { get; set; }

	public decimal Custom { get; set; }

	public decimal CustomFC { get; set; }

	public decimal Cost { get; set; }

	public decimal CostFC { get; set; }

	public decimal PricAtWHFC { get; set; }

	public decimal TotalFrgn { get; set; }

	public decimal GTotalFC { get; set; }

	public int UnitCode1 { get; set; }

	public int UnitCode2 { get; set; }

	public string CardCode { get; set; }

	public string Reference { get; set; }

	public decimal FactNoCust { get; set; }

	public decimal FacWthCust { get; set; }

	public string BlockNum { get; set; }

	public string ImportLog { get; set; }

	public int OrigRow { get; set; }

	public string OrigWhs { get; set; }

	public int ReleaseNum { get; set; }

	public decimal VarCosts { get; set; }

	public decimal ConstCosts { get; set; }

	public decimal VarCostsFR { get; set; }

	public decimal CnstCostFR { get; set; }

	public decimal UserCustom { get; set; }

	public decimal UsrCusomFC { get; set; }

	public decimal FobValueFC { get; set; }

	public decimal TtlExpndLC { get; set; }

	public decimal TtlExpndFC { get; set; }

	public decimal TtlCustLC { get; set; }

	public decimal TtlCustFC { get; set; }

	public decimal TtlCostLC { get; set; }

	public decimal TtlCostFC { get; set; }

	public decimal TtlVolume { get; set; }

	public decimal TtlWeight { get; set; }

	public int BaseRowNum { get; set; }

	public decimal TtlCustSC { get; set; }

	public decimal TtlExpndSC { get; set; }

	public int GRPOAbsEnt { get; set; }

	public int GRPOLineNu { get; set; }

	public int TargetDoc { get; set; }

	public string FobValCurr { get; set; }

	public decimal FobnLaC { get; set; }

	public decimal FobnLaCFC { get; set; }

	public double Rate { get; set; }

	public double PriceBefDi { get; set; }

	public bool SpecifiedPriceBefDi { get; set; }

	public bool SpecifiedPriceAfterVat { get; set; }

	public string SpecPrice { get; set; }

	public decimal VatSumFrgn { get; set; }

	public int ObjType { get; set; }

	public string OcrCode { get; set; }

	public string OcrCode2 { get; set; }

	public string OcrCode3 { get; set; }

	public string OcrCode4 { get; set; }

	public string OcrCode5 { get; set; }

	public string BOMTreeType { get; set; }

	public decimal DelivrdQty { get; set; }

	public int DocEntry_F1 { get; set; }

	public int LineNum_F1 { get; set; }

	public int GroupNum_F1 { get; set; }

	public int ExpnsCode_F1 { get; set; }

	public double LineTotal_F1 { get; set; }

	public double TotalFrgn_F1 { get; set; }

	public double TotalSumSy_F1 { get; set; }

	public double PaidToDate_F1 { get; set; }

	public double PaidFC_F1 { get; set; }

	public double PaidSys_F1 { get; set; }

	public string ObjType_F1 { get; set; }

	public int LogInstanc_F1 { get; set; }

	public string TaxStatus_F1 { get; set; }

	public string VatGroup_F1 { get; set; }

	public double VatPrcnt_F1 { get; set; }

	public double VatSum_F1 { get; set; }

	public double VatSumFrgn_F1 { get; set; }

	public double VatSumSy_F1 { get; set; }

	public double DedVatSum_F1 { get; set; }

	public double DedVatSumF_F1 { get; set; }

	public string IsAcquistn_F1 { get; set; }

	public string TaxCode_F1 { get; set; }

	public string TaxType_F1 { get; set; }

	public double VatApplied_F1 { get; set; }

	public double VatAppldFC_F1 { get; set; }

	public double VatAppldSC_F1 { get; set; }

	public double EquVatSum_F1 { get; set; }

	public double EquVatSumF_F1 { get; set; }

	public double EquVatSumS_F1 { get; set; }

	public double lineVat_F1 { get; set; }

	public double lineVatlF_F1 { get; set; }

	public double lineVatS_F1 { get; set; }

	public string WtLiable_F1 { get; set; }

	public string BaseMethod_F1 { get; set; }

	public string Stock_F1 { get; set; }

	public string LstPchPrce_F1 { get; set; }

	public string AnalysRpt_F1 { get; set; }

	public int BaseGroup_F1 { get; set; }

	public string Status_F1 { get; set; }

	public int TrgGroup_F1 { get; set; }

	public int VisOrder_F1 { get; set; }

	public string FixCurr_F1 { get; set; }

	public double VatDscntPr_F1 { get; set; }

	public string OcrCode_F1 { get; set; }

	public string OcrCode2_F1 { get; set; }

	public string OcrCode3_F1 { get; set; }

	public string OcrCode4_F1 { get; set; }

	public string OcrCode5_F1 { get; set; }

	public string Project_F1 { get; set; }

	public string VatGrpSrc_F1 { get; set; }

	public double DrawnTotal_F1 { get; set; }

	public double DrawnFC_F1 { get; set; }

	public double DrawnSC_F1 { get; set; }

	public int DocEntry_F2 { get; set; }

	public int LineNum_F2 { get; set; }

	public int GroupNum_F2 { get; set; }

	public int ExpnsCode_F2 { get; set; }

	public double LineTotal_F2 { get; set; }

	public double TotalFrgn_F2 { get; set; }

	public double TotalSumSy_F2 { get; set; }

	public double PaidToDate_F2 { get; set; }

	public double PaidFC_F2 { get; set; }

	public double PaidSys_F2 { get; set; }

	public string ObjType_F2 { get; set; }

	public int LogInstanc_F2 { get; set; }

	public string TaxStatus_F2 { get; set; }

	public string VatGroup_F2 { get; set; }

	public double VatPrcnt_F2 { get; set; }

	public double VatSum_F2 { get; set; }

	public double VatSumFrgn_F2 { get; set; }

	public double VatSumSy_F2 { get; set; }

	public double DedVatSum_F2 { get; set; }

	public double DedVatSumF_F2 { get; set; }

	public string IsAcquistn_F2 { get; set; }

	public string TaxCode_F2 { get; set; }

	public string TaxType_F2 { get; set; }

	public double VatApplied_F2 { get; set; }

	public double VatAppldFC_F2 { get; set; }

	public double VatAppldSC_F2 { get; set; }

	public double EquVatSum_F2 { get; set; }

	public double EquVatSumF_F2 { get; set; }

	public double EquVatSumS_F2 { get; set; }

	public double lineVat_F2 { get; set; }

	public double lineVatlF_F2 { get; set; }

	public double lineVatS_F2 { get; set; }

	public string WtLiable_F2 { get; set; }

	public string BaseMethod_F2 { get; set; }

	public string Stock_F2 { get; set; }

	public string LstPchPrce_F2 { get; set; }

	public string AnalysRpt_F2 { get; set; }

	public int BaseGroup_F2 { get; set; }

	public string Status_F2 { get; set; }

	public int TrgGroup_F2 { get; set; }

	public int VisOrder_F2 { get; set; }

	public string FixCurr_F2 { get; set; }

	public double VatDscntPr_F2 { get; set; }

	public string OcrCode_F2 { get; set; }

	public string OcrCode2_F2 { get; set; }

	public string OcrCode3_F2 { get; set; }

	public string OcrCode4_F2 { get; set; }

	public string OcrCode5_F2 { get; set; }

	public string Project_F2 { get; set; }

	public string VatGrpSrc_F2 { get; set; }

	public double DrawnTotal_F2 { get; set; }

	public double DrawnFC_F2 { get; set; }

	public double DrawnSC_F2 { get; set; }

	public int DocEntry_F3 { get; set; }

	public int LineNum_F3 { get; set; }

	public int GroupNum_F3 { get; set; }

	public int ExpnsCode_F3 { get; set; }

	public double LineTotal_F3 { get; set; }

	public double TotalFrgn_F3 { get; set; }

	public double TotalSumSy_F3 { get; set; }

	public double PaidToDate_F3 { get; set; }

	public double PaidFC_F3 { get; set; }

	public double PaidSys_F3 { get; set; }

	public string ObjType_F3 { get; set; }

	public int LogInstanc_F3 { get; set; }

	public string TaxStatus_F3 { get; set; }

	public string VatGroup_F3 { get; set; }

	public double VatPrcnt_F3 { get; set; }

	public double VatSum_F3 { get; set; }

	public double VatSumFrgn_F3 { get; set; }

	public double VatSumSy_F3 { get; set; }

	public double DedVatSum_F3 { get; set; }

	public double DedVatSumF_F3 { get; set; }

	public string IsAcquistn_F3 { get; set; }

	public string TaxCode_F3 { get; set; }

	public string TaxType_F3 { get; set; }

	public double VatApplied_F3 { get; set; }

	public double VatAppldFC_F3 { get; set; }

	public double VatAppldSC_F3 { get; set; }

	public double EquVatSum_F3 { get; set; }

	public double EquVatSumF_F3 { get; set; }

	public double EquVatSumS_F3 { get; set; }

	public double lineVat_F3 { get; set; }

	public double lineVatlF_F3 { get; set; }

	public double lineVatS_F3 { get; set; }

	public string WtLiable_F3 { get; set; }

	public string BaseMethod_F3 { get; set; }

	public string Stock_F3 { get; set; }

	public string LstPchPrce_F3 { get; set; }

	public string AnalysRpt_F3 { get; set; }

	public int BaseGroup_F3 { get; set; }

	public string Status_F3 { get; set; }

	public int TrgGroup_F3 { get; set; }

	public int VisOrder_F3 { get; set; }

	public string FixCurr_F3 { get; set; }

	public double VatDscntPr_F3 { get; set; }

	public string OcrCode_F3 { get; set; }

	public string OcrCode2_F3 { get; set; }

	public string OcrCode3_F3 { get; set; }

	public string OcrCode4_F3 { get; set; }

	public string OcrCode5_F3 { get; set; }

	public string Project_F3 { get; set; }

	public string VatGrpSrc_F3 { get; set; }

	public double DrawnTotal_F3 { get; set; }

	public double DrawnFC_F3 { get; set; }

	public double DrawnSC_F3 { get; set; }

	public string EnSetCost { get; set; }

	public decimal RetCost { get; set; }

	public decimal RateBomSales { get; set; }

	public string IsByPrdct { get; set; }
}
