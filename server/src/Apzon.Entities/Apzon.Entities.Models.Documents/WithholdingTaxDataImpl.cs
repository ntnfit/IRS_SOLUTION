using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;
using SAPbobsCOM;

namespace Apzon.Entities.Models.Documents;

public class WithholdingTaxDataImpl
{
	public int BaseDocEntry { get; set; }

	public int BaseDocLine { get; set; }

	public int BaseDocType { get; set; }

	public int BaseDocumentReference { get; set; }

	public string BaseType { get; set; }

	public string Category { get; set; }

	public int Count { get; set; }

	public string Criteria { get; set; }

	public string GLAccount { get; set; }

	public int LineNum { get; set; }

	public double Rate { get; set; }

	public string RoundingType { get; set; }

	public BoStatus Status { get; set; }

	public int TargetAbsEntry { get; set; }

	public int TargetDocumentType { get; set; }

	public double TaxableAmount { get; set; }

	public double TaxableAmountFC { get; set; }

	public double TaxableAmountinSys { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public double WTAmount { get; set; }

	public double WTAmountFC { get; set; }

	public double WTAmountSys { get; set; }

	public string WTCode { get; set; }

	public string WithholdingType { get; set; }
}
