using System;

namespace Apzon.Entities.Models.PlanningProduction;

public class Mrp
{
	public long Id { get; set; }

	public DateTime? BeginDate { get; set; }

	public string MrpCode { get; set; }

	public string MrpName { get; set; }

	public DateTime? EndDate { get; set; }

	public int? Status { get; set; }

	public DateTime? CreateDate { get; set; }

	public string MpsCode { get; set; }

	public string Whs { get; set; }

	public string Notes { get; set; }

	public string PoEntry { get; set; }

	public DateTime? FixDate { get; set; }
}
