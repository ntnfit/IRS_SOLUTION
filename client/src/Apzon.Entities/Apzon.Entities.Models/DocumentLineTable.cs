using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Apzon.Commons;

namespace Apzon.Entities.Models;

public class DocumentLineTable : DataTable
{
	public int MaxDatasourceIndex = -1;

	public DocumentLineRow this[int idx] => (DocumentLineRow)Rows[idx];

	public int Count => Rows.Count;

	public event DocumentLineRowChangedDlgt DocumentLineRowChanged;

	protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
	{
		return new DocumentLineRow(builder);
	}

	protected override void OnRowChanged(DataRowChangeEventArgs e)
	{
		base.OnRowChanged(e);
		DocumentLineRowChangedEventArgs args = new DocumentLineRowChangedEventArgs(e.Action, (DocumentLineRow)e.Row);
		OnDocumentRowChanged(args);
	}

	protected virtual void OnDocumentRowChanged(DocumentLineRowChangedEventArgs args)
	{
		if (DocumentLineRowChanged != null)
		{
			DocumentLineRowChanged(this, args);
		}
	}

	public DocumentLineRow GetByNum(int lineNum)
	{
		try
		{
			return GetEnumerable().FirstOrDefault((DocumentLineRow t) => Function.ParseInt(t["LineNum"]).Equals(lineNum));
		}
		catch (Exception)
		{
			return null;
		}
	}

	public void Add(DocumentLineRow row)
	{
		if (row != null)
		{
			Rows.Add(row);
		}
	}

	public void Remove(DocumentLineRow row)
	{
		if (row != null)
		{
			Rows.Remove(row);
		}
	}

	public List<DocumentLineRow> GetEnumerable()
	{
		List<DocumentLineRow> list = new List<DocumentLineRow>();
		for (int i = 0; i < Rows.Count; i++)
		{
			list.Add(this[i]);
		}
		return list;
	}

	public DocumentLineRow GetNewRow()
	{
		DocumentLineRow documentLineRow = (DocumentLineRow)NewRow();
		documentLineRow["LineNum"] = -1;
		return documentLineRow;
	}

	protected override Type GetRowType()
	{
		return typeof(DocumentLineRow);
	}

	public object GetMax(string field)
	{
		return this.AsEnumerable().Max((DataRow row) => Function.ParseLong(row[field]));
	}

	public DocumentLineTable GetClone()
	{
		return (DocumentLineTable)Clone();
	}

	public void Insert(int idx, DocumentLineRow line)
	{
		Rows.InsertAt(line, idx);
	}

	public void RemoveAt(int idx)
	{
		Rows.RemoveAt(idx);
	}

	public int GetNewLineNum(int currentLine)
	{
		int num = currentLine;
		IEnumerable<DocumentLineRow> enumerable = from t in GetEnumerable()
			where Function.ToString(t["LineType"]) == "R"
			select t;
		if (enumerable != null && enumerable.Any())
		{
			int? num2 = enumerable.Max((Func<DocumentLineRow, int?>)((DocumentLineRow x) => x.LineNum));
			if (num > MaxDatasourceIndex)
			{
				num = num;
			}
			else
			{
				num = ((!num2.HasValue || num2.Value < MaxDatasourceIndex) ? (MaxDatasourceIndex + 1) : (num2.Value + 1));
			}
		}
		else
		{
			num = MaxDatasourceIndex + 1;
		}
		if (num < 0)
		{
			return 0;
		}
		return num;
	}
}
