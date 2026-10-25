using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Apzon.Commons.Properties;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
internal class MessageBox
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				resourceMan = new ResourceManager("Apzon.Commons.Properties.MessageBox", typeof(MessageBox).Assembly);
			}
			return resourceMan;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture
	{
		get
		{
			return resourceCulture;
		}
		set
		{
			resourceCulture = value;
		}
	}

	internal static string CaptionSystem => ResourceManager.GetString("CaptionSystem", resourceCulture);

	internal static string CloseForm => ResourceManager.GetString("CloseForm", resourceCulture);

	internal static string RemoveDocNum => ResourceManager.GetString("RemoveDocNum", resourceCulture);

	internal static string RemoveItem => ResourceManager.GetString("RemoveItem", resourceCulture);

	internal static string RemoveNo => ResourceManager.GetString("RemoveNo", resourceCulture);

	internal static string RemoveRow => ResourceManager.GetString("RemoveRow", resourceCulture);

	internal static string SaveChange => ResourceManager.GetString("SaveChange", resourceCulture);

	internal static string Unsaved => ResourceManager.GetString("Unsaved", resourceCulture);

	internal MessageBox()
	{
	}
}
