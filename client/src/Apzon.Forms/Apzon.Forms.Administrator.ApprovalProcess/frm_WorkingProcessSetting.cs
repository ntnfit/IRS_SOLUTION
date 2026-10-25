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
using Apzon.Controls.Tree;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTab;

namespace Apzon.Forms.Administrator.ApprovalProcess;

public class frm_WorkingProcessSetting : XtraFormEx
{
	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private string _type = string.Empty;

	private int _userDefault = -1;

	public bool _isHasData;

	public bool IsChanged;

	private IContainer components;

	private PanelControl panelControl1;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	private XtraTabControlEx tab_Functions;

	private XtraTabPageEx tabPage_Process;

	private GridControlEx grd_Process;

	private GridViewEx gv_Process;

	private RepositoryItemCheckEdit rep_ItemCheck;

	private GridColumnEx gc_Id;

	private GridColumnEx gc_UserID;

	private GridColumnEx gc_DocType;

	private GridColumnEx gc_Remarks;

	private GridColumnEx gc_MnuType;

	private XtraTabPageEx tabPage_Document;

	private GridControlEx grd_Document;

	private GridViewEx gv_Document;

	private RepositoryItemCheckEdit repositoryItemCheckEdit3;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private XtraTabPageEx tabPage_Tooltips;

	private GridControlEx grd_Tooltip;

	private GridViewEx gv_Tooltip;

	private GridColumnEx gridColumnEx7;

	private RepositoryItemButtonEdit rep_Columns;

	private GridColumnEx gc_DisplayText;

	private GridColumnEx gc_ColVisible;

	private GridViewEx gridViewEx1;

	private GridViewEx gridViewEx2;

	private GridViewEx gridViewEx3;

	private GridViewEx gridViewEx4;

	private RepositoryItemCalcEdit rep_VisibleOrder;

	private GridColumnEx gc_Visible1;

	private RepositoryItemCheckEdit rep_Visible1;

	private GridColumnEx gc_Visible2;

	private RepositoryItemCheckEdit rep_Visible2;

	private TreeListColumnEx treeListColumnEx1;

	private TreeListColumnEx treeListColumnEx2;

	private TreeListColumnEx treeListColumnEx3;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem restoreDefaultToolStripMenuItem;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_WorkingProcessSetting(string type, bool isHasData)
	{
		InitializeComponent();
		_type = type;
		_isHasData = isHasData;
		OnLoadAsync += frm_WorkingProcessSetting_Load;
	}

	public void LoadData()
	{
		try
		{
			WinCommons._OpenProcessing();
			DataSet workingProcessSetting = _httpClient.WorkingProcessClient.GetWorkingProcessSetting(_userDefault);
			List<DataRow> list = (from row in workingProcessSetting.Tables[0].AsEnumerable()
				where Function.ToString(row["MnuType"]) == "P"
				select row).ToList();
			if (list.Count > 0)
			{
				grd_Process.DataSource = list.CopyToDataTable();
				grd_Process.RefreshDataSource();
			}
			List<DataRow> list2 = (from row in workingProcessSetting.Tables[0].AsEnumerable()
				where Function.ToString(row["MnuType"]) == "D"
				select row).ToList();
			if (list2.Count > 0)
			{
				grd_Document.DataSource = list2.CopyToDataTable();
				grd_Document.RefreshDataSource();
			}
			grd_Tooltip.DataSource = workingProcessSetting.Tables[1];
			grd_Tooltip.RefreshDataSource();
			gv_Process.BestFitColumns();
			gv_Document.BestFitColumns();
			gv_Tooltip.BestFitColumns();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void AddNewTooltips()
	{
		try
		{
			DataTable dataTable = grd_Tooltip.DataSourceTable;
			if (dataTable.Columns.Count == 0)
			{
				dataTable = WinFuntions.GetTableStructure("APZ_WPS2");
			}
			DataRow dataRow = dataTable.NewRow();
			int num = -1;
			num = (dataTable.IsNotNull() ? (Function.ParseInt(dataTable.AsEnumerable().Max((DataRow r) => Function.ParseInt(r["VisOrder"]))) + 1) : 0);
			dataRow["UserId"] = _userDefault;
			dataRow["VisOrder"] = num;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grd_Tooltip.DataSource = dataTable;
			grd_Tooltip.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task frm_WorkingProcessSetting_Load(object sender, EventArgs e)
	{
		try
		{
			_userDefault = GlobalData.UserID;
			switch (_type)
			{
			case "P":
				tab_Functions.SelectedTabPageIndex = 0;
				break;
			case "D":
				tab_Functions.SelectedTabPageIndex = 1;
				break;
			case "T":
				tab_Functions.SelectedTabPageIndex = 3;
				break;
			}
			LoadData();
			AddNewTooltips();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				WinCommons._OpenProcessing();
				btn_OK.Enabled = false;
				grd_Document.DataSourceTable.AcceptChanges();
				WinFuntions.GetTableStructure("APZ_WPS1");
				DataTable dataTable = grd_Process.DataSourceTable.Copy();
				dataTable.Merge(grd_Document.DataSourceTable.Copy());
				dataTable.TableName = "dtMenu";
				DataTable dataSourceTable = grd_Tooltip.DataSourceTable;
				dataSourceTable.TableName = "dtTooltips";
				DataSet dataSet = new DataSet();
				dataSet.Tables.Add(dataTable.Copy());
				dataSet.Tables.Add(dataSourceTable.Copy());
				if (dataSet.Tables.Count > 0)
				{
					HttpResult httpResult = await _httpClient.WorkingProcessClient.SaveWorkingProcessSetting(new Tuple<DataSet>(dataSet));
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
						IsChanged = false;
						btn_OK.Enabled = true;
						return;
					}
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					LoadData();
					if (tab_Functions.SelectedTabPageIndex == 2)
					{
						AddNewTooltips();
					}
					IsChanged = true;
					FormMode = Enums.FormViewModeType.View;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btn_OK.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gv_Data_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
	}

	private void frm_WorkingProcessSetting_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					e.Cancel = true;
				}
				IsChanged = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_Columns_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			using frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.WorkingProcessTooltips, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
			DataTable dataTable = (frm_CFL2.dtSource = grd_Tooltip.DataSourceTable);
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems.Count > 0)
			{
				WinCommons._OpenProcessing();
				dataTable.Rows.RemoveAt(dataTable.Rows.Count - 1);
				foreach (DataRow chooseItem in frm_CFL2.ChooseItems)
				{
					DataRow dataRow = dataTable.NewRow();
					int num = Function.ParseInt(dataTable.AsEnumerable().Max((DataRow r) => r["VisOrder"])) + 1;
					dataRow["UserId"] = _userDefault;
					dataRow["ColId"] = chooseItem["ColumnName"];
					dataRow["DispText"] = chooseItem["ColumnName"];
					dataRow["TableName"] = chooseItem["TableName"];
					dataRow["ExecQuery"] = chooseItem["Query"];
					dataRow["VisOrder"] = num;
					dataTable.Rows.Add(dataRow);
				}
				dataTable.AcceptChanges();
				grd_Tooltip.DataSource = dataTable;
				grd_Tooltip.RefreshDataSource();
				AddNewTooltips();
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			gv_Tooltip.DeleteDataSourceRowSelected();
			grd_Tooltip.DataSourceTable.AcceptChanges();
			grd_Tooltip.RefreshDataSource();
			if (!grd_Tooltip.DataSourceTable.IsNotNull())
			{
				AddNewTooltips();
			}
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Tooltip_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.ApprovalProcess.frm_WorkingProcessSetting));
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.tab_Functions = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tabPage_Process = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Process = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Process = new Apzon.Controls.Grid.GridViewEx();
		this.gc_Id = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_UserID = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_DocType = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Remarks = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_MnuType = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Visible1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Visible1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.rep_ItemCheck = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridViewEx4 = new Apzon.Controls.Grid.GridViewEx();
		this.tabPage_Document = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Document = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Document = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Visible2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Visible2 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.repositoryItemCheckEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridViewEx3 = new Apzon.Controls.Grid.GridViewEx();
		this.tabPage_Tooltips = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Tooltip = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Tooltip = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Columns = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_DisplayText = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ColVisible = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_VisibleOrder = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gridViewEx1 = new Apzon.Controls.Grid.GridViewEx();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.restoreDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.treeListColumnEx1 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx2 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx3 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.gridViewEx2 = new Apzon.Controls.Grid.GridViewEx();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tab_Functions).BeginInit();
		this.tab_Functions.SuspendLayout();
		this.tabPage_Process.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Process).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Process).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Visible1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemCheck).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx4).BeginInit();
		this.tabPage_Document.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Document).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Document).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Visible2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx3).BeginInit();
		this.tabPage_Tooltips.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Tooltip).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Tooltip).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Columns).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_VisibleOrder).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).BeginInit();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx2).BeginInit();
		base.SuspendLayout();
		this.panelControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.panelControl1.Controls.Add(this.tab_Functions);
		this.panelControl1.Location = new System.Drawing.Point(2, 2);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(582, 385);
		this.panelControl1.TabIndex = 0;
		this.tab_Functions.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tab_Functions.IsSearchControl = false;
		this.tab_Functions.Location = new System.Drawing.Point(0, 0);
		this.tab_Functions.Name = "tab_Functions";
		this.tab_Functions.SelectedTabPage = this.tabPage_Process;
		this.tab_Functions.Size = new System.Drawing.Size(582, 385);
		this.tab_Functions.TabIndex = 2;
		this.tab_Functions.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[3] { this.tabPage_Process, this.tabPage_Document, this.tabPage_Tooltips });
		this.tabPage_Process.Controls.Add(this.grd_Process);
		this.tabPage_Process.EditControl = null;
		this.tabPage_Process.EditValue = "Process";
		this.tabPage_Process.FieldName = "";
		this.tabPage_Process.IsSearchControl = false;
		this.tabPage_Process.Name = "tabPage_Process";
		this.tabPage_Process.ReadOnly = false;
		this.tabPage_Process.SettingInVisible = false;
		this.tabPage_Process.SettingReadonly = false;
		this.tabPage_Process.Size = new System.Drawing.Size(580, 360);
		this.tabPage_Process.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabPage_Process.Text = "Process";
		this.grd_Process.AllowPasteExcel = true;
		this.grd_Process.ClipboardData = null;
		this.grd_Process.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Process.Location = new System.Drawing.Point(0, 0);
		this.grd_Process.MainView = this.gv_Process;
		this.grd_Process.Name = "grd_Process";
		this.grd_Process.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_ItemCheck, this.rep_Visible1 });
		this.grd_Process.Size = new System.Drawing.Size(580, 360);
		this.grd_Process.TabIndex = 1;
		this.grd_Process.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gv_Process, this.gridViewEx4 });
		this.gv_Process.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Process.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Process.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Process.ColumnKey = null;
		this.gv_Process.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gc_Id, this.gc_UserID, this.gc_DocType, this.gc_Remarks, this.gc_MnuType, this.gc_Visible1 });
		this.gv_Process.DisableFormSetting = false;
		this.gv_Process.DisplayException = false;
		this.gv_Process.EditColumn = null;
		this.gv_Process.FieldCurrency = null;
		this.gv_Process.FieldRate = null;
		this.gv_Process.FormId = "oWorkingProcessSetting";
		this.gv_Process.GridControl = this.grd_Process;
		this.gv_Process.IgnoreFilter = false;
		this.gv_Process.IndexEditRow = -1;
		this.gv_Process.IndicatorWidth = 35;
		this.gv_Process.IsApzTable = false;
		this.gv_Process.IsInsertNewRow = false;
		this.gv_Process.IsLoadingData = false;
		this.gv_Process.IsMappingContextAction = false;
		this.gv_Process.ItemId = "APZ_WPS1";
		this.gv_Process.LstColumnAllowBatch = null;
		this.gv_Process.LstColumnNotAllowBatch = null;
		this.gv_Process.LstColumnNotEditable = null;
		this.gv_Process.LstColumNotSetting = null;
		this.gv_Process.Name = "gv_Process";
		this.gv_Process.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Process.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Process.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_Process.OptionsPrint.AutoWidth = false;
		this.gv_Process.OptionsPrint.UsePrintStyles = false;
		this.gv_Process.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Process.OptionsView.ColumnAutoWidth = false;
		this.gv_Process.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Process.OptionsView.ShowGroupedColumns = true;
		this.gv_Process.OptionsView.ShowGroupPanel = false;
		this.gv_Process.UseBatchMod = false;
		this.gv_Process.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Data_CellValueChanged);
		this.gc_Id.Caption = "Id";
		this.gc_Id.CurrencyDateField = null;
		this.gc_Id.CurrencyField = null;
		this.gc_Id.CurrencyFixed = null;
		this.gc_Id.CurrencyFixedRate = 0.0;
		this.gc_Id.CurrencyRateField = null;
		this.gc_Id.FieldName = "Id";
		this.gc_Id.Name = "gc_Id";
		this.gc_Id.OptionsColumn.ReadOnly = true;
		this.gc_Id.PrimaryKey = true;
		this.gc_Id.UnitField = null;
		this.gc_UserID.Caption = "UserId";
		this.gc_UserID.CurrencyDateField = null;
		this.gc_UserID.CurrencyField = null;
		this.gc_UserID.CurrencyFixed = null;
		this.gc_UserID.CurrencyFixedRate = 0.0;
		this.gc_UserID.CurrencyRateField = null;
		this.gc_UserID.FieldName = "UserId";
		this.gc_UserID.Name = "gc_UserID";
		this.gc_UserID.OptionsColumn.ReadOnly = true;
		this.gc_UserID.PrimaryKey = false;
		this.gc_UserID.UnitField = null;
		this.gc_DocType.Caption = "Name";
		this.gc_DocType.CurrencyDateField = null;
		this.gc_DocType.CurrencyField = null;
		this.gc_DocType.CurrencyFixed = null;
		this.gc_DocType.CurrencyFixedRate = 0.0;
		this.gc_DocType.CurrencyRateField = null;
		this.gc_DocType.FieldName = "DocType";
		this.gc_DocType.Name = "gc_DocType";
		this.gc_DocType.OptionsColumn.ReadOnly = true;
		this.gc_DocType.PrimaryKey = false;
		this.gc_DocType.UnitField = null;
		this.gc_DocType.Visible = true;
		this.gc_DocType.VisibleIndex = 0;
		this.gc_Remarks.Caption = "Remarks";
		this.gc_Remarks.CurrencyDateField = null;
		this.gc_Remarks.CurrencyField = null;
		this.gc_Remarks.CurrencyFixed = null;
		this.gc_Remarks.CurrencyFixedRate = 0.0;
		this.gc_Remarks.CurrencyRateField = null;
		this.gc_Remarks.FieldName = "Remarks";
		this.gc_Remarks.Name = "gc_Remarks";
		this.gc_Remarks.OptionsColumn.ReadOnly = true;
		this.gc_Remarks.PrimaryKey = false;
		this.gc_Remarks.UnitField = null;
		this.gc_Remarks.Visible = true;
		this.gc_Remarks.VisibleIndex = 1;
		this.gc_MnuType.Caption = "Menu Type";
		this.gc_MnuType.CurrencyDateField = null;
		this.gc_MnuType.CurrencyField = null;
		this.gc_MnuType.CurrencyFixed = null;
		this.gc_MnuType.CurrencyFixedRate = 0.0;
		this.gc_MnuType.CurrencyRateField = null;
		this.gc_MnuType.FieldName = "MnuType";
		this.gc_MnuType.Name = "gc_MnuType";
		this.gc_MnuType.OptionsColumn.ReadOnly = true;
		this.gc_MnuType.PrimaryKey = false;
		this.gc_MnuType.UnitField = null;
		this.gc_Visible1.Caption = "Visible";
		this.gc_Visible1.ColumnEdit = this.rep_Visible1;
		this.gc_Visible1.CurrencyDateField = null;
		this.gc_Visible1.CurrencyField = null;
		this.gc_Visible1.CurrencyFixed = null;
		this.gc_Visible1.CurrencyFixedRate = 0.0;
		this.gc_Visible1.CurrencyRateField = null;
		this.gc_Visible1.FieldName = "Visible";
		this.gc_Visible1.Name = "gc_Visible1";
		this.gc_Visible1.PrimaryKey = false;
		this.gc_Visible1.UnitField = null;
		this.gc_Visible1.Visible = true;
		this.gc_Visible1.VisibleIndex = 2;
		this.gc_Visible1.Width = 40;
		this.rep_Visible1.AutoHeight = false;
		this.rep_Visible1.DisplayValueChecked = "Y";
		this.rep_Visible1.DisplayValueUnchecked = "N";
		this.rep_Visible1.Name = "rep_Visible1";
		this.rep_Visible1.ValueChecked = "Y";
		this.rep_Visible1.ValueUnchecked = "N";
		this.rep_ItemCheck.AutoHeight = false;
		this.rep_ItemCheck.AutoWidth = true;
		this.rep_ItemCheck.DisplayValueChecked = "Y";
		this.rep_ItemCheck.DisplayValueUnchecked = "N";
		this.rep_ItemCheck.Name = "rep_ItemCheck";
		this.rep_ItemCheck.ValueChecked = "Y";
		this.rep_ItemCheck.ValueUnchecked = "N";
		this.gridViewEx4.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gridViewEx4.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gridViewEx4.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridViewEx4.ColumnKey = null;
		this.gridViewEx4.DisableFormSetting = false;
		this.gridViewEx4.DisplayException = false;
		this.gridViewEx4.EditColumn = null;
		this.gridViewEx4.FieldCurrency = null;
		this.gridViewEx4.FieldRate = null;
		this.gridViewEx4.FormId = null;
		this.gridViewEx4.GridControl = this.grd_Process;
		this.gridViewEx4.IgnoreFilter = false;
		this.gridViewEx4.IndexEditRow = -1;
		this.gridViewEx4.IndicatorWidth = 35;
		this.gridViewEx4.IsApzTable = false;
		this.gridViewEx4.IsInsertNewRow = false;
		this.gridViewEx4.IsLoadingData = false;
		this.gridViewEx4.IsMappingContextAction = false;
		this.gridViewEx4.ItemId = null;
		this.gridViewEx4.LstColumnAllowBatch = null;
		this.gridViewEx4.LstColumnNotAllowBatch = null;
		this.gridViewEx4.LstColumnNotEditable = null;
		this.gridViewEx4.LstColumNotSetting = null;
		this.gridViewEx4.Name = "gridViewEx4";
		this.gridViewEx4.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gridViewEx4.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gridViewEx4.OptionsPrint.AutoWidth = false;
		this.gridViewEx4.OptionsPrint.UsePrintStyles = false;
		this.gridViewEx4.OptionsView.BestFitMaxRowCount = 100;
		this.gridViewEx4.OptionsView.ColumnAutoWidth = false;
		this.gridViewEx4.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridViewEx4.OptionsView.ShowGroupedColumns = true;
		this.gridViewEx4.OptionsView.ShowGroupPanel = false;
		this.gridViewEx4.UseBatchMod = false;
		this.tabPage_Document.Controls.Add(this.grd_Document);
		this.tabPage_Document.EditControl = null;
		this.tabPage_Document.EditValue = "Document";
		this.tabPage_Document.FieldName = "";
		this.tabPage_Document.IsSearchControl = false;
		this.tabPage_Document.Name = "tabPage_Document";
		this.tabPage_Document.ReadOnly = false;
		this.tabPage_Document.SettingInVisible = false;
		this.tabPage_Document.SettingReadonly = false;
		this.tabPage_Document.Size = new System.Drawing.Size(580, 360);
		this.tabPage_Document.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabPage_Document.Text = "Document";
		this.grd_Document.AllowPasteExcel = false;
		this.grd_Document.ClipboardData = null;
		this.grd_Document.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Document.Location = new System.Drawing.Point(0, 0);
		this.grd_Document.MainView = this.gv_Document;
		this.grd_Document.Name = "grd_Document";
		this.grd_Document.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemCheckEdit3, this.rep_Visible2 });
		this.grd_Document.Size = new System.Drawing.Size(580, 360);
		this.grd_Document.TabIndex = 2;
		this.grd_Document.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gv_Document, this.gridViewEx3 });
		this.gv_Document.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Document.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Document.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Document.ColumnKey = null;
		this.gv_Document.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gc_Visible2 });
		this.gv_Document.DisableFormSetting = false;
		this.gv_Document.DisplayException = false;
		this.gv_Document.EditColumn = null;
		this.gv_Document.FieldCurrency = null;
		this.gv_Document.FieldRate = null;
		this.gv_Document.FormId = "oWorkingProcessSetting";
		this.gv_Document.GridControl = this.grd_Document;
		this.gv_Document.IgnoreFilter = false;
		this.gv_Document.IndexEditRow = -1;
		this.gv_Document.IndicatorWidth = 35;
		this.gv_Document.IsApzTable = false;
		this.gv_Document.IsInsertNewRow = false;
		this.gv_Document.IsLoadingData = false;
		this.gv_Document.IsMappingContextAction = false;
		this.gv_Document.ItemId = "APZ_WPS1";
		this.gv_Document.LstColumnAllowBatch = null;
		this.gv_Document.LstColumnNotAllowBatch = null;
		this.gv_Document.LstColumnNotEditable = null;
		this.gv_Document.LstColumNotSetting = null;
		this.gv_Document.Name = "gv_Document";
		this.gv_Document.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Document.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Document.OptionsPrint.AutoWidth = false;
		this.gv_Document.OptionsPrint.UsePrintStyles = false;
		this.gv_Document.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Document.OptionsView.ColumnAutoWidth = false;
		this.gv_Document.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Document.OptionsView.ShowGroupedColumns = true;
		this.gv_Document.OptionsView.ShowGroupPanel = false;
		this.gv_Document.UseBatchMod = false;
		this.gv_Document.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Data_CellValueChanged);
		this.gridColumnEx2.Caption = "Id";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "Id";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.OptionsColumn.ReadOnly = true;
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx3.Caption = "UserId";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "UserId";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsColumn.ReadOnly = true;
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx4.Caption = "Name";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "DocType";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.OptionsColumn.ReadOnly = true;
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 0;
		this.gridColumnEx5.Caption = "Remarks";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "Remarks";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.OptionsColumn.ReadOnly = true;
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 1;
		this.gridColumnEx6.Caption = "Menu Type";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "MnuType";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.OptionsColumn.ReadOnly = true;
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gc_Visible2.Caption = "Visible";
		this.gc_Visible2.ColumnEdit = this.rep_Visible2;
		this.gc_Visible2.CurrencyDateField = null;
		this.gc_Visible2.CurrencyField = null;
		this.gc_Visible2.CurrencyFixed = null;
		this.gc_Visible2.CurrencyFixedRate = 0.0;
		this.gc_Visible2.CurrencyRateField = null;
		this.gc_Visible2.FieldName = "Visible";
		this.gc_Visible2.Name = "gc_Visible2";
		this.gc_Visible2.PrimaryKey = false;
		this.gc_Visible2.UnitField = null;
		this.gc_Visible2.Visible = true;
		this.gc_Visible2.VisibleIndex = 2;
		this.gc_Visible2.Width = 40;
		this.rep_Visible2.AutoHeight = false;
		this.rep_Visible2.DisplayValueChecked = "Y";
		this.rep_Visible2.DisplayValueUnchecked = "N";
		this.rep_Visible2.Name = "rep_Visible2";
		this.rep_Visible2.ValueChecked = "Y";
		this.rep_Visible2.ValueUnchecked = "N";
		this.repositoryItemCheckEdit3.AutoHeight = false;
		this.repositoryItemCheckEdit3.AutoWidth = true;
		this.repositoryItemCheckEdit3.DisplayValueChecked = "Y";
		this.repositoryItemCheckEdit3.DisplayValueUnchecked = "N";
		this.repositoryItemCheckEdit3.Name = "repositoryItemCheckEdit3";
		this.repositoryItemCheckEdit3.ValueChecked = "Y";
		this.repositoryItemCheckEdit3.ValueUnchecked = "N";
		this.gridViewEx3.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gridViewEx3.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gridViewEx3.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridViewEx3.ColumnKey = null;
		this.gridViewEx3.DisableFormSetting = false;
		this.gridViewEx3.DisplayException = false;
		this.gridViewEx3.EditColumn = null;
		this.gridViewEx3.FieldCurrency = null;
		this.gridViewEx3.FieldRate = null;
		this.gridViewEx3.FormId = null;
		this.gridViewEx3.GridControl = this.grd_Document;
		this.gridViewEx3.IgnoreFilter = false;
		this.gridViewEx3.IndexEditRow = -1;
		this.gridViewEx3.IndicatorWidth = 35;
		this.gridViewEx3.IsApzTable = false;
		this.gridViewEx3.IsInsertNewRow = false;
		this.gridViewEx3.IsLoadingData = false;
		this.gridViewEx3.IsMappingContextAction = false;
		this.gridViewEx3.ItemId = null;
		this.gridViewEx3.LstColumnAllowBatch = null;
		this.gridViewEx3.LstColumnNotAllowBatch = null;
		this.gridViewEx3.LstColumnNotEditable = null;
		this.gridViewEx3.LstColumNotSetting = null;
		this.gridViewEx3.Name = "gridViewEx3";
		this.gridViewEx3.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gridViewEx3.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gridViewEx3.OptionsPrint.AutoWidth = false;
		this.gridViewEx3.OptionsPrint.UsePrintStyles = false;
		this.gridViewEx3.OptionsView.BestFitMaxRowCount = 100;
		this.gridViewEx3.OptionsView.ColumnAutoWidth = false;
		this.gridViewEx3.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridViewEx3.OptionsView.ShowGroupedColumns = true;
		this.gridViewEx3.OptionsView.ShowGroupPanel = false;
		this.gridViewEx3.UseBatchMod = false;
		this.tabPage_Tooltips.Controls.Add(this.grd_Tooltip);
		this.tabPage_Tooltips.EditControl = null;
		this.tabPage_Tooltips.EditValue = "Tooltips";
		this.tabPage_Tooltips.FieldName = "";
		this.tabPage_Tooltips.IsSearchControl = false;
		this.tabPage_Tooltips.Name = "tabPage_Tooltips";
		this.tabPage_Tooltips.ReadOnly = false;
		this.tabPage_Tooltips.SettingInVisible = false;
		this.tabPage_Tooltips.SettingReadonly = false;
		this.tabPage_Tooltips.Size = new System.Drawing.Size(580, 360);
		this.tabPage_Tooltips.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabPage_Tooltips.Text = "Tooltips";
		this.grd_Tooltip.AllowPasteExcel = false;
		this.grd_Tooltip.ClipboardData = null;
		this.grd_Tooltip.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Tooltip.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Tooltip.Location = new System.Drawing.Point(0, 0);
		this.grd_Tooltip.MainView = this.gv_Tooltip;
		this.grd_Tooltip.Name = "grd_Tooltip";
		this.grd_Tooltip.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_Columns, this.rep_VisibleOrder });
		this.grd_Tooltip.Size = new System.Drawing.Size(580, 360);
		this.grd_Tooltip.TabIndex = 4;
		this.grd_Tooltip.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gv_Tooltip, this.gridViewEx1 });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_Tooltip.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Tooltip.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Tooltip.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Tooltip.ColumnKey = null;
		this.gv_Tooltip.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gridColumnEx7, this.gc_DisplayText, this.gc_ColVisible });
		this.gv_Tooltip.DisableFormSetting = false;
		this.gv_Tooltip.DisplayException = false;
		this.gv_Tooltip.EditColumn = null;
		this.gv_Tooltip.FieldCurrency = null;
		this.gv_Tooltip.FieldRate = null;
		this.gv_Tooltip.FormId = "oWorkingProcessSetting";
		this.gv_Tooltip.GridControl = this.grd_Tooltip;
		this.gv_Tooltip.IgnoreFilter = false;
		this.gv_Tooltip.IndexEditRow = -1;
		this.gv_Tooltip.IndicatorWidth = 35;
		this.gv_Tooltip.IsApzTable = false;
		this.gv_Tooltip.IsInsertNewRow = false;
		this.gv_Tooltip.IsLoadingData = false;
		this.gv_Tooltip.IsMappingContextAction = false;
		this.gv_Tooltip.ItemId = "APZ_WPS2";
		this.gv_Tooltip.LstColumnAllowBatch = null;
		this.gv_Tooltip.LstColumnNotAllowBatch = null;
		this.gv_Tooltip.LstColumnNotEditable = null;
		this.gv_Tooltip.LstColumNotSetting = null;
		this.gv_Tooltip.Name = "gv_Tooltip";
		this.gv_Tooltip.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Tooltip.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Tooltip.OptionsNavigation.AutoFocusNewRow = true;
		this.gv_Tooltip.OptionsPrint.AutoWidth = false;
		this.gv_Tooltip.OptionsPrint.UsePrintStyles = false;
		this.gv_Tooltip.OptionsSelection.MultiSelect = true;
		this.gv_Tooltip.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Tooltip.OptionsView.ColumnAutoWidth = false;
		this.gv_Tooltip.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Tooltip.OptionsView.ShowGroupedColumns = true;
		this.gv_Tooltip.OptionsView.ShowGroupPanel = false;
		this.gv_Tooltip.UseBatchMod = false;
		this.gv_Tooltip.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Tooltip_CellValueChanged);
		this.gridColumnEx7.Caption = "Column Name";
		this.gridColumnEx7.ColumnEdit = this.rep_Columns;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "ColId";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 0;
		this.rep_Columns.AutoHeight = false;
		this.rep_Columns.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_Columns.Name = "rep_Columns";
		this.rep_Columns.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Columns_ButtonClick);
		this.gc_DisplayText.Caption = "Display Text";
		this.gc_DisplayText.CurrencyDateField = null;
		this.gc_DisplayText.CurrencyField = null;
		this.gc_DisplayText.CurrencyFixed = null;
		this.gc_DisplayText.CurrencyFixedRate = 0.0;
		this.gc_DisplayText.CurrencyRateField = null;
		this.gc_DisplayText.FieldName = "DispText";
		this.gc_DisplayText.Name = "gc_DisplayText";
		this.gc_DisplayText.PrimaryKey = false;
		this.gc_DisplayText.UnitField = null;
		this.gc_DisplayText.Visible = true;
		this.gc_DisplayText.VisibleIndex = 1;
		this.gc_ColVisible.Caption = "Visible Order";
		this.gc_ColVisible.ColumnEdit = this.rep_VisibleOrder;
		this.gc_ColVisible.CurrencyDateField = null;
		this.gc_ColVisible.CurrencyField = null;
		this.gc_ColVisible.CurrencyFixed = null;
		this.gc_ColVisible.CurrencyFixedRate = 0.0;
		this.gc_ColVisible.CurrencyRateField = null;
		this.gc_ColVisible.FieldName = "VisOrder";
		this.gc_ColVisible.Name = "gc_ColVisible";
		this.gc_ColVisible.PrimaryKey = false;
		this.gc_ColVisible.UnitField = null;
		this.gc_ColVisible.Visible = true;
		this.gc_ColVisible.VisibleIndex = 2;
		this.gc_ColVisible.Width = 80;
		this.rep_VisibleOrder.AutoHeight = false;
		this.rep_VisibleOrder.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_VisibleOrder.Name = "rep_VisibleOrder";
		this.gridViewEx1.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gridViewEx1.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gridViewEx1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridViewEx1.ColumnKey = null;
		this.gridViewEx1.DisableFormSetting = false;
		this.gridViewEx1.DisplayException = false;
		this.gridViewEx1.EditColumn = null;
		this.gridViewEx1.FieldCurrency = null;
		this.gridViewEx1.FieldRate = null;
		this.gridViewEx1.FormId = null;
		this.gridViewEx1.GridControl = this.grd_Tooltip;
		this.gridViewEx1.IgnoreFilter = false;
		this.gridViewEx1.IndexEditRow = -1;
		this.gridViewEx1.IndicatorWidth = 35;
		this.gridViewEx1.IsApzTable = false;
		this.gridViewEx1.IsInsertNewRow = false;
		this.gridViewEx1.IsLoadingData = false;
		this.gridViewEx1.IsMappingContextAction = false;
		this.gridViewEx1.ItemId = null;
		this.gridViewEx1.LstColumnAllowBatch = null;
		this.gridViewEx1.LstColumnNotAllowBatch = null;
		this.gridViewEx1.LstColumnNotEditable = null;
		this.gridViewEx1.LstColumNotSetting = null;
		this.gridViewEx1.Name = "gridViewEx1";
		this.gridViewEx1.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gridViewEx1.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gridViewEx1.OptionsPrint.AutoWidth = false;
		this.gridViewEx1.OptionsPrint.UsePrintStyles = false;
		this.gridViewEx1.OptionsView.BestFitMaxRowCount = 100;
		this.gridViewEx1.OptionsView.ColumnAutoWidth = false;
		this.gridViewEx1.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridViewEx1.OptionsView.ShowGroupedColumns = true;
		this.gridViewEx1.OptionsView.ShowGroupPanel = false;
		this.gridViewEx1.UseBatchMod = false;
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.restoreDefaultToolStripMenuItem });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(158, 26);
		this.restoreDefaultToolStripMenuItem.Name = "restoreDefaultToolStripMenuItem";
		this.restoreDefaultToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
		this.restoreDefaultToolStripMenuItem.Text = " Restore Default";
		this.treeListColumnEx1.Caption = "Tile Item";
		this.treeListColumnEx1.CurrencyDateField = null;
		this.treeListColumnEx1.CurrencyField = null;
		this.treeListColumnEx1.CurrencyFixed = null;
		this.treeListColumnEx1.CurrencyFixedRate = 0.0;
		this.treeListColumnEx1.CurrencyRateField = null;
		this.treeListColumnEx1.FieldName = "Tile Item";
		this.treeListColumnEx1.Name = "treeListColumnEx1";
		this.treeListColumnEx1.Visible = true;
		this.treeListColumnEx1.VisibleIndex = 0;
		this.treeListColumnEx2.Caption = "Display Text";
		this.treeListColumnEx2.CurrencyDateField = null;
		this.treeListColumnEx2.CurrencyField = null;
		this.treeListColumnEx2.CurrencyFixed = null;
		this.treeListColumnEx2.CurrencyFixedRate = 0.0;
		this.treeListColumnEx2.CurrencyRateField = null;
		this.treeListColumnEx2.FieldName = "Display Text";
		this.treeListColumnEx2.Name = "treeListColumnEx2";
		this.treeListColumnEx2.Visible = true;
		this.treeListColumnEx2.VisibleIndex = 1;
		this.treeListColumnEx3.Caption = "Visible";
		this.treeListColumnEx3.CurrencyDateField = null;
		this.treeListColumnEx3.CurrencyField = null;
		this.treeListColumnEx3.CurrencyFixed = null;
		this.treeListColumnEx3.CurrencyFixedRate = 0.0;
		this.treeListColumnEx3.CurrencyRateField = null;
		this.treeListColumnEx3.FieldName = "Visible";
		this.treeListColumnEx3.Name = "treeListColumnEx3";
		this.treeListColumnEx3.Visible = true;
		this.treeListColumnEx3.VisibleIndex = 2;
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(81, 388);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 20);
		this.btn_Cancel.TabIndex = 1;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(3, 388);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 20);
		this.btn_OK.TabIndex = 2;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.gridViewEx2.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gridViewEx2.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gridViewEx2.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridViewEx2.ColumnKey = null;
		this.gridViewEx2.DisableFormSetting = false;
		this.gridViewEx2.DisplayException = false;
		this.gridViewEx2.EditColumn = null;
		this.gridViewEx2.FieldCurrency = null;
		this.gridViewEx2.FieldRate = null;
		this.gridViewEx2.FormId = null;
		this.gridViewEx2.IgnoreFilter = false;
		this.gridViewEx2.IndexEditRow = -1;
		this.gridViewEx2.IndicatorWidth = 35;
		this.gridViewEx2.IsApzTable = false;
		this.gridViewEx2.IsInsertNewRow = false;
		this.gridViewEx2.IsLoadingData = false;
		this.gridViewEx2.IsMappingContextAction = false;
		this.gridViewEx2.ItemId = null;
		this.gridViewEx2.LstColumnAllowBatch = null;
		this.gridViewEx2.LstColumnNotAllowBatch = null;
		this.gridViewEx2.LstColumnNotEditable = null;
		this.gridViewEx2.LstColumNotSetting = null;
		this.gridViewEx2.Name = "gridViewEx2";
		this.gridViewEx2.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gridViewEx2.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gridViewEx2.OptionsPrint.AutoWidth = false;
		this.gridViewEx2.OptionsPrint.UsePrintStyles = false;
		this.gridViewEx2.OptionsView.BestFitMaxRowCount = 100;
		this.gridViewEx2.OptionsView.ColumnAutoWidth = false;
		this.gridViewEx2.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridViewEx2.OptionsView.ShowGroupedColumns = true;
		this.gridViewEx2.OptionsView.ShowGroupPanel = false;
		this.gridViewEx2.UseBatchMod = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(584, 412);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.panelControl1);
		base.FormId = "oWorkingProcessSetting";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_WorkingProcessSetting";
		this.Text = "Working Process Setting";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_WorkingProcessSetting_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tab_Functions).EndInit();
		this.tab_Functions.ResumeLayout(false);
		this.tabPage_Process.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Process).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Process).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Visible1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemCheck).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx4).EndInit();
		this.tabPage_Document.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Document).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Document).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Visible2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx3).EndInit();
		this.tabPage_Tooltips.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Tooltip).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Tooltip).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Columns).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_VisibleOrder).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).EndInit();
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridViewEx2).EndInit();
		base.ResumeLayout(false);
	}
}
