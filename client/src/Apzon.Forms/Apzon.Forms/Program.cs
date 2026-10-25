using System;
using System.Windows.Forms;
using Apzon.Commons.Constants;
using Apzon.Controls.Form;
using Apzon.Forms.Commons;
using Apzon.Global;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms;

internal static class Program
{
	public static bool IsLoginOk;

	public static bool IsLoginClose;

	public static Main MainForm;

	[STAThread]
	private static void Main()
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(defaultValue: false);
		UserLookAndFeel.Default.UseDefaultLookAndFeel = true;
		UserLookAndFeel.Default.UseWindowsXPTheme = false;
		SkinManager.EnableFormSkins();
		SkinManager.EnableMdiFormSkins();
		Localizer.Active = new CustomizeDialogButtonText();
		GlobalData.MessageDataCenter = FormMessage.DefaultMessageCenter;
		MainForm = new Main();
		Application.Run(MainForm);
	}

	public static void AplicationExit()
	{
		if (MainForm != null)
		{
			MainForm.CloseAllChildForm();
			MainForm.DisConnectSocket();
			MainForm.Close();
		}
	}

	public static void AplicationRestart()
	{
		if (MainForm != null)
		{
			MainForm.CloseAllChildForm();
			MainForm.DisConnectSocket();
			Application.Restart();
		}
	}
}
