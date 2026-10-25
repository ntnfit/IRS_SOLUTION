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
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTab;

namespace Apzon.Forms.DMS;

public class frm_PromotionSetup : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private XtraTabControlEx tabControl;

	private XtraTabPageEx tab_Category;

	private GridControlEx grd_Category;

	private GridViewEx gv_Category;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx colCateName;

	private GridColumnEx gridColumnEx3;

	private XtraTabPageEx tab_Type;

	private GridControlEx grd_Type;

	private GridViewEx gv_Type;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx colTypeName;

	private GridColumnEx gridColumnEx6;

	private XtraTabPageEx tab_Group;

	private GridControlEx grd_Group;

	private GridViewEx gv_Group;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx colGroupName;

	private GridColumnEx gridColumnEx9;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private ToolStripMenuItem removeRowToolStripMenuItem;

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
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_PromotionSetup()
	{
		InitializeComponent();
	}

	private async void frm_PromotionSetup_Load(object sender, EventArgs e)
	{
		await LoadData();
	}

	private async Task LoadData()
	{
		try
		{
			if (!CheckTablesExist())
			{
				WinCommons.Error(Enums.MessageCode.CannotLoadStructure, "");
				addRowToolStripMenuItem.Enabled = false;
				removeRowToolStripMenuItem.Enabled = false;
				return;
			}
			addRowToolStripMenuItem.Enabled = true;
			removeRowToolStripMenuItem.Enabled = true;
			DataSet dataSet = await _httpClient.PromotionSetupClient.GetData();
			grd_Category.DataSource = dataSet.Tables[0];
			grd_Category.RefreshDataSource();
			grd_Type.DataSource = dataSet.Tables[1];
			grd_Type.RefreshDataSource();
			grd_Group.DataSource = dataSet.Tables[2];
			grd_Group.RefreshDataSource();
			AddNewRow(addAllTab: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.Error, ex.Message);
		}
	}

	private void AddNewRow(bool addAllTab = false)
	{
		try
		{
			if (addAllTab)
			{
				GridControlEx[] array = new GridControlEx[3] { grd_Category, grd_Type, grd_Group };
				foreach (GridControlEx gridControlEx in array)
				{
					if (gridControlEx.DataSource is DataTable dataTable)
					{
						dataTable.Rows.Add(dataTable.NewRow());
						gridControlEx.DataSource = dataTable;
						gridControlEx.RefreshDataSource();
					}
				}
			}
			else
			{
				GridControlEx gridControlEx2;
				if (tabControl.SelectedTabPage == tab_Category)
				{
					gridControlEx2 = grd_Category;
				}
				else
				{
					gridControlEx2 = ((tabControl.SelectedTabPage == tab_Type) ? grd_Type : grd_Group);
				}
				if (gridControlEx2.DataSource is DataTable dataTable2)
				{
					dataTable2.Rows.Add(dataTable2.NewRow());
					gridControlEx2.DataSource = dataTable2;
					gridControlEx2.RefreshDataSource();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool RemoveRow()
	{
		try
		{
			GridControlEx gridControlEx;
			if (tabControl.SelectedTabPage == tab_Category)
			{
				gridControlEx = grd_Category;
			}
			else
			{
				gridControlEx = ((tabControl.SelectedTabPage == tab_Type) ? grd_Type : grd_Group);
			}
			DataTable dataTable = gridControlEx.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				return false;
			}
			if (!(gridControlEx.MainView is GridViewEx gridViewEx))
			{
				return false;
			}
			int num = Function.ParseInt(gridViewEx.GetFocusedRowCellValue("ID"));
			if (num > 0 && !CheckUsed((tabControl.SelectedTabPage == tab_Category) ? "C" : ((tabControl.SelectedTabPage == tab_Group) ? "G" : "T"), num))
			{
				WinCommons.Error(Enums.MessageCode.CannotDeleteThisRecordIsBeingUsed, "");
				return false;
			}
			dataTable.Rows.RemoveAt(gridViewEx.FocusedRowHandle);
			if (dataTable.AsEnumerable().All((DataRow r) => !string.IsNullOrEmpty(Function.ToString(r["Name"]))))
			{
				AddNewRow();
			}
			gridControlEx.DataSource = dataTable;
			gridControlEx.RefreshDataSource();
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	private async Task<string> Save()
	{
		try
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = grd_Category.DataSourceTable.Copy();
			DataTable dataTable2 = grd_Type.DataSourceTable.Copy();
			DataTable dataTable3 = grd_Group.DataSourceTable.Copy();
			dataSet.Tables.AddRange(new DataTable[3] { dataTable, dataTable2, dataTable3 });
			HttpResult httpResult = await _httpClient.PromotionSetupClient.SaveData(dataSet);
			if (!string.IsNullOrEmpty(httpResult.Message) && httpResult.Content != null)
			{
				int num = Function.ParseInt(httpResult.Content);
				tabControl.SelectedTabPage = num switch
				{
					2 => (XtraTabPage)tab_Type, 
					1 => tab_Category, 
					_ => tab_Group, 
				};
			}
			return httpResult.Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private bool CheckTablesExist()
	{
		try
		{
			if (WinFuntions.GetTableStructure("APZ_D_OPRC").Columns.Count > 0 && WinFuntions.GetTableStructure("APZ_D_OPRT").Columns.Count > 0 && WinFuntions.GetTableStructure("APZ_D_OPRG").Columns.Count > 0)
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

	private bool CheckUsed(string tab, int id)
	{
		try
		{
			if (string.IsNullOrEmpty(_httpClient.PromotionSetupClient.CheckUsed(tab, id)))
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	private void EnableDisableControl(bool isEnable)
	{
		btn_OK.Enabled = isEnable;
		GridViewEx gridViewEx = gv_Category;
		GridViewEx gridViewEx2 = gv_Type;
		bool flag = (gv_Group.ReadOnly = !isEnable);
		bool readOnly = (gridViewEx2.ReadOnly = flag);
		gridViewEx.ReadOnly = readOnly;
	}

	private void gv_Category_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
			GridControlEx gridControl;
			if (tabControl.SelectedTabPage == tab_Category)
			{
				gridControl = grd_Category;
			}
			else
			{
				gridControl = ((tabControl.SelectedTabPage == tab_Type) ? grd_Type : grd_Group);
			}
			DataTable dataTable = gridControl.DataSource as DataTable;
			if (dataTable.IsNotNull() && e.RowHandle == dataTable.Rows.Count - 1)
			{
				AddNewRow();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		EnableDisableControl(isEnable: false);
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				string text = await Save();
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					await LoadData();
					FormMode = Enums.FormViewModeType.View;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.Error(Enums.MessageCode.None, ex.Message);
		}
		EnableDisableControl(isEnable: true);
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (RemoveRow())
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void gv_Category_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		if (sender is GridViewEx { FocusedColumn: var focusedColumn } gridViewEx && (focusedColumn == colCateName || focusedColumn == colTypeName || focusedColumn == colGroupName) && Function.ParseInt(gridViewEx.GetFocusedRowCellValue("ID")) != 0 && !Function.ToString(gridViewEx.ActiveEditor.OldEditValue).Equals(Function.ToString(e.Value)) && !CheckUsed((tabControl.SelectedTabPage == tab_Category) ? "C" : ((tabControl.SelectedTabPage == tab_Group) ? "G" : "T"), Function.ParseInt(gridViewEx.GetFocusedRowCellValue("ID"))))
		{
			e.Valid = false;
		}
	}

	private void gv_Category_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		WinCommons.Error(Enums.MessageCode.None, "You cannot change name or delete this record");
		e.ExceptionMode = ExceptionMode.Ignore;
	}

	private async void frm_PromotionSetup_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (FormMode != Enums.FormViewModeType.Update)
		{
			return;
		}
		switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
		{
		case DialogResult.Yes:
		{
			e.Cancel = true;
			EnableDisableControl(isEnable: false);
			WinCommons._OpenProcessing();
			string text = await Save();
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				FormMode = Enums.FormViewModeType.View;
				Close();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
				EnableDisableControl(isEnable: true);
			}
			break;
		}
		case DialogResult.Cancel:
			e.Cancel = true;
			break;
		}
	}

	private void grd_Category_Click(object sender, EventArgs e)
	{
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
		this.tabControl = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_Category = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Category = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Category = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.colCateName = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.tab_Type = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Type = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Type = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.colTypeName = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.tab_Group = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Group = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Group = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.colGroupName = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.tabControl).BeginInit();
		this.tabControl.SuspendLayout();
		this.tab_Category.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Category).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Category).BeginInit();
		this.tab_Type.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Type).BeginInit();
		this.tab_Group.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Group).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Group).BeginInit();
		base.SuspendLayout();
		this.tabControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tabControl.IsSearchControl = false;
		this.tabControl.Location = new System.Drawing.Point(0, 0);
		this.tabControl.Name = "tabControl";
		this.tabControl.SelectedTabPage = this.tab_Category;
		this.tabControl.Size = new System.Drawing.Size(800, 411);
		this.tabControl.TabIndex = 0;
		this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[3] { this.tab_Category, this.tab_Type, this.tab_Group });
		this.tab_Category.Controls.Add(this.grd_Category);
		this.tab_Category.EditControl = null;
		this.tab_Category.EditValue = "Category";
		this.tab_Category.FieldName = "";
		this.tab_Category.IsSearchControl = false;
		this.tab_Category.Name = "tab_Category";
		this.tab_Category.ReadOnly = false;
		this.tab_Category.SettingInVisible = false;
		this.tab_Category.SettingReadonly = false;
		this.tab_Category.Size = new System.Drawing.Size(798, 386);
		this.tab_Category.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Category.Text = "Category";
		this.grd_Category.AllowPasteExcel = true;
		this.grd_Category.ClipboardData = null;
		this.grd_Category.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Category.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Category.Location = new System.Drawing.Point(0, 0);
		this.grd_Category.MainView = this.gv_Category;
		this.grd_Category.Name = "grd_Category";
		this.grd_Category.Size = new System.Drawing.Size(798, 386);
		this.grd_Category.TabIndex = 0;
		this.grd_Category.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Category });
		this.grd_Category.Click += new System.EventHandler(this.grd_Category_Click);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addRowToolStripMenuItem, this.removeRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(144, 48);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.addRowToolStripMenuItem.Text = "Add Row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gv_Category.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Category.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Category.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Category.ColumnKey = null;
		this.gv_Category.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gridColumnEx1, this.colCateName, this.gridColumnEx3 });
		this.gv_Category.DisableFormSetting = false;
		this.gv_Category.DisplayException = false;
		this.gv_Category.EditColumn = null;
		this.gv_Category.FieldCurrency = null;
		this.gv_Category.FieldRate = null;
		this.gv_Category.FormId = "oPromotionSetup";
		this.gv_Category.GridControl = this.grd_Category;
		this.gv_Category.IgnoreFilter = false;
		this.gv_Category.IndexEditRow = -1;
		this.gv_Category.IndicatorWidth = 35;
		this.gv_Category.IsApzTable = false;
		this.gv_Category.IsInsertNewRow = false;
		this.gv_Category.IsLoadingData = false;
		this.gv_Category.IsMappingContextAction = false;
		this.gv_Category.ItemId = "APZ_D_OPRC";
		this.gv_Category.LstColumnAllowBatch = null;
		this.gv_Category.LstColumnNotAllowBatch = null;
		this.gv_Category.LstColumnNotEditable = new string[0];
		this.gv_Category.LstColumNotSetting = new System.Collections.Generic.List<string> { "ID" };
		this.gv_Category.Name = "gv_Category";
		this.gv_Category.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Category.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Category.OptionsPrint.AutoWidth = false;
		this.gv_Category.OptionsPrint.UsePrintStyles = false;
		this.gv_Category.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Category.OptionsView.ColumnAutoWidth = false;
		this.gv_Category.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Category.OptionsView.ShowGroupedColumns = true;
		this.gv_Category.OptionsView.ShowGroupPanel = false;
		this.gv_Category.UseBatchMod = false;
		this.gv_Category.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Category_CellValueChanged);
		this.gv_Category.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_Category_ValidatingEditor);
		this.gv_Category.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gv_Category_InvalidValueException);
		this.gridColumnEx1.Caption = "Category ID";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "ID";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.colCateName.Caption = "Name";
		this.colCateName.CurrencyDateField = null;
		this.colCateName.CurrencyField = null;
		this.colCateName.CurrencyFixed = null;
		this.colCateName.CurrencyFixedRate = 0.0;
		this.colCateName.CurrencyRateField = null;
		this.colCateName.FieldName = "Name";
		this.colCateName.Name = "colCateName";
		this.colCateName.PrimaryKey = false;
		this.colCateName.UnitField = null;
		this.colCateName.Visible = true;
		this.colCateName.VisibleIndex = 0;
		this.colCateName.Width = 91;
		this.gridColumnEx3.Caption = "Description";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "Descr";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 1;
		this.gridColumnEx3.Width = 143;
		this.tab_Type.Controls.Add(this.grd_Type);
		this.tab_Type.EditControl = null;
		this.tab_Type.EditValue = "Type";
		this.tab_Type.FieldName = "";
		this.tab_Type.IsSearchControl = false;
		this.tab_Type.Name = "tab_Type";
		this.tab_Type.PageVisible = false;
		this.tab_Type.ReadOnly = false;
		this.tab_Type.SettingInVisible = false;
		this.tab_Type.SettingReadonly = false;
		this.tab_Type.Size = new System.Drawing.Size(798, 386);
		this.tab_Type.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Type.Text = "Type";
		this.grd_Type.AllowPasteExcel = false;
		this.grd_Type.ClipboardData = null;
		this.grd_Type.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Type.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Type.Location = new System.Drawing.Point(0, 0);
		this.grd_Type.MainView = this.gv_Type;
		this.grd_Type.Name = "grd_Type";
		this.grd_Type.Size = new System.Drawing.Size(798, 386);
		this.grd_Type.TabIndex = 0;
		this.grd_Type.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Type });
		this.gv_Type.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Type.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Type.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Type.ColumnKey = null;
		this.gv_Type.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gridColumnEx4, this.colTypeName, this.gridColumnEx6 });
		this.gv_Type.DisableFormSetting = false;
		this.gv_Type.DisplayException = false;
		this.gv_Type.EditColumn = null;
		this.gv_Type.FieldCurrency = null;
		this.gv_Type.FieldRate = null;
		this.gv_Type.FormId = "oPromotionSetup";
		this.gv_Type.GridControl = this.grd_Type;
		this.gv_Type.IgnoreFilter = false;
		this.gv_Type.IndexEditRow = -1;
		this.gv_Type.IndicatorWidth = 35;
		this.gv_Type.IsApzTable = false;
		this.gv_Type.IsInsertNewRow = false;
		this.gv_Type.IsLoadingData = false;
		this.gv_Type.IsMappingContextAction = false;
		this.gv_Type.ItemId = "APZ_D_OPRT";
		this.gv_Type.LstColumnAllowBatch = null;
		this.gv_Type.LstColumnNotAllowBatch = null;
		this.gv_Type.LstColumnNotEditable = null;
		this.gv_Type.LstColumNotSetting = new System.Collections.Generic.List<string> { "ID" };
		this.gv_Type.Name = "gv_Type";
		this.gv_Type.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Type.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Type.OptionsPrint.AutoWidth = false;
		this.gv_Type.OptionsPrint.UsePrintStyles = false;
		this.gv_Type.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Type.OptionsView.ColumnAutoWidth = false;
		this.gv_Type.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Type.OptionsView.ShowGroupedColumns = true;
		this.gv_Type.OptionsView.ShowGroupPanel = false;
		this.gv_Type.UseBatchMod = false;
		this.gv_Type.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Category_CellValueChanged);
		this.gv_Type.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_Category_ValidatingEditor);
		this.gv_Type.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gv_Category_InvalidValueException);
		this.gridColumnEx4.Caption = "Type ID";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "ID";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.colTypeName.Caption = "Name";
		this.colTypeName.CurrencyDateField = null;
		this.colTypeName.CurrencyField = null;
		this.colTypeName.CurrencyFixed = null;
		this.colTypeName.CurrencyFixedRate = 0.0;
		this.colTypeName.CurrencyRateField = null;
		this.colTypeName.FieldName = "Name";
		this.colTypeName.Name = "colTypeName";
		this.colTypeName.PrimaryKey = false;
		this.colTypeName.UnitField = null;
		this.colTypeName.Visible = true;
		this.colTypeName.VisibleIndex = 0;
		this.colTypeName.Width = 116;
		this.gridColumnEx6.Caption = "Description";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "Descr";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 1;
		this.gridColumnEx6.Width = 385;
		this.tab_Group.Controls.Add(this.grd_Group);
		this.tab_Group.EditControl = null;
		this.tab_Group.EditValue = "Group";
		this.tab_Group.FieldName = "";
		this.tab_Group.IsSearchControl = false;
		this.tab_Group.Name = "tab_Group";
		this.tab_Group.ReadOnly = false;
		this.tab_Group.SettingInVisible = false;
		this.tab_Group.SettingReadonly = false;
		this.tab_Group.Size = new System.Drawing.Size(798, 386);
		this.tab_Group.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Group.Text = "Group";
		this.grd_Group.AllowPasteExcel = false;
		this.grd_Group.ClipboardData = null;
		this.grd_Group.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Group.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Group.Location = new System.Drawing.Point(0, 0);
		this.grd_Group.MainView = this.gv_Group;
		this.grd_Group.Name = "grd_Group";
		this.grd_Group.Size = new System.Drawing.Size(798, 386);
		this.grd_Group.TabIndex = 0;
		this.grd_Group.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Group });
		this.gv_Group.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Group.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Group.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Group.ColumnKey = null;
		this.gv_Group.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gridColumnEx7, this.colGroupName, this.gridColumnEx9 });
		this.gv_Group.DisableFormSetting = false;
		this.gv_Group.DisplayException = false;
		this.gv_Group.EditColumn = null;
		this.gv_Group.FieldCurrency = null;
		this.gv_Group.FieldRate = null;
		this.gv_Group.FormId = "oPromotionSetup";
		this.gv_Group.GridControl = this.grd_Group;
		this.gv_Group.IgnoreFilter = false;
		this.gv_Group.IndexEditRow = -1;
		this.gv_Group.IndicatorWidth = 35;
		this.gv_Group.IsApzTable = false;
		this.gv_Group.IsInsertNewRow = false;
		this.gv_Group.IsLoadingData = false;
		this.gv_Group.IsMappingContextAction = false;
		this.gv_Group.ItemId = "APZ_D_OPRG";
		this.gv_Group.LstColumnAllowBatch = null;
		this.gv_Group.LstColumnNotAllowBatch = null;
		this.gv_Group.LstColumnNotEditable = null;
		this.gv_Group.LstColumNotSetting = new System.Collections.Generic.List<string> { "ID" };
		this.gv_Group.Name = "gv_Group";
		this.gv_Group.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Group.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Group.OptionsPrint.AutoWidth = false;
		this.gv_Group.OptionsPrint.UsePrintStyles = false;
		this.gv_Group.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Group.OptionsView.ColumnAutoWidth = false;
		this.gv_Group.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Group.OptionsView.ShowGroupedColumns = true;
		this.gv_Group.OptionsView.ShowGroupPanel = false;
		this.gv_Group.UseBatchMod = false;
		this.gv_Group.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Category_CellValueChanged);
		this.gv_Group.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_Category_ValidatingEditor);
		this.gv_Group.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gv_Category_InvalidValueException);
		this.gridColumnEx7.Caption = "Group ID";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "ID";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.colGroupName.Caption = "Name";
		this.colGroupName.CurrencyDateField = null;
		this.colGroupName.CurrencyField = null;
		this.colGroupName.CurrencyFixed = null;
		this.colGroupName.CurrencyFixedRate = 0.0;
		this.colGroupName.CurrencyRateField = null;
		this.colGroupName.FieldName = "Name";
		this.colGroupName.Name = "colGroupName";
		this.colGroupName.PrimaryKey = false;
		this.colGroupName.UnitField = null;
		this.colGroupName.Visible = true;
		this.colGroupName.VisibleIndex = 0;
		this.colGroupName.Width = 191;
		this.gridColumnEx9.Caption = "Description";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "Descr";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 1;
		this.gridColumnEx9.Width = 433;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(12, 417);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(93, 417);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(800, 450);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.tabControl);
		base.FormId = "oPromotionSetup";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_PromotionSetup";
		this.Text = "Promotion Setup";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_PromotionSetup_FormClosing);
		base.Load += new System.EventHandler(this.frm_PromotionSetup_Load);
		((System.ComponentModel.ISupportInitialize)this.tabControl).EndInit();
		this.tabControl.ResumeLayout(false);
		this.tab_Category.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Category).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Category).EndInit();
		this.tab_Type.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Type).EndInit();
		this.tab_Group.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Group).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Group).EndInit();
		base.ResumeLayout(false);
	}
}
