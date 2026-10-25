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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources.Salary;

public class frm_SalaryParameters : XtraFormEx
{
	private DataTable _dtdoc;

	private bool _isLoading;

	private List<CurrencyCodesImpl> salaryCurrData;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private string _priceOldValue = "";

	private bool _changingPrice;

	private IContainer components;

	private GridControlEx gridControl1;

	private GridViewEx gridView1;

	private GridColumnEx Code;

	private GridColumnEx ValidFrom;

	private RepositoryItemDateEditEx rep_dateFrom;

	private GridColumnEx ValidTo;

	private RepositoryItemDateEditEx rep_dateTo;

	private GridColumnEx BasicSal;

	private GridColumnEx BasicCur;

	private RepositoryItemLookUpEdit rep_looCuB;

	private GridColumnEx PITSal;

	private GridColumnEx PITCur;

	private RepositoryItemLookUpEdit rep_looCuP;

	private GridColumnEx AmtRelat;

	private GridColumnEx AmtRelatCur;

	private RepositoryItemLookUpEdit rep_looCuE;

	private SimpleButtonEx btn_Ok;

	private SimpleButtonEx btn_Cancel;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private ToolStripMenuItem dupplicateToolStripMenuItem;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice1;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice2;

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
		case Enums.FormViewModeType.Update:
			btn_Ok.Text = Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.View:
			btn_Ok.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_SalaryParameters()
	{
		InitializeComponent();
		FormId = "oSalaryParameters";
		gridView1.FormId = FormId;
		gridView1.ItemId = "APZ_OHSP";
		OnLoadAsync += frm_SalaryParameters_Load;
	}

	private void gridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private async Task frm_SalaryParameters_Load(object sender, EventArgs e)
	{
		try
		{
			SetDataCurrency();
			DataTable dataSource = await GetList();
			gridControl1.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		REFRESH();
	}

	private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (gridView1.GetSelectedRows() == null || gridView1.GetSelectedRows().Count() == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteSelectedRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				if (string.IsNullOrEmpty(Function.ToString(gridView1.GetFocusedRowCellValue("Code"))))
				{
					gridControl1.BeginUpdate();
					gridView1.DeleteSelectedRows();
					gridView1.EndUpdate();
					return;
				}
				int[] selectedRows = gridView1.GetSelectedRows();
				string text = "";
				int[] array = selectedRows;
				foreach (int rowHandle in array)
				{
					text = text + gridView1.GetRowCellValue(rowHandle, "Code").ToString() + ",";
				}
				if (text.Length > 0)
				{
					text = text.Remove(text.Length - 1);
					string text2 = await _httpClient.SalaryParameters.DeleteDocs(text);
					if (string.IsNullOrEmpty(text2))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						GridControlEx gridControlEx = gridControl1;
						gridControlEx.DataSource = await GetList();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, text2);
					}
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void dupplicateToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DateTime ID = Function.ParseDateTimes(gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "ValidFrom"));
			DataTable dataSourceTable = gridControl1.DataSourceTable;
			DataRow[] array = (from t in dataSourceTable.AsEnumerable()
				where Function.ParseDateTimes(t["ValidFrom"]) == ID
				select t).ToArray();
			if (array != null && array.Length != 0)
			{
				for (int num = 0; num < array.Count(); num++)
				{
					DataRow dataRow = dataSourceTable.NewRow();
					dataRow.ItemArray = array[num].ItemArray;
					dataSourceTable.Rows.Add(dataRow);
				}
				gridControl1.DataSource = dataSourceTable;
			}
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				WinCommons._OpenProcessing();
				btn_Ok.Enabled = false;
				await Saves();
			}
			else if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btn_Ok.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gridView1_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			string currencyTrial = GlobalData.MainCurrency;
			int num = -1;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currencyTrial.ToUpper()));
				if (currencyCodesImpl != null)
				{
					num = currencyCodesImpl.Decimals;
				}
			}
			string currencyB = Function.ToString(gridView1.GetListSourceRowCellValue(e.ListSourceRowIndex, "BasicCur"));
			int num2 = -1;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl2 = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currencyB.ToUpper()));
				if (currencyCodesImpl2 != null)
				{
					num2 = currencyCodesImpl2.Decimals;
				}
			}
			if (string.IsNullOrEmpty(currencyB))
			{
				if (e.Column.FieldName.ToUpper().Equals("BASICSAL"))
				{
					e.DisplayText = string.Format("{0:n" + ((num == -1) ? GlobalData.UnitPriceFormat.NumberDecimalDigits : num) + "} {1}", e.Value, currencyTrial.ToUpper());
				}
			}
			else if (e.Column.FieldName.ToUpper().Equals("BASICSAL"))
			{
				e.DisplayText = string.Format("{0:n" + ((num2 == -1) ? GlobalData.UnitPriceFormat.NumberDecimalDigits : num2) + "} {1}", e.Value, currencyB.ToUpper());
			}
			string currencyP = Function.ToString(gridView1.GetListSourceRowCellValue(e.ListSourceRowIndex, "PITCur"));
			int num3 = -1;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl3 = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currencyP.ToUpper()));
				if (currencyCodesImpl3 != null)
				{
					num3 = currencyCodesImpl3.Decimals;
				}
			}
			if (string.IsNullOrEmpty(currencyP))
			{
				if (e.Column.FieldName.ToUpper().Equals("PITSAL") && Function.ParseDouble(e.Value) > 0.0)
				{
					e.DisplayText = string.Format("{0:n" + ((num == -1) ? GlobalData.UnitPriceFormat.NumberDecimalDigits : num) + "} {1}", e.Value, currencyTrial.ToUpper());
				}
			}
			else if (e.Column.FieldName.ToUpper().Equals("PITSAL") && Function.ParseDouble(e.Value) > 0.0)
			{
				e.DisplayText = string.Format("{0:n" + ((num3 == -1) ? GlobalData.UnitPriceFormat.NumberDecimalDigits : num3) + "} {1}", e.Value, currencyP.ToUpper());
			}
			string currencyE = Function.ToString(gridView1.GetListSourceRowCellValue(e.ListSourceRowIndex, "AmtRelatCur"));
			int num4 = -1;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl4 = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currencyE.ToUpper()));
				if (currencyCodesImpl4 != null)
				{
					num4 = currencyCodesImpl4.Decimals;
				}
			}
			if (string.IsNullOrEmpty(currencyE))
			{
				if (e.Column.FieldName.ToUpper().Equals("AMTRELAT") && Function.ParseDouble(e.Value) > 0.0)
				{
					e.DisplayText = string.Format("{0:n" + ((num == -1) ? GlobalData.UnitPriceFormat.NumberDecimalDigits : num) + "} {1}", e.Value, currencyTrial.ToUpper());
				}
			}
			else if (e.Column.FieldName.ToUpper().Equals("AMTRELAT") && Function.ParseDouble(e.Value) > 0.0)
			{
				e.DisplayText = string.Format("{0:n" + ((num4 == -1) ? GlobalData.UnitPriceFormat.NumberDecimalDigits : num4) + "} {1}", e.Value, currencyE.ToUpper());
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridView1_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			if (gridView1 == null)
			{
				return;
			}
			GridColumn focusedColumn = gridView1.FocusedColumn;
			int focusedRowHandle = gridView1.FocusedRowHandle;
			List<string> currency = Function.GetCurrency(Function.ToString(e.Value));
			DateTime lineNum = Function.ParseDateTimes(gridView1.GetRowCellValue(focusedRowHandle, "ValidFrom"));
			DataRow dataRow = gridControl1.DataSourceTable.AsEnumerable().FirstOrDefault((DataRow s) => Function.ParseDateTimes(s["ValidFrom"]) == lineNum);
			if (focusedColumn.FieldName.ToUpper().Equals("BASICSAL"))
			{
				if (currency.Count > 0 && dataRow != null)
				{
					decimal num = Function.ParseDecimal(currency[0]);
					string alpha = Function.ToString(currency[1]);
					if (string.IsNullOrEmpty(alpha))
					{
						alpha = Function.ToString(dataRow["BasicCur"]);
					}
					if (GlobalData.Currencies.Exists((Common.Table_Code t) => t.Code.ToUpper().Equals(alpha.ToUpper())))
					{
						dataRow["BasicCur"] = alpha;
						e.Value = num;
						gridView1.SetRowCellValue(focusedRowHandle, focusedColumn, num);
					}
				}
			}
			else if (focusedColumn.FieldName.ToUpper().Equals("PITSAL"))
			{
				if (currency.Count > 0 && dataRow != null)
				{
					decimal num2 = Function.ParseDecimal(currency[0]);
					string alpha2 = Function.ToString(currency[1]);
					if (string.IsNullOrEmpty(alpha2))
					{
						alpha2 = Function.ToString(dataRow["PITCur"]);
					}
					if (GlobalData.Currencies.Exists((Common.Table_Code t) => t.Code.ToUpper().Equals(alpha2.ToUpper())))
					{
						dataRow["PITCur"] = alpha2;
						e.Value = num2;
						gridView1.SetRowCellValue(focusedRowHandle, focusedColumn, num2);
					}
				}
			}
			else if (focusedColumn.FieldName.ToUpper().Equals("AMTRELAT") && currency.Count > 0 && dataRow != null)
			{
				decimal num3 = Function.ParseDecimal(currency[0]);
				string alpha3 = Function.ToString(currency[1]);
				if (string.IsNullOrEmpty(alpha3))
				{
					alpha3 = Function.ToString(dataRow["AmtRelatCur"]);
				}
				if (GlobalData.Currencies.Exists((Common.Table_Code t) => t.Code.ToUpper().Equals(alpha3.ToUpper())))
				{
					dataRow["AmtRelatCur"] = alpha3;
					e.Value = num3;
					gridView1.SetRowCellValue(focusedRowHandle, focusedColumn, num3);
				}
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridView1_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_ = gridView1.FocusedColumn;
			int rowHandle = e.RowHandle;
			List<string> currency = Function.GetCurrency(Function.ToString(e.Value));
			DateTime lineNum = Function.ParseDateTimes(gridView1.GetRowCellValue(rowHandle, "ValidFrom"));
			DataRow dataRow = gridControl1.DataSourceTable.AsEnumerable().FirstOrDefault((DataRow s) => Function.ParseDateTimes(s["ValidFrom"]) == lineNum);
			int num = GlobalData.UnitPriceFormat.NumberDecimalDigits;
			string alpha = Function.ToString(currency[1]);
			if (e.Column.FieldName.Equals("BasicSal"))
			{
				double num2 = Function.ParseDouble(gridView1.GetRowCellValue(e.RowHandle, "BasicSal"));
				if (!string.IsNullOrEmpty(alpha) && dataRow != null)
				{
					alpha = Function.ToString(dataRow["BasicCur"]);
				}
				if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
				{
					CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => alpha != null && t.CurrCode.ToUpper().Equals(alpha.ToUpper()));
					if (currencyCodesImpl != null)
					{
						num = ((currencyCodesImpl.Decimals == -1) ? num : currencyCodesImpl.Decimals);
					}
				}
				if (!string.IsNullOrEmpty(alpha))
				{
					double num3 = Math.Round(num2, num);
					if (num3 != num2)
					{
						gridView1.SetRowCellValue(e.RowHandle, "BasicSal", num3);
						return;
					}
				}
			}
			if (e.Column.FieldName.Equals("PITSal"))
			{
				double num4 = Function.ParseDouble(gridView1.GetRowCellValue(e.RowHandle, "PITSal"));
				if (string.IsNullOrEmpty(alpha) && dataRow != null)
				{
					alpha = Function.ToString(dataRow["PITCur"]);
				}
				if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
				{
					CurrencyCodesImpl currencyCodesImpl2 = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => alpha != null && t.CurrCode.ToUpper().Equals(alpha.ToUpper()));
					if (currencyCodesImpl2 != null)
					{
						num = ((currencyCodesImpl2.Decimals == -1) ? num : currencyCodesImpl2.Decimals);
					}
				}
				if (!string.IsNullOrEmpty(alpha))
				{
					double num5 = Math.Round(num4, num);
					if (num5 != num4)
					{
						gridView1.SetRowCellValue(e.RowHandle, "PITSal", num5);
						return;
					}
				}
			}
			if (!e.Column.FieldName.Equals("AmtRelat"))
			{
				return;
			}
			double num6 = Function.ParseDouble(gridView1.GetRowCellValue(e.RowHandle, "AmtRelat"));
			if (!string.IsNullOrEmpty(alpha) && dataRow != null)
			{
				alpha = Function.ToString(dataRow["AmtRelatCur"]);
			}
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl3 = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(alpha.ToUpper()));
				if (currencyCodesImpl3 != null)
				{
					num = ((currencyCodesImpl3.Decimals == -1) ? num : currencyCodesImpl3.Decimals);
				}
			}
			if (!string.IsNullOrEmpty(alpha))
			{
				double num7 = Math.Round(num6, num);
				if (num7 != num6)
				{
					gridView1.SetRowCellValue(e.RowHandle, "AmtRelat", num7);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridView1_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
	}

	public async Task REFRESH()
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
			_dtdoc = gridControl1.DataSourceTable;
			if (_dtdoc == null && _dtdoc.Columns.Count < 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			DataRow row = _dtdoc.NewRow();
			_dtdoc.Rows.Add(row);
			gridControl1.DataSource = _dtdoc;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task<DataTable> GetList()
	{
		try
		{
			return await _httpClient.SalaryParameters.GetList();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<int> Saves()
	{
		_ = 1;
		try
		{
			_dtdoc = gridControl1.DataSourceTable;
			_dtdoc.AcceptChanges();
			foreach (DataRow item in _dtdoc.AsEnumerable())
			{
				if (!(Function.ToString(item["ValidFrom"]) != ""))
				{
					if (_dtdoc.Rows.IndexOf(item) != _dtdoc.Rows.Count - 1)
					{
						WinCommons.Error(Enums.MessageCode.ValidFromFieldCannotBeNull, "");
						return 1;
					}
					if (Function.ToString(item["ValidFrom"]) != "")
					{
						WinCommons.Error(Enums.MessageCode.ValidFromFieldCannotBeNull, "");
						return 1;
					}
				}
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				_dtdoc = gridControl1.DataSourceTable;
				for (int i = 0; i < _dtdoc.Rows.Count; i++)
				{
					if (Function.ToString(_dtdoc.Rows[i]["ValidFrom"]) == "")
					{
						_dtdoc.Rows[i].Delete();
					}
				}
				HttpResult httpResult = await _httpClient.SalaryParameters.Saves(_dtdoc);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					GridControlEx gridControlEx = gridControl1;
					gridControlEx.DataSource = await GetList();
					FormMode = Enums.FormViewModeType.View;
					return 0;
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
		return 1;
	}

	public void SetDataCurrency()
	{
		try
		{
			salaryCurrData = GlobalData.CurrencyCodes;
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("CurrCode", typeof(string));
			dataTable.Columns.Add("CurrName", typeof(string));
			foreach (CurrencyCodesImpl salaryCurrDatum in salaryCurrData)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["CurrCode"] = salaryCurrDatum.CurrCode;
				dataRow["CurrName"] = salaryCurrDatum.CurrName;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			rep_looCuB.DataSource = dataTable.Copy();
			rep_looCuB.NullText = "";
			rep_looCuB.ValueMember = "CurrCode";
			rep_looCuB.DisplayMember = "CurrCode";
			rep_looCuP.DataSource = dataTable.Copy();
			rep_looCuP.NullText = "";
			rep_looCuP.ValueMember = "CurrCode";
			rep_looCuP.DisplayMember = "CurrCode";
			rep_looCuE.DataSource = dataTable.Copy();
			rep_looCuE.NullText = "";
			rep_looCuE.ValueMember = "CurrCode";
			rep_looCuE.DisplayMember = "CurrCode";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_SalaryParameters_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.Salary.frm_SalaryParameters));
		this.repositoryItemTextEditExPrice1 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice2 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.gridControl1 = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.dupplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gridView1 = new Apzon.Controls.Grid.GridViewEx();
		this.Code = new Apzon.Controls.Grid.GridColumnEx();
		this.ValidFrom = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_dateFrom = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.ValidTo = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_dateTo = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.BasicSal = new Apzon.Controls.Grid.GridColumnEx();
		this.BasicCur = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_looCuB = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.PITSal = new Apzon.Controls.Grid.GridColumnEx();
		this.PITCur = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_looCuP = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.AmtRelat = new Apzon.Controls.Grid.GridColumnEx();
		this.AmtRelatCur = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_looCuE = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateFrom).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateFrom.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateTo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateTo.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_looCuB).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_looCuP).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_looCuE).BeginInit();
		base.SuspendLayout();
		this.repositoryItemTextEditExPrice1.Name = "repositoryItemTextEditExPrice1";
		this.repositoryItemTextEditExPrice2.Name = "repositoryItemTextEditExPrice2";
		this.gridControl1.AllowPasteExcel = true;
		this.gridControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridControl1.ClipboardData = null;
		this.gridControl1.ContextMenuStrip = this.contextMenuStrip1;
		this.gridControl1.Location = new System.Drawing.Point(-1, 2);
		this.gridControl1.MainView = this.gridView1;
		this.gridControl1.Name = "gridControl1";
		this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rep_dateFrom, this.rep_dateTo, this.rep_looCuB, this.rep_looCuP, this.rep_looCuE });
		this.gridControl1.Size = new System.Drawing.Size(876, 277);
		this.gridControl1.TabIndex = 0;
		this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView1 });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addNewToolStripMenuItem, this.deleteToolStripMenuItem, this.dupplicateToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(132, 70);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
		this.addNewToolStripMenuItem.Text = "Add new";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.dupplicateToolStripMenuItem.Name = "dupplicateToolStripMenuItem";
		this.dupplicateToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
		this.dupplicateToolStripMenuItem.Text = "Dupplicate";
		this.dupplicateToolStripMenuItem.Click += new System.EventHandler(this.dupplicateToolStripMenuItem_Click);
		this.gridView1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView1.ColumnKey = null;
		this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.Code, this.ValidFrom, this.ValidTo, this.BasicSal, this.BasicCur, this.PITSal, this.PITCur, this.AmtRelat, this.AmtRelatCur });
		this.gridView1.DisableFormSetting = false;
		this.gridView1.DisplayException = false;
		this.gridView1.EditColumn = null;
		this.gridView1.FieldCurrency = null;
		this.gridView1.FieldRate = null;
		this.gridView1.FormId = "700024";
		this.gridView1.GridControl = this.gridControl1;
		this.gridView1.IgnoreFilter = false;
		this.gridView1.IndexEditRow = -1;
		this.gridView1.IndicatorWidth = 30;
		this.gridView1.IsApzTable = false;
		this.gridView1.IsInsertNewRow = false;
		this.gridView1.IsLoadingData = false;
		this.gridView1.IsMappingContextAction = false;
		this.gridView1.ItemId = "APZ_OHSP";
		this.gridView1.LstColumnAllowBatch = null;
		this.gridView1.LstColumnNotAllowBatch = null;
		this.gridView1.LstColumnNotEditable = null;
		this.gridView1.LstColumNotSetting = null;
		this.gridView1.Name = "gridView1";
		this.gridView1.OptionsCustomization.AllowSort = false;
		this.gridView1.OptionsView.ShowGroupPanel = false;
		this.gridView1.UseBatchMod = false;
		this.gridView1.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridView1_CustomDrawRowIndicator);
		this.gridView1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanged);
		this.gridView1.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanging);
		this.gridView1.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridView1_CustomColumnDisplayText);
		this.gridView1.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gridView1_ValidatingEditor);
		this.Code.Caption = "Code";
		this.Code.CurrencyDateField = null;
		this.Code.CurrencyField = null;
		this.Code.CurrencyFixed = null;
		this.Code.CurrencyFixedRate = 0.0;
		this.Code.CurrencyRateField = null;
		this.Code.FieldName = "Code";
		this.Code.Name = "Code";
		this.Code.PrimaryKey = false;
		this.Code.UnitField = null;
		this.ValidFrom.Caption = "Valid From";
		this.ValidFrom.ColumnEdit = this.rep_dateFrom;
		this.ValidFrom.CurrencyDateField = null;
		this.ValidFrom.CurrencyField = null;
		this.ValidFrom.CurrencyFixed = null;
		this.ValidFrom.CurrencyFixedRate = 0.0;
		this.ValidFrom.CurrencyRateField = null;
		this.ValidFrom.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.ValidFrom.FieldName = "ValidFrom";
		this.ValidFrom.Name = "ValidFrom";
		this.ValidFrom.PrimaryKey = true;
		this.ValidFrom.UnitField = null;
		this.ValidFrom.Visible = true;
		this.ValidFrom.VisibleIndex = 0;
		this.rep_dateFrom.AutoHeight = false;
		this.rep_dateFrom.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_dateFrom.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_dateFrom.IsSearchControl = false;
		this.rep_dateFrom.Name = "rep_dateFrom";
		this.ValidTo.Caption = "Valid To";
		this.ValidTo.ColumnEdit = this.rep_dateTo;
		this.ValidTo.CurrencyDateField = null;
		this.ValidTo.CurrencyField = null;
		this.ValidTo.CurrencyFixed = null;
		this.ValidTo.CurrencyFixedRate = 0.0;
		this.ValidTo.CurrencyRateField = null;
		this.ValidTo.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.ValidTo.FieldName = "ValidTo";
		this.ValidTo.Name = "ValidTo";
		this.ValidTo.PrimaryKey = false;
		this.ValidTo.UnitField = null;
		this.ValidTo.Visible = true;
		this.ValidTo.VisibleIndex = 1;
		this.rep_dateTo.AutoHeight = false;
		this.rep_dateTo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_dateTo.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_dateTo.IsSearchControl = false;
		this.rep_dateTo.Name = "rep_dateTo";
		this.BasicSal.Caption = "Basic Salary";
		this.BasicSal.ColumnEdit = this.repositoryItemTextEditExPrice1;
		this.BasicSal.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.BasicSal.CurrencyDateField = null;
		this.BasicSal.CurrencyField = "BasicCur";
		this.BasicSal.CurrencyFixed = null;
		this.BasicSal.CurrencyFixedRate = 0.0;
		this.BasicSal.CurrencyRateField = null;
		this.BasicSal.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.BasicSal.FieldName = "BasicSal";
		this.BasicSal.Name = "BasicSal";
		this.BasicSal.PrimaryKey = false;
		this.BasicSal.UnitField = null;
		this.BasicSal.Visible = true;
		this.BasicSal.VisibleIndex = 2;
		this.BasicCur.Caption = "BasicCur";
		this.BasicCur.ColumnEdit = this.rep_looCuB;
		this.BasicCur.CurrencyDateField = null;
		this.BasicCur.CurrencyField = null;
		this.BasicCur.CurrencyFixed = null;
		this.BasicCur.CurrencyFixedRate = 0.0;
		this.BasicCur.CurrencyRateField = null;
		this.BasicCur.FieldName = "BasicCur";
		this.BasicCur.Name = "BasicCur";
		this.BasicCur.PrimaryKey = false;
		this.BasicCur.UnitField = null;
		this.rep_looCuB.AutoHeight = false;
		this.rep_looCuB.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_looCuB.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrName", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrCode", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_looCuB.DisplayMember = "CurrName";
		this.rep_looCuB.Name = "rep_looCuB";
		this.rep_looCuB.NullText = "";
		this.rep_looCuB.ValueMember = "CurrCode";
		this.PITSal.Caption = "PIT Salary";
		this.PITSal.ColumnEdit = this.repositoryItemTextEditExPrice2;
		this.PITSal.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.PITSal.CurrencyDateField = null;
		this.PITSal.CurrencyField = "PITCur";
		this.PITSal.CurrencyFixed = null;
		this.PITSal.CurrencyFixedRate = 0.0;
		this.PITSal.CurrencyRateField = null;
		this.PITSal.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.PITSal.FieldName = "PITSal";
		this.PITSal.Name = "PITSal";
		this.PITSal.PrimaryKey = false;
		this.PITSal.UnitField = null;
		this.PITSal.Visible = true;
		this.PITSal.VisibleIndex = 3;
		this.PITCur.Caption = "PITCur";
		this.PITCur.ColumnEdit = this.rep_looCuP;
		this.PITCur.CurrencyDateField = null;
		this.PITCur.CurrencyField = null;
		this.PITCur.CurrencyFixed = null;
		this.PITCur.CurrencyFixedRate = 0.0;
		this.PITCur.CurrencyRateField = null;
		this.PITCur.FieldName = "PITCur";
		this.PITCur.Name = "PITCur";
		this.PITCur.PrimaryKey = false;
		this.PITCur.UnitField = null;
		this.rep_looCuP.AutoHeight = false;
		this.rep_looCuP.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_looCuP.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrName", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrCode", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_looCuP.DisplayMember = "CurrName";
		this.rep_looCuP.Name = "rep_looCuP";
		this.rep_looCuP.NullText = "";
		this.rep_looCuP.ValueMember = "CurrCode";
		this.AmtRelat.Caption = "Exampt amount for relatives";
		this.AmtRelat.CurrencyDateField = null;
		this.AmtRelat.CurrencyField = null;
		this.AmtRelat.CurrencyFixed = null;
		this.AmtRelat.CurrencyFixedRate = 0.0;
		this.AmtRelat.CurrencyRateField = null;
		this.AmtRelat.FieldName = "AmtRelat";
		this.AmtRelat.Name = "AmtRelat";
		this.AmtRelat.PrimaryKey = false;
		this.AmtRelat.UnitField = null;
		this.AmtRelat.Visible = true;
		this.AmtRelat.VisibleIndex = 4;
		this.AmtRelatCur.Caption = "AmtRelatCur";
		this.AmtRelatCur.ColumnEdit = this.rep_looCuE;
		this.AmtRelatCur.CurrencyDateField = null;
		this.AmtRelatCur.CurrencyField = null;
		this.AmtRelatCur.CurrencyFixed = null;
		this.AmtRelatCur.CurrencyFixedRate = 0.0;
		this.AmtRelatCur.CurrencyRateField = null;
		this.AmtRelatCur.FieldName = "AmtRelatCur";
		this.AmtRelatCur.Name = "AmtRelatCur";
		this.AmtRelatCur.PrimaryKey = false;
		this.AmtRelatCur.UnitField = null;
		this.rep_looCuE.AutoHeight = false;
		this.rep_looCuE.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_looCuE.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrName", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrCode", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_looCuE.DisplayMember = "CurrName";
		this.rep_looCuE.Name = "rep_looCuE";
		this.rep_looCuE.NullText = "";
		this.rep_looCuE.ValueMember = "CurrCode";
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "OK";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(2, 284);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.ReadOnly = false;
		this.btn_Ok.SettingInVisible = false;
		this.btn_Ok.SettingReadonly = false;
		this.btn_Ok.Size = new System.Drawing.Size(75, 23);
		this.btn_Ok.TabIndex = 1;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(80, 284);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AcceptButton = this.btn_Ok;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(874, 309);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.gridControl1);
		base.FormId = "700024";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SalaryParameters";
		this.Text = "Salary Parameters";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_SalaryParameters_Load);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_SalaryParameters_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateFrom.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateFrom).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateTo.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateTo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_looCuB).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_looCuP).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_looCuE).EndInit();
		base.ResumeLayout(false);
	}
}
