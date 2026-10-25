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
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Apzon.Forms.Dashboard;

public class frm_DataSubController : XtraFormEx
{
	private const string Controller = "Dashboard";

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridSplitContainer gridSplitContainer1;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridSplitContainer gridSplitContainer2;

	private GridControlEx grdLine;

	private GridViewEx gvLine;

	private SimpleButtonEx btnClose;

	private GridColumnEx gcCode;

	private GridColumnEx gcControllerName;

	private GridColumnEx gcRemarks;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem viewDetailToolStripMenuItem;

	private DateEditEx txtToDate;

	private DateEditEx txtFromDate;

	private LabelLine lbFromDate;

	private LabelLine lbToDate;

	public frm_DataSubController()
	{
		InitializeComponent();
	}

	public frm_DataSubController(DataSet dt, DateTime fromDate, DateTime toDate)
	{
		InitializeComponent();
		if (dt == null || dt.Tables.Count < 2 || dt.Tables[0].Rows.Count == 0)
		{
			throw new NullReferenceException();
		}
		Text = Function.ToString(dt.Tables[0].Rows[0]["Description"]) + " - Detail";
		txtFromDate.Properties.DisplayFormat.FormatType = FormatType.DateTime;
		txtFromDate.Properties.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
		txtFromDate.Properties.EditFormat.FormatType = FormatType.DateTime;
		txtFromDate.Properties.EditFormat.FormatString = GlobalData.DateTimeFormat;
		txtToDate.Properties.DisplayFormat.FormatType = FormatType.DateTime;
		txtToDate.Properties.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
		txtToDate.Properties.EditFormat.FormatType = FormatType.DateTime;
		txtToDate.Properties.EditFormat.FormatString = GlobalData.DateTimeFormat;
		txtFromDate.EditValue = new DateTime(DateTime.Now.Year, 1, 1);
		txtToDate.EditValue = DateTime.Now;
		txtFromDate.EditValue = fromDate;
		txtToDate.EditValue = toDate;
		grdDoc.DataSource = dt.Tables[0];
		CreateColumGrid(dt.Tables[1]);
		grdLine.DataSource = dt.Tables[1];
		grdDoc.RefreshDataSource();
		grdLine.RefreshDataSource();
	}

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gvLine_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void frm_DataSubController_Load(object sender, EventArgs e)
	{
	}

	private async void viewDetailToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			await LoadDataChildController();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private async void gvLine_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			GridView gridView = (GridView)sender;
			Point pt = gridView.GridControl.PointToClient(Control.MousePosition);
			GridHitInfo gridHitInfo = gridView.CalcHitInfo(pt);
			if (gridHitInfo.InRow || gridHitInfo.InRowCell)
			{
				await LoadDataChildController();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void btnClose_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void CreateColumGrid(DataTable dt)
	{
		try
		{
			gvLine.Columns.Clear();
			if (dt == null)
			{
				return;
			}
			for (int i = 0; i < dt.Columns.Count; i++)
			{
				GridColumn gridColumn = gvLine.Columns.Add();
				gridColumn.Caption = (dt.Columns[i].ColumnName.StartsWith("U_") ? dt.Columns[i].ColumnName.Substring(2) : dt.Columns[i].ColumnName);
				gridColumn.FieldName = dt.Columns[i].ColumnName;
				gridColumn.Name = dt.Columns[i].ColumnName + i;
				gridColumn.VisibleIndex = i;
				gridColumn.Visible = true;
				gridColumn.OptionsColumn.AllowSort = DefaultBoolean.False;
				gridColumn.OptionsColumn.AllowMove = false;
				gridColumn.OptionsColumn.AllowSize = true;
				gridColumn.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
				if (dt.Columns[i].DataType == typeof(DateTime))
				{
					gridColumn.DisplayFormat.FormatType = FormatType.DateTime;
					gridColumn.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
				}
				else if (dt.Columns[i].DataType == typeof(double) || dt.Columns[i].DataType == typeof(float) || dt.Columns[i].DataType == typeof(decimal))
				{
					gridColumn.DisplayFormat.FormatType = FormatType.Numeric;
					gridColumn.DisplayFormat.FormatString = ((GlobalData.QuantitiesFormat == null) ? "" : ("n" + GlobalData.QuantitiesFormat.NumberDecimalDigits));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataChildController()
	{
		try
		{
			int fatherId = Function.ParseInt(gvDoc.GetFocusedRowCellValue("ControllerId"));
			DataTable dataSourceTable = grdLine.DataSourceTable;
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("FromDate", typeof(DateTime));
			dataTable.Columns.Add("ToDate", typeof(DateTime));
			string text = Function.ToString(gvDoc.GetFocusedRowCellValue("ColumnKey"));
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split(',');
				for (int i = 0; i < array.Length; i++)
				{
					dataTable.Columns.Add(new DataColumn(array[i], (!dataSourceTable.Columns.Contains(array[i])) ? typeof(string) : dataSourceTable.Columns[array[i]].DataType));
				}
				DataRow dataRow = dataTable.NewRow();
				dataRow["FromDate"] = txtFromDate.EditValue;
				dataRow["ToDate"] = txtToDate.EditValue;
				for (int j = 0; j < array.Length; j++)
				{
					dataRow[array[j]] = (dataSourceTable.Columns.Contains(array[j]) ? dataSourceTable.Rows[gvLine.FocusedRowHandle][array[j]] : "");
				}
				dataTable.Rows.Add(dataRow);
			}
			DataSet dataSet = await GetDataChidController(fatherId, dataTable);
			if (dataSet != null && dataSet.Tables.Count == 2 && dataSet.Tables[0].Rows.Count > 0)
			{
				frm_DataSubController frm_DataSubController2 = new frm_DataSubController(dataSet, Function.ParseDateTimes(txtFromDate.EditValue), Function.ParseDateTimes(txtToDate.EditValue));
				frm_DataSubController2.MdiParent = MdiParent;
				frm_DataSubController2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async Task<DataSet> GetDataChidController(int fatherId, DataTable dtPara)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "Dashboard");
		return await http.PostAsync("GetDataChidController", new Tuple<int, DataTable>(fatherId, dtPara));
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Dashboard.frm_DataSubController));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.gridSplitContainer1 = new DevExpress.XtraGrid.GridSplitContainer();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcControllerName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRemarks = new Apzon.Controls.Grid.GridColumnEx();
		this.gridSplitContainer2 = new DevExpress.XtraGrid.GridSplitContainer();
		this.grdLine = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.viewDetailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvLine = new Apzon.Controls.Grid.GridViewEx();
		this.btnClose = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtToDate = new Apzon.Controls.Editor.DateEditEx();
		this.txtFromDate = new Apzon.Controls.Editor.DateEditEx();
		this.lbFromDate = new Apzon.Controls.Controls.LabelLine();
		this.lbToDate = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridSplitContainer1).BeginInit();
		this.gridSplitContainer1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridSplitContainer2).BeginInit();
		this.gridSplitContainer2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdLine).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 38);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.gridSplitContainer1);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.gridSplitContainer2);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(589, 290);
		this.splitContainerControl1.SplitterPosition = 117;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.gridSplitContainer1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridSplitContainer1.Grid = this.grdDoc;
		this.gridSplitContainer1.Location = new System.Drawing.Point(3, 2);
		this.gridSplitContainer1.Name = "gridSplitContainer1";
		this.gridSplitContainer1.Panel1.Controls.Add(this.grdDoc);
		this.gridSplitContainer1.Size = new System.Drawing.Size(583, 112);
		this.gridSplitContainer1.TabIndex = 0;
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.Location = new System.Drawing.Point(3, 2);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.Size = new System.Drawing.Size(583, 112);
		this.grdDoc.TabIndex = 0;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gcCode, this.gcControllerName, this.gcRemarks });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = null;
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 20;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = null;
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsBehavior.ReadOnly = true;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gcCode.Caption = "Controller Code";
		this.gcCode.CurrencyDateField = null;
		this.gcCode.CurrencyField = null;
		this.gcCode.CurrencyFixed = null;
		this.gcCode.CurrencyFixedRate = 0.0;
		this.gcCode.CurrencyRateField = null;
		this.gcCode.FieldName = "Code";
		this.gcCode.Name = "gcCode";
		this.gcCode.PrimaryKey = false;
		this.gcCode.UnitField = null;
		this.gcCode.Visible = true;
		this.gcCode.VisibleIndex = 0;
		this.gcControllerName.Caption = "Controller Desc";
		this.gcControllerName.CurrencyDateField = null;
		this.gcControllerName.CurrencyField = null;
		this.gcControllerName.CurrencyFixed = null;
		this.gcControllerName.CurrencyFixedRate = 0.0;
		this.gcControllerName.CurrencyRateField = null;
		this.gcControllerName.FieldName = "Description";
		this.gcControllerName.Name = "gcControllerName";
		this.gcControllerName.PrimaryKey = false;
		this.gcControllerName.UnitField = null;
		this.gcControllerName.Visible = true;
		this.gcControllerName.VisibleIndex = 1;
		this.gcRemarks.Caption = "Remarks";
		this.gcRemarks.CurrencyDateField = null;
		this.gcRemarks.CurrencyField = null;
		this.gcRemarks.CurrencyFixed = null;
		this.gcRemarks.CurrencyFixedRate = 0.0;
		this.gcRemarks.CurrencyRateField = null;
		this.gcRemarks.FieldName = "Remarks";
		this.gcRemarks.Name = "gcRemarks";
		this.gcRemarks.PrimaryKey = false;
		this.gcRemarks.UnitField = null;
		this.gcRemarks.Visible = true;
		this.gcRemarks.VisibleIndex = 2;
		this.gridSplitContainer2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridSplitContainer2.Grid = this.grdLine;
		this.gridSplitContainer2.Location = new System.Drawing.Point(6, 2);
		this.gridSplitContainer2.Name = "gridSplitContainer2";
		this.gridSplitContainer2.Panel1.Controls.Add(this.grdLine);
		this.gridSplitContainer2.Size = new System.Drawing.Size(580, 163);
		this.gridSplitContainer2.TabIndex = 0;
		this.grdLine.AllowPasteExcel = false;
		this.grdLine.ClipboardData = null;
		this.grdLine.ContextMenuStrip = this.contextMenuStrip1;
		this.grdLine.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdLine.Location = new System.Drawing.Point(0, 0);
		this.grdLine.MainView = this.gvLine;
		this.grdLine.Name = "grdLine";
		this.grdLine.Size = new System.Drawing.Size(580, 163);
		this.grdLine.TabIndex = 0;
		this.grdLine.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvLine });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.viewDetailToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(133, 26);
		this.viewDetailToolStripMenuItem.Name = "viewDetailToolStripMenuItem";
		this.viewDetailToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
		this.viewDetailToolStripMenuItem.Text = "View Detail";
		this.viewDetailToolStripMenuItem.Click += new System.EventHandler(this.viewDetailToolStripMenuItem_Click);
		this.gvLine.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvLine.ColumnKey = null;
		this.gvLine.DisableFormSetting = false;
		this.gvLine.DisplayException = false;
		this.gvLine.EditColumn = null;
		this.gvLine.FieldCurrency = null;
		this.gvLine.FieldRate = null;
		this.gvLine.FormId = null;
		this.gvLine.GridControl = this.grdLine;
		this.gvLine.IgnoreFilter = false;
		this.gvLine.IndexEditRow = -1;
		this.gvLine.IndicatorWidth = 40;
		this.gvLine.IsApzTable = false;
		this.gvLine.IsInsertNewRow = false;
		this.gvLine.IsLoadingData = false;
		this.gvLine.IsMappingContextAction = false;
		this.gvLine.ItemId = null;
		this.gvLine.LstColumnAllowBatch = null;
		this.gvLine.LstColumnNotAllowBatch = null;
		this.gvLine.LstColumnNotEditable = null;
		this.gvLine.LstColumNotSetting = null;
		this.gvLine.Name = "gvLine";
		this.gvLine.OptionsBehavior.ReadOnly = true;
		this.gvLine.OptionsView.ColumnAutoWidth = false;
		this.gvLine.OptionsView.ShowGroupPanel = false;
		this.gvLine.UseBatchMod = false;
		this.gvLine.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvLine_CustomDrawRowIndicator);
		this.gvLine.DoubleClick += new System.EventHandler(this.gvLine_DoubleClick);
		this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnClose.EditControl = null;
		this.btnClose.EditValue = "Close";
		this.btnClose.FieldName = null;
		this.btnClose.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnClose.ImageOptions.Image");
		this.btnClose.IsSearchControl = false;
		this.btnClose.Location = new System.Drawing.Point(512, 334);
		this.btnClose.Name = "btnClose";
		this.btnClose.ReadOnly = false;
		this.btnClose.SettingInVisible = false;
		this.btnClose.SettingReadonly = false;
		this.btnClose.Size = new System.Drawing.Size(70, 20);
		this.btnClose.TabIndex = 1;
		this.btnClose.Text = "Close";
		this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
		this.txtToDate.EditControl = null;
		this.txtToDate.EditValue = null;
		this.txtToDate.FieldName = null;
		this.txtToDate.Location = new System.Drawing.Point(349, 6);
		this.txtToDate.Name = "txtToDate";
		this.txtToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtToDate.Properties.IsSearchControl = false;
		this.txtToDate.Properties.Mask.EditMask = "";
		this.txtToDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.txtToDate.SettingInVisible = false;
		this.txtToDate.SettingReadonly = false;
		this.txtToDate.Size = new System.Drawing.Size(160, 20);
		this.txtToDate.TabIndex = 6;
		this.txtFromDate.EditControl = null;
		this.txtFromDate.EditValue = null;
		this.txtFromDate.FieldName = null;
		this.txtFromDate.Location = new System.Drawing.Point(92, 6);
		this.txtFromDate.Name = "txtFromDate";
		this.txtFromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFromDate.Properties.IsSearchControl = false;
		this.txtFromDate.Properties.Mask.EditMask = "";
		this.txtFromDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.txtFromDate.SettingInVisible = false;
		this.txtFromDate.SettingReadonly = false;
		this.txtFromDate.Size = new System.Drawing.Size(160, 20);
		this.txtFromDate.TabIndex = 5;
		this.lbFromDate.EditControl = null;
		this.lbFromDate.EditValue = "From Date";
		this.lbFromDate.FieldName = null;
		this.lbFromDate.HideWhenEmpty = false;
		this.lbFromDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbFromDate.Location = new System.Drawing.Point(12, 8);
		this.lbFromDate.Name = "lbFromDate";
		this.lbFromDate.ReadOnly = false;
		this.lbFromDate.SettingInVisible = false;
		this.lbFromDate.SettingReadonly = false;
		this.lbFromDate.ShowUnderLine = true;
		this.lbFromDate.Size = new System.Drawing.Size(80, 20);
		this.lbFromDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbFromDate.TabIndex = 3;
		this.lbFromDate.Utext = "From Date";
		this.lbToDate.EditControl = null;
		this.lbToDate.EditValue = "To Date";
		this.lbToDate.FieldName = null;
		this.lbToDate.HideWhenEmpty = false;
		this.lbToDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbToDate.Location = new System.Drawing.Point(269, 8);
		this.lbToDate.Name = "lbToDate";
		this.lbToDate.ReadOnly = false;
		this.lbToDate.SettingInVisible = false;
		this.lbToDate.SettingReadonly = false;
		this.lbToDate.ShowUnderLine = true;
		this.lbToDate.Size = new System.Drawing.Size(80, 20);
		this.lbToDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbToDate.TabIndex = 4;
		this.lbToDate.Utext = "To Date";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(590, 359);
		base.Controls.Add(this.txtToDate);
		base.Controls.Add(this.txtFromDate);
		base.Controls.Add(this.lbFromDate);
		base.Controls.Add(this.lbToDate);
		base.Controls.Add(this.btnClose);
		base.Controls.Add(this.splitContainerControl1);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_DataSubController";
		this.Text = "frm_DataSubController";
		base.Load += new System.EventHandler(this.frm_DataSubController_Load);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridSplitContainer1).EndInit();
		this.gridSplitContainer1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridSplitContainer2).EndInit();
		this.gridSplitContainer2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdLine).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
