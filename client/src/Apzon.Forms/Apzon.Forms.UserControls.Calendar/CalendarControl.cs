using System;
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
using Apzon.Forms.UserControls.FilterControl;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
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

namespace Apzon.Forms.UserControls.Calendar;

public class CalendarControl : XtraUserControl
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

	private bool _isChanging;

	private bool _formLoading;

	private bool _isFilter;

	private SchedulerHitInfo _Shi;

	private TimeInterval _lastFetchedInterval = new TimeInterval();

	private string _filterString = "";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private DataTable _dtUser;

	public Func<DateTime, DateTime, Task<DataTable>> ReloadSource;

	public Func<DataTable, string> SaveAppointment;

	public Func<DataTable, string> RemoveAppointment;

	public Func<DataTable, string> ViewAppointment;

	public Func<DataTable, DateTime, DateTime, string, string> CopyTask;

	public Func<DataTable, DateTime, DateTime, string, string> MoveAppointment;

	public Func<DateTime, DateTime, object, string> AddNewAppointment;

	public DataTable DtStatus;

	public DataTable DtAction;

	public DataTable DtResource;

	private readonly List<string> listMapped = new List<string>();

	private readonly LookUpEdit FilterFieldControl;

	private readonly FilterValueControl FilterValueControl;

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

	public AppointmentMappingInfo AptMapping { get; set; }

	public AppointmentCustomFieldMappingCollection AptCustomMapping { get; set; }

	public ResourceMappingInfo ResMapping { get; set; }

	public ResourceCustomFieldMappingCollection ResCustomMapping { get; set; }

	public StatusMappingInfo StatusMapping { get; set; }

	public string StatusColorFieldName { get; set; }

	public LabelMappingInfo ActionMapping { get; set; }

	public string HeaderColorFieldName { get; set; }

	public string ResourceCheckFieldName { get; set; }

	public bool IsMultiResources { get; set; }

	public CalendarControl()
	{
		try
		{
			InitializeComponent();
			FilterFieldControl = txtFilterField;
			FilterValueControl = txtFilterValue;
			SchedulerLocalizer.Active = new MyLocalizer();
			Load += OnLoad;
			schedulerStorage1.FetchAppointments += SchedulerStorage_FetchAppointments;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void OnLoad(object sender, EventArgs eventArgs)
	{
		DataSet dataSet = await _httpClient.Cfl.GetUserList();
		if (dataSet != null && dataSet.Tables.Count >= 2)
		{
			_dtUser = dataSet.Tables[1];
		}
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
		chk_Action.ValueMember = "Action";
		chk_Action.DisplayMember = "ActionName";
		chk_Action.CheckMember = "IsChecked";
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
			}
			else
			{
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
			schedulerStorage1.RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void InitCalendar()
	{
		try
		{
			schedulerControl1.BeginUpdate();
			_formLoading = true;
			MappingStatus();
			MappingAction();
			InitResources();
			MappingAppointment();
			schedulerControl1.OptionsView.ShowOnlyResourceAppointments = false;
			schedulerStorage1.Appointments.ResourceSharing = IsMultiResources;
			double value = 8.0;
			double value2 = 17.0;
			int resourcesPerPage = 3;
			schedulerControl1.GroupType = SchedulerGroupType.Resource;
			schedulerControl1.ActiveViewType = SchedulerViewType.Day;
			schedulerControl1.OptionsView.ToolTipVisibility = ToolTipVisibility.Standard;
			_filterString = "";
			if (!string.IsNullOrEmpty(_filterString) && _filterString.Length > 3)
			{
				schedulerStorage1.Appointments.Filter = _filterString.Substring(3);
			}
			else
			{
				schedulerStorage1.Appointments.Filter = "";
			}
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
			schedulerControl1.Start = DateTime.Now;
			ApzObjectTypes objectType = ApzObjectTypes.oCalendar;
			FilterFieldControl.Properties.Columns.Clear();
			FilterFieldControl.Properties.Columns.Add(new LookUpColumnInfo("FilterCode"));
			FilterFieldControl.Properties.Columns.Add(new LookUpColumnInfo("FilterName"));
			FilterFieldControl.Properties.DisplayMember = "FilterName";
			FilterFieldControl.Properties.ValueMember = "FilterCode";
			FilterFieldControl.EditValueChanged += FilterFieldControl_EditValueChanged;
			FilterValueControl.EditValueChanged += txtFilterValue_EditValueChanged;
			DataTable dataFilterField = _httpClient.FormFilterSettingClient.GetDataFilterField(objectType);
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
			schedulerControl1.EndUpdate();
			_formLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void MappingAppointment()
	{
		schedulerStorage1.BeginUpdate();
		schedulerStorage1.Appointments.Items.Clear();
		schedulerStorage1.Appointments.CustomFieldMappings.Clear();
		if (AptCustomMapping == null)
		{
			AptCustomMapping = new AppointmentCustomFieldMappingCollection();
		}
		foreach (AppointmentCustomFieldMapping item in AptCustomMapping)
		{
			schedulerStorage1.Appointments.CustomFieldMappings.Add(item);
		}
		if (AptMapping != null)
		{
			schedulerStorage1.Appointments.Mappings = AptMapping;
		}
		schedulerStorage1.EndUpdate();
	}

	private void MappingStatus()
	{
		try
		{
			if (!DtStatus.IsNotNull() || StatusMapping == null)
			{
				return;
			}
			DtStatus.AcceptChanges();
			AppointmentStatusCollection statuses = schedulerStorage1.Appointments.Statuses;
			statuses.Clear();
			statuses.BeginUpdate();
			for (int i = 0; i < DtStatus.Rows.Count; i++)
			{
				Color color = default;
				if (!string.IsNullOrWhiteSpace(StatusColorFieldName))
				{
					color = Color.FromArgb(Function.ParseInt(DtStatus.Rows[i][StatusColorFieldName]));
				}
				SolidBrush brush = new SolidBrush(color);
				int num = Function.ParseInt(DtStatus.Rows[i][StatusMapping.Id]);
				string text = Function.ToString(DtStatus.Rows[i][StatusMapping.MenuCaption]);
				statuses.Add(num, AppointmentStatusType.Custom, text, text, brush);
			}
			statuses.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void MappingAction()
	{
		try
		{
			if (!DtAction.IsNotNull() || ActionMapping == null)
			{
				return;
			}
			AppointmentLabelCollection labels = schedulerStorage1.Appointments.Labels;
			labels.Clear();
			labels.BeginUpdate();
			foreach (DataRow item in DtAction.AsEnumerable())
			{
				Color labelColor = Color.FromArgb(Function.ParseInt(item[ActionMapping.Color]));
				string displayName = Function.ToString(item[ActionMapping.DisplayName]);
				string menuCaption = Function.ToString(item[ActionMapping.MenuCaption]);
				string id = Function.ToString(item[ActionMapping.Id]);
				labels.Add(id, displayName, menuCaption, labelColor);
			}
			labels.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void InitResources()
	{
		try
		{
			schedulerControl1.BeginUpdate();
			if (DtResource.IsNotNull() && ResMapping != null)
			{
				schedulerStorage1.Resources.CustomFieldMappings.Clear();
				schedulerStorage1.BeginUpdate();
				ResourceMappingInfo mappings = schedulerStorage1.Resources.Mappings;
				mappings.Caption = ResMapping.Caption;
				mappings.Id = ResMapping.Id;
				mappings.Color = ResMapping.Color;
				if (ResCustomMapping == null)
				{
					ResCustomMapping = new ResourceCustomFieldMappingCollection();
				}
				foreach (ResourceCustomFieldMapping item in ResCustomMapping)
				{
					schedulerStorage1.Resources.CustomFieldMappings.Add(item);
				}
				schedulerStorage1.Resources.DataSource = DtResource;
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
				if (!(item.GetRow(schedulerStorage1) is DataRowView dataRowView))
				{
					continue;
				}
				ResourceMappingInfo mappings = schedulerStorage1.Resources.Mappings;
				object value = dataRowView[mappings.Id];
				object obj = dataRowView[mappings.Caption];
				CheckedListBoxItem checkedListBoxItem = new CheckedListBoxItem();
				checkedListBoxItem.Value = value;
				checkedListBoxItem.Description = Function.ToString(obj);
				if (!string.IsNullOrWhiteSpace(ResourceCheckFieldName))
				{
					if (Function.ToString(dataRowView[ResourceCheckFieldName]) == "Y")
					{
						checkedListBoxItem.CheckState = CheckState.Checked;
					}
					else
					{
						checkedListBoxItem.CheckState = CheckState.Unchecked;
					}
				}
				else
				{
					checkedListBoxItem.CheckState = CheckState.Unchecked;
				}
				chk_Users.Items.Add(checkedListBoxItem);
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

	private void AddNewForm()
	{
		try
		{
			if (_Shi != null)
			{
				TimeInterval selectedInterval = schedulerControl1.SelectedInterval;
				_ = selectedInterval.Start.Date;
				selectedInterval.Start.ToString("HH:mm");
				_ = selectedInterval.End.Date;
				selectedInterval.End.ToString("HH:mm");
				_ = selectedInterval.AllDay;
				ResourceIdCollection resourceIds = new ResourceIdCollection();
				SchedulerCompatibility.Base64XmlObjectSerialization = false;
				new AppointmentResourceIdCollectionXmlPersistenceHelper(resourceIds).ToXml();
				WinCommons._OpenProcessing();
				if (AddNewAppointment != null && string.IsNullOrWhiteSpace(AddNewAppointment(selectedInterval.Start, selectedInterval.End, _Shi.ViewInfo.Resource.Id)))
				{
					ReloadCalendar();
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

	private void OpenAppoiment(object sender, EventArgs eventArgs)
	{
		try
		{
			_Shi = schedulerControl1.ActiveView.ViewInfo.CalcHitInfo(schedulerControl1.PointToClient(System.Windows.Forms.Cursor.Position), layoutOnly: false);
			if (schedulerControl1.SelectedAppointments.Count > 0)
			{
				Appointment appointment = schedulerControl1.SelectedAppointments[0];
				object id = appointment.Id;
				DataTable dataTable = schedulerStorage1.Appointments.DataSource as DataTable;
				if (dataTable.IsNotNull())
				{
					DataTable dataTable2 = dataTable.Clone();
					DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t[AptMapping.AppointmentId]) == Function.ToString(id));
					if (dataRow != null)
					{
						DataRow dataRow2 = dataTable2.NewRow();
						dataRow2.ItemArray = dataRow.ItemArray;
						dataTable2.Rows.Add(dataRow2);
						dataTable2.AcceptChanges();
						ViewAppointment?.Invoke(dataTable2);
					}
				}
			}
			else
			{
				AddNewForm();
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
					popupMenuById.Caption = "&Action";
				}
				SchedulerPopupMenu popupMenuById2 = e.Menu.GetPopupMenuById(SchedulerMenuItemId.StatusSubMenu);
				if (popupMenuById2 != null)
				{
					popupMenuById2.Caption = "Status";
				}
				if (schedulerControl1.SelectedAppointments.Count > 0)
				{
					SchedulerMenuItem schedulerMenuItem2 = null;
					schedulerMenuItem2 = new SchedulerMenuItem("&Remove this Appointment", RemoveTask);
					schedulerMenuItem2.BeginGroup = true;
					e.Menu.Items.Add(schedulerMenuItem2);
					if (schedulerMenuItem2 != null)
					{
						schedulerMenuItem2.BeginGroup = true;
					}
				}
			}
			if (e.Menu.Id == SchedulerMenuItemId.DefaultMenu)
			{
				string text = ((_Shi.ViewInfo.Resource != null) ? Function.ToString(_Shi.ViewInfo.Resource.Caption) : "<empty>");
				string caption = "New &Appointment";
				if (_Shi.ViewInfo.Resource != null)
				{
					caption = "New &Appointment for " + text;
				}
				SchedulerMenuItem schedulerMenuItem3 = new SchedulerMenuItem(caption, AddTask);
				schedulerMenuItem3.BeginGroup = true;
				e.Menu.Items.Add(schedulerMenuItem3);
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

	private async void RemoveTask(object sender, EventArgs e)
	{
		await Remove();
	}

	private async Task Remove()
	{
		try
		{
			if (schedulerControl1.SelectedAppointments.Count <= 0)
			{
				return;
			}
			Appointment appointment = schedulerControl1.SelectedAppointments[0];
			object selectedClgCode = appointment.Id;
			DataTable dataTable = schedulerStorage1.Appointments.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.Copy().AsEnumerable().FirstOrDefault((DataRow t) => t[AptMapping.AppointmentId] == selectedClgCode);
				DataTable dataTable2 = dataTable.Clone();
				dataTable2.ImportRow(dataRow);
				if (dataRow != null && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes && RemoveAppointment != null)
				{
					string text = RemoveAppointment(dataTable2);
					if (!string.IsNullOrEmpty(text))
					{
						WinCommons.Error(Enums.MessageCode.None, text);
						return;
					}
					WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
					await ReloadCalendar();
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.RemoveError, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddTask(object sender, EventArgs eventArgs)
	{
		AddNewForm();
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
				object id = appointment.Id;
				DataTable dataTable = schedulerStorage1.Appointments.DataSource as DataTable;
				if (dataTable.IsNotNull())
				{
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
							for (int i = 0; i < dataTable2.Rows.Count; i++)
							{
								DataRow dataRow = dataTable2.Rows[i];
								string text = Function.ToString(dataRow["FilterName"]);
								string text2 = Function.ToString(dataRow["FilterCode"]);
								if (!listMapped.Contains(text2) && !string.IsNullOrEmpty(text2))
								{
									ToolTipItem contents3 = superToolTipSetupArgs.Contents;
									contents3.Text = contents3.Text + text + ": " + Function.ToString(appointment.CustomFields[text2]) + "\r\n";
								}
							}
						}
						if (appointment.Start.Date != appointment.End.Date)
						{
							superToolTipSetupArgs.Footer.Text = appointment.Start.Date.ToShortDateString() + " " + appointment.Start.ToShortTimeString() + " - " + appointment.End.Date.ToShortDateString() + " " + appointment.End.ToShortTimeString();
						}
						else
						{
							superToolTipSetupArgs.Footer.Text = appointment.Start.ToShortTimeString() + " - " + appointment.End.ToShortTimeString();
						}
						if (dataTable.AsEnumerable().FirstOrDefault((DataRow t) => t[AptMapping.AppointmentId] == id) != null)
						{
							superToolTipSetupArgs.Footer.Text += $". \\r\\n ID: {id}";
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

	private void schedulerControl1_AppointmentDrop(object sender, AppointmentDragEventArgs e)
	{
		try
		{
			_isChanging = true;
			if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
			{
				e.Allow = DropNormalAppointment(e.SourceAppointment, e.EditedAppointment, isCopy: true);
			}
			else if (e.EditedAppointment.IsRecurring)
			{
				e.Allow = DropRecurringAppointment(e.SourceAppointment, e.EditedAppointment);
			}
			else
			{
				e.Allow = DropNormalAppointment(e.SourceAppointment, e.EditedAppointment);
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

	private bool DropNormalAppointment(Appointment srcAppointment, Appointment editedAppointment, bool isCopy = false)
	{
		try
		{
			ResourceIdCollection resourceIdCollection = new ResourceIdCollection();
			SchedulerCompatibility.Base64XmlObjectSerialization = false;
			DateTime start = editedAppointment.Start;
			DateTime start2 = srcAppointment.Start;
			DateTime end = editedAppointment.End;
			DateTime end2 = srcAppointment.End;
			object id = srcAppointment.Id;
			string text = Function.ToString(srcAppointment.ResourceId);
			string text2 = Function.ToString(editedAppointment.ResourceId);
			string caption = schedulerStorage1.Resources.GetResourceById(editedAppointment.ResourceId).Caption;
			string caption2 = schedulerStorage1.Resources.GetResourceById(srcAppointment.ResourceId).Caption;
			_ = srcAppointment.ResourceIds;
			AppointmentResourceIdCollection resourceIds = editedAppointment.ResourceIds;
			Function.ToString(srcAppointment.LabelKey);
			string text3 = "Appointment";
			if (!isCopy)
			{
				string text4;
				if (Function.ToString(text) == Function.ToString(text2))
				{
					text4 = ((start2 == DateTime.MinValue) ? string.Format("Creating an {2} on {0:D} at {1:t}.", start.Date, start.TimeOfDay, text3) : string.Format("Moving the {5} \r\nscheduled on {0:D} Start at {1:T}\r\nto {2:dddd, dd MMM yyyy HH:mm:ss } and End at {3:T}\r\nto {4:dddd, dd MMM yyyy HH:mm:ss }.", start2.Date, start2.TimeOfDay, start, end2.TimeOfDay, end, text3));
				}
				else
				{
					text4 = ((start2 == DateTime.MinValue) ? string.Format("Creating an {2} on {0:D} at {1:t}.", start.Date, start.TimeOfDay, text3) : string.Format("Moving the {7} \r\nscheduled on {0:D} Start at {1:T}\r\nto {2:dddd, dd MMM yyyy HH:mm:ss } and End at {3:T}\r\nto {4:dddd, dd MMM yyyy HH:mm:ss }, From resource {5} to {6}", start2.Date, start2.TimeOfDay, start, end2.TimeOfDay, end, caption2, caption, text3));
				}
				if (XtraMessageBox.Show(text4 + " Proceed?", "Confirm the action", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					DataTable dataTable = schedulerStorage1.Appointments.DataSource as DataTable;
					if (dataTable.IsNotNull())
					{
						DataTable dataTable2 = dataTable.Clone();
						DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => t[AptMapping.AppointmentId] == id);
						if (dataRow == null)
						{
							return false;
						}
						dataTable2.ImportRow(dataRow);
						resourceIdCollection.Add(text2);
						string arg = new AppointmentResourceIdCollectionXmlPersistenceHelper(resourceIds).ToXml();
						if (CopyTask != null)
						{
							string.IsNullOrEmpty(MoveAppointment(dataTable2, start, end, arg));
							return false;
						}
					}
				}
			}
			else
			{
				DialogResult dialogResult = ((!(Function.ToString(text) != Function.ToString(text2))) ? XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCopyAppointment, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) : XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCopyAppointmentFrom, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, caption2, caption));
				if (dialogResult == DialogResult.Yes)
				{
					DataTable dataTable3 = schedulerStorage1.Appointments.DataSource as DataTable;
					if (dataTable3.IsNotNull())
					{
						DataTable dataTable4 = dataTable3.Clone();
						DataRow dataRow2 = dataTable3.AsEnumerable().FirstOrDefault((DataRow t) => t[AptMapping.AppointmentId] == id);
						if (dataRow2 == null)
						{
							return false;
						}
						string arg2 = new AppointmentResourceIdCollectionXmlPersistenceHelper(resourceIds).ToXml();
						dataTable4.ImportRow(dataRow2);
						if (CopyTask != null)
						{
							string.IsNullOrEmpty(CopyTask(dataTable4, start, end, arg2));
							return false;
						}
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

	private bool DropRecurringAppointment(Appointment srcAppointment, Appointment editedAppointment)
	{
		return false;
	}

	private void schedulerControl1_AppointmentResized(object sender, AppointmentResizeEventArgs e)
	{
		try
		{
			_isChanging = true;
			if (e.EditedAppointment.IsRecurring)
			{
				e.Handled = true;
				e.Allow = DropRecurringAppointment(e.SourceAppointment, e.EditedAppointment);
			}
			else
			{
				e.Handled = true;
				e.Allow = DropNormalAppointment(e.SourceAppointment, e.EditedAppointment);
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

	private void schedulerStorage1_AppointmentChanging(object sender, PersistentObjectCancelEventArgs e)
	{
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
			object id = appointment.Id;
			DataTable dataTable = schedulerStorage1.Appointments.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				return;
			}
			DataRow row = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => t[AptMapping.AppointmentId] == id);
			DataTable dataTable2 = dataTable.Clone();
			dataTable2.ImportRow(row);
			dataTable2.AcceptChanges();
			if (!dataTable2.IsNotNull())
			{
				return;
			}
			if (string.IsNullOrEmpty(Function.ToString(appointment.Subject)))
			{
				XtraMessageBoxEx.Show(Enums.MessageCode.InfoSubjectCanNotEmpty, Enums.MessageCode.SystemMessage);
				e.Cancel = true;
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmChange, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				if (SaveAppointment != null)
				{
					string text = SaveAppointment(dataTable2);
					if (string.IsNullOrEmpty(text))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						return;
					}
					WinCommons.Error(Enums.MessageCode.None, text);
					e.Cancel = true;
				}
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
			if (!(e.ObjectInfo is SchedulerHeader schedulerHeader) || string.IsNullOrWhiteSpace(HeaderColorFieldName))
			{
				return;
			}
			AppearanceObject headerCaption = schedulerHeader.Appearance.HeaderCaption;
			Color color = Color.FromArgb(Function.ParseInt(schedulerHeader.Resource.CustomFields[HeaderColorFieldName]));
			if (schedulerControl == null)
			{
				return;
			}
			SchedulerGroupType groupType = schedulerControl.ActiveView.GroupType;
			int num;
			int num2;
			if (!(schedulerControl.ActiveView is WeekView) || groupType != SchedulerGroupType.Date)
			{
				if (schedulerControl.ActiveView is TimelineView)
				{
					num = ((groupType != SchedulerGroupType.None) ? 1 : 0);
					if (num != 0)
					{
						goto IL_0099;
					}
				}
				else
				{
					num = 0;
				}
				num2 = 1;
				goto IL_009a;
			}
			num = 1;
			goto IL_0099;
			IL_0099:
			num2 = 0;
			goto IL_009a;
			IL_009a:
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
			e.Handled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void chk_Action_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)
	{
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
		_ = 1;
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
				schedulerControl1.BeginUpdate();
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
				if (ReloadSource != null)
				{
					await RefreshMainCalendarData(await ReloadSource(_lastFetchedInterval.Start, _lastFetchedInterval.End));
				}
				schedulerControl1.EndUpdate();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task RefreshMainCalendarData(DataTable dt)
	{
		try
		{
			WinCommons._OpenProcessing();
			if (dt.HasColumn())
			{
				schedulerStorage1.Appointments.Clear();
				schedulerStorage1.Appointments.DataSource = dt.Copy();
				await PushValueToFilterValueControl();
				DoFilter();
			}
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

	private async void schedulerStorage1_AppointmentsChanged(object sender, PersistentObjectsEventArgs e)
	{
		try
		{
			if (!_isFilter)
			{
				await PushValueToFilterValueControl();
				DoFilter();
			}
			schedulerStorage1.RefreshData();
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
		this.schedulerBarController1 = new DevExpress.XtraScheduler.UI.SchedulerBarController();
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
		this.schedulerControl1.Size = new System.Drawing.Size(1194, 676);
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
		this.schedulerControl1.AppointmentViewInfoCustomizing += new DevExpress.XtraScheduler.AppointmentViewInfoCustomizingEventHandler(this.schedulerControl1_AppointmentViewInfoCustomizing);
		this.schedulerControl1.AppointmentDrop += new DevExpress.XtraScheduler.AppointmentDragEventHandler(this.schedulerControl1_AppointmentDrop);
		this.schedulerControl1.AppointmentResized += new DevExpress.XtraScheduler.AppointmentResizeEventHandler(this.schedulerControl1_AppointmentResized);
		this.schedulerControl1.PopupMenuShowing += new DevExpress.XtraScheduler.PopupMenuShowingEventHandler(this.schedulerControl1_PopupMenuShowing);
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
		this.switchToDayViewItem1.Name = "switchToDayViewItem1";
		toolTipTitleItem.Text = "&Day View";
		toolTipItem.LeftIndent = 6;
		toolTipItem.Text = "Switch to the Day view. The most detailed view of activities for a specific day(s).";
		superToolTip.Items.Add(toolTipTitleItem);
		superToolTip.Items.Add(toolTipItem);
		this.switchToDayViewItem1.SuperTip = superToolTip;
		this.switchToWorkWeekViewItem1.Id = 8;
		this.switchToWorkWeekViewItem1.Name = "switchToWorkWeekViewItem1";
		this.switchToWeekViewItem1.Id = 9;
		this.switchToWeekViewItem1.Name = "switchToWeekViewItem1";
		toolTipTitleItem2.Text = "&Week View";
		toolTipItem2.LeftIndent = 6;
		toolTipItem2.Text = "Switch to the Week view. Arranges activities for a particular week in a compact form.";
		superToolTip2.Items.Add(toolTipTitleItem2);
		superToolTip2.Items.Add(toolTipItem2);
		this.switchToWeekViewItem1.SuperTip = superToolTip2;
		this.switchToFullWeekViewItem1.Id = 10;
		this.switchToFullWeekViewItem1.Name = "switchToFullWeekViewItem1";
		toolTipTitleItem3.Text = "&Full Week View";
		toolTipItem3.LeftIndent = 6;
		toolTipItem3.Text = "Switch to the Full Week View. Displays activities for the entire week.";
		superToolTip3.Items.Add(toolTipTitleItem3);
		superToolTip3.Items.Add(toolTipItem3);
		this.switchToFullWeekViewItem1.SuperTip = superToolTip3;
		this.switchToMonthViewItem1.Id = 11;
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
		this.repositoryItemDuration1.NullValuePromptShowForEmptyValue = true;
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
		this.barDockControlTop.Size = new System.Drawing.Size(1420, 31);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 712);
		this.barDockControlBottom.Manager = this.barManager1;
		this.barDockControlBottom.Size = new System.Drawing.Size(1420, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 31);
		this.barDockControlLeft.Manager = this.barManager1;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 681);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(1420, 31);
		this.barDockControlRight.Manager = this.barManager1;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 681);
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
		this.xtraTabControl2.SelectedTabPage = this.xtraTabPage2;
		this.xtraTabControl2.Size = new System.Drawing.Size(202, 129);
		this.xtraTabControl2.TabIndex = 25;
		this.xtraTabControl2.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtraTabPage1, this.xtraTabPage2 });
		this.xtraTabPage1.Controls.Add(this.chk_Action);
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.PageVisible = false;
		this.xtraTabPage1.Size = new System.Drawing.Size(196, 101);
		this.xtraTabPage1.Text = "Type";
		this.chk_Action.CheckOnClick = true;
		this.chk_Action.Cursor = System.Windows.Forms.Cursors.Default;
		this.chk_Action.Dock = System.Windows.Forms.DockStyle.Fill;
		this.chk_Action.HotTrackSelectMode = DevExpress.XtraEditors.HotTrackSelectMode.SelectItemOnClick;
		this.chk_Action.Location = new System.Drawing.Point(0, 0);
		this.chk_Action.Name = "chk_Action";
		this.chk_Action.Size = new System.Drawing.Size(196, 101);
		this.chk_Action.TabIndex = 22;
		this.chk_Action.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(this.chk_Action_ItemCheck);
		this.xtraTabPage2.ContextMenuStrip = this.contextMenuStrip2;
		this.xtraTabPage2.Controls.Add(this.simpleButton1);
		this.xtraTabPage2.Controls.Add(this.labelLine1);
		this.xtraTabPage2.Controls.Add(this.txtFilterValue);
		this.xtraTabPage2.Controls.Add(this.txtFilterField);
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.Size = new System.Drawing.Size(196, 101);
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
		this.xtraTabControl1.Location = new System.Drawing.Point(11, 396);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.tab_User;
		this.xtraTabControl1.Size = new System.Drawing.Size(202, 280);
		this.xtraTabControl1.TabIndex = 23;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.tab_User, this.tab_Employee });
		this.xtraTabControl1.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl1_SelectedPageChanged);
		this.tab_User.Controls.Add(this.chk_Users);
		this.tab_User.FieldName = null;
		this.tab_User.Name = "tab_User";
		this.tab_User.Size = new System.Drawing.Size(196, 252);
		this.tab_User.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_User.Text = "Resources";
		this.chk_Users.Cursor = System.Windows.Forms.Cursors.Default;
		this.chk_Users.Dock = System.Windows.Forms.DockStyle.Fill;
		this.chk_Users.Location = new System.Drawing.Point(0, 0);
		this.chk_Users.Name = "chk_Users";
		this.chk_Users.Size = new System.Drawing.Size(196, 252);
		this.chk_Users.TabIndex = 0;
		this.tab_Employee.Controls.Add(this.chk_Employees);
		this.tab_Employee.FieldName = null;
		this.tab_Employee.Name = "tab_Employee";
		this.tab_Employee.PageVisible = false;
		this.tab_Employee.Size = new System.Drawing.Size(196, 252);
		this.tab_Employee.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Employee.Text = "Employee";
		this.chk_Employees.Cursor = System.Windows.Forms.Cursors.Default;
		this.chk_Employees.Dock = System.Windows.Forms.DockStyle.Fill;
		this.chk_Employees.Location = new System.Drawing.Point(0, 0);
		this.chk_Employees.Name = "chk_Employees";
		this.chk_Employees.Size = new System.Drawing.Size(196, 252);
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
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "CalendarControl";
		base.Size = new System.Drawing.Size(1420, 712);
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
