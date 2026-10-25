using System;

namespace Apzon.Entities.Models.PlanningProduction;

public class MrpRunCapacity
{
	public int Id { get; set; }

	public int? Type { get; set; }

	public DateTime? WorkDay { get; set; }

	public double? Qty { get; set; }

	public string Item { get; set; }

	public int? MRPID { get; set; }

	public string TypeName { get; set; }
}
