using System;

namespace Apzon.Entities.Models.Return;

public class RDR1Impl
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

	public double Quantity { get; set; }

	public double OpenQty { get; set; }

	public double Price { get; set; }

	public string Currency { get; set; }

	public double Rate { get; set; }

	public double DiscPrcnt { get; set; }

	public double LineTotal { get; set; }

	public double TotalFrgn { get; set; }

	public double OpenSum { get; set; }

	public double OpenSumFC { get; set; }

	public string WhsCode { get; set; }

	public int SlpCode { get; set; }

	public string AcctCode { get; set; }

	public double PriceBefDi { get; set; }

	public DateTime DocDate { get; set; }

	public string OcrCode { get; set; }

	public string Project { get; set; }

	public string CodeBars { get; set; }

	public double VatPrcnt { get; set; }

	public string VatGroup { get; set; }

	public double PriceAfVAT { get; set; }

	public int BaseDocNum { get; set; }

	public double VatSum { get; set; }

	public double VatSumFrgn { get; set; }

	public double VatSumSy { get; set; }

	public int FinncPriod { get; set; }

	public string ObjType { get; set; }

	public double GrssProfit { get; set; }

	public double GrssProfFC { get; set; }

	public string TaxCode { get; set; }

	public double BaseQty { get; set; }

	public double BaseOpnQty { get; set; }

	public double VatDscntPr { get; set; }

	public double GTotal { get; set; }

	public double GTotalFC { get; set; }

	public string OcrCode2 { get; set; }

	public string OcrCode3 { get; set; }

	public string OcrCode4 { get; set; }

	public string OcrCode5 { get; set; }

	public double Factor1 { get; set; }

	public double Factor2 { get; set; }

	public double Factor3 { get; set; }

	public double Factor4 { get; set; }

	public string UseBaseUn { get; set; }

	public string unitMsr { get; set; }

	public double NumPerMsr { get; set; }

	public DateTime ShipDate { get; set; }

	public string FreeTxt { get; set; }

	public double SumLineTotalRate { get; set; }
}
