using System;

namespace Apzon.Entities.Models.Inventory;

public class BatchTransactionImpl
{
	public string ItemCode { get; set; }

	public int SysNumber { get; set; }

	public string DistNumber { get; set; }

	public string MnfSerial { get; set; }

	public string LotNumber { get; set; }

	public DateTime? ExpDate { get; set; }

	public DateTime? MnfDate { get; set; }

	public DateTime? InDate { get; set; }

	public DateTime? GrntStart { get; set; }

	public DateTime? GrntExp { get; set; }

	public string Status { get; set; }

	public string Notes { get; set; }

	public string Transfered { get; set; }

	public int AbsEntry { get; set; }

	public string ObjType { get; set; }

	public string itemName { get; set; }

	public int? LogInstanc { get; set; }

	public short? UserSign2 { get; set; }

	public DateTime? UpdateDate { get; set; }

	public decimal? CostTotal { get; set; }

	public decimal? Quantity { get; set; }

	public decimal? QuantOut { get; set; }

	public int? TrackingNt { get; set; }

	public int? TrackiNtLn { get; set; }

	public int? TransNum { get; set; }

	public int? TransType { get; set; }

	public int? CreatedBy { get; set; }

	public string BASE_REF { get; set; }

	public int? DocLineNum { get; set; }

	public DateTime? DocDate { get; set; }

	public DateTime? DocDueDate { get; set; }

	public string CardCode { get; set; }

	public string CardName { get; set; }

	public string Ref1 { get; set; }

	public string Ref2 { get; set; }

	public string Comments { get; set; }

	public string JrnlMemo { get; set; }

	public short? DocTime { get; set; }

	public string Dscription { get; set; }

	public decimal? InQty { get; set; }

	public decimal? OutQty { get; set; }

	public decimal? Price { get; set; }

	public string Currency { get; set; }

	public decimal? Rate { get; set; }

	public string VendorNum { get; set; }

	public string SerialNum { get; set; }

	public string Warehouse { get; set; }

	public string TreeType { get; set; }

	public int? SlpCode { get; set; }

	public DateTime? TaxDate { get; set; }

	public string DataSource { get; set; }

	public string PrjCode { get; set; }

	public short? UserSign { get; set; }

	public string BlockNum { get; set; }

	public string ImportLog { get; set; }

	public string UseDocPric { get; set; }

	public int? Location { get; set; }

	public decimal? CalcPrice { get; set; }

	public decimal? OpenQty { get; set; }

	public int? Instance { get; set; }

	public int LastInst { get; set; }

	public decimal? RevalTotal { get; set; }

	public string BaseCurr { get; set; }

	public int? ApplObj { get; set; }

	public int? AppObjAbs { get; set; }

	public string AppObjType { get; set; }

	public string StockAct { get; set; }

	public string TrnsfrAct { get; set; }

	public string PriceDifAc { get; set; }

	public string VarianceAc { get; set; }

	public string ReturnAct { get; set; }

	public string ExcRateAct { get; set; }

	public string ClearAct { get; set; }

	public string CostAct { get; set; }

	public string WipAct { get; set; }

	public decimal? Balance { get; set; }

	public DateTime? CreateDate { get; set; }

	public int? BaseLine { get; set; }

	public decimal? TransValue { get; set; }

	public decimal? PriceDiff { get; set; }

	public int TransSeq { get; set; }

	public string InvntAct { get; set; }

	public string RemMethod { get; set; }

	public decimal? OpenValue { get; set; }

	public int? SubLineNum { get; set; }

	public int? AppObjLine { get; set; }

	public decimal? Expenses { get; set; }

	public decimal? OpenExp { get; set; }

	public decimal? Allocation { get; set; }

	public decimal? OpenAlloc { get; set; }

	public decimal? ExpAlloc { get; set; }

	public decimal? OExpAlloc { get; set; }

	public decimal? OpenPDiff { get; set; }

	public decimal? ExchDiff { get; set; }

	public decimal? OpenEDiff { get; set; }

	public decimal? NegInvAdjs { get; set; }

	public decimal? OpenNegInv { get; set; }

	public string NegStckAct { get; set; }

	public int BTransVal { get; set; }

	public decimal? VarVal { get; set; }

	public decimal? BExpVal { get; set; }

	public decimal? CogsVal { get; set; }

	public decimal? BNegAVal { get; set; }

	public string IOffIncAcc { get; set; }

	public decimal? IOffIncVal { get; set; }

	public string DOffDecAcc { get; set; }

	public decimal? DOffDecVal { get; set; }

	public string DecAcc { get; set; }

	public decimal? DecVal { get; set; }

	public decimal? WipVal { get; set; }

	public string WipVarAcc { get; set; }

	public decimal? WipVarVal { get; set; }

	public string IncAct { get; set; }

	public decimal? IncVal { get; set; }

	public string ExpCAcc { get; set; }

	public string CostMethod { get; set; }

	public string OcrCode { get; set; }

	public int BaseQty { get; set; }

	public int PrevTrans { get; set; }

	public int HTransSeq { get; set; }

	public string OcrCode2 { get; set; }

	public string OcrCode3 { get; set; }

	public string OcrCode4 { get; set; }

	public string OcrCode5 { get; set; }
}
