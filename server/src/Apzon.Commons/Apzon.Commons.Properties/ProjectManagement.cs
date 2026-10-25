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
public class ProjectManagement
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
				resourceMan = new ResourceManager("Apzon.Commons.Properties.ProjectManagement", typeof(ProjectManagement).Assembly);
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

	public static string AbsEntry => ResourceManager.GetString("AbsEntry", resourceCulture);

	public static string ActualAmount => ResourceManager.GetString("ActualAmount", resourceCulture);

	public static string ActualDate => ResourceManager.GetString("ActualDate", resourceCulture);

	public static string AmountDiff => ResourceManager.GetString("AmountDiff", resourceCulture);

	public static string BudgetAmount => ResourceManager.GetString("BudgetAmount", resourceCulture);

	public static string BudgetLevel => ResourceManager.GetString("BudgetLevel", resourceCulture);

	public static string CardCode => ResourceManager.GetString("CardCode", resourceCulture);

	public static string CardName => ResourceManager.GetString("CardName", resourceCulture);

	public static string Code => ResourceManager.GetString("Code", resourceCulture);

	public static string DefaultWhs => ResourceManager.GetString("DefaultWhs", resourceCulture);

	public static string DocCur => ResourceManager.GetString("DocCur", resourceCulture);

	public static string DueDate => ResourceManager.GetString("DueDate", resourceCulture);

	public static string FinProject => ResourceManager.GetString("FinProject", resourceCulture);

	public static string IndCode => ResourceManager.GetString("IndCode", resourceCulture);

	public static string MasterEntry => ResourceManager.GetString("MasterEntry", resourceCulture);

	public static string Name => ResourceManager.GetString("Name", resourceCulture);

	public static string OcrCode => ResourceManager.GetString("OcrCode", resourceCulture);

	public static string OcrCode2 => ResourceManager.GetString("OcrCode2", resourceCulture);

	public static string OcrCode3 => ResourceManager.GetString("OcrCode3", resourceCulture);

	public static string OcrCode4 => ResourceManager.GetString("OcrCode4", resourceCulture);

	public static string OcrCode5 => ResourceManager.GetString("OcrCode5", resourceCulture);

	public static string Order => ResourceManager.GetString("Order", resourceCulture);

	public static string Owner => ResourceManager.GetString("Owner", resourceCulture);

	public static string PM => ResourceManager.GetString("PM", resourceCulture);

	public static string PrgStatus => ResourceManager.GetString("PrgStatus", resourceCulture);

	public static string Priority => ResourceManager.GetString("Priority", resourceCulture);

	public static string Project => ResourceManager.GetString("Project", resourceCulture);

	public static string ProjectType => ResourceManager.GetString("ProjectType", resourceCulture);

	public static string Remark => ResourceManager.GetString("Remark", resourceCulture);

	public static string RiskLvl => ResourceManager.GetString("RiskLvl", resourceCulture);

	public static string StartDate => ResourceManager.GetString("StartDate", resourceCulture);

	internal ProjectManagement()
	{
	}
}
