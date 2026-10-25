using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.HttpClientHelper;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.MTO_Manufacturing;

public class frm_SpecialIssueForProductionByBatch : XtraFormEx
{
	private const string FormController = "SpecialIssueForBatch";

	private bool _edit;

	private IContainer components;

	private LabelLine lb_BatchIssue;

	private TextEditEx txt_BarCode;

	private GridViewEx gridView1;

	private SimpleButtonEx btRefresh;

	private SimpleButtonEx btn_GoodIssue;

	private SimpleButtonEx btn_Cancel;

	private GridViewEx gridView2;

	private GridControlEx grDocs;

	private GridViewEx gvDocs;

	private GridViewEx gridView3;

	private GridViewEx gridView4;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private async Task<DataTable> InsertRowByBatch(string batIssue)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SpecialIssueForBatch");
			return await _helper.PostAsync("InsertRowByBatch", batIssue);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<HttpResult> GoodIssue(List<DocumentsImpl> doc)
	{
		using HttpClientHelper<HttpResult> _helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "SpecialIssueForBatch");
		return await _helper.PostAsync("SaveDoc", doc);
	}

	private async void btn_GoodIssue_Click(object sender, EventArgs e)
	{
		try
		{
			_edit = false;
			_ = grDocs.DataSource;
			int[] selectedRows = gvDocs.GetSelectedRows();
			if (selectedRows.Count() > 0)
			{
				List<DocumentsImpl> list = new List<DocumentsImpl>();
				for (int i = 0; i < selectedRows.Count(); i++)
				{
					DocumentsImpl documentsImpl = new DocumentsImpl();
					documentsImpl.DocDate = DateTime.Today;
					documentsImpl.DocDueDate = DateTime.Today;
					documentsImpl.BPLId = 1;
					documentsImpl.BPL_IDAssignedToInvoice = 1;
					documentsImpl.Lines = new List<Document_LinesImpl>();
					Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
					document_LinesImpl.Quantity = Function.ParseDouble(gvDocs.GetRowCellValue(selectedRows[i], "Complete"));
					document_LinesImpl.ItemCode = Function.ToString(gvDocs.GetRowCellValue(selectedRows[i], "ItemCode"));
					document_LinesImpl.WhsCode = Function.ToString(gvDocs.GetRowCellValue(selectedRows[i], "Warehouse"));
					document_LinesImpl.AcctCode = "154100";
					document_LinesImpl.AcctCode = "154100";
					document_LinesImpl.BatchNumbers = new List<BatchNumbersImpl>();
					BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl();
					batchNumbersImpl.BatchNumber = Function.ToString(gvDocs.GetRowCellValue(selectedRows[i], "BatchIssue"));
					batchNumbersImpl.Quantity = document_LinesImpl.Quantity;
					document_LinesImpl.BatchNumbers.Add(batchNumbersImpl);
					documentsImpl.Lines.Add(document_LinesImpl);
					list.Add(documentsImpl);
				}
				HttpResult httpResult = await GoodIssue(list);
				if (httpResult.MessageCode == Enums.MessageCode.Success)
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_SpecialIssueForProductionByBatch()
	{
		InitializeComponent();
	}

	private void gridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void frm_SpecialIssueForProductionByBatch_Load(object sender, EventArgs e)
	{
		try
		{
			txt_BarCode.Focus();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDocs_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_edit = true;
			if (e.Column.FieldName == "ToCompletedQtyUoM")
			{
				double num = Function.ParseDouble(Function.ParseDouble(gvDocs.GetRowCellValue(gvDocs.FocusedRowHandle, "ToCompletedQtyUoM")) * Function.ParseDouble(gvDocs.GetRowCellValue(gvDocs.FocusedRowHandle, "Width")) * Function.ParseDouble(gvDocs.GetRowCellValue(gvDocs.FocusedRowHandle, "Length")) / 100.0);
				gvDocs.SetRowCellValue(gvDocs.FocusedRowHandle, "Complete", num);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDocs_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
	}

	private bool IsAllowingEdit(GridViewEx view, int row)
	{
		try
		{
			return Convert.ToBoolean(view.GetRowCellValue(row, "AllowEdit"));
		}
		catch
		{
			return false;
		}
	}

	private void gvDocs_ShowingEditor(object sender, CancelEventArgs e)
	{
		if (gvDocs.FocusedColumn.FieldName != "ToCompletedQtyUoM" && !IsAllowingEdit(gvDocs, gvDocs.FocusedRowHandle))
		{
			e.Cancel = true;
		}
	}

	private async void txt_BarCode_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void btn_GoodReceipt_Click(object sender, EventArgs e)
	{
		try
		{
			_edit = false;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (keyData == Keys.Return)
		{
			GetListIssue();
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private async void GetListIssue()
	{
		try
		{
			if (!string.IsNullOrEmpty(txt_BarCode.Text))
			{
				DataTable dataTable = await InsertRowByBatch(txt_BarCode.Text);
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					if (grDocs.DataSource is DataTable dataTable2 && dataTable2.Rows.Count > 0)
					{
						for (int i = 0; i < dataTable2.Rows.Count; i++)
						{
							if (txt_BarCode.Text.Trim().ToUpper() == Function.ToString(dataTable2.Rows[i]["BatchIssue"]).Trim().ToUpper())
							{
								WinCommons.Error(Enums.MessageCode.BardCodeIsUsed, "");
								return;
							}
						}
						DataRow dataRow = dataTable2.NewRow();
						for (int j = 0; j < dataTable2.Columns.Count; j++)
						{
							string columnName = dataTable2.Columns[j].ColumnName;
							dataRow[columnName] = dataTable.Rows[0][columnName];
						}
						dataTable2.Rows.Add(dataRow);
						dataTable2.AcceptChanges();
						grDocs.DataSource = dataTable2;
					}
					else
					{
						grDocs.DataSource = dataTable;
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.DoesNotExists, "");
				}
			}
			txt_BarCode.Text = "";
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_SpecialIssueForProductionByBatch_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (_edit)
			{
				if (MessageBox.Show(this, "Are you sure?", "Do you still want ... ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					e.Cancel = true;
				}
				_edit = false;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_SpecialIssueForProductionByBatch_KeyDown(object sender, KeyEventArgs e)
	{
	}

	private void btRefresh_Click(object sender, EventArgs e)
	{
		try
		{
			grDocs.DataSource = null;
			grDocs.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		try
		{
			Close();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gvDocs.RowCount > 0)
			{
				DataTable dataTable = grDocs.DataSource as DataTable;
				dataTable.Rows.RemoveAt(gvDocs.FocusedRowHandle);
				dataTable.AcceptChanges();
				grDocs.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.MTO_Manufacturing.frm_SpecialIssueForProductionByBatch));
		this.lb_BatchIssue = new Apzon.Controls.Controls.LabelLine();
		this.txt_BarCode = new Apzon.Controls.Editor.TextEditEx();
		this.gridView1 = new Apzon.Controls.Grid.GridViewEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_GoodIssue = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btRefresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.gridView2 = new Apzon.Controls.Grid.GridViewEx();
		this.grDocs = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDocs = new Apzon.Controls.Grid.GridViewEx();
		this.gridView3 = new Apzon.Controls.Grid.GridViewEx();
		this.gridView4 = new Apzon.Controls.Grid.GridViewEx();
		((System.ComponentModel.ISupportInitialize)this.txt_BarCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grDocs).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDocs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView4).BeginInit();
		base.SuspendLayout();
		this.lb_BatchIssue.EditControl = null;
		this.lb_BatchIssue.EditValue = "Bar Code";
		this.lb_BatchIssue.FieldName = null;
		this.lb_BatchIssue.HideWhenEmpty = false;
		this.lb_BatchIssue.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BatchIssue.Location = new System.Drawing.Point(4, 7);
		this.lb_BatchIssue.Name = "lb_BatchIssue";
		this.lb_BatchIssue.ReadOnly = false;
		this.lb_BatchIssue.SettingInVisible = false;
		this.lb_BatchIssue.SettingReadonly = false;
		this.lb_BatchIssue.ShowUnderLine = true;
		this.lb_BatchIssue.Size = new System.Drawing.Size(61, 20);
		this.lb_BatchIssue.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BatchIssue.TabIndex = 0;
		this.lb_BatchIssue.Utext = "Bar Code";
		this.txt_BarCode.EditControl = null;
		this.txt_BarCode.FieldName = null;
		this.txt_BarCode.IsSearchControl = false;
		this.txt_BarCode.Location = new System.Drawing.Point(64, 4);
		this.txt_BarCode.Name = "txt_BarCode";
		this.txt_BarCode.SettingInVisible = false;
		this.txt_BarCode.SettingReadonly = false;
		this.txt_BarCode.Size = new System.Drawing.Size(256, 20);
		this.txt_BarCode.TabIndex = 1;
		this.txt_BarCode.EditValueChanged += new System.EventHandler(this.txt_BarCode_EditValueChanged);
		this.gridView1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView1.ColumnKey = null;
		this.gridView1.DisableFormSetting = false;
		this.gridView1.DisplayException = false;
		this.gridView1.EditColumn = null;
		this.gridView1.FieldCurrency = null;
		this.gridView1.FieldRate = null;
		this.gridView1.FormId = null;
		this.gridView1.IgnoreFilter = false;
		this.gridView1.IndexEditRow = -1;
		this.gridView1.IndicatorWidth = 35;
		this.gridView1.IsApzTable = false;
		this.gridView1.IsInsertNewRow = false;
		this.gridView1.IsLoadingData = false;
		this.gridView1.IsMappingContextAction = false;
		this.gridView1.ItemId = null;
		this.gridView1.LstColumnAllowBatch = null;
		this.gridView1.LstColumnNotAllowBatch = null;
		this.gridView1.LstColumnNotEditable = null;
		this.gridView1.LstColumNotSetting = null;
		this.gridView1.Name = "gridView1";
		this.gridView1.UseBatchMod = false;
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(222, 376);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(104, 21);
		this.btn_Cancel.TabIndex = 5;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_GoodIssue.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_GoodIssue.EditControl = null;
		this.btn_GoodIssue.EditValue = "Good Issue";
		this.btn_GoodIssue.FieldName = null;
		this.btn_GoodIssue.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_GoodIssue.ImageOptions.Image");
		this.btn_GoodIssue.IsSearchControl = false;
		this.btn_GoodIssue.Location = new System.Drawing.Point(4, 375);
		this.btn_GoodIssue.Name = "btn_GoodIssue";
		this.btn_GoodIssue.ReadOnly = false;
		this.btn_GoodIssue.SettingInVisible = false;
		this.btn_GoodIssue.SettingReadonly = false;
		this.btn_GoodIssue.Size = new System.Drawing.Size(104, 22);
		this.btn_GoodIssue.TabIndex = 3;
		this.btn_GoodIssue.Text = "Good Issue";
		this.btn_GoodIssue.Click += new System.EventHandler(this.btn_GoodIssue_Click);
		this.btRefresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btRefresh.EditControl = null;
		this.btRefresh.EditValue = "Refresh";
		this.btRefresh.FieldName = null;
		this.btRefresh.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btRefresh.ImageOptions.Image");
		this.btRefresh.IsSearchControl = false;
		this.btRefresh.Location = new System.Drawing.Point(112, 376);
		this.btRefresh.Name = "btRefresh";
		this.btRefresh.ReadOnly = false;
		this.btRefresh.SettingInVisible = false;
		this.btRefresh.SettingReadonly = false;
		this.btRefresh.Size = new System.Drawing.Size(107, 21);
		this.btRefresh.TabIndex = 45;
		this.btRefresh.Text = "Refresh";
		this.btRefresh.Click += new System.EventHandler(this.btRefresh_Click);
		this.gridView2.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView2.ColumnKey = null;
		this.gridView2.DisableFormSetting = false;
		this.gridView2.DisplayException = false;
		this.gridView2.EditColumn = null;
		this.gridView2.FieldCurrency = null;
		this.gridView2.FieldRate = null;
		this.gridView2.FormId = null;
		this.gridView2.GridControl = this.grDocs;
		this.gridView2.IgnoreFilter = false;
		this.gridView2.IndexEditRow = -1;
		this.gridView2.IndicatorWidth = 35;
		this.gridView2.IsApzTable = false;
		this.gridView2.IsInsertNewRow = false;
		this.gridView2.IsLoadingData = false;
		this.gridView2.IsMappingContextAction = false;
		this.gridView2.ItemId = null;
		this.gridView2.LstColumnAllowBatch = null;
		this.gridView2.LstColumnNotAllowBatch = null;
		this.gridView2.LstColumnNotEditable = null;
		this.gridView2.LstColumNotSetting = null;
		this.gridView2.Name = "gridView2";
		this.gridView2.UseBatchMod = false;
		this.grDocs.AllowPasteExcel = true;
		this.grDocs.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grDocs.ClipboardData = null;
		this.grDocs.ContextMenuStrip = this.contextMenuStrip1;
		this.grDocs.Location = new System.Drawing.Point(4, 28);
		this.grDocs.MainView = this.gvDocs;
		this.grDocs.Name = "grDocs";
		this.grDocs.Size = new System.Drawing.Size(845, 342);
		this.grDocs.TabIndex = 2;
		this.grDocs.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[4] { this.gvDocs, this.gridView3, this.gridView4, this.gridView2 });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(134, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gvDocs.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDocs.ColumnKey = null;
		this.gvDocs.DisableFormSetting = false;
		this.gvDocs.DisplayException = false;
		this.gvDocs.EditColumn = null;
		this.gvDocs.FieldCurrency = null;
		this.gvDocs.FieldRate = null;
		this.gvDocs.FormId = null;
		this.gvDocs.GridControl = this.grDocs;
		this.gvDocs.IgnoreFilter = false;
		this.gvDocs.IndexEditRow = -1;
		this.gvDocs.IndicatorWidth = 40;
		this.gvDocs.IsApzTable = false;
		this.gvDocs.IsInsertNewRow = false;
		this.gvDocs.IsLoadingData = false;
		this.gvDocs.IsMappingContextAction = false;
		this.gvDocs.ItemId = null;
		this.gvDocs.LstColumnAllowBatch = null;
		this.gvDocs.LstColumnNotAllowBatch = null;
		this.gvDocs.LstColumnNotEditable = null;
		this.gvDocs.LstColumNotSetting = null;
		this.gvDocs.Name = "gvDocs";
		this.gvDocs.OptionsSelection.CheckBoxSelectorColumnWidth = 40;
		this.gvDocs.OptionsSelection.InvertSelection = true;
		this.gvDocs.OptionsSelection.MultiSelect = true;
		this.gvDocs.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gvDocs.OptionsView.ShowGroupPanel = false;
		this.gvDocs.UseBatchMod = false;
		this.gvDocs.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridView1_CustomDrawRowIndicator);
		this.gvDocs.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gvDocs_CustomRowCellEdit);
		this.gvDocs.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvDocs_ShowingEditor);
		this.gvDocs.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDocs_CellValueChanged);
		this.gridView3.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView3.ColumnKey = null;
		this.gridView3.DisableFormSetting = false;
		this.gridView3.DisplayException = false;
		this.gridView3.EditColumn = null;
		this.gridView3.FieldCurrency = null;
		this.gridView3.FieldRate = null;
		this.gridView3.FormId = null;
		this.gridView3.GridControl = this.grDocs;
		this.gridView3.IgnoreFilter = false;
		this.gridView3.IndexEditRow = -1;
		this.gridView3.IndicatorWidth = 35;
		this.gridView3.IsApzTable = false;
		this.gridView3.IsInsertNewRow = false;
		this.gridView3.IsLoadingData = false;
		this.gridView3.IsMappingContextAction = false;
		this.gridView3.ItemId = null;
		this.gridView3.LstColumnAllowBatch = null;
		this.gridView3.LstColumnNotAllowBatch = null;
		this.gridView3.LstColumnNotEditable = null;
		this.gridView3.LstColumNotSetting = null;
		this.gridView3.Name = "gridView3";
		this.gridView3.UseBatchMod = false;
		this.gridView4.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView4.ColumnKey = null;
		this.gridView4.DisableFormSetting = false;
		this.gridView4.DisplayException = false;
		this.gridView4.EditColumn = null;
		this.gridView4.FieldCurrency = null;
		this.gridView4.FieldRate = null;
		this.gridView4.FormId = null;
		this.gridView4.GridControl = this.grDocs;
		this.gridView4.IgnoreFilter = false;
		this.gridView4.IndexEditRow = -1;
		this.gridView4.IndicatorWidth = 35;
		this.gridView4.IsApzTable = false;
		this.gridView4.IsInsertNewRow = false;
		this.gridView4.IsLoadingData = false;
		this.gridView4.IsMappingContextAction = false;
		this.gridView4.ItemId = null;
		this.gridView4.LstColumnAllowBatch = null;
		this.gridView4.LstColumnNotAllowBatch = null;
		this.gridView4.LstColumnNotEditable = null;
		this.gridView4.LstColumNotSetting = null;
		this.gridView4.Name = "gridView4";
		this.gridView4.UseBatchMod = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(850, 402);
		base.Controls.Add(this.grDocs);
		base.Controls.Add(this.btRefresh);
		base.Controls.Add(this.btn_GoodIssue);
		base.Controls.Add(this.txt_BarCode);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.lb_BatchIssue);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SpecialIssueForProductionByBatch";
		this.Text = "Goods Issue and Receipt By Batch";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_SpecialIssueForProductionByBatch_FormClosing);
		base.Load += new System.EventHandler(this.frm_SpecialIssueForProductionByBatch_Load);
		base.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm_SpecialIssueForProductionByBatch_KeyDown);
		((System.ComponentModel.ISupportInitialize)this.txt_BarCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grDocs).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDocs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView4).EndInit();
		base.ResumeLayout(false);
	}
}
