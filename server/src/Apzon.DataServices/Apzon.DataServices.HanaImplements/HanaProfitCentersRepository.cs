using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaProfitCentersRepository : ProfitCentersRepository
{
	public HanaProfitCentersRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public override string Mapping(DataRow row, string sessionId)
	{
		string text = "{";
		try
		{
			text += Function.GetJsonString("CenterCode", row["PrcCode"]);
			text += Function.GetJsonString("CenterName", row["PrcName"]);
			text += Function.GetJsonString("GroupCode", row["GrpCode"]);
			text += Function.GetJsonString("InWhichDimension", row["DimCode"]);
			text += Function.GetJsonString("CostCenterType", row["CCTypeCode"]);
			text += Function.GetJsonString("EffectiveFrom", DateTime.Parse(Function.ToString(row["ValidFrom"])).ToString("yyyy-MM-dd"));
			text += Function.GetJsonString("EffectiveTo", string.IsNullOrEmpty(row["ValidTo"]?.ToString() ?? "") ? Function.ToString(row["ValidTo"]) : DateTime.Parse(Function.ToString(row["ValidTo"])).ToString("yyyy-MM-dd"));
			text += Function.GetJsonString("Active", row["Active"]);
			return text + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	public bool CheckExists(string prcCode, string sessionId)
	{
		string query = "select 1 from OPRC where \"PrcCode\"=@prcCode;";
		try
		{
			if (ExecuteDataTable(query, CommandType.Text, new SqlParameter("@prcCode", prcCode)).IsNotNull())
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return false;
		}
	}

	public override string Save(string sessionId, DataTable dt)
	{
		HttpResult httpResult = new HttpResult();
		string text = "";
		List<BodyRequestObject> list = new List<BodyRequestObject>();
		try
		{
			foreach (DataRow row in dt.Rows)
			{
				if (CheckExists(Function.ToString(row["PrcCode"]), sessionId))
				{
					list.Add(new BodyRequestObject
					{
						Action = Enums.HttpAction.Update,
						Key = Function.ToString(row["PrcCode"]),
						ObjectType = ApzObjectTypes.oProfitCenters
					});
				}
				else
				{
					text = Mapping(row, sessionId);
					list.Add(new BodyRequestObject
					{
						Action = Enums.HttpAction.AddNew,
						Key = Function.ToString(row["PrcCode"]),
						ObjectType = ApzObjectTypes.oProfitCenters,
						BodyObject = text
					});
				}
			}
			httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	public override string Remove(string sessionId, string prcCode)
	{
		new HttpResult();
		try
		{
			return DataService.ProcessJson(ApzObjectTypes.oProfitCenters, sessionId, "", Enums.HttpAction.Delete, prcCode).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	public new void Dispose()
	{
	}
}
