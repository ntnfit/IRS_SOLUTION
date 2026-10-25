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
using DevExpress.XtraEditors;

namespace Apzon.Forms.Interfaces.FormDocument;

public class frm_OpeningClosingRemarks : XtraFormEx
{
	private IContainer components;

	private MemoEditEx memo_Opening;

	private LabelLine labelLine1;

	private SplitContainerControl splitContainerControl1;

	private MemoEditEx memo_Closing;

	private LabelLine labelLine2;

	private SimpleButtonEx btn_Insert;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	public string OpeningRemarks { get; set; }

	public string ClosingRemarks { get; set; }

	public bool ActiveMemo { get; set; }

	public frm_OpeningClosingRemarks()
	{
		InitializeComponent();
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
			MemoEditEx memoEditEx = (ActiveMemo ? memo_Closing : memo_Opening);
			if (memoEditEx == null)
			{
				return;
			}
			foreach (DataRow chooseItem in frm_UserDefinedTexts2.ChooseItems)
			{
				memoEditEx.Text = memoEditEx.Text.Insert(memoEditEx.SelectionStart, Function.ToString(chooseItem["Text"]));
				memoEditEx.Select();
				memoEditEx.SelectAll();
				memoEditEx.SelectionStart = memoEditEx.Text.Length;
				memoEditEx.SelectionLength = 0;
				memoEditEx.ScrollToCaret();
			}
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
		Close();
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			OpeningRemarks = memo_Opening.Text;
			ClosingRemarks = memo_Closing.Text;
			DialogResult = DialogResult.OK;
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_OpeningClosingRemarks_Load(object sender, EventArgs e)
	{
		try
		{
			memo_Opening.Text = OpeningRemarks;
			memo_Closing.Text = ClosingRemarks;
			memo_Opening.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void memo_Opening_MouseDown(object sender, MouseEventArgs e)
	{
		ActiveMemo = false;
	}

	private void memo_Closing_MouseDown(object sender, MouseEventArgs e)
	{
		ActiveMemo = true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Interfaces.FormDocument.frm_OpeningClosingRemarks));
		this.memo_Opening = new Apzon.Controls.Editor.MemoEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.memo_Closing = new Apzon.Controls.Editor.MemoEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Insert = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.memo_Opening.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.memo_Closing.Properties).BeginInit();
		base.SuspendLayout();
		this.memo_Opening.Dock = System.Windows.Forms.DockStyle.Fill;
		this.memo_Opening.Location = new System.Drawing.Point(0, 0);
		this.memo_Opening.Name = "memo_Opening";
		this.memo_Opening.Size = new System.Drawing.Size(479, 140);
		this.memo_Opening.TabIndex = 0;
		this.memo_Opening.MouseDown += new System.Windows.Forms.MouseEventHandler(this.memo_Opening_MouseDown);
		this.labelLine1.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.labelLine1.Appearance.Options.UseFont = true;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.Location = new System.Drawing.Point(5, 6);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(93, 20);
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Opeing Remarks";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(1, 27);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.memo_Opening);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.memo_Closing);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine2);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(479, 343);
		this.splitContainerControl1.SplitterPosition = 140;
		this.splitContainerControl1.TabIndex = 3;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.memo_Closing.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.memo_Closing.Location = new System.Drawing.Point(0, 27);
		this.memo_Closing.Name = "memo_Closing";
		this.memo_Closing.Size = new System.Drawing.Size(479, 171);
		this.memo_Closing.TabIndex = 1;
		this.memo_Closing.MouseDown += new System.Windows.Forms.MouseEventHandler(this.memo_Closing_MouseDown);
		this.labelLine2.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.labelLine2.Appearance.Options.UseFont = true;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.Location = new System.Drawing.Point(3, 1);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.TabIndex = 0;
		this.labelLine2.Utext = "Closing Remarks";
		this.btn_Insert.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Insert.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Insert.ImageOptions.Image");
		this.btn_Insert.Location = new System.Drawing.Point(322, 376);
		this.btn_Insert.Name = "btn_Insert";
		this.btn_Insert.Size = new System.Drawing.Size(158, 23);
		this.btn_Insert.TabIndex = 5;
		this.btn_Insert.Text = "Insert Predefined Texts";
		this.btn_Insert.Click += new System.EventHandler(this.btn_Insert_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.Location = new System.Drawing.Point(81, 376);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 4;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_OK.Location = new System.Drawing.Point(0, 376);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 3;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(481, 404);
		base.Controls.Add(this.btn_Insert);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.labelLine1);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "frm_OpeningClosingRemarks";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Opening Closing Remarks";
		base.Load += new System.EventHandler(this.frm_OpeningClosingRemarks_Load);
		((System.ComponentModel.ISupportInitialize)this.memo_Opening.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.memo_Closing.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
