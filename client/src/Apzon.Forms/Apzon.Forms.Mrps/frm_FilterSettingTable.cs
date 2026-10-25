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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Mrps;

public class frm_FilterSettingTable : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private GridControlEx grd_FilterSettingTable;

	private GridViewEx gv_FilterSettingTable;

	private GridColumnEx cl_TableName;

	private GridColumnEx cl_Code;

	private GridColumnEx cl_Name;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private RepositoryItemComboBox cbb_ListColumns;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addToolStripMenuItem;

	private ToolStripMenuItem removeToolStripMenuItem;

	private TextEditEx txt_FilterCode;

	private LabelLine labelLine1;

	public ApzObjectTypes objType { get; set; }

	public string filterCode { get; set; }

	public int docEntry { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.View:
			btn_OK.Text = Resources.ButtonOK;
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Resources.ButtonUpdate;
			break;
		}
	}

	public frm_FilterSettingTable()
	{
		InitializeComponent();
		OnLoadAsync += frm_FilterSettingTable_Load;
	}

	private async Task frm_FilterSettingTable_Load(object sender, EventArgs e)
	{
		try
		{
			txt_FilterCode.Text = filterCode;
			gv_FilterSettingTable.OptionsView.ColumnAutoWidth = true;
			await LoadDataFilterSettingTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataFilterSettingTable()
	{
		try
		{
			DataTable dataSource = await _httpClient.FormFilterSettingClient.GetDataFilterSettingTable(docEntry);
			grd_FilterSettingTable.DataSource = dataSource;
			if (gv_FilterSettingTable.RowCount > 0)
			{
				addToolStripMenuItem.Enabled = false;
			}
			else
			{
				addToolStripMenuItem.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async void AddNewRow()
	{
		try
		{
			if (gv_FilterSettingTable.RowCount > 0)
			{
				addToolStripMenuItem.Enabled = false;
				return;
			}
			addToolStripMenuItem.Enabled = true;
			DataTable dataTable = await _httpClient.FormFilterSettingClient.GetDataFilterSettingTable(docEntry);
			DataRow dataRow = dataTable.NewRow();
			dataRow["DocEntry"] = docEntry;
			dataTable.Rows.Add(dataRow);
			grd_FilterSettingTable.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_FilterSettingTable_ShownEditor(object sender, EventArgs e)
	{
		try
		{
			ColumnView view = (ColumnView)sender;
			if (!(view.ActiveEditor is ComboBoxEdit))
			{
				return;
			}
			int index = gv_FilterSettingTable.FocusedRowHandle;
			string tableName = Function.ToString(((DataRowView)gv_FilterSettingTable.GetRow(index)).Row["TableName"]);
			DataTable dataTable = await _httpClient.FormFilterSettingClient.GetDataTableByTableName(tableName);
			if (dataTable.Rows.Count > 0)
			{
				List<string> list = new List<string>();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					list.Add(Function.ToString(dataTable.Rows[i][3]));
				}
				if (view.FocusedColumn.FieldName == "Code" || view.FocusedColumn.FieldName == "Name")
				{
					((ComboBoxEdit)view.ActiveEditor).Properties.Items.AddRange(list);
				}
			}
			else
			{
				gv_FilterSettingTable.RefreshRow(index);
				WinCommons.Error(Enums.MessageCode.TableNotFound, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_FilterSettingTable_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			int index = gv_FilterSettingTable.FocusedRowHandle;
			string tableName = Function.ToString(((DataRowView)gv_FilterSettingTable.GetRow(index)).Row["TableName"]);
			if ((await _httpClient.FormFilterSettingClient.GetDataTableByTableName(tableName)).Rows.Count > 0)
			{
				if (FormMode != Enums.FormViewModeType.Update)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
			}
			else
			{
				gv_FilterSettingTable.RefreshRow(index);
				WinCommons.Error(Enums.MessageCode.TableNotFound, "");
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
			AddNewRow();
			FormMode = Enums.FormViewModeType.Update;
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

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				bool flag = false;
				DataTable table = ((DataView)gv_FilterSettingTable.DataSource).Table;
				foreach (DataRow row in table.Rows)
				{
					if (Function.ToString(row["TableName"]) == "" || Function.ToString(row["Code"]) == "" || Function.ToString(row["Name"]) == "")
					{
						flag = true;
					}
				}
				if (!flag)
				{
					DataTable item = table.Copy();
					HttpResult httpResult = await _httpClient.FormFilterSettingClient.SaveFilterSettingTable(new Tuple<DataTable, ApzObjectTypes>(item, objType));
					if (httpResult.Message == "")
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						addToolStripMenuItem.Enabled = true;
						await LoadDataFilterSettingTable();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					}
					FormMode = Enums.FormViewModeType.View;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.MissingData, "");
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

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.No)
			{
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("DocEntry");
				DataRow dataRow = dataTable.NewRow();
				dataRow["DocEntry"] = docEntry;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				HttpResult httpResult = await _httpClient.FormFilterSettingClient.DeleteFilterSettingTable(new Tuple<DataTable, ApzObjectTypes>(dataTable, objType));
				if (httpResult.Message == "")
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				FormMode = Enums.FormViewModeType.View;
				await LoadDataFilterSettingTable();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Mrps.frm_FilterSettingTable));
		this.grd_FilterSettingTable = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_FilterSettingTable = new Apzon.Controls.Grid.GridViewEx();
		this.cl_TableName = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Code = new Apzon.Controls.Grid.GridColumnEx();
		this.cbb_ListColumns = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.cl_Name = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_FilterCode = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.grd_FilterSettingTable).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_FilterSettingTable).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_ListColumns).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FilterCode.Properties).BeginInit();
		base.SuspendLayout();
		this.grd_FilterSettingTable.AllowPasteExcel = true;
		this.grd_FilterSettingTable.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_FilterSettingTable.ClipboardData = null;
		this.grd_FilterSettingTable.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_FilterSettingTable.Location = new System.Drawing.Point(1, 38);
		this.grd_FilterSettingTable.MainView = this.gv_FilterSettingTable;
		this.grd_FilterSettingTable.Name = "grd_FilterSettingTable";
		this.grd_FilterSettingTable.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.cbb_ListColumns });
		this.grd_FilterSettingTable.Size = new System.Drawing.Size(499, 268);
		this.grd_FilterSettingTable.TabIndex = 0;
		this.grd_FilterSettingTable.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_FilterSettingTable });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addToolStripMenuItem, this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 48);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.addToolStripMenuItem.Text = "Add";
		this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_FilterSettingTable.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_FilterSettingTable.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_FilterSettingTable.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_FilterSettingTable.ColumnKey = null;
		this.gv_FilterSettingTable.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.cl_TableName, this.cl_Code, this.cl_Name });
		this.gv_FilterSettingTable.DisableFormSetting = false;
		this.gv_FilterSettingTable.DisplayException = false;
		this.gv_FilterSettingTable.EditColumn = null;
		this.gv_FilterSettingTable.FieldCurrency = null;
		this.gv_FilterSettingTable.FieldRate = null;
		this.gv_FilterSettingTable.FormId = "oFilterSettingTable";
		this.gv_FilterSettingTable.GridControl = this.grd_FilterSettingTable;
		this.gv_FilterSettingTable.IgnoreFilter = false;
		this.gv_FilterSettingTable.IndexEditRow = -1;
		this.gv_FilterSettingTable.IndicatorWidth = 35;
		this.gv_FilterSettingTable.IsApzTable = false;
		this.gv_FilterSettingTable.IsInsertNewRow = false;
		this.gv_FilterSettingTable.IsLoadingData = false;
		this.gv_FilterSettingTable.IsMappingContextAction = false;
		this.gv_FilterSettingTable.ItemId = "ODATA";
		this.gv_FilterSettingTable.LstColumnAllowBatch = null;
		this.gv_FilterSettingTable.LstColumnNotAllowBatch = null;
		this.gv_FilterSettingTable.LstColumnNotEditable = null;
		this.gv_FilterSettingTable.LstColumNotSetting = null;
		this.gv_FilterSettingTable.Name = "gv_FilterSettingTable";
		this.gv_FilterSettingTable.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_FilterSettingTable.OptionsView.BestFitMaxRowCount = 100;
		this.gv_FilterSettingTable.OptionsView.ColumnAutoWidth = false;
		this.gv_FilterSettingTable.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_FilterSettingTable.OptionsView.ShowGroupPanel = false;
		this.gv_FilterSettingTable.UseBatchMod = false;
		this.gv_FilterSettingTable.ShownEditor += new System.EventHandler(this.gv_FilterSettingTable_ShownEditor);
		this.gv_FilterSettingTable.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_FilterSettingTable_CellValueChanged);
		this.cl_TableName.Caption = "Table Name";
		this.cl_TableName.CurrencyDateField = null;
		this.cl_TableName.CurrencyField = null;
		this.cl_TableName.CurrencyFixed = null;
		this.cl_TableName.CurrencyFixedRate = 0.0;
		this.cl_TableName.CurrencyRateField = null;
		this.cl_TableName.FieldName = "TableName";
		this.cl_TableName.Name = "cl_TableName";
		this.cl_TableName.PrimaryKey = true;
		this.cl_TableName.UnitField = null;
		this.cl_TableName.Visible = true;
		this.cl_TableName.VisibleIndex = 0;
		this.cl_Code.Caption = "Code";
		this.cl_Code.ColumnEdit = this.cbb_ListColumns;
		this.cl_Code.CurrencyDateField = null;
		this.cl_Code.CurrencyField = null;
		this.cl_Code.CurrencyFixed = null;
		this.cl_Code.CurrencyFixedRate = 0.0;
		this.cl_Code.CurrencyRateField = null;
		this.cl_Code.FieldName = "Code";
		this.cl_Code.Name = "cl_Code";
		this.cl_Code.PrimaryKey = false;
		this.cl_Code.UnitField = null;
		this.cl_Code.Visible = true;
		this.cl_Code.VisibleIndex = 1;
		this.cbb_ListColumns.AutoHeight = false;
		this.cbb_ListColumns.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbb_ListColumns.Name = "cbb_ListColumns";
		this.cl_Name.Caption = "Name";
		this.cl_Name.ColumnEdit = this.cbb_ListColumns;
		this.cl_Name.CurrencyDateField = null;
		this.cl_Name.CurrencyField = null;
		this.cl_Name.CurrencyFixed = null;
		this.cl_Name.CurrencyFixedRate = 0.0;
		this.cl_Name.CurrencyRateField = null;
		this.cl_Name.FieldName = "Name";
		this.cl_Name.Name = "cl_Name";
		this.cl_Name.PrimaryKey = false;
		this.cl_Name.UnitField = null;
		this.cl_Name.Visible = true;
		this.cl_Name.VisibleIndex = 2;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(1, 312);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 7;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(79, 312);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 8;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.txt_FilterCode.EditControl = null;
		this.txt_FilterCode.FieldName = null;
		this.txt_FilterCode.IsSearchControl = false;
		this.txt_FilterCode.Location = new System.Drawing.Point(389, 10);
		this.txt_FilterCode.Name = "txt_FilterCode";
		this.txt_FilterCode.Properties.ReadOnly = true;
		this.txt_FilterCode.SettingInVisible = false;
		this.txt_FilterCode.SettingReadonly = false;
		this.txt_FilterCode.Size = new System.Drawing.Size(111, 20);
		this.txt_FilterCode.TabIndex = 14;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Filter Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(324, 13);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(65, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 13;
		this.labelLine1.Utext = "Filter Code";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(501, 338);
		this.ContextMenuStrip = this.contextMenuStrip1;
		base.Controls.Add(this.txt_FilterCode);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.grd_FilterSettingTable);
		base.FormId = "oFilterSettingTable";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_FilterSettingTable";
		this.Text = "Filter Setting Table";
		((System.ComponentModel.ISupportInitialize)this.grd_FilterSettingTable).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_FilterSettingTable).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_ListColumns).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FilterCode.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
