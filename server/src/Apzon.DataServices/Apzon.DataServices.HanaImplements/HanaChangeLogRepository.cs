using System;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;
using Newtonsoft.Json;

namespace Apzon.DataServices.HanaImplements;

public class HanaChangeLogRepository : ChangeLogRepository
{
	public HanaChangeLogRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public override DataTable GetChangeLog(GetChangeLogParams pr, string sessionId)
	{
		try
		{
			string bodyString = string.Format("{{\r\n    \"GetChangeLogParams\": {{\r\n        \"Object\": \"{1}\",\r\n        \"PrimaryKey\": \"{0}\",\r\n        \"UDOObjectCode\": \"{2}\"\r\n    }}\r\n    }}", pr.PrimaryKey, pr.Object, pr.UDOObjectCode);
			HttpResult dataObjectFromService = DataService.GetDataObjectFromService(ApzObjectTypes.oChangeLog, sessionId, Enums.HttpAction.GetChangeLog, null, bodyString, isServiceAction: true);
			if (dataObjectFromService != null)
			{
				if (!string.IsNullOrEmpty(dataObjectFromService.Message))
				{
					Logging.Write("ERROR", dataObjectFromService.Message);
				}
				else
				{
					dynamic val = JsonConvert.DeserializeObject(Function.ToString(dataObjectFromService.Content));
					dynamic val2 = Function.ToString(val.value);
					if ((!string.IsNullOrEmpty(val2)))
					{
						return JsonConvert.DeserializeObject<DataTable>(val2);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public override DataTable GetChangeLogDifferences(GetChangeLogParams pr, string sessionId)
	{
		try
		{
			string bodyString = $"{{\r\n    \"ShowDifferenceParams\": {{\r\n        \"LogInstance\": {pr.LogInstance},\r\n        \"LogInstance2\": {pr.LogInstance2},\r\n        \"Object\": \"{pr.Object}\",\r\n        \"PrimaryKey\": \"{pr.PrimaryKey}\",\r\n        \"UDOObjectCode\": \"{pr.UDOObjectCode}\"\r\n    }}\r\n    }}";
			HttpResult dataObjectFromService = DataService.GetDataObjectFromService(ApzObjectTypes.oChangeLog, sessionId, Enums.HttpAction.GetChangeLogDiff, null, bodyString, isServiceAction: true);
			if (dataObjectFromService != null)
			{
				if (!string.IsNullOrEmpty(dataObjectFromService.Message))
				{
					Logging.Write("ERROR", dataObjectFromService.Message);
				}
				else
				{
					dynamic val = JsonConvert.DeserializeObject(Function.ToString(dataObjectFromService.Content));
					dynamic val2 = Function.ToString(val.value);
					if ((!string.IsNullOrEmpty(val2)))
					{
						return JsonConvert.DeserializeObject<DataTable>(val2);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}
}
