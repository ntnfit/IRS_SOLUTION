using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Forms.Properties;

namespace Apzon.Forms.Interfaces.FormDocument;

public class frm_RowTextDetails : XtraFormEx
{
	private IContainer components;

	private MemoEditEx memo_Text;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_Insert;

	public string RowText { get; set; }

	public frm_RowTextDetails()
	{
		InitializeComponent();
	}

	private void frm_RowTextDetails_Load(object sender, EventArgs e)
	{
		try
		{
			memo_Text.Text = RowText;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			RowText = memo_Text.Text;
			DialogResult = DialogResult.OK;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		DialogResult = DialogResult.Cancel;
	}

	private void memo_Text_KeyDown(object sender, KeyEventArgs e)
	{
	}

	private void btn_Insert_Click(object sender, EventArgs e)
	{
		try
		{
			frm_UserDefinedTexts frm_UserDefinedTexts2 = new frm_UserDefinedTexts();
			frm_UserDefinedTexts2.OpenToChoose = true;
			frm_UserDefinedTexts2.ShowDialog(this);
			if (frm_UserDefinedTexts2.ChooseItems == null || frm_UserDefinedTexts2.ChooseItems.Count <= 0)
			{
				return;
			}
			foreach (DataRow chooseItem in frm_UserDefinedTexts2.ChooseItems)
			{
				memo_Text.Text = memo_Text.Text.Insert(memo_Text.SelectionStart, Function.ToString(chooseItem["Text"]));
				memo_Text.Select();
				memo_Text.SelectAll();
				memo_Text.SelectionStart = memo_Text.Text.Length;
				memo_Text.SelectionLength = 0;
				memo_Text.ScrollToCaret();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_RowTextDetails_Shown(object sender, EventArgs e)
	{
		memo_Text.SelectionStart = memo_Text.Text.Length;
		memo_Text.SelectionLength = 0;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Interfaces.FormDocument.frm_RowTextDetails));
		this.memo_Text = new Apzon.Controls.Editor.MemoEditEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Insert = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.memo_Text.Properties).BeginInit();
		base.SuspendLayout();
		this.memo_Text.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.memo_Text.Location = new System.Drawing.Point(0, -1);
		this.memo_Text.Name = "memo_Text";
		this.memo_Text.Size = new System.Drawing.Size(623, 420);
		this.memo_Text.TabIndex = 0;
		this.memo_Text.KeyDown += new System.Windows.Forms.KeyEventHandler(this.memo_Text_KeyDown);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_OK.Location = new System.Drawing.Point(4, 425);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.Location = new System.Drawing.Point(85, 425);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_Insert.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Insert.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Insert.ImageOptions.Image");
		this.btn_Insert.Location = new System.Drawing.Point(465, 424);
		this.btn_Insert.Name = "btn_Insert";
		this.btn_Insert.Size = new System.Drawing.Size(158, 23);
		this.btn_Insert.TabIndex = 3;
		this.btn_Insert.Text = "Insert Predefined Texts";
		this.btn_Insert.Click += new System.EventHandler(this.btn_Insert_Click);
		base.AcceptButton = this.btn_OK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(623, 452);
		base.Controls.Add(this.btn_Insert);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.memo_Text);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "frm_RowTextDetails";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Row Text Details";
		base.Load += new System.EventHandler(this.frm_RowTextDetails_Load);
		base.Shown += new System.EventHandler(this.frm_RowTextDetails_Shown);
		((System.ComponentModel.ISupportInitialize)this.memo_Text.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
