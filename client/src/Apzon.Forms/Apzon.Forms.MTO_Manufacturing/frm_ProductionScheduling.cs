using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Commons;
using Apzon.Global;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.MTO_Manufacturing;

public class frm_ProductionScheduling : XtraFormEx
{
	private IContainer components;

	private Panel panel1;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btSave;

	private PanelControl panelControl1;

	private DateEditEx dateEdit2;

	private DateEditEx dateEdit1;

	private SimpleButtonEx btSearch;

	private LabelLine lb_ToDate;

	private LabelLine lb_FromDate;

	private ButtonEditEx txtCardCode;

	private LabelLine lb_MOCode;

	private SplitContainerControl splitContainerControl1;

	private GroupControl groupControl1;

	private GridControlEx gridControl1;

	private GridViewEx gridView1;

	private GridColumnEx gcMasterOrderCode;

	private GridColumnEx gcItemCode;

	private GridColumnEx gcItemName;

	private GridColumnEx gcOpCode;

	private GridColumnEx gcRouting;

	private GridColumnEx gcUoM;

	private GridColumnEx gcTotalDemand;

	private GridColumnEx gcLeadTime;

	private GridColumnEx gridColumn9;

	private GridColumnEx gridColumn10;

	private GridColumnEx gridColumn11;

	private GridColumnEx gridColumn12;

	private GridColumnEx gridColumn13;

	private GridColumnEx gridColumn14;

	private GroupControl groupControl2;

	private GridControlEx gridControl2;

	private GridViewEx gridView2;

	private GridColumnEx gcResouceNo;

	private GridColumnEx gcResouceName;

	private GridColumnEx gcLine_UoM;

	private GridColumnEx gcPlant;

	private GridColumnEx gcDescription;

	private GridColumnEx gridColumn20;

	private GridColumnEx gridColumn21;

	private GridColumnEx gridColumn22;

	private GridColumnEx gridColumn23;

	private GridColumnEx gridColumn24;

	private GridColumnEx gridColumn25;

	public frm_ProductionScheduling()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ProductionScheduling"))
		{
			InitializeComponent();
			SettingLanguage();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ProductionScheduling" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void SettingLanguage()
	{
		Text = Resources.Title_frmProductionScheduling;
		lb_MOCode.Utext = Resources.lb_MOCode;
		lb_FromDate.Utext = Resources.Doc_lb_FromDate;
		lb_ToDate.Utext = Resources.Doc_lb_ToDate;
		btSearch.Text = Resources.ButtonSearch;
		btSave.Text = Resources.ButtonSave;
		btCancel.Text = Resources.ButtonCancel;
		groupControl1.Text = Resources.grc_GeneralMOToPlanning;
		gcMasterOrderCode.Caption = Resources.lb_MasterOrderCode;
		gcItemCode.Caption = Resources.Line_gvLine_gcItemCode;
		gcItemName.Caption = Resources.Line_gvLine_gcItemName;
		gcOpCode.Caption = Resources.lb_OpCode;
		gcRouting.Caption = Resources.Titel_frmRouting;
		gcUoM.Caption = Resources.lb_UoM;
		gcTotalDemand.Caption = Resources.lb_TotalDemand;
		gcLeadTime.Caption = Resources.lb_LeadTime;
		groupControl2.Text = Resources.tab_ResourceLoad;
		gcResouceNo.Caption = Resources.lb_ResourceNo;
		gcResouceName.Caption = Resources.lb_ResourceName;
		gcLine_UoM.Caption = Resources.lb_UoM;
		gcPlant.Caption = Resources.lb_Plant;
		gcDescription.Caption = Resources.lb_Description;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.MTO_Manufacturing.frm_ProductionScheduling));
		this.panel1 = new System.Windows.Forms.Panel();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btSave = new Apzon.Controls.Controls.SimpleButtonEx();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.dateEdit2 = new Apzon.Controls.Editor.DateEditEx();
		this.dateEdit1 = new Apzon.Controls.Editor.DateEditEx();
		this.btSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lb_ToDate = new Apzon.Controls.Controls.LabelLine();
		this.lb_FromDate = new Apzon.Controls.Controls.LabelLine();
		this.txtCardCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.lb_MOCode = new Apzon.Controls.Controls.LabelLine();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.gridControl1 = new Apzon.Controls.Grid.GridControlEx();
		this.gridView1 = new Apzon.Controls.Grid.GridViewEx();
		this.gcMasterOrderCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOpCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRouting = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUoM = new Apzon.Controls.Grid.GridColumnEx();
		this.gcTotalDemand = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLeadTime = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn14 = new Apzon.Controls.Grid.GridColumnEx();
		this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
		this.gridControl2 = new Apzon.Controls.Grid.GridControlEx();
		this.gridView2 = new Apzon.Controls.Grid.GridViewEx();
		this.gcResouceNo = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResouceName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLine_UoM = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPlant = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDescription = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn22 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn23 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn24 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn25 = new Apzon.Controls.Grid.GridColumnEx();
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dateEdit2.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateEdit2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateEdit1.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateEdit1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCardCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).BeginInit();
		this.groupControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridControl2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView2).BeginInit();
		base.SuspendLayout();
		this.panel1.Controls.Add(this.btCancel);
		this.panel1.Controls.Add(this.btSave);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel1.Location = new System.Drawing.Point(0, 335);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(728, 33);
		this.panel1.TabIndex = 67;
		this.panel1.Tag = "ss";
		this.btCancel.Appearance.Font = new System.Drawing.Font("Tahoma", 7.5f);
		this.btCancel.Appearance.Options.UseFont = true;
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Refresh";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(116, 8);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(99, 19);
		this.btCancel.TabIndex = 14;
		this.btCancel.Text = "Refresh";
		this.btSave.Appearance.Font = new System.Drawing.Font("Tahoma", 7.5f);
		this.btSave.Appearance.Options.UseFont = true;
		this.btSave.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btSave.EditControl = null;
		this.btSave.EditValue = "Save";
		this.btSave.FieldName = null;
		this.btSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btSave.ImageOptions.Image");
		this.btSave.IsSearchControl = false;
		this.btSave.Location = new System.Drawing.Point(12, 8);
		this.btSave.Name = "btSave";
		this.btSave.ReadOnly = false;
		this.btSave.SettingInVisible = false;
		this.btSave.SettingReadonly = false;
		this.btSave.Size = new System.Drawing.Size(99, 19);
		this.btSave.TabIndex = 13;
		this.btSave.Text = "Save";
		this.panelControl1.Controls.Add(this.dateEdit2);
		this.panelControl1.Controls.Add(this.dateEdit1);
		this.panelControl1.Controls.Add(this.btSearch);
		this.panelControl1.Controls.Add(this.lb_ToDate);
		this.panelControl1.Controls.Add(this.lb_FromDate);
		this.panelControl1.Controls.Add(this.txtCardCode);
		this.panelControl1.Controls.Add(this.lb_MOCode);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(728, 53);
		this.panelControl1.TabIndex = 68;
		this.dateEdit2.EditControl = null;
		this.dateEdit2.EditValue = null;
		this.dateEdit2.FieldName = null;
		this.dateEdit2.Location = new System.Drawing.Point(447, 28);
		this.dateEdit2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.dateEdit2.Name = "dateEdit2";
		this.dateEdit2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateEdit2.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateEdit2.Properties.IsSearchControl = false;
		this.dateEdit2.SettingInVisible = false;
		this.dateEdit2.SettingReadonly = false;
		this.dateEdit2.Size = new System.Drawing.Size(147, 20);
		this.dateEdit2.TabIndex = 69;
		this.dateEdit1.EditControl = null;
		this.dateEdit1.EditValue = null;
		this.dateEdit1.FieldName = null;
		this.dateEdit1.Location = new System.Drawing.Point(447, 3);
		this.dateEdit1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.dateEdit1.Name = "dateEdit1";
		this.dateEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateEdit1.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateEdit1.Properties.IsSearchControl = false;
		this.dateEdit1.SettingInVisible = false;
		this.dateEdit1.SettingReadonly = false;
		this.dateEdit1.Size = new System.Drawing.Size(147, 20);
		this.dateEdit1.TabIndex = 68;
		this.btSearch.Appearance.Font = new System.Drawing.Font("Tahoma", 7.5f);
		this.btSearch.Appearance.Options.UseFont = true;
		this.btSearch.EditControl = null;
		this.btSearch.EditValue = "Search";
		this.btSearch.FieldName = null;
		this.btSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btSearch.ImageOptions.Image");
		this.btSearch.IsSearchControl = false;
		this.btSearch.Location = new System.Drawing.Point(615, 27);
		this.btSearch.Name = "btSearch";
		this.btSearch.ReadOnly = false;
		this.btSearch.SettingInVisible = false;
		this.btSearch.SettingReadonly = false;
		this.btSearch.Size = new System.Drawing.Size(99, 19);
		this.btSearch.TabIndex = 66;
		this.btSearch.Text = "Search";
		this.lb_ToDate.EditControl = null;
		this.lb_ToDate.EditValue = "To Date";
		this.lb_ToDate.FieldName = null;
		this.lb_ToDate.HideWhenEmpty = false;
		this.lb_ToDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ToDate.Location = new System.Drawing.Point(336, 32);
		this.lb_ToDate.Name = "lb_ToDate";
		this.lb_ToDate.ReadOnly = false;
		this.lb_ToDate.SettingInVisible = false;
		this.lb_ToDate.SettingReadonly = false;
		this.lb_ToDate.ShowUnderLine = true;
		this.lb_ToDate.Size = new System.Drawing.Size(114, 20);
		this.lb_ToDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ToDate.TabIndex = 64;
		this.lb_ToDate.Utext = "To Date";
		this.lb_FromDate.EditControl = null;
		this.lb_FromDate.EditValue = "From Date";
		this.lb_FromDate.FieldName = null;
		this.lb_FromDate.HideWhenEmpty = false;
		this.lb_FromDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_FromDate.Location = new System.Drawing.Point(336, 7);
		this.lb_FromDate.Name = "lb_FromDate";
		this.lb_FromDate.ReadOnly = false;
		this.lb_FromDate.SettingInVisible = false;
		this.lb_FromDate.SettingReadonly = false;
		this.lb_FromDate.ShowUnderLine = true;
		this.lb_FromDate.Size = new System.Drawing.Size(114, 20);
		this.lb_FromDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_FromDate.TabIndex = 63;
		this.lb_FromDate.Utext = "From Date";
		this.txtCardCode.EditControl = null;
		this.txtCardCode.FieldName = null;
		this.txtCardCode.IsSearchControl = false;
		this.txtCardCode.Location = new System.Drawing.Point(127, 4);
		this.txtCardCode.Name = "txtCardCode";
		this.txtCardCode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.txtCardCode.Properties.Appearance.Options.UseFont = true;
		this.txtCardCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txtCardCode.SettingInVisible = false;
		this.txtCardCode.SettingReadonly = false;
		this.txtCardCode.Size = new System.Drawing.Size(147, 18);
		this.txtCardCode.TabIndex = 52;
		this.lb_MOCode.EditControl = null;
		this.lb_MOCode.EditValue = "M/O Code";
		this.lb_MOCode.FieldName = null;
		this.lb_MOCode.HideWhenEmpty = false;
		this.lb_MOCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_MOCode.Location = new System.Drawing.Point(12, 6);
		this.lb_MOCode.Name = "lb_MOCode";
		this.lb_MOCode.ReadOnly = false;
		this.lb_MOCode.SettingInVisible = false;
		this.lb_MOCode.SettingReadonly = false;
		this.lb_MOCode.ShowUnderLine = true;
		this.lb_MOCode.Size = new System.Drawing.Size(117, 20);
		this.lb_MOCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_MOCode.TabIndex = 49;
		this.lb_MOCode.Utext = "M/O Code";
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 53);
		this.splitContainerControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.groupControl1);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.groupControl2);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(728, 282);
		this.splitContainerControl1.SplitterPosition = 163;
		this.splitContainerControl1.TabIndex = 69;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.groupControl1.Controls.Add(this.gridControl1);
		this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl1.Location = new System.Drawing.Point(0, 0);
		this.groupControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(728, 163);
		this.groupControl1.TabIndex = 0;
		this.groupControl1.Text = "General MO To Planning";
		this.gridControl1.AllowPasteExcel = true;
		this.gridControl1.ClipboardData = null;
		this.gridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.gridControl1.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.gridControl1.Location = new System.Drawing.Point(2, 23);
		this.gridControl1.MainView = this.gridView1;
		this.gridControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.gridControl1.Name = "gridControl1";
		this.gridControl1.Size = new System.Drawing.Size(724, 138);
		this.gridControl1.TabIndex = 0;
		this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView1 });
		this.gridView1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView1.ColumnKey = null;
		this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[14]
		{
			this.gcMasterOrderCode, this.gcItemCode, this.gcItemName, this.gcOpCode, this.gcRouting, this.gcUoM, this.gcTotalDemand, this.gcLeadTime, this.gridColumn9, this.gridColumn10,
			this.gridColumn11, this.gridColumn12, this.gridColumn13, this.gridColumn14
		});
		this.gridView1.DisableFormSetting = false;
		this.gridView1.DisplayException = false;
		this.gridView1.EditColumn = null;
		this.gridView1.FieldCurrency = null;
		this.gridView1.FieldRate = null;
		this.gridView1.FormId = null;
		this.gridView1.GridControl = this.gridControl1;
		this.gridView1.IgnoreFilter = false;
		this.gridView1.IndexEditRow = -1;
		this.gridView1.IndicatorWidth = 40;
		this.gridView1.IsApzTable = false;
		this.gridView1.IsInsertNewRow = false;
		this.gridView1.IsLoadingData = false;
		this.gridView1.IsMappingContextAction = false;
		this.gridView1.ItemId = null;
		this.gridView1.LstColumnAllowBatch = null;
		this.gridView1.LstColumnNotAllowBatch = null;
		this.gridView1.LstColumnNotEditable = null;
		this.gridView1.LstColumNotSetting = null;
		this.gridView1.Name = "gridView1";
		this.gridView1.OptionsView.ColumnAutoWidth = false;
		this.gridView1.OptionsView.ShowGroupPanel = false;
		this.gridView1.UseBatchMod = false;
		this.gcMasterOrderCode.Caption = "Master Order Code";
		this.gcMasterOrderCode.CurrencyDateField = null;
		this.gcMasterOrderCode.CurrencyField = null;
		this.gcMasterOrderCode.CurrencyFixed = null;
		this.gcMasterOrderCode.CurrencyFixedRate = 0.0;
		this.gcMasterOrderCode.CurrencyRateField = null;
		this.gcMasterOrderCode.Name = "gcMasterOrderCode";
		this.gcMasterOrderCode.PrimaryKey = true;
		this.gcMasterOrderCode.UnitField = null;
		this.gcMasterOrderCode.Visible = true;
		this.gcMasterOrderCode.VisibleIndex = 0;
		this.gcMasterOrderCode.Width = 127;
		this.gcItemCode.Caption = "Item Code";
		this.gcItemCode.CurrencyDateField = null;
		this.gcItemCode.CurrencyField = null;
		this.gcItemCode.CurrencyFixed = null;
		this.gcItemCode.CurrencyFixedRate = 0.0;
		this.gcItemCode.CurrencyRateField = null;
		this.gcItemCode.Name = "gcItemCode";
		this.gcItemCode.PrimaryKey = false;
		this.gcItemCode.UnitField = null;
		this.gcItemCode.Visible = true;
		this.gcItemCode.VisibleIndex = 1;
		this.gcItemCode.Width = 119;
		this.gcItemName.Caption = "Item Name";
		this.gcItemName.CurrencyDateField = null;
		this.gcItemName.CurrencyField = null;
		this.gcItemName.CurrencyFixed = null;
		this.gcItemName.CurrencyFixedRate = 0.0;
		this.gcItemName.CurrencyRateField = null;
		this.gcItemName.Name = "gcItemName";
		this.gcItemName.PrimaryKey = false;
		this.gcItemName.UnitField = null;
		this.gcItemName.Visible = true;
		this.gcItemName.VisibleIndex = 2;
		this.gcItemName.Width = 145;
		this.gcOpCode.Caption = "Op Code";
		this.gcOpCode.CurrencyDateField = null;
		this.gcOpCode.CurrencyField = null;
		this.gcOpCode.CurrencyFixed = null;
		this.gcOpCode.CurrencyFixedRate = 0.0;
		this.gcOpCode.CurrencyRateField = null;
		this.gcOpCode.Name = "gcOpCode";
		this.gcOpCode.PrimaryKey = false;
		this.gcOpCode.UnitField = null;
		this.gcOpCode.Visible = true;
		this.gcOpCode.VisibleIndex = 3;
		this.gcRouting.Caption = "Routing";
		this.gcRouting.CurrencyDateField = null;
		this.gcRouting.CurrencyField = null;
		this.gcRouting.CurrencyFixed = null;
		this.gcRouting.CurrencyFixedRate = 0.0;
		this.gcRouting.CurrencyRateField = null;
		this.gcRouting.Name = "gcRouting";
		this.gcRouting.PrimaryKey = false;
		this.gcRouting.UnitField = null;
		this.gcRouting.Visible = true;
		this.gcRouting.VisibleIndex = 4;
		this.gcUoM.Caption = "UoM";
		this.gcUoM.CurrencyDateField = null;
		this.gcUoM.CurrencyField = null;
		this.gcUoM.CurrencyFixed = null;
		this.gcUoM.CurrencyFixedRate = 0.0;
		this.gcUoM.CurrencyRateField = null;
		this.gcUoM.Name = "gcUoM";
		this.gcUoM.PrimaryKey = false;
		this.gcUoM.UnitField = null;
		this.gcUoM.Visible = true;
		this.gcUoM.VisibleIndex = 5;
		this.gcTotalDemand.Caption = "Total Demand";
		this.gcTotalDemand.CurrencyDateField = null;
		this.gcTotalDemand.CurrencyField = null;
		this.gcTotalDemand.CurrencyFixed = null;
		this.gcTotalDemand.CurrencyFixedRate = 0.0;
		this.gcTotalDemand.CurrencyRateField = null;
		this.gcTotalDemand.Name = "gcTotalDemand";
		this.gcTotalDemand.PrimaryKey = false;
		this.gcTotalDemand.UnitField = null;
		this.gcTotalDemand.Visible = true;
		this.gcTotalDemand.VisibleIndex = 6;
		this.gcTotalDemand.Width = 98;
		this.gcLeadTime.Caption = "Lead Time";
		this.gcLeadTime.CurrencyDateField = null;
		this.gcLeadTime.CurrencyField = null;
		this.gcLeadTime.CurrencyFixed = null;
		this.gcLeadTime.CurrencyFixedRate = 0.0;
		this.gcLeadTime.CurrencyRateField = null;
		this.gcLeadTime.Name = "gcLeadTime";
		this.gcLeadTime.PrimaryKey = false;
		this.gcLeadTime.UnitField = null;
		this.gcLeadTime.Visible = true;
		this.gcLeadTime.VisibleIndex = 7;
		this.gridColumn9.Caption = "01/10";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 8;
		this.gridColumn10.Caption = "02/10";
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 9;
		this.gridColumn11.Caption = "03/10";
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.UnitField = null;
		this.gridColumn11.Visible = true;
		this.gridColumn11.VisibleIndex = 10;
		this.gridColumn12.Caption = "04/10";
		this.gridColumn12.CurrencyDateField = null;
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.PrimaryKey = false;
		this.gridColumn12.UnitField = null;
		this.gridColumn12.Visible = true;
		this.gridColumn12.VisibleIndex = 11;
		this.gridColumn13.Caption = "05/10";
		this.gridColumn13.CurrencyDateField = null;
		this.gridColumn13.CurrencyField = null;
		this.gridColumn13.CurrencyFixed = null;
		this.gridColumn13.CurrencyFixedRate = 0.0;
		this.gridColumn13.CurrencyRateField = null;
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.PrimaryKey = false;
		this.gridColumn13.UnitField = null;
		this.gridColumn13.Visible = true;
		this.gridColumn13.VisibleIndex = 12;
		this.gridColumn14.Caption = "06/10";
		this.gridColumn14.CurrencyDateField = null;
		this.gridColumn14.CurrencyField = null;
		this.gridColumn14.CurrencyFixed = null;
		this.gridColumn14.CurrencyFixedRate = 0.0;
		this.gridColumn14.CurrencyRateField = null;
		this.gridColumn14.Name = "gridColumn14";
		this.gridColumn14.PrimaryKey = false;
		this.gridColumn14.UnitField = null;
		this.gridColumn14.Visible = true;
		this.gridColumn14.VisibleIndex = 13;
		this.groupControl2.Controls.Add(this.gridControl2);
		this.groupControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl2.Location = new System.Drawing.Point(0, 0);
		this.groupControl2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.groupControl2.Name = "groupControl2";
		this.groupControl2.Size = new System.Drawing.Size(728, 109);
		this.groupControl2.TabIndex = 1;
		this.groupControl2.Text = "Resource Load";
		this.gridControl2.AllowPasteExcel = false;
		this.gridControl2.ClipboardData = null;
		this.gridControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.gridControl2.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.gridControl2.Location = new System.Drawing.Point(2, 23);
		this.gridControl2.MainView = this.gridView2;
		this.gridControl2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.gridControl2.Name = "gridControl2";
		this.gridControl2.Size = new System.Drawing.Size(724, 84);
		this.gridControl2.TabIndex = 1;
		this.gridControl2.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView2 });
		this.gridView2.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView2.ColumnKey = null;
		this.gridView2.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[11]
		{
			this.gcResouceNo, this.gcResouceName, this.gcLine_UoM, this.gcPlant, this.gcDescription, this.gridColumn20, this.gridColumn21, this.gridColumn22, this.gridColumn23, this.gridColumn24,
			this.gridColumn25
		});
		this.gridView2.DisableFormSetting = false;
		this.gridView2.DisplayException = false;
		this.gridView2.EditColumn = null;
		this.gridView2.FieldCurrency = null;
		this.gridView2.FieldRate = null;
		this.gridView2.FormId = null;
		this.gridView2.GridControl = this.gridControl2;
		this.gridView2.IgnoreFilter = false;
		this.gridView2.IndexEditRow = -1;
		this.gridView2.IndicatorWidth = 40;
		this.gridView2.IsApzTable = false;
		this.gridView2.IsInsertNewRow = false;
		this.gridView2.IsLoadingData = false;
		this.gridView2.IsMappingContextAction = false;
		this.gridView2.ItemId = null;
		this.gridView2.LstColumnAllowBatch = null;
		this.gridView2.LstColumnNotAllowBatch = null;
		this.gridView2.LstColumnNotEditable = null;
		this.gridView2.LstColumNotSetting = null;
		this.gridView2.Name = "gridView2";
		this.gridView2.OptionsView.ColumnAutoWidth = false;
		this.gridView2.OptionsView.ShowGroupPanel = false;
		this.gridView2.UseBatchMod = false;
		this.gcResouceNo.Caption = "Resouce No";
		this.gcResouceNo.CurrencyDateField = null;
		this.gcResouceNo.CurrencyField = null;
		this.gcResouceNo.CurrencyFixed = null;
		this.gcResouceNo.CurrencyFixedRate = 0.0;
		this.gcResouceNo.CurrencyRateField = null;
		this.gcResouceNo.Name = "gcResouceNo";
		this.gcResouceNo.PrimaryKey = true;
		this.gcResouceNo.UnitField = null;
		this.gcResouceNo.Visible = true;
		this.gcResouceNo.VisibleIndex = 0;
		this.gcResouceNo.Width = 80;
		this.gcResouceName.Caption = "Resouce Name";
		this.gcResouceName.CurrencyDateField = null;
		this.gcResouceName.CurrencyField = null;
		this.gcResouceName.CurrencyFixed = null;
		this.gcResouceName.CurrencyFixedRate = 0.0;
		this.gcResouceName.CurrencyRateField = null;
		this.gcResouceName.Name = "gcResouceName";
		this.gcResouceName.PrimaryKey = false;
		this.gcResouceName.UnitField = null;
		this.gcResouceName.Visible = true;
		this.gcResouceName.VisibleIndex = 1;
		this.gcResouceName.Width = 135;
		this.gcLine_UoM.Caption = "UoM";
		this.gcLine_UoM.CurrencyDateField = null;
		this.gcLine_UoM.CurrencyField = null;
		this.gcLine_UoM.CurrencyFixed = null;
		this.gcLine_UoM.CurrencyFixedRate = 0.0;
		this.gcLine_UoM.CurrencyRateField = null;
		this.gcLine_UoM.Name = "gcLine_UoM";
		this.gcLine_UoM.PrimaryKey = false;
		this.gcLine_UoM.UnitField = null;
		this.gcLine_UoM.Visible = true;
		this.gcLine_UoM.VisibleIndex = 2;
		this.gcPlant.Caption = "Plant";
		this.gcPlant.CurrencyDateField = null;
		this.gcPlant.CurrencyField = null;
		this.gcPlant.CurrencyFixed = null;
		this.gcPlant.CurrencyFixedRate = 0.0;
		this.gcPlant.CurrencyRateField = null;
		this.gcPlant.Name = "gcPlant";
		this.gcPlant.PrimaryKey = false;
		this.gcPlant.UnitField = null;
		this.gcPlant.Visible = true;
		this.gcPlant.VisibleIndex = 3;
		this.gcDescription.Caption = "Description";
		this.gcDescription.CurrencyDateField = null;
		this.gcDescription.CurrencyField = null;
		this.gcDescription.CurrencyFixed = null;
		this.gcDescription.CurrencyFixedRate = 0.0;
		this.gcDescription.CurrencyRateField = null;
		this.gcDescription.Name = "gcDescription";
		this.gcDescription.PrimaryKey = false;
		this.gcDescription.UnitField = null;
		this.gcDescription.Visible = true;
		this.gcDescription.VisibleIndex = 4;
		this.gcDescription.Width = 149;
		this.gridColumn20.Caption = "01/10";
		this.gridColumn20.CurrencyDateField = null;
		this.gridColumn20.CurrencyField = null;
		this.gridColumn20.CurrencyFixed = null;
		this.gridColumn20.CurrencyFixedRate = 0.0;
		this.gridColumn20.CurrencyRateField = null;
		this.gridColumn20.Name = "gridColumn20";
		this.gridColumn20.PrimaryKey = false;
		this.gridColumn20.UnitField = null;
		this.gridColumn20.Visible = true;
		this.gridColumn20.VisibleIndex = 5;
		this.gridColumn21.Caption = "02/10";
		this.gridColumn21.CurrencyDateField = null;
		this.gridColumn21.CurrencyField = null;
		this.gridColumn21.CurrencyFixed = null;
		this.gridColumn21.CurrencyFixedRate = 0.0;
		this.gridColumn21.CurrencyRateField = null;
		this.gridColumn21.Name = "gridColumn21";
		this.gridColumn21.PrimaryKey = false;
		this.gridColumn21.UnitField = null;
		this.gridColumn21.Visible = true;
		this.gridColumn21.VisibleIndex = 6;
		this.gridColumn22.Caption = "03/10";
		this.gridColumn22.CurrencyDateField = null;
		this.gridColumn22.CurrencyField = null;
		this.gridColumn22.CurrencyFixed = null;
		this.gridColumn22.CurrencyFixedRate = 0.0;
		this.gridColumn22.CurrencyRateField = null;
		this.gridColumn22.Name = "gridColumn22";
		this.gridColumn22.PrimaryKey = false;
		this.gridColumn22.UnitField = null;
		this.gridColumn22.Visible = true;
		this.gridColumn22.VisibleIndex = 7;
		this.gridColumn23.Caption = "04/10";
		this.gridColumn23.CurrencyDateField = null;
		this.gridColumn23.CurrencyField = null;
		this.gridColumn23.CurrencyFixed = null;
		this.gridColumn23.CurrencyFixedRate = 0.0;
		this.gridColumn23.CurrencyRateField = null;
		this.gridColumn23.Name = "gridColumn23";
		this.gridColumn23.PrimaryKey = false;
		this.gridColumn23.UnitField = null;
		this.gridColumn23.Visible = true;
		this.gridColumn23.VisibleIndex = 8;
		this.gridColumn24.Caption = "05/10";
		this.gridColumn24.CurrencyDateField = null;
		this.gridColumn24.CurrencyField = null;
		this.gridColumn24.CurrencyFixed = null;
		this.gridColumn24.CurrencyFixedRate = 0.0;
		this.gridColumn24.CurrencyRateField = null;
		this.gridColumn24.Name = "gridColumn24";
		this.gridColumn24.PrimaryKey = false;
		this.gridColumn24.UnitField = null;
		this.gridColumn24.Visible = true;
		this.gridColumn24.VisibleIndex = 9;
		this.gridColumn25.Caption = "06/10";
		this.gridColumn25.CurrencyDateField = null;
		this.gridColumn25.CurrencyField = null;
		this.gridColumn25.CurrencyFixed = null;
		this.gridColumn25.CurrencyFixedRate = 0.0;
		this.gridColumn25.CurrencyRateField = null;
		this.gridColumn25.Name = "gridColumn25";
		this.gridColumn25.PrimaryKey = false;
		this.gridColumn25.UnitField = null;
		this.gridColumn25.Visible = true;
		this.gridColumn25.VisibleIndex = 10;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(728, 368);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.panel1);
		base.Controls.Add(this.panelControl1);
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		base.Name = "frm_ProductionScheduling";
		base.ShowInTaskbar = false;
		this.Text = "Production Scheduling";
		this.panel1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dateEdit2.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateEdit2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateEdit1.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateEdit1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCardCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).EndInit();
		this.groupControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridControl2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView2).EndInit();
		base.ResumeLayout(false);
	}
}
