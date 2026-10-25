using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Reports;

public class frm_ValidValue : XtraFormEx
{
	public DataTable _dtColumn;

	public DataTable _dtData;

	public bool isSave;

	public string dfColumn = "";

	public int _id;

	public string _code = "";

	public string _type = "1";

	private IContainer components;

	private LabelLine labelLine3;

	private GridControlEx grd_ListValue;

	private GridViewEx gv_ListValue;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	private ComboBoxEditEx cbx_Column;

	private SimpleButtonEx btn_NewRow;

	private SimpleButtonEx btn_Del;

	private GridColumnEx cl_ColumnName;

	private GridColumnEx cl_Value;

	private GridColumnEx cl_Description;

	private GridColumnEx cl_BackColor;

	private GridColumnEx cl_ForeColor;

	private GridColumnEx cl_ID;

	public frm_ValidValue()
	{
		InitializeComponent();
	}

	public frm_ValidValue(DataTable dtColumn, DataTable dtData)
	{
		_dtColumn = dtColumn.Copy();
		_dtData = dtData.Copy();
		InitializeComponent();
	}

	private void frm_ValidValue_Load(object sender, EventArgs e)
	{
		try
		{
			if (_dtData == null || _dtData.Columns.Count == 0)
			{
				if (_type == "1")
				{
					_dtData.Columns.Add("ID", typeof(int));
				}
				else
				{
					_dtData.Columns.Add("Code", typeof(string));
					cl_ID.FieldName = "Code";
				}
				_dtData.Columns.Add("ColumnName", typeof(string));
				_dtData.Columns.Add("Value", typeof(string));
				_dtData.Columns.Add("Description", typeof(string));
				_dtData.Columns.Add("BackColor", typeof(string));
				_dtData.Columns.Add("ForeColor", typeof(string));
			}
			grd_ListValue.DataSource = _dtData;
			grd_ListValue.RefreshDataSource();
			gv_ListValue.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
			if (!_dtColumn.IsNotNull() || !_dtColumn.IsExistsColumn("ColumnName"))
			{
				return;
			}
			foreach (DataRow row in _dtColumn.Rows)
			{
				cbx_Column.Properties.Items.Add(Function.ToString(row["ColumnName"]));
			}
			cbx_Column.EditValue = dfColumn;
			gv_ListValue.ActiveFilterString = $"ColumnName = '{Function.ToString(cbx_Column.EditValue)}'";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void cbx_Collumn_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			gv_ListValue.ActiveFilterString = $"ColumnName = '{Function.ToString(cbx_Column.EditValue)}'";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		isSave = false;
		Close();
	}

	private void btn_NewRow_Click(object sender, EventArgs e)
	{
		try
		{
			if (cbx_Column.SelectedIndex >= 0)
			{
				DataTable dataTable = grd_ListValue.DatasourceTable();
				DataRow dataRow = dataTable.NewRow();
				if (_type == "1")
				{
					dataRow["ID"] = _id;
				}
				else
				{
					dataRow["Code"] = _code;
				}
				dataRow["ColumnName"] = cbx_Column.EditValue;
				dataTable.Rows.Add(dataRow);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Del_Click(object sender, EventArgs e)
	{
		try
		{
			gv_ListValue.DeleteRow(gv_ListValue.FocusedRowHandle);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			_dtData = grd_ListValue.DatasourceTable();
			isSave = true;
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Reports.frm_ValidValue));
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.grd_ListValue = new Apzon.Controls.Grid.GridControlEx();
		this.gv_ListValue = new Apzon.Controls.Grid.GridViewEx();
		this.cl_ID = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_ColumnName = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Value = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Description = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_BackColor = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_ForeColor = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.cbx_Column = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.btn_NewRow = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Del = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_ListValue).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ListValue).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbx_Column.Properties).BeginInit();
		base.SuspendLayout();
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Column";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(8, 14);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(53, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 33;
		this.labelLine3.Utext = "Column";
		this.grd_ListValue.AllowPasteExcel = true;
		this.grd_ListValue.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_ListValue.ClipboardData = null;
		this.grd_ListValue.Location = new System.Drawing.Point(0, 40);
		this.grd_ListValue.MainView = this.gv_ListValue;
		this.grd_ListValue.Name = "grd_ListValue";
		this.grd_ListValue.Size = new System.Drawing.Size(342, 304);
		this.grd_ListValue.TabIndex = 34;
		this.grd_ListValue.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ListValue });
		this.gv_ListValue.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ListValue.ColumnKey = null;
		this.gv_ListValue.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.cl_ID, this.cl_ColumnName, this.cl_Value, this.cl_Description, this.cl_BackColor, this.cl_ForeColor });
		this.gv_ListValue.DisableFormSetting = false;
		this.gv_ListValue.DisplayException = false;
		this.gv_ListValue.EditColumn = null;
		this.gv_ListValue.FieldCurrency = null;
		this.gv_ListValue.FieldRate = null;
		this.gv_ListValue.FormId = "oSetValidValue";
		this.gv_ListValue.GridControl = this.grd_ListValue;
		this.gv_ListValue.IgnoreFilter = false;
		this.gv_ListValue.IndexEditRow = -1;
		this.gv_ListValue.IndicatorWidth = 35;
		this.gv_ListValue.IsApzTable = false;
		this.gv_ListValue.IsInsertNewRow = false;
		this.gv_ListValue.IsLoadingData = false;
		this.gv_ListValue.IsMappingContextAction = false;
		this.gv_ListValue.ItemId = "ODATA";
		this.gv_ListValue.LstColumnAllowBatch = null;
		this.gv_ListValue.LstColumnNotAllowBatch = null;
		this.gv_ListValue.LstColumnNotEditable = null;
		this.gv_ListValue.LstColumNotSetting = null;
		this.gv_ListValue.Name = "gv_ListValue";
		this.gv_ListValue.OptionsView.BestFitMaxRowCount = 100;
		this.gv_ListValue.OptionsView.ColumnAutoWidth = false;
		this.gv_ListValue.OptionsView.ShowGroupPanel = false;
		this.gv_ListValue.UseBatchMod = false;
		this.cl_ID.Caption = "ID";
		this.cl_ID.CurrencyDateField = null;
		this.cl_ID.CurrencyField = null;
		this.cl_ID.CurrencyFixed = null;
		this.cl_ID.CurrencyFixedRate = 0.0;
		this.cl_ID.CurrencyRateField = null;
		this.cl_ID.FieldName = "ID";
		this.cl_ID.Name = "cl_ID";
		this.cl_ID.OptionsColumn.AllowFocus = false;
		this.cl_ID.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
		this.cl_ID.OptionsColumn.AllowMove = false;
		this.cl_ID.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.cl_ID.OptionsColumn.ReadOnly = true;
		this.cl_ID.PrimaryKey = false;
		this.cl_ID.UnitField = null;
		this.cl_ColumnName.Caption = "Column Name";
		this.cl_ColumnName.CurrencyDateField = null;
		this.cl_ColumnName.CurrencyField = null;
		this.cl_ColumnName.CurrencyFixed = null;
		this.cl_ColumnName.CurrencyFixedRate = 0.0;
		this.cl_ColumnName.CurrencyRateField = null;
		this.cl_ColumnName.FieldName = "ColumnName";
		this.cl_ColumnName.Name = "cl_ColumnName";
		this.cl_ColumnName.OptionsColumn.AllowFocus = false;
		this.cl_ColumnName.OptionsColumn.ReadOnly = true;
		this.cl_ColumnName.PrimaryKey = false;
		this.cl_ColumnName.UnitField = null;
		this.cl_Value.Caption = "Value";
		this.cl_Value.CurrencyDateField = null;
		this.cl_Value.CurrencyField = null;
		this.cl_Value.CurrencyFixed = null;
		this.cl_Value.CurrencyFixedRate = 0.0;
		this.cl_Value.CurrencyRateField = null;
		this.cl_Value.FieldName = "Value";
		this.cl_Value.Name = "cl_Value";
		this.cl_Value.OptionsColumn.AllowMove = false;
		this.cl_Value.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.cl_Value.PrimaryKey = false;
		this.cl_Value.UnitField = null;
		this.cl_Value.Visible = true;
		this.cl_Value.VisibleIndex = 0;
		this.cl_Description.Caption = "Description";
		this.cl_Description.CurrencyDateField = null;
		this.cl_Description.CurrencyField = null;
		this.cl_Description.CurrencyFixed = null;
		this.cl_Description.CurrencyFixedRate = 0.0;
		this.cl_Description.CurrencyRateField = null;
		this.cl_Description.FieldName = "Description";
		this.cl_Description.Name = "cl_Description";
		this.cl_Description.OptionsColumn.AllowMove = false;
		this.cl_Description.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.cl_Description.PrimaryKey = false;
		this.cl_Description.UnitField = null;
		this.cl_Description.Visible = true;
		this.cl_Description.VisibleIndex = 1;
		this.cl_BackColor.Caption = "Back Color";
		this.cl_BackColor.CurrencyDateField = null;
		this.cl_BackColor.CurrencyField = null;
		this.cl_BackColor.CurrencyFixed = null;
		this.cl_BackColor.CurrencyFixedRate = 0.0;
		this.cl_BackColor.CurrencyRateField = null;
		this.cl_BackColor.FieldName = "BackColor";
		this.cl_BackColor.Name = "cl_BackColor";
		this.cl_BackColor.OptionsColumn.AllowMove = false;
		this.cl_BackColor.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.cl_BackColor.PrimaryKey = false;
		this.cl_BackColor.UnitField = null;
		this.cl_BackColor.Visible = true;
		this.cl_BackColor.VisibleIndex = 2;
		this.cl_ForeColor.Caption = "Fore Color";
		this.cl_ForeColor.CurrencyDateField = null;
		this.cl_ForeColor.CurrencyField = null;
		this.cl_ForeColor.CurrencyFixed = null;
		this.cl_ForeColor.CurrencyFixedRate = 0.0;
		this.cl_ForeColor.CurrencyRateField = null;
		this.cl_ForeColor.FieldName = "ForeColor";
		this.cl_ForeColor.Name = "cl_ForeColor";
		this.cl_ForeColor.OptionsColumn.AllowMove = false;
		this.cl_ForeColor.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.cl_ForeColor.PrimaryKey = false;
		this.cl_ForeColor.UnitField = null;
		this.cl_ForeColor.Visible = true;
		this.cl_ForeColor.VisibleIndex = 3;
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(81, 350);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 35;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(0, 350);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 36;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.cbx_Column.EditControl = null;
		this.cbx_Column.FieldName = null;
		this.cbx_Column.IsSearchControl = false;
		this.cbx_Column.Location = new System.Drawing.Point(57, 11);
		this.cbx_Column.Name = "cbx_Column";
		this.cbx_Column.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbx_Column.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.cbx_Column.SettingInVisible = false;
		this.cbx_Column.SettingReadonly = false;
		this.cbx_Column.Size = new System.Drawing.Size(150, 20);
		this.cbx_Column.TabIndex = 37;
		this.cbx_Column.SelectedIndexChanged += new System.EventHandler(this.cbx_Collumn_SelectedIndexChanged);
		this.btn_NewRow.EditControl = null;
		this.btn_NewRow.EditValue = "New";
		this.btn_NewRow.FieldName = null;
		this.btn_NewRow.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_NewRow.ImageOptions.Image");
		this.btn_NewRow.IsSearchControl = false;
		this.btn_NewRow.Location = new System.Drawing.Point(211, 9);
		this.btn_NewRow.Name = "btn_NewRow";
		this.btn_NewRow.ReadOnly = false;
		this.btn_NewRow.SettingInVisible = false;
		this.btn_NewRow.SettingReadonly = false;
		this.btn_NewRow.Size = new System.Drawing.Size(54, 23);
		this.btn_NewRow.TabIndex = 38;
		this.btn_NewRow.Text = "New";
		this.btn_NewRow.Click += new System.EventHandler(this.btn_NewRow_Click);
		this.btn_Del.EditControl = null;
		this.btn_Del.EditValue = "Delete";
		this.btn_Del.FieldName = null;
		this.btn_Del.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Del.ImageOptions.Image");
		this.btn_Del.IsSearchControl = false;
		this.btn_Del.Location = new System.Drawing.Point(271, 9);
		this.btn_Del.Name = "btn_Del";
		this.btn_Del.ReadOnly = false;
		this.btn_Del.SettingInVisible = false;
		this.btn_Del.SettingReadonly = false;
		this.btn_Del.Size = new System.Drawing.Size(66, 23);
		this.btn_Del.TabIndex = 39;
		this.btn_Del.Text = "Delete";
		this.btn_Del.Click += new System.EventHandler(this.btn_Del_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(342, 375);
		base.Controls.Add(this.btn_Del);
		base.Controls.Add(this.btn_NewRow);
		base.Controls.Add(this.cbx_Column);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.grd_ListValue);
		base.Controls.Add(this.labelLine3);
		base.FormId = "oSetValidValue";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ValidValue";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Set Valid Value";
		base.Load += new System.EventHandler(this.frm_ValidValue_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_ListValue).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ListValue).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbx_Column.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
