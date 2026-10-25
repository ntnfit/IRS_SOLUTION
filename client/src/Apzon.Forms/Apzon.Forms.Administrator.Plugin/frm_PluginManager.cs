using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using Apzon.Plugin;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;

namespace Apzon.Forms.Administrator.Plugin;

public class frm_PluginManager : XtraFormEx
{
	public BarSubItem MenuBar;

	public TreeList TreeMenu;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public CreateMainMenu CreateMainMenu;

	private IContainer components;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx xtraTabPage1;

	private GridControlEx grd_installed;

	private GridViewEx gv_installed;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Start;

	private SimpleButtonEx btn_Stop;

	private GridColumnEx gcPlugId;

	private GridColumnEx gcName;

	private GridColumnEx gcStatus;

	private GridColumnEx gcVersion;

	private GridColumnEx gcPlatform;

	private RepositoryItemLookUpEdit rep_Status;

	private GridColumnEx gcGroup;

	private RepositoryItemLookUpEdit rGroup;

	private ContextMenuStrip contextMenuStrip;

	private ToolStripMenuItem resigterPluginToolStripMenuItem;

	private ToolStripMenuItem removePluginToolStripMenuItem;

	public XtraFormEx MainForm { get; set; }

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

	public frm_PluginManager()
	{
		InitializeComponent();
	}

	private async void frm_PluginManager_Load(object sender, EventArgs e)
	{
		try
		{
			rep_Status.SetDataSource(new Dictionary<string, string>
			{
				{ "D", "Disconnected" },
				{ "C", "Connected" },
				{ "P", "Pending" }
			});
			rGroup.DataSource = new Dictionary<string, string>
			{
				{ "M", "Manual" },
				{ "A", "Automatic" },
				{ "D", "Disabled" }
			};
			MainForm = ParentForm as XtraFormEx;
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Start_Click(object sender, EventArgs e)
	{
		SimpleButtonEx simpleButtonEx = btn_OK;
		SimpleButtonEx simpleButtonEx2 = btn_Start;
		bool flag = (btn_Stop.Enabled = false);
		bool enabled = (simpleButtonEx2.Enabled = flag);
		simpleButtonEx.Enabled = enabled;
		WinCommons._OpenProcessing();
		try
		{
			string text = Function.ToString(gv_installed.GetFocusedRowCellValue("Name"));
			if (text != null)
			{
				PluginManagement pluginManagement = new PluginManagement();
				pluginManagement.CreateMainMenu = CreateMainMenu;
				new PluginLoader().LoadPlugins(pluginManagement, $"{GlobalData.DatabaseName}\\{text}");
				foreach (IPluginApplication pluginApplication in pluginManagement.PluginApplications)
				{
					pluginApplication.MainForm = MainForm;
					pluginApplication.HttpClient = _httpClient;
					pluginApplication.Initialize(Function.ParseInt(gv_installed.GetFocusedRowCellValue("PlugId")));
				}
				gv_installed.SetFocusedRowCellValue("Status", "C");
				btn_Start.Enabled = false;
				SimpleButtonEx simpleButtonEx3 = btn_Stop;
				enabled = (btn_OK.Enabled = true);
				simpleButtonEx3.Enabled = enabled;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			SimpleButtonEx simpleButtonEx4 = btn_OK;
			enabled = (btn_Start.Enabled = true);
			simpleButtonEx4.Enabled = enabled;
			btn_Stop.Enabled = false;
		}
		WinCommons._CloseProcessing();
	}

	private void gv_installed_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_installed.GetFocusedRowCellValue("Status"));
			string text2 = Function.ToString(gv_installed.GetFocusedRowCellValue("Group"));
			btn_Start.Enabled = text == "D" || text2.Equals("D");
			btn_Stop.Enabled = text == "C";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Stop_Click(object sender, EventArgs e)
	{
		try
		{
			if (CheckStartPlugin(MenuBar, Function.ParseInt(gv_installed.GetFocusedRowCellValue("PlugId")), stop: true))
			{
				WinCommons.Success(Enums.MessageCode.PluginDisconnected, "");
				gv_installed.SetFocusedRowCellValue("Status", "D");
				grd_installed.DataSourceTable.AcceptChanges();
				gv_installed_FocusedRowObjectChanged(null, null);
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.CannotDisconnectPlugin, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_installed_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		if (e.Column == gcGroup)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				await UpdatePluginInfomation();
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

	private async void resigterPluginToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			frm_RegisterPlugin frm_RegisterPlugin2 = new frm_RegisterPlugin();
			frm_RegisterPlugin2.ShowDialog(this);
			if (frm_RegisterPlugin2.DialogResult == DialogResult.OK)
			{
				await LoadData();
				gv_installed_FocusedRowObjectChanged(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void removePluginToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			string text = Function.ToString(gv_installed.GetFocusedRowCellValue("Name"));
			string text2 = $"{Environment.CurrentDirectory}\\Extensions\\{GlobalData.DatabaseName}\\{text}";
			if (Directory.Exists(text2))
			{
				Function.DeleteFolder(text2);
			}
			string text3 = await _httpClient.PluginManagerClient.Delete(text);
			if (string.IsNullOrEmpty(text3))
			{
				await LoadData();
				WinCommons.Success(Enums.MessageCode.RemoveSuccess, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text3);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadData()
	{
		try
		{
			DataTable dataTable = await _httpClient.PluginManagerClient.GetData(GlobalData.DatabaseName);
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				int id = Function.ParseInt(dataTable.Rows[i]["PlugId"]);
				if (CheckStartPlugin(MenuBar, id))
				{
					dataTable.Rows[i]["Status"] = "C";
				}
			}
			if (!dataTable.IsNotNull())
			{
				SimpleButtonEx simpleButtonEx = btn_Start;
				bool enabled = (btn_Stop.Enabled = false);
				simpleButtonEx.Enabled = enabled;
			}
			grd_installed.DataSource = dataTable;
			grd_installed.DataSourceTable.AcceptChanges();
			removePluginToolStripMenuItem.Enabled = dataTable.IsNotNull();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
	}

	private bool CheckStartPlugin(BarSubItem bar, int id, bool stop = false)
	{
		bool result = false;
		for (int i = 0; i < bar.ItemLinks.Count; i++)
		{
			BarItemLink barItemLink = bar.ItemLinks[i];
			if (barItemLink.GetType() == typeof(BarSubItemLink))
			{
				if (barItemLink is BarSubItemLink { Item: not null, Item: BarSubItem item } && item.ItemLinks.Count > 0 && CheckStartPlugin(item, id, stop))
				{
					result = true;
					if (!stop)
					{
						return true;
					}
				}
			}
			else
			{
				if (!(barItemLink is BarButtonItemLink { Item: not null } barButtonItemLink))
				{
					continue;
				}
				string[] array = barButtonItemLink.Item.Name.Split('@');
				if (array.Length > 1 && Function.ParseInt(array[array.Length - 1]) == id)
				{
					if (!stop)
					{
						return true;
					}
					result = true;
					bar.RemoveLink(barButtonItemLink);
					i--;
				}
			}
		}
		if (stop && TreeMenu != null)
		{
			DataTable dataTable = TreeMenu.DataSourceTable();
			if (dataTable.IsNotNull())
			{
				List<DataRow> list = (from t in dataTable.AsEnumerable()
					where Function.ToString(t["Code"]).Split('@').Length > 1 && Function.ParseInt(Function.ToString(t["Code"]).Split('@')[1]) == id
					select t).ToList();
				for (int num = 0; num < list.Count; num++)
				{
					dataTable.Rows.Remove(list[num]);
				}
			}
			dataTable.AcceptChanges();
			TreeMenu.Refresh();
		}
		return result;
	}

	private async Task UpdatePluginInfomation()
	{
		DataTable dataTable = grd_installed.DataSourceTable.Clone();
		dataTable.ImportRow(gv_installed.GetFocusedDataRow());
		dataTable.AcceptChanges();
		string text = await _httpClient.PluginManagerClient.Save(dataTable);
		if (string.IsNullOrEmpty(text))
		{
			await LoadData();
			WinCommons.Success(Enums.MessageCode.UpdateSuccess, "");
		}
		else
		{
			WinCommons.Error(Enums.MessageCode.None, text);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.Plugin.frm_PluginManager));
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_installed = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.resigterPluginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removePluginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_installed = new Apzon.Controls.Grid.GridViewEx();
		this.gcPlugId = new Apzon.Controls.Grid.GridColumnEx();
		this.gcName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcVersion = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPlatform = new Apzon.Controls.Grid.GridColumnEx();
		this.gcGroup = new Apzon.Controls.Grid.GridColumnEx();
		this.rGroup = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Start = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Stop = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_installed).BeginInit();
		this.contextMenuStrip.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_installed).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rGroup).BeginInit();
		base.SuspendLayout();
		this.xtraTabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(1, 1);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControl1.Size = new System.Drawing.Size(750, 466);
		this.xtraTabControl1.TabIndex = 0;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.xtraTabPage1 });
		this.xtraTabPage1.Controls.Add(this.grd_installed);
		this.xtraTabPage1.FieldName = null;
		this.xtraTabPage1.IsSearchControl = false;
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.Size = new System.Drawing.Size(744, 438);
		this.xtraTabPage1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage1.Text = "Installed Plugin";
		this.grd_installed.AllowPasteExcel = false;
		this.grd_installed.ClipboardData = null;
		this.grd_installed.ContextMenuStrip = this.contextMenuStrip;
		this.grd_installed.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_installed.Location = new System.Drawing.Point(0, 0);
		this.grd_installed.MainView = this.gv_installed;
		this.grd_installed.Name = "grd_installed";
		this.grd_installed.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_Status, this.rGroup });
		this.grd_installed.Size = new System.Drawing.Size(744, 438);
		this.grd_installed.TabIndex = 0;
		this.grd_installed.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_installed });
		this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.resigterPluginToolStripMenuItem, this.removePluginToolStripMenuItem });
		this.contextMenuStrip.Name = "contextMenuStrip";
		this.contextMenuStrip.Size = new System.Drawing.Size(155, 48);
		this.resigterPluginToolStripMenuItem.Name = "resigterPluginToolStripMenuItem";
		this.resigterPluginToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
		this.resigterPluginToolStripMenuItem.Text = "Register Plugin";
		this.resigterPluginToolStripMenuItem.Click += new System.EventHandler(this.resigterPluginToolStripMenuItem_Click);
		this.removePluginToolStripMenuItem.Name = "removePluginToolStripMenuItem";
		this.removePluginToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
		this.removePluginToolStripMenuItem.Text = "Remove Plugin";
		this.removePluginToolStripMenuItem.Click += new System.EventHandler(this.removePluginToolStripMenuItem_Click);
		this.gv_installed.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_installed.ColumnKey = null;
		this.gv_installed.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gcPlugId, this.gcName, this.gcStatus, this.gcVersion, this.gcPlatform, this.gcGroup });
		this.gv_installed.DisableFormSetting = false;
		this.gv_installed.DisplayException = false;
		this.gv_installed.EditColumn = null;
		this.gv_installed.FieldCurrency = null;
		this.gv_installed.FieldRate = null;
		this.gv_installed.FormId = "oPlugin";
		this.gv_installed.GridControl = this.grd_installed;
		this.gv_installed.IndexEditRow = -1;
		this.gv_installed.IndicatorWidth = 35;
		this.gv_installed.IsApzTable = false;
		this.gv_installed.IsInsertNewRow = false;
		this.gv_installed.IsLoadingData = false;
		this.gv_installed.ItemId = "OPLUGINITEM";
		this.gv_installed.LstColumnAllowBatch = null;
		this.gv_installed.LstColumnNotAllowBatch = null;
		this.gv_installed.LstColumNotSetting = null;
		this.gv_installed.Name = "gv_installed";
		this.gv_installed.OptionsView.ShowGroupPanel = false;
		this.gv_installed.UseBatchMod = false;
		this.gv_installed.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_installed_FocusedRowObjectChanged);
		this.gv_installed.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_installed_CellValueChanging);
		this.gcPlugId.Caption = "Plugin Id";
		this.gcPlugId.CurrencyDateField = null;
		this.gcPlugId.CurrencyField = null;
		this.gcPlugId.CurrencyFixed = null;
		this.gcPlugId.CurrencyFixedRate = 0.0;
		this.gcPlugId.CurrencyRateField = null;
		this.gcPlugId.FieldName = "PlugId";
		this.gcPlugId.Name = "gcPlugId";
		this.gcPlugId.OptionsColumn.ReadOnly = true;
		this.gcPlugId.PrimaryKey = false;
		this.gcPlugId.UnitField = null;
		this.gcPlugId.Visible = true;
		this.gcPlugId.VisibleIndex = 0;
		this.gcName.Caption = "Name";
		this.gcName.CurrencyDateField = null;
		this.gcName.CurrencyField = null;
		this.gcName.CurrencyFixed = null;
		this.gcName.CurrencyFixedRate = 0.0;
		this.gcName.CurrencyRateField = null;
		this.gcName.FieldName = "Name";
		this.gcName.Name = "gcName";
		this.gcName.OptionsColumn.ReadOnly = true;
		this.gcName.PrimaryKey = false;
		this.gcName.UnitField = null;
		this.gcName.Visible = true;
		this.gcName.VisibleIndex = 1;
		this.gcStatus.Caption = "Status";
		this.gcStatus.ColumnEdit = this.rep_Status;
		this.gcStatus.CurrencyDateField = null;
		this.gcStatus.CurrencyField = null;
		this.gcStatus.CurrencyFixed = null;
		this.gcStatus.CurrencyFixedRate = 0.0;
		this.gcStatus.CurrencyRateField = null;
		this.gcStatus.FieldName = "Status";
		this.gcStatus.Name = "gcStatus";
		this.gcStatus.OptionsColumn.ReadOnly = true;
		this.gcStatus.PrimaryKey = false;
		this.gcStatus.UnitField = null;
		this.gcStatus.Visible = true;
		this.gcStatus.VisibleIndex = 2;
		this.rep_Status.AutoHeight = false;
		this.rep_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Status.Name = "rep_Status";
		this.rep_Status.NullText = "";
		this.gcVersion.Caption = "Version";
		this.gcVersion.CurrencyDateField = null;
		this.gcVersion.CurrencyField = null;
		this.gcVersion.CurrencyFixed = null;
		this.gcVersion.CurrencyFixedRate = 0.0;
		this.gcVersion.CurrencyRateField = null;
		this.gcVersion.FieldName = "Version";
		this.gcVersion.Name = "gcVersion";
		this.gcVersion.OptionsColumn.ReadOnly = true;
		this.gcVersion.PrimaryKey = false;
		this.gcVersion.UnitField = null;
		this.gcVersion.Visible = true;
		this.gcVersion.VisibleIndex = 3;
		this.gcPlatform.Caption = "Platform";
		this.gcPlatform.CurrencyDateField = null;
		this.gcPlatform.CurrencyField = null;
		this.gcPlatform.CurrencyFixed = null;
		this.gcPlatform.CurrencyFixedRate = 0.0;
		this.gcPlatform.CurrencyRateField = null;
		this.gcPlatform.FieldName = "Platform";
		this.gcPlatform.Name = "gcPlatform";
		this.gcPlatform.OptionsColumn.ReadOnly = true;
		this.gcPlatform.PrimaryKey = false;
		this.gcPlatform.UnitField = null;
		this.gcPlatform.Visible = true;
		this.gcPlatform.VisibleIndex = 4;
		this.gcGroup.Caption = "Group";
		this.gcGroup.ColumnEdit = this.rGroup;
		this.gcGroup.CurrencyDateField = null;
		this.gcGroup.CurrencyField = null;
		this.gcGroup.CurrencyFixed = null;
		this.gcGroup.CurrencyFixedRate = 0.0;
		this.gcGroup.CurrencyRateField = null;
		this.gcGroup.FieldName = "Group";
		this.gcGroup.Name = "gcGroup";
		this.gcGroup.PrimaryKey = false;
		this.gcGroup.UnitField = null;
		this.gcGroup.Visible = true;
		this.gcGroup.VisibleIndex = 5;
		this.rGroup.AutoHeight = false;
		this.rGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rGroup.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Key", "Name1", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name2")
		});
		this.rGroup.DisplayMember = "Value";
		this.rGroup.Name = "rGroup";
		this.rGroup.NullText = "";
		this.rGroup.ShowFooter = false;
		this.rGroup.ShowHeader = false;
		this.rGroup.ValueMember = "Key";
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(3, 470);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Start.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Start.FieldName = null;
		this.btn_Start.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Start.ImageOptions.Image");
		this.btn_Start.IsSearchControl = false;
		this.btn_Start.Location = new System.Drawing.Point(590, 470);
		this.btn_Start.Name = "btn_Start";
		this.btn_Start.Size = new System.Drawing.Size(75, 23);
		this.btn_Start.TabIndex = 2;
		this.btn_Start.Text = "Start";
		this.btn_Start.Click += new System.EventHandler(this.btn_Start_Click);
		this.btn_Stop.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Stop.FieldName = null;
		this.btn_Stop.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Stop.ImageOptions.Image");
		this.btn_Stop.IsSearchControl = false;
		this.btn_Stop.Location = new System.Drawing.Point(671, 470);
		this.btn_Stop.Name = "btn_Stop";
		this.btn_Stop.Size = new System.Drawing.Size(75, 23);
		this.btn_Stop.TabIndex = 3;
		this.btn_Stop.Text = "Stop";
		this.btn_Stop.Click += new System.EventHandler(this.btn_Stop_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(750, 498);
		base.Controls.Add(this.btn_Stop);
		base.Controls.Add(this.btn_Start);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.xtraTabControl1);
		base.FormId = "oPlugin";
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "frm_PluginManager";
		this.Text = "Plugins Manager";
		base.Load += new System.EventHandler(this.frm_PluginManager_Load);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_installed).EndInit();
		this.contextMenuStrip.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_installed).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rGroup).EndInit();
		base.ResumeLayout(false);
	}
}
