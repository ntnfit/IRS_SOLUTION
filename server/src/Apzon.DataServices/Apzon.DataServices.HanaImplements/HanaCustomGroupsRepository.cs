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

public class HanaCustomGroupsRepository : CustomGroupsRepository
{
	public HanaCustomGroupsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	public override bool CheckExists(string cstGroupCode)
	{
		string query = "select 1 from OARG where \"CstGrpCode\"=@cstGroupCode;";
		try
		{
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@cstGroupCode", cstGroupCode));
			return (dataTable.IsNotNull() && dataTable.Rows.Count > 0) ? true : false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return false;
		}
	}

	public override string Mapping(DataRow doc, string sessionId)
	{
		string text = "{";
		try
		{
			text += Function.GetJsonString("Code", Function.ToString(doc["CstGrpCode"]));
			text += Function.GetJsonString("Name", Function.ToString(doc["CstGrpName"]));
			text += Function.GetJsonString("Number", Function.ToString(doc["GroupNum"]));
			text += Function.GetJsonString("Customs", doc["Custom"]);
			text += Function.GetJsonString("Purchase", doc["BuyTax"]);
			text += Function.GetJsonString("Other", doc["OtherTax"]);
			text += Function.GetJsonString("Total", doc["TotalTax"]);
			text = ((!(Function.ToString(doc["Locked"]) == "Y")) ? (text + Function.GetJsonString("Locked", "tNO")) : (text + Function.GetJsonString("Locked", "tYES")));
			text += Function.GetJsonString("CustomsAllocationAccount", Function.ToString(doc["cstAllcAcc"]));
			text += Function.GetJsonString("CustomsExpenseAccount", Function.ToString(doc["cstExpAcc"]));
			text += Function.GetJsonString("PortAddress", Function.ToString(doc["PortAddr"]));
			text += Function.GetJsonString("PortState", Function.ToString(doc["PortState"]));
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
				if (CheckExists(Function.ToString(row["CstGrpCode"])))
				{
					text = Mapping(row, sessionId);
					list.Add(new BodyRequestObject
					{
						BodyObject = text,
						Action = Enums.HttpAction.Update,
						ObjectType = ApzObjectTypes.oCustomsGroups,
						Key = Function.ParseInt(row["CstGrpCode"]),
						OutObjectKey = true
					});
				}
				else
				{
					text = Mapping(row, sessionId);
					list.Add(new BodyRequestObject
					{
						Action = Enums.HttpAction.AddNew,
						ObjectType = ApzObjectTypes.oCustomsGroups,
						BodyObject = text
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
		List<BodyRequestObject> list = new List<BodyRequestObject>();
		try
		{
			foreach (DataRow row in dt.Rows)
			{
				list.Add(new BodyRequestObject
				{
					Action = Enums.HttpAction.Delete,
					ObjectType = ApzObjectTypes.oCustomsGroups,
					Key = Function.ParseInt(row["CstGrpCode"]),
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
