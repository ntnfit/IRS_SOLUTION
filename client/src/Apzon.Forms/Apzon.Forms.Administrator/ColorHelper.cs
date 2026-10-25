using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;

namespace Apzon.Forms.Administrator;

public class ColorHelper
{
	public static string HtmlHyperLinkTextColor => GetRgbColor(EditorsSkins.GetSkin(UserLookAndFeel.Default).Colors.GetColor("HyperLinkTextColor"));

	public static string HtmlHighlightTextColor => GetRgbColor(CommonSkins.GetSkin(UserLookAndFeel.Default).Colors.GetColor("HighlightText"));

	public static string HtmlControlTextColor => GetRgbColor(CommonSkins.GetSkin(UserLookAndFeel.Default).Colors.GetColor("ControlText"));

	public static string HtmlControlTextColor2 => GetRgbColor(CommonSkins.GetSkin(UserLookAndFeel.Default).Colors.GetColor("WindowText"));

	public static string HtmlQuestionColor => GetRgbColor(CommonColors.GetQuestionColor(UserLookAndFeel.Default));

	public static string HtmlWarningColor => GetRgbColor(CommonColors.GetWarningColor(UserLookAndFeel.Default));

	public static Color DisabledTextColor => CommonSkins.GetSkin(UserLookAndFeel.Default).Colors.GetColor("DisabledText");

	public static Color CriticalColor => CommonColors.GetCriticalColor(UserLookAndFeel.Default);

	public static Color WarningColor => CommonColors.GetWarningColor(UserLookAndFeel.Default);

	public static void UpdateColor(ImageList list, UserLookAndFeel lf)
	{
		for (int i = 0; i < list.Images.Count; i++)
		{
			list.Images[i] = SetColor(list.Images[i] as Bitmap, GetHeaderForeColor(lf));
		}
	}

	public static Color GetHeaderForeColor(UserLookAndFeel lf)
	{
		Color controlText = SystemColors.ControlText;
		if (lf.ActiveStyle != ActiveLookAndFeelStyle.Skin)
		{
			return controlText;
		}
		return GridSkins.GetSkin(lf)[GridSkins.SkinHeader].Color.GetForeColor();
	}

	private static Bitmap SetColor(Bitmap bmp, Color color)
	{
		for (int i = 0; i < bmp.Width; i++)
		{
			for (int j = 0; j < bmp.Height; j++)
			{
				if (bmp.GetPixel(i, j).Name != "0")
				{
					bmp.SetPixel(i, j, color);
				}
			}
		}
		return bmp;
	}

	private static string GetRgbColor(Color color)
	{
		return $"{color.R},{color.G},{color.B}";
	}
}
