using System;
using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraBars.Ribbon;

namespace Apzon.Controls.Form;

public class RibbonFormEx : RibbonForm
{
	private static Icon _icon;

	public RibbonFormEx()
	{
		if (_icon == null)
		{
			_icon = (Icon)new ComponentResourceManager(typeof(XtraFormEx)).GetObject("XtraFormEx.IconOptions.Icon");
		}
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		IconOptions.Icon = _icon;
	}

	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Controls.Form.RibbonFormEx));
		base.SuspendLayout();
		base.ClientSize = new System.Drawing.Size(298, 299);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("RibbonFormEx.IconOptions.Icon");
		base.Name = "RibbonFormEx";
		base.ResumeLayout(false);
	}
}
