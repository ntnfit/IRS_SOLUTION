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
using Apzon.Forms.Commons;
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

namespace Apzon.Forms.HumanResources.Salary;

public class frm_SalAdvanced : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public ReloadMain ReloadMain;

	public DataTable DtSearch;

	private bool _isCancel;

	private IContainer components;

	public SimpleButtonEx btn_Search;

	private LabelLine lbl_Period;

	private GridControlEx grd_Advanced;

	private GridViewEx gv_Advanced;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private GridColumnEx gc_EmpNo;

	private GridColumnEx gc_EmployeeName;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gc_Branch;

	private GridColumnEx gc_Department;

	private GridColumnEx gc_Status;

	private GridColumnEx gc_Period;

	private GridColumnEx gc_Amount;

	private GridColumnEx gc_Remain;

	private GridColumnEx gc_MonthlyPay;

	private GridColumnEx gc_Received;

	private GridColumnEx gc_Remarks;

	private RepositoryItemLookUpEdit luk_Branch;

	private RepositoryItemLookUpEdit luk_Status;

	private RepositoryItemLookUpEdit luk_Department;

	private GridColumnEx gc_LineNum;

	private RepositoryItemButtonEditCFL btn_EmpName;

	private RepositoryItemCheckEdit chk_MonthlyPay;

	private RepositoryItemCheckEdit chk_Received;

	private RepositoryItemMemoExEdit mem_Remarks;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private LookUpEditCombo luk_SearchPeriod;

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
			btn_Save.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_SalAdvanced()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SalaryInfo"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SalaryInfo" && x.authoriza == "F"))
			{
				Controls.EnableDisableControl(isEnable: false);
				btn_Search.Enabled = true;
			}
			Load += Frm_SalAdvanced_Load;
			FormClosing += Frm_SalAdvanced_FormClosing;
			gv_Advanced.CellValueChanged += Gv_Advanced_CellValueChanged;
			gv_Advanced.CellValueChanging += Gv_Advanced_CellValueChanging;
			gv_Advanced.FormId = FormId;
			gv_Advanced.ItemId = "APZ_HSI2";
			gv_Advanced.ChangeLogType = Enums.ChangeLog.clSalAdvance;
			gv_Advanced.IsApzTable = true;
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void Frm_SalAdvanced_Load(object sender, EventArgs e)
	{
		LoadInitForm();
	}

	private async void LoadInitForm()
	{
		_ = 2;
		try
		{
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadComboDept();
			luk_Department.DataSource = dataTable.Copy();
			luk_Department.DisplayMember = "Name";
			luk_Department.ValueMember = "Code";
			luk_Department.BestFitMode = BestFitMode.BestFitResizePopup;
			luk_Department.ForceInitialize();
			luk_Department.PopulateColumns();
			luk_Department.Columns["Code"].Visible = false;
			luk_Department.Columns["Remarks"].Visible = false;
			DataTable dataTable2 = await _httpClient.EmployeeMasterClient.GetEmployeeStatus();
			luk_Status.DataSource = dataTable2.Copy();
			luk_Status.DisplayMember = "Name";
			luk_Status.ValueMember = "Code";
			luk_Status.BestFitMode = BestFitMode.BestFitResizePopup;
			luk_Status.ForceInitialize();
			luk_Status.PopulateColumns();
			luk_Status.Columns["Code"].Visible = false;
			DataTable dataTable3 = await _httpClient.EmployeeMasterClient.LoadComboBranch();
			DataRow row = dataTable3.NewRow();
			dataTable3.Rows.InsertAt(row, 0);
			dataTable3.AcceptChanges();
			luk_Branch.DataSource = dataTable3.Copy();
			luk_Branch.DisplayMember = "Name";
			luk_Branch.ValueMember = "Code";
			luk_Branch.BestFitMode = BestFitMode.BestFitResizePopup;
			luk_Branch.ForceInitialize();
			luk_Branch.PopulateColumns();
			luk_Branch.Columns["Code"].Visible = false;
			btn_EmpName.ValueMember = "empID";
			btn_EmpName.DisplayMember = "lastName";
			btn_EmpName.DisplayMember2 = "firstName";
			btn_EmpName.SeparateString = " ";
			btn_EmpName.ContainerControl = this;
			btn_EmpName.AutoLoadDataSource = false;
			btn_EmpName.FindCFLBy(Enums.TypeCFL.Employees, true, "Apz_Cfl_EmployleeData", Enums.ItemObjectType.NoItemType, null, "");
			gc_EmployeeName.FilterMode = ColumnFilterMode.DisplayText;
			btn_EmpName.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			btn_EmpName.RefreshDataSource();
			DataTable workPeriodsByBranch = _httpClient.SalaryInfoAdvanceClient.GetWorkPeriodsByBranch(Function.ParseInt(DtSearch.Rows[0]["branch"]));
			DataRow row2 = workPeriodsByBranch.NewRow();
			workPeriodsByBranch.Rows.InsertAt(row2, 0);
			workPeriodsByBranch.AcceptChanges();
			luk_SearchPeriod.Properties.DataSource = workPeriodsByBranch.Copy();
			luk_SearchPeriod.Properties.DisplayMember = "Name";
			luk_SearchPeriod.Properties.ValueMember = "Code";
			luk_SearchPeriod.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			luk_SearchPeriod.Properties.ForceInitialize();
			luk_SearchPeriod.Properties.PopulateColumns();
			luk_SearchPeriod.Properties.Columns["Code"].Visible = false;
			luk_SearchPeriod.Properties.Columns["status"].Visible = false;
			FormMode = Enums.FormViewModeType.View;
			luk_SearchPeriod.EditValue = string.Empty;
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
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_HSI2");
			tableStructure.Columns["EmpId"].ColumnName = "empID";
			DataTable dataSourceTable = grd_Advanced.DataSourceTable;
			if (dataSourceTable.IsNotNull())
			{
				List<DataRow> list = (from r in dataSourceTable.AsEnumerable()
					where r["Amount"] != DBNull.Value || r["Remain"] != DBNull.Value
					select r).ToList();
				if (list.Count > 0)
				{
					foreach (DataRow item in list)
					{
						tableStructure.Rows.Add(item["empID"], item["LineNum"], item["Period"], item["Amount"], item["Remain"], item["MonthlyPay"], item["Received"], item["Remarks"], null, GlobalData.UserID, DateTime.Now.Date);
					}
					tableStructure.AcceptChanges();
				}
			}
			WinCommons._OpenProcessing();
			if (tableStructure.IsNotNull())
			{
				string text = await _httpClient.SalaryInfoAdvanceClient.SaveDoc(tableStructure);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					FormMode = Enums.FormViewModeType.View;
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
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
		return string.Empty;
	}

	private void DataChanged()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void Frm_SalAdvanced_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (_isCancel || FormMode != Enums.FormViewModeType.Update)
			{
				return;
			}
			switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
			{
			case DialogResult.No:
				Close();
				break;
			case DialogResult.Yes:
				if (string.IsNullOrEmpty(await SaveData()))
				{
					Close();
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

	private void Gv_Advanced_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Amount")
			{
				if (e.Value == DBNull.Value)
				{
					gv_Advanced.SetFocusedRowCellValue(e.Column.FieldName, 0);
				}
			}
			else if (e.Column.FieldName == "Remain" && e.Value == DBNull.Value)
			{
				gv_Advanced.SetFocusedRowCellValue(e.Column.FieldName, 0);
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Gv_Advanced_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		DataChanged();
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			string value = Function.ToString(luk_SearchPeriod.EditValue);
			if (string.IsNullOrWhiteSpace(value))
			{
				WinCommons.Error(Enums.MessageCode.PeriodCannotBeNull, "");
				return;
			}
			if (DtSearch.IsNotNull() && !DtSearch.Columns.Contains("period"))
			{
				DtSearch.Columns.Add("period");
			}
			DtSearch.Rows[0]["period"] = value;
			DataTable dataTable = await _httpClient.SalaryInfoAdvanceClient.SearchByPeriod(DtSearch);
			if (dataTable.IsNotNull())
			{
				grd_Advanced.DataSource = dataTable;
				grd_Advanced.RefreshDataSource();
			}
			gv_Advanced.ExpandAllGroups();
			gv_Advanced.LayoutChanged();
			await gv_Advanced.ReloadFormSetting();
			FormMode = Enums.FormViewModeType.View;
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
				btn_Search_Click(null, null);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.Salary.frm_SalAdvanced));
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
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbl_Period = new Apzon.Controls.Controls.LabelLine();
		this.grd_Advanced = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Advanced = new Apzon.Controls.Grid.GridViewEx();
		this.gc_Branch = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Branch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Status = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_LineNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Department = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Department = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_EmpNo = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_EmployeeName = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_EmpName = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Period = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Amount = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gc_Remain = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_MonthlyPay = new Apzon.Controls.Grid.GridColumnEx();
		this.chk_MonthlyPay = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gc_Received = new Apzon.Controls.Grid.GridColumnEx();
		this.chk_Received = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gc_Remarks = new Apzon.Controls.Grid.GridColumnEx();
		this.mem_Remarks = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.luk_SearchPeriod = new Apzon.Forms.UserControls.LookUpEditCombo();
		((System.ComponentModel.ISupportInitialize)this.grd_Advanced).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Advanced).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Department).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_EmpName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_MonthlyPay).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Received).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mem_Remarks).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_SearchPeriod.Properties).BeginInit();
		base.SuspendLayout();
		this.btn_Search.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Search.Appearance.Options.UseBackColor = true;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = true;
		this.btn_Search.Location = new System.Drawing.Point(315, 12);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(99, 20);
		this.btn_Search.TabIndex = 92;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.lbl_Period.AllowDrop = true;
		this.lbl_Period.EditControl = null;
		this.lbl_Period.EditValue = "Period";
		this.lbl_Period.FieldName = null;
		this.lbl_Period.HideWhenEmpty = false;
		this.lbl_Period.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Period.Location = new System.Drawing.Point(27, 15);
		this.lbl_Period.Name = "lbl_Period";
		this.lbl_Period.ReadOnly = false;
		this.lbl_Period.SettingInVisible = false;
		this.lbl_Period.SettingReadonly = false;
		this.lbl_Period.ShowUnderLine = true;
		this.lbl_Period.Size = new System.Drawing.Size(117, 20);
		this.lbl_Period.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Period.TabIndex = 93;
		this.lbl_Period.Utext = "Period";
		this.grd_Advanced.AllowPasteExcel = true;
		this.grd_Advanced.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Advanced.ClipboardData = null;
		this.grd_Advanced.Location = new System.Drawing.Point(12, 42);
		this.grd_Advanced.MainView = this.gv_Advanced;
		this.grd_Advanced.Name = "grd_Advanced";
		this.grd_Advanced.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.luk_Branch, this.luk_Status, this.luk_Department, this.btn_EmpName, this.chk_MonthlyPay, this.chk_Received, this.mem_Remarks, this.repositoryItemCalcEdit1 });
		this.grd_Advanced.Size = new System.Drawing.Size(700, 403);
		this.grd_Advanced.TabIndex = 95;
		this.grd_Advanced.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Advanced });
		this.gv_Advanced.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Advanced.ColumnKey = "empID";
		this.gv_Advanced.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[13]
		{
			this.gc_Branch, this.gc_Status, this.gc_LineNum, this.gc_Department, this.gc_EmpNo, this.gc_EmployeeName, this.gridColumnEx9, this.gc_Period, this.gc_Amount, this.gc_Remain,
			this.gc_MonthlyPay, this.gc_Received, this.gc_Remarks
		});
		this.gv_Advanced.DisableFormSetting = false;
		this.gv_Advanced.DisplayException = false;
		this.gv_Advanced.EditColumn = null;
		this.gv_Advanced.FieldCurrency = null;
		this.gv_Advanced.FieldRate = null;
		this.gv_Advanced.FormId = "oSalAdvance";
		this.gv_Advanced.GridControl = this.grd_Advanced;
		this.gv_Advanced.GroupCount = 1;
		this.gv_Advanced.IgnoreFilter = false;
		this.gv_Advanced.IndexEditRow = -1;
		this.gv_Advanced.IndicatorWidth = 30;
		this.gv_Advanced.IsApzTable = false;
		this.gv_Advanced.IsInsertNewRow = true;
		this.gv_Advanced.IsLoadingData = false;
		this.gv_Advanced.IsMappingContextAction = false;
		this.gv_Advanced.ItemId = "APZ_HSI1";
		this.gv_Advanced.LstColumnAllowBatch = null;
		this.gv_Advanced.LstColumnNotAllowBatch = null;
		this.gv_Advanced.LstColumnNotEditable = null;
		this.gv_Advanced.LstColumNotSetting = null;
		this.gv_Advanced.Name = "gv_Advanced";
		this.gv_Advanced.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_Advanced.OptionsView.ColumnAutoWidth = false;
		this.gv_Advanced.OptionsView.ShowGroupPanel = false;
		this.gv_Advanced.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gc_Department, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gv_Advanced.UseBatchMod = false;
		this.gc_Branch.Caption = "Branch";
		this.gc_Branch.ColumnEdit = this.luk_Branch;
		this.gc_Branch.CurrencyDateField = null;
		this.gc_Branch.CurrencyField = null;
		this.gc_Branch.CurrencyFixed = null;
		this.gc_Branch.CurrencyFixedRate = 0.0;
		this.gc_Branch.CurrencyRateField = null;
		this.gc_Branch.Name = "gc_Branch";
		this.gc_Branch.OptionsColumn.ReadOnly = true;
		this.gc_Branch.PrimaryKey = false;
		this.gc_Branch.UnitField = null;
		this.luk_Branch.AutoHeight = false;
		this.luk_Branch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Branch.Name = "luk_Branch";
		this.luk_Branch.NullText = "";
		this.gc_Status.Caption = "Status";
		this.gc_Status.ColumnEdit = this.luk_Status;
		this.gc_Status.CurrencyDateField = null;
		this.gc_Status.CurrencyField = null;
		this.gc_Status.CurrencyFixed = null;
		this.gc_Status.CurrencyFixedRate = 0.0;
		this.gc_Status.CurrencyRateField = null;
		this.gc_Status.FieldName = "status";
		this.gc_Status.Name = "gc_Status";
		this.gc_Status.OptionsColumn.ReadOnly = true;
		this.gc_Status.PrimaryKey = false;
		this.gc_Status.UnitField = null;
		this.luk_Status.AutoHeight = false;
		this.luk_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Status.Name = "luk_Status";
		this.luk_Status.NullText = "";
		this.gc_LineNum.Caption = "LineNum";
		this.gc_LineNum.CurrencyDateField = null;
		this.gc_LineNum.CurrencyField = null;
		this.gc_LineNum.CurrencyFixed = null;
		this.gc_LineNum.CurrencyFixedRate = 0.0;
		this.gc_LineNum.CurrencyRateField = null;
		this.gc_LineNum.FieldName = "LineNum";
		this.gc_LineNum.Name = "gc_LineNum";
		this.gc_LineNum.PrimaryKey = false;
		this.gc_LineNum.UnitField = null;
		this.gc_Department.Caption = "Department";
		this.gc_Department.ColumnEdit = this.luk_Department;
		this.gc_Department.CurrencyDateField = null;
		this.gc_Department.CurrencyField = null;
		this.gc_Department.CurrencyFixed = null;
		this.gc_Department.CurrencyFixedRate = 0.0;
		this.gc_Department.CurrencyRateField = null;
		this.gc_Department.FieldName = "dept";
		this.gc_Department.Name = "gc_Department";
		this.gc_Department.OptionsColumn.ReadOnly = true;
		this.gc_Department.PrimaryKey = false;
		this.gc_Department.UnitField = null;
		this.gc_Department.Visible = true;
		this.gc_Department.VisibleIndex = 0;
		this.gc_Department.Width = 86;
		this.luk_Department.AutoHeight = false;
		this.luk_Department.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Department.Name = "luk_Department";
		this.luk_Department.NullText = "";
		this.gc_EmpNo.Caption = "Emp No";
		this.gc_EmpNo.CurrencyDateField = null;
		this.gc_EmpNo.CurrencyField = null;
		this.gc_EmpNo.CurrencyFixed = null;
		this.gc_EmpNo.CurrencyFixedRate = 0.0;
		this.gc_EmpNo.CurrencyRateField = null;
		this.gc_EmpNo.FieldName = "empID";
		this.gc_EmpNo.Name = "gc_EmpNo";
		this.gc_EmpNo.OptionsColumn.ReadOnly = true;
		this.gc_EmpNo.PrimaryKey = false;
		this.gc_EmpNo.UnitField = null;
		this.gc_EmpNo.Visible = true;
		this.gc_EmpNo.VisibleIndex = 0;
		this.gc_EmpNo.Width = 65;
		this.gc_EmployeeName.Caption = "Employee Name";
		this.gc_EmployeeName.ColumnEdit = this.btn_EmpName;
		this.gc_EmployeeName.CurrencyDateField = null;
		this.gc_EmployeeName.CurrencyField = null;
		this.gc_EmployeeName.CurrencyFixed = null;
		this.gc_EmployeeName.CurrencyFixedRate = 0.0;
		this.gc_EmployeeName.CurrencyRateField = null;
		this.gc_EmployeeName.FieldName = "empName";
		this.gc_EmployeeName.Name = "gc_EmployeeName";
		this.gc_EmployeeName.OptionsColumn.ReadOnly = true;
		this.gc_EmployeeName.PrimaryKey = false;
		this.gc_EmployeeName.UnitField = null;
		this.gc_EmployeeName.Visible = true;
		this.gc_EmployeeName.VisibleIndex = 1;
		this.gc_EmployeeName.Width = 110;
		this.btn_EmpName.AutoHeight = false;
		this.btn_EmpName.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.btn_EmpName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_EmpName.ContainerControl = this;
		this.btn_EmpName.DisplayMember = "";
		this.btn_EmpName.DisplayMember2 = "";
		this.btn_EmpName.LoadDataSourceWhenEnter = false;
		this.btn_EmpName.Name = "btn_EmpName";
		this.btn_EmpName.ReadOnly = true;
		this.btn_EmpName.SeparateCharacterForMulti = ';';
		this.btn_EmpName.SeparateString = ", ";
		this.btn_EmpName.UsingAutoCompleteBox = false;
		this.btn_EmpName.UsingDataSource = true;
		this.btn_EmpName.ValidateWhenNoDataSource = false;
		this.btn_EmpName.ValueMember = "";
		this.gridColumnEx9.Caption = "Emp Ext. No";
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
		this.gridColumnEx9.Width = 80;
		this.gc_Period.Caption = "Period";
		this.gc_Period.CurrencyDateField = null;
		this.gc_Period.CurrencyField = null;
		this.gc_Period.CurrencyFixed = null;
		this.gc_Period.CurrencyFixedRate = 0.0;
		this.gc_Period.CurrencyRateField = null;
		this.gc_Period.FieldName = "Period";
		this.gc_Period.Name = "gc_Period";
		this.gc_Period.OptionsColumn.ReadOnly = true;
		this.gc_Period.PrimaryKey = false;
		this.gc_Period.UnitField = null;
		this.gc_Period.Visible = true;
		this.gc_Period.VisibleIndex = 3;
		this.gc_Period.Width = 70;
		this.gc_Amount.Caption = "Amount";
		this.gc_Amount.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gc_Amount.CurrencyDateField = null;
		this.gc_Amount.CurrencyField = null;
		this.gc_Amount.CurrencyFixed = null;
		this.gc_Amount.CurrencyFixedRate = 0.0;
		this.gc_Amount.CurrencyRateField = null;
		this.gc_Amount.FieldName = "Amount";
		this.gc_Amount.Name = "gc_Amount";
		this.gc_Amount.PrimaryKey = false;
		this.gc_Amount.UnitField = null;
		this.gc_Amount.Visible = true;
		this.gc_Amount.VisibleIndex = 4;
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.gc_Remain.Caption = "Remain";
		this.gc_Remain.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gc_Remain.CurrencyDateField = null;
		this.gc_Remain.CurrencyField = null;
		this.gc_Remain.CurrencyFixed = null;
		this.gc_Remain.CurrencyFixedRate = 0.0;
		this.gc_Remain.CurrencyRateField = null;
		this.gc_Remain.FieldName = "Remain";
		this.gc_Remain.Name = "gc_Remain";
		this.gc_Remain.PrimaryKey = false;
		this.gc_Remain.UnitField = null;
		this.gc_Remain.Visible = true;
		this.gc_Remain.VisibleIndex = 5;
		this.gc_MonthlyPay.Caption = "MonthlyPay";
		this.gc_MonthlyPay.ColumnEdit = this.chk_MonthlyPay;
		this.gc_MonthlyPay.CurrencyDateField = null;
		this.gc_MonthlyPay.CurrencyField = null;
		this.gc_MonthlyPay.CurrencyFixed = null;
		this.gc_MonthlyPay.CurrencyFixedRate = 0.0;
		this.gc_MonthlyPay.CurrencyRateField = null;
		this.gc_MonthlyPay.FieldName = "MonthlyPay";
		this.gc_MonthlyPay.Name = "gc_MonthlyPay";
		this.gc_MonthlyPay.PrimaryKey = false;
		this.gc_MonthlyPay.UnitField = null;
		this.gc_MonthlyPay.Visible = true;
		this.gc_MonthlyPay.VisibleIndex = 6;
		this.gc_MonthlyPay.Width = 70;
		this.chk_MonthlyPay.AutoHeight = false;
		this.chk_MonthlyPay.Name = "chk_MonthlyPay";
		this.chk_MonthlyPay.ValueChecked = "Y";
		this.chk_MonthlyPay.ValueUnchecked = "N";
		this.gc_Received.Caption = "Received";
		this.gc_Received.ColumnEdit = this.chk_Received;
		this.gc_Received.CurrencyDateField = null;
		this.gc_Received.CurrencyField = null;
		this.gc_Received.CurrencyFixed = null;
		this.gc_Received.CurrencyFixedRate = 0.0;
		this.gc_Received.CurrencyRateField = null;
		this.gc_Received.FieldName = "Received";
		this.gc_Received.Name = "gc_Received";
		this.gc_Received.PrimaryKey = false;
		this.gc_Received.UnitField = null;
		this.gc_Received.Visible = true;
		this.gc_Received.VisibleIndex = 7;
		this.gc_Received.Width = 60;
		this.chk_Received.AutoHeight = false;
		this.chk_Received.Name = "chk_Received";
		this.chk_Received.ValueChecked = "Y";
		this.chk_Received.ValueUnchecked = "N";
		this.gc_Remarks.Caption = "Remarks";
		this.gc_Remarks.ColumnEdit = this.mem_Remarks;
		this.gc_Remarks.CurrencyDateField = null;
		this.gc_Remarks.CurrencyField = null;
		this.gc_Remarks.CurrencyFixed = null;
		this.gc_Remarks.CurrencyFixedRate = 0.0;
		this.gc_Remarks.CurrencyRateField = null;
		this.gc_Remarks.FieldName = "Remarks";
		this.gc_Remarks.Name = "gc_Remarks";
		this.gc_Remarks.PrimaryKey = false;
		this.gc_Remarks.UnitField = null;
		this.gc_Remarks.Visible = true;
		this.gc_Remarks.VisibleIndex = 8;
		this.gc_Remarks.Width = 60;
		this.mem_Remarks.AutoHeight = false;
		this.mem_Remarks.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.mem_Remarks.Name = "mem_Remarks";
		this.mem_Remarks.ShowIcon = false;
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(12, 451);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(93, 20);
		this.btn_Save.TabIndex = 96;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(110, 451);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 97;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.luk_SearchPeriod.EditControl = null;
		this.luk_SearchPeriod.FieldName = null;
		this.luk_SearchPeriod.IsSearchControl = true;
		this.luk_SearchPeriod.Location = new System.Drawing.Point(142, 12);
		this.luk_SearchPeriod.Name = "luk_SearchPeriod";
		this.luk_SearchPeriod.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_SearchPeriod.Properties.IsSearchControl = false;
		this.luk_SearchPeriod.Properties.NullText = "";
		this.luk_SearchPeriod.SettingInVisible = false;
		this.luk_SearchPeriod.SettingReadonly = false;
		this.luk_SearchPeriod.Size = new System.Drawing.Size(167, 20);
		this.luk_SearchPeriod.TabIndex = 98;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(724, 483);
		base.Controls.Add(this.luk_SearchPeriod);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.grd_Advanced);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.lbl_Period);
		base.FormId = "oSalAdvanced";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SalAdvanced";
		this.Text = "Employee Salary Advanced - Edit";
		((System.ComponentModel.ISupportInitialize)this.grd_Advanced).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Advanced).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Department).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_EmpName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_MonthlyPay).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Received).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mem_Remarks).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_SearchPeriod.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
