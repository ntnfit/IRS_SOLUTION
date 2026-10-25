using System;

namespace Apzon.Entities.Models.Administrator;

public class ProjectImpl
{
	public string PrjCode { get; set; }

	public string PrjName { get; set; }

	public string Locked { get; set; }

	public string DataSource { get; set; }

	public string UserSign { get; set; }

	public DateTime? ValidFrom { get; set; }

	public DateTime? ValidTo { get; set; }

	public string Active { get; set; }

	public int LogInstanc { get; set; }

	public int UserSign2 { get; set; }

	public DateTime UpdateDate { get; set; }
}
