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
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Form;
using Apzon.Controls.Form.ExchangeRateAndIndex;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
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
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Apzon.Forms.Payment;

public class frm_VirPayments : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	protected bool _isChanging;

	protected bool _preAdd;

	protected bool _isLoading;

	protected bool _isCancel;

	protected bool _forceClose;

	public new ApzObjectTypes FormObjectType;

	protected Enums.FormViewModeType _viewMode = Enums.FormViewModeType.View;

	private int _hotTrackRow = int.MinValue;

	private IContainer components;

	private GridViewEx gridViewEx1;

	private RepositoryItemLookUpEditDefineNew repositoryItemLookUpEditDefineNew1;

	private RepositoryItemLookUpEditSimple repositoryItemLookUpEditSimple1;

	private GridViewEx gridViewEx2;

	protected LookUpEditEx cbxStatus;

	public LabelLine lb_DocStatus;

	public LabelLine lb_DocNumber;

	public LabelLine lb_ToDate;

	public LabelLine lb_FromDate;

	public SimpleButtonEx btnSearch;

	protected TextEditEx txtNoOfTrans;

	protected DateEditEx txtToDate;

	protected DateEditEx txtFromDate;

	protected SimpleButtonEx btn_Save;

	protected SimpleButtonEx btnCancel;

	protected SplitContainerControl splitContainerControl1;

	protected GridControlEx grd_Doc;

	protected GridViewEx gv_Doc;

	private GridColumnEx gridColumnEx3;

	protected GridColumnEx col_CardType;

	protected GridColumnEx col_CardCode;

	protected GridColumnEx gridColumnEx5;

	protected GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx1;

	protected GridColumnEx gridColumnEx9;

	protected GridControlEx grd_Line;

	protected GridViewEx gv_Line;

	protected GridColumnEx gridColumnEx7;

	protected GridColumnEx gridColumnEx2;

	private RepositoryItemLookUpEditDefineNew rep_PaymentMethod;

	protected GridColumnEx gridColumnEx4;

	private RepositoryItemLookUpEditSimple rep_CurLine;

	protected GridColumnEx gridColumnEx15;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	protected GridColumnEx gridColumnEx14;

	protected GridColumnEx gridColumnEx13;

	protected GridColumnEx gridColumnEx12;

	protected RepositoryItemMemoExEdit repositoryItemMemoExEdit2;

	private RepositoryItemLookUpEditSimple rep_CardType;

	private RepositoryItemCalcEdit repositoryItemCalcEdit2;

	private RepositoryItemLookUpEditSimple rep_Currency;

	private RepositoryItemDateEditEx repositoryItemDateEditEx2;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx17;

	private GridColumnEx gridColumnEx16;

	private RepositoryItemLookUpEditSimple rep_DocStatus;

	private GridColumnEx gridColumnEx21;

	private GridColumnEx gridColumnEx20;

	private GridColumnEx gridColumnEx19;

	private GridColumnEx gridColumnEx18;

	private GridColumnEx gridColumnEx11;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gridColumnEx22;

	private RepositoryItemCalcEdit repositoryItemCalcEdit3;

	protected ContextMenuStrip ctxDoc;

	protected ToolStripMenuItem addNewToolStripMenuItem;

	protected ContextMenuStrip ctxLine;

	protected ToolStripMenuItem removeRowToolStripMenuItem;

	private SimpleButton btn_PaidAll;

	private int HotTrackRow
	{
		get
		{
			return _hotTrackRow;
		}
		set
		{
			try
			{
				if (_hotTrackRow != value)
				{
					int hotTrackRow = _hotTrackRow;
					_hotTrackRow = value;
					if (gv_Doc.ActiveEditor != null)
					{
						gv_Doc.PostEditor();
					}
					gv_Doc.RefreshRow(hotTrackRow);
					gv_Doc.RefreshRow(_hotTrackRow);
					if (_hotTrackRow >= 0)
					{
						grd_Doc.Cursor = Cursors.Hand;
					}
					else
					{
						grd_Doc.Cursor = Cursors.Default;
					}
				}
			}
			catch (Exception)
			{
			}
		}
	}

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_viewMode = value;
		EnableDisableControls();
	}

	private void EnableDisableControls()
	{
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Find:
				btn_Save.Enabled = true;
				btnCancel.Enabled = true;
				btnSearch.Enabled = true;
				ctxDoc.Enabled = true;
				ctxLine.Enabled = true;
				break;
			case Enums.FormViewModeType.Update:
				btn_Save.Enabled = true;
				btnCancel.Enabled = true;
				btnSearch.Enabled = true;
				ctxDoc.Enabled = true;
				ctxLine.Enabled = true;
				btn_Save.Text = Resources.ButtonUpdate;
				break;
			case Enums.FormViewModeType.Add:
				btn_Save.Enabled = true;
				btnCancel.Enabled = true;
				btnSearch.Enabled = true;
				ctxDoc.Enabled = true;
				ctxLine.Enabled = true;
				btn_Save.Text = Resources.ButtonAdd;
				break;
			case Enums.FormViewModeType.View:
				btn_Save.Enabled = true;
				btnCancel.Enabled = true;
				btnSearch.Enabled = true;
				ctxDoc.Enabled = true;
				ctxLine.Enabled = true;
				btn_Save.Text = Resources.ButtonOK;
				break;
			case Enums.FormViewModeType.Busy:
				btn_Save.Enabled = false;
				btnCancel.Enabled = false;
				btnSearch.Enabled = false;
				ctxDoc.Enabled = false;
				ctxLine.Enabled = false;
				break;
			default:
				btn_Save.Enabled = true;
				btnCancel.Enabled = true;
				btnSearch.Enabled = true;
				ctxDoc.Enabled = true;
				ctxLine.Enabled = true;
				btn_Save.Text = Resources.ButtonOK;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected frm_VirPayments()
	{
		InitializeComponent();
		gv_Doc.RowClick += gv_Doc_RowClick;
		gv_Doc.RowCellStyle += gv_Doc_RowCellStyle;
		gv_Doc.FocusedRowObjectChanged += gv_Doc_FocusedRowObjectChanged;
		gv_Doc.CellValueChanged += gv_Doc_CellValueChanged;
		gv_Doc.BeforeLeaveRow += gv_Doc_BeforeLeaveRow;
		gv_Doc.MouseMove += gv_Doc_MouseMove;
		gv_Doc.MouseLeave += gv_Doc_MouseLeave;
		gv_Doc.CustomRowCellEdit += gv_Doc_CustomRowCellEdit;
		gv_Line.RowClick += gv_Line_RowClick;
		gv_Line.CellValueChanged += gv_Line_CellValueChanged;
		btn_Save.Click += btn_Save_Click;
		btnCancel.Click += btnCancel_Click;
		btnSearch.Click += btnSearch_Click;
		addNewToolStripMenuItem.Click += addNewToolStripMenuItem_Click;
		removeRowToolStripMenuItem.Click += removeRowToolStripMenuItem_Click;
		rep_Currency.EditValueChanged += RepCurrencyOnEditValueChanged;
		rep_CurLine.EditValueChanged += RepCurLineOnEditValueChanged;
		FormClosing += OnFormClosing;
		OnLoadAsync += frm_AdvancePayment_Load;
	}

	private void RepCurLineOnEditValueChanged(object sender, EventArgs eventArgs)
	{
		try
		{
			if (!(sender is LookUpEdit lookUpEdit))
			{
				return;
			}
			string text = Function.ToString(lookUpEdit.EditValue);
			DateTime date = Convert.ToDateTime(gv_Line.GetRowCellValue(gv_Line.FocusedRowHandle, "DocDate").ToString());
			int? docNum = Function.ParseInt(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "DocEntry"));
			if (gv_Doc.GridControl.DataSource is DataTable source)
			{
				DataRow doc = source.AsEnumerable().FirstOrDefault((DataRow x) => Function.ParseInt(x["DocEntry"]) == docNum);
				if (doc == null)
				{
					return;
				}
				if (text.ToUpper() == GlobalData.MainCurrency.ToUpper())
				{
					doc["DocRate"] = 1;
					return;
				}
				double bpRate = HttpClientHelperFactory.GetHttpClient().ExRateAndIndex.GetExchangeRate(text, date);
				if (bpRate > 0.0)
				{
					doc["DocRate"] = Function.ParseDouble(bpRate);
				}
				else if (text != "##" && text.ToUpper() != GlobalData.MainCurrency)
				{
					frm_ExchangeRatesAndIndexs frm = new frm_ExchangeRatesAndIndexs();
					frm.Curr = text;
					frm._date = date;
					frm.FormClosing += (object s, FormClosingEventArgs e1) =>
					{
						bpRate = frm.rate;
						doc["DocRate"] = Function.ParseDouble(bpRate);
					};
					frm.ShowDialog();
				}
				else if (text.ToUpper() == GlobalData.MainCurrency)
				{
					doc["DocRate"] = 1;
				}
			}
			gv_Line.GridControl.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void RepCurrencyOnEditValueChanged(object sender, EventArgs eventArgs)
	{
		try
		{
			if (!(sender is LookUpEdit lookUpEdit))
			{
				return;
			}
			string text = Function.ToString(lookUpEdit.EditValue);
			DateTime date = Convert.ToDateTime(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "DocDate").ToString());
			int? docNum = Function.ParseInt(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "DocEntry"));
			if (gv_Doc.GridControl.DataSource is DataTable source)
			{
				DataRow doc = source.AsEnumerable().FirstOrDefault((DataRow x) => Function.ParseInt(x["DocEntry"]) == docNum);
				if (doc == null)
				{
					return;
				}
				if (text.ToUpper() == GlobalData.MainCurrency.ToUpper())
				{
					doc["DocRate"] = 1;
					return;
				}
				double bpRate = HttpClientHelperFactory.GetHttpClient().ExRateAndIndex.GetExchangeRate(text, date);
				if (bpRate > 0.0)
				{
					doc["DocRate"] = Function.ParseDouble(bpRate);
				}
				else if (text != "##" && text.ToUpper() != GlobalData.MainCurrency)
				{
					frm_ExchangeRatesAndIndexs frm = new frm_ExchangeRatesAndIndexs();
					frm.Curr = text;
					frm._date = date;
					frm.FormClosing += (object s, FormClosingEventArgs e1) =>
					{
						bpRate = frm.rate;
						doc["DocRate"] = Function.ParseDouble(bpRate);
					};
					frm.ShowDialog();
				}
				else if (text.ToUpper() == GlobalData.MainCurrency)
				{
					doc["DocRate"] = 1;
				}
			}
			gv_Doc.GridControl.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void OnFormClosing(object sender, FormClosingEventArgs e)
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
					if (string.IsNullOrEmpty(await SaveProcess()))
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

	private async Task frm_AdvancePayment_Load(object sender, EventArgs e)
	{
		try
		{
			gv_Doc.CellValueChanged += ChangeViewMode;
			gv_Line.CellValueChanged += ChangeViewMode;
			LoadInitForm();
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		await SaveProcess();
	}

	protected virtual async Task<string> SaveProcess()
	{
		try
		{
			if (string.Compare(btn_Save.Text, "OK", StringComparison.OrdinalIgnoreCase) != 0)
			{
				WinCommons._OpenProcessing();
				string text = CheckSave(out var retDoc, out var retLine);
				if (string.IsNullOrEmpty(text))
				{
					DataSet dataSet = new DataSet();
					retLine.TableName = "dtLine";
					retDoc.TableName = "dtDoc";
					dataSet.Tables.Add(retDoc.Copy());
					dataSet.Tables.Add(retLine.Copy());
					string text2 = await Save(dataSet);
					if (string.IsNullOrEmpty(text2))
					{
						return string.Empty;
					}
					WinCommons.Error(Enums.MessageCode.None, text2);
					return text2;
				}
				WinCommons.Error(Enums.MessageCode.None, text);
				return text;
			}
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
			btn_Save.Enabled = true;
		}
		return "Something went wrong!";
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

	private async void btnSearch_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			if (Function.ParseDateTimes(txtToDate.EditValue) < Function.ParseDateTimes(txtFromDate.EditValue) && txtToDate.EditValue != null)
			{
				WinCommons.Error(Enums.MessageCode.FromDateMustBeBefore2Date, "");
				return;
			}
			await REFRESH_DATA();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async void LoadInitForm()
	{
		try
		{
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
			rep_CurLine.Assign(rep_Currency);
			rep_PaymentMethod.FormProperties.TableName = "APZ_OPYM";
			rep_PaymentMethod.ValueMember = "Code";
			rep_PaymentMethod.DisplayMember = "Name";
			rep_PaymentMethod.FormProperties.HideFirstColumn = true;
			rep_PaymentMethod.RefreshDataSource();
			rep_PaymentMethod.BeforeShowForm = (Form frm) =>
			{
				if (frm is frm_DefineNew frm_DefineNew2)
				{
					frm_DefineNew2.Text = "Payment Method - Setup";
					frm_DefineNew2.UseFormSetting = true;
				}
			};
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "O",
					Name = "Open"
				},
				new Common.Table_Code
				{
					Code = "L",
					Name = "Closed"
				},
				new Common.Table_Code
				{
					Code = "C",
					Name = "Cancel"
				}
			};
			rep_DocStatus.DataSource = dataSource;
			rep_DocStatus.DisplayMember = "Name";
			rep_DocStatus.ValueMember = "Code";
			rep_DocStatus.BestFitMode = BestFitMode.BestFitResizePopup;
			cbxStatus.Properties.DataSource = dataSource;
			cbxStatus.Properties.DisplayMember = "Name";
			cbxStatus.Properties.ValueMember = "Code";
			cbxStatus.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			cbxStatus.Properties.PopulateColumns();
			cbxStatus.Properties.Columns["Code"].Visible = false;
			cbxStatus.Properties.UseDropDownRowsAsMaxCount = true;
			List<Common.Table_Code> dataSource2 = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "C",
					Name = "Customer"
				},
				new Common.Table_Code
				{
					Code = "S",
					Name = "Vendor"
				},
				new Common.Table_Code
				{
					Code = "L",
					Name = "Lead"
				},
				new Common.Table_Code
				{
					Code = "E",
					Name = "Employee"
				}
			};
			rep_CardType.DataSource = dataSource2;
			rep_CardType.DisplayMember = "Name";
			rep_CardType.ValueMember = "Code";
			rep_CardType.BestFitMode = BestFitMode.BestFitResizePopup;
			_isLoading = true;
			txtFromDate.EditValue = DateTime.Today.AddDays(-Configuration.SearchDate);
			txtToDate.EditValue = DateTime.Today.AddDays(Configuration.SearchDate);
			cbxStatus.EditValue = "O";
			_isLoading = false;
			await Task.WhenAll(WinFuntions.LoadUdfColumns(gv_Doc, gv_Doc.ItemId), WinFuntions.LoadUdfColumns(gv_Line, gv_Line.ItemId));
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
			if ((FormMode != Enums.FormViewModeType.Update && FormMode != Enums.FormViewModeType.Add) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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

	private void FillData(DataRow dataRow)
	{
		try
		{
			if (dataRow.IsExistsColumn("empID"))
			{
				gv_Line.SetFocusedRowCellValue("CardCode", Function.ToString(dataRow["empID"]));
				gv_Line.SetFocusedRowCellValue("CardName", Function.ToString(dataRow["lastName"]) + ", " + Function.ToString(dataRow["firstName"]));
			}
			if (dataRow.IsExistsColumn("CardCode"))
			{
				gv_Line.SetFocusedRowCellValue("CardCode", Function.ToString(dataRow["CardCode"]));
				gv_Line.SetFocusedRowCellValue("CardName", Function.ToString(dataRow["CardName"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ChangeViewMode(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Find)
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

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		REFRESH();
	}

	private void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (Function.ParseInt(gv_Line.GetFocusedRowCellValue("LineNum")) <= 0)
			{
				return;
			}
			DataTable dataTable = gv_Line.GridControl.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
				gv_Line.UnselectRow(gv_Line.DataRowCount - 1);
				gv_Line.DeleteSelectedRows();
				dataTable.AcceptChanges();
			}
			else
			{
				Logging.Write("WATCH", "Data line is empty");
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
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = (GridViewEx)sender;
			if (_isChanging || e.Column != col_CardType)
			{
				return;
			}
			_isChanging = true;
			gridViewEx.SetRowCellValue(e.RowHandle, "CardCode", "");
			gridViewEx.SetRowCellValue(e.RowHandle, "CardName", "");
			_isChanging = false;
			object value = e.Value;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = new RepositoryItemButtonEditCFL();
			if (Function.ToString(value) != "E")
			{
				repositoryItemButtonEditCFL.ValueMember = "CardCode";
				repositoryItemButtonEditCFL.DisplayMember = "CardCode";
				switch (Function.ToString(value))
				{
				case "C":
					repositoryItemButtonEditCFL.TypeCFL = Enums.TypeCFL.Customer;
					break;
				case "S":
					repositoryItemButtonEditCFL.TypeCFL = Enums.TypeCFL.Vendor;
					break;
				case "L":
					repositoryItemButtonEditCFL.TypeCFL = Enums.TypeCFL.Lead;
					break;
				}
				repositoryItemButtonEditCFL.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			}
			else
			{
				repositoryItemButtonEditCFL.ValueMember = "empID";
				repositoryItemButtonEditCFL.DisplayMember = "empID";
				repositoryItemButtonEditCFL.TypeCFL = Enums.TypeCFL.Employees;
				repositoryItemButtonEditCFL.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			}
			repositoryItemButtonEditCFL.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL.HandleChooseItem, new Action<DataRow>(FillData));
			col_CardCode.ColumnEdit = repositoryItemButtonEditCFL;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_RowClick(object sender, RowClickEventArgs e)
	{
		ApzonCommon.SetColorRowSelected(ref gv_Doc);
	}

	private void gv_Line_RowClick(object sender, RowClickEventArgs e)
	{
		ApzonCommon.SetColorRowSelected(ref gv_Line);
	}

	private void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			int docEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
			LoadLines(docEntry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = (GridViewEx)sender;
			if (e.Column != col_CardCode)
			{
				return;
			}
			object rowCellValue = gridViewEx.GetRowCellValue(e.RowHandle, "CardType");
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = new RepositoryItemButtonEditCFL();
			if (Function.ToString(rowCellValue) != "E")
			{
				repositoryItemButtonEditCFL.ValueMember = "CardCode";
				repositoryItemButtonEditCFL.DisplayMember = "CardCode";
				switch (Function.ToString(rowCellValue))
				{
				case "C":
					repositoryItemButtonEditCFL.TypeCFL = Enums.TypeCFL.Customer;
					break;
				case "S":
					repositoryItemButtonEditCFL.TypeCFL = Enums.TypeCFL.Vendor;
					break;
				case "L":
					repositoryItemButtonEditCFL.TypeCFL = Enums.TypeCFL.Lead;
					break;
				}
				repositoryItemButtonEditCFL.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			}
			else
			{
				repositoryItemButtonEditCFL.ValueMember = "empID";
				repositoryItemButtonEditCFL.DisplayMember = "empID";
				repositoryItemButtonEditCFL.TypeCFL = Enums.TypeCFL.Employees;
				repositoryItemButtonEditCFL.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			}
			repositoryItemButtonEditCFL.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL.HandleChooseItem, new Action<DataRow>(FillData));
			e.RepositoryItem = repositoryItemButtonEditCFL;
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

	private void gv_Doc_MouseLeave(object sender, EventArgs e)
	{
		HotTrackRow = int.MinValue;
	}

	private void gv_Doc_MouseMove(object sender, MouseEventArgs e)
	{
		try
		{
			GridHitInfo gridHitInfo = (sender as GridViewEx).CalcHitInfo(new Point(e.X, e.Y));
			if (gridHitInfo.InRowCell)
			{
				HotTrackRow = gridHitInfo.RowHandle;
			}
			else
			{
				HotTrackRow = int.MinValue;
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
			if (e.RowHandle == HotTrackRow)
			{
				e.Appearance.BackColor = ApzonCommon.colorFocusRow;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual async Task<string> Save(DataSet ds)
	{
		return "";
	}

	protected virtual async Task LoadData()
	{
	}

	protected virtual string CheckSave(out DataTable retDoc, out DataTable retLine)
	{
		retLine = new DataTable();
		retDoc = new DataTable();
		try
		{
			DataTable dataTable = grd_Doc.DataSource as DataTable;
			DataTable dataTable2 = grd_Line.DataSource as DataTable;
			if (!dataTable2.IsNotNull() || !dataTable.IsNotNull())
			{
				return "No data to save.";
			}
			if (dataTable.IsNotNull())
			{
				int focusedDataSourceRowIndex = gv_Doc.GetFocusedDataSourceRowIndex();
				DataRow dataRow = dataTable.Rows[focusedDataSourceRowIndex];
				if (string.IsNullOrEmpty(Function.ToString(dataRow["PeyMethod"])))
				{
					return "Payment Method can not be empty!";
				}
				DataTable dataTable3 = dataTable.Clone();
				dataTable3.ImportRow(dataRow);
				retDoc = dataTable3.Copy();
				retDoc.AcceptChanges();
			}
			if (dataTable2.IsNotNull())
			{
				if (Function.ParseInt(dataTable2.Rows[dataTable2.Rows.Count - 1]["LineNum"]) == -1)
				{
					dataTable2.Rows.RemoveAt(dataTable2.Rows.Count - 1);
					dataTable2.AcceptChanges();
				}
				if (dataTable2.IsNotNull() && dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["CardCode"]))) != null)
				{
					return "CardCode can not be empty!";
				}
				retLine = dataTable2.Copy();
				retLine.AcceptChanges();
			}
			if (!retDoc.IsNotNull())
			{
				return "No data to save.";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return "Something went wrong!";
	}

	private void btn_PaidAll_Click(object sender, EventArgs e)
	{
	}

	protected virtual void AddNewDoc()
	{
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			_preAdd = true;
			gv_Doc.BeginInsertRow();
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			if (grd_Doc.DataSource is DataTable dataTable)
			{
				dataTable.Rows.Clear();
			}
			if (grd_Line.DataSource is DataTable dataTable2)
			{
				dataTable2.Rows.Clear();
			}
			dataSourceTable = (dataSourceTable.IsNotNull() ? dataSourceTable.Clone() : WinFuntions.GetTableStructure(gv_Doc.ItemId).Copy());
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow["Handwrtten"] = "N";
			dataRow["DocStatus"] = "O";
			dataRow["CardType"] = "C";
			dataRow["ObjType"] = (int)FormObjectType;
			dataRow["DocDate"] = DateTime.Today;
			dataRow["DocCur"] = GlobalData.MainCurrency;
			dataRow["DocRate"] = 1;
			dataSourceTable.Rows.Add(dataRow);
			grd_Doc.DataSource = dataSourceTable;
			grd_Doc.RefreshDataSource();
			grd_Doc.DataSourceTable.AcceptChanges();
			AddNewRow();
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

	protected void AddNewRow()
	{
		try
		{
			gv_Line.BeginInsertRow();
			if (grd_Line.DataSource is DataTable dataTable)
			{
				int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("DocCur"));
				DataRow dataRow = dataTable.NewRow();
				dataRow["DocEntry"] = num;
				dataRow["LineNum"] = -1;
				dataRow["ObjType"] = (int)FormObjectType;
				dataRow["Currency"] = (string.IsNullOrWhiteSpace(text) ? GlobalData.MainCurrency : text);
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
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
	}

	protected virtual async Task LoadLines(int docEntry)
	{
	}

	protected virtual async Task<string> Delete(int id)
	{
		try
		{
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ex.Message;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Payment.frm_VirPayments));
		Apzon.Controls.Editor.RepositoryItemCalcEditEx repositoryItemCalcEditEx = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		Apzon.Controls.Editor.RepositoryItemCalcEditEx repositoryItemCalcEditEx2 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.rep_CardType = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.repositoryItemCalcEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.rep_Currency = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.repositoryItemDateEditEx2 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gridViewEx1 = new Apzon.Controls.Grid.GridViewEx();
		this.repositoryItemLookUpEditDefineNew1 = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.repositoryItemLookUpEditSimple1 = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridViewEx2 = new Apzon.Controls.Grid.GridViewEx();
		this.cbxStatus = new Apzon.Controls.Editor.LookUpEditEx();
		this.lb_DocStatus = new Apzon.Controls.Controls.LabelLine();
		this.lb_DocNumber = new Apzon.Controls.Controls.LabelLine();
		this.lb_ToDate = new Apzon.Controls.Controls.LabelLine();
		this.lb_FromDate = new Apzon.Controls.Controls.LabelLine();
		this.btnSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtNoOfTrans = new Apzon.Controls.Editor.TextEditEx();
		this.txtToDate = new Apzon.Controls.Editor.DateEditEx();
		this.txtFromDate = new Apzon.Controls.Editor.DateEditEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctxDoc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.col_CardType = new Apzon.Controls.Grid.GridColumnEx();
		this.col_CardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx16 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_DocStatus = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.ctxLine = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx22 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CurLine = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx19 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx18 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_PaymentMethod = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.btn_PaidAll = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.rep_CardType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Currency).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEditDefineNew1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEditSimple1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbxStatus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtNoOfTrans.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctxDoc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)repositoryItemCalcEditEx).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DocStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		this.ctxLine.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CurLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)repositoryItemCalcEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PaymentMethod).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit2).BeginInit();
		base.SuspendLayout();
		this.rep_CardType.AutoHeight = false;
		this.rep_CardType.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_CardType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_CardType.IsSearchControl = false;
		this.rep_CardType.Name = "rep_CardType";
		this.rep_CardType.NullText = "";
		this.rep_CardType.ShowFooter = false;
		this.rep_CardType.ShowHeader = false;
		this.repositoryItemCalcEdit2.AutoHeight = false;
		this.repositoryItemCalcEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit2.Name = "repositoryItemCalcEdit2";
		this.rep_Currency.AutoHeight = false;
		this.rep_Currency.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_Currency.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Currency.IsSearchControl = false;
		this.rep_Currency.Name = "rep_Currency";
		this.rep_Currency.NullText = "";
		this.rep_Currency.ShowFooter = false;
		this.rep_Currency.ShowHeader = false;
		this.repositoryItemDateEditEx2.AutoHeight = false;
		this.repositoryItemDateEditEx2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx2.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx2.DisplayFormat.FormatString = "";
		this.repositoryItemDateEditEx2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx2.EditFormat.FormatString = "";
		this.repositoryItemDateEditEx2.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx2.IsSearchControl = false;
		this.repositoryItemDateEditEx2.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx2.Name = "repositoryItemDateEditEx2";
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.gridViewEx1.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gridViewEx1.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gridViewEx1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridViewEx1.ColumnKey = null;
		this.gridViewEx1.DisableFormSetting = false;
		this.gridViewEx1.DisplayException = false;
		this.gridViewEx1.EditColumn = null;
		this.gridViewEx1.FieldCurrency = null;
		this.gridViewEx1.FieldRate = null;
		this.gridViewEx1.FormId = "oVirPayment";
		this.gridViewEx1.IndexEditRow = -1;
		this.gridViewEx1.IndicatorWidth = 35;
		this.gridViewEx1.IsApzTable = false;
		this.gridViewEx1.IsInsertNewRow = false;
		this.gridViewEx1.IsLoadingData = false;
		this.gridViewEx1.ItemId = "OGRID1";
		this.gridViewEx1.LstColumnAllowBatch = null;
		this.gridViewEx1.LstColumnNotAllowBatch = null;
		this.gridViewEx1.LstColumNotSetting = null;
		this.gridViewEx1.Name = "gridViewEx1";
		this.gridViewEx1.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gridViewEx1.OptionsView.BestFitMaxRowCount = 100;
		this.gridViewEx1.OptionsView.ColumnAutoWidth = false;
		this.gridViewEx1.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridViewEx1.OptionsView.ShowGroupPanel = false;
		this.gridViewEx1.UseBatchMod = false;
		this.repositoryItemLookUpEditDefineNew1.AutoHeight = false;
		this.repositoryItemLookUpEditDefineNew1.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.repositoryItemLookUpEditDefineNew1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEditDefineNew1.CheckFields = null;
		this.repositoryItemLookUpEditDefineNew1.CheckTables = null;
		this.repositoryItemLookUpEditDefineNew1.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.repositoryItemLookUpEditDefineNew1.Name = "repositoryItemLookUpEditDefineNew1";
		this.repositoryItemLookUpEditDefineNew1.NullText = "";
		this.repositoryItemLookUpEditDefineNew1.ShowFooter = false;
		this.repositoryItemLookUpEditDefineNew1.ShowHeader = false;
		this.repositoryItemLookUpEditSimple1.AutoHeight = false;
		this.repositoryItemLookUpEditSimple1.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.repositoryItemLookUpEditSimple1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEditSimple1.IsSearchControl = false;
		this.repositoryItemLookUpEditSimple1.Name = "repositoryItemLookUpEditSimple1";
		this.repositoryItemLookUpEditSimple1.NullText = "";
		this.repositoryItemLookUpEditSimple1.ShowFooter = false;
		this.repositoryItemLookUpEditSimple1.ShowHeader = false;
		this.gridViewEx2.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gridViewEx2.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gridViewEx2.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridViewEx2.ColumnKey = null;
		this.gridViewEx2.DisableFormSetting = false;
		this.gridViewEx2.DisplayException = false;
		this.gridViewEx2.EditColumn = null;
		this.gridViewEx2.FieldCurrency = null;
		this.gridViewEx2.FieldRate = null;
		this.gridViewEx2.FormId = "oVirPayment";
		this.gridViewEx2.IndexEditRow = -1;
		this.gridViewEx2.IndicatorWidth = 35;
		this.gridViewEx2.IsApzTable = false;
		this.gridViewEx2.IsInsertNewRow = false;
		this.gridViewEx2.IsLoadingData = false;
		this.gridViewEx2.ItemId = "OGRID2";
		this.gridViewEx2.LstColumnAllowBatch = null;
		this.gridViewEx2.LstColumnNotAllowBatch = null;
		this.gridViewEx2.LstColumNotSetting = null;
		this.gridViewEx2.Name = "gridViewEx2";
		this.gridViewEx2.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gridViewEx2.OptionsView.BestFitMaxRowCount = 100;
		this.gridViewEx2.OptionsView.ColumnAutoWidth = false;
		this.gridViewEx2.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridViewEx2.OptionsView.ShowGroupPanel = false;
		this.gridViewEx2.UseBatchMod = false;
		this.cbxStatus.FieldName = null;
		this.cbxStatus.IsSearchControl = false;
		this.cbxStatus.Location = new System.Drawing.Point(313, 25);
		this.cbxStatus.Name = "cbxStatus";
		this.cbxStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbxStatus.Properties.NullText = "";
		this.cbxStatus.Size = new System.Drawing.Size(140, 20);
		this.cbxStatus.TabIndex = 106;
		this.lb_DocStatus.FieldName = null;
		this.lb_DocStatus.HideWhenEmpty = false;
		this.lb_DocStatus.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DocStatus.Location = new System.Drawing.Point(225, 28);
		this.lb_DocStatus.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_DocStatus.Name = "lb_DocStatus";
		this.lb_DocStatus.ShowUnderLine = true;
		this.lb_DocStatus.Size = new System.Drawing.Size(96, 20);
		this.lb_DocStatus.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DocStatus.TabIndex = 114;
		this.lb_DocStatus.Utext = "Doc. Status";
		this.lb_DocNumber.FieldName = null;
		this.lb_DocNumber.HideWhenEmpty = false;
		this.lb_DocNumber.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DocNumber.Location = new System.Drawing.Point(225, 4);
		this.lb_DocNumber.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_DocNumber.Name = "lb_DocNumber";
		this.lb_DocNumber.ShowUnderLine = true;
		this.lb_DocNumber.Size = new System.Drawing.Size(88, 20);
		this.lb_DocNumber.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DocNumber.TabIndex = 113;
		this.lb_DocNumber.Utext = "Doc. Number";
		this.lb_ToDate.FieldName = null;
		this.lb_ToDate.HideWhenEmpty = false;
		this.lb_ToDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ToDate.Location = new System.Drawing.Point(1, 28);
		this.lb_ToDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_ToDate.Name = "lb_ToDate";
		this.lb_ToDate.ShowUnderLine = true;
		this.lb_ToDate.Size = new System.Drawing.Size(80, 20);
		this.lb_ToDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ToDate.TabIndex = 112;
		this.lb_ToDate.Utext = "To Date";
		this.lb_FromDate.FieldName = null;
		this.lb_FromDate.HideWhenEmpty = false;
		this.lb_FromDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_FromDate.Location = new System.Drawing.Point(1, 3);
		this.lb_FromDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_FromDate.Name = "lb_FromDate";
		this.lb_FromDate.ShowUnderLine = true;
		this.lb_FromDate.Size = new System.Drawing.Size(80, 20);
		this.lb_FromDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_FromDate.TabIndex = 111;
		this.lb_FromDate.Utext = "From Date";
		this.btnSearch.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnSearch.Appearance.Options.UseBackColor = true;
		this.btnSearch.FieldName = null;
		this.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		this.btnSearch.IsSearchControl = false;
		this.btnSearch.Location = new System.Drawing.Point(459, 24);
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.Size = new System.Drawing.Size(99, 20);
		this.btnSearch.TabIndex = 107;
		this.btnSearch.Text = "Search";
		this.txtNoOfTrans.FieldName = null;
		this.txtNoOfTrans.IsSearchControl = false;
		this.txtNoOfTrans.Location = new System.Drawing.Point(313, 3);
		this.txtNoOfTrans.Name = "txtNoOfTrans";
		this.txtNoOfTrans.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.txtNoOfTrans.Properties.Appearance.Options.UseFont = true;
		this.txtNoOfTrans.Size = new System.Drawing.Size(140, 18);
		this.txtNoOfTrans.TabIndex = 105;
		this.txtToDate.EditValue = null;
		this.txtToDate.FieldName = null;
		this.txtToDate.Location = new System.Drawing.Point(81, 27);
		this.txtToDate.Name = "txtToDate";
		this.txtToDate.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.txtToDate.Properties.Appearance.Options.UseFont = true;
		this.txtToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtToDate.Properties.IsSearchControl = false;
		this.txtToDate.Size = new System.Drawing.Size(140, 18);
		this.txtToDate.TabIndex = 104;
		this.txtFromDate.EditValue = null;
		this.txtFromDate.FieldName = null;
		this.txtFromDate.Location = new System.Drawing.Point(81, 3);
		this.txtFromDate.Name = "txtFromDate";
		this.txtFromDate.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.txtFromDate.Properties.Appearance.Options.UseFont = true;
		this.txtFromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFromDate.Properties.IsSearchControl = false;
		this.txtFromDate.Size = new System.Drawing.Size(140, 18);
		this.txtFromDate.TabIndex = 103;
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(2, 467);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.Size = new System.Drawing.Size(61, 20);
		this.btn_Save.TabIndex = 110;
		this.btn_Save.Text = "OK";
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(69, 467);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 109;
		this.btnCancel.Text = "Cancel";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(1, 51);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Line);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(968, 410);
		this.splitContainerControl1.SplitterPosition = 222;
		this.splitContainerControl1.TabIndex = 108;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctxDoc;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_DocStatus, this.repositoryItemCalcEdit3 });
		this.grd_Doc.Size = new System.Drawing.Size(968, 222);
		this.grd_Doc.TabIndex = 83;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctxDoc.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.addNewToolStripMenuItem });
		this.ctxDoc.Name = "contextMenuStrip2";
		this.ctxDoc.Size = new System.Drawing.Size(124, 26);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = "EmpId";
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.gridColumnEx3, this.col_CardType, this.col_CardCode, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx8, this.gridColumnEx17, this.gridColumnEx1, this.gridColumnEx9, this.gridColumnEx16 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oVirPayment";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 30;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = true;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.ItemId = "ODOC";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_Doc.OptionsView.RowAutoHeight = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gridColumnEx3.Caption = "Doc Number";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "DocNum";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsColumn.ReadOnly = true;
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.col_CardType.Caption = "Object Type";
		this.col_CardType.ColumnEdit = this.rep_CardType;
		this.col_CardType.CurrencyDateField = null;
		this.col_CardType.CurrencyField = null;
		this.col_CardType.CurrencyFixed = null;
		this.col_CardType.CurrencyFixedRate = 0.0;
		this.col_CardType.CurrencyRateField = null;
		this.col_CardType.FieldName = "CardType";
		this.col_CardType.Name = "col_CardType";
		this.col_CardType.PrimaryKey = false;
		this.col_CardType.UnitField = null;
		this.col_CardType.Visible = true;
		this.col_CardType.VisibleIndex = 0;
		this.col_CardCode.Caption = "Code";
		this.col_CardCode.CurrencyDateField = null;
		this.col_CardCode.CurrencyField = null;
		this.col_CardCode.CurrencyFixed = null;
		this.col_CardCode.CurrencyFixedRate = 0.0;
		this.col_CardCode.CurrencyRateField = null;
		this.col_CardCode.FieldName = "CardCode";
		this.col_CardCode.Name = "col_CardCode";
		this.col_CardCode.PrimaryKey = false;
		this.col_CardCode.UnitField = null;
		this.col_CardCode.Visible = true;
		this.col_CardCode.VisibleIndex = 1;
		this.gridColumnEx5.Caption = "Name";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "CardName";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.OptionsColumn.ReadOnly = true;
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 2;
		this.gridColumnEx5.Width = 68;
		this.gridColumnEx6.Caption = "Amount";
		this.gridColumnEx6.ColumnEdit = this.repositoryItemCalcEdit3;
		this.gridColumnEx6.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = "Currency";
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx6.FieldName = "DocTotal";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 4;
		this.gridColumnEx6.Width = 62;
		this.repositoryItemCalcEdit3.AutoHeight = false;
		this.repositoryItemCalcEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit3.Name = "repositoryItemCalcEdit3";
		this.gridColumnEx8.Caption = "Amount FC";
		this.gridColumnEx8.ColumnEdit = this.repositoryItemCalcEdit3;
		this.gridColumnEx8.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = "DocCur";
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx8.FieldName = "DocTotalFC";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 5;
		this.gridColumnEx8.Width = 79;
		this.gridColumnEx17.Caption = "Exchange Rate";
		repositoryItemCalcEditEx.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		repositoryItemCalcEditEx.IsSearchControl = false;
		this.gridColumnEx17.ColumnEdit = repositoryItemCalcEditEx;
		this.gridColumnEx17.CurrencyDateField = null;
		this.gridColumnEx17.CurrencyField = null;
		this.gridColumnEx17.CurrencyFixed = null;
		this.gridColumnEx17.CurrencyFixedRate = 0.0;
		this.gridColumnEx17.CurrencyRateField = null;
		this.gridColumnEx17.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Rate;
		this.gridColumnEx17.FieldName = "DocRate";
		this.gridColumnEx17.Name = "gridColumnEx17";
		this.gridColumnEx17.PrimaryKey = false;
		this.gridColumnEx17.UnitField = null;
		this.gridColumnEx17.Visible = true;
		this.gridColumnEx17.VisibleIndex = 7;
		this.gridColumnEx17.Width = 80;
		this.gridColumnEx1.Caption = "Currency";
		this.gridColumnEx1.ColumnEdit = this.rep_Currency;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "DocCur";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 8;
		this.gridColumnEx1.Width = 88;
		this.gridColumnEx9.Caption = "Posting Date";
		this.gridColumnEx9.ColumnEdit = this.repositoryItemDateEditEx2;
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx9.FieldName = "DocDate";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 3;
		this.gridColumnEx9.Width = 76;
		this.gridColumnEx16.Caption = "Doc Status";
		this.gridColumnEx16.ColumnEdit = this.rep_DocStatus;
		this.gridColumnEx16.CurrencyDateField = null;
		this.gridColumnEx16.CurrencyField = null;
		this.gridColumnEx16.CurrencyFixed = null;
		this.gridColumnEx16.CurrencyFixedRate = 0.0;
		this.gridColumnEx16.CurrencyRateField = null;
		this.gridColumnEx16.FieldName = "DocStatus";
		this.gridColumnEx16.Name = "gridColumnEx16";
		this.gridColumnEx16.PrimaryKey = false;
		this.gridColumnEx16.UnitField = null;
		this.gridColumnEx16.Visible = true;
		this.gridColumnEx16.VisibleIndex = 6;
		this.rep_DocStatus.AutoHeight = false;
		this.rep_DocStatus.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_DocStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_DocStatus.IsSearchControl = false;
		this.rep_DocStatus.Name = "rep_DocStatus";
		this.rep_DocStatus.NullText = "";
		this.rep_DocStatus.ShowFooter = false;
		this.rep_DocStatus.ShowHeader = false;
		this.grd_Line.AllowPasteExcel = true;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.ContextMenuStrip = this.ctxLine;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(0, 0);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.repositoryItemMemoExEdit2, this.repositoryItemCalcEdit1, this.rep_PaymentMethod, this.rep_CurLine });
		this.grd_Line.Size = new System.Drawing.Size(968, 183);
		this.grd_Line.TabIndex = 86;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		this.ctxLine.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeRowToolStripMenuItem });
		this.ctxLine.Name = "contextMenuStrip1";
		this.ctxLine.Size = new System.Drawing.Size(144, 26);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.gv_Line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Line.ColumnKey = null;
		this.gv_Line.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[14]
		{
			this.gridColumnEx7, this.gridColumnEx22, this.gridColumnEx4, this.gridColumnEx14, this.gridColumnEx15, this.gridColumnEx13, this.gridColumnEx21, this.gridColumnEx20, this.gridColumnEx19, this.gridColumnEx18,
			this.gridColumnEx11, this.gridColumnEx10, this.gridColumnEx2, this.gridColumnEx12
		});
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FormId = "oVirPayment";
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.IndexEditRow = -1;
		this.gv_Line.IndicatorWidth = 30;
		this.gv_Line.IsApzTable = false;
		this.gv_Line.IsInsertNewRow = false;
		this.gv_Line.IsLoadingData = false;
		this.gv_Line.ItemId = "OLINE";
		this.gv_Line.LstColumnAllowBatch = null;
		this.gv_Line.LstColumnNotAllowBatch = null;
		this.gv_Line.LstColumNotSetting = null;
		this.gv_Line.Name = "gv_Line";
		this.gv_Line.OptionsView.RowAutoHeight = true;
		this.gv_Line.OptionsView.ShowGroupPanel = false;
		this.gv_Line.UseBatchMod = false;
		this.gridColumnEx7.Caption = "Line Num";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "LineNum";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.OptionsColumn.ReadOnly = true;
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Width = 80;
		this.gridColumnEx22.Caption = "Invoices / Advance";
		this.gridColumnEx22.CurrencyDateField = null;
		this.gridColumnEx22.CurrencyField = null;
		this.gridColumnEx22.CurrencyFixed = null;
		this.gridColumnEx22.CurrencyFixedRate = 0.0;
		this.gridColumnEx22.CurrencyRateField = null;
		this.gridColumnEx22.Name = "gridColumnEx22";
		this.gridColumnEx22.PrimaryKey = false;
		this.gridColumnEx22.UnitField = null;
		this.gridColumnEx22.Visible = true;
		this.gridColumnEx22.VisibleIndex = 0;
		this.gridColumnEx4.Caption = "Currency";
		this.gridColumnEx4.ColumnEdit = this.rep_CurLine;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "Currency";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 2;
		this.gridColumnEx4.Width = 67;
		this.rep_CurLine.AutoHeight = false;
		this.rep_CurLine.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_CurLine.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_CurLine.IsSearchControl = false;
		this.rep_CurLine.Name = "rep_CurLine";
		this.rep_CurLine.NullText = "";
		this.rep_CurLine.ShowFooter = false;
		this.rep_CurLine.ShowHeader = false;
		this.gridColumnEx14.Caption = "Exchange Rate";
		repositoryItemCalcEditEx2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		repositoryItemCalcEditEx2.IsSearchControl = false;
		this.gridColumnEx14.ColumnEdit = repositoryItemCalcEditEx2;
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Rate;
		this.gridColumnEx14.FieldName = "DocRate";
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.UnitField = null;
		this.gridColumnEx14.Visible = true;
		this.gridColumnEx14.VisibleIndex = 3;
		this.gridColumnEx14.Width = 83;
		this.gridColumnEx15.Caption = "Amount";
		this.gridColumnEx15.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gridColumnEx15.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx15.CurrencyDateField = null;
		this.gridColumnEx15.CurrencyField = "DocCur";
		this.gridColumnEx15.CurrencyFixed = null;
		this.gridColumnEx15.CurrencyFixedRate = 0.0;
		this.gridColumnEx15.CurrencyRateField = null;
		this.gridColumnEx15.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx15.FieldName = "DocTotal";
		this.gridColumnEx15.Name = "gridColumnEx15";
		this.gridColumnEx15.PrimaryKey = false;
		this.gridColumnEx15.UnitField = null;
		this.gridColumnEx15.Visible = true;
		this.gridColumnEx15.VisibleIndex = 1;
		this.gridColumnEx15.Width = 67;
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.gridColumnEx13.Caption = "Amount FC";
		this.gridColumnEx13.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx13.FieldName = "DocTotalFC";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.UnitField = null;
		this.gridColumnEx13.Visible = true;
		this.gridColumnEx13.VisibleIndex = 4;
		this.gridColumnEx13.Width = 67;
		this.gridColumnEx21.Caption = "Paid To Date";
		this.gridColumnEx21.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gridColumnEx21.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gridColumnEx21.CurrencyDateField = null;
		this.gridColumnEx21.CurrencyField = "Currency";
		this.gridColumnEx21.CurrencyFixed = null;
		this.gridColumnEx21.CurrencyFixedRate = 0.0;
		this.gridColumnEx21.CurrencyRateField = null;
		this.gridColumnEx21.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx21.Name = "gridColumnEx21";
		this.gridColumnEx21.PrimaryKey = false;
		this.gridColumnEx21.UnitField = null;
		this.gridColumnEx21.Visible = true;
		this.gridColumnEx21.VisibleIndex = 5;
		this.gridColumnEx21.Width = 76;
		this.gridColumnEx20.Caption = "Paid To Date (FC)";
		this.gridColumnEx20.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gridColumnEx20.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx20.CurrencyDateField = null;
		this.gridColumnEx20.CurrencyField = "Currency";
		this.gridColumnEx20.CurrencyFixed = null;
		this.gridColumnEx20.CurrencyFixedRate = 0.0;
		this.gridColumnEx20.CurrencyRateField = null;
		this.gridColumnEx20.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx20.Name = "gridColumnEx20";
		this.gridColumnEx20.PrimaryKey = false;
		this.gridColumnEx20.UnitField = null;
		this.gridColumnEx20.Visible = true;
		this.gridColumnEx20.VisibleIndex = 6;
		this.gridColumnEx20.Width = 95;
		this.gridColumnEx19.Caption = "Balance To Date";
		this.gridColumnEx19.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gridColumnEx19.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gridColumnEx19.CurrencyDateField = null;
		this.gridColumnEx19.CurrencyField = "Currency";
		this.gridColumnEx19.CurrencyFixed = null;
		this.gridColumnEx19.CurrencyFixedRate = 0.0;
		this.gridColumnEx19.CurrencyRateField = null;
		this.gridColumnEx19.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx19.Name = "gridColumnEx19";
		this.gridColumnEx19.PrimaryKey = false;
		this.gridColumnEx19.UnitField = null;
		this.gridColumnEx19.Visible = true;
		this.gridColumnEx19.VisibleIndex = 7;
		this.gridColumnEx19.Width = 88;
		this.gridColumnEx18.Caption = "Balance To Date (FC)";
		this.gridColumnEx18.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gridColumnEx18.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx18.CurrencyDateField = null;
		this.gridColumnEx18.CurrencyField = "Currency";
		this.gridColumnEx18.CurrencyFixed = null;
		this.gridColumnEx18.CurrencyFixedRate = 0.0;
		this.gridColumnEx18.CurrencyRateField = null;
		this.gridColumnEx18.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx18.Name = "gridColumnEx18";
		this.gridColumnEx18.PrimaryKey = false;
		this.gridColumnEx18.UnitField = null;
		this.gridColumnEx18.Visible = true;
		this.gridColumnEx18.VisibleIndex = 8;
		this.gridColumnEx18.Width = 112;
		this.gridColumnEx11.Caption = "Payment Amount";
		this.gridColumnEx11.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gridColumnEx11.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = "Currency";
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Visible = true;
		this.gridColumnEx11.VisibleIndex = 9;
		this.gridColumnEx11.Width = 92;
		this.gridColumnEx10.Caption = "Payment Amount (FC)";
		this.gridColumnEx10.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gridColumnEx10.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = "Currency";
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 10;
		this.gridColumnEx10.Width = 116;
		this.gridColumnEx2.Caption = "Payment Method";
		this.gridColumnEx2.ColumnEdit = this.rep_PaymentMethod;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "PeyMethod";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 11;
		this.gridColumnEx2.Width = 92;
		this.rep_PaymentMethod.AutoHeight = false;
		this.rep_PaymentMethod.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.rep_PaymentMethod.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_PaymentMethod.CheckFields = null;
		this.rep_PaymentMethod.CheckTables = null;
		this.rep_PaymentMethod.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_PaymentMethod.Name = "rep_PaymentMethod";
		this.rep_PaymentMethod.NullText = "";
		this.rep_PaymentMethod.ShowFooter = false;
		this.rep_PaymentMethod.ShowHeader = false;
		this.gridColumnEx12.Caption = "Remark";
		this.gridColumnEx12.ColumnEdit = this.repositoryItemMemoExEdit2;
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "Comments";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx12.Visible = true;
		this.gridColumnEx12.VisibleIndex = 12;
		this.gridColumnEx12.Width = 100;
		this.repositoryItemMemoExEdit2.AutoHeight = false;
		this.repositoryItemMemoExEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit2.Name = "repositoryItemMemoExEdit2";
		this.repositoryItemMemoExEdit2.ShowIcon = false;
		this.btn_PaidAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_PaidAll.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_PaidAll.ImageOptions.Image");
		this.btn_PaidAll.Location = new System.Drawing.Point(859, 466);
		this.btn_PaidAll.Name = "btn_PaidAll";
		this.btn_PaidAll.Size = new System.Drawing.Size(110, 23);
		this.btn_PaidAll.TabIndex = 115;
		this.btn_PaidAll.Text = "Paid All Selected";
		this.btn_PaidAll.Click += new System.EventHandler(this.btn_PaidAll_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(971, 490);
		base.Controls.Add(this.btn_PaidAll);
		base.Controls.Add(this.cbxStatus);
		base.Controls.Add(this.lb_DocStatus);
		base.Controls.Add(this.lb_DocNumber);
		base.Controls.Add(this.lb_ToDate);
		base.Controls.Add(this.lb_FromDate);
		base.Controls.Add(this.btnSearch);
		base.Controls.Add(this.txtNoOfTrans);
		base.Controls.Add(this.txtToDate);
		base.Controls.Add(this.txtFromDate);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oVirPayment";
		base.Name = "frm_VirPayments";
		base.ShowIcon = false;
		this.Text = "frm_VirPayments";
		((System.ComponentModel.ISupportInitialize)this.rep_CardType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Currency).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEditDefineNew1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEditSimple1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbxStatus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtNoOfTrans.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctxDoc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)repositoryItemCalcEditEx).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DocStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		this.ctxLine.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CurLine).EndInit();
		((System.ComponentModel.ISupportInitialize)repositoryItemCalcEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PaymentMethod).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit2).EndInit();
		base.ResumeLayout(false);
	}
}
