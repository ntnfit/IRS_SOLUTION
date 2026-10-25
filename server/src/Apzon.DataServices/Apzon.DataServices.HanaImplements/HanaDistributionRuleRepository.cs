using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.DataServices.Implements;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaDistributionRuleRepository : DistributionRuleRepository
{
	public HanaDistributionRuleRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	protected override async Task<string> UpdateData(string sessionId, DataSet ds)
	{
		try
		{
			string xmlObject = Mapping(ds);
			return DataService.UpdateObject(ApzObjectTypes.oDistributionRule, sessionId, xmlObject).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected override async Task<string> AddNew(string sessionId, DataSet ds)
	{
		try
		{
			string xmlObject = Mapping(ds);
			return DataService.AddObject(ApzObjectTypes.oDistributionRule, sessionId, xmlObject).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string Mapping(DataSet ds)
	{
		string text = "{";
		try
		{
			DataRow dataRow = ds.Tables[0].Rows[0];
			DataTable dataTable = ds.Tables[1];
			text += Function.GetJsonString("FactorCode", Function.ToString(dataRow["OcrCode"]));
			text = (string.IsNullOrEmpty(Function.ToString(dataRow["OcrName"])) ? (text + Function.GetJsonString("FactorDescription", null)) : (text + Function.GetJsonString("FactorDescription", Function.ToString(dataRow["OcrName"]))));
			text += Function.GetJsonString("TotalFactor", Function.ParseDouble(dataRow["OcrTotal"]));
			text = ((!string.IsNullOrEmpty(Function.ToString(dataRow["Direct"])) && !(Function.ToString(dataRow["Direct"]) == "N")) ? (text + Function.GetJsonString("Direct", "Y")) : (text + Function.GetJsonString("Direct", "N")));
			text += Function.GetJsonString("InWhichDimension", Function.ParseShort(dataRow["DimCode"]));
			text = ((!string.IsNullOrEmpty(Function.ToString(dataRow["Active"])) && !(Function.ToString(dataRow["Active"]) == "N")) ? (text + Function.GetJsonString("Active", "tYES")) : (text + Function.GetJsonString("Active", "tNO")));
			text = ((!string.IsNullOrEmpty(Function.ToString(dataRow["IsFixedAmt"])) && !(Function.ToString(dataRow["IsFixedAmt"]) == "N")) ? (text + Function.GetJsonString("IsFixedAmount", "tYES")) : (text + Function.GetJsonString("IsFixedAmount", "tNO")));
			text += "\"DistributionRuleLines\":[";
			for (int i = 0; i < dataTable.Rows.Count; text += "},", i++)
			{
				text += "{";
				DataRow dataRow2 = dataTable.Rows[i];
				text = (string.IsNullOrEmpty(Function.ToString(dataRow2["PrcCode"])) ? (text + Function.GetJsonString("CenterCode", null)) : (text + Function.GetJsonString("CenterCode", Function.ToString(dataRow2["PrcCode"]))));
				text += Function.GetJsonString("TotalInCenter", Function.ParseDouble(dataRow2["PrcAmount"]));
				DateTime? dateTime = Function.ParseDateTime(dataRow2["ValidFrom"]);
				if (dateTime.HasValue)
				{
					_ = dateTime.Value;
					if (dateTime.Value.Year >= 1990)
					{
						text += Function.GetJsonString("EffectiveFrom", dateTime.Value.ToString("yyyy-MM-dd"));
						goto IL_0323;
					}
				}
				text += Function.GetJsonString("EffectiveFrom", null);
				goto IL_0323;
				IL_0323:
				DateTime? dateTime2 = Function.ParseDateTime(dataRow2["ValidTo"]);
				if (dateTime2.HasValue)
				{
					_ = dateTime2.Value;
					if (dateTime2.Value.Year >= 1990)
					{
						text += Function.GetJsonString("EffectiveTo", dateTime2.Value.ToString("yyyy-MM-dd"));
						continue;
					}
				}
				text += Function.GetJsonString("EffectiveTo", null);
			}
			text += "]";
			text += "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	public override async Task<string> DeleteDistributeRule(string sessionId, string ocrCode)
	{
		try
		{
			return DataService.RemoveObject(ApzObjectTypes.oDistributionRule, sessionId, ocrCode).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
