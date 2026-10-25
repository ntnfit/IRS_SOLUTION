using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucEditOrder : XtraUserControl
{
	public double Total;

	public double SubTotal;

	public double VatPrcnt;

	public double VatSum;

	public double Discount;

	public double DiscPrcnt;

	public string VatGroup;

	public string Comments;

	public DataTable DtLines;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isLoading;

	private bool _isDisSumChanging;

	private bool _isRecalc;

	private IContainer components;

	private LabelControlEx labelControlEx3;

	private CalcEditResizable txt_SubTotal;

	private ucNumPad ucNumPad1;

	private LabelControlEx labelControlEx4;

	private CalcEditResizable calc_DiscPrcnt;

	private LabelControlEx labelControlEx5;

	private LabelControlEx labelControlEx1;

	private LabelControlEx labelControlEx2;

	private LabelControlEx lbl_Disc;

	private CalcEditResizable txt_Total;

	private CalcEditResizable calc_DicsSum;

	private LabelControlEx labelControlEx6;

	private CalcEditResizable calc_VatSum;

	private LabelControlEx labelControlEx7;

	private GridLookUpEdit luk_Tax;

	private RetailGrid gridLookUpEdit1View;

	private GridColumn gridColumn1;

	private GridColumn gridColumn2;

	private GridColumn gridColumn3;

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

	public ucEditOrder()
	{
		InitializeComponent();
		SetEventControl(this);
		Load += OnLoad;
	}

	private async void OnLoad(object sender, EventArgs eventArgs)
	{
		try
		{
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
		ButtonEdit clientControl = sender as ButtonEdit;
		ucNumPad1.ClientControl = clientControl;
	}

	public void SetValues()
	{
		try
		{
			_isLoading = true;
			txt_Total.EditValue = Total;
			txt_SubTotal.EditValue = SubTotal;
			luk_Tax.EditValue = null;
			calc_VatSum.EditValue = VatSum;
			calc_DiscPrcnt.EditValue = DiscPrcnt;
			calc_DicsSum.EditValue = Discount;
			memo_Remarks.EditValue = Comments;
			_isLoading = false;
			calc_DiscPrcnt.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void RecalcValues()
	{
		try
		{
			if (_isLoading)
			{
				return;
			}
			_isRecalc = true;
			double num = (DiscPrcnt = ((!_isDisSumChanging) ? Math.Round(Function.ParseDouble(calc_DiscPrcnt.EditValue), GlobalData.GeneralSetting.RateDec.GetValueOrDefault()) : Math.Round(Function.ParseDouble(calc_DicsSum.EditValue) / SubTotal * 100.0, GlobalData.GeneralSetting.RateDec.GetValueOrDefault())));
			int indexByKeyValue = luk_Tax.Properties.GetIndexByKeyValue(luk_Tax.EditValue);
			VatGroup = Function.ToString(luk_Tax.EditValue);
			double num2 = Function.ParseDouble(luk_Tax.Properties.GetDisplayValue(indexByKeyValue));
			if (!string.IsNullOrEmpty(VatGroup))
			{
				for (int i = 0; i < DtLines.Rows.Count; i++)
				{
					DataRow dataRow = DtLines.Rows[i];
					dataRow["VatPrcnt"] = num2;
					double num3 = Function.ParseDouble(dataRow["Quantity"]);
					double num4 = Function.ParseDouble(dataRow["PriceBefDi"]);
					double num5 = Function.ParseDouble(dataRow["DiscPrcnt"]);
					double num6 = num3 * num4;
					double num7 = (num6 - num6 * num5 / 100.0) * num2 / 100.0 * (1.0 - num / 100.0);
					dataRow["LineVat"] = num7;
					double num8 = CalcLineTotal(num3, num4, num5);
					dataRow["LineTotal"] = num8;
				}
			}
			else if (DiscPrcnt >= 0.0)
			{
				foreach (DataRow row in DtLines.Rows)
				{
					double num9 = Function.ParseDouble(row["LineTotal"]);
					double num10 = Function.ParseDouble(row["VatPrcnt"]);
					row["LineVat"] = num9 * num10 / 100.0 * (1.0 - num / 100.0);
				}
			}
			DtLines.AcceptChanges();
			VatPrcnt = num2;
			double value = DtLines.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["LineVat"]));
			string currency = GlobalData.MainCurrency;
			int num11 = 0;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currency.ToUpper()));
				if (currencyCodesImpl != null)
				{
					num11 = currencyCodesImpl.Decimals;
				}
			}
			calc_VatSum.EditValue = Math.Round(value, (num11 >= 0) ? num11 : 0);
			VatSum = Function.ParseDouble(calc_VatSum.EditValue);
			double num12 = (SubTotal = DtLines.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["LineTotal"])));
			txt_SubTotal.EditValue = num12;
			Total = num12 + VatSum - Discount;
			txt_Total.EditValue = Total;
			_isRecalc = false;
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

	private void calc_VatSum_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void luk_Tax_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			RecalcValues();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Discount_EditValueChanged(object sender, EventArgs e)
	{
		if (!_isDisSumChanging)
		{
			string currency = GlobalData.MainCurrency;
			int num = 0;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currency.ToUpper()));
				if (currencyCodesImpl != null)
				{
					num = currencyCodesImpl.Decimals;
				}
			}
			double num2 = DtLines.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["LineTotal"]));
			DiscPrcnt = Function.ParseDouble(calc_DiscPrcnt.EditValue);
			Discount = Math.Round(num2 * DiscPrcnt / 100.0, (num >= 0) ? num : 0);
			calc_DicsSum.EditValue = Discount;
		}
		else
		{
			Discount = Function.ParseDouble(calc_DicsSum.EditValue);
		}
		RecalcValues();
	}

	private void calc_DiscSum_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isRecalc)
			{
				_isDisSumChanging = true;
				calc_DiscPrcnt.EditValue = Math.Round(Function.ParseDouble(calc_DicsSum.EditValue) / SubTotal * 100.0, GlobalData.GeneralSetting.RateDec.GetValueOrDefault());
				_isDisSumChanging = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void calc_DiscPrcnt_EditValueChanging(object sender, ChangingEventArgs e)
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

	private void calc_DicsSum_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			double num = Function.ParseDouble(e.NewValue) / SubTotal * 100.0;
			if (Function.ParseDouble(num) > 100.0 || Function.ParseDouble(num) < -100.0)
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

	private void calc_DicsSum_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
	{
		if (!(Function.ParseDouble(e.Value) > 100.0))
		{
			Function.ParseDouble(e.Value);
			_ = -100.0;
		}
	}

	private void memo_Remarks_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			Comments = Function.ToString(memo_Remarks.EditValue);
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
		this.labelControlEx3 = new Apzon.Controls.Controls.LabelControlEx();
		this.txt_SubTotal = new Apzon.Forms.UserControls.POS_UI.CalcEditResizable();
		this.ucNumPad1 = new Apzon.Forms.UserControls.POS_UI.ucNumPad();
		this.calc_DiscPrcnt = new Apzon.Forms.UserControls.POS_UI.CalcEditResizable();
		this.txt_Total = new Apzon.Forms.UserControls.POS_UI.CalcEditResizable();
		this.labelControlEx4 = new Apzon.Controls.Controls.LabelControlEx();
		this.labelControlEx5 = new Apzon.Controls.Controls.LabelControlEx();
		this.labelControlEx1 = new Apzon.Controls.Controls.LabelControlEx();
		this.labelControlEx2 = new Apzon.Controls.Controls.LabelControlEx();
		this.lbl_Disc = new Apzon.Controls.Controls.LabelControlEx();
		this.calc_DicsSum = new Apzon.Forms.UserControls.POS_UI.CalcEditResizable();
		this.labelControlEx6 = new Apzon.Controls.Controls.LabelControlEx();
		this.calc_VatSum = new Apzon.Forms.UserControls.POS_UI.CalcEditResizable();
		this.labelControlEx7 = new Apzon.Controls.Controls.LabelControlEx();
		this.luk_Tax = new DevExpress.XtraEditors.GridLookUpEdit();
		this.gridLookUpEdit1View = new Apzon.Forms.UserControls.POS_UI.RetailGrid();
		this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.memo_Remarks = new DevExpress.XtraEditors.MemoEdit();
		((System.ComponentModel.ISupportInitialize)this.txt_SubTotal.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_DiscPrcnt.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Total.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_DicsSum.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_VatSum.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Tax.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridLookUpEdit1View).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Remarks.Properties).BeginInit();
		base.SuspendLayout();
		this.labelControlEx3.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx3.Appearance.Options.UseFont = true;
		this.labelControlEx3.FieldName = "labelControlEx1";
		this.labelControlEx3.Location = new System.Drawing.Point(5, 15);
		this.labelControlEx3.Name = "labelControlEx3";
		this.labelControlEx3.Size = new System.Drawing.Size(72, 18);
		this.labelControlEx3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx3.TabIndex = 30;
		this.labelControlEx3.Text = "Tiền hàng";
		this.txt_SubTotal.FieldName = null;
		this.txt_SubTotal.Location = new System.Drawing.Point(109, 6);
		this.txt_SubTotal.Name = "txt_SubTotal";
		this.txt_SubTotal.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.txt_SubTotal.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 16.77385f);
		this.txt_SubTotal.Properties.Appearance.Options.UseBackColor = true;
		this.txt_SubTotal.Properties.Appearance.Options.UseFont = true;
		this.txt_SubTotal.Properties.AutoHeight = false;
		this.txt_SubTotal.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.txt_SubTotal.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.txt_SubTotal.Properties.NullValuePrompt = "0";
		this.txt_SubTotal.Properties.NullValuePromptShowForEmptyValue = true;
		this.txt_SubTotal.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.txt_SubTotal.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.txt_SubTotal.Size = new System.Drawing.Size(171, 32);
		this.txt_SubTotal.TabIndex = 32;
		this.ucNumPad1.ClientControl = this.calc_DiscPrcnt;
		this.ucNumPad1.Location = new System.Drawing.Point(313, 48);
		this.ucNumPad1.Name = "ucNumPad1";
		this.ucNumPad1.Size = new System.Drawing.Size(300, 280);
		this.ucNumPad1.TabIndex = 33;
		this.calc_DiscPrcnt.FieldName = null;
		this.calc_DiscPrcnt.Location = new System.Drawing.Point(109, 128);
		this.calc_DiscPrcnt.Name = "calc_DiscPrcnt";
		this.calc_DiscPrcnt.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 16.67575f);
		this.calc_DiscPrcnt.Properties.Appearance.Options.UseFont = true;
		this.calc_DiscPrcnt.Properties.AutoHeight = false;
		this.calc_DiscPrcnt.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Rate;
		this.calc_DiscPrcnt.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calc_DiscPrcnt.Properties.NullValuePromptShowForEmptyValue = true;
		this.calc_DiscPrcnt.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.calc_DiscPrcnt.Size = new System.Drawing.Size(170, 32);
		this.calc_DiscPrcnt.TabIndex = 0;
		this.calc_DiscPrcnt.EditValueChanged += new System.EventHandler(this.txt_Discount_EditValueChanged);
		this.calc_DiscPrcnt.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.calc_DiscPrcnt_EditValueChanging);
		this.txt_Total.FieldName = null;
		this.txt_Total.Location = new System.Drawing.Point(109, 209);
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
		this.labelControlEx4.Location = new System.Drawing.Point(7, 223);
		this.labelControlEx4.Name = "labelControlEx4";
		this.labelControlEx4.Size = new System.Drawing.Size(69, 18);
		this.labelControlEx4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx4.TabIndex = 34;
		this.labelControlEx4.Text = "Tổng tiền";
		this.labelControlEx5.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx5.Appearance.Options.UseFont = true;
		this.labelControlEx5.FieldName = "labelControlEx1";
		this.labelControlEx5.Location = new System.Drawing.Point(5, 138);
		this.labelControlEx5.Name = "labelControlEx5";
		this.labelControlEx5.Size = new System.Drawing.Size(91, 18);
		this.labelControlEx5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx5.TabIndex = 36;
		this.labelControlEx5.Text = "C. khấu (%)";
		this.labelControlEx1.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx1.Appearance.Options.UseFont = true;
		this.labelControlEx1.FieldName = "labelControlEx1";
		this.labelControlEx1.Location = new System.Drawing.Point(5, 56);
		this.labelControlEx1.Name = "labelControlEx1";
		this.labelControlEx1.Size = new System.Drawing.Size(65, 18);
		this.labelControlEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx1.TabIndex = 40;
		this.labelControlEx1.Text = "VAT (%)";
		this.labelControlEx2.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx2.Appearance.Options.UseFont = true;
		this.labelControlEx2.FieldName = "labelControlEx1";
		this.labelControlEx2.Location = new System.Drawing.Point(289, 56);
		this.labelControlEx2.Name = "labelControlEx2";
		this.labelControlEx2.Size = new System.Drawing.Size(18, 18);
		this.labelControlEx2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx2.TabIndex = 42;
		this.labelControlEx2.Text = "%";
		this.lbl_Disc.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_Disc.Appearance.Options.UseFont = true;
		this.lbl_Disc.FieldName = "labelControlEx1";
		this.lbl_Disc.Location = new System.Drawing.Point(289, 138);
		this.lbl_Disc.Name = "lbl_Disc";
		this.lbl_Disc.Size = new System.Drawing.Size(18, 18);
		this.lbl_Disc.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Disc.TabIndex = 43;
		this.lbl_Disc.Text = "%";
		this.calc_DicsSum.FieldName = null;
		this.calc_DicsSum.Location = new System.Drawing.Point(109, 171);
		this.calc_DicsSum.Name = "calc_DicsSum";
		this.calc_DicsSum.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 16.67575f);
		this.calc_DicsSum.Properties.Appearance.Options.UseFont = true;
		this.calc_DicsSum.Properties.AutoHeight = false;
		this.calc_DicsSum.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.calc_DicsSum.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calc_DicsSum.Properties.NullValuePrompt = "0";
		this.calc_DicsSum.Properties.NullValuePromptShowForEmptyValue = true;
		this.calc_DicsSum.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.calc_DicsSum.Size = new System.Drawing.Size(170, 32);
		this.calc_DicsSum.TabIndex = 44;
		this.calc_DicsSum.InvalidValue += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.calc_DicsSum_InvalidValue);
		this.calc_DicsSum.EditValueChanged += new System.EventHandler(this.calc_DiscSum_EditValueChanged);
		this.calc_DicsSum.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.calc_DicsSum_EditValueChanging);
		this.labelControlEx6.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx6.Appearance.Options.UseFont = true;
		this.labelControlEx6.FieldName = "labelControlEx1";
		this.labelControlEx6.Location = new System.Drawing.Point(5, 181);
		this.labelControlEx6.Name = "labelControlEx6";
		this.labelControlEx6.Size = new System.Drawing.Size(87, 18);
		this.labelControlEx6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx6.TabIndex = 45;
		this.labelControlEx6.Text = "Tiền C.khấu";
		this.calc_VatSum.FieldName = null;
		this.calc_VatSum.Location = new System.Drawing.Point(109, 86);
		this.calc_VatSum.Name = "calc_VatSum";
		this.calc_VatSum.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 16.67575f);
		this.calc_VatSum.Properties.Appearance.Options.UseFont = true;
		this.calc_VatSum.Properties.AutoHeight = false;
		this.calc_VatSum.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.calc_VatSum.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calc_VatSum.Properties.NullValuePrompt = "0";
		this.calc_VatSum.Properties.NullValuePromptShowForEmptyValue = true;
		this.calc_VatSum.Properties.ReadOnly = true;
		this.calc_VatSum.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.calc_VatSum.Size = new System.Drawing.Size(170, 32);
		this.calc_VatSum.TabIndex = 47;
		this.calc_VatSum.EditValueChanged += new System.EventHandler(this.calc_VatSum_EditValueChanged);
		this.labelControlEx7.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx7.Appearance.Options.UseFont = true;
		this.labelControlEx7.FieldName = "labelControlEx1";
		this.labelControlEx7.Location = new System.Drawing.Point(5, 96);
		this.labelControlEx7.Name = "labelControlEx7";
		this.labelControlEx7.Size = new System.Drawing.Size(69, 18);
		this.labelControlEx7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx7.TabIndex = 46;
		this.labelControlEx7.Text = "Tổng VAT";
		this.luk_Tax.Location = new System.Drawing.Point(108, 45);
		this.luk_Tax.Name = "luk_Tax";
		this.luk_Tax.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 17f);
		this.luk_Tax.Properties.Appearance.Options.UseFont = true;
		this.luk_Tax.Properties.Appearance.Options.UseTextOptions = true;
		this.luk_Tax.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.luk_Tax.Properties.NullText = "";
		this.luk_Tax.Properties.View = this.gridLookUpEdit1View;
		this.luk_Tax.Properties.EditValueChanged += new System.EventHandler(this.luk_Tax_EditValueChanged);
		this.luk_Tax.Size = new System.Drawing.Size(171, 34);
		this.luk_Tax.TabIndex = 54;
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
		this.gridLookUpEdit1View.ItemId = "OEDITVIEW";
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
		this.memo_Remarks.EditValue = "";
		this.memo_Remarks.Location = new System.Drawing.Point(4, 247);
		this.memo_Remarks.Name = "memo_Remarks";
		this.memo_Remarks.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.memo_Remarks.Properties.Appearance.Options.UseFont = true;
		this.memo_Remarks.Properties.NullValuePrompt = "Ghi chú đơn hàng";
		this.memo_Remarks.Properties.NullValuePromptShowForEmptyValue = true;
		this.memo_Remarks.Size = new System.Drawing.Size(276, 87);
		this.memo_Remarks.TabIndex = 55;
		this.memo_Remarks.EditValueChanged += new System.EventHandler(this.memo_Remarks_EditValueChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.memo_Remarks);
		base.Controls.Add(this.luk_Tax);
		base.Controls.Add(this.calc_VatSum);
		base.Controls.Add(this.labelControlEx7);
		base.Controls.Add(this.calc_DicsSum);
		base.Controls.Add(this.labelControlEx6);
		base.Controls.Add(this.lbl_Disc);
		base.Controls.Add(this.labelControlEx2);
		base.Controls.Add(this.labelControlEx1);
		base.Controls.Add(this.calc_DiscPrcnt);
		base.Controls.Add(this.labelControlEx5);
		base.Controls.Add(this.txt_Total);
		base.Controls.Add(this.labelControlEx4);
		base.Controls.Add(this.ucNumPad1);
		base.Controls.Add(this.txt_SubTotal);
		base.Controls.Add(this.labelControlEx3);
		base.Name = "ucEditOrder";
		base.Size = new System.Drawing.Size(616, 339);
		((System.ComponentModel.ISupportInitialize)this.txt_SubTotal.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_DiscPrcnt.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Total.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_DicsSum.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_VatSum.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Tax.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridLookUpEdit1View).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Remarks.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
