using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaProjectRepository : ProjectRepository
{
	public HanaProjectRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public override string Save(DataTable dt, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				string code = Function.ToString(dt.Rows[i]["PrjCode"]);
				if (CheckExists(code, sessionId) == 0)
				{
					DataTable dataTable = dt.Copy().Clone();
					DataRow row = dt.Rows[i];
					dataTable.ImportRow(row);
					string bodyObject = Mapping(dataTable, sessionId);
					list.Add(new BodyRequestObject
					{
						ObjectType = ApzObjectTypes.oProjectCodes,
						OutObjectKey = true,
						Action = Enums.HttpAction.AddNew,
						BodyObject = bodyObject
					});
				}
				else
				{
					DataTable dataTable2 = dt.Copy().Clone();
					DataRow row2 = dt.Rows[i];
					dataTable2.ImportRow(row2);
					string bodyObject2 = Mapping(dataTable2, sessionId);
					list.Add(new BodyRequestObject
					{
						ObjectType = ApzObjectTypes.oProjectCodes,
						OutObjectKey = true,
						Action = Enums.HttpAction.Update,
						Key = dataTable2.Rows[0]["PrjCode"],
						BodyObject = bodyObject2
					});
				}
			}
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			string text = UpdateUDF(dt);
			if (!string.IsNullOrEmpty(text))
			{
				Logging.Write("ERROR", "[Projects setup] Update UDF failed: \"" + text + "\"");
				return text;
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public int CheckExists(string code, string sessionId)
	{
		try
		{
			if (string.IsNullOrEmpty(code))
			{
				return -1;
			}
			return Function.ParseInt(ExecuteScarlar("select count(1) from OPRJ where \"PrjCode\" = @code;", CommandType.Text, new SqlParameter("@code", code)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return -1;
	}

	public override string Mapping(DataTable dt, string sessionId)
	{
		try
		{
			string text = "{";
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				string text2 = Function.ToString(dt.Rows[i]["PrjCode"]);
				text += Function.GetJsonString("Code", Function.ToString(text2));
				text += Function.GetJsonString("Name", Function.ToString(dt.Rows[i]["PrjName"]));
				DateTime? dateTime = Function.ParseDateTime(dt.Rows[i]["ValidFrom"]);
				if (dateTime.HasValue)
				{
					_ = dateTime.Value;
					text += Function.GetJsonString("ValidFrom", dateTime.Value.ToString("yyyyMMdd"));
				}
				DateTime? dateTime2 = Function.ParseDateTime(dt.Rows[i]["ValidTo"]);
				if (dateTime2.HasValue)
				{
					_ = dateTime2.Value;
					text += Function.GetJsonString("ValidTo", dateTime2.Value.ToString("yyyyMMdd"));
				}
				text = ((!(Function.ToString(dt.Rows[i]["Active"]) == "Y")) ? (text + Function.GetJsonString("Active", "tNO")) : (text + Function.GetJsonString("Active", "tYES")));
				text += "}";
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "ERR:" + ex.Message;
		}
	}

	public override string DeleteProject(string code, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			list.Add(new BodyRequestObject
			{
				Action = Enums.HttpAction.Delete,
				Key = code,
				OutObjectKey = true,
				ObjectType = ApzObjectTypes.oProjectCodes
			});
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
