using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucPowQuantity : XtraUserControl
{
	public Func<double, bool> SetQty;

	private IContainer components;

	private KeyboardSimpleButton btn_X;

	private KeyboardSimpleButton btn_2;

	private KeyboardSimpleButton btn_3;

	private KeyboardSimpleButton btn_4;

	private KeyboardSimpleButton btn_5;

	private KeyboardSimpleButton btn_6;

	private KeyboardSimpleButton btn_7;

	private KeyboardSimpleButton btn_8;

	private KeyboardSimpleButton btn_9;

	private TableLayoutPanel tableLayoutPanel1;

	public ucPowQuantity()
	{
		InitializeComponent();
	}

	private void ucPowQuantity_Load(object sender, EventArgs e)
	{
		foreach (Control control in tableLayoutPanel1.Controls)
		{
			if (control is SimpleButton simpleButton)
			{
				simpleButton.Click += ButtonOnClick;
			}
		}
	}

	private void ButtonOnClick(object sender, EventArgs eventArgs)
	{
		try
		{
			if (SetQty != null && sender is SimpleButton simpleButton)
			{
				SetQty(Function.ParseDouble(simpleButton.Name.Split('_')[1]));
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
		this.btn_X = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_2 = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_3 = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_4 = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_5 = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_6 = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_7 = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_8 = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.btn_9 = new Apzon.Forms.UserControls.POS_UI.KeyboardSimpleButton();
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.tableLayoutPanel1.SuspendLayout();
		base.SuspendLayout();
		this.btn_X.Appearance.BackColor = System.Drawing.Color.FromArgb(250, 192, 144);
		this.btn_X.Appearance.Font = new System.Drawing.Font("Tahoma", 29.6439f);
		this.btn_X.Appearance.Options.UseBackColor = true;
		this.btn_X.Appearance.Options.UseFont = true;
		this.btn_X.Dock = System.Windows.Forms.DockStyle.Fill;
		this.btn_X.Location = new System.Drawing.Point(3, 3);
		this.btn_X.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_X.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_X.Name = "btn_X";
		this.btn_X.Size = new System.Drawing.Size(94, 63);
		this.btn_X.TabIndex = 0;
		this.btn_X.Text = "X";
		this.btn_2.Appearance.BackColor = System.Drawing.Color.FromArgb(250, 192, 144);
		this.btn_2.Appearance.Font = new System.Drawing.Font("Tahoma", 29.6439f);
		this.btn_2.Appearance.Options.UseBackColor = true;
		this.btn_2.Appearance.Options.UseFont = true;
		this.btn_2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.btn_2.Location = new System.Drawing.Point(3, 72);
		this.btn_2.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_2.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_2.Name = "btn_2";
		this.btn_2.Size = new System.Drawing.Size(94, 63);
		this.btn_2.TabIndex = 1;
		this.btn_2.Text = "2";
		this.btn_3.Appearance.BackColor = System.Drawing.Color.FromArgb(250, 192, 144);
		this.btn_3.Appearance.Font = new System.Drawing.Font("Tahoma", 29.6439f);
		this.btn_3.Appearance.Options.UseBackColor = true;
		this.btn_3.Appearance.Options.UseFont = true;
		this.btn_3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.btn_3.Location = new System.Drawing.Point(3, 141);
		this.btn_3.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_3.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_3.Name = "btn_3";
		this.btn_3.Size = new System.Drawing.Size(94, 63);
		this.btn_3.TabIndex = 2;
		this.btn_3.Text = "3";
		this.btn_4.Appearance.BackColor = System.Drawing.Color.FromArgb(250, 192, 144);
		this.btn_4.Appearance.Font = new System.Drawing.Font("Tahoma", 29.6439f);
		this.btn_4.Appearance.Options.UseBackColor = true;
		this.btn_4.Appearance.Options.UseFont = true;
		this.btn_4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.btn_4.Location = new System.Drawing.Point(3, 210);
		this.btn_4.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_4.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_4.Name = "btn_4";
		this.btn_4.Size = new System.Drawing.Size(94, 63);
		this.btn_4.TabIndex = 3;
		this.btn_4.Text = "4";
		this.btn_5.Appearance.BackColor = System.Drawing.Color.FromArgb(250, 192, 144);
		this.btn_5.Appearance.Font = new System.Drawing.Font("Tahoma", 29.6439f);
		this.btn_5.Appearance.Options.UseBackColor = true;
		this.btn_5.Appearance.Options.UseFont = true;
		this.btn_5.Dock = System.Windows.Forms.DockStyle.Fill;
		this.btn_5.Location = new System.Drawing.Point(3, 279);
		this.btn_5.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_5.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_5.Name = "btn_5";
		this.btn_5.Size = new System.Drawing.Size(94, 63);
		this.btn_5.TabIndex = 4;
		this.btn_5.Text = "5";
		this.btn_6.Appearance.BackColor = System.Drawing.Color.FromArgb(250, 192, 144);
		this.btn_6.Appearance.Font = new System.Drawing.Font("Tahoma", 29.6439f);
		this.btn_6.Appearance.Options.UseBackColor = true;
		this.btn_6.Appearance.Options.UseFont = true;
		this.btn_6.Dock = System.Windows.Forms.DockStyle.Fill;
		this.btn_6.Location = new System.Drawing.Point(3, 348);
		this.btn_6.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_6.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_6.Name = "btn_6";
		this.btn_6.Size = new System.Drawing.Size(94, 63);
		this.btn_6.TabIndex = 5;
		this.btn_6.Text = "6";
		this.btn_7.Appearance.BackColor = System.Drawing.Color.FromArgb(250, 192, 144);
		this.btn_7.Appearance.Font = new System.Drawing.Font("Tahoma", 29.6439f);
		this.btn_7.Appearance.Options.UseBackColor = true;
		this.btn_7.Appearance.Options.UseFont = true;
		this.btn_7.Dock = System.Windows.Forms.DockStyle.Fill;
		this.btn_7.Location = new System.Drawing.Point(3, 417);
		this.btn_7.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_7.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_7.Name = "btn_7";
		this.btn_7.Size = new System.Drawing.Size(94, 63);
		this.btn_7.TabIndex = 6;
		this.btn_7.Text = "7";
		this.btn_8.Appearance.BackColor = System.Drawing.Color.FromArgb(250, 192, 144);
		this.btn_8.Appearance.Font = new System.Drawing.Font("Tahoma", 29.6439f);
		this.btn_8.Appearance.Options.UseBackColor = true;
		this.btn_8.Appearance.Options.UseFont = true;
		this.btn_8.Dock = System.Windows.Forms.DockStyle.Fill;
		this.btn_8.Location = new System.Drawing.Point(3, 486);
		this.btn_8.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_8.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_8.Name = "btn_8";
		this.btn_8.Size = new System.Drawing.Size(94, 63);
		this.btn_8.TabIndex = 7;
		this.btn_8.Text = "8";
		this.btn_9.Appearance.BackColor = System.Drawing.Color.FromArgb(250, 192, 144);
		this.btn_9.Appearance.Font = new System.Drawing.Font("Tahoma", 29.6439f);
		this.btn_9.Appearance.Options.UseBackColor = true;
		this.btn_9.Appearance.Options.UseFont = true;
		this.btn_9.Dock = System.Windows.Forms.DockStyle.Fill;
		this.btn_9.Location = new System.Drawing.Point(3, 555);
		this.btn_9.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_9.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_9.Name = "btn_9";
		this.btn_9.Size = new System.Drawing.Size(94, 63);
		this.btn_9.TabIndex = 8;
		this.btn_9.Text = "9";
		this.tableLayoutPanel1.ColumnCount = 1;
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20f));
		this.tableLayoutPanel1.Controls.Add(this.btn_X, 0, 0);
		this.tableLayoutPanel1.Controls.Add(this.btn_9, 0, 8);
		this.tableLayoutPanel1.Controls.Add(this.btn_2, 0, 1);
		this.tableLayoutPanel1.Controls.Add(this.btn_8, 0, 7);
		this.tableLayoutPanel1.Controls.Add(this.btn_3, 0, 2);
		this.tableLayoutPanel1.Controls.Add(this.btn_7, 0, 6);
		this.tableLayoutPanel1.Controls.Add(this.btn_4, 0, 3);
		this.tableLayoutPanel1.Controls.Add(this.btn_6, 0, 5);
		this.tableLayoutPanel1.Controls.Add(this.btn_5, 0, 4);
		this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		this.tableLayoutPanel1.RowCount = 10;
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10f));
		this.tableLayoutPanel1.Size = new System.Drawing.Size(100, 695);
		this.tableLayoutPanel1.TabIndex = 9;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.tableLayoutPanel1);
		base.Name = "ucPowQuantity";
		base.Size = new System.Drawing.Size(100, 695);
		base.Load += new System.EventHandler(this.ucPowQuantity_Load);
		this.tableLayoutPanel1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
