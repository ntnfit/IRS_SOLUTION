using System;

namespace Apzon.Entities.Models.PlanningProduction;

public class Mps
{
	public long Id { get; set; }

	public DateTime? BeginDate { get; set; }

	public string MpsCode { get; set; }

	public DateTime? EndDate { get; set; }

	public int? Status { get; set; }

	public DateTime? CreateDate { get; set; }

	public string Notes { get; set; }

	public int? ForCastID { get; set; }

	public DateTime? FixDate { get; set; }
}
