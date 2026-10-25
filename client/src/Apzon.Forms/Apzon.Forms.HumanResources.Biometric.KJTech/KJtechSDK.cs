using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using AxSFC3KPCLib;

namespace Apzon.Forms.HumanResources.Biometric.KJTech;

public class KJtechSDK : Form
{
	private IContainer components;

	private AxSFC3KPC SFC3KPC1;

	public KJtechSDK()
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
		base.ClientSize = new System.Drawing.Size(800, 450);
		this.Text = "KJtechSDK";
		this.SFC3KPC1 = new AxSFC3KPCLib.AxSFC3KPC();
		((System.ComponentModel.ISupportInitialize)this.SFC3KPC1).BeginInit();
		base.SuspendLayout();
		this.SFC3KPC1.Enabled = true;
		this.SFC3KPC1.Location = new System.Drawing.Point(0, 0);
		this.SFC3KPC1.Name = "SFC3KPC1";
		this.SFC3KPC1.TabIndex = 0;
		this.SFC3KPC1.Visible = false;
		base.Controls.Add(this.SFC3KPC1);
		((System.ComponentModel.ISupportInitialize)this.SFC3KPC1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
