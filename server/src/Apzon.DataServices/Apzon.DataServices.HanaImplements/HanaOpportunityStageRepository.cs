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

public class HanaOpportunityStageRepository : OpportunityStageRepository
{
	public HanaOpportunityStageRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public override bool Exists(string seqNo, string sessionId)
	{
		try
		{
			string query = "select 1 from OOST where \"Num\" = @seqNo;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@seqNo", seqNo));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	public override string Mapping(DataRow doc, EnumCommandType commandType)
	{
		try
		{
			return string.Concat(string.Concat(string.Concat(string.Concat(string.Concat(string.Concat("{" + Function.GetJsonString("Name", Function.ToString(doc["Descript"])), Function.GetJsonString("Stageno", doc["StepId"])), Function.GetJsonString("ClosingPercentage", doc["CloPrcnt"])), Function.GetJsonString("Cancelled", (Function.ToString(doc["Canceled"]) == "Y") ? "tYES" : "tNO")), Function.GetJsonString("IsSales", (Function.ToString(doc["SalesStage"]) == "Y") ? "tYES" : "tNO")), Function.GetJsonString("IsPurchasing", (Function.ToString(doc["PurStage"]) == "Y") ? "tYES" : "tNO")), "}");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
				bool flag = Exists(Function.ToString(row["Num"]), "");
				text = Mapping(row, (EnumCommandType)0);
				if (flag)
				{
					list.Add(new BodyRequestObject
					{
						Action = Enums.HttpAction.Update,
						BodyObject = text,
						ObjectType = ApzObjectTypes.oSalesStages,
						Key = Function.ParseInt(row["Num"]),
						OutObjectKey = true
					});
				}
				else
				{
					list.Add(new BodyRequestObject
					{
						Action = Enums.HttpAction.AddNew,
						BodyObject = text,
						ObjectType = ApzObjectTypes.oSalesStages
					});
				}
			}
			return DataService.ProcessMutiTransaction(sessionId, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
