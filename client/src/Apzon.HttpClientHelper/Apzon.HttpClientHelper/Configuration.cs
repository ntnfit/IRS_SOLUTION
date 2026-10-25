using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Apzon.Commons;

namespace Apzon.HttpClientHelper;

public class Configuration
{
	public static string ApiServer = GetApiServer();

	public static string MoPlanning = GetMoPlanning();

	public static bool FcPurchaseRequest = GetFcPurchaseRequest();

	public static bool KmvPr = GetKmvPr();

	public static int SearchDate = GetSearchDate();

	private static string GetApiServer()
	{
		try
		{
			string pathRoot = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
			string startupFolderName = Function.GetStartupFolderName();
			string text = string.Format("{0}\\Users\\All Users\\APZON\\{2}\\{1}\\Config\\ShareConfig.xml", pathRoot, Environment.UserName, startupFolderName);
			if (File.Exists(text))
			{
				System.Configuration.Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
				{
					ExeConfigFilename = text
				}, ConfigurationUserLevel.None);
				if (configuration.AppSettings.Settings.AllKeys.Contains("ApiServer") && !string.IsNullOrEmpty(configuration.AppSettings.Settings["ApiServer"].Value))
				{
					return configuration.AppSettings.Settings["ApiServer"].Value;
				}
			}
			string text2 = $"{Environment.CurrentDirectory}\\Apzon.Forms.exe.config";
			if (File.Exists(text2))
			{
				string directoryName = Path.GetDirectoryName(text);
				if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				File.Copy(text2, text, overwrite: true);
			}
			return ConfigurationManager.AppSettings["ApiServer"];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Error ", ex.ToString());
			return "";
		}
	}

	private static string GetMoPlanning()
	{
		try
		{
			string startupFolderName = Function.GetStartupFolderName();
			string pathRoot = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
			string text = string.Format("{0}\\Users\\All Users\\APZON\\{2}\\{1}\\Config\\ShareConfig.xml", pathRoot, Environment.UserName, startupFolderName);
			if (File.Exists(text))
			{
				System.Configuration.Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
				{
					ExeConfigFilename = text
				}, ConfigurationUserLevel.None);
				if (configuration.AppSettings.Settings.AllKeys.Contains("MOPlanning") && !string.IsNullOrEmpty(configuration.AppSettings.Settings["MOPlanning"].Value))
				{
					return configuration.AppSettings.Settings["MOPlanning"].Value;
				}
			}
			return ConfigurationManager.AppSettings["MOPlanning"];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Error ", ex.ToString());
			return "";
		}
	}

	private static bool GetFcPurchaseRequest()
	{
		try
		{
			string startupFolderName = Function.GetStartupFolderName();
			string pathRoot = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
			string text = string.Format("{0}\\Users\\All Users\\APZON\\{2}\\{1}\\Config\\ShareConfig.xml", pathRoot, Environment.UserName, startupFolderName);
			if (File.Exists(text))
			{
				System.Configuration.Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
				{
					ExeConfigFilename = text
				}, ConfigurationUserLevel.None);
				if (configuration.AppSettings.Settings.AllKeys.Contains("FcPurchaseRequest") && !string.IsNullOrEmpty(configuration.AppSettings.Settings["FcPurchaseRequest"].Value))
				{
					return configuration.AppSettings.Settings["FcPurchaseRequest"].Value.Equals("1");
				}
			}
			return Function.ToString(ConfigurationManager.AppSettings["FcPurchaseRequest"]).Equals("1");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Error ", ex.ToString());
			return false;
		}
	}

	private static bool GetKmvPr()
	{
		try
		{
			string startupFolderName = Function.GetStartupFolderName();
			string pathRoot = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
			string text = string.Format("{0}\\Users\\All Users\\APZON\\{2}\\{1}\\Config\\ShareConfig.xml", pathRoot, Environment.UserName, startupFolderName);
			if (File.Exists(text))
			{
				System.Configuration.Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
				{
					ExeConfigFilename = text
				}, ConfigurationUserLevel.None);
				if (configuration.AppSettings.Settings.AllKeys.Contains("KmvPr") && !string.IsNullOrEmpty(configuration.AppSettings.Settings["KmvPr"].Value))
				{
					return configuration.AppSettings.Settings["KmvPr"].Value.Equals("1");
				}
			}
			return Function.ToString(ConfigurationManager.AppSettings["KmvPr"]).Equals("1");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Error ", ex.ToString());
			return false;
		}
	}

	private static int GetSearchDate()
	{
		try
		{
			string startupFolderName = Function.GetStartupFolderName();
			string pathRoot = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
			string text = string.Format("{0}\\Users\\All Users\\APZON\\{2}\\{1}\\Config\\ShareConfig.xml", pathRoot, Environment.UserName, startupFolderName);
			if (File.Exists(text))
			{
				System.Configuration.Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
				{
					ExeConfigFilename = text
				}, ConfigurationUserLevel.None);
				if (configuration.AppSettings.Settings.AllKeys.Contains("SearchDate") && !string.IsNullOrEmpty(configuration.AppSettings.Settings["SearchDate"].Value))
				{
					return Function.ParseInt(configuration.AppSettings.Settings["SearchDate"].Value);
				}
			}
			return Function.ParseInt(ConfigurationManager.AppSettings["SearchDate"]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Error ", ex.ToString());
			return 1;
		}
	}
}
