using System.ComponentModel;
using System.Drawing;
using Apzon.Commons.Enums;
using Apzon.Global;
using DevExpress.Utils;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;

namespace Apzon.Controls.Editor;

[UserRepositoryItem("RegisterAmountEditEx")]
public class RepositoryItemCalcEditEx : RepositoryItemCalcEdit
{
	private Enums.DisplayFormatType _displayFormatType;

	public bool IsRounding;

	private int _numOfDecs;

	public const string CustomEditName = "AmountEditEx";

	public bool IsSearchControl { get; set; }

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
					DisplayFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.PercentDec ?? 6)}";
					_numOfDecs = GlobalData.GeneralSetting.PercentDec ?? 6;
					break;
				case Enums.DisplayFormatType.Qty:
					DisplayFormat.FormatType = FormatType.Numeric;
					DisplayFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.QtyDec ?? 6)}";
					_numOfDecs = GlobalData.GeneralSetting.QtyDec ?? 6;
					break;
				case Enums.DisplayFormatType.Rate:
					DisplayFormat.FormatType = FormatType.Numeric;
					DisplayFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.RateDec ?? 6)}";
					_numOfDecs = GlobalData.GeneralSetting.RateDec ?? 6;
					break;
				case Enums.DisplayFormatType.Unit:
					DisplayFormat.FormatType = FormatType.Numeric;
					DisplayFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.QtyDec ?? 6)}";
					_numOfDecs = GlobalData.GeneralSetting.QtyDec ?? 6;
					break;
				case Enums.DisplayFormatType.Amount:
					DisplayFormat.FormatType = FormatType.Numeric;
					DisplayFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.SumDec ?? 6)}";
					_numOfDecs = GlobalData.GeneralSetting.SumDec ?? 6;
					break;
				case Enums.DisplayFormatType.Price:
					DisplayFormat.FormatType = FormatType.Numeric;
					DisplayFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.PriceDec ?? 6)}";
					_numOfDecs = GlobalData.GeneralSetting.PriceDec ?? 6;
					break;
				case Enums.DisplayFormatType.Time:
					DisplayFormat.FormatString = "##:##";
					DisplayFormat.FormatType = FormatType.Custom;
					break;
				case Enums.DisplayFormatType.Integer:
					DisplayFormat.FormatType = FormatType.Numeric;
					DisplayFormat.FormatString = "n0";
					break;
				}
			}
		}
	}

	public int NumOfDecs => _numOfDecs;

	public override string EditorTypeName => "AmountEditEx";

	static RepositoryItemCalcEditEx()
	{
		RegisterAmountEditEx();
	}

	public static void RegisterAmountEditEx()
	{
		Image image = null;
		EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("AmountEditEx", typeof(CalcEditEx), typeof(RepositoryItemCalcEditEx), typeof(AmountEditExViewInfo), new AmountEditExPainter(), designTimeVisible: true, image));
	}

	public override void Assign(RepositoryItem item)
	{
		BeginUpdate();
		try
		{
			base.Assign(item);
			_ = item is RepositoryItemCalcEditEx;
		}
		finally
		{
			EndUpdate();
		}
	}
}
