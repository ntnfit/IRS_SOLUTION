using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTab;

namespace Apzon.Controls.Form.ExchangeRateAndIndex;

public class frm_ExchangeRatesAndIndexs : XtraFormEx
{
	private DataTable _dtOriginal = new DataTable();

	private readonly List<Common.Table_Code> _lstMonth = new List<Common.Table_Code>();

	private readonly HttpClientHelper<ExchangeRateAndIndexImpl> _httpClientHelper = new HttpClientHelper<ExchangeRateAndIndexImpl>(Configuration.ApiServer, "ExRateAndIndex");

	private Enums.FormViewModeType _viewMode = Enums.FormViewModeType.View;

	private bool _isLoadForm = true;

	private bool _forceClose;

	private bool _isChangeCbYear;

	private int _focusRow = -1;

	private GridColumn _focusCol;

	public ExchangeRateAndIndexImpl _objItems = new ExchangeRateAndIndexImpl();

	public string Curr = "";

	public DateTime _date = DateTime.Today;

	public double rate;

	public static bool isClosing;

	public static bool _isOpen;

	private IContainer components;

	private XtraTabControl xtraTabControl1;

	private XtraTabPage xtp_ExchangeRate;

	private XtraTabPage xtp_Indexes;

	private ComboBoxEdit cbRateYear;

	private ComboBoxEdit cbMonth;

	private GridControlEx grdRates;

	private SimpleButton btOK;

	private SimpleButton btCancel;

	private SimpleButton btSetRateFSelCriterial;

	private SimpleButton btAutoImport;

	private SimpleButton btAutoExport;

	private ComboBoxEdit cbIndexesYear;

	private GridControlEx grdIndexes;

	private GridColumn gcMonth;

	private GridColumn gcCP;

	private RepositoryItemCalcEdit rep_CalcEdit1;

	private RepositoryItemCalcEdit rep_CalcEdit2;

	private GridViewEx gvRates;

	private GridViewEx gvIndexes;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private RepositoryItemCalcEdit repositoryItemCalcEdit2;

	private GridColumnEx gc_MonthName;

	private Enums.FormViewModeType ViewMode
	{
		get
		{
			return _viewMode;
		}
		set
		{
			switch (value)
			{
			case Enums.FormViewModeType.Update:
				btOK.Text = "Update";
				break;
			case Enums.FormViewModeType.View:
				btOK.Text = "OK";
				break;
			}
			_viewMode = value;
		}
	}

	public frm_ExchangeRatesAndIndexs()
	{
		_isOpen = true;
		InitializeComponent();
		OnLoadAsync += frm_ExchangeRatesAndIndexs_Load;
	}

	private async Task frm_ExchangeRatesAndIndexs_Load(object sender, EventArgs e)
	{
		try
		{
			LoadComboBox();
			await LoadWaitData();
			SetFormatControls();
			_focusRow = _date.Day - 1;
			gvRates.FocusedRowHandle = _focusRow;
			GridColumn focusedColumn = (_focusCol = gvRates.Columns.FirstOrDefault((GridColumn t) => t.FieldName.ToUpper().Equals(Curr.ToUpper())));
			gvRates.FocusedColumn = focusedColumn;
			gc_MonthName.AppearanceCell.BackColor = StyleColor.GridDisable1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_ExchangeRatesAndIndexs_Shown(object sender, EventArgs e)
	{
		try
		{
			gvRates.ShowEditor();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void cbMonth_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoadForm)
			{
				string month = _lstMonth.Find((Common.Table_Code x) => x.Name == cbMonth.EditValue.ToString()).Code;
				DataTable result = Task.Run(() => LoadDataRate(month, cbRateYear.EditValue.ToString())).Result;
				RefreshGridData(ref grdRates, result);
				if (cbMonth.SelectedIndex == _date.Month - 1)
				{
					gvRates.FocusedRowHandle = _focusRow;
					gvRates.FocusedColumn = _focusCol;
					gvRates.ShowEditor();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void cbRateYear_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoadForm)
			{
				if (!_isChangeCbYear)
				{
					_isChangeCbYear = true;
					cbIndexesYear.EditValue = cbRateYear.EditValue;
					_isChangeCbYear = false;
				}
				string year = cbRateYear.EditValue.ToString();
				string month = _lstMonth.Find((Common.Table_Code x) => x.Name == cbMonth.EditValue.ToString()).Code;
				DataTable result = Task.Run(() => LoadDataRate(month, year)).Result;
				RefreshGridData(ref grdRates, result);
				int j = Function.ParseInt(year);
				LoadCbxYear(j, ref cbRateYear);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void cbIndexesYear_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoadForm)
			{
				if (!_isChangeCbYear)
				{
					_isChangeCbYear = true;
					cbRateYear.EditValue = cbIndexesYear.EditValue;
				}
				int num = Function.ParseInt(cbIndexesYear.EditValue);
				DataTable dt = LoadDataIndex(num);
				RefreshGridData(ref grdIndexes, dt);
				int j = Function.ParseInt(num);
				LoadCbxYear(j, ref cbIndexesYear);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvRates_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
	{
		ProcessChangedCell();
	}

	private void gvRates_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		ProcessChangedCell();
	}

	private void gvRates_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			if (Function.ParseDouble(e.Value) == 0.0 && e.Column.AbsoluteIndex > 0)
			{
				e.DisplayText = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (ViewMode == Enums.FormViewModeType.View)
			{
				ViewMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_ExchangeRatesAndIndexs_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (string.IsNullOrEmpty(Curr) || !(grdRates.DataSource is DataTable) || (grdRates.DataSource as DataTable).Rows.Count == 0)
		{
			_forceClose = true;
			isClosing = false;
			_isOpen = false;
			return;
		}
		if (!_forceClose)
		{
			e.Cancel = true;
		}
		isClosing = true;
		WinCommons._OpenProcessing();
		try
		{
			if (!string.IsNullOrEmpty(Curr))
			{
				double exchangeRate = HttpClientHelperFactory.GetHttpClient().ExRateAndIndex.GetExchangeRate(Curr, _date);
				if (exchangeRate <= 0.0)
				{
					if (Curr.ToUpper() != GlobalData.MainCurrency.ToUpper() && Curr.IndexOf("##") < 0)
					{
						if (cbMonth.SelectedIndex != _date.Month - 1)
						{
							cbMonth.SelectedIndex = _date.Month - 1;
						}
						for (int i = 0; i < cbRateYear.Properties.Items.Count; i++)
						{
							if (Function.ParseInt(cbRateYear.Properties.Items[i]) == _date.Year)
							{
								cbRateYear.SelectedIndex = i;
							}
						}
						btOK.Text = "OK";
						ProcessChangedCell();
					}
					else
					{
						_forceClose = true;
						isClosing = false;
						_isOpen = false;
						Dispose();
					}
				}
				else
				{
					rate = exchangeRate;
					_forceClose = true;
					isClosing = false;
					_isOpen = false;
					Dispose();
				}
			}
			else
			{
				_forceClose = true;
				isClosing = false;
				_isOpen = false;
				Dispose();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		_forceClose = true;
		isClosing = false;
		_isOpen = false;
		WinCommons._CloseProcessing();
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if ((msg.Msg == 256 || msg.Msg == 260) && keyData == Keys.Return)
		{
			btOK.Focus();
			btOK_Click(null, null);
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void cbRate_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (ViewMode != Enums.FormViewModeType.Update)
			{
				return;
			}
			switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel))
			{
			case DialogResult.Yes:
			{
				string text = SaveDoc();
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					ViewMode = Enums.FormViewModeType.View;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
					e.Cancel = true;
				}
				break;
			}
			case DialogResult.Cancel:
				e.Cancel = true;
				break;
			default:
				ViewMode = Enums.FormViewModeType.View;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void xtraTabControl1_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		try
		{
			btSetRateFSelCriterial.Visible = !(e.Page.Name == "xtp_Indexes");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btOK_Click(object sender, EventArgs e)
	{
		try
		{
			string text = SaveDoc();
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				ViewMode = Enums.FormViewModeType.View;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btSetRateFSelCriterial_Click(object sender, EventArgs e)
	{
		try
		{
			int month = Function.ParseInt(_lstMonth.Find((Common.Table_Code x) => x.Name == cbMonth.EditValue.ToString()).Code);
			int year = Function.ParseInt(cbRateYear.EditValue.ToString());
			frm_SetRateForSelectionCriteria frm_SetRateForSelectionCriteria2 = new frm_SetRateForSelectionCriteria();
			frm_SetRateForSelectionCriteria2.ReLoadParent = ReLoadFromChild;
			frm_SetRateForSelectionCriteria2._FromDate = new DateTime(year, month, 1);
			frm_SetRateForSelectionCriteria2._ToDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
			frm_SetRateForSelectionCriteria2.ShowDialog();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btAutoExport_Click(object sender, EventArgs e)
	{
	}

	private void btAutoImport_Click(object sender, EventArgs e)
	{
	}

	private string SaveDoc()
	{
		WinCommons._OpenProcessing();
		string text = "";
		try
		{
			DisableControls(isDisable: true);
			if (ViewMode == Enums.FormViewModeType.View)
			{
				DisableControls(isDisable: false);
				Close();
				return "";
			}
			List<ExchangeRateAndIndexImpl> list = new List<ExchangeRateAndIndexImpl>();
			if (grdRates.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				DataTable dataTable2 = dataTable.Copy();
				if (dataTable2.Columns.Contains("daynum"))
				{
					dataTable2.Columns.Remove("daynum");
				}
				dataTable2.AcceptChanges();
				for (int i = 0; i < dataTable2.Rows.Count; i++)
				{
					for (int j = 1; j < dataTable2.Columns.Count; j++)
					{
						DateTime? date = Function.ParseDateTime(dataTable2.Rows[i][0]);
						string columnName = dataTable2.Columns[j].ColumnName;
						if (!date.HasValue)
						{
							continue;
						}
						_ = date.Value;
						decimal num = Function.ParseDecimal(_dtOriginal.AsEnumerable().FirstOrDefault((DataRow r) =>
						{
							DateTime? dateTime = Function.ParseDateTime(r[0]);
							DateTime? dateTime2 = date;
							if (dateTime.HasValue != dateTime2.HasValue)
							{
								return false;
							}
							return !dateTime.HasValue || dateTime.GetValueOrDefault() == dateTime2.GetValueOrDefault();
						})[columnName]);
						decimal num2 = Function.ParseDecimal(dataTable2.Rows[i][j]);
						if (num != num2)
						{
							ExchangeRateAndIndexImpl exchangeRateAndIndexImpl = new ExchangeRateAndIndexImpl();
							exchangeRateAndIndexImpl.Rate = num2;
							exchangeRateAndIndexImpl.RateDate = date.Value;
							exchangeRateAndIndexImpl.Currency = columnName;
							exchangeRateAndIndexImpl.DataSource = "I";
							exchangeRateAndIndexImpl.UserSign = GlobalData.UserID;
							list.Add(exchangeRateAndIndexImpl);
						}
					}
				}
			}
			if (grdIndexes.DataSource is DataTable dataTable3 && dataTable3.Rows.Count > 0)
			{
				DataTable dataTable4 = dataTable3.Copy();
				foreach (DataRow row in dataTable4.Rows)
				{
					decimal num3 = Function.ParseDecimal(row["Rate"]);
					ExchangeRateAndIndexImpl exchangeRateAndIndexImpl2 = new ExchangeRateAndIndexImpl();
					exchangeRateAndIndexImpl2.Currency = "CP";
					int year = Function.ParseInt(cbIndexesYear.EditValue.ToString());
					int month = dataTable4.Rows.IndexOf(row) + 1;
					exchangeRateAndIndexImpl2.RateDate = new DateTime(year, month, 1);
					exchangeRateAndIndexImpl2.DataSource = "I";
					exchangeRateAndIndexImpl2.UserSign = GlobalData.UserID;
					exchangeRateAndIndexImpl2.Rate = num3;
					list.Add(exchangeRateAndIndexImpl2);
				}
			}
			text = _httpClientHelper.PostAsJson("SaveDoc", list);
			DisableControls(isDisable: false);
			if (!string.IsNullOrEmpty(Curr) && Curr.ToUpper() != GlobalData.MainCurrency.ToUpper())
			{
				if (cbMonth.SelectedIndex != _date.Month - 1)
				{
					cbMonth.SelectedIndex = _date.Month - 1;
				}
				if (Function.ParseInt(cbRateYear.EditValue) != _date.Year)
				{
					for (int num4 = 0; num4 < cbRateYear.Properties.Items.Count; num4++)
					{
						if (Function.ParseInt(cbRateYear.Properties.Items[num4]) == _date.Year)
						{
							cbRateYear.SelectedIndex = num4;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			text = ex.Message;
		}
		WinCommons._CloseProcessing();
		DisableControls(isDisable: false);
		return text;
	}

	private void ReLoadFromChild(List<Common.Table_Code> lstCurrency, int from, int to)
	{
		try
		{
			DataTable dataTable = grdRates.DataSource as DataTable;
			foreach (Common.Table_Code item in lstCurrency)
			{
				for (int i = from - 1; i < to; i++)
				{
					for (int j = 0; j < dataTable.Columns.Count; j++)
					{
						if (dataTable.Columns[j].ColumnName == item.Code)
						{
							dataTable.Rows[i][j] = item.Name;
						}
					}
				}
			}
			grdRates.RefreshDataSource();
			ViewMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DisableControls(bool isDisable)
	{
		try
		{
			if (isDisable)
			{
				cbMonth.Enabled = false;
				cbRateYear.Enabled = false;
				btOK.Enabled = false;
				btCancel.Enabled = false;
				grdRates.Enabled = false;
				btAutoExport.Enabled = false;
				btAutoImport.Enabled = false;
				btSetRateFSelCriterial.Enabled = false;
				grdIndexes.Enabled = false;
				cbIndexesYear.Enabled = false;
				ControlBox = false;
				Text = "Exchange Rates And Indexs - Saving...";
			}
			else if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ExchangeRatesAndIndexs" && x.authoriza == "F"))
			{
				cbMonth.Enabled = true;
				cbRateYear.Enabled = true;
				btOK.Enabled = true;
				btCancel.Enabled = true;
				grdRates.Enabled = true;
				btAutoExport.Enabled = true;
				btAutoImport.Enabled = true;
				btSetRateFSelCriterial.Enabled = true;
				grdIndexes.Enabled = true;
				cbIndexesYear.Enabled = true;
				ControlBox = false;
				Text = "Exchange Rates And Indexs";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ProcessChangedCell()
	{
		try
		{
			int num = Function.ParseInt(_lstMonth.Find((Common.Table_Code x) => x.Name == cbMonth.EditValue.ToString()).Code);
			int num2 = Function.ParseInt(cbRateYear.EditValue);
			if ((!(Curr == "") || Curr != null) && _focusRow != -1 && _focusCol != null && num == _date.Month && num2 == _date.Year)
			{
				double num3 = Function.ParseDouble(gvRates.GetRowCellValue(_focusRow, _focusCol));
				if (num3 <= 0.0)
				{
					gvRates.FocusedRowHandle = _focusRow;
					gvRates.FocusedColumn = _focusCol;
					gvRates.ShowEditor();
				}
				else
				{
					rate = num3;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetFormatControls()
	{
		try
		{
			if (rep_CalcEdit1 != null && rep_CalcEdit2 != null)
			{
				rep_CalcEdit1.Mask.MaskType = MaskType.Numeric;
				rep_CalcEdit1.Mask.EditMask = "n" + GlobalData.RateAccuracyFormat.NumberDecimalDigits;
				rep_CalcEdit1.Mask.UseMaskAsDisplayFormat = true;
				rep_CalcEdit2.Mask.MaskType = MaskType.Numeric;
				rep_CalcEdit2.Mask.EditMask = "n" + GlobalData.RateAccuracyFormat.NumberDecimalDigits;
				rep_CalcEdit2.Mask.UseMaskAsDisplayFormat = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadComboBox()
	{
		try
		{
			string[] monthNames = new DateTimeFormatInfo().MonthNames;
			int num = 1;
			string[] array = monthNames;
			foreach (string text in array)
			{
				Common.Table_Code table_Code = new Common.Table_Code();
				table_Code.Code = num.ToString();
				table_Code.Name = text;
				_lstMonth.Add(table_Code);
				cbMonth.Properties.Items.Add(text);
				num++;
			}
			cbMonth.SelectedIndex = DateTime.Today.Month - 1;
			num = ((_date.Year != DateTime.Today.Year) ? _date.Year : DateTime.Today.Year);
			LoadCbxYear(num, ref cbRateYear);
			LoadCbxYear(num, ref cbIndexesYear);
			_isLoadForm = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadCbxYear(int j, ref ComboBoxEdit cb)
	{
		try
		{
			cb.Properties.Items.Clear();
			List<Common.Table_Code> list = new List<Common.Table_Code>();
			for (int i = j - 5; i < j + 5; i++)
			{
				Common.Table_Code table_Code = new Common.Table_Code();
				table_Code.Code = i.ToString();
				list.Add(table_Code);
				cb.Properties.Items.Add(i);
			}
			cb.SelectedIndex = list.FindIndex((Common.Table_Code x) => x.Code == j.ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadWaitData()
	{
		try
		{
			string month = _date.Month.ToString();
			cbMonth.SelectedIndex = _date.Month - 1;
			string year = _date.Year.ToString();
			DataTable dataTable = await LoadDataRate(month, year);
			if (dataTable != null)
			{
				_dtOriginal = dataTable.Copy();
			}
			RefreshGridData(ref grdRates, dataTable);
			DataTable dt = LoadDataIndex(Function.ParseInt(year));
			RefreshGridData(ref grdIndexes, dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> LoadDataRate(string month, string year)
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ExRateAndIndex");
			DataTable dataTable = await hp.GetAsync("GetRatesByMonthYear", new { month, year });
			if (dataTable != null && dataTable.Columns.Contains("RateDate"))
			{
				dataTable.Columns.Remove("RateDate");
				dataTable.AcceptChanges();
				_dtOriginal = dataTable.Copy();
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private DataTable LoadDataIndex(int year)
	{
		try
		{
			return _httpClientHelper.GetDataTable("GetIndexByYear", new { year });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private void RefreshGridData(ref GridControlEx grid, object dt)
	{
		try
		{
			grid.DataSource = dt;
			grid.RefreshDataSource();
			if (grid.Name == "grdRates")
			{
				if (dt is DataTable dataTable && dataTable.Rows.Count > 0)
				{
					if (!dataTable.Columns.Contains("daynum"))
					{
						dataTable.Columns.Add(new DataColumn("daynum", typeof(short)));
					}
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						dataTable.Rows[i]["daynum"] = Function.ParseShort(i + 1);
					}
					dataTable.AcceptChanges();
					grid.DataSource = dataTable;
					grid.RefreshDataSource();
					gvRates.Columns.Clear();
					gvRates.PopulateColumns();
				}
				if (gvRates.Columns.Count <= 0)
				{
					return;
				}
				foreach (GridColumn column in gvRates.Columns)
				{
					column.Visible = false;
				}
				gvRates.Columns["daynum"].Caption = cbMonth.EditValue.ToString();
				gvRates.Columns["daynum"].Width = 60;
				gvRates.Columns["daynum"].OptionsColumn.ReadOnly = true;
				gvRates.Columns["daynum"].OptionsColumn.AllowEdit = false;
				gvRates.Columns["daynum"].OptionsColumn.AllowSort = DefaultBoolean.False;
				gvRates.Columns["daynum"].Fixed = FixedStyle.Left;
				gvRates.Columns["daynum"].Visible = true;
				gvRates.Columns["daynum"].AppearanceCell.BackColor = StyleColor.GridDisable1;
				for (int j = 1; j < gvRates.Columns.Count - 1; j++)
				{
					gvRates.Columns[j].ColumnEdit = rep_CalcEdit1;
					gvRates.Columns[j].Width = 70;
					gvRates.Columns[j].DisplayFormat.FormatType = FormatType.Numeric;
					gvRates.Columns[j].DisplayFormat.FormatString = "n" + GlobalData.RateAccuracyFormat.NumberDecimalDigits;
					gvRates.Columns[j].OptionsColumn.AllowSort = DefaultBoolean.False;
					gvRates.Columns[j].Visible = true;
				}
			}
			else
			{
				gc_MonthName.OptionsColumn.ReadOnly = true;
				gcMonth.Caption = cbIndexesYear.EditValue.ToString();
				gcMonth.OptionsColumn.ReadOnly = true;
				gvIndexes.Columns["Name"].OptionsColumn.AllowSort = DefaultBoolean.False;
				gvIndexes.Columns["Rate"].ColumnEdit = rep_CalcEdit1;
				gvIndexes.Columns["Rate"].Width = 70;
				gvIndexes.Columns["Rate"].DisplayFormat.FormatType = FormatType.Numeric;
				gvIndexes.Columns["Rate"].DisplayFormat.FormatString = "n" + GlobalData.RateAccuracyFormat.NumberDecimalDigits;
				gvIndexes.Columns["Rate"].OptionsColumn.AllowSort = DefaultBoolean.False;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Controls.Form.ExchangeRateAndIndex.frm_ExchangeRatesAndIndexs));
		this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
		this.xtp_ExchangeRate = new DevExpress.XtraTab.XtraTabPage();
		this.cbRateYear = new DevExpress.XtraEditors.ComboBoxEdit();
		this.cbMonth = new DevExpress.XtraEditors.ComboBoxEdit();
		this.grdRates = new Apzon.Controls.Grid.GridControlEx();
		this.gvRates = new Apzon.Controls.Grid.GridViewEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.xtp_Indexes = new DevExpress.XtraTab.XtraTabPage();
		this.cbIndexesYear = new DevExpress.XtraEditors.ComboBoxEdit();
		this.grdIndexes = new Apzon.Controls.Grid.GridControlEx();
		this.gvIndexes = new Apzon.Controls.Grid.GridViewEx();
		this.gcMonth = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gcCP = new DevExpress.XtraGrid.Columns.GridColumn();
		this.repositoryItemCalcEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gc_MonthName = new Apzon.Controls.Grid.GridColumnEx();
		this.btAutoExport = new DevExpress.XtraEditors.SimpleButton();
		this.btAutoImport = new DevExpress.XtraEditors.SimpleButton();
		this.btSetRateFSelCriterial = new DevExpress.XtraEditors.SimpleButton();
		this.btCancel = new DevExpress.XtraEditors.SimpleButton();
		this.btOK = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xtp_ExchangeRate.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cbRateYear.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbMonth.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdRates).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvRates).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		this.xtp_Indexes.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cbIndexesYear.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdIndexes).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvIndexes).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).BeginInit();
		base.SuspendLayout();
		this.xtraTabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabControl1.Location = new System.Drawing.Point(8, 2);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.xtp_ExchangeRate;
		this.xtraTabControl1.Size = new System.Drawing.Size(555, 363);
		this.xtraTabControl1.TabIndex = 0;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtp_ExchangeRate, this.xtp_Indexes });
		this.xtraTabControl1.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl1_SelectedPageChanged);
		this.xtp_ExchangeRate.Controls.Add(this.cbRateYear);
		this.xtp_ExchangeRate.Controls.Add(this.cbMonth);
		this.xtp_ExchangeRate.Controls.Add(this.grdRates);
		this.xtp_ExchangeRate.Name = "xtp_ExchangeRate";
		this.xtp_ExchangeRate.Size = new System.Drawing.Size(551, 339);
		this.xtp_ExchangeRate.Text = "Exchange Rates";
		this.cbRateYear.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.cbRateYear.Location = new System.Drawing.Point(476, 3);
		this.cbRateYear.Name = "cbRateYear";
		this.cbRateYear.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbRateYear.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.cbRateYear.Size = new System.Drawing.Size(72, 20);
		this.cbRateYear.TabIndex = 2;
		this.cbRateYear.SelectedIndexChanged += new System.EventHandler(this.cbRateYear_SelectedIndexChanged);
		this.cbRateYear.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.cbRate_EditValueChanging);
		this.cbMonth.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.cbMonth.Location = new System.Drawing.Point(399, 3);
		this.cbMonth.Name = "cbMonth";
		this.cbMonth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbMonth.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.cbMonth.Size = new System.Drawing.Size(72, 20);
		this.cbMonth.TabIndex = 1;
		this.cbMonth.SelectedIndexChanged += new System.EventHandler(this.cbMonth_SelectedIndexChanged);
		this.cbMonth.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.cbRate_EditValueChanging);
		this.grdRates.AllowPasteExcel = false;
		this.grdRates.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdRates.ClipboardData = null;
		this.grdRates.Location = new System.Drawing.Point(4, 26);
		this.grdRates.MainView = this.gvRates;
		this.grdRates.Name = "grdRates";
		this.grdRates.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemCalcEdit1 });
		this.grdRates.Size = new System.Drawing.Size(544, 310);
		this.grdRates.TabIndex = 0;
		this.grdRates.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvRates });
		this.gvRates.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvRates.ColumnKey = null;
		this.gvRates.DisableFormSetting = false;
		this.gvRates.DisplayException = false;
		this.gvRates.EditColumn = null;
		this.gvRates.FieldCurrency = null;
		this.gvRates.FieldRate = null;
		this.gvRates.FormId = "oExchangeRateAndIndex";
		this.gvRates.GridControl = this.grdRates;
		this.gvRates.IndexEditRow = -1;
		this.gvRates.IndicatorWidth = 35;
		this.gvRates.IsApzTable = false;
		this.gvRates.IsInsertNewRow = false;
		this.gvRates.IsLoadingData = false;
		this.gvRates.ItemId = "OEXR";
		this.gvRates.LstColumnAllowBatch = null;
		this.gvRates.LstColumnNotAllowBatch = null;
		this.gvRates.LstColumNotSetting = null;
		this.gvRates.Name = "gvRates";
		this.gvRates.OptionsView.ColumnAutoWidth = false;
		this.gvRates.OptionsView.ShowDetailButtons = false;
		this.gvRates.OptionsView.ShowGroupPanel = false;
		this.gvRates.OptionsView.ShowIndicator = false;
		this.gvRates.UseBatchMod = false;
		this.gvRates.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gvRates_FocusedRowChanged);
		this.gvRates.FocusedColumnChanged += new DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventHandler(this.gvRates_FocusedColumnChanged);
		this.gvRates.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_CellValueChanging);
		this.gvRates.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_CellValueChanging);
		this.gvRates.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gvRates_CustomColumnDisplayText);
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.xtp_Indexes.Controls.Add(this.cbIndexesYear);
		this.xtp_Indexes.Controls.Add(this.grdIndexes);
		this.xtp_Indexes.Name = "xtp_Indexes";
		this.xtp_Indexes.Size = new System.Drawing.Size(551, 339);
		this.xtp_Indexes.Text = "Indexes";
		this.cbIndexesYear.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.cbIndexesYear.Location = new System.Drawing.Point(476, 3);
		this.cbIndexesYear.Name = "cbIndexesYear";
		this.cbIndexesYear.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbIndexesYear.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.cbIndexesYear.Size = new System.Drawing.Size(72, 20);
		this.cbIndexesYear.TabIndex = 4;
		this.cbIndexesYear.SelectedIndexChanged += new System.EventHandler(this.cbIndexesYear_SelectedIndexChanged);
		this.cbIndexesYear.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.cbRate_EditValueChanging);
		this.grdIndexes.AllowPasteExcel = false;
		this.grdIndexes.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdIndexes.ClipboardData = null;
		this.grdIndexes.Location = new System.Drawing.Point(4, 26);
		this.grdIndexes.MainView = this.gvIndexes;
		this.grdIndexes.Name = "grdIndexes";
		this.grdIndexes.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemCalcEdit2 });
		this.grdIndexes.Size = new System.Drawing.Size(544, 310);
		this.grdIndexes.TabIndex = 3;
		this.grdIndexes.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvIndexes });
		this.gvIndexes.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvIndexes.ColumnKey = null;
		this.gvIndexes.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gcMonth, this.gcCP, this.gc_MonthName });
		this.gvIndexes.DisableFormSetting = false;
		this.gvIndexes.DisplayException = false;
		this.gvIndexes.EditColumn = null;
		this.gvIndexes.FieldCurrency = null;
		this.gvIndexes.FieldRate = null;
		this.gvIndexes.FormId = "oExchangeRateAndIndex";
		this.gvIndexes.GridControl = this.grdIndexes;
		this.gvIndexes.IndexEditRow = -1;
		this.gvIndexes.IndicatorWidth = 35;
		this.gvIndexes.IsApzTable = false;
		this.gvIndexes.IsInsertNewRow = false;
		this.gvIndexes.IsLoadingData = false;
		this.gvIndexes.ItemId = "OINDEX";
		this.gvIndexes.LstColumnAllowBatch = null;
		this.gvIndexes.LstColumnNotAllowBatch = null;
		this.gvIndexes.LstColumNotSetting = null;
		this.gvIndexes.Name = "gvIndexes";
		this.gvIndexes.OptionsView.ShowDetailButtons = false;
		this.gvIndexes.OptionsView.ShowGroupPanel = false;
		this.gvIndexes.OptionsView.ShowIndicator = false;
		this.gvIndexes.UseBatchMod = false;
		this.gvIndexes.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_CellValueChanging);
		this.gvIndexes.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_CellValueChanging);
		this.gcMonth.Caption = "Year";
		this.gcMonth.FieldName = "Month";
		this.gcMonth.Name = "gcMonth";
		this.gcMonth.Width = 83;
		this.gcCP.Caption = "CP";
		this.gcCP.ColumnEdit = this.repositoryItemCalcEdit2;
		this.gcCP.FieldName = "Rate";
		this.gcCP.Name = "gcCP";
		this.gcCP.Visible = true;
		this.gcCP.VisibleIndex = 1;
		this.gcCP.Width = 442;
		this.repositoryItemCalcEdit2.AutoHeight = false;
		this.repositoryItemCalcEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit2.Name = "repositoryItemCalcEdit2";
		this.gc_MonthName.CurrencyDateField = null;
		this.gc_MonthName.CurrencyField = null;
		this.gc_MonthName.CurrencyFixed = null;
		this.gc_MonthName.CurrencyFixedRate = 0.0;
		this.gc_MonthName.CurrencyRateField = null;
		this.gc_MonthName.FieldName = "Name";
		this.gc_MonthName.Name = "gc_MonthName";
		this.gc_MonthName.OptionsColumn.AllowEdit = false;
		this.gc_MonthName.OptionsColumn.ReadOnly = true;
		this.gc_MonthName.PrimaryKey = false;
		this.gc_MonthName.UnitField = null;
		this.gc_MonthName.Visible = true;
		this.gc_MonthName.VisibleIndex = 0;
		this.btAutoExport.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
		this.btAutoExport.Location = new System.Drawing.Point(193, 371);
		this.btAutoExport.Name = "btAutoExport";
		this.btAutoExport.Size = new System.Drawing.Size(98, 23);
		this.btAutoExport.TabIndex = 5;
		this.btAutoExport.Text = "Auto. Export";
		this.btAutoExport.Visible = false;
		this.btAutoExport.Click += new System.EventHandler(this.btAutoExport_Click);
		this.btAutoImport.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
		this.btAutoImport.Location = new System.Drawing.Point(294, 371);
		this.btAutoImport.Name = "btAutoImport";
		this.btAutoImport.Size = new System.Drawing.Size(98, 23);
		this.btAutoImport.TabIndex = 4;
		this.btAutoImport.Text = "Auto. Import";
		this.btAutoImport.Visible = false;
		this.btAutoImport.Click += new System.EventHandler(this.btAutoImport_Click);
		this.btSetRateFSelCriterial.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
		this.btSetRateFSelCriterial.Location = new System.Drawing.Point(394, 371);
		this.btSetRateFSelCriterial.Name = "btSetRateFSelCriterial";
		this.btSetRateFSelCriterial.Size = new System.Drawing.Size(161, 23);
		this.btSetRateFSelCriterial.TabIndex = 3;
		this.btSetRateFSelCriterial.Text = "Set Rate for Selection Criteria";
		this.btSetRateFSelCriterial.Click += new System.EventHandler(this.btSetRateFSelCriterial_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.Location = new System.Drawing.Point(87, 371);
		this.btCancel.Name = "btCancel";
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 2;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOK.ImageOptions.Image");
		this.btOK.Location = new System.Drawing.Point(6, 371);
		this.btOK.Name = "btOK";
		this.btOK.Size = new System.Drawing.Size(75, 23);
		this.btOK.TabIndex = 1;
		this.btOK.Text = "OK";
		this.btOK.Click += new System.EventHandler(this.btOK_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(566, 398);
		base.Controls.Add(this.btAutoExport);
		base.Controls.Add(this.xtraTabControl1);
		base.Controls.Add(this.btOK);
		base.Controls.Add(this.btAutoImport);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btSetRateFSelCriterial);
		base.FormId = "oExchangeRateAndIndex";
		base.Name = "frm_ExchangeRatesAndIndexs";
		base.ShowIcon = false;
		this.Text = "Exchange Rates And Indexes";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_ExchangeRatesAndIndexs_FormClosing);
		base.Shown += new System.EventHandler(this.frm_ExchangeRatesAndIndexs_Shown);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xtp_ExchangeRate.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cbRateYear.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbMonth.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdRates).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvRates).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		this.xtp_Indexes.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cbIndexesYear.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdIndexes).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvIndexes).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).EndInit();
		base.ResumeLayout(false);
	}
}
