using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace Apzon.Forms.Production;

public class frm_ManufacturingSummary : XtraForm
{
	private IContainer components;

	public frm_ManufacturingSummary()
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
		this.Text = "frm_ManufacturingSummary";
	}
}
