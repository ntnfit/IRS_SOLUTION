using System;

namespace Apzon.Entities.Models.PlanningProduction;

public class Shift
{
	public string Code { get; set; }

	public string Name { get; set; }

	public double? Time { get; set; }

	public string Notes { get; set; }

	public TimeSpan? Start1 { get; set; }

	public TimeSpan? End1 { get; set; }

	public TimeSpan? StartBreak1 { get; set; }

	public TimeSpan? EndBreak1 { get; set; }

	public TimeSpan? Start2 { get; set; }

	public TimeSpan? End2 { get; set; }

	public TimeSpan? StartBreak2 { get; set; }

	public TimeSpan? EndBreak2 { get; set; }

	public bool? Time2 { get; set; }
}
