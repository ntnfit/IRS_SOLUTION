using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Apzon.Api.JsonFormatter;
using Apzon.Api.Models;
using Apzon.Commons;
using Apzon.Commons.ApzJson;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.Entities.Models;
using Apzon.Entities.Models.UserFields;
using Apzon.Services;

namespace Apzon.Api;

public class Global : HttpApplication
{
	public static string LicenseServer = Function.Decrypt(RSA.PrivateKey, ConfigurationManager.AppSettings["LicenseServer"]);

	public static string CommonDbUserName = Function.Decrypt(RSA.PrivateKey, ConfigurationManager.AppSettings["CommonDbUserName"]);

	public static string CommonDbPassword = Function.Decrypt(RSA.PrivateKey, ConfigurationManager.AppSettings["CommonDbPassword"]);

	public static string CommonDbServerType = Function.ToString(ConfigurationManager.AppSettings["CommonDbServerType"]);

	public static string CommonLanguage = Function.ToString(ConfigurationManager.AppSettings["CommonLanguage"]);

	public static string CommonDbServer = Function.Decrypt(RSA.PrivateKey, Function.ToString(ConfigurationManager.AppSettings["CommonDbServer"]));

	public static string CommonDb = Function.Decrypt(RSA.PrivateKey, ConfigurationManager.AppSettings["CommonDb"]);

	public static string ServiceAddress = Function.Decrypt(RSA.PrivateKey, ConfigurationManager.AppSettings["ServiceAddress"]);

	public static int QueueRequestWait = Function.ParseInt(ConfigurationManager.AppSettings["QueueRequestWait"]);

	public static int MaxRequestProcess = Function.ParseInt(ConfigurationManager.AppSettings["MaxRequestProcess"]);

	public static int RequestTimeout = Function.ParseInt(ConfigurationManager.AppSettings["RequestTimeout"]);

	public static int MrpRequestTimeout = Function.ParseInt(ConfigurationManager.AppSettings["MrpClientRequestTimeout"]);

	public static Enums.ServiceType ServiceType = (Enums.ServiceType)Function.ParseInt(Function.Decrypt(RSA.PrivateKey, ConfigurationManager.AppSettings["ServiceType"]));

	public static Enums.DatabaseSystemType DatabaseSystemType = (Enums.DatabaseSystemType)Function.ParseInt(Function.Decrypt(RSA.PrivateKey, ConfigurationManager.AppSettings["DatabaseSystemType"]));

	public static string BiometricServer = Function.ToString(ConfigurationManager.AppSettings["BiometricServer"]);

	public static int BiometricPort = Function.ParseInt(ConfigurationManager.AppSettings["BiometricPort"]);

	public static string PassSuperUser = Function.Decrypt(RSA.PrivateKey, Function.ToString(ConfigurationManager.AppSettings["Pass"]));

	public static string SuperUser = Function.Decrypt(RSA.PrivateKey, Function.ToString(ConfigurationManager.AppSettings["SuperUser"])).ToLower();

	public static Dictionary<string, Apzon.DataServices.IUnitOfWork> DatabaseWorkList = new Dictionary<string, Apzon.DataServices.IUnitOfWork>();

	public static Dictionary<string, string> PosWorkList = new Dictionary<string, string>();

	public static Dictionary<string, SessionUserInfoImpl> SessionUserInfo = new Dictionary<string, SessionUserInfoImpl>();

	public static Dictionary<string, TokenAuthorizationImpl> TokenUserInfo = new Dictionary<string, TokenAuthorizationImpl>();

	public static Dictionary<string, string> CacheImage100 = new Dictionary<string, string>();

	public static Dictionary<string, List<UserFieldsImpl>> ListUDF = new Dictionary<string, List<UserFieldsImpl>>();

	public static Dictionary<string, object> DatabaseUpdate = new Dictionary<string, object>();

	public static List<string> ListAccessDb;

	public static List<string> ListNotAccessDb;

	public static string AppDataFolder = ConfigurationManager.AppSettings["AppDataFolder"];

	public static Dictionary<string, RequestDatabase> QueueRequestModifiedData = new Dictionary<string, RequestDatabase>();

	public static bool LoginSap => Function.Decrypt(RSA.PrivateKey, ConfigurationManager.AppSettings["LoginSap"]).Equals("1");

	public static bool CloudSystem => Function.Decrypt(RSA.PrivateKey, ConfigurationManager.AppSettings["CloudSystem"]).Equals("101");

	public static string CommonConnectionString
	{
		get
		{
			if (DatabaseSystemType != Enums.DatabaseSystemType.HANA)
			{
				return string.Format("Data Source={0}; Initial Catalog={3}; Persist Security Info=True; User ID={1}; Password={2}; ", CommonDbServer, CommonDbUserName, CommonDbPassword, string.IsNullOrEmpty(CommonDb) ? "SBO-COMMON" : CommonDb);
			}
			return $"Server = {CommonDbServer};CS=\"{CommonDb}\"; UserID = {CommonDbUserName}; Password = {CommonDbPassword}";
		}
	}

	public static SboCommonContext SboCommonContext
	{
		get
		{
			if (DatabaseSystemType != Enums.DatabaseSystemType.HANA)
			{
				return new SboCommonContext(CommonConnectionString);
			}
			return new SboCommonContext("");
		}
	}

	public static Apzon.DataServices.IUnitOfWork GetDataWorks(string cpnDbName, string cpnDbUser, string cpnDbPass, string dbName)
	{
		try
		{
			return Apzon.DataServices.UnitOfWorkFactory.GetUnitOfWork((DatabaseSystemType == Enums.DatabaseSystemType.HANA) ? $"Server = {cpnDbName};CS=\"{dbName}\"; UserID = {cpnDbUser}; Password = {cpnDbPass}" : string.Format("Data Source={0}; Initial Catalog={3}; Persist Security Info=True; User ID={1}; Password={2};Max Pool Size=32767;Pooling=true; ", cpnDbName, cpnDbUser, cpnDbPass, dbName), ServiceAddress, DatabaseSystemType, ServiceType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}

	internal static Apzon.Services.IUnitOfWork GetPosWorks(string cpnDbName, string cpnDbUser, string cpnDbPass, string dbName)
	{
		try
		{
			return Apzon.Services.UnitOfWorkFactory.GetUnitOfWork((DatabaseSystemType == Enums.DatabaseSystemType.HANA) ? $"Server = {cpnDbName};CS=\"{dbName}\"; UserID = {cpnDbUser}; Password = {cpnDbPass}" : string.Format("Data Source={0}; Initial Catalog={3}; Persist Security Info=True; User ID={1}; Password={2};Max Pool Size=32767;Pooling=true; ", cpnDbName, cpnDbUser, cpnDbPass, dbName), DatabaseSystemType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}

	private void Application_Start(object sender, EventArgs e)
	{
		AreaRegistration.RegisterAllAreas();
		GlobalConfiguration.Configure(WebApiConfig.Register);
		RouteConfig.RegisterRoutes(RouteTable.Routes);
		GlobalConfiguration.Configuration.Formatters.Clear();
		GlobalConfiguration.Configuration.Formatters.Add(new ApzJsonFormater());
		GlobalConfiguration.Configuration.Formatters.Add(new ApzJsonFormatterWorkFlowProcess());
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, (RemoteCertificateValidationCallback)((object s, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true));
		string text = ConfigurationManager.AppSettings["ListAccessDB"] ?? "";
		if (string.IsNullOrWhiteSpace(text))
		{
			return;
		}
		if (!text.StartsWith("!"))
		{
			ListAccessDb = (from i in text.Split(',')
				where !string.IsNullOrWhiteSpace(i)
				select i.Trim().ToUpper()).ToList();
		}
		else
		{
			ListNotAccessDb = (from i in text.Substring(1).Split(',')
				where !string.IsNullOrWhiteSpace(i)
				select i.Trim().ToUpper()).ToList();
		}
	}

	protected void Application_BeginRequest()
	{
		if (Request.Headers.AllKeys.Contains("Origin") && Request.HttpMethod == "OPTIONS")
		{
			Response.Flush();
		}
	}
}
