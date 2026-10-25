using System;

namespace Apzon.Entities.Models.PlanningProduction;

public class Mrp1
{
	public long Id { get; set; }

	public string ItemCode { get; set; }

	public string ItemName { get; set; }

	public decimal? Qty { get; set; }

	public decimal? ConfirmQty { get; set; }

	public DateTime? DueDate { get; set; }

	public string DocumentType { get; set; }

	public string Notes { get; set; }

	public int? OMrpId { get; set; }

	public int? Type { get; set; }

	public int? PONo { get; set; }

	public decimal? OB { get; set; }

	public string Line { get; set; }

	public int? LTime { get; set; }

	public double? MachineTime { get; set; }

	public double? NumMachine { get; set; }

	public DateTime? GomNgay { get; set; }
}
