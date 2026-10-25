using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources.Salary;

public class frm_SalaryCalculation : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private readonly List<Common.Table_Code> _lstAllowanceType = new List<Common.Table_Code>();

	private bool _isChanging;

	private readonly string _pathFile = Application.StartupPath + "\\Report\\" + GlobalData.DatabaseName + "\\";

	private IContainer components;

	private LabelLine lbl_Department;

	private LabelLine lbl_Branch;

	private LabelLine labelLine3;

	private TextEditEx txt_EmpNo;

	private TextEditEx txt_EmpName;

	private LabelLine lbl_EmpNo;

	private LabelLine lbl_FirstName;

	public SimpleButtonEx btnSearch;

	public SimpleButtonEx btn_Calculate;

	public SimpleButtonEx btn_Cancel;

	private LookUpEditCombo luk_Period;

	private LabelLine lbl_Period;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridColumnEx gridColumn7;

	private RepositoryItemLookUpEdit rep_DeptAllowance;

	private GridColumnEx gc_empNo;

	private GridColumnEx gridColumn9;

	private CheckedComboBoxEditEx luk_Status;

	private LookUpEditCombo luk_Branch;

	private GridColumnEx gridColumn1;

	private RepositoryItemCheckEdit rep_Checked;

	public SimpleButtonEx btn_SelectAll;

	private SeparatorControl separatorControl1;

	public SimpleButtonEx btn_View;

	public SimpleButtonEx btn_Formula;

	private LookUpEditCombo luk_Department;

	private RepositoryItemButtonEditCFL rep_EmpNo;

	private GridColumnEx gridColumnEx2;

	private RepositoryItemLookUpEdit rep_Branch;

	private GridColumnEx gridColumnEx1;

	private RepositoryItemLookUpEdit rep_Status;

	public SimpleButtonEx btn_Import;

	public frm_SalaryCalculation()
	{
		InitializeComponent();
		FormId = "oSalaryCalculation";
		gv_Doc.ItemId = "OHEM";
		gv_Doc.FormId = "oSalaryCalculation";
		OnLoadAsync += frm_SalaryCalCulation_Load;
	}

	private async Task frm_SalaryCalCulation_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			await LoadInitForm();
			await LoadSearchSetting();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_SalaryInfo_FormClosing(object sender, FormClosingEventArgs e)
	{
	}

	private void gv_MainInfo_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator && e.RowHandle >= 0)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btnSearch_Click(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(luk_Branch.EditValue)))
			{
				grd_Doc.DataSource = null;
				grd_Doc.RefreshDataSource();
				luk_Period.Properties.DataSource = null;
			}
			else
			{
				await LoadData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SaveSearchSetting()
	{
		try
		{
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_SEARCHSETTING");
			DataRow dataRow = tableStructure.NewRow();
			dataRow["FormId"] = Function.ToString(FormId);
			dataRow["ItemId"] = Function.ToString(gv_Doc.ItemId);
			dataRow["UserSign"] = Function.ToString(GlobalData.UserCodeLogin);
			dataRow["Branch"] = Function.ParseShort(luk_Branch.EditValue);
			dataRow["Period"] = Function.ToString(luk_Period.EditValue);
			dataRow["Status"] = Function.ToString(luk_Status.EditValue);
			dataRow["Dept"] = Function.ParseShort(luk_Department.EditValue);
			dataRow["EmpId"] = Function.ParseInt(txt_EmpNo.EditValue);
			dataRow["EmpName"] = Function.ToString(txt_EmpName.EditValue);
			tableStructure.Rows.Add(dataRow);
			string text = await _httpClient.SearchSetting.SaveData(tableStructure);
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void luk_Branch_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			grd_Doc.DataSource = null;
			grd_Doc.RefreshDataSource();
			if (Function.ParseInt(luk_Branch.EditValue) == 0)
			{
				luk_Period.EditValue = null;
				return;
			}
			int branch = Function.ParseInt(luk_Branch.EditValue);
			DataTable workPeriodsByBranch = _httpClient.SalaryInfoAdvanceClient.GetWorkPeriodsByBranch(branch);
			luk_Period.Properties.DataSource = null;
			EnumerableRowCollection<DataRow> source = from t in workPeriodsByBranch.AsEnumerable()
				where Function.ToString(t["status"]).Equals("C")
				select t;
			DataTable dataTable = workPeriodsByBranch.Clone();
			if (source.Any())
			{
				dataTable = source.CopyToDataTable();
			}
			workPeriodsByBranch.AcceptChanges();
			luk_Period.Properties.DataSource = dataTable.Copy();
			luk_Period.Properties.DisplayMember = "Name";
			luk_Period.Properties.ValueMember = "Code";
			luk_Period.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			luk_Period.Properties.ForceInitialize();
			luk_Period.Properties.PopulateColumns();
			luk_Period.Properties.Columns["Code"].Visible = false;
			luk_Period.Properties.Columns["status"].Visible = false;
			luk_Period.EditValue = null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Calculate_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = luk_Period.Properties.DataSource as DataTable;
			string selectedPeriod = Function.ToString(luk_Period.EditValue);
			if (dataTable == null)
			{
				return;
			}
			DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Code"]) == selectedPeriod);
			if (dataRow != null)
			{
				if (Function.ToString(dataRow["status"]) == "Y")
				{
					WinCommons.Warning(Enums.MessageCode.ThisPeriodHasBeenClosed, "");
				}
				else
				{
					NavigateToResult(viewMode: false);
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.CannotGetPeriodInfomation, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_SelectAll_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			if (!dataSourceTable.IsNotNull())
			{
				return;
			}
			if (btn_SelectAll.Text == "Select All")
			{
				foreach (DataRow item in dataSourceTable.AsEnumerable())
				{
					item["Checked"] = true;
				}
				btn_SelectAll.Text = "Unselect All";
				return;
			}
			foreach (DataRow item2 in dataSourceTable.AsEnumerable())
			{
				item2["Checked"] = false;
			}
			btn_SelectAll.Text = "Select All";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NavigateToResult(bool viewMode, bool readOnly = false)
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(luk_Branch.EditValue)))
			{
				WinCommons.Warning(Enums.MessageCode.PleaseSelectedABranch, "");
				return;
			}
			if (string.IsNullOrEmpty(Function.ToString(luk_Period.EditValue)))
			{
				WinCommons.Warning(Enums.MessageCode.PleaseSelectPeriod2Continue, "");
				return;
			}
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			EnumerableRowCollection<DataRow> source = from t in dataSourceTable.AsEnumerable()
				where Function.ParseBool(t["Checked"])
				select t;
			string seed = "";
			if (dataSourceTable.IsNotNull() && source.Any())
			{
				seed = source.Aggregate(seed, (string current, DataRow row) => current + Function.ToString(row["empID"]) + ",");
				if (!viewMode)
				{
					frm_SalaryResult frm_SalaryResult2 = new frm_SalaryResult();
					frm_SalaryResult2.MdiParent = MdiParent;
					frm_SalaryResult2.Period = Function.ToString(luk_Period.EditValue);
					frm_SalaryResult2.Dept = Function.ParseInt(luk_Department.EditValue);
					frm_SalaryResult2.Branch = Function.ParseInt(luk_Branch.EditValue);
					frm_SalaryResult2.ListEmpNo = seed.Substring(0, seed.Length - 1);
					frm_SalaryResult2.ReadOnlyResult = readOnly;
					frm_SalaryResult2.Show();
				}
				else
				{
					frm_SalaryViewResult frm_SalaryViewResult2 = new frm_SalaryViewResult();
					frm_SalaryViewResult2.MdiParent = MdiParent;
					frm_SalaryViewResult2.Period = Function.ToString(luk_Period.EditValue);
					frm_SalaryViewResult2.Dept = Function.ParseInt(luk_Department.EditValue);
					frm_SalaryViewResult2.Branch = Function.ParseInt(luk_Branch.EditValue);
					frm_SalaryViewResult2.ListEmpNo = seed.Substring(0, seed.Length - 1);
					frm_SalaryViewResult2.ReadOnlyResult = readOnly;
					frm_SalaryViewResult2.Show();
				}
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.NoSalesEmployeeIsSelected, "");
			}
			SaveSearchSetting();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_View_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = luk_Period.Properties.DataSource as DataTable;
			string selectedPeriod = Function.ToString(luk_Period.EditValue);
			if (dataTable == null)
			{
				return;
			}
			DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Code"]) == selectedPeriod);
			if (dataRow != null)
			{
				if (Function.ToString(dataRow["status"]) == "Y")
				{
					NavigateToResult(viewMode: true, readOnly: true);
				}
				else
				{
					NavigateToResult(viewMode: true);
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.CannotGetPeriodInfomation, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Formula_Click(object sender, EventArgs e)
	{
		frm_SalaryResult frm_SalaryResult2 = new frm_SalaryResult();
		frm_SalaryFormula frm_SalaryFormula2 = new frm_SalaryFormula(frm_SalaryResult2, frm_SalaryResult2.GetType().Name);
		frm_SalaryFormula2.Branch = Function.ParseInt(luk_Branch.EditValue);
		frm_SalaryFormula2.Dept = Function.ParseInt(luk_Department.EditValue);
		frm_SalaryFormula2.MdiParent = MdiParent;
		frm_SalaryFormula2.Show();
	}

	private async Task LoadData()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("branch", typeof(int));
			dataTable.Columns.Add("dept", typeof(string));
			dataTable.Columns.Add("empId", typeof(int));
			dataTable.Columns.Add("empName", typeof(string));
			dataTable.Columns.Add("status", typeof(string));
			dataTable.Columns.Add("Period", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["branch"] = Function.ParseInt(luk_Branch.EditValue);
			dataRow["dept"] = Function.ToString(luk_Department.EditValue);
			dataRow["empId"] = Function.ParseInt(txt_EmpNo.EditValue);
			dataRow["empName"] = Function.ToString(txt_EmpName.EditValue);
			dataRow["status"] = Function.ToString(luk_Status.EditValue);
			dataRow["Period"] = Function.ToString(luk_Period.EditValue);
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataTable dataTable2 = await _httpClient.SalaryCalculationClient.SearchEmployees(dataTable);
			dataTable2.Columns.Add("Checked", typeof(bool));
			foreach (DataRow item in dataTable2.AsEnumerable())
			{
				item["Checked"] = true;
			}
			btn_SelectAll.Text = "Unselect All";
			dataTable2.AcceptChanges();
			grd_Doc.DataSource = dataTable2.Copy();
			grd_Doc.RefreshDataSource();
			gv_Doc.ExpandAllGroups();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadInitForm()
	{
		_ = 3;
		try
		{
			gv_Doc.CustomDrawRowIndicator += gv_MainInfo_CustomDrawRowIndicator;
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadComboDept();
			DataRow row = dataTable.NewRow();
			dataTable.Rows.InsertAt(row, 0);
			dataTable.AcceptChanges();
			luk_Department.Properties.DataSource = dataTable.Copy();
			luk_Department.Properties.DisplayMember = "Name";
			luk_Department.Properties.ValueMember = "Code";
			luk_Department.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			luk_Department.Properties.ForceInitialize();
			luk_Department.Properties.PopulateColumns();
			luk_Department.Properties.Columns["Code"].Visible = false;
			luk_Department.Properties.Columns["Remarks"].Visible = false;
			rep_DeptAllowance.DataSource = dataTable.Copy();
			rep_DeptAllowance.DisplayMember = "Name";
			rep_DeptAllowance.ValueMember = "Code";
			rep_DeptAllowance.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_DeptAllowance.ForceInitialize();
			rep_DeptAllowance.PopulateColumns();
			rep_DeptAllowance.Columns["Code"].Visible = false;
			rep_DeptAllowance.Columns["Remarks"].Visible = false;
			DataTable dataTable2 = await _httpClient.EmployeeMasterClient.GetEmployeeStatus();
			luk_Status.Properties.DataSource = dataTable2.Copy();
			luk_Status.Properties.DisplayMember = "Name";
			luk_Status.Properties.ValueMember = "Code";
			rep_Status.DataSource = dataTable2.Copy();
			rep_Status.DisplayMember = "Name";
			rep_Status.ValueMember = "Code";
			rep_Status.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Status.ForceInitialize();
			rep_Status.PopulateColumns();
			rep_Status.Columns["Code"].Visible = false;
			DataTable dataTable3 = await _httpClient.EmployeeMasterClient.LoadComboBranch();
			dataTable3.AcceptChanges();
			luk_Branch.Properties.DataSource = dataTable3.Copy();
			luk_Branch.Properties.DisplayMember = "Name";
			luk_Branch.Properties.ValueMember = "Code";
			luk_Branch.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			luk_Branch.Properties.ForceInitialize();
			luk_Branch.Properties.PopulateColumns();
			luk_Branch.Properties.Columns["Code"].Visible = false;
			rep_Branch.DataSource = dataTable3.Copy();
			rep_Branch.DisplayMember = "Name";
			rep_Branch.ValueMember = "Code";
			rep_Branch.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Branch.ForceInitialize();
			rep_Branch.PopulateColumns();
			rep_Branch.Columns["Code"].Visible = false;
			foreach (DataRow row2 in (await _httpClient.AllowanceType.GetAllowanceType()).Rows)
			{
				Common.Table_Code item = new Common.Table_Code
				{
					Code = Function.ToString(row2["Code"]),
					Name = Function.ToString(row2["Name"])
				};
				_lstAllowanceType.Add(item);
			}
			rep_EmpNo.Buttons[0].Visible = false;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = rep_EmpNo;
			repositoryItemButtonEditCFL.DrillDownMethod = (Action<string>)Delegate.Combine(repositoryItemButtonEditCFL.DrillDownMethod, (Action<string>)((string s) =>
			{
				this.DrillDown(ApzObjectTypes.oEmployeesInfo, s);
			}));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void NoAuthSetControlSearch()
	{
		SimpleButtonEx simpleButtonEx = btnSearch;
		SimpleButtonEx simpleButtonEx2 = btn_View;
		bool flag = (btn_Formula.Enabled = true);
		bool enabled = (simpleButtonEx2.Enabled = flag);
		simpleButtonEx.Enabled = enabled;
		TextEditEx textEditEx = txt_EmpNo;
		TextEditEx textEditEx2 = txt_EmpName;
		LookUpEditCombo lookUpEditCombo = luk_Branch;
		LookUpEditCombo lookUpEditCombo2 = luk_Department;
		CheckedComboBoxEditEx checkedComboBoxEditEx = luk_Status;
		bool flag4 = (luk_Period.ReadOnly = false);
		bool flag6 = (checkedComboBoxEditEx.ReadOnly = flag4);
		bool flag8 = (lookUpEditCombo2.ReadOnly = flag6);
		flag = (lookUpEditCombo.ReadOnly = flag8);
		enabled = (textEditEx2.ReadOnly = flag);
		textEditEx.ReadOnly = enabled;
	}

	private async Task LoadSearchSetting()
	{
		try
		{
			DataTable dataTable = await WinFuntions.LoadSearchSetting(FormId, gv_Doc.ItemId);
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.Rows[0];
				luk_Branch.EditValue = Function.ParseShort(dataRow["Branch"]);
				if (Function.ParseInt(dataRow["Dept"]) != 0)
				{
					luk_Department.EditValue = Function.ParseInt(dataRow["Dept"]);
				}
				luk_Period.EditValue = Function.ToString(dataRow["Period"]);
				luk_Status.EditValue = Function.ToString(dataRow["Status"]);
				luk_Status.RefreshEditValue();
				if (Function.ParseInt(dataRow["EmpId"]) > 0)
				{
					txt_EmpNo.EditValue = Function.ParseInt(dataRow["EmpId"]);
				}
				txt_EmpName.EditValue = Function.ToString(dataRow["EmpName"]);
			}
			else
			{
				luk_Branch.EditValue = Function.ParseShort(-2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Import_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Multiselect = false,
				Filter = "Report file(.rpt) | *.rpt"
			};
			if (openFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			string[] files = openFileDialog.FileNames;
			if (files.Length != 0)
			{
				string text = await _httpClient.FileNetworkManagerClient.UpLoadFile("\\Report\\", true, files[0]);
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
					return;
				}
				DataSet dataSet = new DataSet();
				DataTable tableStructure = WinFuntions.GetTableStructure("APZ_SAL5");
				tableStructure.Rows.Add(1, Path.GetFileName(files[0]));
				dataSet.Tables.Add(tableStructure.Copy());
				string text2 = await _httpClient.SalaryCalculationClient.SaveRptFileName(dataSet);
				if (string.IsNullOrEmpty(text2))
				{
					File.Copy(Path.GetFullPath(files[0]), _pathFile + Path.GetFileName(files[0]), overwrite: true);
					WinCommons.Success(Enums.MessageCode.ImportSuccess, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text2);
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.ImportRptFileFailed, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_Department_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			(grd_Doc.DataSource as DataTable)?.Rows.Clear();
			grd_Doc.RefreshDataSource();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.Salary.frm_SalaryCalculation));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.lbl_Department = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Branch = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_EmpNo = new Apzon.Controls.Editor.TextEditEx();
		this.txt_EmpName = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_EmpNo = new Apzon.Controls.Controls.LabelLine();
		this.lbl_FirstName = new Apzon.Controls.Controls.LabelLine();
		this.btnSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Calculate = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.luk_Period = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.lbl_Period = new Apzon.Controls.Controls.LabelLine();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Checked = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Branch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_DeptAllowance = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_empNo = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_EmpNo = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Status = new Apzon.Controls.Editor.CheckedComboBoxEditEx();
		this.luk_Branch = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.btn_SelectAll = new Apzon.Controls.Controls.SimpleButtonEx();
		this.separatorControl1 = new DevExpress.XtraEditors.SeparatorControl();
		this.btn_View = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Formula = new Apzon.Controls.Controls.SimpleButtonEx();
		this.luk_Department = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.btn_Import = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Period.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Checked).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DeptAllowance).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_EmpNo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.separatorControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Department.Properties).BeginInit();
		base.SuspendLayout();
		this.lbl_Department.FieldName = null;
		this.lbl_Department.HideWhenEmpty = false;
		this.lbl_Department.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Department.Location = new System.Drawing.Point(4, 39);
		this.lbl_Department.Name = "lbl_Department";
		this.lbl_Department.ShowUnderLine = true;
		this.lbl_Department.Size = new System.Drawing.Size(117, 20);
		this.lbl_Department.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Department.TabIndex = 74;
		this.lbl_Department.Utext = "Department";
		this.lbl_Branch.FieldName = null;
		this.lbl_Branch.HideWhenEmpty = false;
		this.lbl_Branch.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Branch.Location = new System.Drawing.Point(4, 13);
		this.lbl_Branch.Name = "lbl_Branch";
		this.lbl_Branch.ShowUnderLine = true;
		this.lbl_Branch.Size = new System.Drawing.Size(117, 20);
		this.lbl_Branch.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Branch.TabIndex = 76;
		this.lbl_Branch.Utext = "Branch";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(4, 65);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(117, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 85;
		this.labelLine3.Utext = "Status";
		this.txt_EmpNo.FieldName = null;
		this.txt_EmpNo.IsSearchControl = true;
		this.txt_EmpNo.Location = new System.Drawing.Point(120, 88);
		this.txt_EmpNo.Name = "txt_EmpNo";
		this.txt_EmpNo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_EmpNo.Size = new System.Drawing.Size(167, 20);
		this.txt_EmpNo.TabIndex = 4;
		this.txt_EmpNo.Tag = " ";
		this.txt_EmpName.FieldName = null;
		this.txt_EmpName.IsSearchControl = true;
		this.txt_EmpName.Location = new System.Drawing.Point(120, 114);
		this.txt_EmpName.Name = "txt_EmpName";
		this.txt_EmpName.Size = new System.Drawing.Size(167, 20);
		this.txt_EmpName.TabIndex = 5;
		this.lbl_EmpNo.FieldName = null;
		this.lbl_EmpNo.HideWhenEmpty = false;
		this.lbl_EmpNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_EmpNo.Location = new System.Drawing.Point(4, 91);
		this.lbl_EmpNo.Name = "lbl_EmpNo";
		this.lbl_EmpNo.ShowUnderLine = true;
		this.lbl_EmpNo.Size = new System.Drawing.Size(117, 20);
		this.lbl_EmpNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_EmpNo.TabIndex = 89;
		this.lbl_EmpNo.Utext = "Employee No.";
		this.lbl_FirstName.FieldName = null;
		this.lbl_FirstName.HideWhenEmpty = false;
		this.lbl_FirstName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_FirstName.Location = new System.Drawing.Point(4, 117);
		this.lbl_FirstName.Name = "lbl_FirstName";
		this.lbl_FirstName.ShowUnderLine = true;
		this.lbl_FirstName.Size = new System.Drawing.Size(117, 20);
		this.lbl_FirstName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_FirstName.TabIndex = 88;
		this.lbl_FirstName.Utext = "Employee's Name";
		this.btnSearch.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnSearch.Appearance.Options.UseBackColor = true;
		this.btnSearch.FieldName = null;
		this.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		this.btnSearch.IsSearchControl = false;
		this.btnSearch.Location = new System.Drawing.Point(188, 140);
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.Size = new System.Drawing.Size(99, 20);
		this.btnSearch.TabIndex = 6;
		this.btnSearch.Text = "Search";
		this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
		this.btn_Calculate.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Calculate.Appearance.Options.UseBackColor = true;
		this.btn_Calculate.FieldName = null;
		this.btn_Calculate.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Calculate.ImageOptions.Image");
		this.btn_Calculate.IsSearchControl = false;
		this.btn_Calculate.Location = new System.Drawing.Point(178, 262);
		this.btn_Calculate.Name = "btn_Calculate";
		this.btn_Calculate.Size = new System.Drawing.Size(109, 20);
		this.btn_Calculate.TabIndex = 9;
		this.btn_Calculate.Text = "Calculate  Salary";
		this.btn_Calculate.Click += new System.EventHandler(this.btn_Calculate_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Cancel.Appearance.Options.UseBackColor = true;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(4, 456);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(93, 20);
		this.btn_Cancel.TabIndex = 11;
		this.btn_Cancel.Text = "Close";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.luk_Period.FieldName = null;
		this.luk_Period.IsSearchControl = true;
		this.luk_Period.Location = new System.Drawing.Point(120, 212);
		this.luk_Period.Name = "luk_Period";
		this.luk_Period.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Period.Properties.IsSearchControl = false;
		this.luk_Period.Properties.NullText = "";
		this.luk_Period.Properties.PopupSizeable = false;
		this.luk_Period.Size = new System.Drawing.Size(167, 20);
		this.luk_Period.TabIndex = 7;
		this.lbl_Period.FieldName = null;
		this.lbl_Period.HideWhenEmpty = false;
		this.lbl_Period.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Period.Location = new System.Drawing.Point(7, 215);
		this.lbl_Period.Name = "lbl_Period";
		this.lbl_Period.ShowUnderLine = true;
		this.lbl_Period.Size = new System.Drawing.Size(114, 20);
		this.lbl_Period.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Period.TabIndex = 97;
		this.lbl_Period.Utext = "Period";
		this.grd_Doc.AllowPasteExcel = false;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Location = new System.Drawing.Point(293, 2);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rep_DeptAllowance, this.rep_Checked, this.rep_EmpNo, this.rep_Branch, this.rep_Status });
		this.grd_Doc.Size = new System.Drawing.Size(536, 474);
		this.grd_Doc.TabIndex = 98;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumn1, this.gridColumnEx2, this.gridColumnEx1, this.gridColumn7, this.gc_empNo, this.gridColumn9 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oSalaryCalculation";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.GroupCount = 1;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 40;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.ItemId = "OHEM";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsFind.AllowFindPanel = false;
		this.gv_Doc.OptionsFind.AlwaysVisible = true;
		this.gv_Doc.OptionsFind.SearchInPreview = true;
		this.gv_Doc.OptionsFind.ShowClearButton = false;
		this.gv_Doc.OptionsFind.ShowFindButton = false;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowGroupedColumns = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumn7, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gv_Doc.UseBatchMod = false;
		this.gridColumn1.Caption = "Select";
		this.gridColumn1.ColumnEdit = this.rep_Checked;
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "Checked";
		this.gridColumn1.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.rep_Checked.AutoHeight = false;
		this.rep_Checked.Name = "rep_Checked";
		this.gridColumnEx2.Caption = "Branch";
		this.gridColumnEx2.ColumnEdit = this.rep_Branch;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "branch";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.OptionsColumn.ReadOnly = true;
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.rep_Branch.AutoHeight = false;
		this.rep_Branch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Branch.Name = "rep_Branch";
		this.rep_Branch.NullText = "";
		this.gridColumnEx1.Caption = "Status";
		this.gridColumnEx1.ColumnEdit = this.rep_Status;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "status";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.rep_Status.AutoHeight = false;
		this.rep_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Status.Name = "rep_Status";
		this.rep_Status.NullText = "";
		this.gridColumn7.Caption = "Department";
		this.gridColumn7.ColumnEdit = this.rep_DeptAllowance;
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "dept";
		this.gridColumn7.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.OptionsColumn.ReadOnly = true;
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 1;
		this.gridColumn7.Width = 90;
		this.rep_DeptAllowance.AutoHeight = false;
		this.rep_DeptAllowance.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_DeptAllowance.Name = "rep_DeptAllowance";
		this.rep_DeptAllowance.NullText = "";
		this.gc_empNo.Caption = "Employee No.";
		this.gc_empNo.ColumnEdit = this.rep_EmpNo;
		this.gc_empNo.CurrencyDateField = null;
		this.gc_empNo.CurrencyField = null;
		this.gc_empNo.CurrencyFixed = null;
		this.gc_empNo.CurrencyFixedRate = 0.0;
		this.gc_empNo.CurrencyRateField = null;
		this.gc_empNo.FieldName = "empID";
		this.gc_empNo.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gc_empNo.Name = "gc_empNo";
		this.gc_empNo.OptionsColumn.ReadOnly = true;
		this.gc_empNo.PrimaryKey = false;
		this.gc_empNo.UnitField = null;
		this.gc_empNo.Visible = true;
		this.gc_empNo.VisibleIndex = 2;
		this.gc_empNo.Width = 159;
		this.rep_EmpNo.AutoHeight = false;
		this.rep_EmpNo.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.rep_EmpNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_EmpNo.ContainerControl = this;
		this.rep_EmpNo.DisplayMember = "";
		this.rep_EmpNo.DisplayMember2 = "";
		this.rep_EmpNo.LoadDataSourceWhenEnter = false;
		this.rep_EmpNo.Name = "rep_EmpNo";
		this.rep_EmpNo.SeparateCharacterForMulti = ';';
		this.rep_EmpNo.SeparateString = ", ";
		this.rep_EmpNo.UsingAutoCompleteBox = false;
		this.rep_EmpNo.UsingDataSource = true;
		this.rep_EmpNo.ValidateWhenNoDataSource = false;
		this.rep_EmpNo.ValueMember = "";
		this.gridColumn9.Caption = "Employee Name";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "empName";
		this.gridColumn9.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.OptionsColumn.ReadOnly = true;
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 3;
		this.gridColumn9.Width = 265;
		this.luk_Status.FieldName = null;
		this.luk_Status.IsSearchControl = true;
		this.luk_Status.Location = new System.Drawing.Point(120, 62);
		this.luk_Status.Name = "luk_Status";
		this.luk_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Status.Size = new System.Drawing.Size(167, 20);
		this.luk_Status.TabIndex = 3;
		this.luk_Branch.FieldName = null;
		this.luk_Branch.IsSearchControl = true;
		this.luk_Branch.Location = new System.Drawing.Point(120, 10);
		this.luk_Branch.Name = "luk_Branch";
		this.luk_Branch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Branch.Properties.IsSearchControl = false;
		this.luk_Branch.Properties.NullText = "";
		this.luk_Branch.Size = new System.Drawing.Size(167, 20);
		this.luk_Branch.TabIndex = 1;
		this.luk_Branch.EditValueChanged += new System.EventHandler(this.luk_Branch_EditValueChanged);
		this.btn_SelectAll.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_SelectAll.Appearance.Options.UseBackColor = true;
		this.btn_SelectAll.FieldName = null;
		this.btn_SelectAll.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_SelectAll.ImageOptions.Image");
		this.btn_SelectAll.IsSearchControl = true;
		this.btn_SelectAll.Location = new System.Drawing.Point(702, 15);
		this.btn_SelectAll.Name = "btn_SelectAll";
		this.btn_SelectAll.Size = new System.Drawing.Size(99, 20);
		this.btn_SelectAll.TabIndex = 99;
		this.btn_SelectAll.Text = "Select all";
		this.btn_SelectAll.Click += new System.EventHandler(this.btn_SelectAll_Click);
		this.separatorControl1.Location = new System.Drawing.Point(4, 166);
		this.separatorControl1.Name = "separatorControl1";
		this.separatorControl1.Size = new System.Drawing.Size(283, 23);
		this.separatorControl1.TabIndex = 100;
		this.btn_View.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_View.Appearance.Options.UseBackColor = true;
		this.btn_View.FieldName = null;
		this.btn_View.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_View.ImageOptions.Image");
		this.btn_View.IsSearchControl = true;
		this.btn_View.Location = new System.Drawing.Point(63, 262);
		this.btn_View.Name = "btn_View";
		this.btn_View.Size = new System.Drawing.Size(109, 20);
		this.btn_View.TabIndex = 8;
		this.btn_View.Text = "View  Salary";
		this.btn_View.Click += new System.EventHandler(this.btn_View_Click);
		this.btn_Formula.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Formula.Appearance.Options.UseBackColor = true;
		this.btn_Formula.FieldName = null;
		this.btn_Formula.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Formula.ImageOptions.Image");
		this.btn_Formula.IsSearchControl = false;
		this.btn_Formula.Location = new System.Drawing.Point(178, 288);
		this.btn_Formula.Name = "btn_Formula";
		this.btn_Formula.Size = new System.Drawing.Size(109, 22);
		this.btn_Formula.TabIndex = 10;
		this.btn_Formula.Text = "Set up Formula";
		this.btn_Formula.Click += new System.EventHandler(this.btn_Formula_Click);
		this.luk_Department.FieldName = null;
		this.luk_Department.IsSearchControl = true;
		this.luk_Department.Location = new System.Drawing.Point(120, 36);
		this.luk_Department.Name = "luk_Department";
		this.luk_Department.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Department.Properties.IsSearchControl = false;
		this.luk_Department.Properties.NullText = "";
		this.luk_Department.Size = new System.Drawing.Size(167, 20);
		this.luk_Department.TabIndex = 2;
		this.luk_Department.EditValueChanged += new System.EventHandler(this.luk_Department_EditValueChanged);
		this.btn_Import.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Import.Appearance.Options.UseBackColor = true;
		this.btn_Import.FieldName = null;
		this.btn_Import.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Import.ImageOptions.Image");
		this.btn_Import.IsSearchControl = false;
		this.btn_Import.Location = new System.Drawing.Point(63, 288);
		this.btn_Import.Name = "btn_Import";
		this.btn_Import.Size = new System.Drawing.Size(109, 22);
		this.btn_Import.TabIndex = 101;
		this.btn_Import.Text = "Import Salary Slip";
		this.btn_Import.Click += new System.EventHandler(this.btn_Import_Click);
		base.AcceptButton = this.btnSearch;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(833, 478);
		base.Controls.Add(this.btn_Import);
		base.Controls.Add(this.luk_Department);
		base.Controls.Add(this.btn_Formula);
		base.Controls.Add(this.btn_View);
		base.Controls.Add(this.separatorControl1);
		base.Controls.Add(this.btn_SelectAll);
		base.Controls.Add(this.luk_Status);
		base.Controls.Add(this.luk_Branch);
		base.Controls.Add(this.grd_Doc);
		base.Controls.Add(this.luk_Period);
		base.Controls.Add(this.lbl_Period);
		base.Controls.Add(this.btn_Calculate);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btnSearch);
		base.Controls.Add(this.txt_EmpNo);
		base.Controls.Add(this.txt_EmpName);
		base.Controls.Add(this.lbl_EmpNo);
		base.Controls.Add(this.lbl_FirstName);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.lbl_Branch);
		base.Controls.Add(this.lbl_Department);
		base.FormId = "oSalaryCalculation";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SalaryCalculation";
		base.ShowInTaskbar = false;
		this.Text = "Employee Salary Calculation";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_SalaryInfo_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.txt_EmpNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Period.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Checked).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DeptAllowance).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_EmpNo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.separatorControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Department.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
