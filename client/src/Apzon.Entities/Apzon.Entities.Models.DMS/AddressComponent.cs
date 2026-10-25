using System.Collections.Generic;

namespace Apzon.Entities.Models.DMS;

public class AddressComponent
{
	public string long_name { get; set; }

	public string short_name { get; set; }

	public List<string> types { get; set; }
}
