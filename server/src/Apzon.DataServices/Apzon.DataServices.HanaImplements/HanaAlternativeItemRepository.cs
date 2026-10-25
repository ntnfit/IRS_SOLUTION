using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaAlternativeItemRepository : AlternativeItemRepository
{
	public HanaAlternativeItemRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public override async Task<string> AddNew(string sessionId, DataTable dt, string itemNo)
	{
		try
		{
			string xmlObject = Mapping(itemNo, dt);
			HttpResult httpResult = DataService.AddServiceObject(ApzObjectTypes.oAlternativeItems, sessionId, xmlObject);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override async Task<string> UpdateData(string sessionId, DataTable dt, string itemNo)
	{
		try
		{
			string xmlObject = Mapping(itemNo, dt);
			HttpResult httpResult = DataService.UpdateServiceObject(ApzObjectTypes.oAlternativeItems, sessionId, xmlObject, itemNo);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected override string Mapping(string itemNo, DataTable dt)
	{
		string text = "{\r\n";
		try
		{
			text += "\"OriginalItem\":{";
			text += Function.GetJsonString("ItemCode", itemNo);
			text += "\"AlternativeItems\":[";
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				text += "{";
				text += Function.GetJsonString("AlternativeItemCode", Function.ToString(dt.Rows[i]["AltItem"]));
				text += Function.GetJsonString("MatchFactor", Function.ToString(dt.Rows[i]["Match"]));
				if (!string.IsNullOrWhiteSpace(Function.ToString(dt.Rows[i]["Remarks"])))
				{
					text += Function.GetJsonString("Remarks", Function.ToString(dt.Rows[i]["Remarks"]));
				}
				text += "},";
			}
			text += "]";
			return text + "}}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override async Task<string> DeleteData(string sessionId, string itemNo)
	{
		try
		{
			string jsonObject = string.Format("{{\"OriginalItem\": {{\r\n        {0}\r\n    }} }}", Function.GetJsonString("ItemCode", itemNo));
			HttpResult httpResult = DataService.RemoveServiceObject(ApzObjectTypes.oAlternativeItems, sessionId, jsonObject);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
