using SAPbobsCOM;

namespace Apzon.Entities.Models.Inventory;

public class StockTransferLinesBinAllocationsImpl
{
	public bool AllowNegativeQuantity { get; set; }

	public int BaseLineNumber { get; set; }

	public int BinAbsEntry { get; set; }

	public BinActionTypeEnum BinActionType { get; set; }

	public double Quantity { get; set; }

	public int SerialAndBatchNumbersBaseLine { get; set; }
}
