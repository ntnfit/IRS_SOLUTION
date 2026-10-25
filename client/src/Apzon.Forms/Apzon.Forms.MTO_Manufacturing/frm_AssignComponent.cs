using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.MTO_Manufacturing;

public class frm_AssignComponent : XtraFormEx
{
	private IContainer components;

	private SimpleButtonEx btn_Load;

	private LabelLine lb_MaterialCode;

	private LabelLine lb_MaterialName;

	private TextEditEx txt_MaterialCode;

	private TextEditEx txt_MaterialName;

	private GridControlEx grdComponent;

	private GridViewEx gvComponent;

	private SimpleButtonEx btn_Back;

	private SimpleButtonEx btn_AssignTo;

	private GroupControl groupControl1;

	private GridColumnEx gcComponentCode;

	private GridColumnEx gcComponentName;

	private GridColumnEx gcQuantity;

	private GridColumnEx gcPrice;

	private GridColumnEx gcStatus;

	public frm_AssignComponent()
	{
		InitializeComponent();
		SettingLanguage();
	}

	private void SettingLanguage()
	{
		Text = Resources.Titel_frmAssignComponent;
		btn_Load.Text = Resources.ButtonLoad;
		btn_Back.Text = Resources.ButtonBack;
		btn_AssignTo.Text = Resources.ButtonAssignTo;
		lb_MaterialCode.Utext = Resources.lb_MaterialCode;
		lb_MaterialName.Utext = Resources.lb_MaterialName;
		gcComponentCode.Caption = Resources.lb_ComponentCode;
		gcComponentName.Caption = Resources.lb_ComponentName;
		gcQuantity.Caption = Resources.lb_Quantity;
		gcPrice.Caption = Resources.Line_gvLine_gcPrice;
		gcStatus.Caption = Resources.lb_Status;
	}

	private void btn_AssignTo_Click(object sender, EventArgs e)
	{
		try
		{
			frm_AsignMaterialToOperation frm_AsignMaterialToOperation2 = new frm_AsignMaterialToOperation();
			frm_AsignMaterialToOperation2.MdiParent = MdiParent;
			frm_AsignMaterialToOperation2.Show();
		}
		catch (Exception ex)
		{
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
		this.btn_Load = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lb_MaterialCode = new Apzon.Controls.Controls.LabelLine();
		this.lb_MaterialName = new Apzon.Controls.Controls.LabelLine();
		this.txt_MaterialCode = new Apzon.Controls.Editor.TextEditEx();
		this.txt_MaterialName = new Apzon.Controls.Editor.TextEditEx();
		this.grdComponent = new Apzon.Controls.Grid.GridControlEx();
		this.gvComponent = new Apzon.Controls.Grid.GridViewEx();
		this.gcComponentCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcComponentName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcQuantity = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPrice = new Apzon.Controls.Grid.GridColumnEx();
		this.gcStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Back = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_AssignTo = new Apzon.Controls.Controls.SimpleButtonEx();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		((System.ComponentModel.ISupportInitialize)this.txt_MaterialCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MaterialName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdComponent).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvComponent).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		base.SuspendLayout();
		this.btn_Load.EditControl = null;
		this.btn_Load.EditValue = "Load";
		this.btn_Load.FieldName = null;
		this.btn_Load.IsSearchControl = false;
		this.btn_Load.Location = new System.Drawing.Point(431, 11);
		this.btn_Load.Name = "btn_Load";
		this.btn_Load.ReadOnly = false;
		this.btn_Load.SettingInVisible = false;
		this.btn_Load.SettingReadonly = false;
		this.btn_Load.Size = new System.Drawing.Size(99, 20);
		this.btn_Load.TabIndex = 0;
		this.btn_Load.Text = "Load";
		this.lb_MaterialCode.EditControl = null;
		this.lb_MaterialCode.EditValue = "Material Code";
		this.lb_MaterialCode.FieldName = null;
		this.lb_MaterialCode.HideWhenEmpty = false;
		this.lb_MaterialCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_MaterialCode.Location = new System.Drawing.Point(3, 12);
		this.lb_MaterialCode.Name = "lb_MaterialCode";
		this.lb_MaterialCode.ReadOnly = false;
		this.lb_MaterialCode.SettingInVisible = false;
		this.lb_MaterialCode.SettingReadonly = false;
		this.lb_MaterialCode.ShowUnderLine = true;
		this.lb_MaterialCode.Size = new System.Drawing.Size(80, 20);
		this.lb_MaterialCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_MaterialCode.TabIndex = 1;
		this.lb_MaterialCode.Utext = "Material Code";
		this.lb_MaterialName.EditControl = null;
		this.lb_MaterialName.EditValue = "Material Name";
		this.lb_MaterialName.FieldName = null;
		this.lb_MaterialName.HideWhenEmpty = false;
		this.lb_MaterialName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_MaterialName.Location = new System.Drawing.Point(203, 11);
		this.lb_MaterialName.Name = "lb_MaterialName";
		this.lb_MaterialName.ReadOnly = false;
		this.lb_MaterialName.SettingInVisible = false;
		this.lb_MaterialName.SettingReadonly = false;
		this.lb_MaterialName.ShowUnderLine = true;
		this.lb_MaterialName.Size = new System.Drawing.Size(80, 20);
		this.lb_MaterialName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_MaterialName.TabIndex = 2;
		this.lb_MaterialName.Utext = "Material Name";
		this.txt_MaterialCode.EditControl = null;
		this.txt_MaterialCode.FieldName = null;
		this.txt_MaterialCode.IsSearchControl = false;
		this.txt_MaterialCode.Location = new System.Drawing.Point(81, 11);
		this.txt_MaterialCode.Name = "txt_MaterialCode";
		this.txt_MaterialCode.SettingInVisible = false;
		this.txt_MaterialCode.SettingReadonly = false;
		this.txt_MaterialCode.Size = new System.Drawing.Size(103, 20);
		this.txt_MaterialCode.TabIndex = 3;
		this.txt_MaterialName.EditControl = null;
		this.txt_MaterialName.FieldName = null;
		this.txt_MaterialName.IsSearchControl = false;
		this.txt_MaterialName.Location = new System.Drawing.Point(283, 11);
		this.txt_MaterialName.Name = "txt_MaterialName";
		this.txt_MaterialName.SettingInVisible = false;
		this.txt_MaterialName.SettingReadonly = false;
		this.txt_MaterialName.Size = new System.Drawing.Size(129, 20);
		this.txt_MaterialName.TabIndex = 4;
		this.grdComponent.AllowPasteExcel = true;
		this.grdComponent.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdComponent.ClipboardData = null;
		this.grdComponent.Location = new System.Drawing.Point(9, 24);
		this.grdComponent.MainView = this.gvComponent;
		this.grdComponent.Name = "grdComponent";
		this.grdComponent.Size = new System.Drawing.Size(541, 163);
		this.grdComponent.TabIndex = 5;
		this.grdComponent.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvComponent });
		this.gvComponent.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvComponent.ColumnKey = null;
		this.gvComponent.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gcComponentCode, this.gcComponentName, this.gcQuantity, this.gcPrice, this.gcStatus });
		this.gvComponent.DisableFormSetting = false;
		this.gvComponent.DisplayException = false;
		this.gvComponent.EditColumn = null;
		this.gvComponent.FieldCurrency = null;
		this.gvComponent.FieldRate = null;
		this.gvComponent.FormId = null;
		this.gvComponent.GridControl = this.grdComponent;
		this.gvComponent.IgnoreFilter = false;
		this.gvComponent.IndexEditRow = -1;
		this.gvComponent.IndicatorWidth = 35;
		this.gvComponent.IsApzTable = false;
		this.gvComponent.IsInsertNewRow = false;
		this.gvComponent.IsLoadingData = false;
		this.gvComponent.IsMappingContextAction = false;
		this.gvComponent.ItemId = null;
		this.gvComponent.LstColumnAllowBatch = null;
		this.gvComponent.LstColumnNotAllowBatch = null;
		this.gvComponent.LstColumnNotEditable = null;
		this.gvComponent.LstColumNotSetting = null;
		this.gvComponent.Name = "gvComponent";
		this.gvComponent.UseBatchMod = false;
		this.gcComponentCode.Caption = "Component Code";
		this.gcComponentCode.CurrencyDateField = null;
		this.gcComponentCode.CurrencyField = null;
		this.gcComponentCode.CurrencyFixed = null;
		this.gcComponentCode.CurrencyFixedRate = 0.0;
		this.gcComponentCode.CurrencyRateField = null;
		this.gcComponentCode.Name = "gcComponentCode";
		this.gcComponentCode.PrimaryKey = true;
		this.gcComponentCode.UnitField = null;
		this.gcComponentCode.Visible = true;
		this.gcComponentCode.VisibleIndex = 0;
		this.gcComponentName.Caption = "Component Name";
		this.gcComponentName.CurrencyDateField = null;
		this.gcComponentName.CurrencyField = null;
		this.gcComponentName.CurrencyFixed = null;
		this.gcComponentName.CurrencyFixedRate = 0.0;
		this.gcComponentName.CurrencyRateField = null;
		this.gcComponentName.Name = "gcComponentName";
		this.gcComponentName.PrimaryKey = false;
		this.gcComponentName.UnitField = null;
		this.gcComponentName.Visible = true;
		this.gcComponentName.VisibleIndex = 1;
		this.gcQuantity.Caption = "Quantity";
		this.gcQuantity.CurrencyDateField = null;
		this.gcQuantity.CurrencyField = null;
		this.gcQuantity.CurrencyFixed = null;
		this.gcQuantity.CurrencyFixedRate = 0.0;
		this.gcQuantity.CurrencyRateField = null;
		this.gcQuantity.Name = "gcQuantity";
		this.gcQuantity.PrimaryKey = false;
		this.gcQuantity.UnitField = null;
		this.gcQuantity.Visible = true;
		this.gcQuantity.VisibleIndex = 2;
		this.gcPrice.Caption = "Unit Price";
		this.gcPrice.CurrencyDateField = null;
		this.gcPrice.CurrencyField = null;
		this.gcPrice.CurrencyFixed = null;
		this.gcPrice.CurrencyFixedRate = 0.0;
		this.gcPrice.CurrencyRateField = null;
		this.gcPrice.Name = "gcPrice";
		this.gcPrice.PrimaryKey = false;
		this.gcPrice.UnitField = null;
		this.gcPrice.Visible = true;
		this.gcPrice.VisibleIndex = 3;
		this.gcStatus.Caption = "Status";
		this.gcStatus.CurrencyDateField = null;
		this.gcStatus.CurrencyField = null;
		this.gcStatus.CurrencyFixed = null;
		this.gcStatus.CurrencyFixedRate = 0.0;
		this.gcStatus.CurrencyRateField = null;
		this.gcStatus.Name = "gcStatus";
		this.gcStatus.PrimaryKey = false;
		this.gcStatus.UnitField = null;
		this.gcStatus.Visible = true;
		this.gcStatus.VisibleIndex = 4;
		this.btn_Back.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Back.EditControl = null;
		this.btn_Back.EditValue = "Back";
		this.btn_Back.FieldName = null;
		this.btn_Back.IsSearchControl = false;
		this.btn_Back.Location = new System.Drawing.Point(11, 235);
		this.btn_Back.Name = "btn_Back";
		this.btn_Back.ReadOnly = false;
		this.btn_Back.SettingInVisible = false;
		this.btn_Back.SettingReadonly = false;
		this.btn_Back.Size = new System.Drawing.Size(98, 20);
		this.btn_Back.TabIndex = 6;
		this.btn_Back.Text = "Back";
		this.btn_AssignTo.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_AssignTo.EditControl = null;
		this.btn_AssignTo.EditValue = "Assign To";
		this.btn_AssignTo.FieldName = null;
		this.btn_AssignTo.IsSearchControl = false;
		this.btn_AssignTo.Location = new System.Drawing.Point(123, 235);
		this.btn_AssignTo.Name = "btn_AssignTo";
		this.btn_AssignTo.ReadOnly = false;
		this.btn_AssignTo.SettingInVisible = false;
		this.btn_AssignTo.SettingReadonly = false;
		this.btn_AssignTo.Size = new System.Drawing.Size(107, 20);
		this.btn_AssignTo.TabIndex = 7;
		this.btn_AssignTo.Text = "Assign To";
		this.btn_AssignTo.Click += new System.EventHandler(this.btn_AssignTo_Click);
		this.groupControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.groupControl1.Controls.Add(this.grdComponent);
		this.groupControl1.Location = new System.Drawing.Point(3, 37);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(555, 192);
		this.groupControl1.TabIndex = 8;
		this.groupControl1.Text = "Components";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(557, 263);
		base.Controls.Add(this.groupControl1);
		base.Controls.Add(this.btn_AssignTo);
		base.Controls.Add(this.btn_Back);
		base.Controls.Add(this.txt_MaterialName);
		base.Controls.Add(this.txt_MaterialCode);
		base.Controls.Add(this.lb_MaterialName);
		base.Controls.Add(this.lb_MaterialCode);
		base.Controls.Add(this.btn_Load);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_AssignComponent";
		base.ShowInTaskbar = false;
		this.Text = "MATERIAL / COMPONENTS ASSIGNMENT";
		((System.ComponentModel.ISupportInitialize)this.txt_MaterialCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MaterialName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdComponent).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvComponent).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
