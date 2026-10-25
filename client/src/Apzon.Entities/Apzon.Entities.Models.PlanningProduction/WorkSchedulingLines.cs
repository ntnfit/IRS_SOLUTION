using System;

namespace Apzon.Entities.Models.PlanningProduction;

public class WorkSchedulingLines
{
	public DateTime WorkDate { get; set; }

	public int ID { get; set; }

	public string Shift { get; set; }

	public string Remark { get; set; }

	public short? LinID { get; set; }
}
