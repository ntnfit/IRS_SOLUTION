using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucStatusMessage : XtraUserControl
{
	private MessageBoxIcon _icon;

	private string _message;

	private IContainer components;

	private Panel panel_Caption;

	private RichTextBox rtb_Content;

	private SimpleButton btn_Icon;

	private ImageList iml_Icon;

	public MessageBoxIcon MessageBoxIcon
	{
		get
		{
			return _icon;
		}
		set
		{
			_icon = value;
			switch (_icon)
			{
			case MessageBoxIcon.Hand:
				btn_Icon.ImageIndex = 0;
				rtb_Content.ForeColor = Color.DarkRed;
				break;
			case MessageBoxIcon.Question:
				btn_Icon.ImageIndex = 3;
				break;
			case MessageBoxIcon.Exclamation:
				btn_Icon.ImageIndex = 5;
				rtb_Content.ForeColor = Color.DarkOrange;
				break;
			case MessageBoxIcon.Asterisk:
				btn_Icon.ImageIndex = 6;
				rtb_Content.ForeColor = Color.DarkGreen;
				break;
			}
		}
	}

	public string Message
	{
		get
		{
			return _message;
		}
		set
		{
			_message = value;
			rtb_Content.Text = _message;
		}
	}

	public ucStatusMessage()
	{
		InitializeComponent();
	}

	public ucStatusMessage(string message, MessageBoxIcon icon)
	{
		InitializeComponent();
		_message = message;
		_icon = icon;
	}

	private void uc_StatusMessage_Load(object sender, EventArgs e)
	{
		try
		{
			switch (_icon)
			{
			case MessageBoxIcon.Hand:
				btn_Icon.ImageIndex = 0;
				break;
			case MessageBoxIcon.Question:
				btn_Icon.ImageIndex = 3;
				break;
			case MessageBoxIcon.Exclamation:
				btn_Icon.ImageIndex = 5;
				break;
			case MessageBoxIcon.Asterisk:
				btn_Icon.ImageIndex = 6;
				break;
			}
			rtb_Content.Text = _message;
		}
		catch (Exception)
		{
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.UserControls.POS_UI.ucStatusMessage));
		this.panel_Caption = new System.Windows.Forms.Panel();
		this.rtb_Content = new System.Windows.Forms.RichTextBox();
		this.btn_Icon = new DevExpress.XtraEditors.SimpleButton();
		this.iml_Icon = new System.Windows.Forms.ImageList(this.components);
		this.panel_Caption.SuspendLayout();
		base.SuspendLayout();
		this.panel_Caption.BackColor = System.Drawing.Color.WhiteSmoke;
		this.panel_Caption.Controls.Add(this.rtb_Content);
		this.panel_Caption.Controls.Add(this.btn_Icon);
		this.panel_Caption.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panel_Caption.Font = new System.Drawing.Font("Microsoft Sans Serif", 20f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.panel_Caption.ForeColor = System.Drawing.Color.White;
		this.panel_Caption.Location = new System.Drawing.Point(0, 0);
		this.panel_Caption.Name = "panel_Caption";
		this.panel_Caption.Size = new System.Drawing.Size(737, 50);
		this.panel_Caption.TabIndex = 1;
		this.rtb_Content.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.rtb_Content.BackColor = System.Drawing.Color.WhiteSmoke;
		this.rtb_Content.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.rtb_Content.Font = new System.Drawing.Font("Microsoft Sans Serif", 20f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.rtb_Content.ForeColor = System.Drawing.Color.White;
		this.rtb_Content.Location = new System.Drawing.Point(40, 4);
		this.rtb_Content.Name = "rtb_Content";
		this.rtb_Content.ReadOnly = true;
		this.rtb_Content.Size = new System.Drawing.Size(696, 43);
		this.rtb_Content.TabIndex = 0;
		this.rtb_Content.Text = "";
		this.btn_Icon.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.btn_Icon.Appearance.Options.UseBackColor = true;
		this.btn_Icon.AppearanceDisabled.BackColor = System.Drawing.Color.Transparent;
		this.btn_Icon.AppearanceDisabled.Options.UseBackColor = true;
		this.btn_Icon.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.btn_Icon.ImageList = this.iml_Icon;
		this.btn_Icon.Location = new System.Drawing.Point(4, 4);
		this.btn_Icon.Name = "btn_Icon";
		this.btn_Icon.Size = new System.Drawing.Size(28, 28);
		this.btn_Icon.TabIndex = 1;
		this.iml_Icon.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("iml_Icon.ImageStream");
		this.iml_Icon.TransparentColor = System.Drawing.Color.Transparent;
		this.iml_Icon.Images.SetKeyName(0, "error.png");
		this.iml_Icon.Images.SetKeyName(1, "hand.png");
		this.iml_Icon.Images.SetKeyName(2, "stop.png");
		this.iml_Icon.Images.SetKeyName(3, "question.png");
		this.iml_Icon.Images.SetKeyName(4, "exclamation-sign.png");
		this.iml_Icon.Images.SetKeyName(5, "warning.png");
		this.iml_Icon.Images.SetKeyName(6, "info.png");
		this.iml_Icon.Images.SetKeyName(7, "asterisk.png");
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.panel_Caption);
		base.Name = "uc_StatusMessage";
		base.Size = new System.Drawing.Size(737, 50);
		base.Load += new System.EventHandler(this.uc_StatusMessage_Load);
		this.panel_Caption.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
