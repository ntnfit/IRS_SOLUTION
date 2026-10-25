using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web.Configuration;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Api.Hubs;
using Apzon.Api.Models;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MobileApp;
using Apzon.Services;

namespace Apzon.Api.Controllers.Administrator.Setup.General;

public class UsersController : BaseApiController
{
	private Apzon.Services.IUnitOfWork _unitOfWork;

	private readonly Apzon.DataServices.IUnitOfWork _sapUnitOfWork = Apzon.DataServices.UnitOfWorkFactory.GetUnitOfWork(Global.CommonConnectionString, Global.ServiceAddress, Global.DatabaseSystemType, Global.ServiceType);

	[HttpGet]
	public DataTable ShowUsers()
	{
		return UnitOfWork.Users.ShowUsers();
	}

	[HttpGet]
	public DataTable GetUsersByCode(string userCode)
	{
		return UnitOfWork.Users.GetUsersByCode(userCode);
	}

	[HttpGet]
	public ResponseResult GetUsersByCodeMobile(string userCode)
	{
		try
		{
			if (UnitOfWork == null)
			{
				return new ResponseResult
				{
					Message = "Invalid SesionId",
					Content = new
					{
						Table0 = "401"
					},
					MessageCode = "Lỗi401"
				};
			}
			DataTable usersByCode = UnitOfWork.Users.GetUsersByCode(userCode);
			usersByCode.TableName = "User";
			return new ResponseResult
			{
				Message = "",
				Content = new
				{
					Table0 = usersByCode
				},
				MessageCode = "0"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				Content = new
				{
					Table0 = ex.Message
				}
			};
		}
	}

	[HttpGet]
	public int GetMaxCode()
	{
		return UnitOfWork.Users.GetMaxCode();
	}

	[HttpPost]
	public DataTable Search([FromBody] UsersImpl doc)
	{
		return UnitOfWork.Users.Search(doc);
	}

	[HttpPost]
	public string DeleteDoc([FromBody] int code)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).Users.DeleteDoc(code, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.Users.DeleteDoc(code);
	}

	[HttpPost]
	public string SaveDoc([FromBody] UsersImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).Users.Save(doc, SessionUserInfo.SessionDiServer);
		}
		if (Function.ToString(doc.USER_CODE) != Function.Decrypt(RSA.PrivateKey, Function.ToString(ConfigurationManager.AppSettings["SuperUser"])))
		{
			return UnitOfWork.Users.SaveDoc(doc);
		}
		return "you cant't use account Administrator!";
	}

	[HttpGet]
	public DataTable ShowBranch()
	{
		return UnitOfWork.Users.ShowBranch();
	}

	[HttpPost]
	public string UpdateUsersAcount([FromBody] UsersImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return BrokerService.GetUnitOfWork(SessionId).Users.Save(doc, SessionUserInfo.SessionDiServer);
		}
		return UnitOfWork.Users.UpdateUsersAcount(doc);
	}

	[HttpGet]
	public DataTable GetUserLogin(string madk)
	{
		return UnitOfWork.Users.GetUserLogin(madk);
	}

	[HttpGet]
	public DataTable GetUserSign(string nameLogin)
	{
		return UnitOfWork.Users.GetUserSign(nameLogin);
	}

	[HttpGet]
	public DataSet GetALl()
	{
		try
		{
			return UnitOfWork.Users.GetAll();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	[HttpGet]
	public UserDefaultGroupImpl GetUserDefaultGroup(string username)
	{
		try
		{
			return UnitOfWork.Users.GetUserDefaultGroup(username);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new UserDefaultGroupImpl();
	}

	[HttpPost]
	public string SaveAdmin([FromBody] Tuple<string, string, string> model)
	{
		try
		{
			if (string.IsNullOrEmpty(model.Item1) || string.IsNullOrEmpty(model.Item2) || string.IsNullOrEmpty(model.Item3))
			{
				return "Data is not provide!";
			}
			if (!Function.Decrypt(RSA.PrivateKey, Function.ToString(ConfigurationManager.AppSettings["Pass"])).Equals(model.Item2))
			{
				return "Old Pass is not match!";
			}
			Configuration configuration = WebConfigurationManager.OpenWebConfiguration("~");
			AppSettingsSection appSettingsSection = (AppSettingsSection)configuration.GetSection("appSettings");
			appSettingsSection.Settings["SuperUser"].Value = Function.Encrypt(RSA.PublicKey, model.Item1);
			appSettingsSection.Settings["Pass"].Value = Function.Encrypt(RSA.PublicKey, model.Item3);
			configuration.Save();
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpGet]
	public string GetUserIdByEmpId(int empID)
	{
		try
		{
			return UnitOfWork.Users.GetUserIdByEmpId(empID);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	[HttpGet]
	public DataTable GetUserBranchAssignment(string userCode)
	{
		try
		{
			return UnitOfWork.Users.GetUserBranchAssignment(userCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable GetEmployee()
	{
		try
		{
			return UnitOfWork.Users.GetEmployee();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable LoadData()
	{
		try
		{
			return UnitOfWork.Users.LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	[HttpGet]
	public DataTable GetAnUser(int userId)
	{
		try
		{
			return UnitOfWork.Users.GetAnUser(userId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	[HttpPost]
	public ResponseResult GetSettings([FromBody] KeyInput key)
	{
		try
		{
			if (string.IsNullOrEmpty(key.secretKey))
			{
				return new ResponseResult
				{
					Message = "Invalid Key",
					MessageCode = "404"
				};
			}
			string applicationKey = Guid.NewGuid().ToString();
			DataTable aPISetting = Apzon.Services.UnitOfWorkFactory.GetUnitOfWork(Global.CommonConnectionString, Global.DatabaseSystemType).MobileDashboard.GetAPISetting(key.secretKey);
			if (aPISetting.IsNotNull())
			{
				return new ResponseResult
				{
					Message = "",
					Content = new UserSettings
					{
						Ip = Function.ToString(aPISetting.Rows[0]["Ip"]),
						Port = Function.ToString(aPISetting.Rows[0]["Port"]),
						ApiVersion = Function.ToString(aPISetting.Rows[0]["ApiVersion"]),
						ApplicationKey = applicationKey,
						LogoUrl = Function.ToString(aPISetting.Rows[0]["LogoUrl"])
					},
					MessageCode = "0"
				};
			}
			UserSettings content = new UserSettings
			{
				Ip = "http://vn.apzon.com",
				Port = "8990/API",
				ApiVersion = "1.00.002",
				ApplicationKey = applicationKey,
				LogoUrl = "https://www.upsieutoc.com/images/2019/03/12/Picture1.png"
			};
			return new ResponseResult
			{
				Message = "",
				Content = content,
				MessageCode = "0"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "502"
			};
		}
	}

	[HttpPost]
	public ResponseResult CustomerLogin([FromBody] LoginImpl model)
	{
		try
		{
			string dbName = Function.ToString(ConfigurationManager.AppSettings["MobileCompany"]);
			string username = model.UserName;
			string password = model.Password;
			if (string.IsNullOrEmpty(dbName))
			{
				return new ResponseResult
				{
					Message = "DataBase could not empty",
					MessageCode = "404"
				};
			}
			if (string.IsNullOrEmpty(username))
			{
				return new ResponseResult
				{
					Message = "the UserName could not empty",
					MessageCode = "404"
				};
			}
			if (string.IsNullOrEmpty(password))
			{
				return new ResponseResult
				{
					Message = "the Password could not empty",
					MessageCode = "404"
				};
			}
			Apzon.Services.IUnitOfWork unitOfWork = Apzon.Services.UnitOfWorkFactory.GetUnitOfWork(Global.CommonConnectionString, Global.DatabaseSystemType);
			DataTable infoCompanyDbServer = unitOfWork.Commons.GetInfoCompanyDbServer(dbName);
			if (infoCompanyDbServer == null)
			{
				return new ResponseResult
				{
					Message = "Could not connect to database",
					MessageCode = "404"
				};
			}
			if (infoCompanyDbServer.Rows.Count == 0)
			{
				return new ResponseResult
				{
					Message = "Could not get infomation database to login",
					MessageCode = "404"
				};
			}
			string text;
			string text2;
			string text3;
			try
			{
				if (Function.ToString(infoCompanyDbServer.Rows[0][3]).Equals("P"))
				{
					return new ResponseResult
					{
						Message = "Database is in the update period. You can not login",
						MessageCode = "405"
					};
				}
				if (Function.ToString(infoCompanyDbServer.Rows[0][3]).Equals("N"))
				{
					return new ResponseResult
					{
						Message = "Database is not active",
						MessageCode = "406"
					};
				}
				text = Function.Decrypt(RSA.PrivateKey, Function.ToString(infoCompanyDbServer.Rows[0][0]));
				text2 = Function.Decrypt(RSA.PrivateKey, Function.ToString(infoCompanyDbServer.Rows[0][1]));
				text3 = Function.Decrypt(RSA.PrivateKey, Function.ToString(infoCompanyDbServer.Rows[0][2]));
			}
			catch
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Data config are not real");
				return new ResponseResult
				{
					Message = "Could not get infomation database to login",
					MessageCode = "407"
				};
			}
			string strConnection = ((Global.DatabaseSystemType == Enums.DatabaseSystemType.HANA) ? $"Server = {text};CS=\"{dbName}\"; UserID = {text2}; Password = {text3}" : string.Format("Data Source={0}; Initial Catalog={3}; Persist Security Info=True; User ID={1}; Password={2}; ", text, text2, text3, dbName));
			_unitOfWork = Apzon.Services.UnitOfWorkFactory.GetUnitOfWork(strConnection, Global.DatabaseSystemType);
			string text4 = _unitOfWork.LoginIRS.CheckAdministrator(username, Global.SuperUser);
			if (!string.IsNullOrEmpty(text4))
			{
				return new ResponseResult
				{
					Message = text4,
					MessageCode = "182"
				};
			}
			HttpResult httpResult = new HttpResult();
			string message = httpResult.Message;
			if (!string.IsNullOrEmpty(message) && !username.ToLower().Equals(Global.SuperUser))
			{
				if (Function.ParseInt(httpResult.Content) == 1 && !Global.LoginSap)
				{
					httpResult = _unitOfWork.LoginIRS.CheckTrialDatabase(username, dbName);
				}
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					return new ResponseResult
					{
						Message = message,
						MessageCode = "182"
					};
				}
			}
			HttpResult httpResult2 = ((!Global.LoginSap) ? _unitOfWork.LoginIRS.LoginIrs(username, password, dbName) : _sapUnitOfWork.User.Login(Global.CommonDbServer, dbName, Global.CommonDbServerType, username, password, Global.CommonLanguage, Global.LicenseServer, Global.CommonDbUserName, Global.CommonDbPassword, $"{SessionId},{SessionUserInfo.SessionDiServer}"));
			if ((httpResult2 != null && string.IsNullOrEmpty(httpResult2.Message)) || (username.ToLower().Equals(Global.SuperUser) && password == Global.PassSuperUser))
			{
				string sessionDiServer = "";
				string text5;
				if (httpResult2 == null || string.IsNullOrEmpty(Function.ToString(httpResult2.Content)))
				{
					text5 = Function.RandomSession(username, dbName);
				}
				else
				{
					string[] array = httpResult2.Content.ToString().Split(',');
					text5 = array[0];
					if (array.Length > 1)
					{
						sessionDiServer = array[1];
					}
				}
				NotificationHub notificationHub = new NotificationHub();
				List<KeyValuePair<string, SessionUserInfoImpl>> list = (Global.CloudSystem ? Global.SessionUserInfo.Where((KeyValuePair<string, SessionUserInfoImpl> t) => t.Value.UserName.Equals(username) && t.Value.DatabaseName.Equals(dbName)).ToList() : Global.SessionUserInfo.Where((KeyValuePair<string, SessionUserInfoImpl> t) => t.Value.UserName.Equals(username)).ToList());
				for (int num = 0; num < list.Count; num++)
				{
					string key = list[num].Key;
					if (!key.Equals(SessionId) && list[num].Value.ListSocketClientConect != null && list[num].Value.ListSocketClientConect.Count > 0)
					{
						notificationHub.CheckOnline(isChangeLic: false, list[num].Value.ListSocketClientConect[0].SocketId);
					}
					BrokerService.LogOut(key);
				}
				lock (Global.SessionUserInfo)
				{
					BrokerService.LogOut(SessionId);
					SessionUserInfoImpl sessionUserInfoImpl = new SessionUserInfoImpl();
					sessionUserInfoImpl.ListSocketClientConect = new List<KeySocketModel>();
					sessionUserInfoImpl.DatabaseName = dbName;
					if (Global.DatabaseWorkList.Count == 0 || !Global.DatabaseWorkList.ContainsKey(dbName))
					{
						Apzon.DataServices.IUnitOfWork dataWorks = Global.GetDataWorks(text, text2, text3, dbName);
						if (dataWorks != null)
						{
							Global.DatabaseWorkList.Add(dbName, dataWorks);
						}
					}
					if (!Global.PosWorkList.ContainsKey(dbName))
					{
						string connectionString = BrokerService.GetConnectionString(text, text2, text3, dbName);
						Global.PosWorkList.Add(dbName, connectionString);
					}
					if (Global.LoginSap)
					{
						sessionUserInfoImpl.SessionDiServer = sessionDiServer;
					}
					sessionUserInfoImpl.UserName = username;
					Global.SessionUserInfo.Add(text5, sessionUserInfoImpl);
				}
				unitOfWork.LicenseManager.UpdateSessionUser(text5, username, dbName, model.ClientLocalIpAdress, model.ClientMachineName, model.ClientWindowsLogonName);
				string lastProductUpdatedDate = Apzon.Services.UnitOfWorkFactory.GetUnitOfWork(Global.PosWorkList[dbName], Global.DatabaseSystemType).MobileDashboard.GetLastProductUpdatedDate();
				return new ResponseResult
				{
					Content = new
					{
						SessionId = text5,
						LastProductUpdatedDate = Function.ParseDateTimes(lastProductUpdatedDate)
					},
					Message = "",
					MessageCode = "0"
				};
			}
			if (httpResult2 != null && !string.IsNullOrEmpty(httpResult2.Message))
			{
				return new ResponseResult
				{
					Message = ((httpResult2.Message.Length > 100) ? httpResult2.Message.Substring(0, 100) : httpResult2.Message),
					MessageCode = "182"
				};
			}
			return new ResponseResult
			{
				Message = "The username or password is incorrect",
				MessageCode = "183"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "502"
			};
		}
	}

	[HttpPost]
	public ResponseResult ChangePassword([FromBody] UserInput model)
	{
		try
		{
			_ = model.OldPassword;
			string newPassword = model.NewPassword;
			DataTable usersByCode = GetUsersByCode(SessionUserInfo.UserName);
			if (!usersByCode.IsNotNull())
			{
				return new ResponseResult
				{
					Message = "Could not found User",
					MessageCode = "404"
				};
			}
			UsersImpl usersImpl = new UsersImpl();
			usersImpl.USERID = Function.ParseInt(usersByCode.Rows[0]["USERID"]);
			usersImpl.USER_CODE = Function.ToString(usersByCode.Rows[0]["USER_CODE"]);
			usersImpl.U_NAME = Function.ToString(usersByCode.Rows[0]["U_NAME"]);
			usersImpl.INTERNAL_K = Function.ParseInt(usersByCode.Rows[0]["INTERNAL_K"]);
			usersImpl.GROUPS = Function.ParseInt(usersByCode.Rows[0]["GROUPS"]);
			usersImpl.SUPERUSER = Function.ToString(usersByCode.Rows[0]["SUPERUSER"]);
			usersImpl.E_Mail = Function.ToString(usersByCode.Rows[0]["E_Mail"]);
			usersImpl.PortNum = Function.ToString(usersByCode.Rows[0]["PortNum"]);
			usersImpl.Fax = Function.ToString(usersByCode.Rows[0]["Fax"]);
			usersImpl.Locked = Function.ToString(usersByCode.Rows[0]["Locked"]);
			usersImpl.Department = Function.ParseInt(usersByCode.Rows[0]["Department"]);
			usersImpl.Branch = Function.ParseInt(usersByCode.Rows[0]["Branch"]);
			usersImpl.PASSWORD = newPassword;
			usersImpl.IsChangePassword = true;
			string text = UpdateUsersAcount(usersImpl);
			if (string.IsNullOrEmpty(text))
			{
				return new ResponseResult
				{
					Message = "",
					MessageCode = "0"
				};
			}
			return new ResponseResult
			{
				Message = text,
				MessageCode = "184"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "502"
			};
		}
	}

	[HttpPost]
	public ResponseResult LogOut()
	{
		try
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new ResponseResult
				{
					Message = "",
					MessageCode = "0"
				};
			}
			BrokerService.LogOut(SessionId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new ResponseResult
		{
			Message = "",
			MessageCode = "0"
		};
	}

	[HttpGet]
	public DataTable DataPassAdmin()
	{
		try
		{
			return UnitOfWork.Users.DataPassAdmin();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	[HttpGet]
	public string GetUserCodeById(int id)
	{
		try
		{
			return UnitOfWork.Users.GetUserCodeById(id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}
}
