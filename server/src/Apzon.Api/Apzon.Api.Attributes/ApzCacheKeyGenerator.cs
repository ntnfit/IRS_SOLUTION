using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http.Controllers;
using Apzon.Commons;
using Newtonsoft.Json;
using WebApi.OutputCache.V2;

namespace Apzon.Api.Attributes;

public class ApzCacheKeyGenerator : DefaultCacheKeyGenerator
{
	public override string MakeCacheKey(HttpActionContext context, MediaTypeHeaderValue mediaType, bool excludeQueryString = false)
	{
		try
		{
			string text = MakeBaseKey(context);
			string text2 = FormatParameters(context, excludeQueryString);
			string key = Function.ToString(context.Request.Headers.GetValues("SessionId").FirstOrDefault());
			string text3 = (Global.SessionUserInfo.ContainsKey(key) ? Global.SessionUserInfo[key].DatabaseName : "");
			return $"{text}{text2}:{text3}:{mediaType}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return MakeBaseKey(context);
		}
	}

	protected virtual string MakeBaseKey(HttpActionContext context)
	{
		string fullName = context.ControllerContext.ControllerDescriptor.ControllerType.FullName;
		string actionName = context.ActionDescriptor.ActionName;
		return context.Request.GetConfiguration().CacheOutputConfiguration().MakeBaseCachekey(fullName, actionName);
	}

	protected virtual string FormatParameters(HttpActionContext context, bool excludeQueryString)
	{
		try
		{
			IEnumerable<string> enumerable = from x in context.ActionArguments
				where x.Value != null
				select x.Key + "=" + GetValue(x.Value);
			string text;
			if (!excludeQueryString)
			{
				text = PrepareGetParameters(context, enumerable);
				if (context.Request.Method == HttpMethod.Post)
				{
					foreach (KeyValuePair<string, object> item in JsonConvert.DeserializeObject<Dictionary<string, object>>(context.Request.Content.ReadAsStringAsync().Result, new JsonSerializerSettings
					{
						NullValueHandling = NullValueHandling.Ignore,
						Formatting = Formatting.None,
						DefaultValueHandling = DefaultValueHandling.Ignore
					}))
					{
						if (!string.IsNullOrEmpty(item.Key) && !string.IsNullOrEmpty(Function.ToString(item.Value)))
						{
							text = text + item.Key.ToLower() + "=" + Function.ToString(item.Value).ToLower() + "&";
						}
					}
					text = text.TrimEnd('&');
				}
			}
			else
			{
				text = "-" + string.Join("&", enumerable);
			}
			if (text == "-")
			{
				text = string.Empty;
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	private string PrepareGetParameters(HttpActionContext context, IEnumerable<string> actionParameters)
	{
		IEnumerable<string> second = from x in context.Request.GetQueryNameValuePairs()
			where x.Key.ToLower() != "callback"
			select x.Key + "=" + x.Value;
		IEnumerable<string> values = actionParameters.Union(second);
		string text = ":" + string.Join("&", values);
		string jsonpCallback = GetJsonpCallback(context.Request);
		if (!string.IsNullOrWhiteSpace(jsonpCallback))
		{
			string text2 = "callback=" + jsonpCallback;
			if (text.Contains("&" + text2))
			{
				text = text.Replace("&" + text2, string.Empty);
			}
			if (text.Contains(text2 + "&"))
			{
				text = text.Replace(text2 + "&", string.Empty);
			}
			if (text.Contains("-" + text2))
			{
				text = text.Replace("-" + text2, string.Empty);
			}
			if (text.EndsWith("&"))
			{
				text = text.TrimEnd('&');
			}
		}
		return text;
	}

	private string GetJsonpCallback(HttpRequestMessage request)
	{
		string result = string.Empty;
		if (request.Method == HttpMethod.Get)
		{
			IEnumerable<KeyValuePair<string, string>> queryNameValuePairs = request.GetQueryNameValuePairs();
			if (queryNameValuePairs != null)
			{
				KeyValuePair<string, string> keyValuePair = queryNameValuePairs.FirstOrDefault((KeyValuePair<string, string> x) => x.Key.ToLower() == "callback");
				if (!keyValuePair.Equals(default(KeyValuePair<string, string>)))
				{
					result = keyValuePair.Value;
				}
			}
		}
		return result;
	}

	private string GetValue(object val)
	{
		if (val is IEnumerable && !(val is string))
		{
			string empty = string.Empty;
			return (val as IEnumerable).Cast<object>().Aggregate(empty, (string current, object paramValue) => current + paramValue?.ToString() + ";");
		}
		return val.ToString();
	}
}
