using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Mrps;

public class frm_InputGenerateForecastItemPropeties : XtraFormEx
{
	protected readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public string IgnorePropeties = "Y";

	public string CompareType = "";

	public string Propeties = "";

	private IContainer components;

	private CheckEditEx chk_Ignore;

	private LabelLine lbLink;

	private CheckEditEx chkOr;

	private CheckEditEx chkAnd;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private SimpleButtonEx btnOk;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnClearSelection;

	private SimpleButtonEx btnInvertSelection;

	private SimpleButtonEx btnSelectAll;

	private GridColumnEx gcCode;

	private GridColumnEx gcChecked;

	private RepositoryItemCheckEdit rChecked;

	public frm_InputGenerateForecastItemPropeties()
	{
		InitializeComponent();
	}

	private async void frm_InputGenerateForecastItemPropeties_Load(object sender, EventArgs e)
	{
		try
		{
			chkOr.Checked = CompareType.Equals("or") || string.IsNullOrEmpty(CompareType);
			chkAnd.Checked = CompareType.Equals("and");
			chk_Ignore.Checked = IgnorePropeties.Equals("Y");
			ChangeIgnore(chk_Ignore.Checked);
			await GetDataPropeties();
			DialogResult = DialogResult.None;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ChangeIgnore(bool value)
	{
		RepositoryItemCheckEdit properties = chkOr.Properties;
		RepositoryItemCheckEdit properties2 = chkAnd.Properties;
		bool flag = (gvDoc.ReadOnly = value);
		bool readOnly = (properties2.ReadOnly = flag);
		properties.ReadOnly = readOnly;
		SimpleButtonEx simpleButtonEx = btnClearSelection;
		SimpleButtonEx simpleButtonEx2 = btnInvertSelection;
		flag = (btnSelectAll.Enabled = !value);
		readOnly = (simpleButtonEx2.Enabled = flag);
		simpleButtonEx.Enabled = readOnly;
	}

	private async Task GetDataPropeties()
	{
		DataTable dataTable = await _httpClient.Items.LoadProperties();
		if (dataTable.IsNotNull())
		{
			string[] source = Propeties.Split(',');
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				if (source.Contains(Function.ToString(dataTable.Rows[i]["Code"])))
				{
					dataTable.Rows[i]["Checked"] = "Y";
				}
			}
		}
		grdDoc.DataSource = dataTable;
		grdDoc.DataSourceTable.AcceptChanges();
	}

	private void chk_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (sender is CheckEdit checkEdit)
			{
				if (checkEdit.Name.Equals(chk_Ignore.Name))
				{
					ChangeIgnore(checkEdit.Checked);
				}
				else if (checkEdit.Name.Equals(chkOr.Name))
				{
					chkAnd.Checked = !checkEdit.Checked;
				}
				else if (checkEdit.Name.Equals(chkAnd.Name))
				{
					chkOr.Checked = !checkEdit.Checked;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btnOk_Click(object sender, EventArgs e)
	{
		try
		{
			IgnorePropeties = Function.ToString(chk_Ignore.EditValue);
			CompareType = (chkOr.Checked ? "or" : "and");
			grdDoc.DataSourceTable.AcceptChanges();
			DataTable dataSourceTable = grdDoc.DataSourceTable;
			if (dataSourceTable != null)
			{
				Propeties = "";
				for (int i = 0; i < dataSourceTable.Rows.Count; i++)
				{
					if (Function.ToString(dataSourceTable.Rows[i]["Checked"]).Equals("Y"))
					{
						Propeties = Propeties + Function.ToString(dataSourceTable.Rows[i]["Code"]) + ",";
					}
				}
				if (!string.IsNullOrEmpty(Propeties))
				{
					Propeties = Propeties.Substring(0, Propeties.Length - 1);
				}
			}
			DialogResult = DialogResult.OK;
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		try
		{
			int[] selectedRows = gvDoc.GetSelectedRows();
			if (selectedRows.Length > 1 && !chk_Ignore.Checked)
			{
				DataTable dataSourceTable = grdDoc.DataSourceTable;
				for (int i = 0; i < selectedRows.Length; i++)
				{
					dataSourceTable.Rows[gvDoc.GetDataSourceRowIndex(selectedRows[i])]["Checked"] = "Y";
				}
				grdDoc.DataSource = dataSourceTable;
				grdDoc.DataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnSelectAll_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grdDoc.DataSourceTable;
			for (int i = 0; i < dataSourceTable.Rows.Count; i++)
			{
				dataSourceTable.Rows[i]["Checked"] = "Y";
			}
			grdDoc.DataSource = dataSourceTable;
			grdDoc.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnInvertSelection_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grdDoc.DataSourceTable;
			for (int i = 0; i < dataSourceTable.Rows.Count; i++)
			{
				dataSourceTable.Rows[i]["Checked"] = (Function.ToString(dataSourceTable.Rows[i]["Checked"]).Equals("Y") ? "N" : "Y");
			}
			grdDoc.DataSource = dataSourceTable;
			grdDoc.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnClearSelection_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grdDoc.DataSourceTable;
			for (int i = 0; i < dataSourceTable.Rows.Count; i++)
			{
				dataSourceTable.Rows[i]["Checked"] = "N";
			}
			grdDoc.DataSource = dataSourceTable;
			grdDoc.DataSourceTable.AcceptChanges();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Mrps.frm_InputGenerateForecastItemPropeties));
		this.chk_Ignore = new Apzon.Controls.Editor.CheckEditEx();
		this.lbLink = new Apzon.Controls.Controls.LabelLine();
		this.chkOr = new Apzon.Controls.Editor.CheckEditEx();
		this.chkAnd = new Apzon.Controls.Editor.CheckEditEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcChecked = new Apzon.Controls.Grid.GridColumnEx();
		this.rChecked = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.btnOk = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnClearSelection = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnInvertSelection = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnSelectAll = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.chk_Ignore.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkOr.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkAnd.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rChecked).BeginInit();
		base.SuspendLayout();
		this.chk_Ignore.EditControl = null;
		this.chk_Ignore.EditValue = true;
		this.chk_Ignore.FieldName = null;
		this.chk_Ignore.IsSearchControl = false;
		this.chk_Ignore.Location = new System.Drawing.Point(22, 13);
		this.chk_Ignore.Name = "chk_Ignore";
		this.chk_Ignore.Properties.Caption = "Ignore Properties";
		this.chk_Ignore.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.chk_Ignore.Properties.ValueChecked = "Y";
		this.chk_Ignore.Properties.ValueUnchecked = "N";
		this.chk_Ignore.SettingInVisible = false;
		this.chk_Ignore.SettingReadonly = false;
		this.chk_Ignore.Size = new System.Drawing.Size(149, 20);
		this.chk_Ignore.TabIndex = 1;
		this.chk_Ignore.CheckedChanged += new System.EventHandler(this.chk_CheckedChanged);
		this.lbLink.EditControl = null;
		this.lbLink.EditValue = "Link";
		this.lbLink.FieldName = null;
		this.lbLink.HideWhenEmpty = false;
		this.lbLink.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbLink.Location = new System.Drawing.Point(50, 39);
		this.lbLink.Name = "lbLink";
		this.lbLink.ReadOnly = false;
		this.lbLink.SettingInVisible = false;
		this.lbLink.SettingReadonly = false;
		this.lbLink.ShowUnderLine = true;
		this.lbLink.Size = new System.Drawing.Size(51, 20);
		this.lbLink.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbLink.TabIndex = 0;
		this.lbLink.Utext = "Link";
		this.chkOr.EditControl = null;
		this.chkOr.EditValue = true;
		this.chkOr.FieldName = null;
		this.chkOr.IsSearchControl = false;
		this.chkOr.Location = new System.Drawing.Point(116, 40);
		this.chkOr.Name = "chkOr";
		this.chkOr.Properties.Caption = "Or";
		this.chkOr.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.chkOr.Properties.ValueChecked = "Y";
		this.chkOr.Properties.ValueUnchecked = "N";
		this.chkOr.SettingInVisible = false;
		this.chkOr.SettingReadonly = false;
		this.chkOr.Size = new System.Drawing.Size(75, 20);
		this.chkOr.TabIndex = 2;
		this.chkOr.CheckedChanged += new System.EventHandler(this.chk_CheckedChanged);
		this.chkAnd.EditControl = null;
		this.chkAnd.FieldName = null;
		this.chkAnd.IsSearchControl = false;
		this.chkAnd.Location = new System.Drawing.Point(187, 40);
		this.chkAnd.Name = "chkAnd";
		this.chkAnd.Properties.Caption = "And";
		this.chkAnd.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.chkAnd.Properties.ValueChecked = "Y";
		this.chkAnd.Properties.ValueUnchecked = "N";
		this.chkAnd.SettingInVisible = false;
		this.chkAnd.SettingReadonly = false;
		this.chkAnd.Size = new System.Drawing.Size(75, 20);
		this.chkAnd.TabIndex = 3;
		this.chkAnd.CheckedChanged += new System.EventHandler(this.chk_CheckedChanged);
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.Location = new System.Drawing.Point(2, 64);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rChecked });
		this.grdDoc.Size = new System.Drawing.Size(318, 324);
		this.grdDoc.TabIndex = 4;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gcCode, this.gcChecked });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oInputGeneralFIP";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "OPROPETIES";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsSelection.MultiSelect = true;
		this.gvDoc.OptionsView.BestFitMaxRowCount = 100;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gvDoc_SelectionChanged);
		this.gcCode.Caption = "Propeties";
		this.gcCode.CurrencyDateField = null;
		this.gcCode.CurrencyField = null;
		this.gcCode.CurrencyFixed = null;
		this.gcCode.CurrencyFixedRate = 0.0;
		this.gcCode.CurrencyRateField = null;
		this.gcCode.FieldName = "Name";
		this.gcCode.Name = "gcCode";
		this.gcCode.PrimaryKey = true;
		this.gcCode.UnitField = null;
		this.gcCode.Visible = true;
		this.gcCode.VisibleIndex = 0;
		this.gcChecked.Caption = "gcChecked";
		this.gcChecked.ColumnEdit = this.rChecked;
		this.gcChecked.CurrencyDateField = null;
		this.gcChecked.CurrencyField = null;
		this.gcChecked.CurrencyFixed = null;
		this.gcChecked.CurrencyFixedRate = 0.0;
		this.gcChecked.CurrencyRateField = null;
		this.gcChecked.FieldName = "Checked";
		this.gcChecked.Name = "gcChecked";
		this.gcChecked.PrimaryKey = false;
		this.gcChecked.UnitField = null;
		this.gcChecked.Visible = true;
		this.gcChecked.VisibleIndex = 1;
		this.rChecked.AutoHeight = false;
		this.rChecked.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.rChecked.Name = "rChecked";
		this.rChecked.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.rChecked.ValueChecked = "Y";
		this.rChecked.ValueUnchecked = "N";
		this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOk.EditControl = null;
		this.btnOk.EditValue = "OK";
		this.btnOk.FieldName = null;
		this.btnOk.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOk.ImageOptions.Image");
		this.btnOk.IsSearchControl = false;
		this.btnOk.Location = new System.Drawing.Point(12, 422);
		this.btnOk.Name = "btnOk";
		this.btnOk.ReadOnly = false;
		this.btnOk.SettingInVisible = false;
		this.btnOk.SettingReadonly = false;
		this.btnOk.Size = new System.Drawing.Size(70, 20);
		this.btnOk.TabIndex = 5;
		this.btnOk.Text = "OK";
		this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(85, 422);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 20);
		this.btnCancel.TabIndex = 6;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnClearSelection.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnClearSelection.EditControl = null;
		this.btnClearSelection.EditValue = "Clear Selection";
		this.btnClearSelection.FieldName = null;
		this.btnClearSelection.IsSearchControl = false;
		this.btnClearSelection.Location = new System.Drawing.Point(56, 394);
		this.btnClearSelection.Name = "btnClearSelection";
		this.btnClearSelection.ReadOnly = false;
		this.btnClearSelection.SettingInVisible = false;
		this.btnClearSelection.SettingReadonly = false;
		this.btnClearSelection.Size = new System.Drawing.Size(80, 20);
		this.btnClearSelection.TabIndex = 7;
		this.btnClearSelection.Text = "Clear Selection";
		this.btnClearSelection.Click += new System.EventHandler(this.btnClearSelection_Click);
		this.btnInvertSelection.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnInvertSelection.EditControl = null;
		this.btnInvertSelection.EditValue = "Invert Selection";
		this.btnInvertSelection.FieldName = null;
		this.btnInvertSelection.IsSearchControl = false;
		this.btnInvertSelection.Location = new System.Drawing.Point(142, 394);
		this.btnInvertSelection.Name = "btnInvertSelection";
		this.btnInvertSelection.ReadOnly = false;
		this.btnInvertSelection.SettingInVisible = false;
		this.btnInvertSelection.SettingReadonly = false;
		this.btnInvertSelection.Size = new System.Drawing.Size(90, 20);
		this.btnInvertSelection.TabIndex = 8;
		this.btnInvertSelection.Text = "Invert Selection";
		this.btnInvertSelection.Click += new System.EventHandler(this.btnInvertSelection_Click);
		this.btnSelectAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnSelectAll.EditControl = null;
		this.btnSelectAll.EditValue = "Select All";
		this.btnSelectAll.FieldName = null;
		this.btnSelectAll.IsSearchControl = false;
		this.btnSelectAll.Location = new System.Drawing.Point(239, 394);
		this.btnSelectAll.Name = "btnSelectAll";
		this.btnSelectAll.ReadOnly = false;
		this.btnSelectAll.SettingInVisible = false;
		this.btnSelectAll.SettingReadonly = false;
		this.btnSelectAll.Size = new System.Drawing.Size(75, 20);
		this.btnSelectAll.TabIndex = 9;
		this.btnSelectAll.Text = "Select All";
		this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
		base.AcceptButton = this.btnOk;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(322, 447);
		base.Controls.Add(this.btnSelectAll);
		base.Controls.Add(this.btnInvertSelection);
		base.Controls.Add(this.btnClearSelection);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnOk);
		base.Controls.Add(this.grdDoc);
		base.Controls.Add(this.chkAnd);
		base.Controls.Add(this.chkOr);
		base.Controls.Add(this.lbLink);
		base.Controls.Add(this.chk_Ignore);
		base.FormId = "oInputGeneralFIP";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_InputGenerateForecastItemPropeties";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Propeties";
		base.Load += new System.EventHandler(this.frm_InputGenerateForecastItemPropeties_Load);
		((System.ComponentModel.ISupportInitialize)this.chk_Ignore.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkOr.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkAnd.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rChecked).EndInit();
		base.ResumeLayout(false);
	}
}
