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

public class HanaCurrenciesRepository(Apzon.Services.IUnitOfWork serviceUOW, IUnitOfWork unitOfWork, IDataService dataService) : CurrenciesRepository(serviceUOW, unitOfWork, dataService)
{
	private string _tableName = "OCRN";

	private ApzObjectTypes _objType = ApzObjectTypes.oCurrencyCodes;

	public override bool CheckExists(string currCode, string sessionId)
	{
		string query = "select 1 from OCRN where \"CurrCode\"=@currCode;";
		try
		{
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@currCode", currCode));
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
			text += Function.GetJsonString("Code", doc["CurrCode"]);
			text += Function.GetJsonString("Name", doc["CurrName"]);
			text += Function.GetJsonString("DocumentsCode", doc["DocCurrCod"]);
			text += Function.GetJsonString("InternationalDescription", doc["ChkName"]);
			text += Function.GetJsonString("HundredthName", doc["Chk100Name"]);
			text += Function.GetJsonString("EnglishName", doc["FrgnName"]);
			text += Function.GetJsonString("EnglishHundredthName", doc["F100Name"]);
			text += Function.GetJsonString("PluralInternationalDescription", "");
			text += Function.GetJsonString("PluralHundredthName", "");
			text += Function.GetJsonString("PluralEnglishName", "");
			text += Function.GetJsonString("PluralEnglishHundredthName", "");
			text = Function.ParseInt(doc["Decimals"]) switch
			{
				-1 => text + Function.GetJsonString("Decimals", "cdDefault"), 
				0 => text + Function.GetJsonString("Decimals", "cdWithoutDecimals"), 
				1 => text + Function.GetJsonString("Decimals", "cd1Digit"), 
				2 => text + Function.GetJsonString("Decimals", "cd2Digits"), 
				3 => text + Function.GetJsonString("Decimals", "cd3Digits"), 
				4 => text + Function.GetJsonString("Decimals", "cd4Digits"), 
				5 => text + Function.GetJsonString("Decimals", "cd5Digits"), 
				6 => text + Function.GetJsonString("Decimals", "cd6Digits"), 
				_ => text + Function.GetJsonString("Decimals", ""), 
			};
			text = Function.ParseInt(doc["RoundSys"]) switch
			{
				0 => text + Function.GetJsonString("Rounding", "rsNoRounding"), 
				1 => text + Function.GetJsonString("Rounding", "rsRoundToTenHundredth"), 
				2 => text + Function.GetJsonString("Rounding", "rsRoundToOne"), 
				3 => text + Function.GetJsonString("Rounding", "rsRoundToTen"), 
				4 => text + Function.GetJsonString("Rounding", "rsRoundToFiveHundredth"), 
				_ => text + Function.GetJsonString("Rounding", ""), 
			};
			text = ((!(Function.ToString(doc["RoundPym"]) == "N")) ? (text + Function.GetJsonString("RoundingInPayment", "tYES")) : (text + Function.GetJsonString("RoundingInPayment", "tNO")));
			text += Function.GetJsonString("MaxIncomingAmtDiff", Function.ParseDouble(doc["MaxInDiff"]));
			text += Function.GetJsonString("MaxOutgoingAmtDiff", Function.ParseDouble(doc["MaxOutDiff"]));
			text += Function.GetJsonString("MaxIncomingAmtDiffPercent", Function.ParseDouble(doc["MaxInPcnt"]));
			text += Function.GetJsonString("MaxOutgoingAmtDiffPercent", Function.ParseDouble(doc["MaxOutPcnt"]));
			return text + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return "";
		}
	}

	public override HttpResult Save(string sessionId, DataTable dt)
	{
		string text = "";
		List<BodyRequestObject> list = new List<BodyRequestObject>();
		try
		{
			foreach (DataRow row in dt.Rows)
			{
				if (CheckExists(Function.ToString(row["CurrCode"]), sessionId))
				{
					text = Mapping(row, sessionId);
					list.Add(new BodyRequestObject
					{
						BodyObject = text,
						Action = Enums.HttpAction.Update,
						ObjectType = ApzObjectTypes.oCurrencyCodes,
						Key = row["CurrCode"],
						OutObjectKey = true
					});
				}
				else
				{
					text = Mapping(row, sessionId);
					list.Add(new BodyRequestObject
					{
						Action = Enums.HttpAction.AddNew,
						ObjectType = ApzObjectTypes.oCurrencyCodes,
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

	public override HttpResult Remove(string currCode, string sessionId)
	{
		List<BodyRequestObject> list = new List<BodyRequestObject>();
		try
		{
			list.Add(new BodyRequestObject
			{
				Action = Enums.HttpAction.Delete,
				ObjectType = ApzObjectTypes.oCurrencyCodes,
				Key = Function.ToString(currCode),
				OutObjectKey = true
			});
			return DataService.ProcessMutiTransaction(sessionId, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public new void Dispose()
	{
	}
}
