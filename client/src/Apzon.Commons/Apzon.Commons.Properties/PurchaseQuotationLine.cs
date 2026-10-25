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
public class PurchaseQuotationLine
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
				resourceMan = new ResourceManager("Apzon.Commons.Properties.PurchaseQuotationLine", typeof(PurchaseQuotationLine).Assembly);
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

	public static string PQTReqDate => ResourceManager.GetString("PQTReqDate", resourceCulture);

	public static string PQTReqQty => ResourceManager.GetString("PQTReqQty", resourceCulture);

	public static string Quantity => ResourceManager.GetString("Quantity", resourceCulture);

	public static string ShipDate => ResourceManager.GetString("ShipDate", resourceCulture);

	internal PurchaseQuotationLine()
	{
	}
}
