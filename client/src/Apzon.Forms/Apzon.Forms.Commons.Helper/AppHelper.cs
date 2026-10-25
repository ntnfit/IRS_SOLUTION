using System;
using System.Diagnostics;
using System.Drawing;
using DevExpress.Utils;

namespace Apzon.Forms.Commons.Helper;

internal static class AppHelper
{
	private static Image img;

	private static WeakReference wRef;

	public static Icon AppIcon => ResourceImageHelperCore.CreateIconFromResourcesEx("DevExpress.DevAV.Resources.AppIcon.ico", typeof(Main).Assembly);

	public static Image AppImage
	{
		get
		{
			if (img == null)
			{
				img = AppIcon.ToBitmap();
			}
			return img;
		}
	}

	public static Main MainForm
	{
		get
		{
			if (wRef == null)
			{
				return null;
			}
			return wRef.Target as Main;
		}
		set
		{
			wRef = new WeakReference(value);
		}
	}

	public static void ProcessStart(string name)
	{
		ProcessStart(name, string.Empty);
	}

	public static void ProcessStart(string name, string arguments)
	{
		Process process = new Process();
		process.StartInfo.FileName = name;
		process.StartInfo.Arguments = arguments;
		process.StartInfo.Verb = "Open";
		process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
		process.Start();
	}

	public static float GetDefaultSize()
	{
		return 8.25f;
	}
}
