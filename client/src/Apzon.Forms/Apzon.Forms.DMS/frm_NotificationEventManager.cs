using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using DevExpress.Export;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.DMS;

public class frm_NotificationEventManager : XtraFormEx
{
	private enum saveType
	{
		Add = 1,
		Update,
		View
	}

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private const string OkString = "OK";

	private const string UpdateString = "Update";

	private const string AddString = "Add";

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private SimpleButtonEx btn_cancel;

	private SimpleButtonEx btn_ok;

	private GridControlEx grd_Parent;

	private GridViewEx gv_Parent;

	private GridControlEx grd_Child;

	private GridViewEx gv_Child;

	private GridColumnEx gc_AppName;

	private GridColumnEx gc_EventCode;

	private GridColumnEx gc_EventName;

	private GridColumnEx gc_Message;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private GridColumnEx gc_Title;

	private GridColumnEx gc_Active;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addToolStripMenuItem;

	private ToolStripMenuItem refreshToolStripMenuItem;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
			{
				WinFuntions.DisableAllControl(Controls);
				btn_cancel.Enabled = true;
				return;
			}
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
				btn_ok.Text = "Add";
				break;
			case Enums.FormViewModeType.Update:
				btn_ok.Text = "Update";
				break;
			case Enums.FormViewModeType.View:
				btn_ok.Text = "OK";
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_NotificationEventManager()
	{
		InitializeComponent();
		OnLoadAsync += frm_NotificationEventManager_Load;
	}

	private async Task frm_NotificationEventManager_Load(object sender, EventArgs e)
	{
		try
		{
			await RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task RefreshData()
	{
		_ = 1;
		try
		{
			RefreshStatus();
			await LoadData();
			await LoadDataLine();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void RefreshStatus()
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadData()
	{
		try
		{
			DataTable dataTable = new DataTable();
			using (HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Notification"))
			{
				dataTable = await http.GetAsync("GetListApp");
			}
			if (dataTable.IsNotNull())
			{
				grd_Parent.DataSource = dataTable;
				grd_Parent.RefreshDataSource();
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadDataLine()
	{
		try
		{
			string appName = Function.ToString(gv_Parent.GetFocusedRowCellValue(gc_AppName));
			DataTable dataTable = new DataTable();
			using (HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Notification"))
			{
				dataTable = await http.GetAsync("GetEventSetting", new { appName });
			}
			if (dataTable.IsNotNull())
			{
				grd_Child.DataSource = dataTable;
				grd_Child.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_Parent_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			await LoadDataLine();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Child_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
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

	private async void btn_ok_Click(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (FormMode == Enums.FormViewModeType.Add)
			{
				WinCommons._OpenProcessing();
				DataTable dataTable = ((DataRowView)gv_Child.GetFocusedRow()).Row.Table.Clone();
				DataRow row = ((DataRowView)gv_Child.GetFocusedRow()).Row;
				if (string.IsNullOrEmpty(Function.ToString(row["EventCode"])))
				{
					WinCommons.Error(Enums.MessageCode.None, "EventCode is empty");
					return;
				}
				row["Active"] = ((Function.ToString(row["Active"]) == "Y") ? "Y" : "N");
				dataTable.Rows.Add(row.ItemArray);
				string text = "";
				using (HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "Notification"))
				{
					text = await http.PostAsync("AddNotificationEvent", dataTable);
				}
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					RefreshStatus();
					await LoadDataLine();
				}
				WinCommons._CloseProcessing();
			}
			else
			{
				WinCommons._OpenProcessing();
				DataTable dataTable2 = ((DataRowView)gv_Child.GetFocusedRow()).Row.Table.Clone();
				DataRow row2 = ((DataRowView)gv_Child.GetFocusedRow()).Row;
				row2["Active"] = ((Function.ToString(row2["Active"]) == "Y") ? "Y" : "N");
				row2["AppName"] = Function.ToString(gv_Parent.GetFocusedRowCellValue(gc_AppName));
				dataTable2.Rows.Add(row2.ItemArray);
				dataTable2.AcceptChanges();
				string text2 = "";
				using (HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "Notification"))
				{
					text2 = await http.PostAsync("UpdateNotificationEvent", dataTable2);
				}
				if (!string.IsNullOrEmpty(text2))
				{
					WinCommons.Error(Enums.MessageCode.None, text2);
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					RefreshStatus();
					await LoadDataLine();
				}
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_Child_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				e.Allow = false;
				DialogResult dialogResult = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
				if (dialogResult == DialogResult.Yes && dialogResult == DialogResult.Yes)
				{
					RefreshStatus();
					await LoadDataLine();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_cancel_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
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

	private async void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add)
			{
				RefreshStatus();
				await LoadDataLine();
				return;
			}
			FormMode = Enums.FormViewModeType.Add;
			DataRow dataRow = grd_Child.DatasourceTable().NewRow();
			dataRow["Active"] = "Y";
			dataRow["Message"] = "{Message}";
			dataRow["Title"] = "Thông báo";
			dataRow["AppUse"] = Function.ToString(gv_Parent.GetFocusedRowCellValue(gc_AppName));
			dataRow["AppName"] = Function.ToString(gv_Parent.GetFocusedRowCellValue(gc_AppName));
			DataTable dataTable = dataRow.Table.Clone();
			dataTable.Rows.Add(dataRow.ItemArray);
			grd_Child.DataSource = dataTable;
			grd_Child.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void refreshToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			await RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void contextMenuStrip1_Opened(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add)
			{
				addToolStripMenuItem.Text = "Cancel";
			}
			else
			{
				addToolStripMenuItem.Text = "Add";
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.DMS.frm_NotificationEventManager));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Parent = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Parent = new Apzon.Controls.Grid.GridViewEx();
		this.gc_AppName = new Apzon.Controls.Grid.GridColumnEx();
		this.grd_Child = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Child = new Apzon.Controls.Grid.GridViewEx();
		this.gc_EventCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_EventName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Message = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gc_Title = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Active = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.btn_cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_ok = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Parent).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Parent).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Child).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Child).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 1);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Parent);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Child);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(801, 429);
		this.splitContainerControl1.SplitterPosition = 120;
		this.splitContainerControl1.TabIndex = 0;
		this.grd_Parent.AllowPasteExcel = false;
		this.grd_Parent.ClipboardData = null;
		this.grd_Parent.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Parent.Location = new System.Drawing.Point(0, 0);
		this.grd_Parent.MainView = this.gv_Parent;
		this.grd_Parent.Name = "grd_Parent";
		this.grd_Parent.Size = new System.Drawing.Size(801, 120);
		this.grd_Parent.TabIndex = 0;
		this.grd_Parent.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Parent });
		this.gv_Parent.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Parent.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Parent.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Parent.ColumnKey = null;
		this.gv_Parent.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[1] { this.gc_AppName });
		this.gv_Parent.DisableFormSetting = false;
		this.gv_Parent.DisplayException = false;
		this.gv_Parent.EditColumn = null;
		this.gv_Parent.FieldCurrency = null;
		this.gv_Parent.FieldRate = null;
		this.gv_Parent.FormId = null;
		this.gv_Parent.GridControl = this.grd_Parent;
		this.gv_Parent.IgnoreFilter = false;
		this.gv_Parent.IndexEditRow = -1;
		this.gv_Parent.IndicatorWidth = 35;
		this.gv_Parent.IsApzTable = false;
		this.gv_Parent.IsInsertNewRow = false;
		this.gv_Parent.IsLoadingData = false;
		this.gv_Parent.IsMappingContextAction = false;
		this.gv_Parent.ItemId = "APZ_D_OAPC";
		this.gv_Parent.LstColumnAllowBatch = null;
		this.gv_Parent.LstColumnNotAllowBatch = null;
		this.gv_Parent.LstColumnNotEditable = null;
		this.gv_Parent.LstColumNotSetting = null;
		this.gv_Parent.Name = "gv_Parent";
		this.gv_Parent.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Parent.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Parent.OptionsClipboard.ClipboardMode = DevExpress.Export.ClipboardMode.Formatted;
		this.gv_Parent.OptionsPrint.AutoWidth = false;
		this.gv_Parent.OptionsPrint.UsePrintStyles = false;
		this.gv_Parent.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Parent.OptionsView.ColumnAutoWidth = false;
		this.gv_Parent.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Parent.OptionsView.ShowGroupedColumns = true;
		this.gv_Parent.OptionsView.ShowGroupPanel = false;
		this.gv_Parent.UseBatchMod = false;
		this.gv_Parent.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Parent_FocusedRowChanged);
		this.gc_AppName.Caption = "App Name";
		this.gc_AppName.CurrencyDateField = null;
		this.gc_AppName.CurrencyField = null;
		this.gc_AppName.CurrencyFixed = null;
		this.gc_AppName.CurrencyFixedRate = 0.0;
		this.gc_AppName.CurrencyRateField = null;
		this.gc_AppName.FieldName = "AppName";
		this.gc_AppName.Name = "gc_AppName";
		this.gc_AppName.OptionsColumn.ReadOnly = true;
		this.gc_AppName.PrimaryKey = false;
		this.gc_AppName.UnitField = null;
		this.gc_AppName.Visible = true;
		this.gc_AppName.VisibleIndex = 0;
		this.grd_Child.AllowPasteExcel = true;
		this.grd_Child.ClipboardData = null;
		this.grd_Child.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Child.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Child.Location = new System.Drawing.Point(0, 0);
		this.grd_Child.MainView = this.gv_Child;
		this.grd_Child.Name = "grd_Child";
		this.grd_Child.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemCheckEdit1, this.repositoryItemMemoExEdit1 });
		this.grd_Child.Size = new System.Drawing.Size(801, 299);
		this.grd_Child.TabIndex = 0;
		this.grd_Child.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Child });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addToolStripMenuItem, this.refreshToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(114, 48);
		this.contextMenuStrip1.Opened += new System.EventHandler(this.contextMenuStrip1_Opened);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
		this.addToolStripMenuItem.Text = "Add";
		this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
		this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
		this.refreshToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
		this.refreshToolStripMenuItem.Text = "Refresh";
		this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
		this.gv_Child.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Child.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Child.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Child.ColumnKey = null;
		this.gv_Child.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gc_EventCode, this.gc_EventName, this.gc_Message, this.gc_Title, this.gc_Active });
		this.gv_Child.DisableFormSetting = false;
		this.gv_Child.DisplayException = false;
		this.gv_Child.EditColumn = null;
		this.gv_Child.FieldCurrency = null;
		this.gv_Child.FieldRate = null;
		this.gv_Child.FormId = null;
		this.gv_Child.GridControl = this.grd_Child;
		this.gv_Child.IgnoreFilter = false;
		this.gv_Child.IndexEditRow = -1;
		this.gv_Child.IndicatorWidth = 35;
		this.gv_Child.IsApzTable = false;
		this.gv_Child.IsInsertNewRow = false;
		this.gv_Child.IsLoadingData = false;
		this.gv_Child.IsMappingContextAction = false;
		this.gv_Child.ItemId = "APZ_D_EVENTCONFIG";
		this.gv_Child.LstColumnAllowBatch = null;
		this.gv_Child.LstColumnNotAllowBatch = null;
		this.gv_Child.LstColumnNotEditable = null;
		this.gv_Child.LstColumNotSetting = null;
		this.gv_Child.Name = "gv_Child";
		this.gv_Child.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Child.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Child.OptionsClipboard.ClipboardMode = DevExpress.Export.ClipboardMode.Formatted;
		this.gv_Child.OptionsPrint.AutoWidth = false;
		this.gv_Child.OptionsPrint.UsePrintStyles = false;
		this.gv_Child.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Child.OptionsView.ColumnAutoWidth = false;
		this.gv_Child.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Child.OptionsView.ShowGroupedColumns = true;
		this.gv_Child.OptionsView.ShowGroupPanel = false;
		this.gv_Child.UseBatchMod = false;
		this.gv_Child.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Child_CellValueChanged);
		this.gv_Child.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Child_BeforeLeaveRow);
		this.gc_EventCode.Caption = "Event Code";
		this.gc_EventCode.CurrencyDateField = null;
		this.gc_EventCode.CurrencyField = null;
		this.gc_EventCode.CurrencyFixed = null;
		this.gc_EventCode.CurrencyFixedRate = 0.0;
		this.gc_EventCode.CurrencyRateField = null;
		this.gc_EventCode.FieldName = "EventCode";
		this.gc_EventCode.Name = "gc_EventCode";
		this.gc_EventCode.PrimaryKey = true;
		this.gc_EventCode.UnitField = null;
		this.gc_EventCode.Visible = true;
		this.gc_EventCode.VisibleIndex = 0;
		this.gc_EventName.Caption = "Event Name";
		this.gc_EventName.CurrencyDateField = null;
		this.gc_EventName.CurrencyField = null;
		this.gc_EventName.CurrencyFixed = null;
		this.gc_EventName.CurrencyFixedRate = 0.0;
		this.gc_EventName.CurrencyRateField = null;
		this.gc_EventName.FieldName = "EventName";
		this.gc_EventName.Name = "gc_EventName";
		this.gc_EventName.PrimaryKey = false;
		this.gc_EventName.UnitField = null;
		this.gc_EventName.Visible = true;
		this.gc_EventName.VisibleIndex = 1;
		this.gc_Message.Caption = "Message";
		this.gc_Message.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.gc_Message.CurrencyDateField = null;
		this.gc_Message.CurrencyField = null;
		this.gc_Message.CurrencyFixed = null;
		this.gc_Message.CurrencyFixedRate = 0.0;
		this.gc_Message.CurrencyRateField = null;
		this.gc_Message.FieldName = "Message";
		this.gc_Message.Name = "gc_Message";
		this.gc_Message.PrimaryKey = false;
		this.gc_Message.UnitField = null;
		this.gc_Message.Visible = true;
		this.gc_Message.VisibleIndex = 2;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.gc_Title.Caption = "Title";
		this.gc_Title.CurrencyDateField = null;
		this.gc_Title.CurrencyField = null;
		this.gc_Title.CurrencyFixed = null;
		this.gc_Title.CurrencyFixedRate = 0.0;
		this.gc_Title.CurrencyRateField = null;
		this.gc_Title.FieldName = "Title";
		this.gc_Title.Name = "gc_Title";
		this.gc_Title.PrimaryKey = false;
		this.gc_Title.UnitField = null;
		this.gc_Title.Visible = true;
		this.gc_Title.VisibleIndex = 3;
		this.gc_Active.Caption = "Active";
		this.gc_Active.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gc_Active.CurrencyDateField = null;
		this.gc_Active.CurrencyField = null;
		this.gc_Active.CurrencyFixed = null;
		this.gc_Active.CurrencyFixedRate = 0.0;
		this.gc_Active.CurrencyRateField = null;
		this.gc_Active.FieldName = "Active";
		this.gc_Active.Name = "gc_Active";
		this.gc_Active.PrimaryKey = false;
		this.gc_Active.UnitField = null;
		this.gc_Active.Visible = true;
		this.gc_Active.VisibleIndex = 4;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.btn_cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_cancel.EditControl = null;
		this.btn_cancel.EditValue = "Cancel";
		this.btn_cancel.FieldName = null;
		this.btn_cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_cancel.ImageOptions.Image");
		this.btn_cancel.IsSearchControl = false;
		this.btn_cancel.Location = new System.Drawing.Point(713, 444);
		this.btn_cancel.Name = "btn_cancel";
		this.btn_cancel.ReadOnly = false;
		this.btn_cancel.SettingInVisible = false;
		this.btn_cancel.SettingReadonly = false;
		this.btn_cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_cancel.TabIndex = 13;
		this.btn_cancel.Text = "Cancel";
		this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
		this.btn_ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_ok.EditControl = null;
		this.btn_ok.EditValue = "OK";
		this.btn_ok.FieldName = null;
		this.btn_ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ok.ImageOptions.Image");
		this.btn_ok.IsSearchControl = false;
		this.btn_ok.Location = new System.Drawing.Point(632, 444);
		this.btn_ok.Name = "btn_ok";
		this.btn_ok.ReadOnly = false;
		this.btn_ok.SettingInVisible = false;
		this.btn_ok.SettingReadonly = false;
		this.btn_ok.Size = new System.Drawing.Size(75, 23);
		this.btn_ok.TabIndex = 14;
		this.btn_ok.Text = "OK";
		this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(800, 479);
		base.Controls.Add(this.btn_cancel);
		base.Controls.Add(this.btn_ok);
		base.Controls.Add(this.splitContainerControl1);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_NotificationEventManager";
		this.Text = "Notification Event Manager";
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Parent).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Parent).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Child).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Child).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		base.ResumeLayout(false);
	}
}
