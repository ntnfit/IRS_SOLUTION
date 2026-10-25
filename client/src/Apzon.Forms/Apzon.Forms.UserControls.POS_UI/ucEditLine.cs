using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucEditLine : XtraUserControl
{
	public DataRow DrLine;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isLoading;

	public bool IsPromotionItem;

	private IContainer components;

	private LabelControlEx labelControlEx3;

	private ucNumPad ucNumPad1;

	private LabelControlEx labelControlEx4;

	private CalcEditResizable txt_Discount;

	private LabelControlEx labelControlEx5;

	private LabelControlEx labelControlEx1;

	private CalcEditResizable txt_Quantity;

	private LabelControlEx labelControlEx6;

	private CalcEditResizable txt_Total;

	private CalcEditResizable txt_Price;

	private CalcEditResizable txt_Tax;

	private LabelControlEx labelControlEx8;

	private LabelControlEx lbl_ItemName;

	private GridLookUpEdit luk_Tax;

	private RetailGrid gridLookUpEdit1View;

	private GridColumn gridColumn1;

	private GridColumn gridColumn2;

	private GridColumn gridColumn3;

	private LabelControlEx labelControlEx7;

	private LabelControlEx labelControlEx2;

	private MemoEdit memo_Remarks;

	public string FormId
	{
		get
		{
			return gridLookUpEdit1View.FormId;
		}
		set
		{
			gridLookUpEdit1View.FormId = value;
		}
	}

	public string ItemId
	{
		get
		{
			return gridLookUpEdit1View.ItemId;
		}
		set
		{
			gridLookUpEdit1View.ItemId = value;
		}
	}

	public ucEditLine()
	{
		InitializeComponent();
		SetEventControl(this);
		Load += OnLoad;
	}

	private async void OnLoad(object sender, EventArgs eventArgs)
	{
		try
		{
			txt_Total.ReadOnly = IsPromotionItem;
			txt_Price.ReadOnly = IsPromotionItem;
			txt_Discount.ReadOnly = IsPromotionItem;
			txt_Tax.ReadOnly = IsPromotionItem;
			luk_Tax.ReadOnly = IsPromotionItem;
			DataTable dataSource = await _httpClient.Retail.GetAllTaxCodes();
			luk_Tax.Properties.ValueMember = "Code";
			luk_Tax.Properties.DisplayMember = "Rate";
			luk_Tax.Properties.DataSource = dataSource;
			SetValues();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetEventControl(Control ctr)
	{
		try
		{
			foreach (Control control in ctr.Controls)
			{
				if (control is ButtonEdit { ReadOnly: false } buttonEdit && buttonEdit.Properties.TextEditStyle != TextEditStyles.DisableTextEditor)
				{
					buttonEdit.GotFocus += BtnOnGotFocus;
				}
				else
				{
					SetEventControl(control);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void BtnOnGotFocus(object sender, EventArgs eventArgs1)
	{
		try
		{
			ButtonEdit clientControl = sender as ButtonEdit;
			ucNumPad1.ClientControl = clientControl;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void SetValues()
	{
		try
		{
			if (DrLine != null)
			{
				_isLoading = true;
				txt_Total.EditValue = CalcLineTotalAfterTax(Function.ParseDouble(DrLine["LineTotal"]), Function.ParseDouble(DrLine["VatPrcnt"]));
				txt_Price.EditValue = Function.ParseDouble(DrLine["PriceBefDi"]);
				txt_Quantity.EditValue = Function.ParseDouble(DrLine["Quantity"]);
				txt_Discount.EditValue = Function.ParseDouble(DrLine["DiscPrcnt"]);
				txt_Tax.EditValue = Function.ParseDouble(DrLine["VatPrcnt"]);
				luk_Tax.EditValue = Function.ToString(DrLine["VatGroup"]);
				lbl_ItemName.Text = Function.ToString(DrLine["Dscription"]);
				memo_Remarks.EditValue = Function.ToString(DrLine["FreeTxt"]);
				_isLoading = false;
				txt_Quantity.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Quantity_EditValueChanged(object sender, EventArgs e)
	{
		RecalcValues();
	}

	private void RecalcValues()
	{
		try
		{
			if (DrLine != null && !_isLoading)
			{
				DrLine["PriceBefDi"] = Function.ParseDouble(txt_Price.EditValue);
				DrLine["VatPrcnt"] = Function.ParseDouble(txt_Tax.EditValue);
				DrLine["DiscPrcnt"] = Function.ParseDouble(txt_Discount.EditValue);
				DrLine["Quantity"] = Function.ParseDouble(txt_Quantity.EditValue);
				DrLine["VatGroup"] = Function.ToString(luk_Tax.EditValue);
				double price = Function.ParseDouble(DrLine["PriceBefDi"]);
				double vat = Function.ParseDouble(DrLine["VatPrcnt"]);
				double disc = Function.ParseDouble(DrLine["DiscPrcnt"]);
				double qty = Function.ParseDouble(DrLine["Quantity"]);
				double lineTotal = CalcLineTotal(qty, price, disc);
				double num = CalcLineTotalAfterTax(lineTotal, vat);
				txt_Total.EditValue = num;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public double CalcLineTotal(double qty, double price, double disc)
	{
		try
		{
			double num = qty * price;
			return num - num * disc / 100.0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return 0.0;
	}

	private double CalcLineTotalAfterTax(double lineTotal, double vat)
	{
		try
		{
			return lineTotal + lineTotal * vat / 100.0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return 0.0;
	}

	private void txt_Discount_EditValueChanged(object sender, EventArgs e)
	{
		RecalcValues();
	}

	private void txt_Tax_EditValueChanged(object sender, EventArgs e)
	{
		RecalcValues();
	}

	private void txt_Price_EditValueChanged(object sender, EventArgs e)
	{
		RecalcValues();
	}

	private void luk_Tax_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			int indexByKeyValue = luk_Tax.Properties.GetIndexByKeyValue(luk_Tax.EditValue);
			txt_Tax.EditValue = luk_Tax.Properties.GetDisplayValue(indexByKeyValue);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Discount_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseDouble(e.NewValue) > 100.0 || Function.ParseDouble(e.NewValue) < -100.0)
			{
				e.Cancel = true;
				WinCommons.Error(Enums.MessageCode.None, "Không thể chiết khấu quá 100%");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void memo_Remarks_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			DrLine["FreeTxt"] = Function.ToString(memo_Remarks.EditValue);
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
		this.luk_Tax = new DevExpress.XtraEditors.GridLookUpEdit();
		this.gridLookUpEdit1View = new Apzon.Forms.UserControls.POS_UI.RetailGrid();
		this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.labelControlEx7 = new Apzon.Controls.Controls.LabelControlEx();
		this.labelControlEx2 = new Apzon.Controls.Controls.LabelControlEx();
		this.lbl_ItemName = new Apzon.Controls.Controls.LabelControlEx();
		this.labelControlEx8 = new Apzon.Controls.Controls.LabelControlEx();
		this.txt_Tax = new Apzon.Forms.UserControls.POS_UI.CalcEditResizable();
		this.txt_Price = new Apzon.Forms.UserControls.POS_UI.CalcEditResizable();
		this.labelControlEx6 = new Apzon.Controls.Controls.LabelControlEx();
		this.txt_Quantity = new Apzon.Forms.UserControls.POS_UI.CalcEditResizable();
		this.labelControlEx1 = new Apzon.Controls.Controls.LabelControlEx();
		this.txt_Discount = new Apzon.Forms.UserControls.POS_UI.CalcEditResizable();
		this.labelControlEx5 = new Apzon.Controls.Controls.LabelControlEx();
		this.txt_Total = new Apzon.Forms.UserControls.POS_UI.CalcEditResizable();
		this.labelControlEx4 = new Apzon.Controls.Controls.LabelControlEx();
		this.ucNumPad1 = new Apzon.Forms.UserControls.POS_UI.ucNumPad();
		this.labelControlEx3 = new Apzon.Controls.Controls.LabelControlEx();
		this.memo_Remarks = new DevExpress.XtraEditors.MemoEdit();
		((System.ComponentModel.ISupportInitialize)this.luk_Tax.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridLookUpEdit1View).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Tax.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Price.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Quantity.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Discount.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Total.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Remarks.Properties).BeginInit();
		base.SuspendLayout();
		this.luk_Tax.Location = new System.Drawing.Point(90, 130);
		this.luk_Tax.Name = "luk_Tax";
		this.luk_Tax.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 17.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.luk_Tax.Properties.Appearance.Options.UseFont = true;
		this.luk_Tax.Properties.Appearance.Options.UseTextOptions = true;
		this.luk_Tax.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.luk_Tax.Properties.NullText = "";
		this.luk_Tax.Properties.View = this.gridLookUpEdit1View;
		this.luk_Tax.Properties.EditValueChanged += new System.EventHandler(this.luk_Tax_EditValueChanged);
		this.luk_Tax.Size = new System.Drawing.Size(171, 34);
		this.luk_Tax.TabIndex = 53;
		this.gridLookUpEdit1View.ColumnKey = null;
		this.gridLookUpEdit1View.ColumnPanelRowHeight = 25;
		this.gridLookUpEdit1View.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gridColumn1, this.gridColumn2, this.gridColumn3 });
		this.gridLookUpEdit1View.DisableFormSetting = false;
		this.gridLookUpEdit1View.DisplayException = false;
		this.gridLookUpEdit1View.EditColumn = null;
		this.gridLookUpEdit1View.FieldCurrency = null;
		this.gridLookUpEdit1View.FieldRate = null;
		this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.gridLookUpEdit1View.FormId = null;
		this.gridLookUpEdit1View.IndexEditRow = -1;
		this.gridLookUpEdit1View.IndicatorWidth = 70;
		this.gridLookUpEdit1View.IsInsertNewRow = false;
		this.gridLookUpEdit1View.IsLoadingData = false;
		this.gridLookUpEdit1View.ItemId = "ODATA";
		this.gridLookUpEdit1View.LstColumNotSetting = null;
		this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
		this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.gridLookUpEdit1View.OptionsView.ShowColumnHeaders = false;
		this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
		this.gridLookUpEdit1View.OptionsView.ShowIndicator = false;
		this.gridLookUpEdit1View.RowHeight = 60;
		this.gridColumn1.Caption = "Code";
		this.gridColumn1.FieldName = "Code";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn2.Caption = "Name";
		this.gridColumn2.FieldName = "Name";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn3.Caption = "Rate";
		this.gridColumn3.FieldName = "Rate";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 1;
		this.labelControlEx7.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx7.Appearance.Options.UseFont = true;
		this.labelControlEx7.FieldName = "labelControlEx1";
		this.labelControlEx7.Location = new System.Drawing.Point(267, 140);
		this.labelControlEx7.Name = "labelControlEx7";
		this.labelControlEx7.Size = new System.Drawing.Size(18, 18);
		this.labelControlEx7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx7.TabIndex = 52;
		this.labelControlEx7.Text = "%";
		this.labelControlEx2.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx2.Appearance.Options.UseFont = true;
		this.labelControlEx2.FieldName = "labelControlEx1";
		this.labelControlEx2.Location = new System.Drawing.Point(4, 141);
		this.labelControlEx2.Name = "labelControlEx2";
		this.labelControlEx2.Size = new System.Drawing.Size(80, 18);
		this.labelControlEx2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx2.TabIndex = 51;
		this.labelControlEx2.Text = "Thuế GTGT";
		this.lbl_ItemName.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_ItemName.Appearance.Options.UseFont = true;
		this.lbl_ItemName.FieldName = "labelControlEx1";
		this.lbl_ItemName.Location = new System.Drawing.Point(90, 12);
		this.lbl_ItemName.Name = "lbl_ItemName";
		this.lbl_ItemName.Size = new System.Drawing.Size(0, 18);
		this.lbl_ItemName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ItemName.TabIndex = 49;
		this.labelControlEx8.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx8.Appearance.Options.UseFont = true;
		this.labelControlEx8.FieldName = "labelControlEx1";
		this.labelControlEx8.Location = new System.Drawing.Point(5, 12);
		this.labelControlEx8.Name = "labelControlEx8";
		this.labelControlEx8.Size = new System.Drawing.Size(68, 18);
		this.labelControlEx8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx8.TabIndex = 48;
		this.labelControlEx8.Text = "Mặt hàng";
		this.txt_Tax.FieldName = null;
		this.txt_Tax.Location = new System.Drawing.Point(286, 4);
		this.txt_Tax.Name = "txt_Tax";
		this.txt_Tax.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 16.77385f);
		this.txt_Tax.Properties.Appearance.Options.UseFont = true;
		this.txt_Tax.Properties.AutoHeight = false;
		this.txt_Tax.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.txt_Tax.Properties.NullValuePrompt = "0";
		this.txt_Tax.Properties.NullValuePromptShowForEmptyValue = true;
		this.txt_Tax.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.txt_Tax.Size = new System.Drawing.Size(171, 32);
		this.txt_Tax.TabIndex = 45;
		this.txt_Tax.Visible = false;
		this.txt_Tax.EditValueChanged += new System.EventHandler(this.txt_Tax_EditValueChanged);
		this.txt_Price.FieldName = null;
		this.txt_Price.Location = new System.Drawing.Point(91, 43);
		this.txt_Price.Name = "txt_Price";
		this.txt_Price.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 16.77385f);
		this.txt_Price.Properties.Appearance.Options.UseFont = true;
		this.txt_Price.Properties.AutoHeight = false;
		this.txt_Price.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.txt_Price.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.txt_Price.Properties.NullValuePrompt = "0";
		this.txt_Price.Properties.NullValuePromptShowForEmptyValue = true;
		this.txt_Price.Properties.ReadOnly = true;
		this.txt_Price.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.txt_Price.Size = new System.Drawing.Size(171, 32);
		this.txt_Price.TabIndex = 44;
		this.txt_Price.EditValueChanged += new System.EventHandler(this.txt_Price_EditValueChanged);
		this.labelControlEx6.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx6.Appearance.Options.UseFont = true;
		this.labelControlEx6.FieldName = "labelControlEx1";
		this.labelControlEx6.Location = new System.Drawing.Point(268, 185);
		this.labelControlEx6.Name = "labelControlEx6";
		this.labelControlEx6.Size = new System.Drawing.Size(18, 18);
		this.labelControlEx6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx6.TabIndex = 43;
		this.labelControlEx6.Text = "%";
		this.txt_Quantity.FieldName = null;
		this.txt_Quantity.Location = new System.Drawing.Point(91, 86);
		this.txt_Quantity.Name = "txt_Quantity";
		this.txt_Quantity.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 16.77385f);
		this.txt_Quantity.Properties.Appearance.Options.UseFont = true;
		this.txt_Quantity.Properties.AutoHeight = false;
		this.txt_Quantity.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.txt_Quantity.Properties.NullValuePrompt = "0";
		this.txt_Quantity.Properties.NullValuePromptShowForEmptyValue = true;
		this.txt_Quantity.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.txt_Quantity.Size = new System.Drawing.Size(171, 32);
		this.txt_Quantity.TabIndex = 41;
		this.txt_Quantity.EditValueChanged += new System.EventHandler(this.txt_Quantity_EditValueChanged);
		this.labelControlEx1.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx1.Appearance.Options.UseFont = true;
		this.labelControlEx1.FieldName = "labelControlEx1";
		this.labelControlEx1.Location = new System.Drawing.Point(5, 98);
		this.labelControlEx1.Name = "labelControlEx1";
		this.labelControlEx1.Size = new System.Drawing.Size(65, 18);
		this.labelControlEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx1.TabIndex = 40;
		this.labelControlEx1.Text = "Số lượng";
		this.txt_Discount.FieldName = null;
		this.txt_Discount.Location = new System.Drawing.Point(91, 174);
		this.txt_Discount.Name = "txt_Discount";
		this.txt_Discount.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 16.77385f);
		this.txt_Discount.Properties.Appearance.Options.UseFont = true;
		this.txt_Discount.Properties.AutoHeight = false;
		this.txt_Discount.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Rate;
		this.txt_Discount.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_Discount.Properties.NullValuePrompt = "0";
		this.txt_Discount.Properties.NullValuePromptShowForEmptyValue = true;
		this.txt_Discount.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.txt_Discount.Size = new System.Drawing.Size(171, 32);
		this.txt_Discount.TabIndex = 0;
		this.txt_Discount.EditValueChanged += new System.EventHandler(this.txt_Discount_EditValueChanged);
		this.txt_Discount.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_Discount_EditValueChanging);
		this.labelControlEx5.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx5.Appearance.Options.UseFont = true;
		this.labelControlEx5.FieldName = "labelControlEx1";
		this.labelControlEx5.Location = new System.Drawing.Point(5, 186);
		this.labelControlEx5.Name = "labelControlEx5";
		this.labelControlEx5.Size = new System.Drawing.Size(79, 18);
		this.labelControlEx5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx5.TabIndex = 36;
		this.labelControlEx5.Text = "Chiết khấu";
		this.txt_Total.FieldName = null;
		this.txt_Total.Location = new System.Drawing.Point(90, 215);
		this.txt_Total.Name = "txt_Total";
		this.txt_Total.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.txt_Total.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 16.77385f);
		this.txt_Total.Properties.Appearance.Options.UseBackColor = true;
		this.txt_Total.Properties.Appearance.Options.UseFont = true;
		this.txt_Total.Properties.AutoHeight = false;
		this.txt_Total.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.txt_Total.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.txt_Total.Properties.NullValuePrompt = "0";
		this.txt_Total.Properties.NullValuePromptShowForEmptyValue = true;
		this.txt_Total.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.txt_Total.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.txt_Total.Size = new System.Drawing.Size(171, 32);
		this.txt_Total.TabIndex = 35;
		this.labelControlEx4.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx4.Appearance.Options.UseFont = true;
		this.labelControlEx4.FieldName = "labelControlEx1";
		this.labelControlEx4.Location = new System.Drawing.Point(5, 226);
		this.labelControlEx4.Name = "labelControlEx4";
		this.labelControlEx4.Size = new System.Drawing.Size(69, 18);
		this.labelControlEx4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx4.TabIndex = 34;
		this.labelControlEx4.Text = "Tổng tiền";
		this.ucNumPad1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.ucNumPad1.ClientControl = this.txt_Discount;
		this.ucNumPad1.Location = new System.Drawing.Point(286, 57);
		this.ucNumPad1.Name = "ucNumPad1";
		this.ucNumPad1.Size = new System.Drawing.Size(300, 280);
		this.ucNumPad1.TabIndex = 33;
		this.labelControlEx3.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx3.Appearance.Options.UseFont = true;
		this.labelControlEx3.FieldName = "labelControlEx1";
		this.labelControlEx3.Location = new System.Drawing.Point(5, 55);
		this.labelControlEx3.Name = "labelControlEx3";
		this.labelControlEx3.Size = new System.Drawing.Size(57, 18);
		this.labelControlEx3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx3.TabIndex = 30;
		this.labelControlEx3.Text = "Đơn giá";
		this.memo_Remarks.EditValue = "";
		this.memo_Remarks.Location = new System.Drawing.Point(5, 250);
		this.memo_Remarks.Name = "memo_Remarks";
		this.memo_Remarks.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.memo_Remarks.Properties.Appearance.Options.UseFont = true;
		this.memo_Remarks.Properties.NullValuePrompt = "Ghi chú dòng";
		this.memo_Remarks.Properties.NullValuePromptShowForEmptyValue = true;
		this.memo_Remarks.Size = new System.Drawing.Size(257, 87);
		this.memo_Remarks.TabIndex = 54;
		this.memo_Remarks.EditValueChanged += new System.EventHandler(this.memo_Remarks_EditValueChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.memo_Remarks);
		base.Controls.Add(this.luk_Tax);
		base.Controls.Add(this.labelControlEx7);
		base.Controls.Add(this.labelControlEx2);
		base.Controls.Add(this.lbl_ItemName);
		base.Controls.Add(this.labelControlEx8);
		base.Controls.Add(this.txt_Tax);
		base.Controls.Add(this.txt_Price);
		base.Controls.Add(this.labelControlEx6);
		base.Controls.Add(this.txt_Quantity);
		base.Controls.Add(this.labelControlEx1);
		base.Controls.Add(this.txt_Discount);
		base.Controls.Add(this.labelControlEx5);
		base.Controls.Add(this.txt_Total);
		base.Controls.Add(this.labelControlEx4);
		base.Controls.Add(this.ucNumPad1);
		base.Controls.Add(this.labelControlEx3);
		base.Name = "ucEditLine";
		base.Size = new System.Drawing.Size(587, 340);
		((System.ComponentModel.ISupportInitialize)this.luk_Tax.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridLookUpEdit1View).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Tax.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Price.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Quantity.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Discount.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Total.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Remarks.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
