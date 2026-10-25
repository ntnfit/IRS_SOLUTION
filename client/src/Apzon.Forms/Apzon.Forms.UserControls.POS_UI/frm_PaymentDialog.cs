using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Controls.Form;

namespace Apzon.Forms.UserControls.POS_UI;

public class frm_PaymentDialog : XtraFormEx
{
	public double InvTotal;

	private IContainer components;

	private ucPaymentDialog ucPaymentDialog1;

	public frm_PaymentDialog()
	{
		InitializeComponent();
		Load += OnLoad;
		SizeChanged += Form1_SizeChanged;
	}

	private void OnLoad(object sender, EventArgs eventArgs)
	{
		ucPaymentDialog1.SetValues(InvTotal);
	}

	private void Cancel()
	{
		DialogResult = DialogResult.Cancel;
		Close();
	}

	private void Agree()
	{
		DialogResult = DialogResult.OK;
		Close();
	}

	private void Form1_SizeChanged(object sender, EventArgs e)
	{
		CenterControlInParent(ucPaymentDialog1);
	}

	private void CenterControlInParent(Control ctrlToCenter)
	{
		ctrlToCenter.Left = (ctrlToCenter.Parent.Width - ctrlToCenter.Width) / 2;
		ctrlToCenter.Top = (ctrlToCenter.Parent.Height - ctrlToCenter.Height) / 2;
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
		this.ucPaymentDialog1 = new Apzon.Forms.UserControls.POS_UI.ucPaymentDialog();
		base.SuspendLayout();
		this.ucPaymentDialog1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucPaymentDialog1.FormId = "oPaymentDialog";
		this.ucPaymentDialog1.ItemId = "ODATA";
		this.ucPaymentDialog1.Location = new System.Drawing.Point(0, 0);
		this.ucPaymentDialog1.Name = "ucPaymentDialog1";
		this.ucPaymentDialog1.Size = new System.Drawing.Size(566, 448);
		this.ucPaymentDialog1.TabIndex = 0;
		base.Appearance.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(566, 448);
		base.Controls.Add(this.ucPaymentDialog1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		base.Name = "frm_PaymentDialog";
		base.ShowIcon = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "frm_PaymentDialog";
		base.ResumeLayout(false);
	}
}
