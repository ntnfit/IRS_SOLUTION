using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Apzon.Forms.Commons;

public class SmtpMailClient
{
	private SmtpClient _oSmtp;

	private string _emailFrom;

	private string _error;

	private string _fromName;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private async Task<bool> DownloadTokenFile()
	{
		_ = 1;
		try
		{
			DataTable dataTable = await _httpClient.MailSettingClient.GetDoc();
			if (dataTable.IsNotNull() && Function.ToString(dataTable.Rows[0]["SmtpMethod"]) == "T")
			{
				if (!Directory.Exists(Application.StartupPath + "\\Analytics.Auth.Store\\"))
				{
					Directory.CreateDirectory(Application.StartupPath + "\\Analytics.Auth.Store\\");
				}
				string text = await _httpClient.FileNetworkManagerClient.DownLoadFile("\\Report\\Google.Apis.Auth.OAuth2.Responses.TokenResponse-Apzon", Application.StartupPath + "\\Analytics.Auth.Store\\", isOverride: true);
				if (!string.IsNullOrEmpty(text))
				{
					Logging.Write("ERROR", text);
					return false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return false;
		}
		return true;
	}

	private async Task<string> TryConnect()
	{
		_ = 8;
		try
		{
			string smtpAddress = "";
			string emailFrom = "";
			string method = "";
			string clientId = "";
			string clientSecret = "";
			string userName = "";
			string password = "";
			string mailAddress = "";
			int portNumber = 0;
			int timeout = 0;
			bool enableSsl = false;
			bool useAuthen = false;
			DataTable dataTable = await _httpClient.MailSettingClient.GetDoc();
			if (dataTable.IsNotNull())
			{
				DataRow r = dataTable.Rows[0];
				method = Function.ToString(r["SmtpMethod"]);
				clientId = Function.ToString(r["ClientId"]);
				clientSecret = Function.ToString(r["ClientSecret"]);
				userName = Function.ToString(r["UserName"]);
				timeout = Function.ParseInt(r["Timeout"]);
				smtpAddress = Function.ToString(r["ServerAddress"]);
				portNumber = Function.ParseInt(r["SSLPort"]);
				string fullName = Function.ToString(r["FullName"]);
				enableSsl = Function.ToString(r["EnableSSL"]) == "Y";
				mailAddress = Function.ToString(r["MailAddress"]);
				useAuthen = Function.ToString(r["UseAuthen"]) == "Y";
				DataRow dataRow = (await _httpClient.SubActivities.GetUnlockedUsers()).AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["USERID"]) == GlobalData.UserID);
				switch (method)
				{
				case "N":
					emailFrom = userName;
					if (!string.IsNullOrEmpty(Function.ToString(r["Password"])))
					{
						password = Function.Decrypt(RSA.PrivateKey, Function.ToString(r["Password"]));
					}
					if (useAuthen && (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(userName)))
					{
						_error = "You must setup Smtp Credentials!";
					}
					break;
				case "T":
					if (!string.IsNullOrEmpty(mailAddress))
					{
						emailFrom = mailAddress;
					}
					else
					{
						_error = "Sender's email is missing!";
					}
					if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
					{
						_error = "ClientId or ClientSecret is missing!";
					}
					break;
				case "S":
					if (dataRow != null && !string.IsNullOrEmpty(Function.ToString(dataRow["E_Mail"])))
					{
						string text = Function.ToString(dataRow["E_Mail"]);
						emailFrom = text;
					}
					else if (!string.IsNullOrEmpty(mailAddress))
					{
						emailFrom = mailAddress;
					}
					else
					{
						_error = "Sender's email is missing!";
					}
					if (!string.IsNullOrEmpty(Function.ToString(r["Password"])))
					{
						password = Function.Decrypt(RSA.PrivateKey, Function.ToString(r["Password"]));
					}
					if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(userName))
					{
						_error = "You must setup Smtp Credentials!";
					}
					break;
				}
				if (dataRow != null && !string.IsNullOrEmpty(Function.ToString(dataRow["U_NAME"])))
				{
					_fromName = Function.ToString(dataRow["U_NAME"]);
				}
				else if (string.IsNullOrEmpty(fullName))
				{
					_fromName = fullName;
				}
				else
				{
					_error = "You must setup Mail From's Display Name!";
				}
				_emailFrom = emailFrom;
			}
			else
			{
				_error = "You must setup Mail Smtp Setting first!";
			}
			if (!string.IsNullOrWhiteSpace(_error))
			{
				return _error;
			}
			_oSmtp = new SmtpClient
			{
				Timeout = timeout * 1000,
				ServerCertificateValidationCallback = (object s, X509Certificate c, X509Chain h, SslPolicyErrors e) => true
			};
			await _oSmtp.ConnectAsync(smtpAddress, portNumber, enableSsl);
			switch (method)
			{
			case "N":
				if (useAuthen)
				{
					await _oSmtp.AuthenticateAsync(userName, password);
				}
				break;
			case "S":
				await _oSmtp.AuthenticateAsync(userName, password);
				break;
			case "T":
				if (await DownloadTokenFile())
				{
					UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
					{
						ClientId = clientId,
						ClientSecret = clientSecret
					}, new string[1] { "https://mail.google.com/" }, "Apzon", CancellationToken.None, new FileDataStore(Application.StartupPath + "\\Analytics.Auth.Store\\"));
					if (!(await credential.RefreshTokenAsync(CancellationToken.None)))
					{
						Logging.Write("TRACE", "Refresh Token FAILED!");
					}
					SaslMechanism mechanism = new SaslMechanismOAuth2(mailAddress, credential.Token.AccessToken);
					await _oSmtp.AuthenticateAsync(mechanism);
				}
				else
				{
					_error = "Can not download Token file! Please check log for more detail";
				}
				break;
			}
			return _error;
		}
		catch (Exception ex)
		{
			_error = ex.Message;
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	private void SaveDraft()
	{
		using ImapClient imapClient = new ImapClient();
		try
		{
			imapClient.Connect(ConfigurationManager.AppSettings["ImapServer"], int.Parse(ConfigurationManager.AppSettings["ImapPort"]));
			imapClient.AuthenticationMechanisms.Remove("XOAUTH2");
			imapClient.Authenticate("thanhluantl2304@gmail.com", "komdnnaermhgbrrp");
			MimeMessage mimeMessage = new MimeMessage();
			mimeMessage.From.Add(new MailboxAddress("Luan Apzon", "luannt@apzon.com"));
			mimeMessage.To.Add(new MailboxAddress("thanhluantl2304@gmail.com"));
			mimeMessage.Subject = "Helooooooooooooooooooo Bay beeeeeeeeeeeee!";
			new BodyBuilder();
			IMailFolder folder = imapClient.GetFolder(SpecialFolder.Drafts);
			if (folder != null)
			{
				folder.Open(FolderAccess.ReadWrite);
				folder.Append(mimeMessage, MessageFlags.Draft);
				folder.Expunge();
			}
			else
			{
				IMailFolder mailFolder = imapClient.GetFolder(imapClient.PersonalNamespaces[0]).Create(SpecialFolder.Drafts.ToString(), isMessageFolder: true);
				mailFolder.Open(FolderAccess.ReadWrite);
				mailFolder.Append(mimeMessage, MessageFlags.Draft);
				mailFolder.Expunge();
			}
		}
		catch (Exception ex)
		{
			throw new ApplicationException("IMAPException has occured: " + ex.Message);
		}
		imapClient.Disconnect(quit: true);
	}

	public async Task<string> SendEmail(List<string> listTo, string subject, string body, string fromName = null, List<string> cc = null, List<string> bcc = null, List<string> listAttachFile = null)
	{
		_ = 2;
		try
		{
			string text = await TryConnect();
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			if (listTo.Count <= 0)
			{
				return "No received email found!";
			}
			MimeMessage mimeMessage = new MimeMessage();
			if (fromName != null)
			{
				mimeMessage.From.Add(new MailboxAddress(fromName, _emailFrom));
			}
			else
			{
				mimeMessage.From.Add(new MailboxAddress(_fromName, _emailFrom));
			}
			foreach (string item in listTo.Where((string t) => !string.IsNullOrEmpty(t)))
			{
				mimeMessage.To.Add(new MailboxAddress(item));
			}
			if (cc != null)
			{
				foreach (string item2 in cc.Where((string t) => !string.IsNullOrEmpty(t)))
				{
					mimeMessage.Cc.Add(new MailboxAddress(item2));
				}
			}
			if (bcc != null)
			{
				foreach (string item3 in bcc.Where((string t) => !string.IsNullOrEmpty(t)))
				{
					mimeMessage.Bcc.Add(new MailboxAddress(item3));
				}
			}
			mimeMessage.Subject = subject;
			BodyBuilder bodyBuilder = new BodyBuilder();
			bodyBuilder.HtmlBody = body;
			if (listAttachFile != null && listAttachFile.Count > 0)
			{
				foreach (string item4 in listAttachFile)
				{
					bodyBuilder.Attachments.Add(item4);
				}
			}
			mimeMessage.Body = bodyBuilder.ToMessageBody();
			try
			{
				await _oSmtp.SendAsync(mimeMessage);
				await _oSmtp.DisconnectAsync(quit: true);
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
				return ex.Message;
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.ToString());
			return ex2.Message;
		}
		return string.Empty;
	}

	public async Task<string> SendEmail(List<string> listTo, string subject, string body, string fromName = null, List<string> cc = null, List<string> bcc = null, Dictionary<string, Stream> listAttachFile = null)
	{
		_ = 2;
		try
		{
			string text = await TryConnect();
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			if (listTo.Count <= 0)
			{
				return "No received email found!";
			}
			MimeMessage mimeMessage = new MimeMessage();
			if (fromName != null)
			{
				mimeMessage.From.Add(new MailboxAddress(fromName, _emailFrom));
			}
			else
			{
				mimeMessage.From.Add(new MailboxAddress(_fromName, _emailFrom));
			}
			foreach (string item in listTo.Where((string t) => !string.IsNullOrEmpty(t)))
			{
				mimeMessage.To.Add(new MailboxAddress(item));
			}
			if (cc != null)
			{
				foreach (string item2 in cc.Where((string t) => !string.IsNullOrEmpty(t)))
				{
					mimeMessage.Cc.Add(new MailboxAddress(item2));
				}
			}
			if (bcc != null)
			{
				foreach (string item3 in bcc.Where((string t) => !string.IsNullOrEmpty(t)))
				{
					mimeMessage.Bcc.Add(new MailboxAddress(item3));
				}
			}
			mimeMessage.Subject = subject;
			BodyBuilder bodyBuilder = new BodyBuilder();
			bodyBuilder.HtmlBody = body;
			if (listAttachFile != null && listAttachFile.Count > 0)
			{
				foreach (KeyValuePair<string, Stream> item4 in listAttachFile)
				{
					bodyBuilder.Attachments.Add(item4.Key, item4.Value);
				}
			}
			mimeMessage.Body = bodyBuilder.ToMessageBody();
			try
			{
				await _oSmtp.SendAsync(mimeMessage);
				await _oSmtp.DisconnectAsync(quit: true);
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
				return ex.Message;
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.ToString());
			return ex2.Message;
		}
		return string.Empty;
	}
}
