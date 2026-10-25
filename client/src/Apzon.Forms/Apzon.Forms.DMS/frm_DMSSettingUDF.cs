using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Export;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using Newtonsoft.Json;

namespace Apzon.Forms.DMS;

public class frm_DMSSettingUDF : XtraFormEx
{
	private const string OkString = "OK";

	private const string UpdateString = "Update";

	private const string AddString = "Add";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	protected SimpleButtonEx btn_cancel;

	protected SimpleButtonEx btn_ok;

	private TreeList tv_ListFunction;

	private GridControlEx grd_SettingUDF;

	private GridViewEx gv_SettingUDF;

	private TreeListColumn col_Descr;

	private GridColumnEx gc_AliasID;

	private GridColumnEx gc_Descr;

	private GridColumnEx gc_Active;

	private RepositoryItemCheckEdit check_active;

	private GridColumnEx gc_Editable;

	private RepositoryItemCheckEdit check_editable;

	private GridColumnEx gc_NotNull;

	private RepositoryItemCheckEdit check_mandatory;

	private GridColumnEx gc_DefaultValue;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
			{
				WinFuntions.DisableAllControl(Controls);
				btn_cancel.Enabled = true;
				return;
			}
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
				btn_ok.Text = "Add";
				break;
			case Enums.FormViewModeType.Update:
				btn_ok.Text = "Update";
				break;
			case Enums.FormViewModeType.View:
				btn_ok.Text = "OK";
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_DMSSettingUDF()
	{
		InitializeComponent();
		OnLoadAsync += frm_SettingUDF_Load;
	}

	private new async Task Load()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ID", typeof(int));
			dataTable.Columns.Add("ParentID", typeof(int));
			dataTable.Columns.Add("Descr", typeof(string));
			dataTable.Columns.Add("TableName", typeof(string));
			dataTable.Rows.Add(1, -1, "Master Data", null);
			dataTable.Rows.Add(2, 1, "Business Partner", "OCRD");
			dataTable.Rows.Add(3, 2, "Add new store", "OCRD");
			dataTable.Rows.Add(48, -1, "Marketing Documents", null);
			dataTable.Rows.Add(49, 48, "Order", "ORDR");
			dataTable.Rows.Add(2011, 48, "Order Line", "RDR1");
			dataTable.Rows.Add(2008, 48, "Delivery", "ODLN");
			dataTable.Rows.Add(2012, 48, "Delivery Line", "DLN1");
			dataTable.Rows.Add(2010, 48, "Van Sale", "OINV");
			dataTable.Rows.Add(2013, 48, "Van Sale Line", "INV1");
			dataTable.Rows.Add(2009, 48, "Draft", "ODRF");
			dataTable.Rows.Add(2014, 48, "Draft Line", "DRF1");
			dataTable.AcceptChanges();
			tv_ListFunction.OptionsBehavior.ReadOnly = true;
			tv_ListFunction.DataSource = dataTable;
			tv_ListFunction.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_SettingUDF_Load(object sender, EventArgs e)
	{
		await Load();
	}

	private async void tv_ListFunction_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		try
		{
			string text = Function.ToString(tv_ListFunction.FocusedNode.GetValue("TableName"));
			int menuId = Function.ParseInt(tv_ListFunction.FocusedNode.GetValue("ID"));
			if (!string.IsNullOrEmpty(text))
			{
				HttpResult httpResult = await _httpClient.DMSSettingUDF.GetData(text, menuId);
				DataSet dataSet = new DataSet();
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					dataSet = JsonConvert.DeserializeObject<DataSet>(Function.ToString(httpResult.Content));
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				grd_SettingUDF.DataSource = dataSet.Tables["ListUDF"];
				grd_SettingUDF.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_SettingUDF_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_cancel_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_ok_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			WinCommons._OpenProcessing();
			DataTable dataTable = grd_SettingUDF.DatasourceTable();
			int num = Function.ParseInt(tv_ListFunction.FocusedNode.GetValue("ID"));
			DataTable dataTable2 = dataTable.Copy();
			dataTable2.Columns.Add("MenuID", typeof(int));
			dataTable2.Columns.Add("UserSign", typeof(string));
			for (int i = 0; i < dataTable2.Rows.Count; i++)
			{
				dataTable2.Rows[i]["MenuID"] = num;
				dataTable2.Rows[i]["UserSign"] = GlobalData.UserID;
			}
			HttpResult httpResult = await _httpClient.DMSSettingUDF.Update(dataTable2);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				FormMode = Enums.FormViewModeType.View;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
			WinCommons._CloseProcessing();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.DMS.frm_DMSSettingUDF));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.tv_ListFunction = new DevExpress.XtraTreeList.TreeList();
		this.col_Descr = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.grd_SettingUDF = new Apzon.Controls.Grid.GridControlEx();
		this.gv_SettingUDF = new Apzon.Controls.Grid.GridViewEx();
		this.gc_AliasID = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Descr = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Active = new Apzon.Controls.Grid.GridColumnEx();
		this.check_active = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gc_Editable = new Apzon.Controls.Grid.GridColumnEx();
		this.check_editable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gc_NotNull = new Apzon.Controls.Grid.GridColumnEx();
		this.check_mandatory = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gc_DefaultValue = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_ok = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tv_ListFunction).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_SettingUDF).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_SettingUDF).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.check_active).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.check_editable).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.check_mandatory).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(12, 12);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.tv_ListFunction);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_SettingUDF);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(749, 443);
		this.splitContainerControl1.SplitterPosition = 193;
		this.splitContainerControl1.TabIndex = 0;
		this.tv_ListFunction.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[1] { this.col_Descr });
		this.tv_ListFunction.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tv_ListFunction.Location = new System.Drawing.Point(0, 0);
		this.tv_ListFunction.Name = "tv_ListFunction";
		this.tv_ListFunction.Size = new System.Drawing.Size(193, 443);
		this.tv_ListFunction.TabIndex = 0;
		this.tv_ListFunction.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tv_ListFunction_FocusedNodeChanged);
		this.col_Descr.Caption = "Category";
		this.col_Descr.FieldName = "Descr";
		this.col_Descr.Name = "col_Descr";
		this.col_Descr.Visible = true;
		this.col_Descr.VisibleIndex = 0;
		this.grd_SettingUDF.AllowPasteExcel = false;
		this.grd_SettingUDF.ClipboardData = null;
		this.grd_SettingUDF.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_SettingUDF.Location = new System.Drawing.Point(0, 0);
		this.grd_SettingUDF.MainView = this.gv_SettingUDF;
		this.grd_SettingUDF.Name = "grd_SettingUDF";
		this.grd_SettingUDF.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.check_active, this.check_editable, this.check_mandatory });
		this.grd_SettingUDF.Size = new System.Drawing.Size(546, 443);
		this.grd_SettingUDF.TabIndex = 0;
		this.grd_SettingUDF.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_SettingUDF });
		this.gv_SettingUDF.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_SettingUDF.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_SettingUDF.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_SettingUDF.ColumnKey = null;
		this.gv_SettingUDF.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gc_AliasID, this.gc_Descr, this.gc_Active, this.gc_Editable, this.gc_NotNull, this.gc_DefaultValue });
		this.gv_SettingUDF.DisableFormSetting = false;
		this.gv_SettingUDF.DisplayException = false;
		this.gv_SettingUDF.EditColumn = null;
		this.gv_SettingUDF.FieldCurrency = null;
		this.gv_SettingUDF.FieldRate = null;
		this.gv_SettingUDF.FormId = null;
		this.gv_SettingUDF.GridControl = this.grd_SettingUDF;
		this.gv_SettingUDF.HasFormSetting = false;
		this.gv_SettingUDF.IgnoreFilter = false;
		this.gv_SettingUDF.IndexEditRow = -1;
		this.gv_SettingUDF.IndicatorWidth = 35;
		this.gv_SettingUDF.IsApzTable = false;
		this.gv_SettingUDF.IsInsertNewRow = false;
		this.gv_SettingUDF.IsLoadingData = false;
		this.gv_SettingUDF.IsMappingContextAction = false;
		this.gv_SettingUDF.ItemId = null;
		this.gv_SettingUDF.LstColumnAllowBatch = null;
		this.gv_SettingUDF.LstColumnNotAllowBatch = null;
		this.gv_SettingUDF.LstColumnNotEditable = null;
		this.gv_SettingUDF.LstColumNotSetting = null;
		this.gv_SettingUDF.LstOldUDF = null;
		this.gv_SettingUDF.Name = "gv_SettingUDF";
		this.gv_SettingUDF.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_SettingUDF.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_SettingUDF.OptionsClipboard.ClipboardMode = DevExpress.Export.ClipboardMode.Formatted;
		this.gv_SettingUDF.OptionsPrint.AutoWidth = false;
		this.gv_SettingUDF.OptionsPrint.UsePrintStyles = false;
		this.gv_SettingUDF.OptionsView.BestFitMaxRowCount = 100;
		this.gv_SettingUDF.OptionsView.ColumnAutoWidth = false;
		this.gv_SettingUDF.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_SettingUDF.OptionsView.ShowGroupedColumns = true;
		this.gv_SettingUDF.OptionsView.ShowGroupPanel = false;
		this.gv_SettingUDF.UseBatchMod = false;
		this.gv_SettingUDF.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_SettingUDF_CellValueChanged);
		this.gc_AliasID.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.gc_AliasID.AppearanceCell.Options.UseBackColor = true;
		this.gc_AliasID.Caption = "Name";
		this.gc_AliasID.CurrencyDateField = null;
		this.gc_AliasID.CurrencyField = null;
		this.gc_AliasID.CurrencyFixed = null;
		this.gc_AliasID.CurrencyFixedRate = 0.0;
		this.gc_AliasID.CurrencyRateField = null;
		this.gc_AliasID.FieldName = "AliasID";
		this.gc_AliasID.Name = "gc_AliasID";
		this.gc_AliasID.OptionsColumn.ReadOnly = true;
		this.gc_AliasID.PrimaryKey = false;
		this.gc_AliasID.UnitField = null;
		this.gc_AliasID.Visible = true;
		this.gc_AliasID.VisibleIndex = 0;
		this.gc_Descr.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.gc_Descr.AppearanceCell.Options.UseBackColor = true;
		this.gc_Descr.Caption = "Description";
		this.gc_Descr.CurrencyDateField = null;
		this.gc_Descr.CurrencyField = null;
		this.gc_Descr.CurrencyFixed = null;
		this.gc_Descr.CurrencyFixedRate = 0.0;
		this.gc_Descr.CurrencyRateField = null;
		this.gc_Descr.FieldName = "Descr";
		this.gc_Descr.Name = "gc_Descr";
		this.gc_Descr.OptionsColumn.ReadOnly = true;
		this.gc_Descr.PrimaryKey = false;
		this.gc_Descr.UnitField = null;
		this.gc_Descr.Visible = true;
		this.gc_Descr.VisibleIndex = 1;
		this.gc_Active.Caption = "Active";
		this.gc_Active.ColumnEdit = this.check_active;
		this.gc_Active.CurrencyDateField = null;
		this.gc_Active.CurrencyField = null;
		this.gc_Active.CurrencyFixed = null;
		this.gc_Active.CurrencyFixedRate = 0.0;
		this.gc_Active.CurrencyRateField = null;
		this.gc_Active.FieldName = "IsActive";
		this.gc_Active.Name = "gc_Active";
		this.gc_Active.PrimaryKey = false;
		this.gc_Active.UnitField = null;
		this.gc_Active.Visible = true;
		this.gc_Active.VisibleIndex = 2;
		this.check_active.AutoHeight = false;
		this.check_active.Name = "check_active";
		this.check_active.ValueChecked = "Y";
		this.check_active.ValueUnchecked = "N";
		this.gc_Editable.Caption = "Editable";
		this.gc_Editable.ColumnEdit = this.check_editable;
		this.gc_Editable.CurrencyDateField = null;
		this.gc_Editable.CurrencyField = null;
		this.gc_Editable.CurrencyFixed = null;
		this.gc_Editable.CurrencyFixedRate = 0.0;
		this.gc_Editable.CurrencyRateField = null;
		this.gc_Editable.FieldName = "IsEditable";
		this.gc_Editable.Name = "gc_Editable";
		this.gc_Editable.PrimaryKey = false;
		this.gc_Editable.UnitField = null;
		this.gc_Editable.Visible = true;
		this.gc_Editable.VisibleIndex = 3;
		this.check_editable.AutoHeight = false;
		this.check_editable.Name = "check_editable";
		this.check_editable.ValueChecked = "Y";
		this.check_editable.ValueUnchecked = "N";
		this.gc_NotNull.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.gc_NotNull.AppearanceCell.Options.UseBackColor = true;
		this.gc_NotNull.Caption = "Mandatory";
		this.gc_NotNull.ColumnEdit = this.check_mandatory;
		this.gc_NotNull.CurrencyDateField = null;
		this.gc_NotNull.CurrencyField = null;
		this.gc_NotNull.CurrencyFixed = null;
		this.gc_NotNull.CurrencyFixedRate = 0.0;
		this.gc_NotNull.CurrencyRateField = null;
		this.gc_NotNull.FieldName = "NotNull";
		this.gc_NotNull.Name = "gc_NotNull";
		this.gc_NotNull.OptionsColumn.ReadOnly = true;
		this.gc_NotNull.PrimaryKey = false;
		this.gc_NotNull.UnitField = null;
		this.gc_NotNull.Visible = true;
		this.gc_NotNull.VisibleIndex = 4;
		this.check_mandatory.AutoHeight = false;
		this.check_mandatory.Name = "check_mandatory";
		this.check_mandatory.ValueChecked = "Y";
		this.check_mandatory.ValueUnchecked = "N";
		this.gc_DefaultValue.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.gc_DefaultValue.AppearanceCell.Options.UseBackColor = true;
		this.gc_DefaultValue.Caption = "Default Value";
		this.gc_DefaultValue.CurrencyDateField = null;
		this.gc_DefaultValue.CurrencyField = null;
		this.gc_DefaultValue.CurrencyFixed = null;
		this.gc_DefaultValue.CurrencyFixedRate = 0.0;
		this.gc_DefaultValue.CurrencyRateField = null;
		this.gc_DefaultValue.FieldName = "Dflt";
		this.gc_DefaultValue.Name = "gc_DefaultValue";
		this.gc_DefaultValue.OptionsColumn.ReadOnly = true;
		this.gc_DefaultValue.PrimaryKey = false;
		this.gc_DefaultValue.UnitField = null;
		this.gc_DefaultValue.Visible = true;
		this.gc_DefaultValue.VisibleIndex = 5;
		this.btn_cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_cancel.EditControl = null;
		this.btn_cancel.EditValue = "Cancel";
		this.btn_cancel.FieldName = null;
		this.btn_cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_cancel.ImageOptions.Image");
		this.btn_cancel.IsSearchControl = false;
		this.btn_cancel.Location = new System.Drawing.Point(686, 461);
		this.btn_cancel.Name = "btn_cancel";
		this.btn_cancel.ReadOnly = false;
		this.btn_cancel.SettingInVisible = false;
		this.btn_cancel.SettingReadonly = false;
		this.btn_cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_cancel.TabIndex = 13;
		this.btn_cancel.Text = "Cancel";
		this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
		this.btn_ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_ok.EditControl = null;
		this.btn_ok.EditValue = "OK";
		this.btn_ok.FieldName = null;
		this.btn_ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ok.ImageOptions.Image");
		this.btn_ok.IsSearchControl = false;
		this.btn_ok.Location = new System.Drawing.Point(605, 461);
		this.btn_ok.Name = "btn_ok";
		this.btn_ok.ReadOnly = false;
		this.btn_ok.SettingInVisible = false;
		this.btn_ok.SettingReadonly = false;
		this.btn_ok.Size = new System.Drawing.Size(75, 23);
		this.btn_ok.TabIndex = 14;
		this.btn_ok.Text = "OK";
		this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(773, 496);
		base.Controls.Add(this.btn_cancel);
		base.Controls.Add(this.btn_ok);
		base.Controls.Add(this.splitContainerControl1);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_DMSSettingUDF";
		this.Text = "Setting UDF";
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tv_ListFunction).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_SettingUDF).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_SettingUDF).EndInit();
		((System.ComponentModel.ISupportInitialize)this.check_active).EndInit();
		((System.ComponentModel.ISupportInitialize)this.check_editable).EndInit();
		((System.ComponentModel.ISupportInitialize)this.check_mandatory).EndInit();
		base.ResumeLayout(false);
	}
}
