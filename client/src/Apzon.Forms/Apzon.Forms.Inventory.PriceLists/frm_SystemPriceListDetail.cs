using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.UserControls;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Inventory.PriceLists;

public class frm_SystemPriceListDetail : XtraFormEx
{
	private IContainer components;

	private LabelLine lbSearch;

	private TextEditEx txtSearchText;

	private SimpleButtonEx btnCancel;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridColumnEx gcItemCode;

	private GridColumnEx gcItemName;

	private GridColumnEx gcPrice;

	private GridColumnEx gcInventoryUom;

	private GridColumnEx gcPriceUnit;

	private RepositoryItemButtonEditCFL rItemCode;

	private RepositoryItemButtonEditCFL rItemName;

	private RepositoryItemTextEditExPrice rPrice;

	private SystemPriceList _priceList { get; set; }

	private frm_SystemPriceListDetail()
	{
		InitializeComponent();
	}

	public frm_SystemPriceListDetail(SystemPriceList priceList)
	{
		InitializeComponent();
		_priceList = priceList;
	}

	private void frm_SystemPriceListDetail_Load(object sender, EventArgs e)
	{
		Text = ((_priceList == SystemPriceList.LastEvaluatedPrice) ? "Last Evaluated Price" : "Last Purchase Price");
		try
		{
			DataTable dataSystemPriceListDetail = HttpClient.PriceListSetup.GetDataSystemPriceListDetail((int)_priceList);
			grdDoc.DataSource = dataSystemPriceListDetail;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void txtSearchText_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!string.IsNullOrEmpty(txtSearchText.Text))
			{
				string activeFilterString = $"ItemCode LIKE '%{txtSearchText.Text}%' ";
				gvDoc.ActiveFilterString = activeFilterString;
			}
			else
			{
				gvDoc.ActiveFilterString = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.PriceLists.frm_SystemPriceListDetail));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		this.lbSearch = new Apzon.Controls.Controls.LabelLine();
		this.txtSearchText = new Apzon.Controls.Editor.TextEditEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rItemCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.rItemName = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcPrice = new Apzon.Controls.Grid.GridColumnEx();
		this.rPrice = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.gcInventoryUom = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPriceUnit = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.txtSearchText.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rItemName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rPrice).BeginInit();
		base.SuspendLayout();
		this.lbSearch.EditControl = null;
		this.lbSearch.EditValue = "Search text";
		this.lbSearch.FieldName = null;
		this.lbSearch.HideWhenEmpty = false;
		this.lbSearch.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbSearch.Location = new System.Drawing.Point(32, 9);
		this.lbSearch.Name = "lbSearch";
		this.lbSearch.ReadOnly = false;
		this.lbSearch.SettingInVisible = false;
		this.lbSearch.SettingReadonly = false;
		this.lbSearch.ShowUnderLine = true;
		this.lbSearch.Size = new System.Drawing.Size(80, 20);
		this.lbSearch.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbSearch.TabIndex = 0;
		this.lbSearch.Utext = "Search text";
		this.txtSearchText.EditControl = null;
		this.txtSearchText.FieldName = null;
		this.txtSearchText.IsSearchControl = false;
		this.txtSearchText.Location = new System.Drawing.Point(113, 6);
		this.txtSearchText.Name = "txtSearchText";
		this.txtSearchText.SettingInVisible = false;
		this.txtSearchText.SettingReadonly = false;
		this.txtSearchText.Size = new System.Drawing.Size(180, 20);
		this.txtSearchText.TabIndex = 1;
		this.txtSearchText.EditValueChanged += new System.EventHandler(this.txtSearchText_EditValueChanged);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(508, 524);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 2;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.Location = new System.Drawing.Point(1, 34);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rItemCode, this.rItemName, this.rPrice });
		this.grdDoc.Size = new System.Drawing.Size(582, 484);
		this.grdDoc.TabIndex = 4;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.gvDoc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gvDoc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gcItemCode, this.gcItemName, this.gcPrice, this.gcInventoryUom, this.gcPriceUnit });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = null;
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = null;
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gvDoc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gvDoc.OptionsPrint.AutoWidth = false;
		this.gvDoc.OptionsPrint.UsePrintStyles = false;
		this.gvDoc.OptionsView.BestFitMaxRowCount = 100;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvDoc.OptionsView.ShowGroupedColumns = true;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gcItemCode.AllowChSet = false;
		this.gcItemCode.Caption = "Item Code";
		this.gcItemCode.ColumnEdit = this.rItemCode;
		this.gcItemCode.CurrencyDateField = null;
		this.gcItemCode.CurrencyField = null;
		this.gcItemCode.CurrencyFixed = null;
		this.gcItemCode.CurrencyFixedRate = 0.0;
		this.gcItemCode.CurrencyRateField = null;
		this.gcItemCode.FieldName = "ItemCode";
		this.gcItemCode.Name = "gcItemCode";
		this.gcItemCode.OptionsColumn.ReadOnly = true;
		this.gcItemCode.PrimaryKey = true;
		this.gcItemCode.UnitField = null;
		this.gcItemCode.Visible = true;
		this.gcItemCode.VisibleIndex = 0;
		this.gcItemCode.Width = 124;
		this.rItemCode.AutoHeight = false;
		this.rItemCode.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.rItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rItemCode.ContainerControl = this;
		this.rItemCode.DisplayMember = "ItemCode";
		this.rItemCode.DisplayMember2 = "";
		this.rItemCode.LoadDataSourceWhenEnter = false;
		this.rItemCode.Name = "rItemCode";
		this.rItemCode.ReadOnly = true;
		this.rItemCode.SeparateCharacterForMulti = ';';
		this.rItemCode.SeparateString = ", ";
		this.rItemCode.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Items;
		this.rItemCode.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oItems;
		this.rItemCode.UsingAutoCompleteBox = false;
		this.rItemCode.UsingDataSource = true;
		this.rItemCode.ValidateWhenNoDataSource = false;
		this.rItemCode.ValueMember = "ItemCode";
		this.gcItemName.AllowChSet = false;
		this.gcItemName.Caption = "Item Name";
		this.gcItemName.ColumnEdit = this.rItemName;
		this.gcItemName.CurrencyDateField = null;
		this.gcItemName.CurrencyField = null;
		this.gcItemName.CurrencyFixed = null;
		this.gcItemName.CurrencyFixedRate = 0.0;
		this.gcItemName.CurrencyRateField = null;
		this.gcItemName.FieldName = "ItemName";
		this.gcItemName.Name = "gcItemName";
		this.gcItemName.OptionsColumn.ReadOnly = true;
		this.gcItemName.PrimaryKey = false;
		this.gcItemName.UnitField = null;
		this.gcItemName.Visible = true;
		this.gcItemName.VisibleIndex = 1;
		this.gcItemName.Width = 156;
		this.rItemName.AutoHeight = false;
		this.rItemName.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.rItemName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rItemName.ContainerControl = this;
		this.rItemName.DisplayMember = "ItemName";
		this.rItemName.DisplayMember2 = "";
		this.rItemName.LoadDataSourceWhenEnter = false;
		this.rItemName.Name = "rItemName";
		this.rItemName.ReadOnly = true;
		this.rItemName.SeparateCharacterForMulti = ';';
		this.rItemName.SeparateString = ", ";
		this.rItemName.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Items;
		this.rItemName.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oItems;
		this.rItemName.UsingAutoCompleteBox = false;
		this.rItemName.UsingDataSource = true;
		this.rItemName.ValidateWhenNoDataSource = false;
		this.rItemName.ValueMember = "ItemName";
		this.gcPrice.AllowChSet = false;
		this.gcPrice.Caption = "Price per Inventory UoM";
		this.gcPrice.ColumnEdit = this.rPrice;
		this.gcPrice.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcPrice.CurrencyDateField = null;
		this.gcPrice.CurrencyField = "Currency";
		this.gcPrice.CurrencyFixed = null;
		this.gcPrice.CurrencyFixedRate = 0.0;
		this.gcPrice.CurrencyRateField = null;
		this.gcPrice.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcPrice.FieldName = "Price";
		this.gcPrice.Name = "gcPrice";
		this.gcPrice.OptionsColumn.ReadOnly = true;
		this.gcPrice.PrimaryKey = false;
		this.gcPrice.UnitField = null;
		this.gcPrice.Visible = true;
		this.gcPrice.VisibleIndex = 2;
		this.gcPrice.Width = 155;
		this.rPrice.AutoHeight = false;
		this.rPrice.Name = "rPrice";
		this.gcInventoryUom.AllowChSet = false;
		this.gcInventoryUom.Caption = "Inventory UoM";
		this.gcInventoryUom.CurrencyDateField = null;
		this.gcInventoryUom.CurrencyField = null;
		this.gcInventoryUom.CurrencyFixed = null;
		this.gcInventoryUom.CurrencyFixedRate = 0.0;
		this.gcInventoryUom.CurrencyRateField = null;
		this.gcInventoryUom.FieldName = "InvUom";
		this.gcInventoryUom.Name = "gcInventoryUom";
		this.gcInventoryUom.OptionsColumn.ReadOnly = true;
		this.gcInventoryUom.PrimaryKey = false;
		this.gcInventoryUom.UnitField = null;
		this.gcInventoryUom.Visible = true;
		this.gcInventoryUom.VisibleIndex = 3;
		this.gcInventoryUom.Width = 115;
		this.gcPriceUnit.Caption = "Pricing Unit";
		this.gcPriceUnit.CurrencyDateField = null;
		this.gcPriceUnit.CurrencyField = null;
		this.gcPriceUnit.CurrencyFixed = null;
		this.gcPriceUnit.CurrencyFixedRate = 0.0;
		this.gcPriceUnit.CurrencyRateField = null;
		this.gcPriceUnit.FieldName = "PriceUnit";
		this.gcPriceUnit.Name = "gcPriceUnit";
		this.gcPriceUnit.PrimaryKey = false;
		this.gcPriceUnit.UnitField = null;
		this.gcPriceUnit.Visible = true;
		this.gcPriceUnit.VisibleIndex = 4;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(585, 551);
		base.Controls.Add(this.grdDoc);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.txtSearchText);
		base.Controls.Add(this.lbSearch);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SystemPriceListDetail";
		this.Text = "Last Purchase Price";
		base.Load += new System.EventHandler(this.frm_SystemPriceListDetail_Load);
		((System.ComponentModel.ISupportInitialize)this.txtSearchText.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rItemName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rPrice).EndInit();
		base.ResumeLayout(false);
	}
}
