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
public class BPAddresses
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
				resourceMan = new ResourceManager("Apzon.Commons.Properties.BPAddresses", typeof(BPAddresses).Assembly);
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

	public static string Address => ResourceManager.GetString("Address", resourceCulture);

	public static string Address2 => ResourceManager.GetString("Address2", resourceCulture);

	public static string Address3 => ResourceManager.GetString("Address3", resourceCulture);

	public static string AddrType => ResourceManager.GetString("AddrType", resourceCulture);

	public static string AdresType => ResourceManager.GetString("AdresType", resourceCulture);

	public static string AltCrdName => ResourceManager.GetString("AltCrdName", resourceCulture);

	public static string AltTaxId => ResourceManager.GetString("AltTaxId", resourceCulture);

	public static string Block => ResourceManager.GetString("Block", resourceCulture);

	public static string Building => ResourceManager.GetString("Building", resourceCulture);

	public static string CardCode => ResourceManager.GetString("CardCode", resourceCulture);

	public static string City => ResourceManager.GetString("City", resourceCulture);

	public static string Country => ResourceManager.GetString("Country", resourceCulture);

	public static string County => ResourceManager.GetString("County", resourceCulture);

	public static string GlblLocNum => ResourceManager.GetString("GlblLocNum", resourceCulture);

	public static string LicTradNum => ResourceManager.GetString("LicTradNum", resourceCulture);

	public static string LineNum => ResourceManager.GetString("LineNum", resourceCulture);

	public static string LogInstanc => ResourceManager.GetString("LogInstanc", resourceCulture);

	public static string Ntnlty => ResourceManager.GetString("Ntnlty", resourceCulture);

	public static string ObjType => ResourceManager.GetString("ObjType", resourceCulture);

	public static string State => ResourceManager.GetString("State", resourceCulture);

	public static string Street => ResourceManager.GetString("Street", resourceCulture);

	public static string StreetNo => ResourceManager.GetString("StreetNo", resourceCulture);

	public static string TaxCode => ResourceManager.GetString("TaxCode", resourceCulture);

	public static string TaxOffice => ResourceManager.GetString("TaxOffice", resourceCulture);

	public static string UserSign => ResourceManager.GetString("UserSign", resourceCulture);

	public static string ZipCode => ResourceManager.GetString("ZipCode", resourceCulture);

	internal BPAddresses()
	{
	}
}
