using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraSplashScreen;

namespace Apzon.Forms.Commons;

public class SplashScreenApplication : SplashScreen
{
	public enum SplashScreenCommand
	{

	}

	private IContainer components;

	private PictureEdit pic_Apzon;

	private LabelControl lbl_Start;

	private LabelControl lbl_Copy;

	private MarqueeProgressBarControl Processing;

	public SplashScreenApplication()
	{
		InitializeComponent();
		lbl_Copy.Text = "Copyright © 2012-" + DateTime.Today.Year;
	}

	public override void ProcessCommand(Enum cmd, object arg)
	{
		base.ProcessCommand(cmd, arg);
	}

	private async void SplashScreenApplication_Load(object sender, EventArgs e)
	{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.SplashScreenApplication));
		this.pic_Apzon = new DevExpress.XtraEditors.PictureEdit();
		this.lbl_Start = new DevExpress.XtraEditors.LabelControl();
		this.lbl_Copy = new DevExpress.XtraEditors.LabelControl();
		this.Processing = new DevExpress.XtraEditors.MarqueeProgressBarControl();
		((System.ComponentModel.ISupportInitialize)this.pic_Apzon.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Processing.Properties).BeginInit();
		base.SuspendLayout();
		this.pic_Apzon.EditValue = resources.GetObject("pic_Apzon.EditValue");
		this.pic_Apzon.Location = new System.Drawing.Point(5, 2);
		this.pic_Apzon.Name = "pic_Apzon";
		this.pic_Apzon.Properties.AllowFocused = false;
		this.pic_Apzon.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.pic_Apzon.Properties.Appearance.Options.UseBackColor = true;
		this.pic_Apzon.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.pic_Apzon.Properties.ShowMenu = false;
		this.pic_Apzon.Size = new System.Drawing.Size(295, 96);
		this.pic_Apzon.TabIndex = 13;
		this.lbl_Start.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.lbl_Start.Location = new System.Drawing.Point(5, 104);
		this.lbl_Start.Name = "lbl_Start";
		this.lbl_Start.Size = new System.Drawing.Size(55, 13);
		this.lbl_Start.TabIndex = 12;
		this.lbl_Start.Text = "Starting...";
		this.lbl_Copy.Appearance.Font = new System.Drawing.Font("Tahoma", 7f, System.Drawing.FontStyle.Bold);
		this.lbl_Copy.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.lbl_Copy.Location = new System.Drawing.Point(5, 146);
		this.lbl_Copy.Name = "lbl_Copy";
		this.lbl_Copy.Size = new System.Drawing.Size(123, 11);
		this.lbl_Copy.TabIndex = 11;
		this.lbl_Copy.Text = "Copyright © 2012-2016";
		this.Processing.EditValue = "";
		this.Processing.Location = new System.Drawing.Point(5, 122);
		this.Processing.Name = "Processing";
		this.Processing.Properties.Appearance.ForeColor = System.Drawing.SystemColors.Window;
		this.Processing.Properties.MarqueeAnimationSpeed = 30;
		this.Processing.Size = new System.Drawing.Size(295, 12);
		this.Processing.TabIndex = 10;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(305, 172);
		base.Controls.Add(this.pic_Apzon);
		base.Controls.Add(this.lbl_Start);
		base.Controls.Add(this.lbl_Copy);
		base.Controls.Add(this.Processing);
		base.Name = "SplashScreenApplication";
		this.Text = "Form1";
		base.Load += new System.EventHandler(this.SplashScreenApplication_Load);
		((System.ComponentModel.ISupportInitialize)this.pic_Apzon.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Processing.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
