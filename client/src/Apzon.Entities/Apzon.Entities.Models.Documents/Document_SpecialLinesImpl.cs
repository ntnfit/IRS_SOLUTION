using SAPbobsCOM;

namespace Apzon.Entities.Models.Documents;

public class Document_SpecialLinesImpl
{
	public int AfterLineNumber { get; set; }

	public int Count { get; set; }

	public double Freight1 { get; set; }

	public double Freight1FC { get; set; }

	public double Freight1SC { get; set; }

	public double Freight2 { get; set; }

	public double Freight2FC { get; set; }

	public double Freight2SC { get; set; }

	public double Freight3 { get; set; }

	public double Freight3FC { get; set; }

	public double Freight3SC { get; set; }

	public double GrossTotal { get; set; }

	public double GrossTotalFC { get; set; }

	public double GrossTotalSC { get; set; }

	public int LineNum { get; set; }

	public string LineText { get; set; }

	public BoDocSpecialLineType LineType { get; set; }

	public int OrderNumber { get; set; }

	public double Subtotal { get; set; }

	public double SubtotalFC { get; set; }

	public double SubtotalSC { get; set; }

	public double TaxAmount { get; set; }

	public double TaxAmountFC { get; set; }

	public double TaxAmountSC { get; set; }
}
