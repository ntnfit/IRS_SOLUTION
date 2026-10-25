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
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator.SetUp.Inventory.Bin_Location;

public class frm_BinLocList : XtraFormEx
{
	private const string FormController = "BinLocList";

	public string SlCode = "";

	public string FldAbs = "";

	public string AttrVal = "";

	private IContainer components;

	private GridControlEx grdBinLocList;

	private GridViewEx gvBinLocList;

	private RepositoryItemCheckEdit ce_Select;

	private GridColumnEx colBinLoc;

	private GridColumnEx colQuantity;

	private GridColumnEx colNoOfItems;

	private GridColumnEx colCCDNo;

	private RepositoryItemButtonEdit be_BinLoc;

	private SimpleButtonEx btnOk;

	private GridColumnEx colSelect;

	private RepositoryItemButtonEdit be_ItemQty;

	public frm_BinLocList()
	{
		InitializeComponent();
	}

	private async Task LoadData()
	{
		_ = 1;
		try
		{
			DataTable dataSource;
			if (!string.IsNullOrEmpty(SlCode))
			{
				using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BinLocList");
				dataSource = await hp.GetAsync("GetBinLocListBySLCode", new
				{
					fldAbs = FldAbs,
					slCode = SlCode
				});
			}
			else
			{
				using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BinLocList");
				dataSource = await hp.GetAsync("GetBinLocListByAttrVal", new
				{
					fldAbs = FldAbs,
					attrVal = AttrVal
				});
			}
			grdBinLocList.DataSource = dataSource;
			grdBinLocList.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private async void frm_BinLocList_Load(object sender, EventArgs e)
	{
		await LoadData();
	}

	private void gvBinLocList_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void be_BinLoc_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_BinLocMasterData frm_BinLocMasterData2 = new frm_BinLocMasterData();
			int rowHandle = gvBinLocList.GetSelectedRows()[0];
			DataRow dataRow = gvBinLocList.GetDataRow(rowHandle);
			frm_BinLocMasterData2.AbsEntry = Function.ParseInt(dataRow["AbsEntry"]);
			frm_BinLocMasterData2.MdiParent = MdiParent;
			frm_BinLocMasterData2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private void be_ItemQty_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
	}

	private void btnOk_Click(object sender, EventArgs e)
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		this.grdBinLocList = new Apzon.Controls.Grid.GridControlEx();
		this.gvBinLocList = new Apzon.Controls.Grid.GridViewEx();
		this.colSelect = new Apzon.Controls.Grid.GridColumnEx();
		this.ce_Select = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.colBinLoc = new Apzon.Controls.Grid.GridColumnEx();
		this.be_BinLoc = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.colQuantity = new Apzon.Controls.Grid.GridColumnEx();
		this.be_ItemQty = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.colNoOfItems = new Apzon.Controls.Grid.GridColumnEx();
		this.colCCDNo = new Apzon.Controls.Grid.GridColumnEx();
		this.btnOk = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grdBinLocList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvBinLocList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ce_Select).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.be_BinLoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.be_ItemQty).BeginInit();
		base.SuspendLayout();
		this.grdBinLocList.AllowPasteExcel = true;
		this.grdBinLocList.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdBinLocList.ClipboardData = null;
		this.grdBinLocList.Location = new System.Drawing.Point(0, 0);
		this.grdBinLocList.MainView = this.gvBinLocList;
		this.grdBinLocList.Name = "grdBinLocList";
		this.grdBinLocList.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.ce_Select, this.be_BinLoc, this.be_ItemQty });
		this.grdBinLocList.Size = new System.Drawing.Size(785, 254);
		this.grdBinLocList.TabIndex = 0;
		this.grdBinLocList.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvBinLocList });
		this.gvBinLocList.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvBinLocList.ColumnKey = null;
		this.gvBinLocList.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.colSelect, this.colBinLoc, this.colQuantity, this.colNoOfItems, this.colCCDNo });
		this.gvBinLocList.DisableFormSetting = false;
		this.gvBinLocList.DisplayException = false;
		this.gvBinLocList.EditColumn = null;
		this.gvBinLocList.FieldCurrency = null;
		this.gvBinLocList.FieldRate = null;
		this.gvBinLocList.FormId = null;
		this.gvBinLocList.GridControl = this.grdBinLocList;
		this.gvBinLocList.IgnoreFilter = false;
		this.gvBinLocList.IndexEditRow = -1;
		this.gvBinLocList.IndicatorWidth = 40;
		this.gvBinLocList.IsApzTable = false;
		this.gvBinLocList.IsInsertNewRow = false;
		this.gvBinLocList.IsLoadingData = false;
		this.gvBinLocList.IsMappingContextAction = false;
		this.gvBinLocList.ItemId = null;
		this.gvBinLocList.LstColumnAllowBatch = null;
		this.gvBinLocList.LstColumnNotAllowBatch = null;
		this.gvBinLocList.LstColumnNotEditable = null;
		this.gvBinLocList.LstColumNotSetting = null;
		this.gvBinLocList.Name = "gvBinLocList";
		this.gvBinLocList.OptionsSelection.MultiSelect = true;
		this.gvBinLocList.OptionsView.ShowFooter = true;
		this.gvBinLocList.OptionsView.ShowGroupPanel = false;
		this.gvBinLocList.UseBatchMod = false;
		this.gvBinLocList.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvBinLocList_CustomDrawRowIndicator);
		this.colSelect.Caption = "Select";
		this.colSelect.ColumnEdit = this.ce_Select;
		this.colSelect.CurrencyDateField = null;
		this.colSelect.CurrencyField = null;
		this.colSelect.CurrencyFixed = null;
		this.colSelect.CurrencyFixedRate = 0.0;
		this.colSelect.CurrencyRateField = null;
		this.colSelect.FieldName = "Select";
		this.colSelect.Name = "colSelect";
		this.colSelect.PrimaryKey = true;
		this.colSelect.UnitField = null;
		this.colSelect.Visible = true;
		this.colSelect.VisibleIndex = 0;
		this.ce_Select.AutoHeight = false;
		this.ce_Select.Name = "ce_Select";
		this.ce_Select.ValueChecked = "Y";
		this.ce_Select.ValueUnchecked = "N";
		this.colBinLoc.AppearanceCell.BackColor = System.Drawing.Color.LightGray;
		this.colBinLoc.AppearanceCell.BackColor2 = System.Drawing.Color.LightGray;
		this.colBinLoc.AppearanceCell.Options.UseBackColor = true;
		this.colBinLoc.AppearanceCell.Options.UseTextOptions = true;
		this.colBinLoc.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.colBinLoc.Caption = "Bin Location";
		this.colBinLoc.ColumnEdit = this.be_BinLoc;
		this.colBinLoc.CurrencyDateField = null;
		this.colBinLoc.CurrencyField = null;
		this.colBinLoc.CurrencyFixed = null;
		this.colBinLoc.CurrencyFixedRate = 0.0;
		this.colBinLoc.CurrencyRateField = null;
		this.colBinLoc.FieldName = "BinCode";
		this.colBinLoc.Name = "colBinLoc";
		this.colBinLoc.OptionsColumn.ReadOnly = true;
		this.colBinLoc.PrimaryKey = false;
		this.colBinLoc.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.colBinLoc.UnitField = null;
		this.colBinLoc.Visible = true;
		this.colBinLoc.VisibleIndex = 1;
		this.colBinLoc.Width = 168;
		this.be_BinLoc.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.be_BinLoc.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.be_BinLoc.Name = "be_BinLoc";
		this.be_BinLoc.ReadOnly = true;
		this.be_BinLoc.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.be_BinLoc.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.be_BinLoc_ButtonClick);
		this.colQuantity.AppearanceCell.BackColor = System.Drawing.Color.LightGray;
		this.colQuantity.AppearanceCell.BackColor2 = System.Drawing.Color.LightGray;
		this.colQuantity.AppearanceCell.Options.UseBackColor = true;
		this.colQuantity.Caption = "Item Qty";
		this.colQuantity.ColumnEdit = this.be_ItemQty;
		this.colQuantity.CurrencyDateField = null;
		this.colQuantity.CurrencyField = null;
		this.colQuantity.CurrencyFixed = null;
		this.colQuantity.CurrencyFixedRate = 0.0;
		this.colQuantity.CurrencyRateField = null;
		this.colQuantity.FieldName = "ItemQty";
		this.colQuantity.Name = "colQuantity";
		this.colQuantity.OptionsColumn.ReadOnly = true;
		this.colQuantity.PrimaryKey = false;
		this.colQuantity.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.colQuantity.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "ItemQty", "{0}")
		});
		this.colQuantity.UnitField = null;
		this.colQuantity.Visible = true;
		this.colQuantity.VisibleIndex = 2;
		this.colQuantity.Width = 168;
		this.be_ItemQty.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.be_ItemQty.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.be_ItemQty.Name = "be_ItemQty";
		this.be_ItemQty.ReadOnly = true;
		this.be_ItemQty.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.be_ItemQty.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.be_ItemQty_ButtonClick);
		this.colNoOfItems.AppearanceCell.BackColor = System.Drawing.Color.LightGray;
		this.colNoOfItems.AppearanceCell.BackColor2 = System.Drawing.Color.LightGray;
		this.colNoOfItems.AppearanceCell.Options.UseBackColor = true;
		this.colNoOfItems.Caption = "No. Of Items";
		this.colNoOfItems.CurrencyDateField = null;
		this.colNoOfItems.CurrencyField = null;
		this.colNoOfItems.CurrencyFixed = null;
		this.colNoOfItems.CurrencyFixedRate = 0.0;
		this.colNoOfItems.CurrencyRateField = null;
		this.colNoOfItems.FieldName = "NoOfItems";
		this.colNoOfItems.Name = "colNoOfItems";
		this.colNoOfItems.OptionsColumn.ReadOnly = true;
		this.colNoOfItems.PrimaryKey = false;
		this.colNoOfItems.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "NoOfItems", "{0}")
		});
		this.colNoOfItems.UnitField = null;
		this.colNoOfItems.Visible = true;
		this.colNoOfItems.VisibleIndex = 3;
		this.colNoOfItems.Width = 168;
		this.colCCDNo.AppearanceCell.BackColor = System.Drawing.Color.LightGray;
		this.colCCDNo.AppearanceCell.BackColor2 = System.Drawing.Color.LightGray;
		this.colCCDNo.AppearanceCell.Options.UseBackColor = true;
		this.colCCDNo.Caption = "CCD No.";
		this.colCCDNo.CurrencyDateField = null;
		this.colCCDNo.CurrencyField = null;
		this.colCCDNo.CurrencyFixed = null;
		this.colCCDNo.CurrencyFixedRate = 0.0;
		this.colCCDNo.CurrencyRateField = null;
		this.colCCDNo.FieldName = "CCDNo";
		this.colCCDNo.Name = "colCCDNo";
		this.colCCDNo.OptionsColumn.AllowFocus = false;
		this.colCCDNo.OptionsColumn.ReadOnly = true;
		this.colCCDNo.PrimaryKey = false;
		this.colCCDNo.UnitField = null;
		this.colCCDNo.Visible = true;
		this.colCCDNo.VisibleIndex = 4;
		this.colCCDNo.Width = 178;
		this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOk.EditControl = null;
		this.btnOk.EditValue = "OK";
		this.btnOk.FieldName = null;
		this.btnOk.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btnOk.IsSearchControl = false;
		this.btnOk.Location = new System.Drawing.Point(12, 260);
		this.btnOk.Name = "btnOk";
		this.btnOk.ReadOnly = false;
		this.btnOk.SettingInVisible = false;
		this.btnOk.SettingReadonly = false;
		this.btnOk.Size = new System.Drawing.Size(75, 23);
		this.btnOk.TabIndex = 10;
		this.btnOk.Text = "OK";
		this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(785, 295);
		base.Controls.Add(this.btnOk);
		base.Controls.Add(this.grdBinLocList);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_BinLocList";
		this.Text = "Bin Location List";
		base.Load += new System.EventHandler(this.frm_BinLocList_Load);
		((System.ComponentModel.ISupportInitialize)this.grdBinLocList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvBinLocList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ce_Select).EndInit();
		((System.ComponentModel.ISupportInitialize)this.be_BinLoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.be_ItemQty).EndInit();
		base.ResumeLayout(false);
	}
}
