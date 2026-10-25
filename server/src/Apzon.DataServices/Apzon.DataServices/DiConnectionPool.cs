using System.Collections.Generic;
using System.Linq;
using Apzon.Commons;

namespace Apzon.DataServices;

public class DiConnectionPool
{
	public static List<DiConnection> DiConnections { get; set; }

	public static bool IsAllocating { get; set; }

	public static string OpenConnection(string sessionId)
	{
		if (DiConnections == null)
		{
			return sessionId;
		}
		DiConnection diConnection = DiConnections.FirstOrDefault((DiConnection t) => t.Id.Split(',').Contains(sessionId));
		if (diConnection == null)
		{
			return sessionId;
		}
		return diConnection.SessionId;
	}

	public static string ReConnect(string sessionId)
	{
		if (DiConnections != null)
		{
			DiConnection diConnection = DiConnections.FirstOrDefault((DiConnection t) => t.SessionId.Contains(sessionId));
			if (diConnection != null)
			{
				string text = Function.ToString(new DiServices().Login(diConnection.DbServer, diConnection.DbName, diConnection.DbTypeStr, diConnection.DbUserName, diConnection.DbPwd, diConnection.UserName, diConnection.UserPwd, diConnection.Lang, diConnection.LicServer).Content);
				if (string.IsNullOrEmpty(text) || !text.ToUpper().Contains("ERROR"))
				{
					diConnection.SessionId = text;
					Logging.Write("WATCH", $"API auto ReConnect\t{diConnection.DbName}\t{diConnection.UserName}", text);
					return text;
				}
				Logging.Write("ERROR", $"API auto ReConnect Error\t{diConnection.DbName}\t{diConnection.UserName}\t{text}");
			}
			else
			{
				Logging.Write("ERROR", "Unknow Session. " + sessionId);
			}
		}
		else
		{
			Logging.Write("ERROR", "DI Connections is empty");
		}
		return string.Empty;
	}

	public static void CloseConnection(string sessionId)
	{
		if (DiConnections != null)
		{
			DiConnection diConnection = DiConnections.FirstOrDefault((DiConnection t) => t.Id.Split(',').Contains(sessionId));
			if (diConnection != null)
			{
				DiConnections.Remove(diConnection);
			}
		}
	}
}
