using System;

namespace Apzon.Entities.Models.Production;

public class MaterialShortageReport_Impl
{
	public DateTime? FromDate { get; set; }

	public DateTime? ToDate { get; set; }

	public string Type { get; set; }

	public string XcludProductionOrder { get; set; }
}
