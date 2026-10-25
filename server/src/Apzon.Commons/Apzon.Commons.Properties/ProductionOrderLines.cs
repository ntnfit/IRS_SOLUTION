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
public class ProductionOrderLines
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
				resourceMan = new ResourceManager("Apzon.Commons.Properties.ProductionOrderLines", typeof(ProductionOrderLines).Assembly);
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

	public static string AdditQty => ResourceManager.GetString("AdditQty", resourceCulture);

	public static string BaseQty => ResourceManager.GetString("BaseQty", resourceCulture);

	public static string CompTotal => ResourceManager.GetString("CompTotal", resourceCulture);

	public static string DocEntry => ResourceManager.GetString("DocEntry", resourceCulture);

	public static string IssuedQty => ResourceManager.GetString("IssuedQty", resourceCulture);

	public static string IssueType => ResourceManager.GetString("IssueType", resourceCulture);

	public static string ItemCode => ResourceManager.GetString("ItemCode", resourceCulture);

	public static string ItemType => ResourceManager.GetString("ItemType", resourceCulture);

	public static string LineNum => ResourceManager.GetString("LineNum", resourceCulture);

	public static string LineText => ResourceManager.GetString("LineText", resourceCulture);

	public static string LoadFrBOM => ResourceManager.GetString("LoadFrBOM", resourceCulture);

	public static string LocCode => ResourceManager.GetString("LocCode", resourceCulture);

	public static string LogInstanc => ResourceManager.GetString("LogInstanc", resourceCulture);

	public static string OcrCode => ResourceManager.GetString("OcrCode", resourceCulture);

	public static string OcrCode2 => ResourceManager.GetString("OcrCode2", resourceCulture);

	public static string OcrCode3 => ResourceManager.GetString("OcrCode3", resourceCulture);

	public static string OcrCode4 => ResourceManager.GetString("OcrCode4", resourceCulture);

	public static string OcrCode5 => ResourceManager.GetString("OcrCode5", resourceCulture);

	public static string PickIdNo => ResourceManager.GetString("PickIdNo", resourceCulture);

	public static string PickQty => ResourceManager.GetString("PickQty", resourceCulture);

	public static string PickStatus => ResourceManager.GetString("PickStatus", resourceCulture);

	public static string PlannedQty => ResourceManager.GetString("PlannedQty", resourceCulture);

	public static string Project => ResourceManager.GetString("Project", resourceCulture);

	public static string ReleaseQty => ResourceManager.GetString("ReleaseQty", resourceCulture);

	public static string UomCode => ResourceManager.GetString("UomCode", resourceCulture);

	public static string UomEntry => ResourceManager.GetString("UomEntry", resourceCulture);

	public static string VisOrder => ResourceManager.GetString("VisOrder", resourceCulture);

	public static string wareHouse => ResourceManager.GetString("wareHouse", resourceCulture);

	public static string WipActCode => ResourceManager.GetString("WipActCode", resourceCulture);

	internal ProductionOrderLines()
	{
	}
}
