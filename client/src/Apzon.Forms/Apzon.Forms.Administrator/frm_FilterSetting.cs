using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator;

public class frm_FilterSetting : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GroupControl groupControl1;

	private GridControlEx grd_Category;

	private GridViewEx gv_Category;

	private GroupControl groupControl2;

	private GridControlEx grd_Filters;

	private GridViewEx gv_Filters;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx simpleButton1;

	private ContextMenuStrip ctx_Main;

	private ToolStripMenuItem addToolStripMenuItem;

	private ToolStripMenuItem editToolStripMenuItem1;

	private ToolStripMenuItem removeToolStripMenuItem;

	private SplitContainerControl splitContainerControl2;

	private GroupControl groupControl3;

	private GridControlEx grd_User;

	private GridViewEx gv_User;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btn_OK.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_FilterSetting()
	{
		InitializeComponent();
	}

	private void ctx_Main_Opening(object sender, CancelEventArgs e)
	{
	}

	private async void frm_FilterSetting_Load(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			WinCommons._OpenProcessing();
			DataSet ds = await _httpClient.FilterSetting.GetCategory();
			if (ds.IsHasStructure())
			{
				await GridViewHelper.GenerateGrid(grd_Category, gv_Category, ds.Tables[0], ds.Tables[1]);
			}
			if (gv_Category.FocusedRowHandle >= 0)
			{
				string cat = Function.ToString(gv_Category.GetFocusedRowCellValue("ObjType"));
				await GetFilterByCategory(cat);
			}
			DataSet dataSet = await _httpClient.FilterSetting.GetUsers();
			if (ds.IsHasStructure())
			{
				grd_User.DataSource = dataSet.Tables[1];
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async void gv_Category_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			string cat = Function.ToString(gv_Category.GetFocusedRowCellValue("ObjType"));
			await GetFilterByCategory(cat);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetFilterByCategory(string cat)
	{
		DataSet dataSet = await _httpClient.FilterSetting.GetFilter(cat);
		if (dataSet.IsHasStructure())
		{
			await GridViewHelper.GenerateGrid(grd_Filters, gv_Filters, dataSet.Tables[0], dataSet.Tables[1]);
			return;
		}
		grd_Filters.DataSource = null;
		grd_Filters.RefreshDataSource();
		gv_Filters.PopulateColumns();
	}

	private async void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			frm_UpdateFilter frm_UpdateFilter2 = new frm_UpdateFilter();
			frm_UpdateFilter2.Category = Function.ToString(gv_Category.GetFocusedRowCellValue("ObjType"));
			frm_UpdateFilter2.ShowDialog(this);
			if (gv_Category.FocusedRowHandle >= 0)
			{
				string cat = Function.ToString(gv_Category.GetFocusedRowCellValue("ObjType"));
				await GetFilterByCategory(cat);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_Filters_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			gv_User.ClearSelection();
			string code = Function.ToString(gv_Filters.GetFocusedRowCellValue("Code"));
			DataTable dataTable = await _httpClient.FilterSetting.GetUserByFilter(code);
			if (!dataTable.IsNotNull())
			{
				return;
			}
			List<string> list = new List<string>();
			list.AddRange(from t in dataTable.AsEnumerable()
				select Function.ToString(t["UserName"]));
			for (int num = 0; num < gv_User.RowCount; num++)
			{
				string item = Function.ToString(gv_User.GetRowCellValue(num, "USER_CODE"));
				if (list.Contains(item))
				{
					gv_User.SelectRow(num);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void simpleButton1_Click(object sender, EventArgs e)
	{
		Close();
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
				string text = Function.ToString(gv_Filters.GetFocusedRowCellValue("Code"));
				if (text == "")
				{
					WinCommons.Error(Enums.MessageCode.None, "Select a filter");
					return;
				}
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					return;
				}
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("ObjType", typeof(string));
				dataTable.Columns.Add("FilterCode", typeof(string));
				dataTable.Columns.Add("UserName", typeof(string));
				string value = Function.ToString(gv_Category.GetFocusedRowCellValue("ObjType"));
				int[] selectedRows = gv_User.GetSelectedRows();
				if (selectedRows.Length != 0)
				{
					for (int i = 0; i < selectedRows.Length; i++)
					{
						DataRow dataRow = dataTable.NewRow();
						dataRow["ObjType"] = value;
						dataRow["FilterCode"] = text;
						dataRow["UserName"] = Function.ToString(gv_User.GetRowCellValue(selectedRows[i], "USER_CODE"));
						dataTable.Rows.Add(dataRow);
					}
				}
				if (dataTable.IsNotNull())
				{
					WinCommons._OpenProcessing();
					string text2 = await _httpClient.FilterSetting.SaveUserSign(dataTable);
					if (!string.IsNullOrEmpty(text2))
					{
						WinCommons.Error(Enums.MessageCode.None, text2);
					}
					else
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						FormMode = Enums.FormViewModeType.View;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void gv_User_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
	}

	private async void editToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			frm_UpdateFilter frm_UpdateFilter2 = new frm_UpdateFilter();
			frm_UpdateFilter2.Category = Function.ToString(gv_Category.GetFocusedRowCellValue("ObjType"));
			frm_UpdateFilter2.Code = Function.ToString(gv_Filters.GetFocusedRowCellValue("Code"));
			frm_UpdateFilter2.ShowDialog(this);
			if (gv_Category.FocusedRowHandle >= 0)
			{
				string cat = Function.ToString(gv_Category.GetFocusedRowCellValue("ObjType"));
				await GetFilterByCategory(cat);
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
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			string code = Function.ToString(gv_Filters.GetFocusedRowCellValue("Code"));
			WinCommons._OpenProcessing();
			string text = await _httpClient.FilterSetting.Remove(code);
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
			else if (gv_Category.FocusedRowHandle >= 0)
			{
				string cat = Function.ToString(gv_Category.GetFocusedRowCellValue("ObjType"));
				await GetFilterByCategory(cat);
				WinCommons.Success(Enums.MessageCode.Success, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void frm_FilterSetting_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_FilterSetting));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.grd_Category = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Category = new Apzon.Controls.Grid.GridViewEx();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
		this.grd_Filters = new Apzon.Controls.Grid.GridControlEx();
		this.ctx_Main = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.editToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Filters = new Apzon.Controls.Grid.GridViewEx();
		this.groupControl3 = new DevExpress.XtraEditors.GroupControl();
		this.grd_User = new Apzon.Controls.Grid.GridControlEx();
		this.gv_User = new Apzon.Controls.Grid.GridViewEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton1 = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Category).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Category).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).BeginInit();
		this.groupControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Filters).BeginInit();
		this.ctx_Main.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Filters).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl3).BeginInit();
		this.groupControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_User).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_User).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.groupControl1);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.splitContainerControl2);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(983, 495);
		this.splitContainerControl1.SplitterPosition = 238;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.groupControl1.Controls.Add(this.grd_Category);
		this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl1.Location = new System.Drawing.Point(0, 0);
		this.groupControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(238, 495);
		this.groupControl1.TabIndex = 0;
		this.groupControl1.Text = "Catetory";
		this.grd_Category.AllowPasteExcel = true;
		this.grd_Category.ClipboardData = null;
		this.grd_Category.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Category.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_Category.Location = new System.Drawing.Point(2, 23);
		this.grd_Category.MainView = this.gv_Category;
		this.grd_Category.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_Category.Name = "grd_Category";
		this.grd_Category.Size = new System.Drawing.Size(234, 470);
		this.grd_Category.TabIndex = 0;
		this.grd_Category.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Category });
		this.gv_Category.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Category.ColumnKey = null;
		this.gv_Category.DisableFormSetting = false;
		this.gv_Category.DisplayException = false;
		this.gv_Category.EditColumn = null;
		this.gv_Category.FieldCurrency = null;
		this.gv_Category.FieldRate = null;
		this.gv_Category.FormId = "oFilterSetting";
		this.gv_Category.GridControl = this.grd_Category;
		this.gv_Category.IgnoreFilter = false;
		this.gv_Category.IndexEditRow = -1;
		this.gv_Category.IndicatorWidth = 35;
		this.gv_Category.IsApzTable = false;
		this.gv_Category.IsInsertNewRow = false;
		this.gv_Category.IsLoadingData = false;
		this.gv_Category.IsMappingContextAction = false;
		this.gv_Category.ItemId = "Catetory";
		this.gv_Category.LstColumnAllowBatch = null;
		this.gv_Category.LstColumnNotAllowBatch = null;
		this.gv_Category.LstColumnNotEditable = null;
		this.gv_Category.LstColumNotSetting = null;
		this.gv_Category.Name = "gv_Category";
		this.gv_Category.UseBatchMod = false;
		this.gv_Category.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Category_FocusedRowObjectChanged);
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.groupControl2);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.groupControl3);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(735, 495);
		this.splitContainerControl2.SplitterPosition = 479;
		this.splitContainerControl2.TabIndex = 2;
		this.splitContainerControl2.Text = "splitContainerControl2";
		this.groupControl2.Controls.Add(this.grd_Filters);
		this.groupControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl2.Location = new System.Drawing.Point(0, 0);
		this.groupControl2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.groupControl2.Name = "groupControl2";
		this.groupControl2.Size = new System.Drawing.Size(479, 495);
		this.groupControl2.TabIndex = 1;
		this.groupControl2.Text = "Filters";
		this.grd_Filters.AllowPasteExcel = true;
		this.grd_Filters.ClipboardData = null;
		this.grd_Filters.ContextMenuStrip = this.ctx_Main;
		this.grd_Filters.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Filters.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_Filters.Location = new System.Drawing.Point(2, 23);
		this.grd_Filters.MainView = this.gv_Filters;
		this.grd_Filters.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_Filters.Name = "grd_Filters";
		this.grd_Filters.Size = new System.Drawing.Size(475, 470);
		this.grd_Filters.TabIndex = 0;
		this.grd_Filters.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Filters });
		this.ctx_Main.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.ctx_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addToolStripMenuItem, this.editToolStripMenuItem1, this.removeToolStripMenuItem });
		this.ctx_Main.Name = "ctx_Main";
		this.ctx_Main.Size = new System.Drawing.Size(118, 70);
		this.ctx_Main.Opening += new System.ComponentModel.CancelEventHandler(this.ctx_Main_Opening);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.addToolStripMenuItem.Text = "Add";
		this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
		this.editToolStripMenuItem1.Name = "editToolStripMenuItem1";
		this.editToolStripMenuItem1.Size = new System.Drawing.Size(117, 22);
		this.editToolStripMenuItem1.Text = "Edit";
		this.editToolStripMenuItem1.Click += new System.EventHandler(this.editToolStripMenuItem1_Click);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_Filters.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Filters.ColumnKey = null;
		this.gv_Filters.DisableFormSetting = false;
		this.gv_Filters.DisplayException = false;
		this.gv_Filters.EditColumn = null;
		this.gv_Filters.FieldCurrency = null;
		this.gv_Filters.FieldRate = null;
		this.gv_Filters.FormId = "oFilterSetting";
		this.gv_Filters.GridControl = this.grd_Filters;
		this.gv_Filters.IgnoreFilter = false;
		this.gv_Filters.IndexEditRow = -1;
		this.gv_Filters.IndicatorWidth = 35;
		this.gv_Filters.IsApzTable = false;
		this.gv_Filters.IsInsertNewRow = false;
		this.gv_Filters.IsLoadingData = false;
		this.gv_Filters.IsMappingContextAction = false;
		this.gv_Filters.ItemId = "Filter";
		this.gv_Filters.LstColumnAllowBatch = null;
		this.gv_Filters.LstColumnNotAllowBatch = null;
		this.gv_Filters.LstColumnNotEditable = null;
		this.gv_Filters.LstColumNotSetting = null;
		this.gv_Filters.Name = "gv_Filters";
		this.gv_Filters.UseBatchMod = false;
		this.gv_Filters.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Filters_FocusedRowObjectChanged);
		this.groupControl3.Controls.Add(this.grd_User);
		this.groupControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl3.Location = new System.Drawing.Point(0, 0);
		this.groupControl3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.groupControl3.Name = "groupControl3";
		this.groupControl3.Size = new System.Drawing.Size(246, 495);
		this.groupControl3.TabIndex = 2;
		this.groupControl3.Text = "Users";
		this.grd_User.AllowPasteExcel = true;
		this.grd_User.ClipboardData = null;
		this.grd_User.ContextMenuStrip = this.ctx_Main;
		this.grd_User.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_User.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_User.Location = new System.Drawing.Point(2, 23);
		this.grd_User.MainView = this.gv_User;
		this.grd_User.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_User.Name = "grd_User";
		this.grd_User.Size = new System.Drawing.Size(242, 470);
		this.grd_User.TabIndex = 0;
		this.grd_User.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_User });
		this.gv_User.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_User.ColumnKey = null;
		this.gv_User.DisableFormSetting = false;
		this.gv_User.DisplayException = false;
		this.gv_User.EditColumn = null;
		this.gv_User.FieldCurrency = null;
		this.gv_User.FieldRate = null;
		this.gv_User.FormId = "oFilterSetting";
		this.gv_User.GridControl = this.grd_User;
		this.gv_User.IgnoreFilter = false;
		this.gv_User.IndexEditRow = -1;
		this.gv_User.IndicatorWidth = 35;
		this.gv_User.IsApzTable = false;
		this.gv_User.IsInsertNewRow = false;
		this.gv_User.IsLoadingData = false;
		this.gv_User.IsMappingContextAction = false;
		this.gv_User.ItemId = "OUSR";
		this.gv_User.LstColumnAllowBatch = null;
		this.gv_User.LstColumnNotAllowBatch = null;
		this.gv_User.LstColumnNotEditable = null;
		this.gv_User.LstColumNotSetting = null;
		this.gv_User.Name = "gv_User";
		this.gv_User.OptionsSelection.CheckBoxSelectorColumnWidth = 40;
		this.gv_User.OptionsSelection.MultiSelect = true;
		this.gv_User.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_User.UseBatchMod = false;
		this.gv_User.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gv_User_SelectionChanged);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(846, 497);
		this.btn_OK.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(64, 20);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.simpleButton1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleButton1.EditControl = null;
		this.simpleButton1.EditValue = "Cancel";
		this.simpleButton1.FieldName = null;
		this.simpleButton1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButton1.ImageOptions.Image");
		this.simpleButton1.IsSearchControl = false;
		this.simpleButton1.Location = new System.Drawing.Point(914, 497);
		this.simpleButton1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.simpleButton1.Name = "simpleButton1";
		this.simpleButton1.ReadOnly = false;
		this.simpleButton1.SettingInVisible = false;
		this.simpleButton1.SettingReadonly = false;
		this.simpleButton1.Size = new System.Drawing.Size(64, 20);
		this.simpleButton1.TabIndex = 2;
		this.simpleButton1.Text = "Cancel";
		this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(982, 519);
		base.Controls.Add(this.simpleButton1);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oFilterSetting";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_FilterSetting.IconOptions.Icon");
		base.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		base.Name = "frm_FilterSetting";
		this.Text = "Filters Setting";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_FilterSetting_FormClosing);
		base.Load += new System.EventHandler(this.frm_FilterSetting_Load);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Category).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Category).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.groupControl2).EndInit();
		this.groupControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Filters).EndInit();
		this.ctx_Main.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Filters).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl3).EndInit();
		this.groupControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_User).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_User).EndInit();
		base.ResumeLayout(false);
	}
}
