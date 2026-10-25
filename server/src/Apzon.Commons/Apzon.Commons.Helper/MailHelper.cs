using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows.Forms;
using Apzon.Commons.Constants;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Apzon.Commons.Helper;

public class MailHelper
{
	private SmtpClient _oSmtp;

	private string _emailFrom;

	private string _error;

	private string _fromName;

	public DataTable EmailConfig { get; set; }

	public DataTable UnlockedUsers { get; set; }

	public string TryConnect(int userId)
	{
		try
		{
			string host = "";
			string emailFrom = "";
			string text = "";
			string text2 = "";
			string text3 = "";
			string text4 = "";
			string text5 = "";
			string text6 = "";
			string text7 = "";
			int port = 0;
			int num = 0;
			bool useSsl = false;
			bool flag = false;
			DataTable emailConfig = EmailConfig;
			if (emailConfig.IsNotNull())
			{
				DataRow dataRow = emailConfig.Rows[0];
				text2 = Function.ToString(dataRow["SmtpMethod"]);
				text3 = Function.ToString(dataRow["ClientId"]);
				text4 = Function.ToString(dataRow["ClientSecret"]);
				text5 = Function.ToString(dataRow["UserName"]);
				num = Function.ParseInt(dataRow["Timeout"]);
				host = Function.ToString(dataRow["ServerAddress"]);
				port = Function.ParseInt(dataRow["SSLPort"]);
				text = Function.ToString(dataRow["FullName"]);
				useSsl = Function.ToString(dataRow["EnableSSL"]) == "Y";
				text7 = Function.ToString(dataRow["MailAddress"]);
				flag = Function.ToString(dataRow["UseAuthen"]) == "Y";
				_fromName = text;
				DataRow dataRow2 = UnlockedUsers.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["USERID"]) == userId);
				switch (text2)
				{
				case "N":
					emailFrom = text5;
					if (!string.IsNullOrEmpty(Function.ToString(dataRow["Password"])))
					{
						text6 = Function.Decrypt(RSA.PrivateKey, Function.ToString(dataRow["Password"]));
					}
					if (flag && (string.IsNullOrEmpty(text6) || string.IsNullOrEmpty(text5)))
					{
						_error = "You must setup Smtp Credentials!";
					}
					break;
				case "T":
					if (!string.IsNullOrEmpty(text7))
					{
						emailFrom = text7;
					}
					else if (dataRow2 != null)
					{
						string text9 = Function.ToString(dataRow2["E_Mail"]);
						if (!string.IsNullOrEmpty(text9))
						{
							emailFrom = text9;
						}
						else
						{
							_error = "Sender's email is missing!";
						}
					}
					else
					{
						_error = "Sender's email is missing!";
					}
					if (string.IsNullOrEmpty(text3) || string.IsNullOrEmpty(text4))
					{
						_error = "ClientId or ClientSecret is missing!";
					}
					break;
				case "S":
					emailFrom = text7;
					if (string.IsNullOrEmpty(text7))
					{
						if (dataRow2 != null)
						{
							string text8 = Function.ToString(dataRow2["E_Mail"]);
							if (!string.IsNullOrEmpty(text8))
							{
								emailFrom = text8;
							}
							else
							{
								_error = "Sender's email is missing!";
							}
						}
						else
						{
							_error = "Sender's email is missing!";
						}
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataRow["Password"])))
					{
						text6 = Function.Decrypt(RSA.PrivateKey, Function.ToString(dataRow["Password"]));
					}
					if (string.IsNullOrEmpty(text6) || string.IsNullOrEmpty(text5))
					{
						_error = "You must setup Smtp Credentials!";
					}
					break;
				}
				_emailFrom = emailFrom;
			}
			else
			{
				_error = "You must setup Mail Smtp Setting first!";
			}
			_oSmtp = new SmtpClient
			{
				Timeout = num * 1000,
				ServerCertificateValidationCallback = (object s, X509Certificate c, X509Chain h, SslPolicyErrors e) => true
			};
			_oSmtp.Connect(host, port, useSsl);
			SaslMechanism saslMechanism = null;
			switch (text2)
			{
			case "N":
				if (flag)
				{
					saslMechanism = new SaslMechanismPlain(new NetworkCredential(text5, text6));
					_oSmtp.Authenticate(saslMechanism);
				}
				break;
			case "S":
				saslMechanism = new SaslMechanismPlain(new NetworkCredential(text5, text6));
				_oSmtp.Authenticate(saslMechanism);
				break;
			case "T":
			{
				UserCredential result = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets
				{
					ClientId = text3,
					ClientSecret = text4
				}, new string[1] { "https://mail.google.com/" }, "Apzon", CancellationToken.None, new FileDataStore(Application.StartupPath + "\\Analytics.Auth.Store\\")).Result;
				result.RefreshTokenAsync(CancellationToken.None);
				saslMechanism = new SaslMechanismOAuth2(text7, result.Token.AccessToken);
				_oSmtp.Authenticate(saslMechanism);
				break;
			}
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

	public string SendEmail(int currentUser, List<string> listTo, string subject, string body, string fromName = null, List<string> cc = null, List<string> bcc = null, List<string> listAttachFile = null)
	{
		try
		{
			string text = TryConnect(currentUser);
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
				_oSmtp.Send(mimeMessage);
				_oSmtp.Disconnect(quit: true);
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

	public string SendEmail(int currentUser, List<string> listTo, string subject, string body, string fromName = null, List<string> cc = null, List<string> bcc = null, Dictionary<string, Stream> listAttachFile = null)
	{
		try
		{
			string text = TryConnect(currentUser);
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
				_oSmtp.Send(mimeMessage);
				_oSmtp.Disconnect(quit: true);
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
