using System;

namespace Apzon.Entities.Models.PlanningProduction;

public class Prs2
{
	public long Id { get; set; }

	public string Item { get; set; }

	public int? Type { get; set; }

	public DateTime? WorkDay { get; set; }

	public double? Amount { get; set; }

	public string Shift { get; set; }

	public string Notes { get; set; }

	public string PSCode { get; set; }
}
