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
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.HumanResources.Salary;

public class frm_DeductionType : XtraFormEx
{
	private int _ID = -1;

	private DataTable _dtdoc;

	private DataTable _dtLine;

	private bool _isLoading;

	private List<CurrencyCodesImpl> salaryCurrData;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_De;

	private GridViewEx gv_De;

	private GridColumnEx De_Code;

	private GridColumnEx De_Name;

	private GridColumnEx De_Remarks;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx xtraTabPage1;

	private GridControlEx grd_Detail;

	private GridViewEx gv_Detail;

	private GridColumnEx detail_Code;

	private GridColumnEx detail_LineNum;

	private GridColumnEx ValidFrom;

	private RepositoryItemDateEditEx rep_dateFrom;

	private GridColumnEx ValidTo;

	private RepositoryItemDateEditEx rep_dateTo;

	private GridColumnEx CompRat;

	private RepositoryItemCalcEdit rep_caComp;

	private GridColumnEx EmpRat;

	private RepositoryItemCalcEdit rep_caEmp;

	private GridColumnEx LimitedAmt;

	private GridColumnEx Apply;

	private RepositoryItemCheckEdit rep_checkApply;

	private SimpleButtonEx btn_Ok;

	private SimpleButtonEx btn_Cancel;

	private GridSplitContainer gridSplitContainer1;

	private ContextMenuStrip contextMenuStrip1;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private ToolStripMenuItem duplicateToolStripMenuItem;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private RepositoryItemCalcEdit rep_caLi;

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
			btn_Ok.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_Ok.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.Find:
			btn_Ok.Text = Resources.ButtonFind;
			break;
		case Enums.FormViewModeType.View:
			btn_Ok.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_DeductionType()
	{
		InitializeComponent();
		FormId = "oDeductionType";
		gv_De.FormId = FormId;
		gv_Detail.FormId = FormId;
		gv_De.ItemId = "APZ_OHDT";
		gv_Detail.ItemId = "APZ_HDT1";
		gv_De.ChangeLogType = Enums.ChangeLog.clDeductionType;
		gv_De.IsApzTable = true;
		gv_De.ShowingEditor += GvDeOnShowingEditor;
		gv_Detail.ShowingEditor += GvDetailOnShowingEditor;
		OnLoadAsync += frm_DeductionType_Load;
	}

	private void GvDetailOnShowingEditor(object sender, CancelEventArgs e)
	{
		if (gv_De.FocusedColumn.FieldName == "Code" || gv_De.FocusedColumn.FieldName == "LineNum")
		{
			e.Cancel = true;
		}
	}

	private void GvDeOnShowingEditor(object sender, CancelEventArgs e)
	{
		if (gv_De.FocusedColumn.FieldName == "Code")
		{
			e.Cancel = true;
		}
	}

	private void gv_De_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void xtraTabControl1_Click(object sender, EventArgs e)
	{
	}

	private void gridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private async Task frm_DeductionType_Load(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = await GetList();
			grd_De.DataSource = dataTable;
			if (dataTable.IsNotNull())
			{
				gv_De_FocusedRowObjectChanged(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_De_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			await GetDetails(Function.ParseInt(gv_De.GetFocusedRowCellValue("Code")));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
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
				await Saves();
				break;
			case Enums.FormViewModeType.Update:
				await Update();
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
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
		Close();
	}

	private void gv_Detail_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
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

	private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (gv_De.GetSelectedRows() == null || gv_De.GetSelectedRows().Count() == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteSelectedRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				int[] selectedRows = gv_De.GetSelectedRows();
				string text = "";
				int[] array = selectedRows;
				foreach (int rowHandle in array)
				{
					text = text + gv_De.GetRowCellValue(rowHandle, "Code").ToString() + ",";
				}
				if (text.Length > 0)
				{
					text = text.Remove(text.Length - 1);
					string text2 = await _httpClient.DeductionType.DeleteDocs(text);
					if (string.IsNullOrEmpty(text2))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						GridControlEx gridControlEx = grd_De;
						gridControlEx.DataSource = await GetList();
						grd_Detail.DataSource = null;
						gv_Detail.RefreshData();
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int ID = Function.ParseInt(gv_De.GetRowCellValue(gv_De.FocusedRowHandle, "Code"));
			DataTable dataSourceTable = grd_De.DataSourceTable;
			DataRow[] dr = (from t in dataSourceTable.AsEnumerable()
				where Function.ParseInt(t["Code"]) == ID
				select t).ToArray();
			Function.ParseInt(await GetMaxCode());
			if (dr != null && dr.Length != 0)
			{
				DataTable dataSource = dr.CopyToDataTable();
				grd_De.DataSource = null;
				grd_De.DataSource = dataSource;
				_dtLine = grd_Detail.DataSourceTable.Copy();
				foreach (DataRow item in _dtLine.AsEnumerable())
				{
					item["Code"] = 0;
					item["LineNum"] = 0;
				}
				grd_Detail.DataSource = null;
				grd_Detail.DataSource = _dtLine;
			}
			FormMode = Enums.FormViewModeType.Add;
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
			_dtLine = grd_Detail.DataSourceTable;
			if (_dtLine == null && _dtLine.Columns.Count < 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			DataRow row = _dtLine.NewRow();
			_dtLine.Rows.Add(row);
			grd_Detail.DataSource = _dtLine;
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

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
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
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_De_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
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
				if (string.IsNullOrEmpty(Function.ToString(_dtLine.Rows[i]["ValidFrom"])))
				{
					flag = true;
				}
			}
			if (!flag)
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
			WinCommons.ErrorLineNumber(ex);
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
			WinCommons.ErrorLineNumber(ex);
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
			WinCommons.ErrorLineNumber(ex);
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
			WinCommons.ErrorLineNumber(ex);
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
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH()
	{
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			grd_De.DataSourceTable.Rows.Clear();
			grd_Detail.DataSourceTable.Rows.Clear();
			_dtdoc = grd_De.DataSourceTable.Clone();
			if (_dtdoc == null && _dtdoc.Columns.Count < 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			DataRow row = _dtdoc.NewRow();
			_dtdoc.Rows.Add(row);
			grd_De.DataSource = _dtdoc;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void AddNewDetail()
	{
		try
		{
			gv_Detail.BeginInsertRow();
			_dtLine = grd_Detail.DataSourceTable;
			if (_dtLine == null && _dtLine.Columns.Count < 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			DataRow row = _dtLine.NewRow();
			_dtLine.Rows.Add(row);
			grd_Detail.DataSource = _dtLine;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gv_Detail.EndInsertRow();
		}
	}

	public async Task REFRESH_DATA()
	{
		_ = 1;
		try
		{
			DataTable dataTable = await GetList();
			if (dataTable.IsNotNull())
			{
				grd_De.DataSource = dataTable;
				grd_De.RefreshDataSource();
				int code = Function.ParseInt(gv_De.GetFocusedRowCellValue("Code"));
				await GetDetails(code);
			}
			else
			{
				grd_De.DataSource = null;
				grd_De.RefreshDataSource();
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task<int> FindDataNextLast(Enums.NavigateType type)
	{
		_ = 1;
		try
		{
			DataTable dataTable = await _httpClient.DeductionType.FindDataNextLast(Function.ParseInt(gv_De.GetFocusedRowCellValue("Code")), (int)type);
			if (dataTable.IsNotNull())
			{
				grd_De.DataSource = dataTable;
				int code = Function.ParseInt(dataTable.Rows[0]["Code"]);
				await GetDetails(code);
				FormMode = Enums.FormViewModeType.View;
				return 1;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	private async Task<int> Saves()
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				btn_Ok.Enabled = false;
				DataSet dataSet = new DataSet();
				_dtdoc = grd_De.DataSourceTable;
				CheckRowNull(_dtdoc, "Name");
				if (grd_Detail.DataSource != null)
				{
					_dtLine = grd_Detail.DataSourceTable;
					for (int i = 0; i < _dtLine.Rows.Count; i++)
					{
						if (Function.ToString(_dtLine.Rows[i]["ValidFrom"]) == "" && Function.ToString(_dtLine.Rows[i]["ValidTo"]) == "" && Function.ToString(_dtLine.Rows[i]["CompRat"]) == "")
						{
							_dtLine.Rows.RemoveAt(i--);
						}
					}
					_dtLine.AcceptChanges();
				}
				_dtdoc.TableName = "Doc";
				_dtLine.TableName = "Line";
				dataSet.Tables.Add(_dtdoc.Copy());
				dataSet.Tables.Add(_dtLine.Copy());
				HttpResult httpResult = await _httpClient.DeductionType.Saves(dataSet);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					GridControlEx gridControlEx = grd_De;
					gridControlEx.DataSource = await GetList();
					FormMode = Enums.FormViewModeType.View;
					btn_Ok.Enabled = true;
					return 0;
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				btn_Ok.Enabled = true;
				return 1;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
		return 1;
	}

	private new async Task<int> Update()
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				btn_Ok.Enabled = false;
				DataSet dataSet = new DataSet();
				int num = Function.ParseInt(gv_De.GetFocusedRowCellValue("Code"));
				_dtdoc = grd_De.DataSourceTable;
				DataRow[] array = _dtdoc.Select("Code=" + num);
				if (array.Length != 0)
				{
					array[0]["UserSign2"] = GlobalData.UserID;
					array[0]["UpdateDate"] = DateTime.Today;
					_dtdoc = array.CopyToDataTable();
					CheckRowNull(_dtdoc, "Name");
				}
				if (grd_Detail.DataSource != null)
				{
					_dtLine = grd_Detail.DataSourceTable;
					for (int i = 0; i < _dtLine.Rows.Count; i++)
					{
						if (Function.ToString(_dtLine.Rows[i]["ValidFrom"]) == "" && Function.ToString(_dtLine.Rows[i]["ValidTo"]) == "" && Function.ToString(_dtLine.Rows[i]["CompRat"]) == "")
						{
							_dtLine.Rows.RemoveAt(i--);
						}
					}
					_dtLine.AcceptChanges();
				}
				_dtdoc.TableName = "Doc";
				_dtLine.TableName = "Line";
				dataSet.Tables.Add(_dtdoc.Copy());
				dataSet.Tables.Add(_dtLine.Copy());
				HttpResult httpResult = await _httpClient.DeductionType.Update(dataSet);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.UpdateSuccess, "");
					grd_De.DatasourceTable().AcceptChanges();
					grd_Detail.DatasourceTable().AcceptChanges();
					FormMode = Enums.FormViewModeType.View;
					btn_Ok.Enabled = true;
					return 0;
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				btn_Ok.Enabled = true;
				return 1;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
		return 1;
	}

	public async Task GetDetails(int code)
	{
		try
		{
			_isLoading = true;
			WinCommons._OpenProcessing();
			this.ResetData();
			DataTable dataSource = await _httpClient.DeductionType.GetDetail(code);
			grd_Detail.DataSource = dataSource;
			AddNewDetail();
			WinCommons._CloseProcessing();
			_isLoading = false;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task<DataTable> GetList()
	{
		try
		{
			return await _httpClient.DeductionType.GetList();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private void CheckRowNull(DataTable dt, string key)
	{
		try
		{
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				if (Function.ToString(dt.Rows[i][key]) == "")
				{
					dt.Rows.RemoveAt(i--);
				}
			}
			dt.AcceptChanges();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> GetByDate(DateTime fromDate, DateTime toDate)
	{
		try
		{
			if ((await _httpClient.DeductionType.GetByDate(fromDate, toDate)).Rows.Count > 0)
			{
				return 1;
			}
			return 0;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	public async Task<int> GetMaxCode()
	{
		try
		{
			return await _httpClient.DeductionType.GetMaxCode();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	private void frm_DeductionType_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
		{
			e.Cancel = true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.Salary.frm_DeductionType));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.gridSplitContainer1 = new DevExpress.XtraGrid.GridSplitContainer();
		this.grd_De = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_De = new Apzon.Controls.Grid.GridViewEx();
		this.De_Code = new Apzon.Controls.Grid.GridColumnEx();
		this.De_Name = new Apzon.Controls.Grid.GridColumnEx();
		this.De_Remarks = new Apzon.Controls.Grid.GridColumnEx();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Detail = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Detail = new Apzon.Controls.Grid.GridViewEx();
		this.detail_Code = new Apzon.Controls.Grid.GridColumnEx();
		this.detail_LineNum = new Apzon.Controls.Grid.GridColumnEx();
		this.ValidFrom = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_dateFrom = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.ValidTo = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_dateTo = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.CompRat = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_caComp = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.EmpRat = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_caEmp = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.LimitedAmt = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_caLi = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.Apply = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_checkApply = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridSplitContainer1).BeginInit();
		this.gridSplitContainer1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_De).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_De).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Detail).BeginInit();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Detail).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateFrom).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateFrom.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateTo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateTo.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_caComp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_caEmp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_caLi).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_checkApply).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 1);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.gridSplitContainer1);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControl1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(810, 390);
		this.splitContainerControl1.SplitterPosition = 153;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.gridSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.gridSplitContainer1.Grid = this.grd_De;
		this.gridSplitContainer1.Location = new System.Drawing.Point(0, 0);
		this.gridSplitContainer1.Name = "gridSplitContainer1";
		this.gridSplitContainer1.Panel1.Controls.Add(this.grd_De);
		this.gridSplitContainer1.Size = new System.Drawing.Size(810, 153);
		this.gridSplitContainer1.TabIndex = 0;
		this.grd_De.AllowPasteExcel = false;
		this.grd_De.ClipboardData = null;
		this.grd_De.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_De.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_De.Location = new System.Drawing.Point(0, 0);
		this.grd_De.MainView = this.gv_De;
		this.grd_De.Name = "grd_De";
		this.grd_De.Size = new System.Drawing.Size(810, 153);
		this.grd_De.TabIndex = 0;
		this.grd_De.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_De });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addNewToolStripMenuItem, this.deleteToolStripMenuItem, this.duplicateToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(125, 70);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
		this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.duplicateToolStripMenuItem.Text = "Duplicate";
		this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
		this.gv_De.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_De.ColumnKey = null;
		this.gv_De.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.De_Code, this.De_Name, this.De_Remarks });
		this.gv_De.DisableFormSetting = false;
		this.gv_De.DisplayException = false;
		this.gv_De.EditColumn = null;
		this.gv_De.FieldCurrency = null;
		this.gv_De.FieldRate = null;
		this.gv_De.FormId = "700023";
		this.gv_De.GridControl = this.grd_De;
		this.gv_De.IgnoreFilter = false;
		this.gv_De.IndexEditRow = -1;
		this.gv_De.IndicatorWidth = 30;
		this.gv_De.IsApzTable = false;
		this.gv_De.IsInsertNewRow = false;
		this.gv_De.IsLoadingData = false;
		this.gv_De.IsMappingContextAction = false;
		this.gv_De.ItemId = "APZ_OHDT";
		this.gv_De.LstColumnAllowBatch = null;
		this.gv_De.LstColumnNotAllowBatch = null;
		this.gv_De.LstColumnNotEditable = null;
		this.gv_De.LstColumNotSetting = null;
		this.gv_De.Name = "gv_De";
		this.gv_De.OptionsView.ShowGroupPanel = false;
		this.gv_De.UseBatchMod = false;
		this.gv_De.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_De_CustomDrawRowIndicator);
		this.gv_De.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_De_FocusedRowObjectChanged);
		this.gv_De.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_De_CellValueChanging);
		this.De_Code.Caption = "Code";
		this.De_Code.CurrencyDateField = null;
		this.De_Code.CurrencyField = null;
		this.De_Code.CurrencyFixed = null;
		this.De_Code.CurrencyFixedRate = 0.0;
		this.De_Code.CurrencyRateField = null;
		this.De_Code.FieldName = "Code";
		this.De_Code.Name = "De_Code";
		this.De_Code.PrimaryKey = false;
		this.De_Code.UnitField = null;
		this.De_Name.Caption = "Name";
		this.De_Name.CurrencyDateField = null;
		this.De_Name.CurrencyField = null;
		this.De_Name.CurrencyFixed = null;
		this.De_Name.CurrencyFixedRate = 0.0;
		this.De_Name.CurrencyRateField = null;
		this.De_Name.FieldName = "Name";
		this.De_Name.Name = "De_Name";
		this.De_Name.PrimaryKey = false;
		this.De_Name.UnitField = null;
		this.De_Name.Visible = true;
		this.De_Name.VisibleIndex = 0;
		this.De_Remarks.Caption = "Remarks";
		this.De_Remarks.CurrencyDateField = null;
		this.De_Remarks.CurrencyField = null;
		this.De_Remarks.CurrencyFixed = null;
		this.De_Remarks.CurrencyFixedRate = 0.0;
		this.De_Remarks.CurrencyRateField = null;
		this.De_Remarks.FieldName = "Remarks";
		this.De_Remarks.Name = "De_Remarks";
		this.De_Remarks.PrimaryKey = false;
		this.De_Remarks.UnitField = null;
		this.De_Remarks.Visible = true;
		this.De_Remarks.VisibleIndex = 1;
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControl1.Size = new System.Drawing.Size(810, 227);
		this.xtraTabControl1.TabIndex = 0;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.xtraTabPage1 });
		this.xtraTabControl1.Click += new System.EventHandler(this.xtraTabControl1_Click);
		this.xtraTabPage1.Controls.Add(this.grd_Detail);
		this.xtraTabPage1.EditControl = null;
		this.xtraTabPage1.EditValue = "Detail";
		this.xtraTabPage1.FieldName = "xtraTabPage1";
		this.xtraTabPage1.IsSearchControl = false;
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.ReadOnly = false;
		this.xtraTabPage1.SettingInVisible = false;
		this.xtraTabPage1.SettingReadonly = false;
		this.xtraTabPage1.Size = new System.Drawing.Size(808, 202);
		this.xtraTabPage1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage1.Text = "Detail";
		this.grd_Detail.AllowPasteExcel = true;
		this.grd_Detail.ClipboardData = null;
		this.grd_Detail.ContextMenuStrip = this.contextMenuStrip2;
		this.grd_Detail.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Detail.Location = new System.Drawing.Point(0, 0);
		this.grd_Detail.MainView = this.gv_Detail;
		this.grd_Detail.Name = "grd_Detail";
		this.grd_Detail.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.rep_checkApply, this.rep_dateFrom, this.rep_dateTo, this.rep_caComp, this.rep_caEmp, this.rep_caLi });
		this.grd_Detail.Size = new System.Drawing.Size(808, 202);
		this.grd_Detail.TabIndex = 0;
		this.grd_Detail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Detail });
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addRowToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.contextMenuStrip2.Name = "contextMenuStrip1";
		this.contextMenuStrip2.Size = new System.Drawing.Size(134, 48);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.addRowToolStripMenuItem.Text = "Add Row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gv_Detail.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Detail.ColumnKey = null;
		this.gv_Detail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.detail_Code, this.detail_LineNum, this.ValidFrom, this.ValidTo, this.CompRat, this.EmpRat, this.LimitedAmt, this.Apply });
		this.gv_Detail.DisableFormSetting = false;
		this.gv_Detail.DisplayException = false;
		this.gv_Detail.EditColumn = null;
		this.gv_Detail.FieldCurrency = null;
		this.gv_Detail.FieldRate = null;
		this.gv_Detail.FormId = "700023";
		this.gv_Detail.GridControl = this.grd_Detail;
		this.gv_Detail.IgnoreFilter = false;
		this.gv_Detail.IndexEditRow = -1;
		this.gv_Detail.IndicatorWidth = 30;
		this.gv_Detail.IsApzTable = false;
		this.gv_Detail.IsInsertNewRow = false;
		this.gv_Detail.IsLoadingData = false;
		this.gv_Detail.IsMappingContextAction = false;
		this.gv_Detail.ItemId = "APZ_HDT1";
		this.gv_Detail.LstColumnAllowBatch = null;
		this.gv_Detail.LstColumnNotAllowBatch = null;
		this.gv_Detail.LstColumnNotEditable = null;
		this.gv_Detail.LstColumNotSetting = null;
		this.gv_Detail.Name = "gv_Detail";
		this.gv_Detail.OptionsCustomization.AllowSort = false;
		this.gv_Detail.OptionsView.ColumnAutoWidth = false;
		this.gv_Detail.OptionsView.ShowGroupPanel = false;
		this.gv_Detail.UseBatchMod = false;
		this.gv_Detail.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridView1_CustomDrawRowIndicator);
		this.gv_Detail.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Detail_CellValueChanging);
		this.gv_Detail.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_Detail_CustomColumnDisplayText);
		this.detail_Code.Caption = "Code";
		this.detail_Code.CurrencyDateField = null;
		this.detail_Code.CurrencyField = null;
		this.detail_Code.CurrencyFixed = null;
		this.detail_Code.CurrencyFixedRate = 0.0;
		this.detail_Code.CurrencyRateField = null;
		this.detail_Code.FieldName = "Code";
		this.detail_Code.Name = "detail_Code";
		this.detail_Code.PrimaryKey = false;
		this.detail_Code.UnitField = null;
		this.detail_LineNum.Caption = "LineNum";
		this.detail_LineNum.CurrencyDateField = null;
		this.detail_LineNum.CurrencyField = null;
		this.detail_LineNum.CurrencyFixed = null;
		this.detail_LineNum.CurrencyFixedRate = 0.0;
		this.detail_LineNum.CurrencyRateField = null;
		this.detail_LineNum.FieldName = "LineNum";
		this.detail_LineNum.Name = "detail_LineNum";
		this.detail_LineNum.PrimaryKey = false;
		this.detail_LineNum.UnitField = null;
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
		this.CompRat.Caption = "Company Ratio";
		this.CompRat.ColumnEdit = this.rep_caComp;
		this.CompRat.CurrencyDateField = null;
		this.CompRat.CurrencyField = null;
		this.CompRat.CurrencyFixed = null;
		this.CompRat.CurrencyFixedRate = 0.0;
		this.CompRat.CurrencyRateField = null;
		this.CompRat.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Rate;
		this.CompRat.FieldName = "CompRat";
		this.CompRat.Name = "CompRat";
		this.CompRat.PrimaryKey = false;
		this.CompRat.UnitField = null;
		this.CompRat.Visible = true;
		this.CompRat.VisibleIndex = 2;
		this.CompRat.Width = 85;
		this.rep_caComp.AutoHeight = false;
		this.rep_caComp.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_caComp.Name = "rep_caComp";
		this.EmpRat.Caption = "Employee Ratio";
		this.EmpRat.ColumnEdit = this.rep_caEmp;
		this.EmpRat.CurrencyDateField = null;
		this.EmpRat.CurrencyField = null;
		this.EmpRat.CurrencyFixed = null;
		this.EmpRat.CurrencyFixedRate = 0.0;
		this.EmpRat.CurrencyRateField = null;
		this.EmpRat.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Rate;
		this.EmpRat.FieldName = "EmpRat";
		this.EmpRat.Name = "EmpRat";
		this.EmpRat.PrimaryKey = false;
		this.EmpRat.UnitField = null;
		this.EmpRat.Visible = true;
		this.EmpRat.VisibleIndex = 3;
		this.EmpRat.Width = 90;
		this.rep_caEmp.AutoHeight = false;
		this.rep_caEmp.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_caEmp.Name = "rep_caEmp";
		this.LimitedAmt.Caption = "Limited Amount";
		this.LimitedAmt.ColumnEdit = this.rep_caLi;
		this.LimitedAmt.CurrencyDateField = null;
		this.LimitedAmt.CurrencyField = null;
		this.LimitedAmt.CurrencyFixed = null;
		this.LimitedAmt.CurrencyFixedRate = 0.0;
		this.LimitedAmt.CurrencyRateField = null;
		this.LimitedAmt.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.LimitedAmt.FieldName = "LimitedAmt";
		this.LimitedAmt.Name = "LimitedAmt";
		this.LimitedAmt.PrimaryKey = false;
		this.LimitedAmt.UnitField = null;
		this.LimitedAmt.Visible = true;
		this.LimitedAmt.VisibleIndex = 4;
		this.LimitedAmt.Width = 85;
		this.rep_caLi.AutoHeight = false;
		this.rep_caLi.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_caLi.Name = "rep_caLi";
		this.Apply.Caption = "Apply";
		this.Apply.ColumnEdit = this.rep_checkApply;
		this.Apply.CurrencyDateField = null;
		this.Apply.CurrencyField = null;
		this.Apply.CurrencyFixed = null;
		this.Apply.CurrencyFixedRate = 0.0;
		this.Apply.CurrencyRateField = null;
		this.Apply.FieldName = "Apply";
		this.Apply.Name = "Apply";
		this.Apply.PrimaryKey = false;
		this.Apply.UnitField = null;
		this.Apply.Visible = true;
		this.Apply.VisibleIndex = 5;
		this.rep_checkApply.AutoHeight = false;
		this.rep_checkApply.Name = "rep_checkApply";
		this.rep_checkApply.ValueChecked = "Y";
		this.rep_checkApply.ValueGrayed = "N";
		this.rep_checkApply.ValueUnchecked = "N";
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "OK";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(1, 396);
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
		this.btn_Cancel.Location = new System.Drawing.Point(79, 396);
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
		base.ClientSize = new System.Drawing.Size(812, 422);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "700023";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_DeductionType";
		this.Text = "Deduction Type";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_DeductionType_Load);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_DeductionType_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridSplitContainer1).EndInit();
		this.gridSplitContainer1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_De).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_De).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Detail).EndInit();
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Detail).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateFrom.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateFrom).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateTo.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateTo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_caComp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_caEmp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_caLi).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_checkApply).EndInit();
		base.ResumeLayout(false);
	}
}
