using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Controls.Controls;
using Apzon.Forms.Properties;
using DevExpress.Utils;
using DevExpress.XtraEditors;

namespace Apzon.Forms.Service;

public class frm_ConfirmDialog : XtraForm
{
	public string EventSubject;

	private int _type;

	private IContainer components;

	private LabelControl labelControl1;

	private PictureBox pictureBox1;

	public SimpleButtonEx btn_Cancel;

	public SimpleButtonEx btn_OK;

	private RadioButton rad_thisOccur;

	private RadioButton rad_allOccur;

	private RadioButton rad_unchanged;

	private int _value { get; set; }

	public frm_ConfirmDialog()
	{
		InitializeComponent();
	}

	public frm_ConfirmDialog(int type)
	{
		InitializeComponent();
		_type = type;
	}

	private void frm_ConfirmDialog_Load(object sender, EventArgs e)
	{
		if (_type == 1)
		{
			Text = "Change Confirm";
			labelControl1.Text = "Do you want to Change all occurrences of recurring event '" + EventSubject + "' , or just this one?";
			rad_allOccur.Text = "Change all in series";
			rad_thisOccur.Text = "Change this occurrence";
			rad_unchanged.Visible = false;
		}
		else
		{
			Text = "Delete Confirm";
			labelControl1.Text = "Do you want to delete all occurrences of recurring event '" + EventSubject + "' , or just this one?";
			rad_allOccur.Text = "Delete the series";
			rad_thisOccur.Text = "Delete this occurrence";
			rad_unchanged.Text = "Delete only unchanged occurrence";
			rad_unchanged.Visible = true;
		}
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		DialogResult = DialogResult.OK;
		Close();
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		DialogResult = DialogResult.Cancel;
		Close();
	}

	public int GetValue()
	{
		if (rad_thisOccur.Checked)
		{
			_value = 1;
		}
		else if (rad_allOccur.Checked)
		{
			_value = 2;
		}
		else if (rad_unchanged.Checked)
		{
			_value = 3;
		}
		return _value;
	}

	private void frm_ConfirmDialog_Shown(object sender, EventArgs e)
	{
		btn_Cancel.Focus();
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
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		this.rad_thisOccur = new System.Windows.Forms.RadioButton();
		this.rad_allOccur = new System.Windows.Forms.RadioButton();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.rad_unchanged = new System.Windows.Forms.RadioButton();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
		base.SuspendLayout();
		this.labelControl1.Appearance.Options.UseTextOptions = true;
		this.labelControl1.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.labelControl1.AppearanceDisabled.Options.UseTextOptions = true;
		this.labelControl1.AppearanceDisabled.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.labelControl1.AppearanceHovered.Options.UseTextOptions = true;
		this.labelControl1.AppearanceHovered.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.labelControl1.AppearancePressed.Options.UseTextOptions = true;
		this.labelControl1.AppearancePressed.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.labelControl1.Location = new System.Drawing.Point(52, 4);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(285, 55);
		this.labelControl1.TabIndex = 0;
		this.labelControl1.Text = "labelControl1";
		this.pictureBox1.Image = Apzon.Forms.Properties.Resources.dialog_icon;
		this.pictureBox1.Location = new System.Drawing.Point(13, 17);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(33, 33);
		this.pictureBox1.TabIndex = 1;
		this.pictureBox1.TabStop = false;
		this.rad_thisOccur.AutoSize = true;
		this.rad_thisOccur.Checked = true;
		this.rad_thisOccur.Location = new System.Drawing.Point(86, 55);
		this.rad_thisOccur.Name = "rad_thisOccur";
		this.rad_thisOccur.Size = new System.Drawing.Size(132, 17);
		this.rad_thisOccur.TabIndex = 19;
		this.rad_thisOccur.TabStop = true;
		this.rad_thisOccur.Text = "Delete this occurrence";
		this.rad_thisOccur.UseVisualStyleBackColor = true;
		this.rad_allOccur.AutoSize = true;
		this.rad_allOccur.Location = new System.Drawing.Point(86, 78);
		this.rad_allOccur.Name = "rad_allOccur";
		this.rad_allOccur.Size = new System.Drawing.Size(106, 17);
		this.rad_allOccur.TabIndex = 20;
		this.rad_allOccur.Text = "Delete the series";
		this.rad_allOccur.UseVisualStyleBackColor = true;
		this.btn_Cancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Cancel.Appearance.Options.UseBackColor = true;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.Location = new System.Drawing.Point(179, 122);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(90, 25);
		this.btn_Cancel.TabIndex = 18;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_OK.Appearance.Options.UseBackColor = true;
		this.btn_OK.Location = new System.Drawing.Point(83, 122);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(90, 25);
		this.btn_OK.TabIndex = 17;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.rad_unchanged.AutoSize = true;
		this.rad_unchanged.Location = new System.Drawing.Point(86, 101);
		this.rad_unchanged.Name = "rad_unchanged";
		this.rad_unchanged.Size = new System.Drawing.Size(135, 17);
		this.rad_unchanged.TabIndex = 21;
		this.rad_unchanged.Text = "Delete only unchanged";
		this.rad_unchanged.UseVisualStyleBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(341, 149);
		base.Controls.Add(this.rad_unchanged);
		base.Controls.Add(this.rad_allOccur);
		base.Controls.Add(this.rad_thisOccur);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.pictureBox1);
		base.Controls.Add(this.labelControl1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frm_ConfirmDialog";
		this.Text = "Confirm Delete";
		base.Load += new System.EventHandler(this.frm_ConfirmDialog_Load);
		base.Shown += new System.EventHandler(this.frm_ConfirmDialog_Shown);
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
