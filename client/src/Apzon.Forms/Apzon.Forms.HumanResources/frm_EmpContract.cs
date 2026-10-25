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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources;

public class frm_EmpContract : XtraFormEx
{
	private enum AutoCompleteFlag
	{
		Auto = 1,
		BackToOldText = 2,
		SetEmpty = 3,
		None = 0
	}

	private const string FormController = "EmpContract";

	private bool isCancel;

	private bool _isLoading;

	private int focusedRow;

	private bool fromCFL;

	private bool isTab;

	private bool isLeave;

	private bool isPressed;

	private bool hasValueOnNewRow;

	private bool isFill;

	private bool isDelete;

	private bool isSorting;

	private bool fromUser;

	private bool canAutoComplete;

	private string autoName = "";

	private int autoNumber;

	private DataTable dtOHEM;

	private Dictionary<int, string> dicOHEM;

	private AutoCompleteFlag _autoCompleteFlag;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private List<CurrencyCodesImpl> salaryCurrData;

	public int EmpID;

	public string FirstName;

	public string MiddleName;

	public string LastName;

	public ReloadContract ReloadContractDelegate;

	private IContainer components;

	private GroupControl group_ContractList;

	private GroupControl group_Details;

	private GridControlEx gridControl1;

	private GridViewEx gridView1;

	private GridColumnEx contractID;

	private GridColumnEx code;

	private GridColumnEx typeName;

	private GridColumnEx salary;

	private GridColumnEx signDate;

	private GridColumnEx startDate;

	private GridColumnEx endDate;

	private GridColumnEx note;

	private TabControl tbl_Details;

	private TabPage tab_GeneralDetails;

	private TextEditEx txt_contractCode;

	private TextEditEx txt_EmpName;

	private TextEditEx txt_EmpID;

	private LabelControl labelControl5;

	private LabelControl labelControl4;

	private LabelControl labelControl3;

	private LabelControl labelControl2;

	private LabelControl labelControl1;

	private LabelControl lbl_TenNhanVien;

	private LabelControl lbl_ID;

	private MemoEditEx txt_note;

	private CalcEditEx cal_salary;

	private CalcEditEx cal_ProbSalary;

	private CalcEditEx cal_basicSalary;

	private DateEditEx date_endDate;

	private DateEditEx date_startDate;

	private DateEditEx date_signDate;

	private LookUpEdit luk_contractType;

	private LabelControl labelControl12;

	private LabelControl labelControl11;

	private LabelControl labelControl10;

	private LabelControl labelControl9;

	private LabelControl labelControl8;

	private LabelControl labelControl7;

	private LabelControl labelControl6;

	private RepositoryItemDateEdit repositoryItemDateEdit2;

	private GridColumnEx signer;

	private GridColumnEx ProbSalary;

	private GridColumnEx basicSalary;

	private GridColumnEx firstName;

	private GridColumnEx middleName;

	private GridColumnEx lastName;

	private GridColumnEx gc_empID;

	private GridColumnEx gridColumn3;

	private LookUpEdit luk_PymMeth;

	private GridColumnEx gridColumn4;

	private GridColumnEx salaryCurr;

	private GridColumnEx signerID;

	private TextEditEx txt_signerID;

	private ButtonEditEx txt_signer;

	private TextEditEx txt_contractID;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private LookUpEdit luk_salaryCurr;

	private SimpleButtonEx btnOK;

	private SimpleButtonEx btCancel;

	private GridColumnEx gridColumn1;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btCancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btnOK.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btnOK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btnOK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_EmpContract()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			FormId = "oEmpContract";
			gridView1.FormId = FormId;
			gridView1.ItemId = "APZ_OHEC";
			return;
		}
		throw new Exception(Apzon.Commons.Properties.Resources.Msg_FormAccessDenied);
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		isCancel = true;
		Close();
	}

	private void FillControls()
	{
		isFill = true;
		txt_contractID.EditValue = Function.ToString(gridView1.GetFocusedRowCellValue("contractID"));
		luk_contractType.EditValue = Function.ParseInt(gridView1.GetFocusedRowCellValue("type"));
		txt_contractCode.EditValue = Function.ToString(gridView1.GetFocusedRowCellValue("code"));
		txt_signerID.EditValue = Function.ToString(gridView1.GetFocusedRowCellValue("signerID"));
		txt_signer.EditValue = Function.ToString(gridView1.GetFocusedRowCellValue("signer"));
		date_signDate.EditValue = Function.ParseDateTime(gridView1.GetFocusedRowCellValue("signDate"));
		date_startDate.EditValue = Function.ParseDateTime(gridView1.GetFocusedRowCellValue("startDate"));
		date_endDate.EditValue = Function.ParseDateTime(gridView1.GetFocusedRowCellValue("endDate"));
		cal_basicSalary.EditValue = Function.ParseDouble(gridView1.GetFocusedRowCellValue("basicSalary"));
		cal_ProbSalary.EditValue = Function.ParseDouble(gridView1.GetFocusedRowCellValue("ProbSalary"));
		cal_salary.EditValue = Function.ParseDouble(gridView1.GetFocusedRowCellValue("salary"));
		luk_salaryCurr.EditValue = Function.ToString(gridView1.GetFocusedRowCellValue("salaryCurr"));
		luk_PymMeth.EditValue = Function.ToString(gridView1.GetFocusedRowCellValue("PymMeth"));
		txt_note.EditValue = Function.ToString(gridView1.GetFocusedRowCellValue("note"));
		if (Function.ToString(txt_contractID.EditValue) == "0" || Function.ToString(txt_contractID.EditValue) == "")
		{
			FormMode = Enums.FormViewModeType.Add;
		}
		else
		{
			FormMode = Enums.FormViewModeType.View;
		}
		isFill = false;
	}

	private async Task LoadData()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "EmpContract");
			DataTable dataTable = await helper.GetAsync("LoadDoc", new
			{
				empID = EmpID
			});
			if (!dataTable.IsNotNull())
			{
				dataTable = WinFuntions.GetTableStructure("APZ_OHEC");
				REFRESH();
			}
			DataView defaultView = dataTable.DefaultView;
			defaultView.Sort = "signDate asc";
			dataTable = defaultView.ToTable();
			if (!dataTable.ExistsColumn("IsNewRow"))
			{
				dataTable.Columns.Add("IsNewRow", typeof(string));
			}
			foreach (DataRow row in dataTable.Rows)
			{
				row["IsNewRow"] = "N";
			}
			focusedRow = gridView1.FocusedRowHandle;
			gridControl1.DataSource = dataTable.Copy();
			gridView1.FocusedRowHandle = focusedRow;
			if (Function.ToString(EmpID) != "" || Function.ToString(EmpID) != "0")
			{
				txt_EmpID.Text = Function.ToString(EmpID);
				txt_EmpName.Text = FirstName + " " + MiddleName + " " + LastName;
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if ((FormMode != Enums.FormViewModeType.Update && !hasValueOnNewRow) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.No)
			{
				_isLoading = true;
				await LoadData();
				FillControls();
				LoadContractType();
				hasValueOnNewRow = false;
				_isLoading = false;
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetEmployeeList()
	{
		try
		{
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadEmployees();
			if (!dataTable.IsNotNull())
			{
				dataTable = WinFuntions.GetTableStructure("OHEM");
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async void frm_ContractManagement_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			_isLoading = true;
			gridView1.OptionsBehavior.KeepFocusedRowOnUpdate = true;
			gridView1.OptionsCustomization.AllowGroup = false;
			dtOHEM = await GetEmployeeList();
			dicOHEM = dtOHEM.AsEnumerable().ToDictionary((DataRow s) => Function.ParseInt(s["empID"]), (DataRow s) => s["lastName"]?.ToString() + ", " + s["firstName"]);
			await LoadData();
			LoadContractType();
			FillControls();
			luk_contractType.Properties.NullText = "";
			luk_contractType.Properties.ValueMember = "typeID";
			luk_contractType.Properties.DisplayMember = "code";
			luk_contractType.Properties.Columns["typeID"].Visible = false;
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("typeID", typeof(string));
			dataTable.Columns.Add("name", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["typeID"] = "-1";
			dataRow["name"] = "";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			dataRow = dataTable.NewRow();
			dataRow["typeID"] = "5";
			dataRow["name"] = "Bank Transfer";
			dataTable.Rows.Add(dataRow);
			luk_PymMeth.Properties.Columns.Add(new LookUpColumnInfo("typeID", 100, "type"));
			luk_PymMeth.Properties.Columns.Add(new LookUpColumnInfo("name", 150, "Name"));
			luk_PymMeth.Properties.DropDownRows = dataTable.Rows.Count;
			dataTable.AcceptChanges();
			luk_PymMeth.Properties.DataSource = dataTable.Copy();
			luk_PymMeth.Properties.NullText = "";
			luk_PymMeth.Properties.ValueMember = "typeID";
			luk_PymMeth.Properties.DisplayMember = "name";
			luk_PymMeth.Properties.Columns["typeID"].Visible = false;
			luk_PymMeth.Properties.ShowHeader = false;
			salaryCurrData = GlobalData.CurrencyCodes;
			DataTable dataTable2 = new DataTable();
			dataTable2.Columns.Add("CurrCode", typeof(string));
			dataTable2.Columns.Add("CurrName", typeof(string));
			foreach (CurrencyCodesImpl salaryCurrDatum in salaryCurrData)
			{
				DataRow dataRow2 = dataTable2.NewRow();
				dataRow2["CurrCode"] = salaryCurrDatum.CurrCode;
				dataRow2["CurrName"] = salaryCurrDatum.CurrName;
				dataTable2.Rows.Add(dataRow2);
				dataTable2.AcceptChanges();
			}
			luk_salaryCurr.Properties.DataSource = dataTable2.Copy();
			luk_salaryCurr.Properties.NullText = "";
			luk_salaryCurr.Properties.ValueMember = "CurrCode";
			luk_salaryCurr.Properties.DisplayMember = "CurrCode";
			luk_salaryCurr.Properties.ShowHeader = false;
			date_signDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
			date_startDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
			date_endDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
			cal_basicSalary.SetFormatControl(Enums.DisplayFormatType.Amount);
			cal_ProbSalary.SetFormatControl(Enums.DisplayFormatType.Amount);
			cal_salary.SetFormatControl(Enums.DisplayFormatType.Amount);
			txt_contractCode.Properties.MaxLength = 50;
			txt_signer.Properties.MaxLength = 100;
			txt_note.Properties.MaxLength = 254;
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AndEnterKeyEvent(Control.ControlCollection Controls, KeyEventHandler handler)
	{
		foreach (Control Control in Controls)
		{
			if (Control is BaseEdit)
			{
				(Control as BaseEdit).KeyDown += handler;
			}
			else
			{
				AndEnterKeyEvent(Control.Controls, handler);
			}
		}
	}

	private void frm_ContractManagement_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Update || hasValueOnNewRow) && !isCancel)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
				{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					btnOK_Click(null, null);
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

	private void LoadContractType()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "EmpContract");
			DataTable dataTable = httpClientHelper.Get("LoadContractTypes");
			if (!dataTable.IsNotNull())
			{
				dataTable = WinFuntions.GetTableStructure("APZ_OCTY");
			}
			dataTable.Columns.Remove("descriptio");
			dataTable.Columns.Remove("empStatus");
			dataTable.Columns.Remove("locked");
			DataRow dataRow = dataTable.NewRow();
			dataRow["typeID"] = -1;
			dataRow["code"] = "";
			dataRow["name"] = "";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataView dataView = dataTable.AsDataView();
			dataView.Sort = "typeID asc";
			dataTable = dataView.ToTable();
			dataRow = dataTable.NewRow();
			dataRow["typeID"] = -2;
			dataRow["code"] = "Define New";
			dataRow["name"] = "";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			luk_contractType.Properties.DropDownRows = dataTable.Rows.Count;
			luk_contractType.Properties.DataSource = dataTable.Copy();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewRow()
	{
		try
		{
			gridView1.BeginInsertRow();
			gridView1.ClearSorting();
			DataTable dataTable = gridControl1.DataSource as DataTable;
			DataRow dataRow = dataTable.NewRow();
			dataRow["contractID"] = 0;
			dataRow["code"] = "";
			dataRow["empID"] = 0;
			dataRow["type"] = -1;
			dataRow["signerID"] = DBNull.Value;
			dataRow["signer"] = "";
			dataRow["signDate"] = DBNull.Value;
			dataRow["startDate"] = DBNull.Value;
			dataRow["endDate"] = DBNull.Value;
			dataRow["ProbSalary"] = DBNull.Value;
			dataRow["basicSalary"] = DBNull.Value;
			dataRow["salary"] = DBNull.Value;
			dataRow["salaryCurr"] = GlobalData.MainCurrency;
			dataRow["PymMeth"] = "5";
			dataRow["note"] = "";
			dataRow["BPLId"] = -1;
			dataRow["IsNewRow"] = "Y";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			gridControl1.DataSource = dataTable;
			gridView1.MakeRowVisible(gridView1.DataRowCount - 1);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gridView1.EndInsertRow();
		}
	}

	private async Task<string> SaveData()
	{
		_ = 1;
		try
		{
			DataTable dataTable = (gridControl1.DataSource as DataTable).Clone();
			DataRow dataRow = dataTable.NewRow();
			if (Function.ToString(gridView1.GetFocusedRowCellValue("contractID")) == "" || Function.ToString(gridView1.GetFocusedRowCellValue("contractID")) == "0")
			{
				dataRow["contractID"] = 0;
			}
			else
			{
				dataRow["contractID"] = Function.ParseInt(gridView1.GetFocusedRowCellValue("contractID"));
			}
			dataRow["code"] = Function.ToString(txt_contractCode.EditValue);
			dataRow["empID"] = Function.ParseInt(txt_EmpID.EditValue);
			dataRow["type"] = Function.ParseInt(luk_contractType.EditValue);
			dataRow["signerID"] = Function.ParseInt(txt_signerID.EditValue);
			dataRow["signer"] = Function.ToString(txt_signer.EditValue);
			DateTime? dateTime = Function.ParseDateTime(date_signDate.EditValue);
			if (dateTime.HasValue)
			{
				_ = dateTime.Value;
				dataRow["signDate"] = dateTime.Value;
			}
			else
			{
				dataRow["signDate"] = DBNull.Value;
			}
			dateTime = Function.ParseDateTime(date_startDate.EditValue);
			if (dateTime.HasValue)
			{
				_ = dateTime.Value;
				dataRow["startDate"] = dateTime.Value;
			}
			else
			{
				dataRow["startDate"] = DBNull.Value;
			}
			dateTime = Function.ParseDateTime(date_endDate.EditValue);
			if (dateTime.HasValue)
			{
				_ = dateTime.Value;
				dataRow["endDate"] = dateTime.Value;
			}
			else
			{
				dataRow["endDate"] = DBNull.Value;
			}
			dataRow["ProbSalary"] = Function.ParseDouble(cal_ProbSalary.EditValue);
			dataRow["basicSalary"] = Function.ParseDouble(cal_basicSalary.EditValue);
			dataRow["salary"] = Function.ParseDouble(cal_salary.EditValue);
			dataRow["salaryCurr"] = Function.ToString(luk_salaryCurr.EditValue);
			dataRow["PymMeth"] = Function.ToString(luk_PymMeth.EditValue);
			dataRow["note"] = Function.ToString(txt_note.EditValue);
			dataRow["BPLId"] = -1;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			if (dataTable.ExistsColumn("IsNewRow"))
			{
				dataTable.Columns.Remove("IsNewRow");
			}
			using (HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "EmpContract"))
			{
				string text = await helper.PostAsync("SaveContract", dataTable);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					hasValueOnNewRow = false;
					FormMode = Enums.FormViewModeType.View;
					await LoadData();
					ReloadContractDelegate((gridControl1.DataSource as DataTable).Copy());
					return "success";
				}
				WinCommons.Error(Enums.MessageCode.None, text);
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return "";
		}
	}

	private bool ValidateFields()
	{
		if (txt_contractCode.Text == "")
		{
			WinCommons.Error(Enums.MessageCode.ContractCodeCannotBeNull, "");
			return false;
		}
		if (Function.ToString(date_signDate.EditValue) == "")
		{
			WinCommons.Error(Enums.MessageCode.SignDateCannotBeNull, "");
			return false;
		}
		if (Function.ToString(date_startDate.EditValue) == "")
		{
			WinCommons.Error(Enums.MessageCode.StartDateIsRequired, "");
			return false;
		}
		if (!txt_signer.Text.Contains(',') && Function.ToString(txt_signer.Text) != "" && (txt_signerID.Text == "0" || txt_signerID.Text == ""))
		{
			WinCommons.Error(Enums.MessageCode.PleaseChooseSignerFromEmployeeList, "");
			return false;
		}
		if (date_startDate.Text != "" && date_endDate.Text != "" && Function.ParseDateTime(date_startDate.Text) > Function.ParseDateTime(date_endDate.Text))
		{
			WinCommons.Error(Enums.MessageCode.EndDateGreaterStartDate, "");
			return false;
		}
		return true;
	}

	private async void UpdateData()
	{
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.View:
				if (FormMode == Enums.FormViewModeType.View)
				{
					Close();
				}
				break;
			case Enums.FormViewModeType.Update:
			case Enums.FormViewModeType.Add:
				if (ValidateFields() && !(await SaveData() != "success"))
				{
				}
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btnOK_Click(object sender, EventArgs e)
	{
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.View:
				if (FormMode == Enums.FormViewModeType.View)
				{
					Close();
				}
				break;
			case Enums.FormViewModeType.Update:
			case Enums.FormViewModeType.Add:
				if (ValidateFields() && !(await SaveData() != "success"))
				{
				}
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Contract_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle != -2147483647)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	public async Task REFRESH()
	{
		try
		{
			if (gridView1.DataRowCount > 0)
			{
				DataTable obj = gridControl1.DataSource as DataTable;
				obj.AcceptChanges();
				if (obj.AsEnumerable().Any((DataRow s) => Function.ToString(s.Field<string>("IsNewRow")) == "Y"))
				{
					return;
				}
			}
			AddNewRow();
			int focusedRowHandle = gridView1.LocateByValue("IsNewRow", "Y");
			gridView1.FocusedRowHandle = focusedRowHandle;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridView1_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		if (!_isLoading && !isSorting && !isDelete && (FormMode == Enums.FormViewModeType.Update || hasValueOnNewRow))
		{
			DialogResult dialogResult = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
			if (dialogResult == DialogResult.Yes && dialogResult == DialogResult.Yes)
			{
				e.Allow = true;
				hasValueOnNewRow = false;
			}
			if (dialogResult == DialogResult.No)
			{
				e.Allow = false;
			}
		}
	}

	private void gv_Contract_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			if (!_isLoading && !isSorting && !isDelete && !(Function.ToString(gridView1.GetFocusedRowCellValue("empID")) == ""))
			{
				if (Function.ToString(gridView1.GetRowCellValue(e.PrevFocusedRowHandle, "IsNewRow")) == "Y")
				{
					gridView1.BeginUpdate();
					gridView1.DeleteRow(e.PrevFocusedRowHandle);
					gridView1.EndUpdate();
					gridControl1.RefreshDataSource();
				}
				FillControls();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_contractType_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				if (Function.ParseInt(e.NewValue) == -2)
				{
					frm_EmpContractTypes frm_EmpContractTypes2 = new frm_EmpContractTypes();
					frm_EmpContractTypes2.MdiParent = MdiParent;
					frm_EmpContractTypes2.childSend = parentGet;
					frm_EmpContractTypes2.Show();
					e.Cancel = true;
				}
				else
				{
					CheckSaveState();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void parentGet(int selectedIndex)
	{
		try
		{
			gridView1.BeginUpdate();
			LoadContractType();
			luk_contractType.EditValue = selectedIndex;
			gridView1.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CheckSaveState()
	{
		if (!_isLoading && !isFill)
		{
			if (FormMode == Enums.FormViewModeType.Add)
			{
				hasValueOnNewRow = true;
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
	}

	private void textEdit5_EditValueChanging(object sender, ChangingEventArgs e)
	{
		CheckSaveState();
	}

	private void SetAutoComplete()
	{
		switch (_autoCompleteFlag)
		{
		case AutoCompleteFlag.Auto:
			_isLoading = true;
			txt_signer.Text = autoName;
			txt_signerID.Text = Function.ToString(autoNumber);
			_isLoading = false;
			_autoCompleteFlag = AutoCompleteFlag.None;
			break;
		case AutoCompleteFlag.BackToOldText:
		{
			_isLoading = true;
			int key = Function.ParseInt(txt_signerID.Text);
			string text = dicOHEM[key];
			txt_signer.Text = text;
			_isLoading = false;
			_autoCompleteFlag = AutoCompleteFlag.None;
			break;
		}
		case AutoCompleteFlag.SetEmpty:
			_isLoading = true;
			txt_signerID.Text = "0";
			_isLoading = false;
			_autoCompleteFlag = AutoCompleteFlag.None;
			break;
		default:
			_autoCompleteFlag = AutoCompleteFlag.None;
			break;
		}
	}

	private void textEdit6_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			int num = Function.ParseInt(txt_signerID);
			string text = "";
			if (num != 0)
			{
				text = dicOHEM[num];
			}
			DataRow[] array = (from s in dtOHEM.AsEnumerable()
				where Function.ToString(s["lastName"]).ToLower().Contains(Function.ToString(e.NewValue).ToLower()) && Function.ToString(e.NewValue) != ""
				select s).ToArray();
			switch (array.Length)
			{
			case 0:
				if (num != 0)
				{
					if (Function.ToString(e.NewValue) == "")
					{
						_autoCompleteFlag = AutoCompleteFlag.SetEmpty;
						return;
					}
					if (text.Contains(Function.ToString(e.NewValue)))
					{
						_autoCompleteFlag = AutoCompleteFlag.BackToOldText;
						return;
					}
				}
				_autoCompleteFlag = AutoCompleteFlag.None;
				break;
			case 1:
				_autoCompleteFlag = AutoCompleteFlag.Auto;
				autoNumber = Function.ParseInt(array[0]["empID"]);
				autoName = dicOHEM[autoNumber];
				break;
			default:
				_autoCompleteFlag = AutoCompleteFlag.None;
				break;
			}
			CheckSaveState();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_note_EditValueChanging(object sender, ChangingEventArgs e)
	{
		CheckSaveState();
	}

	private void txt_PymMeth_EditValueChanging(object sender, ChangingEventArgs e)
	{
		CheckSaveState();
	}

	private void txt_salaryCurr_EditValueChanging(object sender, ChangingEventArgs e)
	{
		if (e.NewValue != null && e.NewValue.ToString().Length > 3)
		{
			e.Cancel = true;
		}
		CheckSaveState();
	}

	private void cal_salary_EditValueChanging(object sender, ChangingEventArgs e)
	{
		CheckSaveState();
	}

	private void cal_ProbSalary_EditValueChanging(object sender, ChangingEventArgs e)
	{
		CheckSaveState();
	}

	private void cal_basicSalary_EditValueChanging(object sender, ChangingEventArgs e)
	{
		CheckSaveState();
	}

	private void date_endDate_EditValueChanging(object sender, ChangingEventArgs e)
	{
		CheckSaveState();
	}

	private void date_startDate_EditValueChanging(object sender, ChangingEventArgs e)
	{
		CheckSaveState();
	}

	private void date_signDate_EditValueChanging(object sender, ChangingEventArgs e)
	{
		CheckSaveState();
	}

	private void FindByCFL()
	{
		try
		{
			string text = Function.ToString(txt_signer.Text);
			int num = Function.ParseInt(txt_signerID.Text);
			string value = "";
			if (num != 0)
			{
				value = dicOHEM[num];
			}
			frm_CFL frm_CFL2;
			if (text.Equals(value))
			{
				frm_CFL2 = new frm_CFL(Enums.TypeCFL.EmployeeListForSalesEmpSetup, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			}
			else
			{
				gridView1.SetFocusedRowCellValue("signerID", 0);
				txt_signerID.Text = "0";
				frm_CFL2 = new frm_CFL(Enums.TypeCFL.EmployeeListForSalesEmpSetup, false, text, "", Enums.ItemObjectType.NoItemType, null, "");
			}
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				int num2 = Function.ParseInt(frm_CFL2.ChooseItem["empID"]);
				fromCFL = true;
				txt_signer.Text = Function.ToString(frm_CFL2.ChooseItem["lastName"]) + ", " + Function.ToString(frm_CFL2.ChooseItem["firstName"]);
				txt_signerID.Text = Function.ToString(num2);
				txt_signer.Refresh();
				fromCFL = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_signer_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			isPressed = true;
			if (e.Button.Index == 0)
			{
				if (_autoCompleteFlag != AutoCompleteFlag.None)
				{
					SetAutoComplete();
					isPressed = false;
				}
				else
				{
					FindByCFL();
					isPressed = false;
				}
			}
			else
			{
				if (e.Button.Index != 1)
				{
					return;
				}
				if (_autoCompleteFlag != AutoCompleteFlag.None)
				{
					SetAutoComplete();
					isPressed = false;
					return;
				}
				int num = Function.ParseInt(txt_signerID.Text);
				if (!txt_signer.Text.Contains(',') && num == 0)
				{
					isPressed = false;
					return;
				}
				if (txt_signer.Text == dicOHEM[num])
				{
					this.DrillDown(ApzObjectTypes.oEmployeesInfo, ApzObjectTypes.oEmployeesInfo, Function.ToString(num));
				}
				isPressed = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_signer_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Tab && txt_signer.Text == "")
			{
				isTab = true;
				if (_autoCompleteFlag != AutoCompleteFlag.None)
				{
					SetAutoComplete();
					isTab = false;
				}
				else
				{
					FindByCFL();
					isTab = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_signer_EditValueChanged(object sender, EventArgs e)
	{
		if (!_isLoading && !fromCFL && !isTab && !isPressed)
		{
			_ = isLeave;
		}
	}

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		REFRESH();
	}

	private async void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			if (gridView1.DataRowCount <= 0)
			{
				return;
			}
			DialogResult dialogResult = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
			if (dialogResult == DialogResult.No || dialogResult != DialogResult.Yes)
			{
				return;
			}
			isDelete = true;
			if (Function.ToString(gridView1.GetFocusedRowCellValue("IsNewRow")) == "Y")
			{
				gridView1.BeginUpdate();
				gridView1.DeleteSelectedRows();
				gridView1.EndUpdate();
				gridControl1.RefreshDataSource();
				await LoadData();
				ReloadContractDelegate((gridControl1.DataSource as DataTable).Copy());
				FillControls();
				isDelete = false;
				return;
			}
			DataTable dataTable = (gridControl1.DataSource as DataTable).Clone();
			DataRow dataRow = dataTable.NewRow();
			dataRow["contractID"] = Function.ParseInt(gridView1.GetFocusedRowCellValue("contractID"));
			dataRow["empID"] = Function.ParseInt(txt_EmpID.EditValue);
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			using (HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "EmpContract"))
			{
				string text = await helper.PostAsync("DeleteContract", dataTable);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					hasValueOnNewRow = false;
					FormMode = Enums.FormViewModeType.View;
					await LoadData();
					ReloadContractDelegate((gridControl1.DataSource as DataTable).Copy());
					FillControls();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
			isDelete = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_PymMeth_EditValueChanging(object sender, ChangingEventArgs e)
	{
		CheckSaveState();
	}

	private void txt_signer_Leave(object sender, EventArgs e)
	{
		try
		{
			if (_isLoading || fromCFL || isTab || isPressed || isLeave)
			{
				return;
			}
			isLeave = true;
			if (_autoCompleteFlag != AutoCompleteFlag.None)
			{
				SetAutoComplete();
				isLeave = false;
				return;
			}
			string text = Function.ToString(txt_signer.Text);
			int num = Function.ParseInt(txt_signerID.Text);
			string value = "";
			if (num != 0)
			{
				value = dicOHEM[num];
			}
			if (!text.Equals(value) && text != "")
			{
				FindByCFL();
			}
			isLeave = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_salaryCurr_EditValueChanged(object sender, EventArgs e)
	{
		if (_isLoading)
		{
			return;
		}
		foreach (CurrencyCodesImpl salaryCurrDatum in salaryCurrData)
		{
			if (salaryCurrDatum.CurrCode == Function.ToString(luk_salaryCurr.EditValue))
			{
				string text = "0";
				if (salaryCurrDatum.Decimals != -1)
				{
					text = $"n{Function.ToString(salaryCurrDatum.Decimals)}";
					cal_basicSalary.Properties.DisplayFormat.FormatString = text;
					cal_ProbSalary.Properties.DisplayFormat.FormatString = text;
					cal_salary.Properties.DisplayFormat.FormatString = text;
				}
				else
				{
					text = $"n{GlobalData.QuantitiesFormat.NumberDecimalDigits}";
					cal_basicSalary.Properties.DisplayFormat.FormatString = text;
					cal_ProbSalary.Properties.DisplayFormat.FormatString = text;
					cal_salary.Properties.DisplayFormat.FormatString = text;
				}
				break;
			}
		}
	}

	private void gridView1_StartSorting(object sender, EventArgs e)
	{
		isSorting = true;
	}

	private void gridView1_EndSorting(object sender, EventArgs e)
	{
		if (FormMode != Enums.FormViewModeType.View)
		{
			gridView1.ClearSorting();
		}
		isSorting = false;
	}

	private void gridControl1_Click(object sender, EventArgs e)
	{
		fromUser = true;
	}

	private void gridView1_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
		{
			fromUser = true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_EmpContract));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.repositoryItemDateEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.group_ContractList = new DevExpress.XtraEditors.GroupControl();
		this.gridControl1 = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gridView1 = new Apzon.Controls.Grid.GridViewEx();
		this.gc_empID = new Apzon.Controls.Grid.GridColumnEx();
		this.contractID = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.typeName = new Apzon.Controls.Grid.GridColumnEx();
		this.code = new Apzon.Controls.Grid.GridColumnEx();
		this.signerID = new Apzon.Controls.Grid.GridColumnEx();
		this.signer = new Apzon.Controls.Grid.GridColumnEx();
		this.salary = new Apzon.Controls.Grid.GridColumnEx();
		this.signDate = new Apzon.Controls.Grid.GridColumnEx();
		this.startDate = new Apzon.Controls.Grid.GridColumnEx();
		this.endDate = new Apzon.Controls.Grid.GridColumnEx();
		this.ProbSalary = new Apzon.Controls.Grid.GridColumnEx();
		this.salaryCurr = new Apzon.Controls.Grid.GridColumnEx();
		this.basicSalary = new Apzon.Controls.Grid.GridColumnEx();
		this.note = new Apzon.Controls.Grid.GridColumnEx();
		this.firstName = new Apzon.Controls.Grid.GridColumnEx();
		this.middleName = new Apzon.Controls.Grid.GridColumnEx();
		this.lastName = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.group_Details = new DevExpress.XtraEditors.GroupControl();
		this.btnOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.tbl_Details = new System.Windows.Forms.TabControl();
		this.tab_GeneralDetails = new System.Windows.Forms.TabPage();
		this.txt_contractID = new Apzon.Controls.Editor.TextEditEx();
		this.txt_signerID = new Apzon.Controls.Editor.TextEditEx();
		this.luk_PymMeth = new DevExpress.XtraEditors.LookUpEdit();
		this.labelControl12 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl11 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl10 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl9 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl8 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl7 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl6 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.lbl_TenNhanVien = new DevExpress.XtraEditors.LabelControl();
		this.lbl_ID = new DevExpress.XtraEditors.LabelControl();
		this.txt_note = new Apzon.Controls.Editor.MemoEditEx();
		this.cal_salary = new Apzon.Controls.Editor.CalcEditEx();
		this.cal_ProbSalary = new Apzon.Controls.Editor.CalcEditEx();
		this.cal_basicSalary = new Apzon.Controls.Editor.CalcEditEx();
		this.date_endDate = new Apzon.Controls.Editor.DateEditEx();
		this.date_startDate = new Apzon.Controls.Editor.DateEditEx();
		this.date_signDate = new Apzon.Controls.Editor.DateEditEx();
		this.luk_contractType = new DevExpress.XtraEditors.LookUpEdit();
		this.txt_contractCode = new Apzon.Controls.Editor.TextEditEx();
		this.txt_EmpName = new Apzon.Controls.Editor.TextEditEx();
		this.txt_EmpID = new Apzon.Controls.Editor.TextEditEx();
		this.txt_signer = new Apzon.Controls.Editor.ButtonEditEx();
		this.luk_salaryCurr = new DevExpress.XtraEditors.LookUpEdit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.group_ContractList).BeginInit();
		this.group_ContractList.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.group_Details).BeginInit();
		this.group_Details.SuspendLayout();
		this.tbl_Details.SuspendLayout();
		this.tab_GeneralDetails.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_contractID.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_signerID.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PymMeth.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_note.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cal_salary.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cal_ProbSalary.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cal_basicSalary.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_endDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_endDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_startDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_startDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_signDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_signDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_contractType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_contractCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpID.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_signer.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_salaryCurr.Properties).BeginInit();
		base.SuspendLayout();
		this.repositoryItemDateEdit2.AutoHeight = false;
		this.repositoryItemDateEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit2.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit2.Name = "repositoryItemDateEdit2";
		this.group_ContractList.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.group_ContractList.Controls.Add(this.gridControl1);
		this.group_ContractList.Location = new System.Drawing.Point(0, 0);
		this.group_ContractList.Name = "group_ContractList";
		this.group_ContractList.Size = new System.Drawing.Size(290, 570);
		this.group_ContractList.TabIndex = 0;
		this.group_ContractList.Text = "Contract List";
		this.gridControl1.AllowPasteExcel = true;
		this.gridControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridControl1.ClipboardData = null;
		this.gridControl1.ContextMenuStrip = this.contextMenuStrip1;
		this.gridControl1.Location = new System.Drawing.Point(0, 23);
		this.gridControl1.MainView = this.gridView1;
		this.gridControl1.Name = "gridControl1";
		this.gridControl1.Size = new System.Drawing.Size(290, 542);
		this.gridControl1.TabIndex = 1;
		this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView1 });
		this.gridControl1.Click += new System.EventHandler(this.gridControl1_Click);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addRowToolStripMenuItem, this.removeRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(144, 48);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.addRowToolStripMenuItem.Text = "Add Row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gridView1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView1.ColumnKey = null;
		this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[20]
		{
			this.gc_empID, this.contractID, this.gridColumn4, this.typeName, this.code, this.signerID, this.signer, this.salary, this.signDate, this.startDate,
			this.endDate, this.ProbSalary, this.salaryCurr, this.basicSalary, this.note, this.firstName, this.middleName, this.lastName, this.gridColumn3, this.gridColumn1
		});
		this.gridView1.DisableFormSetting = false;
		this.gridView1.DisplayException = false;
		this.gridView1.EditColumn = null;
		this.gridView1.FieldCurrency = null;
		this.gridView1.FieldRate = null;
		this.gridView1.FormId = null;
		this.gridView1.GridControl = this.gridControl1;
		this.gridView1.IgnoreFilter = false;
		this.gridView1.IndexEditRow = -1;
		this.gridView1.IndicatorWidth = 30;
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
		this.gridView1.OptionsBehavior.Editable = false;
		this.gridView1.OptionsView.ColumnAutoWidth = false;
		this.gridView1.OptionsView.ShowGroupPanel = false;
		this.gridView1.UseBatchMod = false;
		this.gridView1.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Contract_CustomDrawRowIndicator);
		this.gridView1.StartSorting += new System.EventHandler(this.gridView1_StartSorting);
		this.gridView1.EndSorting += new System.EventHandler(this.gridView1_EndSorting);
		this.gridView1.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Contract_FocusedRowChanged);
		this.gridView1.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gridView1_BeforeLeaveRow);
		this.gridView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridView1_KeyDown);
		this.gc_empID.Caption = "gridColumn1";
		this.gc_empID.CurrencyDateField = null;
		this.gc_empID.CurrencyField = null;
		this.gc_empID.CurrencyFixed = null;
		this.gc_empID.CurrencyFixedRate = 0.0;
		this.gc_empID.CurrencyRateField = null;
		this.gc_empID.FieldName = "empID";
		this.gc_empID.Name = "gc_empID";
		this.gc_empID.PrimaryKey = false;
		this.gc_empID.UnitField = null;
		this.contractID.Caption = "contractID";
		this.contractID.CurrencyDateField = null;
		this.contractID.CurrencyField = null;
		this.contractID.CurrencyFixed = null;
		this.contractID.CurrencyFixedRate = 0.0;
		this.contractID.CurrencyRateField = null;
		this.contractID.FieldName = "contractID";
		this.contractID.Name = "contractID";
		this.contractID.PrimaryKey = false;
		this.contractID.UnitField = null;
		this.gridColumn4.Caption = "type";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "type";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.typeName.Caption = "Contract Type";
		this.typeName.CurrencyDateField = null;
		this.typeName.CurrencyField = null;
		this.typeName.CurrencyFixed = null;
		this.typeName.CurrencyFixedRate = 0.0;
		this.typeName.CurrencyRateField = null;
		this.typeName.FieldName = "typeName";
		this.typeName.Name = "typeName";
		this.typeName.OptionsColumn.AllowEdit = false;
		this.typeName.OptionsColumn.ReadOnly = true;
		this.typeName.PrimaryKey = false;
		this.typeName.UnitField = null;
		this.typeName.Visible = true;
		this.typeName.VisibleIndex = 1;
		this.typeName.Width = 90;
		this.code.Caption = "Contract Code.";
		this.code.CurrencyDateField = null;
		this.code.CurrencyField = null;
		this.code.CurrencyFixed = null;
		this.code.CurrencyFixedRate = 0.0;
		this.code.CurrencyRateField = null;
		this.code.FieldName = "code";
		this.code.Name = "code";
		this.code.OptionsColumn.AllowEdit = false;
		this.code.OptionsColumn.ReadOnly = true;
		this.code.PrimaryKey = true;
		this.code.UnitField = null;
		this.code.Visible = true;
		this.code.VisibleIndex = 0;
		this.code.Width = 104;
		this.signerID.Caption = "signerID";
		this.signerID.CurrencyDateField = null;
		this.signerID.CurrencyField = null;
		this.signerID.CurrencyFixed = null;
		this.signerID.CurrencyFixedRate = 0.0;
		this.signerID.CurrencyRateField = null;
		this.signerID.FieldName = "signerID";
		this.signerID.Name = "signerID";
		this.signerID.PrimaryKey = false;
		this.signerID.UnitField = null;
		this.signer.Caption = "Sign By";
		this.signer.CurrencyDateField = null;
		this.signer.CurrencyField = null;
		this.signer.CurrencyFixed = null;
		this.signer.CurrencyFixedRate = 0.0;
		this.signer.CurrencyRateField = null;
		this.signer.FieldName = "signer";
		this.signer.Name = "signer";
		this.signer.PrimaryKey = false;
		this.signer.UnitField = null;
		this.signer.Visible = true;
		this.signer.VisibleIndex = 5;
		this.salary.Caption = "Salary";
		this.salary.CurrencyDateField = null;
		this.salary.CurrencyField = null;
		this.salary.CurrencyFixed = null;
		this.salary.CurrencyFixedRate = 0.0;
		this.salary.CurrencyRateField = null;
		this.salary.FieldName = "salary";
		this.salary.Name = "salary";
		this.salary.PrimaryKey = false;
		this.salary.UnitField = null;
		this.salary.Visible = true;
		this.salary.VisibleIndex = 8;
		this.signDate.Caption = "Sign Date";
		this.signDate.ColumnEdit = this.repositoryItemDateEdit2;
		this.signDate.CurrencyDateField = null;
		this.signDate.CurrencyField = null;
		this.signDate.CurrencyFixed = null;
		this.signDate.CurrencyFixedRate = 0.0;
		this.signDate.CurrencyRateField = null;
		this.signDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.signDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.signDate.FieldName = "signDate";
		this.signDate.Name = "signDate";
		this.signDate.PrimaryKey = false;
		this.signDate.UnitField = null;
		this.signDate.Visible = true;
		this.signDate.VisibleIndex = 2;
		this.signDate.Width = 82;
		this.startDate.Caption = "Start Date";
		this.startDate.ColumnEdit = this.repositoryItemDateEdit2;
		this.startDate.CurrencyDateField = null;
		this.startDate.CurrencyField = null;
		this.startDate.CurrencyFixed = null;
		this.startDate.CurrencyFixedRate = 0.0;
		this.startDate.CurrencyRateField = null;
		this.startDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.startDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.startDate.FieldName = "startDate";
		this.startDate.Name = "startDate";
		this.startDate.PrimaryKey = false;
		this.startDate.UnitField = null;
		this.startDate.Visible = true;
		this.startDate.VisibleIndex = 3;
		this.startDate.Width = 103;
		this.endDate.Caption = "End Date";
		this.endDate.ColumnEdit = this.repositoryItemDateEdit2;
		this.endDate.CurrencyDateField = null;
		this.endDate.CurrencyField = null;
		this.endDate.CurrencyFixed = null;
		this.endDate.CurrencyFixedRate = 0.0;
		this.endDate.CurrencyRateField = null;
		this.endDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.endDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.endDate.FieldName = "endDate";
		this.endDate.Name = "endDate";
		this.endDate.PrimaryKey = false;
		this.endDate.UnitField = null;
		this.endDate.Visible = true;
		this.endDate.VisibleIndex = 4;
		this.endDate.Width = 107;
		this.ProbSalary.Caption = "Probationary Salary";
		this.ProbSalary.CurrencyDateField = null;
		this.ProbSalary.CurrencyField = null;
		this.ProbSalary.CurrencyFixed = null;
		this.ProbSalary.CurrencyFixedRate = 0.0;
		this.ProbSalary.CurrencyRateField = null;
		this.ProbSalary.FieldName = "ProbSalary";
		this.ProbSalary.Name = "ProbSalary";
		this.ProbSalary.PrimaryKey = false;
		this.ProbSalary.UnitField = null;
		this.ProbSalary.Visible = true;
		this.ProbSalary.VisibleIndex = 7;
		this.ProbSalary.Width = 126;
		this.salaryCurr.Caption = "Salary Currency";
		this.salaryCurr.CurrencyDateField = null;
		this.salaryCurr.CurrencyField = null;
		this.salaryCurr.CurrencyFixed = null;
		this.salaryCurr.CurrencyFixedRate = 0.0;
		this.salaryCurr.CurrencyRateField = null;
		this.salaryCurr.FieldName = "salaryCurr";
		this.salaryCurr.Name = "salaryCurr";
		this.salaryCurr.PrimaryKey = false;
		this.salaryCurr.UnitField = null;
		this.salaryCurr.Visible = true;
		this.salaryCurr.VisibleIndex = 9;
		this.salaryCurr.Width = 89;
		this.basicSalary.Caption = "Basic Salary";
		this.basicSalary.CurrencyDateField = null;
		this.basicSalary.CurrencyField = null;
		this.basicSalary.CurrencyFixed = null;
		this.basicSalary.CurrencyFixedRate = 0.0;
		this.basicSalary.CurrencyRateField = null;
		this.basicSalary.FieldName = "basicSalary";
		this.basicSalary.Name = "basicSalary";
		this.basicSalary.PrimaryKey = false;
		this.basicSalary.UnitField = null;
		this.basicSalary.Visible = true;
		this.basicSalary.VisibleIndex = 6;
		this.note.Caption = "Note";
		this.note.CurrencyDateField = null;
		this.note.CurrencyField = null;
		this.note.CurrencyFixed = null;
		this.note.CurrencyFixedRate = 0.0;
		this.note.CurrencyRateField = null;
		this.note.FieldName = "note";
		this.note.Name = "note";
		this.note.PrimaryKey = false;
		this.note.UnitField = null;
		this.note.Visible = true;
		this.note.VisibleIndex = 10;
		this.note.Width = 178;
		this.firstName.Caption = "gridColumn1";
		this.firstName.CurrencyDateField = null;
		this.firstName.CurrencyField = null;
		this.firstName.CurrencyFixed = null;
		this.firstName.CurrencyFixedRate = 0.0;
		this.firstName.CurrencyRateField = null;
		this.firstName.FieldName = "firstName";
		this.firstName.Name = "firstName";
		this.firstName.PrimaryKey = false;
		this.firstName.UnitField = null;
		this.middleName.Caption = "gridColumn2";
		this.middleName.CurrencyDateField = null;
		this.middleName.CurrencyField = null;
		this.middleName.CurrencyFixed = null;
		this.middleName.CurrencyFixedRate = 0.0;
		this.middleName.CurrencyRateField = null;
		this.middleName.FieldName = "middleName";
		this.middleName.Name = "middleName";
		this.middleName.PrimaryKey = false;
		this.middleName.UnitField = null;
		this.lastName.Caption = "gridColumn3";
		this.lastName.CurrencyDateField = null;
		this.lastName.CurrencyField = null;
		this.lastName.CurrencyFixed = null;
		this.lastName.CurrencyFixedRate = 0.0;
		this.lastName.CurrencyRateField = null;
		this.lastName.FieldName = "lastName";
		this.lastName.Name = "lastName";
		this.lastName.PrimaryKey = false;
		this.lastName.UnitField = null;
		this.gridColumn3.Caption = "BPLId";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "BPLId";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn1.Caption = "gridColumn1";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "IsNewRow";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.group_Details.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.group_Details.Controls.Add(this.btnOK);
		this.group_Details.Controls.Add(this.btCancel);
		this.group_Details.Controls.Add(this.tbl_Details);
		this.group_Details.Location = new System.Drawing.Point(296, 0);
		this.group_Details.Name = "group_Details";
		this.group_Details.Size = new System.Drawing.Size(649, 570);
		this.group_Details.TabIndex = 1;
		this.group_Details.Text = "Details";
		this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOK.EditControl = null;
		this.btnOK.EditValue = "OK";
		this.btnOK.FieldName = null;
		this.btnOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOK.ImageOptions.Image");
		this.btnOK.IsSearchControl = false;
		this.btnOK.Location = new System.Drawing.Point(5, 542);
		this.btnOK.Name = "btnOK";
		this.btnOK.ReadOnly = false;
		this.btnOK.SettingInVisible = false;
		this.btnOK.SettingReadonly = false;
		this.btnOK.Size = new System.Drawing.Size(75, 23);
		this.btnOK.TabIndex = 103;
		this.btnOK.Text = "OK";
		this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(83, 542);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 104;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.tbl_Details.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.tbl_Details.Controls.Add(this.tab_GeneralDetails);
		this.tbl_Details.Location = new System.Drawing.Point(2, 20);
		this.tbl_Details.Name = "tbl_Details";
		this.tbl_Details.SelectedIndex = 0;
		this.tbl_Details.Size = new System.Drawing.Size(645, 520);
		this.tbl_Details.TabIndex = 0;
		this.tab_GeneralDetails.Controls.Add(this.txt_contractID);
		this.tab_GeneralDetails.Controls.Add(this.txt_signerID);
		this.tab_GeneralDetails.Controls.Add(this.luk_PymMeth);
		this.tab_GeneralDetails.Controls.Add(this.labelControl12);
		this.tab_GeneralDetails.Controls.Add(this.labelControl11);
		this.tab_GeneralDetails.Controls.Add(this.labelControl10);
		this.tab_GeneralDetails.Controls.Add(this.labelControl9);
		this.tab_GeneralDetails.Controls.Add(this.labelControl8);
		this.tab_GeneralDetails.Controls.Add(this.labelControl7);
		this.tab_GeneralDetails.Controls.Add(this.labelControl6);
		this.tab_GeneralDetails.Controls.Add(this.labelControl5);
		this.tab_GeneralDetails.Controls.Add(this.labelControl4);
		this.tab_GeneralDetails.Controls.Add(this.labelControl3);
		this.tab_GeneralDetails.Controls.Add(this.labelControl2);
		this.tab_GeneralDetails.Controls.Add(this.labelControl1);
		this.tab_GeneralDetails.Controls.Add(this.lbl_TenNhanVien);
		this.tab_GeneralDetails.Controls.Add(this.lbl_ID);
		this.tab_GeneralDetails.Controls.Add(this.txt_note);
		this.tab_GeneralDetails.Controls.Add(this.cal_salary);
		this.tab_GeneralDetails.Controls.Add(this.cal_ProbSalary);
		this.tab_GeneralDetails.Controls.Add(this.cal_basicSalary);
		this.tab_GeneralDetails.Controls.Add(this.date_endDate);
		this.tab_GeneralDetails.Controls.Add(this.date_startDate);
		this.tab_GeneralDetails.Controls.Add(this.date_signDate);
		this.tab_GeneralDetails.Controls.Add(this.luk_contractType);
		this.tab_GeneralDetails.Controls.Add(this.txt_contractCode);
		this.tab_GeneralDetails.Controls.Add(this.txt_EmpName);
		this.tab_GeneralDetails.Controls.Add(this.txt_EmpID);
		this.tab_GeneralDetails.Controls.Add(this.txt_signer);
		this.tab_GeneralDetails.Controls.Add(this.luk_salaryCurr);
		this.tab_GeneralDetails.Location = new System.Drawing.Point(4, 22);
		this.tab_GeneralDetails.Name = "tab_GeneralDetails";
		this.tab_GeneralDetails.Padding = new System.Windows.Forms.Padding(3);
		this.tab_GeneralDetails.Size = new System.Drawing.Size(637, 494);
		this.tab_GeneralDetails.TabIndex = 0;
		this.tab_GeneralDetails.Text = "General";
		this.tab_GeneralDetails.UseVisualStyleBackColor = true;
		this.txt_contractID.EditControl = null;
		this.txt_contractID.FieldName = null;
		this.txt_contractID.IsSearchControl = false;
		this.txt_contractID.Location = new System.Drawing.Point(442, 22);
		this.txt_contractID.Name = "txt_contractID";
		this.txt_contractID.SettingInVisible = false;
		this.txt_contractID.SettingReadonly = false;
		this.txt_contractID.Size = new System.Drawing.Size(57, 20);
		this.txt_contractID.TabIndex = 103;
		this.txt_contractID.TabStop = false;
		this.txt_contractID.Visible = false;
		this.txt_signerID.EditControl = null;
		this.txt_signerID.FieldName = null;
		this.txt_signerID.IsSearchControl = false;
		this.txt_signerID.Location = new System.Drawing.Point(442, 133);
		this.txt_signerID.Name = "txt_signerID";
		this.txt_signerID.SettingInVisible = false;
		this.txt_signerID.SettingReadonly = false;
		this.txt_signerID.Size = new System.Drawing.Size(57, 20);
		this.txt_signerID.TabIndex = 102;
		this.txt_signerID.TabStop = false;
		this.txt_signerID.Visible = false;
		this.luk_PymMeth.Location = new System.Drawing.Point(112, 340);
		this.luk_PymMeth.Name = "luk_PymMeth";
		this.luk_PymMeth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_PymMeth.Properties.DisplayMember = "name";
		this.luk_PymMeth.Properties.DropDownRows = 3;
		this.luk_PymMeth.Properties.NullText = "";
		this.luk_PymMeth.Properties.ValueMember = "typeID";
		this.luk_PymMeth.Size = new System.Drawing.Size(306, 20);
		this.luk_PymMeth.TabIndex = 13;
		this.luk_PymMeth.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_PymMeth_EditValueChanging);
		this.labelControl12.Location = new System.Drawing.Point(6, 369);
		this.labelControl12.Name = "labelControl12";
		this.labelControl12.Size = new System.Drawing.Size(27, 13);
		this.labelControl12.TabIndex = 100;
		this.labelControl12.Text = "Note:";
		this.labelControl11.Location = new System.Drawing.Point(6, 343);
		this.labelControl11.Name = "labelControl11";
		this.labelControl11.Size = new System.Drawing.Size(85, 13);
		this.labelControl11.TabIndex = 99;
		this.labelControl11.Text = "Payment Method:";
		this.labelControl10.Location = new System.Drawing.Point(6, 317);
		this.labelControl10.Name = "labelControl10";
		this.labelControl10.Size = new System.Drawing.Size(81, 13);
		this.labelControl10.TabIndex = 98;
		this.labelControl10.Text = "Salary Currency:";
		this.labelControl9.Location = new System.Drawing.Point(6, 291);
		this.labelControl9.Name = "labelControl9";
		this.labelControl9.Size = new System.Drawing.Size(34, 13);
		this.labelControl9.TabIndex = 97;
		this.labelControl9.Text = "Salary:";
		this.labelControl8.Location = new System.Drawing.Point(6, 265);
		this.labelControl8.Name = "labelControl8";
		this.labelControl8.Size = new System.Drawing.Size(99, 13);
		this.labelControl8.TabIndex = 96;
		this.labelControl8.Text = "Probationary Salary:";
		this.labelControl7.Location = new System.Drawing.Point(6, 239);
		this.labelControl7.Name = "labelControl7";
		this.labelControl7.Size = new System.Drawing.Size(61, 13);
		this.labelControl7.TabIndex = 95;
		this.labelControl7.Text = "Basic Salary:";
		this.labelControl6.Location = new System.Drawing.Point(6, 212);
		this.labelControl6.Name = "labelControl6";
		this.labelControl6.Size = new System.Drawing.Size(48, 13);
		this.labelControl6.TabIndex = 94;
		this.labelControl6.Text = "End Date:";
		this.labelControl5.Location = new System.Drawing.Point(6, 186);
		this.labelControl5.Name = "labelControl5";
		this.labelControl5.Size = new System.Drawing.Size(54, 13);
		this.labelControl5.TabIndex = 93;
		this.labelControl5.Text = "Start Date:";
		this.labelControl4.Location = new System.Drawing.Point(6, 160);
		this.labelControl4.Name = "labelControl4";
		this.labelControl4.Size = new System.Drawing.Size(50, 13);
		this.labelControl4.TabIndex = 92;
		this.labelControl4.Text = "Sign Date:";
		this.labelControl3.Location = new System.Drawing.Point(6, 133);
		this.labelControl3.Name = "labelControl3";
		this.labelControl3.Size = new System.Drawing.Size(51, 13);
		this.labelControl3.TabIndex = 91;
		this.labelControl3.Text = "Signed By:";
		this.labelControl2.Location = new System.Drawing.Point(6, 107);
		this.labelControl2.Name = "labelControl2";
		this.labelControl2.Size = new System.Drawing.Size(74, 13);
		this.labelControl2.TabIndex = 90;
		this.labelControl2.Text = "Contract Code:";
		this.labelControl1.Location = new System.Drawing.Point(6, 81);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(73, 13);
		this.labelControl1.TabIndex = 89;
		this.labelControl1.Text = "Contract Type:";
		this.lbl_TenNhanVien.Location = new System.Drawing.Point(6, 55);
		this.lbl_TenNhanVien.Name = "lbl_TenNhanVien";
		this.lbl_TenNhanVien.Size = new System.Drawing.Size(80, 13);
		this.lbl_TenNhanVien.TabIndex = 88;
		this.lbl_TenNhanVien.Text = "Employee Name:";
		this.lbl_ID.Location = new System.Drawing.Point(6, 29);
		this.lbl_ID.Name = "lbl_ID";
		this.lbl_ID.Size = new System.Drawing.Size(70, 13);
		this.lbl_ID.TabIndex = 87;
		this.lbl_ID.Text = "Employee No.:";
		this.txt_note.EditControl = null;
		this.txt_note.FieldName = null;
		this.txt_note.IsSearchControl = false;
		this.txt_note.Location = new System.Drawing.Point(112, 367);
		this.txt_note.Name = "txt_note";
		this.txt_note.SettingInVisible = false;
		this.txt_note.SettingReadonly = false;
		this.txt_note.Size = new System.Drawing.Size(519, 97);
		this.txt_note.TabIndex = 14;
		this.txt_note.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_note_EditValueChanging);
		this.cal_salary.EditControl = null;
		this.cal_salary.FieldName = null;
		this.cal_salary.IsSearchControl = false;
		this.cal_salary.Location = new System.Drawing.Point(112, 288);
		this.cal_salary.Name = "cal_salary";
		this.cal_salary.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cal_salary.Properties.IsSearchControl = false;
		this.cal_salary.SettingInVisible = false;
		this.cal_salary.SettingReadonly = false;
		this.cal_salary.Size = new System.Drawing.Size(147, 20);
		this.cal_salary.TabIndex = 11;
		this.cal_salary.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.cal_salary_EditValueChanging);
		this.cal_ProbSalary.EditControl = null;
		this.cal_ProbSalary.FieldName = null;
		this.cal_ProbSalary.IsSearchControl = false;
		this.cal_ProbSalary.Location = new System.Drawing.Point(112, 262);
		this.cal_ProbSalary.Name = "cal_ProbSalary";
		this.cal_ProbSalary.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cal_ProbSalary.Properties.IsSearchControl = false;
		this.cal_ProbSalary.SettingInVisible = false;
		this.cal_ProbSalary.SettingReadonly = false;
		this.cal_ProbSalary.Size = new System.Drawing.Size(147, 20);
		this.cal_ProbSalary.TabIndex = 10;
		this.cal_ProbSalary.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.cal_ProbSalary_EditValueChanging);
		this.cal_basicSalary.EditControl = null;
		this.cal_basicSalary.FieldName = null;
		this.cal_basicSalary.IsSearchControl = false;
		this.cal_basicSalary.Location = new System.Drawing.Point(112, 236);
		this.cal_basicSalary.Name = "cal_basicSalary";
		this.cal_basicSalary.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cal_basicSalary.Properties.IsSearchControl = false;
		this.cal_basicSalary.SettingInVisible = false;
		this.cal_basicSalary.SettingReadonly = false;
		this.cal_basicSalary.Size = new System.Drawing.Size(147, 20);
		this.cal_basicSalary.TabIndex = 9;
		this.cal_basicSalary.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.cal_basicSalary_EditValueChanging);
		this.date_endDate.EditControl = null;
		this.date_endDate.EditValue = null;
		this.date_endDate.FieldName = null;
		this.date_endDate.Location = new System.Drawing.Point(112, 209);
		this.date_endDate.Name = "date_endDate";
		this.date_endDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_endDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_endDate.Properties.IsSearchControl = false;
		this.date_endDate.SettingInVisible = false;
		this.date_endDate.SettingReadonly = false;
		this.date_endDate.Size = new System.Drawing.Size(306, 20);
		this.date_endDate.TabIndex = 8;
		this.date_endDate.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.date_endDate_EditValueChanging);
		this.date_startDate.EditControl = null;
		this.date_startDate.EditValue = null;
		this.date_startDate.FieldName = null;
		this.date_startDate.Location = new System.Drawing.Point(112, 183);
		this.date_startDate.Name = "date_startDate";
		this.date_startDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_startDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_startDate.Properties.IsSearchControl = false;
		this.date_startDate.SettingInVisible = false;
		this.date_startDate.SettingReadonly = false;
		this.date_startDate.Size = new System.Drawing.Size(306, 20);
		this.date_startDate.TabIndex = 7;
		this.date_startDate.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.date_startDate_EditValueChanging);
		this.date_signDate.EditControl = null;
		this.date_signDate.EditValue = null;
		this.date_signDate.FieldName = null;
		this.date_signDate.Location = new System.Drawing.Point(112, 157);
		this.date_signDate.Name = "date_signDate";
		this.date_signDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_signDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_signDate.Properties.IsSearchControl = false;
		this.date_signDate.SettingInVisible = false;
		this.date_signDate.SettingReadonly = false;
		this.date_signDate.Size = new System.Drawing.Size(306, 20);
		this.date_signDate.TabIndex = 6;
		this.date_signDate.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.date_signDate_EditValueChanging);
		this.luk_contractType.Location = new System.Drawing.Point(112, 78);
		this.luk_contractType.Name = "luk_contractType";
		this.luk_contractType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_contractType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[3]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("typeID", "\"\""),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("name", "Name")
		});
		this.luk_contractType.Properties.DisplayMember = "name";
		this.luk_contractType.Properties.DropDownRows = 3;
		this.luk_contractType.Properties.NullText = "";
		this.luk_contractType.Properties.ValueMember = "typeID";
		this.luk_contractType.Size = new System.Drawing.Size(306, 20);
		this.luk_contractType.TabIndex = 3;
		this.luk_contractType.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_contractType_EditValueChanging);
		this.txt_contractCode.EditControl = null;
		this.txt_contractCode.FieldName = null;
		this.txt_contractCode.IsSearchControl = false;
		this.txt_contractCode.Location = new System.Drawing.Point(112, 104);
		this.txt_contractCode.Name = "txt_contractCode";
		this.txt_contractCode.SettingInVisible = false;
		this.txt_contractCode.SettingReadonly = false;
		this.txt_contractCode.Size = new System.Drawing.Size(306, 20);
		this.txt_contractCode.TabIndex = 4;
		this.txt_contractCode.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.textEdit5_EditValueChanging);
		this.txt_EmpName.EditControl = null;
		this.txt_EmpName.Enabled = false;
		this.txt_EmpName.FieldName = null;
		this.txt_EmpName.IsSearchControl = false;
		this.txt_EmpName.Location = new System.Drawing.Point(112, 52);
		this.txt_EmpName.Name = "txt_EmpName";
		this.txt_EmpName.SettingInVisible = false;
		this.txt_EmpName.SettingReadonly = false;
		this.txt_EmpName.Size = new System.Drawing.Size(306, 20);
		this.txt_EmpName.TabIndex = 1;
		this.txt_EmpID.EditControl = null;
		this.txt_EmpID.Enabled = false;
		this.txt_EmpID.FieldName = null;
		this.txt_EmpID.IsSearchControl = false;
		this.txt_EmpID.Location = new System.Drawing.Point(112, 26);
		this.txt_EmpID.Name = "txt_EmpID";
		this.txt_EmpID.SettingInVisible = false;
		this.txt_EmpID.SettingReadonly = false;
		this.txt_EmpID.Size = new System.Drawing.Size(48, 20);
		this.txt_EmpID.TabIndex = 0;
		this.txt_signer.EditControl = null;
		this.txt_signer.FieldName = null;
		this.txt_signer.IsSearchControl = false;
		this.txt_signer.Location = new System.Drawing.Point(112, 130);
		this.txt_signer.Name = "txt_signer";
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.txt_signer.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_signer.SettingInVisible = false;
		this.txt_signer.SettingReadonly = false;
		this.txt_signer.Size = new System.Drawing.Size(306, 20);
		this.txt_signer.TabIndex = 5;
		this.txt_signer.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_signer_ButtonPressed);
		this.txt_signer.EditValueChanged += new System.EventHandler(this.txt_signer_EditValueChanged);
		this.txt_signer.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.textEdit6_EditValueChanging);
		this.txt_signer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_signer_KeyDown);
		this.txt_signer.Leave += new System.EventHandler(this.txt_signer_Leave);
		this.luk_salaryCurr.Location = new System.Drawing.Point(112, 314);
		this.luk_salaryCurr.Name = "luk_salaryCurr";
		this.luk_salaryCurr.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_salaryCurr.Properties.NullText = "";
		this.luk_salaryCurr.Size = new System.Drawing.Size(63, 20);
		this.luk_salaryCurr.TabIndex = 12;
		this.luk_salaryCurr.EditValueChanged += new System.EventHandler(this.luk_salaryCurr_EditValueChanged);
		this.luk_salaryCurr.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_salaryCurr_EditValueChanging);
		base.AcceptButton = this.btnOK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(943, 570);
		base.Controls.Add(this.group_Details);
		base.Controls.Add(this.group_ContractList);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_EmpContract";
		base.ShowInTaskbar = false;
		this.Text = "Contract Management";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_ContractManagement_FormClosing);
		base.Load += new System.EventHandler(this.frm_ContractManagement_Load);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.group_ContractList).EndInit();
		this.group_ContractList.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridControl1).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.group_Details).EndInit();
		this.group_Details.ResumeLayout(false);
		this.tbl_Details.ResumeLayout(false);
		this.tab_GeneralDetails.ResumeLayout(false);
		this.tab_GeneralDetails.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_contractID.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_signerID.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PymMeth.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_note.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cal_salary.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cal_ProbSalary.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cal_basicSalary.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_endDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_endDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_startDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_startDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_signDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_signDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_contractType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_contractCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpID.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_signer.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_salaryCurr.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
