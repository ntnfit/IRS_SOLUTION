using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Widget;

public class frm_AddGroup : XtraFormEx
{
	private IContainer components;

	private GridControlEx grd_Tabs;

	private GridViewEx gridViewEx1;

	private SimpleButton btn_OK;

	private SimpleButton btn_Cancel;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private RepositoryItemColorEdit rep_BackColor;

	public Action<string, DataTable> GroupData { get; set; }

	public frm_AddGroup()
	{
		InitializeComponent();
	}

	private void btn_OK_Click(object sender, EventArgs e)
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Widget.frm_AddGroup));
		this.grd_Tabs = new Apzon.Controls.Grid.GridControlEx();
		this.gridViewEx1 = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_BackColor = new DevExpress.XtraEditors.Repository.RepositoryItemColorEdit();
		this.btn_OK = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Cancel = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.grd_Tabs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BackColor).BeginInit();
		base.SuspendLayout();
		this.grd_Tabs.AllowPasteExcel = true;
		this.grd_Tabs.ClipboardData = null;
		this.grd_Tabs.Location = new System.Drawing.Point(5, 2);
		this.grd_Tabs.MainView = this.gridViewEx1;
		this.grd_Tabs.Name = "grd_Tabs";
		this.grd_Tabs.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_BackColor });
		this.grd_Tabs.Size = new System.Drawing.Size(695, 348);
		this.grd_Tabs.TabIndex = 3;
		this.grd_Tabs.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridViewEx1 });
		this.gridViewEx1.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gridViewEx1.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gridViewEx1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridViewEx1.ColumnKey = null;
		this.gridViewEx1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gridColumnEx1, this.gridColumnEx2 });
		this.gridViewEx1.DisableFormSetting = false;
		this.gridViewEx1.DisplayException = false;
		this.gridViewEx1.EditColumn = null;
		this.gridViewEx1.FieldCurrency = null;
		this.gridViewEx1.FieldRate = null;
		this.gridViewEx1.FormId = "oAddGroup";
		this.gridViewEx1.GridControl = this.grd_Tabs;
		this.gridViewEx1.IgnoreFilter = false;
		this.gridViewEx1.IndexEditRow = -1;
		this.gridViewEx1.IndicatorWidth = 35;
		this.gridViewEx1.IsApzTable = false;
		this.gridViewEx1.IsInsertNewRow = false;
		this.gridViewEx1.IsLoadingData = false;
		this.gridViewEx1.IsMappingContextAction = false;
		this.gridViewEx1.ItemId = "OTAB";
		this.gridViewEx1.LstColumnAllowBatch = null;
		this.gridViewEx1.LstColumnNotAllowBatch = null;
		this.gridViewEx1.LstColumnNotEditable = null;
		this.gridViewEx1.LstColumNotSetting = null;
		this.gridViewEx1.Name = "gridViewEx1";
		this.gridViewEx1.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gridViewEx1.OptionsView.BestFitMaxRowCount = 100;
		this.gridViewEx1.OptionsView.ColumnAutoWidth = false;
		this.gridViewEx1.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridViewEx1.OptionsView.ShowGroupPanel = false;
		this.gridViewEx1.UseBatchMod = false;
		this.gridColumnEx1.Caption = "Name";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "Name";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx1.Width = 181;
		this.gridColumnEx2.Caption = "Background Color";
		this.gridColumnEx2.ColumnEdit = this.rep_BackColor;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "BackColor";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx2.Width = 203;
		this.rep_BackColor.AutoHeight = false;
		this.rep_BackColor.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_BackColor.Name = "rep_BackColor";
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.Location = new System.Drawing.Point(5, 356);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 4;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.Location = new System.Drawing.Point(85, 356);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 5;
		this.btn_Cancel.Text = "Cancel";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(701, 384);
		base.Controls.Add(this.grd_Tabs);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.FormId = "oAddGroup";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_AddGroup.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_AddGroup";
		base.ShowInTaskbar = false;
		this.Text = "Group";
		((System.ComponentModel.ISupportInitialize)this.grd_Tabs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BackColor).EndInit();
		base.ResumeLayout(false);
	}
}
