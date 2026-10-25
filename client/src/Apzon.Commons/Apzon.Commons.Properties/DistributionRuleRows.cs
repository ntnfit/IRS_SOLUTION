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
public class DistributionRuleRows
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
				resourceMan = new ResourceManager("Apzon.Commons.Properties.DistributionRuleRows", typeof(DistributionRuleRows).Assembly);
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

	public static string Direct => ResourceManager.GetString("Direct", resourceCulture);

	public static string logInstanc => ResourceManager.GetString("logInstanc", resourceCulture);

	public static string OcrCode => ResourceManager.GetString("OcrCode", resourceCulture);

	public static string OcrTotal => ResourceManager.GetString("OcrTotal", resourceCulture);

	public static string PrcAmount => ResourceManager.GetString("PrcAmount", resourceCulture);

	public static string PrcCode => ResourceManager.GetString("PrcCode", resourceCulture);

	public static string updateDate => ResourceManager.GetString("updateDate", resourceCulture);

	public static string UserSign => ResourceManager.GetString("UserSign", resourceCulture);

	public static string UserSign2 => ResourceManager.GetString("UserSign2", resourceCulture);

	public static string ValidFrom => ResourceManager.GetString("ValidFrom", resourceCulture);

	public static string ValidTo => ResourceManager.GetString("ValidTo", resourceCulture);

	internal DistributionRuleRows()
	{
	}
}
