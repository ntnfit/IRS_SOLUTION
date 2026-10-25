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
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons.CFL;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;

namespace Apzon.Forms.Administrator.SetUp.Inventory;

public class frm_InventoryCycles : XtraFormEx
{
	private const string Controller = "InventoryCycles";

	private bool isLoad;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private LabelLine lab_Code;

	private LabelLine lab_RepeatEvery;

	private LabelLine labelLine4;

	private LabelLine lbl_Time;

	private LabelLine lab_Recurrence;

	private SimpleButtonEx btn_Add;

	private SimpleButtonEx btn_Cancel;

	private TextEditEx txt_Code;

	private LookUpEditEx look_Recurrence;

	private DateEditEx date_StartCount;

	private LabelLine lab_End;

	private TextEditEx txt_Occurrence;

	private TextEditEx txt_Day;

	private LabelLine lab_Day;

	private LabelLine lab_Occurrence;

	private CheckEditEx check_Mon;

	private CheckEditEx check_Tue;

	private CheckEditEx check_Wed;

	private CheckEditEx check_Thu;

	private CheckEditEx check_Fri;

	private CheckEditEx check_Sat;

	private CheckEditEx check_Sun;

	private TextEditEx txt_DayInMonth;

	private LookUpEditEx look_Week;

	private LookUpEditEx look_DayOfWeek;

	private LabelLine lab_RepeatOn;

	private LabelLine lab_Of;

	private LookUpEditEx look_MonthInAnnually;

	private LookUpEditEx look_MonthOf;

	private CheckEditEx radio_SubOption1;

	private CheckEditEx radio_SubOption2;

	private DateEditEx date_SeEndDat;

	private TextEditEx txtCode;

	private RadioGroupEx radioGroup_EndType;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	private TimeEditEx time_TimeCycle;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			EnableDisableControl(isEnable: true);
			btn_Add.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			EnableDisableControl(isEnable: true);
			btn_Add.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.Find:
			EnableDisableControl(isEnable: true);
			btn_Add.Text = Resources.ButtonFind;
			SetDataDoc(null);
			break;
		case Enums.FormViewModeType.View:
			EnableDisableControl(isEnable: true);
			btn_Add.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_InventoryCycles()
	{
		InitializeComponent();
	}

	private void frm_InventoryCycles_Load(object sender, EventArgs e)
	{
		try
		{
			LoadForm();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void check_Sun_CheckedChanged(object sender, EventArgs e)
	{
	}

	private void look_Recurrence_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			GetDate(Function.ParseDateTimes(date_SeEndDat.EditValue));
			switch (Function.ToString(look_Recurrence.EditValue))
			{
			case "7":
				SetNone();
				break;
			case "0":
				lab_RepeatEvery.Visible = true;
				radioGroup_EndType.Visible = true;
				radio_SubOption1.Visible = true;
				radio_SubOption2.Visible = true;
				radio_SubOption1.Text = "";
				radio_SubOption2.Text = "Every_Weekday";
				txt_Day.Visible = true;
				lab_Day.Visible = true;
				date_SeEndDat.Visible = true;
				txt_Occurrence.Visible = true;
				lab_Occurrence.Visible = true;
				lab_End.Visible = true;
				lab_Day.Text = "Days";
				SetAnnuallyVisible();
				SetWeekVisible();
				radio_SubOption1.Location = new Point(89, 66);
				radio_SubOption2.Location = new Point(89, 89);
				date_StartCount.ReadOnly = true;
				date_StartCount.ReadOnly = true;
				look_MonthInAnnually.Visible = false;
				lab_Of.Visible = false;
				look_MonthOf.Visible = false;
				break;
			case "1":
				date_SeEndDat.Visible = true;
				txt_Occurrence.Visible = true;
				lab_Occurrence.Visible = true;
				lab_End.Visible = true;
				radioGroup_EndType.Visible = true;
				lab_RepeatEvery.Visible = true;
				txt_Day.Visible = true;
				lab_Day.Visible = true;
				lab_Day.Text = "Weeks";
				check_Mon.Visible = true;
				check_Fri.Visible = true;
				check_Sat.Visible = true;
				check_Sun.Visible = true;
				check_Thu.Visible = true;
				check_Tue.Visible = true;
				check_Wed.Visible = true;
				radio_SubOption1.Visible = false;
				radio_SubOption2.Visible = false;
				SetAnnuallyVisible();
				date_StartCount.ReadOnly = true;
				look_MonthInAnnually.Visible = false;
				lab_Of.Visible = false;
				look_MonthOf.Visible = false;
				break;
			case "3":
				date_SeEndDat.Visible = true;
				txt_Occurrence.Visible = true;
				lab_Occurrence.Visible = true;
				lab_End.Visible = true;
				lab_RepeatEvery.Visible = true;
				radioGroup_EndType.Visible = true;
				txt_Day.Visible = true;
				lab_Day.Visible = true;
				lab_RepeatOn.Visible = true;
				lab_Day.Text = "Months";
				txt_DayInMonth.Visible = true;
				look_Week.Visible = true;
				look_DayOfWeek.Visible = true;
				radio_SubOption1.Visible = true;
				radio_SubOption2.Visible = true;
				radio_SubOption1.Text = "Day";
				radio_SubOption2.Text = "";
				radio_SubOption1.Location = new Point(89, 89);
				radio_SubOption2.Location = new Point(89, 113);
				txt_DayInMonth.Location = new Point(135, 89);
				look_DayOfWeek.Location = new Point(229, 112);
				look_Week.Location = new Point(120, 112);
				SetWeekVisible();
				date_StartCount.ReadOnly = true;
				look_MonthInAnnually.Visible = false;
				lab_Of.Visible = false;
				look_MonthOf.Visible = false;
				break;
			case "6":
				lab_RepeatOn.Visible = true;
				date_SeEndDat.Visible = true;
				txt_Occurrence.Visible = true;
				lab_Occurrence.Visible = true;
				lab_End.Visible = true;
				radio_SubOption1.Visible = true;
				radio_SubOption2.Visible = true;
				lab_RepeatEvery.Visible = true;
				radioGroup_EndType.Visible = true;
				txt_Day.Visible = true;
				lab_Day.Visible = true;
				lab_Day.Text = "Years";
				txt_DayInMonth.Visible = true;
				look_Week.Visible = true;
				look_DayOfWeek.Visible = true;
				look_MonthInAnnually.Visible = true;
				lab_Of.Visible = true;
				look_MonthOf.Visible = true;
				radio_SubOption1.Text = "";
				radio_SubOption2.Text = "";
				radio_SubOption1.Location = new Point(89, 89);
				radio_SubOption2.Location = new Point(89, 113);
				look_MonthInAnnually.Location = new Point(117, 89);
				lab_Of.Location = new Point(347, 119);
				look_DayOfWeek.Location = new Point(229, 112);
				look_Week.Location = new Point(120, 112);
				look_MonthOf.Location = new Point(383, 112);
				txt_DayInMonth.Location = new Point(229, 89);
				SetWeekVisible();
				date_StartCount.ReadOnly = true;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Add_Click(object sender, EventArgs e)
	{
		_ = FormMode;
		try
		{
			if (FormMode == Enums.FormViewModeType.Add)
			{
				await Save(1);
			}
			else if (FormMode == Enums.FormViewModeType.Update)
			{
				await Save(2);
			}
			else if (FormMode == Enums.FormViewModeType.Find)
			{
				WinCommons._OpenProcessing();
				await Find(isFind: true);
			}
			else
			{
				Close();
				WinCommons._OpenProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void radio_SubOption1_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!isLoad)
			{
				look_DayOfWeek.ReadOnly = true;
				look_Week.ReadOnly = true;
				look_MonthOf.ReadOnly = true;
				look_MonthInAnnually.ReadOnly = false;
				txt_DayInMonth.ReadOnly = false;
				txt_Day.ReadOnly = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void radio_SubOption2_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!isLoad)
			{
				look_DayOfWeek.ReadOnly = false;
				look_Week.ReadOnly = false;
				look_MonthOf.ReadOnly = false;
				look_MonthInAnnually.ReadOnly = true;
				txt_DayInMonth.ReadOnly = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void radioGroup_EndType_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			if (!isLoad)
			{
				switch (radioGroup_EndType.SelectedIndex)
				{
				case 0:
					txt_Occurrence.ReadOnly = true;
					date_SeEndDat.ReadOnly = true;
					break;
				case 1:
					txt_Occurrence.ReadOnly = false;
					date_SeEndDat.ReadOnly = true;
					break;
				case 2:
					txt_Occurrence.ReadOnly = true;
					date_SeEndDat.ReadOnly = false;
					break;
				default:
					txt_Occurrence.ReadOnly = true;
					date_SeEndDat.ReadOnly = true;
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

	private void look_Week_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!isLoad)
			{
				DateTime dateTime = Function.ParseDateTimes(date_StartCount);
				int week = Function.ParseInt(look_Week.EditValue);
				string weekOfDay = Function.ToString(look_DayOfWeek.EditValue);
				date_StartCount.EditValue = GetDateByDayWeek(dateTime, week, weekOfDay);
				txt_DayInMonth.EditValue = Function.ParseDateTimes(date_StartCount.EditValue).Day;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void look_DayOfWeek_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!isLoad)
			{
				DateTime dateTime = Function.ParseDateTimes(date_StartCount);
				int week = Function.ParseInt(look_Week.EditValue);
				string weekOfDay = Function.ToString(look_DayOfWeek.EditValue);
				date_StartCount.EditValue = GetDateByDayWeek(dateTime, week, weekOfDay);
				txt_DayInMonth.EditValue = Function.ParseDateTimes(date_StartCount.EditValue).Day;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void look_MonthInAnnually_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!isLoad && (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update))
			{
				look_MonthOf.EditValue = look_MonthInAnnually.EditValue;
				DateTime dateTime = Function.ParseDateTimes(date_StartCount);
				int week = Function.ParseInt(look_Week.EditValue);
				string weekOfDay = Function.ToString(look_DayOfWeek.EditValue);
				int month = Function.ParseInt(look_MonthInAnnually.EditValue);
				date_StartCount.EditValue = GetDateByMonth(dateTime, week, weekOfDay, month);
				txt_DayInMonth.EditValue = Function.ParseDateTimes(date_StartCount.EditValue).Day;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_DayInMonth_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!isLoad && (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update))
			{
				DateTime dateTime = Function.ParseDateTimes(date_StartCount.EditValue);
				int month = dateTime.Month;
				int year = dateTime.Year;
				int num = Function.ParseInt(txt_DayInMonth.EditValue);
				if (num > 0)
				{
					DateTime dateTime2 = new DateTime(year, month, num);
					GetDate(dateTime2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_Day_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void look_MonthOf_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!isLoad && (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update))
			{
				look_MonthInAnnually.EditValue = look_MonthOf.EditValue;
				DateTime dateTime = Function.ParseDateTimes(date_StartCount);
				int week = Function.ParseInt(look_Week.EditValue);
				string weekOfDay = Function.ToString(look_DayOfWeek.EditValue);
				int month = Function.ParseInt(look_MonthInAnnually.EditValue);
				date_StartCount.EditValue = GetDateByMonth(dateTime, week, weekOfDay, month);
				txt_DayInMonth.EditValue = Function.ParseDateTimes(date_StartCount.EditValue).Day;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (!isLoad)
		{
			Enums.FormViewModeType oldView = FormMode;
			FormMode = Enums.FormViewModeType.Busy;
			try
			{
				await Save(3);
				ResetFormControl();
				oldView = Enums.FormViewModeType.Find;
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.ErrorLineNumber(ex);
			}
			FormMode = oldView;
			WinCommons._CloseProcessing();
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.Previous) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.Next) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_FIRST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.First) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.Last) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MODE_FIND()
	{
		ResetFormControl();
		FormMode = Enums.FormViewModeType.Find;
		txt_Code.Focus();
	}

	public async Task REFRESH()
	{
		try
		{
			LoadForm();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Find(bool isFind)
	{
		frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.InventoryCyles, false, Function.ToString(txtCode.EditValue), "", Enums.ItemObjectType.NoItemType, null, "");
		DataTable dt = new DataTable();
		GetDataDocFind(ref dt);
		frm_CFL2.Search = dt;
		frm_CFL2.ShowDialog(this);
		if (frm_CFL2.ChooseItem != null)
		{
			SetDataDoc(frm_CFL2.ChooseItem);
		}
		FormMode = Enums.FormViewModeType.View;
	}

	private async Task<int> FindDataNextLast(Enums.NavigateType type)
	{
		try
		{
			DataTable dataTable = await _httpClient.InventoryCycles.FindNextLastFrist(Function.ParseInt(txtCode.EditValue), (int)type);
			if (dataTable.IsNotNull())
			{
				SetDataDoc(dataTable.Rows[0]);
				return 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	public void SetReccurrence()
	{
		try
		{
			List<Common.Table_Code> list = new List<Common.Table_Code>();
			list.Add(new Common.Table_Code
			{
				Name = "Daily",
				Code = "0"
			});
			list.Add(new Common.Table_Code
			{
				Name = "Weekly",
				Code = "1"
			});
			list.Add(new Common.Table_Code
			{
				Name = "Monthly",
				Code = "3"
			});
			list.Add(new Common.Table_Code
			{
				Name = "Annually",
				Code = "6"
			});
			list.Add(new Common.Table_Code
			{
				Name = "None",
				Code = "7"
			});
			look_Recurrence.Properties.DataSource = list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void SetWeekInMonth()
	{
		try
		{
			List<Common.Table_Code> list = new List<Common.Table_Code>();
			list.Add(new Common.Table_Code
			{
				Name = "First",
				Code = "1"
			});
			list.Add(new Common.Table_Code
			{
				Name = "Second",
				Code = "2"
			});
			list.Add(new Common.Table_Code
			{
				Name = "Third",
				Code = "3"
			});
			list.Add(new Common.Table_Code
			{
				Name = "Fourth",
				Code = "4"
			});
			list.Add(new Common.Table_Code
			{
				Name = "Last",
				Code = "5"
			});
			look_Week.Properties.DataSource = list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void SetDayOfWeek()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		list.Add(new Common.Table_Code
		{
			Name = "Day",
			Code = "1"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Weekday",
			Code = "2"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Weekend day",
			Code = "3"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Sunday",
			Code = "4"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Monday",
			Code = "5"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Tuesday",
			Code = "6"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Wednesday",
			Code = "7"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Thursday",
			Code = "8"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Friday",
			Code = "9"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Saturday",
			Code = "10"
		});
		look_DayOfWeek.Properties.DataSource = list;
	}

	public void SetMonthInAnnually()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		list.Add(new Common.Table_Code
		{
			Name = "January",
			Code = "1"
		});
		list.Add(new Common.Table_Code
		{
			Name = "February",
			Code = "2"
		});
		list.Add(new Common.Table_Code
		{
			Name = "March",
			Code = "3"
		});
		list.Add(new Common.Table_Code
		{
			Name = "April",
			Code = "4"
		});
		list.Add(new Common.Table_Code
		{
			Name = "May",
			Code = "5"
		});
		list.Add(new Common.Table_Code
		{
			Name = "June",
			Code = "6"
		});
		list.Add(new Common.Table_Code
		{
			Name = "July",
			Code = "7"
		});
		list.Add(new Common.Table_Code
		{
			Name = "August",
			Code = "8"
		});
		list.Add(new Common.Table_Code
		{
			Name = "September",
			Code = "9"
		});
		list.Add(new Common.Table_Code
		{
			Name = "October",
			Code = "10"
		});
		list.Add(new Common.Table_Code
		{
			Name = "November",
			Code = "11"
		});
		list.Add(new Common.Table_Code
		{
			Name = "December",
			Code = "12"
		});
		look_MonthInAnnually.Properties.DataSource = list;
		look_MonthOf.Properties.DataSource = list;
	}

	public void SetNone()
	{
		lab_RepeatEvery.Visible = false;
		radioGroup_EndType.Visible = false;
		radio_SubOption1.Visible = false;
		radio_SubOption2.Visible = false;
		txt_Day.Visible = false;
		lab_Day.Visible = false;
		check_Mon.Visible = false;
		check_Fri.Visible = false;
		check_Sat.Visible = false;
		check_Sun.Visible = false;
		check_Thu.Visible = false;
		check_Tue.Visible = false;
		check_Wed.Visible = false;
		txt_DayInMonth.Visible = false;
		look_MonthOf.Visible = false;
		txt_Occurrence.Visible = false;
		date_SeEndDat.Visible = false;
		lab_End.Visible = false;
		lab_Occurrence.Visible = false;
		lab_RepeatOn.Visible = false;
		look_DayOfWeek.Visible = false;
		look_MonthInAnnually.Visible = false;
		look_Week.Visible = false;
		lab_Of.Visible = false;
	}

	public void SetWeekVisible()
	{
		check_Mon.Visible = false;
		check_Fri.Visible = false;
		check_Sat.Visible = false;
		check_Sun.Visible = false;
		check_Thu.Visible = false;
		check_Tue.Visible = false;
		check_Wed.Visible = false;
	}

	public void SetAnnuallyVisible()
	{
		txt_DayInMonth.Visible = false;
		look_Week.Visible = false;
		look_DayOfWeek.Visible = false;
		look_MonthInAnnually.Visible = false;
		lab_Of.Visible = false;
		look_MonthOf.Visible = false;
	}

	private void EnableDisableControl(bool isEnable)
	{
		try
		{
			btn_Add.Enabled = isEnable;
			btn_Cancel.Enabled = isEnable;
			date_StartCount.ReadOnly = !isEnable;
			time_TimeCycle.ReadOnly = !isEnable;
			txt_Code.ReadOnly = !isEnable;
			look_Recurrence.ReadOnly = !isEnable;
			look_Week.ReadOnly = !isEnable;
			look_MonthOf.ReadOnly = !isEnable;
			look_MonthInAnnually.ReadOnly = !isEnable;
			look_DayOfWeek.ReadOnly = !isEnable;
			txt_Day.Enabled = isEnable;
			txt_DayInMonth.Enabled = isEnable;
			date_SeEndDat.ReadOnly = !isEnable;
			txt_Occurrence.ReadOnly = !isEnable;
			radioGroup_EndType.Enabled = isEnable;
			radio_SubOption1.Enabled = isEnable;
			radio_SubOption2.Enabled = isEnable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetDataDoc(DataRow row)
	{
		try
		{
			ResetFormControl();
			if (row != null)
			{
				txtCode.EditValue = Function.ParseInt(row["Code"]);
				txt_Code.EditValue = Function.ToString(row["Name"]);
				txt_Day.EditValue = Function.ParseInt(row["Interval"]);
				txt_DayInMonth.EditValue = Function.ParseInt(row["DayInMonth"]);
				txt_Occurrence.EditValue = Function.ParseInt(row["MaxOccur"]);
				date_StartCount.EditValue = Function.ParseDateTime(row["NextDate"]);
				date_SeEndDat.EditValue = Function.ParseDateTime(row["SeEndDat"]);
				time_TimeCycle.EditValue = Function.ParseInt(row["Hour"]);
				string text = Function.ToString(row["SubOption"]);
				if (text == "1")
				{
					radio_SubOption1.Checked = true;
				}
				else if (text == "2")
				{
					radio_SubOption1.Checked = true;
				}
				switch (Function.ToString(row["EndType"]))
				{
				case "N":
					radioGroup_EndType.SelectedIndex = 0;
					break;
				case "C":
					radioGroup_EndType.SelectedIndex = 1;
					break;
				case "D":
					radioGroup_EndType.SelectedIndex = 2;
					break;
				default:
					radioGroup_EndType.SelectedIndex = 0;
					break;
				}
				look_Recurrence.EditValue = Function.ToString(row["Frequency"]);
				look_Week.EditValue = Function.ParseInt(row["Week"]);
				look_DayOfWeek.EditValue = Function.ParseInt(row["DayOfWeek"]);
				look_MonthInAnnually.EditValue = Function.ToString(row["Month"]);
				look_MonthOf.EditValue = Function.ToString(row["Month"]);
				if (Function.ToString(row["Monday"]) == "Y")
				{
					check_Mon.Checked = true;
				}
				else
				{
					check_Mon.Checked = false;
				}
				if (Function.ToString(row["Friday"]) == "Y")
				{
					check_Fri.Checked = true;
				}
				else
				{
					check_Fri.Checked = false;
				}
				if (Function.ToString(row["Saturday"]) == "Y")
				{
					check_Sat.Checked = true;
				}
				else
				{
					check_Sat.Checked = false;
				}
				if (Function.ToString(row["Sunday"]) == "Y")
				{
					check_Sun.Checked = true;
				}
				else
				{
					check_Sun.Checked = false;
				}
				if (Function.ToString(row["Thursday"]) == "Y")
				{
					check_Thu.Checked = true;
				}
				else
				{
					check_Thu.Checked = false;
				}
				if (Function.ToString(row["Tuesday"]) == "Y")
				{
					check_Tue.Checked = true;
				}
				else
				{
					check_Tue.Checked = false;
				}
				if (Function.ToString(row["Wednesday"]) == "Y")
				{
					check_Wed.Checked = true;
				}
				else
				{
					check_Wed.Checked = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ResetFormControl()
	{
		isLoad = true;
		txt_Day.EditValue = null;
		txt_Code.EditValue = null;
		txtCode.EditValue = null;
		txt_DayInMonth.EditValue = null;
		txt_Occurrence.EditValue = null;
		date_SeEndDat.EditValue = null;
		date_StartCount.EditValue = null;
		time_TimeCycle.EditValue = null;
		radioGroup_EndType.EditValue = null;
		radio_SubOption1.Checked = false;
		radio_SubOption2.Checked = false;
		check_Mon.EditValue = "N";
		check_Fri.EditValue = "N";
		check_Sat.EditValue = "N";
		check_Sun.EditValue = "N";
		check_Thu.EditValue = "N";
		check_Tue.EditValue = "N";
		check_Wed.EditValue = "N";
		look_DayOfWeek.EditValue = null;
		look_MonthInAnnually.EditValue = null;
		look_MonthOf.EditValue = null;
		look_Recurrence.EditValue = null;
		look_Week.EditValue = null;
		isLoad = false;
	}

	public async void LoadForm()
	{
		try
		{
			ResetFormControl();
			TextEditEx textEditEx = txtCode;
			textEditEx.EditValue = await GetMaxId() + 1;
			look_Recurrence.EditValue = "0";
			date_StartCount.EditValue = DateTime.Now.Date;
			time_TimeCycle.EditValue = DateTime.Now.Hour * 100 + DateTime.Now.Minute;
			radio_SubOption1.Checked = true;
			SetReccurrence();
			SetWeekInMonth();
			SetDayOfWeek();
			SetMonthInAnnually();
			SetNone();
			txt_Day.EditValue = "1";
			txt_Occurrence.EditValue = "1";
			txt_DayInMonth.EditValue = DateTime.Now.Day;
			date_SeEndDat.EditValue = DateTime.Now.Date;
			radioGroup_EndType.SelectedIndex = 0;
			GetDate(Function.ParseDateTimes(date_SeEndDat.EditValue));
			FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task<int> GetMaxId()
	{
		try
		{
			return await _httpClient.InventoryCycles.GetMaxId();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	private void GetDataDoc(ref DataTable dt)
	{
		try
		{
			dt = new DataTable();
			dt.Columns.Add("Code", typeof(int));
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("Frequency", typeof(string));
			dt.Columns.Add("Day", typeof(int));
			dt.Columns.Add("Hour", typeof(int));
			dt.Columns.Add("UserSign", typeof(int));
			dt.Columns.Add("NextDate", typeof(DateTime));
			dt.Columns.Add("Type", typeof(string));
			dt.Columns.Add("SubOption", typeof(string));
			dt.Columns.Add("Interval", typeof(int));
			dt.Columns.Add("EndType", typeof(string));
			dt.Columns.Add("MaxOccur", typeof(int));
			dt.Columns.Add("SeEndDat", typeof(DateTime));
			dt.Columns.Add("Sunday", typeof(string));
			dt.Columns.Add("Monday", typeof(string));
			dt.Columns.Add("Tuesday", typeof(string));
			dt.Columns.Add("Wednesday", typeof(string));
			dt.Columns.Add("Thursday", typeof(string));
			dt.Columns.Add("Friday", typeof(string));
			dt.Columns.Add("Saturday", typeof(string));
			dt.Columns.Add("DayInMonth", typeof(int));
			dt.Columns.Add("Week", typeof(int));
			dt.Columns.Add("DayOfWeek", typeof(int));
			dt.Columns.Add("Month", typeof(int));
			DataRow dataRow = dt.NewRow();
			dataRow["Code"] = Function.ParseInt(txtCode.EditValue);
			dataRow["Name"] = Function.ToString(txt_Code.Text);
			if (radio_SubOption1.Checked)
			{
				dataRow["SubOption"] = "1";
			}
			else if (radio_SubOption2.Checked)
			{
				dataRow["SubOption"] = "2";
			}
			switch (radioGroup_EndType.SelectedIndex)
			{
			case 0:
				dataRow["EndType"] = "N";
				break;
			case 1:
				dataRow["EndType"] = "C";
				break;
			case 2:
				dataRow["EndType"] = "D";
				break;
			default:
				dataRow["EndType"] = "N";
				break;
			}
			dataRow["Frequency"] = Function.ToString(look_Recurrence.EditValue);
			dataRow["Day"] = DBNull.Value;
			dataRow["Hour"] = Function.ParseInt(time_TimeCycle.EditValue);
			dataRow["UserSign"] = GlobalData.UserID;
			DateTime? dateTime = Function.ParseDateTime(date_StartCount.EditValue);
			if (dateTime.HasValue)
			{
				dataRow["NextDate"] = dateTime.Value;
			}
			dataRow["Type"] = "C";
			dataRow["Sunday"] = Function.ToString(check_Sun.EditValue);
			dataRow["Monday"] = Function.ToString(check_Mon.EditValue);
			dataRow["Tuesday"] = Function.ToString(check_Tue.EditValue);
			dataRow["Wednesday"] = Function.ToString(check_Wed.EditValue);
			dataRow["Thursday"] = Function.ToString(check_Thu.EditValue);
			dataRow["Friday"] = Function.ToString(check_Fri.EditValue);
			dataRow["Saturday"] = Function.ToString(check_Sat.EditValue);
			dataRow["Interval"] = Function.ParseInt(txt_Day.EditValue);
			dataRow["MaxOccur"] = Function.ParseInt(txt_Occurrence.EditValue);
			dataRow["DayInMonth"] = Function.ParseInt(txt_DayInMonth.EditValue);
			dataRow["Week"] = Function.ParseInt(look_Week.EditValue);
			dataRow["DayOfWeek"] = Function.ParseInt(look_DayOfWeek.EditValue);
			dataRow["Month"] = Function.ParseInt(look_MonthInAnnually.EditValue);
			DateTime? dateTime2 = Function.ParseDateTime(date_SeEndDat.EditValue);
			if (dateTime2.HasValue)
			{
				dataRow["SeEndDat"] = dateTime2.Value;
			}
			dt.Rows.Add(dataRow);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetDataDocFind(ref DataTable dt)
	{
		try
		{
			dt = new DataTable();
			dt.Columns.Add("Code", typeof(int));
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("EndType", typeof(string));
			dt.Columns.Add("Frequency", typeof(string));
			dt.Columns.Add("SubOption", typeof(string));
			dt.Columns.Add("Interval", typeof(int));
			dt.Columns.Add("MaxOccur", typeof(int));
			dt.Columns.Add("DayInMonth", typeof(int));
			dt.Columns.Add("Week", typeof(int));
			dt.Columns.Add("DayOfWeek", typeof(int));
			dt.Columns.Add("Month", typeof(int));
			dt.Columns.Add("SeEndDat", typeof(string));
			dt.Columns.Add("NextDate", typeof(string));
			DataRow dataRow = dt.NewRow();
			dataRow["Code"] = Function.ParseInt(txtCode.EditValue);
			dataRow["Name"] = Function.ToString(txt_Code.EditValue);
			if (radio_SubOption1.Checked)
			{
				dataRow["SubOption"] = "1";
			}
			else if (radio_SubOption2.Checked)
			{
				dataRow["SubOption"] = "2";
			}
			switch (radioGroup_EndType.SelectedIndex)
			{
			case 0:
				dataRow["Frequency"] = "N";
				break;
			case 1:
				dataRow["Frequency"] = "C";
				break;
			case 2:
				dataRow["Frequency"] = "D";
				break;
			default:
				dataRow["Frequency"] = "";
				break;
			}
			dataRow["Frequency"] = Function.ToString(look_Recurrence.EditValue);
			dataRow["Interval"] = Function.ParseInt(txt_Day.EditValue);
			dataRow["MaxOccur"] = Function.ParseInt(txt_Occurrence.EditValue);
			dataRow["DayInMonth"] = Function.ParseInt(txt_DayInMonth.EditValue);
			dataRow["Week"] = Function.ParseInt(look_Week.EditValue);
			dataRow["DayOfWeek"] = Function.ParseInt(look_DayOfWeek.EditValue);
			dataRow["Month"] = Function.ParseInt(look_MonthInAnnually.EditValue);
			dataRow["SeEndDat"] = ((!string.IsNullOrEmpty(Function.ToString(date_SeEndDat.EditValue))) ? Function.ToString(Function.ParseDateTimes(date_SeEndDat.EditValue).Date) : "");
			dataRow["NextDate"] = ((!string.IsNullOrEmpty(Function.ToString(date_StartCount.EditValue))) ? Function.ToString(Function.ParseDateTimes(date_StartCount.EditValue).Date) : "");
			dt.Rows.Add(dataRow);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> Save(int type)
	{
		_ = 2;
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(date_StartCount.EditValue)))
			{
				date_StartCount.Focus();
				WinCommons.Error(Enums.MessageCode.StartCountDataIsNotNull, "");
				return 1;
			}
			if (string.IsNullOrEmpty(Function.ToString(time_TimeCycle.EditValue)))
			{
				time_TimeCycle.Focus();
				WinCommons.Error(Enums.MessageCode.TimeMustNotBeNull, "");
				return 1;
			}
			if (XtraMessageBoxEx.Show((type == 3) ? Enums.MessageCode.ConfirmDelete : Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				DataTable dtdoc = new DataTable();
				GetDataDoc(ref dtdoc);
				HttpResult httpResult = type switch
				{
					3 => await _httpClient.InventoryCycles.Delete(dtdoc), 
					2 => await _httpClient.InventoryCycles.Update(dtdoc), 
					_ => await _httpClient.InventoryCycles.Saves(dtdoc), 
				};
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					txtCode.EditValue = Function.ParseInt(dtdoc.Rows[0][0]);
					if (type == 3)
					{
						WinCommons.Success(Enums.MessageCode.RemoveSuccess, "");
						FormMode = Enums.FormViewModeType.Find;
					}
					else
					{
						WinCommons.Success((type == 1) ? Enums.MessageCode.SaveSuccess : Enums.MessageCode.UpdateSuccess, "");
						FormMode = Enums.FormViewModeType.View;
					}
					return 0;
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
		return 1;
	}

	public void GetDate(DateTime dateTime)
	{
		try
		{
			DayOfWeek dayOfWeek = dateTime.DayOfWeek;
			dateTime.DayOfWeek.ToString();
			int month = dateTime.Month;
			int year = dateTime.Year;
			DateTime dateTime2 = new DateTime(year, month, 1);
			while (dateTime2.DayOfWeek != dayOfWeek)
			{
				dateTime2 = dateTime2.AddDays(1.0);
			}
			DateTime dateTime3 = dateTime2.AddDays(7.0);
			DateTime dateTime4 = dateTime3.AddDays(7.0);
			DateTime dateTime5 = dateTime4.AddDays(7.0);
			DateTime dateTime6 = new DateTime(year, month, 1).AddMonths(1).AddDays(-1.0);
			while (dateTime6.DayOfWeek != dayOfWeek)
			{
				dateTime6 = dateTime6.AddDays(-1.0);
			}
			look_DayOfWeek.EditValue = Function.ParseInt(GetDayOfWeek(dayOfWeek));
			look_MonthOf.EditValue = month;
			look_MonthInAnnually.EditValue = month;
			if (dateTime.Date == dateTime2.Date)
			{
				look_Week.EditValue = "1";
			}
			else if (dateTime.Date == dateTime3.Date)
			{
				look_Week.EditValue = "2";
			}
			else if (dateTime.Date == dateTime4.Date)
			{
				look_Week.EditValue = "3";
			}
			else if (dateTime.Date == dateTime5.Date)
			{
				look_Week.EditValue = "4";
			}
			else if (dateTime.Date == dateTime6.Date)
			{
				look_Week.EditValue = "5";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public DateTime GetDateByDayWeek(DateTime dateTime, int week, string weekOfDay)
	{
		try
		{
			DateTime dateTime2 = default;
			DateTime dateTime3 = default;
			DateTime dateTime4 = default;
			DateTime dateTime5 = default;
			DateTime dateTime6 = default;
			DayOfWeek dayOfWeek = ConvertDayOfWeek(weekOfDay);
			dateTime.DayOfWeek.ToString();
			int month = dateTime.Month;
			int year = dateTime.Year;
			switch (weekOfDay)
			{
			case "1":
				dateTime2 = new DateTime(year, month, 1);
				dateTime3 = dateTime2.AddDays(1.0);
				dateTime4 = dateTime3.AddDays(1.0);
				dateTime5 = dateTime4.AddDays(1.0);
				dateTime6 = new DateTime(year, month + 1, 1).AddDays(-1.0);
				break;
			case "2":
				dateTime2 = new DateTime(year, month, 1).AddDays(1.0);
				dateTime3 = dateTime2.AddDays(1.0);
				dateTime4 = dateTime3.AddDays(1.0);
				dateTime5 = dateTime4.AddDays(1.0);
				dateTime6 = new DateTime(year, month + 1, 1).AddDays(-1.0);
				break;
			case "3":
				dateTime2 = new DateTime(year, month, 1);
				while (dateTime2.DayOfWeek != DayOfWeek.Saturday)
				{
					dateTime2 = dateTime2.AddDays(1.0);
				}
				dateTime3 = dateTime2.AddDays(1.0);
				dateTime4 = dateTime2.AddDays(7.0);
				dateTime5 = dateTime4.AddDays(1.0);
				dateTime6 = new DateTime(year, month + 1, 1).AddDays(-1.0);
				while (dateTime6.DayOfWeek != DayOfWeek.Saturday)
				{
					dateTime6 = dateTime6.AddDays(-1.0);
				}
				break;
			default:
				dateTime2 = new DateTime(year, month, 1);
				while (dateTime2.DayOfWeek != dayOfWeek)
				{
					dateTime2 = dateTime2.AddDays(1.0);
				}
				dateTime3 = dateTime2.AddDays(7.0);
				dateTime4 = dateTime3.AddDays(7.0);
				dateTime5 = dateTime4.AddDays(7.0);
				dateTime6 = new DateTime(year, month, 1).AddMonths(1).AddDays(-1.0);
				while (dateTime6.DayOfWeek != dayOfWeek)
				{
					dateTime6 = dateTime6.AddDays(-1.0);
				}
				break;
			}
			switch (week)
			{
			case 1:
				date_StartCount.EditValue = dateTime2;
				break;
			case 2:
				date_StartCount.EditValue = dateTime3;
				break;
			case 3:
				date_StartCount.EditValue = dateTime4;
				break;
			case 4:
				date_StartCount.EditValue = dateTime5;
				break;
			case 5:
				date_StartCount.EditValue = dateTime6;
				break;
			}
			return Function.ParseDateTimes(date_StartCount.EditValue);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return default;
		}
	}

	public DateTime GetDateByMonth(DateTime dateTime, int week, string weekOfDay, int month)
	{
		try
		{
			DateTime dateTime2 = default;
			DateTime dateTime3 = default;
			DateTime dateTime4 = default;
			DateTime dateTime5 = default;
			DateTime dateTime6 = default;
			DayOfWeek dayOfWeek = ConvertDayOfWeek(weekOfDay);
			dateTime.DayOfWeek.ToString();
			int year = dateTime.Year;
			switch (weekOfDay)
			{
			case "1":
				dateTime2 = new DateTime(year, month, 1);
				dateTime3 = dateTime2.AddDays(1.0);
				dateTime4 = dateTime3.AddDays(1.0);
				dateTime5 = dateTime4.AddDays(1.0);
				dateTime6 = new DateTime(year, month, 1).AddMonths(1).AddDays(-1.0);
				break;
			case "2":
				dateTime2 = new DateTime(year, month, 1).AddDays(1.0);
				dateTime3 = dateTime2.AddDays(1.0);
				dateTime4 = dateTime3.AddDays(1.0);
				dateTime5 = dateTime4.AddDays(1.0);
				dateTime6 = new DateTime(year, month, 1).AddMonths(1).AddDays(-1.0);
				break;
			case "3":
				dateTime2 = new DateTime(year, month, 1);
				while (dateTime2.DayOfWeek != DayOfWeek.Saturday)
				{
					dateTime2 = dateTime2.AddDays(1.0);
				}
				dateTime3 = dateTime2.AddDays(1.0);
				dateTime4 = dateTime2.AddDays(7.0);
				dateTime5 = dateTime4.AddDays(1.0);
				dateTime6 = new DateTime(year, month, 1).AddMonths(1).AddDays(-1.0);
				while (dateTime6.DayOfWeek != DayOfWeek.Saturday)
				{
					dateTime6 = dateTime6.AddDays(-1.0);
				}
				break;
			default:
				dateTime2 = new DateTime(year, month, 1);
				while (dateTime2.DayOfWeek != dayOfWeek)
				{
					dateTime2 = dateTime2.AddDays(1.0);
				}
				dateTime3 = dateTime2.AddDays(7.0);
				dateTime4 = dateTime3.AddDays(7.0);
				dateTime5 = dateTime4.AddDays(7.0);
				dateTime6 = new DateTime(year, month, 1).AddMonths(1).AddDays(-1.0);
				while (dateTime6.DayOfWeek != dayOfWeek)
				{
					dateTime6 = dateTime6.AddDays(-1.0);
				}
				break;
			}
			switch (week)
			{
			case 1:
				date_StartCount.EditValue = dateTime2;
				break;
			case 2:
				date_StartCount.EditValue = dateTime3;
				break;
			case 3:
				date_StartCount.EditValue = dateTime4;
				break;
			case 4:
				date_StartCount.EditValue = dateTime5;
				break;
			case 5:
				date_StartCount.EditValue = dateTime6;
				break;
			}
			return Function.ParseDateTimes(date_StartCount.EditValue);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return default;
		}
	}

	public string GetDayOfWeek(DayOfWeek dayOfWeek)
	{
		try
		{
			switch (dayOfWeek)
			{
			case DayOfWeek.Sunday:
				look_DayOfWeek.EditValue = "4";
				break;
			case DayOfWeek.Monday:
				look_DayOfWeek.EditValue = "5";
				break;
			case DayOfWeek.Tuesday:
				look_DayOfWeek.EditValue = "6";
				break;
			case DayOfWeek.Wednesday:
				look_DayOfWeek.EditValue = "7";
				break;
			case DayOfWeek.Thursday:
				look_DayOfWeek.EditValue = "8";
				break;
			case DayOfWeek.Friday:
				look_DayOfWeek.EditValue = "9";
				break;
			case DayOfWeek.Saturday:
				look_DayOfWeek.EditValue = "10";
				break;
			}
			return Function.ToString(look_DayOfWeek.EditValue);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return "";
		}
	}

	public DayOfWeek ConvertDayOfWeek(string dayOfWeek)
	{
		try
		{
			DayOfWeek result = DayOfWeek.Sunday;
			switch (dayOfWeek)
			{
			case "4":
				result = DayOfWeek.Sunday;
				break;
			case "5":
				result = DayOfWeek.Monday;
				break;
			case "6":
				result = DayOfWeek.Tuesday;
				break;
			case "7":
				result = DayOfWeek.Wednesday;
				break;
			case "8":
				result = DayOfWeek.Thursday;
				break;
			case "9":
				result = DayOfWeek.Friday;
				break;
			case "10":
				result = DayOfWeek.Saturday;
				break;
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return DayOfWeek.Sunday;
		}
	}

	private void frm_InventoryCycles_OnControlValueChanged(object sender, ControlChangedEventArgs e)
	{
		if (!isLoad && FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void frm_InventoryCycles_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.SetUp.Inventory.frm_InventoryCycles));
		this.btn_Add = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_Code = new Apzon.Controls.Editor.TextEditEx();
		this.look_Recurrence = new Apzon.Controls.Editor.LookUpEditEx();
		this.date_StartCount = new Apzon.Controls.Editor.DateEditEx();
		this.txt_Occurrence = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Day = new Apzon.Controls.Editor.TextEditEx();
		this.lab_Day = new Apzon.Controls.Controls.LabelLine();
		this.lab_Occurrence = new Apzon.Controls.Controls.LabelLine();
		this.check_Mon = new Apzon.Controls.Editor.CheckEditEx();
		this.check_Tue = new Apzon.Controls.Editor.CheckEditEx();
		this.check_Wed = new Apzon.Controls.Editor.CheckEditEx();
		this.check_Thu = new Apzon.Controls.Editor.CheckEditEx();
		this.check_Fri = new Apzon.Controls.Editor.CheckEditEx();
		this.check_Sat = new Apzon.Controls.Editor.CheckEditEx();
		this.check_Sun = new Apzon.Controls.Editor.CheckEditEx();
		this.txt_DayInMonth = new Apzon.Controls.Editor.TextEditEx();
		this.look_Week = new Apzon.Controls.Editor.LookUpEditEx();
		this.look_DayOfWeek = new Apzon.Controls.Editor.LookUpEditEx();
		this.lab_Of = new Apzon.Controls.Controls.LabelLine();
		this.look_MonthInAnnually = new Apzon.Controls.Editor.LookUpEditEx();
		this.look_MonthOf = new Apzon.Controls.Editor.LookUpEditEx();
		this.radio_SubOption1 = new Apzon.Controls.Editor.CheckEditEx();
		this.radio_SubOption2 = new Apzon.Controls.Editor.CheckEditEx();
		this.date_SeEndDat = new Apzon.Controls.Editor.DateEditEx();
		this.txtCode = new Apzon.Controls.Editor.TextEditEx();
		this.radioGroup_EndType = new Apzon.Controls.Editor.RadioGroupEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.lab_RepeatOn = new Apzon.Controls.Controls.LabelLine();
		this.lab_End = new Apzon.Controls.Controls.LabelLine();
		this.lab_Recurrence = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Time = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.lab_RepeatEvery = new Apzon.Controls.Controls.LabelLine();
		this.lab_Code = new Apzon.Controls.Controls.LabelLine();
		this.time_TimeCycle = new Apzon.Controls.Editor.TimeEditEx();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Recurrence.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_StartCount.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_StartCount.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Occurrence.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Day.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.check_Mon.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.check_Tue.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.check_Wed.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.check_Thu.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.check_Fri.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.check_Sat.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.check_Sun.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DayInMonth.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Week.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_DayOfWeek.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_MonthInAnnually.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_MonthOf.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.radio_SubOption1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.radio_SubOption2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_SeEndDat.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_SeEndDat.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.radioGroup_EndType.Properties).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.time_TimeCycle.Properties).BeginInit();
		base.SuspendLayout();
		this.btn_Add.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Add.EditControl = null;
		this.btn_Add.EditValue = "OK";
		this.btn_Add.FieldName = null;
		this.btn_Add.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Add.ImageOptions.Image");
		this.btn_Add.IsSearchControl = false;
		this.btn_Add.Location = new System.Drawing.Point(3, 389);
		this.btn_Add.Name = "btn_Add";
		this.btn_Add.ReadOnly = false;
		this.btn_Add.SettingInVisible = false;
		this.btn_Add.SettingReadonly = false;
		this.btn_Add.Size = new System.Drawing.Size(75, 23);
		this.btn_Add.TabIndex = 20;
		this.btn_Add.Text = "OK";
		this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(81, 389);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 21;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.txt_Code.EditControl = null;
		this.txt_Code.FieldName = "Code";
		this.txt_Code.IsSearchControl = false;
		this.txt_Code.Location = new System.Drawing.Point(89, 9);
		this.txt_Code.Name = "txt_Code";
		this.txt_Code.SettingInVisible = false;
		this.txt_Code.SettingReadonly = false;
		this.txt_Code.Size = new System.Drawing.Size(100, 20);
		this.txt_Code.TabIndex = 1;
		this.look_Recurrence.EditControl = null;
		this.look_Recurrence.FieldName = null;
		this.look_Recurrence.IsSearchControl = false;
		this.look_Recurrence.Location = new System.Drawing.Point(89, 35);
		this.look_Recurrence.Name = "look_Recurrence";
		this.look_Recurrence.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Recurrence.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.look_Recurrence.Properties.DisplayMember = "Name";
		this.look_Recurrence.Properties.HotTrackItems = false;
		this.look_Recurrence.Properties.NullText = "";
		this.look_Recurrence.Properties.ValueMember = "Code";
		this.look_Recurrence.SettingInVisible = false;
		this.look_Recurrence.SettingReadonly = false;
		this.look_Recurrence.Size = new System.Drawing.Size(100, 20);
		this.look_Recurrence.TabIndex = 2;
		this.look_Recurrence.EditValueChanged += new System.EventHandler(this.look_Recurrence_EditValueChanged);
		this.date_StartCount.EditControl = null;
		this.date_StartCount.EditValue = null;
		this.date_StartCount.FieldName = null;
		this.date_StartCount.Location = new System.Drawing.Point(120, 187);
		this.date_StartCount.Name = "date_StartCount";
		this.date_StartCount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_StartCount.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_StartCount.Properties.IsSearchControl = false;
		this.date_StartCount.SettingInVisible = false;
		this.date_StartCount.SettingReadonly = false;
		this.date_StartCount.Size = new System.Drawing.Size(100, 20);
		this.date_StartCount.TabIndex = 15;
		this.txt_Occurrence.EditControl = null;
		this.txt_Occurrence.FieldName = null;
		this.txt_Occurrence.IsSearchControl = false;
		this.txt_Occurrence.Location = new System.Drawing.Point(229, 281);
		this.txt_Occurrence.Name = "txt_Occurrence";
		this.txt_Occurrence.SettingInVisible = false;
		this.txt_Occurrence.SettingReadonly = false;
		this.txt_Occurrence.Size = new System.Drawing.Size(54, 20);
		this.txt_Occurrence.TabIndex = 18;
		this.txt_Day.EditControl = null;
		this.txt_Day.FieldName = null;
		this.txt_Day.IsSearchControl = false;
		this.txt_Day.Location = new System.Drawing.Point(117, 61);
		this.txt_Day.Name = "txt_Day";
		this.txt_Day.SettingInVisible = false;
		this.txt_Day.SettingReadonly = false;
		this.txt_Day.Size = new System.Drawing.Size(54, 20);
		this.txt_Day.TabIndex = 4;
		this.txt_Day.EditValueChanged += new System.EventHandler(this.txt_Day_EditValueChanged);
		this.lab_Day.AutoSize = true;
		this.lab_Day.EditControl = null;
		this.lab_Day.EditValue = "Days";
		this.lab_Day.FieldName = null;
		this.lab_Day.HideWhenEmpty = false;
		this.lab_Day.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lab_Day.Location = new System.Drawing.Point(177, 68);
		this.lab_Day.Name = "lab_Day";
		this.lab_Day.ReadOnly = false;
		this.lab_Day.SettingInVisible = false;
		this.lab_Day.SettingReadonly = false;
		this.lab_Day.ShowUnderLine = true;
		this.lab_Day.Size = new System.Drawing.Size(526, 18);
		this.lab_Day.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lab_Day.TabIndex = 22;
		this.lab_Day.Utext = "Days";
		this.lab_Occurrence.AutoSize = true;
		this.lab_Occurrence.EditControl = null;
		this.lab_Occurrence.EditValue = "Occurrence";
		this.lab_Occurrence.FieldName = null;
		this.lab_Occurrence.HideWhenEmpty = false;
		this.lab_Occurrence.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lab_Occurrence.Location = new System.Drawing.Point(299, 288);
		this.lab_Occurrence.Name = "lab_Occurrence";
		this.lab_Occurrence.ReadOnly = false;
		this.lab_Occurrence.SettingInVisible = false;
		this.lab_Occurrence.SettingReadonly = false;
		this.lab_Occurrence.ShowUnderLine = true;
		this.lab_Occurrence.Size = new System.Drawing.Size(557, 18);
		this.lab_Occurrence.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lab_Occurrence.TabIndex = 23;
		this.lab_Occurrence.Utext = "Occurrence";
		this.check_Mon.EditControl = null;
		this.check_Mon.FieldName = null;
		this.check_Mon.IsSearchControl = false;
		this.check_Mon.Location = new System.Drawing.Point(89, 89);
		this.check_Mon.Name = "check_Mon";
		this.check_Mon.Properties.Caption = "Mon.";
		this.check_Mon.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.check_Mon.Properties.ValueChecked = "Y";
		this.check_Mon.Properties.ValueGrayed = "N";
		this.check_Mon.Properties.ValueUnchecked = "N";
		this.check_Mon.SettingInVisible = false;
		this.check_Mon.SettingReadonly = false;
		this.check_Mon.Size = new System.Drawing.Size(75, 20);
		this.check_Mon.TabIndex = 26;
		this.check_Tue.EditControl = null;
		this.check_Tue.FieldName = null;
		this.check_Tue.IsSearchControl = false;
		this.check_Tue.Location = new System.Drawing.Point(170, 91);
		this.check_Tue.Name = "check_Tue";
		this.check_Tue.Properties.Caption = "Tue.";
		this.check_Tue.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.check_Tue.Properties.ValueChecked = "Y";
		this.check_Tue.Properties.ValueGrayed = "N";
		this.check_Tue.Properties.ValueUnchecked = "N";
		this.check_Tue.SettingInVisible = false;
		this.check_Tue.SettingReadonly = false;
		this.check_Tue.Size = new System.Drawing.Size(75, 20);
		this.check_Tue.TabIndex = 27;
		this.check_Wed.EditControl = null;
		this.check_Wed.FieldName = null;
		this.check_Wed.IsSearchControl = false;
		this.check_Wed.Location = new System.Drawing.Point(229, 89);
		this.check_Wed.Name = "check_Wed";
		this.check_Wed.Properties.Caption = "Wed.";
		this.check_Wed.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.check_Wed.Properties.ValueChecked = "Y";
		this.check_Wed.Properties.ValueGrayed = "N";
		this.check_Wed.Properties.ValueUnchecked = "N";
		this.check_Wed.SettingInVisible = false;
		this.check_Wed.SettingReadonly = false;
		this.check_Wed.Size = new System.Drawing.Size(79, 20);
		this.check_Wed.TabIndex = 7;
		this.check_Thu.EditControl = null;
		this.check_Thu.FieldName = null;
		this.check_Thu.IsSearchControl = false;
		this.check_Thu.Location = new System.Drawing.Point(302, 88);
		this.check_Thu.Name = "check_Thu";
		this.check_Thu.Properties.Caption = "Thu.";
		this.check_Thu.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.check_Thu.Properties.ValueChecked = "Y";
		this.check_Thu.Properties.ValueGrayed = "N";
		this.check_Thu.Properties.ValueUnchecked = "N";
		this.check_Thu.SettingInVisible = false;
		this.check_Thu.SettingReadonly = false;
		this.check_Thu.Size = new System.Drawing.Size(75, 20);
		this.check_Thu.TabIndex = 8;
		this.check_Fri.EditControl = null;
		this.check_Fri.FieldName = null;
		this.check_Fri.IsSearchControl = false;
		this.check_Fri.Location = new System.Drawing.Point(374, 89);
		this.check_Fri.Name = "check_Fri";
		this.check_Fri.Properties.Caption = "Fri.";
		this.check_Fri.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.check_Fri.Properties.ValueChecked = "Y";
		this.check_Fri.Properties.ValueGrayed = "N";
		this.check_Fri.Properties.ValueUnchecked = "N";
		this.check_Fri.SettingInVisible = false;
		this.check_Fri.SettingReadonly = false;
		this.check_Fri.Size = new System.Drawing.Size(75, 20);
		this.check_Fri.TabIndex = 9;
		this.check_Sat.EditControl = null;
		this.check_Sat.FieldName = null;
		this.check_Sat.IsSearchControl = false;
		this.check_Sat.Location = new System.Drawing.Point(432, 89);
		this.check_Sat.Name = "check_Sat";
		this.check_Sat.Properties.Caption = "Sat.";
		this.check_Sat.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.check_Sat.Properties.ValueChecked = "Y";
		this.check_Sat.Properties.ValueGrayed = "N";
		this.check_Sat.Properties.ValueUnchecked = "N";
		this.check_Sat.SettingInVisible = false;
		this.check_Sat.SettingReadonly = false;
		this.check_Sat.Size = new System.Drawing.Size(75, 20);
		this.check_Sat.TabIndex = 10;
		this.check_Sun.EditControl = null;
		this.check_Sun.FieldName = null;
		this.check_Sun.IsSearchControl = false;
		this.check_Sun.Location = new System.Drawing.Point(513, 89);
		this.check_Sun.Name = "check_Sun";
		this.check_Sun.Properties.Caption = "Sun.";
		this.check_Sun.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.check_Sun.Properties.ValueChecked = "Y";
		this.check_Sun.Properties.ValueGrayed = "N";
		this.check_Sun.Properties.ValueUnchecked = "N";
		this.check_Sun.SettingInVisible = false;
		this.check_Sun.SettingReadonly = false;
		this.check_Sun.Size = new System.Drawing.Size(75, 20);
		this.check_Sun.TabIndex = 11;
		this.check_Sun.CheckedChanged += new System.EventHandler(this.check_Sun_CheckedChanged);
		this.txt_DayInMonth.EditControl = null;
		this.txt_DayInMonth.FieldName = null;
		this.txt_DayInMonth.IsSearchControl = false;
		this.txt_DayInMonth.Location = new System.Drawing.Point(117, 87);
		this.txt_DayInMonth.Name = "txt_DayInMonth";
		this.txt_DayInMonth.SettingInVisible = false;
		this.txt_DayInMonth.SettingReadonly = false;
		this.txt_DayInMonth.Size = new System.Drawing.Size(54, 20);
		this.txt_DayInMonth.TabIndex = 33;
		this.txt_DayInMonth.EditValueChanged += new System.EventHandler(this.txt_DayInMonth_EditValueChanged);
		this.look_Week.EditControl = null;
		this.look_Week.FieldName = null;
		this.look_Week.IsSearchControl = false;
		this.look_Week.Location = new System.Drawing.Point(120, 112);
		this.look_Week.Name = "look_Week";
		this.look_Week.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Week.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.look_Week.Properties.DisplayMember = "Name";
		this.look_Week.Properties.NullText = "";
		this.look_Week.Properties.ValueMember = "Code";
		this.look_Week.SettingInVisible = false;
		this.look_Week.SettingReadonly = false;
		this.look_Week.Size = new System.Drawing.Size(100, 20);
		this.look_Week.TabIndex = 12;
		this.look_Week.EditValueChanged += new System.EventHandler(this.look_Week_EditValueChanged);
		this.look_DayOfWeek.EditControl = null;
		this.look_DayOfWeek.FieldName = null;
		this.look_DayOfWeek.IsSearchControl = false;
		this.look_DayOfWeek.Location = new System.Drawing.Point(229, 112);
		this.look_DayOfWeek.Name = "look_DayOfWeek";
		this.look_DayOfWeek.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_DayOfWeek.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.look_DayOfWeek.Properties.DisplayMember = "Name";
		this.look_DayOfWeek.Properties.NullText = "";
		this.look_DayOfWeek.Properties.ValueMember = "Code";
		this.look_DayOfWeek.SettingInVisible = false;
		this.look_DayOfWeek.SettingReadonly = false;
		this.look_DayOfWeek.Size = new System.Drawing.Size(100, 20);
		this.look_DayOfWeek.TabIndex = 13;
		this.look_DayOfWeek.EditValueChanged += new System.EventHandler(this.look_DayOfWeek_EditValueChanged);
		this.lab_Of.AutoSize = true;
		this.lab_Of.EditControl = null;
		this.lab_Of.EditValue = "Of";
		this.lab_Of.FieldName = null;
		this.lab_Of.HideWhenEmpty = false;
		this.lab_Of.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lab_Of.Location = new System.Drawing.Point(347, 119);
		this.lab_Of.Name = "lab_Of";
		this.lab_Of.ReadOnly = false;
		this.lab_Of.SettingInVisible = false;
		this.lab_Of.SettingReadonly = false;
		this.lab_Of.ShowUnderLine = true;
		this.lab_Of.Size = new System.Drawing.Size(514, 18);
		this.lab_Of.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lab_Of.TabIndex = 38;
		this.lab_Of.Utext = "Of";
		this.look_MonthInAnnually.EditControl = null;
		this.look_MonthInAnnually.FieldName = null;
		this.look_MonthInAnnually.IsSearchControl = false;
		this.look_MonthInAnnually.Location = new System.Drawing.Point(117, 86);
		this.look_MonthInAnnually.Name = "look_MonthInAnnually";
		this.look_MonthInAnnually.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_MonthInAnnually.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.look_MonthInAnnually.Properties.DisplayMember = "Name";
		this.look_MonthInAnnually.Properties.NullText = "";
		this.look_MonthInAnnually.Properties.ValueMember = "Code";
		this.look_MonthInAnnually.SettingInVisible = false;
		this.look_MonthInAnnually.SettingReadonly = false;
		this.look_MonthInAnnually.Size = new System.Drawing.Size(100, 20);
		this.look_MonthInAnnually.TabIndex = 6;
		this.look_MonthInAnnually.EditValueChanged += new System.EventHandler(this.look_MonthInAnnually_EditValueChanged);
		this.look_MonthOf.EditControl = null;
		this.look_MonthOf.FieldName = null;
		this.look_MonthOf.IsSearchControl = false;
		this.look_MonthOf.Location = new System.Drawing.Point(383, 112);
		this.look_MonthOf.Name = "look_MonthOf";
		this.look_MonthOf.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_MonthOf.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.look_MonthOf.Properties.DisplayMember = "Name";
		this.look_MonthOf.Properties.NullText = "";
		this.look_MonthOf.Properties.ValueMember = "Code";
		this.look_MonthOf.SettingInVisible = false;
		this.look_MonthOf.SettingReadonly = false;
		this.look_MonthOf.Size = new System.Drawing.Size(100, 20);
		this.look_MonthOf.TabIndex = 14;
		this.look_MonthOf.EditValueChanged += new System.EventHandler(this.look_MonthOf_EditValueChanged);
		this.radio_SubOption1.EditControl = null;
		this.radio_SubOption1.FieldName = null;
		this.radio_SubOption1.IsSearchControl = false;
		this.radio_SubOption1.Location = new System.Drawing.Point(88, 66);
		this.radio_SubOption1.Name = "radio_SubOption1";
		this.radio_SubOption1.Properties.Caption = "";
		this.radio_SubOption1.SettingInVisible = false;
		this.radio_SubOption1.SettingReadonly = false;
		this.radio_SubOption1.Size = new System.Drawing.Size(23, 20);
		this.radio_SubOption1.TabIndex = 3;
		this.radio_SubOption1.CheckedChanged += new System.EventHandler(this.radio_SubOption1_CheckedChanged);
		this.radio_SubOption2.EditControl = null;
		this.radio_SubOption2.FieldName = null;
		this.radio_SubOption2.IsSearchControl = false;
		this.radio_SubOption2.Location = new System.Drawing.Point(88, 89);
		this.radio_SubOption2.Name = "radio_SubOption2";
		this.radio_SubOption2.Properties.Caption = "Every_Weekday";
		this.radio_SubOption2.SettingInVisible = false;
		this.radio_SubOption2.SettingReadonly = false;
		this.radio_SubOption2.Size = new System.Drawing.Size(104, 20);
		this.radio_SubOption2.TabIndex = 5;
		this.radio_SubOption2.CheckedChanged += new System.EventHandler(this.radio_SubOption2_CheckedChanged);
		this.date_SeEndDat.EditControl = null;
		this.date_SeEndDat.EditValue = null;
		this.date_SeEndDat.FieldName = null;
		this.date_SeEndDat.Location = new System.Drawing.Point(229, 307);
		this.date_SeEndDat.Name = "date_SeEndDat";
		this.date_SeEndDat.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_SeEndDat.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_SeEndDat.Properties.IsSearchControl = false;
		this.date_SeEndDat.SettingInVisible = false;
		this.date_SeEndDat.SettingReadonly = false;
		this.date_SeEndDat.Size = new System.Drawing.Size(100, 20);
		this.date_SeEndDat.TabIndex = 19;
		this.txtCode.EditControl = null;
		this.txtCode.FieldName = null;
		this.txtCode.IsSearchControl = false;
		this.txtCode.Location = new System.Drawing.Point(358, 214);
		this.txtCode.Name = "txtCode";
		this.txtCode.SettingInVisible = false;
		this.txtCode.SettingReadonly = false;
		this.txtCode.Size = new System.Drawing.Size(100, 20);
		this.txtCode.TabIndex = 48;
		this.txtCode.Visible = false;
		this.radioGroup_EndType.EditControl = null;
		this.radioGroup_EndType.FieldName = null;
		this.radioGroup_EndType.IsSearchControl = false;
		this.radioGroup_EndType.Location = new System.Drawing.Point(120, 245);
		this.radioGroup_EndType.Name = "radioGroup_EndType";
		this.radioGroup_EndType.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[3]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem(0, "None end date"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(1, "After"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(2, "By")
		});
		this.radioGroup_EndType.SettingInVisible = false;
		this.radioGroup_EndType.SettingReadonly = false;
		this.radioGroup_EndType.Size = new System.Drawing.Size(100, 96);
		this.radioGroup_EndType.TabIndex = 17;
		this.radioGroup_EndType.SelectedIndexChanged += new System.EventHandler(this.radioGroup_EndType_SelectedIndexChanged);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.lab_RepeatOn.EditControl = null;
		this.lab_RepeatOn.EditValue = "RepeatOn";
		this.lab_RepeatOn.FieldName = "";
		this.lab_RepeatOn.HideWhenEmpty = false;
		this.lab_RepeatOn.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lab_RepeatOn.Location = new System.Drawing.Point(22, 90);
		this.lab_RepeatOn.Name = "lab_RepeatOn";
		this.lab_RepeatOn.ReadOnly = false;
		this.lab_RepeatOn.SettingInVisible = false;
		this.lab_RepeatOn.SettingReadonly = false;
		this.lab_RepeatOn.ShowUnderLine = true;
		this.lab_RepeatOn.Size = new System.Drawing.Size(61, 20);
		this.lab_RepeatOn.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lab_RepeatOn.TabIndex = 36;
		this.lab_RepeatOn.Utext = "RepeatOn";
		this.lab_End.EditControl = null;
		this.lab_End.EditValue = "End";
		this.lab_End.FieldName = "";
		this.lab_End.HideWhenEmpty = false;
		this.lab_End.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lab_End.Location = new System.Drawing.Point(12, 245);
		this.lab_End.Name = "lab_End";
		this.lab_End.ReadOnly = false;
		this.lab_End.SettingInVisible = false;
		this.lab_End.SettingReadonly = false;
		this.lab_End.ShowUnderLine = true;
		this.lab_End.Size = new System.Drawing.Size(33, 20);
		this.lab_End.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lab_End.TabIndex = 16;
		this.lab_End.Utext = "End";
		this.lab_Recurrence.EditControl = null;
		this.lab_Recurrence.EditValue = "Recurrence";
		this.lab_Recurrence.FieldName = "";
		this.lab_Recurrence.HideWhenEmpty = false;
		this.lab_Recurrence.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lab_Recurrence.Location = new System.Drawing.Point(12, 38);
		this.lab_Recurrence.Name = "lab_Recurrence";
		this.lab_Recurrence.ReadOnly = false;
		this.lab_Recurrence.SettingInVisible = false;
		this.lab_Recurrence.SettingReadonly = false;
		this.lab_Recurrence.ShowUnderLine = true;
		this.lab_Recurrence.Size = new System.Drawing.Size(85, 20);
		this.lab_Recurrence.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lab_Recurrence.TabIndex = 5;
		this.lab_Recurrence.Utext = "Recurrence";
		this.lbl_Time.EditControl = null;
		this.lbl_Time.EditValue = "Time";
		this.lbl_Time.FieldName = "";
		this.lbl_Time.HideWhenEmpty = false;
		this.lbl_Time.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Time.Location = new System.Drawing.Point(12, 222);
		this.lbl_Time.Name = "lbl_Time";
		this.lbl_Time.ReadOnly = false;
		this.lbl_Time.SettingInVisible = false;
		this.lbl_Time.SettingReadonly = false;
		this.lbl_Time.ShowUnderLine = true;
		this.lbl_Time.Size = new System.Drawing.Size(112, 20);
		this.lbl_Time.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Time.TabIndex = 4;
		this.lbl_Time.Utext = "Time";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Start count date";
		this.labelLine4.FieldName = "";
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(12, 190);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(112, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 3;
		this.labelLine4.Utext = "Start count date";
		this.lab_RepeatEvery.EditControl = null;
		this.lab_RepeatEvery.EditValue = "RepeatEvery";
		this.lab_RepeatEvery.FieldName = "";
		this.lab_RepeatEvery.HideWhenEmpty = false;
		this.lab_RepeatEvery.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lab_RepeatEvery.Location = new System.Drawing.Point(12, 64);
		this.lab_RepeatEvery.Name = "lab_RepeatEvery";
		this.lab_RepeatEvery.ReadOnly = false;
		this.lab_RepeatEvery.SettingInVisible = false;
		this.lab_RepeatEvery.SettingReadonly = false;
		this.lab_RepeatEvery.ShowUnderLine = true;
		this.lab_RepeatEvery.Size = new System.Drawing.Size(71, 20);
		this.lab_RepeatEvery.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lab_RepeatEvery.TabIndex = 2;
		this.lab_RepeatEvery.Utext = "RepeatEvery";
		this.lab_Code.EditControl = null;
		this.lab_Code.EditValue = "Cycle code";
		this.lab_Code.FieldName = "";
		this.lab_Code.HideWhenEmpty = false;
		this.lab_Code.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lab_Code.Location = new System.Drawing.Point(12, 12);
		this.lab_Code.Name = "lab_Code";
		this.lab_Code.ReadOnly = false;
		this.lab_Code.SettingInVisible = false;
		this.lab_Code.SettingReadonly = false;
		this.lab_Code.ShowUnderLine = true;
		this.lab_Code.Size = new System.Drawing.Size(85, 20);
		this.lab_Code.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lab_Code.TabIndex = 0;
		this.lab_Code.Utext = "Cycle code";
		this.time_TimeCycle.EditControl = null;
		this.time_TimeCycle.FieldName = null;
		this.time_TimeCycle.IsSearchControl = false;
		this.time_TimeCycle.Location = new System.Drawing.Point(120, 219);
		this.time_TimeCycle.Name = "time_TimeCycle";
		this.time_TimeCycle.Properties.DisplayFormat.FormatString = "##:##";
		this.time_TimeCycle.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.time_TimeCycle.Properties.EditFormat.FormatString = "##:##";
		this.time_TimeCycle.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.time_TimeCycle.Properties.Mask.EditMask = "(2[0-3]|[01]?[0-9]):([0-5]{1}[0-9])";
		this.time_TimeCycle.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
		this.time_TimeCycle.SettingInVisible = false;
		this.time_TimeCycle.SettingReadonly = false;
		this.time_TimeCycle.Size = new System.Drawing.Size(100, 20);
		this.time_TimeCycle.TabIndex = 49;
		base.AcceptButton = this.btn_Add;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(811, 415);
		this.ContextMenuStrip = this.contextMenuStrip1;
		base.Controls.Add(this.time_TimeCycle);
		base.Controls.Add(this.radioGroup_EndType);
		base.Controls.Add(this.txtCode);
		base.Controls.Add(this.date_SeEndDat);
		base.Controls.Add(this.radio_SubOption2);
		base.Controls.Add(this.radio_SubOption1);
		base.Controls.Add(this.look_MonthOf);
		base.Controls.Add(this.look_MonthInAnnually);
		base.Controls.Add(this.lab_Of);
		base.Controls.Add(this.lab_RepeatOn);
		base.Controls.Add(this.look_DayOfWeek);
		base.Controls.Add(this.look_Week);
		base.Controls.Add(this.txt_DayInMonth);
		base.Controls.Add(this.check_Sun);
		base.Controls.Add(this.check_Sat);
		base.Controls.Add(this.check_Fri);
		base.Controls.Add(this.check_Thu);
		base.Controls.Add(this.check_Wed);
		base.Controls.Add(this.check_Tue);
		base.Controls.Add(this.check_Mon);
		base.Controls.Add(this.lab_Occurrence);
		base.Controls.Add(this.lab_Day);
		base.Controls.Add(this.txt_Day);
		base.Controls.Add(this.txt_Occurrence);
		base.Controls.Add(this.lab_End);
		base.Controls.Add(this.date_StartCount);
		base.Controls.Add(this.look_Recurrence);
		base.Controls.Add(this.txt_Code);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Add);
		base.Controls.Add(this.lab_Recurrence);
		base.Controls.Add(this.lbl_Time);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.lab_RepeatEvery);
		base.Controls.Add(this.lab_Code);
		base.FormId = "oInventoryCycles";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_InventoryCycles";
		this.Text = "Inventory Cycles - Setup";
		base.OnControlValueChanged += new Apzon.Controls.Form.ControlValueChanged(this.frm_InventoryCycles_OnControlValueChanged);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_InventoryCycles_FormClosing);
		base.Load += new System.EventHandler(this.frm_InventoryCycles_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Recurrence.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_StartCount.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_StartCount.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Occurrence.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Day.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.check_Mon.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.check_Tue.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.check_Wed.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.check_Thu.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.check_Fri.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.check_Sat.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.check_Sun.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DayInMonth.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Week.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_DayOfWeek.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_MonthInAnnually.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_MonthOf.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.radio_SubOption1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.radio_SubOption2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_SeEndDat.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_SeEndDat.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.radioGroup_EndType.Properties).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.time_TimeCycle.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
