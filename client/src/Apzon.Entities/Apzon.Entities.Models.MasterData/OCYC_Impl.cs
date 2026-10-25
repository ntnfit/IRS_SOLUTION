using System;

namespace Apzon.Entities.Models.MasterData;

public class OCYC_Impl
{
	public int Code { get; set; }

	public string Name { get; set; }

	public string Frequency { get; set; }

	public int? Day { get; set; }

	public int? Hour { get; set; }

	public int? UserSign { get; set; }

	public DateTime? NextDate { get; set; }

	public string Type { get; set; }

	public string SubOption { get; set; }

	public int? Interval { get; set; }

	public string EndType { get; set; }

	public int? MaxOccur { get; set; }

	public DateTime? SeEndDat { get; set; }

	public string Sunday { get; set; }

	public string Monday { get; set; }

	public string Tuesday { get; set; }

	public string Wednesday { get; set; }

	public string Thursday { get; set; }

	public string Friday { get; set; }

	public string Saturday { get; set; }

	public int? DayInMonth { get; set; }

	public int? Week { get; set; }

	public int? DayOfWeek { get; set; }

	public int? Month { get; set; }
}
