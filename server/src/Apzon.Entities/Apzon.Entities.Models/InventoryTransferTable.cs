using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Inventory;

namespace Apzon.Entities.Models;

public class InventoryTransferTable : Table<InventoryTranferImpl>
{
	public new InventoryTransferRow this[int idx] => (InventoryTransferRow)Rows[idx];

	public int Count => Rows.Count;

	public event InventoryTransferRowChangedDlgt InventoryTransferRowChanged;

	protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
	{
		return new InventoryTransferRow(builder);
	}

	protected override void OnRowChanged(DataRowChangeEventArgs e)
	{
		base.OnRowChanged(e);
		InventoryTransferRowChangedEventArgs args = new InventoryTransferRowChangedEventArgs(e.Action, (InventoryTransferRow)e.Row);
		OnInventoryTransferRowChanged(args);
	}

	protected virtual void OnInventoryTransferRowChanged(InventoryTransferRowChangedEventArgs args)
	{
		if (InventoryTransferRowChanged != null)
		{
			InventoryTransferRowChanged(this, args);
		}
	}

	internal new List<InventoryTransferRow> GetEnumerable()
	{
		List<InventoryTransferRow> list = new List<InventoryTransferRow>();
		for (int i = 0; i < Rows.Count; i++)
		{
			list.Add(this[i]);
		}
		return list;
	}

	public new InventoryTransferRow GetNewRow()
	{
		return (InventoryTransferRow)NewRow();
	}

	public new InventoryTransferTable GetClone()
	{
		return (InventoryTransferTable)Clone();
	}
}
