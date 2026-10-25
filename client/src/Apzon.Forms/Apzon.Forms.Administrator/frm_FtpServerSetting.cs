using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Newtonsoft.Json;

namespace Apzon.Forms.Administrator;

public class frm_FtpServerSetting : XtraFormEx
{
	private IContainer components;

	private PanelControl panelControl2;

	private TextEditEx txtFptServer;

	private SimpleButtonEx btnFtpCancel;

	private LabelLine labelLine5;

	private SimpleButtonEx btnFtpCheckConnection;

	private LabelLine labelLine6;

	private SimpleButtonEx btnFtpRefresh;

	private LabelLine lb_UserName;

	private SimpleButtonEx btnFtpUpdate;

	private LabelLine lb_Password;

	private TextEditEx txtFtpPassword;

	private TextEditEx txtFtpFolder;

	private TextEditEx txtFtpUsername;

	private RadioGroupEx rdo_Method;

	public frm_FtpServerSetting()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_FtpServerSetting"))
		{
			InitializeComponent();
			SettingLanguage();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_FtpServerSetting" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btnFtpCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void SettingLanguage()
	{
		btnFtpUpdate.Text = Resources.ButtonUpdate;
		btnFtpRefresh.Text = Resources.ButtonRefresh;
		btnFtpCheckConnection.Text = Resources.ButtonCheckConnect;
		btnFtpCancel.Text = Resources.ButtonCancel;
		lb_UserName.Utext = Resources.lb_UserName;
		lb_Password.Utext = Resources.lb_Password;
	}

	private void frm_FtpServerSetting_Load(object sender, EventArgs e)
	{
		GetFtpConfig();
	}

	private void btnFtpCheckConnection_Click(object sender, EventArgs e)
	{
		if (ValidateFtpForm())
		{
			CheckFtpConnection();
		}
	}

	private void btnFtpUpdate_Click(object sender, EventArgs e)
	{
		if (ValidateFtpForm())
		{
			UpdateFtpConfig();
		}
	}

	private void btnFtpCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void GetFtpConfig()
	{
		_ = 1;
		try
		{
			using HttpClient client = new HttpClient();
			using HttpResponseMessage response = await client.GetAsync(Function.GetApiUri(Configuration.ApiServer, "ConfigConnect", "GetFtpConfig"));
			if (response.IsSuccessStatusCode)
			{
				Function.ConfigFTPServer configFTPServer = JsonConvert.DeserializeObject<Function.ConfigFTPServer>(await response.Content.ReadAsStringAsync());
				txtFptServer.Text = (string.IsNullOrEmpty(configFTPServer.Address) ? "ftp://" : configFTPServer.Address);
				txtFtpFolder.Text = (string.IsNullOrEmpty(configFTPServer.Folder) ? "" : configFTPServer.Folder);
				txtFtpUsername.Text = (string.IsNullOrEmpty(configFTPServer.User) ? "" : configFTPServer.User);
				txtFtpPassword.Text = (string.IsNullOrEmpty(configFTPServer.PassWord) ? "" : configFTPServer.PassWord);
				rdo_Method.EditValue = (string.IsNullOrEmpty(configFTPServer.Method) ? "ftp" : configFTPServer.Method);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void CheckFtpConnection()
	{
		try
		{
			WinCommons._OpenProcessing();
			_ = string.Empty;
			Function.ConfigFTPServer configFTPServer = new Function.ConfigFTPServer();
			configFTPServer.Address = txtFptServer.Text.Trim();
			configFTPServer.Folder = txtFtpFolder.Text.Trim();
			configFTPServer.User = txtFtpUsername.Text.Trim();
			configFTPServer.PassWord = txtFtpPassword.Text.Trim();
			configFTPServer.Method = rdo_Method.EditValue.ToString();
			if (configFTPServer.Method.Equals("api"))
			{
				string[] array = Configuration.ApiServer.Split(new string[1] { "//" }, StringSplitOptions.None);
				if (array.Length <= 1)
				{
					WinCommons.Warning(Enums.MessageCode.CannotCheck4UpdateClient, "");
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ApiServer error " + array);
					return;
				}
				array = array[1].Split(':');
				string text = string.Format("\\\\{0}\\{1}\\Update\\NewVersion.xml", array[0], "APZON-IRS Client");
				if (ReadXml(text, array[0], configFTPServer.User, configFTPServer.PassWord) == null)
				{
					WinCommons.Error(Enums.MessageCode.CouldNotReadFileToUpdate, "");
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Could not read file " + text);
					XtraMessageBox.Show(Resources.Msg_ConnectFaile, Resources.Msg, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
				else
				{
					XtraMessageBox.Show(Resources.Msg_ConnectSuccess, Resources.Msg, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				}
				return;
			}
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "ConfigConnect");
			string text2 = await helper.PostParamsAsJsonAsync("CheckFtpConnection", configFTPServer);
			if (string.IsNullOrEmpty(text2))
			{
				XtraMessageBox.Show(Resources.Msg_ConnectSuccess, Resources.Msg, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
			else
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text2);
				XtraMessageBox.Show(Resources.Msg_ConnectFaile, Resources.Msg, MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void UpdateFtpConfig()
	{
		try
		{
			WinCommons._OpenProcessing();
			Function.ConfigFTPServer configFTPServer = new Function.ConfigFTPServer();
			configFTPServer.Address = txtFptServer.Text.Trim();
			configFTPServer.Folder = txtFtpFolder.Text.Trim();
			configFTPServer.User = txtFtpUsername.Text.Trim();
			configFTPServer.PassWord = txtFtpPassword.Text.Trim();
			configFTPServer.Method = rdo_Method.EditValue.ToString();
			HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
			httpRequestMessage.Content = new ObjectContent<Function.ConfigFTPServer>(configFTPServer, new JsonMediaTypeFormatter());
			httpRequestMessage.RequestUri = new Uri(Function.GetApiUri(Configuration.ApiServer, "ConfigConnect", "SetFtpConfig"));
			httpRequestMessage.Method = HttpMethod.Post;
			new HttpClient().SendAsync(httpRequestMessage).ContinueWith((Task<HttpResponseMessage> task) =>
			{
				if (task.Result.IsSuccessStatusCode)
				{
					task.Result.Content.ReadAsStringAsync();
					WinCommons.writeStatus(Enums.MessageCode.UpdateSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
				}
				else
				{
					WinCommons.writeStatus(Enums.MessageCode.UpdateFailed, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
				}
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private bool ValidateFtpForm()
	{
		if (!Check._text(txtFptServer))
		{
			return false;
		}
		return true;
	}

	private XDocument ReadXml(string pathFile, string computerName, string userName, string password)
	{
		try
		{
			using (WinFuntions.NetworkShareAccesser.Access(computerName, userName, password))
			{
				if (FileIO.FileExist(pathFile))
				{
					return XDocument.Load(pathFile);
				}
			}
		}
		catch (Exception ex)
		{
			if (ex.Message.StartsWith("Multiple connections") && FileIO.FileExist(pathFile))
			{
				return XDocument.Load(pathFile);
			}
		}
		return new XDocument();
	}

	private void btnFtpRefresh_Click(object sender, EventArgs e)
	{
		GetFtpConfig();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_FtpServerSetting));
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.rdo_Method = new Apzon.Controls.Editor.RadioGroupEx();
		this.txtFptServer = new Apzon.Controls.Editor.TextEditEx();
		this.btnFtpCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.btnFtpCheckConnection = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.btnFtpRefresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lb_UserName = new Apzon.Controls.Controls.LabelLine();
		this.btnFtpUpdate = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lb_Password = new Apzon.Controls.Controls.LabelLine();
		this.txtFtpPassword = new Apzon.Controls.Editor.TextEditEx();
		this.txtFtpFolder = new Apzon.Controls.Editor.TextEditEx();
		this.txtFtpUsername = new Apzon.Controls.Editor.TextEditEx();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.rdo_Method.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFptServer.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFtpPassword.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFtpFolder.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFtpUsername.Properties).BeginInit();
		base.SuspendLayout();
		this.panelControl2.Controls.Add(this.rdo_Method);
		this.panelControl2.Controls.Add(this.txtFptServer);
		this.panelControl2.Controls.Add(this.btnFtpCancel);
		this.panelControl2.Controls.Add(this.labelLine5);
		this.panelControl2.Controls.Add(this.btnFtpCheckConnection);
		this.panelControl2.Controls.Add(this.labelLine6);
		this.panelControl2.Controls.Add(this.btnFtpRefresh);
		this.panelControl2.Controls.Add(this.lb_UserName);
		this.panelControl2.Controls.Add(this.btnFtpUpdate);
		this.panelControl2.Controls.Add(this.lb_Password);
		this.panelControl2.Controls.Add(this.txtFtpPassword);
		this.panelControl2.Controls.Add(this.txtFtpFolder);
		this.panelControl2.Controls.Add(this.txtFtpUsername);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl2.Location = new System.Drawing.Point(0, 0);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(339, 250);
		this.panelControl2.TabIndex = 0;
		this.rdo_Method.EditValue = "ftp";
		this.rdo_Method.FieldName = null;
		this.rdo_Method.IsSearchControl = false;
		this.rdo_Method.Location = new System.Drawing.Point(21, 124);
		this.rdo_Method.Name = "rdo_Method";
		this.rdo_Method.Properties.Columns = 3;
		this.rdo_Method.Properties.GlyphAlignment = DevExpress.Utils.HorzAlignment.Default;
		this.rdo_Method.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[3]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("ftp", "Ftp Server"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("net", "Network Server"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("api", "API Server")
		});
		this.rdo_Method.Size = new System.Drawing.Size(293, 27);
		this.rdo_Method.TabIndex = 12;
		this.txtFptServer.FieldName = null;
		this.txtFptServer.IsSearchControl = false;
		this.txtFptServer.Location = new System.Drawing.Point(101, 22);
		this.txtFptServer.Name = "txtFptServer";
		this.txtFptServer.Size = new System.Drawing.Size(213, 20);
		this.txtFptServer.TabIndex = 0;
		this.btnFtpCancel.FieldName = null;
		this.btnFtpCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnFtpCancel.ImageOptions.Image");
		this.btnFtpCancel.IsSearchControl = false;
		this.btnFtpCancel.Location = new System.Drawing.Point(239, 215);
		this.btnFtpCancel.Name = "btnFtpCancel";
		this.btnFtpCancel.Size = new System.Drawing.Size(75, 23);
		this.btnFtpCancel.TabIndex = 7;
		this.btnFtpCancel.Text = "Exit";
		this.btnFtpCancel.Click += new System.EventHandler(this.btnFtpCancel_Click);
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(21, 24);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(80, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 8;
		this.labelLine5.Utext = "Ftp Server";
		this.btnFtpCheckConnection.FieldName = null;
		this.btnFtpCheckConnection.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnFtpCheckConnection.ImageOptions.Image");
		this.btnFtpCheckConnection.IsSearchControl = false;
		this.btnFtpCheckConnection.Location = new System.Drawing.Point(26, 215);
		this.btnFtpCheckConnection.Name = "btnFtpCheckConnection";
		this.btnFtpCheckConnection.Size = new System.Drawing.Size(115, 23);
		this.btnFtpCheckConnection.TabIndex = 4;
		this.btnFtpCheckConnection.Text = "Check Connection";
		this.btnFtpCheckConnection.Click += new System.EventHandler(this.btnFtpCheckConnection_Click);
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(21, 48);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(80, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 9;
		this.labelLine6.Utext = "Folder";
		this.btnFtpRefresh.FieldName = null;
		this.btnFtpRefresh.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnFtpRefresh.ImageOptions.Image");
		this.btnFtpRefresh.IsSearchControl = false;
		this.btnFtpRefresh.Location = new System.Drawing.Point(181, 171);
		this.btnFtpRefresh.Name = "btnFtpRefresh";
		this.btnFtpRefresh.Size = new System.Drawing.Size(75, 23);
		this.btnFtpRefresh.TabIndex = 6;
		this.btnFtpRefresh.Text = "Refresh";
		this.btnFtpRefresh.Click += new System.EventHandler(this.btnFtpRefresh_Click);
		this.lb_UserName.FieldName = null;
		this.lb_UserName.HideWhenEmpty = false;
		this.lb_UserName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_UserName.Location = new System.Drawing.Point(21, 75);
		this.lb_UserName.Name = "lb_UserName";
		this.lb_UserName.ShowUnderLine = true;
		this.lb_UserName.Size = new System.Drawing.Size(80, 20);
		this.lb_UserName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_UserName.TabIndex = 10;
		this.lb_UserName.Utext = "User Name";
		this.btnFtpUpdate.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Update;
		this.btnFtpUpdate.FieldName = null;
		this.btnFtpUpdate.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnFtpUpdate.ImageOptions.Image");
		this.btnFtpUpdate.IsSearchControl = false;
		this.btnFtpUpdate.Location = new System.Drawing.Point(100, 171);
		this.btnFtpUpdate.Name = "btnFtpUpdate";
		this.btnFtpUpdate.Size = new System.Drawing.Size(75, 23);
		this.btnFtpUpdate.TabIndex = 5;
		this.btnFtpUpdate.Text = "Update";
		this.btnFtpUpdate.Click += new System.EventHandler(this.btnFtpUpdate_Click);
		this.lb_Password.FieldName = null;
		this.lb_Password.HideWhenEmpty = false;
		this.lb_Password.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Password.Location = new System.Drawing.Point(21, 101);
		this.lb_Password.Name = "lb_Password";
		this.lb_Password.ShowUnderLine = true;
		this.lb_Password.Size = new System.Drawing.Size(80, 20);
		this.lb_Password.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Password.TabIndex = 11;
		this.lb_Password.Utext = "Password";
		this.txtFtpPassword.FieldName = null;
		this.txtFtpPassword.IsSearchControl = false;
		this.txtFtpPassword.Location = new System.Drawing.Point(101, 98);
		this.txtFtpPassword.Name = "txtFtpPassword";
		this.txtFtpPassword.Properties.PasswordChar = '*';
		this.txtFtpPassword.Size = new System.Drawing.Size(213, 20);
		this.txtFtpPassword.TabIndex = 3;
		this.txtFtpFolder.FieldName = null;
		this.txtFtpFolder.IsSearchControl = false;
		this.txtFtpFolder.Location = new System.Drawing.Point(101, 46);
		this.txtFtpFolder.Name = "txtFtpFolder";
		this.txtFtpFolder.Size = new System.Drawing.Size(213, 20);
		this.txtFtpFolder.TabIndex = 1;
		this.txtFtpUsername.FieldName = null;
		this.txtFtpUsername.IsSearchControl = false;
		this.txtFtpUsername.Location = new System.Drawing.Point(101, 72);
		this.txtFtpUsername.Name = "txtFtpUsername";
		this.txtFtpUsername.Size = new System.Drawing.Size(213, 20);
		this.txtFtpUsername.TabIndex = 2;
		base.AcceptButton = this.btnFtpUpdate;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(339, 250);
		base.Controls.Add(this.panelControl2);
		base.FormId = "oFtpSetting";
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frm_FtpServerSetting";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Ftp Server Setting";
		base.Load += new System.EventHandler(this.frm_FtpServerSetting_Load);
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.rdo_Method.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFptServer.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFtpPassword.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFtpFolder.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFtpUsername.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
