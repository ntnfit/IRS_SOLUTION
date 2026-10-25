using System;
using System.Collections.Generic;

namespace Apzon.Entities.Models.Mrps;

public class ScheduleImpl
{
	public long Id { get; set; }

	public DateTime BeginDate { get; set; }

	public string MrpCode { get; set; }

	public DateTime EndDate { get; set; }

	public string Status { get; set; }

	public DateTime CreateDate { get; set; }

	public string Notes { get; set; }

	public string Remark { get; set; }

	public string PSCode { get; set; }

	public List<ScheduleDetailImpl> LineDetail { get; set; }
}
