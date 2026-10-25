using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Commons;
using Apzon.Global;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Inventory.PriceLists;

public class frm_PeriodDiscounts : XtraFormEx
{
	private string _privilege = "";

	private DataTable dt = new DataTable();

	private DataTable dtObj = new DataTable();

	private DataTable dtOriginal = new DataTable();

	private List<string> lstChanged = new List<string>();

	private DataView clone;

	private IContainer components;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private GridColumnEx gcExpand;

	private GridColumnEx gcAutoUpdt;

	private RepositoryItemDateEdit repositoryItemDateEdit2;

	private GridColumnEx gcToDate;

	private RepositoryItemDateEdit repositoryItemDateEdit1;

	private GridColumnEx gcFromDate;

	private RepositoryItemLookUpEdit rLU_PriceList;

	private GridColumnEx gcListNum;

	private RepositoryItemCalcEdit repositoryItemCalcEdit2_Discount;

	private GridColumnEx gcDiscount;

	private GridColumnEx gcCurrency;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private GridColumnEx gcPriceAfterDiscount;

	private GridColumnEx gcLINENUM;

	private GridColumnEx gcCardCode;

	private GridColumnEx gcItemCode;

	private GridViewEx gvDocLineSPP1;

	private GridControlEx grdDocLineSPP1;

	private SimpleButtonEx btOK;

	private SimpleButtonEx bt_Cancel;

	private Panel panel1;

	private SimpleButtonEx bt_CopyDiscuount;

	public frm_PeriodDiscounts()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_PeriodDiscounts"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_PeriodDiscounts" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				_privilege = "R";
			}
			else
			{
				_privilege = "F";
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void frm_PeriodDiscounts_Load(object sender, EventArgs e)
	{
	}

	private void bt_Cancel_Click(object sender, EventArgs e)
	{
		Close();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.PriceLists.frm_PeriodDiscounts));
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gcExpand = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAutoUpdt = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcToDate = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcFromDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rLU_PriceList = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcListNum = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit2_Discount = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcDiscount = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCurrency = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcPriceAfterDiscount = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLINENUM = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gvDocLineSPP1 = new Apzon.Controls.Grid.GridViewEx();
		this.grdDocLineSPP1 = new Apzon.Controls.Grid.GridControlEx();
		this.btOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.panel1 = new System.Windows.Forms.Panel();
		this.bt_CopyDiscuount = new Apzon.Controls.Controls.SimpleButtonEx();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLU_PriceList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2_Discount).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDocLineSPP1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdDocLineSPP1).BeginInit();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(134, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.gcExpand.Caption = "Expand";
		this.gcExpand.CurrencyDateField = null;
		this.gcExpand.CurrencyField = null;
		this.gcExpand.CurrencyFixed = null;
		this.gcExpand.CurrencyFixedRate = 0.0;
		this.gcExpand.CurrencyRateField = null;
		this.gcExpand.FieldName = "Expand";
		this.gcExpand.Name = "gcExpand";
		this.gcExpand.PrimaryKey = false;
		this.gcExpand.UnitField = null;
		this.gcExpand.Width = 48;
		this.gcAutoUpdt.Caption = "AutoUpdt";
		this.gcAutoUpdt.CurrencyDateField = null;
		this.gcAutoUpdt.CurrencyField = null;
		this.gcAutoUpdt.CurrencyFixed = null;
		this.gcAutoUpdt.CurrencyFixedRate = 0.0;
		this.gcAutoUpdt.CurrencyRateField = null;
		this.gcAutoUpdt.FieldName = "AutoUpdt";
		this.gcAutoUpdt.Name = "gcAutoUpdt";
		this.gcAutoUpdt.PrimaryKey = false;
		this.gcAutoUpdt.UnitField = null;
		this.gcAutoUpdt.Width = 58;
		this.repositoryItemDateEdit2.AutoHeight = false;
		this.repositoryItemDateEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit2.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit2.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEdit2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEdit2.EditFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEdit2.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEdit2.Mask.EditMask = "dd/MM/yyyy";
		this.repositoryItemDateEdit2.Name = "repositoryItemDateEdit2";
		this.gcToDate.Caption = "Valid until";
		this.gcToDate.ColumnEdit = this.repositoryItemDateEdit2;
		this.gcToDate.CurrencyDateField = null;
		this.gcToDate.CurrencyField = null;
		this.gcToDate.CurrencyFixed = null;
		this.gcToDate.CurrencyFixedRate = 0.0;
		this.gcToDate.CurrencyRateField = null;
		this.gcToDate.FieldName = "ToDate";
		this.gcToDate.Name = "gcToDate";
		this.gcToDate.PrimaryKey = false;
		this.gcToDate.UnitField = null;
		this.gcToDate.Visible = true;
		this.gcToDate.VisibleIndex = 1;
		this.gcToDate.Width = 97;
		this.repositoryItemDateEdit1.AutoHeight = false;
		this.repositoryItemDateEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEdit1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEdit1.EditFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEdit1.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEdit1.Mask.EditMask = "dd/MM/yyyy";
		this.repositoryItemDateEdit1.Name = "repositoryItemDateEdit1";
		this.gcFromDate.Caption = "Valid from";
		this.gcFromDate.ColumnEdit = this.repositoryItemDateEdit1;
		this.gcFromDate.CurrencyDateField = null;
		this.gcFromDate.CurrencyField = null;
		this.gcFromDate.CurrencyFixed = null;
		this.gcFromDate.CurrencyFixedRate = 0.0;
		this.gcFromDate.CurrencyRateField = null;
		this.gcFromDate.FieldName = "FromDate";
		this.gcFromDate.Name = "gcFromDate";
		this.gcFromDate.PrimaryKey = false;
		this.gcFromDate.UnitField = null;
		this.gcFromDate.Visible = true;
		this.gcFromDate.VisibleIndex = 0;
		this.gcFromDate.Width = 93;
		this.rLU_PriceList.AutoHeight = false;
		this.rLU_PriceList.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLU_PriceList.DisplayMember = "ListName";
		this.rLU_PriceList.Name = "rLU_PriceList";
		this.rLU_PriceList.NullText = "";
		this.rLU_PriceList.ValueMember = "ListNum";
		this.gcListNum.Caption = "Price list";
		this.gcListNum.ColumnEdit = this.rLU_PriceList;
		this.gcListNum.CurrencyDateField = null;
		this.gcListNum.CurrencyField = null;
		this.gcListNum.CurrencyFixed = null;
		this.gcListNum.CurrencyFixedRate = 0.0;
		this.gcListNum.CurrencyRateField = null;
		this.gcListNum.FieldName = "ListNum";
		this.gcListNum.Name = "gcListNum";
		this.gcListNum.PrimaryKey = false;
		this.gcListNum.UnitField = null;
		this.gcListNum.Visible = true;
		this.gcListNum.VisibleIndex = 2;
		this.gcListNum.Width = 143;
		this.repositoryItemCalcEdit2_Discount.AutoHeight = false;
		this.repositoryItemCalcEdit2_Discount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit2_Discount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemCalcEdit2_Discount.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemCalcEdit2_Discount.Name = "repositoryItemCalcEdit2_Discount";
		this.gcDiscount.Caption = "Discount";
		this.gcDiscount.ColumnEdit = this.repositoryItemCalcEdit2_Discount;
		this.gcDiscount.CurrencyDateField = null;
		this.gcDiscount.CurrencyField = null;
		this.gcDiscount.CurrencyFixed = null;
		this.gcDiscount.CurrencyFixedRate = 0.0;
		this.gcDiscount.CurrencyRateField = null;
		this.gcDiscount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcDiscount.FieldName = "Discount";
		this.gcDiscount.Name = "gcDiscount";
		this.gcDiscount.PrimaryKey = false;
		this.gcDiscount.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gcDiscount.UnitField = null;
		this.gcDiscount.Visible = true;
		this.gcDiscount.VisibleIndex = 3;
		this.gcDiscount.Width = 141;
		this.gcCurrency.Caption = "Currency";
		this.gcCurrency.CurrencyDateField = null;
		this.gcCurrency.CurrencyField = null;
		this.gcCurrency.CurrencyFixed = null;
		this.gcCurrency.CurrencyFixedRate = 0.0;
		this.gcCurrency.CurrencyRateField = null;
		this.gcCurrency.FieldName = "Currency";
		this.gcCurrency.Name = "gcCurrency";
		this.gcCurrency.OptionsColumn.ReadOnly = true;
		this.gcCurrency.PrimaryKey = false;
		this.gcCurrency.UnitField = null;
		this.gcCurrency.Visible = true;
		this.gcCurrency.VisibleIndex = 5;
		this.gcCurrency.Width = 77;
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemCalcEdit1.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.gcPriceAfterDiscount.Caption = "Price after Discount";
		this.gcPriceAfterDiscount.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gcPriceAfterDiscount.CurrencyDateField = null;
		this.gcPriceAfterDiscount.CurrencyField = null;
		this.gcPriceAfterDiscount.CurrencyFixed = null;
		this.gcPriceAfterDiscount.CurrencyFixedRate = 0.0;
		this.gcPriceAfterDiscount.CurrencyRateField = null;
		this.gcPriceAfterDiscount.FieldName = "Price";
		this.gcPriceAfterDiscount.Name = "gcPriceAfterDiscount";
		this.gcPriceAfterDiscount.PrimaryKey = false;
		this.gcPriceAfterDiscount.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gcPriceAfterDiscount.UnitField = null;
		this.gcPriceAfterDiscount.Visible = true;
		this.gcPriceAfterDiscount.VisibleIndex = 4;
		this.gcPriceAfterDiscount.Width = 115;
		this.gcLINENUM.Caption = "LINENUM";
		this.gcLINENUM.CurrencyDateField = null;
		this.gcLINENUM.CurrencyField = null;
		this.gcLINENUM.CurrencyFixed = null;
		this.gcLINENUM.CurrencyFixedRate = 0.0;
		this.gcLINENUM.CurrencyRateField = null;
		this.gcLINENUM.FieldName = "LINENUM";
		this.gcLINENUM.Name = "gcLINENUM";
		this.gcLINENUM.PrimaryKey = false;
		this.gcLINENUM.UnitField = null;
		this.gcLINENUM.Width = 54;
		this.gcCardCode.Caption = "CardCode";
		this.gcCardCode.CurrencyDateField = null;
		this.gcCardCode.CurrencyField = null;
		this.gcCardCode.CurrencyFixed = null;
		this.gcCardCode.CurrencyFixedRate = 0.0;
		this.gcCardCode.CurrencyRateField = null;
		this.gcCardCode.FieldName = "CardCode";
		this.gcCardCode.Name = "gcCardCode";
		this.gcCardCode.PrimaryKey = false;
		this.gcCardCode.UnitField = null;
		this.gcCardCode.Width = 69;
		this.gcItemCode.Caption = "ItemCode";
		this.gcItemCode.CurrencyDateField = null;
		this.gcItemCode.CurrencyField = null;
		this.gcItemCode.CurrencyFixed = null;
		this.gcItemCode.CurrencyFixedRate = 0.0;
		this.gcItemCode.CurrencyRateField = null;
		this.gcItemCode.FieldName = "ItemCode";
		this.gcItemCode.Name = "gcItemCode";
		this.gcItemCode.PrimaryKey = false;
		this.gcItemCode.UnitField = null;
		this.gcItemCode.Width = 70;
		this.gvDocLineSPP1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDocLineSPP1.ColumnKey = null;
		this.gvDocLineSPP1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[11]
		{
			this.gcItemCode, this.gcCardCode, this.gcLINENUM, this.gcPriceAfterDiscount, this.gcCurrency, this.gcDiscount, this.gcListNum, this.gcFromDate, this.gcToDate, this.gcAutoUpdt,
			this.gcExpand
		});
		this.gvDocLineSPP1.DisableFormSetting = false;
		this.gvDocLineSPP1.DisplayException = false;
		this.gvDocLineSPP1.EditColumn = null;
		this.gvDocLineSPP1.FieldCurrency = null;
		this.gvDocLineSPP1.FieldRate = null;
		this.gvDocLineSPP1.FormId = "oPeriodDiscount";
		this.gvDocLineSPP1.GridControl = this.grdDocLineSPP1;
		this.gvDocLineSPP1.IgnoreFilter = false;
		this.gvDocLineSPP1.IndexEditRow = -1;
		this.gvDocLineSPP1.IndicatorWidth = 35;
		this.gvDocLineSPP1.IsApzTable = false;
		this.gvDocLineSPP1.IsInsertNewRow = false;
		this.gvDocLineSPP1.IsLoadingData = false;
		this.gvDocLineSPP1.IsMappingContextAction = false;
		this.gvDocLineSPP1.ItemId = "SPP1";
		this.gvDocLineSPP1.LstColumnAllowBatch = null;
		this.gvDocLineSPP1.LstColumnNotAllowBatch = null;
		this.gvDocLineSPP1.LstColumnNotEditable = null;
		this.gvDocLineSPP1.LstColumNotSetting = null;
		this.gvDocLineSPP1.Name = "gvDocLineSPP1";
		this.gvDocLineSPP1.OptionsView.ColumnAutoWidth = false;
		this.gvDocLineSPP1.OptionsView.ShowFooter = true;
		this.gvDocLineSPP1.OptionsView.ShowGroupPanel = false;
		this.gvDocLineSPP1.UseBatchMod = false;
		this.grdDocLineSPP1.AllowPasteExcel = true;
		this.grdDocLineSPP1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDocLineSPP1.ClipboardData = null;
		this.grdDocLineSPP1.Location = new System.Drawing.Point(0, 0);
		this.grdDocLineSPP1.MainView = this.gvDocLineSPP1;
		this.grdDocLineSPP1.Name = "grdDocLineSPP1";
		this.grdDocLineSPP1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rLU_PriceList, this.repositoryItemDateEdit1, this.repositoryItemDateEdit2, this.repositoryItemCalcEdit1, this.repositoryItemCalcEdit2_Discount });
		this.grdDocLineSPP1.Size = new System.Drawing.Size(510, 274);
		this.grdDocLineSPP1.TabIndex = 0;
		this.grdDocLineSPP1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDocLineSPP1 });
		this.btOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOK.EditControl = null;
		this.btOK.EditValue = "OK";
		this.btOK.FieldName = null;
		this.btOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOK.ImageOptions.Image");
		this.btOK.IsSearchControl = false;
		this.btOK.Location = new System.Drawing.Point(0, 280);
		this.btOK.Name = "btOK";
		this.btOK.ReadOnly = false;
		this.btOK.SettingInVisible = false;
		this.btOK.SettingReadonly = false;
		this.btOK.Size = new System.Drawing.Size(99, 20);
		this.btOK.TabIndex = 38;
		this.btOK.Text = "OK";
		this.bt_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.bt_Cancel.EditControl = null;
		this.bt_Cancel.EditValue = "Cancel";
		this.bt_Cancel.FieldName = null;
		this.bt_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_Cancel.ImageOptions.Image");
		this.bt_Cancel.IsSearchControl = false;
		this.bt_Cancel.Location = new System.Drawing.Point(102, 280);
		this.bt_Cancel.Name = "bt_Cancel";
		this.bt_Cancel.ReadOnly = false;
		this.bt_Cancel.SettingInVisible = false;
		this.bt_Cancel.SettingReadonly = false;
		this.bt_Cancel.Size = new System.Drawing.Size(99, 20);
		this.bt_Cancel.TabIndex = 39;
		this.bt_Cancel.Text = "Cancel";
		this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
		this.panel1.Controls.Add(this.bt_CopyDiscuount);
		this.panel1.Controls.Add(this.btOK);
		this.panel1.Controls.Add(this.bt_Cancel);
		this.panel1.Controls.Add(this.grdDocLineSPP1);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel1.Location = new System.Drawing.Point(0, 0);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(510, 303);
		this.panel1.TabIndex = 1;
		this.bt_CopyDiscuount.EditControl = null;
		this.bt_CopyDiscuount.EditValue = "Copy Discounts";
		this.bt_CopyDiscuount.FieldName = null;
		this.bt_CopyDiscuount.IsSearchControl = false;
		this.bt_CopyDiscuount.Location = new System.Drawing.Point(395, 277);
		this.bt_CopyDiscuount.Name = "bt_CopyDiscuount";
		this.bt_CopyDiscuount.ReadOnly = false;
		this.bt_CopyDiscuount.SettingInVisible = false;
		this.bt_CopyDiscuount.SettingReadonly = false;
		this.bt_CopyDiscuount.Size = new System.Drawing.Size(112, 23);
		this.bt_CopyDiscuount.TabIndex = 40;
		this.bt_CopyDiscuount.Text = "Copy Discounts";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(510, 303);
		base.Controls.Add(this.panel1);
		base.FormId = "oPeriodDiscount";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_PeriodDiscounts";
		this.Text = "Period Discounts";
		base.Load += new System.EventHandler(this.frm_PeriodDiscounts_Load);
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLU_PriceList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2_Discount).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDocLineSPP1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdDocLineSPP1).EndInit();
		this.panel1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
