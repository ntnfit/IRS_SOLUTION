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
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.UI;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;

namespace Apzon.Forms.RoutingAndResource;

public class frm_CapacityGantt : XtraFormEx
{
	private const string OkString = "Ok";

	private const string SaveString = "Save";

	private HilightApoinment _hilightApoinment = new HilightApoinment();

	private DragType _dragType = DragType.Forward;

	private DataTable _dtTooltipSetting = new DataTable();

	private IContainer components;

	private SchedulerControl ScheduleGanttChart;

	private SchedulerStorage ScheduleStorageGanttChart;

	private SplitContainerControl SplitReource;

	private ResourcesTree ResourceTree;

	private ResourceTreeColumn treeColumnCaption;

	private ResourceTreeColumn treeColumnType;

	private ResourceTreeColumn treeColumnId;

	private SimpleButton btnCancel;

	private SimpleButton btnSave;

	private SplitContainerControl SplitDetail;

	private GridControlEx grdOpeColor;

	private GridViewEx gvOpeColor;

	private GridColumnEx gcOpeName;

	private GridColumnEx gcOpeColor;

	private RepositoryItemColorEdit rColorEdit;

	private GridControlEx grdStatus;

	private GridViewEx gvStatus;

	private GridColumnEx gcStatus;

	private GridColumnEx gcStatusColor;

	private RepositoryItemColorEdit rStatusColor;

	private ToolTipController toolTipController1;

	private SimpleButton btnRun;

	private LabelLine lbScheCode;

	private ButtonEditEx txtScheCode;

	private SplitContainerControl SplitApointment;

	private SimpleButton btnViewDetail;

	private RepositoryItemLookUpEdit rResType;

	private LabelLine lbCapacityCode;

	private ButtonEditEx txtCapacityCode;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		btnSave.Text = ((value == Enums.FormViewModeType.View) ? "Ok" : "Save");
	}

	public frm_CapacityGantt(int scheduleId)
	{
		InitializeComponent();
		txtScheCode.EditValue = scheduleId;
	}

	public frm_CapacityGantt()
	{
		InitializeComponent();
	}

	private async Task frm_CapacityGantt_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			LoadResType();
			MappingAction();
			ScheduleGanttChart.Views.TimelineView.ShowResourceHeaders = false;
			ScheduleGanttChart.OptionsView.ShowOnlyResourceAppointments = false;
			ScheduleGanttChart.GroupType = SchedulerGroupType.Date;
			ScheduleGanttChart.ActiveViewType = SchedulerViewType.Timeline;
			for (int i = 0; i < ScheduleGanttChart.Views.TimelineView.Scales.Count; i++)
			{
				ScheduleGanttChart.Views.TimelineView.Scales[i].DisplayFormat = GlobalData.DateTimeFormat;
			}
			for (int j = 0; j < ScheduleGanttChart.Views.GanttView.Scales.Count; j++)
			{
				ScheduleGanttChart.Views.GanttView.Scales[j].DisplayFormat = GlobalData.DateTimeFormat;
			}
			ScheduleStorageGanttChart.Resources.Mappings.Caption = "ResName";
			ScheduleStorageGanttChart.Resources.Mappings.Id = "ResCode";
			ScheduleStorageGanttChart.Appointments.Mappings.ResourceId = "ResCode";
			ScheduleStorageGanttChart.Appointments.Mappings.Start = "StartDate";
			ScheduleStorageGanttChart.Appointments.Mappings.End = "EndDate";
			ScheduleStorageGanttChart.Appointments.Mappings.Subject = "ItemCode";
			ScheduleStorageGanttChart.Appointments.Mappings.Description = "Qty";
			ScheduleStorageGanttChart.Appointments.Mappings.Location = "OpeName";
			ScheduleStorageGanttChart.Appointments.Mappings.Label = "IdColor";
			ScheduleStorageGanttChart.Appointments.Mappings.AppointmentId = "Line";
			ScheduleStorageGanttChart.Appointments.Mappings.ReminderInfo = "IsBase";
			ScheduleStorageGanttChart.Appointments.Mappings.Status = "Complete";
			ScheduleStorageGanttChart.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping
			{
				Name = "DemandType",
				Member = "DemandType",
				ValueType = FieldValueType.String
			});
			ScheduleStorageGanttChart.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping
			{
				Name = "DemandEntry",
				Member = "DemandEntry",
				ValueType = FieldValueType.Integer
			});
			ScheduleStorageGanttChart.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping
			{
				Name = "DemandLine",
				Member = "DemandLine",
				ValueType = FieldValueType.Integer
			});
			ScheduleStorageGanttChart.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping
			{
				Name = "FatherCode",
				Member = "FatherCode",
				ValueType = FieldValueType.String
			});
			ScheduleStorageGanttChart.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping
			{
				Name = "FatherLine",
				Member = "FatherLine",
				ValueType = FieldValueType.Integer
			});
			ScheduleStorageGanttChart.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping
			{
				Name = "StartTime",
				Member = "StartTime",
				ValueType = FieldValueType.Integer
			});
			ScheduleStorageGanttChart.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping
			{
				Name = "EndTime",
				Member = "EndTime",
				ValueType = FieldValueType.Integer
			});
			ScheduleStorageGanttChart.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping
			{
				Name = "WorkDay",
				Member = "WorkDay",
				ValueType = FieldValueType.DateTime
			});
			ScheduleStorageGanttChart.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping
			{
				Name = "EndWorkDay",
				Member = "EndWorkDay",
				ValueType = FieldValueType.DateTime
			});
			ScheduleStorageGanttChart.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping
			{
				Name = "IsBase",
				Member = "IsBase",
				ValueType = FieldValueType.String
			});
			ScheduleStorageGanttChart.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping
			{
				Name = "OpeOrder",
				Member = "OpeOrder",
				ValueType = FieldValueType.Integer
			});
			ScheduleStorageGanttChart.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping
			{
				Name = "ProId",
				Member = "ProId",
				ValueType = FieldValueType.Integer
			});
			ScheduleStorageGanttChart.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping
			{
				Name = "TimeRun",
				Member = "TimeRun",
				ValueType = FieldValueType.Decimal
			});
			ScheduleStorageGanttChart.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping
			{
				Name = "LineType",
				Member = "LineType",
				ValueType = FieldValueType.String
			});
			ScheduleStorageGanttChart.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping
			{
				Name = "LinkEntry",
				Member = "LinkEntry",
				ValueType = FieldValueType.String
			});
			ScheduleStorageGanttChart.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping
			{
				Name = "RoutCode",
				Member = "RoutCode",
				ValueType = FieldValueType.String
			});
			ScheduleStorageGanttChart.Appointments.CustomFieldMappings.Add(new AppointmentCustomFieldMapping
			{
				Name = "RoutLine",
				Member = "RoutLine",
				ValueType = FieldValueType.Integer
			});
			DataSet dataSetting = HttpClient.CapacitySettingClient.GetDataSetting();
			int resourcesPerPage = 8;
			SolidBrush brush = new SolidBrush(Color.White);
			SolidBrush brush2 = new SolidBrush(Color.Green);
			SolidBrush brush3 = new SolidBrush(Color.LightCoral);
			SolidBrush brush4 = new SolidBrush(Color.DarkGray);
			SolidBrush brush5 = new SolidBrush(Color.Black);
			int num = 2;
			if (dataSetting != null && dataSetting.Tables.Count == 2 && dataSetting.Tables[0].IsNotNull())
			{
				_dragType = (DragType)Function.ParseInt(dataSetting.Tables[0].Rows[0]["DragType"]);
				_dtTooltipSetting = dataSetting.Tables[1];
				if (Function.ParseInt(dataSetting.Tables[0].Rows[0]["RsPerPag"]) > 0)
				{
					resourcesPerPage = Function.ParseInt(dataSetting.Tables[0].Rows[0]["RsPerPag"]);
				}
				if (Function.ParseInt(dataSetting.Tables[0].Rows[0]["TlvSpaWdth"]) > 0)
				{
					num = Function.ParseInt(dataSetting.Tables[0].Rows[0]["TlvSpaWdth"]);
				}
				if (Function.ParseInt(dataSetting.Tables[0].Rows[0]["ApHeight"]) > 0)
				{
					int num2 = Function.ParseInt(dataSetting.Tables[0].Rows[0]["ApHeight"]);
					ScheduleGanttChart.TimelineView.AppointmentDisplayOptions.AppointmentHeight = num2;
					ScheduleGanttChart.TimelineView.Appearance.Appointment.Font = new Font("Tahoma", Function.ParseFloat((num2 >= 20) ? 8.25 : 6.0));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataSetting.Tables[0].Rows[0]["NotCompColor"])))
				{
					brush = new SolidBrush(Color.FromArgb(Function.ParseInt(dataSetting.Tables[0].Rows[0]["NotCompColor"])));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataSetting.Tables[0].Rows[0]["CompColor"])))
				{
					brush2 = new SolidBrush(Color.FromArgb(Function.ParseInt(dataSetting.Tables[0].Rows[0]["CompColor"])));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataSetting.Tables[0].Rows[0]["GenPoColor"])))
				{
					brush3 = new SolidBrush(Color.FromArgb(Function.ParseInt(dataSetting.Tables[0].Rows[0]["GenPoColor"])));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataSetting.Tables[0].Rows[0]["CloseColor"])))
				{
					brush4 = new SolidBrush(Color.FromArgb(Function.ParseInt(dataSetting.Tables[0].Rows[0]["CloseColor"])));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataSetting.Tables[0].Rows[0]["CancelColor"])))
				{
					brush4 = new SolidBrush(Color.FromArgb(Function.ParseInt(dataSetting.Tables[0].Rows[0]["CancelColor"])));
				}
			}
			TimeScale timeScale = ScheduleGanttChart.TimelineView.Scales.FirstOrDefault((TimeScale t) => t.DisplayName.Equals("15 Minutes"));
			if (timeScale != null)
			{
				timeScale.Width = num;
			}
			ScheduleGanttChart.DayView.ResourcesPerPage = resourcesPerPage;
			ScheduleGanttChart.WeekView.ResourcesPerPage = resourcesPerPage;
			ScheduleGanttChart.WorkWeekView.ResourcesPerPage = resourcesPerPage;
			ScheduleGanttChart.FullWeekView.ResourcesPerPage = resourcesPerPage;
			ScheduleGanttChart.MonthView.ResourcesPerPage = resourcesPerPage;
			ScheduleGanttChart.TimelineView.ResourcesPerPage = resourcesPerPage;
			ScheduleGanttChart.GanttView.ResourcesPerPage = resourcesPerPage;
			AppointmentStatusCollection statuses = ScheduleStorageGanttChart.Appointments.Statuses;
			statuses.Clear();
			statuses.BeginUpdate();
			statuses.Add("N", AppointmentStatusType.Custom, "Not Complete", "Not Complete", brush);
			statuses.Add("C", AppointmentStatusType.Custom, "Complete", "Complete", brush2);
			statuses.Add("G", AppointmentStatusType.Custom, "Gen Production Order", "Gen Production Order", brush3);
			statuses.Add("D", AppointmentStatusType.Custom, "Close", "Close", brush4);
			statuses.Add("L", AppointmentStatusType.Custom, "Cancel", "Cancel", brush5);
			statuses.EndUpdate();
			ScheduleGanttChart.CustomAppointmentSort += CustomAppointmentSort;
			DataTable dataSource = await HttpClient.Capacity.GetResources();
			ScheduleStorageGanttChart.BeginUpdate();
			ScheduleStorageGanttChart.Resources.DataSource = dataSource;
			ResourceTree.RefreshDataSource();
			ResourceTree.Refresh();
			ScheduleStorageGanttChart.EndUpdate();
			DataTable dt = await HttpClient.Capacity.GetDataAppointments(Function.ParseInt(txtScheCode.EditValue));
			if (dt.IsNotNull())
			{
				if (dt.AsEnumerable().All((DataRow t) => Function.ParseInt(t["GrcId"]) == 0) && dt.AsEnumerable().Any((DataRow t) => Function.ToString(t["IsBase"]).Equals("N") && Function.ToString(t["LineType"]).Equals("C")))
				{
					ProcessData(ref dt);
				}
				else
				{
					DataRow dataRow = dt.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["GrcId"]) > 0);
					if (dataRow != null)
					{
						txtCapacityCode.EditValue = dataRow["GrcId"];
					}
					UpdateStatusOverLoad(ref dt);
				}
				ScheduleGanttChart.Start = (dt.AsEnumerable().Any((DataRow t) => Function.ToString(t["IsBase"]).Equals("N") && Function.ToString(t["LineType"]).Equals("C")) ? (from t in dt.AsEnumerable()
					where Function.ToString(t["IsBase"]).Equals("N") && Function.ToString(t["LineType"]).Equals("C")
					select t).Min((DataRow t) => Function.ParseDateTimes(t["StartDate"])) : DateTime.Now.Date);
				btnRun.Enabled = !dt.AsEnumerable().Any((DataRow t) => Function.ToString(t["LineType"]).Equals("C") && Function.ParseInt(t["LinkEntry"]) > 0);
				FormMode = (dt.AsEnumerable().Any((DataRow t) => Function.ParseInt(t["GrcId"]) > 0) ? Enums.FormViewModeType.View : Enums.FormViewModeType.Add);
			}
			else
			{
				ScheduleGanttChart.Start = DateTime.Now.Date;
				FormMode = Enums.FormViewModeType.View;
			}
			dt.AcceptChanges();
			ScheduleStorageGanttChart.BeginUpdate();
			ScheduleStorageGanttChart.Appointments.DataSource = dt;
			ScheduleStorageGanttChart.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CustomAppointmentSort(object sender, CustomAppointmentSortEventArgs e)
	{
		try
		{
			IAppointmentLayoutInfo appointmentLayoutInfo = e.AppointmentLayoutInfo1;
			IAppointmentLayoutInfo appointmentLayoutInfo2 = e.AppointmentLayoutInfo2;
			if (Function.ToString(appointmentLayoutInfo.Appointment.CustomFields["IsBase"]).Equals("Y"))
			{
				e.Result = -1;
			}
			else if (Function.ToString(appointmentLayoutInfo2.Appointment.CustomFields["IsBase"]).Equals("Y"))
			{
				e.Result = 1;
			}
			else if (appointmentLayoutInfo.Start == appointmentLayoutInfo2.Start)
			{
				e.Result = ((appointmentLayoutInfo.End > appointmentLayoutInfo2.End) ? 1 : ((!(appointmentLayoutInfo.End == appointmentLayoutInfo2.End)) ? (-1) : 0));
			}
			else
			{
				e.Result = ((appointmentLayoutInfo.Start > appointmentLayoutInfo2.Start) ? 1 : (-1));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void ScheduleGanttChart_DragDrop(object sender, DragEventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmChangeCalendar, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				e.Effect = DragDropEffects.None;
				return;
			}
			Point pt = ScheduleGanttChart.PointToClient(new Point(e.X, e.Y));
			SchedulerHitInfo schedulerHitInfo = ScheduleGanttChart.ActiveView.ViewInfo.CalcHitInfo(pt, layoutOnly: true);
			if (!(e.Data.GetData(typeof(SchedulerDragData)) is SchedulerDragData schedulerDragData))
			{
				return;
			}
			if (!Function.ToString(schedulerDragData.PrimaryAppointment.StatusKey).Equals("N"))
			{
				WinCommons.Warning(Enums.MessageCode.CannotUpdateCompleteApointment, "");
				e.Effect = DragDropEffects.None;
				return;
			}
			string value = Function.ToString(schedulerDragData.PrimaryAppointment.ResourceId);
			string text = Function.ToString(schedulerHitInfo.ViewInfo.Resource.Id);
			if (!text.Equals(value) && !HttpClient.Capacity.GetDataResourceEffic(text, Function.ToString(schedulerDragData.PrimaryAppointment.CustomFields["RoutCode"]), Function.ParseInt(schedulerDragData.PrimaryAppointment.CustomFields["RoutLine"])).IsNotNull())
			{
				WinCommons.Warning(Enums.MessageCode.ResourceDoesNotAssign2ProduceThisItem, "", text);
				e.Effect = DragDropEffects.None;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			e.Effect = DragDropEffects.None;
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ScheduleGanttChart_AppointmentDrag(object sender, AppointmentDragEventArgs e)
	{
		try
		{
			if (Function.ToString(e.SourceAppointment.CustomFields["IsBase"]).Equals("Y") || new string[2] { "G", "D" }.Contains(Function.ToString(e.SourceAppointment.StatusKey)))
			{
				e.Allow = false;
				return;
			}
			e.AdditionalAppointments.Clear();
			if (Function.ToString(e.SourceAppointment.ResourceId).Equals(Function.ToString(e.EditedAppointment.ResourceId)) || !(e.SourceAppointment.Start == e.EditedAppointment.Start))
			{
				AppointmentBaseCollection appointments = ScheduleGanttChart.ActiveView.GetAppointments();
				string demandType = Function.ToString(e.SourceAppointment.CustomFields["DemandType"]);
				int demandEntry = Function.ParseInt(e.SourceAppointment.CustomFields["DemandEntry"]);
				int demandLine = Function.ParseInt(e.SourceAppointment.CustomFields["DemandLine"]);
				List<Appointment> lstAppointments = appointments.Where((Appointment t) => Function.ToString(t.CustomFields["DemandType"]).Equals(demandType) && Function.ParseInt(t.CustomFields["DemandEntry"]) == demandEntry && Function.ParseInt(t.CustomFields["DemandLine"]) == demandLine).ToList();
				DataTable dataTable = ScheduleStorageGanttChart.Appointments.DataSource as DataTable;
				DataRow childAppointment = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["Line"]) == Function.ParseInt(e.SourceAppointment.Id));
				switch (_dragType)
				{
				case DragType.Backward:
				case DragType.BackwardThenForward:
					AddChildApointmentBackward(e, childAppointment, lstAppointments, dataTable);
					break;
				case DragType.Forward:
				case DragType.ForwardThenBackward:
					AddFatherApointmentForward(e, childAppointment, lstAppointments, dataTable);
					break;
				case DragType.None:
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ScheduleGanttChart_AdditionalAppointmentsDrag(object sender, AdditionalAppointmentsDragEventArgs e)
	{
		try
		{
			for (int i = 0; i < e.AdditionalAppointmentInfos.Count; i++)
			{
				e.AdditionalAppointmentInfos[i].EditedAppointment.ResourceId = e.AdditionalAppointmentInfos[i].SourceAppointment.ResourceId;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btnSave_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			DataTable dataTable = ScheduleStorageGanttChart.Appointments.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.SaveDataCanNotBeNull, "");
				return;
			}
			WinCommons._OpenProcessing();
			EnumerableRowCollection<DataRow> source = from t in dataTable.AsEnumerable()
				where Function.ToString(t["LineType"]).Equals("C")
				select t;
			if (!source.Any())
			{
				WinCommons.Error(Enums.MessageCode.SaveDataCanNotBeNull, "");
				return;
			}
			string text = await HttpClient.Capacity.Save(source.CopyToDataTable());
			if (string.IsNullOrEmpty(text))
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void toolTipController1_BeforeShow(object sender, ToolTipControllerShowEventArgs e)
	{
		try
		{
			if (!(toolTipController1.ActiveObject is AppointmentViewInfo))
			{
				return;
			}
			Appointment appointment = ((AppointmentViewInfo)toolTipController1.ActiveObject).Appointment;
			if (appointment == null || string.IsNullOrEmpty(appointment.Subject) || Function.ToString(appointment.CustomFields["IsBase"]).Equals("Y"))
			{
				return;
			}
			if (toolTipController1.ToolTipType == ToolTipType.Standard)
			{
				e.ToolTip = appointment.Description;
			}
			int id = Function.ParseInt(appointment.Id);
			DataTable dataTable = ScheduleStorageGanttChart.Appointments.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				return;
			}
			DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["Line"]) == id);
			if (dataRow == null || toolTipController1.ToolTipType != ToolTipType.SuperTip)
			{
				return;
			}
			SuperToolTip superToolTip = new SuperToolTip();
			SuperToolTipSetupArgs superToolTipSetupArgs = new SuperToolTipSetupArgs();
			superToolTipSetupArgs.Title.Text = "Item Code: " + appointment.Subject;
			superToolTipSetupArgs.Title.Font = new Font("Tahoma", Function.ParseFloat(8.25), FontStyle.Bold);
			superToolTipSetupArgs.ShowFooterSeparator = true;
			string text = "";
			if (_dtTooltipSetting.IsNotNull())
			{
				for (int num = 0; num < _dtTooltipSetting.Rows.Count; num++)
				{
					string columnName = Function.ToString(_dtTooltipSetting.Rows[num]["ColName"]);
					string arg = Function.ToString(_dtTooltipSetting.Rows[num]["ColText"]);
					text = string.Format("{2}{0}: {1}\r\n", arg, Function.ToString(dataRow[columnName]), text);
				}
			}
			else
			{
				text = string.Format("Item Name: {0} \r\nOperation: {1}\r\nWork Shift: {2}\r\nQuantity: {3}\r\nTime run: {4}", Function.ToString(dataRow["ItemName"]), Function.ToString(dataRow["OpeName"]), Function.ToString(dataRow["Shift"]), Function.ToString(dataRow["Qty"]), Function.ToString(dataRow["TimeRun"]));
			}
			superToolTipSetupArgs.Contents.Text += text;
			if (appointment.Start.Date != appointment.End.Date)
			{
				superToolTipSetupArgs.Footer.Text = appointment.Start.Date.ToShortDateString() + " " + appointment.Start.ToShortTimeString() + " - " + appointment.End.Date.ToShortDateString() + " " + appointment.End.ToShortTimeString();
			}
			else
			{
				string text2 = GlobalData.DateTimeFormat + " HH:mm";
				superToolTipSetupArgs.Footer.Text = appointment.Start.ToString(text2) + " - " + appointment.End.ToString(text2);
			}
			superToolTipSetupArgs.Footer.Font = new Font("Tahoma", Function.ParseFloat(7.25));
			superToolTip.Setup(superToolTipSetupArgs);
			superToolTip.AllowHtmlText = DefaultBoolean.True;
			e.SuperTip = superToolTip;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void ScheduleStorageGanttChart_AppointmentsChanged(object sender, PersistentObjectsEventArgs e)
	{
		try
		{
			DataTable dt = ScheduleStorageGanttChart.Appointments.DataSource as DataTable;
			if (!dt.IsNotNull() || _dragType == DragType.None)
			{
				return;
			}
			dt.AcceptChanges();
			ScheduleStorageGanttChart.BeginUpdate();
			Appointment fatherAppointment = null;
			if (ScheduleGanttChart.SelectedAppointments.Any() && ScheduleGanttChart.SelectedAppointments[0] != null)
			{
				fatherAppointment = ScheduleGanttChart.SelectedAppointments[0];
			}
			WinCommons._OpenProcessing();
			if (fatherAppointment != null)
			{
				DataRow row = dt.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["Line"]) == Function.ParseInt(fatherAppointment.Id));
				if (row != null)
				{
					DataTable dataResourceEffic = HttpClient.Capacity.GetDataResourceEffic(Function.ToString(row["ResCode"]), Function.ToString(row["RoutCode"]), Function.ParseInt(row["RoutLine"]));
					double num = Function.ParseDouble(row["Qty"]) * Function.ParseDouble(dataResourceEffic.Rows[0]["BaseTime"]) * 100.0 / (Function.ParseDouble(dataResourceEffic.Rows[0]["InvQty"]) * Function.ParseDouble(dataResourceEffic.Rows[0]["Effic"]));
					row["TimeRun"] = num;
					List<DateTime> list = ((_dragType == DragType.Backward || _dragType == DragType.BackwardThenForward) ? GetForwardUpdateDate(dt, Function.ToString(row["ResCode"]), Function.ParseDateTimes(row["StartDate"]), Function.ParseDouble(row["TimeRun"])) : GetBackwardUpdateDate(dt, Function.ToString(row["ResCode"]), Function.ParseDateTimes(row["EndDate"]), Function.ParseDouble(row["TimeRun"])));
					if (list.Count > 1)
					{
						row["StartDate"] = list[0];
						row["EndDate"] = list[1];
					}
					DateTime dateTime = Function.ParseDateTimes(row["StartDate"]);
					DateTime dateTime2 = Function.ParseDateTimes(row["EndDate"]);
					row["StartTime"] = dateTime.Hour * 100 + dateTime.Minute;
					row["EndTime"] = dateTime2.Hour * 100 + dateTime2.Minute;
					row["WorkDay"] = dateTime.Date;
					row["EndWorkDay"] = dateTime2.Date;
					string demandType = Function.ToString(row["DemandType"]);
					int demandEntry = Function.ParseInt(row["DemandEntry"]);
					int demandLine = Function.ParseInt(row["DemandLine"]);
					if (!string.IsNullOrEmpty(demandType) && !demandType.Equals("ZZ"))
					{
						List<DataRow> list2 = (from t in dt.AsEnumerable()
							where Function.ToString(t["DemandType"]).Equals(demandType) && Function.ParseInt(t["DemandEntry"]) == demandEntry && Function.ParseInt(t["DemandLine"]) == demandLine
							select t).ToList();
						if (_dragType == DragType.Backward || _dragType == DragType.BackwardThenForward)
						{
							List<DataRow> lstSaveLevel = (from t in list2
								where Function.ParseInt(t["ProId"]) == Function.ParseInt(row["ProId"]) && Function.ParseInt(t["OpeOrder"]) <= Function.ParseInt(row["OpeOrder"])
								orderby Function.ParseInt(t["OpeOrder"]) descending
								select t).ToList();
							UpdateTimeDataApointmnetBackward(lstSaveLevel, dt, list2);
						}
						else
						{
							List<DataRow> lstSaveLevel2 = (from t in list2
								where Function.ParseInt(t["ProId"]) == Function.ParseInt(row["ProId"]) && Function.ParseInt(t["OpeOrder"]) >= Function.ParseInt(row["OpeOrder"])
								orderby Function.ParseInt(t["OpeOrder"])
								select t).ToList();
							UpdateTimeDataApointmnetForward(lstSaveLevel2, dt, list2);
						}
					}
				}
			}
			UpdateStatusOverLoad(ref dt);
			dt.AcceptChanges();
			ScheduleStorageGanttChart.RefreshData();
			ScheduleStorageGanttChart.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private async void btnRun_Click(object sender, EventArgs e)
	{
		_hilightApoinment.DemandType = "";
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRun, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			ScheduleStorageGanttChart.BeginUpdate();
			DataTable dtAppointments = await HttpClient.Capacity.GetDataAppointments(Function.ParseInt(txtScheCode.EditValue), Enums.TransactionType.R);
			if (dtAppointments.IsNotNull() && dtAppointments.AsEnumerable().Any((DataRow t) => Function.ToString(t["IsBase"]).Equals("N") && Function.ToString(t["LineType"]).Equals("C")))
			{
				ProcessData(ref dtAppointments);
				ScheduleGanttChart.Start = (dtAppointments.AsEnumerable().Any((DataRow t) => Function.ToString(t["IsBase"]).Equals("N") && Function.ToString(t["LineType"]).Equals("C")) ? (from t in dtAppointments.AsEnumerable()
					where Function.ToString(t["IsBase"]).Equals("N") && Function.ToString(t["LineType"]).Equals("C")
					select t).Min((DataRow t) => Function.ParseDateTimes(t["StartDate"])) : DateTime.Now.Date);
			}
			else
			{
				ScheduleGanttChart.Start = DateTime.Now.Date;
			}
			dtAppointments.AcceptChanges();
			ScheduleStorageGanttChart.Appointments.DataSource = dtAppointments;
			FormMode = ((FormMode == Enums.FormViewModeType.View) ? Enums.FormViewModeType.Update : Enums.FormViewModeType.Add);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		ScheduleStorageGanttChart.EndUpdate();
		WinCommons._CloseProcessing();
	}

	private void ScheduleGanttChart_Click(object sender, EventArgs e)
	{
		_hilightApoinment.DemandType = "";
		try
		{
			if (ScheduleGanttChart.ActiveView.ViewInfo.CalcHitInfo(ScheduleGanttChart.PointToClient(Control.MousePosition), layoutOnly: false).HitTest == SchedulerHitTest.AppointmentContent && ScheduleGanttChart.SelectedAppointments.Any() && ScheduleGanttChart.SelectedAppointments[0] != null)
			{
				int apointmentId = Function.ParseInt(ScheduleGanttChart.SelectedAppointments[0].Id);
				_hilightApoinment.ApointmentId = apointmentId;
				_hilightApoinment.ApontmentItemCode = Function.ToString(ScheduleGanttChart.SelectedAppointments[0].Subject);
				_hilightApoinment.DemandType = Function.ToString(ScheduleGanttChart.SelectedAppointments[0].CustomFields["DemandType"]);
				_hilightApoinment.DemandEntry = Function.ParseInt(ScheduleGanttChart.SelectedAppointments[0].CustomFields["DemandEntry"]);
				_hilightApoinment.DemandLine = Function.ParseInt(ScheduleGanttChart.SelectedAppointments[0].CustomFields["DemandLine"]);
				_hilightApoinment.FatherCode = Function.ToString(ScheduleGanttChart.SelectedAppointments[0].CustomFields["FatherCode"]);
				_hilightApoinment.FatherLine = Function.ParseInt(ScheduleGanttChart.SelectedAppointments[0].CustomFields["FatherLine"]);
				_hilightApoinment.OpeOrder = Function.ParseInt(ScheduleGanttChart.SelectedAppointments[0].CustomFields["OpeOrder"]);
				_hilightApoinment.ProId = Function.ParseInt(ScheduleGanttChart.SelectedAppointments[0].CustomFields["ProId"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		ScheduleGanttChart.Refresh();
	}

	private void ScheduleGanttChart_AppointmentViewInfoCustomizing(object sender, AppointmentViewInfoCustomizingEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(_hilightApoinment.DemandType))
			{
				return;
			}
			if (Function.ToString(e.ViewInfo.Appointment.CustomFields["IsBase"]).Equals("Y"))
			{
				e.ViewInfo.BackColor = Color.WhiteSmoke;
				return;
			}
			switch (CheckHilightType(Function.ParseInt(e.ViewInfo.Appointment.Id)))
			{
			case HilightType.Current:
				e.ViewInfo.BackColor = Color.DarkRed;
				break;
			case HilightType.Father:
				e.ViewInfo.BackColor = Color.DarkSeaGreen;
				break;
			case HilightType.Child:
				e.ViewInfo.BackColor = Color.DeepSkyBlue;
				break;
			default:
				e.ViewInfo.BackColor = Color.WhiteSmoke;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ScheduleGanttChart_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			if (ScheduleGanttChart.ActiveView.ViewInfo.CalcHitInfo(ScheduleGanttChart.PointToClient(Control.MousePosition), layoutOnly: false).HitTest == SchedulerHitTest.AppointmentContent)
			{
				HandleEditClik(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ScheduleStorageGanttChart_AppointmentDeleting(object sender, PersistentObjectCancelEventArgs e)
	{
		e.Cancel = true;
	}

	private void ScheduleGanttChart_PopupMenuShowing(object sender, DevExpress.XtraScheduler.PopupMenuShowingEventArgs e)
	{
		try
		{
			if (e.Menu.Id == SchedulerMenuItemId.DefaultMenu)
			{
				e.Menu.RemoveMenuItem(SchedulerMenuItemId.NewAllDayEvent);
				e.Menu.RemoveMenuItem(SchedulerMenuItemId.NewRecurringAppointment);
				SchedulerMenuItem menuItemById = e.Menu.GetMenuItemById(SchedulerMenuItemId.NewAppointment);
				if (menuItemById != null)
				{
					menuItemById.Caption = "New Schedule Item";
				}
				e.Menu.Items.Add(new SchedulerMenuItem("Complete Date", HandleCompleteDate));
				e.Menu.Items.Add(new SchedulerMenuItem("UnComplete Date", HandleUnCompleteDate));
				e.Menu.Items.Add(new SchedulerMenuItem("Capacity Setting", HandleCapacitySetting));
				if (SplitApointment.PanelVisibility == SplitPanelVisibility.Panel1)
				{
					e.Menu.Items.Insert(3, new SchedulerMenuItem("Open Operation Detail", HandleOpenOperationDetailClik));
				}
			}
			if (e.Menu.Id != SchedulerMenuItemId.AppointmentMenu)
			{
				return;
			}
			for (int i = 1; i < e.Menu.Items.Count; i++)
			{
				e.Menu.Items[i].Visible = false;
			}
			AppointmentBaseCollection selectedAppointments = ScheduleGanttChart.SelectedAppointments;
			if (selectedAppointments.Count <= 0 || !Function.ToString(selectedAppointments[0].CustomFields["IsBase"]).Equals("N"))
			{
				return;
			}
			if (new string[2] { "N", "C" }.Contains(Function.ToString(selectedAppointments[0].StatusKey)))
			{
				e.Menu.Items.Insert(1, new SchedulerMenuItem("Edit", HandleEditClik));
				e.Menu.Items.Insert(2, new SchedulerMenuItem("Delete", HandleDeleteClik));
				if (Function.ToString(selectedAppointments[0].StatusKey).Equals("N"))
				{
					e.Menu.Items.Add(new SchedulerMenuItem("Complete", HandleCompleteClik));
				}
				else
				{
					e.Menu.Items.Add(new SchedulerMenuItem("UnComplete", HandleUnCompleteClik));
				}
			}
			else if (Function.ToString(selectedAppointments[0].StatusKey).Equals("L"))
			{
				e.Menu.Items.Add(new SchedulerMenuItem("Reopen", HandleReopenClik));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void HandleUnCompleteClik(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = ScheduleStorageGanttChart.Appointments.DataSource as DataTable;
			if (ScheduleGanttChart.SelectedAppointments.Any() && ScheduleGanttChart.SelectedAppointments[0] != null && dataTable.IsNotNull())
			{
				Appointment apointment = ScheduleGanttChart.SelectedAppointments[0];
				ScheduleStorageGanttChart.BeginUpdate();
				List<DataRow> list = (from t in dataTable.AsEnumerable()
					where Function.ToString(t["Complete"]).Equals("C") && Function.ParseInt(t["ProId"]) == Function.ParseInt(apointment.CustomFields["ProId"])
					select t).ToList();
				for (int num = 0; num < list.Count; num++)
				{
					list[num]["Complete"] = "N";
				}
				dataTable.AcceptChanges();
				ScheduleStorageGanttChart.EndUpdate();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	public void HandleCompleteClik(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = ScheduleStorageGanttChart.Appointments.DataSource as DataTable;
			if (ScheduleGanttChart.SelectedAppointments.Any() && ScheduleGanttChart.SelectedAppointments[0] != null && dataTable.IsNotNull())
			{
				Appointment apointment = ScheduleGanttChart.SelectedAppointments[0];
				ScheduleStorageGanttChart.BeginUpdate();
				List<DataRow> list = (from t in dataTable.AsEnumerable()
					where Function.ToString(t["Complete"]).Equals("N") && Function.ParseInt(t["ProId"]) == Function.ParseInt(apointment.CustomFields["ProId"])
					select t).ToList();
				for (int num = 0; num < list.Count; num++)
				{
					list[num]["Complete"] = "C";
				}
				dataTable.AcceptChanges();
				ScheduleStorageGanttChart.EndUpdate();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	public void HandleUnCompleteDate(object sender, EventArgs e)
	{
		try
		{
			TimeInterval selectedInterval = ScheduleGanttChart.SelectedInterval;
			DataTable dataTable = ScheduleStorageGanttChart.Appointments.DataSource as DataTable;
			if (selectedInterval == null || !dataTable.IsNotNull())
			{
				return;
			}
			DateTime currentDate = selectedInterval.Start.Date;
			List<DataRow> lstComp = dataTable.AsEnumerable().Where((DataRow t) =>
			{
				if (Function.ToString(t["IsBase"]).Equals("N") && Function.ToString(t["Complete"]).Equals("C"))
				{
					DateTime? dateTime = Function.ParseDateTime(t["WorkDay"]);
					DateTime dateTime2 = currentDate;
					if (!dateTime.HasValue)
					{
						return false;
					}
					if (!dateTime.HasValue)
					{
						return true;
					}
					return dateTime.GetValueOrDefault() == dateTime2;
				}
				return false;
			}).ToList();
			ScheduleStorageGanttChart.BeginUpdate();
			int i;
			for (i = 0; i < lstComp.Count; i++)
			{
				lstComp[i]["Complete"] = "N";
				List<DataRow> list = (from t in dataTable.AsEnumerable()
					where Function.ToString(t["Complete"]).Equals("C") && Function.ParseInt(t["ProId"]) == Function.ParseInt(lstComp[i]["ProId"])
					select t).ToList();
				for (int num = 0; num < list.Count; num++)
				{
					list[num]["Complete"] = "N";
				}
			}
			dataTable.AcceptChanges();
			ScheduleStorageGanttChart.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	public void HandleCapacitySetting(object sender, EventArgs e)
	{
		try
		{
			new frm_CapacitySetting().ShowDialog(this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	public void HandleCompleteDate(object sender, EventArgs e)
	{
		try
		{
			TimeInterval selectedInterval = ScheduleGanttChart.SelectedInterval;
			DataTable dataTable = ScheduleStorageGanttChart.Appointments.DataSource as DataTable;
			if (selectedInterval == null || !dataTable.IsNotNull())
			{
				return;
			}
			DateTime currentDate = selectedInterval.Start.Date;
			List<DataRow> lstComp = dataTable.AsEnumerable().Where((DataRow t) =>
			{
				if (Function.ToString(t["IsBase"]).Equals("N") && Function.ToString(t["Complete"]).Equals("N"))
				{
					DateTime? dateTime = Function.ParseDateTime(t["WorkDay"]);
					DateTime dateTime2 = currentDate;
					if (!dateTime.HasValue)
					{
						return false;
					}
					if (!dateTime.HasValue)
					{
						return true;
					}
					return dateTime.GetValueOrDefault() == dateTime2;
				}
				return false;
			}).ToList();
			ScheduleStorageGanttChart.BeginUpdate();
			int i;
			for (i = 0; i < lstComp.Count; i++)
			{
				lstComp[i]["Complete"] = "C";
				List<DataRow> list = (from t in dataTable.AsEnumerable()
					where Function.ToString(t["Complete"]).Equals("N") && Function.ParseInt(t["ProId"]) == Function.ParseInt(lstComp[i]["ProId"])
					select t).ToList();
				for (int num = 0; num < list.Count; num++)
				{
					list[num]["Complete"] = "C";
				}
			}
			dataTable.AcceptChanges();
			ScheduleStorageGanttChart.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	public void HandleDeleteClik(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			if (ScheduleGanttChart.SelectedAppointments.Any() && ScheduleGanttChart.SelectedAppointments[0] != null)
			{
				int id = Function.ParseInt(ScheduleGanttChart.SelectedAppointments[0].Id);
				DataTable dt = ScheduleStorageGanttChart.Appointments.DataSource as DataTable;
				if (dt.IsNotNull())
				{
					DataRow row = dt.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["Line"]) == id);
					if (row != null)
					{
						if (Function.ParseInt(row["LinkEntry"]) > 0)
						{
							WinCommons.Warning(Enums.MessageCode.ItemLink2ProductionOrderCannotDelete, "");
							return;
						}
						ScheduleStorageGanttChart.BeginUpdate();
						if (Function.ToString(row["DemandType"]).Equals("ZZ") || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteChildrenItem, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
						{
							List<DataRow> list = (from t in dt.AsEnumerable()
								where Function.ParseInt(t["ProId"]) == Function.ParseInt(row["ProId"])
								select t).ToList();
							for (int num = 0; num < list.Count; num++)
							{
								if (Function.ParseInt(list[num]["LinkEntry"]) == 0)
								{
									dt.Rows.Remove(list[num]);
								}
							}
						}
						else
						{
							List<DataRow> lstChild = (from t in dt.AsEnumerable()
								where Function.ToString(t["DemandType"]).Equals(Function.ToString(row["DemandType"])) && Function.ParseInt(t["DemandEntry"]) == Function.ParseInt(row["DemandEntry"]) && Function.ParseInt(t["DemandLine"]) == Function.ParseInt(row["DemandLine"])
								select t).ToList();
							DeleteAppointment(ref dt, ref lstChild, Function.ToString(row["ItemCode"]), Function.ParseInt(row["Line"]), Function.ParseInt(row["ProId"]));
						}
						UpdateStatusOverLoad(ref dt);
						dt.AcceptChanges();
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_hilightApoinment.DemandType = "";
		ScheduleStorageGanttChart.EndUpdate();
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	public void HandleEditClik(object sender, EventArgs e)
	{
		try
		{
			if (ScheduleGanttChart.SelectedAppointments.Any() && ScheduleGanttChart.SelectedAppointments[0] != null)
			{
				int id = Function.ParseInt(ScheduleGanttChart.SelectedAppointments[0].Id);
				DataTable dt = ScheduleStorageGanttChart.Appointments.DataSource as DataTable;
				if (dt.IsNotNull())
				{
					DataRow dataRow = dt.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["Line"]) == id);
					if (dataRow != null)
					{
						if (Function.ToString(dataRow["IsBase"]).Equals("Y"))
						{
							return;
						}
						if (!Function.ToString(dataRow["Complete"]).Equals("N"))
						{
							WinCommons.Warning(Enums.MessageCode.CannotEditCompleteAppointment, "");
							return;
						}
						DataTable dataTable = dt.Clone();
						dataTable.ImportRow(dataRow);
						frm_AppointmentEditting frm_AppointmentEditting2 = new frm_AppointmentEditting(dataTable);
						frm_AppointmentEditting2.ShowDialog(this);
						if (frm_AppointmentEditting2.DialogResult != DialogResult.OK)
						{
							return;
						}
						WinCommons._OpenProcessing();
						ScheduleStorageGanttChart.BeginUpdate();
						if (Function.ToString(dataRow["DemandType"]).Equals("ZZ"))
						{
							dt.Rows.Remove(dataRow);
							for (int num = 0; num < frm_AppointmentEditting2.DataAppointment.Rows.Count; num++)
							{
								DataRow dataRow2 = dt.NewRow();
								dataRow2.ItemArray = frm_AppointmentEditting2.DataAppointment.Rows[num].ItemArray;
								dataRow2["Line"] = dt.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["Line"])) + 1;
								dataRow2["ProId"] = dt.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["ProId"])) + 1;
								List<DateTime> forwardUpdateDate = GetForwardUpdateDate(dt, Function.ToString(dataRow2["ResCode"]), Function.ParseDateTimes(dataRow2["StartDate"]), Function.ParseDouble(dataRow2["TimeRun"]));
								if (forwardUpdateDate.Count > 1)
								{
									dataRow2["StartDate"] = forwardUpdateDate[0];
									dataRow2["WorkDay"] = forwardUpdateDate[0].Date;
									dataRow2["StartTime"] = forwardUpdateDate[0].Hour * 100 + forwardUpdateDate[0].Minute;
									dataRow2["EndDate"] = forwardUpdateDate[1];
									dataRow2["EndWorkDay"] = forwardUpdateDate[1].Date;
									dataRow2["EndTime"] = forwardUpdateDate[1].Hour * 100 + forwardUpdateDate[1].Minute;
								}
								dt.Rows.Add(dataRow2);
							}
						}
						else
						{
							string demandType = Function.ToString(dataRow["DemandType"]);
							int demandEntry = Function.ParseInt(dataRow["DemandEntry"]);
							int demandLine = Function.ParseInt(dataRow["DemandLine"]);
							string fatherItemCode = Function.ToString(dataRow["FatherCode"]);
							int fatherLine = Function.ParseInt(dataRow["FatherLine"]);
							List<DataRow> lstTree = (from t in dt.AsEnumerable()
								where Function.ToString(t["DemandType"]).Equals(demandType) && Function.ParseInt(t["DemandEntry"]) == demandEntry && Function.ParseInt(t["DemandLine"]) == demandLine
								select t).ToList();
							if (!frm_AppointmentEditting2.DataAppointment.IsNotNull())
							{
								DeleteAppointment(ref dt, ref lstTree, Function.ToString(dataRow["ItemCode"]), Function.ParseInt(dataRow["Line"]), Function.ParseInt(dataRow["ProId"]));
							}
							else
							{
								List<DataRow> lstDeleteRow = new List<DataRow> { dataRow };
								DateTime? dateTime = null;
								for (int num2 = 0; num2 < frm_AppointmentEditting2.DataAppointment.Rows.Count; num2++)
								{
									DateTime? endDate = null;
									SeparationAppointment(ref dt, ref lstDeleteRow, ref lstTree, ref endDate, Function.ParseInt(dataRow["Line"]), Function.ParseInt(dataRow["ProId"]), Function.ToString(dataRow["ItemCode"]), Function.ToString(dataRow["BomVersion"]), Function.ParseInt(dataRow["FatherLine"]), Function.ParseDouble(frm_AppointmentEditting2.DataAppointment.Rows[num2]["Qty"]), Function.ParseDateTimes(frm_AppointmentEditting2.DataAppointment.Rows[num2]["StartDate"]), backward: true);
									if (endDate.HasValue && (!dateTime.HasValue || dateTime < endDate))
									{
										dateTime = endDate;
									}
								}
								for (int num3 = 0; num3 < lstDeleteRow.Count; num3++)
								{
									dt.Rows.Remove(lstDeleteRow[num3]);
								}
								if ((_dragType == DragType.Forward || _dragType == DragType.ForwardThenBackward) && dateTime.HasValue)
								{
									List<DataRow> lstSource = (from t in dt.AsEnumerable()
										where Function.ToString(t["DemandType"]).Equals(demandType) && Function.ParseInt(t["DemandEntry"]) == demandEntry && Function.ParseInt(t["DemandLine"]) == demandLine
										select t).ToList();
									SetTimeByFatherTimeForward(ref lstSource, dt, fatherItemCode, dateTime.Value, fatherLine);
								}
							}
						}
						UpdateStatusOverLoad(ref dt);
						dt.AcceptChanges();
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_hilightApoinment.DemandType = "";
		ScheduleStorageGanttChart.EndUpdate();
		WinCommons._CloseProcessing();
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	public void HandleReopenClik(object sender, EventArgs e)
	{
		try
		{
			DataTable dt = ScheduleStorageGanttChart.Appointments.DataSource as DataTable;
			if (ScheduleGanttChart.SelectedAppointments.Any() && ScheduleGanttChart.SelectedAppointments[0] != null && dt.IsNotNull())
			{
				Appointment apointment = ScheduleGanttChart.SelectedAppointments[0];
				ScheduleStorageGanttChart.BeginUpdate();
				List<DataRow> list = (from t in dt.AsEnumerable()
					where Function.ParseInt(t["ProId"]) == Function.ParseInt(apointment.CustomFields["ProId"])
					select t).ToList();
				for (int num = 0; num < list.Count; num++)
				{
					int oldLine = Function.ParseInt(list[num]["Line"]);
					int num2 = dt.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["Line"])) + 1;
					list[num]["Complete"] = "N";
					list[num]["Line"] = num2;
					List<DataRow> list2 = (from t in dt.AsEnumerable()
						where Function.ParseInt(t["FatherLine"]) == oldLine
						select t).ToList();
					for (int num3 = 0; num3 < list2.Count; num3++)
					{
						list2[num3]["FatherLine"] = num2;
					}
				}
				UpdateStatusOverLoad(ref dt);
				dt.AcceptChanges();
				ScheduleStorageGanttChart.EndUpdate();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	public void HandleOpenOperationDetailClik(object sender, EventArgs e)
	{
		try
		{
			SplitApointment.PanelVisibility = SplitPanelVisibility.Both;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtScheCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.IsLeft)
			{
				this.DrillDown(ApzObjectTypes.oCapacity, ApzObjectTypes.oSchedule, Function.ToString(txtScheCode.EditValue));
				return;
			}
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Schedule, false, txtScheCode.Text, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ScheduleStatus = "A";
			frm_CFL2.ScheduleType = "O";
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				txtScheCode.EditValue = frm_CFL2.ChooseItem["PSCode"];
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtScheCode_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void btnViewDetail_Click(object sender, EventArgs e)
	{
		SplitApointment.PanelVisibility = SplitPanelVisibility.Panel1;
	}

	private void frm_CapacityGantt_Resize(object sender, EventArgs e)
	{
		if (SplitApointment.Panel2.Visible)
		{
			SplitApointment.SplitterPosition = SplitApointment.Size.Width - 200;
		}
	}

	private void SeparationAppointment(ref DataTable dt, ref List<DataRow> lstDeleteRow, ref List<DataRow> lstTree, ref DateTime? endDate, int currentLine, int proId, string itemCode, string version, int fatherLine, double qty, DateTime startDate, bool backward = false)
	{
		DateTime dateTime = startDate;
		IEnumerable<DataRow> source = lstTree.Where((DataRow t) => Function.ParseInt(t["ProId"]) == proId);
		if (source.Any())
		{
			List<DataRow> lstSort = source.OrderBy((DataRow t) => Function.ParseInt(t["OpeOrder"])).ToList();
			int num = dt.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["ProId"])) + 1;
			int i;
			for (i = 0; i < lstSort.Count; i++)
			{
				if (Function.ParseInt(lstSort[i]["LinkEntry"]) > 0)
				{
					return;
				}
				if (i > 0 && Function.ParseInt(lstSort[i]["OpeOrder"]) < Function.ParseInt(lstSort[i - 1]["OpeOrder"]))
				{
					startDate = dateTime;
				}
				DataRow dataRow = dt.NewRow();
				dataRow.ItemArray = lstSort[i].ItemArray;
				dataRow["Qty"] = qty;
				dataRow["TimeRun"] = Function.ParseDouble(dataRow["Qty"]) * Function.ParseDouble(lstSort[i]["TimeRun"]) / Function.ParseDouble(lstSort[i]["Qty"]);
				lstSort[i]["Qty"] = Function.ParseDouble(lstSort[i]["Qty"]) - Function.ParseDouble(dataRow["Qty"]);
				lstSort[i]["TimeRun"] = Function.ParseDouble(lstSort[i]["TimeRun"]) - Function.ParseDouble(dataRow["TimeRun"]);
				dataRow["Line"] = dt.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["Line"])) + 1;
				dataRow["FatherLine"] = fatherLine;
				dataRow["ProId"] = num;
				List<DateTime> list = (backward ? GetForwardUpdateDate(dt, Function.ToString(dataRow["ResCode"]), startDate, Function.ParseDouble(dataRow["TimeRun"])) : GetBackwardUpdateDate(dt, Function.ToString(dataRow["ResCode"]), startDate, Function.ParseDouble(dataRow["TimeRun"])));
				if (list.Count > 1)
				{
					dataRow["StartDate"] = list[0];
					dataRow["WorkDay"] = list[0].Date;
					dataRow["StartTime"] = list[0].Hour * 100 + list[0].Minute;
					dataRow["EndDate"] = list[1];
					dataRow["EndWorkDay"] = list[1].Date;
					dataRow["EndTime"] = list[1].Hour * 100 + list[1].Minute;
				}
				if (!endDate.HasValue || endDate.Value < Function.ParseDateTimes(dataRow["EndDate"]))
				{
					endDate = Function.ParseDateTimes(dataRow["EndDate"]);
				}
				if (i == 0 || (i > 0 && ((Function.ParseInt(lstSort[i]["OpeOrder"]) == Function.ParseInt(lstSort[i - 1]["OpeOrder"]) && dateTime > Function.ParseDateTimes(dataRow["StartDate"])) || Function.ParseInt(lstSort[i]["OpeOrder"]) < Function.ParseInt(lstSort[i - 1]["OpeOrder"]))))
				{
					dateTime = Function.ParseDateTimes(dataRow["StartDate"]);
				}
				dt.Rows.Add(dataRow);
				if (i == 0)
				{
					currentLine = Function.ParseInt(dataRow["Line"]);
				}
				if (lstDeleteRow.All((DataRow t) => Function.ParseInt(t["Line"]) != Function.ParseInt(lstSort[i]["Line"])))
				{
					lstDeleteRow.Add(lstSort[i]);
				}
			}
		}
		List<DataRow> lstChild = lstTree.Where((DataRow t) => Function.ToString(t["FatherCode"]).Equals(itemCode) && (Function.ParseInt(t["FatherLine"]) == -1 || Function.ParseInt(t["FatherLine"]) == currentLine) && Function.ParseDouble(t["Qty"]) > 0.0).ToList();
		if (!lstChild.Any())
		{
			return;
		}
		DataTable dataLineBomVersion = HttpClient.BillOfMasterials.GetDataLineBomVersion(itemCode, version);
		if (!dataLineBomVersion.IsNotNull())
		{
			return;
		}
		while (lstChild.Count > 0)
		{
			List<DataRow> lstChildSameLevel = lstChild.Where((DataRow t) => Function.ParseInt(t["ProId"]) == Function.ParseInt(lstChild[0]["ProId"])).ToList();
			if (!lstChildSameLevel.Any())
			{
				continue;
			}
			string itemCode2 = Function.ToString(lstChildSameLevel[0]["ItemCode"]);
			int currentLine2 = Function.ParseInt(lstChildSameLevel[0]["Line"]);
			int proId2 = Function.ParseInt(lstChildSameLevel[0]["ProId"]);
			string version2 = Function.ToString(lstChildSameLevel[0]["BomVersion"]);
			bool flag = !Function.ToString(lstChild[0]["Complete"]).Equals("N");
			for (int num2 = 0; num2 < lstChildSameLevel.Count; num2++)
			{
				lstChild.Remove(lstChildSameLevel[num2]);
			}
			if (!flag)
			{
				DataRow dataRow2 = dataLineBomVersion.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["ItemCode"]).Equals(lstChildSameLevel[0]["ItemCode"]));
				if (dataRow2 != null)
				{
					double qty2 = ((qty * Function.ParseDouble(dataRow2["Qty"]) < Function.ParseDouble(lstChildSameLevel[0]["Qty"])) ? (qty * Function.ParseDouble(dataRow2["Qty"])) : Function.ParseDouble(lstChildSameLevel[0]["Qty"]));
					DateTime? endDate2 = null;
					SeparationAppointment(ref dt, ref lstDeleteRow, ref lstTree, ref endDate2, currentLine2, proId2, itemCode2, version2, currentLine, qty2, dateTime);
				}
			}
		}
	}

	private void AddChildApointmentBackward(AppointmentDragEventArgs e, DataRow childAppointment, List<Appointment> lstAppointments, DataTable dtSource)
	{
		try
		{
			EnumerableRowCollection<DataRow> source = from t in dtSource.AsEnumerable()
				where Function.ParseInt(t["ProId"]) == Function.ParseInt(childAppointment["ProId"]) && Function.ParseInt(t["OpeOrder"]) < Function.ParseInt(childAppointment["OpeOrder"])
				select t;
			if (source.Any())
			{
				List<DataRow> list = source.OrderBy((DataRow t) => Function.ParseInt(t["OpeOrder"])).ToList();
				for (int num = 1; num < list.Count; num++)
				{
					int lineId = Function.ParseInt(list[num]["Line"]);
					if (e.AdditionalAppointments.All((Appointment t) => Function.ParseInt(t.Id) != lineId))
					{
						if (!Function.ToString(list[num]["Complete"]).Equals("N"))
						{
							return;
						}
						Appointment appointment = lstAppointments.FirstOrDefault((Appointment t) => Function.ParseInt(t.Id) == lineId);
						if (appointment != null)
						{
							e.AdditionalAppointments.Add(appointment);
						}
					}
				}
				int lineFather = Function.ParseInt(list[0]["Line"]);
				if (!e.AdditionalAppointments.All((Appointment t) => Function.ParseInt(t.Id) != lineFather) || !Function.ToString(list[0]["Complete"]).Equals("N"))
				{
					return;
				}
				Appointment appointment2 = lstAppointments.FirstOrDefault((Appointment t) => Function.ParseInt(t.Id) == lineFather);
				if (appointment2 != null)
				{
					e.AdditionalAppointments.Add(appointment2);
				}
				childAppointment = list[0];
			}
			string itemCode = Function.ToString(childAppointment["ItemCode"]);
			int line = Function.ParseInt(childAppointment["Line"]);
			List<DataRow> list2 = (from t in dtSource.AsEnumerable()
				where Function.ToString(t["FatherCode"]).Equals(itemCode) && (Function.ParseInt(t["FatherLine"]) == -1 || Function.ParseInt(t["FatherLine"]) == line)
				select t).ToList();
			for (int num2 = 0; num2 < list2.Count; num2++)
			{
				int childLine = Function.ParseInt(list2[num2]["Line"]);
				if (e.AdditionalAppointments.All((Appointment t) => Function.ParseInt(t.Id) != childLine) && Function.ToString(list2[num2]["Complete"]).Equals("N"))
				{
					Appointment appointment3 = lstAppointments.FirstOrDefault((Appointment t) => Function.ParseInt(t.Id) == childLine);
					if (appointment3 != null)
					{
						e.AdditionalAppointments.Add(appointment3);
					}
					AddChildApointmentBackward(e, list2[num2], lstAppointments, dtSource);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddFatherApointmentForward(AppointmentDragEventArgs e, DataRow childAppointment, List<Appointment> lstAppointments, DataTable dtSource)
	{
		try
		{
			EnumerableRowCollection<DataRow> source = from t in dtSource.AsEnumerable()
				where Function.ParseInt(t["ProId"]) == Function.ParseInt(childAppointment["ProId"]) && Function.ParseInt(t["OpeOrder"]) > Function.ParseInt(childAppointment["OpeOrder"])
				select t;
			if (source.Any())
			{
				List<DataRow> list = source.OrderByDescending((DataRow t) => Function.ParseInt(t["OpeOrder"])).ToList();
				for (int num = 1; num < list.Count; num++)
				{
					int lineId = Function.ParseInt(list[num]["Line"]);
					if (e.AdditionalAppointments.All((Appointment t) => Function.ParseInt(t.Id) != lineId))
					{
						if (!Function.ToString(list[num]["Complete"]).Equals("N"))
						{
							return;
						}
						Appointment appointment = lstAppointments.FirstOrDefault((Appointment t) => Function.ParseInt(t.Id) == lineId);
						if (appointment != null)
						{
							e.AdditionalAppointments.Add(appointment);
						}
					}
				}
				int lineFather = Function.ParseInt(list[0]["Line"]);
				if (!e.AdditionalAppointments.All((Appointment t) => Function.ParseInt(t.Id) != lineFather) || !Function.ToString(list[0]["Complete"]).Equals("N"))
				{
					return;
				}
				Appointment appointment2 = lstAppointments.FirstOrDefault((Appointment t) => Function.ParseInt(t.Id) == lineFather);
				if (appointment2 != null)
				{
					e.AdditionalAppointments.Add(appointment2);
				}
				childAppointment = list[0];
			}
			string itemCode = Function.ToString(childAppointment["FatherCode"]);
			int line = Function.ParseInt(childAppointment["FatherLine"]);
			List<DataRow> list2 = (from t in dtSource.AsEnumerable()
				where Function.ToString(t["ItemCode"]).Equals(itemCode) && (line == -1 || Function.ParseInt(t["Line"]) == line)
				select t).ToList();
			for (int num2 = 0; num2 < list2.Count; num2++)
			{
				int fatherLine = Function.ParseInt(list2[num2]["Line"]);
				if (e.AdditionalAppointments.All((Appointment t) => Function.ParseInt(t.Id) != fatherLine) && Function.ToString(list2[num2]["Complete"]).Equals("N"))
				{
					Appointment appointment3 = lstAppointments.FirstOrDefault((Appointment t) => Function.ParseInt(t.Id) == fatherLine);
					if (appointment3 != null)
					{
						e.AdditionalAppointments.Add(appointment3);
					}
					AddFatherApointmentForward(e, list2[num2], lstAppointments, dtSource);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void MappingAction()
	{
		try
		{
			DataTable dataTable = await HttpClient.Capacity.GetColorSetting();
			if (!dataTable.IsNotNull())
			{
				return;
			}
			EnumerableRowCollection<DataRow> source = from t in dataTable.AsEnumerable()
				where !Function.ToString(t["ActionId"]).Equals("ApzOverLoad") && !Function.ToString(t["ActionId"]).Equals("ApzNormal") && !Function.ToString(t["ActionId"]).Equals("ApzProd")
				select t;
			if (source.Any())
			{
				grdOpeColor.DataSource = source.CopyToDataTable();
			}
			EnumerableRowCollection<DataRow> source2 = from t in dataTable.AsEnumerable()
				where Function.ToString(t["ActionId"]).Equals("ApzOverLoad") || Function.ToString(t["ActionId"]).Equals("ApzNormal") || Function.ToString(t["ActionId"]).Equals("ApzProd")
				select t;
			if (source2.Any())
			{
				grdStatus.DataSource = source2.CopyToDataTable();
			}
			grdStatus.DataSourceTable.AcceptChanges();
			AppointmentLabelCollection labels = ScheduleStorageGanttChart.Appointments.Labels;
			labels.Clear();
			labels.BeginUpdate();
			foreach (DataRow item in dataTable.AsEnumerable())
			{
				Color labelColor = Color.FromArgb(Function.ParseInt(item["Color"]));
				string displayName = Function.ToString(item["ActionName"]);
				string menuCaption = Function.ToString(item["ActionName"]);
				string id = Function.ToString(item["ActionId"]);
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

	public void LoadResType()
	{
		List<Common.Table_Code> dataSource = new List<Common.Table_Code>
		{
			new Common.Table_Code
			{
				Code = "M",
				Name = "Machine"
			},
			new Common.Table_Code
			{
				Code = "L",
				Name = "Labor"
			},
			new Common.Table_Code
			{
				Code = "O",
				Name = "Other"
			}
		};
		rResType.DataSource = dataSource;
	}

	private void DeleteAppointment(ref DataTable dt, ref List<DataRow> lstChild, string itemCode, int line, int proId)
	{
		IEnumerable<DataRow> source = lstChild.Where((DataRow t) => Function.ParseInt(t["ProId"]) == proId);
		if (source.Any())
		{
			List<DataRow> list = source.OrderBy((DataRow t) => Function.ParseInt(t["OpeOrder"])).ToList();
			line = Function.ParseInt(list[0]["Line"]);
			for (int num = 0; num < list.Count; num++)
			{
				if (Function.ParseInt(list[num]["LinkEntry"]) == 0)
				{
					lstChild.Remove(list[num]);
					dt.Rows.Remove(list[num]);
					continue;
				}
				return;
			}
		}
		List<DataRow> lst = lstChild.Where((DataRow t) => Function.ToString(t["FatherCode"]).Equals(itemCode) && (Function.ParseInt(t["FatherLine"]) == -1 || Function.ParseInt(t["FatherLine"]) == line)).ToList();
		while (lst.Count > 0)
		{
			List<DataRow> list2 = lst.Where((DataRow t) => Function.ParseInt(t["ProId"]) == Function.ParseInt(lst[0]["ProId"])).ToList();
			if (!list2.Any())
			{
				continue;
			}
			string itemCode2 = Function.ToString(list2[0]["ItemCode"]);
			int line2 = Function.ParseInt(list2[0]["Line"]);
			int proId2 = Function.ParseInt(list2[0]["ProId"]);
			for (int num2 = 0; num2 < list2.Count; num2++)
			{
				if (Function.ParseInt(list2[num2]["LinkEntry"]) == 0)
				{
					lst.Remove(list2[num2]);
					continue;
				}
				return;
			}
			DeleteAppointment(ref dt, ref lstChild, itemCode2, line2, proId2);
		}
	}

	private HilightType CheckHilightType(int apointmentId)
	{
		try
		{
			if (apointmentId == _hilightApoinment.ApointmentId)
			{
				return HilightType.Current;
			}
			DataTable dataTable = ScheduleStorageGanttChart.Appointments.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				List<DataRow> list = (from t in dataTable.AsEnumerable()
					where Function.ToString(t["DemandType"]).Equals(_hilightApoinment.DemandType) && Function.ParseInt(t["DemandEntry"]) == _hilightApoinment.DemandEntry && Function.ParseInt(t["DemandLine"]) == _hilightApoinment.DemandLine
					select t).ToList();
				if (list.Count > 0)
				{
					DataRow dataRow = list.FirstOrDefault((DataRow t) => Function.ParseInt(t["Line"]) == apointmentId);
					if (dataRow != null)
					{
						string fatherCode = Function.ToString(dataRow["FatherCode"]);
						int fatherLine = Function.ParseInt(dataRow["FatherLine"]);
						string text = Function.ToString(dataRow["ItemCode"]);
						if (CheckChild(list, text, Function.ParseInt(dataRow["Line"]), fatherCode, fatherLine, Function.ParseInt(dataRow["OpeOrder"]), Function.ParseInt(dataRow["ProId"]), _hilightApoinment))
						{
							return HilightType.Child;
						}
						if (CheckChild(list, _hilightApoinment.ApontmentItemCode, _hilightApoinment.ApointmentId, _hilightApoinment.FatherCode, _hilightApoinment.FatherLine, _hilightApoinment.OpeOrder, _hilightApoinment.ProId, new HilightApoinment
						{
							ApointmentId = Function.ParseInt(dataRow["Line"]),
							ApontmentItemCode = text,
							DemandEntry = _hilightApoinment.DemandEntry,
							DemandType = _hilightApoinment.DemandType,
							DemandLine = _hilightApoinment.DemandLine,
							FatherCode = fatherCode,
							FatherLine = fatherLine,
							OpeOrder = Function.ParseInt(dataRow["OpeOrder"]),
							ProId = Function.ParseInt(dataRow["ProId"])
						}))
						{
							return HilightType.Father;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return HilightType.None;
	}

	public bool CheckChild(List<DataRow> lstSource, string itemCode, int itemLine, string fatherCode, int fatherLine, int opeOrder, int proId, HilightApoinment itemCheck)
	{
		if ((fatherCode.Equals(itemCheck.ApontmentItemCode) && (fatherLine == -1 || fatherLine == itemCheck.ApointmentId)) || (itemCode.Equals(itemCheck.ApontmentItemCode) && fatherCode.Equals(itemCheck.FatherCode) && fatherLine == itemCheck.FatherLine && proId == itemCheck.ProId && opeOrder < itemCheck.OpeOrder))
		{
			return true;
		}
		List<DataRow> source = lstSource.Where((DataRow t) => Function.ParseInt(t["ProId"]) == proId && Function.ParseInt(t["OpeOrder"]) > opeOrder).ToList();
		if (source.Any())
		{
			List<DataRow> list = source.OrderBy((DataRow t) => Function.ParseInt(t["OpeOrder"])).ToList();
			return CheckChild(lstSource, itemCode, Function.ParseInt(list[0]["Line"]), fatherCode, fatherLine, Function.ParseInt(list[0]["OpeOrder"]), Function.ParseInt(list[0]["ProId"]), itemCheck);
		}
		IEnumerable<DataRow> source2 = lstSource.Where((DataRow t) => Function.ToString(t["ItemCode"]).Equals(fatherCode) && (fatherLine == -1 || fatherLine == Function.ParseInt(t["Line"])));
		if (source2.Any())
		{
			List<DataRow> list2 = source2.OrderBy((DataRow t) => Function.ParseInt(t["OpeOrder"])).ToList();
			return CheckChild(lstSource, Function.ToString(list2[0]["ItemCode"]), Function.ParseInt(list2[0]["Line"]), Function.ToString(list2[0]["FatherCode"]), Function.ParseInt(list2[0]["FatherLine"]), Function.ParseInt(list2[0]["OpeOrder"]), Function.ParseInt(list2[0]["ProId"]), itemCheck);
		}
		return false;
	}

	private void UpdateStatusOverLoad(ref DataTable dt)
	{
		try
		{
			if (!dt.IsNotNull())
			{
				return;
			}
			List<DataRow> lstBase = (from t in dt.AsEnumerable()
				where Function.ToString(t["IsBase"]).Equals("Y")
				select t).ToList();
			if (!lstBase.Any())
			{
				return;
			}
			List<DataRow> source = (from t in dt.AsEnumerable()
				where Function.ToString(t["IsBase"]).Equals("N") && !Function.ToString(t["Complete"]).Equals("L")
				select t).ToList();
			if (source.Any())
			{
				for (int i = 0; i < lstBase.Count; i++)
				{
					DateTime startDate = Function.ParseDateTimes(lstBase[i]["StartDate"]);
					DateTime endDate = Function.ParseDateTimes(lstBase[i]["EndDate"]);
					List<DataRow> list = source.Where((DataRow t) => Function.ToString(t["ResCode"]).Equals(Function.ToString(lstBase[i]["ResCode"])) && ((startDate >= Function.ParseDateTimes(t["StartDate"]) && startDate <= Function.ParseDateTimes(t["EndDate"])) || (endDate <= Function.ParseDateTimes(t["EndDate"]) && endDate >= Function.ParseDateTimes(t["StartDate"])) || (startDate <= Function.ParseDateTimes(t["StartDate"]) && endDate >= Function.ParseDateTimes(t["EndDate"])))).ToList();
					bool flag = false;
					if (list.Count > 1)
					{
						int num = 0;
						while (num < list.Count)
						{
							DateTime checkStar = Function.ParseDateTimes(list[num]["StartDate"]);
							DateTime checkEnd = Function.ParseDateTimes(list[num]["EndDate"]);
							if (list.Count((DataRow t) => (checkStar >= Function.ParseDateTimes(t["StartDate"]) && checkStar <= Function.ParseDateTimes(t["EndDate"])) || (checkEnd <= Function.ParseDateTimes(t["EndDate"]) && checkEnd >= Function.ParseDateTimes(t["StartDate"])) || (checkStar <= Function.ParseDateTimes(t["StartDate"]) && checkEnd >= Function.ParseDateTimes(t["EndDate"]))) > 1)
							{
								flag = true;
								break;
							}
							list.RemoveAt(num);
						}
					}
					if (flag)
					{
						lstBase[i]["IsOverLoad"] = "Y";
						lstBase[i]["IdColor"] = "ApzOverLoad";
					}
					else
					{
						lstBase[i]["IsOverLoad"] = "N";
						lstBase[i]["IdColor"] = "ApzNormal";
					}
				}
			}
			else
			{
				for (int num2 = 0; num2 < lstBase.Count; num2++)
				{
					lstBase[num2]["IsOverLoad"] = "N";
					lstBase[num2]["IdColor"] = "ApzNormal";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private List<DateTime> GetForwardUpdateDate(DataTable dt, string resCode, DateTime startDate, double timeRun)
	{
		List<DateTime> list = new List<DateTime>();
		try
		{
			List<DataRow> source = (from t in dt.AsEnumerable()
				where Function.ToString(t["ResCode"]).Equals(resCode) && Function.ToString(t["IsBase"]).Equals("Y") && Function.ParseDateTimes(t["EndDate"]) > startDate
				select t).ToList();
			if (source.Any())
			{
				source = source.OrderBy((DataRow t) => Function.ParseDateTimes(t["StartDate"])).ToList();
				if (startDate < Function.ParseDateTimes(source[0]["StartDate"]))
				{
					startDate = Function.ParseDateTimes(source[0]["StartDate"]);
				}
				list.Add(startDate);
				for (int num = 0; num < source.Count; num++)
				{
					DateTime dateTime = ((num == 0) ? startDate : Function.ParseDateTimes(source[num]["StartDate"]));
					if (dateTime.AddMinutes(timeRun) <= Function.ParseDateTimes(source[num]["EndDate"]) || num == source.Count - 1)
					{
						if (num == source.Count - 1)
						{
							list.Add(Function.ParseDateTimes(source[num]["EndDate"]));
						}
						else
						{
							list.Add(dateTime.AddMinutes(timeRun));
						}
						break;
					}
					timeRun -= (Function.ParseDateTimes(source[num]["EndDate"]) - dateTime).TotalMinutes;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return list;
	}

	private void ProcessData(ref DataTable dtAppointments)
	{
		List<DataRow> list = (from t in dtAppointments.AsEnumerable()
			where !string.IsNullOrEmpty(Function.ToString(t["DemandEntry"])) && !Function.ToString(t["DemandEntry"]).Equals("ZZ") && string.IsNullOrEmpty(Function.ToString(t["FatherCode"])) && Function.ToString(t["LineType"]).Equals("C")
			select t).ToList();
		int num = 0;
		while (num < list.Count)
		{
			string demandType = Function.ToString(list[num]["DemandType"]);
			int demandEntry = Function.ParseInt(list[num]["DemandEntry"]);
			int demandLine = Function.ParseInt(list[num]["DemandLine"]);
			List<DataRow> list2 = (from t in list
				where Function.ToString(t["DemandType"]).Equals(demandType) && Function.ParseInt(t["DemandEntry"]) == demandEntry && Function.ParseInt(t["DemandLine"]) == demandLine
				orderby Function.ParseInt(t["OpeOrder"]) descending
				select t).ToList();
			List<DataRow> lstSource = (from t in dtAppointments.AsEnumerable()
				where Function.ToString(t["DemandType"]).Equals(demandType) && Function.ParseInt(t["DemandEntry"]) == demandEntry && Function.ParseInt(t["DemandLine"]) == demandLine
				select t).ToList();
			UpdateTimeDataApointmnetBackward(list2, dtAppointments, lstSource);
			for (int num2 = 0; num2 < list2.Count; num2++)
			{
				list.Remove(list2[num2]);
			}
		}
		dtAppointments.AcceptChanges();
		UpdateStatusOverLoad(ref dtAppointments);
	}

	private void UpdateTimeDataApointmnetBackward(List<DataRow> lstSaveLevel, DataTable dtAppointments, List<DataRow> lstSource)
	{
		DateTime endDate = Function.ParseDateTimes(lstSaveLevel[0]["EndDate"]);
		DateTime dateTime = Function.ParseDateTimes(lstSaveLevel[0]["StartDate"]);
		for (int i = 1; i < lstSaveLevel.Count; i++)
		{
			if (Function.ParseInt(lstSaveLevel[i]["OpeOrder"]) < Function.ParseInt(lstSaveLevel[i - 1]["OpeOrder"]))
			{
				endDate = dateTime;
			}
			List<DateTime> backwardUpdateDate = GetBackwardUpdateDate(dtAppointments, Function.ToString(lstSaveLevel[i]["ResCode"]), endDate, Function.ParseDouble(lstSaveLevel[i]["TimeRun"]));
			if (backwardUpdateDate != null && backwardUpdateDate.Count == 2)
			{
				lstSaveLevel[i]["StartDate"] = backwardUpdateDate[0];
				lstSaveLevel[i]["WorkDay"] = backwardUpdateDate[0].Date;
				lstSaveLevel[i]["StartTime"] = backwardUpdateDate[0].Hour * 100 + backwardUpdateDate[0].Minute;
				lstSaveLevel[i]["EndDate"] = backwardUpdateDate[1];
				lstSaveLevel[i]["EndWorkDay"] = backwardUpdateDate[1].Date;
				lstSaveLevel[i]["EndTime"] = backwardUpdateDate[1].Hour * 100 + backwardUpdateDate[1].Minute;
			}
			if (Function.ParseInt(lstSaveLevel[i]["OpeOrder"]) == Function.ParseInt(lstSaveLevel[i - 1]["OpeOrder"]))
			{
				if (dateTime > Function.ParseDateTimes(lstSaveLevel[i]["StartDate"]))
				{
					dateTime = Function.ParseDateTimes(lstSaveLevel[i]["StartDate"]);
				}
			}
			else
			{
				dateTime = Function.ParseDateTimes(lstSaveLevel[i]["StartDate"]);
			}
		}
		int index = lstSaveLevel.Count - 1;
		SetTimeByFatherTimeBackward(ref lstSource, dtAppointments, Function.ToString(lstSaveLevel[index]["ItemCode"]), dateTime, Function.ParseInt(lstSaveLevel[index]["Line"]));
	}

	private void UpdateTimeDataApointmnetForward(List<DataRow> lstSaveLevel, DataTable dtAppointments, List<DataRow> lstSource)
	{
		DateTime endDate = Function.ParseDateTimes(lstSaveLevel[0]["StartDate"]);
		DateTime dateTime = Function.ParseDateTimes(lstSaveLevel[0]["EndDate"]);
		for (int i = 1; i < lstSaveLevel.Count; i++)
		{
			if (Function.ParseInt(lstSaveLevel[i]["OpeOrder"]) > Function.ParseInt(lstSaveLevel[i - 1]["OpeOrder"]))
			{
				endDate = dateTime;
			}
			List<DateTime> backwardUpdateDate = GetBackwardUpdateDate(dtAppointments, Function.ToString(lstSaveLevel[i]["ResCode"]), endDate, Function.ParseDouble(lstSaveLevel[i]["TimeRun"]));
			if (backwardUpdateDate != null && backwardUpdateDate.Count == 2)
			{
				lstSaveLevel[i]["StartDate"] = backwardUpdateDate[0];
				lstSaveLevel[i]["WorkDay"] = backwardUpdateDate[0].Date;
				lstSaveLevel[i]["StartTime"] = backwardUpdateDate[0].Hour * 100 + backwardUpdateDate[0].Minute;
				lstSaveLevel[i]["EndDate"] = backwardUpdateDate[1];
				lstSaveLevel[i]["EndWorkDay"] = backwardUpdateDate[1].Date;
				lstSaveLevel[i]["EndTime"] = backwardUpdateDate[1].Hour * 100 + backwardUpdateDate[1].Minute;
			}
			if (Function.ParseInt(lstSaveLevel[i]["OpeOrder"]) == Function.ParseInt(lstSaveLevel[i - 1]["OpeOrder"]))
			{
				if (dateTime < Function.ParseDateTimes(lstSaveLevel[i]["EndDate"]))
				{
					dateTime = Function.ParseDateTimes(lstSaveLevel[i]["EndDate"]);
				}
			}
			else
			{
				dateTime = Function.ParseDateTimes(lstSaveLevel[i]["EndDate"]);
			}
		}
		int index = lstSaveLevel.Count - 1;
		SetTimeByFatherTimeForward(ref lstSource, dtAppointments, Function.ToString(lstSaveLevel[index]["FatherCode"]), dateTime, Function.ParseInt(lstSaveLevel[index]["FatherLine"]));
	}

	private void SetTimeByFatherTimeForward(ref List<DataRow> lstSource, DataTable dtAppointments, string fatherItemCode, DateTime childEndDate, int fatherLine)
	{
		try
		{
			if (lstSource == null || !lstSource.Any())
			{
				return;
			}
			DataRow fatherRow = lstSource.FirstOrDefault((DataRow t) => Function.ToString(t["ItemCode"]).Equals(fatherItemCode) && (fatherLine == -1 || Function.ParseInt(t["Line"]) == fatherLine));
			if (fatherRow == null)
			{
				return;
			}
			List<DataRow> list = (from t in lstSource
				where Function.ToString(t["ItemCode"]).Equals(fatherItemCode) && Function.ParseInt(t["ProId"]) == Function.ParseInt(fatherRow["ProId"])
				orderby Function.ParseInt(t["OpeOrder"])
				select t).ToList();
			DateTime dateTime = childEndDate;
			DateTime startDate = dateTime;
			for (int num = 0; num < list.Count && Function.ToString(list[num]["Complete"]).Equals("N"); num++)
			{
				if (num > 0 && Function.ParseInt(list[num - 1]["OpeOrder"]) < Function.ParseInt(list[num]["OpeOrder"]))
				{
					startDate = dateTime;
				}
				List<DateTime> forwardUpdateDate = GetForwardUpdateDate(dtAppointments, Function.ToString(list[num]["ResCode"]), startDate, Function.ParseDouble(list[num]["TimeRun"]));
				if (forwardUpdateDate != null && forwardUpdateDate.Count == 2)
				{
					list[num]["StartDate"] = forwardUpdateDate[0];
					list[num]["WorkDay"] = forwardUpdateDate[0].Date;
					list[num]["StartTime"] = forwardUpdateDate[0].Hour * 100 + forwardUpdateDate[0].Minute;
					list[num]["EndDate"] = forwardUpdateDate[1];
					list[num]["EndWorkDay"] = forwardUpdateDate[1].Date;
					list[num]["EndTime"] = forwardUpdateDate[1].Hour * 100 + forwardUpdateDate[1].Minute;
				}
				if (dateTime < Function.ParseDateTimes(list[num]["EndDate"]))
				{
					dateTime = Function.ParseDateTimes(list[num]["EndDate"]);
				}
			}
			int index = list.Count - 1;
			SetTimeByFatherTimeForward(ref lstSource, dtAppointments, Function.ToString(list[index]["FatherCode"]), dateTime, Function.ParseInt(list[index]["FatherLine"]));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DateTime SetTimeByFatherTimeBackward(ref List<DataRow> lstSource, DataTable dtAppointments, string fatherItemCode, DateTime fatherStartDate, int fatherLine)
	{
		try
		{
			if (lstSource == null || !lstSource.Any())
			{
				return fatherStartDate;
			}
			IEnumerable<DataRow> source = lstSource.Where((DataRow t) => Function.ToString(t["FatherCode"]).Equals(fatherItemCode) && (Function.ParseInt(t["FatherLine"]) == -1 || Function.ParseInt(t["FatherLine"]) == fatherLine));
			if (source.Any())
			{
				List<int> lstOrder = (from t in source.Select((DataRow t) => Function.ParseInt(t["FatherOpeOrder"])).Distinct()
					orderby t descending
					select t).ToList();
				DateTime dateTime = fatherStartDate;
				int i;
				for (i = 0; i < lstOrder.Count; i++)
				{
					List<DataRow> source2 = source.Where((DataRow t) => Function.ParseInt(t["FatherOpeOrder"]) == lstOrder[i]).ToList();
					List<string> lstItem = source2.Select((DataRow t) => Function.ToString(t["ItemCode"])).Distinct().ToList();
					DateTime dateTime2 = dateTime;
					int j;
					for (j = 0; j < lstItem.Count; j++)
					{
						DateTime endDate = dateTime;
						List<DataRow> list = (from t in source2
							where Function.ToString(t["ItemCode"]).Equals(lstItem[j])
							orderby Function.ParseInt(t["OpeOrder"]) descending
							select t).ToList();
						for (int num = 0; num < list.Count && Function.ToString(list[num]["Complete"]).Equals("N"); num++)
						{
							if (num > 0 && Function.ParseInt(list[num - 1]["OpeOrder"]) > Function.ParseInt(list[num]["OpeOrder"]))
							{
								endDate = Function.ParseDateTimes(list[num - 1]["StartDate"]);
							}
							List<DateTime> backwardUpdateDate = GetBackwardUpdateDate(dtAppointments, Function.ToString(list[num]["ResCode"]), endDate, Function.ParseDouble(list[num]["TimeRun"]));
							if (backwardUpdateDate != null && backwardUpdateDate.Count == 2)
							{
								list[num]["StartDate"] = backwardUpdateDate[0];
								list[num]["WorkDay"] = backwardUpdateDate[0].Date;
								list[num]["StartTime"] = backwardUpdateDate[0].Hour * 100 + backwardUpdateDate[0].Minute;
								list[num]["EndDate"] = backwardUpdateDate[1];
								list[num]["EndWorkDay"] = backwardUpdateDate[1].Date;
								list[num]["EndTime"] = backwardUpdateDate[1].Hour * 100 + backwardUpdateDate[1].Minute;
							}
							if (dateTime2 > Function.ParseDateTimes(list[num]["StartDate"]))
							{
								dateTime2 = Function.ParseDateTimes(list[num]["StartDate"]);
							}
						}
						int index = list.Count - 1;
						endDate = SetTimeByFatherTimeBackward(ref lstSource, dtAppointments, Function.ToString(list[index]["ItemCode"]), Function.ParseDateTimes(list[index]["StartDate"]), Function.ParseInt(list[index]["Line"]));
						if (dateTime2 > endDate)
						{
							dateTime2 = endDate;
						}
					}
					dateTime = dateTime2;
				}
				return dateTime;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return fatherStartDate;
	}

	private List<DateTime> GetBackwardUpdateDate(DataTable dtAppointments, string resCode, DateTime endDate, double timeRun)
	{
		List<DateTime> list = new List<DateTime>();
		try
		{
			List<DataRow> source = (from t in dtAppointments.AsEnumerable()
				where Function.ToString(t["ResCode"]).Equals(resCode) && Function.ToString(t["IsBase"]).Equals("Y") && Function.ParseDateTimes(t["StartDate"]) < endDate && Function.ParseDateTimes(t["StartDate"]) >= DateTime.Now.Date
				select t).ToList();
			if (source.Any())
			{
				source = source.OrderByDescending((DataRow t) => Function.ParseDateTimes(t["StartDate"])).ToList();
				if (endDate > Function.ParseDateTimes(source[0]["EndDate"]))
				{
					endDate = Function.ParseDateTimes(source[0]["EndDate"]);
				}
				for (int num = 0; num < source.Count; num++)
				{
					DateTime dateTime = ((num == 0) ? endDate : Function.ParseDateTimes(source[num]["EndDate"]));
					if (dateTime.AddMinutes(-1.0 * timeRun) >= Function.ParseDateTimes(source[num]["StartDate"]))
					{
						list.Add(dateTime.AddMinutes(-1.0 * timeRun));
						list.Add(endDate);
						return list;
					}
					timeRun -= (dateTime - Function.ParseDateTimes(source[num]["StartDate"])).TotalMinutes;
				}
			}
			List<DataRow> source2 = (from t in dtAppointments.AsEnumerable()
				where Function.ToString(t["ResCode"]).Equals(resCode) && Function.ToString(t["IsBase"]).Equals("Y") && Function.ParseDateTimes(t["StartDate"]) >= DateTime.Now.Date
				select t).ToList();
			if (source2.Any())
			{
				source2 = source2.OrderBy((DataRow t) => Function.ParseDateTimes(t["StartDate"])).ToList();
				DateTime item = Function.ParseDateTimes(source2[0]["StartDate"]);
				for (int num2 = 0; num2 < source2.Count; num2++)
				{
					if (timeRun <= Function.ParseDouble(source2[num2]["TotalTimeShift"]))
					{
						list.Add(item);
						list.Add(Function.ParseDateTimes(source2[num2]["StartDate"]).AddMinutes(timeRun));
						break;
					}
					timeRun -= Function.ParseDouble(source2[num2]["TotalTimeShift"]);
					if (num2 == source2.Count - 1)
					{
						list.Add(item);
						list.Add(Function.ParseDateTimes(source2[num2]["EndDate"]));
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return list;
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
		DevExpress.XtraScheduler.TimeScaleYear timeScaleYear = new DevExpress.XtraScheduler.TimeScaleYear();
		DevExpress.XtraScheduler.TimeScaleQuarter timeScaleQuarter = new DevExpress.XtraScheduler.TimeScaleQuarter();
		DevExpress.XtraScheduler.TimeScaleMonth timeScaleMonth = new DevExpress.XtraScheduler.TimeScaleMonth();
		DevExpress.XtraScheduler.TimeScaleWeek value3 = new DevExpress.XtraScheduler.TimeScaleWeek();
		DevExpress.XtraScheduler.TimeScaleDay value4 = new DevExpress.XtraScheduler.TimeScaleDay();
		DevExpress.XtraScheduler.TimeScaleHour value5 = new DevExpress.XtraScheduler.TimeScaleHour();
		DevExpress.XtraScheduler.TimeScale15Minutes timeScale15Minutes = new DevExpress.XtraScheduler.TimeScale15Minutes();
		DevExpress.XtraScheduler.TimeScaleYear timeScaleYear2 = new DevExpress.XtraScheduler.TimeScaleYear();
		DevExpress.XtraScheduler.TimeScaleQuarter timeScaleQuarter2 = new DevExpress.XtraScheduler.TimeScaleQuarter();
		DevExpress.XtraScheduler.TimeScaleMonth timeScaleMonth2 = new DevExpress.XtraScheduler.TimeScaleMonth();
		DevExpress.XtraScheduler.TimeScaleWeek value6 = new DevExpress.XtraScheduler.TimeScaleWeek();
		DevExpress.XtraScheduler.TimeScaleDay value7 = new DevExpress.XtraScheduler.TimeScaleDay();
		DevExpress.XtraScheduler.TimeScaleHour timeScaleHour = new DevExpress.XtraScheduler.TimeScaleHour();
		DevExpress.XtraScheduler.TimeScale15Minutes timeScale15Minutes2 = new DevExpress.XtraScheduler.TimeScale15Minutes();
		DevExpress.XtraScheduler.TimeRuler value8 = new DevExpress.XtraScheduler.TimeRuler();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.RoutingAndResource.frm_CapacityGantt));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		this.ScheduleGanttChart = new DevExpress.XtraScheduler.SchedulerControl();
		this.ScheduleStorageGanttChart = new DevExpress.XtraScheduler.SchedulerStorage(this.components);
		this.toolTipController1 = new DevExpress.Utils.ToolTipController(this.components);
		this.SplitReource = new DevExpress.XtraEditors.SplitContainerControl();
		this.ResourceTree = new DevExpress.XtraScheduler.UI.ResourcesTree();
		this.treeColumnId = new DevExpress.XtraScheduler.Native.ResourceTreeColumn();
		this.treeColumnCaption = new DevExpress.XtraScheduler.Native.ResourceTreeColumn();
		this.treeColumnType = new DevExpress.XtraScheduler.Native.ResourceTreeColumn();
		this.rResType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.SplitApointment = new DevExpress.XtraEditors.SplitContainerControl();
		this.btnViewDetail = new DevExpress.XtraEditors.SimpleButton();
		this.SplitDetail = new DevExpress.XtraEditors.SplitContainerControl();
		this.grdOpeColor = new Apzon.Controls.Grid.GridControlEx();
		this.gvOpeColor = new Apzon.Controls.Grid.GridViewEx();
		this.gcOpeName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOpeColor = new Apzon.Controls.Grid.GridColumnEx();
		this.rColorEdit = new DevExpress.XtraEditors.Repository.RepositoryItemColorEdit();
		this.grdStatus = new Apzon.Controls.Grid.GridControlEx();
		this.gvStatus = new Apzon.Controls.Grid.GridViewEx();
		this.gcStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.gcStatusColor = new Apzon.Controls.Grid.GridColumnEx();
		this.rStatusColor = new DevExpress.XtraEditors.Repository.RepositoryItemColorEdit();
		this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
		this.btnSave = new DevExpress.XtraEditors.SimpleButton();
		this.btnRun = new DevExpress.XtraEditors.SimpleButton();
		this.lbScheCode = new Apzon.Controls.Controls.LabelLine();
		this.txtScheCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.lbCapacityCode = new Apzon.Controls.Controls.LabelLine();
		this.txtCapacityCode = new Apzon.Controls.Editor.ButtonEditEx();
		((System.ComponentModel.ISupportInitialize)this.ScheduleGanttChart).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ScheduleStorageGanttChart).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SplitReource).BeginInit();
		this.SplitReource.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.ResourceTree).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rResType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SplitApointment).BeginInit();
		this.SplitApointment.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.SplitDetail).BeginInit();
		this.SplitDetail.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdOpeColor).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvOpeColor).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rColorEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rStatusColor).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtScheCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCapacityCode.Properties).BeginInit();
		base.SuspendLayout();
		this.ScheduleGanttChart.ActiveViewType = DevExpress.XtraScheduler.SchedulerViewType.Gantt;
		this.ScheduleGanttChart.DataStorage = this.ScheduleStorageGanttChart;
		this.ScheduleGanttChart.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ScheduleGanttChart.GroupType = DevExpress.XtraScheduler.SchedulerGroupType.Resource;
		this.ScheduleGanttChart.Location = new System.Drawing.Point(0, 0);
		this.ScheduleGanttChart.Name = "ScheduleGanttChart";
		this.ScheduleGanttChart.OptionsCustomization.AllowAppointmentMultiSelect = false;
		this.ScheduleGanttChart.OptionsCustomization.AllowAppointmentResize = DevExpress.XtraScheduler.UsedAppointmentType.None;
		this.ScheduleGanttChart.OptionsCustomization.AllowDisplayAppointmentFlyout = false;
		this.ScheduleGanttChart.OptionsDragDrop.KeepRelativeResourceDistance = true;
		this.ScheduleGanttChart.OptionsView.ToolTipVisibility = DevExpress.XtraScheduler.ToolTipVisibility.Always;
		this.ScheduleGanttChart.Size = new System.Drawing.Size(1346, 595);
		this.ScheduleGanttChart.Start = new System.DateTime(2018, 8, 12, 0, 0, 0, 0);
		this.ScheduleGanttChart.TabIndex = 0;
		this.ScheduleGanttChart.Text = "schedulerControl1";
		this.ScheduleGanttChart.ToolTipController = this.toolTipController1;
		this.ScheduleGanttChart.Views.DayView.DayCount = 7;
		this.ScheduleGanttChart.Views.DayView.GroupSeparatorWidth = 2;
		this.ScheduleGanttChart.Views.DayView.ShowMoreButtons = false;
		this.ScheduleGanttChart.Views.DayView.TimeRulers.Add(value);
		this.ScheduleGanttChart.Views.FullWeekView.Enabled = true;
		this.ScheduleGanttChart.Views.FullWeekView.GroupSeparatorWidth = 2;
		this.ScheduleGanttChart.Views.FullWeekView.ShowMoreButtons = false;
		this.ScheduleGanttChart.Views.FullWeekView.TimeRulers.Add(value2);
		this.ScheduleGanttChart.Views.GanttView.CellsAutoHeightOptions.Enabled = true;
		this.ScheduleGanttChart.Views.GanttView.CellsAutoHeightOptions.MinHeight = 20;
		this.ScheduleGanttChart.Views.GanttView.GroupSeparatorWidth = 2;
		timeScaleYear.Enabled = false;
		timeScaleQuarter.Enabled = false;
		timeScaleMonth.Enabled = false;
		timeScale15Minutes.Enabled = false;
		this.ScheduleGanttChart.Views.GanttView.Scales.Add(timeScaleYear);
		this.ScheduleGanttChart.Views.GanttView.Scales.Add(timeScaleQuarter);
		this.ScheduleGanttChart.Views.GanttView.Scales.Add(timeScaleMonth);
		this.ScheduleGanttChart.Views.GanttView.Scales.Add(value3);
		this.ScheduleGanttChart.Views.GanttView.Scales.Add(value4);
		this.ScheduleGanttChart.Views.GanttView.Scales.Add(value5);
		this.ScheduleGanttChart.Views.GanttView.Scales.Add(timeScale15Minutes);
		this.ScheduleGanttChart.Views.GanttView.ShowMoreButtons = false;
		this.ScheduleGanttChart.Views.MonthView.GroupSeparatorWidth = 2;
		this.ScheduleGanttChart.Views.MonthView.ShowMoreButtons = false;
		this.ScheduleGanttChart.Views.TimelineView.Appearance.Appointment.Options.UseTextOptions = true;
		this.ScheduleGanttChart.Views.TimelineView.Appearance.Appointment.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.ScheduleGanttChart.Views.TimelineView.Appearance.Appointment.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.ScheduleGanttChart.Views.TimelineView.AppointmentDisplayOptions.AppointmentHeight = 20;
		this.ScheduleGanttChart.Views.TimelineView.CellsAutoHeightOptions.MinHeight = 20;
		this.ScheduleGanttChart.Views.TimelineView.GroupSeparatorWidth = 2;
		timeScaleYear2.Enabled = false;
		timeScaleYear2.Visible = false;
		timeScaleQuarter2.Enabled = false;
		timeScaleQuarter2.Visible = false;
		timeScaleMonth2.Enabled = false;
		timeScaleHour.Visible = false;
		timeScaleHour.Width = 10;
		timeScale15Minutes2.Visible = false;
		timeScale15Minutes2.Width = 5;
		this.ScheduleGanttChart.Views.TimelineView.Scales.Add(timeScaleYear2);
		this.ScheduleGanttChart.Views.TimelineView.Scales.Add(timeScaleQuarter2);
		this.ScheduleGanttChart.Views.TimelineView.Scales.Add(timeScaleMonth2);
		this.ScheduleGanttChart.Views.TimelineView.Scales.Add(value6);
		this.ScheduleGanttChart.Views.TimelineView.Scales.Add(value7);
		this.ScheduleGanttChart.Views.TimelineView.Scales.Add(timeScaleHour);
		this.ScheduleGanttChart.Views.TimelineView.Scales.Add(timeScale15Minutes2);
		this.ScheduleGanttChart.Views.TimelineView.ShowMoreButtons = false;
		this.ScheduleGanttChart.Views.TimelineView.TimelineScrollBarVisible = true;
		this.ScheduleGanttChart.Views.WeekView.Enabled = false;
		this.ScheduleGanttChart.Views.WeekView.GroupSeparatorWidth = 2;
		this.ScheduleGanttChart.Views.WeekView.ShowMoreButtons = false;
		this.ScheduleGanttChart.Views.WorkWeekView.GroupSeparatorWidth = 2;
		this.ScheduleGanttChart.Views.WorkWeekView.ShowMoreButtons = false;
		this.ScheduleGanttChart.Views.WorkWeekView.TimeRulers.Add(value8);
		this.ScheduleGanttChart.AppointmentViewInfoCustomizing += new DevExpress.XtraScheduler.AppointmentViewInfoCustomizingEventHandler(this.ScheduleGanttChart_AppointmentViewInfoCustomizing);
		this.ScheduleGanttChart.AppointmentDrag += new DevExpress.XtraScheduler.AppointmentDragEventHandler(this.ScheduleGanttChart_AppointmentDrag);
		this.ScheduleGanttChart.AdditionalAppointmentsDrag += new System.EventHandler<DevExpress.XtraScheduler.AdditionalAppointmentsDragEventArgs>(this.ScheduleGanttChart_AdditionalAppointmentsDrag);
		this.ScheduleGanttChart.PopupMenuShowing += new DevExpress.XtraScheduler.PopupMenuShowingEventHandler(this.ScheduleGanttChart_PopupMenuShowing);
		this.ScheduleGanttChart.Click += new System.EventHandler(this.ScheduleGanttChart_Click);
		this.ScheduleGanttChart.DragDrop += new System.Windows.Forms.DragEventHandler(this.ScheduleGanttChart_DragDrop);
		this.ScheduleGanttChart.DoubleClick += new System.EventHandler(this.ScheduleGanttChart_DoubleClick);
		this.ScheduleStorageGanttChart.AppointmentsChanged += new DevExpress.XtraScheduler.PersistentObjectsEventHandler(this.ScheduleStorageGanttChart_AppointmentsChanged);
		this.ScheduleStorageGanttChart.AppointmentDeleting += new DevExpress.XtraScheduler.PersistentObjectCancelEventHandler(this.ScheduleStorageGanttChart_AppointmentDeleting);
		this.toolTipController1.AutoPopDelay = 50000;
		this.toolTipController1.CloseOnClick = DevExpress.Utils.DefaultBoolean.False;
		this.toolTipController1.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.toolTipController1.BeforeShow += new DevExpress.Utils.ToolTipControllerBeforeShowEventHandler(this.toolTipController1_BeforeShow);
		this.SplitReource.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.SplitReource.Location = new System.Drawing.Point(2, 32);
		this.SplitReource.Name = "SplitReource";
		this.SplitReource.Panel1.Controls.Add(this.ResourceTree);
		this.SplitReource.Panel1.Text = "Panel1";
		this.SplitReource.Panel2.Controls.Add(this.SplitApointment);
		this.SplitReource.Panel2.Text = "Panel2";
		this.SplitReource.Size = new System.Drawing.Size(1588, 595);
		this.SplitReource.SplitterPosition = 237;
		this.SplitReource.TabIndex = 1;
		this.SplitReource.Text = "splitContainerControl1";
		this.ResourceTree.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[3] { this.treeColumnId, this.treeColumnCaption, this.treeColumnType });
		this.ResourceTree.Cursor = System.Windows.Forms.Cursors.Default;
		this.ResourceTree.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ResourceTree.KeyFieldName = "ResourceId";
		this.ResourceTree.Location = new System.Drawing.Point(0, 0);
		this.ResourceTree.Name = "ResourceTree";
		this.ResourceTree.OptionsBehavior.Editable = false;
		this.ResourceTree.OptionsView.AutoWidth = false;
		this.ResourceTree.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.False;
		this.ResourceTree.ParentFieldName = "ParentId";
		this.ResourceTree.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rResType });
		this.ResourceTree.SchedulerControl = this.ScheduleGanttChart;
		this.ResourceTree.Size = new System.Drawing.Size(237, 595);
		this.ResourceTree.TabIndex = 0;
		this.ResourceTree.VertScrollVisibility = DevExpress.XtraTreeList.ScrollVisibility.Never;
		this.treeColumnId.Caption = "Resource No";
		this.treeColumnId.FieldName = "ResCode";
		this.treeColumnId.Name = "treeColumnId";
		this.treeColumnId.Visible = true;
		this.treeColumnId.VisibleIndex = 0;
		this.treeColumnId.Width = 78;
		this.treeColumnCaption.Caption = "Resource name";
		this.treeColumnCaption.FieldName = "ResName";
		this.treeColumnCaption.Name = "treeColumnCaption";
		this.treeColumnCaption.Visible = true;
		this.treeColumnCaption.VisibleIndex = 1;
		this.treeColumnCaption.Width = 90;
		this.treeColumnType.Caption = "Resource Type";
		this.treeColumnType.ColumnEdit = this.rResType;
		this.treeColumnType.FieldName = "ResType";
		this.treeColumnType.Name = "treeColumnType";
		this.treeColumnType.Visible = true;
		this.treeColumnType.VisibleIndex = 2;
		this.treeColumnType.Width = 114;
		this.rResType.AutoHeight = false;
		this.rResType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rResType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code")
		});
		this.rResType.DisplayMember = "Name";
		this.rResType.Name = "rResType";
		this.rResType.NullText = "";
		this.rResType.ValueMember = "Code";
		this.SplitApointment.Dock = System.Windows.Forms.DockStyle.Fill;
		this.SplitApointment.Location = new System.Drawing.Point(0, 0);
		this.SplitApointment.Name = "SplitApointment";
		this.SplitApointment.Panel1.Controls.Add(this.ScheduleGanttChart);
		this.SplitApointment.Panel1.Text = "Panel1";
		this.SplitApointment.Panel2.Controls.Add(this.btnViewDetail);
		this.SplitApointment.Panel2.Controls.Add(this.SplitDetail);
		this.SplitApointment.Panel2.Text = "Panel2";
		this.SplitApointment.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel1;
		this.SplitApointment.Size = new System.Drawing.Size(1346, 595);
		this.SplitApointment.SplitterPosition = 1226;
		this.SplitApointment.TabIndex = 1;
		this.SplitApointment.Text = "splitContainerControl2";
		this.btnViewDetail.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnViewDetail.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnViewDetail.ImageOptions.Image");
		this.btnViewDetail.Location = new System.Drawing.Point(-29, 3);
		this.btnViewDetail.Name = "btnViewDetail";
		this.btnViewDetail.Size = new System.Drawing.Size(26, 23);
		this.btnViewDetail.TabIndex = 1;
		this.btnViewDetail.Click += new System.EventHandler(this.btnViewDetail_Click);
		this.SplitDetail.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.SplitDetail.Horizontal = false;
		this.SplitDetail.Location = new System.Drawing.Point(0, 27);
		this.SplitDetail.Name = "SplitDetail";
		this.SplitDetail.Panel1.Controls.Add(this.grdOpeColor);
		this.SplitDetail.Panel1.Text = "Panel1";
		this.SplitDetail.Panel2.Controls.Add(this.grdStatus);
		this.SplitDetail.Panel2.Text = "Panel2";
		this.SplitDetail.Size = new System.Drawing.Size(0, 0);
		this.SplitDetail.SplitterPosition = 0;
		this.SplitDetail.TabIndex = 0;
		this.SplitDetail.Text = "splitContainerControl3";
		this.grdOpeColor.AllowPasteExcel = false;
		this.grdOpeColor.ClipboardData = null;
		this.grdOpeColor.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdOpeColor.Location = new System.Drawing.Point(0, 0);
		this.grdOpeColor.MainView = this.gvOpeColor;
		this.grdOpeColor.Name = "grdOpeColor";
		this.grdOpeColor.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rColorEdit });
		this.grdOpeColor.Size = new System.Drawing.Size(0, 0);
		this.grdOpeColor.TabIndex = 0;
		this.grdOpeColor.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvOpeColor });
		this.gvOpeColor.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gvOpeColor.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gvOpeColor.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvOpeColor.ColumnKey = null;
		this.gvOpeColor.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gcOpeName, this.gcOpeColor });
		this.gvOpeColor.DisableFormSetting = false;
		this.gvOpeColor.DisplayException = false;
		this.gvOpeColor.EditColumn = null;
		this.gvOpeColor.FieldCurrency = null;
		this.gvOpeColor.FieldRate = null;
		this.gvOpeColor.FormId = "oCapacityGantt";
		this.gvOpeColor.GridControl = this.grdOpeColor;
		this.gvOpeColor.IndexEditRow = -1;
		this.gvOpeColor.IndicatorWidth = 20;
		this.gvOpeColor.IsApzTable = false;
		this.gvOpeColor.IsInsertNewRow = false;
		this.gvOpeColor.IsLoadingData = false;
		this.gvOpeColor.ItemId = "OOPECOLOR";
		this.gvOpeColor.LstColumnAllowBatch = null;
		this.gvOpeColor.LstColumnNotAllowBatch = null;
		this.gvOpeColor.LstColumNotSetting = null;
		this.gvOpeColor.Name = "gvOpeColor";
		this.gvOpeColor.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gvOpeColor.OptionsPrint.AutoWidth = false;
		this.gvOpeColor.OptionsView.BestFitMaxRowCount = 100;
		this.gvOpeColor.OptionsView.ColumnAutoWidth = false;
		this.gvOpeColor.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvOpeColor.OptionsView.ShowGroupPanel = false;
		this.gvOpeColor.UseBatchMod = false;
		this.gcOpeName.Caption = "Operation Name";
		this.gcOpeName.CurrencyDateField = null;
		this.gcOpeName.CurrencyField = null;
		this.gcOpeName.CurrencyFixed = null;
		this.gcOpeName.CurrencyFixedRate = 0.0;
		this.gcOpeName.CurrencyRateField = null;
		this.gcOpeName.FieldName = "ActionName";
		this.gcOpeName.Name = "gcOpeName";
		this.gcOpeName.PrimaryKey = false;
		this.gcOpeName.UnitField = null;
		this.gcOpeName.Visible = true;
		this.gcOpeName.VisibleIndex = 0;
		this.gcOpeName.Width = 62;
		this.gcOpeColor.Caption = "Color";
		this.gcOpeColor.ColumnEdit = this.rColorEdit;
		this.gcOpeColor.CurrencyDateField = null;
		this.gcOpeColor.CurrencyField = null;
		this.gcOpeColor.CurrencyFixed = null;
		this.gcOpeColor.CurrencyFixedRate = 0.0;
		this.gcOpeColor.CurrencyRateField = null;
		this.gcOpeColor.FieldName = "Color";
		this.gcOpeColor.Name = "gcOpeColor";
		this.gcOpeColor.PrimaryKey = false;
		this.gcOpeColor.UnitField = null;
		this.gcOpeColor.Visible = true;
		this.gcOpeColor.VisibleIndex = 1;
		this.rColorEdit.AutoHeight = false;
		this.rColorEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rColorEdit.Name = "rColorEdit";
		this.rColorEdit.StoreColorAsInteger = true;
		this.grdStatus.AllowPasteExcel = false;
		this.grdStatus.ClipboardData = null;
		this.grdStatus.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdStatus.Location = new System.Drawing.Point(0, 0);
		this.grdStatus.MainView = this.gvStatus;
		this.grdStatus.Name = "grdStatus";
		this.grdStatus.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rStatusColor });
		this.grdStatus.Size = new System.Drawing.Size(0, 0);
		this.grdStatus.TabIndex = 0;
		this.grdStatus.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvStatus });
		this.gvStatus.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gvStatus.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gvStatus.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvStatus.ColumnKey = null;
		this.gvStatus.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gcStatus, this.gcStatusColor });
		this.gvStatus.DisableFormSetting = false;
		this.gvStatus.DisplayException = false;
		this.gvStatus.EditColumn = null;
		this.gvStatus.FieldCurrency = null;
		this.gvStatus.FieldRate = null;
		this.gvStatus.FormId = "oCapacityGantt";
		this.gvStatus.GridControl = this.grdStatus;
		this.gvStatus.IndexEditRow = -1;
		this.gvStatus.IndicatorWidth = 20;
		this.gvStatus.IsApzTable = false;
		this.gvStatus.IsInsertNewRow = false;
		this.gvStatus.IsLoadingData = false;
		this.gvStatus.ItemId = "OSTATUS";
		this.gvStatus.LstColumnAllowBatch = null;
		this.gvStatus.LstColumnNotAllowBatch = null;
		this.gvStatus.LstColumNotSetting = null;
		this.gvStatus.Name = "gvStatus";
		this.gvStatus.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gvStatus.OptionsPrint.AutoWidth = false;
		this.gvStatus.OptionsView.BestFitMaxRowCount = 100;
		this.gvStatus.OptionsView.ColumnAutoWidth = false;
		this.gvStatus.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvStatus.OptionsView.ShowGroupPanel = false;
		this.gvStatus.UseBatchMod = false;
		this.gcStatus.Caption = "Status";
		this.gcStatus.CurrencyDateField = null;
		this.gcStatus.CurrencyField = null;
		this.gcStatus.CurrencyFixed = null;
		this.gcStatus.CurrencyFixedRate = 0.0;
		this.gcStatus.CurrencyRateField = null;
		this.gcStatus.FieldName = "ActionName";
		this.gcStatus.Name = "gcStatus";
		this.gcStatus.PrimaryKey = false;
		this.gcStatus.UnitField = null;
		this.gcStatus.Visible = true;
		this.gcStatus.VisibleIndex = 0;
		this.gcStatus.Width = 53;
		this.gcStatusColor.Caption = "Color";
		this.gcStatusColor.ColumnEdit = this.rStatusColor;
		this.gcStatusColor.CurrencyDateField = null;
		this.gcStatusColor.CurrencyField = null;
		this.gcStatusColor.CurrencyFixed = null;
		this.gcStatusColor.CurrencyFixedRate = 0.0;
		this.gcStatusColor.CurrencyRateField = null;
		this.gcStatusColor.FieldName = "Color";
		this.gcStatusColor.Name = "gcStatusColor";
		this.gcStatusColor.PrimaryKey = false;
		this.gcStatusColor.UnitField = null;
		this.gcStatusColor.Visible = true;
		this.gcStatusColor.VisibleIndex = 1;
		this.rStatusColor.AutoHeight = false;
		this.rStatusColor.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rStatusColor.Name = "rStatusColor";
		this.rStatusColor.StoreColorAsInteger = true;
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.Location = new System.Drawing.Point(1511, 633);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 2;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSave.ImageOptions.Image");
		this.btnSave.Location = new System.Drawing.Point(1431, 633);
		this.btnSave.Name = "btnSave";
		this.btnSave.Size = new System.Drawing.Size(75, 23);
		this.btnSave.TabIndex = 3;
		this.btnSave.Text = "Save";
		this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
		this.btnRun.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnRun.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnRun.ImageOptions.Image");
		this.btnRun.Location = new System.Drawing.Point(1351, 633);
		this.btnRun.Name = "btnRun";
		this.btnRun.Size = new System.Drawing.Size(75, 23);
		this.btnRun.TabIndex = 4;
		this.btnRun.Text = "Run";
		this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
		this.lbScheCode.FieldName = "lbScheCode";
		this.lbScheCode.HideWhenEmpty = false;
		this.lbScheCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbScheCode.Location = new System.Drawing.Point(8, 8);
		this.lbScheCode.Name = "lbScheCode";
		this.lbScheCode.ShowUnderLine = true;
		this.lbScheCode.Size = new System.Drawing.Size(96, 20);
		this.lbScheCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbScheCode.TabIndex = 5;
		this.lbScheCode.Utext = "Schedule Code";
		this.txtScheCode.Enabled = false;
		this.txtScheCode.FieldName = "ScheCode";
		this.txtScheCode.IsSearchControl = false;
		this.txtScheCode.Location = new System.Drawing.Point(106, 5);
		this.txtScheCode.Name = "txtScheCode";
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.txtScheCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions)
		});
		this.txtScheCode.Size = new System.Drawing.Size(147, 20);
		this.txtScheCode.TabIndex = 6;
		this.txtScheCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtScheCode_ButtonClick);
		this.txtScheCode.EditValueChanged += new System.EventHandler(this.txtScheCode_EditValueChanged);
		this.lbCapacityCode.FieldName = null;
		this.lbCapacityCode.HideWhenEmpty = false;
		this.lbCapacityCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbCapacityCode.Location = new System.Drawing.Point(272, 8);
		this.lbCapacityCode.Name = "lbCapacityCode";
		this.lbCapacityCode.ShowUnderLine = true;
		this.lbCapacityCode.Size = new System.Drawing.Size(96, 20);
		this.lbCapacityCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbCapacityCode.TabIndex = 7;
		this.lbCapacityCode.Utext = "Capacity Code";
		this.txtCapacityCode.Enabled = false;
		this.txtCapacityCode.FieldName = null;
		this.txtCapacityCode.IsSearchControl = false;
		this.txtCapacityCode.Location = new System.Drawing.Point(369, 5);
		this.txtCapacityCode.Name = "txtCapacityCode";
		this.txtCapacityCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txtCapacityCode.Size = new System.Drawing.Size(147, 20);
		this.txtCapacityCode.TabIndex = 8;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1593, 661);
		base.Controls.Add(this.txtCapacityCode);
		base.Controls.Add(this.lbCapacityCode);
		base.Controls.Add(this.txtScheCode);
		base.Controls.Add(this.lbScheCode);
		base.Controls.Add(this.btnRun);
		base.Controls.Add(this.btnSave);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.SplitReource);
		base.DisableContextAction = true;
		base.FormId = "oCapacityGantt";
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "frm_CapacityGantt";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		this.Text = "Capacity Gantt";
		base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_CapacityGantt_Load);
		base.Resize += new System.EventHandler(this.frm_CapacityGantt_Resize);
		((System.ComponentModel.ISupportInitialize)this.ScheduleGanttChart).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ScheduleStorageGanttChart).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SplitReource).EndInit();
		this.SplitReource.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.ResourceTree).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rResType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SplitApointment).EndInit();
		this.SplitApointment.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.SplitDetail).EndInit();
		this.SplitDetail.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdOpeColor).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvOpeColor).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rColorEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rStatusColor).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtScheCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCapacityCode.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
