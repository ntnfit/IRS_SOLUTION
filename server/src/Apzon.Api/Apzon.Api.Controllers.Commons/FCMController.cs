using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Newtonsoft.Json;

namespace Apzon.Api.Controllers.Commons;

public class FCMController : BaseApiController
{
	private const string FcmEndpoint = "https://fcm.googleapis.com/fcm/send";
	private string fcmKey;

	[HttpPost]
	public async Task<string> SendAsync([FromBody] string notification)
	{
		_ = 1;
		try
		{
			HttpClient httpClient = CreateClient(ResolveFcmKey());
			httpClient.DefaultRequestHeaders.TryAddWithoutValidation("content-length", notification.Length.ToString());
			StringContent content = new StringContent(notification, Encoding.UTF8, "application/json");
			return await (await httpClient.PostAsync(FcmEndpoint, content)).Content.ReadAsStringAsync();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpPost]
	public async Task<string> SendMessageJson([FromBody] object model)
	{
		_ = 1;
		try
		{
			string text = JsonConvert.SerializeObject(model);
			HttpClient httpClient = CreateClient(ResolveFcmKey());
			httpClient.DefaultRequestHeaders.TryAddWithoutValidation("content-length", text.Length.ToString());
			StringContent content = new StringContent(text, Encoding.UTF8, "application/json");
			return await (await httpClient.PostAsync(FcmEndpoint, content)).Content.ReadAsStringAsync();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpPost]
	public async Task<string> SendMessageData([FromBody] dynamic data)
	{
		_ = 1;
		try
		{
			if (null == data)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Data is NULL!");
				return "Data is empty!";
			}
			fcmKey = Function.ToString(data.FcmKey);
			string title = Function.ToString(data.Title);
			string body = Function.ToString(data.Body);
			string newsId = Function.ToString(data.NewsId);
			string userId = Function.ToString(data.UserId);
			string regId = Function.ToString(data.RegId);
			string text = Function.ToString(data.RegIds);
			int badge = Function.ParseInt(data.Badge);
			string message;
			if (string.IsNullOrEmpty(text))
			{
				message = getMessage(title, body, newsId, userId, regId, badge);
			}
			else
			{
				string[] regIds = text.Split(',');
				message = getMessage(title, body, newsId, regIds, badge);
			}
			HttpClient httpClient = CreateClient(ResolveFcmKey(fcmKey));
			httpClient.DefaultRequestHeaders.TryAddWithoutValidation("content-length", message.Length.ToString());
			StringContent content = new StringContent(message, Encoding.UTF8, "application/json");
			return await (await httpClient.PostAsync(FcmEndpoint, content)).Content.ReadAsStringAsync();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpPost]
	public async Task<string> SendMessageDataTable([FromBody] DataTable data)
	{
		_ = 1;
		try
		{
			if (!data.IsNotNull())
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Data is NULL!");
				return "Data is empty!";
			}
			fcmKey = Function.ToString(data.Rows[0]["FcmKey"]);
			string text = "";
			foreach (DataRow row in data.Rows)
			{
				string title = Function.ToString(row["Title"]);
				string body = Function.ToString(row["Body"]);
				string newsId = Function.ToString(row["NewsId"]);
				string userId = Function.ToString(row["UserId"]);
				string regId = Function.ToString(row["RegId"]);
				int badge = Function.ParseInt(row["Badge"]);
				string message = getMessage(title, body, newsId, userId, regId, badge);
				HttpClient httpClient = CreateClient(ResolveFcmKey(fcmKey));
				httpClient.DefaultRequestHeaders.TryAddWithoutValidation("content-length", message.Length.ToString());
				StringContent content = new StringContent(message, Encoding.UTF8, "application/json");
				HttpResponseMessage response = await httpClient.PostAsync(FcmEndpoint, content);
				text = await response.Content.ReadAsStringAsync();
				if (!response.IsSuccessStatusCode)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text);
				}
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	private static HttpClient CreateClient(string serverKey)
	{
		HttpClient httpClient = new HttpClient();
		httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=" + serverKey);
		return httpClient;
	}

	private static string ResolveFcmKey(string suppliedKey = null)
	{
		string value = string.IsNullOrWhiteSpace(suppliedKey)
			? Environment.GetEnvironmentVariable("IRS_FIREBASE_SERVER_KEY")
			: suppliedKey;
		if (string.IsNullOrWhiteSpace(value))
		{
			throw new InvalidOperationException("IRS_FIREBASE_SERVER_KEY is required for Firebase notifications.");
		}
		return value;
	}

	[HttpGet]
	public string getMessage(string title, string body, string newsId, string userId, string regId, int badge = 1)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
		Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
		dictionary.Add("title", title);
		dictionary.Add("body", body);
		dictionary.Add("badge", badge);
		dictionary2.Add("user_id", userId);
		dictionary2.Add("news_id", newsId);
		dictionary3.Add("priority", "high");
		dictionary3.Add("notification", dictionary);
		dictionary3.Add("data", dictionary2);
		dictionary3.Add("to", regId);
		return JsonConvert.SerializeObject(dictionary3);
	}

	[HttpGet]
	public string getMessage(string title, string body, string newsId, string[] regIds, int badge = 1)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
		Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
		dictionary.Add("title", title);
		dictionary.Add("body", body);
		dictionary.Add("badge", badge);
		dictionary2.Add("news_id", newsId);
		dictionary3.Add("priority", "high");
		dictionary3.Add("notification", dictionary);
		dictionary3.Add("data", dictionary2);
		dictionary3.Add("registration_ids", regIds);
		return JsonConvert.SerializeObject(dictionary3);
	}
}
