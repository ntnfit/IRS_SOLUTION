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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources.Salary;

public class frm_SalAllowance : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public ReloadMain ReloadMain;

	public DataTable DtSearch;

	private bool _flag;

	private bool _isCancel;

	private bool _forceClose;

	private readonly List<Common.Table_Code> _lstAllowanceType = new List<Common.Table_Code>();

	private RepositoryItemButtonEditCFL rep_CFL_NoSearch = new RepositoryItemButtonEditCFL();

	private RepositoryItemButtonEditCFL rep_empName_NoSearch = new RepositoryItemButtonEditCFL();

	private IContainer components;

	public SimpleButtonEx btnCancel;

	private LabelLine lbl_ValidFrom;

	private LabelLine lbl_ValidTo;

	private GridControlEx grd_Allowance;

	private GridViewEx gv_Allowance;

	private GridColumnEx gridColumn1;

	private RepositoryItemLookUpEdit rep_Deparment;

	private GridColumnEx gc_EmpNo;

	private GridColumnEx gc_empName;

	public SimpleButtonEx btn_Save;

	private GridColumnEx gridColumn4;

	private RepositoryItemLookUpEdit rep_SalCur;

	public SimpleButtonEx btnSearch;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem duplicateRowToolStripMenuItem;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private RepositoryItemButtonEditCFL rep_CFL;

	private ToolStripMenuItem newRowToolStripMenuItem;

	private RepositoryItemButtonEditCFL rep_empName;

	private GridColumnEx gridColumnEx4;

	private RepositoryItemLookUpEdit rep_Branch;

	private GridColumnEx gridColumnEx3;

	private RepositoryItemLookUpEdit rep_Status;

	private RepositoryItemButtonEdit repositoryItemButtonEdit1;

	private DateEditEx date_ValidTo;

	private DateEditEx date_ValidFrom;

	private GridColumnEx gridColumnEx5;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_SalAllowance()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SalaryInfo"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SalaryInfo" && x.authoriza == "F"))
			{
				Controls.EnableDisableControl(isEnable: false);
				btnSearch.Enabled = true;
				date_ValidTo.ReadOnly = (date_ValidFrom.ReadOnly = false);
			}
			FormId = "oSalAllowance";
			gv_Allowance.FormId = FormId;
			gv_Allowance.ItemId = "APZ_HSI1";
			gv_Allowance.ChangeLogType = Enums.ChangeLog.clSalAllowance;
			gv_Allowance.IsApzTable = true;
			return;
		}
		throw new Exception(Apzon.Commons.Properties.Resources.Msg_FormAccessDenied);
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				btn_Save.Enabled = false;
				btnCancel.Enabled = false;
				string value = await SaveData();
				btn_Save.Enabled = true;
				btnCancel.Enabled = true;
				if (string.IsNullOrEmpty(value))
				{
					await LoadData();
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

	private void gv_MainInfo_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_SalaryInfo_Load(object sender, EventArgs e)
	{
		try
		{
			LoadInitForm();
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
					if (string.IsNullOrEmpty(await SaveData()))
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

	private void date_ValidFrom_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Allowance.DataSourceTable;
			if (!_flag)
			{
				dataSourceTable.IsNotNull();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void date_ValidTo_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Allowance.DataSourceTable;
			if (!_flag)
			{
				dataSourceTable.IsNotNull();
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
			if (date_ValidFrom.EditValue == null)
			{
				WinCommons.Error(Enums.MessageCode.ValidFromDateCannotBeNull, "");
			}
			else if (Function.ParseDateTimes(date_ValidTo.EditValue) < Function.ParseDateTimes(date_ValidFrom.EditValue) && date_ValidTo.EditValue != null)
			{
				WinCommons.Error(Enums.MessageCode.ValidFromDateCannotBeLargeThanValidToDate, "");
			}
			else if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				await LoadData();
				WinCommons._CloseProcessing();
			}
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
	}

	private void gv_MainInfo_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Allowance.GetFocusedRowCellValue("empID"));
			string text = Function.ToString(gv_Allowance.GetFocusedRowCellDisplayText("empName"));
			if (grd_Allowance.DataSourceTable.IsNotNull() && num > 0)
			{
				frm_SalAllowanceByEmp frm_SalAllowanceByEmp2 = new frm_SalAllowanceByEmp();
				frm_SalAllowanceByEmp2.MdiParent = MdiParent;
				frm_SalAllowanceByEmp2.EmpId = num;
				frm_SalAllowanceByEmp2.Text = Text + " - " + text;
				frm_SalAllowanceByEmp2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void duplicateRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Allowance.DataSourceTable;
			if (gv_Allowance.FocusedRowHandle < 0 || (string.IsNullOrEmpty(Function.ToString(gv_Allowance.GetFocusedRowCellValue("ValidFrom"))) && string.IsNullOrEmpty(Function.ToString(gv_Allowance.GetFocusedRowCellValue("ValidTo")))) || dataSourceTable == null)
			{
				return;
			}
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow["empID"] = ((gv_Allowance.GetFocusedRowCellValue("empID") != null) ? ((object)Function.ParseInt(gv_Allowance.GetFocusedRowCellValue("empID"))) : DBNull.Value);
			dataRow["empName"] = gv_Allowance.GetFocusedRowCellValue("empName");
			dataRow["dept"] = ((gv_Allowance.GetFocusedRowCellValue("dept") != null) ? ((object)Function.ParseInt(gv_Allowance.GetFocusedRowCellValue("dept"))) : DBNull.Value);
			dataRow["LineNum"] = DBNull.Value;
			dataRow["Currency"] = gv_Allowance.GetFocusedRowCellValue("Currency");
			dataRow["ExtEmpNo"] = gv_Allowance.GetFocusedRowCellValue("ExtEmpNo");
			foreach (DataColumn column in dataSourceTable.Columns)
			{
				if (column.ColumnName.Contains("EmpAllowance"))
				{
					dataRow[column.ColumnName] = dataSourceTable.Rows[0][column.ColumnName];
				}
			}
			dataSourceTable.Rows.Add(dataRow);
			dataSourceTable.AcceptChanges();
			FormMode = Enums.FormViewModeType.Update;
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
			DataTable dt = grd_Allowance.DataSourceTable;
			int num = Function.ParseInt(gv_Allowance.GetFocusedRowCellValue("LineNum"));
			if (gv_Allowance.FocusedRowHandle < 0)
			{
				return;
			}
			if (num == 0)
			{
				gv_Allowance.BeginUpdate();
				gv_Allowance.DeleteSelectedRows();
				gv_Allowance.EndUpdate();
				dt.AcceptChanges();
				grd_Allowance.RefreshDataSource();
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				string text = await _httpClient.SalaryAllowanceByEmpClient.DeleteDoc(Function.ParseInt(num), Function.ParseInt(gv_Allowance.GetFocusedRowCellValue("empID")));
				if (string.IsNullOrEmpty(text))
				{
					gv_Allowance.BeginUpdate();
					gv_Allowance.DeleteSelectedRows();
					gv_Allowance.EndUpdate();
					dt.AcceptChanges();
					grd_Allowance.RefreshDataSource();
					WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
					if (ReloadMain != null && !((XtraForm)ReloadMain.Target).IsDisposed)
					{
						ReloadMain();
					}
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

	private void rep_CFL_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
	}

	private void newRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewRow();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Allowance_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (Function.ParseInt(gv_Allowance.GetRowCellValue(e.RowHandle, "LineNum")) != 0)
			{
				e.Appearance.BackColor = Color.Wheat;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Allowance_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gv_Allowance.FocusedColumn.FieldName == "LineNum")
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

	private async void LoadInitForm()
	{
		_ = 3;
		try
		{
			DataTable dataTable = await _httpClient.EmployeeMasterClient.GetEmployeeStatus();
			rep_Status.DataSource = dataTable.Copy();
			rep_Status.DisplayMember = "Name";
			rep_Status.ValueMember = "Code";
			rep_Status.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Status.ForceInitialize();
			rep_Status.PopulateColumns();
			rep_Status.Columns["Code"].Visible = false;
			DataTable dataTable2 = await _httpClient.EmployeeMasterClient.LoadComboBranch();
			DataRow row = dataTable2.NewRow();
			dataTable2.Rows.InsertAt(row, 0);
			dataTable2.AcceptChanges();
			rep_Branch.DataSource = dataTable2.Copy();
			rep_Branch.DisplayMember = "Name";
			rep_Branch.ValueMember = "Code";
			rep_Branch.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Branch.ForceInitialize();
			rep_Branch.PopulateColumns();
			rep_Branch.Columns["Code"].Visible = false;
			DataTable dataTable3 = await _httpClient.EmployeeMasterClient.LoadComboDept();
			DataRow row2 = dataTable3.NewRow();
			dataTable3.Rows.InsertAt(row2, 0);
			dataTable3.AcceptChanges();
			rep_Deparment.DataSource = dataTable3.Copy();
			rep_Deparment.DisplayMember = "Name";
			rep_Deparment.ValueMember = "Code";
			rep_Deparment.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Deparment.ForceInitialize();
			rep_Deparment.PopulateColumns();
			rep_Deparment.Columns["Code"].Visible = false;
			rep_Deparment.Columns["Remarks"].Visible = false;
			List<CurrencyCodesImpl> currencyCodes = GlobalData.CurrencyCodes;
			rep_SalCur.DataSource = currencyCodes;
			rep_SalCur.DisplayMember = "CurrCode";
			rep_SalCur.ValueMember = "CurrCode";
			rep_SalCur.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_SalCur.ForceInitialize();
			rep_SalCur.PopulateColumns();
			foreach (LookUpColumnInfo column in rep_SalCur.Columns)
			{
				column.Visible = column.FieldName == "CurrName" || column.FieldName == "CurrCode";
			}
			_flag = true;
			date_ValidFrom.EditValue = DateTime.Today.AddDays(-Configuration.SearchDate);
			date_ValidTo.EditValue = DateTime.Today.AddDays(Configuration.SearchDate);
			_flag = false;
			CreateView(await _httpClient.SalaryInfoAllowanceClient.SearchByDate(DtSearch));
			rep_CFL.ValueMember = "empID";
			rep_CFL.DisplayMember = "empID";
			rep_CFL.ContainerControl = this;
			rep_CFL.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			rep_CFL.AutoLoadDataSource = false;
			rep_CFL.FindCFLBy(Enums.TypeCFL.Employees, true, "Apz_Cfl_EmployleeData", Enums.ItemObjectType.NoItemType, null, "");
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = rep_CFL;
			repositoryItemButtonEditCFL.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL cfl) =>
			{
				cfl.Branch = Function.ParseInt(DtSearch.Rows[0]["branch"]);
			}));
			rep_CFL.RefreshDataSource();
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = rep_CFL;
			repositoryItemButtonEditCFL2.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL2.HandleChooseItem, new Action<DataRow>(FillData));
			rep_CFL.HandleChooseItemMulti = HandleChooseItemMulti;
			rep_empName.ValueMember = "empID";
			rep_empName.DisplayMember = "lastName";
			rep_empName.DisplayMember2 = "firstName";
			rep_empName.SeparateString = " ";
			rep_empName.ContainerControl = this;
			rep_CFL.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			rep_empName.AutoLoadDataSource = false;
			rep_empName.FindCFLBy(Enums.TypeCFL.Employees, true, "Apz_Cfl_EmployleeData", Enums.ItemObjectType.NoItemType, null, "");
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL3 = rep_empName;
			repositoryItemButtonEditCFL3.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL3.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL cfl) =>
			{
				cfl.Branch = Function.ParseInt(DtSearch.Rows[0]["branch"]);
			}));
			rep_empName.RefreshDataSource();
			gc_empName.FilterMode = ColumnFilterMode.DisplayText;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL4 = rep_empName;
			repositoryItemButtonEditCFL4.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL4.HandleChooseItem, new Action<DataRow>(FillData));
			rep_empName.HandleChooseItemMulti = HandleChooseItemMulti;
			rep_CFL_NoSearch.Assign(rep_CFL);
			rep_CFL_NoSearch.Buttons[0].Visible = false;
			rep_CFL_NoSearch.TextEditStyle = TextEditStyles.DisableTextEditor;
			rep_empName_NoSearch.Assign(rep_empName);
			rep_empName_NoSearch.Buttons[0].Visible = false;
			rep_empName_NoSearch.TextEditStyle = TextEditStyles.DisableTextEditor;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void HandleChooseItemMulti(List<DataRow> rows)
	{
		try
		{
			if (!(grd_Allowance.DataSource is DataTable dataTable) || dataTable.Rows.Count <= 0 || rows.Count <= 0)
			{
				return;
			}
			gv_Allowance.SetFocusedRowCellValue("empID", Function.ParseInt(rows[0]["EmpId"]));
			gv_Allowance.SetFocusedRowCellValue("empName", Function.ParseInt(rows[0]["EmpId"]));
			gv_Allowance.SetFocusedRowCellValue("dept", Function.ParseInt(rows[0]["dept"]));
			gv_Allowance.SetFocusedRowCellValue("ExtEmpNo", Function.ToString(rows[0]["ExtEmpNo"]));
			for (int i = 1; i < rows.Count; i++)
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["EmpId"])));
				if (dataRow != null)
				{
					int dataSourceIndex = dataTable.Rows.IndexOf(dataRow);
					gv_Allowance.SetRowCellValue(gv_Allowance.GetRowHandle(dataSourceIndex), "empID", Function.ParseInt(rows[i]["EmpId"]));
					gv_Allowance.SetRowCellValue(gv_Allowance.GetRowHandle(dataSourceIndex), "empName", Function.ParseInt(rows[i]["EmpId"]));
					gv_Allowance.SetRowCellValue(gv_Allowance.GetRowHandle(dataSourceIndex), "dept", Function.ParseInt(rows[i]["dept"]));
					gv_Allowance.SetRowCellValue(gv_Allowance.GetRowHandle(dataSourceIndex), "ExtEmpNo", Function.ParseInt(rows[i]["ExtEmpNo"]));
					continue;
				}
				AddNewRow();
				DataRow row = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["empID"])));
				int dataSourceIndex2 = dataTable.Rows.IndexOf(row);
				gv_Allowance.SetRowCellValue(gv_Allowance.GetRowHandle(dataSourceIndex2), "empID", Function.ParseInt(rows[i]["EmpId"]));
				gv_Allowance.SetRowCellValue(gv_Allowance.GetRowHandle(dataSourceIndex2), "empName", Function.ParseInt(rows[i]["EmpId"]));
				gv_Allowance.SetRowCellValue(gv_Allowance.GetRowHandle(dataSourceIndex2), "dept", Function.ParseInt(rows[i]["dept"]));
				gv_Allowance.SetRowCellValue(gv_Allowance.GetRowHandle(dataSourceIndex2), "ExtEmpNo", Function.ParseInt(rows[i]["ExtEmpNo"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void FillData(DataRow dataRow)
	{
		try
		{
			if (Function.ParseInt(gv_Allowance.GetFocusedRowCellValue("LineNum")) == 0)
			{
				gv_Allowance.SetFocusedRowCellValue("empID", Function.ParseInt(dataRow["empID"]));
				gv_Allowance.SetFocusedRowCellValue("dept", Function.ParseInt(dataRow["dept"]));
				gv_Allowance.SetFocusedRowCellValue("ExtEmpNo", Function.ToString(dataRow["ExtEmpNo"]));
				gv_Allowance.SetFocusedRowCellValue("empName", Function.ParseInt(dataRow["empID"]));
				gv_Allowance.PostEditor();
				gv_Allowance.RefreshData();
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.CannotReselectedEmployee4ExistedRecord, "");
			}
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
			if (DtSearch.Rows.Count > 0)
			{
				DtSearch.Rows[0]["validFrom"] = Function.ParseDateTimes(date_ValidFrom.EditValue);
				DtSearch.Rows[0]["validTo"] = ((date_ValidTo.EditValue == null) ? DBNull.Value : ((object)Function.ParseDateTimes(date_ValidTo.EditValue)));
			}
			DataTable dataTable = await _httpClient.SalaryInfoAllowanceClient.SearchByDate(DtSearch);
			grd_Allowance.DataSource = dataTable;
			grd_Allowance.RefreshDataSource();
			if (dataTable.IsNotNull())
			{
				CreateView(dataTable);
				grd_Allowance.DataSource = dataTable;
				grd_Allowance.RefreshDataSource();
			}
			gv_Allowance.ExpandAllGroups();
			gv_Allowance.LayoutChanged();
			await gv_Allowance.ReloadFormSetting();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CreateView(DataTable dt)
	{
		try
		{
			RepositoryItemCalcEdit columnEdit = new RepositoryItemCalcEdit();
			foreach (DataColumn col in dt.Columns)
			{
				if (!(col.ColumnName != "empID") || !(col.ColumnName != "ExtEmpNo") || !(col.ColumnName != "empName") || !(col.ColumnName != "ExtEmpNo") || !(col.ColumnName != "LineNum") || !(col.ColumnName != "ValidFrom") || !(col.ColumnName != "ValidTo") || !(col.ColumnName != "dept") || !(col.ColumnName != "Code") || col.ColumnName.StartsWith("U_"))
				{
					continue;
				}
				GridColumn gridColumn = gv_Allowance.Columns.AsEnumerable().FirstOrDefault((GridColumn t) => t.FieldName == col.ColumnName);
				if (gridColumn == null)
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
					gridColumnEx.DisplayFormatType = Enums.DisplayFormatType.Amount;
					gridColumnEx.ColumnEdit = columnEdit;
					gv_Allowance.BeginUpdate();
					gv_Allowance.Columns.Insert(gv_Allowance.Columns.Count, gridColumnEx);
					gv_Allowance.EndUpdate();
				}
				else if (col.DataType.Name == "Decimal")
				{
					GridColumnEx obj = gridColumn as GridColumnEx;
					obj.ColumnEdit = columnEdit;
					obj.DisplayFormatType = Enums.DisplayFormatType.Amount;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> SaveData()
	{
		try
		{
			int num = 0;
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_HSI1");
			DataTable dataSourceTable = grd_Allowance.DataSourceTable;
			if (dataSourceTable.IsNotNull())
			{
				if (dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["EmpId"]))) != null)
				{
					WinCommons.Error(Enums.MessageCode.EmpIdCannotBeEmpty, "");
					return "Error";
				}
				DataRow dataRow = dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["ValidFrom"])));
				if (dataRow != null)
				{
					WinCommons.Error(Enums.MessageCode.FromDateCannotBeEmptyWithEmpId, "", Function.ToString(dataRow["EmpId"]));
					return "Error";
				}
				DataRow dataRow2 = dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseDateTimes(t["ValidFrom"]) > Function.ParseDateTimes(t["ValidTo"]) && t["ValidTo"] != DBNull.Value);
				if (dataRow2 != null)
				{
					WinCommons.Error(Enums.MessageCode.ValidFromCannotbeLargeThanValidToDateWithEmpId, "", Function.ToString(dataRow2["EmpId"]));
					return "Error";
				}
				foreach (int item in (from t in dataSourceTable.AsEnumerable()
					select Function.ParseInt(t["empID"])).Distinct().ToList())
				{
					int num2 = Function.ParseInt(_httpClient.SalaryInfoAllowanceClient.GetMaxLineNum(item).Rows[0]["MaxLineNum"]);
					foreach (DataRow row in dataSourceTable.Rows)
					{
						if (Function.ParseInt(row["empID"]) != item)
						{
							continue;
						}
						if (Function.ParseInt(row["LineNum"]) != 0)
						{
							row["LineNum"] = Function.ParseInt(row["LineNum"]);
						}
						else
						{
							row["LineNum"] = num2 + 1;
							num2++;
						}
						num += (from DataColumn col in dataSourceTable.Columns
							where col.ColumnName != "empID" && col.ColumnName != "empName" && col.ColumnName != "LineNum" && col.ColumnName != "ValidFrom" && col.ColumnName != "ValidTo" && col.ColumnName != "dept" && col.ColumnName != "Code" && col.ColumnName != "Currency" && col.ColumnName != "ExtEmpNo"
							select col).Count((DataColumn col) => row[col.ColumnName] != DBNull.Value);
						if (num <= 0)
						{
							continue;
						}
						foreach (DataColumn column in dataSourceTable.Columns)
						{
							if (column.ColumnName.Contains("_"))
							{
								string[] array = column.ColumnName.Split('_');
								if (array.Length >= 3)
								{
									DataRow dataRow3 = tableStructure.NewRow();
									dataRow3["empID"] = Function.ParseInt(row["empID"]);
									dataRow3["LineNum"] = row["LineNum"];
									dataRow3["Currency"] = row["Currency"];
									dataRow3["ValidFrom"] = Function.ParseDateTimes(row["ValidFrom"]).Date;
									dataRow3["ValidTo"] = ((row["ValidTo"] == DBNull.Value) ? DBNull.Value : ((object)Function.ParseDateTimes(row["ValidTo"]).Date));
									dataRow3["Amount"] = Function.ParseDouble(row[column.ColumnName]);
									dataRow3["AllowanceType"] = Function.ParseInt(array[1]);
									dataRow3["UserSign2"] = GlobalData.UserID;
									dataRow3["UpdateDate"] = DateTime.Today;
									tableStructure.Rows.Add(dataRow3);
								}
							}
						}
					}
				}
				tableStructure.AcceptChanges();
			}
			WinCommons._OpenProcessing();
			if (tableStructure.IsNotNull())
			{
				string text = await _httpClient.SalaryInfoAllowanceClient.SaveDoc(tableStructure);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					FormMode = Enums.FormViewModeType.View;
					REFRESH_DATA();
					if (ReloadMain != null && !((XtraForm)ReloadMain.Target).IsDisposed)
					{
						ReloadMain();
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
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
		return string.Empty;
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				btnSearch_Click(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void AddNewRow()
	{
		DataTable dt = grd_Allowance.DataSourceTable();
		try
		{
			gv_Allowance.BeginInsertRow();
			if (!dt.IsNotNull())
			{
				if (DtSearch.Rows.Count > 0)
				{
					DtSearch.Rows[0]["validFrom"] = Function.ParseDateTimes(date_ValidFrom.EditValue);
					DtSearch.Rows[0]["validTo"] = ((date_ValidTo.EditValue == null) ? DBNull.Value : ((object)Function.ParseDateTimes(date_ValidTo.EditValue)));
				}
				DataTable dataTable = await _httpClient.SalaryInfoAllowanceClient.SearchByDate(DtSearch);
				CreateView(dataTable);
				dt = dataTable.Clone();
			}
			DataRow dataRow = dt.NewRow();
			dataRow["empID"] = DBNull.Value;
			dataRow["LineNum"] = DBNull.Value;
			dt.Rows.Add(dataRow);
			grd_Allowance.DataSource = dt;
			grd_Allowance.RefreshDataSource();
			gv_Allowance.EndInsertRow();
			gv_Allowance.ExpandAllGroups();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		gv_Allowance.EndInsertRow();
		dt.AcceptChanges();
	}

	private void gv_Allowance_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column == gc_EmpNo)
			{
				if (Function.ParseInt(gv_Allowance.GetRowCellValue(e.RowHandle, "LineNum")) != 0)
				{
					e.RepositoryItem = rep_CFL_NoSearch;
				}
				else
				{
					e.RepositoryItem = rep_CFL;
				}
			}
			if (e.Column == gc_empName)
			{
				if (Function.ParseInt(gv_Allowance.GetRowCellValue(e.RowHandle, "LineNum")) != 0)
				{
					e.RepositoryItem = rep_empName_NoSearch;
				}
				else
				{
					e.RepositoryItem = rep_empName;
				}
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
			if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SalaryInfo" && x.authoriza == "R"))
			{
				WinCommons.Error(Enums.MessageCode.YouDoNorHavePermisionToModifiedForm, "");
				return;
			}
			AddNewRow();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void NoAuthSetControlSearch()
	{
		btnSearch.Enabled = true;
		DateEditEx dateEditEx = date_ValidTo;
		bool readOnly = (date_ValidFrom.ReadOnly = false);
		dateEditEx.ReadOnly = readOnly;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.Salary.frm_SalAllowance));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbl_ValidFrom = new Apzon.Controls.Controls.LabelLine();
		this.lbl_ValidTo = new Apzon.Controls.Controls.LabelLine();
		this.grd_Allowance = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.newRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Allowance = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Branch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Deparment = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_EmpNo = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CFL = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_empName = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_empName = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_SalCur = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemButtonEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.date_ValidFrom = new Apzon.Controls.Editor.DateEditEx();
		this.date_ValidTo = new Apzon.Controls.Editor.DateEditEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Allowance).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Allowance).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Deparment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_empName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SalCur).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidFrom.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidTo.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidTo.Properties).BeginInit();
		base.SuspendLayout();
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(102, 406);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 6;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.lbl_ValidFrom.AllowDrop = true;
		this.lbl_ValidFrom.FieldName = null;
		this.lbl_ValidFrom.HideWhenEmpty = false;
		this.lbl_ValidFrom.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ValidFrom.Location = new System.Drawing.Point(4, 13);
		this.lbl_ValidFrom.Name = "lbl_ValidFrom";
		this.lbl_ValidFrom.ShowUnderLine = true;
		this.lbl_ValidFrom.Size = new System.Drawing.Size(117, 20);
		this.lbl_ValidFrom.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ValidFrom.TabIndex = 74;
		this.lbl_ValidFrom.Utext = "Valid From";
		this.lbl_ValidTo.FieldName = null;
		this.lbl_ValidTo.HideWhenEmpty = false;
		this.lbl_ValidTo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ValidTo.Location = new System.Drawing.Point(297, 14);
		this.lbl_ValidTo.Name = "lbl_ValidTo";
		this.lbl_ValidTo.ShowUnderLine = true;
		this.lbl_ValidTo.Size = new System.Drawing.Size(126, 20);
		this.lbl_ValidTo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ValidTo.TabIndex = 89;
		this.lbl_ValidTo.Utext = "Valid To";
		this.grd_Allowance.AllowPasteExcel = true;
		this.grd_Allowance.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Allowance.ClipboardData = null;
		this.grd_Allowance.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Allowance.Location = new System.Drawing.Point(4, 40);
		this.grd_Allowance.MainView = this.gv_Allowance;
		this.grd_Allowance.Name = "grd_Allowance";
		this.grd_Allowance.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.rep_Deparment, this.rep_SalCur, this.rep_CFL, this.rep_empName, this.rep_Status, this.repositoryItemButtonEdit1, this.rep_Branch });
		this.grd_Allowance.Size = new System.Drawing.Size(784, 360);
		this.grd_Allowance.TabIndex = 4;
		this.grd_Allowance.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Allowance });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.newRowToolStripMenuItem, this.duplicateRowToolStripMenuItem, this.removeRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(151, 70);
		this.newRowToolStripMenuItem.Name = "newRowToolStripMenuItem";
		this.newRowToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
		this.newRowToolStripMenuItem.Text = "New Row";
		this.newRowToolStripMenuItem.Click += new System.EventHandler(this.newRowToolStripMenuItem_Click);
		this.duplicateRowToolStripMenuItem.Name = "duplicateRowToolStripMenuItem";
		this.duplicateRowToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
		this.duplicateRowToolStripMenuItem.Text = "Duplicate Row";
		this.duplicateRowToolStripMenuItem.Click += new System.EventHandler(this.duplicateRowToolStripMenuItem_Click);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gv_Allowance.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Allowance.ColumnKey = "empID";
		this.gv_Allowance.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gridColumnEx4, this.gridColumnEx3, this.gridColumn1, this.gc_EmpNo, this.gridColumnEx5, this.gc_empName, this.gridColumn4, this.gridColumnEx1, this.gridColumnEx2 });
		this.gv_Allowance.DisableFormSetting = false;
		this.gv_Allowance.DisplayException = false;
		this.gv_Allowance.EditColumn = null;
		this.gv_Allowance.FieldCurrency = null;
		this.gv_Allowance.FieldRate = null;
		this.gv_Allowance.FormId = "oSalAllowance";
		this.gv_Allowance.GridControl = this.grd_Allowance;
		this.gv_Allowance.GroupCount = 1;
		this.gv_Allowance.IgnoreFilter = false;
		this.gv_Allowance.IndexEditRow = -1;
		this.gv_Allowance.IndicatorWidth = 30;
		this.gv_Allowance.IsApzTable = false;
		this.gv_Allowance.IsInsertNewRow = true;
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
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumn1, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gv_Allowance.UseBatchMod = false;
		this.gv_Allowance.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_MainInfo_CustomDrawRowIndicator);
		this.gv_Allowance.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Allowance_RowCellStyle);
		this.gv_Allowance.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_Allowance_CustomRowCellEdit);
		this.gv_Allowance.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Allowance_ShowingEditor);
		this.gv_Allowance.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_MainInfo_CellValueChanged);
		this.gv_Allowance.DoubleClick += new System.EventHandler(this.gv_MainInfo_DoubleClick);
		this.gridColumnEx4.Caption = "Branch";
		this.gridColumnEx4.ColumnEdit = this.rep_Branch;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "branch";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.OptionsColumn.ReadOnly = true;
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.rep_Branch.AutoHeight = false;
		this.rep_Branch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Branch.Name = "rep_Branch";
		this.rep_Branch.NullText = "";
		this.gridColumnEx3.Caption = "Status";
		this.gridColumnEx3.ColumnEdit = this.rep_Status;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "status";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsColumn.ReadOnly = true;
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.rep_Status.AutoHeight = false;
		this.rep_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Status.Name = "rep_Status";
		this.rep_Status.NullText = "";
		this.gridColumn1.Caption = "Department";
		this.gridColumn1.ColumnEdit = this.rep_Deparment;
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "dept";
		this.gridColumn1.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
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
		this.gc_EmpNo.Caption = "Employee No";
		this.gc_EmpNo.ColumnEdit = this.rep_CFL;
		this.gc_EmpNo.CurrencyDateField = null;
		this.gc_EmpNo.CurrencyField = null;
		this.gc_EmpNo.CurrencyFixed = null;
		this.gc_EmpNo.CurrencyFixedRate = 0.0;
		this.gc_EmpNo.CurrencyRateField = null;
		this.gc_EmpNo.FieldName = "empID";
		this.gc_EmpNo.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gc_EmpNo.Name = "gc_EmpNo";
		this.gc_EmpNo.OptionsColumn.ReadOnly = true;
		this.gc_EmpNo.PrimaryKey = true;
		this.gc_EmpNo.UnitField = null;
		this.gc_EmpNo.Visible = true;
		this.gc_EmpNo.VisibleIndex = 0;
		this.gc_EmpNo.Width = 93;
		this.rep_CFL.AutoHeight = false;
		this.rep_CFL.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_CFL.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_CFL.ContainerControl = this;
		this.rep_CFL.DisplayMember = "";
		this.rep_CFL.DisplayMember2 = "";
		this.rep_CFL.LoadDataSourceWhenEnter = false;
		this.rep_CFL.Name = "rep_CFL";
		this.rep_CFL.ReadOnly = true;
		this.rep_CFL.SeparateCharacterForMulti = ';';
		this.rep_CFL.SeparateString = ", ";
		this.rep_CFL.UsingAutoCompleteBox = false;
		this.rep_CFL.UsingDataSource = true;
		this.rep_CFL.ValidateWhenNoDataSource = false;
		this.rep_CFL.ValueMember = "";
		this.rep_CFL.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_CFL_ButtonClick);
		this.gridColumnEx5.Caption = "Extened Employee No";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "ExtEmpNo";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.OptionsColumn.ReadOnly = true;
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 4;
		this.gc_empName.Caption = "Employee Name";
		this.gc_empName.ColumnEdit = this.rep_empName;
		this.gc_empName.CurrencyDateField = null;
		this.gc_empName.CurrencyField = null;
		this.gc_empName.CurrencyFixed = null;
		this.gc_empName.CurrencyFixedRate = 0.0;
		this.gc_empName.CurrencyRateField = null;
		this.gc_empName.FieldName = "empName";
		this.gc_empName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gc_empName.Name = "gc_empName";
		this.gc_empName.OptionsColumn.ReadOnly = true;
		this.gc_empName.PrimaryKey = false;
		this.gc_empName.UnitField = null;
		this.gc_empName.Visible = true;
		this.gc_empName.VisibleIndex = 1;
		this.gc_empName.Width = 182;
		this.rep_empName.AutoHeight = false;
		this.rep_empName.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.rep_empName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_empName.ContainerControl = this;
		this.rep_empName.DisplayMember = "";
		this.rep_empName.DisplayMember2 = "";
		this.rep_empName.LoadDataSourceWhenEnter = false;
		this.rep_empName.Name = "rep_empName";
		this.rep_empName.ReadOnly = true;
		this.rep_empName.SeparateCharacterForMulti = ';';
		this.rep_empName.SeparateString = ", ";
		this.rep_empName.UsingAutoCompleteBox = false;
		this.rep_empName.UsingDataSource = true;
		this.rep_empName.ValidateWhenNoDataSource = false;
		this.rep_empName.ValueMember = "";
		this.gridColumn4.Caption = "Currency";
		this.gridColumn4.ColumnEdit = this.rep_SalCur;
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "Currency";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 5;
		this.rep_SalCur.AutoHeight = false;
		this.rep_SalCur.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SalCur.Name = "rep_SalCur";
		this.rep_SalCur.NullText = "";
		this.gridColumnEx1.Caption = "Valid From";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx1.FieldName = "ValidFrom";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 2;
		this.gridColumnEx1.Width = 105;
		this.gridColumnEx2.Caption = "Valid To";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx2.FieldName = "ValidTo";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 3;
		this.gridColumnEx2.Width = 92;
		this.repositoryItemButtonEdit1.AutoHeight = false;
		this.repositoryItemButtonEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.repositoryItemButtonEdit1.Name = "repositoryItemButtonEdit1";
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(4, 406);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.Size = new System.Drawing.Size(93, 20);
		this.btn_Save.TabIndex = 5;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btnSearch.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnSearch.Appearance.Options.UseBackColor = true;
		this.btnSearch.FieldName = null;
		this.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		this.btnSearch.IsSearchControl = true;
		this.btnSearch.Location = new System.Drawing.Point(628, 10);
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.Size = new System.Drawing.Size(99, 20);
		this.btnSearch.TabIndex = 3;
		this.btnSearch.Text = "Search";
		this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
		this.date_ValidFrom.EditValue = null;
		this.date_ValidFrom.FieldName = null;
		this.date_ValidFrom.Location = new System.Drawing.Point(116, 10);
		this.date_ValidFrom.Name = "date_ValidFrom";
		this.date_ValidFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ValidFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ValidFrom.Properties.IsSearchControl = true;
		this.date_ValidFrom.Size = new System.Drawing.Size(170, 20);
		this.date_ValidFrom.TabIndex = 1;
		this.date_ValidFrom.EditValueChanged += new System.EventHandler(this.date_ValidFrom_EditValueChanged);
		this.date_ValidTo.EditValue = null;
		this.date_ValidTo.FieldName = null;
		this.date_ValidTo.Location = new System.Drawing.Point(419, 11);
		this.date_ValidTo.Name = "date_ValidTo";
		this.date_ValidTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ValidTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ValidTo.Properties.IsSearchControl = true;
		this.date_ValidTo.Size = new System.Drawing.Size(170, 20);
		this.date_ValidTo.TabIndex = 2;
		this.date_ValidTo.EditValueChanged += new System.EventHandler(this.date_ValidTo_EditValueChanged);
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(794, 434);
		base.Controls.Add(this.date_ValidTo);
		base.Controls.Add(this.date_ValidFrom);
		base.Controls.Add(this.btnSearch);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.grd_Allowance);
		base.Controls.Add(this.lbl_ValidTo);
		base.Controls.Add(this.lbl_ValidFrom);
		base.Controls.Add(this.btnCancel);
		base.FormId = "oSalAllowance";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SalAllowance";
		base.ShowInTaskbar = false;
		this.Text = "Employee Salary Allowance - Edit";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_SalaryInfo_FormClosing);
		base.Load += new System.EventHandler(this.frm_SalaryInfo_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Allowance).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Allowance).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Deparment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_empName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SalCur).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidFrom.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidTo.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidTo.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
