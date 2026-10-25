using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Inventory;

namespace Apzon.Entities.Models;

public class InventoryTransferLineTable : Table<InventoryTranfer_LineImpl>
{
	public new InventoryTransferLineRow this[int idx] => (InventoryTransferLineRow)Rows[idx];

	public int Count => Rows.Count;

	public event InventoryTransferLineRowChangedDlgt InventoryTransferLineRowChanged;

	protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
	{
		return new InventoryTransferLineRow(builder);
	}

	protected override void OnRowChanged(DataRowChangeEventArgs e)
	{
		base.OnRowChanged(e);
		InventoryTransferLineRowChangedEventArgs args = new InventoryTransferLineRowChangedEventArgs(e.Action, (InventoryTransferLineRow)e.Row);
		OnInventoryTransferLineRowChanged(args);
	}

	protected virtual void OnInventoryTransferLineRowChanged(InventoryTransferLineRowChangedEventArgs args)
	{
		if (InventoryTransferLineRowChanged != null)
		{
			InventoryTransferLineRowChanged(this, args);
		}
	}

	public new InventoryTransferLineTable GetClone()
	{
		return (InventoryTransferLineTable)Clone();
	}

	internal new List<InventoryTransferLineRow> GetEnumerable()
	{
		List<InventoryTransferLineRow> list = new List<InventoryTransferLineRow>();
		for (int i = 0; i < Rows.Count; i++)
		{
			list.Add(this[i]);
		}
		return list;
	}

	public new InventoryTransferLineRow GetNewRow()
	{
		return (InventoryTransferLineRow)NewRow();
	}
}
