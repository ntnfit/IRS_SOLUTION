using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Dashboard;

public class frm_DashboardCategories : XtraFormEx
{
	private IContainer components;

	private PanelControl panelControl1;

	private GridControlEx grd_Categories;

	private GridViewEx gv_Categories;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn3;

	private RepositoryItemCheckEdit rep_Valid;

	private SimpleButtonEx btn_Open;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_Save;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private GridColumnEx gridColumn2;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btn_Cancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_DashboardCategories()
	{
		InitializeComponent();
	}

	private void gv_Categories_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void frm_DashboardCategories_Load(object sender, EventArgs e)
	{
		try
		{
			LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void LoadData()
	{
		try
		{
			DataTable dataTable = new DataTable();
			using (HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DashboardCategories"))
			{
				dataTable = await helper.GetAsync("GetData");
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["Valid"] = "false";
			dataRow["CatId"] = 0;
			dataTable.Rows.Add(dataRow);
			grd_Categories.DataSource = dataTable;
			grd_Categories.RefreshDataSource();
			FormMode = Enums.FormViewModeType.View;
			btn_Save.Enabled = true;
			btn_Cancel.Enabled = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int catId = Function.ParseInt(gv_Categories.GetFocusedRowCellValue("CatId"));
			string value = "";
			using (HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "DashboardCategories"))
			{
				value = httpClientHelper.Get("CheckExistDashboardTab", new
				{
					CatId = catId
				});
			}
			if (string.IsNullOrEmpty(value))
			{
				WinCommons.Error(Enums.MessageCode.CannotDeleteThisRecordIsBeingUsed, "");
				return;
			}
			gv_Categories.DeleteSelectedRows();
			FormMode = Enums.FormViewModeType.Update;
			btn_Cancel.Enabled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Categories_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		contextMenuStrip1.Show(grd_Categories, e.Point);
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			DataTable dataTable = (DataTable)grd_Categories.DataSource;
			string text = "";
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				DataTable dataTable2 = dataTable.Clone();
				int num = Function.ParseInt(dataTable.AsEnumerable().Max((DataRow r) => Function.ParseInt(Function.ToString(r["CatId"])))) + 1;
				foreach (DataRow row in dataTable.Rows)
				{
					string value = Function.ToString(row["Name"]);
					int num2 = Function.ParseInt(Function.ToString(row["CatId"]));
					if (!string.IsNullOrEmpty(value))
					{
						DataRow dataRow2 = dataTable2.NewRow();
						if (num2 > 0)
						{
							dataRow2["CatId"] = row["CatId"];
							dataRow2["Name"] = row["Name"];
							dataRow2["Valid"] = row["Valid"];
						}
						else
						{
							dataRow2["CatId"] = num;
							dataRow2["Name"] = row["Name"];
							dataRow2["Valid"] = Function.ParseBool(row["Valid"]);
							num++;
						}
						dataTable2.Rows.Add(dataRow2);
					}
				}
				if (!(from r in dataTable2.AsEnumerable()
					group r by r["Name"] into gr
					where gr.Count() > 1
					select gr).ToList().Any())
				{
					using (HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "DashboardCategories"))
					{
						text = httpClientHelper.Post("SaveData", dataTable2);
					}
					if (string.IsNullOrEmpty(text))
					{
						WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
						LoadData();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, text);
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.NameCannotBeDuplicate, "");
				}
			}
			else
			{
				using (HttpClientHelper<string> httpClientHelper2 = new HttpClientHelper<string>(Configuration.ApiServer, "DashboardCategories"))
				{
					text = httpClientHelper2.Post("SaveData", dataTable);
				}
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					LoadData();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gv_Categories_InitNewRow(object sender, InitNewRowEventArgs e)
	{
		gv_Categories.SetFocusedRowCellValue("Valid", false);
		FormMode = Enums.FormViewModeType.Update;
		btn_Cancel.Enabled = true;
	}

	private void gv_Categories_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
		btn_Cancel.Enabled = true;
	}

	private async void grd_Categories_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				ExtenseionMethods.PasteFromExcelToDataTable(grd_Categories, gv_Categories, new List<string> { "Name" });
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Open_Click(object sender, EventArgs e)
	{
		try
		{
			int catId = Function.ParseInt(gv_Categories.GetFocusedRowCellValue("CatId"));
			frm_DashboardTab frm_DashboardTab2 = new frm_DashboardTab();
			frm_DashboardTab2.MdiParent = MdiParent;
			frm_DashboardTab2.catId = catId;
			frm_DashboardTab2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Categories_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			DataTable dataTable = (DataTable)grd_Categories.DataSource;
			if (!string.IsNullOrEmpty(Function.ToString(gv_Categories.GetDataRow(gv_Categories.RowCount - 1)["Name"])) && e.KeyCode == Keys.Tab)
			{
				DataRow row = dataTable.NewRow();
				dataTable.Rows.Add(row);
				grd_Categories.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.btn_Open = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Categories = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Categories = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Valid = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Categories).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Categories).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Valid).BeginInit();
		base.SuspendLayout();
		this.panelControl1.Controls.Add(this.btn_Open);
		this.panelControl1.Controls.Add(this.btn_Cancel);
		this.panelControl1.Controls.Add(this.btn_Save);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelControl1.Location = new System.Drawing.Point(0, 335);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(492, 47);
		this.panelControl1.TabIndex = 0;
		this.btn_Open.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Open.EditControl = null;
		this.btn_Open.EditValue = "New Tab";
		this.btn_Open.FieldName = null;
		this.btn_Open.ImageOptions.Image = Apzon.Forms.Properties.Resources.withtextwrapping_topright_16x16;
		this.btn_Open.IsSearchControl = false;
		this.btn_Open.Location = new System.Drawing.Point(406, 12);
		this.btn_Open.Name = "btn_Open";
		this.btn_Open.ReadOnly = false;
		this.btn_Open.SettingInVisible = false;
		this.btn_Open.SettingReadonly = false;
		this.btn_Open.Size = new System.Drawing.Size(75, 23);
		this.btn_Open.TabIndex = 0;
		this.btn_Open.Text = "New Tab";
		this.btn_Open.Click += new System.EventHandler(this.btn_Open_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(93, 12);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 0;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(12, 12);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(75, 23);
		this.btn_Save.TabIndex = 0;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.grd_Categories.AllowPasteExcel = true;
		this.grd_Categories.ClipboardData = null;
		this.grd_Categories.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Categories.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Categories.Location = new System.Drawing.Point(0, 0);
		this.grd_Categories.MainView = this.gv_Categories;
		this.grd_Categories.Name = "grd_Categories";
		this.grd_Categories.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_Valid });
		this.grd_Categories.Size = new System.Drawing.Size(492, 335);
		this.grd_Categories.TabIndex = 1;
		this.grd_Categories.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Categories });
		this.grd_Categories.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grd_Categories_EditorKeyDown);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.deleteToolStripMenuItem.Text = "Remove";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.gv_Categories.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Categories.ColumnKey = null;
		this.gv_Categories.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gridColumn2, this.gridColumn1, this.gridColumn3 });
		this.gv_Categories.DisableFormSetting = false;
		this.gv_Categories.DisplayException = false;
		this.gv_Categories.EditColumn = null;
		this.gv_Categories.FieldCurrency = null;
		this.gv_Categories.FieldRate = null;
		this.gv_Categories.FormId = "oDashboardCategories";
		this.gv_Categories.GridControl = this.grd_Categories;
		this.gv_Categories.IgnoreFilter = false;
		this.gv_Categories.IndexEditRow = -1;
		this.gv_Categories.IndicatorWidth = 30;
		this.gv_Categories.IsApzTable = false;
		this.gv_Categories.IsInsertNewRow = false;
		this.gv_Categories.IsLoadingData = false;
		this.gv_Categories.IsMappingContextAction = false;
		this.gv_Categories.ItemId = "APZ_ODCT";
		this.gv_Categories.LstColumnAllowBatch = null;
		this.gv_Categories.LstColumnNotAllowBatch = null;
		this.gv_Categories.LstColumnNotEditable = null;
		this.gv_Categories.LstColumNotSetting = null;
		this.gv_Categories.Name = "gv_Categories";
		this.gv_Categories.OptionsView.ColumnAutoWidth = false;
		this.gv_Categories.OptionsView.ShowGroupPanel = false;
		this.gv_Categories.UseBatchMod = false;
		this.gv_Categories.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Categories_CustomDrawRowIndicator);
		this.gv_Categories.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_Categories_PopupMenuShowing);
		this.gv_Categories.InitNewRow += new DevExpress.XtraGrid.Views.Grid.InitNewRowEventHandler(this.gv_Categories_InitNewRow);
		this.gv_Categories.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Categories_CellValueChanged);
		this.gv_Categories.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gv_Categories_KeyDown);
		this.gridColumn2.Caption = "CategoryID";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "CatId";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn1.Caption = "Category Name";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "Name";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn1.Width = 400;
		this.gridColumn3.Caption = "Valid";
		this.gridColumn3.ColumnEdit = this.rep_Valid;
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "Valid";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 1;
		this.gridColumn3.Width = 60;
		this.rep_Valid.AutoHeight = false;
		this.rep_Valid.Name = "rep_Valid";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(492, 382);
		base.Controls.Add(this.grd_Categories);
		base.Controls.Add(this.panelControl1);
		base.FormId = "oDashboardCategories";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_DashboardCategories";
		this.Text = "Dashboard Categories";
		base.Load += new System.EventHandler(this.frm_DashboardCategories_Load);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Categories).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Categories).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Valid).EndInit();
		base.ResumeLayout(false);
	}
}
