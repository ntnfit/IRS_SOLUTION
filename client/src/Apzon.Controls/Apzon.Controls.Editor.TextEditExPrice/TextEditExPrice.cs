using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls.Form.ExchangeRateAndIndex;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Controls.Editor.TextEditExPrice;

[ToolboxItem(true)]
public class TextEditExPrice : TextEditEx
{
	private string _currency = "";

	public string Currency
	{
		get
		{
			return _currency;
		}
		set
		{
			_currency = value;
			if (!DesignMode && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
			{
				Refresh();
			}
		}
	}

	[DefaultValue(Enums.DisplayFormatType.Price)]
	public Enums.DisplayFormatType DisplayFormatType { get; set; }

	[DefaultValue(true)]
	public bool CheckExChangeRate { get; set; }

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public new RepositoryItemTextEditExPrice Properties => base.Properties as RepositoryItemTextEditExPrice;

	public override string EditorTypeName => "TextEditExPrice";

	static TextEditExPrice()
	{
		RepositoryItemTextEditExPrice.RegisterTextEditExPrice();
	}

	public TextEditExPrice()
	{
		Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
		CustomDisplayText += CustomDisplayTextProcess;
		KeyPress += KeyDownProcess;
		DisplayFormatType = Enums.DisplayFormatType.Price;
		CheckExChangeRate = true;
		if (!DesignMode && LicenseManager.UsageMode != LicenseUsageMode.Designtime)
		{
			Currency = GlobalData.MainCurrency;
		}
	}

	protected override void OnValidating(CancelEventArgs e)
	{
		if (IsUserChangeValue)
		{
			Validating(this, e);
		}
		if (e.Cancel)
		{
			ErrorText = "Invalid currency";
		}
		else
		{
			ErrorText = "";
		}
	}

	protected new void Validating(object sender, CancelEventArgs e)
	{
		try
		{
			string[] dataCurrency = Function.ToString(EditValue).GetDataCurrency();
			if (dataCurrency == null || dataCurrency.Length == 0)
			{
				return;
			}
			decimal d = Function.ParseDecimal(dataCurrency[0]);
			string alpha = Function.ToString(dataCurrency[1]);
			Currency = ((!string.IsNullOrEmpty(alpha)) ? alpha.ToUpper() : GlobalData.MainCurrency);
			if (CheckExChangeRate && alpha.ToUpper() != GlobalData.MainCurrency.ToUpper() && !string.IsNullOrEmpty(alpha))
			{
				DateTime today = DateTime.Today;
				if (HttpClientHelperFactory.GetHttpClient().ExRateAndIndex.GetExchangeRate(alpha, today) <= 0.0)
				{
					frm_ExchangeRatesAndIndexs frm_ExchangeRatesAndIndexs2 = new frm_ExchangeRatesAndIndexs();
					frm_ExchangeRatesAndIndexs2.Curr = alpha;
					frm_ExchangeRatesAndIndexs2._date = today;
					frm_ExchangeRatesAndIndexs2.ShowDialog();
				}
			}
			int num = -1;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(alpha.ToUpper()));
				if (currencyCodesImpl != null)
				{
					num = currencyCodesImpl.Decimals;
				}
			}
			d = Math.Round(d, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
			EditValue = d;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override void OnEditorEnter(EventArgs e)
	{
	}

	private void CustomDisplayTextProcess(object sender, CustomDisplayTextEventArgs e)
	{
		try
		{
			if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
			{
				return;
			}
			if (string.IsNullOrEmpty(Currency))
			{
				Currency = GlobalData.MainCurrency;
			}
			int num = -1;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(Currency.ToUpper()));
				if (currencyCodesImpl != null)
				{
					num = currencyCodesImpl.Decimals;
				}
			}
			if (DisplayFormatType == Enums.DisplayFormatType.Price)
			{
				string text = new string('#', (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
				e.DisplayText = string.Format("{0:#,0." + text + "} {1}", e.Value, Currency);
			}
			if (DisplayFormatType == Enums.DisplayFormatType.Amount)
			{
				string text2 = new string('#', (num == -1) ? GlobalData.GeneralSetting.SumDec.GetValueOrDefault() : num);
				e.DisplayText = string.Format("{0:#,0." + text2 + "} {1}", e.Value, Currency);
			}
			if (Function.ParseDouble(e.Value) == 0.0)
			{
				e.DisplayText = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void KeyDownProcess(object sender, KeyPressEventArgs e)
	{
		try
		{
			if (e.KeyChar == '.' || e.KeyChar == ',')
			{
				e.KeyChar = Function.ParseChar(GlobalData.GeneralSetting.DecSep) ?? e.KeyChar;
				e.Handled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}
}
