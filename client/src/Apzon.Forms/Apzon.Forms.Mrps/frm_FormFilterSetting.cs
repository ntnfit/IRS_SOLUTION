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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTab;

namespace Apzon.Forms.Mrps;

public class frm_FormFilterSetting : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isAddNew;

	private bool isLoading;

	private IContainer components;

	private GridControlEx grd_FilterSettingMrps;

	private GridViewEx gv_FilterSettingMrps;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit1;

	private GridViewEx gridViewEx1;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	private GridColumnEx cl_FilterCode;

	private GridColumnEx cl_FilterName;

	private GridColumnEx cl_Active;

	private GridColumnEx cl_SourceType;

	private GridColumnEx cl_Remarks;

	private RepositoryItemLookUpEdit lookup_Active;

	private RepositoryItemLookUpEdit lookup_SourceType;

	private RepositoryItemLookUpEdit lookup_FilterCode;

	private SplitContainer splitContainer1;

	private XtraTabControlEx xtraTab;

	private XtraTabPageEx tab_Table;

	private GridControlEx grd_Table;

	private GridViewEx gv_Table;

	private XtraTabPageEx tab_Line;

	private GridControlEx grd_Line;

	private GridViewEx gv_Line;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private ToolStripMenuItem refreshToolStripMenuItem;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private ContextMenuStrip contextMenuStrip3;

	private ToolStripMenuItem deleteRowToolStripMenuItem1;

	private RepositoryItemLookUpEdit rep_Active;

	private RepositoryItemComboBox cb_lstCol;

	public ApzObjectTypes objType { get; set; }

	public DataTable dtPgcMrps { get; set; }

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
		case Enums.FormViewModeType.View:
			btn_OK.Text = Resources.ButtonOK;
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.Add:
			btn_OK.Text = Resources.ButtonAdd;
			break;
		}
	}

	public frm_FormFilterSetting()
	{
		InitializeComponent();
		OnLoadAsync += frm_FilterSettingMrps_Load;
	}

	private async Task frm_FilterSettingMrps_Load(object sender, EventArgs e)
	{
		try
		{
			LoadDataSourceEditor();
			await LoadDataFormFilterSetting();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadDataSourceEditor()
	{
		gv_FilterSettingMrps.OptionsView.ColumnAutoWidth = true;
		Dictionary<string, string> dataSource = new Dictionary<string, string>
		{
			{ "Y", "Yes" },
			{ "N", "No" }
		};
		lookup_Active.DataSource = dataSource;
		Dictionary<string, string> dataSource2 = new Dictionary<string, string>
		{
			{ "U", "UI" },
			{ "D", "Data Source" },
			{ "T", "Table" }
		};
		lookup_SourceType.DataSource = dataSource2;
		List<string> list = new List<string>();
		foreach (DataColumn column in dtPgcMrps.Columns)
		{
			list.Add(column.ColumnName);
		}
		lookup_FilterCode.DataSource = list;
		Dictionary<string, string> dataSource3 = new Dictionary<string, string>
		{
			{ "Y", "Yes" },
			{ "N", "No" }
		};
		rep_Active.DataSource = dataSource3;
	}

	private async Task LoadDataFormFilterSetting()
	{
		_ = 1;
		try
		{
			DataTable dataSource = await _httpClient.FormFilterSettingClient.GetDataFilterSetting(objType);
			grd_FilterSettingMrps.DataSource = dataSource;
			grd_FilterSettingMrps.RefreshDataSource();
			await AddNewRow();
			FormMode = Enums.FormViewModeType.View;
			isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadTable()
	{
		_ = 1;
		try
		{
			int docEntry = Function.ParseInt(gv_FilterSettingMrps.GetFocusedRowCellValue("DocEntry"));
			DataTable dataSource = await _httpClient.FormFilterSettingClient.GetDataFilterSettingTable(docEntry);
			grd_Table.DataSource = dataSource;
			await AddRowTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadLine()
	{
		_ = 1;
		try
		{
			int docEntry = Function.ParseInt(gv_FilterSettingMrps.GetFocusedRowCellValue("DocEntry"));
			DataTable dataSource = await _httpClient.FormFilterSettingClient.GetDataFilterSettingLine(docEntry);
			grd_Line.DataSource = dataSource;
			await AddRowLine();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task AddNewRow()
	{
		try
		{
			DataTable dataTable = grd_FilterSettingMrps.DataSourceTable;
			if (!dataTable.IsNotNull())
			{
				dataTable = await _httpClient.FormFilterSettingClient.GetFilterStructure();
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["DocEntry"] = 0;
			dataRow["ObjType"] = (int)objType;
			dataRow["Active"] = "Y";
			dataRow["SourceType"] = "U";
			dataTable.Rows.Add(dataRow);
			grd_FilterSettingMrps.DataSource = dataTable;
			grd_FilterSettingMrps.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task AddRowLine()
	{
		DataTable dataTable = grd_Line.DataSourceTable;
		if (!dataTable.IsNotNull())
		{
			dataTable = await _httpClient.FormFilterSettingClient.GetFilterLineStructure();
		}
		DataRow dataRow = dataTable.NewRow();
		dataRow["DocEntry"] = Function.ParseInt(gv_FilterSettingMrps.GetFocusedRowCellValue("DocEntry"));
		dataRow["LineNum"] = 0;
		dataRow["Active"] = "Y";
		dataTable.Rows.Add(dataRow);
		grd_Line.DataSource = dataTable;
		grd_Line.RefreshDataSource();
	}

	private async Task AddRowTable()
	{
		DataTable dataSourceTable = grd_Table.DataSourceTable;
		if (!dataSourceTable.IsNotNull())
		{
			dataSourceTable = await _httpClient.FormFilterSettingClient.GetFilterTableStructure();
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow["DocEntry"] = Function.ParseInt(gv_FilterSettingMrps.GetFocusedRowCellValue("DocEntry"));
			dataSourceTable.Rows.Add(dataRow);
			grd_Table.DataSource = dataSourceTable;
			grd_Table.RefreshDataSource();
		}
	}

	public bool IsNotFocusNewRow()
	{
		try
		{
			int focusedRowHandle = gv_FilterSettingMrps.FocusedRowHandle;
			int rowCount = gv_FilterSettingMrps.RowCount;
			if (focusedRowHandle != rowCount - 1 || !_isAddNew)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode != Enums.FormViewModeType.View)
			{
				WinCommons._OpenProcessing();
				DataRow focusedDataRow = gv_FilterSettingMrps.GetFocusedDataRow();
				DataTable dataTable = grd_FilterSettingMrps.DataSourceTable.Clone();
				dataTable.ImportRow(focusedDataRow);
				dataTable.TableName = "TbDoc";
				DataTable dataTable2 = grd_Line.DataSourceTable.Copy();
				dataTable2.TableName = "TbLine";
				DataTable dataTable3 = grd_Table.DataSourceTable.Copy();
				dataTable3.TableName = "TbTable";
				DataSet dataSet = new DataSet();
				dataSet.Tables.AddRange(new DataTable[3] { dataTable, dataTable2, dataTable3 });
				HttpResult httpResult = await _httpClient.FormFilterSettingClient.SaveFilterSetting(new Tuple<DataSet, ApzObjectTypes>(dataSet, objType));
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					isLoading = true;
					await LoadDataFormFilterSetting();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
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
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		try
		{
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_FilterSettingMrps_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode != Enums.FormViewModeType.Update)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			if (e.Column.FieldName == "SourceType")
			{
				string text = Function.ToString(gv_FilterSettingMrps.GetFocusedValue());
				tab_Line.PageEnabled = text == "D";
				tab_Table.PageEnabled = text == "T";
				grd_Table.DataSourceTable.Rows.Clear();
				grd_Line.DataSourceTable.Rows.Clear();
				await AddRowTable();
				await AddRowLine();
			}
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
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			if (gv_FilterSettingMrps.FocusedRowHandle != gv_FilterSettingMrps.RowCount - 1)
			{
				int docEntry = Function.ParseInt(gv_FilterSettingMrps.GetFocusedRowCellValue("DocEntry"));
				HttpResult httpResult = await _httpClient.FormFilterSettingClient.DeleteFilterSetting(docEntry);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Info(Enums.MessageCode.DeleteSuccess, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
			}
			await LoadDataFormFilterSetting();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_FilterSettingMrps_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		_ = 1;
		try
		{
			string text = Function.ToString(gv_FilterSettingMrps.GetFocusedRowCellValue("SourceType"));
			tab_Line.PageEnabled = text == "D";
			tab_Table.PageEnabled = text == "T";
			await LoadTable();
			await LoadLine();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void refreshToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel) != DialogResult.Cancel)
			{
				isLoading = true;
				await LoadDataFormFilterSetting();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Table_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "TableName")
			{
				string tableName = Function.ToString(gv_Table.GetFocusedValue());
				if ((await _httpClient.FormFilterSettingClient.GetDataTableByTableName(tableName)).Rows.Count > 0)
				{
					FormMode = Enums.FormViewModeType.Update;
					gv_Table.SetRowCellValue(e.RowHandle, "Code", null);
					gv_Table.SetRowCellValue(e.RowHandle, "Name", null);
				}
				else
				{
					gv_Table.RefreshRow(gv_Table.FocusedRowHandle);
					WinCommons.Info(Enums.MessageCode.TableNotFound, "");
				}
			}
			else
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

	private async void gv_Line_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
			if (e.RowHandle == gv_Line.RowCount - 1 && e.Column.FieldName == "Code")
			{
				await AddRowLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grd_Table.DatasourceTable();
			int focusedRowHandle = gv_Table.FocusedRowHandle;
			dataTable.Rows.RemoveAt(focusedRowHandle);
			dataTable.AcceptChanges();
			if (dataTable.Rows.Count == 0)
			{
				await AddRowTable();
			}
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void deleteRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grd_Line.DatasourceTable();
			int focusedRowHandle = gv_Line.FocusedRowHandle;
			dataTable.Rows.RemoveAt(focusedRowHandle);
			dataTable.AcceptChanges();
			if (dataTable.Rows.Count == 0)
			{
				await AddRowLine();
			}
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_FilterSettingMrps_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (isLoading)
			{
				e.Allow = false;
			}
			else if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
				{
					e.Allow = false;
					return;
				}
				isLoading = true;
				await LoadDataFormFilterSetting();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Table_ShownEditor(object sender, EventArgs e)
	{
		try
		{
			ColumnView view = sender as ColumnView;
			if (!(view.FocusedColumn.FieldName == "Code"))
			{
				return;
			}
			string tableName = Function.ToString(gv_Table.GetFocusedRowCellValue("TableName"));
			DataTable dataTable = await _httpClient.FormFilterSettingClient.GetDataTableByTableName(tableName);
			if (dataTable.Rows.Count > 0)
			{
				List<string> collection = (from r in dataTable.AsEnumerable()
					select Function.ToString(r[0])).ToList();
				(view.ActiveEditor as ComboBoxEdit).Properties.Items.AddRange(collection);
			}
			else
			{
				gv_Table.RefreshRow(gv_Table.FocusedRowHandle);
				WinCommons.Info(Enums.MessageCode.TableNotFound, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Mrps.frm_FormFilterSetting));
		this.grd_FilterSettingMrps = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_FilterSettingMrps = new Apzon.Controls.Grid.GridViewEx();
		this.cl_FilterCode = new Apzon.Controls.Grid.GridColumnEx();
		this.lookup_FilterCode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_FilterName = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Active = new Apzon.Controls.Grid.GridColumnEx();
		this.lookup_Active = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_SourceType = new Apzon.Controls.Grid.GridColumnEx();
		this.lookup_SourceType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_Remarks = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemLookUpEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridViewEx1 = new Apzon.Controls.Grid.GridViewEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainer1 = new System.Windows.Forms.SplitContainer();
		this.xtraTab = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_Table = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Table = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Table = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.cb_lstCol = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.tab_Line = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Active = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.grd_FilterSettingMrps).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_FilterSettingMrps).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_FilterCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_Active).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_SourceType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).BeginInit();
		this.splitContainer1.Panel1.SuspendLayout();
		this.splitContainer1.Panel2.SuspendLayout();
		this.splitContainer1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTab).BeginInit();
		this.xtraTab.SuspendLayout();
		this.tab_Table.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Table).BeginInit();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Table).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_lstCol).BeginInit();
		this.tab_Line.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		this.contextMenuStrip3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Active).BeginInit();
		base.SuspendLayout();
		this.grd_FilterSettingMrps.AllowPasteExcel = true;
		this.grd_FilterSettingMrps.ClipboardData = null;
		this.grd_FilterSettingMrps.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_FilterSettingMrps.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_FilterSettingMrps.Location = new System.Drawing.Point(0, 0);
		this.grd_FilterSettingMrps.MainView = this.gv_FilterSettingMrps;
		this.grd_FilterSettingMrps.Name = "grd_FilterSettingMrps";
		this.grd_FilterSettingMrps.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.repositoryItemLookUpEdit1, this.lookup_SourceType, this.lookup_Active, this.lookup_FilterCode });
		this.grd_FilterSettingMrps.Size = new System.Drawing.Size(591, 162);
		this.grd_FilterSettingMrps.TabIndex = 0;
		this.grd_FilterSettingMrps.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gv_FilterSettingMrps, this.gridViewEx1 });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.removeToolStripMenuItem, this.refreshToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 48);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
		this.refreshToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.refreshToolStripMenuItem.Text = "Refresh";
		this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
		this.gv_FilterSettingMrps.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_FilterSettingMrps.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_FilterSettingMrps.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_FilterSettingMrps.ColumnKey = null;
		this.gv_FilterSettingMrps.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.cl_FilterCode, this.cl_FilterName, this.cl_Active, this.cl_SourceType, this.cl_Remarks });
		this.gv_FilterSettingMrps.DisableFormSetting = false;
		this.gv_FilterSettingMrps.DisplayException = false;
		this.gv_FilterSettingMrps.EditColumn = null;
		this.gv_FilterSettingMrps.FieldCurrency = null;
		this.gv_FilterSettingMrps.FieldRate = null;
		this.gv_FilterSettingMrps.FormId = "oFilterSetting";
		this.gv_FilterSettingMrps.GridControl = this.grd_FilterSettingMrps;
		this.gv_FilterSettingMrps.IgnoreFilter = false;
		this.gv_FilterSettingMrps.IndexEditRow = -1;
		this.gv_FilterSettingMrps.IndicatorWidth = 35;
		this.gv_FilterSettingMrps.IsApzTable = false;
		this.gv_FilterSettingMrps.IsInsertNewRow = false;
		this.gv_FilterSettingMrps.IsLoadingData = false;
		this.gv_FilterSettingMrps.IsMappingContextAction = false;
		this.gv_FilterSettingMrps.ItemId = "ODATA";
		this.gv_FilterSettingMrps.LstColumnAllowBatch = null;
		this.gv_FilterSettingMrps.LstColumnNotAllowBatch = null;
		this.gv_FilterSettingMrps.LstColumnNotEditable = null;
		this.gv_FilterSettingMrps.LstColumNotSetting = null;
		this.gv_FilterSettingMrps.Name = "gv_FilterSettingMrps";
		this.gv_FilterSettingMrps.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_FilterSettingMrps.OptionsView.BestFitMaxRowCount = 100;
		this.gv_FilterSettingMrps.OptionsView.ColumnAutoWidth = false;
		this.gv_FilterSettingMrps.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_FilterSettingMrps.OptionsView.ShowGroupPanel = false;
		this.gv_FilterSettingMrps.UseBatchMod = false;
		this.gv_FilterSettingMrps.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_FilterSettingMrps_FocusedRowObjectChanged);
		this.gv_FilterSettingMrps.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_FilterSettingMrps_CellValueChanged);
		this.gv_FilterSettingMrps.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_FilterSettingMrps_BeforeLeaveRow);
		this.cl_FilterCode.Caption = "Filter Code";
		this.cl_FilterCode.ColumnEdit = this.lookup_FilterCode;
		this.cl_FilterCode.CurrencyDateField = null;
		this.cl_FilterCode.CurrencyField = null;
		this.cl_FilterCode.CurrencyFixed = null;
		this.cl_FilterCode.CurrencyFixedRate = 0.0;
		this.cl_FilterCode.CurrencyRateField = null;
		this.cl_FilterCode.FieldName = "FilterCode";
		this.cl_FilterCode.Name = "cl_FilterCode";
		this.cl_FilterCode.PrimaryKey = true;
		this.cl_FilterCode.UnitField = null;
		this.cl_FilterCode.Visible = true;
		this.cl_FilterCode.VisibleIndex = 0;
		this.lookup_FilterCode.AutoHeight = false;
		this.lookup_FilterCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookup_FilterCode.Name = "lookup_FilterCode";
		this.lookup_FilterCode.NullText = "";
		this.cl_FilterName.Caption = "Filter Name";
		this.cl_FilterName.CurrencyDateField = null;
		this.cl_FilterName.CurrencyField = null;
		this.cl_FilterName.CurrencyFixed = null;
		this.cl_FilterName.CurrencyFixedRate = 0.0;
		this.cl_FilterName.CurrencyRateField = null;
		this.cl_FilterName.FieldName = "FilterName";
		this.cl_FilterName.Name = "cl_FilterName";
		this.cl_FilterName.PrimaryKey = false;
		this.cl_FilterName.UnitField = null;
		this.cl_FilterName.Visible = true;
		this.cl_FilterName.VisibleIndex = 1;
		this.cl_Active.Caption = "Active";
		this.cl_Active.ColumnEdit = this.lookup_Active;
		this.cl_Active.CurrencyDateField = null;
		this.cl_Active.CurrencyField = null;
		this.cl_Active.CurrencyFixed = null;
		this.cl_Active.CurrencyFixedRate = 0.0;
		this.cl_Active.CurrencyRateField = null;
		this.cl_Active.FieldName = "Active";
		this.cl_Active.Name = "cl_Active";
		this.cl_Active.PrimaryKey = false;
		this.cl_Active.UnitField = null;
		this.cl_Active.Visible = true;
		this.cl_Active.VisibleIndex = 2;
		this.lookup_Active.AutoHeight = false;
		this.lookup_Active.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookup_Active.Name = "lookup_Active";
		this.lookup_Active.NullText = "";
		this.cl_SourceType.Caption = "Source Type";
		this.cl_SourceType.ColumnEdit = this.lookup_SourceType;
		this.cl_SourceType.CurrencyDateField = null;
		this.cl_SourceType.CurrencyField = null;
		this.cl_SourceType.CurrencyFixed = null;
		this.cl_SourceType.CurrencyFixedRate = 0.0;
		this.cl_SourceType.CurrencyRateField = null;
		this.cl_SourceType.FieldName = "SourceType";
		this.cl_SourceType.Name = "cl_SourceType";
		this.cl_SourceType.PrimaryKey = false;
		this.cl_SourceType.UnitField = null;
		this.cl_SourceType.Visible = true;
		this.cl_SourceType.VisibleIndex = 3;
		this.lookup_SourceType.AutoHeight = false;
		this.lookup_SourceType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookup_SourceType.Name = "lookup_SourceType";
		this.lookup_SourceType.NullText = "";
		this.cl_Remarks.Caption = "Remarks";
		this.cl_Remarks.CurrencyDateField = null;
		this.cl_Remarks.CurrencyField = null;
		this.cl_Remarks.CurrencyFixed = null;
		this.cl_Remarks.CurrencyFixedRate = 0.0;
		this.cl_Remarks.CurrencyRateField = null;
		this.cl_Remarks.FieldName = "Remarks";
		this.cl_Remarks.Name = "cl_Remarks";
		this.cl_Remarks.PrimaryKey = false;
		this.cl_Remarks.UnitField = null;
		this.cl_Remarks.Visible = true;
		this.cl_Remarks.VisibleIndex = 4;
		this.repositoryItemLookUpEdit1.AutoHeight = false;
		this.repositoryItemLookUpEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEdit1.DisplayMember = "Yes";
		this.repositoryItemLookUpEdit1.Name = "repositoryItemLookUpEdit1";
		this.repositoryItemLookUpEdit1.ValueMember = "Y";
		this.gridViewEx1.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gridViewEx1.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gridViewEx1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridViewEx1.ColumnKey = null;
		this.gridViewEx1.DisableFormSetting = false;
		this.gridViewEx1.DisplayException = false;
		this.gridViewEx1.EditColumn = null;
		this.gridViewEx1.FieldCurrency = null;
		this.gridViewEx1.FieldRate = null;
		this.gridViewEx1.FormId = "oFilterSetting";
		this.gridViewEx1.GridControl = this.grd_FilterSettingMrps;
		this.gridViewEx1.IgnoreFilter = false;
		this.gridViewEx1.IndexEditRow = -1;
		this.gridViewEx1.IndicatorWidth = 35;
		this.gridViewEx1.IsApzTable = false;
		this.gridViewEx1.IsInsertNewRow = false;
		this.gridViewEx1.IsLoadingData = false;
		this.gridViewEx1.IsMappingContextAction = false;
		this.gridViewEx1.ItemId = "OGRID1";
		this.gridViewEx1.LstColumnAllowBatch = null;
		this.gridViewEx1.LstColumnNotAllowBatch = null;
		this.gridViewEx1.LstColumnNotEditable = null;
		this.gridViewEx1.LstColumNotSetting = null;
		this.gridViewEx1.Name = "gridViewEx1";
		this.gridViewEx1.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gridViewEx1.OptionsView.BestFitMaxRowCount = 100;
		this.gridViewEx1.OptionsView.ColumnAutoWidth = false;
		this.gridViewEx1.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridViewEx1.OptionsView.ShowGroupPanel = false;
		this.gridViewEx1.UseBatchMod = false;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(6, 331);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 5;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(85, 331);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 6;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.splitContainer1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainer1.Location = new System.Drawing.Point(0, 0);
		this.splitContainer1.Name = "splitContainer1";
		this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
		this.splitContainer1.Panel1.Controls.Add(this.grd_FilterSettingMrps);
		this.splitContainer1.Panel2.Controls.Add(this.xtraTab);
		this.splitContainer1.Size = new System.Drawing.Size(591, 325);
		this.splitContainer1.SplitterDistance = 162;
		this.splitContainer1.TabIndex = 7;
		this.xtraTab.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTab.IsSearchControl = false;
		this.xtraTab.Location = new System.Drawing.Point(0, 0);
		this.xtraTab.Name = "xtraTab";
		this.xtraTab.SelectedTabPage = this.tab_Table;
		this.xtraTab.Size = new System.Drawing.Size(591, 159);
		this.xtraTab.TabIndex = 0;
		this.xtraTab.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.tab_Table, this.tab_Line });
		this.tab_Table.Controls.Add(this.grd_Table);
		this.tab_Table.EditControl = null;
		this.tab_Table.EditValue = "Filter Setting Table";
		this.tab_Table.FieldName = "";
		this.tab_Table.IsSearchControl = false;
		this.tab_Table.Name = "tab_Table";
		this.tab_Table.ReadOnly = false;
		this.tab_Table.SettingInVisible = false;
		this.tab_Table.SettingReadonly = false;
		this.tab_Table.Size = new System.Drawing.Size(589, 134);
		this.tab_Table.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Table.Text = "Filter Setting Table";
		this.grd_Table.AllowPasteExcel = true;
		this.grd_Table.ClipboardData = null;
		this.grd_Table.ContextMenuStrip = this.contextMenuStrip2;
		this.grd_Table.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Table.Location = new System.Drawing.Point(0, 0);
		this.grd_Table.MainView = this.gv_Table;
		this.grd_Table.Name = "grd_Table";
		this.grd_Table.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.cb_lstCol });
		this.grd_Table.Size = new System.Drawing.Size(589, 134);
		this.grd_Table.TabIndex = 0;
		this.grd_Table.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Table });
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(134, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gv_Table.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Table.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Table.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Table.ColumnKey = null;
		this.gv_Table.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7 });
		this.gv_Table.DisableFormSetting = false;
		this.gv_Table.DisplayException = false;
		this.gv_Table.EditColumn = null;
		this.gv_Table.FieldCurrency = null;
		this.gv_Table.FieldRate = null;
		this.gv_Table.FormId = null;
		this.gv_Table.GridControl = this.grd_Table;
		this.gv_Table.IgnoreFilter = false;
		this.gv_Table.IndexEditRow = -1;
		this.gv_Table.IndicatorWidth = 35;
		this.gv_Table.IsApzTable = false;
		this.gv_Table.IsInsertNewRow = false;
		this.gv_Table.IsLoadingData = false;
		this.gv_Table.IsMappingContextAction = false;
		this.gv_Table.ItemId = null;
		this.gv_Table.LstColumnAllowBatch = null;
		this.gv_Table.LstColumnNotAllowBatch = null;
		this.gv_Table.LstColumnNotEditable = null;
		this.gv_Table.LstColumNotSetting = null;
		this.gv_Table.Name = "gv_Table";
		this.gv_Table.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Table.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Table.OptionsPrint.AutoWidth = false;
		this.gv_Table.OptionsPrint.UsePrintStyles = false;
		this.gv_Table.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Table.OptionsView.ColumnAutoWidth = false;
		this.gv_Table.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Table.OptionsView.ShowGroupedColumns = true;
		this.gv_Table.OptionsView.ShowGroupPanel = false;
		this.gv_Table.UseBatchMod = false;
		this.gv_Table.ShownEditor += new System.EventHandler(this.gv_Table_ShownEditor);
		this.gv_Table.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Table_CellValueChanged);
		this.gridColumnEx5.Caption = "Table Name";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "TableName";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = true;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 0;
		this.gridColumnEx6.Caption = "Code";
		this.gridColumnEx6.ColumnEdit = this.cb_lstCol;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "Code";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 1;
		this.cb_lstCol.AutoHeight = false;
		this.cb_lstCol.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cb_lstCol.Name = "cb_lstCol";
		this.gridColumnEx7.Caption = "Name";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "Name";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 2;
		this.tab_Line.Controls.Add(this.grd_Line);
		this.tab_Line.EditControl = null;
		this.tab_Line.EditValue = "Filter Setting Line";
		this.tab_Line.FieldName = "";
		this.tab_Line.IsSearchControl = false;
		this.tab_Line.Name = "tab_Line";
		this.tab_Line.ReadOnly = false;
		this.tab_Line.SettingInVisible = false;
		this.tab_Line.SettingReadonly = false;
		this.tab_Line.Size = new System.Drawing.Size(589, 134);
		this.tab_Line.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Line.Text = "Filter Setting Line";
		this.grd_Line.AllowPasteExcel = false;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.ContextMenuStrip = this.contextMenuStrip3;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(0, 0);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_Active });
		this.grd_Line.Size = new System.Drawing.Size(589, 134);
		this.grd_Line.TabIndex = 0;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		this.contextMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem1 });
		this.contextMenuStrip3.Name = "contextMenuStrip3";
		this.contextMenuStrip3.Size = new System.Drawing.Size(134, 26);
		this.deleteRowToolStripMenuItem1.Name = "deleteRowToolStripMenuItem1";
		this.deleteRowToolStripMenuItem1.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem1.Text = "Delete Row";
		this.deleteRowToolStripMenuItem1.Click += new System.EventHandler(this.deleteRowToolStripMenuItem1_Click);
		this.gv_Line.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Line.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Line.ColumnKey = null;
		this.gv_Line.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4 });
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FormId = null;
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.IgnoreFilter = false;
		this.gv_Line.IndexEditRow = -1;
		this.gv_Line.IndicatorWidth = 35;
		this.gv_Line.IsApzTable = false;
		this.gv_Line.IsInsertNewRow = false;
		this.gv_Line.IsLoadingData = false;
		this.gv_Line.IsMappingContextAction = false;
		this.gv_Line.ItemId = null;
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
		this.gv_Line.OptionsView.ShowGroupedColumns = true;
		this.gv_Line.OptionsView.ShowGroupPanel = false;
		this.gv_Line.UseBatchMod = false;
		this.gv_Line.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Line_CellValueChanged);
		this.gridColumnEx1.Caption = "Code";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "Code";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx2.Caption = "Name";
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
		this.gridColumnEx3.Caption = "Active";
		this.gridColumnEx3.ColumnEdit = this.rep_Active;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "Active";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 2;
		this.rep_Active.AutoHeight = false;
		this.rep_Active.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Active.Name = "rep_Active";
		this.rep_Active.NullText = "";
		this.gridColumnEx4.Caption = "Remarks";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "Remarks";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 3;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(591, 360);
		this.ContextMenuStrip = this.contextMenuStrip1;
		base.Controls.Add(this.splitContainer1);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.btn_Cancel);
		base.FormId = "oFilterSetting";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_FormFilterSetting";
		this.Text = "Filter Setting";
		((System.ComponentModel.ISupportInitialize)this.grd_FilterSettingMrps).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_FilterSettingMrps).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_FilterCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_Active).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_SourceType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).EndInit();
		this.splitContainer1.Panel1.ResumeLayout(false);
		this.splitContainer1.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).EndInit();
		this.splitContainer1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTab).EndInit();
		this.xtraTab.ResumeLayout(false);
		this.tab_Table.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Table).EndInit();
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Table).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_lstCol).EndInit();
		this.tab_Line.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		this.contextMenuStrip3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Active).EndInit();
		base.ResumeLayout(false);
	}
}
