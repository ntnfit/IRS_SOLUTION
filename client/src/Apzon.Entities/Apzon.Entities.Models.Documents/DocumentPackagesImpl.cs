using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Documents;

public class DocumentPackagesImpl
{
	public int Count { get; set; }

	public List<DocumentPackageItemsImpl> Items { get; set; }

	public int Number { get; set; }

	public double TotalWeight { get; set; }

	public string Type { get; set; }

	public int Units { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }
}
