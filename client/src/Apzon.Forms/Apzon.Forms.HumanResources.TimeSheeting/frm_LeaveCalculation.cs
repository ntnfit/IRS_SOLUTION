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
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources.TimeSheeting;

public class frm_LeaveCalculation : XtraFormEx
{
	public readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private DataTable dtOUBR;

	private DataTable dtAPZ_OHWP;

	private DataTable dtAPZ_OSLP;

	private DataTable dtYear;

	private IContainer components;

	private RepositoryItemLookUpEdit rep_Deparment;

	public SimpleButtonEx btCancel;

	public SimpleButtonEx btnOK;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private TextEditEx txt_LeaveAccrual;

	private LookUpEditSimple luk_ToPeriod;

	private LabelLine labelLine7;

	public LookUpEditSimple luk_Branch;

	public LookUpEditSimple luk_FromPeriod;

	public GridControlEx grd_Doc;

	public GridViewEx gv_Doc;

	public GridColumnEx EmpId;

	private GridColumnEx gcExtEmpNo;

	private GridColumnEx EmpName;

	private RepositoryItemButtonEditCFL rep_DrillDownEmp;

	private GridColumnEx gcDepart;

	private RepositoryItemLookUpEdit rep_Dept;

	private RepositoryItemLookUpEdit rep_Branch;

	private GridColumnEx gcLeaveAccrual;

	private LookUpEditEx luk_calcMethod;

	private LabelLine labelLine12;

	private PanelControl panelControl1;

	public SimpleButtonEx btn_Search;

	private CheckedComboBoxEditEx luk_Status;

	private LabelLine labelLine8;

	public TextEditEx txt_EmpName;

	private LabelLine labelLine9;

	public TextEditEx txt_EmpId;

	private LabelLine labelLine10;

	private LabelLine labelLine11;

	private GridColumnEx gridColumnEx1;

	private CheckedComboBoxEditEx luk_Department;

	public int Branch { get; set; }

	public string FromPeriod { get; set; }

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
			btnOK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btnOK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_LeaveCalculation()
	{
		InitializeComponent();
		OnLoadAsync += frm_LeaveCalculation_Load;
		FormId = "oLeaveCalculation";
		IsScrollable = true;
		gv_Doc.FormId = FormId;
		gv_Doc.ItemId = "APZ_OTSL";
		RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = rep_DrillDownEmp;
		repositoryItemButtonEditCFL.DrillDownMethodWithGridView = (Action<string, GridView>)Delegate.Combine(repositoryItemButtonEditCFL.DrillDownMethodWithGridView, (Action<string, GridView>)((string input, GridView view) =>
		{
			this.DrillDown(ApzObjectTypes.oEmployeesInfo, Function.ToString(view.GetFocusedRowCellValue("empID")));
		}));
	}

	private async Task frm_LeaveCalculation_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			await LoadInitForm();
			SetupBranch_LookUp();
			if (Branch != 0)
			{
				luk_Branch.EditValue = Function.ParseShort(Branch);
			}
			SetControlValueByBranchAndPeriod();
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("branch", typeof(int));
			dataTable.Columns.Add("dept", typeof(string));
			dataTable.Columns.Add("empId", typeof(int));
			dataTable.Columns.Add("empName", typeof(string));
			dataTable.Columns.Add("status", typeof(string));
			dataTable.Columns.Add("period", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			int num = Function.ParseInt(luk_Branch.EditValue);
			string value = Function.ToString(luk_FromPeriod.EditValue);
			if (string.IsNullOrEmpty(value))
			{
				return;
			}
			dataRow["branch"] = num;
			dataRow["period"] = value;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataTable dataTable2 = (await _httpClient.LeaveInformationClient.SearchLeaveInfo(dataTable)).Copy();
			List<int> list = new List<int>();
			for (int i = 0; i < dataTable2.Columns.Count; i++)
			{
				DataColumn dataColumn = dataTable2.Columns[i];
				if (dataColumn.ColumnName == "Period" || dataColumn.ColumnName == "WorkTotal" || dataColumn.ColumnName == "LeaveTotal" || dataColumn.ColumnName == "Used" || dataColumn.ColumnName.Contains("WorkCode_"))
				{
					list.Add(i);
				}
			}
			for (int num2 = list.Count - 1; num2 >= 0; num2--)
			{
				dataTable2.Columns.RemoveAt(list[num2]);
			}
			dataTable2.Columns.Add("LeaveAccrual", typeof(double));
			grd_Doc.DataSource = dataTable2;
			grd_Doc.RefreshDataSource();
			DataRow dataRow2 = dtAPZ_OHWP.AsEnumerable().FirstOrDefault((DataRow s) => Function.ToString(s["PeriodCode"]) == FromPeriod && Function.ParseInt(s["Branch"]) == Branch);
			if (dataRow2 != null)
			{
				double num3 = Function.ParseDouble(dataRow2["FPLTotal"]);
				if (num3 > 0.0 || (Math.Abs(num3) < double.Epsilon && dataRow2["FPLTotal"] == DBNull.Value))
				{
					luk_calcMethod.EditValue = "Y";
				}
				else
				{
					luk_calcMethod.EditValue = "M";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadInitForm()
	{
		_ = 4;
		try
		{
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadComboDept();
			luk_Department.Properties.DataSource = dataTable.Copy();
			luk_Department.Properties.DisplayMember = "Name";
			luk_Department.Properties.ValueMember = "Code";
			rep_Dept.DataSource = dataTable.Copy();
			rep_Dept.DisplayMember = "Name";
			rep_Dept.ValueMember = "Code";
			rep_Dept.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Dept.ForceInitialize();
			rep_Dept.PopulateColumns();
			rep_Dept.Columns["Code"].Visible = false;
			rep_Dept.Columns["Remarks"].Visible = false;
			DataTable dataTable2 = await _httpClient.EmployeeMasterClient.GetEmployeeStatus();
			luk_Status.Properties.DataSource = dataTable2.Copy();
			luk_Status.Properties.DisplayMember = "Name";
			luk_Status.Properties.ValueMember = "Code";
			DataTable dataTable3 = await _httpClient.EmployeeMasterClient.LoadComboBranch();
			DataRow row = dataTable3.NewRow();
			dataTable3.Rows.InsertAt(row, 0);
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
			dtAPZ_OHWP = await GetPeriod();
			dtAPZ_OSLP = await GetLeaveParameters();
			rep_DrillDownEmp.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			rep_DrillDownEmp.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			List<Common.Table_Code> list = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "M",
					Name = "Increase Leave"
				},
				new Common.Table_Code
				{
					Code = "Y",
					Name = "Reset Leave"
				}
			};
			luk_calcMethod.Properties.DataSource = list;
			luk_calcMethod.Properties.DisplayMember = "Name";
			luk_calcMethod.Properties.ValueMember = "Code";
			luk_calcMethod.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			luk_calcMethod.Properties.PopulateColumns();
			luk_calcMethod.Properties.Columns["Code"].Visible = false;
			luk_calcMethod.Properties.DropDownRows = list.Count;
			luk_calcMethod.Properties.PopupWidth = luk_calcMethod.Width;
			luk_calcMethod.Properties.ShowHeader = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetControlValueByBranchAndPeriod()
	{
		try
		{
			if (Function.ToString(FromPeriod) == "")
			{
				panelControl1.Visible = true;
				return;
			}
			panelControl1.Visible = false;
			luk_FromPeriod.EditValue = FromPeriod;
			DataRow dataRow = dtAPZ_OHWP.AsEnumerable().FirstOrDefault((DataRow s) => Function.ToString(s["PeriodCode"]) == FromPeriod && Function.ParseInt(s["Branch"]) == Branch);
			DateTime currentPeriodFromDate = DateTime.MaxValue;
			if (dataRow != null)
			{
				currentPeriodFromDate = Function.ParseDateTimes(dataRow["FromDate"]);
			}
			DataRow[] array = (from dr in dtAPZ_OHWP.AsEnumerable()
				where Function.ParseInt(dr["Branch"]) == Branch && Function.ToString(dr["PeriodStatus"]) == "U" && Function.ParseDateTimes(dr["FromDate"]) > currentPeriodFromDate
				orderby dr["FromDate"]
				select dr).ToArray();
			if (array.Length != 0)
			{
				DataRow dataRow2 = array[0];
				luk_ToPeriod.EditValue = Function.ToString(dataRow2["PeriodCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetBranch()
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

	private async Task<DataTable> GetPeriod()
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

	private async Task<DataTable> GetLeaveParameters()
	{
		_ = 1;
		try
		{
			DataTable dataTable = await _httpClient.LeaveParameterClient.GetDoc();
			if (!dataTable.HasColumn())
			{
				dataTable = await WinFuntions.GetTableStructureAsync("APZ_OSLP");
			}
			DataView defaultView = dataTable.DefaultView;
			defaultView.RowFilter = "status='Y'";
			return defaultView.ToTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private void SetupBranch_LookUp()
	{
		try
		{
			if (dtOUBR != null && dtOUBR.Copy().Rows.Count == 2)
			{
				luk_Branch.ItemIndex = 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void PeriodFilter()
	{
		int num = Function.ParseInt(luk_Branch.EditValue);
		if (dtAPZ_OHWP != null)
		{
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
			luk_FromPeriod.Properties.DataSource = dataTable.Copy();
			luk_FromPeriod.Properties.DisplayMember = "PeriodName";
			luk_FromPeriod.Properties.ValueMember = "PeriodCode";
			if (luk_FromPeriod.Text == "")
			{
				luk_FromPeriod.EditValue = "";
			}
			luk_ToPeriod.Properties.ValueMember = "PeriodCode";
			luk_ToPeriod.Properties.DisplayMember = "PeriodName";
			luk_ToPeriod.Properties.DataSource = dataTable.Copy();
			if (luk_ToPeriod.Text == "")
			{
				luk_ToPeriod.EditValue = "";
			}
		}
	}

	private void luk_Branch_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			PeriodFilter();
			DataTable dataTable = grd_Doc.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				dataTable.Rows.Clear();
				grd_Doc.DataSource = dataTable;
				grd_Doc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btnOK_Click(object sender, EventArgs e)
	{
		_ = 4;
		try
		{
			btnOK.Enabled = false;
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("branch", typeof(int));
			dataTable.Columns.Add("dept", typeof(string));
			dataTable.Columns.Add("empId", typeof(int));
			dataTable.Columns.Add("empName", typeof(string));
			dataTable.Columns.Add("status", typeof(string));
			dataTable.Columns.Add("period", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			int _branch = Function.ParseInt(luk_Branch.EditValue);
			string _fromPeriod = Function.ToString(luk_FromPeriod.EditValue);
			if (string.IsNullOrWhiteSpace(_fromPeriod) || _branch == 0)
			{
				WinCommons.Warning(Enums.MessageCode.FromPeriodIsEmpty, "");
				return;
			}
			dataRow["branch"] = _branch;
			dataRow["period"] = _fromPeriod;
			dataRow["dept"] = Function.ToString(luk_Department.EditValue);
			dataRow["empId"] = Function.ParseInt(txt_EmpId.EditValue);
			dataRow["empName"] = Function.ToString(txt_EmpName.EditValue);
			dataRow["status"] = Function.ToString(luk_Status.EditValue);
			string _toPeriod = Function.ToString(luk_ToPeriod.EditValue);
			if (string.IsNullOrWhiteSpace(_toPeriod))
			{
				WinCommons.Warning(Enums.MessageCode.ToPeriodIsEmpty, "");
				return;
			}
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataTable dataTable2 = await _httpClient.LeaveInformationClient.SearchLeaveInfo(dataTable);
			if (dataTable2.EmptyData())
			{
				WinCommons.Error(Enums.MessageCode.UnableToCalculateLeaveNoData4PeriodWasFound, "", _fromPeriod);
				return;
			}
			DataTable dataTable3 = grd_Doc.DataSource as DataTable;
			if (!dataTable3.IsNotNull())
			{
				return;
			}
			foreach (DataRow dr in dataTable2.AsEnumerable())
			{
				dr["Period"] = _toPeriod;
				Function.ParseDouble(dr["Remain"]);
				DataRow dataRow2 = dataTable3.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["empID"]) == Function.ParseInt(dr["empID"]) && Function.ParseInt(t["empID"]) != 0);
				if (dataRow2 != null && (Function.ToString(luk_calcMethod.EditValue) == "M" || Function.ToString(luk_calcMethod.EditValue) == "Y"))
				{
					double num = Function.ParseDouble(dataRow2["LeaveAccrual"]);
					dr["LeaveTotal"] = num;
					dr["Used"] = 0;
					dr["Remain"] = num;
				}
			}
			if (dataTable2.ExistsColumn("dept"))
			{
				dataTable2.Columns.Remove("dept");
			}
			if (dataTable2.ExistsColumn("branch"))
			{
				dataTable2.Columns.Remove("branch");
			}
			if (dataTable2.ExistsColumn("status"))
			{
				dataTable2.Columns.Remove("status");
			}
			if (dataTable2.ExistsColumn("WorkTotal"))
			{
				dataTable2.Columns.Remove("WorkTotal");
			}
			for (int num2 = 0; num2 < dataTable2.Rows.Count; num2++)
			{
				DataRow dataRow3 = dataTable2.Rows[num2];
				dataRow3["UserSign2"] = GlobalData.UserID;
				dataRow3["UpdateDate"] = DateTime.Today;
			}
			dataTable2.AcceptChanges();
			HttpResult httpResult = await _httpClient.LeaveInformationClient.SaveLeaveInformation(dataTable2);
			if (httpResult != null && httpResult.Message != "")
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return;
			}
			DataTable dtOldOT = await _httpClient.TimeSheetingClient.LoadOvertimeLeave(_fromPeriod, _branch);
			dtOldOT.SetPrimaryKey("Period", "EmpId");
			DataTable dtCalculateOT = dtOldOT.Clone();
			DataTable dataTable4 = await _httpClient.TimeSheetingClient.LoadOvertimeNew(_fromPeriod, _branch);
			dataTable4.SetPrimaryKey("Period", "EmpId");
			dtOldOT.Merge(dataTable4);
			foreach (DataRow item in dtOldOT.AsEnumerable())
			{
				DataRow dataRow4 = dtCalculateOT.NewRow();
				dataRow4["Period"] = _toPeriod;
				dataRow4["EmpId"] = item["EmpId"];
				dataRow4["EmpName"] = item["EmpName"];
				double num3 = Function.ParseDouble(item["Remain"]) + Function.ParseDouble(item["OvertimeTotal"]);
				dataRow4["CompLeaveTotal"] = num3;
				dataRow4["Used"] = 0;
				dataRow4["Remain"] = num3;
				if (dataRow4.IsExistsColumn("UserSign2"))
				{
					dataRow4["UserSign2"] = GlobalData.UserID;
				}
				if (dataRow4.IsExistsColumn("UpdateDate"))
				{
					dataRow4["UpdateDate"] = DateTime.Today;
				}
				dtCalculateOT.Rows.Add(dataRow4);
				dtCalculateOT.AcceptChanges();
			}
			HttpResult httpResult2 = await _httpClient.TimeSheetingClient.SaveOvertimeInformation(dtCalculateOT);
			if (httpResult2 != null && httpResult2.Message != "")
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult2.Message);
				return;
			}
			WinCommons.Success(Enums.MessageCode.LeavehaveBeenCalculateSuccess, "");
			Close();
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

	private void luk_ToPeriod_Properties_PopupFilter(object sender, PopupFilterEventArgs e)
	{
		try
		{
			CriteriaOperator criteria = CriteriaOperator.Parse("PeriodStatus='U'");
			e.Criteria = criteria;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_FromPeriod_Properties_PopupFilter(object sender, PopupFilterEventArgs e)
	{
		try
		{
			CriteriaOperator criteria = CriteriaOperator.Parse("PeriodStatus='C' OR PeriodStatus='S'");
			e.Criteria = criteria;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("branch", typeof(int));
			dataTable.Columns.Add("dept", typeof(string));
			dataTable.Columns.Add("empId", typeof(int));
			dataTable.Columns.Add("empName", typeof(string));
			dataTable.Columns.Add("status", typeof(string));
			dataTable.Columns.Add("period", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["branch"] = Function.ParseInt(luk_Branch.EditValue);
			dataRow["dept"] = Function.ToString(luk_Department.EditValue);
			dataRow["empId"] = Function.ParseInt(txt_EmpId.EditValue);
			dataRow["empName"] = Function.ToString(txt_EmpName.EditValue);
			dataRow["status"] = Function.ToString(luk_Status.EditValue);
			string value = Function.ToString(luk_FromPeriod.EditValue);
			if (!string.IsNullOrWhiteSpace(value))
			{
				dataRow["period"] = value;
			}
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataTable dataTable2 = (await _httpClient.LeaveInformationClient.SearchLeaveInfo(dataTable)).Copy();
			List<int> list = new List<int>();
			for (int i = 0; i < dataTable2.Columns.Count; i++)
			{
				DataColumn dataColumn = dataTable2.Columns[i];
				if (dataColumn.ColumnName == "Period" || dataColumn.ColumnName == "WorkTotal" || dataColumn.ColumnName == "LeaveTotal" || dataColumn.ColumnName == "Used" || dataColumn.ColumnName.Contains("WorkCode_"))
				{
					list.Add(i);
				}
			}
			for (int num = list.Count - 1; num >= 0; num--)
			{
				dataTable2.Columns.RemoveAt(list[num]);
			}
			dataTable2.Columns.Add("LeaveAccrual", typeof(double));
			grd_Doc.DataSource = dataTable2;
			grd_Doc.RefreshDataSource();
			await gv_Doc.ReloadFormSetting();
			luk_calcMethod.EditValue = null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_calcMethod_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			int _branch = Function.ParseInt(luk_Branch.EditValue);
			string _fromPeriod = Function.ToString(luk_FromPeriod.EditValue);
			DataRow currentPeriod = dtAPZ_OHWP.AsEnumerable().FirstOrDefault((DataRow s) => Function.ToString(s["PeriodCode"]) == _fromPeriod && Function.ParseInt(s["Branch"]) == _branch);
			if (currentPeriod == null)
			{
				return;
			}
			string text = Function.ToString(luk_calcMethod.EditValue);
			if (text == "Y")
			{
				double num = Function.ParseDouble(currentPeriod["FPLTotal"]);
				txt_LeaveAccrual.EditValue = num;
			}
			else
			{
				txt_LeaveAccrual.EditValue = Function.ParseDouble(currentPeriod["IncreaseLeaveTotal"]);
			}
			DataTable dataTable = grd_Doc.DatasourceTable();
			if (dataTable.IsNotNull())
			{
				for (int num2 = 0; num2 < dataTable.Rows.Count; num2++)
				{
					decimal num3 = Function.ParseDecimal(dataTable.Rows[num2]["Remain"]);
					if (dtAPZ_OSLP.IsNotNull() && (from s in dtAPZ_OSLP.AsEnumerable()
						where Function.ParseDateTimes(currentPeriod["FromDate"]) >= Function.ParseDateTimes(s["FromDate"]) && Function.ParseDateTimes(currentPeriod["ToDate"]) <= Function.ParseDateTimes(s["ToDate"])
						select s).Any() && num3 >= Function.ParseDecimal(dtAPZ_OSLP.AsEnumerable().FirstOrDefault((DataRow s) => Function.ParseDateTimes(currentPeriod["FromDate"]) >= Function.ParseDateTimes(s["FromDate"]) && Function.ParseDateTimes(currentPeriod["ToDate"]) <= Function.ParseDateTimes(s["ToDate"]))["NoOfMaxCarryLeave"]))
					{
						num3 = Function.ParseDecimal(dtAPZ_OSLP.AsEnumerable().FirstOrDefault((DataRow s) => Function.ParseDateTimes(currentPeriod["FromDate"]) >= Function.ParseDateTimes(s["FromDate"]) && Function.ParseDateTimes(currentPeriod["ToDate"]) <= Function.ParseDateTimes(s["ToDate"]))["NoOfMaxCarryLeave"]);
					}
					dataTable.Rows[num2]["LeaveAccrual"] = ((text == "Y") ? Function.ParseDecimal(txt_LeaveAccrual.EditValue) : (num3 + Function.ParseDecimal(txt_LeaveAccrual.EditValue)));
				}
				dataTable.AcceptChanges();
			}
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_calcMethod_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (e.NewValue == null)
			{
				txt_LeaveAccrual.EditValue = null;
				return;
			}
			int num = Function.ParseInt(luk_Branch.EditValue);
			if (string.IsNullOrWhiteSpace(Function.ToString(luk_FromPeriod.EditValue)) || num == 0)
			{
				WinCommons.Warning(Enums.MessageCode.FromPeriodIsEmpty, "");
				e.Cancel = true;
			}
			else if (!(grd_Doc.DataSource as DataTable).IsNotNull())
			{
				WinCommons.Warning(Enums.MessageCode.DataIsEmpty, "");
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_FromPeriod_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			luk_calcMethod.EditValue = null;
			int _branch = Function.ParseInt(luk_Branch.EditValue);
			string _fromPeriod = Function.ToString(luk_FromPeriod.EditValue);
			DataRow dataRow = dtAPZ_OHWP.AsEnumerable().FirstOrDefault((DataRow s) => Function.ToString(s["PeriodCode"]) == _fromPeriod && Function.ParseInt(s["Branch"]) == _branch);
			DateTime currentPeriodFromDate = DateTime.MaxValue;
			if (dataRow != null)
			{
				currentPeriodFromDate = Function.ParseDateTimes(dataRow["FromDate"]);
			}
			DataRow[] array = (from dr in dtAPZ_OHWP.AsEnumerable()
				where Function.ParseInt(dr["Branch"]) == Branch && Function.ToString(dr["PeriodStatus"]) == "U" && Function.ParseDateTimes(dr["FromDate"]) > currentPeriodFromDate
				orderby dr["FromDate"]
				select dr).ToArray();
			if (array.Length == 0)
			{
				return;
			}
			DataRow dataRow2 = array[0];
			for (int num = 0; num < array.Length; num++)
			{
				if (Function.ToString(array[num]["PeriodCode"]) == Function.ToString(luk_ToPeriod.EditValue))
				{
					return;
				}
			}
			luk_ToPeriod.EditValue = Function.ToString(dataRow2["PeriodCode"]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_LeaveCalculation_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.TimeSheeting.frm_LeaveCalculation));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.rep_Deparment = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_LeaveAccrual = new Apzon.Controls.Editor.TextEditEx();
		this.luk_FromPeriod = new Apzon.Controls.Editor.MyLookUpEdit.LookUpEditSimple();
		this.luk_ToPeriod = new Apzon.Controls.Editor.MyLookUpEdit.LookUpEditSimple();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.luk_Branch = new Apzon.Controls.Editor.MyLookUpEdit.LookUpEditSimple();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.EmpId = new Apzon.Controls.Grid.GridColumnEx();
		this.gcExtEmpNo = new Apzon.Controls.Grid.GridColumnEx();
		this.EmpName = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_DrillDownEmp = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcDepart = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Dept = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcLeaveAccrual = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Branch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.luk_calcMethod = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine12 = new Apzon.Controls.Controls.LabelLine();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.luk_Department = new Apzon.Controls.Editor.CheckedComboBoxEditEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.luk_Status = new Apzon.Controls.Editor.CheckedComboBoxEditEx();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.txt_EmpName = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.txt_EmpId = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine10 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine11 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.rep_Deparment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_LeaveAccrual.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_FromPeriod.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ToPeriod.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DrillDownEmp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Dept).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_calcMethod.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_Department.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpId.Properties).BeginInit();
		base.SuspendLayout();
		this.rep_Deparment.AutoHeight = false;
		this.rep_Deparment.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Deparment.Name = "rep_Deparment";
		this.rep_Deparment.NullText = "";
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(88, 571);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 13;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOK.EditControl = null;
		this.btnOK.EditValue = "OK";
		this.btnOK.FieldName = null;
		this.btnOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOK.ImageOptions.Image");
		this.btnOK.IsSearchControl = false;
		this.btnOK.Location = new System.Drawing.Point(7, 571);
		this.btnOK.Name = "btnOK";
		this.btnOK.ReadOnly = false;
		this.btnOK.SettingInVisible = false;
		this.btnOK.SettingReadonly = false;
		this.btnOK.Size = new System.Drawing.Size(75, 23);
		this.btnOK.TabIndex = 12;
		this.btnOK.Text = "OK";
		this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "From period";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(7, 36);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(188, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 14;
		this.labelLine1.Utext = "From period";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "To period";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(7, 63);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(188, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 14;
		this.labelLine2.Utext = "To period";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Leave Accrual";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(7, 138);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(121, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 17;
		this.labelLine3.Utext = "Leave Accrual";
		this.txt_LeaveAccrual.EditControl = null;
		this.txt_LeaveAccrual.FieldName = "txt_LeaveAccrual";
		this.txt_LeaveAccrual.IsSearchControl = false;
		this.txt_LeaveAccrual.Location = new System.Drawing.Point(88, 136);
		this.txt_LeaveAccrual.Name = "txt_LeaveAccrual";
		this.txt_LeaveAccrual.Properties.Mask.EditMask = "n";
		this.txt_LeaveAccrual.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
		this.txt_LeaveAccrual.Properties.ReadOnly = true;
		this.txt_LeaveAccrual.SettingInVisible = false;
		this.txt_LeaveAccrual.SettingReadonly = false;
		this.txt_LeaveAccrual.Size = new System.Drawing.Size(156, 20);
		this.txt_LeaveAccrual.TabIndex = 18;
		this.luk_FromPeriod.EditControl = null;
		this.luk_FromPeriod.FieldName = "luk_FromPeriod";
		this.luk_FromPeriod.IsSearchControl = false;
		this.luk_FromPeriod.Location = new System.Drawing.Point(88, 33);
		this.luk_FromPeriod.Name = "luk_FromPeriod";
		this.luk_FromPeriod.Properties.AutoHeight = false;
		this.luk_FromPeriod.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_FromPeriod.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_FromPeriod.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("PeriodName", "PeriodName")
		});
		this.luk_FromPeriod.Properties.IsSearchControl = false;
		this.luk_FromPeriod.Properties.NullText = "";
		this.luk_FromPeriod.Properties.ShowFooter = false;
		this.luk_FromPeriod.Properties.ShowHeader = false;
		this.luk_FromPeriod.Properties.PopupFilter += new DevExpress.XtraEditors.Controls.PopupFilterEventHandler(this.luk_FromPeriod_Properties_PopupFilter);
		this.luk_FromPeriod.SettingInVisible = false;
		this.luk_FromPeriod.SettingReadonly = false;
		this.luk_FromPeriod.Size = new System.Drawing.Size(156, 20);
		this.luk_FromPeriod.TabIndex = 19;
		this.luk_FromPeriod.EditValueChanged += new System.EventHandler(this.luk_FromPeriod_EditValueChanged);
		this.luk_ToPeriod.EditControl = null;
		this.luk_ToPeriod.FieldName = "luk_ToPeriod";
		this.luk_ToPeriod.IsSearchControl = false;
		this.luk_ToPeriod.Location = new System.Drawing.Point(88, 60);
		this.luk_ToPeriod.Name = "luk_ToPeriod";
		this.luk_ToPeriod.Properties.AutoHeight = false;
		this.luk_ToPeriod.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_ToPeriod.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_ToPeriod.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("PeriodName", "PeriodName")
		});
		this.luk_ToPeriod.Properties.IsSearchControl = false;
		this.luk_ToPeriod.Properties.NullText = "";
		this.luk_ToPeriod.Properties.ShowFooter = false;
		this.luk_ToPeriod.Properties.ShowHeader = false;
		this.luk_ToPeriod.Properties.PopupFilter += new DevExpress.XtraEditors.Controls.PopupFilterEventHandler(this.luk_ToPeriod_Properties_PopupFilter);
		this.luk_ToPeriod.SettingInVisible = false;
		this.luk_ToPeriod.SettingReadonly = false;
		this.luk_ToPeriod.Size = new System.Drawing.Size(156, 20);
		this.luk_ToPeriod.TabIndex = 20;
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "Branch";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(7, 9);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(188, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 14;
		this.labelLine7.Utext = "Branch";
		this.luk_Branch.EditControl = null;
		this.luk_Branch.FieldName = "luk_Branch";
		this.luk_Branch.IsSearchControl = false;
		this.luk_Branch.Location = new System.Drawing.Point(88, 9);
		this.luk_Branch.Name = "luk_Branch";
		this.luk_Branch.Properties.AutoHeight = false;
		this.luk_Branch.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Branch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Branch.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_Branch.Properties.IsSearchControl = false;
		this.luk_Branch.Properties.NullText = "";
		this.luk_Branch.Properties.ShowFooter = false;
		this.luk_Branch.Properties.ShowHeader = false;
		this.luk_Branch.SettingInVisible = false;
		this.luk_Branch.SettingReadonly = false;
		this.luk_Branch.Size = new System.Drawing.Size(156, 17);
		this.luk_Branch.TabIndex = 19;
		this.luk_Branch.EditValueChanged += new System.EventHandler(this.luk_Branch_EditValueChanged);
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Location = new System.Drawing.Point(7, 162);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_DrillDownEmp, this.rep_Dept, this.rep_Branch });
		this.grd_Doc.Size = new System.Drawing.Size(550, 403);
		this.grd_Doc.TabIndex = 57;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = "EmpId";
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.EmpId, this.gcExtEmpNo, this.EmpName, this.gcDepart, this.gcLeaveAccrual, this.gridColumnEx1 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oTimeSheeting";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.GroupCount = 1;
		this.gv_Doc.HasFormSetting = true;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 30;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_OTSL";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.LstOldUDF = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_Doc.OptionsEditForm.ShowOnEnterKey = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.ReadOnly = false;
		this.gv_Doc.SettingReadonly = false;
		this.gv_Doc.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gcDepart, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gv_Doc.Tag = "APZ_TSL2";
		this.gv_Doc.UseBatchMod = false;
		this.EmpId.Caption = "Emp No.";
		this.EmpId.CurrencyDateField = null;
		this.EmpId.CurrencyField = null;
		this.EmpId.CurrencyFixed = null;
		this.EmpId.CurrencyFixedRate = 0.0;
		this.EmpId.CurrencyRateField = null;
		this.EmpId.FieldName = "empID";
		this.EmpId.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.EmpId.Name = "EmpId";
		this.EmpId.OptionsColumn.ReadOnly = true;
		this.EmpId.OptionsEditForm.VisibleIndex = 1;
		this.EmpId.PrimaryKey = false;
		this.EmpId.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedRow;
		this.EmpId.Tag = "APZ_OTSL EmpId";
		this.EmpId.UnitField = null;
		this.EmpId.Width = 50;
		this.gcExtEmpNo.Caption = "Extened Employee No";
		this.gcExtEmpNo.CurrencyDateField = null;
		this.gcExtEmpNo.CurrencyField = null;
		this.gcExtEmpNo.CurrencyFixed = null;
		this.gcExtEmpNo.CurrencyFixedRate = 0.0;
		this.gcExtEmpNo.CurrencyRateField = null;
		this.gcExtEmpNo.FieldName = "ExtEmpNo";
		this.gcExtEmpNo.Name = "gcExtEmpNo";
		this.gcExtEmpNo.OptionsColumn.ReadOnly = true;
		this.gcExtEmpNo.PrimaryKey = false;
		this.gcExtEmpNo.UnitField = null;
		this.EmpName.Caption = "Employee Name";
		this.EmpName.ColumnEdit = this.rep_DrillDownEmp;
		this.EmpName.CurrencyDateField = null;
		this.EmpName.CurrencyField = null;
		this.EmpName.CurrencyFixed = null;
		this.EmpName.CurrencyFixedRate = 0.0;
		this.EmpName.CurrencyRateField = null;
		this.EmpName.FieldName = "empName";
		this.EmpName.Name = "EmpName";
		this.EmpName.OptionsColumn.ReadOnly = true;
		this.EmpName.PrimaryKey = false;
		this.EmpName.Tag = "APZ_TSL2 EmpName";
		this.EmpName.UnitField = null;
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
		this.rep_DrillDownEmp.ListMandatoryField = null;
		this.rep_DrillDownEmp.LoadDataSourceWhenEnter = false;
		this.rep_DrillDownEmp.Name = "rep_DrillDownEmp";
		this.rep_DrillDownEmp.ReadOnly = true;
		this.rep_DrillDownEmp.SeparateCharacterForMulti = ';';
		this.rep_DrillDownEmp.SeparateString = ", ";
		this.rep_DrillDownEmp.UsingAutoCompleteBox = false;
		this.rep_DrillDownEmp.UsingDataSource = true;
		this.rep_DrillDownEmp.ValidateWhenNoDataSource = false;
		this.rep_DrillDownEmp.ValueMember = "";
		this.gcDepart.Caption = "Department";
		this.gcDepart.ColumnEdit = this.rep_Dept;
		this.gcDepart.CurrencyDateField = null;
		this.gcDepart.CurrencyField = null;
		this.gcDepart.CurrencyFixed = null;
		this.gcDepart.CurrencyFixedRate = 0.0;
		this.gcDepart.CurrencyRateField = null;
		this.gcDepart.FieldName = "dept";
		this.gcDepart.Name = "gcDepart";
		this.gcDepart.OptionsColumn.ReadOnly = true;
		this.gcDepart.PrimaryKey = false;
		this.gcDepart.UnitField = null;
		this.gcDepart.Visible = true;
		this.gcDepart.VisibleIndex = 2;
		this.rep_Dept.AutoHeight = false;
		this.rep_Dept.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Dept.Name = "rep_Dept";
		this.rep_Dept.NullText = "";
		this.gcLeaveAccrual.Caption = "Leave Accrual";
		this.gcLeaveAccrual.CurrencyDateField = null;
		this.gcLeaveAccrual.CurrencyField = null;
		this.gcLeaveAccrual.CurrencyFixed = null;
		this.gcLeaveAccrual.CurrencyFixedRate = 0.0;
		this.gcLeaveAccrual.CurrencyRateField = null;
		this.gcLeaveAccrual.FieldName = "LeaveAccrual";
		this.gcLeaveAccrual.Name = "gcLeaveAccrual";
		this.gcLeaveAccrual.PrimaryKey = false;
		this.gcLeaveAccrual.UnitField = null;
		this.gcLeaveAccrual.Width = 60;
		this.gridColumnEx1.Caption = "Remain";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "Remain";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.rep_Branch.AutoHeight = false;
		this.rep_Branch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Branch.Name = "rep_Branch";
		this.luk_calcMethod.EditControl = null;
		this.luk_calcMethod.FieldName = "luk_calcMethod";
		this.luk_calcMethod.IsSearchControl = false;
		this.luk_calcMethod.Location = new System.Drawing.Point(7, 107);
		this.luk_calcMethod.Name = "luk_calcMethod";
		this.luk_calcMethod.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_calcMethod.Properties.NullText = "";
		this.luk_calcMethod.SettingInVisible = false;
		this.luk_calcMethod.SettingReadonly = false;
		this.luk_calcMethod.Size = new System.Drawing.Size(237, 20);
		this.luk_calcMethod.TabIndex = 90;
		this.luk_calcMethod.EditValueChanged += new System.EventHandler(this.luk_calcMethod_EditValueChanged);
		this.luk_calcMethod.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_calcMethod_EditValueChanging);
		this.labelLine12.EditControl = null;
		this.labelLine12.EditValue = "Calculate Leave Accrual by Method";
		this.labelLine12.FieldName = null;
		this.labelLine12.HideWhenEmpty = false;
		this.labelLine12.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine12.Location = new System.Drawing.Point(7, 83);
		this.labelLine12.Name = "labelLine12";
		this.labelLine12.ReadOnly = false;
		this.labelLine12.SettingInVisible = false;
		this.labelLine12.SettingReadonly = false;
		this.labelLine12.ShowUnderLine = true;
		this.labelLine12.Size = new System.Drawing.Size(194, 20);
		this.labelLine12.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine12.TabIndex = 91;
		this.labelLine12.Utext = "Calculate Leave Accrual by Method";
		this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.panelControl1.Controls.Add(this.luk_Department);
		this.panelControl1.Controls.Add(this.btn_Search);
		this.panelControl1.Controls.Add(this.luk_Status);
		this.panelControl1.Controls.Add(this.labelLine8);
		this.panelControl1.Controls.Add(this.txt_EmpName);
		this.panelControl1.Controls.Add(this.labelLine9);
		this.panelControl1.Controls.Add(this.txt_EmpId);
		this.panelControl1.Controls.Add(this.labelLine10);
		this.panelControl1.Controls.Add(this.labelLine11);
		this.panelControl1.Location = new System.Drawing.Point(250, 7);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(254, 149);
		this.panelControl1.TabIndex = 92;
		this.luk_Department.EditControl = null;
		this.luk_Department.FieldName = null;
		this.luk_Department.IsSearchControl = false;
		this.luk_Department.Location = new System.Drawing.Point(90, 38);
		this.luk_Department.Name = "luk_Department";
		this.luk_Department.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Department.SettingInVisible = false;
		this.luk_Department.SettingReadonly = false;
		this.luk_Department.Size = new System.Drawing.Size(156, 20);
		this.luk_Department.TabIndex = 97;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(125, 123);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(121, 22);
		this.btn_Search.TabIndex = 98;
		this.btn_Search.Tag = "Button To Search Time Sheeting by Period";
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.luk_Status.EditControl = null;
		this.luk_Status.FieldName = null;
		this.luk_Status.IsSearchControl = false;
		this.luk_Status.Location = new System.Drawing.Point(90, 5);
		this.luk_Status.Name = "luk_Status";
		this.luk_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Status.SettingInVisible = false;
		this.luk_Status.SettingReadonly = false;
		this.luk_Status.Size = new System.Drawing.Size(156, 20);
		this.luk_Status.TabIndex = 96;
		this.labelLine8.EditControl = null;
		this.labelLine8.EditValue = "Status";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(5, 8);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ReadOnly = false;
		this.labelLine8.SettingInVisible = false;
		this.labelLine8.SettingReadonly = false;
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(104, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 95;
		this.labelLine8.Utext = "Status";
		this.txt_EmpName.EditControl = null;
		this.txt_EmpName.FieldName = null;
		this.txt_EmpName.IsSearchControl = false;
		this.txt_EmpName.Location = new System.Drawing.Point(90, 98);
		this.txt_EmpName.Name = "txt_EmpName";
		this.txt_EmpName.SettingInVisible = false;
		this.txt_EmpName.SettingReadonly = false;
		this.txt_EmpName.Size = new System.Drawing.Size(156, 20);
		this.txt_EmpName.TabIndex = 91;
		this.labelLine9.EditControl = null;
		this.labelLine9.EditValue = "Employee Name";
		this.labelLine9.FieldName = null;
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(5, 101);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ReadOnly = false;
		this.labelLine9.SettingInVisible = false;
		this.labelLine9.SettingReadonly = false;
		this.labelLine9.ShowUnderLine = true;
		this.labelLine9.Size = new System.Drawing.Size(104, 20);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 94;
		this.labelLine9.Tag = "Employee Name";
		this.labelLine9.Utext = "Employee Name";
		this.txt_EmpId.EditControl = null;
		this.txt_EmpId.FieldName = null;
		this.txt_EmpId.IsSearchControl = false;
		this.txt_EmpId.Location = new System.Drawing.Point(90, 68);
		this.txt_EmpId.Name = "txt_EmpId";
		this.txt_EmpId.SettingInVisible = false;
		this.txt_EmpId.SettingReadonly = false;
		this.txt_EmpId.Size = new System.Drawing.Size(156, 20);
		this.txt_EmpId.TabIndex = 90;
		this.labelLine10.EditControl = null;
		this.labelLine10.EditValue = "Employee No.";
		this.labelLine10.FieldName = null;
		this.labelLine10.HideWhenEmpty = false;
		this.labelLine10.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine10.Location = new System.Drawing.Point(5, 71);
		this.labelLine10.Name = "labelLine10";
		this.labelLine10.ReadOnly = false;
		this.labelLine10.SettingInVisible = false;
		this.labelLine10.SettingReadonly = false;
		this.labelLine10.ShowUnderLine = true;
		this.labelLine10.Size = new System.Drawing.Size(104, 20);
		this.labelLine10.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine10.TabIndex = 92;
		this.labelLine10.Utext = "Employee No.";
		this.labelLine11.EditControl = null;
		this.labelLine11.EditValue = "Department";
		this.labelLine11.FieldName = null;
		this.labelLine11.HideWhenEmpty = false;
		this.labelLine11.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine11.Location = new System.Drawing.Point(5, 41);
		this.labelLine11.Name = "labelLine11";
		this.labelLine11.ReadOnly = false;
		this.labelLine11.SettingInVisible = false;
		this.labelLine11.SettingReadonly = false;
		this.labelLine11.ShowUnderLine = true;
		this.labelLine11.Size = new System.Drawing.Size(104, 20);
		this.labelLine11.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine11.TabIndex = 93;
		this.labelLine11.Utext = "Department";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(569, 601);
		base.Controls.Add(this.panelControl1);
		base.Controls.Add(this.labelLine12);
		base.Controls.Add(this.luk_calcMethod);
		base.Controls.Add(this.grd_Doc);
		base.Controls.Add(this.luk_ToPeriod);
		base.Controls.Add(this.luk_Branch);
		base.Controls.Add(this.luk_FromPeriod);
		base.Controls.Add(this.txt_LeaveAccrual);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine7);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btnOK);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_LeaveCalculation";
		base.ShowInTaskbar = false;
		this.Text = "Leave Calculation";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_LeaveCalculation_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.rep_Deparment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_LeaveAccrual.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_FromPeriod.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ToPeriod.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DrillDownEmp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Dept).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_calcMethod.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.luk_Department.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpId.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
