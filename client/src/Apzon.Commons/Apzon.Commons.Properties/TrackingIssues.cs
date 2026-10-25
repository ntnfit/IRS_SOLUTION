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
public class TrackingIssues
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
				resourceMan = new ResourceManager("Apzon.Commons.Properties.TrackingIssues", typeof(TrackingIssues).Assembly);
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

	public static string AttachedFiled => ResourceManager.GetString("AttachedFiled", resourceCulture);

	public static string BpCode => ResourceManager.GetString("BpCode", resourceCulture);

	public static string HandleBy => ResourceManager.GetString("HandleBy", resourceCulture);

	public static string Id => ResourceManager.GetString("Id", resourceCulture);

	public static string IssueDate => ResourceManager.GetString("IssueDate", resourceCulture);

	public static string IssueType => ResourceManager.GetString("IssueType", resourceCulture);

	public static string Note => ResourceManager.GetString("Note", resourceCulture);

	public static string Priority => ResourceManager.GetString("Priority", resourceCulture);

	public static string ProjectCode => ResourceManager.GetString("ProjectCode", resourceCulture);

	public static string ProjectOwner => ResourceManager.GetString("ProjectOwner", resourceCulture);

	public static string Reporter => ResourceManager.GetString("Reporter", resourceCulture);

	public static string ResponseDate => ResourceManager.GetString("ResponseDate", resourceCulture);

	public static string ServiceCall => ResourceManager.GetString("ServiceCall", resourceCulture);

	public static string SolutionBy => ResourceManager.GetString("SolutionBy", resourceCulture);

	public static string SolutionDesc => ResourceManager.GetString("SolutionDesc", resourceCulture);

	public static string SolutionId => ResourceManager.GetString("SolutionId", resourceCulture);

	public static string Status => ResourceManager.GetString("Status", resourceCulture);

	public static string Subject => ResourceManager.GetString("Subject", resourceCulture);

	public static string TaskCode => ResourceManager.GetString("TaskCode", resourceCulture);

	public static string TaskName => ResourceManager.GetString("TaskName", resourceCulture);

	public static string UserSign => ResourceManager.GetString("UserSign", resourceCulture);

	public static string UserSign2 => ResourceManager.GetString("UserSign2", resourceCulture);

	public static string WbsCode => ResourceManager.GetString("WbsCode", resourceCulture);

	internal TrackingIssues()
	{
	}
}
