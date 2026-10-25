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

public class HanaWeightRepository : WeightRepository
{
	public HanaWeightRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	public override int CheckExists(int unitCode, string sessionId)
	{
		string query = "select 1 from OWGT where \"UnitCode\"=@untCode;";
		try
		{
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@untCode", unitCode)).Rows.Count;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return 0;
		}
	}

	public override string Mapping(DataRow doc, string sessionId)
	{
		string text = "{";
		try
		{
			text += Function.GetJsonString("UnitDisplay", doc["UnitDisply"]);
			text += Function.GetJsonString("UnitWeightinmg", Function.ToString(doc["WightInMG"]));
			text += Function.GetJsonString("UnitName", doc["UnitName"]);
			return text + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return "";
		}
	}

	public override HttpResult Save(DataTable dt, string sessionId)
	{
		string text = "";
		List<BodyRequestObject> list = new List<BodyRequestObject>();
		try
		{
			foreach (DataRow row in dt.Rows)
			{
				if (CheckExists(Function.ParseInt(row["UnitCode"]?.ToString() ?? ""), sessionId) == 0)
				{
					text = Mapping(row, sessionId);
					list.Add(new BodyRequestObject
					{
						Action = Enums.HttpAction.AddNew,
						BodyObject = text,
						ObjectType = ApzObjectTypes.oWeightMeasures
					});
				}
				else
				{
					text = Mapping(row, sessionId);
					list.Add(new BodyRequestObject
					{
						Action = Enums.HttpAction.Update,
						BodyObject = text,
						ObjectType = ApzObjectTypes.oWeightMeasures,
						Key = Function.ParseInt(row["UnitCode"]),
						OutObjectKey = true
					});
				}
			}
			return DataService.ProcessMutiTransaction(sessionId, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult Remove(DataTable dt, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			foreach (DataRow row in dt.Rows)
			{
				list.Add(new BodyRequestObject
				{
					Action = Enums.HttpAction.Delete,
					Key = Function.ParseInt(row["UnitCode"]?.ToString() ?? ""),
					ObjectType = ApzObjectTypes.oWeightMeasures,
					OutObjectKey = true
				});
			}
			return DataService.ProcessMutiTransaction(sessionId, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
