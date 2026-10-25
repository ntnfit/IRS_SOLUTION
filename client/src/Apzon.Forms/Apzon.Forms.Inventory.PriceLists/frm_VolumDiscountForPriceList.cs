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
using Apzon.Commons.Properties;
using Apzon.Controls;
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

public class frm_VolumDiscountForPriceList : XtraFormEx
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

	private SimpleButtonEx btOK;

	private SimpleButtonEx btCancel;

	private GridColumnEx gcCurrency;

	private GridColumnEx gcPrice;

	private RepositoryItemCalcEdit repositoryItemCalcEdit2_Discount;

	private GridColumnEx gcDiscount;

	private GridColumnEx gcAmount;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1_Amount;

	private GridColumnEx gcSPP2LNum;

	private GridColumnEx gcLINENUM;

	private GridColumnEx gcCardCode;

	private GridColumnEx gcItemCode;

	private GridViewEx gvDocLineSPP2;

	private GridControlEx grdDocLineSPP2;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btOK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btOK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_VolumDiscountForPriceList()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_VolumDiscountForPriceList"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_VolumDiscountForPriceList" && x.authoriza == "F"))
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

	private void frm_VolumDiscountForPriceList_Load(object sender, EventArgs e)
	{
	}

	private void bt_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btOK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				await Save();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Save()
	{
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			gvDocLineSPP2.DeleteSelectedRows();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.PriceLists.frm_VolumDiscountForPriceList));
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.btOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.gcCurrency = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPrice = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit2_Discount = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcDiscount = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAmount = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1_Amount = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcSPP2LNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLINENUM = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gvDocLineSPP2 = new Apzon.Controls.Grid.GridViewEx();
		this.grdDocLineSPP2 = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2_Discount).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1_Amount).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDocLineSPP2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdDocLineSPP2).BeginInit();
		base.SuspendLayout();
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(134, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.btOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOK.EditControl = null;
		this.btOK.EditValue = "OK";
		this.btOK.FieldName = null;
		this.btOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOK.ImageOptions.Image");
		this.btOK.IsSearchControl = false;
		this.btOK.Location = new System.Drawing.Point(2, 284);
		this.btOK.Name = "btOK";
		this.btOK.ReadOnly = false;
		this.btOK.SettingInVisible = false;
		this.btOK.SettingReadonly = false;
		this.btOK.Size = new System.Drawing.Size(77, 20);
		this.btOK.TabIndex = 43;
		this.btOK.Text = "OK";
		this.btOK.Click += new System.EventHandler(this.btOK_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(83, 284);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(77, 20);
		this.btCancel.TabIndex = 44;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.bt_Cancel_Click);
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
		this.gcCurrency.VisibleIndex = 3;
		this.gcCurrency.Width = 116;
		this.gcPrice.Caption = "Special price";
		this.gcPrice.CurrencyDateField = null;
		this.gcPrice.CurrencyField = null;
		this.gcPrice.CurrencyFixed = null;
		this.gcPrice.CurrencyFixedRate = 0.0;
		this.gcPrice.CurrencyRateField = null;
		this.gcPrice.FieldName = "Price";
		this.gcPrice.Name = "gcPrice";
		this.gcPrice.PrimaryKey = false;
		this.gcPrice.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gcPrice.UnitField = null;
		this.gcPrice.Visible = true;
		this.gcPrice.VisibleIndex = 2;
		this.gcPrice.Width = 126;
		this.repositoryItemCalcEdit2_Discount.AutoHeight = false;
		this.repositoryItemCalcEdit2_Discount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit2_Discount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemCalcEdit2_Discount.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemCalcEdit2_Discount.Name = "repositoryItemCalcEdit2_Discount";
		this.gcDiscount.Caption = "Disc . in %";
		this.gcDiscount.ColumnEdit = this.repositoryItemCalcEdit2_Discount;
		this.gcDiscount.CurrencyDateField = null;
		this.gcDiscount.CurrencyField = null;
		this.gcDiscount.CurrencyFixed = null;
		this.gcDiscount.CurrencyFixedRate = 0.0;
		this.gcDiscount.CurrencyRateField = null;
		this.gcDiscount.FieldName = "Discount";
		this.gcDiscount.Name = "gcDiscount";
		this.gcDiscount.PrimaryKey = false;
		this.gcDiscount.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gcDiscount.UnitField = null;
		this.gcDiscount.Visible = true;
		this.gcDiscount.VisibleIndex = 1;
		this.gcDiscount.Width = 132;
		this.gcAmount.Caption = "Quantity";
		this.gcAmount.ColumnEdit = this.repositoryItemCalcEdit1_Amount;
		this.gcAmount.CurrencyDateField = null;
		this.gcAmount.CurrencyField = null;
		this.gcAmount.CurrencyFixed = null;
		this.gcAmount.CurrencyFixedRate = 0.0;
		this.gcAmount.CurrencyRateField = null;
		this.gcAmount.FieldName = "Amount";
		this.gcAmount.Name = "gcAmount";
		this.gcAmount.PrimaryKey = false;
		this.gcAmount.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gcAmount.UnitField = null;
		this.gcAmount.Visible = true;
		this.gcAmount.VisibleIndex = 0;
		this.gcAmount.Width = 109;
		this.repositoryItemCalcEdit1_Amount.AutoHeight = false;
		this.repositoryItemCalcEdit1_Amount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1_Amount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemCalcEdit1_Amount.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemCalcEdit1_Amount.Name = "repositoryItemCalcEdit1_Amount";
		this.gcSPP2LNum.Caption = "gcSPP2LNum";
		this.gcSPP2LNum.CurrencyDateField = null;
		this.gcSPP2LNum.CurrencyField = null;
		this.gcSPP2LNum.CurrencyFixed = null;
		this.gcSPP2LNum.CurrencyFixedRate = 0.0;
		this.gcSPP2LNum.CurrencyRateField = null;
		this.gcSPP2LNum.FieldName = "SPP2LNum";
		this.gcSPP2LNum.Name = "gcSPP2LNum";
		this.gcSPP2LNum.PrimaryKey = false;
		this.gcSPP2LNum.UnitField = null;
		this.gcLINENUM.Caption = "SPP1LNum";
		this.gcLINENUM.CurrencyDateField = null;
		this.gcLINENUM.CurrencyField = null;
		this.gcLINENUM.CurrencyFixed = null;
		this.gcLINENUM.CurrencyFixedRate = 0.0;
		this.gcLINENUM.CurrencyRateField = null;
		this.gcLINENUM.FieldName = "LINENUM";
		this.gcLINENUM.Name = "gcLINENUM";
		this.gcLINENUM.PrimaryKey = false;
		this.gcLINENUM.UnitField = null;
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
		this.gvDocLineSPP2.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDocLineSPP2.ColumnKey = null;
		this.gvDocLineSPP2.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gcItemCode, this.gcCardCode, this.gcLINENUM, this.gcSPP2LNum, this.gcAmount, this.gcDiscount, this.gcPrice, this.gcCurrency });
		this.gvDocLineSPP2.DisableFormSetting = false;
		this.gvDocLineSPP2.DisplayException = false;
		this.gvDocLineSPP2.EditColumn = null;
		this.gvDocLineSPP2.FieldCurrency = null;
		this.gvDocLineSPP2.FieldRate = null;
		this.gvDocLineSPP2.FormId = "oVolumnDiscountForPriceList";
		this.gvDocLineSPP2.GridControl = this.grdDocLineSPP2;
		this.gvDocLineSPP2.IgnoreFilter = false;
		this.gvDocLineSPP2.IndexEditRow = -1;
		this.gvDocLineSPP2.IndicatorWidth = 35;
		this.gvDocLineSPP2.IsApzTable = false;
		this.gvDocLineSPP2.IsInsertNewRow = false;
		this.gvDocLineSPP2.IsLoadingData = false;
		this.gvDocLineSPP2.IsMappingContextAction = false;
		this.gvDocLineSPP2.ItemId = "SPP2";
		this.gvDocLineSPP2.LstColumnAllowBatch = null;
		this.gvDocLineSPP2.LstColumnNotAllowBatch = null;
		this.gvDocLineSPP2.LstColumnNotEditable = null;
		this.gvDocLineSPP2.LstColumNotSetting = null;
		this.gvDocLineSPP2.Name = "gvDocLineSPP2";
		this.gvDocLineSPP2.OptionsView.ColumnAutoWidth = false;
		this.gvDocLineSPP2.OptionsView.ShowFooter = true;
		this.gvDocLineSPP2.OptionsView.ShowGroupPanel = false;
		this.gvDocLineSPP2.UseBatchMod = false;
		this.grdDocLineSPP2.AllowPasteExcel = true;
		this.grdDocLineSPP2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDocLineSPP2.ClipboardData = null;
		this.grdDocLineSPP2.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDocLineSPP2.Location = new System.Drawing.Point(2, 0);
		this.grdDocLineSPP2.MainView = this.gvDocLineSPP2;
		this.grdDocLineSPP2.Name = "grdDocLineSPP2";
		this.grdDocLineSPP2.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemCalcEdit1_Amount, this.repositoryItemCalcEdit2_Discount });
		this.grdDocLineSPP2.Size = new System.Drawing.Size(512, 278);
		this.grdDocLineSPP2.TabIndex = 42;
		this.grdDocLineSPP2.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDocLineSPP2 });
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(515, 307);
		base.Controls.Add(this.btOK);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.grdDocLineSPP2);
		base.FormId = "oVolumnDiscountForPriceList";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_VolumDiscountForPriceList";
		this.Text = "Volum Discounts for Price List";
		base.Load += new System.EventHandler(this.frm_VolumDiscountForPriceList_Load);
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2_Discount).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1_Amount).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDocLineSPP2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdDocLineSPP2).EndInit();
		base.ResumeLayout(false);
	}
}
