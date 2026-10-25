using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Documents;

public class DocumentPackageItemsImpl
{
	public int Count { get; set; }

	public string ItemCode { get; set; }

	public string MeasureUnit { get; set; }

	public double Quantity { get; set; }

	public double UnitsOfMeasurement { get; set; }

	public int UoMEntry { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }
}
