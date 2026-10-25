using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using Apzon.Api.Controllers.Commons;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;

namespace Apzon.Api.Controllers.Administrator.Plugin;

public class PluginManagerController : BaseApiController
{
	[HttpGet]
	public DataTable GetData(string databaseName)
	{
		DataTable data = UnitOfWork.PluginManager.GetData();
		try
		{
			bool flag = false;
			FileNetworkManagerController fileNetworkManagerController = new FileNetworkManagerController();
			List<string> list = new List<string>();
			for (int i = 0; i < data.Rows.Count; i++)
			{
				string text = Function.ToString(data.Rows[i]["Name"]);
				if (fileNetworkManagerController.CheckExistFolder(string.Format("{0}\\{2}\\{1}", databaseName, text, "Extensions"), isFullPath: false).Equals("0"))
				{
					flag = true;
					data.Rows.RemoveAt(i);
					i--;
				}
				else
				{
					list.Add(text);
				}
			}
			List<string> list2 = new List<string>();
			if (string.IsNullOrWhiteSpace(Global.AppDataFolder))
			{
				if (Directory.Exists(HttpContext.Current.Server.MapPath(string.Format("~/App_Data/{0}/{1}/{2}", databaseName, "Extensions", databaseName))))
				{
					list2 = Directory.GetDirectories(HttpContext.Current.Server.MapPath($"~/App_Data/{databaseName}/Extensions")).ToList();
				}
			}
			else if (Directory.Exists(Path.Combine(Global.AppDataFolder, databaseName, "Extensions", databaseName)))
			{
				list2 = Directory.GetDirectories(Path.Combine(Global.AppDataFolder, databaseName, "Extensions")).ToList();
			}
			for (int j = 0; j < list2.Count; j++)
			{
				if (!list.Contains(Function.ToString(Path.GetFileName(list2[j]))))
				{
					fileNetworkManagerController.DeleteFolder(list2[j]);
				}
			}
			data.AcceptChanges();
			if (flag)
			{
				UnitOfWork.PluginManager.Update(data);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return data;
	}

	[HttpPost]
	public string Save([FromBody] DataTable dt)
	{
		return UnitOfWork.PluginManager.Save(dt);
	}

	[HttpPost]
	public string Delete([FromBody] string plugName)
	{
		try
		{
			new FileNetworkManagerController().DeleteFolder(plugName);
			return UnitOfWork.PluginManager.Delete(plugName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
