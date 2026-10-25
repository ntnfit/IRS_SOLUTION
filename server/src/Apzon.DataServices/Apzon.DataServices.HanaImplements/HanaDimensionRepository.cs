using System;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaDimensionRepository : DimensionRepository
{
	public HanaDimensionRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public override string Mapping(DataRow row, EnumCommandType commandType)
	{
		string text = "{";
		try
		{
			text = ((!(Function.ToString(row["DimActive"]) == "Y")) ? (text + Function.GetJsonString("IsActive", "tNO")) : (text + Function.GetJsonString("IsActive", "tYES")));
			text += Function.GetJsonString("DimensionDescription", Function.ToString(row["DimDesc"]));
			text += "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	public override async Task<string> UpdateData(string sessionId, DataRow row)
	{
		try
		{
			string jsonObject = Mapping(row, EnumCommandType.Update);
			HttpResult httpResult = DataService.ProcessJson(ApzObjectTypes.oDimension, sessionId, jsonObject, Enums.HttpAction.Update, row["DimCode"]);
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
