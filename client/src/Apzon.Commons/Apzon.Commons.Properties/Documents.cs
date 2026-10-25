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
public class Documents
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
				resourceMan = new ResourceManager("Apzon.Commons.Properties.Documents", typeof(Documents).Assembly);
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

	public static string AtcEntry => ResourceManager.GetString("AtcEntry", resourceCulture);

	public static string BPLId => ResourceManager.GetString("BPLId", resourceCulture);

	public static string Canceled => ResourceManager.GetString("Canceled", resourceCulture);

	public static string CardCode => ResourceManager.GetString("CardCode", resourceCulture);

	public static string CardName => ResourceManager.GetString("CardName", resourceCulture);

	public static string CntctCode => ResourceManager.GetString("CntctCode", resourceCulture);

	public static string Comments => ResourceManager.GetString("Comments", resourceCulture);

	public static string Currency => ResourceManager.GetString("Currency", resourceCulture);

	public static string DiscPrcnt => ResourceManager.GetString("DiscPrcnt", resourceCulture);

	public static string DocCur => ResourceManager.GetString("DocCur", resourceCulture);

	public static string DocDate => ResourceManager.GetString("DocDate", resourceCulture);

	public static string DocDueDate => ResourceManager.GetString("DocDueDate", resourceCulture);

	public static string DocEntry => ResourceManager.GetString("DocEntry", resourceCulture);

	public static string DocNum => ResourceManager.GetString("DocNum", resourceCulture);

	public static string DocRate => ResourceManager.GetString("DocRate", resourceCulture);

	public static string DocStatus => ResourceManager.GetString("DocStatus", resourceCulture);

	public static string DocStatusName => ResourceManager.GetString("DocStatusName", resourceCulture);

	public static string DocTotal => ResourceManager.GetString("DocTotal", resourceCulture);

	public static string DocTotalFC => ResourceManager.GetString("DocTotalFC", resourceCulture);

	public static string OwnerCode => ResourceManager.GetString("OwnerCode", resourceCulture);

	public static string OwnerName => ResourceManager.GetString("OwnerName", resourceCulture);

	public static string ReqDate => ResourceManager.GetString("ReqDate", resourceCulture);

	public static string Series => ResourceManager.GetString("Series", resourceCulture);

	public static string SlpCode => ResourceManager.GetString("SlpCode", resourceCulture);

	public static string TaxDate => ResourceManager.GetString("TaxDate", resourceCulture);

	public static string TaxTotal => ResourceManager.GetString("TaxTotal", resourceCulture);

	public static string Total => ResourceManager.GetString("Total", resourceCulture);

	public static string TotalBeforeDis => ResourceManager.GetString("TotalBeforeDis", resourceCulture);

	public static string TotalExpns => ResourceManager.GetString("TotalExpns", resourceCulture);

	public static string VATRegNum => ResourceManager.GetString("VATRegNum", resourceCulture);

	public static string VatSum => ResourceManager.GetString("VatSum", resourceCulture);

	public static string VatSumFC => ResourceManager.GetString("VatSumFC", resourceCulture);

	public static string WddStatus => ResourceManager.GetString("WddStatus", resourceCulture);

	internal Documents()
	{
	}
}
