using System;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;

namespace Apzon.Api.Controllers.Administrator.Setup;

public class ConfigConnectController : BaseApiController
{
	private readonly string pathFptConfig = "ConfigFTPServer.dat";

	[HttpGet]
	public Function.ConfigData GetConfig()
	{
		return new Function.ConfigData
		{
			Server = Global.CommonDbServer,
			User = Global.CommonDbUserName,
			Pass = Global.CommonDbPassword
		};
	}

	[HttpPost]
	public void SaveConfig([FromBody] Function.ConfigData doc)
	{
		Function.setConfigData(doc, AppDomain.CurrentDomain.BaseDirectory + "bin\\\\config.dat");
	}

	[HttpGet]
	public bool CheckConnected(string _sql)
	{
		return Function.testConnection(_sql);
	}

	[HttpGet]
	public Function.ConfigFTPServer GetFtpConfig()
	{
		string text = AppDomain.CurrentDomain.BaseDirectory + pathFptConfig;
		if (FileIO.FileExist(text))
		{
			return Function.getConfigFTPServer(text);
		}
		return Function.getConfigFTPServer(AppDomain.CurrentDomain.BaseDirectory + "bin\\\\ConfigFTPServer.dat");
	}

	[HttpPost]
	public void SetFtpConfig([FromBody] Function.ConfigFTPServer doc)
	{
		Function.setConfigFTPServer(doc, AppDomain.CurrentDomain.BaseDirectory + pathFptConfig);
	}

	[HttpPost]
	public string CheckFtpConnection([FromBody] Function.ConfigFTPServer ftp)
	{
		return Function.CheckConnectFTP(ftp);
	}
}
