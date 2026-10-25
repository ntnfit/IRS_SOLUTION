using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucLeftPanel : XtraUserControl
{
	private IContainer components;

	private NavigationFrame navigationFrame1;

	private NavigationPage navigationPage1;

	private NavigationPage navigationPage2;

	private ucMemberBarcode ucMemberBarcode2;

	private ucMemberDetail ucMemberDetail1;

	private NavigationPage navigationPage3;

	private ucPaymentScreen ucPaymentScreen1;

	private NavigationPage navigationPage4;

	private ucBarcode ucBarcode1;

	private ucRetailGrid ucRetailGrid1;

	public ucLeftPanel()
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
		this.navigationFrame1 = new DevExpress.XtraBars.Navigation.NavigationFrame();
		this.navigationPage1 = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.ucMemberBarcode2 = new Apzon.Forms.UserControls.POS_UI.ucMemberBarcode();
		this.navigationPage2 = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.ucMemberDetail1 = new Apzon.Forms.UserControls.POS_UI.ucMemberDetail();
		this.navigationPage3 = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.ucPaymentScreen1 = new Apzon.Forms.UserControls.POS_UI.ucPaymentScreen();
		this.navigationPage4 = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.ucBarcode1 = new Apzon.Forms.UserControls.POS_UI.ucBarcode();
		this.ucRetailGrid1 = new Apzon.Forms.UserControls.POS_UI.ucRetailGrid();
		((System.ComponentModel.ISupportInitialize)this.navigationFrame1).BeginInit();
		this.navigationFrame1.SuspendLayout();
		this.navigationPage1.SuspendLayout();
		this.navigationPage2.SuspendLayout();
		this.navigationPage3.SuspendLayout();
		this.navigationPage4.SuspendLayout();
		base.SuspendLayout();
		this.navigationFrame1.Controls.Add(this.navigationPage1);
		this.navigationFrame1.Controls.Add(this.navigationPage2);
		this.navigationFrame1.Controls.Add(this.navigationPage3);
		this.navigationFrame1.Controls.Add(this.navigationPage4);
		this.navigationFrame1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.navigationFrame1.Location = new System.Drawing.Point(0, 0);
		this.navigationFrame1.Name = "navigationFrame1";
		this.navigationFrame1.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[4] { this.navigationPage4, this.navigationPage1, this.navigationPage2, this.navigationPage3 });
		this.navigationFrame1.SelectedPage = this.navigationPage4;
		this.navigationFrame1.Size = new System.Drawing.Size(613, 476);
		this.navigationFrame1.TabIndex = 0;
		this.navigationFrame1.Text = "navigationFrame1";
		this.navigationPage1.Caption = "navigationPage1";
		this.navigationPage1.Controls.Add(this.ucMemberBarcode2);
		this.navigationPage1.Name = "navigationPage1";
		this.navigationPage1.Size = new System.Drawing.Size(613, 476);
		this.ucMemberBarcode2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucMemberBarcode2.Location = new System.Drawing.Point(0, 0);
		this.ucMemberBarcode2.Name = "ucMemberBarcode2";
		this.ucMemberBarcode2.Size = new System.Drawing.Size(613, 476);
		this.ucMemberBarcode2.TabIndex = 1;
		this.navigationPage2.Caption = "navigationPage2";
		this.navigationPage2.Controls.Add(this.ucMemberDetail1);
		this.navigationPage2.Name = "navigationPage2";
		this.navigationPage2.Size = new System.Drawing.Size(613, 476);
		this.ucMemberDetail1.Appearance.BackColor = System.Drawing.SystemColors.Control;
		this.ucMemberDetail1.Appearance.Options.UseBackColor = true;
		this.ucMemberDetail1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucMemberDetail1.Location = new System.Drawing.Point(0, 0);
		this.ucMemberDetail1.Name = "ucMemberDetail1";
		this.ucMemberDetail1.Size = new System.Drawing.Size(613, 476);
		this.ucMemberDetail1.TabIndex = 0;
		this.navigationPage3.Caption = "navigationPage3";
		this.navigationPage3.Controls.Add(this.ucPaymentScreen1);
		this.navigationPage3.Name = "navigationPage3";
		this.navigationPage3.Size = new System.Drawing.Size(613, 476);
		this.ucPaymentScreen1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucPaymentScreen1.Location = new System.Drawing.Point(0, 0);
		this.ucPaymentScreen1.Name = "ucPaymentScreen1";
		this.ucPaymentScreen1.Size = new System.Drawing.Size(613, 476);
		this.ucPaymentScreen1.TabIndex = 0;
		this.navigationPage4.Caption = "navigationPage4";
		this.navigationPage4.Controls.Add(this.ucBarcode1);
		this.navigationPage4.Controls.Add(this.ucRetailGrid1);
		this.navigationPage4.Name = "navigationPage4";
		this.navigationPage4.Size = new System.Drawing.Size(613, 476);
		this.ucBarcode1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.ucBarcode1.Location = new System.Drawing.Point(0, 3);
		this.ucBarcode1.Name = "ucBarcode1";
		this.ucBarcode1.RetailGrid = null;
		this.ucBarcode1.Size = new System.Drawing.Size(610, 40);
		this.ucBarcode1.TabIndex = 1;
		this.ucRetailGrid1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.ucRetailGrid1.Location = new System.Drawing.Point(0, 49);
		this.ucRetailGrid1.Name = "ucRetailGrid1";
		this.ucRetailGrid1.Size = new System.Drawing.Size(610, 426);
		this.ucRetailGrid1.TabIndex = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.navigationFrame1);
		base.Name = "ucLeftPanel";
		base.Size = new System.Drawing.Size(613, 476);
		((System.ComponentModel.ISupportInitialize)this.navigationFrame1).EndInit();
		this.navigationFrame1.ResumeLayout(false);
		this.navigationPage1.ResumeLayout(false);
		this.navigationPage2.ResumeLayout(false);
		this.navigationPage3.ResumeLayout(false);
		this.navigationPage4.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
