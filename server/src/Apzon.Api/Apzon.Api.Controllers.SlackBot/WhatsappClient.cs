using System;
using WhatsAppApi;

namespace Apzon.Api.Controllers.SlackBot;

public class WhatsappClient
{
	private string _from = "84987818552";

	private string _pass = "";

	public void send(string user, string message)
	{
		WhatsApp wa = new WhatsApp(_from, _pass, "Thanh", debug: true);
		wa.OnConnectSuccess += () =>
		{
			wa.OnLoginSuccess += (string phone, byte[] data) =>
			{
				wa.SendMessage("841667442028", "hello");
			};
			wa.OnLoginFailed += (string data) =>
			{
			};
			wa.Login();
		};
		wa.OnConnectFailed += (Exception data) =>
		{
		};
		wa.Connect();
	}
}
