using System;

namespace Apzon.Entities.Models.ARInvoice;

public class ARInvoicesImpl
{
	public int DocEntry { get; set; }

	public int DocNum { get; set; }

	public string DocType { get; set; }

	public string CANCELED { get; set; }

	public string Handwrtten { get; set; }

	public string Printed { get; set; }

	public string DocStatus { get; set; }

	public string ObjType { get; set; }

	public DateTime DocDate { get; set; }

	public DateTime DocDueDate { get; set; }

	public string CardCode { get; set; }

	public string CardName { get; set; }

	public string Address { get; set; }

	public string NumAtCard { get; set; }

	public decimal VatSum { get; set; }

	public decimal VatSumFC { get; set; }

	public decimal DiscPrcnt { get; set; }

	public decimal DiscSum { get; set; }

	public decimal DiscSumFC { get; set; }

	public string DocCur { get; set; }

	public decimal DocRate { get; set; }

	public decimal DocTotal { get; set; }

	public decimal DocTotalFC { get; set; }

	public decimal PaidToDate { get; set; }

	public decimal PaidFC { get; set; }

	public decimal GrosProfit { get; set; }

	public decimal GrosProfFC { get; set; }

	public string Ref1 { get; set; }

	public string Ref2 { get; set; }

	public string Comments { get; set; }

	public string JrnlMemo { get; set; }

	public int TransId { get; set; }

	public int ReceiptNum { get; set; }

	public int GroupNum { get; set; }

	public TimeSpan DocTime { get; set; }

	public int SlpCode { get; set; }

	public int TrnspCode { get; set; }

	public int CntctCode { get; set; }

	public decimal VatSumSy { get; set; }

	public decimal DiscSumSy { get; set; }

	public decimal DocTotalSy { get; set; }

	public decimal PaidSys { get; set; }

	public DateTime UpdateDate { get; set; }

	public string IsICT { get; set; }

	public DateTime CreateDate { get; set; }

	public int Series { get; set; }

	public DateTime TaxDate { get; set; }

	public int FinncPriod { get; set; }

	public int UserSign { get; set; }

	public int UserSign2 { get; set; }

	public DateTime VatDate { get; set; }

	public decimal PaidSum { get; set; }

	public decimal PaidSumFc { get; set; }

	public decimal PaidSumSc { get; set; }

	public string L_Code { get; set; }

	public string L_Name { get; set; }

	public decimal Cashremainder { get; set; }

	public string IsRIV { get; set; }

	public string CurSource { get; set; }

	public string TypeDisc { get; set; }

	public string ShipToCode { get; set; }

	public string PayToCode { get; set; }

	public string Confirmed { get; set; }

	public string Rounding { get; set; }

	public string Address2 { get; set; }

	public int LineNum { get; set; }

	public int TargetType { get; set; }

	public int TrgetEntry { get; set; }

	public int BaseRef { get; set; }

	public int BaseType { get; set; }

	public int BaseEntry { get; set; }

	public int BaseLine { get; set; }

	public string LineStatus { get; set; }

	public string ItemCode { get; set; }

	public string Dscription { get; set; }

	public decimal Quantity { get; set; }

	public decimal OpenQty { get; set; }

	public decimal Price { get; set; }

	public string Currency { get; set; }

	public decimal Rate { get; set; }

	public decimal LineTotal { get; set; }

	public decimal TotalFrgn { get; set; }

	public decimal OpenSum { get; set; }

	public decimal OpenSumFC { get; set; }

	public string WhsCode { get; set; }

	public string AcctCode { get; set; }

	public decimal PriceBefDi { get; set; }

	public string OcrCode { get; set; }

	public string Project { get; set; }

	public string CodeBars { get; set; }

	public string VatPrcnt { get; set; }

	public string VatGroup { get; set; }

	public decimal PriceAfVAT { get; set; }

	public int BaseDocNum { get; set; }

	public decimal VatSumFrgn { get; set; }

	public decimal GrssProfit { get; set; }

	public decimal GrssProfFC { get; set; }

	public string TaxCode { get; set; }

	public string BaseQty { get; set; }

	public decimal BaseOpnQty { get; set; }

	public decimal VatDscntPr { get; set; }

	public decimal GTotal { get; set; }

	public decimal GTotalFC { get; set; }

	public string OcrCode2 { get; set; }

	public string OcrCode3 { get; set; }

	public string OcrCode4 { get; set; }

	public string OcrCode5 { get; set; }

	public string CogsOcrCo2 { get; set; }

	public string CogsOcrCo3 { get; set; }

	public string CogsOcrCo4 { get; set; }

	public string CogsOcrCo5 { get; set; }

	public decimal Factor1 { get; set; }

	public decimal Factor2 { get; set; }

	public decimal Factor3 { get; set; }

	public decimal Factor4 { get; set; }

	public string UseBaseUn { get; set; }

	public string unitMsr { get; set; }

	public decimal NumPerMsr { get; set; }

	public DateTime ShipDate { get; set; }

	public string FreeTxt { get; set; }

	public string UoMCode { get; set; }

	public decimal LineVat { get; set; }

	public string LinManClsd { get; set; }
}
