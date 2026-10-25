using System;

namespace Apzon.Entities.Models.Mrps;

public class ScheduleCapacityImpl
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
