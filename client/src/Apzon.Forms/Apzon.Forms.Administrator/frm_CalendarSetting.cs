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
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator;

public class frm_CalendarSetting : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public int EmpId;

	private int _maxCodeInDb;

	private bool _isChanging;

	private bool _isCancel;

	private bool _forceClose;

	private bool _isGridChanged;

	private bool _isControlChanged;

	private DataTable _dtGeneSettings;

	public Func<Task> RefreshCal;

	public Func<Task> RefreshSettings;

	private IContainer components;

	public SimpleButtonEx btnCancel;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	public SimpleButtonEx btn_Save;

	private RepositoryItemLookUpEdit rep_Deparment;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private RepositoryItemColorPickEdit repositoryItemColorPickEdit1;

	private GridColumnEx gridColumnEx4;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private GridColumnEx gridColumnEx5;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem visibleAllToolStripMenuItem;

	private ToolStripMenuItem visibleNoneToolStripMenuItem;

	private LabelLine lb_Remarks;

	private LabelLine labelLine3;

	private ColorPickEditEx color_Task;

	private SpinEditEx spin_ResPerPage;

	private LabelLine labelLine4;

	private LabelLine labelLine5;

	private HourEdit hour_WorkingEnd;

	private HourEdit hour_WorkingStart;

	private HourEdit hour_SatWStart;

	private LabelLine lblSatWStart;

	private LabelLine lblSatWEnd;

	private CheckEditEx chk_WorkSat;

	private SplitContainerControl splitContainerControl1;

	private HourEdit hour_SatWEnd;

	private ColorPickEditEx color_Note;

	private LabelLine labelLine8;

	private ColorPickEditEx color_Event;

	private LabelLine labelLine2;

	private ColorPickEditEx color_Completed;

	private LabelLine labelLine6;

	private ColorPickEditEx color_NotCompleted;

	private LabelLine labelLine7;

	private ColorPickEditEx color_NoStatus;

	private LabelLine labelLine9;

	private LabelControlEx labelControl3;

	private LabelControlEx labelControl2;

	private LabelControlEx labelControl1;

	private LookUpEditCombo luk_ActiveViewType;

	private LookUpEditCombo luk_GroupType;

	private LabelLine labelLine10;

	private LabelLine labelLine1;

	private CheckEditEx chk_Event;

	private CheckEditEx chk_Note;

	private CheckEditEx chk_Completed;

	private CheckEditEx chk_UnCompleted;

	private CheckEditEx chk_NoAssign;

	private LookUpEditCombo luk_Tooltip;

	private LabelLine labelLine11;

	private GridColumnEx gc_Type;

	private GridColumnEx gridColumnEx6;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btnCancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_CalendarSetting()
	{
		InitializeComponent();
		IsScrollable = true;
		IsSetSuperTip = true;
		FormId = "oCalendarSetting";
		gv_Doc.ItemId = "APZ_OACS";
		gv_Doc.FormId = FormId;
		OnLoadAsync += frm_CalendarSetting_Load;
		gv_Doc.OptionsView.ColumnAutoWidth = true;
	}

	private async Task frm_CalendarSetting_Load(object sender, EventArgs e)
	{
		try
		{
			LoadInitForm();
			await LoadData();
			SetEventControl(this);
			AcceptButton = btn_Save;
			CancelButton = btnCancel;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_CalendarSetting_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (_isCancel)
			{
				return;
			}
			if (!_forceClose)
			{
				e.Cancel = true;
			}
			if (_forceClose)
			{
				return;
			}
			if (FormMode == Enums.FormViewModeType.Update)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
				{
				case DialogResult.No:
					_forceClose = true;
					Close();
					break;
				case DialogResult.Yes:
					if (string.IsNullOrEmpty(await SaveData()))
					{
						_forceClose = true;
						Close();
					}
					break;
				}
			}
			else
			{
				_forceClose = true;
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				btn_Save.Enabled = false;
				btnCancel.Enabled = false;
				await SaveData();
				btn_Save.Enabled = true;
				btnCancel.Enabled = true;
			}
			else
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

	private void btnCancel_Click(object sender, EventArgs e)
	{
		try
		{
			_isCancel = true;
			Close();
			_isCancel = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_isChanging)
			{
				FormMode = Enums.FormViewModeType.Update;
				_isGridChanged = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
	}

	private void visibleAllToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			gv_Doc.BeginUpdate();
			for (int i = 0; i <= gv_Doc.RowCount - 1; i++)
			{
				gv_Doc.SetRowCellValue(i, "Checked", "Y");
			}
			gv_Doc.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void visibleNoneToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			gv_Doc.BeginUpdate();
			for (int i = 0; i <= gv_Doc.RowCount - 1; i++)
			{
				gv_Doc.SetRowCellValue(i, "Checked", "N");
			}
			gv_Doc.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void hour_WorkingStart_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(hour_WorkingStart.EditValue)))
			{
				hour_WorkingStart.ErrorText = "Start Time must have a value";
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void hour_WorkingEnd_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(hour_WorkingEnd.EditValue)))
			{
				hour_WorkingEnd.ErrorText = "End Time must have a value";
				e.Cancel = true;
			}
			else if (Function.ParseInt(hour_WorkingEnd.EditValue) < Function.ParseInt(hour_WorkingStart.EditValue))
			{
				hour_WorkingEnd.ErrorText = "Invalid end time";
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void hour_SatWStart_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(hour_SatWStart.EditValue)))
			{
				hour_SatWStart.ErrorText = "Start Time must have a value";
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void hour_SatWEnd_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(hour_SatWEnd.EditValue)))
			{
				hour_SatWEnd.ErrorText = "End Time must have a value";
				e.Cancel = true;
			}
			else if (Function.ParseInt(hour_SatWEnd.EditValue) < Function.ParseInt(hour_SatWStart.EditValue))
			{
				hour_SatWEnd.ErrorText = "Invalid end time";
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_WorkSat_CheckStateChanged(object sender, EventArgs e)
	{
		try
		{
			if (chk_WorkSat.CheckState == CheckState.Checked)
			{
				if (string.IsNullOrEmpty(Function.ToString(hour_SatWStart.EditValue)))
				{
					hour_SatWStart.EditValue = 800;
				}
				if (string.IsNullOrEmpty(Function.ToString(hour_SatWEnd.EditValue)))
				{
					hour_SatWEnd.EditValue = 1700;
				}
				hour_SatWStart.Visible = true;
				hour_SatWEnd.Visible = true;
				lblSatWEnd.Visible = true;
				lblSatWStart.Visible = true;
			}
			else
			{
				hour_SatWStart.Visible = false;
				hour_SatWEnd.Visible = false;
				lblSatWEnd.Visible = false;
				lblSatWStart.Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			if (sender is ColumnView columnView && ((GridColumnEx)columnView.FocusedColumn).FieldName == "Checked")
			{
				string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("ResourceId"));
				int length = text.Length;
				int num = Function.ParseInt(text.Substring(1, length - 1));
				if (text.Contains("U") && num == GlobalData.UserID)
				{
					_ = Function.ToString(e.Value) == "N";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		try
		{
			if (sender is ColumnView columnView)
			{
				e.ExceptionMode = ExceptionMode.DisplayError;
				e.WindowCaption = "Input Error";
				columnView.HideEditor();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
	{
		try
		{
			if (sender is GridView gridView && !(e.Column.FieldName != "Type") && e.IsGetData)
			{
				int listSourceRowIndex = e.ListSourceRowIndex;
				if (Function.ToString(gridView.GetListSourceRowCellValue(listSourceRowIndex, "ResourceId")).Contains("U"))
				{
					e.Value = "USER";
				}
				else
				{
					e.Value = "EMPLOYEE";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadData()
	{
		try
		{
			DataSet dataSet = await _httpClient.CalendarSettingClient.GetDoc(Function.ToString(GlobalData.UserCodeLogin));
			if (dataSet.IsNotNull())
			{
				DataTable dataTable = dataSet.Tables[0];
				dataTable.SetDefaultAndMaxLen(gv_Doc.ItemId);
				grd_Doc.DataSource = dataTable;
				grd_Doc.RefreshDataSource();
				gv_Doc.ExpandAllGroups();
				DataTable dataTable2 = dataSet.Tables[1];
				if (dataTable2.IsNotNull())
				{
					DataRow[] array = dataTable2.AsEnumerable().FirstOrDefault().ToArray();
					if (array.Length != 0)
					{
						_dtGeneSettings = array.CopyToDataTable();
					}
				}
				else
				{
					_dtGeneSettings = WinFuntions.GetTableStructure("APZ_ACS1");
				}
				if (_dtGeneSettings.Rows.Count <= 0)
				{
					DataRow row = _dtGeneSettings.NewRow();
					_dtGeneSettings.Rows.Add(row);
				}
				DataRow dataRow = _dtGeneSettings.Rows[0];
				if (string.IsNullOrEmpty(Function.ToString(dataRow["StartWorkingHour"])))
				{
					dataRow["StartWorkingHour"] = 800;
				}
				if (string.IsNullOrEmpty(Function.ToString(dataRow["EndWorkingHour"])))
				{
					dataRow["EndWorkingHour"] = 1700;
				}
				if (string.IsNullOrEmpty(Function.ToString(dataRow["SatWorking"])))
				{
					dataRow["SatWorking"] = "N";
				}
				if (string.IsNullOrEmpty(Function.ToString(dataRow["ResPerPage"])))
				{
					dataRow["ResPerPage"] = 3;
				}
				if (string.IsNullOrEmpty(Function.ToString(dataRow["TaskColor"])))
				{
					dataRow["TaskColor"] = -4980813;
				}
				if (string.IsNullOrEmpty(Function.ToString(dataRow["EventColor"])))
				{
					dataRow["EventColor"] = -14669;
				}
				if (string.IsNullOrEmpty(Function.ToString(dataRow["NoteColor"])))
				{
					dataRow["NoteColor"] = -77;
				}
				if (string.IsNullOrEmpty(Function.ToString(dataRow["NoSttColor"])))
				{
					dataRow["NoSttColor"] = -1;
				}
				if (string.IsNullOrEmpty(Function.ToString(dataRow["NotCompletedColor"])))
				{
					dataRow["NotCompletedColor"] = -3883625;
				}
				if (string.IsNullOrEmpty(Function.ToString(dataRow["CompletedColor"])))
				{
					dataRow["CompletedColor"] = -7156260;
				}
				if (string.IsNullOrEmpty(Function.ToString(dataRow["ShowToolTip"])))
				{
					dataRow["ShowToolTip"] = "S";
				}
				if (string.IsNullOrEmpty(Function.ToString(dataRow["ActiveViewType"])))
				{
					dataRow["ActiveViewType"] = 0;
				}
				if (string.IsNullOrEmpty(Function.ToString(dataRow["GroupType"])))
				{
					dataRow["GroupType"] = 2;
				}
				if (string.IsNullOrEmpty(Function.ToString(dataRow["ShowCompleted"])))
				{
					dataRow["ShowCompleted"] = "Y";
				}
				if (string.IsNullOrEmpty(Function.ToString(dataRow["ShowUncompleted"])))
				{
					dataRow["ShowUncompleted"] = "Y";
				}
				if (string.IsNullOrEmpty(Function.ToString(dataRow["ShowEvent"])))
				{
					dataRow["ShowEvent"] = "Y";
				}
				if (string.IsNullOrEmpty(Function.ToString(dataRow["ShowNote"])))
				{
					dataRow["ShowNote"] = "Y";
				}
				if (string.IsNullOrEmpty(Function.ToString(dataRow["ShowNoneResource"])))
				{
					dataRow["ShowNoneResource"] = "Y";
				}
				_dtGeneSettings.AcceptChanges();
				BindControls(_dtGeneSettings);
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void BindControls(DataTable dt)
	{
		try
		{
			if (!dt.IsNotNull())
			{
				return;
			}
			color_Task.DataBindings.Clear();
			color_Task.DataBindings.Add("EditValue", dt, "TaskColor", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			color_Event.DataBindings.Clear();
			color_Event.DataBindings.Add("EditValue", dt, "EventColor", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			color_Note.DataBindings.Clear();
			color_Note.DataBindings.Add("EditValue", dt, "NoteColor", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			color_NoStatus.DataBindings.Clear();
			color_NoStatus.DataBindings.Add("EditValue", dt, "NoSttColor", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			color_NotCompleted.DataBindings.Clear();
			color_NotCompleted.DataBindings.Add("EditValue", dt, "NotCompletedColor", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			color_Completed.DataBindings.Clear();
			color_Completed.DataBindings.Add("EditValue", dt, "CompletedColor", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			spin_ResPerPage.DataBindings.Clear();
			spin_ResPerPage.DataBindings.Add("EditValue", dt, "ResPerPage", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			hour_WorkingStart.DataBindings.Clear();
			hour_WorkingStart.DataBindings.Add("EditValue", dt, "StartWorkingHour", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			hour_WorkingEnd.DataBindings.Clear();
			hour_WorkingEnd.DataBindings.Add("EditValue", dt, "EndWorkingHour", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			chk_WorkSat.DataBindings.Clear();
			chk_WorkSat.DataBindings.Add("EditValue", dt, "SatWorking", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			chk_WorkSat.Refresh();
			hour_SatWStart.DataBindings.Clear();
			hour_SatWStart.DataBindings.Add("EditValue", dt, "SatStartWHour", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			hour_SatWEnd.DataBindings.Clear();
			hour_SatWEnd.DataBindings.Add("EditValue", dt, "SatEndWHour", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			luk_ActiveViewType.DataBindings.Clear();
			luk_ActiveViewType.DataBindings.Add("EditValue", dt, "ActiveViewType", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			luk_GroupType.DataBindings.Clear();
			luk_GroupType.DataBindings.Add("EditValue", dt, "GroupType", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			luk_Tooltip.DataBindings.Clear();
			luk_Tooltip.DataBindings.Add("EditValue", dt, "ShowToolTip", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			luk_Tooltip.Refresh();
			chk_Completed.DataBindings.Clear();
			chk_Completed.DataBindings.Add("EditValue", dt, "ShowCompleted", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			chk_Completed.Refresh();
			chk_UnCompleted.DataBindings.Clear();
			chk_UnCompleted.DataBindings.Add("EditValue", dt, "ShowUncompleted", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			chk_UnCompleted.Refresh();
			chk_Event.DataBindings.Clear();
			chk_Event.DataBindings.Add("EditValue", dt, "ShowEvent", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			chk_Event.Refresh();
			chk_Note.DataBindings.Clear();
			chk_Note.DataBindings.Add("EditValue", dt, "ShowNote", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			chk_Note.Refresh();
			chk_NoAssign.DataBindings.Clear();
			chk_NoAssign.DataBindings.Add("EditValue", dt, "ShowNoneResource", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			chk_NoAssign.Refresh();
			if (Function.ToString(dt.Rows[0]["SatWorking"]) == "Y")
			{
				if (string.IsNullOrEmpty(Function.ToString(hour_SatWStart.EditValue)))
				{
					hour_SatWStart.EditValue = 800;
				}
				if (string.IsNullOrEmpty(Function.ToString(hour_SatWEnd.EditValue)))
				{
					hour_SatWEnd.EditValue = 1700;
				}
				hour_SatWStart.Visible = true;
				hour_SatWEnd.Visible = true;
				lblSatWEnd.Visible = true;
				lblSatWStart.Visible = true;
			}
			else
			{
				hour_SatWStart.Visible = false;
				hour_SatWEnd.Visible = false;
				lblSatWEnd.Visible = false;
				lblSatWStart.Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadInitForm()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(int));
			dataTable.Columns.Add("Name", typeof(string));
			dataTable.Rows.Add(0, "None");
			dataTable.Rows.Add(1, "Date");
			dataTable.Rows.Add(2, "Resource");
			luk_GroupType.Properties.DataSource = dataTable;
			luk_GroupType.Properties.DisplayMember = "Name";
			luk_GroupType.Properties.ValueMember = "Code";
			luk_GroupType.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			luk_GroupType.Properties.ForceInitialize();
			luk_GroupType.Properties.PopulateColumns();
			luk_GroupType.Properties.Columns["Code"].Visible = false;
			luk_GroupType.Properties.DropDownRows = dataTable.Rows.Count;
			DataTable dataTable2 = dataTable.Clone();
			dataTable2.Rows.Add(0, "Day");
			dataTable2.Rows.Add(1, "Week");
			dataTable2.Rows.Add(2, "Month");
			dataTable2.Rows.Add(3, "WorkWeek");
			dataTable2.Rows.Add(4, "Timeline");
			dataTable2.Rows.Add(5, "Gantt");
			dataTable2.Rows.Add(6, "FullWeek");
			dataTable2.Rows.Add(7, "Agenda");
			luk_ActiveViewType.Properties.DataSource = dataTable2;
			luk_ActiveViewType.Properties.DisplayMember = "Name";
			luk_ActiveViewType.Properties.ValueMember = "Code";
			luk_ActiveViewType.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			luk_ActiveViewType.Properties.ForceInitialize();
			luk_ActiveViewType.Properties.PopulateColumns();
			luk_ActiveViewType.Properties.Columns["Code"].Visible = false;
			luk_ActiveViewType.Properties.DropDownRows = dataTable2.Rows.Count;
			DataTable dataTable3 = new DataTable();
			dataTable3.Columns.Add("Code", typeof(string));
			dataTable3.Columns.Add("Name", typeof(string));
			dataTable3.Rows.Add("S", "Standard");
			dataTable3.Rows.Add("A", "Always");
			dataTable3.Rows.Add("N", "Never");
			luk_Tooltip.Properties.DataSource = dataTable3;
			luk_Tooltip.Properties.DisplayMember = "Name";
			luk_Tooltip.Properties.ValueMember = "Code";
			luk_Tooltip.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			luk_Tooltip.Properties.ForceInitialize();
			luk_Tooltip.Properties.PopulateColumns();
			luk_Tooltip.Properties.Columns["Code"].Visible = false;
			luk_Tooltip.Properties.DropDownRows = dataTable3.Rows.Count;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> SaveData()
	{
		_ = 2;
		try
		{
			DataTable dataTable = grd_Doc.DataSource as DataTable;
			if (dataTable.IsNotNull() && _dtGeneSettings.IsNotNull())
			{
				DataRow dataRow = _dtGeneSettings.Rows[0];
				if (chk_WorkSat.CheckState != CheckState.Checked)
				{
					dataRow["SatStartWHour"] = DBNull.Value;
					dataRow["SatEndWHour"] = DBNull.Value;
				}
				if (Function.ToString(GlobalData.UserCodeLogin) == "manager")
				{
					dataRow["UserSign"] = "";
				}
				else
				{
					dataRow["UserSign"] = Function.ToString(GlobalData.UserCodeLogin);
				}
				_dtGeneSettings.AcceptChanges();
				DataTable source = dataTable.Copy();
				if (source.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["ResourceId"]))) != null)
				{
					WinCommons.Error(Enums.MessageCode.ResourceIdCannotBeEmpty, "");
					return "Error";
				}
				if ((from x in source.AsEnumerable()
					group x by Function.ToString(x["ResourceId"])).Any((IGrouping<string, DataRow> x) => x.Skip(1).Any()))
				{
					WinCommons.Error(Enums.MessageCode.ResourceIdCannotBeDuplicate, "");
					return "Error";
				}
				EnumerableRowCollection<DataRow> source2 = from t in dataTable.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["ResourceId"]))
					select t;
				if (source2.Any())
				{
					dataTable = source2.CopyToDataTable().Copy();
				}
				DataSet dataSet = new DataSet();
				dataSet.Tables.Add(dataTable.Copy());
				dataSet.Tables.Add(_dtGeneSettings.Copy());
				WinCommons._OpenProcessing();
				if (dataSet.IsNotNull())
				{
					string text = await _httpClient.CalendarSettingClient.SaveDoc(dataSet);
					if (string.IsNullOrEmpty(text))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						FormMode = Enums.FormViewModeType.View;
						REFRESH_DATA();
						if (RefreshCal != null && !((XtraForm)RefreshCal.Target).IsDisposed && _isGridChanged)
						{
							await RefreshCal();
						}
						if (RefreshSettings != null && !((XtraForm)RefreshSettings.Target).IsDisposed && _isControlChanged)
						{
							await RefreshSettings();
						}
						_isGridChanged = false;
						_isControlChanged = false;
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, text);
					}
				}
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
		return string.Empty;
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				LoadData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetEventControl(Control ctr)
	{
		foreach (Control control in ctr.Controls)
		{
			if (control is CheckEdit checkEdit)
			{
				checkEdit.CheckStateChanged += OnContentChanged;
			}
			else if (control is ComboBoxEdit comboBoxEdit)
			{
				comboBoxEdit.SelectedIndexChanged += OnContentChanged;
				comboBoxEdit.SelectedValueChanged += OnContentChanged;
				comboBoxEdit.EditValueChanged += OnContentChanged;
			}
			else if (control is LookUpEdit lookUpEdit)
			{
				lookUpEdit.EditValueChanged += OnContentChanged;
				lookUpEdit.Properties.ShowHeader = false;
				lookUpEdit.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			}
			else if (control is DateEdit dateEdit)
			{
				dateEdit.EditValueChanged += OnContentChanged;
			}
			else if (control is TextEdit || control is MemoEdit)
			{
				control.TextChanged += OnContentChanged;
			}
			else if (control is SeparatorComboBox)
			{
				((SeparatorComboBox)control).SelectedIndexChanged += OnContentChanged;
			}
			else if (control is RadioButton)
			{
				((RadioButton)control).CheckedChanged += OnContentChanged;
			}
			else
			{
				SetEventControl(control);
			}
		}
	}

	protected void OnContentChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isChanging && FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Update;
				_isControlChanged = true;
			}
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_CalendarSetting));
		this.rep_Deparment = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.visibleAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.visibleNoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemColorPickEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemColorPickEdit();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gc_Type = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lb_Remarks = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.color_Task = new Apzon.Controls.Editor.ColorPickEditEx();
		this.spin_ResPerPage = new Apzon.Controls.Editor.SpinEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.hour_WorkingEnd = new Apzon.Controls.Controls.HourEdit();
		this.hour_WorkingStart = new Apzon.Controls.Controls.HourEdit();
		this.hour_SatWStart = new Apzon.Controls.Controls.HourEdit();
		this.lblSatWStart = new Apzon.Controls.Controls.LabelLine();
		this.lblSatWEnd = new Apzon.Controls.Controls.LabelLine();
		this.chk_WorkSat = new Apzon.Controls.Editor.CheckEditEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.luk_Tooltip = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.labelLine11 = new Apzon.Controls.Controls.LabelLine();
		this.chk_NoAssign = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_Completed = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_UnCompleted = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_Event = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_Note = new Apzon.Controls.Editor.CheckEditEx();
		this.luk_GroupType = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_ActiveViewType = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.labelLine10 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelControl3 = new Apzon.Controls.Controls.LabelControlEx();
		this.labelControl2 = new Apzon.Controls.Controls.LabelControlEx();
		this.labelControl1 = new Apzon.Controls.Controls.LabelControlEx();
		this.color_Completed = new Apzon.Controls.Editor.ColorPickEditEx();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.color_NotCompleted = new Apzon.Controls.Editor.ColorPickEditEx();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.color_NoStatus = new Apzon.Controls.Editor.ColorPickEditEx();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.hour_SatWEnd = new Apzon.Controls.Controls.HourEdit();
		this.color_Note = new Apzon.Controls.Editor.ColorPickEditEx();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.color_Event = new Apzon.Controls.Editor.ColorPickEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.rep_Deparment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemColorPickEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.color_Task.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.spin_ResPerPage.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hour_WorkingEnd.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hour_WorkingStart.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hour_SatWStart.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_WorkSat.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_Tooltip.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_NoAssign.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Completed.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_UnCompleted.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Event.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Note.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_GroupType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ActiveViewType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.color_Completed.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.color_NotCompleted.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.color_NoStatus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hour_SatWEnd.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.color_Note.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.color_Event.Properties).BeginInit();
		base.SuspendLayout();
		this.rep_Deparment.AutoHeight = false;
		this.rep_Deparment.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Deparment.Name = "rep_Deparment";
		this.rep_Deparment.NullText = "";
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(100, 419);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 23;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repositoryItemMemoExEdit1, this.repositoryItemColorPickEdit1, this.repositoryItemCheckEdit1 });
		this.grd_Doc.Size = new System.Drawing.Size(643, 166);
		this.grd_Doc.TabIndex = 21;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.visibleAllToolStripMenuItem, this.visibleNoneToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(141, 48);
		this.visibleAllToolStripMenuItem.Name = "visibleAllToolStripMenuItem";
		this.visibleAllToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
		this.visibleAllToolStripMenuItem.Text = "Visible All";
		this.visibleAllToolStripMenuItem.Click += new System.EventHandler(this.visibleAllToolStripMenuItem_Click);
		this.visibleNoneToolStripMenuItem.Name = "visibleNoneToolStripMenuItem";
		this.visibleNoneToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
		this.visibleNoneToolStripMenuItem.Text = "Visible None";
		this.visibleNoneToolStripMenuItem.Click += new System.EventHandler(this.visibleNoneToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx5, this.gridColumnEx4, this.gc_Type, this.gridColumnEx6 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oCalendarSetting";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.GroupCount = 1;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 30;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_OCLS";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[2]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gc_Type, DevExpress.Data.ColumnSortOrder.Descending),
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumnEx2, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gv_Doc_CustomUnboundColumnData);
		this.gv_Doc.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_Doc_ValidatingEditor);
		this.gv_Doc.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gv_Doc_InvalidValueException);
		this.gridColumnEx1.Caption = "ResourceId";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "ResourceId";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx2.Caption = "Resource Name";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "ResName";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.OptionsColumn.ReadOnly = true;
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 0;
		this.gridColumnEx3.Caption = "Header's Color";
		this.gridColumnEx3.ColumnEdit = this.repositoryItemColorPickEdit1;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "Color";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 1;
		this.repositoryItemColorPickEdit1.AutoHeight = false;
		this.repositoryItemColorPickEdit1.AutomaticColor = System.Drawing.Color.Black;
		this.repositoryItemColorPickEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemColorPickEdit1.Name = "repositoryItemColorPickEdit1";
		this.repositoryItemColorPickEdit1.StoreColorAsInteger = true;
		this.gridColumnEx5.Caption = "Visible";
		this.gridColumnEx5.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "Checked";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 2;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.gridColumnEx4.Caption = "Remarks";
		this.gridColumnEx4.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "Remarks";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 3;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.gc_Type.Caption = "Type";
		this.gc_Type.CurrencyDateField = null;
		this.gc_Type.CurrencyField = null;
		this.gc_Type.CurrencyFixed = null;
		this.gc_Type.CurrencyFixedRate = 0.0;
		this.gc_Type.CurrencyRateField = null;
		this.gc_Type.FieldName = "Type";
		this.gc_Type.Name = "gc_Type";
		this.gc_Type.OptionsColumn.ReadOnly = true;
		this.gc_Type.PrimaryKey = false;
		this.gc_Type.UnboundType = DevExpress.Data.UnboundColumnType.String;
		this.gc_Type.UnitField = null;
		this.gc_Type.Visible = true;
		this.gc_Type.VisibleIndex = 4;
		this.gridColumnEx6.Caption = "Cell Color";
		this.gridColumnEx6.ColumnEdit = this.repositoryItemColorPickEdit1;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "CellColor";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(4, 419);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(93, 20);
		this.btn_Save.TabIndex = 22;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.lb_Remarks.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_Remarks.Appearance.Options.UseFont = true;
		this.lb_Remarks.EditControl = null;
		this.lb_Remarks.EditValue = "Task";
		this.lb_Remarks.FieldName = null;
		this.lb_Remarks.HideWhenEmpty = false;
		this.lb_Remarks.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Remarks.Location = new System.Drawing.Point(236, 132);
		this.lb_Remarks.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_Remarks.Name = "lb_Remarks";
		this.lb_Remarks.ReadOnly = false;
		this.lb_Remarks.SettingInVisible = false;
		this.lb_Remarks.SettingReadonly = false;
		this.lb_Remarks.ShowUnderLine = true;
		this.lb_Remarks.Size = new System.Drawing.Size(71, 19);
		this.lb_Remarks.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Remarks.TabIndex = 106;
		this.lb_Remarks.Utext = "Task";
		this.labelLine3.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine3.Appearance.Options.UseFont = true;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Resources Per Page";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(236, 23);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(104, 19);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 109;
		this.labelLine3.Utext = "Resources Per Page";
		this.color_Task.EditControl = null;
		this.color_Task.EditValue = System.Drawing.Color.Empty;
		this.color_Task.FieldName = "TaskColor";
		this.color_Task.IsSearchControl = false;
		this.color_Task.Location = new System.Drawing.Point(301, 129);
		this.color_Task.Name = "color_Task";
		this.color_Task.Properties.AutomaticColor = System.Drawing.Color.Black;
		this.color_Task.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.color_Task.Properties.StoreColorAsInteger = true;
		this.color_Task.SettingInVisible = false;
		this.color_Task.SettingReadonly = false;
		this.color_Task.Size = new System.Drawing.Size(120, 20);
		this.color_Task.TabIndex = 13;
		this.spin_ResPerPage.EditControl = null;
		this.spin_ResPerPage.EditValue = new decimal(new int[4] { 1, 0, 0, 0 });
		this.spin_ResPerPage.FieldName = "ResPerPage";
		this.spin_ResPerPage.IsSearchControl = false;
		this.spin_ResPerPage.Location = new System.Drawing.Point(337, 20);
		this.spin_ResPerPage.Name = "spin_ResPerPage";
		this.spin_ResPerPage.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.spin_ResPerPage.Properties.MaxValue = new decimal(new int[4] { 100, 0, 0, 0 });
		this.spin_ResPerPage.Properties.MinValue = new decimal(new int[4] { 1, 0, 0, 0 });
		this.spin_ResPerPage.SettingInVisible = false;
		this.spin_ResPerPage.SettingReadonly = false;
		this.spin_ResPerPage.Size = new System.Drawing.Size(50, 20);
		this.spin_ResPerPage.TabIndex = 4;
		this.labelLine4.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine4.Appearance.Options.UseFont = true;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Start Working Hour";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(236, 53);
		this.labelLine4.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(104, 19);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 113;
		this.labelLine4.Utext = "Start Working Hour";
		this.labelLine5.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine5.Appearance.Options.UseFont = true;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "End Working Hour";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(234, 81);
		this.labelLine5.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(104, 19);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 114;
		this.labelLine5.Utext = "End Working Hour";
		this.hour_WorkingEnd.EditControl = null;
		this.hour_WorkingEnd.FieldName = "EndWorkingHour";
		this.hour_WorkingEnd.IsSearchControl = false;
		this.hour_WorkingEnd.Location = new System.Drawing.Point(337, 78);
		this.hour_WorkingEnd.Name = "hour_WorkingEnd";
		this.hour_WorkingEnd.Properties.IsSearchControl = false;
		this.hour_WorkingEnd.Properties.MaxLength = 5;
		this.hour_WorkingEnd.SettingInVisible = false;
		this.hour_WorkingEnd.SettingReadonly = false;
		this.hour_WorkingEnd.Size = new System.Drawing.Size(50, 20);
		this.hour_WorkingEnd.TabIndex = 6;
		this.hour_WorkingEnd.Validating += new System.ComponentModel.CancelEventHandler(this.hour_WorkingEnd_Validating);
		this.hour_WorkingStart.EditControl = null;
		this.hour_WorkingStart.FieldName = "StartWorkingHour";
		this.hour_WorkingStart.IsSearchControl = false;
		this.hour_WorkingStart.Location = new System.Drawing.Point(337, 50);
		this.hour_WorkingStart.Name = "hour_WorkingStart";
		this.hour_WorkingStart.Properties.IsSearchControl = false;
		this.hour_WorkingStart.Properties.MaxLength = 5;
		this.hour_WorkingStart.SettingInVisible = false;
		this.hour_WorkingStart.SettingReadonly = false;
		this.hour_WorkingStart.Size = new System.Drawing.Size(50, 20);
		this.hour_WorkingStart.TabIndex = 5;
		this.hour_WorkingStart.Validating += new System.ComponentModel.CancelEventHandler(this.hour_WorkingStart_Validating);
		this.hour_SatWStart.EditControl = null;
		this.hour_SatWStart.FieldName = "SatStartWHour";
		this.hour_SatWStart.IsSearchControl = false;
		this.hour_SatWStart.Location = new System.Drawing.Point(488, 48);
		this.hour_SatWStart.Name = "hour_SatWStart";
		this.hour_SatWStart.Properties.IsSearchControl = false;
		this.hour_SatWStart.Properties.MaxLength = 5;
		this.hour_SatWStart.SettingInVisible = false;
		this.hour_SatWStart.SettingReadonly = false;
		this.hour_SatWStart.Size = new System.Drawing.Size(50, 20);
		this.hour_SatWStart.TabIndex = 8;
		this.hour_SatWStart.Validating += new System.ComponentModel.CancelEventHandler(this.hour_SatWStart_Validating);
		this.lblSatWStart.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblSatWStart.Appearance.Options.UseFont = true;
		this.lblSatWStart.EditControl = null;
		this.lblSatWStart.EditValue = "Start";
		this.lblSatWStart.FieldName = null;
		this.lblSatWStart.HideWhenEmpty = false;
		this.lblSatWStart.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lblSatWStart.Location = new System.Drawing.Point(449, 51);
		this.lblSatWStart.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lblSatWStart.Name = "lblSatWStart";
		this.lblSatWStart.ReadOnly = false;
		this.lblSatWStart.SettingInVisible = false;
		this.lblSatWStart.SettingReadonly = false;
		this.lblSatWStart.ShowUnderLine = true;
		this.lblSatWStart.Size = new System.Drawing.Size(43, 19);
		this.lblSatWStart.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lblSatWStart.TabIndex = 119;
		this.lblSatWStart.Utext = "Start";
		this.lblSatWEnd.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lblSatWEnd.Appearance.Options.UseFont = true;
		this.lblSatWEnd.EditControl = null;
		this.lblSatWEnd.EditValue = "End";
		this.lblSatWEnd.FieldName = null;
		this.lblSatWEnd.HideWhenEmpty = false;
		this.lblSatWEnd.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lblSatWEnd.Location = new System.Drawing.Point(449, 76);
		this.lblSatWEnd.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lblSatWEnd.Name = "lblSatWEnd";
		this.lblSatWEnd.ReadOnly = false;
		this.lblSatWEnd.SettingInVisible = false;
		this.lblSatWEnd.SettingReadonly = false;
		this.lblSatWEnd.ShowUnderLine = true;
		this.lblSatWEnd.Size = new System.Drawing.Size(43, 19);
		this.lblSatWEnd.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lblSatWEnd.TabIndex = 120;
		this.lblSatWEnd.Utext = "End";
		this.chk_WorkSat.EditControl = null;
		this.chk_WorkSat.FieldName = "SatWorking";
		this.chk_WorkSat.IsSearchControl = false;
		this.chk_WorkSat.Location = new System.Drawing.Point(428, 20);
		this.chk_WorkSat.Name = "chk_WorkSat";
		this.chk_WorkSat.Properties.Caption = "Work on Saturday";
		this.chk_WorkSat.Properties.ValueChecked = "Y";
		this.chk_WorkSat.Properties.ValueUnchecked = "N";
		this.chk_WorkSat.SettingInVisible = false;
		this.chk_WorkSat.SettingReadonly = false;
		this.chk_WorkSat.Size = new System.Drawing.Size(114, 20);
		this.chk_WorkSat.TabIndex = 7;
		this.chk_WorkSat.CheckStateChanged += new System.EventHandler(this.chk_WorkSat_CheckStateChanged);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel1;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.IsSplitterFixed = true;
		this.splitContainerControl1.Location = new System.Drawing.Point(4, 2);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.luk_Tooltip);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine11);
		this.splitContainerControl1.Panel1.Controls.Add(this.chk_NoAssign);
		this.splitContainerControl1.Panel1.Controls.Add(this.chk_Completed);
		this.splitContainerControl1.Panel1.Controls.Add(this.chk_UnCompleted);
		this.splitContainerControl1.Panel1.Controls.Add(this.chk_Event);
		this.splitContainerControl1.Panel1.Controls.Add(this.chk_Note);
		this.splitContainerControl1.Panel1.Controls.Add(this.luk_GroupType);
		this.splitContainerControl1.Panel1.Controls.Add(this.luk_ActiveViewType);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine10);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine1);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelControl3);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelControl2);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelControl1);
		this.splitContainerControl1.Panel1.Controls.Add(this.color_Completed);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine6);
		this.splitContainerControl1.Panel1.Controls.Add(this.color_NotCompleted);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine7);
		this.splitContainerControl1.Panel1.Controls.Add(this.color_NoStatus);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine9);
		this.splitContainerControl1.Panel1.Controls.Add(this.hour_SatWEnd);
		this.splitContainerControl1.Panel1.Controls.Add(this.hour_SatWStart);
		this.splitContainerControl1.Panel1.Controls.Add(this.hour_WorkingStart);
		this.splitContainerControl1.Panel1.Controls.Add(this.color_Note);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine8);
		this.splitContainerControl1.Panel1.Controls.Add(this.color_Event);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine2);
		this.splitContainerControl1.Panel1.Controls.Add(this.color_Task);
		this.splitContainerControl1.Panel1.Controls.Add(this.spin_ResPerPage);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine4);
		this.splitContainerControl1.Panel1.Controls.Add(this.chk_WorkSat);
		this.splitContainerControl1.Panel1.Controls.Add(this.lblSatWEnd);
		this.splitContainerControl1.Panel1.Controls.Add(this.lb_Remarks);
		this.splitContainerControl1.Panel1.Controls.Add(this.lblSatWStart);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine3);
		this.splitContainerControl1.Panel1.Controls.Add(this.hour_WorkingEnd);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine5);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(643, 410);
		this.splitContainerControl1.SplitterPosition = 234;
		this.splitContainerControl1.TabIndex = 122;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.luk_Tooltip.EditControl = null;
		this.luk_Tooltip.FieldName = "ShowToolTip";
		this.luk_Tooltip.IsSearchControl = false;
		this.luk_Tooltip.Location = new System.Drawing.Point(95, 78);
		this.luk_Tooltip.Name = "luk_Tooltip";
		this.luk_Tooltip.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Tooltip.Properties.IsSearchControl = false;
		this.luk_Tooltip.Properties.NullText = "";
		this.luk_Tooltip.Properties.PopupSizeable = false;
		this.luk_Tooltip.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.luk_Tooltip.SettingInVisible = false;
		this.luk_Tooltip.SettingReadonly = false;
		this.luk_Tooltip.Size = new System.Drawing.Size(120, 20);
		this.luk_Tooltip.TabIndex = 3;
		this.labelLine11.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine11.Appearance.Options.UseFont = true;
		this.labelLine11.EditControl = null;
		this.labelLine11.EditValue = "Show Tooltip";
		this.labelLine11.FieldName = null;
		this.labelLine11.HideWhenEmpty = false;
		this.labelLine11.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine11.Location = new System.Drawing.Point(12, 81);
		this.labelLine11.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine11.Name = "labelLine11";
		this.labelLine11.ReadOnly = false;
		this.labelLine11.SettingInVisible = false;
		this.labelLine11.SettingReadonly = false;
		this.labelLine11.ShowUnderLine = true;
		this.labelLine11.Size = new System.Drawing.Size(89, 19);
		this.labelLine11.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine11.TabIndex = 151;
		this.labelLine11.Utext = "Show Tooltip";
		this.chk_NoAssign.EditControl = null;
		this.chk_NoAssign.EditValue = "Y";
		this.chk_NoAssign.FieldName = "ShowNoneResource";
		this.chk_NoAssign.IsSearchControl = false;
		this.chk_NoAssign.Location = new System.Drawing.Point(449, 213);
		this.chk_NoAssign.Name = "chk_NoAssign";
		this.chk_NoAssign.Properties.Caption = "Show No Assignees";
		this.chk_NoAssign.Properties.ValueChecked = "Y";
		this.chk_NoAssign.Properties.ValueUnchecked = "N";
		this.chk_NoAssign.SettingInVisible = false;
		this.chk_NoAssign.SettingReadonly = false;
		this.chk_NoAssign.Size = new System.Drawing.Size(139, 20);
		this.chk_NoAssign.TabIndex = 20;
		this.chk_NoAssign.Visible = false;
		this.chk_Completed.EditControl = null;
		this.chk_Completed.EditValue = "Y";
		this.chk_Completed.FieldName = "ShowCompleted";
		this.chk_Completed.IsSearchControl = false;
		this.chk_Completed.Location = new System.Drawing.Point(449, 101);
		this.chk_Completed.Name = "chk_Completed";
		this.chk_Completed.Properties.Caption = "Show Completed Task";
		this.chk_Completed.Properties.ValueChecked = "Y";
		this.chk_Completed.Properties.ValueUnchecked = "N";
		this.chk_Completed.SettingInVisible = false;
		this.chk_Completed.SettingReadonly = false;
		this.chk_Completed.Size = new System.Drawing.Size(139, 20);
		this.chk_Completed.TabIndex = 16;
		this.chk_UnCompleted.EditControl = null;
		this.chk_UnCompleted.EditValue = "Y";
		this.chk_UnCompleted.FieldName = "ShowUncompleted";
		this.chk_UnCompleted.IsSearchControl = false;
		this.chk_UnCompleted.Location = new System.Drawing.Point(449, 131);
		this.chk_UnCompleted.Name = "chk_UnCompleted";
		this.chk_UnCompleted.Properties.Caption = "Show Uncompleted Task";
		this.chk_UnCompleted.Properties.ValueChecked = "Y";
		this.chk_UnCompleted.Properties.ValueUnchecked = "N";
		this.chk_UnCompleted.SettingInVisible = false;
		this.chk_UnCompleted.SettingReadonly = false;
		this.chk_UnCompleted.Size = new System.Drawing.Size(139, 20);
		this.chk_UnCompleted.TabIndex = 17;
		this.chk_Event.EditControl = null;
		this.chk_Event.EditValue = "Y";
		this.chk_Event.FieldName = "ShowEvent";
		this.chk_Event.IsSearchControl = false;
		this.chk_Event.Location = new System.Drawing.Point(449, 158);
		this.chk_Event.Name = "chk_Event";
		this.chk_Event.Properties.Caption = "Show Events";
		this.chk_Event.Properties.ValueChecked = "Y";
		this.chk_Event.Properties.ValueUnchecked = "N";
		this.chk_Event.SettingInVisible = false;
		this.chk_Event.SettingReadonly = false;
		this.chk_Event.Size = new System.Drawing.Size(139, 20);
		this.chk_Event.TabIndex = 18;
		this.chk_Note.EditControl = null;
		this.chk_Note.EditValue = "Y";
		this.chk_Note.FieldName = "ShowNote";
		this.chk_Note.IsSearchControl = false;
		this.chk_Note.Location = new System.Drawing.Point(449, 186);
		this.chk_Note.Name = "chk_Note";
		this.chk_Note.Properties.Caption = "Show Notes";
		this.chk_Note.Properties.ValueChecked = "Y";
		this.chk_Note.Properties.ValueUnchecked = "N";
		this.chk_Note.SettingInVisible = false;
		this.chk_Note.SettingReadonly = false;
		this.chk_Note.Size = new System.Drawing.Size(139, 20);
		this.chk_Note.TabIndex = 19;
		this.luk_GroupType.EditControl = null;
		this.luk_GroupType.FieldName = "GroupType";
		this.luk_GroupType.IsSearchControl = false;
		this.luk_GroupType.Location = new System.Drawing.Point(95, 20);
		this.luk_GroupType.Name = "luk_GroupType";
		this.luk_GroupType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_GroupType.Properties.IsSearchControl = false;
		this.luk_GroupType.Properties.NullText = "";
		this.luk_GroupType.Properties.PopupSizeable = false;
		this.luk_GroupType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.luk_GroupType.SettingInVisible = false;
		this.luk_GroupType.SettingReadonly = false;
		this.luk_GroupType.Size = new System.Drawing.Size(120, 20);
		this.luk_GroupType.TabIndex = 1;
		this.luk_ActiveViewType.EditControl = null;
		this.luk_ActiveViewType.FieldName = "ActiveViewType";
		this.luk_ActiveViewType.IsSearchControl = false;
		this.luk_ActiveViewType.Location = new System.Drawing.Point(95, 50);
		this.luk_ActiveViewType.Name = "luk_ActiveViewType";
		this.luk_ActiveViewType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_ActiveViewType.Properties.IsSearchControl = false;
		this.luk_ActiveViewType.Properties.NullText = "";
		this.luk_ActiveViewType.Properties.PopupSizeable = false;
		this.luk_ActiveViewType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.luk_ActiveViewType.SettingInVisible = false;
		this.luk_ActiveViewType.SettingReadonly = false;
		this.luk_ActiveViewType.Size = new System.Drawing.Size(120, 20);
		this.luk_ActiveViewType.TabIndex = 2;
		this.labelLine10.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine10.Appearance.Options.UseFont = true;
		this.labelLine10.EditControl = null;
		this.labelLine10.EditValue = "Active View";
		this.labelLine10.FieldName = null;
		this.labelLine10.HideWhenEmpty = false;
		this.labelLine10.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine10.Location = new System.Drawing.Point(12, 53);
		this.labelLine10.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine10.Name = "labelLine10";
		this.labelLine10.ReadOnly = false;
		this.labelLine10.SettingInVisible = false;
		this.labelLine10.SettingReadonly = false;
		this.labelLine10.ShowUnderLine = true;
		this.labelLine10.Size = new System.Drawing.Size(89, 19);
		this.labelLine10.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine10.TabIndex = 143;
		this.labelLine10.Utext = "Active View";
		this.labelLine1.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine1.Appearance.Options.UseFont = true;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Group By";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(12, 23);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(89, 19);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 142;
		this.labelLine1.Utext = "Group By";
		this.labelControl3.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControl3.Appearance.Options.UseFont = true;
		this.labelControl3.EditControl = null;
		this.labelControl3.EditValue = "General";
		this.labelControl3.FieldName = null;
		this.labelControl3.Location = new System.Drawing.Point(11, 1);
		this.labelControl3.Name = "labelControl3";
		this.labelControl3.ReadOnly = false;
		this.labelControl3.SettingInVisible = false;
		this.labelControl3.SettingReadonly = false;
		this.labelControl3.Size = new System.Drawing.Size(44, 13);
		this.labelControl3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControl3.TabIndex = 138;
		this.labelControl3.Text = "General";
		this.labelControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControl2.Appearance.Options.UseFont = true;
		this.labelControl2.EditControl = null;
		this.labelControl2.EditValue = "Status Color";
		this.labelControl2.FieldName = null;
		this.labelControl2.Location = new System.Drawing.Point(12, 104);
		this.labelControl2.Name = "labelControl2";
		this.labelControl2.ReadOnly = false;
		this.labelControl2.SettingInVisible = false;
		this.labelControl2.SettingReadonly = false;
		this.labelControl2.Size = new System.Drawing.Size(69, 13);
		this.labelControl2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControl2.TabIndex = 137;
		this.labelControl2.Text = "Status Color";
		this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControl1.Appearance.Options.UseFont = true;
		this.labelControl1.EditControl = null;
		this.labelControl1.EditValue = "Action Color";
		this.labelControl1.FieldName = null;
		this.labelControl1.Location = new System.Drawing.Point(236, 104);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.ReadOnly = false;
		this.labelControl1.SettingInVisible = false;
		this.labelControl1.SettingReadonly = false;
		this.labelControl1.Size = new System.Drawing.Size(68, 13);
		this.labelControl1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControl1.TabIndex = 136;
		this.labelControl1.Text = "Action Color";
		this.color_Completed.EditControl = null;
		this.color_Completed.EditValue = System.Drawing.Color.Empty;
		this.color_Completed.FieldName = "CompletedColor";
		this.color_Completed.IsSearchControl = false;
		this.color_Completed.Location = new System.Drawing.Point(95, 185);
		this.color_Completed.Name = "color_Completed";
		this.color_Completed.Properties.AutomaticColor = System.Drawing.Color.Black;
		this.color_Completed.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.color_Completed.Properties.StoreColorAsInteger = true;
		this.color_Completed.SettingInVisible = false;
		this.color_Completed.SettingReadonly = false;
		this.color_Completed.Size = new System.Drawing.Size(120, 20);
		this.color_Completed.TabIndex = 12;
		this.labelLine6.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine6.Appearance.Options.UseFont = true;
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Completed";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(12, 188);
		this.labelLine6.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(89, 19);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 133;
		this.labelLine6.Utext = "Completed";
		this.color_NotCompleted.EditControl = null;
		this.color_NotCompleted.EditValue = System.Drawing.Color.Empty;
		this.color_NotCompleted.FieldName = "NotCompletedColor";
		this.color_NotCompleted.IsSearchControl = false;
		this.color_NotCompleted.Location = new System.Drawing.Point(95, 157);
		this.color_NotCompleted.Name = "color_NotCompleted";
		this.color_NotCompleted.Properties.AutomaticColor = System.Drawing.Color.Black;
		this.color_NotCompleted.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.color_NotCompleted.Properties.StoreColorAsInteger = true;
		this.color_NotCompleted.SettingInVisible = false;
		this.color_NotCompleted.SettingReadonly = false;
		this.color_NotCompleted.Size = new System.Drawing.Size(120, 20);
		this.color_NotCompleted.TabIndex = 11;
		this.labelLine7.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine7.Appearance.Options.UseFont = true;
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "Uncompleted";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(12, 160);
		this.labelLine7.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(89, 19);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 132;
		this.labelLine7.Utext = "Uncompleted";
		this.color_NoStatus.EditControl = null;
		this.color_NoStatus.EditValue = System.Drawing.Color.Empty;
		this.color_NoStatus.FieldName = "NoSttColor";
		this.color_NoStatus.IsSearchControl = false;
		this.color_NoStatus.Location = new System.Drawing.Point(95, 129);
		this.color_NoStatus.Name = "color_NoStatus";
		this.color_NoStatus.Properties.AutomaticColor = System.Drawing.Color.Black;
		this.color_NoStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.color_NoStatus.Properties.StoreColorAsInteger = true;
		this.color_NoStatus.SettingInVisible = false;
		this.color_NoStatus.SettingReadonly = false;
		this.color_NoStatus.Size = new System.Drawing.Size(120, 20);
		this.color_NoStatus.TabIndex = 10;
		this.labelLine9.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine9.Appearance.Options.UseFont = true;
		this.labelLine9.EditControl = null;
		this.labelLine9.EditValue = "No status ";
		this.labelLine9.FieldName = null;
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(12, 132);
		this.labelLine9.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ReadOnly = false;
		this.labelLine9.SettingInVisible = false;
		this.labelLine9.SettingReadonly = false;
		this.labelLine9.ShowUnderLine = true;
		this.labelLine9.Size = new System.Drawing.Size(89, 19);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 131;
		this.labelLine9.Utext = "No status ";
		this.hour_SatWEnd.EditControl = null;
		this.hour_SatWEnd.FieldName = "SatEndWHour";
		this.hour_SatWEnd.IsSearchControl = false;
		this.hour_SatWEnd.Location = new System.Drawing.Point(488, 73);
		this.hour_SatWEnd.Name = "hour_SatWEnd";
		this.hour_SatWEnd.Properties.IsSearchControl = false;
		this.hour_SatWEnd.Properties.MaxLength = 5;
		this.hour_SatWEnd.SettingInVisible = false;
		this.hour_SatWEnd.SettingReadonly = false;
		this.hour_SatWEnd.Size = new System.Drawing.Size(50, 20);
		this.hour_SatWEnd.TabIndex = 9;
		this.hour_SatWEnd.Validating += new System.ComponentModel.CancelEventHandler(this.hour_SatWEnd_Validating);
		this.color_Note.EditControl = null;
		this.color_Note.EditValue = System.Drawing.Color.Empty;
		this.color_Note.FieldName = "NoteColor";
		this.color_Note.IsSearchControl = false;
		this.color_Note.Location = new System.Drawing.Point(301, 185);
		this.color_Note.Name = "color_Note";
		this.color_Note.Properties.AutomaticColor = System.Drawing.Color.Black;
		this.color_Note.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.color_Note.Properties.StoreColorAsInteger = true;
		this.color_Note.SettingInVisible = false;
		this.color_Note.SettingReadonly = false;
		this.color_Note.Size = new System.Drawing.Size(120, 20);
		this.color_Note.TabIndex = 15;
		this.labelLine8.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine8.Appearance.Options.UseFont = true;
		this.labelLine8.EditControl = null;
		this.labelLine8.EditValue = "Note";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(236, 188);
		this.labelLine8.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ReadOnly = false;
		this.labelLine8.SettingInVisible = false;
		this.labelLine8.SettingReadonly = false;
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(71, 19);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 124;
		this.labelLine8.Utext = "Note";
		this.color_Event.EditControl = null;
		this.color_Event.EditValue = System.Drawing.Color.Empty;
		this.color_Event.FieldName = "EventColor";
		this.color_Event.IsSearchControl = false;
		this.color_Event.Location = new System.Drawing.Point(301, 157);
		this.color_Event.Name = "color_Event";
		this.color_Event.Properties.AutomaticColor = System.Drawing.Color.Black;
		this.color_Event.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.color_Event.Properties.StoreColorAsInteger = true;
		this.color_Event.SettingInVisible = false;
		this.color_Event.SettingReadonly = false;
		this.color_Event.Size = new System.Drawing.Size(120, 20);
		this.color_Event.TabIndex = 14;
		this.labelLine2.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine2.Appearance.Options.UseFont = true;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Event";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(236, 160);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(71, 19);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 122;
		this.labelLine2.Utext = "Event";
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(647, 444);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.FormId = "oCalendarSetting";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_CalendarSetting";
		base.ShowInTaskbar = false;
		this.Text = "Calendar Setting";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_CalendarSetting_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.rep_Deparment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemColorPickEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.color_Task.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.spin_ResPerPage.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hour_WorkingEnd.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hour_WorkingStart.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hour_SatWStart.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_WorkSat.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.luk_Tooltip.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_NoAssign.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Completed.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_UnCompleted.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Event.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Note.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_GroupType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ActiveViewType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.color_Completed.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.color_NotCompleted.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.color_NoStatus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hour_SatWEnd.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.color_Note.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.color_Event.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
