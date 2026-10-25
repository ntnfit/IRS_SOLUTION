using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Forms.Administrator;

namespace Apzon.Forms.DMS;

public class frm_DMSAuth : frm_Authorization
{
	private IContainer components;

	public frm_DMSAuth()
	{
		InitializeComponent();
		isAuthorizationDMS = true;
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
		this.components = new System.ComponentModel.Container();
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(800, 450);
		this.Text = "frm_DMSAuth";
	}
}
