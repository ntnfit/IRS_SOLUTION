using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Pivot;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPivotGrid;

namespace Apzon.Forms.KPIs;

public class frm_KPIsScoringDetail : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public Action ReloadDataListPoint;

	private int absEntry = -1;

	private string setCode = "";

	private bool isShowBenmarking;

	private IContainer components;

	private PivotGridControlEx pv_KPIsScoringDetail;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_Ok;

	private PivotGridField field_EmpName;

	private PivotGridField field_Name;

	private PivotGridField field_Scoring;

	private RepositoryItemCalcEdit txt_Score;

	private PivotGridField field_Benmarking;

	private LabelLine labelLine2;

	private ButtonEditCFL txt_KPIsSet;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private LabelLine labelLine5;

	private LookUpEditEx txt_ViewType;

	private DateEditEx txt_FromDate;

	private DateEditEx txt_ToDate;

	private CheckEditEx check_showBenchmarking;

	private LabelLine labelLine6;

	private LabelLine labelLine7;

	private PivotGridField field_Branch;

	private PivotGridField field_Department;

	private PivotGridField field_Position;

	private PivotGridField field_Manager;

	private PivotGridField field_ManagerId;

	private PivotGridField field_JobTitle;

	private LabelLine labelLine8;

	private TextEditEx txt_KPIsSetName;

	private MemoExEdit txt_Remarks;

	private DateEditEx txt_period;

	private LabelLine labelLine1;

	public DataRow dataFromListPoint { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btn_Cancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.View:
			btn_Ok.Text = "OK";
			break;
		case Enums.FormViewModeType.Update:
			btn_Ok.Text = "Save";
			break;
		case Enums.FormViewModeType.Add:
			btn_Ok.Text = "Add";
			break;
		}
	}

	public frm_KPIsScoringDetail()
	{
		InitializeComponent();
		OnLoadAsync += frm_ListPointKPIs_Load;
	}

	private void LoadViewTypeSource()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Display", typeof(string));
			dataTable.Columns.Add("Value", typeof(string));
			dataTable.Rows.Add("Tuần", "W");
			dataTable.Rows.Add("Tháng", "M");
			dataTable.Rows.Add("Quý", "Q");
			dataTable.Rows.Add("Năm", "Y");
			txt_ViewType.Properties.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadScoringDetail(int absEntry, string setCode, DateTime period)
	{
		try
		{
			DataTable dataSource = await _httpClient.ListPointKPIsClient.GetDetailScoring(absEntry, setCode, period);
			pv_KPIsScoringDetail.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task frm_ListPointKPIs_Load(object sender, EventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			LoadViewTypeSource();
			if (dataFromListPoint != null && dataFromListPoint.Table.IsNotNull())
			{
				txt_KPIsSet.Properties.Buttons.Remove(txt_KPIsSet.Properties.Buttons[0]);
				txt_KPIsSet.EditValue = Function.ToString(dataFromListPoint["Code"]);
				txt_KPIsSet.Text = Function.ToString(dataFromListPoint["Code"]);
				await LoadData(dataFromListPoint);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ChangeCellValue(EditValueChangedEventArgs e, double oldValue, double newValue)
	{
		PivotDrillDownDataSource pivotDrillDownDataSource = e.CreateDrillDownDataSource();
		Function.ParseDouble(pivotDrillDownDataSource[0]["Benchmarking"]);
		if (Function.ToString(pivotDrillDownDataSource[0]["FormulaType"]) == "F")
		{
			WinCommons.Warning(Enums.MessageCode.None, "Ô này được chấm điểm tự động bởi công thức");
			return;
		}
		for (int i = 0; i < pivotDrillDownDataSource.RowCount; i++)
		{
			pivotDrillDownDataSource.SetValue(i, e.DataField, newValue);
		}
		FormMode = Enums.FormViewModeType.Update;
	}

	private void pv_KPIsScoringDetail_EditValueChanged(object sender, EditValueChangedEventArgs e)
	{
		try
		{
			ChangeCellValue(e, Function.ParseDouble(e.Value), Function.ParseDouble(e.Editor.EditValue));
			pv_KPIsScoringDetail.RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task SaveData()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			DataTable dataTable = pv_KPIsScoringDetail.DataSourceTable().Copy();
			dataTable.Columns.Add("UserSignScoring", typeof(string));
			dataTable.Columns.Add("Remarks", typeof(string));
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				dataTable.Rows[i]["UserSignScoring"] = Function.ToString(GlobalData.UserID);
				dataTable.Rows[i]["Remarks"] = Function.ToString(txt_Remarks.EditValue);
			}
			dataTable.AcceptChanges();
			string text = await _httpClient.ListPointKPIsClient.SaveDetailScoring(dataTable);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.None, "Success");
				FormMode = Enums.FormViewModeType.View;
				if (ReloadDataListPoint != null)
				{
					ReloadDataListPoint();
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void simpleButtonEx2_Click(object sender, EventArgs e)
	{
		try
		{
			await SaveData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadData(DataRow item)
	{
		try
		{
			if (item.Table.IsNotNull())
			{
				setCode = Function.ToString(item["Code"]);
				absEntry = Function.ParseInt(item["AbsEntry"]);
				txt_FromDate.EditValue = Function.ParseDateTimes(item["FromDate"]);
				txt_ToDate.EditValue = Function.ParseDateTimes(item["ToDate"]);
				txt_ViewType.EditValue = Function.ToString(item["ViewType"]);
				txt_KPIsSetName.EditValue = Function.ToString(item["Name"]);
				txt_Remarks.EditValue = Function.ToString(item["Remarks"]);
				txt_period.EditValue = Function.ParseDateTimes(item["Period"]);
				txt_period.Properties.DisplayFormat.FormatType = FormatType.Custom;
				txt_period.Properties.DisplayFormat.Format = new CustomFormatPeriod();
				switch (Function.ToString(item["ViewType"]))
				{
				case "M":
					txt_period.Properties.DisplayFormat.FormatString = "apz_month";
					break;
				case "Q":
					txt_period.Properties.DisplayFormat.FormatString = "apz_quarter";
					break;
				case "Y":
					txt_period.Properties.DisplayFormat.FormatString = "apz_year";
					break;
				case "W":
					txt_period.Properties.DisplayFormat.FormatString = "apz_week";
					break;
				}
				await LoadScoringDetail(absEntry, setCode, Function.ParseDateTimes(item["Period"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void buttonEditCFL1_Properties_SelectedFromCFL(object sender, ButtonEditCFLEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._OpenProcessing();
				pv_KPIsScoringDetail.DataSource = null;
				if (e.ChooseItem != null)
				{
					DataRow chooseItem = e.ChooseItem;
					await LoadData(chooseItem);
				}
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void CloseForm()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				FormMode = Enums.FormViewModeType.View;
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		try
		{
			CloseForm();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pv_KPIsScoringDetail_CustomAppearance(object sender, PivotCustomAppearanceEventArgs e)
	{
		try
		{
			if (e != null && e.RowField != null && e.ColumnField != null && (Function.ToString(e.CreateDrillDownDataSource()[0]["FormulaType"]) == "F" || e.DataField == field_Benmarking))
			{
				e.Appearance.BackColor = Color.FromArgb(204, 204, 204);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pv_KPIsScoringDetail_FieldValueDisplayText(object sender, PivotFieldDisplayTextEventArgs e)
	{
		try
		{
			if (e.ValueType == PivotGridValueType.GrandTotal && e.IsColumn)
			{
				if (e.DisplayText == "Grand Total")
				{
					e.DisplayText = "Tổng Điểm";
				}
				else if (e.DataField == field_Scoring)
				{
					e.DisplayText = "Chấm Điểm";
				}
				else if (e.DataField == field_Benmarking)
				{
					e.DisplayText = "Điểm Chuẩn";
				}
				else
				{
					e.DisplayText = "Tổng Điểm";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void checkEditEx1_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!isShowBenmarking)
			{
				check_showBenchmarking.EditValue = true;
				field_Benmarking.Visible = true;
				isShowBenmarking = true;
			}
			else
			{
				check_showBenchmarking.EditValue = false;
				field_Benmarking.Visible = false;
				isShowBenmarking = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pv_KPIsScoringDetail_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			Point focusedCell = pv_KPIsScoringDetail.Cells.FocusedCell;
			DataTable dataTable = pv_KPIsScoringDetail.CreateDrillDownDataSource(focusedCell.X, focusedCell.Y).ConvertPivotDrillDownDataSourceToDataTable();
			if (dataTable.IsNotNull())
			{
				double num = Function.ParseDouble(dataTable.Rows[0]["Benchmarking"]);
				if (Function.ParseDouble(e.Value) > num)
				{
					e.Valid = false;
					e.ErrorText = "Điểm chấm lớn hơn điểm chuẩn";
				}
				else
				{
					e.Valid = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_KPIsScoringDetail_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.Yes)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pv_KPIsScoringDetail_CustomCellEditForEditing(object sender, PivotCustomCellEditEventArgs e)
	{
		try
		{
			if (e == null || e.RowField == null || e.ColumnField == null)
			{
				if (e.RepositoryItem != null)
				{
					e.RepositoryItem.ReadOnly = true;
				}
				return;
			}
			string text = Function.ToString(e.CreateDrillDownDataSource()[0]["FormulaType"]);
			if (((!e.IsFieldValueExpanded(field_Branch) && field_Branch.Visible) || (!e.IsFieldValueExpanded(field_Department) && field_Department.Visible) || (!e.IsFieldValueExpanded(field_Manager) && field_Manager.Visible) || (!e.IsFieldValueExpanded(field_Position) && field_Position.Visible) || (!e.IsFieldValueExpanded(field_EmpName) && field_EmpName.Visible)) && e.RepositoryItem != null)
			{
				e.RepositoryItem.ReadOnly = true;
			}
			else if (text == "F" && e.RepositoryItem != null)
			{
				e.RepositoryItem.ReadOnly = true;
			}
			else if (e.RepositoryItem != null && e.ColumnValueType == PivotGridValueType.GrandTotal)
			{
				e.RepositoryItem.ReadOnly = true;
			}
			else if (e.RepositoryItem != null)
			{
				e.RepositoryItem.ReadOnly = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.KPIs.frm_KPIsScoringDetail));
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
		this.field_Branch = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_Department = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_ManagerId = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_Manager = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_Name = new DevExpress.XtraPivotGrid.PivotGridField();
		this.pv_KPIsScoringDetail = new Apzon.Controls.Pivot.PivotGridControlEx();
		this.field_Position = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_JobTitle = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_EmpName = new DevExpress.XtraPivotGrid.PivotGridField();
		this.field_Scoring = new DevExpress.XtraPivotGrid.PivotGridField();
		this.txt_Score = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.field_Benmarking = new DevExpress.XtraPivotGrid.PivotGridField();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txt_KPIsSet = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.txt_ViewType = new Apzon.Controls.Editor.LookUpEditEx();
		this.txt_FromDate = new Apzon.Controls.Editor.DateEditEx();
		this.txt_ToDate = new Apzon.Controls.Editor.DateEditEx();
		this.check_showBenchmarking = new Apzon.Controls.Editor.CheckEditEx();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.txt_KPIsSetName = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Remarks = new DevExpress.XtraEditors.MemoExEdit();
		this.txt_period = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.pv_KPIsScoringDetail).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Score).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_KPIsSet.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ViewType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.check_showBenchmarking.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_KPIsSetName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Remarks.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_period.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_period.Properties).BeginInit();
		base.SuspendLayout();
		this.field_Branch.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_Branch.AreaIndex = 0;
		this.field_Branch.Caption = "Chi Nhánh";
		this.field_Branch.FieldName = "Branch";
		this.field_Branch.Name = "field_Branch";
		this.field_Department.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_Department.AreaIndex = 1;
		this.field_Department.Caption = "Phòng Ban";
		this.field_Department.FieldName = "Department";
		this.field_Department.Name = "field_Department";
		this.field_ManagerId.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_ManagerId.AreaIndex = 2;
		this.field_ManagerId.FieldName = "ManagerId";
		this.field_ManagerId.Name = "field_ManagerId";
		this.field_ManagerId.Visible = false;
		this.field_Manager.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_Manager.AreaIndex = 2;
		this.field_Manager.Caption = "Quản Lý";
		this.field_Manager.FieldName = "Manager";
		this.field_Manager.Name = "field_Manager";
		this.field_Name.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
		this.field_Name.AreaIndex = 0;
		this.field_Name.Caption = "Tên";
		this.field_Name.FieldName = "KPI_Name";
		this.field_Name.Name = "field_Name";
		this.pv_KPIsScoringDetail.AllowPasteExcel = true;
		this.pv_KPIsScoringDetail.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pv_KPIsScoringDetail.ClipboardData = null;
		this.pv_KPIsScoringDetail.Fields.AddRange(new DevExpress.XtraPivotGrid.PivotGridField[10] { this.field_Branch, this.field_Department, this.field_ManagerId, this.field_Manager, this.field_Position, this.field_JobTitle, this.field_EmpName, this.field_Name, this.field_Scoring, this.field_Benmarking });
		this.pv_KPIsScoringDetail.FilterFieldControl = null;
		this.pv_KPIsScoringDetail.FilterValueControl = null;
		this.pv_KPIsScoringDetail.FormId = "oKPIsDetailScoring";
		this.pv_KPIsScoringDetail.ItemId = "APZ_D_KPR1";
		this.pv_KPIsScoringDetail.Location = new System.Drawing.Point(12, 60);
		this.pv_KPIsScoringDetail.Name = "pv_KPIsScoringDetail";
		this.pv_KPIsScoringDetail.ObjectType = Apzon.Commons.ApzObjectTypes.oNone;
		this.pv_KPIsScoringDetail.OptionsBehavior.CopyToClipboardWithFieldValues = true;
		this.pv_KPIsScoringDetail.OptionsView.ShowColumnHeaders = false;
		this.pv_KPIsScoringDetail.OptionsView.ShowFilterHeaders = false;
		this.pv_KPIsScoringDetail.OptionsView.ShowRowGrandTotalHeader = false;
		this.pv_KPIsScoringDetail.OptionsView.ShowRowGrandTotals = false;
		this.pv_KPIsScoringDetail.OptionsView.ShowRowTotals = false;
		this.pv_KPIsScoringDetail.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.txt_Score });
		this.pv_KPIsScoringDetail.Size = new System.Drawing.Size(1018, 460);
		this.pv_KPIsScoringDetail.TabIndex = 1;
		this.pv_KPIsScoringDetail.FieldValueDisplayText += new DevExpress.XtraPivotGrid.PivotFieldDisplayTextEventHandler(this.pv_KPIsScoringDetail_FieldValueDisplayText);
		this.pv_KPIsScoringDetail.CustomAppearance += new DevExpress.XtraPivotGrid.PivotCustomAppearanceEventHandler(this.pv_KPIsScoringDetail_CustomAppearance);
		this.pv_KPIsScoringDetail.EditValueChanged += new DevExpress.XtraPivotGrid.EditValueChangedEventHandler(this.pv_KPIsScoringDetail_EditValueChanged);
		this.pv_KPIsScoringDetail.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.pv_KPIsScoringDetail_ValidatingEditor);
		this.pv_KPIsScoringDetail.CustomCellEditForEditing += new System.EventHandler<DevExpress.XtraPivotGrid.PivotCustomCellEditEventArgs>(this.pv_KPIsScoringDetail_CustomCellEditForEditing);
		this.field_Position.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_Position.AreaIndex = 3;
		this.field_Position.Caption = "Vị Trí";
		this.field_Position.FieldName = "Position";
		this.field_Position.Name = "field_Position";
		this.field_JobTitle.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_JobTitle.AreaIndex = 5;
		this.field_JobTitle.Caption = "Tên Công Việc";
		this.field_JobTitle.FieldName = "JobTitle";
		this.field_JobTitle.Name = "field_JobTitle";
		this.field_EmpName.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.field_EmpName.AreaIndex = 4;
		this.field_EmpName.Caption = "Tên Nhân Viên";
		this.field_EmpName.FieldName = "U_fullName";
		this.field_EmpName.Name = "field_EmpName";
		this.field_Scoring.Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
		this.field_Scoring.AreaIndex = 0;
		this.field_Scoring.Caption = "Chấm Điểm";
		this.field_Scoring.CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.field_Scoring.FieldEdit = this.txt_Score;
		this.field_Scoring.FieldName = "Point";
		this.field_Scoring.Name = "field_Scoring";
		this.txt_Score.AutoHeight = false;
		this.txt_Score.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_Score.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_Score.Name = "txt_Score";
		this.field_Benmarking.Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
		this.field_Benmarking.AreaIndex = 1;
		this.field_Benmarking.Caption = "Điểm Chuẩn";
		this.field_Benmarking.CellFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.field_Benmarking.FieldName = "Benchmarking";
		this.field_Benmarking.Name = "field_Benmarking";
		this.field_Benmarking.Visible = false;
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(93, 526);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = true;
		this.btn_Ok.Location = new System.Drawing.Point(12, 526);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.Size = new System.Drawing.Size(75, 23);
		this.btn_Ok.TabIndex = 3;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.simpleButtonEx2_Click);
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(12, 7);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(85, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 5;
		this.labelLine2.Utext = "Bộ KPIs";
		this.txt_KPIsSet.FieldName = null;
		this.txt_KPIsSet.IsSearchControl = false;
		this.txt_KPIsSet.Location = new System.Drawing.Point(103, 6);
		this.txt_KPIsSet.Name = "txt_KPIsSet";
		this.txt_KPIsSet.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.txt_KPIsSet.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_KPIsSet.Properties.ContainerControl = null;
		this.txt_KPIsSet.Properties.DisplayMember = "AbsEntry";
		this.txt_KPIsSet.Properties.DisplayMember2 = "";
		this.txt_KPIsSet.Properties.LoadDataSourceWhenEnter = false;
		this.txt_KPIsSet.Properties.ReadOnly = true;
		this.txt_KPIsSet.Properties.SeparateCharacterForMulti = ';';
		this.txt_KPIsSet.Properties.SeparateString = ", ";
		this.txt_KPIsSet.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.KPIsSettings;
		this.txt_KPIsSet.Properties.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oSetIndicatorKPIs;
		this.txt_KPIsSet.Properties.UsingAutoCompleteBox = false;
		this.txt_KPIsSet.Properties.UsingDataSource = true;
		this.txt_KPIsSet.Properties.ValidateWhenNoDataSource = false;
		this.txt_KPIsSet.Properties.ValueMember = "AbsEntry";
		this.txt_KPIsSet.Properties.SelectedFromCFL += new System.EventHandler<Apzon.Forms.UserControls.ButtonEditCFLEventArgs>(this.buttonEditCFL1_Properties_SelectedFromCFL);
		this.txt_KPIsSet.Size = new System.Drawing.Size(130, 20);
		this.txt_KPIsSet.TabIndex = 7;
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(783, 7);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(109, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 8;
		this.labelLine3.Utext = "Kiểu xem";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(12, 32);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(85, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 8;
		this.labelLine4.Utext = "Từ ngày";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(276, 32);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(71, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 8;
		this.labelLine5.Utext = "Đến ngày";
		this.txt_ViewType.FieldName = null;
		this.txt_ViewType.IsSearchControl = false;
		this.txt_ViewType.Location = new System.Drawing.Point(898, 7);
		this.txt_ViewType.Name = "txt_ViewType";
		this.txt_ViewType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_ViewType.Properties.DisplayMember = "Display";
		this.txt_ViewType.Properties.NullText = "";
		this.txt_ViewType.Properties.ReadOnly = true;
		this.txt_ViewType.Properties.ValueMember = "Value";
		this.txt_ViewType.Size = new System.Drawing.Size(130, 20);
		this.txt_ViewType.TabIndex = 9;
		this.txt_FromDate.EditValue = null;
		this.txt_FromDate.FieldName = null;
		this.txt_FromDate.Location = new System.Drawing.Point(103, 32);
		this.txt_FromDate.Name = "txt_FromDate";
		this.txt_FromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_FromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_FromDate.Properties.IsSearchControl = false;
		this.txt_FromDate.Properties.ReadOnly = true;
		this.txt_FromDate.Size = new System.Drawing.Size(130, 20);
		this.txt_FromDate.TabIndex = 10;
		this.txt_ToDate.EditValue = null;
		this.txt_ToDate.FieldName = null;
		this.txt_ToDate.Location = new System.Drawing.Point(353, 31);
		this.txt_ToDate.Name = "txt_ToDate";
		this.txt_ToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_ToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_ToDate.Properties.IsSearchControl = false;
		this.txt_ToDate.Properties.ReadOnly = true;
		this.txt_ToDate.Size = new System.Drawing.Size(130, 20);
		this.txt_ToDate.TabIndex = 11;
		this.check_showBenchmarking.FieldName = null;
		this.check_showBenchmarking.IsSearchControl = false;
		this.check_showBenchmarking.Location = new System.Drawing.Point(898, 31);
		this.check_showBenchmarking.Name = "check_showBenchmarking";
		this.check_showBenchmarking.Properties.Caption = "";
		this.check_showBenchmarking.Size = new System.Drawing.Size(21, 20);
		this.check_showBenchmarking.TabIndex = 13;
		this.check_showBenchmarking.CheckedChanged += new System.EventHandler(this.checkEditEx1_CheckedChanged);
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(783, 32);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(109, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 14;
		this.labelLine6.Utext = "Hiển thị điểm chuẩn";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(529, 32);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(63, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 16;
		this.labelLine7.Utext = "Ghi chú";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(276, 7);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(71, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 18;
		this.labelLine8.Utext = "Tên bộ KPIs";
		this.txt_KPIsSetName.FieldName = null;
		this.txt_KPIsSetName.IsSearchControl = false;
		this.txt_KPIsSetName.Location = new System.Drawing.Point(353, 6);
		this.txt_KPIsSetName.Name = "txt_KPIsSetName";
		this.txt_KPIsSetName.Properties.ReadOnly = true;
		this.txt_KPIsSetName.Size = new System.Drawing.Size(130, 20);
		this.txt_KPIsSetName.TabIndex = 19;
		this.txt_Remarks.Location = new System.Drawing.Point(598, 32);
		this.txt_Remarks.Name = "txt_Remarks";
		this.txt_Remarks.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_Remarks.Properties.ShowIcon = false;
		this.txt_Remarks.Size = new System.Drawing.Size(130, 20);
		this.txt_Remarks.TabIndex = 20;
		this.txt_period.EditValue = null;
		this.txt_period.FieldName = null;
		this.txt_period.Location = new System.Drawing.Point(598, 6);
		this.txt_period.Name = "txt_period";
		this.txt_period.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_period.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_period.Properties.IsSearchControl = false;
		this.txt_period.Properties.ReadOnly = true;
		this.txt_period.Size = new System.Drawing.Size(130, 20);
		this.txt_period.TabIndex = 21;
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(529, 7);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(63, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 22;
		this.labelLine1.Utext = "Kỳ";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1042, 561);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.txt_period);
		base.Controls.Add(this.txt_Remarks);
		base.Controls.Add(this.txt_KPIsSetName);
		base.Controls.Add(this.labelLine8);
		base.Controls.Add(this.labelLine7);
		base.Controls.Add(this.labelLine6);
		base.Controls.Add(this.check_showBenchmarking);
		base.Controls.Add(this.txt_ToDate);
		base.Controls.Add(this.txt_FromDate);
		base.Controls.Add(this.txt_ViewType);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.txt_KPIsSet);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.pv_KPIsScoringDetail);
		base.FormId = "oKPIsDetailScoring";
		base.FormObjectType = Apzon.Commons.ApzObjectTypes.oKPIsDetailScoring;
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_KPIsScoringDetail";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Chi Tiết Chấm Điểm KPIs";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_KPIsScoringDetail_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.pv_KPIsScoringDetail).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Score).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_KPIsSet.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ViewType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.check_showBenchmarking.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_KPIsSetName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Remarks.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_period.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_period.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
