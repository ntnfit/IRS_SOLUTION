using SAPbobsCOM;

namespace Apzon.Entities.Models.Documents;

public class DownPaymentToDrawDetailImpl
{
	public int RowNum { get; set; }

	public int SeqNum { get; }

	public int DocEntry { get; }

	public string VatGroupCode { get; set; }

	public double VatPercent { get; }

	public double AmountToDraw { get; set; }

	public double AmountToDrawFC { get; set; }

	public double AmountToDrawSC { get; set; }

	public double Tax { get; set; }

	public double TaxFC { get; set; }

	public double TaxSC { get; set; }

	public BoYesNoEnum IsGrossLine { get; }

	public double GrossAmountToDraw { get; set; }

	public double GrossAmountToDrawFC { get; set; }

	public double GrossAmountToDrawSC { get; set; }

	public LineTypeEnum LineType { get; set; }

	public int Count { get; }

	public int DocInternalID { get; }

	public BoYesNoEnum TaxAdjust { get; }
}
