using System.Collections.Generic;

namespace Apzon.Entities.Models.DMS;

public class GoogleApiGeoCodeResultFormatedAddressImpl
{
	public string formatted_address { get; set; }

	public List<AddressComponent> address_components { get; set; }
}
