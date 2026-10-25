using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons.Helper;
using Newtonsoft.Json;

namespace Apzon.Commons.ApzJson;

public class ApzJsonClientObjectToDataTable : JsonConverter
{
	public dynamic CreateObj()
	{
		return new DataTable();
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		serializer.Serialize(writer, value);
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		try
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}
			DataTable dataTable = new DataTable();
			if (reader.TokenType == JsonToken.None)
			{
				Logging.Write("TRACE", "Cannot read json");
				return null;
			}
			if (reader.TokenType == JsonToken.StartObject)
			{
				reader.Read();
				while (reader.TokenType != JsonToken.EndObject)
				{
					while (reader.TokenType == JsonToken.PropertyName)
					{
						object value = reader.Value;
						reader.Read();
						if (reader.TokenType == JsonToken.StartArray)
						{
							DataTable dataTable2 = ReadArraytoTable(reader, objectType, existingValue, serializer);
							if (!dataTable2.IsNotNull())
							{
								continue;
							}
							DataTable dataTable3 = dataTable.Clone();
							for (int i = 0; i < dataTable2.Columns.Count; i++)
							{
								dataTable3.Columns.Add(value.ToString() + "." + dataTable2.Columns[i].ColumnName);
							}
							if (dataTable.IsNotNull())
							{
								for (int j = 0; j < dataTable.Rows.Count; j++)
								{
									for (int k = 0; k < dataTable2.Rows.Count; k++)
									{
										DataRow dataRow = dataTable3.NewRow();
										List<object> list = dataTable.Rows[j].ItemArray.ToList();
										list.AddRange(dataTable2.Rows[k].ItemArray);
										dataRow.ItemArray = list.ToArray();
										dataTable3.Rows.Add(dataRow);
									}
								}
							}
							else if (dataTable.Columns.Count > 0)
							{
								for (int l = 0; l < dataTable2.Rows.Count; l++)
								{
									DataRow dataRow2 = dataTable3.NewRow();
									List<object> list2 = dataTable.NewRow().ItemArray.ToList();
									list2.AddRange(dataTable2.Rows[l].ItemArray);
									dataRow2.ItemArray = list2.ToArray();
									dataTable3.Rows.Add(dataRow2);
								}
							}
							else
							{
								for (int m = 0; m < dataTable2.Rows.Count; m++)
								{
									DataRow dataRow3 = dataTable3.NewRow();
									dataRow3.ItemArray = dataTable2.Rows[m].ItemArray;
									dataTable3.Rows.Add(dataRow3);
								}
							}
							dataTable = dataTable3;
							dataTable.AcceptChanges();
							continue;
						}
						if (reader.TokenType == JsonToken.StartObject)
						{
							Function.ToString(value);
							DataTable dataTable4 = ReadJson(reader, objectType, existingValue, serializer) as DataTable;
							if (!dataTable4.IsNotNull() || !dataTable4.IsNotNull())
							{
								continue;
							}
							if (dataTable.IsNotNull())
							{
								DataTable dataTable5 = dataTable.Clone();
								for (int n = 0; n < dataTable4.Columns.Count; n++)
								{
									dataTable5.Columns.Add(value.ToString() + "." + dataTable4.Columns[n].ColumnName);
								}
								for (int num = 0; num < dataTable.Rows.Count; num++)
								{
									for (int num2 = 0; num2 < dataTable4.Rows.Count; num2++)
									{
										DataRow dataRow4 = dataTable5.NewRow();
										List<object> list3 = dataTable.Rows[num].ItemArray.ToList();
										list3.AddRange(dataTable4.Rows[num2].ItemArray);
										dataRow4.ItemArray = list3.ToArray();
										dataTable5.Rows.Add(dataRow4);
									}
								}
								dataTable = dataTable5;
							}
							else
							{
								dataTable = dataTable4;
							}
							continue;
						}
						object value2 = reader.Value;
						_ = reader.Path;
						if (!dataTable.Columns.Contains(value.ToString()))
						{
							dataTable.Columns.Add(value.ToString(), GetColumnDataType(reader));
						}
						if (dataTable.Rows.Count > 0)
						{
							for (int num3 = 0; num3 < dataTable.Rows.Count; num3++)
							{
								dataTable.Rows[num3][value.ToString()] = value2;
							}
						}
						else
						{
							DataRow dataRow5 = dataTable.NewRow();
							dataRow5[value.ToString()] = value2;
							dataTable.Rows.Add(dataRow5);
						}
						reader.Read();
					}
					if (reader.TokenType == JsonToken.EndObject)
					{
						reader.Read();
						return dataTable;
					}
					reader.Read();
				}
				return dataTable;
			}
			if (reader.TokenType == JsonToken.StartArray)
			{
				return ReadArraytoTable(reader, objectType, existingValue, serializer);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}

	private DataTable ReadArraytoTable(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		bool flag = true;
		DataTable dataTable = new DataTable();
		reader.Read();
		while (reader.TokenType != JsonToken.EndArray)
		{
			DataTable dataTable2 = ReadJson(reader, objectType, existingValue, serializer) as DataTable;
			if (reader.TokenType == JsonToken.EndObject)
			{
				reader.Read();
			}
			if (flag)
			{
				dataTable = dataTable2;
				flag = false;
			}
			else
			{
				if (!dataTable2.IsNotNull())
				{
					continue;
				}
				foreach (DataRow row in dataTable2.Rows)
				{
					dataTable.Rows.Add(row.ItemArray);
				}
			}
		}
		dataTable.AcceptChanges();
		return dataTable;
	}

	private Type GetColumnDataType(JsonReader reader)
	{
		JsonToken tokenType = reader.TokenType;
		if ((uint)(tokenType - 7) <= 3u || (uint)(tokenType - 16) <= 1u)
		{
			return reader.ValueType;
		}
		return typeof(string);
	}

	public override bool CanConvert(Type objectType)
	{
		return true;
	}
}
