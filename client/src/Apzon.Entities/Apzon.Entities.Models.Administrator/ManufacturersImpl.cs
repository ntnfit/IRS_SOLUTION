using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Administrator;

public class ManufacturersImpl
{
	public int Code { get; set; }

	public string ManufacturerName { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public int FIRMCODE { get; set; }

	public string FIRMNAME { get; set; }

	public int UserSign { get; set; }

	public string DataSource { get; set; }
}
