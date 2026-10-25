using System;
using System.Collections.Generic;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Newtonsoft.Json;

namespace Apzon.Api.JsonFormatter;

public class ApzJsonConverterListWorkFlowProcess : JsonConverter
{
	public override bool CanConvert(Type objectType)
	{
		return true;
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		List<IWorkflowProcess> list = new List<IWorkflowProcess>();
		ApzJsonConverterIWorkFlowProcess apzJsonConverterIWorkFlowProcess = new ApzJsonConverterIWorkFlowProcess();
		try
		{
			while (reader.TokenType != JsonToken.EndArray && reader.TokenType != JsonToken.None)
			{
				reader.Read();
				list.Add((IWorkflowProcess)apzJsonConverterIWorkFlowProcess.ReadJson(reader, objectType, null, serializer));
				reader.Read();
			}
			return list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		try
		{
			string json = JsonConvert.SerializeObject(value);
			writer.WriteRaw(json);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}
}
