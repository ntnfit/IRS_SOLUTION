using System;

namespace Apzon.Entities.Models.Production_Addon;

public class ProductionPlanningImpl
{
	public string CardCode { get; set; }

	public string CardName { get; set; }

	public DateTime? FromDate { get; set; }

	public DateTime? ToDate { get; set; }

	public int MONo { get; set; }

	public string MOName { get; set; }

	public DateTime PostDate { get; set; }

	public DateTime DueDate { get; set; }

	public string BatchToIssue { get; set; }

	public string UoM { get; set; }

	public double Quantity { get; set; }

	public string Project { get; set; }

	public string OcrCode { get; set; }

	public string OcrCode2 { get; set; }

	public string OcrCode3 { get; set; }

	public string OcrCode4 { get; set; }

	public string OcrCode5 { get; set; }

	public string RoutCode { get; set; }

	public string Remark { get; set; }

	public string Status { get; set; }

	public string RouCode { get; set; }

	public double Width { get; set; }

	public double Length { get; set; }

	public string BatchSOFixed { get; set; }

	public string ItemCode { get; set; }

	public string ItemName { get; set; }

	public string BatchPO { get; set; }

	public int SONo { get; set; }

	public DateTime ShipDate { get; set; }

	public int OrderLine { get; set; }

	public string Whse { get; set; }

	public string Operation { get; set; }

	public string OpCode { get; set; }

	public int LineNum { get; set; }

	public int SaleOrder { get; set; }

	public DateTime DelDate { get; set; }

	public TimeSpan? DelTime { get; set; }

	public string BatchComplete { get; set; }

	public double ThickPlan { get; set; }

	public double WidthPlan { get; set; }

	public double LengthPlan { get; set; }

	public double ThickFinal { get; set; }

	public double WidthFinal { get; set; }

	public double LengthFinal { get; set; }

	public int LineSale { get; set; }

	public string DocEntrySO { get; set; }

	public string LineNumSO { get; set; }
}
