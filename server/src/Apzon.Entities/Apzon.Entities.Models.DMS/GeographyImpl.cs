using System;

namespace Apzon.Entities.Models.DMS;

public class GeographyImpl
{
	public string CountryCode { get; set; }

	public string CountryName { get; set; }

	public string Status { get; set; }

	public string Remarks { get; set; }

	public DateTime CreateDate { get; set; }

	public DateTime CreateTime { get; set; }

	public string UserSign { get; set; }

	public string Location { get; set; }
}
