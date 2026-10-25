using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.ApzJson;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Global;
using Apzon.HttpClientHelper;
using CronExpressionDescriptor;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using Newtonsoft.Json;

namespace Apzon.Forms;

public class frm_JobsManagement : XtraFormEx
{
	private string _second = "*";

	private string _minute = "*";

	private string _hour = "*";

	private string _dayOfMonth = "?";

	private string _month = "*";

	private string _dayOfWeek = "*";

	private bool _isLoading;

	private bool _preAdd;

	private DataTable _dtRecur = new DataTable();

	private bool _isOkExpression;

	private static readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private Dictionary<Control, string> _dicControl = new Dictionary<Control, string>();

	private IContainer components;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GroupControl groupControl1;

	private SplitContainerControl splitContainerControl1;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private SimpleButton btn_OK;

	private SimpleButton btnCancel;

	private SimpleButton btn_Start;

	private SimpleButton btn_Stop;

	private LabelLine lbl_Message;

	private GridColumnEx gridColumn4;

	private TextEditEx txtResult;

	private GridColumnEx gridColumn5;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private SimpleButton btn_OpenDashBoard;

	private RepositoryItemLookUpEdit rep_Status;

	private GridColumnEx gridColumn6;

	private GridColumnEx gcStoreName;

	private ContextMenuStrip ctx_Header;

	private ToolStripMenuItem menuItem_AddNew;

	private GridColumnEx gc_Active;

	private RepositoryItemCheckEdit rep_Active;

	private XtraTabControl tabCtrl_Recurring;

	private XtraTabPage tab_Second;

	private LabelControl labelControl2;

	private LookUpEdit luk_SecondTo;

	private LookUpEdit luk_SecondFr;

	private RadioButton radSec_between;

	private CheckedComboBoxEdit chk_Second;

	private RadioButton radSec_spec;

	private LookUpEdit luk_Second;

	private LabelControl labelControl1;

	private LookUpEdit luk_noOfSecs;

	private RadioButton radSec_everyStart;

	private RadioButton radSec_every;

	private XtraTabPage tab_Minute;

	private LabelControl labelControl3;

	private LookUpEdit luk_MinTo;

	private LookUpEdit luk_MinFr;

	private RadioButton radMin_between;

	private CheckedComboBoxEdit chk_Min;

	private RadioButton radMin_spec;

	private LookUpEdit luk_Min;

	private LabelControl labelControl4;

	private LookUpEdit luk_NoOfMins;

	private RadioButton radMin_everyStart;

	private RadioButton radMin_every;

	private XtraTabPage tab_Hour;

	private LabelControl labelControl5;

	private LookUpEdit luk_HourTo;

	private LookUpEdit luk_HourFr;

	private RadioButton radHour_between;

	private CheckedComboBoxEdit chk_Hour;

	private RadioButton radHour_spec;

	private LookUpEdit luk_Hour;

	private LabelControl labelControl6;

	private LookUpEdit luk_noOfHours;

	private RadioButton radHour_everyStart;

	private RadioButton radHour_every;

	private XtraTabPage tab_Day;

	private LookUpEdit luk_WeekDayOrdinal;

	private LookUpEdit luk_OrdinalNumber;

	private LabelControl labelControl15;

	private LabelControl labelControl14;

	private LookUpEdit luk_DayInMonthNear;

	private LabelControl labelControl13;

	private LookUpEdit luk_NoOfMdBef;

	private LabelControl labelControl9;

	private LookUpEdit luk_WeekDayLast;

	private CheckedComboBoxEdit chk_DayInMonth;

	private RadioButton radDay_specMth;

	private LabelControl labelControl12;

	private LookUpEdit luk_DayInMonth;

	private LabelControl labelControl11;

	private LookUpEdit luk_NoOfMd;

	private RadioButton radDay_everyStartMd;

	private RadioButton radDay_OrdinalWd;

	private RadioButton radDay_nearestWd;

	private RadioButton radDay_BefEndOfMth;

	private RadioButton radDay_lastWdOfMth;

	private RadioButton radDay_lastWd;

	private RadioButton radDay_lastInMth;

	private CheckedComboBoxEdit chk_WeekDay;

	private RadioButton radDay_specWk;

	private LookUpEdit luk_WeekDay;

	private LabelControl labelControl10;

	private LookUpEdit luk_NoOfWd;

	private RadioButton radDay_everyStartDw;

	private RadioButton radDay_every;

	private XtraTabPage tab_Month;

	private LabelControl labelControl7;

	private LookUpEdit luk_MonthTo;

	private LookUpEdit luk_MonthFr;

	private RadioButton radMonth_between;

	private CheckedComboBoxEdit chk_Month;

	private RadioButton radMonth_spec;

	private LookUpEdit luk_Month;

	private LabelControl labelControl8;

	private LookUpEdit luk_NoOfMonths;

	private RadioButton radMonth_everyStart;

	private RadioButton radMonth_every;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private ToolStripMenuItem deleteToolStripMenuItem;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
			{
				SimpleButton simpleButton4 = btn_OK;
				bool enabled = (btnCancel.Enabled = true);
				simpleButton4.Enabled = enabled;
				btn_OK.Text = Resources.ButtonAdd;
				gv_Doc.Columns["JobCode"].OptionsColumn.ReadOnly = false;
				break;
			}
			case Enums.FormViewModeType.Update:
			{
				SimpleButton simpleButton3 = btn_OK;
				bool enabled = (btnCancel.Enabled = true);
				simpleButton3.Enabled = enabled;
				btn_OK.Text = Resources.ButtonUpdate;
				break;
			}
			case Enums.FormViewModeType.View:
			{
				SimpleButton simpleButton2 = btn_OK;
				bool enabled = (btnCancel.Enabled = true);
				simpleButton2.Enabled = enabled;
				btn_OK.Text = Resources.ButtonOK;
				gv_Doc.Columns["JobCode"].OptionsColumn.ReadOnly = true;
				break;
			}
			case Enums.FormViewModeType.Busy:
			{
				SimpleButton simpleButton = btn_OK;
				bool enabled = (btnCancel.Enabled = false);
				simpleButton.Enabled = enabled;
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
	}

	public frm_JobsManagement()
	{
		InitializeComponent();
		OnLoadAsync += Form1_Load;
	}

	private async Task Form1_Load(object sender, EventArgs e)
	{
		LoadInitForm();
		SetEventControl(tabCtrl_Recurring);
		await LoadData();
	}

	private async Task LoadData()
	{
		try
		{
			tab_Month.Show();
			tab_Day.Show();
			tab_Hour.Show();
			tab_Minute.Show();
			tab_Second.Show();
			DataTable dataTable = await _httpClient.JobsManagementClient.GetAllJobs();
			dataTable.AcceptChanges();
			grd_Doc.DataSource = dataTable;
			grd_Doc.RefreshDataSource();
			if (dataTable.Rows.Count <= 0)
			{
				tabCtrl_Recurring.Enabled = false;
			}
			else
			{
				tabCtrl_Recurring.Enabled = true;
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
	}

	private void SetEventControl(Control ctr)
	{
		foreach (Control control in ctr.Controls)
		{
			if (control is RadioButton)
			{
				((RadioButton)control).CheckedChanged += RadSecondOnCheckedChanged;
			}
			else if (control is LookUpEdit)
			{
				((LookUpEdit)control).EditValueChanged += LookupEditValueChanged;
				_dicControl.Add(control, Function.ToString(((LookUpEdit)control).Properties.Tag));
			}
			else if (control is CheckedComboBoxEdit)
			{
				((CheckedComboBoxEdit)control).EditValueChanged += LookupEditValueChanged;
				_dicControl.Add(control, Function.ToString(((CheckedComboBoxEdit)control).Properties.Tag));
			}
			else
			{
				SetEventControl(control);
			}
		}
	}

	private void LookupEditValueChanged(object sender, EventArgs e)
	{
		Control control = sender as Control;
		XtraTabPage xtraTabPage = (XtraTabPage)(control?.Parent);
		if (xtraTabPage == null)
		{
			return;
		}
		foreach (RadioButton item in xtraTabPage.Controls.OfType<RadioButton>())
		{
			if (item.Tag == control.Tag)
			{
				if (!item.Checked)
				{
					item.Checked = true;
				}
				CalculateComponent(item);
				if (FormMode != Enums.FormViewModeType.Add && !_isLoading)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
				break;
			}
		}
	}

	private void RadSecondOnCheckedChanged(object sender, EventArgs e)
	{
		try
		{
			RadioButton radioButton = (RadioButton)sender;
			if (radioButton != null && radioButton.Checked)
			{
				CalculateComponent(radioButton);
				if (FormMode != Enums.FormViewModeType.Add && !_isLoading)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write(ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CalculateComponent(RadioButton radio)
	{
		if (radio.Name == radSec_every.Name)
		{
			_second = "*";
		}
		if (radio.Name == radSec_everyStart.Name)
		{
			_second = Function.ToString(Function.ParseInt(luk_Second.EditValue)) + "/" + Function.ToString(luk_noOfSecs.EditValue);
		}
		if (radio.Name == radSec_spec.Name)
		{
			string text = Function.ToString(chk_Second.EditValue);
			if (string.IsNullOrWhiteSpace(text))
			{
				text = "0";
			}
			_second = text.Replace(" ", "");
		}
		if (radio.Name == radSec_between.Name)
		{
			_second = $"{luk_SecondFr.EditValue}-{luk_SecondTo.EditValue}";
		}
		if (radio.Name == radMin_every.Name)
		{
			_minute = "*";
		}
		if (radio.Name == radMin_everyStart.Name)
		{
			_minute = Function.ToString(Function.ParseInt(luk_Min.EditValue)) + "/" + Function.ToString(luk_NoOfMins.EditValue);
		}
		if (radio.Name == radMin_spec.Name)
		{
			string text2 = Function.ToString(chk_Min.EditValue);
			if (string.IsNullOrWhiteSpace(text2))
			{
				text2 = "0";
			}
			_minute = text2.Replace(" ", "");
		}
		if (radio.Name == radMin_between.Name)
		{
			_minute = $"{luk_MinFr.EditValue}-{luk_MinTo.EditValue}";
		}
		if (radio.Name == radHour_every.Name)
		{
			_hour = "*";
		}
		if (radio.Name == radHour_everyStart.Name)
		{
			_hour = Function.ToString(Function.ParseInt(luk_Hour.EditValue)) + "/" + Function.ToString(luk_noOfHours.EditValue);
		}
		if (radio.Name == radHour_spec.Name)
		{
			string text3 = Function.ToString(chk_Hour.EditValue);
			if (string.IsNullOrWhiteSpace(text3))
			{
				text3 = "0";
			}
			_hour = text3.Replace(" ", "");
		}
		if (radio.Name == radHour_between.Name)
		{
			_hour = $"{luk_HourFr.EditValue}-{luk_HourTo.EditValue}";
		}
		if (radio.Name == radDay_every.Name)
		{
			_dayOfMonth = "?";
			_dayOfWeek = "*";
		}
		if (radio.Name == radDay_everyStartDw.Name)
		{
			_dayOfMonth = "?";
			_dayOfWeek = Function.ToString(Function.ParseInt(luk_WeekDay.EditValue)) + "/" + Function.ToString(luk_NoOfWd.EditValue);
		}
		if (radio.Name == radDay_everyStartMd.Name)
		{
			_dayOfMonth = Function.ToString(Function.ParseInt(luk_DayInMonth.EditValue)) + "/" + Function.ToString(luk_NoOfMd.EditValue);
			_dayOfWeek = "?";
		}
		if (radio.Name == radDay_specWk.Name)
		{
			string text4 = Function.ToString(chk_WeekDay.EditValue);
			if (string.IsNullOrWhiteSpace(text4))
			{
				text4 = "1";
			}
			_dayOfMonth = "?";
			_dayOfWeek = text4.Replace(" ", "");
		}
		if (radio.Name == radDay_specMth.Name)
		{
			string text5 = Function.ToString(chk_DayInMonth.EditValue);
			if (string.IsNullOrWhiteSpace(text5))
			{
				text5 = "1";
			}
			_dayOfMonth = text5.Replace(" ", "");
			_dayOfWeek = "?";
		}
		if (radio.Name == radDay_lastInMth.Name)
		{
			_dayOfMonth = "L";
			_dayOfWeek = "?";
		}
		if (radio.Name == radDay_lastWd.Name)
		{
			_dayOfMonth = "LW";
			_dayOfWeek = "?";
		}
		if (radio.Name == radDay_lastWdOfMth.Name)
		{
			_dayOfMonth = "?";
			_dayOfWeek = $"{luk_WeekDayLast.EditValue}L";
		}
		if (radio.Name == radDay_BefEndOfMth.Name)
		{
			_dayOfMonth = $"L-{luk_NoOfMdBef.EditValue}";
			_dayOfWeek = "?";
		}
		if (radio.Name == radDay_nearestWd.Name)
		{
			_dayOfMonth = $"L-{luk_DayInMonthNear.EditValue}";
			_dayOfWeek = "?";
		}
		if (radio.Name == radDay_OrdinalWd.Name)
		{
			_dayOfMonth = "?";
			_dayOfWeek = $"{luk_WeekDayOrdinal.EditValue}#{luk_OrdinalNumber}";
		}
		if (radio.Name == radMonth_every.Name)
		{
			_month = "*";
		}
		if (radio.Name == radMonth_everyStart.Name)
		{
			_month = Function.ToString(Function.ParseInt(luk_Month.EditValue)) + "/" + Function.ToString(luk_NoOfMonths.EditValue);
		}
		if (radio.Name == radMonth_spec.Name)
		{
			string text6 = Function.ToString(chk_Month.EditValue);
			if (string.IsNullOrWhiteSpace(text6))
			{
				text6 = "1";
			}
			_month = text6.Replace(" ", "");
		}
		if (radio.Name == radMonth_between.Name)
		{
			_month = $"{luk_MonthFr.EditValue}-{luk_MonthTo.EditValue}";
		}
		RefreshCron();
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (!_preAdd && !_isLoading)
			{
				await GetDetails();
				if (e.RowHandle < 0)
				{
					tabCtrl_Recurring.Enabled = false;
				}
				else
				{
					tabCtrl_Recurring.Enabled = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetDetails()
	{
		try
		{
			_isLoading = true;
			int jobId = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("JobId"));
			Function.ToString(gv_Doc.GetFocusedRowCellValue("IsRecurring"));
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("CronString"));
			Function.ToString(gv_Doc.GetFocusedRowCellValue("Status"));
			try
			{
				ExpressionDescriptor.GetDescription(text);
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", ex.Message);
			}
			string[] array = text.Split(' ');
			if (array.Length == 6)
			{
				_second = array[0];
				_minute = array[1];
				_hour = array[2];
				_dayOfMonth = array[3];
				_month = array[4];
				_dayOfWeek = array[5];
			}
			await LoadRecurInfo(jobId);
			_isOkExpression = true;
			_isLoading = false;
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private async Task LoadRecurInfo(int jobId)
	{
		try
		{
			DataTable dataTable = await _httpClient.JobsManagementClient.GetRecurInfo(jobId);
			if (dataTable.Rows.Count <= 0)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Sec_Type"] = "1";
				dataRow["Sec_Every"] = "1";
				dataRow["Sec_Spec"] = "0";
				dataRow["Sec_StartAt"] = "0";
				dataRow["Sec_From"] = "0";
				dataRow["Sec_To"] = "0";
				dataRow["Min_Type"] = "1";
				dataRow["Min_Every"] = "1";
				dataRow["Min_Spec"] = "0";
				dataRow["Min_StartAt"] = "0";
				dataRow["Min_From"] = "0";
				dataRow["Min_To"] = "0";
				dataRow["Hour_Type"] = "1";
				dataRow["Hour_Every"] = "1";
				dataRow["Hour_Spec"] = "0";
				dataRow["Hour_StartAt"] = "0";
				dataRow["Hour_From"] = "0";
				dataRow["Hour_To"] = "0";
				dataRow["Day_Type"] = "1";
				dataRow["Day_EveryWD"] = "1";
				dataRow["Day_StartAtWD"] = "1";
				dataRow["Day_EveryMD"] = "1";
				dataRow["Day_StartAtMD"] = "1";
				dataRow["Day_SpecWD"] = "1";
				dataRow["Day_SpecMD"] = "1";
				dataRow["Day_LastWDOfMonth"] = "1";
				dataRow["Day_BefMonthEnd"] = "1";
				dataRow["Day_OrdinalWD"] = "1";
				dataRow["Day_WeekDayOrd"] = "1";
				dataRow["Month_Type"] = "1";
				dataRow["Month_Every"] = "1";
				dataRow["Month_Spec"] = "1";
				dataRow["Month_StartAt"] = "1";
				dataRow["Month_From"] = "1";
				dataRow["Month_To"] = "1";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			_dtRecur = dataTable.Copy();
			_dtRecur.AcceptChanges();
			BindControl(_dtRecur);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void BindControl(DataTable dt)
	{
		try
		{
			if (dt.IsNotNull())
			{
				SetBinding(_dicControl, dt);
				CheckRadio(tabCtrl_Recurring, dt);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void CheckRadio(Control ctr, DataTable dt)
	{
		foreach (Control control in ctr.Controls)
		{
			if (control is RadioButton)
			{
				if (Function.ToString(((RadioButton)control).Tag) == Function.ToString(dt.Rows[0][Function.ToString(ctr.Tag)]))
				{
					((RadioButton)control).Checked = true;
				}
				else
				{
					((RadioButton)control).Checked = false;
				}
			}
			else
			{
				CheckRadio(control, dt);
			}
		}
	}

	private void SetBinding(Dictionary<Control, string> dicCtrl, DataTable dt)
	{
		try
		{
			foreach (KeyValuePair<Control, string> item in dicCtrl)
			{
				if (!string.IsNullOrWhiteSpace(item.Value))
				{
					item.Key.DataBindings.Clear();
					item.Key.DataBindings.Add("EditValue", dt, item.Value, formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
					if (item.Key is CheckedComboBoxEdit)
					{
						((CheckedComboBoxEdit)item.Key).RefreshEditValue();
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (!_isLoading && FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
			{
				btn_OK.Enabled = false;
				btnCancel.Enabled = false;
				await SaveData();
				btn_OK.Enabled = true;
				btnCancel.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SaveData(bool isRemove = false)
	{
		_ = 2;
		try
		{
			if (!isRemove && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveJobManagement, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			if (!_isOkExpression)
			{
				WinCommons.Error(Enums.MessageCode.PleaseCheckCronExpressionBeforeSaving, "");
				return;
			}
			int currentJob = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("JobId"));
			DataSet jobDataSet = GetJobDataSet(isRemove);
			_ = grd_Doc.DataSourceTable;
			string text = await _httpClient.JobsManagementClient.SaveData(jobDataSet);
			if (string.IsNullOrWhiteSpace(text))
			{
				await LoadData();
				int num = gv_Doc.LocateByValue("JobId", currentJob);
				if (num != int.MinValue)
				{
					gv_Doc.FocusedRowHandle = num;
					gv_Doc.MakeRowVisible(num);
				}
				await PostToHangFire();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, "Error:" + text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task PostToHangFire()
	{
		_ = 1;
		try
		{
			ServicePointManager.Expect100Continue = true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, (RemoteCertificateValidationCallback)((object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true));
			string requestUri = Function.ToString(ConfigurationManager.AppSettings["HangfireAddress"] + "Home/RestartJobs/");
			string text = Function.ToString(ConfigurationManager.AppSettings["ApiServer"]);
			string databaseName = GlobalData.DatabaseName;
			HttpClient httpClient = new HttpClient();
			httpClient.Timeout = new TimeSpan(0, 30, 0);
			DataTable dataTable = new DataTable("Sender");
			dataTable.Columns.Add("ApiServer");
			dataTable.Columns.Add("DatabaseName");
			dataTable.Rows.Add(text, databaseName);
			StringContent content = new StringContent(JsonConvert.SerializeObject(dataTable), Encoding.UTF8, "application/json");
			HttpResponseMessage response = await httpClient.PostAsync(requestUri, content);
			string text2 = await response.Content.ReadAsStringAsync();
			if (response.IsSuccessStatusCode)
			{
				if (string.IsNullOrEmpty(text2))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					return;
				}
				WinCommons.Error(Enums.MessageCode.StartJobFailedWitId, "", Function.ToString(text2));
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.StartJobFailedWitId, "", Function.ToString(response.ReasonPhrase));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetValsFromRadios(Control ctr)
	{
		foreach (Control control in ctr.Controls)
		{
			if (control is XtraTabPage)
			{
				RadioButton radioButton = control.Controls.OfType<RadioButton>().FirstOrDefault((RadioButton r) => r.Checked);
				if (radioButton != null)
				{
					_dtRecur.Rows[0][Function.ToString(control.Tag)] = Function.ToString(radioButton.Tag);
				}
			}
			else
			{
				GetValsFromRadios(control);
			}
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Start_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.View)
			{
				WinCommons.Error(Enums.MessageCode.PleaseSaveDataFirst, "");
			}
			btn_Start.Enabled = false;
			if (GetJobDataSet().IsNotNull())
			{
				await PostToHangFire();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataSet GetJobDataSet(bool isRemove = false)
	{
		DataTable dataTable = grd_Doc.DataSource as DataTable;
		DataSet dataSet = new DataSet();
		if (dataTable != null && dataTable.Rows.Count > 0)
		{
			DataTable dataTable2 = dataTable.Clone();
			int focusedDataSourceRowIndex = gv_Doc.GetFocusedDataSourceRowIndex();
			dataTable.AcceptChanges();
			dataTable.Rows[focusedDataSourceRowIndex]["IsRemove"] = (isRemove ? 'Y' : 'N');
			dataTable.Rows[focusedDataSourceRowIndex]["Active"] = (isRemove ? ((object)'N') : dataTable.Rows[focusedDataSourceRowIndex]["Active"]);
			dataTable2.ImportRow(dataTable.Rows[focusedDataSourceRowIndex]);
			dataTable2.AcceptChanges();
			GetValsFromRadios(tabCtrl_Recurring);
			dataSet.Tables.Add(dataTable2.Copy());
			_dtRecur.AcceptChanges();
			DataTable dataTable3 = _dtRecur.Copy();
			dataTable3.TableName = "dtRec";
			dataSet.Tables.Add(dataTable3);
		}
		return dataSet;
	}

	private async void btn_Stop_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode != Enums.FormViewModeType.View)
			{
				WinCommons.Error(Enums.MessageCode.PleaseSaveDataFirst, "");
			}
			btn_Stop.Enabled = false;
			DataSet jobDataSet = GetJobDataSet();
			string requestUri = Function.ToString(ConfigurationManager.AppSettings["HangfireAddress"] + "Home/StopJob/");
			HttpResponseMessage httpResponseMessage = await new HttpClient
			{
				Timeout = new TimeSpan(0, 30, 0)
			}.PostAsync(requestUri, jobDataSet, new ApzJsonFormater());
			Task<string> task = httpResponseMessage.Content.ReadAsStringAsync();
			if (httpResponseMessage.IsSuccessStatusCode)
			{
				int jobId = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("JobId"));
				await UpdateStatus(jobId, "S");
				WinCommons.Success(Enums.MessageCode.StopJobSuccess, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.StopJobFailedWithId, "", Function.ToString(task));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task UpdateStatus(int jobId, string status)
	{
		try
		{
			string text = await _httpClient.JobsManagementClient.UpdateStatus(jobId, status);
			if (!string.IsNullOrWhiteSpace(text))
			{
				Logging.Write("ERROR", "Update Job " + jobId + " Status Failed: " + text);
				if (status == "R")
				{
					btn_Start.Enabled = true;
					btn_Stop.Enabled = false;
				}
				else
				{
					btn_Start.Enabled = false;
					btn_Stop.Enabled = true;
				}
				return;
			}
			if (status == "R")
			{
				btn_Start.Enabled = false;
				btn_Stop.Enabled = true;
			}
			else
			{
				btn_Start.Enabled = true;
				btn_Stop.Enabled = false;
			}
			_isLoading = true;
			gv_Doc.SetFocusedRowCellValue("Status", status);
			grd_Doc.RefreshDataSource();
			if (grd_Doc.DataSource is DataTable dataTable)
			{
				dataTable.AcceptChanges();
			}
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
	}

	private void btn_OpenDashBoard_Click(object sender, EventArgs e)
	{
		try
		{
			Process.Start(ConfigurationManager.AppSettings["HangfireAddress"] + "/hangfire");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		if (e.Column.FieldName == "Status")
		{
			if (Function.ToString(e.CellValue) == "R")
			{
				e.Appearance.ForeColor = Color.Green;
			}
			if (Function.ToString(e.CellValue) == "S")
			{
				e.Appearance.ForeColor = Color.Red;
			}
		}
	}

	private void gv_Doc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (_isLoading || _preAdd || FormMode != Enums.FormViewModeType.Update)
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

	private void frm_JobsManagement_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
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

	public async Task REFRESH()
	{
		try
		{
			if ((FormMode != Enums.FormViewModeType.Update && FormMode != Enums.FormViewModeType.Add) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				_preAdd = true;
				FormMode = Enums.FormViewModeType.View;
				DataTable dataTable = grd_Doc.DatasourceTable();
				dataTable.Rows.Clear();
				DataRow dataRow = dataTable.NewRow();
				dataRow["Active"] = "Y";
				dataTable.Rows.Add(dataRow);
				GetDetails();
				FormMode = Enums.FormViewModeType.Add;
				_preAdd = false;
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
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void menuItem_AddNew_Click(object sender, EventArgs e)
	{
		try
		{
			REFRESH();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				SaveData(isRemove: true);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void RefreshCron()
	{
		try
		{
			string text = string.Join(" ", _second, _minute, _hour, _dayOfMonth, _month, _dayOfWeek);
			string text2 = "";
			try
			{
				text2 = ExpressionDescriptor.GetDescription(text);
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
				_isOkExpression = false;
				text2 = "";
			}
			txtResult.EditValue = text + "   Description:  " + text2;
			gv_Doc.SetFocusedRowCellValue("CronString", text);
			if (text2 != "")
			{
				_isOkExpression = true;
			}
		}
		catch (Exception ex2)
		{
			Logging.Write(ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
			_isOkExpression = false;
		}
	}

	private void LoadInitForm()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		for (int i = 1; i <= 60; i++)
		{
			dictionary.Add(i.ToString(), i.ToString());
		}
		Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
		for (int j = 0; j < 60; j++)
		{
			dictionary2.Add(j.ToString(), j.ToString("D2"));
		}
		SetDataSource(luk_noOfSecs, dictionary, hideCodeColumn: true);
		SetDataSource(luk_Second, dictionary2, hideCodeColumn: true);
		SetDataSource(chk_Second, dictionary2, hideCodeColumn: true);
		SetDataSource(luk_SecondFr, dictionary2, hideCodeColumn: true);
		SetDataSource(luk_SecondTo, dictionary2, hideCodeColumn: true);
		SetDataSource(luk_NoOfMins, dictionary, hideCodeColumn: true);
		SetDataSource(luk_Min, dictionary2, hideCodeColumn: true);
		SetDataSource(luk_MinFr, dictionary2, hideCodeColumn: true);
		SetDataSource(luk_MinTo, dictionary2, hideCodeColumn: true);
		SetDataSource(chk_Min, dictionary2, hideCodeColumn: true);
		Dictionary<string, string> dic = (from t in dictionary.AsEnumerable()
			where int.Parse(t.Key) <= 24
			select t).ToDictionary((KeyValuePair<string, string> item) => item.Key, (KeyValuePair<string, string> item) => item.Value);
		Dictionary<string, string> dic2 = (from t in dictionary2.AsEnumerable()
			where int.Parse(t.Key) < 24
			select t).ToDictionary((KeyValuePair<string, string> item) => item.Key, (KeyValuePair<string, string> item) => item.Value);
		SetDataSource(luk_noOfHours, dic, hideCodeColumn: true);
		SetDataSource(chk_Hour, dic2, hideCodeColumn: true);
		SetDataSource(luk_Hour, dic2, hideCodeColumn: true);
		SetDataSource(luk_HourFr, dic2, hideCodeColumn: true);
		SetDataSource(luk_HourTo, dic2, hideCodeColumn: true);
		Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
		Dictionary<string, string> dictionary4 = new Dictionary<string, string>();
		Dictionary<string, string> dic3 = (from t in dictionary.AsEnumerable()
			where int.Parse(t.Key) <= 31
			select t).ToDictionary((KeyValuePair<string, string> item) => item.Key, (KeyValuePair<string, string> item) => item.Value);
		foreach (int value in Enum.GetValues(typeof(DayOfWeek)))
		{
			string key = (value + 1).ToString();
			DayOfWeek dayOfWeek = (DayOfWeek)value;
			dictionary4.Add(key, dayOfWeek.ToString());
			dictionary3.Add((value + 1).ToString(), (value + 1).ToString());
		}
		SetDataSource(luk_NoOfWd, dictionary3, hideCodeColumn: true);
		SetDataSource(luk_WeekDay, dictionary4, hideCodeColumn: true);
		SetDataSource(chk_WeekDay, dictionary4, hideCodeColumn: true);
		SetDataSource(luk_WeekDayLast, dictionary4, hideCodeColumn: true);
		SetDataSource(luk_WeekDayOrdinal, dictionary4, hideCodeColumn: true);
		SetDataSource(luk_NoOfMd, dic3, hideCodeColumn: true);
		SetDataSource(luk_NoOfMdBef, dic3, hideCodeColumn: true);
		Dictionary<string, string> dictionary5 = new Dictionary<string, string>();
		for (int num2 = 1; num2 <= 31; num2++)
		{
			dictionary5.Add(num2.ToString(), AddOrdinal(num2));
		}
		SetDataSource(luk_DayInMonth, dictionary5, hideCodeColumn: true);
		SetDataSource(luk_DayInMonthNear, dictionary5, hideCodeColumn: true);
		SetDataSource(chk_DayInMonth, dictionary5, hideCodeColumn: true);
		Dictionary<string, string> dic4 = (from t in dictionary5.AsEnumerable()
			where int.Parse(t.Key) <= 5
			select t).ToDictionary((KeyValuePair<string, string> item) => item.Key, (KeyValuePair<string, string> item) => item.Value);
		SetDataSource(luk_OrdinalNumber, dic4, hideCodeColumn: true);
		Dictionary<string, string> dictionary6 = new Dictionary<string, string>();
		Dictionary<string, string> dictionary7 = new Dictionary<string, string>();
		for (int num3 = 1; num3 <= 12; num3++)
		{
			dictionary7.Add(num3.ToString(), DateTimeFormatInfo.CurrentInfo?.GetMonthName(num3));
			dictionary6.Add(num3.ToString(), num3.ToString());
		}
		SetDataSource(luk_NoOfMonths, dictionary6, hideCodeColumn: true);
		SetDataSource(luk_Month, dictionary7, hideCodeColumn: true);
		SetDataSource(luk_MonthFr, dictionary7, hideCodeColumn: true);
		SetDataSource(luk_MonthTo, dictionary7, hideCodeColumn: true);
		SetDataSource(chk_Month, dictionary7, hideCodeColumn: true);
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("Code", typeof(string));
		dataTable.Columns.Add("Name", typeof(string));
		dataTable.Rows.Add("R", "Running");
		dataTable.Rows.Add("S", "Stopped");
		rep_Status.DataSource = dataTable;
		rep_Status.DisplayMember = "Name";
		rep_Status.ValueMember = "Code";
		rep_Status.BestFitMode = BestFitMode.BestFitResizePopup;
		rep_Status.ShowHeader = false;
		rep_Status.ForceInitialize();
		rep_Status.PopulateColumns();
	}

	public static string AddOrdinal(int num)
	{
		if (num <= 0)
		{
			return num.ToString();
		}
		int num2 = num % 100;
		if ((uint)(num2 - 11) <= 2u)
		{
			return num + "th";
		}
		return (num % 10) switch
		{
			1 => num + "st", 
			2 => num + "nd", 
			3 => num + "rd", 
			_ => num + "th", 
		};
	}

	public void SetDataSource(LookUpEdit look, Dictionary<string, string> dic, bool hideCodeColumn = false)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			foreach (KeyValuePair<string, string> item in dic)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = item.Key;
				dataRow["Name"] = item.Value;
				dataTable.Rows.Add(dataRow);
			}
			look.Properties.DataSource = dataTable;
			look.Properties.DisplayMember = "Name";
			look.Properties.ValueMember = "Code";
			look.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			look.Properties.ShowHeader = false;
			look.Properties.ForceInitialize();
			look.Properties.PopulateColumns();
			if (hideCodeColumn && look.Properties.Columns.Count > 0)
			{
				look.Properties.Columns[0].Visible = false;
			}
			look.EditValue = dataTable.Rows[0]["Code"];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
		}
	}

	public void SetDataSource(CheckedComboBoxEdit look, Dictionary<string, string> dic, bool hideCodeColumn = false)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			foreach (KeyValuePair<string, string> item in dic)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = item.Key;
				dataRow["Name"] = item.Value;
				dataTable.Rows.Add(dataRow);
			}
			look.Properties.DataSource = dataTable;
			look.Properties.DisplayMember = "Name";
			look.Properties.ValueMember = "Code";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.frm_JobsManagement));
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctx_Header = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menuItem_AddNew = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gcStoreName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Active = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Active = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.tabCtrl_Recurring = new DevExpress.XtraTab.XtraTabControl();
		this.tab_Second = new DevExpress.XtraTab.XtraTabPage();
		this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
		this.luk_SecondTo = new DevExpress.XtraEditors.LookUpEdit();
		this.luk_SecondFr = new DevExpress.XtraEditors.LookUpEdit();
		this.radSec_between = new System.Windows.Forms.RadioButton();
		this.chk_Second = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.radSec_spec = new System.Windows.Forms.RadioButton();
		this.luk_Second = new DevExpress.XtraEditors.LookUpEdit();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.luk_noOfSecs = new DevExpress.XtraEditors.LookUpEdit();
		this.radSec_everyStart = new System.Windows.Forms.RadioButton();
		this.radSec_every = new System.Windows.Forms.RadioButton();
		this.tab_Minute = new DevExpress.XtraTab.XtraTabPage();
		this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
		this.luk_MinTo = new DevExpress.XtraEditors.LookUpEdit();
		this.luk_MinFr = new DevExpress.XtraEditors.LookUpEdit();
		this.radMin_between = new System.Windows.Forms.RadioButton();
		this.chk_Min = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.radMin_spec = new System.Windows.Forms.RadioButton();
		this.luk_Min = new DevExpress.XtraEditors.LookUpEdit();
		this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
		this.luk_NoOfMins = new DevExpress.XtraEditors.LookUpEdit();
		this.radMin_everyStart = new System.Windows.Forms.RadioButton();
		this.radMin_every = new System.Windows.Forms.RadioButton();
		this.tab_Hour = new DevExpress.XtraTab.XtraTabPage();
		this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
		this.luk_HourTo = new DevExpress.XtraEditors.LookUpEdit();
		this.luk_HourFr = new DevExpress.XtraEditors.LookUpEdit();
		this.radHour_between = new System.Windows.Forms.RadioButton();
		this.chk_Hour = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.radHour_spec = new System.Windows.Forms.RadioButton();
		this.luk_Hour = new DevExpress.XtraEditors.LookUpEdit();
		this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
		this.luk_noOfHours = new DevExpress.XtraEditors.LookUpEdit();
		this.radHour_everyStart = new System.Windows.Forms.RadioButton();
		this.radHour_every = new System.Windows.Forms.RadioButton();
		this.tab_Day = new DevExpress.XtraTab.XtraTabPage();
		this.luk_WeekDayOrdinal = new DevExpress.XtraEditors.LookUpEdit();
		this.luk_OrdinalNumber = new DevExpress.XtraEditors.LookUpEdit();
		this.labelControl15 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl14 = new DevExpress.XtraEditors.LabelControl();
		this.luk_DayInMonthNear = new DevExpress.XtraEditors.LookUpEdit();
		this.labelControl13 = new DevExpress.XtraEditors.LabelControl();
		this.luk_NoOfMdBef = new DevExpress.XtraEditors.LookUpEdit();
		this.labelControl9 = new DevExpress.XtraEditors.LabelControl();
		this.luk_WeekDayLast = new DevExpress.XtraEditors.LookUpEdit();
		this.chk_DayInMonth = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.radDay_specMth = new System.Windows.Forms.RadioButton();
		this.labelControl12 = new DevExpress.XtraEditors.LabelControl();
		this.luk_DayInMonth = new DevExpress.XtraEditors.LookUpEdit();
		this.labelControl11 = new DevExpress.XtraEditors.LabelControl();
		this.luk_NoOfMd = new DevExpress.XtraEditors.LookUpEdit();
		this.radDay_everyStartMd = new System.Windows.Forms.RadioButton();
		this.radDay_OrdinalWd = new System.Windows.Forms.RadioButton();
		this.radDay_nearestWd = new System.Windows.Forms.RadioButton();
		this.radDay_BefEndOfMth = new System.Windows.Forms.RadioButton();
		this.radDay_lastWdOfMth = new System.Windows.Forms.RadioButton();
		this.radDay_lastWd = new System.Windows.Forms.RadioButton();
		this.radDay_lastInMth = new System.Windows.Forms.RadioButton();
		this.chk_WeekDay = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.radDay_specWk = new System.Windows.Forms.RadioButton();
		this.luk_WeekDay = new DevExpress.XtraEditors.LookUpEdit();
		this.labelControl10 = new DevExpress.XtraEditors.LabelControl();
		this.luk_NoOfWd = new DevExpress.XtraEditors.LookUpEdit();
		this.radDay_everyStartDw = new System.Windows.Forms.RadioButton();
		this.radDay_every = new System.Windows.Forms.RadioButton();
		this.tab_Month = new DevExpress.XtraTab.XtraTabPage();
		this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
		this.luk_MonthTo = new DevExpress.XtraEditors.LookUpEdit();
		this.luk_MonthFr = new DevExpress.XtraEditors.LookUpEdit();
		this.radMonth_between = new System.Windows.Forms.RadioButton();
		this.chk_Month = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.radMonth_spec = new System.Windows.Forms.RadioButton();
		this.luk_Month = new DevExpress.XtraEditors.LookUpEdit();
		this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
		this.luk_NoOfMonths = new DevExpress.XtraEditors.LookUpEdit();
		this.radMonth_everyStart = new System.Windows.Forms.RadioButton();
		this.radMonth_every = new System.Windows.Forms.RadioButton();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.btn_OK = new DevExpress.XtraEditors.SimpleButton();
		this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Start = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Stop = new DevExpress.XtraEditors.SimpleButton();
		this.lbl_Message = new Apzon.Controls.Controls.LabelLine();
		this.txtResult = new Apzon.Controls.Editor.TextEditEx();
		this.btn_OpenDashBoard = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctx_Header.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Active).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tabCtrl_Recurring).BeginInit();
		this.tabCtrl_Recurring.SuspendLayout();
		this.tab_Second.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_SecondTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_SecondFr.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Second.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Second.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_noOfSecs.Properties).BeginInit();
		this.tab_Minute.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_MinTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_MinFr.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Min.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Min.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_NoOfMins.Properties).BeginInit();
		this.tab_Hour.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_HourTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_HourFr.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Hour.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Hour.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_noOfHours.Properties).BeginInit();
		this.tab_Day.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_WeekDayOrdinal.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_OrdinalNumber.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_DayInMonthNear.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_NoOfMdBef.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_WeekDayLast.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_DayInMonth.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_DayInMonth.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_NoOfMd.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_WeekDay.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_WeekDay.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_NoOfWd.Properties).BeginInit();
		this.tab_Month.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_MonthTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_MonthFr.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Month.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Month.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_NoOfMonths.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtResult.Properties).BeginInit();
		base.SuspendLayout();
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctx_Header;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.repositoryItemCheckEdit1, this.rep_Status, this.rep_Active, this.repositoryItemMemoExEdit1 });
		this.grd_Doc.Size = new System.Drawing.Size(924, 179);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctx_Header.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.menuItem_AddNew, this.deleteToolStripMenuItem });
		this.ctx_Header.Name = "ctx_Header";
		this.ctx_Header.Size = new System.Drawing.Size(124, 48);
		this.menuItem_AddNew.Name = "menuItem_AddNew";
		this.menuItem_AddNew.Size = new System.Drawing.Size(123, 22);
		this.menuItem_AddNew.Text = "Add New";
		this.menuItem_AddNew.Click += new System.EventHandler(this.menuItem_AddNew_Click);
		this.deleteToolStripMenuItem.ForeColor = System.Drawing.Color.Red;
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gridColumn1, this.gridColumn6, this.gridColumn2, this.gcStoreName, this.gc_Active, this.gridColumn3, this.gridColumn4, this.gridColumn5 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oJobManagement";
		this.gv_Doc.GridControl = this.grd_Doc;
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
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Doc_ShowingEditor);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.gridColumn1.Caption = "ID";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "JobId";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn6.Caption = "Ma\u0303 Job";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "JobCode";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 1;
		this.gridColumn2.Caption = "Mô ta\u0309";
		this.gridColumn2.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "Dscription";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 2;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.gcStoreName.Caption = "Tên store";
		this.gcStoreName.CurrencyDateField = null;
		this.gcStoreName.CurrencyField = null;
		this.gcStoreName.CurrencyFixed = null;
		this.gcStoreName.CurrencyFixedRate = 0.0;
		this.gcStoreName.CurrencyRateField = null;
		this.gcStoreName.FieldName = "StoreName";
		this.gcStoreName.Name = "gcStoreName";
		this.gcStoreName.PrimaryKey = false;
		this.gcStoreName.UnitField = null;
		this.gcStoreName.Visible = true;
		this.gcStoreName.VisibleIndex = 3;
		this.gc_Active.Caption = "Hoa\u0323t đô\u0323ng";
		this.gc_Active.ColumnEdit = this.rep_Active;
		this.gc_Active.CurrencyDateField = null;
		this.gc_Active.CurrencyField = null;
		this.gc_Active.CurrencyFixed = null;
		this.gc_Active.CurrencyFixedRate = 0.0;
		this.gc_Active.CurrencyRateField = null;
		this.gc_Active.FieldName = "Active";
		this.gc_Active.Name = "gc_Active";
		this.gc_Active.PrimaryKey = false;
		this.gc_Active.UnitField = null;
		this.gc_Active.Visible = true;
		this.gc_Active.VisibleIndex = 4;
		this.rep_Active.AutoHeight = false;
		this.rep_Active.Name = "rep_Active";
		this.rep_Active.ValueChecked = "Y";
		this.rep_Active.ValueUnchecked = "N";
		this.gridColumn3.Caption = "Status";
		this.gridColumn3.ColumnEdit = this.rep_Status;
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "Status";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Width = 167;
		this.rep_Status.AutoHeight = false;
		this.rep_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Status.Name = "rep_Status";
		this.rep_Status.NullText = "";
		this.gridColumn4.Caption = "Is recurring";
		this.gridColumn4.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "IsRecurring";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Width = 167;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.gridColumn5.Caption = "Chuô\u0303i Cron Expression";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "CronString";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.OptionsColumn.ReadOnly = true;
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 5;
		this.groupControl1.Controls.Add(this.tabCtrl_Recurring);
		this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl1.Location = new System.Drawing.Point(0, 0);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(924, 312);
		this.groupControl1.TabIndex = 2;
		this.groupControl1.Text = "Recurring Info";
		this.tabCtrl_Recurring.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tabCtrl_Recurring.Location = new System.Drawing.Point(2, 23);
		this.tabCtrl_Recurring.Name = "tabCtrl_Recurring";
		this.tabCtrl_Recurring.SelectedTabPage = this.tab_Second;
		this.tabCtrl_Recurring.Size = new System.Drawing.Size(920, 287);
		this.tabCtrl_Recurring.TabIndex = 2;
		this.tabCtrl_Recurring.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[5] { this.tab_Second, this.tab_Minute, this.tab_Hour, this.tab_Day, this.tab_Month });
		this.tabCtrl_Recurring.Tag = "";
		this.tab_Second.Controls.Add(this.labelControl2);
		this.tab_Second.Controls.Add(this.luk_SecondTo);
		this.tab_Second.Controls.Add(this.luk_SecondFr);
		this.tab_Second.Controls.Add(this.radSec_between);
		this.tab_Second.Controls.Add(this.chk_Second);
		this.tab_Second.Controls.Add(this.radSec_spec);
		this.tab_Second.Controls.Add(this.luk_Second);
		this.tab_Second.Controls.Add(this.labelControl1);
		this.tab_Second.Controls.Add(this.luk_noOfSecs);
		this.tab_Second.Controls.Add(this.radSec_everyStart);
		this.tab_Second.Controls.Add(this.radSec_every);
		this.tab_Second.Name = "tab_Second";
		this.tab_Second.Size = new System.Drawing.Size(918, 262);
		this.tab_Second.Tag = "Sec_Type";
		this.tab_Second.Text = "Seconds";
		this.labelControl2.Location = new System.Drawing.Point(303, 85);
		this.labelControl2.Name = "labelControl2";
		this.labelControl2.Size = new System.Drawing.Size(55, 13);
		this.labelControl2.TabIndex = 11;
		this.labelControl2.Text = "and second";
		this.luk_SecondTo.Location = new System.Drawing.Point(364, 82);
		this.luk_SecondTo.Name = "luk_SecondTo";
		this.luk_SecondTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_SecondTo.Properties.NullText = "";
		this.luk_SecondTo.Properties.Tag = "Sec_To";
		this.luk_SecondTo.Size = new System.Drawing.Size(120, 20);
		this.luk_SecondTo.TabIndex = 9;
		this.luk_SecondTo.Tag = "4";
		this.luk_SecondFr.Location = new System.Drawing.Point(175, 82);
		this.luk_SecondFr.Name = "luk_SecondFr";
		this.luk_SecondFr.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
		this.luk_SecondFr.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_SecondFr.Properties.NullText = "";
		this.luk_SecondFr.Properties.Tag = "Sec_From";
		this.luk_SecondFr.Size = new System.Drawing.Size(120, 20);
		this.luk_SecondFr.TabIndex = 8;
		this.luk_SecondFr.Tag = "4";
		this.radSec_between.AutoSize = true;
		this.radSec_between.Location = new System.Drawing.Point(3, 83);
		this.radSec_between.Name = "radSec_between";
		this.radSec_between.Size = new System.Drawing.Size(172, 17);
		this.radSec_between.TabIndex = 7;
		this.radSec_between.Tag = "4";
		this.radSec_between.Text = "Every second between second";
		this.radSec_between.UseVisualStyleBackColor = true;
		this.chk_Second.Location = new System.Drawing.Point(213, 59);
		this.chk_Second.Name = "chk_Second";
		this.chk_Second.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.chk_Second.Properties.Tag = "Sec_Spec";
		this.chk_Second.Size = new System.Drawing.Size(120, 20);
		this.chk_Second.TabIndex = 6;
		this.chk_Second.Tag = "3";
		this.radSec_spec.AutoSize = true;
		this.radSec_spec.Location = new System.Drawing.Point(3, 60);
		this.radSec_spec.Name = "radSec_spec";
		this.radSec_spec.Size = new System.Drawing.Size(206, 17);
		this.radSec_spec.TabIndex = 5;
		this.radSec_spec.Tag = "3";
		this.radSec_spec.Text = "Specific second (choose one or many)";
		this.radSec_spec.UseVisualStyleBackColor = true;
		this.luk_Second.Location = new System.Drawing.Point(273, 36);
		this.luk_Second.Name = "luk_Second";
		this.luk_Second.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Second.Properties.NullText = "";
		this.luk_Second.Properties.Tag = "Sec_StartAt";
		this.luk_Second.Size = new System.Drawing.Size(120, 20);
		this.luk_Second.TabIndex = 4;
		this.luk_Second.Tag = "2";
		this.labelControl1.Location = new System.Drawing.Point(130, 39);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(137, 13);
		this.labelControl1.TabIndex = 3;
		this.labelControl1.Text = "second(s) starting at second";
		this.luk_noOfSecs.Location = new System.Drawing.Point(62, 36);
		this.luk_noOfSecs.Name = "luk_noOfSecs";
		this.luk_noOfSecs.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_noOfSecs.Properties.NullText = "";
		this.luk_noOfSecs.Properties.Tag = "Sec_Every";
		this.luk_noOfSecs.Size = new System.Drawing.Size(60, 20);
		this.luk_noOfSecs.TabIndex = 3;
		this.luk_noOfSecs.Tag = "2";
		this.radSec_everyStart.AutoSize = true;
		this.radSec_everyStart.Location = new System.Drawing.Point(3, 37);
		this.radSec_everyStart.Name = "radSec_everyStart";
		this.radSec_everyStart.Size = new System.Drawing.Size(53, 17);
		this.radSec_everyStart.TabIndex = 2;
		this.radSec_everyStart.Tag = "2";
		this.radSec_everyStart.Text = "Every";
		this.radSec_everyStart.UseVisualStyleBackColor = true;
		this.radSec_every.AutoSize = true;
		this.radSec_every.Location = new System.Drawing.Point(3, 14);
		this.radSec_every.Name = "radSec_every";
		this.radSec_every.Size = new System.Drawing.Size(91, 17);
		this.radSec_every.TabIndex = 1;
		this.radSec_every.TabStop = true;
		this.radSec_every.Tag = "1";
		this.radSec_every.Text = "Every Second";
		this.radSec_every.UseVisualStyleBackColor = true;
		this.tab_Minute.Controls.Add(this.labelControl3);
		this.tab_Minute.Controls.Add(this.luk_MinTo);
		this.tab_Minute.Controls.Add(this.luk_MinFr);
		this.tab_Minute.Controls.Add(this.radMin_between);
		this.tab_Minute.Controls.Add(this.chk_Min);
		this.tab_Minute.Controls.Add(this.radMin_spec);
		this.tab_Minute.Controls.Add(this.luk_Min);
		this.tab_Minute.Controls.Add(this.labelControl4);
		this.tab_Minute.Controls.Add(this.luk_NoOfMins);
		this.tab_Minute.Controls.Add(this.radMin_everyStart);
		this.tab_Minute.Controls.Add(this.radMin_every);
		this.tab_Minute.Name = "tab_Minute";
		this.tab_Minute.Size = new System.Drawing.Size(918, 262);
		this.tab_Minute.Tag = "Min_Type";
		this.tab_Minute.Text = "Minutes";
		this.labelControl3.Location = new System.Drawing.Point(237, 84);
		this.labelControl3.Name = "labelControl3";
		this.labelControl3.Size = new System.Drawing.Size(53, 13);
		this.labelControl3.TabIndex = 22;
		this.labelControl3.Text = "and minute";
		this.luk_MinTo.Location = new System.Drawing.Point(298, 81);
		this.luk_MinTo.Name = "luk_MinTo";
		this.luk_MinTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_MinTo.Properties.NullText = "";
		this.luk_MinTo.Properties.Tag = "Min_To";
		this.luk_MinTo.Size = new System.Drawing.Size(60, 20);
		this.luk_MinTo.TabIndex = 9;
		this.luk_MinTo.Tag = "4";
		this.luk_MinFr.Location = new System.Drawing.Point(171, 81);
		this.luk_MinFr.Name = "luk_MinFr";
		this.luk_MinFr.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_MinFr.Properties.NullText = "";
		this.luk_MinFr.Properties.Tag = "Min_From";
		this.luk_MinFr.Size = new System.Drawing.Size(60, 20);
		this.luk_MinFr.TabIndex = 8;
		this.luk_MinFr.Tag = "4";
		this.radMin_between.AutoSize = true;
		this.radMin_between.Location = new System.Drawing.Point(3, 82);
		this.radMin_between.Name = "radMin_between";
		this.radMin_between.Size = new System.Drawing.Size(170, 17);
		this.radMin_between.TabIndex = 7;
		this.radMin_between.TabStop = true;
		this.radMin_between.Tag = "4";
		this.radMin_between.Text = "Every minute between second";
		this.radMin_between.UseVisualStyleBackColor = true;
		this.chk_Min.Location = new System.Drawing.Point(213, 58);
		this.chk_Min.Name = "chk_Min";
		this.chk_Min.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.chk_Min.Properties.Tag = "Min_Spec";
		this.chk_Min.Size = new System.Drawing.Size(120, 20);
		this.chk_Min.TabIndex = 6;
		this.chk_Min.Tag = "3";
		this.radMin_spec.AutoSize = true;
		this.radMin_spec.Location = new System.Drawing.Point(3, 59);
		this.radMin_spec.Name = "radMin_spec";
		this.radMin_spec.Size = new System.Drawing.Size(204, 17);
		this.radMin_spec.TabIndex = 5;
		this.radMin_spec.TabStop = true;
		this.radMin_spec.Tag = "3";
		this.radMin_spec.Text = "Specific minute (choose one or many)";
		this.radMin_spec.UseVisualStyleBackColor = true;
		this.luk_Min.Location = new System.Drawing.Point(273, 35);
		this.luk_Min.Name = "luk_Min";
		this.luk_Min.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Min.Properties.NullText = "";
		this.luk_Min.Properties.Tag = "Min_StartAt";
		this.luk_Min.Size = new System.Drawing.Size(60, 20);
		this.luk_Min.TabIndex = 4;
		this.luk_Min.Tag = "2";
		this.labelControl4.Location = new System.Drawing.Point(130, 38);
		this.labelControl4.Name = "labelControl4";
		this.labelControl4.Size = new System.Drawing.Size(133, 13);
		this.labelControl4.TabIndex = 15;
		this.labelControl4.Text = "minute(s) starting at minute";
		this.luk_NoOfMins.Location = new System.Drawing.Point(62, 35);
		this.luk_NoOfMins.Name = "luk_NoOfMins";
		this.luk_NoOfMins.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_NoOfMins.Properties.NullText = "";
		this.luk_NoOfMins.Properties.Tag = "Min_Every";
		this.luk_NoOfMins.Size = new System.Drawing.Size(60, 20);
		this.luk_NoOfMins.TabIndex = 3;
		this.luk_NoOfMins.Tag = "2";
		this.radMin_everyStart.AutoSize = true;
		this.radMin_everyStart.Location = new System.Drawing.Point(3, 36);
		this.radMin_everyStart.Name = "radMin_everyStart";
		this.radMin_everyStart.Size = new System.Drawing.Size(53, 17);
		this.radMin_everyStart.TabIndex = 2;
		this.radMin_everyStart.TabStop = true;
		this.radMin_everyStart.Tag = "2";
		this.radMin_everyStart.Text = "Every";
		this.radMin_everyStart.UseVisualStyleBackColor = true;
		this.radMin_every.AutoSize = true;
		this.radMin_every.Location = new System.Drawing.Point(3, 13);
		this.radMin_every.Name = "radMin_every";
		this.radMin_every.Size = new System.Drawing.Size(88, 17);
		this.radMin_every.TabIndex = 1;
		this.radMin_every.TabStop = true;
		this.radMin_every.Tag = "1";
		this.radMin_every.Text = "Every minute";
		this.radMin_every.UseVisualStyleBackColor = true;
		this.tab_Hour.Controls.Add(this.labelControl5);
		this.tab_Hour.Controls.Add(this.luk_HourTo);
		this.tab_Hour.Controls.Add(this.luk_HourFr);
		this.tab_Hour.Controls.Add(this.radHour_between);
		this.tab_Hour.Controls.Add(this.chk_Hour);
		this.tab_Hour.Controls.Add(this.radHour_spec);
		this.tab_Hour.Controls.Add(this.luk_Hour);
		this.tab_Hour.Controls.Add(this.labelControl6);
		this.tab_Hour.Controls.Add(this.luk_noOfHours);
		this.tab_Hour.Controls.Add(this.radHour_everyStart);
		this.tab_Hour.Controls.Add(this.radHour_every);
		this.tab_Hour.Name = "tab_Hour";
		this.tab_Hour.Size = new System.Drawing.Size(918, 262);
		this.tab_Hour.Tag = "Hour_Type";
		this.tab_Hour.Text = "Hours";
		this.labelControl5.Location = new System.Drawing.Point(214, 84);
		this.labelControl5.Name = "labelControl5";
		this.labelControl5.Size = new System.Drawing.Size(43, 13);
		this.labelControl5.TabIndex = 22;
		this.labelControl5.Text = "and hour";
		this.luk_HourTo.Location = new System.Drawing.Point(263, 81);
		this.luk_HourTo.Name = "luk_HourTo";
		this.luk_HourTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_HourTo.Properties.NullText = "";
		this.luk_HourTo.Properties.Tag = "Hour_To";
		this.luk_HourTo.Size = new System.Drawing.Size(60, 20);
		this.luk_HourTo.TabIndex = 9;
		this.luk_HourTo.Tag = "4";
		this.luk_HourFr.Location = new System.Drawing.Point(148, 81);
		this.luk_HourFr.Name = "luk_HourFr";
		this.luk_HourFr.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_HourFr.Properties.NullText = "";
		this.luk_HourFr.Properties.Tag = "Hour_From";
		this.luk_HourFr.Size = new System.Drawing.Size(60, 20);
		this.luk_HourFr.TabIndex = 8;
		this.luk_HourFr.Tag = "4";
		this.radHour_between.AutoSize = true;
		this.radHour_between.Location = new System.Drawing.Point(3, 82);
		this.radHour_between.Name = "radHour_between";
		this.radHour_between.Size = new System.Drawing.Size(148, 17);
		this.radHour_between.TabIndex = 7;
		this.radHour_between.TabStop = true;
		this.radHour_between.Tag = "4";
		this.radHour_between.Text = "Every hour between hour";
		this.radHour_between.UseVisualStyleBackColor = true;
		this.chk_Hour.Location = new System.Drawing.Point(203, 58);
		this.chk_Hour.Name = "chk_Hour";
		this.chk_Hour.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.chk_Hour.Properties.Tag = "Hour_Spec";
		this.chk_Hour.Size = new System.Drawing.Size(120, 20);
		this.chk_Hour.TabIndex = 6;
		this.chk_Hour.Tag = "3";
		this.radHour_spec.AutoSize = true;
		this.radHour_spec.Location = new System.Drawing.Point(3, 59);
		this.radHour_spec.Name = "radHour_spec";
		this.radHour_spec.Size = new System.Drawing.Size(194, 17);
		this.radHour_spec.TabIndex = 5;
		this.radHour_spec.TabStop = true;
		this.radHour_spec.Tag = "3";
		this.radHour_spec.Text = "Specific hour (choose one or many)";
		this.radHour_spec.UseVisualStyleBackColor = true;
		this.luk_Hour.Location = new System.Drawing.Point(249, 35);
		this.luk_Hour.Name = "luk_Hour";
		this.luk_Hour.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Hour.Properties.NullText = "";
		this.luk_Hour.Properties.Tag = "Hour_StartAt";
		this.luk_Hour.Size = new System.Drawing.Size(60, 20);
		this.luk_Hour.TabIndex = 4;
		this.luk_Hour.Tag = "2";
		this.labelControl6.Location = new System.Drawing.Point(130, 38);
		this.labelControl6.Name = "labelControl6";
		this.labelControl6.Size = new System.Drawing.Size(113, 13);
		this.labelControl6.TabIndex = 15;
		this.labelControl6.Text = "hour(s) starting at hour";
		this.luk_noOfHours.Location = new System.Drawing.Point(62, 35);
		this.luk_noOfHours.Name = "luk_noOfHours";
		this.luk_noOfHours.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_noOfHours.Properties.NullText = "";
		this.luk_noOfHours.Properties.Tag = "Hour_Every";
		this.luk_noOfHours.Size = new System.Drawing.Size(60, 20);
		this.luk_noOfHours.TabIndex = 3;
		this.luk_noOfHours.Tag = "2";
		this.radHour_everyStart.AutoSize = true;
		this.radHour_everyStart.Location = new System.Drawing.Point(3, 36);
		this.radHour_everyStart.Name = "radHour_everyStart";
		this.radHour_everyStart.Size = new System.Drawing.Size(53, 17);
		this.radHour_everyStart.TabIndex = 2;
		this.radHour_everyStart.TabStop = true;
		this.radHour_everyStart.Tag = "2";
		this.radHour_everyStart.Text = "Every";
		this.radHour_everyStart.UseVisualStyleBackColor = true;
		this.radHour_every.AutoSize = true;
		this.radHour_every.Location = new System.Drawing.Point(3, 13);
		this.radHour_every.Name = "radHour_every";
		this.radHour_every.Size = new System.Drawing.Size(79, 17);
		this.radHour_every.TabIndex = 1;
		this.radHour_every.TabStop = true;
		this.radHour_every.Tag = "1";
		this.radHour_every.Text = "Every Hour";
		this.radHour_every.UseVisualStyleBackColor = true;
		this.tab_Day.Controls.Add(this.luk_WeekDayOrdinal);
		this.tab_Day.Controls.Add(this.luk_OrdinalNumber);
		this.tab_Day.Controls.Add(this.labelControl15);
		this.tab_Day.Controls.Add(this.labelControl14);
		this.tab_Day.Controls.Add(this.luk_DayInMonthNear);
		this.tab_Day.Controls.Add(this.labelControl13);
		this.tab_Day.Controls.Add(this.luk_NoOfMdBef);
		this.tab_Day.Controls.Add(this.labelControl9);
		this.tab_Day.Controls.Add(this.luk_WeekDayLast);
		this.tab_Day.Controls.Add(this.chk_DayInMonth);
		this.tab_Day.Controls.Add(this.radDay_specMth);
		this.tab_Day.Controls.Add(this.labelControl12);
		this.tab_Day.Controls.Add(this.luk_DayInMonth);
		this.tab_Day.Controls.Add(this.labelControl11);
		this.tab_Day.Controls.Add(this.luk_NoOfMd);
		this.tab_Day.Controls.Add(this.radDay_everyStartMd);
		this.tab_Day.Controls.Add(this.radDay_OrdinalWd);
		this.tab_Day.Controls.Add(this.radDay_nearestWd);
		this.tab_Day.Controls.Add(this.radDay_BefEndOfMth);
		this.tab_Day.Controls.Add(this.radDay_lastWdOfMth);
		this.tab_Day.Controls.Add(this.radDay_lastWd);
		this.tab_Day.Controls.Add(this.radDay_lastInMth);
		this.tab_Day.Controls.Add(this.chk_WeekDay);
		this.tab_Day.Controls.Add(this.radDay_specWk);
		this.tab_Day.Controls.Add(this.luk_WeekDay);
		this.tab_Day.Controls.Add(this.labelControl10);
		this.tab_Day.Controls.Add(this.luk_NoOfWd);
		this.tab_Day.Controls.Add(this.radDay_everyStartDw);
		this.tab_Day.Controls.Add(this.radDay_every);
		this.tab_Day.Name = "tab_Day";
		this.tab_Day.Size = new System.Drawing.Size(918, 262);
		this.tab_Day.Tag = "Day_Type";
		this.tab_Day.Text = "Day";
		this.luk_WeekDayOrdinal.Location = new System.Drawing.Point(147, 241);
		this.luk_WeekDayOrdinal.Name = "luk_WeekDayOrdinal";
		this.luk_WeekDayOrdinal.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_WeekDayOrdinal.Properties.NullText = "";
		this.luk_WeekDayOrdinal.Properties.Tag = "Day_WeekDayOrd";
		this.luk_WeekDayOrdinal.Size = new System.Drawing.Size(79, 20);
		this.luk_WeekDayOrdinal.TabIndex = 21;
		this.luk_WeekDayOrdinal.Tag = "11";
		this.luk_OrdinalNumber.Location = new System.Drawing.Point(62, 241);
		this.luk_OrdinalNumber.Name = "luk_OrdinalNumber";
		this.luk_OrdinalNumber.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_OrdinalNumber.Properties.NullText = "";
		this.luk_OrdinalNumber.Properties.Tag = "Day_OrdinalWD";
		this.luk_OrdinalNumber.Size = new System.Drawing.Size(79, 20);
		this.luk_OrdinalNumber.TabIndex = 20;
		this.luk_OrdinalNumber.Tag = "11";
		this.labelControl15.Location = new System.Drawing.Point(232, 244);
		this.labelControl15.Name = "labelControl15";
		this.labelControl15.Size = new System.Drawing.Size(62, 13);
		this.labelControl15.TabIndex = 42;
		this.labelControl15.Text = "of the month";
		this.labelControl14.Location = new System.Drawing.Point(303, 221);
		this.labelControl14.Name = "labelControl14";
		this.labelControl14.Size = new System.Drawing.Size(62, 13);
		this.labelControl14.TabIndex = 42;
		this.labelControl14.Text = "of the month";
		this.luk_DayInMonthNear.Location = new System.Drawing.Point(237, 218);
		this.luk_DayInMonthNear.Name = "luk_DayInMonthNear";
		this.luk_DayInMonthNear.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_DayInMonthNear.Properties.NullText = "";
		this.luk_DayInMonthNear.Size = new System.Drawing.Size(60, 20);
		this.luk_DayInMonthNear.TabIndex = 18;
		this.luk_DayInMonthNear.Tag = "10";
		this.labelControl13.Location = new System.Drawing.Point(87, 196);
		this.labelControl13.Name = "labelControl13";
		this.labelControl13.Size = new System.Drawing.Size(152, 13);
		this.labelControl13.TabIndex = 40;
		this.labelControl13.Text = "day(s) before end of the month";
		this.luk_NoOfMdBef.Location = new System.Drawing.Point(21, 193);
		this.luk_NoOfMdBef.Name = "luk_NoOfMdBef";
		this.luk_NoOfMdBef.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_NoOfMdBef.Properties.NullText = "";
		this.luk_NoOfMdBef.Properties.Tag = "Day_BefMonthEnd";
		this.luk_NoOfMdBef.Size = new System.Drawing.Size(60, 20);
		this.luk_NoOfMdBef.TabIndex = 16;
		this.luk_NoOfMdBef.Tag = "9";
		this.labelControl9.Location = new System.Drawing.Point(163, 175);
		this.labelControl9.Name = "labelControl9";
		this.labelControl9.Size = new System.Drawing.Size(62, 13);
		this.labelControl9.TabIndex = 38;
		this.labelControl9.Text = "of the month";
		this.luk_WeekDayLast.Location = new System.Drawing.Point(78, 172);
		this.luk_WeekDayLast.Name = "luk_WeekDayLast";
		this.luk_WeekDayLast.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_WeekDayLast.Properties.NullText = "";
		this.luk_WeekDayLast.Properties.Tag = "Day_LastWDOfMonth";
		this.luk_WeekDayLast.Size = new System.Drawing.Size(79, 20);
		this.luk_WeekDayLast.TabIndex = 15;
		this.luk_WeekDayLast.Tag = "8";
		this.chk_DayInMonth.EditValue = "";
		this.chk_DayInMonth.Location = new System.Drawing.Point(237, 103);
		this.chk_DayInMonth.Name = "chk_DayInMonth";
		this.chk_DayInMonth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.chk_DayInMonth.Properties.Tag = "Day_SpecMD";
		this.chk_DayInMonth.Size = new System.Drawing.Size(132, 20);
		this.chk_DayInMonth.TabIndex = 11;
		this.chk_DayInMonth.Tag = "5";
		this.radDay_specMth.AutoSize = true;
		this.radDay_specMth.Location = new System.Drawing.Point(3, 104);
		this.radDay_specMth.Name = "radDay_specMth";
		this.radDay_specMth.Size = new System.Drawing.Size(236, 17);
		this.radDay_specMth.TabIndex = 10;
		this.radDay_specMth.TabStop = true;
		this.radDay_specMth.Tag = "5";
		this.radDay_specMth.Text = "Specific day of month (choose one or many)";
		this.radDay_specMth.UseVisualStyleBackColor = true;
		this.labelControl12.Location = new System.Drawing.Point(307, 63);
		this.labelControl12.Name = "labelControl12";
		this.labelControl12.Size = new System.Drawing.Size(62, 13);
		this.labelControl12.TabIndex = 34;
		this.labelControl12.Text = "of the month";
		this.luk_DayInMonth.Location = new System.Drawing.Point(241, 60);
		this.luk_DayInMonth.Name = "luk_DayInMonth";
		this.luk_DayInMonth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_DayInMonth.Properties.NullText = "";
		this.luk_DayInMonth.Properties.Tag = "Day_StartAtMD";
		this.luk_DayInMonth.Size = new System.Drawing.Size(60, 20);
		this.luk_DayInMonth.TabIndex = 7;
		this.luk_DayInMonth.Tag = "3";
		this.labelControl11.Location = new System.Drawing.Point(130, 63);
		this.labelControl11.Name = "labelControl11";
		this.labelControl11.Size = new System.Drawing.Size(105, 13);
		this.labelControl11.TabIndex = 32;
		this.labelControl11.Text = "day(s) starting on the";
		this.luk_NoOfMd.Location = new System.Drawing.Point(62, 60);
		this.luk_NoOfMd.Name = "luk_NoOfMd";
		this.luk_NoOfMd.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_NoOfMd.Properties.NullText = "";
		this.luk_NoOfMd.Properties.Tag = "Day_EveryMD";
		this.luk_NoOfMd.Size = new System.Drawing.Size(60, 20);
		this.luk_NoOfMd.TabIndex = 6;
		this.luk_NoOfMd.Tag = "3";
		this.radDay_everyStartMd.AutoSize = true;
		this.radDay_everyStartMd.Location = new System.Drawing.Point(3, 61);
		this.radDay_everyStartMd.Name = "radDay_everyStartMd";
		this.radDay_everyStartMd.Size = new System.Drawing.Size(53, 17);
		this.radDay_everyStartMd.TabIndex = 5;
		this.radDay_everyStartMd.TabStop = true;
		this.radDay_everyStartMd.Tag = "3";
		this.radDay_everyStartMd.Text = "Every";
		this.radDay_everyStartMd.UseVisualStyleBackColor = true;
		this.radDay_OrdinalWd.AutoSize = true;
		this.radDay_OrdinalWd.Location = new System.Drawing.Point(3, 242);
		this.radDay_OrdinalWd.Name = "radDay_OrdinalWd";
		this.radDay_OrdinalWd.Size = new System.Drawing.Size(58, 17);
		this.radDay_OrdinalWd.TabIndex = 19;
		this.radDay_OrdinalWd.TabStop = true;
		this.radDay_OrdinalWd.Tag = "11";
		this.radDay_OrdinalWd.Text = "On the";
		this.radDay_OrdinalWd.UseVisualStyleBackColor = true;
		this.radDay_nearestWd.AutoSize = true;
		this.radDay_nearestWd.Location = new System.Drawing.Point(3, 219);
		this.radDay_nearestWd.Name = "radDay_nearestWd";
		this.radDay_nearestWd.Size = new System.Drawing.Size(236, 17);
		this.radDay_nearestWd.TabIndex = 17;
		this.radDay_nearestWd.TabStop = true;
		this.radDay_nearestWd.Tag = "10";
		this.radDay_nearestWd.Text = "Nearest weekday (Monday to Friday) to the";
		this.radDay_nearestWd.UseVisualStyleBackColor = true;
		this.radDay_BefEndOfMth.AutoSize = true;
		this.radDay_BefEndOfMth.Location = new System.Drawing.Point(3, 196);
		this.radDay_BefEndOfMth.Name = "radDay_BefEndOfMth";
		this.radDay_BefEndOfMth.Size = new System.Drawing.Size(14, 13);
		this.radDay_BefEndOfMth.TabIndex = 27;
		this.radDay_BefEndOfMth.TabStop = true;
		this.radDay_BefEndOfMth.Tag = "9";
		this.radDay_BefEndOfMth.UseVisualStyleBackColor = true;
		this.radDay_lastWdOfMth.AutoSize = true;
		this.radDay_lastWdOfMth.Location = new System.Drawing.Point(3, 173);
		this.radDay_lastWdOfMth.Name = "radDay_lastWdOfMth";
		this.radDay_lastWdOfMth.Size = new System.Drawing.Size(78, 17);
		this.radDay_lastWdOfMth.TabIndex = 14;
		this.radDay_lastWdOfMth.TabStop = true;
		this.radDay_lastWdOfMth.Tag = "8";
		this.radDay_lastWdOfMth.Text = "On the last";
		this.radDay_lastWdOfMth.UseVisualStyleBackColor = true;
		this.radDay_lastWd.AutoSize = true;
		this.radDay_lastWd.Location = new System.Drawing.Point(3, 150);
		this.radDay_lastWd.Name = "radDay_lastWd";
		this.radDay_lastWd.Size = new System.Drawing.Size(189, 17);
		this.radDay_lastWd.TabIndex = 13;
		this.radDay_lastWd.TabStop = true;
		this.radDay_lastWd.Tag = "7";
		this.radDay_lastWd.Text = "On the last weekday of the month";
		this.radDay_lastWd.UseVisualStyleBackColor = true;
		this.radDay_lastInMth.AutoSize = true;
		this.radDay_lastInMth.Location = new System.Drawing.Point(2, 127);
		this.radDay_lastInMth.Name = "radDay_lastInMth";
		this.radDay_lastInMth.Size = new System.Drawing.Size(164, 17);
		this.radDay_lastInMth.TabIndex = 12;
		this.radDay_lastInMth.TabStop = true;
		this.radDay_lastInMth.Tag = "6";
		this.radDay_lastInMth.Text = "On the last day of the month";
		this.radDay_lastInMth.UseVisualStyleBackColor = true;
		this.chk_WeekDay.EditValue = "";
		this.chk_WeekDay.Location = new System.Drawing.Point(237, 83);
		this.chk_WeekDay.Name = "chk_WeekDay";
		this.chk_WeekDay.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.chk_WeekDay.Properties.Tag = "Day_SpecWD";
		this.chk_WeekDay.Size = new System.Drawing.Size(132, 20);
		this.chk_WeekDay.TabIndex = 9;
		this.chk_WeekDay.Tag = "4";
		this.radDay_specWk.AutoSize = true;
		this.radDay_specWk.Location = new System.Drawing.Point(3, 84);
		this.radDay_specWk.Name = "radDay_specWk";
		this.radDay_specWk.Size = new System.Drawing.Size(231, 17);
		this.radDay_specWk.TabIndex = 8;
		this.radDay_specWk.TabStop = true;
		this.radDay_specWk.Tag = "4";
		this.radDay_specWk.Text = "Specific day of week (choose one or many)";
		this.radDay_specWk.UseVisualStyleBackColor = true;
		this.luk_WeekDay.Location = new System.Drawing.Point(222, 34);
		this.luk_WeekDay.Name = "luk_WeekDay";
		this.luk_WeekDay.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_WeekDay.Properties.NullText = "";
		this.luk_WeekDay.Properties.Tag = "Day_StartAtWD";
		this.luk_WeekDay.Size = new System.Drawing.Size(79, 20);
		this.luk_WeekDay.TabIndex = 4;
		this.luk_WeekDay.Tag = "2";
		this.labelControl10.Location = new System.Drawing.Point(130, 37);
		this.labelControl10.Name = "labelControl10";
		this.labelControl10.Size = new System.Drawing.Size(86, 13);
		this.labelControl10.TabIndex = 15;
		this.labelControl10.Text = "day(s) starting on";
		this.luk_NoOfWd.Location = new System.Drawing.Point(62, 34);
		this.luk_NoOfWd.Name = "luk_NoOfWd";
		this.luk_NoOfWd.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_NoOfWd.Properties.NullText = "";
		this.luk_NoOfWd.Properties.Tag = "Day_EveryWD";
		this.luk_NoOfWd.Size = new System.Drawing.Size(60, 20);
		this.luk_NoOfWd.TabIndex = 3;
		this.luk_NoOfWd.Tag = "2";
		this.radDay_everyStartDw.AutoSize = true;
		this.radDay_everyStartDw.Location = new System.Drawing.Point(3, 35);
		this.radDay_everyStartDw.Name = "radDay_everyStartDw";
		this.radDay_everyStartDw.Size = new System.Drawing.Size(53, 17);
		this.radDay_everyStartDw.TabIndex = 2;
		this.radDay_everyStartDw.TabStop = true;
		this.radDay_everyStartDw.Tag = "2";
		this.radDay_everyStartDw.Text = "Every";
		this.radDay_everyStartDw.UseVisualStyleBackColor = true;
		this.radDay_every.AutoSize = true;
		this.radDay_every.Location = new System.Drawing.Point(3, 12);
		this.radDay_every.Name = "radDay_every";
		this.radDay_every.Size = new System.Drawing.Size(75, 17);
		this.radDay_every.TabIndex = 1;
		this.radDay_every.TabStop = true;
		this.radDay_every.Tag = "1";
		this.radDay_every.Text = "Every Day";
		this.radDay_every.UseVisualStyleBackColor = true;
		this.tab_Month.Controls.Add(this.labelControl7);
		this.tab_Month.Controls.Add(this.luk_MonthTo);
		this.tab_Month.Controls.Add(this.luk_MonthFr);
		this.tab_Month.Controls.Add(this.radMonth_between);
		this.tab_Month.Controls.Add(this.chk_Month);
		this.tab_Month.Controls.Add(this.radMonth_spec);
		this.tab_Month.Controls.Add(this.luk_Month);
		this.tab_Month.Controls.Add(this.labelControl8);
		this.tab_Month.Controls.Add(this.luk_NoOfMonths);
		this.tab_Month.Controls.Add(this.radMonth_everyStart);
		this.tab_Month.Controls.Add(this.radMonth_every);
		this.tab_Month.Name = "tab_Month";
		this.tab_Month.Size = new System.Drawing.Size(918, 262);
		this.tab_Month.Tag = "Month_Type";
		this.tab_Month.Text = "Month";
		this.labelControl7.Location = new System.Drawing.Point(268, 84);
		this.labelControl7.Name = "labelControl7";
		this.labelControl7.Size = new System.Drawing.Size(18, 13);
		this.labelControl7.TabIndex = 22;
		this.labelControl7.Text = "and";
		this.luk_MonthTo.Location = new System.Drawing.Point(292, 81);
		this.luk_MonthTo.Name = "luk_MonthTo";
		this.luk_MonthTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_MonthTo.Properties.NullText = "";
		this.luk_MonthTo.Properties.Tag = "Month_To";
		this.luk_MonthTo.Size = new System.Drawing.Size(120, 20);
		this.luk_MonthTo.TabIndex = 9;
		this.luk_MonthTo.Tag = "4";
		this.luk_MonthFr.Location = new System.Drawing.Point(140, 81);
		this.luk_MonthFr.Name = "luk_MonthFr";
		this.luk_MonthFr.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_MonthFr.Properties.NullText = "";
		this.luk_MonthFr.Properties.Tag = "Month_From";
		this.luk_MonthFr.Size = new System.Drawing.Size(120, 20);
		this.luk_MonthFr.TabIndex = 8;
		this.luk_MonthFr.Tag = "4";
		this.radMonth_between.AutoSize = true;
		this.radMonth_between.Location = new System.Drawing.Point(3, 82);
		this.radMonth_between.Name = "radMonth_between";
		this.radMonth_between.Size = new System.Drawing.Size(131, 17);
		this.radMonth_between.TabIndex = 7;
		this.radMonth_between.TabStop = true;
		this.radMonth_between.Tag = "4";
		this.radMonth_between.Text = "Every month between";
		this.radMonth_between.UseVisualStyleBackColor = true;
		this.chk_Month.Location = new System.Drawing.Point(213, 58);
		this.chk_Month.Name = "chk_Month";
		this.chk_Month.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.chk_Month.Properties.Tag = "Month_Spec";
		this.chk_Month.Size = new System.Drawing.Size(199, 20);
		this.chk_Month.TabIndex = 6;
		this.chk_Month.Tag = "3";
		this.radMonth_spec.AutoSize = true;
		this.radMonth_spec.Location = new System.Drawing.Point(3, 59);
		this.radMonth_spec.Name = "radMonth_spec";
		this.radMonth_spec.Size = new System.Drawing.Size(202, 17);
		this.radMonth_spec.TabIndex = 5;
		this.radMonth_spec.TabStop = true;
		this.radMonth_spec.Tag = "3";
		this.radMonth_spec.Text = "Specific month (choose one or many)";
		this.radMonth_spec.UseVisualStyleBackColor = true;
		this.luk_Month.Location = new System.Drawing.Point(230, 35);
		this.luk_Month.Name = "luk_Month";
		this.luk_Month.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Month.Properties.NullText = "";
		this.luk_Month.Properties.Tag = "Month_StartAt";
		this.luk_Month.Size = new System.Drawing.Size(120, 20);
		this.luk_Month.TabIndex = 4;
		this.luk_Month.Tag = "2";
		this.labelControl8.Location = new System.Drawing.Point(130, 38);
		this.labelControl8.Name = "labelControl8";
		this.labelControl8.Size = new System.Drawing.Size(94, 13);
		this.labelControl8.TabIndex = 15;
		this.labelControl8.Text = "month(s) starting in";
		this.luk_NoOfMonths.Location = new System.Drawing.Point(62, 35);
		this.luk_NoOfMonths.Name = "luk_NoOfMonths";
		this.luk_NoOfMonths.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_NoOfMonths.Properties.NullText = "";
		this.luk_NoOfMonths.Properties.Tag = "Month_Every";
		this.luk_NoOfMonths.Size = new System.Drawing.Size(60, 20);
		this.luk_NoOfMonths.TabIndex = 3;
		this.luk_NoOfMonths.Tag = "2";
		this.radMonth_everyStart.AutoSize = true;
		this.radMonth_everyStart.Location = new System.Drawing.Point(3, 36);
		this.radMonth_everyStart.Name = "radMonth_everyStart";
		this.radMonth_everyStart.Size = new System.Drawing.Size(53, 17);
		this.radMonth_everyStart.TabIndex = 2;
		this.radMonth_everyStart.TabStop = true;
		this.radMonth_everyStart.Tag = "2";
		this.radMonth_everyStart.Text = "Every";
		this.radMonth_everyStart.UseVisualStyleBackColor = true;
		this.radMonth_every.AutoSize = true;
		this.radMonth_every.Location = new System.Drawing.Point(3, 13);
		this.radMonth_every.Name = "radMonth_every";
		this.radMonth_every.Size = new System.Drawing.Size(86, 17);
		this.radMonth_every.TabIndex = 1;
		this.radMonth_every.TabStop = true;
		this.radMonth_every.Tag = "1";
		this.radMonth_every.Text = "Every month";
		this.radMonth_every.UseVisualStyleBackColor = true;
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.groupControl1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(924, 501);
		this.splitContainerControl1.SplitterPosition = 179;
		this.splitContainerControl1.TabIndex = 3;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.Location = new System.Drawing.Point(6, 535);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 4;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.Location = new System.Drawing.Point(84, 535);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 5;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btn_Start.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Start.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Start.ImageOptions.Image");
		this.btn_Start.Location = new System.Drawing.Point(181, 535);
		this.btn_Start.Name = "btn_Start";
		this.btn_Start.Size = new System.Drawing.Size(75, 23);
		this.btn_Start.TabIndex = 6;
		this.btn_Start.Text = "Start";
		this.btn_Start.Visible = false;
		this.btn_Start.Click += new System.EventHandler(this.btn_Start_Click);
		this.btn_Stop.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Stop.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Stop.ImageOptions.Image");
		this.btn_Stop.Location = new System.Drawing.Point(259, 535);
		this.btn_Stop.Name = "btn_Stop";
		this.btn_Stop.Size = new System.Drawing.Size(75, 23);
		this.btn_Stop.TabIndex = 7;
		this.btn_Stop.Text = "Stop";
		this.btn_Stop.Visible = false;
		this.btn_Stop.Click += new System.EventHandler(this.btn_Stop_Click);
		this.lbl_Message.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lbl_Message.EditControl = null;
		this.lbl_Message.EditValue = "labelControl1";
		this.lbl_Message.FieldName = null;
		this.lbl_Message.HideWhenEmpty = false;
		this.lbl_Message.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Message.Location = new System.Drawing.Point(450, 540);
		this.lbl_Message.Name = "lbl_Message";
		this.lbl_Message.ReadOnly = false;
		this.lbl_Message.SettingInVisible = false;
		this.lbl_Message.SettingReadonly = false;
		this.lbl_Message.ShowUnderLine = true;
		this.lbl_Message.Size = new System.Drawing.Size(0, 13);
		this.lbl_Message.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Message.TabIndex = 8;
		this.lbl_Message.Utext = "labelControl1";
		this.txtResult.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txtResult.EditControl = null;
		this.txtResult.FieldName = null;
		this.txtResult.IsSearchControl = false;
		this.txtResult.Location = new System.Drawing.Point(5, 509);
		this.txtResult.Name = "txtResult";
		this.txtResult.Properties.ReadOnly = true;
		this.txtResult.SettingInVisible = false;
		this.txtResult.SettingReadonly = false;
		this.txtResult.Size = new System.Drawing.Size(916, 20);
		this.txtResult.TabIndex = 9;
		this.btn_OpenDashBoard.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OpenDashBoard.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OpenDashBoard.ImageOptions.Image");
		this.btn_OpenDashBoard.Location = new System.Drawing.Point(337, 535);
		this.btn_OpenDashBoard.Name = "btn_OpenDashBoard";
		this.btn_OpenDashBoard.Size = new System.Drawing.Size(109, 23);
		this.btn_OpenDashBoard.TabIndex = 10;
		this.btn_OpenDashBoard.Text = "Open DashBoard";
		this.btn_OpenDashBoard.Visible = false;
		this.btn_OpenDashBoard.Click += new System.EventHandler(this.btn_OpenDashBoard_Click);
		base.AcceptButton = this.btn_OK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(924, 562);
		base.Controls.Add(this.btn_OpenDashBoard);
		base.Controls.Add(this.txtResult);
		base.Controls.Add(this.lbl_Message);
		base.Controls.Add(this.btn_Stop);
		base.Controls.Add(this.btn_Start);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oJobManagement";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_JobsManagement";
		this.Text = "Recurring Jobs Management";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_JobsManagement_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctx_Header.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Active).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tabCtrl_Recurring).EndInit();
		this.tabCtrl_Recurring.ResumeLayout(false);
		this.tab_Second.ResumeLayout(false);
		this.tab_Second.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_SecondTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_SecondFr.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Second.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Second.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_noOfSecs.Properties).EndInit();
		this.tab_Minute.ResumeLayout(false);
		this.tab_Minute.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_MinTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_MinFr.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Min.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Min.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_NoOfMins.Properties).EndInit();
		this.tab_Hour.ResumeLayout(false);
		this.tab_Hour.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_HourTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_HourFr.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Hour.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Hour.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_noOfHours.Properties).EndInit();
		this.tab_Day.ResumeLayout(false);
		this.tab_Day.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_WeekDayOrdinal.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_OrdinalNumber.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_DayInMonthNear.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_NoOfMdBef.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_WeekDayLast.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_DayInMonth.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_DayInMonth.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_NoOfMd.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_WeekDay.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_WeekDay.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_NoOfWd.Properties).EndInit();
		this.tab_Month.ResumeLayout(false);
		this.tab_Month.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_MonthTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_MonthFr.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Month.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Month.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_NoOfMonths.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtResult.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
