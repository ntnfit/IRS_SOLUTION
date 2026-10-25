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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.DMS;

public class frm_AlertTemplate : XtraFormEx
{
	private enum UploadStatus
	{
		Waiting,
		Success,
		Fail
	}

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isLoading;

	private IContainer components;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	private ContextMenuStrip ctx_Doc;

	private ToolStripMenuItem ctm_AddNew;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridColumnEx gcCode;

	private GridColumnEx gcName;

	private WysiwygEdit wysiwygEdit1;

	private GridColumnEx gcActive;

	private RepositoryItemCheckEdit chk_Active;

	private ToolStripMenuItem ctm_Remove;

	public int AbsEntry { get; set; }

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
		case Enums.FormViewModeType.Add:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			gvDoc.Columns["Code"].OptionsColumn.ReadOnly = true;
			break;
		}
	}

	public frm_AlertTemplate()
	{
		InitializeComponent();
		OnLoadAsync += frm_AlertTemplate_Load;
	}

	private async Task frm_AlertTemplate_Load(object sender, EventArgs e)
	{
		try
		{
			_isLoading = true;
			wysiwygEdit1.richEditor.ModifiedChanged += RichEditor_ModifiedChanged;
			await LoadData();
			FormMode = Enums.FormViewModeType.View;
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void RichEditor_ModifiedChanged(object sender, EventArgs e)
	{
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private async Task LoadData()
	{
		try
		{
			DataTable dataSource = await _httpClient.AlertTemplate.GetAllTemplate();
			grdDoc.DataSource = dataSource;
			grdDoc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if ((FormMode != Enums.FormViewModeType.Update && FormMode != Enums.FormViewModeType.Add) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await frm_AlertTemplate_Load(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH()
	{
		try
		{
			if ((FormMode != Enums.FormViewModeType.Update && FormMode != Enums.FormViewModeType.Add) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				FormMode = Enums.FormViewModeType.View;
				DataTable dataTable = grdDoc.DatasourceTable();
				dataTable.Rows.Clear();
				DataRow dataRow = dataTable.NewRow();
				dataRow["Active"] = "Y";
				dataRow["UserSign"] = GlobalData.UserID;
				dataTable.Rows.Add(dataRow);
				gvDoc.Columns["Code"].OptionsColumn.ReadOnly = false;
				FormMode = Enums.FormViewModeType.Add;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> SaveData(Enums.FormViewModeType formViewMode)
	{
		try
		{
			DataTable dataTable = gvDoc.GetListDataSourceSelectedRow().CopyToDataTable();
			dataTable.Rows[0]["UserSign2"] = GlobalData.UserID;
			dataTable.Rows[0]["Text"] = wysiwygEdit1.richEditor.Text;
			dataTable.Rows[0]["HtmlText"] = wysiwygEdit1.richEditor.HtmlText;
			HttpResult httpResult = await _httpClient.AlertTemplate.SaveData(new Tuple<DataTable, string>(dataTable, (formViewMode == Enums.FormViewModeType.Add) ? "A" : "U"));
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ex.Message;
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			string currentTemplate = Function.ToString(gvDoc.GetFocusedRowCellValue("Code"));
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBox.Show((FormMode == Enums.FormViewModeType.Add) ? "Do you want to save?" : "Do you want to save changes?", "System Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				btn_OK.Enabled = false;
				string text = await SaveData(FormMode);
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
					btn_OK.Enabled = true;
					return;
				}
				FormMode = Enums.FormViewModeType.View;
				await LoadData();
				int num = gvDoc.LocateByValue("Code", currentTemplate);
				if (num != int.MinValue)
				{
					gvDoc.FocusedRowHandle = num;
					gvDoc.MakeRowVisible(num);
				}
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
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

	private void frm_AlertTemplate_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.Add)
		{
			e.Cancel = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes;
		}
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
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

	private void gvDoc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			DataRow focusedDataRow = gvDoc.GetFocusedDataRow();
			if (focusedDataRow != null)
			{
				focusedDataRow.AcceptChanges();
				wysiwygEdit1.richEditor.DataBindings.Clear();
				wysiwygEdit1.richEditor.DataBindings.Add("Text", focusedDataRow.ToArray().CopyToDataTable(), "Text", formattingEnabled: false, DataSourceUpdateMode.OnValidation);
				wysiwygEdit1.richEditor.DataBindings.Add("HtmlText", focusedDataRow.ToArray().CopyToDataTable(), "HtmlText", formattingEnabled: false, DataSourceUpdateMode.OnValidation);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ctm_AddNew_Click(object sender, EventArgs e)
	{
		REFRESH();
	}

	private async void ctm_Remove_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.View && XtraMessageBox.Show("Do you want to remove this template?", "System Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				Tuple<string> tupData = new Tuple<string>(Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcCode.FieldName)));
				HttpResult httpResult = await _httpClient.AlertTemplate.Remove(tupData);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					return;
				}
				WinCommons.Success(Enums.MessageCode.Success, "");
				await LoadData();
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.Add)
			{
				bool flag = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
				if (flag)
				{
					grdDoc.DatasourceTable().RejectChanges();
					FormMode = Enums.FormViewModeType.View;
				}
				e.Allow = flag;
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
		this.ctx_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.ctm_AddNew = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_Remove = new System.Windows.Forms.ToolStripMenuItem();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcActive = new Apzon.Controls.Grid.GridColumnEx();
		this.chk_Active = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.wysiwygEdit1 = new Apzon.Forms.UserControls.WysiwygEdit();
		this.ctx_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Active).BeginInit();
		base.SuspendLayout();
		this.ctx_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.ctm_AddNew, this.ctm_Remove });
		this.ctx_Doc.Name = "ctx_Doc";
		this.ctx_Doc.Size = new System.Drawing.Size(129, 48);
		this.ctm_AddNew.Name = "ctm_AddNew";
		this.ctm_AddNew.Size = new System.Drawing.Size(128, 22);
		this.ctm_AddNew.Text = "Thêm mới";
		this.ctm_AddNew.Click += new System.EventHandler(this.ctm_AddNew_Click);
		this.ctm_Remove.Name = "ctm_Remove";
		this.ctm_Remove.Size = new System.Drawing.Size(128, 22);
		this.ctm_Remove.Text = "Xóa";
		this.ctm_Remove.Click += new System.EventHandler(this.ctm_Remove_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(83, 482);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 11;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(2, 482);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 10;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, 3);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grdDoc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.wysiwygEdit1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1133, 473);
		this.splitContainerControl1.SplitterPosition = 362;
		this.splitContainerControl1.TabIndex = 79;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.ctx_Doc;
		this.grdDoc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDoc.Location = new System.Drawing.Point(0, 0);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.chk_Active });
		this.grdDoc.Size = new System.Drawing.Size(362, 473);
		this.grdDoc.TabIndex = 77;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gcCode, this.gcName, this.gcActive });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oPosAssignment";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "OUSR";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvDoc_FocusedRowObjectChanged);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gvDoc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gvDoc_BeforeLeaveRow);
		this.gcCode.Caption = "Mã";
		this.gcCode.CurrencyDateField = null;
		this.gcCode.CurrencyField = null;
		this.gcCode.CurrencyFixed = null;
		this.gcCode.CurrencyFixedRate = 0.0;
		this.gcCode.CurrencyRateField = null;
		this.gcCode.FieldName = "Code";
		this.gcCode.Name = "gcCode";
		this.gcCode.OptionsColumn.ReadOnly = true;
		this.gcCode.PrimaryKey = true;
		this.gcCode.UnitField = null;
		this.gcCode.Visible = true;
		this.gcCode.VisibleIndex = 0;
		this.gcCode.Width = 125;
		this.gcName.Caption = "Tên";
		this.gcName.CurrencyDateField = null;
		this.gcName.CurrencyField = null;
		this.gcName.CurrencyFixed = null;
		this.gcName.CurrencyFixedRate = 0.0;
		this.gcName.CurrencyRateField = null;
		this.gcName.FieldName = "Name";
		this.gcName.Name = "gcName";
		this.gcName.PrimaryKey = false;
		this.gcName.UnitField = null;
		this.gcName.Visible = true;
		this.gcName.VisibleIndex = 1;
		this.gcName.Width = 372;
		this.gcActive.Caption = "Kích hoạt";
		this.gcActive.ColumnEdit = this.chk_Active;
		this.gcActive.CurrencyDateField = null;
		this.gcActive.CurrencyField = null;
		this.gcActive.CurrencyFixed = null;
		this.gcActive.CurrencyFixedRate = 0.0;
		this.gcActive.CurrencyRateField = null;
		this.gcActive.FieldName = "Active";
		this.gcActive.Name = "gcActive";
		this.gcActive.PrimaryKey = false;
		this.gcActive.UnitField = null;
		this.gcActive.Visible = true;
		this.gcActive.VisibleIndex = 2;
		this.gcActive.Width = 176;
		this.chk_Active.AutoHeight = false;
		this.chk_Active.Name = "chk_Active";
		this.chk_Active.ValueChecked = "Y";
		this.chk_Active.ValueUnchecked = "N";
		this.wysiwygEdit1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.wysiwygEdit1.FieldName = null;
		this.wysiwygEdit1.Location = new System.Drawing.Point(0, 0);
		this.wysiwygEdit1.Name = "wysiwygEdit1";
		this.wysiwygEdit1.Size = new System.Drawing.Size(761, 473);
		this.wysiwygEdit1.TabIndex = 135;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1135, 510);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.FormId = "ofrm_AlertTemplate";
		base.FormObjectType = Apzon.Commons.ApzObjectTypes.oKPIsSetting;
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_AlertTemplate";
		this.Text = "Mẫu thông báo";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_AlertTemplate_FormClosing);
		this.ctx_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Active).EndInit();
		base.ResumeLayout(false);
	}
}
