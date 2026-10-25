using System;
using System.Collections.Concurrent;
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
using Apzon.Controls.Commons;
using Apzon.Controls.Compiler;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.Reports;
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

public class frm_SalaryResult : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private readonly Dictionary<int, string> _dicAllowanceName = new Dictionary<int, string>();

	private readonly Dictionary<int, string> _dicDeductionName = new Dictionary<int, string>();

	private readonly Dictionary<string, string> _dicOtName = new Dictionary<string, string>();

	private bool _isChanging;

	private bool _isCancel;

	private bool _forceClose;

	public string ListEmpNo;

	public string Period;

	public int Branch;

	public int Dept;

	public bool ViewMode;

	public bool ReadOnlyResult;

	private GridColumn currentCol;

	private RepositoryItemLookUpEdit rep_gender = new RepositoryItemLookUpEdit();

	private GridGroupSummaryItemCollection grpSumCollection;

	private DataTable dtMainInfo;

	private DataTable dtAllowance;

	private DataTable dtDeduction;

	private DataTable dtOverTime;

	private DataTable dtGrid;

	private ConcurrentBag<DataRow> _mainInfo;

	private ConcurrentBag<DataRow> _allowance;

	private ConcurrentBag<DataRow> _deduction;

	private ConcurrentBag<DataRow> _overTime;

	private IContainer components;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btn_Cancel;

	public GridControlEx grd_Doc;

	public GridViewEx gv_Doc;

	private GridColumnEx gridColumn7;

	private RepositoryItemLookUpEdit rep_DeptAllowance;

	private GridColumnEx gc_empNo;

	private GridColumnEx gridColumn9;

	private GridColumnEx gridColumn1;

	private RepositoryItemLookUpEdit rep_SalCur;

	private GridColumnEx gridColumnEx1;

	private RepositoryItemLookUpEdit rep_Branch;

	private GridColumnEx gridColumnEx2;

	private RepositoryItemLookUpEdit rep_Position;

	private RepositoryItemButtonEditCFL rep_CFL;

	private GridColumnEx gridColumnEx3;

	private RepositoryItemLookUpEdit rep_Status;

	public SimpleButtonEx btn_SendSlip;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx8;

	protected GridViewEx gv_SendMailStt;

	protected GridControlEx grd_SendMailStt;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private GridColumnEx gridColumnEx11;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gridColumnEx9;

	private RepositoryItemLookUpEdit rep_branch1;

	private RepositoryItemLookUpEdit rep_Dept;

	public ProgressBarControl progressSendMail;

	private GridColumnEx gridColumnEx12;

	private LookUpEdit lookFormSetting;

	private LabelLine lbFormSetting;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_SalaryResult()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SalaryCalculation"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SalaryCalculation" && x.authoriza == "F"))
			{
				Controls.EnableDisableControl(isEnable: false);
			}
			FormObjectType = ApzObjectTypes.oSalCalculation;
			base.Text = "Salary Result - Calculate";
			FormId = "oSalaryResult";
			gv_Doc.FormId = FormId;
			gv_Doc.ItemId = "APZ_OSAL";
			gv_Doc.ChangeLogType = Enums.ChangeLog.clSalaryCalculation;
			gv_Doc.IsApzTable = true;
			OnLoadAsync += frm_SalaryCalCulation_Load;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = rep_CFL;
			repositoryItemButtonEditCFL.DrillDownMethodWithGridView = (Action<string, GridView>)Delegate.Combine(repositoryItemButtonEditCFL.DrillDownMethodWithGridView, (Action<string, GridView>)((string input, GridView view) =>
			{
				this.DrillDown(ApzObjectTypes.oEmployeesInfo, Function.ToString(view.GetFocusedRowCellValue("empID")));
			}));
			return;
		}
		throw new Exception(Apzon.Commons.Properties.Resources.Msg_FormAccessDenied);
	}

	private async Task frm_SalaryCalCulation_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient(FormObjectType);
			string formulaText = httpClient.SalaryFormula.GetFormulaText(FormId, Enums.EventId.OnCellValueChanged, Branch, Dept);
			gv_Doc.SyntaxCellValueChanged = ((!string.IsNullOrEmpty(formulaText)) ? ControlFunction.GetToken(formulaText) : new List<Token>());
			formulaText = httpClient.SalaryFormula.GetFormulaText(FormId, Enums.EventId.OnFocusedRowObjectChanged, Branch, Dept);
			gv_Doc.SyntaxFocusedRowObjectChanged = ((!string.IsNullOrEmpty(formulaText)) ? ControlFunction.GetToken(formulaText) : new List<Token>());
			gv_Doc.PopupMenuShowing += GvDocOnPopupMenuShowing;
			gv_Doc.OptionsView.ShowFooter = false;
			gv_Doc.OptionsView.GroupFooterShowMode = GroupFooterShowMode.Hidden;
			gv_Doc.OptionsMenu.EnableFooterMenu = true;
			gv_Doc.OptionsMenu.ShowGroupSummaryEditorItem = false;
			gv_Doc.ShowingEditor += GvDocOnShowingEditor;
			gv_Doc.OptionsBehavior.AlignGroupSummaryInGroupRow = DefaultBoolean.False;
			await LoadInitForm();
			await LoadData();
			await LoadListFormSetting();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GvDocOnShowingEditor(object sender, CancelEventArgs e)
	{
		if (gv_Doc.FocusedColumn.FieldName == "Period" || gv_Doc.FocusedColumn.FieldName == "CalFrom" || gv_Doc.FocusedColumn.FieldName == "CalTo")
		{
			e.Cancel = true;
		}
	}

	private void GvDocOnPopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			if (e.MenuType == GridMenuType.Summary)
			{
				e.Menu.CloseUp += Menu_CloseUp;
				currentCol = e.HitInfo.Column;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Menu_CloseUp(object sender, EventArgs e)
	{
		try
		{
			foreach (GridGroupSummaryItem item in gv_Doc.GroupSummary)
			{
				string text = "";
				if (!(currentCol.FieldName == item.FieldName))
				{
					continue;
				}
				text = item.SummaryType switch
				{
					SummaryItemType.Average => "AVG: ", 
					SummaryItemType.Count => "", 
					_ => item.SummaryType.ToString().ToUpper() + ": ", 
				};
				item.DisplayFormat = text + grpSumCollection.AsEnumerable().FirstOrDefault((GridSummaryItem t) => t.FieldName == item.FieldName)?.Tag;
				foreach (GridSummaryItem item2 in currentCol.Summary)
				{
					string text2 = "";
					item2.DisplayFormat = item2.SummaryType switch
					{
						SummaryItemType.Average => "AVG: ", 
						SummaryItemType.Count => "", 
						_ => item2.SummaryType.ToString().ToUpper() + ": ", 
					} + grpSumCollection.AsEnumerable().FirstOrDefault((GridSummaryItem t) => t.FieldName == item.FieldName)?.Tag;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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
			Logging.Write("ERROR", ex);
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

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					btn_Save.Enabled = false;
					btn_Cancel.Enabled = false;
					await SaveData();
					btn_Save.Enabled = true;
					btn_Cancel.Enabled = true;
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

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		_isCancel = true;
		Close();
		_isCancel = false;
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_isChanging)
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
		try
		{
			if (Function.ParseDecimal(e.CellValue) < 0m && !e.Column.FieldName.StartsWith("U_") && e.Column.FieldName != "Period" && e.Column.FieldName != "StartDate" && e.Column.FieldName != "bankAcount" && !e.Column.FieldName.Contains("Work") && e.Column.FieldName != "branch" && e.Column.FieldName != "dept" && !e.Column.FieldName.Contains("OTWTotal") && !e.Column.FieldName.Contains("position") && !e.Column.FieldName.Contains("Bank") && !e.Column.FieldName.Contains("Remark") && !e.Column.FieldName.Equals("RelatedPerson") && !e.Column.FieldName.Contains("Rat") && e.Column.FieldName != "UpdateDate" && e.Column.FieldName != "UserSign" && e.Column.FieldName != "UserSign2" && e.Column.FieldName != "MailSent" && e.Column.FieldName != "SendMailSuccess" && e.Column.FieldName != "FailReason")
			{
				e.Appearance.ForeColor = Color.Red;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void AfterOpen(EventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(GlobalData.SessionId) || string.IsNullOrEmpty(FormId))
			{
				return;
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			WinCommons._OpenProcessing();
			if (SyntaxAfterOpen == null)
			{
				string formulaText = HttpClientHelperFactory.GetHttpClient(FormObjectType).SalaryFormula.GetFormulaText(FormId, Enums.EventId.OnAfterOpen, Branch, Dept);
				SyntaxAfterOpen = ((!string.IsNullOrEmpty(formulaText)) ? ControlFunction.GetToken(formulaText) : new List<Token>());
			}
			if (SyntaxAfterOpen.Count != 0)
			{
				CtdMessage ctdMessage = new Compiler(Configuration.ApiServer, GlobalData.SessionId).UserdefinedFomular(this, SyntaxAfterOpen, out var _);
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					WinCommons.Warning(Enums.MessageCode.None, ctdMessage.Message);
				}
			}
			WinCommons._CloseProcessing();
			stopwatch.Stop();
			long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			Logging.Write("TRACE", $"- Thời gian tính toán bằng công thức lương (chi nhánh {Function.ParseInt(Branch)}, kỳ {Function.ToString(Period)}) là: " + (double)elapsedMilliseconds * 1.0 / 1000.0 + "s");
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

	protected override void BeforeOpen(out bool cancelEvent)
	{
		try
		{
			if (string.IsNullOrEmpty(GlobalData.SessionId) || string.IsNullOrEmpty(FormId))
			{
				cancelEvent = false;
				return;
			}
			if (SyntaxBeforeOpen == null)
			{
				string formulaText = HttpClientHelperFactory.GetHttpClient(FormObjectType).SalaryFormula.GetFormulaText(FormId, Enums.EventId.OnBeforeOpen, Branch, Dept);
				SyntaxBeforeOpen = ((!string.IsNullOrEmpty(formulaText)) ? ControlFunction.GetToken(formulaText) : new List<Token>());
			}
			if (SyntaxBeforeOpen.Count != 0)
			{
				CtdMessage ctdMessage = new Compiler(Configuration.ApiServer, GlobalData.SessionId).UserdefinedFomular(this, SyntaxBeforeOpen, out cancelEvent);
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					WinCommons.Warning(Enums.MessageCode.None, ctdMessage.Message);
				}
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		cancelEvent = false;
	}

	protected virtual async Task LoadData()
	{
		Stopwatch watch = Stopwatch.StartNew();
		List<string> listOhemCols = new List<string>();
		try
		{
			WinCommons._OpenProcessing();
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("empId", typeof(string));
			dataTable.Columns.Add("Period", typeof(string));
			dataTable.Columns.Add("branch", typeof(int));
			DataRow dataRow = dataTable.NewRow();
			dataRow["empId"] = Function.ToString(ListEmpNo);
			dataRow["Period"] = Function.ToString(Period);
			dataRow["branch"] = Function.ParseInt(Branch);
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataSet dataSet = ((!ViewMode) ? (await _httpClient.SalaryCalculationClient.Calculate(dataTable)) : (await _httpClient.SalaryCalculationClient.ViewSalary(dataTable)));
			DataTable tableStructure = WinFuntions.GetTableStructure("OHEM");
			for (int i = 0; i < tableStructure.Columns.Count; i++)
			{
				listOhemCols.Add(tableStructure.Columns[i].ColumnName);
			}
			gv_Doc.BeginUpdate();
			gv_Doc.OptionsView.ColumnHeaderAutoHeight = DefaultBoolean.True;
			grpSumCollection = new GridGroupSummaryItemCollection(gv_Doc);
			if (dataSet.IsNotNull() && dataSet.Tables[0] != null)
			{
				foreach (DataColumn col in dataSet.Tables[0].Columns)
				{
					if (!(col.ColumnName.ToUpper() != "EMPID") || !(col.ColumnName.ToUpper() != "EMPNAME") || !(col.ColumnName.ToUpper() != "DEPT") || !(col.ColumnName.ToUpper() != "BRANCH") || gv_Doc.Columns.AsEnumerable().FirstOrDefault((GridColumn t) => t.FieldName == col.ColumnName) != null)
					{
						continue;
					}
					RepositoryItemCalcEdit columnEdit = new RepositoryItemCalcEdit();
					if (col.ColumnName == "NetSalary")
					{
						GridColumnEx gridColumnEx = new GridColumnEx
						{
							Caption = col.Caption,
							FieldName = col.ColumnName,
							Visible = true,
							Fixed = FixedStyle.Right,
							Width = 120,
							Currency = Enums.CurrencyColumn.Custom,
							CurrencyField = "SalCur",
							DisplayFormatType = Enums.DisplayFormatType.Amount
						};
						gridColumnEx.ColumnEdit = columnEdit;
						gridColumnEx.OptionsColumn.AllowEdit = true;
						gridColumnEx.OptionsColumn.ReadOnly = true;
						gv_Doc.Columns.Insert(gv_Doc.Columns.Count, gridColumnEx);
						GridGroupSummaryItem gridGroupSummaryItem = new GridGroupSummaryItem
						{
							FieldName = gridColumnEx.FieldName,
							ShowInGroupColumnFooter = gv_Doc.Columns[gridColumnEx.FieldName],
							SummaryType = SummaryItemType.Sum
						};
						gridGroupSummaryItem.DisplayFormat = ((gridGroupSummaryItem.SummaryType != SummaryItemType.Count) ? (gridGroupSummaryItem.SummaryType.ToString().ToUpper() + ": ") : "") + "{0:" + gridColumnEx.DisplayFormat.FormatString + "}";
						gridGroupSummaryItem.Tag = "{0:" + gridColumnEx.DisplayFormat.FormatString + "}";
						GridColumnSummaryItem item = new GridColumnSummaryItem(gridGroupSummaryItem.SummaryType, gridGroupSummaryItem.FieldName, gridGroupSummaryItem.DisplayFormat, gridGroupSummaryItem.Tag);
						gv_Doc.Columns[gridColumnEx.FieldName].Summary.Add(item);
						grpSumCollection.Add(gridGroupSummaryItem);
						gridColumnEx.AllowSummaryMenu = true;
						continue;
					}
					GridGroupSummaryItem gridGroupSummaryItem2 = new GridGroupSummaryItem();
					GridColumnEx gridColumnEx2 = new GridColumnEx
					{
						FieldName = col.ColumnName,
						Width = 120
					};
					string[] array = col.ColumnName.Split('_');
					if (col.ColumnName.Contains("UserSign") || col.ColumnName.Contains("CreateDate") || col.ColumnName.Contains("UpdateDate") || col.ColumnName == "MailSent" || col.ColumnName == "SendMailSuccess" || col.ColumnName == "FailReason" || col.ColumnName == "FromDate" || col.ColumnName == "ToDate" || listOhemCols.Contains(col.ColumnName))
					{
						gridColumnEx2.Visible = false;
					}
					else
					{
						gridColumnEx2.Visible = true;
					}
					if (col.ColumnName.StartsWith("AllowQta_"))
					{
						gridColumnEx2.Caption = array[0] + "_" + Function.ToString(_dicAllowanceName[Function.ParseInt(array[1])]);
					}
					else if (col.ColumnName.StartsWith("EmpRat_") || col.ColumnName.StartsWith("CompRat_") || col.ColumnName.StartsWith("EmpDeducAmt_") || col.ColumnName.StartsWith("CompDeducAmt_"))
					{
						gridColumnEx2.Caption = array[0] + "_" + Function.ToString(_dicDeductionName[Function.ParseInt(array[1])]);
					}
					else if (col.ColumnName.StartsWith("OTAmt_") || col.ColumnName.StartsWith("OTWTotal_"))
					{
						gridColumnEx2.Caption = array[0] + "_" + Function.ToString(_dicOtName[Function.ToString(array[1])]);
					}
					else
					{
						gridColumnEx2.Caption = col.Caption;
					}
					if (col.ColumnName == "RetroactiveSalaryIncrease" || col.ColumnName == "Arrears" || col.ColumnName.Contains("BankTransfer") || col.ColumnName == "Remarks" || col.ColumnName == "Cash" || col.ColumnName.StartsWith("U_"))
					{
						gridColumnEx2.OptionsColumn.ReadOnly = false;
					}
					else
					{
						gridColumnEx2.OptionsColumn.ReadOnly = true;
					}
					if (!col.ColumnName.StartsWith("U_") && col.ColumnName != "Period" && col.ColumnName != "StartDate" && col.ColumnName != "bankAcount" && !col.ColumnName.Contains("Work") && col.ColumnName != "HourPerDay" && col.ColumnName != "ExtEmpNo" && col.ColumnName != "branch" && col.ColumnName != "dept" && !col.ColumnName.Contains("OTWTotal") && !col.ColumnName.Contains("position") && !col.ColumnName.Equals("BankTransfer") && !col.ColumnName.Contains("Remark") && !col.ColumnName.Equals("RelatedPerson") && !col.ColumnName.Contains("Rat") && col.ColumnName != "UpdateDate" && col.ColumnName != "UserSign" && col.ColumnName != "UserSign2" && col.ColumnName != "MailSent" && col.ColumnName != "SendMailSuccess" && col.ColumnName != "FailReason" && col.ColumnName != "LeaveSum" && col.ColumnName != "AbsentSum" && col.ColumnName != "DeptRemarks" && col.ColumnName != "CalFrom" && col.ColumnName != "CalTo" && col.ColumnName != "FromDate" && col.ColumnName != "ToDate" && col.ColumnName != "LineStatus" && col.ColumnName != "LeaveRemain" && col.ColumnName != "IsLastPeriod" && !listOhemCols.Contains(col.ColumnName) && col.ColumnName != "LineNum" && col.ColumnName != "LinesCount" && col.ColumnName != "LateSum" && col.ColumnName != "EarlySum")
					{
						gridColumnEx2.Currency = Enums.CurrencyColumn.Custom;
						gridColumnEx2.CurrencyField = "SalCur";
						gridColumnEx2.DisplayFormatType = Enums.DisplayFormatType.Amount;
						gridColumnEx2.ColumnEdit = columnEdit;
					}
					else if (col.ColumnName.ToUpper().Contains("DATE") || col.ColumnName == "StartDate" || col.ColumnName == "UpdateDate" || col.ColumnName == "CalFrom" || col.ColumnName == "CalTo" || col.ColumnName == "FromDate" || col.ColumnName == "ToDate")
					{
						gridColumnEx2.DisplayFormatType = Enums.DisplayFormatType.DateTime;
					}
					else if ((col.ColumnName.Contains("Work") && !col.ColumnName.StartsWith("U_")) || (col.ColumnName.Contains("OTWTotal") && !col.ColumnName.StartsWith("U_")) || col.ColumnName == "HourPerDay" || col.ColumnName == "LeaveSum" || col.ColumnName == "AbsentSum" || col.ColumnName == "LeaveRemain")
					{
						gridColumnEx2.DisplayFormatType = Enums.DisplayFormatType.Unit;
						gridGroupSummaryItem2.DisplayFormat = "{0:" + gridColumnEx2.DisplayFormat.FormatString + "}";
					}
					else if (col.ColumnName.Contains("Rat") && !col.ColumnName.StartsWith("U_"))
					{
						gridColumnEx2.DisplayFormatType = Enums.DisplayFormatType.Rate;
						gridGroupSummaryItem2.DisplayFormat = "{0:" + gridColumnEx2.DisplayFormat.FormatString + "}";
					}
					else if (col.ColumnName.Equals("RelatedPerson"))
					{
						gridColumnEx2.DisplayFormatType = Enums.DisplayFormatType.Integer;
						gridGroupSummaryItem2.DisplayFormat = "{0:" + gridColumnEx2.DisplayFormat.FormatString + "}";
					}
					if (col.ColumnName.Equals("BankTransfer"))
					{
						RepositoryItemCheckEdit columnEdit2 = new RepositoryItemCheckEdit
						{
							ValueChecked = "Y",
							ValueUnchecked = "N"
						};
						gridColumnEx2.ColumnEdit = columnEdit2;
					}
					if (col.ColumnName.Equals("IsLastPeriod"))
					{
						RepositoryItemCheckEdit columnEdit3 = new RepositoryItemCheckEdit
						{
							ValueChecked = "Y",
							ValueUnchecked = "N"
						};
						gridColumnEx2.ColumnEdit = columnEdit3;
					}
					if (col.ColumnName.Equals("Remarks"))
					{
						RepositoryItemMemoExEdit columnEdit4 = new RepositoryItemMemoExEdit
						{
							WordWrap = true,
							ShowIcon = false
						};
						gridColumnEx2.ColumnEdit = columnEdit4;
					}
					if (col.ColumnName == "SendMailSuccess")
					{
						RepositoryItemCheckEdit columnEdit5 = new RepositoryItemCheckEdit
						{
							ValueChecked = "Y",
							ValueUnchecked = "N"
						};
						gridColumnEx2.ColumnEdit = columnEdit5;
					}
					gv_Doc.Columns.Insert(gv_Doc.Columns.Count, gridColumnEx2);
					if (!string.IsNullOrWhiteSpace(gridGroupSummaryItem2.DisplayFormat))
					{
						gridGroupSummaryItem2.FieldName = gridColumnEx2.FieldName;
						gridGroupSummaryItem2.ShowInGroupColumnFooter = gv_Doc.Columns[gridColumnEx2.FieldName];
						gridGroupSummaryItem2.SummaryType = SummaryItemType.Sum;
						gridGroupSummaryItem2.Tag = gridGroupSummaryItem2.DisplayFormat;
						string displayFormat = (gridGroupSummaryItem2.DisplayFormat = ((gridGroupSummaryItem2.SummaryType != SummaryItemType.Count) ? (gridGroupSummaryItem2.SummaryType.ToString().ToUpper() + ": ") : "") + gridGroupSummaryItem2.DisplayFormat);
						gridGroupSummaryItem2.DisplayFormat = displayFormat;
						GridColumnSummaryItem item2 = new GridColumnSummaryItem(gridGroupSummaryItem2.SummaryType, gridGroupSummaryItem2.FieldName, gridGroupSummaryItem2.DisplayFormat, gridGroupSummaryItem2.Tag);
						gv_Doc.Columns[gridColumnEx2.FieldName].Summary.Add(item2);
						grpSumCollection.Add(gridGroupSummaryItem2);
						gridColumnEx2.AllowSummaryMenu = true;
					}
				}
				gv_Doc.GroupSummary.Assign(grpSumCollection);
				gv_Doc.EndUpdate();
				string[] array2 = ListEmpNo.Split(',');
				DataTable dataTable2 = dataSet.Tables[0];
				dataTable2.SetDefaultAndMaxLen("APZ_OSAL");
				grd_Doc.DataSource = dataTable2;
				grd_Doc.RefreshDataSource();
				IEnumerable<string> collection = (from t in dataSet.Tables[0].AsEnumerable()
					select Function.ToString(t["empID"])).Distinct();
				List<string> list = new List<string>();
				List<string> listB = new List<string>();
				list.AddRange(array2);
				listB.AddRange(collection);
				string text2 = list.Where((string x) => !listB.Contains(x)).ToList().Aggregate("", (string current, string text4) => current + text4 + ",");
				if (array2.Length > dataSet.Tables[0].Rows.Count)
				{
					Logging.Write("TRACE", ViewMode ? $"Some Employee's Salary Result is missing! EmpID = [{text2.Substring(0, text2.Length - 1)}]" : $"Can not calculate for some employee because of missing TimeSheeting! EmpID = [{text2.Substring(0, text2.Length - 1)}]");
				}
				btn_Save.Text = (ViewMode ? Apzon.Commons.Properties.Resources.ButtonOK : Apzon.Commons.Properties.Resources.ButtonSave);
			}
			else
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons.Error(ViewMode ? Enums.MessageCode.NoMatchingRecordFoundPleaseCalculateBeforeViewSalary : Enums.MessageCode.CannotCalculateSalaryBeforeMissingTimeSheeting, "");
			}
			gv_Doc.ExpandAllGroups();
			WinCommons._CloseProcessing();
			watch.Stop();
			string text3 = (ViewMode ? "tính" : "xem");
			long elapsedMilliseconds = watch.ElapsedMilliseconds;
			Logging.Write("TRACE", $"- Thời gian load dữ liệu {text3} lương (chi nhánh {Function.ParseInt(Branch)}, kỳ {Function.ToString(Period)} , số dòng: {dataSet.Tables[0].Rows.Count}) là: " + (double)elapsedMilliseconds * 1.0 / 1000.0 + "s");
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

	public override void FORMSETTING()
	{
		try
		{
			WinCommons._OpenProcessing();
			int settingId = Function.ParseInt(lookFormSetting.EditValue);
			frm_ReportFormSettings frm = new frm_ReportFormSettings("1", grd_Doc, null, gv_Doc.ItemId, FormId, settingId);
			frm.Text = string.Format("{0} - {1}", "Form Settings", Text);
			frm.MdiParent = MdiParent;
			frm.WindowState = FormWindowState.Normal;
			frm.StartPosition = FormStartPosition.CenterScreen;
			frm.FormClosed += async (object s, FormClosedEventArgs v) =>
			{
				if (frm.isSave)
				{
					lookFormSetting.EditValue = null;
					await LoadListFormSetting(settingId);
				}
			};
			frm.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public override async Task ReloadFormSetting(Control ctl, Enums.PivotFormSetting pivotType = Enums.PivotFormSetting.None)
	{
		CustomFormSetting();
	}

	private async void btn_SendSlip_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (string.IsNullOrEmpty(await SendSalarySlip()))
			{
				await SaveSendStatus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual async Task<string> SendSalarySlip()
	{
		return "";
	}

	protected virtual async Task SaveSendStatus()
	{
	}

	private async Task LoadInitForm()
	{
		_ = 6;
		try
		{
			ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
			ToolStripItem toolStripItem = contextMenuStrip.Items.Add("Show Summary");
			toolStripItem.Tag = 0;
			toolStripItem.Click += CtxOnItemClicked;
			ToolStripItem toolStripItem2 = contextMenuStrip.Items.Add("Copy Formsetting To " + (ViewMode ? "Calculate Table" : "View Table"));
			toolStripItem2.Tag = 1;
			toolStripItem2.Click += ItemCopyFormSettingOnClick;
			grd_Doc.ContextMenuStrip = contextMenuStrip;
			gv_Doc.CustomDrawRowIndicator += gv_MainInfo_CustomDrawRowIndicator;
			DataTable dataTable = await _httpClient.EmployeeMasterClient.GetEmployeeStatus();
			rep_Status.DataSource = dataTable.Copy();
			rep_Status.DisplayMember = "Name";
			rep_Status.ValueMember = "Code";
			rep_Status.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Status.ForceInitialize();
			rep_Status.PopulateColumns();
			rep_Status.Columns["Code"].Visible = false;
			DataTable dataTable2 = await _httpClient.EmployeeMasterClient.LoadComboDept();
			rep_DeptAllowance.DataSource = dataTable2.Copy();
			rep_DeptAllowance.DisplayMember = "Name";
			rep_DeptAllowance.ValueMember = "Code";
			rep_DeptAllowance.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_DeptAllowance.ForceInitialize();
			rep_DeptAllowance.PopulateColumns();
			rep_DeptAllowance.Columns["Code"].Visible = false;
			rep_DeptAllowance.Columns["Remarks"].Visible = false;
			rep_Dept.Assign(rep_DeptAllowance);
			DataTable dataTable3 = await _httpClient.EmployeeMasterClient.LoadComboBranch();
			rep_Branch.DataSource = dataTable3.Copy();
			rep_Branch.DisplayMember = "Name";
			rep_Branch.ValueMember = "Code";
			rep_Branch.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Branch.ForceInitialize();
			rep_Branch.PopulateColumns();
			rep_Branch.Columns["Code"].Visible = false;
			rep_branch1.Assign(rep_Branch);
			DataTable dataTable4 = await _httpClient.EmployeeMasterClient.LoadComboPosition();
			rep_Position.DataSource = dataTable4.Copy();
			rep_Position.DisplayMember = "name";
			rep_Position.ValueMember = "posID";
			rep_Position.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Position.ForceInitialize();
			rep_Position.PopulateColumns();
			rep_Position.Columns["posID"].Visible = false;
			List<CurrencyCodesImpl> currencyCodes = GlobalData.CurrencyCodes;
			rep_SalCur.DataSource = currencyCodes;
			rep_SalCur.DisplayMember = "CurrCode";
			rep_SalCur.ValueMember = "CurrCode";
			rep_SalCur.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_SalCur.ForceInitialize();
			rep_SalCur.PopulateColumns();
			foreach (LookUpColumnInfo column in rep_SalCur.Columns)
			{
				column.Visible = column.FieldName == "CurrName";
			}
			DataTable dataTable5 = await _httpClient.AllowanceType.GetAllowanceType();
			if (dataTable5.IsNotNull())
			{
				foreach (DataRow row in dataTable5.Rows)
				{
					_dicAllowanceName.Add(Function.ParseInt(row["Code"]), Function.ToString(row["Name"]));
				}
			}
			DataTable dataTable6 = await _httpClient.DeductionType.GetList();
			if (dataTable6.IsNotNull())
			{
				foreach (DataRow row2 in dataTable6.Rows)
				{
					_dicDeductionName.Add(Function.ParseInt(row2["Code"]), Function.ToString(row2["Name"]));
				}
			}
			using (HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheeting"))
			{
				DataTable dataTable7 = await _help.GetAsync("GetOvertimeType");
				if (dataTable7.IsNotNull())
				{
					EnumerableRowCollection<DataRow> source = from t in dataTable7.AsEnumerable()
						where Function.ParseInt(t["Branch"]) == Branch
						select t;
					if (source.Any())
					{
						DataTable dataTable8 = source.CopyToDataTable();
						if (dataTable8.IsNotNull())
						{
							foreach (DataRow row3 in dataTable8.Rows)
							{
								_dicOtName.Add(Function.ToString(row3["Code"]), Function.ToString(row3["Name"]));
							}
						}
					}
				}
			}
			gc_empNo.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
			rep_CFL.TextEditStyle = TextEditStyles.DisableTextEditor;
			rep_Branch.TextEditStyle = TextEditStyles.DisableTextEditor;
			DataTable dataTable9 = new DataTable();
			dataTable9.Columns.Add("Code", typeof(string));
			dataTable9.Columns.Add("Name", typeof(string));
			dataTable9.Rows.Add("M", "Male");
			dataTable9.Rows.Add("F", "Female");
			dataTable9.Rows.Add("E", "Not Specified");
			rep_gender.ValueMember = "Code";
			rep_gender.DisplayMember = "Name";
			rep_gender.Buttons[0].Visible = false;
			rep_gender.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_gender.DataSource = dataTable9;
			WinFuntions.LoadUdfColumns(gv_Doc, "APZ_OSAL");
			WinFuntions.LoadUdfColumns(gv_Doc, "OHEM");
			WinFuntions.LoadUdfColumns(gv_Doc, "APZ_OHSI");
			gv_Doc.ReadOnly = ReadOnlyResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadListFormSetting(int settingId = 0)
	{
		try
		{
			DataTable dataTable = await HttpClient.FormSettingClient.GetListFormSetting(FormId, gv_Doc.ItemId, GlobalData.UserName);
			lookFormSetting.Properties.DataSource = dataTable;
			if (dataTable.IsNotNull())
			{
				if (settingId > 0 && dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ID"]) == settingId) == null)
				{
					settingId = 0;
				}
				if (settingId == 0)
				{
					DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["SetDefault"]) == 1);
					if (dataRow != null)
					{
						settingId = Function.ParseInt(dataRow["ID"]);
					}
				}
			}
			lookFormSetting.EditValue = settingId;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void ItemCopyFormSettingOnClick(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(lookFormSetting.EditValue);
			if (num == 0)
			{
				WinCommons.Warning(Enums.MessageCode.ChooseASettingBeforeCopy, "");
				return;
			}
			_ = FormId == "oSalaryResult";
			string text = ((FormId == "oSalaryResult") ? "Calculate Table" : "View Table");
			string text2 = ((FormId == "oSalaryResult") ? "View Table" : "Calculate Table");
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCopyFormSettingBetweenFunction, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, text, text2) == DialogResult.Yes)
			{
				string text3 = await _httpClient.FormSettingClient.CopyFormSettingToItem(num);
				if (string.IsNullOrEmpty(text3))
				{
					WinCommons.Success(Enums.MessageCode.CopyFormSettingSuccess, "");
					WinCommons._CloseProcessing();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text3);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CtxOnItemClicked(object sender, EventArgs e)
	{
		try
		{
			ToolStripItem toolStripItem = (ToolStripItem)sender;
			if (toolStripItem != null)
			{
				if (Function.ParseInt(toolStripItem.Tag) == 1)
				{
					gv_Doc.OptionsView.ShowFooter = false;
					gv_Doc.OptionsView.GroupFooterShowMode = GroupFooterShowMode.Hidden;
					gv_Doc.OptionsBehavior.AlignGroupSummaryInGroupRow = DefaultBoolean.False;
					toolStripItem.Tag = 0;
					toolStripItem.Text = "Show Summary";
				}
				else
				{
					gv_Doc.OptionsView.ShowFooter = true;
					gv_Doc.OptionsView.GroupFooterShowMode = GroupFooterShowMode.VisibleAlways;
					gv_Doc.OptionsBehavior.AlignGroupSummaryInGroupRow = DefaultBoolean.True;
					toolStripItem.Tag = 1;
					toolStripItem.Text = "Hide Summary";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void lookFormSetting_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(lookFormSetting.EditValue);
			if (num == 0)
			{
				return;
			}
			WinCommons._OpenProcessing();
			GridMultiSelectMode selectMode = gv_Doc.OptionsSelection.MultiSelectMode;
			if (selectMode == GridMultiSelectMode.CheckBoxRowSelect)
			{
				gv_Doc.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
			}
			DataTable source = grd_Doc.DataSource as DataTable;
			DataTable dataTable = (await _httpClient.FormSettingClient.GetStructure(num, FormId, gv_Doc.ItemId, GlobalData.UserName, GlobalData.DefaultBranch, "1")).Tables[1];
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				GridColumn[] array = gv_Doc.Columns.Where((GridColumn t) => t.Fixed != FixedStyle.None).ToArray();
				for (int num2 = 0; num2 < array.Length; num2++)
				{
					array[num2].Fixed = FixedStyle.None;
				}
				IEnumerable<string> source2 = gv_Doc.Columns.Select((GridColumn t) => t.FieldName).Distinct();
				for (int num3 = 0; num3 < dataTable.Rows.Count; num3++)
				{
					string text = Function.ToString(dataTable.Rows[num3]["ColumnName"]);
					if (text.StartsWith("U_") && (source == null || !source.Columns.Contains(text)) && source != null)
					{
						continue;
					}
					DataRow dataRow = dataTable.Rows[num3];
					FormSetting formSetting = new FormSetting
					{
						ColumnId = text,
						ColumnName = Function.ToString(dataRow["Caption"]),
						Width = Function.ParseInt(dataRow["Width"]),
						Visible = Function.ParseBool(dataRow["Visible"]),
						VisIndex = Function.ParseInt(dataRow["VisIndex"]),
						GroupIndex = Function.ParseInt(dataRow["GroupIndex"]),
						IsReadOnly = Function.ParseBool(dataRow["ReadOnly"]),
						Fixed = Function.ToString(dataRow["Fixed"]),
						NeverEditable = false
					};
					try
					{
						if (!source2.Contains(formSetting.ColumnId))
						{
							if (!formSetting.Visible)
							{
								continue;
							}
							GridColumnEx column = new GridColumnEx
							{
								FieldName = formSetting.ColumnId
							};
							if (gv_Doc.Columns.Count <= formSetting.VisIndex || formSetting.VisIndex < 0)
							{
								gv_Doc.Columns.Add(column);
							}
							else
							{
								gv_Doc.Columns.Insert(formSetting.VisIndex, column);
							}
						}
					}
					catch (Exception)
					{
					}
					if (!(gv_Doc.Columns[formSetting.ColumnId] is GridColumnEx gridColumnEx))
					{
						continue;
					}
					if (!string.IsNullOrEmpty(formSetting.ColumnName))
					{
						gridColumnEx.Caption = formSetting.ColumnName;
					}
					else
					{
						gridColumnEx.Caption = formSetting.ColumnId;
					}
					gridColumnEx.Width = formSetting.Width;
					if (!formSetting.Visible)
					{
						if (gridColumnEx.GroupIndex == 0)
						{
							gridColumnEx.GroupIndex = -1;
							gridColumnEx.Visible = formSetting.Visible;
						}
						else
						{
							gridColumnEx.Visible = formSetting.Visible;
						}
					}
					else
					{
						gridColumnEx.Visible = formSetting.Visible;
						gridColumnEx.Width = formSetting.Width;
						gridColumnEx.GroupIndex = formSetting.GroupIndex;
					}
					if (formSetting.Visible)
					{
						gridColumnEx.VisibleIndex = formSetting.VisIndex;
						gridColumnEx.AbsoluteIndex = formSetting.VisIndex;
					}
					gridColumnEx.OptionsColumn.ReadOnly = formSetting.IsReadOnly;
					gridColumnEx.Fixed = ((formSetting.Fixed == "L") ? FixedStyle.Left : ((formSetting.Fixed == "R") ? FixedStyle.Right : FixedStyle.None));
					gridColumnEx.AllowChSet = formSetting.AllowChSet;
					if (!gridColumnEx.FieldName.StartsWith("U_") || !gridColumnEx.Visible)
					{
						continue;
					}
					string text2 = Function.ToString(dataRow["FormatType"]).ToLower();
					if (!(text2 == "numeric"))
					{
						if (text2 == "datetime")
						{
							gridColumnEx.DisplayFormat.FormatType = FormatType.DateTime;
						}
						else
						{
							gridColumnEx.DisplayFormat.FormatType = FormatType.None;
						}
					}
					else
					{
						gridColumnEx.DisplayFormat.FormatType = FormatType.Numeric;
					}
					if (Function.ParseInt(dataRow["DisplayFormat"]) > 0)
					{
						gridColumnEx.DisplayFormatType = (Enums.DisplayFormatType)Function.ParseInt(dataRow["DisplayFormat"]);
						if (Function.ParseInt(dataRow["DisplayFormat"]) == 3)
						{
							gridColumnEx.Currency = Enums.CurrencyColumn.Custom;
							gridColumnEx.CurrencyField = "SalCur";
						}
					}
					else
					{
						gridColumnEx.DisplayFormat.FormatString = Function.ToString(dataRow["DisplayFormat"]);
					}
				}
			}
			gv_Doc.OptionsSelection.MultiSelectMode = selectMode;
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private void gv_Doc_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.FieldName.Equals("sex"))
			{
				e.RepositoryItem = rep_gender;
			}
			GridColumnEx gridColumnEx = e.Column as GridColumnEx;
			if (gridColumnEx.FieldName.StartsWith("U_") && (gridColumnEx.DisplayFormatType == Enums.DisplayFormatType.Amount || gridColumnEx.DisplayFormatType == Enums.DisplayFormatType.Price))
			{
				gridColumnEx.Currency = Enums.CurrencyColumn.Custom;
				gridColumnEx.CurrencyField = "SalCur";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ProcessRow(DataRow row)
	{
		if (Function.ParseInt(row["empID"]) == 0 || string.IsNullOrEmpty(Function.ToString(row["WorkTotal"])))
		{
			return;
		}
		object value = row["branch"] ?? ((object)(-1));
		DataRow dataRow = dtMainInfo.Clone().NewRow();
		dataRow["empID"] = row["empID"];
		dataRow["Period"] = row["Period"];
		dataRow["CalFrom"] = row["CalFrom"];
		dataRow["CalTo"] = row["CalTo"];
		dataRow["branch"] = value;
		dataRow["WorkQuota"] = row["WorkQuota"];
		dataRow["WorkTotal"] = row["WorkTotal"];
		dataRow["WorkSum"] = row["WorkSum"];
		dataRow["LeaveSum"] = row["LeaveSum"];
		dataRow["AbsentSum"] = row["AbsentSum"];
		dataRow["LeaveRemain"] = row["LeaveRemain"];
		dataRow["LineStatus"] = row["LineStatus"];
		dataRow["BasicSal"] = row["BasicSal"];
		dataRow["SalesSal"] = row["SalesSal"];
		dataRow["SalTotal"] = row["SalTotal"];
		dataRow["SalCur"] = row["SalCur"];
		dataRow["InsuranceSal"] = row["InsuranceSal"];
		dataRow["LineNum"] = row["LineNum"];
		dataRow["LinesCount"] = row["LinesCount"];
		dataRow["LineActualWork"] = row["LineActualWork"];
		dataRow["TotalActualWork"] = row["TotalActualWork"];
		dataRow["CalcWorkTotal"] = row["CalcWorkTotal"];
		dataRow["RealSalary"] = row["RealSalary"];
		dataRow["RealSalTotal"] = row["RealSalTotal"];
		dataRow["TotalAllowance"] = row["TotalAllowance"];
		dataRow["TotalDeduction"] = row["TotalDeduction"];
		dataRow["TotalOT"] = row["TotalOT"];
		dataRow["TaxableIncome"] = row["TaxableIncome"];
		dataRow["NontaxableIncome"] = row["NontaxableIncome"];
		dataRow["RelatedPerson"] = row["RelatedPerson"];
		dataRow["RelatedCalculation"] = row["RelatedCalculation"];
		dataRow["TaxIncAfterFamDeduc"] = row["TaxIncAfterFamDeduc"];
		dataRow["PersonalIncomeTax"] = row["PersonalIncomeTax"];
		dataRow["TotalSalAfterTax"] = row["TotalSalAfterTax"];
		dataRow["Arrears"] = row["Arrears"];
		dataRow["RetroactiveSalaryIncrease"] = row["RetroactiveSalaryIncrease"];
		dataRow["AdvanceAmount"] = row["AdvanceAmount"];
		dataRow["PenalTotal"] = row["PenalTotal"];
		dataRow["BonusTotal"] = row["BonusTotal"];
		dataRow["NetSalary"] = row["NetSalary"];
		dataRow["PITSAL"] = row["PITSAL"];
		dataRow["AmtRelat"] = row["AmtRelat"];
		dataRow["Cash"] = row["Cash"];
		dataRow["BankTransfer"] = row["BankTransfer"];
		dataRow["BankTransferAmt"] = row["BankTransferAmt"];
		dataRow["Remarks"] = row["Remarks"];
		dataRow["LeavePayAmt"] = row["LeavePayAmt"];
		dataRow["TaxableOT"] = row["TaxableOT"];
		dataRow["ExemptOT"] = row["ExemptOT"];
		dataRow["UserSign2"] = GlobalData.UserID;
		dataRow["UpdateDate"] = DateTime.Now;
		if (!ViewMode)
		{
			dataRow["UserSign"] = GlobalData.UserID;
			dataRow["CreateDate"] = DateTime.Now;
		}
		else
		{
			dataRow["UserSign"] = row["UserSign"];
			dataRow["CreateDate"] = row["CreateDate"];
		}
		DataTable dataTable = dtDeduction.Clone();
		DataTable dataTable2 = dtAllowance.Clone();
		DataTable dataTable3 = dtOverTime.Clone();
		foreach (DataColumn column in dtGrid.Columns)
		{
			string[] array = column.ColumnName.Split('_');
			if (array.Length >= 2)
			{
				if (column.ColumnName.Contains("AllowQta"))
				{
					DataRow dataRow2 = dataTable2.NewRow();
					dataRow2["empID"] = row["empID"];
					dataRow2["Period"] = row["Period"];
					dataRow2["QuotaAmt"] = row[column.ColumnName];
					dataRow2["AllowanceType"] = Function.ParseInt(array[1]);
					dataRow2["CalFrom"] = row["CalFrom"];
					dataRow2["CalTo"] = row["CalTo"];
					dataTable2.Rows.Add(dataRow2);
				}
				else if (column.ColumnName.Contains("EmpDeducAmt"))
				{
					DataRow dataRow3 = dataTable.NewRow();
					dataRow3["empID"] = row["empID"];
					dataRow3["Period"] = row["Period"];
					dataRow3["EmpAmount"] = row[column.ColumnName];
					dataRow3["DeducCode"] = Function.ParseInt(array[1]);
					dataRow3["CalFrom"] = row["CalFrom"];
					dataRow3["CalTo"] = row["CalTo"];
					dataTable.Rows.Add(dataRow3);
				}
				else if (column.ColumnName.Contains("CompDeducAmt"))
				{
					DataRow dataRow4 = dataTable.NewRow();
					dataRow4["empID"] = row["empID"];
					dataRow4["Period"] = row["Period"];
					dataRow4["CompAmount"] = row[column.ColumnName];
					dataRow4["DeducCode"] = Function.ParseInt(array[1]);
					dataRow4["CalFrom"] = row["CalFrom"];
					dataRow4["CalTo"] = row["CalTo"];
					dataTable.Rows.Add(dataRow4);
				}
				else if (column.ColumnName.Contains("EmpRat"))
				{
					DataRow dataRow5 = dataTable.NewRow();
					dataRow5["empID"] = row["empID"];
					dataRow5["Period"] = row["Period"];
					dataRow5["EmpRat"] = row[column.ColumnName];
					dataRow5["DeducCode"] = Function.ParseInt(array[1]);
					dataRow5["CalFrom"] = row["CalFrom"];
					dataRow5["CalTo"] = row["CalTo"];
					dataTable.Rows.Add(dataRow5);
				}
				else if (column.ColumnName.Contains("CompRat"))
				{
					DataRow dataRow6 = dataTable.NewRow();
					dataRow6["empID"] = row["empID"];
					dataRow6["Period"] = row["Period"];
					dataRow6["CompRat"] = row[column.ColumnName];
					dataRow6["DeducCode"] = Function.ParseInt(array[1]);
					dataRow6["CalFrom"] = row["CalFrom"];
					dataRow6["CalTo"] = row["CalTo"];
					dataTable.Rows.Add(dataRow6);
				}
				else if (column.ColumnName.Contains("OTAmt"))
				{
					DataRow dataRow7 = dataTable3.NewRow();
					dataRow7["empID"] = row["empID"];
					dataRow7["Period"] = row["Period"];
					dataRow7["OTCode"] = Function.ToString(array[1]);
					dataRow7["OTAmount"] = row[column.ColumnName];
					dataRow7["CalFrom"] = row["CalFrom"];
					dataRow7["CalTo"] = row["CalTo"];
					dataTable3.Rows.Add(dataRow7);
				}
				else if (column.ColumnName.Contains("OTRate"))
				{
					DataRow dataRow8 = dataTable3.NewRow();
					dataRow8["empID"] = row["empID"];
					dataRow8["Period"] = row["Period"];
					dataRow8["OTCode"] = Function.ToString(array[1]);
					dataRow8["OTRate"] = row[column.ColumnName];
					dataRow8["CalFrom"] = row["CalFrom"];
					dataRow8["CalTo"] = row["CalTo"];
					dataTable3.Rows.Add(dataRow8);
				}
				else if (column.ColumnName.Contains("OTWTotal"))
				{
					DataRow dataRow9 = dataTable3.NewRow();
					dataRow9["empID"] = row["empID"];
					dataRow9["Period"] = row["Period"];
					dataRow9["OTCode"] = Function.ToString(array[1]);
					dataRow9["OTWorkTotal"] = row[column.ColumnName];
					dataRow9["CalFrom"] = row["CalFrom"];
					dataRow9["CalTo"] = row["CalTo"];
					dataTable3.Rows.Add(dataRow9);
				}
				else if (column.ColumnName.ToUpper().StartsWith("U_") && dtMainInfo.Columns.Contains(column.ColumnName))
				{
					dataRow[column.ColumnName] = row[column.ColumnName];
				}
			}
		}
		_mainInfo.Add(dataRow);
		foreach (int item in (from t in dataTable.AsEnumerable()
			select Function.ParseInt(t["DeducCode"])).Distinct())
		{
			DataRow dataRow10 = dataTable.NewRow();
			dataRow10["DeducCode"] = item;
			dataRow10["empID"] = row["empID"];
			dataRow10["Period"] = row["Period"];
			dataRow10["CalFrom"] = row["CalFrom"];
			dataRow10["CalTo"] = row["CalTo"];
			int code1 = item;
			DataRow dataRow11 = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["DeducCode"]) == Function.ParseInt(code1) && t["EmpAmount"] != DBNull.Value && Function.ParseInt(t["empID"]) == Function.ParseInt(row["empID"]) && Function.ParseDateTimes(t["CalFrom"]) == Function.ParseDateTimes(row["Calfrom"]));
			if (dataRow11 != null)
			{
				dataRow10["EmpAmount"] = Function.ParseDouble(dataRow11["EmpAmount"]);
			}
			DataRow dataRow12 = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["DeducCode"]) == Function.ParseInt(code1) && t["CompAmount"] != DBNull.Value && Function.ParseInt(t["empID"]) == Function.ParseInt(row["empID"]) && Function.ParseDateTimes(t["CalFrom"]) == Function.ParseDateTimes(row["Calfrom"]));
			if (dataRow12 != null)
			{
				dataRow10["CompAmount"] = Function.ParseDouble(dataRow12["CompAmount"]);
			}
			DataRow dataRow13 = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["DeducCode"]) == Function.ParseInt(code1) && t["EmpRat"] != DBNull.Value && Function.ParseInt(t["empID"]) == Function.ParseInt(row["empID"]) && Function.ParseDateTimes(t["CalFrom"]) == Function.ParseDateTimes(row["Calfrom"]));
			if (dataRow13 != null)
			{
				dataRow10["EmpRat"] = Function.ParseDouble(dataRow13["EmpRat"]);
			}
			DataRow dataRow14 = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["DeducCode"]) == Function.ParseInt(code1) && t["CompRat"] != DBNull.Value && Function.ParseInt(t["empID"]) == Function.ParseInt(row["empID"]) && Function.ParseDateTimes(t["CalFrom"]) == Function.ParseDateTimes(row["Calfrom"]));
			if (dataRow14 != null)
			{
				dataRow10["CompRat"] = Function.ParseDouble(dataRow14["CompRat"]);
			}
			_deduction.Add(dataRow10);
		}
		foreach (int item2 in (from t in dataTable2.AsEnumerable()
			select Function.ParseInt(t["AllowanceType"])).Distinct())
		{
			DataRow dataRow15 = dataTable2.NewRow();
			dataRow15["AllowanceType"] = item2;
			dataRow15["empID"] = row["empID"];
			dataRow15["Period"] = row["Period"];
			dataRow15["CalFrom"] = row["CalFrom"];
			dataRow15["CalTo"] = row["CalTo"];
			int code2 = item2;
			DataRow dataRow16 = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["AllowanceType"]) == Function.ParseInt(code2) && t["QuotaAmt"] != DBNull.Value && Function.ParseInt(t["empID"]) == Function.ParseInt(row["empID"]) && Function.ParseDateTimes(t["CalFrom"]) == Function.ParseDateTimes(row["Calfrom"]));
			if (dataRow16 != null)
			{
				dataRow15["QuotaAmt"] = Function.ParseDouble(dataRow16["QuotaAmt"]);
			}
			_allowance.Add(dataRow15);
		}
		foreach (string item3 in (from t in dataTable3.AsEnumerable()
			select Function.ToString(t["OTCode"])).Distinct())
		{
			DataRow dataRow17 = dataTable3.NewRow();
			dataRow17["OTCode"] = item3;
			dataRow17["empID"] = row["empID"];
			dataRow17["Period"] = row["Period"];
			dataRow17["CalFrom"] = row["CalFrom"];
			dataRow17["CalTo"] = row["CalTo"];
			string code3 = item3;
			DataRow dataRow18 = dataTable3.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["OTCode"]) == Function.ToString(code3) && t["OTAmount"] != DBNull.Value && Function.ParseInt(t["empID"]) == Function.ParseInt(row["empID"]) && Function.ParseDateTimes(t["CalFrom"]) == Function.ParseDateTimes(row["Calfrom"]));
			if (dataRow18 != null)
			{
				dataRow17["OTAmount"] = Function.ParseDouble(dataRow18["OTAmount"]);
			}
			DataRow dataRow19 = dataTable3.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["OTCode"]) == Function.ToString(code3) && t["OTWorkTotal"] != DBNull.Value && Function.ParseInt(t["empID"]) == Function.ParseInt(row["empID"]) && Function.ParseDateTimes(t["CalFrom"]) == Function.ParseDateTimes(row["Calfrom"]));
			if (dataRow19 != null)
			{
				dataRow17["OTWorkTotal"] = Function.ParseDouble(dataRow19["OTWorkTotal"]);
			}
			DataRow dataRow20 = dataTable3.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["OTCode"]) == Function.ToString(code3) && t["OTRate"] != DBNull.Value && Function.ParseInt(t["empID"]) == Function.ParseInt(row["empID"]) && Function.ParseDateTimes(t["CalFrom"]) == Function.ParseDateTimes(row["Calfrom"]));
			if (dataRow20 != null)
			{
				dataRow17["OTRate"] = Function.ParseDouble(dataRow20["OTRate"]);
			}
			_overTime.Add(dataRow17);
		}
	}

	private async Task<string> SaveData()
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			await Task.WhenAll<DataTable>(Task.Run(() => dtMainInfo = WinFuntions.GetTableStructure("APZ_OSAL")), Task.Run(() => dtAllowance = WinFuntions.GetTableStructure("APZ_SAL1")), Task.Run(() => dtDeduction = WinFuntions.GetTableStructure("APZ_SAL2")), Task.Run(() => dtOverTime = WinFuntions.GetTableStructure("APZ_SAL3")));
			_mainInfo = new ConcurrentBag<DataRow>();
			_allowance = new ConcurrentBag<DataRow>();
			_deduction = new ConcurrentBag<DataRow>();
			_overTime = new ConcurrentBag<DataRow>();
			dtGrid = grd_Doc.DataSourceTable.Copy();
			if (dtGrid.IsNotNull())
			{
				if (!Parallel.ForEach(dtGrid.AsEnumerable(), new ParallelOptions
				{
					MaxDegreeOfParallelism = 100
				}, (DataRow row) =>
				{
					ProcessRow(row);
				}).IsCompleted)
				{
					throw new Exception("Could not complete action-parallel prepare data");
				}
				dtMainInfo = _mainInfo.CopyToDataTable();
				dtAllowance = _allowance.CopyToDataTable();
				dtDeduction = _deduction.CopyToDataTable();
				dtOverTime = _overTime.CopyToDataTable();
			}
			dtMainInfo.TableName = "dtMainInfo";
			dtAllowance.TableName = "dtAllowance";
			dtDeduction.TableName = "dtDeduction";
			dtOverTime.TableName = "dtOverTime";
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dtMainInfo.Copy());
			dataSet.Tables.Add(dtAllowance.Copy());
			dataSet.Tables.Add(dtDeduction.Copy());
			dataSet.Tables.Add(dtOverTime.Copy());
			if (dataSet.IsNotNull())
			{
				string text = await _httpClient.SalaryCalculationClient.SaveDoc(dataSet);
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

	public override void CustomFormSetting()
	{
		try
		{
			if (ViewMode)
			{
				return;
			}
			for (int i = 0; i < gv_Doc.Columns.Count; i++)
			{
				GridColumn gridColumn = gv_Doc.Columns[i];
				if (gridColumn.FieldName == "MailSent" || gridColumn.FieldName == "SendMailSuccess" || gridColumn.FieldName == "FailReason")
				{
					gridColumn.Visible = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual async Task REFRESH_DATA()
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.Salary.frm_SalaryResult));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_DeptAllowance = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_empNo = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CFL = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Branch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Position = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_SalCur = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btn_SendSlip = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_SendMailStt = new Apzon.Controls.Grid.GridControlEx();
		this.gv_SendMailStt = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_branch1 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Dept = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.progressSendMail = new DevExpress.XtraEditors.ProgressBarControl();
		this.lbFormSetting = new Apzon.Controls.Controls.LabelLine();
		this.lookFormSetting = new DevExpress.XtraEditors.LookUpEdit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DeptAllowance).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Position).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SalCur).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_SendMailStt).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_SendMailStt).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_branch1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Dept).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.progressSendMail.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookFormSetting.Properties).BeginInit();
		base.SuspendLayout();
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "Save";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(3, 453);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(109, 20);
		this.btn_Save.TabIndex = 95;
		this.btn_Save.Text = "Save";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Cancel.Appearance.Options.UseBackColor = true;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(118, 453);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(93, 20);
		this.btn_Cancel.TabIndex = 94;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Location = new System.Drawing.Point(4, 4);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.rep_DeptAllowance, this.rep_SalCur, this.rep_Branch, this.rep_Position, this.rep_CFL, this.rep_Status });
		this.grd_Doc.Size = new System.Drawing.Size(911, 443);
		this.grd_Doc.TabIndex = 98;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = "empID";
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gridColumn7, this.gc_empNo, this.gridColumn9, this.gridColumnEx1, this.gridColumnEx12, this.gridColumnEx3, this.gridColumnEx2, this.gridColumn1 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oSalaryResult";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.GroupCount = 1;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 40;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_OSAL";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumn7, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_Doc_CustomRowCellEdit);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
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
		this.gc_empNo.Caption = "Employee No.";
		this.gc_empNo.ColumnEdit = this.rep_CFL;
		this.gc_empNo.CurrencyDateField = null;
		this.gc_empNo.CurrencyField = null;
		this.gc_empNo.CurrencyFixed = null;
		this.gc_empNo.CurrencyFixedRate = 0.0;
		this.gc_empNo.CurrencyRateField = null;
		this.gc_empNo.FieldName = "empID";
		this.gc_empNo.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gc_empNo.Name = "gc_empNo";
		this.gc_empNo.OptionsColumn.ReadOnly = true;
		this.gc_empNo.PrimaryKey = false;
		this.gc_empNo.UnitField = null;
		this.gc_empNo.Visible = true;
		this.gc_empNo.VisibleIndex = 0;
		this.gc_empNo.Width = 94;
		this.rep_CFL.AutoHeight = false;
		this.rep_CFL.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_CFL.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_CFL.ContainerControl = this;
		this.rep_CFL.DisplayMember = "";
		this.rep_CFL.DisplayMember2 = "";
		this.rep_CFL.LoadDataSourceWhenEnter = false;
		this.rep_CFL.Name = "rep_CFL";
		this.rep_CFL.ReadOnly = true;
		this.rep_CFL.SeparateCharacterForMulti = ';';
		this.rep_CFL.SeparateString = ", ";
		this.rep_CFL.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Employees;
		this.rep_CFL.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oEmployeesInfo;
		this.rep_CFL.UsingAutoCompleteBox = false;
		this.rep_CFL.UsingDataSource = true;
		this.rep_CFL.ValidateWhenNoDataSource = false;
		this.rep_CFL.ValueMember = "";
		this.gridColumn9.Caption = "Employee Name";
		this.gridColumn9.ColumnEdit = this.rep_CFL;
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
		this.gridColumnEx1.Caption = "Branch";
		this.gridColumnEx1.ColumnEdit = this.rep_Branch;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "branch";
		this.gridColumnEx1.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 2;
		this.rep_Branch.AutoHeight = false;
		this.rep_Branch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Branch.Name = "rep_Branch";
		this.rep_Branch.NullText = "";
		this.gridColumnEx12.Caption = "Line Status";
		this.gridColumnEx12.ColumnEdit = this.rep_Status;
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "LineStatus";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx12.Visible = true;
		this.gridColumnEx12.VisibleIndex = 4;
		this.rep_Status.AutoHeight = false;
		this.rep_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Status.Name = "rep_Status";
		this.rep_Status.NullText = "";
		this.rep_Status.ReadOnly = true;
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
		this.gridColumnEx2.Caption = "Position";
		this.gridColumnEx2.ColumnEdit = this.rep_Position;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "position";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.OptionsColumn.ReadOnly = true;
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 3;
		this.rep_Position.AutoHeight = false;
		this.rep_Position.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Position.Name = "rep_Position";
		this.rep_Position.NullText = "";
		this.gridColumn1.Caption = "Currency";
		this.gridColumn1.ColumnEdit = this.rep_SalCur;
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "SalCur";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.rep_SalCur.AutoHeight = false;
		this.rep_SalCur.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SalCur.Name = "rep_SalCur";
		this.rep_SalCur.NullText = "";
		this.btn_SendSlip.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_SendSlip.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_SendSlip.Appearance.Options.UseBackColor = true;
		this.btn_SendSlip.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_SendSlip.EditControl = null;
		this.btn_SendSlip.EditValue = "Send Salary Slip";
		this.btn_SendSlip.FieldName = null;
		this.btn_SendSlip.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_SendSlip.ImageOptions.Image");
		this.btn_SendSlip.IsSearchControl = false;
		this.btn_SendSlip.Location = new System.Drawing.Point(217, 453);
		this.btn_SendSlip.Name = "btn_SendSlip";
		this.btn_SendSlip.ReadOnly = false;
		this.btn_SendSlip.SettingInVisible = false;
		this.btn_SendSlip.SettingReadonly = false;
		this.btn_SendSlip.Size = new System.Drawing.Size(109, 20);
		this.btn_SendSlip.TabIndex = 99;
		this.btn_SendSlip.Text = "Send Salary Slip";
		this.btn_SendSlip.Visible = false;
		this.btn_SendSlip.Click += new System.EventHandler(this.btn_SendSlip_Click);
		this.grd_SendMailStt.AllowPasteExcel = false;
		this.grd_SendMailStt.ClipboardData = null;
		this.grd_SendMailStt.Location = new System.Drawing.Point(405, 87);
		this.grd_SendMailStt.MainView = this.gv_SendMailStt;
		this.grd_SendMailStt.Name = "grd_SendMailStt";
		this.grd_SendMailStt.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repositoryItemCheckEdit1, this.rep_Dept, this.rep_branch1 });
		this.grd_SendMailStt.Size = new System.Drawing.Size(341, 150);
		this.grd_SendMailStt.TabIndex = 100;
		this.grd_SendMailStt.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_SendMailStt });
		this.grd_SendMailStt.Visible = false;
		this.gv_SendMailStt.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_SendMailStt.ColumnKey = null;
		this.gv_SendMailStt.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gridColumnEx4, this.gridColumnEx8, this.gridColumnEx11, this.gridColumnEx10, this.gridColumnEx9, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7 });
		this.gv_SendMailStt.DisableFormSetting = false;
		this.gv_SendMailStt.DisplayException = false;
		this.gv_SendMailStt.EditColumn = null;
		this.gv_SendMailStt.FieldCurrency = null;
		this.gv_SendMailStt.FieldRate = null;
		this.gv_SendMailStt.FormId = "oSalaryResult";
		this.gv_SendMailStt.GridControl = this.grd_SendMailStt;
		this.gv_SendMailStt.GroupCount = 1;
		this.gv_SendMailStt.IgnoreFilter = false;
		this.gv_SendMailStt.IndexEditRow = -1;
		this.gv_SendMailStt.IndicatorWidth = 35;
		this.gv_SendMailStt.IsApzTable = false;
		this.gv_SendMailStt.IsInsertNewRow = false;
		this.gv_SendMailStt.IsLoadingData = false;
		this.gv_SendMailStt.IsMappingContextAction = false;
		this.gv_SendMailStt.ItemId = "APZ_SAL1";
		this.gv_SendMailStt.LstColumnAllowBatch = null;
		this.gv_SendMailStt.LstColumnNotAllowBatch = null;
		this.gv_SendMailStt.LstColumnNotEditable = null;
		this.gv_SendMailStt.LstColumNotSetting = null;
		this.gv_SendMailStt.Name = "gv_SendMailStt";
		this.gv_SendMailStt.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_SendMailStt.OptionsView.BestFitMaxRowCount = 100;
		this.gv_SendMailStt.OptionsView.ColumnAutoWidth = false;
		this.gv_SendMailStt.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_SendMailStt.OptionsView.ShowGroupPanel = false;
		this.gv_SendMailStt.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumnEx9, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gv_SendMailStt.UseBatchMod = false;
		this.gridColumnEx4.Caption = "Employee No.";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "empID";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 3;
		this.gridColumnEx8.Caption = "Employee Name";
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "empName";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 4;
		this.gridColumnEx11.Caption = "Branch";
		this.gridColumnEx11.ColumnEdit = this.rep_branch1;
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "branch";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Visible = true;
		this.gridColumnEx11.VisibleIndex = 2;
		this.rep_branch1.AutoHeight = false;
		this.rep_branch1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_branch1.Name = "rep_branch1";
		this.gridColumnEx10.Caption = "Period";
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "Period";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 1;
		this.gridColumnEx9.Caption = "Department";
		this.gridColumnEx9.ColumnEdit = this.rep_Dept;
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "dept";
		this.gridColumnEx9.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 0;
		this.rep_Dept.AutoHeight = false;
		this.rep_Dept.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Dept.Name = "rep_Dept";
		this.gridColumnEx5.Caption = "E-mail";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "MailSent";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 5;
		this.gridColumnEx6.Caption = "Send Sucessfully!";
		this.gridColumnEx6.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "SendMailSuccess";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 6;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.gridColumnEx7.Caption = "Reason Of Failure";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "FailReason";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 7;
		this.progressSendMail.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.progressSendMail.Location = new System.Drawing.Point(332, 453);
		this.progressSendMail.Name = "progressSendMail";
		this.progressSendMail.Properties.FlowAnimationEnabled = true;
		this.progressSendMail.Properties.PercentView = false;
		this.progressSendMail.Properties.ReadOnly = true;
		this.progressSendMail.Properties.ShowTitle = true;
		this.progressSendMail.ShowProgressInTaskBar = true;
		this.progressSendMail.Size = new System.Drawing.Size(270, 20);
		this.progressSendMail.TabIndex = 101;
		this.progressSendMail.Visible = false;
		this.lbFormSetting.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.lbFormSetting.EditControl = null;
		this.lbFormSetting.EditValue = "Form Setting";
		this.lbFormSetting.FieldName = null;
		this.lbFormSetting.HideWhenEmpty = false;
		this.lbFormSetting.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbFormSetting.Location = new System.Drawing.Point(674, 457);
		this.lbFormSetting.Name = "lbFormSetting";
		this.lbFormSetting.ReadOnly = false;
		this.lbFormSetting.SettingInVisible = false;
		this.lbFormSetting.SettingReadonly = false;
		this.lbFormSetting.ShowUnderLine = true;
		this.lbFormSetting.Size = new System.Drawing.Size(72, 20);
		this.lbFormSetting.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbFormSetting.TabIndex = 102;
		this.lbFormSetting.Utext = "Form Setting";
		this.lookFormSetting.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.lookFormSetting.Location = new System.Drawing.Point(747, 454);
		this.lookFormSetting.Name = "lookFormSetting";
		this.lookFormSetting.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookFormSetting.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.lookFormSetting.Properties.DisplayMember = "Name";
		this.lookFormSetting.Properties.NullText = "";
		this.lookFormSetting.Properties.ValueMember = "ID";
		this.lookFormSetting.Size = new System.Drawing.Size(168, 20);
		this.lookFormSetting.TabIndex = 103;
		this.lookFormSetting.EditValueChanged += new System.EventHandler(this.lookFormSetting_EditValueChanged);
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(919, 481);
		base.Controls.Add(this.lookFormSetting);
		base.Controls.Add(this.lbFormSetting);
		base.Controls.Add(this.progressSendMail);
		base.Controls.Add(this.grd_SendMailStt);
		base.Controls.Add(this.btn_SendSlip);
		base.Controls.Add(this.grd_Doc);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btn_Cancel);
		base.FormId = "oSalaryResult";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SalaryResult";
		base.ShowInTaskbar = false;
		this.Text = "Salary Calculation - Result";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_SalaryInfo_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DeptAllowance).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Position).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SalCur).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_SendMailStt).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_SendMailStt).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_branch1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Dept).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.progressSendMail.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookFormSetting.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
