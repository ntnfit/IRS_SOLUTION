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
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.RoutingAndResource;

public class frm_ResourceWorkCenter : XtraFormEx
{
	public bool _isSearch;

	public bool _isAddNew;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private GridControlEx grd_ResourceWorkCenter;

	private GridViewEx gv_ResourceWorkCenter;

	private GridViewEx gridViewEx1;

	private GridColumnEx gcResCode;

	private GridColumnEx gcResName;

	private GridColumnEx gcResGroup;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gcBpCode;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gcBardCode;

	private GridColumnEx gridColumnEx9;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private RepositoryItemLookUpEdit lookup_Status;

	private RepositoryItemLookUpEdit lookup_ResourceGroup;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addToolStripMenuItem;

	private RepositoryItemTextEdit txt_Untilization;

	private RepositoryItemLookUpEdit lookup_Dept;

	private ToolStripMenuItem removeToolStripMenuItem;

	private RepositoryItemButtonEditCFL bte_Calendar;

	private RepositoryItemButtonEditCFL bte_BPCode;

	private RepositoryItemTextEdit txt_ResCode;

	private LookUpEditEx txt_ResGroup;

	private LookUpEditEx txt_Status;

	private LookUpEditEx txt_Department;

	private SimpleButtonEx btSearch;

	private LabelLine labelLine6;

	private LabelLine labelLine5;

	private LabelLine labelLine2;

	private LabelLine labelLine1;

	private LabelLine labelLine4;

	private LabelLine labelLine3;

	private ButtonEditCFL txt_BPCode;

	private ButtonEditCFL txt_Calendar;

	private ButtonEditCFL txt_ResourceCode;

	private GridColumnEx gcResType;

	private RepositoryItemLookUpEdit rResType;

	public string resCode { get; set; }

	public string resourceCode { get; set; }

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

	public frm_ResourceWorkCenter()
	{
		InitializeComponent();
		OnLoadAsync += frm_ResourceWorkCenter_Load;
	}

	private async Task frm_ResourceWorkCenter_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			DataSet dataSet = await _httpClient.ResourceWorkCenter.GetDataOriginal();
			DataTable dataSource = dataSet.Tables[0];
			DataTable dataTable = dataSet.Tables[1];
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("A", "Active");
			dictionary.Add("I", "Inactive");
			lookup_Status.DataSource = dictionary;
			txt_Status.Properties.DataSource = dictionary;
			lookup_ResourceGroup.DataSource = dataSource;
			txt_ResGroup.Properties.DataSource = dataSource;
			List<Common.Table_Code> dataSource2 = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Name = "Machine",
					Code = "M"
				},
				new Common.Table_Code
				{
					Name = "Labor",
					Code = "L"
				},
				new Common.Table_Code
				{
					Name = "Other",
					Code = "O"
				}
			};
			rResType.DataSource = dataSource2;
			bte_BPCode.Buttons[0].Visible = false;
			bte_BPCode.Buttons[1].Visible = false;
			bte_Calendar.Buttons[0].Visible = false;
			bte_Calendar.Buttons[1].Visible = false;
			lookup_Dept.DataSource = dataTable.Copy();
			txt_Department.Properties.DataSource = dataTable;
			await LoadResourceWorkCenter();
			await FocusedRowByResCode();
			gv_ResourceWorkCenter.OptionsView.ColumnAutoWidth = true;
			gv_ResourceWorkCenter.OptionsBehavior.ReadOnly = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task LoadResourceWorkCenter()
	{
		_ = 1;
		try
		{
			DataTable dataSource = await _httpClient.ResourceWorkCenter.GetDataResourceWorkCenter();
			grd_ResourceWorkCenter.DataSource = dataSource;
			FormMode = Enums.FormViewModeType.View;
			await gv_ResourceWorkCenter.ReloadFormSetting();
			_isSearch = false;
			_isAddNew = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task LoadData(Enums.NavigateType type)
	{
		try
		{
			DataTable dataSource = await _httpClient.ResourceWorkCenter.GetDataDoc(Function.ToString(resourceCode), type);
			grd_ResourceWorkCenter.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void Search()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ResCode");
			dataTable.Columns.Add("ResGroup");
			dataTable.Columns.Add("DeptCode");
			dataTable.Columns.Add("Valid");
			dataTable.Columns.Add("CardCode");
			dataTable.Columns.Add("CalCode");
			dataTable.AcceptChanges();
			DataRow dataRow = dataTable.NewRow();
			dataRow["ResCode"] = Function.ToString(txt_ResourceCode.EditValue);
			dataRow["ResGroup"] = Function.ToString(txt_ResGroup.EditValue);
			dataRow["DeptCode"] = Function.ToString(txt_Department.EditValue);
			dataRow["Valid"] = Function.ToString(txt_Status.EditValue);
			dataRow["CardCode"] = Function.ToString(txt_BPCode.EditValue);
			dataRow["CalCode"] = Function.ToString(txt_Calendar.EditValue);
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataTable dataSource = await _httpClient.ResourceWorkCenter.SearchResourceWorkCenter(dataTable);
			grd_ResourceWorkCenter.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewRow()
	{
		try
		{
			if (!_isAddNew)
			{
				FormMode = Enums.FormViewModeType.Update;
				gv_ResourceWorkCenter.AddNewRow();
				int focusedRowHandle = gv_ResourceWorkCenter.FocusedRowHandle;
				if (gv_ResourceWorkCenter.IsNewItemRow(focusedRowHandle))
				{
					gv_ResourceWorkCenter.SetRowCellValue(focusedRowHandle, gv_ResourceWorkCenter.Columns["Valid"], "A");
				}
			}
			_isAddNew = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task FocusedRowByResCode()
	{
		try
		{
			DataTable dataTable = await _httpClient.ResourceWorkCenter.GetDataWorkCenterByCode(resCode);
			if (dataTable.IsNotNull())
			{
				grd_ResourceWorkCenter.DataSource = dataTable;
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
			await LoadResourceWorkCenter();
			txt_ResourceCode.ResetText();
			txt_ResGroup.EditValue = null;
			txt_Status.EditValue = null;
			txt_Department.EditValue = null;
			txt_BPCode.ResetText();
			txt_Calendar.ResetText();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
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
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				WinCommons.Warning(Enums.MessageCode.ThisFunctionIsNotSupported, "");
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

	private void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons.Warning(Enums.MessageCode.ThisFunctionIsNotSupported, "");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_ResourceWorkCenter_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
	}

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.No)
			{
				DataTable dataTable = gv_ResourceWorkCenter.GetListDataSourceSelectedRow().CopyToDataTable();
				HttpResult httpResult = await _httpClient.ResourceWorkCenter.DeleteResource(dataTable.Copy());
				if (httpResult.Message == "")
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				await LoadResourceWorkCenter();
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
		Search();
		_isSearch = true;
	}

	private void gv_ResourceWorkCenter_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			resourceCode = Function.ToString(gv_ResourceWorkCenter.GetFocusedRowCellValue("ResCode"));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_ResourceWorkCenter_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			_ = gv_ResourceWorkCenter.FocusedColumn.FieldName == "ResCode";
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.RoutingAndResource.frm_ResourceWorkCenter));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		this.grd_ResourceWorkCenter = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_ResourceWorkCenter = new Apzon.Controls.Grid.GridViewEx();
		this.gcResCode = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_ResCode = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gcResName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResGroup = new Apzon.Controls.Grid.GridColumnEx();
		this.lookup_ResourceGroup = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.lookup_Dept = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcBpCode = new Apzon.Controls.Grid.GridColumnEx();
		this.bte_BPCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.lookup_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.bte_Calendar = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcBardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_Untilization = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gcResType = new Apzon.Controls.Grid.GridColumnEx();
		this.rResType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridViewEx1 = new Apzon.Controls.Grid.GridViewEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_Department = new Apzon.Controls.Editor.LookUpEditEx();
		this.txt_Status = new Apzon.Controls.Editor.LookUpEditEx();
		this.txt_ResGroup = new Apzon.Controls.Editor.LookUpEditEx();
		this.btSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.txt_ResourceCode = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.txt_Calendar = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.txt_BPCode = new Apzon.Forms.UserControls.ButtonEditCFL();
		((System.ComponentModel.ISupportInitialize)this.grd_ResourceWorkCenter).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_ResourceWorkCenter).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ResCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_ResourceGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_Dept).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_BPCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_Calendar).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Untilization).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rResType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Department.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ResGroup.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ResourceCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Calendar.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPCode.Properties).BeginInit();
		base.SuspendLayout();
		this.grd_ResourceWorkCenter.AllowPasteExcel = true;
		this.grd_ResourceWorkCenter.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_ResourceWorkCenter.ClipboardData = null;
		this.grd_ResourceWorkCenter.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_ResourceWorkCenter.Location = new System.Drawing.Point(0, 64);
		this.grd_ResourceWorkCenter.MainView = this.gv_ResourceWorkCenter;
		this.grd_ResourceWorkCenter.Name = "grd_ResourceWorkCenter";
		this.grd_ResourceWorkCenter.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.lookup_Status, this.lookup_ResourceGroup, this.txt_Untilization, this.lookup_Dept, this.bte_Calendar, this.bte_BPCode, this.txt_ResCode, this.rResType });
		this.grd_ResourceWorkCenter.Size = new System.Drawing.Size(775, 300);
		this.grd_ResourceWorkCenter.TabIndex = 1;
		this.grd_ResourceWorkCenter.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gv_ResourceWorkCenter, this.gridViewEx1 });
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
		this.gv_ResourceWorkCenter.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_ResourceWorkCenter.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_ResourceWorkCenter.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ResourceWorkCenter.ColumnKey = null;
		this.gv_ResourceWorkCenter.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.gcResCode, this.gcResName, this.gcResGroup, this.gridColumnEx4, this.gcBpCode, this.gridColumnEx6, this.gridColumnEx7, this.gcBardCode, this.gridColumnEx9, this.gcResType });
		this.gv_ResourceWorkCenter.DisableFormSetting = false;
		this.gv_ResourceWorkCenter.DisplayException = false;
		this.gv_ResourceWorkCenter.EditColumn = null;
		this.gv_ResourceWorkCenter.FieldCurrency = null;
		this.gv_ResourceWorkCenter.FieldRate = null;
		this.gv_ResourceWorkCenter.FormId = "WorkCenter";
		this.gv_ResourceWorkCenter.GridControl = this.grd_ResourceWorkCenter;
		this.gv_ResourceWorkCenter.IndexEditRow = -1;
		this.gv_ResourceWorkCenter.IndicatorWidth = 35;
		this.gv_ResourceWorkCenter.IsApzTable = false;
		this.gv_ResourceWorkCenter.IsInsertNewRow = false;
		this.gv_ResourceWorkCenter.IsLoadingData = false;
		this.gv_ResourceWorkCenter.ItemId = "APZ_ORSC";
		this.gv_ResourceWorkCenter.LstColumNotSetting = null;
		this.gv_ResourceWorkCenter.Name = "gv_ResourceWorkCenter";
		this.gv_ResourceWorkCenter.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_ResourceWorkCenter.OptionsSelection.MultiSelect = true;
		this.gv_ResourceWorkCenter.OptionsView.BestFitMaxRowCount = 100;
		this.gv_ResourceWorkCenter.OptionsView.ColumnAutoWidth = false;
		this.gv_ResourceWorkCenter.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_ResourceWorkCenter.OptionsView.ShowGroupPanel = false;
		this.gv_ResourceWorkCenter.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_ResourceWorkCenter_ShowingEditor);
		this.gv_ResourceWorkCenter.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_ResourceWorkCenter_FocusedRowObjectChanged);
		this.gv_ResourceWorkCenter.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_ResourceWorkCenter_CellValueChanged);
		this.gcResCode.Caption = "Resource No";
		this.gcResCode.ColumnEdit = this.txt_ResCode;
		this.gcResCode.CurrencyDateField = null;
		this.gcResCode.CurrencyField = null;
		this.gcResCode.CurrencyFixed = null;
		this.gcResCode.CurrencyFixedRate = 0.0;
		this.gcResCode.CurrencyRateField = null;
		this.gcResCode.FieldName = "ResCode";
		this.gcResCode.Name = "gcResCode";
		this.gcResCode.PrimaryKey = true;
		this.gcResCode.Visible = true;
		this.gcResCode.VisibleIndex = 0;
		this.txt_ResCode.AutoHeight = false;
		this.txt_ResCode.Name = "txt_ResCode";
		this.gcResName.Caption = "Resource Name";
		this.gcResName.CurrencyDateField = null;
		this.gcResName.CurrencyField = null;
		this.gcResName.CurrencyFixed = null;
		this.gcResName.CurrencyFixedRate = 0.0;
		this.gcResName.CurrencyRateField = null;
		this.gcResName.FieldName = "ResName";
		this.gcResName.Name = "gcResName";
		this.gcResName.PrimaryKey = false;
		this.gcResName.Visible = true;
		this.gcResName.VisibleIndex = 1;
		this.gcResGroup.Caption = "Resource Group";
		this.gcResGroup.ColumnEdit = this.lookup_ResourceGroup;
		this.gcResGroup.CurrencyDateField = null;
		this.gcResGroup.CurrencyField = null;
		this.gcResGroup.CurrencyFixed = null;
		this.gcResGroup.CurrencyFixedRate = 0.0;
		this.gcResGroup.CurrencyRateField = null;
		this.gcResGroup.FieldName = "ResGrpCod";
		this.gcResGroup.Name = "gcResGroup";
		this.gcResGroup.PrimaryKey = false;
		this.gcResGroup.Visible = true;
		this.gcResGroup.VisibleIndex = 2;
		this.lookup_ResourceGroup.AutoHeight = false;
		this.lookup_ResourceGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookup_ResourceGroup.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ResGrpNam", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ResGrpCod", "Code")
		});
		this.lookup_ResourceGroup.DisplayMember = "ResGrpNam";
		this.lookup_ResourceGroup.Name = "lookup_ResourceGroup";
		this.lookup_ResourceGroup.NullText = "";
		this.lookup_ResourceGroup.ValueMember = "ResGrpCod";
		this.gridColumnEx4.Caption = "Dept";
		this.gridColumnEx4.ColumnEdit = this.lookup_Dept;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "DeptCode";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.lookup_Dept.AutoHeight = false;
		this.lookup_Dept.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookup_Dept.DisplayMember = "Name";
		this.lookup_Dept.Name = "lookup_Dept";
		this.lookup_Dept.NullText = "";
		this.lookup_Dept.ValueMember = "DeptCode";
		this.gcBpCode.Caption = "BP Code";
		this.gcBpCode.ColumnEdit = this.bte_BPCode;
		this.gcBpCode.CurrencyDateField = null;
		this.gcBpCode.CurrencyField = null;
		this.gcBpCode.CurrencyFixed = null;
		this.gcBpCode.CurrencyFixedRate = 0.0;
		this.gcBpCode.CurrencyRateField = null;
		this.gcBpCode.FieldName = "CardCode";
		this.gcBpCode.Name = "gcBpCode";
		this.gcBpCode.PrimaryKey = false;
		this.gcBpCode.Visible = true;
		this.gcBpCode.VisibleIndex = 3;
		this.bte_BPCode.AutoHeight = false;
		this.bte_BPCode.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.bte_BPCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions)
		});
		this.bte_BPCode.ContainerControl = this;
		this.bte_BPCode.DisplayMember = "CardCode";
		this.bte_BPCode.DisplayMember2 = "";
		this.bte_BPCode.LoadDataSourceWhenEnter = false;
		this.bte_BPCode.Name = "bte_BPCode";
		this.bte_BPCode.SeparateCharacterForMulti = ';';
		this.bte_BPCode.SeparateString = ", ";
		this.bte_BPCode.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Vendor;
		this.bte_BPCode.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oBusinessPartners;
		this.bte_BPCode.UsingAutoCompleteBox = false;
		this.bte_BPCode.UsingDataSource = true;
		this.bte_BPCode.ValidateWhenNoDataSource = false;
		this.bte_BPCode.ValueMember = "CardCode";
		this.gridColumnEx6.Caption = "Status";
		this.gridColumnEx6.ColumnEdit = this.lookup_Status;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "Valid";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.lookup_Status.AutoHeight = false;
		this.lookup_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookup_Status.Name = "lookup_Status";
		this.lookup_Status.NullText = "";
		this.gridColumnEx7.Caption = "Calendar";
		this.gridColumnEx7.ColumnEdit = this.bte_Calendar;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "CalCode";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.bte_Calendar.AutoHeight = false;
		this.bte_Calendar.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions10.Image");
		this.bte_Calendar.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2)
		});
		this.bte_Calendar.ContainerControl = this;
		this.bte_Calendar.DisplayMember = "CalCode";
		this.bte_Calendar.DisplayMember2 = "";
		this.bte_Calendar.LoadDataSourceWhenEnter = false;
		this.bte_Calendar.Name = "bte_Calendar";
		this.bte_Calendar.SeparateCharacterForMulti = ';';
		this.bte_Calendar.SeparateString = ", ";
		this.bte_Calendar.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.WorkSchedule;
		this.bte_Calendar.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oWorkSchedule;
		this.bte_Calendar.UsingAutoCompleteBox = false;
		this.bte_Calendar.UsingDataSource = true;
		this.bte_Calendar.ValidateWhenNoDataSource = false;
		this.bte_Calendar.ValueMember = "CalCode";
		this.gcBardCode.Caption = "Bar Code";
		this.gcBardCode.CurrencyDateField = null;
		this.gcBardCode.CurrencyField = null;
		this.gcBardCode.CurrencyFixed = null;
		this.gcBardCode.CurrencyFixedRate = 0.0;
		this.gcBardCode.CurrencyRateField = null;
		this.gcBardCode.FieldName = "BarCode";
		this.gcBardCode.Name = "gcBardCode";
		this.gcBardCode.PrimaryKey = false;
		this.gcBardCode.Visible = true;
		this.gcBardCode.VisibleIndex = 4;
		this.gridColumnEx9.Caption = "Untilization";
		this.gridColumnEx9.ColumnEdit = this.txt_Untilization;
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Percent;
		this.gridColumnEx9.FieldName = "Untilization";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.txt_Untilization.AutoHeight = false;
		this.txt_Untilization.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
		this.txt_Untilization.Name = "txt_Untilization";
		this.gcResType.Caption = "Resource Type";
		this.gcResType.ColumnEdit = this.rResType;
		this.gcResType.CurrencyDateField = null;
		this.gcResType.CurrencyField = null;
		this.gcResType.CurrencyFixed = null;
		this.gcResType.CurrencyFixedRate = 0.0;
		this.gcResType.CurrencyRateField = null;
		this.gcResType.FieldName = "ResType";
		this.gcResType.Name = "gcResType";
		this.gcResType.PrimaryKey = false;
		this.gcResType.Visible = true;
		this.gcResType.VisibleIndex = 5;
		this.rResType.AutoHeight = false;
		this.rResType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rResType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rResType.DisplayMember = "Name";
		this.rResType.Name = "rResType";
		this.rResType.NullText = "";
		this.rResType.ShowHeader = false;
		this.rResType.ValueMember = "Code";
		this.gridViewEx1.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gridViewEx1.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gridViewEx1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridViewEx1.ColumnKey = null;
		this.gridViewEx1.DisableFormSetting = false;
		this.gridViewEx1.DisplayException = false;
		this.gridViewEx1.EditColumn = null;
		this.gridViewEx1.FieldCurrency = null;
		this.gridViewEx1.FieldRate = null;
		this.gridViewEx1.FormId = "WorkCenter";
		this.gridViewEx1.GridControl = this.grd_ResourceWorkCenter;
		this.gridViewEx1.IndexEditRow = -1;
		this.gridViewEx1.IndicatorWidth = 35;
		this.gridViewEx1.IsApzTable = false;
		this.gridViewEx1.IsInsertNewRow = false;
		this.gridViewEx1.IsLoadingData = false;
		this.gridViewEx1.ItemId = "OGRID1";
		this.gridViewEx1.LstColumNotSetting = null;
		this.gridViewEx1.Name = "gridViewEx1";
		this.gridViewEx1.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gridViewEx1.OptionsView.BestFitMaxRowCount = 100;
		this.gridViewEx1.OptionsView.ColumnAutoWidth = false;
		this.gridViewEx1.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridViewEx1.OptionsView.ShowGroupPanel = false;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(8, 370);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 7;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(89, 370);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 8;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.txt_Department.FieldName = null;
		this.txt_Department.IsSearchControl = false;
		this.txt_Department.Location = new System.Drawing.Point(307, 11);
		this.txt_Department.Name = "txt_Department";
		this.txt_Department.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_Department.Properties.DisplayMember = "Name";
		this.txt_Department.Properties.NullText = "";
		this.txt_Department.Properties.ValueMember = "DeptCode";
		this.txt_Department.Size = new System.Drawing.Size(100, 20);
		this.txt_Department.TabIndex = 11;
		this.txt_Status.FieldName = null;
		this.txt_Status.IsSearchControl = false;
		this.txt_Status.Location = new System.Drawing.Point(307, 37);
		this.txt_Status.Name = "txt_Status";
		this.txt_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_Status.Properties.NullText = "";
		this.txt_Status.Size = new System.Drawing.Size(100, 20);
		this.txt_Status.TabIndex = 12;
		this.txt_ResGroup.FieldName = null;
		this.txt_ResGroup.IsSearchControl = false;
		this.txt_ResGroup.Location = new System.Drawing.Point(108, 37);
		this.txt_ResGroup.Name = "txt_ResGroup";
		this.txt_ResGroup.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_ResGroup.Properties.DisplayMember = "ResGrpNam";
		this.txt_ResGroup.Properties.NullText = "";
		this.txt_ResGroup.Properties.ValueMember = "ResGrpNam";
		this.txt_ResGroup.Size = new System.Drawing.Size(100, 20);
		this.txt_ResGroup.TabIndex = 14;
		this.btSearch.Appearance.Font = new System.Drawing.Font("Tahoma", 7.5f);
		this.btSearch.Appearance.Options.UseFont = true;
		this.btSearch.FieldName = null;
		this.btSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btSearch.ImageOptions.Image");
		this.btSearch.IsSearchControl = false;
		this.btSearch.Location = new System.Drawing.Point(609, 38);
		this.btSearch.Name = "btSearch";
		this.btSearch.Size = new System.Drawing.Size(93, 19);
		this.btSearch.TabIndex = 59;
		this.btSearch.Text = "Search";
		this.btSearch.Click += new System.EventHandler(this.btSearch_Click);
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(416, 37);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(84, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 62;
		this.labelLine3.Utext = "Calendar";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(416, 13);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(84, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 63;
		this.labelLine4.Utext = "BP Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(220, 37);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(84, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 64;
		this.labelLine1.Utext = "Status";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(220, 11);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(84, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 65;
		this.labelLine2.Utext = "Department";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(22, 37);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(84, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 66;
		this.labelLine5.Utext = "Resource Group";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(22, 13);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(84, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 67;
		this.labelLine6.Utext = "Resource Code";
		this.txt_ResourceCode.FieldName = null;
		this.txt_ResourceCode.IsSearchControl = false;
		this.txt_ResourceCode.Location = new System.Drawing.Point(108, 11);
		this.txt_ResourceCode.Name = "txt_ResourceCode";
		this.txt_ResourceCode.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions8.Image");
		this.txt_ResourceCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, false, true, editorButtonImageOptions3)
		});
		this.txt_ResourceCode.Properties.ContainerControl = null;
		this.txt_ResourceCode.Properties.DisplayMember = "ResCode";
		this.txt_ResourceCode.Properties.DisplayMember2 = "";
		this.txt_ResourceCode.Properties.LoadDataSourceWhenEnter = false;
		this.txt_ResourceCode.Properties.SeparateCharacterForMulti = ';';
		this.txt_ResourceCode.Properties.SeparateString = ", ";
		this.txt_ResourceCode.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ListWorkCenter;
		this.txt_ResourceCode.Properties.UsingAutoCompleteBox = false;
		this.txt_ResourceCode.Properties.UsingDataSource = true;
		this.txt_ResourceCode.Properties.ValidateWhenNoDataSource = false;
		this.txt_ResourceCode.Properties.ValueMember = "ResCode";
		this.txt_ResourceCode.Size = new System.Drawing.Size(100, 20);
		this.txt_ResourceCode.TabIndex = 68;
		this.txt_Calendar.FieldName = null;
		this.txt_Calendar.IsSearchControl = false;
		this.txt_Calendar.Location = new System.Drawing.Point(502, 37);
		this.txt_Calendar.Name = "txt_Calendar";
		this.txt_Calendar.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions4.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions6.Image");
		this.txt_Calendar.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, false, true, editorButtonImageOptions4)
		});
		this.txt_Calendar.Properties.ContainerControl = null;
		this.txt_Calendar.Properties.DisplayMember = "CalCode";
		this.txt_Calendar.Properties.DisplayMember2 = "";
		this.txt_Calendar.Properties.LoadDataSourceWhenEnter = false;
		this.txt_Calendar.Properties.SeparateCharacterForMulti = ';';
		this.txt_Calendar.Properties.SeparateString = ", ";
		this.txt_Calendar.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.WorkSchedule;
		this.txt_Calendar.Properties.UsingAutoCompleteBox = false;
		this.txt_Calendar.Properties.UsingDataSource = true;
		this.txt_Calendar.Properties.ValidateWhenNoDataSource = false;
		this.txt_Calendar.Properties.ValueMember = "CalCode";
		this.txt_Calendar.Size = new System.Drawing.Size(100, 20);
		this.txt_Calendar.TabIndex = 69;
		this.txt_BPCode.FieldName = null;
		this.txt_BPCode.IsSearchControl = false;
		this.txt_BPCode.Location = new System.Drawing.Point(502, 11);
		this.txt_BPCode.Name = "txt_BPCode";
		this.txt_BPCode.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions5.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.txt_BPCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, false, true, editorButtonImageOptions5)
		});
		this.txt_BPCode.Properties.ContainerControl = null;
		this.txt_BPCode.Properties.DisplayMember = "CardCode";
		this.txt_BPCode.Properties.DisplayMember2 = "";
		this.txt_BPCode.Properties.LoadDataSourceWhenEnter = false;
		this.txt_BPCode.Properties.SeparateCharacterForMulti = ';';
		this.txt_BPCode.Properties.SeparateString = ", ";
		this.txt_BPCode.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Vendor;
		this.txt_BPCode.Properties.UsingAutoCompleteBox = false;
		this.txt_BPCode.Properties.UsingDataSource = true;
		this.txt_BPCode.Properties.ValidateWhenNoDataSource = false;
		this.txt_BPCode.Properties.ValueMember = "CardCode";
		this.txt_BPCode.Size = new System.Drawing.Size(100, 20);
		this.txt_BPCode.TabIndex = 70;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(776, 401);
		this.ContextMenuStrip = this.contextMenuStrip1;
		base.Controls.Add(this.txt_BPCode);
		base.Controls.Add(this.txt_Calendar);
		base.Controls.Add(this.txt_ResourceCode);
		base.Controls.Add(this.labelLine6);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.btSearch);
		base.Controls.Add(this.txt_ResGroup);
		base.Controls.Add(this.txt_Status);
		base.Controls.Add(this.txt_Department);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.grd_ResourceWorkCenter);
		base.FormId = "WorkCenter";
		base.Name = "frm_ResourceWorkCenter";
		base.ShowIcon = false;
		this.Text = "Resource Work Center";
		((System.ComponentModel.ISupportInitialize)this.grd_ResourceWorkCenter).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_ResourceWorkCenter).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ResCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_ResourceGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_Dept).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_BPCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_Calendar).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Untilization).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rResType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Department.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ResGroup.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ResourceCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Calendar.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPCode.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
