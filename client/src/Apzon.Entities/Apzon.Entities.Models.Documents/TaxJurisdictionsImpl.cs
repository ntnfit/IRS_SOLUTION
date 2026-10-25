using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Documents;

public class TaxJurisdictionsImpl
{
	public int Count { get; set; }

	public int DocEntry { get; set; }

	public string JurisdictionCode { get; set; }

	public int JurisdictionType { get; set; }

	public int LineNumber { get; set; }

	public int RowSequence { get; set; }

	public double TaxAmount { get; set; }

	public double TaxAmountFC { get; set; }

	public double TaxAmountSC { get; set; }

	public double TaxRate { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }
}
