using System;
using System.Collections.Generic;

namespace Apzon.Entities.Models.Production;

public class IssueForProductionLine_Impl_IGE1
{
	public int DocEntry { get; set; }

	public int LineNum { get; set; }

	public int TargetType { get; set; }

	public int TrgetEntry { get; set; }

	public string BaseRef { get; set; }

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

	public decimal DiscPrcnt { get; set; }

	public decimal LineTotal { get; set; }

	public decimal TotalFrgn { get; set; }

	public decimal OpenSum { get; set; }

	public decimal OpenSumFC { get; set; }

	public string WhsCode { get; set; }

	public int SlpCode { get; set; }

	public string AcctCode { get; set; }

	public decimal PriceBefDi { get; set; }

	public DateTime DocDate { get; set; }

	public string OcrCode { get; set; }

	public string Project { get; set; }

	public string CodeBars { get; set; }

	public decimal VatPrcnt { get; set; }

	public string VatGroup { get; set; }

	public decimal PriceAfVAT { get; set; }

	public int BaseDocNum { get; set; }

	public decimal VatSum { get; set; }

	public decimal VatSumFrgn { get; set; }

	public decimal VatSumSy { get; set; }

	public int FinncPriod { get; set; }

	public string ObjType { get; set; }

	public decimal GrssProfit { get; set; }

	public decimal GrssProfFC { get; set; }

	public string TaxCode { get; set; }

	public decimal BaseQty { get; set; }

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

	public string UomCode { get; set; }

	public DateTime PQTReqdate { get; set; }

	public string U_Dept { get; set; }

	public string U_Reason { get; set; }

	public string U_Remark { get; set; }

	public string U_DelTime { get; set; }

	public decimal ItemCost { get; set; }

	public decimal PlannedQty { get; set; }

	public decimal CmpltQty { get; set; }

	public decimal Avail { get; set; }

	public List<ProductionOder_LineImpl_WOR1> ListProductionsOrderLine { get; set; }
}
