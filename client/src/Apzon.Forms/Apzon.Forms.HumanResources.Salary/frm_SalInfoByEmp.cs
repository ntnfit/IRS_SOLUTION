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

public class frm_SalInfoByEmp : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public ReloadMain ReloadMain;

	public int EmpId;

	private int _maxLineNumInDb;

	private bool _isChanging;

	private bool _isCancel;

	private bool _forceClose;

	private IContainer components;

	public SimpleButtonEx btnCancel;

	private GridControlEx grd_MainInfo;

	private GridViewEx gv_MainInfo;

	private GridColumnEx gridColumn1;

	private GridColumnEx gc_EmpNo;

	private GridColumnEx gridColumn3;

	private GridColumnEx gc_BasicSal;

	private GridColumnEx gc_SalesSal;

	private GridColumnEx gc_SalTotal;

	public SimpleButtonEx btn_Save;

	private GridColumnEx gridColumn8;

	private GridColumnEx gridColumn7;

	private RepositoryItemLookUpEdit rep_Deparment;

	private RepositoryItemDateEdit rep_dateValidFrom;

	private RepositoryItemDateEdit rep_dateValidTo;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private GridColumnEx gridColumn4;

	private RepositoryItemLookUpEdit rep_SalCur;

	private ToolStripMenuItem duplicateRowToolStripMenuItem;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private RepositoryItemButtonEdit rep_CFL;

	private GridColumnEx gridColumnEx1;

	private RepositoryItemLookUpEdit rep_Branch;

	private GridColumnEx gridColumnEx2;

	private RepositoryItemLookUpEdit rep_Status;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

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
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_SalInfoByEmp()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SalaryInfo"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SalaryInfo" && x.authoriza == "F"))
			{
				Controls.EnableDisableControl(isEnable: false);
			}
			FormId = "oSalInfoByEmp";
			gv_MainInfo.ItemId = "APZ_OHSI";
			gv_MainInfo.FormId = FormId;
			gv_MainInfo.ChangeLogType = Enums.ChangeLog.clSalaryInfo;
			gv_MainInfo.IsApzTable = true;
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

	private void gv_MainInfo_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
			DataTable dataSourceTable = grd_MainInfo.DataSourceTable;
			if (!_isChanging)
			{
				double num = Function.ParseDouble(gv_MainInfo.GetRowCellValue(e.RowHandle, "BasicSal"));
				if (e.Column.FieldName == "BasicSal" || e.Column.FieldName == "SalesSal")
				{
					double num2 = Function.ParseDouble(gv_MainInfo.GetRowCellValue(e.RowHandle, "SalesSal"));
					_isChanging = true;
					gv_MainInfo.BeginUpdate();
					gv_MainInfo.SetRowCellValue(e.RowHandle, "SalTotal", num + num2);
					gv_MainInfo.EndUpdate();
					_isChanging = false;
				}
				else if (e.Column.FieldName == "SalTotal")
				{
					double num3 = Function.ParseDouble(gv_MainInfo.GetRowCellValue(e.RowHandle, "SalTotal"));
					_isChanging = true;
					gv_MainInfo.BeginUpdate();
					gv_MainInfo.SetRowCellValue(e.RowHandle, "SalesSal", num3 - num);
					gv_MainInfo.EndUpdate();
					_isChanging = false;
				}
				FormMode = Enums.FormViewModeType.Update;
				string value = Function.ToString(gv_MainInfo.GetRowCellValue(e.RowHandle, "ValidFrom"));
				string value2 = Function.ToString(gv_MainInfo.GetRowCellValue(e.RowHandle, "ValidTo"));
				int num4 = Function.ParseInt(gv_MainInfo.GetRowCellValue(e.RowHandle, "LineNum"));
				int num5 = dataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"]));
				if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value2) && num4 == num5)
				{
					AddNewRow();
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
			DataTable dt = grd_MainInfo.DataSourceTable;
			int num = Function.ParseInt(gv_MainInfo.GetFocusedRowCellValue("LineNum"));
			int num2 = dt.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"]));
			if (num == num2 || gv_MainInfo.FocusedRowHandle < 0)
			{
				return;
			}
			if (num > _maxLineNumInDb)
			{
				gv_MainInfo.BeginUpdate();
				gv_MainInfo.DeleteSelectedRows();
				gv_MainInfo.EndUpdate();
				dt.AcceptChanges();
				grd_MainInfo.RefreshDataSource();
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				string text = await _httpClient.SalaryInfoMainByEmpClient.DeleteDoc(Function.ParseInt(num), EmpId);
				if (string.IsNullOrEmpty(text))
				{
					gv_MainInfo.BeginUpdate();
					gv_MainInfo.DeleteSelectedRows();
					gv_MainInfo.EndUpdate();
					dt.AcceptChanges();
					grd_MainInfo.RefreshDataSource();
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

	private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			e.Cancel = gv_MainInfo.FocusedRowHandle < 0;
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
			_isChanging = true;
			DataTable dataSourceTable = grd_MainInfo.DataSourceTable;
			if (gv_MainInfo.FocusedRowHandle >= 0 && (!string.IsNullOrEmpty(Function.ToString(gv_MainInfo.GetFocusedRowCellValue("ValidFrom"))) || !string.IsNullOrEmpty(Function.ToString(gv_MainInfo.GetFocusedRowCellValue("ValidTo")))) && dataSourceTable != null)
			{
				int num = dataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"]));
				DataRow dataRow = dataSourceTable.NewRow();
				dataRow["empID"] = EmpId;
				dataRow["empName"] = dataSourceTable.Rows[0]["empName"];
				dataRow["dept"] = dataSourceTable.Rows[0]["dept"];
				dataRow["LineNum"] = num;
				dataRow["SalCur"] = dataSourceTable.Rows[0]["SalCur"];
				dataRow["BasicSal"] = gv_MainInfo.GetFocusedRowCellValue("BasicSal");
				dataRow["SalesSal"] = gv_MainInfo.GetFocusedRowCellValue("SalesSal");
				dataRow["SalTotal"] = gv_MainInfo.GetFocusedRowCellValue("SalTotal");
				dataRow["LineStatus"] = gv_MainInfo.GetFocusedRowCellValue("LineStatus");
				dataRow["ExtEmpNo"] = dataSourceTable.Rows[0]["ExtEmpNo"];
				dataSourceTable.Rows.InsertAt(dataRow, dataSourceTable.Rows.Count - 1);
				dataSourceTable.Rows[dataSourceTable.Rows.Count - 1]["LineNum"] = num + 1;
				dataSourceTable.AcceptChanges();
				FormMode = Enums.FormViewModeType.Update;
			}
			_isChanging = false;
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

	private void gv_MainInfo_RowCellStyle(object sender, RowCellStyleEventArgs e)
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
			DataTable dataTable = await _httpClient.SalaryInfoMainByEmpClient.GetDetail(EmpId);
			if (dataTable.IsNotNull())
			{
				grd_MainInfo.DataSource = dataTable;
				grd_MainInfo.RefreshDataSource();
				_maxLineNumInDb = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"]));
				if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["ValidFrom"])) || !string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["ValidTo"])))
				{
					AddNewRow();
				}
			}
			else
			{
				AddNewRow();
			}
			gv_MainInfo.ExpandAllGroups();
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
			gc_SalTotal.OptionsColumn.AllowEdit = false;
			gc_SalTotal.OptionsColumn.ReadOnly = true;
			await WinFuntions.LoadUdfColumns(gv_MainInfo, "APZ_OHSI");
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
			gv_MainInfo.BeginInsertRow();
			DataTable dataTable = grd_MainInfo.DataSourceTable;
			if (dataTable.IsNotNull())
			{
				int num = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"]));
				DataRow dataRow = dataTable.NewRow();
				dataRow["empID"] = EmpId;
				dataRow["empName"] = dataTable.Rows[0]["empName"];
				dataRow["dept"] = dataTable.Rows[0]["dept"];
				dataRow["ExtEmpNo"] = dataTable.Rows[0]["ExtEmpNo"];
				dataRow["LineStatus"] = dataTable.Rows[0]["LineStatus"];
				dataRow["LineNum"] = num + 1;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			else
			{
				dataTable = WinFuntions.GetTableStructure("APZ_OHSI");
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
			grd_MainInfo.DataSource = dataTable;
			grd_MainInfo.RefreshDataSource();
			gv_MainInfo.EndInsertRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		gv_MainInfo.EndInsertRow();
	}

	private async Task<string> SaveData()
	{
		try
		{
			DataTable dataTable = grd_MainInfo.DataSource as DataTable;
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OHSI");
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
					WinCommons.Error(Enums.MessageCode.FromDateCannotBeEmptyWithEmpId, "", Function.ToString(dataRow["EmpId"]));
					return "Error";
				}
				DataRow dataRow2 = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseDateTimes(t["ValidFrom"]) > Function.ParseDateTimes(t["ValidTo"]) && t["ValidTo"] != DBNull.Value);
				if (dataRow2 != null)
				{
					WinCommons.Error(Enums.MessageCode.ValidFromCannotbeLargeThanValidToDateWithEmpId, "", Function.ToString(dataRow2["EmpId"]));
					return "Error";
				}
				foreach (DataRow row in dataTable2.Rows)
				{
					if (row["SalTotal"] == DBNull.Value || (row["BasicSal"] == DBNull.Value && row["SalesSal"] == DBNull.Value))
					{
						continue;
					}
					DataRow dataRow4 = tableStructure.NewRow();
					dataRow4["empID"] = Function.ParseInt(row["empID"]);
					dataRow4["LineNum"] = row["LineNum"];
					dataRow4["SalCur"] = row["SalCur"];
					dataRow4["validFrom"] = Function.ParseDateTimes(row["validFrom"]).Date;
					dataRow4["ValidTo"] = ((row["ValidTo"] == DBNull.Value) ? DBNull.Value : ((object)Function.ParseDateTimes(row["ValidTo"]).Date));
					dataRow4["BasicSal"] = Function.ParseDouble(row["BasicSal"]);
					dataRow4["SalesSal"] = Function.ParseDouble(row["SalesSal"]);
					dataRow4["SalTotal"] = Function.ParseDouble(row["SalTotal"]);
					dataRow4["LineStatus"] = Function.ParseInt(row["LineStatus"]);
					dataRow4["UserSign2"] = GlobalData.UserID;
					dataRow4["UpdateDate"] = DateTime.Today;
					foreach (DataColumn column in dataTable2.Columns)
					{
						if (column.ColumnName.ToUpper().StartsWith("U_") && tableStructure.Columns.Contains(column.ColumnName))
						{
							dataRow4[column.ColumnName] = row[column.ColumnName];
						}
					}
					tableStructure.Rows.Add(dataRow4);
					tableStructure.AcceptChanges();
				}
				WinCommons._OpenProcessing();
				if (tableStructure.IsNotNull())
				{
					string text = await _httpClient.SalaryInfoMainByEmpClient.SaveDoc(tableStructure);
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
				await LoadData();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.Salary.frm_SalInfoByEmp));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.rep_Deparment = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_MainInfo = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.duplicateRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_MainInfo = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Branch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_EmpNo = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CFL = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_dateValidFrom = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_dateValidTo = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gc_BasicSal = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gc_SalesSal = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_SalTotal = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_SalCur = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.rep_Deparment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_MainInfo).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_MainInfo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateValidFrom).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateValidFrom.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateValidTo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateValidTo.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SalCur).BeginInit();
		base.SuspendLayout();
		this.rep_Deparment.AutoHeight = false;
		this.rep_Deparment.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Deparment.Name = "rep_Deparment";
		this.rep_Deparment.NullText = "";
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
		this.grd_MainInfo.AllowPasteExcel = true;
		this.grd_MainInfo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_MainInfo.ClipboardData = null;
		this.grd_MainInfo.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_MainInfo.Location = new System.Drawing.Point(2, 4);
		this.grd_MainInfo.MainView = this.gv_MainInfo;
		this.grd_MainInfo.Name = "grd_MainInfo";
		this.grd_MainInfo.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.rep_dateValidFrom, this.rep_dateValidTo, this.rep_SalCur, this.repositoryItemCalcEdit1, this.rep_CFL, this.rep_Branch, this.rep_Status });
		this.grd_MainInfo.Size = new System.Drawing.Size(623, 396);
		this.grd_MainInfo.TabIndex = 92;
		this.grd_MainInfo.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_MainInfo });
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
		this.gv_MainInfo.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_MainInfo.ColumnKey = "empID";
		this.gv_MainInfo.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[13]
		{
			this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx4, this.gridColumn1, this.gc_EmpNo, this.gridColumnEx3, this.gridColumn3, this.gridColumn8, this.gridColumn7, this.gc_BasicSal,
			this.gc_SalesSal, this.gc_SalTotal, this.gridColumn4
		});
		this.gv_MainInfo.DisableFormSetting = false;
		this.gv_MainInfo.DisplayException = false;
		this.gv_MainInfo.EditColumn = null;
		this.gv_MainInfo.FieldCurrency = null;
		this.gv_MainInfo.FieldRate = null;
		this.gv_MainInfo.FormId = "oSalInfoByEmp";
		this.gv_MainInfo.GridControl = this.grd_MainInfo;
		this.gv_MainInfo.GroupCount = 1;
		this.gv_MainInfo.IndexEditRow = -1;
		this.gv_MainInfo.IndicatorWidth = 30;
		this.gv_MainInfo.IsApzTable = false;
		this.gv_MainInfo.IsInsertNewRow = true;
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
		this.gv_MainInfo.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_MainInfo_RowCellStyle);
		this.gv_MainInfo.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_MainInfo_CellValueChanged);
		this.gridColumnEx1.Caption = "Branch";
		this.gridColumnEx1.ColumnEdit = this.rep_Branch;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "branch";
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
		this.gridColumnEx4.Caption = "Line Status";
		this.gridColumnEx4.ColumnEdit = this.rep_Status;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "LineStatus";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 2;
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
		this.gc_EmpNo.Caption = "Employee No";
		this.gc_EmpNo.ColumnEdit = this.rep_CFL;
		this.gc_EmpNo.CurrencyDateField = null;
		this.gc_EmpNo.CurrencyField = null;
		this.gc_EmpNo.CurrencyFixed = null;
		this.gc_EmpNo.CurrencyFixedRate = 0.0;
		this.gc_EmpNo.CurrencyRateField = null;
		this.gc_EmpNo.FieldName = "empID";
		this.gc_EmpNo.Name = "gc_EmpNo";
		this.gc_EmpNo.OptionsColumn.ReadOnly = true;
		this.gc_EmpNo.PrimaryKey = true;
		this.gc_EmpNo.UnitField = null;
		this.gc_EmpNo.Visible = true;
		this.gc_EmpNo.VisibleIndex = 0;
		this.gc_EmpNo.Width = 83;
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
		this.gridColumnEx3.VisibleIndex = 5;
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
		this.gridColumn3.Width = 134;
		this.gridColumn8.Caption = "Valid From";
		this.gridColumn8.ColumnEdit = this.rep_dateValidFrom;
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumn8.FieldName = "ValidFrom";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 3;
		this.gridColumn8.Width = 127;
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
		this.gridColumn7.Caption = "Valid To";
		this.gridColumn7.ColumnEdit = this.rep_dateValidTo;
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumn7.FieldName = "ValidTo";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 4;
		this.gridColumn7.Width = 119;
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
		this.gc_BasicSal.Caption = "Basic Salary";
		this.gc_BasicSal.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gc_BasicSal.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gc_BasicSal.CurrencyDateField = null;
		this.gc_BasicSal.CurrencyField = "SalCur";
		this.gc_BasicSal.CurrencyFixed = null;
		this.gc_BasicSal.CurrencyFixedRate = 0.0;
		this.gc_BasicSal.CurrencyRateField = null;
		this.gc_BasicSal.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gc_BasicSal.FieldName = "BasicSal";
		this.gc_BasicSal.Name = "gc_BasicSal";
		this.gc_BasicSal.PrimaryKey = false;
		this.gc_BasicSal.UnitField = null;
		this.gc_BasicSal.Visible = true;
		this.gc_BasicSal.VisibleIndex = 7;
		this.gc_BasicSal.Width = 183;
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.gc_SalesSal.Caption = "Sales Salary";
		this.gc_SalesSal.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gc_SalesSal.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gc_SalesSal.CurrencyDateField = null;
		this.gc_SalesSal.CurrencyField = "SalCur";
		this.gc_SalesSal.CurrencyFixed = null;
		this.gc_SalesSal.CurrencyFixedRate = 0.0;
		this.gc_SalesSal.CurrencyRateField = null;
		this.gc_SalesSal.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gc_SalesSal.FieldName = "SalesSal";
		this.gc_SalesSal.Name = "gc_SalesSal";
		this.gc_SalesSal.PrimaryKey = false;
		this.gc_SalesSal.UnitField = null;
		this.gc_SalesSal.Visible = true;
		this.gc_SalesSal.VisibleIndex = 8;
		this.gc_SalesSal.Width = 160;
		this.gc_SalTotal.Caption = "Total";
		this.gc_SalTotal.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gc_SalTotal.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gc_SalTotal.CurrencyDateField = null;
		this.gc_SalTotal.CurrencyField = "SalCur";
		this.gc_SalTotal.CurrencyFixed = null;
		this.gc_SalTotal.CurrencyFixedRate = 0.0;
		this.gc_SalTotal.CurrencyRateField = null;
		this.gc_SalTotal.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gc_SalTotal.FieldName = "SalTotal";
		this.gc_SalTotal.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
		this.gc_SalTotal.Name = "gc_SalTotal";
		this.gc_SalTotal.PrimaryKey = false;
		this.gc_SalTotal.UnitField = null;
		this.gc_SalTotal.Visible = true;
		this.gc_SalTotal.VisibleIndex = 9;
		this.gc_SalTotal.Width = 230;
		this.gridColumn4.Caption = "Currency";
		this.gridColumn4.ColumnEdit = this.rep_SalCur;
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "SalCur";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 6;
		this.gridColumn4.Width = 59;
		this.rep_SalCur.AutoHeight = false;
		this.rep_SalCur.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SalCur.Name = "rep_SalCur";
		this.rep_SalCur.NullText = "";
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
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(628, 434);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.grd_MainInfo);
		base.Controls.Add(this.btnCancel);
		base.FormId = "oSalInfoByEmp";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SalInfoByEmp";
		base.ShowInTaskbar = false;
		this.Text = "Employee Salary Information";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_SalaryInfo_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.rep_Deparment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_MainInfo).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_MainInfo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateValidFrom.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateValidFrom).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateValidTo.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateValidTo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SalCur).EndInit();
		base.ResumeLayout(false);
	}
}
