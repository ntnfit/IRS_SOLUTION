using System;

namespace Apzon.Entities.Models.Mrps;

public class MRP2Impl
{
	public long Id { get; set; }

	public int? Type { get; set; }

	public DateTime? WorkDay { get; set; }

	public double? Qty { get; set; }

	public string Item { get; set; }

	public string Notes { get; set; }

	public int? MRPID { get; set; }

	public string Line { get; set; }

	public string TypeName { get; set; }

	public string Remark { get; set; }

	public string Remark1 { get; set; }

	public string Remark2 { get; set; }
}
