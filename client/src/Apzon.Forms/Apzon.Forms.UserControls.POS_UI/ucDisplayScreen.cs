using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using DevExpress.Utils;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucDisplayScreen : XtraUserControl
{
	private IContainer components;

	private NavigationFrame navigationFrame1;

	private NavigationPage npage_ScanItem;

	private ucScanItem ucScanItem1;

	private NavigationPage npage_CustomerPay;

	private ucCustomerPay ucCustomerPay1;

	public ucDisplayScreen()
	{
		InitializeComponent();
	}

	public void NavigateToItemScan(string itName = "", double qty = 0.0)
	{
		try
		{
			navigationFrame1.SelectedPage = npage_ScanItem;
			if (!string.IsNullOrEmpty(itName))
			{
				ucScanItem1.SetText(itName, qty);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void NavigateToCusPay(double amount = 0.0)
	{
		try
		{
			navigationFrame1.SelectedPage = npage_CustomerPay;
			if (amount > 0.0)
			{
				ucCustomerPay1.SetCusPayValue(amount);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
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
		this.npage_ScanItem = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.ucScanItem1 = new Apzon.Forms.UserControls.POS_UI.ucScanItem();
		this.npage_CustomerPay = new DevExpress.XtraBars.Navigation.NavigationPage();
		this.ucCustomerPay1 = new Apzon.Forms.UserControls.POS_UI.ucCustomerPay();
		((System.ComponentModel.ISupportInitialize)this.navigationFrame1).BeginInit();
		this.navigationFrame1.SuspendLayout();
		this.npage_ScanItem.SuspendLayout();
		this.npage_CustomerPay.SuspendLayout();
		base.SuspendLayout();
		this.navigationFrame1.AllowTransitionAnimation = DevExpress.Utils.DefaultBoolean.False;
		this.navigationFrame1.Controls.Add(this.npage_ScanItem);
		this.navigationFrame1.Controls.Add(this.npage_CustomerPay);
		this.navigationFrame1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.navigationFrame1.Location = new System.Drawing.Point(0, 0);
		this.navigationFrame1.Name = "navigationFrame1";
		this.navigationFrame1.Pages.AddRange(new DevExpress.XtraBars.Navigation.NavigationPageBase[2] { this.npage_ScanItem, this.npage_CustomerPay });
		this.navigationFrame1.SelectedPage = this.npage_ScanItem;
		this.navigationFrame1.Size = new System.Drawing.Size(563, 94);
		this.navigationFrame1.TabIndex = 0;
		this.navigationFrame1.Text = "navigationFrame1";
		this.npage_ScanItem.Caption = "npage_ScanItem";
		this.npage_ScanItem.Controls.Add(this.ucScanItem1);
		this.npage_ScanItem.Name = "npage_ScanItem";
		this.npage_ScanItem.Size = new System.Drawing.Size(563, 94);
		this.ucScanItem1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucScanItem1.Location = new System.Drawing.Point(0, 0);
		this.ucScanItem1.Name = "ucScanItem1";
		this.ucScanItem1.Size = new System.Drawing.Size(563, 94);
		this.ucScanItem1.TabIndex = 0;
		this.npage_CustomerPay.Caption = "npage_CustomerPay";
		this.npage_CustomerPay.Controls.Add(this.ucCustomerPay1);
		this.npage_CustomerPay.Name = "npage_CustomerPay";
		this.npage_CustomerPay.Size = new System.Drawing.Size(563, 94);
		this.ucCustomerPay1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucCustomerPay1.Location = new System.Drawing.Point(0, 0);
		this.ucCustomerPay1.Name = "ucCustomerPay1";
		this.ucCustomerPay1.Size = new System.Drawing.Size(563, 94);
		this.ucCustomerPay1.TabIndex = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.navigationFrame1);
		base.Name = "ucDisplayScreen";
		base.Size = new System.Drawing.Size(563, 94);
		((System.ComponentModel.ISupportInitialize)this.navigationFrame1).EndInit();
		this.navigationFrame1.ResumeLayout(false);
		this.npage_ScanItem.ResumeLayout(false);
		this.npage_CustomerPay.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
