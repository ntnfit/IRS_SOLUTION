using System;

namespace Apzon.Entities.Models.Mrps;

public class SalesForeCastImpl
{
	public int AbsID { get; set; }

	public string Code { get; set; }

	public string Name { get; set; }

	public short UserSign { get; set; }

	public DateTime? StartDate { get; set; }

	public DateTime? EndDate { get; set; }

	public string FormView { get; set; }
}
