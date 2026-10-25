using System;

namespace Apzon.Entities.Models.Inventory;

public class SNBLinesImpl
{
	public DateTime AdmissionDate { get; set; }

	public int BaseLine { get; set; }

	public int Count { get; set; }

	public double DebitCredit { get; set; }

	public DateTime ExpirationDate { get; set; }

	public string LotNumber { get; set; }

	public string ManufactureNumber { get; set; }

	public double NewCost { get; set; }

	public int SnbAbsEntry { get; set; }

	public int SystemNumber { get; set; }
}
