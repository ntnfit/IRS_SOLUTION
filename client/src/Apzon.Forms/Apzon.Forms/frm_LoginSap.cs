using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.FileDialog;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms;

public class frm_LoginSap : XtraForm
{
	public bool Login;

	public static bool Loaded;

	public bool HasActiveForm;

	public bool ChangeUser;

	private bool _isLoadingForm = true;

	private List<Common.Table_Code> _domainCert = new List<Common.Table_Code>();

	private string _oldDomain = "";

	public static List<Common.Table_ID> DbList = new List<Common.Table_ID>();

	private Function.ConfigFTPServer _ftp;

	private bool _changeUser;

	private bool _focus;

	private IContainer components;

	private ImageSlider imageSliderBackground;

	private Panel panel1;

	private LookUpEdit txt_Domain;

	private LabelLine lbDomain;

	private SimpleButtonEx btnSignUp;

	private LookUpEdit txtDatabase;

	private LabelControl lblVersion;

	private LabelLine lbDatabase;

	private SimpleButtonEx btnReset;

	private TextEditEx txtPassword;

	private LabelLine lbPass;

	private TextEditEx txtUserName;

	private LabelLine lbUser;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnLogin;

	public frm_LoginSap(bool changeUser = false)
	{
		InitializeComponent();
		ToolTipController toolTipController = new ToolTipController();
		toolTipController.BeforeShow += ToolTipController_BeforeShow;
		txtDatabase.ToolTipController = toolTipController;
		try
		{
			string path = Application.StartupPath + "\\BackgroundImage";
			if (Directory.Exists(path) && Directory.GetFiles(path).Length != 0)
			{
				imageSliderBackground.Visible = true;
				string[] files = Directory.GetFiles(path);
				for (int i = 0; i < files.Length; i++)
				{
					Image image = Image.FromFile(files[i]);
					imageSliderBackground.Images.Add(image);
				}
			}
			else
			{
				imageSliderBackground.Visible = false;
				if (new Random().Next(0, 2) == 1)
				{
					BackgroundImageStore = Apzon.Forms.Properties.Resources.Login1;
				}
				else
				{
					BackgroundImageStore = Apzon.Forms.Properties.Resources.Login0;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		_changeUser = changeUser;
	}

	private void ToolTipController_BeforeShow(object sender, ToolTipControllerShowEventArgs e)
	{
		if (!string.IsNullOrEmpty(txtDatabase.Text))
		{
			e.ToolTip = txtDatabase.Text;
		}
	}

	private void LoginSap_Load(object sender, EventArgs e)
	{
		txtUserName.Focus();
		try
		{
			if (ChangeUser)
			{
				btnCancel.Text = Apzon.Commons.Properties.Resources.ButtonCancel;
			}
			lblVersion.Text = $"Version: {WinFuntions.GetVersionString()}";
			_ftp = new Function.ConfigFTPServer();
			ConfigLogin login = WinFuntions.getConfigLogin();
			ReadFileCertificate();
			if (_domainCert.Exists((Common.Table_Code t) => t.Code.Equals(Function.ToString(login.Domain))))
			{
				txt_Domain.EditValue = Function.ToString(login.Domain);
				txtDatabase.EditValue = login.Database;
				txtUserName.Text = login.User;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
		Activate();
		Loaded = true;
		if (_changeUser || string.IsNullOrWhiteSpace(txtUserName.Text))
		{
			txtUserName.Select();
		}
		else
		{
			txtPassword.Select();
		}
		_focus = true;
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		DialogResult = DialogResult.Cancel;
		Close();
	}

	private async void btLogin_Click(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(txt_Domain.Text))
			{
				WinCommons.Error(Enums.MessageCode.PleaseChooseDomain, "");
				txt_Domain.ErrorIconAlignment = ErrorIconAlignment.BottomRight;
				txt_Domain.ToolTipIconType = ToolTipIconType.Error;
				txt_Domain.ErrorText = "Enter domain!";
				txt_Domain.Focus();
			}
			else if (string.IsNullOrEmpty(Function.ToString(txtDatabase.EditValue)))
			{
				WinCommons.Error(Enums.MessageCode.PleaseChooseDatabase, "");
				txtDatabase.ErrorIconAlignment = ErrorIconAlignment.BottomRight;
				txtDatabase.ToolTipIconType = ToolTipIconType.Error;
				txtDatabase.ErrorText = "Choose company!";
				txtDatabase.Focus();
			}
			else if (string.IsNullOrEmpty(txtUserName.Text))
			{
				WinCommons.Error(Enums.MessageCode.PleaseEnterUserName, "");
				txtUserName.ErrorIconAlignment = ErrorIconAlignment.BottomRight;
				txtUserName.ToolTipIconType = ToolTipIconType.Error;
				txtUserName.ErrorText = "Username cannot be empty!";
				txtUserName.Focus();
			}
			else if (string.IsNullOrEmpty(txtPassword.Text))
			{
				WinCommons.Error(Enums.MessageCode.PleaseEnterPassword, "");
				txtPassword.ErrorIconAlignment = ErrorIconAlignment.BottomRight;
				txtPassword.ToolTipIconType = ToolTipIconType.Error;
				txtPassword.ErrorText = "Password cannot be empty!";
				txtPassword.Focus();
			}
			else if (_domainCert.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.ClientDoesNotAuthenticated, "");
			}
			else if (!_domainCert.Exists((Common.Table_Code t) => t.Code.Equals(txt_Domain.Text)))
			{
				WinCommons.Error(Enums.MessageCode.ClientDoesNotAuthenticatedWithDomain, "");
			}
			else
			{
				await LoginClick();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Reset_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			EnableDisableControl(isEnable: false);
			using (HttpClientHelper<Common.Table_ID> commonClientHelper = new HttpClientHelper<Common.Table_ID>(Configuration.ApiServer, "Commons"))
			{
				DbList = await commonClientHelper.GetListAsync("GetNewDbList", new
				{
					domain = txt_Domain.Text
				});
			}
			txtDatabase.Properties.DataSource = DbList;
			txtDatabase.Properties.DisplayMember = "Name";
			txtDatabase.Properties.ValueMember = "ID";
			txtUserName.Text = "";
			txtPassword.Text = "";
			BeginInvoke((Action)(() =>
			{
				if (DbList.Any((Common.Table_ID x) => Function.ToString(x.ID) == Function.ToString(txtDatabase.EditValue)))
				{
					txtUserName.Focus();
				}
				else
				{
					txtDatabase.Focus();
				}
			}));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		EnableDisableControl(isEnable: true);
		WinCommons._CloseProcessing();
	}

	private async void look_Database_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			txtPassword.Text = "";
			if (!string.IsNullOrEmpty(Function.ToString(txtDatabase.EditValue)))
			{
				Enabled = false;
				await WinFuntions.CheckForUpdateClientHttp(Function.ToString(txtDatabase.EditValue), isOpenAplication: true);
			}
			if (_isLoadingForm)
			{
				_isLoadingForm = false;
			}
			else
			{
				txtUserName.BeginInvoke((Action)(() =>
				{
					txtUserName.Focus();
					_focus = true;
				}));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		Enabled = true;
	}

	private async void txt_Domain_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			string domain = Function.ToString(e.NewValue);
			if (domain.Equals("-12345678"))
			{
				ImportFileCertificate();
				e.Cancel = true;
				return;
			}
			if (string.IsNullOrEmpty(domain) || _domainCert.Count == 0 || !_domainCert.Exists((Common.Table_Code t) => t.Code.Equals(domain)))
			{
				txtDatabase.Properties.DataSource = null;
				return;
			}
			if (!domain.Equals(_oldDomain))
			{
				await LoadListDatabase(domain);
			}
			_oldDomain = domain;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtPassword_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyData == Keys.Tab)
			{
				txtUserName.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtPassword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		try
		{
			if (e.KeyData == Keys.Tab)
			{
				e.IsInputKey = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task CheckForUpdate()
	{
		try
		{
			WinCommons._OpenProcessing();
			using (HttpClientHelper<Function.ConfigFTPServer> helper = new HttpClientHelper<Function.ConfigFTPServer>(Configuration.ApiServer, "ConfigConnect"))
			{
				_ftp = await helper.GetAsync("GetFtpConfig");
				if (_ftp == null || string.IsNullOrEmpty(_ftp.Address))
				{
					WinCommons._CloseProcessing();
					WinCommons.Warning(Enums.MessageCode.CannotConnect2FtpServer, "");
					return;
				}
			}
			GlobalData.FtpConfig = _ftp;
			if (!string.IsNullOrEmpty(_ftp.Method) && "net".Equals(_ftp.Method))
			{
				UpdateNetworkClient(_ftp);
			}
			else if (!string.IsNullOrEmpty(_ftp.Method) && "ftp".Equals(_ftp.Method))
			{
				UpdateFtpClient(_ftp);
			}
			else
			{
				CheckUpdateFolderShareClient(_ftp);
			}
		}
		catch (WebException ex)
		{
			WinCommons._CloseProcessing();
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void UpdateFtpClient(Function.ConfigFTPServer ftp)
	{
		try
		{
			string address = ftp.Address;
			string folder = ftp.Folder;
			string user = ftp.User;
			string passWord = ftp.PassWord;
			string text = $"{address}/{folder}";
			string address2 = text + "/NewVersion.xml";
			using WebClient webClient = new WebClient();
			webClient.Credentials = new NetworkCredential(user, passWord);
			Stream stream = webClient.OpenRead(address2);
			XmlDocument xmlDocument = new XmlDocument();
			if (stream != null)
			{
				xmlDocument.Load(stream);
			}
			XElement xElement = GetXDocument(xmlDocument).Element("NewVersion");
			if (xElement != null)
			{
				int idMax = xElement.Elements("Versions").Max((XElement obj) => int.Parse(obj.Attribute("Id").Value));
				XElement xElement2 = (from c in xElement.Elements("Versions")
					where (int)c.Attribute("Id") == idMax
					select c).SingleOrDefault();
				if (xElement2 != null)
				{
					NumberFormatInfo provider = new NumberFormatInfo
					{
						NumberDecimalSeparator = ".",
						NumberGroupSeparator = ","
					};
					XElement xElement3 = xElement2.Element("VersionAp");
					XElement xElement4 = xElement2.Element("VersionBuild");
					if (xElement3 != null && xElement4 != null)
					{
						double num = Convert.ToDouble(xElement3.Value, provider);
						int num2 = Function.ParseInt(xElement4.Value);
						if (!(num > Versions.ApplicationVersion) && num2 <= Versions.BuildVersion)
						{
							return;
						}
						XElement xElement5 = xElement2.Element("FolderName");
						if (xElement5 != null)
						{
							string value = xElement5.Value;
							string folderName = $"{text}/{value}";
							WinCommons._CloseProcessing();
							if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdateNewVersion, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
							{
								WinCommons._OpenProcessing();
								SaveFolder(folderName, user, passWord);
							}
						}
					}
					else
					{
						WinCommons.Warning(Enums.MessageCode.VersionUpdateCannotBeNull, "");
					}
				}
				else
				{
					WinCommons.Warning(Enums.MessageCode.NewVersionUpdateCannotBeNull, "");
				}
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.CannotReadFileToUpdate, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private XDocument GetXDocument(XmlDocument document)
	{
		XDocument xDocument = new XDocument();
		using (XmlWriter w = xDocument.CreateWriter())
		{
			document.WriteTo(w);
		}
		XmlDeclaration xmlDeclaration = document.ChildNodes.OfType<XmlDeclaration>().FirstOrDefault();
		if (xmlDeclaration != null)
		{
			xDocument.Declaration = new XDeclaration(xmlDeclaration.Version, xmlDeclaration.Encoding, xmlDeclaration.Standalone);
		}
		return xDocument;
	}

	private void SaveFolder(string folderName, string userName, string passWord)
	{
		string text = WinFuntions.DownLoadFolderOnServer(folderName, WinFuntions._StartupPath, userName, passWord);
		if (!string.IsNullOrEmpty(text))
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text);
			XtraMessageBoxEx.Show(Enums.MessageCode.InfoDownloadFail, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		else
		{
			XtraMessageBoxEx.Show(Enums.MessageCode.InfoDownloadSuccess, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			Main.Exit = true;
			Main.IsRestart = true;
			Program.AplicationRestart();
		}
	}

	private void UpdateNetworkClient(Function.ConfigFTPServer ftp)
	{
		try
		{
			WinCommons._OpenProcessing();
			string text = ftp.Address;
			string folder = ftp.Folder;
			string user = ftp.User;
			string passWord = ftp.PassWord;
			if (!string.IsNullOrEmpty(ftp.Address) && ftp.Address.Length > 5 && ftp.Address.StartsWith("ftp://"))
			{
				text = ftp.Address.Substring(6, ftp.Address.Length - 6);
			}
			string text2 = $"{text}\\{folder}";
			string text3 = text2 + "\\NewVersion.xml";
			XElement xElement = ReadXml("\\\\" + text3, text, user, passWord).Element("NewVersion");
			if (xElement != null)
			{
				int idMax = xElement.Elements("Versions").Max((XElement obj) => int.Parse(obj.Attribute("Id").Value));
				XElement xElement2 = (from c in xElement.Elements("Versions")
					where (int)c.Attribute("Id") == idMax
					select c).SingleOrDefault();
				if (xElement2 != null)
				{
					NumberFormatInfo provider = new NumberFormatInfo
					{
						NumberDecimalSeparator = ".",
						NumberGroupSeparator = ","
					};
					XElement xElement3 = xElement2.Element("VersionAp");
					XElement xElement4 = xElement2.Element("VersionBuild");
					if (xElement3 != null && xElement4 != null)
					{
						double num = Convert.ToDouble(xElement3.Value, provider);
						int num2 = Function.ParseInt(xElement4.Value);
						if (xElement2.Element("FileName") != null)
						{
							XElement xElement5 = xElement2.Element("FolderName");
							if (xElement5 != null)
							{
								string value = xElement5.Value;
								string folderName = $"{text2}\\{value}";
								if (num > Versions.ApplicationVersion || num2 > Versions.BuildVersion)
								{
									WinCommons._CloseProcessing();
									if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdateNewVersion, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
									{
										WinCommons._OpenProcessing();
										SaveFolderNetwork(folderName, text, user, passWord);
									}
								}
							}
						}
					}
				}
			}
			txtUserName.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
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

	private void SaveFolderNetwork(string folderName, string computerName, string userName, string passWord)
	{
		string text = WinFuntions.CopyFolderFromNetwork("\\\\" + folderName, WinFuntions._StartupPath, computerName, userName, passWord);
		if (!string.IsNullOrEmpty(text))
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text);
			XtraMessageBoxEx.Show(Enums.MessageCode.InfoDownloadFail, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		else
		{
			Main.Exit = true;
			Main.IsRestart = true;
			Program.AplicationRestart();
		}
	}

	private void CheckUpdateFolderShareClient(Function.ConfigFTPServer ftp)
	{
		try
		{
			string[] array = Configuration.ApiServer.Split(new string[1] { "//" }, StringSplitOptions.None);
			if (array.Length <= 1)
			{
				WinCommons.Warning(Enums.MessageCode.CannotCheck4UpdateClient, "");
				return;
			}
			array = array[1].Split(':');
			string pathFile = string.Format("\\\\{0}\\{1}\\Update\\NewVersion.xml", array[0], "APZON-IRS Client");
			XDocument xDocument = ReadXml(pathFile, array[0], ftp.User, ftp.PassWord);
			if (xDocument == null)
			{
				WinCommons.Error(Enums.MessageCode.CannotReadFileToUpdate, "");
				return;
			}
			XElement xElement = xDocument.Element("NewVersion");
			if (xElement != null)
			{
				int idMax = xElement.Elements("Versions").Max((XElement obj) => int.Parse(obj.Attribute("Id").Value));
				XElement xElement2 = (from c in xElement.Elements("Versions")
					where (int)c.Attribute("Id") == idMax
					select c).SingleOrDefault();
				if (xElement2 != null)
				{
					NumberFormatInfo provider = new NumberFormatInfo
					{
						NumberDecimalSeparator = ".",
						NumberGroupSeparator = ","
					};
					XElement xElement3 = xElement2.Element("VersionAp");
					XElement xElement4 = xElement2.Element("VersionBuild");
					if (xElement3 != null && xElement4 != null)
					{
						double num = Convert.ToDouble(xElement3.Value, provider);
						int num2 = Function.ParseInt(xElement4.Value);
						if (num > Versions.ApplicationVersion || num2 > Versions.BuildVersion)
						{
							WinCommons._CloseProcessing();
							if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdateNewVersion, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
							{
								WinCommons._OpenProcessing();
								string sourceFolder = string.Format("\\\\{0}\\{1}\\Update\\{2}", array[0], "APZON-IRS Client", GetFolderUpdateByPlatform());
								CopyFolder(sourceFolder, Application.StartupPath);
								XtraMessageBoxEx.Show(Enums.MessageCode.InfoRestartSystem, Enums.MessageCode.SystemMessage);
								Main.Exit = true;
								Main.IsRestart = true;
								Program.AplicationRestart();
							}
						}
					}
					else
					{
						WinCommons.Warning(Enums.MessageCode.VersionUpdateCannotBeNull, "");
					}
				}
				else
				{
					WinCommons.Warning(Enums.MessageCode.NewVersionUpdateCannotBeNull, "");
				}
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.CannotReadFileToUpdate, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Warning(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private string GetFolderUpdateByPlatform()
	{
		if (Marshal.SizeOf(typeof(IntPtr)) == 8)
		{
			return "Updatex64";
		}
		return "Updatex86";
	}

	public bool CopyFolder(string sourceFolder, string destFolder)
	{
		if (Directory.Exists(destFolder))
		{
			MoveFileToFolderOld(destFolder, sourceFolder, $"{destFolder}\\Old");
		}
		if (!Directory.Exists(destFolder))
		{
			Directory.CreateDirectory(destFolder);
		}
		string[] files = Directory.GetFiles(sourceFolder);
		foreach (string text in files)
		{
			string fileName = Path.GetFileName(text);
			if (fileName != null)
			{
				string destFileName = Path.Combine(destFolder, fileName);
				File.Copy(text, destFileName);
			}
		}
		files = Directory.GetDirectories(sourceFolder);
		foreach (string text2 in files)
		{
			string fileName2 = Path.GetFileName(text2);
			if (fileName2 != null)
			{
				string destFolder2 = Path.Combine(destFolder, fileName2);
				CopyFolder(text2, destFolder2);
			}
		}
		return true;
	}

	private void MoveFileToFolderOld(string destFolder, string sourceFolder, string oldFolder)
	{
		if (!Directory.Exists(destFolder))
		{
			return;
		}
		if (Directory.Exists(oldFolder))
		{
			DeleteFolder(oldFolder);
		}
		Directory.CreateDirectory(oldFolder);
		DirectoryInfo directoryInfo = new DirectoryInfo(oldFolder);
		DirectorySecurity accessControl = directoryInfo.GetAccessControl();
		accessControl.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
		directoryInfo.SetAccessControl(accessControl);
		string[] files = Directory.GetFiles(sourceFolder);
		string newVersion = "";
		string[] array = files;
		for (int i = 0; i < array.Length; i++)
		{
			string fileName = Path.GetFileName(array[i]);
			if (fileName != null)
			{
				WinFuntions.MoveFileOldVersionToFolderOld(destFolder, oldFolder, fileName, ref newVersion);
			}
		}
		array = Directory.GetDirectories(sourceFolder);
		for (int i = 0; i < array.Length; i++)
		{
			string fileName2 = Path.GetFileName(array[i]);
			if (fileName2 != null)
			{
				string text = Path.Combine(destFolder, fileName2);
				string oldFolder2 = Path.Combine(oldFolder, fileName2);
				if (Directory.Exists(text))
				{
					MoveFileToFolderOld(text, text, oldFolder2);
				}
			}
		}
	}

	public void DeleteFolder(string sourceFolder)
	{
		try
		{
			if (Directory.Exists(sourceFolder))
			{
				string[] files = Directory.GetFiles(sourceFolder);
				for (int i = 0; i < files.Length; i++)
				{
					File.Delete(files[i]);
				}
				files = Directory.GetDirectories(sourceFolder);
				foreach (string sourceFolder2 in files)
				{
					DeleteFolder(sourceFolder2);
				}
				Function.DeleteFolder(sourceFolder);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async Task LoginClick()
	{
		string dbName = Function.ToString(txtDatabase.EditValue);
		WinCommons._OpenProcessing();
		EnableDisableControl(isEnable: false);
		try
		{
			LoginImpl pr = new LoginImpl
			{
				Doimain = Function.Encrypt(RSA.PublicKey, txt_Domain.Text),
				UserName = Function.Encrypt(RSA.PublicKey, txtUserName.Text),
				Password = Function.Encrypt(RSA.PublicKey, txtPassword.Text),
				DbName = Function.Encrypt(RSA.PublicKey, dbName),
				ClientVersion = Function.Encrypt(RSA.PublicKey, Function.ConvertDoubleToStringEnUs(Versions.ApplicationVersion, 2, 1)),
				BuildVersion = Function.Encrypt(RSA.PublicKey, Function.ConvertDoubleToStringEnUs(Versions.BuildVersion, 0, 3)),
				ClientLocalIpAdress = GetLocalIPAddress(),
				ClientMachineName = GetMachineName(),
				ClientWindowsLogonName = GetWindowsLogonName()
			};
			using HttpClientHelper<LoginResult> http = new HttpClientHelper<LoginResult>(Configuration.ApiServer, "LoginIRS");
			LoginResult loginResult = await http.PostAsync("Login", pr);
			if (!string.IsNullOrEmpty(loginResult.Message))
			{
				if (Function.Decrypt(RSA.PrivateKey, loginResult.Message).Equals("success"))
				{
					ConfigLogin loginConfig = new ConfigLogin
					{
						Remeber = "1",
						Domain = txt_Domain.Text,
						Database = dbName,
						User = txtUserName.Text,
						RePass = "0",
						PassWord = txtPassword.Text
					};
					Task.Run(() =>
					{
						WinFuntions.setConfigLogin(loginConfig);
					});
					if ((!string.IsNullOrEmpty(GlobalData.UserName) && !GlobalData.UserName.ToUpper().Equals(txtUserName.Text.ToUpper())) || (!string.IsNullOrEmpty(GlobalData.DatabaseName) && !GlobalData.DatabaseName.ToUpper().Equals(dbName)))
					{
						GlobalData.DefaultBranch = -1;
						ChangeUser = true;
					}
					else
					{
						ChangeUser = false;
					}
					string[] array = Function.Decrypt(RSA.PrivateKey, loginResult.Content.ToString()).Split(',');
					string text = array[0];
					GlobalData.UserLicense = loginResult.LicenseInfo;
					GlobalData.LoginSap = array[2].Equals("1");
					GlobalData.SessionId = Function.ToString(text);
					GlobalData.UserName = txtUserName.Text;
					GlobalData.DatabaseName = dbName;
					GlobalData.Domain = txt_Domain.Text;
					GlobalData.CompanyName = $"{txt_Domain.Text} - {dbName} - {txtDatabase.Text}";
					GlobalData.OldPassword = EncodePassword.EncryptString(txtPassword.Text, RSA.PasswordPhrase);
					Program.IsLoginOk = true;
					Login = true;
					DialogResult = DialogResult.OK;
					Close();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, Function.Decrypt(RSA.PrivateKey, loginResult.Message));
					BeginInvoke((Action)(() =>
					{
						txtPassword.Focus();
						_focus = true;
					}));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableDisableControl(isEnable: true);
		WinCommons._CloseProcessing();
	}

	private string GetLocalIPAddress()
	{
		try
		{
			IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
			foreach (IPAddress iPAddress in addressList)
			{
				if (iPAddress.AddressFamily == AddressFamily.InterNetwork)
				{
					return iPAddress.ToString();
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
		return "";
	}

	private string GetMachineName()
	{
		try
		{
			return Environment.MachineName;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
		return "";
	}

	private string GetWindowsLogonName()
	{
		try
		{
			return WindowsIdentity.GetCurrent().Name;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
		return "";
	}

	private async Task LoadListDatabase(string domain)
	{
		DbList = await HttpClientHelperFactory.GetHttpClient().Commons.GetDatabaseListByByDomain(domain);
		txtDatabase.Properties.DataSource = DbList;
	}

	private void EnableDisableControl(bool isEnable)
	{
		try
		{
			btnLogin.Enabled = isEnable;
			btnCancel.Enabled = isEnable;
			btnReset.Enabled = isEnable;
			btnSignUp.Enabled = isEnable;
			txt_Domain.Enabled = isEnable;
			txtDatabase.Properties.Enabled = isEnable;
			txtPassword.Enabled = isEnable;
			txtUserName.Enabled = isEnable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ReadFileCertificate()
	{
		try
		{
			List<Common.Table_Code> list = (_domainCert = WinFuntions.GetDomainList());
			list.Add(new Common.Table_Code
			{
				Name = "--Import domain--",
				Code = "-12345678"
			});
			txt_Domain.Properties.DataSource = list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ImportFileCertificate()
	{
		try
		{
			using frm_FileDialog frm_FileDialog2 = new frm_FileDialog(Enums.TypeCFL.ImportFileCertificate, multiselect: false);
			frm_FileDialog2.ShowDialog();
			ReadFileCertificate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtPassword_Enter(object sender, EventArgs e)
	{
		if (sender is TextEditEx textEditEx)
		{
			textEditEx.SelectAll();
		}
	}

	private void txtPassword_Leave(object sender, EventArgs e)
	{
		_focus = false;
	}

	private void txtPassword_MouseUp(object sender, MouseEventArgs e)
	{
		if (sender is TextEditEx { SelectedText: "" } textEditEx && !_focus)
		{
			textEditEx.SelectAll();
			_focus = true;
		}
	}

	private void txtPassword_KeyUp(object sender, KeyEventArgs e)
	{
		if (e.KeyData == Keys.Tab)
		{
			_focus = true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.frm_LoginSap));
		this.imageSliderBackground = new DevExpress.XtraEditors.Controls.ImageSlider();
		this.panel1 = new System.Windows.Forms.Panel();
		this.txt_Domain = new DevExpress.XtraEditors.LookUpEdit();
		this.lbDomain = new Apzon.Controls.Controls.LabelLine();
		this.btnSignUp = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtDatabase = new DevExpress.XtraEditors.LookUpEdit();
		this.lblVersion = new DevExpress.XtraEditors.LabelControl();
		this.lbDatabase = new Apzon.Controls.Controls.LabelLine();
		this.btnReset = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtPassword = new Apzon.Controls.Editor.TextEditEx();
		this.lbPass = new Apzon.Controls.Controls.LabelLine();
		this.txtUserName = new Apzon.Controls.Editor.TextEditEx();
		this.lbUser = new Apzon.Controls.Controls.LabelLine();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnLogin = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.imageSliderBackground).BeginInit();
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_Domain.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtDatabase.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPassword.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtUserName.Properties).BeginInit();
		base.SuspendLayout();
		this.imageSliderBackground.Cursor = System.Windows.Forms.Cursors.Default;
		this.imageSliderBackground.Location = new System.Drawing.Point(0, 0);
		this.imageSliderBackground.Name = "imageSliderBackground";
		this.imageSliderBackground.Size = new System.Drawing.Size(264, 283);
		this.imageSliderBackground.TabIndex = 0;
		this.imageSliderBackground.Text = "imageSlider1";
		this.panel1.BackColor = System.Drawing.Color.White;
		this.panel1.Controls.Add(this.txt_Domain);
		this.panel1.Controls.Add(this.lbDomain);
		this.panel1.Controls.Add(this.btnSignUp);
		this.panel1.Controls.Add(this.txtDatabase);
		this.panel1.Controls.Add(this.lblVersion);
		this.panel1.Controls.Add(this.lbDatabase);
		this.panel1.Controls.Add(this.btnReset);
		this.panel1.Controls.Add(this.txtPassword);
		this.panel1.Controls.Add(this.lbPass);
		this.panel1.Controls.Add(this.txtUserName);
		this.panel1.Controls.Add(this.lbUser);
		this.panel1.Controls.Add(this.btnCancel);
		this.panel1.Controls.Add(this.btnLogin);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
		this.panel1.Location = new System.Drawing.Point(264, 0);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(270, 282);
		this.panel1.TabIndex = 1;
		this.txt_Domain.Location = new System.Drawing.Point(78, 79);
		this.txt_Domain.Name = "txt_Domain";
		this.txt_Domain.Properties.AllowFocused = false;
		this.txt_Domain.Properties.AllowMouseWheel = false;
		this.txt_Domain.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.txt_Domain.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txt_Domain.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Domain"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.txt_Domain.Properties.DisplayMember = "Name";
		this.txt_Domain.Properties.DropDownRows = 5;
		this.txt_Domain.Properties.NullText = "";
		this.txt_Domain.Properties.ShowHeader = false;
		this.txt_Domain.Properties.ValueMember = "Code";
		this.txt_Domain.Size = new System.Drawing.Size(176, 20);
		this.txt_Domain.TabIndex = 14;
		this.txt_Domain.ToolTip = "Choose Domain";
		this.txt_Domain.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_Domain_EditValueChanging);
		this.lbDomain.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lbDomain.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.lbDomain.Appearance.Options.UseBackColor = true;
		this.lbDomain.EditControl = null;
		this.lbDomain.EditValue = "Domain";
		this.lbDomain.FieldName = null;
		this.lbDomain.HideWhenEmpty = false;
		this.lbDomain.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbDomain.Location = new System.Drawing.Point(6, 82);
		this.lbDomain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbDomain.Name = "lbDomain";
		this.lbDomain.ReadOnly = false;
		this.lbDomain.SettingInVisible = false;
		this.lbDomain.SettingReadonly = false;
		this.lbDomain.ShowUnderLine = true;
		this.lbDomain.Size = new System.Drawing.Size(77, 20);
		this.lbDomain.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbDomain.TabIndex = 5;
		this.lbDomain.Utext = "Domain";
		this.btnSignUp.Appearance.ForeColor = System.Drawing.Color.Green;
		this.btnSignUp.Appearance.Options.UseForeColor = true;
		this.btnSignUp.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.btnSignUp.EditControl = null;
		this.btnSignUp.EditValue = "Sign up";
		this.btnSignUp.FieldName = null;
		this.btnSignUp.IsSearchControl = false;
		this.btnSignUp.Location = new System.Drawing.Point(194, 252);
		this.btnSignUp.Name = "btnSignUp";
		this.btnSignUp.ReadOnly = false;
		this.btnSignUp.SettingInVisible = false;
		this.btnSignUp.SettingReadonly = false;
		this.btnSignUp.Size = new System.Drawing.Size(69, 23);
		this.btnSignUp.TabIndex = 6;
		this.btnSignUp.Text = "Sign up";
		this.btnSignUp.Visible = false;
		this.txtDatabase.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.txtDatabase.Location = new System.Drawing.Point(78, 104);
		this.txtDatabase.Name = "txtDatabase";
		this.txtDatabase.Properties.AllowFocused = false;
		this.txtDatabase.Properties.AllowMouseWheel = false;
		this.txtDatabase.Properties.Appearance.Options.UseTextOptions = true;
		this.txtDatabase.Properties.Appearance.TextOptions.Trimming = DevExpress.Utils.Trimming.None;
		this.txtDatabase.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.txtDatabase.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtDatabase.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.txtDatabase.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.txtDatabase.Properties.DisplayMember = "Name";
		this.txtDatabase.Properties.NullText = "";
		this.txtDatabase.Properties.ShowHeader = false;
		this.txtDatabase.Properties.ValueMember = "ID";
		this.txtDatabase.Size = new System.Drawing.Size(176, 20);
		this.txtDatabase.TabIndex = 15;
		this.txtDatabase.ToolTip = "Choose Company";
		this.txtDatabase.EditValueChanged += new System.EventHandler(this.look_Database_EditValueChanged);
		this.lblVersion.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.lblVersion.Appearance.Font = new System.Drawing.Font("Tahoma", 8.26f);
		this.lblVersion.Appearance.Options.UseFont = true;
		this.lblVersion.Location = new System.Drawing.Point(13, 257);
		this.lblVersion.Name = "lblVersion";
		this.lblVersion.Size = new System.Drawing.Size(42, 13);
		this.lblVersion.TabIndex = 7;
		this.lblVersion.Text = "Version: ";
		this.lbDatabase.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lbDatabase.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.lbDatabase.Appearance.Options.UseBackColor = true;
		this.lbDatabase.EditControl = null;
		this.lbDatabase.EditValue = "Database";
		this.lbDatabase.FieldName = null;
		this.lbDatabase.HideWhenEmpty = false;
		this.lbDatabase.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbDatabase.Location = new System.Drawing.Point(6, 107);
		this.lbDatabase.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbDatabase.Name = "lbDatabase";
		this.lbDatabase.ReadOnly = false;
		this.lbDatabase.SettingInVisible = false;
		this.lbDatabase.SettingReadonly = false;
		this.lbDatabase.ShowUnderLine = true;
		this.lbDatabase.Size = new System.Drawing.Size(77, 20);
		this.lbDatabase.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbDatabase.TabIndex = 8;
		this.lbDatabase.Utext = "Database";
		this.btnReset.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnReset.AppearanceHovered.FontSizeDelta = 2;
		this.btnReset.AppearanceHovered.FontStyleDelta = System.Drawing.FontStyle.Bold;
		this.btnReset.AppearanceHovered.Options.UseFont = true;
		this.btnReset.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.btnReset.EditControl = null;
		this.btnReset.EditValue = "Refresh";
		this.btnReset.FieldName = null;
		this.btnReset.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnReset.ImageOptions.Image");
		this.btnReset.IsSearchControl = false;
		this.btnReset.Location = new System.Drawing.Point(100, 192);
		this.btnReset.Name = "btnReset";
		this.btnReset.ReadOnly = false;
		this.btnReset.SettingInVisible = false;
		this.btnReset.SettingReadonly = false;
		this.btnReset.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
		this.btnReset.Size = new System.Drawing.Size(74, 23);
		this.btnReset.TabIndex = 9;
		this.btnReset.Text = "Refresh";
		this.btnReset.Click += new System.EventHandler(this.btn_Reset_Click);
		this.txtPassword.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.txtPassword.EditControl = null;
		this.txtPassword.FieldName = null;
		this.txtPassword.IsSearchControl = false;
		this.txtPassword.Location = new System.Drawing.Point(78, 156);
		this.txtPassword.Name = "txtPassword";
		this.txtPassword.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.txtPassword.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtPassword.Properties.PasswordChar = '•';
		this.txtPassword.SettingInVisible = false;
		this.txtPassword.SettingReadonly = false;
		this.txtPassword.Size = new System.Drawing.Size(176, 20);
		this.txtPassword.TabIndex = 17;
		this.txtPassword.Enter += new System.EventHandler(this.txtPassword_Enter);
		this.txtPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPassword_KeyDown);
		this.txtPassword.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPassword_KeyUp);
		this.txtPassword.Leave += new System.EventHandler(this.txtPassword_Leave);
		this.txtPassword.MouseUp += new System.Windows.Forms.MouseEventHandler(this.txtPassword_MouseUp);
		this.txtPassword.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtPassword_PreviewKeyDown);
		this.lbPass.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lbPass.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.lbPass.Appearance.Options.UseBackColor = true;
		this.lbPass.EditControl = null;
		this.lbPass.EditValue = "Password";
		this.lbPass.FieldName = null;
		this.lbPass.HideWhenEmpty = false;
		this.lbPass.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbPass.Location = new System.Drawing.Point(6, 159);
		this.lbPass.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbPass.Name = "lbPass";
		this.lbPass.ReadOnly = false;
		this.lbPass.SettingInVisible = false;
		this.lbPass.SettingReadonly = false;
		this.lbPass.ShowUnderLine = true;
		this.lbPass.Size = new System.Drawing.Size(77, 20);
		this.lbPass.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbPass.TabIndex = 10;
		this.lbPass.Utext = "Password";
		this.txtUserName.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.txtUserName.EditControl = null;
		this.txtUserName.EnterMoveNextControl = true;
		this.txtUserName.FieldName = null;
		this.txtUserName.IsSearchControl = false;
		this.txtUserName.Location = new System.Drawing.Point(78, 130);
		this.txtUserName.Name = "txtUserName";
		this.txtUserName.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.txtUserName.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.txtUserName.SettingInVisible = false;
		this.txtUserName.SettingReadonly = false;
		this.txtUserName.Size = new System.Drawing.Size(176, 20);
		this.txtUserName.TabIndex = 16;
		this.txtUserName.Enter += new System.EventHandler(this.txtPassword_Enter);
		this.txtUserName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPassword_KeyUp);
		this.txtUserName.Leave += new System.EventHandler(this.txtPassword_Leave);
		this.txtUserName.MouseUp += new System.Windows.Forms.MouseEventHandler(this.txtPassword_MouseUp);
		this.lbUser.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lbUser.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.lbUser.Appearance.Options.UseBackColor = true;
		this.lbUser.EditControl = null;
		this.lbUser.EditValue = "Username";
		this.lbUser.FieldName = null;
		this.lbUser.HideWhenEmpty = false;
		this.lbUser.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbUser.Location = new System.Drawing.Point(6, 133);
		this.lbUser.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbUser.Name = "lbUser";
		this.lbUser.ReadOnly = false;
		this.lbUser.SettingInVisible = false;
		this.lbUser.SettingReadonly = false;
		this.lbUser.ShowUnderLine = true;
		this.lbUser.Size = new System.Drawing.Size(77, 20);
		this.lbUser.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbUser.TabIndex = 11;
		this.lbUser.Utext = "Username";
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.AppearanceHovered.FontSizeDelta = 2;
		this.btnCancel.AppearanceHovered.FontStyleDelta = System.Drawing.FontStyle.Bold;
		this.btnCancel.AppearanceHovered.Options.UseFont = true;
		this.btnCancel.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Exit";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(180, 192);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
		this.btnCancel.Size = new System.Drawing.Size(74, 23);
		this.btnCancel.TabIndex = 12;
		this.btnCancel.Text = "Exit";
		this.btnCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btnLogin.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnLogin.AppearanceHovered.FontSizeDelta = 2;
		this.btnLogin.AppearanceHovered.FontStyleDelta = System.Drawing.FontStyle.Bold;
		this.btnLogin.AppearanceHovered.Options.UseFont = true;
		this.btnLogin.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.btnLogin.EditControl = null;
		this.btnLogin.EditValue = "Login";
		this.btnLogin.FieldName = null;
		this.btnLogin.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnLogin.ImageOptions.Image");
		this.btnLogin.IsSearchControl = false;
		this.btnLogin.Location = new System.Drawing.Point(20, 192);
		this.btnLogin.Name = "btnLogin";
		this.btnLogin.ReadOnly = false;
		this.btnLogin.SettingInVisible = false;
		this.btnLogin.SettingReadonly = false;
		this.btnLogin.ShowFocusRectangle = DevExpress.Utils.DefaultBoolean.False;
		this.btnLogin.Size = new System.Drawing.Size(74, 23);
		this.btnLogin.TabIndex = 13;
		this.btnLogin.Text = "Login";
		this.btnLogin.Click += new System.EventHandler(this.btLogin_Click);
		base.AcceptButton = this.btnLogin;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.BackgroundImageLayoutStore = System.Windows.Forms.ImageLayout.Stretch;
		base.BackgroundImageStore = Apzon.Forms.Properties.Resources.Login0;
		base.ClientSize = new System.Drawing.Size(534, 282);
		base.Controls.Add(this.panel1);
		base.Controls.Add(this.imageSliderBackground);
		base.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Shadow;
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		base.IconOptions.ShowIcon = false;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frm_LoginSap";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Login";
		base.Load += new System.EventHandler(this.LoginSap_Load);
		((System.ComponentModel.ISupportInitialize)this.imageSliderBackground).EndInit();
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_Domain.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtDatabase.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPassword.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtUserName.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
