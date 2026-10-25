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
public class Item_Prices
{
	private static ResourceManager resourceMan;

	private static CultureInfo resourceCulture;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static ResourceManager ResourceManager
	{
		get
		{
			if (resourceMan == null)
			{
				resourceMan = new ResourceManager("Apzon.Commons.Properties.Item_Prices", typeof(Item_Prices).Assembly);
			}
			return resourceMan;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public static CultureInfo Culture
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

	public static string AddPrice1 => ResourceManager.GetString("AddPrice1", resourceCulture);

	public static string AddPrice2 => ResourceManager.GetString("AddPrice2", resourceCulture);

	public static string Currency => ResourceManager.GetString("Currency", resourceCulture);

	public static string Currency1 => ResourceManager.GetString("Currency1", resourceCulture);

	public static string Currency2 => ResourceManager.GetString("Currency2", resourceCulture);

	public static string Factor => ResourceManager.GetString("Factor", resourceCulture);

	public static string ItemCode => ResourceManager.GetString("ItemCode", resourceCulture);

	public static string Ovrwrite1 => ResourceManager.GetString("Ovrwrite1", resourceCulture);

	public static string Ovrwrite2 => ResourceManager.GetString("Ovrwrite2", resourceCulture);

	public static string Ovrwritten => ResourceManager.GetString("Ovrwritten", resourceCulture);

	public static string Price => ResourceManager.GetString("Price", resourceCulture);

	public static string PriceList => ResourceManager.GetString("PriceList", resourceCulture);

	internal Item_Prices()
	{
	}
}
