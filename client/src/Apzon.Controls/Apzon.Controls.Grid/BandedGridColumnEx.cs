using System.ComponentModel;
using Apzon.Commons.Enums;
using Apzon.Global;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.BandedGrid;

namespace Apzon.Controls.Grid;

public class BandedGridColumnEx : BandedGridColumn
{
	private Enums.DisplayFormatType _displayFormatType;

	[DefaultValue(Enums.CurrencyColumn.None)]
	public Enums.CurrencyColumn Currency { get; set; }

	public string CurrencyFixed { get; set; }

	public double CurrencyFixedRate { get; set; }

	public string CurrencyField { get; set; }

	public string CurrencyRateField { get; set; }

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
					DisplayFormat.FormatType = FormatType.DateTime;
					DisplayFormat.FormatString = GlobalData.DateTimeFormat;
					break;
				case Enums.DisplayFormatType.Percent:
					DisplayFormat.FormatType = FormatType.Numeric;
					DisplayFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.PercentDec ?? 6)}\\%";
					break;
				case Enums.DisplayFormatType.Qty:
					DisplayFormat.FormatType = FormatType.Numeric;
					DisplayFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.QtyDec ?? 6)}";
					break;
				case Enums.DisplayFormatType.Rate:
					DisplayFormat.FormatType = FormatType.Numeric;
					DisplayFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.RateDec ?? 6)}";
					break;
				case Enums.DisplayFormatType.Unit:
					DisplayFormat.FormatType = FormatType.Numeric;
					DisplayFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.QtyDec ?? 6)}";
					break;
				case Enums.DisplayFormatType.Time:
					DisplayFormat.FormatString = "##:##";
					DisplayFormat.FormatType = FormatType.Custom;
					break;
				case Enums.DisplayFormatType.Integer:
					DisplayFormat.FormatType = FormatType.Numeric;
					DisplayFormat.FormatString = "n0";
					break;
				case Enums.DisplayFormatType.Price:
				case Enums.DisplayFormatType.Amount:
					break;
				}
			}
		}
	}

	[DefaultValue(true)]
	public bool AllowChSet { get; set; }

	public BandedGridColumnEx()
	{
		AllowChSet = true;
	}
}
