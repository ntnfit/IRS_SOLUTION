using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.Api.Controllers.Commons;

public class UpdateClientController : BaseApiController
{
	[HttpGet]
	public HttpResult CheckForUpdateClient(string dataBaseName, string appVersion, string buldVersion)
	{
		try
		{
			IUnitOfWork unitOfWork = UnitOfWorkFactory.GetUnitOfWork(Global.CommonConnectionString, Global.DatabaseSystemType);
			NumberFormatInfo provider = new NumberFormatInfo
			{
				NumberDecimalSeparator = ".",
				NumberGroupSeparator = ","
			};
			if (Versions.ApplicationVersion > Convert.ToDouble(appVersion, provider) || (Versions.ApplicationVersion == Convert.ToDouble(appVersion, provider) && (double)Versions.BuildVersion > Convert.ToDouble(buldVersion, provider)))
			{
				return new HttpResult(Enums.MessageCode.Success, "1", unitOfWork.Commons.GetNewApiAddress());
			}
			DataTable infoCompanyDbServer = unitOfWork.Commons.GetInfoCompanyDbServer(dataBaseName);
			if (infoCompanyDbServer == null)
			{
				return new HttpResult(Enums.MessageCode.Error, "Cannot connect to database", "");
			}
			if (infoCompanyDbServer.Rows.Count == 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "Cannot get infomation database", "");
			}
			string text;
			string text2;
			string text3;
			try
			{
				if (Function.ToString(infoCompanyDbServer.Rows[0][3]).Equals("P"))
				{
					return new HttpResult(Enums.MessageCode.Error, "Database is in the update period. You can not login", "");
				}
				if (Function.ToString(infoCompanyDbServer.Rows[0][3]).Equals("N"))
				{
					return new HttpResult(Enums.MessageCode.Error, "Database is not active", "");
				}
				text = Function.Decrypt(RSA.PrivateKey, Function.ToString(infoCompanyDbServer.Rows[0][0]));
				text2 = Function.Decrypt(RSA.PrivateKey, Function.ToString(infoCompanyDbServer.Rows[0][1]));
				text3 = Function.Decrypt(RSA.PrivateKey, Function.ToString(infoCompanyDbServer.Rows[0][2]));
			}
			catch
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Data config are not real");
				return new HttpResult(Enums.MessageCode.Error, "Cannot get infomation database", "");
			}
			string strConnection = ((Global.DatabaseSystemType == Enums.DatabaseSystemType.HANA) ? $"Server = {text};CS=\"{dataBaseName}\"; UserID = {text2}; Password = {text3}" : string.Format("Data Source={0}; Initial Catalog={3}; Persist Security Info=True; User ID={1}; Password={2}; ", text, text2, text3, dataBaseName));
			UnitOfWork = UnitOfWorkFactory.GetUnitOfWork(strConnection, Global.DatabaseSystemType);
			string databaseVersion = UnitOfWork.Commons.GetDatabaseVersion();
			if (string.IsNullOrEmpty(databaseVersion) || databaseVersion.Length != 8)
			{
				return new HttpResult(Enums.MessageCode.Error, "Cannot get database version", "");
			}
			appVersion = databaseVersion.Substring(0, 4);
			buldVersion = databaseVersion.Substring(5, 3);
			if (Versions.ApplicationVersion > Convert.ToDouble(appVersion, provider) || (Versions.ApplicationVersion == Convert.ToDouble(appVersion, provider) && (double)Versions.BuildVersion > Convert.ToDouble(buldVersion, provider)))
			{
				return new HttpResult(Enums.MessageCode.Success, "2", unitOfWork.Commons.GetNewApiAddress());
			}
			return new HttpResult(Enums.MessageCode.Success, "", "");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message, "");
		}
	}

	[HttpGet]
	public string CheckForUpdateClient(string appVersion, string buldVersion)
	{
		try
		{
			string arg = (string.IsNullOrWhiteSpace(Global.AppDataFolder) ? HttpContext.Current.Server.MapPath("~/App_Data/") : Global.AppDataFolder);
			string text = $"{arg}Apzon-IRS\\Update";
			if (!Directory.Exists(text))
			{
				return "Folder update Irs did not exists";
			}
			string text2 = $"{text}\\NewVersion.xml";
			if (!File.Exists(text2))
			{
				return "Could not check for update. Check file did not exists";
			}
			XElement xElement = XDocument.Load(text2).Element("NewVersion");
			if (xElement == null)
			{
				return "New Version update element could not null";
			}
			int idMax = xElement.Elements("Versions").Max((XElement obj) => int.Parse(obj.Attribute("Id").Value));
			XElement xElement2 = (from c in xElement.Elements("Versions")
				where (int)c.Attribute("Id") == idMax
				select c).SingleOrDefault();
			if (xElement2 == null)
			{
				return "New Version update could not null";
			}
			NumberFormatInfo provider = new NumberFormatInfo
			{
				NumberDecimalSeparator = ".",
				NumberGroupSeparator = ","
			};
			XElement xElement3 = xElement2.Element("VersionAp");
			XElement xElement4 = xElement2.Element("VersionBuild");
			if (xElement3 == null && xElement4 == null)
			{
				return "Version update could not null";
			}
			if (xElement3 != null)
			{
				double num = Convert.ToDouble(xElement3.Value, provider);
				if (xElement4 != null)
				{
					int num2 = Function.ParseInt(xElement4.Value);
					if (num > Convert.ToDouble(appVersion, provider) || (num == Convert.ToDouble(appVersion, provider) && (double)num2 > Convert.ToDouble(buldVersion, provider)))
					{
						return "1";
					}
					return "";
				}
				return "Could not get version build config";
			}
			return "Could not get version application config";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpPost]
	public HttpResponseMessage DownLoadFileUpdateClient([FromBody] Enums.PlatformClient platform)
	{
		try
		{
			string arg = (string.IsNullOrWhiteSpace(Global.AppDataFolder) ? HttpContext.Current.Server.MapPath("~/App_Data/") : Global.AppDataFolder);
			string sourceFolder = string.Format("{0}\\Apzon-IRS\\Update\\{1}", arg, (platform == Enums.PlatformClient.X86) ? "Updatex86" : "Updatex64");
			return new FileNetworkManagerController(fullPath: true).DownLoadFolder(sourceFolder);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return Request.CreateResponse(HttpStatusCode.BadRequest);
	}

	[HttpPost]
	public HttpResult UpdateDatabase([FromBody] string databaseName)
	{
		try
		{
			if (Global.ListAccessDb != null && Global.ListAccessDb.Count != 0)
			{
				if (!Global.ListAccessDb.Contains(databaseName.ToUpper()))
				{
					return new HttpResult(Enums.MessageCode.Error, "API could not process this database.");
				}
			}
			else if (Global.ListNotAccessDb != null && Global.ListNotAccessDb.Count != 0 && Global.ListNotAccessDb.Contains(databaseName.ToUpper()))
			{
				return new HttpResult(Enums.MessageCode.Error, "API could not process this database.");
			}
			lock (Global.DatabaseUpdate)
			{
				if (!Global.DatabaseUpdate.ContainsKey(databaseName))
				{
					Global.DatabaseUpdate.Add(databaseName, new object());
				}
			}
			lock (Global.DatabaseUpdate[databaseName])
			{
				IUnitOfWork unitOfWork = UnitOfWorkFactory.GetUnitOfWork(Global.CommonConnectionString, Global.DatabaseSystemType);
				NumberFormatInfo provider = new NumberFormatInfo
				{
					NumberDecimalSeparator = ".",
					NumberGroupSeparator = ","
				};
				DataTable infoCompanyDbServer = unitOfWork.Commons.GetInfoCompanyDbServer(databaseName);
				if (infoCompanyDbServer == null)
				{
					return new HttpResult(Enums.MessageCode.Error, "Cannot connect to database", "");
				}
				if (infoCompanyDbServer.Rows.Count == 0)
				{
					return new HttpResult(Enums.MessageCode.Error, "Cannot get infomation database", "");
				}
				string text;
				string text2;
				string text3;
				try
				{
					text = Function.Decrypt(RSA.PrivateKey, Function.ToString(infoCompanyDbServer.Rows[0][0]));
					text2 = Function.Decrypt(RSA.PrivateKey, Function.ToString(infoCompanyDbServer.Rows[0][1]));
					text3 = Function.Decrypt(RSA.PrivateKey, Function.ToString(infoCompanyDbServer.Rows[0][2]));
				}
				catch
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Data config are not real");
					return new HttpResult(Enums.MessageCode.Error, "Cannot get infomation database", "");
				}
				string strConnection = ((Global.DatabaseSystemType == Enums.DatabaseSystemType.HANA) ? $"Server = {text};CS=\"{databaseName}\"; UserID = {text2}; Password = {text3}" : string.Format("Data Source={0}; Initial Catalog={3}; Persist Security Info=True; User ID={1}; Password={2}; ", text, text2, text3, databaseName));
				UnitOfWork = UnitOfWorkFactory.GetUnitOfWork(strConnection, Global.DatabaseSystemType);
				string databaseVersion = UnitOfWork.Commons.GetDatabaseVersion();
				if (string.IsNullOrEmpty(databaseVersion) || databaseVersion.Length != 8)
				{
					return new HttpResult(Enums.MessageCode.Error, "Cannot get database version", "");
				}
				double num = Convert.ToDouble(databaseVersion.Substring(0, 4), provider);
				int num2 = Function.ParseInt(databaseVersion.Substring(5, 3));
				if (Versions.ApplicationVersion > num || (Versions.ApplicationVersion == num && Versions.BuildVersion > num2))
				{
					string arg = (string.IsNullOrWhiteSpace(Global.AppDataFolder) ? HttpContext.Current.Server.MapPath("~/App_Data/") : Global.AppDataFolder);
					string foldeUpdate = $"{arg}Apzon-IRS\\Update";
					Logging.Write("TRACE", "Update Database", string.Format("From {0} to {1}", databaseVersion, Function.ConvertDoubleToStringEnUs(Versions.ApplicationVersion, 2, 1) + "." + Function.ConvertDoubleToStringEnUs(Versions.BuildVersion, 0, 3)), databaseName);
					return UnitOfWork.Commons.UpdateDatabase(num, num2, foldeUpdate);
				}
				return new HttpResult(Enums.MessageCode.Success, "", "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString(), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message, "");
		}
	}
}
