using System;
using System.Data;
using System.Diagnostics;
using Newtonsoft.Json;

namespace Apzon.Commons.ApzJson;

public class ApzJsonConverterDataTable : JsonConverter
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
			DataTable dataTable = (DataTable)value;
			writer.WriteStartObject();
			writer.WritePropertyName("Apz-Columns");
			writer.WriteStartArray();
			foreach (DataColumn column in dataTable.Columns)
			{
				writer.WriteStartObject();
				writer.WritePropertyName("Name");
				serializer.Serialize(writer, column.ColumnName);
				writer.WritePropertyName("Type");
				serializer.Serialize(writer, column.DataType.Name);
				writer.WritePropertyName("dflt");
				serializer.Serialize(writer, column.DefaultValue);
				writer.WriteEndObject();
			}
			writer.WriteEndArray();
			writer.WritePropertyName("Apz-Rows");
			writer.WriteStartArray();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				writer.WriteStartArray();
				for (int j = 0; j < dataTable.Columns.Count; j++)
				{
					serializer.Serialize(writer, dataTable.Rows[i][j]);
				}
				writer.WriteEndArray();
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
		try
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}
			DataTable dataTable;
			if (reader.TokenType == JsonToken.PropertyName)
			{
				dataTable = new DataTable((string)reader.Value);
				reader.Read();
			}
			else
			{
				dataTable = new DataTable();
			}
			reader.Read();
			if (reader.TokenType == JsonToken.PropertyName && reader.Value is string text && text == "Apz-Columns")
			{
				reader.Read();
				string text2 = null;
				Type type = null;
				object defaultValue = null;
				while (reader.TokenType == JsonToken.StartArray)
				{
					reader.Read();
					while (reader.TokenType != JsonToken.EndArray)
					{
						reader.Read();
						if (reader.TokenType == JsonToken.PropertyName && reader.Value is string text3 && text3 == "Name")
						{
							reader.Read();
							text2 = reader.Value.ToString();
							reader.Read();
						}
						if (reader.TokenType == JsonToken.PropertyName && reader.Value is string text4 && text4 == "Type")
						{
							reader.Read();
							type = Function.GetColumnDataType(reader.Value.ToString());
							reader.Read();
						}
						if (reader.TokenType == JsonToken.PropertyName && reader.Value is string text5 && text5 == "dflt")
						{
							reader.Read();
							defaultValue = reader.Value;
							reader.Read();
						}
						if (text2 == null || type == null)
						{
							throw new JsonSerializationException("Invalid DataTable column definition.");
						}
						dataTable.Columns.Add(new DataColumn
						{
							ColumnName = text2,
							DataType = type,
							DefaultValue = defaultValue
						});
						reader.Read();
					}
					reader.Read();
				}
			}
			if (reader.TokenType == JsonToken.PropertyName && reader.Value is string text6 && text6 == "Apz-Rows")
			{
				reader.Read();
				if (reader.TokenType == JsonToken.StartArray)
				{
					reader.Read();
					while (reader.TokenType == JsonToken.StartArray)
					{
						reader.Read();
						DataRow dataRow = dataTable.NewRow();
						int num = 0;
						while (reader.TokenType != JsonToken.EndArray)
						{
							dataRow[num] = ((reader.Value != null) ? (serializer.Deserialize(reader, dataTable.Columns[num].DataType) ?? DBNull.Value) : DBNull.Value);
							num++;
							reader.Read();
						}
						dataRow.EndEdit();
						dataTable.Rows.Add(dataRow);
						reader.Read();
					}
					reader.Read();
				}
				reader.Read();
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}

	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(DataTable);
	}
}
