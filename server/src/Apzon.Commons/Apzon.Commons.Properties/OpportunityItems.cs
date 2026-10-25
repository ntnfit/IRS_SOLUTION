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
public class OpportunityItems
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
				resourceMan = new ResourceManager("Apzon.Commons.Properties.OpportunityItems", typeof(OpportunityItems).Assembly);
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

	public static string AttFile => ResourceManager.GetString("AttFile", resourceCulture);

	public static string AttFullPath => ResourceManager.GetString("AttFullPath", resourceCulture);

	public static string DeliveryDate => ResourceManager.GetString("DeliveryDate", resourceCulture);

	public static string FrgnName => ResourceManager.GetString("FrgnName", resourceCulture);

	public static string IPercent => ResourceManager.GetString("IPercent", resourceCulture);

	public static string ItemCode => ResourceManager.GetString("ItemCode", resourceCulture);

	public static string ItemName => ResourceManager.GetString("ItemName", resourceCulture);

	public static string Line => ResourceManager.GetString("Line", resourceCulture);

	public static string OpportId => ResourceManager.GetString("OpportId", resourceCulture);

	public static string PotentAmount => ResourceManager.GetString("PotentAmount", resourceCulture);

	public static string Price => ResourceManager.GetString("Price", resourceCulture);

	public static string Quantity => ResourceManager.GetString("Quantity", resourceCulture);

	internal OpportunityItems()
	{
	}
}
