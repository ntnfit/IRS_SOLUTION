using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucCommandPanel : XtraUserControl
{
	public bool InMainScrn = true;

	private IContainer components;

	private NavigationFrame navigationFrame1;

	private NavigationPage npage_MainCmd;

	private NavigationPage npage_PaymentCmd;

	public ucMainCmd ucMainCmd1;

	public ucPaymentCmd ucPaymentCmd1;

	public ucCommandPanel()
	{
		InitializeComponent();
	}

	public void NavigateToMainCmd()
	{
		navigationFrame1.SelectedPage = npage_MainCmd;
		InMainScrn = true;
	}

	public void NavigateToPaymentCmd()
	{
		navigationFrame1.SelectedPage = npage_PaymentCmd;
		InMainScrn = false;
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
		this.navigationFrame1 = new DevExpress.XtraBars.Navigation.NavigationFrame();
		this.npage_MainCmd = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.ucMainCmd1 = new Apzon.Forms.UserControls.POS_UI.ucMainCmd();
		this.npage_PaymentCmd = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.ucPaymentCmd1 = new Apzon.Forms.UserControls.POS_UI.ucPaymentCmd();
		((System.ComponentModel.ISupportInitialize)this.navigationFrame1).BeginInit();
		this.navigationFrame1.SuspendLayout();
		this.npage_MainCmd.SuspendLayout();
		this.npage_PaymentCmd.SuspendLayout();
		base.SuspendLayout();
		this.navigationFrame1.AllowTransitionAnimation = DevExpress.Utils.DefaultBoolean.False;
		this.navigationFrame1.Controls.Add(this.npage_MainCmd);
		this.navigationFrame1.Controls.Add(this.npage_PaymentCmd);
		this.navigationFrame1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.navigationFrame1.Location = new System.Drawing.Point(0, 0);
		this.navigationFrame1.Name = "navigationFrame1";
		this.navigationFrame1.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[2] { this.npage_MainCmd, this.npage_PaymentCmd });
		this.navigationFrame1.SelectedPage = this.npage_MainCmd;
		this.navigationFrame1.Size = new System.Drawing.Size(851, 430);
		this.navigationFrame1.TabIndex = 0;
		this.navigationFrame1.Text = "navigationFrame1";
		this.npage_MainCmd.Caption = "npage_MainCmd";
		this.npage_MainCmd.Controls.Add(this.ucMainCmd1);
		this.npage_MainCmd.Name = "npage_MainCmd";
		this.npage_MainCmd.Size = new System.Drawing.Size(851, 430);
		this.ucMainCmd1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucMainCmd1.Location = new System.Drawing.Point(0, 0);
		this.ucMainCmd1.Name = "ucMainCmd1";
		this.ucMainCmd1.Size = new System.Drawing.Size(851, 430);
		this.ucMainCmd1.TabIndex = 0;
		this.npage_PaymentCmd.Caption = "npage_PaymentCmd";
		this.npage_PaymentCmd.Controls.Add(this.ucPaymentCmd1);
		this.npage_PaymentCmd.Name = "npage_PaymentCmd";
		this.npage_PaymentCmd.Size = new System.Drawing.Size(851, 430);
		this.ucPaymentCmd1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucPaymentCmd1.Location = new System.Drawing.Point(0, 0);
		this.ucPaymentCmd1.Name = "ucPaymentCmd1";
		this.ucPaymentCmd1.Size = new System.Drawing.Size(851, 430);
		this.ucPaymentCmd1.TabIndex = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.navigationFrame1);
		base.Name = "ucCommandPanel";
		base.Size = new System.Drawing.Size(851, 430);
		((System.ComponentModel.ISupportInitialize)this.navigationFrame1).EndInit();
		this.navigationFrame1.ResumeLayout(false);
		this.npage_MainCmd.ResumeLayout(false);
		this.npage_PaymentCmd.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
