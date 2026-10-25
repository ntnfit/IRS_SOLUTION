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
public class ProductionOrder
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
				resourceMan = new ResourceManager("Apzon.Commons.Properties.ProductionOrder", typeof(ProductionOrder).Assembly);
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

	public static string CardCode => ResourceManager.GetString("CardCode", resourceCulture);

	public static string CloseDate => ResourceManager.GetString("CloseDate", resourceCulture);

	public static string CmpltQty => ResourceManager.GetString("CmpltQty", resourceCulture);

	public static string Comments => ResourceManager.GetString("Comments", resourceCulture);

	public static string CreateDate => ResourceManager.GetString("CreateDate", resourceCulture);

	public static string DocNum => ResourceManager.GetString("DocNum", resourceCulture);

	public static string DueDate => ResourceManager.GetString("DueDate", resourceCulture);

	public static string ItemCode => ResourceManager.GetString("ItemCode", resourceCulture);

	public static string JrnlMemo => ResourceManager.GetString("JrnlMemo", resourceCulture);

	public static string LineDirty => ResourceManager.GetString("LineDirty", resourceCulture);

	public static string LogInstanc => ResourceManager.GetString("LogInstanc", resourceCulture);

	public static string OcrCode => ResourceManager.GetString("OcrCode", resourceCulture);

	public static string OcrCode2 => ResourceManager.GetString("OcrCode2", resourceCulture);

	public static string OcrCode3 => ResourceManager.GetString("OcrCode3", resourceCulture);

	public static string OcrCode4 => ResourceManager.GetString("OcrCode4", resourceCulture);

	public static string OcrCode5 => ResourceManager.GetString("OcrCode5", resourceCulture);

	public static string OriginAbs => ResourceManager.GetString("OriginAbs", resourceCulture);

	public static string OriginNum => ResourceManager.GetString("OriginNum", resourceCulture);

	public static string OriginType => ResourceManager.GetString("OriginType", resourceCulture);

	public static string PickRmrk => ResourceManager.GetString("PickRmrk", resourceCulture);

	public static string PIndicator => ResourceManager.GetString("PIndicator", resourceCulture);

	public static string PlannedQty => ResourceManager.GetString("PlannedQty", resourceCulture);

	public static string PostDate => ResourceManager.GetString("PostDate", resourceCulture);

	public static string Printed => ResourceManager.GetString("Printed", resourceCulture);

	public static string Project => ResourceManager.GetString("Project", resourceCulture);

	public static string RjctQty => ResourceManager.GetString("RjctQty", resourceCulture);

	public static string RlsDate => ResourceManager.GetString("RlsDate", resourceCulture);

	public static string SeqCode => ResourceManager.GetString("SeqCode", resourceCulture);

	public static string Serial => ResourceManager.GetString("Serial", resourceCulture);

	public static string Series => ResourceManager.GetString("Series", resourceCulture);

	public static string SeriesStr => ResourceManager.GetString("SeriesStr", resourceCulture);

	public static string Status => ResourceManager.GetString("Status", resourceCulture);

	public static string SubStr => ResourceManager.GetString("SubStr", resourceCulture);

	public static string SupplCode => ResourceManager.GetString("SupplCode", resourceCulture);

	public static string TransId => ResourceManager.GetString("TransId", resourceCulture);

	public static string Type => ResourceManager.GetString("Type", resourceCulture);

	public static string Uom => ResourceManager.GetString("Uom", resourceCulture);

	public static string UomEntry => ResourceManager.GetString("UomEntry", resourceCulture);

	public static string UpdateDate => ResourceManager.GetString("UpdateDate", resourceCulture);

	public static string UserSign => ResourceManager.GetString("UserSign", resourceCulture);

	public static string UserSign2 => ResourceManager.GetString("UserSign2", resourceCulture);

	public static string Warehouse => ResourceManager.GetString("Warehouse", resourceCulture);

	public static string WOR1Count => ResourceManager.GetString("WOR1Count", resourceCulture);

	internal ProductionOrder()
	{
	}
}
