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
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources.TimeSheeting;

public class frm_LeaveInformation : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private readonly Dictionary<string, string> _dicWorkCode = new Dictionary<string, string>();

	private bool _forceClose;

	private bool _isCancel;

	private bool _isChanging;

	private DataTable _dtSetting;

	private IContainer components;

	public CheckedComboBoxEditEx ccb_WorkDay;

	private LabelLine labelLine7;

	private LabelLine labelLine6;

	private LabelLine labelLine5;

	public TextEditEx txt_EmpName;

	private LabelLine labelLine4;

	public TextEditEx txt_EmpId;

	private LabelLine labelLine3;

	private LabelLine labelLine2;

	private LabelLine labelLine1;

	public SimpleButtonEx btnExport;

	public GridControl grd_Export;

	public GridView gv_Export;

	public SimpleButtonEx btnImport;

	public SimpleButtonEx btn_Search;

	public SimpleButtonEx btCancel;

	public SimpleButtonEx btnOK;

	public GridControlEx grd_Doc;

	public GridViewEx gv_Doc;

	public GridColumnEx EmpId;

	private RepositoryItemButtonEditCFL rep_DrillDownEmp;

	public GridColumnEx gcPeriod;

	private GridColumnEx gcLeaveTotal;

	private GridColumnEx gcUsed;

	private GridColumnEx gcRemain;

	private CheckedComboBoxEditEx luk_Status;

	private LookUpEditCombo luk_Branch;

	private LookUpEditCombo luk_Period;

	private GridColumnEx gcDepart;

	private GridColumnEx gcBranch;

	private RepositoryItemLookUpEdit rep_Dept;

	private RepositoryItemLookUpEdit rep_Branch;

	private GridColumnEx EmpName;

	private GridColumnEx gridColumnEx1;

	private CheckedComboBoxEditEx luk_Department;

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

	public frm_LeaveInformation()
	{
		InitializeComponent();
		FormId = "oLeaveInformation";
		gv_Doc.ItemId = "APZ_TSL2";
		gv_Doc.FormId = "oLeaveInformation";
		RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = rep_DrillDownEmp;
		repositoryItemButtonEditCFL.DrillDownMethodWithGridView = (Action<string, GridView>)Delegate.Combine(repositoryItemButtonEditCFL.DrillDownMethodWithGridView, (Action<string, GridView>)((string input, GridView view) =>
		{
			this.DrillDown(ApzObjectTypes.oEmployeesInfo, Function.ToString(view.GetFocusedRowCellValue("empID")));
		}));
		OnLoadAsync += frm_LeaveInformation_Load;
	}

	private async Task frm_LeaveInformation_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			await LoadInitForm();
			await LoadSearchSetting();
			await LoadWorkCode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadInitForm()
	{
		_ = 2;
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
			rep_DrillDownEmp.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			rep_DrillDownEmp.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			int branch = Function.ParseInt(luk_Branch.EditValue);
			luk_Period.Properties.DataSource = _httpClient.LeaveInformationClient.GetWorkPeriodsByBranch(branch);
			luk_Period.Properties.DisplayMember = "Name";
			luk_Period.Properties.ValueMember = "Code";
			luk_Period.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			luk_Period.Properties.ForceInitialize();
			luk_Period.Properties.PopulateColumns();
			luk_Period.Properties.Columns["Code"].Visible = false;
			if (dataTable3.Rows.Count == 2)
			{
				luk_Branch.ItemIndex = 1;
			}
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
			_dtSetting = await WinFuntions.LoadSearchSetting(FormId, gv_Doc.ItemId);
			if (_dtSetting.HasColumn() && _dtSetting.IsNotNull())
			{
				DataRow dataRow = _dtSetting.Rows[0];
				luk_Branch.EditValue = Function.ParseShort(dataRow["Branch"]);
				luk_Department.EditValue = Function.ToString(dataRow["dept"]);
				luk_Department.RefreshEditValue();
				luk_Period.EditValue = Function.ToString(dataRow["Period"]);
				luk_Status.EditValue = Function.ToString(dataRow["Status"]);
				luk_Status.RefreshEditValue();
				if (Function.ParseInt(dataRow["EmpId"]) > 0)
				{
					txt_EmpId.EditValue = Function.ParseInt(dataRow["EmpId"]);
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

	private async Task LoadWorkCode()
	{
		try
		{
			int currentBranch = Function.ParseInt(luk_Branch.EditValue);
			DataTable dataTable = await _httpClient.TimeSheetingClient.LoadWorkCode();
			if (!dataTable.IsNotNull())
			{
				return;
			}
			_dicWorkCode.Clear();
			EnumerableRowCollection<DataRow> enumerableRowCollection = from t in dataTable.AsEnumerable()
				where Function.ParseInt(t["Branch"]) == currentBranch
				select t;
			if (!enumerableRowCollection.Any())
			{
				return;
			}
			foreach (DataRow item in enumerableRowCollection)
			{
				_dicWorkCode.Add(Function.ToString(item["Code"]), Function.ToString(item["Name"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_Branch_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(luk_Branch.EditValue);
			if (num == 0)
			{
				luk_Period.EditValue = null;
				return;
			}
			grd_Doc.DataSource = null;
			grd_Doc.RefreshDataSource();
			DataTable workPeriodsByBranch = _httpClient.LeaveInformationClient.GetWorkPeriodsByBranch(num);
			luk_Period.Properties.DataSource = null;
			workPeriodsByBranch.AcceptChanges();
			luk_Period.Properties.DataSource = workPeriodsByBranch.Copy();
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

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(luk_Branch.EditValue)))
			{
				WinCommons.Warning(Enums.MessageCode.PleaseSelectedABranch, "");
				grd_Doc.DataSource = null;
				grd_Doc.RefreshDataSource();
				luk_Period.Properties.DataSource = null;
			}
			else if (string.IsNullOrEmpty(Function.ToString(luk_Period.EditValue)))
			{
				WinCommons.Warning(Enums.MessageCode.PleaseSelectPeriod2Continue, "");
			}
			else if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await LoadWorkCode();
				await LoadData();
				SaveSearchSetting();
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
			dataRow["Dept"] = Function.ToString(luk_Department.EditValue);
			dataRow["EmpId"] = Function.ParseInt(txt_EmpId.EditValue);
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

	private async Task LoadData()
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
			dataRow["period"] = Function.ToString(luk_Period.EditValue);
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataTable dataTable2 = await _httpClient.LeaveInformationClient.SearchLeaveInfo(dataTable);
			if (dataTable2 != null)
			{
				List<GridColumnEx> list = new List<GridColumnEx>();
				foreach (GridColumnEx column in gv_Doc.Columns)
				{
					if (column.FieldName.Contains("WorkCode_"))
					{
						list.Add(column);
					}
				}
				foreach (GridColumnEx item in list)
				{
					gv_Doc.Columns.Remove(item);
				}
				foreach (DataColumn col in dataTable2.Columns)
				{
					if ((col.ColumnName.StartsWith("WorkCode_") || (col.ColumnName != "dept" && col.ColumnName != "status" && col.ColumnName != "branch" && col.ColumnName != "empID" && col.ColumnName != "empName" && col.ColumnName != "LeaveTotal" && col.ColumnName != "Used" && col.ColumnName != "Remain" && col.ColumnName != "ExtEmpNo")) && gv_Doc.Columns.AsEnumerable().FirstOrDefault((GridColumn t) => t.FieldName == col.ColumnName) == null)
					{
						GridColumnEx gridColumnEx2 = new GridColumnEx
						{
							FieldName = col.ColumnName,
							Visible = true,
							Width = 150
						};
						if (col.ColumnName.StartsWith("WorkCode_"))
						{
							gridColumnEx2.Caption = Function.ToString(_dicWorkCode[col.ColumnName.Substring(9)]);
						}
						else
						{
							gridColumnEx2.Caption = col.Caption;
						}
						gridColumnEx2.OptionsColumn.ReadOnly = true;
						new RepositoryItemCalcEdit();
						gv_Doc.BeginUpdate();
						gv_Doc.Columns.Insert(gv_Doc.Columns.Count, gridColumnEx2);
						gv_Doc.EndUpdate();
					}
				}
				dataTable2.AcceptChanges();
				grd_Doc.DataSource = dataTable2.Copy();
				grd_Doc.RefreshDataSource();
				await gv_Doc.ReloadFormSetting();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			gv_Doc.ExpandAllGroups();
			FormMode = Enums.FormViewModeType.View;
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

	private void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			ColumnView columnView = (ColumnView)sender;
			bool flag = false;
			if (!(columnView.FocusedColumn.FieldName == "LeaveTotal"))
			{
				return;
			}
			foreach (GridColumn column in columnView.Columns)
			{
				if (column.FieldName.Contains("WorkCode") && !string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue(column.FieldName))))
				{
					flag = true;
					break;
				}
			}
			if (flag)
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

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column == gcLeaveTotal)
			{
				bool flag = false;
				foreach (GridColumn column in gv_Doc.Columns)
				{
					if (column.FieldName.Contains("WorkCode") && !string.IsNullOrEmpty(Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, column.FieldName))))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					e.Appearance.BackColor = Color.LightGray;
				}
				else
				{
					e.Appearance.BackColor = Color.White;
				}
			}
			if (e.Column.FieldName.Contains("WorkCode") || e.Column == gcRemain || e.Column == gcUsed)
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

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_isChanging)
			{
				if (e.Column.FieldName == "LeaveTotal")
				{
					double num = Function.ParseDouble(gv_Doc.GetRowCellValue(e.RowHandle, "LeaveTotal"));
					double num2 = Function.ParseDouble(gv_Doc.GetRowCellValue(e.RowHandle, "Used"));
					_isChanging = true;
					gv_Doc.BeginUpdate();
					gv_Doc.SetRowCellValue(e.RowHandle, "Used", 0);
					gv_Doc.SetRowCellValue(e.RowHandle, "Remain", num - num2);
					gv_Doc.EndUpdate();
					_isChanging = false;
				}
				FormMode = Enums.FormViewModeType.Update;
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
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					btnOK.Enabled = false;
					btCancel.Enabled = false;
					await SaveData();
					btnOK.Enabled = true;
					btCancel.Enabled = true;
				}
			}
			else
			{
				_isCancel = true;
				Close();
				_isCancel = false;
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
			DataTable dataTable = grd_Doc.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				object currentDocNum = gv_Doc.GetFocusedRowCellValue("EmpId");
				WinCommons._OpenProcessing();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					DataRow dataRow = dataTable.Rows[i];
					dataRow["UserSign2"] = GlobalData.UserID;
					dataRow["UpdateDate"] = DateTime.Today;
				}
				HttpResult httpResult = await _httpClient.LeaveInformationClient.SaveLeaveInformation(dataTable);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					FormMode = Enums.FormViewModeType.View;
					REFRESH_DATA();
					int num = gv_Doc.LocateByValue("EmpId", currentDocNum);
					if (num != int.MinValue)
					{
						gv_Doc.FocusedRowHandle = num;
						gv_Doc.MakeRowVisible(num);
						gv_Doc.Focus();
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
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

	private void btCancel_Click(object sender, EventArgs e)
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

	protected override void NoAuthSetControlSearch()
	{
		TextEditEx textEditEx = txt_EmpId;
		TextEditEx textEditEx2 = txt_EmpName;
		LookUpEditCombo lookUpEditCombo = luk_Branch;
		CheckedComboBoxEditEx checkedComboBoxEditEx = luk_Department;
		CheckedComboBoxEditEx checkedComboBoxEditEx2 = luk_Status;
		bool flag = (luk_Period.ReadOnly = false);
		bool flag3 = (checkedComboBoxEditEx2.ReadOnly = flag);
		bool flag5 = (checkedComboBoxEditEx.ReadOnly = flag3);
		bool flag7 = (lookUpEditCombo.ReadOnly = flag5);
		bool readOnly = (textEditEx2.ReadOnly = flag7);
		textEditEx.ReadOnly = readOnly;
	}

	private async void frm_LeaveInformation_FormClosing(object sender, FormClosingEventArgs e)
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.TimeSheeting.frm_LeaveInformation));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.ccb_WorkDay = new Apzon.Controls.Editor.CheckedComboBoxEditEx();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.txt_EmpName = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.txt_EmpId = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.btnExport = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Export = new DevExpress.XtraGrid.GridControl();
		this.gv_Export = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.btnImport = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.EmpId = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.EmpName = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_DrillDownEmp = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcDepart = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Dept = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcBranch = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Branch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcPeriod = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLeaveTotal = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUsed = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRemain = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Status = new Apzon.Controls.Editor.CheckedComboBoxEditEx();
		this.luk_Branch = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_Period = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_Department = new Apzon.Controls.Editor.CheckedComboBoxEditEx();
		((System.ComponentModel.ISupportInitialize)this.ccb_WorkDay.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpId.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Export).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Export).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DrillDownEmp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Dept).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Period.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Department.Properties).BeginInit();
		base.SuspendLayout();
		this.ccb_WorkDay.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.ccb_WorkDay.EditControl = null;
		this.ccb_WorkDay.FieldName = null;
		this.ccb_WorkDay.IsSearchControl = true;
		this.ccb_WorkDay.Location = new System.Drawing.Point(907, 111);
		this.ccb_WorkDay.Name = "ccb_WorkDay";
		this.ccb_WorkDay.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.ccb_WorkDay.SettingInVisible = false;
		this.ccb_WorkDay.SettingReadonly = false;
		this.ccb_WorkDay.Size = new System.Drawing.Size(135, 20);
		this.ccb_WorkDay.TabIndex = 66;
		this.ccb_WorkDay.Visible = false;
		this.labelLine7.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "Date Filter";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(850, 114);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(66, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 78;
		this.labelLine7.Utext = "Date Filter";
		this.labelLine7.Visible = false;
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Period";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(259, 47);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(54, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 76;
		this.labelLine6.Utext = "Period";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Status";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(12, 14);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(104, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 75;
		this.labelLine5.Utext = "Status";
		this.txt_EmpName.EditControl = null;
		this.txt_EmpName.FieldName = null;
		this.txt_EmpName.IsSearchControl = true;
		this.txt_EmpName.Location = new System.Drawing.Point(97, 104);
		this.txt_EmpName.Name = "txt_EmpName";
		this.txt_EmpName.SettingInVisible = false;
		this.txt_EmpName.SettingReadonly = false;
		this.txt_EmpName.Size = new System.Drawing.Size(156, 20);
		this.txt_EmpName.TabIndex = 60;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Employee Name";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(12, 107);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(104, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 74;
		this.labelLine4.Tag = "Employee Name";
		this.labelLine4.Utext = "Employee Name";
		this.txt_EmpId.EditControl = null;
		this.txt_EmpId.FieldName = null;
		this.txt_EmpId.IsSearchControl = true;
		this.txt_EmpId.Location = new System.Drawing.Point(97, 74);
		this.txt_EmpId.Name = "txt_EmpId";
		this.txt_EmpId.SettingInVisible = false;
		this.txt_EmpId.SettingReadonly = false;
		this.txt_EmpId.Size = new System.Drawing.Size(156, 20);
		this.txt_EmpId.TabIndex = 59;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Employee No.";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(12, 77);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(104, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 72;
		this.labelLine3.Utext = "Employee No.";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Department";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(12, 47);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(104, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 73;
		this.labelLine2.Utext = "Department";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Branch";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(259, 14);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(54, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 71;
		this.labelLine1.Utext = "Branch";
		this.btnExport.EditControl = null;
		this.btnExport.EditValue = "Export To Excel";
		this.btnExport.FieldName = null;
		this.btnExport.IsSearchControl = false;
		this.btnExport.Location = new System.Drawing.Point(513, 106);
		this.btnExport.Name = "btnExport";
		this.btnExport.ReadOnly = false;
		this.btnExport.SettingInVisible = false;
		this.btnExport.SettingReadonly = false;
		this.btnExport.Size = new System.Drawing.Size(121, 22);
		this.btnExport.TabIndex = 69;
		this.btnExport.Tag = "Export To Excel";
		this.btnExport.Text = "Export To Excel";
		this.btnExport.Visible = false;
		this.grd_Export.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Export.Location = new System.Drawing.Point(761, 10);
		this.grd_Export.MainView = this.gv_Export;
		this.grd_Export.Name = "grd_Export";
		this.grd_Export.Size = new System.Drawing.Size(281, 86);
		this.grd_Export.TabIndex = 68;
		this.grd_Export.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Export });
		this.grd_Export.Visible = false;
		this.gv_Export.GridControl = this.grd_Export;
		this.gv_Export.Name = "gv_Export";
		this.btnImport.EditControl = null;
		this.btnImport.EditValue = "Import From Excel";
		this.btnImport.FieldName = null;
		this.btnImport.IsSearchControl = false;
		this.btnImport.Location = new System.Drawing.Point(386, 106);
		this.btnImport.Name = "btnImport";
		this.btnImport.ReadOnly = false;
		this.btnImport.SettingInVisible = false;
		this.btnImport.SettingReadonly = false;
		this.btnImport.Size = new System.Drawing.Size(121, 22);
		this.btnImport.TabIndex = 67;
		this.btnImport.Tag = "Import Time Sheeting from Excel";
		this.btnImport.Text = "Import From Excel";
		this.btnImport.Visible = false;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = true;
		this.btn_Search.Location = new System.Drawing.Point(259, 106);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(121, 22);
		this.btn_Search.TabIndex = 65;
		this.btn_Search.Tag = "Button To Search Time Sheeting by Period";
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(93, 467);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 58;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOK.EditControl = null;
		this.btnOK.EditValue = "OK";
		this.btnOK.FieldName = null;
		this.btnOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOK.ImageOptions.Image");
		this.btnOK.IsSearchControl = false;
		this.btnOK.Location = new System.Drawing.Point(12, 467);
		this.btnOK.Name = "btnOK";
		this.btnOK.ReadOnly = false;
		this.btnOK.SettingInVisible = false;
		this.btnOK.SettingReadonly = false;
		this.btnOK.Size = new System.Drawing.Size(75, 23);
		this.btnOK.TabIndex = 57;
		this.btnOK.Text = "OK";
		this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Location = new System.Drawing.Point(0, 140);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_DrillDownEmp, this.rep_Dept, this.rep_Branch });
		this.grd_Doc.Size = new System.Drawing.Size(1049, 321);
		this.grd_Doc.TabIndex = 56;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = "EmpId";
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.EmpId, this.gridColumnEx1, this.EmpName, this.gcDepart, this.gcBranch, this.gcPeriod, this.gcLeaveTotal, this.gcUsed, this.gcRemain });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oTimeSheeting";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.GroupCount = 1;
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
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsEditForm.ShowOnEnterKey = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gcDepart, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gv_Doc.Tag = "APZ_TSL2";
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Doc_ShowingEditor);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
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
		this.EmpId.PrimaryKey = true;
		this.EmpId.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedRow;
		this.EmpId.Tag = "APZ_OTSL EmpId";
		this.EmpId.UnitField = null;
		this.EmpId.Visible = true;
		this.EmpId.VisibleIndex = 0;
		this.EmpId.Width = 50;
		this.gridColumnEx1.Caption = "Extened Employee No";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "ExtEmpNo";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 6;
		this.EmpName.Caption = "Employee Name";
		this.EmpName.ColumnEdit = this.rep_DrillDownEmp;
		this.EmpName.CurrencyDateField = null;
		this.EmpName.CurrencyField = null;
		this.EmpName.CurrencyFixed = null;
		this.EmpName.CurrencyFixedRate = 0.0;
		this.EmpName.CurrencyRateField = null;
		this.EmpName.FieldName = "empName";
		this.EmpName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.EmpName.Name = "EmpName";
		this.EmpName.OptionsColumn.ReadOnly = true;
		this.EmpName.PrimaryKey = false;
		this.EmpName.Tag = "APZ_TSL2 EmpName";
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
		this.gcDepart.Caption = "Department";
		this.gcDepart.ColumnEdit = this.rep_Dept;
		this.gcDepart.CurrencyDateField = null;
		this.gcDepart.CurrencyField = null;
		this.gcDepart.CurrencyFixed = null;
		this.gcDepart.CurrencyFixedRate = 0.0;
		this.gcDepart.CurrencyRateField = null;
		this.gcDepart.FieldName = "dept";
		this.gcDepart.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
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
		this.gcBranch.Caption = "Branch";
		this.gcBranch.ColumnEdit = this.rep_Branch;
		this.gcBranch.CurrencyDateField = null;
		this.gcBranch.CurrencyField = null;
		this.gcBranch.CurrencyFixed = null;
		this.gcBranch.CurrencyFixedRate = 0.0;
		this.gcBranch.CurrencyRateField = null;
		this.gcBranch.FieldName = "branch";
		this.gcBranch.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcBranch.Name = "gcBranch";
		this.gcBranch.PrimaryKey = false;
		this.gcBranch.UnitField = null;
		this.rep_Branch.AutoHeight = false;
		this.rep_Branch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Branch.Name = "rep_Branch";
		this.gcPeriod.Caption = "Period";
		this.gcPeriod.CurrencyDateField = null;
		this.gcPeriod.CurrencyField = null;
		this.gcPeriod.CurrencyFixed = null;
		this.gcPeriod.CurrencyFixedRate = 0.0;
		this.gcPeriod.CurrencyRateField = null;
		this.gcPeriod.FieldName = "Period";
		this.gcPeriod.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcPeriod.Name = "gcPeriod";
		this.gcPeriod.OptionsColumn.ReadOnly = true;
		this.gcPeriod.PrimaryKey = false;
		this.gcPeriod.UnitField = null;
		this.gcPeriod.Visible = true;
		this.gcPeriod.VisibleIndex = 2;
		this.gcPeriod.Width = 60;
		this.gcLeaveTotal.Caption = "Leave Total";
		this.gcLeaveTotal.CurrencyDateField = null;
		this.gcLeaveTotal.CurrencyField = null;
		this.gcLeaveTotal.CurrencyFixed = null;
		this.gcLeaveTotal.CurrencyFixedRate = 0.0;
		this.gcLeaveTotal.CurrencyRateField = null;
		this.gcLeaveTotal.FieldName = "LeaveTotal";
		this.gcLeaveTotal.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcLeaveTotal.Name = "gcLeaveTotal";
		this.gcLeaveTotal.PrimaryKey = false;
		this.gcLeaveTotal.UnitField = null;
		this.gcLeaveTotal.Visible = true;
		this.gcLeaveTotal.VisibleIndex = 3;
		this.gcLeaveTotal.Width = 60;
		this.gcUsed.Caption = "Used";
		this.gcUsed.CurrencyDateField = null;
		this.gcUsed.CurrencyField = null;
		this.gcUsed.CurrencyFixed = null;
		this.gcUsed.CurrencyFixedRate = 0.0;
		this.gcUsed.CurrencyRateField = null;
		this.gcUsed.FieldName = "Used";
		this.gcUsed.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcUsed.Name = "gcUsed";
		this.gcUsed.OptionsColumn.ReadOnly = true;
		this.gcUsed.PrimaryKey = false;
		this.gcUsed.UnitField = null;
		this.gcUsed.Visible = true;
		this.gcUsed.VisibleIndex = 4;
		this.gcUsed.Width = 60;
		this.gcRemain.Caption = "Remain";
		this.gcRemain.CurrencyDateField = null;
		this.gcRemain.CurrencyField = null;
		this.gcRemain.CurrencyFixed = null;
		this.gcRemain.CurrencyFixedRate = 0.0;
		this.gcRemain.CurrencyRateField = null;
		this.gcRemain.FieldName = "Remain";
		this.gcRemain.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcRemain.Name = "gcRemain";
		this.gcRemain.OptionsColumn.ReadOnly = true;
		this.gcRemain.PrimaryKey = false;
		this.gcRemain.UnitField = null;
		this.gcRemain.Visible = true;
		this.gcRemain.VisibleIndex = 5;
		this.gcRemain.Width = 60;
		this.luk_Status.EditControl = null;
		this.luk_Status.FieldName = null;
		this.luk_Status.IsSearchControl = true;
		this.luk_Status.Location = new System.Drawing.Point(97, 11);
		this.luk_Status.Name = "luk_Status";
		this.luk_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Status.SettingInVisible = false;
		this.luk_Status.SettingReadonly = false;
		this.luk_Status.Size = new System.Drawing.Size(156, 20);
		this.luk_Status.TabIndex = 79;
		this.luk_Branch.EditControl = null;
		this.luk_Branch.FieldName = null;
		this.luk_Branch.IsSearchControl = true;
		this.luk_Branch.Location = new System.Drawing.Point(310, 11);
		this.luk_Branch.Name = "luk_Branch";
		this.luk_Branch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Branch.Properties.IsSearchControl = false;
		this.luk_Branch.Properties.NullText = "";
		this.luk_Branch.SettingInVisible = false;
		this.luk_Branch.SettingReadonly = false;
		this.luk_Branch.Size = new System.Drawing.Size(156, 20);
		this.luk_Branch.TabIndex = 81;
		this.luk_Branch.EditValueChanged += new System.EventHandler(this.luk_Branch_EditValueChanged);
		this.luk_Period.EditControl = null;
		this.luk_Period.FieldName = null;
		this.luk_Period.IsSearchControl = true;
		this.luk_Period.Location = new System.Drawing.Point(310, 44);
		this.luk_Period.Name = "luk_Period";
		this.luk_Period.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Period.Properties.IsSearchControl = false;
		this.luk_Period.Properties.NullText = "";
		this.luk_Period.Properties.PopupSizeable = false;
		this.luk_Period.SettingInVisible = false;
		this.luk_Period.SettingReadonly = false;
		this.luk_Period.Size = new System.Drawing.Size(156, 20);
		this.luk_Period.TabIndex = 82;
		this.luk_Department.EditControl = null;
		this.luk_Department.FieldName = null;
		this.luk_Department.IsSearchControl = true;
		this.luk_Department.Location = new System.Drawing.Point(97, 44);
		this.luk_Department.Name = "luk_Department";
		this.luk_Department.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Department.SettingInVisible = false;
		this.luk_Department.SettingReadonly = false;
		this.luk_Department.Size = new System.Drawing.Size(156, 20);
		this.luk_Department.TabIndex = 80;
		base.AcceptButton = this.btn_Search;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btCancel;
		base.ClientSize = new System.Drawing.Size(1049, 500);
		base.Controls.Add(this.luk_Department);
		base.Controls.Add(this.luk_Period);
		base.Controls.Add(this.luk_Branch);
		base.Controls.Add(this.luk_Status);
		base.Controls.Add(this.ccb_WorkDay);
		base.Controls.Add(this.labelLine7);
		base.Controls.Add(this.labelLine6);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.txt_EmpName);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.txt_EmpId);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btnExport);
		base.Controls.Add(this.grd_Export);
		base.Controls.Add(this.btnImport);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btnOK);
		base.Controls.Add(this.grd_Doc);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_LeaveInformation";
		this.Text = "Employee Leave Information";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_LeaveInformation_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.ccb_WorkDay.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpId.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Export).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Export).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DrillDownEmp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Dept).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Period.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Department.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
