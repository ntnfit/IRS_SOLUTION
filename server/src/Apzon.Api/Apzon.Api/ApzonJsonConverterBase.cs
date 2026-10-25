using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apzon.Api;

public class ApzonJsonConverterBase : JsonConverter
{
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		try
		{
			JToken jToken = JToken.FromObject(value);
			if (jToken.Type != JTokenType.Object)
			{
				jToken.WriteTo(writer);
				return;
			}
			JObject jObject = (JObject)jToken;
			IList<string> content = (from p in jObject.Properties()
				select p.Name).ToList();
			jObject.AddFirst(new JProperty("Keys", new JArray(content)));
			jObject.WriteTo(writer);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		return serializer.Deserialize(reader, objectType);
	}

	public override bool CanConvert(Type objectType)
	{
		if (objectType != typeof(DataTable) && objectType != typeof(DataRow))
		{
			return objectType != typeof(DataSet);
		}
		return false;
	}
}
