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
public class DistributionRule
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
				resourceMan = new ResourceManager("Apzon.Commons.Properties.DistributionRule", typeof(DistributionRule).Assembly);
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

	public static string AbsEntry => ResourceManager.GetString("AbsEntry", resourceCulture);

	public static string Active => ResourceManager.GetString("Active", resourceCulture);

	public static string DimCode => ResourceManager.GetString("DimCode", resourceCulture);

	public static string Direct => ResourceManager.GetString("Direct", resourceCulture);

	public static string IsFixedAmt => ResourceManager.GetString("IsFixedAmt", resourceCulture);

	public static string Locked => ResourceManager.GetString("Locked", resourceCulture);

	public static string OcrCode => ResourceManager.GetString("OcrCode", resourceCulture);

	public static string OcrName => ResourceManager.GetString("OcrName", resourceCulture);

	public static string OcrTotal => ResourceManager.GetString("OcrTotal", resourceCulture);

	internal DistributionRule()
	{
	}
}
