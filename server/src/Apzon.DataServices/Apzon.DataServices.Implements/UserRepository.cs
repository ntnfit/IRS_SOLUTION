using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class UserRepository : Repository<UsersImpl>, IUser, IRepository<UsersImpl>
{
	public UserRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public virtual HttpResult Login(string dbServer, string dbName, string dbTypeStr, string userName, string userPwd, string language, string licServer, string dbuserName, string dbPwd, string sessionId)
	{
		try
		{
			string text = Function.ToString(DataService.Login(dbServer, dbName, dbTypeStr, dbuserName, dbPwd, userName, userPwd, language, licServer).Content);
			string text2 = text;
			if (!string.IsNullOrEmpty(text) && text.ToUpper().Contains("ERROR"))
			{
				Logging.Write("ERROR", $"User Log In Error\t{dbName}\t{userName}\t{text}");
				throw new Exception(text);
			}
			Logging.Write("WATCH", $"User Log In\t{dbName}\t{userName}", text);
			if (DiConnectionPool.DiConnections == null)
			{
				DiConnectionPool.DiConnections = new List<DiConnection>();
			}
			if (!DiConnectionPool.IsAllocating)
			{
				DiConnection diConnection = DiConnectionPool.DiConnections.FirstOrDefault((DiConnection t) => t.SessionId == sessionId);
				if (diConnection != null)
				{
					diConnection.Id = string.Join(",", text2, text);
					diConnection.SessionId = text;
					diConnection.DbServer = dbServer;
					diConnection.DbName = dbName;
					diConnection.DbTypeStr = dbTypeStr;
					diConnection.UserName = userName;
					diConnection.UserPwd = userPwd;
					diConnection.Lang = language;
					diConnection.LicServer = licServer;
					diConnection.DbUserName = dbuserName;
					diConnection.DbPwd = dbPwd;
				}
				else
				{
					DiConnectionPool.DiConnections.Add(new DiConnection
					{
						Id = string.Join(",", text2, text),
						SessionId = text,
						DbServer = dbServer,
						DbName = dbName,
						DbTypeStr = dbTypeStr,
						UserName = userName,
						UserPwd = userPwd,
						Lang = language,
						LicServer = licServer,
						DbUserName = dbuserName,
						DbPwd = dbPwd
					});
				}
				Logging.Write("WATCH", "DiConnections count:" + DiConnectionPool.DiConnections.Count);
			}
			HttpResult httpResult = new HttpResult();
			httpResult.Content = string.Join(",", text2, text);
			httpResult.Message = "";
			return httpResult;
		}
		catch (Exception ex)
		{
			return new HttpResult
			{
				Content = "",
				Message = ex.Message
			};
		}
	}

	public HttpResult Logout(string sessionId)
	{
		if (!string.IsNullOrEmpty(sessionId))
		{
			try
			{
				string[] array = sessionId.Split(',');
				DataService.LogOut(array[0]);
				Logging.Write("WATCH", "User Log Out: " + array[0]);
				return new HttpResult();
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
			}
		}
		return new HttpResult();
	}
}
