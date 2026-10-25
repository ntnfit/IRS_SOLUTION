using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models;
using Newtonsoft.Json;

namespace Apzon.HttpClientHelper;

public class ApzonJsonClientDataTableApzConverter<T> : JsonConverter
{
	public dynamic CreateObj()
	{
		try
		{
			return (T)typeof(T).GetConstructor(new Type[0]).Invoke(new object[0]);
		}
		catch
		{
			return default(T);
		}
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		serializer.Serialize(writer, value);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		try
		{
			dynamic val = CreateObj();
			List<string> list = new List<string>();
			if (reader.TokenType == JsonToken.PropertyName)
			{
				val = new DataTable((string)reader.Value);
				reader.Read();
			}
			reader.Read();
			if (reader.TokenType == JsonToken.PropertyName && reader.Value is string && (string)reader.Value == "Apz-Columns")
			{
				reader.Read();
				string text = null;
				Type type = null;
				object defaultValue = null;
				while (reader.TokenType == JsonToken.StartArray)
				{
					reader.Read();
					while (reader.TokenType != JsonToken.EndArray)
					{
						reader.Read();
						if (reader.TokenType == JsonToken.PropertyName && reader.Value is string && (string)reader.Value == "Name")
						{
							reader.Read();
							text = reader.Value.ToString();
							reader.Read();
						}
						if (reader.TokenType == JsonToken.PropertyName && reader.Value is string && (string)reader.Value == "Type")
						{
							reader.Read();
							type = Function.GetColumnDataType(reader.Value.ToString());
							reader.Read();
						}
						if (reader.TokenType == JsonToken.PropertyName && reader.Value is string && (string)reader.Value == "dflt")
						{
							reader.Read();
							defaultValue = reader.Value;
							reader.Read();
						}
						if (text == null || type == null)
						{
							throw new JsonSerializationException("Invalid DataTable column definition.");
						}
						val.Columns.Add(new DataColumn
						{
							ColumnName = text,
							DataType = type,
							DefaultValue = defaultValue
						});
						list.Add(text);
						reader.Read();
					}
					reader.Read();
				}
			}
			if (reader.TokenType == JsonToken.PropertyName && reader.Value is string && (string)reader.Value == "Apz-Rows")
			{
				reader.Read();
				if (reader.TokenType == JsonToken.StartArray)
				{
					reader.Read();
					while (reader.TokenType == JsonToken.StartArray)
					{
						reader.Read();
						dynamic newRow = val.GetNewRow();
						int num = 0;
						while (reader.TokenType != JsonToken.EndArray)
						{
							newRow[list[num]] = ((reader.Value != null) ? (serializer.Deserialize(reader, val.Columns[num].DataType) ?? DBNull.Value) : DBNull.Value);
							num++;
							reader.Read();
						}
						newRow.EndEdit();
						val.Add(newRow);
						reader.Read();
					}
					reader.Read();
				}
				reader.Read();
			}
			return val;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}

	public override bool CanConvert(Type objectType)
	{
		if (!(objectType == typeof(DocumentTable)) && !(objectType == typeof(DocumentLineTable)))
		{
			return objectType == typeof(DocPaymentTable);
		}
		return true;
	}
}
