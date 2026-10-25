using System.Data;

namespace Apzon.Entities.Models;

public class InventoryTransferLineRowChangedEventArgs
{
	protected DataRowAction action;

	protected InventoryTransferLineRow row;

	public DataRowAction Action => action;

	public InventoryTransferLineRow Row => row;

	public InventoryTransferLineRowChangedEventArgs(DataRowAction action, InventoryTransferLineRow row)
	{
		this.action = action;
		this.row = row;
	}
}
