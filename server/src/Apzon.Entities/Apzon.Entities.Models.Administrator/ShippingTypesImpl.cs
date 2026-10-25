using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Administrator;

public class ShippingTypesImpl
{
	public int Code { get; set; }

	public string Name { get; set; }

	public int TrnspCode { get; set; }

	public string TrnspName { get; set; }

	public string WebSite { get; set; }

	public string DataSource { get; set; }

	public int UserSign { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }
}
