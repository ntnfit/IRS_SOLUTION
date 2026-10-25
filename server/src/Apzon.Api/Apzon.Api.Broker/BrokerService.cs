using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Apzon.Api.Hubs;
using Apzon.Api.Models;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.Api.Broker;

public class BrokerService
{
	public static Apzon.DataServices.IUnitOfWork GetUnitOfWork(string sessionId)
	{
		try
		{
			if (!string.IsNullOrEmpty(sessionId))
			{
				if (Global.SessionUserInfo.ContainsKey(sessionId))
				{
					string databaseName = Global.SessionUserInfo[sessionId].DatabaseName;
					if (Global.DatabaseWorkList.ContainsKey(databaseName))
					{
						return Global.DatabaseWorkList[databaseName];
					}
				}
				else
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Session Db is null");
				}
			}
			else
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Session Id is null");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}

	public static Apzon.Services.IUnitOfWork GetApzUnitOfWork(string sessionId)
	{
		try
		{
			if (!string.IsNullOrEmpty(sessionId))
			{
				if (Global.SessionUserInfo.ContainsKey(sessionId))
				{
					string databaseName = Global.SessionUserInfo[sessionId].DatabaseName;
					if (Global.PosWorkList.ContainsKey(databaseName))
					{
						return Apzon.Services.UnitOfWorkFactory.GetUnitOfWork(Global.PosWorkList[databaseName], Global.DatabaseSystemType);
					}
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "PosWorkList is null");
				}
				else
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Session Db is null", sessionId);
				}
			}
			else
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Session Id is null");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}

	public static string LogOut(string sessionId)
	{
		try
		{
			if (string.IsNullOrEmpty(sessionId))
			{
				return "";
			}
			string text = "";
			if (Global.SessionUserInfo.ContainsKey(sessionId))
			{
				text = Global.SessionUserInfo[sessionId].UserName;
			}
			if (string.IsNullOrEmpty(text))
			{
				return "";
			}
			Apzon.DataServices.IUnitOfWork unitOfWork = Apzon.DataServices.UnitOfWorkFactory.GetUnitOfWork(Global.CommonConnectionString, Global.ServiceAddress, Global.DatabaseSystemType, Global.ServiceType);
			Apzon.Services.UnitOfWorkFactory.GetUnitOfWork(Global.CommonConnectionString, Global.DatabaseSystemType).LicenseManager.UpdateSession(sessionId);
			Apzon.Services.IUnitOfWork apzUnitOfWork = GetApzUnitOfWork(sessionId);
			if (apzUnitOfWork == null)
			{
				Logging.Write("TRACE", $"Không tìm thấy dữ liệu session {sessionId} trên hệ thống");
			}
			else
			{
				apzUnitOfWork.LicenseManager.UpdateDataTrial(text);
			}
			lock (Global.SessionUserInfo)
			{
				if (Global.LoginSap)
				{
					string sessionDiServer = Global.SessionUserInfo[sessionId].SessionDiServer;
					unitOfWork.User.Logout(sessionId + "," + sessionDiServer);
				}
				Global.SessionUserInfo.Remove(sessionId);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public static LoginResult Login(LoginImpl model, string SessionId, string SesssionDI)
	{
		try
		{
			string dbName = Function.Decrypt(RSA.PrivateKey, model.DbName);
			string username = Function.Decrypt(RSA.PrivateKey, model.UserName);
			string text = Function.Decrypt(RSA.PrivateKey, model.Password);
			if (string.IsNullOrEmpty(dbName))
			{
				return new LoginResult
				{
					Message = Function.Encrypt(RSA.PublicKey, "Database cannot be empty")
				};
			}
			if (Global.ListAccessDb != null && Global.ListAccessDb.Count != 0)
			{
				if (!Global.ListAccessDb.Contains(dbName.ToUpper()))
				{
					return new LoginResult
					{
						Message = Function.Encrypt(RSA.PublicKey, "Cannot login to this database")
					};
				}
			}
			else if (Global.ListNotAccessDb != null && Global.ListNotAccessDb.Count != 0 && Global.ListNotAccessDb.Contains(dbName.ToUpper()))
			{
				return new LoginResult
				{
					Message = Function.Encrypt(RSA.PublicKey, "Cannot login to this database")
				};
			}
			if (string.IsNullOrEmpty(username))
			{
				return new LoginResult
				{
					Message = Function.Encrypt(RSA.PublicKey, "UserName cannot be empty")
				};
			}
			if (string.IsNullOrEmpty(text))
			{
				return new LoginResult
				{
					Message = Function.Encrypt(RSA.PublicKey, "Password cannot be empty")
				};
			}
			string text2 = Function.Decrypt(RSA.PrivateKey, model.ClientVersion);
			int num = Function.ParseInt(Function.Decrypt(RSA.PrivateKey, model.BuildVersion));
			if (string.IsNullOrEmpty(text2))
			{
				return new LoginResult
				{
					Message = Function.Encrypt(RSA.PublicKey, "Client version is incorrect.")
				};
			}
			if (!text2.Equals(Function.ConvertDoubleToStringEnUs(Versions.ApplicationVersion, 2, 1)) || num != Versions.BuildVersion)
			{
				return new LoginResult
				{
					Message = Function.Encrypt(RSA.PublicKey, "Client version and server version do not match.")
				};
			}
			Apzon.Services.IUnitOfWork unitofWorkCommon = Apzon.Services.UnitOfWorkFactory.GetUnitOfWork(Global.CommonConnectionString, Global.DatabaseSystemType);
			DataTable infoCompanyDbServer = unitofWorkCommon.Commons.GetInfoCompanyDbServer(dbName);
			if (infoCompanyDbServer == null)
			{
				return new LoginResult
				{
					Message = Function.Encrypt(RSA.PublicKey, "Cannot connect to database")
				};
			}
			if (infoCompanyDbServer.Rows.Count == 0)
			{
				return new LoginResult
				{
					Message = Function.Encrypt(RSA.PublicKey, "Cannot get database infomation to login")
				};
			}
			string text3;
			string text4;
			string text5;
			try
			{
				if (Function.ToString(infoCompanyDbServer.Rows[0][3]).Equals("P"))
				{
					return new LoginResult
					{
						Message = Function.Encrypt(RSA.PublicKey, "Database is in the update period. You cannot login")
					};
				}
				if (Function.ToString(infoCompanyDbServer.Rows[0][3]).Equals("N"))
				{
					return new LoginResult
					{
						Message = Function.Encrypt(RSA.PublicKey, "Database is not active")
					};
				}
				text3 = Function.Decrypt(RSA.PrivateKey, Function.ToString(infoCompanyDbServer.Rows[0][0]));
				text4 = Function.Decrypt(RSA.PrivateKey, Function.ToString(infoCompanyDbServer.Rows[0][1]));
				text5 = Function.Decrypt(RSA.PrivateKey, Function.ToString(infoCompanyDbServer.Rows[0][2]));
			}
			catch
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Data config are not real");
				return new LoginResult
				{
					Message = Function.Encrypt(RSA.PublicKey, "Cannot get the database infomation to login")
				};
			}
			Apzon.Services.IUnitOfWork unitOfWork = Apzon.Services.UnitOfWorkFactory.GetUnitOfWork(GetConnectionString(text3, text4, text5, dbName), Global.DatabaseSystemType);
			string text6 = unitOfWork.Commons.CheckExitsDatabase(dbName);
			if (string.IsNullOrEmpty(text6))
			{
				return new LoginResult
				{
					Message = Function.Encrypt(RSA.PublicKey, "Cannot connect to database server")
				};
			}
			if (!text6.Equals($"{text2}.{Function.ConvertDoubleToStringEnUs(num, 0, 3)}"))
			{
				return new LoginResult
				{
					Message = Function.Encrypt(RSA.PublicKey, "Version Client and Database does not match")
				};
			}
			if (Global.SessionUserInfo == null)
			{
				Global.SessionUserInfo = new Dictionary<string, SessionUserInfoImpl>();
			}
			string text7 = unitOfWork.LoginIRS.CheckAdministrator(username, Global.SuperUser);
			if (!string.IsNullOrEmpty(text7))
			{
				return new LoginResult
				{
					Message = Function.Encrypt(RSA.PublicKey, text7)
				};
			}
			HttpResult httpResult = unitofWorkCommon.LoginIRS.CheckLicenseIrs(unitofWorkCommon.Commons.GetHardwarekey(Global.CloudSystem, dbName), username, dbName, $"{text2}.{Function.ConvertDoubleToStringEnUs(num, 0, 3)}");
			string message = httpResult.Message;
			if (!string.IsNullOrEmpty(message) && !username.ToLower().Equals(Global.SuperUser))
			{
				if (httpResult.Content == null && !Global.LoginSap)
				{
					HttpResult httpResult2 = unitOfWork.LoginIRS.CheckTrialDatabase(username, dbName);
					httpResult.Message = httpResult2.Message;
					httpResult.Content = httpResult2.Content;
				}
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					return new LoginResult
					{
						Message = Function.Encrypt(RSA.PublicKey, message)
					};
				}
			}
			Apzon.DataServices.IUnitOfWork unitOfWork2 = Apzon.DataServices.UnitOfWorkFactory.GetUnitOfWork(Global.CommonConnectionString, Global.ServiceAddress, Global.DatabaseSystemType, Global.ServiceType);
			HttpResult httpResult3 = ((!Global.LoginSap) ? unitOfWork.LoginIRS.LoginIrs(username, text, dbName) : unitOfWork2.User.Login(Global.CommonDbServer, dbName, Global.CommonDbServerType, username, text, Global.CommonLanguage, Global.LicenseServer, Global.CommonDbUserName, Global.CommonDbPassword, $"{SessionId},{SesssionDI}"));
			if ((httpResult3 != null && string.IsNullOrEmpty(httpResult3.Message)) || (username.ToLower().Equals(Global.SuperUser) && text == Global.PassSuperUser))
			{
				string sessionDiServer = "";
				string newSession;
				if (httpResult3 == null || string.IsNullOrEmpty(Function.ToString(httpResult3.Content)))
				{
					newSession = Function.RandomSession(username, dbName);
				}
				else
				{
					string[] array = httpResult3.Content.ToString().Split(',');
					newSession = array[0];
					if (array.Length > 1)
					{
						sessionDiServer = array[1];
					}
				}
				Task.Run(() =>
				{
					unitofWorkCommon.LicenseManager.UpdateSessionUser(newSession, username, dbName, model.ClientLocalIpAdress, model.ClientMachineName, model.ClientWindowsLogonName);
				});
				UserLicenseImpl userLicenseImpl = httpResult.Content as UserLicenseImpl;
				List<KeyValuePair<string, SessionUserInfoImpl>> list;
				lock (Global.SessionUserInfo)
				{
					LogOut(SessionId);
					list = (Global.CloudSystem ? Global.SessionUserInfo.Where((KeyValuePair<string, SessionUserInfoImpl> t) => t.Value.UserName.Equals(username) && t.Value.DatabaseName.Equals(dbName)).ToList() : Global.SessionUserInfo.Where((KeyValuePair<string, SessionUserInfoImpl> t) => t.Value.UserName.Equals(username)).ToList());
					SessionUserInfoImpl sessionUserInfoImpl = new SessionUserInfoImpl();
					sessionUserInfoImpl.ListSocketClientConect = new List<KeySocketModel>();
					sessionUserInfoImpl.DatabaseName = dbName;
					lock (Global.DatabaseWorkList)
					{
						if (!Global.DatabaseWorkList.ContainsKey(dbName))
						{
							Apzon.DataServices.IUnitOfWork dataWorks = Global.GetDataWorks(text3, text4, text5, dbName);
							if (dataWorks != null)
							{
								Global.DatabaseWorkList.Add(dbName, dataWorks);
							}
						}
					}
					lock (Global.PosWorkList)
					{
						if (!Global.PosWorkList.ContainsKey(dbName))
						{
							string connectionString = GetConnectionString(text3, text4, text5, dbName);
							Global.PosWorkList.Add(dbName, connectionString);
						}
					}
					if (Global.LoginSap)
					{
						sessionUserInfoImpl.SessionDiServer = sessionDiServer;
					}
					sessionUserInfoImpl.UserName = username;
					sessionUserInfoImpl.ListLicense = ((userLicenseImpl != null && userLicenseImpl.LstLicenseMapping != null) ? userLicenseImpl.LstLicenseMapping.Select((LicenseInfoImpl t) => t.TypeCode).ToList() : new List<string>());
					Global.SessionUserInfo.Add(newSession, sessionUserInfoImpl);
					lock (Global.QueueRequestModifiedData)
					{
						if (!Global.QueueRequestModifiedData.ContainsKey(dbName))
						{
							Global.QueueRequestModifiedData.Add(dbName, new RequestDatabase());
						}
					}
				}
				NotificationHub notificationHub = new NotificationHub();
				for (int num2 = 0; num2 < list.Count; num2++)
				{
					string key = list[num2].Key;
					if (!key.Equals(SessionId) && list[num2].Value.ListSocketClientConect != null && list[num2].Value.ListSocketClientConect.Count > 0)
					{
						KeySocketModel keySocketModel = list[num2].Value.ListSocketClientConect.Where((KeySocketModel s) => s.SocketType == "NotificationHub").FirstOrDefault();
						if (keySocketModel != null)
						{
							notificationHub.CheckOnline(isChangeLic: false, keySocketModel.SocketId);
						}
					}
					LogOut(key);
				}
				return new LoginResult
				{
					Content = Function.Encrypt(RSA.PublicKey, string.Format("{0},{1},{2}", newSession, "P", Global.LoginSap ? "1" : "0")),
					Message = Function.Encrypt(RSA.PublicKey, "success"),
					LicenseInfo = userLicenseImpl
				};
			}
			if (httpResult3 != null && !string.IsNullOrEmpty(httpResult3.Message))
			{
				return new LoginResult
				{
					Message = Function.Encrypt(RSA.PublicKey, (httpResult3.Message.Length > 100) ? httpResult3.Message.Substring(0, 100) : httpResult3.Message)
				};
			}
			return new LoginResult
			{
				Message = Function.Encrypt(RSA.PublicKey, "Username or password is incorrect")
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new LoginResult
			{
				Message = Function.Encrypt(RSA.PublicKey, (ex.Message.Length > 100) ? ex.Message.Substring(0, 100) : ex.Message)
			};
		}
	}

	public static string GetConnectionString(string serverName, string dbUserName, string dbPassword, string databaseName)
	{
		return Global.DatabaseSystemType switch
		{
			Enums.DatabaseSystemType.SQL => string.Format("Data Source={0}; Initial Catalog={3}; Persist Security Info=True; User ID={1}; Password={2};Max Pool Size=32767;Pooling=true; ", serverName, dbUserName, dbPassword, databaseName), 
			Enums.DatabaseSystemType.HANA => $"Server = {serverName};CS=\"{databaseName}\"; UserID = {dbUserName}; Password = {dbPassword}", 
			_ => "", 
		};
	}
}
