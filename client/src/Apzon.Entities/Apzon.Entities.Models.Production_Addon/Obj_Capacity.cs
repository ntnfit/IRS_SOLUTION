using System;

namespace Apzon.Entities.Models.Production_Addon;

public class Obj_Capacity
{
	public string ResCode { get; set; }

	public int WeekDay { get; set; }

	public int LineNum { get; set; }

	public string UnitOfMsr { get; set; }

	public decimal? CapFactor1 { get; set; }

	public decimal? CapFactor2 { get; set; }

	public decimal? CapFactor3 { get; set; }

	public decimal? CapFactor4 { get; set; }

	public decimal? CapTotal { get; set; }

	public string Remarks { get; set; }

	public DateTime validFrom { get; set; }

	public DateTime validTo { get; set; }

	public TimeSpan StartTime { get; set; }

	public TimeSpan FinishTime { get; set; }

	public double? BreaksTime { get; set; }
}
