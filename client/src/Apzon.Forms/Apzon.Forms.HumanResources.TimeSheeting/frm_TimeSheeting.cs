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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Export;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;

namespace Apzon.Forms.HumanResources.TimeSheeting;

public class frm_TimeSheeting : XtraFormEx
{
	public string FormController = "TimeSheeting";

	public string formMenuCode = "frm_TimeSheeting";

	public bool isCancel;

	public bool isKeyDown;

	public bool isCheckingItem;

	public bool changeCell;

	public bool _isLoading;

	public DXPopupMenu _menu;

	public bool hasValueOnNewRow;

	public RepositoryItemTextEdit workDay_Rep = new RepositoryItemTextEdit();

	public readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public Dictionary<string, int> _dicWorkColor = new Dictionary<string, int>();

	public Dictionary<string, string> _dicWorkDay = new Dictionary<string, string>();

	public Dictionary<string, double> _dicWorkRate = new Dictionary<string, double>();

	public Dictionary<Tuple<int, string>, int> dicErrorWorkCode = new Dictionary<Tuple<int, string>, int>();

	public DataTable dtOHST;

	public DataTable dtOUBR;

	public DataTable dtOUDP;

	public DataTable dtAPZ_OHWP;

	public DataTable dtAPZ_OHWC;

	public DataTable dtSetting;

	public string tableName = "APZ_OTSL";

	public string overtimeType = "";

	public DataTable dtAPZ_OHOT;

	public DataTable dtAPZ_OHWT;

	public Dictionary<string, string> _dicWorkType = new Dictionary<string, string>();

	private int selectedBranch = -1;

	private string selectedPeriod = "";

	public double hourPerDay = 8.0;

	public int BaseEntry;

	public int Branch;

	public string Period;

	private IContainer components;

	public GridControlEx gridControl1;

	public GridViewEx gridView1;

	public GridColumnEx EmpId;

	public GridColumnEx EmpName;

	public GridColumnEx TotalBefore;

	public GridColumnEx WorkQuota;

	public GridColumnEx DocStatus;

	public GridColumnEx Extra;

	public GridColumnEx WorkTotal;

	public SimpleButtonEx btCancel;

	public SimpleButtonEx btnOK;

	public TextEditEx txt_EmpId;

	public TextEditEx txt_EmpName;

	public LookUpEditEx luk_Period;

	public CheckedComboBoxEditEx ccb_Department;

	public CheckedComboBoxEditEx ccb_Status;

	public SimpleButtonEx btn_Search;

	public LookUpEditEx luk_Branch;

	public CheckedComboBoxEditEx ccb_WorkDay;

	public SimpleButtonEx btnImport;

	public GridColumnEx gcPeriod;

	public GridControlEx grd_Export;

	public GridViewEx gv_Export;

	public SimpleButtonEx btnExport;

	public LookUpEditEx luk_OvertimeType;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private LabelLine labelLine5;

	private LabelLine labelLine6;

	private LabelLine lbl_OT;

	private GridColumnEx gcWorkSum;

	private GridColumnEx gcAbsentSum;

	private GridColumnEx gcLeaveSum;

	private LabelLine labelLine7;

	private RepositoryItemButtonEditCFL rep_DrillDownEmp;

	public SimpleButtonEx btn_CancelTimeSheet;

	protected SimpleButtonEx btn_ClosePeriod;

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
		case Enums.FormViewModeType.Update:
			btnOK.Text = Apzon.Commons.Properties.Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.View:
			btnOK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_TimeSheeting()
	{
		SetMenuCode();
		InitializeComponent();
		SetupFormControls();
		FormId = "oTimeSheeting";
		gridView1.FormId = FormId;
		RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = rep_DrillDownEmp;
		repositoryItemButtonEditCFL.DrillDownMethodWithGridView = (Action<string, GridView>)Delegate.Combine(repositoryItemButtonEditCFL.DrillDownMethodWithGridView, (Action<string, GridView>)((string input, GridView view) =>
		{
			this.DrillDown(ApzObjectTypes.oEmployeesInfo, Function.ToString(view.GetFocusedRowCellValue("EmpId")));
		}));
		rep_DrillDownEmp.TextEditStyle = TextEditStyles.DisableTextEditor;
		OnLoadAsync += frm_TimeSheeting_Load;
		gridView1.ChangeLogType = Enums.ChangeLog.clTimeSheeting;
		gridView1.IsApzTable = true;
	}

	public virtual void SetMenuCode()
	{
	}

	protected override void NoAuthSetControlSearch()
	{
		btn_Search.Enabled = true;
		btnExport.Enabled = true;
		ccb_Status.Enabled = true;
		ccb_Department.Enabled = true;
		txt_EmpName.Enabled = true;
		txt_EmpId.Enabled = true;
		luk_Branch.Enabled = true;
		luk_OvertimeType.Enabled = true;
		luk_Period.Enabled = true;
		ccb_WorkDay.Enabled = true;
	}

	public virtual DataTable UnPivotData(out string mess)
	{
		mess = "";
		try
		{
			DataTable dataSourceTable = gridControl1.DataSourceTable;
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Datasource has " + dataSourceTable.Rows.Count);
			if (dataSourceTable.Rows.Count == 0)
			{
				mess = "Data is empty";
			}
			if (_dicWorkDay.Keys.Count <= 0)
			{
				return null;
			}
			DataTable dataTable = dataSourceTable.Clone();
			dataTable.Columns.Add("WorkDay", typeof(DateTime));
			dataTable.Columns.Add("WorkCode", typeof(string));
			for (int i = 0; i < dataSourceTable.Rows.Count; i++)
			{
				DataRow dataRow = dataSourceTable.Rows[i];
				dataRow["UserSign2"] = GlobalData.UserID;
				dataRow["UpdateDate"] = DateTime.Today;
				double num = Function.ParseDouble(dataRow["LeaveSum"]);
				double num2 = Function.ParseDouble(dataRow["LeaveTotal"]);
				if (num > num2)
				{
					string text = Function.ToString(dataRow["EmpName"]);
					WinCommons.Error(Enums.MessageCode.LeaveDayHasExeededLimit, "", Function.ToString(num), text, Function.ToString(num2));
					return new DataTable();
				}
				double num3 = Function.ParseDouble(dataRow["UsedOT"]);
				double num4 = Function.ParseDouble(dataRow["totalOT"]);
				if (num3 > num4)
				{
					string text2 = Function.ToString(dataRow["EmpName"]);
					WinCommons.Error(Enums.MessageCode.OTHourHasExeededLimit, "", Function.ToString(num3 * hourPerDay), text2, Function.ToString(num4));
					return new DataTable();
				}
				bool flag = false;
				foreach (string key in _dicWorkDay.Keys)
				{
					if (!string.IsNullOrEmpty(Function.ToString(dataSourceTable.Rows[i][key])))
					{
						flag = true;
						DataRow dataRow2 = dataTable.NewRow();
						dataRow2.ItemArray = dataSourceTable.Rows[i].ItemArray;
						dataRow2["WorkDay"] = Function.ParseDateTimes(key).Date;
						dataRow2["WorkCode"] = dataSourceTable.Rows[i][key];
						dataTable.Rows.Add(dataRow2);
						dataTable.AcceptChanges();
					}
				}
				if (!flag)
				{
					DataRow dataRow3 = dataTable.NewRow();
					dataRow3.ItemArray = dataSourceTable.Rows[i].ItemArray;
					dataTable.Rows.Add(dataRow3);
					dataTable.AcceptChanges();
				}
			}
			foreach (string key2 in _dicWorkDay.Keys)
			{
				dataTable.Columns.Remove(key2);
			}
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Datasource after pivot has " + dataTable.Rows.Count);
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task UpdateData()
	{
		try
		{
			if (dicErrorWorkCode.Count > 0)
			{
				WinCommons.Error(Enums.MessageCode.InvalidWorkCodeInRow, "", Function.ToString(dicErrorWorkCode.First().Key.Item1 + 1), dicErrorWorkCode.First().Key.Item2);
			}
			else
			{
				if (gridControl1.DataSourceTable.EmptyData())
				{
					return;
				}
				DataTable dataTable = UnPivotData(out var _);
				if (!dataTable.EmptyData())
				{
					WinCommons._OpenProcessing();
					string text = await SaveData(dataTable);
					WinCommons._CloseProcessing();
					if (!(text != "success"))
					{
						BaseEntry = 0;
						FormMode = Enums.FormViewModeType.View;
					}
				}
			}
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

	private async void btnOK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			btnOK.Enabled = false;
			await UpdateData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			btnOK.Enabled = true;
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		isCancel = true;
		Close();
	}

	private void frm_JVApplicants_FormClosing(object sender, FormClosingEventArgs e)
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
					BaseEntry = 0;
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

	protected virtual async Task<DataTable> GetDepartment()
	{
		_ = 1;
		try
		{
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadComboDept();
			if (!dataTable.HasColumn())
			{
				dataTable = await WinFuntions.GetTableStructureAsync("OUDP");
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	protected virtual async Task<DataTable> GetBranch()
	{
		_ = 1;
		try
		{
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadComboBranch();
			if (!dataTable.HasColumn())
			{
				dataTable = await WinFuntions.GetTableStructureAsync("OUBR");
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	protected virtual async Task<DataTable> GetStatus()
	{
		try
		{
			DataTable dataTable = await _httpClient.EmployeeMasterClient.GetEmployeeStatus();
			if (!dataTable.HasColumn())
			{
				dataTable.Columns.Add("Code", typeof(int));
				dataTable.Columns.Add("Name", typeof(string));
				dataTable.AcceptChanges();
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	protected virtual async Task<DataTable> LoadTimeSheeting()
	{
		try
		{
			int branch = Function.ParseInt(luk_Branch.EditValue);
			string dept = Function.ToString(ccb_Department.EditValue);
			int empID = Function.ParseInt(txt_EmpId.EditValue);
			string empName = Function.ToString(txt_EmpName.EditValue);
			string status = Function.ToString(ccb_Status.EditValue);
			string period = Function.ToString(luk_Period.EditValue);
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("LoadTimeSheeting", new
			{
				branch = branch,
				dept = dept,
				empID = empID,
				empName = empName,
				status = status,
				period = period,
				workcode = "",
				baseEntry = BaseEntry
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	protected virtual async Task<DataTable> GetPeriod()
	{
		_ = 1;
		try
		{
			DataTable dataTable = await _httpClient.WorkPeriod.GetListWorkPeriod();
			if (!dataTable.HasColumn())
			{
				dataTable = await WinFuntions.GetTableStructureAsync("APZ_OHWP");
			}
			DataView defaultView = dataTable.DefaultView;
			defaultView.RowFilter = "PeriodStatus='U' OR PeriodStatus='C' OR PeriodStatus='S'";
			return defaultView.ToTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	protected virtual async Task<DataTable> GetWorkCode()
	{
		_ = 1;
		try
		{
			DataTable dataTable = await _httpClient.TimeSheetingClient.LoadWorkCode(2);
			if (!dataTable.HasColumn())
			{
				dataTable = await WinFuntions.GetTableStructureAsync("APZ_OHWC");
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	protected virtual async Task<DataTable> GetOvertimeType()
	{
		return new DataTable();
	}

	public virtual async Task<DataTable> LoadTimeSheetingSetting()
	{
		_ = 1;
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheeting");
			DataTable dataTable = await _help.GetAsync("LoadTimeSheetingSetting", new
			{
				userSign = GlobalData.UserName
			});
			if (!dataTable.HasColumn())
			{
				dataTable = await WinFuntions.GetTableStructureAsync("APZ_TSLS");
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	protected virtual async Task<DataTable> GetWorkType()
	{
		_ = 1;
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheeting");
			DataTable dataTable = await _help.GetAsync("GetWorkType");
			if (!dataTable.HasColumn())
			{
				dataTable = await WinFuntions.GetTableStructureAsync("APZ_OHWT");
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	public virtual async Task LoadControlsData()
	{
		_ = 7;
		try
		{
			Task<DataTable> branch = GetBranch();
			Task<DataTable> _dtOUDP = GetDepartment();
			Task<DataTable> _dtOHST = GetStatus();
			Task<DataTable> _dtAPZ_OHWP = GetPeriod();
			Task<DataTable> _dtSetting = LoadTimeSheetingSetting();
			Task<DataTable> _dtAPZ_OHOT = GetOvertimeType();
			Task<DataTable> _dtAPZ_OHWT = GetWorkType();
			Task<DataTable> _dtAPZ_OHWC = GetWorkCode();
			dtOUBR = await branch;
			dtOUDP = await _dtOUDP;
			dtOHST = await _dtOHST;
			dtAPZ_OHWP = await _dtAPZ_OHWP;
			dtSetting = await _dtSetting;
			dtAPZ_OHOT = await _dtAPZ_OHOT;
			dtAPZ_OHWT = await _dtAPZ_OHWT;
			dtAPZ_OHWC = await _dtAPZ_OHWC;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable DataTableWithDefineNew(DataTable dtSource, string valueMember, string displayMember = "")
	{
		try
		{
			DataTable dataTable = dtSource.Copy();
			DataRow dataRow = dataTable.NewRow();
			switch (Function.ToString(dataTable.Columns[valueMember].DataType))
			{
			case "System.Int32":
			case "System.Int16":
				dataRow[valueMember] = -123;
				break;
			case "System.String":
				dataRow[valueMember] = "Define New";
				break;
			}
			if (!string.IsNullOrEmpty(displayMember))
			{
				dataRow[displayMember] = "Define New";
			}
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return dtSource;
		}
	}

	protected virtual void SetupBranch_LookUp()
	{
		try
		{
			if (dtOUBR != null)
			{
				DataTable dataTable = dtOUBR.Copy();
				DataRow row = dataTable.NewRow();
				dataTable.Rows.InsertAt(row, 0);
				dataTable.AcceptChanges();
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = -1;
				dataRow["Name"] = "Define New";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				luk_Branch.Properties.NullText = "";
				luk_Branch.Properties.ValueMember = "Code";
				luk_Branch.Properties.DisplayMember = "Name";
				luk_Branch.Properties.ShowHeader = false;
				luk_Branch.Properties.DropDownRows = dataTable.Rows.Count;
				luk_Branch.Properties.DataSource = dataTable.Copy();
				luk_Branch.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
				if (dataTable.Rows.Count == 2)
				{
					luk_Branch.ItemIndex = 1;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void SetupDepartment__CheckedComboBox()
	{
		string valueMember = "Code";
		string displayMember = "Name";
		ccb_Department.Properties.ValueMember = valueMember;
		ccb_Department.Properties.DisplayMember = displayMember;
		if (dtOUDP != null)
		{
			ccb_Department.Properties.DataSource = DataTableWithDefineNew(dtOUDP, valueMember, displayMember);
		}
		else
		{
			DataTable tableStructure = WinFuntions.GetTableStructure("OUDP");
			ccb_Status.Properties.DataSource = DataTableWithDefineNew(tableStructure, valueMember, displayMember);
		}
		ccb_Department.Properties.DropDownRows = (ccb_Department.Properties.DataSource as DataTable).Rows.Count + 1;
		ccb_Department.Properties.SeparatorChar = ',';
	}

	protected virtual void SetupStatus__CheckedComboBox()
	{
		string valueMember = "Code";
		string displayMember = "Name";
		ccb_Status.Properties.ValueMember = valueMember;
		ccb_Status.Properties.DisplayMember = displayMember;
		if (dtOHST != null)
		{
			ccb_Status.Properties.DataSource = DataTableWithDefineNew(dtOHST, valueMember, displayMember);
		}
		else
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(int));
			dataTable.Columns.Add("Name", typeof(string));
			dataTable.AcceptChanges();
			ccb_Status.Properties.DataSource = DataTableWithDefineNew(dataTable, valueMember, displayMember);
		}
		ccb_Status.Properties.DropDownRows = (ccb_Status.Properties.DataSource as DataTable).Rows.Count + 1;
		ccb_Status.Properties.SeparatorChar = ',';
	}

	protected virtual void SetupPeriod_LookUp()
	{
		try
		{
			if (dtAPZ_OHWP != null)
			{
				DataTable dataTable = dtAPZ_OHWP.Copy();
				DataRow dataRow = dataTable.NewRow();
				dataRow["PeriodCode"] = "";
				dataRow["PeriodName"] = "";
				dataTable.Rows.InsertAt(dataRow, 0);
				dataTable.AcceptChanges();
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["PeriodCode"] = "Define New";
				dataRow2["PeriodName"] = "Define New";
				dataTable.Rows.Add(dataRow2);
				dataTable.AcceptChanges();
				luk_Period.Properties.NullText = "";
				luk_Period.Properties.ValueMember = "PeriodCode";
				luk_Period.Properties.DisplayMember = "PeriodName";
				luk_Period.Properties.ShowHeader = false;
				luk_Period.Properties.DropDownRows = dataTable.Rows.Count;
				luk_Period.Properties.DataSource = dataTable.Copy();
				luk_Period.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void SetupTimeSheeting_Value()
	{
		try
		{
			if (dtSetting.HasColumn() && dtSetting.IsNotNull())
			{
				DataRow dataRow = dtSetting.Rows[0];
				luk_Branch.EditValue = Function.ParseShort(dataRow["Branch"]);
				ccb_Department.SetEditValue(Function.ToString(dataRow["Department"]));
				ccb_Status.SetEditValue(Function.ToString(dataRow["Status"]));
				luk_Period.EditValue = Function.ToString(dataRow["Period"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void SetupOvertimeType_LookUp()
	{
	}

	public virtual void Setup_Controls()
	{
		SetupBranch_LookUp();
		SetupDepartment__CheckedComboBox();
		SetupStatus__CheckedComboBox();
		SetupPeriod_LookUp();
		SetupTimeSheeting_Value();
		SetupOvertimeType_LookUp();
	}

	private bool StayToSave()
	{
		if (FormMode == Enums.FormViewModeType.Update)
		{
			switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
			{
			case DialogResult.Yes:
				return false;
			case DialogResult.No:
				return true;
			}
		}
		return false;
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if (!StayToSave())
			{
				await LoadForm();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void SaveTimeSheetingSetting()
	{
		try
		{
			if (dtSetting == null)
			{
				return;
			}
			DataTable dataTable = dtSetting.Clone();
			DataRow dataRow = dataTable.NewRow();
			dataRow["Branch"] = Function.ToString(luk_Branch.EditValue);
			dataRow["Department"] = Function.ToString(ccb_Department.EditValue);
			dataRow["Status"] = Function.ToString(ccb_Status.EditValue);
			dataRow["Period"] = Function.ToString(luk_Period.EditValue);
			dataRow["UserSign"] = GlobalData.UserName;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			using HttpClientHelper<HttpResult> httpClientHelper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "TimeSheeting");
			HttpResult httpResult = httpClientHelper.Post("SaveTimeSheetingSetting", dataTable);
			if (httpResult != null)
			{
				string.IsNullOrEmpty(httpResult.Message);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetupWorkDay_CheckedComboBox()
	{
		ccb_WorkDay.Properties.Items.Add("Sunday", "Exclude Sundays");
		ccb_WorkDay.Properties.Items.Add("Saturday", "Exclude Saturdays");
	}

	private void DefaultColumnSetting()
	{
	}

	protected virtual void SetVisiblePeriodButton()
	{
		btn_ClosePeriod.Visible = true;
	}

	public async Task Search()
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			SetVisiblePeriodButton();
			btn_CancelTimeSheet.Enabled = true;
			selectedBranch = Function.ParseInt(luk_Branch.EditValue);
			selectedPeriod = Function.ToString(luk_Period.EditValue);
			DataRow dataRow = dtAPZ_OHWP.AsEnumerable().First((DataRow s) => Function.ParseInt(s["Branch"]) == selectedBranch && Function.ToString(s["PeriodCode"]) == selectedPeriod);
			hourPerDay = Function.ParseDouble(dataRow["HourPerDay"]);
			if (hourPerDay == 0.0)
			{
				hourPerDay = 8.0;
			}
			gridView1.OptionsBehavior.Editable = true;
			await GridViewColumnsGenerater(await LoadTimeSheeting());
			gridView1.ExpandAllGroups();
			WinCommons._CloseProcessing();
			FormMode = Enums.FormViewModeType.View;
			DataRow periodInfo = GetPeriodInfo(selectedPeriod, selectedBranch);
			if (periodInfo != null)
			{
				string text = Function.ToString(periodInfo["PeriodStatus"]);
				if (text == "U")
				{
					btn_ClosePeriod.Enabled = true;
					btn_CancelTimeSheet.Enabled = true;
				}
				if (text != "U")
				{
					btn_ClosePeriod.Enabled = false;
					btn_CancelTimeSheet.Enabled = false;
					gridView1.OptionsBehavior.Editable = false;
				}
			}
			dicErrorWorkCode.Clear();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void RemoveSplitter(object sender, CustomDisplayTextEventArgs e)
	{
		try
		{
			if (e.DisplayText.Length > 1 && e.DisplayText[e.DisplayText.Length - 1] == ',')
			{
				e.DisplayText = e.DisplayText.Remove(e.DisplayText.Length - 1, 1);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GridViewColumnsGenerater(DataTable dataSource)
	{
		_dicWorkDay = new Dictionary<string, string>();
		ccb_WorkDay.Properties.Items.Clear();
		SetupWorkDay_CheckedComboBox();
		dataSource.BeginInit();
		gridView1.Columns.Clear();
		gridControl1.DataSource = dataSource.Copy();
		gridControl1.RefreshDataSource();
		await WinFuntions.LoadUdfColumns(gridView1, "APZ_OTSL", null, overrideOld: true);
		gridView1.BeginUpdate();
		await gridView1.ReloadFormSetting(Enums.PivotFormSetting.TimeSheeting);
		gridView1.OptionsView.ColumnAutoWidth = false;
		foreach (DataColumn column in dataSource.Columns)
		{
			if (!char.IsDigit(Function.ToString(column.ColumnName)[0]))
			{
				if (gridView1.Columns[column.ColumnName] != null && column.ColumnName.Equals("EmpName"))
				{
					if (gridView1.Columns[column.ColumnName].ColumnEdit == null)
					{
						gridView1.Columns[column.ColumnName].ColumnEdit = rep_DrillDownEmp;
					}
					gridView1.Columns[column.ColumnName].OptionsColumn.ReadOnly = false;
					gridView1.Columns[column.ColumnName].OptionsColumn.AllowEdit = true;
					gridView1.Columns[column.ColumnName].ShowButtonMode = ShowButtonModeEnum.ShowAlways;
				}
				continue;
			}
			if (gridView1.Columns[column.ColumnName].ColumnEdit == null)
			{
				gridView1.Columns[column.ColumnName].ColumnEdit = workDay_Rep;
			}
			_dicWorkDay.Add(column.ColumnName, column.Caption);
			DateTime dateTime = Convert.ToDateTime(column.ColumnName);
			ccb_WorkDay.Properties.Items.Add(column.ColumnName, dateTime.ToString("d"));
			gridView1.Columns[column.ColumnName].Width = 50;
			gridView1.Columns[column.ColumnName].Caption = dateTime.Day.ToString();
			if (dateTime.DayOfWeek == DayOfWeek.Saturday)
			{
				gridView1.Columns[column.ColumnName].AppearanceCell.BackColor = Color.FromArgb(230, 255, 153);
			}
			else if (dateTime.DayOfWeek == DayOfWeek.Sunday)
			{
				gridView1.Columns[column.ColumnName].AppearanceCell.BackColor = Color.FromArgb(255, 221, 153);
			}
			gridView1.Columns[column.ColumnName].AppearanceHeader.Font = new Font(gridView1.Columns[column.ColumnName].AppearanceHeader.Font.FontFamily, gridView1.Columns[column.ColumnName].AppearanceHeader.Font.Size, FontStyle.Bold);
		}
		gridControl1.DataSourceTable.AcceptChanges();
		ccb_WorkDay.Properties.DropDownRows = _dicWorkDay.Count + 3;
		if (gridView1.Columns["EmpId"] != null)
		{
			gridView1.Columns["EmpId"].SortOrder = ColumnSortOrder.Ascending;
		}
		gridView1.EndUpdate();
		dataSource.EndInit();
		SaveTimeSheetingSetting();
		CreateContextMenu();
	}

	public virtual void SetupWorkDayRep()
	{
		try
		{
			workDay_Rep = new RepositoryItemTextEdit();
			workDay_Rep.CustomDisplayText += RemoveSplitter;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void SetupDictionary()
	{
		try
		{
			if (!dtAPZ_OHWC.HasColumn())
			{
				return;
			}
			if (_dicWorkColor != null)
			{
				_dicWorkColor.Clear();
			}
			if (_dicWorkRate != null)
			{
				_dicWorkRate.Clear();
			}
			foreach (DataRow item in dtAPZ_OHWC.AsEnumerable())
			{
				_dicWorkColor[Function.ToString(item["Code"]) + "_" + Function.ToString(item["Branch"])] = Function.ParseInt(item["Color"]);
				_dicWorkRate[Function.ToString(item["Code"]) + "_" + Function.ToString(item["Branch"])] = Function.ParseDouble(Function.ToString(item["Rate"]));
			}
			if (!dtAPZ_OHWT.HasColumn())
			{
				return;
			}
			var source = from s in dtAPZ_OHWC.AsEnumerable()
				join t in dtAPZ_OHWT.AsEnumerable().DefaultIfEmpty() on Function.ParseInt(s["WorkType"]) equals Function.ParseInt(t["Id"])
				select new
				{
					Code = Function.ToString(s["Code"]) + "_" + Function.ToString(s["Branch"]),
					Id = Function.ToString(t["Typ"])
				};
			if (_dicWorkType != null)
			{
				_dicWorkType.Clear();
			}
			try
			{
				_dicWorkType = source.ToDictionary(x => x.Code, x => x.Id);
			}
			catch
			{
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void SetupFormControls()
	{
		lbl_OT.Visible = false;
		luk_OvertimeType.Visible = false;
	}

	private async Task frm_TimeSheeting_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			await LoadForm();
			if (Function.ParseInt(BaseEntry) <= 0)
			{
				return;
			}
			WinCommons._OpenProcessing();
			DataSet ds = await GetDataTimeSheetingCopy();
			await GridViewColumnsGenerater(ds.Tables[0]);
			ccb_Department.EditValue = Function.ToString(ds.Tables[1].Rows[0]["Dept"]);
			ccb_Department.RefreshEditValue();
			ccb_Status.EditValue = Function.ToString(ds.Tables[1].Rows[0]["SStatus"]);
			ccb_Status.RefreshEditValue();
			luk_Branch.EditValue = ds.Tables[1].Rows[0]["Branch"];
			luk_Branch.RefreshEditValue();
			luk_Period.EditValue = ds.Tables[1].Rows[0]["Period"];
			luk_Period.RefreshEditValue();
			selectedBranch = Function.ParseInt(luk_Branch.EditValue);
			selectedPeriod = Function.ToString(luk_Period.EditValue);
			if (FormId.Equals("oTimeSheetingHour"))
			{
				luk_OvertimeType.EditValue = ds.Tables[1].Rows[0]["OvertimeType"];
				luk_OvertimeType.RefreshEditValue();
			}
			WinCommons._CloseProcessing();
			FormMode = Enums.FormViewModeType.Update;
			DataRow periodInfo = GetPeriodInfo(Period, Branch);
			if (periodInfo != null)
			{
				string text = Function.ToString(periodInfo["PeriodStatus"]);
				if (text == "U")
				{
					btn_ClosePeriod.Enabled = true;
					btn_CancelTimeSheet.Enabled = true;
				}
				if (text != "U")
				{
					btn_ClosePeriod.Enabled = false;
					btn_CancelTimeSheet.Enabled = false;
					gridView1.OptionsBehavior.Editable = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual async Task<DataSet> GetDataTimeSheetingCopy()
	{
		try
		{
			using HttpClientHelper<DataSet> _help = new HttpClientHelper<DataSet>(Configuration.ApiServer, "TimeSheeting");
			return await _help.GetAsync("LoadTimeSheetingCopyFrom", new
			{
				baseEntry = BaseEntry
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	private async Task LoadForm()
	{
		try
		{
			gridView1.ItemId = tableName;
			SetupWorkDayRep();
			_isLoading = true;
			if (BaseEntry > 0)
			{
				luk_Branch.ReadOnly = true;
				luk_Period.ReadOnly = true;
			}
			await LoadControlsData();
			Setup_Controls();
			SetupDictionary();
			PeriodFilter();
			OvertimeFilter();
			DataRow periodInfo = GetPeriodInfo(selectedPeriod, selectedBranch);
			if (periodInfo != null)
			{
				string text = Function.ToString(periodInfo["PeriodStatus"]);
				if (text == "U")
				{
					btn_ClosePeriod.Enabled = true;
					btn_CancelTimeSheet.Enabled = true;
				}
				if (text != "U")
				{
					btn_ClosePeriod.Enabled = false;
					btn_CancelTimeSheet.Enabled = false;
					gridView1.OptionsBehavior.Editable = false;
				}
			}
			gv_Export.RowStyle += GvExportOnRowStyle;
			if (Function.ParseInt(BaseEntry) > 0)
			{
				luk_Branch.EditValue = Branch;
				luk_Period.EditValue = Period;
			}
			_isLoading = false;
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			_isLoading = false;
		}
	}

	protected virtual void CreateContextMenu()
	{
		_menu = new DXPopupMenu();
		if (dtAPZ_OHWC.HasColumn())
		{
			DataView defaultView = dtAPZ_OHWC.DefaultView;
			defaultView.RowFilter = "Branch= " + selectedBranch;
			foreach (DataRow item in defaultView.ToTable().AsEnumerable())
			{
				DXMenuItem dXMenuItem = new DXMenuItem();
				dXMenuItem.Caption = Function.ToString(item["Code"]) + " - " + Function.ToString(item["Name"]);
				dXMenuItem.Click += MenuGridClick;
				dXMenuItem.Tag = new Tuple<string, string>(Function.ToString(item["Name"]), Function.ToString(item["Code"]));
				_menu.Items.Add(dXMenuItem);
			}
		}
		DXMenuItem dXMenuItem2 = new DXMenuItem();
		dXMenuItem2.Caption = Apzon.Commons.Properties.Resources.ButtonDelete;
		dXMenuItem2.Click += MenuGridClick;
		dXMenuItem2.Tag = new Tuple<string, string>(Apzon.Commons.Properties.Resources.ButtonDelete, "-");
		_menu.Items.Add(dXMenuItem2);
		DXMenuItem dXMenuItem3 = new DXMenuItem();
		dXMenuItem3.Caption = "Define New";
		dXMenuItem3.Click += MenuGridClick;
		dXMenuItem3.Tag = new Tuple<string, string>("Define New", "Define New");
		_menu.Items.Add(dXMenuItem3);
	}

	protected virtual void MenuGridClick(object sender, EventArgs e)
	{
		try
		{
			if (!(sender is DXMenuItem dXMenuItem))
			{
				return;
			}
			Tuple<string, string> obj = dXMenuItem.Tag as Tuple<string, string>;
			string text = Function.ToString(obj.Item2);
			if (obj == null)
			{
				return;
			}
			GridCell[] selectedCells = gridView1.GetSelectedCells();
			gridView1.BeginUpdate();
			for (int num = selectedCells.Length - 1; num >= 0; num--)
			{
				GridCell gridCell = selectedCells[num];
				if (_dicWorkDay.ContainsKey(gridCell.Column.FieldName))
				{
					if (text.Equals("Define New"))
					{
						frm_WorkCode frm_WorkCode2 = new frm_WorkCode();
						frm_WorkCode2.MdiParent = MdiParent;
						frm_WorkCode2.StartPosition = FormStartPosition.CenterScreen;
						frm_WorkCode2.FormClosing += WorkCode_FormClosing;
						frm_WorkCode2.Show();
						gridView1.EndUpdate();
						return;
					}
					if (text.Equals("-"))
					{
						gridView1.SetRowCellValue(gridCell.RowHandle, gridCell.Column, "");
					}
					else
					{
						string text2 = Function.ToString(gridView1.GetRowCellValue(gridCell.RowHandle, gridCell.Column));
						if (text2 != "")
						{
							List<string> list = text2.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
							list.Add(text);
							gridView1.SetRowCellValue(gridCell.RowHandle, gridCell.Column, string.Join(",", list.Distinct()));
						}
						else
						{
							gridView1.SetRowCellValue(gridCell.RowHandle, gridCell.Column, text);
						}
					}
				}
			}
			gridView1.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual async Task<HttpResult> SaveResult(DataTable dtNew)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController);
			return await helper.PostAsync("SaveTimeSheeting", dtNew);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new HttpResult(Enums.MessageCode.Error, "Error");
		}
	}

	private async Task<string> SaveData(DataTable dtNew)
	{
		_ = 1;
		try
		{
			dtNew.TableName = tableName;
			object currentDocNum = gridView1.GetFocusedRowCellValue("EmpId");
			HttpResult httpResult = await SaveResult(dtNew);
			if (httpResult != null && string.IsNullOrEmpty(httpResult.Message))
			{
				BaseEntry = 0;
				await Search();
				int num = gridView1.LocateByValue("EmpId", currentDocNum);
				if (num != int.MinValue)
				{
					gridView1.FocusedRowHandle = num;
					gridView1.MakeRowVisible(num);
					gridView1.Focus();
				}
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				return "success";
			}
			WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return "";
		}
	}

	private void CheckSaveState()
	{
		if (!_isLoading)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void gridView1_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		CheckSaveState();
	}

	public virtual void SetWorkTotal(int focusedRow)
	{
		try
		{
			double num = 0.0;
			foreach (KeyValuePair<string, string> item in _dicWorkDay)
			{
				Tuple<int, string> key = new Tuple<int, string>(focusedRow, item.Key);
				string[] array = Function.ToString(gridView1.GetRowCellValue(focusedRow, item.Key)).Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length == 0)
				{
					if (dicErrorWorkCode.ContainsKey(key))
					{
						dicErrorWorkCode.Remove(key);
					}
					continue;
				}
				string[] array2 = array;
				foreach (string text in array2)
				{
					string key2 = text + "_" + selectedBranch;
					if (!_dicWorkRate.ContainsKey(key2))
					{
						dicErrorWorkCode[key] = 1;
						WinCommons.Error(Enums.MessageCode.WorkCodeIsInvalidWithCode, "", text);
						return;
					}
					if (dicErrorWorkCode.ContainsKey(key))
					{
						dicErrorWorkCode.Remove(key);
					}
					if (_dicWorkType[key2] == "W" || _dicWorkType[key2] == "L")
					{
						num += Function.ParseDouble(_dicWorkRate[key2]);
					}
					if (_dicWorkType[key2] == "O")
					{
						num += Function.ParseDouble(_dicWorkRate[key2]);
					}
				}
			}
			gridView1.SetRowCellValue(focusedRow, "TotalBefore", num);
			double num2 = Function.ParseDouble(gridView1.GetRowCellValue(focusedRow, "Extra"));
			gridView1.SetRowCellValue(focusedRow, "WorkTotal", num + num2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void SetWorkType(int rowHandle)
	{
		try
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			foreach (KeyValuePair<string, string> item in _dicWorkDay)
			{
				string[] array = Function.ToString(gridView1.GetRowCellValue(rowHandle, item.Key)).Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < array.Length; i++)
				{
					string key = array[i] + "_" + selectedBranch;
					if (_dicWorkRate.ContainsKey(key) && _dicWorkType.ContainsKey(key))
					{
						switch (_dicWorkType[key])
						{
						case "W":
							num += Function.ParseDouble(_dicWorkRate[key]);
							break;
						case "A":
							num2 += Function.ParseDouble(_dicWorkRate[key]);
							break;
						case "L":
							num3 += Function.ParseDouble(_dicWorkRate[key]);
							break;
						case "O":
							num4 += Function.ParseDouble(_dicWorkRate[key]);
							break;
						}
					}
				}
			}
			gridView1.SetRowCellValue(rowHandle, "WorkSum", num);
			gridView1.SetRowCellValue(rowHandle, "AbsentSum", num2);
			gridView1.SetRowCellValue(rowHandle, "LeaveSum", num3);
			double num5 = num4 * hourPerDay;
			gridView1.SetRowCellValue(rowHandle, "UsedOT", num5);
			double num6 = Function.ParseDouble(gridView1.GetRowCellValue(rowHandle, "LeaveTotal"));
			double num7 = num6 - num3;
			gridView1.SetRowCellValue(rowHandle, "Remain", num7);
			if (num7 < 0.0)
			{
				string text = Function.ToString(gridView1.GetRowCellValue(rowHandle, "EmpName"));
				WinCommons.Error(Enums.MessageCode.LeaveDayHasExeededLimit, "", Function.ToString(num3), text, Function.ToString(num6));
			}
			double num8 = Function.ParseDouble(gridView1.GetRowCellValue(rowHandle, "TotalOT"));
			double num9 = num8 - num5;
			gridView1.SetRowCellValue(rowHandle, "RemainOT", num9);
			if (num9 < 0.0)
			{
				string text2 = Function.ToString(gridView1.GetRowCellValue(rowHandle, "EmpName"));
				WinCommons.Error(Enums.MessageCode.OTHourHasExeededLimit, "", Function.ToString(num5), text2, Function.ToString(num8));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void gridView1_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!changeCell)
			{
				changeCell = true;
				if (_dicWorkDay.ContainsKey(e.Column.FieldName))
				{
					SetWorkTotal(e.RowHandle);
					SetWorkType(e.RowHandle);
				}
				if (e.Column.FieldName.Equals("Extra"))
				{
					double num = Function.ParseDouble(gridView1.GetRowCellValue(e.RowHandle, "TotalBefore"));
					double num2 = Function.ParseDouble(e.Value);
					gridView1.SetRowCellValue(e.RowHandle, "WorkTotal", num + num2);
				}
				if (!_isLoading)
				{
					gridView1.SetFocusedRowModified();
					CheckSaveState();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			changeCell = false;
		}
	}

	private void gridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private async void gridControl1_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (!isKeyDown)
			{
				isKeyDown = true;
				if (e.KeyCode == Keys.Return)
				{
					gridView1.PostEditor();
					await UpdateData();
				}
				else if (gridControl1 != null && e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
				{
					ExtenseionMethods.PasteFromExcelToDataTable(gridControl1, gridView1, new List<string>());
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			isKeyDown = false;
		}
	}

	public virtual bool CheckBeforeSearch()
	{
		if (Function.ToString(luk_Branch.EditValue) == "")
		{
			WinCommons.Error(Enums.MessageCode.PleaseChooseABranch, "");
			return false;
		}
		if (Function.ToString(luk_Period.EditValue) == "")
		{
			WinCommons.Error(Enums.MessageCode.PleaseSelectAPeriod, "");
			return false;
		}
		if (Function.ToString(txt_EmpId.EditValue) != "" && Function.ToString(txt_EmpId.EditValue) != "0" && Function.ParseInt(txt_EmpId.EditValue) == 0)
		{
			WinCommons.Error(Enums.MessageCode.PleaseEnterValidEmployeeNo, "");
			return false;
		}
		return true;
	}

	private async Task btn_Search_Click(object sender, EventArgs e)
	{
		try
		{
			if (CheckBeforeSearch() && !StayToSave())
			{
				btn_Search.Enabled = false;
				await Search();
				gridView1.ExpandAllGroups();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			btn_Search.Enabled = true;
		}
	}

	public virtual void gridView1_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			if (e.MenuType != GridMenuType.Row || !e.HitInfo.InRow || e.HitInfo.InGroupRow || (e.HitInfo.Column != null && !char.IsDigit(e.HitInfo.Column.Caption[0])) || !btn_ClosePeriod.Enabled)
			{
				return;
			}
			gridView1.GridControl.ContextMenuStrip = new ContextMenuStrip();
			string textToSearch = Function.ToString(gridView1.GetFocusedRowCellValue(e.HitInfo.Column.FieldName));
			frm_CFL frm = new frm_CFL(Enums.TypeCFL.WorkCodeAll, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
			DataSet dataSource = GenerateDataSource();
			frm.DataSource = dataSource;
			frm.ButtonEditMultiSelect = true;
			frm_CFL frm_CFL2 = frm;
			frm_CFL2.AfterLoadForm = (Action)Delegate.Combine(frm_CFL2.AfterLoadForm, (Action)(() =>
			{
				frm.gv_CFL.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
				string[] array = textToSearch.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				GridColumn column = frm.gv_CFL.Columns["Code"];
				frm.gv_CFL.ClearSelection();
				frm.gv_CFL.OptionsBehavior.ImmediateUpdateRowPosition = false;
				string[] array2 = array;
				foreach (string text2 in array2)
				{
					int rowHandle = frm.gv_CFL.LocateByDisplayText(0, column, text2.Trim());
					frm.gv_CFL.SelectRow(rowHandle);
				}
			}));
			frm.ShowDialog(this);
			string text = "";
			if (frm.ChooseItems != null && frm.ChooseItems.Count > 0)
			{
				foreach (DataRow chooseItem in frm.ChooseItems)
				{
					text = text + "," + chooseItem["Code"];
				}
				text = ((!string.IsNullOrEmpty(text) && text.StartsWith(",")) ? text.Substring(1, text.Length - 1) : "");
			}
			GridCell[] selectedCells = gridView1.GetSelectedCells();
			for (int num = 0; num < gridView1.GetSelectedCells().Length; num++)
			{
				if (_dicWorkDay.ContainsKey(selectedCells[num].Column.FieldName))
				{
					gridView1.SetRowCellValue(selectedCells[num].RowHandle, selectedCells[num].Column, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private DataSet GenerateDataSource()
	{
		try
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ColumnName", typeof(string));
			dataTable.Columns.Add("Caption", typeof(string));
			dataTable.Columns.Add("DataType", typeof(string));
			dataTable.Columns.Add("SubType", typeof(string));
			dataTable.Columns.Add("Width", typeof(int));
			dataTable.Columns.Add("GroupIndex", typeof(int));
			dataTable.Columns.Add("Visible", typeof(int));
			dataTable.Columns.Add("FormatType", typeof(string));
			dataTable.Columns.Add("DisplayFormat", typeof(string));
			dataTable.Columns.Add("ReadOnly", typeof(int));
			dataTable.Columns.Add("AllowEdit", typeof(int));
			dataTable.Columns.Add("AllowMove", typeof(int));
			dataTable.Columns.Add("AllowSize", typeof(int));
			dataTable.Columns.Add("AllowSort", typeof(int));
			dataTable.Columns.Add("Fixed", typeof(string));
			dataTable.Columns.Add("SummaryType", typeof(string));
			dataTable.Columns.Add("GroupSummaryType", typeof(string));
			dataTable.Rows.Add("Code", "Code", "nvarchar", "", 80, -1, 1, DBNull.Value, DBNull.Value, 1, 0, 1, 1, 1, "L", DBNull.Value, DBNull.Value);
			dataTable.Rows.Add("Name", "Name", "nvarchar", "", 275, -1, 1, DBNull.Value, DBNull.Value, 1, 0, 1, 1, 1, "L", DBNull.Value, DBNull.Value);
			DataTable dataTable2 = new DataTable();
			DataView defaultView = dtAPZ_OHWC.DefaultView;
			defaultView.RowFilter = "Branch= " + selectedBranch;
			dataTable2 = defaultView.ToTable(false, "Code", "Name").Copy();
			dataSet.Tables.Add(dataTable);
			dataSet.Tables.Add(dataTable2);
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public virtual void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			GridView gridView = sender as GridView;
			if (e.Column.FieldName == "Remain")
			{
				double num = Function.ParseDouble(gridView.GetRowCellValue(e.RowHandle, "LeaveSum"));
				double num2 = Function.ParseDouble(gridView.GetRowCellValue(e.RowHandle, "LeaveTotal"));
				if (num > num2)
				{
					e.Appearance.ForeColor = Color.FromArgb(204, 59, 74);
				}
				if (num == num2)
				{
					e.Appearance.ForeColor = Color.Orange;
				}
			}
			else if (e.Column.FieldName == "RemainOT")
			{
				double num3 = Function.ParseDouble(gridView.GetRowCellValue(e.RowHandle, "UsedOT"));
				double num4 = Function.ParseDouble(gridView.GetRowCellValue(e.RowHandle, "TotalOT"));
				if (num3 > num4)
				{
					e.Appearance.ForeColor = Color.FromArgb(204, 59, 74);
				}
				if (num3 == num4)
				{
					e.Appearance.ForeColor = Color.Orange;
				}
			}
			else
			{
				if (!_dicWorkDay.ContainsKey(e.Column.FieldName))
				{
					return;
				}
				if (gridView1.IsCellSelected(e.RowHandle, e.Column))
				{
					e.Appearance.BackColor = Color.FromArgb(226, 234, 253);
					return;
				}
				string text = Function.ToString(e.CellValue);
				if (!(text == ""))
				{
					string text2 = text.Split(',')[0];
					if (!string.IsNullOrEmpty(text2) && _dicWorkColor != null && _dicWorkColor.ContainsKey(text2 + "_" + selectedBranch))
					{
						int num5 = _dicWorkColor[text2 + "_" + selectedBranch];
						ColorEdit colorEdit = new ColorEdit();
						colorEdit.EditValue = num5;
						e.Appearance.ForeColor = Color.Black;
						e.Appearance.BackColor = colorEdit.Color;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void checkedComboBoxEdit1_EditValueChanged(object sender, EventArgs e)
	{
		if (_isLoading)
		{
			return;
		}
		try
		{
			bool flag = false;
			bool flag2 = false;
			List<string> list;
			if (!string.IsNullOrEmpty(Function.ToString(ccb_WorkDay.EditValue)))
			{
				list = (from t in Function.ToString(ccb_WorkDay.EditValue).Split(',')
					select t.Trim()).ToList();
				foreach (string item in list)
				{
					if (item == "Sunday")
					{
						flag = true;
					}
					if (item == "Saturday")
					{
						flag2 = true;
					}
					if (item != "Sunday" && item != "Saturday")
					{
						flag = false;
						flag2 = false;
					}
				}
			}
			else
			{
				list = _dicWorkDay.Select((KeyValuePair<string, string> d) => d.Key).ToList();
			}
			if (flag | flag2)
			{
				list = _dicWorkDay.Select((KeyValuePair<string, string> d) => d.Key).ToList();
				if (flag)
				{
					list = list.Where((string d) => Convert.ToDateTime(d).DayOfWeek != DayOfWeek.Sunday).ToList();
				}
				if (flag2)
				{
					list = list.Where((string d) => Convert.ToDateTime(d).DayOfWeek != DayOfWeek.Saturday).ToList();
				}
			}
			gridView1.BeginUpdate();
			foreach (KeyValuePair<string, string> item2 in _dicWorkDay)
			{
				gridView1.Columns.Remove(gridView1.Columns[item2.Key]);
			}
			for (int num = 0; num < list.Count; num++)
			{
				if (char.IsDigit(Function.ToString(list[num])[0]))
				{
					GridColumnEx gridColumnEx = new GridColumnEx
					{
						Name = list[num],
						FieldName = list[num],
						Caption = Convert.ToDateTime(list[num]).Day.ToString(),
						Visible = true
					};
					gridView1.Columns.Add(gridColumnEx);
					gridView1.Columns[gridColumnEx.Name].Width = 50;
					DateTime dateTime = Convert.ToDateTime(list[num]);
					if (dateTime.DayOfWeek == DayOfWeek.Saturday)
					{
						gridView1.Columns[gridColumnEx.Name].AppearanceCell.BackColor = Color.FromArgb(230, 255, 153);
					}
					else if (dateTime.DayOfWeek == DayOfWeek.Sunday)
					{
						gridView1.Columns[gridColumnEx.Name].AppearanceCell.BackColor = Color.FromArgb(255, 221, 153);
					}
					gridView1.Columns[gridColumnEx.Name].AppearanceHeader.Font = new Font(gridColumnEx.AppearanceHeader.Font.FontFamily, gridColumnEx.AppearanceHeader.Font.Size, FontStyle.Bold);
					if (gridView1.Columns[gridColumnEx.Name].ColumnEdit == null)
					{
						gridView1.Columns[gridColumnEx.Name].ColumnEdit = workDay_Rep;
					}
				}
			}
			gridView1.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void gridView1_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode != Keys.Delete)
			{
				return;
			}
			gridView1.BeginUpdate();
			GridCell[] selectedCells = gridView1.GetSelectedCells();
			foreach (GridCell gridCell in selectedCells)
			{
				if (gridCell.Column.OptionsColumn.AllowEdit && !gridCell.Column.ReadOnly)
				{
					gridView1.SetRowCellValue(gridCell.RowHandle, gridCell.Column, null);
				}
			}
			gridView1.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void ccb_WorkDay_Popup(object sender, EventArgs e)
	{
		try
		{
			PopupContainerForm obj = (sender as IPopupControl).PopupWindow as PopupContainerForm;
			(obj.Controls[3].Controls[0] as CheckedListBoxControl).ItemChecking -= Form1_ItemChecking;
			(obj.Controls[3].Controls[0] as CheckedListBoxControl).ItemChecking += Form1_ItemChecking;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void Form1_ItemChecking(object sender, ItemCheckingEventArgs e)
	{
		try
		{
			CheckedListBoxControl checkedListBoxControl = sender as CheckedListBoxControl;
			if (e.NewValue != CheckState.Checked)
			{
				return;
			}
			if (e.Index == 1 || e.Index == 2)
			{
				for (int i = 3; i < checkedListBoxControl.Items.Count; i++)
				{
					checkedListBoxControl.SetItemCheckState(i, CheckState.Unchecked);
				}
			}
			else
			{
				checkedListBoxControl.SetItemCheckState(1, CheckState.Unchecked);
				checkedListBoxControl.SetItemCheckState(2, CheckState.Unchecked);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public virtual void btnImport_Click(object sender, EventArgs e)
	{
		try
		{
			frm_ImportTimeSheeting frm_ImportTimeSheeting2 = new frm_ImportTimeSheeting();
			frm_ImportTimeSheeting2.BranchValue = Function.ParseInt(luk_Branch.EditValue);
			frm_ImportTimeSheeting2.PeriodValue = Function.ToString(luk_Period.EditValue);
			frm_ImportTimeSheeting2.MdiParent = MdiParent;
			frm_ImportTimeSheeting2.StartPosition = FormStartPosition.CenterScreen;
			frm_ImportTimeSheeting2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void luk_Branch_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				PeriodFilter();
				OvertimeFilter();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void OvertimeFilter()
	{
		if (!(FormId != "oTimeSheetingHour"))
		{
			int num = Function.ParseInt(luk_Branch.EditValue);
			DataTable dataTable = dtAPZ_OHOT.Copy();
			if (dataTable.HasColumn())
			{
				DataView dataView = dataTable.AsDataView();
				dataView.RowFilter = "Branch= " + num;
				dataTable = dataView.ToTable();
			}
			else
			{
				dataTable = WinFuntions.GetTableStructure("APZ_OHOT");
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = "Define New";
			dataRow["Name"] = "Define New";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			luk_OvertimeType.Properties.DisplayMember = "Name";
			luk_OvertimeType.Properties.ValueMember = "Code";
			luk_OvertimeType.Properties.DataSource = dataTable;
			luk_OvertimeType.Properties.DropDownRows = dataTable.Rows.Count;
			if (luk_OvertimeType.Text == "")
			{
				luk_OvertimeType.EditValue = "";
			}
		}
	}

	private void PeriodFilter()
	{
		int num = Function.ParseInt(luk_Branch.EditValue);
		DataTable dataTable = dtAPZ_OHWP.Copy();
		if (dataTable.HasColumn())
		{
			DataView dataView = dataTable.AsDataView();
			dataView.RowFilter = "Branch= " + num;
			dataTable = dataView.ToTable();
		}
		else
		{
			dataTable = WinFuntions.GetTableStructure("APZ_OHWP");
		}
		DataRow dataRow = dataTable.NewRow();
		dataRow["PeriodCode"] = "Define New";
		dataRow["PeriodName"] = "Define New";
		dataTable.Rows.Add(dataRow);
		dataTable.AcceptChanges();
		luk_Period.Properties.DataSource = dataTable;
		luk_Period.Properties.DropDownRows = dataTable.Rows.Count;
		luk_Period.Properties.DisplayMember = "PeriodName";
		luk_Period.Properties.ValueMember = "PeriodCode";
		if (luk_Period.Text == "")
		{
			luk_Period.EditValue = "";
		}
	}

	private void btnExport_Click(object sender, EventArgs e)
	{
		try
		{
			if (Function.ToString(luk_Branch.EditValue) == "")
			{
				WinCommons.Error(Enums.MessageCode.PleaseChooseABranch, "");
			}
			else if (Function.ToString(luk_Period.EditValue) == "")
			{
				WinCommons.Error(Enums.MessageCode.PleaseSelectAPeriod, "");
			}
			else if (Function.ToString(txt_EmpId.EditValue) != "" && Function.ToString(txt_EmpId.EditValue) != "0" && Function.ParseInt(txt_EmpId.EditValue) == 0)
			{
				WinCommons.Error(Enums.MessageCode.PleaseEnterValidEmployeeNo, "");
			}
			else
			{
				ExportToExcel();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GvExportOnRowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (!(Function.ToString(gv_Export.GetRowCellValue(e.RowHandle, "WorkDay")) == ""))
			{
				DateTime dateTime = Function.ParseDateTimes(gv_Export.GetRowCellValue(e.RowHandle, "WorkDay"));
				if (dateTime.DayOfWeek == DayOfWeek.Saturday)
				{
					e.Appearance.BackColor = Color.FromArgb(230, 255, 153);
				}
				else if (dateTime.DayOfWeek == DayOfWeek.Sunday)
				{
					e.Appearance.BackColor = Color.FromArgb(255, 221, 153);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void ExportToExcel()
	{
		try
		{
			Task<DataTable> exportData = GetExportData();
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = "Excel Files (*.xlsx)|*.xlsx";
			dlg.DefaultExt = "xlsx";
			dlg.ShowDialog(this);
			if (!string.IsNullOrEmpty(dlg.FileName))
			{
				DataTable dataTable = await exportData;
				if (dataTable.HasColumn())
				{
					DataView dataView = new DataView(dataTable);
					dataView.Sort = "EmpId ASC, WorkDay ASC";
					gv_Export.Columns.Clear();
					grd_Export.DataSource = dataView;
					gv_Export.PopulateColumns();
					grd_Export.RefreshDataSource();
					XlsxExportOptionsEx options = new XlsxExportOptionsEx
					{
						ExportType = ExportType.WYSIWYG
					};
					gv_Export.OptionsPrint.UsePrintStyles = false;
					gv_Export.ExportToXlsx(dlg.FileName, options);
					WinCommons.Success(Enums.MessageCode.ExportSuccess, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GetExportData()
	{
		try
		{
			int branch = Function.ParseInt(luk_Branch.EditValue);
			string dept = Function.ToString(ccb_Department.EditValue);
			int empID = Function.ParseInt(txt_EmpId.EditValue);
			string empName = Function.ToString(txt_EmpName.EditValue);
			string status = Function.ToString(ccb_Status.EditValue);
			string period = Function.ToString(luk_Period.EditValue);
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			DataTable dataTable = await helper.GetAsync("ExportTimeSheeting", new { branch, dept, empID, empName, status, period });
			if (dataTable.HasColumn())
			{
				return dataTable;
			}
			WinCommons.Error(Enums.MessageCode.ExportFailedNoDataWereFound, "");
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private void luk_Branch_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseInt(e.NewValue) == -1)
			{
				frm_HRBranches frm_HRBranches2 = new frm_HRBranches();
				frm_HRBranches2.MdiParent = MdiParent;
				frm_HRBranches2.StartPosition = FormStartPosition.CenterScreen;
				frm_HRBranches2.FormClosing += Branch_FormClosing;
				frm_HRBranches2.Show();
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void Branch_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			dtOUBR = await GetBranch();
			SetupBranch_LookUp();
			luk_Branch.EditValue = Function.ParseShort(dtOUBR.AsEnumerable().Max((DataRow s) => s["Code"]));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_Period_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ToString(e.NewValue) == "Define New")
			{
				frm_WorkPeriod frm_WorkPeriod2 = new frm_WorkPeriod();
				frm_WorkPeriod2.MdiParent = MdiParent;
				frm_WorkPeriod2.StartPosition = FormStartPosition.CenterScreen;
				frm_WorkPeriod2.FormClosing += Period_FormClosing;
				frm_WorkPeriod2.BranchFromTimeSheeting = Function.ParseInt(luk_Branch.EditValue);
				frm_WorkPeriod2.Show();
				frm_WorkPeriod2.OpenFormNewPeriod();
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void Period_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			dtAPZ_OHWP = await GetPeriod();
			SetupPeriod_LookUp();
			PeriodFilter();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_OvertimeType_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ToString(e.NewValue) == "Define New")
			{
				frm_OvertimeType frm_OvertimeType2 = new frm_OvertimeType();
				frm_OvertimeType2.MdiParent = MdiParent;
				frm_OvertimeType2.StartPosition = FormStartPosition.CenterScreen;
				frm_OvertimeType2.FormClosing += OvertimeType_FormClosing;
				frm_OvertimeType2.Show();
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void OvertimeType_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			dtAPZ_OHOT = await GetOvertimeType();
			SetupOvertimeType_LookUp();
			OvertimeFilter();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void WorkCode_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			dtAPZ_OHWC = await GetWorkCode();
			SetupDictionary();
			CreateContextMenu();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ccb_Status_Popup(object sender, EventArgs e)
	{
		try
		{
			CheckedComboBoxEdit obj = sender as CheckedComboBoxEdit;
			obj.InternalListBox.ItemChecking -= ccb_Status_ItemChecking;
			obj.InternalListBox.ItemChecking += ccb_Status_ItemChecking;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void ccb_Status_ItemChecking(object sender, ItemCheckingEventArgs e)
	{
		try
		{
			if ((sender as CheckedListBoxControl).Items[e.Index].Description == "Define New" && e.OldValue == CheckState.Unchecked && e.NewValue == CheckState.Checked)
			{
				frm_HREmpStatus frm_HREmpStatus2 = new frm_HREmpStatus();
				frm_HREmpStatus2.FormClosing += Status_FormClosing;
				frm_HREmpStatus2.Show();
				e.Cancel = true;
				ccb_Department.ClosePopup();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async void Status_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			dtOHST = await GetStatus();
			SetupStatus__CheckedComboBox();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ccb_Department_Popup(object sender, EventArgs e)
	{
		try
		{
			CheckedComboBoxEdit obj = sender as CheckedComboBoxEdit;
			obj.InternalListBox.ItemChecking -= ccb_Department_ItemChecking;
			obj.InternalListBox.ItemChecking += ccb_Department_ItemChecking;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void ccb_Department_ItemChecking(object sender, ItemCheckingEventArgs e)
	{
		try
		{
			if ((sender as CheckedListBoxControl).Items[e.Index].Description == "Define New" && e.OldValue == CheckState.Unchecked && e.NewValue == CheckState.Checked)
			{
				frm_HRDepartment frm_HRDepartment2 = new frm_HRDepartment();
				frm_HRDepartment2.FormClosing += Department_FormClosing;
				frm_HRDepartment2.Show();
				e.Cancel = true;
				ccb_Department.ClosePopup();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async void Department_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			dtOUDP = await GetDepartment();
			SetupDepartment__CheckedComboBox();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ccb_Status_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
	{
		try
		{
			if (e.DisplayText.Contains(", Define New"))
			{
				e.DisplayText = e.DisplayText.Replace(", Define New", "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ccb_Status_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			string text = Function.ToString(e.NewValue);
			if (text.Contains("-123"))
			{
				text = text.Replace("-123", "");
				e.NewValue = text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void labelLine5_Load(object sender, EventArgs e)
	{
	}

	private void ccb_Status_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void labelLine1_Load(object sender, EventArgs e)
	{
	}

	private async void btn_CancelTimeSheet_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			btn_CancelTimeSheet.Enabled = false;
			if (selectedPeriod == "")
			{
				return;
			}
			if (!gridControl1.DataSourceTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.NoDataFound, "");
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCancelTimeSheeting, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2, selectedPeriod) != DialogResult.No)
			{
				string text = await _httpClient.TimeSheetingClient.CancelTimeSheeting(selectedPeriod);
				if (string.IsNullOrWhiteSpace(text))
				{
					WinCommons.Success(Enums.MessageCode.CancelSuccess, "");
					await Search();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, "Cancel TimeSheeting Failed: " + text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			btn_CancelTimeSheet.Enabled = true;
		}
	}

	private void gridView1_RowCellClick(object sender, RowCellClickEventArgs e)
	{
		try
		{
			if (!Function.ParseDateTime(e.Column.FieldName, "yyyy.MM.dd").HasValue)
			{
				gridView1.ShowEditor();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridView1_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "RemainOT")
			{
				double num = Function.ParseDouble(e.Value);
				if (num > 0.0)
				{
					e.DisplayText = num / hourPerDay + $" ({num}h)";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void lbl_OT_Load(object sender, EventArgs e)
	{
	}

	private async void btn_ClosePeriod_Click(object sender, EventArgs e)
	{
		try
		{
			if (selectedPeriod == "")
			{
				return;
			}
			if (this is frm_TimeSheetingHour)
			{
				WinCommons.Error(Enums.MessageCode.PeriodCannotBeCloseHere, "");
				return;
			}
			DataRow dataRow = dtOUBR.AsEnumerable().FirstOrDefault((DataRow s) => Function.ParseInt(s["Code"]) == selectedBranch);
			string text = "";
			if (dataRow != null)
			{
				text = Function.ToString(dataRow["Name"]);
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmClosePeriod, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, selectedPeriod, text) == DialogResult.No)
			{
				return;
			}
			btn_ClosePeriod.Enabled = false;
			DataRow periodInfo = GetPeriodInfo(selectedPeriod, selectedBranch);
			if (periodInfo == null)
			{
				WinCommons.Error(Enums.MessageCode.Unable2ClosePeriodNoInfomationAboutCurrentPeriodFound, "");
				btn_ClosePeriod.Enabled = true;
				return;
			}
			periodInfo["PeriodStatus"] = "C";
			DataTable dataTable = dtAPZ_OHWP.Clone();
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2.ItemArray = periodInfo.ItemArray;
			dataTable.Rows.Add(dataRow2);
			dataTable.AcceptChanges();
			HttpResult httpResult = await _httpClient.WorkPeriod.Update(dataTable);
			if (httpResult != null && httpResult.Message != "")
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				btn_ClosePeriod.Enabled = true;
				return;
			}
			gridView1.OptionsBehavior.Editable = false;
			WinCommons.Success(Enums.MessageCode.ClosePeriodSuccess, "");
			frm_LeaveCalculation frm = new frm_LeaveCalculation();
			frm.Branch = selectedBranch;
			frm.FromPeriod = selectedPeriod;
			frm.MdiParent = MdiParent;
			frm.StartPosition = FormStartPosition.CenterScreen;
			BeginInvoke((Action)(() =>
			{
				frm.Show();
			}));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataRow GetPeriodInfo(string periodCode, int branch)
	{
		try
		{
			return dtAPZ_OHWP.AsEnumerable().FirstOrDefault((DataRow dr) => Function.ToString(dr["PeriodCode"]) == periodCode && Function.ParseInt(dr["Branch"]) == branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		return null;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.TimeSheeting.frm_TimeSheeting));
		this.gridControl1 = new Apzon.Controls.Grid.GridControlEx();
		this.gridView1 = new Apzon.Controls.Grid.GridViewEx();
		this.EmpId = new Apzon.Controls.Grid.GridColumnEx();
		this.EmpName = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_DrillDownEmp = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcPeriod = new Apzon.Controls.Grid.GridColumnEx();
		this.gcWorkSum = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAbsentSum = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLeaveSum = new Apzon.Controls.Grid.GridColumnEx();
		this.TotalBefore = new Apzon.Controls.Grid.GridColumnEx();
		this.Extra = new Apzon.Controls.Grid.GridColumnEx();
		this.WorkQuota = new Apzon.Controls.Grid.GridColumnEx();
		this.WorkTotal = new Apzon.Controls.Grid.GridColumnEx();
		this.DocStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_EmpId = new Apzon.Controls.Editor.TextEditEx();
		this.txt_EmpName = new Apzon.Controls.Editor.TextEditEx();
		this.luk_Period = new Apzon.Controls.Editor.LookUpEditEx();
		this.ccb_Department = new Apzon.Controls.Editor.CheckedComboBoxEditEx();
		this.ccb_Status = new Apzon.Controls.Editor.CheckedComboBoxEditEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.luk_Branch = new Apzon.Controls.Editor.LookUpEditEx();
		this.ccb_WorkDay = new Apzon.Controls.Editor.CheckedComboBoxEditEx();
		this.btnImport = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Export = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Export = new Apzon.Controls.Grid.GridViewEx();
		this.btnExport = new Apzon.Controls.Controls.SimpleButtonEx();
		this.luk_OvertimeType = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.lbl_OT = new Apzon.Controls.Controls.LabelLine();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.btn_ClosePeriod = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_CancelTimeSheet = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DrillDownEmp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpId.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Period.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ccb_Department.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ccb_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ccb_WorkDay.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Export).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Export).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_OvertimeType.Properties).BeginInit();
		base.SuspendLayout();
		this.gridControl1.AllowPasteExcel = true;
		this.gridControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridControl1.ClipboardData = null;
		this.gridControl1.Location = new System.Drawing.Point(0, 138);
		this.gridControl1.MainView = this.gridView1;
		this.gridControl1.Name = "gridControl1";
		this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_DrillDownEmp });
		this.gridControl1.Size = new System.Drawing.Size(1049, 321);
		this.gridControl1.TabIndex = 2;
		this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView1 });
		this.gridControl1.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.gridControl1_EditorKeyDown);
		this.gridView1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView1.ColumnKey = null;
		this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[11]
		{
			this.EmpId, this.EmpName, this.gcPeriod, this.gcWorkSum, this.gcAbsentSum, this.gcLeaveSum, this.TotalBefore, this.Extra, this.WorkQuota, this.WorkTotal,
			this.DocStatus
		});
		this.gridView1.DisableFormSetting = false;
		this.gridView1.DisplayException = false;
		this.gridView1.EditColumn = null;
		this.gridView1.FieldCurrency = null;
		this.gridView1.FieldRate = null;
		this.gridView1.FormId = "oTimeSheeting";
		this.gridView1.GridControl = this.gridControl1;
		this.gridView1.IgnoreFilter = false;
		this.gridView1.IndexEditRow = -1;
		this.gridView1.IndicatorWidth = 30;
		this.gridView1.IsApzTable = false;
		this.gridView1.IsInsertNewRow = false;
		this.gridView1.IsLoadingData = false;
		this.gridView1.IsMappingContextAction = false;
		this.gridView1.ItemId = "APZ_OTSL";
		this.gridView1.LstColumnAllowBatch = null;
		this.gridView1.LstColumnNotAllowBatch = null;
		this.gridView1.LstColumnNotEditable = null;
		this.gridView1.LstColumNotSetting = null;
		this.gridView1.Name = "gridView1";
		this.gridView1.OptionsEditForm.ShowOnEnterKey = DevExpress.Utils.DefaultBoolean.False;
		this.gridView1.OptionsSelection.MultiSelect = true;
		this.gridView1.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.gridView1.OptionsView.ColumnAutoWidth = false;
		this.gridView1.OptionsView.ShowGroupPanel = false;
		this.gridView1.Tag = "APZ_OTSL";
		this.gridView1.UseBatchMod = false;
		this.gridView1.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(this.gridView1_RowCellClick);
		this.gridView1.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridView1_CustomDrawRowIndicator);
		this.gridView1.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gridView1_RowCellStyle);
		this.gridView1.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gridView1_PopupMenuShowing);
		this.gridView1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanged);
		this.gridView1.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanging);
		this.gridView1.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridView1_CustomColumnDisplayText);
		this.gridView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridView1_KeyDown);
		this.EmpId.Caption = "Emp No.";
		this.EmpId.CurrencyDateField = null;
		this.EmpId.CurrencyField = null;
		this.EmpId.CurrencyFixed = null;
		this.EmpId.CurrencyFixedRate = 0.0;
		this.EmpId.CurrencyRateField = null;
		this.EmpId.FieldName = "EmpId";
		this.EmpId.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.EmpId.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.EmpId.Name = "EmpId";
		this.EmpId.OptionsColumn.AllowEdit = false;
		this.EmpId.OptionsColumn.ReadOnly = true;
		this.EmpId.OptionsEditForm.VisibleIndex = 1;
		this.EmpId.PrimaryKey = true;
		this.EmpId.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedRow;
		this.EmpId.Tag = "APZ_OTSL EmpId";
		this.EmpId.UnitField = null;
		this.EmpId.Visible = true;
		this.EmpId.VisibleIndex = 0;
		this.EmpId.Width = 50;
		this.EmpName.Caption = "Employee Name";
		this.EmpName.ColumnEdit = this.rep_DrillDownEmp;
		this.EmpName.CurrencyDateField = null;
		this.EmpName.CurrencyField = null;
		this.EmpName.CurrencyFixed = null;
		this.EmpName.CurrencyFixedRate = 0.0;
		this.EmpName.CurrencyRateField = null;
		this.EmpName.FieldName = "EmpName";
		this.EmpName.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.EmpName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.EmpName.Name = "EmpName";
		this.EmpName.OptionsColumn.AllowEdit = false;
		this.EmpName.OptionsColumn.ReadOnly = true;
		this.EmpName.PrimaryKey = false;
		this.EmpName.Tag = "APZ_OTSL EmpName";
		this.EmpName.UnitField = null;
		this.EmpName.Visible = true;
		this.EmpName.VisibleIndex = 1;
		this.EmpName.Width = 100;
		this.rep_DrillDownEmp.AutoHeight = false;
		this.rep_DrillDownEmp.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_DrillDownEmp.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_DrillDownEmp.ContainerControl = this;
		this.rep_DrillDownEmp.DisplayMember = "";
		this.rep_DrillDownEmp.DisplayMember2 = "";
		this.rep_DrillDownEmp.LoadDataSourceWhenEnter = false;
		this.rep_DrillDownEmp.Name = "rep_DrillDownEmp";
		this.rep_DrillDownEmp.ReadOnly = true;
		this.rep_DrillDownEmp.SeparateCharacterForMulti = ';';
		this.rep_DrillDownEmp.SeparateString = ", ";
		this.rep_DrillDownEmp.UsingAutoCompleteBox = false;
		this.rep_DrillDownEmp.UsingDataSource = true;
		this.rep_DrillDownEmp.ValidateWhenNoDataSource = false;
		this.rep_DrillDownEmp.ValueMember = "";
		this.gcPeriod.Caption = "Period";
		this.gcPeriod.CurrencyDateField = null;
		this.gcPeriod.CurrencyField = null;
		this.gcPeriod.CurrencyFixed = null;
		this.gcPeriod.CurrencyFixedRate = 0.0;
		this.gcPeriod.CurrencyRateField = null;
		this.gcPeriod.FieldName = "Period";
		this.gcPeriod.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcPeriod.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcPeriod.Name = "gcPeriod";
		this.gcPeriod.OptionsColumn.AllowEdit = false;
		this.gcPeriod.OptionsColumn.ReadOnly = true;
		this.gcPeriod.PrimaryKey = false;
		this.gcPeriod.UnitField = null;
		this.gcPeriod.Visible = true;
		this.gcPeriod.VisibleIndex = 2;
		this.gcPeriod.Width = 60;
		this.gcWorkSum.Caption = "Work";
		this.gcWorkSum.CurrencyDateField = null;
		this.gcWorkSum.CurrencyField = null;
		this.gcWorkSum.CurrencyFixed = null;
		this.gcWorkSum.CurrencyFixedRate = 0.0;
		this.gcWorkSum.CurrencyRateField = null;
		this.gcWorkSum.FieldName = "WorkSum";
		this.gcWorkSum.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcWorkSum.Name = "gcWorkSum";
		this.gcWorkSum.OptionsColumn.AllowEdit = false;
		this.gcWorkSum.OptionsColumn.ReadOnly = true;
		this.gcWorkSum.PrimaryKey = false;
		this.gcWorkSum.UnitField = null;
		this.gcWorkSum.Visible = true;
		this.gcWorkSum.VisibleIndex = 4;
		this.gcWorkSum.Width = 60;
		this.gcAbsentSum.Caption = "Absent";
		this.gcAbsentSum.CurrencyDateField = null;
		this.gcAbsentSum.CurrencyField = null;
		this.gcAbsentSum.CurrencyFixed = null;
		this.gcAbsentSum.CurrencyFixedRate = 0.0;
		this.gcAbsentSum.CurrencyRateField = null;
		this.gcAbsentSum.FieldName = "AbsentSum";
		this.gcAbsentSum.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcAbsentSum.Name = "gcAbsentSum";
		this.gcAbsentSum.OptionsColumn.AllowEdit = false;
		this.gcAbsentSum.OptionsColumn.ReadOnly = true;
		this.gcAbsentSum.PrimaryKey = false;
		this.gcAbsentSum.UnitField = null;
		this.gcAbsentSum.Visible = true;
		this.gcAbsentSum.VisibleIndex = 5;
		this.gcAbsentSum.Width = 60;
		this.gcLeaveSum.Caption = "Leave";
		this.gcLeaveSum.CurrencyDateField = null;
		this.gcLeaveSum.CurrencyField = null;
		this.gcLeaveSum.CurrencyFixed = null;
		this.gcLeaveSum.CurrencyFixedRate = 0.0;
		this.gcLeaveSum.CurrencyRateField = null;
		this.gcLeaveSum.FieldName = "LeaveSum";
		this.gcLeaveSum.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcLeaveSum.Name = "gcLeaveSum";
		this.gcLeaveSum.OptionsColumn.AllowEdit = false;
		this.gcLeaveSum.OptionsColumn.ReadOnly = true;
		this.gcLeaveSum.PrimaryKey = false;
		this.gcLeaveSum.UnitField = null;
		this.gcLeaveSum.Visible = true;
		this.gcLeaveSum.VisibleIndex = 6;
		this.gcLeaveSum.Width = 60;
		this.TotalBefore.Caption = "Total Before";
		this.TotalBefore.CurrencyDateField = null;
		this.TotalBefore.CurrencyField = null;
		this.TotalBefore.CurrencyFixed = null;
		this.TotalBefore.CurrencyFixedRate = 0.0;
		this.TotalBefore.CurrencyRateField = null;
		this.TotalBefore.FieldName = "TotalBefore";
		this.TotalBefore.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.TotalBefore.Name = "TotalBefore";
		this.TotalBefore.OptionsColumn.AllowEdit = false;
		this.TotalBefore.OptionsColumn.ReadOnly = true;
		this.TotalBefore.PrimaryKey = false;
		this.TotalBefore.UnitField = null;
		this.TotalBefore.Visible = true;
		this.TotalBefore.VisibleIndex = 3;
		this.Extra.Caption = "Extra";
		this.Extra.CurrencyDateField = null;
		this.Extra.CurrencyField = null;
		this.Extra.CurrencyFixed = null;
		this.Extra.CurrencyFixedRate = 0.0;
		this.Extra.CurrencyRateField = null;
		this.Extra.FieldName = "Extra";
		this.Extra.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.Extra.Name = "Extra";
		this.Extra.PrimaryKey = false;
		this.Extra.UnitField = null;
		this.Extra.Visible = true;
		this.Extra.VisibleIndex = 7;
		this.Extra.Width = 60;
		this.WorkQuota.AppearanceHeader.Options.UseTextOptions = true;
		this.WorkQuota.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.WorkQuota.Caption = "Work Quota";
		this.WorkQuota.CurrencyDateField = null;
		this.WorkQuota.CurrencyField = null;
		this.WorkQuota.CurrencyFixed = null;
		this.WorkQuota.CurrencyFixedRate = 0.0;
		this.WorkQuota.CurrencyRateField = null;
		this.WorkQuota.FieldName = "WorkQuota";
		this.WorkQuota.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.WorkQuota.Name = "WorkQuota";
		this.WorkQuota.OptionsColumn.AllowEdit = false;
		this.WorkQuota.OptionsColumn.ReadOnly = true;
		this.WorkQuota.PrimaryKey = false;
		this.WorkQuota.UnitField = null;
		this.WorkQuota.Visible = true;
		this.WorkQuota.VisibleIndex = 8;
		this.WorkTotal.Caption = "Work Total";
		this.WorkTotal.CurrencyDateField = null;
		this.WorkTotal.CurrencyField = null;
		this.WorkTotal.CurrencyFixed = null;
		this.WorkTotal.CurrencyFixedRate = 0.0;
		this.WorkTotal.CurrencyRateField = null;
		this.WorkTotal.FieldName = "WorkTotal";
		this.WorkTotal.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.WorkTotal.Name = "WorkTotal";
		this.WorkTotal.PrimaryKey = false;
		this.WorkTotal.UnitField = null;
		this.WorkTotal.Visible = true;
		this.WorkTotal.VisibleIndex = 9;
		this.DocStatus.Caption = "DocStatus";
		this.DocStatus.CurrencyDateField = null;
		this.DocStatus.CurrencyField = null;
		this.DocStatus.CurrencyFixed = null;
		this.DocStatus.CurrencyFixedRate = 0.0;
		this.DocStatus.CurrencyRateField = null;
		this.DocStatus.FieldName = "DocStatus";
		this.DocStatus.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.DocStatus.Name = "DocStatus";
		this.DocStatus.PrimaryKey = false;
		this.DocStatus.UnitField = null;
		this.DocStatus.Width = 279;
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(93, 465);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 11;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOK.EditControl = null;
		this.btnOK.EditValue = "OK";
		this.btnOK.FieldName = null;
		this.btnOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOK.ImageOptions.Image");
		this.btnOK.IsSearchControl = false;
		this.btnOK.Location = new System.Drawing.Point(12, 465);
		this.btnOK.Name = "btnOK";
		this.btnOK.ReadOnly = false;
		this.btnOK.SettingInVisible = false;
		this.btnOK.SettingReadonly = false;
		this.btnOK.Size = new System.Drawing.Size(75, 23);
		this.btnOK.TabIndex = 10;
		this.btnOK.Text = "OK";
		this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
		this.txt_EmpId.EditControl = null;
		this.txt_EmpId.FieldName = null;
		this.txt_EmpId.IsSearchControl = true;
		this.txt_EmpId.Location = new System.Drawing.Point(97, 72);
		this.txt_EmpId.Name = "txt_EmpId";
		this.txt_EmpId.Properties.Mask.EditMask = "\\d+";
		this.txt_EmpId.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
		this.txt_EmpId.SettingInVisible = false;
		this.txt_EmpId.SettingReadonly = false;
		this.txt_EmpId.Size = new System.Drawing.Size(156, 20);
		this.txt_EmpId.TabIndex = 17;
		this.txt_EmpName.EditControl = null;
		this.txt_EmpName.FieldName = null;
		this.txt_EmpName.IsSearchControl = true;
		this.txt_EmpName.Location = new System.Drawing.Point(97, 102);
		this.txt_EmpName.Name = "txt_EmpName";
		this.txt_EmpName.SettingInVisible = false;
		this.txt_EmpName.SettingReadonly = false;
		this.txt_EmpName.Size = new System.Drawing.Size(156, 20);
		this.txt_EmpName.TabIndex = 18;
		this.luk_Period.EditControl = null;
		this.luk_Period.FieldName = null;
		this.luk_Period.IsSearchControl = true;
		this.luk_Period.Location = new System.Drawing.Point(313, 42);
		this.luk_Period.Name = "luk_Period";
		this.luk_Period.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Period.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("PeriodCode", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("PeriodName", "Name")
		});
		this.luk_Period.Properties.DropDownRows = 3;
		this.luk_Period.Properties.NullText = "";
		this.luk_Period.SettingInVisible = false;
		this.luk_Period.SettingReadonly = false;
		this.luk_Period.Size = new System.Drawing.Size(156, 20);
		this.luk_Period.TabIndex = 20;
		this.luk_Period.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_Period_EditValueChanging);
		this.ccb_Department.EditControl = null;
		this.ccb_Department.FieldName = null;
		this.ccb_Department.IsSearchControl = true;
		this.ccb_Department.Location = new System.Drawing.Point(97, 42);
		this.ccb_Department.Name = "ccb_Department";
		this.ccb_Department.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.ccb_Department.SettingInVisible = false;
		this.ccb_Department.SettingReadonly = false;
		this.ccb_Department.Size = new System.Drawing.Size(156, 20);
		this.ccb_Department.TabIndex = 22;
		this.ccb_Department.Tag = "Department";
		this.ccb_Department.Popup += new System.EventHandler(this.ccb_Department_Popup);
		this.ccb_Department.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.ccb_Status_EditValueChanging);
		this.ccb_Department.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.ccb_Status_CustomDisplayText);
		this.ccb_Status.EditControl = null;
		this.ccb_Status.FieldName = null;
		this.ccb_Status.IsSearchControl = true;
		this.ccb_Status.Location = new System.Drawing.Point(97, 9);
		this.ccb_Status.Name = "ccb_Status";
		this.ccb_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.ccb_Status.SettingInVisible = false;
		this.ccb_Status.SettingReadonly = false;
		this.ccb_Status.Size = new System.Drawing.Size(156, 20);
		this.ccb_Status.TabIndex = 23;
		this.ccb_Status.Tag = "Employee's Status";
		this.ccb_Status.Popup += new System.EventHandler(this.ccb_Status_Popup);
		this.ccb_Status.EditValueChanged += new System.EventHandler(this.ccb_Status_EditValueChanged);
		this.ccb_Status.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.ccb_Status_EditValueChanging);
		this.ccb_Status.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.ccb_Status_CustomDisplayText);
		this.btn_Search.Appearance.ForeColor = System.Drawing.Color.FromArgb(62, 61, 83);
		this.btn_Search.Appearance.Options.UseForeColor = true;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = true;
		this.btn_Search.Location = new System.Drawing.Point(265, 104);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(121, 22);
		this.btn_Search.TabIndex = 24;
		this.btn_Search.Tag = "";
		this.btn_Search.Text = "Search";
		this.btn_Search.OnClickAsync += new Apzon.Controls.Controls.OnClickAsync(this.btn_Search_Click);
		this.luk_Branch.EditControl = null;
		this.luk_Branch.FieldName = null;
		this.luk_Branch.IsSearchControl = true;
		this.luk_Branch.Location = new System.Drawing.Point(313, 9);
		this.luk_Branch.Name = "luk_Branch";
		this.luk_Branch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Branch.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_Branch.Properties.NullText = "";
		this.luk_Branch.SettingInVisible = false;
		this.luk_Branch.SettingReadonly = false;
		this.luk_Branch.Size = new System.Drawing.Size(156, 20);
		this.luk_Branch.TabIndex = 21;
		this.luk_Branch.Tag = "Branch";
		this.luk_Branch.EditValueChanged += new System.EventHandler(this.luk_Branch_EditValueChanged);
		this.luk_Branch.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_Branch_EditValueChanging);
		this.ccb_WorkDay.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.ccb_WorkDay.EditControl = null;
		this.ccb_WorkDay.FieldName = null;
		this.ccb_WorkDay.IsSearchControl = true;
		this.ccb_WorkDay.Location = new System.Drawing.Point(907, 109);
		this.ccb_WorkDay.Name = "ccb_WorkDay";
		this.ccb_WorkDay.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.ccb_WorkDay.SettingInVisible = false;
		this.ccb_WorkDay.SettingReadonly = false;
		this.ccb_WorkDay.Size = new System.Drawing.Size(135, 20);
		this.ccb_WorkDay.TabIndex = 25;
		this.ccb_WorkDay.Popup += new System.EventHandler(this.ccb_WorkDay_Popup);
		this.ccb_WorkDay.EditValueChanged += new System.EventHandler(this.checkedComboBoxEdit1_EditValueChanged);
		this.btnImport.Appearance.ForeColor = System.Drawing.Color.FromArgb(62, 61, 83);
		this.btnImport.Appearance.Options.UseForeColor = true;
		this.btnImport.EditControl = null;
		this.btnImport.EditValue = "Import From Excel";
		this.btnImport.FieldName = null;
		this.btnImport.IsSearchControl = false;
		this.btnImport.Location = new System.Drawing.Point(392, 104);
		this.btnImport.Name = "btnImport";
		this.btnImport.ReadOnly = false;
		this.btnImport.SettingInVisible = false;
		this.btnImport.SettingReadonly = false;
		this.btnImport.Size = new System.Drawing.Size(121, 22);
		this.btnImport.TabIndex = 27;
		this.btnImport.Tag = "Import Time Sheeting from Excel";
		this.btnImport.Text = "Import From Excel";
		this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
		this.grd_Export.AllowPasteExcel = false;
		this.grd_Export.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Export.ClipboardData = null;
		this.grd_Export.Location = new System.Drawing.Point(481, 7);
		this.grd_Export.MainView = this.gv_Export;
		this.grd_Export.Name = "grd_Export";
		this.grd_Export.Size = new System.Drawing.Size(170, 86);
		this.grd_Export.TabIndex = 28;
		this.grd_Export.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Export });
		this.grd_Export.Visible = false;
		this.gv_Export.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Export.ColumnKey = null;
		this.gv_Export.DisableFormSetting = false;
		this.gv_Export.DisplayException = false;
		this.gv_Export.EditColumn = null;
		this.gv_Export.FieldCurrency = null;
		this.gv_Export.FieldRate = null;
		this.gv_Export.FormId = "oTimeSheeting";
		this.gv_Export.GridControl = this.grd_Export;
		this.gv_Export.IgnoreFilter = false;
		this.gv_Export.IndexEditRow = -1;
		this.gv_Export.IndicatorWidth = 35;
		this.gv_Export.IsApzTable = false;
		this.gv_Export.IsInsertNewRow = false;
		this.gv_Export.IsLoadingData = false;
		this.gv_Export.IsMappingContextAction = false;
		this.gv_Export.ItemId = "OEXPORT";
		this.gv_Export.LstColumnAllowBatch = null;
		this.gv_Export.LstColumnNotAllowBatch = null;
		this.gv_Export.LstColumnNotEditable = null;
		this.gv_Export.LstColumNotSetting = null;
		this.gv_Export.Name = "gv_Export";
		this.gv_Export.UseBatchMod = false;
		this.btnExport.Appearance.ForeColor = System.Drawing.Color.FromArgb(62, 61, 83);
		this.btnExport.Appearance.Options.UseForeColor = true;
		this.btnExport.EditControl = null;
		this.btnExport.EditValue = "Export Template To Excel";
		this.btnExport.FieldName = null;
		this.btnExport.IsSearchControl = false;
		this.btnExport.Location = new System.Drawing.Point(519, 104);
		this.btnExport.Name = "btnExport";
		this.btnExport.ReadOnly = false;
		this.btnExport.SettingInVisible = false;
		this.btnExport.SettingReadonly = false;
		this.btnExport.Size = new System.Drawing.Size(132, 22);
		this.btnExport.TabIndex = 29;
		this.btnExport.Tag = "Export To Excel";
		this.btnExport.Text = "Export Template To Excel";
		this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
		this.luk_OvertimeType.EditControl = null;
		this.luk_OvertimeType.FieldName = null;
		this.luk_OvertimeType.IsSearchControl = true;
		this.luk_OvertimeType.Location = new System.Drawing.Point(313, 75);
		this.luk_OvertimeType.Name = "luk_OvertimeType";
		this.luk_OvertimeType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_OvertimeType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_OvertimeType.Properties.NullText = "";
		this.luk_OvertimeType.SettingInVisible = false;
		this.luk_OvertimeType.SettingReadonly = false;
		this.luk_OvertimeType.Size = new System.Drawing.Size(156, 20);
		this.luk_OvertimeType.TabIndex = 48;
		this.luk_OvertimeType.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_OvertimeType_EditValueChanging);
		this.labelLine1.Appearance.ForeColor = System.Drawing.Color.FromArgb(62, 61, 83);
		this.labelLine1.Appearance.Options.UseForeColor = true;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Branch";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(262, 12);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(54, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 49;
		this.labelLine1.Utext = "Branch";
		this.labelLine1.Load += new System.EventHandler(this.labelLine1_Load);
		this.labelLine2.Appearance.ForeColor = System.Drawing.Color.FromArgb(62, 61, 83);
		this.labelLine2.Appearance.Options.UseForeColor = true;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Department";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(12, 45);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(104, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 50;
		this.labelLine2.Utext = "Department";
		this.labelLine3.Appearance.ForeColor = System.Drawing.Color.FromArgb(62, 61, 83);
		this.labelLine3.Appearance.Options.UseForeColor = true;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Employee No.";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(12, 75);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(104, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 50;
		this.labelLine3.Utext = "Employee No.";
		this.labelLine4.Appearance.ForeColor = System.Drawing.Color.FromArgb(62, 61, 83);
		this.labelLine4.Appearance.Options.UseForeColor = true;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Employee Name";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(12, 105);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(104, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 51;
		this.labelLine4.Tag = "Employee Name";
		this.labelLine4.Utext = "Employee Name";
		this.labelLine5.Appearance.ForeColor = System.Drawing.Color.FromArgb(62, 61, 83);
		this.labelLine5.Appearance.Options.UseForeColor = true;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Status";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(12, 12);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(104, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 52;
		this.labelLine5.Utext = "Status";
		this.labelLine5.Load += new System.EventHandler(this.labelLine5_Load);
		this.labelLine6.Appearance.ForeColor = System.Drawing.Color.FromArgb(62, 61, 83);
		this.labelLine6.Appearance.Options.UseForeColor = true;
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Period";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(262, 45);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(54, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 53;
		this.labelLine6.Utext = "Period";
		this.lbl_OT.Appearance.ForeColor = System.Drawing.Color.FromArgb(62, 61, 83);
		this.lbl_OT.Appearance.Options.UseForeColor = true;
		this.lbl_OT.EditControl = null;
		this.lbl_OT.EditValue = "OT Type";
		this.lbl_OT.FieldName = null;
		this.lbl_OT.HideWhenEmpty = false;
		this.lbl_OT.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_OT.Location = new System.Drawing.Point(262, 78);
		this.lbl_OT.Name = "lbl_OT";
		this.lbl_OT.ReadOnly = false;
		this.lbl_OT.SettingInVisible = false;
		this.lbl_OT.SettingReadonly = false;
		this.lbl_OT.ShowUnderLine = true;
		this.lbl_OT.Size = new System.Drawing.Size(54, 20);
		this.lbl_OT.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_OT.TabIndex = 54;
		this.lbl_OT.Utext = "OT Type";
		this.lbl_OT.Load += new System.EventHandler(this.lbl_OT_Load);
		this.labelLine7.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine7.Appearance.ForeColor = System.Drawing.Color.FromArgb(62, 61, 83);
		this.labelLine7.Appearance.Options.UseForeColor = true;
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "Date Filter";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(850, 112);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(66, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 55;
		this.labelLine7.Utext = "Date Filter";
		this.btn_ClosePeriod.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_ClosePeriod.Appearance.Font = new System.Drawing.Font("Consolas", 8.25f, System.Drawing.FontStyle.Bold);
		this.btn_ClosePeriod.Appearance.ForeColor = System.Drawing.Color.FromArgb(62, 61, 83);
		this.btn_ClosePeriod.Appearance.Options.UseFont = true;
		this.btn_ClosePeriod.Appearance.Options.UseForeColor = true;
		this.btn_ClosePeriod.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_ClosePeriod.EditControl = null;
		this.btn_ClosePeriod.EditValue = "CLOSE PERIOD";
		this.btn_ClosePeriod.Enabled = false;
		this.btn_ClosePeriod.FieldName = null;
		this.btn_ClosePeriod.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ClosePeriod.ImageOptions.Image");
		this.btn_ClosePeriod.IsSearchControl = false;
		this.btn_ClosePeriod.Location = new System.Drawing.Point(925, 7);
		this.btn_ClosePeriod.Name = "btn_ClosePeriod";
		this.btn_ClosePeriod.ReadOnly = false;
		this.btn_ClosePeriod.SettingInVisible = false;
		this.btn_ClosePeriod.SettingReadonly = false;
		this.btn_ClosePeriod.Size = new System.Drawing.Size(117, 22);
		this.btn_ClosePeriod.TabIndex = 56;
		this.btn_ClosePeriod.Text = "CLOSE PERIOD";
		this.btn_ClosePeriod.Visible = false;
		this.btn_ClosePeriod.Click += new System.EventHandler(this.btn_ClosePeriod_Click);
		this.btn_CancelTimeSheet.Appearance.ForeColor = System.Drawing.Color.FromArgb(62, 61, 83);
		this.btn_CancelTimeSheet.Appearance.Options.UseForeColor = true;
		this.btn_CancelTimeSheet.EditControl = null;
		this.btn_CancelTimeSheet.EditValue = "Cancel TimeSheet";
		this.btn_CancelTimeSheet.FieldName = null;
		this.btn_CancelTimeSheet.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_CancelTimeSheet.ImageOptions.Image");
		this.btn_CancelTimeSheet.IsSearchControl = true;
		this.btn_CancelTimeSheet.Location = new System.Drawing.Point(657, 104);
		this.btn_CancelTimeSheet.Name = "btn_CancelTimeSheet";
		this.btn_CancelTimeSheet.ReadOnly = false;
		this.btn_CancelTimeSheet.SettingInVisible = false;
		this.btn_CancelTimeSheet.SettingReadonly = false;
		this.btn_CancelTimeSheet.Size = new System.Drawing.Size(121, 22);
		this.btn_CancelTimeSheet.TabIndex = 57;
		this.btn_CancelTimeSheet.Tag = "";
		this.btn_CancelTimeSheet.Text = "Cancel TimeSheet";
		this.btn_CancelTimeSheet.Click += new System.EventHandler(this.btn_CancelTimeSheet_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1049, 500);
		base.Controls.Add(this.btn_CancelTimeSheet);
		base.Controls.Add(this.btn_ClosePeriod);
		base.Controls.Add(this.ccb_WorkDay);
		base.Controls.Add(this.labelLine7);
		base.Controls.Add(this.luk_OvertimeType);
		base.Controls.Add(this.lbl_OT);
		base.Controls.Add(this.luk_Period);
		base.Controls.Add(this.labelLine6);
		base.Controls.Add(this.ccb_Status);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.txt_EmpName);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.txt_EmpId);
		base.Controls.Add(this.ccb_Department);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.luk_Branch);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btnExport);
		base.Controls.Add(this.grd_Export);
		base.Controls.Add(this.btnImport);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btnOK);
		base.Controls.Add(this.gridControl1);
		base.FormId = "oTimeSheeting";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_TimeSheeting";
		base.ShowInTaskbar = false;
		this.Text = "Time Sheeting";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_JVApplicants_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.gridControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DrillDownEmp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpId.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Period.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ccb_Department.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ccb_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ccb_WorkDay.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Export).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Export).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_OvertimeType.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
