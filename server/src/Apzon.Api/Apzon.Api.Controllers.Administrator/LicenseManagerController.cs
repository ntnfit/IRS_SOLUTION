using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Controllers;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Api.Hubs;
using Apzon.Api.Models;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.Api.Controllers.Administrator;

[PreventBrowser]
public class LicenseManagerController : BaseApiController
{
	protected IUnitOfWork UnitOfWorkCommons;

	protected IUnitOfWork UnitofWork;

	protected override void Initialize(HttpControllerContext controllerContext)
	{
		try
		{
			string text = Function.ToString(controllerContext.Request.Headers.GetValues("SessionId").FirstOrDefault());
			if (!string.IsNullOrEmpty(text))
			{
				IUnitOfWork apzUnitOfWork = BrokerService.GetApzUnitOfWork(text);
				if (apzUnitOfWork != null)
				{
					UnitOfWorkCommons = UnitOfWorkFactory.GetUnitOfWork(Global.CommonConnectionString, Global.DatabaseSystemType);
					UnitofWork = apzUnitOfWork;
					base.Initialize(controllerContext);
					return;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		base.Initialize(controllerContext);
	}

	[HttpGet]
	public string GetHardwareKey(string database)
	{
		return UnitOfWorkCommons.Commons.GetHardwarekey(Global.CloudSystem, database);
	}

	[HttpGet]
	public string GetIpServer()
	{
		string result = "";
		try
		{
			IPAddress[] addressList = Dns.GetHostByName(Dns.GetHostName()).AddressList;
			for (int i = 0; i < addressList.Length; i++)
			{
				result = addressList[i].ToString();
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	[HttpPost]
	public string Save([FromBody] Tuple<string, string, string> model)
	{
		if (string.IsNullOrEmpty(model.Item1))
		{
			return "Can not load license file";
		}
		try
		{
			string databaseVersion = UnitOfWork.Commons.GetDatabaseVersion();
			if (!model.Item2.Equals(databaseVersion))
			{
				return "Client version and Server version does not match";
			}
			Function.ReadLicense(model.Item1, out var bpCode, out var installNumber, out var hardwareKey, out var fileId, out var sysType, out var dateCreate, out var version, out var lstLicense);
			if (hardwareKey != GetHardwareKey(model.Item3))
			{
				return "License file is used for another server";
			}
			if (!Function.CompareVersionLicenseAndVersionIrs(version, databaseVersion))
			{
				return "License file is used for another Version IRS";
			}
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("TypeCode", typeof(string));
			dataTable.Columns.Add("Amount", typeof(int));
			for (int i = 0; i < lstLicense.Count; i++)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow[0] = lstLicense[i].TypeCode;
				dataRow[1] = lstLicense[i].Amount;
				dataTable.Rows.Add(dataRow);
			}
			dataTable.AcceptChanges();
			string text = UnitOfWorkCommons.LicenseManager.Save(bpCode, installNumber, hardwareKey, model.Item1, fileId, sysType, dateCreate, dataTable);
			if (string.IsNullOrEmpty(text))
			{
				new NotificationHub().CheckOnline(isChangeLic: true);
				string databaseName = SessionUserInfo.DatabaseName;
				List<string> list = Global.SessionUserInfo.Select((KeyValuePair<string, SessionUserInfoImpl> t) => t.Key).ToList();
				for (int num = 0; num < list.Count; num++)
				{
					string text2 = list[num];
					if (!Global.CloudSystem || Global.SessionUserInfo[text2].DatabaseName.Equals(databaseName))
					{
						BrokerService.LogOut(text2);
					}
				}
				return "";
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpGet]
	public DataTable GetUser()
	{
		try
		{
			return UnitofWork.LicenseManager.GetUser();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	[HttpPost]
	public DataTable GetLicenseUser([FromBody] Tuple<string, string, string> model)
	{
		try
		{
			return UnitOfWorkCommons.LicenseManager.GetLicenseUser(model.Item1, model.Item2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public Tuple<string, string, string, DataTable, List<Common.Table_Code>> GetLicense(string database, string versionAp)
	{
		try
		{
			DataSet license = UnitOfWorkCommons.LicenseManager.GetLicense(GetHardwareKey(database));
			if (license == null || license.Tables.Count != 2 || license.Tables[0].IsNotNull())
			{
				string file = Function.ToString(license.Tables[0].Rows[0][0]);
				string value = Function.ToString(license.Tables[0].Rows[0][1]);
				Function.ReadLicense(file, out var _, out var installNumber, out var hardwareKey, out var _, out var sysType, out var _, out var version, out var lstLicense);
				if (!Function.CompareVersionLicenseAndVersionIrs(version, versionAp))
				{
					return new Tuple<string, string, string, DataTable, List<Common.Table_Code>>("License use for other version!", "", "", new DataTable(), new List<Common.Table_Code>());
				}
				if (lstLicense.Count == 0)
				{
					return new Tuple<string, string, string, DataTable, List<Common.Table_Code>>("License file does not have a user license!", "", "", new DataTable(), new List<Common.Table_Code>());
				}
				if (!hardwareKey.Equals(GetHardwareKey(database)) || !installNumber.Equals(value))
				{
					return new Tuple<string, string, string, DataTable, List<Common.Table_Code>>("Data base casting arbitrarily be changed!", "", "", new DataTable(), new List<Common.Table_Code>());
				}
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("LicenseType", typeof(string));
				dataTable.Columns.Add("Count", typeof(int));
				dataTable.Columns.Add("Use", typeof(int));
				dataTable.Columns.Add("ExpiredDate", typeof(DateTime));
				List<Common.Table_Code> list = new List<Common.Table_Code>();
				DataTable source = license.Tables[1];
				int i;
				for (i = 0; i < lstLicense.Count; i++)
				{
					DataRow dataRow = source.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["LicenseType"]).Equals(lstLicense[i].TypeCode));
					if (dataRow != null && lstLicense[i].Amount < Function.ParseInt(dataRow["Used"]))
					{
						return new Tuple<string, string, string, DataTable, List<Common.Table_Code>>("Data base casting arbitrarily be changed!", "", "", new DataTable(), new List<Common.Table_Code>());
					}
					DataRow dataRow2 = dataTable.NewRow();
					dataRow2[0] = lstLicense[i].TypeCode;
					dataRow2[1] = lstLicense[i].Amount;
					dataRow2[2] = ((dataRow != null) ? Function.ParseInt(dataRow["Used"]) : 0);
					dataRow2[3] = lstLicense[i].ExpiredDate;
					dataTable.Rows.Add(dataRow2);
					Common.Table_Code table_Code = new Common.Table_Code();
					table_Code.Code = lstLicense[i].TypeCode;
					table_Code.Name = lstLicense[i].TypeName;
					list.Add(table_Code);
				}
				return new Tuple<string, string, string, DataTable, List<Common.Table_Code>>("", installNumber, sysType, dataTable, list);
			}
			return new Tuple<string, string, string, DataTable, List<Common.Table_Code>>("The system had not been imported license", "", "", new DataTable(), new List<Common.Table_Code>());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new Tuple<string, string, string, DataTable, List<Common.Table_Code>>("cant not read license file", "", "", new DataTable(), new List<Common.Table_Code>());
		}
	}

	[HttpPost]
	public string SaveUser([FromBody] Tuple<DataTable, string, string> model)
	{
		string text = UnitOfWorkCommons.LicenseManager.SaveUser(model.Item1, model.Item2, model.Item3);
		try
		{
			if (string.IsNullOrEmpty(text))
			{
				List<string> list = new List<string>();
				lock (Global.SessionUserInfo)
				{
					string databaseName = SessionUserInfo.DatabaseName;
					EnumerableRowCollection<DataRow> lstRowLic = model.Item1.AsEnumerable();
					foreach (KeyValuePair<string, SessionUserInfoImpl> data in Global.SessionUserInfo)
					{
						if (data.Value.DatabaseName.Equals(databaseName) && !data.Value.UserName.Equals(Global.SuperUser, StringComparison.OrdinalIgnoreCase) && (data.Value.ListLicense == null || !data.Value.ListLicense.Exists((string t) => lstRowLic.Any((DataRow x) => Function.ToString(x["User"]).Equals(data.Value.UserName, StringComparison.OrdinalIgnoreCase) && Function.ToString(x["LicenseType"]).Equals(t)))))
						{
							list.Add(data.Key);
						}
					}
				}
				NotificationHub notificationHub = new NotificationHub();
				for (int num = 0; num < list.Count; num++)
				{
					string text2 = list[num];
					if (Global.SessionUserInfo.ContainsKey(text2) && Global.SessionUserInfo[text2].ListSocketClientConect != null && Global.SessionUserInfo[text2].ListSocketClientConect.Count > 0)
					{
						KeySocketModel keySocketModel = Global.SessionUserInfo[text2].ListSocketClientConect.Where((KeySocketModel s) => s.SocketType == "NotificationHub").FirstOrDefault();
						if (keySocketModel != null)
						{
							notificationHub.CheckOnline(isChangeLic: false, keySocketModel.SocketId);
						}
					}
					BrokerService.LogOut(text2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	[HttpGet]
	public string DeleteAllLicenseUser(string database)
	{
		return UnitOfWorkCommons.LicenseManager.DeleteAllLicenseUser(UnitOfWorkCommons.Commons.GetHardwarekey(Global.CloudSystem, database));
	}
}
