using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.ApzJson;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Global;
using Newtonsoft.Json;

namespace Apzon.HttpClientHelper;

public class HttpClientHelper<TEntity> : IDisposable, IHttpClientHelper<TEntity> where TEntity : class
{
	public HttpClient HttpClient { get; set; }

	public string ApiServer { get; set; }

	public string Uri { get; set; }

	public string Controller { get; set; }

	public string SessionType { get; set; }

	public HttpClientHelper(string apiServer, string controler, int requestTimeout = 30)
	{
		if (string.IsNullOrEmpty(apiServer))
		{
			throw new ArgumentNullException(apiServer);
		}
		if (string.IsNullOrEmpty(controler))
		{
			throw new ArgumentNullException(controler);
		}
		ApiServer = apiServer;
		Controller = controler;
		HttpClient = new HttpClient();
		HttpClient.Timeout = new TimeSpan(0, requestTimeout, 0);
		HttpClient.DefaultRequestHeaders.Add("SessionId", GlobalData.SessionId);
	}

	public HttpClientHelper(string apiServer, string controler, string sessionId, int requestTimeout = 30)
	{
		if (string.IsNullOrEmpty(apiServer))
		{
			throw new ArgumentNullException(apiServer);
		}
		if (string.IsNullOrEmpty(controler))
		{
			throw new ArgumentNullException(controler);
		}
		ApiServer = apiServer;
		Controller = controler;
		ServicePointManager.Expect100Continue = true;
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, (RemoteCertificateValidationCallback)((object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true));
		HttpClient = new HttpClient();
		HttpClient.Timeout = new TimeSpan(0, requestTimeout, 0);
		HttpClient.DefaultRequestHeaders.Add("SessionId", sessionId);
		SessionType = "W";
	}

	public HttpClientHelper(string apiServer, int requestTimeout = 30)
	{
		if (string.IsNullOrEmpty(apiServer))
		{
			throw new ArgumentNullException(apiServer);
		}
		ApiServer = apiServer;
		ServicePointManager.Expect100Continue = true;
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, (RemoteCertificateValidationCallback)((object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true));
		HttpClient = new HttpClient();
		HttpClient.Timeout = new TimeSpan(0, requestTimeout, 0);
		HttpClient.DefaultRequestHeaders.Add("SessionId", GlobalData.SessionId);
	}

	public void Dispose()
	{
		HttpClient.Dispose();
	}

	public async Task<TEntity> GetAsync(string action, object pr)
	{
		_ = 2;
		try
		{
			UpdateSessionId();
			using HttpResponseMessage response = await HttpClient.GetAsync(GetUri(action, pr)).ConfigureAwait(continueOnCapturedContext: false);
			if (response.IsSuccessStatusCode)
			{
				string text = await GetResponseString(response.Content);
				return (TEntity)((!text.Contains("Apz-Columns")) ? JsonConvert.DeserializeObject(text, typeof(TEntity)) : JsonConvert.DeserializeObject(text, typeof(TEntity), new ApzonJsonClientEntityConverter<TEntity>()));
			}
			return (dynamic)(await HandleHttpResponse(response));
		}
		catch (Exception)
		{
			throw;
		}
	}

	public async Task<TEntity> GetJsonToTableAsync(string action)
	{
		_ = 2;
		try
		{
			UpdateSessionId();
			using HttpResponseMessage response = await HttpClient.GetAsync(GetUri(action)).ConfigureAwait(continueOnCapturedContext: false);
			if (response.IsSuccessStatusCode)
			{
				return (TEntity)JsonConvert.DeserializeObject(await GetResponseString(response.Content), typeof(TEntity), new ApzonJsonClientJsonToDataTableApzConverter<TEntity>());
			}
			return (dynamic)(await HandleHttpResponse(response));
		}
		catch (Exception)
		{
			throw;
		}
	}

	public async Task<TEntity> GetJsonToTableAsync(string action, object param)
	{
		_ = 2;
		try
		{
			UpdateSessionId();
			using HttpResponseMessage response = await HttpClient.GetAsync(GetUri(action, param)).ConfigureAwait(continueOnCapturedContext: false);
			if (response.IsSuccessStatusCode)
			{
				return (TEntity)JsonConvert.DeserializeObject(await GetResponseString(response.Content), typeof(TEntity), new ApzonJsonClientJsonToDataTableApzConverter<TEntity>());
			}
			return (dynamic)(await HandleHttpResponse(response));
		}
		catch (Exception)
		{
			throw;
		}
	}

	public async Task<TEntity> GetAsync(string action)
	{
		UpdateSessionId();
		using HttpResponseMessage response = await HttpClient.GetAsync(GetUri(action));
		if (response.IsSuccessStatusCode)
		{
			string text = await GetResponseString(response.Content);
			return (!text.Contains("Apz-Columns")) ? JsonConvert.DeserializeObject<TEntity>(text) : JsonConvert.DeserializeObject<TEntity>(text, new JsonConverter[1]
			{
				new ApzonJsonClientEntityConverter<TEntity>()
			});
		}
		return (dynamic)(await HandleHttpResponse(response));
	}

	public async Task<List<TEntity>> GetListAsync(string action)
	{
		UpdateSessionId();
		using HttpResponseMessage response = await HttpClient.GetAsync(GetUri(action));
		if (response.IsSuccessStatusCode)
		{
			string text = await GetResponseString(response.Content);
			return (!text.Contains("Apz-Columns")) ? JsonConvert.DeserializeObject<List<TEntity>>(text) : JsonConvert.DeserializeObject<List<TEntity>>(text, new JsonConverter[1]
			{
				new ApzonJsonClientListEntityConverter<TEntity>()
			});
		}
		return (dynamic)(await HandleHttpResponse(response));
	}

	public async Task<List<TEntity>> GetListAsyncByPr(string action, object pr)
	{
		UpdateSessionId();
		using HttpResponseMessage response = await HttpClient.GetAsync(GetUri(action, pr));
		if (response.IsSuccessStatusCode)
		{
			string text = await GetResponseString(response.Content);
			return (!text.Contains("Apz-Columns")) ? JsonConvert.DeserializeObject<List<TEntity>>(text) : JsonConvert.DeserializeObject<List<TEntity>>(text, new JsonConverter[1]
			{
				new ApzonJsonClientListEntityConverter<TEntity>()
			});
		}
		return (dynamic)(await HandleHttpResponse(response));
	}

	public async Task<string> GetStringAsync(string action, object pr)
	{
		UpdateSessionId();
		using HttpResponseMessage response = await HttpClient.GetAsync(GetUri(action, pr));
		if (response.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<string>(await GetResponseString(response.Content));
		}
		return response.ReasonPhrase;
	}

	public async Task<string> GetStringAsync(string action)
	{
		UpdateSessionId();
		using HttpResponseMessage response = await HttpClient.GetAsync(GetUri(action));
		if (response.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<string>(await GetResponseString(response.Content));
		}
		return response.ReasonPhrase;
	}

	public async Task<HttpResult> GetHttpResult(string action, object pr)
	{
		UpdateSessionId();
		using HttpResponseMessage response = await HttpClient.GetAsync(GetUri(action, pr));
		if (response.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<HttpResult>(await GetResponseString(response.Content));
		}
		return (dynamic)(await HandleHttpResponse(response));
	}

	public async Task<HttpResult> GetHttpResultDataSet(string action, object pr)
	{
		UpdateSessionId();
		using HttpResponseMessage response = await HttpClient.GetAsync(GetUri(action, pr));
		if (response.IsSuccessStatusCode)
		{
			string text = await GetResponseString(response.Content);
			if (text.Contains("Message"))
			{
				return JsonConvert.DeserializeObject<HttpResult>(text);
			}
			if (text.Contains("Apz-Columns"))
			{
				return new HttpResult(Enums.MessageCode.Success, "", JsonConvert.DeserializeObject<DataSet>(text, new JsonConverter[1]
				{
					new ApzJsonConverterDataTable()
				}));
			}
			new HttpResult(Enums.MessageCode.Success, "", JsonConvert.DeserializeObject<DataSet>(text));
		}
		return (dynamic)(await HandleHttpResponse(response));
	}

	public string GetUri(string action, object pr)
	{
		return Function.GetApiUri(ApiServer, Controller, action, pr);
	}

	public string GetUri(string action)
	{
		return Function.GetApiUri(ApiServer, Controller, action);
	}

	public string GetUri(string action, Type type, ref bool isParseData)
	{
		isParseData = type == typeof(DataTable) || type == typeof(DataSet) || type.Name.Substring(0, 5).ToLower().Equals("tuple");
		return Function.GetApiUri(ApiServer, Controller, action);
	}

	public void HandleHttpResponseMessage(HttpResponseMessage result)
	{
		if (result != null)
		{
			throw new Exception(result.StatusCode.ToString() + "-" + result.RequestMessage);
		}
	}

	public async Task<string> PostAsJsonAsync(string action, TEntity entity)
	{
		UpdateSessionId();
		bool isParseData = false;
		string uri = GetUri(action, typeof(TEntity), ref isParseData);
		HttpResponseMessage httpResponseMessage = ((!isParseData) ? (await HttpClient.PostAsJsonAsync(uri, entity)) : (await HttpClient.PostAsync(uri, entity, new ApzJsonFormater())));
		using HttpResponseMessage response = httpResponseMessage;
		if (response.IsSuccessStatusCode)
		{
			string result = JsonConvert.DeserializeObject<string>(await GetResponseString(response.Content));
			if ((!string.IsNullOrEmpty(result) && response.ReasonPhrase.Equals("INVALIDSESSION")) || Function.ToString(response.ReasonPhrase).ToUpper().Equals("INVALID SESSION ID"))
			{
				await Relogin();
			}
			return result;
		}
		return (dynamic)(await HandleHttpResponse(response));
	}

	public async Task<string> PostAsJsonAsync(string action, List<TEntity> entities)
	{
		UpdateSessionId();
		using HttpResponseMessage response = await HttpClient.PostAsJsonAsync(GetUri(action), entities);
		if (response.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<string>(await GetResponseString(response.Content));
		}
		return (dynamic)(await HandleHttpResponse(response));
	}

	public async Task<string> PostParamsAsJsonAsync(string action, object pr)
	{
		UpdateSessionId();
		bool isParseData = false;
		string uri = GetUri(action, pr.GetType(), ref isParseData);
		HttpResponseMessage httpResponseMessage3;
		if (isParseData)
		{
			HttpResponseMessage httpResponseMessage;
			if (pr.GetType() == typeof(DataTable))
			{
				httpResponseMessage = await HttpClient.PostAsync(uri, (DataTable)pr, new ApzJsonFormater());
			}
			else
			{
				HttpResponseMessage httpResponseMessage2 = ((!(pr.GetType() == typeof(DataSet))) ? (await HttpClient.PostAsync(uri, pr, new ApzJsonFormater())) : (await HttpClient.PostAsync(uri, (DataSet)pr, new ApzJsonFormater())));
				httpResponseMessage = httpResponseMessage2;
			}
			httpResponseMessage3 = httpResponseMessage;
		}
		else
		{
			httpResponseMessage3 = await HttpClient.PostAsJsonAsync(GetUri(action), pr);
		}
		using HttpResponseMessage response = httpResponseMessage3;
		if (response.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<string>(await GetResponseString(response.Content));
		}
		return (dynamic)(await HandleHttpResponse(response));
	}

	public async Task<DataTable> GetDataTableAsJsonAsync(string action, object obj)
	{
		UpdateSessionId();
		using HttpResponseMessage response = await HttpClient.PostAsJsonAsync(GetUri(action), obj);
		if (response.IsSuccessStatusCode)
		{
			string text = await GetResponseString(response.Content);
			return text.Contains("Apz-Columns") ? JsonConvert.DeserializeObject<DataTable>(text, new JsonConverter[1]
			{
				new ApzJsonConverterDataTable()
			}) : JsonConvert.DeserializeObject<DataTable>(text);
		}
		return (dynamic)(await HandleHttpResponse(response));
	}

	public async Task<int> GetIntAsync(string action, object pr)
	{
		UpdateSessionId();
		using HttpResponseMessage response = await HttpClient.GetAsync(GetUri(action, pr));
		if (response.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<int>(await GetResponseString(response.Content));
		}
		return (dynamic)(await HandleHttpResponse(response));
	}

	public async Task<bool> GetBoolAsync(string action, object pr)
	{
		UpdateSessionId();
		using HttpResponseMessage response = await HttpClient.GetAsync(GetUri(action, pr));
		if (response.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<bool>(await GetResponseString(response.Content));
		}
		return (dynamic)(await HandleHttpResponse(response));
	}

	public async Task<int> GetIntAsync(string action)
	{
		UpdateSessionId();
		using HttpResponseMessage response = await HttpClient.GetAsync(GetUri(action));
		if (response.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<int>(await GetResponseString(response.Content));
		}
		return (dynamic)(await HandleHttpResponse(response));
	}

	public async Task<DataTable> GetDataTableAsync(string action, object pr)
	{
		UpdateSessionId();
		using HttpResponseMessage response = await HttpClient.GetAsync(GetUri(action, pr));
		if (response.IsSuccessStatusCode)
		{
			string text = await GetResponseString(response.Content);
			return text.Contains("Apz-Columns") ? JsonConvert.DeserializeObject<DataTable>(text, new JsonConverter[1]
			{
				new ApzJsonConverterDataTable()
			}) : JsonConvert.DeserializeObject<DataTable>(text);
		}
		return (dynamic)(await HandleHttpResponse(response));
	}

	public async Task<DataTable> GetDataTableAsync(string action)
	{
		UpdateSessionId();
		using HttpResponseMessage response = await HttpClient.GetAsync(GetUri(action));
		if (response.IsSuccessStatusCode)
		{
			string text = await GetResponseString(response.Content);
			return text.Contains("Apz-Columns") ? JsonConvert.DeserializeObject<DataTable>(text, new JsonConverter[1]
			{
				new ApzJsonConverterDataTable()
			}) : JsonConvert.DeserializeObject<DataTable>(text);
		}
		return (dynamic)(await HandleHttpResponse(response));
	}

	public async Task<List<ExpandoObject>> PostListDynamic(string action, TEntity obj)
	{
		UpdateSessionId();
		bool isParseData = false;
		string uri = GetUri(action, obj.GetType(), ref isParseData);
		HttpResponseMessage httpResponseMessage = ((!isParseData) ? (await HttpClient.PostAsJsonAsync(uri, obj)) : (await HttpClient.PostAsync(uri, obj, new ApzJsonFormater())));
		using HttpResponseMessage response = httpResponseMessage;
		if (response.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<List<ExpandoObject>>(await GetResponseString(response.Content));
		}
		return (dynamic)(await HandleHttpResponse(response));
	}

	public async Task<TEntity> PostAsync(string action, object pr, string sessionId = "")
	{
		_ = 5;
		try
		{
			UpdateSessionId(sessionId);
			bool isParseData = false;
			string uri = GetUri(action, pr.GetType(), ref isParseData);
			HttpResponseMessage httpResponseMessage3;
			if (isParseData)
			{
				HttpResponseMessage httpResponseMessage;
				if (pr.GetType() == typeof(DataTable))
				{
					httpResponseMessage = await HttpClient.PostAsync(uri, (DataTable)pr, new ApzJsonFormater());
				}
				else
				{
					HttpResponseMessage httpResponseMessage2 = ((!(pr.GetType() == typeof(DataSet))) ? (await HttpClient.PostAsync(uri, pr, new ApzJsonFormater())) : (await HttpClient.PostAsync(uri, (DataSet)pr, new ApzJsonFormater())));
					httpResponseMessage = httpResponseMessage2;
				}
				httpResponseMessage3 = httpResponseMessage;
			}
			else
			{
				httpResponseMessage3 = await HttpClient.PostAsJsonAsync(GetUri(action), pr);
			}
			using HttpResponseMessage response = httpResponseMessage3;
			if (response.IsSuccessStatusCode)
			{
				string text = await GetResponseString(response.Content);
				return (!text.Contains("Apz-Columns")) ? JsonConvert.DeserializeObject<TEntity>(text) : JsonConvert.DeserializeObject<TEntity>(text, new JsonConverter[1]
				{
					new ApzonJsonClientEntityConverter<TEntity>()
				});
			}
			return (dynamic)(await HandleHttpResponse(response));
		}
		catch (Exception)
		{
			throw;
		}
	}

	public async Task<TEntity> PostJsonToTableAsync(string action, object pr)
	{
		_ = 5;
		try
		{
			UpdateSessionId();
			bool isParseData = false;
			string uri = GetUri(action, pr.GetType(), ref isParseData);
			HttpResponseMessage httpResponseMessage3;
			if (isParseData)
			{
				HttpResponseMessage httpResponseMessage;
				if (pr.GetType() == typeof(DataTable))
				{
					httpResponseMessage = await HttpClient.PostAsync(uri, (DataTable)pr, new ApzJsonFormater());
				}
				else
				{
					HttpResponseMessage httpResponseMessage2 = ((!(pr.GetType() == typeof(DataSet))) ? (await HttpClient.PostAsync(uri, pr, new ApzJsonFormater())) : (await HttpClient.PostAsync(uri, (DataSet)pr, new ApzJsonFormater())));
					httpResponseMessage = httpResponseMessage2;
				}
				httpResponseMessage3 = httpResponseMessage;
			}
			else
			{
				httpResponseMessage3 = await HttpClient.PostAsJsonAsync(GetUri(action), pr);
			}
			using HttpResponseMessage response = httpResponseMessage3;
			if (response.IsSuccessStatusCode)
			{
				return JsonConvert.DeserializeObject<TEntity>(await GetResponseString(response.Content), new JsonConverter[1]
				{
					new ApzonJsonClientJsonToDataTableApzConverter<TEntity>()
				});
			}
			return (dynamic)(await HandleHttpResponse(response));
		}
		catch (Exception)
		{
			throw;
		}
	}

	public async Task<HttpResult> GetHttpResult(string action)
	{
		UpdateSessionId();
		using HttpResponseMessage response = await HttpClient.GetAsync(GetUri(action));
		if (response.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<HttpResult>(await GetResponseString(response.Content));
		}
		return (dynamic)(await HandleHttpResponse(response));
	}

	public async Task<TEntity> PostAsJsonAsyncTable(string action, TEntity pr)
	{
		UpdateSessionId();
		bool isParseData = false;
		string uri = GetUri(action, typeof(TEntity), ref isParseData);
		HttpResponseMessage httpResponseMessage = ((!isParseData) ? (await HttpClient.PostAsJsonAsync(GetUri(action), pr)) : (await HttpClient.PostAsync(uri, pr, new ApzJsonFormater())));
		using HttpResponseMessage response = httpResponseMessage;
		if (response.IsSuccessStatusCode)
		{
			string text = await GetResponseString(response.Content);
			return (!text.Contains("Apz-Columns")) ? JsonConvert.DeserializeObject<TEntity>(text) : JsonConvert.DeserializeObject<TEntity>(text, new JsonConverter[1]
			{
				new ApzonJsonClientEntityConverter<TEntity>()
			});
		}
		return (dynamic)(await HandleHttpResponse(response));
	}

	public async Task<List<TEntity>> GetListAsync(string action, object pr)
	{
		UpdateSessionId();
		using HttpResponseMessage response = await HttpClient.GetAsync(GetUri(action, pr));
		if (response.IsSuccessStatusCode)
		{
			string text = await GetResponseString(response.Content);
			return (!text.Contains("Apz-Columns")) ? JsonConvert.DeserializeObject<List<TEntity>>(text) : JsonConvert.DeserializeObject<List<TEntity>>(text, new JsonConverter[1]
			{
				new ApzonJsonClientListEntityConverter<TEntity>()
			});
		}
		return (dynamic)(await HandleHttpResponse(response));
	}

	public async Task<List<TEntity>> PostListParameter(string action, object pr)
	{
		UpdateSessionId();
		bool isParseData = false;
		string uri = GetUri(action, pr.GetType(), ref isParseData);
		HttpResponseMessage httpResponseMessage3;
		if (isParseData)
		{
			HttpResponseMessage httpResponseMessage;
			if (pr.GetType() == typeof(DataTable))
			{
				httpResponseMessage = await HttpClient.PostAsync(uri, (DataTable)pr, new ApzJsonFormater());
			}
			else
			{
				HttpResponseMessage httpResponseMessage2 = ((!(pr.GetType() == typeof(DataSet))) ? (await HttpClient.PostAsync(uri, pr, new ApzJsonFormater())) : (await HttpClient.PostAsync(uri, (DataSet)pr, new ApzJsonFormater())));
				httpResponseMessage = httpResponseMessage2;
			}
			httpResponseMessage3 = httpResponseMessage;
		}
		else
		{
			httpResponseMessage3 = await HttpClient.PostAsJsonAsync(GetUri(action), pr);
		}
		using HttpResponseMessage response = httpResponseMessage3;
		if (response.IsSuccessStatusCode)
		{
			string text = await GetResponseString(response.Content);
			return (!text.Contains("Apz-Columns")) ? JsonConvert.DeserializeObject<List<TEntity>>(text) : JsonConvert.DeserializeObject<List<TEntity>>(text, new JsonConverter[1]
			{
				new ApzonJsonClientListEntityConverter<TEntity>()
			});
		}
		return (dynamic)(await HandleHttpResponse(response));
	}

	public async Task<string> PostMultiPartAsync(string action, object obj)
	{
		UpdateSessionId();
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
		string uri = GetUri(action, obj);
		httpRequestMessage.Method = HttpMethod.Post;
		httpRequestMessage.RequestUri = new Uri(uri);
		using HttpResponseMessage response = await HttpClient.SendAsync(httpRequestMessage);
		if (response.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<string>(await GetResponseString(response.Content));
		}
		return (dynamic)(await HandleHttpResponse(response));
	}

	private async Task<dynamic> HandleHttpResponse(HttpResponseMessage response)
	{
		if (response.StatusCode == HttpStatusCode.InternalServerError && response.ReasonPhrase.Equals("This function is not supported"))
		{
			throw new Exception(response.ReasonPhrase);
		}
		if ((response.StatusCode == HttpStatusCode.InternalServerError && response.ReasonPhrase.Equals("INVALIDSESSION")) || Function.ToString(response.ReasonPhrase).ToUpper().Equals("INVALID SESSION ID"))
		{
			await Relogin();
		}
		if (response.IsSuccessStatusCode)
		{
			string text = JsonConvert.DeserializeObject<string>(await GetResponseString(response.Content));
			if ((string.IsNullOrEmpty(text) || !Function.ToString(response.ReasonPhrase).ToLower().Equals("INVALIDSESSION".ToLower())) && !Function.ToString(response.ReasonPhrase).ToUpper().Equals("INVALID SESSION ID"))
			{
				return text;
			}
			await Relogin();
		}
		throw new Exception(response.StatusCode.ToString());
	}

	private async Task Relogin()
	{
		if (GlobalData.IsRelogin || GlobalData.UserName.ToLower().Equals(GlobalData.SuperUser))
		{
			return;
		}
		Logging.Write("TRACE", "Invalid session. Re-logining");
		GlobalData.IsRelogin = true;
		LoginImpl pr = new LoginImpl
		{
			Doimain = Function.Encrypt(RSA.PublicKey, GlobalData.Domain),
			UserName = Function.Encrypt(RSA.PublicKey, GlobalData.UserName),
			Password = Function.Encrypt(RSA.PublicKey, EncodePassword.DecryptString(GlobalData.OldPassword, RSA.PasswordPhrase)),
			DbName = Function.Encrypt(RSA.PublicKey, GlobalData.DatabaseName),
			ClientVersion = Function.Encrypt(RSA.PublicKey, Function.ConvertDoubleToStringEnUs(Versions.ApplicationVersion, 2, 1)),
			BuildVersion = Function.Encrypt(RSA.PublicKey, Function.ConvertDoubleToStringEnUs(Versions.BuildVersion, 0, 3))
		};
		using (HttpClientHelper<LoginResult> http = new HttpClientHelper<LoginResult>(Configuration.ApiServer, "LoginIRS"))
		{
			LoginResult loginResult = await http.PostAsync("Login", pr);
			if (!string.IsNullOrEmpty(loginResult.Message))
			{
				if (!Function.Decrypt(RSA.PrivateKey, loginResult.Message).Equals("success"))
				{
					throw new Exception(Function.Decrypt(RSA.PrivateKey, loginResult.Message));
				}
				string[] array = Function.Decrypt(RSA.PrivateKey, loginResult.Content.ToString()).Split(',');
				string text = array[0];
				GlobalData.UserLicense = loginResult.LicenseInfo;
				GlobalData.LoginSap = array[2].Equals("1");
				GlobalData.SessionId = Function.ToString(text);
			}
		}
		GlobalData.IsRelogin = false;
		throw new Exception("INVALIDSESSION");
	}

	public async Task<DataTable> PostDataTableAsync(string action, object pr)
	{
		UpdateSessionId();
		bool isParseData = false;
		string uri = GetUri(action, pr.GetType(), ref isParseData);
		HttpResponseMessage httpResponseMessage3;
		if (isParseData)
		{
			HttpResponseMessage httpResponseMessage;
			if (pr.GetType() == typeof(DataTable))
			{
				httpResponseMessage = await HttpClient.PostAsync(uri, (DataTable)pr, new ApzJsonFormater());
			}
			else
			{
				HttpResponseMessage httpResponseMessage2 = ((!(pr.GetType() == typeof(DataSet))) ? (await HttpClient.PostAsync(uri, pr, new ApzJsonFormater())) : (await HttpClient.PostAsync(uri, (DataSet)pr, new ApzJsonFormater())));
				httpResponseMessage = httpResponseMessage2;
			}
			httpResponseMessage3 = httpResponseMessage;
		}
		else
		{
			httpResponseMessage3 = await HttpClient.PostAsJsonAsync(GetUri(action), pr);
		}
		using HttpResponseMessage response = httpResponseMessage3;
		if (response.IsSuccessStatusCode)
		{
			string text = await GetResponseString(response.Content);
			return text.Contains("Apz-Columns") ? JsonConvert.DeserializeObject<DataTable>(text, new JsonConverter[1]
			{
				new ApzJsonConverterDataTable()
			}) : JsonConvert.DeserializeObject<DataTable>(text);
		}
		return (dynamic)(await HandleHttpResponse(response));
	}

	public async Task<TEntity> PutAsync(string action, object pr)
	{
		_ = 2;
		try
		{
			UpdateSessionId();
			bool isParseData = false;
			string uri = GetUri(action, pr.GetType(), ref isParseData);
			using HttpResponseMessage response = await HttpClient.PutAsJsonAsync(uri, pr);
			if (response.IsSuccessStatusCode)
			{
				string text = await GetResponseString(response.Content);
				return (!text.Contains("Apz-Columns")) ? JsonConvert.DeserializeObject<TEntity>(text) : JsonConvert.DeserializeObject<TEntity>(text, new JsonConverter[1]
				{
					new ApzonJsonClientEntityConverter<TEntity>()
				});
			}
			return (dynamic)(await HandleHttpResponse(response));
		}
		catch (Exception)
		{
			throw;
		}
	}

	public void UpdateSessionId(string id = "")
	{
		if (!string.IsNullOrEmpty(SessionType))
		{
			return;
		}
		KeyValuePair<string, IEnumerable<string>> keyValuePair = HttpClient.DefaultRequestHeaders.ToList().FirstOrDefault((KeyValuePair<string, IEnumerable<string>> t) => t.Key.Equals("SessionId"));
		if (keyValuePair.Value.ToList().Count > 0)
		{
			if (!string.IsNullOrEmpty(id))
			{
				HttpClient.DefaultRequestHeaders.Remove("SessionId");
				HttpClient.DefaultRequestHeaders.Add("SessionId", id);
				return;
			}
			string text = keyValuePair.Value.ToList()[0];
			if (string.IsNullOrEmpty(text) || !text.Equals(GlobalData.SessionId))
			{
				HttpClient.DefaultRequestHeaders.Remove("SessionId");
				HttpClient.DefaultRequestHeaders.Add("SessionId", GlobalData.SessionId);
			}
		}
		else if (!string.IsNullOrEmpty(id))
		{
			HttpClient.DefaultRequestHeaders.Add("SessionId", id);
		}
		else
		{
			HttpClient.DefaultRequestHeaders.Add("SessionId", GlobalData.SessionId);
		}
	}

	public async Task<string> GetResponseString(HttpContent content)
	{
		_ = 2;
		try
		{
			if (content.Headers.ContentEncoding.ToList().Exists((string t) => t.Equals("deflate")))
			{
				return CompressionHelper.UndeflateByte(await content.ReadAsByteArrayAsync()).ConvertByteToString();
			}
			if (content.Headers.ContentEncoding.ToList().Exists((string t) => t.Equals("deflate-Gzip")))
			{
				return JsoncCompressionHelper.Decompress(await content.ReadAsByteArrayAsync()).ConvertByteToString();
			}
			return await content.ReadAsStringAsync();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public HttpResult GetResult(string action, object pr)
	{
		UpdateSessionId();
		using HttpResponseMessage httpResponseMessage = HttpClient.GetAsync(GetUri(action, pr)).Result;
		if (httpResponseMessage.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<HttpResult>(GetResponseString(httpResponseMessage.Content).Result);
		}
		return HandleHttpResponse(httpResponseMessage).Result;
	}

	public TEntity Get(string action, object pr)
	{
		try
		{
			UpdateSessionId();
			using HttpResponseMessage httpResponseMessage = HttpClient.GetAsync(GetUri(action, pr)).Result;
			if (httpResponseMessage.IsSuccessStatusCode)
			{
				string result = GetResponseString(httpResponseMessage.Content).Result;
				return (!result.Contains("Apz-Columns")) ? JsonConvert.DeserializeObject<TEntity>(result) : JsonConvert.DeserializeObject<TEntity>(result, new JsonConverter[1]
				{
					new ApzonJsonClientEntityConverter<TEntity>()
				});
			}
			return HandleHttpResponse(httpResponseMessage).Result;
		}
		catch (Exception)
		{
			throw;
		}
	}

	public TEntity Get(string action)
	{
		UpdateSessionId();
		using HttpResponseMessage httpResponseMessage = HttpClient.GetAsync(GetUri(action)).Result;
		if (httpResponseMessage.IsSuccessStatusCode)
		{
			string result = GetResponseString(httpResponseMessage.Content).Result;
			return (!result.Contains("Apz-Columns")) ? JsonConvert.DeserializeObject<TEntity>(result) : JsonConvert.DeserializeObject<TEntity>(result, new JsonConverter[1]
			{
				new ApzonJsonClientEntityConverter<TEntity>()
			});
		}
		return HandleHttpResponse(httpResponseMessage).Result;
	}

	public List<TEntity> GetListByPr(string action, object pr)
	{
		UpdateSessionId();
		using HttpResponseMessage httpResponseMessage = HttpClient.GetAsync(GetUri(action, pr)).Result;
		if (httpResponseMessage.IsSuccessStatusCode)
		{
			string result = GetResponseString(httpResponseMessage.Content).Result;
			return (!result.Contains("Apz-Columns")) ? JsonConvert.DeserializeObject<List<TEntity>>(result) : JsonConvert.DeserializeObject<List<TEntity>>(result, new JsonConverter[1]
			{
				new ApzonJsonClientListEntityConverter<TEntity>()
			});
		}
		return HandleHttpResponse(httpResponseMessage).Result;
	}

	public List<TEntity> GetList(string action)
	{
		UpdateSessionId();
		using HttpResponseMessage httpResponseMessage = HttpClient.GetAsync(GetUri(action)).Result;
		if (httpResponseMessage.IsSuccessStatusCode)
		{
			string result = GetResponseString(httpResponseMessage.Content).Result;
			return (!result.Contains("Apz-Columns")) ? JsonConvert.DeserializeObject<List<TEntity>>(result) : JsonConvert.DeserializeObject<List<TEntity>>(result, new JsonConverter[1]
			{
				new ApzonJsonClientListEntityConverter<TEntity>()
			});
		}
		return HandleHttpResponse(httpResponseMessage).Result;
	}

	public List<TEntity> GetList(string action, object pr)
	{
		UpdateSessionId();
		using HttpResponseMessage httpResponseMessage = HttpClient.GetAsync(GetUri(action, pr)).Result;
		if (httpResponseMessage.IsSuccessStatusCode)
		{
			string result = GetResponseString(httpResponseMessage.Content).Result;
			return (!result.Contains("Apz-Columns")) ? JsonConvert.DeserializeObject<List<TEntity>>(result) : JsonConvert.DeserializeObject<List<TEntity>>(result, new JsonConverter[1]
			{
				new ApzonJsonClientListEntityConverter<TEntity>()
			});
		}
		return HandleHttpResponse(httpResponseMessage).Result;
	}

	public string GetString(string action, object pr)
	{
		UpdateSessionId();
		using HttpResponseMessage httpResponseMessage = HttpClient.GetAsync(GetUri(action, pr)).Result;
		if (httpResponseMessage.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<string>(GetResponseString(httpResponseMessage.Content).Result);
		}
		return HandleHttpResponse(httpResponseMessage).Result;
	}

	public DataTable GetJsonToTable(string action, object pr)
	{
		UpdateSessionId();
		using HttpResponseMessage httpResponseMessage = HttpClient.GetAsync(GetUri(action, pr)).Result;
		if (httpResponseMessage.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<DataTable>(GetResponseString(httpResponseMessage.Content).Result, new JsonConverter[1]
			{
				new ApzonJsonClientJsonToDataTableApzConverter<DataTable>()
			});
		}
		return HandleHttpResponse(httpResponseMessage).Result;
	}

	public DataTable GetJsonToTable(string action)
	{
		UpdateSessionId();
		using HttpResponseMessage httpResponseMessage = HttpClient.GetAsync(GetUri(action)).Result;
		if (httpResponseMessage.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<DataTable>(GetResponseString(httpResponseMessage.Content).Result, new JsonConverter[1]
			{
				new ApzonJsonClientJsonToDataTableApzConverter<DataTable>()
			});
		}
		return HandleHttpResponse(httpResponseMessage).Result;
	}

	public string GetString(string action)
	{
		UpdateSessionId();
		using HttpResponseMessage httpResponseMessage = HttpClient.GetAsync(GetUri(action)).Result;
		if (httpResponseMessage.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<string>(GetResponseString(httpResponseMessage.Content).Result);
		}
		return HandleHttpResponse(httpResponseMessage).Result;
	}

	public int GetInt(string action, object pr)
	{
		UpdateSessionId();
		using HttpResponseMessage httpResponseMessage = HttpClient.GetAsync(GetUri(action, pr)).Result;
		if (httpResponseMessage.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<int>(GetResponseString(httpResponseMessage.Content).Result);
		}
		return HandleHttpResponse(httpResponseMessage).Result;
	}

	public double GetDouble(string action, object pr)
	{
		UpdateSessionId();
		using HttpResponseMessage httpResponseMessage = HttpClient.GetAsync(GetUri(action, pr)).Result;
		if (httpResponseMessage.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<double>(GetResponseString(httpResponseMessage.Content).Result);
		}
		return HandleHttpResponse(httpResponseMessage).Result;
	}

	public bool GetBool(string action, object pr)
	{
		UpdateSessionId();
		using HttpResponseMessage httpResponseMessage = HttpClient.GetAsync(GetUri(action, pr)).Result;
		if (httpResponseMessage.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<bool>(GetResponseString(httpResponseMessage.Content).Result);
		}
		return HandleHttpResponse(httpResponseMessage).Result;
	}

	public int GetInt(string action)
	{
		UpdateSessionId();
		using HttpResponseMessage httpResponseMessage = HttpClient.GetAsync(GetUri(action)).Result;
		if (httpResponseMessage.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<int>(GetResponseString(httpResponseMessage.Content).Result);
		}
		return HandleHttpResponse(httpResponseMessage).Result;
	}

	public DataTable GetDataTable(string action, object pr)
	{
		UpdateSessionId();
		using HttpResponseMessage httpResponseMessage = HttpClient.GetAsync(GetUri(action, pr)).Result;
		if (httpResponseMessage.IsSuccessStatusCode)
		{
			string result = GetResponseString(httpResponseMessage.Content).Result;
			return result.Contains("Apz-Columns") ? JsonConvert.DeserializeObject<DataTable>(result, new JsonConverter[1]
			{
				new ApzJsonConverterDataTable()
			}) : JsonConvert.DeserializeObject<DataTable>(result);
		}
		return HandleHttpResponse(httpResponseMessage).Result;
	}

	public DataTable GetDataTable(string action)
	{
		UpdateSessionId();
		using HttpResponseMessage httpResponseMessage = HttpClient.GetAsync(GetUri(action)).Result;
		if (httpResponseMessage.IsSuccessStatusCode)
		{
			string result = GetResponseString(httpResponseMessage.Content).Result;
			return result.Contains("Apz-Columns") ? JsonConvert.DeserializeObject<DataTable>(result, new JsonConverter[1]
			{
				new ApzJsonConverterDataTable()
			}) : JsonConvert.DeserializeObject<DataTable>(result);
		}
		return HandleHttpResponse(httpResponseMessage).Result;
	}

	public DataTable GetDataTableAsJson(string action, object obj)
	{
		UpdateSessionId();
		using HttpResponseMessage httpResponseMessage = HttpClient.GetAsync(GetUri(action, obj)).Result;
		if (httpResponseMessage.IsSuccessStatusCode)
		{
			string result = GetResponseString(httpResponseMessage.Content).Result;
			return result.Contains("Apz-Columns") ? JsonConvert.DeserializeObject<DataTable>(result, new JsonConverter[1]
			{
				new ApzJsonConverterDataTable()
			}) : JsonConvert.DeserializeObject<DataTable>(result);
		}
		return HandleHttpResponse(httpResponseMessage).Result;
	}

	public string PostAsJson(string action, TEntity entity)
	{
		UpdateSessionId();
		bool isParseData = false;
		string uri = GetUri(action, typeof(TEntity), ref isParseData);
		using HttpResponseMessage httpResponseMessage = (isParseData ? HttpClient.PostAsync(uri, entity, new ApzJsonFormater()).Result : HttpClient.PostAsJsonAsync(uri, entity).Result);
		if (httpResponseMessage.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<string>(GetResponseString(httpResponseMessage.Content).Result);
		}
		return HandleHttpResponse(httpResponseMessage).Result;
	}

	public string PostParamsAsJson(string action, object pr)
	{
		UpdateSessionId();
		bool isParseData = false;
		string uri = GetUri(action, pr.GetType(), ref isParseData);
		using HttpResponseMessage httpResponseMessage = ((!isParseData) ? HttpClient.PostAsJsonAsync(GetUri(action), pr).Result : ((pr.GetType() == typeof(DataTable)) ? HttpClient.PostAsync(uri, (DataTable)pr, new ApzJsonFormater()).Result : ((pr.GetType() == typeof(DataSet)) ? HttpClient.PostAsync(uri, (DataSet)pr, new ApzJsonFormater()).Result : HttpClient.PostAsync(uri, pr, new ApzJsonFormater()).Result)));
		if (httpResponseMessage.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<string>(GetResponseString(httpResponseMessage.Content).Result);
		}
		return HandleHttpResponse(httpResponseMessage).Result;
	}

	public string PostAsJson(string action, List<TEntity> entities)
	{
		UpdateSessionId();
		using HttpResponseMessage httpResponseMessage = HttpClient.PostAsJsonAsync(GetUri(action), entities).Result;
		if (httpResponseMessage.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<string>(GetResponseString(httpResponseMessage.Content).Result);
		}
		return HandleHttpResponse(httpResponseMessage).Result;
	}

	public TEntity Post(string action, object pr)
	{
		UpdateSessionId();
		bool isParseData = false;
		string uri = GetUri(action, pr.GetType(), ref isParseData);
		using HttpResponseMessage httpResponseMessage = ((!isParseData) ? HttpClient.PostAsJsonAsync(GetUri(action), pr).Result : ((pr.GetType() == typeof(DataTable)) ? HttpClient.PostAsync(uri, (DataTable)pr, new ApzJsonFormater()).Result : ((pr.GetType() == typeof(DataSet)) ? HttpClient.PostAsync(uri, (DataSet)pr, new ApzJsonFormater()).Result : HttpClient.PostAsync(uri, pr, new ApzJsonFormater()).Result)));
		if (httpResponseMessage.IsSuccessStatusCode)
		{
			string result = GetResponseString(httpResponseMessage.Content).Result;
			return (!result.Contains("Apz-Columns")) ? JsonConvert.DeserializeObject<TEntity>(result) : JsonConvert.DeserializeObject<TEntity>(result, new JsonConverter[1]
			{
				new ApzonJsonClientEntityConverter<TEntity>()
			});
		}
		return HandleHttpResponse(httpResponseMessage).Result;
	}

	public TEntity PostJsonToTable(string action, object pr)
	{
		UpdateSessionId();
		bool isParseData = false;
		string uri = GetUri(action, pr.GetType(), ref isParseData);
		using HttpResponseMessage httpResponseMessage = ((!isParseData) ? HttpClient.PostAsJsonAsync(GetUri(action), pr).Result : ((pr.GetType() == typeof(DataTable)) ? HttpClient.PostAsync(uri, (DataTable)pr, new ApzJsonFormater()).Result : ((pr.GetType() == typeof(DataSet)) ? HttpClient.PostAsync(uri, (DataSet)pr, new ApzJsonFormater()).Result : HttpClient.PostAsync(uri, pr, new ApzJsonFormater()).Result)));
		if (httpResponseMessage.IsSuccessStatusCode)
		{
			return JsonConvert.DeserializeObject<TEntity>(GetResponseString(httpResponseMessage.Content).Result, new JsonConverter[1]
			{
				new ApzonJsonClientJsonToDataTableApzConverter<TEntity>()
			});
		}
		return HandleHttpResponse(httpResponseMessage).Result;
	}
}
