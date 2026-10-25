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
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraTab;

namespace Apzon.Forms.KPIs;

public class frm_KPIsSetting : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool isAdding;

	private bool _isChanging;

	private DataTable _dtKPISet;

	private DataTable _dtPoints;

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	private ContextMenuStrip ctx_Doc;

	private ToolStripMenuItem ctm_Add;

	private ToolStripMenuItem ctm_Duplicate;

	private ToolStripMenuItem ctm_Delete;

	private GridColumnEx gc_Code;

	private GridColumnEx gc_Name;

	private GridColumnEx gc_FromDate;

	private RepositoryItemDateEditEx repositoryItemDateEditEx1;

	private GridColumnEx gc_ToDate;

	private GridColumnEx gc_ViewType;

	private RepositoryItemLookUpEdit rep_ViewType;

	private GridColumnEx gc_Remark;

	private RepositoryItemMemoExEdit memo_Remark;

	private GridColumnEx gc_StatusCode;

	private RepositoryItemCheckEdit chk_StatusCode;

	private GridColumnEx gc_empID;

	private GridColumnEx gc_empName;

	private RepositoryItemButtonEditCFL cfl_KPIsMaster;

	private GridColumnEx gc_AbsEntry;

	private ContextMenuStrip ctx_Point;

	private ToolStripMenuItem ctm_DeleteEmp;

	private GridColumnEx gc_jobTitle;

	private GridColumnEx gc_PositionName;

	private GridColumnEx gc_Department;

	private GridColumnEx gc_BranchName;

	private GridColumnEx gc_ManagerName;

	private GridColumnEx gc_ExtEmpNo;

	private GridColumnEx gc_Period;

	private SplitContainerControl splitContainerControl2;

	private GroupControl groupControl1;

	private XtraTabControlEx xtraTabControlEx1;

	private XtraTabPageEx tab_Index;

	private GridControlEx grd_Points;

	private GridViewEx gv_Points;

	private RepositoryItemButtonEdit rep_CardCode;

	private RepositoryItemCheckEdit chk_Active;

	private GridControlEx grd_KPIs;

	private GridViewEx gv_KPIs;

	private RepositoryItemButtonEdit repositoryItemButtonEdit1;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private GridColumnEx gc_KPICode;

	private GridColumnEx gc_KPIName;

	private GridColumnEx gc_Condition;

	private GridColumnEx gc_FromValue;

	private GridColumnEx gc_ToValue;

	private GridColumnEx gc_Point;

	private GridColumnEx gc_ConditionName;

	private RepositoryItemCalcEdit calc_Point;

	private RepositoryItemButtonEditCFL rep_CFL;

	private GridColumnEx gc_DisplayCode;

	private GridColumnEx gc_Currency;

	private GridColumnEx gc_KpiType;

	private RepositoryItemLookUpEdit rep_KpiType;

	private RepositoryItemButtonEditCFL cfl_Condition;

	private RepositoryItemLookUpEdit luk_Period;

	private SimpleButton btn_CopyFormSetting;

	public int AbsEntry { get; set; }

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
		case Enums.FormViewModeType.Add:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			ctm_Add.Enabled = false;
			ctm_Duplicate.Enabled = false;
			ctm_Delete.Enabled = false;
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonSave;
			ctm_Add.Enabled = true;
			ctm_Duplicate.Enabled = true;
			ctm_Delete.Enabled = true;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			ctm_Add.Enabled = true;
			ctm_Duplicate.Enabled = true;
			ctm_Delete.Enabled = true;
			gv_Doc.Columns["Code"].OptionsColumn.ReadOnly = true;
			break;
		}
	}

	public frm_KPIsSetting()
	{
		InitializeComponent();
		OnLoadAsync += frm_KPIsSetting_Load;
	}

	private async Task frm_KPIsSetting_Load(object sender, EventArgs e)
	{
		try
		{
			LoadInit();
			calc_Point.DisplayFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.SumDec.GetValueOrDefault())}";
			ctm_Add.Text = Apzon.Commons.Properties.Resources.ctx_Add;
			ctm_Duplicate.Text = Apzon.Commons.Properties.Resources.ctx_Duplicate;
			ctm_Delete.Text = Apzon.Commons.Properties.Resources.ctx_Delete;
			grd_Doc.ContextMenuStrip = ctx_Doc;
			FormMode = Enums.FormViewModeType.View;
			DataSetResult dataSetResult = await _httpClient.KPIsSettingClient.GetKPIsSetting();
			if (!string.IsNullOrEmpty(dataSetResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.Error, dataSetResult.Message);
				return;
			}
			if (!dataSetResult.Data.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.Error, "Data is Empty!");
				return;
			}
			DataSet data = dataSetResult.Data;
			if (data.Tables.Count >= 1)
			{
				DataTable dataTable = data.Tables[0];
				EnumerableRowCollection<DataRow> enumerableRowCollection = from t in dataTable.AsEnumerable()
					where Function.ParseInt(t["AbsEntry"]) == AbsEntry
					select t;
				if (AbsEntry > 0 && enumerableRowCollection != null)
				{
					grd_Doc.DataSource = enumerableRowCollection.CopyToDataTable();
				}
				else
				{
					grd_Doc.DataSource = dataTable;
				}
				grd_Doc.DatasourceTable().AcceptChanges();
			}
			data.DisposeDataSet();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadInit()
	{
		try
		{
			Dictionary<string, string> dic = new Dictionary<string, string>
			{
				{ "W", "Tuân" },
				{ "M", "Tháng" },
				{ "Q", "Quý" },
				{ "Y", "Năm" }
			};
			rep_ViewType.SetDataSource(dic, hideCodeColumn: true);
			rep_CFL.ValueMember = "empID";
			rep_CFL.DisplayMember = "empID";
			rep_CFL.ContainerControl = this;
			rep_CFL.FindCFLWith(Enums.TypeCFL.Employees, true, "Apz_Cfl_EmployleeData", Enums.ItemObjectType.NoItemType, null, "");
			rep_CFL.MultiSelectMode = false;
			rep_CFL.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = rep_CFL;
			repositoryItemButtonEditCFL.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL.HandleChooseItem, new Action<DataRow>(FillData));
			rep_CFL.HandleChooseItemMulti = HandleChooseItemMulti;
			rep_CFL.RefreshDataSource();
			cfl_KPIsMaster.ValueMember = "Code";
			cfl_KPIsMaster.DisplayMember = "Code";
			cfl_KPIsMaster.ContainerControl = this;
			cfl_KPIsMaster.AutoLoadDataSource = true;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = cfl_KPIsMaster;
			repositoryItemButtonEditCFL2.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL2.HandleChooseItem, new Action<DataRow>(ChooseKPIs));
			cfl_KPIsMaster.RefreshDataSource();
			cfl_Condition.ValueMember = "Code";
			cfl_Condition.DisplayMember = "Code";
			cfl_Condition.ContainerControl = this;
			cfl_Condition.AutoLoadDataSource = true;
			cfl_Condition.FindCFLWith(Enums.TypeCFL.PromotionCondition, false, "", Enums.ItemObjectType.NoItemType, new List<Common.ParameterValue>
			{
				new Common.ParameterValue
				{
					Name = "ConditionType",
					Value = ""
				}
			});
			cfl_Condition.TypeDrillDown = ApzObjectTypes.oPromotionCondition;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL3 = cfl_Condition;
			repositoryItemButtonEditCFL3.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL3.HandleChooseItem, new Action<DataRow>(ChooseCondition));
			cfl_Condition.RefreshDataSource();
			rep_KpiType.SetDataSource(new Dictionary<string, string>
			{
				{ "Q", "Số lượng" },
				{ "C", "Tiền tệ" },
				{ "R", "%" }
			});
			luk_Period.DisplayMember = "PeriodName";
			luk_Period.ValueMember = "PeriodCode";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AfterShow(frm_CFL frm)
	{
		try
		{
			EnumerableRowCollection<DataRow> source = from t in grd_Points.DatasourceTable().AsEnumerable()
				where Function.ParseInt(t["EmpId"]) != 0
				select t;
			IEnumerable<object> selectedEmps = source.Select((DataRow t) => t["EmpId"]).Distinct();
			DataTable dataTable = frm.gv_CFL.GridControl.DataSource as DataTable;
			if (dataTable.Rows.Count > 0 && dataTable != null)
			{
				DataRow[] source2 = (from s in dataTable.AsEnumerable()
					where !selectedEmps.Contains(Function.ParseInt(s["EmpId"]))
					select s).ToArray();
				if (source2.Count() > 0)
				{
					frm.gv_CFL.GridControl.DataSource = source2.CopyToDataTable();
					frm.gv_CFL.ExpandAllGroups();
				}
				else
				{
					dataTable.Clear();
					dataTable.AcceptChanges();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ChooseKPIs(DataRow dataRow)
	{
		try
		{
			gv_Doc.SetFocusedRowCellValue("Code", Function.ToString(dataRow["Code"]));
			gv_Doc.SetFocusedRowCellValue("Name", Function.ToString(dataRow["Name"]));
			gv_Doc.SetFocusedRowCellValue("ViewType", Function.ToString(dataRow["ViewType"]));
			gv_Doc.SetFocusedRowCellValue("FromDate", DateTime.Now);
			gv_Doc.PostEditor();
			gv_Doc.RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ChooseCondition(DataRow dataRow)
	{
		try
		{
			gv_Points.SetFocusedRowCellValue("Condition_Code", Function.ToString(dataRow["Code"]));
			gv_Points.SetFocusedRowCellValue("Condition_Name", Function.ToString(dataRow["Name"]));
			gv_Points.PostEditor();
			gv_Points.RefreshData();
			grd_Points.RefreshDataSource();
			if ((grd_Points.DataSource as DataTable).AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["Condition_Code"]))) == null)
			{
				AddNewLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadSetKPI(string setCode)
	{
		try
		{
			DataTable dataTable = await _httpClient.SetIndicatorKPIsClient.GetDataById(setCode);
			List<DataRow> list = (from t in dataTable.AsEnumerable()
				where Function.ToString(t["FormulaType"]) == "F"
				select t).ToList();
			if (list.Count > 0)
			{
				_dtKPISet = list.CopyToDataTable();
			}
			else
			{
				_dtKPISet = dataTable.Clone();
			}
			grd_KPIs.DataSource = _dtKPISet.Copy();
			grd_KPIs.DataSourceTable().AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewLine()
	{
		try
		{
			object focusedRowCellValue = gv_KPIs.GetFocusedRowCellValue("KPI_Code");
			DataTable dataTable = grd_Points.DatasourceTable();
			DataRow dataRow = dataTable.NewRow();
			dataRow["KPI_Code"] = focusedRowCellValue;
			dataRow["Condition_Code"] = "";
			dataRow["FromValue"] = 0;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grd_Points.DataSource = dataTable;
			grd_Points.RefreshDataSource();
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
			if (grd_Doc.DataSource is DataTable dataTable && dataTable.Rows.Count > 0 && rows.Count > 0)
			{
				gv_Doc.SetFocusedRowCellValue("empID", Function.ParseInt(rows[0]["EmpId"]));
				gv_Doc.SetFocusedRowCellValue("empName", Function.ToString(rows[0]["FirstName"]) + ", " + Function.ToString(rows[0]["LastName"]));
				gv_Doc.SetFocusedRowCellValue("jobTitle", Function.ToString(rows[0]["jobTitle"]));
				gv_Doc.SetFocusedRowCellValue("PositionName", Function.ToString(rows[0]["PositionName"]));
				gv_Doc.SetFocusedRowCellValue("Department", Function.ToString(rows[0]["Department"]));
				gv_Doc.SetFocusedRowCellValue("BranchName", Function.ToString(rows[0]["BranchName"]));
				gv_Doc.SetFocusedRowCellValue("ManagerName", Function.ToString(rows[0]["ManagerName"]));
				gv_Doc.SetFocusedRowCellValue("ExtEmpNo", Function.ToString(rows[0]["ExtEmpNo"]));
				gv_Doc.PostEditor();
				gv_Doc.RefreshData();
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
			gv_Doc.SetFocusedRowCellValue("empID", Function.ParseInt(dataRow["EmpId"]));
			gv_Doc.SetFocusedRowCellValue("empName", Function.ToString(dataRow["FirstName"]) + ", " + Function.ToString(dataRow["LastName"]));
			gv_Doc.SetFocusedRowCellValue("jobTitle", Function.ToString(dataRow["jobTitle"]));
			gv_Doc.SetFocusedRowCellValue("PositionName", Function.ToString(dataRow["PositionName"]));
			gv_Doc.SetFocusedRowCellValue("Department", Function.ToString(dataRow["Department"]));
			gv_Doc.SetFocusedRowCellValue("BranchName", Function.ToString(dataRow["BranchName"]));
			gv_Doc.SetFocusedRowCellValue("ManagerName", Function.ToString(dataRow["ManagerName"]));
			gv_Doc.SetFocusedRowCellValue("ExtEmpNo", Function.ToString(dataRow["ExtEmpNo"]));
			gv_Doc.PostEditor();
			gv_Doc.RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private Tuple<Dictionary<DateTime, string>, DateTime> GetDatesByViewType(DateTime periodStart, DateTime periodEnd, string viewType)
	{
		Dictionary<DateTime, string> dictionary = new Dictionary<DateTime, string>();
		periodEnd = periodStart;
		DateTime item = periodEnd;
		try
		{
			string text = "";
			switch (viewType)
			{
			case "W":
				text = "dd/MM/yyyy";
				break;
			case "M":
				text = "MM/yyyy";
				break;
			case "Q":
				text = "dd/MM/yyyy";
				break;
			case "Y":
				text = "yyyy";
				break;
			}
			switch (viewType)
			{
			case "W":
			{
				int num = (int)(periodStart.DayOfWeek - 1);
				DateTime dateTime7 = ((num < 0) ? periodStart.AddDays(-(num + 7)) : periodStart.AddDays(-num));
				int num2 = 0 - periodEnd.DayOfWeek;
				DateTime dateTime8 = ((num2 < 0) ? periodEnd.AddDays(num2 + 7) : periodEnd.AddDays(num2));
				DateTime dateTime9 = dateTime7;
				while (dateTime9 <= dateTime8)
				{
					if (dateTime9.DayOfWeek == DayOfWeek.Monday)
					{
						dictionary.Add(dateTime9, dateTime9.ToString(text));
					}
					dateTime9 = dateTime9.AddDays(1.0);
				}
				item = dateTime8;
				break;
			}
			case "M":
			{
				DateTime dateTime4 = new DateTime(periodStart.Year, periodStart.Month, 1);
				DateTime dateTime5 = new DateTime(periodEnd.Year, periodEnd.Month, 1);
				DateTime dateTime6 = dateTime4;
				while (dateTime6 <= dateTime5)
				{
					if (dateTime6.Day == 1)
					{
						dictionary.Add(dateTime6, dateTime6.ToString(text));
					}
					dateTime6 = dateTime6.AddMonths(1);
				}
				item = new DateTime(periodEnd.Year, periodEnd.Month, DateTime.DaysInMonth(periodEnd.Year, periodEnd.Month));
				break;
			}
			case "Q":
			{
				int num3 = (periodStart.Month - 1) / 3 + 1;
				DateTime dateTime10 = new DateTime(periodStart.Year, (num3 - 1) * 3 + 1, 1);
				dateTime10.AddMonths(3).AddDays(-1.0);
				int num4 = (periodEnd.Month - 1) / 3 + 1;
				DateTime dateTime11 = new DateTime(periodEnd.Year, (num4 - 1) * 3 + 1, 1);
				DateTime dateTime12 = dateTime11.AddMonths(3).AddDays(-1.0);
				DateTime dateTime13 = dateTime10;
				while (dateTime13 <= dateTime12)
				{
					if (dateTime13.Day == 1)
					{
						dictionary.Add(dateTime13, "Q" + ((dateTime13.Month - 1) / 3 + 1) + "/" + dateTime13.Year);
					}
					dateTime13 = dateTime13.AddMonths(3);
				}
				item = new DateTime(periodEnd.Year, dateTime11.Month + 2, DateTime.DaysInMonth(periodEnd.Year, dateTime11.Month + 2));
				break;
			}
			case "Y":
			{
				DateTime dateTime = new DateTime(periodStart.Year, 1, 1);
				DateTime dateTime2 = new DateTime(periodEnd.Year, 1, 1);
				DateTime dateTime3 = dateTime;
				while (dateTime3 <= dateTime2)
				{
					if (dateTime3.Day == 1)
					{
						dictionary.Add(dateTime3, dateTime3.ToString(text));
					}
					dateTime3 = dateTime3.AddYears(1);
				}
				item = new DateTime(periodEnd.Year, 12, DateTime.DaysInMonth(periodEnd.Year, 12));
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new Tuple<Dictionary<DateTime, string>, DateTime>(dictionary, item);
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if ((FormMode != Enums.FormViewModeType.Update && FormMode != Enums.FormViewModeType.Add) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await frm_KPIsSetting_Load(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				MergeDataPoint();
				int absEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
				DataSet ds = new DataSet();
				DataTable dataTable = grd_Doc.DatasourceTable().Select("AbsEntry = " + absEntry).CopyToDataTable();
				if (dataTable.Rows.Count > 0)
				{
					DataRow dataRow = dataTable.Rows[0];
					if (string.IsNullOrEmpty(Function.ToString(dataRow["Code"])))
					{
						WinCommons.Error(Enums.MessageCode.None, "Vui lòng chọn bộ chỉ tiêu!");
						return;
					}
					if (string.IsNullOrEmpty(Function.ToString(dataRow["Period"])))
					{
						WinCommons.Error(Enums.MessageCode.None, "Vui lòng chọn kỳ!");
						return;
					}
					if (string.IsNullOrEmpty(Function.ToString(dataRow["empID"])))
					{
						WinCommons.Error(Enums.MessageCode.None, "Vui lòng chọn nhân viên!");
						return;
					}
				}
				dataTable.TableName = "dtHeader";
				DataTable dataTable2 = _dtPoints.Copy();
				dataTable2.TableName = "dtPoints";
				ds.Tables.AddRange(new DataTable[2] { dataTable, dataTable2 });
				HttpResult httpResult = new HttpResult();
				switch (FormMode)
				{
				case Enums.FormViewModeType.Add:
					httpResult = await _httpClient.KPIsSettingClient.Add(ds);
					break;
				case Enums.FormViewModeType.Update:
					dataTable.Rows[0]["UserSign2"] = GlobalData.UserID;
					httpResult = await _httpClient.KPIsSettingClient.Update(ds);
					break;
				}
				ds.DisposeDataSet();
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.Success, Apzon.Commons.Properties.Resources.Msg_Success + httpResult.Content);
					FormMode = Enums.FormViewModeType.View;
					await frm_KPIsSetting_Load(null, null);
					DataRow[] array = grd_Doc.DatasourceTable().Select("AbsEntry = '" + absEntry + "'");
					if (array != null && array.Length != 0)
					{
						int dataSourceIndex = grd_Doc.DatasourceTable().Rows.IndexOf(array[0]);
						gv_Doc.FocusedRowHandle = gv_Doc.GetRowHandle(dataSourceIndex);
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public new async Task ReloadSettingForm()
	{
		try
		{
			await REFRESH_DATA();
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

	private async Task GetKPISettingDetail(int absEntry)
	{
		_ = 1;
		try
		{
			if (absEntry < 0)
			{
				return;
			}
			WinCommons._OpenProcessing();
			splitContainerControl1.PanelVisibility = SplitPanelVisibility.Both;
			DataSetResult dataSetResult = await _httpClient.KPIsSettingClient.GetDetail(absEntry);
			if (!string.IsNullOrEmpty(dataSetResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.Error, dataSetResult.Message);
				return;
			}
			if (!dataSetResult.Data.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.Error, "Data is Empty!");
				return;
			}
			DataSet data = dataSetResult.Data;
			_dtPoints = data.Tables[0];
			if (absEntry > 0)
			{
				Function.ToString(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "ViewType"));
				string setCode = Function.ToString(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "Code"));
				await LoadSetKPI(setCode);
			}
			else
			{
				grd_KPIs.DataSourceTable().Clear();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		if (isAdding)
		{
			return;
		}
		try
		{
			int absEntry = Function.ParseInt(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "AbsEntry"));
			await GetKPISettingDetail(absEntry);
			if (FormMode == Enums.FormViewModeType.Update)
			{
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadIndexPivot(DateTime periodStart, DateTime periodEnd, string type)
	{
		try
		{
			DataTable dataTable = grd_KPIs.DataSourceTable();
			DataTable source = dataTable.Copy();
			dataTable.Rows.Clear();
			DataTable dataTable2 = dataTable.Clone();
			foreach (KeyValuePair<DateTime, string> entry in GetDatesByViewType(periodStart, periodEnd, type).Item1)
			{
				if (!_dtKPISet.IsNotNull())
				{
					continue;
				}
				for (int i = 0; i < _dtKPISet.Rows.Count; i++)
				{
					DataRow kpi = _dtKPISet.Rows[i];
					DataRow dataRow = (from t in source.AsEnumerable()
						where Function.ToString(t["KPI_Code"]) == Function.ToString(kpi["KPI_Code"]) && Function.ParseDateTimes(t["DDate"]) == entry.Key
						select t).FirstOrDefault();
					double num = 0.0;
					if (dataRow != null)
					{
						num = Function.ParseDouble(dataRow["Point"]);
					}
					DataRow dataRow2 = dataTable2.NewRow();
					dataRow2["AbsEntry"] = 0;
					dataRow2["KPI_Code"] = kpi["KPI_Code"];
					dataRow2["Point"] = num;
					dataRow2["DDate"] = entry.Key;
					dataRow2["Name"] = kpi["Name"];
					dataRow2["DisplayCode"] = kpi["DisplayCode"];
					dataRow2["Currency"] = kpi["Currency"];
					dataRow2["KpiType"] = kpi["KpiType"];
					dataTable2.Rows.Add(dataRow2);
				}
			}
			grd_KPIs.DataSource = dataTable2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_isChanging)
			{
				if (e.Column.FieldName.Equals("Code"))
				{
					string setCode = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "Code"));
					await LoadSetKPI(setCode);
				}
				if (e.Column.FieldName.Equals("FromDate"))
				{
					_isChanging = true;
					string viewType = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "ViewType"));
					DateTime periodStart = Function.ParseDateTimes(gv_Doc.GetRowCellValue(e.RowHandle, "FromDate"));
					DateTime periodEnd = Function.ParseDateTimes(gv_Doc.GetRowCellValue(e.RowHandle, "ToDate"));
					Tuple<Dictionary<DateTime, string>, DateTime> datesByViewType = GetDatesByViewType(periodStart, periodEnd, viewType);
					gv_Doc.SetRowCellValue(e.RowHandle, "Period", Function.ParseDateTimes(datesByViewType.Item1.First().Key));
					gv_Doc.SetRowCellValue(e.RowHandle, "FromDate", Function.ParseDateTimes(datesByViewType.Item1.First().Key));
					gv_Doc.SetRowCellValue(e.RowHandle, "ToDate", Function.ParseDateTimes(datesByViewType.Item2));
					_isChanging = false;
				}
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (!isAdding && (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.Add))
			{
				bool flag = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
				if (flag)
				{
					grd_Doc.DatasourceTable().RejectChanges();
					grd_Points.DatasourceTable().RejectChanges();
					grd_KPIs.DataSourceTable().RejectChanges();
				}
				e.Allow = flag;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_KPIsSetting_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.Add)
		{
			e.Cancel = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes;
		}
	}

	private void ctm_Add_Click(object sender, EventArgs e)
	{
		REFRESH();
	}

	public async Task REFRESH()
	{
		try
		{
			if ((FormMode != Enums.FormViewModeType.Update && FormMode != Enums.FormViewModeType.Add) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				FormMode = Enums.FormViewModeType.View;
				DataTable dataTable = grd_Doc.DatasourceTable();
				dataTable.Rows.Clear();
				DataRow dataRow = dataTable.NewRow();
				dataRow["StatusCode"] = "Y";
				dataRow["UserSign"] = GlobalData.UserID;
				dataRow["UserSign2"] = GlobalData.UserID;
				dataRow["AbsEntry"] = 0;
				dataTable.Rows.Add(dataRow);
				gv_Doc.Columns["Code"].OptionsColumn.ReadOnly = false;
				FormMode = Enums.FormViewModeType.Add;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ctm_Duplicate_Click(object sender, EventArgs e)
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.Add) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			isAdding = true;
			DataRow focusedDataRow = gv_Doc.GetFocusedDataRow();
			if (focusedDataRow != null)
			{
				DataTable dataTable = grd_Doc.DatasourceTable();
				DataRow dataRow = dataTable.NewRow();
				dataRow.ItemArray = focusedDataRow.ItemArray;
				dataRow["UserSign"] = GlobalData.UserID;
				dataRow["AbsEntry"] = 0;
				dataTable.Rows.Clear();
				dataTable.Rows.Add(dataRow);
				gv_Doc.Columns["AbsEntry"].OptionsColumn.ReadOnly = true;
				gv_Doc.Columns["Code"].OptionsColumn.ReadOnly = false;
				FormMode = Enums.FormViewModeType.Add;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		isAdding = false;
	}

	private async void ctm_Delete_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			if (num > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				DataSet dataSet = new DataSet();
				DataTable dataTable = grd_Doc.DatasourceTable().Select("AbsEntry = '" + num + "'").CopyToDataTable();
				dataTable.TableName = "dtHeader";
				dataSet.Tables.Add(dataTable);
				new HttpResult();
				HttpResult httpResult = await _httpClient.KPIsSettingClient.Delete(dataSet);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.None, Apzon.Commons.Properties.Resources.Msg_Success + httpResult.Content);
					await frm_KPIsSetting_Load(null, null);
					FormMode = Enums.FormViewModeType.View;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ChangeCellValue(EditValueChangedEventArgs e, double oldValue, double newValue)
	{
		try
		{
			PivotDrillDownDataSource pivotDrillDownDataSource = e.CreateDrillDownDataSource();
			for (int i = 0; i < pivotDrillDownDataSource.RowCount; i++)
			{
				pivotDrillDownDataSource.SetValue(i, e.DataField, newValue);
			}
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

	private void grd_KPIs_EditValueChanged(object sender, EditValueChangedEventArgs e)
	{
		ChangeCellValue(e, Function.ParseDouble(e.Value), Function.ParseDouble(e.Editor.EditValue));
	}

	private void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
	}

	private void gv_Doc_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "Period" || _isChanging)
			{
				return;
			}
			string viewType = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "ViewType"));
			DateTime periodStart = Function.ParseDateTimes(gv_Doc.GetRowCellValue(e.RowHandle, "FromDate"));
			DateTime periodEnd = Function.ParseDateTimes(gv_Doc.GetRowCellValue(e.RowHandle, "ToDate"));
			Tuple<Dictionary<DateTime, string>, DateTime> datesByViewType = GetDatesByViewType(periodStart, periodEnd, viewType);
			DataTable dataTable = new DataTable
			{
				Columns = 
				{
					{
						"PeriodCode",
						typeof(DateTime)
					},
					{
						"PeriodName",
						typeof(string)
					}
				}
			};
			foreach (KeyValuePair<DateTime, string> item in datesByViewType.Item1)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["PeriodCode"] = item.Key;
				dataRow["PeriodName"] = item.Value;
				dataTable.Rows.Add(dataRow);
			}
			dataTable.AcceptChanges();
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit
			{
				NullText = "",
				DisplayMember = "PeriodName",
				ValueMember = "PeriodCode"
			};
			repositoryItemLookUpEdit.ShowHeader = false;
			repositoryItemLookUpEdit.DataSource = dataTable;
			repositoryItemLookUpEdit.PopulateColumns();
			repositoryItemLookUpEdit.Columns["PeriodCode"].Visible = false;
			e.RepositoryItem = repositoryItemLookUpEdit;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "Period")
			{
				return;
			}
			string viewType = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "ViewType"));
			DateTime periodStart = Function.ParseDateTimes(gv_Doc.GetRowCellValue(e.RowHandle, "FromDate"));
			DateTime periodEnd = Function.ParseDateTimes(gv_Doc.GetRowCellValue(e.RowHandle, "ToDate"));
			Tuple<Dictionary<DateTime, string>, DateTime> datesByViewType = GetDatesByViewType(periodStart, periodEnd, viewType);
			DataTable dataTable = new DataTable
			{
				Columns = 
				{
					{
						"PeriodCode",
						typeof(DateTime)
					},
					{
						"PeriodName",
						typeof(string)
					}
				}
			};
			foreach (KeyValuePair<DateTime, string> item in datesByViewType.Item1)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["PeriodCode"] = item.Key;
				dataRow["PeriodName"] = item.Value;
				dataTable.Rows.Add(dataRow);
			}
			dataTable.AcceptChanges();
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit
			{
				NullText = "",
				DisplayMember = "PeriodName",
				ValueMember = "PeriodCode"
			};
			repositoryItemLookUpEdit.ShowHeader = false;
			repositoryItemLookUpEdit.DataSource = dataTable;
			repositoryItemLookUpEdit.PopulateColumns();
			repositoryItemLookUpEdit.Columns["PeriodCode"].Visible = false;
			e.RepositoryItem = repositoryItemLookUpEdit;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_KPIs_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			string code = Function.ToString(gv_KPIs.GetRowCellValue(e.FocusedRowHandle, "KPI_Code"));
			List<DataRow> list = (from t in _dtPoints.Copy().AsEnumerable()
				where Function.ToString(t["KPI_Code"]) == code
				select t).ToList();
			if (list != null && list.Count > 0)
			{
				grd_Points.DataSource = list.CopyToDataTable();
			}
			else
			{
				grd_Points.DataSource = _dtPoints.Clone();
			}
			if (!string.IsNullOrEmpty(code) && (grd_Points.DataSource as DataTable).AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["Condition_Code"]))) == null)
			{
				AddNewLine();
			}
			grd_Points.DatasourceTable().AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void MergeDataPoint()
	{
		try
		{
			List<DataRow> list = (from t in grd_Points.DatasourceTable().Copy().AsEnumerable()
				where !string.IsNullOrEmpty(Function.ToString(t["Condition_Code"])) && !string.IsNullOrEmpty(Function.ToString(t["Point"])) && !string.IsNullOrEmpty(Function.ToString(t["FromValue"]))
				select t).ToList();
			if (list.Count > 0)
			{
				_dtPoints.SetPrimaryKey("KPI_Code", "Condition_Code", "FromValue");
				_dtPoints.Merge(list.CopyToDataTable());
			}
			_dtPoints.PrimaryKey = null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_KPIs_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			string kpiCode = Function.ToString(gv_KPIs.GetRowCellValue(e.RowHandle, "KPI_Code"));
			List<DataRow> list = (from t in _dtPoints.AsEnumerable()
				where Function.ToString(t["KPI_Code"]) != kpiCode
				select t).ToList();
			if (list.Count > 0)
			{
				_dtPoints = list.CopyToDataTable();
			}
			else
			{
				_dtPoints = _dtPoints.Clone();
			}
			MergeDataPoint();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Points_CellValueChanged(object sender, CellValueChangedEventArgs e)
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

	private void btn_CopyFormSetting_Click(object sender, EventArgs e)
	{
		try
		{
			frm_KPIsSetting_Copy frm_KPIsSetting_Copy2 = new frm_KPIsSetting_Copy();
			frm_KPIsSetting_Copy2.AbsEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			frm_KPIsSetting_Copy2.ShowDialog();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ctm_DeleteEmp_Click(object sender, EventArgs e)
	{
		try
		{
			int[] selectedRows = gv_Points.GetSelectedRows();
			DataTable dataSourceTable = grd_Points.DataSourceTable;
			gv_Points.BeginUpdate();
			for (int i = 0; i < selectedRows.Length; i++)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataSourceTable.Rows[gv_Points.GetDataSourceRowIndex(selectedRows[i])]["Condition_Code"])))
				{
					gv_Points.UnselectRow(selectedRows[i]);
				}
			}
			selectedRows = gv_Points.GetSelectedRows();
			bool flag = false;
			if (selectedRows.Length != 0)
			{
				flag = true;
			}
			gv_Points.DeleteSelectedRows();
			gv_Points.EndUpdate();
			dataSourceTable.AcceptChanges();
			if ((FormMode == Enums.FormViewModeType.View) & flag)
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.KPIs.frm_KPIsSetting));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctx_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.ctm_Add = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_Duplicate = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_Delete = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gc_AbsEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Code = new Apzon.Controls.Grid.GridColumnEx();
		this.cfl_KPIsMaster = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_Name = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_FromDate = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx1 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gc_ToDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Period = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ViewType = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ViewType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Remark = new Apzon.Controls.Grid.GridColumnEx();
		this.memo_Remark = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gc_StatusCode = new Apzon.Controls.Grid.GridColumnEx();
		this.chk_StatusCode = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gc_empID = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CFL = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_empName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_jobTitle = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_PositionName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Department = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_BranchName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ManagerName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ExtEmpNo = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Period = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.grd_KPIs = new Apzon.Controls.Grid.GridControlEx();
		this.gv_KPIs = new Apzon.Controls.Grid.GridViewEx();
		this.gc_KPICode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_KPIName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_DisplayCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Currency = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_KpiType = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_KpiType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.repositoryItemButtonEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.xtraTabControlEx1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_Index = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Points = new Apzon.Controls.Grid.GridControlEx();
		this.ctx_Point = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.ctm_DeleteEmp = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Points = new Apzon.Controls.Grid.GridViewEx();
		this.gc_Condition = new Apzon.Controls.Grid.GridColumnEx();
		this.cfl_Condition = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_ConditionName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_FromValue = new Apzon.Controls.Grid.GridColumnEx();
		this.calc_Point = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gc_ToValue = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Point = new Apzon.Controls.Grid.GridColumnEx();
		this.chk_Active = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.rep_CardCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_CopyFormSetting = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctx_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_KPIsMaster).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ViewType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Remark).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_StatusCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Period).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_KPIs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_KPIs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_KpiType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).BeginInit();
		this.xtraTabControlEx1.SuspendLayout();
		this.tab_Index.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Points).BeginInit();
		this.ctx_Point.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Points).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_Condition).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_Point).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Active).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CardCode).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, 3);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.splitContainerControl2);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1131, 473);
		this.splitContainerControl1.SplitterPosition = 174;
		this.splitContainerControl1.TabIndex = 9;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctx_Doc;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.repositoryItemDateEditEx1, this.rep_ViewType, this.memo_Remark, this.chk_StatusCode, this.cfl_KPIsMaster, this.rep_CFL, this.luk_Period });
		this.grd_Doc.Size = new System.Drawing.Size(1131, 174);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctx_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.ctm_Add, this.ctm_Duplicate, this.ctm_Delete });
		this.ctx_Doc.Name = "ctx_Doc";
		this.ctx_Doc.Size = new System.Drawing.Size(125, 70);
		this.ctm_Add.Name = "ctm_Add";
		this.ctm_Add.Size = new System.Drawing.Size(124, 22);
		this.ctm_Add.Text = "Add New";
		this.ctm_Add.Click += new System.EventHandler(this.ctm_Add_Click);
		this.ctm_Duplicate.Name = "ctm_Duplicate";
		this.ctm_Duplicate.Size = new System.Drawing.Size(124, 22);
		this.ctm_Duplicate.Text = "Duplicate";
		this.ctm_Duplicate.Click += new System.EventHandler(this.ctm_Duplicate_Click);
		this.ctm_Delete.Name = "ctm_Delete";
		this.ctm_Delete.Size = new System.Drawing.Size(124, 22);
		this.ctm_Delete.Text = "Delete";
		this.ctm_Delete.Click += new System.EventHandler(this.ctm_Delete_Click);
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clKPIsSetting;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[17]
		{
			this.gc_AbsEntry, this.gc_Code, this.gc_Name, this.gc_FromDate, this.gc_ToDate, this.gc_Period, this.gc_ViewType, this.gc_Remark, this.gc_StatusCode, this.gc_empID,
			this.gc_empName, this.gc_jobTitle, this.gc_PositionName, this.gc_Department, this.gc_BranchName, this.gc_ManagerName, this.gc_ExtEmpNo
		});
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oKPIsSetting";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = true;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_D_OKPM";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Doc.OptionsPrint.AutoWidth = false;
		this.gv_Doc.OptionsPrint.UsePrintStyles = false;
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupedColumns = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_Doc_CustomRowCellEdit);
		this.gv_Doc.CustomRowCellEditForEditing += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_Doc_CustomRowCellEditForEditing);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Doc_ShowingEditor);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.gc_AbsEntry.Caption = "ID";
		this.gc_AbsEntry.CurrencyDateField = null;
		this.gc_AbsEntry.CurrencyField = null;
		this.gc_AbsEntry.CurrencyFixed = null;
		this.gc_AbsEntry.CurrencyFixedRate = 0.0;
		this.gc_AbsEntry.CurrencyRateField = null;
		this.gc_AbsEntry.FieldName = "AbsEntry";
		this.gc_AbsEntry.Name = "gc_AbsEntry";
		this.gc_AbsEntry.OptionsColumn.ReadOnly = true;
		this.gc_AbsEntry.PrimaryKey = true;
		this.gc_AbsEntry.UnitField = null;
		this.gc_Code.Caption = "Mã bộ chỉ tiêu";
		this.gc_Code.ColumnEdit = this.cfl_KPIsMaster;
		this.gc_Code.CurrencyDateField = null;
		this.gc_Code.CurrencyField = null;
		this.gc_Code.CurrencyFixed = null;
		this.gc_Code.CurrencyFixedRate = 0.0;
		this.gc_Code.CurrencyRateField = null;
		this.gc_Code.FieldName = "Code";
		this.gc_Code.Name = "gc_Code";
		this.gc_Code.PrimaryKey = false;
		this.gc_Code.UnitField = null;
		this.gc_Code.Visible = true;
		this.gc_Code.VisibleIndex = 0;
		this.cfl_KPIsMaster.AutoHeight = false;
		this.cfl_KPIsMaster.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.cfl_KPIsMaster.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_KPIsMaster.ContainerControl = this;
		this.cfl_KPIsMaster.DisplayMember = "";
		this.cfl_KPIsMaster.DisplayMember2 = "";
		this.cfl_KPIsMaster.LoadDataSourceWhenEnter = false;
		this.cfl_KPIsMaster.Name = "cfl_KPIsMaster";
		this.cfl_KPIsMaster.SeparateCharacterForMulti = ';';
		this.cfl_KPIsMaster.SeparateString = ", ";
		this.cfl_KPIsMaster.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.KPIsMaster;
		this.cfl_KPIsMaster.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oSetIndicatorKPIs;
		this.cfl_KPIsMaster.UsingAutoCompleteBox = false;
		this.cfl_KPIsMaster.UsingDataSource = true;
		this.cfl_KPIsMaster.ValidateWhenNoDataSource = false;
		this.cfl_KPIsMaster.ValueMember = "";
		this.gc_Name.Caption = "Tên bộ chỉ tiêu";
		this.gc_Name.CurrencyDateField = null;
		this.gc_Name.CurrencyField = null;
		this.gc_Name.CurrencyFixed = null;
		this.gc_Name.CurrencyFixedRate = 0.0;
		this.gc_Name.CurrencyRateField = null;
		this.gc_Name.FieldName = "Name";
		this.gc_Name.Name = "gc_Name";
		this.gc_Name.OptionsColumn.ReadOnly = true;
		this.gc_Name.PrimaryKey = false;
		this.gc_Name.UnitField = null;
		this.gc_Name.Visible = true;
		this.gc_Name.VisibleIndex = 1;
		this.gc_FromDate.Caption = "Từ ngày";
		this.gc_FromDate.ColumnEdit = this.repositoryItemDateEditEx1;
		this.gc_FromDate.CurrencyDateField = null;
		this.gc_FromDate.CurrencyField = null;
		this.gc_FromDate.CurrencyFixed = null;
		this.gc_FromDate.CurrencyFixedRate = 0.0;
		this.gc_FromDate.CurrencyRateField = null;
		this.gc_FromDate.FieldName = "FromDate";
		this.gc_FromDate.Name = "gc_FromDate";
		this.gc_FromDate.PrimaryKey = false;
		this.gc_FromDate.UnitField = null;
		this.gc_FromDate.Visible = true;
		this.gc_FromDate.VisibleIndex = 3;
		this.repositoryItemDateEditEx1.AutoHeight = false;
		this.repositoryItemDateEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx1.EditFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx1.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx1.IsSearchControl = false;
		this.repositoryItemDateEditEx1.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx1.Name = "repositoryItemDateEditEx1";
		this.gc_ToDate.Caption = "Đến ngày";
		this.gc_ToDate.ColumnEdit = this.repositoryItemDateEditEx1;
		this.gc_ToDate.CurrencyDateField = null;
		this.gc_ToDate.CurrencyField = null;
		this.gc_ToDate.CurrencyFixed = null;
		this.gc_ToDate.CurrencyFixedRate = 0.0;
		this.gc_ToDate.CurrencyRateField = null;
		this.gc_ToDate.FieldName = "ToDate";
		this.gc_ToDate.Name = "gc_ToDate";
		this.gc_ToDate.OptionsColumn.ReadOnly = true;
		this.gc_ToDate.PrimaryKey = false;
		this.gc_ToDate.UnitField = null;
		this.gc_ToDate.Visible = true;
		this.gc_ToDate.VisibleIndex = 4;
		this.gc_Period.Caption = "Kỳ";
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
		this.gc_Period.VisibleIndex = 6;
		this.gc_ViewType.Caption = "Kiểu xem";
		this.gc_ViewType.ColumnEdit = this.rep_ViewType;
		this.gc_ViewType.CurrencyDateField = null;
		this.gc_ViewType.CurrencyField = null;
		this.gc_ViewType.CurrencyFixed = null;
		this.gc_ViewType.CurrencyFixedRate = 0.0;
		this.gc_ViewType.CurrencyRateField = null;
		this.gc_ViewType.FieldName = "ViewType";
		this.gc_ViewType.Name = "gc_ViewType";
		this.gc_ViewType.OptionsColumn.ReadOnly = true;
		this.gc_ViewType.PrimaryKey = false;
		this.gc_ViewType.UnitField = null;
		this.gc_ViewType.Visible = true;
		this.gc_ViewType.VisibleIndex = 2;
		this.rep_ViewType.AutoHeight = false;
		this.rep_ViewType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ViewType.Name = "rep_ViewType";
		this.rep_ViewType.NullText = "";
		this.gc_Remark.Caption = "Ghi chú";
		this.gc_Remark.ColumnEdit = this.memo_Remark;
		this.gc_Remark.CurrencyDateField = null;
		this.gc_Remark.CurrencyField = null;
		this.gc_Remark.CurrencyFixed = null;
		this.gc_Remark.CurrencyFixedRate = 0.0;
		this.gc_Remark.CurrencyRateField = null;
		this.gc_Remark.FieldName = "Remarks";
		this.gc_Remark.Name = "gc_Remark";
		this.gc_Remark.PrimaryKey = false;
		this.gc_Remark.UnitField = null;
		this.gc_Remark.Visible = true;
		this.gc_Remark.VisibleIndex = 7;
		this.memo_Remark.AutoHeight = false;
		this.memo_Remark.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.memo_Remark.Name = "memo_Remark";
		this.memo_Remark.ShowIcon = false;
		this.gc_StatusCode.Caption = "Kích hoạt";
		this.gc_StatusCode.ColumnEdit = this.chk_StatusCode;
		this.gc_StatusCode.CurrencyDateField = null;
		this.gc_StatusCode.CurrencyField = null;
		this.gc_StatusCode.CurrencyFixed = null;
		this.gc_StatusCode.CurrencyFixedRate = 0.0;
		this.gc_StatusCode.CurrencyRateField = null;
		this.gc_StatusCode.FieldName = "StatusCode";
		this.gc_StatusCode.Name = "gc_StatusCode";
		this.gc_StatusCode.PrimaryKey = false;
		this.gc_StatusCode.UnitField = null;
		this.gc_StatusCode.Visible = true;
		this.gc_StatusCode.VisibleIndex = 5;
		this.chk_StatusCode.AutoHeight = false;
		this.chk_StatusCode.Name = "chk_StatusCode";
		this.chk_StatusCode.ValueChecked = "Y";
		this.chk_StatusCode.ValueUnchecked = "N";
		this.gc_empID.Caption = "Mã nhân viên";
		this.gc_empID.ColumnEdit = this.rep_CFL;
		this.gc_empID.CurrencyDateField = null;
		this.gc_empID.CurrencyField = null;
		this.gc_empID.CurrencyFixed = null;
		this.gc_empID.CurrencyFixedRate = 0.0;
		this.gc_empID.CurrencyRateField = null;
		this.gc_empID.FieldName = "empID";
		this.gc_empID.Name = "gc_empID";
		this.gc_empID.PrimaryKey = false;
		this.gc_empID.UnitField = null;
		this.gc_empID.Visible = true;
		this.gc_empID.VisibleIndex = 8;
		this.rep_CFL.AutoHeight = false;
		this.rep_CFL.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_CFL.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_CFL.ContainerControl = this;
		this.rep_CFL.DisplayMember = "";
		this.rep_CFL.DisplayMember2 = "";
		this.rep_CFL.LoadDataSourceWhenEnter = false;
		this.rep_CFL.Name = "rep_CFL";
		this.rep_CFL.SeparateCharacterForMulti = ';';
		this.rep_CFL.SeparateString = ", ";
		this.rep_CFL.UsingAutoCompleteBox = false;
		this.rep_CFL.UsingDataSource = true;
		this.rep_CFL.ValidateWhenNoDataSource = false;
		this.rep_CFL.ValueMember = "";
		this.gc_empName.Caption = "Tên nhân viên";
		this.gc_empName.CurrencyDateField = null;
		this.gc_empName.CurrencyField = null;
		this.gc_empName.CurrencyFixed = null;
		this.gc_empName.CurrencyFixedRate = 0.0;
		this.gc_empName.CurrencyRateField = null;
		this.gc_empName.FieldName = "empName";
		this.gc_empName.Name = "gc_empName";
		this.gc_empName.OptionsColumn.ReadOnly = true;
		this.gc_empName.PrimaryKey = false;
		this.gc_empName.UnitField = null;
		this.gc_empName.Visible = true;
		this.gc_empName.VisibleIndex = 9;
		this.gc_jobTitle.Caption = "Chức danh";
		this.gc_jobTitle.CurrencyDateField = null;
		this.gc_jobTitle.CurrencyField = null;
		this.gc_jobTitle.CurrencyFixed = null;
		this.gc_jobTitle.CurrencyFixedRate = 0.0;
		this.gc_jobTitle.CurrencyRateField = null;
		this.gc_jobTitle.FieldName = "jobTitle";
		this.gc_jobTitle.Name = "gc_jobTitle";
		this.gc_jobTitle.OptionsColumn.ReadOnly = true;
		this.gc_jobTitle.PrimaryKey = false;
		this.gc_jobTitle.UnitField = null;
		this.gc_jobTitle.Visible = true;
		this.gc_jobTitle.VisibleIndex = 10;
		this.gc_PositionName.Caption = "Vị trí";
		this.gc_PositionName.CurrencyDateField = null;
		this.gc_PositionName.CurrencyField = null;
		this.gc_PositionName.CurrencyFixed = null;
		this.gc_PositionName.CurrencyFixedRate = 0.0;
		this.gc_PositionName.CurrencyRateField = null;
		this.gc_PositionName.FieldName = "PositionName";
		this.gc_PositionName.Name = "gc_PositionName";
		this.gc_PositionName.OptionsColumn.ReadOnly = true;
		this.gc_PositionName.PrimaryKey = false;
		this.gc_PositionName.UnitField = null;
		this.gc_PositionName.Visible = true;
		this.gc_PositionName.VisibleIndex = 11;
		this.gc_Department.Caption = "Phòng ban";
		this.gc_Department.CurrencyDateField = null;
		this.gc_Department.CurrencyField = null;
		this.gc_Department.CurrencyFixed = null;
		this.gc_Department.CurrencyFixedRate = 0.0;
		this.gc_Department.CurrencyRateField = null;
		this.gc_Department.FieldName = "Department";
		this.gc_Department.Name = "gc_Department";
		this.gc_Department.OptionsColumn.ReadOnly = true;
		this.gc_Department.PrimaryKey = false;
		this.gc_Department.UnitField = null;
		this.gc_Department.Visible = true;
		this.gc_Department.VisibleIndex = 12;
		this.gc_BranchName.Caption = "Chi nhánh";
		this.gc_BranchName.CurrencyDateField = null;
		this.gc_BranchName.CurrencyField = null;
		this.gc_BranchName.CurrencyFixed = null;
		this.gc_BranchName.CurrencyFixedRate = 0.0;
		this.gc_BranchName.CurrencyRateField = null;
		this.gc_BranchName.FieldName = "BranchName";
		this.gc_BranchName.Name = "gc_BranchName";
		this.gc_BranchName.OptionsColumn.ReadOnly = true;
		this.gc_BranchName.PrimaryKey = false;
		this.gc_BranchName.UnitField = null;
		this.gc_BranchName.Visible = true;
		this.gc_BranchName.VisibleIndex = 13;
		this.gc_ManagerName.Caption = "Quản lý";
		this.gc_ManagerName.CurrencyDateField = null;
		this.gc_ManagerName.CurrencyField = null;
		this.gc_ManagerName.CurrencyFixed = null;
		this.gc_ManagerName.CurrencyFixedRate = 0.0;
		this.gc_ManagerName.CurrencyRateField = null;
		this.gc_ManagerName.FieldName = "ManagerName";
		this.gc_ManagerName.Name = "gc_ManagerName";
		this.gc_ManagerName.OptionsColumn.ReadOnly = true;
		this.gc_ManagerName.PrimaryKey = false;
		this.gc_ManagerName.UnitField = null;
		this.gc_ManagerName.Visible = true;
		this.gc_ManagerName.VisibleIndex = 14;
		this.gc_ExtEmpNo.Caption = "Mã nhân viên mở rộng";
		this.gc_ExtEmpNo.CurrencyDateField = null;
		this.gc_ExtEmpNo.CurrencyField = null;
		this.gc_ExtEmpNo.CurrencyFixed = null;
		this.gc_ExtEmpNo.CurrencyFixedRate = 0.0;
		this.gc_ExtEmpNo.CurrencyRateField = null;
		this.gc_ExtEmpNo.FieldName = "ExtEmpNo";
		this.gc_ExtEmpNo.Name = "gc_ExtEmpNo";
		this.gc_ExtEmpNo.OptionsColumn.ReadOnly = true;
		this.gc_ExtEmpNo.PrimaryKey = false;
		this.gc_ExtEmpNo.UnitField = null;
		this.gc_ExtEmpNo.Visible = true;
		this.gc_ExtEmpNo.VisibleIndex = 15;
		this.luk_Period.AutoHeight = false;
		this.luk_Period.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Period.Name = "luk_Period";
		this.luk_Period.NullText = "";
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.groupControl1);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.xtraTabControlEx1);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(1131, 289);
		this.splitContainerControl2.SplitterPosition = 403;
		this.splitContainerControl2.TabIndex = 8;
		this.splitContainerControl2.Text = "splitContainerControl2";
		this.groupControl1.Controls.Add(this.grd_KPIs);
		this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl1.Location = new System.Drawing.Point(0, 0);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(403, 289);
		this.groupControl1.TabIndex = 8;
		this.groupControl1.Text = "Danh sách chỉ tiêu";
		this.grd_KPIs.AllowPasteExcel = true;
		this.grd_KPIs.ClipboardData = null;
		this.grd_KPIs.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_KPIs.Location = new System.Drawing.Point(2, 23);
		this.grd_KPIs.MainView = this.gv_KPIs;
		this.grd_KPIs.Name = "grd_KPIs";
		this.grd_KPIs.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repositoryItemCheckEdit1, this.repositoryItemButtonEdit1, this.rep_KpiType });
		this.grd_KPIs.Size = new System.Drawing.Size(399, 264);
		this.grd_KPIs.TabIndex = 5;
		this.grd_KPIs.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_KPIs });
		this.gv_KPIs.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_KPIs.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_KPIs.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_KPIs.ColumnKey = null;
		this.gv_KPIs.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gc_KPICode, this.gc_KPIName, this.gc_DisplayCode, this.gc_Currency, this.gc_KpiType });
		this.gv_KPIs.DisableFormSetting = false;
		this.gv_KPIs.DisplayException = false;
		this.gv_KPIs.EditColumn = null;
		this.gv_KPIs.FieldCurrency = null;
		this.gv_KPIs.FieldRate = null;
		this.gv_KPIs.FormId = null;
		this.gv_KPIs.GridControl = this.grd_KPIs;
		this.gv_KPIs.IgnoreFilter = false;
		this.gv_KPIs.IndexEditRow = -1;
		this.gv_KPIs.IndicatorWidth = 35;
		this.gv_KPIs.IsApzTable = false;
		this.gv_KPIs.IsInsertNewRow = false;
		this.gv_KPIs.IsLoadingData = false;
		this.gv_KPIs.IsMappingContextAction = false;
		this.gv_KPIs.ItemId = null;
		this.gv_KPIs.LstColumnAllowBatch = null;
		this.gv_KPIs.LstColumnNotAllowBatch = null;
		this.gv_KPIs.LstColumnNotEditable = null;
		this.gv_KPIs.LstColumNotSetting = null;
		this.gv_KPIs.Name = "gv_KPIs";
		this.gv_KPIs.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_KPIs.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_KPIs.OptionsNavigation.AutoFocusNewRow = true;
		this.gv_KPIs.OptionsPrint.AutoWidth = false;
		this.gv_KPIs.OptionsPrint.UsePrintStyles = false;
		this.gv_KPIs.OptionsSelection.MultiSelect = true;
		this.gv_KPIs.OptionsView.BestFitMaxRowCount = 100;
		this.gv_KPIs.OptionsView.ColumnAutoWidth = false;
		this.gv_KPIs.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_KPIs.OptionsView.ShowGroupedColumns = true;
		this.gv_KPIs.OptionsView.ShowGroupPanel = false;
		this.gv_KPIs.UseBatchMod = false;
		this.gv_KPIs.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_KPIs_FocusedRowObjectChanged);
		this.gv_KPIs.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_KPIs_BeforeLeaveRow);
		this.gc_KPICode.Caption = "Mã chỉ tiêu";
		this.gc_KPICode.CurrencyDateField = null;
		this.gc_KPICode.CurrencyField = null;
		this.gc_KPICode.CurrencyFixed = null;
		this.gc_KPICode.CurrencyFixedRate = 0.0;
		this.gc_KPICode.CurrencyRateField = null;
		this.gc_KPICode.FieldName = "KPI_Code";
		this.gc_KPICode.Name = "gc_KPICode";
		this.gc_KPICode.PrimaryKey = true;
		this.gc_KPICode.UnitField = null;
		this.gc_KPICode.Visible = true;
		this.gc_KPICode.VisibleIndex = 0;
		this.gc_KPIName.Caption = "Tên chỉ tiêu";
		this.gc_KPIName.CurrencyDateField = null;
		this.gc_KPIName.CurrencyField = null;
		this.gc_KPIName.CurrencyFixed = null;
		this.gc_KPIName.CurrencyFixedRate = 0.0;
		this.gc_KPIName.CurrencyRateField = null;
		this.gc_KPIName.FieldName = "Name";
		this.gc_KPIName.Name = "gc_KPIName";
		this.gc_KPIName.OptionsColumn.ReadOnly = true;
		this.gc_KPIName.PrimaryKey = false;
		this.gc_KPIName.UnitField = null;
		this.gc_KPIName.Visible = true;
		this.gc_KPIName.VisibleIndex = 1;
		this.gc_DisplayCode.Caption = "Mã hiển thị";
		this.gc_DisplayCode.CurrencyDateField = null;
		this.gc_DisplayCode.CurrencyField = null;
		this.gc_DisplayCode.CurrencyFixed = null;
		this.gc_DisplayCode.CurrencyFixedRate = 0.0;
		this.gc_DisplayCode.CurrencyRateField = null;
		this.gc_DisplayCode.FieldName = "DisplayCode";
		this.gc_DisplayCode.Name = "gc_DisplayCode";
		this.gc_DisplayCode.OptionsColumn.ReadOnly = true;
		this.gc_DisplayCode.PrimaryKey = false;
		this.gc_DisplayCode.UnitField = null;
		this.gc_DisplayCode.Visible = true;
		this.gc_DisplayCode.VisibleIndex = 2;
		this.gc_Currency.Caption = "Tiền tệ";
		this.gc_Currency.CurrencyDateField = null;
		this.gc_Currency.CurrencyField = null;
		this.gc_Currency.CurrencyFixed = null;
		this.gc_Currency.CurrencyFixedRate = 0.0;
		this.gc_Currency.CurrencyRateField = null;
		this.gc_Currency.FieldName = "Currency";
		this.gc_Currency.Name = "gc_Currency";
		this.gc_Currency.OptionsColumn.ReadOnly = true;
		this.gc_Currency.PrimaryKey = false;
		this.gc_Currency.UnitField = null;
		this.gc_Currency.Visible = true;
		this.gc_Currency.VisibleIndex = 3;
		this.gc_KpiType.Caption = "Kiểu";
		this.gc_KpiType.ColumnEdit = this.rep_KpiType;
		this.gc_KpiType.CurrencyDateField = null;
		this.gc_KpiType.CurrencyField = null;
		this.gc_KpiType.CurrencyFixed = null;
		this.gc_KpiType.CurrencyFixedRate = 0.0;
		this.gc_KpiType.CurrencyRateField = null;
		this.gc_KpiType.FieldName = "KpiType";
		this.gc_KpiType.Name = "gc_KpiType";
		this.gc_KpiType.OptionsColumn.ReadOnly = true;
		this.gc_KpiType.PrimaryKey = false;
		this.gc_KpiType.UnitField = null;
		this.gc_KpiType.Visible = true;
		this.gc_KpiType.VisibleIndex = 4;
		this.rep_KpiType.AutoHeight = false;
		this.rep_KpiType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_KpiType.Name = "rep_KpiType";
		this.rep_KpiType.NullText = "";
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.DisplayValueChecked = "Y";
		this.repositoryItemCheckEdit1.DisplayValueUnchecked = "N";
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.repositoryItemButtonEdit1.AutoHeight = false;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.repositoryItemButtonEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.repositoryItemButtonEdit1.Name = "repositoryItemButtonEdit1";
		this.xtraTabControlEx1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControlEx1.IsSearchControl = false;
		this.xtraTabControlEx1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControlEx1.Name = "xtraTabControlEx1";
		this.xtraTabControlEx1.SelectedTabPage = this.tab_Index;
		this.xtraTabControlEx1.Size = new System.Drawing.Size(718, 289);
		this.xtraTabControlEx1.TabIndex = 0;
		this.xtraTabControlEx1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.tab_Index });
		this.tab_Index.Controls.Add(this.grd_Points);
		this.tab_Index.EditControl = null;
		this.tab_Index.EditValue = "Trọng số điểm";
		this.tab_Index.FieldName = "";
		this.tab_Index.IsSearchControl = false;
		this.tab_Index.Name = "tab_Index";
		this.tab_Index.ReadOnly = false;
		this.tab_Index.SettingInVisible = false;
		this.tab_Index.SettingReadonly = false;
		this.tab_Index.Size = new System.Drawing.Size(716, 264);
		this.tab_Index.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Index.Text = "Trọng số điểm";
		this.grd_Points.AllowPasteExcel = true;
		this.grd_Points.ClipboardData = null;
		this.grd_Points.ContextMenuStrip = this.ctx_Point;
		this.grd_Points.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Points.Location = new System.Drawing.Point(0, 0);
		this.grd_Points.MainView = this.gv_Points;
		this.grd_Points.Name = "grd_Points";
		this.grd_Points.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.chk_Active, this.rep_CardCode, this.calc_Point, this.cfl_Condition });
		this.grd_Points.Size = new System.Drawing.Size(716, 264);
		this.grd_Points.TabIndex = 4;
		this.grd_Points.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Points });
		this.ctx_Point.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.ctm_DeleteEmp });
		this.ctx_Point.Name = "ctx_Doc";
		this.ctx_Point.Size = new System.Drawing.Size(108, 26);
		this.ctm_DeleteEmp.Name = "ctm_DeleteEmp";
		this.ctm_DeleteEmp.Size = new System.Drawing.Size(107, 22);
		this.ctm_DeleteEmp.Text = "Delete";
		this.ctm_DeleteEmp.Click += new System.EventHandler(this.ctm_DeleteEmp_Click);
		this.gv_Points.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Points.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Points.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Points.ColumnKey = null;
		this.gv_Points.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gc_Condition, this.gc_ConditionName, this.gc_FromValue, this.gc_ToValue, this.gc_Point });
		this.gv_Points.DisableFormSetting = false;
		this.gv_Points.DisplayException = false;
		this.gv_Points.EditColumn = null;
		this.gv_Points.FieldCurrency = null;
		this.gv_Points.FieldRate = null;
		this.gv_Points.FormId = null;
		this.gv_Points.GridControl = this.grd_Points;
		this.gv_Points.IgnoreFilter = false;
		this.gv_Points.IndexEditRow = -1;
		this.gv_Points.IndicatorWidth = 35;
		this.gv_Points.IsApzTable = false;
		this.gv_Points.IsInsertNewRow = false;
		this.gv_Points.IsLoadingData = false;
		this.gv_Points.IsMappingContextAction = false;
		this.gv_Points.ItemId = null;
		this.gv_Points.LstColumnAllowBatch = null;
		this.gv_Points.LstColumnNotAllowBatch = null;
		this.gv_Points.LstColumnNotEditable = null;
		this.gv_Points.LstColumNotSetting = null;
		this.gv_Points.Name = "gv_Points";
		this.gv_Points.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Points.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Points.OptionsNavigation.AutoFocusNewRow = true;
		this.gv_Points.OptionsPrint.AutoWidth = false;
		this.gv_Points.OptionsPrint.UsePrintStyles = false;
		this.gv_Points.OptionsSelection.MultiSelect = true;
		this.gv_Points.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Points.OptionsView.ColumnAutoWidth = false;
		this.gv_Points.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Points.OptionsView.ShowGroupedColumns = true;
		this.gv_Points.OptionsView.ShowGroupPanel = false;
		this.gv_Points.UseBatchMod = false;
		this.gv_Points.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Points_CellValueChanged);
		this.gc_Condition.Caption = "Điều kiện";
		this.gc_Condition.ColumnEdit = this.cfl_Condition;
		this.gc_Condition.CurrencyDateField = null;
		this.gc_Condition.CurrencyField = null;
		this.gc_Condition.CurrencyFixed = null;
		this.gc_Condition.CurrencyFixedRate = 0.0;
		this.gc_Condition.CurrencyRateField = null;
		this.gc_Condition.FieldName = "Condition_Code";
		this.gc_Condition.Name = "gc_Condition";
		this.gc_Condition.PrimaryKey = false;
		this.gc_Condition.UnitField = null;
		this.gc_Condition.Visible = true;
		this.gc_Condition.VisibleIndex = 0;
		this.cfl_Condition.AutoHeight = false;
		this.cfl_Condition.AutoLoadDataSource = true;
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		this.cfl_Condition.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_Condition.ContainerControl = this;
		this.cfl_Condition.DisplayMember = "";
		this.cfl_Condition.DisplayMember2 = "";
		this.cfl_Condition.LoadDataSourceWhenEnter = false;
		this.cfl_Condition.Name = "cfl_Condition";
		this.cfl_Condition.SeparateCharacterForMulti = ';';
		this.cfl_Condition.SeparateString = ", ";
		this.cfl_Condition.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.PromotionCondition;
		this.cfl_Condition.UsingAutoCompleteBox = false;
		this.cfl_Condition.UsingDataSource = true;
		this.cfl_Condition.ValidateWhenNoDataSource = false;
		this.cfl_Condition.ValueMember = "";
		this.gc_ConditionName.Caption = "Tên điều kiện";
		this.gc_ConditionName.CurrencyDateField = null;
		this.gc_ConditionName.CurrencyField = null;
		this.gc_ConditionName.CurrencyFixed = null;
		this.gc_ConditionName.CurrencyFixedRate = 0.0;
		this.gc_ConditionName.CurrencyRateField = null;
		this.gc_ConditionName.FieldName = "Condition_Name";
		this.gc_ConditionName.Name = "gc_ConditionName";
		this.gc_ConditionName.PrimaryKey = false;
		this.gc_ConditionName.UnitField = null;
		this.gc_ConditionName.Visible = true;
		this.gc_ConditionName.VisibleIndex = 1;
		this.gc_FromValue.Caption = "Từ giá trị";
		this.gc_FromValue.ColumnEdit = this.calc_Point;
		this.gc_FromValue.CurrencyDateField = null;
		this.gc_FromValue.CurrencyField = null;
		this.gc_FromValue.CurrencyFixed = null;
		this.gc_FromValue.CurrencyFixedRate = 0.0;
		this.gc_FromValue.CurrencyRateField = null;
		this.gc_FromValue.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gc_FromValue.FieldName = "FromValue";
		this.gc_FromValue.Name = "gc_FromValue";
		this.gc_FromValue.PrimaryKey = false;
		this.gc_FromValue.UnitField = null;
		this.gc_FromValue.Visible = true;
		this.gc_FromValue.VisibleIndex = 2;
		this.calc_Point.AutoHeight = false;
		this.calc_Point.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_Point.Name = "calc_Point";
		this.gc_ToValue.Caption = "Đến giá trị";
		this.gc_ToValue.ColumnEdit = this.calc_Point;
		this.gc_ToValue.CurrencyDateField = null;
		this.gc_ToValue.CurrencyField = null;
		this.gc_ToValue.CurrencyFixed = null;
		this.gc_ToValue.CurrencyFixedRate = 0.0;
		this.gc_ToValue.CurrencyRateField = null;
		this.gc_ToValue.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gc_ToValue.FieldName = "ToValue";
		this.gc_ToValue.Name = "gc_ToValue";
		this.gc_ToValue.PrimaryKey = false;
		this.gc_ToValue.UnitField = null;
		this.gc_ToValue.Visible = true;
		this.gc_ToValue.VisibleIndex = 3;
		this.gc_Point.Caption = "Điểm tương ứng";
		this.gc_Point.ColumnEdit = this.calc_Point;
		this.gc_Point.CurrencyDateField = null;
		this.gc_Point.CurrencyField = null;
		this.gc_Point.CurrencyFixed = null;
		this.gc_Point.CurrencyFixedRate = 0.0;
		this.gc_Point.CurrencyRateField = null;
		this.gc_Point.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gc_Point.FieldName = "Point";
		this.gc_Point.Name = "gc_Point";
		this.gc_Point.PrimaryKey = false;
		this.gc_Point.UnitField = null;
		this.gc_Point.Visible = true;
		this.gc_Point.VisibleIndex = 4;
		this.gc_Point.Width = 157;
		this.chk_Active.AutoHeight = false;
		this.chk_Active.DisplayValueChecked = "Y";
		this.chk_Active.DisplayValueUnchecked = "N";
		this.chk_Active.Name = "chk_Active";
		this.chk_Active.ValueChecked = "Y";
		this.chk_Active.ValueUnchecked = "N";
		this.rep_CardCode.AutoHeight = false;
		editorButtonImageOptions5.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_CardCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_CardCode.Name = "rep_CardCode";
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(83, 482);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 11;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(2, 482);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 10;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_CopyFormSetting.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_CopyFormSetting.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_CopyFormSetting.ImageOptions.Image");
		this.btn_CopyFormSetting.Location = new System.Drawing.Point(1017, 483);
		this.btn_CopyFormSetting.Name = "btn_CopyFormSetting";
		this.btn_CopyFormSetting.Size = new System.Drawing.Size(116, 22);
		this.btn_CopyFormSetting.TabIndex = 12;
		this.btn_CopyFormSetting.Text = "Copy KPI Setting";
		this.btn_CopyFormSetting.Click += new System.EventHandler(this.btn_CopyFormSetting_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1135, 510);
		base.Controls.Add(this.btn_CopyFormSetting);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oKPIsSetting";
		base.FormObjectType = Apzon.Commons.ApzObjectTypes.oKPIsSetting;
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_KPIsSetting";
		this.Text = "Cài đặt điểm chuẩn KPI";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_KPIsSetting_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctx_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_KPIsMaster).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ViewType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Remark).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_StatusCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Period).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_KPIs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_KPIs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_KpiType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).EndInit();
		this.xtraTabControlEx1.ResumeLayout(false);
		this.tab_Index.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Points).EndInit();
		this.ctx_Point.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Points).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_Condition).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_Point).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Active).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CardCode).EndInit();
		base.ResumeLayout(false);
	}
}
