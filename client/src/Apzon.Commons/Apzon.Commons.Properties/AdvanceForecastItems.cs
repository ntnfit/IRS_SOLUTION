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
internal class AdvanceForecastItems
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
				resourceMan = new ResourceManager("Apzon.Commons.Properties.AdvanceForecastItems", typeof(AdvanceForecastItems).Assembly);
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

	internal static string BPCode => ResourceManager.GetString("BPCode", resourceCulture);

	internal static string Code => ResourceManager.GetString("Code", resourceCulture);

	internal static string Date => ResourceManager.GetString("Date", resourceCulture);

	internal static string ItemCode => ResourceManager.GetString("ItemCode", resourceCulture);

	internal static string ItemName => ResourceManager.GetString("ItemName", resourceCulture);

	internal static string LineID => ResourceManager.GetString("LineID", resourceCulture);

	internal static string Price => ResourceManager.GetString("Price", resourceCulture);

	internal static string Quantity => ResourceManager.GetString("Quantity", resourceCulture);

	internal static string SplCode => ResourceManager.GetString("SplCode", resourceCulture);

	internal static string SubCatNum => ResourceManager.GetString("SubCatNum", resourceCulture);

	internal static string WhsCode => ResourceManager.GetString("WhsCode", resourceCulture);

	internal AdvanceForecastItems()
	{
	}
}
