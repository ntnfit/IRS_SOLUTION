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
public class ProductTree
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
				resourceMan = new ResourceManager("Apzon.Commons.Properties.ProductTree", typeof(ProductTree).Assembly);
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

	public static string Code => ResourceManager.GetString("Code", resourceCulture);

	public static string DispCurr => ResourceManager.GetString("DispCurr", resourceCulture);

	public static string HideComp => ResourceManager.GetString("HideComp", resourceCulture);

	public static string OcrCode => ResourceManager.GetString("OcrCode", resourceCulture);

	public static string OcrCode2 => ResourceManager.GetString("OcrCode2", resourceCulture);

	public static string OcrCode3 => ResourceManager.GetString("OcrCode3", resourceCulture);

	public static string OcrCode4 => ResourceManager.GetString("OcrCode4", resourceCulture);

	public static string OcrCode5 => ResourceManager.GetString("OcrCode5", resourceCulture);

	public static string PriceList => ResourceManager.GetString("PriceList", resourceCulture);

	public static string Project => ResourceManager.GetString("Project", resourceCulture);

	public static string Qauntity => ResourceManager.GetString("Qauntity", resourceCulture);

	public static string SCNCounter => ResourceManager.GetString("SCNCounter", resourceCulture);

	public static string ToWH => ResourceManager.GetString("ToWH", resourceCulture);

	public static string TreeType => ResourceManager.GetString("TreeType", resourceCulture);

	public static string UseFthrWhs => ResourceManager.GetString("UseFthrWhs", resourceCulture);

	internal ProductTree()
	{
	}
}
