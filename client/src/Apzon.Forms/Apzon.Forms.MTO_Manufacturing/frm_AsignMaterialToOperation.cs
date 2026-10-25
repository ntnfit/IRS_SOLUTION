using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.MTO_Manufacturing;

public class frm_AsignMaterialToOperation : XtraFormEx
{
	private IContainer components;

	private GroupControl grc_RoutingCode;

	private GridControlEx grdRouting;

	private GridViewEx gvRouting;

	private SimpleButtonEx btn_Save;

	private SimpleButtonEx btn_Closed;

	private GridColumnEx gcOperationCode;

	private GridColumnEx gcOperationName;

	private GridColumnEx gcAssigned;

	private RepositoryItemCheckEdit repAssign;

	public frm_AsignMaterialToOperation()
	{
		InitializeComponent();
		SettingLanguage();
	}

	private void SettingLanguage()
	{
		Text = Resources.Titel_frmAsignMaterialToOperation;
		grc_RoutingCode.Text = Resources.lb_RoutingCode;
		gcOperationCode.Caption = Resources.lb_OperationCode;
		gcAssigned.Caption = Resources.lb_Assigned;
		btn_Save.Text = Resources.ButtonSave;
		btn_Closed.Text = Resources.ButtonClose;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.MTO_Manufacturing.frm_AsignMaterialToOperation));
		this.grc_RoutingCode = new DevExpress.XtraEditors.GroupControl();
		this.grdRouting = new Apzon.Controls.Grid.GridControlEx();
		this.gvRouting = new Apzon.Controls.Grid.GridViewEx();
		this.gcOperationCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOperationName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAssigned = new Apzon.Controls.Grid.GridColumnEx();
		this.repAssign = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Closed = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grc_RoutingCode).BeginInit();
		this.grc_RoutingCode.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdRouting).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvRouting).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repAssign).BeginInit();
		base.SuspendLayout();
		this.grc_RoutingCode.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grc_RoutingCode.Controls.Add(this.grdRouting);
		this.grc_RoutingCode.Location = new System.Drawing.Point(2, 0);
		this.grc_RoutingCode.Name = "grc_RoutingCode";
		this.grc_RoutingCode.Size = new System.Drawing.Size(508, 227);
		this.grc_RoutingCode.TabIndex = 0;
		this.grc_RoutingCode.Text = "Routing Code";
		this.grdRouting.AllowPasteExcel = true;
		this.grdRouting.ClipboardData = null;
		this.grdRouting.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdRouting.Location = new System.Drawing.Point(2, 23);
		this.grdRouting.MainView = this.gvRouting;
		this.grdRouting.Name = "grdRouting";
		this.grdRouting.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repAssign });
		this.grdRouting.Size = new System.Drawing.Size(504, 202);
		this.grdRouting.TabIndex = 0;
		this.grdRouting.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvRouting });
		this.gvRouting.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvRouting.ColumnKey = null;
		this.gvRouting.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gcOperationCode, this.gcOperationName, this.gcAssigned });
		this.gvRouting.DisableFormSetting = false;
		this.gvRouting.DisplayException = false;
		this.gvRouting.EditColumn = null;
		this.gvRouting.FieldCurrency = null;
		this.gvRouting.FieldRate = null;
		this.gvRouting.FormId = null;
		this.gvRouting.GridControl = this.grdRouting;
		this.gvRouting.IgnoreFilter = false;
		this.gvRouting.IndexEditRow = -1;
		this.gvRouting.IndicatorWidth = 35;
		this.gvRouting.IsApzTable = false;
		this.gvRouting.IsInsertNewRow = false;
		this.gvRouting.IsLoadingData = false;
		this.gvRouting.IsMappingContextAction = false;
		this.gvRouting.ItemId = null;
		this.gvRouting.LstColumnAllowBatch = null;
		this.gvRouting.LstColumnNotAllowBatch = null;
		this.gvRouting.LstColumnNotEditable = null;
		this.gvRouting.LstColumNotSetting = null;
		this.gvRouting.Name = "gvRouting";
		this.gvRouting.UseBatchMod = false;
		this.gcOperationCode.Caption = "Operation Code";
		this.gcOperationCode.CurrencyDateField = null;
		this.gcOperationCode.CurrencyField = null;
		this.gcOperationCode.CurrencyFixed = null;
		this.gcOperationCode.CurrencyFixedRate = 0.0;
		this.gcOperationCode.CurrencyRateField = null;
		this.gcOperationCode.Name = "gcOperationCode";
		this.gcOperationCode.PrimaryKey = true;
		this.gcOperationCode.UnitField = null;
		this.gcOperationCode.Visible = true;
		this.gcOperationCode.VisibleIndex = 0;
		this.gcOperationCode.Width = 117;
		this.gcOperationName.Caption = "Operation Name";
		this.gcOperationName.CurrencyDateField = null;
		this.gcOperationName.CurrencyField = null;
		this.gcOperationName.CurrencyFixed = null;
		this.gcOperationName.CurrencyFixedRate = 0.0;
		this.gcOperationName.CurrencyRateField = null;
		this.gcOperationName.Name = "gcOperationName";
		this.gcOperationName.PrimaryKey = false;
		this.gcOperationName.UnitField = null;
		this.gcOperationName.Visible = true;
		this.gcOperationName.VisibleIndex = 1;
		this.gcOperationName.Width = 308;
		this.gcAssigned.Caption = "Assigned";
		this.gcAssigned.ColumnEdit = this.repAssign;
		this.gcAssigned.CurrencyDateField = null;
		this.gcAssigned.CurrencyField = null;
		this.gcAssigned.CurrencyFixed = null;
		this.gcAssigned.CurrencyFixedRate = 0.0;
		this.gcAssigned.CurrencyRateField = null;
		this.gcAssigned.Name = "gcAssigned";
		this.gcAssigned.PrimaryKey = false;
		this.gcAssigned.UnitField = null;
		this.gcAssigned.Visible = true;
		this.gcAssigned.VisibleIndex = 2;
		this.gcAssigned.Width = 61;
		this.repAssign.AutoHeight = false;
		this.repAssign.Name = "repAssign";
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "Save";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(2, 233);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(98, 20);
		this.btn_Save.TabIndex = 7;
		this.btn_Save.Text = "Save";
		this.btn_Closed.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Closed.EditControl = null;
		this.btn_Closed.EditValue = "Close";
		this.btn_Closed.FieldName = null;
		this.btn_Closed.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Closed.ImageOptions.Image");
		this.btn_Closed.IsSearchControl = false;
		this.btn_Closed.Location = new System.Drawing.Point(104, 233);
		this.btn_Closed.Name = "btn_Closed";
		this.btn_Closed.ReadOnly = false;
		this.btn_Closed.SettingInVisible = false;
		this.btn_Closed.SettingReadonly = false;
		this.btn_Closed.Size = new System.Drawing.Size(98, 20);
		this.btn_Closed.TabIndex = 8;
		this.btn_Closed.Text = "Close";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(511, 259);
		base.Controls.Add(this.btn_Closed);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.grc_RoutingCode);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_AsignMaterialToOperation";
		base.ShowInTaskbar = false;
		this.Text = "Assign Material To Opertation";
		((System.ComponentModel.ISupportInitialize)this.grc_RoutingCode).EndInit();
		this.grc_RoutingCode.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdRouting).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvRouting).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repAssign).EndInit();
		base.ResumeLayout(false);
	}
}
