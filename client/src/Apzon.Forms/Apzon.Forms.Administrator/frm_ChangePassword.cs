using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Newtonsoft.Json;

namespace Apzon.Forms.Administrator;

public class frm_ChangePassword : XtraFormEx
{
	public delegate void ShowForms(XtraFormEx frm);

	public bool CallFromUser;

	public string Password = "";

	public bool IsChanged;

	private bool _isLoading;

	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private LabelLine lb_Username;

	private LabelLine lb_OldPassword;

	private LabelLine lb_NewPassword;

	private LabelLine lb_RePassword;

	private TextEditEx txtName;

	private TextEditEx txtPassOld;

	private TextEditEx txtPassNew;

	private TextEditEx txtRepass;

	private SimpleButtonEx btOk;

	private SimpleButtonEx btCancel;

	private TextEditEx textEditEx1;

	private Label label4;

	private Label label3;

	private Label label2;

	private Label label1;

	public frm_ChangePassword()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ChangePassword"))
		{
			InitializeComponent();
			if (CallFromUser)
			{
				Text = "Password";
			}
			SettingLanguage();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ChangePassword" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void SettingLanguage()
	{
		Text = Resources.Titel_frmChangePassword;
		lb_Username.Utext = Resources.lb_UserName;
		lb_OldPassword.Utext = Resources.lb_OldPassword;
		lb_NewPassword.Utext = Resources.lb_NewPassword;
		lb_RePassword.Utext = Resources.lb_RePassword;
		btOk.Text = Resources.ButtonOK;
		btCancel.Text = Resources.ButtonCancel;
	}

	private void frm_ChangePassword_Load(object sender, EventArgs e)
	{
		if (CallFromUser)
		{
			lb_OldPassword.Visible = false;
			txtPassOld.Visible = false;
			txtPassNew.Focus();
		}
		else
		{
			txtPassOld.Focus();
		}
		txtName.Text = GlobalData.UserName;
		txtName.Enabled = false;
		SetEventControl(this);
	}

	private void SetEventControl(Control ctr)
	{
		foreach (Control control in ctr.Controls)
		{
			if (control is CheckEdit checkEdit)
			{
				checkEdit.CheckStateChanged += OnContentChanged;
			}
			else if (control is ComboBoxEdit comboBoxEdit)
			{
				comboBoxEdit.SelectedIndexChanged += OnContentChanged;
				comboBoxEdit.SelectedValueChanged += OnContentChanged;
				comboBoxEdit.EditValueChanged += OnContentChanged;
			}
			else if (control is LookUpEdit lookUpEdit)
			{
				lookUpEdit.EditValueChanged += OnContentChanged;
				lookUpEdit.Properties.ShowHeader = false;
				lookUpEdit.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			}
			else if (control is DateEdit dateEdit)
			{
				dateEdit.EditValueChanged += OnContentChanged;
			}
			else if (control is TextEdit || control is MemoEdit)
			{
				control.TextChanged += OnContentChanged;
			}
			else if (control is SeparatorComboBox)
			{
				((SeparatorComboBox)control).SelectedIndexChanged += OnContentChanged;
			}
			else if (control is RadioButton)
			{
				((RadioButton)control).CheckedChanged += OnContentChanged;
			}
			else
			{
				SetEventControl(control);
			}
		}
	}

	protected void OnContentChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				btOk.Text = Resources.ButtonUpdate;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btOk_Click(object sender, EventArgs e)
	{
		try
		{
			if (btOk.Text != Resources.ButtonOK)
			{
				btOk.Enabled = false;
				btCancel.Enabled = false;
				await Save();
				btOk.Enabled = true;
				btCancel.Enabled = true;
			}
			else
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Save()
	{
		if (!CheckNewPass())
		{
			return;
		}
		if (CallFromUser)
		{
			IsChanged = true;
			Password = txtPassNew.Text;
			btOk.Text = Resources.ButtonOK;
			return;
		}
		WinCommons._OpenProcessing();
		try
		{
			UsersImpl usersImpl = await ExtenseionMethods.GetUsersByCode(txtName.Text);
			ClassMD5 classMD = new ClassMD5();
			if (GlobalData.LoginSap)
			{
				usersImpl.PASSWORD = Function.ToString(txtPassNew.Text);
				usersImpl.IsChangePassword = true;
			}
			else
			{
				usersImpl.PASSWORD = classMD.GetMd5Hash(txtPassNew.Text);
			}
			DataTable tableStructure = WinFuntions.GetTableStructure("OPRM");
			for (int i = 0; i < GlobalData.AllModule.Rows.Count; i++)
			{
				string value = Function.ToString(GlobalData.AllModule.Rows[i]["Code"]);
				if (Function.ToString(GlobalData.AllModule.Rows[i]["MenuType"]) != "N")
				{
					DataRow dataRow = tableStructure.NewRow();
					dataRow["authoriza"] = "N";
					dataRow["menucode"] = value;
					dataRow["USER_CODE"] = usersImpl.USER_CODE;
					tableStructure.Rows.Add(dataRow);
				}
			}
			tableStructure.AcceptChanges();
			usersImpl.DTAuth = tableStructure;
			string text = await _httpClient.User.UpdateUsersAcount(usersImpl);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				GlobalData.OldPassword = EncodePassword.EncryptString(txtPassNew.Text, RSA.PasswordPhrase);
				btOk.Text = Resources.ButtonOK;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, Resources.Msg_SaveError + ": " + text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private bool CheckNewPass()
	{
		if (txtPassNew.Text == string.Empty)
		{
			txtRepass.ErrorIconAlignment = ErrorIconAlignment.BottomRight;
			txtRepass.ToolTipIconType = ToolTipIconType.Error;
			txtRepass.ErrorText = "New Password cannot be empty!";
			txtRepass.Focus();
		}
		if (txtPassNew.Text != txtRepass.Text)
		{
			txtRepass.ErrorIconAlignment = ErrorIconAlignment.BottomRight;
			txtRepass.ToolTipIconType = ToolTipIconType.Error;
			txtRepass.ErrorText = "Two passwords are the same";
			txtRepass.Focus();
			return false;
		}
		if (!CallFromUser && EncodePassword.EncryptString(txtPassOld.Text, RSA.PasswordPhrase) != GlobalData.OldPassword)
		{
			WinCommons.Error(Enums.MessageCode.PasswordIncorrect, "");
			return false;
		}
		return true;
	}

	private async void CheckPassOld()
	{
		_ = 1;
		try
		{
			if (GlobalData.LoginSap)
			{
				if (EncodePassword.EncryptString(txtPassOld.Text, RSA.PasswordPhrase) != GlobalData.OldPassword)
				{
					txtPassOld.ErrorIconAlignment = ErrorIconAlignment.BottomRight;
					txtPassOld.ToolTipIconType = ToolTipIconType.Error;
					txtPassOld.ErrorText = "Password enter failed";
					txtPassOld.Focus();
				}
				else
				{
					txtPassOld.ErrorIconAlignment = ErrorIconAlignment.BottomRight;
					txtPassOld.ToolTipIconType = ToolTipIconType.None;
				}
				return;
			}
			ClassMD5 classMD = new ClassMD5();
			string userCode = txtName.Text;
			string md5Hash = classMD.GetMd5Hash(txtPassOld.Text.ToString().Trim());
			WinCommons._OpenProcessing();
			using HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Add("SessionId", GlobalData.SessionId);
			using HttpResponseMessage response = await client.GetAsync(Function.GetApiUri(Configuration.ApiServer, "Users", "CheckLogIn", new
			{
				userCode = userCode,
				passWord = md5Hash
			}));
			if (response.IsSuccessStatusCode)
			{
				if (!JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync()))
				{
					txtPassOld.ErrorIconAlignment = ErrorIconAlignment.BottomRight;
					txtPassOld.ToolTipIconType = ToolTipIconType.Error;
					txtPassOld.ErrorText = "Password enter failed";
					txtPassOld.Focus();
					return;
				}
				txtPassOld.ErrorIconAlignment = ErrorIconAlignment.BottomRight;
				txtPassOld.ToolTipIconType = ToolTipIconType.None;
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtPassOld_Leave(object sender, EventArgs e)
	{
		CheckPassOld();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_ChangePassword));
		this.lb_Username = new Apzon.Controls.Controls.LabelLine();
		this.lb_OldPassword = new Apzon.Controls.Controls.LabelLine();
		this.lb_NewPassword = new Apzon.Controls.Controls.LabelLine();
		this.lb_RePassword = new Apzon.Controls.Controls.LabelLine();
		this.txtName = new Apzon.Controls.Editor.TextEditEx();
		this.txtPassOld = new Apzon.Controls.Editor.TextEditEx();
		this.txtPassNew = new Apzon.Controls.Editor.TextEditEx();
		this.txtRepass = new Apzon.Controls.Editor.TextEditEx();
		this.btOk = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.textEditEx1 = new Apzon.Controls.Editor.TextEditEx();
		this.label4 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.label1 = new System.Windows.Forms.Label();
		((System.ComponentModel.ISupportInitialize)this.txtName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPassOld.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPassNew.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtRepass.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEditEx1.Properties).BeginInit();
		base.SuspendLayout();
		this.lb_Username.FieldName = null;
		this.lb_Username.HideWhenEmpty = false;
		this.lb_Username.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Username.Location = new System.Drawing.Point(15, 130);
		this.lb_Username.Name = "lb_Username";
		this.lb_Username.ShowUnderLine = true;
		this.lb_Username.Size = new System.Drawing.Size(107, 20);
		this.lb_Username.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Username.TabIndex = 0;
		this.lb_Username.Utext = "User Name";
		this.lb_Username.Visible = false;
		this.lb_OldPassword.FieldName = null;
		this.lb_OldPassword.HideWhenEmpty = false;
		this.lb_OldPassword.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_OldPassword.Location = new System.Drawing.Point(15, 156);
		this.lb_OldPassword.Name = "lb_OldPassword";
		this.lb_OldPassword.ShowUnderLine = true;
		this.lb_OldPassword.Size = new System.Drawing.Size(107, 20);
		this.lb_OldPassword.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_OldPassword.TabIndex = 0;
		this.lb_OldPassword.Utext = "Old Password";
		this.lb_NewPassword.FieldName = null;
		this.lb_NewPassword.HideWhenEmpty = false;
		this.lb_NewPassword.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_NewPassword.Location = new System.Drawing.Point(15, 182);
		this.lb_NewPassword.Name = "lb_NewPassword";
		this.lb_NewPassword.ShowUnderLine = true;
		this.lb_NewPassword.Size = new System.Drawing.Size(107, 20);
		this.lb_NewPassword.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_NewPassword.TabIndex = 0;
		this.lb_NewPassword.Utext = "New Password";
		this.lb_RePassword.FieldName = null;
		this.lb_RePassword.HideWhenEmpty = false;
		this.lb_RePassword.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_RePassword.Location = new System.Drawing.Point(15, 208);
		this.lb_RePassword.Name = "lb_RePassword";
		this.lb_RePassword.ShowUnderLine = true;
		this.lb_RePassword.Size = new System.Drawing.Size(107, 20);
		this.lb_RePassword.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_RePassword.TabIndex = 0;
		this.lb_RePassword.Utext = "Confirm";
		this.txtName.FieldName = null;
		this.txtName.IsSearchControl = false;
		this.txtName.Location = new System.Drawing.Point(118, 127);
		this.txtName.Name = "txtName";
		this.txtName.Size = new System.Drawing.Size(206, 20);
		this.txtName.TabIndex = 1;
		this.txtName.Visible = false;
		this.txtPassOld.FieldName = null;
		this.txtPassOld.IsSearchControl = false;
		this.txtPassOld.Location = new System.Drawing.Point(118, 153);
		this.txtPassOld.Name = "txtPassOld";
		this.txtPassOld.Properties.PasswordChar = '*';
		this.txtPassOld.Size = new System.Drawing.Size(206, 20);
		this.txtPassOld.TabIndex = 2;
		this.txtPassOld.Leave += new System.EventHandler(this.txtPassOld_Leave);
		this.txtPassNew.FieldName = null;
		this.txtPassNew.IsSearchControl = false;
		this.txtPassNew.Location = new System.Drawing.Point(118, 179);
		this.txtPassNew.Name = "txtPassNew";
		this.txtPassNew.Properties.PasswordChar = '*';
		this.txtPassNew.Size = new System.Drawing.Size(206, 20);
		this.txtPassNew.TabIndex = 3;
		this.txtRepass.FieldName = null;
		this.txtRepass.IsSearchControl = false;
		this.txtRepass.Location = new System.Drawing.Point(118, 205);
		this.txtRepass.Name = "txtRepass";
		this.txtRepass.Properties.PasswordChar = '*';
		this.txtRepass.Size = new System.Drawing.Size(206, 20);
		this.txtRepass.TabIndex = 4;
		this.btOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOk.FieldName = null;
		this.btOk.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOk.ImageOptions.Image");
		this.btOk.IsSearchControl = false;
		this.btOk.Location = new System.Drawing.Point(117, 236);
		this.btOk.Name = "btOk";
		this.btOk.Size = new System.Drawing.Size(75, 20);
		this.btOk.TabIndex = 8;
		this.btOk.Text = "OK";
		this.btOk.Click += new System.EventHandler(this.btOk_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(195, 236);
		this.btCancel.Name = "btCancel";
		this.btCancel.Size = new System.Drawing.Size(75, 20);
		this.btCancel.TabIndex = 9;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.textEditEx1.EditValue = "abcd";
		this.textEditEx1.Enabled = false;
		this.textEditEx1.FieldName = null;
		this.textEditEx1.IsSearchControl = false;
		this.textEditEx1.Location = new System.Drawing.Point(118, 101);
		this.textEditEx1.Name = "textEditEx1";
		this.textEditEx1.Size = new System.Drawing.Size(100, 20);
		this.textEditEx1.TabIndex = 14;
		this.label4.AutoSize = true;
		this.label4.Location = new System.Drawing.Point(12, 105);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(100, 13);
		this.label4.TabIndex = 13;
		this.label4.Text = "Password Example:";
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(14, 43);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(176, 13);
		this.label3.TabIndex = 12;
		this.label3.Text = "Minimum of 4 Characters in Length.";
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(12, 26);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(142, 13);
		this.label2.TabIndex = 11;
		this.label2.Text = "The password must contain:";
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(12, 9);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(370, 13);
		this.label1.TabIndex = 10;
		this.label1.Text = "Choose a new password that complies with your company's password policy";
		base.AcceptButton = this.btOk;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btCancel;
		base.ClientSize = new System.Drawing.Size(394, 268);
		base.Controls.Add(this.textEditEx1);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btOk);
		base.Controls.Add(this.txtRepass);
		base.Controls.Add(this.txtPassNew);
		base.Controls.Add(this.txtPassOld);
		base.Controls.Add(this.txtName);
		base.Controls.Add(this.lb_RePassword);
		base.Controls.Add(this.lb_NewPassword);
		base.Controls.Add(this.lb_OldPassword);
		base.Controls.Add(this.lb_Username);
		base.FirstItemId = "";
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.FormId = "oChangePass";
		base.IconOptions.ShowIcon = false;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frm_ChangePassword";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Change Password";
		base.Load += new System.EventHandler(this.frm_ChangePassword_Load);
		((System.ComponentModel.ISupportInitialize)this.txtName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPassOld.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPassNew.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtRepass.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEditEx1.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
