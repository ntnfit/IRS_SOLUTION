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
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Interfaces.FormDocument;

public class frm_UserDefinedTexts : XtraFormEx
{
	public bool OpenToChoose;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private GridControlEx grd_Data;

	public GridViewEx gv_Data;

	private ButtonEditEx txt_Search;

	private LabelLine lb_Search;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	public DataTable DtStruct { get; set; }

	public List<DataRow> Changed { get; set; }

	public List<DataRow> ChooseItems { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
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
			{
				SimpleButtonEx simpleButtonEx4 = btn_OK;
				bool enabled = (btn_Cancel.Enabled = true);
				simpleButtonEx4.Enabled = enabled;
				btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
				WinCommons._CloseProcessing();
				break;
			}
			case Enums.FormViewModeType.Update:
			{
				SimpleButtonEx simpleButtonEx3 = btn_OK;
				bool enabled = (btn_Cancel.Enabled = true);
				simpleButtonEx3.Enabled = enabled;
				btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
				WinCommons._CloseProcessing();
				break;
			}
			case Enums.FormViewModeType.View:
			{
				SimpleButtonEx simpleButtonEx2 = btn_OK;
				bool enabled = (btn_Cancel.Enabled = true);
				simpleButtonEx2.Enabled = enabled;
				WinCommons._CloseProcessing();
				btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
				break;
			}
			case Enums.FormViewModeType.Busy:
			{
				SimpleButtonEx simpleButtonEx = btn_OK;
				bool enabled = (btn_Cancel.Enabled = false);
				simpleButtonEx.Enabled = enabled;
				WinCommons._OpenProcessing();
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public frm_UserDefinedTexts()
	{
		InitializeComponent();
	}

	private void txt_Search_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string activeFilterString = string.Format("TextCode LIKE '%{0}%' OR Text LIKE '%{0}%' ", txt_Search.Text);
			gv_Data.ActiveFilterString = activeFilterString;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_UserDefinedTexts_Load(object sender, EventArgs e)
	{
		try
		{
			gv_Data.DisplayException = true;
			ChooseItems = new List<DataRow>();
			DtStruct = new DataTable();
			DtStruct.Columns.Add("AbsEntry", typeof(int));
			DtStruct.Columns.Add("TextCode", typeof(string));
			DtStruct.Columns.Add("Text", typeof(string));
			await Search();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Search()
	{
		try
		{
			DataTable dataTable = await _httpClient.UserDefinedTexts.GetAll();
			if (!dataTable.IsNotNull())
			{
				dataTable = DtStruct.Copy();
			}
			grd_Data.DataSource = dataTable;
			grd_Data.RefreshDataSource();
			NewRow();
			FormMode = Enums.FormViewModeType.View;
			Changed = new List<DataRow>();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddChooseItemsGrid()
	{
		try
		{
			if (gv_Data.GetSelectedRows() != null && gv_Data.GetSelectedRows().Length != 0)
			{
				int[] selectedRows = gv_Data.GetSelectedRows();
				foreach (int rowHandle in selectedRows)
				{
					DataRow dataRow = gv_Data.GetDataRow(rowHandle);
					ChooseItems.Add(dataRow);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewRow()
	{
		try
		{
			DataTable dataTable = grd_Data.DataSourceTable;
			if (!dataTable.IsNotNull())
			{
				dataTable = DtStruct.Copy();
			}
			if (dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["TextCode"]))) == null)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["AbsEntry"] = -1;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grd_Data.DataSource = dataTable;
				grd_Data.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Data_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			string textcode = Function.ToString(gv_Data.GetFocusedRowCellValue("TextCode"));
			if (e.Column.FieldName.Equals("TextCode") && !string.IsNullOrEmpty(textcode))
			{
				NewRow();
			}
			DataRow dataRow = gv_Data.GetDataRow(e.RowHandle);
			if (dataRow != null)
			{
				DataRow dataRow2 = Changed.FirstOrDefault((DataRow t) => Function.ToString(t["TextCode"]) == textcode);
				if (dataRow2 != null)
				{
					Changed.Remove(dataRow2);
				}
				Changed.Add(dataRow);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		Enums.FormViewModeType currMode = FormMode;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				AddChooseItemsGrid();
				Close();
			}
			else if (Changed.Count > 0)
			{
				FormMode = Enums.FormViewModeType.Busy;
				string text = await _httpClient.UserDefinedTexts.Save(Changed.CopyToDataTable());
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					await Search();
					return;
				}
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = currMode;
	}

	private void gv_Data_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			if (OpenToChoose && FormMode == Enums.FormViewModeType.View)
			{
				AddChooseItemsGrid();
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Data.GetFocusedRowCellValue("AbsEntry"));
			removeToolStripMenuItem.Enabled = num > 0;
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
			int num = Function.ParseInt(gv_Data.GetFocusedRowCellValue("AbsEntry"));
			if (num > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveItem, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				string text = await _httpClient.UserDefinedTexts.Remove(num);
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
					return;
				}
				WinCommons.Success(Enums.MessageCode.Success, "");
				await Search();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Data_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			e.ErrorText = "";
			e.Valid = true;
			if (gv_Data.FocusedColumn == gv_Data.Columns["TextCode"])
			{
				string textcode = Function.ToString(e.Value);
				if (!string.IsNullOrEmpty(textcode) && grd_Data.DataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["TextCode"]) == textcode) != null)
				{
					e.ErrorText = "This item already exists";
					e.Valid = false;
				}
			}
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
			if (FormMode == Enums.FormViewModeType.View || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.Cancel)
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Interfaces.FormDocument.frm_UserDefinedTexts));
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.grd_Data = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Data = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.txt_Search = new Apzon.Controls.Editor.ButtonEditEx();
		this.lb_Search = new Apzon.Controls.Controls.LabelLine();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Data).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Data).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Search.Properties).BeginInit();
		base.SuspendLayout();
		this.grd_Data.AllowPasteExcel = true;
		this.grd_Data.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Data.ClipboardData = null;
		this.grd_Data.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Data.Location = new System.Drawing.Point(1, 33);
		this.grd_Data.MainView = this.gv_Data;
		this.grd_Data.Name = "grd_Data";
		this.grd_Data.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemMemoExEdit1 });
		this.grd_Data.Size = new System.Drawing.Size(580, 347);
		this.grd_Data.TabIndex = 48;
		this.grd_Data.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Data });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
		this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_Data.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Data.ColumnKey = null;
		this.gv_Data.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gridColumnEx1, this.gridColumnEx2 });
		this.gv_Data.DisableFormSetting = false;
		this.gv_Data.DisplayException = false;
		this.gv_Data.EditColumn = null;
		this.gv_Data.FieldCurrency = null;
		this.gv_Data.FieldRate = null;
		this.gv_Data.FormId = "oUserDefinedText";
		this.gv_Data.GridControl = this.grd_Data;
		this.gv_Data.IgnoreFilter = false;
		this.gv_Data.IndexEditRow = -1;
		this.gv_Data.IndicatorWidth = 60;
		this.gv_Data.IsApzTable = false;
		this.gv_Data.IsInsertNewRow = false;
		this.gv_Data.IsLoadingData = false;
		this.gv_Data.IsMappingContextAction = false;
		this.gv_Data.ItemId = "ODATA";
		this.gv_Data.LstColumnAllowBatch = null;
		this.gv_Data.LstColumnNotAllowBatch = null;
		this.gv_Data.LstColumnNotEditable = null;
		this.gv_Data.LstColumNotSetting = null;
		this.gv_Data.Name = "gv_Data";
		this.gv_Data.OptionsSelection.MultiSelect = true;
		this.gv_Data.OptionsView.ColumnAutoWidth = false;
		this.gv_Data.OptionsView.EnableAppearanceEvenRow = true;
		this.gv_Data.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.gv_Data.UseBatchMod = false;
		this.gv_Data.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Data_CellValueChanged);
		this.gv_Data.DoubleClick += new System.EventHandler(this.gv_Data_DoubleClick);
		this.gv_Data.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_Data_ValidatingEditor);
		this.gridColumnEx1.Caption = "Code";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "TextCode";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx1.Width = 112;
		this.gridColumnEx2.Caption = "Text";
		this.gridColumnEx2.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "Text";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx2.Width = 405;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.repositoryItemMemoExEdit1.SuppressMouseEventOnOuterMouseClick = true;
		this.txt_Search.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Search.EditControl = null;
		this.txt_Search.FieldName = null;
		this.txt_Search.IsSearchControl = false;
		this.txt_Search.Location = new System.Drawing.Point(76, 5);
		this.txt_Search.Name = "txt_Search";
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.txt_Search.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_Search.SettingInVisible = false;
		this.txt_Search.SettingReadonly = false;
		this.txt_Search.Size = new System.Drawing.Size(337, 24);
		this.txt_Search.TabIndex = 47;
		this.txt_Search.ToolTip = "Search by Item code";
		this.txt_Search.EditValueChanged += new System.EventHandler(this.txt_Search_EditValueChanged);
		this.lb_Search.EditControl = null;
		this.lb_Search.EditValue = "Search";
		this.lb_Search.FieldName = null;
		this.lb_Search.HideWhenEmpty = false;
		this.lb_Search.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Search.Location = new System.Drawing.Point(1, 10);
		this.lb_Search.Name = "lb_Search";
		this.lb_Search.ReadOnly = false;
		this.lb_Search.SettingInVisible = false;
		this.lb_Search.SettingReadonly = false;
		this.lb_Search.ShowUnderLine = true;
		this.lb_Search.Size = new System.Drawing.Size(80, 20);
		this.lb_Search.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Search.TabIndex = 49;
		this.lb_Search.Utext = "Search";
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(8, 387);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 50;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(89, 387);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 51;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AcceptButton = this.btn_OK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(580, 417);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.grd_Data);
		base.Controls.Add(this.txt_Search);
		base.Controls.Add(this.lb_Search);
		base.FormId = "oUserDefinedText";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_UserDefinedTexts.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_UserDefinedTexts";
		base.ShowInTaskbar = false;
		this.Text = "User Defined Texts";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_UserDefinedTexts_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Data).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Data).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Search.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
