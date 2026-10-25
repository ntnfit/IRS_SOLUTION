using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Global;
using DevExpress.Data;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.TableLayout;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Tile;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucRetailGrid : XtraUserControl
{
	public bool RefundMode;

	private IContainer components;

	public GridControlEx grd_Contents;

	public RetailGrid gv_Contents;

	public RepositoryItemButtonEdit rep_Qty;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn8;

	private GridColumnEx gridColumn9;

	private GridColumnEx gridColumn7;

	private RepositoryItemCalcEdit rep_Price;

	private RepositoryItemMemoEdit repositoryItemMemoEdit1;

	private GridColumnEx gc_Edit;

	public RepositoryItemButtonEdit rep_Edit;

	public TileView tileView1;

	private TileViewColumn tileViewColumn1;

	private TileViewColumn tileCol_ItemCode;

	private TileViewColumn tileCol_ItemName;

	private TileViewColumn tileCol_Qty;

	private TileViewColumn tileCol_Price;

	private TileViewColumn tileViewColumn6;

	private TileViewColumn tileViewColumn7;

	private TileViewColumn tileViewColumn8;

	private TileViewColumn tileCol_Total;

	private TileViewColumn tileViewColumn10;

	private TileViewColumn tileCol_Check;

	private TileViewColumn tileViewColumn2;

	public GridControlEx gridControlEx1;

	public TileView tileView2;

	private TileViewColumn tileViewColumn3;

	private TileViewColumn tileViewColumn4;

	private TileViewColumn tileViewColumn5;

	private RepositoryItemMemoEdit repositoryItemMemoEdit2;

	private TileViewColumn tileViewColumn9;

	public RepositoryItemButtonEdit repositoryItemButtonEdit1;

	private TileViewColumn tileViewColumn11;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private TileViewColumn tileViewColumn12;

	private TileViewColumn tileViewColumn13;

	private TileViewColumn tileViewColumn14;

	private TileViewColumn tileViewColumn15;

	private TileViewColumn tileViewColumn16;

	public RepositoryItemButtonEdit repositoryItemButtonEdit2;

	private TileViewColumn tileViewColumn17;

	private TileViewColumn tileViewColumn18;

	public RetailGrid retailGrid1;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx10;

	private TileViewColumn tileViewColumn19;

	private RepositoryItemCalcEditEx rep_Rate;

	private TileViewColumn tileViewColumn20;

	public ucRetailGrid()
	{
		InitializeComponent();
	}

	private void gv_Contents_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			if (sender is ColumnView columnView)
			{
				_ = ((GridColumnEx)columnView.FocusedColumn).FieldName != "Quantity";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ucRetailGrid_Load(object sender, EventArgs e)
	{
		try
		{
			if (RefundMode)
			{
				gv_Contents.OptionsSelection.MultiSelect = true;
				gv_Contents.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
				tileView1.ContextButtons[0].Visibility = ContextItemVisibility.Hidden;
			}
			string currency = string.Empty;
			currency = GlobalData.MainCurrency;
			int num = -1;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currency.ToUpper()));
				if (currencyCodesImpl != null)
				{
					num = currencyCodesImpl.Decimals;
				}
			}
			string text = new string('#', (num == -1) ? GlobalData.GeneralSetting.SumDec.GetValueOrDefault() : num);
			rep_Price.DisplayFormat.FormatType = FormatType.Numeric;
			rep_Price.DisplayFormat.FormatString = "#,0." + text;
			rep_Qty.DisplayFormat.FormatType = FormatType.Numeric;
			rep_Qty.DisplayFormat.FormatString = "n2";
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Dscription");
			dataTable.Columns.Add("Price");
			dataTable.Columns.Add("Quantity");
			dataTable.Columns.Add("VatPrcnt");
			dataTable.Columns.Add("LineTotal");
			dataTable.Rows.Add("Tên hàng", "Đơn giá", "SL", "VAT(%)", "Thành tiền");
			gridControlEx1.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.UserControls.POS_UI.ucRetailGrid));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.ContextButton contextButton = new DevExpress.Utils.ContextButton();
		DevExpress.XtraGrid.Views.Tile.ItemTemplate itemTemplate = new DevExpress.XtraGrid.Views.Tile.ItemTemplate();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition2 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition3 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition4 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition5 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement2 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement3 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement4 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement5 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement6 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition obj = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition obj2 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition6 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition7 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition8 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition9 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition10 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition2 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan2 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement7 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement8 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement9 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement10 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement11 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement12 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement13 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement14 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraGrid.Views.Tile.ItemTemplate itemTemplate2 = new DevExpress.XtraGrid.Views.Tile.ItemTemplate();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition11 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition12 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition13 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition14 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition15 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement15 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement16 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement17 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement18 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement19 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement20 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition obj3 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition obj4 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan3 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition16 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition17 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition18 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition19 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition20 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition3 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition4 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan4 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement21 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement22 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement23 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement24 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement25 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement26 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement27 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		this.tileCol_ItemCode = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tileCol_ItemName = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.repositoryItemMemoEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
		this.tileCol_Qty = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.rep_Qty = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.tileCol_Price = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.rep_Price = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.tileCol_Total = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tileViewColumn2 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tileViewColumn7 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.rep_Rate = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.tileViewColumn4 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tileViewColumn5 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.repositoryItemMemoEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
		this.tileViewColumn9 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.repositoryItemButtonEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.tileViewColumn11 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.tileViewColumn15 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tileViewColumn13 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tileViewColumn18 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.grd_Contents = new Apzon.Controls.Grid.GridControlEx();
		this.tileView1 = new DevExpress.XtraGrid.Views.Tile.TileView();
		this.tileViewColumn1 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tileViewColumn6 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tileViewColumn8 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tileViewColumn10 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.rep_Edit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.tileCol_Check = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tileViewColumn19 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.gv_Contents = new Apzon.Forms.UserControls.POS_UI.RetailGrid();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Edit = new Apzon.Controls.Grid.GridColumnEx();
		this.gridControlEx1 = new Apzon.Controls.Grid.GridControlEx();
		this.tileView2 = new DevExpress.XtraGrid.Views.Tile.TileView();
		this.tileViewColumn3 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tileViewColumn12 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tileViewColumn14 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.tileViewColumn16 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.repositoryItemButtonEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.tileViewColumn17 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.retailGrid1 = new Apzon.Forms.UserControls.POS_UI.RetailGrid();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.tileViewColumn20 = new DevExpress.XtraGrid.Columns.TileViewColumn();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Qty).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Price).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Rate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Contents).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tileView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Edit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Contents).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridControlEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tileView2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.retailGrid1).BeginInit();
		base.SuspendLayout();
		this.tileCol_ItemCode.Caption = "Product Code";
		this.tileCol_ItemCode.FieldName = "ItemCode";
		this.tileCol_ItemCode.Name = "tileCol_ItemCode";
		this.tileCol_ItemCode.OptionsColumn.ReadOnly = true;
		this.tileCol_ItemCode.Visible = true;
		this.tileCol_ItemCode.VisibleIndex = 1;
		this.tileCol_ItemCode.Width = 116;
		this.tileCol_ItemName.Caption = "Description";
		this.tileCol_ItemName.ColumnEdit = this.repositoryItemMemoEdit1;
		this.tileCol_ItemName.FieldName = "Dscription";
		this.tileCol_ItemName.Name = "tileCol_ItemName";
		this.tileCol_ItemName.OptionsColumn.ReadOnly = true;
		this.tileCol_ItemName.Visible = true;
		this.tileCol_ItemName.VisibleIndex = 2;
		this.tileCol_ItemName.Width = 131;
		this.repositoryItemMemoEdit1.Name = "repositoryItemMemoEdit1";
		this.tileCol_Qty.AppearanceCell.Options.UseTextOptions = true;
		this.tileCol_Qty.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.tileCol_Qty.Caption = "Qty";
		this.tileCol_Qty.ColumnEdit = this.rep_Qty;
		this.tileCol_Qty.FieldName = "Quantity";
		this.tileCol_Qty.Name = "tileCol_Qty";
		this.tileCol_Qty.Visible = true;
		this.tileCol_Qty.VisibleIndex = 3;
		this.tileCol_Qty.Width = 141;
		this.rep_Qty.AutoHeight = false;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.rep_Qty.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", 25, true, true, false, editorButtonImageOptions),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", 25, true, true, true, editorButtonImageOptions2)
		});
		this.rep_Qty.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.rep_Qty.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.rep_Qty.LookAndFeel.UseDefaultLookAndFeel = false;
		this.rep_Qty.Name = "rep_Qty";
		this.tileCol_Price.Caption = "Price";
		this.tileCol_Price.ColumnEdit = this.rep_Price;
		this.tileCol_Price.FieldName = "PriceBefDi";
		this.tileCol_Price.Name = "tileCol_Price";
		this.tileCol_Price.Visible = true;
		this.tileCol_Price.VisibleIndex = 4;
		this.tileCol_Price.Width = 146;
		this.rep_Price.AutoHeight = false;
		this.rep_Price.Name = "rep_Price";
		this.rep_Price.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.tileCol_Total.Caption = "Total";
		this.tileCol_Total.ColumnEdit = this.rep_Price;
		this.tileCol_Total.FieldName = "LineTotal";
		this.tileCol_Total.Name = "tileCol_Total";
		this.tileCol_Total.Width = 164;
		this.tileViewColumn2.Caption = "Total";
		this.tileViewColumn2.ColumnEdit = this.rep_Price;
		this.tileViewColumn2.FieldName = "Total";
		this.tileViewColumn2.Name = "tileViewColumn2";
		this.tileViewColumn2.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
		this.tileViewColumn2.Visible = true;
		this.tileViewColumn2.VisibleIndex = 10;
		this.tileViewColumn7.Caption = "Tax (%)";
		this.tileViewColumn7.ColumnEdit = this.rep_Rate;
		this.tileViewColumn7.FieldName = "VatPrcnt";
		this.tileViewColumn7.Name = "tileViewColumn7";
		this.tileViewColumn7.Visible = true;
		this.tileViewColumn7.VisibleIndex = 6;
		this.tileViewColumn7.Width = 78;
		this.rep_Rate.AutoHeight = false;
		this.rep_Rate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Rate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Rate;
		this.rep_Rate.Name = "rep_Rate";
		this.tileViewColumn4.Caption = "Product Code";
		this.tileViewColumn4.FieldName = "ItemCode";
		this.tileViewColumn4.Name = "tileViewColumn4";
		this.tileViewColumn4.OptionsColumn.ReadOnly = true;
		this.tileViewColumn4.Visible = true;
		this.tileViewColumn4.VisibleIndex = 1;
		this.tileViewColumn4.Width = 116;
		this.tileViewColumn5.Caption = "Description";
		this.tileViewColumn5.ColumnEdit = this.repositoryItemMemoEdit2;
		this.tileViewColumn5.FieldName = "Dscription";
		this.tileViewColumn5.Name = "tileViewColumn5";
		this.tileViewColumn5.OptionsColumn.ReadOnly = true;
		this.tileViewColumn5.Visible = true;
		this.tileViewColumn5.VisibleIndex = 2;
		this.tileViewColumn5.Width = 131;
		this.repositoryItemMemoEdit2.Name = "repositoryItemMemoEdit2";
		this.tileViewColumn9.AppearanceCell.Options.UseTextOptions = true;
		this.tileViewColumn9.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.tileViewColumn9.Caption = "Qty";
		this.tileViewColumn9.ColumnEdit = this.repositoryItemButtonEdit1;
		this.tileViewColumn9.FieldName = "Quantity";
		this.tileViewColumn9.Name = "tileViewColumn9";
		this.tileViewColumn9.Visible = true;
		this.tileViewColumn9.VisibleIndex = 3;
		this.tileViewColumn9.Width = 141;
		this.repositoryItemButtonEdit1.AutoHeight = false;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions3.Image");
		editorButtonImageOptions4.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.repositoryItemButtonEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", 25, true, true, false, editorButtonImageOptions3),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", 25, true, true, true, editorButtonImageOptions4)
		});
		this.repositoryItemButtonEdit1.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.repositoryItemButtonEdit1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.repositoryItemButtonEdit1.LookAndFeel.UseDefaultLookAndFeel = false;
		this.repositoryItemButtonEdit1.Name = "repositoryItemButtonEdit1";
		this.tileViewColumn11.Caption = "Price";
		this.tileViewColumn11.ColumnEdit = this.repositoryItemCalcEdit1;
		this.tileViewColumn11.FieldName = "Price";
		this.tileViewColumn11.Name = "tileViewColumn11";
		this.tileViewColumn11.Visible = true;
		this.tileViewColumn11.VisibleIndex = 4;
		this.tileViewColumn11.Width = 146;
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.repositoryItemCalcEdit1.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.tileViewColumn15.Caption = "Total";
		this.tileViewColumn15.ColumnEdit = this.repositoryItemCalcEdit1;
		this.tileViewColumn15.FieldName = "LineTotal";
		this.tileViewColumn15.Name = "tileViewColumn15";
		this.tileViewColumn15.Visible = true;
		this.tileViewColumn15.VisibleIndex = 8;
		this.tileViewColumn15.Width = 164;
		this.tileViewColumn13.Caption = "Tax (%)";
		this.tileViewColumn13.ColumnEdit = this.repositoryItemCalcEdit1;
		this.tileViewColumn13.FieldName = "VatPrcnt";
		this.tileViewColumn13.Name = "tileViewColumn13";
		this.tileViewColumn13.Visible = true;
		this.tileViewColumn13.VisibleIndex = 6;
		this.tileViewColumn13.Width = 78;
		this.tileViewColumn18.Caption = "Total";
		this.tileViewColumn18.ColumnEdit = this.repositoryItemCalcEdit1;
		this.tileViewColumn18.FieldName = "Total";
		this.tileViewColumn18.Name = "tileViewColumn18";
		this.tileViewColumn18.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
		this.tileViewColumn18.Visible = true;
		this.tileViewColumn18.VisibleIndex = 11;
		this.grd_Contents.AllowPasteExcel = false;
		this.grd_Contents.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Contents.ClipboardData = null;
		this.grd_Contents.EmbeddedNavigator.Appearance.BackColor = System.Drawing.Color.White;
		this.grd_Contents.EmbeddedNavigator.Appearance.Options.UseBackColor = true;
		this.grd_Contents.Location = new System.Drawing.Point(0, 49);
		this.grd_Contents.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.grd_Contents.LookAndFeel.UseDefaultLookAndFeel = false;
		this.grd_Contents.MainView = this.tileView1;
		this.grd_Contents.Name = "grd_Contents";
		this.grd_Contents.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rep_Qty, this.rep_Price, this.repositoryItemMemoEdit1, this.rep_Edit, this.rep_Rate });
		this.grd_Contents.Size = new System.Drawing.Size(753, 424);
		this.grd_Contents.TabIndex = 9;
		this.grd_Contents.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.tileView1, this.gv_Contents });
		this.tileView1.Appearance.EmptySpace.BackColor = System.Drawing.Color.FromArgb(219, 238, 244);
		this.tileView1.Appearance.EmptySpace.Options.UseBackColor = true;
		this.tileView1.Appearance.ItemFocused.BackColor = System.Drawing.Color.FromArgb(192, 255, 255);
		this.tileView1.Appearance.ItemFocused.Options.UseBackColor = true;
		this.tileView1.Appearance.ItemNormal.BackColor = System.Drawing.Color.FromArgb(219, 238, 244);
		this.tileView1.Appearance.ItemNormal.Options.UseBackColor = true;
		this.tileView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[14]
		{
			this.tileViewColumn1, this.tileCol_ItemCode, this.tileCol_ItemName, this.tileCol_Qty, this.tileCol_Price, this.tileViewColumn6, this.tileViewColumn7, this.tileViewColumn8, this.tileCol_Total, this.tileViewColumn10,
			this.tileCol_Check, this.tileViewColumn2, this.tileViewColumn19, this.tileViewColumn20
		});
		this.tileView1.ColumnSet.CheckedColumn = this.tileCol_Check;
		contextButton.Alignment = DevExpress.Utils.ContextItemAlignment.FarCenter;
		contextButton.Id = new System.Guid("10f44b9a-d45f-4e7b-a080-bcc4fe5de1a3");
		contextButton.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("resource.Image");
		contextButton.Name = "ctxButton_Remove";
		contextButton.Padding = new System.Windows.Forms.Padding(0, 0, 15, 0);
		contextButton.Visibility = DevExpress.Utils.ContextItemVisibility.Visible;
		this.tileView1.ContextButtons.Add(contextButton);
		this.tileView1.GridControl = this.grd_Contents;
		this.tileView1.Name = "tileView1";
		this.tileView1.OptionsTiles.IndentBetweenGroups = 0;
		this.tileView1.OptionsTiles.IndentBetweenItems = 0;
		this.tileView1.OptionsTiles.ItemSize = new System.Drawing.Size(248, 60);
		this.tileView1.OptionsTiles.LayoutMode = DevExpress.XtraGrid.Views.Tile.TileViewLayoutMode.List;
		this.tileView1.OptionsTiles.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.tileView1.OptionsTiles.Padding = new System.Windows.Forms.Padding(0);
		tableColumnDefinition.Length.Value = 27.0;
		tableColumnDefinition2.Length.Value = 54.0;
		tableColumnDefinition3.Length.Value = 39.0;
		tableColumnDefinition4.Length.Value = 71.0;
		tableColumnDefinition5.Length.Value = 33.0;
		itemTemplate.Columns.Add(tableColumnDefinition);
		itemTemplate.Columns.Add(tableColumnDefinition2);
		itemTemplate.Columns.Add(tableColumnDefinition3);
		itemTemplate.Columns.Add(tableColumnDefinition4);
		itemTemplate.Columns.Add(tableColumnDefinition5);
		tileViewItemElement.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement.Column = this.tileCol_ItemCode;
		tileViewItemElement.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement.ImageVisible = false;
		tileViewItemElement.RowIndex = 1;
		tileViewItemElement.Text = "tileCol_ItemCode";
		tileViewItemElement.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement.TextVisible = false;
		tileViewItemElement2.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement2.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement2.Column = this.tileCol_ItemName;
		tileViewItemElement2.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement2.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement2.Text = "tileCol_ItemName";
		tileViewItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement3.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement3.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement3.Column = this.tileCol_Qty;
		tileViewItemElement3.ColumnIndex = 2;
		tileViewItemElement3.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement3.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement3.RowIndex = 1;
		tileViewItemElement3.Text = "tileCol_Qty";
		tileViewItemElement3.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement4.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement4.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement4.Column = this.tileCol_Price;
		tileViewItemElement4.ColumnIndex = 1;
		tileViewItemElement4.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement4.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement4.RowIndex = 1;
		tileViewItemElement4.Text = "tileCol_Price";
		tileViewItemElement4.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement5.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement5.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement5.Column = this.tileCol_Total;
		tileViewItemElement5.ColumnIndex = 3;
		tileViewItemElement5.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement5.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement5.RowIndex = 1;
		tileViewItemElement5.Text = "tileCol_Total";
		tileViewItemElement5.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement6.ColumnIndex = 3;
		tileViewItemElement6.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement6.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement6.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		itemTemplate.Elements.Add(tileViewItemElement);
		itemTemplate.Elements.Add(tileViewItemElement2);
		itemTemplate.Elements.Add(tileViewItemElement3);
		itemTemplate.Elements.Add(tileViewItemElement4);
		itemTemplate.Elements.Add(tileViewItemElement5);
		itemTemplate.Elements.Add(tileViewItemElement6);
		itemTemplate.Name = "template0";
		itemTemplate.Rows.Add(obj);
		itemTemplate.Rows.Add(obj2);
		tableSpan.ColumnSpan = 3;
		itemTemplate.Spans.Add(tableSpan);
		this.tileView1.Templates.Add(itemTemplate);
		tableColumnDefinition6.Length.Value = 20.0;
		tableColumnDefinition7.Length.Value = 53.0;
		tableColumnDefinition8.Length.Value = 46.0;
		tableColumnDefinition9.Length.Value = 39.0;
		tableColumnDefinition10.Length.Value = 66.0;
		this.tileView1.TileColumns.Add(tableColumnDefinition6);
		this.tileView1.TileColumns.Add(tableColumnDefinition7);
		this.tileView1.TileColumns.Add(tableColumnDefinition8);
		this.tileView1.TileColumns.Add(tableColumnDefinition9);
		this.tileView1.TileColumns.Add(tableColumnDefinition10);
		tableRowDefinition.Length.Value = 25.0;
		tableRowDefinition2.Length.Value = 21.0;
		this.tileView1.TileRows.Add(tableRowDefinition);
		this.tileView1.TileRows.Add(tableRowDefinition2);
		tableSpan2.ColumnSpan = 4;
		this.tileView1.TileSpans.Add(tableSpan2);
		tileViewItemElement7.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement7.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement7.Column = this.tileCol_ItemCode;
		tileViewItemElement7.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement7.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement7.ImageVisible = false;
		tileViewItemElement7.RowIndex = 1;
		tileViewItemElement7.Text = "tileCol_ItemCode";
		tileViewItemElement7.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement7.TextVisible = false;
		tileViewItemElement8.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement8.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement8.Column = this.tileCol_ItemName;
		tileViewItemElement8.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement8.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement8.Text = "tileCol_ItemName";
		tileViewItemElement8.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement9.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement9.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement9.Column = this.tileCol_Qty;
		tileViewItemElement9.ColumnIndex = 2;
		tileViewItemElement9.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement9.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement9.RowIndex = 1;
		tileViewItemElement9.Text = "tileCol_Qty";
		tileViewItemElement9.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement10.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement10.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement10.Column = this.tileCol_Price;
		tileViewItemElement10.ColumnIndex = 1;
		tileViewItemElement10.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement10.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement10.RowIndex = 1;
		tileViewItemElement10.Text = "tileCol_Price";
		tileViewItemElement10.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement11.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement11.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement11.Column = this.tileCol_Total;
		tileViewItemElement11.ColumnIndex = 4;
		tileViewItemElement11.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement11.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement11.Text = "tileCol_Total";
		tileViewItemElement11.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement12.ColumnIndex = 4;
		tileViewItemElement12.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement12.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement12.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement13.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement13.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement13.Column = this.tileViewColumn2;
		tileViewItemElement13.ColumnIndex = 4;
		tileViewItemElement13.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement13.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement13.RowIndex = 1;
		tileViewItemElement13.Text = "tileViewColumn2";
		tileViewItemElement13.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement14.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement14.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement14.Column = this.tileViewColumn7;
		tileViewItemElement14.ColumnIndex = 3;
		tileViewItemElement14.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement14.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement14.RowIndex = 1;
		tileViewItemElement14.Text = "tileViewColumn7";
		tileViewItemElement14.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		this.tileView1.TileTemplate.Add(tileViewItemElement7);
		this.tileView1.TileTemplate.Add(tileViewItemElement8);
		this.tileView1.TileTemplate.Add(tileViewItemElement9);
		this.tileView1.TileTemplate.Add(tileViewItemElement10);
		this.tileView1.TileTemplate.Add(tileViewItemElement11);
		this.tileView1.TileTemplate.Add(tileViewItemElement12);
		this.tileView1.TileTemplate.Add(tileViewItemElement13);
		this.tileView1.TileTemplate.Add(tileViewItemElement14);
		this.tileViewColumn1.Caption = "Type";
		this.tileViewColumn1.FieldName = "TranType";
		this.tileViewColumn1.Name = "tileViewColumn1";
		this.tileViewColumn1.OptionsColumn.AllowEdit = false;
		this.tileViewColumn1.OptionsColumn.ReadOnly = true;
		this.tileViewColumn1.Visible = true;
		this.tileViewColumn1.VisibleIndex = 0;
		this.tileViewColumn1.Width = 68;
		this.tileViewColumn6.Caption = "Discount (%)";
		this.tileViewColumn6.ColumnEdit = this.rep_Price;
		this.tileViewColumn6.FieldName = "DiscPrcnt";
		this.tileViewColumn6.Name = "tileViewColumn6";
		this.tileViewColumn6.Visible = true;
		this.tileViewColumn6.VisibleIndex = 5;
		this.tileViewColumn6.Width = 106;
		this.tileViewColumn8.Caption = "Tax Amount";
		this.tileViewColumn8.FieldName = "LineVat";
		this.tileViewColumn8.Name = "tileViewColumn8";
		this.tileViewColumn8.Visible = true;
		this.tileViewColumn8.VisibleIndex = 7;
		this.tileViewColumn10.Caption = "Edit";
		this.tileViewColumn10.ColumnEdit = this.rep_Edit;
		this.tileViewColumn10.ImageAlignment = System.Drawing.StringAlignment.Center;
		this.tileViewColumn10.Name = "tileViewColumn10";
		this.tileViewColumn10.Visible = true;
		this.tileViewColumn10.VisibleIndex = 8;
		this.tileViewColumn10.Width = 139;
		this.rep_Edit.AutoHeight = false;
		editorButtonImageOptions5.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions5.Image");
		editorButtonImageOptions6.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions6.Image");
		this.rep_Edit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5),
			new DevExpress.XtraEditors.Controls.EditorButton(editorButtonImageOptions6, DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, null)
		});
		this.rep_Edit.Name = "rep_Edit";
		this.rep_Edit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
		this.tileCol_Check.Caption = "tileViewColumn11";
		this.tileCol_Check.FieldName = "Checked";
		this.tileCol_Check.Name = "tileCol_Check";
		this.tileCol_Check.UnboundType = DevExpress.Data.UnboundColumnType.Boolean;
		this.tileCol_Check.Visible = true;
		this.tileCol_Check.VisibleIndex = 9;
		this.tileViewColumn19.Caption = "tileViewColumn19";
		this.tileViewColumn19.FieldName = "VatGroup";
		this.tileViewColumn19.Name = "tileViewColumn19";
		this.tileViewColumn19.Visible = true;
		this.tileViewColumn19.VisibleIndex = 11;
		this.gv_Contents.Appearance.Empty.BackColor = System.Drawing.Color.FromArgb(219, 238, 244);
		this.gv_Contents.Appearance.Empty.Options.UseBackColor = true;
		this.gv_Contents.Appearance.HeaderPanel.BackColor = System.Drawing.Color.FromArgb(219, 238, 244);
		this.gv_Contents.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.gv_Contents.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
		this.gv_Contents.Appearance.HeaderPanel.Options.UseBackColor = true;
		this.gv_Contents.Appearance.HeaderPanel.Options.UseFont = true;
		this.gv_Contents.Appearance.HeaderPanel.Options.UseForeColor = true;
		this.gv_Contents.Appearance.Row.BackColor = System.Drawing.Color.FromArgb(219, 238, 244);
		this.gv_Contents.Appearance.Row.Options.UseBackColor = true;
		this.gv_Contents.Appearance.SelectedRow.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.gv_Contents.Appearance.SelectedRow.Options.UseBackColor = true;
		this.gv_Contents.AutoFillColumn = this.gridColumn7;
		this.gv_Contents.ColumnKey = null;
		this.gv_Contents.ColumnPanelRowHeight = 25;
		this.gv_Contents.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn5, this.gridColumn6, this.gridColumn4, this.gridColumn8, this.gridColumn9, this.gridColumn7, this.gc_Edit });
		this.gv_Contents.DisableFormSetting = false;
		this.gv_Contents.DisplayException = false;
		this.gv_Contents.EditColumn = null;
		this.gv_Contents.FieldCurrency = null;
		this.gv_Contents.FieldRate = null;
		this.gv_Contents.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.gv_Contents.FormId = "ucRetailGrid";
		this.gv_Contents.GridControl = this.grd_Contents;
		this.gv_Contents.IndexEditRow = -1;
		this.gv_Contents.IndicatorWidth = 50;
		this.gv_Contents.IsInsertNewRow = false;
		this.gv_Contents.IsLoadingData = false;
		this.gv_Contents.ItemId = "OCONTENT";
		this.gv_Contents.LstColumNotSetting = null;
		this.gv_Contents.Name = "gv_Contents";
		this.gv_Contents.OptionsClipboard.AllowCopy = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Contents.OptionsClipboard.CopyCollapsedData = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Contents.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Contents.OptionsNavigation.AutoFocusNewRow = true;
		this.gv_Contents.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Contents.OptionsView.RowAutoHeight = true;
		this.gv_Contents.OptionsView.ShowGroupPanel = false;
		this.gv_Contents.PreviewLineCount = 1;
		this.gv_Contents.RowHeight = 50;
		this.gv_Contents.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_Contents_ValidatingEditor);
		this.gridColumn7.Caption = "Total";
		this.gridColumn7.ColumnEdit = this.rep_Price;
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.DisplayFormat.FormatString = "n0";
		this.gridColumn7.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn7.FieldName = "LineTotal";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 4;
		this.gridColumn7.Width = 164;
		this.gridColumn1.Caption = "Type";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "TranType";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowEdit = false;
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.Width = 68;
		this.gridColumn2.Caption = "Product Code";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "ItemCode";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 0;
		this.gridColumn2.Width = 116;
		this.gridColumn3.Caption = "Description";
		this.gridColumn3.ColumnEdit = this.repositoryItemMemoEdit1;
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "Dscription";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsColumn.ReadOnly = true;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 1;
		this.gridColumn3.Width = 131;
		this.gridColumn5.AppearanceCell.Options.UseTextOptions = true;
		this.gridColumn5.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gridColumn5.Caption = "Qty";
		this.gridColumn5.ColumnEdit = this.rep_Qty;
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "Quantity";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 2;
		this.gridColumn5.Width = 141;
		this.gridColumn6.Caption = "Price";
		this.gridColumn6.ColumnEdit = this.rep_Price;
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.DisplayFormat.FormatString = "n0";
		this.gridColumn6.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn6.FieldName = "Price";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 3;
		this.gridColumn6.Width = 146;
		this.gridColumn4.Caption = "Discount (%)";
		this.gridColumn4.ColumnEdit = this.rep_Price;
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "DiscPrcnt";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.Width = 106;
		this.gridColumn8.Caption = "Tax (%)";
		this.gridColumn8.ColumnEdit = this.rep_Price;
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "VatPrcnt";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.Width = 78;
		this.gridColumn9.Caption = "Tax Amount";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "Vatsum";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.PrimaryKey = false;
		this.gc_Edit.Caption = "Edit";
		this.gc_Edit.ColumnEdit = this.rep_Edit;
		this.gc_Edit.CurrencyDateField = null;
		this.gc_Edit.CurrencyField = null;
		this.gc_Edit.CurrencyFixed = null;
		this.gc_Edit.CurrencyFixedRate = 0.0;
		this.gc_Edit.CurrencyRateField = null;
		this.gc_Edit.ImageAlignment = System.Drawing.StringAlignment.Center;
		this.gc_Edit.Name = "gc_Edit";
		this.gc_Edit.PrimaryKey = false;
		this.gc_Edit.Visible = true;
		this.gc_Edit.VisibleIndex = 5;
		this.gc_Edit.Width = 139;
		this.gridControlEx1.AllowPasteExcel = false;
		this.gridControlEx1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridControlEx1.ClipboardData = null;
		this.gridControlEx1.EmbeddedNavigator.Appearance.BackColor = System.Drawing.Color.White;
		this.gridControlEx1.EmbeddedNavigator.Appearance.Options.UseBackColor = true;
		this.gridControlEx1.Location = new System.Drawing.Point(0, 0);
		this.gridControlEx1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.gridControlEx1.LookAndFeel.UseDefaultLookAndFeel = false;
		this.gridControlEx1.MainView = this.tileView2;
		this.gridControlEx1.Name = "gridControlEx1";
		this.gridControlEx1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.repositoryItemButtonEdit1, this.repositoryItemCalcEdit1, this.repositoryItemMemoEdit2, this.repositoryItemButtonEdit2 });
		this.gridControlEx1.Size = new System.Drawing.Size(753, 51);
		this.gridControlEx1.TabIndex = 10;
		this.gridControlEx1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.tileView2, this.retailGrid1 });
		this.tileView2.Appearance.EmptySpace.BackColor = System.Drawing.Color.FromArgb(219, 238, 244);
		this.tileView2.Appearance.EmptySpace.Options.UseBackColor = true;
		this.tileView2.Appearance.ItemFocused.BackColor = System.Drawing.Color.FromArgb(192, 255, 255);
		this.tileView2.Appearance.ItemFocused.Options.UseBackColor = true;
		this.tileView2.Appearance.ItemNormal.BackColor = System.Drawing.Color.FromArgb(192, 192, 255);
		this.tileView2.Appearance.ItemNormal.Options.UseBackColor = true;
		this.tileView2.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.tileViewColumn3, this.tileViewColumn4, this.tileViewColumn5, this.tileViewColumn9, this.tileViewColumn11, this.tileViewColumn12, this.tileViewColumn13, this.tileViewColumn14, this.tileViewColumn15, this.tileViewColumn16,
			this.tileViewColumn17, this.tileViewColumn18
		});
		this.tileView2.ColumnSet.CheckedColumn = this.tileViewColumn17;
		this.tileView2.GridControl = this.gridControlEx1;
		this.tileView2.Name = "tileView2";
		this.tileView2.OptionsTiles.IndentBetweenGroups = 0;
		this.tileView2.OptionsTiles.IndentBetweenItems = 0;
		this.tileView2.OptionsTiles.ItemPadding = new System.Windows.Forms.Padding(0);
		this.tileView2.OptionsTiles.ItemSize = new System.Drawing.Size(248, 50);
		this.tileView2.OptionsTiles.LayoutMode = DevExpress.XtraGrid.Views.Tile.TileViewLayoutMode.List;
		this.tileView2.OptionsTiles.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.tileView2.OptionsTiles.Padding = new System.Windows.Forms.Padding(0);
		tableColumnDefinition11.Length.Value = 27.0;
		tableColumnDefinition12.Length.Value = 54.0;
		tableColumnDefinition13.Length.Value = 39.0;
		tableColumnDefinition14.Length.Value = 71.0;
		tableColumnDefinition15.Length.Value = 33.0;
		itemTemplate2.Columns.Add(tableColumnDefinition11);
		itemTemplate2.Columns.Add(tableColumnDefinition12);
		itemTemplate2.Columns.Add(tableColumnDefinition13);
		itemTemplate2.Columns.Add(tableColumnDefinition14);
		itemTemplate2.Columns.Add(tableColumnDefinition15);
		tileViewItemElement15.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement15.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement15.Column = this.tileViewColumn4;
		tileViewItemElement15.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement15.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement15.ImageVisible = false;
		tileViewItemElement15.RowIndex = 1;
		tileViewItemElement15.Text = "tileViewColumn4";
		tileViewItemElement15.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement15.TextVisible = false;
		tileViewItemElement16.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement16.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement16.Column = this.tileViewColumn5;
		tileViewItemElement16.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement16.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement16.Text = "tileViewColumn5";
		tileViewItemElement16.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement17.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement17.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement17.Column = this.tileViewColumn9;
		tileViewItemElement17.ColumnIndex = 2;
		tileViewItemElement17.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement17.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement17.RowIndex = 1;
		tileViewItemElement17.Text = "tileViewColumn9";
		tileViewItemElement17.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement18.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement18.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement18.Column = this.tileViewColumn11;
		tileViewItemElement18.ColumnIndex = 1;
		tileViewItemElement18.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement18.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement18.RowIndex = 1;
		tileViewItemElement18.Text = "tileViewColumn11";
		tileViewItemElement18.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement19.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement19.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement19.Column = this.tileViewColumn15;
		tileViewItemElement19.ColumnIndex = 3;
		tileViewItemElement19.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement19.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement19.RowIndex = 1;
		tileViewItemElement19.Text = "tileViewColumn15";
		tileViewItemElement19.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement20.ColumnIndex = 3;
		tileViewItemElement20.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement20.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement20.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		itemTemplate2.Elements.Add(tileViewItemElement15);
		itemTemplate2.Elements.Add(tileViewItemElement16);
		itemTemplate2.Elements.Add(tileViewItemElement17);
		itemTemplate2.Elements.Add(tileViewItemElement18);
		itemTemplate2.Elements.Add(tileViewItemElement19);
		itemTemplate2.Elements.Add(tileViewItemElement20);
		itemTemplate2.Name = "template0";
		itemTemplate2.Rows.Add(obj3);
		itemTemplate2.Rows.Add(obj4);
		tableSpan3.ColumnSpan = 3;
		itemTemplate2.Spans.Add(tableSpan3);
		this.tileView2.Templates.Add(itemTemplate2);
		tableColumnDefinition16.Length.Value = 20.0;
		tableColumnDefinition17.Length.Value = 53.0;
		tableColumnDefinition18.Length.Value = 46.0;
		tableColumnDefinition19.Length.Value = 39.0;
		tableColumnDefinition20.Length.Value = 66.0;
		this.tileView2.TileColumns.Add(tableColumnDefinition16);
		this.tileView2.TileColumns.Add(tableColumnDefinition17);
		this.tileView2.TileColumns.Add(tableColumnDefinition18);
		this.tileView2.TileColumns.Add(tableColumnDefinition19);
		this.tileView2.TileColumns.Add(tableColumnDefinition20);
		tableRowDefinition3.Length.Value = 25.0;
		tableRowDefinition4.Length.Value = 21.0;
		this.tileView2.TileRows.Add(tableRowDefinition3);
		this.tileView2.TileRows.Add(tableRowDefinition4);
		tableSpan4.ColumnSpan = 4;
		this.tileView2.TileSpans.Add(tableSpan4);
		tileViewItemElement21.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement21.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement21.Column = this.tileViewColumn4;
		tileViewItemElement21.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement21.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement21.ImageVisible = false;
		tileViewItemElement21.RowIndex = 1;
		tileViewItemElement21.Text = "tileViewColumn4";
		tileViewItemElement21.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement21.TextVisible = false;
		tileViewItemElement22.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement22.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement22.Column = this.tileViewColumn5;
		tileViewItemElement22.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement22.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement22.Text = "tileViewColumn5";
		tileViewItemElement22.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement23.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement23.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement23.Column = this.tileViewColumn9;
		tileViewItemElement23.ColumnIndex = 2;
		tileViewItemElement23.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement23.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement23.RowIndex = 1;
		tileViewItemElement23.Text = "tileViewColumn9";
		tileViewItemElement23.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement24.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement24.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement24.Column = this.tileViewColumn11;
		tileViewItemElement24.ColumnIndex = 1;
		tileViewItemElement24.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement24.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement24.RowIndex = 1;
		tileViewItemElement24.Text = "tileViewColumn11";
		tileViewItemElement24.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement25.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement25.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement25.Column = this.tileViewColumn15;
		tileViewItemElement25.ColumnIndex = 4;
		tileViewItemElement25.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement25.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement25.RowIndex = 1;
		tileViewItemElement25.Text = "tileViewColumn15";
		tileViewItemElement25.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement26.ColumnIndex = 4;
		tileViewItemElement26.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement26.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement26.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement27.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		tileViewItemElement27.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement27.Column = this.tileViewColumn13;
		tileViewItemElement27.ColumnIndex = 3;
		tileViewItemElement27.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement27.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement27.RowIndex = 1;
		tileViewItemElement27.Text = "tileViewColumn13";
		tileViewItemElement27.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		this.tileView2.TileTemplate.Add(tileViewItemElement21);
		this.tileView2.TileTemplate.Add(tileViewItemElement22);
		this.tileView2.TileTemplate.Add(tileViewItemElement23);
		this.tileView2.TileTemplate.Add(tileViewItemElement24);
		this.tileView2.TileTemplate.Add(tileViewItemElement25);
		this.tileView2.TileTemplate.Add(tileViewItemElement26);
		this.tileView2.TileTemplate.Add(tileViewItemElement27);
		this.tileViewColumn3.Caption = "Type";
		this.tileViewColumn3.FieldName = "TranType";
		this.tileViewColumn3.Name = "tileViewColumn3";
		this.tileViewColumn3.OptionsColumn.AllowEdit = false;
		this.tileViewColumn3.OptionsColumn.ReadOnly = true;
		this.tileViewColumn3.Visible = true;
		this.tileViewColumn3.VisibleIndex = 0;
		this.tileViewColumn3.Width = 68;
		this.tileViewColumn12.Caption = "Discount (%)";
		this.tileViewColumn12.ColumnEdit = this.repositoryItemCalcEdit1;
		this.tileViewColumn12.FieldName = "DiscPrcnt";
		this.tileViewColumn12.Name = "tileViewColumn12";
		this.tileViewColumn12.Visible = true;
		this.tileViewColumn12.VisibleIndex = 5;
		this.tileViewColumn12.Width = 106;
		this.tileViewColumn14.Caption = "Tax Amount";
		this.tileViewColumn14.FieldName = "VatSum";
		this.tileViewColumn14.Name = "tileViewColumn14";
		this.tileViewColumn14.Visible = true;
		this.tileViewColumn14.VisibleIndex = 7;
		this.tileViewColumn16.Caption = "Edit";
		this.tileViewColumn16.ColumnEdit = this.repositoryItemButtonEdit2;
		this.tileViewColumn16.ImageAlignment = System.Drawing.StringAlignment.Center;
		this.tileViewColumn16.Name = "tileViewColumn16";
		this.tileViewColumn16.Visible = true;
		this.tileViewColumn16.VisibleIndex = 9;
		this.tileViewColumn16.Width = 139;
		this.repositoryItemButtonEdit2.AutoHeight = false;
		editorButtonImageOptions7.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions7.Image");
		editorButtonImageOptions8.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions8.Image");
		this.repositoryItemButtonEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions7),
			new DevExpress.XtraEditors.Controls.EditorButton(editorButtonImageOptions8, DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, null)
		});
		this.repositoryItemButtonEdit2.Name = "repositoryItemButtonEdit2";
		this.repositoryItemButtonEdit2.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
		this.tileViewColumn17.Caption = "tileViewColumn11";
		this.tileViewColumn17.FieldName = "Checked";
		this.tileViewColumn17.Name = "tileViewColumn17";
		this.tileViewColumn17.UnboundType = DevExpress.Data.UnboundColumnType.Boolean;
		this.tileViewColumn17.Visible = true;
		this.tileViewColumn17.VisibleIndex = 10;
		this.retailGrid1.Appearance.Empty.BackColor = System.Drawing.Color.FromArgb(219, 238, 244);
		this.retailGrid1.Appearance.Empty.Options.UseBackColor = true;
		this.retailGrid1.Appearance.HeaderPanel.BackColor = System.Drawing.Color.FromArgb(219, 238, 244);
		this.retailGrid1.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.retailGrid1.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.Black;
		this.retailGrid1.Appearance.HeaderPanel.Options.UseBackColor = true;
		this.retailGrid1.Appearance.HeaderPanel.Options.UseFont = true;
		this.retailGrid1.Appearance.HeaderPanel.Options.UseForeColor = true;
		this.retailGrid1.Appearance.Row.BackColor = System.Drawing.Color.FromArgb(219, 238, 244);
		this.retailGrid1.Appearance.Row.Options.UseBackColor = true;
		this.retailGrid1.Appearance.SelectedRow.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.retailGrid1.Appearance.SelectedRow.Options.UseBackColor = true;
		this.retailGrid1.AutoFillColumn = this.gridColumnEx1;
		this.retailGrid1.ColumnKey = null;
		this.retailGrid1.ColumnPanelRowHeight = 25;
		this.retailGrid1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7, this.gridColumnEx8, this.gridColumnEx9, this.gridColumnEx1, this.gridColumnEx10 });
		this.retailGrid1.DisableFormSetting = false;
		this.retailGrid1.DisplayException = false;
		this.retailGrid1.EditColumn = null;
		this.retailGrid1.FieldCurrency = null;
		this.retailGrid1.FieldRate = null;
		this.retailGrid1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.retailGrid1.FormId = "ucRetailGrid";
		this.retailGrid1.GridControl = this.gridControlEx1;
		this.retailGrid1.IndexEditRow = -1;
		this.retailGrid1.IndicatorWidth = 50;
		this.retailGrid1.IsInsertNewRow = false;
		this.retailGrid1.IsLoadingData = false;
		this.retailGrid1.ItemId = "ORETAILGRID";
		this.retailGrid1.LstColumNotSetting = null;
		this.retailGrid1.Name = "retailGrid1";
		this.retailGrid1.OptionsClipboard.AllowCopy = DevExpress.Utils.DefaultBoolean.True;
		this.retailGrid1.OptionsClipboard.CopyCollapsedData = DevExpress.Utils.DefaultBoolean.True;
		this.retailGrid1.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.True;
		this.retailGrid1.OptionsNavigation.AutoFocusNewRow = true;
		this.retailGrid1.OptionsView.ColumnHeaderAutoHeight = DevExpress.Utils.DefaultBoolean.False;
		this.retailGrid1.OptionsView.RowAutoHeight = true;
		this.retailGrid1.OptionsView.ShowGroupPanel = false;
		this.retailGrid1.PreviewLineCount = 1;
		this.retailGrid1.RowHeight = 50;
		this.gridColumnEx1.Caption = "Total";
		this.gridColumnEx1.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.DisplayFormat.FormatString = "n0";
		this.gridColumnEx1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumnEx1.FieldName = "LineTotal";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 4;
		this.gridColumnEx1.Width = 164;
		this.gridColumnEx2.Caption = "Type";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "TranType";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.OptionsColumn.AllowEdit = false;
		this.gridColumnEx2.OptionsColumn.ReadOnly = true;
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.Width = 68;
		this.gridColumnEx3.Caption = "Product Code";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "ItemCode";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsColumn.ReadOnly = true;
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 0;
		this.gridColumnEx3.Width = 116;
		this.gridColumnEx4.Caption = "Description";
		this.gridColumnEx4.ColumnEdit = this.repositoryItemMemoEdit2;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "Dscription";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.OptionsColumn.ReadOnly = true;
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 1;
		this.gridColumnEx4.Width = 131;
		this.gridColumnEx5.AppearanceCell.Options.UseTextOptions = true;
		this.gridColumnEx5.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gridColumnEx5.Caption = "Qty";
		this.gridColumnEx5.ColumnEdit = this.repositoryItemButtonEdit1;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "Quantity";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 2;
		this.gridColumnEx5.Width = 141;
		this.gridColumnEx6.Caption = "Price";
		this.gridColumnEx6.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.DisplayFormat.FormatString = "n0";
		this.gridColumnEx6.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumnEx6.FieldName = "Price";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 3;
		this.gridColumnEx6.Width = 146;
		this.gridColumnEx7.Caption = "Discount (%)";
		this.gridColumnEx7.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "DiscPrcnt";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.Width = 106;
		this.gridColumnEx8.Caption = "Tax (%)";
		this.gridColumnEx8.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "VatPrcnt";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.Width = 78;
		this.gridColumnEx9.Caption = "Tax Amount";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "Vatsum";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx10.Caption = "Edit";
		this.gridColumnEx10.ColumnEdit = this.repositoryItemButtonEdit2;
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.ImageAlignment = System.Drawing.StringAlignment.Center;
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 5;
		this.gridColumnEx10.Width = 139;
		this.tileViewColumn20.Caption = "Free Text";
		this.tileViewColumn20.FieldName = "FreeTxt";
		this.tileViewColumn20.Name = "tileViewColumn20";
		this.tileViewColumn20.Visible = true;
		this.tileViewColumn20.VisibleIndex = 12;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.gridControlEx1);
		base.Controls.Add(this.grd_Contents);
		base.Name = "ucRetailGrid";
		base.Size = new System.Drawing.Size(753, 475);
		base.Load += new System.EventHandler(this.ucRetailGrid_Load);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Qty).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Price).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Rate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Contents).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tileView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Edit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Contents).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridControlEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tileView2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.retailGrid1).EndInit();
		base.ResumeLayout(false);
	}
}
