using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
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
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Production;
using Apzon.Forms.Administrator;
using Apzon.Forms.BusinessMaster;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Inventory.InventoryTransaction;
using Apzon.Forms.ItemsMaster;
using Apzon.Forms.Payment;
using Apzon.Forms.Production;
using Apzon.Forms.Purchase;
using Apzon.Forms.Sales;
using Apzon.Forms.SalesOppotunity.SetUp.Activity;
using Apzon.Forms.Service;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Win;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraTab;

namespace Apzon.Forms.SalesOppotunity;

public class frm_ActivitiesForOpportunity : XtraFormEx
{
	private List<Common.Table_Code> _listContactPerson = new List<Common.Table_Code>();

	private string _beginTime;

	private string _endTime;

	private string _phone;

	private string _privilege = "";

	private string lastFocusLeave;

	private int _newClgCode;

	private int _upClgCode;

	private int _valueDocType;

	private int _currentFileEntry;

	private int weekOfMonth;

	private int day;

	private int dayOfMonth;

	private int month;

	private DateTime _seStartDat;

	private bool _activebtFollowUp;

	private bool _TimeDurationTextChanging;

	private bool _TimeTextChanged;

	private bool _FormLoading;

	private bool _isLoadingData;

	private bool _isSettingCbAttendUser;

	private bool _isChangedValueInModeAdd;

	private bool _isSettingCbDocType;

	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public string _linkCardCode = string.Empty;

	private DataTable dtObject = new DataTable();

	private DataTable dt;

	private DataTable dtACT1 = new DataTable();

	public static DataTable dtDoc = new DataTable();

	private DataTable dtDuplicate = new DataTable();

	public DataTable dtCopy = new DataTable();

	public string _cardCodefrm_SalesOpportunity;

	public bool isLoadFromOpportunity;

	private List<Common.Table_Code> _listStatus = new List<Common.Table_Code>();

	public int _clgCodeFrmActivity;

	public int _clgCode_FromOppotunity = -1;

	public int _linkeDocType = -1;

	public int _linkDocNum = -1;

	public frm_ActivityCalendar frmCalendar;

	public DateTime CalStartDate;

	public string CalStartTime;

	public DateTime CalEndDate;

	public string CalEndTime;

	public string CalUserId;

	public string CalDuration;

	public string CalDurType;

	public Frm_ServiceCall frmSCall;

	private IContainer components;

	private BarDockControl barDockControlRight;

	private GridColumnEx gcAllTotal;

	private GridColumnEx gcAllVatSum;

	private ToolStripMenuItem addNewToolStripMenuItem_Document;

	private ToolStripMenuItem cmtDuplicate_Return;

	private ToolStripMenuItem cmtClose_Doc;

	private ToolStripMenuItem ctmCancel_Doc;

	private ToolStripMenuItem saveAsDraftToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem_Documents;

	private GroupControl groupControl1;

	private TextBoxEx txtNo;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private BarDockControl barDockControl1;

	private XtraTabPageEx xtraTabPage2;

	private GridColumnEx gcDocAllTotal;

	private GridColumnEx gcDocAllVat;

	private Panel panel1;

	private GroupControl groupControl2;

	private LabelLine labelLine4;

	private LabelLine labelLine6;

	private LabelLine labelLine7;

	private LabelLine labelLine8;

	private LabelLine labelLine9;

	private GridColumnEx gcCusName;

	private BarButtonItem barButtonItem1;

	private BarButtonItem barButtonItem2;

	private BarButtonItem barButtonItem5;

	private DropDownButton drpBtCoppyTo;

	private BarDockControl barDockControl2;

	private XtraTabPageEx xtraTabPage3;

	private LabelLine labelLine3;

	private LabelLine labelLine5;

	private LabelLine lb_TelephoneNo;

	private LabelLine lb_AssignedBy;

	private LabelLine lb_ContactPerson;

	private LabelLine lb_AssignedTo;

	private LabelLine lb_BPName;

	private LabelLine lb_Subject;

	private LabelLine lb_BPCode;

	private LabelLine lb_Type;

	private LabelLine lb_Number;

	private LabelLine lb_Activity;

	private XtraTabPageEx tap_General;

	private Panel panel_Daily;

	private RadioButton radio_SubOption2;

	private Label label_RepeatEvery;

	private RadioButton radio_SubOption1;

	private LabelLine lb_Of;

	private LabelLine lb_RepeatOn;

	private LabelLine lb_RepeatEvery;

	private RadioButton radio_SubOption;

	private LabelLine lb_Recurrence;

	private LabelLine label_MettingLocation;

	private LabelLine label_Priority;

	private LabelLine lb_Duration;

	private LabelLine lb_EndTime;

	private LabelLine lb_StartTime;

	private LabelLine lb_Remarks;

	private XtraTabPageEx tab_Content;

	private XtraTabPageEx tap_LinkedDocument;

	private LabelLine lb_SourceObjectNo;

	private LabelLine lb_SourceObjectType;

	private LabelLine lb_PreviousActivity;

	private LabelLine lb_DocumentNumber;

	private LabelLine lb_DocumentType;

	private SeparatorComboBox cbDocType;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private ToolStripMenuItem relayActiviToolStripMenuItem;

	private LabelLine lb_Status;

	private LabelLine lb_Country;

	private LabelLine lb_Room;

	private LabelLine lb_State;

	private LabelLine lb_City;

	private LabelLine lb_Street;

	private LabelLine lb_Address;

	private SimpleButtonEx btSearch;

	private SimpleButtonEx simpleButton1;

	private SimpleButtonEx btAdd;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btFollowUp;

	private Panel panel_Range;

	private LabelLine lb_End;

	private LabelLine lb_Start;

	private LabelControl lb_Occurrence;

	private RadioButton radio_By;

	private RadioButton radio_After;

	private RadioButton radio_NoEndDate;

	private LabelControl lb_Range;

	private LabelLine labelLine32;

	private XtraTabPageEx tap_Att;

	private ucAttachment ucAttachment1;

	private LookUpEditCombo luk_Subject;

	private LookUpEditCombo luk_Type;

	private LookUpEditCombo luk_Meetting;

	private LookUpEditCombo cbRecurPat;

	private LookUpEditCombo cbStatus;

	private LookUpEditCombo cbActivity;

	private LookUpEditCombo txtState;

	private LookUpEditCombo txtCountry;

	private LookUpEditCombo cbAttendUser;

	private ButtonEditEx txtName;

	private ComboBoxEditEx cbRequester;

	private DateEditEx fDate;

	private DateEditEx tDate;

	private TextEditEx textEdit1;

	private DateEditEx dateEdit1;

	private ComboBoxEditEx comboBoxEdit1;

	private DateEditEx dateEdit2;

	private ButtonEditEx txtCardCode;

	private ButtonEditEx buttonEdit1;

	private CheckEditEx chkPerson;

	private ButtonEditEx txtBpCode;

	private XtraTabControlEx Xtap;

	private TextEditEx txtRemQty;

	private CheckEditEx chkInactive;

	private CheckEditEx chkClosed;

	private CheckEditEx chkReminder;

	private CheckEditEx chkSun;

	private CheckEditEx chkSat;

	private CheckEditEx chkFri;

	private CheckEditEx chkThu;

	private CheckEditEx chkWed;

	private CheckEditEx chkTue;

	private CheckEditEx chkMon;

	private ComboBoxEditEx cbMonth;

	private ComboBoxEditEx cbMonth1;

	private ComboBoxEditEx cbDayOfWeek;

	private ComboBoxEditEx cbWeek;

	private TextEditEx txtDayInMonth;

	private TextEditEx txtInterval;

	private ComboBoxEditEx cbPriority;

	private TextEditEx txtDuration;

	private TextEditEx txtEndTime;

	private TextEditEx txtBeginTime;

	private DateEditEx dEnDate;

	private DateEditEx dStart_Recontact;

	private MemoEditEx memoEdit_Note;

	private CheckEditEx chkShowDocumentRelatedBP;

	private CheckEditEx chkLinkDraft;

	private TextEditEx txtAssignedBy;

	private TextEditEx txtTelePhone;

	private ComboBoxEditEx cbDocNum;

	private ComboBoxEditEx cb_ContactPerson;

	private CheckEditEx chk_Tentative;

	private TextEditEx txtBpName;

	private TextEditEx txtBpType;

	private TextEditEx txtNumber;

	private ComboBoxEditEx cbUser;

	private TextEditEx txtDetails;

	private TextEditEx txtStreet;

	private TextEditEx txtRoom;

	private TextEditEx txtCity;

	private TextEditEx txtParentType;

	private TextEditEx txtParentId;

	private TextEditEx txtPrevActivity;

	private TextEditEx txtAfterMaxOccur;

	private DateEditEx txtBySeEndDat;

	private DateEditEx dSeStartDat;

	public DateTime dateChange { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btAdd.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btAdd.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btAdd.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btAdd.Text = Resources.ButtonOK;
			break;
		case Enums.FormViewModeType.Find:
			btAdd.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_ActivitiesForOpportunity()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ActivitiesForOpportunity"))
		{
			InitializeComponent();
			SettingLanguage();
			WinFuntions.SetFormatControlValue(this);
			cbDocType.SeparatorColor = Color.Black;
			cbDocType.SeparatorWidth = 1;
			cbDocType.AutoAdjustItemHeight = true;
			cbDocType.Items.Clear();
			cbDocType.AddString(string.Empty);
			cbDocType.AddString("Sales Quotations");
			cbDocType.AddString("Sales Orders");
			cbDocType.AddString("Deliveries");
			cbDocType.AddString("Returns");
			cbDocType.AddString("A/R Invoices");
			cbDocType.AddString("A/R Credit Memos");
			cbDocType.AddString("A/R Down Payment");
			cbDocType.SetSeparator(8);
			cbDocType.AddString(string.Empty);
			cbDocType.AddString("Purchase Request");
			cbDocType.AddString("Purchase Quotation");
			cbDocType.AddString("Purchase Orders");
			cbDocType.AddString("Goods Receipt PO");
			cbDocType.AddString("Goods Return");
			cbDocType.AddString("A/P Invoices");
			cbDocType.AddString("A/P Credit Memos");
			cbDocType.AddString("Landed Costs");
			cbDocType.AddString("A/P Down Payment");
			cbDocType.SetSeparator(18);
			cbDocType.AddString(string.Empty);
			cbDocType.AddString("Incoming Payments");
			cbDocType.AddString("Outgoing Payments");
			cbDocType.SetSeparator(21);
			cbDocType.AddString(string.Empty);
			cbDocType.AddString("Inventory Transfer Request");
			cbDocType.AddString("Inventory Transfer");
			cbDocType.AddString("Goods Issue");
			cbDocType.AddString("Goods Receipt");
			cbDocType.AddString("Inventory Revaluation");
			cbDocType.AddString("Production Orders");
			cbDocType.AddString("Items");
			cbDocType.SetSeparator(29);
			cbDocType.AddString(string.Empty);
			cbDocType.AddString("Blanket Agreements");
			_isSettingCbDocType = true;
			cbDocType.SelectedIndex = 0;
			_isSettingCbDocType = false;
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ActivitiesForOpportunity" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
				_privilege = "R";
			}
			else
			{
				_privilege = "F";
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void SettingLanguage()
	{
		lb_Activity.Utext = Resources.OCLG_Action;
		lb_Type.Utext = Resources.OCLG_CntctType;
		lb_Subject.Utext = Resources.OCLG_CntctSbjct;
		lb_AssignedTo.Utext = Resources.lb_AssignedTo;
		lb_AssignedBy.Utext = Resources.lb_AssignedBy;
		lb_Number.Utext = Resources.OCLG_ClgCode;
		lb_BPCode.Utext = Resources.lb_BPCode;
		lb_BPName.Utext = Resources.lb_BPName;
		lb_ContactPerson.Utext = Resources.lb_CntctCode;
		lb_TelephoneNo.Utext = Resources.lb_TelephoneNo;
		chkPerson.Text = Resources.OCLG_personal;
		lb_Remarks.Utext = Resources.OCLG_Details;
		lb_StartTime.Utext = Resources.OCLG_BeginTime;
		lb_EndTime.Utext = Resources.lb_EndTime;
		lb_Duration.Utext = Resources.OCLG_Duration;
		lb_Status.Utext = Resources.OCLG_status;
		lb_Address.Utext = Resources.lb_Address;
		lb_Street.Utext = Resources.lb_Street;
		lb_City.Utext = Resources.lb_City;
		lb_Room.Utext = Resources.lb_Room;
		lb_State.Utext = Resources.lb_State;
		lb_Country.Utext = Resources.lb_Country;
		lb_Recurrence.Utext = Resources.lb_Recurrence;
		lb_RepeatEvery.Utext = Resources.lb_RepeatEvery;
		lb_RepeatOn.Utext = Resources.lb_RepeatOn;
		chkMon.Text = Resources.lb_Mon;
		chkTue.Text = Resources.lb_Tue;
		chkWed.Text = Resources.lb_Wed;
		chkThu.Text = Resources.lb_Thu;
		chkFri.Text = Resources.lb_Fri;
		chkSat.Text = Resources.lb_Sat;
		chkSun.Text = Resources.lb_Sun;
		lb_Of.Utext = Resources.lb_Of;
		lb_Range.Text = Resources.lb_Range;
		lb_Start.Utext = Resources.lb_Start;
		lb_End.Utext = Resources.lb_End;
		radio_NoEndDate.Text = Resources.lb_NoEndDate;
		radio_After.Text = Resources.lb_After;
		radio_By.Text = Resources.lb_by;
		lb_Occurrence.Text = Resources.lb_Occurrence;
		chkInactive.Text = Resources.lb_Inactive;
		chk_Tentative.Text = Resources.lb_Tentative;
		chkClosed.Text = Resources.lb_Closed;
		chkReminder.Text = Resources.lb_Reminder;
		chkLinkDraft.Text = Resources.lb_LinkDraft;
		lb_DocumentType.Utext = Resources.lb_DocumentType;
		lb_DocumentNumber.Utext = Resources.lb_DocumentNumber;
		lb_SourceObjectType.Utext = Resources.lb_SourceObjectType;
		chkShowDocumentRelatedBP.Text = Resources.lb_ShowDocumentsRelatedtotheBP;
		lb_PreviousActivity.Utext = Resources.lb_PreviousActivity;
		btCancel.Text = Resources.ButtonCancel;
		FormMode = Enums.FormViewModeType.Add;
	}

	private async void frm_ActivitiesForOpportunity_Load(object sender, EventArgs e)
	{
		_ = 9;
		try
		{
			_FormLoading = true;
			string dateTimeFormat = GlobalData.DateTimeFormat;
			dStart_Recontact.Properties.DisplayFormat.FormatType = FormatType.DateTime;
			dStart_Recontact.Properties.DisplayFormat.FormatString = dateTimeFormat;
			dEnDate.Properties.DisplayFormat.FormatType = FormatType.DateTime;
			dEnDate.Properties.DisplayFormat.FormatString = dateTimeFormat;
			dSeStartDat.Properties.DisplayFormat.FormatType = FormatType.DateTime;
			dSeStartDat.Properties.DisplayFormat.FormatString = dateTimeFormat;
			txtBySeEndDat.Properties.DisplayFormat.FormatType = FormatType.DateTime;
			txtBySeEndDat.Properties.DisplayFormat.FormatString = dateTimeFormat;
			txtNumber.ReadOnly = true;
			SetLookUpDataSource();
			await LoadLookupDatasource();
			await LoadData();
			if (isLoadFromOpportunity)
			{
				await ChooseCarCode();
			}
			if (_clgCode_FromOppotunity != -1)
			{
				await GetTableDefault();
				FormMode = Enums.FormViewModeType.Find;
				dt = GetSearchDataTable();
				DataRow dataRow = dt.NewRow();
				dataRow["ClgCode"] = _clgCode_FromOppotunity;
				dt.Rows.Add(dataRow);
				await Search(dt);
				await ucAttachment1.LoadAttachments(_clgCode_FromOppotunity);
			}
			if (_activebtFollowUp)
			{
				FormMode = Enums.FormViewModeType.Find;
				await GetTableDefault();
				dt = GetSearchDataTable();
				DataRow dataRow2 = dt.NewRow();
				dataRow2["ClgCode"] = _clgCodeFrmActivity;
				dt.Rows.Add(dataRow2);
				await Search(dt);
			}
			if (_linkDocNum > 0 && _linkeDocType > 0)
			{
				cbDocType.SelectedItem = GetDocNameByType(_linkeDocType);
				cbDocNum.Text = _linkDocNum.ToString();
				cbDocNum.Properties.Buttons[1].Visible = true;
			}
			if (!string.IsNullOrEmpty(_linkCardCode))
			{
				txtBpCode.EditValue = _linkCardCode;
				BusinessPartnersImpl buss = new BusinessPartnersImpl();
				DataTable dataTable = await ExtenseionMethods.GetBPByCardCode(_linkCardCode);
				buss.CardCode = _linkCardCode;
				buss.CardType = dataTable.Rows[0]["CardType"].ToString();
				buss.CardName = dataTable.Rows[0]["CardName"].ToString();
				await SetBPData(buss);
			}
			_FormLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void dStart_Recontact_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (_TimeDurationTextChanging)
			{
				return;
			}
			_TimeDurationTextChanging = true;
			if (FormMode != Enums.FormViewModeType.Find)
			{
				dateChange = Function.ParseDateTime(dStart_Recontact.EditValue) ?? DateTime.MinValue;
				dEnDate.EditValue = dateChange;
				dSeStartDat.EditValue = dateChange;
				chkMon.Checked = false;
				chkTue.Checked = false;
				chkWed.Checked = false;
				chkThu.Checked = false;
				chkFri.Checked = false;
				chkSat.Checked = false;
				chkSun.Checked = false;
				GetDay(dateChange);
				GetDayOfMonth(dateChange);
				GetMonth(dateChange);
				GetWeekOfMonth(dateChange);
				DateTime dateTime = dateChange;
				string source = txtDuration.Text;
				decimal? num = 0m;
				string obj = string.Join("", source.Where((char c) => char.IsDigit(c) || c == '.'));
				string text = string.Join("", source.Where((char c) => char.IsLetter(c)));
				num = Function.ParseDecimal(obj);
				if (text == "Days")
				{
					decimal? num2 = num;
					decimal num3 = 1;
					if ((num2.GetValueOrDefault() >= num3) & num2.HasValue)
					{
						dateTime = dateTime.AddDays((int)Math.Round(num.Value));
					}
				}
				dEnDate.DateTime = dateTime;
				_TimeTextChanged = false;
				_TimeDurationTextChanging = false;
			}
			SetUpdateButton(sender);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			_TimeDurationTextChanging = false;
		}
	}

	private void dEnDate_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_TimeDurationTextChanging)
			{
				_TimeDurationTextChanging = true;
				TimeSpan dateTimeDiff = GetDateTimeDiff();
				if (dateTimeDiff.TotalDays >= 1.0)
				{
					string text = Math.Round(dateTimeDiff.TotalDays, 5) + " Days";
					txtDuration.Text = text;
				}
				else if (dateTimeDiff.TotalHours >= 1.0)
				{
					string text = Math.Round(dateTimeDiff.TotalHours, 2) + " Hours";
					txtDuration.Text = text;
				}
				else if (dateTimeDiff.TotalMinutes > 0.0)
				{
					string text = Math.Round(dateTimeDiff.TotalMinutes) + " Minutes";
					txtDuration.Text = text;
				}
				_TimeTextChanged = false;
				_TimeDurationTextChanging = false;
				SetUpdateButton(sender);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			_TimeDurationTextChanging = false;
		}
	}

	private void dEnDate_Leave(object sender, EventArgs e)
	{
		try
		{
			if (!_TimeDurationTextChanging)
			{
				_TimeDurationTextChanging = true;
				if (GetDateTimeDiff().TotalDays < 0.0)
				{
					WinCommons.Error(Enums.MessageCode.StartDateShouldNotGreaterThanEndDate, "");
					dEnDate.Focus();
				}
				_TimeDurationTextChanging = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			_TimeDurationTextChanging = false;
		}
	}

	private void txtBeginTime_Leave(object sender, EventArgs e)
	{
		try
		{
			if (!_TimeDurationTextChanging && _TimeTextChanged)
			{
				_TimeDurationTextChanging = true;
				_TimeTextChanged = false;
				SetDurationTimeTextFormat(txtBeginTime);
				DateTime dateTime = Convert.ToDateTime(dStart_Recontact.EditValue);
				TimeSpan timeSpanTimeTextBox = GetTimeSpanTimeTextBox(txtBeginTime.Text);
				DateTime dateTime2 = dateTime.AddHours(timeSpanTimeTextBox.Hours).AddMinutes(timeSpanTimeTextBox.Minutes).AddDays(GetTimeSpanDuration(txtDuration.Text).TotalDays);
				dEnDate.DateTime = dateTime2;
				string text = ((dateTime2.Hour.ToString().Length < 2) ? ("0" + dateTime2.Hour) : dateTime2.Hour.ToString());
				string text2 = ((dateTime2.Minute.ToString().Length < 2) ? ("0" + dateTime2.Minute) : dateTime2.Minute.ToString());
				txtEndTime.Text = text + ":" + text2;
				_TimeTextChanged = false;
				_TimeDurationTextChanging = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			_TimeDurationTextChanging = false;
		}
	}

	private void txtEndTime_Leave(object sender, EventArgs e)
	{
		try
		{
			if (!_TimeDurationTextChanging && _TimeTextChanged)
			{
				_TimeDurationTextChanging = true;
				_TimeTextChanged = false;
				SetDurationTimeTextFormat(txtEndTime);
				TimeSpan dateTimeDiff = GetDateTimeDiff();
				if (dateTimeDiff.TotalDays >= 1.0)
				{
					string text = Math.Round(dateTimeDiff.TotalDays, 5) + " Days";
					txtDuration.Text = text;
				}
				else if (dateTimeDiff.TotalHours >= 1.0)
				{
					string text = Math.Round(dateTimeDiff.TotalHours, 2) + " Hours";
					txtDuration.Text = text;
				}
				else if (dateTimeDiff.TotalMinutes > 0.0)
				{
					string text = Math.Round(dateTimeDiff.TotalMinutes) + " Minutes";
					txtDuration.Text = text;
				}
				else
				{
					txtEndTime.Focus();
				}
				_TimeTextChanged = false;
				_TimeDurationTextChanging = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			_TimeDurationTextChanging = false;
		}
	}

	private void txtBeginTime_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_FormLoading)
			{
				_TimeTextChanged = true;
			}
			SetUpdateButton(sender);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtEndTime_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_FormLoading)
			{
				_TimeTextChanged = true;
			}
			SetUpdateButton(sender);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtDuration_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_FormLoading)
			{
				_TimeTextChanged = true;
			}
			SetUpdateButton(sender);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtDuration_Leave(object sender, EventArgs e)
	{
		try
		{
			if (!_TimeDurationTextChanging && _TimeTextChanged)
			{
				_TimeDurationTextChanging = true;
				_TimeTextChanged = false;
				SetDurationTextFormat();
				DateTime dateTime = Convert.ToDateTime(dStart_Recontact.EditValue);
				DateTime dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
				TimeSpan timeSpanTimeTextBox = GetTimeSpanTimeTextBox(txtBeginTime.Text);
				DateTime dateTime3 = dateTime2 + timeSpanTimeTextBox;
				TimeSpan timeSpanDuration = GetTimeSpanDuration(txtDuration.Text);
				DateTime dateTime4 = dateTime3;
				dateTime4 += timeSpanDuration;
				string text = ((dateTime4.Hour.ToString().Length < 2) ? ("0" + dateTime4.Hour) : dateTime4.Hour.ToString());
				string text2 = ((dateTime4.Minute.ToString().Length < 2) ? ("0" + dateTime4.Minute) : dateTime4.Minute.ToString());
				txtEndTime.Text = text + ":" + text2;
				dEnDate.DateTime = dateTime4;
				_TimeDurationTextChanging = false;
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
			bool flag = false;
			int num = Function.ParseInt(txtNumber.Text);
			string linkedObject = _httpClient.ActivityClient.GetLinkedObject(num);
			if (!string.IsNullOrEmpty(linkedObject))
			{
				if (linkedObject.StartsWith("ERR:"))
				{
					WinCommons.Error(Enums.MessageCode.None, linkedObject);
					return;
				}
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteActivityWhenExistsOp, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					flag = true;
				}
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteActivity, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				flag = true;
			}
			if (flag)
			{
				WinCommons._OpenProcessing();
				await DeleteDoc(num);
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void relayActiviToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			string cardCode = txtBpCode.Text;
			await GetTableDefault();
			dt = dtObject.Clone();
			DataRow dataRow = dt.NewRow();
			dataRow["CardCode"] = cardCode;
			dt.Rows.Add(dataRow);
			if (!string.IsNullOrWhiteSpace(cardCode))
			{
				frm_ActivitiesOverviewBP frm_ActivitiesOverviewBP2 = new frm_ActivitiesOverviewBP(dt);
				frm_ActivitiesOverviewBP2._cardCode = cardCode;
				frm_ActivitiesOverviewBP2.ShowDialog(this);
			}
			else
			{
				WinCommons.writeStatus(Enums.MessageCode.BusinessPartnerCanotBeNull, "");
				txtBpCode.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtAfterMaxOccur_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			SetUpdateButton(sender);
			CheckInputTxtAter();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtAfterMaxOccur_Leave(object sender, EventArgs e)
	{
		if (!CheckInputTxtAter())
		{
			txtAfterMaxOccur.Focus();
		}
		else
		{
			txtAfterMaxOccur.BackColor = Color.White;
		}
	}

	private void cbStatus_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (!_isLoadingData && !_FormLoading && Function.ToString(e.NewValue) == "0")
			{
				frm_ActivityStatus frm_ActivityStatus2 = new frm_ActivityStatus();
				frm_ActivityStatus2.MdiParent = MdiParent;
				frm_ActivityStatus2.FormClosing += async (object s, FormClosingEventArgs e2) =>
				{
					await GetStatus();
				};
				frm_ActivityStatus2.Show();
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task AddExistedActivityToServiceCall()
	{
		await GetTableDefault();
		dt = GetSearchDataTable();
		DataRow dataRow = dt.NewRow();
		int num = Function.ParseInt(txtNumber.Text);
		dataRow["ClgCode"] = num;
		dt.Rows.Add(dataRow);
		DateTime cntctDate = DateTime.Today;
		string attachment = "";
		dtDoc = await _httpClient.ActivityClient.SearchDoc(dt);
		if (dtDoc != null && dtDoc.Rows.Count > 0)
		{
			cntctDate = Function.ParseDateTimes(dtDoc.Rows[0]["CntctDate"]);
			attachment = Function.ToString(dtDoc.Rows[0]["Attachment"]);
		}
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("ClgCode", typeof(int));
		dataTable.Columns.Add("CntctDate", typeof(DateTime));
		dataTable.Columns.Add("CntctTime", typeof(string));
		dataTable.Columns.Add("handledby", typeof(int));
		dataTable.Columns.Add("ActivityDate", typeof(DateTime));
		dataTable.Columns.Add("RecurPat", typeof(string));
		dataTable.Columns.Add("Notes", typeof(string));
		dataTable.Columns.Add("Attachment", typeof(string));
		dataTable.Columns.Add("DocType", typeof(string));
		DataRow dataRow2 = dataTable.NewRow();
		dataRow2["ClgCode"] = Function.ParseInt(txtNumber.Text);
		dataRow2["CntctDate"] = cntctDate;
		dataRow2["CntctTime"] = Function.ToString(txtBeginTime.Text);
		dataRow2["handledby"] = Function.ParseInt(cbAttendUser.EditValue);
		dataRow2["ActivityDate"] = Function.ParseDateTimes(dStart_Recontact.EditValue);
		dataRow2["RecurPat"] = Function.ToString(cbRecurPat.EditValue);
		dataRow2["Notes"] = Function.ToString(memoEdit_Note.Text);
		dataRow2["Attachment"] = attachment;
		dataRow2["DocType"] = Function.ToString(_valueDocType);
		dataTable.Rows.Add(dataRow2);
		frmSCall.AddActivityToGrid(dataTable);
	}

	private async void btAdd_Click(object sender, EventArgs e)
	{
		_ = 4;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				if (frmSCall != null)
				{
					await AddExistedActivityToServiceCall();
				}
				if (Function.ParseInt(txtNumber.Text) > 0)
				{
					dtCopy = await _httpClient.ActivityClient.GetActivityByClgCode(Function.ParseInt(txtNumber.Text));
				}
				Close();
			}
			else if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
			{
				bool flag = CheckSave();
				bool flag2 = CheckInputTxtAter();
				if (flag & flag2)
				{
					await Save();
				}
			}
			else if (FormMode == Enums.FormViewModeType.Find)
			{
				txtNumber.ReadOnly = false;
				cb_ContactPerson.EditValue = string.Empty;
				dt = GetSearchDataTable();
				DataRow dataRow = dt.NewRow();
				if (!string.IsNullOrWhiteSpace(txtNumber.Text))
				{
					dataRow["ClgCode"] = txtNumber.Text.Trim();
					_upClgCode = Function.ParseInt(txtNumber.Text.Trim());
				}
				if (!string.IsNullOrWhiteSpace(Function.ToString(cbDocType.SelectedItem)))
				{
					dataRow["DocType"] = Function.ParseInt(_valueDocType);
				}
				if (cbUser.Text == "User")
				{
					dataRow["AttendUser"] = Function.ParseInt(cbAttendUser.EditValue);
				}
				else if (cbUser.Text == "User")
				{
					dataRow["AttendEmpl"] = Function.ParseInt(cbAttendUser.EditValue);
				}
				dataRow["CardCode"] = (string.IsNullOrWhiteSpace(txtBpCode.Text) ? "" : Function.ToString(txtBpCode.Text.Trim()));
				DateTime? dateTime = Function.ParseDateTime(dStart_Recontact.EditValue);
				if (dateTime.HasValue)
				{
					_ = dateTime.Value;
					dataRow["Recontact"] = dateTime;
				}
				DateTime? dateTime2 = Function.ParseDateTime(dEnDate.EditValue);
				if (dateTime2.HasValue)
				{
					_ = dateTime2.Value;
					dataRow["endDate"] = dateTime2;
				}
				dt.Rows.Add(dataRow);
				await Search(dt);
				if (isLoadFromOpportunity)
				{
					dtCopy = dt;
				}
				int absEntry = Function.ParseInt(txtNumber.Text.Trim());
				await ucAttachment1.LoadAttachments(absEntry);
				FormMode = Enums.FormViewModeType.Find;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataTable GetSearchDataTable()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ClgCode", typeof(int));
			dataTable.Columns.Add("DocType", typeof(int));
			dataTable.Columns.Add("AttendUser", typeof(int));
			dataTable.Columns.Add("AttendEmpl", typeof(int));
			dataTable.Columns.Add("CardCode", typeof(string));
			dataTable.Columns.Add("Recontact", typeof(DateTime));
			dataTable.Columns.Add("endDate", typeof(DateTime));
			dataTable.AcceptChanges();
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		if (dtCopy != null)
		{
			dtCopy.Rows.Clear();
			dtCopy.AcceptChanges();
		}
		Close();
	}

	private void btFollowUp_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(txtNumber.Text);
			if (num != _newClgCode)
			{
				frm_ActivitiesForOpportunity frm_ActivitiesForOpportunity2 = new frm_ActivitiesForOpportunity();
				frm_ActivitiesForOpportunity2._activebtFollowUp = true;
				frm_ActivitiesForOpportunity2._clgCodeFrmActivity = num;
				frm_ActivitiesForOpportunity2.Show();
			}
			else
			{
				WinCommons.writeStatus(Enums.MessageCode.AddCurrentActivityBeforeAddFollowActivity, Enums.MessageType.Error, Msg_Time._TimeShort, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
	{
		try
		{
			if (msg.Msg == 256 || msg.Msg == 260)
			{
				btCancel.Enabled = true;
				if (keyData == Keys.Return && FormMode != Enums.FormViewModeType.Update)
				{
					btAdd_Click(null, null);
				}
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return false;
	}

	private void txtBpCode_Leave(object sender, EventArgs e)
	{
		lastFocusLeave = "";
	}

	private void txtBpCode_Enter(object sender, EventArgs e)
	{
		lastFocusLeave = txtBpCode.Name;
	}

	private async void txtBpCode_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0 && !chkClosed.Checked)
			{
				await ChooseCarCode();
				return;
			}
			string text = txtBpCode.Text;
			if (!string.IsNullOrEmpty(text))
			{
				frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
				frm_BusinessMasterData2._copyF_CodeCustomer = text;
				frm_BusinessMasterData2.MdiParent = MdiParent;
				frm_BusinessMasterData2.Show();
			}
			lastFocusLeave = "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtBpCode_TextChanged(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(txtBpCode.Text))
		{
			txtBpType.Text = string.Empty;
			txtBpName.Text = string.Empty;
			txtTelePhone.Text = string.Empty;
			WinFuntions._selectItemComboBoxByIndex(cb_ContactPerson, 0);
			GetImageButton();
			relayActiviToolStripMenuItem.Visible = false;
		}
		else
		{
			relayActiviToolStripMenuItem.Visible = true;
		}
	}

	private void radio_NoEndDate_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			SetUpdateButton(sender);
			if (radio_NoEndDate.Checked)
			{
				txtAfterMaxOccur.ReadOnly = true;
				txtBySeEndDat.ReadOnly = true;
				txtBySeEndDat.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void radio_After_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			SetUpdateButton(sender);
			if (radio_After.Checked)
			{
				txtAfterMaxOccur.ReadOnly = false;
				txtBySeEndDat.ReadOnly = true;
				txtBySeEndDat.Enabled = false;
			}
			if (!CheckInputTxtAter())
			{
				radio_After.Focus();
				radio_After.Checked = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void radio_By_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			SetUpdateButton(sender);
			if (radio_By.Checked)
			{
				txtAfterMaxOccur.ReadOnly = true;
				txtBySeEndDat.ReadOnly = false;
				txtBySeEndDat.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void radio_SubOption1_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (radio_SubOption1.Checked)
			{
				cbWeek.ReadOnly = true;
				cbDayOfWeek.ReadOnly = true;
				cbMonth1.ReadOnly = true;
				txtDayInMonth.ReadOnly = false;
				cbMonth.ReadOnly = false;
			}
			SetUpdateButton(sender);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void radio_SubOption2_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			SetUpdateButton(sender);
			if (radio_SubOption2.Checked)
			{
				txtDayInMonth.ReadOnly = true;
				cbMonth.ReadOnly = true;
				cbWeek.ReadOnly = false;
				cbDayOfWeek.ReadOnly = false;
				cbMonth1.ReadOnly = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void cbUser_SelectedValueChanged(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (cbUser.SelectedItem == "User")
			{
				await GetListUser();
			}
			else
			{
				await GetListEmployee();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cbRecurPat_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			SetUpdateButton(sender);
			string text = Function.ToString(cbRecurPat.EditValue);
			switch (text)
			{
			case "N":
				btFollowUp.Visible = true;
				panel_Daily.Visible = false;
				panel_Range.Visible = false;
				chkMon.Visible = false;
				chkMon.Checked = false;
				chkTue.Visible = false;
				chkTue.Checked = false;
				chkWed.Visible = false;
				chkWed.Checked = false;
				chkThu.Visible = false;
				chkThu.Checked = false;
				chkFri.Visible = false;
				chkFri.Checked = false;
				chkSat.Visible = false;
				chkSat.Checked = false;
				chkSun.Visible = false;
				chkSun.Checked = false;
				return;
			case "D":
				label_RepeatEvery.Text = "Day(s)";
				radio_SubOption1.Text = "";
				radio_SubOption2.Text = "Every Weekday";
				lb_RepeatEvery.Size = new Size(123, 18);
				radio_SubOption1.Location = new Point(207, 10);
				radio_SubOption1.Size = new Size(14, 13);
				radio_SubOption2.Location = new Point(207, 31);
				radio_SubOption2.Size = new Size(101, 17);
				radio_SubOption1.Visible = true;
				radio_SubOption2.Visible = true;
				lb_RepeatOn.Visible = false;
				txtDayInMonth.Visible = false;
				cbWeek.Visible = false;
				cbDayOfWeek.Visible = false;
				cbMonth.Visible = false;
				cbMonth1.Visible = false;
				lb_Of.Visible = false;
				chkMon.Visible = false;
				chkTue.Visible = false;
				chkWed.Visible = false;
				chkThu.Visible = false;
				chkFri.Visible = false;
				chkSat.Visible = false;
				chkSun.Visible = false;
				radio_NoEndDate.Checked = true;
				break;
			case "W":
				label_RepeatEvery.Text = "Week(s)";
				radio_SubOption1.Visible = false;
				radio_SubOption2.Visible = false;
				cbMonth.Visible = false;
				cbMonth1.Visible = false;
				txtDayInMonth.Visible = false;
				cbWeek.Visible = false;
				cbDayOfWeek.Visible = false;
				lb_Of.Visible = false;
				lb_RepeatEvery.Size = new Size(148, 18);
				lb_RepeatOn.Size = new Size(148, 18);
				chkMon.Location = new Point(226, 28);
				chkMon.Size = new Size(46, 19);
				chkMon.TabIndex = 33;
				chkTue.Location = new Point(278, 28);
				chkTue.Size = new Size(46, 19);
				chkTue.TabIndex = 34;
				chkWed.Location = new Point(325, 28);
				chkWed.Size = new Size(46, 19);
				chkWed.TabIndex = 35;
				chkThu.Location = new Point(378, 28);
				chkThu.Size = new Size(46, 19);
				chkThu.TabIndex = 36;
				chkFri.Location = new Point(429, 28);
				chkFri.Size = new Size(33, 19);
				chkFri.TabIndex = 37;
				chkSat.Location = new Point(468, 28);
				chkSat.Size = new Size(37, 19);
				chkSat.TabIndex = 38;
				chkSun.Location = new Point(510, 28);
				chkSun.Size = new Size(46, 19);
				chkSun.TabIndex = 39;
				chkMon.Visible = true;
				chkTue.Visible = true;
				chkWed.Visible = true;
				chkThu.Visible = true;
				chkFri.Visible = true;
				chkSat.Visible = true;
				chkSun.Visible = true;
				cbMonth.Visible = false;
				cbMonth1.Visible = false;
				lb_Of.Visible = false;
				radio_NoEndDate.Checked = true;
				goto default;
			default:
				if (text == "M")
				{
					lb_RepeatEvery.Size = new Size(148, 18);
					label_RepeatEvery.Text = "Month(s)";
					lb_RepeatOn.Size = new Size(123, 18);
					radio_SubOption1.Text = string.Empty;
					radio_SubOption2.Text = string.Empty;
					radio_SubOption1.Text = "Day";
					radio_SubOption1.Visible = true;
					radio_SubOption1.Location = new Point(207, 31);
					radio_SubOption1.Size = new Size(101, 17);
					radio_SubOption2.Location = new Point(207, 53);
					radio_SubOption2.Size = new Size(14, 13);
					txtDayInMonth.Visible = true;
					txtDayInMonth.Location = new Point(253, 29);
					txtDayInMonth.Size = new Size(73, 18);
					radio_SubOption2.Visible = true;
					cbWeek.Visible = true;
					cbDayOfWeek.Visible = true;
					cbMonth.Visible = false;
					cbMonth1.Visible = false;
					lb_Of.Visible = false;
					chkMon.Visible = false;
					chkTue.Visible = false;
					chkWed.Visible = false;
					chkThu.Visible = false;
					chkFri.Visible = false;
					chkSat.Visible = false;
					chkSun.Visible = false;
					radio_NoEndDate.Checked = true;
				}
				if (text == "A")
				{
					label_RepeatEvery.Text = "Year(s)";
					lb_RepeatOn.Size = new Size(123, 18);
					lb_RepeatEvery.Size = new Size(148, 18);
					radio_SubOption1.Text = "";
					radio_SubOption1.Visible = true;
					radio_SubOption2.Text = "";
					radio_SubOption2.Visible = true;
					cbMonth.Location = new Point(253, 28);
					cbMonth.Size = new Size(100, 18);
					txtDayInMonth.Visible = true;
					txtDayInMonth.Location = new Point(359, 28);
					txtDayInMonth.Size = new Size(73, 18);
					cbWeek.Visible = true;
					cbDayOfWeek.Visible = true;
					cbMonth.Visible = true;
					cbMonth1.Visible = true;
					lb_Of.Visible = true;
					chkMon.Visible = false;
					chkTue.Visible = false;
					chkWed.Visible = false;
					chkThu.Visible = false;
					chkFri.Visible = false;
					chkSat.Visible = false;
					chkSun.Visible = false;
					radio_SubOption1.Checked = true;
					radio_NoEndDate.Checked = true;
				}
				lb_RepeatOn.Visible = true;
				break;
			}
			btFollowUp.Visible = false;
			panel_Daily.Visible = true;
			panel_Range.Visible = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cbDocType_DrawItem(object sender, DrawItemEventArgs e)
	{
		try
		{
			if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
			{
				e.DrawBackground();
				e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(254, 234, 163)), e.Bounds);
				Font font = cbDocType.Font;
				e.Graphics.DrawString(cbDocType.Items[e.Index].ToString(), font, new SolidBrush(ColorTranslator.FromHtml("Black")), e.Bounds, StringFormat.GenericDefault);
				e.DrawFocusRectangle();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cbMonth_SelectedValueChanged(object sender, EventArgs e)
	{
		string selectedItem = cbMonth.EditValue.ToString();
		cbMonth1.SelectedItem = selectedItem;
	}

	private void cbMonth1_SelectedValueChanged(object sender, EventArgs e)
	{
		try
		{
			SetUpdateButton(sender);
			string selectedItem = cbMonth1.EditValue.ToString();
			cbMonth.SelectedItem = selectedItem;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void GetTelePh_ByCotacPerson(string cardCode, string cntctCode)
	{
		try
		{
			DataTable dataTable = await _httpClient.ActivityClient.BusinessPartner_GetTel_ByContactPerson(cardCode, cntctCode);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				txtTelePhone.EditValue = Function.ToString(dataTable.Rows[0]["Tel1"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cbDocNum_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string empty = string.Empty;
			string text = ((cbDocType.SelectedItem != null) ? cbDocType.SelectedItem.ToString() : string.Empty);
			empty = ((!chkShowDocumentRelatedBP.Checked) ? string.Empty : txtBpCode.Text);
			if (e.Button.Index == 0)
			{
				if (text != "Items" && text != "Purchase Request")
				{
					WinCommons._OpenProcessing();
					frm_List_Document frm_List_Document2 = new frm_List_Document(GetDocTypeByName(text), multiSelect: false, empty, isNoStatus: true);
					frm_List_Document2.ShowDialog(this);
					WinCommons._CloseProcessing();
					if (frm_List_Document2.ChooseItem != null)
					{
						DocumentsImpl chooseItem = frm_List_Document2.ChooseItem;
						cbDocNum.EditValue = chooseItem.DocEntry;
						if (!string.IsNullOrWhiteSpace(cbDocNum.EditValue.ToString()))
						{
							cbDocNum.Properties.Buttons[1].Visible = true;
						}
						else
						{
							cbDocNum.Properties.Buttons[1].Visible = false;
						}
					}
				}
				else if (text == "Items")
				{
					frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Items, multiSelect: false, string.Empty);
					frm_ChooseFromList2.ShowDialog(this);
					if (frm_ChooseFromList2.ChooseItem != null)
					{
						dynamic chooseItem2 = frm_ChooseFromList2.ChooseItem;
						cbDocNum.EditValue = (object)chooseItem2.ItemCode;
						if (!string.IsNullOrWhiteSpace(cbDocNum.EditValue.ToString()))
						{
							cbDocNum.Properties.Buttons[1].Visible = true;
						}
						else
						{
							cbDocNum.Properties.Buttons[1].Visible = false;
						}
					}
				}
				else
				{
					if (!(text == "Purchase Request"))
					{
						return;
					}
					frm_ChooseFromList frm_ChooseFromList3 = new frm_ChooseFromList(Enums.TypeCFL.ListPurchaseRequest, multiSelect: false, string.Empty);
					frm_ChooseFromList3.ShowDialog(this);
					if (frm_ChooseFromList3.ChooseItem != null)
					{
						dynamic chooseItem3 = frm_ChooseFromList3.ChooseItem;
						cbDocNum.EditValue = (object)chooseItem3.DocEntry;
						if (!string.IsNullOrWhiteSpace(cbDocNum.EditValue.ToString()))
						{
							cbDocNum.Properties.Buttons[1].Visible = true;
						}
						else
						{
							cbDocNum.Properties.Buttons[1].Visible = false;
						}
					}
				}
			}
			else if (e.Button.Index == 1 && cbDocNum.EditValue != null)
			{
				if (text == "Items")
				{
					string itemCode = cbDocNum.EditValue.ToString();
					new frm_ItemMasterData(flag: false, flagSearchFrm: true, itemCode).ShowDialog(this);
				}
				else
				{
					OpenDocForm(GetDocTypeByName(text), chkLinkDraft.Checked);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cbDocType_SelectedValueChanged(object sender, EventArgs e)
	{
		try
		{
			try
			{
				if (cbDocType.SelectedItem != null)
				{
					string text = ((cbDocType.SelectedItem != null) ? cbDocType.SelectedItem.ToString() : string.Empty);
					if (!string.IsNullOrEmpty(text))
					{
						WinFuntions._selectItemComboBoxByIndex(cbDocNum, 0);
						cbDocNum.Properties.Buttons[1].Visible = false;
						switch (text)
						{
						case "":
							_valueDocType = -1;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						case "Sales Quotations":
							_valueDocType = 23;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						case "Sales Orders":
							_valueDocType = 17;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						case "Deliveries":
							_valueDocType = 15;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						case "Returns":
							_valueDocType = 16;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						case "A/R Invoices":
							_valueDocType = 13;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						case "A/R Credit Memos":
							_valueDocType = 14;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						case "A/R Down Payment":
							_valueDocType = 203;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						case "Purchase Request":
							_valueDocType = 1470000113;
							chkShowDocumentRelatedBP.Visible = false;
							break;
						case "Purchase Quotation":
							_valueDocType = 540000006;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						case "Purchase Orders":
							_valueDocType = 22;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						case "Goods Receipt PO":
							_valueDocType = 20;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						case "Goods Return":
							_valueDocType = 21;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						case "A/P Invoices":
							_valueDocType = 18;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						case "A/P Credit Memos":
							_valueDocType = 19;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						case "Landed Costs":
							_valueDocType = 48;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						case "A/P Down Payment":
							_valueDocType = 204;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						case "Incoming Payments":
							_valueDocType = 24;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						case "Outgoing Payments":
							_valueDocType = 46;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						case "Inventory Transfer Request":
							_valueDocType = 1250000001;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						case "Inventory Transfer":
							_valueDocType = 67;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						case "Goods Issue":
							_valueDocType = 60;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						case "Goods Receipt":
							_valueDocType = 59;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						case "Inventory Revaluation":
							_valueDocType = 162;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						case "Production Orders":
							_valueDocType = 202;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						case "Items":
							_valueDocType = 4;
							break;
						case "Blanket Agreements":
							_valueDocType = 1250000025;
							chkShowDocumentRelatedBP.Visible = true;
							break;
						default:
							_valueDocType = -1;
							break;
						}
					}
					else
					{
						_valueDocType = -1;
					}
				}
				else
				{
					_valueDocType = -1;
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			}
			SetUpdateButton(sender);
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
	}

	private string GetDocNameByType(int dType)
	{
		string result = "";
		try
		{
			result = dType switch
			{
				-1 => "", 
				23 => "Sales Quotations", 
				17 => "Sales Orders", 
				15 => "Deliveries", 
				16 => "Returns", 
				13 => "A/R Invoices", 
				14 => "A/R Credit Memos", 
				203 => "A/R Down Payment", 
				1470000113 => "Purchase Request", 
				540000006 => "Purchase Quotation", 
				22 => "Purchase Orders", 
				20 => "Goods Receipt PO", 
				21 => "Goods Return", 
				18 => "A/P Invoices", 
				19 => "A/P Credit Memos", 
				48 => "Landed Costs", 
				204 => "A/P Down Payment", 
				24 => "Incoming Payments", 
				46 => "Outgoing Payments", 
				1250000001 => "Inventory Transfer Request", 
				67 => "Inventory Transfer", 
				60 => "Goods Issue", 
				59 => "Goods Receipt", 
				162 => "Inventory Revaluation", 
				202 => "Production Orders", 
				4 => "Items", 
				1250000025 => "Blanket Agreements", 
				_ => "", 
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return result;
	}

	private void cb_ContactPerson_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string copyF_CodeCustomer = (GlobalData.CardCodeDK = txtBpCode.Text);
			if (e.Button.Index == 1)
			{
				frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
				frm_BusinessMasterData2._copyF_CodeCustomer = copyF_CodeCustomer;
				frm_BusinessMasterData2._cntCode = Function.ParseInt(((Common.ListItems)cb_ContactPerson.SelectedItem).Value);
				frm_BusinessMasterData2.MdiParent = MdiParent;
				frm_BusinessMasterData2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cb_ContactPerson_SelectedValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (cb_ContactPerson.SelectedItem == null)
			{
				return;
			}
			string cardCode = txtBpCode.Text;
			string tmp = Function.ToString(cb_ContactPerson.SelectedItem);
			if (!string.IsNullOrWhiteSpace(tmp))
			{
				string code = _listContactPerson.FirstOrDefault((Common.Table_Code x) => x.Name == tmp).Code;
				GetTelePh_ByCotacPerson(cardCode, code);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetVisibleWhenCbActi_PhoneCall()
	{
		lb_EndTime.Visible = true;
		lb_Duration.Visible = true;
		lb_Status.Visible = true;
		label_MettingLocation.Visible = true;
		txtEndTime.Visible = true;
		txtDuration.Visible = true;
		cbStatus.Visible = true;
		luk_Meetting.Visible = true;
		lb_Recurrence.Visible = true;
		cbRecurPat.Visible = true;
		panel_Daily.Visible = false;
		panel_Range.Visible = false;
		lb_Address.Visible = false;
		lb_Street.Visible = false;
		lb_City.Visible = false;
		lb_Room.Visible = false;
		lb_State.Visible = false;
		lb_Country.Visible = false;
		txtStreet.Visible = false;
		txtCity.Visible = false;
		txtRoom.Visible = false;
		txtState.Visible = false;
		txtCountry.Visible = false;
		lb_Status.Visible = false;
		cbStatus.Visible = false;
		dEnDate.Visible = true;
		chk_Tentative.Visible = false;
	}

	private void cbActivity1_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			SetUpdateButton(sender);
			string text = Function.ToString(cbActivity.EditValue);
			if (text == "C" || text == "P")
			{
				SetVisibleWhenCbActi_PhoneCall();
			}
			if (text == "N")
			{
				SetVisibleWhenCbActi_PhoneCall();
				lb_Recurrence.Visible = false;
				cbRecurPat.Visible = false;
			}
			if (text == "T")
			{
				lb_Status.Visible = true;
				cbStatus.Visible = true;
				chk_Tentative.Visible = false;
				lb_Address.Visible = false;
				lb_Street.Visible = false;
				lb_City.Visible = false;
				lb_Room.Visible = false;
				lb_State.Visible = false;
				lb_Country.Visible = false;
				txtStreet.Visible = false;
				txtCity.Visible = false;
				txtRoom.Visible = false;
				txtCountry.Visible = false;
				txtState.Visible = false;
				chk_Tentative.Visible = false;
			}
			else
			{
				lb_Status.Visible = false;
				cbStatus.Visible = false;
			}
			if (text == "M")
			{
				lb_EndTime.Visible = true;
				lb_Duration.Visible = true;
				label_MettingLocation.Visible = true;
				lb_Recurrence.Visible = true;
				cbRecurPat.Visible = true;
				lb_Address.Visible = true;
				lb_Street.Visible = true;
				lb_City.Visible = true;
				lb_Room.Visible = true;
				lb_State.Visible = true;
				lb_Country.Visible = true;
				txtStreet.Visible = true;
				txtCity.Visible = true;
				txtRoom.Visible = true;
				txtCountry.Visible = true;
				txtState.Visible = true;
				chk_Tentative.Visible = true;
			}
			if (text == "E")
			{
				panel_Daily.Visible = false;
				panel_Range.Visible = false;
				lb_Address.Visible = false;
				lb_Street.Visible = false;
				lb_City.Visible = false;
				lb_Room.Visible = false;
				lb_State.Visible = false;
				lb_Country.Visible = false;
				txtStreet.Visible = false;
				txtCity.Visible = false;
				txtRoom.Visible = false;
				txtState.Visible = false;
				txtCountry.Visible = false;
				lb_EndTime.Visible = false;
				lb_Duration.Visible = false;
				lb_Status.Visible = false;
				label_MettingLocation.Visible = false;
				txtEndTime.Visible = false;
				txtDuration.Visible = false;
				cbStatus.Visible = false;
				luk_Meetting.Visible = false;
				lb_StartTime.Utext = "Time";
				lb_Recurrence.Visible = false;
				cbRecurPat.Visible = false;
				dEnDate.Visible = false;
				chk_Tentative.Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_Type_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseInt(e.NewValue) == 0 && Function.ToString(e.NewValue) == "0")
			{
				frm_ActivityType frm_ActivityType2 = new frm_ActivityType
				{
					MdiParent = MdiParent
				};
				frm_ActivityType2.RefeshParentLookUpSource = (RefreshLookupType)Delegate.Combine(frm_ActivityType2.RefeshParentLookUpSource, new RefreshLookupType(LoadLookupDatasource));
				frm_ActivityType2.Show();
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_Subject_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseInt(e.NewValue) != 0 || !(Function.ToString(e.NewValue) == "0"))
			{
				return;
			}
			short code = Function.ParseShort(luk_Type.EditValue).Value;
			string nameType = "";
			if (luk_Type.Properties.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseShort(r["Code"]) == code);
				if (dataRow != null)
				{
					nameType = Function.ToString(dataRow["Name"]);
				}
			}
			frm_ActivitySubjects frm_ActivitySubjects2 = new frm_ActivitySubjects
			{
				MdiParent = MdiParent,
				madk = code.ToString(),
				nameType = nameType
			};
			frm_ActivitySubjects2.RefeshParentLookUpSource = (RefreshLookupType)Delegate.Combine(frm_ActivitySubjects2.RefeshParentLookUpSource, new RefreshLookupType(ReloadComboSubject));
			frm_ActivitySubjects2.Show();
			e.Cancel = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_Metting_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseInt(e.NewValue) == 0 && Function.ToString(e.NewValue) == "0")
			{
				frm_ActivityMeetingLocation frm_ActivityMeetingLocation2 = new frm_ActivityMeetingLocation
				{
					MdiParent = MdiParent
				};
				frm_ActivityMeetingLocation2.RefeshParentLookUpSource = (RefreshLookupType)Delegate.Combine(frm_ActivityMeetingLocation2.RefeshParentLookUpSource, new RefreshLookupType(LoadLookupDatasource));
				frm_ActivityMeetingLocation2.Show();
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_Popup(object sender, EventArgs e)
	{
		try
		{
			LookUpEdit lookUpEdit = sender as LookUpEdit;
			(((IPopupControl)lookUpEdit).PopupWindow as PopupLookUpEditForm).Width = lookUpEdit.Width;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void luk_Type_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateButton(sender);
		try
		{
			LookUpEdit lookUpEdit = sender as LookUpEdit;
			DataTable dataTable = await GetActivitySubject(Function.ParseInt(lookUpEdit.EditValue).ToString());
			if (dataTable != null && dataTable.Columns.Count > 0)
			{
				if (!dataTable.AsEnumerable().Any((DataRow r) => Function.ParseShort(r["Code"]) == -10))
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["Code"] = Function.ParseShort(-10);
					dataRow["Name"] = "";
					dataTable.Rows.InsertAt(dataRow, 0);
				}
				if (!dataTable.AsEnumerable().Any((DataRow r) => Function.ParseShort(r["Code"]) == 0))
				{
					DataRow dataRow2 = dataTable.NewRow();
					dataRow2["Code"] = Function.ParseShort(0);
					dataRow2["Name"] = "Define New";
					dataTable.Rows.Add(dataRow2);
				}
				dataTable.AcceptChanges();
			}
			luk_Subject.Properties.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_QueryPopUp(object sender, CancelEventArgs e)
	{
		try
		{
			LookUpEdit lookUpEdit = sender as LookUpEdit;
			if (lookUpEdit.Properties.DataSource is DataTable dataTable && dataTable.Rows.Count > 0 && dataTable.Rows.Count < 11)
			{
				lookUpEdit.Properties.DropDownRows = dataTable.Rows.Count;
			}
			else
			{
				lookUpEdit.Properties.DropDownRows = 10;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtCountry_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			SetUpdateButton(sender);
			string country = Function.ToString(txtCountry.EditValue);
			txtState.Properties.DataSource = GetStates(country);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Controls_EditValueChanged(object sender, EventArgs e)
	{
		SetUpdateButton(sender);
	}

	private void SetControlsBackColor()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Find)
			{
				this.SetFindMode();
			}
			else
			{
				this.ResetData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadLookupDatasource()
	{
		_ = 2;
		try
		{
			Task<DataTable> activityType = GetActivityType();
			Task<DataTable> T3 = GetActivityMeetingLocation();
			Task<DataTable> T4 = GetCountries();
			DataTable dt1 = await activityType;
			DataTable dt3 = await T3;
			DataTable dataSource = await T4;
			SetDataSourceByDataTable(luk_Type, dt1);
			if (Function.ParseInt(luk_Type.EditValue) == 0)
			{
				luk_Type.EditValue = Function.ParseShort(-1);
			}
			luk_Subject.Properties.ShowHeader = false;
			luk_Subject.Properties.ShowFooter = false;
			luk_Subject.Properties.AutoHeight = true;
			SetDataSourceByDataTable(luk_Meetting, dt3);
			if (Function.ParseInt(luk_Meetting.EditValue) == 0)
			{
				luk_Meetting.EditValue = Function.ParseShort(-10);
			}
			txtCountry.Properties.Columns.Clear();
			txtCountry.Properties.Columns.Add(new LookUpColumnInfo("Name", ""));
			txtCountry.Properties.DisplayMember = "Name";
			txtCountry.Properties.ValueMember = "Code";
			txtCountry.Properties.NullText = "";
			txtCountry.Properties.DataSource = dataSource;
			txtState.Properties.Columns.Clear();
			txtState.Properties.Columns.Add(new LookUpColumnInfo("Name", ""));
			txtState.Properties.DisplayMember = "Name";
			txtState.Properties.ValueMember = "Code";
			txtState.Properties.NullText = "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GetCountries()
	{
		try
		{
			return await _httpClient.ActivityClient.GetCountries();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private async Task<DataTable> GetStates(string country)
	{
		try
		{
			return await _httpClient.ActivityClient.GetStates(country);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private void SetDataSourceByDataTable(LookUpEdit luk, DataTable dtSource, string displayMember = "Name", string valueMember = "Code")
	{
		try
		{
			if (dtSource.IsNotNull())
			{
				dtSource.NewRow();
				if (luk.Name.Equals("luk_Type") || luk.Name.Equals("luk_Subject") || luk.Name.Equals("luk_Meetting"))
				{
					if (!dtSource.AsEnumerable().Any((DataRow r) => Function.ParseShort(r["Code"]) == 0))
					{
						DataRow dataRow = dtSource.NewRow();
						dataRow[valueMember] = Function.ParseShort(0);
						dataRow[displayMember] = "Define New";
						dtSource.Rows.Add(dataRow);
						dtSource.AcceptChanges();
					}
					if (luk.Name.Equals("luk_Meetting") && !dtSource.AsEnumerable().Any((DataRow r) => Function.ParseShort(r["Code"]) == -10))
					{
						DataRow dataRow2 = dtSource.NewRow();
						dataRow2[valueMember] = Function.ParseShort(-10);
						dataRow2[displayMember] = "";
						dtSource.Rows.InsertAt(dataRow2, 0);
						dtSource.AcceptChanges();
					}
				}
				luk.Properties.DataSource = dtSource;
				luk.Properties.DisplayMember = displayMember;
				luk.Properties.ValueMember = valueMember;
				luk.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
				luk.Properties.DropDownRows = dtSource.Rows.Count;
				SetLookupProperties(luk);
				return;
			}
			if (luk.Name.Equals("luk_Type") || luk.Name.Equals("luk_Subject") || luk.Name.Equals("luk_Metting"))
			{
				DataRow row = dtSource.NewRow();
				dtSource.Rows.InsertAt(row, 0);
				dtSource.AcceptChanges();
				DataRow dataRow3 = dtSource.NewRow();
				dataRow3[valueMember] = Function.ParseShort(0);
				dataRow3[displayMember] = "Define New";
				dtSource.Rows.Add(dataRow3);
				dtSource.AcceptChanges();
				luk.Properties.DataSource = dtSource;
				luk.Properties.DisplayMember = displayMember;
				luk.Properties.ValueMember = valueMember;
				luk.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
				SetLookupProperties(luk);
			}
			else
			{
				DataTable dataTable = (DataTable)luk.Properties.DataSource;
				if (!dataTable.IsNotNull())
				{
					return;
				}
				dataTable.Clear();
				dataTable.AcceptChanges();
				luk.Properties.DataSource = dataTable;
			}
			luk.Properties.Columns.Clear();
			luk.Properties.Columns.Add(new LookUpColumnInfo("Name", ""));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadLookupRecur()
	{
		try
		{
			cbRecurPat.Properties.Columns.Clear();
			cbRecurPat.Properties.Columns.Add(new LookUpColumnInfo("Name", ""));
			cbRecurPat.Properties.NullText = "";
			cbRecurPat.Properties.DisplayMember = "Name";
			cbRecurPat.Properties.ValueMember = "Code";
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("Code", typeof(string)));
			dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
			dataTable.AcceptChanges();
			string[,] array = new string[5, 2]
			{
				{ "N", "None" },
				{ "D", "Daily" },
				{ "W", "Weekly" },
				{ "M", "Monthly" },
				{ "A", "Annually" }
			};
			for (int i = 0; i < array.GetLength(0); i++)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = array[i, 0];
				dataRow["Name"] = array[i, 1];
				dataTable.Rows.Add(dataRow);
			}
			dataTable.AcceptChanges();
			cbRecurPat.Properties.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private TimeSpan GetTimeSpanTimeTextBox(string timeSpan)
	{
		try
		{
			if (!string.IsNullOrWhiteSpace(timeSpan))
			{
				int hours = Function.ParseInt(timeSpan.Split(':')[0]);
				int minutes = Function.ParseInt(timeSpan.Split(':')[1]);
				return new TimeSpan(hours, minutes, 0);
			}
			return new TimeSpan(0, 0, 0);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new TimeSpan(0, 0, 0);
		}
	}

	private TimeSpan GetTimeSpanDuration(string timeSpan)
	{
		try
		{
			if (!string.IsNullOrWhiteSpace(timeSpan))
			{
				TimeSpan result = default;
				double value = Function.ParseDouble(timeSpan.Split(' ')[0]);
				switch (Function.ToString(timeSpan.Split(' ')[1]))
				{
				case "Days":
					result = TimeSpan.FromDays(value);
					break;
				case "Hours":
					result = TimeSpan.FromHours(value);
					break;
				case "Minutes":
					result = TimeSpan.FromMinutes(value);
					break;
				}
				return result;
			}
			return new TimeSpan(0, 0, 0);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new TimeSpan(0, 0, 0);
		}
	}

	private TimeSpan GetDateTimeDiff()
	{
		try
		{
			DateTime dateTime = Function.ParseDateTime(dStart_Recontact.EditValue) ?? DateTime.MinValue;
			DateTime dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
			TimeSpan timeSpanTimeTextBox = GetTimeSpanTimeTextBox(txtBeginTime.Text);
			dateTime2 += timeSpanTimeTextBox;
			DateTime dateTime3 = Function.ParseDateTime(dEnDate.EditValue) ?? DateTime.MinValue;
			DateTime dateTime4 = new DateTime(dateTime3.Year, dateTime3.Month, dateTime3.Day);
			TimeSpan timeSpanTimeTextBox2 = GetTimeSpanTimeTextBox(txtEndTime.Text);
			return dateTime4 + timeSpanTimeTextBox2 - dateTime2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return default;
		}
	}

	private void SetDurationTimeTextFormat(TextEdit tEdit)
	{
		try
		{
			string source = tEdit.Text;
			string text = string.Join("", source.Where((char c) => char.IsDigit(c)));
			switch (text.Length)
			{
			case 1:
				source = "00:0" + text;
				break;
			case 2:
				source = "00:" + text;
				break;
			case 3:
				text = text.Insert(1, ":");
				source = "0" + text;
				break;
			case 4:
				text = text.Insert(2, ":");
				source = text;
				break;
			}
			tEdit.Text = source;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetDurationTextFormat()
	{
		try
		{
			string source = txtDuration.Text;
			string text = string.Join("", source.Where((char c) => char.IsDigit(c)));
			source = string.Join("", source.Where((char c) => char.IsLetter(c))).ToUpper() switch
			{
				"D" => text + " Days", 
				"H" => text + " Hours", 
				"M" => text + " Minutes", 
				_ => (Function.ParseDouble(text) > 60.0 && Function.ParseDouble(text) < 1440.0) ? (Math.Round(Function.ParseDouble(text) / 60.0, 2) + " Hours") : ((!(Function.ParseDouble(text) >= 1440.0)) ? (text + " Minutes") : (Math.Round(Function.ParseDouble(text) / 1440.0, 5) + " Days")), 
			};
			txtDuration.Text = source;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetLookUpDataSource()
	{
		try
		{
			cbActivity.Properties.Columns.Clear();
			cbActivity.Properties.Columns.Add(new LookUpColumnInfo("Name", ""));
			cbActivity.Properties.DisplayMember = "Name";
			cbActivity.Properties.ValueMember = "Code";
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("Code", typeof(string)));
			dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
			dataTable.AcceptChanges();
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = "C";
			dataRow["Name"] = "Phone Call";
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["Code"] = "M";
			dataRow2["Name"] = "Meeting";
			dataTable.Rows.Add(dataRow2);
			DataRow dataRow3 = dataTable.NewRow();
			dataRow3["Code"] = "T";
			dataRow3["Name"] = "Task";
			dataTable.Rows.Add(dataRow3);
			DataRow dataRow4 = dataTable.NewRow();
			dataRow4["Code"] = "E";
			dataRow4["Name"] = "Note";
			dataTable.Rows.Add(dataRow4);
			DataRow dataRow5 = dataTable.NewRow();
			dataRow5["Code"] = "P";
			dataRow5["Name"] = "Campaign";
			dataTable.Rows.Add(dataRow5);
			DataRow dataRow6 = dataTable.NewRow();
			dataRow6["Code"] = "N";
			dataRow6["Name"] = "Other";
			dataTable.Rows.Add(dataRow6);
			dataTable.AcceptChanges();
			cbActivity.Properties.DataSource = dataTable;
			SetLookupProperties(cbActivity);
			LoadLookupRecur();
			SetLookupProperties(cbRecurPat);
			cbStatus.Popup += luk_Popup;
			cbActivity.Popup += luk_Popup;
			cbRecurPat.Popup += luk_Popup;
			cbStatus.QueryPopUp += luk_QueryPopUp;
			cbActivity.QueryPopUp += luk_QueryPopUp;
			cbRecurPat.QueryPopUp += luk_QueryPopUp;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Save()
	{
		try
		{
			DataTable dtOCLG = dtObject.Clone();
			DataRow dataRow = dtOCLG.NewRow();
			string text = (string)(dataRow["Action"] = Function.ToString(cbActivity.EditValue));
			short? num = Function.ParseShort(luk_Type.EditValue);
			if (num.HasValue)
			{
				dataRow["CntctType"] = num;
			}
			short? num2 = Function.ParseShort(luk_Subject.EditValue);
			if (num2.HasValue)
			{
				dataRow["CntctSbjct"] = num2;
			}
			string text2 = cbUser.SelectedItem.ToString();
			if (text2.Equals("User"))
			{
				dataRow["AttendUser"] = Function.ParseInt(cbAttendUser.EditValue);
				dataRow["AttendEmpl"] = DBNull.Value;
				dataRow["SlpCode"] = -1;
			}
			else if (text2.Equals("Employee"))
			{
				dataRow["AttendEmpl"] = Function.ParseInt(cbAttendUser.EditValue);
				dataRow["AttendUser"] = DBNull.Value;
				dataRow["SlpCode"] = 1;
			}
			dataRow["AssignedBy"] = GlobalData.UserID;
			_ = string.Empty;
			string value = ((!chkPerson.Checked) ? "N" : "Y");
			dataRow["personal"] = value;
			dataRow["ClgCode"] = txtNumber.Text;
			dataRow["CardCode"] = txtBpCode.Text;
			if (cb_ContactPerson.SelectedItem != null && _listContactPerson.Count > 0)
			{
				dataRow["CntctCode"] = _listContactPerson.FirstOrDefault((Common.Table_Code x) => x.Name == Function.ToString(cb_ContactPerson.SelectedItem)).Code;
			}
			else
			{
				dataRow["CntctCode"] = 0;
			}
			dataRow["Notes"] = memoEdit_Note.Text;
			dataRow["ContactPer"] = DBNull.Value;
			GetFormatPhone();
			dataRow["Tel"] = _phone;
			dataRow["Fax"] = _phone;
			dataRow["Details"] = txtDetails.Text;
			DateTime? dateTime = Function.ParseDateTime(dStart_Recontact.EditValue);
			DateTime dateTime2;
			if (dateTime.HasValue)
			{
				_ = dateTime.Value;
				if (dateTime.Value.Year >= 1900)
				{
					dateTime2 = dateTime.Value;
					goto IL_02e2;
				}
			}
			dateTime2 = new DateTime(1900, 1, 1);
			goto IL_02e2;
			IL_0744:
			DateTime dateTime3;
			dataRow["SeEndDat"] = dateTime3;
			_ = string.Empty;
			string value2 = ((!chkReminder.Checked) ? "N" : "Y");
			dataRow["Reminder"] = value2;
			string value3 = Function.ToString(WinFuntions.GetNumber(txtRemQty.Text)[0]);
			dataRow["RemQty"] = value3;
			_ = string.Empty;
			string value4 = ((!chkInactive.Checked) ? "N" : "Y");
			dataRow["inactive"] = value4;
			_ = string.Empty;
			string value5;
			if (chkClosed.Checked)
			{
				value5 = "Y";
				dataRow["CloseDate"] = DateTime.Today;
			}
			else
			{
				value5 = "N";
				dataRow["CloseDate"] = new DateTime(1900, 1, 1);
			}
			dataRow["Closed"] = value5;
			dataRow["CntctDate"] = DateTime.Today;
			dataRow["DayInMonth"] = txtDayInMonth.Text;
			switch (Function.ToString(cbWeek.SelectedItem))
			{
			case "first":
				dataRow["Week"] = 1;
				break;
			case "second":
				dataRow["Week"] = 2;
				break;
			case "third":
				dataRow["Week"] = 3;
				break;
			case "fourth":
				dataRow["Week"] = 4;
				break;
			case "last":
				dataRow["Week"] = 5;
				break;
			}
			switch (Function.ToString(cbDayOfWeek.SelectedItem))
			{
			case "day":
				dataRow["DayOfWeek"] = 8;
				break;
			case "weekday":
				dataRow["DayOfWeek"] = 9;
				break;
			case "weekend day":
				dataRow["DayOfWeek"] = 0;
				break;
			case "Sunday":
				dataRow["DayOfWeek"] = 1;
				break;
			case "Monday":
				dataRow["DayOfWeek"] = 2;
				break;
			case "Tuesday":
				dataRow["DayOfWeek"] = 3;
				break;
			case "Wednesday":
				dataRow["DayOfWeek"] = 4;
				break;
			case "Thursday":
				dataRow["DayOfWeek"] = 5;
				break;
			case "Friday":
				dataRow["DayOfWeek"] = 6;
				break;
			case "Saturday":
				dataRow["DayOfWeek"] = 7;
				break;
			}
			switch (Function.ToString(cbMonth.SelectedItem))
			{
			case "January":
				dataRow["Month"] = 1;
				break;
			case "February":
				dataRow["Month"] = 2;
				break;
			case "March":
				dataRow["Month"] = 3;
				break;
			case "April":
				dataRow["Month"] = 4;
				break;
			case "May":
				dataRow["Month"] = 5;
				break;
			case "June":
				dataRow["Month"] = 6;
				break;
			case "July":
				dataRow["Month"] = 7;
				break;
			case "August":
				dataRow["Month"] = 8;
				break;
			case "September":
				dataRow["Month"] = 9;
				break;
			case "October":
				dataRow["Month"] = 10;
				break;
			case "November":
				dataRow["Month"] = 11;
				break;
			case "December":
				dataRow["Month"] = 12;
				break;
			}
			switch ((cbMonth1.SelectedItem != null) ? cbMonth.SelectedItem.ToString() : string.Empty)
			{
			case "January":
				dataRow["Month"] = 1;
				break;
			case "February":
				dataRow["Month"] = 2;
				break;
			case "March":
				dataRow["Month"] = 3;
				break;
			case "April":
				dataRow["Month"] = 4;
				break;
			case "May":
				dataRow["Month"] = 5;
				break;
			case "June":
				dataRow["Month"] = 6;
				break;
			case "July":
				dataRow["Month"] = 7;
				break;
			case "August":
				dataRow["Month"] = 8;
				break;
			case "September":
				dataRow["Month"] = 9;
				break;
			case "October":
				dataRow["Month"] = 10;
				break;
			case "November":
				dataRow["Month"] = 11;
				break;
			case "December":
				dataRow["Month"] = 12;
				break;
			}
			dataRow["DocType"] = _valueDocType;
			if (cbDocNum.SelectedItem != null)
			{
				dataRow["DocNum"] = Function.ParseInt(cbDocNum.EditValue);
				dataRow["DocEntry"] = Function.ParseInt(cbDocNum.EditValue);
			}
			else
			{
				dataRow["DocNum"] = -1;
				dataRow["DocEntry"] = -1;
			}
			dataRow["parentType"] = txtParentType.Text;
			dataRow["parentId"] = Function.ParseInt(txtParentId.Text);
			dataRow["prevActvty"] = Function.ParseInt(txtPrevActivity.Text);
			dataRow["Transfered"] = "N";
			dataRow["Attachment"] = string.Empty;
			dataRow["DataSource"] = "I";
			dataRow["UserSign"] = 1;
			dataRow["LastRemind"] = DBNull.Value;
			dataRow["IsRemoved"] = "N";
			dataRow["OrigDate"] = new DateTime(1900, 1, 1);
			dataRow["SeriesNum"] = DBNull.Value;
			dataRow["state"] = txtState.Text;
			dataRow["room"] = txtRoom.Text;
			dataRow["country"] = txtCountry.Text;
			dataRow["city"] = txtCity.Text;
			dataRow["street"] = txtStreet.Text;
			if (chk_Tentative.Checked)
			{
				dataRow["tentative"] = "Y";
			}
			else
			{
				dataRow["tentative"] = "N";
			}
			if (text == "T")
			{
				int num3 = Function.ParseInt(cbStatus.EditValue);
				if (num3 == -4)
				{
					dataRow["status"] = DBNull.Value;
				}
				else
				{
					dataRow["status"] = num3;
				}
			}
			else
			{
				dataRow["status"] = DBNull.Value;
			}
			dataRow["Instance"] = DBNull.Value;
			dataRow["RemSented"] = "N";
			dataRow["RemTime"] = DBNull.Value;
			dataRow["RemDate"] = new DateTime(1900, 1, 1);
			dataRow["OprId"] = DBNull.Value;
			dataRow["OprLine"] = DBNull.Value;
			string text3 = Function.ToString(WinFuntions.GetChains(txtRemQty.Text)[0]);
			if (!string.IsNullOrEmpty(text3))
			{
				switch (text3)
				{
				case "Minutes":
					dataRow["RemType"] = "M";
					break;
				case "Hours":
					dataRow["RemType"] = "H";
					break;
				case "Days":
					dataRow["RemType"] = "D";
					break;
				default:
					dataRow["RemType"] = "M";
					break;
				}
			}
			string text4 = Function.ToString(WinFuntions.GetChains(txtDuration.Text)[0]);
			if (!string.IsNullOrEmpty(text4))
			{
				switch (text4)
				{
				case "Minutes":
					dataRow["DurType"] = "M";
					break;
				case "Hours":
					dataRow["DurType"] = "H";
					break;
				case "Days":
					dataRow["DurType"] = "D";
					break;
				default:
					dataRow["DurType"] = "M";
					break;
				}
			}
			dataRow["NextTime"] = DBNull.Value;
			dataRow["NextDate"] = new DateTime(1900, 1, 1);
			dataRow["AddrType"] = "S";
			dataRow["AddrName"] = string.Empty;
			if (FormMode == Enums.FormViewModeType.Add)
			{
				dataRow["ClgCode"] = 0;
			}
			dtOCLG.Rows.Add(dataRow);
			dtOCLG.AcceptChanges();
			string text5 = await SaveDoc(dtOCLG);
			if (isLoadFromOpportunity && text5.StartsWith("OK"))
			{
				dtCopy = dtOCLG.Copy();
				dtCopy.Rows[0]["ClgCode"] = Function.ParseInt(text5.Split(':')[1]);
				Close();
			}
			return;
			IL_0676:
			DateTime dateTime4;
			dataRow["SeStartDat"] = dateTime4;
			string value6 = string.Empty;
			if (radio_NoEndDate.Checked)
			{
				value6 = "N";
			}
			else if (radio_After.Checked)
			{
				value6 = "C";
			}
			else if (radio_By.Checked)
			{
				value6 = "D";
			}
			dataRow["EndType"] = value6;
			dataRow["MaxOccur"] = txtAfterMaxOccur.Text;
			DateTime? dateTime5 = Function.ParseDateTime(txtBySeEndDat.EditValue);
			if (dateTime5.HasValue)
			{
				_ = dateTime5.Value;
				if (dateTime5.Value.Year >= 1900)
				{
					dateTime3 = dateTime5.Value;
					goto IL_0744;
				}
			}
			dateTime3 = new DateTime(1900, 1, 1);
			goto IL_0744;
			IL_0346:
			DateTime dateTime6;
			dataRow["endDate"] = dateTime6;
			decimal num4 = Function.ParseDecimal(txtDuration.Text.Split(' ')[0]);
			dataRow["Duration"] = num4;
			int num5 = Function.ParseInt(WinFuntions.GetNumber(txtBeginTime.Text)[0]);
			dataRow["BeginTime"] = num5;
			dataRow["CntctTime"] = num5;
			int num6 = Function.ParseInt(WinFuntions.GetNumber(txtEndTime.Text)[0]);
			dataRow["ENDTime"] = num6;
			switch ((cbPriority.SelectedItem != null) ? cbPriority.SelectedItem.ToString() : string.Empty)
			{
			case "Low":
				dataRow["Priority"] = "0";
				break;
			case "Normal":
				dataRow["Priority"] = "1";
				break;
			case "High":
				dataRow["Priority"] = "2";
				break;
			}
			string text6 = Function.ToString(cbRecurPat.EditValue);
			string value7 = string.Empty;
			dataRow["RecurPat"] = text6;
			if (radio_SubOption1.Checked)
			{
				value7 = "1";
			}
			else if (radio_SubOption2.Checked)
			{
				value7 = "2";
			}
			if (text6 == "N")
			{
				dataRow["SubOption"] = value7;
				dataRow["Monday"] = "N";
				dataRow["Tuesday"] = "N";
				dataRow["Wednesday"] = "N";
				dataRow["Thursday"] = "N";
				dataRow["Friday"] = "N";
				dataRow["Saturday"] = "N";
				dataRow["Sunday"] = "N";
			}
			else
			{
				dataRow["SubOption"] = value7;
				dataRow["Monday"] = chkMon.EditValue;
				dataRow["Tuesday"] = chkTue.EditValue;
				dataRow["Wednesday"] = chkWed.EditValue;
				dataRow["Thursday"] = chkThu.EditValue;
				dataRow["Friday"] = chkFri.EditValue;
				dataRow["Saturday"] = chkSat.EditValue;
				dataRow["Sunday"] = chkSun.EditValue;
			}
			dataRow["Interval"] = txtInterval.Text;
			DateTime? dateTime7 = Function.ParseDateTime(dSeStartDat.EditValue);
			if (dateTime7.HasValue)
			{
				_ = dateTime7.Value;
				if (dateTime7.Value.Year >= 1900)
				{
					dateTime4 = dateTime7.Value;
					goto IL_0676;
				}
			}
			dateTime4 = new DateTime(1900, 1, 1);
			goto IL_0676;
			IL_02e2:
			dataRow["Recontact"] = dateTime2;
			DateTime? dateTime8 = Function.ParseDateTime(dEnDate.EditValue);
			if (dateTime8.HasValue)
			{
				_ = dateTime8.Value;
				if (dateTime8.Value.Year >= 1900)
				{
					dateTime6 = dateTime8.Value;
					goto IL_0346;
				}
			}
			dateTime6 = new DateTime(1900, 1, 1);
			goto IL_0346;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<string> SaveDoc(DataTable dt)
	{
		string ret = "";
		try
		{
			WinCommons._OpenProcessing();
			Function.ParseInt(dt.Rows[0]["ClgCode"]);
			List<AttachmentFile_Impl> list = ucAttachment1.UploadFile();
			if (ucAttachment1.DataSource.IsNotNull() && list.Count == 0)
			{
				return "error";
			}
			Tuple<List<AttachmentFile_Impl>, DataTable> tpl = new Tuple<List<AttachmentFile_Impl>, DataTable>(list, dt);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < dt.Columns.Count; i++)
			{
				stringBuilder.Append(dt.Columns[i].ColumnName + "\t");
			}
			stringBuilder.Append(Environment.NewLine);
			for (int j = 0; j < dt.Rows.Count; j++)
			{
				for (int k = 0; k < dt.Columns.Count; k++)
				{
					stringBuilder.Append(dt.Rows[j][dt.Columns[k]].ToString() + "\t");
				}
				stringBuilder.Append(Environment.NewLine);
			}
			string text = await _httpClient.ActivityClient.SaveDoc(tpl);
			ret = text;
			if (text.StartsWith("OK"))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				if (FormMode == Enums.FormViewModeType.Add)
				{
					await ResetControl();
					await LoadData();
					FormMode = Enums.FormViewModeType.Add;
					await GetNewClgCode();
					if (frmCalendar == null)
					{
					}
				}
				else
				{
					FormMode = Enums.FormViewModeType.View;
					btCancel.Enabled = false;
					_ = frmCalendar;
				}
				if (frmSCall != null && FormMode == Enums.FormViewModeType.Add)
				{
					string text2 = ((Function.ToString(dt.Rows[0]["RecurPat"]) != "") ? Function.ToString(dt.Rows[0]["RecurPat"]) : "N");
					string value = "";
					switch (text2)
					{
					case "N":
						value = "None";
						break;
					case "D":
						value = "Daily";
						break;
					case "W":
						value = "Weekly";
						break;
					case "A":
						value = "Annually";
						break;
					}
					DataTable dataTable = new DataTable();
					dataTable.Columns.Add("ClgCode", typeof(int));
					dataTable.Columns.Add("CntctDate", typeof(DateTime));
					dataTable.Columns.Add("CntctTime", typeof(string));
					dataTable.Columns.Add("handledby", typeof(int));
					dataTable.Columns.Add("ActivityDate", typeof(DateTime));
					dataTable.Columns.Add("RecurPat", typeof(string));
					dataTable.Columns.Add("Notes", typeof(string));
					dataTable.Columns.Add("Attachment", typeof(string));
					dataTable.Columns.Add("DocType", typeof(string));
					DataRow dataRow = dataTable.NewRow();
					dataRow["ClgCode"] = Function.ParseInt(dt.Rows[0]["ClgCode"]);
					dataRow["CntctDate"] = Function.ParseDateTimes(dt.Rows[0]["CntctDate"]);
					dataRow["CntctTime"] = Function.ToString(Function.IntToTime(Function.ParseInt(dt.Rows[0]["CntctTime"])).ToString("HH:mm"));
					dataRow["handledby"] = Function.ParseInt(dt.Rows[0]["AttendUser"]);
					dataRow["ActivityDate"] = Function.ParseDateTimes(dt.Rows[0]["Recontact"]);
					dataRow["RecurPat"] = value;
					dataRow["Notes"] = Function.ToString(dt.Rows[0]["Notes"]);
					dataRow["Attachment"] = Function.ToString(dt.Rows[0]["Attachment"]);
					dataRow["DocType"] = Function.ToString(dt.Rows[0]["DocType"]);
					dataTable.Rows.Add(dataRow);
					frmSCall.AddActivityToGrid(dataTable);
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return ret;
	}

	private async Task Search(DataTable dt)
	{
		_ = 3;
		try
		{
			WinCommons._OpenProcessing();
			dtDoc = await _httpClient.ActivityClient.SearchDoc(dt);
			if (dtDoc.Rows.Count > 1)
			{
				frm_ActivitiesListForOpportunity frm_ActivitiesListForOpportunity2 = new frm_ActivitiesListForOpportunity(dt);
				frm_ActivitiesListForOpportunity2.ShowDialog(this);
				_clgCode_FromOppotunity = frm_ActivitiesListForOpportunity2._clgCode_ActivityListForOpportunity;
				if (_clgCode_FromOppotunity != -1)
				{
					await GetTableDefault();
					dt = GetSearchDataTable();
					DataRow dataRow = dt.NewRow();
					dataRow["ClgCode"] = _clgCode_FromOppotunity;
					dt.Rows.Add(dataRow);
					await Search(dt);
				}
			}
			else
			{
				if (dtDoc == null || dtDoc.Rows.Count <= 0)
				{
					WinCommons.Warning(Enums.MessageCode.NoMatchingRecordFound, "");
					return;
				}
				dtDoc.PrimaryKey = new DataColumn[1] { dtDoc.Columns["ClgCode"] };
				await BindingDataControl(dtDoc);
				deleteToolStripMenuItem.Visible = true;
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadOneDoc(int clgCode)
	{
		_ = 1;
		try
		{
			dtDoc = await _httpClient.ActivityClient.LoadOneDoc(clgCode);
			if (dtDoc != null && dtDoc.Rows.Count > 0)
			{
				dtDoc.PrimaryKey = new DataColumn[1] { dtDoc.Columns["ClgCode"] };
				SetControlsBackColor();
				await BindingDataControl(dtDoc);
				deleteToolStripMenuItem.Visible = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task BindingDataControl(DataTable dtDoc)
	{
		_ = 5;
		try
		{
			_FormLoading = true;
			if (!_activebtFollowUp)
			{
				_TimeDurationTextChanging = true;
				FormMode = Enums.FormViewModeType.View;
				SetControlsBackColor();
				if (_activebtFollowUp)
				{
					txtNumber.Text = Function.ToString(_newClgCode);
				}
				else
				{
					txtNumber.Text = dtDoc.Rows[0]["ClgCode"].ToString();
				}
				txtBpCode.Text = dtDoc.Rows[0]["CardCode"].ToString();
				txtBpType.Text = dtDoc.Rows[0]["CardType"].ToString();
				txtBpName.Text = dtDoc.Rows[0]["CardName"].ToString();
				_ = txtBpCode.Text;
				await GetListContactPerson();
				string value = dtDoc.Rows[0]["CntctCode"].ToString();
				WinFuntions._selectItemComboBoxByValue(cb_ContactPerson, value);
				txtTelePhone.Text = dtDoc.Rows[0]["Tel"].ToString();
				cbActivity.EditValue = Function.ToString(dtDoc.Rows[0]["Action"]);
				dtDoc.Rows[0]["CntctType"].ToString();
				dtDoc.Rows[0]["CntctSbjct"].ToString();
				txtAssignedBy.EditValue = GlobalData.UserNameLogin;
				short? attendUser = Function.ParseShort(dtDoc.Rows[0]["AttendUser"]);
				int attendEmployee = Function.ParseInt(dtDoc.Rows[0]["AttendEmpl"]);
				if (attendUser > 0)
				{
					cbUser.SelectedItem = "User";
					await GetListUser();
					_isSettingCbAttendUser = true;
					cbAttendUser.EditValue = attendUser;
					_isSettingCbAttendUser = false;
				}
				if (attendEmployee > 0)
				{
					cbUser.SelectedItem = "Employee";
					await GetListEmployee();
					_isSettingCbAttendUser = true;
					cbAttendUser.EditValue = attendEmployee;
					_isSettingCbAttendUser = false;
				}
				chkPerson.EditValue = dtDoc.Rows[0]["personal"].ToString();
				txtDetails.Text = dtDoc.Rows[0]["Details"].ToString();
				dStart_Recontact.EditValue = Function.ParseDateTime(dtDoc.Rows[0]["Recontact"]);
				dEnDate.EditValue = Function.ParseDateTime(dtDoc.Rows[0]["endDate"]);
				string text = Function.ToString(dtDoc.Rows[0]["BeginTime"]);
				switch (text.Length)
				{
				case 4:
					txtBeginTime.EditValue = text.Insert(2, ":");
					break;
				case 3:
					txtBeginTime.EditValue = text.Insert(0, "0").Insert(2, ":");
					break;
				case 2:
					txtBeginTime.EditValue = "00:" + text;
					break;
				case 1:
					txtBeginTime.EditValue = "00:0" + text;
					break;
				default:
					txtBeginTime.EditValue = "00:00" + text;
					break;
				}
				string text2 = Function.ToString(dtDoc.Rows[0]["ENDTime"]);
				switch (text2.Length)
				{
				case 4:
					txtEndTime.EditValue = text2.Insert(2, ":");
					break;
				case 3:
					txtEndTime.EditValue = text2.Insert(0, "0").Insert(2, ":");
					break;
				case 2:
					txtEndTime.EditValue = "00:" + text2;
					break;
				case 1:
					txtEndTime.EditValue = "00:0" + text2;
					break;
				default:
					txtEndTime.EditValue = "00:00" + text2;
					break;
				}
				txtDuration.EditValue = dtDoc.Rows[0]["Duration"]?.ToString() + " " + dtDoc.Rows[0]["DurType"].ToString();
				cbPriority.EditValue = dtDoc.Rows[0]["Priority"].ToString();
				cbRecurPat.EditValue = Function.ToString(dtDoc.Rows[0]["RecurPat"]);
				string text3 = Function.ToString(cbRecurPat.EditValue);
				string text4 = dtDoc.Rows[0]["SubOption"].ToString();
				switch (text3)
				{
				case "D":
					if (!(text4 == "1"))
					{
						if (text4 == "2")
						{
							radio_SubOption2.Checked = true;
						}
					}
					else
					{
						radio_SubOption1.Checked = true;
					}
					break;
				case "M":
				case "A":
					if (!(text4 == "1"))
					{
						if (text4 == "2")
						{
							radio_SubOption2.Checked = true;
						}
					}
					else
					{
						radio_SubOption1.Checked = true;
					}
					break;
				}
				chkMon.EditValue = dtDoc.Rows[0]["Monday"].ToString();
				chkTue.EditValue = dtDoc.Rows[0]["Tuesday"].ToString();
				chkWed.EditValue = dtDoc.Rows[0]["Wednesday"].ToString();
				chkThu.EditValue = dtDoc.Rows[0]["Thursday"].ToString();
				chkFri.EditValue = dtDoc.Rows[0]["Friday"].ToString();
				chkSat.EditValue = dtDoc.Rows[0]["Saturday"].ToString();
				chkSun.EditValue = dtDoc.Rows[0]["Sunday"].ToString();
				txtInterval.EditValue = dtDoc.Rows[0]["Interval"].ToString();
				dSeStartDat.EditValue = dtDoc.Rows[0]["SeStartDat"].ToString();
				switch (dtDoc.Rows[0]["EndType"].ToString())
				{
				case "N":
					radio_NoEndDate.Checked = true;
					break;
				case "C":
					radio_After.Checked = true;
					break;
				case "D":
					radio_By.Checked = true;
					break;
				}
				txtAfterMaxOccur.EditValue = dtDoc.Rows[0]["MaxOccur"].ToString();
				txtBySeEndDat.EditValue = ((dtDoc.Rows[0]["SeEndDat"].ToString() != null) ? dtDoc.Rows[0]["SeEndDat"].ToString() : string.Empty);
				chkReminder.EditValue = dtDoc.Rows[0]["Reminder"].ToString();
				txtRemQty.EditValue = dtDoc.Rows[0]["RemQty"].ToString() + " " + dtDoc.Rows[0]["RemType"].ToString();
				chkInactive.EditValue = dtDoc.Rows[0]["inactive"].ToString();
				chkClosed.EditValue = dtDoc.Rows[0]["Closed"].ToString();
				SetEnabel_WhenClosed_Y(chkClosed.Checked);
				txtDayInMonth.EditValue = dtDoc.Rows[0]["DayInMonth"].ToString();
				cbWeek.SelectedItem = dtDoc.Rows[0]["Week"].ToString();
				cbDayOfWeek.EditValue = dtDoc.Rows[0]["DayOfWeek"].ToString();
				cbMonth1.SelectedItem = dtDoc.Rows[0]["Month"].ToString();
				cbMonth.SelectedItem = dtDoc.Rows[0]["Month"].ToString();
				memoEdit_Note.EditValue = dtDoc.Rows[0]["Notes"].ToString();
				cbDocType.SelectedItem = dtDoc.Rows[0]["DocType"].ToString();
				cbDocNum.EditValue = ((dtDoc.Rows[0]["DocNum"].ToString() != "-1") ? dtDoc.Rows[0]["DocNum"].ToString() : string.Empty);
				txtParentType.Text = dtDoc.Rows[0]["parentType"].ToString();
				txtParentId.Text = ((dtDoc.Rows[0]["parentId"].ToString() != "0") ? dtDoc.Rows[0]["parentId"].ToString() : string.Empty);
				txtPrevActivity.Text = ((dtDoc.Rows[0]["prevActvty"].ToString() != "0") ? dtDoc.Rows[0]["prevActvty"].ToString() : string.Empty);
				GetImageButton();
				_TimeDurationTextChanging = false;
			}
			else if (_activebtFollowUp)
			{
				FormMode = Enums.FormViewModeType.Add;
				SetControlsBackColor();
				txtNumber.Text = Function.ToString(_newClgCode);
				txtBpCode.Text = dtDoc.Rows[0]["CardCode"].ToString();
				txtBpType.Text = dtDoc.Rows[0]["CardType"].ToString();
				txtBpName.Text = dtDoc.Rows[0]["CardName"].ToString();
				_ = txtBpCode.Text;
				await GetListContactPerson();
				string value2 = dtDoc.Rows[0]["CntctCode"].ToString();
				WinFuntions._selectItemComboBoxByValue(cb_ContactPerson, value2);
				txtTelePhone.Text = dtDoc.Rows[0]["Tel"].ToString();
				cbActivity.EditValue = Function.ToString(dtDoc.Rows[0]["Action"]);
				txtAssignedBy.EditValue = GlobalData.UserNameLogin;
				short? attendUser = Function.ParseShort(dtDoc.Rows[0]["AttendUser"]);
				int attendEmployee = Function.ParseInt(dtDoc.Rows[0]["AttendEmpl"]);
				if (attendUser != 0)
				{
					cbUser.SelectedItem = "User";
					await GetListUser();
					_isSettingCbAttendUser = true;
					cbAttendUser.EditValue = attendUser;
					_isSettingCbAttendUser = false;
				}
				if (attendEmployee != 0)
				{
					cbUser.SelectedItem = "Employee";
					await GetListEmployee();
					_isSettingCbAttendUser = true;
					cbAttendUser.EditValue = attendEmployee;
					_isSettingCbAttendUser = false;
				}
				chkPerson.EditValue = dtDoc.Rows[0]["personal"].ToString();
				txtDetails.Text = dtDoc.Rows[0]["Details"].ToString().Insert(0, "Continuation: ");
				dStart_Recontact.EditValue = DateTime.Now.Date;
				dEnDate.EditValue = DateTime.Now.Date;
				GetTime();
				txtBeginTime.EditValue = _beginTime;
				txtEndTime.EditValue = _endTime;
				txtDuration.EditValue = dtDoc.Rows[0]["Duration"].ToString() + " " + dtDoc.Rows[0]["DurType"].ToString();
				cbPriority.EditValue = dtDoc.Rows[0]["Priority"].ToString();
				dtDoc.Rows[0]["Location"].ToString();
				cbRecurPat.EditValue = Function.ToString(dtDoc.Rows[0]["RecurPat"]);
				string text5 = Function.ToString(cbRecurPat.EditValue);
				string text6 = dtDoc.Rows[0]["SubOption"].ToString();
				switch (text5)
				{
				case "D":
					if (!(text6 == "1"))
					{
						if (text6 == "2")
						{
							radio_SubOption2.Checked = true;
						}
					}
					else
					{
						radio_SubOption1.Checked = true;
					}
					break;
				case "M":
				case "A":
					if (!(text6 == "1"))
					{
						if (text6 == "2")
						{
							radio_SubOption2.Checked = true;
						}
					}
					else
					{
						radio_SubOption1.Checked = true;
					}
					break;
				}
				txtInterval.EditValue = dtDoc.Rows[0]["Interval"].ToString();
				dSeStartDat.EditValue = DateTime.Now.Date;
				switch (dtDoc.Rows[0]["EndType"].ToString())
				{
				case "N":
					radio_NoEndDate.Checked = true;
					break;
				case "C":
					radio_After.Checked = true;
					break;
				case "D":
					radio_By.Checked = true;
					break;
				}
				txtAfterMaxOccur.EditValue = dtDoc.Rows[0]["MaxOccur"].ToString();
				txtBySeEndDat.EditValue = string.Empty;
				chkReminder.EditValue = dtDoc.Rows[0]["Reminder"].ToString();
				txtRemQty.EditValue = dtDoc.Rows[0]["RemQty"].ToString() + " " + dtDoc.Rows[0]["RemType"].ToString();
				chkInactive.EditValue = dtDoc.Rows[0]["inactive"].ToString();
				chkClosed.EditValue = dtDoc.Rows[0]["Closed"].ToString();
				SetEnabel_WhenClosed_Y(chkClosed.Checked);
				GetDay(dateChange);
				GetDayOfMonth(dateChange);
				GetWeekOfMonth(dateChange);
				GetMonth(dateChange);
				memoEdit_Note.EditValue = dtDoc.Rows[0]["Notes"].ToString();
				cbDocType.SelectedItem = dtDoc.Rows[0]["DocType"].ToString();
				cbDocNum.EditValue = ((dtDoc.Rows[0]["DocNum"].ToString() != "-1") ? dtDoc.Rows[0]["DocNum"].ToString() : string.Empty);
				txtParentType.Text = dtDoc.Rows[0]["parentType"].ToString();
				txtParentId.Text = ((dtDoc.Rows[0]["parentId"].ToString() != "0") ? dtDoc.Rows[0]["parentId"].ToString() : string.Empty);
				txtPrevActivity.Text = Function.ToString(_clgCodeFrmActivity);
				GetImageButton();
			}
			txtNumber.ReadOnly = true;
			string text7 = dtDoc.Rows[0]["status"].ToString();
			if (text7 == "" || text7 == "0")
			{
				cbStatus.EditValue = -2;
			}
			else
			{
				cbStatus.EditValue = Function.ParseInt(dtDoc.Rows[0]["status"]);
			}
			chk_Tentative.Checked = Function.ToString(dtDoc.Rows[0]["tentative"]) == "Y";
			luk_Type.EditValue = Function.ParseShort(dtDoc.Rows[0]["CntctType"]);
			txtState.Text = Function.ToString(dtDoc.Rows[0]["state"]);
			txtRoom.Text = Function.ToString(dtDoc.Rows[0]["room"]);
			txtCountry.Text = Function.ToString(dtDoc.Rows[0]["country"]);
			txtCity.Text = Function.ToString(dtDoc.Rows[0]["city"]);
			txtStreet.Text = Function.ToString(dtDoc.Rows[0]["street"]);
			txtBpCode.ReadOnly = true;
			txtBpCode.Properties.Buttons[0].Visible = false;
			luk_Subject.EditValue = Function.ParseShort(dtDoc.Rows[0]["CntctSbjct"]);
			_FormLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetEnabel_WhenClosed_Y(bool disable)
	{
		try
		{
			btAdd.Enabled = !disable;
			cbActivity.ReadOnly = disable;
			luk_Type.ReadOnly = disable;
			luk_Subject.ReadOnly = disable;
			cbUser.Enabled = !disable;
			cbAttendUser.ReadOnly = disable;
			txtBpCode.ReadOnly = disable;
			cb_ContactPerson.ReadOnly = disable;
			txtTelePhone.ReadOnly = disable;
			chkPerson.ReadOnly = disable;
			txtDetails.ReadOnly = disable;
			dStart_Recontact.ReadOnly = disable;
			txtBeginTime.ReadOnly = disable;
			dEnDate.ReadOnly = disable;
			txtEndTime.ReadOnly = disable;
			cbPriority.ReadOnly = disable;
			luk_Meetting.ReadOnly = disable;
			txtDuration.ReadOnly = disable;
			cbRecurPat.ReadOnly = disable;
			txtInterval.ReadOnly = disable;
			cbMonth.ReadOnly = disable;
			txtDayInMonth.ReadOnly = disable;
			cbWeek.ReadOnly = disable;
			cbDayOfWeek.ReadOnly = disable;
			cbMonth1.ReadOnly = disable;
			radio_SubOption1.Enabled = !disable;
			radio_SubOption2.Enabled = !disable;
			chkMon.ReadOnly = disable;
			chkTue.ReadOnly = disable;
			chkWed.ReadOnly = disable;
			chkThu.ReadOnly = disable;
			chkFri.ReadOnly = disable;
			chkSat.ReadOnly = disable;
			chkSun.ReadOnly = disable;
			dSeStartDat.ReadOnly = disable;
			radio_NoEndDate.Enabled = !disable;
			radio_After.Enabled = !disable;
			radio_By.Enabled = !disable;
			txtAfterMaxOccur.ReadOnly = disable;
			txtBySeEndDat.ReadOnly = disable;
			chkReminder.ReadOnly = disable;
			txtRemQty.ReadOnly = disable;
			chkInactive.ReadOnly = disable;
			chkClosed.ReadOnly = disable;
			memoEdit_Note.ReadOnly = disable;
			cbDocType.Enabled = !disable;
			cbDocNum.ReadOnly = disable;
			chkLinkDraft.ReadOnly = disable;
			chkShowDocumentRelatedBP.ReadOnly = disable;
			chk_Tentative.ReadOnly = disable;
			txtStreet.ReadOnly = disable;
			txtCity.ReadOnly = disable;
			txtRoom.ReadOnly = disable;
			txtState.ReadOnly = disable;
			txtCountry.ReadOnly = disable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task ResetControl()
	{
		_ = 2;
		try
		{
			if (_privilege == "F")
			{
				btAdd.Enabled = true;
			}
			cbActivity.ReadOnly = false;
			luk_Type.ReadOnly = false;
			luk_Subject.ReadOnly = false;
			cbUser.Enabled = true;
			cbAttendUser.ReadOnly = false;
			txtBpCode.ReadOnly = false;
			cb_ContactPerson.ReadOnly = false;
			txtTelePhone.ReadOnly = false;
			chkPerson.ReadOnly = false;
			txtDetails.ReadOnly = false;
			dStart_Recontact.ReadOnly = false;
			txtBeginTime.ReadOnly = false;
			dEnDate.ReadOnly = false;
			txtEndTime.ReadOnly = false;
			cbPriority.ReadOnly = false;
			luk_Meetting.ReadOnly = false;
			txtDuration.ReadOnly = false;
			cbRecurPat.ReadOnly = false;
			txtInterval.ReadOnly = false;
			cbMonth.ReadOnly = false;
			txtDayInMonth.ReadOnly = false;
			cbWeek.ReadOnly = false;
			cbDayOfWeek.ReadOnly = false;
			cbMonth1.ReadOnly = false;
			radio_SubOption1.Enabled = true;
			radio_SubOption2.Enabled = true;
			chkMon.ReadOnly = false;
			chkTue.ReadOnly = false;
			chkWed.ReadOnly = false;
			chkThu.ReadOnly = false;
			chkFri.ReadOnly = false;
			chkSat.ReadOnly = false;
			chkSun.ReadOnly = false;
			dSeStartDat.ReadOnly = false;
			radio_NoEndDate.Enabled = true;
			radio_After.Enabled = true;
			radio_By.Enabled = true;
			txtAfterMaxOccur.ReadOnly = false;
			txtBySeEndDat.ReadOnly = false;
			chkReminder.ReadOnly = false;
			txtRemQty.ReadOnly = false;
			chkInactive.ReadOnly = false;
			chkClosed.ReadOnly = false;
			memoEdit_Note.ReadOnly = false;
			cbDocType.Enabled = true;
			cbDocNum.ReadOnly = false;
			txtNumber.Text = string.Empty;
			txtNumber.Focus();
			txtBpCode.Text = string.Empty;
			txtBpCode.ReadOnly = false;
			txtBpCode.Properties.Buttons[0].Visible = true;
			txtBpType.Text = string.Empty;
			txtCardCode.Text = string.Empty;
			txtTelePhone.Text = string.Empty;
			await LoadLookupDatasource();
			_isSettingCbAttendUser = true;
			cbAttendUser.EditValue = Function.ParseShort(-1);
			_isSettingCbAttendUser = false;
			txtAssignedBy.Text = string.Empty;
			chkPerson.Checked = false;
			txtDetails.Text = string.Empty;
			_TimeDurationTextChanging = true;
			dStart_Recontact.EditValue = Function.ParseDateTime(string.Empty);
			txtBeginTime.Text = string.Empty;
			_TimeDurationTextChanging = true;
			dEnDate.EditValue = string.Empty;
			_TimeDurationTextChanging = true;
			txtEndTime.EditValue = string.Empty;
			_TimeDurationTextChanging = true;
			txtDuration.EditValue = string.Empty;
			cbRecurPat.EditValue = "N";
			chkMon.Checked = false;
			chkTue.Checked = false;
			chkWed.Checked = false;
			chkThu.Checked = false;
			chkFri.Checked = false;
			chkSat.Checked = false;
			chkSun.Checked = false;
			cbMonth.EditValue = string.Empty;
			txtDayInMonth.EditValue = string.Empty;
			cbWeek.EditValue = string.Empty;
			cbDayOfWeek.EditValue = string.Empty;
			cbMonth1.EditValue = string.Empty;
			dSeStartDat.EditValue = string.Empty;
			txtAfterMaxOccur.EditValue = 1;
			txtBySeEndDat.EditValue = string.Empty;
			radio_SubOption1.Checked = false;
			radio_SubOption2.Checked = false;
			radio_NoEndDate.Checked = false;
			radio_After.Checked = false;
			radio_By.Checked = false;
			chkReminder.Checked = false;
			txtRemQty.EditValue = string.Empty;
			chkInactive.Checked = false;
			chkClosed.Checked = false;
			memoEdit_Note.EditValue = string.Empty;
			_isSettingCbDocType = true;
			cbDocType.SelectedIndex = -1;
			_isSettingCbDocType = false;
			cbDocNum.EditValue = string.Empty;
			txtParentType.Text = string.Empty;
			txtParentId.Text = string.Empty;
			txtPrevActivity.Text = string.Empty;
			await GetListContactPerson();
			cb_ContactPerson.EditValue = string.Empty;
			await GetActivityMeetingLocation();
			cbPriority.SelectedIndex = 0;
			ucAttachment1.DataSource = null;
			ucAttachment1.grd_File.RefreshDataSource();
			_TimeDurationTextChanging = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetFormatPhone()
	{
		try
		{
			_phone = txtTelePhone.Text;
			for (int num = _phone.Length - 1; num > 0; num--)
			{
				if (char.IsDigit(_phone[num]))
				{
					if (num != _phone.Length - 1)
					{
						_phone = _phone.Remove(num + 1);
					}
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool CheckSave()
	{
		try
		{
			string text = txtTelePhone.Text;
			if (!string.IsNullOrWhiteSpace(text) && text.Length > 20)
			{
				WinCommons.writeStatus(Enums.MessageCode.TelephoneNotExceeding20, Enums.MessageType.Error, Msg_Time._TimeShort, "");
				return false;
			}
			if (!string.IsNullOrWhiteSpace(text) && text.Length < 10)
			{
				WinCommons.writeStatus(Enums.MessageCode.TelephoneLeast10, Enums.MessageType.Error, Msg_Time._TimeShort, "");
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return false;
	}

	private bool CheckInputTxtAter()
	{
		try
		{
			string text = Function.ToString(cbRecurPat.EditValue);
			if (text != "N" && !string.IsNullOrWhiteSpace(text))
			{
				int result;
				if (string.IsNullOrWhiteSpace(txtAfterMaxOccur.Text))
				{
					if (FormMode != Enums.FormViewModeType.Find)
					{
						WinCommons.writeStatus(Enums.MessageCode.AfterNotNull, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
						txtAfterMaxOccur.Focus();
						txtAfterMaxOccur.BackColor = Color.FromArgb(254, 234, 163);
						return false;
					}
				}
				else if (!int.TryParse(txtAfterMaxOccur.Text, out result))
				{
					WinCommons.writeStatus(Enums.MessageCode.InvalidNumericValue, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
					txtAfterMaxOccur.Focus();
					txtAfterMaxOccur.BackColor = Color.FromArgb(254, 234, 163);
					return false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return true;
	}

	private ApzObjectTypes GetDocTypeByName(string docName)
	{
		try
		{
			ApzObjectTypes result = ApzObjectTypes.oSalesOpportunities;
			switch (docName)
			{
			case "Sales Quotations":
				result = ApzObjectTypes.oQuotations;
				break;
			case "Sales Orders":
				result = ApzObjectTypes.oOrders;
				break;
			case "Deliveries":
				result = ApzObjectTypes.oDeliveryNotes;
				break;
			case "Returns":
				result = ApzObjectTypes.oReturns;
				break;
			case "A/R Invoices":
				result = ApzObjectTypes.oInvoices;
				break;
			case "A/R Credit Memos":
				result = ApzObjectTypes.oCreditNotes;
				break;
			case "A/R DownPayment":
				result = ApzObjectTypes.oDownPayments;
				break;
			case "Purchase Request":
				result = ApzObjectTypes.oPurchaseRequest;
				break;
			case "Purchase Quotation":
				result = ApzObjectTypes.oPurchaseQuotations;
				break;
			case "Purchase Orders":
				result = ApzObjectTypes.oPurchaseOrders;
				break;
			case "Goods Receipt PO":
				result = ApzObjectTypes.oPurchaseDeliveryNotes;
				break;
			case "Goods Return":
				result = ApzObjectTypes.oPurchaseReturns;
				break;
			case "A/P Invoices":
				result = ApzObjectTypes.oPurchaseInvoices;
				break;
			case "A/P Credit Memos":
				result = ApzObjectTypes.oPurchaseCreditNotes;
				break;
			case "Landed Costs":
				result = ApzObjectTypes.oLandedCostsCodes;
				break;
			case "A/P Down Payment":
				result = ApzObjectTypes.oPurchaseDownPayments;
				break;
			case "Incoming Payments":
				result = ApzObjectTypes.oIncomingPayments;
				break;
			case "Outgoing Payments":
				result = ApzObjectTypes.oVendorPayments;
				break;
			case "Inventory Transfer Request":
				result = ApzObjectTypes.oInventoryTransferRequest;
				break;
			case "Inventory Transfer":
				result = ApzObjectTypes.oStockTransfer;
				break;
			case "Goods Issue":
				result = ApzObjectTypes.oInventoryGenExit;
				break;
			case "Goods Receipt":
				result = ApzObjectTypes.oInventoryGenEntry;
				break;
			case "Inventory Revaluation":
				result = ApzObjectTypes.oMaterialRevaluation;
				break;
			case "Production Orders":
				result = ApzObjectTypes.oProductionOrders;
				break;
			case "Blanket Agreements":
				result = ApzObjectTypes.oBlanketAgreement;
				break;
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ApzObjectTypes.oSalesOpportunities;
		}
	}

	private void OpenDocForm(ApzObjectTypes docType, bool isDraft)
	{
		try
		{
			if (isDraft)
			{
				this.DrillDown(ApzObjectTypes.oDrafts, _linkDocNum.ToString());
				return;
			}
			dynamic val = new XtraFormEx();
			switch (docType)
			{
			case ApzObjectTypes.oQuotations:
				val = new frm_SalesQuotationForBatch(_linkDocNum);
				break;
			case ApzObjectTypes.oOrders:
				val = new frm_SalesOrder(_linkDocNum);
				break;
			case ApzObjectTypes.oDeliveryNotes:
				val = new frm_Delivery(_linkDocNum);
				break;
			case ApzObjectTypes.oReturns:
				val = new frm_Return(_linkDocNum);
				break;
			case ApzObjectTypes.oInvoices:
				val = new frm_ARInvoices(_linkDocNum);
				break;
			case ApzObjectTypes.oCreditNotes:
				val = new frm_APCreditMemos(_linkDocNum);
				break;
			case ApzObjectTypes.oPurchaseRequest:
				val = new frm_PurchaseRequest();
				val.isLoadFrmActivity = true;
				val._docEntrySearchFrmActivity = Function.ParseInt(cbDocNum.EditValue);
				break;
			case ApzObjectTypes.oPurchaseQuotations:
				val = new frm_PurchaseQuotation(_linkDocNum);
				break;
			case ApzObjectTypes.oPurchaseOrders:
				val = new frm_PurchaseOder(_linkDocNum);
				break;
			case ApzObjectTypes.oPurchaseDeliveryNotes:
				val = new frm_GoodReceiptPO(_linkDocNum);
				break;
			case ApzObjectTypes.oPurchaseReturns:
				val = new frm_GoodsReturn(_linkDocNum);
				break;
			case ApzObjectTypes.oPurchaseInvoices:
				val = new frm_APInvoices(_linkDocNum);
				break;
			case ApzObjectTypes.oPurchaseCreditNotes:
				val = new frm_APCreditMemos(_linkDocNum);
				break;
			case ApzObjectTypes.oLandedCostsCodes:
				val = new frm_PurchaseLandedCost();
				break;
			case ApzObjectTypes.oIncomingPayments:
				val = new frm_PaymentIncoming();
				break;
			case ApzObjectTypes.oVendorPayments:
				val = new frm_PaymentOutgoing();
				break;
			case ApzObjectTypes.oInventoryTransferRequest:
				val = new frm_InventoryTranfer();
				break;
			case ApzObjectTypes.oStockTransfer:
				val = new frm_PaymentOutgoing();
				break;
			case ApzObjectTypes.oInventoryGenExit:
				val = new frm_GoodsIssue();
				break;
			case ApzObjectTypes.oInventoryGenEntry:
				val = new frm_InventoryTranferRequest();
				break;
			case ApzObjectTypes.oMaterialRevaluation:
				val = new frm_InventoryRevaluation();
				break;
			case ApzObjectTypes.oProductionOrders:
				val = new frm_ProductionOrder();
				break;
			}
			val.MdiParent = MdiParent;
			val.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task DeleteDoc(int id)
	{
		_ = string.Empty;
		try
		{
			string text = await _httpClient.ActivityClient.DeleteOCLG(id);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
				REFRESH();
				if (frmCalendar != null)
				{
					await frmCalendar.ReloadCalendar();
				}
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
	}

	private void ChooseContactPerson(string cardCode)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.ContactPerson, multiSelect: false, string.Empty);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				BusinessPartnersImpl businessPartnersImpl = (BusinessPartnersImpl)frm_ChooseFromList2.ChooseItem;
				cb_ContactPerson.EditValue = businessPartnersImpl.Name;
				txtTelePhone.Text = businessPartnersImpl.Tel1;
			}
			frm_ChooseFromList2.Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task ChooseCarCode()
	{
		_ = 2;
		try
		{
			BusinessPartnersImpl buss = new BusinessPartnersImpl();
			if (isLoadFromOpportunity && _cardCodefrm_SalesOpportunity != null)
			{
				DataTable dataTable = await ExtenseionMethods.GetBPByCardCode(_cardCodefrm_SalesOpportunity + ";" + GlobalData.UserName);
				buss.CardCode = _cardCodefrm_SalesOpportunity;
				buss.CardType = dataTable.Rows[0]["CardType"].ToString();
				buss.CardName = dataTable.Rows[0]["CardName"].ToString();
				await SetBPData(buss);
			}
			else if ((!isLoadFromOpportunity || !string.IsNullOrEmpty(_cardCodefrm_SalesOpportunity)) && string.IsNullOrEmpty(_cardCodefrm_SalesOpportunity))
			{
				string txtText = txtBpCode.Text;
				frm_ChooseFromList frm = new frm_ChooseFromList(Enums.TypeCFL.Bp, multiSelect: false, txtText);
				frm.ShowDialog(this);
				if (frm.ChooseItem != null)
				{
					buss = (BusinessPartnersImpl)frm.ChooseItem;
					await SetBPData(buss);
				}
				frm.Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task SetBPData(BusinessPartnersImpl buss)
	{
		try
		{
			txtBpCode.Text = buss.CardCode;
			if (!string.IsNullOrWhiteSpace(txtBpCode.Text))
			{
				txtBpCode.Properties.Buttons[1].Visible = true;
				await GetListContactPerson();
				string cardType = buss.CardType;
				if (cardType == "S")
				{
					txtBpType.Text = "Vendor";
				}
				if (cardType == "C")
				{
					txtBpType.Text = "Customer";
				}
				if (cardType == "L")
				{
					txtBpType.Text = "Lead";
				}
				txtBpName.Text = buss.CardName;
				txtTelePhone.Text = buss.Tel1;
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
		_ = 2;
		try
		{
			if (FormMode == Enums.FormViewModeType.Add)
			{
				await GetNewClgCode();
			}
			cbUser.SelectedIndex = 0;
			cbRecurPat.EditValue = "N";
			cbPriority.SelectedIndex = 0;
			cbActivity.EditValue = "C";
			cbDocNum.SelectedIndex = -1;
			_isSettingCbDocType = true;
			cbDocType.SelectedIndex = -1;
			_isSettingCbDocType = false;
			cbStatus.EditValue = -2;
			txtDayInMonth.Visible = false;
			radio_SubOption2.Visible = false;
			cbWeek.Visible = false;
			cbDayOfWeek.Visible = false;
			_TimeDurationTextChanging = true;
			_TimeDurationTextChanging = true;
			cbMonth.Visible = false;
			cbMonth1.Visible = false;
			lb_Of.Visible = false;
			txtStreet.Visible = false;
			txtCity.Visible = false;
			txtRoom.Visible = false;
			txtState.Visible = false;
			txtCountry.Visible = false;
			lb_Status.Visible = false;
			lb_Address.Visible = false;
			lb_Street.Visible = false;
			lb_City.Visible = false;
			lb_Room.Visible = false;
			lb_State.Visible = false;
			lb_Country.Visible = false;
			panel_Daily.Visible = false;
			panel_Range.Visible = false;
			txtInterval.EditValue = 1;
			txtAfterMaxOccur.EditValue = 1;
			_seStartDat = dStart_Recontact.DateTime;
			dSeStartDat.EditValue = _seStartDat;
			radio_SubOption.Visible = false;
			radio_SubOption1.Checked = true;
			radio_NoEndDate.Checked = true;
			chkMon.Visible = false;
			chkTue.Visible = false;
			chkWed.Visible = false;
			chkThu.Visible = false;
			chkFri.Visible = false;
			chkSat.Visible = false;
			chkSun.Visible = false;
			GetMonth(dateChange);
			GetDayOfMonth(dateChange);
			GetDay(dateChange);
			GetWeekOfMonth(dateChange);
			await LoadCb();
			Refresh();
			await GetTableDefault();
			GetImageButton();
			if (frmCalendar != null && CalUserId != null && CalStartTime != null)
			{
				_ = CalStartDate;
				_ = CalEndDate;
				if (CalEndTime != null && CalDuration != null)
				{
					GetCalendarTime();
					dStart_Recontact.DateTime = CalStartDate;
					dEnDate.DateTime = CalEndDate;
					if (CalUserId.Contains("U"))
					{
						cbUser.SelectedItem = "User";
						_isSettingCbAttendUser = true;
						cbAttendUser.EditValue = Function.ParseShort(CalUserId.Substring(1));
						_isSettingCbAttendUser = false;
					}
					else
					{
						cbUser.SelectedItem = "Employee";
						_isSettingCbAttendUser = true;
						cbAttendUser.EditValue = Function.ParseInt(CalUserId.Substring(1));
						_isSettingCbAttendUser = false;
					}
					goto IL_04ed;
				}
			}
			dStart_Recontact.DateTime = DateTime.Now.Date;
			dEnDate.DateTime = DateTime.Now.Date;
			GetTime();
			goto IL_04ed;
			IL_04ed:
			if (FormMode != Enums.FormViewModeType.Update)
			{
				deleteToolStripMenuItem.Visible = false;
			}
			if (!string.IsNullOrWhiteSpace(txtBpCode.Text))
			{
				relayActiviToolStripMenuItem.Visible = false;
			}
			_TimeDurationTextChanging = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetWeekOfMonth(DateTime dateChange)
	{
		try
		{
			if (dateChange == DateTime.Now.Date)
			{
				DateTime date = DateTime.Now.Date;
				weekOfMonth = (int)(date.Day + date.DayOfWeek) / 7 + 1;
			}
			else
			{
				weekOfMonth = (int)(dateChange.Day + dateChange.DayOfWeek) / 7 + 1;
			}
			switch (weekOfMonth)
			{
			case 1:
				cbWeek.SelectedItem = "first";
				break;
			case 2:
				cbWeek.SelectedItem = "second";
				break;
			case 3:
				cbWeek.SelectedItem = "third";
				break;
			case 4:
				cbWeek.SelectedItem = "fourth";
				break;
			case 5:
				cbWeek.SelectedItem = "last";
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetImageButton()
	{
		try
		{
			if (string.IsNullOrWhiteSpace(txtBpCode.Text))
			{
				txtBpCode.Properties.Buttons[1].Visible = false;
			}
			else
			{
				txtBpCode.Properties.Buttons[1].Visible = true;
			}
			if (cb_ContactPerson.SelectedItem != null)
			{
				if (string.IsNullOrWhiteSpace((cb_ContactPerson.SelectedItem != null) ? cb_ContactPerson.SelectedItem.ToString() : string.Empty))
				{
					cb_ContactPerson.Properties.Buttons[1].Visible = false;
				}
				else
				{
					cb_ContactPerson.Properties.Buttons[1].Visible = true;
				}
			}
			else
			{
				cb_ContactPerson.Properties.Buttons[1].Visible = false;
			}
			if (string.IsNullOrWhiteSpace((cbDocNum.SelectedItem != null) ? cbDocNum.SelectedItem.ToString() : string.Empty))
			{
				cbDocNum.Properties.Buttons[1].Visible = false;
			}
			else
			{
				cbDocNum.Properties.Buttons[1].Visible = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetTime()
	{
		try
		{
			DateTime now = DateTime.Now;
			_beginTime = now.ToString("HH:mm");
			_endTime = now.AddMinutes(15.0).ToString("HH:mm");
			txtBeginTime.EditValue = _beginTime;
			txtEndTime.EditValue = _endTime;
			txtDuration.EditValue = "15 Minutes";
			txtRemQty.EditValue = "15 Minutes";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetCalendarTime()
	{
		try
		{
			_ = DateTime.Now;
			_beginTime = CalStartTime;
			_endTime = CalEndTime;
			txtBeginTime.EditValue = _beginTime;
			txtEndTime.EditValue = _endTime;
			if (CalDurType == "M")
			{
				txtDuration.EditValue = CalDuration + " Minutes";
			}
			else if (CalDurType == "H")
			{
				txtDuration.EditValue = CalDuration + " Hours";
			}
			else
			{
				txtDuration.EditValue = CalDuration + " Days";
			}
			txtRemQty.EditValue = "15 Minutes";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetDay(DateTime dateChange)
	{
		try
		{
			dateChange = Function.ParseDateTime(dStart_Recontact.EditValue) ?? DateTime.MinValue;
			DateTime date = DateTime.Now.Date;
			if (dateChange == date)
			{
				day = (int)(date.DayOfWeek + 1);
			}
			else
			{
				day = (int)(dateChange.DayOfWeek + 1);
			}
			switch (day)
			{
			case 1:
				cbDayOfWeek.EditValue = "Sunday";
				chkSun.Checked = true;
				break;
			case 2:
				cbDayOfWeek.EditValue = "Monday";
				chkMon.Checked = true;
				break;
			case 3:
				cbDayOfWeek.EditValue = "Tuesday";
				chkTue.Checked = true;
				break;
			case 4:
				cbDayOfWeek.EditValue = "Wednesday";
				chkWed.Checked = true;
				break;
			case 5:
				cbDayOfWeek.EditValue = "Thursday";
				chkThu.Checked = true;
				break;
			case 6:
				cbDayOfWeek.EditValue = "Friday";
				chkFri.Checked = true;
				break;
			case 7:
				cbDayOfWeek.EditValue = "Saturday";
				chkSat.Checked = true;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetDayOfMonth(DateTime dateChange)
	{
		try
		{
			dateChange = Function.ParseDateTime(dStart_Recontact.EditValue) ?? DateTime.MinValue;
			DateTime date = DateTime.Now.Date;
			if (dateChange == date)
			{
				dayOfMonth = date.Day;
				txtDayInMonth.Text = Function.ToString(dayOfMonth);
			}
			else
			{
				dayOfMonth = dateChange.Day;
				txtDayInMonth.Text = Function.ToString(dayOfMonth);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetMonth(DateTime dateChange)
	{
		try
		{
			if (dateChange == DateTime.Now.Date)
			{
				month = DateTime.Now.Month;
			}
			else
			{
				month = dateChange.Month;
			}
			switch (month)
			{
			case 1:
				cbMonth.SelectedItem = "January";
				cbMonth1.SelectedItem = "January";
				break;
			case 2:
				cbMonth.SelectedItem = "February";
				cbMonth1.SelectedItem = "February";
				break;
			case 3:
				cbMonth.SelectedItem = "March";
				cbMonth1.SelectedItem = "March";
				break;
			case 4:
				cbMonth.SelectedItem = "April";
				cbMonth1.SelectedItem = "April";
				break;
			case 5:
				cbMonth.SelectedItem = "May";
				cbMonth1.SelectedItem = "May";
				break;
			case 6:
				cbMonth.SelectedItem = "June";
				cbMonth1.SelectedItem = "June";
				break;
			case 7:
				cbMonth.SelectedItem = "July";
				cbMonth1.SelectedItem = "July";
				break;
			case 8:
				cbMonth.SelectedItem = "August";
				cbMonth1.SelectedItem = "August";
				break;
			case 9:
				cbMonth.SelectedItem = "September";
				cbMonth1.SelectedItem = "September";
				break;
			case 10:
				cbMonth.SelectedItem = "October";
				cbMonth1.SelectedItem = "October";
				break;
			case 11:
				cbMonth.SelectedItem = "November";
				cbMonth1.SelectedItem = "November";
				break;
			case 12:
				cbMonth.SelectedItem = "December";
				cbMonth1.SelectedItem = "December";
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetTableDefault()
	{
		try
		{
			dtObject = await _httpClient.ActivityClient.GetDefaultTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private new void Refresh()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add)
			{
				GetDefaultItemcbAttendUser();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void GetDefaultItemcbAttendUser()
	{
		try
		{
			if (cbUser.SelectedItem == "User")
			{
				await GetListUser();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetNewClgCode()
	{
		try
		{
			_newClgCode = await _httpClient.ActivityClient.GetMaxActivity_ClgCode() + 1;
			txtNumber.Text = Function.ToString(_newClgCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadCb()
	{
		_ = 1;
		try
		{
			string madk = cbUser.SelectedItem.ToString();
			await GetRequesterByUser(madk);
			await GetStatus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetStatus()
	{
		try
		{
			DataTable dataTable = await _httpClient.ActivityClient.ActivityStatus_GetAll();
			cbStatus.Properties.NullText = "";
			cbStatus.Properties.Columns.Clear();
			cbStatus.Properties.Columns.Add(new LookUpColumnInfo("name", ""));
			cbStatus.Properties.DisplayMember = "name";
			cbStatus.Properties.ValueMember = "statusID";
			cbStatus.Properties.DataSource = dataTable;
			SetLookupProperties(cbStatus);
			if (dataTable.IsNotNull() && !dataTable.AsEnumerable().Any((DataRow r) => Function.ParseShort(r["statusID"]) == 0))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["statusID"] = Function.ParseShort(0);
				dataRow["name"] = "Define New";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetListContactPerson()
	{
		string cardCode = txtBpCode.Text;
		try
		{
			DataTable dataTable = await _httpClient.ActivityClient.BusinessPartner_GetListContactPerson(cardCode);
			_listContactPerson = new List<Common.Table_Code>();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				Common.Table_Code table_Code = new Common.Table_Code();
				table_Code.Code = dataTable.Rows[i]["CntctCode"].ToString();
				table_Code.Name = dataTable.Rows[i]["Name"].ToString();
				_listContactPerson.Add(table_Code);
			}
			WinFuntions._addComboBoxBlank(cb_ContactPerson, _listContactPerson, _blank: true);
			if (FormMode != Enums.FormViewModeType.Find)
			{
				WinFuntions._selectItemComboBoxByIndex(cb_ContactPerson, 1);
			}
			if (cb_ContactPerson.SelectedItem != null && cb_ContactPerson.SelectedItem != "")
			{
				string value = ((Common.ListItems)cb_ContactPerson.SelectedItem).Value;
				if (string.IsNullOrEmpty(value) || value == "-1")
				{
					cb_ContactPerson.Properties.Buttons[1].Visible = false;
				}
				else
				{
					cb_ContactPerson.Properties.Buttons[1].Visible = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GetActivitySubject(string madk = "")
	{
		try
		{
			if (madk == "")
			{
				madk = Function.ToString(luk_Type.EditValue);
			}
			return await _httpClient.ActivityClient.ActivitySubject_GetAll(madk);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private async Task<DataTable> GetActivityType()
	{
		try
		{
			return await _httpClient.ActivityClient.ActivityType_GetAll();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private async Task GetRequesterByUser(string madk)
	{
		_ = 1;
		try
		{
			if (madk == "User")
			{
				await GetListUser();
			}
			else
			{
				await GetListEmployee();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetListEmployee()
	{
		try
		{
			DataTable dataTable = await _httpClient.ActivityClient.Requester_GetRequesterByEmployee();
			cbAttendUser.Properties.DataSource = dataTable;
			if (cbUser.Text == "Employee" && FormMode != Enums.FormViewModeType.Find && dataTable.IsNotNull())
			{
				_isSettingCbAttendUser = true;
				cbAttendUser.EditValue = Function.ParseInt(dataTable.Rows[0]["EmpID"]);
				_isSettingCbAttendUser = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetListUser()
	{
		try
		{
			DataTable dataTable = await _httpClient.ActivityClient.Requester_GetUser_tableOUSR();
			if (dataTable.IsNotNull())
			{
				if (dataTable.Columns.Contains("USERID"))
				{
					dataTable.Columns["USERID"].ColumnName = "EmpID";
				}
				if (dataTable.Columns.Contains("U_NAME"))
				{
					dataTable.Columns["U_NAME"].ColumnName = "ReqName";
				}
			}
			dataTable.AcceptChanges();
			cbAttendUser.Properties.DataSource = dataTable;
			if (cbUser.Text == "User" && FormMode != Enums.FormViewModeType.Find && dataTable.IsNotNull())
			{
				_isSettingCbAttendUser = true;
				cbAttendUser.EditValue = Function.ParseShort(dataTable.Rows[0]["EmpID"]);
				_isSettingCbAttendUser = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GetActivityMeetingLocation()
	{
		try
		{
			return await _httpClient.ActivityClient.ActivityMeetingLocation_GetAll();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private void SetLookupProperties(LookUpEdit luk)
	{
		try
		{
			luk.Properties.ShowHeader = false;
			luk.Properties.ShowFooter = false;
			luk.Properties.AutoHeight = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetUpdateButton(object sender)
	{
		try
		{
			if (!_FormLoading && !_isLoadingData && !_isSettingCbAttendUser && !_isSettingCbDocType)
			{
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
				else if (FormMode == Enums.FormViewModeType.Add && !_isChangedValueInModeAdd)
				{
					_isChangedValueInModeAdd = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetRecord(string position, int clgCode)
	{
		_ = 4;
		try
		{
			bool flag = true;
			if ((FormMode == Enums.FormViewModeType.Update || _isChangedValueInModeAdd) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				flag = false;
			}
			if (flag)
			{
				int retClgCode = await _httpClient.ActivityClient.GetRecord(position, clgCode);
				if (retClgCode > 0)
				{
					DataTable searchDataTable = GetSearchDataTable();
					DataRow dataRow = searchDataTable.NewRow();
					dataRow["ClgCode"] = retClgCode;
					searchDataTable.Rows.Add(dataRow);
					await Search(searchDataTable);
					await ucAttachment1.LoadAttachments(retClgCode);
				}
				else if (position == "N")
				{
					await GetRecord("F", 0);
				}
				else if (position == "P")
				{
					await GetRecord("L", 0);
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<bool> ExistsCode(int clgCode)
	{
		try
		{
			return await _httpClient.ActivityClient.CheckDoc(clgCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	private async Task ReloadComboSubject()
	{
		try
		{
			DataTable dataTable = await GetActivitySubject();
			if (dataTable != null && dataTable.Columns.Count > 0)
			{
				if (!dataTable.AsEnumerable().Any((DataRow r) => Function.ParseShort(r["Code"]) == -10))
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["Code"] = Function.ParseShort(-10);
					dataRow["Name"] = "";
					dataTable.Rows.InsertAt(dataRow, 0);
				}
				if (!dataTable.AsEnumerable().Any((DataRow r) => Function.ParseShort(r["Code"]) == 0))
				{
					DataRow dataRow2 = dataTable.NewRow();
					dataRow2["Code"] = Function.ParseShort(0);
					dataRow2["Name"] = "Define New";
					dataTable.Rows.Add(dataRow2);
				}
				dataTable.AcceptChanges();
			}
			SetDataSourceByDataTable(luk_Subject, dataTable);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetFirst()
	{
		_ = 2;
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ActivitiesForOpportunity");
			DataTable dataTable = await _helper.GetAsync("GetFirstRecord");
			if (dataTable != null)
			{
				DataTable searchDataTable = GetSearchDataTable();
				DataRow dr = searchDataTable.NewRow();
				dr["ClgCode"] = dataTable.Rows[0]["ClgCode"].ToString();
				searchDataTable.Rows.Add(dr);
				await Search(searchDataTable);
				await ucAttachment1.LoadAttachments(Function.ParseInt(dr["ClgCode"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetLast()
	{
		_ = 2;
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ActivitiesForOpportunity");
			DataTable dataTable = await _helper.GetAsync("GetLastRecord");
			if (dataTable != null)
			{
				DataTable searchDataTable = GetSearchDataTable();
				DataRow dr = searchDataTable.NewRow();
				dr["ClgCode"] = dataTable.Rows[0]["ClgCode"].ToString();
				searchDataTable.Rows.Add(dr);
				await Search(searchDataTable);
				await ucAttachment1.LoadAttachments(Function.ParseInt(dr["ClgCode"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetNext()
	{
		string id = txtNumber.Text;
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ActivitiesForOpportunity");
			DataTable dataTable = await _helper.GetDataTableAsync("GetNextRecord", new { id });
			if (dataTable != null)
			{
				DataTable searchDataTable = GetSearchDataTable();
				DataRow dr = searchDataTable.NewRow();
				dr["ClgCode"] = dataTable.Rows[0]["ClgCode"].ToString();
				searchDataTable.Rows.Add(dr);
				await Search(searchDataTable);
				await ucAttachment1.LoadAttachments(Function.ParseInt(dr["ClgCode"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetPrevious()
	{
		string id = txtNumber.Text;
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ActivitiesForOpportunity");
			DataTable dataTable = await _helper.GetDataTableAsync("GetPrevious", new { id });
			if (dataTable != null)
			{
				DataTable searchDataTable = GetSearchDataTable();
				DataRow dr = searchDataTable.NewRow();
				dr["ClgCode"] = dataTable.Rows[0]["ClgCode"].ToString();
				searchDataTable.Rows.Add(dr);
				await Search(searchDataTable);
				await ucAttachment1.LoadAttachments(Function.ParseInt(dr["ClgCode"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH_DATA()
	{
		_isLoadingData = true;
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					await LoadLookupDatasource();
					await LoadOneDoc(Function.ParseInt(txtNumber.Text));
				}
			}
			else if (FormMode == Enums.FormViewModeType.View)
			{
				await LoadLookupDatasource();
				await LoadOneDoc(Function.ParseInt(txtNumber.Text));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_FormLoading = false;
		_isLoadingData = false;
	}

	public async Task MODE_FIND()
	{
		_isLoadingData = true;
		try
		{
			bool flag = true;
			if ((FormMode == Enums.FormViewModeType.Update || _isChangedValueInModeAdd) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) != DialogResult.Yes)
			{
				flag = false;
			}
			if (flag)
			{
				_isChangedValueInModeAdd = false;
				FormMode = Enums.FormViewModeType.Find;
				await ResetControl();
				txtNumber.ReadOnly = false;
				GetImageButton();
				SetControlsBackColor();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_FormLoading = false;
		_isLoadingData = false;
	}

	public async Task REFRESH()
	{
		_isLoadingData = true;
		try
		{
			bool flag = true;
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) != DialogResult.Yes)
			{
				flag = false;
			}
			if (flag)
			{
				await LoadLookupDatasource();
				FormMode = Enums.FormViewModeType.Add;
				SetControlsBackColor();
				await ResetControl();
				await GetNewClgCode();
				txtNumber.ReadOnly = true;
				string userCodeLogin = GlobalData.UserCodeLogin;
				if (cbUser.Text == "User")
				{
					_isSettingCbAttendUser = true;
					cbAttendUser.EditValue = Function.ParseShort(userCodeLogin);
					_isSettingCbAttendUser = false;
				}
				else
				{
					_isSettingCbAttendUser = true;
					cbAttendUser.EditValue = Function.ParseInt(userCodeLogin);
					_isSettingCbAttendUser = false;
				}
				await LoadData();
				_isChangedValueInModeAdd = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_FormLoading = false;
		_isLoadingData = false;
	}

	public async Task MOVE_FIRST()
	{
		_isLoadingData = true;
		try
		{
			await GetRecord("F", 0);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoadingData = false;
	}

	public async Task MOVE_NEXT()
	{
		_isLoadingData = true;
		try
		{
			int code = Function.ParseInt(txtNumber.Text);
			bool flag = code > 0;
			if (flag)
			{
				flag = await ExistsCode(code);
			}
			if (!flag)
			{
				await GetRecord("L", 0);
			}
			else
			{
				await GetRecord("N", code);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoadingData = false;
	}

	public async Task MOVE_LAST()
	{
		_isLoadingData = true;
		try
		{
			await GetRecord("L", 0);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoadingData = false;
	}

	public async Task MOVE_PREVIOUS()
	{
		_isLoadingData = true;
		try
		{
			int code = Function.ParseInt(txtNumber.Text);
			bool flag = code > 0;
			if (flag)
			{
				flag = await ExistsCode(code);
			}
			if (!flag)
			{
				await GetRecord("F", 0);
			}
			else
			{
				await GetRecord("P", code);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoadingData = false;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.SalesOppotunity.frm_ActivitiesForOpportunity));
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		this.txtName = new Apzon.Controls.Editor.ButtonEditEx();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.gcAllTotal = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAllVatSum = new Apzon.Controls.Grid.GridColumnEx();
		this.addNewToolStripMenuItem_Document = new System.Windows.Forms.ToolStripMenuItem();
		this.cmtDuplicate_Return = new System.Windows.Forms.ToolStripMenuItem();
		this.cmtClose_Doc = new System.Windows.Forms.ToolStripMenuItem();
		this.ctmCancel_Doc = new System.Windows.Forms.ToolStripMenuItem();
		this.saveAsDraftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem_Documents = new System.Windows.Forms.ToolStripMenuItem();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.txtNo = new Apzon.Controls.Editor.TextBoxEx();
		this.cbRequester = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.btSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.fDate = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.tDate = new Apzon.Controls.Editor.DateEditEx();
		this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
		this.xtraTabPage2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.gcDocAllTotal = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocAllVat = new Apzon.Controls.Grid.GridColumnEx();
		this.panel1 = new System.Windows.Forms.Panel();
		this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
		this.textEdit1 = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.simpleButton1 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.dateEdit1 = new Apzon.Controls.Editor.DateEditEx();
		this.comboBoxEdit1 = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.dateEdit2 = new Apzon.Controls.Editor.DateEditEx();
		this.txtCardCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.gcCusName = new Apzon.Controls.Grid.GridColumnEx();
		this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem5 = new DevExpress.XtraBars.BarButtonItem();
		this.drpBtCoppyTo = new DevExpress.XtraEditors.DropDownButton();
		this.barDockControl2 = new DevExpress.XtraBars.BarDockControl();
		this.xtraTabPage3 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.buttonEdit1 = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.luk_Subject = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_Type = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.cb_ContactPerson = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.txtAssignedBy = new Apzon.Controls.Editor.TextEditEx();
		this.txtTelePhone = new Apzon.Controls.Editor.TextEditEx();
		this.btAdd = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.Xtap = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tap_General = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txtState = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.txtCountry = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.txtRoom = new Apzon.Controls.Editor.TextEditEx();
		this.txtCity = new Apzon.Controls.Editor.TextEditEx();
		this.txtStreet = new Apzon.Controls.Editor.TextEditEx();
		this.txtDetails = new Apzon.Controls.Editor.TextEditEx();
		this.cbStatus = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.cbRecurPat = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_Meetting = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.panel_Range = new System.Windows.Forms.Panel();
		this.lb_End = new Apzon.Controls.Controls.LabelLine();
		this.lb_Start = new Apzon.Controls.Controls.LabelLine();
		this.lb_Occurrence = new DevExpress.XtraEditors.LabelControl();
		this.txtAfterMaxOccur = new Apzon.Controls.Editor.TextEditEx();
		this.radio_By = new System.Windows.Forms.RadioButton();
		this.radio_After = new System.Windows.Forms.RadioButton();
		this.radio_NoEndDate = new System.Windows.Forms.RadioButton();
		this.lb_Range = new DevExpress.XtraEditors.LabelControl();
		this.labelLine32 = new Apzon.Controls.Controls.LabelLine();
		this.txtBySeEndDat = new Apzon.Controls.Editor.DateEditEx();
		this.dSeStartDat = new Apzon.Controls.Editor.DateEditEx();
		this.txtRemQty = new Apzon.Controls.Editor.TextEditEx();
		this.btFollowUp = new Apzon.Controls.Controls.SimpleButtonEx();
		this.chk_Tentative = new Apzon.Controls.Editor.CheckEditEx();
		this.chkInactive = new Apzon.Controls.Editor.CheckEditEx();
		this.chkClosed = new Apzon.Controls.Editor.CheckEditEx();
		this.chkReminder = new Apzon.Controls.Editor.CheckEditEx();
		this.panel_Daily = new System.Windows.Forms.Panel();
		this.chkSun = new Apzon.Controls.Editor.CheckEditEx();
		this.cbMonth = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.cbMonth1 = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.chkSat = new Apzon.Controls.Editor.CheckEditEx();
		this.cbDayOfWeek = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.cbWeek = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.chkFri = new Apzon.Controls.Editor.CheckEditEx();
		this.txtDayInMonth = new Apzon.Controls.Editor.TextEditEx();
		this.radio_SubOption2 = new System.Windows.Forms.RadioButton();
		this.chkThu = new Apzon.Controls.Editor.CheckEditEx();
		this.label_RepeatEvery = new System.Windows.Forms.Label();
		this.txtInterval = new Apzon.Controls.Editor.TextEditEx();
		this.chkWed = new Apzon.Controls.Editor.CheckEditEx();
		this.radio_SubOption1 = new System.Windows.Forms.RadioButton();
		this.lb_Of = new Apzon.Controls.Controls.LabelLine();
		this.chkTue = new Apzon.Controls.Editor.CheckEditEx();
		this.lb_RepeatOn = new Apzon.Controls.Controls.LabelLine();
		this.lb_RepeatEvery = new Apzon.Controls.Controls.LabelLine();
		this.chkMon = new Apzon.Controls.Editor.CheckEditEx();
		this.radio_SubOption = new System.Windows.Forms.RadioButton();
		this.lb_Recurrence = new Apzon.Controls.Controls.LabelLine();
		this.cbPriority = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.txtDuration = new Apzon.Controls.Editor.TextEditEx();
		this.txtEndTime = new Apzon.Controls.Editor.TextEditEx();
		this.txtBeginTime = new Apzon.Controls.Editor.TextEditEx();
		this.dEnDate = new Apzon.Controls.Editor.DateEditEx();
		this.dStart_Recontact = new Apzon.Controls.Editor.DateEditEx();
		this.label_MettingLocation = new Apzon.Controls.Controls.LabelLine();
		this.label_Priority = new Apzon.Controls.Controls.LabelLine();
		this.lb_Country = new Apzon.Controls.Controls.LabelLine();
		this.lb_Room = new Apzon.Controls.Controls.LabelLine();
		this.lb_State = new Apzon.Controls.Controls.LabelLine();
		this.lb_City = new Apzon.Controls.Controls.LabelLine();
		this.lb_Street = new Apzon.Controls.Controls.LabelLine();
		this.lb_Address = new Apzon.Controls.Controls.LabelLine();
		this.lb_Status = new Apzon.Controls.Controls.LabelLine();
		this.lb_Duration = new Apzon.Controls.Controls.LabelLine();
		this.lb_EndTime = new Apzon.Controls.Controls.LabelLine();
		this.lb_StartTime = new Apzon.Controls.Controls.LabelLine();
		this.lb_Remarks = new Apzon.Controls.Controls.LabelLine();
		this.tab_Content = new Apzon.Controls.Controls.XtraTabPageEx();
		this.memoEdit_Note = new Apzon.Controls.Editor.MemoEditEx();
		this.tap_LinkedDocument = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txtParentType = new Apzon.Controls.Editor.TextEditEx();
		this.txtParentId = new Apzon.Controls.Editor.TextEditEx();
		this.txtPrevActivity = new Apzon.Controls.Editor.TextEditEx();
		this.cbDocNum = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.cbDocType = new Apzon.Forms.Commons.SeparatorComboBox();
		this.chkShowDocumentRelatedBP = new Apzon.Controls.Editor.CheckEditEx();
		this.chkLinkDraft = new Apzon.Controls.Editor.CheckEditEx();
		this.lb_SourceObjectNo = new Apzon.Controls.Controls.LabelLine();
		this.lb_SourceObjectType = new Apzon.Controls.Controls.LabelLine();
		this.lb_PreviousActivity = new Apzon.Controls.Controls.LabelLine();
		this.lb_DocumentNumber = new Apzon.Controls.Controls.LabelLine();
		this.lb_DocumentType = new Apzon.Controls.Controls.LabelLine();
		this.tap_Att = new Apzon.Controls.Controls.XtraTabPageEx();
		this.ucAttachment1 = new Apzon.Forms.UserControls.ucAttachment();
		this.chkPerson = new Apzon.Controls.Editor.CheckEditEx();
		this.lb_Activity = new Apzon.Controls.Controls.LabelLine();
		this.lb_Type = new Apzon.Controls.Controls.LabelLine();
		this.lb_TelephoneNo = new Apzon.Controls.Controls.LabelLine();
		this.lb_ContactPerson = new Apzon.Controls.Controls.LabelLine();
		this.lb_Subject = new Apzon.Controls.Controls.LabelLine();
		this.lb_BPName = new Apzon.Controls.Controls.LabelLine();
		this.lb_BPCode = new Apzon.Controls.Controls.LabelLine();
		this.lb_AssignedTo = new Apzon.Controls.Controls.LabelLine();
		this.lb_Number = new Apzon.Controls.Controls.LabelLine();
		this.txtBpCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.lb_AssignedBy = new Apzon.Controls.Controls.LabelLine();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.relayActiviToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.txtBpName = new Apzon.Controls.Editor.TextEditEx();
		this.txtBpType = new Apzon.Controls.Editor.TextEditEx();
		this.txtNumber = new Apzon.Controls.Editor.TextEditEx();
		this.cbUser = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.cbActivity = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.cbAttendUser = new Apzon.Forms.UserControls.LookUpEditCombo();
		((System.ComponentModel.ISupportInitialize)this.txtName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbRequester.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateEdit1.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateEdit1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.comboBoxEdit1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateEdit2.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateEdit2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCardCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.buttonEdit1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Subject.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Type.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_ContactPerson.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtAssignedBy.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtTelePhone.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Xtap).BeginInit();
		this.Xtap.SuspendLayout();
		this.tap_General.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtState.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCountry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtRoom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCity.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtStreet.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtDetails.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbStatus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbRecurPat.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Meetting.Properties).BeginInit();
		this.panel_Range.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtAfterMaxOccur.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBySeEndDat.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBySeEndDat.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dSeStartDat.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dSeStartDat.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtRemQty.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Tentative.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkInactive.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkClosed.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkReminder.Properties).BeginInit();
		this.panel_Daily.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chkSun.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbMonth.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbMonth1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkSat.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbDayOfWeek.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbWeek.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkFri.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtDayInMonth.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkThu.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtInterval.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkWed.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkTue.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkMon.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbPriority.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtDuration.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtEndTime.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBeginTime.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dEnDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dEnDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dStart_Recontact.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dStart_Recontact.Properties).BeginInit();
		this.tab_Content.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.memoEdit_Note.Properties).BeginInit();
		this.tap_LinkedDocument.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtParentType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtParentId.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPrevActivity.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbDocNum.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkShowDocumentRelatedBP.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkLinkDraft.Properties).BeginInit();
		this.tap_Att.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chkPerson.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBpCode.Properties).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtBpName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBpType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtNumber.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbUser.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbActivity.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbAttendUser.Properties).BeginInit();
		base.SuspendLayout();
		this.txtName.EditControl = null;
		this.txtName.FieldName = null;
		this.txtName.IsSearchControl = false;
		this.txtName.Location = new System.Drawing.Point(0, 0);
		this.txtName.Name = "txtName";
		this.txtName.SettingInVisible = false;
		this.txtName.SettingReadonly = false;
		this.txtName.Size = new System.Drawing.Size(100, 20);
		this.txtName.TabIndex = 0;
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Location = new System.Drawing.Point(0, 0);
		this.barDockControlRight.Manager = null;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 0);
		this.gcAllTotal.CurrencyDateField = null;
		this.gcAllTotal.CurrencyField = null;
		this.gcAllTotal.CurrencyFixed = null;
		this.gcAllTotal.CurrencyFixedRate = 0.0;
		this.gcAllTotal.CurrencyRateField = null;
		this.gcAllTotal.Name = "gcAllTotal";
		this.gcAllTotal.PrimaryKey = false;
		this.gcAllTotal.UnitField = null;
		this.gcAllVatSum.CurrencyDateField = null;
		this.gcAllVatSum.CurrencyField = null;
		this.gcAllVatSum.CurrencyFixed = null;
		this.gcAllVatSum.CurrencyFixedRate = 0.0;
		this.gcAllVatSum.CurrencyRateField = null;
		this.gcAllVatSum.Name = "gcAllVatSum";
		this.gcAllVatSum.PrimaryKey = false;
		this.gcAllVatSum.UnitField = null;
		this.addNewToolStripMenuItem_Document.Name = "addNewToolStripMenuItem_Document";
		this.addNewToolStripMenuItem_Document.Size = new System.Drawing.Size(32, 19);
		this.cmtDuplicate_Return.Name = "cmtDuplicate_Return";
		this.cmtDuplicate_Return.Size = new System.Drawing.Size(32, 19);
		this.cmtClose_Doc.Name = "cmtClose_Doc";
		this.cmtClose_Doc.Size = new System.Drawing.Size(32, 19);
		this.ctmCancel_Doc.Name = "ctmCancel_Doc";
		this.ctmCancel_Doc.Size = new System.Drawing.Size(32, 19);
		this.saveAsDraftToolStripMenuItem.Name = "saveAsDraftToolStripMenuItem";
		this.saveAsDraftToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
		this.deleteToolStripMenuItem_Documents.Name = "deleteToolStripMenuItem_Documents";
		this.deleteToolStripMenuItem_Documents.Size = new System.Drawing.Size(32, 19);
		this.groupControl1.Location = new System.Drawing.Point(0, 0);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(200, 100);
		this.groupControl1.TabIndex = 0;
		this.txtNo.EditControl = null;
		this.txtNo.EditValue = "";
		this.txtNo.FieldName = null;
		this.txtNo.IsSearchControl = false;
		this.txtNo.Location = new System.Drawing.Point(0, 0);
		this.txtNo.Name = "txtNo";
		this.txtNo.SettingInVisible = false;
		this.txtNo.SettingReadonly = false;
		this.txtNo.Size = new System.Drawing.Size(100, 20);
		this.txtNo.TabIndex = 0;
		this.cbRequester.EditControl = null;
		this.cbRequester.FieldName = null;
		this.cbRequester.IsSearchControl = false;
		this.cbRequester.Location = new System.Drawing.Point(0, 0);
		this.cbRequester.Name = "cbRequester";
		this.cbRequester.SettingInVisible = false;
		this.cbRequester.SettingReadonly = false;
		this.cbRequester.Size = new System.Drawing.Size(100, 20);
		this.cbRequester.TabIndex = 0;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "labelControl1";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(0, 0);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "labelControl1";
		this.btSearch.EditControl = null;
		this.btSearch.EditValue = "";
		this.btSearch.FieldName = null;
		this.btSearch.IsSearchControl = false;
		this.btSearch.Location = new System.Drawing.Point(0, 0);
		this.btSearch.Name = "btSearch";
		this.btSearch.ReadOnly = false;
		this.btSearch.SettingInVisible = false;
		this.btSearch.SettingReadonly = false;
		this.btSearch.Size = new System.Drawing.Size(75, 23);
		this.btSearch.TabIndex = 0;
		this.fDate.EditControl = null;
		this.fDate.EditValue = new System.DateTime(2015, 8, 25, 0, 0, 0, 0);
		this.fDate.FieldName = null;
		this.fDate.Location = new System.Drawing.Point(0, 0);
		this.fDate.Name = "fDate";
		this.fDate.Properties.IsSearchControl = false;
		this.fDate.SettingInVisible = false;
		this.fDate.SettingReadonly = false;
		this.fDate.Size = new System.Drawing.Size(100, 20);
		this.fDate.TabIndex = 0;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "labelControl1";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(0, 0);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 0;
		this.labelLine2.Utext = "labelControl1";
		this.tDate.EditControl = null;
		this.tDate.EditValue = new System.DateTime(2015, 8, 25, 0, 0, 0, 0);
		this.tDate.FieldName = null;
		this.tDate.Location = new System.Drawing.Point(0, 0);
		this.tDate.Name = "tDate";
		this.tDate.Properties.IsSearchControl = false;
		this.tDate.SettingInVisible = false;
		this.tDate.SettingReadonly = false;
		this.tDate.Size = new System.Drawing.Size(100, 20);
		this.tDate.TabIndex = 0;
		this.barDockControl1.CausesValidation = false;
		this.barDockControl1.Location = new System.Drawing.Point(0, 0);
		this.barDockControl1.Manager = null;
		this.barDockControl1.Size = new System.Drawing.Size(0, 0);
		this.xtraTabPage2.EditControl = null;
		this.xtraTabPage2.EditValue = "";
		this.xtraTabPage2.FieldName = null;
		this.xtraTabPage2.IsSearchControl = false;
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.ReadOnly = false;
		this.xtraTabPage2.SettingInVisible = false;
		this.xtraTabPage2.SettingReadonly = false;
		this.xtraTabPage2.Size = new System.Drawing.Size(0, 0);
		this.xtraTabPage2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.gcDocAllTotal.CurrencyDateField = null;
		this.gcDocAllTotal.CurrencyField = null;
		this.gcDocAllTotal.CurrencyFixed = null;
		this.gcDocAllTotal.CurrencyFixedRate = 0.0;
		this.gcDocAllTotal.CurrencyRateField = null;
		this.gcDocAllTotal.Name = "gcDocAllTotal";
		this.gcDocAllTotal.PrimaryKey = false;
		this.gcDocAllTotal.UnitField = null;
		this.gcDocAllVat.CurrencyDateField = null;
		this.gcDocAllVat.CurrencyField = null;
		this.gcDocAllVat.CurrencyFixed = null;
		this.gcDocAllVat.CurrencyFixedRate = 0.0;
		this.gcDocAllVat.CurrencyRateField = null;
		this.gcDocAllVat.Name = "gcDocAllVat";
		this.gcDocAllVat.PrimaryKey = false;
		this.gcDocAllVat.UnitField = null;
		this.panel1.Location = new System.Drawing.Point(0, 0);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(200, 100);
		this.panel1.TabIndex = 0;
		this.groupControl2.Location = new System.Drawing.Point(0, 0);
		this.groupControl2.Name = "groupControl2";
		this.groupControl2.Size = new System.Drawing.Size(200, 100);
		this.groupControl2.TabIndex = 0;
		this.textEdit1.EditControl = null;
		this.textEdit1.FieldName = null;
		this.textEdit1.IsSearchControl = false;
		this.textEdit1.Location = new System.Drawing.Point(0, 0);
		this.textEdit1.Name = "textEdit1";
		this.textEdit1.SettingInVisible = false;
		this.textEdit1.SettingReadonly = false;
		this.textEdit1.Size = new System.Drawing.Size(100, 20);
		this.textEdit1.TabIndex = 0;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "labelControl1";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(0, 0);
		this.labelLine4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(80, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 0;
		this.labelLine4.Utext = "labelControl1";
		this.simpleButton1.EditControl = null;
		this.simpleButton1.EditValue = "";
		this.simpleButton1.FieldName = null;
		this.simpleButton1.IsSearchControl = false;
		this.simpleButton1.Location = new System.Drawing.Point(0, 0);
		this.simpleButton1.Name = "simpleButton1";
		this.simpleButton1.ReadOnly = false;
		this.simpleButton1.SettingInVisible = false;
		this.simpleButton1.SettingReadonly = false;
		this.simpleButton1.Size = new System.Drawing.Size(75, 23);
		this.simpleButton1.TabIndex = 0;
		this.dateEdit1.EditControl = null;
		this.dateEdit1.EditValue = new System.DateTime(2015, 8, 25, 0, 0, 0, 0);
		this.dateEdit1.FieldName = null;
		this.dateEdit1.Location = new System.Drawing.Point(0, 0);
		this.dateEdit1.Name = "dateEdit1";
		this.dateEdit1.Properties.IsSearchControl = false;
		this.dateEdit1.SettingInVisible = false;
		this.dateEdit1.SettingReadonly = false;
		this.dateEdit1.Size = new System.Drawing.Size(100, 20);
		this.dateEdit1.TabIndex = 0;
		this.comboBoxEdit1.EditControl = null;
		this.comboBoxEdit1.FieldName = null;
		this.comboBoxEdit1.IsSearchControl = false;
		this.comboBoxEdit1.Location = new System.Drawing.Point(0, 0);
		this.comboBoxEdit1.Name = "comboBoxEdit1";
		this.comboBoxEdit1.SettingInVisible = false;
		this.comboBoxEdit1.SettingReadonly = false;
		this.comboBoxEdit1.Size = new System.Drawing.Size(100, 20);
		this.comboBoxEdit1.TabIndex = 0;
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "labelControl1";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(0, 0);
		this.labelLine6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(80, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 0;
		this.labelLine6.Utext = "labelControl1";
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "labelControl1";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(0, 0);
		this.labelLine7.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(80, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 0;
		this.labelLine7.Utext = "labelControl1";
		this.dateEdit2.EditControl = null;
		this.dateEdit2.EditValue = new System.DateTime(2015, 8, 25, 0, 0, 0, 0);
		this.dateEdit2.FieldName = null;
		this.dateEdit2.Location = new System.Drawing.Point(0, 0);
		this.dateEdit2.Name = "dateEdit2";
		this.dateEdit2.Properties.IsSearchControl = false;
		this.dateEdit2.SettingInVisible = false;
		this.dateEdit2.SettingReadonly = false;
		this.dateEdit2.Size = new System.Drawing.Size(100, 20);
		this.dateEdit2.TabIndex = 0;
		this.txtCardCode.EditControl = null;
		this.txtCardCode.FieldName = null;
		this.txtCardCode.IsSearchControl = false;
		this.txtCardCode.Location = new System.Drawing.Point(0, 0);
		this.txtCardCode.Name = "txtCardCode";
		this.txtCardCode.SettingInVisible = false;
		this.txtCardCode.SettingReadonly = false;
		this.txtCardCode.Size = new System.Drawing.Size(100, 20);
		this.txtCardCode.TabIndex = 0;
		this.labelLine8.EditControl = null;
		this.labelLine8.EditValue = "labelControl1";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(0, 0);
		this.labelLine8.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ReadOnly = false;
		this.labelLine8.SettingInVisible = false;
		this.labelLine8.SettingReadonly = false;
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(80, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 0;
		this.labelLine8.Utext = "labelControl1";
		this.labelLine9.EditControl = null;
		this.labelLine9.EditValue = "labelControl1";
		this.labelLine9.FieldName = null;
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(0, 0);
		this.labelLine9.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ReadOnly = false;
		this.labelLine9.SettingInVisible = false;
		this.labelLine9.SettingReadonly = false;
		this.labelLine9.ShowUnderLine = true;
		this.labelLine9.Size = new System.Drawing.Size(80, 20);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 0;
		this.labelLine9.Utext = "labelControl1";
		this.gcCusName.CurrencyDateField = null;
		this.gcCusName.CurrencyField = null;
		this.gcCusName.CurrencyFixed = null;
		this.gcCusName.CurrencyFixedRate = 0.0;
		this.gcCusName.CurrencyRateField = null;
		this.gcCusName.Name = "gcCusName";
		this.gcCusName.PrimaryKey = false;
		this.gcCusName.UnitField = null;
		this.barButtonItem1.Name = "barButtonItem1";
		this.barButtonItem2.Name = "barButtonItem2";
		this.barButtonItem5.Name = "barButtonItem5";
		this.drpBtCoppyTo.Location = new System.Drawing.Point(0, 0);
		this.drpBtCoppyTo.Name = "drpBtCoppyTo";
		this.drpBtCoppyTo.Size = new System.Drawing.Size(135, 23);
		this.drpBtCoppyTo.TabIndex = 0;
		this.barDockControl2.CausesValidation = false;
		this.barDockControl2.Location = new System.Drawing.Point(0, 0);
		this.barDockControl2.Manager = null;
		this.barDockControl2.Size = new System.Drawing.Size(0, 0);
		this.xtraTabPage3.EditControl = null;
		this.xtraTabPage3.EditValue = "";
		this.xtraTabPage3.FieldName = null;
		this.xtraTabPage3.IsSearchControl = false;
		this.xtraTabPage3.Name = "xtraTabPage3";
		this.xtraTabPage3.ReadOnly = false;
		this.xtraTabPage3.SettingInVisible = false;
		this.xtraTabPage3.SettingReadonly = false;
		this.xtraTabPage3.Size = new System.Drawing.Size(0, 0);
		this.xtraTabPage3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.buttonEdit1.EditControl = null;
		this.buttonEdit1.FieldName = null;
		this.buttonEdit1.IsSearchControl = false;
		this.buttonEdit1.Location = new System.Drawing.Point(0, 0);
		this.buttonEdit1.Name = "buttonEdit1";
		this.buttonEdit1.SettingInVisible = false;
		this.buttonEdit1.SettingReadonly = false;
		this.buttonEdit1.Size = new System.Drawing.Size(100, 20);
		this.buttonEdit1.TabIndex = 0;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "labelControl1";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(0, 0);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 0;
		this.labelLine3.Utext = "labelControl1";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "labelControl1";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(0, 0);
		this.labelLine5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(80, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 0;
		this.labelLine5.Utext = "labelControl1";
		this.luk_Subject.EditControl = null;
		this.luk_Subject.FieldName = null;
		this.luk_Subject.IsSearchControl = false;
		this.luk_Subject.Location = new System.Drawing.Point(119, 40);
		this.luk_Subject.Name = "luk_Subject";
		this.luk_Subject.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_Subject.Properties.Appearance.Options.UseFont = true;
		this.luk_Subject.Properties.AutoHeight = false;
		this.luk_Subject.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Subject.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_Subject.Properties.DisplayMember = "Name";
		this.luk_Subject.Properties.IsSearchControl = false;
		this.luk_Subject.Properties.NullText = "";
		this.luk_Subject.Properties.ValueMember = "Code";
		this.luk_Subject.SettingInVisible = false;
		this.luk_Subject.SettingReadonly = false;
		this.luk_Subject.Size = new System.Drawing.Size(214, 18);
		this.luk_Subject.TabIndex = 44;
		this.luk_Subject.Popup += new System.EventHandler(this.luk_Popup);
		this.luk_Subject.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.luk_QueryPopUp);
		this.luk_Subject.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.luk_Subject.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_Subject_EditValueChanging);
		this.luk_Type.EditControl = null;
		this.luk_Type.FieldName = null;
		this.luk_Type.IsSearchControl = false;
		this.luk_Type.Location = new System.Drawing.Point(119, 21);
		this.luk_Type.Name = "luk_Type";
		this.luk_Type.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_Type.Properties.Appearance.Options.UseFont = true;
		this.luk_Type.Properties.AutoHeight = false;
		this.luk_Type.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Type.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_Type.Properties.DisplayMember = "Name";
		this.luk_Type.Properties.IsSearchControl = false;
		this.luk_Type.Properties.NullText = "";
		this.luk_Type.Properties.ValueMember = "Code";
		this.luk_Type.SettingInVisible = false;
		this.luk_Type.SettingReadonly = false;
		this.luk_Type.Size = new System.Drawing.Size(214, 18);
		this.luk_Type.TabIndex = 43;
		this.luk_Type.Popup += new System.EventHandler(this.luk_Popup);
		this.luk_Type.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.luk_QueryPopUp);
		this.luk_Type.EditValueChanged += new System.EventHandler(this.luk_Type_EditValueChanged);
		this.luk_Type.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_Type_EditValueChanging);
		this.cb_ContactPerson.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.cb_ContactPerson.EditControl = null;
		this.cb_ContactPerson.FieldName = null;
		this.cb_ContactPerson.IsSearchControl = false;
		this.cb_ContactPerson.Location = new System.Drawing.Point(490, 59);
		this.cb_ContactPerson.Name = "cb_ContactPerson";
		this.cb_ContactPerson.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cb_ContactPerson.Properties.Appearance.Options.UseFont = true;
		this.cb_ContactPerson.Properties.AutoHeight = false;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.cb_ContactPerson.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cb_ContactPerson.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cb_ContactPerson_Properties_ButtonPressed);
		this.cb_ContactPerson.SettingInVisible = false;
		this.cb_ContactPerson.SettingReadonly = false;
		this.cb_ContactPerson.Size = new System.Drawing.Size(210, 18);
		this.cb_ContactPerson.TabIndex = 11;
		this.cb_ContactPerson.SelectedValueChanged += new System.EventHandler(this.cb_ContactPerson_SelectedValueChanged);
		this.cb_ContactPerson.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txtAssignedBy.EditControl = null;
		this.txtAssignedBy.FieldName = null;
		this.txtAssignedBy.IsSearchControl = false;
		this.txtAssignedBy.Location = new System.Drawing.Point(119, 78);
		this.txtAssignedBy.Name = "txtAssignedBy";
		this.txtAssignedBy.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtAssignedBy.Properties.Appearance.Options.UseFont = true;
		this.txtAssignedBy.Properties.AutoHeight = false;
		this.txtAssignedBy.Properties.ReadOnly = true;
		this.txtAssignedBy.SettingInVisible = false;
		this.txtAssignedBy.SettingReadonly = false;
		this.txtAssignedBy.Size = new System.Drawing.Size(214, 18);
		this.txtAssignedBy.TabIndex = 6;
		this.txtTelePhone.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtTelePhone.EditControl = null;
		this.txtTelePhone.FieldName = null;
		this.txtTelePhone.IsSearchControl = false;
		this.txtTelePhone.Location = new System.Drawing.Point(490, 78);
		this.txtTelePhone.Name = "txtTelePhone";
		this.txtTelePhone.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtTelePhone.Properties.Appearance.Options.UseFont = true;
		this.txtTelePhone.Properties.AutoHeight = false;
		this.txtTelePhone.Properties.Mask.SaveLiteral = false;
		this.txtTelePhone.Properties.Mask.ShowPlaceHolders = false;
		this.txtTelePhone.Properties.Mask.UseMaskAsDisplayFormat = true;
		this.txtTelePhone.SettingInVisible = false;
		this.txtTelePhone.SettingReadonly = false;
		this.txtTelePhone.Size = new System.Drawing.Size(210, 18);
		this.txtTelePhone.TabIndex = 12;
		this.txtTelePhone.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.btAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btAdd.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Add;
		this.btAdd.EditControl = null;
		this.btAdd.EditValue = "Add";
		this.btAdd.FieldName = null;
		this.btAdd.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btAdd.ImageOptions.Image");
		this.btAdd.IsSearchControl = false;
		this.btAdd.Location = new System.Drawing.Point(5, 571);
		this.btAdd.Name = "btAdd";
		this.btAdd.ReadOnly = false;
		this.btAdd.SettingInVisible = false;
		this.btAdd.SettingReadonly = false;
		this.btAdd.Size = new System.Drawing.Size(75, 23);
		this.btAdd.TabIndex = 41;
		this.btAdd.Text = "Add";
		this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(86, 571);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 42;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.Xtap.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.Xtap.IsSearchControl = false;
		this.Xtap.Location = new System.Drawing.Point(5, 118);
		this.Xtap.Name = "Xtap";
		this.Xtap.SelectedTabPage = this.tap_General;
		this.Xtap.Size = new System.Drawing.Size(776, 453);
		this.Xtap.TabIndex = 40;
		this.Xtap.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[4] { this.tap_General, this.tab_Content, this.tap_LinkedDocument, this.tap_Att });
		this.tap_General.Controls.Add(this.txtState);
		this.tap_General.Controls.Add(this.txtCountry);
		this.tap_General.Controls.Add(this.txtRoom);
		this.tap_General.Controls.Add(this.txtCity);
		this.tap_General.Controls.Add(this.txtStreet);
		this.tap_General.Controls.Add(this.txtDetails);
		this.tap_General.Controls.Add(this.cbStatus);
		this.tap_General.Controls.Add(this.cbRecurPat);
		this.tap_General.Controls.Add(this.luk_Meetting);
		this.tap_General.Controls.Add(this.panel_Range);
		this.tap_General.Controls.Add(this.txtRemQty);
		this.tap_General.Controls.Add(this.btFollowUp);
		this.tap_General.Controls.Add(this.chk_Tentative);
		this.tap_General.Controls.Add(this.chkInactive);
		this.tap_General.Controls.Add(this.chkClosed);
		this.tap_General.Controls.Add(this.chkReminder);
		this.tap_General.Controls.Add(this.panel_Daily);
		this.tap_General.Controls.Add(this.lb_Recurrence);
		this.tap_General.Controls.Add(this.cbPriority);
		this.tap_General.Controls.Add(this.txtDuration);
		this.tap_General.Controls.Add(this.txtEndTime);
		this.tap_General.Controls.Add(this.txtBeginTime);
		this.tap_General.Controls.Add(this.dEnDate);
		this.tap_General.Controls.Add(this.dStart_Recontact);
		this.tap_General.Controls.Add(this.label_MettingLocation);
		this.tap_General.Controls.Add(this.label_Priority);
		this.tap_General.Controls.Add(this.lb_Country);
		this.tap_General.Controls.Add(this.lb_Room);
		this.tap_General.Controls.Add(this.lb_State);
		this.tap_General.Controls.Add(this.lb_City);
		this.tap_General.Controls.Add(this.lb_Street);
		this.tap_General.Controls.Add(this.lb_Address);
		this.tap_General.Controls.Add(this.lb_Status);
		this.tap_General.Controls.Add(this.lb_Duration);
		this.tap_General.Controls.Add(this.lb_EndTime);
		this.tap_General.Controls.Add(this.lb_StartTime);
		this.tap_General.Controls.Add(this.lb_Remarks);
		this.tap_General.EditControl = null;
		this.tap_General.EditValue = "General";
		this.tap_General.FieldName = null;
		this.tap_General.IsSearchControl = false;
		this.tap_General.Name = "tap_General";
		this.tap_General.ReadOnly = false;
		this.tap_General.SettingInVisible = false;
		this.tap_General.SettingReadonly = false;
		this.tap_General.Size = new System.Drawing.Size(774, 428);
		this.tap_General.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tap_General.Text = "General";
		this.txtState.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtState.EditControl = null;
		this.txtState.FieldName = null;
		this.txtState.IsSearchControl = false;
		this.txtState.Location = new System.Drawing.Point(534, 116);
		this.txtState.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.txtState.Name = "txtState";
		this.txtState.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtState.Properties.Appearance.Options.UseFont = true;
		this.txtState.Properties.AutoHeight = false;
		this.txtState.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtState.Properties.IsSearchControl = false;
		this.txtState.Properties.NullText = "";
		this.txtState.SettingInVisible = false;
		this.txtState.SettingReadonly = false;
		this.txtState.Size = new System.Drawing.Size(163, 18);
		this.txtState.TabIndex = 60;
		this.txtState.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txtCountry.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtCountry.EditControl = null;
		this.txtCountry.FieldName = null;
		this.txtCountry.IsSearchControl = false;
		this.txtCountry.Location = new System.Drawing.Point(534, 135);
		this.txtCountry.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.txtCountry.Name = "txtCountry";
		this.txtCountry.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtCountry.Properties.Appearance.Options.UseFont = true;
		this.txtCountry.Properties.AutoHeight = false;
		this.txtCountry.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtCountry.Properties.IsSearchControl = false;
		this.txtCountry.Properties.NullText = "";
		this.txtCountry.SettingInVisible = false;
		this.txtCountry.SettingReadonly = false;
		this.txtCountry.Size = new System.Drawing.Size(163, 18);
		this.txtCountry.TabIndex = 59;
		this.txtCountry.EditValueChanged += new System.EventHandler(this.txtCountry_EditValueChanged);
		this.txtRoom.EditControl = null;
		this.txtRoom.FieldName = null;
		this.txtRoom.IsSearchControl = false;
		this.txtRoom.Location = new System.Drawing.Point(120, 154);
		this.txtRoom.Name = "txtRoom";
		this.txtRoom.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtRoom.Properties.Appearance.Options.UseFont = true;
		this.txtRoom.Properties.AutoHeight = false;
		this.txtRoom.SettingInVisible = false;
		this.txtRoom.SettingReadonly = false;
		this.txtRoom.Size = new System.Drawing.Size(189, 18);
		this.txtRoom.TabIndex = 51;
		this.txtRoom.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txtCity.EditControl = null;
		this.txtCity.FieldName = null;
		this.txtCity.IsSearchControl = false;
		this.txtCity.Location = new System.Drawing.Point(120, 135);
		this.txtCity.Name = "txtCity";
		this.txtCity.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtCity.Properties.Appearance.Options.UseFont = true;
		this.txtCity.Properties.AutoHeight = false;
		this.txtCity.SettingInVisible = false;
		this.txtCity.SettingReadonly = false;
		this.txtCity.Size = new System.Drawing.Size(189, 18);
		this.txtCity.TabIndex = 50;
		this.txtCity.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txtStreet.EditControl = null;
		this.txtStreet.FieldName = null;
		this.txtStreet.IsSearchControl = false;
		this.txtStreet.Location = new System.Drawing.Point(120, 116);
		this.txtStreet.Name = "txtStreet";
		this.txtStreet.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtStreet.Properties.Appearance.Options.UseFont = true;
		this.txtStreet.Properties.AutoHeight = false;
		this.txtStreet.SettingInVisible = false;
		this.txtStreet.SettingReadonly = false;
		this.txtStreet.Size = new System.Drawing.Size(189, 18);
		this.txtStreet.TabIndex = 49;
		this.txtStreet.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txtDetails.EditControl = null;
		this.txtDetails.FieldName = null;
		this.txtDetails.IsSearchControl = false;
		this.txtDetails.Location = new System.Drawing.Point(124, 3);
		this.txtDetails.Name = "txtDetails";
		this.txtDetails.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtDetails.Properties.Appearance.Options.UseFont = true;
		this.txtDetails.Properties.AutoHeight = false;
		this.txtDetails.SettingInVisible = false;
		this.txtDetails.SettingReadonly = false;
		this.txtDetails.Size = new System.Drawing.Size(569, 18);
		this.txtDetails.TabIndex = 48;
		this.txtDetails.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.cbStatus.EditControl = null;
		this.cbStatus.FieldName = null;
		this.cbStatus.IsSearchControl = false;
		this.cbStatus.Location = new System.Drawing.Point(124, 79);
		this.cbStatus.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.cbStatus.Name = "cbStatus";
		this.cbStatus.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbStatus.Properties.Appearance.Options.UseFont = true;
		this.cbStatus.Properties.AutoHeight = false;
		this.cbStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbStatus.Properties.IsSearchControl = false;
		this.cbStatus.Properties.NullText = "";
		this.cbStatus.SettingInVisible = false;
		this.cbStatus.SettingReadonly = false;
		this.cbStatus.Size = new System.Drawing.Size(100, 18);
		this.cbStatus.TabIndex = 47;
		this.cbStatus.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.cbStatus.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.cbStatus_EditValueChanging);
		this.cbRecurPat.EditControl = null;
		this.cbRecurPat.FieldName = null;
		this.cbRecurPat.IsSearchControl = false;
		this.cbRecurPat.Location = new System.Drawing.Point(120, 181);
		this.cbRecurPat.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.cbRecurPat.Name = "cbRecurPat";
		this.cbRecurPat.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbRecurPat.Properties.Appearance.Options.UseFont = true;
		this.cbRecurPat.Properties.AutoHeight = false;
		this.cbRecurPat.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbRecurPat.Properties.IsSearchControl = false;
		this.cbRecurPat.Properties.NullText = "";
		this.cbRecurPat.SettingInVisible = false;
		this.cbRecurPat.SettingReadonly = false;
		this.cbRecurPat.Size = new System.Drawing.Size(189, 20);
		this.cbRecurPat.TabIndex = 46;
		this.cbRecurPat.EditValueChanged += new System.EventHandler(this.cbRecurPat_EditValueChanged);
		this.luk_Meetting.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_Meetting.EditControl = null;
		this.luk_Meetting.FieldName = null;
		this.luk_Meetting.IsSearchControl = false;
		this.luk_Meetting.Location = new System.Drawing.Point(529, 41);
		this.luk_Meetting.Name = "luk_Meetting";
		this.luk_Meetting.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_Meetting.Properties.Appearance.Options.UseFont = true;
		this.luk_Meetting.Properties.AutoHeight = false;
		this.luk_Meetting.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Meetting.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_Meetting.Properties.DisplayMember = "Name";
		this.luk_Meetting.Properties.IsSearchControl = false;
		this.luk_Meetting.Properties.NullText = "";
		this.luk_Meetting.Properties.ValueMember = "Code";
		this.luk_Meetting.SettingInVisible = false;
		this.luk_Meetting.SettingReadonly = false;
		this.luk_Meetting.Size = new System.Drawing.Size(163, 18);
		this.luk_Meetting.TabIndex = 45;
		this.luk_Meetting.Popup += new System.EventHandler(this.luk_Popup);
		this.luk_Meetting.QueryPopUp += new System.ComponentModel.CancelEventHandler(this.luk_QueryPopUp);
		this.luk_Meetting.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.luk_Meetting.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_Metting_EditValueChanging);
		this.panel_Range.Controls.Add(this.lb_End);
		this.panel_Range.Controls.Add(this.lb_Start);
		this.panel_Range.Controls.Add(this.lb_Occurrence);
		this.panel_Range.Controls.Add(this.txtAfterMaxOccur);
		this.panel_Range.Controls.Add(this.radio_By);
		this.panel_Range.Controls.Add(this.radio_After);
		this.panel_Range.Controls.Add(this.radio_NoEndDate);
		this.panel_Range.Controls.Add(this.lb_Range);
		this.panel_Range.Controls.Add(this.labelLine32);
		this.panel_Range.Controls.Add(this.txtBySeEndDat);
		this.panel_Range.Controls.Add(this.dSeStartDat);
		this.panel_Range.Location = new System.Drawing.Point(12, 269);
		this.panel_Range.Name = "panel_Range";
		this.panel_Range.Size = new System.Drawing.Size(624, 93);
		this.panel_Range.TabIndex = 19;
		this.lb_End.EditControl = null;
		this.lb_End.EditValue = "End";
		this.lb_End.FieldName = null;
		this.lb_End.HideWhenEmpty = false;
		this.lb_End.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_End.Location = new System.Drawing.Point(75, 35);
		this.lb_End.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_End.Name = "lb_End";
		this.lb_End.ReadOnly = false;
		this.lb_End.SettingInVisible = false;
		this.lb_End.SettingReadonly = false;
		this.lb_End.ShowUnderLine = true;
		this.lb_End.Size = new System.Drawing.Size(94, 20);
		this.lb_End.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_End.TabIndex = 0;
		this.lb_End.Utext = "End";
		this.lb_Start.EditControl = null;
		this.lb_Start.EditValue = "Start";
		this.lb_Start.FieldName = null;
		this.lb_Start.HideWhenEmpty = false;
		this.lb_Start.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Start.Location = new System.Drawing.Point(75, 16);
		this.lb_Start.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Start.Name = "lb_Start";
		this.lb_Start.ReadOnly = false;
		this.lb_Start.SettingInVisible = false;
		this.lb_Start.SettingReadonly = false;
		this.lb_Start.ShowUnderLine = true;
		this.lb_Start.Size = new System.Drawing.Size(94, 20);
		this.lb_Start.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Start.TabIndex = 0;
		this.lb_Start.Utext = "Start";
		this.lb_Occurrence.Location = new System.Drawing.Point(348, 58);
		this.lb_Occurrence.Name = "lb_Occurrence";
		this.lb_Occurrence.Size = new System.Drawing.Size(68, 13);
		this.lb_Occurrence.TabIndex = 52;
		this.lb_Occurrence.Text = "Occurrence(s)";
		this.txtAfterMaxOccur.EditControl = null;
		this.txtAfterMaxOccur.FieldName = null;
		this.txtAfterMaxOccur.IsSearchControl = false;
		this.txtAfterMaxOccur.Location = new System.Drawing.Point(242, 51);
		this.txtAfterMaxOccur.Name = "txtAfterMaxOccur";
		this.txtAfterMaxOccur.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtAfterMaxOccur.Properties.Appearance.Options.UseFont = true;
		this.txtAfterMaxOccur.Properties.AutoHeight = false;
		this.txtAfterMaxOccur.SettingInVisible = false;
		this.txtAfterMaxOccur.SettingReadonly = false;
		this.txtAfterMaxOccur.Size = new System.Drawing.Size(100, 18);
		this.txtAfterMaxOccur.TabIndex = 51;
		this.txtAfterMaxOccur.EditValueChanged += new System.EventHandler(this.txtAfterMaxOccur_EditValueChanged);
		this.txtAfterMaxOccur.Leave += new System.EventHandler(this.txtAfterMaxOccur_Leave);
		this.radio_By.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.radio_By.Location = new System.Drawing.Point(169, 66);
		this.radio_By.Name = "radio_By";
		this.radio_By.Size = new System.Drawing.Size(37, 16);
		this.radio_By.TabIndex = 34;
		this.radio_By.Text = "By";
		this.radio_By.UseVisualStyleBackColor = true;
		this.radio_By.CheckedChanged += new System.EventHandler(this.radio_By_CheckedChanged);
		this.radio_After.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.radio_After.Location = new System.Drawing.Point(169, 52);
		this.radio_After.Name = "radio_After";
		this.radio_After.Size = new System.Drawing.Size(43, 14);
		this.radio_After.TabIndex = 33;
		this.radio_After.Text = "After";
		this.radio_After.UseVisualStyleBackColor = true;
		this.radio_After.CheckedChanged += new System.EventHandler(this.radio_After_CheckedChanged);
		this.radio_NoEndDate.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.radio_NoEndDate.Location = new System.Drawing.Point(169, 37);
		this.radio_NoEndDate.Name = "radio_NoEndDate";
		this.radio_NoEndDate.Size = new System.Drawing.Size(85, 14);
		this.radio_NoEndDate.TabIndex = 32;
		this.radio_NoEndDate.Text = "No End Date";
		this.radio_NoEndDate.UseVisualStyleBackColor = true;
		this.radio_NoEndDate.CheckedChanged += new System.EventHandler(this.radio_NoEndDate_CheckedChanged);
		this.lb_Range.Location = new System.Drawing.Point(77, 1);
		this.lb_Range.Name = "lb_Range";
		this.lb_Range.Size = new System.Drawing.Size(31, 13);
		this.lb_Range.TabIndex = 0;
		this.lb_Range.Text = "Range";
		this.labelLine32.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine32.Appearance.Options.UseFont = true;
		this.labelLine32.EditControl = null;
		this.labelLine32.EditValue = "Start";
		this.labelLine32.FieldName = null;
		this.labelLine32.HideWhenEmpty = false;
		this.labelLine32.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine32.Location = new System.Drawing.Point(75, 19);
		this.labelLine32.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine32.Name = "labelLine32";
		this.labelLine32.ReadOnly = false;
		this.labelLine32.SettingInVisible = false;
		this.labelLine32.SettingReadonly = false;
		this.labelLine32.ShowUnderLine = true;
		this.labelLine32.Size = new System.Drawing.Size(94, 0);
		this.labelLine32.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine32.TabIndex = 45;
		this.labelLine32.Utext = "Start";
		this.txtBySeEndDat.EditControl = null;
		this.txtBySeEndDat.EditValue = null;
		this.txtBySeEndDat.FieldName = null;
		this.txtBySeEndDat.Location = new System.Drawing.Point(242, 71);
		this.txtBySeEndDat.Name = "txtBySeEndDat";
		this.txtBySeEndDat.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtBySeEndDat.Properties.Appearance.Options.UseFont = true;
		this.txtBySeEndDat.Properties.AutoHeight = false;
		this.txtBySeEndDat.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtBySeEndDat.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtBySeEndDat.Properties.IsSearchControl = false;
		this.txtBySeEndDat.Properties.Mask.EditMask = "";
		this.txtBySeEndDat.Properties.Mask.UseMaskAsDisplayFormat = true;
		this.txtBySeEndDat.SettingInVisible = false;
		this.txtBySeEndDat.SettingReadonly = false;
		this.txtBySeEndDat.Size = new System.Drawing.Size(100, 18);
		this.txtBySeEndDat.TabIndex = 50;
		this.txtBySeEndDat.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.dSeStartDat.EditControl = null;
		this.dSeStartDat.EditValue = null;
		this.dSeStartDat.FieldName = null;
		this.dSeStartDat.Location = new System.Drawing.Point(169, 16);
		this.dSeStartDat.Name = "dSeStartDat";
		this.dSeStartDat.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.dSeStartDat.Properties.Appearance.Options.UseFont = true;
		this.dSeStartDat.Properties.AutoHeight = false;
		this.dSeStartDat.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dSeStartDat.Properties.IsSearchControl = false;
		this.dSeStartDat.Properties.Mask.EditMask = "";
		this.dSeStartDat.Properties.Mask.UseMaskAsDisplayFormat = true;
		this.dSeStartDat.Properties.ReadOnly = true;
		this.dSeStartDat.SettingInVisible = false;
		this.dSeStartDat.SettingReadonly = false;
		this.dSeStartDat.Size = new System.Drawing.Size(100, 18);
		this.dSeStartDat.TabIndex = 31;
		this.dSeStartDat.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txtRemQty.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.txtRemQty.EditControl = null;
		this.txtRemQty.FieldName = null;
		this.txtRemQty.IsSearchControl = false;
		this.txtRemQty.Location = new System.Drawing.Point(87, 365);
		this.txtRemQty.Name = "txtRemQty";
		this.txtRemQty.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtRemQty.Properties.Appearance.Options.UseFont = true;
		this.txtRemQty.Properties.AutoHeight = false;
		this.txtRemQty.SettingInVisible = false;
		this.txtRemQty.SettingReadonly = false;
		this.txtRemQty.Size = new System.Drawing.Size(114, 18);
		this.txtRemQty.TabIndex = 36;
		this.txtRemQty.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.btFollowUp.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
		this.btFollowUp.EditControl = null;
		this.btFollowUp.EditValue = "Follow Up";
		this.btFollowUp.FieldName = null;
		this.btFollowUp.IsSearchControl = false;
		this.btFollowUp.Location = new System.Drawing.Point(551, 367);
		this.btFollowUp.Name = "btFollowUp";
		this.btFollowUp.ReadOnly = false;
		this.btFollowUp.SettingInVisible = false;
		this.btFollowUp.SettingReadonly = false;
		this.btFollowUp.Size = new System.Drawing.Size(75, 23);
		this.btFollowUp.TabIndex = 40;
		this.btFollowUp.Text = "Follow Up";
		this.btFollowUp.Click += new System.EventHandler(this.btFollowUp_Click);
		this.chk_Tentative.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
		this.chk_Tentative.EditControl = null;
		this.chk_Tentative.EditValue = "N";
		this.chk_Tentative.FieldName = null;
		this.chk_Tentative.IsSearchControl = false;
		this.chk_Tentative.Location = new System.Drawing.Point(327, 362);
		this.chk_Tentative.Name = "chk_Tentative";
		this.chk_Tentative.Properties.Caption = "Tentative";
		this.chk_Tentative.Properties.DisplayValueChecked = "Y";
		this.chk_Tentative.Properties.DisplayValueGrayed = "N";
		this.chk_Tentative.Properties.DisplayValueUnchecked = "N";
		this.chk_Tentative.Properties.ValueChecked = "Y";
		this.chk_Tentative.Properties.ValueGrayed = "N";
		this.chk_Tentative.Properties.ValueUnchecked = "N";
		this.chk_Tentative.SettingInVisible = false;
		this.chk_Tentative.SettingReadonly = false;
		this.chk_Tentative.Size = new System.Drawing.Size(75, 20);
		this.chk_Tentative.TabIndex = 37;
		this.chk_Tentative.CheckedChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chk_Tentative.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chkInactive.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
		this.chkInactive.EditControl = null;
		this.chkInactive.EditValue = "N";
		this.chkInactive.FieldName = null;
		this.chkInactive.IsSearchControl = false;
		this.chkInactive.Location = new System.Drawing.Point(327, 383);
		this.chkInactive.Name = "chkInactive";
		this.chkInactive.Properties.Caption = "Inactive";
		this.chkInactive.Properties.DisplayValueChecked = "Y";
		this.chkInactive.Properties.DisplayValueGrayed = "N";
		this.chkInactive.Properties.DisplayValueUnchecked = "N";
		this.chkInactive.Properties.ValueChecked = "Y";
		this.chkInactive.Properties.ValueGrayed = "N";
		this.chkInactive.Properties.ValueUnchecked = "N";
		this.chkInactive.SettingInVisible = false;
		this.chkInactive.SettingReadonly = false;
		this.chkInactive.Size = new System.Drawing.Size(75, 20);
		this.chkInactive.TabIndex = 38;
		this.chkInactive.CheckedChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chkInactive.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chkClosed.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
		this.chkClosed.EditControl = null;
		this.chkClosed.EditValue = "N";
		this.chkClosed.FieldName = null;
		this.chkClosed.IsSearchControl = false;
		this.chkClosed.Location = new System.Drawing.Point(327, 403);
		this.chkClosed.Name = "chkClosed";
		this.chkClosed.Properties.Caption = "Closed";
		this.chkClosed.Properties.DisplayValueChecked = "Y";
		this.chkClosed.Properties.DisplayValueGrayed = "N";
		this.chkClosed.Properties.DisplayValueUnchecked = "N";
		this.chkClosed.Properties.ValueChecked = "Y";
		this.chkClosed.Properties.ValueGrayed = "N";
		this.chkClosed.Properties.ValueUnchecked = "N";
		this.chkClosed.SettingInVisible = false;
		this.chkClosed.SettingReadonly = false;
		this.chkClosed.Size = new System.Drawing.Size(75, 20);
		this.chkClosed.TabIndex = 39;
		this.chkClosed.CheckedChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chkClosed.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chkReminder.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.chkReminder.EditControl = null;
		this.chkReminder.EditValue = "N";
		this.chkReminder.FieldName = null;
		this.chkReminder.IsSearchControl = false;
		this.chkReminder.Location = new System.Drawing.Point(20, 365);
		this.chkReminder.Name = "chkReminder";
		this.chkReminder.Properties.Caption = "Reminder";
		this.chkReminder.Properties.ValueChecked = "Y";
		this.chkReminder.Properties.ValueUnchecked = "N";
		this.chkReminder.SettingInVisible = false;
		this.chkReminder.SettingReadonly = false;
		this.chkReminder.Size = new System.Drawing.Size(75, 20);
		this.chkReminder.TabIndex = 35;
		this.chkReminder.CheckedChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.panel_Daily.Controls.Add(this.chkSun);
		this.panel_Daily.Controls.Add(this.cbMonth);
		this.panel_Daily.Controls.Add(this.cbMonth1);
		this.panel_Daily.Controls.Add(this.chkSat);
		this.panel_Daily.Controls.Add(this.cbDayOfWeek);
		this.panel_Daily.Controls.Add(this.cbWeek);
		this.panel_Daily.Controls.Add(this.chkFri);
		this.panel_Daily.Controls.Add(this.txtDayInMonth);
		this.panel_Daily.Controls.Add(this.radio_SubOption2);
		this.panel_Daily.Controls.Add(this.chkThu);
		this.panel_Daily.Controls.Add(this.label_RepeatEvery);
		this.panel_Daily.Controls.Add(this.txtInterval);
		this.panel_Daily.Controls.Add(this.chkWed);
		this.panel_Daily.Controls.Add(this.radio_SubOption1);
		this.panel_Daily.Controls.Add(this.lb_Of);
		this.panel_Daily.Controls.Add(this.chkTue);
		this.panel_Daily.Controls.Add(this.lb_RepeatOn);
		this.panel_Daily.Controls.Add(this.lb_RepeatEvery);
		this.panel_Daily.Controls.Add(this.chkMon);
		this.panel_Daily.Controls.Add(this.radio_SubOption);
		this.panel_Daily.Location = new System.Drawing.Point(13, 204);
		this.panel_Daily.Name = "panel_Daily";
		this.panel_Daily.Size = new System.Drawing.Size(623, 65);
		this.panel_Daily.TabIndex = 13;
		this.chkSun.EditControl = null;
		this.chkSun.EditValue = "N";
		this.chkSun.FieldName = null;
		this.chkSun.IsSearchControl = false;
		this.chkSun.Location = new System.Drawing.Point(135, 42);
		this.chkSun.Name = "chkSun";
		this.chkSun.Properties.Caption = "Sun";
		this.chkSun.Properties.ValueChecked = "Y";
		this.chkSun.Properties.ValueUnchecked = "N";
		this.chkSun.SettingInVisible = false;
		this.chkSun.SettingReadonly = false;
		this.chkSun.Size = new System.Drawing.Size(46, 20);
		this.chkSun.TabIndex = 33;
		this.chkSun.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.cbMonth.EditControl = null;
		this.cbMonth.FieldName = null;
		this.cbMonth.IsSearchControl = false;
		this.cbMonth.Location = new System.Drawing.Point(321, 24);
		this.cbMonth.Name = "cbMonth";
		this.cbMonth.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbMonth.Properties.Appearance.Options.UseFont = true;
		this.cbMonth.Properties.AutoHeight = false;
		this.cbMonth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbMonth.Properties.Items.AddRange(new object[12]
		{
			"January", "February", "March", "April", "May", "June", "July", "August", "September", "October",
			"November", "December"
		});
		this.cbMonth.SettingInVisible = false;
		this.cbMonth.SettingReadonly = false;
		this.cbMonth.Size = new System.Drawing.Size(100, 18);
		this.cbMonth.TabIndex = 32;
		this.cbMonth.SelectedValueChanged += new System.EventHandler(this.cbMonth_SelectedValueChanged);
		this.cbMonth1.EditControl = null;
		this.cbMonth1.FieldName = null;
		this.cbMonth1.IsSearchControl = false;
		this.cbMonth1.Location = new System.Drawing.Point(516, 44);
		this.cbMonth1.Name = "cbMonth1";
		this.cbMonth1.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbMonth1.Properties.Appearance.Options.UseFont = true;
		this.cbMonth1.Properties.AutoHeight = false;
		this.cbMonth1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbMonth1.Properties.Items.AddRange(new object[12]
		{
			"January", "February", "March", "April", "May", "June", "July", "August", "September", "October",
			"November", "December"
		});
		this.cbMonth1.SettingInVisible = false;
		this.cbMonth1.SettingReadonly = false;
		this.cbMonth1.Size = new System.Drawing.Size(100, 18);
		this.cbMonth1.TabIndex = 31;
		this.cbMonth1.SelectedValueChanged += new System.EventHandler(this.cbMonth1_SelectedValueChanged);
		this.chkSat.EditControl = null;
		this.chkSat.EditValue = "N";
		this.chkSat.FieldName = null;
		this.chkSat.IsSearchControl = false;
		this.chkSat.Location = new System.Drawing.Point(93, 42);
		this.chkSat.Name = "chkSat";
		this.chkSat.Properties.Caption = "Sat";
		this.chkSat.Properties.ValueChecked = "Y";
		this.chkSat.Properties.ValueUnchecked = "N";
		this.chkSat.SettingInVisible = false;
		this.chkSat.SettingReadonly = false;
		this.chkSat.Size = new System.Drawing.Size(37, 20);
		this.chkSat.TabIndex = 33;
		this.chkSat.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.cbDayOfWeek.EditControl = null;
		this.cbDayOfWeek.FieldName = null;
		this.cbDayOfWeek.IsSearchControl = false;
		this.cbDayOfWeek.Location = new System.Drawing.Point(377, 44);
		this.cbDayOfWeek.Name = "cbDayOfWeek";
		this.cbDayOfWeek.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbDayOfWeek.Properties.Appearance.Options.UseFont = true;
		this.cbDayOfWeek.Properties.AutoHeight = false;
		this.cbDayOfWeek.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbDayOfWeek.Properties.Items.AddRange(new object[10] { "day", "weekday", "weekend day", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" });
		this.cbDayOfWeek.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.cbDayOfWeek.SettingInVisible = false;
		this.cbDayOfWeek.SettingReadonly = false;
		this.cbDayOfWeek.Size = new System.Drawing.Size(100, 18);
		this.cbDayOfWeek.TabIndex = 30;
		this.cbDayOfWeek.SelectedIndexChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.cbWeek.EditControl = null;
		this.cbWeek.FieldName = null;
		this.cbWeek.IsSearchControl = false;
		this.cbWeek.Location = new System.Drawing.Point(271, 44);
		this.cbWeek.Name = "cbWeek";
		this.cbWeek.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbWeek.Properties.Appearance.Options.UseFont = true;
		this.cbWeek.Properties.AutoHeight = false;
		this.cbWeek.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbWeek.Properties.Items.AddRange(new object[5] { "first", "second", "third", "fourth", "last" });
		this.cbWeek.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.cbWeek.SettingInVisible = false;
		this.cbWeek.SettingReadonly = false;
		this.cbWeek.Size = new System.Drawing.Size(100, 18);
		this.cbWeek.TabIndex = 30;
		this.cbWeek.SelectedIndexChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chkFri.EditControl = null;
		this.chkFri.EditValue = "N";
		this.chkFri.FieldName = null;
		this.chkFri.IsSearchControl = false;
		this.chkFri.Location = new System.Drawing.Point(573, 7);
		this.chkFri.Name = "chkFri";
		this.chkFri.Properties.Caption = "Fri";
		this.chkFri.Properties.ValueChecked = "Y";
		this.chkFri.Properties.ValueUnchecked = "N";
		this.chkFri.SettingInVisible = false;
		this.chkFri.SettingReadonly = false;
		this.chkFri.Size = new System.Drawing.Size(33, 20);
		this.chkFri.TabIndex = 33;
		this.chkFri.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txtDayInMonth.EditControl = null;
		this.txtDayInMonth.FieldName = null;
		this.txtDayInMonth.IsSearchControl = false;
		this.txtDayInMonth.Location = new System.Drawing.Point(427, 24);
		this.txtDayInMonth.Name = "txtDayInMonth";
		this.txtDayInMonth.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtDayInMonth.Properties.Appearance.Options.UseFont = true;
		this.txtDayInMonth.Properties.AutoHeight = false;
		this.txtDayInMonth.SettingInVisible = false;
		this.txtDayInMonth.SettingReadonly = false;
		this.txtDayInMonth.Size = new System.Drawing.Size(73, 18);
		this.txtDayInMonth.TabIndex = 29;
		this.radio_SubOption2.AutoSize = true;
		this.radio_SubOption2.Location = new System.Drawing.Point(207, 48);
		this.radio_SubOption2.Name = "radio_SubOption2";
		this.radio_SubOption2.Size = new System.Drawing.Size(14, 13);
		this.radio_SubOption2.TabIndex = 30;
		this.radio_SubOption2.UseVisualStyleBackColor = true;
		this.radio_SubOption2.CheckedChanged += new System.EventHandler(this.radio_SubOption2_CheckedChanged);
		this.chkThu.EditControl = null;
		this.chkThu.EditValue = "N";
		this.chkThu.FieldName = null;
		this.chkThu.IsSearchControl = false;
		this.chkThu.Location = new System.Drawing.Point(522, 7);
		this.chkThu.Name = "chkThu";
		this.chkThu.Properties.Caption = "Thu";
		this.chkThu.Properties.ValueChecked = "Y";
		this.chkThu.Properties.ValueUnchecked = "N";
		this.chkThu.SettingInVisible = false;
		this.chkThu.SettingReadonly = false;
		this.chkThu.Size = new System.Drawing.Size(46, 20);
		this.chkThu.TabIndex = 33;
		this.chkThu.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.label_RepeatEvery.AutoSize = true;
		this.label_RepeatEvery.Location = new System.Drawing.Point(325, 10);
		this.label_RepeatEvery.Name = "label_RepeatEvery";
		this.label_RepeatEvery.Size = new System.Drawing.Size(39, 13);
		this.label_RepeatEvery.TabIndex = 27;
		this.label_RepeatEvery.Text = "Day(s)";
		this.txtInterval.EditControl = null;
		this.txtInterval.FieldName = null;
		this.txtInterval.IsSearchControl = false;
		this.txtInterval.Location = new System.Drawing.Point(226, 5);
		this.txtInterval.Name = "txtInterval";
		this.txtInterval.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtInterval.Properties.Appearance.Options.UseFont = true;
		this.txtInterval.Properties.AutoHeight = false;
		this.txtInterval.SettingInVisible = false;
		this.txtInterval.SettingReadonly = false;
		this.txtInterval.Size = new System.Drawing.Size(93, 18);
		this.txtInterval.TabIndex = 26;
		this.chkWed.EditControl = null;
		this.chkWed.EditValue = "N";
		this.chkWed.FieldName = null;
		this.chkWed.IsSearchControl = false;
		this.chkWed.Location = new System.Drawing.Point(469, 7);
		this.chkWed.Name = "chkWed";
		this.chkWed.Properties.Caption = "Web";
		this.chkWed.Properties.ValueChecked = "Y";
		this.chkWed.Properties.ValueUnchecked = "N";
		this.chkWed.SettingInVisible = false;
		this.chkWed.SettingReadonly = false;
		this.chkWed.Size = new System.Drawing.Size(46, 20);
		this.chkWed.TabIndex = 33;
		this.chkWed.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.radio_SubOption1.AutoSize = true;
		this.radio_SubOption1.Location = new System.Drawing.Point(207, 26);
		this.radio_SubOption1.Name = "radio_SubOption1";
		this.radio_SubOption1.Size = new System.Drawing.Size(101, 17);
		this.radio_SubOption1.TabIndex = 29;
		this.radio_SubOption1.Text = "Every Weekday";
		this.radio_SubOption1.UseVisualStyleBackColor = true;
		this.radio_SubOption1.CheckedChanged += new System.EventHandler(this.radio_SubOption1_CheckedChanged);
		this.lb_Of.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_Of.Appearance.Options.UseFont = true;
		this.lb_Of.EditControl = null;
		this.lb_Of.EditValue = "of";
		this.lb_Of.FieldName = null;
		this.lb_Of.HideWhenEmpty = false;
		this.lb_Of.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Of.Location = new System.Drawing.Point(485, 44);
		this.lb_Of.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_Of.Name = "lb_Of";
		this.lb_Of.ReadOnly = false;
		this.lb_Of.SettingInVisible = false;
		this.lb_Of.SettingReadonly = false;
		this.lb_Of.ShowUnderLine = true;
		this.lb_Of.Size = new System.Drawing.Size(38, 16);
		this.lb_Of.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Of.TabIndex = 24;
		this.lb_Of.Utext = "of";
		this.chkTue.EditControl = null;
		this.chkTue.EditValue = "N";
		this.chkTue.FieldName = null;
		this.chkTue.IsSearchControl = false;
		this.chkTue.Location = new System.Drawing.Point(422, 7);
		this.chkTue.Name = "chkTue";
		this.chkTue.Properties.Caption = "Tue";
		this.chkTue.Properties.ValueChecked = "Y";
		this.chkTue.Properties.ValueUnchecked = "N";
		this.chkTue.SettingInVisible = false;
		this.chkTue.SettingReadonly = false;
		this.chkTue.Size = new System.Drawing.Size(46, 20);
		this.chkTue.TabIndex = 33;
		this.chkTue.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.lb_RepeatOn.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_RepeatOn.Appearance.Options.UseFont = true;
		this.lb_RepeatOn.EditControl = null;
		this.lb_RepeatOn.EditValue = "Repeat on";
		this.lb_RepeatOn.FieldName = null;
		this.lb_RepeatOn.HideWhenEmpty = false;
		this.lb_RepeatOn.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_RepeatOn.Location = new System.Drawing.Point(79, 23);
		this.lb_RepeatOn.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_RepeatOn.Name = "lb_RepeatOn";
		this.lb_RepeatOn.ReadOnly = false;
		this.lb_RepeatOn.SettingInVisible = false;
		this.lb_RepeatOn.SettingReadonly = false;
		this.lb_RepeatOn.ShowUnderLine = true;
		this.lb_RepeatOn.Size = new System.Drawing.Size(123, 20);
		this.lb_RepeatOn.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_RepeatOn.TabIndex = 0;
		this.lb_RepeatOn.Utext = "Repeat on";
		this.lb_RepeatEvery.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_RepeatEvery.Appearance.Options.UseFont = true;
		this.lb_RepeatEvery.EditControl = null;
		this.lb_RepeatEvery.EditValue = "Repeat Every";
		this.lb_RepeatEvery.FieldName = null;
		this.lb_RepeatEvery.HideWhenEmpty = false;
		this.lb_RepeatEvery.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_RepeatEvery.Location = new System.Drawing.Point(79, 4);
		this.lb_RepeatEvery.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_RepeatEvery.Name = "lb_RepeatEvery";
		this.lb_RepeatEvery.ReadOnly = false;
		this.lb_RepeatEvery.SettingInVisible = false;
		this.lb_RepeatEvery.SettingReadonly = false;
		this.lb_RepeatEvery.ShowUnderLine = true;
		this.lb_RepeatEvery.Size = new System.Drawing.Size(123, 19);
		this.lb_RepeatEvery.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_RepeatEvery.TabIndex = 0;
		this.lb_RepeatEvery.Utext = "Repeat Every";
		this.chkMon.EditControl = null;
		this.chkMon.EditValue = "N";
		this.chkMon.FieldName = null;
		this.chkMon.IsSearchControl = false;
		this.chkMon.Location = new System.Drawing.Point(370, 7);
		this.chkMon.Name = "chkMon";
		this.chkMon.Properties.Caption = "Mon";
		this.chkMon.Properties.ValueChecked = "Y";
		this.chkMon.Properties.ValueUnchecked = "N";
		this.chkMon.SettingInVisible = false;
		this.chkMon.SettingReadonly = false;
		this.chkMon.Size = new System.Drawing.Size(46, 20);
		this.chkMon.TabIndex = 33;
		this.chkMon.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.radio_SubOption.AutoSize = true;
		this.radio_SubOption.Location = new System.Drawing.Point(207, 10);
		this.radio_SubOption.Name = "radio_SubOption";
		this.radio_SubOption.Size = new System.Drawing.Size(14, 13);
		this.radio_SubOption.TabIndex = 28;
		this.radio_SubOption.UseVisualStyleBackColor = true;
		this.radio_SubOption.CheckedChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.lb_Recurrence.EditControl = null;
		this.lb_Recurrence.EditValue = "Recurrence";
		this.lb_Recurrence.FieldName = null;
		this.lb_Recurrence.HideWhenEmpty = false;
		this.lb_Recurrence.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Recurrence.Location = new System.Drawing.Point(13, 184);
		this.lb_Recurrence.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Recurrence.Name = "lb_Recurrence";
		this.lb_Recurrence.ReadOnly = false;
		this.lb_Recurrence.SettingInVisible = false;
		this.lb_Recurrence.SettingReadonly = false;
		this.lb_Recurrence.ShowUnderLine = true;
		this.lb_Recurrence.Size = new System.Drawing.Size(110, 19);
		this.lb_Recurrence.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Recurrence.TabIndex = 9;
		this.lb_Recurrence.Utext = "Recurrence";
		this.cbPriority.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.cbPriority.EditControl = null;
		this.cbPriority.FieldName = null;
		this.cbPriority.IsSearchControl = false;
		this.cbPriority.Location = new System.Drawing.Point(529, 22);
		this.cbPriority.Name = "cbPriority";
		this.cbPriority.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbPriority.Properties.Appearance.Options.UseFont = true;
		this.cbPriority.Properties.AutoHeight = false;
		this.cbPriority.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbPriority.Properties.Items.AddRange(new object[3] { "Low", "Normal", "High" });
		this.cbPriority.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.cbPriority.SettingInVisible = false;
		this.cbPriority.SettingReadonly = false;
		this.cbPriority.Size = new System.Drawing.Size(163, 18);
		this.cbPriority.TabIndex = 20;
		this.cbPriority.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txtDuration.EditControl = null;
		this.txtDuration.FieldName = null;
		this.txtDuration.IsSearchControl = false;
		this.txtDuration.Location = new System.Drawing.Point(124, 60);
		this.txtDuration.Name = "txtDuration";
		this.txtDuration.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtDuration.Properties.Appearance.Options.UseFont = true;
		this.txtDuration.Properties.AutoHeight = false;
		this.txtDuration.SettingInVisible = false;
		this.txtDuration.SettingReadonly = false;
		this.txtDuration.Size = new System.Drawing.Size(100, 18);
		this.txtDuration.TabIndex = 18;
		this.txtDuration.EditValueChanged += new System.EventHandler(this.txtDuration_EditValueChanged);
		this.txtDuration.Leave += new System.EventHandler(this.txtDuration_Leave);
		this.txtEndTime.EditControl = null;
		this.txtEndTime.FieldName = null;
		this.txtEndTime.IsSearchControl = false;
		this.txtEndTime.Location = new System.Drawing.Point(230, 41);
		this.txtEndTime.Name = "txtEndTime";
		this.txtEndTime.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtEndTime.Properties.Appearance.Options.UseFont = true;
		this.txtEndTime.Properties.AutoHeight = false;
		this.txtEndTime.SettingInVisible = false;
		this.txtEndTime.SettingReadonly = false;
		this.txtEndTime.Size = new System.Drawing.Size(84, 18);
		this.txtEndTime.TabIndex = 17;
		this.txtEndTime.EditValueChanged += new System.EventHandler(this.txtEndTime_EditValueChanged);
		this.txtEndTime.Leave += new System.EventHandler(this.txtEndTime_Leave);
		this.txtBeginTime.EditControl = null;
		this.txtBeginTime.FieldName = null;
		this.txtBeginTime.IsSearchControl = false;
		this.txtBeginTime.Location = new System.Drawing.Point(230, 22);
		this.txtBeginTime.Name = "txtBeginTime";
		this.txtBeginTime.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtBeginTime.Properties.Appearance.Options.UseFont = true;
		this.txtBeginTime.Properties.AutoHeight = false;
		this.txtBeginTime.SettingInVisible = false;
		this.txtBeginTime.SettingReadonly = false;
		this.txtBeginTime.Size = new System.Drawing.Size(84, 18);
		this.txtBeginTime.TabIndex = 15;
		this.txtBeginTime.EditValueChanged += new System.EventHandler(this.txtBeginTime_EditValueChanged);
		this.txtBeginTime.Leave += new System.EventHandler(this.txtBeginTime_Leave);
		this.dEnDate.EditControl = null;
		this.dEnDate.EditValue = null;
		this.dEnDate.FieldName = null;
		this.dEnDate.Location = new System.Drawing.Point(124, 41);
		this.dEnDate.Name = "dEnDate";
		this.dEnDate.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.dEnDate.Properties.Appearance.Options.UseFont = true;
		this.dEnDate.Properties.AutoHeight = false;
		this.dEnDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dEnDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dEnDate.Properties.IsSearchControl = false;
		this.dEnDate.Properties.Mask.EditMask = "";
		this.dEnDate.SettingInVisible = false;
		this.dEnDate.SettingReadonly = false;
		this.dEnDate.Size = new System.Drawing.Size(100, 18);
		this.dEnDate.TabIndex = 16;
		this.dEnDate.EditValueChanged += new System.EventHandler(this.dEnDate_EditValueChanged);
		this.dEnDate.Leave += new System.EventHandler(this.dEnDate_Leave);
		this.dStart_Recontact.EditControl = null;
		this.dStart_Recontact.EditValue = null;
		this.dStart_Recontact.FieldName = null;
		this.dStart_Recontact.Location = new System.Drawing.Point(124, 22);
		this.dStart_Recontact.Name = "dStart_Recontact";
		this.dStart_Recontact.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.dStart_Recontact.Properties.Appearance.Options.UseFont = true;
		this.dStart_Recontact.Properties.AutoHeight = false;
		this.dStart_Recontact.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dStart_Recontact.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dStart_Recontact.Properties.IsSearchControl = false;
		this.dStart_Recontact.Properties.Mask.EditMask = "";
		this.dStart_Recontact.SettingInVisible = false;
		this.dStart_Recontact.SettingReadonly = false;
		this.dStart_Recontact.Size = new System.Drawing.Size(100, 18);
		this.dStart_Recontact.TabIndex = 14;
		this.dStart_Recontact.EditValueChanged += new System.EventHandler(this.dStart_Recontact_EditValueChanged);
		this.label_MettingLocation.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.label_MettingLocation.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label_MettingLocation.Appearance.Options.UseFont = true;
		this.label_MettingLocation.EditControl = null;
		this.label_MettingLocation.EditValue = "Metting Location";
		this.label_MettingLocation.FieldName = null;
		this.label_MettingLocation.HideWhenEmpty = false;
		this.label_MettingLocation.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.label_MettingLocation.Location = new System.Drawing.Point(412, 42);
		this.label_MettingLocation.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.label_MettingLocation.Name = "label_MettingLocation";
		this.label_MettingLocation.ReadOnly = false;
		this.label_MettingLocation.SettingInVisible = false;
		this.label_MettingLocation.SettingReadonly = false;
		this.label_MettingLocation.ShowUnderLine = true;
		this.label_MettingLocation.Size = new System.Drawing.Size(119, 19);
		this.label_MettingLocation.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.label_MettingLocation.TabIndex = 0;
		this.label_MettingLocation.Utext = "Metting Location";
		this.label_Priority.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.label_Priority.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label_Priority.Appearance.Options.UseFont = true;
		this.label_Priority.EditControl = null;
		this.label_Priority.EditValue = "Priority";
		this.label_Priority.FieldName = null;
		this.label_Priority.HideWhenEmpty = false;
		this.label_Priority.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.label_Priority.Location = new System.Drawing.Point(412, 23);
		this.label_Priority.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.label_Priority.Name = "label_Priority";
		this.label_Priority.ReadOnly = false;
		this.label_Priority.SettingInVisible = false;
		this.label_Priority.SettingReadonly = false;
		this.label_Priority.ShowUnderLine = true;
		this.label_Priority.Size = new System.Drawing.Size(122, 19);
		this.label_Priority.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.label_Priority.TabIndex = 0;
		this.label_Priority.Utext = "Priority";
		this.lb_Country.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Country.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_Country.Appearance.Options.UseFont = true;
		this.lb_Country.EditControl = null;
		this.lb_Country.EditValue = "Country";
		this.lb_Country.FieldName = null;
		this.lb_Country.HideWhenEmpty = false;
		this.lb_Country.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Country.Location = new System.Drawing.Point(416, 136);
		this.lb_Country.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_Country.Name = "lb_Country";
		this.lb_Country.ReadOnly = false;
		this.lb_Country.SettingInVisible = false;
		this.lb_Country.SettingReadonly = false;
		this.lb_Country.ShowUnderLine = true;
		this.lb_Country.Size = new System.Drawing.Size(122, 19);
		this.lb_Country.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Country.TabIndex = 0;
		this.lb_Country.Utext = "Country";
		this.lb_Room.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_Room.Appearance.Options.UseFont = true;
		this.lb_Room.EditControl = null;
		this.lb_Room.EditValue = "Room";
		this.lb_Room.FieldName = null;
		this.lb_Room.HideWhenEmpty = false;
		this.lb_Room.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Room.Location = new System.Drawing.Point(13, 155);
		this.lb_Room.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_Room.Name = "lb_Room";
		this.lb_Room.ReadOnly = false;
		this.lb_Room.SettingInVisible = false;
		this.lb_Room.SettingReadonly = false;
		this.lb_Room.ShowUnderLine = true;
		this.lb_Room.Size = new System.Drawing.Size(110, 19);
		this.lb_Room.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Room.TabIndex = 0;
		this.lb_Room.Utext = "Room";
		this.lb_State.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_State.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_State.Appearance.Options.UseFont = true;
		this.lb_State.EditControl = null;
		this.lb_State.EditValue = "State";
		this.lb_State.FieldName = null;
		this.lb_State.HideWhenEmpty = false;
		this.lb_State.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_State.Location = new System.Drawing.Point(416, 117);
		this.lb_State.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_State.Name = "lb_State";
		this.lb_State.ReadOnly = false;
		this.lb_State.SettingInVisible = false;
		this.lb_State.SettingReadonly = false;
		this.lb_State.ShowUnderLine = true;
		this.lb_State.Size = new System.Drawing.Size(122, 19);
		this.lb_State.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_State.TabIndex = 0;
		this.lb_State.Utext = "State";
		this.lb_City.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_City.Appearance.Options.UseFont = true;
		this.lb_City.EditControl = null;
		this.lb_City.EditValue = "City";
		this.lb_City.FieldName = null;
		this.lb_City.HideWhenEmpty = false;
		this.lb_City.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_City.Location = new System.Drawing.Point(13, 136);
		this.lb_City.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_City.Name = "lb_City";
		this.lb_City.ReadOnly = false;
		this.lb_City.SettingInVisible = false;
		this.lb_City.SettingReadonly = false;
		this.lb_City.ShowUnderLine = true;
		this.lb_City.Size = new System.Drawing.Size(110, 19);
		this.lb_City.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_City.TabIndex = 0;
		this.lb_City.Utext = "City";
		this.lb_Street.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_Street.Appearance.Options.UseFont = true;
		this.lb_Street.EditControl = null;
		this.lb_Street.EditValue = "Street";
		this.lb_Street.FieldName = null;
		this.lb_Street.HideWhenEmpty = false;
		this.lb_Street.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Street.Location = new System.Drawing.Point(13, 117);
		this.lb_Street.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_Street.Name = "lb_Street";
		this.lb_Street.ReadOnly = false;
		this.lb_Street.SettingInVisible = false;
		this.lb_Street.SettingReadonly = false;
		this.lb_Street.ShowUnderLine = true;
		this.lb_Street.Size = new System.Drawing.Size(110, 19);
		this.lb_Street.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Street.TabIndex = 0;
		this.lb_Street.Utext = "Street";
		this.lb_Address.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_Address.Appearance.Options.UseFont = true;
		this.lb_Address.EditControl = null;
		this.lb_Address.EditValue = "Address";
		this.lb_Address.FieldName = null;
		this.lb_Address.HideWhenEmpty = false;
		this.lb_Address.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Address.Location = new System.Drawing.Point(13, 99);
		this.lb_Address.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_Address.Name = "lb_Address";
		this.lb_Address.ReadOnly = false;
		this.lb_Address.SettingInVisible = false;
		this.lb_Address.SettingReadonly = false;
		this.lb_Address.ShowUnderLine = true;
		this.lb_Address.Size = new System.Drawing.Size(45, 19);
		this.lb_Address.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Address.TabIndex = 0;
		this.lb_Address.Utext = "Address";
		this.lb_Status.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_Status.Appearance.Options.UseFont = true;
		this.lb_Status.EditControl = null;
		this.lb_Status.EditValue = "Status";
		this.lb_Status.FieldName = null;
		this.lb_Status.HideWhenEmpty = false;
		this.lb_Status.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Status.Location = new System.Drawing.Point(13, 80);
		this.lb_Status.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_Status.Name = "lb_Status";
		this.lb_Status.ReadOnly = false;
		this.lb_Status.SettingInVisible = false;
		this.lb_Status.SettingReadonly = false;
		this.lb_Status.ShowUnderLine = true;
		this.lb_Status.Size = new System.Drawing.Size(114, 19);
		this.lb_Status.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Status.TabIndex = 0;
		this.lb_Status.Utext = "Status";
		this.lb_Duration.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_Duration.Appearance.Options.UseFont = true;
		this.lb_Duration.EditControl = null;
		this.lb_Duration.EditValue = "Duration";
		this.lb_Duration.FieldName = null;
		this.lb_Duration.HideWhenEmpty = false;
		this.lb_Duration.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Duration.Location = new System.Drawing.Point(13, 62);
		this.lb_Duration.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_Duration.Name = "lb_Duration";
		this.lb_Duration.ReadOnly = false;
		this.lb_Duration.SettingInVisible = false;
		this.lb_Duration.SettingReadonly = false;
		this.lb_Duration.ShowUnderLine = true;
		this.lb_Duration.Size = new System.Drawing.Size(114, 19);
		this.lb_Duration.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Duration.TabIndex = 0;
		this.lb_Duration.Utext = "Duration";
		this.lb_EndTime.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_EndTime.Appearance.Options.UseFont = true;
		this.lb_EndTime.EditControl = null;
		this.lb_EndTime.EditValue = "End Time";
		this.lb_EndTime.FieldName = null;
		this.lb_EndTime.HideWhenEmpty = false;
		this.lb_EndTime.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_EndTime.Location = new System.Drawing.Point(13, 42);
		this.lb_EndTime.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_EndTime.Name = "lb_EndTime";
		this.lb_EndTime.ReadOnly = false;
		this.lb_EndTime.SettingInVisible = false;
		this.lb_EndTime.SettingReadonly = false;
		this.lb_EndTime.ShowUnderLine = true;
		this.lb_EndTime.Size = new System.Drawing.Size(114, 19);
		this.lb_EndTime.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_EndTime.TabIndex = 0;
		this.lb_EndTime.Utext = "End Time";
		this.lb_StartTime.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_StartTime.Appearance.Options.UseFont = true;
		this.lb_StartTime.EditControl = null;
		this.lb_StartTime.EditValue = "Start Time";
		this.lb_StartTime.FieldName = null;
		this.lb_StartTime.HideWhenEmpty = false;
		this.lb_StartTime.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_StartTime.Location = new System.Drawing.Point(13, 23);
		this.lb_StartTime.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_StartTime.Name = "lb_StartTime";
		this.lb_StartTime.ReadOnly = false;
		this.lb_StartTime.SettingInVisible = false;
		this.lb_StartTime.SettingReadonly = false;
		this.lb_StartTime.ShowUnderLine = true;
		this.lb_StartTime.Size = new System.Drawing.Size(114, 19);
		this.lb_StartTime.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_StartTime.TabIndex = 0;
		this.lb_StartTime.Utext = "Start Time";
		this.lb_Remarks.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_Remarks.Appearance.Options.UseFont = true;
		this.lb_Remarks.EditControl = null;
		this.lb_Remarks.EditValue = "Remarks";
		this.lb_Remarks.FieldName = null;
		this.lb_Remarks.HideWhenEmpty = false;
		this.lb_Remarks.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Remarks.Location = new System.Drawing.Point(13, 5);
		this.lb_Remarks.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_Remarks.Name = "lb_Remarks";
		this.lb_Remarks.ReadOnly = false;
		this.lb_Remarks.SettingInVisible = false;
		this.lb_Remarks.SettingReadonly = false;
		this.lb_Remarks.ShowUnderLine = true;
		this.lb_Remarks.Size = new System.Drawing.Size(114, 19);
		this.lb_Remarks.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Remarks.TabIndex = 0;
		this.lb_Remarks.Utext = "Remarks";
		this.tab_Content.Controls.Add(this.memoEdit_Note);
		this.tab_Content.EditControl = null;
		this.tab_Content.EditValue = "Content";
		this.tab_Content.FieldName = null;
		this.tab_Content.IsSearchControl = false;
		this.tab_Content.Name = "tab_Content";
		this.tab_Content.ReadOnly = false;
		this.tab_Content.SettingInVisible = false;
		this.tab_Content.SettingReadonly = false;
		this.tab_Content.Size = new System.Drawing.Size(774, 428);
		this.tab_Content.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Content.Text = "Content";
		this.memoEdit_Note.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.memoEdit_Note.EditControl = null;
		this.memoEdit_Note.FieldName = null;
		this.memoEdit_Note.IsSearchControl = false;
		this.memoEdit_Note.Location = new System.Drawing.Point(20, 19);
		this.memoEdit_Note.Name = "memoEdit_Note";
		this.memoEdit_Note.SettingInVisible = false;
		this.memoEdit_Note.SettingReadonly = false;
		this.memoEdit_Note.Size = new System.Drawing.Size(702, 401);
		this.memoEdit_Note.TabIndex = 0;
		this.memoEdit_Note.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.tap_LinkedDocument.Controls.Add(this.txtParentType);
		this.tap_LinkedDocument.Controls.Add(this.txtParentId);
		this.tap_LinkedDocument.Controls.Add(this.txtPrevActivity);
		this.tap_LinkedDocument.Controls.Add(this.cbDocNum);
		this.tap_LinkedDocument.Controls.Add(this.cbDocType);
		this.tap_LinkedDocument.Controls.Add(this.chkShowDocumentRelatedBP);
		this.tap_LinkedDocument.Controls.Add(this.chkLinkDraft);
		this.tap_LinkedDocument.Controls.Add(this.lb_SourceObjectNo);
		this.tap_LinkedDocument.Controls.Add(this.lb_SourceObjectType);
		this.tap_LinkedDocument.Controls.Add(this.lb_PreviousActivity);
		this.tap_LinkedDocument.Controls.Add(this.lb_DocumentNumber);
		this.tap_LinkedDocument.Controls.Add(this.lb_DocumentType);
		this.tap_LinkedDocument.EditControl = null;
		this.tap_LinkedDocument.EditValue = "Linked Document";
		this.tap_LinkedDocument.FieldName = null;
		this.tap_LinkedDocument.IsSearchControl = false;
		this.tap_LinkedDocument.Name = "tap_LinkedDocument";
		this.tap_LinkedDocument.ReadOnly = false;
		this.tap_LinkedDocument.SettingInVisible = false;
		this.tap_LinkedDocument.SettingReadonly = false;
		this.tap_LinkedDocument.Size = new System.Drawing.Size(774, 428);
		this.tap_LinkedDocument.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tap_LinkedDocument.Text = "Linked Document";
		this.txtParentType.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtParentType.EditControl = null;
		this.txtParentType.FieldName = null;
		this.txtParentType.IsSearchControl = false;
		this.txtParentType.Location = new System.Drawing.Point(456, 29);
		this.txtParentType.Name = "txtParentType";
		this.txtParentType.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtParentType.Properties.Appearance.Options.UseFont = true;
		this.txtParentType.Properties.AutoHeight = false;
		this.txtParentType.Properties.ReadOnly = true;
		this.txtParentType.SettingInVisible = false;
		this.txtParentType.SettingReadonly = false;
		this.txtParentType.Size = new System.Drawing.Size(135, 18);
		this.txtParentType.TabIndex = 52;
		this.txtParentId.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtParentId.EditControl = null;
		this.txtParentId.FieldName = null;
		this.txtParentId.IsSearchControl = false;
		this.txtParentId.Location = new System.Drawing.Point(456, 52);
		this.txtParentId.Name = "txtParentId";
		this.txtParentId.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtParentId.Properties.Appearance.Options.UseFont = true;
		this.txtParentId.Properties.AutoHeight = false;
		this.txtParentId.Properties.ReadOnly = true;
		this.txtParentId.SettingInVisible = false;
		this.txtParentId.SettingReadonly = false;
		this.txtParentId.Size = new System.Drawing.Size(135, 18);
		this.txtParentId.TabIndex = 51;
		this.txtPrevActivity.EditControl = null;
		this.txtPrevActivity.FieldName = null;
		this.txtPrevActivity.IsSearchControl = false;
		this.txtPrevActivity.Location = new System.Drawing.Point(121, 104);
		this.txtPrevActivity.Name = "txtPrevActivity";
		this.txtPrevActivity.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtPrevActivity.Properties.Appearance.Options.UseFont = true;
		this.txtPrevActivity.Properties.AutoHeight = false;
		this.txtPrevActivity.Properties.ReadOnly = true;
		this.txtPrevActivity.SettingInVisible = false;
		this.txtPrevActivity.SettingReadonly = false;
		this.txtPrevActivity.Size = new System.Drawing.Size(135, 18);
		this.txtPrevActivity.TabIndex = 50;
		this.cbDocNum.EditControl = null;
		this.cbDocNum.FieldName = null;
		this.cbDocNum.IsSearchControl = false;
		this.cbDocNum.Location = new System.Drawing.Point(121, 55);
		this.cbDocNum.Name = "cbDocNum";
		this.cbDocNum.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbDocNum.Properties.Appearance.Options.UseFont = true;
		this.cbDocNum.Properties.AutoHeight = false;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.cbDocNum.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cbDocNum.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cbDocNum_Properties_ButtonPressed);
		this.cbDocNum.SettingInVisible = false;
		this.cbDocNum.SettingReadonly = false;
		this.cbDocNum.Size = new System.Drawing.Size(135, 18);
		this.cbDocNum.TabIndex = 45;
		this.cbDocNum.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.cbDocType.AutoAdjustItemHeight = false;
		this.cbDocType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
		this.cbDocType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbDocType.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbDocType.IsSearchControl = false;
		this.cbDocType.ItemHeight = 14;
		this.cbDocType.Location = new System.Drawing.Point(121, 29);
		this.cbDocType.Name = "cbDocType";
		this.cbDocType.SeparatorColor = System.Drawing.Color.Black;
		this.cbDocType.SeparatorMargin = 1;
		this.cbDocType.SeparatorStyle = System.Drawing.Drawing2D.DashStyle.Solid;
		this.cbDocType.SeparatorWidth = 1;
		this.cbDocType.Size = new System.Drawing.Size(135, 20);
		this.cbDocType.TabIndex = 44;
		this.cbDocType.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbDocType_DrawItem);
		this.cbDocType.SelectedValueChanged += new System.EventHandler(this.cbDocType_SelectedValueChanged);
		this.chkShowDocumentRelatedBP.EditControl = null;
		this.chkShowDocumentRelatedBP.EditValue = "N";
		this.chkShowDocumentRelatedBP.FieldName = null;
		this.chkShowDocumentRelatedBP.IsSearchControl = false;
		this.chkShowDocumentRelatedBP.Location = new System.Drawing.Point(17, 81);
		this.chkShowDocumentRelatedBP.Name = "chkShowDocumentRelatedBP";
		this.chkShowDocumentRelatedBP.Properties.Caption = "Show Documents Related to the BP";
		this.chkShowDocumentRelatedBP.Properties.ValueChecked = "Y";
		this.chkShowDocumentRelatedBP.Properties.ValueUnchecked = "N";
		this.chkShowDocumentRelatedBP.SettingInVisible = false;
		this.chkShowDocumentRelatedBP.SettingReadonly = false;
		this.chkShowDocumentRelatedBP.Size = new System.Drawing.Size(201, 20);
		this.chkShowDocumentRelatedBP.TabIndex = 46;
		this.chkShowDocumentRelatedBP.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chkLinkDraft.EditControl = null;
		this.chkLinkDraft.FieldName = null;
		this.chkLinkDraft.IsSearchControl = false;
		this.chkLinkDraft.Location = new System.Drawing.Point(17, 7);
		this.chkLinkDraft.Name = "chkLinkDraft";
		this.chkLinkDraft.Properties.Caption = "Link Draft";
		this.chkLinkDraft.SettingInVisible = false;
		this.chkLinkDraft.SettingReadonly = false;
		this.chkLinkDraft.Size = new System.Drawing.Size(75, 20);
		this.chkLinkDraft.TabIndex = 43;
		this.chkLinkDraft.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.lb_SourceObjectNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_SourceObjectNo.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_SourceObjectNo.Appearance.Options.UseFont = true;
		this.lb_SourceObjectNo.EditControl = null;
		this.lb_SourceObjectNo.EditValue = "Source Object No.";
		this.lb_SourceObjectNo.FieldName = null;
		this.lb_SourceObjectNo.HideWhenEmpty = false;
		this.lb_SourceObjectNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_SourceObjectNo.Location = new System.Drawing.Point(352, 53);
		this.lb_SourceObjectNo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_SourceObjectNo.Name = "lb_SourceObjectNo";
		this.lb_SourceObjectNo.ReadOnly = false;
		this.lb_SourceObjectNo.SettingInVisible = false;
		this.lb_SourceObjectNo.SettingReadonly = false;
		this.lb_SourceObjectNo.ShowUnderLine = false;
		this.lb_SourceObjectNo.Size = new System.Drawing.Size(110, 19);
		this.lb_SourceObjectNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_SourceObjectNo.TabIndex = 0;
		this.lb_SourceObjectNo.Utext = "Source Object No.";
		this.lb_SourceObjectType.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_SourceObjectType.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_SourceObjectType.Appearance.Options.UseFont = true;
		this.lb_SourceObjectType.EditControl = null;
		this.lb_SourceObjectType.EditValue = "Source Object Type";
		this.lb_SourceObjectType.FieldName = null;
		this.lb_SourceObjectType.HideWhenEmpty = false;
		this.lb_SourceObjectType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_SourceObjectType.Location = new System.Drawing.Point(352, 30);
		this.lb_SourceObjectType.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_SourceObjectType.Name = "lb_SourceObjectType";
		this.lb_SourceObjectType.ReadOnly = false;
		this.lb_SourceObjectType.SettingInVisible = false;
		this.lb_SourceObjectType.SettingReadonly = false;
		this.lb_SourceObjectType.ShowUnderLine = false;
		this.lb_SourceObjectType.Size = new System.Drawing.Size(110, 19);
		this.lb_SourceObjectType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_SourceObjectType.TabIndex = 0;
		this.lb_SourceObjectType.Utext = "Source Object Type";
		this.lb_PreviousActivity.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_PreviousActivity.Appearance.Options.UseFont = true;
		this.lb_PreviousActivity.EditControl = null;
		this.lb_PreviousActivity.EditValue = "Previous Activity";
		this.lb_PreviousActivity.FieldName = null;
		this.lb_PreviousActivity.HideWhenEmpty = false;
		this.lb_PreviousActivity.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_PreviousActivity.Location = new System.Drawing.Point(17, 105);
		this.lb_PreviousActivity.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_PreviousActivity.Name = "lb_PreviousActivity";
		this.lb_PreviousActivity.ReadOnly = false;
		this.lb_PreviousActivity.SettingInVisible = false;
		this.lb_PreviousActivity.SettingReadonly = false;
		this.lb_PreviousActivity.ShowUnderLine = false;
		this.lb_PreviousActivity.Size = new System.Drawing.Size(110, 19);
		this.lb_PreviousActivity.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_PreviousActivity.TabIndex = 0;
		this.lb_PreviousActivity.Utext = "Previous Activity";
		this.lb_DocumentNumber.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_DocumentNumber.Appearance.Options.UseFont = true;
		this.lb_DocumentNumber.EditControl = null;
		this.lb_DocumentNumber.EditValue = "Document Number";
		this.lb_DocumentNumber.FieldName = null;
		this.lb_DocumentNumber.HideWhenEmpty = false;
		this.lb_DocumentNumber.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DocumentNumber.Location = new System.Drawing.Point(17, 55);
		this.lb_DocumentNumber.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_DocumentNumber.Name = "lb_DocumentNumber";
		this.lb_DocumentNumber.ReadOnly = false;
		this.lb_DocumentNumber.SettingInVisible = false;
		this.lb_DocumentNumber.SettingReadonly = false;
		this.lb_DocumentNumber.ShowUnderLine = false;
		this.lb_DocumentNumber.Size = new System.Drawing.Size(110, 19);
		this.lb_DocumentNumber.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DocumentNumber.TabIndex = 0;
		this.lb_DocumentNumber.Utext = "Document Number";
		this.lb_DocumentType.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_DocumentType.Appearance.Options.UseFont = true;
		this.lb_DocumentType.EditControl = null;
		this.lb_DocumentType.EditValue = "Document Type";
		this.lb_DocumentType.FieldName = null;
		this.lb_DocumentType.HideWhenEmpty = false;
		this.lb_DocumentType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DocumentType.Location = new System.Drawing.Point(19, 32);
		this.lb_DocumentType.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_DocumentType.Name = "lb_DocumentType";
		this.lb_DocumentType.ReadOnly = false;
		this.lb_DocumentType.SettingInVisible = false;
		this.lb_DocumentType.SettingReadonly = false;
		this.lb_DocumentType.ShowUnderLine = false;
		this.lb_DocumentType.Size = new System.Drawing.Size(108, 19);
		this.lb_DocumentType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DocumentType.TabIndex = 0;
		this.lb_DocumentType.Utext = "Document Type";
		this.tap_Att.Controls.Add(this.ucAttachment1);
		this.tap_Att.EditControl = null;
		this.tap_Att.EditValue = "Attachments";
		this.tap_Att.FieldName = null;
		this.tap_Att.IsSearchControl = false;
		this.tap_Att.Name = "tap_Att";
		this.tap_Att.ReadOnly = false;
		this.tap_Att.SettingInVisible = false;
		this.tap_Att.SettingReadonly = false;
		this.tap_Att.Size = new System.Drawing.Size(774, 428);
		this.tap_Att.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tap_Att.Text = "Attachments";
		this.ucAttachment1.DataSource = null;
		this.ucAttachment1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucAttachment1.FormId = "oActivitiesForOpportunity";
		this.ucAttachment1.ItemId = "ATC1";
		this.ucAttachment1.Location = new System.Drawing.Point(0, 0);
		this.ucAttachment1.Name = "ucAttachment1";
		this.ucAttachment1.Size = new System.Drawing.Size(774, 428);
		this.ucAttachment1.TabIndex = 0;
		this.chkPerson.EditControl = null;
		this.chkPerson.EditValue = "N";
		this.chkPerson.FieldName = null;
		this.chkPerson.IsSearchControl = false;
		this.chkPerson.Location = new System.Drawing.Point(10, 98);
		this.chkPerson.Name = "chkPerson";
		this.chkPerson.Properties.Caption = "Personal";
		this.chkPerson.Properties.ValueChecked = "Y";
		this.chkPerson.Properties.ValueUnchecked = "N";
		this.chkPerson.SettingInVisible = false;
		this.chkPerson.SettingReadonly = false;
		this.chkPerson.Size = new System.Drawing.Size(75, 20);
		this.chkPerson.TabIndex = 38;
		this.chkPerson.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.lb_Activity.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_Activity.Appearance.Options.UseFont = true;
		this.lb_Activity.EditControl = null;
		this.lb_Activity.EditValue = "Activity";
		this.lb_Activity.FieldName = null;
		this.lb_Activity.HideWhenEmpty = false;
		this.lb_Activity.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Activity.Location = new System.Drawing.Point(10, 5);
		this.lb_Activity.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_Activity.Name = "lb_Activity";
		this.lb_Activity.ReadOnly = false;
		this.lb_Activity.SettingInVisible = false;
		this.lb_Activity.SettingReadonly = false;
		this.lb_Activity.ShowUnderLine = true;
		this.lb_Activity.Size = new System.Drawing.Size(110, 19);
		this.lb_Activity.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Activity.TabIndex = 0;
		this.lb_Activity.Utext = "Activity";
		this.lb_Type.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_Type.Appearance.Options.UseFont = true;
		this.lb_Type.EditControl = null;
		this.lb_Type.EditValue = "Type";
		this.lb_Type.FieldName = null;
		this.lb_Type.HideWhenEmpty = false;
		this.lb_Type.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Type.Location = new System.Drawing.Point(10, 24);
		this.lb_Type.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_Type.Name = "lb_Type";
		this.lb_Type.ReadOnly = false;
		this.lb_Type.SettingInVisible = false;
		this.lb_Type.SettingReadonly = false;
		this.lb_Type.ShowUnderLine = true;
		this.lb_Type.Size = new System.Drawing.Size(110, 19);
		this.lb_Type.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Type.TabIndex = 0;
		this.lb_Type.Utext = "Type";
		this.lb_TelephoneNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_TelephoneNo.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_TelephoneNo.Appearance.Options.UseFont = true;
		this.lb_TelephoneNo.EditControl = null;
		this.lb_TelephoneNo.EditValue = "Telephone No.";
		this.lb_TelephoneNo.FieldName = null;
		this.lb_TelephoneNo.HideWhenEmpty = false;
		this.lb_TelephoneNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_TelephoneNo.Location = new System.Drawing.Point(375, 79);
		this.lb_TelephoneNo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_TelephoneNo.Name = "lb_TelephoneNo";
		this.lb_TelephoneNo.ReadOnly = false;
		this.lb_TelephoneNo.SettingInVisible = false;
		this.lb_TelephoneNo.SettingReadonly = false;
		this.lb_TelephoneNo.ShowUnderLine = true;
		this.lb_TelephoneNo.Size = new System.Drawing.Size(120, 19);
		this.lb_TelephoneNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_TelephoneNo.TabIndex = 0;
		this.lb_TelephoneNo.Utext = "Telephone No.";
		this.lb_ContactPerson.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_ContactPerson.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_ContactPerson.Appearance.Options.UseFont = true;
		this.lb_ContactPerson.EditControl = null;
		this.lb_ContactPerson.EditValue = "Contact Person";
		this.lb_ContactPerson.FieldName = null;
		this.lb_ContactPerson.HideWhenEmpty = false;
		this.lb_ContactPerson.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ContactPerson.Location = new System.Drawing.Point(375, 61);
		this.lb_ContactPerson.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_ContactPerson.Name = "lb_ContactPerson";
		this.lb_ContactPerson.ReadOnly = false;
		this.lb_ContactPerson.SettingInVisible = false;
		this.lb_ContactPerson.SettingReadonly = false;
		this.lb_ContactPerson.ShowUnderLine = true;
		this.lb_ContactPerson.Size = new System.Drawing.Size(116, 19);
		this.lb_ContactPerson.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ContactPerson.TabIndex = 0;
		this.lb_ContactPerson.Utext = "Contact Person";
		this.lb_Subject.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_Subject.Appearance.Options.UseFont = true;
		this.lb_Subject.EditControl = null;
		this.lb_Subject.EditValue = "Subject";
		this.lb_Subject.FieldName = null;
		this.lb_Subject.HideWhenEmpty = false;
		this.lb_Subject.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Subject.Location = new System.Drawing.Point(10, 42);
		this.lb_Subject.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_Subject.Name = "lb_Subject";
		this.lb_Subject.ReadOnly = false;
		this.lb_Subject.SettingInVisible = false;
		this.lb_Subject.SettingReadonly = false;
		this.lb_Subject.ShowUnderLine = true;
		this.lb_Subject.Size = new System.Drawing.Size(110, 19);
		this.lb_Subject.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Subject.TabIndex = 0;
		this.lb_Subject.Utext = "Subject";
		this.lb_BPName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_BPName.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_BPName.Appearance.Options.UseFont = true;
		this.lb_BPName.EditControl = null;
		this.lb_BPName.EditValue = "BP Name";
		this.lb_BPName.FieldName = null;
		this.lb_BPName.HideWhenEmpty = false;
		this.lb_BPName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BPName.Location = new System.Drawing.Point(375, 41);
		this.lb_BPName.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_BPName.Name = "lb_BPName";
		this.lb_BPName.ReadOnly = false;
		this.lb_BPName.SettingInVisible = false;
		this.lb_BPName.SettingReadonly = false;
		this.lb_BPName.ShowUnderLine = true;
		this.lb_BPName.Size = new System.Drawing.Size(120, 19);
		this.lb_BPName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BPName.TabIndex = 0;
		this.lb_BPName.Utext = "BP Name";
		this.lb_BPCode.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_BPCode.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_BPCode.Appearance.Options.UseFont = true;
		this.lb_BPCode.EditControl = null;
		this.lb_BPCode.EditValue = "BP Code";
		this.lb_BPCode.FieldName = null;
		this.lb_BPCode.HideWhenEmpty = false;
		this.lb_BPCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BPCode.Location = new System.Drawing.Point(375, 22);
		this.lb_BPCode.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_BPCode.Name = "lb_BPCode";
		this.lb_BPCode.ReadOnly = false;
		this.lb_BPCode.SettingInVisible = false;
		this.lb_BPCode.SettingReadonly = false;
		this.lb_BPCode.ShowUnderLine = true;
		this.lb_BPCode.Size = new System.Drawing.Size(120, 19);
		this.lb_BPCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BPCode.TabIndex = 0;
		this.lb_BPCode.Utext = "BP Code";
		this.lb_AssignedTo.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_AssignedTo.Appearance.Options.UseFont = true;
		this.lb_AssignedTo.EditControl = null;
		this.lb_AssignedTo.EditValue = "Assigned To";
		this.lb_AssignedTo.FieldName = null;
		this.lb_AssignedTo.HideWhenEmpty = false;
		this.lb_AssignedTo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_AssignedTo.Location = new System.Drawing.Point(10, 61);
		this.lb_AssignedTo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_AssignedTo.Name = "lb_AssignedTo";
		this.lb_AssignedTo.ReadOnly = false;
		this.lb_AssignedTo.SettingInVisible = false;
		this.lb_AssignedTo.SettingReadonly = false;
		this.lb_AssignedTo.ShowUnderLine = true;
		this.lb_AssignedTo.Size = new System.Drawing.Size(110, 19);
		this.lb_AssignedTo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_AssignedTo.TabIndex = 0;
		this.lb_AssignedTo.Utext = "Assigned To";
		this.lb_Number.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lb_Number.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_Number.Appearance.Options.UseFont = true;
		this.lb_Number.EditControl = null;
		this.lb_Number.EditValue = "Number";
		this.lb_Number.FieldName = null;
		this.lb_Number.HideWhenEmpty = false;
		this.lb_Number.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Number.Location = new System.Drawing.Point(375, 3);
		this.lb_Number.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_Number.Name = "lb_Number";
		this.lb_Number.ReadOnly = false;
		this.lb_Number.SettingInVisible = false;
		this.lb_Number.SettingReadonly = false;
		this.lb_Number.ShowUnderLine = true;
		this.lb_Number.Size = new System.Drawing.Size(120, 19);
		this.lb_Number.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Number.TabIndex = 0;
		this.lb_Number.Utext = "Number";
		this.txtBpCode.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtBpCode.EditControl = null;
		this.txtBpCode.FieldName = null;
		this.txtBpCode.IsSearchControl = false;
		this.txtBpCode.Location = new System.Drawing.Point(490, 21);
		this.txtBpCode.Name = "txtBpCode";
		this.txtBpCode.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtBpCode.Properties.Appearance.Options.UseFont = true;
		this.txtBpCode.Properties.AutoHeight = false;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.txtBpCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtBpCode.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtBpCode_Properties_ButtonPressed);
		this.txtBpCode.SettingInVisible = false;
		this.txtBpCode.SettingReadonly = false;
		this.txtBpCode.Size = new System.Drawing.Size(117, 18);
		this.txtBpCode.TabIndex = 8;
		this.txtBpCode.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txtBpCode.TextChanged += new System.EventHandler(this.txtBpCode_TextChanged);
		this.txtBpCode.Enter += new System.EventHandler(this.txtBpCode_Enter);
		this.txtBpCode.Leave += new System.EventHandler(this.txtBpCode_Leave);
		this.lb_AssignedBy.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lb_AssignedBy.Appearance.Options.UseFont = true;
		this.lb_AssignedBy.EditControl = null;
		this.lb_AssignedBy.EditValue = "Assigned By";
		this.lb_AssignedBy.FieldName = null;
		this.lb_AssignedBy.HideWhenEmpty = false;
		this.lb_AssignedBy.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_AssignedBy.Location = new System.Drawing.Point(10, 79);
		this.lb_AssignedBy.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_AssignedBy.Name = "lb_AssignedBy";
		this.lb_AssignedBy.ReadOnly = false;
		this.lb_AssignedBy.SettingInVisible = false;
		this.lb_AssignedBy.SettingReadonly = false;
		this.lb_AssignedBy.ShowUnderLine = true;
		this.lb_AssignedBy.Size = new System.Drawing.Size(110, 19);
		this.lb_AssignedBy.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_AssignedBy.TabIndex = 0;
		this.lb_AssignedBy.Utext = "Assigned By";
		this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.deleteToolStripMenuItem, this.relayActiviToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(165, 48);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
		this.deleteToolStripMenuItem.Text = "Remove";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.relayActiviToolStripMenuItem.Name = "relayActiviToolStripMenuItem";
		this.relayActiviToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
		this.relayActiviToolStripMenuItem.Text = "Related Activities";
		this.relayActiviToolStripMenuItem.Click += new System.EventHandler(this.relayActiviToolStripMenuItem_Click);
		this.txtBpName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtBpName.EditControl = null;
		this.txtBpName.FieldName = null;
		this.txtBpName.IsSearchControl = false;
		this.txtBpName.Location = new System.Drawing.Point(490, 40);
		this.txtBpName.Name = "txtBpName";
		this.txtBpName.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtBpName.Properties.Appearance.Options.UseFont = true;
		this.txtBpName.Properties.AutoHeight = false;
		this.txtBpName.Properties.Mask.SaveLiteral = false;
		this.txtBpName.Properties.Mask.ShowPlaceHolders = false;
		this.txtBpName.Properties.Mask.UseMaskAsDisplayFormat = true;
		this.txtBpName.Properties.ReadOnly = true;
		this.txtBpName.SettingInVisible = false;
		this.txtBpName.SettingReadonly = false;
		this.txtBpName.Size = new System.Drawing.Size(210, 18);
		this.txtBpName.TabIndex = 48;
		this.txtBpType.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtBpType.EditControl = null;
		this.txtBpType.FieldName = null;
		this.txtBpType.IsSearchControl = false;
		this.txtBpType.Location = new System.Drawing.Point(614, 21);
		this.txtBpType.Name = "txtBpType";
		this.txtBpType.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtBpType.Properties.Appearance.Options.UseFont = true;
		this.txtBpType.Properties.AutoHeight = false;
		this.txtBpType.Properties.Mask.SaveLiteral = false;
		this.txtBpType.Properties.Mask.ShowPlaceHolders = false;
		this.txtBpType.Properties.Mask.UseMaskAsDisplayFormat = true;
		this.txtBpType.Properties.ReadOnly = true;
		this.txtBpType.SettingInVisible = false;
		this.txtBpType.SettingReadonly = false;
		this.txtBpType.Size = new System.Drawing.Size(86, 18);
		this.txtBpType.TabIndex = 49;
		this.txtNumber.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtNumber.EditControl = null;
		this.txtNumber.FieldName = null;
		this.txtNumber.IsSearchControl = false;
		this.txtNumber.Location = new System.Drawing.Point(490, 2);
		this.txtNumber.Name = "txtNumber";
		this.txtNumber.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txtNumber.Properties.Appearance.Options.UseFont = true;
		this.txtNumber.Properties.AutoHeight = false;
		this.txtNumber.Properties.Mask.SaveLiteral = false;
		this.txtNumber.Properties.Mask.ShowPlaceHolders = false;
		this.txtNumber.Properties.Mask.UseMaskAsDisplayFormat = true;
		this.txtNumber.Properties.ReadOnly = true;
		this.txtNumber.SettingInVisible = false;
		this.txtNumber.SettingReadonly = false;
		this.txtNumber.Size = new System.Drawing.Size(210, 18);
		this.txtNumber.TabIndex = 50;
		this.txtNumber.Tag = "OCLG,ClgCode";
		this.txtNumber.ToolTip = "OCLG,ClgCode";
		this.cbUser.EditControl = null;
		this.cbUser.FieldName = null;
		this.cbUser.IsSearchControl = false;
		this.cbUser.Location = new System.Drawing.Point(119, 59);
		this.cbUser.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.cbUser.Name = "cbUser";
		this.cbUser.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbUser.Properties.Appearance.Options.UseFont = true;
		this.cbUser.Properties.AutoHeight = false;
		this.cbUser.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbUser.Properties.Items.AddRange(new object[2] { "User", "Employee" });
		this.cbUser.SettingInVisible = false;
		this.cbUser.SettingReadonly = false;
		this.cbUser.Size = new System.Drawing.Size(82, 18);
		this.cbUser.TabIndex = 54;
		this.cbUser.SelectedIndexChanged += new System.EventHandler(this.cbUser_SelectedValueChanged);
		this.cbActivity.EditControl = null;
		this.cbActivity.FieldName = null;
		this.cbActivity.IsSearchControl = false;
		this.cbActivity.Location = new System.Drawing.Point(119, 2);
		this.cbActivity.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.cbActivity.Name = "cbActivity";
		this.cbActivity.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbActivity.Properties.Appearance.Options.UseFont = true;
		this.cbActivity.Properties.AutoHeight = false;
		this.cbActivity.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbActivity.Properties.IsSearchControl = false;
		this.cbActivity.Properties.NullText = "";
		this.cbActivity.SettingInVisible = false;
		this.cbActivity.SettingReadonly = false;
		this.cbActivity.Size = new System.Drawing.Size(214, 18);
		this.cbActivity.TabIndex = 58;
		this.cbActivity.Popup += new System.EventHandler(this.luk_Popup);
		this.cbActivity.EditValueChanged += new System.EventHandler(this.cbActivity1_EditValueChanged);
		this.cbAttendUser.EditControl = null;
		this.cbAttendUser.FieldName = null;
		this.cbAttendUser.IsSearchControl = false;
		this.cbAttendUser.Location = new System.Drawing.Point(209, 59);
		this.cbAttendUser.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.cbAttendUser.Name = "cbAttendUser";
		this.cbAttendUser.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.cbAttendUser.Properties.Appearance.Options.UseFont = true;
		this.cbAttendUser.Properties.AutoHeight = false;
		this.cbAttendUser.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbAttendUser.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ReqName", "ReqName")
		});
		this.cbAttendUser.Properties.DisplayMember = "ReqName";
		this.cbAttendUser.Properties.IsSearchControl = false;
		this.cbAttendUser.Properties.NullText = "";
		this.cbAttendUser.Properties.ValueMember = "EmpID";
		this.cbAttendUser.SettingInVisible = false;
		this.cbAttendUser.SettingReadonly = false;
		this.cbAttendUser.Size = new System.Drawing.Size(124, 18);
		this.cbAttendUser.TabIndex = 62;
		this.cbAttendUser.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(790, 596);
		this.ContextMenuStrip = this.contextMenuStrip1;
		base.Controls.Add(this.cbAttendUser);
		base.Controls.Add(this.txtNumber);
		base.Controls.Add(this.cbActivity);
		base.Controls.Add(this.cbUser);
		base.Controls.Add(this.txtBpType);
		base.Controls.Add(this.txtBpName);
		base.Controls.Add(this.txtBpCode);
		base.Controls.Add(this.luk_Subject);
		base.Controls.Add(this.luk_Type);
		base.Controls.Add(this.cb_ContactPerson);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControl1);
		base.Controls.Add(this.txtAssignedBy);
		base.Controls.Add(this.barDockControl2);
		base.Controls.Add(this.txtTelePhone);
		base.Controls.Add(this.lb_Subject);
		base.Controls.Add(this.lb_AssignedBy);
		base.Controls.Add(this.btAdd);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.lb_Number);
		base.Controls.Add(this.Xtap);
		base.Controls.Add(this.lb_AssignedTo);
		base.Controls.Add(this.chkPerson);
		base.Controls.Add(this.lb_BPCode);
		base.Controls.Add(this.lb_BPName);
		base.Controls.Add(this.lb_Activity);
		base.Controls.Add(this.lb_ContactPerson);
		base.Controls.Add(this.lb_TelephoneNo);
		base.Controls.Add(this.lb_Type);
		base.FormId = "oActivitiesForOpportunity";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ActivitiesForOpportunity";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Activities";
		base.Load += new System.EventHandler(this.frm_ActivitiesForOpportunity_Load);
		((System.ComponentModel.ISupportInitialize)this.txtName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbRequester.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateEdit1.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateEdit1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.comboBoxEdit1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateEdit2.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateEdit2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCardCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.buttonEdit1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Subject.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Type.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_ContactPerson.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtAssignedBy.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtTelePhone.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Xtap).EndInit();
		this.Xtap.ResumeLayout(false);
		this.tap_General.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtState.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCountry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtRoom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCity.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtStreet.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtDetails.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbStatus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbRecurPat.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Meetting.Properties).EndInit();
		this.panel_Range.ResumeLayout(false);
		this.panel_Range.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.txtAfterMaxOccur.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBySeEndDat.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBySeEndDat.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dSeStartDat.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dSeStartDat.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtRemQty.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Tentative.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkInactive.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkClosed.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkReminder.Properties).EndInit();
		this.panel_Daily.ResumeLayout(false);
		this.panel_Daily.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.chkSun.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbMonth.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbMonth1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkSat.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbDayOfWeek.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbWeek.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkFri.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtDayInMonth.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkThu.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtInterval.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkWed.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkTue.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkMon.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbPriority.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtDuration.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtEndTime.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBeginTime.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dEnDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dEnDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dStart_Recontact.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dStart_Recontact.Properties).EndInit();
		this.tab_Content.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.memoEdit_Note.Properties).EndInit();
		this.tap_LinkedDocument.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtParentType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtParentId.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPrevActivity.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbDocNum.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkShowDocumentRelatedBP.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkLinkDraft.Properties).EndInit();
		this.tap_Att.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chkPerson.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBpCode.Properties).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtBpName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBpType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtNumber.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbUser.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbActivity.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbAttendUser.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
