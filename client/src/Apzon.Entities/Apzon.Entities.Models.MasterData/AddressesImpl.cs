using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.MasterData;

public class AddressesImpl
{
	public string Address { get; set; }

	public string CardCode { get; set; }

	public string Street { get; set; }

	public string Block { get; set; }

	public string ZipCode { get; set; }

	public string City { get; set; }

	public string County { get; set; }

	public string Country { get; set; }

	public string State { get; set; }

	public short? UserSign { get; set; }

	public int? LogInstanc { get; set; }

	public string ObjType { get; set; }

	public string LicTradNum { get; set; }

	public int? LineNum { get; set; }

	public string TaxCode { get; set; }

	public string Building { get; set; }

	public string AdresType { get; set; }

	public string Address2 { get; set; }

	public string Address3 { get; set; }

	public string AddrType { get; set; }

	public string StreetNo { get; set; }

	public string AltCrdName { get; set; }

	public string AltTaxId { get; set; }

	public string TaxOffice { get; set; }

	public string GlblLocNum { get; set; }

	public string Ntnlty { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }
}
