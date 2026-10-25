using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucGridCanvas : XtraUserControl
{
	public Action BackToMain;

	public Action<double> GetCusPayResult;

	private IContainer components;

	private NavigationFrame navigationFrame1;

	private NavigationPage npage_PaymentScreen;

	private NavigationPage npage_RetailGrid;

	private ucPaymentScreen ucPaymentScreen1;

	public ucRetailGrid ucRetailGrid1;

	public ucGridCanvas()
	{
		InitializeComponent();
	}

	public void NavigateToGrid()
	{
		navigationFrame1.SelectedPage = npage_RetailGrid;
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
		this.npage_PaymentScreen = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.ucPaymentScreen1 = new Apzon.Forms.UserControls.POS_UI.ucPaymentScreen();
		this.npage_RetailGrid = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.ucRetailGrid1 = new Apzon.Forms.UserControls.POS_UI.ucRetailGrid();
		((System.ComponentModel.ISupportInitialize)this.navigationFrame1).BeginInit();
		this.navigationFrame1.SuspendLayout();
		this.npage_PaymentScreen.SuspendLayout();
		this.npage_RetailGrid.SuspendLayout();
		base.SuspendLayout();
		this.navigationFrame1.AllowTransitionAnimation = DevExpress.Utils.DefaultBoolean.False;
		this.navigationFrame1.Controls.Add(this.npage_PaymentScreen);
		this.navigationFrame1.Controls.Add(this.npage_RetailGrid);
		this.navigationFrame1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.navigationFrame1.Location = new System.Drawing.Point(0, 0);
		this.navigationFrame1.Name = "navigationFrame1";
		this.navigationFrame1.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[2] { this.npage_RetailGrid, this.npage_PaymentScreen });
		this.navigationFrame1.SelectedPage = this.npage_RetailGrid;
		this.navigationFrame1.Size = new System.Drawing.Size(753, 475);
		this.navigationFrame1.TabIndex = 10;
		this.navigationFrame1.Text = "navigationFrame1";
		this.npage_PaymentScreen.Controls.Add(this.ucPaymentScreen1);
		this.npage_PaymentScreen.Name = "npage_PaymentScreen";
		this.npage_PaymentScreen.Size = new System.Drawing.Size(753, 475);
		this.ucPaymentScreen1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucPaymentScreen1.Location = new System.Drawing.Point(0, 0);
		this.ucPaymentScreen1.Name = "ucPaymentScreen1";
		this.ucPaymentScreen1.Size = new System.Drawing.Size(753, 475);
		this.ucPaymentScreen1.TabIndex = 0;
		this.npage_RetailGrid.Caption = "npage_RetailGrid";
		this.npage_RetailGrid.Controls.Add(this.ucRetailGrid1);
		this.npage_RetailGrid.Name = "npage_RetailGrid";
		this.npage_RetailGrid.Size = new System.Drawing.Size(753, 475);
		this.ucRetailGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucRetailGrid1.Location = new System.Drawing.Point(0, 0);
		this.ucRetailGrid1.Name = "ucRetailGrid1";
		this.ucRetailGrid1.Size = new System.Drawing.Size(753, 475);
		this.ucRetailGrid1.TabIndex = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.navigationFrame1);
		base.Name = "ucGridCanvas";
		base.Size = new System.Drawing.Size(753, 475);
		((System.ComponentModel.ISupportInitialize)this.navigationFrame1).EndInit();
		this.navigationFrame1.ResumeLayout(false);
		this.npage_PaymentScreen.ResumeLayout(false);
		this.npage_RetailGrid.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
