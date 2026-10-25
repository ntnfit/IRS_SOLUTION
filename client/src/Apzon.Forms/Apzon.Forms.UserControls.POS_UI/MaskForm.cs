using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using DevExpress.XtraEditors;

namespace Apzon.Forms.UserControls.POS_UI;

public class MaskForm : Form
{
	public Action FrmShown;

	private IContainer components;

	public MaskForm(XtraUserControl control, string message, string caption)
	{
		MaskForm maskForm = this;
		InitializeComponent();
		Load += OnLoad;
		Shown += (object o, EventArgs args) =>
		{
			try
			{
				frm_Dialog frm_Dialog2 = new frm_Dialog(control)
				{
					Caption = caption,
					StartPosition = FormStartPosition.CenterParent,
					ShowInTaskbar = false
				};
				frm_Dialog2.Shown += (object sender2, EventArgs eventArgs) =>
				{
					maskForm.FrmShown?.Invoke();
				};
				maskForm.DialogResult = frm_Dialog2.ShowDialog(maskForm);
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", ex);
			}
		};
	}

	public MaskForm(XtraForm form)
	{
		MaskForm maskForm = this;
		InitializeComponent();
		Load += OnLoad;
		Shown += (object o, EventArgs args) =>
		{
			maskForm.DialogResult = form.ShowDialog(maskForm);
		};
	}

	private void OnLoad(object sender, EventArgs eventArgs)
	{
		Opacity = 0.25;
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
		base.SuspendLayout();
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
		base.ClientSize = new System.Drawing.Size(814, 497);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		base.Name = "MaskForm";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "MaskForm";
		base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
		base.ResumeLayout(false);
	}
}
