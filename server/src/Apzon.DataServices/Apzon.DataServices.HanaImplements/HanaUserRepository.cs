using System;
using System.Collections.Generic;
using System.Linq;
using Apzon.Commons;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaUserRepository : UserRepository
{
	public HanaUserRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public override HttpResult Login(string dbServer, string dbName, string dbTypeStr, string userName, string userPwd, string language, string licServer, string dbuserName, string dbPwd, string sessionId)
	{
		try
		{
			HttpResult httpResult = DataService.Login(dbServer, dbName, dbTypeStr, dbuserName, dbPwd, userName, userPwd, language, licServer);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", $"User Log In Error\t{dbName}\t{userName}\t{httpResult.Message}");
				throw new Exception(httpResult.Message);
			}
			Logging.Write("WATCH", $"User Log In\t{dbName}\t{userName}", Function.ToString(httpResult.Content));
			if (HanaService.LstDiConnections == null)
			{
				HanaService.LstDiConnections = new List<DiConnection>();
			}
			DiConnection diConnection = HanaService.LstDiConnections.FirstOrDefault((DiConnection t) => t != null && t.SessionId == sessionId);
			if (diConnection != null)
			{
				diConnection.Id = string.Join(",", httpResult.Content, httpResult.Content);
				diConnection.SessionId = Function.ToString(httpResult.Content);
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
				HanaService.LstDiConnections.Add(new DiConnection
				{
					Id = string.Join(",", httpResult.Content, httpResult.Content),
					SessionId = Function.ToString(httpResult.Content),
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
			Logging.Write("WATCH", "SAP Service Connection Count: " + HanaService.LstDiConnections.Count);
			HttpResult httpResult2 = new HttpResult();
			httpResult2.Content = string.Join(",", httpResult.Content, httpResult.Content);
			httpResult2.Message = "";
			return httpResult2;
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
}
