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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.MTS_Manufacturing;

public class FrmDefinedShift : XtraFormEx
{
	private const string OkText = "OK";

	private const string UpdateText = "Update";

	private const string SaveText = "Save";

	private const string Controller = "Shift";

	public string findCode = "";

	private IContainer components;

	private TimeEdit txt_BreakEnd2;

	private TimeEdit txt_BreakBegin2;

	private TimeEdit txt_EndTime2;

	private TimeEdit txt_StartTime2;

	private CheckEditEx check_Time2;

	private TimeEdit txt_BreakEnd1;

	private TimeEdit txt_BreakBegins1;

	private TimeEdit txt_EndTime1;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btn_Refresh;

	private SimpleButtonEx btn_Save;

	private MemoEditEx txt_Remarks;

	private CalcEditEx txt_Time;

	private TextEditEx txt_Name;

	private TextEditEx txt_Code;

	private GridColumnEx gc_Code;

	private GridColumnEx gc_Name;

	private GridControlEx grd_Shift;

	private GroupControl groupControl1;

	private GroupControl groupControl2;

	private LabelLine labelLine2;

	private LabelLine labelLine1;

	private LabelLine labelLine4;

	private LabelLine labelLine3;

	private LabelLine labelLine10;

	private LabelLine labelLine9;

	private LabelLine labelLine8;

	private LabelLine labelLine7;

	private LabelLine labelLine6;

	private LabelLine labelLine5;

	private LabelLine labelLine13;

	private LabelLine labelLine12;

	private LabelLine labelLine11;

	private GridViewEx gv_Shift;

	private ContextMenuStrip contextMenuStrip;

	private ToolStripMenuItem addToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private LabelLine lbBranch;

	private LookUpEdit txt_Branch;

	private TimeEdit txt_StartTime1;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btnCancel.Enabled = true;
			return;
		}
		if (FormAuth == Enums.FormAuthorities.Read)
		{
			_formMode = Enums.FormViewModeType.View;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.View:
			EnableDisableControl(isEnable: true);
			txt_Code.Properties.ReadOnly = true;
			btn_Save.Text = "OK";
			break;
		case Enums.FormViewModeType.Update:
			EnableDisableControl(isEnable: true);
			txt_Code.Properties.ReadOnly = true;
			btn_Save.Text = "Update";
			break;
		case Enums.FormViewModeType.Add:
			EnableDisableControl(isEnable: true);
			txt_Code.Properties.ReadOnly = false;
			btn_Save.Text = "Save";
			contextMenuStrip.Enabled = false;
			break;
		case Enums.FormViewModeType.Busy:
			EnableDisableControl(isEnable: false);
			break;
		}
	}

	public FrmDefinedShift()
	{
		InitializeComponent();
		OnLoadAsync += FrmDefinedShift_Load;
		FormMode = Enums.FormViewModeType.View;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private async void RefreshData(bool isRowRefresh = true)
	{
		try
		{
			if (!isRowRefresh)
			{
				await LoadData();
			}
			SetDataControl(gv_Shift.GetFocusedDataRow());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetDataControl(DataRow row)
	{
		try
		{
			if (row == null)
			{
				txt_Code.Text = "";
				txt_Name.Text = "";
				txt_StartTime1.EditValue = TimeSpan.Parse("00:00");
				txt_StartTime2.EditValue = TimeSpan.Parse("00:00");
				txt_Remarks.Text = "";
				txt_Time.EditValue = 0;
				check_Time2.EditValue = "N";
				txt_EndTime2.EditValue = TimeSpan.Parse("00:00");
				txt_EndTime1.EditValue = TimeSpan.Parse("00:00");
				txt_BreakEnd2.EditValue = TimeSpan.Parse("00:00");
				txt_BreakEnd1.EditValue = TimeSpan.Parse("00:00");
				txt_BreakBegins1.EditValue = TimeSpan.Parse("00:00");
				txt_BreakBegin2.EditValue = TimeSpan.Parse("00:00");
				txt_BreakBegin2.Enabled = check_Time2.EditValue.Equals("Y");
				txt_BreakEnd2.Enabled = check_Time2.EditValue.Equals("Y");
				txt_EndTime2.Enabled = check_Time2.EditValue.Equals("Y");
				txt_StartTime2.Enabled = check_Time2.EditValue.Equals("Y");
			}
			else
			{
				txt_Code.Text = Function.ToString(row["Code"]);
				txt_Name.Text = Function.ToString(row["Name"]);
				txt_StartTime1.EditValue = Function.ParseIntToTimeSpan(Function.ParseInt(row["Start1"]));
				txt_StartTime2.EditValue = Function.ParseIntToTimeSpan(Function.ParseInt(row["Start2"]));
				txt_Remarks.Text = Function.ToString(row["Notes"]);
				txt_EndTime2.EditValue = Function.ParseIntToTimeSpan(Function.ParseInt(row["End2"]));
				txt_EndTime1.EditValue = Function.ParseIntToTimeSpan(Function.ParseInt(row["End1"]));
				txt_BreakEnd2.EditValue = Function.ParseIntToTimeSpan(Function.ParseInt(row["EndBreak2"]));
				txt_BreakEnd1.EditValue = Function.ParseIntToTimeSpan(Function.ParseInt(row["EndBreak1"]));
				txt_BreakBegins1.EditValue = Function.ParseIntToTimeSpan(Function.ParseInt(row["StartBreak1"]));
				txt_BreakBegin2.EditValue = Function.ParseIntToTimeSpan(Function.ParseInt(row["StartBreak2"]));
				check_Time2.EditValue = Function.ToString(row["Time2"]);
				txt_Time.EditValue = Function.ParseDouble(row["Time"]);
				txt_BreakBegin2.Enabled = check_Time2.EditValue.Equals("Y");
				txt_BreakEnd2.Enabled = check_Time2.EditValue.Equals("Y");
				txt_EndTime2.Enabled = check_Time2.EditValue.Equals("Y");
				txt_StartTime2.Enabled = check_Time2.EditValue.Equals("Y");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MODE_FIND()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			AddNew();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.Add;
		WinCommons._CloseProcessing();
	}

	private void EnableDisableControl(bool isEnable)
	{
		try
		{
			txt_Branch.ReadOnly = !isEnable;
			txt_BreakBegin2.ReadOnly = !isEnable;
			txt_BreakBegins1.ReadOnly = !isEnable;
			txt_BreakEnd1.ReadOnly = !isEnable;
			txt_BreakEnd2.ReadOnly = !isEnable;
			txt_Code.ReadOnly = !isEnable;
			txt_EndTime1.ReadOnly = !isEnable;
			txt_EndTime2.ReadOnly = !isEnable;
			txt_Name.ReadOnly = !isEnable;
			txt_Remarks.ReadOnly = !isEnable;
			txt_StartTime1.ReadOnly = !isEnable;
			txt_StartTime2.ReadOnly = !isEnable;
			btnCancel.Enabled = isEnable;
			btn_Refresh.Enabled = isEnable;
			btn_Save.Enabled = isEnable;
			check_Time2.Enabled = isEnable;
			contextMenuStrip.Enabled = isEnable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNew()
	{
		try
		{
			int focusedRowHandle = gv_Shift.FocusedRowHandle;
			DataTable dataTable = grd_Shift.DataSourceTable.Copy();
			if (dataTable.Rows.Count == 0)
			{
				dataTable = GetColumn();
			}
			else
			{
				dataTable.Clear();
				DataRow row = dataTable.NewRow();
				dataTable.Rows.Add(row);
			}
			dataTable.AcceptChanges();
			grd_Shift.DataSource = dataTable;
			grd_Shift.RefreshDataSource();
			gv_Shift.FocusedRowHandle = 0;
			if (gv_Shift.FocusedRowHandle == focusedRowHandle)
			{
				SetDataControl(null);
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
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Shift");
			DataTable dataSource = await http.GetAsync("GetData", new
			{
				branch = Function.ParseInt(txt_Branch.EditValue)
			});
			grd_Shift.DataSource = dataSource;
			grd_Shift.RefreshDataSource();
			grd_Shift.DataSourceTable.AcceptChanges();
			if (gv_Shift.FocusedRowHandle < 0)
			{
				SetDataControl(gv_Shift.GetFocusedDataRow());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataTable GetColumn()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Shift");
			return httpClientHelper.Get("GetColumn");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private string GetDataSave(ref DataRow row)
	{
		if (row == null)
		{
			return "Data must not null";
		}
		row["BplId"] = -1;
		if (GlobalData.IsMultiBranch)
		{
			row["BplId"] = Function.ParseInt(txt_Branch.EditValue);
		}
		row["Code"] = txt_Code.Text;
		row["Name"] = txt_Name.Text;
		row["Start1"] = Function.ParseTimeSpanToInt(TimeSpan.Parse(Function.ToString(txt_StartTime1.Text)));
		row["Start2"] = Function.ParseTimeSpanToInt(TimeSpan.Parse(Function.ToString(txt_StartTime2.Text)));
		row["Notes"] = txt_Remarks.Text;
		row["End2"] = Function.ParseTimeSpanToInt(TimeSpan.Parse(Function.ToString(txt_EndTime2.Text)));
		row["End1"] = Function.ParseTimeSpanToInt(TimeSpan.Parse(Function.ToString(txt_EndTime1.Text)));
		row["EndBreak2"] = Function.ParseTimeSpanToInt(TimeSpan.Parse(Function.ToString(txt_BreakEnd2.Text)));
		row["EndBreak1"] = Function.ParseTimeSpanToInt(TimeSpan.Parse(Function.ToString(txt_BreakEnd1.Text)));
		row["StartBreak1"] = Function.ParseTimeSpanToInt(TimeSpan.Parse(Function.ToString(txt_BreakBegins1.Text)));
		row["StartBreak2"] = Function.ParseTimeSpanToInt(TimeSpan.Parse(Function.ToString(txt_BreakBegin2.Text)));
		row["Time2"] = Function.ToString(check_Time2.EditValue);
		row["Time"] = Function.ParseDouble(txt_Time.EditValue);
		return "";
	}

	private async Task Save()
	{
		WinCommons._OpenProcessing();
		Enums.FormViewModeType oldview = FormMode;
		string action = ((FormMode == Enums.FormViewModeType.Add) ? "Save" : "Update");
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			grd_Shift.RefreshDataSource();
			grd_Shift.DataSourceTable.AcceptChanges();
			DataRow row = gv_Shift.GetFocusedDataRow();
			string text = GetDataSave(ref row);
			DataTable dataTable = grd_Shift.DataSourceTable.Clone();
			dataTable.ImportRow(row);
			if (string.IsNullOrEmpty(text))
			{
				using (HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Shift"))
				{
					text = (await http.PostAsync(action, dataTable)).Message;
				}
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(action.Equals("Save") ? Enums.MessageCode.SaveSuccess : Enums.MessageCode.UpdateSuccess, "");
					await LoadData();
					FormMode = Enums.FormViewModeType.View;
					return;
				}
				WinCommons.Error(Enums.MessageCode.None, text);
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = oldview;
		WinCommons._CloseProcessing();
	}

	private async Task Delete()
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
		{
			return;
		}
		Enums.FormViewModeType oldView = FormMode;
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			int num = Function.ParseInt(gv_Shift.GetFocusedRowCellValue("SpID"));
			using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Shift");
			HttpResult httpResult = await http.PostAsync("Delete", num);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				await LoadData();
				WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
			WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = oldView;
		WinCommons._CloseProcessing();
	}

	private void GetTimeTotal()
	{
		TimeSpan t = TimeSpan.Parse(Function.ToString(txt_StartTime1.Text));
		TimeSpan t2 = TimeSpan.Parse(Function.ToString(txt_StartTime2.Text));
		TimeSpan t3 = TimeSpan.Parse(Function.ToString(txt_EndTime1.Text));
		TimeSpan t4 = TimeSpan.Parse(Function.ToString(txt_EndTime2.Text));
		txt_Time.EditValue = (double)(GetTime(t, t3) + GetTime(t2, t4)) / 60.0;
	}

	private int GetTime(TimeSpan t1, TimeSpan t2)
	{
		return (t2.Hours - t1.Hours) * 60 + t2.Minutes - t1.Minutes;
	}

	private string CheckDataSave()
	{
		if (GlobalData.IsMultiBranch && Function.ParseInt(txt_Branch.EditValue) == 0)
		{
			txt_Branch.Focus();
			return "You must choose branch to save!";
		}
		if (string.IsNullOrEmpty(Function.ToString(gv_Shift.GetFocusedRowCellValue(gc_Code))))
		{
			txt_Code.Focus();
			return "Shift Code must not null!";
		}
		if (TimeSpan.Parse(Function.ToString(txt_StartTime1.Text)) > TimeSpan.Parse(Function.ToString(txt_EndTime1.Text)))
		{
			txt_StartTime1.Focus();
			return "start time 1  and end time 1 is invalid!";
		}
		if (TimeSpan.Parse(Function.ToString(txt_StartTime2.Text)) > TimeSpan.Parse(Function.ToString(txt_EndTime2.Text)))
		{
			txt_StartTime2.Focus();
			return "start time 2 and end time 2 is invalid!";
		}
		if (TimeSpan.Parse(Function.ToString(txt_BreakBegins1.Text)) > TimeSpan.Parse(Function.ToString(txt_BreakEnd1.Text)))
		{
			txt_BreakBegins1.Focus();
			return "Start Break time 1 and end break time 1 is invalid!";
		}
		if (TimeSpan.Parse(Function.ToString(txt_BreakBegin2.Text)) > TimeSpan.Parse(Function.ToString(txt_BreakEnd2.Text)))
		{
			txt_BreakBegin2.Focus();
			return "Start Break time 2 and end break time 2 is invalid!";
		}
		if (check_Time2.Checked && TimeSpan.Parse(Function.ToString(txt_StartTime2.Text)) < TimeSpan.Parse(Function.ToString(txt_EndTime1.Text)))
		{
			txt_StartTime2.Focus();
			return "Time shift 1 and time shift 2 is invalid!";
		}
		if (TimeSpan.Parse(Function.ToString(txt_BreakBegin2.Text)) != TimeSpan.Parse(Function.ToString(txt_BreakEnd2.Text)) && TimeSpan.Parse(Function.ToString(txt_BreakBegin2.Text)) < TimeSpan.Parse(Function.ToString(txt_EndTime2.Text)))
		{
			txt_BreakBegin2.Focus();
			return "Time shift 2 and time break 2 is invalid!";
		}
		if (TimeSpan.Parse(Function.ToString(txt_BreakBegins1.Text)) != TimeSpan.Parse(Function.ToString(txt_BreakEnd1.Text)) && TimeSpan.Parse(Function.ToString(txt_EndTime1.Text)) > TimeSpan.Parse(Function.ToString(txt_BreakBegins1.Text)))
		{
			txt_EndTime1.Focus();
			return "Time shift 1 and time break 1 is invalid!";
		}
		return "";
	}

	private async Task<DataTable> GetDataBrach()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			return await http.GetAsync("GetDataBrach", new
			{
				userCode = GlobalData.UserCodeLogin
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private async Task SetDataBranch()
	{
		try
		{
			txt_Branch.EditValue = GlobalData.DefaultBranch;
			if (!GlobalData.IsMultiBranch)
			{
				txt_Branch.Enabled = false;
				return;
			}
			DataTable dataSource = await GetDataBrach();
			txt_Branch.Properties.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task FrmDefinedShift_Load(object sender, EventArgs e)
	{
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			await SetDataBranch();
			await LoadData();
			if (!string.IsNullOrEmpty(findCode))
			{
				gv_Shift.ExpandAllGroups();
				for (int i = 0; i < gv_Shift.RowCount; i++)
				{
					if (findCode.Equals(Function.ToString(gv_Shift.GetRowCellValue(i, gc_Code))))
					{
						gv_Shift.FocusedRowHandle = i;
						break;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
	}

	private void btn_Refresh_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void check_Time2_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (check_Time2.Checked)
			{
				txt_StartTime2.Enabled = true;
				txt_EndTime2.Enabled = true;
				txt_BreakBegin2.Enabled = true;
				txt_BreakEnd2.Enabled = true;
			}
			else
			{
				txt_StartTime2.Enabled = false;
				txt_EndTime2.Enabled = false;
				txt_BreakBegin2.Enabled = false;
				txt_BreakEnd2.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Shift_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			AddNew();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.Add;
		WinCommons._CloseProcessing();
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					Close();
				}
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.No)
			{
				string text = CheckDataSave();
				if (string.IsNullOrEmpty(text))
				{
					await Save();
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
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Code_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			gv_Shift.SetFocusedRowCellValue(gc_Code.FieldName, e.NewValue);
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Name_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(txt_Code.Text) && FormMode != Enums.FormViewModeType.Busy)
			{
				e.Cancel = true;
			}
			gv_Shift.SetFocusedRowCellValue(gc_Name.FieldName, e.NewValue);
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Time_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Remarks_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(txt_Code.Text) && FormMode != Enums.FormViewModeType.Busy)
			{
				e.Cancel = true;
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void check_Time2_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(txt_Code.Text) && FormMode != Enums.FormViewModeType.Busy)
			{
				e.Cancel = true;
			}
			if (Function.ToString(e.NewValue) == "N")
			{
				txt_StartTime2.EditValue = TimeSpan.Parse("00:00");
				txt_EndTime2.EditValue = TimeSpan.Parse("00:00");
				txt_BreakEnd2.EditValue = TimeSpan.Parse("00:00");
				txt_BreakBegin2.EditValue = TimeSpan.Parse("00:00");
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (grd_Shift.DataSourceTable != null && grd_Shift.DataSourceTable.Rows.Count > 0)
			{
				await Delete();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Shift_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		Enums.FormViewModeType formMode = FormMode;
		WinCommons._OpenProcessing();
		try
		{
			SetDataControl(gv_Shift.GetFocusedDataRow());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = formMode;
		WinCommons._CloseProcessing();
	}

	private void gv_Shift_ColumnFilterChanged(object sender, EventArgs e)
	{
		try
		{
			gv_Shift.FocusedRowHandle = -2147483646;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Conbtrol_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			GetTimeTotal();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Control_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(txt_Code.Text) && FormMode != Enums.FormViewModeType.Busy)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void txt_Branch_EditValueChanged(object sender, EventArgs e)
	{
		Enums.FormViewModeType view = FormMode;
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (view != Enums.FormViewModeType.Add)
			{
				await LoadData();
				view = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = view;
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.MTS_Manufacturing.FrmDefinedShift));
		this.txt_BreakEnd2 = new DevExpress.XtraEditors.TimeEdit();
		this.txt_BreakBegin2 = new DevExpress.XtraEditors.TimeEdit();
		this.txt_EndTime2 = new DevExpress.XtraEditors.TimeEdit();
		this.txt_StartTime2 = new DevExpress.XtraEditors.TimeEdit();
		this.check_Time2 = new Apzon.Controls.Editor.CheckEditEx();
		this.txt_BreakEnd1 = new DevExpress.XtraEditors.TimeEdit();
		this.txt_BreakBegins1 = new DevExpress.XtraEditors.TimeEdit();
		this.txt_EndTime1 = new DevExpress.XtraEditors.TimeEdit();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Refresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_Remarks = new Apzon.Controls.Editor.MemoEditEx();
		this.txt_Time = new Apzon.Controls.Editor.CalcEditEx();
		this.txt_Name = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Code = new Apzon.Controls.Editor.TextEditEx();
		this.gc_Code = new Apzon.Controls.Grid.GridColumnEx();
		this.gv_Shift = new Apzon.Controls.Grid.GridViewEx();
		this.gc_Name = new Apzon.Controls.Grid.GridColumnEx();
		this.grd_Shift = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.lbBranch = new Apzon.Controls.Controls.LabelLine();
		this.txt_Branch = new DevExpress.XtraEditors.LookUpEdit();
		this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
		this.labelLine13 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine12 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine11 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine10 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.txt_StartTime1 = new DevExpress.XtraEditors.TimeEdit();
		((System.ComponentModel.ISupportInitialize)this.txt_BreakEnd2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BreakBegin2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EndTime2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_StartTime2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.check_Time2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BreakEnd1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BreakBegins1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EndTime1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Remarks.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Time.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Shift).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Shift).BeginInit();
		this.contextMenuStrip.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_Branch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).BeginInit();
		this.groupControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_StartTime1.Properties).BeginInit();
		base.SuspendLayout();
		this.txt_BreakEnd2.EditValue = "00:00";
		this.txt_BreakEnd2.Location = new System.Drawing.Point(112, 335);
		this.txt_BreakEnd2.Name = "txt_BreakEnd2";
		this.txt_BreakEnd2.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_BreakEnd2.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_BreakEnd2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txt_BreakEnd2.Properties.DisplayFormat.FormatString = "HH:mm";
		this.txt_BreakEnd2.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.txt_BreakEnd2.Properties.EditFormat.FormatString = "HH:mm";
		this.txt_BreakEnd2.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.txt_BreakEnd2.Properties.Mask.AutoComplete = DevExpress.XtraEditors.Mask.AutoCompleteType.None;
		this.txt_BreakEnd2.Properties.Mask.EditMask = "HH:mm";
		this.txt_BreakEnd2.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
		this.txt_BreakEnd2.Properties.Mask.UseMaskAsDisplayFormat = true;
		this.txt_BreakEnd2.Properties.NullText = "00:00";
		this.txt_BreakEnd2.Size = new System.Drawing.Size(70, 20);
		this.txt_BreakEnd2.TabIndex = 13;
		this.txt_BreakEnd2.EditValueChanged += new System.EventHandler(this.txt_Conbtrol_EditValueChanged);
		this.txt_BreakEnd2.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_Control_EditValueChanging);
		this.txt_BreakBegin2.EditValue = "00:00";
		this.txt_BreakBegin2.Location = new System.Drawing.Point(112, 312);
		this.txt_BreakBegin2.Name = "txt_BreakBegin2";
		this.txt_BreakBegin2.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_BreakBegin2.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_BreakBegin2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txt_BreakBegin2.Properties.DisplayFormat.FormatString = "HH:mm";
		this.txt_BreakBegin2.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.txt_BreakBegin2.Properties.EditFormat.FormatString = "HH:mm";
		this.txt_BreakBegin2.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.txt_BreakBegin2.Properties.Mask.AutoComplete = DevExpress.XtraEditors.Mask.AutoCompleteType.None;
		this.txt_BreakBegin2.Properties.Mask.EditMask = "HH:mm";
		this.txt_BreakBegin2.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
		this.txt_BreakBegin2.Properties.Mask.UseMaskAsDisplayFormat = true;
		this.txt_BreakBegin2.Properties.NullText = "00:00";
		this.txt_BreakBegin2.Size = new System.Drawing.Size(70, 20);
		this.txt_BreakBegin2.TabIndex = 12;
		this.txt_BreakBegin2.EditValueChanged += new System.EventHandler(this.txt_Conbtrol_EditValueChanged);
		this.txt_BreakBegin2.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_Control_EditValueChanging);
		this.txt_EndTime2.EditValue = "00:00";
		this.txt_EndTime2.Location = new System.Drawing.Point(112, 289);
		this.txt_EndTime2.Name = "txt_EndTime2";
		this.txt_EndTime2.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_EndTime2.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_EndTime2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txt_EndTime2.Properties.DisplayFormat.FormatString = "HH:mm";
		this.txt_EndTime2.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.txt_EndTime2.Properties.EditFormat.FormatString = "HH:mm";
		this.txt_EndTime2.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.txt_EndTime2.Properties.Mask.AutoComplete = DevExpress.XtraEditors.Mask.AutoCompleteType.None;
		this.txt_EndTime2.Properties.Mask.EditMask = "HH:mm";
		this.txt_EndTime2.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
		this.txt_EndTime2.Properties.Mask.UseMaskAsDisplayFormat = true;
		this.txt_EndTime2.Properties.NullText = "00:00";
		this.txt_EndTime2.Size = new System.Drawing.Size(70, 20);
		this.txt_EndTime2.TabIndex = 11;
		this.txt_EndTime2.EditValueChanged += new System.EventHandler(this.txt_Conbtrol_EditValueChanged);
		this.txt_EndTime2.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_Control_EditValueChanging);
		this.txt_StartTime2.EditValue = "00:00";
		this.txt_StartTime2.Location = new System.Drawing.Point(112, 266);
		this.txt_StartTime2.Name = "txt_StartTime2";
		this.txt_StartTime2.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_StartTime2.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_StartTime2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txt_StartTime2.Properties.DisplayFormat.FormatString = "HH:mm";
		this.txt_StartTime2.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.txt_StartTime2.Properties.EditFormat.FormatString = "HH:mm";
		this.txt_StartTime2.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.txt_StartTime2.Properties.Mask.AutoComplete = DevExpress.XtraEditors.Mask.AutoCompleteType.None;
		this.txt_StartTime2.Properties.Mask.EditMask = "HH:mm";
		this.txt_StartTime2.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
		this.txt_StartTime2.Properties.Mask.UseMaskAsDisplayFormat = true;
		this.txt_StartTime2.Properties.NullText = "00:00";
		this.txt_StartTime2.Size = new System.Drawing.Size(70, 20);
		this.txt_StartTime2.TabIndex = 10;
		this.txt_StartTime2.EditValueChanged += new System.EventHandler(this.txt_Conbtrol_EditValueChanged);
		this.txt_StartTime2.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_Control_EditValueChanging);
		this.check_Time2.EditControl = null;
		this.check_Time2.FieldName = null;
		this.check_Time2.IsSearchControl = false;
		this.check_Time2.Location = new System.Drawing.Point(111, 241);
		this.check_Time2.Name = "check_Time2";
		this.check_Time2.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.check_Time2.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.check_Time2.Properties.Caption = "";
		this.check_Time2.Properties.ValueChecked = "Y";
		this.check_Time2.Properties.ValueUnchecked = "N";
		this.check_Time2.SettingInVisible = false;
		this.check_Time2.SettingReadonly = false;
		this.check_Time2.Size = new System.Drawing.Size(23, 20);
		this.check_Time2.TabIndex = 9;
		this.check_Time2.CheckedChanged += new System.EventHandler(this.check_Time2_CheckedChanged);
		this.check_Time2.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.check_Time2_EditValueChanging);
		this.txt_BreakEnd1.EditValue = "00:00";
		this.txt_BreakEnd1.Location = new System.Drawing.Point(112, 215);
		this.txt_BreakEnd1.Name = "txt_BreakEnd1";
		this.txt_BreakEnd1.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_BreakEnd1.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_BreakEnd1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txt_BreakEnd1.Properties.DisplayFormat.FormatString = "HH:mm";
		this.txt_BreakEnd1.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.txt_BreakEnd1.Properties.EditFormat.FormatString = "HH:mm";
		this.txt_BreakEnd1.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.txt_BreakEnd1.Properties.Mask.AutoComplete = DevExpress.XtraEditors.Mask.AutoCompleteType.None;
		this.txt_BreakEnd1.Properties.Mask.EditMask = "HH:mm";
		this.txt_BreakEnd1.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
		this.txt_BreakEnd1.Properties.Mask.UseMaskAsDisplayFormat = true;
		this.txt_BreakEnd1.Properties.NullText = "00:00";
		this.txt_BreakEnd1.Size = new System.Drawing.Size(70, 20);
		this.txt_BreakEnd1.TabIndex = 8;
		this.txt_BreakEnd1.EditValueChanged += new System.EventHandler(this.txt_Conbtrol_EditValueChanged);
		this.txt_BreakEnd1.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_Control_EditValueChanging);
		this.txt_BreakBegins1.EditValue = "00:00";
		this.txt_BreakBegins1.Location = new System.Drawing.Point(112, 192);
		this.txt_BreakBegins1.Name = "txt_BreakBegins1";
		this.txt_BreakBegins1.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_BreakBegins1.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_BreakBegins1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txt_BreakBegins1.Properties.DisplayFormat.FormatString = "HH:mm";
		this.txt_BreakBegins1.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.txt_BreakBegins1.Properties.EditFormat.FormatString = "HH:mm";
		this.txt_BreakBegins1.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.txt_BreakBegins1.Properties.Mask.AutoComplete = DevExpress.XtraEditors.Mask.AutoCompleteType.None;
		this.txt_BreakBegins1.Properties.Mask.EditMask = "HH:mm";
		this.txt_BreakBegins1.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
		this.txt_BreakBegins1.Properties.Mask.UseMaskAsDisplayFormat = true;
		this.txt_BreakBegins1.Properties.NullText = "00:00";
		this.txt_BreakBegins1.Size = new System.Drawing.Size(70, 20);
		this.txt_BreakBegins1.TabIndex = 7;
		this.txt_BreakBegins1.EditValueChanged += new System.EventHandler(this.txt_Conbtrol_EditValueChanged);
		this.txt_BreakBegins1.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_Control_EditValueChanging);
		this.txt_EndTime1.EditValue = "00:00";
		this.txt_EndTime1.Location = new System.Drawing.Point(112, 169);
		this.txt_EndTime1.Name = "txt_EndTime1";
		this.txt_EndTime1.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_EndTime1.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_EndTime1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txt_EndTime1.Properties.DisplayFormat.FormatString = "HH:mm";
		this.txt_EndTime1.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.txt_EndTime1.Properties.EditFormat.FormatString = "HH:mm";
		this.txt_EndTime1.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.txt_EndTime1.Properties.Mask.AutoComplete = DevExpress.XtraEditors.Mask.AutoCompleteType.None;
		this.txt_EndTime1.Properties.Mask.EditMask = "HH:mm";
		this.txt_EndTime1.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
		this.txt_EndTime1.Properties.Mask.UseMaskAsDisplayFormat = true;
		this.txt_EndTime1.Properties.NullText = "00:00";
		this.txt_EndTime1.Size = new System.Drawing.Size(70, 20);
		this.txt_EndTime1.TabIndex = 6;
		this.txt_EndTime1.EditValueChanged += new System.EventHandler(this.txt_Conbtrol_EditValueChanged);
		this.txt_EndTime1.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_Control_EditValueChanging);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(159, 369);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(70, 20);
		this.btnCancel.TabIndex = 17;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btn_Refresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Refresh.EditControl = null;
		this.btn_Refresh.EditValue = "Refresh";
		this.btn_Refresh.FieldName = null;
		this.btn_Refresh.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Refresh.ImageOptions.Image");
		this.btn_Refresh.IsSearchControl = false;
		this.btn_Refresh.Location = new System.Drawing.Point(83, 369);
		this.btn_Refresh.Name = "btn_Refresh";
		this.btn_Refresh.ReadOnly = false;
		this.btn_Refresh.SettingInVisible = false;
		this.btn_Refresh.SettingReadonly = false;
		this.btn_Refresh.Size = new System.Drawing.Size(70, 20);
		this.btn_Refresh.TabIndex = 16;
		this.btn_Refresh.Text = "Refresh";
		this.btn_Refresh.Click += new System.EventHandler(this.btn_Refresh_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "Save";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(7, 369);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(70, 20);
		this.btn_Save.TabIndex = 14;
		this.btn_Save.Text = "Save";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.txt_Remarks.EditControl = null;
		this.txt_Remarks.FieldName = null;
		this.txt_Remarks.IsSearchControl = false;
		this.txt_Remarks.Location = new System.Drawing.Point(112, 93);
		this.txt_Remarks.Name = "txt_Remarks";
		this.txt_Remarks.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_Remarks.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_Remarks.SettingInVisible = false;
		this.txt_Remarks.SettingReadonly = false;
		this.txt_Remarks.Size = new System.Drawing.Size(120, 50);
		this.txt_Remarks.TabIndex = 4;
		this.txt_Remarks.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_Remarks_EditValueChanging);
		this.txt_Time.EditControl = null;
		this.txt_Time.FieldName = null;
		this.txt_Time.IsSearchControl = false;
		this.txt_Time.Location = new System.Drawing.Point(112, 70);
		this.txt_Time.Name = "txt_Time";
		this.txt_Time.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_Time.Properties.DisplayFormat.FormatString = "n2";
		this.txt_Time.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_Time.Properties.EditFormat.FormatString = "n2";
		this.txt_Time.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_Time.Properties.IsSearchControl = false;
		this.txt_Time.Properties.Mask.EditMask = "n2";
		this.txt_Time.Properties.ReadOnly = true;
		this.txt_Time.SettingInVisible = false;
		this.txt_Time.SettingReadonly = false;
		this.txt_Time.Size = new System.Drawing.Size(120, 20);
		this.txt_Time.TabIndex = 3;
		this.txt_Time.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_Time_EditValueChanging);
		this.txt_Name.EditControl = null;
		this.txt_Name.FieldName = null;
		this.txt_Name.IsSearchControl = false;
		this.txt_Name.Location = new System.Drawing.Point(112, 47);
		this.txt_Name.Name = "txt_Name";
		this.txt_Name.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_Name.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_Name.SettingInVisible = false;
		this.txt_Name.SettingReadonly = false;
		this.txt_Name.Size = new System.Drawing.Size(120, 20);
		this.txt_Name.TabIndex = 2;
		this.txt_Name.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_Name_EditValueChanging);
		this.txt_Code.EditControl = null;
		this.txt_Code.FieldName = null;
		this.txt_Code.IsSearchControl = false;
		this.txt_Code.Location = new System.Drawing.Point(112, 24);
		this.txt_Code.Name = "txt_Code";
		this.txt_Code.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_Code.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_Code.Properties.MaxLength = 2;
		this.txt_Code.Properties.ReadOnly = true;
		this.txt_Code.SettingInVisible = false;
		this.txt_Code.SettingReadonly = false;
		this.txt_Code.Size = new System.Drawing.Size(120, 20);
		this.txt_Code.TabIndex = 1;
		this.txt_Code.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_Code_EditValueChanging);
		this.gc_Code.Caption = "Code";
		this.gc_Code.CurrencyDateField = null;
		this.gc_Code.CurrencyField = null;
		this.gc_Code.CurrencyFixed = null;
		this.gc_Code.CurrencyFixedRate = 0.0;
		this.gc_Code.CurrencyRateField = null;
		this.gc_Code.FieldName = "Code";
		this.gc_Code.Name = "gc_Code";
		this.gc_Code.OptionsColumn.AllowEdit = false;
		this.gc_Code.PrimaryKey = false;
		this.gc_Code.UnitField = null;
		this.gc_Code.Visible = true;
		this.gc_Code.VisibleIndex = 0;
		this.gv_Shift.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.gv_Shift.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Shift.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Shift.ColumnKey = null;
		this.gv_Shift.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gc_Code, this.gc_Name });
		this.gv_Shift.DisableFormSetting = false;
		this.gv_Shift.DisplayException = false;
		this.gv_Shift.EditColumn = null;
		this.gv_Shift.FieldCurrency = null;
		this.gv_Shift.FieldRate = null;
		this.gv_Shift.FormId = "oShift";
		this.gv_Shift.GridControl = this.grd_Shift;
		this.gv_Shift.IgnoreFilter = false;
		this.gv_Shift.IndexEditRow = -1;
		this.gv_Shift.IndicatorWidth = 30;
		this.gv_Shift.IsApzTable = false;
		this.gv_Shift.IsInsertNewRow = false;
		this.gv_Shift.IsLoadingData = false;
		this.gv_Shift.IsMappingContextAction = false;
		this.gv_Shift.ItemId = "APZ_OSHP";
		this.gv_Shift.LstColumnAllowBatch = null;
		this.gv_Shift.LstColumnNotAllowBatch = null;
		this.gv_Shift.LstColumnNotEditable = null;
		this.gv_Shift.LstColumNotSetting = null;
		this.gv_Shift.Name = "gv_Shift";
		this.gv_Shift.OptionsView.ShowGroupPanel = false;
		this.gv_Shift.UseBatchMod = false;
		this.gv_Shift.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Shift_CustomDrawRowIndicator);
		this.gv_Shift.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Shift_FocusedRowObjectChanged);
		this.gv_Shift.ColumnFilterChanged += new System.EventHandler(this.gv_Shift_ColumnFilterChanged);
		this.gc_Name.Caption = "Name";
		this.gc_Name.CurrencyDateField = null;
		this.gc_Name.CurrencyField = null;
		this.gc_Name.CurrencyFixed = null;
		this.gc_Name.CurrencyFixedRate = 0.0;
		this.gc_Name.CurrencyRateField = null;
		this.gc_Name.FieldName = "Name";
		this.gc_Name.Name = "gc_Name";
		this.gc_Name.OptionsColumn.AllowEdit = false;
		this.gc_Name.PrimaryKey = false;
		this.gc_Name.UnitField = null;
		this.gc_Name.Visible = true;
		this.gc_Name.VisibleIndex = 1;
		this.grd_Shift.AllowPasteExcel = true;
		this.grd_Shift.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.grd_Shift.ClipboardData = null;
		this.grd_Shift.ContextMenuStrip = this.contextMenuStrip;
		this.grd_Shift.Location = new System.Drawing.Point(5, 50);
		this.grd_Shift.MainView = this.gv_Shift;
		this.grd_Shift.Name = "grd_Shift";
		this.grd_Shift.Size = new System.Drawing.Size(250, 349);
		this.grd_Shift.TabIndex = 0;
		this.grd_Shift.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Shift });
		this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addToolStripMenuItem, this.deleteToolStripMenuItem });
		this.contextMenuStrip.Name = "contextMenuStrip";
		this.contextMenuStrip.Size = new System.Drawing.Size(108, 48);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
		this.addToolStripMenuItem.Text = "Add";
		this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.groupControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.groupControl1.Controls.Add(this.lbBranch);
		this.groupControl1.Controls.Add(this.grd_Shift);
		this.groupControl1.Controls.Add(this.txt_Branch);
		this.groupControl1.Location = new System.Drawing.Point(0, 0);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(257, 399);
		this.groupControl1.TabIndex = 3;
		this.groupControl1.Text = "List of Shift";
		this.lbBranch.EditControl = null;
		this.lbBranch.EditValue = "Branch";
		this.lbBranch.FieldName = null;
		this.lbBranch.HideWhenEmpty = false;
		this.lbBranch.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbBranch.Location = new System.Drawing.Point(25, 27);
		this.lbBranch.Name = "lbBranch";
		this.lbBranch.ReadOnly = false;
		this.lbBranch.SettingInVisible = false;
		this.lbBranch.SettingReadonly = false;
		this.lbBranch.ShowUnderLine = true;
		this.lbBranch.Size = new System.Drawing.Size(60, 20);
		this.lbBranch.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbBranch.TabIndex = 1;
		this.lbBranch.Utext = "Branch";
		this.txt_Branch.Location = new System.Drawing.Point(85, 24);
		this.txt_Branch.Name = "txt_Branch";
		this.txt_Branch.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_Branch.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_Branch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_Branch.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.txt_Branch.Properties.DisplayMember = "Name";
		this.txt_Branch.Properties.NullText = "";
		this.txt_Branch.Properties.ShowFooter = false;
		this.txt_Branch.Properties.ShowHeader = false;
		this.txt_Branch.Properties.ValueMember = "Value";
		this.txt_Branch.Size = new System.Drawing.Size(138, 20);
		this.txt_Branch.TabIndex = 2;
		this.txt_Branch.EditValueChanged += new System.EventHandler(this.txt_Branch_EditValueChanged);
		this.groupControl2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.groupControl2.Controls.Add(this.labelLine13);
		this.groupControl2.Controls.Add(this.labelLine12);
		this.groupControl2.Controls.Add(this.labelLine11);
		this.groupControl2.Controls.Add(this.labelLine10);
		this.groupControl2.Controls.Add(this.labelLine9);
		this.groupControl2.Controls.Add(this.labelLine8);
		this.groupControl2.Controls.Add(this.labelLine7);
		this.groupControl2.Controls.Add(this.labelLine6);
		this.groupControl2.Controls.Add(this.labelLine5);
		this.groupControl2.Controls.Add(this.labelLine4);
		this.groupControl2.Controls.Add(this.labelLine3);
		this.groupControl2.Controls.Add(this.labelLine2);
		this.groupControl2.Controls.Add(this.labelLine1);
		this.groupControl2.Controls.Add(this.txt_BreakEnd2);
		this.groupControl2.Controls.Add(this.txt_BreakBegin2);
		this.groupControl2.Controls.Add(this.txt_EndTime2);
		this.groupControl2.Controls.Add(this.txt_StartTime2);
		this.groupControl2.Controls.Add(this.check_Time2);
		this.groupControl2.Controls.Add(this.txt_BreakEnd1);
		this.groupControl2.Controls.Add(this.txt_BreakBegins1);
		this.groupControl2.Controls.Add(this.txt_EndTime1);
		this.groupControl2.Controls.Add(this.btnCancel);
		this.groupControl2.Controls.Add(this.btn_Refresh);
		this.groupControl2.Controls.Add(this.btn_Save);
		this.groupControl2.Controls.Add(this.txt_Remarks);
		this.groupControl2.Controls.Add(this.txt_Time);
		this.groupControl2.Controls.Add(this.txt_Name);
		this.groupControl2.Controls.Add(this.txt_Code);
		this.groupControl2.Controls.Add(this.txt_StartTime1);
		this.groupControl2.Location = new System.Drawing.Point(263, 0);
		this.groupControl2.Name = "groupControl2";
		this.groupControl2.Size = new System.Drawing.Size(252, 399);
		this.groupControl2.TabIndex = 2;
		this.groupControl2.Text = "Details";
		this.labelLine13.EditControl = null;
		this.labelLine13.EditValue = "Break End 2";
		this.labelLine13.FieldName = null;
		this.labelLine13.HideWhenEmpty = false;
		this.labelLine13.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine13.Location = new System.Drawing.Point(5, 338);
		this.labelLine13.Name = "labelLine13";
		this.labelLine13.ReadOnly = false;
		this.labelLine13.SettingInVisible = false;
		this.labelLine13.SettingReadonly = false;
		this.labelLine13.ShowUnderLine = true;
		this.labelLine13.Size = new System.Drawing.Size(107, 20);
		this.labelLine13.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine13.TabIndex = 0;
		this.labelLine13.Utext = "Break End 2";
		this.labelLine12.EditControl = null;
		this.labelLine12.EditValue = "Break Begins 2";
		this.labelLine12.FieldName = null;
		this.labelLine12.HideWhenEmpty = false;
		this.labelLine12.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine12.Location = new System.Drawing.Point(5, 315);
		this.labelLine12.Name = "labelLine12";
		this.labelLine12.ReadOnly = false;
		this.labelLine12.SettingInVisible = false;
		this.labelLine12.SettingReadonly = false;
		this.labelLine12.ShowUnderLine = true;
		this.labelLine12.Size = new System.Drawing.Size(107, 20);
		this.labelLine12.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine12.TabIndex = 0;
		this.labelLine12.Utext = "Break Begins 2";
		this.labelLine11.EditControl = null;
		this.labelLine11.EditValue = "End Time 2";
		this.labelLine11.FieldName = null;
		this.labelLine11.HideWhenEmpty = false;
		this.labelLine11.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine11.Location = new System.Drawing.Point(5, 292);
		this.labelLine11.Name = "labelLine11";
		this.labelLine11.ReadOnly = false;
		this.labelLine11.SettingInVisible = false;
		this.labelLine11.SettingReadonly = false;
		this.labelLine11.ShowUnderLine = true;
		this.labelLine11.Size = new System.Drawing.Size(107, 20);
		this.labelLine11.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine11.TabIndex = 0;
		this.labelLine11.Utext = "End Time 2";
		this.labelLine10.EditControl = null;
		this.labelLine10.EditValue = "Start Time 2";
		this.labelLine10.FieldName = null;
		this.labelLine10.HideWhenEmpty = false;
		this.labelLine10.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine10.Location = new System.Drawing.Point(5, 269);
		this.labelLine10.Name = "labelLine10";
		this.labelLine10.ReadOnly = false;
		this.labelLine10.SettingInVisible = false;
		this.labelLine10.SettingReadonly = false;
		this.labelLine10.ShowUnderLine = true;
		this.labelLine10.Size = new System.Drawing.Size(107, 20);
		this.labelLine10.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine10.TabIndex = 0;
		this.labelLine10.Utext = "Start Time 2";
		this.labelLine9.EditControl = null;
		this.labelLine9.EditValue = "Time 2";
		this.labelLine9.FieldName = null;
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(5, 240);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ReadOnly = false;
		this.labelLine9.SettingInVisible = false;
		this.labelLine9.SettingReadonly = false;
		this.labelLine9.ShowUnderLine = true;
		this.labelLine9.Size = new System.Drawing.Size(107, 20);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 25;
		this.labelLine9.Utext = "Time 2";
		this.labelLine8.EditControl = null;
		this.labelLine8.EditValue = "Break End 1";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(5, 216);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ReadOnly = false;
		this.labelLine8.SettingInVisible = false;
		this.labelLine8.SettingReadonly = false;
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(107, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 0;
		this.labelLine8.Utext = "Break End 1";
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "Break Begins 1";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(5, 194);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(107, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 0;
		this.labelLine7.Utext = "Break Begins 1";
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "End Time 1";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(5, 172);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(107, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 0;
		this.labelLine6.Utext = "End Time 1";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Start Time 1";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(5, 149);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(107, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 0;
		this.labelLine5.Utext = "Start Time 1";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Remark";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(5, 125);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(107, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 0;
		this.labelLine4.Utext = "Remark";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Time";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(5, 73);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(107, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 0;
		this.labelLine3.Utext = "Time";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Name";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(5, 50);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(107, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 0;
		this.labelLine2.Utext = "Name";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(5, 27);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(107, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Code";
		this.txt_StartTime1.EditValue = "00:00";
		this.txt_StartTime1.Location = new System.Drawing.Point(112, 146);
		this.txt_StartTime1.Name = "txt_StartTime1";
		this.txt_StartTime1.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txt_StartTime1.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_StartTime1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_StartTime1.Properties.DisplayFormat.FormatString = "HH:mm";
		this.txt_StartTime1.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.txt_StartTime1.Properties.EditFormat.FormatString = "HH:mm";
		this.txt_StartTime1.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.txt_StartTime1.Properties.Mask.AutoComplete = DevExpress.XtraEditors.Mask.AutoCompleteType.None;
		this.txt_StartTime1.Properties.Mask.EditMask = "HH:mm";
		this.txt_StartTime1.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.DateTimeAdvancingCaret;
		this.txt_StartTime1.Properties.Mask.UseMaskAsDisplayFormat = true;
		this.txt_StartTime1.Properties.NullText = "00:00";
		this.txt_StartTime1.Size = new System.Drawing.Size(70, 20);
		this.txt_StartTime1.TabIndex = 5;
		this.txt_StartTime1.EditValueChanged += new System.EventHandler(this.txt_Conbtrol_EditValueChanged);
		this.txt_StartTime1.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_Control_EditValueChanging);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(518, 403);
		base.Controls.Add(this.groupControl1);
		base.Controls.Add(this.groupControl2);
		base.FormId = "oShift";
		base.IconOptions.ShowIcon = false;
		base.Name = "FrmDefinedShift";
		base.ShowInTaskbar = false;
		this.Text = "Shift";
		((System.ComponentModel.ISupportInitialize)this.txt_BreakEnd2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BreakBegin2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EndTime2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_StartTime2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.check_Time2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BreakEnd1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BreakBegins1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EndTime1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Remarks.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Time.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Shift).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Shift).EndInit();
		this.contextMenuStrip.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_Branch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).EndInit();
		this.groupControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_StartTime1.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
