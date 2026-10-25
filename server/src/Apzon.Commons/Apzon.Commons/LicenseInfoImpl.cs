using System;
using System.Collections.Generic;

namespace Apzon.Commons;

public class LicenseInfoImpl
{
	public string TypeCode { get; set; }

	public string TypeName { get; set; }

	public LicenseRule LicenseRule { get; set; }

	public bool LicenseStatus { get; set; }

	public bool IsMappingUser { get; set; }

	public int Amount { get; set; }

	public int Used { get; set; }

	public DateTime ExpiredDate { get; set; }

	public List<string> LstAccessForm { get; set; }
}
