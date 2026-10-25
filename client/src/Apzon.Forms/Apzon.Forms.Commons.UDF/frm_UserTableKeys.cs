using System.ComponentModel;
using System.Windows.Forms;
using Apzon.Controls.Form;

namespace Apzon.Forms.Commons.UDF;

public class frm_UserTableKeys : XtraFormEx
{
	private IContainer components;

	public frm_UserTableKeys()
	{
		InitializeComponent();
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
		this.Text = "frm_UserTableKeys";
	}
}
