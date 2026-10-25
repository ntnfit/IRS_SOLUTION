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
public class OpportuntityStages
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
				resourceMan = new ResourceManager("Apzon.Commons.Properties.OpportuntityStages", typeof(OpportuntityStages).Assembly);
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

	public static string ChnCrdCode => ResourceManager.GetString("ChnCrdCode", resourceCulture);

	public static string ChnCrdCon => ResourceManager.GetString("ChnCrdCon", resourceCulture);

	public static string ChnCrdName => ResourceManager.GetString("ChnCrdName", resourceCulture);

	public static string CloseDate => ResourceManager.GetString("CloseDate", resourceCulture);

	public static string ClosePrcnt => ResourceManager.GetString("ClosePrcnt", resourceCulture);

	public static string CntctCode => ResourceManager.GetString("CntctCode", resourceCulture);

	public static string DocChkbox => ResourceManager.GetString("DocChkbox", resourceCulture);

	public static string DocId => ResourceManager.GetString("DocId", resourceCulture);

	public static string DocNumber => ResourceManager.GetString("DocNumber", resourceCulture);

	public static string Line => ResourceManager.GetString("Line", resourceCulture);

	public static string Linked => ResourceManager.GetString("Linked", resourceCulture);

	public static string MaxSumLoc => ResourceManager.GetString("MaxSumLoc", resourceCulture);

	public static string MaxSumSys => ResourceManager.GetString("MaxSumSys", resourceCulture);

	public static string Memo => ResourceManager.GetString("Memo", resourceCulture);

	public static string ObjType => ResourceManager.GetString("ObjType", resourceCulture);

	public static string OpenDate => ResourceManager.GetString("OpenDate", resourceCulture);

	public static string OpprId => ResourceManager.GetString("OpprId", resourceCulture);

	public static string Owner => ResourceManager.GetString("Owner", resourceCulture);

	public static string SlpCode => ResourceManager.GetString("SlpCode", resourceCulture);

	public static string Status => ResourceManager.GetString("Status", resourceCulture);

	public static string Step_Id => ResourceManager.GetString("Step_Id", resourceCulture);

	public static string WtSumLoc => ResourceManager.GetString("WtSumLoc", resourceCulture);

	public static string WtSumSys => ResourceManager.GetString("WtSumSys", resourceCulture);

	internal OpportuntityStages()
	{
	}
}
