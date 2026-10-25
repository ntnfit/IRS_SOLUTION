using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Skins;

namespace Apzon.Forms.Commons.Helper;

internal static class ColorHelper
{
	public static Color TextColor => CommonSkins.GetSkin(UserLookAndFeel.Default).Colors.GetColor(CommonColors.ControlText);

	public static Color DisabledTextColor => CommonSkins.GetSkin(UserLookAndFeel.Default).Colors.GetColor(CommonColors.DisabledText);

	public static Color CriticalColor => CommonColors.GetCriticalColor(UserLookAndFeel.Default);

	public static Color WarningColor => CommonColors.GetWarningColor(UserLookAndFeel.Default);

	public static Color QuestionColor => CommonColors.GetQuestionColor(UserLookAndFeel.Default);

	public static Color InformationColor => CommonColors.GetInformationColor(UserLookAndFeel.Default);

	public static Color GetControlColor(UserLookAndFeel provider)
	{
		return LookAndFeelHelper.GetSystemColor(provider, SystemColors.Control);
	}
}
