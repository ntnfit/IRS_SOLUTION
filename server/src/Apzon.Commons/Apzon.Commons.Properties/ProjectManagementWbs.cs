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
public class ProjectManagementWbs
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
				resourceMan = new ResourceManager("Apzon.Commons.Properties.ProjectManagementWbs", typeof(ProjectManagementWbs).Assembly);
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

	public static string BaseLine => ResourceManager.GetString("BaseLine", resourceCulture);

	public static string Dependent => ResourceManager.GetString("Dependent", resourceCulture);

	public static string FatherNum => ResourceManager.GetString("FatherNum", resourceCulture);

	public static string FromDate => ResourceManager.GetString("FromDate", resourceCulture);

	public static string Milestone => ResourceManager.GetString("Milestone", resourceCulture);

	public static string Notes => ResourceManager.GetString("Notes", resourceCulture);

	public static string OcrCode => ResourceManager.GetString("OcrCode", resourceCulture);

	public static string OcrCode2 => ResourceManager.GetString("OcrCode2", resourceCulture);

	public static string OcrCode3 => ResourceManager.GetString("OcrCode3", resourceCulture);

	public static string OcrCode4 => ResourceManager.GetString("OcrCode4", resourceCulture);

	public static string OcrCode5 => ResourceManager.GetString("OcrCode5", resourceCulture);

	public static string Priority => ResourceManager.GetString("Priority", resourceCulture);

	public static string Resource => ResourceManager.GetString("Resource", resourceCulture);

	public static string Status => ResourceManager.GetString("Status", resourceCulture);

	public static string TaskCode => ResourceManager.GetString("TaskCode", resourceCulture);

	public static string TaskName => ResourceManager.GetString("TaskName", resourceCulture);

	public static string ToDate => ResourceManager.GetString("ToDate", resourceCulture);

	public static string VisOrder => ResourceManager.GetString("VisOrder", resourceCulture);

	public static string Wbs => ResourceManager.GetString("Wbs", resourceCulture);

	public static string WBSTemplate => ResourceManager.GetString("WBSTemplate", resourceCulture);

	internal ProjectManagementWbs()
	{
	}
}
