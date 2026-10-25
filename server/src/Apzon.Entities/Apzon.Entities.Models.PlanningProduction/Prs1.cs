using System;

namespace Apzon.Entities.Models.PlanningProduction;

public class Prs1
{
	public long Id { get; set; }

	public string Item { get; set; }

	public int? Type { get; set; }

	public DateTime? WorkDay { get; set; }

	public double? Qty { get; set; }

	public string Notes { get; set; }

	public string Shift { get; set; }

	public string PSCode { get; set; }

	public int? ProductionOrder { get; set; }

	public string Line { get; set; }

	public double? MachineTime { get; set; }

	public double? NumMachine { get; set; }
}
