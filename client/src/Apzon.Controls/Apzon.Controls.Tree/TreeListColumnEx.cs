using System.ComponentModel;
using Apzon.Commons.Enums;
using Apzon.Global;
using DevExpress.Utils;
using DevExpress.XtraTreeList.Columns;

namespace Apzon.Controls.Tree;

public class TreeListColumnEx : TreeListColumn
{
	private Enums.DisplayFormatType _displayFormatType;

	[DefaultValue(Enums.DisplayFormatType.None)]
	public Enums.DisplayFormatType DisplayFormatType
	{
		get
		{
			return _displayFormatType;
		}
		set
		{
			_displayFormatType = value;
			if (GlobalData.GeneralSetting != null)
			{
				switch (value)
				{
				case Enums.DisplayFormatType.DateTime:
					Format.FormatType = FormatType.DateTime;
					Format.FormatString = GlobalData.DateTimeFormat;
					break;
				case Enums.DisplayFormatType.Percent:
					Format.FormatType = FormatType.Numeric;
					Format.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.PercentDec ?? 6)} %";
					break;
				case Enums.DisplayFormatType.Qty:
					Format.FormatType = FormatType.Numeric;
					Format.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.QtyDec ?? 6)}";
					break;
				case Enums.DisplayFormatType.Rate:
					Format.FormatType = FormatType.Numeric;
					Format.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.RateDec ?? 6)}";
					break;
				case Enums.DisplayFormatType.Unit:
					Format.FormatType = FormatType.Numeric;
					Format.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.QtyDec ?? 6)}";
					break;
				case Enums.DisplayFormatType.Time:
					Format.FormatString = "##:##";
					Format.FormatType = FormatType.Custom;
					break;
				case Enums.DisplayFormatType.Integer:
					Format.FormatType = FormatType.Numeric;
					Format.FormatString = "n0";
					break;
				case Enums.DisplayFormatType.Price:
					Format.FormatType = FormatType.Numeric;
					Format.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.PriceDec ?? 6)}";
					break;
				case Enums.DisplayFormatType.Amount:
					Format.FormatType = FormatType.Numeric;
					Format.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.SumDec ?? 6)}";
					break;
				}
			}
		}
	}

	[DefaultValue(Enums.CurrencyColumn.None)]
	public Enums.CurrencyColumn Currency { get; set; }

	public string CurrencyFixed { get; set; }

	public double CurrencyFixedRate { get; set; }

	public string CurrencyField { get; set; }

	public string CurrencyRateField { get; set; }

	public string CurrencyDateField { get; set; }

	[DefaultValue(false)]
	public bool PreventChangeSetting { get; set; }
}
