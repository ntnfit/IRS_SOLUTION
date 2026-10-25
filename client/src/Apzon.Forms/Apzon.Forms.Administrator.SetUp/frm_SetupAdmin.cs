using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Global;
using Apzon.HttpClientHelper;

namespace Apzon.Forms.Administrator.SetUp;

public class frm_SetupAdmin : XtraFormEx
{
	private IContainer components;

	private LabelLine lbName;

	private LabelLine lbOldPass;

	private LabelLine lbNewPass;

	private LabelLine lbConfirm;

	private TextEditEx txtUser;

	private TextEditEx txtOldPass;

	private TextEditEx txtNewPass;

	private TextEditEx txtConfirm;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnSave;

	public frm_SetupAdmin()
	{
		if (GlobalData.UserName.ToLower().Equals(GlobalData.SuperUser))
		{
			InitializeComponent();
			return;
		}
		throw new Exception("User have not permision!");
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btnSave_Click(object sender, EventArgs e)
	{
		try
		{
			await Save();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_SetupAdmin_Load(object sender, EventArgs e)
	{
		try
		{
			txtUser.Text = GlobalData.SuperUser;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Save()
	{
		try
		{
			if (string.IsNullOrWhiteSpace(Function.ToString(txtUser.EditValue)))
			{
				WinCommons.Error(Enums.MessageCode.UserMustNotBeNull, "");
			}
			else if (string.IsNullOrWhiteSpace(Function.ToString(txtOldPass.EditValue)))
			{
				WinCommons.Error(Enums.MessageCode.OldPassMustNotBeNull, "");
			}
			else if (string.IsNullOrWhiteSpace(Function.ToString(txtNewPass.EditValue)))
			{
				WinCommons.Error(Enums.MessageCode.NewPassMustNotBeNull, "");
			}
			else if (Function.ToString(txtNewPass.EditValue).Length < 6)
			{
				WinCommons.Error(Enums.MessageCode.MinLengthPass, "");
			}
			else if (string.IsNullOrWhiteSpace(Function.ToString(txtConfirm.EditValue)))
			{
				WinCommons.Error(Enums.MessageCode.ConfirmPassMustnotBeNull, "");
			}
			else if (Function.ToString(txtConfirm.EditValue) != Function.ToString(txtNewPass.EditValue))
			{
				WinCommons.Error(Enums.MessageCode.ConfirmPassIsNotTheSameNewPass, "");
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmLogoutAllUser, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
				{
					return;
				}
				using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "Users");
				string text = await http.PostAsync("SaveAdmin", new Tuple<string, string, string>(Function.ToString(txtUser.EditValue), Function.ToString(txtOldPass.EditValue), Function.ToString(txtNewPass.EditValue)));
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					Close();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.SetUp.frm_SetupAdmin));
		this.lbName = new Apzon.Controls.Controls.LabelLine();
		this.lbOldPass = new Apzon.Controls.Controls.LabelLine();
		this.lbNewPass = new Apzon.Controls.Controls.LabelLine();
		this.lbConfirm = new Apzon.Controls.Controls.LabelLine();
		this.txtUser = new Apzon.Controls.Editor.TextEditEx();
		this.txtOldPass = new Apzon.Controls.Editor.TextEditEx();
		this.txtNewPass = new Apzon.Controls.Editor.TextEditEx();
		this.txtConfirm = new Apzon.Controls.Editor.TextEditEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnSave = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.txtUser.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtOldPass.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtNewPass.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtConfirm.Properties).BeginInit();
		base.SuspendLayout();
		this.lbName.FieldName = null;
		this.lbName.HideWhenEmpty = false;
		this.lbName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbName.Location = new System.Drawing.Point(22, 12);
		this.lbName.Name = "lbName";
		this.lbName.ShowUnderLine = true;
		this.lbName.Size = new System.Drawing.Size(105, 20);
		this.lbName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbName.TabIndex = 0;
		this.lbName.Utext = "Admin User";
		this.lbOldPass.FieldName = null;
		this.lbOldPass.HideWhenEmpty = false;
		this.lbOldPass.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbOldPass.Location = new System.Drawing.Point(22, 38);
		this.lbOldPass.Name = "lbOldPass";
		this.lbOldPass.ShowUnderLine = true;
		this.lbOldPass.Size = new System.Drawing.Size(105, 20);
		this.lbOldPass.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbOldPass.TabIndex = 1;
		this.lbOldPass.Utext = "Old Password";
		this.lbNewPass.FieldName = null;
		this.lbNewPass.HideWhenEmpty = false;
		this.lbNewPass.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbNewPass.Location = new System.Drawing.Point(22, 64);
		this.lbNewPass.Name = "lbNewPass";
		this.lbNewPass.ShowUnderLine = true;
		this.lbNewPass.Size = new System.Drawing.Size(105, 20);
		this.lbNewPass.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbNewPass.TabIndex = 2;
		this.lbNewPass.Utext = "New Password";
		this.lbConfirm.FieldName = null;
		this.lbConfirm.HideWhenEmpty = false;
		this.lbConfirm.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbConfirm.Location = new System.Drawing.Point(22, 90);
		this.lbConfirm.Name = "lbConfirm";
		this.lbConfirm.ShowUnderLine = true;
		this.lbConfirm.Size = new System.Drawing.Size(105, 20);
		this.lbConfirm.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbConfirm.TabIndex = 3;
		this.lbConfirm.Utext = "Confirm PassWord";
		this.txtUser.FieldName = null;
		this.txtUser.IsSearchControl = false;
		this.txtUser.Location = new System.Drawing.Point(125, 9);
		this.txtUser.Name = "txtUser";
		this.txtUser.Properties.AppearanceFocused.BackColor = System.Drawing.Color.Khaki;
		this.txtUser.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtUser.Size = new System.Drawing.Size(202, 20);
		this.txtUser.TabIndex = 4;
		this.txtOldPass.FieldName = null;
		this.txtOldPass.IsSearchControl = false;
		this.txtOldPass.Location = new System.Drawing.Point(125, 35);
		this.txtOldPass.Name = "txtOldPass";
		this.txtOldPass.Properties.AppearanceFocused.BackColor = System.Drawing.Color.Khaki;
		this.txtOldPass.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtOldPass.Properties.PasswordChar = '*';
		this.txtOldPass.Size = new System.Drawing.Size(202, 20);
		this.txtOldPass.TabIndex = 5;
		this.txtNewPass.FieldName = null;
		this.txtNewPass.IsSearchControl = false;
		this.txtNewPass.Location = new System.Drawing.Point(125, 61);
		this.txtNewPass.Name = "txtNewPass";
		this.txtNewPass.Properties.AppearanceFocused.BackColor = System.Drawing.Color.Khaki;
		this.txtNewPass.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtNewPass.Properties.PasswordChar = '*';
		this.txtNewPass.Size = new System.Drawing.Size(202, 20);
		this.txtNewPass.TabIndex = 6;
		this.txtConfirm.FieldName = null;
		this.txtConfirm.IsSearchControl = false;
		this.txtConfirm.Location = new System.Drawing.Point(125, 87);
		this.txtConfirm.Name = "txtConfirm";
		this.txtConfirm.Properties.AppearanceFocused.BackColor = System.Drawing.Color.Khaki;
		this.txtConfirm.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtConfirm.Properties.PasswordChar = '*';
		this.txtConfirm.Size = new System.Drawing.Size(202, 20);
		this.txtConfirm.TabIndex = 7;
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(252, 116);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 20);
		this.btnCancel.TabIndex = 8;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnSave.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btnSave.FieldName = null;
		this.btnSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSave.ImageOptions.Image");
		this.btnSave.IsSearchControl = false;
		this.btnSave.Location = new System.Drawing.Point(181, 116);
		this.btnSave.Name = "btnSave";
		this.btnSave.Size = new System.Drawing.Size(67, 20);
		this.btnSave.TabIndex = 9;
		this.btnSave.Text = "Save";
		this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(345, 145);
		base.Controls.Add(this.btnSave);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.txtConfirm);
		base.Controls.Add(this.txtNewPass);
		base.Controls.Add(this.txtOldPass);
		base.Controls.Add(this.txtUser);
		base.Controls.Add(this.lbConfirm);
		base.Controls.Add(this.lbNewPass);
		base.Controls.Add(this.lbOldPass);
		base.Controls.Add(this.lbName);
		base.Name = "frm_SetupAdmin";
		base.ShowIcon = false;
		this.Text = "Change Password Administrator";
		base.Load += new System.EventHandler(this.frm_SetupAdmin_Load);
		((System.ComponentModel.ISupportInitialize)this.txtUser.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtOldPass.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtNewPass.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtConfirm.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
