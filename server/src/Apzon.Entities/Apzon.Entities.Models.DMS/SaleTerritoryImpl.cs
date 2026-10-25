using System;

namespace Apzon.Entities.Models.DMS;

public class SaleTerritoryImpl
{
	public string Code { get; set; }

	public string Name { get; set; }

	public string CountryCode { get; set; }

	public string Type { get; set; }

	public string Remarks { get; set; }

	public string Status { get; set; }

	public string UserSign { get; set; }

	public DateTime CreateDate { get; set; }

	public DateTime CreateTime { get; set; }
}
