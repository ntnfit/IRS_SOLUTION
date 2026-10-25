using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Widget;

public class frm_WidgetManagement : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private GridControlEx grd_Data;

	private GridViewEx gv_Data;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private RepositoryItemCheckEdit rep_Active;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_NewWidget;

	private GridColumnEx gridColumnEx3;

	private RepositoryItemButtonEdit rep_Caption;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	private ToolStripMenuItem viewDetailToolStripMenuItem;

	public frm_WidgetManagement()
	{
		InitializeComponent();
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btn_NewWidget_Click(object sender, EventArgs e)
	{
		try
		{
			frm_Widget frm_Widget2 = new frm_Widget
			{
				ShowProperties = true,
				MdiParent = MdiParent,
				Text = "New Widget"
			};
			frm_Widget2.AfterSave = (Action)Delegate.Combine(frm_Widget2.AfterSave, new Action(NewWidget_AfterSave));
			frm_Widget2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void NewWidget_AfterSave()
	{
		await REFRESH();
	}

	private async Task frm_WidgetManagement_Load(object sender, EventArgs e)
	{
		await REFRESH();
	}

	private async Task REFRESH()
	{
		try
		{
			gv_Data.ShowLoadingPanel();
			DataTable dataSource = await _httpClient.Widget.GetAll();
			grd_Data.DataSource = dataSource;
			grd_Data.RefreshDataSource();
			gv_Data.HideLoadingPanel();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			gv_Data.HideLoadingPanel();
		}
	}

	private void rep_Caption_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_Widget frm_Widget2 = new frm_Widget();
			frm_Widget2.ShowProperties = true;
			frm_Widget2.Text = Function.ToString(gv_Data.GetFocusedRowCellValue("Caption"));
			frm_Widget2.WidgetEntry = Function.ParseInt(gv_Data.GetFocusedRowCellValue("AbsEntry"));
			frm_Widget2.MdiParent = MdiParent;
			frm_Widget2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			int absEntry = Function.ParseInt(gv_Data.GetFocusedRowCellValue("AbsEntry"));
			if (XtraMessageBoxEx.Show(this, Enums.MessageCode.ConfirmRemoveItemById, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, Function.ToString(gv_Data.GetFocusedRowCellValue("Caption"))) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				HttpResult httpResult = await _httpClient.Widget.Remove(absEntry);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					await REFRESH();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
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
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Widget.frm_WidgetManagement));
		this.grd_Data = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.viewDetailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Data = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Caption = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Active = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_NewWidget = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Data).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Data).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Caption).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Active).BeginInit();
		base.SuspendLayout();
		this.grd_Data.AllowPasteExcel = true;
		this.grd_Data.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Data.ClipboardData = null;
		this.grd_Data.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Data.Location = new System.Drawing.Point(1, 2);
		this.grd_Data.MainView = this.gv_Data;
		this.grd_Data.Name = "grd_Data";
		this.grd_Data.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_Active, this.rep_Caption });
		this.grd_Data.Size = new System.Drawing.Size(735, 364);
		this.grd_Data.TabIndex = 0;
		this.grd_Data.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Data });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.viewDetailToolStripMenuItem, this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 48);
		this.viewDetailToolStripMenuItem.Name = "viewDetailToolStripMenuItem";
		this.viewDetailToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.viewDetailToolStripMenuItem.Text = "Edit";
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_Data.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Data.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Data.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Data.ColumnKey = null;
		this.gv_Data.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gridColumnEx1, this.gridColumnEx3, this.gridColumnEx2 });
		this.gv_Data.DisableFormSetting = false;
		this.gv_Data.DisplayException = false;
		this.gv_Data.EditColumn = null;
		this.gv_Data.FieldCurrency = null;
		this.gv_Data.FieldRate = null;
		this.gv_Data.FormId = "oWeightManagement";
		this.gv_Data.GridControl = this.grd_Data;
		this.gv_Data.IgnoreFilter = false;
		this.gv_Data.IndexEditRow = -1;
		this.gv_Data.IndicatorWidth = 35;
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
		this.gv_Data.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Data.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Data.OptionsView.ColumnAutoWidth = false;
		this.gv_Data.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Data.OptionsView.ShowGroupPanel = false;
		this.gv_Data.UseBatchMod = false;
		this.gridColumnEx1.Caption = "Caption";
		this.gridColumnEx1.ColumnEdit = this.rep_Caption;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "Caption";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx1.Width = 276;
		this.rep_Caption.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Caption.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Caption.Name = "rep_Caption";
		this.rep_Caption.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Caption_ButtonClick);
		this.gridColumnEx3.Caption = "Father Menu";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "FatherMenu";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 1;
		this.gridColumnEx3.Width = 249;
		this.gridColumnEx2.Caption = "Active";
		this.gridColumnEx2.ColumnEdit = this.rep_Active;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "Active";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 2;
		this.rep_Active.AutoHeight = false;
		this.rep_Active.Name = "rep_Active";
		this.rep_Active.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.rep_Active.ValueChecked = "Y";
		this.rep_Active.ValueGrayed = "N";
		this.rep_Active.ValueUnchecked = "N";
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(7, 372);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_NewWidget.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_NewWidget.EditControl = null;
		this.btn_NewWidget.EditValue = "New Widget";
		this.btn_NewWidget.FieldName = null;
		this.btn_NewWidget.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_NewWidget.ImageOptions.Image");
		this.btn_NewWidget.IsSearchControl = false;
		this.btn_NewWidget.Location = new System.Drawing.Point(642, 372);
		this.btn_NewWidget.Name = "btn_NewWidget";
		this.btn_NewWidget.ReadOnly = false;
		this.btn_NewWidget.SettingInVisible = false;
		this.btn_NewWidget.SettingReadonly = false;
		this.btn_NewWidget.Size = new System.Drawing.Size(91, 23);
		this.btn_NewWidget.TabIndex = 2;
		this.btn_NewWidget.Text = "New Widget";
		this.btn_NewWidget.Click += new System.EventHandler(this.btn_NewWidget_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(737, 399);
		base.Controls.Add(this.btn_NewWidget);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.grd_Data);
		base.FormId = "oWeightManagement";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_WidgetManagement.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_WidgetManagement";
		base.ShowInTaskbar = false;
		this.Text = "Widget Management";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_WidgetManagement_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Data).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Data).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Caption).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Active).EndInit();
		base.ResumeLayout(false);
	}
}
