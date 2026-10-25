using SAPbobsCOM;

namespace Apzon.Entities.Models.Documents;

public class DocumentLinesBinAllocationsImpl
{
	public BoYesNoEnum AllowNegativeQuantity { get; set; }

	public BinActionTypeEnum BinActionType { get; set; }

	public int BaseLineNumber { get; set; }

	public int BinAbsEntry { get; set; }

	public int Count { get; set; }

	public double Quantity { get; set; }

	public int? SerialAndBatchNumbersBaseLine { get; set; }
}
