using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Apzon.Commons.Enums;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Apzon.Controls.Grid;

public class CustomGridView : GridView
{
	protected RepositoryItem fRowPreviewEdit;

	private bool isRowPreviewSelected;

	private int postingEditorValue;

	public Enums.ChangeLog ChangeLogType { get; set; }

	public bool IsApzTable { get; set; }

	protected virtual int PreviewFieldHandle
	{
		get
		{
			int num = DataController.Columns.GetColumnIndex(PreviewFieldName);
			if (num == -1)
			{
				num = -2;
			}
			return num;
		}
	}

	protected override string ViewName => "CustomGridView";

	[Category("Appearance")]
	[Description("Gets or sets the repository item specifying the editor used to show row preview.")]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[TypeConverter("DevExpress.XtraGrid.TypeConverters.ColumnEditConverter, DevExpress.XtraGrid.v20.1.Design")]
	[Editor("DevExpress.XtraGrid.Design.ColumnEditEditor, DevExpress.XtraGrid.v20.1.Design", typeof(UITypeEditor))]
	public RepositoryItem PreviewRowEdit
	{
		get
		{
			return fRowPreviewEdit;
		}
		set
		{
			if (PreviewRowEdit != value)
			{
				RepositoryItem repositoryItem = fRowPreviewEdit;
				fRowPreviewEdit = value;
				if (ViewInfo is CustomGridViewInfo customGridViewInfo)
				{
					customGridViewInfo.UpdateRowPreviewEdit(fRowPreviewEdit);
				}
				repositoryItem?.Disconnect(this);
				if (PreviewRowEdit != null)
				{
					PreviewRowEdit.Connect(this);
				}
			}
		}
	}

	protected internal virtual bool IsRowPreviewSelected
	{
		get
		{
			return isRowPreviewSelected;
		}
		set
		{
			if (IsRowPreviewSelected != value)
			{
				isRowPreviewSelected = value;
				InvalidateRow(FocusedRowHandle);
			}
		}
	}

	protected override bool IsAutoHeight
	{
		get
		{
			if (PreviewRowEdit != null)
			{
				return true;
			}
			return base.IsAutoHeight;
		}
	}

	public override GridColumn FocusedColumn
	{
		get
		{
			return base.FocusedColumn;
		}
		set
		{
			base.FocusedColumn = value;
			if (FocusedColumn != null)
			{
				IsRowPreviewSelected = false;
			}
		}
	}

	public CustomGridView()
	{
		fRowPreviewEdit = null;
		isRowPreviewSelected = false;
	}

	public CustomGridView(GridControlEx grid)
		: base(grid)
	{
		fRowPreviewEdit = null;
		isRowPreviewSelected = false;
	}

	protected internal virtual void SetGridControlAccessMetod(GridControl newControl)
	{
		SetGridControl(newControl);
	}

	protected override void Dispose(bool disposing)
	{
		if (PreviewRowEdit != null)
		{
			PreviewRowEdit.Disconnect(this);
			fRowPreviewEdit = null;
		}
		base.Dispose(disposing);
	}

	public virtual object GetRowPreviewValue(int rowHandle)
	{
		object obj = null;
		if (PreviewFieldName.Length != 0 && DataController.IsReady)
		{
			obj = DataController.GetRowValue(rowHandle, PreviewFieldHandle);
		}
		if (obj is string)
		{
			return GetRowPreviewDisplayText(rowHandle);
		}
		return obj;
	}

	protected internal virtual void ActivatePreviewEditor()
	{
		if (PreviewRowEdit != null && ViewInfo.GetGridRowInfo(FocusedRowHandle) is GridDataRowInfo gridDataRowInfo)
		{
			Rectangle rowPreviewEditBounds = ((CustomGridViewInfo)ViewInfo).GetRowPreviewEditBounds(gridDataRowInfo);
			rowPreviewEditBounds.Offset(gridDataRowInfo.PreviewBounds.Location);
			UpdateEditor(PreviewRowEdit, new UpdateEditorInfoArgs(makeReadOnly: false, rowPreviewEditBounds, gridDataRowInfo.AppearancePreview, GetRowPreviewValue(FocusedRowHandle), ElementsLookAndFeel, string.Empty, null));
		}
	}

	protected internal virtual int RaiseMeasurePreviewHeightAccessMetod(int rowHandle)
	{
		return base.RaiseMeasurePreviewHeight(rowHandle);
	}

	protected override GridColumn GetNearestCanFocusedColumn(GridColumn col, int delta, bool allowChangeFocusedRow, KeyEventArgs e)
	{
		return base.GetNearestCanFocusedColumn(col, delta, allowChangeFocusedRow, e);
	}

	protected override bool PostEditor(bool causeValidation)
	{
		bool result = base.PostEditor(causeValidation);
		if (PreviewRowEdit == null)
		{
			return result;
		}
		if (postingEditorValue != 0)
		{
			return result;
		}
		try
		{
			postingEditorValue++;
			if (ActiveEditor == null || !EditingValueModified || fEditingCell != null)
			{
				return result;
			}
			if (causeValidation && !ValidateEditor())
			{
				return false;
			}
			SetRowPreviewValueCore(FocusedRowHandle, EditingValue);
			return result;
		}
		finally
		{
			postingEditorValue--;
		}
	}

	protected override void CloseEditor(bool causeValidation)
	{
		IsRowPreviewSelected = false;
		base.CloseEditor(causeValidation);
	}

	private void SetRowPreviewValueCore(int rowHandle, object value)
	{
		if (PreviewRowEdit == null || FocusedColumn != null)
		{
			return;
		}
		try
		{
			DataController.SetRowValue(rowHandle, PreviewFieldHandle, value);
			UpdateRowAutoHeight(rowHandle);
			if (rowHandle == FocusedRowHandle && FocusedColumn == null)
			{
				RefreshEditor(updateEditorValue: true);
				SetFocusedRowModified();
			}
			Invalidate();
		}
		catch
		{
		}
	}
}
