using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Apzon.Api.Controllers.SlackBot;

public class SlackClient
{
	private readonly Uri _uri;
	private readonly Encoding _encoding = new UTF8Encoding();
	private string _channel;

	public SlackClient(string channel = "#test-support", string urlWithAccessToken = null)
	{
		string webhookUrl = urlWithAccessToken ?? Environment.GetEnvironmentVariable("IRS_SLACK_WEBHOOK_URL");
		if (string.IsNullOrWhiteSpace(webhookUrl))
		{
			throw new InvalidOperationException("IRS_SLACK_WEBHOOK_URL is required for Slack notifications.");
		}
		_uri = new Uri(webhookUrl);
		_channel = channel;
	}

	private void PostMessage(string text, string username = null, string channel = null)
	{
		PayLoad payload = new PayLoad
		{
			Channel = channel,
			Username = username,
			Text = text
		};
		PostMessage(payload);
	}

	private void PostMessage(PayLoad payload)
	{
		string value = JsonConvert.SerializeObject(payload);
		using WebClient webClient = new WebClient();
		NameValueCollection nameValueCollection = new NameValueCollection();
		nameValueCollection["payload"] = value;
		byte[] bytes = webClient.UploadValues(_uri, "POST", nameValueCollection);
		_encoding.GetString(bytes);
	}

	public void send(string message, string user = "Slack Bot")
	{
		PostMessage(message, user, _channel);
	}
}
