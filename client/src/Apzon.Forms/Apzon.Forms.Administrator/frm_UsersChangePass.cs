using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;

namespace Apzon.Forms.Administrator;

public class frm_UsersChangePass : XtraFormEx
{
	public string Password = "";

	public bool IsChanged;

	private IContainer components;

	private Label label1;

	private Label label2;

	private Label label3;

	private Label label4;

	private Label label5;

	private Label label6;

	private TextEditEx textEditEx1;

	private TextEditEx txt_Password;

	private TextEditEx txt_Confirm;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	public frm_UsersChangePass()
	{
		InitializeComponent();
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (Function.ToString(txt_Password.Text).ToLower() == "abcd")
			{
				WinCommons.Error(Enums.MessageCode.PasswordMustBeDifferentFromPasswordExample, "");
				return;
			}
			if (txt_Password.Text != txt_Confirm.Text)
			{
				WinCommons.Error(Enums.MessageCode.ConfirmationFailedTypeAgain, "");
				return;
			}
			if (btn_OK.Text.ToLower() == "ok")
			{
				Close();
				return;
			}
			IsChanged = true;
			Password = txt_Password.Text;
			btn_OK.Text = "OK";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void txt_Password_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (btn_OK.Text.ToLower() == "ok")
			{
				btn_OK.Text = "Update";
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_UsersChangePass));
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.label4 = new System.Windows.Forms.Label();
		this.label5 = new System.Windows.Forms.Label();
		this.label6 = new System.Windows.Forms.Label();
		this.textEditEx1 = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Password = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Confirm = new Apzon.Controls.Editor.TextEditEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.textEditEx1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Password.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Confirm.Properties).BeginInit();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(16, 19);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(370, 13);
		this.label1.TabIndex = 0;
		this.label1.Text = "Choose a new password that complies with your company's password policy";
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(16, 36);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(142, 13);
		this.label2.TabIndex = 1;
		this.label2.Text = "The password must contain:";
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(16, 53);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(176, 13);
		this.label3.TabIndex = 2;
		this.label3.Text = "Minimum of 4 Characters in Length.";
		this.label4.AutoSize = true;
		this.label4.Location = new System.Drawing.Point(16, 114);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(100, 13);
		this.label4.TabIndex = 3;
		this.label4.Text = "Password Example:";
		this.label5.AutoSize = true;
		this.label5.Location = new System.Drawing.Point(16, 165);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(53, 13);
		this.label5.TabIndex = 4;
		this.label5.Text = "Password";
		this.label6.AutoSize = true;
		this.label6.Location = new System.Drawing.Point(16, 187);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(44, 13);
		this.label6.TabIndex = 5;
		this.label6.Text = "Confirm";
		this.textEditEx1.EditValue = "abcd";
		this.textEditEx1.Enabled = false;
		this.textEditEx1.FieldName = null;
		this.textEditEx1.Location = new System.Drawing.Point(122, 111);
		this.textEditEx1.Name = "textEditEx1";
		this.textEditEx1.Size = new System.Drawing.Size(100, 20);
		this.textEditEx1.TabIndex = 6;
		this.txt_Password.FieldName = null;
		this.txt_Password.Location = new System.Drawing.Point(122, 158);
		this.txt_Password.Name = "txt_Password";
		this.txt_Password.Properties.PasswordChar = '*';
		this.txt_Password.Size = new System.Drawing.Size(100, 20);
		this.txt_Password.TabIndex = 7;
		this.txt_Password.EditValueChanged += new System.EventHandler(this.txt_Password_EditValueChanged);
		this.txt_Confirm.FieldName = null;
		this.txt_Confirm.Location = new System.Drawing.Point(122, 181);
		this.txt_Confirm.Name = "txt_Confirm";
		this.txt_Confirm.Properties.PasswordChar = '*';
		this.txt_Confirm.Size = new System.Drawing.Size(100, 20);
		this.txt_Confirm.TabIndex = 8;
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.Location = new System.Drawing.Point(108, 222);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(83, 25);
		this.btn_Cancel.TabIndex = 14;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Add;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.Location = new System.Drawing.Point(19, 222);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(83, 25);
		this.btn_OK.TabIndex = 13;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(413, 261);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.txt_Confirm);
		base.Controls.Add(this.txt_Password);
		base.Controls.Add(this.textEditEx1);
		base.Controls.Add(this.label6);
		base.Controls.Add(this.label5);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.label1);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.Name = "frm_UsersChangePass";
		base.ShowIcon = false;
		this.Text = "Password";
		((System.ComponentModel.ISupportInitialize)this.textEditEx1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Password.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Confirm.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
