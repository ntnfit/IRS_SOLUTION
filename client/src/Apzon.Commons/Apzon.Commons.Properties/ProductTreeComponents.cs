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
public class ProductTreeComponents
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
				resourceMan = new ResourceManager("Apzon.Commons.Properties.ProductTreeComponents", typeof(ProductTreeComponents).Assembly);
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

	public static string AddQuantit => ResourceManager.GetString("AddQuantit", resourceCulture);

	public static string ChildNum => ResourceManager.GetString("ChildNum", resourceCulture);

	public static string Code => ResourceManager.GetString("Code", resourceCulture);

	public static string Comment => ResourceManager.GetString("Comment", resourceCulture);

	public static string Currency => ResourceManager.GetString("Currency", resourceCulture);

	public static string Father => ResourceManager.GetString("Father", resourceCulture);

	public static string IssueMthd => ResourceManager.GetString("IssueMthd", resourceCulture);

	public static string LineText => ResourceManager.GetString("LineText", resourceCulture);

	public static string OcrCode => ResourceManager.GetString("OcrCode", resourceCulture);

	public static string OcrCode2 => ResourceManager.GetString("OcrCode2", resourceCulture);

	public static string OcrCode3 => ResourceManager.GetString("OcrCode3", resourceCulture);

	public static string OcrCode4 => ResourceManager.GetString("OcrCode4", resourceCulture);

	public static string OcrCode5 => ResourceManager.GetString("OcrCode5", resourceCulture);

	public static string OrigCurr => ResourceManager.GetString("OrigCurr", resourceCulture);

	public static string OrigPrice => ResourceManager.GetString("OrigPrice", resourceCulture);

	public static string Price => ResourceManager.GetString("Price", resourceCulture);

	public static string PriceList => ResourceManager.GetString("PriceList", resourceCulture);

	public static string PrncpInput => ResourceManager.GetString("PrncpInput", resourceCulture);

	public static string Project => ResourceManager.GetString("Project", resourceCulture);

	public static string Quantity => ResourceManager.GetString("Quantity", resourceCulture);

	public static string Type => ResourceManager.GetString("Type", resourceCulture);

	public static string Uom => ResourceManager.GetString("Uom", resourceCulture);

	public static string Warehouse => ResourceManager.GetString("Warehouse", resourceCulture);

	public static string WipActCode => ResourceManager.GetString("WipActCode", resourceCulture);

	internal ProductTreeComponents()
	{
	}
}
