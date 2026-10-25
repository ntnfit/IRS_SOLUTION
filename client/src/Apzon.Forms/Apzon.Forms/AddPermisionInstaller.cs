using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;

namespace Apzon.Forms;

[RunInstaller(true)]
public class AddPermisionInstaller : Installer
{
	private IContainer components;

	public AddPermisionInstaller()
	{
		InitializeComponent();
	}

	public override void Install(IDictionary savedState)
	{
		base.Install(savedState);
	}

	protected override void OnAfterInstall(IDictionary savedState)
	{
		base.OnAfterInstall(savedState);
		try
		{
			string text = Context.Parameters["assemblypath"];
			if (!string.IsNullOrEmpty(text))
			{
				string directoryName = Path.GetDirectoryName(text);
				if (Directory.Exists(directoryName))
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(directoryName);
					DirectorySecurity accessControl = directoryInfo.GetAccessControl();
					accessControl.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
					directoryInfo.SetAccessControl(accessControl);
				}
			}
		}
		catch (Exception ex)
		{
			using StreamWriter streamWriter = new StreamWriter(Environment.CurrentDirectory + "\\Error.txt", append: true, Encoding.UTF8);
			streamWriter.WriteLine(ex.Message);
		}
	}

	public override void Commit(IDictionary savedState)
	{
		base.Commit(savedState);
	}

	public override void Rollback(IDictionary savedState)
	{
		base.Rollback(savedState);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		components = new Container();
	}
}
