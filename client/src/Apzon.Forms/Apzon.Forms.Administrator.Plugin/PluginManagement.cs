using System.Collections.Generic;
using System.ComponentModel.Composition;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Entities.Models.Administrator;
using Apzon.Plugin;

namespace Apzon.Forms.Administrator.Plugin;

public class PluginManagement
{
	public CreateMainMenu CreateMainMenu;

	[ImportMany(typeof(IPluginApplication))]
	public IEnumerable<IPluginApplication> PluginApplications { get; set; }

	[Export("MenuCreation")]
	public object CreateMenu(PluginApplication sender, MenuCreation menu)
	{
		if (CreateMainMenu != null)
		{
			CreateMainMenu(sender, menu);
		}
		return menu;
	}

	[Export("ShowStatus")]
	public void ShowStatus(string text, Enums.MessageType msgType)
	{
		switch (msgType)
		{
		case Enums.MessageType.Error:
			WinCommons.Error(Enums.MessageCode.None, text);
			break;
		case Enums.MessageType.Success:
			WinCommons.Success(Enums.MessageCode.None, text);
			break;
		case Enums.MessageType.Warning:
			WinCommons.Warning(Enums.MessageCode.None, text);
			break;
		case Enums.MessageType.Info:
			WinCommons.Info(Enums.MessageCode.None, text);
			break;
		}
	}
}
