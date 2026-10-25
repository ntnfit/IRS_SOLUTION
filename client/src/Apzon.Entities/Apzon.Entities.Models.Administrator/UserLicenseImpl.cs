using System.Collections.Generic;
using Apzon.Commons;

namespace Apzon.Entities.Models.Administrator;

public class UserLicenseImpl
{
	public string UserCode { get; set; }

	public bool IsUserLicense { get; set; }

	public LicenseRule UserRule { get; set; }

	public LicenseRule Rule { get; set; }

	public List<LicenseInfoImpl> LstLicenseMapping { get; set; }

	public List<string> LstAccessFunctionOfUser { get; set; }

	public List<string> LstAccessFormInLicense { get; set; }
}
