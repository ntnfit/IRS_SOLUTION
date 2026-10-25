using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons.Enums;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Documents;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Commons;

public class FrmApprovalRequest : XtraFormEx
{
	private IContainer components;

	private Label label1;

	private GridControlEx grd;

	private GridViewEx gv;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private SimpleButtonEx btnOK;

	private SimpleButtonEx btn_Cancel;

	private GridColumnEx gridColumnEx1;

	private DocumentsImpl _doc { get; set; }

	public DataTable _source { get; set; }

	public FrmApprovalRequest()
	{
		InitializeComponent();
	}

	public FrmApprovalRequest(DocumentsImpl doc, DataTable source)
	{
		InitializeComponent();
		_doc = doc;
		_source = source;
	}

	private void FrmApprovalRequest_Load(object sender, EventArgs e)
	{
		grd.DataSource = _source;
		grd.RefreshDataSource();
		gv.FocusedColumn = gv.Columns["Remarks"];
		gv.ShowEditor();
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		DialogResult = DialogResult.No;
		Close();
	}

	private void btnOK_Click(object sender, EventArgs e)
	{
		DialogResult = DialogResult.Yes;
		_source = (DataTable)grd.DataSource;
		Close();
	}

	private void FrmApprovalRequest_KeyUp(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			btnOK_Click(null, null);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.FrmApprovalRequest));
		this.label1 = new System.Windows.Forms.Label();
		this.grd = new Apzon.Controls.Grid.GridControlEx();
		this.gv = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.btnOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv).BeginInit();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(3, 9);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(222, 52);
		this.label1.TabIndex = 0;
		this.label1.Text = "Generating this document requires\r\nthe authorization of other users.\r\nIn the table below, type remarks that are\r\nrelevant for the authorizer and choose \"OK\".";
		this.grd.AllowPasteExcel = true;
		this.grd.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd.ClipboardData = null;
		this.grd.Location = new System.Drawing.Point(6, 65);
		this.grd.MainView = this.gv;
		this.grd.Name = "grd";
		this.grd.Size = new System.Drawing.Size(449, 162);
		this.grd.TabIndex = 1;
		this.grd.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv });
		this.gv.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv.ColumnKey = null;
		this.gv.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gridColumn1, this.gridColumnEx1, this.gridColumn2 });
		this.gv.DisableFormSetting = false;
		this.gv.DisplayException = false;
		this.gv.EditColumn = null;
		this.gv.FieldCurrency = null;
		this.gv.FieldRate = null;
		this.gv.FormId = "oApprovalResuest";
		this.gv.GridControl = this.grd;
		this.gv.HasFormSetting = false;
		this.gv.IgnoreFilter = false;
		this.gv.IndexEditRow = -1;
		this.gv.IndicatorWidth = 35;
		this.gv.IsApzTable = false;
		this.gv.IsInsertNewRow = false;
		this.gv.IsLoadingData = false;
		this.gv.IsMappingContextAction = false;
		this.gv.ItemId = "ODATA";
		this.gv.LstColumnAllowBatch = null;
		this.gv.LstColumnNotAllowBatch = null;
		this.gv.LstColumnNotEditable = new string[1] { "Description" };
		this.gv.LstColumNotSetting = null;
		this.gv.LstOldUDF = null;
		this.gv.Name = "gv";
		this.gv.OptionsView.ColumnAutoWidth = false;
		this.gv.OptionsView.ShowGroupPanel = false;
		this.gv.UseBatchMod = false;
		this.gridColumn1.AllowChSet = false;
		this.gridColumn1.Caption = "Authorization Template";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "Name";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowEdit = false;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn1.Width = 195;
		this.gridColumnEx1.Caption = "Description";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "Description";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.AllowEdit = false;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumn2.AllowChSet = false;
		this.gridColumn2.Caption = "Remarks";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "Remarks";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn2.Width = 173;
		this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOK.EditControl = null;
		this.btnOK.EditValue = "OK";
		this.btnOK.FieldName = null;
		this.btnOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOK.ImageOptions.Image");
		this.btnOK.IsSearchControl = false;
		this.btnOK.Location = new System.Drawing.Point(6, 233);
		this.btnOK.Name = "btnOK";
		this.btnOK.ReadOnly = false;
		this.btnOK.SettingInVisible = false;
		this.btnOK.SettingReadonly = false;
		this.btnOK.Size = new System.Drawing.Size(75, 23);
		this.btnOK.TabIndex = 2;
		this.btnOK.Text = "OK";
		this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(87, 233);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 3;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(462, 257);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btnOK);
		base.Controls.Add(this.grd);
		base.Controls.Add(this.label1);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.FormId = "oApprovalResuest";
		base.IconOptions.ShowIcon = false;
		base.Name = "FrmApprovalRequest";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Approval Request";
		base.Load += new System.EventHandler(this.FrmApprovalRequest_Load);
		base.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FrmApprovalRequest_KeyUp);
		((System.ComponentModel.ISupportInitialize)this.grd).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
