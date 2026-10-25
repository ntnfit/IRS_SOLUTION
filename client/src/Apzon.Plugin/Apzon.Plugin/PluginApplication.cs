using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using Apzon.Commons.Enums;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Administrator;
using Apzon.HttpClientHelper;

namespace Apzon.Plugin;

public class PluginApplication : IPluginApplication
{
	public virtual string Name => "Plugin Application";

	[Import("ShowStatus")]
	public Action<string, Enums.MessageType> ShowStatus { get; set; }

	[Import("MenuCreation")]
	public Func<PluginApplication, MenuCreation, object> CreateMenu { get; set; }

	public XtraFormEx MainForm { get; set; }

	public IHttpClient HttpClient { get; set; }

	public virtual void Initialize(int id)
	{
	}

	public virtual void MenuEvent(MenuEvent val, out bool cancelEvent)
	{
		cancelEvent = false;
	}

	public virtual void ShowChildForm(XtraFormEx form)
	{
		if (MainForm != null)
		{
			((Form)(object)form).MdiParent = (Form)(object)MainForm;
		}
		form.Show();
	}
}
