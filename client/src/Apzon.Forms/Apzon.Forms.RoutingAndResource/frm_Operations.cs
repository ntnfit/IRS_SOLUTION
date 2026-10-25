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
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.UserControls;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.RoutingAndResource;

public class frm_Operations : XtraFormEx
{
	private bool _isSearch;

	private bool _isAddNew;

	private string OpeCode = "";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private GridControlEx grd_Operation;

	private GridViewEx gv_Operation;

	private GridColumnEx gc_OperationCode;

	private GridColumnEx gc_OperationName;

	private GridColumnEx gc_Remark;

	private GridColumnEx gc_Status;

	private RepositoryItemLookUpEdit lookup_Status;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addToolStripMenuItem;

	private ToolStripMenuItem removeToolStripMenuItem;

	private ButtonEditCFL txt_OpeCode;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private LookUpEditEx txt_Status;

	private SimpleButtonEx btSearch;

	private GridColumnEx Color;

	private RepositoryItemColorEdit rColorEdit;

	public string OperationsCode { get; set; }

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
			btn_OK.Text = "OK";
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = "Update";
			break;
		}
	}

	public frm_Operations()
	{
		InitializeComponent();
		OnLoadAsync += frm_Operations_Load;
	}

	private async Task frm_Operations_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("A", "Active");
			dictionary.Add("I", "Inactive");
			lookup_Status.DataSource = dictionary;
			txt_Status.Properties.DataSource = dictionary;
			await LoadOperation();
			await FocusedRowByOpeCode();
			gv_Operation.OptionsView.ColumnAutoWidth = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadOperation()
	{
		try
		{
			txt_OpeCode.ResetText();
			txt_Status.EditValue = null;
			Search();
			FormMode = Enums.FormViewModeType.View;
			await gv_Operation.ReloadFormSetting();
			_isAddNew = false;
			_isSearch = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewRow()
	{
		if (!_isAddNew)
		{
			FormMode = Enums.FormViewModeType.Update;
			gv_Operation.AddNewRow();
			int focusedRowHandle = gv_Operation.FocusedRowHandle;
			if (gv_Operation.IsNewItemRow(focusedRowHandle))
			{
				gv_Operation.SetRowCellValue(focusedRowHandle, gv_Operation.Columns["Status"], "A");
			}
		}
		_isAddNew = true;
	}

	private async void Search()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("OpeCode");
			dataTable.Columns.Add("Status");
			DataRow dataRow = dataTable.NewRow();
			dataRow[0] = Function.ToString(txt_OpeCode.EditValue);
			dataRow[1] = Function.ToString(txt_Status.EditValue);
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataTable dataSource = await _httpClient.OpearationsClient.SearchData(dataTable);
			grd_Operation.DataSource = dataSource;
			_isSearch = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadData(Enums.NavigateType type)
	{
		try
		{
			DataTable dataSource = await _httpClient.OpearationsClient.OperationsGetDataDoc(OpeCode, type);
			grd_Operation.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task FocusedRowByOpeCode()
	{
		try
		{
			DataTable dataTable = await _httpClient.OpearationsClient.GetDataLine(OperationsCode);
			if (dataTable.IsNotNull())
			{
				grd_Operation.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MODE_FIND()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			Search();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH()
	{
		try
		{
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadData(Enums.NavigateType.Previous);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadData(Enums.NavigateType.Last);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadData(Enums.NavigateType.Next);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadData(Enums.NavigateType.First);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			await LoadOperation();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Operation_CellValueChanged(object sender, CellValueChangedEventArgs e)
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

	private void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Operation_ShowingEditor(object sender, CancelEventArgs e)
	{
	}

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.No)
			{
				DataTable dataTable = gv_Operation.GetListDataSourceSelectedRow().CopyToDataTable();
				HttpResult httpResult = await _httpClient.OpearationsClient.DeleteData(dataTable.Copy());
				if (httpResult.Message == "")
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					await LoadOperation();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
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
		_ = 2;
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				DataTable dataTable = grd_Operation.DataSource as DataTable;
				HttpResult result = await _httpClient.OpearationsClient.SaveData(dataTable.Copy());
				if (result.Message == "")
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					if (_isSearch)
					{
						Search();
						FormMode = Enums.FormViewModeType.View;
						_isAddNew = false;
					}
					else
					{
						await LoadOperation();
					}
				}
				else
				{
					if (result.Message == "Operations Code Duplicate")
					{
						await LoadOperation();
						FormMode = Enums.FormViewModeType.Update;
						AddNewRow();
					}
					WinCommons.Error(Enums.MessageCode.None, result.Message);
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

	private void btSearch_Click(object sender, EventArgs e)
	{
		try
		{
			Search();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Operation_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			OpeCode = Function.ToString(gv_Operation.GetFocusedRowCellValue(gc_OperationCode));
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.RoutingAndResource.frm_Operations));
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
		this.grd_Operation = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Operation = new Apzon.Controls.Grid.GridViewEx();
		this.gc_OperationCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_OperationName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Remark = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Status = new Apzon.Controls.Grid.GridColumnEx();
		this.lookup_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.Color = new Apzon.Controls.Grid.GridColumnEx();
		this.rColorEdit = new DevExpress.XtraEditors.Repository.RepositoryItemColorEdit();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_OpeCode = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Status = new Apzon.Controls.Editor.LookUpEditEx();
		this.btSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Operation).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Operation).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rColorEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_OpeCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Status.Properties).BeginInit();
		base.SuspendLayout();
		this.grd_Operation.AllowPasteExcel = true;
		this.grd_Operation.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Operation.ClipboardData = null;
		this.grd_Operation.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Operation.Location = new System.Drawing.Point(0, 38);
		this.grd_Operation.MainView = this.gv_Operation;
		this.grd_Operation.Name = "grd_Operation";
		this.grd_Operation.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.lookup_Status, this.rColorEdit });
		this.grd_Operation.Size = new System.Drawing.Size(595, 256);
		this.grd_Operation.TabIndex = 0;
		this.grd_Operation.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Operation });
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
		this.gv_Operation.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Operation.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Operation.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Operation.ColumnKey = null;
		this.gv_Operation.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gc_OperationCode, this.gc_OperationName, this.gc_Remark, this.gc_Status, this.Color });
		this.gv_Operation.DisableFormSetting = false;
		this.gv_Operation.DisplayException = false;
		this.gv_Operation.EditColumn = null;
		this.gv_Operation.FieldCurrency = null;
		this.gv_Operation.FieldRate = null;
		this.gv_Operation.FormId = "oOperation";
		this.gv_Operation.GridControl = this.grd_Operation;
		this.gv_Operation.IgnoreFilter = false;
		this.gv_Operation.IndexEditRow = -1;
		this.gv_Operation.IndicatorWidth = 35;
		this.gv_Operation.IsApzTable = false;
		this.gv_Operation.IsInsertNewRow = false;
		this.gv_Operation.IsLoadingData = false;
		this.gv_Operation.IsMappingContextAction = false;
		this.gv_Operation.ItemId = "APZ_OOMD";
		this.gv_Operation.LstColumnAllowBatch = null;
		this.gv_Operation.LstColumnNotAllowBatch = null;
		this.gv_Operation.LstColumnNotEditable = null;
		this.gv_Operation.LstColumNotSetting = null;
		this.gv_Operation.Name = "gv_Operation";
		this.gv_Operation.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Operation.OptionsSelection.MultiSelect = true;
		this.gv_Operation.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Operation.OptionsView.ColumnAutoWidth = false;
		this.gv_Operation.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Operation.OptionsView.ShowGroupPanel = false;
		this.gv_Operation.UseBatchMod = false;
		this.gv_Operation.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Operation_ShowingEditor);
		this.gv_Operation.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Operation_FocusedRowObjectChanged);
		this.gv_Operation.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Operation_CellValueChanged);
		this.gc_OperationCode.Caption = "Operation Code";
		this.gc_OperationCode.CurrencyDateField = null;
		this.gc_OperationCode.CurrencyField = null;
		this.gc_OperationCode.CurrencyFixed = null;
		this.gc_OperationCode.CurrencyFixedRate = 0.0;
		this.gc_OperationCode.CurrencyRateField = null;
		this.gc_OperationCode.FieldName = "OpeCode";
		this.gc_OperationCode.Name = "gc_OperationCode";
		this.gc_OperationCode.PrimaryKey = true;
		this.gc_OperationCode.UnitField = null;
		this.gc_OperationCode.Visible = true;
		this.gc_OperationCode.VisibleIndex = 0;
		this.gc_OperationName.Caption = "Operation Name";
		this.gc_OperationName.CurrencyDateField = null;
		this.gc_OperationName.CurrencyField = null;
		this.gc_OperationName.CurrencyFixed = null;
		this.gc_OperationName.CurrencyFixedRate = 0.0;
		this.gc_OperationName.CurrencyRateField = null;
		this.gc_OperationName.FieldName = "OpeName";
		this.gc_OperationName.Name = "gc_OperationName";
		this.gc_OperationName.PrimaryKey = false;
		this.gc_OperationName.UnitField = null;
		this.gc_OperationName.Visible = true;
		this.gc_OperationName.VisibleIndex = 1;
		this.gc_Remark.Caption = "Remark";
		this.gc_Remark.CurrencyDateField = null;
		this.gc_Remark.CurrencyField = null;
		this.gc_Remark.CurrencyFixed = null;
		this.gc_Remark.CurrencyFixedRate = 0.0;
		this.gc_Remark.CurrencyRateField = null;
		this.gc_Remark.FieldName = "Remarks";
		this.gc_Remark.Name = "gc_Remark";
		this.gc_Remark.PrimaryKey = false;
		this.gc_Remark.UnitField = null;
		this.gc_Remark.Visible = true;
		this.gc_Remark.VisibleIndex = 2;
		this.gc_Status.Caption = "Status";
		this.gc_Status.ColumnEdit = this.lookup_Status;
		this.gc_Status.CurrencyDateField = null;
		this.gc_Status.CurrencyField = null;
		this.gc_Status.CurrencyFixed = null;
		this.gc_Status.CurrencyFixedRate = 0.0;
		this.gc_Status.CurrencyRateField = null;
		this.gc_Status.FieldName = "Status";
		this.gc_Status.Name = "gc_Status";
		this.gc_Status.PrimaryKey = false;
		this.gc_Status.UnitField = null;
		this.gc_Status.Visible = true;
		this.gc_Status.VisibleIndex = 3;
		this.lookup_Status.AutoHeight = false;
		this.lookup_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookup_Status.Name = "lookup_Status";
		this.lookup_Status.NullText = "";
		this.Color.Caption = "Color in Gantt Chart";
		this.Color.ColumnEdit = this.rColorEdit;
		this.Color.CurrencyDateField = null;
		this.Color.CurrencyField = null;
		this.Color.CurrencyFixed = null;
		this.Color.CurrencyFixedRate = 0.0;
		this.Color.CurrencyRateField = null;
		this.Color.FieldName = "Color";
		this.Color.Name = "Color";
		this.Color.PrimaryKey = false;
		this.Color.UnitField = null;
		this.Color.Visible = true;
		this.Color.VisibleIndex = 4;
		this.rColorEdit.AutoHeight = false;
		this.rColorEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rColorEdit.Name = "rColorEdit";
		this.rColorEdit.StoreColorAsInteger = true;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(12, 305);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 9;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(93, 305);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 10;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.txt_OpeCode.EditControl = null;
		this.txt_OpeCode.FieldName = null;
		this.txt_OpeCode.IsSearchControl = false;
		this.txt_OpeCode.Location = new System.Drawing.Point(117, 12);
		this.txt_OpeCode.Name = "txt_OpeCode";
		this.txt_OpeCode.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.txt_OpeCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, false, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_OpeCode.Properties.ContainerControl = null;
		this.txt_OpeCode.Properties.DisplayMember = "OpeCode";
		this.txt_OpeCode.Properties.DisplayMember2 = "";
		this.txt_OpeCode.Properties.LoadDataSourceWhenEnter = false;
		this.txt_OpeCode.Properties.SeparateCharacterForMulti = ';';
		this.txt_OpeCode.Properties.SeparateString = ", ";
		this.txt_OpeCode.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ListOperation;
		this.txt_OpeCode.Properties.UsingAutoCompleteBox = false;
		this.txt_OpeCode.Properties.UsingDataSource = true;
		this.txt_OpeCode.Properties.ValidateWhenNoDataSource = false;
		this.txt_OpeCode.Properties.ValueMember = "OpeCode";
		this.txt_OpeCode.SettingInVisible = false;
		this.txt_OpeCode.SettingReadonly = false;
		this.txt_OpeCode.Size = new System.Drawing.Size(100, 20);
		this.txt_OpeCode.TabIndex = 11;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Status";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(237, 15);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(57, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 12;
		this.labelLine1.Utext = "Status";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Opearations Code";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(12, 15);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(103, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 13;
		this.labelLine2.Utext = "Opearations Code";
		this.txt_Status.EditControl = null;
		this.txt_Status.FieldName = null;
		this.txt_Status.IsSearchControl = false;
		this.txt_Status.Location = new System.Drawing.Point(296, 12);
		this.txt_Status.Name = "txt_Status";
		this.txt_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_Status.Properties.NullText = "";
		this.txt_Status.SettingInVisible = false;
		this.txt_Status.SettingReadonly = false;
		this.txt_Status.Size = new System.Drawing.Size(100, 20);
		this.txt_Status.TabIndex = 14;
		this.btSearch.Appearance.Font = new System.Drawing.Font("Tahoma", 7.5f);
		this.btSearch.Appearance.Options.UseFont = true;
		this.btSearch.EditControl = null;
		this.btSearch.EditValue = "Search";
		this.btSearch.FieldName = null;
		this.btSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btSearch.ImageOptions.Image");
		this.btSearch.IsSearchControl = false;
		this.btSearch.Location = new System.Drawing.Point(402, 13);
		this.btSearch.Name = "btSearch";
		this.btSearch.ReadOnly = false;
		this.btSearch.SettingInVisible = false;
		this.btSearch.SettingReadonly = false;
		this.btSearch.Size = new System.Drawing.Size(93, 19);
		this.btSearch.TabIndex = 60;
		this.btSearch.Text = "Search";
		this.btSearch.Click += new System.EventHandler(this.btSearch_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(595, 336);
		this.ContextMenuStrip = this.contextMenuStrip1;
		base.Controls.Add(this.btSearch);
		base.Controls.Add(this.txt_Status);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.txt_OpeCode);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.grd_Operation);
		base.FormId = "oOperation";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Operations";
		this.Text = "Operation Master Data";
		((System.ComponentModel.ISupportInitialize)this.grd_Operation).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Operation).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rColorEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_OpeCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Status.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
