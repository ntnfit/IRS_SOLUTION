using System;

namespace Apzon.Entities.Models.Production_Addon;

public class ProductionControlImpl
{
	public int SONo { get; set; }

	public int LineSO { get; set; }

	public string ItemCode { get; set; }

	public string ItemName { get; set; }

	public string BatchSO { get; set; }

	public decimal? WidthSO { get; set; }

	public decimal? LengthSO { get; set; }

	public decimal? QtySO { get; set; }

	public string CardCode { get; set; }

	public string CardName { get; set; }

	public DateTime? FromDate { get; set; }

	public DateTime? ToDate { get; set; }

	public DateTime? ShipDate { get; set; }

	public string RemarkSO { get; set; }

	public decimal? WidthBatch { get; set; }

	public decimal? LengthBatch { get; set; }

	public string BatchPO { get; set; }

	public string RouCode { get; set; }

	public DateTime? DueDate { get; set; }

	public decimal? QtyPO { get; set; }

	public decimal? QtyBatch { get; set; }

	public decimal? OpenQty { get; set; }

	public string Remark { get; set; }

	public string Status { get; set; }

	public int? PONo { get; set; }

	public string BatchSOFixed { get; set; }

	public string DelRemark { get; set; }
}
