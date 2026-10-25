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
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Properties;
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

namespace Apzon.Forms.KPIs;

public class frm_SetIndicatorKPIs : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private const string OkString = "OK";

	private const string UpdateString = "Update";

	private const string AddString = "Add";

	private DataTable _dtDoc;

	private DataTable _dtLine;

	private bool _isLoading;

	private bool _isCpyWbs;

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridControlEx grd_Line;

	private GridViewEx gv_Line;

	private SimpleButtonEx btn_Ok;

	private SimpleButtonEx btn_Closed;

	private SimpleButtonEx btn_Search;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private GridColumnEx gridColumnEx4;

	private RepositoryItemButtonEditCFL btn_KPI;

	private TextEditEx txt_Name;

	private TextEditEx txt_Code;

	private LabelLine labelLine2;

	private LabelLine labelLine1;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private RepositoryItemLookUpEditSimple luk_FType;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx9;

	private RepositoryItemLookUpEditSimple luk_KType;

	private GridColumnEx gridColumnEx10;

	private RepositoryItemLookUpEdit luk_Currency;

	private GridColumnEx gridColumnEx11;

	private GridColumnEx gridColumnEx12;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit2;

	private GridColumnEx gridColumnEx13;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem removeToolStripMenuItem;

	private RepositoryItemLookUpEdit luk_FC;

	private RepositoryItemTextEdit txt_CodeGrid;

	private GridColumnEx gridColumnEx14;

	private RepositoryItemLookUpEditSimple luk_ViewType;

	private GridColumnEx gridColumnEx15;

	private RepositoryItemLookUpEditSimple luk_Type;

	private RepositoryItemButtonEditCFL btn_KPIS;

	public string Code { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
			{
				WinFuntions.DisableAllControl(Controls);
				btn_Closed.Enabled = true;
				return;
			}
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
				btn_Ok.Text = "Add";
				addNewToolStripMenuItem.Enabled = false;
				removeToolStripMenuItem.Enabled = false;
				txt_CodeGrid.ReadOnly = false;
				break;
			case Enums.FormViewModeType.Update:
				btn_Ok.Text = "Update";
				addNewToolStripMenuItem.Enabled = true;
				removeToolStripMenuItem.Enabled = true;
				txt_CodeGrid.ReadOnly = true;
				break;
			case Enums.FormViewModeType.View:
				btn_Ok.Text = "OK";
				addNewToolStripMenuItem.Enabled = true;
				removeToolStripMenuItem.Enabled = true;
				txt_CodeGrid.ReadOnly = true;
				break;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_SetIndicatorKPIs()
	{
		InitializeComponent();
		OnLoadAsync += Frm_SetIndicatorKPIs_Load;
		RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = btn_KPIS;
		repositoryItemButtonEditCFL.AfterShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL.AfterShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
		{
			try
			{
				DataTable dataTable = frm.gv_CFL.GridControl.DataSource as DataTable;
				Function.ToString(gv_Line.GetFocusedRowCellValue("Type"));
				string code = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
				if (dataTable.Rows.Count > 0 && dataTable != null)
				{
					DataRow[] source = (from s in dataTable.AsEnumerable()
						where Function.ToString(s["Code"]) != code
						select s).ToArray();
					if (source.Count() > 0)
					{
						frm.gv_CFL.GridControl.DataSource = source.CopyToDataTable();
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
				Logging.Write("ERROR", ex);
				WinCommons.ErrorLineNumber(ex);
			}
		}));
	}

	private async Task Frm_SetIndicatorKPIs_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			_isLoading = true;
			FormMode = Enums.FormViewModeType.View;
			Dictionary<string, string> dic = GlobalData.CurrencyCodes.ToDictionary((CurrencyCodesImpl item) => item.CurrCode, (CurrencyCodesImpl item) => item.CurrName);
			luk_Currency.SetDataSource(dic);
			luk_KType.SetDataSource(new Dictionary<string, string>
			{
				{ "Q", "Số lượng" },
				{ "C", "Tiền tệ" },
				{ "R", "%" }
			});
			SetDataFormulaCode();
			luk_FType.SetDataSource(new Dictionary<string, string>
			{
				{ "F", "Công thức" },
				{ "M", "Nhập tay" }
			});
			luk_ViewType.SetDataSource(new Dictionary<string, string>
			{
				{ "W", "Tuần" },
				{ "M", "Tháng" },
				{ "Q", "Quý" },
				{ "Y", "Năm" }
			});
			luk_Type.SetDataSource(new Dictionary<string, string>
			{
				{ "K", "KPI" },
				{ "S", "Bộ KPI" }
			});
			btn_KPI.ValueMember = "Code";
			btn_KPI.DisplayMember = "Code";
			btn_KPI.FindCFLBy(Enums.TypeCFL.GetListKPIs, true, "", Enums.ItemObjectType.NoItemType, null);
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = btn_KPI;
			repositoryItemButtonEditCFL.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL.HandleChooseItem, new Action<DataRow>(KPI_HandleChooseItem));
			btn_KPI.HandleChooseItemMulti = HandleChooseItemMulti;
			btn_KPI.RefreshDataSource();
			btn_KPI.TypeDrillDown = ApzObjectTypes.oKPIsMasterData;
			btn_KPIS.ValueMember = "Code";
			btn_KPIS.DisplayMember = "Code";
			btn_KPIS.FindCFLBy(Enums.TypeCFL.KPIsMaster, true, "", Enums.ItemObjectType.NoItemType, null);
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = btn_KPIS;
			repositoryItemButtonEditCFL2.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL2.HandleChooseItem, new Action<DataRow>(KPI_HandleChooseKPIS));
			btn_KPIS.HandleChooseItemMulti = HandleChooseKPISMulti;
			btn_KPIS.RefreshDataSource();
			btn_KPIS.TypeDrillDown = ApzObjectTypes.oSetIndicatorKPIs;
			await Task.WhenAll(WinFuntions.LoadUdfColumns(gv_Doc, "APZ_D_KPIS"), WinFuntions.LoadUdfColumns(gv_Line, "APZ_D_KPIS1"));
			new DataTable();
			if (!string.IsNullOrEmpty(Code))
			{
				txt_Code.EditValue = Code;
			}
			DataTable dataTable = await LoadData(Function.ToString(txt_Code.EditValue), Function.ToString(txt_Name.EditValue));
			Code = "";
			grd_Doc.DataSource = dataTable;
			grd_Doc.DatasourceTable().AcceptChanges();
			if (!dataTable.IsNotNull())
			{
				Gv_Doc_FocusedRowChanged(null, null);
			}
			gv_Doc.FocusedRowHandle = 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	public async void SetDataFormulaCode()
	{
		try
		{
			DataTable dataTable = await _httpClient.KPIsMaterDataClient.LoadDataFormulaCode();
			luk_FC.DisplayMember = "Name";
			luk_FC.ValueMember = "Code";
			luk_FC.DataSource = dataTable.Copy();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void KPI_HandleChooseItem(DataRow dr)
	{
		try
		{
			_dtLine = grd_Line.DataSourceTable();
			if (dr != null)
			{
				if (_dtLine.Select(string.Format("KPI_Code = '{0}'", Function.ToString(dr["Code"]))).Count() > 0)
				{
					WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExistsInTable, "");
					return;
				}
				int focusedRowHandle = gv_Line.FocusedRowHandle;
				gv_Line.SetRowCellValue(focusedRowHandle, "KPI_Code", Function.ToString(dr["Code"]));
				gv_Line.SetRowCellValue(focusedRowHandle, "Name", Function.ToString(dr["Name"]));
				gv_Line.SetRowCellValue(focusedRowHandle, "DisplayCode", Function.ToString(dr["DisplayCode"]));
				gv_Line.SetRowCellValue(focusedRowHandle, "FormulaType", Function.ToString(dr["FormulaType"]));
				gv_Line.SetRowCellValue(focusedRowHandle, "FormulaCode", Function.ToString(dr["FormulaCodeDisplay"]));
				gv_Line.SetRowCellValue(focusedRowHandle, "KpiType", Function.ToString(dr["KpiType"]));
				gv_Line.SetRowCellValue(focusedRowHandle, "Currency", Function.ToString(dr["Currency"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void HandleChooseItemMulti(List<DataRow> rows)
	{
		try
		{
			_dtLine = grd_Line.DataSourceTable();
			if (_dtLine == null || _dtLine.Rows.Count <= 0 || rows.Count <= 0)
			{
				return;
			}
			DataRow[] source = _dtLine.Select(string.Format("KPI_Code = '{0}'", Function.ToString(rows[0]["Code"])));
			int focusedRowHandle = gv_Line.FocusedRowHandle;
			if (source.Count() == 0)
			{
				gv_Line.SetRowCellValue(focusedRowHandle, "KPI_Code", Function.ToString(rows[0]["Code"]));
				gv_Line.SetRowCellValue(focusedRowHandle, "Name", Function.ToString(rows[0]["Name"]));
				gv_Line.SetRowCellValue(focusedRowHandle, "DisplayCode", Function.ToString(rows[0]["DisplayCode"]));
				gv_Line.SetRowCellValue(focusedRowHandle, "FormulaType", Function.ToString(rows[0]["FormulaType"]));
				gv_Line.SetRowCellValue(focusedRowHandle, "FormulaCode", Function.ToString(rows[0]["FormulaCodeDisplay"]));
				gv_Line.SetRowCellValue(focusedRowHandle, "KpiType", Function.ToString(rows[0]["KpiType"]));
				gv_Line.SetRowCellValue(focusedRowHandle, "Currency", Function.ToString(rows[0]["Currency"]));
				gv_Line.SetRowCellValue(focusedRowHandle, "Type", "K");
			}
			for (int i = 1; i < rows.Count; i++)
			{
				if (_dtLine.Select(string.Format("KPI_Code = '{0}'", Function.ToString(rows[i]["Code"]))).Count() > 0)
				{
					continue;
				}
				DataRow dataRow = _dtLine.AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["KPI_Code"])));
				if (dataRow != null)
				{
					int dataSourceIndex = _dtLine.Rows.IndexOf(dataRow);
					gv_Line.SetRowCellValue(gv_Line.GetRowHandle(dataSourceIndex), "KPI_Code", Function.ToString(rows[i]["Code"]));
					gv_Line.SetRowCellValue(gv_Line.GetRowHandle(dataSourceIndex), "Name", Function.ToString(rows[i]["Name"]));
					gv_Line.SetRowCellValue(gv_Line.GetRowHandle(dataSourceIndex), "DisplayCode", Function.ToString(rows[i]["DisplayCode"]));
					gv_Line.SetRowCellValue(gv_Line.GetRowHandle(dataSourceIndex), "FormulaType", Function.ToString(rows[i]["FormulaType"]));
					gv_Line.SetRowCellValue(gv_Line.GetRowHandle(dataSourceIndex), "FormulaCode", Function.ToString(rows[i]["FormulaCodeDisplay"]));
					gv_Line.SetRowCellValue(gv_Line.GetRowHandle(dataSourceIndex), "KpiType", Function.ToString(rows[i]["KpiType"]));
					gv_Line.SetRowCellValue(gv_Line.GetRowHandle(dataSourceIndex), "Currency", Function.ToString(rows[i]["Currency"]));
					gv_Line.SetRowCellValue(focusedRowHandle, "Type", "K");
					continue;
				}
				NewLine();
				DataRow row = _dtLine.AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["KPI_Code"])));
				int dataSourceIndex2 = _dtLine.Rows.IndexOf(row);
				gv_Line.SetRowCellValue(gv_Line.GetRowHandle(dataSourceIndex2), "KPI_Code", Function.ToString(rows[i]["Code"]));
				gv_Line.SetRowCellValue(gv_Line.GetRowHandle(dataSourceIndex2), "Name", Function.ToString(rows[i]["Name"]));
				gv_Line.SetRowCellValue(gv_Line.GetRowHandle(dataSourceIndex2), "DisplayCode", Function.ToString(rows[i]["DisplayCode"]));
				gv_Line.SetRowCellValue(gv_Line.GetRowHandle(dataSourceIndex2), "FormulaType", Function.ToString(rows[i]["FormulaType"]));
				gv_Line.SetRowCellValue(gv_Line.GetRowHandle(dataSourceIndex2), "FormulaCode", Function.ToString(rows[i]["FormulaCodeDisplay"]));
				gv_Line.SetRowCellValue(gv_Line.GetRowHandle(dataSourceIndex2), "KpiType", Function.ToString(rows[i]["KpiType"]));
				gv_Line.SetRowCellValue(gv_Line.GetRowHandle(dataSourceIndex2), "Currency", Function.ToString(rows[i]["Currency"]));
				gv_Line.SetRowCellValue(focusedRowHandle, "Type", "K");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void KPI_HandleChooseKPIS(DataRow dr)
	{
		try
		{
			_dtLine = grd_Line.DataSourceTable();
			if (dr != null)
			{
				string text = Function.ToString(Function.ToString(dr["Code"]));
				string value = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
				int focusedRowHandle = gv_Line.FocusedRowHandle;
				if (text.Equals(value))
				{
					gv_Line.SetRowCellValue(focusedRowHandle, "KPI_Code", "");
					return;
				}
				if (_dtLine.Select(string.Format("KPI_Code = '{0}'", Function.ToString(dr["Code"]))).Count() > 0)
				{
					WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExistsInTable, "");
					return;
				}
				gv_Line.SetRowCellValue(focusedRowHandle, "KPI_Code", Function.ToString(dr["Code"]));
				gv_Line.SetRowCellValue(focusedRowHandle, "Name", Function.ToString(dr["Name"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void HandleChooseKPISMulti(List<DataRow> rows)
	{
		try
		{
			_dtLine = grd_Line.DataSourceTable();
			if (_dtLine == null || _dtLine.Rows.Count <= 0 || rows.Count <= 0)
			{
				return;
			}
			DataRow[] source = _dtLine.Select(string.Format("KPI_Code = '{0}'", Function.ToString(rows[0]["Code"])));
			int focusedRowHandle = gv_Line.FocusedRowHandle;
			if (source.Count() == 0)
			{
				gv_Line.SetRowCellValue(focusedRowHandle, "KPI_Code", Function.ToString(rows[0]["Code"]));
				gv_Line.SetRowCellValue(focusedRowHandle, "Name", Function.ToString(rows[0]["Name"]));
				gv_Line.SetRowCellValue(focusedRowHandle, "Type", "S");
			}
			for (int i = 1; i < rows.Count; i++)
			{
				if (_dtLine.Select(string.Format("KPI_Code = '{0}'", Function.ToString(rows[i]["Code"]))).Count() > 0)
				{
					continue;
				}
				DataRow dataRow = _dtLine.AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["KPI_Code"])));
				if (dataRow != null)
				{
					int dataSourceIndex = _dtLine.Rows.IndexOf(dataRow);
					gv_Line.SetRowCellValue(gv_Line.GetRowHandle(dataSourceIndex), "KPI_Code", Function.ToString(rows[i]["Code"]));
					gv_Line.SetRowCellValue(gv_Line.GetRowHandle(dataSourceIndex), "Name", Function.ToString(rows[i]["Name"]));
					gv_Line.SetRowCellValue(gv_Line.GetRowHandle(dataSourceIndex), "Type", "S");
					continue;
				}
				NewLine();
				DataRow row = _dtLine.AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["KPI_Code"])));
				int dataSourceIndex2 = _dtLine.Rows.IndexOf(row);
				gv_Line.SetRowCellValue(gv_Line.GetRowHandle(dataSourceIndex2), "KPI_Code", Function.ToString(rows[i]["Code"]));
				gv_Line.SetRowCellValue(gv_Line.GetRowHandle(dataSourceIndex2), "Name", Function.ToString(rows[i]["Name"]));
				gv_Line.SetRowCellValue(gv_Line.GetRowHandle(dataSourceIndex2), "Type", "S");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_dtLine = grd_Line.DataSourceTable;
			if (e.Column.FieldName == "Code" && _dtLine != null && _dtLine.Rows.Count > 0)
			{
				for (int i = 0; i < _dtLine.Rows.Count; i++)
				{
					_dtLine.Rows[i]["Code"] = Function.ToString(e.Value);
				}
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void Gv_Doc_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				string code = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
				await LoadLineById(code);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		NewLine();
	}

	private void DeleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Line.DataRowCount > 0)
			{
				grd_Line.BeginUpdate();
				gv_Line.DeleteSelectedRows();
				grd_Line.EndUpdate();
				grd_Line.DatasourceTable().AcceptChanges();
				DataChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void AddNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		await REFRESH();
	}

	private async void RemoveToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Doc.GetSelectedRows() == null && gv_Doc.GetSelectedRows().Count() == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
				return;
			}
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveByCode, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, text) == DialogResult.Yes)
			{
				string text2 = await _httpClient.SetIndicatorKPIsClient.DeleteDocs(text);
				if (string.IsNullOrEmpty(text2))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					Btn_Search_Click(null, null);
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void Btn_Search_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (!string.IsNullOrEmpty(Code))
			{
				txt_Code.EditValue = Code;
			}
			string code = Function.ToString(txt_Code.EditValue);
			string name = Function.ToString(txt_Name.EditValue);
			DataTable dataSource = await LoadData(code, name);
			Code = "";
			if (!_isLoading && ((FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes))
			{
				FormMode = Enums.FormViewModeType.View;
				grd_Doc.DataSourceTable.Rows.Clear();
				grd_Line.DatasourceTable().Rows.Clear();
				grd_Doc.DataSource = dataSource;
				grd_Doc.DatasourceTable().AcceptChanges();
				string code2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
				await LoadLineById(code2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void Btn_Ok_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Add:
				await Save(1);
				break;
			case Enums.FormViewModeType.Update:
				await Save(2);
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Btn_Closed_Click(object sender, EventArgs e)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			Close();
		}
	}

	private async Task<int> Save(int type)
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			DataSet dataSet = new DataSet();
			if (type == 2)
			{
				DataTable dataTable = grd_Doc.DataSourceTable.Copy();
				dataTable.AcceptChanges();
				string id = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
				DataRow[] array = (from s in dataTable.AsEnumerable()
					where Function.ToString(s["Code"]) == id
					select s).ToArray();
				if (array.Length != 0)
				{
					_dtDoc = array.CopyToDataTable();
				}
			}
			else
			{
				_dtDoc = grd_Doc.DataSourceTable;
			}
			for (int num = 0; num < _dtDoc.Rows.Count; num++)
			{
				_dtDoc.Rows[num]["UpdateDate"] = DateTime.Now.Date;
				_dtDoc.Rows[num]["UpdateTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
			}
			_dtLine = grd_Line.DataSourceTable().Copy();
			if (_dtLine != null && _dtLine.Rows.Count > 0)
			{
				for (int num2 = 0; num2 < _dtLine.Rows.Count; num2++)
				{
					if (Function.ToString(_dtLine.Rows[num2]["KPI_Code"]) == "")
					{
						_dtLine.Rows.RemoveAt(num2--);
					}
				}
			}
			_dtDoc.TableName = "dtDoc";
			_dtLine.TableName = "dtLine";
			dataSet.Tables.Add(_dtDoc.Copy());
			dataSet.Tables.Add(_dtLine.Copy());
			HttpResult httpResult = ((type != 2) ? (await _httpClient.SetIndicatorKPIsClient.Add(dataSet)) : (await _httpClient.SetIndicatorKPIsClient.Update(dataSet)));
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				_isLoading = true;
				FormMode = Enums.FormViewModeType.View;
				if (type == 2)
				{
					grd_Doc.DatasourceTable().AcceptChanges();
					grd_Line.DatasourceTable().AcceptChanges();
				}
				else
				{
					Btn_Search_Click(null, null);
				}
				_isLoading = false;
				return 0;
			}
			WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			WinCommons._CloseProcessing();
			return 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
	}

	private async Task<DataTable> LoadData(string code, string name)
	{
		try
		{
			_isLoading = true;
			DataTable result = await _httpClient.SetIndicatorKPIsClient.LoadData(code, name);
			_isLoading = false;
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task LoadLineById(string code)
	{
		try
		{
			WinCommons._OpenProcessing();
			_isLoading = true;
			DataTable dataSource = await _httpClient.SetIndicatorKPIsClient.GetDataById(code);
			grd_Line.DataSource = dataSource;
			grd_Line.DatasourceTable().AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || (FormMode != Enums.FormViewModeType.Update && FormMode != Enums.FormViewModeType.Add) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.No)
			{
				FormMode = Enums.FormViewModeType.Add;
				grd_Doc.DataSourceTable.Rows.Clear();
				grd_Line.DataSourceTable().Rows.Clear();
				_dtDoc = grd_Doc.DataSourceTable;
				if (_dtDoc == null || _dtDoc.Columns.Count == 0)
				{
					WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				}
				DataRow dataRow = _dtDoc.NewRow();
				dataRow["CreateDate"] = DateTime.Now.Date;
				dataRow["CreateTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
				dataRow["UpdateDate"] = DateTime.Now.Date;
				dataRow["UpdateTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
				dataRow["Usersign"] = GlobalData.UserID;
				dataRow["Usersign2"] = GlobalData.UserID;
				_dtDoc.Rows.Add(dataRow);
				grd_Doc.DataSource = _dtDoc;
				NewLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewLine()
	{
		try
		{
			_dtLine = grd_Line.DataSourceTable();
			if (_dtLine == null || _dtLine.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				return;
			}
			DataRow dataRow = _dtLine.NewRow();
			dataRow["Code"] = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
			dataRow["Type"] = "K";
			_dtLine.Rows.Add(dataRow);
			_dtLine.AcceptChanges();
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DataChanged()
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Gv_Line_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_dtDoc = grd_Doc.DataSourceTable;
			_dtLine = grd_Line.DataSourceTable();
			if (e.Column.FieldName == "KPI_Code")
			{
				string text = Function.ToString(e.Value);
				string value = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
				if (text.Equals(value))
				{
					return;
				}
			}
			if (e.Column.FieldName == "Point" || e.Column.FieldName == "KPI_Code" || e.Column.FieldName == "Remarks")
			{
				if ((from t in _dtLine.AsEnumerable()
					where string.IsNullOrWhiteSpace(Function.ToString(t["KPI_Code"]))
					select t).ToList().Count <= 0)
				{
					NewLine();
				}
				DataChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Gv_Doc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.Update)
			{
				DialogResult dialogResult = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
				if (dialogResult == DialogResult.Yes && dialogResult == DialogResult.Yes)
				{
					grd_Doc.DataSourceTable.RejectChanges();
					FormMode = Enums.FormViewModeType.View;
				}
				if (dialogResult == DialogResult.No)
				{
					e.Allow = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Gv_Line_ShowingEditor(object sender, CancelEventArgs e)
	{
	}

	private void Gv_Line_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Name" || e.Column.FieldName == "DisplayCode" || e.Column.FieldName == "FormulaType" || e.Column.FieldName == "FormulaCode" || e.Column.FieldName == "KpiType" || e.Column.FieldName == "Currency")
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		if (FormMode != Enums.FormViewModeType.Add && e.Column.FieldName == "Code")
		{
			e.Appearance.BackColor = GridColor.GridReadOnly;
		}
	}

	private void Gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
	}

	private void Gv_Line_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "KPI_Code")
			{
				return;
			}
			string text = Function.ToString(gv_Line.GetRowCellValue(e.RowHandle, "Type"));
			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}
			if (!(text == "K"))
			{
				if (text == "S")
				{
					e.RepositoryItem = btn_KPIS;
				}
			}
			else
			{
				e.RepositoryItem = btn_KPI;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			Btn_Search_Click(null, null);
		}
		catch (Exception ex)
		{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.KPIs.frm_SetIndicatorKPIs));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_CodeGrid = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_ViewType = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_FType = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_KType = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Currency = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Type = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.btn_KPI = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.luk_FC = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btn_KPIS = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Closed = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Code = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Name = new Apzon.Controls.Editor.TextEditEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CodeGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ViewType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_FType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_KType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Currency).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_KPI).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_FC).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_KPIS).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(1, 58);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Line);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(925, 326);
		this.splitContainerControl1.SplitterPosition = 131;
		this.splitContainerControl1.TabIndex = 0;
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repositoryItemMemoExEdit1, this.txt_CodeGrid, this.luk_ViewType });
		this.grd_Doc.Size = new System.Drawing.Size(925, 131);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(124, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.AddNewToolStripMenuItem_Click);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.RemoveToolStripMenuItem_Click);
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clSetIndicatorKPIs;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx14 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oSetIndicatorKPIs";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = true;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_D_KPIS";
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
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.Gv_Doc_RowCellStyle);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.Gv_Doc_ShowingEditor);
		this.gv_Doc.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.Gv_Doc_FocusedRowChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.Gv_Doc_CellValueChanged);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.Gv_Doc_BeforeLeaveRow);
		this.gridColumnEx1.Caption = "Mã bộ KPI";
		this.gridColumnEx1.ColumnEdit = this.txt_CodeGrid;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "Code";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx1.Width = 93;
		this.txt_CodeGrid.AutoHeight = false;
		this.txt_CodeGrid.Name = "txt_CodeGrid";
		this.gridColumnEx2.Caption = "Tên bộ KPI";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "Name";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx2.Width = 184;
		this.gridColumnEx3.Caption = "Ghi chú";
		this.gridColumnEx3.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "Remarks";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 3;
		this.gridColumnEx3.Width = 369;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.gridColumnEx14.Caption = "Loại kỳ";
		this.gridColumnEx14.ColumnEdit = this.luk_ViewType;
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.FieldName = "ViewType";
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.UnitField = null;
		this.gridColumnEx14.Visible = true;
		this.gridColumnEx14.VisibleIndex = 2;
		this.gridColumnEx14.Width = 164;
		this.luk_ViewType.AutoHeight = false;
		this.luk_ViewType.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_ViewType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_ViewType.IsSearchControl = false;
		this.luk_ViewType.Name = "luk_ViewType";
		this.luk_ViewType.NullText = "";
		this.luk_ViewType.ShowFooter = false;
		this.luk_ViewType.ShowHeader = false;
		this.grd_Line.AllowPasteExcel = true;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.ContextMenuStrip = this.contextMenuStrip2;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(0, 0);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.repositoryItemMemoExEdit2, this.btn_KPI, this.luk_FType, this.luk_KType, this.luk_Currency, this.luk_FC, this.luk_Type, this.btn_KPIS });
		this.grd_Line.Size = new System.Drawing.Size(925, 185);
		this.grd_Line.TabIndex = 0;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addRowToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(134, 48);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.addRowToolStripMenuItem.Text = "Add Row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.AddRowToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.DeleteRowToolStripMenuItem_Click);
		this.gv_Line.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Line.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Line.ColumnKey = null;
		this.gv_Line.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[11]
		{
			this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7, this.gridColumnEx8, this.gridColumnEx9, this.gridColumnEx10, this.gridColumnEx11, this.gridColumnEx12, this.gridColumnEx13,
			this.gridColumnEx15
		});
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FormId = "oSetIndicatorKPIs";
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.GroupSummary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridGroupSummaryItem(DevExpress.Data.SummaryItemType.Sum, "Point", this.gridColumnEx11, "Tổng điểm : {0}")
		});
		this.gv_Line.IgnoreFilter = false;
		this.gv_Line.IndexEditRow = -1;
		this.gv_Line.IndicatorWidth = 35;
		this.gv_Line.IsApzTable = false;
		this.gv_Line.IsInsertNewRow = false;
		this.gv_Line.IsLoadingData = false;
		this.gv_Line.IsMappingContextAction = false;
		this.gv_Line.ItemId = "APZ_D_KPIS1";
		this.gv_Line.LstColumnAllowBatch = null;
		this.gv_Line.LstColumnNotAllowBatch = null;
		this.gv_Line.LstColumnNotEditable = null;
		this.gv_Line.LstColumNotSetting = null;
		this.gv_Line.Name = "gv_Line";
		this.gv_Line.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Line.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Line.OptionsPrint.AutoWidth = false;
		this.gv_Line.OptionsPrint.UsePrintStyles = false;
		this.gv_Line.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Line.OptionsView.ColumnAutoWidth = false;
		this.gv_Line.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Line.OptionsView.ShowFooter = true;
		this.gv_Line.OptionsView.ShowGroupedColumns = true;
		this.gv_Line.OptionsView.ShowGroupPanel = false;
		this.gv_Line.UseBatchMod = false;
		this.gv_Line.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.Gv_Line_RowCellStyle);
		this.gv_Line.CustomRowCellEditForEditing += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.Gv_Line_CustomRowCellEditForEditing);
		this.gv_Line.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.Gv_Line_ShowingEditor);
		this.gv_Line.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.Gv_Line_CellValueChanged);
		this.gridColumnEx4.Caption = "Mã KPI";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "KPI_Code";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = true;
		this.gridColumnEx4.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 1;
		this.gridColumnEx4.Width = 94;
		this.gridColumnEx5.Caption = "Tên KPI";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "Name";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.OptionsColumn.ReadOnly = true;
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 2;
		this.gridColumnEx5.Width = 178;
		this.gridColumnEx6.Caption = "Mã hiển thị";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "DisplayCode";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.OptionsColumn.ReadOnly = true;
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 3;
		this.gridColumnEx6.Width = 105;
		this.gridColumnEx7.Caption = "Loại công thức";
		this.gridColumnEx7.ColumnEdit = this.luk_FType;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "FormulaType";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.OptionsColumn.ReadOnly = true;
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 4;
		this.gridColumnEx7.Width = 126;
		this.luk_FType.AutoHeight = false;
		this.luk_FType.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_FType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_FType.IsSearchControl = false;
		this.luk_FType.Name = "luk_FType";
		this.luk_FType.NullText = "";
		this.luk_FType.ShowFooter = false;
		this.luk_FType.ShowHeader = false;
		this.gridColumnEx8.Caption = "Công thức";
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "FormulaCode";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.OptionsColumn.ReadOnly = true;
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 5;
		this.gridColumnEx8.Width = 173;
		this.gridColumnEx9.Caption = "Kiểu";
		this.gridColumnEx9.ColumnEdit = this.luk_KType;
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "KpiType";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.OptionsColumn.ReadOnly = true;
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 6;
		this.gridColumnEx9.Width = 107;
		this.luk_KType.AutoHeight = false;
		this.luk_KType.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_KType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_KType.IsSearchControl = false;
		this.luk_KType.Name = "luk_KType";
		this.luk_KType.NullText = "";
		this.luk_KType.ShowFooter = false;
		this.luk_KType.ShowHeader = false;
		this.gridColumnEx10.Caption = "Đồng tiền";
		this.gridColumnEx10.ColumnEdit = this.luk_Currency;
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "Currency";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.OptionsColumn.ReadOnly = true;
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 7;
		this.gridColumnEx10.Width = 109;
		this.luk_Currency.AutoHeight = false;
		this.luk_Currency.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Currency.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_Currency.DisplayMember = "Name";
		this.luk_Currency.Name = "luk_Currency";
		this.luk_Currency.NullText = "";
		this.luk_Currency.ValueMember = "Code";
		this.gridColumnEx11.Caption = "Điểm";
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Integer;
		this.gridColumnEx11.FieldName = "Point";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "Point", "Tổng điểm : {0}")
		});
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Visible = true;
		this.gridColumnEx11.VisibleIndex = 8;
		this.gridColumnEx11.Width = 107;
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
		this.gridColumnEx12.VisibleIndex = 9;
		this.gridColumnEx12.Width = 211;
		this.repositoryItemMemoExEdit2.AutoHeight = false;
		this.repositoryItemMemoExEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit2.Name = "repositoryItemMemoExEdit2";
		this.repositoryItemMemoExEdit2.ShowIcon = false;
		this.gridColumnEx13.Caption = "gridColumnEx13";
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.FieldName = "Code";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.OptionsColumn.AllowEdit = false;
		this.gridColumnEx13.OptionsColumn.ReadOnly = true;
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.UnitField = null;
		this.gridColumnEx15.Caption = "Loại";
		this.gridColumnEx15.ColumnEdit = this.luk_Type;
		this.gridColumnEx15.CurrencyDateField = null;
		this.gridColumnEx15.CurrencyField = null;
		this.gridColumnEx15.CurrencyFixed = null;
		this.gridColumnEx15.CurrencyFixedRate = 0.0;
		this.gridColumnEx15.CurrencyRateField = null;
		this.gridColumnEx15.FieldName = "Type";
		this.gridColumnEx15.Name = "gridColumnEx15";
		this.gridColumnEx15.PrimaryKey = false;
		this.gridColumnEx15.UnitField = null;
		this.gridColumnEx15.Visible = true;
		this.gridColumnEx15.VisibleIndex = 0;
		this.gridColumnEx15.Width = 112;
		this.luk_Type.AutoHeight = false;
		this.luk_Type.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Type.IsSearchControl = false;
		this.luk_Type.Name = "luk_Type";
		this.luk_Type.NullText = "";
		this.luk_Type.ShowFooter = false;
		this.luk_Type.ShowHeader = false;
		this.btn_KPI.AutoHeight = false;
		this.btn_KPI.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_KPI.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDow", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_KPI.ContainerControl = this;
		this.btn_KPI.DisplayMember = "";
		this.btn_KPI.DisplayMember2 = "";
		this.btn_KPI.LoadDataSourceWhenEnter = false;
		this.btn_KPI.Name = "btn_KPI";
		this.btn_KPI.SeparateCharacterForMulti = ';';
		this.btn_KPI.SeparateString = ", ";
		this.btn_KPI.UsingAutoCompleteBox = false;
		this.btn_KPI.UsingDataSource = true;
		this.btn_KPI.ValidateWhenNoDataSource = false;
		this.btn_KPI.ValueMember = "";
		this.luk_FC.AutoHeight = false;
		this.luk_FC.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_FC.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_FC.Name = "luk_FC";
		this.luk_FC.NullText = "";
		this.btn_KPIS.AutoHeight = false;
		this.btn_KPIS.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.btn_KPIS.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_KPIS.ContainerControl = this;
		this.btn_KPIS.DisplayMember = "";
		this.btn_KPIS.DisplayMember2 = "";
		this.btn_KPIS.LoadDataSourceWhenEnter = false;
		this.btn_KPIS.Name = "btn_KPIS";
		this.btn_KPIS.SeparateCharacterForMulti = ';';
		this.btn_KPIS.SeparateString = ", ";
		this.btn_KPIS.UsingAutoCompleteBox = false;
		this.btn_KPIS.UsingDataSource = true;
		this.btn_KPIS.ValidateWhenNoDataSource = false;
		this.btn_KPIS.ValueMember = "";
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "OK";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(11, 405);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.ReadOnly = false;
		this.btn_Ok.SettingInVisible = false;
		this.btn_Ok.SettingReadonly = false;
		this.btn_Ok.Size = new System.Drawing.Size(75, 23);
		this.btn_Ok.TabIndex = 1;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.Btn_Ok_Click);
		this.btn_Closed.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Closed.EditControl = null;
		this.btn_Closed.EditValue = "Closed";
		this.btn_Closed.FieldName = null;
		this.btn_Closed.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Closed.IsSearchControl = false;
		this.btn_Closed.Location = new System.Drawing.Point(92, 405);
		this.btn_Closed.Name = "btn_Closed";
		this.btn_Closed.ReadOnly = false;
		this.btn_Closed.SettingInVisible = false;
		this.btn_Closed.SettingReadonly = false;
		this.btn_Closed.Size = new System.Drawing.Size(75, 23);
		this.btn_Closed.TabIndex = 2;
		this.btn_Closed.Text = "Closed";
		this.btn_Closed.Click += new System.EventHandler(this.Btn_Closed_Click);
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(499, 8);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(75, 23);
		this.btn_Search.TabIndex = 3;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.Btn_Search_Click);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Mã bộ KPI";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(13, 11);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 4;
		this.labelLine1.Utext = "Mã bộ KPI";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Tên bộ KPI";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(237, 11);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(77, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 5;
		this.labelLine2.Utext = "Tên bộ KPI";
		this.txt_Code.EditControl = null;
		this.txt_Code.FieldName = null;
		this.txt_Code.IsSearchControl = false;
		this.txt_Code.Location = new System.Drawing.Point(91, 11);
		this.txt_Code.Name = "txt_Code";
		this.txt_Code.SettingInVisible = false;
		this.txt_Code.SettingReadonly = false;
		this.txt_Code.Size = new System.Drawing.Size(100, 20);
		this.txt_Code.TabIndex = 6;
		this.txt_Name.EditControl = null;
		this.txt_Name.FieldName = null;
		this.txt_Name.IsSearchControl = false;
		this.txt_Name.Location = new System.Drawing.Point(311, 11);
		this.txt_Name.Name = "txt_Name";
		this.txt_Name.SettingInVisible = false;
		this.txt_Name.SettingReadonly = false;
		this.txt_Name.Size = new System.Drawing.Size(100, 20);
		this.txt_Name.TabIndex = 7;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(938, 440);
		base.Controls.Add(this.txt_Name);
		base.Controls.Add(this.txt_Code);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.btn_Closed);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oSetIndicatorKPIs";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SetIndicatorKPIs";
		this.Text = "Bộ chỉ tiêu KPIs";
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CodeGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ViewType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_FType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_KType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Currency).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_KPI).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_FC).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_KPIS).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
