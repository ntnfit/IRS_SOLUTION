using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
using Apzon.Forms.Properties;
using DevExpress.Export;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Administrator;

public class frm_AccessLog : XtraFormEx
{
	private IContainer components;

	private SimpleButtonEx btn_OK;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Last;

	private GridViewEx gv_Last;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridControlEx grd_Detail;

	private GridViewEx gv_Detail;

	private LabelLine labelLine1;

	private DateEditEx date_From;

	private SimpleButtonEx simpleButtonEx1;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx9;

	private DateEditEx date_ToDate;

	private LabelLine labelLine2;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gridColumnEx11;

	private GridColumnEx gridColumnEx12;

	private GridColumnEx gridColumnEx13;

	private GridColumnEx gridColumnEx14;

	private GridColumnEx gridColumnEx15;

	private GridColumnEx gridColumnEx16;

	private GridColumnEx col_Status;

	private GridColumnEx gridColumnEx17;

	private GridColumnEx gridColumnEx18;

	public frm_AccessLog()
	{
		InitializeComponent();
	}

	private async Task LoadData()
	{
		_ = 1;
		try
		{
			grd_Last.DataSource = null;
			gv_Last.ShowLoadingPanel();
			date_From.EditValue = DateTime.Today;
			date_ToDate.EditValue = DateTime.Today;
			DataTable dtUser = await HttpClient.Commons.GetAccessLogUser();
			DataTable dataTable = await HttpClient.Commons.GetAccessLogLastLogin();
			if (dtUser.IsNotNull() && dataTable.IsNotNull())
			{
				Dictionary<string, DataRow> dictionary = new Dictionary<string, DataRow>();
				for (int i = 0; i < dtUser.Rows.Count; i++)
				{
					string key = Function.ToString(dtUser.Rows[i]["USER_CODE"]);
					if (!dictionary.ContainsKey(key))
					{
						dictionary[key] = dtUser.Rows[i];
					}
				}
				for (int j = 0; j < dataTable.Rows.Count; j++)
				{
					string key2 = Function.ToString(dataTable.Rows[j]["User"]);
					if (dictionary.ContainsKey(key2))
					{
						DataRow dataRow = dictionary[key2];
						dataTable.Rows[j]["Name"] = Function.ToString(dataRow["U_NAME"]);
						dataTable.Rows[j]["Branch"] = Function.ToString(dataRow["Branch"]);
						dataTable.Rows[j]["Department"] = Function.ToString(dataRow["Department"]);
					}
				}
				dataTable.AcceptChanges();
			}
			grd_Last.DataSource = dataTable;
			grd_Last.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
		finally
		{
			gv_Last.HideLoadingPanel();
		}
	}

	public async Task REFRESH_DATA()
	{
		await LoadData();
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void frm_AccessLog_Load(object sender, EventArgs e)
	{
		await LoadData();
	}

	private void gv_Last_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			if (e.Column == col_Status)
			{
				e.DisplayText = "";
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private async Task gv_Last_FocusedRowObjectChangedAsync(FocusedRowObjectChangedEventArgs e)
	{
		await ApplyFilter();
	}

	private async Task ApplyFilter()
	{
		try
		{
			grd_Detail.DataSource = null;
			gv_Detail.ShowLoadingPanel();
			string user = Function.ToString(gv_Last.GetFocusedRowCellValue("User"));
			string fromDate = Function.ParseDateTimes(date_From.EditValue).ToString("yyyy-MM-dd");
			string toDate = Function.ParseDateTimes(date_ToDate.EditValue).ToString("yyyy-MM-dd");
			DataTable dataSource = await HttpClient.Commons.GetAccessLog(user, fromDate, toDate);
			grd_Detail.DataSource = dataSource;
			grd_Detail.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
		finally
		{
			gv_Detail.HideLoadingPanel();
		}
	}

	private async void simpleButtonEx1_Click(object sender, EventArgs e)
	{
		await ApplyFilter();
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
		DevExpress.XtraGrid.GridFormatRule gridFormatRule = new DevExpress.XtraGrid.GridFormatRule();
		DevExpress.XtraEditors.FormatConditionRuleIconSet formatConditionRuleIconSet = new DevExpress.XtraEditors.FormatConditionRuleIconSet();
		DevExpress.XtraEditors.FormatConditionIconSet formatConditionIconSet = new DevExpress.XtraEditors.FormatConditionIconSet();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_AccessLog));
		this.col_Status = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Last = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Last = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx16 = new Apzon.Controls.Grid.GridColumnEx();
		this.date_ToDate = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.simpleButtonEx1 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.date_From = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.grd_Detail = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Detail = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx18 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Last).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Last).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Detail).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Detail).BeginInit();
		base.SuspendLayout();
		this.col_Status.Caption = "Status";
		this.col_Status.CurrencyDateField = null;
		this.col_Status.CurrencyField = null;
		this.col_Status.CurrencyFixed = null;
		this.col_Status.CurrencyFixedRate = 0.0;
		this.col_Status.CurrencyRateField = null;
		this.col_Status.FieldName = "IS_ONLINE";
		this.col_Status.Name = "col_Status";
		this.col_Status.PrimaryKey = true;
		this.col_Status.UnitField = null;
		this.col_Status.Visible = true;
		this.col_Status.VisibleIndex = 0;
		this.col_Status.Width = 50;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(13, 487);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 5;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 12);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Last);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.date_ToDate);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine2);
		this.splitContainerControl1.Panel2.Controls.Add(this.simpleButtonEx1);
		this.splitContainerControl1.Panel2.Controls.Add(this.date_From);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine1);
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Detail);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1141, 469);
		this.splitContainerControl1.SplitterPosition = 503;
		this.splitContainerControl1.TabIndex = 6;
		this.grd_Last.AllowPasteExcel = true;
		this.grd_Last.ClipboardData = null;
		this.grd_Last.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Last.Location = new System.Drawing.Point(0, 0);
		this.grd_Last.MainView = this.gv_Last;
		this.grd_Last.Name = "grd_Last";
		this.grd_Last.Size = new System.Drawing.Size(503, 469);
		this.grd_Last.TabIndex = 0;
		this.grd_Last.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Last });
		this.gv_Last.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Last.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Last.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Last.ColumnKey = null;
		this.gv_Last.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.col_Status, this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx13, this.gridColumnEx4, this.gridColumnEx14, this.gridColumnEx10, this.gridColumnEx11, this.gridColumnEx12,
			this.gridColumnEx15, this.gridColumnEx16
		});
		this.gv_Last.DisableFormSetting = false;
		this.gv_Last.DisplayException = false;
		this.gv_Last.FieldCurrency = null;
		this.gv_Last.FieldRate = null;
		gridFormatRule.Column = this.col_Status;
		gridFormatRule.ColumnApplyTo = this.col_Status;
		gridFormatRule.Name = "Format0";
		formatConditionRuleIconSet.AllowAnimation = DevExpress.Utils.DefaultBoolean.True;
		formatConditionIconSet.CategoryName = "Shapes";
		formatConditionIconSetIcon.Icon = Apzon.Forms.Properties.Resources.apply_16x16;
		formatConditionIconSetIcon.PredefinedName = "TrafficLights4_1.png";
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon);
		formatConditionIconSet.Name = "TrafficLights4";
		formatConditionRuleIconSet.IconSet = formatConditionIconSet;
		gridFormatRule.Rule = formatConditionRuleIconSet;
		this.gv_Last.FormatRules.Add(gridFormatRule);
		this.gv_Last.FormId = "AccessLog";
		this.gv_Last.GridControl = this.grd_Last;
		this.gv_Last.IndicatorWidth = 35;
		this.gv_Last.IsApzTable = false;
		this.gv_Last.IsInsertNewRow = false;
		this.gv_Last.IsLoadingData = false;
		this.gv_Last.ItemId = "AccessLogUser";
		this.gv_Last.LstColumnAllowBatch = null;
		this.gv_Last.LstColumnNotAllowBatch = null;
		this.gv_Last.LstColumnNotEditable = null;
		this.gv_Last.LstColumNotSetting = null;
		this.gv_Last.Name = "gv_Last";
		this.gv_Last.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Last.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Last.OptionsBehavior.Editable = false;
		this.gv_Last.OptionsBehavior.ReadOnly = true;
		this.gv_Last.OptionsClipboard.ClipboardMode = DevExpress.Export.ClipboardMode.Formatted;
		this.gv_Last.OptionsPrint.AutoWidth = false;
		this.gv_Last.OptionsPrint.UsePrintStyles = false;
		this.gv_Last.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Last.OptionsView.ColumnAutoWidth = false;
		this.gv_Last.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Last.OptionsView.ShowGroupedColumns = true;
		this.gv_Last.OptionsView.ShowGroupPanel = false;
		this.gv_Last.UseBatchMod = false;
		this.gv_Last.FocusedRowObjectChangedAsync += new Apzon.Controls.Grid.RaiseFocusedRowObjectChangedAsync(this.gv_Last_FocusedRowObjectChangedAsync);
		this.gv_Last.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_Last_CustomColumnDisplayText);
		this.gridColumnEx1.Caption = "User code";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "User";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 1;
		this.gridColumnEx1.Width = 90;
		this.gridColumnEx2.Caption = "User name";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "Name";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 2;
		this.gridColumnEx2.Width = 150;
		this.gridColumnEx3.Caption = "Last Login Date";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx3.FieldName = "LoginTime";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 3;
		this.gridColumnEx3.Width = 150;
		this.gridColumnEx13.Caption = "Last Login Time";
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.FieldName = "LoginHour";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.UnitField = null;
		this.gridColumnEx13.Visible = true;
		this.gridColumnEx13.VisibleIndex = 4;
		this.gridColumnEx4.Caption = "Last Logoff Date";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx4.FieldName = "LogoffTime";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 5;
		this.gridColumnEx4.Width = 150;
		this.gridColumnEx14.Caption = "Last Logoff Time";
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.FieldName = "LogoffHour";
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.UnitField = null;
		this.gridColumnEx14.Visible = true;
		this.gridColumnEx14.VisibleIndex = 6;
		this.gridColumnEx10.Caption = "IP";
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "ClientLocalIpAddress";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 7;
		this.gridColumnEx10.Width = 150;
		this.gridColumnEx11.Caption = "Machine";
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "ClientMachineName";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Visible = true;
		this.gridColumnEx11.VisibleIndex = 8;
		this.gridColumnEx11.Width = 150;
		this.gridColumnEx12.Caption = "Windows logon";
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "ClientWindowsLogonName";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx12.Visible = true;
		this.gridColumnEx12.VisibleIndex = 9;
		this.gridColumnEx12.Width = 150;
		this.gridColumnEx15.Caption = "Branch";
		this.gridColumnEx15.CurrencyDateField = null;
		this.gridColumnEx15.CurrencyField = null;
		this.gridColumnEx15.CurrencyFixed = null;
		this.gridColumnEx15.CurrencyFixedRate = 0.0;
		this.gridColumnEx15.CurrencyRateField = null;
		this.gridColumnEx15.FieldName = "Branch";
		this.gridColumnEx15.Name = "gridColumnEx15";
		this.gridColumnEx15.PrimaryKey = false;
		this.gridColumnEx15.UnitField = null;
		this.gridColumnEx15.Visible = true;
		this.gridColumnEx15.VisibleIndex = 10;
		this.gridColumnEx16.Caption = "Department";
		this.gridColumnEx16.CurrencyDateField = null;
		this.gridColumnEx16.CurrencyField = null;
		this.gridColumnEx16.CurrencyFixed = null;
		this.gridColumnEx16.CurrencyFixedRate = 0.0;
		this.gridColumnEx16.CurrencyRateField = null;
		this.gridColumnEx16.FieldName = "Department";
		this.gridColumnEx16.Name = "gridColumnEx16";
		this.gridColumnEx16.PrimaryKey = false;
		this.gridColumnEx16.UnitField = null;
		this.gridColumnEx16.Visible = true;
		this.gridColumnEx16.VisibleIndex = 11;
		this.date_ToDate.EditControl = null;
		this.date_ToDate.EditValue = null;
		this.date_ToDate.FieldName = null;
		this.date_ToDate.Location = new System.Drawing.Point(300, 10);
		this.date_ToDate.Name = "date_ToDate";
		this.date_ToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.IsSearchControl = false;
		this.date_ToDate.SettingInVisible = false;
		this.date_ToDate.SettingReadonly = false;
		this.date_ToDate.Size = new System.Drawing.Size(128, 20);
		this.date_ToDate.TabIndex = 5;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "To Date";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(220, 13);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 4;
		this.labelLine2.Utext = "To Date";
		this.simpleButtonEx1.EditControl = null;
		this.simpleButtonEx1.EditValue = "Apply filter";
		this.simpleButtonEx1.FieldName = null;
		this.simpleButtonEx1.IsSearchControl = false;
		this.simpleButtonEx1.Location = new System.Drawing.Point(448, 7);
		this.simpleButtonEx1.Name = "simpleButtonEx1";
		this.simpleButtonEx1.ReadOnly = false;
		this.simpleButtonEx1.SettingInVisible = false;
		this.simpleButtonEx1.SettingReadonly = false;
		this.simpleButtonEx1.Size = new System.Drawing.Size(75, 23);
		this.simpleButtonEx1.TabIndex = 3;
		this.simpleButtonEx1.Text = "Apply filter";
		this.simpleButtonEx1.Click += new System.EventHandler(this.simpleButtonEx1_Click);
		this.date_From.EditControl = null;
		this.date_From.EditValue = null;
		this.date_From.FieldName = null;
		this.date_From.Location = new System.Drawing.Point(86, 10);
		this.date_From.Name = "date_From";
		this.date_From.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_From.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_From.Properties.IsSearchControl = false;
		this.date_From.SettingInVisible = false;
		this.date_From.SettingReadonly = false;
		this.date_From.Size = new System.Drawing.Size(128, 20);
		this.date_From.TabIndex = 2;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "From Date";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(6, 13);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 1;
		this.labelLine1.Utext = "From Date";
		this.grd_Detail.AllowPasteExcel = true;
		this.grd_Detail.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Detail.ClipboardData = null;
		this.grd_Detail.Location = new System.Drawing.Point(6, 45);
		this.grd_Detail.MainView = this.gv_Detail;
		this.grd_Detail.Name = "grd_Detail";
		this.grd_Detail.Size = new System.Drawing.Size(622, 424);
		this.grd_Detail.TabIndex = 0;
		this.grd_Detail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Detail });
		this.gv_Detail.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Detail.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Detail.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Detail.ColumnKey = null;
		this.gv_Detail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gridColumnEx5, this.gridColumnEx17, this.gridColumnEx6, this.gridColumnEx18, this.gridColumnEx7, this.gridColumnEx8, this.gridColumnEx9 });
		this.gv_Detail.DisableFormSetting = false;
		this.gv_Detail.DisplayException = false;
		this.gv_Detail.FieldCurrency = null;
		this.gv_Detail.FieldRate = null;
		this.gv_Detail.FormId = null;
		this.gv_Detail.GridControl = this.grd_Detail;
		this.gv_Detail.IndicatorWidth = 35;
		this.gv_Detail.IsApzTable = false;
		this.gv_Detail.IsInsertNewRow = false;
		this.gv_Detail.IsLoadingData = false;
		this.gv_Detail.ItemId = null;
		this.gv_Detail.LstColumnAllowBatch = null;
		this.gv_Detail.LstColumnNotAllowBatch = null;
		this.gv_Detail.LstColumnNotEditable = null;
		this.gv_Detail.LstColumNotSetting = null;
		this.gv_Detail.Name = "gv_Detail";
		this.gv_Detail.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Detail.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Detail.OptionsBehavior.Editable = false;
		this.gv_Detail.OptionsClipboard.ClipboardMode = DevExpress.Export.ClipboardMode.Formatted;
		this.gv_Detail.OptionsPrint.AutoWidth = false;
		this.gv_Detail.OptionsPrint.UsePrintStyles = false;
		this.gv_Detail.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Detail.OptionsView.ColumnAutoWidth = false;
		this.gv_Detail.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Detail.OptionsView.ShowGroupedColumns = true;
		this.gv_Detail.OptionsView.ShowGroupPanel = false;
		this.gv_Detail.UseBatchMod = false;
		this.gridColumnEx5.Caption = "Login Date";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx5.FieldName = "LoginTime";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = true;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 0;
		this.gridColumnEx5.Width = 150;
		this.gridColumnEx17.Caption = "Login Time";
		this.gridColumnEx17.CurrencyDateField = null;
		this.gridColumnEx17.CurrencyField = null;
		this.gridColumnEx17.CurrencyFixed = null;
		this.gridColumnEx17.CurrencyFixedRate = 0.0;
		this.gridColumnEx17.CurrencyRateField = null;
		this.gridColumnEx17.FieldName = "LoginHour";
		this.gridColumnEx17.Name = "gridColumnEx17";
		this.gridColumnEx17.PrimaryKey = false;
		this.gridColumnEx17.UnitField = null;
		this.gridColumnEx17.Visible = true;
		this.gridColumnEx17.VisibleIndex = 1;
		this.gridColumnEx6.Caption = "Logoff Date";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx6.FieldName = "LogoffTime";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 2;
		this.gridColumnEx6.Width = 150;
		this.gridColumnEx18.Caption = "Logoff Time";
		this.gridColumnEx18.CurrencyDateField = null;
		this.gridColumnEx18.CurrencyField = null;
		this.gridColumnEx18.CurrencyFixed = null;
		this.gridColumnEx18.CurrencyFixedRate = 0.0;
		this.gridColumnEx18.CurrencyRateField = null;
		this.gridColumnEx18.FieldName = "LogoffHour";
		this.gridColumnEx18.Name = "gridColumnEx18";
		this.gridColumnEx18.PrimaryKey = false;
		this.gridColumnEx18.UnitField = null;
		this.gridColumnEx18.Visible = true;
		this.gridColumnEx18.VisibleIndex = 3;
		this.gridColumnEx7.Caption = "IP Address";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "ClientLocalIpAddress";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 4;
		this.gridColumnEx7.Width = 150;
		this.gridColumnEx8.Caption = "Machine";
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "ClientMachineName";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 5;
		this.gridColumnEx8.Width = 150;
		this.gridColumnEx9.Caption = "Windows logon";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "ClientWindowsLogonName";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 6;
		this.gridColumnEx9.Width = 150;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1142, 513);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_OK);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.FormId = "AccessLog";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_AccessLog.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_AccessLog";
		base.ShowInTaskbar = false;
		this.Text = "Access Log";
		base.Load += new System.EventHandler(this.frm_AccessLog_Load);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Last).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Last).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Detail).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Detail).EndInit();
		base.ResumeLayout(false);
	}
}
