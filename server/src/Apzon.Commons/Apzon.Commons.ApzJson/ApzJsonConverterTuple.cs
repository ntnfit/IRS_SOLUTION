using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Apzon.Commons.ApzJson;

public class ApzJsonConverterTuple : JsonConverter
{
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		try
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			Type type = value.GetType();
			ApzJsonConverterDataTable apzJsonConverterDataTable = new ApzJsonConverterDataTable();
			ApzJsonConverterDataRow apzJsonConverterDataRow = new ApzJsonConverterDataRow();
			ApzJsonConverterDataSet apzJsonConverterDataSet = new ApzJsonConverterDataSet();
			writer.WriteStartObject();
			for (int i = 0; i < type.GenericTypeArguments.Length; i++)
			{
				int num = i + 1;
				if (type.GenericTypeArguments[i].Name == "DataTable")
				{
					writer.WritePropertyName("item" + num);
					DataTable value2 = type.GetProperty("Item" + num).GetValue((dynamic)value);
					apzJsonConverterDataTable.WriteJson(writer, value2, serializer);
				}
				else if (type.GenericTypeArguments[i].Name == "DataRow")
				{
					DataRow value3 = type.GetProperty("Item" + num).GetValue((dynamic)value);
					writer.WritePropertyName("item" + num);
					apzJsonConverterDataRow.WriteJson(writer, value3, serializer);
				}
				else if (type.GenericTypeArguments[i].Name == "DataSet")
				{
					writer.WritePropertyName("item" + num);
					DataSet value4 = type.GetProperty("Item" + num).GetValue((dynamic)value);
					apzJsonConverterDataSet.WriteJson(writer, value4, serializer);
				}
				else
				{
					writer.WritePropertyName("item" + num);
					dynamic value5 = type.GetProperty("Item" + num).GetValue((dynamic)value);
					serializer.Serialize(writer, value5);
				}
			}
			writer.WriteEndObject();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		try
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}
			ApzJsonConverterDataTable apzJsonConverterDataTable = new ApzJsonConverterDataTable();
			ApzJsonConverterDataRow apzJsonConverterDataRow = new ApzJsonConverterDataRow();
			ApzJsonConverterDataSet apzJsonConverterDataSet = new ApzJsonConverterDataSet();
			reader.Read();
			List<object> list = new List<object>();
			int num = 0;
			while (reader.TokenType == JsonToken.PropertyName)
			{
				Type type = objectType.GetGenericArguments()[num];
				if (type == typeof(DataTable))
				{
					list.Add(apzJsonConverterDataTable.ReadJson(reader, typeof(DataTable), null, serializer));
				}
				else if (type == typeof(DataRow))
				{
					list.Add(apzJsonConverterDataRow.ReadJson(reader, typeof(DataRow), null, serializer));
				}
				else if (type == typeof(DataSet))
				{
					reader.Read();
					list.Add(apzJsonConverterDataSet.ReadJson(reader, typeof(DataSet), null, serializer));
					reader.Read();
				}
				else
				{
					reader.Read();
					list.Add(serializer.Deserialize(reader, type));
					reader.Read();
				}
				num++;
			}
			switch (list.Count)
			{
			case 1:
				return Tuple.Create((dynamic)list[0]);
			case 2:
				return Tuple.Create((dynamic)list[0], (dynamic)list[1]);
			case 3:
				return Tuple.Create((dynamic)list[0], (dynamic)list[1], (dynamic)list[2]);
			case 4:
				return Tuple.Create((dynamic)list[0], (dynamic)list[1], (dynamic)list[2], (dynamic)list[3]);
			case 5:
				return Tuple.Create((dynamic)list[0], (dynamic)list[1], (dynamic)list[2], (dynamic)list[3], (dynamic)list[4]);
			case 6:
				return Tuple.Create((dynamic)list[0], (dynamic)list[1], (dynamic)list[2], (dynamic)list[3], (dynamic)list[4], (dynamic)list[5]);
			case 7:
				return Tuple.Create((dynamic)list[0], (dynamic)list[1], (dynamic)list[2], (dynamic)list[3], (dynamic)list[4], (dynamic)list[5], (dynamic)list[6]);
			case 8:
				return Tuple.Create((dynamic)list[0], (dynamic)list[1], (dynamic)list[2], (dynamic)list[3], (dynamic)list[4], (dynamic)list[5], (dynamic)list[6], (dynamic)list[7]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}

	public override bool CanConvert(Type objectType)
	{
		if (objectType.Name.Length > 5 && objectType.Name.Substring(0, 5).ToLower().Equals("tuple"))
		{
			return true;
		}
		return false;
	}
}
