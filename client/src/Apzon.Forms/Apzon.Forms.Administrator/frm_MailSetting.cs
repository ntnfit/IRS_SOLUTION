using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace Apzon.Forms.Administrator;

public class frm_MailSetting : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public int EmpId;

	private bool _isChanging;

	private bool _isCancel;

	private bool _forceClose;

	private bool _isLoading;

	private bool _connectOk;

	private DataTable _dtGeneSettings;

	private DataTable _dtDefault;

	public Func<Task> RefreshCal;

	public Func<Task> RefreshSettings;

	private IContainer components;

	public SimpleButtonEx btnCancel;

	public SimpleButtonEx btn_Save;

	private RepositoryItemLookUpEdit rep_Deparment;

	private LabelLine labelLine1;

	private CheckEditEx chk_EnableSSL;

	private TextEditEx txt_ServerAddress;

	private TextEditEx txt_Port;

	private LabelLine labelLine2;

	private TextEditEx txt_FName;

	private LabelLine labelLine3;

	private TextEditEx txt_MailAddress;

	private LabelLine lbl_MailAddress;

	private TextEditEx txt_Password;

	private LabelLine lbl_Pass;

	public SimpleButtonEx btn_Restore;

	private TextEditEx txt_Username;

	private LabelLine lbl_UserName;

	private LookUpEditCombo luk_Method;

	private TextEditEx txt_ClientId;

	private TextEditEx txt_ClientSecret;

	private CalcEditEx cal_Timeout;

	private LabelLine labelControl1;

	private LabelControlEx labelControlEx1;

	public SimpleButtonEx btn_TestConnect;

	private CheckEditEx chk_Authen;

	private LabelControlEx lblAuthen;

	private LabelLine labelLine4;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btnCancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_MailSetting()
	{
		InitializeComponent();
		IsScrollable = true;
		IsSetSuperTip = true;
		FormId = "oMailSetting";
		OnLoadAsync += frm_MailSetting_Load;
	}

	private async Task frm_MailSetting_Load(object sender, EventArgs e)
	{
		try
		{
			_isLoading = true;
			LoadInitForm();
			await LoadData();
			SetEventControl(this);
			SetMaxLengthForControls();
			AcceptButton = btn_Save;
			CancelButton = btnCancel;
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_CalendarSetting_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (_isCancel)
			{
				return;
			}
			if (!_forceClose)
			{
				e.Cancel = true;
			}
			if (_forceClose)
			{
				return;
			}
			if (FormMode == Enums.FormViewModeType.Update)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
				{
				case DialogResult.No:
					_forceClose = true;
					Close();
					break;
				case DialogResult.Yes:
					if (string.IsNullOrEmpty(await SaveData()))
					{
						_forceClose = true;
						Close();
					}
					break;
				}
			}
			else
			{
				_forceClose = true;
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				btn_Save.Enabled = false;
				btnCancel.Enabled = false;
				await SaveData();
				btn_Save.Enabled = true;
				btnCancel.Enabled = true;
			}
			else
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		try
		{
			_isCancel = true;
			Close();
			_isCancel = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Restore_Click(object sender, EventArgs e)
	{
		try
		{
			if (_dtDefault.IsNotNull())
			{
				_dtGeneSettings.Rows.Clear();
				_dtGeneSettings.ImportRow(_dtDefault.Copy().Rows[0]);
				_dtGeneSettings.AcceptChanges();
			}
			if (_dtGeneSettings.IsNotNull())
			{
				BindControls(_dtGeneSettings);
			}
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_MailAddress_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (!WinFuntions._isemail(txt_MailAddress))
			{
				txt_MailAddress.ErrorText = "Invalid Email Address!";
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_Method_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			_isChanging = true;
			switch (Function.ToString(luk_Method.EditValue))
			{
			case "N":
				txt_ClientId.Visible = false;
				txt_ClientSecret.Visible = false;
				txt_Password.Properties.UseSystemPasswordChar = true;
				lbl_Pass.Utext = "Password";
				lbl_UserName.Utext = "Username";
				if (Function.ToString(chk_Authen.EditValue) == "Y")
				{
					lbl_UserName.Visible = true;
					lbl_Pass.Visible = true;
					txt_Username.Visible = true;
					txt_Password.Visible = true;
				}
				else
				{
					lbl_UserName.Visible = false;
					lbl_Pass.Visible = false;
					txt_Username.Visible = false;
					txt_Password.Visible = false;
				}
				lbl_MailAddress.Visible = false;
				txt_MailAddress.Visible = false;
				chk_Authen.Visible = true;
				lblAuthen.Visible = false;
				if (_isLoading)
				{
					return;
				}
				txt_ServerAddress.EditValue = "smtp.gmail.com";
				txt_Username.EditValue = txt_MailAddress.EditValue;
				txt_Password.EditValue = "";
				break;
			case "T":
				txt_ClientId.Visible = true;
				txt_ClientSecret.Visible = true;
				txt_ClientId.Location = txt_Username.Location;
				txt_ClientSecret.Location = txt_Password.Location;
				lbl_Pass.Utext = "Client Secret";
				lbl_UserName.Utext = "Client Id";
				txt_Password.Properties.UseSystemPasswordChar = true;
				lbl_MailAddress.Visible = true;
				txt_MailAddress.Visible = true;
				chk_Authen.Visible = false;
				lblAuthen.Visible = true;
				lblAuthen.Location = chk_Authen.Location;
				lbl_UserName.Visible = true;
				lbl_Pass.Visible = true;
				if (_isLoading)
				{
					return;
				}
				txt_ServerAddress.EditValue = "smtp.gmail.com";
				break;
			case "S":
				txt_ClientId.Visible = false;
				txt_ClientSecret.Visible = false;
				txt_Password.Properties.UseSystemPasswordChar = false;
				lbl_Pass.Utext = "Api Key";
				lbl_UserName.Utext = "Username";
				lbl_MailAddress.Visible = true;
				txt_MailAddress.Visible = true;
				txt_Username.Visible = true;
				txt_Password.Visible = true;
				chk_Authen.Visible = false;
				lblAuthen.Visible = true;
				lblAuthen.Location = chk_Authen.Location;
				lbl_UserName.Visible = true;
				lbl_Pass.Visible = true;
				if (_isLoading)
				{
					return;
				}
				txt_ServerAddress.EditValue = "smtp.sendgrid.com";
				txt_Username.EditValue = "apikey";
				txt_Password.EditValue = "";
				break;
			}
			_isChanging = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			_isChanging = false;
		}
	}

	private async Task LoadData()
	{
		try
		{
			DataTable dataTable = await _httpClient.MailSettingClient.GetDoc();
			if (dataTable.IsNotNull())
			{
				_dtGeneSettings = dataTable;
				_dtGeneSettings.AcceptChanges();
			}
			else if (_dtDefault.IsNotNull())
			{
				_dtGeneSettings = WinFuntions.GetTableStructure("APZ_OEMC");
				_dtGeneSettings.ImportRow(_dtDefault.Copy().Rows[0]);
				_dtGeneSettings.AcceptChanges();
			}
			if (_dtGeneSettings.IsNotNull())
			{
				string text = Function.ToString(_dtGeneSettings.Rows[0]["Password"]);
				if (!string.IsNullOrEmpty(text))
				{
					string value = "";
					try
					{
						value = Function.Decrypt(RSA.PrivateKey, text);
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
						WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
					}
					_dtGeneSettings.Rows[0]["Password"] = value;
					_dtGeneSettings.AcceptChanges();
				}
				if (string.IsNullOrEmpty(Function.ToString(_dtGeneSettings.Rows[0]["SmtpMethod"])))
				{
					_dtGeneSettings.Rows[0]["SmtpMethod"] = "N";
				}
				BindControls(_dtGeneSettings);
				_connectOk = true;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private void BindControls(DataTable dt)
	{
		try
		{
			if (dt.IsNotNull())
			{
				txt_ServerAddress.DataBindings.Clear();
				txt_ServerAddress.DataBindings.Add("EditValue", dt, "ServerAddress", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
				txt_Port.DataBindings.Clear();
				txt_Port.DataBindings.Add("EditValue", dt, "SSLPort", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
				txt_FName.DataBindings.Clear();
				txt_FName.DataBindings.Add("EditValue", dt, "FullName", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
				txt_MailAddress.DataBindings.Clear();
				txt_MailAddress.DataBindings.Add("EditValue", dt, "MailAddress", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
				txt_Password.DataBindings.Clear();
				txt_Password.DataBindings.Add("EditValue", dt, "Password", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
				chk_EnableSSL.DataBindings.Clear();
				chk_EnableSSL.DataBindings.Add("EditValue", dt, "EnableSSL", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
				chk_EnableSSL.Refresh();
				chk_Authen.DataBindings.Clear();
				chk_Authen.DataBindings.Add("EditValue", dt, "UseAuthen", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
				chk_Authen.Refresh();
				txt_Username.DataBindings.Clear();
				txt_Username.DataBindings.Add("EditValue", dt, "Username", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
				txt_ClientId.DataBindings.Clear();
				txt_ClientId.DataBindings.Add("EditValue", dt, "ClientId", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
				txt_ClientSecret.DataBindings.Clear();
				txt_ClientSecret.DataBindings.Add("EditValue", dt, "ClientSecret", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
				luk_Method.DataBindings.Clear();
				luk_Method.DataBindings.Add("EditValue", dt, "SmtpMethod", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
				cal_Timeout.DataBindings.Clear();
				cal_Timeout.DataBindings.Add("EditValue", dt, "Timeout", formattingEnabled: false, DataSourceUpdateMode.OnPropertyChanged);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadInitForm()
	{
		try
		{
			_dtDefault = WinFuntions.GetTableStructure("APZ_OEMC");
			DataRow dataRow = _dtDefault.NewRow();
			dataRow["ServerAddress"] = "smtp.gmail.com";
			dataRow["SSLPort"] = 465;
			dataRow["FullName"] = "";
			dataRow["MailAddress"] = "";
			dataRow["Password"] = "";
			dataRow["EnableSSL"] = "Y";
			dataRow["SmtpMethod"] = "N";
			dataRow["Timeout"] = 30;
			dataRow["UseAuthen"] = "Y";
			_dtDefault.Rows.Add(dataRow);
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "N",
					Name = "Normal"
				},
				new Common.Table_Code
				{
					Code = "T",
					Name = "Google Token"
				},
				new Common.Table_Code
				{
					Code = "S",
					Name = "SendGrid Api"
				}
			};
			luk_Method.Properties.DataSource = dataSource;
			luk_Method.Properties.DisplayMember = "Name";
			luk_Method.Properties.ValueMember = "Code";
			luk_Method.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			luk_Method.Properties.ForceInitialize();
			luk_Method.Properties.PopulateColumns();
			luk_Method.Properties.Columns["Code"].Visible = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_TestConnect_Click(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(await TestConnect()))
			{
				_connectOk = true;
				XtraMessageBoxEx.Show(Enums.MessageCode.InfoConnectSmtpSuccess, Enums.MessageCode.SystemMessage);
			}
			else
			{
				_connectOk = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chk_Authen_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (Function.ToString(chk_Authen.EditValue) == "Y")
			{
				lbl_UserName.Visible = true;
				lbl_Pass.Visible = true;
				txt_Username.Visible = true;
				txt_Password.Visible = true;
			}
			else
			{
				lbl_UserName.Visible = false;
				lbl_Pass.Visible = false;
				txt_Username.Visible = false;
				txt_Password.Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> TestConnect()
	{
		_ = 5;
		try
		{
			if (_dtGeneSettings.IsNotNull())
			{
				DataRow dataRow = _dtGeneSettings.Rows[0];
				string userName = Function.ToString(dataRow["UserName"]);
				string password = Function.ToString(dataRow["Password"]);
				string clientId = Function.ToString(dataRow["ClientId"]);
				string clientSecret = Function.ToString(dataRow["ClientSecret"]);
				string method = Function.ToString(dataRow["SmtpMethod"]);
				int num = Function.ParseInt(dataRow["Timeout"]);
				string host = Function.ToString(dataRow["ServerAddress"]);
				int port = Function.ParseInt(dataRow["SSLPort"]);
				string mailAddress = Function.ToString(dataRow["MailAddress"]);
				bool useSsl = Function.ToString(dataRow["EnableSSL"]) == "Y";
				bool useAuthen = Function.ToString(dataRow["UseAuthen"]) == "Y";
				switch (method)
				{
				case "N":
					if (useAuthen && (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(userName)))
					{
						WinCommons.Error(Enums.MessageCode.YouMustSetupSmtpCredential, "");
						return "Error!";
					}
					break;
				case "T":
					if (string.IsNullOrEmpty(mailAddress))
					{
						WinCommons.Error(Enums.MessageCode.MailAddressIsMissing, "");
						return "Error!";
					}
					if (string.IsNullOrEmpty(clientId))
					{
						WinCommons.Error(Enums.MessageCode.ClientIdIsMissing, "");
						return "Error!";
					}
					if (string.IsNullOrEmpty(clientSecret))
					{
						WinCommons.Error(Enums.MessageCode.ClientScretIsMissing, "");
						return "Error!";
					}
					break;
				case "S":
					if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(userName))
					{
						WinCommons.Error(Enums.MessageCode.YouMustSetupSmtpCredential, "");
						return "Error!";
					}
					break;
				}
				WinCommons._OpenProcessing();
				_dtGeneSettings.AcceptChanges();
				SmtpClient oSmtp = new SmtpClient
				{
					Timeout = num * 1000,
					ServerCertificateValidationCallback = (object s, X509Certificate c, X509Chain h, SslPolicyErrors e) => true
				};
				await oSmtp.ConnectAsync(host, port, useSsl);
				switch (method)
				{
				case "N":
					if (useAuthen)
					{
						await oSmtp.AuthenticateAsync(userName, password);
					}
					break;
				case "S":
					await oSmtp.AuthenticateAsync(userName, password);
					break;
				case "T":
				{
					string path = Application.StartupPath + "\\Analytics.Auth.Store\\Google.Apis.Auth.OAuth2.Responses.TokenResponse-Test";
					if (File.Exists(path) && !_connectOk)
					{
						File.Delete(path);
					}
					UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
					{
						ClientId = clientId,
						ClientSecret = clientSecret
					}, new string[1] { "https://mail.google.com/" }, "Test", CancellationToken.None, new FileDataStore(Application.StartupPath + "\\Analytics.Auth.Store\\"));
					await credential.RefreshTokenAsync(CancellationToken.None);
					SaslMechanism mechanism = new SaslMechanismOAuth2(mailAddress, credential.Token.AccessToken);
					await oSmtp.AuthenticateAsync(mechanism);
					break;
				}
				}
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return ex.Message;
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
		return string.Empty;
	}

	private async Task<string> SaveData()
	{
		_ = 2;
		try
		{
			if (!_connectOk)
			{
				if (!string.IsNullOrEmpty(await TestConnect()))
				{
					return "Error!";
				}
				_connectOk = true;
			}
			if (_dtGeneSettings.IsNotNull())
			{
				DataRow dr = _dtGeneSettings.Rows[0];
				string password = Function.ToString(dr["Password"]);
				string text = Function.ToString(dr["SmtpMethod"]);
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveMailSetting, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return "Error!";
				}
				WinCommons._OpenProcessing();
				if (text == "T")
				{
					string sourceFileName = Application.StartupPath + "\\Analytics.Auth.Store\\Google.Apis.Auth.OAuth2.Responses.TokenResponse-Test";
					string text2 = Application.StartupPath + "\\Analytics.Auth.Store\\Google.Apis.Auth.OAuth2.Responses.TokenResponse-Apzon";
					if (_connectOk)
					{
						if (!Directory.Exists(Application.StartupPath + "\\Analytics.Auth.Store"))
						{
							Directory.CreateDirectory(Application.StartupPath + "\\Analytics.Auth.Store");
						}
						if (File.Exists(text2))
						{
							File.Delete(text2);
						}
						File.Copy(sourceFileName, text2);
					}
					string text3 = await _httpClient.FileNetworkManagerClient.UpLoadFile("\\Report\\", true, text2);
					if (!string.IsNullOrEmpty(text3))
					{
						WinCommons.Error(Enums.MessageCode.CannotUploadTheTokenFilePleaseContactAdministratorForMoreDetail, "");
						Logging.Write("ERROR", text3);
						_dtGeneSettings.RejectChanges();
						return "Error";
					}
				}
				dr["UserSign"] = GlobalData.UserID;
				dr["UpdateDate"] = DateTime.Today;
				if (!string.IsNullOrEmpty(password))
				{
					dr["Password"] = Function.Encrypt(RSA.PublicKey, password);
				}
				DataSet dataSet = new DataSet();
				dataSet.Tables.Add(_dtGeneSettings.Copy());
				if (dataSet.IsNotNull())
				{
					string text4 = await _httpClient.MailSettingClient.SaveDoc(dataSet);
					if (string.IsNullOrEmpty(text4))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						FormMode = Enums.FormViewModeType.View;
						REFRESH_DATA();
					}
					else
					{
						_dtGeneSettings.RejectChanges();
						WinCommons.Error(Enums.MessageCode.None, text4);
					}
					return text4;
				}
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return ex.Message;
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
		return "Error";
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				_isLoading = true;
				await LoadData();
				_isLoading = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
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
			if (!_isChanging && !_isLoading && FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Update;
				_connectOk = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void SetMaxLengthForControls()
	{
		try
		{
			Dictionary<string, Control> dicControls = new Dictionary<string, Control>
			{
				{ "APZ_OEMC.Details", txt_ServerAddress },
				{ "APZ_OEMC.SSLPort", txt_Port },
				{ "APZ_OEMC.FullName", txt_FName },
				{ "APZ_OEMC.MailAddress", txt_MailAddress },
				{ "APZ_OEMC.Password", txt_Password },
				{ "APZ_OEMC.Username", txt_Username },
				{ "APZ_OEMC.ClientId", txt_ClientId },
				{ "APZ_OEMC.ClientSecret", txt_ClientSecret }
			};
			await this.SetControlMaxLength(dicControls);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_MailSetting));
		this.rep_Deparment = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.chk_EnableSSL = new Apzon.Controls.Editor.CheckEditEx();
		this.txt_ServerAddress = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Port = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txt_FName = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_MailAddress = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_MailAddress = new Apzon.Controls.Controls.LabelLine();
		this.txt_Password = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_Pass = new Apzon.Controls.Controls.LabelLine();
		this.btn_Restore = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_Username = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_UserName = new Apzon.Controls.Controls.LabelLine();
		this.luk_Method = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.txt_ClientId = new Apzon.Controls.Editor.TextEditEx();
		this.txt_ClientSecret = new Apzon.Controls.Editor.TextEditEx();
		this.cal_Timeout = new Apzon.Controls.Editor.CalcEditEx();
		this.labelControl1 = new Apzon.Controls.Controls.LabelLine();
		this.labelControlEx1 = new Apzon.Controls.Controls.LabelControlEx();
		this.btn_TestConnect = new Apzon.Controls.Controls.SimpleButtonEx();
		this.chk_Authen = new Apzon.Controls.Editor.CheckEditEx();
		this.lblAuthen = new Apzon.Controls.Controls.LabelControlEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.rep_Deparment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_EnableSSL.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ServerAddress.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Port.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MailAddress.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Password.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Username.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Method.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ClientId.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ClientSecret.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cal_Timeout.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Authen.Properties).BeginInit();
		base.SuspendLayout();
		this.rep_Deparment.AutoHeight = false;
		this.rep_Deparment.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Deparment.Name = "rep_Deparment";
		this.rep_Deparment.NullText = "";
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(297, 231);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 13;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(201, 231);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.Size = new System.Drawing.Size(93, 20);
		this.btn_Save.TabIndex = 12;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.labelLine1.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine1.Appearance.Options.UseFont = true;
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(216, 7);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(89, 19);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 145;
		this.labelLine1.Utext = "Server Address";
		this.chk_EnableSSL.FieldName = "EnableSSL";
		this.chk_EnableSSL.IsSearchControl = false;
		this.chk_EnableSSL.Location = new System.Drawing.Point(387, 31);
		this.chk_EnableSSL.Name = "chk_EnableSSL";
		this.chk_EnableSSL.Properties.Caption = "Enable SSL";
		this.chk_EnableSSL.Properties.ValueChecked = "Y";
		this.chk_EnableSSL.Properties.ValueUnchecked = "N";
		this.chk_EnableSSL.Size = new System.Drawing.Size(114, 20);
		this.chk_EnableSSL.TabIndex = 4;
		this.txt_ServerAddress.FieldName = "ServerAddress";
		this.txt_ServerAddress.IsSearchControl = false;
		this.txt_ServerAddress.Location = new System.Drawing.Point(301, 4);
		this.txt_ServerAddress.Name = "txt_ServerAddress";
		this.txt_ServerAddress.Size = new System.Drawing.Size(200, 20);
		this.txt_ServerAddress.TabIndex = 2;
		this.txt_Port.FieldName = "SSLPort";
		this.txt_Port.IsSearchControl = false;
		this.txt_Port.Location = new System.Drawing.Point(301, 31);
		this.txt_Port.Name = "txt_Port";
		this.txt_Port.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_Port.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_Port.Size = new System.Drawing.Size(80, 20);
		this.txt_Port.TabIndex = 3;
		this.labelLine2.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine2.Appearance.Options.UseFont = true;
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(217, 34);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(89, 19);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 147;
		this.labelLine2.Utext = "Port";
		this.txt_FName.FieldName = "FullName";
		this.txt_FName.IsSearchControl = false;
		this.txt_FName.Location = new System.Drawing.Point(301, 85);
		this.txt_FName.Name = "txt_FName";
		this.txt_FName.Size = new System.Drawing.Size(200, 20);
		this.txt_FName.TabIndex = 6;
		this.labelLine3.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine3.Appearance.Options.UseFont = true;
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(216, 62);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(89, 19);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 149;
		this.labelLine3.Utext = "Timeout";
		this.txt_MailAddress.FieldName = "MailAddress";
		this.txt_MailAddress.IsSearchControl = false;
		this.txt_MailAddress.Location = new System.Drawing.Point(301, 112);
		this.txt_MailAddress.Name = "txt_MailAddress";
		this.txt_MailAddress.Size = new System.Drawing.Size(200, 20);
		this.txt_MailAddress.TabIndex = 7;
		this.txt_MailAddress.Visible = false;
		this.txt_MailAddress.Validating += new System.ComponentModel.CancelEventHandler(this.txt_MailAddress_Validating);
		this.lbl_MailAddress.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_MailAddress.Appearance.Options.UseFont = true;
		this.lbl_MailAddress.FieldName = null;
		this.lbl_MailAddress.HideWhenEmpty = false;
		this.lbl_MailAddress.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_MailAddress.Location = new System.Drawing.Point(217, 115);
		this.lbl_MailAddress.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lbl_MailAddress.Name = "lbl_MailAddress";
		this.lbl_MailAddress.ShowUnderLine = true;
		this.lbl_MailAddress.Size = new System.Drawing.Size(89, 19);
		this.lbl_MailAddress.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_MailAddress.TabIndex = 151;
		this.lbl_MailAddress.Utext = "Mail Address";
		this.lbl_MailAddress.Visible = false;
		this.txt_Password.FieldName = "Password";
		this.txt_Password.IsSearchControl = false;
		this.txt_Password.Location = new System.Drawing.Point(301, 192);
		this.txt_Password.Name = "txt_Password";
		this.txt_Password.Size = new System.Drawing.Size(200, 20);
		this.txt_Password.TabIndex = 10;
		this.lbl_Pass.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_Pass.Appearance.Options.UseFont = true;
		this.lbl_Pass.FieldName = null;
		this.lbl_Pass.HideWhenEmpty = false;
		this.lbl_Pass.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Pass.Location = new System.Drawing.Point(217, 195);
		this.lbl_Pass.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lbl_Pass.Name = "lbl_Pass";
		this.lbl_Pass.ShowUnderLine = true;
		this.lbl_Pass.Size = new System.Drawing.Size(89, 19);
		this.lbl_Pass.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Pass.TabIndex = 153;
		this.lbl_Pass.Utext = "Password";
		this.btn_Restore.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Restore.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.btn_Restore.Appearance.Options.UseBackColor = true;
		this.btn_Restore.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Restore.FieldName = null;
		this.btn_Restore.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Restore.ImageOptions.Image");
		this.btn_Restore.IsSearchControl = false;
		this.btn_Restore.Location = new System.Drawing.Point(393, 231);
		this.btn_Restore.Name = "btn_Restore";
		this.btn_Restore.Size = new System.Drawing.Size(108, 20);
		this.btn_Restore.TabIndex = 14;
		this.btn_Restore.Text = "Restore Default";
		this.btn_Restore.Click += new System.EventHandler(this.btn_Restore_Click);
		this.txt_Username.FieldName = "Username";
		this.txt_Username.IsSearchControl = false;
		this.txt_Username.Location = new System.Drawing.Point(301, 166);
		this.txt_Username.Name = "txt_Username";
		this.txt_Username.Size = new System.Drawing.Size(200, 20);
		this.txt_Username.TabIndex = 9;
		this.lbl_UserName.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_UserName.Appearance.Options.UseFont = true;
		this.lbl_UserName.FieldName = null;
		this.lbl_UserName.HideWhenEmpty = false;
		this.lbl_UserName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_UserName.Location = new System.Drawing.Point(217, 169);
		this.lbl_UserName.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lbl_UserName.Name = "lbl_UserName";
		this.lbl_UserName.ShowUnderLine = true;
		this.lbl_UserName.Size = new System.Drawing.Size(89, 19);
		this.lbl_UserName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_UserName.TabIndex = 155;
		this.lbl_UserName.Utext = "Username";
		this.luk_Method.FieldName = "SmtpMethod";
		this.luk_Method.IsSearchControl = false;
		this.luk_Method.Location = new System.Drawing.Point(12, 32);
		this.luk_Method.Name = "luk_Method";
		this.luk_Method.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Method.Properties.IsSearchControl = false;
		this.luk_Method.Properties.NullText = "";
		this.luk_Method.Size = new System.Drawing.Size(186, 20);
		this.luk_Method.TabIndex = 1;
		this.luk_Method.EditValueChanged += new System.EventHandler(this.luk_Method_EditValueChanged);
		this.txt_ClientId.FieldName = "ClientId";
		this.txt_ClientId.IsSearchControl = false;
		this.txt_ClientId.Location = new System.Drawing.Point(11, 166);
		this.txt_ClientId.Name = "txt_ClientId";
		this.txt_ClientId.Size = new System.Drawing.Size(200, 20);
		this.txt_ClientId.TabIndex = 7;
		this.txt_ClientId.Visible = false;
		this.txt_ClientSecret.FieldName = "ClientSecret";
		this.txt_ClientSecret.IsSearchControl = false;
		this.txt_ClientSecret.Location = new System.Drawing.Point(11, 192);
		this.txt_ClientSecret.Name = "txt_ClientSecret";
		this.txt_ClientSecret.Size = new System.Drawing.Size(200, 20);
		this.txt_ClientSecret.TabIndex = 8;
		this.txt_ClientSecret.Visible = false;
		this.cal_Timeout.FieldName = "SSLPort";
		this.cal_Timeout.IsSearchControl = false;
		this.cal_Timeout.Location = new System.Drawing.Point(301, 59);
		this.cal_Timeout.Name = "cal_Timeout";
		this.cal_Timeout.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cal_Timeout.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Integer;
		this.cal_Timeout.Properties.IsSearchControl = false;
		this.cal_Timeout.Size = new System.Drawing.Size(80, 20);
		this.cal_Timeout.TabIndex = 5;
		this.labelControl1.FieldName = null;
		this.labelControl1.HideWhenEmpty = false;
		this.labelControl1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelControl1.Location = new System.Drawing.Point(387, 62);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.ShowUnderLine = false;
		this.labelControl1.Size = new System.Drawing.Size(16, 13);
		this.labelControl1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControl1.TabIndex = 163;
		this.labelControl1.Utext = "(s)";
		this.labelControlEx1.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.labelControlEx1.Appearance.Options.UseFont = true;
		this.labelControlEx1.FieldName = null;
		this.labelControlEx1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelControlEx1.Location = new System.Drawing.Point(12, 7);
		this.labelControlEx1.Name = "labelControlEx1";
		this.labelControlEx1.Size = new System.Drawing.Size(81, 13);
		this.labelControlEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx1.TabIndex = 165;
		this.labelControlEx1.Text = "Select Method";
		this.btn_TestConnect.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.btn_TestConnect.Appearance.Options.UseBackColor = true;
		this.btn_TestConnect.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_TestConnect.FieldName = null;
		this.btn_TestConnect.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_TestConnect.ImageOptions.Image");
		this.btn_TestConnect.IsSearchControl = false;
		this.btn_TestConnect.Location = new System.Drawing.Point(377, 137);
		this.btn_TestConnect.Name = "btn_TestConnect";
		this.btn_TestConnect.Size = new System.Drawing.Size(124, 20);
		this.btn_TestConnect.TabIndex = 11;
		this.btn_TestConnect.Text = "Test Connection";
		this.btn_TestConnect.Click += new System.EventHandler(this.btn_TestConnect_Click);
		this.chk_Authen.FieldName = "UseAuthen";
		this.chk_Authen.IsSearchControl = false;
		this.chk_Authen.Location = new System.Drawing.Point(217, 137);
		this.chk_Authen.Name = "chk_Authen";
		this.chk_Authen.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.chk_Authen.Properties.Appearance.Options.UseFont = true;
		this.chk_Authen.Properties.Caption = "Authentication";
		this.chk_Authen.Properties.ValueChecked = "Y";
		this.chk_Authen.Properties.ValueUnchecked = "N";
		this.chk_Authen.Size = new System.Drawing.Size(114, 20);
		this.chk_Authen.TabIndex = 8;
		this.chk_Authen.EditValueChanged += new System.EventHandler(this.chk_Authen_EditValueChanged);
		this.lblAuthen.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.lblAuthen.Appearance.Options.UseFont = true;
		this.lblAuthen.FieldName = null;
		this.lblAuthen.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lblAuthen.Location = new System.Drawing.Point(11, 140);
		this.lblAuthen.Name = "lblAuthen";
		this.lblAuthen.Size = new System.Drawing.Size(84, 13);
		this.lblAuthen.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lblAuthen.TabIndex = 168;
		this.lblAuthen.Text = "Authentication";
		this.labelLine4.Appearance.Font = new System.Drawing.Font("Tahoma", 6.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelLine4.Appearance.Options.UseFont = true;
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(216, 87);
		this.labelLine4.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(88, 19);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 169;
		this.labelLine4.Utext = "Display Name";
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(513, 258);
		base.Controls.Add(this.txt_FName);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.lblAuthen);
		base.Controls.Add(this.chk_Authen);
		base.Controls.Add(this.btn_TestConnect);
		base.Controls.Add(this.labelControlEx1);
		base.Controls.Add(this.txt_MailAddress);
		base.Controls.Add(this.labelControl1);
		base.Controls.Add(this.cal_Timeout);
		base.Controls.Add(this.txt_ClientSecret);
		base.Controls.Add(this.txt_ClientId);
		base.Controls.Add(this.luk_Method);
		base.Controls.Add(this.txt_Username);
		base.Controls.Add(this.lbl_UserName);
		base.Controls.Add(this.btn_Restore);
		base.Controls.Add(this.txt_Password);
		base.Controls.Add(this.lbl_Pass);
		base.Controls.Add(this.lbl_MailAddress);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.txt_Port);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.txt_ServerAddress);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.chk_EnableSSL);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_MailSetting";
		base.ShowInTaskbar = false;
		this.Text = "E-Mail Setting";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_CalendarSetting_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.rep_Deparment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_EnableSSL.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ServerAddress.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Port.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MailAddress.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Password.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Username.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Method.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ClientId.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ClientSecret.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cal_Timeout.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Authen.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
