using System;
using System.Collections.Generic;

namespace Apzon.Entities.Models.Mrps;

public class MRPImpl
{
	public long Id { get; set; }

	public DateTime BeginDate { get; set; }

	public string MrpCode { get; set; }

	public string MrpName { get; set; }

	public DateTime EndDate { get; set; }

	public string Status { get; set; }

	public DateTime? CreateDate { get; set; }

	public string MpsCode { get; set; }

	public string Whs { get; set; }

	public string Notes { get; set; }

	public string PoEntry { get; set; }

	public DateTime FixDate { get; set; }

	public string ForeCast { get; set; }

	public string Plant { get; set; }

	public string Calendar { get; set; }

	public string Remark { get; set; }

	public string Type { get; set; }

	public List<MRP1Impl> LineMRP { get; set; }

	public List<MRP2Impl> LineCapacity { get; set; }
}
