using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;
using SAPbobsCOM;

namespace Apzon.Entities.Models.Documents;

public class TaxExtensionImpl
{
	public string BlockB { get; set; }

	public string BlockS { get; set; }

	public string Brand { get; set; }

	public string BuildingB { get; set; }

	public string BuildingS { get; set; }

	public string Carrier { get; set; }

	public string CityB { get; set; }

	public string CityS { get; set; }

	public string CountryB { get; set; }

	public string CountryS { get; set; }

	public string County { get; set; }

	public string CountyB { get; set; }

	public string CountyS { get; set; }

	public string GlobalLocationNumberB { get; set; }

	public string GlobalLocationNumberS { get; set; }

	public double GrossWeight { get; set; }

	public BoYesNoEnum ImportOrExport { get; set; }

	public string Incoterms { get; set; }

	public int MainUsage { get; set; }

	public string NFRef { get; set; }

	public double NetWeight { get; set; }

	public string PackDescription { get; set; }

	public int PackQuantity { get; set; }

	public int ShipUnitNo { get; set; }

	public string State { get; set; }

	public string StateB { get; set; }

	public string StateS { get; set; }

	public string StreetB { get; set; }

	public string StreetS { get; set; }

	public string TaxId0 { get; set; }

	public string TaxId1 { get; set; }

	public string TaxId12 { get; set; }

	public string TaxId2 { get; set; }

	public string TaxId3 { get; set; }

	public string TaxId4 { get; set; }

	public string TaxId5 { get; set; }

	public string TaxId6 { get; set; }

	public string TaxId7 { get; set; }

	public string TaxId8 { get; set; }

	public string TaxId9 { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public string Vehicle { get; set; }

	public string VehicleState { get; set; }

	public string ZipCodeB { get; set; }

	public string ZipCodeS { get; set; }
}
