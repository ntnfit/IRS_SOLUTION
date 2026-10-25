using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraWaitForm;

namespace Apzon.Controls;

public class Processing : WaitForm
{
	public enum WaitFormCommand
	{

	}

	private IContainer components;

	private ProgressPanel progressPanel_Loading;

	private TableLayoutPanel tableLayoutPanel1;

	public Processing()
	{
		InitializeComponent();
		progressPanel_Loading.AutoHeight = true;
	}

	public override void SetCaption(string caption)
	{
		base.SetCaption(caption);
		progressPanel_Loading.Caption = caption;
	}

	public override void SetDescription(string description)
	{
		base.SetDescription(description);
		progressPanel_Loading.Description = description;
	}

	public override void ProcessCommand(Enum cmd, object arg)
	{
		base.ProcessCommand(cmd, arg);
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
		this.progressPanel_Loading = new DevExpress.XtraWaitForm.ProgressPanel();
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.tableLayoutPanel1.SuspendLayout();
		base.SuspendLayout();
		this.progressPanel_Loading.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.progressPanel_Loading.Appearance.Options.UseBackColor = true;
		this.progressPanel_Loading.AppearanceCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f);
		this.progressPanel_Loading.AppearanceCaption.Options.UseFont = true;
		this.progressPanel_Loading.AppearanceDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.progressPanel_Loading.AppearanceDescription.Options.UseFont = true;
		this.progressPanel_Loading.Caption = "Processing ...";
		this.progressPanel_Loading.CaptionToDescriptionDistance = 2;
		this.progressPanel_Loading.Description = "Please Wait";
		this.progressPanel_Loading.Dock = System.Windows.Forms.DockStyle.Fill;
		this.progressPanel_Loading.ImageHorzOffset = 20;
		this.progressPanel_Loading.Location = new System.Drawing.Point(0, 17);
		this.progressPanel_Loading.LookAndFeel.SkinName = "Liquid Sky";
		this.progressPanel_Loading.LookAndFeel.UseDefaultLookAndFeel = false;
		this.progressPanel_Loading.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
		this.progressPanel_Loading.Name = "progressPanel_Loading";
		this.progressPanel_Loading.Size = new System.Drawing.Size(246, 39);
		this.progressPanel_Loading.TabIndex = 0;
		this.progressPanel_Loading.TextHorzOffset = 10;
		this.tableLayoutPanel1.AutoSize = true;
		this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
		this.tableLayoutPanel1.ColumnCount = 1;
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 220f));
		this.tableLayoutPanel1.Controls.Add(this.progressPanel_Loading, 0, 0);
		this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 14, 0, 14);
		this.tableLayoutPanel1.RowCount = 1;
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100f));
		this.tableLayoutPanel1.Size = new System.Drawing.Size(246, 73);
		this.tableLayoutPanel1.TabIndex = 1;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.AutoSize = true;
		base.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		base.ClientSize = new System.Drawing.Size(246, 73);
		base.Controls.Add(this.tableLayoutPanel1);
		this.DoubleBuffered = true;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		this.MinimumSize = new System.Drawing.Size(246, 0);
		base.Name = "Processing";
		base.ShowIcon = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Processing...";
		this.tableLayoutPanel1.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
