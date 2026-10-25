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
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Apzon.Forms.HumanResources;

public class frm_EmpPenalization : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _preAdd;

	private bool _isChanging;

	private bool _isLoading;

	private bool _isCancel;

	private bool _forceClose;

	private int _maxLineNumInDb;

	private DataTable _dtPenalType;

	private RepositoryItemButtonEditCFL rep_CFL_NoSearch = new RepositoryItemButtonEditCFL();

	private RepositoryItemButtonEditCFL rep_empName_NoSearch = new RepositoryItemButtonEditCFL();

	private IContainer components;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private GridControlEx grd_Line;

	private GridViewEx gv_Line;

	private GridColumnEx gc_empNo;

	private GridColumnEx gc_empName;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gc_Type;

	private GridColumnEx gridColumnEx5;

	private RepositoryItemLookUpEdit luk_PenalType;

	private RepositoryItemDateEditEx repositoryItemDateEdit1;

	private RepositoryItemLookUpEdit rep_Deparment;

	private RepositoryItemButtonEditCFL rep_CFL;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private GridColumnEx gridColumnEx6;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private ToolStripMenuItem duplicateRowToolStripMenuItem;

	private SplitContainerControl splitContainerControl1;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem removeToolStripMenuItem;

	private RepositoryItemButtonEditCFL rep_empName;

	private RepositoryItemGridLookUpEdit gridLuk_Type;

	private GridView repositoryItemGridLookUpEdit1View;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx8;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private GridColumnEx gridColumnEx13;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gc_Period;

	private RepositoryItemButtonEditCFL repositoryItemButtonEditCFL1;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gridColumnEx11;

	private GridColumnEx gridColumnEx9;

	private RepositoryItemLookUpEdit rep_Branch;

	private GridColumnEx gridColumnEx12;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit2;

	private GridColumnEx gridColumnEx2;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

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
		case Enums.FormViewModeType.Add:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_EmpPenalization()
	{
		InitializeComponent();
		FormId = "oEmpPenalization";
		gv_Line.ItemId = "APZ_HPT1";
		gv_Line.FormId = FormId;
		gv_Doc.ItemId = "APZ_HPT2";
		gv_Doc.FormId = FormId;
		gv_Doc.ChangeLogType = Enums.ChangeLog.clPenalization;
		gv_Doc.IsApzTable = true;
		OnLoadAsync += frm_PenalType_Load;
	}

	private async Task LoadData()
	{
		try
		{
			DataTable dataTable = await _httpClient.EmpPenalization.GetPeriodRecords();
			if (!dataTable.IsNotNull())
			{
				AddNewDoc();
			}
			else
			{
				dataTable.SetDefaultAndMaxLen(gv_Doc.ItemId);
				grd_Doc.DataSource = dataTable;
				grd_Doc.RefreshDataSource();
				if (grd_Doc.DataSource is DataTable dataTable2)
				{
					dataTable2.AcceptChanges();
				}
				FormMode = Enums.FormViewModeType.View;
				gc_Period.OptionsColumn.ReadOnly = true;
				repositoryItemButtonEditCFL1.Buttons[0].Visible = false;
			}
			gv_Line.RefreshData();
			rep_CFL.RefreshDataSource();
			rep_empName.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewRow()
	{
		DataTable dataTable = grd_Line.DataSourceTable;
		try
		{
			gv_Line.BeginInsertRow();
			int num;
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = WinFuntions.GetTableStructure("APZ_HPT1");
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
				num = -1;
			}
			else
			{
				num = (dataTable.IsNotNull() ? dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"])) : (-1));
			}
			if (dataTable != null && dataTable.Columns.Count > 0 && (dataTable.Rows.Count == 0 || dataTable.AsEnumerable().All((DataRow t) => !string.IsNullOrEmpty(Function.ToString(t["EmpId"])))))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["EmpId"] = DBNull.Value;
				dataRow["LineNum"] = num + 1;
				dataTable.Rows.Add(dataRow);
				dataTable.SetDefaultAndMaxLen(gv_Line.ItemId);
				grd_Line.DataSource = dataTable;
				grd_Line.RefreshDataSource();
			}
			gv_Line.EndInsertRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		gv_Line.EndInsertRow();
		dataTable.AcceptChanges();
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if ((FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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

	public async Task REFRESH()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				AddNewDoc();
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
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			DataSet dataSet = new DataSet();
			DataTable dataTable = grd_Line.DataSourceTable;
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			DataTable dataTable2 = dataTable.Copy();
			DataTable dataTable3 = dataSourceTable.Clone();
			DataRow dataRow = dataTable3.NewRow();
			int currentDocNum = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocNum"));
			dataRow["DocNum"] = currentDocNum;
			dataRow["Period"] = Function.ToString(gv_Doc.GetFocusedRowCellValue("Period"));
			dataRow["Remarks"] = Function.ToString(gv_Doc.GetFocusedRowCellValue("Remarks"));
			dataRow["Branch"] = Function.ToString(gv_Doc.GetFocusedRowCellValue("Branch"));
			dataRow["IncludeBonus"] = Function.ToString(gv_Doc.GetFocusedRowCellValue("IncludeBonus"));
			dataRow["IncludePenalty"] = Function.ToString(gv_Doc.GetFocusedRowCellValue("IncludePenalty"));
			dataRow["UserSign2"] = GlobalData.UserID;
			dataRow["UpdateDate"] = DateTime.Today;
			dataTable3.Rows.Add(dataRow);
			dataTable3.AcceptChanges();
			if (dataTable2.IsNotNull() && Function.ParseInt(dataTable2.Rows[dataTable2.Rows.Count - 1]["EmpId"]) == 0)
			{
				dataTable2.Rows.RemoveAt(dataTable2.Rows.Count - 1);
				dataTable2.AcceptChanges();
			}
			if (dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["EmpId"]))) != null)
			{
				WinCommons.Error(Enums.MessageCode.EmpIdCannotBeEmpty, "");
				return "Error";
			}
			DataRow dataRow2 = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["Date"])));
			if (dataRow2 != null)
			{
				WinCommons.Error(Enums.MessageCode.DateCannotBeEmptyWithEmpNo, "", Function.ToString(dataRow2["EmpId"]));
				return "Error";
			}
			DataRow dataRow3 = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["PenalizationType"]) == 0);
			if (dataRow3 != null)
			{
				WinCommons.Error(Enums.MessageCode.TypeCannotBeEmpty, "", Function.ToString(dataRow3["EmpId"]));
				return "Error";
			}
			DateTime fromDate = Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("FromDate"));
			DateTime toDate = Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("ToDate"));
			DataRow dataRow4 = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseDateTimes(t["Date"]) < fromDate || Function.ParseDateTimes(t["Date"]) > toDate);
			if (dataRow4 != null)
			{
				WinCommons.Error(Enums.MessageCode.DateCannotBeOutOfPeriod, "", Function.ToString(dataRow4["EmpId"]));
				return "Error";
			}
			if (dataTable3.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["Period"]))) != null)
			{
				WinCommons.Error(Enums.MessageCode.PeriodCannotBeNull, "");
				return "Error";
			}
			if (dataTable3.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["Branch"]) == 0) != null)
			{
				WinCommons.Error(Enums.MessageCode.BranchCannotBeEmpty, "");
				return "Error";
			}
			if (dataTable.IsNotNull())
			{
				EnumerableRowCollection<DataRow> source = from t in dataTable.AsEnumerable()
					where Function.ParseInt(t["EmpId"]) > 0
					select t;
				if (source.Any())
				{
					dataTable = source.CopyToDataTable().Copy();
				}
			}
			if (dataTable3.IsNotNull())
			{
				dataTable3.TableName = "dtDoc";
				dataSet.Tables.Add(dataTable3.Copy());
				if (!dataTable.IsNotNull())
				{
					dataTable = WinFuntions.GetTableStructure("APZ_HPT1");
				}
				dataTable.TableName = "dtLine";
				dataSet.Tables.Add(dataTable.Copy());
				string text = await _httpClient.EmpPenalization.Save(dataSet);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					FormMode = Enums.FormViewModeType.View;
					await REFRESH_DATA();
					int num = gv_Doc.LocateByValue("DocNum", currentDocNum);
					if (num != int.MinValue)
					{
						gv_Doc.FocusedRowHandle = num;
						gv_Doc.MakeRowVisible(num);
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

	private async Task LoadInitForm()
	{
		_ = 2;
		try
		{
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadComboDept();
			DataRow row = dataTable.NewRow();
			dataTable.Rows.InsertAt(row, 0);
			dataTable.AcceptChanges();
			rep_Deparment.DataSource = dataTable.Copy();
			rep_Deparment.DisplayMember = "Name";
			rep_Deparment.ValueMember = "Code";
			rep_Deparment.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Deparment.DropDownRows = dataTable.Rows.Count;
			rep_Deparment.ForceInitialize();
			rep_Deparment.PopulateColumns();
			rep_Deparment.Columns["Code"].Visible = false;
			rep_Deparment.Columns["Remarks"].Visible = false;
			DataTable dataTable2 = await _httpClient.EmployeeMasterClient.LoadComboBranch();
			DataRow row2 = dataTable2.NewRow();
			dataTable2.Rows.InsertAt(row2, 0);
			dataTable2.AcceptChanges();
			rep_Branch.DataSource = dataTable2.Copy();
			rep_Branch.DisplayMember = "Name";
			rep_Branch.ValueMember = "Code";
			rep_Branch.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Branch.ForceInitialize();
			rep_Branch.PopulateColumns();
			rep_Branch.Columns["Code"].Visible = false;
			LoadLookupType();
			repositoryItemButtonEditCFL1.DisplayMember = "PeriodCode";
			repositoryItemButtonEditCFL1.ValueMember = "PeriodCode";
			repositoryItemButtonEditCFL1.FindCFLWith(Enums.TypeCFL.UserDefinedProcedure, false, "APZ_WorkPeriod_FindWorkPeriodForPenBonus", Enums.ItemObjectType.NoItemType, null, "");
			repositoryItemButtonEditCFL1.Buttons[1].Visible = false;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = repositoryItemButtonEditCFL1;
			repositoryItemButtonEditCFL.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL.HandleChooseItem, (Action<DataRow>)((DataRow dataRow) =>
			{
				gv_Doc.SetFocusedRowCellValue("Period", Function.ToString(dataRow["PeriodCode"]));
				if (Function.ParseInt(dataRow["BranchCode"]) != Function.ParseInt(gv_Doc.GetFocusedRowCellValue("Branch")))
				{
					if (grd_Line.DataSource is DataTable dataTable3)
					{
						dataTable3.Rows.Clear();
					}
					_maxLineNumInDb = -1;
					AddNewRow();
				}
				gv_Doc.SetFocusedRowCellValue("Branch", Function.ParseInt(dataRow["BranchCode"]));
				gv_Doc.SetFocusedRowCellValue("FromDate", Function.ParseDateTimes(dataRow["FromDate"]));
				gv_Doc.SetFocusedRowCellValue("ToDate", Function.ParseDateTimes(dataRow["ToDate"]));
				gv_Doc.PostEditor();
				gv_Doc.RefreshData();
			}));
			rep_CFL.ValueMember = "EmpId";
			rep_CFL.DisplayMember = "EmpId";
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = rep_CFL;
			repositoryItemButtonEditCFL2.DataSourceLoaded = (Action<DataTable>)Delegate.Combine(repositoryItemButtonEditCFL2.DataSourceLoaded, (Action<DataTable>)((DataTable dt) =>
			{
				rep_empName.DataSource = dt.Copy();
			}));
			rep_CFL.FindCFLWith(Enums.TypeCFL.Employees, true, "Apz_Cfl_EmployleeData", Enums.ItemObjectType.NoItemType, null, "");
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL3 = rep_CFL;
			repositoryItemButtonEditCFL3.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL3.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL cfl) =>
			{
				cfl.Branch = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("Branch"));
			}));
			rep_CFL.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL4 = rep_CFL;
			repositoryItemButtonEditCFL4.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL4.HandleChooseItem, new Action<DataRow>(FillData));
			rep_CFL.HandleChooseItemMulti = HandleChooseItemMulti;
			rep_empName.ValueMember = "EmpId";
			rep_empName.DisplayMember = "lastName";
			rep_empName.DisplayMember2 = "firstName";
			rep_empName.SeparateString = " ";
			rep_empName.AutoLoadDataSource = false;
			rep_empName.FindCFLWith(Enums.TypeCFL.Employees, true, "Apz_Cfl_EmployleeData", Enums.ItemObjectType.NoItemType, null, "");
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL5 = rep_empName;
			repositoryItemButtonEditCFL5.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL5.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL cfl) =>
			{
				cfl.Branch = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("Branch"));
			}));
			rep_empName.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL6 = rep_empName;
			repositoryItemButtonEditCFL6.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL6.HandleChooseItem, new Action<DataRow>(FillData));
			rep_empName.HandleChooseItemMulti = HandleChooseItemMulti;
			gc_empName.FilterMode = ColumnFilterMode.DisplayText;
			Dictionary<string, GridViewEx> dictionary = new Dictionary<string, GridViewEx>();
			dictionary["APZ_HPT1"] = gv_Line;
			await WinFuntions.LoadUdfColumns(gv_Line, "APZ_HPT1", dictionary);
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
			if (!(grd_Line.DataSource is DataTable dataTable) || dataTable.Rows.Count <= 0)
			{
				return;
			}
			if (rows.Count > 0)
			{
				gv_Line.SetFocusedRowCellValue("EmpId", Function.ParseInt(rows[0]["EmpId"]));
				for (int i = 1; i < rows.Count; i++)
				{
					DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["EmpId"])));
					if (dataRow != null)
					{
						int dataSourceIndex = dataTable.Rows.IndexOf(dataRow);
						gv_Line.SetRowCellValue(gv_Line.GetRowHandle(dataSourceIndex), "EmpId", Function.ParseInt(rows[i]["EmpId"]));
						continue;
					}
					DataRow dataRow2 = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["EmpId"])));
					if (dataRow2 != null)
					{
						int dataSourceIndex2 = dataTable.Rows.IndexOf(dataRow2);
						gv_Line.SetRowCellValue(gv_Line.GetRowHandle(dataSourceIndex2), "EmpId", Function.ParseInt(rows[i]["EmpId"]));
					}
				}
			}
			if (FormMode != Enums.FormViewModeType.Add)
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

	private void FillData(DataRow dataRow)
	{
		try
		{
			if (Function.ParseInt(gv_Line.GetFocusedRowCellValue("LineNum")) > _maxLineNumInDb)
			{
				gv_Line.SetFocusedRowCellValue("EmpId", Function.ParseInt(dataRow["empID"]));
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

	private async void LoadLookupType()
	{
		try
		{
			_dtPenalType = await _httpClient.PenalType.GetData();
			DataRow dataRow = _dtPenalType.NewRow();
			dataRow["Code"] = -10000;
			dataRow["Name"] = "-Define New-";
			dataRow["Type"] = "Z";
			_dtPenalType.Rows.Add(dataRow);
			_dtPenalType.AcceptChanges();
			luk_PenalType.DataSource = null;
			luk_PenalType.DataSource = _dtPenalType.Copy();
			luk_PenalType.DisplayMember = "Name";
			luk_PenalType.ValueMember = "Code";
			luk_PenalType.BestFitMode = BestFitMode.BestFitResizePopup;
			luk_PenalType.ForceInitialize();
			luk_PenalType.PopulateColumns();
			luk_PenalType.Columns["Code"].Visible = false;
			luk_PenalType.Columns["Amount"].Visible = false;
			luk_PenalType.Columns["Type"].Visible = false;
			gridLuk_Type.DataSource = _dtPenalType.Copy();
			gridLuk_Type.DisplayMember = "Name";
			gridLuk_Type.ValueMember = "Code";
			gridLuk_Type.BestFitMode = BestFitMode.BestFitResizePopup;
			gridLuk_Type.ViewType = GridLookUpViewType.GridView;
			gridLuk_Type.PopulateViewColumns();
			gridLuk_Type.View.Columns["Code"].Visible = false;
			gridLuk_Type.View.Columns["Amount"].Visible = false;
			gridLuk_Type.View.Columns["Type"].Visible = false;
			gridLuk_Type.View.Columns["TypeGroup"].Visible = false;
			gridLuk_Type.View.Columns["Type"].GroupIndex = 0;
			gridLuk_Type.View.CustomDrawGroupRow += View_CustomDrawGroupRow;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void View_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
	{
		try
		{
			if (sender is GridView)
			{
				if (!(e.Info is GridGroupRowInfo gridGroupRowInfo) || gridGroupRowInfo.Column.FieldName != "Type")
				{
					return;
				}
				string groupValueText = gridGroupRowInfo.GroupValueText;
				if (!(groupValueText == "B"))
				{
					if (groupValueText == "P")
					{
						gridGroupRowInfo.GroupText = "Penalty";
					}
					else
					{
						gridGroupRowInfo.GroupText = "";
					}
				}
				else
				{
					gridGroupRowInfo.GroupText = "Bonus";
				}
			}
			gridLuk_Type.View.ExpandAllGroups();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void GetDetails()
	{
		try
		{
			_isLoading = true;
			int docNo = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocNum"));
			DataTable dataTable = await _httpClient.EmpPenalization.GetData(docNo);
			dataTable.SetDefaultAndMaxLen(gv_Line.ItemId);
			grd_Line.DataSource = dataTable;
			grd_Line.RefreshDataSource();
			if (dataTable.IsNotNull())
			{
				_maxLineNumInDb = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"]));
				if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Date"])))
				{
					AddNewRow();
				}
			}
			else
			{
				_maxLineNumInDb = -1;
				AddNewRow();
			}
			FormMode = Enums.FormViewModeType.View;
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			_isLoading = false;
		}
	}

	private async Task frm_PenalType_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			await LoadInitForm();
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Line.DataSourceTable;
			int num = Function.ParseInt(gv_Line.GetFocusedRowCellValue("LineNum"));
			int num2 = dataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"]));
			if (num == num2 || gv_Line.FocusedRowHandle < 0)
			{
				return;
			}
			if (num > _maxLineNumInDb)
			{
				gv_Line.BeginUpdate();
				gv_Line.DeleteSelectedRows();
				gv_Line.EndUpdate();
				dataSourceTable.AcceptChanges();
				grd_Line.RefreshDataSource();
				return;
			}
			gv_Line.BeginUpdate();
			gv_Line.DeleteSelectedRows();
			gv_Line.EndUpdate();
			dataSourceTable.AcceptChanges();
			grd_Line.RefreshDataSource();
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

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_isChanging)
			{
				if (string.IsNullOrEmpty(Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "Period"))))
				{
					_isChanging = true;
					gv_Doc.SetRowCellValue(e.RowHandle, "Branch", DBNull.Value);
					gv_Doc.SetRowCellValue(e.RowHandle, "FromDate", DBNull.Value);
					gv_Doc.SetRowCellValue(e.RowHandle, "ToDate", DBNull.Value);
					_isChanging = false;
				}
				if (FormMode != Enums.FormViewModeType.Add)
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

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					btn_Save.Enabled = false;
					btnCancel.Enabled = false;
					await SaveData();
					btn_Save.Enabled = true;
					btnCancel.Enabled = true;
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

	private async void frm_PenalType_FormClosing(object sender, FormClosingEventArgs e)
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
			if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
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

	private void rep_CFL_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
	}

	private void luk_PenalType_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseInt(e.NewValue) == -10000)
			{
				frm_PenalType frm_PenalType2 = new frm_PenalType
				{
					MdiParent = MdiParent
				};
				frm_PenalType2.refreshType = (RefreshLookupType)Delegate.Combine(frm_PenalType2.refreshType, new RefreshLookupType(LoadLookupType));
				frm_PenalType2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridLuk_Type_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseInt(e.NewValue) == -10000)
			{
				e.Cancel = true;
				frm_PenalType frm_PenalType2 = new frm_PenalType();
				frm_PenalType2.refreshType = (RefreshLookupType)Delegate.Combine(frm_PenalType2.refreshType, new RefreshLookupType(LoadLookupType));
				frm_PenalType2.ShowDialog(this);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridLuk_Type_Popup(object sender, EventArgs e)
	{
		((GridLookUpEdit)sender).Properties.View.ExpandAllGroups();
	}

	private void duplicateRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Line.DataSourceTable;
			int num = dataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"]));
			if (gv_Line.FocusedRowHandle >= 0 && !string.IsNullOrEmpty(Function.ToString(gv_Line.GetFocusedRowCellValue("PenalizationType"))) && Function.ParseInt(gv_Line.GetFocusedRowCellValue("EmpId")) > 0 && dataSourceTable != null)
			{
				DataRow dataRow = dataSourceTable.NewRow();
				dataRow["EmpId"] = ((gv_Line.GetFocusedRowCellValue("EmpId") != null) ? ((object)Function.ParseInt(gv_Line.GetFocusedRowCellValue("EmpId"))) : DBNull.Value);
				dataRow["empName"] = gv_Line.GetFocusedRowCellValue("empName");
				dataRow["dept"] = ((gv_Line.GetFocusedRowCellValue("dept") != null) ? ((object)Function.ParseInt(gv_Line.GetFocusedRowCellValue("dept"))) : DBNull.Value);
				dataRow["LineNum"] = ((Function.ParseInt(gv_Line.GetFocusedRowCellValue("LineNum")) != -1) ? ((object)num) : DBNull.Value);
				dataRow["PenalizationType"] = ((gv_Line.GetFocusedRowCellValue("PenalizationType") != null) ? ((object)Function.ParseInt(gv_Line.GetFocusedRowCellValue("PenalizationType"))) : DBNull.Value);
				dataRow["Note"] = gv_Line.GetFocusedRowCellValue("Note");
				dataRow["ExtEmpNo"] = gv_Line.GetFocusedRowCellValue("ExtEmpNo");
				dataSourceTable.Rows.InsertAt(dataRow, dataSourceTable.Rows.Count - 1);
				dataSourceTable.Rows[dataSourceTable.Rows.Count - 1]["LineNum"] = num + 1;
				dataSourceTable.AcceptChanges();
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_CFLPeriod_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
	}

	private void gv_Doc_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
	}

	private void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (!_preAdd)
			{
				GetDetails();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Line_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (_isChanging)
			{
				return;
			}
			if (e.Column == gc_Type)
			{
				_isChanging = true;
				gv_Line.BeginUpdate();
				if (_dtPenalType.IsNotNull())
				{
					DataRow dataRow = _dtPenalType.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["Code"]) == Function.ParseInt(e.Value));
					if (dataRow != null)
					{
						gv_Line.SetRowCellValue(e.RowHandle, "TotalAmount", Function.ParseDouble(dataRow["Amount"]));
					}
				}
				gv_Line.EndUpdate();
				_isChanging = false;
			}
			else if (e.Column.FieldName.Equals(gc_empNo.FieldName))
			{
				DataTableResult employeeInfo = HttpClient.EmployeeMasterClient.GetEmployeeInfo(Function.ParseInt(e.Value));
				if (employeeInfo.Data.IsNotNull())
				{
					gv_Line.SetRowCellValue(e.RowHandle, "dept", Function.ParseInt(employeeInfo.Data.Rows[0]["dept"]));
					gv_Line.SetRowCellValue(e.RowHandle, "empName", Function.ParseInt(employeeInfo.Data.Rows[0]["empID"]));
					gv_Line.SetRowCellValue(e.RowHandle, "ExtEmpNo", Function.ToString(employeeInfo.Data.Rows[0]["ExtEmpNo"]));
				}
			}
			DataTable dataTable = grd_Line.DataSource as DataTable;
			int num = Function.ParseInt(gv_Line.GetRowCellValue(e.RowHandle, "EmpId"));
			Function.ToString(gv_Line.GetRowCellValue(e.RowHandle, "Date"));
			Function.ParseInt(gv_Line.GetRowCellValue(e.RowHandle, "PenalizationType"));
			Function.ParseInt(gv_Line.GetRowCellValue(e.RowHandle, "LineNum"));
			dataTable?.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"]));
			if (num == 0 && e.Column == gc_empNo)
			{
				_isChanging = true;
				gv_Line.SetRowCellValue(e.RowHandle, "empName", DBNull.Value);
				gv_Line.SetRowCellValue(e.RowHandle, "dept", null);
				_isChanging = false;
			}
			AddNewRow();
			if (FormMode != Enums.FormViewModeType.Add)
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

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
	}

	private void gv_Line_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			DataTable dataTable = gridLuk_Type.DataSource as DataTable;
			if (e.Column != gc_Type || dataTable == null)
			{
				return;
			}
			DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["Code"]) == Function.ParseInt(e.CellValue));
			if (dataRow != null)
			{
				if (Function.ToString(dataRow["Type"]).Equals("B"))
				{
					e.Appearance.BackColor = Color.LightGreen;
				}
				else if (Function.ToString(dataRow["Type"]).Equals("P"))
				{
					e.Appearance.BackColor = Color.LightCoral;
				}
				else
				{
					e.Appearance.BackColor = Color.White;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Line_ShowingEditor(object sender, CancelEventArgs e)
	{
	}

	private void gv_Line_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			if (!(sender is ColumnView columnView) || !(((GridColumnEx)columnView.FocusedColumn).FieldName != "Date"))
			{
				DateTime dateTime = Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("FromDate"));
				DateTime dateTime2 = Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("ToDate"));
				DateTime dateTime3 = Function.ParseDateTimes(e.Value);
				if (dateTime3 < dateTime || dateTime3 > dateTime2)
				{
					e.Valid = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Line_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		try
		{
			if (sender is ColumnView columnView && !(((GridColumnEx)columnView.FocusedColumn).FieldName != "Date"))
			{
				e.ExceptionMode = ExceptionMode.DisplayError;
				e.WindowCaption = "Input Error";
				e.ErrorText = "Date is not valid (Not in period's Time range)!";
				columnView.HideEditor();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Line_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column == gc_empNo)
			{
				if (Function.ParseInt(gv_Line.GetRowCellValue(e.RowHandle, "LineNum")) <= _maxLineNumInDb || string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue("Period"))))
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
				if (Function.ParseInt(gv_Line.GetRowCellValue(e.RowHandle, "LineNum")) <= _maxLineNumInDb || string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue("Period"))))
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

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			REFRESH();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocNum"));
			if (num != 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				string text = await _httpClient.EmpPenalization.Remove(Function.ParseInt(num));
				if (string.IsNullOrEmpty(text))
				{
					await REFRESH_DATA();
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

	private void AddNewDoc()
	{
		try
		{
			_preAdd = true;
			gv_Doc.BeginInsertRow();
			gc_Period.OptionsColumn.ReadOnly = false;
			repositoryItemButtonEditCFL1.Buttons[0].Visible = true;
			DataTable dataTable = grd_Doc.DataSourceTable();
			if (grd_Line.DataSource is DataTable dataTable2)
			{
				dataTable2.Rows.Clear();
			}
			if (grd_Line.DataSource is DataTable dataTable3)
			{
				dataTable3.Rows.Clear();
			}
			if (!dataTable.IsNotNull())
			{
				DataTable tableStructure = WinFuntions.GetTableStructure("APZ_HPT2");
				if (!tableStructure.Columns.Contains("FromDate"))
				{
					tableStructure.Columns.Add("FromDate", typeof(DateTime));
				}
				if (!tableStructure.Columns.Contains("ToDate"))
				{
					tableStructure.Columns.Add("ToDate", typeof(DateTime));
				}
				tableStructure.Rows.Add(tableStructure.NewRow());
				dataTable = tableStructure.Copy();
			}
			else
			{
				dataTable = dataTable.Clone();
				dataTable.Rows.Add(dataTable.NewRow());
			}
			dataTable.Rows[0]["IncludeBonus"] = "Y";
			dataTable.Rows[0]["IncludePenalty"] = "Y";
			grd_Doc.DataSource = dataTable;
			grd_Doc.RefreshDataSource();
			grd_Doc.DataSourceTable.AcceptChanges();
			_maxLineNumInDb = -1;
			AddNewRow();
			FormMode = Enums.FormViewModeType.Add;
			gv_Doc.EndInsertRow();
			_preAdd = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		gv_Doc.EndInsertRow();
	}

	private void gv_Doc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (!_preAdd && !_isLoading && FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					e.Allow = false;
				}
				else if (grd_Doc.DataSource is DataTable dataTable)
				{
					dataTable.RejectChanges();
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_EmpPenalization));
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.duplicateRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.gc_empNo = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CFL = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_empName = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_empName = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Deparment = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Type = new Apzon.Controls.Grid.GridColumnEx();
		this.gridLuk_Type = new DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit();
		this.repositoryItemGridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEdit1 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.luk_PenalType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.rep_Branch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.repositoryItemMemoExEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.repositoryItemButtonEditCFL1 = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Period = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_empName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Deparment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridLuk_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemGridLookUpEdit1View).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PenalType).BeginInit();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEditCFL1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		base.SuspendLayout();
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(1, 466);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(61, 20);
		this.btn_Save.TabIndex = 85;
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
		this.btnCancel.Location = new System.Drawing.Point(68, 466);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 84;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.grd_Line.AllowPasteExcel = true;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(0, 0);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.repositoryItemDateEdit1, this.luk_PenalType, this.rep_Deparment, this.rep_CFL, this.repositoryItemMemoExEdit1, this.rep_empName, this.gridLuk_Type, this.repositoryItemCalcEdit1 });
		this.grd_Line.Size = new System.Drawing.Size(968, 227);
		this.grd_Line.TabIndex = 83;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.duplicateRowToolStripMenuItem, this.removeRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(151, 48);
		this.duplicateRowToolStripMenuItem.Name = "duplicateRowToolStripMenuItem";
		this.duplicateRowToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
		this.duplicateRowToolStripMenuItem.Text = "Duplicate Row";
		this.duplicateRowToolStripMenuItem.Click += new System.EventHandler(this.duplicateRowToolStripMenuItem_Click);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gv_Line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Line.ColumnKey = "EmpId";
		this.gv_Line.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gc_empNo, this.gridColumnEx13, this.gc_empName, this.gridColumnEx3, this.gc_Type, this.gridColumnEx1, this.gridColumnEx8, this.gridColumnEx5, this.gridColumnEx6 });
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FormId = "oEmpPenalization";
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.IgnoreFilter = false;
		this.gv_Line.IndexEditRow = -1;
		this.gv_Line.IndicatorWidth = 30;
		this.gv_Line.IsApzTable = false;
		this.gv_Line.IsInsertNewRow = true;
		this.gv_Line.IsLoadingData = false;
		this.gv_Line.IsMappingContextAction = false;
		this.gv_Line.ItemId = "APZ_HPT2";
		this.gv_Line.LstColumnAllowBatch = null;
		this.gv_Line.LstColumnNotAllowBatch = null;
		this.gv_Line.LstColumnNotEditable = null;
		this.gv_Line.LstColumNotSetting = null;
		this.gv_Line.Name = "gv_Line";
		this.gv_Line.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_Line.OptionsView.RowAutoHeight = true;
		this.gv_Line.OptionsView.ShowGroupPanel = false;
		this.gv_Line.UseBatchMod = false;
		this.gv_Line.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Line_RowCellStyle);
		this.gv_Line.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_Line_CustomRowCellEdit);
		this.gv_Line.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Line_ShowingEditor);
		this.gv_Line.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Line_CellValueChanged);
		this.gv_Line.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_Line_ValidatingEditor);
		this.gv_Line.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gv_Line_InvalidValueException);
		this.gc_empNo.Caption = "Employee No.";
		this.gc_empNo.ColumnEdit = this.rep_CFL;
		this.gc_empNo.CurrencyDateField = null;
		this.gc_empNo.CurrencyField = null;
		this.gc_empNo.CurrencyFixed = null;
		this.gc_empNo.CurrencyFixedRate = 0.0;
		this.gc_empNo.CurrencyRateField = null;
		this.gc_empNo.FieldName = "EmpId";
		this.gc_empNo.Name = "gc_empNo";
		this.gc_empNo.PrimaryKey = true;
		this.gc_empNo.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gc_empNo.UnitField = null;
		this.gc_empNo.Visible = true;
		this.gc_empNo.VisibleIndex = 0;
		this.gc_empNo.Width = 136;
		this.rep_CFL.AutoHeight = false;
		this.rep_CFL.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_CFL.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
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
		this.rep_CFL.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_CFL_ButtonClick);
		this.gridColumnEx13.Caption = "Extend Employee No.";
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.FieldName = "ExtEmpNo";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.OptionsColumn.ReadOnly = true;
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.UnitField = null;
		this.gridColumnEx13.Visible = true;
		this.gridColumnEx13.VisibleIndex = 1;
		this.gc_empName.Caption = "Employee Name";
		this.gc_empName.ColumnEdit = this.rep_empName;
		this.gc_empName.CurrencyDateField = null;
		this.gc_empName.CurrencyField = null;
		this.gc_empName.CurrencyFixed = null;
		this.gc_empName.CurrencyFixedRate = 0.0;
		this.gc_empName.CurrencyRateField = null;
		this.gc_empName.FieldName = "empName";
		this.gc_empName.Name = "gc_empName";
		this.gc_empName.PrimaryKey = false;
		this.gc_empName.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gc_empName.UnitField = null;
		this.gc_empName.Visible = true;
		this.gc_empName.VisibleIndex = 2;
		this.gc_empName.Width = 112;
		this.rep_empName.AutoHeight = false;
		this.rep_empName.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.rep_empName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_empName.ContainerControl = this;
		this.rep_empName.DisplayMember = "";
		this.rep_empName.DisplayMember2 = "";
		this.rep_empName.LoadDataSourceWhenEnter = false;
		this.rep_empName.Name = "rep_empName";
		this.rep_empName.SeparateCharacterForMulti = ';';
		this.rep_empName.SeparateString = ", ";
		this.rep_empName.UsingAutoCompleteBox = false;
		this.rep_empName.UsingDataSource = true;
		this.rep_empName.ValidateWhenNoDataSource = false;
		this.rep_empName.ValueMember = "";
		this.gridColumnEx3.Caption = "Department";
		this.gridColumnEx3.ColumnEdit = this.rep_Deparment;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "dept";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsColumn.ReadOnly = true;
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 3;
		this.gridColumnEx3.Width = 79;
		this.rep_Deparment.AutoHeight = false;
		this.rep_Deparment.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Deparment.Name = "rep_Deparment";
		this.rep_Deparment.NullText = "";
		this.gc_Type.Caption = "Penal/Bonus Type";
		this.gc_Type.ColumnEdit = this.gridLuk_Type;
		this.gc_Type.CurrencyDateField = null;
		this.gc_Type.CurrencyField = null;
		this.gc_Type.CurrencyFixed = null;
		this.gc_Type.CurrencyFixedRate = 0.0;
		this.gc_Type.CurrencyRateField = null;
		this.gc_Type.FieldName = "PenalizationType";
		this.gc_Type.Name = "gc_Type";
		this.gc_Type.PrimaryKey = false;
		this.gc_Type.UnitField = null;
		this.gc_Type.Visible = true;
		this.gc_Type.VisibleIndex = 5;
		this.gc_Type.Width = 112;
		this.gridLuk_Type.AutoHeight = false;
		this.gridLuk_Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.gridLuk_Type.Name = "gridLuk_Type";
		this.gridLuk_Type.NullText = "";
		this.gridLuk_Type.PopupView = this.repositoryItemGridLookUpEdit1View;
		this.gridLuk_Type.Popup += new System.EventHandler(this.gridLuk_Type_Popup);
		this.gridLuk_Type.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.gridLuk_Type_EditValueChanging);
		this.repositoryItemGridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.repositoryItemGridLookUpEdit1View.Name = "repositoryItemGridLookUpEdit1View";
		this.repositoryItemGridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.repositoryItemGridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
		this.gridColumnEx1.Caption = "Fixed Amount";
		this.gridColumnEx1.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gridColumnEx1.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx1.FieldName = "Amount";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.AllowEdit = false;
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.gridColumnEx8.Caption = "Total Amount";
		this.gridColumnEx8.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gridColumnEx8.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx8.FieldName = "TotalAmount";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 6;
		this.gridColumnEx8.Width = 149;
		this.gridColumnEx5.Caption = "Date";
		this.gridColumnEx5.ColumnEdit = this.repositoryItemDateEdit1;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx5.FieldName = "Date";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 4;
		this.gridColumnEx5.Width = 86;
		this.repositoryItemDateEdit1.AutoHeight = false;
		this.repositoryItemDateEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.IsSearchControl = false;
		this.repositoryItemDateEdit1.Name = "repositoryItemDateEdit1";
		this.gridColumnEx6.Caption = "Note";
		this.gridColumnEx6.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "Note";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 7;
		this.gridColumnEx6.Width = 140;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.luk_PenalType.AutoHeight = false;
		this.luk_PenalType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_PenalType.Name = "luk_PenalType";
		this.luk_PenalType.NullText = "";
		this.luk_PenalType.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_PenalType_EditValueChanging);
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.removeToolStripMenuItem });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(124, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(1, 1);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Line);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(968, 459);
		this.splitContainerControl1.SplitterPosition = 222;
		this.splitContainerControl1.TabIndex = 88;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.rep_Branch.AutoHeight = false;
		this.rep_Branch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Branch.Name = "rep_Branch";
		this.rep_Branch.NullText = "";
		this.repositoryItemMemoExEdit2.AutoHeight = false;
		this.repositoryItemMemoExEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit2.Name = "repositoryItemMemoExEdit2";
		this.repositoryItemMemoExEdit2.ShowIcon = false;
		this.repositoryItemButtonEditCFL1.AutoHeight = false;
		this.repositoryItemButtonEditCFL1.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions3.Image");
		this.repositoryItemButtonEditCFL1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repositoryItemButtonEditCFL1.ContainerControl = this;
		this.repositoryItemButtonEditCFL1.DisplayMember = "";
		this.repositoryItemButtonEditCFL1.DisplayMember2 = "";
		this.repositoryItemButtonEditCFL1.LoadDataSourceWhenEnter = false;
		this.repositoryItemButtonEditCFL1.Name = "repositoryItemButtonEditCFL1";
		this.repositoryItemButtonEditCFL1.ReadOnly = true;
		this.repositoryItemButtonEditCFL1.SeparateCharacterForMulti = ';';
		this.repositoryItemButtonEditCFL1.SeparateString = ", ";
		this.repositoryItemButtonEditCFL1.UsingAutoCompleteBox = false;
		this.repositoryItemButtonEditCFL1.UsingDataSource = true;
		this.repositoryItemButtonEditCFL1.ValidateWhenNoDataSource = false;
		this.repositoryItemButtonEditCFL1.ValueMember = "";
		this.repositoryItemButtonEditCFL1.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_CFLPeriod_ButtonClick);
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gridColumnEx7, this.gc_Period, this.gridColumnEx10, this.gridColumnEx11, this.gridColumnEx9, this.gridColumnEx12, this.gridColumnEx2, this.gridColumnEx4 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oEmpPenalization";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 30;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_HPT1";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.RowAutoHeight = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Doc_FocusedRowChanged);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.gridColumnEx7.Caption = "Doc No.";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "DocNum";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.OptionsColumn.ReadOnly = true;
		this.gridColumnEx7.PrimaryKey = true;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Width = 157;
		this.gc_Period.Caption = "Period";
		this.gc_Period.ColumnEdit = this.repositoryItemButtonEditCFL1;
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
		this.gc_Period.VisibleIndex = 0;
		this.gc_Period.Width = 126;
		this.gridColumnEx10.Caption = "From Date";
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "FromDate";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.OptionsColumn.ReadOnly = true;
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx11.Caption = "To Date";
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "ToDate";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.OptionsColumn.ReadOnly = true;
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx9.Caption = "Branch";
		this.gridColumnEx9.ColumnEdit = this.rep_Branch;
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "Branch";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.OptionsColumn.ReadOnly = true;
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 1;
		this.gridColumnEx12.Caption = "Remark";
		this.gridColumnEx12.ColumnEdit = this.repositoryItemMemoExEdit2;
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "Remarks";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx12.Visible = true;
		this.gridColumnEx12.VisibleIndex = 2;
		this.gridColumnEx12.Width = 121;
		this.gridColumnEx2.Caption = "Include Bonus in Payroll";
		this.gridColumnEx2.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "IncludeBonus";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 3;
		this.gridColumnEx2.Width = 172;
		this.gridColumnEx4.Caption = "Include Penalty in Payroll";
		this.gridColumnEx4.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "IncludePenalty";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 4;
		this.gridColumnEx4.Width = 184;
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip2;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rep_Branch, this.repositoryItemMemoExEdit2, this.repositoryItemButtonEditCFL1, this.repositoryItemCheckEdit1 });
		this.grd_Doc.Size = new System.Drawing.Size(968, 222);
		this.grd_Doc.TabIndex = 86;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(971, 490);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.FormId = "oEmpPenalization";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_EmpPenalization";
		this.Text = "Employee's Penalty/Bonus";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_PenalType_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_empName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Deparment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridLuk_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemGridLookUpEdit1View).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PenalType).EndInit();
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEditCFL1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		base.ResumeLayout(false);
	}
}
