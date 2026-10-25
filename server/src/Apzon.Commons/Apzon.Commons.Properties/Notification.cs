using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Apzon.Commons.Properties;

[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
[DebuggerNonUserCode]
[CompilerGenerated]
public class Notification
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
				resourceMan = new ResourceManager("Apzon.Commons.Properties.Notification", typeof(Notification).Assembly);
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

	public static Bitmap dismissAll => (Bitmap)ResourceManager.GetObject("dismissAll", resourceCulture);

	public static string Group_Alert => ResourceManager.GetString("Group_Alert", resourceCulture);

	public static string Group_Comments => ResourceManager.GetString("Group_Comments", resourceCulture);

	public static string Group_Event => ResourceManager.GetString("Group_Event", resourceCulture);

	public static string Group_Issue => ResourceManager.GetString("Group_Issue", resourceCulture);

	public static string Group_Task => ResourceManager.GetString("Group_Task", resourceCulture);

	public static string Title_Event => ResourceManager.GetString("Title_Event", resourceCulture);

	public static string Title_Event_Comment => ResourceManager.GetString("Title_Event_Comment", resourceCulture);

	public static string Title_Issue => ResourceManager.GetString("Title_Issue", resourceCulture);

	public static string Title_Issue_Comment => ResourceManager.GetString("Title_Issue_Comment", resourceCulture);

	public static string Title_Note => ResourceManager.GetString("Title_Note", resourceCulture);

	public static string Title_Note_Comment => ResourceManager.GetString("Title_Note_Comment", resourceCulture);

	public static string Title_Task => ResourceManager.GetString("Title_Task", resourceCulture);

	public static string Title_Task_Comment => ResourceManager.GetString("Title_Task_Comment", resourceCulture);

	public static Bitmap unread_icon => (Bitmap)ResourceManager.GetObject("unread_icon", resourceCulture);

	internal Notification()
	{
	}
}
