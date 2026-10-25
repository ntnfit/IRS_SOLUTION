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
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using DevExpress.Export;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.HumanResources.TimeSheeting;

public class frm_AboutRangeTime : XtraFormEx
{
	private IContainer components;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private SimpleButtonEx btn_Save;

	private SimpleButtonEx btn_Cancel;

	private GridColumnEx gcEntry;

	private GridColumnEx gcFromTime;

	private GridColumnEx gcTo;

	private GridColumnEx gcRecordTime;

	private GridColumnEx gcRemarks;

	private ContextMenuStrip contextMenuStripDoc;

	private ToolStripMenuItem deleteToolStripMenuItem;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (FormMode)
		{
		case Enums.FormViewModeType.View:
			EnableDisableControl(isEnable: true);
			btn_Save.Text = "OK";
			WinCommons._CloseProcessing();
			break;
		case Enums.FormViewModeType.Add:
			EnableDisableControl(isEnable: true);
			btn_Save.Text = "Save";
			WinCommons._CloseProcessing();
			break;
		case Enums.FormViewModeType.Busy:
			WinCommons._OpenProcessing();
			EnableDisableControl(isEnable: false);
			break;
		}
	}

	public frm_AboutRangeTime()
	{
		InitializeComponent();
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		try
		{
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_AboutRangeTime_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.View)
			{
				XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo);
				_ = 7;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_AboutRangeTime_OnLoadAsync(object sender, EventArgs e)
	{
		try
		{
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
	}

	private async Task gv_Doc_CellValueChangedAsync(CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName.Equals("FromTime") || e.Column.FieldName.Equals("ToTime") || e.Column.FieldName.Equals("RecTime"))
			{
				AddNewRow();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.Add;
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		Enums.FormViewModeType currentView = FormMode;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if (FormMode != Enums.FormViewModeType.Add || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			FormMode = Enums.FormViewModeType.Busy;
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			HttpResult httpResult = await HttpClient.AboutRangeTimeClient.SaveData(dataSourceTable);
			if (httpResult.MessageCode == Enums.MessageCode.Success)
			{
				await LoadData();
				WinCommons.Success(Enums.MessageCode.Success, "");
				currentView = Enums.FormViewModeType.View;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = currentView;
	}

	private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		Enums.FormViewModeType currenView = FormMode;
		try
		{
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
			if (num > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				FormMode = Enums.FormViewModeType.Busy;
				HttpResult httpResult = await HttpClient.AboutRangeTimeClient.Delete(num);
				if (httpResult.MessageCode == Enums.MessageCode.Success)
				{
					await LoadData();
					WinCommons.Success(Enums.MessageCode.Success, "");
					currenView = Enums.FormViewModeType.View;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = currenView;
	}

	private void AddNewRow()
	{
		try
		{
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			if (dataSourceTable != null && dataSourceTable.Columns.Count != 0 && !dataSourceTable.AsEnumerable().Any((DataRow t) => Function.ParseInt(t["FromTime"]) == 0 && Function.ParseInt(t["ToTime"]) == 0 && Function.ParseInt(t["RecTime"]) == 0))
			{
				DataRow row = dataSourceTable.NewRow();
				dataSourceTable.Rows.Add(row);
				dataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void EnableDisableControl(bool isEnable)
	{
		SimpleButtonEx simpleButtonEx = btn_Cancel;
		SimpleButtonEx simpleButtonEx2 = btn_Save;
		bool flag = (grd_Doc.Enabled = isEnable);
		bool enabled = (simpleButtonEx2.Enabled = flag);
		simpleButtonEx.Enabled = enabled;
	}

	private async Task LoadData()
	{
		DataTable dataSource = await HttpClient.AboutRangeTimeClient.GetData();
		grd_Doc.DataSource = dataSource;
		grd_Doc.RefreshDataSource();
		AddNewRow();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.TimeSheeting.frm_AboutRangeTime));
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStripDoc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gcEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.gcFromTime = new Apzon.Controls.Grid.GridColumnEx();
		this.gcTo = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRecordTime = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRemarks = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStripDoc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		base.SuspendLayout();
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStripDoc;
		this.grd_Doc.Location = new System.Drawing.Point(2, 4);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.Size = new System.Drawing.Size(426, 265);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.contextMenuStripDoc.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteToolStripMenuItem });
		this.contextMenuStripDoc.Name = "contextMenuStripDoc";
		this.contextMenuStripDoc.Size = new System.Drawing.Size(108, 26);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gcEntry, this.gcFromTime, this.gcTo, this.gcRecordTime, this.gcRemarks });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = null;
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = null;
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Doc.OptionsClipboard.ClipboardMode = DevExpress.Export.ClipboardMode.Formatted;
		this.gv_Doc.OptionsPrint.AutoWidth = false;
		this.gv_Doc.OptionsPrint.UsePrintStyles = false;
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupedColumns = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.CellValueChangedAsync += new Apzon.Controls.Grid.CellValueChangedAsync(this.gv_Doc_CellValueChangedAsync);
		this.gcEntry.Caption = "DocEntry";
		this.gcEntry.CurrencyDateField = null;
		this.gcEntry.CurrencyField = null;
		this.gcEntry.CurrencyFixed = null;
		this.gcEntry.CurrencyFixedRate = 0.0;
		this.gcEntry.CurrencyRateField = null;
		this.gcEntry.FieldName = "DocEntry";
		this.gcEntry.Name = "gcEntry";
		this.gcEntry.PrimaryKey = true;
		this.gcEntry.UnitField = null;
		this.gcEntry.Visible = true;
		this.gcEntry.VisibleIndex = 0;
		this.gcFromTime.Caption = "From";
		this.gcFromTime.CurrencyDateField = null;
		this.gcFromTime.CurrencyField = null;
		this.gcFromTime.CurrencyFixed = null;
		this.gcFromTime.CurrencyFixedRate = 0.0;
		this.gcFromTime.CurrencyRateField = null;
		this.gcFromTime.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Time;
		this.gcFromTime.FieldName = "FromTime";
		this.gcFromTime.Name = "gcFromTime";
		this.gcFromTime.PrimaryKey = false;
		this.gcFromTime.UnitField = null;
		this.gcFromTime.Visible = true;
		this.gcFromTime.VisibleIndex = 1;
		this.gcTo.Caption = "To";
		this.gcTo.CurrencyDateField = null;
		this.gcTo.CurrencyField = null;
		this.gcTo.CurrencyFixed = null;
		this.gcTo.CurrencyFixedRate = 0.0;
		this.gcTo.CurrencyRateField = null;
		this.gcTo.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Time;
		this.gcTo.FieldName = "ToTime";
		this.gcTo.Name = "gcTo";
		this.gcTo.PrimaryKey = false;
		this.gcTo.UnitField = null;
		this.gcTo.Visible = true;
		this.gcTo.VisibleIndex = 2;
		this.gcRecordTime.Caption = "Record Time";
		this.gcRecordTime.CurrencyDateField = null;
		this.gcRecordTime.CurrencyField = null;
		this.gcRecordTime.CurrencyFixed = null;
		this.gcRecordTime.CurrencyFixedRate = 0.0;
		this.gcRecordTime.CurrencyRateField = null;
		this.gcRecordTime.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Time;
		this.gcRecordTime.FieldName = "RecTime";
		this.gcRecordTime.Name = "gcRecordTime";
		this.gcRecordTime.PrimaryKey = false;
		this.gcRecordTime.UnitField = null;
		this.gcRecordTime.Visible = true;
		this.gcRecordTime.VisibleIndex = 3;
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
		this.gcRemarks.VisibleIndex = 4;
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.AppearanceDisabled.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.AppearanceDisabled.Options.UseBackColor = true;
		this.btn_Save.AppearanceHovered.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Save.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.AppearancePressed.Options.UseBackColor = true;
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "Save";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(7, 276);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(75, 20);
		this.btn_Save.TabIndex = 1;
		this.btn_Save.Text = "Save";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Cancel.Appearance.Options.UseBackColor = true;
		this.btn_Cancel.AppearanceDisabled.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Cancel.AppearanceDisabled.Options.UseBackColor = true;
		this.btn_Cancel.AppearanceHovered.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Cancel.AppearanceHovered.Options.UseBackColor = true;
		this.btn_Cancel.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Cancel.AppearancePressed.Options.UseBackColor = true;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(89, 273);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(430, 300);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.grd_Doc);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_AboutRangeTime";
		this.Text = "About Range Time";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_AboutRangeTime_OnLoadAsync);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_AboutRangeTime_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStripDoc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		base.ResumeLayout(false);
	}
}
