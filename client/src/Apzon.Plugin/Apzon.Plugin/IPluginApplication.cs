using System;
using Apzon.Commons.Enums;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Administrator;
using Apzon.HttpClientHelper;

namespace Apzon.Plugin;

public interface IPluginApplication
{
	IHttpClient HttpClient { get; set; }

	string Name { get; }

	XtraFormEx MainForm { get; set; }

	Action<string, Enums.MessageType> ShowStatus { get; set; }

	Func<PluginApplication, MenuCreation, object> CreateMenu { get; set; }

	void Initialize(int id);

	void MenuEvent(MenuEvent val, out bool cancelEvent);
}
