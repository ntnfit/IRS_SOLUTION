using System;

namespace Apzon.Entities.Models.Production_Addon;

public class RoutingImpl
{
	public string RouCode { get; set; }

	public string RouName { get; set; }

	public string Status { get; set; }

	public DateTime? validFrom { get; set; }

	public DateTime? validTo { get; set; }

	public string Type { get; set; }

	public string CardCode { get; set; }

	public decimal? LotSize { get; set; }

	public string Remark { get; set; }

	public string ModelCode { get; set; }

	public int LineNum { get; set; }

	public string OpCode { get; set; }

	public string KeyCode { get; set; }

	public decimal? Quantity { get; set; }

	public string UoM { get; set; }

	public string SubOpCode { get; set; }

	public string ResCode { get; set; }

	public string OpName { get; set; }

	public string BOMCode { get; set; }

	public decimal? SetupTime { get; set; }

	public string SetupUoM { get; set; }

	public decimal? MachineTime { get; set; }

	public string MachineUoM { get; set; }

	public decimal? LaborTime { get; set; }

	public string LaborUoM { get; set; }

	public string Comments { get; set; }

	public string PRT { get; set; }

	public string PrevOp { get; set; }

	public string NextOp { get; set; }
}
