using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.SalesOppotunity;
using Apzon.Forms.Service;
using Apzon.Forms.UserControls;
using Apzon.Forms.UserControls.FilterControl;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Localization;
using DevExpress.XtraScheduler.UI;
using DevExpress.XtraScheduler.Xml;
using DevExpress.XtraTab;

namespace Apzon.Forms.Administrator;

public class frm_ActivityCalendar : XtraFormEx
{
	public class MyLocalizer : SchedulerLocalizer
	{
		public override string GetLocalizedString(SchedulerStringId id)
		{
			try
			{
				return id switch
				{
					SchedulerStringId.Caption_PrevAppointment => "Previous Activity", 
					SchedulerStringId.Caption_NextAppointment => "Next Activity", 
					_ => base.GetLocalizedString(id), 
				};
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.ErrorLineNumber(ex);
				return string.Empty;
			}
		}
	}

	private enum ActivityType
	{
		Task,
		Event,
		Note
	}

	public class CalendarAction
	{
		private readonly string _actionCode;

		private readonly string _actionName;

		public string Action => _actionCode;

		public string ActionName => _actionName;

		public bool? IsChecked { get; set; }

		public CalendarAction(string strActionCode, string strActionName, bool? boolIsSelected)
		{
			_actionCode = strActionCode;
			_actionName = strActionName;
			IsChecked = boolIsSelected;
		}
	}

	private enum FormModeType
	{
		Normal,
		SingleView,
		SingleUpdate,
		SingleAdd
	}

	private enum TaskFilterType
	{
		None,
		Overdue,
		Today,
		ThisWeek,
		LastWeek,
		Users,
		Employees,
		ProjectNew,
		Unassigned
	}

	private enum SaveType
	{
		GridAdd,
		ValidateControl,
		UDF,
		ChangeStatus,
		ChangePosition,
		SingleAdd,
		SingleUpdate,
		Attachment,
		SubAdd,
		Copy
	}

	private enum DueDateOptions
	{
		Recalculate,
		RecalculateKeepStartTime,
		KeepDueDate,
		ResetDueDate,
		GetFromRow
	}

	private readonly List<string> listMapped = new List<string>
	{
		"startDateTime", "endDateTime", "LocationCode", "status", "Action", "Details", "ResourceIdsInfo", "Notes", "RecurrenceInfo", "CntctType",
		"AllDayEvent", "AssignedBy", "ClgCode", "Closed"
	};

	private static readonly Dictionary<string, Color> DicAction = new Dictionary<string, Color>
	{
		{
			"T",
			Color.FromArgb(255, 198, 179)
		},
		{
			"E",
			Color.FromArgb(255, 255, 179)
		},
		{
			"V",
			Color.FromArgb(179, 255, 179)
		}
	};

	private static readonly Dictionary<string, string> DicActionName = new Dictionary<string, string>
	{
		{ "T", "Task" },
		{ "E", "Note" },
		{ "V", "Event" }
	};

	private readonly List<string> _statusList = new List<string>();

	private readonly List<int> _statusIdList = new List<int>();

	private readonly List<Brush> _sttColorList = new List<Brush>();

	private bool _isChanging;

	private bool _isSatWorking;

	private bool _formLoading;

	private bool copyTask;

	private bool addMainTask;

	private bool _isFilter;

	private int _taskColor;

	private int _eventColor;

	private int _noteColor;

	private int _noSttColor;

	private int _compColor;

	private int _notCompColor;

	private SchedulerHitInfo _Shi;

	private double _satStartWHour;

	private double _satEndWHour;

	private DataTable _dtActivities;

	private DataTable _dtSearch;

	private TimeInterval _lastFetchedInterval = new TimeInterval();

	private string _filterString = "";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private DataTable _dtUser;

	private Action ReloadFilterSetting;

	public LookUpEdit FilterFieldControl;

	public FilterValueControl FilterValueControl;

	private new FormModeType _formMode;

	private TaskFilterType TaskFilter;

	private object AllData;

	private IContainer components;

	private SchedulerStorage schedulerStorage1;

	private SchedulerControl schedulerControl1;

	private BarManager barManager1;

	private NewAppointmentItem newAppointmentItem1;

	private NewRecurringAppointmentItem newRecurringAppointmentItem1;

	private NavigatorBar navigatorBar1;

	private NavigateViewBackwardItem navigateViewBackwardItem1;

	private NavigateViewForwardItem navigateViewForwardItem1;

	private GotoTodayItem gotoTodayItem1;

	private ViewZoomInItem viewZoomInItem1;

	private ViewZoomOutItem viewZoomOutItem1;

	private ArrangeBar arrangeBar1;

	private SwitchToDayViewItem switchToDayViewItem1;

	private SwitchToWorkWeekViewItem switchToWorkWeekViewItem1;

	private SwitchToWeekViewItem switchToWeekViewItem1;

	private SwitchToFullWeekViewItem switchToFullWeekViewItem1;

	private SwitchToMonthViewItem switchToMonthViewItem1;

	private SwitchToTimelineViewItem switchToTimelineViewItem1;

	private SwitchToGanttViewItem switchToGanttViewItem1;

	private GroupByBar groupByBar1;

	private GroupByNoneItem groupByNoneItem1;

	private GroupByDateItem groupByDateItem1;

	private GroupByResourceItem groupByResourceItem1;

	private TimeScaleBar timeScaleBar1;

	private SwitchTimeScalesItem switchTimeScalesItem1;

	private ChangeScaleWidthItem changeScaleWidthItem1;

	private RepositoryItemSpinEdit repositoryItemSpinEdit1;

	private SwitchTimeScalesCaptionItem switchTimeScalesCaptionItem1;

	private LayoutBar layoutBar1;

	private SwitchCompressWeekendItem switchCompressWeekendItem1;

	private SwitchShowWorkTimeOnlyItem switchShowWorkTimeOnlyItem1;

	private SwitchCellsAutoHeightItem switchCellsAutoHeightItem1;

	private ChangeSnapToCellsUIItem changeSnapToCellsUIItem1;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private SchedulerBarController schedulerBarController1;

	private EditAppointmentQueryItem editAppointmentQueryItem1;

	private EditOccurrenceUICommandItem editOccurrenceUICommandItem1;

	private EditSeriesUICommandItem editSeriesUICommandItem1;

	private DeleteAppointmentsItem deleteAppointmentsItem1;

	private DeleteOccurrenceItem deleteOccurrenceItem1;

	private DeleteSeriesItem deleteSeriesItem1;

	private SplitAppointmentItem splitAppointmentItem1;

	private OptionsBar optionsBar1;

	private ChangeAppointmentStatusItem changeAppointmentStatusItem1;

	private ChangeAppointmentLabelItem changeAppointmentLabelItem1;

	private ToggleRecurrenceItem toggleRecurrenceItem1;

	private ChangeAppointmentReminderItem changeAppointmentReminderItem1;

	private RepositoryItemDuration repositoryItemDuration1;

	private CommonBar commonBar1;

	private OpenScheduleItem openScheduleItem1;

	private SaveScheduleItem saveScheduleItem1;

	private PrintBar printBar1;

	private PrintPreviewItem printPreviewItem1;

	private PrintItem printItem1;

	private PrintPageSetupItem printPageSetupItem1;

	private ToolTipController toolTipController1;

	private DateNavigator dateNavigator1;

	private LabelLine labelControl1;

	private SplitContainerControl splitContainerControl1;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem checkAllToolStripMenuItem;

	private ToolStripMenuItem checkNoneToolStripMenuItem;

	private CheckedListBoxControl chk_Action;

	private SearchControl searchControl1;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx tab_User;

	private XtraTabPageEx tab_Employee;

	private CheckedListBoxControl chk_Users;

	private CheckedListBoxControl chk_Employees;

	private XtraTabControl xtraTabControl2;

	private XtraTabPage xtraTabPage1;

	private XtraTabPage xtraTabPage2;

	private FilterValueControlEx txtFilterValue;

	private LookUpEditEx txtFilterField;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItemFilter toolStripMenuItemFilter1;

	private LabelLine labelLine1;

	private SimpleButton simpleButton1;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			return;
		}
		base._formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Run:
		{
			for (int j = 0; j < barManager1.Bars.Count; j++)
			{
				barManager1.Bars[j].Visible = false;
			}
			schedulerControl1.Enabled = false;
			dateNavigator1.Enabled = false;
			xtraTabControl1.Enabled = false;
			xtraTabControl2.Enabled = false;
			searchControl1.Enabled = false;
			break;
		}
		case Enums.FormViewModeType.View:
		{
			for (int i = 0; i < barManager1.Bars.Count; i++)
			{
				barManager1.Bars[i].Visible = true;
			}
			schedulerControl1.Enabled = true;
			dateNavigator1.Enabled = true;
			xtraTabControl1.Enabled = true;
			xtraTabControl2.Enabled = true;
			searchControl1.Enabled = true;
			break;
		}
		}
	}

	public frm_ActivityCalendar()
	{
		try
		{
			InitializeComponent();
			FilterFieldControl = txtFilterField;
			FilterValueControl = txtFilterValue;
			SchedulerLocalizer.Active = new MyLocalizer();
			OnLoadAsync += frm_ActivityCalendar_Load;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadInitForm()
	{
		try
		{
			DataSet dataSet = await _httpClient.Cfl.GetUserList();
			if (dataSet != null && dataSet.Tables.Count >= 2)
			{
				_dtUser = dataSet.Tables[1];
			}
			schedulerControl1.AllowAppointmentCopy += SchedulerControl1OnAllowAppointmentCopy;
			chk_Users.CheckOnClick = true;
			chk_Users.ContextMenuStrip = contextMenuStrip1;
			chk_Users.HotTrackSelectMode = HotTrackSelectMode.SelectItemOnClick;
			chk_Users.SelectionMode = SelectionMode.MultiExtended;
			chk_Employees.CheckOnClick = true;
			chk_Employees.ContextMenuStrip = contextMenuStrip1;
			chk_Employees.HotTrackSelectMode = HotTrackSelectMode.SelectItemOnClick;
			chk_Employees.SelectionMode = SelectionMode.MultiExtended;
			chk_Users.ItemCheck += chk_Users_ItemCheck;
			chk_Employees.ItemCheck += chk_Users_ItemCheck;
			chk_Action.BackColor = BackColor;
			xtraTabPage2.BackColor = BackColor;
			chk_Action.BorderStyle = BorderStyles.NoBorder;
			chk_Users.BackColor = BackColor;
			chk_Users.BorderStyle = BorderStyles.NoBorder;
			chk_Employees.BackColor = BackColor;
			chk_Employees.BorderStyle = BorderStyles.NoBorder;
			searchControl1.Client = chk_Users;
			_dtSearch = new DataTable();
			_dtSearch.Columns.Add("userCode", typeof(string));
			_dtSearch.Columns.Add("startDate", typeof(DateTime));
			_dtSearch.Columns.Add("endDate", typeof(DateTime));
			schedulerControl1.BeginUpdate();
			schedulerControl1.ResourceColorSchemas.Clear();
			SchedulerColorSchema value = new SchedulerColorSchema(Color.White);
			schedulerControl1.ResourceColorSchemas.Add(value);
			toolTipController1.ToolTipType = ToolTipType.SuperTip;
			schedulerControl1.Views.DayView.AppointmentDisplayOptions.StartTimeVisibility = AppointmentTimeVisibility.Never;
			schedulerControl1.Views.DayView.AppointmentDisplayOptions.EndTimeVisibility = AppointmentTimeVisibility.Never;
			schedulerControl1.Views.DayView.AppointmentDisplayOptions.StatusDisplayType = AppointmentStatusDisplayType.Bounds;
			schedulerControl1.Views.WeekView.AppointmentDisplayOptions.StartTimeVisibility = AppointmentTimeVisibility.Never;
			schedulerControl1.Views.WeekView.AppointmentDisplayOptions.EndTimeVisibility = AppointmentTimeVisibility.Never;
			schedulerControl1.Views.WeekView.AppointmentDisplayOptions.StatusDisplayType = AppointmentStatusDisplayType.Bounds;
			schedulerControl1.Views.WorkWeekView.AppointmentDisplayOptions.StartTimeVisibility = AppointmentTimeVisibility.Never;
			schedulerControl1.Views.WorkWeekView.AppointmentDisplayOptions.EndTimeVisibility = AppointmentTimeVisibility.Never;
			schedulerControl1.Views.WorkWeekView.AppointmentDisplayOptions.StatusDisplayType = AppointmentStatusDisplayType.Bounds;
			schedulerControl1.Views.FullWeekView.AppointmentDisplayOptions.StartTimeVisibility = AppointmentTimeVisibility.Never;
			schedulerControl1.Views.FullWeekView.AppointmentDisplayOptions.EndTimeVisibility = AppointmentTimeVisibility.Never;
			schedulerControl1.Views.FullWeekView.AppointmentDisplayOptions.StatusDisplayType = AppointmentStatusDisplayType.Bounds;
			schedulerControl1.Views.TimelineView.AppointmentDisplayOptions.StartTimeVisibility = AppointmentTimeVisibility.Never;
			schedulerControl1.Views.TimelineView.AppointmentDisplayOptions.EndTimeVisibility = AppointmentTimeVisibility.Never;
			schedulerControl1.Views.TimelineView.AppointmentDisplayOptions.StatusDisplayType = AppointmentStatusDisplayType.Bounds;
			schedulerControl1.Views.GanttView.AppointmentDisplayOptions.StartTimeVisibility = AppointmentTimeVisibility.Never;
			schedulerControl1.Views.GanttView.AppointmentDisplayOptions.EndTimeVisibility = AppointmentTimeVisibility.Never;
			schedulerControl1.Views.GanttView.AppointmentDisplayOptions.StatusDisplayType = AppointmentStatusDisplayType.Bounds;
			ResourceMappingInfo mappings = schedulerStorage1.Resources.Mappings;
			mappings.Caption = "ResName";
			mappings.Id = "ResourceId";
			ResourceCustomFieldMapping mapping = new ResourceCustomFieldMapping("Color", "Color", FieldValueType.Integer);
			ResourceCustomFieldMapping mapping2 = new ResourceCustomFieldMapping("Checked", "Checked", FieldValueType.String);
			schedulerStorage1.Resources.CustomFieldMappings.Clear();
			schedulerStorage1.Resources.CustomFieldMappings.Add(mapping);
			schedulerStorage1.Resources.CustomFieldMappings.Add(mapping2);
			mappings.Color = "CellColor";
			chk_Action.ValueMember = "Action";
			chk_Action.DisplayMember = "ActionName";
			chk_Action.CheckMember = "IsChecked";
			schedulerStorage1.Appointments.Items.Clear();
			schedulerStorage1.Appointments.CustomFieldMappings.Clear();
			schedulerStorage1.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping("ClgCode", "ClgCode", FieldValueType.Integer));
			schedulerStorage1.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping("Closed", "Closed", FieldValueType.String));
			schedulerStorage1.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping("AssignedBy", "AssignedBy", FieldValueType.Integer));
			AppointmentMappingInfo mappings2 = schedulerStorage1.Appointments.Mappings;
			mappings2.Start = "startDateTime";
			mappings2.End = "endDateTime";
			mappings2.OriginalOccurrenceStart = "startDateTime";
			mappings2.OriginalOccurrenceEnd = "endDateTime";
			mappings2.Location = "LocationCode";
			mappings2.Status = "status";
			mappings2.Label = "Action";
			mappings2.Subject = "Details";
			mappings2.ResourceId = "ResourceIdsInfo";
			mappings2.Description = "Notes";
			mappings2.RecurrenceInfo = "RecurrenceInfo";
			mappings2.Type = "CntctType";
			mappings2.AllDay = "AllDayEvent";
			mappings2.ReminderInfo = "ReminderInfo";
			schedulerControl1.EndUpdate();
			schedulerStorage1.EnableReminders = false;
			ApzObjectTypes objectType = ApzObjectTypes.oCalendar;
			FilterFieldControl.Properties.Columns.Clear();
			FilterFieldControl.Properties.Columns.Add(new LookUpColumnInfo("FilterCode"));
			FilterFieldControl.Properties.Columns.Add(new LookUpColumnInfo("FilterName"));
			FilterFieldControl.Properties.DisplayMember = "FilterName";
			FilterFieldControl.Properties.ValueMember = "FilterCode";
			FilterFieldControl.EditValueChanged += FilterFieldControl_EditValueChanged;
			FilterValueControl.EditValueChanged += txtFilterValue_EditValueChanged;
			DataTable dataFilterField = _httpClient.FormFilterSettingClient.GetDataFilterField(objectType);
			for (int i = 0; i < dataFilterField.Rows.Count; i++)
			{
				string text = Function.ToString(dataFilterField.Rows[i]["FilterCode"]);
				if (!listMapped.Contains(text))
				{
					schedulerStorage1.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping(text, text, FieldValueType.String));
				}
			}
			if (dataFilterField.IsNotNull())
			{
				DataRow dataRow = dataFilterField.NewRow();
				dataRow[0] = -1;
				dataRow[1] = "";
				dataRow[2] = "";
				dataFilterField.Rows.Add(dataRow);
				dataFilterField.AcceptChanges();
			}
			FilterFieldControl.Properties.DataSource = dataFilterField;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void FilterFieldControl_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			FilterValueControl.EditValue = "";
			await PushValueToFilterValueControl();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task PushValueToFilterValueControl()
	{
		try
		{
			toolStripMenuItemFilter1.DataSource = schedulerStorage1.Appointments.DataSource as DataTable;
			AllData = schedulerStorage1.Appointments.DataSource as DataTable;
			DataTable dataTable = FilterFieldControl.Properties.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				return;
			}
			string fieldName = Function.ToString(FilterFieldControl.EditValue);
			if (string.IsNullOrEmpty(fieldName))
			{
				return;
			}
			DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["FilterCode"]).Equals(fieldName));
			if (dataRow == null)
			{
				return;
			}
			if (Function.ToString(dataRow["SourceType"]).Equals("U"))
			{
				if (AllData is DataTable dataTable2 && dataTable2.Columns.Contains(fieldName))
				{
					DataTable dataTable3 = dataTable2.AsDataView().ToTable(true, fieldName);
					dataTable3.Columns[0].ColumnName = "Code";
					DataColumn column = new DataColumn("Name")
					{
						Expression = "Code"
					};
					dataTable3.Columns.Add(column);
					dataTable3.AcceptChanges();
					FilterValueControl.SourceFilter = dataTable3;
				}
			}
			else
			{
				FilterValueControl filterValueControl = FilterValueControl;
				filterValueControl.SourceFilter = await _httpClient.FormFilterSettingClient.GetDataFilterValue(Function.ParseInt(dataRow["DocEntry"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtFilterValue_EditValueChanged(object sender, EventArgs e)
	{
		DoFilter();
	}

	private void simpleButton1_Click(object sender, EventArgs e)
	{
		FilterFieldControl.EditValue = "";
	}

	private void DoFilter()
	{
		try
		{
			if (!(AllData is DataTable dataTable))
			{
				return;
			}
			DataTable dataTable2 = dataTable.Copy();
			string filterField = Function.ToString(FilterFieldControl.EditValue);
			string text = Function.ToString(FilterValueControl.EditValue);
			if (string.IsNullOrEmpty(text) || !dataTable2.Columns.Contains(filterField))
			{
				_isFilter = true;
				schedulerStorage1.Appointments.DataSource = dataTable2;
				_isFilter = false;
				return;
			}
			DataTable dataTable3 = new DataTable();
			Type dataType = dataTable2.Columns[filterField].DataType;
			if (dataType == typeof(int) || dataType == typeof(short) || dataType == typeof(int) || dataType == typeof(long))
			{
				List<int> lstValue = text.Split(';').Select(Function.ParseInt).ToList();
				EnumerableRowCollection<DataRow> source = from t in dataTable2.AsEnumerable()
					where lstValue.Contains(Function.ParseInt(t[filterField]))
					select t;
				if (source.Any())
				{
					dataTable3 = source.CopyToDataTable();
				}
			}
			else if (dataType == typeof(double) || dataType == typeof(decimal) || dataType == typeof(float))
			{
				List<double> lstValue2 = text.Split(';').Select(Function.ParseDouble).ToList();
				EnumerableRowCollection<DataRow> source2 = from t in dataTable2.AsEnumerable()
					where lstValue2.Exists((double x) => Math.Abs(Function.ParseDouble(t[filterField]) - Function.ParseDouble(x)) < 1E-06)
					select t;
				if (source2.Any())
				{
					dataTable3 = source2.CopyToDataTable();
				}
			}
			else if (dataType == typeof(DateTime))
			{
				List<DateTime> lstValue3 = text.Split(';').Select(Function.ParseDateTimes).ToList();
				EnumerableRowCollection<DataRow> source3 = from t in dataTable2.AsEnumerable()
					where lstValue3.Contains(Function.ParseDateTimes(t[filterField]))
					select t;
				if (source3.Any())
				{
					dataTable3 = source3.CopyToDataTable();
				}
			}
			else
			{
				List<string> lstValue4 = text.Split(';').ToList();
				EnumerableRowCollection<DataRow> source4 = from t in dataTable2.AsEnumerable()
					where lstValue4.Contains(Function.ToString(t[filterField]))
					select t;
				if (source4.Any())
				{
					dataTable3 = source4.CopyToDataTable();
				}
			}
			dataTable3.AcceptChanges();
			_isFilter = true;
			schedulerStorage1.Appointments.DataSource = dataTable3;
			_isFilter = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SchedulerControl1OnAllowAppointmentCopy(object sender, AppointmentOperationEventArgs e)
	{
		try
		{
			if (Function.ToString(e.Appointment.LabelKey) == "V" || Function.ToString(e.Appointment.LabelKey) == "E" || Function.ToString(e.Appointment.LabelKey) == "T")
			{
				e.Allow = false;
			}
		}
		catch (Exception value)
		{
			Console.WriteLine(value);
			throw;
		}
	}

	private async Task InitCalendar()
	{
		try
		{
			ArrayList dtAction = new ArrayList();
			CalendarAction calAction1 = new CalendarAction("T", "Completed Task", true);
			CalendarAction calAction2 = new CalendarAction("U", "Uncompleted Task", true);
			CalendarAction calAction3 = new CalendarAction("V", "Event", true);
			CalendarAction calAction4 = new CalendarAction("E", "Note", true);
			schedulerControl1.OptionsView.ShowOnlyResourceAppointments = false;
			schedulerControl1.BeginUpdate();
			DataSet dataSet = await _httpClient.CalendarSettingClient.GetDoc(Function.ToString(GlobalData.UserCodeLogin));
			if (dataSet.IsNotNull() && dataSet.Tables.Count >= 2)
			{
				DataTable dataTable = dataSet.Tables[1];
				double value;
				double value2;
				int resourcesPerPage;
				string text;
				double satStartWHour;
				double satEndWHour;
				int taskColor;
				int eventColor;
				int noteColor;
				int noSttColor;
				int notCompColor;
				int compColor;
				if (dataTable.IsNotNull())
				{
					value = Function.ParseDouble(dataTable.Rows[0]["StartWorkingHour"]) / 100.0;
					value2 = Function.ParseDouble(dataTable.Rows[0]["EndWorkingHour"]) / 100.0;
					resourcesPerPage = Function.ParseInt(dataTable.Rows[0]["ResPerPage"]);
					text = Function.ToString(dataTable.Rows[0]["SatWorking"]);
					satStartWHour = Function.ParseDouble(dataTable.Rows[0]["SatStartWHour"]) / 100.0;
					satEndWHour = Function.ParseDouble(dataTable.Rows[0]["SatEndWHour"]) / 100.0;
					taskColor = Function.ParseInt(dataTable.Rows[0]["TaskColor"]);
					eventColor = Function.ParseInt(dataTable.Rows[0]["EventColor"]);
					noteColor = Function.ParseInt(dataTable.Rows[0]["NoteColor"]);
					noSttColor = Function.ParseInt(dataTable.Rows[0]["NoSttColor"]);
					notCompColor = Function.ParseInt(dataTable.Rows[0]["NotCompletedColor"]);
					compColor = Function.ParseInt(dataTable.Rows[0]["CompletedColor"]);
					schedulerControl1.GroupType = (SchedulerGroupType)Function.ParseInt(dataTable.Rows[0]["GroupType"]);
					schedulerControl1.ActiveViewType = (SchedulerViewType)Function.ParseInt(dataTable.Rows[0]["ActiveViewType"]);
					if (Function.ToString(dataTable.Rows[0]["ShowToolTip"]) == "S")
					{
						schedulerControl1.OptionsView.ToolTipVisibility = ToolTipVisibility.Standard;
					}
					else if (Function.ToString(dataTable.Rows[0]["ShowToolTip"]) == "A")
					{
						schedulerControl1.OptionsView.ToolTipVisibility = ToolTipVisibility.Always;
					}
					else
					{
						schedulerControl1.OptionsView.ToolTipVisibility = ToolTipVisibility.Never;
					}
					calAction1.IsChecked = Function.ToString(dataTable.Rows[0]["ShowCompleted"]) == "Y";
					calAction2.IsChecked = Function.ToString(dataTable.Rows[0]["ShowUncompleted"]) == "Y";
					calAction3.IsChecked = Function.ToString(dataTable.Rows[0]["ShowEvent"]) == "Y";
					calAction4.IsChecked = Function.ToString(dataTable.Rows[0]["ShowNote"]) == "Y";
				}
				else
				{
					value = 8.0;
					value2 = 17.0;
					resourcesPerPage = 3;
					text = "N";
					satStartWHour = 8.0;
					satEndWHour = 17.0;
					taskColor = -4980813;
					eventColor = -14669;
					noteColor = -77;
					noSttColor = -1;
					compColor = -7156260;
					notCompColor = -3883625;
					schedulerControl1.GroupType = SchedulerGroupType.Resource;
					schedulerControl1.ActiveViewType = SchedulerViewType.Day;
					schedulerControl1.OptionsView.ToolTipVisibility = ToolTipVisibility.Standard;
				}
				dtAction.Add(calAction1);
				dtAction.Add(calAction2);
				dtAction.Add(calAction3);
				dtAction.Add(calAction4);
				chk_Action.DataSource = dtAction;
				_filterString = "";
				object[] array = dtAction.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					CalendarAction calendarAction = (CalendarAction)array[i];
					if (calendarAction.IsChecked == true)
					{
						if (calendarAction.Action == "T")
						{
							_filterString += " OR ([Label] = 'T' AND [Status] = 2) ";
						}
						else if (calendarAction.Action == "U")
						{
							_filterString += " OR ([Label] = 'T' AND [Status] = 1) ";
						}
						else
						{
							_filterString = _filterString + " OR [Label] = '" + calendarAction.Action + "' ";
						}
					}
					else if (calendarAction.Action == "T")
					{
						_filterString += "AND ([Label] = 'T' AND [Status] != 2) ";
					}
					else if (calendarAction.Action == "U")
					{
						_filterString += "AND ([Label] = 'T' AND [Status] != 1) ";
					}
					else
					{
						_filterString = _filterString + "AND [Label] != '" + calendarAction.Action + "' ";
					}
				}
				if (!string.IsNullOrEmpty(_filterString) && _filterString.Length > 3)
				{
					schedulerStorage1.Appointments.Filter = _filterString.Substring(3);
				}
				else
				{
					schedulerStorage1.Appointments.Filter = "";
				}
				_isSatWorking = Function.ToString(text) == "Y";
				_satStartWHour = satStartWHour;
				_satEndWHour = satEndWHour;
				_taskColor = taskColor;
				_eventColor = eventColor;
				_noteColor = noteColor;
				_noSttColor = noSttColor;
				_compColor = compColor;
				_notCompColor = notCompColor;
				schedulerControl1.DayView.ResourcesPerPage = resourcesPerPage;
				schedulerControl1.WeekView.ResourcesPerPage = resourcesPerPage;
				schedulerControl1.WorkWeekView.ResourcesPerPage = resourcesPerPage;
				schedulerControl1.FullWeekView.ResourcesPerPage = resourcesPerPage;
				schedulerControl1.MonthView.ResourcesPerPage = resourcesPerPage;
				schedulerControl1.TimelineView.ResourcesPerPage = resourcesPerPage;
				schedulerControl1.GanttView.ResourcesPerPage = resourcesPerPage;
				schedulerControl1.WorkWeekView.WorkTime = new TimeOfDayInterval(TimeSpan.FromHours(value), TimeSpan.FromHours(value2));
				schedulerControl1.DayView.WorkTime = new TimeOfDayInterval(TimeSpan.FromHours(value), TimeSpan.FromHours(value2));
				schedulerControl1.FullWeekView.WorkTime = new TimeOfDayInterval(TimeSpan.FromHours(value), TimeSpan.FromHours(value2));
				schedulerControl1.TimelineView.WorkTime = new TimeOfDayInterval(TimeSpan.FromHours(value), TimeSpan.FromHours(value2));
				schedulerControl1.GanttView.WorkTime = new TimeOfDayInterval(TimeSpan.FromHours(value), TimeSpan.FromHours(value2));
				schedulerControl1.WorkDays.BeginUpdate();
				if (Function.ToString(text) == "Y")
				{
					schedulerControl1.WorkDays.Add(WeekDays.Saturday);
				}
				else
				{
					schedulerControl1.WorkDays.Remove(new WeekDaysWorkDay(WeekDays.Saturday));
				}
				schedulerControl1.WorkDays.EndUpdate();
				AppointmentLabelCollection labels = schedulerStorage1.Appointments.Labels;
				if (labels.Count > 0)
				{
					labels.BeginUpdate();
					AppointmentLabel byId = labels.GetById("T");
					if (byId != null)
					{
						byId.Color = Color.FromArgb(_taskColor);
					}
					AppointmentLabel byId2 = labels.GetById("V");
					if (byId2 != null)
					{
						byId2.Color = Color.FromArgb(_eventColor);
					}
					AppointmentLabel byId3 = labels.GetById("E");
					if (byId3 != null)
					{
						byId3.Color = Color.FromArgb(_noteColor);
					}
					labels.EndUpdate();
				}
				AppointmentStatusCollection statuses = schedulerStorage1.Appointments.Statuses;
				if (statuses.Count > 0)
				{
					statuses.BeginUpdate();
					AppointmentStatus byId4 = statuses.GetById(0);
					if (byId4 != null)
					{
						byId4.Brush = new SolidBrush(Color.FromArgb(_noSttColor));
					}
					AppointmentStatus byId5 = statuses.GetById(1);
					if (byId5 != null)
					{
						byId5.Brush = new SolidBrush(Color.FromArgb(_notCompColor));
					}
					AppointmentStatus byId6 = statuses.GetById(2);
					if (byId6 != null)
					{
						byId6.Brush = new SolidBrush(Color.FromArgb(_compColor));
					}
					statuses.EndUpdate();
				}
			}
			schedulerControl1.Start = DateTime.Now;
			schedulerControl1.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadData()
	{
		try
		{
			schedulerControl1.BeginUpdate();
			schedulerStorage1.FetchAppointments += SchedulerStorage_FetchAppointments;
			InitSttAndAction();
			await InitResources();
			schedulerControl1.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void InitSttAndAction()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("statusID", typeof(int));
			dataTable.Columns.Add("name", typeof(string));
			dataTable.Columns.Add("color", typeof(int));
			DataRow dataRow = dataTable.NewRow();
			dataRow["statusID"] = 0;
			dataRow["name"] = "No status";
			dataRow["color"] = _noSttColor;
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["statusID"] = 1;
			dataRow2["name"] = "Not Completed";
			dataRow2["color"] = _notCompColor;
			DataRow dataRow3 = dataTable.NewRow();
			dataRow3["statusID"] = 2;
			dataRow3["name"] = "Completed";
			dataRow3["color"] = _compColor;
			dataTable.Rows.Add(dataRow);
			dataTable.Rows.Add(dataRow2);
			dataTable.Rows.Add(dataRow3);
			dataTable.AcceptChanges();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				SolidBrush item = new SolidBrush(Color.FromArgb(Function.ParseInt(dataTable.Rows[i]["color"])));
				_statusList.Add(Function.ToString(dataTable.Rows[i]["name"]));
				_statusIdList.Add(Function.ParseInt(dataTable.Rows[i]["statusID"]));
				_sttColorList.Add(item);
			}
			AppointmentLabelCollection labels = schedulerStorage1.Appointments.Labels;
			labels.Clear();
			labels.BeginUpdate();
			foreach (KeyValuePair<string, Color> item2 in DicAction)
			{
				labels.Add(labelColor: (!(item2.Key == "T")) ? ((!(item2.Key == "V")) ? Color.FromArgb(_noteColor) : Color.FromArgb(_eventColor)) : Color.FromArgb(_taskColor), id: item2.Key, displayName: DicActionName[item2.Key], menuCaption: DicActionName[item2.Key]);
			}
			labels.EndUpdate();
			AppointmentStatusCollection statuses = schedulerStorage1.Appointments.Statuses;
			statuses.Clear();
			int count = _statusList.Count;
			statuses.BeginUpdate();
			for (int j = 0; j < count; j++)
			{
				statuses.Add(_statusIdList[j], AppointmentStatusType.Custom, _statusList[j], _statusList[j], _sttColorList[j]);
			}
			statuses.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task InitResources()
	{
		try
		{
			schedulerControl1.BeginUpdate();
			DataSet dataSet = await _httpClient.CalendarSettingClient.GetDoc(Function.ToString(GlobalData.UserCodeLogin));
			if (dataSet.IsNotNull() && dataSet.Tables.Count >= 2)
			{
				schedulerStorage1.BeginUpdate();
				DataTable dataSource = dataSet.Tables[0];
				schedulerStorage1.Resources.DataSource = dataSource;
				schedulerStorage1.EndUpdate();
				InitResourceCheckBoxes();
			}
			schedulerControl1.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void InitResourceCheckBoxes()
	{
		try
		{
			chk_Users.Items.Clear();
			chk_Employees.Items.Clear();
			foreach (Resource item in schedulerStorage1.Resources.Items)
			{
				if (item.GetRow(schedulerStorage1) is DataRowView dataRowView)
				{
					string text = Function.ToString(dataRowView["ResourceId"]);
					Function.ParseInt(text.Substring(1, text.Length - 1));
					string description = Function.ToString(dataRowView["ResName"]);
					string text2 = text.Substring(0, text.Length - 1);
					CheckedListBoxItem checkedListBoxItem = new CheckedListBoxItem
					{
						Value = text,
						Description = description
					};
					if (Function.ToString(dataRowView["Checked"]) == "Y")
					{
						checkedListBoxItem.CheckState = CheckState.Checked;
					}
					else
					{
						checkedListBoxItem.CheckState = CheckState.Unchecked;
					}
					if (text2.Contains("U"))
					{
						chk_Users.Items.Add(checkedListBoxItem);
					}
					else
					{
						chk_Employees.Items.Add(checkedListBoxItem);
					}
					if (checkedListBoxItem.CheckState == CheckState.Checked)
					{
						item.Visible = true;
					}
					else
					{
						item.Visible = false;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			_lastFetchedInterval = new TimeInterval();
			schedulerStorage1.RaiseFetchAppointments(new TimeInterval(dateNavigator1.SelectionStart, dateNavigator1.SelectionEnd));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task ReloadCalendar()
	{
		try
		{
			await REFRESH_DATA();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void NoAuthSetControlSearch()
	{
		chk_Users.Enabled = true;
		chk_Employees.Enabled = true;
	}

	public override void FORMSETTING()
	{
		try
		{
			frm_CalendarSetting frm_CalendarSetting2 = new frm_CalendarSetting();
			frm_CalendarSetting2.MdiParent = MdiParent;
			frm_CalendarSetting2.RefreshCal = InitResources;
			frm_CalendarSetting2.RefreshSettings = InitCalendar;
			frm_CalendarSetting2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewForm(ActivityType type)
	{
		try
		{
			if (_Shi != null)
			{
				TimeInterval selectedInterval = schedulerControl1.SelectedInterval;
				DateTime date = selectedInterval.Start.Date;
				string text = selectedInterval.Start.ToString("HH:mm");
				DateTime date2 = selectedInterval.End.Date;
				string text2 = selectedInterval.End.ToString("HH:mm");
				bool allDay = selectedInterval.AllDay;
				ResourceIdCollection resourceIdCollection = new ResourceIdCollection();
				SchedulerCompatibility.Base64XmlObjectSerialization = false;
				string text3 = "";
				if (_Shi.ViewInfo.Resource != null)
				{
					text3 = ((_Shi.ViewInfo.Resource.Id == EmptyResourceId.Id) ? ("U" + GlobalData.UserID) : Function.ToString(_Shi.ViewInfo.Resource.Id));
				}
				_ = schedulerStorage1.Appointments.DataSource;
				DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OCLG");
				DataRow dataRow = tableStructure.NewRow();
				dataRow["ClgCode"] = 0;
				dataRow["Recontact"] = date;
				dataRow["EndDate"] = date2;
				dataRow["BeginTime"] = Function.ParseInt(text.Replace(":", ""));
				dataRow["ENDTime"] = Function.ParseInt(text2.Replace(":", ""));
				dataRow["AllDayEvent"] = allDay;
				if (Function.ToString(text3).StartsWith("U"))
				{
					dataRow["AttendUser"] = Function.ParseInt(text3.Substring(1));
				}
				else
				{
					dataRow["AttendEmpl"] = Function.ParseInt(text3.Substring(1));
				}
				if (text3.StartsWith("U"))
				{
					dataRow["GroupUser"] = Function.ParseInt(text3.Substring(1));
					dataRow["GroupEmployee"] = "";
				}
				else
				{
					dataRow["GroupEmployee"] = Function.ParseInt(text3.Substring(1));
					dataRow["GroupUser"] = "";
				}
				resourceIdCollection.Add(Function.ToString(text3));
				string value = new AppointmentResourceIdCollectionXmlPersistenceHelper(resourceIdCollection).ToXml();
				dataRow["ResourceIdsInfo"] = value;
				tableStructure.Rows.Add(dataRow);
				tableStructure.AcceptChanges();
				WinCommons._OpenProcessing();
				switch (type)
				{
				case ActivityType.Task:
				{
					frm_TaskManagement frm_TaskManagement3 = new frm_TaskManagement();
					frm_TaskManagement3.MdiParent = MdiParent;
					frm_TaskManagement3.CallFromCal = true;
					frm_TaskManagement3.DataRowFromCal = dataRow;
					frm_TaskManagement3.Show();
					break;
				}
				case ActivityType.Event:
				{
					frm_EventManagement frm_EventManagement2 = new frm_EventManagement();
					frm_EventManagement2.MdiParent = MdiParent;
					frm_EventManagement2.CallFromCal = true;
					frm_EventManagement2.DataRowFromCal = dataRow;
					frm_EventManagement2.Show();
					break;
				}
				case ActivityType.Note:
				{
					frm_NoteManagement frm_NoteManagement2 = new frm_NoteManagement();
					frm_NoteManagement2.MdiParent = MdiParent;
					frm_NoteManagement2.CallFromCal = true;
					frm_NoteManagement2.DataRowFromCal = dataRow;
					frm_NoteManagement2.Show();
					break;
				}
				default:
				{
					frm_TaskManagement frm_TaskManagement2 = new frm_TaskManagement();
					frm_TaskManagement2.MdiParent = MdiParent;
					frm_TaskManagement2.CallFromCal = true;
					frm_TaskManagement2.DataRowFromCal = dataRow;
					frm_TaskManagement2.Show();
					break;
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
	}

	public async Task PRINTLAYOUT()
	{
		try
		{
			schedulerControl1.ShowPrintPreview();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task PRINT()
	{
		try
		{
			schedulerControl1.ShowPrintOptionsForm();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_ActivityCalendar_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			_formLoading = true;
			FormMode = Enums.FormViewModeType.Run;
			await LoadInitForm();
			await InitCalendar();
			await LoadData();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void schedulerStorage1_FilterResource(object sender, PersistentObjectCancelEventArgs e)
	{
		try
		{
			if (!((Resource)e.Object).Visible)
			{
				e.Cancel = !((Resource)e.Object).Visible;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void schedulerControl1_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
	{
		try
		{
			OpenAppoiment(sender, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void OpenAppoiment(object sender, EventArgs eventArgs)
	{
		_ = 1;
		try
		{
			_Shi = schedulerControl1.ActiveView.ViewInfo.CalcHitInfo(schedulerControl1.PointToClient(System.Windows.Forms.Cursor.Position), layoutOnly: false);
			if (schedulerControl1.SelectedAppointments.Count > 0)
			{
				Appointment currentApt = schedulerControl1.SelectedAppointments[0];
				int selectedClgCode = Function.ParseInt(currentApt.CustomFields["ClgCode"]);
				DataTable dataTable = schedulerStorage1.Appointments.DataSource as DataTable;
				int recurIndex = currentApt.RecurrenceIndex;
				IRecurrenceInfo recurrenceInfo = currentApt.RecurrenceInfo;
				string recurXml = "";
				string origXml = "";
				if (recurrenceInfo != null)
				{
					RecurrenceInfoXmlPersistenceHelper recurrenceInfoXmlPersistenceHelper = new RecurrenceInfoXmlPersistenceHelper(recurrenceInfo, DateTimeSavingMode.Appointment);
					recurXml = recurrenceInfoXmlPersistenceHelper.ToXml();
				}
				AppointmentType type = currentApt.Type;
				if (dataTable.IsNotNull())
				{
					if (Function.ParseInt(selectedClgCode) == 0)
					{
						return;
					}
					DataTable dataTable2 = dataTable.Copy();
					DataRow dtRow = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ClgCode"]) == selectedClgCode);
					if (dtRow != null && currentApt.IsOccurrence)
					{
						DateTime date = currentApt.Start.Date;
						DateTime date2 = currentApt.End.Date;
						string text = currentApt.Start.ToString("HH:mm");
						string text2 = currentApt.End.ToString("HH:mm");
						dtRow["Recontact"] = date;
						dtRow["BeginTime"] = Function.ParseInt(text.Replace(":", ""));
						dtRow["EndDate"] = date2;
						dtRow["ENDTime"] = Function.ParseInt(text2.Replace(":", ""));
					}
					dataTable2.AcceptChanges();
					DataTable dataTable3 = ((Function.ParseInt(dtRow["SeriesNum"]) == 0) ? (await _httpClient.SubActivities.LoadEventByCode(selectedClgCode, GlobalData.UserCodeLogin)) : (await _httpClient.SubActivities.LoadEventByCode(Function.ParseInt(dtRow["SeriesNum"]), GlobalData.UserCodeLogin)));
					if (dataTable3.IsNotNull())
					{
						origXml = Function.ToString(dataTable3.Rows[0]["RecurrenceInfo"]);
					}
					WinCommons._OpenProcessing();
					switch (Function.ToString(currentApt.LabelKey))
					{
					case "T":
					{
						frm_TaskManagement frm_TaskManagement3 = new frm_TaskManagement();
						frm_TaskManagement3.ClgCode = selectedClgCode;
						frm_TaskManagement3.CallFromCal = true;
						frm_TaskManagement3.MdiParent = MdiParent;
						frm_TaskManagement3.DataRowFromCal = dtRow;
						frm_TaskManagement3.Show();
						break;
					}
					case "V":
					{
						frm_EventManagement frm_EventManagement2 = new frm_EventManagement();
						frm_EventManagement2.ClgCode = selectedClgCode;
						frm_EventManagement2.CallFromCal = true;
						frm_EventManagement2.MdiParent = MdiParent;
						frm_EventManagement2.DataRowFromCal = dtRow;
						frm_EventManagement2.RecurIndex = recurIndex;
						frm_EventManagement2.RecurXml = recurXml;
						frm_EventManagement2.AptType = type;
						frm_EventManagement2.OrigRecurXml = origXml;
						frm_EventManagement2.Show();
						break;
					}
					case "E":
					{
						frm_NoteManagement frm_NoteManagement2 = new frm_NoteManagement();
						frm_NoteManagement2.ClgCode = selectedClgCode;
						frm_NoteManagement2.CallFromCal = true;
						frm_NoteManagement2.MdiParent = MdiParent;
						frm_NoteManagement2.DataRowFromCal = dtRow;
						frm_NoteManagement2.Show();
						break;
					}
					default:
					{
						frm_TaskManagement frm_TaskManagement2 = new frm_TaskManagement();
						frm_TaskManagement2.ClgCode = selectedClgCode;
						frm_TaskManagement2.CallFromCal = true;
						frm_TaskManagement2.MdiParent = MdiParent;
						frm_TaskManagement2.DataRowFromCal = dtRow;
						frm_TaskManagement2.Show();
						break;
					}
					}
				}
				WinCommons._CloseProcessing();
			}
			else
			{
				AddNewForm(ActivityType.Task);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void schedulerControl1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			_Shi = schedulerControl1.ActiveView.ViewInfo.CalcHitInfo(schedulerControl1.PointToClient(System.Windows.Forms.Cursor.Position), layoutOnly: false);
			if (e.Menu.Id == SchedulerMenuItemId.AppointmentMenu)
			{
				e.Menu.RemoveMenuItem(SchedulerMenuItemId.OpenAppointment);
				e.Menu.RemoveMenuItem(SchedulerMenuItemId.EditSeries);
				e.Menu.RemoveMenuItem(SchedulerMenuItemId.DeleteAppointment);
				e.Menu.RemoveMenuItem(SchedulerMenuItemId.LabelSubMenu);
				e.Menu.RemoveMenuItem(SchedulerMenuItemId.RestoreOccurrence);
				SchedulerMenuItem schedulerMenuItem = new SchedulerMenuItem("&Open", OpenAppoiment);
				schedulerMenuItem.BeginGroup = true;
				schedulerMenuItem.Appearance.Font = new Font(schedulerMenuItem.Appearance.Font, schedulerMenuItem.Appearance.Font.Style | FontStyle.Bold);
				e.Menu.Items.Add(schedulerMenuItem);
				SchedulerPopupMenu popupMenuById = e.Menu.GetPopupMenuById(SchedulerMenuItemId.LabelSubMenu);
				if (popupMenuById != null)
				{
					popupMenuById.Caption = "&Activity Action";
				}
				SchedulerPopupMenu popupMenuById2 = e.Menu.GetPopupMenuById(SchedulerMenuItemId.StatusSubMenu);
				if (popupMenuById2 != null)
				{
					popupMenuById2.Caption = "Task &Status";
					popupMenuById2.Items[1].Caption = "&" + popupMenuById2.Items[1].Caption;
					popupMenuById2.Items[2].Caption = "&" + popupMenuById2.Items[2].Caption;
					popupMenuById2.Items.RemoveAt(0);
				}
				if (schedulerControl1.SelectedAppointments.Count > 0)
				{
					Appointment appointment = schedulerControl1.SelectedAppointments[0];
					string text = Function.ToString(appointment.LabelKey);
					SchedulerMenuItem schedulerMenuItem2 = null;
					switch (text)
					{
					case "T":
					{
						schedulerMenuItem2 = new SchedulerMenuItem("&Remove this Task", RemoveTask);
						schedulerMenuItem2.BeginGroup = true;
						SchedulerMenuItem item2 = new SchedulerMenuItem("&Copy Task", CopyEvent);
						SchedulerMenuItem item3 = new SchedulerMenuItem("Copy Task &Advance", CopyAdvanceEvent);
						e.Menu.Items.Add(item2);
						e.Menu.Items.Add(item3);
						e.Menu.Items.Add(schedulerMenuItem2);
						break;
					}
					case "V":
						schedulerMenuItem2 = new SchedulerMenuItem("&Remove this Event", RemoveEvent);
						schedulerMenuItem2.BeginGroup = true;
						if (appointment.Type == AppointmentType.ChangedOccurrence)
						{
							SchedulerMenuItem item = new SchedulerMenuItem("Restore default state", RestoreItemOnClick);
							e.Menu.Items.Add(item);
						}
						e.Menu.Items.Add(schedulerMenuItem2);
						e.Menu.RemoveMenuItem(SchedulerMenuItemId.StatusSubMenu);
						break;
					case "E":
						schedulerMenuItem2 = new SchedulerMenuItem("&Remove this Note", RemoveNote);
						e.Menu.Items.Add(schedulerMenuItem2);
						e.Menu.RemoveMenuItem(SchedulerMenuItemId.StatusSubMenu);
						break;
					}
					if (schedulerMenuItem2 != null)
					{
						schedulerMenuItem2.BeginGroup = true;
					}
				}
			}
			if (e.Menu.Id == SchedulerMenuItemId.DefaultMenu)
			{
				string text2 = ((_Shi.ViewInfo.Resource != null) ? Function.ToString(_Shi.ViewInfo.Resource.Caption) : "<empty>");
				string caption = "New &Task";
				string caption2 = "New &Event";
				string caption3 = "New &Note";
				if (_Shi.ViewInfo.Resource != null && Function.ToString(_Shi.ViewInfo.Resource.Id) != "U" + GlobalData.UserID)
				{
					caption = "New &Task for " + text2;
					caption2 = "New &Event for " + text2;
					caption3 = "New &Note for" + text2;
				}
				SchedulerMenuItem schedulerMenuItem3 = new SchedulerMenuItem(caption, AddTask);
				SchedulerMenuItem item4 = new SchedulerMenuItem(caption2, AddEvent);
				SchedulerMenuItem item5 = new SchedulerMenuItem(caption3, AddNote);
				schedulerMenuItem3.BeginGroup = true;
				e.Menu.Items.Add(schedulerMenuItem3);
				e.Menu.Items.Add(item4);
				e.Menu.Items.Add(item5);
			}
			for (int i = 0; i < e.Menu.Items.Count; i++)
			{
				e.Menu.Items[i].Appearance.TextOptions.HotkeyPrefix = HKeyPrefix.Show;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void CopyEvent(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCopyTaskWithSubTask, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.Yes || schedulerControl1.SelectedAppointments.Count <= 0)
			{
				return;
			}
			Appointment appointment = schedulerControl1.SelectedAppointments[0];
			int selectedClgCode = Function.ParseInt(appointment.CustomFields["ClgCode"]);
			DataTable dataTable = schedulerStorage1.Appointments.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataTable dataTable2 = dataTable.Copy();
				if (dataTable2.ExistsColumn("ResName"))
				{
					dataTable2.Columns.Remove("ResName");
				}
				if (dataTable2.ExistsColumn("ResourceId"))
				{
					dataTable2.Columns.Remove("ResourceId");
				}
				if (dataTable2.ExistsColumn("startDateTime"))
				{
					dataTable2.Columns.Remove("startDateTime");
				}
				if (dataTable2.ExistsColumn("endDateTime"))
				{
					dataTable2.Columns.Remove("endDateTime");
				}
				if (dataTable2.ExistsColumn("LocationCode"))
				{
					dataTable2.Columns.Remove("LocationCode");
				}
				if (dataTable2.ExistsColumn("ProjectCode"))
				{
					dataTable2.Columns.Remove("ProjectCode");
				}
				DataRow dataRow = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ClgCode"]) == selectedClgCode);
				if (dataRow != null && !copyTask)
				{
					await CopyTask(dataRow);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void CopyAdvanceEvent(object sender, EventArgs e)
	{
		try
		{
			if (schedulerControl1.SelectedAppointments.Count <= 0)
			{
				return;
			}
			Appointment appointment = schedulerControl1.SelectedAppointments[0];
			int selectedClgCode = Function.ParseInt(appointment.CustomFields["ClgCode"]);
			DataTable dataTable = schedulerStorage1.Appointments.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataTable dataTable2 = dataTable.Copy();
				if (dataTable2.ExistsColumn("ResName"))
				{
					dataTable2.Columns.Remove("ResName");
				}
				if (dataTable2.ExistsColumn("ResourceId"))
				{
					dataTable2.Columns.Remove("ResourceId");
				}
				if (dataTable2.ExistsColumn("startDateTime"))
				{
					dataTable2.Columns.Remove("startDateTime");
				}
				if (dataTable2.ExistsColumn("endDateTime"))
				{
					dataTable2.Columns.Remove("endDateTime");
				}
				if (dataTable2.ExistsColumn("LocationCode"))
				{
					dataTable2.Columns.Remove("LocationCode");
				}
				if (dataTable2.ExistsColumn("ProjectCode"))
				{
					dataTable2.Columns.Remove("ProjectCode");
				}
				DataRow dataRow = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ClgCode"]) == selectedClgCode);
				if (dataRow != null && !copyTask)
				{
					await AdvancedCopyTask(dataRow);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task AdvancedCopyTask(DataRow copyDataRow)
	{
		try
		{
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OCLG");
			tableStructure.ImportRow(copyDataRow);
			DataRow dataRow = tableStructure.Rows[0];
			frm_CopyTaskSetting frm_CopyTaskSetting2 = new frm_CopyTaskSetting(dataRow);
			frm_CopyTaskSetting2.StartPosition = FormStartPosition.CenterScreen;
			frm_CopyTaskSetting2.ShowDialog();
			if (!frm_CopyTaskSetting2.CopyTask)
			{
				return;
			}
			dataRow["Details"] = frm_CopyTaskSetting2.txtDetails.Text;
			int num = Function.ParseInt(frm_CopyTaskSetting2.rgp_DueDate.EditValue);
			DueDateOptions dueDateOptions = DueDateOptions.ResetDueDate;
			switch (num)
			{
			case 0:
				dueDateOptions = DueDateOptions.Recalculate;
				break;
			case 1:
				dueDateOptions = DueDateOptions.RecalculateKeepStartTime;
				break;
			case 2:
				dueDateOptions = DueDateOptions.KeepDueDate;
				break;
			case 3:
				dueDateOptions = DueDateOptions.ResetDueDate;
				break;
			}
			if (!frm_CopyTaskSetting2.chk_CopyAssignee.Checked)
			{
				dataRow["GroupUser"] = DBNull.Value;
				dataRow["GroupEmployee"] = DBNull.Value;
			}
			if (!frm_CopyTaskSetting2.chk_CopyProject.Checked)
			{
				dataRow["ProjectEntry"] = DBNull.Value;
				dataRow["ProjectWBS"] = DBNull.Value;
			}
			if (!frm_CopyTaskSetting2.chk_CopyAttachment.Checked)
			{
				dataRow["AtcEntry"] = DBNull.Value;
			}
			Function.ParseInt(dataRow["ClgCode"]);
			if (frm_CopyTaskSetting2.chk_IsSubTask.Checked)
			{
				int num2 = Function.ParseInt(frm_CopyTaskSetting2.txtParentTask.Text);
				if (num2 != 0)
				{
					dataRow["parentTask"] = num2;
				}
				if (num2 == 0)
				{
					dataRow["parentTask"] = DBNull.Value;
				}
			}
			if (!frm_CopyTaskSetting2.chk_IsSubTask.Checked)
			{
				dataRow["parentTask"] = DBNull.Value;
			}
			bool keepSubTask = frm_CopyTaskSetting2.chk_CopySubTask.Checked;
			bool keepSubTaskOfSubTask = frm_CopyTaskSetting2.chk_CopySubTaskOfSubTask.Checked;
			bool copyCompletedSubTask = frm_CopyTaskSetting2.chk_CopyCompletedSubTask.Checked;
			await CopyTask(dataRow, advancedCopy: true, keepSubTask, keepSubTaskOfSubTask, dueDateOptions, copyCompletedSubTask);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task CopyTask(DataRow copyDataRow, bool advancedCopy = false, bool keepSubTask = true, bool keepSubTaskOfSubTask = true, DueDateOptions dueDateOptions = DueDateOptions.ResetDueDate, bool copyCompletedSubTask = true)
	{
		_ = 1;
		try
		{
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OCLG");
			tableStructure.ImportRow(copyDataRow);
			DataRow dr = tableStructure.Rows[0];
			int clgCode = Function.ParseInt(dr["ClgCode"]);
			if (clgCode == 0)
			{
				return;
			}
			if (Function.ToString(dr["Details"]) == "")
			{
				WinCommons.Error(Enums.MessageCode.TaskNameIsRequired4Copy, "");
				return;
			}
			dr["prevActvty"] = dr["ClgCode"];
			dr["ClgCode"] = 0;
			dr["CntctDate"] = DBNull.Value;
			dr["AssignedBy"] = DBNull.Value;
			if (advancedCopy)
			{
				switch (dueDateOptions)
				{
				case DueDateOptions.Recalculate:
					RecalculateDueDateAndDueTime(dr);
					dr["Recontact"] = DBNull.Value;
					dr["BeginTime"] = DBNull.Value;
					break;
				case DueDateOptions.RecalculateKeepStartTime:
					RecalculateDueDateOnly(dr);
					dr["Recontact"] = DateTime.Today;
					break;
				case DueDateOptions.KeepDueDate:
					if (Function.ToString(dr["endDate"]) != "")
					{
						DateTime dateTime = Function.ParseDateTimes(dr["endDate"]);
						TimeSpan value = Function.IntSAPToTimeSpan(dr["ENDtime"]);
						dateTime = dateTime.Add(value);
						if (dateTime <= DateTime.Now)
						{
							dr["endDate"] = DBNull.Value;
							dr["ENDtime"] = DBNull.Value;
						}
					}
					dr["Recontact"] = DBNull.Value;
					dr["BeginTime"] = DBNull.Value;
					break;
				case DueDateOptions.ResetDueDate:
					dr["endDate"] = DBNull.Value;
					dr["ENDtime"] = DBNull.Value;
					dr["Recontact"] = DBNull.Value;
					dr["BeginTime"] = DBNull.Value;
					break;
				}
			}
			if (!advancedCopy)
			{
				dr["Recontact"] = DBNull.Value;
				dr["BeginTime"] = DBNull.Value;
				DataRow dataRow = dr;
				dataRow["Details"] = dataRow["Details"]?.ToString() + " (Copy)";
				dr["endDate"] = DBNull.Value;
				dr["ENDTime"] = DBNull.Value;
			}
			copyTask = true;
			await AddMainTask(dr, SaveType.Copy);
			int copyParentTask = Function.ParseInt(dr["ClgCode"]);
			if (keepSubTask)
			{
				await CopyingSubTask(keepSubTaskOfSubTask, dueDateOptions, copyCompletedSubTask, clgCode, copyParentTask, dr);
			}
			copyTask = false;
			ReloadCalendar();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			copyTask = false;
		}
	}

	private async Task CopySubTaskForNewCopiedTask(DataRow copyDataRow, int copyParentTask, bool keepSubTaskOfSubTask = true, DueDateOptions dueDateOptions = DueDateOptions.ResetDueDate, bool copyCompletedSubTask = true)
	{
		_ = 1;
		try
		{
			int clgCode = Function.ParseInt(copyDataRow["ClgCode"]);
			if (clgCode != 0)
			{
				copyDataRow["prevActvty"] = copyDataRow["ClgCode"];
				copyDataRow["ClgCode"] = 0;
				switch (dueDateOptions)
				{
				case DueDateOptions.Recalculate:
					RecalculateDueDateAndDueTime(copyDataRow);
					copyDataRow["Recontact"] = DBNull.Value;
					copyDataRow["BeginTime"] = DBNull.Value;
					break;
				case DueDateOptions.RecalculateKeepStartTime:
					RecalculateDueDateOnly(copyDataRow);
					copyDataRow["Recontact"] = DateTime.Today;
					break;
				case DueDateOptions.KeepDueDate:
					copyDataRow["endDate"] = DBNull.Value;
					copyDataRow["ENDTime"] = DBNull.Value;
					copyDataRow["Recontact"] = DBNull.Value;
					copyDataRow["BeginTime"] = DBNull.Value;
					break;
				case DueDateOptions.ResetDueDate:
					copyDataRow["endDate"] = DBNull.Value;
					copyDataRow["ENDTime"] = DBNull.Value;
					copyDataRow["Recontact"] = DBNull.Value;
					copyDataRow["BeginTime"] = DBNull.Value;
					break;
				}
				copyDataRow["CntctDate"] = DBNull.Value;
				copyDataRow["AssignedBy"] = DBNull.Value;
				copyDataRow["parentTask"] = copyParentTask;
				await AddMainTask(copyDataRow, SaveType.Copy);
				int copyParentTask2 = Function.ParseInt(copyDataRow["ClgCode"]);
				if (keepSubTaskOfSubTask)
				{
					await CopyingSubTask(keepSubTaskOfSubTask, dueDateOptions, copyCompletedSubTask, clgCode, copyParentTask2, copyDataRow);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task CopyingSubTask(bool keepSubTaskOfSubTask, DueDateOptions dueDateOptions, bool copyCompletedSubTask, int clgCode, int copyParentTask, DataRow rowCopy = null)
	{
		_ = 1;
		try
		{
			string userNameLogin = GlobalData.UserNameLogin;
			DataTable dataTable = await _httpClient.SubActivities.LoadSubTask(clgCode, userNameLogin);
			if (!dataTable.IsNotNull())
			{
				return;
			}
			foreach (DataRow item in dataTable.AsEnumerable())
			{
				if (Function.ToString(item["Closed"]) == "Y" && !copyCompletedSubTask)
				{
					continue;
				}
				int num = Function.ParseInt(item["ClgCode"]);
				if (num != clgCode && num != copyParentTask)
				{
					if (rowCopy != null && dueDateOptions == DueDateOptions.GetFromRow)
					{
						item["Recontact"] = rowCopy["Recontact"];
						item["BeginTime"] = rowCopy["BeginTime"];
						item["endDate"] = rowCopy["endDate"];
						item["ENDTime"] = rowCopy["ENDTime"];
						item["GroupUser"] = rowCopy["GroupUser"];
						item["GroupEmployee"] = rowCopy["GroupEmployee"];
						item["ResourceIdsInfo"] = rowCopy["ResourceIdsInfo"];
					}
					await CopySubTaskForNewCopiedTask(item, copyParentTask, keepSubTaskOfSubTask, dueDateOptions, copyCompletedSubTask);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void RecalculateDueDateAndDueTime(DataRow dr)
	{
		if (Function.ToString(dr["endDate"]) == "")
		{
			return;
		}
		DateTime dateTime = Function.ParseDateTimes(dr["endDate"]);
		TimeSpan value = Function.IntSAPToTimeSpan(dr["ENDtime"]);
		dateTime = dateTime.Add(value);
		if (!(Function.ToString(dr["Recontact"]) == ""))
		{
			DateTime dateTime2 = Function.ParseDateTimes(dr["Recontact"]);
			TimeSpan value2 = Function.IntSAPToTimeSpan(dr["BeginTime"]);
			dateTime2 = dateTime2.Add(value2);
			if (!(dateTime < dateTime2))
			{
				TimeSpan value3 = dateTime - dateTime2;
				DateTime dateTime3 = DateTime.Now.Add(value3);
				DateTime date = dateTime3.Date;
				int num = Function.ParseTimeSpanToInt(dateTime3.TimeOfDay);
				dr["endDate"] = date;
				dr["ENDTime"] = num;
			}
		}
	}

	private void RecalculateDueDateOnly(DataRow dr)
	{
		if (Function.ToString(dr["endDate"]) == "")
		{
			return;
		}
		DateTime dateTime = Function.ParseDateTimes(dr["endDate"]);
		if (!(Function.ToString(dr["Recontact"]) == ""))
		{
			DateTime dateTime2 = Function.ParseDateTimes(dr["Recontact"]);
			Function.IntSAPToTimeSpan(dr["BeginTime"]);
			if (!(dateTime < dateTime2))
			{
				TimeSpan timeSpan = dateTime.Date - dateTime2.Date;
				DateTime dateTime3 = DateTime.Today + timeSpan;
				dr["endDate"] = dateTime3;
			}
		}
	}

	private void SetDefaultValueForNewTask(DataRow dr, SaveType saveType)
	{
		if (Function.ToString(dr["GroupUser"]) == "" && Function.ToString(dr["GroupEmployee"]) == "")
		{
			dr["GroupUser"] = GlobalData.UserID;
		}
		if (Function.ToString(dr["Recontact"]) == "")
		{
			dr["Recontact"] = DateTime.Today;
			dr["BeginTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
		}
		dr["Action"] = "T";
		dr["AssignedBy"] = GlobalData.UserID;
		dr["Closed"] = "N";
		if (saveType == SaveType.GridAdd || saveType == SaveType.SubAdd)
		{
			if (Function.ToString(dr["Priority"]) == "")
			{
				dr["Priority"] = "1";
			}
			if (Function.ToString(dr["DocType"]) == "")
			{
				dr["DocType"] = -1;
			}
		}
	}

	private void SetAutoValueForTask(DataRow dr)
	{
		if (Function.ToString(dr["Closed"]) == "Y")
		{
			dr["CloseDate"] = DateTime.Now;
			dr["status"] = 2;
		}
		else
		{
			dr["status"] = 1;
		}
		ResourceIdCollection resourceIdCollection = new ResourceIdCollection();
		SchedulerCompatibility.Base64XmlObjectSerialization = false;
		string text = Function.ToString(dr["GroupUser"]);
		string text2 = Function.ToString(dr["GroupEmployee"]);
		if (text != "")
		{
			string text3 = text;
			if (!string.IsNullOrEmpty(text3))
			{
				string[] array = Function.ToString(text3).Split(';');
				foreach (string text4 in array)
				{
					if (!string.IsNullOrWhiteSpace(text4))
					{
						resourceIdCollection.Add("U" + Function.ToString(text4));
					}
				}
			}
		}
		if (text == "")
		{
			string text3 = text2;
			if (!string.IsNullOrEmpty(text3))
			{
				string[] array = Function.ToString(text3).Split(';');
				foreach (string text5 in array)
				{
					if (!string.IsNullOrWhiteSpace(text5))
					{
						resourceIdCollection.Add("E" + Function.ToString(text5));
					}
				}
			}
		}
		string value = new AppointmentResourceIdCollectionXmlPersistenceHelper(resourceIdCollection).ToXml();
		dr["ResourceIdsInfo"] = value;
	}

	private async Task AddMainTask(DataRow dr, SaveType saveType = SaveType.GridAdd, DataTable dtAtt = null)
	{
		if (addMainTask)
		{
			return;
		}
		try
		{
			if (dr == null || Function.ParseInt(dr["ClgCode"]) > 0 || string.IsNullOrEmpty(Function.ToString(dr["Details"])))
			{
				return;
			}
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OCLG");
			if (tableStructure.HasColumn())
			{
				addMainTask = true;
				SetDefaultValueForNewTask(dr, saveType);
				SetAutoValueForTask(dr);
				tableStructure.ImportRow(dr);
				DataRow dataRow = tableStructure.Rows[0];
				dataRow["CntctDate"] = DateTime.Today;
				dataRow["CntctTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
				dataRow["UpdateDate"] = DateTime.Today;
				dataRow["UpdateTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
				tableStructure.AcceptChanges();
				DataSet dataSet = new DataSet();
				dataSet.Tables.Add(tableStructure);
				if (dtAtt != null && (saveType == SaveType.SingleAdd || saveType == SaveType.Copy))
				{
					dataSet.Tables.Add(dtAtt);
				}
				HttpResult httpResult = await _httpClient.SubActivities.AddTask(dataSet);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					return;
				}
				int num = Function.ParseInt(httpResult.Content);
				dr["ClgCode"] = num;
				ReloadCalendar();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			addMainTask = false;
		}
	}

	private async void RestoreItemOnClick(object sender, EventArgs eventArgs)
	{
		try
		{
			if (schedulerControl1.SelectedAppointments.Count > 0)
			{
				Appointment currentApt = schedulerControl1.SelectedAppointments[0];
				if (!(schedulerStorage1.Appointments.DataSource is DataTable dataTable))
				{
					return;
				}
				DataRow dtRow = dataTable.Copy().AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ClgCode"]) == Function.ParseInt(currentApt.CustomFields["ClgCode"]));
				DataTable dataTable2 = dataTable.Clone();
				if (dtRow != null)
				{
					dataTable2.ImportRow(dtRow);
					dataTable2.AcceptChanges();
					if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRestoreThisRecord, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
					{
						return;
					}
					string text = await _httpClient.SubActivities.RemoveAllEvent(dataTable2);
					if (!string.IsNullOrEmpty(text))
					{
						WinCommons.Error(Enums.MessageCode.None, text);
						return;
					}
					currentApt.RestoreOccurrence();
					schedulerControl1.RefreshData();
					DataRow dataRow = _dtActivities.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ClgCode"]) == Function.ParseInt(dtRow["ClgCode"]));
					if (dataRow != null)
					{
						_dtActivities.Rows.Remove(dataRow);
					}
					_dtActivities.AcceptChanges();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.ThisActivityDoesNotExistAnyMore, "");
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.SomethingWrong, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void RemoveNote(object sender, EventArgs e)
	{
		Remove(ActivityType.Note);
	}

	private void RemoveEvent(object sender, EventArgs e)
	{
		Remove(ActivityType.Event);
	}

	private void RemoveTask(object sender, EventArgs e)
	{
		Remove(ActivityType.Task);
	}

	private async void Remove(ActivityType type)
	{
		_ = 9;
		try
		{
			if (schedulerControl1.SelectedAppointments.Count <= 0)
			{
				return;
			}
			Appointment appointment = schedulerControl1.SelectedAppointments[0];
			int selectedClgCode = Function.ParseInt(appointment.CustomFields["ClgCode"]);
			DataTable dtSource = schedulerStorage1.Appointments.DataSource as DataTable;
			int recurrenceIndex = appointment.RecurrenceIndex;
			RecurrenceInfo recurrenceInfo = (RecurrenceInfo)appointment.RecurrenceInfo;
			_ = appointment.Type;
			if (!dtSource.IsNotNull())
			{
				return;
			}
			DataRow dataRow = dtSource.Copy().AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ClgCode"]) == selectedClgCode);
			if (dataRow == null)
			{
				WinCommons.Error(Enums.MessageCode.CannotGetInfo, "");
				return;
			}
			string text = appointment.Start.ToString("HH:mm");
			string text2 = appointment.End.ToString("HH:mm");
			dataRow["Recontact"] = appointment.Start.Date;
			dataRow["BeginTime"] = Function.ParseInt(text.Replace(":", ""));
			dataRow["EndDate"] = appointment.End.Date;
			dataRow["ENDTime"] = Function.ParseInt(text2.Replace(":", ""));
			DataTable dataTable = dtSource.Clone();
			dataTable.ImportRow(dataRow);
			dataTable.AcceptChanges();
			if (!dataTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.RemoveError, "");
				return;
			}
			DataRow rowDel = dtSource.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ClgCode"]) == selectedClgCode);
			switch (type)
			{
			case ActivityType.Task:
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					break;
				}
				string text7 = await _httpClient.SubActivities.RemoveTask(dataTable);
				if (!string.IsNullOrEmpty(text7))
				{
					WinCommons.Error(Enums.MessageCode.None, text7);
					break;
				}
				ShowDeleteSuccess();
				if (rowDel != null)
				{
					dtSource.Rows.Remove(rowDel);
				}
				break;
			}
			case ActivityType.Event:
			{
				if (appointment.Type == AppointmentType.Normal)
				{
					if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
					{
						break;
					}
					string text4 = await _httpClient.SubActivities.RemoveAllEvent(dataTable);
					if (!string.IsNullOrEmpty(text4))
					{
						WinCommons.Error(Enums.MessageCode.None, text4);
						break;
					}
					if (rowDel != null)
					{
						dtSource.Rows.Remove(rowDel);
					}
					ShowDeleteSuccess();
					break;
				}
				frm_ConfirmDialog frm_ConfirmDialog2 = new frm_ConfirmDialog(2)
				{
					EventSubject = Function.ToString(appointment.Subject),
					StartPosition = FormStartPosition.CenterScreen
				};
				if (frm_ConfirmDialog2.ShowDialog(this) != DialogResult.OK)
				{
					break;
				}
				DataSet dataSet = new DataSet();
				dataSet.Tables.Add(dataTable);
				switch (frm_ConfirmDialog2.GetValue())
				{
				case 1:
					if (dataTable != null)
					{
						DataRow dataRow2 = dataTable.Rows[0];
						if (Function.ParseInt(dataRow2["SeriesNum"]) == 0)
						{
							dataRow2["SeriesNum"] = selectedClgCode;
							dataRow2["ClgCode"] = 0;
						}
						else
						{
							dataRow2["ClgCode"] = selectedClgCode;
						}
						dataRow2["IsRemoved"] = "Y";
						dataRow2["OrigDate"] = Function.ParseDateTimes(dataRow2["Recontact"]);
						PatternReferenceXmlPersistenceHelper patternReferenceXmlPersistenceHelper = new PatternReferenceXmlPersistenceHelper(new PatternReference(recurrenceInfo.Id, recurrenceIndex));
						dataRow2["RecurrenceInfo"] = patternReferenceXmlPersistenceHelper.ToXml();
						dataRow2["CntctType"] = AppointmentType.DeletedOccurrence;
						dataRow2["UpdateDate"] = DateTime.Today;
						dataRow2["UpdateTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
						HttpResult httpResult = ((Function.ParseInt(dataRow2["ClgCode"]) != 0) ? (await _httpClient.SubActivities.SaveThisEvent(dataSet)) : (await _httpClient.SubActivities.AddEvent(dataSet)));
						if (!string.IsNullOrEmpty(httpResult.Message))
						{
							WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
							break;
						}
						await ReloadCalendar();
						ShowDeleteSuccess();
					}
					break;
				case 3:
				{
					string text6 = await _httpClient.SubActivities.RemoveUnchangedEvent(dataTable);
					if (!string.IsNullOrEmpty(text6))
					{
						WinCommons.Error(Enums.MessageCode.None, text6);
						break;
					}
					await ReloadCalendar();
					ShowDeleteSuccess();
					break;
				}
				default:
				{
					string text5 = await _httpClient.SubActivities.RemoveAllEvent(dataTable);
					if (!string.IsNullOrEmpty(text5))
					{
						WinCommons.Error(Enums.MessageCode.None, text5);
						break;
					}
					await ReloadCalendar();
					ShowDeleteSuccess();
					break;
				}
				}
				break;
			}
			case ActivityType.Note:
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					break;
				}
				string text3 = await _httpClient.SubActivities.DeleteNote(dataTable);
				if (!string.IsNullOrEmpty(text3))
				{
					WinCommons.Error(Enums.MessageCode.None, text3);
					break;
				}
				if (rowDel != null)
				{
					dtSource.Rows.Remove(rowDel);
				}
				ShowDeleteSuccess();
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ShowDeleteSuccess()
	{
		WinCommons.Success(Enums.MessageCode.OperationCompletedSuccess, "");
	}

	private void AddTask(object sender, EventArgs eventArgs)
	{
		AddNewForm(ActivityType.Task);
	}

	private void AddEvent(object sender, EventArgs eventArgs)
	{
		AddNewForm(ActivityType.Event);
	}

	private void AddNote(object sender, EventArgs eventArgs)
	{
		AddNewForm(ActivityType.Note);
	}

	private void toolTipController1_BeforeShow(object sender, ToolTipControllerShowEventArgs e)
	{
		try
		{
			if (toolTipController1.ActiveObject is AppointmentViewInfo)
			{
				Appointment appointment = ((AppointmentViewInfo)toolTipController1.ActiveObject).Appointment;
				if (appointment == null)
				{
					return;
				}
				int clgCode = Function.ParseInt(appointment.CustomFields["ClgCode"]);
				DataTable dataTable = schedulerStorage1.Appointments.DataSource as DataTable;
				if (dataTable.IsNotNull())
				{
					dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ClgCode"]) == clgCode);
					if (toolTipController1.ToolTipType == ToolTipType.Standard)
					{
						e.ToolTip = appointment.Description;
					}
					if (toolTipController1.ToolTipType == ToolTipType.SuperTip)
					{
						SuperToolTip superToolTip = new SuperToolTip();
						SuperToolTipSetupArgs superToolTipSetupArgs = new SuperToolTipSetupArgs();
						superToolTipSetupArgs.Title.Text = appointment.Subject;
						superToolTipSetupArgs.Title.Font = new Font("Tahoma", Function.ParseFloat(8.25), FontStyle.Bold);
						superToolTipSetupArgs.ShowFooterSeparator = true;
						if (Function.ToString(appointment.LabelKey) == "V")
						{
							ToolTipItem contents = superToolTipSetupArgs.Contents;
							contents.Text = contents.Text + "Location:" + appointment.Location + "\r\n";
						}
						ToolTipItem contents2 = superToolTipSetupArgs.Contents;
						contents2.Text = contents2.Text + "Content: " + appointment.Description + "\r\n";
						DataTable dataTable2 = FilterFieldControl.Properties.DataSource as DataTable;
						if (dataTable2.IsNotNull())
						{
							for (int num = 0; num < dataTable2.Rows.Count; num++)
							{
								DataRow dataRow = dataTable2.Rows[num];
								string text = Function.ToString(dataRow["FilterName"]);
								string text2 = Function.ToString(dataRow["FilterCode"]);
								if (!listMapped.Contains(text2) && !string.IsNullOrEmpty(text2))
								{
									ToolTipItem contents3 = superToolTipSetupArgs.Contents;
									contents3.Text = contents3.Text + text + ": " + Function.ToString(appointment.CustomFields[text2]) + "\r\n";
								}
							}
						}
						if (Function.ToString(appointment.LabelKey) == "T" && appointment.StatusKey != null)
						{
							ToolTipItem contents4 = superToolTipSetupArgs.Contents;
							contents4.Text = contents4.Text + "Status: " + ((Function.ParseInt(appointment.StatusKey) == 2) ? "<color=green>Completed</color>" : "<color=red>Uncompleted</color>") + "\r\n";
						}
						if (appointment.Start.Date != appointment.End.Date)
						{
							superToolTipSetupArgs.Footer.Text = appointment.Start.Date.ToShortDateString() + " " + appointment.Start.ToShortTimeString() + " - " + appointment.End.Date.ToShortDateString() + " " + appointment.End.ToShortTimeString();
						}
						else
						{
							superToolTipSetupArgs.Footer.Text = appointment.Start.ToShortTimeString() + " - " + appointment.End.ToShortTimeString();
						}
						string text3 = "";
						int assignedId = Function.ParseInt(appointment.CustomFields["AssignedBy"]);
						DataRow dataRow2 = _dtUser.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["USERID"]) == assignedId);
						if (dataRow2 != null)
						{
							text3 = Function.ToString(dataRow2["U_NAME"]);
						}
						if (!string.IsNullOrEmpty(text3))
						{
							ToolTipTitleItem footer = superToolTipSetupArgs.Footer;
							footer.Text = footer.Text + " By: <b>" + text3 + " </b> \r\n";
							ToolTipTitleItem footer2 = superToolTipSetupArgs.Footer;
							footer2.Text = footer2.Text + " ID: <b>" + clgCode + " </b> \r\n";
						}
						superToolTipSetupArgs.Footer.Font = new Font("Tahoma", Function.ParseFloat(7.25));
						superToolTip.Setup(superToolTipSetupArgs);
						superToolTip.AllowHtmlText = DefaultBoolean.True;
						e.SuperTip = superToolTip;
					}
				}
			}
			if (toolTipController1.ActiveObject is ResourceHeader)
			{
				Resource resource = ((ResourceHeader)toolTipController1.ActiveObject).Resource;
				if (resource == null)
				{
					return;
				}
				e.ToolTipType = ToolTipType.Standard;
				e.Rounded = true;
				e.Title = "Resource";
				e.ToolTip = resource.Caption;
			}
			if (!(toolTipController1.ActiveObject is DayHeader))
			{
				_ = toolTipController1.ActiveObject is TimeScaleHeader;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void schedulerControl1_CustomDrawDayHeader1(object sender, CustomDrawObjectEventArgs e)
	{
		if (schedulerControl1.ActiveViewType != SchedulerViewType.WorkWeek && schedulerControl1.ActiveViewType != SchedulerViewType.Week && schedulerControl1.ActiveViewType != SchedulerViewType.FullWeek)
		{
			_ = schedulerControl1.ActiveViewType;
			_ = 2;
		}
	}

	private void schedulerControl1_QueryWorkTime(object sender, QueryWorkTimeEventArgs e)
	{
		try
		{
			if (e.Interval.Start.DayOfWeek == DayOfWeek.Saturday && _isSatWorking)
			{
				e.WorkTime = new TimeOfDayInterval(TimeSpan.FromHours(_satStartWHour), TimeSpan.FromHours(_satEndWHour));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void schedulerControl1_AppointmentDrop(object sender, AppointmentDragEventArgs e)
	{
		_ = 2;
		try
		{
			_isChanging = true;
			if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
			{
				AppointmentDragEventArgs e2 = e;
				e2.Allow = await DropNormalAppointment(e.SourceAppointment, e.EditedAppointment, isCopy: true);
			}
			else if (e.EditedAppointment.IsRecurring)
			{
				AppointmentDragEventArgs e2 = e;
				e2.Allow = await DropRecurringAppointment(e.SourceAppointment, e.EditedAppointment);
			}
			else
			{
				AppointmentDragEventArgs e2 = e;
				e2.Allow = await DropNormalAppointment(e.SourceAppointment, e.EditedAppointment);
			}
			if (!e.Allow)
			{
				_isChanging = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<bool> DropNormalAppointment(Appointment srcAppointment, Appointment editedAppointment, bool isCopy = false)
	{
		_ = 1;
		try
		{
			new ResourceIdCollection();
			SchedulerCompatibility.Base64XmlObjectSerialization = false;
			DateTime start = editedAppointment.Start;
			DateTime start2 = srcAppointment.Start;
			DateTime end = editedAppointment.End;
			DateTime end2 = srcAppointment.End;
			int clgCode = Function.ParseInt(srcAppointment.CustomFields["ClgCode"]);
			string text = Function.ToString(srcAppointment.ResourceId);
			string text2 = Function.ToString(editedAppointment.ResourceId);
			string caption = schedulerStorage1.Resources.GetResourceById(editedAppointment.ResourceId).Caption;
			string caption2 = schedulerStorage1.Resources.GetResourceById(srcAppointment.ResourceId).Caption;
			_ = srcAppointment.ResourceIds;
			AppointmentResourceIdCollection resourceIds = editedAppointment.ResourceIds;
			string text3 = Function.ToString(srcAppointment.LabelKey);
			string text4 = "EVENT";
			switch (text3)
			{
			case "T":
				text4 = "TASK";
				break;
			case "V":
				text4 = "EVENT";
				break;
			case "E":
				text4 = "NOTE";
				break;
			}
			if (!isCopy)
			{
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("ClgCode", typeof(int));
				dataTable.Columns.Add("newStart", typeof(DateTime));
				dataTable.Columns.Add("newEnd", typeof(DateTime));
				dataTable.Columns.Add("startTime", typeof(int));
				dataTable.Columns.Add("endTime", typeof(int));
				dataTable.Columns.Add("resourceIdsInfo", typeof(string));
				dataTable.Columns.Add("GroupUser", typeof(string));
				dataTable.Columns.Add("GroupEmployee", typeof(string));
				DataRow dr = dataTable.NewRow();
				dr["ClgCode"] = clgCode;
				dr["newStart"] = start;
				dr["newEnd"] = end;
				dr["startTime"] = Function.ParseInt(WinFuntions.GetNumber(start.ToString("HH:mm"))[0]);
				dr["endTime"] = Function.ParseInt(WinFuntions.GetNumber(end.ToString("HH:mm"))[0]);
				if (Function.ToString(text2).StartsWith("E"))
				{
					dr["GroupEmployee"] = ((Function.ToString(text).Substring(1) == Function.ToString(text2).Substring(1)) ? "" : Function.ToString(text2).Substring(1));
					dr["GroupUser"] = "";
				}
				else
				{
					dr["GroupUser"] = ((Function.ToString(text).Substring(1) == Function.ToString(text2).Substring(1)) ? "" : Function.ToString(text2).Substring(1));
					dr["GroupEmployee"] = "";
				}
				string value = new AppointmentResourceIdCollectionXmlPersistenceHelper(resourceIds).ToXml();
				dr["ResourceIdsInfo"] = value;
				dataTable.Rows.Add(dr);
				string text5;
				if (Function.ToString(text) == Function.ToString(text2))
				{
					text5 = ((start2 == DateTime.MinValue) ? string.Format("Creating an {2} on {0:D} at {1:t}.", start.Date, start.TimeOfDay, text4) : string.Format("Moving the {5} \r\nscheduled on {0:D} Start at {1:T}\r\nto {2:dddd, dd MMM yyyy HH:mm:ss } and End at {3:T}\r\nto {4:dddd, dd MMM yyyy HH:mm:ss }.", start2.Date, start2.TimeOfDay, start, end2.TimeOfDay, end, text4));
				}
				else
				{
					text5 = ((start2 == DateTime.MinValue) ? string.Format("Creating an {2} on {0:D} at {1:t}.", start.Date, start.TimeOfDay, text4) : string.Format("Moving the {7} \r\nscheduled on {0:D} Start at {1:T}\r\nto {2:dddd, dd MMM yyyy HH:mm:ss } and End at {3:T}\r\nto {4:dddd, dd MMM yyyy HH:mm:ss }, From resource {5} to {6}", start2.Date, start2.TimeOfDay, start, end2.TimeOfDay, end, caption2, caption, text4));
				}
				if (XtraMessageBox.Show(text5 + " Proceed?", "Confirm the action", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					WinCommons._OpenProcessing();
					string text6 = await _httpClient.SubActivities.SaveActivityInterval(dataTable);
					if (string.IsNullOrEmpty(text6))
					{
						ShowSuccessMsg();
						DataTable dataTable2 = schedulerStorage1.Appointments.DataSource as DataTable;
						if (dataTable2.IsNotNull())
						{
							DataRow dataRow = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ClgCode"]) == clgCode);
							if (dataRow != null)
							{
								dataRow["Recontact"] = dr["newStart"];
								dataRow["endDate"] = dr["newEnd"];
								dataRow["BeginTime"] = dr["startTime"];
								dataRow["ENDTime"] = dr["endTime"];
								dataRow["GroupUser"] = dr["GroupUser"];
								dataRow["GroupEmployee"] = dr["GroupEmployee"];
							}
						}
						return true;
					}
					WinCommons.Error(Enums.MessageCode.None, text6);
					WinCommons._CloseProcessing();
				}
			}
			else
			{
				DateTime date = editedAppointment.Start.Date;
				DateTime date2 = editedAppointment.End.Date;
				string text7 = editedAppointment.Start.ToString("HH:mm");
				string text8 = editedAppointment.End.ToString("HH:mm");
				if (text3 != "T")
				{
					WinCommons.Warning(Enums.MessageCode.OnlyTaskCanBeCopied, "");
					return false;
				}
				DialogResult dialogResult = ((!(Function.ToString(text) != Function.ToString(text2))) ? XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCopyThisTask, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) : XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCopyThisTaskWithTime, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, caption2, caption));
				if (dialogResult == DialogResult.Yes)
				{
					DataTable dataTable3 = schedulerStorage1.Appointments.DataSource as DataTable;
					if (dataTable3.IsNotNull())
					{
						DataTable dataTable4 = dataTable3.Copy();
						if (dataTable4.ExistsColumn("ResName"))
						{
							dataTable4.Columns.Remove("ResName");
						}
						if (dataTable4.ExistsColumn("ResourceId"))
						{
							dataTable4.Columns.Remove("ResourceId");
						}
						if (dataTable4.ExistsColumn("startDateTime"))
						{
							dataTable4.Columns.Remove("startDateTime");
						}
						if (dataTable4.ExistsColumn("endDateTime"))
						{
							dataTable4.Columns.Remove("endDateTime");
						}
						if (dataTable4.ExistsColumn("LocationCode"))
						{
							dataTable4.Columns.Remove("LocationCode");
						}
						if (dataTable4.ExistsColumn("ProjectCode"))
						{
							dataTable4.Columns.Remove("ProjectCode");
						}
						DataRow dataRow2 = dataTable4.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ClgCode"]) == clgCode);
						if (copyTask || dataRow2 == null)
						{
							return false;
						}
						dataRow2["CntctDate"] = DateTime.Today;
						dataRow2["CntctTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
						dataRow2["AssignedBy"] = GlobalData.UserID;
						dataRow2["Recontact"] = date;
						dataRow2["BeginTime"] = Function.ParseInt(text7.Replace(":", ""));
						dataRow2["endDate"] = date2;
						dataRow2["ENDTime"] = Function.ParseInt(text8.Replace(":", ""));
						if (Function.ToString(text2).StartsWith("E"))
						{
							dataRow2["GroupEmployee"] = Function.ToString(text2).Substring(1);
							dataRow2["GroupUser"] = "";
						}
						else
						{
							dataRow2["GroupUser"] = Function.ToString(text2).Substring(1);
							dataRow2["GroupEmployee"] = "";
						}
						string value2 = new AppointmentResourceIdCollectionXmlPersistenceHelper(resourceIds).ToXml();
						dataRow2["ResourceIdsInfo"] = value2;
						await CopyTask(dataRow2, advancedCopy: true, keepSubTask: true, keepSubTaskOfSubTask: true, DueDateOptions.GetFromRow);
					}
				}
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return false;
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private void ShowSuccessMsg()
	{
	}

	private async Task<bool> DropRecurringAppointment(Appointment srcAppointment, Appointment editedAppointment)
	{
		_ = 2;
		try
		{
			DialogResult dialogResult = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmChangeOccurence, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo);
			SchedulerCompatibility.Base64XmlObjectSerialization = false;
			DateTime start = editedAppointment.Start;
			_ = srcAppointment.Start;
			DateTime end = editedAppointment.End;
			_ = srcAppointment.End;
			int clgCode = Function.ParseInt(srcAppointment.CustomFields["ClgCode"]);
			string text = Function.ToString(srcAppointment.ResourceId);
			string text2 = Function.ToString(editedAppointment.ResourceId);
			_ = srcAppointment.ResourceIds;
			AppointmentResourceIdCollection resourceIds = editedAppointment.ResourceIds;
			int recurrenceIndex = Function.ParseInt(editedAppointment.RecurrenceIndex);
			DataRow dataRow = _dtActivities.Copy().AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ClgCode"]) == clgCode);
			if (dataRow != null)
			{
				DataTable dataTable = dataRow.ToArray().CopyToDataTable();
				DataRow dataRow2 = dataTable.Rows[0];
				dataRow2["Recontact"] = start.Date;
				dataRow2["EndDate"] = end.Date;
				dataRow2["BeginTime"] = Function.ParseInt(WinFuntions.GetNumber(start.ToString("HH:mm"))[0]);
				dataRow2["ENDTime"] = Function.ParseInt(WinFuntions.GetNumber(end.ToString("HH:mm"))[0]);
				if (text != text2)
				{
					if (Function.ToString(text2).StartsWith("E"))
					{
						dataRow2["GroupEmployee"] = ((Function.ToString(text).Substring(1) == Function.ToString(text2).Substring(1)) ? "" : Function.ToString(text2).Substring(1));
						dataRow2["GroupUser"] = "";
					}
					else
					{
						dataRow2["GroupUser"] = ((Function.ToString(text).Substring(1) == Function.ToString(text2).Substring(1)) ? "" : Function.ToString(text2).Substring(1));
						dataRow2["GroupEmployee"] = "";
					}
					string value = new AppointmentResourceIdCollectionXmlPersistenceHelper(resourceIds).ToXml();
					dataRow2["ResourceIdsInfo"] = value;
				}
				dataTable.AcceptChanges();
				Function.ToString(dataRow2["BeginTime"]);
				Function.ParseInt(dataRow2["BeginTime"]);
				Function.ParseDateTimes(dataRow2["SeStartDat"]);
				new DataTable
				{
					Columns = 
					{
						{
							"DateChanged",
							typeof(string)
						},
						{
							"TimeChanged",
							typeof(string)
						},
						{
							"NewRecurrenceInfo",
							typeof(string)
						}
					}
				};
				if (dialogResult != DialogResult.Yes)
				{
					return false;
				}
				RecurrenceInfo recurrenceInfo = new RecurrenceInfo();
				if (!string.IsNullOrEmpty(Function.ToString(dataRow2["RecurrenceInfo"])))
				{
					recurrenceInfo.FromXml(Function.ToString(dataRow2["RecurrenceInfo"]));
					PatternReferenceXmlPersistenceHelper patternReferenceXmlPersistenceHelper = new PatternReferenceXmlPersistenceHelper(new PatternReference(recurrenceInfo.Id, recurrenceIndex));
					dataRow2["RecurrenceInfo"] = patternReferenceXmlPersistenceHelper.ToXml();
				}
				dataRow2["IsRemoved"] = "N";
				dataRow2["OrigDate"] = Function.ParseDateTimes(dataRow2["Recontact"]);
				dataRow2["CntctType"] = 3;
				dataRow2["UpdateDate"] = DateTime.Today;
				dataRow2["UpdateTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
				DataSet dataSet = new DataSet();
				dataSet.Tables.Add(dataTable);
				HttpResult httpResult;
				if (Function.ParseInt(dataRow2["SeriesNum"]) == 0)
				{
					dataRow2["ClgCode"] = 0;
					dataRow2["SeriesNum"] = clgCode;
					httpResult = await _httpClient.SubActivities.AddEvent(dataSet);
				}
				else
				{
					httpResult = await _httpClient.SubActivities.SaveThisEvent(dataSet);
				}
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					ShowSuccessMsg();
					await ReloadCalendar();
					return true;
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return false;
	}

	private async void schedulerControl1_AppointmentResized(object sender, AppointmentResizeEventArgs e)
	{
		_ = 1;
		try
		{
			_isChanging = true;
			if (e.EditedAppointment.IsRecurring)
			{
				e.Handled = true;
				AppointmentResizeEventArgs e2 = e;
				e2.Allow = await DropRecurringAppointment(e.SourceAppointment, e.EditedAppointment);
			}
			else
			{
				e.Handled = true;
				AppointmentResizeEventArgs e2 = e;
				e2.Allow = await DropNormalAppointment(e.SourceAppointment, e.EditedAppointment);
			}
			if (!e.Allow)
			{
				_isChanging = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void schedulerStorage1_AppointmentChanging(object sender, PersistentObjectCancelEventArgs e)
	{
		_ = 4;
		try
		{
			if (_isChanging)
			{
				return;
			}
			Appointment appointment = (Appointment)e.Object;
			if (appointment == null)
			{
				return;
			}
			int clgCode = Function.ParseInt(appointment.CustomFields["ClgCode"]);
			int recurrenceIndex = appointment.RecurrenceIndex;
			RecurrenceInfo recurrenceInfo = (RecurrenceInfo)appointment.RecurrenceInfo;
			DataTable dataTable = schedulerStorage1.Appointments.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				return;
			}
			DataRow row = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ClgCode"]) == clgCode);
			DataTable dataTable2 = dataTable.Clone();
			dataTable2.ImportRow(row);
			dataTable2.AcceptChanges();
			if (!dataTable2.IsNotNull())
			{
				return;
			}
			DataRow dataRow = dataTable2.Rows[0];
			Function.ToString(dataRow["inactive"]);
			Function.ToString(dataRow["Closed"]);
			Function.ParseInt(dataRow["status"]);
			string text = Function.ToString(dataRow["Action"]);
			Function.ToString(dataRow["Details"]);
			int num = Function.ParseInt(appointment.StatusKey);
			string text2 = Function.ToString(appointment.LabelKey);
			string value = Function.ToString(appointment.Subject);
			string value2 = Function.ToString(appointment.Description);
			if (text != text2)
			{
				WinCommons.Error(Enums.MessageCode.YouCannotChangeActivityAction, "");
				e.Cancel = true;
				return;
			}
			if (string.IsNullOrEmpty(value))
			{
				WinCommons.Error(Enums.MessageCode.InfoSubjectCanNotEmpty, "");
				e.Cancel = true;
				return;
			}
			dataRow["Action"] = text2;
			if (num == 2)
			{
				dataRow["CloseDate"] = DateTime.Now;
				dataRow["Closed"] = "Y";
			}
			else
			{
				dataRow["CloseDate"] = DateTime.Now;
				dataRow["Closed"] = "N";
			}
			dataRow["status"] = num;
			dataRow["Details"] = value;
			dataRow["Notes"] = value2;
			dataRow["UpdateDate"] = DateTime.Today;
			dataRow["UpdateTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dataTable2);
			if (!(text == "V"))
			{
				if (text == "T")
				{
					if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmChange, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						DataTable dataTable3 = dataSet.Tables[0];
						if (dataTable3.ExistsColumn("startDateTime"))
						{
							dataTable3.Columns.Remove("startDateTime");
						}
						if (dataTable3.ExistsColumn("endDateTime"))
						{
							dataTable3.Columns.Remove("endDateTime");
						}
						if (dataTable3.ExistsColumn("LocationCode"))
						{
							dataTable3.Columns.Remove("LocationCode");
						}
						if (dataTable3.ExistsColumn("ProjectCode"))
						{
							dataTable3.Columns.Remove("ProjectCode");
						}
						HttpResult httpResult = await _httpClient.SubActivities.SaveTask(dataSet);
						if (string.IsNullOrEmpty(httpResult.Message))
						{
							ShowSuccessMsg();
							return;
						}
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
						e.Cancel = true;
					}
					else
					{
						e.Cancel = true;
					}
				}
				else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmChange, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					DataTable dataTable4 = dataSet.Tables[0];
					if (dataTable4.ExistsColumn("startDateTime"))
					{
						dataTable4.Columns.Remove("startDateTime");
					}
					if (dataTable4.ExistsColumn("endDateTime"))
					{
						dataTable4.Columns.Remove("endDateTime");
					}
					if (dataTable4.ExistsColumn("LocationCode"))
					{
						dataTable4.Columns.Remove("LocationCode");
					}
					if (dataTable4.ExistsColumn("ProjectCode"))
					{
						dataTable4.Columns.Remove("ProjectCode");
					}
					HttpResult httpResult = await _httpClient.SubActivities.SaveNote(dataSet);
					if (string.IsNullOrEmpty(httpResult.Message))
					{
						ShowSuccessMsg();
						return;
					}
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					e.Cancel = true;
				}
				else
				{
					e.Cancel = true;
				}
			}
			else if (appointment.Type != AppointmentType.Normal)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmChangeOccurence, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					DateTime date = appointment.Start.Date;
					DateTime date2 = appointment.End.Date;
					string text3 = appointment.Start.ToString("HH:mm");
					string text4 = appointment.End.ToString("HH:mm");
					dataRow["Recontact"] = date;
					dataRow["BeginTime"] = Function.ParseInt(text3.Replace(":", ""));
					dataRow["EndDate"] = date2;
					dataRow["ENDTime"] = Function.ParseInt(text4.Replace(":", ""));
					dataRow["IsRemoved"] = "N";
					dataRow["OrigDate"] = appointment.Start.Date;
					if (Function.ParseInt(dataRow["SeriesNum"]) == 0)
					{
						dataRow["SeriesNum"] = clgCode;
					}
					if (appointment.Type != AppointmentType.ChangedOccurrence)
					{
						dataRow["ClgCode"] = 0;
					}
					else
					{
						dataRow["ClgCode"] = clgCode;
					}
					PatternReferenceXmlPersistenceHelper patternReferenceXmlPersistenceHelper = new PatternReferenceXmlPersistenceHelper(new PatternReference(recurrenceInfo.Id, recurrenceIndex));
					dataRow["RecurrenceInfo"] = patternReferenceXmlPersistenceHelper.ToXml();
					dataRow["CntctType"] = AppointmentType.ChangedOccurrence;
					dataRow["UpdateDate"] = DateTime.Today;
					dataRow["UpdateTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
					HttpResult httpResult = ((appointment.Type != AppointmentType.ChangedOccurrence) ? (await _httpClient.SubActivities.AddEvent(dataSet)) : (await _httpClient.SubActivities.SaveThisEvent(dataSet)));
					if (string.IsNullOrEmpty(httpResult.Message))
					{
						ShowSuccessMsg();
						return;
					}
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					e.Cancel = true;
				}
				else
				{
					e.Cancel = true;
				}
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmChange, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				HttpResult httpResult = await _httpClient.SubActivities.SaveAllEvent(dataSet);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					ShowSuccessMsg();
					return;
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				e.Cancel = true;
			}
			else
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void checkAllToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			schedulerStorage1.BeginUpdate();
			if (xtraTabControl1.SelectedTabPage == tab_User)
			{
				chk_Users.CheckAll();
			}
			else
			{
				chk_Employees.CheckAll();
			}
			schedulerStorage1.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void checkNoneToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			schedulerStorage1.BeginUpdate();
			if (xtraTabControl1.SelectedTabPage == tab_User)
			{
				chk_Users.UnCheckAll();
			}
			else
			{
				chk_Employees.UnCheckAll();
			}
			schedulerStorage1.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void schedulerControl1_MouseMove1(object sender, System.Windows.Forms.MouseEventArgs e)
	{
	}

	private void schedulerControl1_AppointmentViewInfoCustomizing(object sender, AppointmentViewInfoCustomizingEventArgs e)
	{
		try
		{
			if (e.ViewInfo.DisplayText == string.Empty)
			{
				e.ViewInfo.ToolTipText = $"Started at {e.ViewInfo.Appointment.Start:g}";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void schedulerControl1_CustomDrawResourceHeader(object sender, CustomDrawObjectEventArgs e)
	{
		try
		{
			SchedulerControl schedulerControl = sender as SchedulerControl;
			AppearanceObject headerCaption;
			Color color;
			int num;
			int num2;
			if (e.ObjectInfo is SchedulerHeader schedulerHeader)
			{
				headerCaption = schedulerHeader.Appearance.HeaderCaption;
				color = Color.FromArgb(Function.ParseInt(schedulerHeader.Resource.CustomFields["Color"]));
				if (schedulerControl != null)
				{
					SchedulerGroupType groupType = schedulerControl.ActiveView.GroupType;
					if (!(schedulerControl.ActiveView is WeekView) || groupType != SchedulerGroupType.Date)
					{
						if (schedulerControl.ActiveView is TimelineView)
						{
							num = ((groupType != SchedulerGroupType.None) ? 1 : 0);
							if (num != 0)
							{
								goto IL_0088;
							}
						}
						else
						{
							num = 0;
						}
						num2 = 1;
						goto IL_0089;
					}
					num = 1;
					goto IL_0088;
				}
			}
			goto IL_0141;
			IL_0088:
			num2 = 0;
			goto IL_0089;
			IL_0141:
			e.Handled = true;
			return;
			IL_0089:
			LinearGradientMode linearGradientMode = (LinearGradientMode)num2;
			e.Cache.FillRectangle(new LinearGradientBrush(e.Bounds, color, color, linearGradientMode), e.Bounds);
			Rectangle rectangle = Rectangle.Inflate(e.Bounds, -2, -2);
			e.Cache.FillRectangle(new LinearGradientBrush(e.Bounds, color, color, linearGradientMode), rectangle);
			StringFormat stringFormat = headerCaption.TextOptions.GetStringFormat(TextOptions.DefaultOptionsCenteredWithEllipsis);
			if (num != 0)
			{
				e.Cache.DrawVString(schedulerHeader.Caption, headerCaption.Font, headerCaption.GetForeBrush(e.Cache), rectangle, stringFormat, 270);
			}
			else
			{
				e.Cache.DrawString(schedulerHeader.Caption, headerCaption.Font, headerCaption.GetForeBrush(e.Cache), rectangle, stringFormat);
			}
			goto IL_0141;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void schedulerControl1_AllowAppointmentDragBetweenResources(object sender, AppointmentOperationEventArgs e)
	{
	}

	private void chk_Action_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)
	{
		try
		{
			schedulerStorage1.BeginUpdate();
			_filterString = "";
			if (chk_Action.DataSource is ArrayList arrayList)
			{
				object[] array = arrayList.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					CalendarAction calendarAction = (CalendarAction)array[i];
					if (calendarAction.Action == "N")
					{
						schedulerControl1.OptionsView.ShowOnlyResourceAppointments = calendarAction.IsChecked != true;
					}
					else if (calendarAction.IsChecked == true)
					{
						if (calendarAction.Action == "T")
						{
							_filterString += " OR ([Label] = 'T' AND [Status] = 2) ";
						}
						else if (calendarAction.Action == "U")
						{
							_filterString += " OR ([Label] = 'T' AND [Status] = 1) ";
						}
						else
						{
							_filterString = _filterString + " OR [Label] = '" + calendarAction.Action + "' ";
						}
					}
					else if (calendarAction.Action == "T")
					{
						_filterString += "AND ([Label] = 'T' AND [Status] != 2) ";
					}
					else if (calendarAction.Action == "U")
					{
						_filterString += "AND([Label] = 'T' AND [Status] != 1) ";
					}
					else
					{
						_filterString = _filterString + "AND [Label] != '" + calendarAction.Action + "' ";
					}
				}
			}
			if (!string.IsNullOrEmpty(_filterString) && _filterString.Length > 3)
			{
				schedulerStorage1.Appointments.Filter = _filterString.Substring(3);
			}
			else
			{
				schedulerStorage1.Appointments.Filter = "";
			}
			schedulerStorage1.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void schedulerStorage1_ResourceCollectionLoaded(object sender, EventArgs e)
	{
		try
		{
			foreach (Resource item in schedulerStorage1.Resources.Items)
			{
				if (item.ColorValue != null)
				{
					Color color = ColorTranslator.FromHtml(item.ColorValue);
					item.SetColor(color);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void SchedulerStorage_FetchAppointments(object sender, FetchAppointmentsEventArgs e)
	{
		try
		{
			if (_formLoading)
			{
				return;
			}
			DateTime start = e.Interval.Start;
			DateTime end = e.Interval.End;
			if (start < _lastFetchedInterval.Start || end > _lastFetchedInterval.End)
			{
				int num = 0;
				switch (schedulerControl1.ActiveViewType)
				{
				case SchedulerViewType.Day:
					num = 21;
					break;
				case SchedulerViewType.Week:
					num = 14;
					break;
				case SchedulerViewType.WorkWeek:
					num = 16;
					break;
				case SchedulerViewType.FullWeek:
					num = 14;
					break;
				case SchedulerViewType.Month:
					num = 30;
					break;
				case SchedulerViewType.Timeline:
					num = 30;
					break;
				case SchedulerViewType.Gantt:
					num = 30;
					break;
				case SchedulerViewType.Agenda:
					num = 21;
					break;
				}
				TimeSpan timeSpan = TimeSpan.FromDays(num);
				_lastFetchedInterval = new TimeInterval(start - timeSpan, end + timeSpan);
				await RefreshMainCalendarData(_lastFetchedInterval.Start, _lastFetchedInterval.End);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task RefreshMainCalendarData(DateTime startDate, DateTime endDate)
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			schedulerControl1.BeginUpdate();
			if (!_dtSearch.IsNotNull())
			{
				_dtSearch = new DataTable();
				_dtSearch.Columns.Add("userCode", typeof(string));
				_dtSearch.Columns.Add("startDate", typeof(DateTime));
				_dtSearch.Columns.Add("endDate", typeof(DateTime));
			}
			_dtSearch.Rows.Clear();
			_dtSearch.Rows.Add(GlobalData.UserCodeLogin, startDate, endDate);
			_dtActivities = await _httpClient.SubActivities.GetDocByInterval(_dtSearch);
			if (_dtActivities.HasColumn())
			{
				schedulerStorage1.Appointments.Clear();
				schedulerStorage1.Appointments.DataSource = _dtActivities.Copy();
				await PushValueToFilterValueControl();
				DoFilter();
			}
			schedulerControl1.EndUpdate();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private async void frm_ActivityCalendar_Shown(object sender, EventArgs e)
	{
		try
		{
			_formLoading = false;
			await REFRESH_DATA();
			if (ContextMenuStrip != null)
			{
				ContextMenuStrip.Opening += ContextMenuStripOnOpening;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ContextMenuStripOnOpening(object sender, CancelEventArgs e)
	{
		try
		{
			e.Cancel = schedulerControl1.Bounds.Contains(schedulerControl1.PointToClient(Control.MousePosition));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void schedulerControl1_CustomDrawDayHeader(object sender, CustomDrawObjectEventArgs e)
	{
	}

	private void schedulerStorage1_FilterAppointment(object sender, PersistentObjectCancelEventArgs e)
	{
	}

	private async void schedulerStorage1_AppointmentsChanged(object sender, PersistentObjectsEventArgs e)
	{
		try
		{
			if (!_isFilter)
			{
				await PushValueToFilterValueControl();
				DoFilter();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			_isChanging = false;
		}
	}

	private void schedulerControl1_CustomDrawAppointment(object sender, CustomDrawObjectEventArgs e)
	{
	}

	private void schedulerControl1_InitAppointmentDisplayText(object sender, AppointmentDisplayTextEventArgs e)
	{
		_ = e.ViewInfo is VerticalAppointmentViewInfo;
	}

	private void chk_Users_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)
	{
		try
		{
			schedulerStorage1.BeginUpdate();
			ResourceStorage resources = schedulerStorage1.Resources;
			CheckedListBoxControl checkedListBoxControl = (CheckedListBoxControl)sender;
			if (checkedListBoxControl != null)
			{
				foreach (CheckedListBoxItem item in checkedListBoxControl.Items)
				{
					Resource resourceById = resources.GetResourceById(item.Value);
					if (item.CheckState == CheckState.Checked)
					{
						resourceById.Visible = true;
					}
					else
					{
						resourceById.Visible = false;
					}
				}
			}
			schedulerStorage1.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void xtraTabControl1_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		try
		{
			if (e.Page == tab_User)
			{
				searchControl1.Client = chk_Users;
			}
			else
			{
				searchControl1.Client = chk_Employees;
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
		DevExpress.XtraScheduler.TimeRuler value = new DevExpress.XtraScheduler.TimeRuler();
		DevExpress.XtraScheduler.TimeRuler value2 = new DevExpress.XtraScheduler.TimeRuler();
		DevExpress.XtraScheduler.TimeRuler value3 = new DevExpress.XtraScheduler.TimeRuler();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_ActivityCalendar));
		DevExpress.Utils.SuperToolTip superToolTip = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip2 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem2 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem2 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip3 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem3 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem3 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip4 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem4 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem4 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip5 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem5 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem5 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip6 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem6 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem6 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip7 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem7 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem7 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip8 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem8 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem8 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip9 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem9 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem9 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip10 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem10 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem10 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip11 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem11 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem11 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip12 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem12 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem12 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip13 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem13 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem13 = new DevExpress.Utils.ToolTipItem();
		DevExpress.Utils.SuperToolTip superToolTip14 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipTitleItem toolTipTitleItem14 = new DevExpress.Utils.ToolTipTitleItem();
		DevExpress.Utils.ToolTipItem toolTipItem14 = new DevExpress.Utils.ToolTipItem();
		this.schedulerStorage1 = new DevExpress.XtraScheduler.SchedulerStorage(this.components);
		this.schedulerControl1 = new DevExpress.XtraScheduler.SchedulerControl();
		this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
		this.navigatorBar1 = new DevExpress.XtraScheduler.UI.NavigatorBar();
		this.navigateViewBackwardItem1 = new DevExpress.XtraScheduler.UI.NavigateViewBackwardItem();
		this.navigateViewForwardItem1 = new DevExpress.XtraScheduler.UI.NavigateViewForwardItem();
		this.gotoTodayItem1 = new DevExpress.XtraScheduler.UI.GotoTodayItem();
		this.viewZoomInItem1 = new DevExpress.XtraScheduler.UI.ViewZoomInItem();
		this.viewZoomOutItem1 = new DevExpress.XtraScheduler.UI.ViewZoomOutItem();
		this.arrangeBar1 = new DevExpress.XtraScheduler.UI.ArrangeBar();
		this.switchToDayViewItem1 = new DevExpress.XtraScheduler.UI.SwitchToDayViewItem();
		this.switchToWorkWeekViewItem1 = new DevExpress.XtraScheduler.UI.SwitchToWorkWeekViewItem();
		this.switchToWeekViewItem1 = new DevExpress.XtraScheduler.UI.SwitchToWeekViewItem();
		this.switchToFullWeekViewItem1 = new DevExpress.XtraScheduler.UI.SwitchToFullWeekViewItem();
		this.switchToMonthViewItem1 = new DevExpress.XtraScheduler.UI.SwitchToMonthViewItem();
		this.switchToTimelineViewItem1 = new DevExpress.XtraScheduler.UI.SwitchToTimelineViewItem();
		this.switchToGanttViewItem1 = new DevExpress.XtraScheduler.UI.SwitchToGanttViewItem();
		this.groupByBar1 = new DevExpress.XtraScheduler.UI.GroupByBar();
		this.groupByNoneItem1 = new DevExpress.XtraScheduler.UI.GroupByNoneItem();
		this.groupByDateItem1 = new DevExpress.XtraScheduler.UI.GroupByDateItem();
		this.groupByResourceItem1 = new DevExpress.XtraScheduler.UI.GroupByResourceItem();
		this.timeScaleBar1 = new DevExpress.XtraScheduler.UI.TimeScaleBar();
		this.switchTimeScalesItem1 = new DevExpress.XtraScheduler.UI.SwitchTimeScalesItem();
		this.changeScaleWidthItem1 = new DevExpress.XtraScheduler.UI.ChangeScaleWidthItem();
		this.repositoryItemSpinEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
		this.switchTimeScalesCaptionItem1 = new DevExpress.XtraScheduler.UI.SwitchTimeScalesCaptionItem();
		this.layoutBar1 = new DevExpress.XtraScheduler.UI.LayoutBar();
		this.switchCompressWeekendItem1 = new DevExpress.XtraScheduler.UI.SwitchCompressWeekendItem();
		this.switchShowWorkTimeOnlyItem1 = new DevExpress.XtraScheduler.UI.SwitchShowWorkTimeOnlyItem();
		this.switchCellsAutoHeightItem1 = new DevExpress.XtraScheduler.UI.SwitchCellsAutoHeightItem();
		this.changeSnapToCellsUIItem1 = new DevExpress.XtraScheduler.UI.ChangeSnapToCellsUIItem();
		this.optionsBar1 = new DevExpress.XtraScheduler.UI.OptionsBar();
		this.changeAppointmentStatusItem1 = new DevExpress.XtraScheduler.UI.ChangeAppointmentStatusItem();
		this.changeAppointmentLabelItem1 = new DevExpress.XtraScheduler.UI.ChangeAppointmentLabelItem();
		this.toggleRecurrenceItem1 = new DevExpress.XtraScheduler.UI.ToggleRecurrenceItem();
		this.changeAppointmentReminderItem1 = new DevExpress.XtraScheduler.UI.ChangeAppointmentReminderItem();
		this.repositoryItemDuration1 = new DevExpress.XtraScheduler.UI.RepositoryItemDuration();
		this.commonBar1 = new DevExpress.XtraScheduler.UI.CommonBar();
		this.openScheduleItem1 = new DevExpress.XtraScheduler.UI.OpenScheduleItem();
		this.saveScheduleItem1 = new DevExpress.XtraScheduler.UI.SaveScheduleItem();
		this.printBar1 = new DevExpress.XtraScheduler.UI.PrintBar();
		this.printPreviewItem1 = new DevExpress.XtraScheduler.UI.PrintPreviewItem();
		this.printItem1 = new DevExpress.XtraScheduler.UI.PrintItem();
		this.printPageSetupItem1 = new DevExpress.XtraScheduler.UI.PrintPageSetupItem();
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.newAppointmentItem1 = new DevExpress.XtraScheduler.UI.NewAppointmentItem();
		this.newRecurringAppointmentItem1 = new DevExpress.XtraScheduler.UI.NewRecurringAppointmentItem();
		this.editAppointmentQueryItem1 = new DevExpress.XtraScheduler.UI.EditAppointmentQueryItem();
		this.editOccurrenceUICommandItem1 = new DevExpress.XtraScheduler.UI.EditOccurrenceUICommandItem();
		this.editSeriesUICommandItem1 = new DevExpress.XtraScheduler.UI.EditSeriesUICommandItem();
		this.deleteAppointmentsItem1 = new DevExpress.XtraScheduler.UI.DeleteAppointmentsItem();
		this.deleteOccurrenceItem1 = new DevExpress.XtraScheduler.UI.DeleteOccurrenceItem();
		this.deleteSeriesItem1 = new DevExpress.XtraScheduler.UI.DeleteSeriesItem();
		this.splitAppointmentItem1 = new DevExpress.XtraScheduler.UI.SplitAppointmentItem();
		this.toolTipController1 = new DevExpress.Utils.ToolTipController(this.components);
		this.schedulerBarController1 = new DevExpress.XtraScheduler.UI.SchedulerBarController(this.components);
		this.dateNavigator1 = new DevExpress.XtraScheduler.DateNavigator();
		this.labelControl1 = new Apzon.Controls.Controls.LabelLine();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.xtraTabControl2 = new DevExpress.XtraTab.XtraTabControl();
		this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
		this.chk_Action = new DevExpress.XtraEditors.CheckedListBoxControl();
		this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItemFilter1 = new Apzon.Forms.UserControls.ToolStripMenuItemFilter(this.components);
		this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.txtFilterValue = new Apzon.Forms.UserControls.FilterControl.FilterValueControlEx();
		this.txtFilterField = new Apzon.Controls.Editor.LookUpEditEx();
		this.searchControl1 = new DevExpress.XtraEditors.SearchControl();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_User = new Apzon.Controls.Controls.XtraTabPageEx();
		this.chk_Users = new DevExpress.XtraEditors.CheckedListBoxControl();
		this.tab_Employee = new Apzon.Controls.Controls.XtraTabPageEx();
		this.chk_Employees = new DevExpress.XtraEditors.CheckedListBoxControl();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.checkAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.checkNoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		((System.ComponentModel.ISupportInitialize)this.schedulerStorage1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.schedulerControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemSpinEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDuration1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.schedulerBarController1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateNavigator1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateNavigator1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl2).BeginInit();
		this.xtraTabControl2.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_Action).BeginInit();
		this.xtraTabPage2.SuspendLayout();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtFilterValue.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFilterField.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.searchControl1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.tab_User.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_Users).BeginInit();
		this.tab_Employee.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_Employees).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		base.SuspendLayout();
		this.schedulerStorage1.Appointments.CustomFieldMappings.Add(new DevExpress.XtraScheduler.AppointmentCustomFieldMapping("AddrName", "AddrName"));
		this.schedulerStorage1.Appointments.CustomFieldMappings.Add(new DevExpress.XtraScheduler.AppointmentCustomFieldMapping("AddrType", "AddrType"));
		this.schedulerStorage1.Appointments.ResourceSharing = true;
		this.schedulerStorage1.Resources.ColorSaving = DevExpress.XtraScheduler.ColorSavingType.ArgbColor;
		this.schedulerStorage1.AppointmentChanging += new DevExpress.XtraScheduler.PersistentObjectCancelEventHandler(this.schedulerStorage1_AppointmentChanging);
		this.schedulerStorage1.AppointmentsChanged += new DevExpress.XtraScheduler.PersistentObjectsEventHandler(this.schedulerStorage1_AppointmentsChanged);
		this.schedulerStorage1.FilterAppointment += new DevExpress.XtraScheduler.PersistentObjectCancelEventHandler(this.schedulerStorage1_FilterAppointment);
		this.schedulerStorage1.ResourceCollectionLoaded += new System.EventHandler(this.schedulerStorage1_ResourceCollectionLoaded);
		this.schedulerStorage1.FilterResource += new DevExpress.XtraScheduler.PersistentObjectCancelEventHandler(this.schedulerStorage1_FilterResource);
		this.schedulerControl1.Appearance.HeaderCaption.BackColor = System.Drawing.Color.Aqua;
		this.schedulerControl1.Appearance.HeaderCaption.Options.UseBackColor = true;
		this.schedulerControl1.DataStorage = this.schedulerStorage1;
		this.schedulerControl1.DateNavigationBar.Visible = false;
		this.schedulerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.schedulerControl1.Location = new System.Drawing.Point(0, 0);
		this.schedulerControl1.MenuManager = this.barManager1;
		this.schedulerControl1.Name = "schedulerControl1";
		this.schedulerControl1.OptionsBehavior.SelectOnRightClick = true;
		this.schedulerControl1.OptionsCustomization.AllowAppointmentCopy = DevExpress.XtraScheduler.UsedAppointmentType.NonRecurring;
		this.schedulerControl1.OptionsCustomization.AllowAppointmentCreate = DevExpress.XtraScheduler.UsedAppointmentType.None;
		this.schedulerControl1.OptionsCustomization.AllowAppointmentDelete = DevExpress.XtraScheduler.UsedAppointmentType.None;
		this.schedulerControl1.OptionsCustomization.AllowAppointmentDragBetweenResources = DevExpress.XtraScheduler.UsedAppointmentType.NonRecurring;
		this.schedulerControl1.OptionsCustomization.AllowAppointmentMultiSelect = false;
		this.schedulerControl1.OptionsCustomization.AllowDisplayAppointmentDependencyForm = DevExpress.XtraScheduler.AllowDisplayAppointmentDependencyForm.Never;
		this.schedulerControl1.OptionsCustomization.AllowDisplayAppointmentFlyout = false;
		this.schedulerControl1.OptionsCustomization.AllowDisplayAppointmentForm = DevExpress.XtraScheduler.AllowDisplayAppointmentForm.Never;
		this.schedulerControl1.OptionsView.NavigationButtons.Visibility = DevExpress.XtraScheduler.NavigationButtonVisibility.Never;
		this.schedulerControl1.OptionsView.UseInplaceEditor2016 = DevExpress.Utils.DefaultBoolean.True;
		this.schedulerControl1.Size = new System.Drawing.Size(1189, 676);
		this.schedulerControl1.Start = new System.DateTime(2016, 10, 31, 0, 0, 0, 0);
		this.schedulerControl1.TabIndex = 0;
		this.schedulerControl1.Text = "schedulerControl1";
		this.schedulerControl1.ToolTipController = this.toolTipController1;
		this.schedulerControl1.Views.AgendaView.AppointmentDisplayOptions.AllowMultiLineSubject = true;
		this.schedulerControl1.Views.AgendaView.AppointmentDisplayOptions.ShowResource = true;
		this.schedulerControl1.Views.DayView.AllDayAreaScrollBarVisible = true;
		this.schedulerControl1.Views.DayView.Appearance.AdditionalHeaderCaption.Options.UseBackColor = true;
		this.schedulerControl1.Views.DayView.ResourcesPerPage = 3;
		this.schedulerControl1.Views.DayView.TimeRulers.Add(value);
		this.schedulerControl1.Views.FullWeekView.Enabled = true;
		this.schedulerControl1.Views.FullWeekView.TimeRulers.Add(value2);
		this.schedulerControl1.Views.WorkWeekView.TimeRulers.Add(value3);
		this.schedulerControl1.QueryWorkTime += new DevExpress.XtraScheduler.QueryWorkTimeEventHandler(this.schedulerControl1_QueryWorkTime);
		this.schedulerControl1.InitAppointmentDisplayText += new DevExpress.XtraScheduler.AppointmentDisplayTextEventHandler(this.schedulerControl1_InitAppointmentDisplayText);
		this.schedulerControl1.AppointmentViewInfoCustomizing += new DevExpress.XtraScheduler.AppointmentViewInfoCustomizingEventHandler(this.schedulerControl1_AppointmentViewInfoCustomizing);
		this.schedulerControl1.AllowAppointmentDragBetweenResources += new DevExpress.XtraScheduler.AppointmentOperationEventHandler(this.schedulerControl1_AllowAppointmentDragBetweenResources);
		this.schedulerControl1.AppointmentDrop += new DevExpress.XtraScheduler.AppointmentDragEventHandler(this.schedulerControl1_AppointmentDrop);
		this.schedulerControl1.AppointmentResized += new DevExpress.XtraScheduler.AppointmentResizeEventHandler(this.schedulerControl1_AppointmentResized);
		this.schedulerControl1.PopupMenuShowing += new DevExpress.XtraScheduler.PopupMenuShowingEventHandler(this.schedulerControl1_PopupMenuShowing);
		this.schedulerControl1.CustomDrawAppointment += new DevExpress.XtraScheduler.CustomDrawObjectEventHandler(this.schedulerControl1_CustomDrawAppointment);
		this.schedulerControl1.CustomDrawDayHeader += new DevExpress.XtraScheduler.CustomDrawObjectEventHandler(this.schedulerControl1_CustomDrawDayHeader);
		this.schedulerControl1.CustomDrawResourceHeader += new DevExpress.XtraScheduler.CustomDrawObjectEventHandler(this.schedulerControl1_CustomDrawResourceHeader);
		this.schedulerControl1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.schedulerControl1_MouseDoubleClick);
		this.barManager1.AllowCustomization = false;
		this.barManager1.AllowMoveBarOnToolbar = false;
		this.barManager1.AllowShowToolbarsPopup = false;
		this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[8] { this.navigatorBar1, this.arrangeBar1, this.groupByBar1, this.timeScaleBar1, this.layoutBar1, this.optionsBar1, this.commonBar1, this.printBar1 });
		this.barManager1.DockControls.Add(this.barDockControlTop);
		this.barManager1.DockControls.Add(this.barDockControlBottom);
		this.barManager1.DockControls.Add(this.barDockControlLeft);
		this.barManager1.DockControls.Add(this.barDockControlRight);
		this.barManager1.Form = this;
		this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[40]
		{
			this.newAppointmentItem1, this.newRecurringAppointmentItem1, this.navigateViewBackwardItem1, this.navigateViewForwardItem1, this.gotoTodayItem1, this.viewZoomInItem1, this.viewZoomOutItem1, this.switchToDayViewItem1, this.switchToWorkWeekViewItem1, this.switchToWeekViewItem1,
			this.switchToFullWeekViewItem1, this.switchToMonthViewItem1, this.switchToTimelineViewItem1, this.switchToGanttViewItem1, this.groupByNoneItem1, this.groupByDateItem1, this.groupByResourceItem1, this.switchTimeScalesItem1, this.changeScaleWidthItem1, this.switchTimeScalesCaptionItem1,
			this.switchCompressWeekendItem1, this.switchShowWorkTimeOnlyItem1, this.switchCellsAutoHeightItem1, this.changeSnapToCellsUIItem1, this.editAppointmentQueryItem1, this.editOccurrenceUICommandItem1, this.editSeriesUICommandItem1, this.deleteAppointmentsItem1, this.deleteOccurrenceItem1, this.deleteSeriesItem1,
			this.splitAppointmentItem1, this.changeAppointmentStatusItem1, this.changeAppointmentLabelItem1, this.toggleRecurrenceItem1, this.changeAppointmentReminderItem1, this.openScheduleItem1, this.saveScheduleItem1, this.printPreviewItem1, this.printItem1, this.printPageSetupItem1
		});
		this.barManager1.MaxItemId = 40;
		this.barManager1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemSpinEdit1, this.repositoryItemDuration1 });
		this.navigatorBar1.Control = this.schedulerControl1;
		this.navigatorBar1.DockCol = 1;
		this.navigatorBar1.DockRow = 0;
		this.navigatorBar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		this.navigatorBar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[5]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.navigateViewBackwardItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.navigateViewForwardItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.gotoTodayItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.viewZoomInItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.viewZoomOutItem1)
		});
		this.navigateViewBackwardItem1.Id = 2;
		this.navigateViewBackwardItem1.Name = "navigateViewBackwardItem1";
		this.navigateViewForwardItem1.Id = 3;
		this.navigateViewForwardItem1.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("navigateViewForwardItem1.ImageOptions.SvgImage");
		this.navigateViewForwardItem1.Name = "navigateViewForwardItem1";
		this.gotoTodayItem1.Id = 4;
		this.gotoTodayItem1.Name = "gotoTodayItem1";
		this.viewZoomInItem1.Id = 5;
		this.viewZoomInItem1.Name = "viewZoomInItem1";
		this.viewZoomOutItem1.Id = 6;
		this.viewZoomOutItem1.Name = "viewZoomOutItem1";
		this.arrangeBar1.Control = this.schedulerControl1;
		this.arrangeBar1.DockCol = 3;
		this.arrangeBar1.DockRow = 0;
		this.arrangeBar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		this.arrangeBar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[7]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.switchToDayViewItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.switchToWorkWeekViewItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.switchToWeekViewItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.switchToFullWeekViewItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.switchToMonthViewItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.switchToTimelineViewItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.switchToGanttViewItem1)
		});
		this.switchToDayViewItem1.Id = 7;
		this.switchToDayViewItem1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("switchToDayViewItem1.ImageOptions.Image");
		this.switchToDayViewItem1.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("switchToDayViewItem1.ImageOptions.LargeImage");
		this.switchToDayViewItem1.Name = "switchToDayViewItem1";
		toolTipTitleItem.Text = "&Day View";
		toolTipItem.LeftIndent = 6;
		toolTipItem.Text = "Switch to the Day view. The most detailed view of activities for a specific day(s).";
		superToolTip.Items.Add(toolTipTitleItem);
		superToolTip.Items.Add(toolTipItem);
		this.switchToDayViewItem1.SuperTip = superToolTip;
		this.switchToWorkWeekViewItem1.Id = 8;
		this.switchToWorkWeekViewItem1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("switchToWorkWeekViewItem1.ImageOptions.Image");
		this.switchToWorkWeekViewItem1.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("switchToWorkWeekViewItem1.ImageOptions.LargeImage");
		this.switchToWorkWeekViewItem1.Name = "switchToWorkWeekViewItem1";
		this.switchToWeekViewItem1.Id = 9;
		this.switchToWeekViewItem1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("switchToWeekViewItem1.ImageOptions.Image");
		this.switchToWeekViewItem1.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("switchToWeekViewItem1.ImageOptions.LargeImage");
		this.switchToWeekViewItem1.Name = "switchToWeekViewItem1";
		toolTipTitleItem2.Text = "&Week View";
		toolTipItem2.LeftIndent = 6;
		toolTipItem2.Text = "Switch to the Week view. Arranges activities for a particular week in a compact form.";
		superToolTip2.Items.Add(toolTipTitleItem2);
		superToolTip2.Items.Add(toolTipItem2);
		this.switchToWeekViewItem1.SuperTip = superToolTip2;
		this.switchToFullWeekViewItem1.Id = 10;
		this.switchToFullWeekViewItem1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("switchToFullWeekViewItem1.ImageOptions.Image");
		this.switchToFullWeekViewItem1.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("switchToFullWeekViewItem1.ImageOptions.LargeImage");
		this.switchToFullWeekViewItem1.Name = "switchToFullWeekViewItem1";
		toolTipTitleItem3.Text = "&Full Week View";
		toolTipItem3.LeftIndent = 6;
		toolTipItem3.Text = "Switch to the Full Week View. Displays activities for the entire week.";
		superToolTip3.Items.Add(toolTipTitleItem3);
		superToolTip3.Items.Add(toolTipItem3);
		this.switchToFullWeekViewItem1.SuperTip = superToolTip3;
		this.switchToMonthViewItem1.Id = 11;
		this.switchToMonthViewItem1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("switchToMonthViewItem1.ImageOptions.Image");
		this.switchToMonthViewItem1.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("switchToMonthViewItem1.ImageOptions.LargeImage");
		this.switchToMonthViewItem1.Name = "switchToMonthViewItem1";
		this.switchToTimelineViewItem1.Id = 12;
		this.switchToTimelineViewItem1.Name = "switchToTimelineViewItem1";
		toolTipTitleItem4.Text = "&Timeline View";
		toolTipItem4.LeftIndent = 6;
		toolTipItem4.Text = "Switch to the Timeline view. Plots activities in relation to time.";
		superToolTip4.Items.Add(toolTipTitleItem4);
		superToolTip4.Items.Add(toolTipItem4);
		this.switchToTimelineViewItem1.SuperTip = superToolTip4;
		this.switchToGanttViewItem1.Id = 13;
		this.switchToGanttViewItem1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("switchToGanttViewItem1.ImageOptions.Image");
		this.switchToGanttViewItem1.ImageOptions.LargeImage = (System.Drawing.Image)resources.GetObject("switchToGanttViewItem1.ImageOptions.LargeImage");
		this.switchToGanttViewItem1.Name = "switchToGanttViewItem1";
		toolTipTitleItem5.Text = "&Gantt View";
		toolTipItem5.LeftIndent = 6;
		toolTipItem5.Text = "Switch to the Gantt View. Project management view that shows activities and their dependencies in relation to time.";
		superToolTip5.Items.Add(toolTipTitleItem5);
		superToolTip5.Items.Add(toolTipItem5);
		this.switchToGanttViewItem1.SuperTip = superToolTip5;
		this.groupByBar1.Control = this.schedulerControl1;
		this.groupByBar1.DockCol = 2;
		this.groupByBar1.DockRow = 0;
		this.groupByBar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		this.groupByBar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.groupByNoneItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.groupByDateItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.groupByResourceItem1)
		});
		this.groupByNoneItem1.Id = 14;
		this.groupByNoneItem1.Name = "groupByNoneItem1";
		toolTipTitleItem6.Text = "&Group by None";
		toolTipItem6.LeftIndent = 6;
		toolTipItem6.Text = "Ungroup activities.";
		superToolTip6.Items.Add(toolTipTitleItem6);
		superToolTip6.Items.Add(toolTipItem6);
		this.groupByNoneItem1.SuperTip = superToolTip6;
		this.groupByDateItem1.Id = 15;
		this.groupByDateItem1.Name = "groupByDateItem1";
		toolTipTitleItem7.Text = "&Group by Date";
		toolTipItem7.LeftIndent = 6;
		toolTipItem7.Text = "Group activities by date.";
		superToolTip7.Items.Add(toolTipTitleItem7);
		superToolTip7.Items.Add(toolTipItem7);
		this.groupByDateItem1.SuperTip = superToolTip7;
		this.groupByResourceItem1.Id = 16;
		this.groupByResourceItem1.Name = "groupByResourceItem1";
		toolTipTitleItem8.Text = "&Group by users";
		toolTipItem8.LeftIndent = 6;
		toolTipItem8.Text = "Group activities by users";
		superToolTip8.Items.Add(toolTipTitleItem8);
		superToolTip8.Items.Add(toolTipItem8);
		this.groupByResourceItem1.SuperTip = superToolTip8;
		this.timeScaleBar1.Control = this.schedulerControl1;
		this.timeScaleBar1.DockCol = 5;
		this.timeScaleBar1.DockRow = 0;
		this.timeScaleBar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		this.timeScaleBar1.FloatLocation = new System.Drawing.Point(1163, 237);
		this.timeScaleBar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.switchTimeScalesItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeScaleWidthItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.switchTimeScalesCaptionItem1)
		});
		this.switchTimeScalesItem1.Id = 17;
		this.switchTimeScalesItem1.Name = "switchTimeScalesItem1";
		this.changeScaleWidthItem1.Edit = this.repositoryItemSpinEdit1;
		this.changeScaleWidthItem1.Id = 18;
		this.changeScaleWidthItem1.Name = "changeScaleWidthItem1";
		this.changeScaleWidthItem1.UseCommandCaption = true;
		this.repositoryItemSpinEdit1.AutoHeight = false;
		this.repositoryItemSpinEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemSpinEdit1.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
		this.repositoryItemSpinEdit1.MaxValue = new decimal(new int[4] { 200, 0, 0, 0 });
		this.repositoryItemSpinEdit1.MinValue = new decimal(new int[4] { 10, 0, 0, 0 });
		this.repositoryItemSpinEdit1.Name = "repositoryItemSpinEdit1";
		this.switchTimeScalesCaptionItem1.Id = 19;
		this.switchTimeScalesCaptionItem1.Name = "switchTimeScalesCaptionItem1";
		this.layoutBar1.Control = this.schedulerControl1;
		this.layoutBar1.DockCol = 6;
		this.layoutBar1.DockRow = 0;
		this.layoutBar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		this.layoutBar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.switchCompressWeekendItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.switchShowWorkTimeOnlyItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.switchCellsAutoHeightItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeSnapToCellsUIItem1)
		});
		this.switchCompressWeekendItem1.Id = 20;
		this.switchCompressWeekendItem1.Name = "switchCompressWeekendItem1";
		this.switchShowWorkTimeOnlyItem1.Id = 21;
		this.switchShowWorkTimeOnlyItem1.Name = "switchShowWorkTimeOnlyItem1";
		this.switchCellsAutoHeightItem1.Id = 22;
		this.switchCellsAutoHeightItem1.Name = "switchCellsAutoHeightItem1";
		toolTipTitleItem9.Text = "Cell Auto Height";
		toolTipItem9.LeftIndent = 6;
		toolTipItem9.Text = "Enable a time cell to automatically adjust its size to accommodate activities it contains.";
		superToolTip9.Items.Add(toolTipTitleItem9);
		superToolTip9.Items.Add(toolTipItem9);
		this.switchCellsAutoHeightItem1.SuperTip = superToolTip9;
		this.changeSnapToCellsUIItem1.Id = 23;
		this.changeSnapToCellsUIItem1.Name = "changeSnapToCellsUIItem1";
		toolTipTitleItem10.Text = "Snap to Cells";
		toolTipItem10.LeftIndent = 6;
		toolTipItem10.Text = "Specify a snapping mode for displaying activities within time cells.";
		superToolTip10.Items.Add(toolTipTitleItem10);
		superToolTip10.Items.Add(toolTipItem10);
		this.changeSnapToCellsUIItem1.SuperTip = superToolTip10;
		this.optionsBar1.Control = this.schedulerControl1;
		this.optionsBar1.DockCol = 7;
		this.optionsBar1.DockRow = 0;
		this.optionsBar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		this.optionsBar1.FloatLocation = new System.Drawing.Point(1333, 175);
		this.optionsBar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.changeAppointmentStatusItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeAppointmentLabelItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.toggleRecurrenceItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.changeAppointmentReminderItem1)
		});
		this.optionsBar1.Visible = false;
		this.changeAppointmentStatusItem1.Id = 31;
		this.changeAppointmentStatusItem1.Name = "changeAppointmentStatusItem1";
		toolTipTitleItem11.Text = "&Activity Status As";
		toolTipItem11.LeftIndent = 6;
		toolTipItem11.Text = "Change the selected activity status.";
		superToolTip11.Items.Add(toolTipTitleItem11);
		superToolTip11.Items.Add(toolTipItem11);
		this.changeAppointmentStatusItem1.SuperTip = superToolTip11;
		this.changeAppointmentLabelItem1.Id = 32;
		this.changeAppointmentLabelItem1.Name = "changeAppointmentLabelItem1";
		toolTipTitleItem12.Text = "&Activity Type As";
		toolTipItem12.LeftIndent = 6;
		toolTipItem12.Text = "Change the selected activities label.";
		superToolTip12.Items.Add(toolTipTitleItem12);
		superToolTip12.Items.Add(toolTipItem12);
		this.changeAppointmentLabelItem1.SuperTip = superToolTip12;
		this.toggleRecurrenceItem1.Id = 33;
		this.toggleRecurrenceItem1.Name = "toggleRecurrenceItem1";
		toolTipTitleItem13.Text = "Recurrence";
		toolTipItem13.LeftIndent = 6;
		toolTipItem13.Text = "Make the selected activity recurring, or edit the recurrence pattern. ";
		superToolTip13.Items.Add(toolTipTitleItem13);
		superToolTip13.Items.Add(toolTipItem13);
		this.toggleRecurrenceItem1.SuperTip = superToolTip13;
		this.changeAppointmentReminderItem1.Edit = this.repositoryItemDuration1;
		this.changeAppointmentReminderItem1.Id = 34;
		this.changeAppointmentReminderItem1.Name = "changeAppointmentReminderItem1";
		this.changeAppointmentReminderItem1.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.Caption;
		toolTipTitleItem14.Text = "Reminder";
		toolTipItem14.LeftIndent = 6;
		toolTipItem14.Text = "Choose when to be reminded of the selected activity.";
		superToolTip14.Items.Add(toolTipTitleItem14);
		superToolTip14.Items.Add(toolTipItem14);
		this.changeAppointmentReminderItem1.SuperTip = superToolTip14;
		this.repositoryItemDuration1.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
		this.repositoryItemDuration1.AutoHeight = false;
		this.repositoryItemDuration1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDuration1.DisabledStateText = null;
		this.repositoryItemDuration1.Name = "repositoryItemDuration1";
		this.repositoryItemDuration1.ShowEmptyItem = true;
		this.repositoryItemDuration1.ValidateOnEnterKey = true;
		this.commonBar1.Control = this.schedulerControl1;
		this.commonBar1.DockCol = 0;
		this.commonBar1.DockRow = 0;
		this.commonBar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		this.commonBar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.openScheduleItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.saveScheduleItem1)
		});
		this.openScheduleItem1.Id = 35;
		this.openScheduleItem1.Name = "openScheduleItem1";
		this.saveScheduleItem1.Id = 36;
		this.saveScheduleItem1.Name = "saveScheduleItem1";
		this.printBar1.Control = this.schedulerControl1;
		this.printBar1.DockCol = 4;
		this.printBar1.DockRow = 0;
		this.printBar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		this.printBar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.printPreviewItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.printItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.printPageSetupItem1)
		});
		this.printBar1.Visible = false;
		this.printPreviewItem1.Id = 37;
		this.printPreviewItem1.Name = "printPreviewItem1";
		this.printItem1.Id = 38;
		this.printItem1.Name = "printItem1";
		this.printPageSetupItem1.Id = 39;
		this.printPageSetupItem1.Name = "printPageSetupItem1";
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager1;
		this.barDockControlTop.Size = new System.Drawing.Size(1420, 24);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 712);
		this.barDockControlBottom.Manager = this.barManager1;
		this.barDockControlBottom.Size = new System.Drawing.Size(1420, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 24);
		this.barDockControlLeft.Manager = this.barManager1;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 688);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(1420, 24);
		this.barDockControlRight.Manager = this.barManager1;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 688);
		this.newAppointmentItem1.Id = 0;
		this.newAppointmentItem1.Name = "newAppointmentItem1";
		this.newRecurringAppointmentItem1.Id = 1;
		this.newRecurringAppointmentItem1.Name = "newRecurringAppointmentItem1";
		this.editAppointmentQueryItem1.Id = 24;
		this.editAppointmentQueryItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.editOccurrenceUICommandItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.editSeriesUICommandItem1)
		});
		this.editAppointmentQueryItem1.Name = "editAppointmentQueryItem1";
		this.editAppointmentQueryItem1.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
		this.editOccurrenceUICommandItem1.Id = 25;
		this.editOccurrenceUICommandItem1.Name = "editOccurrenceUICommandItem1";
		this.editSeriesUICommandItem1.Id = 26;
		this.editSeriesUICommandItem1.Name = "editSeriesUICommandItem1";
		this.deleteAppointmentsItem1.Id = 27;
		this.deleteAppointmentsItem1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.deleteOccurrenceItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.deleteSeriesItem1)
		});
		this.deleteAppointmentsItem1.Name = "deleteAppointmentsItem1";
		this.deleteAppointmentsItem1.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
		this.deleteOccurrenceItem1.Id = 28;
		this.deleteOccurrenceItem1.Name = "deleteOccurrenceItem1";
		this.deleteSeriesItem1.Id = 29;
		this.deleteSeriesItem1.Name = "deleteSeriesItem1";
		this.splitAppointmentItem1.Id = 30;
		this.splitAppointmentItem1.Name = "splitAppointmentItem1";
		this.toolTipController1.KeepWhileHovered = true;
		this.toolTipController1.BeforeShow += new DevExpress.Utils.ToolTipControllerBeforeShowEventHandler(this.toolTipController1_BeforeShow);
		this.schedulerBarController1.BarItems.Add(this.newAppointmentItem1);
		this.schedulerBarController1.BarItems.Add(this.newRecurringAppointmentItem1);
		this.schedulerBarController1.BarItems.Add(this.navigateViewBackwardItem1);
		this.schedulerBarController1.BarItems.Add(this.navigateViewForwardItem1);
		this.schedulerBarController1.BarItems.Add(this.gotoTodayItem1);
		this.schedulerBarController1.BarItems.Add(this.viewZoomInItem1);
		this.schedulerBarController1.BarItems.Add(this.viewZoomOutItem1);
		this.schedulerBarController1.BarItems.Add(this.switchToDayViewItem1);
		this.schedulerBarController1.BarItems.Add(this.switchToWorkWeekViewItem1);
		this.schedulerBarController1.BarItems.Add(this.switchToWeekViewItem1);
		this.schedulerBarController1.BarItems.Add(this.switchToFullWeekViewItem1);
		this.schedulerBarController1.BarItems.Add(this.switchToMonthViewItem1);
		this.schedulerBarController1.BarItems.Add(this.switchToTimelineViewItem1);
		this.schedulerBarController1.BarItems.Add(this.switchToGanttViewItem1);
		this.schedulerBarController1.BarItems.Add(this.groupByNoneItem1);
		this.schedulerBarController1.BarItems.Add(this.groupByDateItem1);
		this.schedulerBarController1.BarItems.Add(this.groupByResourceItem1);
		this.schedulerBarController1.BarItems.Add(this.switchTimeScalesItem1);
		this.schedulerBarController1.BarItems.Add(this.changeScaleWidthItem1);
		this.schedulerBarController1.BarItems.Add(this.switchTimeScalesCaptionItem1);
		this.schedulerBarController1.BarItems.Add(this.switchCompressWeekendItem1);
		this.schedulerBarController1.BarItems.Add(this.switchShowWorkTimeOnlyItem1);
		this.schedulerBarController1.BarItems.Add(this.switchCellsAutoHeightItem1);
		this.schedulerBarController1.BarItems.Add(this.changeSnapToCellsUIItem1);
		this.schedulerBarController1.BarItems.Add(this.editAppointmentQueryItem1);
		this.schedulerBarController1.BarItems.Add(this.editOccurrenceUICommandItem1);
		this.schedulerBarController1.BarItems.Add(this.editSeriesUICommandItem1);
		this.schedulerBarController1.BarItems.Add(this.deleteAppointmentsItem1);
		this.schedulerBarController1.BarItems.Add(this.deleteOccurrenceItem1);
		this.schedulerBarController1.BarItems.Add(this.deleteSeriesItem1);
		this.schedulerBarController1.BarItems.Add(this.splitAppointmentItem1);
		this.schedulerBarController1.BarItems.Add(this.changeAppointmentStatusItem1);
		this.schedulerBarController1.BarItems.Add(this.changeAppointmentLabelItem1);
		this.schedulerBarController1.BarItems.Add(this.toggleRecurrenceItem1);
		this.schedulerBarController1.BarItems.Add(this.changeAppointmentReminderItem1);
		this.schedulerBarController1.BarItems.Add(this.openScheduleItem1);
		this.schedulerBarController1.BarItems.Add(this.saveScheduleItem1);
		this.schedulerBarController1.BarItems.Add(this.printPreviewItem1);
		this.schedulerBarController1.BarItems.Add(this.printItem1);
		this.schedulerBarController1.BarItems.Add(this.printPageSetupItem1);
		this.schedulerBarController1.Control = this.schedulerControl1;
		this.dateNavigator1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.dateNavigator1.BoldAppointmentDates = false;
		this.dateNavigator1.CalendarAppearance.DayCellSpecial.FontStyleDelta = System.Drawing.FontStyle.Bold;
		this.dateNavigator1.CalendarAppearance.DayCellSpecial.Options.UseFont = true;
		this.dateNavigator1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateNavigator1.Cursor = System.Windows.Forms.Cursors.Default;
		this.dateNavigator1.DateTime = new System.DateTime(2016, 10, 31, 0, 0, 0, 0);
		this.dateNavigator1.EditValue = new System.DateTime(2016, 10, 31, 0, 0, 0, 0);
		this.dateNavigator1.FirstDayOfWeek = System.DayOfWeek.Sunday;
		this.dateNavigator1.Location = new System.Drawing.Point(11, 133);
		this.dateNavigator1.Name = "dateNavigator1";
		this.dateNavigator1.NavigationMode = DevExpress.XtraScheduler.DateNavigationMode.ScrollCalendar;
		this.dateNavigator1.SchedulerControl = this.schedulerControl1;
		this.dateNavigator1.Size = new System.Drawing.Size(210, 236);
		this.dateNavigator1.TabIndex = 10;
		this.labelControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.labelControl1.FieldName = null;
		this.labelControl1.HideWhenEmpty = false;
		this.labelControl1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelControl1.Location = new System.Drawing.Point(12, 378);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.ShowUnderLine = false;
		this.labelControl1.Size = new System.Drawing.Size(50, 15);
		this.labelControl1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControl1.TabIndex = 21;
		this.labelControl1.Utext = "Calendars";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.CollapsePanel = DevExpress.XtraEditors.SplitCollapsePanel.Panel2;
		this.splitContainerControl1.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 34);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.schedulerControl1);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControl2);
		this.splitContainerControl1.Panel2.Controls.Add(this.searchControl1);
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControl1);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelControl1);
		this.splitContainerControl1.Panel2.Controls.Add(this.dateNavigator1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1412, 676);
		this.splitContainerControl1.SplitterPosition = 213;
		this.splitContainerControl1.TabIndex = 26;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.xtraTabControl2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabControl2.Location = new System.Drawing.Point(11, 3);
		this.xtraTabControl2.Name = "xtraTabControl2";
		this.xtraTabControl2.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControl2.Size = new System.Drawing.Size(202, 129);
		this.xtraTabControl2.TabIndex = 25;
		this.xtraTabControl2.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtraTabPage1, this.xtraTabPage2 });
		this.xtraTabPage1.Controls.Add(this.chk_Action);
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.Size = new System.Drawing.Size(200, 104);
		this.xtraTabPage1.Text = "Type";
		this.chk_Action.CheckOnClick = true;
		this.chk_Action.Cursor = System.Windows.Forms.Cursors.Default;
		this.chk_Action.Dock = System.Windows.Forms.DockStyle.Fill;
		this.chk_Action.HotTrackSelectMode = DevExpress.XtraEditors.HotTrackSelectMode.SelectItemOnClick;
		this.chk_Action.Location = new System.Drawing.Point(0, 0);
		this.chk_Action.Name = "chk_Action";
		this.chk_Action.Size = new System.Drawing.Size(200, 104);
		this.chk_Action.TabIndex = 22;
		this.chk_Action.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(this.chk_Action_ItemCheck);
		this.xtraTabPage2.ContextMenuStrip = this.contextMenuStrip2;
		this.xtraTabPage2.Controls.Add(this.simpleButton1);
		this.xtraTabPage2.Controls.Add(this.labelLine1);
		this.xtraTabPage2.Controls.Add(this.txtFilterValue);
		this.xtraTabPage2.Controls.Add(this.txtFilterField);
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.Size = new System.Drawing.Size(200, 104);
		this.xtraTabPage2.Text = "Filter";
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.toolStripMenuItemFilter1 });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(141, 26);
		this.toolStripMenuItemFilter1.DataSource = null;
		this.toolStripMenuItemFilter1.Name = "toolStripMenuItemFilter1";
		this.toolStripMenuItemFilter1.objType = Apzon.Commons.ApzObjectTypes.oCalendar;
		this.toolStripMenuItemFilter1.pivod = null;
		this.toolStripMenuItemFilter1.Size = new System.Drawing.Size(140, 22);
		this.toolStripMenuItemFilter1.Text = "Filter Setting";
		this.simpleButton1.Location = new System.Drawing.Point(124, 24);
		this.simpleButton1.Name = "simpleButton1";
		this.simpleButton1.Size = new System.Drawing.Size(60, 23);
		this.simpleButton1.TabIndex = 23;
		this.simpleButton1.Text = "Clear";
		this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
		this.labelLine1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(9, 3);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = false;
		this.labelLine1.Size = new System.Drawing.Size(120, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 22;
		this.labelLine1.Utext = "Select Filter Field";
		this.txtFilterValue.FieldName = null;
		this.txtFilterValue.IsSearchControl = false;
		this.txtFilterValue.Location = new System.Drawing.Point(9, 52);
		this.txtFilterValue.Name = "txtFilterValue";
		this.txtFilterValue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txtFilterValue.Size = new System.Drawing.Size(175, 20);
		this.txtFilterValue.SourceFilter = null;
		this.txtFilterValue.TabIndex = 21;
		this.txtFilterField.FieldName = null;
		this.txtFilterField.IsSearchControl = false;
		this.txtFilterField.Location = new System.Drawing.Point(9, 26);
		this.txtFilterField.Name = "txtFilterField";
		this.txtFilterField.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFilterField.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("FilterCode", "FilterCode"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("FilterName", "FilterName")
		});
		this.txtFilterField.Properties.DisplayMember = "FilterName";
		this.txtFilterField.Properties.NullText = "";
		this.txtFilterField.Properties.ShowHeader = false;
		this.txtFilterField.Properties.ValueMember = "FilterCode";
		this.txtFilterField.Size = new System.Drawing.Size(110, 20);
		this.txtFilterField.TabIndex = 20;
		this.searchControl1.Location = new System.Drawing.Point(65, 374);
		this.searchControl1.MenuManager = this.barManager1;
		this.searchControl1.Name = "searchControl1";
		this.searchControl1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[3]
		{
			new DevExpress.XtraEditors.Repository.ClearButton(),
			new DevExpress.XtraEditors.Repository.SearchButton(),
			new DevExpress.XtraEditors.Repository.MRUButton()
		});
		this.searchControl1.Properties.FindDelay = 100;
		this.searchControl1.Properties.ShowMRUButton = true;
		this.searchControl1.Size = new System.Drawing.Size(148, 20);
		this.searchControl1.TabIndex = 24;
		this.xtraTabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(11, 396);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.tab_User;
		this.xtraTabControl1.Size = new System.Drawing.Size(202, 280);
		this.xtraTabControl1.TabIndex = 23;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.tab_User, this.tab_Employee });
		this.xtraTabControl1.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl1_SelectedPageChanged);
		this.tab_User.Controls.Add(this.chk_Users);
		this.tab_User.FieldName = null;
		this.tab_User.IsSearchControl = false;
		this.tab_User.Name = "tab_User";
		this.tab_User.Size = new System.Drawing.Size(200, 255);
		this.tab_User.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_User.Text = "User";
		this.chk_Users.Cursor = System.Windows.Forms.Cursors.Default;
		this.chk_Users.Dock = System.Windows.Forms.DockStyle.Fill;
		this.chk_Users.Location = new System.Drawing.Point(0, 0);
		this.chk_Users.Name = "chk_Users";
		this.chk_Users.Size = new System.Drawing.Size(200, 255);
		this.chk_Users.TabIndex = 0;
		this.tab_Employee.Controls.Add(this.chk_Employees);
		this.tab_Employee.FieldName = null;
		this.tab_Employee.IsSearchControl = false;
		this.tab_Employee.Name = "tab_Employee";
		this.tab_Employee.Size = new System.Drawing.Size(200, 255);
		this.tab_Employee.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Employee.Text = "Employee";
		this.chk_Employees.Cursor = System.Windows.Forms.Cursors.Default;
		this.chk_Employees.Dock = System.Windows.Forms.DockStyle.Fill;
		this.chk_Employees.Location = new System.Drawing.Point(0, 0);
		this.chk_Employees.Name = "chk_Employees";
		this.chk_Employees.Size = new System.Drawing.Size(200, 255);
		this.chk_Employees.TabIndex = 1;
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.checkAllToolStripMenuItem, this.checkNoneToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(104, 48);
		this.checkAllToolStripMenuItem.Name = "checkAllToolStripMenuItem";
		this.checkAllToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
		this.checkAllToolStripMenuItem.Text = "All";
		this.checkAllToolStripMenuItem.Click += new System.EventHandler(this.checkAllToolStripMenuItem_Click);
		this.checkNoneToolStripMenuItem.Name = "checkNoneToolStripMenuItem";
		this.checkNoneToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
		this.checkNoneToolStripMenuItem.Text = "None";
		this.checkNoneToolStripMenuItem.Click += new System.EventHandler(this.checkNoneToolStripMenuItem_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1420, 712);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ActivityCalendar";
		this.Text = "Activity Calendar";
		base.Shown += new System.EventHandler(this.frm_ActivityCalendar_Shown);
		((System.ComponentModel.ISupportInitialize)this.schedulerStorage1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.schedulerControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemSpinEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDuration1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.schedulerBarController1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateNavigator1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateNavigator1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl2).EndInit();
		this.xtraTabControl2.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_Action).EndInit();
		this.xtraTabPage2.ResumeLayout(false);
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtFilterValue.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFilterField.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.searchControl1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.tab_User.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_Users).EndInit();
		this.tab_Employee.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_Employees).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
