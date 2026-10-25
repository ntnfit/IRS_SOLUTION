using System;
using System.Collections.Generic;
using System.Data;

namespace Apzon.Entities.Models;

public class DocumentTable : DataTable
{
	public DocumentRow this[int idx] => (DocumentRow)Rows[idx];

	public int Count => Rows.Count;

	public event DocumentRowChangedDlgt DocumentRowChanged;

	public void Add(DocumentRow row)
	{
		Rows.Add(row);
	}

	public void Remove(DocumentRow row)
	{
		Rows.Remove(row);
	}

	public DocumentRow GetNewRow()
	{
		return (DocumentRow)NewRow();
	}

	public DocumentRow FirstOrDefault()
	{
		try
		{
			return (DocumentRow)Rows[0];
		}
		catch (Exception)
		{
			return null;
		}
	}

	protected override Type GetRowType()
	{
		return typeof(DocumentRow);
	}

	protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
	{
		return new DocumentRow(builder);
	}

	protected override void OnRowChanged(DataRowChangeEventArgs e)
	{
		base.OnRowChanged(e);
		DocumentRowChangedEventArgs args = new DocumentRowChangedEventArgs(e.Action, (DocumentRow)e.Row);
		OnDocumentRowChanged(args);
	}

	protected virtual void OnDocumentRowChanged(DocumentRowChangedEventArgs args)
	{
		if (DocumentRowChanged != null)
		{
			DocumentRowChanged(this, args);
		}
	}

	public DocumentTable GetClone()
	{
		return (DocumentTable)Clone();
	}

	public void Insert(int idx, DocumentRow doc)
	{
		Rows.InsertAt(doc, idx);
	}

	public List<DocumentRow> GetEnumerable()
	{
		List<DocumentRow> list = new List<DocumentRow>();
		for (int i = 0; i < Rows.Count; i++)
		{
			list.Add(this[i]);
		}
		return list;
	}
}
