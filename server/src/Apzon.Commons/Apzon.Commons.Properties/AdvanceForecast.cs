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
internal class AdvanceForecast
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
				resourceMan = new ResourceManager("Apzon.Commons.Properties.AdvanceForecast", typeof(AdvanceForecast).Assembly);
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

	internal static string EndDate => ResourceManager.GetString("EndDate", resourceCulture);

	internal static string FormView => ResourceManager.GetString("FormView", resourceCulture);

	internal static string Name => ResourceManager.GetString("Name", resourceCulture);

	internal static string OriginType => ResourceManager.GetString("OriginType", resourceCulture);

	internal static string PriceList => ResourceManager.GetString("PriceList", resourceCulture);

	internal static string SplCode => ResourceManager.GetString("SplCode", resourceCulture);

	internal static string StartDate => ResourceManager.GetString("StartDate", resourceCulture);

	internal AdvanceForecast()
	{
	}
}
