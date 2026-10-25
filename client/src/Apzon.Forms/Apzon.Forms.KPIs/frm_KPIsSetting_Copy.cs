using System;
using System.Collections.Generic;
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
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.KPIs;

public class frm_KPIsSetting_Copy : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public new Func<Task> ReloadSettingForm;

	private IContainer components;

	private GridControlEx grd_Copy;

	private GridViewEx gv_Copy;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	private GridColumnEx gc_empID;

	private GridColumnEx gc_empName;

	private GridColumnEx gc_jobTitle;

	private GridColumnEx gc_PositionName;

	private GridColumnEx gc_Department;

	private GridColumnEx gc_BranchName;

	private GridColumnEx gc_ManagerName;

	private GridColumnEx gc_ExtEmpNo;

	public int AbsEntry { get; set; }

	public frm_KPIsSetting_Copy()
	{
		InitializeComponent();
		OnLoadAsync += frm_FormSetting_Copy_Load;
	}

	private async Task frm_FormSetting_Copy_Load(object sender, EventArgs e)
	{
		try
		{
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCopyKpiSetting, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await CopyFormSetting(copyall: false);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadData()
	{
		try
		{
			DataTable dataSource = await _httpClient.KPIsSettingClient.LoadCopySetting(AbsEntry);
			grd_Copy.DataSource = dataSource;
			grd_Copy.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public static async Task RefreshKpiSettingForm()
	{
		try
		{
			List<object> list = new List<object>();
			for (int i = 0; i < Application.OpenForms.Count; i++)
			{
				dynamic val = Application.OpenForms[i];
				if (val.GetType() == typeof(frm_KPIsSetting))
				{
					list.Add(val);
				}
			}
			foreach (dynamic item in list)
			{
				if (((frm_KPIsSetting)item).GetType().GetMethod("ReloadSettingForm") != null)
				{
					await item.ReloadSettingForm();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async Task CopyFormSetting(bool copyall)
	{
		try
		{
			WinCommons._OpenProcessing();
			List<string> list = new List<string>();
			int[] selectedRows = gv_Copy.GetSelectedRows();
			if (selectedRows.Length != 0)
			{
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("AbsEntry", typeof(int));
				dataTable.Columns.Add("CopyEmployees", typeof(string));
				dataTable.Columns.Add("CopyAll", typeof(string));
				dataTable.Columns.Add("UserUpdate", typeof(int));
				list.AddRange(from t in selectedRows
					select Function.ToString(gv_Copy.GetRowCellValue(t, "empID")) into user
					where !string.IsNullOrWhiteSpace(user)
					select user);
				DataRow dataRow = dataTable.NewRow();
				dataRow["AbsEntry"] = AbsEntry;
				dataRow["CopyEmployees"] = string.Join(",", list.ToArray());
				dataRow["CopyAll"] = (copyall ? "Y" : "N");
				dataRow["UserUpdate"] = GlobalData.UserID;
				dataTable.Rows.Add(dataRow);
				HttpResult httpResult = await _httpClient.KPIsSettingClient.SaveCopySetting(dataTable);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.None, "Sao chép thành công!");
					WinCommons._CloseProcessing();
					RefreshKpiSettingForm();
					Close();
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
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
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
		this.grd_Copy = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Copy = new Apzon.Controls.Grid.GridViewEx();
		this.gc_empID = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_empName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_jobTitle = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_PositionName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Department = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_BranchName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ManagerName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ExtEmpNo = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Copy).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Copy).BeginInit();
		base.SuspendLayout();
		this.grd_Copy.AllowPasteExcel = true;
		this.grd_Copy.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Copy.ClipboardData = null;
		this.grd_Copy.Location = new System.Drawing.Point(1, -3);
		this.grd_Copy.MainView = this.gv_Copy;
		this.grd_Copy.Name = "grd_Copy";
		this.grd_Copy.Size = new System.Drawing.Size(585, 400);
		this.grd_Copy.TabIndex = 0;
		this.grd_Copy.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Copy });
		this.gv_Copy.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Copy.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Copy.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Copy.ColumnKey = null;
		this.gv_Copy.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gc_empID, this.gc_empName, this.gc_jobTitle, this.gc_PositionName, this.gc_Department, this.gc_BranchName, this.gc_ManagerName, this.gc_ExtEmpNo });
		this.gv_Copy.DisableFormSetting = false;
		this.gv_Copy.DisplayException = false;
		this.gv_Copy.EditColumn = null;
		this.gv_Copy.FieldCurrency = null;
		this.gv_Copy.FieldRate = null;
		this.gv_Copy.FormId = null;
		this.gv_Copy.GridControl = this.grd_Copy;
		this.gv_Copy.IgnoreFilter = false;
		this.gv_Copy.IndexEditRow = -1;
		this.gv_Copy.IndicatorWidth = 35;
		this.gv_Copy.IsApzTable = false;
		this.gv_Copy.IsInsertNewRow = false;
		this.gv_Copy.IsLoadingData = false;
		this.gv_Copy.IsMappingContextAction = false;
		this.gv_Copy.ItemId = null;
		this.gv_Copy.LstColumnAllowBatch = null;
		this.gv_Copy.LstColumnNotAllowBatch = null;
		this.gv_Copy.LstColumnNotEditable = null;
		this.gv_Copy.LstColumNotSetting = null;
		this.gv_Copy.Name = "gv_Copy";
		this.gv_Copy.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Copy.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Copy.OptionsPrint.AutoWidth = false;
		this.gv_Copy.OptionsPrint.UsePrintStyles = false;
		this.gv_Copy.OptionsSelection.MultiSelect = true;
		this.gv_Copy.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Copy.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Copy.OptionsView.ColumnAutoWidth = false;
		this.gv_Copy.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Copy.OptionsView.ShowGroupedColumns = true;
		this.gv_Copy.OptionsView.ShowGroupPanel = false;
		this.gv_Copy.UseBatchMod = false;
		this.gc_empID.Caption = "Mã nhân viên";
		this.gc_empID.CurrencyDateField = null;
		this.gc_empID.CurrencyField = null;
		this.gc_empID.CurrencyFixed = null;
		this.gc_empID.CurrencyFixedRate = 0.0;
		this.gc_empID.CurrencyRateField = null;
		this.gc_empID.FieldName = "empID";
		this.gc_empID.Name = "gc_empID";
		this.gc_empID.OptionsColumn.ReadOnly = true;
		this.gc_empID.PrimaryKey = true;
		this.gc_empID.UnitField = null;
		this.gc_empID.Visible = true;
		this.gc_empID.VisibleIndex = 1;
		this.gc_empName.Caption = "Tên nhân viên";
		this.gc_empName.CurrencyDateField = null;
		this.gc_empName.CurrencyField = null;
		this.gc_empName.CurrencyFixed = null;
		this.gc_empName.CurrencyFixedRate = 0.0;
		this.gc_empName.CurrencyRateField = null;
		this.gc_empName.FieldName = "empName";
		this.gc_empName.Name = "gc_empName";
		this.gc_empName.OptionsColumn.ReadOnly = true;
		this.gc_empName.PrimaryKey = false;
		this.gc_empName.UnitField = null;
		this.gc_empName.Visible = true;
		this.gc_empName.VisibleIndex = 2;
		this.gc_jobTitle.Caption = "Chức danh";
		this.gc_jobTitle.CurrencyDateField = null;
		this.gc_jobTitle.CurrencyField = null;
		this.gc_jobTitle.CurrencyFixed = null;
		this.gc_jobTitle.CurrencyFixedRate = 0.0;
		this.gc_jobTitle.CurrencyRateField = null;
		this.gc_jobTitle.FieldName = "jobTitle";
		this.gc_jobTitle.Name = "gc_jobTitle";
		this.gc_jobTitle.OptionsColumn.ReadOnly = true;
		this.gc_jobTitle.PrimaryKey = false;
		this.gc_jobTitle.UnitField = null;
		this.gc_jobTitle.Visible = true;
		this.gc_jobTitle.VisibleIndex = 3;
		this.gc_PositionName.Caption = "Vị trí";
		this.gc_PositionName.CurrencyDateField = null;
		this.gc_PositionName.CurrencyField = null;
		this.gc_PositionName.CurrencyFixed = null;
		this.gc_PositionName.CurrencyFixedRate = 0.0;
		this.gc_PositionName.CurrencyRateField = null;
		this.gc_PositionName.FieldName = "PositionName";
		this.gc_PositionName.Name = "gc_PositionName";
		this.gc_PositionName.OptionsColumn.ReadOnly = true;
		this.gc_PositionName.PrimaryKey = false;
		this.gc_PositionName.UnitField = null;
		this.gc_PositionName.Visible = true;
		this.gc_PositionName.VisibleIndex = 4;
		this.gc_Department.Caption = "Phòng ban";
		this.gc_Department.CurrencyDateField = null;
		this.gc_Department.CurrencyField = null;
		this.gc_Department.CurrencyFixed = null;
		this.gc_Department.CurrencyFixedRate = 0.0;
		this.gc_Department.CurrencyRateField = null;
		this.gc_Department.FieldName = "Department";
		this.gc_Department.Name = "gc_Department";
		this.gc_Department.OptionsColumn.ReadOnly = true;
		this.gc_Department.PrimaryKey = false;
		this.gc_Department.UnitField = null;
		this.gc_Department.Visible = true;
		this.gc_Department.VisibleIndex = 5;
		this.gc_BranchName.Caption = "Chi nhánh";
		this.gc_BranchName.CurrencyDateField = null;
		this.gc_BranchName.CurrencyField = null;
		this.gc_BranchName.CurrencyFixed = null;
		this.gc_BranchName.CurrencyFixedRate = 0.0;
		this.gc_BranchName.CurrencyRateField = null;
		this.gc_BranchName.FieldName = "BranchName";
		this.gc_BranchName.Name = "gc_BranchName";
		this.gc_BranchName.OptionsColumn.ReadOnly = true;
		this.gc_BranchName.PrimaryKey = false;
		this.gc_BranchName.UnitField = null;
		this.gc_BranchName.Visible = true;
		this.gc_BranchName.VisibleIndex = 6;
		this.gc_ManagerName.Caption = "Quản lý";
		this.gc_ManagerName.CurrencyDateField = null;
		this.gc_ManagerName.CurrencyField = null;
		this.gc_ManagerName.CurrencyFixed = null;
		this.gc_ManagerName.CurrencyFixedRate = 0.0;
		this.gc_ManagerName.CurrencyRateField = null;
		this.gc_ManagerName.FieldName = "ManagerName";
		this.gc_ManagerName.Name = "gc_ManagerName";
		this.gc_ManagerName.OptionsColumn.ReadOnly = true;
		this.gc_ManagerName.PrimaryKey = false;
		this.gc_ManagerName.UnitField = null;
		this.gc_ManagerName.Visible = true;
		this.gc_ManagerName.VisibleIndex = 7;
		this.gc_ExtEmpNo.Caption = "Mã nhân viên mở rộng";
		this.gc_ExtEmpNo.CurrencyDateField = null;
		this.gc_ExtEmpNo.CurrencyField = null;
		this.gc_ExtEmpNo.CurrencyFixed = null;
		this.gc_ExtEmpNo.CurrencyFixedRate = 0.0;
		this.gc_ExtEmpNo.CurrencyRateField = null;
		this.gc_ExtEmpNo.FieldName = "ExtEmpNo";
		this.gc_ExtEmpNo.Name = "gc_ExtEmpNo";
		this.gc_ExtEmpNo.OptionsColumn.ReadOnly = true;
		this.gc_ExtEmpNo.PrimaryKey = false;
		this.gc_ExtEmpNo.UnitField = null;
		this.gc_ExtEmpNo.Visible = true;
		this.gc_ExtEmpNo.VisibleIndex = 8;
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(82, 403);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 13;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(1, 403);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 12;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(598, 432);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.grd_Copy);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_KPIsSetting_Copy";
		this.Text = "Sao chép thiết lập KPIs";
		((System.ComponentModel.ISupportInitialize)this.grd_Copy).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Copy).EndInit();
		base.ResumeLayout(false);
	}
}
