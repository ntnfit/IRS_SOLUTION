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
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
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
using DevExpress.XtraTab;

namespace Apzon.Forms.HumanResources.Salary;

public class frm_SalaryInfo : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private readonly List<Common.Table_Code> _lstAllowanceType = new List<Common.Table_Code>();

	private bool _isChanging;

	private bool _isCancel;

	private bool _forceClose;

	private int _maxLineNumInDb;

	private DataTable _dtSearch;

	private IContainer components;

	private XtraTabControlEx tab_Detail;

	private XtraTabPageEx tab_Allowance;

	private XtraTabPageEx tab_Advanced;

	public SimpleButtonEx btnCancel;

	private PanelControl panelControl5;

	private GridControlEx grd_Allowance;

	private GridViewEx gv_Allowance;

	private GridControlEx grd_Employee;

	private GridViewEx gv_Employee;

	private XtraTabPageEx tab_Main;

	private PanelControl panelControl1;

	private LabelLine lbl_Department;

	private LabelLine lbl_Branch;

	private LabelLine labelLine3;

	private TextEditEx txt_EmpNo;

	private TextEditEx txt_EmpName;

	private LabelLine lbl_EmpNo;

	private LabelLine lbl_FirstName;

	public SimpleButtonEx btnSearch;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Advanced;

	private GridViewEx gv_Advanced;

	private GridControlEx grd_MainInfo;

	private GridViewEx gv_MainInfo;

	private GridColumnEx gridColumn1;

	private GridColumnEx gc_empNo;

	private GridColumnEx gridColumn3;

	private GridColumnEx gc_BasicSal;

	private GridColumnEx gc_SalesSal;

	private GridColumnEx gc_SalTotal;

	private RepositoryItemLookUpEdit rep_Deparment;

	public SimpleButtonEx btn_EditMain;

	private GridColumnEx gridColumn7;

	private RepositoryItemLookUpEdit rep_DeptAllowance;

	private GridColumnEx gc_empNo2;

	private GridColumnEx gridColumn9;

	private GridColumnEx gridColumn10;

	private GridColumnEx gc_empNo3;

	private GridColumnEx gridColumn12;

	private GridColumnEx gridColumn13;

	private GridColumnEx gc_Amount;

	private GridColumnEx gridColumn15;

	private GridColumnEx gridColumn16;

	private RepositoryItemCheckEdit rep_MonthlyPay;

	private GridColumnEx gridColumn17;

	private RepositoryItemCheckEdit rep_Received;

	private GridColumnEx gridColumn18;

	private RepositoryItemMemoExEdit rep_Remarks;

	private RepositoryItemLookUpEdit rep_deptAdvanced;

	public SimpleButtonEx btn_SaveAdvanced;

	public SimpleButtonEx btn_CancelAdvanced;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private RepositoryItemLookUpEdit rep_Period;

	private CheckedComboBoxEditEx luk_Department;

	private CheckedComboBoxEditEx luk_Status;

	private LookUpEditCombo luk_Branch;

	private GridColumnEx gridColumn4;

	private RepositoryItemLookUpEdit rep_Currency;

	private GridColumnEx gridColumnEx1;

	private RepositoryItemLookUpEdit rep_SalCur;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private RepositoryItemButtonEdit rep_CFL;

	private RepositoryItemButtonEdit rep_CFL1;

	private RepositoryItemButtonEdit rep_CFL2;

	private ContextMenuStrip contextMenuStrip2;

	private ContextMenuStrip contextMenuStrip3;

	private ToolStripMenuItem editAllMainInfoToolStripMenuItem;

	private ToolStripMenuItem editAllAllowanceToolStripMenuItem;

	private GridColumnEx gridColumnEx3;

	private RepositoryItemLookUpEdit rep_Branch;

	private GridColumnEx gridColumnEx2;

	private RepositoryItemLookUpEdit rep_Status;

	private GridColumnEx gridColumnEx5;

	private RepositoryItemLookUpEdit rep_Branch1;

	private GridColumnEx gridColumnEx4;

	private RepositoryItemLookUpEdit rep_Status1;

	private GridColumnEx gridColumnEx7;

	private RepositoryItemLookUpEdit rep_Branch2;

	private GridColumnEx gridColumnEx6;

	private RepositoryItemLookUpEdit rep_Status2;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx10;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx3;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx4;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private GridColumnEx gridColumnEx11;

	private GridColumnEx cl_TaxCode;

	private RepositoryItemLookUpEdit rep_TaxCode;

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
		case Enums.FormViewModeType.Update:
			btn_SaveAdvanced.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_SaveAdvanced.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_SalaryInfo()
	{
		InitializeComponent();
		FormId = "oSalaryInfo";
		gv_MainInfo.FormId = FormId;
		gv_Allowance.FormId = FormId;
		gv_Advanced.FormId = FormId;
		gv_MainInfo.ItemId = "APZ_OHSI";
		gv_Allowance.ItemId = "APZ_HSI1";
		gv_Advanced.ItemId = "APZ_HSI2";
		gv_MainInfo.ChangeLogType = Enums.ChangeLog.clSalaryInfo;
		gv_MainInfo.IsApzTable = true;
		gv_Allowance.ChangeLogType = Enums.ChangeLog.clSalAllowance;
		gv_Allowance.IsApzTable = true;
		gv_Employee.ChangeLogType = Enums.ChangeLog.clSalAdvance;
		gv_Advanced.IsApzTable = true;
		OnLoadAsync += frm_SalaryInfo_Load;
	}

	private async Task frm_SalaryInfo_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			await LoadInitForm();
			await WinFuntions.LoadUdfColumns(gv_MainInfo, "APZ_OHSI");
			await LoadSearchSetting();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task LoadSearchSetting()
	{
		try
		{
			DataTable dataTable = await WinFuntions.LoadSearchSetting(FormId, gv_MainInfo.ItemId);
			if (dataTable.HasColumn() && dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.Rows[0];
				luk_Branch.EditValue = Function.ParseShort(dataRow["Branch"]);
				if (Function.ParseInt(dataRow["Dept"]) != 0)
				{
					luk_Department.EditValue = Function.ParseInt(dataRow["Dept"]);
				}
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
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
			dataRow["ItemId"] = Function.ToString(gv_MainInfo.ItemId);
			dataRow["UserSign"] = Function.ToString(GlobalData.UserCodeLogin);
			dataRow["Branch"] = ((luk_Branch.EditValue == null) ? DBNull.Value : luk_Branch.EditValue);
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

	private async void frm_SalaryInfo_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (_isCancel)
			{
				return;
			}
			if (!_forceClose)
			{
				e.Cancel = true;
			}
			if (_forceClose)
			{
				return;
			}
			if (FormMode == Enums.FormViewModeType.Update)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
				{
				case DialogResult.No:
					_forceClose = true;
					Close();
					break;
				case DialogResult.Yes:
					if (string.IsNullOrEmpty(await SaveAdvance()))
					{
						_forceClose = true;
						Close();
					}
					break;
				}
			}
			else
			{
				_forceClose = true;
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
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
		_ = 1;
		try
		{
			await LoadData();
			await SaveSearchSetting();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_EditMain_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_MainInfo.DataSourceTable;
			DataTable dataSourceTable2 = grd_Allowance.DataSourceTable;
			DataTable dataSourceTable3 = grd_Employee.DataSourceTable;
			if (tab_Detail.SelectedTabPage == tab_Allowance)
			{
				if (dataSourceTable2.IsNotNull() && _dtSearch.IsNotNull())
				{
					frm_SalAllowance frm_SalAllowance2 = new frm_SalAllowance
					{
						MdiParent = MdiParent,
						DtSearch = _dtSearch.Copy()
					};
					frm_SalAllowance2.ReloadMain = (ReloadMain)Delegate.Combine(frm_SalAllowance2.ReloadMain, new ReloadMain(LoadData));
					frm_SalAllowance2.Show();
				}
			}
			else if (tab_Detail.SelectedTabPage == tab_Advanced)
			{
				if (dataSourceTable3.IsNotNull() && _dtSearch.IsNotNull())
				{
					frm_SalAdvanced frm_SalAdvanced2 = new frm_SalAdvanced
					{
						MdiParent = MdiParent,
						DtSearch = _dtSearch.Copy()
					};
					frm_SalAdvanced2.ReloadMain = (ReloadMain)Delegate.Combine(frm_SalAdvanced2.ReloadMain, new ReloadMain(LoadData));
					frm_SalAdvanced2.Show();
				}
			}
			else if (dataSourceTable.IsNotNull() && _dtSearch.IsNotNull())
			{
				frm_SalInfoMain frm_SalInfoMain2 = new frm_SalInfoMain
				{
					MdiParent = MdiParent,
					DtSearch = _dtSearch.Copy()
				};
				frm_SalInfoMain2.ReloadMain = (ReloadMain)Delegate.Combine(frm_SalInfoMain2.ReloadMain, new ReloadMain(LoadData));
				frm_SalInfoMain2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_MainInfo_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_MainInfo.GetFocusedRowCellValue("empID"));
			string text = Function.ToString(gv_MainInfo.GetFocusedRowCellValue("empName"));
			if (grd_MainInfo.DataSourceTable.IsNotNull() && num > 0)
			{
				frm_SalInfoByEmp frm_SalInfoByEmp2 = new frm_SalInfoByEmp
				{
					MdiParent = MdiParent,
					EmpId = num,
					Text = Text + " - " + text
				};
				frm_SalInfoByEmp2.ReloadMain = (ReloadMain)Delegate.Combine(frm_SalInfoByEmp2.ReloadMain, new ReloadMain(LoadData));
				frm_SalInfoByEmp2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Allowance_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Allowance.GetFocusedRowCellValue("empID"));
			string text = Function.ToString(gv_Allowance.GetFocusedRowCellValue("empName"));
			if (grd_Allowance.DataSourceTable.IsNotNull() && num > 0)
			{
				frm_SalAllowanceByEmp frm_SalAllowanceByEmp2 = new frm_SalAllowanceByEmp
				{
					MdiParent = MdiParent,
					EmpId = num
				};
				frm_SalAllowanceByEmp2.Text = frm_SalAllowanceByEmp2.Text + " - " + text;
				frm_SalAllowanceByEmp2.ReloadMain = (ReloadMain)Delegate.Combine(frm_SalAllowanceByEmp2.ReloadMain, new ReloadMain(LoadData));
				frm_SalAllowanceByEmp2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tab_Detail_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		try
		{
			if (e.Page == tab_Allowance)
			{
				btn_SaveAdvanced.Visible = false;
				btn_CancelAdvanced.Visible = false;
				btnCancel.Visible = true;
				btn_EditMain.Visible = true;
				btn_EditMain.Text = "Edit Allowance";
			}
			else if (e.Page == tab_Advanced)
			{
				btn_SaveAdvanced.Visible = true;
				btn_CancelAdvanced.Visible = true;
				btnCancel.Visible = false;
				btn_EditMain.Visible = true;
				btn_EditMain.Text = "Edit Advanced";
			}
			else
			{
				btn_SaveAdvanced.Visible = false;
				btn_CancelAdvanced.Visible = false;
				btnCancel.Visible = true;
				btn_EditMain.Visible = true;
				btn_EditMain.Text = "Edit Main Information";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_Employee_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		_ = 1;
		try
		{
			int num = Function.ParseInt(gv_Employee.GetRowCellValue(e.FocusedRowHandle, "empID"));
			if (_isChanging)
			{
				return;
			}
			if (e.FocusedRowHandle < 0)
			{
				grd_Advanced.DataSource = null;
			}
			else if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				_isChanging = true;
				gv_Employee.FocusedRowHandle = e.PrevFocusedRowHandle;
				_isChanging = false;
			}
			else
			{
				if (num <= 0)
				{
					return;
				}
				DataTable dataTable = await _httpClient.SalaryInfoAdvanceClient.GetAdvancedInfo(num);
				dataTable.SetDefaultAndMaxLen(gv_Advanced.ItemId);
				grd_Advanced.DataSource = dataTable;
				grd_Advanced.RefreshDataSource();
				int branch = ((!dataTable.IsNotNull()) ? Function.ParseInt(luk_Branch.EditValue) : Function.ParseInt(dataTable.Rows[0]["branch"]));
				DataTable workPeriodsByBranch = _httpClient.SalaryInfoAdvanceClient.GetWorkPeriodsByBranch(branch);
				rep_Period.DataSource = null;
				DataRow row = workPeriodsByBranch.NewRow();
				workPeriodsByBranch.Rows.InsertAt(row, 0);
				workPeriodsByBranch.AcceptChanges();
				rep_Period.DataSource = workPeriodsByBranch.Copy();
				rep_Period.DisplayMember = "Name";
				rep_Period.ValueMember = "Code";
				rep_Period.BestFitMode = BestFitMode.BestFitResizePopup;
				rep_Period.ForceInitialize();
				rep_Period.PopulateColumns();
				rep_Period.Columns["Code"].Visible = false;
				rep_Period.Columns["status"].Visible = false;
				FormMode = Enums.FormViewModeType.View;
				_maxLineNumInDb = (dataTable.IsNotNull() ? dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"])) : 0);
				if (dataTable.IsNotNull())
				{
					if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Period"])) && !string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Amount"])))
					{
						AddNewAdvanceRow();
					}
				}
				else
				{
					AddNewAdvanceRow();
				}
				await gv_Advanced.ReloadFormSetting();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_SaveAdvanced_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				int currentEmp = Function.ParseInt(gv_Employee.GetFocusedRowCellValue("empID"));
				btn_SaveAdvanced.Enabled = false;
				btnCancel.Enabled = false;
				string value = await SaveAdvance();
				btn_SaveAdvanced.Enabled = true;
				btnCancel.Enabled = true;
				if (string.IsNullOrEmpty(value))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					await LoadData();
					int num = gv_Employee.LocateByValue("empID", currentEmp);
					if (num != int.MinValue)
					{
						gv_Employee.FocusedRowHandle = num;
						gv_Employee.MakeRowVisible(num);
					}
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

	private void btn_CancelAdvanced_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gv_Advanced_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Advanced.DataSourceTable;
			if (!_isChanging)
			{
				FormMode = Enums.FormViewModeType.Update;
				string value = Function.ToString(gv_Advanced.GetRowCellValue(e.RowHandle, "Period"));
				int num = Function.ParseInt(gv_Advanced.GetRowCellValue(e.RowHandle, "LineNum"));
				int num2 = dataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"]));
				if (!string.IsNullOrEmpty(value) && num == num2)
				{
					AddNewAdvanceRow();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dt = grd_Advanced.DataSourceTable;
			int num = Function.ParseInt(gv_Advanced.GetFocusedRowCellValue("LineNum"));
			int num2 = dt.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"]));
			int empId = Function.ParseInt(gv_Employee.GetFocusedRowCellValue("empID"));
			if (num == num2)
			{
				return;
			}
			if (num > _maxLineNumInDb)
			{
				gv_Advanced.BeginUpdate();
				gv_Advanced.DeleteSelectedRows();
				gv_Advanced.EndUpdate();
				dt.AcceptChanges();
				grd_Advanced.RefreshDataSource();
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				string text = await _httpClient.SalaryInfoAdvanceClient.DeleteDoc(Function.ParseInt(num), empId);
				if (string.IsNullOrEmpty(text))
				{
					gv_Advanced.BeginUpdate();
					gv_Advanced.DeleteSelectedRows();
					gv_Advanced.EndUpdate();
					dt.AcceptChanges();
					grd_Advanced.RefreshDataSource();
					WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			e.Cancel = gv_Advanced.RowCount <= 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_CFL_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_MainInfo.GetFocusedRowCellValue("empID"));
			if (num > 0)
			{
				this.DrillDown(ApzObjectTypes.oEmployeesInfo, ApzObjectTypes.oEmployeesInfo, Function.ToString(num));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_CFL1_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Allowance.GetFocusedRowCellValue("empID"));
			if (num > 0)
			{
				this.DrillDown(ApzObjectTypes.oEmployeesInfo, ApzObjectTypes.oEmployeesInfo, Function.ToString(num));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_CFL2_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Employee.GetFocusedRowCellValue("empID"));
			if (num > 0)
			{
				this.DrillDown(ApzObjectTypes.oEmployeesInfo, ApzObjectTypes.oEmployeesInfo, Function.ToString(num));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void editAllAllowanceToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (grd_Allowance.DataSourceTable.IsNotNull())
			{
				frm_SalAllowance frm_SalAllowance2 = new frm_SalAllowance
				{
					MdiParent = MdiParent,
					DtSearch = _dtSearch.Copy()
				};
				frm_SalAllowance2.ReloadMain = (ReloadMain)Delegate.Combine(frm_SalAllowance2.ReloadMain, new ReloadMain(LoadData));
				frm_SalAllowance2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void editAllMainInfoToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (grd_MainInfo.DataSourceTable.IsNotNull())
			{
				frm_SalInfoMain frm_SalInfoMain2 = new frm_SalInfoMain
				{
					MdiParent = MdiParent,
					DtSearch = _dtSearch.Copy()
				};
				frm_SalInfoMain2.ReloadMain = (ReloadMain)Delegate.Combine(frm_SalInfoMain2.ReloadMain, new ReloadMain(LoadData));
				frm_SalInfoMain2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			e.Cancel = gv_MainInfo.RowCount <= 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void contextMenuStrip3_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			e.Cancel = gv_Allowance.RowCount <= 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadData()
	{
		_ = 1;
		try
		{
			_dtSearch = new DataTable();
			_dtSearch.Columns.Add("branch", typeof(int));
			_dtSearch.Columns.Add("dept", typeof(string));
			_dtSearch.Columns.Add("empId", typeof(int));
			_dtSearch.Columns.Add("empName", typeof(string));
			_dtSearch.Columns.Add("status", typeof(string));
			_dtSearch.Columns.Add("validFrom", typeof(DateTime));
			_dtSearch.Columns.Add("validTo", typeof(DateTime));
			DataRow dataRow = _dtSearch.NewRow();
			dataRow["branch"] = Function.ParseInt(luk_Branch.EditValue);
			dataRow["dept"] = Function.ToString(luk_Department.EditValue);
			dataRow["empId"] = Function.ParseInt(txt_EmpNo.EditValue);
			dataRow["empName"] = Function.ToString(txt_EmpName.EditValue);
			dataRow["status"] = Function.ToString(luk_Status.EditValue);
			_dtSearch.Rows.Add(dataRow);
			_dtSearch.AcceptChanges();
			DataSet dataSet = await _httpClient.SalaryInfoClient.Search(_dtSearch);
			if (dataSet.Tables.Count > 0)
			{
				grd_MainInfo.DataSource = dataSet.Tables[0];
				grd_MainInfo.RefreshDataSource();
				grd_Employee.DataSource = dataSet.Tables[0];
				grd_Employee.RefreshDataSource();
				foreach (DataColumn col in dataSet.Tables[1].Columns)
				{
					if (!(col.ColumnName != "empID") || !(col.ColumnName != "empName") || !(col.ColumnName != "LineNum") || !(col.ColumnName != "ValidFrom") || !(col.ColumnName != "ValidTo") || !(col.ColumnName != "dept") || !(col.ColumnName != "ExtEmpNo") || col.ColumnName.StartsWith("U_"))
					{
						continue;
					}
					_lstAllowanceType.AsEnumerable().FirstOrDefault((Common.Table_Code t) => t.Code == col.ColumnName);
					if (gv_Allowance.Columns.AsEnumerable().FirstOrDefault((GridColumn t) => t.FieldName == col.ColumnName) == null)
					{
						GridColumnEx gridColumnEx = new GridColumnEx
						{
							FieldName = col.ColumnName,
							Visible = true,
							Width = 200,
							Currency = Enums.CurrencyColumn.Custom,
							CurrencyField = "Currency",
							DisplayFormatType = Enums.DisplayFormatType.Amount
						};
						string[] array = col.ColumnName.Split('_');
						if (col.ColumnName.Contains("EmpAllowance"))
						{
							gridColumnEx.Caption = array[2];
						}
						else
						{
							gridColumnEx.Caption = col.Caption;
						}
						gridColumnEx.OptionsColumn.AllowEdit = true;
						gridColumnEx.OptionsColumn.ReadOnly = true;
						gv_Allowance.BeginUpdate();
						gv_Allowance.Columns.Insert(gv_Allowance.Columns.Count, gridColumnEx);
						gv_Allowance.EndUpdate();
					}
				}
				grd_Allowance.DataSource = dataSet.Tables[1];
				grd_Allowance.RefreshDataSource();
				gv_Allowance.ExpandAllGroups();
				gv_MainInfo.ExpandAllGroups();
				gv_Employee.ExpandAllGroups();
				Task task = gv_Allowance.ReloadFormSetting();
				Task task2 = gv_MainInfo.ReloadFormSetting();
				Task task3 = gv_Employee.ReloadFormSetting();
				await Task.WhenAll(task, task2, task3);
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadInitForm()
	{
		_ = 4;
		try
		{
			gv_Allowance.CustomDrawRowIndicator += gv_MainInfo_CustomDrawRowIndicator;
			gv_Employee.CustomDrawRowIndicator += gv_MainInfo_CustomDrawRowIndicator;
			gv_Advanced.CustomDrawRowIndicator += gv_MainInfo_CustomDrawRowIndicator;
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadComboDept();
			luk_Department.Properties.DataSource = dataTable.Copy();
			luk_Department.Properties.DisplayMember = "Name";
			luk_Department.Properties.ValueMember = "Code";
			rep_Deparment.DataSource = dataTable.Copy();
			rep_Deparment.DisplayMember = "Name";
			rep_Deparment.ValueMember = "Code";
			rep_Deparment.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Deparment.ForceInitialize();
			rep_Deparment.PopulateColumns();
			rep_Deparment.Columns["Code"].Visible = false;
			rep_Deparment.Columns["Remarks"].Visible = false;
			rep_DeptAllowance.DataSource = dataTable.Copy();
			rep_DeptAllowance.DisplayMember = "Name";
			rep_DeptAllowance.ValueMember = "Code";
			rep_DeptAllowance.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_DeptAllowance.ForceInitialize();
			rep_DeptAllowance.PopulateColumns();
			rep_DeptAllowance.Columns["Code"].Visible = false;
			rep_DeptAllowance.Columns["Remarks"].Visible = false;
			rep_deptAdvanced.DataSource = dataTable.Copy();
			rep_deptAdvanced.DisplayMember = "Name";
			rep_deptAdvanced.ValueMember = "Code";
			rep_deptAdvanced.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_deptAdvanced.ForceInitialize();
			rep_deptAdvanced.PopulateColumns();
			rep_deptAdvanced.Columns["Code"].Visible = false;
			rep_deptAdvanced.Columns["Remarks"].Visible = false;
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
			rep_Status1.DataSource = dataTable2.Copy();
			rep_Status1.DisplayMember = "Name";
			rep_Status1.ValueMember = "Code";
			rep_Status1.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Status1.ForceInitialize();
			rep_Status1.PopulateColumns();
			rep_Status1.Columns["Code"].Visible = false;
			rep_Status2.DataSource = dataTable2.Copy();
			rep_Status2.DisplayMember = "Name";
			rep_Status2.ValueMember = "Code";
			rep_Status2.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Status2.ForceInitialize();
			rep_Status2.PopulateColumns();
			rep_Status2.Columns["Code"].Visible = false;
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
			rep_Branch1.DataSource = dataTable3.Copy();
			rep_Branch1.DisplayMember = "Name";
			rep_Branch1.ValueMember = "Code";
			rep_Branch1.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Branch1.ForceInitialize();
			rep_Branch1.PopulateColumns();
			rep_Branch1.Columns["Code"].Visible = false;
			rep_Branch2.DataSource = dataTable3.Copy();
			rep_Branch2.DisplayMember = "Name";
			rep_Branch2.ValueMember = "Code";
			rep_Branch2.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Branch2.ForceInitialize();
			rep_Branch2.PopulateColumns();
			rep_Branch2.Columns["Code"].Visible = false;
			if (dataTable3.Rows.Count == 2)
			{
				luk_Branch.ItemIndex = 1;
			}
			List<CurrencyCodesImpl> currencyCodes = GlobalData.CurrencyCodes;
			rep_Currency.DataSource = currencyCodes;
			rep_Currency.DisplayMember = "CurrCode";
			rep_Currency.ValueMember = "CurrCode";
			rep_Currency.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Currency.ForceInitialize();
			rep_Currency.PopulateColumns();
			foreach (LookUpColumnInfo column in rep_Currency.Columns)
			{
				column.Visible = column.FieldName == "CurrName" || column.FieldName == "CurrCode";
			}
			rep_SalCur.DataSource = currencyCodes;
			rep_SalCur.DisplayMember = "CurrCode";
			rep_SalCur.ValueMember = "CurrCode";
			rep_SalCur.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_SalCur.ForceInitialize();
			rep_SalCur.PopulateColumns();
			foreach (LookUpColumnInfo column2 in rep_SalCur.Columns)
			{
				column2.Visible = column2.FieldName == "CurrName" || column2.FieldName == "CurrCode";
			}
			DataTable dataSource = await _httpClient.AllowanceType.GetPerIncomeTax();
			rep_TaxCode.DataSource = dataSource;
			rep_TaxCode.DisplayMember = "Name";
			rep_TaxCode.ValueMember = "Code";
			rep_TaxCode.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_TaxCode.ForceInitialize();
			rep_TaxCode.PopulateColumns();
			foreach (LookUpColumnInfo column3 in rep_TaxCode.Columns)
			{
				column3.Visible = column3.FieldName == "Code" || column3.FieldName == "Name";
			}
			btn_SaveAdvanced.Visible = false;
			btn_CancelAdvanced.Visible = false;
			btnCancel.Visible = true;
			btn_EditMain.Visible = true;
			gc_SalTotal.OptionsColumn.AllowEdit = false;
			gc_SalTotal.OptionsColumn.ReadOnly = true;
			foreach (DataRow row2 in (await _httpClient.AllowanceType.GetAllowanceType()).Rows)
			{
				Common.Table_Code item = new Common.Table_Code
				{
					Code = Function.ToString(row2["Code"]),
					Name = Function.ToString(row2["Name"])
				};
				_lstAllowanceType.Add(item);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> SaveAdvance()
	{
		string rs = string.Empty;
		try
		{
			DataTable dataTable = grd_Advanced.DataSourceTable;
			if (dataTable.IsNotNull())
			{
				DataTable dataTable2 = dataTable.Copy();
				if (string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[dataTable2.Rows.Count - 1]["Period"])))
				{
					dataTable2.Rows.RemoveAt(dataTable2.Rows.Count - 1);
					dataTable2.AcceptChanges();
				}
				if (dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["Period"]))) != null)
				{
					WinCommons.Error(Enums.MessageCode.PeriodCannotBeNull, "");
					return "Error";
				}
				if (dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["Amount"]))) != null)
				{
					WinCommons.Error(Enums.MessageCode.AmountCannotBeEmpty, "");
					return "Error";
				}
			}
			EnumerableRowCollection<DataRow> source = from t in dataTable.AsEnumerable()
				where !string.IsNullOrEmpty(Function.ToString(t["Period"]))
				select t;
			if (source.Any())
			{
				dataTable = source.CopyToDataTable().Copy();
			}
			for (int num = 0; num < dataTable.Rows.Count; num++)
			{
				dataTable.Rows[num]["UserSign2"] = GlobalData.UserID;
				dataTable.Rows[num]["UpdateDate"] = DateTime.Today;
			}
			WinCommons._OpenProcessing();
			if (dataTable.IsNotNull())
			{
				rs = await _httpClient.SalaryInfoAdvanceClient.SaveDoc(dataTable);
				if (string.IsNullOrEmpty(rs))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					FormMode = Enums.FormViewModeType.View;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, rs);
				}
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
		return rs;
	}

	private void AddNewAdvanceRow()
	{
		try
		{
			gv_Advanced.BeginInsertRow();
			DataTable dataTable = grd_Advanced.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				int num = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"]));
				DataRow dataRow = dataTable.NewRow();
				dataRow["empID"] = Function.ParseInt(gv_Employee.GetFocusedRowCellValue("empID"));
				dataRow["empName"] = dataTable.Rows[0]["empName"];
				dataRow["dept"] = dataTable.Rows[0]["dept"];
				dataRow["LineNum"] = num + 1;
				dataRow["MonthlyPay"] = "N";
				dataRow["Received"] = "N";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			else
			{
				dataTable = WinFuntions.GetTableStructure("APZ_HSI2");
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["empID"] = Function.ParseInt(gv_Employee.GetFocusedRowCellValue("empID"));
				dataRow2["LineNum"] = 0;
				dataRow2["MonthlyPay"] = "N";
				dataRow2["Received"] = "N";
				dataTable.Rows.Add(dataRow2);
				dataTable.AcceptChanges();
			}
			gv_Advanced.EndInsertRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		gv_Advanced.EndInsertRow();
	}

	protected override void NoAuthSetControlSearch()
	{
		SimpleButtonEx simpleButtonEx = btnSearch;
		bool enabled = (btn_EditMain.Enabled = true);
		simpleButtonEx.Enabled = enabled;
		LookUpEditCombo lookUpEditCombo = luk_Branch;
		CheckedComboBoxEditEx checkedComboBoxEditEx = luk_Department;
		CheckedComboBoxEditEx checkedComboBoxEditEx2 = luk_Status;
		TextEditEx textEditEx = txt_EmpNo;
		bool flag2 = (txt_EmpName.ReadOnly = false);
		bool flag4 = (textEditEx.ReadOnly = flag2);
		bool flag6 = (checkedComboBoxEditEx2.ReadOnly = flag4);
		enabled = (checkedComboBoxEditEx.ReadOnly = flag6);
		lookUpEditCombo.ReadOnly = enabled;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.Salary.frm_SalaryInfo));
		this.repositoryItemCalcEditEx3 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx4 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.tab_Detail = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_Main = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.grd_MainInfo = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.editAllMainInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_MainInfo = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Branch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Deparment = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_empNo = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CFL = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_BasicSal = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_SalesSal = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_TaxCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_TaxCode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_SalTotal = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Currency = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.tab_Allowance = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl5 = new DevExpress.XtraEditors.PanelControl();
		this.grd_Allowance = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.editAllAllowanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Allowance = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Branch1 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status1 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_DeptAllowance = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_empNo2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CFL1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_SalCur = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.tab_Advanced = new Apzon.Controls.Controls.XtraTabPageEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Employee = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Employee = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Branch2 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status2 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_deptAdvanced = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_empNo3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CFL2 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn12 = new Apzon.Controls.Grid.GridColumnEx();
		this.grd_Advanced = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Advanced = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn13 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Period = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Amount = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gridColumn15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn16 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_MonthlyPay = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumn17 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Received = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumn18 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Remarks = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbl_Department = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Branch = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_EmpNo = new Apzon.Controls.Editor.TextEditEx();
		this.txt_EmpName = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_EmpNo = new Apzon.Controls.Controls.LabelLine();
		this.lbl_FirstName = new Apzon.Controls.Controls.LabelLine();
		this.btnSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_EditMain = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_SaveAdvanced = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_CancelAdvanced = new Apzon.Controls.Controls.SimpleButtonEx();
		this.luk_Department = new Apzon.Controls.Editor.CheckedComboBoxEditEx();
		this.luk_Status = new Apzon.Controls.Editor.CheckedComboBoxEditEx();
		this.luk_Branch = new Apzon.Forms.UserControls.LookUpEditCombo();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tab_Detail).BeginInit();
		this.tab_Detail.SuspendLayout();
		this.tab_Main.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_MainInfo).BeginInit();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_MainInfo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Deparment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_TaxCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Currency).BeginInit();
		this.tab_Allowance.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl5).BeginInit();
		this.panelControl5.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Allowance).BeginInit();
		this.contextMenuStrip3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Allowance).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DeptAllowance).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SalCur).BeginInit();
		this.tab_Advanced.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Employee).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Employee).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_deptAdvanced).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Advanced).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Advanced).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Period).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MonthlyPay).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Received).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Remarks).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Department.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch.Properties).BeginInit();
		base.SuspendLayout();
		this.repositoryItemCalcEditEx3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx3.IsSearchControl = false;
		this.repositoryItemCalcEditEx3.Name = "repositoryItemCalcEditEx3";
		this.repositoryItemCalcEditEx4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx4.IsSearchControl = false;
		this.repositoryItemCalcEditEx4.Name = "repositoryItemCalcEditEx4";
		this.repositoryItemCalcEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx1.IsSearchControl = false;
		this.repositoryItemCalcEditEx1.Name = "repositoryItemCalcEditEx1";
		this.tab_Detail.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tab_Detail.IsSearchControl = false;
		this.tab_Detail.Location = new System.Drawing.Point(3, 90);
		this.tab_Detail.Name = "tab_Detail";
		this.tab_Detail.SelectedTabPage = this.tab_Main;
		this.tab_Detail.Size = new System.Drawing.Size(620, 310);
		this.tab_Detail.TabIndex = 40;
		this.tab_Detail.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[3] { this.tab_Main, this.tab_Allowance, this.tab_Advanced });
		this.tab_Detail.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.tab_Detail_SelectedPageChanged);
		this.tab_Main.Appearance.PageClient.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.tab_Main.Appearance.PageClient.Options.UseBackColor = true;
		this.tab_Main.Controls.Add(this.panelControl1);
		this.tab_Main.FieldName = null;
		this.tab_Main.IsSearchControl = false;
		this.tab_Main.Name = "tab_Main";
		this.tab_Main.Size = new System.Drawing.Size(618, 285);
		this.tab_Main.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Main.Text = "Main";
		this.panelControl1.Controls.Add(this.grd_MainInfo);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(618, 285);
		this.panelControl1.TabIndex = 0;
		this.grd_MainInfo.AllowPasteExcel = false;
		this.grd_MainInfo.ClipboardData = null;
		this.grd_MainInfo.ContextMenuStrip = this.contextMenuStrip2;
		this.grd_MainInfo.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_MainInfo.Location = new System.Drawing.Point(2, 2);
		this.grd_MainInfo.MainView = this.gv_MainInfo;
		this.grd_MainInfo.Name = "grd_MainInfo";
		this.grd_MainInfo.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.rep_Deparment, this.rep_Currency, this.rep_CFL, this.rep_Status, this.rep_Branch, this.rep_TaxCode });
		this.grd_MainInfo.Size = new System.Drawing.Size(614, 281);
		this.grd_MainInfo.TabIndex = 7;
		this.grd_MainInfo.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_MainInfo });
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.editAllMainInfoToolStripMenuItem });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(163, 26);
		this.contextMenuStrip2.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip2_Opening);
		this.editAllMainInfoToolStripMenuItem.Name = "editAllMainInfoToolStripMenuItem";
		this.editAllMainInfoToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
		this.editAllMainInfoToolStripMenuItem.Text = "Edit All MainInfo";
		this.editAllMainInfoToolStripMenuItem.Click += new System.EventHandler(this.editAllMainInfoToolStripMenuItem_Click);
		this.gv_MainInfo.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_MainInfo.ColumnKey = null;
		this.gv_MainInfo.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.gridColumnEx3, this.gridColumnEx2, this.gridColumnEx11, this.gridColumn1, this.gc_empNo, this.gridColumn3, this.gridColumnEx8, this.gc_BasicSal, this.gc_SalesSal, this.cl_TaxCode,
			this.gc_SalTotal, this.gridColumn4
		});
		this.gv_MainInfo.DisableFormSetting = false;
		this.gv_MainInfo.DisplayException = false;
		this.gv_MainInfo.EditColumn = null;
		this.gv_MainInfo.FieldCurrency = null;
		this.gv_MainInfo.FieldRate = null;
		this.gv_MainInfo.FormId = "oSalaryInfo";
		this.gv_MainInfo.GridControl = this.grd_MainInfo;
		this.gv_MainInfo.GroupCount = 1;
		this.gv_MainInfo.IgnoreFilter = false;
		this.gv_MainInfo.IndexEditRow = -1;
		this.gv_MainInfo.IndicatorWidth = 40;
		this.gv_MainInfo.IsApzTable = false;
		this.gv_MainInfo.IsInsertNewRow = false;
		this.gv_MainInfo.IsLoadingData = false;
		this.gv_MainInfo.ItemId = "APZ_OHSI";
		this.gv_MainInfo.LstColumnAllowBatch = null;
		this.gv_MainInfo.LstColumnNotAllowBatch = null;
		this.gv_MainInfo.LstColumnNotEditable = null;
		this.gv_MainInfo.LstColumNotSetting = null;
		this.gv_MainInfo.Name = "gv_MainInfo";
		this.gv_MainInfo.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_MainInfo.OptionsView.ShowGroupPanel = false;
		this.gv_MainInfo.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumn1, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gv_MainInfo.UseBatchMod = false;
		this.gv_MainInfo.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_MainInfo_CustomDrawRowIndicator);
		this.gv_MainInfo.DoubleClick += new System.EventHandler(this.gv_MainInfo_DoubleClick);
		this.gridColumnEx3.Caption = "Branch";
		this.gridColumnEx3.ColumnEdit = this.rep_Branch;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "branch";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsColumn.ReadOnly = true;
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.rep_Branch.AutoHeight = false;
		this.rep_Branch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Branch.Name = "rep_Branch";
		this.rep_Branch.NullText = "";
		this.gridColumnEx2.Caption = "Status";
		this.gridColumnEx2.ColumnEdit = this.rep_Status;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "status";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.OptionsColumn.ReadOnly = true;
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.rep_Status.AutoHeight = false;
		this.rep_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Status.Name = "rep_Status";
		this.rep_Status.NullText = "";
		this.gridColumnEx11.Caption = "Line Status";
		this.gridColumnEx11.ColumnEdit = this.rep_Status;
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "LineStatus";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.OptionsColumn.ReadOnly = true;
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumn1.Caption = "Department";
		this.gridColumn1.ColumnEdit = this.rep_Deparment;
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "dept";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.rep_Deparment.AutoHeight = false;
		this.rep_Deparment.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Deparment.Name = "rep_Deparment";
		this.rep_Deparment.NullText = "";
		this.gc_empNo.Caption = "Employee No";
		this.gc_empNo.ColumnEdit = this.rep_CFL;
		this.gc_empNo.CurrencyDateField = null;
		this.gc_empNo.CurrencyField = null;
		this.gc_empNo.CurrencyFixed = null;
		this.gc_empNo.CurrencyFixedRate = 0.0;
		this.gc_empNo.CurrencyRateField = null;
		this.gc_empNo.FieldName = "empID";
		this.gc_empNo.Name = "gc_empNo";
		this.gc_empNo.OptionsColumn.ReadOnly = true;
		this.gc_empNo.PrimaryKey = false;
		this.gc_empNo.UnitField = null;
		this.gc_empNo.Visible = true;
		this.gc_empNo.VisibleIndex = 0;
		this.gc_empNo.Width = 95;
		this.rep_CFL.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_CFL.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_CFL.Name = "rep_CFL";
		this.rep_CFL.ReadOnly = true;
		this.rep_CFL.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_CFL_ButtonClick);
		this.gridColumn3.Caption = "Employee Name";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "empName";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsColumn.ReadOnly = true;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 1;
		this.gridColumnEx8.Caption = "Extened Employee No";
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "ExtEmpNo";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.OptionsColumn.ReadOnly = true;
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 2;
		this.gridColumnEx8.Width = 125;
		this.gc_BasicSal.Caption = "Basic Salary";
		this.gc_BasicSal.ColumnEdit = this.repositoryItemCalcEditEx3;
		this.gc_BasicSal.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gc_BasicSal.CurrencyDateField = null;
		this.gc_BasicSal.CurrencyField = "SalCur";
		this.gc_BasicSal.CurrencyFixed = null;
		this.gc_BasicSal.CurrencyFixedRate = 0.0;
		this.gc_BasicSal.CurrencyRateField = null;
		this.gc_BasicSal.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gc_BasicSal.FieldName = "BasicSal";
		this.gc_BasicSal.Name = "gc_BasicSal";
		this.gc_BasicSal.OptionsColumn.ReadOnly = true;
		this.gc_BasicSal.PrimaryKey = false;
		this.gc_BasicSal.UnitField = null;
		this.gc_BasicSal.Visible = true;
		this.gc_BasicSal.VisibleIndex = 3;
		this.gc_BasicSal.Width = 95;
		this.gc_SalesSal.Caption = "Sales Salary";
		this.gc_SalesSal.ColumnEdit = this.repositoryItemCalcEditEx4;
		this.gc_SalesSal.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gc_SalesSal.CurrencyDateField = null;
		this.gc_SalesSal.CurrencyField = "SalCur";
		this.gc_SalesSal.CurrencyFixed = null;
		this.gc_SalesSal.CurrencyFixedRate = 0.0;
		this.gc_SalesSal.CurrencyRateField = null;
		this.gc_SalesSal.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gc_SalesSal.FieldName = "SalesSal";
		this.gc_SalesSal.Name = "gc_SalesSal";
		this.gc_SalesSal.OptionsColumn.ReadOnly = true;
		this.gc_SalesSal.PrimaryKey = false;
		this.gc_SalesSal.UnitField = null;
		this.gc_SalesSal.Visible = true;
		this.gc_SalesSal.VisibleIndex = 4;
		this.gc_SalesSal.Width = 95;
		this.cl_TaxCode.Caption = "Personal Income Tax";
		this.cl_TaxCode.ColumnEdit = this.rep_TaxCode;
		this.cl_TaxCode.CurrencyDateField = null;
		this.cl_TaxCode.CurrencyField = null;
		this.cl_TaxCode.CurrencyFixed = null;
		this.cl_TaxCode.CurrencyFixedRate = 0.0;
		this.cl_TaxCode.CurrencyRateField = null;
		this.cl_TaxCode.FieldName = "TaxCode";
		this.cl_TaxCode.Name = "cl_TaxCode";
		this.cl_TaxCode.OptionsColumn.ReadOnly = true;
		this.cl_TaxCode.PrimaryKey = false;
		this.cl_TaxCode.UnitField = null;
		this.cl_TaxCode.Visible = true;
		this.cl_TaxCode.VisibleIndex = 5;
		this.rep_TaxCode.AutoHeight = false;
		this.rep_TaxCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_TaxCode.Name = "rep_TaxCode";
		this.rep_TaxCode.NullText = "";
		this.gc_SalTotal.Caption = "Total";
		this.gc_SalTotal.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gc_SalTotal.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gc_SalTotal.CurrencyDateField = null;
		this.gc_SalTotal.CurrencyField = "SalCur";
		this.gc_SalTotal.CurrencyFixed = null;
		this.gc_SalTotal.CurrencyFixedRate = 0.0;
		this.gc_SalTotal.CurrencyRateField = null;
		this.gc_SalTotal.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gc_SalTotal.FieldName = "SalTotal";
		this.gc_SalTotal.Name = "gc_SalTotal";
		this.gc_SalTotal.OptionsColumn.ReadOnly = true;
		this.gc_SalTotal.PrimaryKey = false;
		this.gc_SalTotal.UnitField = null;
		this.gc_SalTotal.Visible = true;
		this.gc_SalTotal.VisibleIndex = 6;
		this.gc_SalTotal.Width = 67;
		this.gridColumn4.Caption = "Currency";
		this.gridColumn4.ColumnEdit = this.rep_Currency;
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "SalCur";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsColumn.ReadOnly = true;
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.rep_Currency.AutoHeight = false;
		this.rep_Currency.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Currency.Name = "rep_Currency";
		this.rep_Currency.NullText = "";
		this.tab_Allowance.Controls.Add(this.panelControl5);
		this.tab_Allowance.FieldName = null;
		this.tab_Allowance.IsSearchControl = false;
		this.tab_Allowance.Name = "tab_Allowance";
		this.tab_Allowance.Size = new System.Drawing.Size(618, 285);
		this.tab_Allowance.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Allowance.Text = "Allowance";
		this.panelControl5.Controls.Add(this.grd_Allowance);
		this.panelControl5.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl5.Location = new System.Drawing.Point(0, 0);
		this.panelControl5.Name = "panelControl5";
		this.panelControl5.Size = new System.Drawing.Size(618, 285);
		this.panelControl5.TabIndex = 1;
		this.grd_Allowance.AllowPasteExcel = false;
		this.grd_Allowance.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Allowance.ClipboardData = null;
		this.grd_Allowance.ContextMenuStrip = this.contextMenuStrip3;
		this.grd_Allowance.Location = new System.Drawing.Point(-1, 1);
		this.grd_Allowance.MainView = this.gv_Allowance;
		this.grd_Allowance.Name = "grd_Allowance";
		this.grd_Allowance.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rep_DeptAllowance, this.rep_SalCur, this.rep_CFL1, this.rep_Status1, this.rep_Branch1 });
		this.grd_Allowance.Size = new System.Drawing.Size(620, 281);
		this.grd_Allowance.TabIndex = 8;
		this.grd_Allowance.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Allowance });
		this.contextMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.editAllAllowanceToolStripMenuItem });
		this.contextMenuStrip3.Name = "contextMenuStrip3";
		this.contextMenuStrip3.Size = new System.Drawing.Size(170, 26);
		this.contextMenuStrip3.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip3_Opening);
		this.editAllAllowanceToolStripMenuItem.Name = "editAllAllowanceToolStripMenuItem";
		this.editAllAllowanceToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
		this.editAllAllowanceToolStripMenuItem.Text = "Edit All Allowance";
		this.editAllAllowanceToolStripMenuItem.Click += new System.EventHandler(this.editAllAllowanceToolStripMenuItem_Click);
		this.gv_Allowance.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Allowance.ColumnKey = null;
		this.gv_Allowance.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gridColumnEx5, this.gridColumnEx4, this.gridColumn7, this.gc_empNo2, this.gridColumnEx9, this.gridColumn9, this.gridColumnEx1 });
		this.gv_Allowance.DisableFormSetting = false;
		this.gv_Allowance.DisplayException = false;
		this.gv_Allowance.EditColumn = null;
		this.gv_Allowance.FieldCurrency = null;
		this.gv_Allowance.FieldRate = null;
		this.gv_Allowance.FormId = "oSalaryInfo";
		this.gv_Allowance.GridControl = this.grd_Allowance;
		this.gv_Allowance.GroupCount = 1;
		this.gv_Allowance.IgnoreFilter = false;
		this.gv_Allowance.IndexEditRow = -1;
		this.gv_Allowance.IndicatorWidth = 40;
		this.gv_Allowance.IsApzTable = false;
		this.gv_Allowance.IsInsertNewRow = false;
		this.gv_Allowance.IsLoadingData = false;
		this.gv_Allowance.ItemId = "APZ_HSI1";
		this.gv_Allowance.LstColumnAllowBatch = null;
		this.gv_Allowance.LstColumnNotAllowBatch = null;
		this.gv_Allowance.LstColumnNotEditable = null;
		this.gv_Allowance.LstColumNotSetting = null;
		this.gv_Allowance.Name = "gv_Allowance";
		this.gv_Allowance.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_Allowance.OptionsView.ColumnAutoWidth = false;
		this.gv_Allowance.OptionsView.ShowGroupPanel = false;
		this.gv_Allowance.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumn7, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gv_Allowance.UseBatchMod = false;
		this.gv_Allowance.DoubleClick += new System.EventHandler(this.gv_Allowance_DoubleClick);
		this.gridColumnEx5.Caption = "Branch";
		this.gridColumnEx5.ColumnEdit = this.rep_Branch1;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "branch";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.OptionsColumn.ReadOnly = true;
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.rep_Branch1.AutoHeight = false;
		this.rep_Branch1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Branch1.Name = "rep_Branch1";
		this.rep_Branch1.NullText = "";
		this.gridColumnEx4.Caption = "Status";
		this.gridColumnEx4.ColumnEdit = this.rep_Status1;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "status";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.OptionsColumn.ReadOnly = true;
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.rep_Status1.AutoHeight = false;
		this.rep_Status1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Status1.Name = "rep_Status1";
		this.rep_Status1.NullText = "";
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
		this.gridColumn7.OptionsColumn.AllowEdit = false;
		this.gridColumn7.OptionsColumn.ReadOnly = true;
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 0;
		this.rep_DeptAllowance.AutoHeight = false;
		this.rep_DeptAllowance.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_DeptAllowance.Name = "rep_DeptAllowance";
		this.rep_DeptAllowance.NullText = "";
		this.gc_empNo2.Caption = "Employee No.";
		this.gc_empNo2.ColumnEdit = this.rep_CFL1;
		this.gc_empNo2.CurrencyDateField = null;
		this.gc_empNo2.CurrencyField = null;
		this.gc_empNo2.CurrencyFixed = null;
		this.gc_empNo2.CurrencyFixedRate = 0.0;
		this.gc_empNo2.CurrencyRateField = null;
		this.gc_empNo2.FieldName = "empID";
		this.gc_empNo2.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gc_empNo2.Name = "gc_empNo2";
		this.gc_empNo2.OptionsColumn.ReadOnly = true;
		this.gc_empNo2.PrimaryKey = false;
		this.gc_empNo2.UnitField = null;
		this.gc_empNo2.Visible = true;
		this.gc_empNo2.VisibleIndex = 0;
		this.gc_empNo2.Width = 94;
		this.rep_CFL1.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_CFL1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_CFL1.Name = "rep_CFL1";
		this.rep_CFL1.ReadOnly = true;
		this.rep_CFL1.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_CFL1_ButtonClick);
		this.gridColumnEx9.Caption = "Extened Employee No";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "ExtEmpNo";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.OptionsColumn.ReadOnly = true;
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 2;
		this.gridColumnEx9.Width = 125;
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
		this.gridColumn9.VisibleIndex = 1;
		this.gridColumn9.Width = 157;
		this.gridColumnEx1.Caption = "Curency";
		this.gridColumnEx1.ColumnEdit = this.rep_SalCur;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "Currency";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.rep_SalCur.AutoHeight = false;
		this.rep_SalCur.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SalCur.Name = "rep_SalCur";
		this.rep_SalCur.NullText = "";
		this.tab_Advanced.Controls.Add(this.splitContainerControl1);
		this.tab_Advanced.FieldName = null;
		this.tab_Advanced.IsSearchControl = false;
		this.tab_Advanced.Name = "tab_Advanced";
		this.tab_Advanced.Size = new System.Drawing.Size(618, 285);
		this.tab_Advanced.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Advanced.Text = "Advanced";
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Employee);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Advanced);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(618, 285);
		this.splitContainerControl1.SplitterPosition = 300;
		this.splitContainerControl1.TabIndex = 1;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Employee.AllowPasteExcel = false;
		this.grd_Employee.ClipboardData = null;
		this.grd_Employee.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Employee.Location = new System.Drawing.Point(0, 0);
		this.grd_Employee.MainView = this.gv_Employee;
		this.grd_Employee.Name = "grd_Employee";
		this.grd_Employee.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rep_deptAdvanced, this.rep_CFL2, this.rep_Branch2, this.rep_Status2 });
		this.grd_Employee.Size = new System.Drawing.Size(300, 285);
		this.grd_Employee.TabIndex = 9;
		this.grd_Employee.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Employee });
		this.gv_Employee.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Employee.ColumnKey = null;
		this.gv_Employee.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumnEx7, this.gridColumnEx6, this.gridColumn10, this.gc_empNo3, this.gridColumnEx10, this.gridColumn12 });
		this.gv_Employee.DisableFormSetting = false;
		this.gv_Employee.DisplayException = false;
		this.gv_Employee.EditColumn = null;
		this.gv_Employee.FieldCurrency = null;
		this.gv_Employee.FieldRate = null;
		this.gv_Employee.FormId = "oSalaryInfo";
		this.gv_Employee.GridControl = this.grd_Employee;
		this.gv_Employee.GroupCount = 1;
		this.gv_Employee.IgnoreFilter = false;
		this.gv_Employee.IndexEditRow = -1;
		this.gv_Employee.IndicatorWidth = 40;
		this.gv_Employee.IsApzTable = false;
		this.gv_Employee.IsInsertNewRow = false;
		this.gv_Employee.IsLoadingData = false;
		this.gv_Employee.ItemId = "APZ_HSI2";
		this.gv_Employee.LstColumnAllowBatch = null;
		this.gv_Employee.LstColumnNotAllowBatch = null;
		this.gv_Employee.LstColumnNotEditable = null;
		this.gv_Employee.LstColumNotSetting = null;
		this.gv_Employee.Name = "gv_Employee";
		this.gv_Employee.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_Employee.OptionsView.ShowGroupPanel = false;
		this.gv_Employee.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumn10, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gv_Employee.UseBatchMod = false;
		this.gv_Employee.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Employee_FocusedRowChanged);
		this.gridColumnEx7.Caption = "Branch";
		this.gridColumnEx7.ColumnEdit = this.rep_Branch2;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "branch";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.OptionsColumn.ReadOnly = true;
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.rep_Branch2.AutoHeight = false;
		this.rep_Branch2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Branch2.Name = "rep_Branch2";
		this.rep_Branch2.NullText = "";
		this.gridColumnEx6.Caption = "Status";
		this.gridColumnEx6.ColumnEdit = this.rep_Status2;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "status";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.OptionsColumn.ReadOnly = true;
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.rep_Status2.AutoHeight = false;
		this.rep_Status2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Status2.Name = "rep_Status2";
		this.rep_Status2.NullText = "";
		this.gridColumn10.Caption = "Department";
		this.gridColumn10.ColumnEdit = this.rep_deptAdvanced;
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "dept";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.OptionsColumn.ReadOnly = true;
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 0;
		this.rep_deptAdvanced.AutoHeight = false;
		this.rep_deptAdvanced.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_deptAdvanced.Name = "rep_deptAdvanced";
		this.rep_deptAdvanced.NullText = "";
		this.gc_empNo3.Caption = "Employee No.";
		this.gc_empNo3.ColumnEdit = this.rep_CFL2;
		this.gc_empNo3.CurrencyDateField = null;
		this.gc_empNo3.CurrencyField = null;
		this.gc_empNo3.CurrencyFixed = null;
		this.gc_empNo3.CurrencyFixedRate = 0.0;
		this.gc_empNo3.CurrencyRateField = null;
		this.gc_empNo3.FieldName = "empID";
		this.gc_empNo3.Name = "gc_empNo3";
		this.gc_empNo3.OptionsColumn.ReadOnly = true;
		this.gc_empNo3.PrimaryKey = false;
		this.gc_empNo3.UnitField = null;
		this.gc_empNo3.Visible = true;
		this.gc_empNo3.VisibleIndex = 0;
		this.gc_empNo3.Width = 86;
		this.rep_CFL2.AutoHeight = false;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_CFL2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_CFL2.Name = "rep_CFL2";
		this.rep_CFL2.ReadOnly = true;
		this.rep_CFL2.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_CFL2_ButtonClick);
		this.gridColumnEx10.Caption = "Extened Employee No";
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "ExtEmpNo";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.OptionsColumn.ReadOnly = true;
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 2;
		this.gridColumnEx10.Width = 125;
		this.gridColumn12.Caption = "Employee Name";
		this.gridColumn12.CurrencyDateField = null;
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.FieldName = "empName";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.OptionsColumn.ReadOnly = true;
		this.gridColumn12.PrimaryKey = false;
		this.gridColumn12.UnitField = null;
		this.gridColumn12.Visible = true;
		this.gridColumn12.VisibleIndex = 1;
		this.gridColumn12.Width = 95;
		this.grd_Advanced.AllowPasteExcel = false;
		this.grd_Advanced.ClipboardData = null;
		this.grd_Advanced.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Advanced.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Advanced.Location = new System.Drawing.Point(0, 0);
		this.grd_Advanced.MainView = this.gv_Advanced;
		this.grd_Advanced.Name = "grd_Advanced";
		this.grd_Advanced.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rep_MonthlyPay, this.rep_Received, this.rep_Remarks, this.rep_Period, this.repositoryItemCalcEdit1 });
		this.grd_Advanced.Size = new System.Drawing.Size(308, 285);
		this.grd_Advanced.TabIndex = 10;
		this.grd_Advanced.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Advanced });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(144, 26);
		this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gv_Advanced.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Advanced.ColumnKey = null;
		this.gv_Advanced.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumn13, this.gc_Amount, this.gridColumn15, this.gridColumn16, this.gridColumn17, this.gridColumn18 });
		this.gv_Advanced.DisableFormSetting = false;
		this.gv_Advanced.DisplayException = false;
		this.gv_Advanced.EditColumn = null;
		this.gv_Advanced.FieldCurrency = null;
		this.gv_Advanced.FieldRate = null;
		this.gv_Advanced.FormId = "oSalaryInfo";
		this.gv_Advanced.GridControl = this.grd_Advanced;
		this.gv_Advanced.IgnoreFilter = false;
		this.gv_Advanced.IndexEditRow = -1;
		this.gv_Advanced.IndicatorWidth = 40;
		this.gv_Advanced.IsApzTable = false;
		this.gv_Advanced.IsInsertNewRow = false;
		this.gv_Advanced.IsLoadingData = false;
		this.gv_Advanced.ItemId = "APZ_HSI3";
		this.gv_Advanced.LstColumnAllowBatch = null;
		this.gv_Advanced.LstColumnNotAllowBatch = null;
		this.gv_Advanced.LstColumnNotEditable = null;
		this.gv_Advanced.LstColumNotSetting = null;
		this.gv_Advanced.Name = "gv_Advanced";
		this.gv_Advanced.OptionsView.ShowGroupPanel = false;
		this.gv_Advanced.UseBatchMod = false;
		this.gv_Advanced.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Advanced_CellValueChanged);
		this.gridColumn13.Caption = "Period";
		this.gridColumn13.ColumnEdit = this.rep_Period;
		this.gridColumn13.CurrencyDateField = null;
		this.gridColumn13.CurrencyField = null;
		this.gridColumn13.CurrencyFixed = null;
		this.gridColumn13.CurrencyFixedRate = 0.0;
		this.gridColumn13.CurrencyRateField = null;
		this.gridColumn13.FieldName = "Period";
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.PrimaryKey = false;
		this.gridColumn13.UnitField = null;
		this.gridColumn13.Visible = true;
		this.gridColumn13.VisibleIndex = 0;
		this.rep_Period.AutoHeight = false;
		this.rep_Period.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Period.Name = "rep_Period";
		this.rep_Period.NullText = "";
		this.gc_Amount.Caption = "Amount";
		this.gc_Amount.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gc_Amount.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gc_Amount.CurrencyDateField = null;
		this.gc_Amount.CurrencyField = null;
		this.gc_Amount.CurrencyFixed = null;
		this.gc_Amount.CurrencyFixedRate = 0.0;
		this.gc_Amount.CurrencyRateField = null;
		this.gc_Amount.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gc_Amount.FieldName = "Amount";
		this.gc_Amount.Name = "gc_Amount";
		this.gc_Amount.PrimaryKey = false;
		this.gc_Amount.UnitField = null;
		this.gc_Amount.Visible = true;
		this.gc_Amount.VisibleIndex = 1;
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.gridColumn15.Caption = "Remain";
		this.gridColumn15.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gridColumn15.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gridColumn15.CurrencyDateField = null;
		this.gridColumn15.CurrencyField = null;
		this.gridColumn15.CurrencyFixed = null;
		this.gridColumn15.CurrencyFixedRate = 0.0;
		this.gridColumn15.CurrencyRateField = null;
		this.gridColumn15.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumn15.FieldName = "Remain";
		this.gridColumn15.Name = "gridColumn15";
		this.gridColumn15.PrimaryKey = false;
		this.gridColumn15.UnitField = null;
		this.gridColumn15.Visible = true;
		this.gridColumn15.VisibleIndex = 2;
		this.gridColumn16.Caption = "Monthly Pay";
		this.gridColumn16.ColumnEdit = this.rep_MonthlyPay;
		this.gridColumn16.CurrencyDateField = null;
		this.gridColumn16.CurrencyField = null;
		this.gridColumn16.CurrencyFixed = null;
		this.gridColumn16.CurrencyFixedRate = 0.0;
		this.gridColumn16.CurrencyRateField = null;
		this.gridColumn16.FieldName = "MonthlyPay";
		this.gridColumn16.Name = "gridColumn16";
		this.gridColumn16.PrimaryKey = false;
		this.gridColumn16.UnitField = null;
		this.gridColumn16.Visible = true;
		this.gridColumn16.VisibleIndex = 3;
		this.rep_MonthlyPay.AutoHeight = false;
		this.rep_MonthlyPay.Name = "rep_MonthlyPay";
		this.rep_MonthlyPay.ValueChecked = "Y";
		this.rep_MonthlyPay.ValueUnchecked = "N";
		this.gridColumn17.Caption = "Received";
		this.gridColumn17.ColumnEdit = this.rep_Received;
		this.gridColumn17.CurrencyDateField = null;
		this.gridColumn17.CurrencyField = null;
		this.gridColumn17.CurrencyFixed = null;
		this.gridColumn17.CurrencyFixedRate = 0.0;
		this.gridColumn17.CurrencyRateField = null;
		this.gridColumn17.FieldName = "Received";
		this.gridColumn17.Name = "gridColumn17";
		this.gridColumn17.PrimaryKey = false;
		this.gridColumn17.UnitField = null;
		this.gridColumn17.Visible = true;
		this.gridColumn17.VisibleIndex = 4;
		this.rep_Received.AutoHeight = false;
		this.rep_Received.Name = "rep_Received";
		this.rep_Received.ValueChecked = "Y";
		this.rep_Received.ValueUnchecked = "N";
		this.gridColumn18.Caption = "Remarks";
		this.gridColumn18.ColumnEdit = this.rep_Remarks;
		this.gridColumn18.CurrencyDateField = null;
		this.gridColumn18.CurrencyField = null;
		this.gridColumn18.CurrencyFixed = null;
		this.gridColumn18.CurrencyFixedRate = 0.0;
		this.gridColumn18.CurrencyRateField = null;
		this.gridColumn18.FieldName = "Remarks";
		this.gridColumn18.Name = "gridColumn18";
		this.gridColumn18.PrimaryKey = false;
		this.gridColumn18.UnitField = null;
		this.gridColumn18.Visible = true;
		this.gridColumn18.VisibleIndex = 5;
		this.rep_Remarks.AutoHeight = false;
		this.rep_Remarks.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Remarks.Name = "rep_Remarks";
		this.rep_Remarks.ShowIcon = false;
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(4, 406);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 57;
		this.btnCancel.Text = "Close";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
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
		this.labelLine3.Location = new System.Drawing.Point(4, 64);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(117, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 85;
		this.labelLine3.Utext = "Status";
		this.txt_EmpNo.FieldName = null;
		this.txt_EmpNo.IsSearchControl = true;
		this.txt_EmpNo.Location = new System.Drawing.Point(455, 10);
		this.txt_EmpNo.Name = "txt_EmpNo";
		this.txt_EmpNo.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_EmpNo.Size = new System.Drawing.Size(167, 20);
		this.txt_EmpNo.TabIndex = 4;
		this.txt_EmpNo.Tag = " ";
		this.txt_EmpName.FieldName = null;
		this.txt_EmpName.IsSearchControl = true;
		this.txt_EmpName.Location = new System.Drawing.Point(455, 36);
		this.txt_EmpName.Name = "txt_EmpName";
		this.txt_EmpName.Size = new System.Drawing.Size(167, 20);
		this.txt_EmpName.TabIndex = 5;
		this.lbl_EmpNo.FieldName = null;
		this.lbl_EmpNo.HideWhenEmpty = false;
		this.lbl_EmpNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_EmpNo.Location = new System.Drawing.Point(330, 14);
		this.lbl_EmpNo.Name = "lbl_EmpNo";
		this.lbl_EmpNo.ShowUnderLine = true;
		this.lbl_EmpNo.Size = new System.Drawing.Size(126, 20);
		this.lbl_EmpNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_EmpNo.TabIndex = 89;
		this.lbl_EmpNo.Utext = "Employee No.";
		this.lbl_FirstName.FieldName = null;
		this.lbl_FirstName.HideWhenEmpty = false;
		this.lbl_FirstName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_FirstName.Location = new System.Drawing.Point(330, 39);
		this.lbl_FirstName.Name = "lbl_FirstName";
		this.lbl_FirstName.ShowUnderLine = true;
		this.lbl_FirstName.Size = new System.Drawing.Size(126, 20);
		this.lbl_FirstName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_FirstName.TabIndex = 88;
		this.lbl_FirstName.Utext = "Employee's Name";
		this.btnSearch.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnSearch.Appearance.Options.UseBackColor = true;
		this.btnSearch.FieldName = null;
		this.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		this.btnSearch.IsSearchControl = true;
		this.btnSearch.Location = new System.Drawing.Point(523, 62);
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.Size = new System.Drawing.Size(99, 20);
		this.btnSearch.TabIndex = 6;
		this.btnSearch.Text = "Search";
		this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
		this.btn_EditMain.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_EditMain.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_EditMain.Appearance.Options.UseBackColor = true;
		this.btn_EditMain.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_EditMain.FieldName = null;
		this.btn_EditMain.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_EditMain.ImageOptions.Image");
		this.btn_EditMain.IsSearchControl = true;
		this.btn_EditMain.Location = new System.Drawing.Point(485, 406);
		this.btn_EditMain.Name = "btn_EditMain";
		this.btn_EditMain.Size = new System.Drawing.Size(138, 20);
		this.btn_EditMain.TabIndex = 11;
		this.btn_EditMain.Text = "Edit Main information";
		this.btn_EditMain.Click += new System.EventHandler(this.btn_EditMain_Click);
		this.btn_SaveAdvanced.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_SaveAdvanced.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_SaveAdvanced.Appearance.Options.UseBackColor = true;
		this.btn_SaveAdvanced.FieldName = null;
		this.btn_SaveAdvanced.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_SaveAdvanced.ImageOptions.Image");
		this.btn_SaveAdvanced.IsSearchControl = false;
		this.btn_SaveAdvanced.Location = new System.Drawing.Point(6, 406);
		this.btn_SaveAdvanced.Name = "btn_SaveAdvanced";
		this.btn_SaveAdvanced.Size = new System.Drawing.Size(93, 20);
		this.btn_SaveAdvanced.TabIndex = 95;
		this.btn_SaveAdvanced.Text = "OK";
		this.btn_SaveAdvanced.Click += new System.EventHandler(this.btn_SaveAdvanced_Click);
		this.btn_CancelAdvanced.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_CancelAdvanced.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_CancelAdvanced.Appearance.Options.UseBackColor = true;
		this.btn_CancelAdvanced.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_CancelAdvanced.FieldName = null;
		this.btn_CancelAdvanced.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_CancelAdvanced.ImageOptions.Image");
		this.btn_CancelAdvanced.IsSearchControl = false;
		this.btn_CancelAdvanced.Location = new System.Drawing.Point(103, 406);
		this.btn_CancelAdvanced.Name = "btn_CancelAdvanced";
		this.btn_CancelAdvanced.Size = new System.Drawing.Size(93, 20);
		this.btn_CancelAdvanced.TabIndex = 94;
		this.btn_CancelAdvanced.Text = "Cancel";
		this.btn_CancelAdvanced.Click += new System.EventHandler(this.btn_CancelAdvanced_Click);
		this.luk_Department.FieldName = null;
		this.luk_Department.IsSearchControl = true;
		this.luk_Department.Location = new System.Drawing.Point(120, 36);
		this.luk_Department.Name = "luk_Department";
		this.luk_Department.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Department.Size = new System.Drawing.Size(167, 20);
		this.luk_Department.TabIndex = 2;
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
		base.AcceptButton = this.btnSearch;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(628, 434);
		base.Controls.Add(this.luk_Status);
		base.Controls.Add(this.luk_Department);
		base.Controls.Add(this.luk_Branch);
		base.Controls.Add(this.btn_SaveAdvanced);
		base.Controls.Add(this.btn_CancelAdvanced);
		base.Controls.Add(this.btn_EditMain);
		base.Controls.Add(this.btnSearch);
		base.Controls.Add(this.txt_EmpNo);
		base.Controls.Add(this.txt_EmpName);
		base.Controls.Add(this.lbl_EmpNo);
		base.Controls.Add(this.lbl_FirstName);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.lbl_Branch);
		base.Controls.Add(this.lbl_Department);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.tab_Detail);
		base.FormId = "oSalaryInfo";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SalaryInfo";
		base.ShowInTaskbar = false;
		this.Text = "Employee Salary Information";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_SalaryInfo_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tab_Detail).EndInit();
		this.tab_Detail.ResumeLayout(false);
		this.tab_Main.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_MainInfo).EndInit();
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_MainInfo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Deparment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_TaxCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Currency).EndInit();
		this.tab_Allowance.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl5).EndInit();
		this.panelControl5.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Allowance).EndInit();
		this.contextMenuStrip3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Allowance).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DeptAllowance).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SalCur).EndInit();
		this.tab_Advanced.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Employee).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Employee).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_deptAdvanced).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Advanced).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Advanced).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Period).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MonthlyPay).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Received).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Remarks).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Department.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
