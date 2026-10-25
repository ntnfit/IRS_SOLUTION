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
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Mrps;

public class frm_FilterSettingLine : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private GridControlEx grd_FilterSettingLine;

	private GridViewEx gv_FilterSettingLine;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	private LabelLine labelLine1;

	private GridColumnEx cl_Code;

	private GridColumnEx cl_Name;

	private GridColumnEx cl_Active;

	private RepositoryItemLookUpEdit lookup_Active;

	private GridColumnEx cl_Remarks;

	private TextEditEx txt_FilterCode;

	private ToolStripMenuItem addToolStripMenuItem;

	public int docEntry { get; set; }

	public ApzObjectTypes objType { get; set; }

	public bool _isAddNew { get; set; }

	public string filterCode { get; set; }

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
			btn_OK.Text = Resources.ButtonFind;
			break;
		}
	}

	public frm_FilterSettingLine()
	{
		InitializeComponent();
		OnLoadAsync += frm_FilterSettingLine_Load;
	}

	private async Task LoadDataFilterSettingLine()
	{
		try
		{
			txt_FilterCode.Text = filterCode;
			DataTable dataSource = await _httpClient.FormFilterSettingClient.GetDataFilterSettingLine(docEntry);
			grd_FilterSettingLine.DataSource = dataSource;
			_isAddNew = false;
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task frm_FilterSettingLine_Load(object sender, EventArgs e)
	{
		try
		{
			gv_FilterSettingLine.OptionsView.ColumnAutoWidth = true;
			List<string> list = new List<string>();
			list.Add("Y");
			list.Add("N");
			lookup_Active.DataSource = list;
			await LoadDataFilterSettingLine();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async void AddNew()
	{
		try
		{
			DataTable dataTable = grd_FilterSettingLine.DataSource as DataTable;
			DataRow dataRow = dataTable.NewRow();
			dataRow["DocEntry"] = docEntry;
			dataRow["LineNum"] = 0;
			dataRow["Active"] = "Y";
			dataTable.Rows.Add(dataRow);
			grd_FilterSettingLine.DataSource = dataTable;
			_isAddNew = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				bool flag = false;
				DataTable dataTable = grd_FilterSettingLine.DataSource as DataTable;
				foreach (DataRow row in dataTable.Rows)
				{
					if (Function.ToString(row["Active"]) == "" || Function.ToString(row["Code"]) == "")
					{
						flag = true;
					}
				}
				if (!flag)
				{
					new HttpResult();
					HttpResult httpResult;
					if (_isAddNew)
					{
						DataTable item = (grd_FilterSettingLine.DataSource as DataTable).Copy();
						httpResult = await _httpClient.FormFilterSettingClient.SaveFilterSettingLineAddNew(new Tuple<DataTable, ApzObjectTypes>(item, objType));
					}
					else
					{
						DataTable item2 = dataTable.Copy();
						httpResult = await _httpClient.FormFilterSettingClient.SaveFilterSettingLine(new Tuple<DataTable, ApzObjectTypes>(item2, objType));
					}
					if (httpResult.Message == "")
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					}
					await LoadDataFilterSettingLine();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.MissingData, "");
					FormMode = Enums.FormViewModeType.View;
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

	private void gv_FilterSettingLine_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update)
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

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.No)
			{
				int focusedRowHandle = gv_FilterSettingLine.FocusedRowHandle;
				int num = Function.ParseInt(((DataRowView)gv_FilterSettingLine.GetRow(focusedRowHandle)).Row["LineNum"]);
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("DocEntry");
				dataTable.Columns.Add("LineNum");
				DataRow dataRow = dataTable.NewRow();
				dataRow["DocEntry"] = docEntry;
				dataRow["LineNum"] = num;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				HttpResult httpResult = await _httpClient.FormFilterSettingClient.DeleteFilterSettingLine(new Tuple<DataTable, ApzObjectTypes>(dataTable, objType));
				if (httpResult.Message == "")
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				await LoadDataFilterSettingLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			AddNew();
			FormMode = Enums.FormViewModeType.Update;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Mrps.frm_FilterSettingLine));
		this.grd_FilterSettingLine = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_FilterSettingLine = new Apzon.Controls.Grid.GridViewEx();
		this.cl_Code = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Name = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Active = new Apzon.Controls.Grid.GridColumnEx();
		this.lookup_Active = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_Remarks = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.txt_FilterCode = new Apzon.Controls.Editor.TextEditEx();
		((System.ComponentModel.ISupportInitialize)this.grd_FilterSettingLine).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_FilterSettingLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_Active).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FilterCode.Properties).BeginInit();
		base.SuspendLayout();
		this.grd_FilterSettingLine.AllowPasteExcel = true;
		this.grd_FilterSettingLine.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_FilterSettingLine.ClipboardData = null;
		this.grd_FilterSettingLine.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_FilterSettingLine.Location = new System.Drawing.Point(2, 31);
		this.grd_FilterSettingLine.MainView = this.gv_FilterSettingLine;
		this.grd_FilterSettingLine.Name = "grd_FilterSettingLine";
		this.grd_FilterSettingLine.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.lookup_Active });
		this.grd_FilterSettingLine.Size = new System.Drawing.Size(584, 309);
		this.grd_FilterSettingLine.TabIndex = 0;
		this.grd_FilterSettingLine.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_FilterSettingLine });
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
		this.gv_FilterSettingLine.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_FilterSettingLine.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_FilterSettingLine.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_FilterSettingLine.ColumnKey = null;
		this.gv_FilterSettingLine.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.cl_Code, this.cl_Name, this.cl_Active, this.cl_Remarks });
		this.gv_FilterSettingLine.DisableFormSetting = false;
		this.gv_FilterSettingLine.DisplayException = false;
		this.gv_FilterSettingLine.EditColumn = null;
		this.gv_FilterSettingLine.FieldCurrency = null;
		this.gv_FilterSettingLine.FieldRate = null;
		this.gv_FilterSettingLine.FormId = "oFilterSettingLine";
		this.gv_FilterSettingLine.GridControl = this.grd_FilterSettingLine;
		this.gv_FilterSettingLine.IgnoreFilter = false;
		this.gv_FilterSettingLine.IndexEditRow = -1;
		this.gv_FilterSettingLine.IndicatorWidth = 35;
		this.gv_FilterSettingLine.IsApzTable = false;
		this.gv_FilterSettingLine.IsInsertNewRow = false;
		this.gv_FilterSettingLine.IsLoadingData = false;
		this.gv_FilterSettingLine.IsMappingContextAction = false;
		this.gv_FilterSettingLine.ItemId = "ODATA";
		this.gv_FilterSettingLine.LstColumnAllowBatch = null;
		this.gv_FilterSettingLine.LstColumnNotAllowBatch = null;
		this.gv_FilterSettingLine.LstColumnNotEditable = null;
		this.gv_FilterSettingLine.LstColumNotSetting = null;
		this.gv_FilterSettingLine.Name = "gv_FilterSettingLine";
		this.gv_FilterSettingLine.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_FilterSettingLine.OptionsView.BestFitMaxRowCount = 100;
		this.gv_FilterSettingLine.OptionsView.ColumnAutoWidth = false;
		this.gv_FilterSettingLine.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_FilterSettingLine.OptionsView.ShowGroupPanel = false;
		this.gv_FilterSettingLine.UseBatchMod = false;
		this.gv_FilterSettingLine.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_FilterSettingLine_CellValueChanged);
		this.cl_Code.Caption = "Code";
		this.cl_Code.CurrencyDateField = null;
		this.cl_Code.CurrencyField = null;
		this.cl_Code.CurrencyFixed = null;
		this.cl_Code.CurrencyFixedRate = 0.0;
		this.cl_Code.CurrencyRateField = null;
		this.cl_Code.FieldName = "Code";
		this.cl_Code.Name = "cl_Code";
		this.cl_Code.PrimaryKey = true;
		this.cl_Code.UnitField = null;
		this.cl_Code.Visible = true;
		this.cl_Code.VisibleIndex = 0;
		this.cl_Name.Caption = "Name";
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
		this.cl_Name.VisibleIndex = 1;
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
		this.cl_Remarks.VisibleIndex = 3;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(7, 347);
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
		this.btn_Cancel.Location = new System.Drawing.Point(85, 347);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 8;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Filter Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(413, 8);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(65, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 11;
		this.labelLine1.Utext = "Filter Code";
		this.txt_FilterCode.EditControl = null;
		this.txt_FilterCode.FieldName = null;
		this.txt_FilterCode.IsSearchControl = false;
		this.txt_FilterCode.Location = new System.Drawing.Point(478, 5);
		this.txt_FilterCode.Name = "txt_FilterCode";
		this.txt_FilterCode.Properties.ReadOnly = true;
		this.txt_FilterCode.SettingInVisible = false;
		this.txt_FilterCode.SettingReadonly = false;
		this.txt_FilterCode.Size = new System.Drawing.Size(108, 20);
		this.txt_FilterCode.TabIndex = 12;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(588, 376);
		this.ContextMenuStrip = this.contextMenuStrip1;
		base.Controls.Add(this.txt_FilterCode);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.grd_FilterSettingLine);
		base.FormId = "oFilterSettingLine";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_FilterSettingLine";
		this.Text = "Filter Setting Line";
		((System.ComponentModel.ISupportInitialize)this.grd_FilterSettingLine).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_FilterSettingLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_Active).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FilterCode.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
