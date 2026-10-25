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
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Payment;

public class frm_CashItemMasterData : XtraFormEx
{
	private const string OkString = "OK";

	private const string UpdateString = "Update";

	private const string AddString = "Add";

	private DataTable _dtdoc;

	private DataTable _dtLine;

	private List<CurrencyCodesImpl> salaryCurrData;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Cash;

	private GridViewEx gv_Cash;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx xtraTabPage1;

	private GridControlEx grd_Detail;

	private GridViewEx gv_Detail;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private RepositoryItemLookUpEditSimple luk_Group;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private RepositoryItemLookUpEditSimple luk_Type;

	private RepositoryItemLookUpEditDefineNew luk_Currency;

	private RepositoryItemLookUpEditSimple luk_Default;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gridColumnEx11;

	private GridColumnEx gridColumnEx12;

	private GridColumnEx gridColumnEx13;

	private RepositoryItemLookUpEdit luk_Curr;

	private SimpleButtonEx btn_Ok;

	private SimpleButtonEx btn_Cancel;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem addRowToolStripMenuItem1;

	private ToolStripMenuItem deleteRowToolStripMenuItem1;

	private GridColumnEx gridColumnEx14;

	private RepositoryItemTextEdit txt_code;

	private ToolStripMenuItem defaultToolStripMenuItem;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice1;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
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
				btn_Ok.Text = "Add";
				txt_code.ReadOnly = false;
				break;
			case Enums.FormViewModeType.Update:
				btn_Ok.Text = "Update";
				txt_code.ReadOnly = true;
				break;
			case Enums.FormViewModeType.View:
				btn_Ok.Text = "OK";
				txt_code.ReadOnly = true;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public frm_CashItemMasterData()
	{
		InitializeComponent();
		OnLoadAsync += frm_CashItemMasterData_Load;
	}

	private async Task frm_CashItemMasterData_Load(object sender, EventArgs e)
	{
		try
		{
			SetDataCurrency();
			SetDataDefault();
			SetDataGroup();
			SetDataType();
			DataTable dataTable = await GetDataAllCash();
			grd_Cash.DataSource = dataTable;
			if (!dataTable.IsNotNull())
			{
				gv_Cash_FocusedRowObjectChanged(null, null);
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Cash_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_Detail_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Add:
				await Saves(1);
				break;
			case Enums.FormViewModeType.Update:
				await Saves(2);
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
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
		Close();
	}

	private async void gv_Cash_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			await GetDetailByCode(Function.ToString(gv_Cash.GetFocusedRowCellValue("CashCode")));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Cash_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Currency")
			{
				Function.ToString(e.Value);
				_dtLine = grd_Detail.DataSourceTable;
				if (_dtLine != null && _dtLine.Rows.Count > 0)
				{
					for (int i = 0; i < _dtLine.Rows.Count; i++)
					{
						_dtLine.Rows[i]["Currency"] = Function.ToString(gv_Cash.GetFocusedRowCellValue("Currency"));
					}
					_dtLine.AcceptChanges();
					grd_Detail.DataSource = _dtLine;
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

	private void gv_Detail_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_dtLine = grd_Detail.DataSourceTable;
			bool flag = false;
			for (int i = 0; i < _dtLine.Rows.Count; i++)
			{
				if (!string.IsNullOrEmpty(Function.ToString(_dtLine.Rows[_dtLine.Rows.Count - 1]["InNumber"])))
				{
					flag = true;
				}
			}
			if (flag)
			{
				AddNewDetail();
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

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
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

	private async void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (gv_Cash.GetSelectedRows() == null || gv_Cash.GetSelectedRows().Count() == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteSelectedRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				int[] selectedRows = gv_Cash.GetSelectedRows();
				string text = "";
				int[] array = selectedRows;
				foreach (int rowHandle in array)
				{
					text = text + gv_Cash.GetRowCellValue(rowHandle, "CashCode").ToString() + ",";
				}
				if (text.Length > 0)
				{
					text = text.Remove(text.Length - 1);
					string text2 = await _httpClient.CashItemMasterData.DeleteDocs(text);
					if (string.IsNullOrEmpty(text2))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						GridControlEx gridControlEx = grd_Cash;
						gridControlEx.DataSource = await GetDataAllCash();
						grd_Detail.DataSource = null;
						gv_Detail.RefreshData();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, text2);
						WinCommons.Error(Enums.MessageCode.DeleteError, "");
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		AddNewDetail();
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void deleteRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			int focusedRowHandle = gv_Detail.FocusedRowHandle;
			_dtLine.Rows.RemoveAt(focusedRowHandle);
			grd_Detail.DataSource = _dtLine;
			grd_Detail.RefreshDataSource();
			if (FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Cash_ShowingEditor(object sender, CancelEventArgs e)
	{
	}

	private void defaultToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			_dtdoc = grd_Cash.DataSourceTable;
			for (int i = 0; i < _dtdoc.Rows.Count; i++)
			{
				_dtdoc.Rows[i]["Default"] = "N";
			}
			_dtdoc.AcceptChanges();
			grd_Cash.DataSource = _dtdoc;
			gv_Cash.SetFocusedRowCellValue("Default", "Y");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Cash_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "CashCode")
			{
				e.Value.ToString().Trim();
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

	private void gv_Cash_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		if (Function.ToString(gv_Cash.GetRowCellValue(e.RowHandle, "Default")).Equals("Y"))
		{
			e.Appearance.BackColor = Color.LightGray;
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.Previous) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.Next) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_FIRST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.First) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.Last) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH()
	{
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			grd_Cash.DataSourceTable.Rows.Clear();
			grd_Detail.DataSourceTable.Rows.Clear();
			_dtdoc = grd_Cash.DataSourceTable.Clone();
			if (_dtdoc == null && _dtdoc.Columns.Count < 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			DataRow row = _dtdoc.NewRow();
			_dtdoc.Rows.Add(row);
			grd_Cash.DataSource = _dtdoc;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void AddNewDetail()
	{
		_dtLine = grd_Detail.DataSourceTable;
		if (_dtLine == null && _dtLine.Columns.Count < 0)
		{
			WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
		}
		DataRow row = _dtLine.NewRow();
		_dtLine.Rows.Add(row);
		grd_Detail.DataSource = _dtLine;
	}

	public async Task REFRESH_DATA()
	{
		_ = 1;
		try
		{
			FormMode = Enums.FormViewModeType.View;
			DataTable dataTable = await GetDataAllCash();
			if (dataTable.IsNotNull())
			{
				grd_Cash.DataSource = dataTable;
				grd_Cash.RefreshDataSource();
				string cashCode = Function.ToString(gv_Cash.GetFocusedRowCellValue("CashCode"));
				await GetDetailByCode(cashCode);
			}
			else
			{
				grd_Cash.DataSource = null;
				grd_Cash.RefreshDataSource();
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
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
			luk_Curr.DataSource = dataTable.Copy();
			luk_Curr.NullText = "";
			luk_Curr.ValueMember = "CurrCode";
			luk_Curr.DisplayMember = "CurrCode";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetDataType()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		list.Add(new Common.Table_Code
		{
			Name = "Cash",
			Code = "C"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Non-Cash",
			Code = "N"
		});
		luk_Type.DataSource = list;
	}

	private void SetDataGroup()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		list.Add(new Common.Table_Code
		{
			Name = "Bank",
			Code = "B"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Cash",
			Code = "C"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Cheque",
			Code = "H"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Non-Cash",
			Code = "N"
		});
		luk_Group.DataSource = list;
	}

	private void SetDataDefault()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		list.Add(new Common.Table_Code
		{
			Name = "Yes",
			Code = "Y"
		});
		list.Add(new Common.Table_Code
		{
			Name = "No",
			Code = "N"
		});
		luk_Default.DataSource = list;
	}

	public async Task<DataTable> GetDataAllCash()
	{
		try
		{
			return await _httpClient.CashItemMasterData.GetAllCash();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public async Task GetDetailByCode(string cashCode)
	{
		try
		{
			WinCommons._OpenProcessing();
			this.ResetData();
			DataTable dataTable = await _httpClient.CashItemMasterData.GetDetailByCode(cashCode);
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				dataTable.Rows[i]["Currency"] = Function.ToString(gv_Cash.GetFocusedRowCellValue("Currency"));
			}
			grd_Detail.DataSource = dataTable;
			AddNewDetail();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> Saves(int type)
	{
		_ = 2;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				DataSet ds = new DataSet();
				switch (type)
				{
				case 2:
				{
					string arg = Function.ToString(gv_Cash.GetFocusedRowCellValue("CashCode"));
					_dtdoc = grd_Cash.DataSourceTable;
					DataRow[] array = _dtdoc.Select($"CashCode='{arg}'");
					if (array.Length != 0)
					{
						_dtdoc = array.CopyToDataTable();
					}
					break;
				}
				case 1:
					_dtdoc = grd_Cash.DataSourceTable;
					break;
				}
				for (int i = 0; i < _dtdoc.Rows.Count; i++)
				{
					if (Function.ToString(_dtdoc.Rows[i]["CashCode"]) == "")
					{
						_dtdoc.Rows.RemoveAt(i--);
					}
				}
				_dtLine.AcceptChanges();
				if (grd_Detail.DataSource != null)
				{
					_dtLine = grd_Detail.DataSourceTable;
					for (int j = 0; j < _dtLine.Rows.Count; j++)
					{
						if (Function.ParseDouble(_dtLine.Rows[j]["InNumber"]) <= 0.0)
						{
							_dtLine.Rows.RemoveAt(j--);
						}
					}
					_dtLine.AcceptChanges();
				}
				_dtdoc.TableName = "Doc";
				_dtLine.TableName = "Line";
				ds.Tables.Add(_dtdoc.Copy());
				ds.Tables.Add(_dtLine.Copy());
				HttpResult httpResult = null;
				if (type == 1)
				{
					httpResult = await _httpClient.CashItemMasterData.Add(ds);
				}
				if (type == 2)
				{
					httpResult = await _httpClient.CashItemMasterData.Update(ds);
				}
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					GridControlEx gridControlEx = grd_Cash;
					gridControlEx.DataSource = await GetDataAllCash();
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
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return 1;
		}
		return 1;
	}

	public async Task<int> FindDataNextLast(Enums.NavigateType type)
	{
		_ = 1;
		try
		{
			DataTable dataTable = await _httpClient.CashItemMasterData.FindDataNextLast(Function.ToString(gv_Cash.GetFocusedRowCellValue("CashCode")), (int)type);
			if (dataTable.IsNotNull())
			{
				grd_Cash.DataSource = dataTable;
				string cashCode = Function.ToString(dataTable.Rows[0]["CashCode"]);
				await GetDetailByCode(cashCode);
				FormMode = Enums.FormViewModeType.View;
				return 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	private void frm_CashItemMasterData_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (btn_Ok.Text == Resources.ButtonUpdate && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			btn_Ok_Click(null, null);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Payment.frm_CashItemMasterData));
		this.repositoryItemTextEditExPrice1 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Cash = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.defaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Cash = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_code = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Type = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Group = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Curr = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Default = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Currency = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Detail = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Detail = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Cash).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Cash).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_code).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Group).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Curr).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Default).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Currency).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Detail).BeginInit();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Detail).BeginInit();
		base.SuspendLayout();
		this.repositoryItemTextEditExPrice1.Name = "repositoryItemTextEditExPrice1";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Cash);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControl1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(729, 367);
		this.splitContainerControl1.SplitterPosition = 180;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Cash.AllowPasteExcel = true;
		this.grd_Cash.ClipboardData = null;
		this.grd_Cash.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Cash.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Cash.Location = new System.Drawing.Point(0, 0);
		this.grd_Cash.MainView = this.gv_Cash;
		this.grd_Cash.Name = "grd_Cash";
		this.grd_Cash.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.luk_Type, this.luk_Group, this.luk_Currency, this.luk_Default, this.luk_Curr, this.txt_code });
		this.grd_Cash.Size = new System.Drawing.Size(729, 180);
		this.grd_Cash.TabIndex = 0;
		this.grd_Cash.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Cash });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addRowToolStripMenuItem, this.deleteRowToolStripMenuItem, this.defaultToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 70);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.addRowToolStripMenuItem.Text = "Add";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.deleteRowToolStripMenuItem.Text = "Remove";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.defaultToolStripMenuItem.Name = "defaultToolStripMenuItem";
		this.defaultToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.defaultToolStripMenuItem.Text = "Default";
		this.defaultToolStripMenuItem.Click += new System.EventHandler(this.defaultToolStripMenuItem_Click);
		this.gv_Cash.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Cash.ColumnKey = null;
		this.gv_Cash.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7 });
		this.gv_Cash.DisableFormSetting = false;
		this.gv_Cash.DisplayException = false;
		this.gv_Cash.EditColumn = null;
		this.gv_Cash.FieldCurrency = null;
		this.gv_Cash.FieldRate = null;
		this.gv_Cash.FormId = "oCashItemMasterData";
		this.gv_Cash.GridControl = this.grd_Cash;
		this.gv_Cash.IgnoreFilter = false;
		this.gv_Cash.IndexEditRow = -1;
		this.gv_Cash.IndicatorWidth = 35;
		this.gv_Cash.IsApzTable = false;
		this.gv_Cash.IsInsertNewRow = false;
		this.gv_Cash.IsLoadingData = false;
		this.gv_Cash.IsMappingContextAction = false;
		this.gv_Cash.ItemId = "OCASH";
		this.gv_Cash.LstColumnAllowBatch = null;
		this.gv_Cash.LstColumnNotAllowBatch = null;
		this.gv_Cash.LstColumnNotEditable = null;
		this.gv_Cash.LstColumNotSetting = null;
		this.gv_Cash.Name = "gv_Cash";
		this.gv_Cash.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Cash.OptionsView.ColumnAutoWidth = false;
		this.gv_Cash.OptionsView.ShowGroupPanel = false;
		this.gv_Cash.UseBatchMod = false;
		this.gv_Cash.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Cash_CustomDrawRowIndicator);
		this.gv_Cash.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Cash_RowCellStyle);
		this.gv_Cash.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Cash_ShowingEditor);
		this.gv_Cash.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Cash_FocusedRowObjectChanged);
		this.gv_Cash.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Cash_CellValueChanged);
		this.gv_Cash.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Cash_CellValueChanging);
		this.gridColumnEx1.Caption = "Cash Code";
		this.gridColumnEx1.ColumnEdit = this.txt_code;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "CashCode";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.txt_code.AutoHeight = false;
		this.txt_code.Name = "txt_code";
		this.txt_code.ReadOnly = true;
		this.gridColumnEx2.Caption = "Cash Name";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "CashName";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx3.Caption = "Type";
		this.gridColumnEx3.ColumnEdit = this.luk_Type;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "Type";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 2;
		this.luk_Type.AutoHeight = false;
		this.luk_Type.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Type.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_Type.DisplayMember = "Name";
		this.luk_Type.DropDownRows = 3;
		this.luk_Type.IsSearchControl = false;
		this.luk_Type.Name = "luk_Type";
		this.luk_Type.NullText = "";
		this.luk_Type.ShowFooter = false;
		this.luk_Type.ShowHeader = false;
		this.luk_Type.ValueMember = "Code";
		this.gridColumnEx4.Caption = "Group";
		this.gridColumnEx4.ColumnEdit = this.luk_Group;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "Group";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 3;
		this.luk_Group.AutoHeight = false;
		this.luk_Group.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Group.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Group.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_Group.DisplayMember = "Name";
		this.luk_Group.IsSearchControl = false;
		this.luk_Group.Name = "luk_Group";
		this.luk_Group.NullText = "";
		this.luk_Group.ShowFooter = false;
		this.luk_Group.ShowHeader = false;
		this.luk_Group.ValueMember = "Code";
		this.gridColumnEx5.Caption = "Currency";
		this.gridColumnEx5.ColumnEdit = this.luk_Curr;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "Currency";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 4;
		this.luk_Curr.AutoHeight = false;
		this.luk_Curr.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Curr.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrName", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrCode", "Value")
		});
		this.luk_Curr.DisplayMember = "CurrName";
		this.luk_Curr.Name = "luk_Curr";
		this.luk_Curr.NullText = "";
		this.luk_Curr.ValueMember = "CurrCode";
		this.gridColumnEx6.Caption = "Default";
		this.gridColumnEx6.ColumnEdit = this.luk_Default;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "Default";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.luk_Default.AutoHeight = false;
		this.luk_Default.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Default.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Default.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_Default.DisplayMember = "Name";
		this.luk_Default.DropDownRows = 4;
		this.luk_Default.IsSearchControl = false;
		this.luk_Default.Name = "luk_Default";
		this.luk_Default.NullText = "";
		this.luk_Default.ShowFooter = false;
		this.luk_Default.ShowHeader = false;
		this.luk_Default.ValueMember = "Code";
		this.gridColumnEx7.Caption = "Remark";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "Remark";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 5;
		this.luk_Currency.AutoHeight = false;
		this.luk_Currency.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_Currency.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Currency.CheckFields = null;
		this.luk_Currency.CheckTables = null;
		this.luk_Currency.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrName", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrCode", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_Currency.DisplayMember = "CurrName";
		this.luk_Currency.Name = "luk_Currency";
		this.luk_Currency.NullText = "";
		this.luk_Currency.ShowFooter = false;
		this.luk_Currency.ShowHeader = false;
		this.luk_Currency.ValueMember = "CurrCode";
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControl1.Size = new System.Drawing.Size(729, 177);
		this.xtraTabControl1.TabIndex = 0;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.xtraTabPage1 });
		this.xtraTabPage1.Controls.Add(this.grd_Detail);
		this.xtraTabPage1.EditControl = null;
		this.xtraTabPage1.EditValue = "Detail";
		this.xtraTabPage1.FieldName = "";
		this.xtraTabPage1.IsSearchControl = false;
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.ReadOnly = false;
		this.xtraTabPage1.SettingInVisible = false;
		this.xtraTabPage1.SettingReadonly = false;
		this.xtraTabPage1.Size = new System.Drawing.Size(727, 152);
		this.xtraTabPage1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage1.Text = "Detail";
		this.grd_Detail.AllowPasteExcel = true;
		this.grd_Detail.ClipboardData = null;
		this.grd_Detail.ContextMenuStrip = this.contextMenuStrip2;
		this.grd_Detail.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Detail.Location = new System.Drawing.Point(0, 0);
		this.grd_Detail.MainView = this.gv_Detail;
		this.grd_Detail.Name = "grd_Detail";
		this.grd_Detail.Size = new System.Drawing.Size(727, 152);
		this.grd_Detail.TabIndex = 0;
		this.grd_Detail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Detail });
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addRowToolStripMenuItem1, this.deleteRowToolStripMenuItem1 });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(134, 48);
		this.addRowToolStripMenuItem1.Name = "addRowToolStripMenuItem1";
		this.addRowToolStripMenuItem1.Size = new System.Drawing.Size(133, 22);
		this.addRowToolStripMenuItem1.Text = "Add Row";
		this.addRowToolStripMenuItem1.Click += new System.EventHandler(this.addRowToolStripMenuItem1_Click);
		this.deleteRowToolStripMenuItem1.Name = "deleteRowToolStripMenuItem1";
		this.deleteRowToolStripMenuItem1.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem1.Text = "Delete Row";
		this.deleteRowToolStripMenuItem1.Click += new System.EventHandler(this.deleteRowToolStripMenuItem1_Click);
		this.gv_Detail.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Detail.ColumnKey = null;
		this.gv_Detail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gridColumnEx8, this.gridColumnEx9, this.gridColumnEx10, this.gridColumnEx11, this.gridColumnEx12, this.gridColumnEx13, this.gridColumnEx14 });
		this.gv_Detail.DisableFormSetting = false;
		this.gv_Detail.DisplayException = false;
		this.gv_Detail.EditColumn = null;
		this.gv_Detail.FieldCurrency = null;
		this.gv_Detail.FieldRate = null;
		this.gv_Detail.FormId = "oCashItemMasterData";
		this.gv_Detail.GridControl = this.grd_Detail;
		this.gv_Detail.IgnoreFilter = false;
		this.gv_Detail.IndexEditRow = -1;
		this.gv_Detail.IndicatorWidth = 35;
		this.gv_Detail.IsApzTable = false;
		this.gv_Detail.IsInsertNewRow = false;
		this.gv_Detail.IsLoadingData = false;
		this.gv_Detail.IsMappingContextAction = false;
		this.gv_Detail.ItemId = "ODETAIL";
		this.gv_Detail.LstColumnAllowBatch = null;
		this.gv_Detail.LstColumnNotAllowBatch = null;
		this.gv_Detail.LstColumnNotEditable = null;
		this.gv_Detail.LstColumNotSetting = null;
		this.gv_Detail.Name = "gv_Detail";
		this.gv_Detail.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Detail.OptionsView.ShowGroupPanel = false;
		this.gv_Detail.UseBatchMod = false;
		this.gv_Detail.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Detail_CustomDrawRowIndicator);
		this.gv_Detail.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Detail_CellValueChanging);
		this.gridColumnEx8.Caption = "gridColumnEx8";
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "CashCode";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = true;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx9.Caption = "gridColumnEx9";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "LineNum";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx10.Caption = "In Number";
		this.gridColumnEx10.ColumnEdit = this.repositoryItemTextEditExPrice1;
		this.gridColumnEx10.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = "Currency";
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx10.FieldName = "InNumber";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 0;
		this.gridColumnEx11.Caption = "In Work";
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "InWork";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Visible = true;
		this.gridColumnEx11.VisibleIndex = 1;
		this.gridColumnEx12.Caption = "Remark";
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "Remark";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx12.Visible = true;
		this.gridColumnEx12.VisibleIndex = 2;
		this.gridColumnEx13.Caption = "Note";
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.FieldName = "Note";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.UnitField = null;
		this.gridColumnEx13.Visible = true;
		this.gridColumnEx13.VisibleIndex = 3;
		this.gridColumnEx14.Caption = "Currency";
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.FieldName = "Currency";
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.UnitField = null;
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "OK";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(1, 376);
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
		this.btn_Cancel.Location = new System.Drawing.Point(80, 376);
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
		base.ClientSize = new System.Drawing.Size(729, 405);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oCashItemMasterData";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_CashItemMasterData";
		this.Text = "Cash Item Master Data";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_CashItemMasterData_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Cash).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Cash).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_code).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Group).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Curr).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Default).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Currency).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Detail).EndInit();
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Detail).EndInit();
		base.ResumeLayout(false);
	}
}
