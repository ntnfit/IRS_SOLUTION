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
public class VatGroupLines
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
				resourceMan = new ResourceManager("Apzon.Commons.Properties.VatGroupLines", typeof(VatGroupLines).Assembly);
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

	public static string DatevCode => ResourceManager.GetString("DatevCode", resourceCulture);

	public static string EffecDate => ResourceManager.GetString("EffecDate", resourceCulture);

	public static string EquVatPr => ResourceManager.GetString("EquVatPr", resourceCulture);

	public static string FixedAmout => ResourceManager.GetString("FixedAmout", resourceCulture);

	public static string MinAmount => ResourceManager.GetString("MinAmount", resourceCulture);

	public static string Rate => ResourceManager.GetString("Rate", resourceCulture);

	public static string TaxType => ResourceManager.GetString("TaxType", resourceCulture);

	internal VatGroupLines()
	{
	}
}
