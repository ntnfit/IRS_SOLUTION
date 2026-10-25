using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources.TimeSheeting;

public class frm_TimeSheetingHour : frm_TimeSheeting
{
	private IContainer components;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btCancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btnOK.Text = Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.View:
			btnOK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_TimeSheetingHour()
	{
		InitializeComponent();
		FormId = "oTimeSheetingHour";
		gridView1.ItemId = "APZ_OTSH";
		gridView1.FormId = FormId;
		gridView1.ChangeLogType = Enums.ChangeLog.clTimeSheetingHour;
		gridView1.IsApzTable = true;
		btn_CancelTimeSheet.Visible = false;
	}

	public override void CHANGELOG()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update)
			{
				string primaryKey = Function.ToString(gridView1.GetFocusedRowCellValue("Period")) + "," + Function.ToString(gridView1.GetFocusedRowCellValue("EmpId")) + "," + Function.ToString(luk_OvertimeType.EditValue);
				Enums.ChangeLog changeLog = Enums.ChangeLog.clTimeSheetingHour;
				GetChangeLogParams getChangeLogParams = new GetChangeLogParams();
				getChangeLogParams.PrimaryKey = primaryKey;
				getChangeLogParams.Object = changeLog;
				getChangeLogParams.IsApzTable = true;
				frm_ChangeLogList frm_ChangeLogList2 = new frm_ChangeLogList();
				frm_ChangeLogList2.GetChangeLogParams = getChangeLogParams;
				frm_ChangeLogList2.MdiParent = MdiParent;
				frm_ChangeLogList2.Show();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
	}

	public new virtual void SetMenuCode()
	{
		formMenuCode = "frm_TimeSheetingHour";
	}

	public override DataTable UnPivotData(out string mess)
	{
		mess = "";
		try
		{
			DataTable dataSourceTable = gridControl1.DataSourceTable;
			if (dataSourceTable.Rows.Count == 0)
			{
				mess = "Data is empty";
			}
			if (_dicWorkDay.Keys.Count <= 0)
			{
				return null;
			}
			DataTable dataTable = dataSourceTable.Clone();
			dataTable.Columns.Add("WorkDay", typeof(DateTime));
			dataTable.Columns.Add("WorkCode", typeof(double));
			dataTable.Columns.Add("OvertimeType", typeof(string));
			for (int i = 0; i < dataSourceTable.Rows.Count; i++)
			{
				dataSourceTable.Rows[i]["UserSign2"] = GlobalData.UserID;
				dataSourceTable.Rows[i]["UpdateDate"] = DateTime.Today;
				bool flag = false;
				foreach (string key in _dicWorkDay.Keys)
				{
					if (!string.IsNullOrEmpty(Function.ToString(dataSourceTable.Rows[i][key])))
					{
						flag = true;
						DataRow dataRow = dataTable.NewRow();
						dataRow.ItemArray = dataSourceTable.Rows[i].ItemArray;
						dataRow["WorkDay"] = Function.ParseDateTimes(key).Date;
						dataRow["WorkCode"] = Function.ParseDouble(dataSourceTable.Rows[i][key]);
						dataRow["OvertimeType"] = overtimeType;
						dataTable.Rows.Add(dataRow);
						dataTable.AcceptChanges();
					}
				}
				if (!flag)
				{
					DataRow dataRow2 = dataTable.NewRow();
					dataRow2.ItemArray = dataSourceTable.Rows[i].ItemArray;
					dataRow2["OvertimeType"] = overtimeType;
					dataTable.Rows.Add(dataRow2);
					dataTable.AcceptChanges();
				}
			}
			foreach (string key2 in _dicWorkDay.Keys)
			{
				dataTable.Columns.Remove(key2);
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	public override async Task<DataTable> LoadTimeSheetingSetting()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheetingHour");
			DataTable dataTable = await _help.GetAsync("LoadTimeSheetingHourSetting", new
			{
				userSign = GlobalData.UserName
			});
			if (!dataTable.IsNotNull())
			{
				dataTable = WinFuntions.GetTableStructure("APZ_TSHS");
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	protected override async Task<DataTable> GetOvertimeType()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheeting");
			DataTable dataTable = await _help.GetAsync("GetOvertimeType");
			if (!dataTable.IsNotNull())
			{
				dataTable = WinFuntions.GetTableStructure("APZ_OHOT");
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	protected override async Task<DataTable> LoadTimeSheeting()
	{
		try
		{
			int branch = Function.ParseInt(luk_Branch.EditValue);
			string dept = Function.ToString(ccb_Department.EditValue);
			int empID = Function.ParseInt(txt_EmpId.EditValue);
			string empName = Function.ToString(txt_EmpName.EditValue);
			string status = Function.ToString(ccb_Status.EditValue);
			string period = Function.ToString(luk_Period.EditValue);
			string workcode = Function.ToString(luk_OvertimeType.EditValue);
			overtimeType = Function.ToString(luk_OvertimeType.EditValue);
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheetingHour");
			return await helper.GetAsync("LoadTimeSheetingHour", new
			{
				branch = branch,
				dept = dept,
				empID = empID,
				empName = empName,
				status = status,
				period = period,
				workcode = workcode,
				baseEntry = BaseEntry
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public override async Task<DataSet> GetDataTimeSheetingCopy()
	{
		try
		{
			using HttpClientHelper<DataSet> _help = new HttpClientHelper<DataSet>(Configuration.ApiServer, "TimeSheetingHour");
			return await _help.GetAsync("LoadTimeSheetingHourCopyFrom", new
			{
				baseEntry = BaseEntry,
				workcode = overtimeType
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	public override void SetupTimeSheeting_Value()
	{
		try
		{
			if (dtSetting.IsNotNull())
			{
				DataRow dataRow = dtSetting.Rows[0];
				luk_Branch.EditValue = short.Parse(Function.ToString(Function.ParseInt(dataRow["Branch"])));
				ccb_Department.SetEditValue(Function.ToString(dataRow["Department"]));
				ccb_Status.SetEditValue(Function.ToString(dataRow["Status"]));
				luk_Period.EditValue = Function.ToString(dataRow["Period"]);
				luk_OvertimeType.EditValue = Function.ToString(dataRow["OvertimeType"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public override void SetupOvertimeType_LookUp()
	{
		try
		{
			if (dtAPZ_OHOT.IsNotNull())
			{
				DataTable dataTable = dtAPZ_OHOT.Copy();
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = "";
				dataRow["Name"] = "";
				dataTable.Rows.InsertAt(dataRow, 0);
				dataTable.AcceptChanges();
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["Code"] = "Define New";
				dataRow2["Name"] = "Define New";
				dataTable.Rows.Add(dataRow2);
				dataTable.AcceptChanges();
				luk_OvertimeType.Properties.NullText = "";
				luk_OvertimeType.Properties.ValueMember = "Code";
				luk_OvertimeType.Properties.DisplayMember = "Name";
				luk_OvertimeType.Properties.ShowHeader = false;
				luk_OvertimeType.Properties.DropDownRows = dataTable.Rows.Count;
				luk_OvertimeType.Properties.DataSource = dataTable.Copy();
				luk_OvertimeType.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public override void SaveTimeSheetingSetting()
	{
		try
		{
			if (dtSetting == null)
			{
				return;
			}
			DataTable dataTable = dtSetting.Clone();
			DataRow dataRow = dataTable.NewRow();
			dataRow["Branch"] = Function.ToString(luk_Branch.EditValue);
			dataRow["Department"] = Function.ToString(ccb_Department.EditValue);
			dataRow["Status"] = Function.ToString(ccb_Status.EditValue);
			dataRow["Period"] = Function.ToString(luk_Period.EditValue);
			dataRow["OvertimeType"] = overtimeType;
			dataRow["UserSign"] = GlobalData.UserName;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			using HttpClientHelper<HttpResult> httpClientHelper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "TimeSheetingHour");
			HttpResult httpResult = httpClientHelper.Post("SaveTimeSheetingHourSetting", dataTable);
			if (httpResult != null)
			{
				string.IsNullOrEmpty(httpResult.Message);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public override void SetupFormControls()
	{
	}

	protected override void CreateContextMenu()
	{
		_menu = new DXPopupMenu();
		DXMenuItem dXMenuItem = new DXMenuItem();
		dXMenuItem.Caption = Function.ToString("+1");
		dXMenuItem.Click += MenuGridClick;
		dXMenuItem.Tag = new Tuple<string, string>("", "1");
		_menu.Items.Add(dXMenuItem);
		DXMenuItem dXMenuItem2 = new DXMenuItem();
		dXMenuItem2.Caption = Function.ToString("+2");
		dXMenuItem2.Click += MenuGridClick;
		dXMenuItem2.Tag = new Tuple<string, string>("", "2");
		_menu.Items.Add(dXMenuItem2);
		DXMenuItem dXMenuItem3 = new DXMenuItem();
		dXMenuItem3.Caption = Function.ToString("+3");
		dXMenuItem3.Click += MenuGridClick;
		dXMenuItem3.Tag = new Tuple<string, string>("", "3");
		_menu.Items.Add(dXMenuItem3);
		DXMenuItem dXMenuItem4 = new DXMenuItem();
		dXMenuItem4.Caption = Function.ToString("+0.5");
		dXMenuItem4.Click += MenuGridClick;
		dXMenuItem4.Tag = new Tuple<string, string>("", "0.5");
		_menu.Items.Add(dXMenuItem4);
		DXMenuItem dXMenuItem5 = new DXMenuItem();
		dXMenuItem5.Caption = Resources.ButtonDelete;
		dXMenuItem5.Click += MenuGridClick;
		dXMenuItem5.Tag = new Tuple<string, string>(Resources.ButtonDelete, "-");
		_menu.Items.Add(dXMenuItem5);
	}

	protected override void MenuGridClick(object sender, EventArgs e)
	{
		try
		{
			if (!(sender is DXMenuItem dXMenuItem))
			{
				return;
			}
			Tuple<string, string> obj = dXMenuItem.Tag as Tuple<string, string>;
			decimal num = Function.ParseDecimal(obj.Item2);
			if (obj == null)
			{
				return;
			}
			GridCell[] selectedCells = gridView1.GetSelectedCells();
			gridView1.BeginUpdate();
			for (int num2 = selectedCells.Length - 1; num2 >= 0; num2--)
			{
				GridCell gridCell = selectedCells[num2];
				if (_dicWorkDay.ContainsKey(gridCell.Column.FieldName))
				{
					if (num.Equals(0m))
					{
						gridView1.SetRowCellValue(gridCell.RowHandle, gridCell.Column, null);
					}
					else
					{
						decimal num3 = Function.ParseDecimal(gridView1.GetRowCellValue(gridCell.RowHandle, gridCell.Column)) + num;
						gridView1.SetRowCellValue(gridCell.RowHandle, gridCell.Column, num3);
					}
				}
			}
			gridView1.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public override void gridView1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			if (e.MenuType == GridMenuType.Row && e.HitInfo.InRow && !e.HitInfo.InGroupRow && (e.HitInfo.Column == null || char.IsDigit(e.HitInfo.Column.Caption[0])) && btn_ClosePeriod.Enabled)
			{
				gridView1.GridControl.ContextMenuStrip = new ContextMenuStrip();
				e.Allow = true;
				Control control = gridControl1;
				_menu.ShowPopup(control, e.Point);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void btnExport_Click(object sender, EventArgs e)
	{
	}

	private async void ExportToExcel()
	{
		try
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = "Excel Files (*.xlsx)|*.xlsx";
			dlg.DefaultExt = "xlsx";
			dlg.ShowDialog(this);
			if (!string.IsNullOrEmpty(dlg.FileName))
			{
				DataTable dataTable = await GetExportData();
				grd_Export.DataSource = dataTable.Copy();
				grd_Export.RefreshDataSource();
				grd_Export.ExportToXlsx(dlg.FileName);
				WinCommons.Success(Enums.MessageCode.ExportSuccess, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GetExportData()
	{
		try
		{
			int branch = Function.ParseInt(luk_Branch.EditValue);
			string dept = Function.ToString(ccb_Department.EditValue);
			int empID = Function.ParseInt(txt_EmpId.EditValue);
			string empName = Function.ToString(txt_EmpName.EditValue);
			string status = Function.ToString(ccb_Status.EditValue);
			string period = Function.ToString(luk_Period.EditValue);
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheetingHour");
			return await helper.GetAsync("ExportTimeSheetingHour", new { branch, dept, empID, empName, status, period });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public override void SetupWorkDayRep()
	{
	}

	public override void SetupDictionary()
	{
		try
		{
			if (!dtAPZ_OHOT.IsNotNull())
			{
				return;
			}
			foreach (DataRow item in dtAPZ_OHOT.AsEnumerable())
			{
				_dicWorkRate[Function.ToString(item["Code"])] = Function.ParseDouble(Function.ToString(item["Rate"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public override async Task<HttpResult> SaveResult(DataTable dtNew)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "TimeSheetingHour");
			return await helper.PostAsync("SaveTimeSheetingHour", dtNew);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new HttpResult(Enums.MessageCode.Error, "Error");
		}
	}

	public override void SetWorkTotal(int focusedRow)
	{
		try
		{
			double num = 0.0;
			foreach (KeyValuePair<string, string> item in _dicWorkDay)
			{
				string obj = Function.ToString(gridView1.GetRowCellValue(focusedRow, item.Key));
				num += Function.ParseDouble(obj);
			}
			gridView1.SetRowCellValue(focusedRow, "TotalBefore", num);
			double num2 = Function.ParseDouble(gridView1.GetRowCellValue(focusedRow, "Extra"));
			gridView1.SetRowCellValue(focusedRow, "WorkTotal", num + num2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public override void SetWorkType(int rowHandle)
	{
	}

	public override bool CheckBeforeSearch()
	{
		if (Function.ToString(luk_Branch.EditValue) == "")
		{
			WinCommons.Error(Enums.MessageCode.BranchCannotBeEmpty, "");
			return false;
		}
		if (Function.ToString(luk_Period.EditValue) == "")
		{
			WinCommons.Error(Enums.MessageCode.PleaseSelectAPeriod, "");
			return false;
		}
		if (Function.ToString(luk_OvertimeType.EditValue) == "")
		{
			WinCommons.Error(Enums.MessageCode.PleaseSelectOTType, "");
			return false;
		}
		if (Function.ToString(txt_EmpId.EditValue) != "" && Function.ToString(txt_EmpId.EditValue) != "0" && Function.ParseInt(txt_EmpId.EditValue) == 0)
		{
			WinCommons.Error(Enums.MessageCode.PleaseEnterValidEmployeeNo, "");
			return false;
		}
		return true;
	}

	public override void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (gridView1.IsCellSelected(e.RowHandle, e.Column))
			{
				e.Appearance.BackColor = Color.FromArgb(226, 234, 253);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void SetVisiblePeriodButton()
	{
		btn_ClosePeriod.Visible = false;
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
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.Text = "Time Sheeting OT";
	}
}
