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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.Administrator.SetUp;

public class frm_NotificationSetting : XtraFormEx
{
	private bool _isCancel;

	private bool _isChanging;

	private bool _isControlChanged;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private CheckEdit[] lstWeekDay = new CheckEdit[7];

	private string resultMessage = "";

	private string checkDayPicked = "";

	private string typeDay;

	private string typeTime;

	private string typeSpecify = "D";

	private int fromD;

	private int toD;

	private int specifyD;

	private int fromT;

	private int toT;

	private IContainer components;

	private SimpleButtonEx btn_Ok;

	private SimpleButtonEx btn_Cancel;

	private Panel panelDayOfWeeks;

	private CheckEditEx cb_Sunday;

	private CheckEditEx cb_Saturday;

	private CheckEditEx cb_Friday;

	private CheckEditEx cb_Thursday;

	private CheckEditEx cb_Wednesday;

	private CheckEditEx cb_Tuesday;

	private CheckEditEx cb_Monday;

	private RadioButton rdo_PickDay;

	private RadioButton rdo_NotReceiveDay;

	private RadioButton rdo_ReceiveDay;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private RadioButton rdo_NotReceiveTime;

	private RadioButton rdo_ReceiveTime;

	private LabelLine labelLine4;

	private LabelLine labelLine6;

	private LabelLine labelLine7;

	private LabelLine labelLine8;

	private LabelLine labelLine5;

	private LabelLine labelLine9;

	private LabelLine labelLine10;

	private LabelLine labelLine11;

	private Panel panel_day;

	private Panel panel_time;

	private Panel panel_additional;

	private LabelLine labelLine13;

	private CheckEditEx cb_DANotReceive;

	private CheckEditEx cb_DAReceive;

	private LabelLine labelLine12;

	private TextEditEx txt_SpecifyNoti;

	private LabelLine labelLine15;

	private Panel panel_specifyDay;

	private LookUpEdit cbb_DayNotReceiveTo;

	private LookUpEdit cbb_DayReceiveTo;

	private LookUpEdit cbb_DayNotReceiveFrom;

	private LookUpEdit cbb_DayReceiveFrom;

	private ComboBoxEditEx cbb_TypeSpecify;

	private Panel panel1;

	private RadioButton rdo_AllNoti;

	private RadioButton rdo_SpecifyNoti;

	private ComboBoxEditEx cbb_AlwaysSetting;

	private HourEdit tex_NotReceiveFrom;

	private HourEdit tex_NotReceiveTo;

	private HourEdit tex_ReceiveTo;

	private HourEdit tex_ReceiveFrom;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Ok.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Ok.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_NotificationSetting()
	{
		InitializeComponent();
		AddCheckBoxList();
		cbb_TypeSpecify.SelectedIndex = 0;
		rdo_PickDay.Checked = true;
		rdo_ReceiveTime.Checked = true;
		rdo_AllNoti.Checked = true;
		OnLoadAsync += frm_NotificationSetting_Load;
	}

	public void AddCheckBoxList()
	{
		cb_Sunday.Tag = 0;
		cb_Monday.Tag = 1;
		cb_Tuesday.Tag = 2;
		cb_Wednesday.Tag = 3;
		cb_Thursday.Tag = 4;
		cb_Friday.Tag = 5;
		cb_Saturday.Tag = 6;
		lstWeekDay[0] = cb_Sunday;
		lstWeekDay[1] = cb_Monday;
		lstWeekDay[2] = cb_Tuesday;
		lstWeekDay[3] = cb_Wednesday;
		lstWeekDay[4] = cb_Thursday;
		lstWeekDay[5] = cb_Friday;
		lstWeekDay[6] = cb_Saturday;
	}

	private async Task LoadData()
	{
		try
		{
			LoadSetting(await _httpClient.NotificationSettingClient.GetData(GlobalData.UserCodeLogin));
			FormMode = Enums.FormViewModeType.View;
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
			CheckEdit checkEdit = control as CheckEdit;
			if (control is System.Windows.Forms.ComboBox comboBox)
			{
				comboBox.SelectedIndexChanged += OnContentChanged;
				comboBox.SelectedValueChanged += OnContentChanged;
			}
			else if (checkEdit != null)
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

	public void LoadSetting(DataTable dt)
	{
		cbb_AlwaysSetting.SelectedIndex = 0;
		if (dt.IsNotNull())
		{
			if (Function.ToString(dt.Rows[0][1]) == "R")
			{
				rdo_ReceiveDay.Checked = true;
				cbb_DayReceiveFrom.EditValue = Function.ToString(dt.Rows[0][2]);
				cbb_DayReceiveTo.EditValue = Function.ToString(dt.Rows[0][3]);
			}
			else if (Function.ToString(dt.Rows[0][1]) == "N")
			{
				rdo_NotReceiveDay.Checked = true;
				cbb_DayNotReceiveFrom.EditValue = Function.ToString(dt.Rows[0][2]);
				cbb_DayNotReceiveTo.EditValue = Function.ToString(dt.Rows[0][3]);
			}
			else if (Function.ToString(dt.Rows[0][1]) == "AR")
			{
				cbb_AlwaysSetting.SelectedIndex = 1;
			}
			else if (Function.ToString(dt.Rows[0][1]) == "AN")
			{
				cbb_AlwaysSetting.SelectedIndex = 2;
			}
			else
			{
				rdo_PickDay.Checked = true;
				string[] array = Function.ToString(dt.Rows[0][4]).Split(",".ToArray());
				for (int i = 0; i < lstWeekDay.Length; i++)
				{
					for (int j = 0; j < array.Length; j++)
					{
						if (array[j] != "" && Function.ParseInt(lstWeekDay[i].Tag) == Function.ParseInt(Function.ToString(array[j])))
						{
							lstWeekDay[i].Checked = true;
						}
					}
				}
			}
			switch (Function.ToString(dt.Rows[0][9]))
			{
			case "A":
				rdo_AllNoti.Checked = true;
				break;
			case "D":
				cbb_TypeSpecify.SelectedIndex = 0;
				rdo_SpecifyNoti.Checked = true;
				break;
			case "W":
				cbb_TypeSpecify.SelectedIndex = 1;
				rdo_SpecifyNoti.Checked = true;
				break;
			case "M":
				cbb_TypeSpecify.SelectedIndex = 2;
				rdo_SpecifyNoti.Checked = true;
				break;
			case "Y":
				cbb_TypeSpecify.SelectedIndex = 3;
				rdo_SpecifyNoti.Checked = true;
				break;
			}
			txt_SpecifyNoti.Text = Function.ToString(dt.Rows[0][8]);
			if (Function.ToString(dt.Rows[0][5]) == "R")
			{
				rdo_ReceiveTime.Checked = true;
				int num = Function.ParseInt(dt.Rows[0][6]);
				int num2 = Function.ParseInt(dt.Rows[0][7]);
				tex_ReceiveFrom.EditValue = num;
				tex_ReceiveTo.EditValue = num2;
			}
			else if (Function.ToString(dt.Rows[0][5]) == "N")
			{
				rdo_NotReceiveTime.Checked = true;
				int num3 = Function.ParseInt(dt.Rows[0][6]);
				int num4 = Function.ParseInt(dt.Rows[0][7]);
				tex_NotReceiveFrom.EditValue = num3;
				tex_NotReceiveTo.EditValue = num4;
			}
			else if (Function.ToString(dt.Rows[0][5]) == "AR" && Function.ToString(dt.Rows[0][1]) != "AR")
			{
				cbb_AlwaysSetting.SelectedIndex = 3;
				panel_time.Enabled = false;
			}
		}
		else
		{
			rdo_AllNoti.Checked = true;
			cbb_AlwaysSetting.SelectedIndex = 1;
			txt_SpecifyNoti.Text = Function.ToString(0);
		}
	}

	public void AddItemCbb(LookUpEdit cbb)
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("Name");
		dataTable.Columns.Add("Value");
		DataRow dataRow = dataTable.NewRow();
		dataRow[0] = "Sunday";
		dataRow[1] = "0";
		dataTable.Rows.Add(dataRow);
		dataRow = dataTable.NewRow();
		dataRow[0] = "Monday";
		dataRow[1] = "1";
		dataTable.Rows.Add(dataRow);
		dataRow = dataTable.NewRow();
		dataRow[0] = "Tuesday";
		dataRow[1] = "2";
		dataTable.Rows.Add(dataRow);
		dataRow = dataTable.NewRow();
		dataRow[0] = "Wednesday";
		dataRow[1] = "3";
		dataTable.Rows.Add(dataRow);
		dataRow = dataTable.NewRow();
		dataRow[0] = "Thursday";
		dataRow[1] = "4";
		dataTable.Rows.Add(dataRow);
		dataRow = dataTable.NewRow();
		dataRow[0] = "Friday";
		dataRow[1] = "5";
		dataTable.Rows.Add(dataRow);
		dataRow = dataTable.NewRow();
		dataRow[0] = "Saturday";
		dataRow[1] = "6";
		dataTable.Rows.Add(dataRow);
		cbb.Properties.DataSource = dataTable;
	}

	public bool check()
	{
		int num = 0;
		for (int i = 0; i < lstWeekDay.Length; i++)
		{
			if (rdo_PickDay.Checked && lstWeekDay[i].Checked)
			{
				num++;
			}
		}
		if ((cbb_AlwaysSetting.SelectedIndex == 0 || cbb_AlwaysSetting.SelectedIndex == 3) && ((rdo_ReceiveDay.Checked && (cbb_DayReceiveFrom.EditValue == null || cbb_DayReceiveTo.EditValue == null)) || (rdo_NotReceiveDay.Checked && (cbb_DayNotReceiveFrom.EditValue == null || cbb_DayNotReceiveTo.EditValue == null))))
		{
			resultMessage = "You have not selected day";
			return false;
		}
		if ((cbb_AlwaysSetting.SelectedIndex == 0 || cbb_AlwaysSetting.SelectedIndex == 3) && rdo_PickDay.Checked && num == 0)
		{
			resultMessage = "You have not selected day";
			return false;
		}
		if ((cbb_AlwaysSetting.SelectedIndex == 0 || cbb_AlwaysSetting.SelectedIndex == 3) && rdo_ReceiveDay.Checked && Function.ParseInt(cbb_DayReceiveFrom.EditValue) > Function.ParseInt(cbb_DayReceiveTo.EditValue))
		{
			resultMessage = "Day in From box must be smaller than To box";
			return false;
		}
		if ((cbb_AlwaysSetting.SelectedIndex == 0 || cbb_AlwaysSetting.SelectedIndex == 3) && rdo_NotReceiveDay.Checked && Function.ParseInt(cbb_DayNotReceiveFrom.EditValue) > Function.ParseInt(cbb_DayNotReceiveTo.EditValue))
		{
			resultMessage = "Day in From box must be smaller than To box";
			return false;
		}
		if ((cbb_AlwaysSetting.SelectedIndex == 0 && rdo_ReceiveTime.Checked && (tex_ReceiveFrom.Text == string.Empty || tex_ReceiveTo.Text == string.Empty)) || (rdo_NotReceiveTime.Checked && (tex_NotReceiveFrom.Text == string.Empty || tex_NotReceiveTo.Text == string.Empty)))
		{
			resultMessage = "You can set value time in From box or To box";
			return false;
		}
		if (rdo_ReceiveTime.Checked && Function.ParseInt(tex_ReceiveFrom.EditValue) >= Function.ParseInt(tex_ReceiveTo.EditValue) && cbb_AlwaysSetting.SelectedIndex == 0)
		{
			resultMessage = "Time in From box must be smaller than To box";
			return false;
		}
		if (rdo_NotReceiveTime.Checked && Function.ParseInt(tex_NotReceiveFrom.EditValue) >= Function.ParseInt(tex_NotReceiveTo.EditValue) && cbb_AlwaysSetting.SelectedIndex == 0)
		{
			resultMessage = "Time in From box must be smaller than To box";
			return false;
		}
		return true;
	}

	public void SetValue()
	{
		specifyD = Function.ParseInt(txt_SpecifyNoti.Text);
		if (cbb_AlwaysSetting.SelectedIndex == 0)
		{
			if (rdo_ReceiveDay.Checked)
			{
				typeDay = "R";
				fromD = Function.ParseInt(cbb_DayReceiveFrom.EditValue);
				toD = Function.ParseInt(cbb_DayReceiveTo.EditValue);
				checkDayPicked = "";
			}
			else if (rdo_NotReceiveDay.Checked)
			{
				typeDay = "N";
				fromD = Function.ParseInt(cbb_DayNotReceiveFrom.EditValue);
				toD = Function.ParseInt(cbb_DayNotReceiveTo.EditValue);
				checkDayPicked = "";
			}
			else if (rdo_PickDay.Checked)
			{
				typeDay = "P";
				fromD = 0;
				toD = 0;
				for (int i = 0; i < lstWeekDay.Length; i++)
				{
					if (rdo_PickDay.Checked && lstWeekDay[i].Checked)
					{
						checkDayPicked = checkDayPicked + lstWeekDay[i].Tag?.ToString() + ",";
					}
				}
			}
			if (rdo_ReceiveTime.Checked && cbb_AlwaysSetting.SelectedIndex == 0)
			{
				typeTime = "R";
				fromT = Function.ParseInt(tex_ReceiveFrom.EditValue);
				toT = Function.ParseInt(tex_ReceiveTo.EditValue);
			}
			else if (rdo_NotReceiveTime.Checked && cbb_AlwaysSetting.SelectedIndex == 0)
			{
				typeTime = "N";
				fromT = Function.ParseInt(tex_NotReceiveFrom.EditValue);
				toT = Function.ParseInt(tex_NotReceiveTo.EditValue);
			}
		}
		else if (cbb_AlwaysSetting.SelectedIndex == 1)
		{
			typeDay = "AR";
			fromD = 0;
			toD = 0;
			checkDayPicked = "";
			typeTime = "AR";
			fromT = 0;
			toT = 0;
		}
		else if (cbb_AlwaysSetting.SelectedIndex == 2)
		{
			typeDay = "AN";
			fromD = 0;
			toD = 0;
			checkDayPicked = "";
			typeTime = "AN";
			fromT = 0;
			toT = 0;
		}
		else
		{
			if (cbb_AlwaysSetting.SelectedIndex != 3)
			{
				return;
			}
			if (rdo_ReceiveDay.Checked)
			{
				typeDay = "R";
				fromD = Function.ParseInt(cbb_DayReceiveFrom.EditValue);
				toD = Function.ParseInt(cbb_DayReceiveTo.EditValue);
				checkDayPicked = "";
			}
			else if (rdo_NotReceiveDay.Checked)
			{
				typeDay = "N";
				fromD = Function.ParseInt(cbb_DayNotReceiveFrom.EditValue);
				toD = Function.ParseInt(cbb_DayNotReceiveTo.EditValue);
				checkDayPicked = "";
			}
			else if (rdo_PickDay.Checked)
			{
				typeDay = "P";
				fromD = 0;
				toD = 0;
				for (int j = 0; j < lstWeekDay.Length; j++)
				{
					if (rdo_PickDay.Checked && lstWeekDay[j].Checked)
					{
						checkDayPicked = checkDayPicked + lstWeekDay[j].Tag?.ToString() + ",";
					}
				}
			}
			typeTime = "AR";
			fromT = 0;
			toT = 0;
		}
	}

	protected void OnContentChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update)
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

	private async void simpleButton1_Click(object sender, EventArgs e)
	{
		try
		{
			checkDayPicked = "";
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (check())
				{
					SetValue();
					if ((resultMessage = await HttpClientHelperFactory.GetHttpClient().NotificationSettingClient.SaveData(new NotificationSettingImpl
					{
						user_code = GlobalData.UserCodeLogin,
						typeDay = typeDay,
						fromDay = fromD,
						toDay = toD,
						pickDay = checkDayPicked,
						typeTime = typeTime,
						fromTime = fromT,
						toTime = toT,
						specifyNotification = specifyD,
						typeSpecify = typeSpecify
					})) == "Successful")
					{
						WinCommons.Success(Enums.MessageCode.None, resultMessage);
						FormMode = Enums.FormViewModeType.View;
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, resultMessage);
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, resultMessage);
				}
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

	private async Task frm_NotificationSetting_Load(object sender, EventArgs e)
	{
		AddItemCbb(cbb_DayReceiveFrom);
		AddItemCbb(cbb_DayNotReceiveTo);
		AddItemCbb(cbb_DayReceiveTo);
		AddItemCbb(cbb_DayNotReceiveFrom);
		await LoadData();
		SetEventControl(this);
	}

	private void simpleButton2_Click(object sender, EventArgs e)
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

	private void rdo_ReceiveDay_CheckedChanged_1(object sender, EventArgs e)
	{
		cbb_DayReceiveFrom.ReadOnly = false;
		cbb_DayReceiveTo.ReadOnly = false;
		cbb_DayNotReceiveFrom.ReadOnly = true;
		cbb_DayNotReceiveTo.ReadOnly = true;
		panelDayOfWeeks.Enabled = false;
	}

	private void rdo_NotReceiveDay_CheckedChanged_1(object sender, EventArgs e)
	{
		cbb_DayReceiveFrom.ReadOnly = true;
		cbb_DayReceiveTo.ReadOnly = true;
		cbb_DayNotReceiveFrom.ReadOnly = false;
		cbb_DayNotReceiveTo.ReadOnly = false;
		panelDayOfWeeks.Enabled = false;
	}

	private void rdo_PickDay_CheckedChanged_1(object sender, EventArgs e)
	{
		cbb_DayReceiveFrom.ReadOnly = true;
		cbb_DayReceiveTo.ReadOnly = true;
		cbb_DayNotReceiveFrom.ReadOnly = true;
		cbb_DayNotReceiveTo.ReadOnly = true;
		panelDayOfWeeks.Enabled = true;
	}

	private void rdo_ReceiveTime_CheckedChanged_1(object sender, EventArgs e)
	{
		tex_ReceiveFrom.ReadOnly = false;
		tex_ReceiveTo.ReadOnly = false;
		tex_NotReceiveFrom.ReadOnly = true;
		tex_NotReceiveTo.ReadOnly = true;
	}

	private void rdo_NotReceiveTime_CheckedChanged_1(object sender, EventArgs e)
	{
		tex_ReceiveFrom.ReadOnly = true;
		tex_ReceiveTo.ReadOnly = true;
		tex_NotReceiveFrom.ReadOnly = false;
		tex_NotReceiveTo.ReadOnly = false;
	}

	private void cbb_TypeSpecify_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (cbb_TypeSpecify.SelectedIndex == 0)
		{
			typeSpecify = "D";
		}
		else if (cbb_TypeSpecify.SelectedIndex == 1)
		{
			typeSpecify = "W";
		}
		else if (cbb_TypeSpecify.SelectedIndex == 2)
		{
			typeSpecify = "M";
		}
		else
		{
			typeSpecify = "Y";
		}
	}

	private void rdo_SpecifyNoti_CheckedChanged(object sender, EventArgs e)
	{
		if (cbb_TypeSpecify.SelectedIndex == 0)
		{
			typeSpecify = "D";
		}
		else if (cbb_TypeSpecify.SelectedIndex == 1)
		{
			typeSpecify = "W";
		}
		else if (cbb_TypeSpecify.SelectedIndex == 2)
		{
			typeSpecify = "M";
		}
		else
		{
			typeSpecify = "Y";
		}
		if (rdo_SpecifyNoti.Checked)
		{
			panel_specifyDay.Enabled = true;
		}
		else
		{
			panel_specifyDay.Enabled = false;
		}
	}

	private void rdo_AllNoti_CheckedChanged(object sender, EventArgs e)
	{
		typeSpecify = "A";
		if (rdo_AllNoti.Checked)
		{
			panel_specifyDay.Enabled = false;
		}
		else
		{
			panel_specifyDay.Enabled = true;
		}
	}

	private void cbb_AlwaysSetting_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (cbb_AlwaysSetting.SelectedIndex == 1)
		{
			panel_day.Enabled = false;
			panel_time.Enabled = false;
		}
		else if (cbb_AlwaysSetting.SelectedIndex == 2)
		{
			panel_day.Enabled = false;
			panel_time.Enabled = false;
		}
		else if (cbb_AlwaysSetting.SelectedIndex == 0)
		{
			panel_day.Enabled = true;
			panel_time.Enabled = true;
		}
		else
		{
			panel_day.Enabled = true;
			panel_time.Enabled = false;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.SetUp.frm_NotificationSetting));
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.panelDayOfWeeks = new System.Windows.Forms.Panel();
		this.cb_Sunday = new Apzon.Controls.Editor.CheckEditEx();
		this.cb_Saturday = new Apzon.Controls.Editor.CheckEditEx();
		this.cb_Friday = new Apzon.Controls.Editor.CheckEditEx();
		this.cb_Thursday = new Apzon.Controls.Editor.CheckEditEx();
		this.cb_Wednesday = new Apzon.Controls.Editor.CheckEditEx();
		this.cb_Tuesday = new Apzon.Controls.Editor.CheckEditEx();
		this.cb_Monday = new Apzon.Controls.Editor.CheckEditEx();
		this.rdo_PickDay = new System.Windows.Forms.RadioButton();
		this.rdo_NotReceiveDay = new System.Windows.Forms.RadioButton();
		this.rdo_ReceiveDay = new System.Windows.Forms.RadioButton();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.rdo_NotReceiveTime = new System.Windows.Forms.RadioButton();
		this.rdo_ReceiveTime = new System.Windows.Forms.RadioButton();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine10 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine11 = new Apzon.Controls.Controls.LabelLine();
		this.panel_day = new System.Windows.Forms.Panel();
		this.cbb_DayNotReceiveTo = new DevExpress.XtraEditors.LookUpEdit();
		this.cbb_DayReceiveTo = new DevExpress.XtraEditors.LookUpEdit();
		this.cbb_DayNotReceiveFrom = new DevExpress.XtraEditors.LookUpEdit();
		this.cbb_DayReceiveFrom = new DevExpress.XtraEditors.LookUpEdit();
		this.panel_time = new System.Windows.Forms.Panel();
		this.tex_NotReceiveTo = new Apzon.Controls.Controls.HourEdit();
		this.tex_ReceiveTo = new Apzon.Controls.Controls.HourEdit();
		this.tex_NotReceiveFrom = new Apzon.Controls.Controls.HourEdit();
		this.tex_ReceiveFrom = new Apzon.Controls.Controls.HourEdit();
		this.panel_additional = new System.Windows.Forms.Panel();
		this.cbb_AlwaysSetting = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.panel1 = new System.Windows.Forms.Panel();
		this.rdo_AllNoti = new System.Windows.Forms.RadioButton();
		this.rdo_SpecifyNoti = new System.Windows.Forms.RadioButton();
		this.panel_specifyDay = new System.Windows.Forms.Panel();
		this.cbb_TypeSpecify = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.txt_SpecifyNoti = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine15 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine13 = new Apzon.Controls.Controls.LabelLine();
		this.cb_DANotReceive = new Apzon.Controls.Editor.CheckEditEx();
		this.cb_DAReceive = new Apzon.Controls.Editor.CheckEditEx();
		this.labelLine12 = new Apzon.Controls.Controls.LabelLine();
		this.panelDayOfWeeks.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cb_Sunday.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Saturday.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Friday.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Thursday.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Wednesday.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Tuesday.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Monday.Properties).BeginInit();
		this.panel_day.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cbb_DayNotReceiveTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_DayReceiveTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_DayNotReceiveFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_DayReceiveFrom.Properties).BeginInit();
		this.panel_time.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tex_NotReceiveTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tex_ReceiveTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tex_NotReceiveFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tex_ReceiveFrom.Properties).BeginInit();
		this.panel_additional.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cbb_AlwaysSetting.Properties).BeginInit();
		this.panel1.SuspendLayout();
		this.panel_specifyDay.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cbb_TypeSpecify.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SpecifyNoti.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_DANotReceive.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_DAReceive.Properties).BeginInit();
		base.SuspendLayout();
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(489, 340);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.Size = new System.Drawing.Size(75, 23);
		this.btn_Ok.TabIndex = 2;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.simpleButton1_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(567, 340);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 3;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.simpleButton2_Click);
		this.panelDayOfWeeks.Controls.Add(this.cb_Sunday);
		this.panelDayOfWeeks.Controls.Add(this.cb_Saturday);
		this.panelDayOfWeeks.Controls.Add(this.cb_Friday);
		this.panelDayOfWeeks.Controls.Add(this.cb_Thursday);
		this.panelDayOfWeeks.Controls.Add(this.cb_Wednesday);
		this.panelDayOfWeeks.Controls.Add(this.cb_Tuesday);
		this.panelDayOfWeeks.Controls.Add(this.cb_Monday);
		this.panelDayOfWeeks.Location = new System.Drawing.Point(105, 58);
		this.panelDayOfWeeks.Name = "panelDayOfWeeks";
		this.panelDayOfWeeks.Size = new System.Drawing.Size(515, 36);
		this.panelDayOfWeeks.TabIndex = 17;
		this.cb_Sunday.FieldName = null;
		this.cb_Sunday.IsSearchControl = false;
		this.cb_Sunday.Location = new System.Drawing.Point(450, 8);
		this.cb_Sunday.Name = "cb_Sunday";
		this.cb_Sunday.Properties.Caption = "Sunday";
		this.cb_Sunday.Size = new System.Drawing.Size(62, 20);
		this.cb_Sunday.TabIndex = 27;
		this.cb_Saturday.FieldName = null;
		this.cb_Saturday.IsSearchControl = false;
		this.cb_Saturday.Location = new System.Drawing.Point(378, 8);
		this.cb_Saturday.Name = "cb_Saturday";
		this.cb_Saturday.Properties.Caption = "Saturday";
		this.cb_Saturday.Size = new System.Drawing.Size(66, 20);
		this.cb_Saturday.TabIndex = 28;
		this.cb_Friday.FieldName = null;
		this.cb_Friday.IsSearchControl = false;
		this.cb_Friday.Location = new System.Drawing.Point(315, 8);
		this.cb_Friday.Name = "cb_Friday";
		this.cb_Friday.Properties.Caption = "Friday";
		this.cb_Friday.Size = new System.Drawing.Size(57, 20);
		this.cb_Friday.TabIndex = 29;
		this.cb_Thursday.FieldName = null;
		this.cb_Thursday.IsSearchControl = false;
		this.cb_Thursday.Location = new System.Drawing.Point(234, 8);
		this.cb_Thursday.Name = "cb_Thursday";
		this.cb_Thursday.Properties.Caption = "Thursday";
		this.cb_Thursday.Size = new System.Drawing.Size(75, 20);
		this.cb_Thursday.TabIndex = 30;
		this.cb_Wednesday.FieldName = null;
		this.cb_Wednesday.IsSearchControl = false;
		this.cb_Wednesday.Location = new System.Drawing.Point(148, 8);
		this.cb_Wednesday.Name = "cb_Wednesday";
		this.cb_Wednesday.Properties.Caption = "Wednesday";
		this.cb_Wednesday.Size = new System.Drawing.Size(80, 20);
		this.cb_Wednesday.TabIndex = 31;
		this.cb_Tuesday.FieldName = null;
		this.cb_Tuesday.IsSearchControl = false;
		this.cb_Tuesday.Location = new System.Drawing.Point(76, 8);
		this.cb_Tuesday.Name = "cb_Tuesday";
		this.cb_Tuesday.Properties.Caption = "Tuesday";
		this.cb_Tuesday.Size = new System.Drawing.Size(66, 20);
		this.cb_Tuesday.TabIndex = 25;
		this.cb_Monday.FieldName = null;
		this.cb_Monday.IsSearchControl = false;
		this.cb_Monday.Location = new System.Drawing.Point(7, 8);
		this.cb_Monday.Name = "cb_Monday";
		this.cb_Monday.Properties.Caption = "Monday";
		this.cb_Monday.Size = new System.Drawing.Size(63, 20);
		this.cb_Monday.TabIndex = 26;
		this.rdo_PickDay.AutoSize = true;
		this.rdo_PickDay.Location = new System.Drawing.Point(3, 67);
		this.rdo_PickDay.Name = "rdo_PickDay";
		this.rdo_PickDay.Size = new System.Drawing.Size(96, 17);
		this.rdo_PickDay.TabIndex = 20;
		this.rdo_PickDay.TabStop = true;
		this.rdo_PickDay.Text = "Pick Weekdays";
		this.rdo_PickDay.UseVisualStyleBackColor = true;
		this.rdo_PickDay.CheckedChanged += new System.EventHandler(this.rdo_PickDay_CheckedChanged_1);
		this.rdo_NotReceiveDay.AutoSize = true;
		this.rdo_NotReceiveDay.Location = new System.Drawing.Point(3, 32);
		this.rdo_NotReceiveDay.Name = "rdo_NotReceiveDay";
		this.rdo_NotReceiveDay.Size = new System.Drawing.Size(83, 17);
		this.rdo_NotReceiveDay.TabIndex = 19;
		this.rdo_NotReceiveDay.TabStop = true;
		this.rdo_NotReceiveDay.Text = "Not Receive";
		this.rdo_NotReceiveDay.UseVisualStyleBackColor = true;
		this.rdo_NotReceiveDay.CheckedChanged += new System.EventHandler(this.rdo_NotReceiveDay_CheckedChanged_1);
		this.rdo_ReceiveDay.AutoSize = true;
		this.rdo_ReceiveDay.Location = new System.Drawing.Point(3, 3);
		this.rdo_ReceiveDay.Name = "rdo_ReceiveDay";
		this.rdo_ReceiveDay.Size = new System.Drawing.Size(65, 17);
		this.rdo_ReceiveDay.TabIndex = 18;
		this.rdo_ReceiveDay.TabStop = true;
		this.rdo_ReceiveDay.Text = "Reiceive";
		this.rdo_ReceiveDay.UseVisualStyleBackColor = true;
		this.rdo_ReceiveDay.CheckedChanged += new System.EventHandler(this.rdo_ReceiveDay_CheckedChanged_1);
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(12, 120);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(227, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 29;
		this.labelLine1.Utext = "Set The Day Of Weeks Receive Notification";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(12, 246);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(236, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 30;
		this.labelLine2.Utext = "Set The Time Receive Notification";
		this.rdo_NotReceiveTime.AutoSize = true;
		this.rdo_NotReceiveTime.Location = new System.Drawing.Point(3, 31);
		this.rdo_NotReceiveTime.Name = "rdo_NotReceiveTime";
		this.rdo_NotReceiveTime.Size = new System.Drawing.Size(83, 17);
		this.rdo_NotReceiveTime.TabIndex = 32;
		this.rdo_NotReceiveTime.TabStop = true;
		this.rdo_NotReceiveTime.Text = "Not Receive";
		this.rdo_NotReceiveTime.UseVisualStyleBackColor = true;
		this.rdo_NotReceiveTime.CheckedChanged += new System.EventHandler(this.rdo_NotReceiveTime_CheckedChanged_1);
		this.rdo_ReceiveTime.AutoSize = true;
		this.rdo_ReceiveTime.Location = new System.Drawing.Point(3, 3);
		this.rdo_ReceiveTime.Name = "rdo_ReceiveTime";
		this.rdo_ReceiveTime.Size = new System.Drawing.Size(63, 17);
		this.rdo_ReceiveTime.TabIndex = 31;
		this.rdo_ReceiveTime.TabStop = true;
		this.rdo_ReceiveTime.Text = "Receive";
		this.rdo_ReceiveTime.UseVisualStyleBackColor = true;
		this.rdo_ReceiveTime.CheckedChanged += new System.EventHandler(this.rdo_ReceiveTime_CheckedChanged_1);
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(149, 3);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(66, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 42;
		this.labelLine4.Utext = "From Date";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(149, 32);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(69, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 44;
		this.labelLine6.Utext = "From Date";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(395, 3);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(53, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 45;
		this.labelLine7.Utext = "To Date";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(395, 31);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(53, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 46;
		this.labelLine8.Utext = "To Time";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(149, 31);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(68, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 42;
		this.labelLine5.Utext = "From Time";
		this.labelLine9.FieldName = null;
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(395, 32);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ShowUnderLine = true;
		this.labelLine9.Size = new System.Drawing.Size(53, 20);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 45;
		this.labelLine9.Utext = "To Date";
		this.labelLine10.FieldName = null;
		this.labelLine10.HideWhenEmpty = false;
		this.labelLine10.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine10.Location = new System.Drawing.Point(395, 4);
		this.labelLine10.Name = "labelLine10";
		this.labelLine10.ShowUnderLine = true;
		this.labelLine10.Size = new System.Drawing.Size(53, 20);
		this.labelLine10.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine10.TabIndex = 45;
		this.labelLine10.Utext = "To Time";
		this.labelLine11.FieldName = null;
		this.labelLine11.HideWhenEmpty = false;
		this.labelLine11.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine11.Location = new System.Drawing.Point(149, 3);
		this.labelLine11.Name = "labelLine11";
		this.labelLine11.ShowUnderLine = true;
		this.labelLine11.Size = new System.Drawing.Size(69, 20);
		this.labelLine11.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine11.TabIndex = 44;
		this.labelLine11.Utext = "From Time";
		this.panel_day.Controls.Add(this.cbb_DayNotReceiveTo);
		this.panel_day.Controls.Add(this.cbb_DayReceiveTo);
		this.panel_day.Controls.Add(this.cbb_DayNotReceiveFrom);
		this.panel_day.Controls.Add(this.cbb_DayReceiveFrom);
		this.panel_day.Controls.Add(this.rdo_ReceiveDay);
		this.panel_day.Controls.Add(this.rdo_NotReceiveDay);
		this.panel_day.Controls.Add(this.rdo_PickDay);
		this.panel_day.Controls.Add(this.labelLine9);
		this.panel_day.Controls.Add(this.labelLine7);
		this.panel_day.Controls.Add(this.labelLine6);
		this.panel_day.Controls.Add(this.panelDayOfWeeks);
		this.panel_day.Controls.Add(this.labelLine4);
		this.panel_day.Location = new System.Drawing.Point(12, 144);
		this.panel_day.Name = "panel_day";
		this.panel_day.Size = new System.Drawing.Size(623, 98);
		this.panel_day.TabIndex = 47;
		this.cbb_DayNotReceiveTo.Location = new System.Drawing.Point(454, 29);
		this.cbb_DayNotReceiveTo.Name = "cbb_DayNotReceiveTo";
		this.cbb_DayNotReceiveTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbb_DayNotReceiveTo.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name13"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name14", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.cbb_DayNotReceiveTo.Properties.DisplayMember = "Name";
		this.cbb_DayNotReceiveTo.Properties.NullText = "";
		this.cbb_DayNotReceiveTo.Properties.ValueMember = "Value";
		this.cbb_DayNotReceiveTo.Size = new System.Drawing.Size(121, 20);
		this.cbb_DayNotReceiveTo.TabIndex = 51;
		this.cbb_DayReceiveTo.Location = new System.Drawing.Point(454, 2);
		this.cbb_DayReceiveTo.Name = "cbb_DayReceiveTo";
		this.cbb_DayReceiveTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbb_DayReceiveTo.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.cbb_DayReceiveTo.Properties.DisplayMember = "Name";
		this.cbb_DayReceiveTo.Properties.NullText = "";
		this.cbb_DayReceiveTo.Properties.ShowHeader = false;
		this.cbb_DayReceiveTo.Properties.ValueMember = "Value";
		this.cbb_DayReceiveTo.Size = new System.Drawing.Size(121, 20);
		this.cbb_DayReceiveTo.TabIndex = 52;
		this.cbb_DayNotReceiveFrom.Location = new System.Drawing.Point(221, 31);
		this.cbb_DayNotReceiveFrom.Name = "cbb_DayNotReceiveFrom";
		this.cbb_DayNotReceiveFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbb_DayNotReceiveFrom.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name11"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name12", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.cbb_DayNotReceiveFrom.Properties.DisplayMember = "Name";
		this.cbb_DayNotReceiveFrom.Properties.NullText = "";
		this.cbb_DayNotReceiveFrom.Properties.ValueMember = "Value";
		this.cbb_DayNotReceiveFrom.Size = new System.Drawing.Size(121, 20);
		this.cbb_DayNotReceiveFrom.TabIndex = 53;
		this.cbb_DayReceiveFrom.EditValue = "";
		this.cbb_DayReceiveFrom.Location = new System.Drawing.Point(221, 2);
		this.cbb_DayReceiveFrom.Name = "cbb_DayReceiveFrom";
		this.cbb_DayReceiveFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbb_DayReceiveFrom.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name9"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name10", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.cbb_DayReceiveFrom.Properties.DisplayMember = "Name";
		this.cbb_DayReceiveFrom.Properties.NullText = "";
		this.cbb_DayReceiveFrom.Properties.ValueMember = "Value";
		this.cbb_DayReceiveFrom.Size = new System.Drawing.Size(121, 20);
		this.cbb_DayReceiveFrom.TabIndex = 54;
		this.panel_time.Controls.Add(this.tex_NotReceiveTo);
		this.panel_time.Controls.Add(this.tex_ReceiveTo);
		this.panel_time.Controls.Add(this.tex_NotReceiveFrom);
		this.panel_time.Controls.Add(this.tex_ReceiveFrom);
		this.panel_time.Controls.Add(this.rdo_ReceiveTime);
		this.panel_time.Controls.Add(this.rdo_NotReceiveTime);
		this.panel_time.Controls.Add(this.labelLine8);
		this.panel_time.Controls.Add(this.labelLine10);
		this.panel_time.Controls.Add(this.labelLine11);
		this.panel_time.Controls.Add(this.labelLine5);
		this.panel_time.Location = new System.Drawing.Point(12, 270);
		this.panel_time.Name = "panel_time";
		this.panel_time.Size = new System.Drawing.Size(623, 56);
		this.panel_time.TabIndex = 48;
		this.tex_NotReceiveTo.FieldName = null;
		this.tex_NotReceiveTo.IsSearchControl = false;
		this.tex_NotReceiveTo.Location = new System.Drawing.Point(454, 30);
		this.tex_NotReceiveTo.Name = "tex_NotReceiveTo";
		this.tex_NotReceiveTo.Properties.IsSearchControl = false;
		this.tex_NotReceiveTo.Properties.MaxLength = 5;
		this.tex_NotReceiveTo.Size = new System.Drawing.Size(121, 20);
		this.tex_NotReceiveTo.TabIndex = 55;
		this.tex_ReceiveTo.FieldName = null;
		this.tex_ReceiveTo.IsSearchControl = false;
		this.tex_ReceiveTo.Location = new System.Drawing.Point(454, 2);
		this.tex_ReceiveTo.Name = "tex_ReceiveTo";
		this.tex_ReceiveTo.Properties.IsSearchControl = false;
		this.tex_ReceiveTo.Properties.MaxLength = 5;
		this.tex_ReceiveTo.Size = new System.Drawing.Size(121, 20);
		this.tex_ReceiveTo.TabIndex = 56;
		this.tex_NotReceiveFrom.FieldName = null;
		this.tex_NotReceiveFrom.IsSearchControl = false;
		this.tex_NotReceiveFrom.Location = new System.Drawing.Point(221, 30);
		this.tex_NotReceiveFrom.Name = "tex_NotReceiveFrom";
		this.tex_NotReceiveFrom.Properties.IsSearchControl = false;
		this.tex_NotReceiveFrom.Properties.MaxLength = 5;
		this.tex_NotReceiveFrom.Size = new System.Drawing.Size(121, 20);
		this.tex_NotReceiveFrom.TabIndex = 54;
		this.tex_ReceiveFrom.FieldName = null;
		this.tex_ReceiveFrom.IsSearchControl = false;
		this.tex_ReceiveFrom.Location = new System.Drawing.Point(221, 2);
		this.tex_ReceiveFrom.Name = "tex_ReceiveFrom";
		this.tex_ReceiveFrom.Properties.IsSearchControl = false;
		this.tex_ReceiveFrom.Properties.MaxLength = 5;
		this.tex_ReceiveFrom.Size = new System.Drawing.Size(121, 20);
		this.tex_ReceiveFrom.TabIndex = 57;
		this.panel_additional.Controls.Add(this.cbb_AlwaysSetting);
		this.panel_additional.Controls.Add(this.panel1);
		this.panel_additional.Controls.Add(this.labelLine13);
		this.panel_additional.Location = new System.Drawing.Point(12, 36);
		this.panel_additional.Name = "panel_additional";
		this.panel_additional.Size = new System.Drawing.Size(623, 78);
		this.panel_additional.TabIndex = 49;
		this.cbb_AlwaysSetting.FieldName = null;
		this.cbb_AlwaysSetting.IsSearchControl = false;
		this.cbb_AlwaysSetting.Location = new System.Drawing.Point(149, 52);
		this.cbb_AlwaysSetting.Name = "cbb_AlwaysSetting";
		this.cbb_AlwaysSetting.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbb_AlwaysSetting.Properties.Items.AddRange(new object[4] { "Setting Receive Notification", "Always Receive", "Always Not Receive", "Time Alway Receive" });
		this.cbb_AlwaysSetting.Size = new System.Drawing.Size(165, 20);
		this.cbb_AlwaysSetting.TabIndex = 52;
		this.cbb_AlwaysSetting.SelectedIndexChanged += new System.EventHandler(this.cbb_AlwaysSetting_SelectedIndexChanged);
		this.panel1.Controls.Add(this.rdo_AllNoti);
		this.panel1.Controls.Add(this.rdo_SpecifyNoti);
		this.panel1.Controls.Add(this.panel_specifyDay);
		this.panel1.Location = new System.Drawing.Point(3, 5);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(597, 41);
		this.panel1.TabIndex = 51;
		this.rdo_AllNoti.AutoSize = true;
		this.rdo_AllNoti.Location = new System.Drawing.Point(11, 9);
		this.rdo_AllNoti.Name = "rdo_AllNoti";
		this.rdo_AllNoti.Size = new System.Drawing.Size(93, 17);
		this.rdo_AllNoti.TabIndex = 55;
		this.rdo_AllNoti.TabStop = true;
		this.rdo_AllNoti.Text = "All Notification";
		this.rdo_AllNoti.UseVisualStyleBackColor = true;
		this.rdo_AllNoti.CheckedChanged += new System.EventHandler(this.rdo_AllNoti_CheckedChanged);
		this.rdo_SpecifyNoti.AutoSize = true;
		this.rdo_SpecifyNoti.Location = new System.Drawing.Point(146, 9);
		this.rdo_SpecifyNoti.Name = "rdo_SpecifyNoti";
		this.rdo_SpecifyNoti.Size = new System.Drawing.Size(144, 17);
		this.rdo_SpecifyNoti.TabIndex = 54;
		this.rdo_SpecifyNoti.TabStop = true;
		this.rdo_SpecifyNoti.Text = "Specify Notification From";
		this.rdo_SpecifyNoti.UseVisualStyleBackColor = true;
		this.rdo_SpecifyNoti.CheckedChanged += new System.EventHandler(this.rdo_SpecifyNoti_CheckedChanged);
		this.panel_specifyDay.Controls.Add(this.cbb_TypeSpecify);
		this.panel_specifyDay.Controls.Add(this.txt_SpecifyNoti);
		this.panel_specifyDay.Controls.Add(this.labelLine15);
		this.panel_specifyDay.Location = new System.Drawing.Point(349, 3);
		this.panel_specifyDay.Name = "panel_specifyDay";
		this.panel_specifyDay.Size = new System.Drawing.Size(239, 31);
		this.panel_specifyDay.TabIndex = 53;
		this.cbb_TypeSpecify.FieldName = null;
		this.cbb_TypeSpecify.IsSearchControl = false;
		this.cbb_TypeSpecify.Location = new System.Drawing.Point(124, 5);
		this.cbb_TypeSpecify.Name = "cbb_TypeSpecify";
		this.cbb_TypeSpecify.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbb_TypeSpecify.Properties.Items.AddRange(new object[4] { "Days", "Weeks", "Months", "Years" });
		this.cbb_TypeSpecify.Size = new System.Drawing.Size(66, 20);
		this.cbb_TypeSpecify.TabIndex = 53;
		this.cbb_TypeSpecify.SelectedIndexChanged += new System.EventHandler(this.cbb_TypeSpecify_SelectedIndexChanged);
		this.txt_SpecifyNoti.FieldName = null;
		this.txt_SpecifyNoti.IsSearchControl = false;
		this.txt_SpecifyNoti.Location = new System.Drawing.Point(5, 5);
		this.txt_SpecifyNoti.Name = "txt_SpecifyNoti";
		this.txt_SpecifyNoti.Size = new System.Drawing.Size(102, 20);
		this.txt_SpecifyNoti.TabIndex = 51;
		this.labelLine15.FieldName = null;
		this.labelLine15.HideWhenEmpty = false;
		this.labelLine15.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine15.Location = new System.Drawing.Point(196, 5);
		this.labelLine15.Name = "labelLine15";
		this.labelLine15.ShowUnderLine = true;
		this.labelLine15.Size = new System.Drawing.Size(27, 20);
		this.labelLine15.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine15.TabIndex = 52;
		this.labelLine15.Utext = "ago";
		this.labelLine13.FieldName = null;
		this.labelLine13.HideWhenEmpty = false;
		this.labelLine13.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine13.Location = new System.Drawing.Point(14, 52);
		this.labelLine13.Name = "labelLine13";
		this.labelLine13.ShowUnderLine = true;
		this.labelLine13.Size = new System.Drawing.Size(104, 20);
		this.labelLine13.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine13.TabIndex = 2;
		this.labelLine13.Utext = "Always setting";
		this.cb_DANotReceive.FieldName = null;
		this.cb_DANotReceive.IsSearchControl = false;
		this.cb_DANotReceive.Location = new System.Drawing.Point(0, 0);
		this.cb_DANotReceive.Name = "cb_DANotReceive";
		this.cb_DANotReceive.Size = new System.Drawing.Size(75, 14);
		this.cb_DANotReceive.TabIndex = 0;
		this.cb_DAReceive.FieldName = null;
		this.cb_DAReceive.IsSearchControl = false;
		this.cb_DAReceive.Location = new System.Drawing.Point(0, 0);
		this.cb_DAReceive.Name = "cb_DAReceive";
		this.cb_DAReceive.Size = new System.Drawing.Size(75, 14);
		this.cb_DAReceive.TabIndex = 0;
		this.labelLine12.FieldName = null;
		this.labelLine12.HideWhenEmpty = false;
		this.labelLine12.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine12.Location = new System.Drawing.Point(12, 11);
		this.labelLine12.Name = "labelLine12";
		this.labelLine12.ShowUnderLine = true;
		this.labelLine12.Size = new System.Drawing.Size(227, 20);
		this.labelLine12.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine12.TabIndex = 50;
		this.labelLine12.Utext = "Specify Notification And Always Setting";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(645, 367);
		base.Controls.Add(this.labelLine12);
		base.Controls.Add(this.panel_additional);
		base.Controls.Add(this.panel_time);
		base.Controls.Add(this.panel_day);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.FormId = "oNotificationSetting";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_NotificationSetting";
		this.Text = "Notification Setting";
		this.panelDayOfWeeks.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cb_Sunday.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Saturday.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Friday.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Thursday.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Wednesday.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Tuesday.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Monday.Properties).EndInit();
		this.panel_day.ResumeLayout(false);
		this.panel_day.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.cbb_DayNotReceiveTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_DayReceiveTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_DayNotReceiveFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_DayReceiveFrom.Properties).EndInit();
		this.panel_time.ResumeLayout(false);
		this.panel_time.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.tex_NotReceiveTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tex_ReceiveTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tex_NotReceiveFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tex_ReceiveFrom.Properties).EndInit();
		this.panel_additional.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cbb_AlwaysSetting.Properties).EndInit();
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		this.panel_specifyDay.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cbb_TypeSpecify.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SpecifyNoti.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_DANotReceive.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_DAReceive.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
