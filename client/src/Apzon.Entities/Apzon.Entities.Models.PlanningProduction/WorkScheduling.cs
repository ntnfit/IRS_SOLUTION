using System;

namespace Apzon.Entities.Models.PlanningProduction;

public class WorkScheduling
{
	public int Id { get; set; }

	public string Name { get; set; }

	public DateTime? FromDate { get; set; }

	public DateTime? ToDate { get; set; }

	public string Notes { get; set; }

	public int? Year { get; set; }
}
