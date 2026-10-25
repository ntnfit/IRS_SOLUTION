using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls.Grid;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Controls.Form;

public class frm_BatchModification : XtraFormEx
{
	public GridViewEx currentGridview;

	private readonly List<GridCell> _modifiedCells = new List<GridCell>();

	private IContainer components;

	private GridControlEx grd_BatchModification;

	public GridViewEx gv_BatchModification;

	private SimpleButton btn_Cancel;

	private SimpleButton btn_OK;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteCellDataToolStripMenuItem;

	private ToolStripSeparator toolStripSeparator1;

	private ToolStripMenuItem cancelCellMod;

	public frm_BatchModification()
	{
		InitializeComponent();
	}

	public void InitGrid(GridViewEx view)
	{
		currentGridview = view;
		gv_BatchModification.Assign(view, copyEvents: false);
		gv_BatchModification.Name = "gv_BatchModification";
		if (view.GridControl is GridControlEx gridControlEx)
		{
			DataTable dataTable = gridControlEx.DataSourceTable.Clone();
			dataTable.Rows.Add(dataTable.NewRow());
			dataTable.AcceptChanges();
			grd_BatchModification.DataSource = dataTable;
			grd_BatchModification.RefreshDataSource();
			grd_BatchModification.MainView = gv_BatchModification;
		}
		gv_BatchModification.CellValueChanged += GvBatchModificationOnCellValueChanged;
		gv_BatchModification.RowCellStyle += GvBatchModificationOnRowCellStyle;
		gv_BatchModification.ShowingEditor += GvBatchModificationOnShowingEditor;
		if (currentGridview.LstColumnAllowBatch != null && currentGridview.LstColumnAllowBatch.Length != 0)
		{
			foreach (GridColumn column in gv_BatchModification.Columns)
			{
				if (currentGridview.LstColumnAllowBatch.Contains(column.FieldName))
				{
					column.Visible = true;
				}
				else
				{
					column.Visible = false;
				}
			}
			return;
		}
		if (currentGridview.LstColumnNotAllowBatch == null)
		{
			return;
		}
		string[] lstColumnNotAllowBatch = currentGridview.LstColumnNotAllowBatch;
		foreach (string fieldName in lstColumnNotAllowBatch)
		{
			GridColumn gridColumn2 = gv_BatchModification.Columns[fieldName];
			if (gridColumn2 != null)
			{
				gridColumn2.Visible = false;
			}
		}
	}

	private void GvBatchModificationOnShowingEditor(object sender, CancelEventArgs e)
	{
		if (currentGridview.LstColumnNotAllowBatch != null && currentGridview.LstColumnNotAllowBatch.Contains(gv_BatchModification.FocusedColumn.FieldName))
		{
			e.Cancel = true;
		}
	}

	private void GvBatchModificationOnRowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		if (CellExists(gv_BatchModification.GetDataSourceRowIndex(e.RowHandle), e.Column))
		{
			e.Appearance.BackColor = Color.Wheat;
		}
	}

	private void GvBatchModificationOnCellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (!CellExists(gv_BatchModification.GetDataSourceRowIndex(e.RowHandle), e.Column))
		{
			_modifiedCells.Add(new GridCell(gv_BatchModification.GetDataSourceRowIndex(e.RowHandle), e.Column));
		}
	}

	private bool CellExists(int sourceHandle, GridColumn col)
	{
		return _modifiedCells.FirstOrDefault((GridCell c) => c.Column == col && c.RowHandle == sourceHandle) != null;
	}

	private void UpdateSelectedRows(DataRow row)
	{
		try
		{
			List<DataRow> listDataSourceSelectedRow = currentGridview.GetListDataSourceSelectedRow();
			DataTable dataTable = currentGridview.GridControl.DatasourceTable();
			foreach (DataRow item in listDataSourceSelectedRow)
			{
				int rowHandle = currentGridview.GetRowHandle(dataTable.Rows.IndexOf(item));
				foreach (GridCell modifiedCell in _modifiedCells)
				{
					string fieldName = modifiedCell.Column.FieldName;
					if (!dataTable.Columns.Contains(fieldName) || (currentGridview.LstColumnNotAllowBatch != null && currentGridview.LstColumnNotAllowBatch.Contains(fieldName)))
					{
						continue;
					}
					DataColumn dataColumn = dataTable.Columns[fieldName];
					GridColumnEx gridColumnEx = currentGridview.Columns[fieldName] as GridColumnEx;
					if (dataColumn.ColumnName == fieldName && (gridColumnEx == null || !gridColumnEx.PrimaryKey))
					{
						currentGridview.FocusedColumn = gridColumnEx;
						currentGridview.FocusedRowHandle = rowHandle;
						currentGridview.ShowEditor();
						if (currentGridview.ActiveEditor != null && (!(currentGridview.ActiveEditor is ButtonEdit) || ((ButtonEdit)currentGridview.ActiveEditor).Properties.TextEditStyle == TextEditStyles.Standard || AnyButtonVisible((ButtonEdit)currentGridview.ActiveEditor)))
						{
							currentGridview.SetRowCellValue(rowHandle, dataColumn.ColumnName, row[dataColumn.ColumnName]);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_BatchModification_Load(object sender, EventArgs e)
	{
	}

	private bool AnyButtonVisible(ButtonEdit button)
	{
		bool result = false;
		for (int i = 0; i < button.Properties.Buttons.Count; i++)
		{
			EditorButton editorButton = button.Properties.Buttons[i];
			if (editorButton.Visible && editorButton.Enabled)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		UpdateSelectedRows(gv_BatchModification.GetDataRow(gv_BatchModification.FocusedRowHandle));
		Close();
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void deleteCellDataToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int focusedRowHandle = gv_BatchModification.FocusedRowHandle;
			GridColumn focusedColumn = gv_BatchModification.FocusedColumn;
			gv_BatchModification.SetRowCellValue(focusedRowHandle, focusedColumn, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cancelCellToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int focusedRowHandle = gv_BatchModification.FocusedRowHandle;
			GridColumn focusedColumn = gv_BatchModification.FocusedColumn;
			gv_BatchModification.SetRowCellValue(focusedRowHandle, focusedColumn, null);
			_modifiedCells.RemoveAll((GridCell t) => t.RowHandle == focusedRowHandle && t.Column == focusedColumn);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
	{
		int focusedRowHandle = gv_BatchModification.FocusedRowHandle;
		GridColumn focusedColumn = gv_BatchModification.FocusedColumn;
		if (_modifiedCells.Any((GridCell t) => t.RowHandle == focusedRowHandle && t.Column == focusedColumn))
		{
			toolStripSeparator1.Visible = true;
			cancelCellMod.Visible = true;
		}
		else
		{
			toolStripSeparator1.Visible = false;
			cancelCellMod.Visible = false;
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.grd_BatchModification = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip();
		this.deleteCellDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.cancelCellMod = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_BatchModification = new Apzon.Controls.Grid.GridViewEx();
		this.btn_Cancel = new DevExpress.XtraEditors.SimpleButton();
		this.btn_OK = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.grd_BatchModification).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_BatchModification).BeginInit();
		base.SuspendLayout();
		this.grd_BatchModification.AllowDrop = true;
		this.grd_BatchModification.AllowPasteExcel = true;
		this.grd_BatchModification.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_BatchModification.ClipboardData = null;
		this.grd_BatchModification.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_BatchModification.Location = new System.Drawing.Point(0, -2);
		this.grd_BatchModification.MainView = this.gv_BatchModification;
		this.grd_BatchModification.Name = "grd_BatchModification";
		this.grd_BatchModification.Size = new System.Drawing.Size(800, 269);
		this.grd_BatchModification.TabIndex = 1;
		this.grd_BatchModification.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_BatchModification });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.deleteCellDataToolStripMenuItem, this.toolStripSeparator1, this.cancelCellMod });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(205, 54);
		this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
		this.deleteCellDataToolStripMenuItem.Name = "deleteCellDataToolStripMenuItem";
		this.deleteCellDataToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
		this.deleteCellDataToolStripMenuItem.Text = "Delete Cell Data";
		this.deleteCellDataToolStripMenuItem.Click += new System.EventHandler(this.deleteCellDataToolStripMenuItem_Click);
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		this.toolStripSeparator1.Size = new System.Drawing.Size(201, 6);
		this.cancelCellMod.Name = "cancelCellMod";
		this.cancelCellMod.Size = new System.Drawing.Size(204, 22);
		this.cancelCellMod.Text = "Cancel Cell Modification";
		this.cancelCellMod.Click += new System.EventHandler(this.cancelCellToolStripMenuItem_Click);
		this.gv_BatchModification.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_BatchModification.ColumnKey = null;
		this.gv_BatchModification.DisableFormSetting = false;
		this.gv_BatchModification.DisplayException = false;
		this.gv_BatchModification.EditColumn = null;
		this.gv_BatchModification.FieldCurrency = null;
		this.gv_BatchModification.FieldRate = null;
		this.gv_BatchModification.FormId = "oBatchModification";
		this.gv_BatchModification.GridControl = this.grd_BatchModification;
		this.gv_BatchModification.IndexEditRow = -1;
		this.gv_BatchModification.IndicatorWidth = 35;
		this.gv_BatchModification.IsApzTable = false;
		this.gv_BatchModification.IsInsertNewRow = false;
		this.gv_BatchModification.IsLoadingData = false;
		this.gv_BatchModification.ItemId = "ODATA";
		this.gv_BatchModification.LstColumnAllowBatch = null;
		this.gv_BatchModification.LstColumnNotAllowBatch = null;
		this.gv_BatchModification.LstColumNotSetting = null;
		this.gv_BatchModification.Name = "gv_BatchModification";
		this.gv_BatchModification.OptionsView.ColumnAutoWidth = false;
		this.gv_BatchModification.OptionsView.ShowGroupPanel = false;
		this.gv_BatchModification.UseBatchMod = false;
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.Location = new System.Drawing.Point(78, 271);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 22);
		this.btn_Cancel.TabIndex = 4;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.Location = new System.Drawing.Point(1, 271);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 22);
		this.btn_OK.TabIndex = 3;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(800, 297);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.grd_BatchModification);
		base.FormId = "oBatchModification";
		base.Name = "frm_BatchModification";
		base.ShowIcon = false;
		this.Text = "Batch Modification";
		base.Load += new System.EventHandler(this.frm_BatchModification_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_BatchModification).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_BatchModification).EndInit();
		base.ResumeLayout(false);
	}
}
