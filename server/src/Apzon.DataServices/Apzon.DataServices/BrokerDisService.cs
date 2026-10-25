using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Apzon.Commons;
using Apzon.DataServices.DiServerServiceWebRef;

namespace Apzon.DataServices;

public static class BrokerDisService
{
	public static string DisAddress = Function.ToString(ConfigurationManager.AppSettings["DISAddress"]);

	public static Dictionary<string, int> ListDis = new Dictionary<string, int>();

	public static Dictionary<string, string> UserDisList = new Dictionary<string, string>();

	public static void SetListDis()
	{
		try
		{
			if (ListDis != null && ListDis.Count != 0)
			{
				return;
			}
			if (string.IsNullOrEmpty(DisAddress))
			{
				DisAddress = Function.ToString(new DiServerServiceSoapClient().Endpoint.Address.Uri).Split('/')[2];
			}
			ListDis = new Dictionary<string, int>();
			foreach (string item in DisAddress.Split(',').Distinct().ToList())
			{
				ListDis[item] = 0;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public static DiServerServiceSoapClient GetUserDiServer(string sessionId)
	{
		try
		{
			if (UserDisList.ContainsKey(sessionId))
			{
				string text = UserDisList[sessionId];
				if (ListDis.Count > 1)
				{
					Logging.Write("WATCH", "Connecting DIS:", text, sessionId);
				}
				return new DiServerServiceSoapClient("DiServerServiceSoap", $"http://{text}/DIS/DISERVERSERVICE.ASMX");
			}
			return GetAvaiableDiServer();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DiServerServiceSoapClient();
	}

	public static DiServerServiceSoapClient GetAvaiableDiServer()
	{
		if (ListDis != null && ListDis.Count > 0)
		{
			foreach (KeyValuePair<string, int> item in ListDis.OrderBy((KeyValuePair<string, int> x) => x.Value).ToList())
			{
				string key = item.Key;
				if (!string.IsNullOrEmpty(key))
				{
					DiServerServiceSoapClient diServerServiceSoapClient = CreateDiServer(key);
					if (diServerServiceSoapClient != null)
					{
						return diServerServiceSoapClient;
					}
				}
			}
		}
		return new DiServerServiceSoapClient();
	}

	public static DiServerServiceSoapClient CreateDiServer(string ipServer)
	{
		DiServerServiceSoapClient diServerServiceSoapClient = new DiServerServiceSoapClient("DiServerServiceSoap", $"http://{ipServer}/DIS/DISERVERSERVICE.ASMX");
		try
		{
			string serviceStatus = diServerServiceSoapClient.GetServiceStatus();
			if (!string.IsNullOrEmpty(serviceStatus) && serviceStatus.Equals("Running"))
			{
				return diServerServiceSoapClient;
			}
			Logging.Write("WATCH", ipServer + " SBODI_Server Status", serviceStatus);
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ipServer, ex.Message);
			return null;
		}
	}

	public static void CloseDiServer(string sessionId)
	{
		try
		{
			if (UserDisList != null && UserDisList.ContainsKey(sessionId))
			{
				string key = UserDisList[sessionId];
				int value = ListDis[key] - 1;
				ListDis[key] = value;
				UserDisList.Remove(key);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public static string ExecuteBatFile(string command)
	{
		try
		{
			if (File.Exists(command))
			{
				Process process = Process.Start(new ProcessStartInfo("cmd.exe", "/c " + command)
				{
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardError = true,
					RedirectStandardOutput = true
				});
				process.WaitForExit();
				string text = process.StandardOutput.ReadToEnd();
				string text2 = process.StandardError.ReadToEnd();
				int exitCode = process.ExitCode;
				Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "output>>" + (string.IsNullOrEmpty(text) ? "(none)" : text));
				Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "error>>" + (string.IsNullOrEmpty(text2) ? "(none)" : text2));
				Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ExitCode: " + exitCode, "ExecuteCommand");
				process.Close();
			}
			else
			{
				Logging.Write("WATCH", "Error: No file found", command);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "";
	}
}
