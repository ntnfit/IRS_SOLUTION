using System.Drawing;
using DevExpress.Utils;

namespace Apzon.Forms.Commons.Helper;

internal static class FontResources
{
	public static Font StrikeoutFont = new Font(AppearanceObject.DefaultFont, FontStyle.Strikeout);

	public static Font BoldFont = new Font(AppearanceObject.DefaultFont, FontStyle.Bold);
}
