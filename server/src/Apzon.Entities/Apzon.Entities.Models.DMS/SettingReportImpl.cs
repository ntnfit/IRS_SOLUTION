using System;
using System.Collections.Generic;

namespace Apzon.Entities.Models.DMS;

public class SettingReportImpl
{
	public int DocEntry { get; set; }

	public string ReportId { get; set; }

	public int UserSign { get; set; }

	public DateTime CreateDate { get; set; }

	public int LogInstanc { get; set; }

	public List<SettingReportLineImpl> Line_SRT { get; set; }
}
