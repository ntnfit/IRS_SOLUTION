using System;
using System.Data;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Apzon.Commons.ApzJson;

public class ApzJsonConverterDataSet : JsonConverter
{
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		if (value == null)
		{
			writer.WriteNull();
			return;
		}
		DataSet dataSet = (DataSet)value;
		ApzJsonConverterDataTable apzJsonConverterDataTable = new ApzJsonConverterDataTable();
		writer.WriteStartObject();
		foreach (DataTable table in dataSet.Tables)
		{
			writer.WritePropertyName(table.TableName);
			apzJsonConverterDataTable.WriteJson(writer, table, serializer);
		}
		writer.WriteEndObject();
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		try
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}
			DataSet dataSet = new DataSet();
			ApzJsonConverterDataTable apzJsonConverterDataTable = new ApzJsonConverterDataTable();
			reader.Read();
			while (reader.TokenType == JsonToken.PropertyName)
			{
				DataTable table = (DataTable)apzJsonConverterDataTable.ReadJson(reader, typeof(DataTable), null, serializer);
				dataSet.Tables.Add(table);
			}
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}

	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(DataSet);
	}
}
