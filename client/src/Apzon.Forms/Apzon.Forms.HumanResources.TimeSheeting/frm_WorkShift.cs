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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Export;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTab;

namespace Apzon.Forms.HumanResources.TimeSheeting;

public class frm_WorkShift : XtraFormEx
{
	public readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isLoading;

	public Func<Task> Reload;

	private DataTable _dtWorkCode;

	private DataTable _dtOverTimeCode;

	private IContainer components;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private SplitContainerControl splitContainerControl1;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx4;

	private XtraTabControlEx xtraTabControlEx1;

	private XtraTabPageEx xtraTabPageEx1;

	private XtraTabPageEx xtraTabPageEx2;

	private TextEditEx txt_Dscription;

	private LabelLine labelLine2;

	private TextEditEx txt_Code;

	private LabelLine labelLine1;

	private LabelLine labelLine3;

	private CheckEditEx chk_NightShift;

	private GroupBox groupBox1;

	private GroupBox groupBox2;

	private LabelLine labelLine4;

	private HourEdit hour_StartTime;

	private HourEdit hour_EndTime;

	private LabelLine labelLine5;

	private HourEdit hour_EndTimeOut;

	private LabelLine labelLine10;

	private HourEdit hour_StartTimeOut;

	private LabelLine labelLine11;

	private HourEdit hour_EndTimeIn;

	private LabelLine labelLine8;

	private HourEdit hour_StartNT;

	private LabelLine labelLine9;

	private HourEdit hour_EndBreak;

	private LabelLine labelLine6;

	private HourEdit hour_StartBreak;

	private LabelLine labelLine7;

	private GridColumnEx gridColumnEx6;

	private LabelLine labelLine15;

	private LabelLine labelLine16;

	private CheckEditEx chk_RequireIn;

	private CheckEditEx chk_RequireOut;

	private LabelLine labelLine17;

	private LabelLine labelLine18;

	private CalcEditEx calc_EarlyGrace;

	private CalcEditEx calc_LateGrace;

	private LookUpEditEx luk_AbsentCode;

	private LookUpEditEx luk_WorkCode;

	private CheckEditEx chk_CalcLate;

	private HourEdit hour_EndNT;

	private LabelLine labelLine19;

	private HourEdit hour_StartTimeIn;

	private LabelLine labelLine21;

	private CalcEditEx calc_MinBeforeDuty;

	private CheckEditEx chk_CalcBeforeOT;

	private LabelControlEx labelControlEx2;

	private CalcEditEx calc_MinAfterDuty;

	private CheckEditEx chk_CalcAfterOT;

	private LookUpEditEx luk_HLOTWCode;

	private LookUpEditEx luk_WKOTWCode;

	private LookUpEditEx luk_WDOTWCode;

	private LabelControlEx labelControlEx1;

	private CheckEditEx chk_IsOTShift;

	private LabelLine labelLine22;

	private LabelLine labelLine23;

	private LabelControlEx labelControlEx3;

	private CalcEditEx calc_MaxAfterOT;

	private LabelControlEx labelControlEx4;

	private CalcEditEx calc_MaxBeforeOT;

	private CheckEditEx chk_CalcAbsent;

	private CheckEditEx chk_CalcHLOT;

	private CheckEditEx chk_CalcWKOT;

	private LabelLine labelLine12;

	private CalcEditEx calc_WorkingTotal;

	private LookUpEditEx luk_OTWCode;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_Ok;

	private ContextMenuStrip ctm_Doc;

	private ToolStripMenuItem addToolStripMenuItem;

	private ToolStripMenuItem removeToolStripMenuItem;

	private CheckEditEx chk_CalcEarly;

	private LabelLine labelLine13;

	private LookUpEditEx luk_Branch;

	private LabelLine labelLine14;

	private CalcEditEx calc_DayCount;

	private RepositoryItemLookUpEdit rep_Branch;

	private LabelControlEx labelControlEx6;

	private LabelControlEx labelControlEx5;

	private LabelControlEx labelControlEx7;

	private CheckEditEx chk_CodeByWTotal;

	private LabelControlEx labelControlEx8;

	private LabelControlEx labelControlEx9;

	private CalcEditEx calc_NoOutWT;

	private CalcEditEx calc_NoInWT;

	private LabelLine labelLine20;

	private LabelLine labelLine24;

	private XtraTabPageEx xtraTabPageEx3;

	private GridControlEx grd_CombineWorkCode;

	private GridViewEx gv_CombineWorkCode;

	private GridColumnEx gc_LineNum;

	private GridColumnEx gc_FromMinutes;

	private GridColumnEx gc_ToMinutes;

	private GridColumnEx gc_WorkCode;

	private GridColumnEx gc_AbsentCode;

	private RepositoryItemCalcEdit rep_FromMin;

	private RepositoryItemCalcEdit rep_ToMin;

	private RepositoryItemLookUpEdit rep_WorkCode;

	private RepositoryItemLookUpEdit rep_AbsentCode;

	private ContextMenuStrip ctm_MinutesConvert;

	private ToolStripMenuItem ctmMcAddMenuTrip;

	private ToolStripMenuItem ctmMcRemoveMenuTrip;

	private GridColumnEx gc_Remarks;

	private CheckEditEx chk_CalHoliday;

	private LookUpEditEx luk_HldCode;

	private LabelLine labelLine25;

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
		case Enums.FormViewModeType.Add:
			btn_Ok.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_Ok.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Ok.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_WorkShift()
	{
		InitializeComponent();
		OnLoadAsync += frm_WorkShift_Load;
	}

	private async Task frm_WorkShift_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			await LoadInitForm();
			await LoadData();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private async Task LoadData()
	{
		try
		{
			_isLoading = true;
			DataTable dataSource = await _httpClient.WorkShiftClient.GetAll();
			grd_Doc.DataSource = dataSource;
			grd_Doc.DataSourceTable.AcceptChanges();
			grd_Doc.RefreshDataSource();
			FormMode = Enums.FormViewModeType.View;
			_isLoading = false;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private async Task LoadInitForm()
	{
		_ = 2;
		try
		{
			_dtWorkCode = await _httpClient.TimeSheetingClient.LoadWorkCode(2);
			_dtOverTimeCode = await _httpClient.TimeSheetingClient.GetOvertimeType();
			luk_HLOTWCode.SetDataSource(_dtOverTimeCode, "Name", "Code", emptyRow: true, emptyTop: true, hideOtherColumns: true);
			luk_OTWCode.Properties.Assign(luk_HLOTWCode.Properties);
			luk_WDOTWCode.Properties.Assign(luk_HLOTWCode.Properties);
			luk_WKOTWCode.Properties.Assign(luk_HLOTWCode.Properties);
			luk_WorkCode.SetDataSource(_dtWorkCode, "Name", "Code", emptyRow: true, emptyTop: true, hideOtherColumns: true);
			luk_AbsentCode.Properties.Assign(luk_WorkCode.Properties);
			luk_HldCode.Properties.Assign(luk_WorkCode.Properties);
			rep_WorkCode.DisplayMember = "Name";
			rep_WorkCode.ValueMember = "Code";
			rep_WorkCode.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_WorkCode.Assign(luk_WorkCode.Properties);
			rep_AbsentCode.DisplayMember = "Name";
			rep_AbsentCode.ValueMember = "Code";
			rep_AbsentCode.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_AbsentCode.Assign(luk_WorkCode.Properties);
			DataTable dt = await _httpClient.EmployeeMasterClient.LoadComboBranch();
			luk_Branch.SetDataSource(dt, "Name", "Code", emptyRow: true, emptyTop: true, hideOtherColumns: true);
			rep_Branch.Assign(luk_Branch.Properties);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private async void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			btn_Ok.Enabled = false;
			await SaveData();
			btn_Ok.Enabled = true;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private async Task SaveData()
	{
		_ = 2;
		try
		{
			DataTable dataTable = DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				string value = Function.ToString(hour_StartBreak.EditValue);
				string value2 = Function.ToString(hour_EndBreak.EditValue);
				string value3 = Function.ToString(hour_StartTime.EditValue);
				string value4 = Function.ToString(hour_EndTime.EditValue);
				string value5 = Function.ToString(hour_StartTimeIn.EditValue);
				string value6 = Function.ToString(hour_EndTimeIn.EditValue);
				string value7 = Function.ToString(hour_StartTimeOut.EditValue);
				string value8 = Function.ToString(hour_EndTimeOut.EditValue);
				if ((string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(value2)) || (!string.IsNullOrWhiteSpace(value) && string.IsNullOrWhiteSpace(value2)))
				{
					WinCommons.Error(Enums.MessageCode.StartAndEndBreakTimeMustBeAllEmptyOrAllHaveValue, "");
					return;
				}
				if (string.IsNullOrWhiteSpace(value3) || string.IsNullOrWhiteSpace(value4))
				{
					WinCommons.Error(Enums.MessageCode.StartAndEndWorkingTimeCannotBeEmpty, "");
					return;
				}
				if (string.IsNullOrWhiteSpace(value5) || string.IsNullOrWhiteSpace(value6))
				{
					WinCommons.Error(Enums.MessageCode.StartAndEndInTimeCannotBeEmpty, "");
					return;
				}
				if (string.IsNullOrWhiteSpace(value7) || string.IsNullOrWhiteSpace(value8))
				{
					WinCommons.Error(Enums.MessageCode.StartAndEndOutTimeCannotbeEmpty, "");
					return;
				}
				if (string.IsNullOrEmpty(luk_Branch.Text))
				{
					WinCommons.Error(Enums.MessageCode.BranchCannotBeEmpty, "");
					return;
				}
				DataTable dataTable2 = grd_CombineWorkCode.DataSourceTable.Copy();
				if (dataTable2.IsNotNull())
				{
					string text = CheckDataLine();
					if (!string.IsNullOrWhiteSpace(text))
					{
						WinCommons.Error(Enums.MessageCode.None, text);
						return;
					}
					for (int i = 0; i < dataTable2.Rows.Count; i++)
					{
						dataTable2.Rows[i]["UserSign"] = GlobalData.UserID;
						if (dataTable2.Rows[i]["CreateDate"] == DBNull.Value)
						{
							dataTable2.Rows[i]["CreateDate"] = DateTime.Now;
							continue;
						}
						dataTable2.Rows[i]["UserSign2"] = GlobalData.UserID;
						dataTable2.Rows[i]["UpdateDate"] = DateTime.Now;
					}
				}
				DataSet dataSet = new DataSet();
				dataSet.Tables.Add(dataTable.Copy());
				dataSet.Tables.Add(dataTable2);
				WinCommons._OpenProcessing();
				string text2 = await _httpClient.WorkShiftClient.Save(dataSet);
				WinCommons._CloseProcessing();
				if (string.IsNullOrWhiteSpace(text2))
				{
					FormMode = Enums.FormViewModeType.View;
					await LoadData();
					if (Reload != null && !((XtraForm)Reload.Target).IsDisposed)
					{
						await Reload();
					}
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text2);
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private string CheckDataLine()
	{
		try
		{
			DataTable dataTable = grd_CombineWorkCode.DataSourceTable();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				int num = Function.ParseInt(dataTable.Rows[i]["FromMinutes"]);
				int num2 = Function.ParseInt(dataTable.Rows[i]["ToMinutes"]);
				int num3 = Function.ParseInt(calc_WorkingTotal.Value);
				if (num < 0 || num > num3 || num2 < 0 || num2 > num3)
				{
					return "Value cannot out of Working Total's range.";
				}
				if (num > num2)
				{
					return "From Minutes cannot be greater than To Minutes.";
				}
				for (int j = i + 1; j < dataTable.Rows.Count; j++)
				{
					if (num < Function.ParseInt(dataTable.Rows[j]["ToMinutes"]) && Function.ParseInt(dataTable.Rows[j]["FromMinutes"]) < num2)
					{
						return "Data cannot overlap.";
					}
				}
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ex.Message;
		}
	}

	private void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewDoc();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			btn_Ok.Enabled = false;
			try
			{
				int focusedRowHandle = gv_Doc.FocusedRowHandle;
				int num = Function.ParseInt(gv_Doc.GetRowCellValue(focusedRowHandle, "ShiftID"));
				if (num > 0)
				{
					string text = await _httpClient.WorkShiftClient.Remove(num);
					if (string.IsNullOrWhiteSpace(text))
					{
						WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
						gv_Doc.BeginUpdate();
						gv_Doc.DeleteSelectedRows();
						gv_Doc.EndUpdate();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, Resources.Msg_DeleteError + ": " + text);
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			}
			btn_Ok.Enabled = true;
		}
		catch (Exception ex2)
		{
			WinCommons.ErrorLineNumber(ex2);
			Logging.Write("ERROR", ex2);
		}
	}

	public async Task REFRESH()
	{
		AddNewDoc();
	}

	public async Task REFRESH_DATA()
	{
		await LoadData();
	}

	private void AddNewDoc()
	{
		_isLoading = true;
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			btn_Ok.Enabled = true;
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OEWS");
			DataRow dataRow = tableStructure.NewRow();
			dataRow["DayCount"] = 1;
			tableStructure.Rows.Add(dataRow);
			grd_Doc.DataSource = tableStructure;
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoading = false;
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			_isLoading = true;
			int focusId = Function.ParseInt(gv_Doc.GetRowCellValue(e.FocusedRowHandle, "ShiftID"));
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			if (dataSourceTable.IsNotNull())
			{
				DataRow dataRow = dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ShiftID"]) == focusId);
				if (dataRow != null)
				{
					DataSource = dataRow.ToArray().CopyToDataTable();
				}
			}
			else
			{
				DataSource = WinFuntions.GetTableStructure("APZ_OEWS");
			}
			DataTable dataTable = await _httpClient.WorkShiftClient.GetLine(Function.ParseInt(focusId));
			if (dataTable.IsNotNull())
			{
				grd_CombineWorkCode.DataSource = dataTable;
			}
			else
			{
				grd_CombineWorkCode.DataSource = WinFuntions.GetTableStructure("APZ_EWS1");
			}
			grd_CombineWorkCode.DataSourceTable.AcceptChanges();
			_isLoading = false;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private void frm_WorkShift_OnControlValueChanged(object sender, ControlChangedEventArgs e)
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private void gv_Doc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (_isLoading || (FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update))
			{
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				e.Allow = false;
				return;
			}
			if (grd_Doc.DataSource is DataTable dataTable)
			{
				dataTable.RejectChanges();
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chk_CodeByWTotal_CheckedChanged(object sender, EventArgs e)
	{
		if (chk_CodeByWTotal.Checked)
		{
			luk_WorkCode.Enabled = false;
		}
		else
		{
			luk_WorkCode.Enabled = true;
		}
	}

	private void hour_StartTime_EditValueChanged(object sender, EventArgs e)
	{
		if (!_isLoading)
		{
			CalculateWorkingTotal();
		}
	}

	private void hour_EndTime_EditValueChanged(object sender, EventArgs e)
	{
		if (!_isLoading)
		{
			CalculateWorkingTotal();
		}
	}

	private void CalculateWorkingTotal()
	{
		_isLoading = true;
		int num = Function.ParseInt(calc_DayCount.EditValue);
		num = ((num <= 0) ? 1 : num);
		double totalMinutes = hour_EndTime.TimeSpanValue.Add(TimeSpan.FromDays(num - 1)).Subtract(hour_StartTime.TimeSpanValue).TotalMinutes;
		double num2 = 0.0;
		if (!string.IsNullOrWhiteSpace(Function.ToString(hour_EndBreak.EditValue)) && !string.IsNullOrWhiteSpace(Function.ToString(hour_EndBreak.EditValue)))
		{
			num2 = hour_EndBreak.TimeSpanValue.Subtract(hour_StartBreak.TimeSpanValue).TotalMinutes;
		}
		calc_WorkingTotal.EditValue = ((totalMinutes - num2 > 0.0) ? (totalMinutes - num2) : 0.0);
		_isLoading = false;
	}

	private void hour_StartBreak_EditValueChanged(object sender, EventArgs e)
	{
		if (!_isLoading)
		{
			CalculateWorkingTotal();
		}
	}

	private void hour_EndBreak_EditValueChanged(object sender, EventArgs e)
	{
		if (!_isLoading)
		{
			CalculateWorkingTotal();
		}
	}

	private string ValidateTime()
	{
		string result = "";
		try
		{
			int num = Function.ParseInt(calc_DayCount.EditValue);
			TimeSpan timeSpanValue = hour_StartBreak.TimeSpanValue;
			TimeSpan timeSpanValue2 = hour_EndBreak.TimeSpanValue;
			TimeSpan timeSpanValue3 = hour_StartTime.TimeSpanValue;
			TimeSpan timeSpanValue4 = hour_EndTime.TimeSpanValue;
			string value = Function.ToString(hour_StartTime.EditValue);
			string value2 = Function.ToString(hour_EndTime.EditValue);
			string value3 = Function.ToString(hour_StartBreak.EditValue);
			string value4 = Function.ToString(hour_EndBreak.EditValue);
			TimeSpan timeSpanValue5 = hour_StartTimeIn.TimeSpanValue;
			TimeSpan timeSpanValue6 = hour_EndTimeIn.TimeSpanValue;
			TimeSpan timeSpanValue7 = hour_StartTimeOut.TimeSpanValue;
			TimeSpan timeSpanValue8 = hour_EndTimeOut.TimeSpanValue;
			string value5 = Function.ToString(hour_StartTimeIn.EditValue);
			string value6 = Function.ToString(hour_EndTimeIn.EditValue);
			string value7 = Function.ToString(hour_StartTimeOut.EditValue);
			string value8 = Function.ToString(hour_EndTimeOut.EditValue);
			if (num == 1 && timeSpanValue4 < timeSpanValue3 && string.IsNullOrWhiteSpace(value) && string.IsNullOrWhiteSpace(value2))
			{
				result = "End Working Time can not be before Start Working Time (Day count = 1)!";
				return result;
			}
			if (num == 1 && timeSpanValue6 < timeSpanValue5 && string.IsNullOrWhiteSpace(value5) && string.IsNullOrWhiteSpace(value6))
			{
				result = "End In Time can not be before Start In Time (Day count = 1)!";
				return result;
			}
			if (num == 1 && timeSpanValue8 < timeSpanValue7 && string.IsNullOrWhiteSpace(value7) && string.IsNullOrWhiteSpace(value8))
			{
				result = "End Out Time can not be before Start Out Time (Day count = 1)!";
				return result;
			}
			if (timeSpanValue2 < timeSpanValue && !string.IsNullOrWhiteSpace(value3) && !string.IsNullOrWhiteSpace(value4))
			{
				result = "End Break Time can not be before Start Break Time!";
				return result;
			}
			if (num == 1 && (timeSpanValue < timeSpanValue3 || timeSpanValue > timeSpanValue4 || timeSpanValue2 < timeSpanValue3 || timeSpanValue2 > timeSpanValue4) && !string.IsNullOrWhiteSpace(value3) && !string.IsNullOrWhiteSpace(value4))
			{
				result = "Break Time must between Start and End WorkingTime!";
				return result;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return result;
	}

	private void frm_WorkShift_OnControlValueChanging(object sender, ControlChangingEventArgs e)
	{
		try
		{
			if (_isLoading)
			{
				return;
			}
			if (e.FormControl is CalcEditEx { Name: "calc_DayCount" } && Function.ParseInt(e.NewValue) == 0)
			{
				e.Cancel = true;
			}
			if (e.FieldName == "StartTime" || e.FieldName == "EndTime" || e.FieldName == "StartBreak" || e.FieldName == "EndBreak" || e.FieldName == "StartTimeIn" || e.FieldName == "EndTimeIn" || e.FieldName == "StartTimeOut" || e.FieldName == "EndTimeOut")
			{
				string text = ValidateTime();
				e.Cancel = !string.IsNullOrWhiteSpace(text);
				if (e.FormControl is HourEdit hourEdit)
				{
					hourEdit.ErrorText = text;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_WorkShift_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			await SaveData();
		}
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void ctmMcAddMenuTrip_Click(object sender, EventArgs e)
	{
		try
		{
			DataRow dataRow = grd_CombineWorkCode.DataSourceTable.NewRow();
			dataRow["ShiftID"] = gv_Doc.GetFocusedRowCellValue("ShiftID");
			grd_CombineWorkCode.DataSourceTable.Rows.Add(dataRow);
			grd_CombineWorkCode.RefreshDataSource();
			DataChange();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private void ctmMcRemoveMenuTrip_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_CombineWorkCode.DataSourceTable;
			int[] selectedRows = gv_CombineWorkCode.GetSelectedRows();
			for (int num = selectedRows.Length - 1; num >= 0; num--)
			{
				int dataSourceRowIndex = gv_CombineWorkCode.GetDataSourceRowIndex(selectedRows[num]);
				dataSourceTable.Rows.RemoveAt(dataSourceRowIndex);
			}
			grd_CombineWorkCode.RefreshDataSource();
			DataChange();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private void DataChange()
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void gv_CombineWorkCode_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		DataChange();
	}

	private void luk_Branch_EditValueChanged(object sender, EventArgs e)
	{
		int selectedBranch = Function.ParseInt(luk_Branch.EditValue);
		EnumerableRowCollection<DataRow> source = from t in _dtWorkCode.AsEnumerable()
			where Function.ParseInt(t["branch"]) == selectedBranch
			select t;
		DataTable dataSource = _dtWorkCode.Clone();
		if (source.Any())
		{
			dataSource = source.CopyToDataTable();
		}
		luk_WorkCode.Properties.DataSource = dataSource;
		luk_AbsentCode.Properties.Assign(luk_WorkCode.Properties);
		rep_WorkCode.Assign(luk_WorkCode.Properties);
		rep_AbsentCode.Assign(luk_WorkCode.Properties);
		DataTable dataSource2 = _dtOverTimeCode.Clone();
		EnumerableRowCollection<DataRow> source2 = from t in _dtOverTimeCode.AsEnumerable()
			where Function.ParseInt(t["Branch"]) == selectedBranch
			select t;
		if (source2.Any())
		{
			dataSource2 = source2.CopyToDataTable();
		}
		luk_HLOTWCode.Properties.DataSource = dataSource2;
		luk_OTWCode.Properties.Assign(luk_HLOTWCode.Properties);
		luk_WDOTWCode.Properties.Assign(luk_HLOTWCode.Properties);
		luk_WKOTWCode.Properties.Assign(luk_HLOTWCode.Properties);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.TimeSheeting.frm_WorkShift));
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Branch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.hour_EndNT = new Apzon.Controls.Controls.HourEdit();
		this.luk_Branch = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine19 = new Apzon.Controls.Controls.LabelLine();
		this.chk_NightShift = new Apzon.Controls.Editor.CheckEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Dscription = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Code = new Apzon.Controls.Editor.TextEditEx();
		this.hour_StartNT = new Apzon.Controls.Controls.HourEdit();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.xtraTabControlEx1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPageEx1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.groupBox2 = new System.Windows.Forms.GroupBox();
		this.chk_CalHoliday = new Apzon.Controls.Editor.CheckEditEx();
		this.luk_HldCode = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine25 = new Apzon.Controls.Controls.LabelLine();
		this.labelControlEx8 = new Apzon.Controls.Controls.LabelControlEx();
		this.labelControlEx9 = new Apzon.Controls.Controls.LabelControlEx();
		this.calc_NoOutWT = new Apzon.Controls.Editor.CalcEditEx();
		this.calc_NoInWT = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine20 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine24 = new Apzon.Controls.Controls.LabelLine();
		this.chk_CodeByWTotal = new Apzon.Controls.Editor.CheckEditEx();
		this.labelControlEx6 = new Apzon.Controls.Controls.LabelControlEx();
		this.labelControlEx5 = new Apzon.Controls.Controls.LabelControlEx();
		this.chk_CalcAbsent = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_CalcLate = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_CalcEarly = new Apzon.Controls.Editor.CheckEditEx();
		this.luk_AbsentCode = new Apzon.Controls.Editor.LookUpEditEx();
		this.luk_WorkCode = new Apzon.Controls.Editor.LookUpEditEx();
		this.calc_EarlyGrace = new Apzon.Controls.Editor.CalcEditEx();
		this.calc_LateGrace = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine17 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine18 = new Apzon.Controls.Controls.LabelLine();
		this.chk_RequireIn = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_RequireOut = new Apzon.Controls.Editor.CheckEditEx();
		this.labelLine16 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine15 = new Apzon.Controls.Controls.LabelLine();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.labelControlEx7 = new Apzon.Controls.Controls.LabelControlEx();
		this.labelLine14 = new Apzon.Controls.Controls.LabelLine();
		this.calc_DayCount = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine12 = new Apzon.Controls.Controls.LabelLine();
		this.calc_WorkingTotal = new Apzon.Controls.Editor.CalcEditEx();
		this.hour_StartTimeIn = new Apzon.Controls.Controls.HourEdit();
		this.labelLine21 = new Apzon.Controls.Controls.LabelLine();
		this.hour_EndTimeOut = new Apzon.Controls.Controls.HourEdit();
		this.labelLine10 = new Apzon.Controls.Controls.LabelLine();
		this.hour_StartTimeOut = new Apzon.Controls.Controls.HourEdit();
		this.labelLine11 = new Apzon.Controls.Controls.LabelLine();
		this.hour_EndTimeIn = new Apzon.Controls.Controls.HourEdit();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.hour_EndBreak = new Apzon.Controls.Controls.HourEdit();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.hour_StartBreak = new Apzon.Controls.Controls.HourEdit();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.hour_EndTime = new Apzon.Controls.Controls.HourEdit();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.hour_StartTime = new Apzon.Controls.Controls.HourEdit();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.xtraTabPageEx2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.labelLine13 = new Apzon.Controls.Controls.LabelLine();
		this.luk_OTWCode = new Apzon.Controls.Editor.LookUpEditEx();
		this.chk_CalcHLOT = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_CalcWKOT = new Apzon.Controls.Editor.CheckEditEx();
		this.labelLine22 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine23 = new Apzon.Controls.Controls.LabelLine();
		this.labelControlEx3 = new Apzon.Controls.Controls.LabelControlEx();
		this.calc_MaxAfterOT = new Apzon.Controls.Editor.CalcEditEx();
		this.labelControlEx4 = new Apzon.Controls.Controls.LabelControlEx();
		this.calc_MaxBeforeOT = new Apzon.Controls.Editor.CalcEditEx();
		this.chk_IsOTShift = new Apzon.Controls.Editor.CheckEditEx();
		this.labelControlEx2 = new Apzon.Controls.Controls.LabelControlEx();
		this.calc_MinAfterDuty = new Apzon.Controls.Editor.CalcEditEx();
		this.chk_CalcAfterOT = new Apzon.Controls.Editor.CheckEditEx();
		this.luk_HLOTWCode = new Apzon.Controls.Editor.LookUpEditEx();
		this.luk_WKOTWCode = new Apzon.Controls.Editor.LookUpEditEx();
		this.luk_WDOTWCode = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelControlEx1 = new Apzon.Controls.Controls.LabelControlEx();
		this.calc_MinBeforeDuty = new Apzon.Controls.Editor.CalcEditEx();
		this.chk_CalcBeforeOT = new Apzon.Controls.Editor.CheckEditEx();
		this.xtraTabPageEx3 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_CombineWorkCode = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_MinutesConvert = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.ctmMcAddMenuTrip = new System.Windows.Forms.ToolStripMenuItem();
		this.ctmMcRemoveMenuTrip = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_CombineWorkCode = new Apzon.Controls.Grid.GridViewEx();
		this.gc_LineNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_FromMinutes = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_FromMin = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gc_ToMinutes = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ToMin = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gc_WorkCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_WorkCode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_AbsentCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_AbsentCode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Remarks = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctm_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.hour_EndNT.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_NightShift.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Dscription.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hour_StartNT.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).BeginInit();
		this.xtraTabControlEx1.SuspendLayout();
		this.xtraTabPageEx1.SuspendLayout();
		this.groupBox2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_CalHoliday.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_HldCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_NoOutWT.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_NoInWT.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CodeByWTotal.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CalcAbsent.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CalcLate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CalcEarly.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_AbsentCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_WorkCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_EarlyGrace.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_LateGrace.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_RequireIn.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_RequireOut.Properties).BeginInit();
		this.groupBox1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.calc_DayCount.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_WorkingTotal.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hour_StartTimeIn.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hour_EndTimeOut.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hour_StartTimeOut.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hour_EndTimeIn.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hour_EndBreak.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hour_StartBreak.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hour_EndTime.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hour_StartTime.Properties).BeginInit();
		this.xtraTabPageEx2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_OTWCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CalcHLOT.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CalcWKOT.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_MaxAfterOT.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_MaxBeforeOT.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_IsOTShift.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_MinAfterDuty.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CalcAfterOT.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_HLOTWCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_WKOTWCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_WDOTWCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_MinBeforeDuty.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CalcBeforeOT.Properties).BeginInit();
		this.xtraTabPageEx3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_CombineWorkCode).BeginInit();
		this.ctm_MinutesConvert.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_CombineWorkCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_FromMin).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ToMin).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WorkCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_AbsentCode).BeginInit();
		base.SuspendLayout();
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctm_Doc;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_Branch });
		this.grd_Doc.Size = new System.Drawing.Size(339, 440);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctm_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addToolStripMenuItem, this.removeToolStripMenuItem });
		this.ctm_Doc.Name = "ctm_Doc";
		this.ctm_Doc.Size = new System.Drawing.Size(118, 48);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.addToolStripMenuItem.Text = "Add";
		this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx5, this.gridColumnEx4, this.gridColumnEx3, this.gridColumnEx6 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oEmployeeWorkShift";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.HasFormSetting = false;
		this.gv_Doc.HeaderExcelExport = null;
		this.gv_Doc.HeaderExcelType = Apzon.Commons.Enums.Enums.HeaderExcelType.Disabled;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "ODOC";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.LstOldUDF = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Doc.OptionsBehavior.Editable = false;
		this.gv_Doc.OptionsPrint.AutoWidth = false;
		this.gv_Doc.OptionsPrint.UsePrintStyles = false;
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupedColumns = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.ReadOnly = false;
		this.gv_Doc.SettingReadonly = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.gridColumnEx1.Caption = "ID";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "ShiftID";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx2.Caption = "Code";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "ShiftCode";
		this.gridColumnEx2.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 0;
		this.gridColumnEx5.Caption = "Start";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "StartTime";
		this.gridColumnEx5.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 2;
		this.gridColumnEx4.Caption = "End";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "EndTime";
		this.gridColumnEx4.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 3;
		this.gridColumnEx3.Caption = "Description";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "Dscription";
		this.gridColumnEx3.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 1;
		this.gridColumnEx6.Caption = "Branch";
		this.gridColumnEx6.ColumnEdit = this.rep_Branch;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "branch";
		this.gridColumnEx6.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 4;
		this.rep_Branch.AutoHeight = false;
		this.rep_Branch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Branch.Name = "rep_Branch";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.hour_EndNT);
		this.splitContainerControl1.Panel2.Controls.Add(this.luk_Branch);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine19);
		this.splitContainerControl1.Panel2.Controls.Add(this.chk_NightShift);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine3);
		this.splitContainerControl1.Panel2.Controls.Add(this.txt_Dscription);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine2);
		this.splitContainerControl1.Panel2.Controls.Add(this.txt_Code);
		this.splitContainerControl1.Panel2.Controls.Add(this.hour_StartNT);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine1);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine9);
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControlEx1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1035, 440);
		this.splitContainerControl1.SplitterPosition = 339;
		this.splitContainerControl1.TabIndex = 1;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.hour_EndNT.EditControl = null;
		this.hour_EndNT.FieldName = "EndNT";
		this.hour_EndNT.IsSearchControl = false;
		this.hour_EndNT.Location = new System.Drawing.Point(588, 28);
		this.hour_EndNT.Name = "hour_EndNT";
		this.hour_EndNT.Properties.IsSearchControl = false;
		this.hour_EndNT.Properties.MaxLength = 5;
		this.hour_EndNT.SettingInVisible = false;
		this.hour_EndNT.SettingReadonly = false;
		this.hour_EndNT.Size = new System.Drawing.Size(59, 20);
		this.hour_EndNT.TabIndex = 6;
		this.luk_Branch.EditControl = null;
		this.luk_Branch.FieldName = "branch";
		this.luk_Branch.IsSearchControl = false;
		this.luk_Branch.Location = new System.Drawing.Point(65, 28);
		this.luk_Branch.Name = "luk_Branch";
		this.luk_Branch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Branch.Properties.NullText = "";
		this.luk_Branch.SettingInVisible = false;
		this.luk_Branch.SettingReadonly = false;
		this.luk_Branch.Size = new System.Drawing.Size(101, 20);
		this.luk_Branch.TabIndex = 2;
		this.luk_Branch.EditValueChanged += new System.EventHandler(this.luk_Branch_EditValueChanged);
		this.labelLine19.EditControl = null;
		this.labelLine19.EditValue = "To";
		this.labelLine19.FieldName = null;
		this.labelLine19.HideWhenEmpty = false;
		this.labelLine19.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine19.Location = new System.Drawing.Point(534, 31);
		this.labelLine19.Name = "labelLine19";
		this.labelLine19.ReadOnly = false;
		this.labelLine19.SettingInVisible = false;
		this.labelLine19.SettingReadonly = false;
		this.labelLine19.ShowUnderLine = true;
		this.labelLine19.Size = new System.Drawing.Size(55, 20);
		this.labelLine19.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine19.TabIndex = 16;
		this.labelLine19.Utext = "To";
		this.chk_NightShift.EditControl = null;
		this.chk_NightShift.FieldName = "IsNightShift";
		this.chk_NightShift.IsSearchControl = false;
		this.chk_NightShift.Location = new System.Drawing.Point(272, 29);
		this.chk_NightShift.Name = "chk_NightShift";
		this.chk_NightShift.Properties.Caption = "Overnight Shift";
		this.chk_NightShift.Properties.ValueChecked = "Y";
		this.chk_NightShift.Properties.ValueUnchecked = "N";
		this.chk_NightShift.SettingInVisible = false;
		this.chk_NightShift.SettingReadonly = false;
		this.chk_NightShift.Size = new System.Drawing.Size(96, 20);
		this.chk_NightShift.TabIndex = 4;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Branch";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(2, 31);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(66, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 8;
		this.labelLine3.Utext = "Branch";
		this.txt_Dscription.EditControl = null;
		this.txt_Dscription.FieldName = "Dscription";
		this.txt_Dscription.IsSearchControl = false;
		this.txt_Dscription.Location = new System.Drawing.Point(272, 2);
		this.txt_Dscription.Name = "txt_Dscription";
		this.txt_Dscription.SettingInVisible = false;
		this.txt_Dscription.SettingReadonly = false;
		this.txt_Dscription.Size = new System.Drawing.Size(375, 20);
		this.txt_Dscription.TabIndex = 3;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Description";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(172, 5);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(102, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 6;
		this.labelLine2.Utext = "Description";
		this.txt_Code.EditControl = null;
		this.txt_Code.FieldName = "ShiftCode";
		this.txt_Code.IsSearchControl = false;
		this.txt_Code.Location = new System.Drawing.Point(65, 3);
		this.txt_Code.Name = "txt_Code";
		this.txt_Code.SettingInVisible = false;
		this.txt_Code.SettingReadonly = false;
		this.txt_Code.Size = new System.Drawing.Size(101, 20);
		this.txt_Code.TabIndex = 1;
		this.hour_StartNT.EditControl = null;
		this.hour_StartNT.FieldName = "StartNT";
		this.hour_StartNT.IsSearchControl = false;
		this.hour_StartNT.Location = new System.Drawing.Point(423, 28);
		this.hour_StartNT.Name = "hour_StartNT";
		this.hour_StartNT.Properties.IsSearchControl = false;
		this.hour_StartNT.Properties.MaxLength = 5;
		this.hour_StartNT.SettingInVisible = false;
		this.hour_StartNT.SettingReadonly = false;
		this.hour_StartNT.Size = new System.Drawing.Size(59, 20);
		this.hour_StartNT.TabIndex = 5;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(2, 6);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(66, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 4;
		this.labelLine1.Utext = "Code";
		this.labelLine9.EditControl = null;
		this.labelLine9.EditValue = "From";
		this.labelLine9.FieldName = null;
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(372, 31);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ReadOnly = false;
		this.labelLine9.SettingInVisible = false;
		this.labelLine9.SettingReadonly = false;
		this.labelLine9.ShowUnderLine = true;
		this.labelLine9.Size = new System.Drawing.Size(55, 20);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 13;
		this.labelLine9.Utext = "From";
		this.xtraTabControlEx1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabControlEx1.IsSearchControl = false;
		this.xtraTabControlEx1.Location = new System.Drawing.Point(1, 55);
		this.xtraTabControlEx1.Name = "xtraTabControlEx1";
		this.xtraTabControlEx1.SelectedTabPage = this.xtraTabPageEx1;
		this.xtraTabControlEx1.Size = new System.Drawing.Size(690, 385);
		this.xtraTabControlEx1.TabIndex = 7;
		this.xtraTabControlEx1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[3] { this.xtraTabPageEx1, this.xtraTabPageEx2, this.xtraTabPageEx3 });
		this.xtraTabPageEx1.Controls.Add(this.groupBox2);
		this.xtraTabPageEx1.Controls.Add(this.groupBox1);
		this.xtraTabPageEx1.EditControl = null;
		this.xtraTabPageEx1.EditValue = "Main Information";
		this.xtraTabPageEx1.FieldName = "";
		this.xtraTabPageEx1.IsSearchControl = false;
		this.xtraTabPageEx1.Name = "xtraTabPageEx1";
		this.xtraTabPageEx1.ReadOnly = false;
		this.xtraTabPageEx1.SettingInVisible = false;
		this.xtraTabPageEx1.SettingReadonly = false;
		this.xtraTabPageEx1.Size = new System.Drawing.Size(688, 360);
		this.xtraTabPageEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageEx1.Text = "Main Information";
		this.groupBox2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.groupBox2.Controls.Add(this.chk_CalHoliday);
		this.groupBox2.Controls.Add(this.luk_HldCode);
		this.groupBox2.Controls.Add(this.labelLine25);
		this.groupBox2.Controls.Add(this.labelControlEx8);
		this.groupBox2.Controls.Add(this.labelControlEx9);
		this.groupBox2.Controls.Add(this.calc_NoOutWT);
		this.groupBox2.Controls.Add(this.calc_NoInWT);
		this.groupBox2.Controls.Add(this.labelLine20);
		this.groupBox2.Controls.Add(this.labelLine24);
		this.groupBox2.Controls.Add(this.chk_CodeByWTotal);
		this.groupBox2.Controls.Add(this.labelControlEx6);
		this.groupBox2.Controls.Add(this.labelControlEx5);
		this.groupBox2.Controls.Add(this.chk_CalcAbsent);
		this.groupBox2.Controls.Add(this.chk_CalcLate);
		this.groupBox2.Controls.Add(this.chk_CalcEarly);
		this.groupBox2.Controls.Add(this.luk_AbsentCode);
		this.groupBox2.Controls.Add(this.luk_WorkCode);
		this.groupBox2.Controls.Add(this.calc_EarlyGrace);
		this.groupBox2.Controls.Add(this.calc_LateGrace);
		this.groupBox2.Controls.Add(this.labelLine17);
		this.groupBox2.Controls.Add(this.labelLine18);
		this.groupBox2.Controls.Add(this.chk_RequireIn);
		this.groupBox2.Controls.Add(this.chk_RequireOut);
		this.groupBox2.Controls.Add(this.labelLine16);
		this.groupBox2.Controls.Add(this.labelLine15);
		this.groupBox2.Location = new System.Drawing.Point(3, 147);
		this.groupBox2.Name = "groupBox2";
		this.groupBox2.Size = new System.Drawing.Size(684, 211);
		this.groupBox2.TabIndex = 1;
		this.groupBox2.TabStop = false;
		this.groupBox2.Text = "Calculation Parameters";
		this.chk_CalHoliday.EditControl = null;
		this.chk_CalHoliday.FieldName = "CalcHld";
		this.chk_CalHoliday.IsSearchControl = false;
		this.chk_CalHoliday.Location = new System.Drawing.Point(267, 72);
		this.chk_CalHoliday.Name = "chk_CalHoliday";
		this.chk_CalHoliday.Properties.Caption = "Calc Holiday";
		this.chk_CalHoliday.Properties.ValueChecked = "Y";
		this.chk_CalHoliday.Properties.ValueUnchecked = "N";
		this.chk_CalHoliday.SettingInVisible = false;
		this.chk_CalHoliday.SettingReadonly = false;
		this.chk_CalHoliday.Size = new System.Drawing.Size(158, 20);
		this.chk_CalHoliday.TabIndex = 41;
		this.luk_HldCode.EditControl = null;
		this.luk_HldCode.FieldName = "HldCode";
		this.luk_HldCode.IsSearchControl = false;
		this.luk_HldCode.Location = new System.Drawing.Point(163, 72);
		this.luk_HldCode.Name = "luk_HldCode";
		this.luk_HldCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_HldCode.Properties.NullText = "";
		this.luk_HldCode.SettingInVisible = false;
		this.luk_HldCode.SettingReadonly = false;
		this.luk_HldCode.Size = new System.Drawing.Size(65, 20);
		this.luk_HldCode.TabIndex = 40;
		this.labelLine25.EditControl = null;
		this.labelLine25.EditValue = "Holiday Code";
		this.labelLine25.FieldName = null;
		this.labelLine25.HideWhenEmpty = false;
		this.labelLine25.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine25.Location = new System.Drawing.Point(6, 75);
		this.labelLine25.Name = "labelLine25";
		this.labelLine25.ReadOnly = false;
		this.labelLine25.SettingInVisible = false;
		this.labelLine25.SettingReadonly = false;
		this.labelLine25.ShowUnderLine = true;
		this.labelLine25.Size = new System.Drawing.Size(160, 20);
		this.labelLine25.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine25.TabIndex = 39;
		this.labelLine25.Utext = "Holiday Code";
		this.labelControlEx8.EditControl = null;
		this.labelControlEx8.EditValue = "(mins)";
		this.labelControlEx8.FieldName = null;
		this.labelControlEx8.Location = new System.Drawing.Point(231, 179);
		this.labelControlEx8.Name = "labelControlEx8";
		this.labelControlEx8.ReadOnly = false;
		this.labelControlEx8.SettingInVisible = false;
		this.labelControlEx8.SettingReadonly = false;
		this.labelControlEx8.Size = new System.Drawing.Size(29, 13);
		this.labelControlEx8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx8.TabIndex = 38;
		this.labelControlEx8.Text = "(mins)";
		this.labelControlEx9.EditControl = null;
		this.labelControlEx9.EditValue = "(mins)";
		this.labelControlEx9.FieldName = null;
		this.labelControlEx9.Location = new System.Drawing.Point(231, 153);
		this.labelControlEx9.Name = "labelControlEx9";
		this.labelControlEx9.ReadOnly = false;
		this.labelControlEx9.SettingInVisible = false;
		this.labelControlEx9.SettingReadonly = false;
		this.labelControlEx9.Size = new System.Drawing.Size(29, 13);
		this.labelControlEx9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx9.TabIndex = 37;
		this.labelControlEx9.Text = "(mins)";
		this.calc_NoOutWT.EditControl = null;
		this.calc_NoOutWT.FieldName = "NoOutWT";
		this.calc_NoOutWT.IsSearchControl = false;
		this.calc_NoOutWT.Location = new System.Drawing.Point(163, 176);
		this.calc_NoOutWT.Name = "calc_NoOutWT";
		this.calc_NoOutWT.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_NoOutWT.Properties.IsSearchControl = false;
		this.calc_NoOutWT.SettingInVisible = false;
		this.calc_NoOutWT.SettingReadonly = false;
		this.calc_NoOutWT.Size = new System.Drawing.Size(65, 20);
		this.calc_NoOutWT.TabIndex = 22;
		this.calc_NoInWT.EditControl = null;
		this.calc_NoInWT.FieldName = "NoInWT";
		this.calc_NoInWT.IsSearchControl = false;
		this.calc_NoInWT.Location = new System.Drawing.Point(163, 150);
		this.calc_NoInWT.Name = "calc_NoInWT";
		this.calc_NoInWT.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_NoInWT.Properties.IsSearchControl = false;
		this.calc_NoInWT.SettingInVisible = false;
		this.calc_NoInWT.SettingReadonly = false;
		this.calc_NoInWT.Size = new System.Drawing.Size(65, 20);
		this.calc_NoInWT.TabIndex = 21;
		this.labelLine20.EditControl = null;
		this.labelLine20.EditValue = "Total Mins if No Out";
		this.labelLine20.FieldName = null;
		this.labelLine20.HideWhenEmpty = false;
		this.labelLine20.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine20.Location = new System.Drawing.Point(6, 179);
		this.labelLine20.Name = "labelLine20";
		this.labelLine20.ReadOnly = false;
		this.labelLine20.SettingInVisible = false;
		this.labelLine20.SettingReadonly = false;
		this.labelLine20.ShowUnderLine = true;
		this.labelLine20.Size = new System.Drawing.Size(160, 20);
		this.labelLine20.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine20.TabIndex = 35;
		this.labelLine20.Utext = "Total Mins if No Out";
		this.labelLine24.EditControl = null;
		this.labelLine24.EditValue = "Total Mins if No In";
		this.labelLine24.FieldName = null;
		this.labelLine24.HideWhenEmpty = false;
		this.labelLine24.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine24.Location = new System.Drawing.Point(6, 153);
		this.labelLine24.Name = "labelLine24";
		this.labelLine24.ReadOnly = false;
		this.labelLine24.SettingInVisible = false;
		this.labelLine24.SettingReadonly = false;
		this.labelLine24.ShowUnderLine = true;
		this.labelLine24.Size = new System.Drawing.Size(160, 20);
		this.labelLine24.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine24.TabIndex = 33;
		this.labelLine24.Utext = "Total Mins if No In";
		this.chk_CodeByWTotal.EditControl = null;
		this.chk_CodeByWTotal.FieldName = "IsFlexWorkCode";
		this.chk_CodeByWTotal.IsSearchControl = false;
		this.chk_CodeByWTotal.Location = new System.Drawing.Point(267, 20);
		this.chk_CodeByWTotal.Name = "chk_CodeByWTotal";
		this.chk_CodeByWTotal.Properties.Caption = "Choose WorkCode by WorkTotal";
		this.chk_CodeByWTotal.Properties.ValueChecked = "Y";
		this.chk_CodeByWTotal.Properties.ValueUnchecked = "N";
		this.chk_CodeByWTotal.SettingInVisible = false;
		this.chk_CodeByWTotal.SettingReadonly = false;
		this.chk_CodeByWTotal.Size = new System.Drawing.Size(189, 20);
		this.chk_CodeByWTotal.TabIndex = 23;
		this.chk_CodeByWTotal.CheckedChanged += new System.EventHandler(this.chk_CodeByWTotal_CheckedChanged);
		this.labelControlEx6.EditControl = null;
		this.labelControlEx6.EditValue = "(mins)";
		this.labelControlEx6.FieldName = null;
		this.labelControlEx6.Location = new System.Drawing.Point(231, 127);
		this.labelControlEx6.Name = "labelControlEx6";
		this.labelControlEx6.ReadOnly = false;
		this.labelControlEx6.SettingInVisible = false;
		this.labelControlEx6.SettingReadonly = false;
		this.labelControlEx6.Size = new System.Drawing.Size(29, 13);
		this.labelControlEx6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx6.TabIndex = 31;
		this.labelControlEx6.Text = "(mins)";
		this.labelControlEx5.EditControl = null;
		this.labelControlEx5.EditValue = "(mins)";
		this.labelControlEx5.FieldName = null;
		this.labelControlEx5.Location = new System.Drawing.Point(231, 101);
		this.labelControlEx5.Name = "labelControlEx5";
		this.labelControlEx5.ReadOnly = false;
		this.labelControlEx5.SettingInVisible = false;
		this.labelControlEx5.SettingReadonly = false;
		this.labelControlEx5.Size = new System.Drawing.Size(29, 13);
		this.labelControlEx5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx5.TabIndex = 30;
		this.labelControlEx5.Text = "(mins)";
		this.chk_CalcAbsent.EditControl = null;
		this.chk_CalcAbsent.FieldName = "CalcAbsent";
		this.chk_CalcAbsent.IsSearchControl = false;
		this.chk_CalcAbsent.Location = new System.Drawing.Point(267, 46);
		this.chk_CalcAbsent.Name = "chk_CalcAbsent";
		this.chk_CalcAbsent.Properties.Caption = "Calc Absent";
		this.chk_CalcAbsent.Properties.ValueChecked = "Y";
		this.chk_CalcAbsent.Properties.ValueUnchecked = "N";
		this.chk_CalcAbsent.SettingInVisible = false;
		this.chk_CalcAbsent.SettingReadonly = false;
		this.chk_CalcAbsent.Size = new System.Drawing.Size(158, 20);
		this.chk_CalcAbsent.TabIndex = 24;
		this.chk_CalcLate.EditControl = null;
		this.chk_CalcLate.FieldName = "CalcLate";
		this.chk_CalcLate.IsSearchControl = false;
		this.chk_CalcLate.Location = new System.Drawing.Point(267, 98);
		this.chk_CalcLate.Name = "chk_CalcLate";
		this.chk_CalcLate.Properties.Caption = "Calc Late";
		this.chk_CalcLate.Properties.ValueChecked = "Y";
		this.chk_CalcLate.Properties.ValueUnchecked = "N";
		this.chk_CalcLate.SettingInVisible = false;
		this.chk_CalcLate.SettingReadonly = false;
		this.chk_CalcLate.Size = new System.Drawing.Size(158, 20);
		this.chk_CalcLate.TabIndex = 25;
		this.chk_CalcEarly.EditControl = null;
		this.chk_CalcEarly.FieldName = "CalcEarly";
		this.chk_CalcEarly.IsSearchControl = false;
		this.chk_CalcEarly.Location = new System.Drawing.Point(267, 124);
		this.chk_CalcEarly.Name = "chk_CalcEarly";
		this.chk_CalcEarly.Properties.Caption = "Calc Leave Early";
		this.chk_CalcEarly.Properties.ValueChecked = "Y";
		this.chk_CalcEarly.Properties.ValueUnchecked = "N";
		this.chk_CalcEarly.SettingInVisible = false;
		this.chk_CalcEarly.SettingReadonly = false;
		this.chk_CalcEarly.Size = new System.Drawing.Size(158, 20);
		this.chk_CalcEarly.TabIndex = 26;
		this.luk_AbsentCode.EditControl = null;
		this.luk_AbsentCode.FieldName = "AbsentCode";
		this.luk_AbsentCode.IsSearchControl = false;
		this.luk_AbsentCode.Location = new System.Drawing.Point(163, 46);
		this.luk_AbsentCode.Name = "luk_AbsentCode";
		this.luk_AbsentCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_AbsentCode.Properties.NullText = "";
		this.luk_AbsentCode.SettingInVisible = false;
		this.luk_AbsentCode.SettingReadonly = false;
		this.luk_AbsentCode.Size = new System.Drawing.Size(65, 20);
		this.luk_AbsentCode.TabIndex = 18;
		this.luk_WorkCode.EditControl = null;
		this.luk_WorkCode.FieldName = "WorkCode";
		this.luk_WorkCode.IsSearchControl = false;
		this.luk_WorkCode.Location = new System.Drawing.Point(163, 20);
		this.luk_WorkCode.Name = "luk_WorkCode";
		this.luk_WorkCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_WorkCode.Properties.NullText = "";
		this.luk_WorkCode.SettingInVisible = false;
		this.luk_WorkCode.SettingReadonly = false;
		this.luk_WorkCode.Size = new System.Drawing.Size(65, 20);
		this.luk_WorkCode.TabIndex = 17;
		this.calc_EarlyGrace.EditControl = null;
		this.calc_EarlyGrace.FieldName = "EarlyGrace";
		this.calc_EarlyGrace.IsSearchControl = false;
		this.calc_EarlyGrace.Location = new System.Drawing.Point(163, 124);
		this.calc_EarlyGrace.Name = "calc_EarlyGrace";
		this.calc_EarlyGrace.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_EarlyGrace.Properties.IsSearchControl = false;
		this.calc_EarlyGrace.SettingInVisible = false;
		this.calc_EarlyGrace.SettingReadonly = false;
		this.calc_EarlyGrace.Size = new System.Drawing.Size(65, 20);
		this.calc_EarlyGrace.TabIndex = 20;
		this.calc_LateGrace.EditControl = null;
		this.calc_LateGrace.FieldName = "LateGrace";
		this.calc_LateGrace.IsSearchControl = false;
		this.calc_LateGrace.Location = new System.Drawing.Point(163, 98);
		this.calc_LateGrace.Name = "calc_LateGrace";
		this.calc_LateGrace.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_LateGrace.Properties.IsSearchControl = false;
		this.calc_LateGrace.SettingInVisible = false;
		this.calc_LateGrace.SettingReadonly = false;
		this.calc_LateGrace.Size = new System.Drawing.Size(65, 20);
		this.calc_LateGrace.TabIndex = 19;
		this.labelLine17.EditControl = null;
		this.labelLine17.EditValue = "Early Time Allowed";
		this.labelLine17.FieldName = null;
		this.labelLine17.HideWhenEmpty = false;
		this.labelLine17.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine17.Location = new System.Drawing.Point(6, 127);
		this.labelLine17.Name = "labelLine17";
		this.labelLine17.ReadOnly = false;
		this.labelLine17.SettingInVisible = false;
		this.labelLine17.SettingReadonly = false;
		this.labelLine17.ShowUnderLine = true;
		this.labelLine17.Size = new System.Drawing.Size(160, 20);
		this.labelLine17.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine17.TabIndex = 19;
		this.labelLine17.Utext = "Early Time Allowed";
		this.labelLine18.EditControl = null;
		this.labelLine18.EditValue = "Late Time Allowed";
		this.labelLine18.FieldName = null;
		this.labelLine18.HideWhenEmpty = false;
		this.labelLine18.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine18.Location = new System.Drawing.Point(6, 101);
		this.labelLine18.Name = "labelLine18";
		this.labelLine18.ReadOnly = false;
		this.labelLine18.SettingInVisible = false;
		this.labelLine18.SettingReadonly = false;
		this.labelLine18.ShowUnderLine = true;
		this.labelLine18.Size = new System.Drawing.Size(160, 20);
		this.labelLine18.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine18.TabIndex = 17;
		this.labelLine18.Utext = "Late Time Allowed";
		this.chk_RequireIn.EditControl = null;
		this.chk_RequireIn.FieldName = "RequireInOnly";
		this.chk_RequireIn.IsSearchControl = false;
		this.chk_RequireIn.Location = new System.Drawing.Point(267, 150);
		this.chk_RequireIn.Name = "chk_RequireIn";
		this.chk_RequireIn.Properties.Caption = "Only Check In Required";
		this.chk_RequireIn.Properties.ValueChecked = "Y";
		this.chk_RequireIn.Properties.ValueUnchecked = "N";
		this.chk_RequireIn.SettingInVisible = false;
		this.chk_RequireIn.SettingReadonly = false;
		this.chk_RequireIn.Size = new System.Drawing.Size(158, 20);
		this.chk_RequireIn.TabIndex = 27;
		this.chk_RequireOut.EditControl = null;
		this.chk_RequireOut.FieldName = "RequireOutOnly";
		this.chk_RequireOut.IsSearchControl = false;
		this.chk_RequireOut.Location = new System.Drawing.Point(267, 176);
		this.chk_RequireOut.Name = "chk_RequireOut";
		this.chk_RequireOut.Properties.Caption = "Only Check Out Required";
		this.chk_RequireOut.Properties.ValueChecked = "Y";
		this.chk_RequireOut.Properties.ValueUnchecked = "N";
		this.chk_RequireOut.SettingInVisible = false;
		this.chk_RequireOut.SettingReadonly = false;
		this.chk_RequireOut.Size = new System.Drawing.Size(158, 20);
		this.chk_RequireOut.TabIndex = 28;
		this.labelLine16.EditControl = null;
		this.labelLine16.EditValue = "Absent Code";
		this.labelLine16.FieldName = null;
		this.labelLine16.HideWhenEmpty = false;
		this.labelLine16.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine16.Location = new System.Drawing.Point(6, 49);
		this.labelLine16.Name = "labelLine16";
		this.labelLine16.ReadOnly = false;
		this.labelLine16.SettingInVisible = false;
		this.labelLine16.SettingReadonly = false;
		this.labelLine16.ShowUnderLine = true;
		this.labelLine16.Size = new System.Drawing.Size(160, 20);
		this.labelLine16.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine16.TabIndex = 13;
		this.labelLine16.Utext = "Absent Code";
		this.labelLine15.EditControl = null;
		this.labelLine15.EditValue = "Fixed Work Code";
		this.labelLine15.FieldName = null;
		this.labelLine15.HideWhenEmpty = false;
		this.labelLine15.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine15.Location = new System.Drawing.Point(6, 23);
		this.labelLine15.Name = "labelLine15";
		this.labelLine15.ReadOnly = false;
		this.labelLine15.SettingInVisible = false;
		this.labelLine15.SettingReadonly = false;
		this.labelLine15.ShowUnderLine = true;
		this.labelLine15.Size = new System.Drawing.Size(160, 20);
		this.labelLine15.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine15.TabIndex = 11;
		this.labelLine15.Utext = "Fixed Work Code";
		this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.groupBox1.Controls.Add(this.labelControlEx7);
		this.groupBox1.Controls.Add(this.labelLine14);
		this.groupBox1.Controls.Add(this.calc_DayCount);
		this.groupBox1.Controls.Add(this.labelLine12);
		this.groupBox1.Controls.Add(this.calc_WorkingTotal);
		this.groupBox1.Controls.Add(this.hour_StartTimeIn);
		this.groupBox1.Controls.Add(this.labelLine21);
		this.groupBox1.Controls.Add(this.hour_EndTimeOut);
		this.groupBox1.Controls.Add(this.labelLine10);
		this.groupBox1.Controls.Add(this.hour_StartTimeOut);
		this.groupBox1.Controls.Add(this.labelLine11);
		this.groupBox1.Controls.Add(this.hour_EndTimeIn);
		this.groupBox1.Controls.Add(this.labelLine8);
		this.groupBox1.Controls.Add(this.hour_EndBreak);
		this.groupBox1.Controls.Add(this.labelLine6);
		this.groupBox1.Controls.Add(this.hour_StartBreak);
		this.groupBox1.Controls.Add(this.labelLine7);
		this.groupBox1.Controls.Add(this.hour_EndTime);
		this.groupBox1.Controls.Add(this.labelLine5);
		this.groupBox1.Controls.Add(this.hour_StartTime);
		this.groupBox1.Controls.Add(this.labelLine4);
		this.groupBox1.Location = new System.Drawing.Point(0, 0);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(684, 196);
		this.groupBox1.TabIndex = 0;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "Working Time";
		this.labelControlEx7.EditControl = null;
		this.labelControlEx7.EditValue = "(mins)";
		this.labelControlEx7.FieldName = null;
		this.labelControlEx7.Location = new System.Drawing.Point(647, 23);
		this.labelControlEx7.Name = "labelControlEx7";
		this.labelControlEx7.ReadOnly = false;
		this.labelControlEx7.SettingInVisible = false;
		this.labelControlEx7.SettingReadonly = false;
		this.labelControlEx7.Size = new System.Drawing.Size(29, 13);
		this.labelControlEx7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx7.TabIndex = 40;
		this.labelControlEx7.Text = "(mins)";
		this.labelLine14.EditControl = null;
		this.labelLine14.EditValue = "Day Count";
		this.labelLine14.FieldName = null;
		this.labelLine14.HideWhenEmpty = false;
		this.labelLine14.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine14.Location = new System.Drawing.Point(456, 49);
		this.labelLine14.Name = "labelLine14";
		this.labelLine14.ReadOnly = false;
		this.labelLine14.SettingInVisible = false;
		this.labelLine14.SettingReadonly = false;
		this.labelLine14.ShowUnderLine = true;
		this.labelLine14.Size = new System.Drawing.Size(124, 20);
		this.labelLine14.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine14.TabIndex = 39;
		this.labelLine14.Utext = "Day Count";
		this.calc_DayCount.EditControl = null;
		this.calc_DayCount.FieldName = "DayCount";
		this.calc_DayCount.IsSearchControl = false;
		this.calc_DayCount.Location = new System.Drawing.Point(580, 46);
		this.calc_DayCount.Name = "calc_DayCount";
		this.calc_DayCount.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
		this.calc_DayCount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_DayCount.Properties.IsSearchControl = false;
		this.calc_DayCount.SettingInVisible = false;
		this.calc_DayCount.SettingReadonly = false;
		this.calc_DayCount.Size = new System.Drawing.Size(65, 20);
		this.calc_DayCount.TabIndex = 16;
		this.labelLine12.EditControl = null;
		this.labelLine12.EditValue = "Working Total";
		this.labelLine12.FieldName = null;
		this.labelLine12.HideWhenEmpty = false;
		this.labelLine12.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine12.Location = new System.Drawing.Point(456, 23);
		this.labelLine12.Name = "labelLine12";
		this.labelLine12.ReadOnly = false;
		this.labelLine12.SettingInVisible = false;
		this.labelLine12.SettingReadonly = false;
		this.labelLine12.ShowUnderLine = true;
		this.labelLine12.Size = new System.Drawing.Size(124, 20);
		this.labelLine12.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine12.TabIndex = 37;
		this.labelLine12.Utext = "Working Total";
		this.calc_WorkingTotal.EditControl = null;
		this.calc_WorkingTotal.FieldName = "WorkingTotal";
		this.calc_WorkingTotal.IsSearchControl = false;
		this.calc_WorkingTotal.Location = new System.Drawing.Point(580, 20);
		this.calc_WorkingTotal.Name = "calc_WorkingTotal";
		this.calc_WorkingTotal.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_WorkingTotal.Properties.IsSearchControl = false;
		this.calc_WorkingTotal.Properties.ReadOnly = true;
		this.calc_WorkingTotal.SettingInVisible = false;
		this.calc_WorkingTotal.SettingReadonly = false;
		this.calc_WorkingTotal.Size = new System.Drawing.Size(65, 20);
		this.calc_WorkingTotal.TabIndex = 15;
		this.hour_StartTimeIn.EditControl = null;
		this.hour_StartTimeIn.FieldName = "StartTimeIn";
		this.hour_StartTimeIn.IsSearchControl = false;
		this.hour_StartTimeIn.Location = new System.Drawing.Point(391, 20);
		this.hour_StartTimeIn.Name = "hour_StartTimeIn";
		this.hour_StartTimeIn.Properties.IsSearchControl = false;
		this.hour_StartTimeIn.Properties.MaxLength = 5;
		this.hour_StartTimeIn.SettingInVisible = false;
		this.hour_StartTimeIn.SettingReadonly = false;
		this.hour_StartTimeIn.Size = new System.Drawing.Size(59, 20);
		this.hour_StartTimeIn.TabIndex = 11;
		this.labelLine21.EditControl = null;
		this.labelLine21.EditValue = "Start In Time";
		this.labelLine21.FieldName = null;
		this.labelLine21.HideWhenEmpty = false;
		this.labelLine21.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine21.Location = new System.Drawing.Point(231, 23);
		this.labelLine21.Name = "labelLine21";
		this.labelLine21.ReadOnly = false;
		this.labelLine21.SettingInVisible = false;
		this.labelLine21.SettingReadonly = false;
		this.labelLine21.ShowUnderLine = true;
		this.labelLine21.Size = new System.Drawing.Size(160, 20);
		this.labelLine21.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine21.TabIndex = 21;
		this.labelLine21.Utext = "Start In Time";
		this.hour_EndTimeOut.EditControl = null;
		this.hour_EndTimeOut.FieldName = "EndTimeOut";
		this.hour_EndTimeOut.IsSearchControl = false;
		this.hour_EndTimeOut.Location = new System.Drawing.Point(391, 113);
		this.hour_EndTimeOut.Name = "hour_EndTimeOut";
		this.hour_EndTimeOut.Properties.IsSearchControl = false;
		this.hour_EndTimeOut.Properties.MaxLength = 5;
		this.hour_EndTimeOut.SettingInVisible = false;
		this.hour_EndTimeOut.SettingReadonly = false;
		this.hour_EndTimeOut.Size = new System.Drawing.Size(59, 20);
		this.hour_EndTimeOut.TabIndex = 14;
		this.labelLine10.EditControl = null;
		this.labelLine10.EditValue = "End Out Time";
		this.labelLine10.FieldName = null;
		this.labelLine10.HideWhenEmpty = false;
		this.labelLine10.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine10.Location = new System.Drawing.Point(231, 116);
		this.labelLine10.Name = "labelLine10";
		this.labelLine10.ReadOnly = false;
		this.labelLine10.SettingInVisible = false;
		this.labelLine10.SettingReadonly = false;
		this.labelLine10.ShowUnderLine = true;
		this.labelLine10.Size = new System.Drawing.Size(160, 20);
		this.labelLine10.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine10.TabIndex = 19;
		this.labelLine10.Utext = "End Out Time";
		this.hour_StartTimeOut.EditControl = null;
		this.hour_StartTimeOut.FieldName = "StartTimeOut";
		this.hour_StartTimeOut.IsSearchControl = false;
		this.hour_StartTimeOut.Location = new System.Drawing.Point(391, 87);
		this.hour_StartTimeOut.Name = "hour_StartTimeOut";
		this.hour_StartTimeOut.Properties.IsSearchControl = false;
		this.hour_StartTimeOut.Properties.MaxLength = 5;
		this.hour_StartTimeOut.SettingInVisible = false;
		this.hour_StartTimeOut.SettingReadonly = false;
		this.hour_StartTimeOut.Size = new System.Drawing.Size(59, 20);
		this.hour_StartTimeOut.TabIndex = 13;
		this.labelLine11.EditControl = null;
		this.labelLine11.EditValue = "Start Out Time";
		this.labelLine11.FieldName = null;
		this.labelLine11.HideWhenEmpty = false;
		this.labelLine11.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine11.Location = new System.Drawing.Point(231, 90);
		this.labelLine11.Name = "labelLine11";
		this.labelLine11.ReadOnly = false;
		this.labelLine11.SettingInVisible = false;
		this.labelLine11.SettingReadonly = false;
		this.labelLine11.ShowUnderLine = true;
		this.labelLine11.Size = new System.Drawing.Size(160, 20);
		this.labelLine11.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine11.TabIndex = 17;
		this.labelLine11.Utext = "Start Out Time";
		this.hour_EndTimeIn.EditControl = null;
		this.hour_EndTimeIn.FieldName = "EndTimeIn";
		this.hour_EndTimeIn.IsSearchControl = false;
		this.hour_EndTimeIn.Location = new System.Drawing.Point(391, 46);
		this.hour_EndTimeIn.Name = "hour_EndTimeIn";
		this.hour_EndTimeIn.Properties.IsSearchControl = false;
		this.hour_EndTimeIn.Properties.MaxLength = 5;
		this.hour_EndTimeIn.SettingInVisible = false;
		this.hour_EndTimeIn.SettingReadonly = false;
		this.hour_EndTimeIn.Size = new System.Drawing.Size(59, 20);
		this.hour_EndTimeIn.TabIndex = 12;
		this.labelLine8.EditControl = null;
		this.labelLine8.EditValue = "End In Time";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(231, 49);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ReadOnly = false;
		this.labelLine8.SettingInVisible = false;
		this.labelLine8.SettingReadonly = false;
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(160, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 15;
		this.labelLine8.Utext = "End In Time";
		this.hour_EndBreak.EditControl = null;
		this.hour_EndBreak.FieldName = "EndBreak";
		this.hour_EndBreak.IsSearchControl = false;
		this.hour_EndBreak.Location = new System.Drawing.Point(166, 113);
		this.hour_EndBreak.Name = "hour_EndBreak";
		this.hour_EndBreak.Properties.IsSearchControl = false;
		this.hour_EndBreak.Properties.MaxLength = 5;
		this.hour_EndBreak.SettingInVisible = false;
		this.hour_EndBreak.SettingReadonly = false;
		this.hour_EndBreak.Size = new System.Drawing.Size(59, 20);
		this.hour_EndBreak.TabIndex = 10;
		this.hour_EndBreak.EditValueChanged += new System.EventHandler(this.hour_EndBreak_EditValueChanged);
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "End Breaking Time";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(6, 116);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(160, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 11;
		this.labelLine6.Utext = "End Breaking Time";
		this.hour_StartBreak.EditControl = null;
		this.hour_StartBreak.FieldName = "StartBreak";
		this.hour_StartBreak.IsSearchControl = false;
		this.hour_StartBreak.Location = new System.Drawing.Point(166, 87);
		this.hour_StartBreak.Name = "hour_StartBreak";
		this.hour_StartBreak.Properties.IsSearchControl = false;
		this.hour_StartBreak.Properties.MaxLength = 5;
		this.hour_StartBreak.SettingInVisible = false;
		this.hour_StartBreak.SettingReadonly = false;
		this.hour_StartBreak.Size = new System.Drawing.Size(59, 20);
		this.hour_StartBreak.TabIndex = 9;
		this.hour_StartBreak.EditValueChanged += new System.EventHandler(this.hour_StartBreak_EditValueChanged);
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "Start  Breaking Time";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(6, 90);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(160, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 9;
		this.labelLine7.Utext = "Start  Breaking Time";
		this.hour_EndTime.EditControl = null;
		this.hour_EndTime.FieldName = "EndTime";
		this.hour_EndTime.IsSearchControl = false;
		this.hour_EndTime.Location = new System.Drawing.Point(166, 46);
		this.hour_EndTime.Name = "hour_EndTime";
		this.hour_EndTime.Properties.IsSearchControl = false;
		this.hour_EndTime.Properties.MaxLength = 5;
		this.hour_EndTime.SettingInVisible = false;
		this.hour_EndTime.SettingReadonly = false;
		this.hour_EndTime.Size = new System.Drawing.Size(59, 20);
		this.hour_EndTime.TabIndex = 8;
		this.hour_EndTime.EditValueChanged += new System.EventHandler(this.hour_EndTime_EditValueChanged);
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "End Working Time";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(6, 49);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(160, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 7;
		this.labelLine5.Utext = "End Working Time";
		this.hour_StartTime.EditControl = null;
		this.hour_StartTime.FieldName = "StartTime";
		this.hour_StartTime.IsSearchControl = false;
		this.hour_StartTime.Location = new System.Drawing.Point(166, 20);
		this.hour_StartTime.Name = "hour_StartTime";
		this.hour_StartTime.Properties.IsSearchControl = false;
		this.hour_StartTime.Properties.MaxLength = 5;
		this.hour_StartTime.SettingInVisible = false;
		this.hour_StartTime.SettingReadonly = false;
		this.hour_StartTime.Size = new System.Drawing.Size(59, 20);
		this.hour_StartTime.TabIndex = 7;
		this.hour_StartTime.EditValueChanged += new System.EventHandler(this.hour_StartTime_EditValueChanged);
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Start Working Time";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(6, 23);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(160, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 5;
		this.labelLine4.Utext = "Start Working Time";
		this.xtraTabPageEx2.Controls.Add(this.labelLine13);
		this.xtraTabPageEx2.Controls.Add(this.luk_OTWCode);
		this.xtraTabPageEx2.Controls.Add(this.chk_CalcHLOT);
		this.xtraTabPageEx2.Controls.Add(this.chk_CalcWKOT);
		this.xtraTabPageEx2.Controls.Add(this.labelLine22);
		this.xtraTabPageEx2.Controls.Add(this.labelLine23);
		this.xtraTabPageEx2.Controls.Add(this.labelControlEx3);
		this.xtraTabPageEx2.Controls.Add(this.calc_MaxAfterOT);
		this.xtraTabPageEx2.Controls.Add(this.labelControlEx4);
		this.xtraTabPageEx2.Controls.Add(this.calc_MaxBeforeOT);
		this.xtraTabPageEx2.Controls.Add(this.chk_IsOTShift);
		this.xtraTabPageEx2.Controls.Add(this.labelControlEx2);
		this.xtraTabPageEx2.Controls.Add(this.calc_MinAfterDuty);
		this.xtraTabPageEx2.Controls.Add(this.chk_CalcAfterOT);
		this.xtraTabPageEx2.Controls.Add(this.luk_HLOTWCode);
		this.xtraTabPageEx2.Controls.Add(this.luk_WKOTWCode);
		this.xtraTabPageEx2.Controls.Add(this.luk_WDOTWCode);
		this.xtraTabPageEx2.Controls.Add(this.labelControlEx1);
		this.xtraTabPageEx2.Controls.Add(this.calc_MinBeforeDuty);
		this.xtraTabPageEx2.Controls.Add(this.chk_CalcBeforeOT);
		this.xtraTabPageEx2.EditControl = null;
		this.xtraTabPageEx2.EditValue = "Other";
		this.xtraTabPageEx2.FieldName = "";
		this.xtraTabPageEx2.IsSearchControl = false;
		this.xtraTabPageEx2.Name = "xtraTabPageEx2";
		this.xtraTabPageEx2.ReadOnly = false;
		this.xtraTabPageEx2.SettingInVisible = false;
		this.xtraTabPageEx2.SettingReadonly = false;
		this.xtraTabPageEx2.Size = new System.Drawing.Size(688, 360);
		this.xtraTabPageEx2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageEx2.Text = "Other";
		this.labelLine13.EditControl = null;
		this.labelLine13.EditValue = "WorkDay OT Code";
		this.labelLine13.FieldName = null;
		this.labelLine13.HideWhenEmpty = false;
		this.labelLine13.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine13.Location = new System.Drawing.Point(5, 34);
		this.labelLine13.Name = "labelLine13";
		this.labelLine13.ReadOnly = false;
		this.labelLine13.SettingInVisible = false;
		this.labelLine13.SettingReadonly = false;
		this.labelLine13.ShowUnderLine = true;
		this.labelLine13.Size = new System.Drawing.Size(167, 20);
		this.labelLine13.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine13.TabIndex = 29;
		this.labelLine13.Utext = "WorkDay OT Code";
		this.luk_OTWCode.EditControl = null;
		this.luk_OTWCode.FieldName = null;
		this.luk_OTWCode.IsSearchControl = false;
		this.luk_OTWCode.Location = new System.Drawing.Point(172, 6);
		this.luk_OTWCode.Name = "luk_OTWCode";
		this.luk_OTWCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_OTWCode.Properties.NullText = "";
		this.luk_OTWCode.SettingInVisible = false;
		this.luk_OTWCode.SettingReadonly = false;
		this.luk_OTWCode.Size = new System.Drawing.Size(65, 20);
		this.luk_OTWCode.TabIndex = 28;
		this.chk_CalcHLOT.EditControl = null;
		this.chk_CalcHLOT.FieldName = "CalcHLOT";
		this.chk_CalcHLOT.IsSearchControl = false;
		this.chk_CalcHLOT.Location = new System.Drawing.Point(4, 84);
		this.chk_CalcHLOT.Name = "chk_CalcHLOT";
		this.chk_CalcHLOT.Properties.Caption = "Calculate Holiday OT";
		this.chk_CalcHLOT.Properties.ValueChecked = "Y";
		this.chk_CalcHLOT.Properties.ValueUnchecked = "N";
		this.chk_CalcHLOT.SettingInVisible = false;
		this.chk_CalcHLOT.SettingReadonly = false;
		this.chk_CalcHLOT.Size = new System.Drawing.Size(167, 20);
		this.chk_CalcHLOT.TabIndex = 33;
		this.chk_CalcWKOT.EditControl = null;
		this.chk_CalcWKOT.FieldName = "CalcWKOT";
		this.chk_CalcWKOT.IsSearchControl = false;
		this.chk_CalcWKOT.Location = new System.Drawing.Point(4, 58);
		this.chk_CalcWKOT.Name = "chk_CalcWKOT";
		this.chk_CalcWKOT.Properties.Caption = "Calculate Weekend OT";
		this.chk_CalcWKOT.Properties.ValueChecked = "Y";
		this.chk_CalcWKOT.Properties.ValueUnchecked = "N";
		this.chk_CalcWKOT.SettingInVisible = false;
		this.chk_CalcWKOT.SettingReadonly = false;
		this.chk_CalcWKOT.Size = new System.Drawing.Size(167, 20);
		this.chk_CalcWKOT.TabIndex = 31;
		this.labelLine22.EditControl = null;
		this.labelLine22.EditValue = "Max OT Aft. Work Time";
		this.labelLine22.FieldName = null;
		this.labelLine22.HideWhenEmpty = false;
		this.labelLine22.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine22.Location = new System.Drawing.Point(3, 188);
		this.labelLine22.Name = "labelLine22";
		this.labelLine22.ReadOnly = false;
		this.labelLine22.SettingInVisible = false;
		this.labelLine22.SettingReadonly = false;
		this.labelLine22.ShowUnderLine = true;
		this.labelLine22.Size = new System.Drawing.Size(169, 20);
		this.labelLine22.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine22.TabIndex = 41;
		this.labelLine22.Utext = "Max OT Aft. Work Time";
		this.labelLine23.EditControl = null;
		this.labelLine23.EditValue = "Max OT Bef. Work Time";
		this.labelLine23.FieldName = null;
		this.labelLine23.HideWhenEmpty = false;
		this.labelLine23.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine23.Location = new System.Drawing.Point(3, 162);
		this.labelLine23.Name = "labelLine23";
		this.labelLine23.ReadOnly = false;
		this.labelLine23.SettingInVisible = false;
		this.labelLine23.SettingReadonly = false;
		this.labelLine23.ShowUnderLine = true;
		this.labelLine23.Size = new System.Drawing.Size(169, 20);
		this.labelLine23.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine23.TabIndex = 39;
		this.labelLine23.Utext = "Max OT Bef. Work Time";
		this.labelControlEx3.EditControl = null;
		this.labelControlEx3.EditValue = "(mins)";
		this.labelControlEx3.FieldName = null;
		this.labelControlEx3.Location = new System.Drawing.Point(243, 188);
		this.labelControlEx3.Name = "labelControlEx3";
		this.labelControlEx3.ReadOnly = false;
		this.labelControlEx3.SettingInVisible = false;
		this.labelControlEx3.SettingReadonly = false;
		this.labelControlEx3.Size = new System.Drawing.Size(29, 13);
		this.labelControlEx3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx3.TabIndex = 34;
		this.labelControlEx3.Text = "(mins)";
		this.calc_MaxAfterOT.EditControl = null;
		this.calc_MaxAfterOT.FieldName = "MaxAfterOT";
		this.calc_MaxAfterOT.IsSearchControl = false;
		this.calc_MaxAfterOT.Location = new System.Drawing.Point(172, 185);
		this.calc_MaxAfterOT.Name = "calc_MaxAfterOT";
		this.calc_MaxAfterOT.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_MaxAfterOT.Properties.IsSearchControl = false;
		this.calc_MaxAfterOT.SettingInVisible = false;
		this.calc_MaxAfterOT.SettingReadonly = false;
		this.calc_MaxAfterOT.Size = new System.Drawing.Size(65, 20);
		this.calc_MaxAfterOT.TabIndex = 42;
		this.labelControlEx4.EditControl = null;
		this.labelControlEx4.EditValue = "(mins)";
		this.labelControlEx4.FieldName = null;
		this.labelControlEx4.Location = new System.Drawing.Point(243, 163);
		this.labelControlEx4.Name = "labelControlEx4";
		this.labelControlEx4.ReadOnly = false;
		this.labelControlEx4.SettingInVisible = false;
		this.labelControlEx4.SettingReadonly = false;
		this.labelControlEx4.Size = new System.Drawing.Size(29, 13);
		this.labelControlEx4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx4.TabIndex = 32;
		this.labelControlEx4.Text = "(mins)";
		this.calc_MaxBeforeOT.EditControl = null;
		this.calc_MaxBeforeOT.FieldName = "MaxBeforeOT";
		this.calc_MaxBeforeOT.IsSearchControl = false;
		this.calc_MaxBeforeOT.Location = new System.Drawing.Point(172, 160);
		this.calc_MaxBeforeOT.Name = "calc_MaxBeforeOT";
		this.calc_MaxBeforeOT.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_MaxBeforeOT.Properties.IsSearchControl = false;
		this.calc_MaxBeforeOT.SettingInVisible = false;
		this.calc_MaxBeforeOT.SettingReadonly = false;
		this.calc_MaxBeforeOT.Size = new System.Drawing.Size(65, 20);
		this.calc_MaxBeforeOT.TabIndex = 40;
		this.chk_IsOTShift.EditControl = null;
		this.chk_IsOTShift.FieldName = "IsOTShift";
		this.chk_IsOTShift.IsSearchControl = false;
		this.chk_IsOTShift.Location = new System.Drawing.Point(5, 6);
		this.chk_IsOTShift.Name = "chk_IsOTShift";
		this.chk_IsOTShift.Properties.Caption = "Overtime Shift";
		this.chk_IsOTShift.Properties.ValueChecked = "Y";
		this.chk_IsOTShift.Properties.ValueUnchecked = "N";
		this.chk_IsOTShift.SettingInVisible = false;
		this.chk_IsOTShift.SettingReadonly = false;
		this.chk_IsOTShift.Size = new System.Drawing.Size(167, 20);
		this.chk_IsOTShift.TabIndex = 27;
		this.labelControlEx2.EditControl = null;
		this.labelControlEx2.EditValue = "(mins)";
		this.labelControlEx2.FieldName = null;
		this.labelControlEx2.Location = new System.Drawing.Point(243, 137);
		this.labelControlEx2.Name = "labelControlEx2";
		this.labelControlEx2.ReadOnly = false;
		this.labelControlEx2.SettingInVisible = false;
		this.labelControlEx2.SettingReadonly = false;
		this.labelControlEx2.Size = new System.Drawing.Size(29, 13);
		this.labelControlEx2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx2.TabIndex = 29;
		this.labelControlEx2.Text = "(mins)";
		this.calc_MinAfterDuty.EditControl = null;
		this.calc_MinAfterDuty.FieldName = "MinAfterDuty";
		this.calc_MinAfterDuty.IsSearchControl = false;
		this.calc_MinAfterDuty.Location = new System.Drawing.Point(172, 134);
		this.calc_MinAfterDuty.Name = "calc_MinAfterDuty";
		this.calc_MinAfterDuty.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_MinAfterDuty.Properties.IsSearchControl = false;
		this.calc_MinAfterDuty.SettingInVisible = false;
		this.calc_MinAfterDuty.SettingReadonly = false;
		this.calc_MinAfterDuty.Size = new System.Drawing.Size(65, 20);
		this.calc_MinAfterDuty.TabIndex = 38;
		this.chk_CalcAfterOT.EditControl = null;
		this.chk_CalcAfterOT.FieldName = "CalcAfterOT";
		this.chk_CalcAfterOT.IsSearchControl = false;
		this.chk_CalcAfterOT.Location = new System.Drawing.Point(4, 134);
		this.chk_CalcAfterOT.Name = "chk_CalcAfterOT";
		this.chk_CalcAfterOT.Properties.Caption = "Calculate OT After WorkTime";
		this.chk_CalcAfterOT.Properties.ValueChecked = "Y";
		this.chk_CalcAfterOT.Properties.ValueUnchecked = "N";
		this.chk_CalcAfterOT.SettingInVisible = false;
		this.chk_CalcAfterOT.SettingReadonly = false;
		this.chk_CalcAfterOT.Size = new System.Drawing.Size(167, 20);
		this.chk_CalcAfterOT.TabIndex = 37;
		this.luk_HLOTWCode.EditControl = null;
		this.luk_HLOTWCode.FieldName = "HLOTWCode";
		this.luk_HLOTWCode.IsSearchControl = false;
		this.luk_HLOTWCode.Location = new System.Drawing.Point(172, 83);
		this.luk_HLOTWCode.Name = "luk_HLOTWCode";
		this.luk_HLOTWCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_HLOTWCode.Properties.NullText = "";
		this.luk_HLOTWCode.SettingInVisible = false;
		this.luk_HLOTWCode.SettingReadonly = false;
		this.luk_HLOTWCode.Size = new System.Drawing.Size(65, 20);
		this.luk_HLOTWCode.TabIndex = 34;
		this.luk_WKOTWCode.EditControl = null;
		this.luk_WKOTWCode.FieldName = "WKOTWCode";
		this.luk_WKOTWCode.IsSearchControl = false;
		this.luk_WKOTWCode.Location = new System.Drawing.Point(172, 57);
		this.luk_WKOTWCode.Name = "luk_WKOTWCode";
		this.luk_WKOTWCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_WKOTWCode.Properties.NullText = "";
		this.luk_WKOTWCode.SettingInVisible = false;
		this.luk_WKOTWCode.SettingReadonly = false;
		this.luk_WKOTWCode.Size = new System.Drawing.Size(65, 20);
		this.luk_WKOTWCode.TabIndex = 32;
		this.luk_WDOTWCode.EditControl = null;
		this.luk_WDOTWCode.FieldName = "WDOTWCode";
		this.luk_WDOTWCode.IsSearchControl = false;
		this.luk_WDOTWCode.Location = new System.Drawing.Point(172, 31);
		this.luk_WDOTWCode.Name = "luk_WDOTWCode";
		this.luk_WDOTWCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_WDOTWCode.Properties.NullText = "";
		this.luk_WDOTWCode.SettingInVisible = false;
		this.luk_WDOTWCode.SettingReadonly = false;
		this.luk_WDOTWCode.Size = new System.Drawing.Size(65, 20);
		this.luk_WDOTWCode.TabIndex = 30;
		this.labelControlEx1.EditControl = null;
		this.labelControlEx1.EditValue = "(mins)";
		this.labelControlEx1.FieldName = null;
		this.labelControlEx1.Location = new System.Drawing.Point(243, 112);
		this.labelControlEx1.Name = "labelControlEx1";
		this.labelControlEx1.ReadOnly = false;
		this.labelControlEx1.SettingInVisible = false;
		this.labelControlEx1.SettingReadonly = false;
		this.labelControlEx1.Size = new System.Drawing.Size(29, 13);
		this.labelControlEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx1.TabIndex = 17;
		this.labelControlEx1.Text = "(mins)";
		this.calc_MinBeforeDuty.EditControl = null;
		this.calc_MinBeforeDuty.FieldName = "MinBeforeDuty";
		this.calc_MinBeforeDuty.IsSearchControl = false;
		this.calc_MinBeforeDuty.Location = new System.Drawing.Point(172, 109);
		this.calc_MinBeforeDuty.Name = "calc_MinBeforeDuty";
		this.calc_MinBeforeDuty.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_MinBeforeDuty.Properties.IsSearchControl = false;
		this.calc_MinBeforeDuty.SettingInVisible = false;
		this.calc_MinBeforeDuty.SettingReadonly = false;
		this.calc_MinBeforeDuty.Size = new System.Drawing.Size(65, 20);
		this.calc_MinBeforeDuty.TabIndex = 36;
		this.chk_CalcBeforeOT.EditControl = null;
		this.chk_CalcBeforeOT.FieldName = "CalcBeforeOT";
		this.chk_CalcBeforeOT.IsSearchControl = false;
		this.chk_CalcBeforeOT.Location = new System.Drawing.Point(4, 109);
		this.chk_CalcBeforeOT.Name = "chk_CalcBeforeOT";
		this.chk_CalcBeforeOT.Properties.Caption = "Calculate OT Before WorkTime";
		this.chk_CalcBeforeOT.Properties.ValueChecked = "Y";
		this.chk_CalcBeforeOT.Properties.ValueUnchecked = "N";
		this.chk_CalcBeforeOT.SettingInVisible = false;
		this.chk_CalcBeforeOT.SettingReadonly = false;
		this.chk_CalcBeforeOT.Size = new System.Drawing.Size(167, 20);
		this.chk_CalcBeforeOT.TabIndex = 35;
		this.xtraTabPageEx3.Controls.Add(this.grd_CombineWorkCode);
		this.xtraTabPageEx3.EditControl = null;
		this.xtraTabPageEx3.EditValue = "Combine WorkCode";
		this.xtraTabPageEx3.FieldName = "";
		this.xtraTabPageEx3.IsSearchControl = false;
		this.xtraTabPageEx3.Name = "xtraTabPageEx3";
		this.xtraTabPageEx3.ReadOnly = false;
		this.xtraTabPageEx3.SettingInVisible = false;
		this.xtraTabPageEx3.SettingReadonly = false;
		this.xtraTabPageEx3.Size = new System.Drawing.Size(688, 360);
		this.xtraTabPageEx3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageEx3.Text = "Combine WorkCode";
		this.grd_CombineWorkCode.AllowPasteExcel = true;
		this.grd_CombineWorkCode.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_CombineWorkCode.ClipboardData = null;
		this.grd_CombineWorkCode.ContextMenuStrip = this.ctm_MinutesConvert;
		this.grd_CombineWorkCode.Location = new System.Drawing.Point(4, 4);
		this.grd_CombineWorkCode.MainView = this.gv_CombineWorkCode;
		this.grd_CombineWorkCode.Name = "grd_CombineWorkCode";
		this.grd_CombineWorkCode.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rep_FromMin, this.rep_ToMin, this.rep_WorkCode, this.rep_AbsentCode });
		this.grd_CombineWorkCode.Size = new System.Drawing.Size(677, 353);
		this.grd_CombineWorkCode.TabIndex = 0;
		this.grd_CombineWorkCode.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_CombineWorkCode });
		this.ctm_MinutesConvert.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.ctmMcAddMenuTrip, this.ctmMcRemoveMenuTrip });
		this.ctm_MinutesConvert.Name = "ctm_Doc";
		this.ctm_MinutesConvert.Size = new System.Drawing.Size(118, 48);
		this.ctmMcAddMenuTrip.Name = "ctmMcAddMenuTrip";
		this.ctmMcAddMenuTrip.Size = new System.Drawing.Size(117, 22);
		this.ctmMcAddMenuTrip.Text = "Add";
		this.ctmMcAddMenuTrip.Click += new System.EventHandler(this.ctmMcAddMenuTrip_Click);
		this.ctmMcRemoveMenuTrip.Name = "ctmMcRemoveMenuTrip";
		this.ctmMcRemoveMenuTrip.Size = new System.Drawing.Size(117, 22);
		this.ctmMcRemoveMenuTrip.Text = "Remove";
		this.ctmMcRemoveMenuTrip.Click += new System.EventHandler(this.ctmMcRemoveMenuTrip_Click);
		this.gv_CombineWorkCode.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_CombineWorkCode.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_CombineWorkCode.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_CombineWorkCode.ColumnKey = null;
		this.gv_CombineWorkCode.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gc_LineNum, this.gc_FromMinutes, this.gc_ToMinutes, this.gc_WorkCode, this.gc_AbsentCode, this.gc_Remarks });
		this.gv_CombineWorkCode.DisableFormSetting = false;
		this.gv_CombineWorkCode.DisplayException = false;
		this.gv_CombineWorkCode.EditColumn = null;
		this.gv_CombineWorkCode.FieldCurrency = null;
		this.gv_CombineWorkCode.FieldRate = null;
		this.gv_CombineWorkCode.FormId = null;
		this.gv_CombineWorkCode.GridControl = this.grd_CombineWorkCode;
		this.gv_CombineWorkCode.HasFormSetting = false;
		this.gv_CombineWorkCode.HeaderExcelExport = null;
		this.gv_CombineWorkCode.HeaderExcelType = Apzon.Commons.Enums.Enums.HeaderExcelType.Disabled;
		this.gv_CombineWorkCode.IgnoreFilter = false;
		this.gv_CombineWorkCode.IndexEditRow = -1;
		this.gv_CombineWorkCode.IndicatorWidth = 35;
		this.gv_CombineWorkCode.IsApzTable = false;
		this.gv_CombineWorkCode.IsInsertNewRow = false;
		this.gv_CombineWorkCode.IsLoadingData = false;
		this.gv_CombineWorkCode.IsMappingContextAction = false;
		this.gv_CombineWorkCode.ItemId = "APZ_EWS1";
		this.gv_CombineWorkCode.LstColumnAllowBatch = null;
		this.gv_CombineWorkCode.LstColumnNotAllowBatch = null;
		this.gv_CombineWorkCode.LstColumnNotEditable = null;
		this.gv_CombineWorkCode.LstColumNotSetting = null;
		this.gv_CombineWorkCode.LstOldUDF = null;
		this.gv_CombineWorkCode.Name = "gv_CombineWorkCode";
		this.gv_CombineWorkCode.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_CombineWorkCode.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_CombineWorkCode.OptionsClipboard.ClipboardMode = DevExpress.Export.ClipboardMode.Formatted;
		this.gv_CombineWorkCode.OptionsPrint.AutoWidth = false;
		this.gv_CombineWorkCode.OptionsPrint.UsePrintStyles = false;
		this.gv_CombineWorkCode.OptionsSelection.MultiSelect = true;
		this.gv_CombineWorkCode.OptionsView.BestFitMaxRowCount = 100;
		this.gv_CombineWorkCode.OptionsView.ColumnAutoWidth = false;
		this.gv_CombineWorkCode.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_CombineWorkCode.OptionsView.ShowGroupedColumns = true;
		this.gv_CombineWorkCode.OptionsView.ShowGroupPanel = false;
		this.gv_CombineWorkCode.ReadOnly = false;
		this.gv_CombineWorkCode.SettingReadonly = false;
		this.gv_CombineWorkCode.UseBatchMod = false;
		this.gv_CombineWorkCode.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_CombineWorkCode_CellValueChanging);
		this.gc_LineNum.Caption = "LineNum";
		this.gc_LineNum.CurrencyDateField = null;
		this.gc_LineNum.CurrencyField = null;
		this.gc_LineNum.CurrencyFixed = null;
		this.gc_LineNum.CurrencyFixedRate = 0.0;
		this.gc_LineNum.CurrencyRateField = null;
		this.gc_LineNum.FieldName = "LineNum";
		this.gc_LineNum.Name = "gc_LineNum";
		this.gc_LineNum.PrimaryKey = false;
		this.gc_LineNum.UnitField = null;
		this.gc_FromMinutes.Caption = "From Minutes";
		this.gc_FromMinutes.ColumnEdit = this.rep_FromMin;
		this.gc_FromMinutes.CurrencyDateField = null;
		this.gc_FromMinutes.CurrencyField = null;
		this.gc_FromMinutes.CurrencyFixed = null;
		this.gc_FromMinutes.CurrencyFixedRate = 0.0;
		this.gc_FromMinutes.CurrencyRateField = null;
		this.gc_FromMinutes.FieldName = "FromMinutes";
		this.gc_FromMinutes.Name = "gc_FromMinutes";
		this.gc_FromMinutes.PrimaryKey = true;
		this.gc_FromMinutes.UnitField = null;
		this.gc_FromMinutes.Visible = true;
		this.gc_FromMinutes.VisibleIndex = 0;
		this.rep_FromMin.AutoHeight = false;
		this.rep_FromMin.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_FromMin.Name = "rep_FromMin";
		this.gc_ToMinutes.Caption = "To Minutes";
		this.gc_ToMinutes.ColumnEdit = this.rep_ToMin;
		this.gc_ToMinutes.CurrencyDateField = null;
		this.gc_ToMinutes.CurrencyField = null;
		this.gc_ToMinutes.CurrencyFixed = null;
		this.gc_ToMinutes.CurrencyFixedRate = 0.0;
		this.gc_ToMinutes.CurrencyRateField = null;
		this.gc_ToMinutes.FieldName = "ToMinutes";
		this.gc_ToMinutes.Name = "gc_ToMinutes";
		this.gc_ToMinutes.PrimaryKey = false;
		this.gc_ToMinutes.UnitField = null;
		this.gc_ToMinutes.Visible = true;
		this.gc_ToMinutes.VisibleIndex = 1;
		this.rep_ToMin.AutoHeight = false;
		this.rep_ToMin.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ToMin.Name = "rep_ToMin";
		this.gc_WorkCode.Caption = "WorkCode";
		this.gc_WorkCode.ColumnEdit = this.rep_WorkCode;
		this.gc_WorkCode.CurrencyDateField = null;
		this.gc_WorkCode.CurrencyField = null;
		this.gc_WorkCode.CurrencyFixed = null;
		this.gc_WorkCode.CurrencyFixedRate = 0.0;
		this.gc_WorkCode.CurrencyRateField = null;
		this.gc_WorkCode.FieldName = "WorkCode";
		this.gc_WorkCode.Name = "gc_WorkCode";
		this.gc_WorkCode.PrimaryKey = false;
		this.gc_WorkCode.UnitField = null;
		this.gc_WorkCode.Visible = true;
		this.gc_WorkCode.VisibleIndex = 2;
		this.gc_WorkCode.Width = 134;
		this.rep_WorkCode.AutoHeight = false;
		this.rep_WorkCode.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_WorkCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_WorkCode.Name = "rep_WorkCode";
		this.rep_WorkCode.NullText = "";
		this.gc_AbsentCode.Caption = "AbsentCode";
		this.gc_AbsentCode.ColumnEdit = this.rep_AbsentCode;
		this.gc_AbsentCode.CurrencyDateField = null;
		this.gc_AbsentCode.CurrencyField = null;
		this.gc_AbsentCode.CurrencyFixed = null;
		this.gc_AbsentCode.CurrencyFixedRate = 0.0;
		this.gc_AbsentCode.CurrencyRateField = null;
		this.gc_AbsentCode.FieldName = "AbsentCode";
		this.gc_AbsentCode.Name = "gc_AbsentCode";
		this.gc_AbsentCode.PrimaryKey = false;
		this.gc_AbsentCode.UnitField = null;
		this.gc_AbsentCode.Visible = true;
		this.gc_AbsentCode.VisibleIndex = 3;
		this.gc_AbsentCode.Width = 126;
		this.rep_AbsentCode.AutoHeight = false;
		this.rep_AbsentCode.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_AbsentCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_AbsentCode.Name = "rep_AbsentCode";
		this.rep_AbsentCode.NullText = "";
		this.gc_Remarks.Caption = "Remarks";
		this.gc_Remarks.CurrencyDateField = null;
		this.gc_Remarks.CurrencyField = null;
		this.gc_Remarks.CurrencyFixed = null;
		this.gc_Remarks.CurrencyFixedRate = 0.0;
		this.gc_Remarks.CurrencyRateField = null;
		this.gc_Remarks.FieldName = "Remarks";
		this.gc_Remarks.Name = "gc_Remarks";
		this.gc_Remarks.PrimaryKey = false;
		this.gc_Remarks.UnitField = null;
		this.gc_Remarks.Visible = true;
		this.gc_Remarks.VisibleIndex = 4;
		this.gc_Remarks.Width = 131;
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(81, 446);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 20);
		this.btn_Cancel.TabIndex = 21;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "OK";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(0, 446);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.ReadOnly = false;
		this.btn_Ok.SettingInVisible = false;
		this.btn_Ok.SettingReadonly = false;
		this.btn_Ok.Size = new System.Drawing.Size(75, 20);
		this.btn_Ok.TabIndex = 20;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		base.AcceptButton = this.btn_Ok;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(1035, 469);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oEmployeeWorkShift";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_WorkShift.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_WorkShift";
		this.Text = "Employee Work Shift";
		base.OnControlValueChanged += new Apzon.Controls.Form.ControlValueChanged(this.frm_WorkShift_OnControlValueChanged);
		base.OnControlValueChanging += new Apzon.Controls.Form.ControlValueChanging(this.frm_WorkShift_OnControlValueChanging);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_WorkShift_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctm_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.hour_EndNT.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_NightShift.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Dscription.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hour_StartNT.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).EndInit();
		this.xtraTabControlEx1.ResumeLayout(false);
		this.xtraTabPageEx1.ResumeLayout(false);
		this.groupBox2.ResumeLayout(false);
		this.groupBox2.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_CalHoliday.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_HldCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_NoOutWT.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_NoInWT.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CodeByWTotal.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CalcAbsent.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CalcLate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CalcEarly.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_AbsentCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_WorkCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_EarlyGrace.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_LateGrace.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_RequireIn.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_RequireOut.Properties).EndInit();
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.calc_DayCount.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_WorkingTotal.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hour_StartTimeIn.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hour_EndTimeOut.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hour_StartTimeOut.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hour_EndTimeIn.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hour_EndBreak.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hour_StartBreak.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hour_EndTime.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hour_StartTime.Properties).EndInit();
		this.xtraTabPageEx2.ResumeLayout(false);
		this.xtraTabPageEx2.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_OTWCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CalcHLOT.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CalcWKOT.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_MaxAfterOT.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_MaxBeforeOT.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_IsOTShift.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_MinAfterDuty.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CalcAfterOT.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_HLOTWCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_WKOTWCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_WDOTWCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_MinBeforeDuty.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_CalcBeforeOT.Properties).EndInit();
		this.xtraTabPageEx3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_CombineWorkCode).EndInit();
		this.ctm_MinutesConvert.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_CombineWorkCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_FromMin).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ToMin).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WorkCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_AbsentCode).EndInit();
		base.ResumeLayout(false);
	}
}
