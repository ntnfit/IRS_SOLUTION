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
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
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

public class frm_AdvancePayment : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	protected bool _isChanging;

	protected bool _preAdd;

	protected bool _isLoading;

	protected bool _isCancel;

	protected bool _forceClose;

	private int _hotTrackRow = int.MinValue;

	private IContainer components;

	protected SplitContainerControl splitContainerControl1;

	protected GridControlEx grd_Doc;

	protected GridViewEx gv_Doc;

	protected GridColumnEx gridColumnEx7;

	protected GridColumnEx gridColumnEx12;

	protected RepositoryItemMemoExEdit repositoryItemMemoExEdit2;

	protected GridControlEx grd_Line;

	protected GridViewEx gv_Line;

	protected SimpleButtonEx btn_Save;

	protected SimpleButtonEx btnCancel;

	protected GridColumnEx gridColumnEx2;

	protected GridColumnEx gridColumnEx4;

	protected GridColumnEx gridColumnEx15;

	protected GridColumnEx gridColumnEx14;

	protected GridColumnEx gridColumnEx13;

	protected GridColumnEx gridColumnEx11;

	protected GridColumnEx gridColumnEx10;

	protected GridColumnEx col_CardType;

	protected GridColumnEx col_CardCode;

	protected GridColumnEx gridColumnEx5;

	protected GridColumnEx gridColumnEx6;

	protected GridColumnEx gridColumnEx9;

	protected GridColumnEx gridColumnEx16;

	protected RepositoryItemDateEditEx repositoryItemDateEditEx1;

	protected RepositoryItemDateEditEx repositoryItemDateEditEx2;

	protected RepositoryItemCalcEdit repositoryItemCalcEdit1;

	protected RepositoryItemCalcEdit repositoryItemCalcEdit2;

	protected RepositoryItemLookUpEditDefineNew rep_PaymentMethod;

	protected RepositoryItemLookUpEditSimple rep_DocStatus;

	protected RepositoryItemLookUpEditSimple rep_Currency;

	protected RepositoryItemLookUpEditSimple rep_CardType;

	protected GridColumnEx gridColumnEx1;

	protected RepositoryItemLookUpEditSimple rep_CurLine;

	protected ContextMenuStrip ctxDoc;

	protected ToolStripMenuItem addNewToolStripMenuItem;

	protected ContextMenuStrip ctxLine;

	protected ToolStripMenuItem removeRowToolStripMenuItem;

	protected LabelLine lb_DocStatus;

	protected LabelLine lb_DocNumber;

	protected LabelLine lb_ToDate;

	protected LabelLine lb_FromDate;

	protected SimpleButtonEx btnSearch;

	protected TextEditEx txtNoOfTrans;

	protected DateEditEx txtToDate;

	protected DateEditEx txtFromDate;

	protected LookUpEditEx cbxStatus;

	protected GridColumnEx gridColumnEx3;

	protected RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	protected GridColumnEx gridColumnEx8;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

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
		_formMode = value;
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

	protected frm_AdvancePayment()
	{
		InitializeComponent();
		gv_Doc.RowClick += gv_Doc_RowClick;
		gv_Doc.RowCellStyle += gv_Doc_RowCellStyle;
		gv_Doc.FocusedRowObjectChanged += gv_Doc_FocusedRowObjectChanged;
		gv_Doc.CellValueChanged += gv_Doc_CellValueChanged;
		gv_Doc.BeforeLeaveRow += gv_Doc_BeforeLeaveRow;
		gv_Doc.MouseMove += gv_Doc_MouseMove;
		gv_Doc.MouseLeave += gv_Doc_MouseLeave;
		gv_Line.RowClick += gv_Line_RowClick;
		gv_Line.CustomRowCellEdit += gv_Line_CustomRowCellEdit;
		gv_Line.CellValueChanged += gv_Line_CellValueChanged;
		btn_Save.Click += btn_Save_Click;
		btnCancel.Click += btnCancel_Click;
		btnSearch.Click += btnSearch_Click;
		addNewToolStripMenuItem.Click += addNewToolStripMenuItem_Click;
		removeRowToolStripMenuItem.Click += removeRowToolStripMenuItem_Click;
		rep_Currency.EditValueChanged += RepCurrencyOnEditValueChanged;
		FormClosing += OnFormClosing;
		OnLoadAsync += frm_AdvancePayment_Load;
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
			_isCancel = true;
			Close();
			_isCancel = false;
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
			Task task = WinFuntions.LoadUdfColumns(gv_Doc, gv_Doc.ItemId);
			Task task2 = WinFuntions.LoadUdfColumns(gv_Line, gv_Line.ItemId);
			CreateColumnsDistRule(gv_Line);
			await Task.WhenAll(task, task2);
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

	private static void CreateColumnsDistRule(GridViewEx gv)
	{
		try
		{
			if (GlobalData.Dimensions == null || GlobalData.Dimensions.Rows.Count <= 0)
			{
				return;
			}
			foreach (DataRow row in GlobalData.Dimensions.Rows)
			{
				AddDistRule(gv, row);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private static void AddDistRule(GridViewEx gv, DataRow r, string fld = "", string caption = "")
	{
		try
		{
			string code = ((r["DimCode"] == null || r["DimCode"] == DBNull.Value) ? "" : r["DimCode"].ToString());
			if (string.IsNullOrWhiteSpace(code))
			{
				return;
			}
			if (code == "1")
			{
				code = "";
			}
			string fieldName = string.Format("{0}{1}", "OcrCode" + fld, code);
			RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit.Name = "rep_line_" + fieldName;
			repositoryItemButtonEdit.Buttons[0].Kind = ButtonPredefines.Search;
			repositoryItemButtonEdit.ButtonClick += (object s, ButtonPressedEventArgs e) =>
			{
				try
				{
					if (!gv.Columns[fieldName].OptionsColumn.ReadOnly)
					{
						frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(code switch
						{
							"2" => Enums.TypeCFL.OrcCode2, 
							"3" => Enums.TypeCFL.OrcCode3, 
							"4" => Enums.TypeCFL.OrcCode4, 
							"5" => Enums.TypeCFL.OrcCode5, 
							_ => Enums.TypeCFL.OrcCode, 
						}, multiSelect: false, "");
						frm_ChooseFromList2.ShowDialog(gv.GridControl.FindForm());
						if (frm_ChooseFromList2.ChooseItem != null)
						{
							DataRow row = ((DataRowView)frm_ChooseFromList2.ChooseItem).Row;
							gv.SetRowCellValue(gv.FocusedRowHandle, fieldName, row[0]);
						}
					}
				}
				catch (Exception ex2)
				{
					WinCommons.ErrorLineNumber(ex2);
				}
			};
			GridColumn gridColumn = gv.Columns[fieldName];
			if (gridColumn == null)
			{
				gridColumn = new GridColumnEx();
				gridColumn.FieldName = fieldName;
				gridColumn.Caption = string.Format("{0} {1}", caption, Function.ToString(r["DimDesc"]));
				gridColumn.Visible = true;
				gridColumn.Width = 100;
				gv.Columns.Add(gridColumn);
			}
			gridColumn.ColumnEdit = repositoryItemButtonEdit;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (frm_ExchangeRatesAndIndexs.isClosing || e.RowHandle < 0)
			{
				return;
			}
			double num = Function.ParseDouble(gv_Doc.GetFocusedRowCellValue("DocRate"));
			Function.ParseDouble(gv_Doc.GetFocusedRowCellValue("DocTotal"));
			Function.ParseDouble(gv_Doc.GetFocusedRowCellValue("DocTotalFC"));
			DateTime date = ((gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "DocDate") == DBNull.Value) ? DateTime.Today : Convert.ToDateTime(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "DocDate").ToString()));
			if (!e.Column.FieldName.ToUpper().Equals("DOCCUR") && !e.Column.FieldName.ToUpper().Equals("DOCDATE"))
			{
				return;
			}
			string text = Function.ToString(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "DocCur"));
			if (string.IsNullOrEmpty(text))
			{
				text = GlobalData.MainCurrency;
			}
			foreach (DataRow row in ((DataTable)gv_Line.GridControl.DataSource).Rows)
			{
				double lineRate = Function.ParseDouble(row["Rate"]);
				double num2 = Function.ParseDouble(row["LineTotal"]);
				string text2 = Function.ToString(row["Currency"]);
				double num3 = Function.ParseDouble(row["TotalFrgn"]);
				if (text2.ToUpper().Equals(text.ToUpper()))
				{
					lineRate = num;
					num2 *= Math.Pow(num, GlobalData.ExchangeRatePow);
					num3 = ((!(text.ToUpper() == GlobalData.MainCurrency.ToUpper())) ? (num2 * Math.Pow(lineRate / num, GlobalData.ExchangeRatePow)) : 0.0);
				}
				else
				{
					double exchangeRate = GetExchangeRate(text2, date);
					if (Math.Abs(exchangeRate) < double.Epsilon)
					{
						frm_ExchangeRatesAndIndexs frm = new frm_ExchangeRatesAndIndexs
						{
							Curr = text2,
							_date = date
						};
						frm.FormClosing += (object s, FormClosingEventArgs e2) =>
						{
							lineRate = Function.ParseDouble(frm.rate);
						};
						frm.ShowDialog();
					}
					else
					{
						lineRate = exchangeRate;
					}
					double exchangeRate2 = GetExchangeRate(GlobalData.MainCurrency, date);
					num2 *= Math.Pow(exchangeRate2, GlobalData.ExchangeRatePow);
					if (text.ToUpper() == GlobalData.MainCurrency.ToUpper())
					{
						num3 = 0.0;
					}
					else if (Math.Abs(num) > double.Epsilon)
					{
						num3 = num2 * Math.Pow(lineRate / num, GlobalData.ExchangeRatePow);
					}
				}
				row["Currency"] = text;
				row["Rate"] = lineRate;
				row["TotalFrgn"] = num3;
				row["LineTotal"] = num2;
			}
			DataTable source = (DataTable)grd_Line.DataSource;
			double num4 = ((!(text == GlobalData.MainCurrency)) ? source.AsEnumerable().Sum((DataRow x) => Function.ParseDouble(x["TotalFrgn"])) : source.AsEnumerable().Sum((DataRow x) => Function.ParseDouble(x["LineTotal"])));
			double num5 = num4 * Math.Pow(num, GlobalData.ExchangeRatePow);
			double num6 = num4;
			gv_Doc.SetFocusedRowCellValue("DocTotal", num5);
			gv_Doc.SetFocusedRowCellValue("DocTotalFC", num6);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static double GetExchangeRate(string curr, DateTime date)
	{
		try
		{
			double result = 1.0;
			if (curr.ToUpper() != GlobalData.MainCurrency.ToUpper())
			{
				result = HttpClientHelperFactory.GetHttpClient().ExRateAndIndex.GetExchangeRate(curr.ToUpper(), date);
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 1.0;
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
			if (e.Column == col_CardType)
			{
				_isChanging = true;
				gv_Line.SetRowCellValue(e.RowHandle, "CardCode", "");
				gv_Line.SetRowCellValue(e.RowHandle, "CardName", "");
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
			if (e.Column != col_CardCode)
			{
				return;
			}
			_isChanging = true;
			if (grd_Line.DataSource is DataTable dataTable)
			{
				int num = (dataTable.IsNotNull() ? dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"])) : 0);
				if (Function.ParseInt(gv_Line.GetRowCellValue(e.RowHandle, "LineNum")) <= 0)
				{
					gv_Line.SetRowCellValue(e.RowHandle, "LineNum", num + 1);
					gv_Line.PostEditor();
					dataTable.AcceptChanges();
				}
				DataRow dataRow = null;
				foreach (DataRow item in dataTable.AsEnumerable())
				{
					if (Function.ParseInt(item["LineNum"]) == -1)
					{
						dataRow = item;
						break;
					}
				}
				if (dataRow != null)
				{
					dataTable.Rows.Remove(dataRow);
				}
			}
			_isChanging = false;
			AddNewRow();
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

	private void gv_Line_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column != col_CardCode)
			{
				return;
			}
			object rowCellValue = gv_Line.GetRowCellValue(e.RowHandle, "CardType");
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
			if (sender is GridViewEx gridViewEx)
			{
				GridHitInfo gridHitInfo = gridViewEx.CalcHitInfo(new Point(e.X, e.Y));
				if (gridHitInfo.InRowCell)
				{
					HotTrackRow = gridHitInfo.RowHandle;
				}
				else
				{
					HotTrackRow = int.MinValue;
				}
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
			retLine = dataTable2.Copy();
			if (retLine.IsNotNull())
			{
				if (Function.ParseInt(retLine.Rows[retLine.Rows.Count - 1]["LineNum"]) == -1)
				{
					retLine.Rows.RemoveAt(retLine.Rows.Count - 1);
					retLine.AcceptChanges();
				}
				if (retLine.IsNotNull() && retLine.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["CardCode"]))) != null)
				{
					return "CardCode can not be empty!";
				}
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

	protected virtual void AddNewDoc()
	{
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			_preAdd = true;
			gv_Doc.BeginInsertRow();
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			dataSourceTable = (dataSourceTable.IsNotNull() ? dataSourceTable.Clone() : WinFuntions.GetTableStructure(gv_Doc.ItemId).Copy());
			if (grd_Doc.DataSource is DataTable dataTable)
			{
				dataTable.Rows.Clear();
			}
			if (grd_Line.DataSource is DataTable dataTable2)
			{
				dataTable2.Rows.Clear();
			}
			dataSourceTable.SetDefaultAndMaxLen(gv_Doc.ItemId);
			grd_Doc.DataSource = dataSourceTable;
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow["DocDate"] = DateTime.Today;
			dataRow["DocCur"] = GlobalData.MainCurrency;
			dataRow["DocRate"] = 1;
			dataSourceTable.Rows.Add(dataRow);
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
				dataRow["CardType"] = "C";
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Payment.frm_AdvancePayment));
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctxDoc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_PaymentMethod = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Currency = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx1 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_DocStatus = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.ctxLine = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.col_CardType = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CardType = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.col_CardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CurLine = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx2 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx16 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lb_DocStatus = new Apzon.Controls.Controls.LabelLine();
		this.lb_DocNumber = new Apzon.Controls.Controls.LabelLine();
		this.lb_ToDate = new Apzon.Controls.Controls.LabelLine();
		this.lb_FromDate = new Apzon.Controls.Controls.LabelLine();
		this.btnSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtNoOfTrans = new Apzon.Controls.Editor.TextEditEx();
		this.txtToDate = new Apzon.Controls.Editor.DateEditEx();
		this.txtFromDate = new Apzon.Controls.Editor.DateEditEx();
		this.cbxStatus = new Apzon.Controls.Editor.LookUpEditEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctxDoc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PaymentMethod).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Currency).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DocStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		this.ctxLine.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CardType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CurLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtNoOfTrans.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbxStatus.Properties).BeginInit();
		base.SuspendLayout();
		this.repositoryItemCalcEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx1.IsSearchControl = false;
		this.repositoryItemCalcEditEx1.Name = "repositoryItemCalcEditEx1";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, 50);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Line);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(968, 410);
		this.splitContainerControl1.SplitterPosition = 222;
		this.splitContainerControl1.TabIndex = 89;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctxDoc;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.repositoryItemMemoExEdit2, this.repositoryItemDateEditEx1, this.repositoryItemCalcEdit1, this.rep_PaymentMethod, this.rep_DocStatus, this.rep_Currency });
		this.grd_Doc.Size = new System.Drawing.Size(968, 222);
		this.grd_Doc.TabIndex = 86;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctxDoc.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.addNewToolStripMenuItem });
		this.ctxDoc.Name = "contextMenuStrip2";
		this.ctxDoc.Size = new System.Drawing.Size(124, 26);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gridColumnEx7, this.gridColumnEx2, this.gridColumnEx4, this.gridColumnEx15, this.gridColumnEx13, this.gridColumnEx14, this.gridColumnEx11, this.gridColumnEx10, this.gridColumnEx12 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oAdvancePayment";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 30;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "OAPM";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.RowAutoHeight = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
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
		this.gridColumnEx7.Width = 80;
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
		this.gridColumnEx2.VisibleIndex = 0;
		this.gridColumnEx2.Width = 91;
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
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_PaymentMethod.Name = "rep_PaymentMethod";
		this.rep_PaymentMethod.NullText = "";
		this.rep_PaymentMethod.ShowFooter = false;
		this.rep_PaymentMethod.ShowHeader = false;
		this.gridColumnEx4.Caption = "Currency";
		this.gridColumnEx4.ColumnEdit = this.rep_Currency;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "DocCur";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 2;
		this.gridColumnEx4.Width = 66;
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
		this.gridColumnEx15.Caption = "Amount";
		this.gridColumnEx15.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gridColumnEx15.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
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
		this.gridColumnEx15.Width = 66;
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.gridColumnEx13.Caption = "Amount FC";
		this.gridColumnEx13.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gridColumnEx13.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = "DocCur";
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
		this.gridColumnEx13.Width = 66;
		this.gridColumnEx14.Caption = "Exchange Rate";
		this.gridColumnEx14.ColumnEdit = this.repositoryItemCalcEditEx1;
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
		this.gridColumnEx14.Width = 66;
		this.gridColumnEx11.Caption = "Posting Date";
		this.gridColumnEx11.ColumnEdit = this.repositoryItemDateEditEx1;
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx11.FieldName = "DocDate";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Visible = true;
		this.gridColumnEx11.VisibleIndex = 5;
		this.gridColumnEx11.Width = 76;
		this.repositoryItemDateEditEx1.AutoHeight = false;
		this.repositoryItemDateEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.DisplayFormat.FormatString = "";
		this.repositoryItemDateEditEx1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx1.EditFormat.FormatString = "";
		this.repositoryItemDateEditEx1.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx1.IsSearchControl = false;
		this.repositoryItemDateEditEx1.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx1.Name = "repositoryItemDateEditEx1";
		this.gridColumnEx10.Caption = "Doc Status";
		this.gridColumnEx10.ColumnEdit = this.rep_DocStatus;
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "DocStatus";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 6;
		this.gridColumnEx10.Width = 68;
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
		this.gridColumnEx12.VisibleIndex = 7;
		this.gridColumnEx12.Width = 99;
		this.repositoryItemMemoExEdit2.AutoHeight = false;
		this.repositoryItemMemoExEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit2.Name = "repositoryItemMemoExEdit2";
		this.repositoryItemMemoExEdit2.ShowIcon = false;
		this.grd_Line.AllowPasteExcel = true;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.ContextMenuStrip = this.ctxLine;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(0, 0);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.repositoryItemDateEditEx2, this.repositoryItemCalcEdit2, this.rep_CardType, this.rep_CurLine, this.repositoryItemMemoExEdit1 });
		this.grd_Line.Size = new System.Drawing.Size(968, 178);
		this.grd_Line.TabIndex = 83;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		this.ctxLine.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeRowToolStripMenuItem });
		this.ctxLine.Name = "contextMenuStrip1";
		this.ctxLine.Size = new System.Drawing.Size(144, 26);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.gv_Line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Line.ColumnKey = "EmpId";
		this.gv_Line.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gridColumnEx3, this.col_CardType, this.col_CardCode, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx8, this.gridColumnEx1, this.gridColumnEx9, this.gridColumnEx16 });
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FormId = "oAdvancePayment";
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.IgnoreFilter = false;
		this.gv_Line.IndexEditRow = -1;
		this.gv_Line.IndicatorWidth = 30;
		this.gv_Line.IsApzTable = false;
		this.gv_Line.IsInsertNewRow = true;
		this.gv_Line.IsLoadingData = false;
		this.gv_Line.IsMappingContextAction = false;
		this.gv_Line.ItemId = "APM1";
		this.gv_Line.LstColumnAllowBatch = null;
		this.gv_Line.LstColumnNotAllowBatch = null;
		this.gv_Line.LstColumnNotEditable = null;
		this.gv_Line.LstColumNotSetting = null;
		this.gv_Line.Name = "gv_Line";
		this.gv_Line.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_Line.OptionsView.RowAutoHeight = true;
		this.gv_Line.OptionsView.ShowGroupPanel = false;
		this.gv_Line.UseBatchMod = false;
		this.gridColumnEx3.Caption = "Line Num";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "LineNum";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsColumn.ReadOnly = true;
		this.gridColumnEx3.PrimaryKey = true;
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
		this.gridColumnEx6.Caption = "Amount";
		this.gridColumnEx6.ColumnEdit = this.repositoryItemCalcEdit2;
		this.gridColumnEx6.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = "";
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx6.FieldName = "LineTotal";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 3;
		this.repositoryItemCalcEdit2.AutoHeight = false;
		this.repositoryItemCalcEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit2.Name = "repositoryItemCalcEdit2";
		this.gridColumnEx8.Caption = "Amount FC";
		this.gridColumnEx8.ColumnEdit = this.repositoryItemCalcEdit2;
		this.gridColumnEx8.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = "Currency";
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx8.FieldName = "TotalFrgn";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 4;
		this.gridColumnEx1.Caption = "Currency";
		this.gridColumnEx1.ColumnEdit = this.rep_CurLine;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "Currency";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 5;
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
		this.gridColumnEx9.Caption = "Trans Date";
		this.gridColumnEx9.ColumnEdit = this.repositoryItemDateEditEx2;
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx9.FieldName = "TransDate";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 6;
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
		this.gridColumnEx16.Caption = "Remark";
		this.gridColumnEx16.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.gridColumnEx16.CurrencyDateField = null;
		this.gridColumnEx16.CurrencyField = null;
		this.gridColumnEx16.CurrencyFixed = null;
		this.gridColumnEx16.CurrencyFixedRate = 0.0;
		this.gridColumnEx16.CurrencyRateField = null;
		this.gridColumnEx16.FieldName = "Comments";
		this.gridColumnEx16.Name = "gridColumnEx16";
		this.gridColumnEx16.PrimaryKey = false;
		this.gridColumnEx16.UnitField = null;
		this.gridColumnEx16.Visible = true;
		this.gridColumnEx16.VisibleIndex = 7;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(3, 466);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(61, 20);
		this.btn_Save.TabIndex = 91;
		this.btn_Save.Text = "OK";
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(70, 466);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 90;
		this.btnCancel.Text = "Cancel";
		this.lb_DocStatus.EditControl = null;
		this.lb_DocStatus.EditValue = "Doc. Status";
		this.lb_DocStatus.FieldName = null;
		this.lb_DocStatus.HideWhenEmpty = false;
		this.lb_DocStatus.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DocStatus.Location = new System.Drawing.Point(226, 27);
		this.lb_DocStatus.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_DocStatus.Name = "lb_DocStatus";
		this.lb_DocStatus.ReadOnly = false;
		this.lb_DocStatus.SettingInVisible = false;
		this.lb_DocStatus.SettingReadonly = false;
		this.lb_DocStatus.ShowUnderLine = true;
		this.lb_DocStatus.Size = new System.Drawing.Size(96, 20);
		this.lb_DocStatus.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DocStatus.TabIndex = 102;
		this.lb_DocStatus.Utext = "Doc. Status";
		this.lb_DocNumber.EditControl = null;
		this.lb_DocNumber.EditValue = "Doc. Number";
		this.lb_DocNumber.FieldName = null;
		this.lb_DocNumber.HideWhenEmpty = false;
		this.lb_DocNumber.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DocNumber.Location = new System.Drawing.Point(226, 3);
		this.lb_DocNumber.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_DocNumber.Name = "lb_DocNumber";
		this.lb_DocNumber.ReadOnly = false;
		this.lb_DocNumber.SettingInVisible = false;
		this.lb_DocNumber.SettingReadonly = false;
		this.lb_DocNumber.ShowUnderLine = true;
		this.lb_DocNumber.Size = new System.Drawing.Size(88, 20);
		this.lb_DocNumber.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DocNumber.TabIndex = 100;
		this.lb_DocNumber.Utext = "Doc. Number";
		this.lb_ToDate.EditControl = null;
		this.lb_ToDate.EditValue = "To Date";
		this.lb_ToDate.FieldName = null;
		this.lb_ToDate.HideWhenEmpty = false;
		this.lb_ToDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ToDate.Location = new System.Drawing.Point(2, 27);
		this.lb_ToDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_ToDate.Name = "lb_ToDate";
		this.lb_ToDate.ReadOnly = false;
		this.lb_ToDate.SettingInVisible = false;
		this.lb_ToDate.SettingReadonly = false;
		this.lb_ToDate.ShowUnderLine = true;
		this.lb_ToDate.Size = new System.Drawing.Size(80, 20);
		this.lb_ToDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ToDate.TabIndex = 99;
		this.lb_ToDate.Utext = "To Date";
		this.lb_FromDate.EditControl = null;
		this.lb_FromDate.EditValue = "From Date";
		this.lb_FromDate.FieldName = null;
		this.lb_FromDate.HideWhenEmpty = false;
		this.lb_FromDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_FromDate.Location = new System.Drawing.Point(2, 2);
		this.lb_FromDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_FromDate.Name = "lb_FromDate";
		this.lb_FromDate.ReadOnly = false;
		this.lb_FromDate.SettingInVisible = false;
		this.lb_FromDate.SettingReadonly = false;
		this.lb_FromDate.ShowUnderLine = true;
		this.lb_FromDate.Size = new System.Drawing.Size(80, 20);
		this.lb_FromDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_FromDate.TabIndex = 98;
		this.lb_FromDate.Utext = "From Date";
		this.btnSearch.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnSearch.Appearance.Options.UseBackColor = true;
		this.btnSearch.EditControl = null;
		this.btnSearch.EditValue = "Search";
		this.btnSearch.FieldName = null;
		this.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		this.btnSearch.IsSearchControl = false;
		this.btnSearch.Location = new System.Drawing.Point(460, 23);
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.ReadOnly = false;
		this.btnSearch.SettingInVisible = false;
		this.btnSearch.SettingReadonly = false;
		this.btnSearch.Size = new System.Drawing.Size(99, 20);
		this.btnSearch.TabIndex = 5;
		this.btnSearch.Text = "Search";
		this.txtNoOfTrans.EditControl = null;
		this.txtNoOfTrans.FieldName = null;
		this.txtNoOfTrans.IsSearchControl = false;
		this.txtNoOfTrans.Location = new System.Drawing.Point(314, 2);
		this.txtNoOfTrans.Name = "txtNoOfTrans";
		this.txtNoOfTrans.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.txtNoOfTrans.Properties.Appearance.Options.UseFont = true;
		this.txtNoOfTrans.SettingInVisible = false;
		this.txtNoOfTrans.SettingReadonly = false;
		this.txtNoOfTrans.Size = new System.Drawing.Size(140, 18);
		this.txtNoOfTrans.TabIndex = 3;
		this.txtToDate.EditControl = null;
		this.txtToDate.EditValue = null;
		this.txtToDate.FieldName = null;
		this.txtToDate.Location = new System.Drawing.Point(82, 26);
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
		this.txtToDate.SettingInVisible = false;
		this.txtToDate.SettingReadonly = false;
		this.txtToDate.Size = new System.Drawing.Size(140, 18);
		this.txtToDate.TabIndex = 2;
		this.txtFromDate.EditControl = null;
		this.txtFromDate.EditValue = null;
		this.txtFromDate.FieldName = null;
		this.txtFromDate.Location = new System.Drawing.Point(82, 2);
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
		this.txtFromDate.SettingInVisible = false;
		this.txtFromDate.SettingReadonly = false;
		this.txtFromDate.Size = new System.Drawing.Size(140, 18);
		this.txtFromDate.TabIndex = 1;
		this.cbxStatus.EditControl = null;
		this.cbxStatus.FieldName = null;
		this.cbxStatus.IsSearchControl = false;
		this.cbxStatus.Location = new System.Drawing.Point(314, 24);
		this.cbxStatus.Name = "cbxStatus";
		this.cbxStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbxStatus.Properties.NullText = "";
		this.cbxStatus.SettingInVisible = false;
		this.cbxStatus.SettingReadonly = false;
		this.cbxStatus.Size = new System.Drawing.Size(140, 20);
		this.cbxStatus.TabIndex = 4;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(971, 490);
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
		base.FormId = "oAdvancePayment";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_AdvancePayment";
		this.Text = "Advance Payment ";
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctxDoc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PaymentMethod).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Currency).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DocStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		this.ctxLine.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CardType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CurLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtNoOfTrans.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbxStatus.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
