namespace Apzon.Entities.Models.BusinessPartners;

public class InternalReconciliationLineImpl
{
	public int LineSeq { get; set; }

	public string ShortName { get; set; }

	public int TransId { get; set; }

	public int TransRowId { get; set; }

	public string SrcObjTyp { get; set; }

	public int SrcObjAbs { get; set; }

	public string IsCredit { get; set; }

	public decimal ReconSum { get; set; }

	public decimal CashDiscount { get; set; }

	public string Account { get; set; }
}
