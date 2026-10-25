using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using DevExpress.XtraEditors;

namespace Apzon.Forms.Commons.CFL;

public class frm_DemoCfl : XtraFormEx
{
	private IContainer components;

	private HyperlinkLabelControl hyperlinkLabelControl1;

	private SimpleButtonEx btn_OK;

	private HyperlinkLabelControl hyperlinkLabelControl2;

	private HyperlinkLabelControl hyperlinkLabelControl3;

	private HyperlinkLabelControl hyperlinkLabelControl4;

	private HyperlinkLabelControl hyperlinkLabelControl5;

	private HyperlinkLabelControl hyperlinkLabelControl6;

	private HyperlinkLabelControl hyperlinkLabelControl7;

	private HyperlinkLabelControl hyperlinkLabelControl8;

	private HyperlinkLabelControl hyperlinkLabelControl9;

	private HyperlinkLabelControl hyperlinkLabelControl10;

	private HyperlinkLabelControl hyperlinkLabelControl11;

	private HyperlinkLabelControl hyperlinkLabelControl12;

	public frm_DemoCfl()
	{
		InitializeComponent();
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void hyperlinkLabelControl1_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		new frm_ChooseFromList(Enums.TypeCFL.Items, multiSelect: true, "").ShowDialog(this);
		WinCommons._CloseProcessing();
	}

	private void hyperlinkLabelControl2_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		new frm_ChooseFromList(Enums.TypeCFL.Bp, multiSelect: true, "").ShowDialog(this);
		WinCommons._CloseProcessing();
	}

	private void hyperlinkLabelControl3_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		new frm_ChooseFromList(Enums.TypeCFL.Customer, multiSelect: true, "").ShowDialog(this);
		WinCommons._CloseProcessing();
	}

	private void hyperlinkLabelControl4_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		new frm_ChooseFromList(Enums.TypeCFL.Vendor, multiSelect: true, "").ShowDialog(this);
		WinCommons._CloseProcessing();
	}

	private void hyperlinkLabelControl5_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		new frm_ChooseFromList(Enums.TypeCFL.Lead, multiSelect: true, "").ShowDialog(this);
		WinCommons._CloseProcessing();
	}

	private void hyperlinkLabelControl6_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		new frm_ChooseFromList(Enums.TypeCFL.SaleEmployee, multiSelect: true, "").ShowDialog(this);
		WinCommons._CloseProcessing();
	}

	private void hyperlinkLabelControl7_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		new frm_ChooseFromList(Enums.TypeCFL.ChartOfAccount, multiSelect: true, "").ShowDialog(this);
		WinCommons._CloseProcessing();
	}

	private void hyperlinkLabelControl8_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		new frm_ChooseFromList(Enums.TypeCFL.ItemGroup, multiSelect: true, "").ShowDialog(this);
		WinCommons._CloseProcessing();
	}

	private void hyperlinkLabelControl9_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		new frm_ChooseFromList(Enums.TypeCFL.Vat, multiSelect: true, "").ShowDialog(this);
		WinCommons._CloseProcessing();
	}

	private void hyperlinkLabelControl10_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		new frm_ChooseFromList(Enums.TypeCFL.VatIn, multiSelect: true, "").ShowDialog(this);
		WinCommons._CloseProcessing();
	}

	private void hyperlinkLabelControl11_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		new frm_ChooseFromList(Enums.TypeCFL.VatOut, multiSelect: true, "").ShowDialog(this);
		WinCommons._CloseProcessing();
	}

	private void hyperlinkLabelControl12_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		new frm_ChooseFromList(Enums.TypeCFL.Warehouse, multiSelect: true, "").ShowDialog(this);
		WinCommons._CloseProcessing();
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
		this.hyperlinkLabelControl1 = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.hyperlinkLabelControl2 = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.hyperlinkLabelControl3 = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.hyperlinkLabelControl4 = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.hyperlinkLabelControl5 = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.hyperlinkLabelControl6 = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.hyperlinkLabelControl7 = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.hyperlinkLabelControl8 = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.hyperlinkLabelControl9 = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.hyperlinkLabelControl10 = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.hyperlinkLabelControl11 = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.hyperlinkLabelControl12 = new DevExpress.XtraEditors.HyperlinkLabelControl();
		base.SuspendLayout();
		this.hyperlinkLabelControl1.Cursor = System.Windows.Forms.Cursors.Hand;
		this.hyperlinkLabelControl1.Location = new System.Drawing.Point(12, 12);
		this.hyperlinkLabelControl1.Name = "hyperlinkLabelControl1";
		this.hyperlinkLabelControl1.Size = new System.Drawing.Size(27, 13);
		this.hyperlinkLabelControl1.TabIndex = 0;
		this.hyperlinkLabelControl1.Text = "Items";
		this.hyperlinkLabelControl1.Click += new System.EventHandler(this.hyperlinkLabelControl1_Click);
		this.btn_OK.Location = new System.Drawing.Point(3, 401);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.hyperlinkLabelControl2.Cursor = System.Windows.Forms.Cursors.Hand;
		this.hyperlinkLabelControl2.Location = new System.Drawing.Point(12, 32);
		this.hyperlinkLabelControl2.Name = "hyperlinkLabelControl2";
		this.hyperlinkLabelControl2.Size = new System.Drawing.Size(80, 13);
		this.hyperlinkLabelControl2.TabIndex = 2;
		this.hyperlinkLabelControl2.Text = "Business Partner";
		this.hyperlinkLabelControl2.Click += new System.EventHandler(this.hyperlinkLabelControl2_Click);
		this.hyperlinkLabelControl3.Cursor = System.Windows.Forms.Cursors.Hand;
		this.hyperlinkLabelControl3.Location = new System.Drawing.Point(12, 51);
		this.hyperlinkLabelControl3.Name = "hyperlinkLabelControl3";
		this.hyperlinkLabelControl3.Size = new System.Drawing.Size(51, 13);
		this.hyperlinkLabelControl3.TabIndex = 3;
		this.hyperlinkLabelControl3.Text = "Customers";
		this.hyperlinkLabelControl3.Click += new System.EventHandler(this.hyperlinkLabelControl3_Click);
		this.hyperlinkLabelControl4.Cursor = System.Windows.Forms.Cursors.Hand;
		this.hyperlinkLabelControl4.Location = new System.Drawing.Point(12, 70);
		this.hyperlinkLabelControl4.Name = "hyperlinkLabelControl4";
		this.hyperlinkLabelControl4.Size = new System.Drawing.Size(39, 13);
		this.hyperlinkLabelControl4.TabIndex = 4;
		this.hyperlinkLabelControl4.Text = "Vendors";
		this.hyperlinkLabelControl4.Click += new System.EventHandler(this.hyperlinkLabelControl4_Click);
		this.hyperlinkLabelControl5.Cursor = System.Windows.Forms.Cursors.Hand;
		this.hyperlinkLabelControl5.Location = new System.Drawing.Point(12, 89);
		this.hyperlinkLabelControl5.Name = "hyperlinkLabelControl5";
		this.hyperlinkLabelControl5.Size = new System.Drawing.Size(23, 13);
		this.hyperlinkLabelControl5.TabIndex = 5;
		this.hyperlinkLabelControl5.Text = "Lead";
		this.hyperlinkLabelControl5.Click += new System.EventHandler(this.hyperlinkLabelControl5_Click);
		this.hyperlinkLabelControl6.Cursor = System.Windows.Forms.Cursors.Hand;
		this.hyperlinkLabelControl6.Location = new System.Drawing.Point(12, 108);
		this.hyperlinkLabelControl6.Name = "hyperlinkLabelControl6";
		this.hyperlinkLabelControl6.Size = new System.Drawing.Size(79, 13);
		this.hyperlinkLabelControl6.TabIndex = 6;
		this.hyperlinkLabelControl6.Text = "Sales Employees";
		this.hyperlinkLabelControl6.Click += new System.EventHandler(this.hyperlinkLabelControl6_Click);
		this.hyperlinkLabelControl7.Cursor = System.Windows.Forms.Cursors.Hand;
		this.hyperlinkLabelControl7.Location = new System.Drawing.Point(12, 127);
		this.hyperlinkLabelControl7.Name = "hyperlinkLabelControl7";
		this.hyperlinkLabelControl7.Size = new System.Drawing.Size(84, 13);
		this.hyperlinkLabelControl7.TabIndex = 7;
		this.hyperlinkLabelControl7.Text = "Chart Of Account";
		this.hyperlinkLabelControl7.Click += new System.EventHandler(this.hyperlinkLabelControl7_Click);
		this.hyperlinkLabelControl8.Cursor = System.Windows.Forms.Cursors.Hand;
		this.hyperlinkLabelControl8.Location = new System.Drawing.Point(12, 146);
		this.hyperlinkLabelControl8.Name = "hyperlinkLabelControl8";
		this.hyperlinkLabelControl8.Size = new System.Drawing.Size(54, 13);
		this.hyperlinkLabelControl8.TabIndex = 8;
		this.hyperlinkLabelControl8.Text = "Item Group";
		this.hyperlinkLabelControl8.Click += new System.EventHandler(this.hyperlinkLabelControl8_Click);
		this.hyperlinkLabelControl9.Cursor = System.Windows.Forms.Cursors.Hand;
		this.hyperlinkLabelControl9.Location = new System.Drawing.Point(12, 165);
		this.hyperlinkLabelControl9.Name = "hyperlinkLabelControl9";
		this.hyperlinkLabelControl9.Size = new System.Drawing.Size(56, 13);
		this.hyperlinkLabelControl9.TabIndex = 9;
		this.hyperlinkLabelControl9.Text = "VAT Groups";
		this.hyperlinkLabelControl9.Click += new System.EventHandler(this.hyperlinkLabelControl9_Click);
		this.hyperlinkLabelControl10.Cursor = System.Windows.Forms.Cursors.Hand;
		this.hyperlinkLabelControl10.Location = new System.Drawing.Point(12, 184);
		this.hyperlinkLabelControl10.Name = "hyperlinkLabelControl10";
		this.hyperlinkLabelControl10.Size = new System.Drawing.Size(32, 13);
		this.hyperlinkLabelControl10.TabIndex = 10;
		this.hyperlinkLabelControl10.Text = "VAT In";
		this.hyperlinkLabelControl10.Click += new System.EventHandler(this.hyperlinkLabelControl10_Click);
		this.hyperlinkLabelControl11.Cursor = System.Windows.Forms.Cursors.Hand;
		this.hyperlinkLabelControl11.Location = new System.Drawing.Point(12, 203);
		this.hyperlinkLabelControl11.Name = "hyperlinkLabelControl11";
		this.hyperlinkLabelControl11.Size = new System.Drawing.Size(40, 13);
		this.hyperlinkLabelControl11.TabIndex = 11;
		this.hyperlinkLabelControl11.Text = "VAT Out";
		this.hyperlinkLabelControl11.Click += new System.EventHandler(this.hyperlinkLabelControl11_Click);
		this.hyperlinkLabelControl12.Cursor = System.Windows.Forms.Cursors.Hand;
		this.hyperlinkLabelControl12.Location = new System.Drawing.Point(12, 222);
		this.hyperlinkLabelControl12.Name = "hyperlinkLabelControl12";
		this.hyperlinkLabelControl12.Size = new System.Drawing.Size(55, 13);
		this.hyperlinkLabelControl12.TabIndex = 12;
		this.hyperlinkLabelControl12.Text = "Warehouse";
		this.hyperlinkLabelControl12.Click += new System.EventHandler(this.hyperlinkLabelControl12_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(629, 427);
		base.Controls.Add(this.hyperlinkLabelControl12);
		base.Controls.Add(this.hyperlinkLabelControl11);
		base.Controls.Add(this.hyperlinkLabelControl10);
		base.Controls.Add(this.hyperlinkLabelControl9);
		base.Controls.Add(this.hyperlinkLabelControl8);
		base.Controls.Add(this.hyperlinkLabelControl7);
		base.Controls.Add(this.hyperlinkLabelControl6);
		base.Controls.Add(this.hyperlinkLabelControl5);
		base.Controls.Add(this.hyperlinkLabelControl4);
		base.Controls.Add(this.hyperlinkLabelControl3);
		base.Controls.Add(this.hyperlinkLabelControl2);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.hyperlinkLabelControl1);
		base.Name = "frm_DemoCfl";
		this.Text = "Choose From List - Demo";
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
