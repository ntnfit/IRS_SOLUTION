using System.Data;

namespace Apzon.Entities.Models;

public class InventoryTransferRowChangedEventArgs
{
	protected DataRowAction action;

	protected InventoryTransferRow row;

	public DataRowAction Action => action;

	public InventoryTransferRow Row => row;

	public InventoryTransferRowChangedEventArgs(DataRowAction action, InventoryTransferRow row)
	{
		this.action = action;
		this.row = row;
	}
}
