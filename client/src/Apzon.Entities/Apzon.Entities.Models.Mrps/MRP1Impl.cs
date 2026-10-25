using System;

namespace Apzon.Entities.Models.Mrps;

public class MRP1Impl
{
	public long Id { get; set; }

	public string ItemCode { get; set; }

	public string ItemName { get; set; }

	public double? Qty { get; set; }

	public double? ConfirmQty { get; set; }

	public DateTime? DueDate { get; set; }

	public string DocumentType { get; set; }

	public string Notes { get; set; }

	public int OMrpId { get; set; }

	public int Type { get; set; }

	public string TypeName { get; set; }

	public int? PONo { get; set; }

	public double? OB { get; set; }

	public string Line { get; set; }

	public int? LTime { get; set; }

	public double? MachineTime { get; set; }

	public double? NumMachine { get; set; }

	public DateTime? GomNgay { get; set; }

	public string ControlArea { get; set; }

	public string Comment1 { get; set; }
}
