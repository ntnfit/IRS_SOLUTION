using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Apzon.Api.Broker;
using Apzon.Api.Models;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Administrator;
using Apzon.Services;

namespace Apzon.Api.Controllers.Sales;

public class BaseApiController : ApiController
{
	protected IUnitOfWork UnitOfWork;

	protected SessionUserInfoImpl SessionUserInfo { get; set; }

	protected string SessionId { get; set; }

	protected override void Initialize(HttpControllerContext controllerContext)
	{
		try
		{
			SessionUserInfo = new SessionUserInfoImpl();
			if (controllerContext.Request.Headers.Contains("SessionId"))
			{
				SessionId = Function.ToString(controllerContext.Request.Headers.GetValues("SessionId").FirstOrDefault());
			}
			else if (controllerContext.Request.Headers.Contains("AuthorizationToken"))
			{
				string text = Function.ToString(controllerContext.Request.Headers.GetValues("AuthorizationToken").FirstOrDefault());
				if (!string.IsNullOrEmpty(text))
				{
					if (Global.TokenUserInfo == null)
					{
						Global.TokenUserInfo = new Dictionary<string, TokenAuthorizationImpl>();
					}
					lock (Global.TokenUserInfo)
					{
						if (!Global.TokenUserInfo.ContainsKey(text) || !Global.SessionUserInfo.ContainsKey(Global.TokenUserInfo[text].SessionId))
						{
							DataTable dataAuthorizationToken = UnitOfWorkFactory.GetUnitOfWork(Global.CommonConnectionString, Global.DatabaseSystemType).Commons.GetDataAuthorizationToken(text);
							if (dataAuthorizationToken.IsNotNull() && dataAuthorizationToken.Rows.Count == 1)
							{
								DateTime? dateTime = Function.ParseDateTime(dataAuthorizationToken.Rows[0]["ExpiredDate"]);
								DateTime expiredDate = ((!dateTime.HasValue) ? DateTime.MaxValue : dateTime.Value);
								LoginResult loginResult = BrokerService.Login(new LoginImpl
								{
									Doimain = Function.Encrypt(RSA.PublicKey, Function.ToString(dataAuthorizationToken.Rows[0]["DomainName"])),
									UserName = Function.Encrypt(RSA.PublicKey, Function.ToString(dataAuthorizationToken.Rows[0]["UserName"])),
									Password = Function.ToString(dataAuthorizationToken.Rows[0]["Password"]),
									DbName = Function.Encrypt(RSA.PublicKey, Function.ToString(dataAuthorizationToken.Rows[0]["DatabaseName"])),
									ClientVersion = Function.Encrypt(RSA.PublicKey, Function.ConvertDoubleToStringEnUs(Versions.ApplicationVersion, 2, 1)),
									BuildVersion = Function.Encrypt(RSA.PublicKey, Function.ConvertDoubleToStringEnUs(Versions.BuildVersion, 0, 3))
								}, "", "");
								string sessionId = "";
								if (!string.IsNullOrEmpty(loginResult.Message) && Function.Decrypt(RSA.PrivateKey, loginResult.Message).Equals("success"))
								{
									sessionId = Function.Decrypt(RSA.PrivateKey, loginResult.Content.ToString()).Split(',')[0];
								}
								if (Global.TokenUserInfo.ContainsKey(text))
								{
									Global.TokenUserInfo.Remove(text);
								}
								Global.TokenUserInfo.Add(text, new TokenAuthorizationImpl
								{
									ExpiredDate = expiredDate,
									SessionId = sessionId
								});
							}
							else
							{
								Logging.Write("TRACE", $"Token {text} is invalid");
							}
						}
					}
					if (Global.TokenUserInfo.ContainsKey(text) && Global.TokenUserInfo[text].ExpiredDate >= DateTime.Now.Date)
					{
						SessionId = Global.TokenUserInfo[text].SessionId;
					}
				}
			}
			if (!string.IsNullOrEmpty(SessionId))
			{
				if (Global.SessionUserInfo.ContainsKey(SessionId))
				{
					SessionUserInfo = Global.SessionUserInfo[SessionId];
				}
				IUnitOfWork apzUnitOfWork = BrokerService.GetApzUnitOfWork(SessionId);
				if (apzUnitOfWork != null)
				{
					UnitOfWork = apzUnitOfWork;
					base.Initialize(controllerContext);
					return;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		base.Initialize(controllerContext);
	}

	public string GetImage100Base64(string url)
	{
		try
		{
			if (string.IsNullOrEmpty(url))
			{
				return "";
			}
			string stringSha256Hash = Function.GetStringSha256Hash(url);
			if (string.IsNullOrEmpty(stringSha256Hash))
			{
				return "";
			}
			if (Global.CacheImage100 != null && Global.CacheImage100.ContainsKey(stringSha256Hash))
			{
				return Global.CacheImage100[stringSha256Hash];
			}
			string text;
			using (WebResponse webResponse = WebRequest.Create(url).GetResponse())
			{
				using Stream stream = webResponse.GetResponseStream();
				Image image = Image.FromStream(stream);
				Bitmap bitmap = Function.ResizeBitmap(image, 100, image.Height * 100 / image.Width);
				using MemoryStream memoryStream = new MemoryStream();
				bitmap.Save(memoryStream, ImageFormat.Bmp);
				text = Convert.ToBase64String(memoryStream.ToArray());
			}
			if (!string.IsNullOrEmpty(text))
			{
				if (Global.CacheImage100 != null)
				{
					Global.CacheImage100[stringSha256Hash] = text;
				}
				return text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return "";
	}

	public async Task<string> GetImage100Base64Async(string url)
	{
		try
		{
			if (string.IsNullOrEmpty(url))
			{
				return "";
			}
			string hash = Function.GetStringSha256Hash(url);
			if (string.IsNullOrEmpty(hash))
			{
				return "";
			}
			if (Global.CacheImage100 != null && Global.CacheImage100.ContainsKey(hash))
			{
				return Global.CacheImage100[hash];
			}
			string text;
			using (WebResponse webResponse = await WebRequest.Create(url).GetResponseAsync())
			{
				using Stream stream = webResponse.GetResponseStream();
				Image image = Image.FromStream(stream);
				Bitmap bitmap = Function.ResizeBitmap(image, 100, image.Height * 100 / image.Width);
				using MemoryStream memoryStream = new MemoryStream();
				bitmap.Save(memoryStream, ImageFormat.Bmp);
				text = Convert.ToBase64String(memoryStream.ToArray());
			}
			if (!string.IsNullOrEmpty(text))
			{
				if (Global.CacheImage100 != null)
				{
					Global.CacheImage100[hash] = text;
				}
				return text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return "";
	}
}
