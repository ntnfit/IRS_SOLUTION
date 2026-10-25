using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;

namespace Apzon.Forms.UserControls.POS_UI;

public class RetailMessageBoxModal : Form
{
	private readonly string _caption;

	private readonly string _message;

	private readonly MessageBoxButtons _buttons;

	private readonly MessageBoxDefaultButton _defaultButton;

	private MessageBoxIcon _icon;

	private IContainer components;

	public RetailMessageBoxModal()
	{
		InitializeComponent();
	}

	public RetailMessageBoxModal(string message = "", string caption = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1, MessageBoxIcon icon = MessageBoxIcon.Asterisk)
	{
		InitializeComponent();
		_caption = caption;
		_message = message;
		_buttons = buttons;
		_defaultButton = defaultButton;
		_icon = icon;
	}

	private void RetailMessageBox_Load(object sender, EventArgs e)
	{
		try
		{
			Opacity = 0.25;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void RetailMessageBox_Shown(object sender, EventArgs e)
	{
		try
		{
			RetailMessageBoxContent retailMessageBoxContent = new RetailMessageBoxContent(_message, _caption, _buttons, _defaultButton, _icon);
			DialogResult = retailMessageBoxContent.ShowDialog();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
		base.SuspendLayout();
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
		base.ClientSize = new System.Drawing.Size(691, 467);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		base.Name = "RetailMessageBox";
		base.ShowIcon = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "RetailMessageBox";
		base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
		base.Load += new System.EventHandler(this.RetailMessageBox_Load);
		base.Shown += new System.EventHandler(this.RetailMessageBox_Shown);
		base.ResumeLayout(false);
	}
}
