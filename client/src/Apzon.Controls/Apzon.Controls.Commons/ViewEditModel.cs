using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Controls.Grid;
using Apzon.Controls.Tree;
using DevExpress.DataProcessing;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;

namespace Apzon.Controls.Commons;

public class ViewEditModel
{
	public Component Grid { get; set; }

	public Component ColumnEdit { get; set; }

	public int IndexEditRow { get; set; }

	public string Name
	{
		get
		{
			try
			{
				if (Grid == null)
				{
					return "";
				}
				if (Grid is GridViewEx)
				{
					GridViewEx gridViewEx = Grid as GridViewEx;
					return (gridViewEx.GridControl == null) ? "" : gridViewEx.Name;
				}
				if (Grid is BandedGridViewEx)
				{
					BandedGridViewEx bandedGridViewEx = Grid as BandedGridViewEx;
					return (bandedGridViewEx.GridControl == null) ? "" : bandedGridViewEx.Name;
				}
				if (Grid is TreeEx)
				{
					return (Grid as TreeEx).Name;
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			}
			return "";
		}
	}

	public string ViewCaption
	{
		get
		{
			try
			{
				if (Grid == null)
				{
					return "";
				}
				if (Grid is GridViewEx)
				{
					GridViewEx gridViewEx = Grid as GridViewEx;
					return (gridViewEx.GridControl == null) ? "" : gridViewEx.ViewCaption;
				}
				if (Grid is BandedGridViewEx)
				{
					BandedGridViewEx bandedGridViewEx = Grid as BandedGridViewEx;
					return (bandedGridViewEx.GridControl == null) ? "" : bandedGridViewEx.ViewCaption;
				}
				if (Grid is TreeEx)
				{
					return (Grid as TreeEx).Name;
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			}
			return "";
		}
	}

	public DataTable DataSource
	{
		get
		{
			try
			{
				if (Grid == null)
				{
					return new DataTable();
				}
				if (Grid is GridViewEx)
				{
					GridViewEx gridViewEx = Grid as GridViewEx;
					return (gridViewEx.GridControl == null) ? new DataTable() : gridViewEx.GridControl.DataSourceTable();
				}
				if (Grid is BandedGridViewEx)
				{
					BandedGridViewEx bandedGridViewEx = Grid as BandedGridViewEx;
					return (bandedGridViewEx.GridControl == null) ? new DataTable() : bandedGridViewEx.GridControl.DataSourceTable();
				}
				if (Grid is TreeEx)
				{
					return (Grid as TreeEx).DataSourceTable;
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			}
			return new DataTable();
		}
		set
		{
			try
			{
				if (Grid != null)
				{
					if (Grid is GridViewEx)
					{
						(Grid as GridViewEx).GridControl.DataSource = value;
					}
					else if (Grid is BandedGridViewEx)
					{
						(Grid as BandedGridViewEx).GridControl.DataSource = value;
					}
					else if (Grid is TreeEx)
					{
						(Grid as TreeEx).DataSource = value;
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			}
		}
	}

	public string ColumnEditFieldName
	{
		get
		{
			try
			{
				if (ColumnEdit == null)
				{
					return "";
				}
				if (ColumnEdit is GridColumnEx)
				{
					return (ColumnEdit as GridColumnEx).FieldName;
				}
				if (ColumnEdit is BandedGridColumnEx)
				{
					return (ColumnEdit as BandedGridColumnEx).FieldName;
				}
				if (ColumnEdit is TreeListColumnEx)
				{
					return (ColumnEdit as TreeListColumnEx).FieldName;
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			}
			return "";
		}
	}

	public string ItemId
	{
		get
		{
			if (Grid == null)
			{
				return "";
			}
			if (Grid is GridViewEx)
			{
				return (Grid as GridViewEx).ItemId;
			}
			if (Grid is BandedGridViewEx)
			{
				return (Grid as BandedGridViewEx).ItemId;
			}
			if (Grid is TreeEx)
			{
				return (Grid as TreeEx).ItemId;
			}
			return "";
		}
	}

	public ViewEditModel()
	{
	}

	public ViewEditModel(Component view)
	{
		Grid = view;
	}

	public int GetDataRowIndex(DataRow row)
	{
		if (!DataSource.IsNotNull())
		{
			return -1;
		}
		return DataSource.AsEnumerable().IndexOf(row);
	}

	public int GetRowIndex(int sourceIndex)
	{
		try
		{
			if (Grid == null || !DataSource.IsNotNull())
			{
				return -1;
			}
			if (Grid is GridViewEx gridViewEx)
			{
				return gridViewEx.GetRowHandle(sourceIndex);
			}
			if (Grid is TreeEx)
			{
				return sourceIndex;
			}
			if (Grid is BandedGridViewEx bandedGridViewEx)
			{
				return bandedGridViewEx.GetRowHandle(sourceIndex);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return -1;
	}

	public int GetSourceIndex(int rowHandle)
	{
		try
		{
			if (Grid == null || !DataSource.IsNotNull())
			{
				return -1;
			}
			if (Grid is GridViewEx gridViewEx)
			{
				return gridViewEx.GetDataSourceRowIndex(rowHandle);
			}
			if (Grid is TreeEx)
			{
				return rowHandle;
			}
			if (Grid is BandedGridViewEx bandedGridViewEx)
			{
				return bandedGridViewEx.GetDataSourceRowIndex(rowHandle);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return -1;
	}

	public object GetFocusedRowCellValue(string fieldName)
	{
		try
		{
			if (Grid == null)
			{
				return null;
			}
			if (Grid is GridViewEx gridViewEx)
			{
				return gridViewEx.GetFocusedRowCellValue(fieldName);
			}
			if (Grid is TreeEx)
			{
				return DataSource.Rows[IndexEditRow][fieldName];
			}
			if (Grid is BandedGridViewEx bandedGridViewEx)
			{
				return bandedGridViewEx.GetFocusedRowCellValue(fieldName);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}

	public int GetParentRowHandle(int rowHandle)
	{
		try
		{
			if (Grid == null)
			{
				return 0;
			}
			if (Grid is GridViewEx gridViewEx)
			{
				return gridViewEx.GetParentRowHandle(rowHandle);
			}
			if (Grid is TreeEx)
			{
				return 0;
			}
			if (Grid is BandedGridViewEx bandedGridViewEx)
			{
				return bandedGridViewEx.GetParentRowHandle(rowHandle);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	public int GetChildRowCount(int rowHandle)
	{
		try
		{
			if (Grid == null)
			{
				return 0;
			}
			if (Grid is GridViewEx gridViewEx)
			{
				return gridViewEx.GetChildRowCount(rowHandle);
			}
			if (Grid is TreeEx)
			{
				return 0;
			}
			if (Grid is BandedGridViewEx bandedGridViewEx)
			{
				return bandedGridViewEx.GetChildRowCount(rowHandle);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	public int GetChildRowHandle(int rowHandle, int childIndex)
	{
		try
		{
			if (Grid == null)
			{
				return int.MinValue;
			}
			if (Grid is GridViewEx gridViewEx)
			{
				return gridViewEx.GetChildRowHandle(rowHandle, childIndex);
			}
			if (Grid is TreeEx)
			{
				return int.MinValue;
			}
			if (Grid is BandedGridViewEx bandedGridViewEx)
			{
				return bandedGridViewEx.GetChildRowHandle(rowHandle, childIndex);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return int.MinValue;
	}

	public object GetRowCellValue(int index, string fieldName)
	{
		try
		{
			if (Grid == null || index < 0)
			{
				return null;
			}
			if (Grid is GridViewEx gridViewEx)
			{
				return gridViewEx.GetRowCellValue(index, fieldName);
			}
			if (Grid is TreeEx)
			{
				return DataSource.Rows[index][fieldName];
			}
			if (Grid is BandedGridViewEx bandedGridViewEx)
			{
				return bandedGridViewEx.GetRowCellValue(index, fieldName);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}

	public void SetFocusedRowCellValue(string fieldName, object value)
	{
		try
		{
			if (Grid == null)
			{
				return;
			}
			if (Grid is GridViewEx gridViewEx)
			{
				gridViewEx.SetFocusedRowCellValue(fieldName, value);
			}
			else if (Grid is BandedGridViewEx bandedGridViewEx)
			{
				bandedGridViewEx.SetFocusedRowCellValue(fieldName, value);
			}
			else if (Grid is TreeEx treeEx)
			{
				object keyID = DataSource.Rows[IndexEditRow][treeEx.KeyFieldName];
				TreeListNode treeListNode = treeEx.FindNodeByKeyID(keyID);
				if (treeListNode != null)
				{
					treeEx.SetRowCellValue(treeListNode, fieldName, value);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public void SetRowCellValue(int rowHandle, string fieldName, object value)
	{
		try
		{
			if (Grid == null)
			{
				return;
			}
			if (Grid is GridViewEx gridViewEx)
			{
				gridViewEx.SetRowCellValue(rowHandle, fieldName, value);
			}
			else if (Grid is BandedGridViewEx bandedGridViewEx)
			{
				bandedGridViewEx.SetRowCellValue(rowHandle, fieldName, value);
			}
			else if (Grid is TreeEx treeEx)
			{
				object keyID = DataSource.Rows[rowHandle][treeEx.KeyFieldName];
				TreeListNode treeListNode = treeEx.FindNodeByKeyID(keyID);
				if (treeListNode != null)
				{
					treeEx.SetRowCellValue(treeListNode, fieldName, value);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public void AddChangedField(string fieldName)
	{
		try
		{
			if (Grid != null && Grid is GridViewEx)
			{
				(Grid as GridViewEx).AddChangedField(fieldName);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public void RemoveChangedField(string fieldName)
	{
		try
		{
			if (Grid != null && Grid is GridViewEx)
			{
				(Grid as GridViewEx).RemoveChangedField(fieldName);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public string GetCaptionColumInView(string fieldName)
	{
		try
		{
			if (Grid is GridViewEx gridViewEx)
			{
				GridColumn gridColumn = gridViewEx.Columns[fieldName];
				if (gridColumn == null)
				{
					return fieldName;
				}
				return gridColumn.Caption;
			}
			if (Grid is BandedGridViewEx bandedGridViewEx)
			{
				BandedGridColumn bandedGridColumn = bandedGridViewEx.Columns[fieldName];
				if (bandedGridColumn == null)
				{
					return fieldName;
				}
				return bandedGridColumn.Caption;
			}
			if (Grid is TreeEx treeEx)
			{
				TreeListColumn treeListColumn = treeEx.Columns[fieldName];
				if (treeListColumn == null)
				{
					return fieldName;
				}
				return treeListColumn.Caption;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return fieldName;
	}

	public int RowCount()
	{
		try
		{
			if (Grid == null)
			{
				return -1;
			}
			if (Grid is GridViewEx { RowCount: var rowCount })
			{
				return rowCount;
			}
			if (Grid is BandedGridViewEx { RowCount: var rowCount2 })
			{
				return rowCount2;
			}
			if (Grid is TreeEx)
			{
				if (DataSource == null)
				{
					return -1;
				}
				return DataSource.Rows.Count;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return -1;
	}

	public bool CheckExistColumnVisible(string fieldName)
	{
		try
		{
			if (Grid == null)
			{
				return false;
			}
			if (Grid is GridViewEx gridViewEx)
			{
				return gridViewEx.Columns[fieldName] != null;
			}
			if (Grid is BandedGridViewEx bandedGridViewEx)
			{
				return bandedGridViewEx.Columns[fieldName] != null;
			}
			if (Grid is TreeEx treeEx)
			{
				return treeEx.Columns[fieldName] != null;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public bool IsSelectedRow(int index)
	{
		try
		{
			if (Grid == null)
			{
				return false;
			}
			if (Grid is GridViewEx gridViewEx)
			{
				return gridViewEx.IsRowSelected(index);
			}
			if (Grid is BandedGridViewEx bandedGridViewEx)
			{
				return bandedGridViewEx.IsRowSelected(index);
			}
			if (Grid is TreeEx treeEx)
			{
				object keyID = DataSource.Rows[index][treeEx.KeyFieldName];
				TreeListNode node = treeEx.FindNodeByKeyID(keyID);
				return treeEx.Selection.Contains(node);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public void SetFocusRow(int rowIndex)
	{
		try
		{
			if (Grid != null)
			{
				if (Grid is GridViewEx gridViewEx)
				{
					gridViewEx.FocusedRowHandle = rowIndex;
					gridViewEx.MakeRowVisible(rowIndex);
				}
				else if (Grid is BandedGridViewEx bandedGridViewEx)
				{
					bandedGridViewEx.FocusedRowHandle = rowIndex;
					bandedGridViewEx.MakeRowVisible(rowIndex);
				}
				else if (Grid is TreeEx treeEx)
				{
					object keyID = DataSource.Rows[rowIndex][treeEx.KeyFieldName];
					TreeListNode focusedNode = treeEx.FindNodeByKeyID(keyID);
					treeEx.SetFocusedNode(focusedNode);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public void DeleteRow(int rowIndex)
	{
		try
		{
			if (DataSource.IsNotNull() || rowIndex >= 0)
			{
				int num = rowIndex;
				if (Grid is GridViewEx gridViewEx)
				{
					num = gridViewEx.GetDataSourceRowIndex(rowIndex);
				}
				else if (Grid is BandedGridViewEx bandedGridViewEx)
				{
					bandedGridViewEx.GetDataSourceRowIndex(rowIndex);
				}
				if (num >= 0)
				{
					DataSource.Rows.RemoveAt(rowIndex);
					DataSource.AcceptChanges();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public DataTable GetSelectedRowIndex()
	{
		try
		{
			if (Grid == null)
			{
				return new DataTable();
			}
			if (Grid is GridViewEx gridViewEx)
			{
				int[] selectedRows = gridViewEx.GetSelectedRows();
				DataTable dataTable = new DataTable
				{
					Columns = { 
					{
						"Index",
						typeof(int)
					} }
				};
				int[] array = selectedRows;
				foreach (int num in array)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["Index"] = num;
					dataTable.Rows.Add(dataRow);
				}
				dataTable.AcceptChanges();
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public void SetSelectedRow(int rowIndex, bool isSelected)
	{
		try
		{
			if (Grid == null)
			{
				return;
			}
			if (Grid is GridViewEx gridViewEx)
			{
				if (isSelected)
				{
					gridViewEx.SelectRow(rowIndex);
				}
				else
				{
					gridViewEx.UnselectRow(rowIndex);
				}
			}
			else if (Grid is BandedGridViewEx bandedGridViewEx)
			{
				if (isSelected)
				{
					bandedGridViewEx.SelectRow(rowIndex);
				}
				else
				{
					bandedGridViewEx.UnselectRow(rowIndex);
				}
			}
			else if (Grid is TreeEx treeEx)
			{
				object keyID = DataSource.Rows[rowIndex][treeEx.KeyFieldName];
				TreeListNode node = treeEx.FindNodeByKeyID(keyID);
				if (isSelected)
				{
					treeEx.SelectNode(node);
				}
				else
				{
					treeEx.UnselectNode(node);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public void SetColumnVisible(string fieldName, bool visble)
	{
		try
		{
			if (Grid == null)
			{
				return;
			}
			if (Grid is GridViewEx)
			{
				GridColumn gridColumn = (Grid as GridViewEx).Columns[fieldName];
				if (gridColumn != null)
				{
					gridColumn.Visible = visble;
				}
			}
			else if (Grid is BandedGridViewEx)
			{
				BandedGridColumn bandedGridColumn = (Grid as BandedGridViewEx).Columns[fieldName];
				if (bandedGridColumn != null)
				{
					bandedGridColumn.Visible = visble;
				}
			}
			else if (Grid is TreeEx)
			{
				TreeListColumn treeListColumn = (Grid as TreeEx).Columns[fieldName];
				if (treeListColumn != null)
				{
					treeListColumn.Visible = visble;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public void SetColumnVisible(int columnIndex, bool visble)
	{
		try
		{
			if (Grid == null)
			{
				return;
			}
			if (Grid is GridViewEx)
			{
				GridColumn gridColumn = (Grid as GridViewEx).Columns[columnIndex];
				if (gridColumn != null)
				{
					gridColumn.Visible = visble;
				}
			}
			else if (Grid is BandedGridViewEx)
			{
				BandedGridColumn bandedGridColumn = (Grid as BandedGridViewEx).Columns[columnIndex];
				if (bandedGridColumn != null)
				{
					bandedGridColumn.Visible = visble;
				}
			}
			else if (Grid is TreeEx)
			{
				TreeListColumn treeListColumn = (Grid as TreeEx).Columns[columnIndex];
				if (treeListColumn != null)
				{
					treeListColumn.Visible = visble;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public void SetCancelNextCellValueChangedScript(bool isCancel)
	{
		try
		{
			if (Grid != null)
			{
				if (Grid is GridViewEx)
				{
					(Grid as GridViewEx).CancelNextCellValueChangedScript = isCancel;
				}
				else if (Grid is BandedGridViewEx)
				{
					(Grid as BandedGridViewEx).CancelNextCellValueChangedScript = isCancel;
				}
				else if (Grid is TreeEx)
				{
					(Grid as TreeEx).CancelNextCellValueChangedScript = isCancel;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public void SetCancelNextSelectedChangedScript(bool isCancel)
	{
		try
		{
			if (Grid != null)
			{
				if (Grid is GridViewEx)
				{
					(Grid as GridViewEx).CancelNextSelectedChangedScript = isCancel;
				}
				else if (Grid is BandedGridViewEx)
				{
					(Grid as BandedGridViewEx).CancelNextSelectedChangedScript = isCancel;
				}
				else if (Grid is TreeEx)
				{
					(Grid as TreeEx).CancelNextSelectedChangedScript = isCancel;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public void SetCancelNextFocusObjectChangedScript(bool isCancel)
	{
		try
		{
			if (Grid != null)
			{
				if (Grid is GridViewEx)
				{
					(Grid as GridViewEx).CancelNextFocusObjectChangedScript = isCancel;
				}
				else if (Grid is BandedGridViewEx)
				{
					(Grid as BandedGridViewEx).CancelNextFocusObjectChangedScript = isCancel;
				}
				else if (Grid is TreeEx)
				{
					(Grid as TreeEx).CancelNextFocusObjectChangedScript = isCancel;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public string GetFocusedColumnName()
	{
		try
		{
			if (Grid == null)
			{
				return "";
			}
			if (Grid is GridViewEx gridViewEx)
			{
				return gridViewEx.FocusedColumn.FieldName;
			}
			if (Grid is TreeEx treeEx)
			{
				return treeEx.FocusedColumn.FieldName;
			}
			if (Grid is BandedGridViewEx bandedGridViewEx)
			{
				return bandedGridViewEx.FocusedColumn.FieldName;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public void SetFocusColumn(int rowIndex, string fieldName)
	{
		try
		{
			if (Grid == null)
			{
				return;
			}
			if (Grid is GridViewEx gridViewEx)
			{
				gridViewEx.FocusedRowHandle = rowIndex;
				GridColumn gridColumn = gridViewEx.Columns[fieldName];
				if (gridColumn != null)
				{
					gridViewEx.FocusedColumn = gridColumn;
					gridViewEx.ShowEditor();
				}
			}
			else if (Grid is BandedGridViewEx bandedGridViewEx)
			{
				bandedGridViewEx.FocusedRowHandle = rowIndex;
				BandedGridColumn bandedGridColumn = bandedGridViewEx.Columns[fieldName];
				if (bandedGridColumn != null)
				{
					bandedGridViewEx.FocusedColumn = bandedGridColumn;
					bandedGridViewEx.ShowEditor();
				}
			}
			else if (Grid is TreeEx treeEx)
			{
				object keyID = DataSource.Rows[rowIndex][treeEx.KeyFieldName];
				TreeListNode focusedNode = treeEx.FindNodeByKeyID(keyID);
				treeEx.SetFocusedNode(focusedNode);
				TreeListColumn treeListColumn = treeEx.Columns[fieldName];
				if (treeListColumn != null)
				{
					treeEx.FocusedColumn = treeListColumn;
					treeEx.ShowEditor();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public int GetFocusedRowHandle()
	{
		try
		{
			if (Grid == null)
			{
				return -1;
			}
			if (Grid is GridViewEx { FocusedRowHandle: var focusedRowHandle })
			{
				return focusedRowHandle;
			}
			if (Grid is BandedGridViewEx { FocusedRowHandle: var focusedRowHandle2 })
			{
				return focusedRowHandle2;
			}
			if (Grid is TreeEx)
			{
				return IndexEditRow;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return -1;
	}

	public void SelectAll()
	{
		try
		{
			if (Grid != null)
			{
				if (Grid is GridViewEx gridViewEx)
				{
					gridViewEx.SelectAll();
				}
				else if (Grid is BandedGridViewEx bandedGridViewEx)
				{
					bandedGridViewEx.SelectAll();
				}
				else if (Grid is TreeEx treeEx)
				{
					treeEx.SelectAll();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public void ClearSelection()
	{
		try
		{
			if (Grid != null)
			{
				if (Grid is GridViewEx gridViewEx)
				{
					gridViewEx.ClearSelection();
				}
				else if (Grid is BandedGridViewEx bandedGridViewEx)
				{
					bandedGridViewEx.ClearSelection();
				}
				else if (Grid is TreeEx treeEx)
				{
					treeEx.ClearSelection();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}
}
