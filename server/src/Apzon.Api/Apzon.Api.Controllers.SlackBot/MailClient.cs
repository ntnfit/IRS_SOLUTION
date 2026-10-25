using System;
using System.Net;
using System.Net.Mail;

namespace Apzon.Api.Controllers.SlackBot;

public class MailClient
{
	private string _from;

	private string _pass;

	private string _toEmail;

	public MailClient(string sender = null, string senderPassword = null, string toEmail = null)
	{
		_from = sender ?? Environment.GetEnvironmentVariable("IRS_SMTP_SENDER");
		_pass = senderPassword ?? Environment.GetEnvironmentVariable("IRS_SMTP_PASSWORD");
		_toEmail = toEmail ?? Environment.GetEnvironmentVariable("IRS_SMTP_RECIPIENT");
	}

	public string SendEmail(string subject, string body)
	{
		string result = "";
		try
		{
			if (string.IsNullOrWhiteSpace(_from) || string.IsNullOrWhiteSpace(_pass) || string.IsNullOrWhiteSpace(_toEmail))
			{
				return "SMTP configuration is missing.";
			}
			SmtpClient smtpClient = new SmtpClient();
			smtpClient.Host = "smtp.gmail.com";
			smtpClient.Port = 587;
			smtpClient.EnableSsl = true;
			smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
			smtpClient.Credentials = new NetworkCredential(_from, _pass);
			smtpClient.Timeout = 30000;
			MailMessage message = new MailMessage(_from, _toEmail, subject, body);
			smtpClient.Send(message);
		}
		catch (Exception)
		{
			result = "Error sending email.!!!";
		}
		return result;
	}
}
