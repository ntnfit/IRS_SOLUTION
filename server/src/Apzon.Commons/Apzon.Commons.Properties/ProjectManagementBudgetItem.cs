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
public class ProjectManagementBudgetItem
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
				resourceMan = new ResourceManager("Apzon.Commons.Properties.ProjectManagementBudgetItem", typeof(ProjectManagementBudgetItem).Assembly);
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

	public static string ActualAmount => ResourceManager.GetString("ActualAmount", resourceCulture);

	public static string ActualQty => ResourceManager.GetString("ActualQty", resourceCulture);

	public static string Amount => ResourceManager.GetString("Amount", resourceCulture);

	public static string BaseLine => ResourceManager.GetString("BaseLine", resourceCulture);

	public static string BaseQty => ResourceManager.GetString("BaseQty", resourceCulture);

	public static string BomItem => ResourceManager.GetString("BomItem", resourceCulture);

	public static string BplId => ResourceManager.GetString("BplId", resourceCulture);

	public static string Currency => ResourceManager.GetString("Currency", resourceCulture);

	public static string DefaultWhs => ResourceManager.GetString("DefaultWhs", resourceCulture);

	public static string Dscription => ResourceManager.GetString("Dscription", resourceCulture);

	public static string FatherNum => ResourceManager.GetString("FatherNum", resourceCulture);

	public static string FromDate => ResourceManager.GetString("FromDate", resourceCulture);

	public static string IsNew => ResourceManager.GetString("IsNew", resourceCulture);

	public static string ItemCode => ResourceManager.GetString("ItemCode", resourceCulture);

	public static string ItmsGrpCod => ResourceManager.GetString("ItmsGrpCod", resourceCulture);

	public static string Notes => ResourceManager.GetString("Notes", resourceCulture);

	public static string OrderItem => ResourceManager.GetString("OrderItem", resourceCulture);

	public static string PlanQty => ResourceManager.GetString("PlanQty", resourceCulture);

	public static string QtyDiff => ResourceManager.GetString("QtyDiff", resourceCulture);

	public static string RefPrice => ResourceManager.GetString("RefPrice", resourceCulture);

	public static string TaskCode => ResourceManager.GetString("TaskCode", resourceCulture);

	public static string TaskName => ResourceManager.GetString("TaskName", resourceCulture);

	public static string ToDate => ResourceManager.GetString("ToDate", resourceCulture);

	public static string UoM => ResourceManager.GetString("UoM", resourceCulture);

	public static string VisOrder => ResourceManager.GetString("VisOrder", resourceCulture);

	internal ProjectManagementBudgetItem()
	{
	}
}
