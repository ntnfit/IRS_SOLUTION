using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.PlanningProduction;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;

namespace Apzon.Forms.MTS_Manufacturing;

public class FrmMps : XtraFormEx
{
	private readonly HttpClientHelper<Mps> _mpsClientHelper = new HttpClientHelper<Mps>(Configuration.ApiServer, "Mps");

	private readonly HttpClientHelper<WorkSchedulingLines> _workLineClientHelper = new HttpClientHelper<WorkSchedulingLines>(Configuration.ApiServer, "WorkScheduling");

	public string MpsCode = string.Empty;

	public int Id;

	private Mps _oMps = new Mps();

	private DataTable _tableMps = new DataTable();

	private DataTable _tableCapacity = new DataTable();

	private DataTable _tableNgayNghi = new DataTable();

	private int id;

	private string mpsCode;

	private IContainer components;

	private SplitContainer splitContainer1;

	private GroupControl groupControl2;

	private GridControlEx grdControlMps;

	private GridViewEx grdViewMps;

	private RepositoryItemTextEdit txtQty;

	private RepositoryItemTextEdit repositoryItemTextEdit1;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private GridViewEx gridView1;

	private GroupControl groupControl3;

	private GridControlEx grdControlRoughCut;

	private GridViewEx grdViewRoughCut;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btn_ChangedStatus;

	private SimpleButtonEx btnRun;

	private MemoEditEx txtNotes;

	private SaveFileDialog saveFile;

	private DateEditEx date_FixDate;

	private ButtonEditEx txtCode;

	private SimpleButtonEx btn_Export;

	private GroupControl groupControl1;

	private DateEditEx beginDate;

	private ComboBoxEditEx cbStatus;

	private SimpleButtonEx btSave;

	private LabelLine labelLine2;

	private LabelLine labelLine1;

	private LabelLine labelLine5;

	private LabelLine labelLine4;

	private LabelLine labelLine3;

	private ComboBoxEditEx cb_ForeCast;

	private DateEditEx date_endDate;

	private LabelLine labelLine6;

	public bool FlagRun { get; set; }

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
		case Enums.FormViewModeType.Add:
			btSave.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btSave.Text = Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.View:
			btSave.Text = Resources.ButtonOK;
			break;
		}
	}

	public FrmMps()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "FrmMps"))
		{
			FlagRun = false;
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "FrmMps" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btnCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	public FrmMps(int id, string mpsCode)
	{
		FlagRun = false;
		InitializeComponent();
		Id = id;
		MpsCode = mpsCode;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		_mpsClientHelper.Dispose();
		base.Dispose(disposing);
	}

	private async void FrmMps_Load(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			WinCommons._OpenProcessing();
			LoadData();
			if (Id != 0)
			{
				_oMps = await GetOMPS(Id, MpsCode);
				LoadColumn(await GetWorkday1(_oMps), await GetWorkday2(_oMps));
				BindDataToControl();
				await Run();
				WinCommons._CloseProcessing();
				return;
			}
			GetCodeMps();
			EnableDisableControl(flag: true);
			txtCode.Focus();
			txtCode.SelectionStart = txtCode.Text.Length - 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void BindDataToControl()
	{
		try
		{
			txtCode.Text = _oMps.MpsCode;
			txtNotes.Text = _oMps.Notes;
			date_FixDate.EditValue = _oMps.FixDate;
			beginDate.EditValue = _oMps.BeginDate;
			date_endDate.EditValue = _oMps.EndDate;
			WinFuntions._selectItemComboBoxByValue(cbStatus, _oMps.Status.ToString());
			WinFuntions._selectItemComboBoxByValue(cb_ForeCast, _oMps.ForCastID.ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void GetCodeMps()
	{
		try
		{
			using HttpClient client = new HttpClient();
			string apiUri = Function.GetApiUri(Configuration.ApiServer, "Mps", "GetCodeMps");
			using HttpResponseMessage response = await client.GetAsync(apiUri);
			if (response.IsSuccessStatusCode)
			{
				string result = JsonConvert.DeserializeObject<string>(await response.Content.ReadAsStringAsync());
				if (txtCode.InvokeRequired)
				{
					BeginInvoke((MethodInvoker)(() =>
					{
						txtCode.Text = result;
					}));
				}
				else
				{
					txtCode.Text = result;
				}
			}
			else
			{
				WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Error, Msg_Time._TimeMedium, response.StatusCode.ToString() + ":" + response.RequestMessage);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void LoadData()
	{
		try
		{
			List<Common.Table_ID> list = new List<Common.Table_ID>
			{
				new Common.Table_ID
				{
					ID = 1.ToString(),
					Name = "Draft"
				},
				new Common.Table_ID
				{
					ID = 2.ToString(),
					Name = "Aprroved"
				},
				new Common.Table_ID
				{
					ID = (-1).ToString(),
					Name = "Reject"
				}
			};
			WinFuntions._addComboBox(cbStatus, list, _selectFirstItem: true);
			beginDate.DateTime = DateTime.Now;
			date_FixDate.DateTime = beginDate.DateTime.AddDays(14.0);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async void btn_ChangedStatus_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			string text = ((Common.ListItems)cbStatus.SelectedItem).Value.Trim();
			string text2 = null;
			if (text.Equals("1"))
			{
				text2 = await UpdateStatus(Id.ToString(), 2.ToString());
				if (string.IsNullOrEmpty(text2))
				{
					WinFuntions._selectItemComboBoxByValue(cbStatus, 2.ToString());
				}
			}
			else if (text.Equals("2"))
			{
				text2 = await UpdateStatus(Id.ToString(), 1.ToString());
				if (string.IsNullOrEmpty(text2))
				{
					WinFuntions._selectItemComboBoxByValue(cbStatus, 1.ToString());
				}
			}
			if (string.IsNullOrEmpty(text2))
			{
				FormMode = Enums.FormViewModeType.Update;
				WinCommons.writeStatus(Enums.MessageCode.UpdateSuccess, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
			}
			else
			{
				WinCommons.writeStatus(Enums.MessageCode.None, text2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async Task<string> UpdateStatus(string id, string status)
	{
		try
		{
			return await _mpsClientHelper.PostParamsAsJsonAsync("UpdateStatus", new Tuple<string, string>(id, status));
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	private async void btnRun_Click(object sender, EventArgs e)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRun, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			WinCommons._OpenProcessing();
			await Run();
			WinCommons._CloseProcessing();
		}
	}

	private async Task<string> Run()
	{
		_ = 9;
		try
		{
			if (string.IsNullOrEmpty(txtCode.Text))
			{
				WinCommons.writeStatus(Enums.MessageCode.MrpCodeNotNull, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
				return string.Empty;
			}
			if (!IsValidate())
			{
				return string.Empty;
			}
			_ = DateTime.Today;
			_oMps.MpsCode = ((!string.IsNullOrEmpty(MpsCode.Trim())) ? MpsCode : txtCode.Text.Trim());
			_oMps.BeginDate = beginDate.DateTime;
			_oMps.EndDate = date_endDate.DateTime;
			_oMps.CreateDate = date_FixDate.DateTime;
			if (_oMps.Id != 0L)
			{
				_tableNgayNghi = await GetNgayNghi(Function.ParseDateTime(beginDate.EditValue), Function.ParseDateTime(date_endDate.EditValue));
				LoadColumn(await GetWorkday1(_oMps), await GetWorkday2(_oMps));
				EnableDisableControl(flag: false);
				_tableMps = await GetMps1(_oMps);
				grdControlMps.DataSource = _tableMps;
				grdControlMps.Refresh();
				_tableCapacity = await GetMps2(_oMps);
				grdControlRoughCut.DataSource = _tableCapacity;
				grdControlRoughCut.Refresh();
				FlagRun = false;
			}
			else if (IsValidate())
			{
				_tableNgayNghi = await GetNgayNghi(DateTime.Parse(beginDate.EditValue.ToString()), DateTime.Parse(date_endDate.EditValue.ToString()));
				_tableMps = await RunMps1(_oMps);
				_tableCapacity = await RunMps2(_oMps);
				LoadColumn(await RunWorkday1(_oMps), await RunWorkday2(_oMps));
				EnableDisableControl(flag: false);
				grdControlMps.DataSource = _tableMps;
				grdControlMps.Refresh();
				grdControlRoughCut.DataSource = _tableCapacity;
				grdControlRoughCut.Refresh();
				FlagRun = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
		return string.Empty;
	}

	private async Task<DataTable> RunWorkday2(Mps oMps)
	{
		try
		{
			return await _mpsClientHelper.GetDataTableAsJsonAsync("RunWorkday2", oMps);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private async Task<DataTable> RunWorkday1(Mps oMps)
	{
		try
		{
			return await _mpsClientHelper.GetDataTableAsJsonAsync("RunWorkday1", oMps);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private async Task<DataTable> RunMps2(Mps oMps)
	{
		try
		{
			return await _mpsClientHelper.GetDataTableAsJsonAsync("RunMps2", oMps);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private async Task<DataTable> RunMps1(Mps oMps)
	{
		try
		{
			return await _mpsClientHelper.GetDataTableAsJsonAsync("RunMps1", oMps);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private async Task<DataTable> GetMps2(Mps oMps)
	{
		try
		{
			return await _mpsClientHelper.GetDataTableAsJsonAsync("GetMPS2", oMps);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private async Task<DataTable> GetMps1(Mps oMps)
	{
		try
		{
			return await _mpsClientHelper.GetDataTableAsJsonAsync("GetMPS1", oMps);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private async Task<DataTable> GetWorkday2(Mps oMps)
	{
		try
		{
			return await _mpsClientHelper.GetDataTableAsJsonAsync("GetWorkday2", oMps);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private async Task<DataTable> GetWorkday1(Mps oMps)
	{
		try
		{
			return await _mpsClientHelper.GetDataTableAsJsonAsync("GetWorkday1", oMps);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private void EnableDisableControl(bool flag)
	{
		try
		{
			txtCode.Properties.ReadOnly = !flag;
			txtCode.Enabled = flag;
			int num = 0;
			if ("-1" != (string)cbStatus.EditValue)
			{
				num = Convert.ToInt32(((Common.ListItems)cbStatus.EditValue).Value);
			}
			if (num == 1)
			{
				btnRun.Enabled = true;
				return;
			}
			FormMode = Enums.FormViewModeType.View;
			btnRun.Enabled = false;
		}
		catch (Exception ex)
		{
			Logging.Write(ex.Message);
		}
	}

	private void LoadColumn(DataTable dt1, DataTable dt2)
	{
		grdViewMps.Columns.Clear();
		GridColumn gridColumn = grdViewMps.Columns.Add();
		gridColumn.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
		gridColumn.Caption = "ID";
		gridColumn.FieldName = "ID";
		gridColumn.Name = "gColId";
		gridColumn.Visible = false;
		gridColumn.OptionsColumn.AllowMerge = DefaultBoolean.False;
		gridColumn.OptionsColumn.AllowMove = false;
		GridColumn gridColumn2 = grdViewMps.Columns.Add();
		gridColumn2.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
		gridColumn2.Caption = "MpsCode";
		gridColumn2.FieldName = "MpsCode";
		gridColumn2.Name = "gColMpsCode";
		gridColumn2.Visible = false;
		gridColumn2.OptionsColumn.AllowMerge = DefaultBoolean.False;
		gridColumn2.OptionsColumn.AllowMove = false;
		GridColumn gridColumn3 = grdViewMps.Columns.Add();
		gridColumn3.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
		gridColumn3.Caption = "Number Machine";
		gridColumn3.FieldName = "NumMachine";
		gridColumn3.Name = "gNumMachine";
		gridColumn3.Visible = false;
		gridColumn3.OptionsColumn.AllowMerge = DefaultBoolean.False;
		gridColumn3.OptionsColumn.AllowMove = false;
		GridColumn gridColumn4 = grdViewMps.Columns.Add();
		gridColumn4.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
		gridColumn4.Caption = "Marchine Time";
		gridColumn4.FieldName = "MachineTime";
		gridColumn4.Name = "gMachineTime";
		gridColumn4.Visible = false;
		gridColumn4.OptionsColumn.AllowMerge = DefaultBoolean.False;
		gridColumn4.OptionsColumn.AllowMove = false;
		GridColumn gridColumn5 = grdViewMps.Columns.Add();
		gridColumn5.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
		gridColumn5.Caption = "Item";
		gridColumn5.FieldName = "Item";
		gridColumn5.Name = "gColItem";
		gridColumn5.GroupIndex = 0;
		gridColumn5.Visible = true;
		gridColumn5.OptionsColumn.AllowMerge = DefaultBoolean.False;
		gridColumn5.OptionsColumn.AllowMove = false;
		gridColumn5.OptionsColumn.AllowSort = DefaultBoolean.False;
		GridColumn gridColumn6 = grdViewMps.Columns.Add();
		gridColumn6.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
		gridColumn6.Caption = "Description";
		gridColumn6.FieldName = "Type";
		gridColumn6.Name = "gColType";
		gridColumn6.Visible = true;
		gridColumn6.Fixed = FixedStyle.Left;
		gridColumn6.Width = 150;
		gridColumn6.VisibleIndex = 0;
		gridColumn6.OptionsColumn.AllowMerge = DefaultBoolean.False;
		gridColumn6.OptionsColumn.AllowMove = false;
		gridColumn6.OptionsColumn.AllowSort = DefaultBoolean.False;
		GridColumn gridColumn7 = grdViewMps.Columns.Add();
		gridColumn7.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
		gridColumn7.Caption = "Line";
		gridColumn7.FieldName = "Line";
		gridColumn7.Name = "Line";
		gridColumn7.Visible = true;
		gridColumn7.VisibleIndex = 1;
		gridColumn7.Fixed = FixedStyle.Left;
		gridColumn7.Width = 80;
		gridColumn7.OptionsColumn.AllowMerge = DefaultBoolean.False;
		gridColumn7.OptionsColumn.AllowMove = false;
		gridColumn7.OptionsColumn.AllowSort = DefaultBoolean.False;
		GridColumn gridColumn8 = grdViewMps.Columns.Add();
		gridColumn8.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
		gridColumn8.Caption = "Open Balance";
		gridColumn8.FieldName = "OpenBalance";
		gridColumn8.Name = "gColOpenBalance";
		gridColumn8.Visible = true;
		gridColumn8.Fixed = FixedStyle.Left;
		gridColumn8.OptionsColumn.AllowEdit = false;
		gridColumn8.Width = 80;
		gridColumn8.VisibleIndex = 2;
		gridColumn8.OptionsColumn.AllowMerge = DefaultBoolean.False;
		gridColumn8.OptionsColumn.AllowMove = false;
		DateTime dateTime = DateTime.Parse(date_FixDate.EditValue.ToString());
		for (int i = 0; i < dt1.Rows.Count; i++)
		{
			string text = dt1.Rows[i]["WorkDay"].ToString();
			DateTime dateTime2 = Convert.ToDateTime(text);
			if (grdViewMps.Columns[text] != null)
			{
				continue;
			}
			GridColumn gridColumn9 = grdViewMps.Columns.Add();
			gridColumn9.GroupFormat.FormatType = FormatType.Numeric;
			RepositoryItemCalcEdit repositoryItemCalcEdit = new RepositoryItemCalcEdit();
			repositoryItemCalcEdit.KeyDown += ri_KeyDown;
			repositoryItemCalcEdit.Name = "gCol" + text;
			repositoryItemCalcEdit.AutoHeight = false;
			repositoryItemCalcEdit.Buttons.Clear();
			repositoryItemCalcEdit.AllowMouseWheel = false;
			repositoryItemCalcEdit.DisplayFormat.FormatString = "n0";
			repositoryItemCalcEdit.DisplayFormat.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit.EditFormat.FormatString = "n0";
			repositoryItemCalcEdit.EditFormat.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit.Mask.EditMask = "n0";
			if (dateTime < dateTime2)
			{
				gridColumn9.AppearanceCell.BackColor = Color.Pink;
			}
			gridColumn9.OptionsFilter.AllowFilter = false;
			gridColumn9.ColumnEdit = repositoryItemCalcEdit;
			gridColumn9.Caption = dateTime2.ToString("dd/MM/yyyy");
			gridColumn9.FieldName = text;
			gridColumn9.Name = text;
			gridColumn9.Visible = true;
			gridColumn9.VisibleIndex = i + 3;
			gridColumn9.Width = 50;
			gridColumn9.DisplayFormat.FormatType = FormatType.Numeric;
			gridColumn9.DisplayFormat.FormatString = "n0";
			gridColumn9.OptionsColumn.AllowMerge = DefaultBoolean.False;
			gridColumn9.OptionsColumn.AllowMove = false;
			gridColumn9.OptionsColumn.AllowSort = DefaultBoolean.False;
			if (_tableNgayNghi == null || _tableNgayNghi.Rows.Count <= 0)
			{
				continue;
			}
			_tableNgayNghi.BeginLoadData();
			for (int j = 0; j < _tableNgayNghi.Rows.Count; j++)
			{
				if (DateTime.Parse(_tableNgayNghi.Rows[j]["WorkDate"].ToString()).Equals(dateTime2))
				{
					gridColumn9.OptionsColumn.AllowEdit = false;
					break;
				}
			}
			_tableNgayNghi.EndLoadData();
		}
		grdViewRoughCut.Columns.Clear();
		GridColumn gridColumn10 = grdViewRoughCut.Columns.Add();
		gridColumn10.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
		gridColumn10.Caption = "ID";
		gridColumn10.FieldName = "ID";
		gridColumn10.Name = "gcId";
		gridColumn10.Visible = false;
		gridColumn10.OptionsColumn.AllowMerge = DefaultBoolean.False;
		gridColumn10.OptionsColumn.AllowMove = false;
		GridColumn gridColumn11 = grdViewRoughCut.Columns.Add();
		gridColumn11.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
		gridColumn11.Caption = "MpsCode";
		gridColumn11.FieldName = "MpsCode";
		gridColumn11.Name = "gcMpsCode";
		gridColumn11.Visible = false;
		gridColumn11.OptionsColumn.AllowMerge = DefaultBoolean.False;
		gridColumn11.OptionsColumn.AllowMove = false;
		GridColumn gridColumn12 = grdViewRoughCut.Columns.Add();
		gridColumn12.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
		gridColumn12.Caption = "Resource";
		gridColumn12.FieldName = "Item";
		gridColumn12.Name = "gcItem";
		gridColumn12.GroupIndex = 0;
		gridColumn12.Visible = true;
		gridColumn12.OptionsColumn.AllowMerge = DefaultBoolean.False;
		gridColumn12.OptionsColumn.AllowMove = false;
		GridColumn gridColumn13 = grdViewRoughCut.Columns.Add();
		gridColumn13.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
		gridColumn13.Caption = "Type";
		gridColumn13.FieldName = "Type";
		gridColumn13.Name = "gColType";
		gridColumn13.Visible = true;
		gridColumn13.Fixed = FixedStyle.Left;
		gridColumn13.Width = 310;
		gridColumn13.VisibleIndex = 0;
		gridColumn13.OptionsColumn.AllowMerge = DefaultBoolean.False;
		gridColumn13.OptionsColumn.AllowMove = false;
		gridColumn13.OptionsColumn.AllowSort = DefaultBoolean.False;
		for (int k = 0; k < dt2.Rows.Count; k++)
		{
			string text2 = dt2.Rows[k]["WorkDay"].ToString();
			DateTime dateTime3 = Convert.ToDateTime(text2);
			if (grdViewRoughCut.Columns[text2] == null)
			{
				GridColumn gridColumn14 = grdViewRoughCut.Columns.Add();
				RepositoryItemCalcEdit repositoryItemCalcEdit2 = new RepositoryItemCalcEdit();
				repositoryItemCalcEdit2.Name = "gc" + text2;
				repositoryItemCalcEdit2.Buttons.Clear();
				repositoryItemCalcEdit2.DisplayFormat.FormatString = "n2";
				repositoryItemCalcEdit2.DisplayFormat.FormatType = FormatType.Numeric;
				repositoryItemCalcEdit2.EditFormat.FormatString = "n2";
				repositoryItemCalcEdit2.EditFormat.FormatType = FormatType.Numeric;
				repositoryItemCalcEdit2.Mask.EditMask = "n2";
				if (dateTime < dateTime3)
				{
					gridColumn14.AppearanceCell.BackColor = Color.Pink;
				}
				gridColumn14.GroupFormat.FormatType = FormatType.Numeric;
				gridColumn14.OptionsFilter.AllowFilter = false;
				gridColumn14.DisplayFormat.FormatType = FormatType.Numeric;
				gridColumn14.DisplayFormat.FormatString = "n2";
				gridColumn14.ColumnEdit = repositoryItemCalcEdit2;
				gridColumn14.Caption = dateTime3.ToString("dd/MM/yyyy");
				gridColumn14.FieldName = text2;
				gridColumn14.Name = text2;
				gridColumn14.Visible = true;
				gridColumn14.Width = 50;
				gridColumn14.VisibleIndex = k + 1;
				gridColumn14.OptionsColumn.AllowMerge = DefaultBoolean.False;
				gridColumn14.OptionsColumn.AllowMove = false;
				gridColumn14.OptionsColumn.AllowSort = DefaultBoolean.False;
				gridColumn14.OptionsColumn.AllowEdit = false;
			}
		}
	}

	private void ri_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.Modifiers != Keys.Control || e.KeyCode != Keys.V)
			{
				return;
			}
			DataObject dataObject = (DataObject)Clipboard.GetDataObject();
			if (dataObject.GetDataPresent(DataFormats.Text))
			{
				string[] array = Regex.Split(dataObject.GetData(DataFormats.Text).ToString().TrimEnd("\t".ToCharArray()), "\t");
				if (array != null && array.Length != 0)
				{
					PasteData(array);
					Clipboard.Clear();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void PasteData(string[] selectedRows)
	{
		try
		{
			if (grdViewMps.GetSelectedRows() != null && grdViewMps.GetSelectedRows().Length != 0)
			{
				for (int i = 0; i < selectedRows.Length; i++)
				{
					double num = (string.IsNullOrEmpty(selectedRows[i].Trim()) ? 0.0 : double.Parse(selectedRows[i].Trim()));
					DateTime dateTime = DateTime.Parse(grdViewMps.FocusedColumn.FieldName).AddDays(i);
					grdViewMps.SetRowCellValue(grdViewMps.FocusedRowHandle, dateTime.ToString("MM/dd/yyyy"), num);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GetNgayNghi(DateTime? tuNgay, DateTime? denNgay)
	{
		try
		{
			return await _workLineClientHelper.GetDataTableAsync("GetNgayNghi", new { tuNgay, denNgay });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
		return null;
	}

	private bool IsValidate()
	{
		if (_oMps.Id == 0L)
		{
			if (DateTime.Compare(DateTime.Now.Date, date_FixDate.DateTime.Date) > 0)
			{
				WinCommons.writeStatus(Enums.MessageCode.CreateDateGreaterNow, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
				date_FixDate.DateTime = DateTime.Now;
				return false;
			}
			if (DateTime.Compare(beginDate.DateTime.Date.Date, DateTime.Now.Date) < 0)
			{
				WinCommons.writeStatus(Enums.MessageCode.FromDateGreaterToday, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
				beginDate.Focus();
				return false;
			}
		}
		if (date_FixDate.DateTime < beginDate.DateTime)
		{
			WinCommons.writeStatus(Enums.MessageCode.FixDateGreaterFromDate, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
			date_FixDate.Focus();
			return false;
		}
		return true;
	}

	private async void btSave_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			WinCommons._OpenProcessing();
			await Save();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
			WinCommons._CloseProcessing();
		}
	}

	private async Task<string> Save()
	{
		_ = 5;
		try
		{
			grdViewMps.ActiveFilterString = null;
			grdViewRoughCut.ActiveFilterString = null;
			string mpsCode = txtCode.Text;
			_oMps = await GetOMPS(0, mpsCode);
			if (_oMps == null)
			{
				_oMps = new Mps();
			}
			int value = 0;
			if ("-1" != cbStatus.EditValue.ToString())
			{
				value = Convert.ToInt32(((Common.ListItems)cbStatus.EditValue).Value);
			}
			if (!IsValidate())
			{
				return string.Empty;
			}
			_oMps.MpsCode = mpsCode;
			_oMps.CreateDate = DateTime.Today;
			_oMps.FixDate = date_FixDate.DateTime;
			_oMps.BeginDate = beginDate.DateTime;
			_oMps.EndDate = date_endDate.DateTime;
			_oMps.Notes = txtNotes.Text;
			_oMps.Status = value;
			if (cb_ForeCast.SelectedItem != null)
			{
				_oMps.ForCastID = int.Parse(((Common.ListItems)cb_ForeCast.SelectedItem).Value);
			}
			DataTable dt1 = new DataTable
			{
				Columns = 
				{
					"Id", "Item", "Type", "Line", "MpsCode", "WorkDay", "Qty", "OpenBalance", "Notes", "MachineTime",
					"NumMachine"
				}
			};
			DataTable dt2 = new DataTable
			{
				Columns = { "Id", "Item", "Type", "MpsCode", "WorkDay", "Qty", "Notes" }
			};
			new DataTable();
			new DataTable();
			DataTable wd1;
			DataTable tbWorkDay;
			if (!FlagRun)
			{
				wd1 = await GetWorkday1(_oMps);
				tbWorkDay = await GetWorkday2(_oMps);
			}
			else
			{
				wd1 = await RunWorkday1(_oMps);
				tbWorkDay = await RunWorkday2(_oMps);
			}
			GetDataTableOnGrid(grdViewMps, wd1, dt1);
			GetDataTableOnGrid(grdViewRoughCut, tbWorkDay, dt2);
			string text = await SaveData(_oMps, dt1, dt2);
			if (text == null)
			{
				WinCommons.writeStatus(Enums.MessageCode.Success, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
				FormMode = Enums.FormViewModeType.View;
			}
			else
			{
				WinCommons.writeStatus(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return string.Empty;
	}

	private async Task<string> SaveData(Mps oMps, DataTable dt1, DataTable dt2)
	{
		try
		{
			return await _mpsClientHelper.PostParamsAsJsonAsync("SaveData", new Tuple<Mps, DataTable, DataTable>(oMps, dt1, dt2));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return string.Empty;
	}

	private void GetDataTableOnGrid(GridView gc, DataTable tbWorkDay, DataTable tbDes)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable = ((!gc.Name.Equals("grdViewMps")) ? ((DataTable)grdControlRoughCut.DataSource) : ((DataView)grdViewMps.DataSource).ToTable());
			if (dataTable == null)
			{
				return;
			}
			int count = dataTable.Rows.Count;
			for (int i = 0; i < count; i++)
			{
				int rowHandle = gc.GetRowHandle(i);
				if (rowHandle < 0)
				{
					continue;
				}
				string value = ((gc.GetDataRow(rowHandle)["Item"] == null) ? null : gc.GetDataRow(rowHandle)["Item"].ToString());
				string value2 = ((gc.GetDataRow(rowHandle)["MpsCode"] == null) ? null : gc.GetDataRow(rowHandle)["MpsCode"].ToString());
				string text = ((gc.GetDataRow(rowHandle)["Type"] == null) ? "" : gc.GetDataRow(rowHandle)["Type"].ToString());
				string value3 = null;
				int num = 0;
				double num2 = 0.0;
				try
				{
					if (gc.Name.Equals("grdViewMps"))
					{
						if (gc.GetDataRow(rowHandle)["Line"] == null)
						{
							value3 = "";
						}
						else
						{
							value3 = ((!string.IsNullOrEmpty(gc.GetDataRow(rowHandle)["Line"].ToString())) ? gc.GetDataRow(rowHandle)["Line"].ToString() : "");
						}
						num = ((gc.GetDataRow(rowHandle)["NumMachine"] != null) ? ((!string.IsNullOrEmpty(gc.GetDataRow(rowHandle)["NumMachine"].ToString())) ? int.Parse(gc.GetDataRow(rowHandle)["NumMachine"].ToString()) : 0) : 0);
						if (gc.GetDataRow(rowHandle)["MachineTime"] == null)
						{
							num2 = 0.0;
						}
						else
						{
							num2 = ((!string.IsNullOrEmpty(gc.GetDataRow(rowHandle)["MachineTime"].ToString())) ? double.Parse(gc.GetDataRow(rowHandle)["MachineTime"].ToString()) : 0.0);
						}
						if (text.Trim().Equals("1") || text.Trim().Equals("2") || text.Trim().Equals("3") || text.Trim().Equals("6"))
						{
							continue;
						}
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
				}
				tbWorkDay.BeginLoadData();
				for (int j = 0; j < tbWorkDay.Rows.Count; j++)
				{
					try
					{
						string text2 = tbWorkDay.Rows[j]["WorkDay"].ToString();
						object obj = gc.GetDataRow(rowHandle)[text2] ?? null;
						object obj2 = 0;
						DataRow dataRow = tbDes.NewRow();
						dataRow["Item"] = value;
						dataRow["MpsCode"] = value2;
						dataRow["Type"] = text;
						dataRow["WorkDay"] = text2;
						if (gc.Name.Equals("grdViewMps"))
						{
							dataRow["Line"] = value3;
							dataRow["NumMachine"] = num;
							dataRow["MachineTime"] = num2;
							obj2 = gc.GetDataRow(rowHandle)["OpenBalance"] ?? null;
							if (obj2 == null)
							{
								dataRow["OpenBalance"] = 0;
							}
							else if (string.IsNullOrEmpty(obj2.ToString()))
							{
								dataRow["OpenBalance"] = 0;
							}
							else
							{
								dataRow["OpenBalance"] = double.Parse(obj2.ToString());
							}
						}
						if (obj == null)
						{
							dataRow["Qty"] = 0;
						}
						else if (string.IsNullOrEmpty(obj.ToString()))
						{
							dataRow["Qty"] = 0;
						}
						else
						{
							dataRow["Qty"] = double.Parse(obj.ToString());
						}
						tbDes.Rows.Add(dataRow);
					}
					catch (Exception ex2)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
						WinCommons.writeStatus(Enums.MessageCode.None, "Error this lan " + j + " grid view " + gc.Name + " " + ex2.Message);
					}
				}
				tbWorkDay.EndLoadData();
			}
		}
		catch (Exception ex3)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex3.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex3.Message);
		}
	}

	private async Task<Mps> GetOMPS(int id, string mpsCode)
	{
		try
		{
			return await _mpsClientHelper.GetAsync("GetOmps", new { id, mpsCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new Mps();
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btn_Export_Click(object sender, EventArgs e)
	{
		try
		{
			saveFile.FileName = "MPS" + DateTime.Now.ToString("yyyyMMdd");
			saveFile.ValidateNames = true;
			saveFile.Filter = "Excel 97-2003 Workbook (*.xls)|*.xls |Excel Workbook (*.xlsx)|*.xlsx";
			saveFile.Title = "Export to excel";
			if (saveFile.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(saveFile.FileName))
			{
				return;
			}
			string directoryName = Path.GetDirectoryName(saveFile.FileName);
			string text = Path.GetExtension(saveFile.FileName).ToLower().Trim();
			if (!(text == ".xls"))
			{
				if (text == ".xlsx")
				{
					grdControlMps.ExportToXlsx(saveFile.FileName);
					grdControlRoughCut.ExportToXls(directoryName + "\\MPSCapacity" + DateTime.Now.ToString("yyyyMMdd"));
				}
				else
				{
					grdControlMps.ExportToXls(saveFile.FileName);
					grdControlRoughCut.ExportToXls(directoryName + "\\MPSCapacity" + DateTime.Now.ToString("yyyyMMdd"));
				}
			}
			else
			{
				grdControlMps.ExportToXls(saveFile.FileName);
				grdControlRoughCut.ExportToXls(directoryName + "\\MPSCapacity" + DateTime.Now.ToString("yyyyMMdd"));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void grdViewMps_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!grdViewMps.GetRowCellValue(e.RowHandle, "Type").ToString().Equals("5"))
			{
				return;
			}
			DateTime dateTime = DateTime.Parse(e.Column.FieldName);
			DateTime dateTime2 = dateTime.AddDays(-1.0);
			string value = ((grdViewMps.GetRowCellValue(e.RowHandle, "Line") == null) ? "" : grdViewMps.GetRowCellValue(e.RowHandle, "Line").ToString());
			double num = ((grdViewMps.GetRowCellValue(e.RowHandle - 1, dateTime2.ToString("MM/dd/yyyy").Substring(0, 10)) == null || string.IsNullOrEmpty(grdViewMps.GetRowCellValue(e.RowHandle - 1, dateTime2.ToString("MM/dd/yyyy").Substring(0, 10)).ToString())) ? 0.0 : double.Parse(grdViewMps.GetRowCellValue(e.RowHandle - 1, dateTime2.ToString("MM/dd/yyyy").Substring(0, 10)).ToString()));
			double num2 = ((grdViewMps.GetRowCellValue(e.RowHandle - 2, e.Column) == null || string.IsNullOrEmpty(grdViewMps.GetRowCellValue(e.RowHandle - 2, e.Column).ToString())) ? 0.0 : double.Parse(grdViewMps.GetRowCellValue(e.RowHandle - 2, e.Column).ToString()));
			double num3 = ((grdViewMps.GetRowCellValue(e.RowHandle, e.Column) == null || string.IsNullOrEmpty(grdViewMps.GetRowCellValue(e.RowHandle, e.Column).ToString())) ? 0.0 : double.Parse(grdViewMps.GetRowCellValue(e.RowHandle, e.Column).ToString()));
			double num4 = ((grdViewMps.GetRowCellValue(e.RowHandle + 1, e.Column) == null || string.IsNullOrEmpty(grdViewMps.GetRowCellValue(e.RowHandle + 1, e.Column).ToString())) ? 0.0 : double.Parse(grdViewMps.GetRowCellValue(e.RowHandle + 1, e.Column).ToString()));
			double num5 = num + num4 + num3 - num2;
			grdViewMps.SetRowCellValue(e.RowHandle - 1, e.Column, num5);
			DateTime dateTime3 = dateTime.AddDays(1.0);
			DateTime dateTime4 = Convert.ToDateTime(date_endDate.EditValue.ToString());
			while (dateTime3 <= dateTime4)
			{
				double num6 = ((grdViewMps.GetRowCellValue(e.RowHandle + 1, dateTime3.ToString("MM/dd/yyyy").Substring(0, 10)) == null) ? 0.0 : double.Parse(grdViewMps.GetRowCellValue(e.RowHandle + 1, dateTime3.ToString("MM/dd/yyyy").Substring(0, 10)).ToString()));
				double num7 = ((grdViewMps.GetRowCellValue(e.RowHandle - 2, dateTime3.ToString("MM/dd/yyyy").Substring(0, 10)) == null) ? 0.0 : double.Parse(grdViewMps.GetRowCellValue(e.RowHandle - 2, dateTime3.ToString("MM/dd/yyyy").Substring(0, 10)).ToString()));
				double num8 = ((grdViewMps.GetRowCellValue(e.RowHandle, dateTime3.ToString("MM/dd/yyyy").Substring(0, 10)) == null || string.IsNullOrEmpty(grdViewMps.GetRowCellValue(e.RowHandle, dateTime3.ToString("MM/dd/yyyy").Substring(0, 10)).ToString())) ? 0.0 : double.Parse(grdViewMps.GetRowCellValue(e.RowHandle, dateTime3.ToString("MM/dd/yyyy").Substring(0, 10)).ToString()));
				double num9 = ((grdViewMps.GetRowCellValue(e.RowHandle - 1, dateTime3.AddDays(-1.0).ToString("MM/dd/yyyy").Substring(0, 10)) == null || string.IsNullOrEmpty(grdViewMps.GetRowCellValue(e.RowHandle - 1, dateTime3.AddDays(-1.0).ToString("MM/dd/yyyy").Substring(0, 10)).ToString())) ? 0.0 : double.Parse(grdViewMps.GetRowCellValue(e.RowHandle - 1, dateTime3.AddDays(-1.0).ToString("MM/dd/yyyy").Substring(0, 10)).ToString())) + num6 + num8 - num7;
				grdViewMps.SetRowCellValue(e.RowHandle - 1, dateTime3.ToString("MM/dd/yyyy").Substring(0, 10), num9);
				dateTime3 = dateTime3.AddDays(1.0);
			}
			double num10 = 0.0;
			for (int i = 0; i < _tableMps.Rows.Count; i++)
			{
				string text = _tableMps.Rows[i]["Line"].ToString().Trim();
				string text2 = _tableMps.Rows[i]["Type"].ToString().Trim();
				if (text.Equals(value) && text2.Equals("3"))
				{
					double num11 = (string.IsNullOrEmpty(_tableMps.Rows[i][e.Column.FieldName.ToString()].ToString()) ? 0.0 : double.Parse(_tableMps.Rows[i][e.Column.FieldName.ToString()].ToString()));
					double num12 = (string.IsNullOrEmpty(_tableMps.Rows[i]["NumMachine"].ToString()) ? 0.0 : double.Parse(_tableMps.Rows[i]["NumMachine"].ToString()));
					double num13 = (string.IsNullOrEmpty(_tableMps.Rows[i]["MachineTime"].ToString()) ? 0.0 : double.Parse(_tableMps.Rows[i]["MachineTime"].ToString()));
					num10 += num11 * num12 * num13;
				}
				else if (text.Equals(value) && text2.Equals("5"))
				{
					double num14 = (string.IsNullOrEmpty(_tableMps.Rows[i][e.Column.FieldName.ToString()].ToString()) ? 0.0 : double.Parse(_tableMps.Rows[i][e.Column.FieldName.ToString()].ToString()));
					double num15 = (string.IsNullOrEmpty(_tableMps.Rows[i]["NumMachine"].ToString()) ? 0.0 : double.Parse(_tableMps.Rows[i]["NumMachine"].ToString()));
					double num16 = (string.IsNullOrEmpty(_tableMps.Rows[i]["MachineTime"].ToString()) ? 0.0 : double.Parse(_tableMps.Rows[i]["MachineTime"].ToString()));
					num10 += num14 * num15 * num16;
				}
			}
			double num17 = 0.0;
			double num18 = 0.0;
			string columnName = dateTime.ToString("MM/dd/yyyy").Substring(0, 10);
			for (int j = 0; j < _tableCapacity.Rows.Count; j++)
			{
				if (_tableCapacity.Rows[j]["Item"].ToString().Equals(value) && _tableCapacity.Rows[j]["Type"].ToString().Equals("2"))
				{
					num17 = 0.0;
					num18 = (string.IsNullOrEmpty(_tableCapacity.Rows[j - 1][columnName].ToString()) ? 0.0 : double.Parse(_tableCapacity.Rows[j - 1][columnName].ToString()));
					if (num18 == 0.0)
					{
						num17 = 0.0;
						continue;
					}
					_tableCapacity.Rows[j][columnName] = num10;
					num17 = num10 / num18;
				}
				else if (_tableCapacity.Rows[j]["Item"].ToString().Equals(value) && _tableCapacity.Rows[j]["Type"].ToString().Equals("3"))
				{
					_tableCapacity.Rows[j][columnName] = num17;
				}
			}
			grdControlRoughCut.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void grdViewMps_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		if (e.Column.FieldName.Equals("OpenBalance") && grdViewMps.FocusedRowHandle >= 0 && Convert.ToInt32(grdViewMps.GetRowCellValue(grdViewMps.FocusedRowHandle, "Type")) != TypeMps1.Stock)
		{
			grdViewMps.SetRowCellValue(grdViewMps.FocusedRowHandle, "OpenBalance", 0);
		}
	}

	private void grdViewMps_ColumnFilterChanged(object sender, EventArgs e)
	{
		try
		{
			string valueDisplayText = grdViewMps.Columns["Line"].FilterInfo.GetValueDisplayText();
			valueDisplayText = valueDisplayText.Replace("[Line]", "[Item]");
			grdViewRoughCut.ActiveFilterString = valueDisplayText;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void grdViewMps_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		if (e.Column.FieldName.Equals("Type") && e.Value != null)
		{
			if (e.DisplayText == TypeMps1.Order.ToString())
			{
				e.DisplayText = "1. Sales Order";
			}
			else if (e.DisplayText == TypeMps1.Forcast.ToString())
			{
				e.DisplayText = "2. Forecast";
			}
			else if (e.DisplayText == TypeMps1.Total12.ToString())
			{
				e.DisplayText = "3. Total 1+2";
			}
			else if (e.DisplayText == TypeMps1.Stock.ToString())
			{
				e.DisplayText = "4. Stock";
			}
			else if (e.DisplayText == TypeMps1.ProductionPlan.ToString())
			{
				e.DisplayText = "5. Production Plan";
			}
			else if (e.DisplayText == TypeMps1.RequirmentMPS.ToString())
			{
				e.DisplayText = "6. Requirement (MPS)";
			}
		}
	}

	private void grdViewMps_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
	{
		SetColorColumn(e);
	}

	private void SetColorColumn(ColumnHeaderCustomDrawEventArgs e)
	{
		try
		{
			if (e.Column != null && !e.Column.FieldName.Equals("Item") && !e.Column.FieldName.Equals("Type") && !e.Column.FieldName.Equals("MpsCode") && !e.Column.FieldName.Equals("Qty") && !e.Column.FieldName.Equals("WorkDay") && !e.Column.FieldName.Equals("Notes") && !e.Column.FieldName.Equals("Line") && !e.Column.FieldName.Equals("OpenBalance"))
			{
				if (Convert.ToDateTime(e.Column.FieldName).DayOfWeek == DayOfWeek.Saturday)
				{
					e.Column.AppearanceCell.BackColor = Color.GreenYellow;
				}
				else if (Convert.ToDateTime(e.Column.FieldName).DayOfWeek == DayOfWeek.Sunday)
				{
					e.Column.AppearanceCell.BackColor = Color.Salmon;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void grdViewMps_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception)
		{
		}
	}

	private void grdViewMps_EndGrouping(object sender, EventArgs e)
	{
		if (grdViewMps.RowCount > 0)
		{
			grdViewMps.SetRowExpanded(-1, expanded: true);
		}
	}

	private void grdViewMps_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			GridViewMenu gridViewMenu = new GridViewMenu(grdViewMps);
			string[] array = new string[1] { "Paste Table" };
			foreach (string caption in array)
			{
				DXMenuItem dXMenuItem = new DXMenuItem();
				dXMenuItem.Caption = caption;
				dXMenuItem.Click += itemMenu_Click;
				gridViewMenu.Items.Add(dXMenuItem);
			}
			if (e.MenuType == GridMenuType.User || e.MenuType == GridMenuType.Row)
			{
				e.Allow = true;
				gridViewMenu.Show(e.Point);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void itemMenu_Click(object sender, EventArgs e)
	{
		try
		{
			if (!(sender as DXMenuItem).Caption.Equals("Paste Table"))
			{
				return;
			}
			DataObject dataObject = (DataObject)Clipboard.GetDataObject();
			if (dataObject.GetDataPresent(DataFormats.Text))
			{
				string[] array = Regex.Split(dataObject.GetData(DataFormats.Text).ToString().TrimEnd("\r\n".ToCharArray()), "\r\n");
				if (array != null && array.Length != 0)
				{
					PasteDataGrid(array);
					Clipboard.Clear();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void PasteDataGrid(string[] selectedRows)
	{
		try
		{
			DataTable dataTable = (DataTable)grdControlMps.DataSource;
			dataTable.BeginLoadData();
			for (int i = 0; i < selectedRows.Length; i++)
			{
				string[] array = Regex.Split(selectedRows[i], "\t");
				string text = array[0].ToString();
				DataRow[] array2 = dataTable.Select("Item = '" + text + "' and Type = 5");
				if (array2 != null && array2.Length != 0)
				{
					DateTime dateTime = DateTime.Parse(beginDate.EditValue.ToString());
					DateTime.Parse(date_FixDate.EditValue.ToString());
					DateTime dateTime2 = DateTime.Parse(date_endDate.EditValue.ToString());
					array2[0].BeginEdit();
					int num = 0;
					while (dateTime <= dateTime2)
					{
						string columnName = dateTime.ToString("MM/dd/yyyy");
						array2[0][columnName] = ((array[num + 2] == null || string.IsNullOrEmpty(array[num + 2].ToString())) ? 0.0 : double.Parse(array[num + 2].ToString()));
						num++;
						dateTime = dateTime.AddDays(1.0);
					}
					array2[0].EndEdit();
					dataTable.AcceptChanges();
				}
			}
			dataTable.EndLoadData();
			grdControlMps.DataSource = dataTable;
			grdControlMps.RefreshDataSource();
			CalculatorGrid(dataTable);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void CalculatorGrid(DataTable dt)
	{
		try
		{
			DataRow[] array = dt.Select("Type = 5");
			if (array == null || array.Length == 0)
			{
				return;
			}
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i]["Item"].ToString();
				DataRow[] array2 = dt.Select("Item = '" + text + "' and Type = 4");
				DataRow[] array3 = dt.Select("Item = '" + text + "' and Type = 3");
				DataRow[] array4 = dt.Select("Item = '" + text + "' and Type = 6");
				DateTime dateTime = DateTime.Parse(beginDate.EditValue.ToString());
				DateTime.Parse(date_FixDate.EditValue.ToString());
				DateTime dateTime2 = DateTime.Parse(date_endDate.EditValue.ToString());
				array[i].BeginEdit();
				int num = 0;
				while (dateTime <= dateTime2)
				{
					string columnName = dateTime.AddDays(-1.0).ToString("MM/dd/yyyy");
					string columnName2 = dateTime.ToString("MM/dd/yyyy");
					double num2 = ((array4[0][columnName2] == null || string.IsNullOrEmpty(array4[0][columnName2].ToString())) ? 0.0 : double.Parse(array4[0][columnName2].ToString()));
					double num3 = ((array3[0][columnName2] == null || string.IsNullOrEmpty(array3[0][columnName2].ToString())) ? 0.0 : double.Parse(array3[0][columnName2].ToString()));
					double num4 = ((array[0][columnName2] == null || string.IsNullOrEmpty(array[0][columnName2].ToString())) ? 0.0 : double.Parse(array[0][columnName2].ToString()));
					double num5 = 0.0;
					if (num > 0)
					{
						num5 = (string.IsNullOrEmpty(array2[0][columnName].ToString()) ? 0.0 : double.Parse(array2[0][columnName].ToString()));
					}
					double num6 = num5 + num2 + num4 - num3;
					array2[0][columnName2] = num6;
					num++;
					dateTime = dateTime.AddDays(1.0);
				}
				array[i].EndEdit();
			}
			dt.AcceptChanges();
			grdControlMps.DataSource = dt;
			grdControlMps.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void grdViewRoughCut_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			double num = ((grdViewRoughCut.GetRowCellValue(e.RowHandle - 1, e.Column) == null) ? 0.0 : double.Parse(grdViewRoughCut.GetRowCellValue(e.RowHandle - 1, e.Column).ToString()));
			double num2 = ((grdViewRoughCut.GetRowCellValue(e.RowHandle, e.Column) == null) ? 0.0 : double.Parse(grdViewRoughCut.GetRowCellValue(e.RowHandle, e.Column).ToString()));
			double num3 = 0.0;
			num3 = ((num != 0.0) ? (num2 / num) : 0.0);
			grdViewRoughCut.SetRowCellValue(e.RowHandle + 1, e.Column, num3);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void grdViewRoughCut_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		if (e.Column.FieldName.Equals("Type") && e.Value != null)
		{
			if (e.DisplayText == TypeMps2.AvailableCapacity.ToString())
			{
				e.DisplayText = "1. Available Capacity";
			}
			if (e.DisplayText == TypeMps2.RequirementCapacity.ToString())
			{
				e.DisplayText = "2. Requirement Capacity";
			}
			if (e.DisplayText == TypeMps2.CapacityLoad.ToString())
			{
				e.DisplayText = "3. Capacity Load";
			}
		}
	}

	private void grdViewRoughCut_CustomDrawColumnHeader(object sender, ColumnHeaderCustomDrawEventArgs e)
	{
		SetColorColumn(e);
	}

	private void grdViewRoughCut_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception)
		{
		}
	}

	private void grdViewRoughCut_EndGrouping(object sender, EventArgs e)
	{
		if (grdViewRoughCut.RowCount > 0)
		{
			grdViewRoughCut.SetRowExpanded(-1, expanded: true);
		}
	}

	private void grdViewRoughCut_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (grdViewRoughCut.GetRowCellValue(e.RowHandle, "Type") != null)
			{
				e.Appearance.BackColor = Color.LightGray;
			}
		}
		catch (Exception)
		{
		}
	}

	private void grdViewRoughCut_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (grdViewRoughCut.GetSelectedRows() != null && grdViewRoughCut.GetSelectedRows().Length != 0)
			{
				e.Cancel = true;
			}
			else
			{
				e.Cancel = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.MTS_Manufacturing.FrmMps));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.splitContainer1 = new System.Windows.Forms.SplitContainer();
		this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
		this.grdControlMps = new Apzon.Controls.Grid.GridControlEx();
		this.grdViewMps = new Apzon.Controls.Grid.GridViewEx();
		this.txtQty = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gridView1 = new Apzon.Controls.Grid.GridViewEx();
		this.groupControl3 = new DevExpress.XtraEditors.GroupControl();
		this.grdControlRoughCut = new Apzon.Controls.Grid.GridControlEx();
		this.grdViewRoughCut = new Apzon.Controls.Grid.GridViewEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_ChangedStatus = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnRun = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtNotes = new Apzon.Controls.Editor.MemoEditEx();
		this.saveFile = new System.Windows.Forms.SaveFileDialog();
		this.date_FixDate = new Apzon.Controls.Editor.DateEditEx();
		this.txtCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.btn_Export = new Apzon.Controls.Controls.SimpleButtonEx();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.cb_ForeCast = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.date_endDate = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.beginDate = new Apzon.Controls.Editor.DateEditEx();
		this.cbStatus = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.btSave = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).BeginInit();
		this.splitContainer1.Panel1.SuspendLayout();
		this.splitContainer1.Panel2.SuspendLayout();
		this.splitContainer1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).BeginInit();
		this.groupControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdControlMps).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdViewMps).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtQty).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl3).BeginInit();
		this.groupControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdControlRoughCut).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdViewRoughCut).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtNotes.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FixDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FixDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cb_ForeCast.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_endDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_endDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.beginDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.beginDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbStatus.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainer1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainer1.Location = new System.Drawing.Point(1, 120);
		this.splitContainer1.Name = "splitContainer1";
		this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
		this.splitContainer1.Panel1.Controls.Add(this.groupControl2);
		this.splitContainer1.Panel2.Controls.Add(this.groupControl3);
		this.splitContainer1.Size = new System.Drawing.Size(685, 300);
		this.splitContainer1.SplitterDistance = 145;
		this.splitContainer1.TabIndex = 57;
		this.groupControl2.Controls.Add(this.grdControlMps);
		this.groupControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl2.Location = new System.Drawing.Point(0, 0);
		this.groupControl2.Name = "groupControl2";
		this.groupControl2.Size = new System.Drawing.Size(685, 145);
		this.groupControl2.TabIndex = 0;
		this.groupControl2.Text = "MPS";
		this.grdControlMps.AllowPasteExcel = true;
		this.grdControlMps.ClipboardData = null;
		this.grdControlMps.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdControlMps.Location = new System.Drawing.Point(2, 23);
		this.grdControlMps.MainView = this.grdViewMps;
		this.grdControlMps.Name = "grdControlMps";
		this.grdControlMps.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.txtQty, this.repositoryItemTextEdit1, this.repositoryItemMemoExEdit1 });
		this.grdControlMps.Size = new System.Drawing.Size(681, 120);
		this.grdControlMps.TabIndex = 0;
		this.grdControlMps.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.grdViewMps, this.gridView1 });
		this.grdViewMps.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.grdViewMps.ColumnKey = null;
		this.grdViewMps.CustomizationFormBounds = new System.Drawing.Rectangle(808, 274, 216, 183);
		this.grdViewMps.DisableFormSetting = false;
		this.grdViewMps.DisplayException = false;
		this.grdViewMps.EditColumn = null;
		this.grdViewMps.FieldCurrency = null;
		this.grdViewMps.FieldRate = null;
		this.grdViewMps.FormId = null;
		this.grdViewMps.GridControl = this.grdControlMps;
		this.grdViewMps.IgnoreFilter = false;
		this.grdViewMps.IndexEditRow = -1;
		this.grdViewMps.IndicatorWidth = 40;
		this.grdViewMps.IsApzTable = false;
		this.grdViewMps.IsInsertNewRow = false;
		this.grdViewMps.IsLoadingData = false;
		this.grdViewMps.IsMappingContextAction = false;
		this.grdViewMps.ItemId = null;
		this.grdViewMps.LstColumnAllowBatch = null;
		this.grdViewMps.LstColumnNotAllowBatch = null;
		this.grdViewMps.LstColumnNotEditable = null;
		this.grdViewMps.LstColumNotSetting = null;
		this.grdViewMps.Name = "grdViewMps";
		this.grdViewMps.OptionsView.AllowCellMerge = true;
		this.grdViewMps.OptionsView.ColumnAutoWidth = false;
		this.grdViewMps.OptionsView.ShowGroupPanel = false;
		this.grdViewMps.UseBatchMod = false;
		this.grdViewMps.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.grdViewMps_CustomDrawColumnHeader);
		this.grdViewMps.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.grdViewMps_CustomDrawRowIndicator);
		this.grdViewMps.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.grdViewMps_PopupMenuShowing);
		this.grdViewMps.EndGrouping += new System.EventHandler(this.grdViewMps_EndGrouping);
		this.grdViewMps.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.grdViewMps_CellValueChanged);
		this.grdViewMps.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.grdViewMps_CellValueChanging);
		this.grdViewMps.ColumnFilterChanged += new System.EventHandler(this.grdViewMps_ColumnFilterChanged);
		this.grdViewMps.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.grdViewMps_CustomColumnDisplayText);
		this.txtQty.AutoHeight = false;
		this.txtQty.Name = "txtQty";
		this.repositoryItemTextEdit1.AutoHeight = false;
		this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.gridView1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView1.ColumnKey = null;
		this.gridView1.DisableFormSetting = false;
		this.gridView1.DisplayException = false;
		this.gridView1.EditColumn = null;
		this.gridView1.FieldCurrency = null;
		this.gridView1.FieldRate = null;
		this.gridView1.FormId = null;
		this.gridView1.GridControl = this.grdControlMps;
		this.gridView1.IgnoreFilter = false;
		this.gridView1.IndexEditRow = -1;
		this.gridView1.IndicatorWidth = 35;
		this.gridView1.IsApzTable = false;
		this.gridView1.IsInsertNewRow = false;
		this.gridView1.IsLoadingData = false;
		this.gridView1.IsMappingContextAction = false;
		this.gridView1.ItemId = null;
		this.gridView1.LstColumnAllowBatch = null;
		this.gridView1.LstColumnNotAllowBatch = null;
		this.gridView1.LstColumnNotEditable = null;
		this.gridView1.LstColumNotSetting = null;
		this.gridView1.Name = "gridView1";
		this.gridView1.UseBatchMod = false;
		this.groupControl3.Controls.Add(this.grdControlRoughCut);
		this.groupControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl3.Location = new System.Drawing.Point(0, 0);
		this.groupControl3.Name = "groupControl3";
		this.groupControl3.Size = new System.Drawing.Size(685, 151);
		this.groupControl3.TabIndex = 0;
		this.groupControl3.Text = "ROUGH CUT CAPACITY";
		this.grdControlRoughCut.AllowPasteExcel = true;
		this.grdControlRoughCut.ClipboardData = null;
		this.grdControlRoughCut.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdControlRoughCut.Location = new System.Drawing.Point(2, 23);
		this.grdControlRoughCut.MainView = this.grdViewRoughCut;
		this.grdControlRoughCut.Name = "grdControlRoughCut";
		this.grdControlRoughCut.Size = new System.Drawing.Size(681, 126);
		this.grdControlRoughCut.TabIndex = 0;
		this.grdControlRoughCut.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.grdViewRoughCut });
		this.grdViewRoughCut.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.grdViewRoughCut.ColumnKey = null;
		this.grdViewRoughCut.DisableFormSetting = false;
		this.grdViewRoughCut.DisplayException = false;
		this.grdViewRoughCut.EditColumn = null;
		this.grdViewRoughCut.FieldCurrency = null;
		this.grdViewRoughCut.FieldRate = null;
		this.grdViewRoughCut.FormId = null;
		this.grdViewRoughCut.GridControl = this.grdControlRoughCut;
		this.grdViewRoughCut.IgnoreFilter = false;
		this.grdViewRoughCut.IndexEditRow = -1;
		this.grdViewRoughCut.IndicatorWidth = 40;
		this.grdViewRoughCut.IsApzTable = false;
		this.grdViewRoughCut.IsInsertNewRow = false;
		this.grdViewRoughCut.IsLoadingData = false;
		this.grdViewRoughCut.IsMappingContextAction = false;
		this.grdViewRoughCut.ItemId = null;
		this.grdViewRoughCut.LstColumnAllowBatch = null;
		this.grdViewRoughCut.LstColumnNotAllowBatch = null;
		this.grdViewRoughCut.LstColumnNotEditable = null;
		this.grdViewRoughCut.LstColumNotSetting = null;
		this.grdViewRoughCut.Name = "grdViewRoughCut";
		this.grdViewRoughCut.OptionsView.AllowCellMerge = true;
		this.grdViewRoughCut.OptionsView.ColumnAutoWidth = false;
		this.grdViewRoughCut.OptionsView.ShowGroupPanel = false;
		this.grdViewRoughCut.UseBatchMod = false;
		this.grdViewRoughCut.CustomDrawColumnHeader += new DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventHandler(this.grdViewRoughCut_CustomDrawColumnHeader);
		this.grdViewRoughCut.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.grdViewRoughCut_CustomDrawRowIndicator);
		this.grdViewRoughCut.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.grdViewRoughCut_RowStyle);
		this.grdViewRoughCut.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.grdViewRoughCut_ShowingEditor);
		this.grdViewRoughCut.EndGrouping += new System.EventHandler(this.grdViewRoughCut_EndGrouping);
		this.grdViewRoughCut.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.grdViewRoughCut_CellValueChanged);
		this.grdViewRoughCut.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.grdViewRoughCut_CustomColumnDisplayText);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(85, 426);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 56;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btn_ChangedStatus.EditControl = null;
		this.btn_ChangedStatus.EditValue = "Change";
		this.btn_ChangedStatus.FieldName = null;
		this.btn_ChangedStatus.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ChangedStatus.ImageOptions.Image");
		this.btn_ChangedStatus.IsSearchControl = false;
		this.btn_ChangedStatus.Location = new System.Drawing.Point(607, 21);
		this.btn_ChangedStatus.Name = "btn_ChangedStatus";
		this.btn_ChangedStatus.ReadOnly = false;
		this.btn_ChangedStatus.SettingInVisible = false;
		this.btn_ChangedStatus.SettingReadonly = false;
		this.btn_ChangedStatus.Size = new System.Drawing.Size(75, 23);
		this.btn_ChangedStatus.TabIndex = 25;
		this.btn_ChangedStatus.Text = "Change";
		this.btn_ChangedStatus.Click += new System.EventHandler(this.btn_ChangedStatus_Click);
		this.btnRun.EditControl = null;
		this.btnRun.EditValue = "Run";
		this.btnRun.FieldName = null;
		this.btnRun.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnRun.ImageOptions.Image");
		this.btnRun.IsSearchControl = false;
		this.btnRun.Location = new System.Drawing.Point(607, 63);
		this.btnRun.Name = "btnRun";
		this.btnRun.ReadOnly = false;
		this.btnRun.SettingInVisible = false;
		this.btnRun.SettingReadonly = false;
		this.btnRun.Size = new System.Drawing.Size(75, 23);
		this.btnRun.TabIndex = 7;
		this.btnRun.Text = "Run";
		this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
		this.txtNotes.EditControl = null;
		this.txtNotes.FieldName = null;
		this.txtNotes.IsSearchControl = false;
		this.txtNotes.Location = new System.Drawing.Point(126, 45);
		this.txtNotes.Name = "txtNotes";
		this.txtNotes.SettingInVisible = false;
		this.txtNotes.SettingReadonly = false;
		this.txtNotes.Size = new System.Drawing.Size(150, 41);
		this.txtNotes.TabIndex = 1;
		this.date_FixDate.EditControl = null;
		this.date_FixDate.EditValue = null;
		this.date_FixDate.FieldName = null;
		this.date_FixDate.Location = new System.Drawing.Point(451, 66);
		this.date_FixDate.Name = "date_FixDate";
		this.date_FixDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FixDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.date_FixDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.date_FixDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_FixDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.date_FixDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_FixDate.Properties.IsSearchControl = false;
		this.date_FixDate.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.date_FixDate.SettingInVisible = false;
		this.date_FixDate.SettingReadonly = false;
		this.date_FixDate.Size = new System.Drawing.Size(150, 20);
		this.date_FixDate.TabIndex = 5;
		this.txtCode.EditControl = null;
		this.txtCode.FieldName = null;
		this.txtCode.IsSearchControl = false;
		this.txtCode.Location = new System.Drawing.Point(126, 21);
		this.txtCode.Name = "txtCode";
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.txtCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtCode.RightToLeft = System.Windows.Forms.RightToLeft.No;
		this.txtCode.SettingInVisible = false;
		this.txtCode.SettingReadonly = false;
		this.txtCode.Size = new System.Drawing.Size(150, 24);
		this.txtCode.TabIndex = 0;
		this.btn_Export.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Export.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Export.EditControl = null;
		this.btn_Export.EditValue = "Export";
		this.btn_Export.FieldName = null;
		this.btn_Export.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Export.ImageOptions.Image");
		this.btn_Export.IsSearchControl = false;
		this.btn_Export.Location = new System.Drawing.Point(166, 426);
		this.btn_Export.Name = "btn_Export";
		this.btn_Export.ReadOnly = false;
		this.btn_Export.SettingInVisible = false;
		this.btn_Export.SettingReadonly = false;
		this.btn_Export.Size = new System.Drawing.Size(75, 23);
		this.btn_Export.TabIndex = 58;
		this.btn_Export.Text = "Export";
		this.btn_Export.Click += new System.EventHandler(this.btn_Export_Click);
		this.groupControl1.Controls.Add(this.labelLine6);
		this.groupControl1.Controls.Add(this.cb_ForeCast);
		this.groupControl1.Controls.Add(this.date_endDate);
		this.groupControl1.Controls.Add(this.labelLine5);
		this.groupControl1.Controls.Add(this.labelLine4);
		this.groupControl1.Controls.Add(this.labelLine3);
		this.groupControl1.Controls.Add(this.labelLine2);
		this.groupControl1.Controls.Add(this.labelLine1);
		this.groupControl1.Controls.Add(this.btn_ChangedStatus);
		this.groupControl1.Controls.Add(this.btnRun);
		this.groupControl1.Controls.Add(this.txtNotes);
		this.groupControl1.Controls.Add(this.beginDate);
		this.groupControl1.Controls.Add(this.date_FixDate);
		this.groupControl1.Controls.Add(this.cbStatus);
		this.groupControl1.Controls.Add(this.txtCode);
		this.groupControl1.Dock = System.Windows.Forms.DockStyle.Top;
		this.groupControl1.Location = new System.Drawing.Point(0, 0);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(686, 114);
		this.groupControl1.TabIndex = 54;
		this.groupControl1.Text = "MPS Information";
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "To Forecast Date";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(327, 90);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(124, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 33;
		this.labelLine6.Utext = "To Forecast Date";
		this.cb_ForeCast.EditControl = null;
		this.cb_ForeCast.FieldName = null;
		this.cb_ForeCast.IsSearchControl = false;
		this.cb_ForeCast.Location = new System.Drawing.Point(126, 87);
		this.cb_ForeCast.Name = "cb_ForeCast";
		this.cb_ForeCast.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cb_ForeCast.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.cb_ForeCast.SettingInVisible = false;
		this.cb_ForeCast.SettingReadonly = false;
		this.cb_ForeCast.Size = new System.Drawing.Size(150, 20);
		this.cb_ForeCast.TabIndex = 31;
		this.cb_ForeCast.Visible = false;
		this.date_endDate.EditControl = null;
		this.date_endDate.EditValue = null;
		this.date_endDate.FieldName = null;
		this.date_endDate.Location = new System.Drawing.Point(451, 89);
		this.date_endDate.Name = "date_endDate";
		this.date_endDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_endDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.date_endDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.date_endDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_endDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.date_endDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_endDate.Properties.IsSearchControl = false;
		this.date_endDate.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.date_endDate.SettingInVisible = false;
		this.date_endDate.SettingReadonly = false;
		this.date_endDate.Size = new System.Drawing.Size(150, 20);
		this.date_endDate.TabIndex = 32;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "To Date";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(327, 68);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(124, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 30;
		this.labelLine5.Utext = "To Date";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "From Date";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(327, 47);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(124, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 29;
		this.labelLine4.Utext = "From Date";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Status";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(327, 23);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(124, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 28;
		this.labelLine3.Utext = "Status";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Remarks";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(4, 67);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(123, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 27;
		this.labelLine2.Utext = "Remarks";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "MPS Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(2, 26);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(124, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 26;
		this.labelLine1.Utext = "MPS Code";
		this.beginDate.EditControl = null;
		this.beginDate.EditValue = null;
		this.beginDate.FieldName = null;
		this.beginDate.Location = new System.Drawing.Point(451, 44);
		this.beginDate.Name = "beginDate";
		this.beginDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.beginDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.beginDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.beginDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.beginDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.beginDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.beginDate.Properties.IsSearchControl = false;
		this.beginDate.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.beginDate.SettingInVisible = false;
		this.beginDate.SettingReadonly = false;
		this.beginDate.Size = new System.Drawing.Size(150, 20);
		this.beginDate.TabIndex = 4;
		this.cbStatus.EditControl = null;
		this.cbStatus.FieldName = null;
		this.cbStatus.IsSearchControl = false;
		this.cbStatus.Location = new System.Drawing.Point(451, 21);
		this.cbStatus.Name = "cbStatus";
		this.cbStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbStatus.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.cbStatus.SettingInVisible = false;
		this.cbStatus.SettingReadonly = false;
		this.cbStatus.Size = new System.Drawing.Size(150, 20);
		this.cbStatus.TabIndex = 3;
		this.btSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btSave.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btSave.EditControl = null;
		this.btSave.EditValue = "Save";
		this.btSave.FieldName = null;
		this.btSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btSave.ImageOptions.Image");
		this.btSave.IsSearchControl = false;
		this.btSave.Location = new System.Drawing.Point(4, 426);
		this.btSave.Name = "btSave";
		this.btSave.ReadOnly = false;
		this.btSave.SettingInVisible = false;
		this.btSave.SettingReadonly = false;
		this.btSave.Size = new System.Drawing.Size(75, 23);
		this.btSave.TabIndex = 55;
		this.btSave.Text = "Save";
		this.btSave.Click += new System.EventHandler(this.btSave_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(686, 459);
		base.Controls.Add(this.splitContainer1);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btn_Export);
		base.Controls.Add(this.groupControl1);
		base.Controls.Add(this.btSave);
		base.IconOptions.ShowIcon = false;
		base.Name = "FrmMps";
		base.ShowInTaskbar = false;
		this.Text = "MPS";
		base.Load += new System.EventHandler(this.FrmMps_Load);
		this.splitContainer1.Panel1.ResumeLayout(false);
		this.splitContainer1.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).EndInit();
		this.splitContainer1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.groupControl2).EndInit();
		this.groupControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdControlMps).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdViewMps).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtQty).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl3).EndInit();
		this.groupControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdControlRoughCut).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdViewRoughCut).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtNotes.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FixDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FixDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cb_ForeCast.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_endDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_endDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.beginDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.beginDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbStatus.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
