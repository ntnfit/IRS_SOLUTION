using System;
using System.Data;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Apzon.Commons.ApzJson;

public class ApzJsonConverterDataRow : JsonConverter
{
	private object ChangeNullData(Type type)
	{
		try
		{
			if (type == typeof(short) || type == typeof(int) || type == typeof(long))
			{
				return 0;
			}
			if (type == typeof(double) || type == typeof(long) || type == typeof(decimal) || type == typeof(ushort) || type == typeof(uint) || type == typeof(ulong))
			{
				return 0.0;
			}
			if (type == typeof(DateTime))
			{
				return DateTime.MinValue;
			}
			if (type == typeof(TimeSpan))
			{
				return new TimeSpan(0, 0, 0);
			}
			if (type == typeof(bool))
			{
				return false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		try
		{
			DataRow dataRow = (DataRow)value;
			writer.WriteStartObject();
			writer.WritePropertyName("Apz-Columns");
			writer.WriteStartArray();
			foreach (DataColumn column in dataRow.Table.Columns)
			{
				writer.WriteStartObject();
				writer.WritePropertyName("Name");
				serializer.Serialize(writer, column.ColumnName);
				writer.WritePropertyName("Type");
				serializer.Serialize(writer, column.DataType.Name);
				writer.WritePropertyName("dflt");
				serializer.Serialize(writer, column.DefaultValue);
				writer.WritePropertyName("Value");
				serializer.Serialize(writer, dataRow[column]);
				writer.WriteEndObject();
			}
			writer.WriteEndArray();
			writer.WriteEndObject();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		DataTable dataTable = new DataTable();
		DataRow dataRow = dataTable.NewRow();
		reader.Read();
		reader.Read();
		if (reader.TokenType == JsonToken.PropertyName && reader.Value is string && (string)reader.Value == "Apz-Columns")
		{
			reader.Read();
			while (reader.TokenType == JsonToken.StartArray)
			{
				reader.Read();
				while (reader.TokenType != JsonToken.EndArray)
				{
					reader.Read();
					string text = null;
					Type type = null;
					object defaultValue = null;
					object value = null;
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
					if (reader.TokenType == JsonToken.PropertyName && reader.Value is string && (string)reader.Value == "Value")
					{
						reader.Read();
						value = ((reader.Value != null) ? (serializer.Deserialize(reader, type) ?? DBNull.Value) : DBNull.Value);
						reader.Read();
					}
					if (text == null || type == null)
					{
						throw new JsonSerializationException("Invalid DataTable column definition.");
					}
					dataTable.Columns.Add(new DataColumn
					{
						ColumnName = text,
						DataType = type,
						DefaultValue = defaultValue
					});
					dataRow[text] = value;
					reader.Read();
				}
				reader.Read();
			}
			reader.Read();
			return dataRow;
		}
		return null;
	}

	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(DataRow);
	}
}
