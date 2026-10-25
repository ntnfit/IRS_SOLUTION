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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
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

public class frm_SalAllowanceByEmp : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public ReloadMain ReloadMain;

	public int EmpId;

	private int _maxLineNumInDb;

	private bool _isCancel;

	private bool _forceClose;

	private readonly List<Common.Table_Code> _lstAllowanceType = new List<Common.Table_Code>();

	private IContainer components;

	public SimpleButtonEx btnCancel;

	public SimpleButtonEx btn_Save;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private GridControlEx grd_Allowance;

	private GridViewEx gv_Allowance;

	private GridColumnEx gridColumn7;

	private RepositoryItemLookUpEdit rep_DeptAllowance;

	private GridColumnEx gc_EmpNo;

	private GridColumnEx gridColumn9;

	private GridColumnEx gridColumn1;

	private RepositoryItemDateEdit rep_dateValidFrom;

	private GridColumnEx gridColumn2;

	private RepositoryItemDateEdit rep_dateValidTo;

	private GridColumnEx gridColumn3;

	private RepositoryItemLookUpEdit rep_SalCur;

	private ToolStripMenuItem duplicateRowToolStripMenuItem;

	private RepositoryItemButtonEdit rep_CFL;

	private GridColumnEx gridColumnEx2;

	private RepositoryItemLookUpEdit rep_Branch;

	private GridColumnEx gridColumnEx1;

	private RepositoryItemLookUpEdit rep_Status;

	private GridColumnEx gridColumnEx3;

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

	public frm_SalAllowanceByEmp()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SalaryInfo"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SalaryInfo" && x.authoriza == "F"))
			{
				Controls.EnableDisableControl(isEnable: false);
			}
			FormId = "oSalAllowanceByEmp";
			gv_Allowance.ItemId = "APZ_HSI1";
			gv_Allowance.FormId = FormId;
			gv_Allowance.ChangeLogType = Enums.ChangeLog.clSalAllowance;
			gv_Allowance.IsApzTable = true;
			OnLoadAsync += frm_SalaryInfo_Load;
			return;
		}
		throw new Exception(Apzon.Commons.Properties.Resources.Msg_FormAccessDenied);
	}

	private async Task frm_SalaryInfo_Load(object sender, EventArgs e)
	{
		try
		{
			LoadInitForm();
			await LoadData();
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

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				btn_Save.Enabled = false;
				btnCancel.Enabled = false;
				await SaveData();
				btn_Save.Enabled = true;
				btnCancel.Enabled = true;
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

	private void gv_Allowance_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
			DataTable dataSourceTable = grd_Allowance.DataSourceTable;
			string value = Function.ToString(gv_Allowance.GetRowCellValue(e.RowHandle, "ValidFrom"));
			string value2 = Function.ToString(gv_Allowance.GetRowCellValue(e.RowHandle, "ValidTo"));
			int num = Function.ParseInt(gv_Allowance.GetRowCellValue(e.RowHandle, "LineNum"));
			int num2 = dataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"]));
			if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value2) && num == num2)
			{
				AddNewRow();
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
			DataTable dt = grd_Allowance.DataSourceTable;
			int num = Function.ParseInt(gv_Allowance.GetFocusedRowCellValue("LineNum"));
			int num2 = dt.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"]));
			if (num == num2 || gv_Allowance.FocusedRowHandle < 0)
			{
				return;
			}
			if (num > _maxLineNumInDb)
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
				string text = await _httpClient.SalaryAllowanceByEmpClient.DeleteDoc(Function.ParseInt(num), EmpId);
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

	private void gv_Allowance_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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

	private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			e.Cancel = gv_Allowance.FocusedRowHandle < 0;
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
			int num = dataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"]));
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow["empID"] = EmpId;
			dataRow["empName"] = dataSourceTable.Rows[0]["empName"];
			dataRow["dept"] = dataSourceTable.Rows[0]["dept"];
			dataRow["LineNum"] = num;
			dataRow["Currency"] = dataSourceTable.Rows[0]["Currency"];
			dataRow["ExtEmpNo"] = dataSourceTable.Rows[0]["ExtEmpNo"];
			foreach (DataColumn column in dataSourceTable.Columns)
			{
				if (column.ColumnName.Contains("EmpAllowance"))
				{
					dataRow[column.ColumnName] = dataSourceTable.Rows[0][column.ColumnName];
				}
			}
			dataSourceTable.Rows.InsertAt(dataRow, dataSourceTable.Rows.Count - 1);
			dataSourceTable.Rows[dataSourceTable.Rows.Count - 1]["LineNum"] = num + 1;
			dataSourceTable.AcceptChanges();
			FormMode = Enums.FormViewModeType.Update;
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

	private void gv_Allowance_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.ReadOnly)
			{
				e.Appearance.BackColor = Color.LightGray;
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
		try
		{
			RepositoryItemCalcEdit calc = new RepositoryItemCalcEdit();
			DataTable dataTable = await _httpClient.SalaryAllowanceByEmpClient.GetDetail(EmpId);
			if (dataTable != null)
			{
				foreach (DataColumn col in dataTable.Columns)
				{
					if (col.ColumnName != "empID" && col.ColumnName != "empName" && col.ColumnName != "ExtEmpNo" && col.ColumnName != "LineNum" && col.ColumnName != "ValidFrom" && col.ColumnName != "ValidTo" && col.ColumnName != "dept" && gv_Allowance.Columns.AsEnumerable().FirstOrDefault((GridColumn t) => t.FieldName == col.ColumnName) == null)
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
						gridColumnEx.ColumnEdit = calc;
						gv_Allowance.BeginUpdate();
						gv_Allowance.Columns.Insert(gv_Allowance.Columns.Count, gridColumnEx);
						gv_Allowance.EndUpdate();
					}
				}
				grd_Allowance.DataSource = dataTable;
				grd_Allowance.RefreshDataSource();
				if (dataTable.IsNotNull())
				{
					_maxLineNumInDb = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"]));
					if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["ValidFrom"])) || !string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["ValidTo"])))
					{
						AddNewRow();
					}
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			gv_Allowance.ExpandAllGroups();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadInitForm()
	{
		_ = 2;
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
			rep_DeptAllowance.DataSource = dataTable3.Copy();
			rep_DeptAllowance.DisplayMember = "Name";
			rep_DeptAllowance.ValueMember = "Code";
			rep_DeptAllowance.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_DeptAllowance.ForceInitialize();
			rep_DeptAllowance.PopulateColumns();
			rep_DeptAllowance.Columns["Code"].Visible = false;
			rep_DeptAllowance.Columns["Remarks"].Visible = false;
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
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewRow()
	{
		try
		{
			gv_Allowance.BeginInsertRow();
			DataTable dataTable = grd_Allowance.DataSourceTable;
			if (dataTable.IsNotNull())
			{
				int num = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"]));
				DataRow dataRow = dataTable.NewRow();
				dataRow["empID"] = EmpId;
				dataRow["empName"] = dataTable.Rows[0]["empName"];
				dataRow["dept"] = dataTable.Rows[0]["dept"];
				dataRow["ExtEmpNo"] = dataTable.Rows[0]["ExtEmpNo"];
				dataRow["LineNum"] = num + 1;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			else
			{
				dataTable = WinFuntions.GetTableStructure("APZ_HSI1");
				if (!dataTable.Columns.Contains("empName"))
				{
					dataTable.Columns.Add("empName", typeof(string));
				}
				if (!dataTable.Columns.Contains("dept"))
				{
					dataTable.Columns.Add("dept", typeof(int));
				}
				if (!dataTable.Columns.Contains("ExtEmpNo"))
				{
					dataTable.Columns.Add("ExtEmpNo", typeof(string));
				}
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["empID"] = EmpId;
				dataRow2["LineNum"] = DBNull.Value;
				dataTable.Rows.Add(dataRow2);
				dataTable.AcceptChanges();
			}
			grd_Allowance.DataSource = dataTable;
			grd_Allowance.RefreshDataSource();
			gv_Allowance.EndInsertRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		gv_Allowance.EndInsertRow();
	}

	private async Task<string> SaveData()
	{
		try
		{
			int num = 0;
			DataTable dataTable = grd_Allowance.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataTable dataTable2 = dataTable.Copy();
				if (string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[dataTable2.Rows.Count - 1]["ValidFrom"])))
				{
					dataTable2.Rows.RemoveAt(dataTable2.Rows.Count - 1);
					dataTable2.AcceptChanges();
				}
				DataRow dataRow = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["ValidFrom"])));
				if (dataRow != null)
				{
					WinCommons.Error(Enums.MessageCode.DateCannotBeEmptyWithEmpNo, "", Function.ToString(dataRow["EmpId"]));
					return "Error";
				}
				DataRow dataRow2 = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseDateTimes(t["ValidFrom"]) > Function.ParseDateTimes(t["ValidTo"]) && t["ValidTo"] != DBNull.Value);
				if (dataRow2 != null)
				{
					WinCommons.Error(Enums.MessageCode.ValidFromCannotbeLargeThanValidToDateWithEmpId, "", Function.ToString(dataRow2["EmpId"]));
					return "Error";
				}
				DataTable tableStructure = WinFuntions.GetTableStructure("APZ_HSI1");
				if (dataTable2.IsNotNull())
				{
					foreach (DataRow row in dataTable2.Rows)
					{
						foreach (DataColumn column in dataTable2.Columns)
						{
							if (!(column.ColumnName == "empID") && !(column.ColumnName == "empName") && !(column.ColumnName == "LineNum") && !(column.ColumnName == "ValidFrom") && !(column.ColumnName == "ValidTo") && !(column.ColumnName == "dept") && !(column.ColumnName == "Code") && !(column.ColumnName == "Currency") && !(column.ColumnName == "ExtEmpNo") && row[column.ColumnName] != DBNull.Value)
							{
								num++;
							}
						}
						if (num <= 0)
						{
							continue;
						}
						foreach (DataColumn column2 in dataTable2.Columns)
						{
							if (column2.ColumnName.Contains("_"))
							{
								string[] array = column2.ColumnName.Split('_');
								if (array.Length >= 3)
								{
									DataRow dataRow4 = tableStructure.NewRow();
									dataRow4["empID"] = Function.ParseInt(row["empID"]);
									dataRow4["LineNum"] = row["LineNum"];
									dataRow4["Currency"] = row["Currency"];
									dataRow4["ValidFrom"] = Function.ParseDateTimes(row["ValidFrom"]).Date;
									dataRow4["ValidTo"] = ((row["ValidTo"] == DBNull.Value) ? DBNull.Value : ((object)Function.ParseDateTimes(row["ValidTo"]).Date));
									dataRow4["Amount"] = Function.ParseDouble(row[column2.ColumnName]);
									dataRow4["AllowanceType"] = Function.ParseInt(array[1]);
									dataRow4["UserSign2"] = GlobalData.UserID;
									dataRow4["UpdateDate"] = DateTime.Today;
									tableStructure.Rows.Add(dataRow4);
								}
							}
						}
					}
					tableStructure.AcceptChanges();
				}
				WinCommons._OpenProcessing();
				if (tableStructure.IsNotNull())
				{
					string text = await _httpClient.SalaryAllowanceByEmpClient.SaveDoc(tableStructure);
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
				LoadData();
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.Salary.frm_SalAllowanceByEmp));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.duplicateRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Allowance = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Allowance = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Branch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_DeptAllowance = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_EmpNo = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CFL = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_dateValidFrom = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_dateValidTo = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_SalCur = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Allowance).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Allowance).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DeptAllowance).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateValidFrom).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateValidFrom.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateValidTo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateValidTo.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SalCur).BeginInit();
		base.SuspendLayout();
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(101, 406);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 57;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.duplicateRowToolStripMenuItem, this.removeRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(151, 48);
		this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
		this.duplicateRowToolStripMenuItem.Name = "duplicateRowToolStripMenuItem";
		this.duplicateRowToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
		this.duplicateRowToolStripMenuItem.Text = "Duplicate Row";
		this.duplicateRowToolStripMenuItem.Click += new System.EventHandler(this.duplicateRowToolStripMenuItem_Click);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(4, 406);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.Size = new System.Drawing.Size(93, 20);
		this.btn_Save.TabIndex = 93;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.grd_Allowance.AllowPasteExcel = true;
		this.grd_Allowance.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Allowance.ClipboardData = null;
		this.grd_Allowance.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Allowance.Location = new System.Drawing.Point(4, 5);
		this.grd_Allowance.MainView = this.gv_Allowance;
		this.grd_Allowance.Name = "grd_Allowance";
		this.grd_Allowance.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.rep_DeptAllowance, this.rep_dateValidFrom, this.rep_dateValidTo, this.rep_SalCur, this.rep_CFL, this.rep_Branch, this.rep_Status });
		this.grd_Allowance.Size = new System.Drawing.Size(623, 395);
		this.grd_Allowance.TabIndex = 94;
		this.grd_Allowance.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Allowance });
		this.gv_Allowance.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Allowance.ColumnKey = "empID";
		this.gv_Allowance.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gridColumnEx2, this.gridColumnEx1, this.gridColumn7, this.gc_EmpNo, this.gridColumnEx3, this.gridColumn9, this.gridColumn1, this.gridColumn2, this.gridColumn3 });
		this.gv_Allowance.DisableFormSetting = false;
		this.gv_Allowance.DisplayException = false;
		this.gv_Allowance.EditColumn = null;
		this.gv_Allowance.FieldCurrency = null;
		this.gv_Allowance.FieldRate = null;
		this.gv_Allowance.FormId = "oSalAllowanceByEmp";
		this.gv_Allowance.GridControl = this.grd_Allowance;
		this.gv_Allowance.GroupCount = 1;
		this.gv_Allowance.IndexEditRow = -1;
		this.gv_Allowance.IndicatorWidth = 40;
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
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumn7, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gv_Allowance.UseBatchMod = false;
		this.gv_Allowance.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Allowance_CustomDrawRowIndicator);
		this.gv_Allowance.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Allowance_RowCellStyle);
		this.gv_Allowance.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Allowance_CellValueChanged);
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
		this.gridColumn7.VisibleIndex = 0;
		this.rep_DeptAllowance.AutoHeight = false;
		this.rep_DeptAllowance.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_DeptAllowance.Name = "rep_DeptAllowance";
		this.rep_DeptAllowance.NullText = "";
		this.gc_EmpNo.Caption = "Employee No.";
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
		this.gc_EmpNo.Width = 89;
		this.rep_CFL.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_CFL.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_CFL.Name = "rep_CFL";
		this.rep_CFL.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_CFL_ButtonClick);
		this.gridColumnEx3.Caption = "Extened Employee No";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "ExtEmpNo";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsColumn.ReadOnly = true;
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 4;
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
		this.gridColumn9.Width = 166;
		this.gridColumn1.Caption = "Valid From";
		this.gridColumn1.ColumnEdit = this.rep_dateValidFrom;
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumn1.FieldName = "ValidFrom";
		this.gridColumn1.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 2;
		this.gridColumn1.Width = 123;
		this.rep_dateValidFrom.AutoHeight = false;
		this.rep_dateValidFrom.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_dateValidFrom.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_dateValidFrom.Name = "rep_dateValidFrom";
		this.gridColumn2.Caption = "Valid To";
		this.gridColumn2.ColumnEdit = this.rep_dateValidTo;
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumn2.FieldName = "ValidTo";
		this.gridColumn2.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 3;
		this.gridColumn2.Width = 128;
		this.rep_dateValidTo.AutoHeight = false;
		this.rep_dateValidTo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_dateValidTo.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_dateValidTo.Name = "rep_dateValidTo";
		this.gridColumn3.Caption = "Currency";
		this.gridColumn3.ColumnEdit = this.rep_SalCur;
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "Currency";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 5;
		this.rep_SalCur.AutoHeight = false;
		this.rep_SalCur.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SalCur.Name = "rep_SalCur";
		this.rep_SalCur.NullText = "";
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(628, 434);
		base.Controls.Add(this.grd_Allowance);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.FormId = "oSalAllowanceByEmp";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SalAllowanceByEmp";
		base.ShowInTaskbar = false;
		this.Text = "Employee Salary Allowance";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_SalaryInfo_FormClosing);
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Allowance).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Allowance).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DeptAllowance).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateValidFrom.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateValidFrom).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateValidTo.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateValidTo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SalCur).EndInit();
		base.ResumeLayout(false);
	}
}
