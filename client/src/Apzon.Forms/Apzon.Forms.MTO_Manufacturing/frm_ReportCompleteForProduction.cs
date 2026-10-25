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
using Apzon.Entities.Models.Documents;
using Apzon.HttpClientHelper;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.MTO_Manufacturing;

public class frm_ReportCompleteForProduction : XtraFormEx
{
	private const string FormController = "ReportCompleteForProduction";

	private bool _edit;

	private IContainer components;

	private SimpleButtonEx btn_GoodIssue;

	private TextEditEx txt_BarCode;

	private LabelLine lb_BatchIssue;

	private SimpleButtonEx btn_ReportComplete;

	private GridControlEx grDocs;

	private GridViewEx gvDocs;

	private TextEditEx txtBarCode;

	private LabelLine labelLine1;

	private SimpleButtonEx simpleButton1;

	private SimpleButtonEx simpleButton3;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteToolStripMenuItem;

	public frm_ReportCompleteForProduction()
	{
		InitializeComponent();
	}

	private async Task<DataTable> InsertRowByBatch(string batIssue)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SpecialIssueForBatch");
			return await _helper.PostAsync("InsertRowByBatchToReceipt", batIssue);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async void GetListRecipt()
	{
		try
		{
			if (!string.IsNullOrEmpty(txtBarCode.Text))
			{
				DataTable dataTable = await InsertRowByBatch(txtBarCode.Text);
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					if (grDocs.DataSource is DataTable dataTable2 && dataTable2.Rows.Count > 0)
					{
						for (int i = 0; i < dataTable2.Rows.Count; i++)
						{
							if (txt_BarCode.Text.Trim().ToUpper() == Function.ToString(dataTable2.Rows[i]["BatchComplete"]).Trim().ToUpper())
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

	private async Task<string> GoodIssue(List<DocumentsImpl> doc)
	{
		try
		{
			using HttpClientHelper<DocumentsImpl> _helper = new HttpClientHelper<DocumentsImpl>(Configuration.ApiServer, "SpecialIssueForBatch");
			return await _helper.PostParamsAsJsonAsync("SaveDocReceipt", doc);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return "Error!";
		}
	}

	private async Task<DataTable> GetDocsByBarCode(string barCode)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ReportCompleteForProduction");
			return await _helper.PostAsync("GetDocsByBarCode", barCode);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (keyData == Keys.Return)
		{
			GetListRecipt();
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void simpleButton1_Click(object sender, EventArgs e)
	{
		try
		{
			if (DialogResult.Yes == XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRefreshData, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
			{
				grDocs.DataSource = null;
				grDocs.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void simpleButton3_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void frm_ReportCompleteForProduction_FormClosing(object sender, FormClosingEventArgs e)
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

	private void txtBarCode_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void frm_ReportCompleteForProduction_Load(object sender, EventArgs e)
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

	private void gvDocs_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_ReportComplete_Click(object sender, EventArgs e)
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
					document_LinesImpl.Quantity = Function.ParseDouble(gvDocs.GetRowCellValue(selectedRows[i], "Quantity"));
					document_LinesImpl.ItemCode = Function.ToString(gvDocs.GetRowCellValue(selectedRows[i], "ItemCode"));
					document_LinesImpl.WhsCode = Function.ToString(gvDocs.GetRowCellValue(selectedRows[i], "Warehouse"));
					document_LinesImpl.AcctCode = "154100";
					document_LinesImpl.AcctCode = "154100";
					document_LinesImpl.BatchNumbers = new List<BatchNumbersImpl>();
					BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl();
					batchNumbersImpl.BatchNumber = Function.ToString(gvDocs.GetRowCellValue(selectedRows[i], "BatchComplete"));
					batchNumbersImpl.Quantity = document_LinesImpl.Quantity;
					document_LinesImpl.BatchNumbers.Add(batchNumbersImpl);
					documentsImpl.Lines.Add(document_LinesImpl);
					list.Add(documentsImpl);
				}
				string text = await GoodIssue(list);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.MTO_Manufacturing.frm_ReportCompleteForProduction));
		this.btn_GoodIssue = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_BarCode = new Apzon.Controls.Editor.TextEditEx();
		this.lb_BatchIssue = new Apzon.Controls.Controls.LabelLine();
		this.btn_ReportComplete = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grDocs = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDocs = new Apzon.Controls.Grid.GridViewEx();
		this.txtBarCode = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.simpleButton1 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton3 = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.txt_BarCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grDocs).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDocs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBarCode.Properties).BeginInit();
		base.SuspendLayout();
		this.btn_GoodIssue.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_GoodIssue.EditControl = null;
		this.btn_GoodIssue.EditValue = "Good Issue";
		this.btn_GoodIssue.FieldName = null;
		this.btn_GoodIssue.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_GoodIssue.ImageOptions.Image");
		this.btn_GoodIssue.IsSearchControl = false;
		this.btn_GoodIssue.Location = new System.Drawing.Point(-80, 459);
		this.btn_GoodIssue.Name = "btn_GoodIssue";
		this.btn_GoodIssue.ReadOnly = false;
		this.btn_GoodIssue.SettingInVisible = false;
		this.btn_GoodIssue.SettingReadonly = false;
		this.btn_GoodIssue.Size = new System.Drawing.Size(14, 22);
		this.btn_GoodIssue.TabIndex = 7;
		this.btn_GoodIssue.Text = "Good Issue";
		this.txt_BarCode.EditControl = null;
		this.txt_BarCode.FieldName = null;
		this.txt_BarCode.IsSearchControl = false;
		this.txt_BarCode.Location = new System.Drawing.Point(-22, -19);
		this.txt_BarCode.Name = "txt_BarCode";
		this.txt_BarCode.SettingInVisible = false;
		this.txt_BarCode.SettingReadonly = false;
		this.txt_BarCode.Size = new System.Drawing.Size(166, 20);
		this.txt_BarCode.TabIndex = 5;
		this.lb_BatchIssue.EditControl = null;
		this.lb_BatchIssue.EditValue = "Bar Code";
		this.lb_BatchIssue.FieldName = null;
		this.lb_BatchIssue.HideWhenEmpty = false;
		this.lb_BatchIssue.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BatchIssue.Location = new System.Drawing.Point(-82, -15);
		this.lb_BatchIssue.Name = "lb_BatchIssue";
		this.lb_BatchIssue.ReadOnly = false;
		this.lb_BatchIssue.SettingInVisible = false;
		this.lb_BatchIssue.SettingReadonly = false;
		this.lb_BatchIssue.ShowUnderLine = true;
		this.lb_BatchIssue.Size = new System.Drawing.Size(10, 20);
		this.lb_BatchIssue.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BatchIssue.TabIndex = 4;
		this.lb_BatchIssue.Utext = "Bar Code";
		this.btn_ReportComplete.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_ReportComplete.EditControl = null;
		this.btn_ReportComplete.EditValue = "Report Complete";
		this.btn_ReportComplete.FieldName = null;
		this.btn_ReportComplete.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ReportComplete.ImageOptions.Image");
		this.btn_ReportComplete.IsSearchControl = false;
		this.btn_ReportComplete.Location = new System.Drawing.Point(3, 431);
		this.btn_ReportComplete.Name = "btn_ReportComplete";
		this.btn_ReportComplete.ReadOnly = false;
		this.btn_ReportComplete.SettingInVisible = false;
		this.btn_ReportComplete.SettingReadonly = false;
		this.btn_ReportComplete.Size = new System.Drawing.Size(129, 22);
		this.btn_ReportComplete.TabIndex = 11;
		this.btn_ReportComplete.Text = "Report Complete";
		this.btn_ReportComplete.Click += new System.EventHandler(this.btn_ReportComplete_Click);
		this.grDocs.AllowPasteExcel = true;
		this.grDocs.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grDocs.ClipboardData = null;
		this.grDocs.ContextMenuStrip = this.contextMenuStrip1;
		this.grDocs.Location = new System.Drawing.Point(0, 34);
		this.grDocs.MainView = this.gvDocs;
		this.grDocs.Name = "grDocs";
		this.grDocs.Size = new System.Drawing.Size(848, 391);
		this.grDocs.TabIndex = 10;
		this.grDocs.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDocs });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(108, 26);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
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
		this.gvDocs.OptionsSelection.MultiSelect = true;
		this.gvDocs.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gvDocs.OptionsView.ShowGroupPanel = false;
		this.gvDocs.UseBatchMod = false;
		this.gvDocs.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDocs_CustomDrawRowIndicator);
		this.txtBarCode.EditControl = null;
		this.txtBarCode.FieldName = null;
		this.txtBarCode.IsSearchControl = false;
		this.txtBarCode.Location = new System.Drawing.Point(63, 8);
		this.txtBarCode.Name = "txtBarCode";
		this.txtBarCode.SettingInVisible = false;
		this.txtBarCode.SettingReadonly = false;
		this.txtBarCode.Size = new System.Drawing.Size(256, 20);
		this.txtBarCode.TabIndex = 9;
		this.txtBarCode.EditValueChanged += new System.EventHandler(this.txtBarCode_EditValueChanged);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Bar Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(3, 11);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(61, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 8;
		this.labelLine1.Utext = "Bar Code";
		this.simpleButton1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.simpleButton1.EditControl = null;
		this.simpleButton1.EditValue = "Refresh";
		this.simpleButton1.FieldName = null;
		this.simpleButton1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButton1.ImageOptions.Image");
		this.simpleButton1.IsSearchControl = false;
		this.simpleButton1.Location = new System.Drawing.Point(135, 431);
		this.simpleButton1.Name = "simpleButton1";
		this.simpleButton1.ReadOnly = false;
		this.simpleButton1.SettingInVisible = false;
		this.simpleButton1.SettingReadonly = false;
		this.simpleButton1.Size = new System.Drawing.Size(113, 22);
		this.simpleButton1.TabIndex = 51;
		this.simpleButton1.Text = "Refresh";
		this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
		this.simpleButton3.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.simpleButton3.EditControl = null;
		this.simpleButton3.EditValue = "Cancel";
		this.simpleButton3.FieldName = null;
		this.simpleButton3.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButton3.ImageOptions.Image");
		this.simpleButton3.IsSearchControl = false;
		this.simpleButton3.Location = new System.Drawing.Point(252, 431);
		this.simpleButton3.Name = "simpleButton3";
		this.simpleButton3.ReadOnly = false;
		this.simpleButton3.SettingInVisible = false;
		this.simpleButton3.SettingReadonly = false;
		this.simpleButton3.Size = new System.Drawing.Size(110, 22);
		this.simpleButton3.TabIndex = 50;
		this.simpleButton3.Text = "Cancel";
		this.simpleButton3.Click += new System.EventHandler(this.simpleButton3_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(847, 463);
		base.Controls.Add(this.simpleButton1);
		base.Controls.Add(this.simpleButton3);
		base.Controls.Add(this.btn_ReportComplete);
		base.Controls.Add(this.grDocs);
		base.Controls.Add(this.txtBarCode);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btn_GoodIssue);
		base.Controls.Add(this.txt_BarCode);
		base.Controls.Add(this.lb_BatchIssue);
		base.Name = "frm_ReportCompleteForProduction";
		this.Text = "Report Complete For Production";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_ReportCompleteForProduction_FormClosing);
		base.Load += new System.EventHandler(this.frm_ReportCompleteForProduction_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_BarCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grDocs).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDocs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBarCode.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
