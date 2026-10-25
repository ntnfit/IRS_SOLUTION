using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaSalesPersonsRepository : SalesPersonsRepository
{
	public HanaSalesPersonsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public override string Save(string sessionId, DataTable dt)
	{
		try
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				return "SalesPersonsRepository->Save(): No rows to save";
			}
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				if (Exist(Function.ParseInt(dt.Rows[i]["SlpCode"]), sessionId))
				{
					list.Add(new BodyRequestObject
					{
						ObjectType = ApzObjectTypes.oSalesPersons,
						Action = Enums.HttpAction.Update,
						BodyObject = Mapping(dt.Rows[i], sessionId, EnumCommandType.Update),
						Key = Function.ParseInt(dt.Rows[i]["SlpCode"])
					});
				}
				else
				{
					list.Add(new BodyRequestObject
					{
						ObjectType = ApzObjectTypes.oSalesPersons,
						Action = Enums.HttpAction.AddNew,
						BodyObject = Mapping(dt.Rows[i], sessionId, EnumCommandType.Add)
					});
				}
			}
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, list);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			Logging.Write("WATCH", $"Update sales employee - Success");
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult AddDoc(string sessionId, DataTable dt)
	{
		try
		{
			if (dt == null || dt.Rows.Count == 0)
			{
				return new HttpResult(Enums.MessageCode.DataIsEmpty, "SalesPersonsRepository->Save(): No rows to save");
			}
			string xmlObject = Mapping(dt.Rows[0], sessionId, EnumCommandType.Add);
			HttpResult httpResult = DataService.AddObject(ApzObjectTypes.oSalesPersons, sessionId, xmlObject);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult;
			}
			int num = Function.ParseInt(httpResult.Content);
			if (num > 0)
			{
				UpdateAdditionalFields(num, dt.Rows[0]);
				int empID = Function.ParseInt(dt.Rows[0]["EmpID"]);
				UpdateEmployee(num, empID);
				Logging.Write("WATCH", $"Add sales employee - {num}");
				return new HttpResult(Enums.MessageCode.Success, "", num);
			}
			return new HttpResult(Enums.MessageCode.Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override string Mapping(DataRow row, string sessionId, EnumCommandType commandType)
	{
		string text = "{\r\n";
		text += Function.GetJsonString("SalesEmployeeName", Function.ToString(row["SlpName"]));
		text += Function.GetJsonString("Remarks", Function.ToString(row["Memo"]));
		text += Function.GetJsonString("CommissionForSalesEmployee", Function.ParseDouble(row["Commission"]));
		if (Function.ParseInt(row["GroupCode"]) > 0)
		{
			text += Function.GetJsonString("CommissionGroup", Function.ParseInt(row["GroupCode"]));
		}
		text = ((!(Function.ToString(row["Active"]) == "Y")) ? (text + Function.GetJsonString("Active", "tNO")) : (text + Function.GetJsonString("Active", "tYES")));
		text += DataService.MappingUdf(row);
		return text + "}";
	}

	public override string Remove(string sessionId, int slpCode)
	{
		try
		{
			HttpResult httpResult = DataService.ProcessMutiTransaction(sessionId, new List<BodyRequestObject>
			{
				new BodyRequestObject
				{
					ObjectType = ApzObjectTypes.oSalesPersons,
					Action = Enums.HttpAction.Delete,
					Key = slpCode
				}
			});
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			Logging.Write("WATCH", $"Remove sales employee - {slpCode}");
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
