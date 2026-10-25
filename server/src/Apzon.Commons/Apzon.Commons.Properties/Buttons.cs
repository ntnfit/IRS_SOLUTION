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
public class Buttons
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
				resourceMan = new ResourceManager("Apzon.Commons.Properties.Buttons", typeof(Buttons).Assembly);
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

	public static string Add => ResourceManager.GetString("Add", resourceCulture);

	public static string Cancel => ResourceManager.GetString("Cancel", resourceCulture);

	public static string ClearSelection => ResourceManager.GetString("ClearSelection", resourceCulture);

	public static string Close => ResourceManager.GetString("Close", resourceCulture);

	public static string Delete => ResourceManager.GetString("Delete", resourceCulture);

	public static string Duplicate => ResourceManager.GetString("Duplicate", resourceCulture);

	public static string First => ResourceManager.GetString("First", resourceCulture);

	public static string Help => ResourceManager.GetString("Help", resourceCulture);

	public static string Last => ResourceManager.GetString("Last", resourceCulture);

	public static string Next => ResourceManager.GetString("Next", resourceCulture);

	public static string No => ResourceManager.GetString("No", resourceCulture);

	public static string OK => ResourceManager.GetString("OK", resourceCulture);

	public static string Previous => ResourceManager.GetString("Previous", resourceCulture);

	public static string Refresh => ResourceManager.GetString("Refresh", resourceCulture);

	public static string Save => ResourceManager.GetString("Save", resourceCulture);

	public static string Search => ResourceManager.GetString("Search", resourceCulture);

	public static string SelectAll => ResourceManager.GetString("SelectAll", resourceCulture);

	public static string SetAsDefault => ResourceManager.GetString("SetAsDefault", resourceCulture);

	public static string Update => ResourceManager.GetString("Update", resourceCulture);

	public static string ViewDiagram => ResourceManager.GetString("ViewDiagram", resourceCulture);

	public static string Yes => ResourceManager.GetString("Yes", resourceCulture);

	internal Buttons()
	{
	}
}
