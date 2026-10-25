using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Api.Models;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.Api.Controllers.Administrator.Setup.General;

public class LoginIRSController : BaseApiController
{
	private Apzon.Services.IUnitOfWork _unitOfWork;

	[HttpPost]
	public LoginResult Login([FromBody] LoginImpl model)
	{
		return BrokerService.Login(model, SessionId, SessionUserInfo?.SessionDiServer);
	}

	[HttpPost]
	public HttpResult PDALogin([FromBody] LoginImpl model)
	{
		try
		{
			LoginImpl loginImpl = new LoginImpl();
			loginImpl.UserName = Function.Encrypt(RSA.PublicKey, Function.ToString(model.UserName));
			loginImpl.Password = Function.Encrypt(RSA.PublicKey, Function.ToString(model.Password));
			loginImpl.DbName = Function.Encrypt(RSA.PublicKey, Function.ToString(model.DbName));
			loginImpl.ClientVersion = Function.Encrypt(RSA.PublicKey, Function.ConvertDoubleToStringEnUs(Versions.ApplicationVersion, 2, 1));
			loginImpl.BuildVersion = Function.Encrypt(RSA.PublicKey, Function.ConvertDoubleToStringEnUs(Versions.BuildVersion, 0, 3));
			LoginResult loginResult = Login(loginImpl);
			new HttpResult();
			if (Function.Decrypt(RSA.PrivateKey, loginResult.Message).Equals("success"))
			{
				string text = Function.Decrypt(RSA.PrivateKey, Function.ToString(loginResult.Content)).Split(',')[0];
				return new HttpResult
				{
					Message = "",
					Content = Function.ToString(text)
				};
			}
			return new HttpResult
			{
				Message = Function.Decrypt(RSA.PrivateKey, loginResult.Message)
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult
			{
				Message = ex.Message
			};
		}
	}

	[HttpPost]
	public List<string> GetSessionPool([FromBody] Tuple<LoginImpl, int> model)
	{
		try
		{
			string text = Function.Decrypt(RSA.PrivateKey, model.Item1.DbName);
			string text2 = Function.Decrypt(RSA.PrivateKey, model.Item1.UserName).ToLower();
			string text3 = Function.Decrypt(RSA.PrivateKey, model.Item1.Password);
			if (string.IsNullOrEmpty(text))
			{
				throw new Exception("DataBase could not empty");
			}
			if (string.IsNullOrEmpty(text2))
			{
				throw new Exception("the UserName could not empty");
			}
			if (string.IsNullOrEmpty(text3))
			{
				throw new Exception("the Password could not empty");
			}
			if (string.IsNullOrEmpty(model.Item1.ClientVersion))
			{
				throw new Exception("Client version is incorrect");
			}
			DataTable infoCompanyDbServer = Apzon.Services.UnitOfWorkFactory.GetUnitOfWork(Global.CommonConnectionString, Global.DatabaseSystemType).Commons.GetInfoCompanyDbServer(text);
			if (infoCompanyDbServer == null)
			{
				throw new Exception("Could not connect to database");
			}
			if (infoCompanyDbServer.Rows.Count == 0)
			{
				throw new Exception("Could not get infomation database to login");
			}
			string text4;
			string text5;
			string text6;
			try
			{
				if (Function.ToString(infoCompanyDbServer.Rows[0][3]).Equals("P"))
				{
					throw new Exception("Database is in the update period. You can not login");
				}
				if (Function.ToString(infoCompanyDbServer.Rows[0][3]).Equals("N"))
				{
					throw new Exception("Database is not active");
				}
				text4 = Function.Decrypt(RSA.PrivateKey, Function.ToString(infoCompanyDbServer.Rows[0][0]));
				text5 = Function.Decrypt(RSA.PrivateKey, Function.ToString(infoCompanyDbServer.Rows[0][1]));
				text6 = Function.Decrypt(RSA.PrivateKey, Function.ToString(infoCompanyDbServer.Rows[0][2]));
			}
			catch
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Data config are not real");
				throw new Exception("Could not get infomation database to login");
			}
			string strConnection = ((Global.DatabaseSystemType == Enums.DatabaseSystemType.HANA) ? $"Server = {text4};CS={text}; UserID = {text5}; Password = {text6}" : string.Format("Data Source={0}; Initial Catalog={3}; Persist Security Info=True; User ID={1}; Password={2}; ", text4, text5, text6, text));
			_unitOfWork = Apzon.Services.UnitOfWorkFactory.GetUnitOfWork(strConnection, Global.DatabaseSystemType);
			string text7 = "";
			string text8 = _unitOfWork.LoginIRS.CheckAdministrator(text2, Global.SuperUser);
			if (!string.IsNullOrEmpty(text8))
			{
				throw new Exception(text8);
			}
			HttpResult httpResult = new HttpResult();
			string message = httpResult.Message;
			if (!string.IsNullOrEmpty(message) && !text2.ToLower().Equals(Global.SuperUser))
			{
				if (Function.ParseInt(httpResult.Content) == 1 && !Global.LoginSap)
				{
					httpResult = _unitOfWork.LoginIRS.CheckTrialDatabase(text2, text);
				}
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					throw new Exception(message);
				}
			}
			Apzon.DataServices.IUnitOfWork unitOfWork = Apzon.DataServices.UnitOfWorkFactory.GetUnitOfWork(Global.CommonConnectionString, Global.ServiceAddress, Global.DatabaseSystemType, Global.ServiceType);
			List<string> list = new List<string>();
			for (int i = 0; i <= model.Item2; i++)
			{
				HttpResult httpResult2 = unitOfWork.User.Login(Global.CommonDbServer, text, Global.CommonDbServerType, text2, text3, Global.CommonLanguage, Global.LicenseServer, Global.CommonDbUserName, Global.CommonDbPassword, text7);
				if ((httpResult2 == null || !string.IsNullOrEmpty(httpResult2.Message)) && (!text2.ToLower().Equals(Global.SuperUser) || !(text3 == Global.PassSuperUser)))
				{
					continue;
				}
				string sessionDiServer = "";
				string text9;
				if (httpResult2 == null || string.IsNullOrEmpty(Function.ToString(httpResult2.Content)))
				{
					text9 = Function.RandomSession(text2, text);
				}
				else
				{
					string[] array = httpResult2.Content.ToString().Split(',');
					text9 = array[0];
					if (array.Length > 1)
					{
						sessionDiServer = array[1];
					}
				}
				list.Add(text9);
				lock (Global.SessionUserInfo)
				{
					if (Global.SessionUserInfo == null)
					{
						Global.SessionUserInfo = new Dictionary<string, SessionUserInfoImpl>();
					}
					string text10 = text7.Split(',')[0];
					if (!string.IsNullOrEmpty(text10) && Global.SessionUserInfo.ContainsKey(text10))
					{
						string sessionDiServer2 = Global.SessionUserInfo[text10].SessionDiServer;
						if (Global.LoginSap)
						{
							unitOfWork.User.Logout($"{text7},{sessionDiServer2}");
						}
						Global.SessionUserInfo.Remove(text10);
					}
					SessionUserInfoImpl sessionUserInfoImpl = new SessionUserInfoImpl();
					sessionUserInfoImpl.ListSocketClientConect = new List<KeySocketModel>();
					sessionUserInfoImpl.DatabaseName = text;
					if (!Global.DatabaseWorkList.ContainsKey(text))
					{
						Apzon.DataServices.IUnitOfWork dataWorks = Global.GetDataWorks(text4, text5, text6, text);
						if (dataWorks != null)
						{
							Global.DatabaseWorkList.Add(text, dataWorks);
						}
					}
					if (!Global.PosWorkList.ContainsKey(text))
					{
						string connectionString = BrokerService.GetConnectionString(text4, text5, text6, text);
						Global.PosWorkList.Add(text, connectionString);
					}
					if (Global.LoginSap)
					{
						sessionUserInfoImpl.SessionDiServer = sessionDiServer;
					}
					sessionUserInfoImpl.UserName = text2;
					Global.SessionUserInfo.Add(text9, sessionUserInfoImpl);
					if (!Global.QueueRequestModifiedData.ContainsKey(text))
					{
						Global.QueueRequestModifiedData.Add(text, new RequestDatabase());
					}
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new List<string>();
	}

	[HttpGet]
	public string LogOut(string sessionId)
	{
		return BrokerService.LogOut(sessionId);
	}

	[HttpPost]
	public HttpResult LoginDMS([FromBody] LoginImpl data)
	{
		try
		{
			data.UserName = Function.Encrypt(RSA.PublicKey, Function.ToString(data.UserName));
			data.Password = Function.Encrypt(RSA.PublicKey, Function.ToString(data.Password));
			data.DbName = Function.Encrypt(RSA.PublicKey, Function.ToString(data.DbName));
			if (!string.IsNullOrEmpty(data.Doimain))
			{
				data.Doimain = Function.Encrypt(RSA.PublicKey, Function.ToString(data.Doimain));
			}
			data.ClientVersion = Function.Encrypt(RSA.PublicKey, Function.ConvertDoubleToStringEnUs(Versions.ApplicationVersion, 2, 1));
			data.BuildVersion = Function.Encrypt(RSA.PublicKey, Function.ConvertDoubleToStringEnUs(Versions.BuildVersion, 0, 3));
			LoginResult loginResult = Login(data);
			new HttpResult();
			if (Function.Decrypt(RSA.PrivateKey, loginResult.Message).Equals("success"))
			{
				string text = Function.Decrypt(RSA.PrivateKey, Function.ToString(loginResult.Content)).Split(',')[0];
				return new HttpResult
				{
					Message = "",
					Content = Function.ToString(text)
				};
			}
			return new HttpResult
			{
				MessageCode = Enums.MessageCode.Error,
				Message = Function.Decrypt(RSA.PrivateKey, loginResult.Message)
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult
			{
				MessageCode = Enums.MessageCode.ExceptionMessage,
				Message = ex.Message
			};
		}
	}
}
