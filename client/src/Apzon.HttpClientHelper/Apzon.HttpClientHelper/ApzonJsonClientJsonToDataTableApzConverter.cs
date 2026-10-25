using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Newtonsoft.Json;

namespace Apzon.HttpClientHelper;

public class ApzonJsonClientJsonToDataTableApzConverter<T> : JsonConverter
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
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}
			DataTable dataTable = new DataTable();
			while (reader.TokenType != JsonToken.EndObject && reader.TokenType != JsonToken.None)
			{
				if (reader.TokenType == JsonToken.StartObject)
				{
					reader.Read();
				}
				while (reader.TokenType == JsonToken.PropertyName)
				{
					object value = reader.Value;
					reader.Read();
					if (reader.TokenType == JsonToken.StartArray)
					{
						bool flag = true;
						DataTable dataTable2 = new DataTable();
						while (reader.TokenType != JsonToken.EndArray && reader.TokenType != JsonToken.None)
						{
							reader.Read();
							if (reader.TokenType == JsonToken.EndArray)
							{
								break;
							}
							DataTable dataTable3 = ReadJson(reader, objectType, existingValue, serializer) as DataTable;
							if (flag)
							{
								dataTable2 = dataTable3;
								flag = false;
							}
							else
							{
								if (!dataTable3.IsNotNull())
								{
									continue;
								}
								foreach (DataRow row in dataTable3.Rows)
								{
									dataTable2.Rows.Add(row.ItemArray);
								}
							}
						}
						if (!dataTable2.IsNotNull())
						{
							continue;
						}
						DataTable dataTable4 = dataTable.Clone();
						for (int i = 0; i < dataTable2.Columns.Count; i++)
						{
							dataTable4.Columns.Add(value.ToString() + "_" + dataTable2.Columns[i].ColumnName);
						}
						for (int j = 0; j < dataTable.Rows.Count; j++)
						{
							for (int k = 0; k < dataTable2.Rows.Count; k++)
							{
								DataRow dataRow2 = dataTable4.NewRow();
								List<object> list = dataTable.Rows[j].ItemArray.ToList();
								list.AddRange(dataTable2.Rows[k].ItemArray);
								dataRow2.ItemArray = list.ToArray();
								dataTable4.Rows.Add(dataRow2);
							}
						}
						dataTable = dataTable4;
						continue;
					}
					if (reader.TokenType == JsonToken.StartObject)
					{
						Function.ToString(value);
						DataTable dataTable5 = ReadJson(reader, objectType, existingValue, serializer) as DataTable;
						if (!dataTable5.IsNotNull())
						{
							continue;
						}
						DataTable dataTable6 = dataTable.Clone();
						for (int l = 0; l < dataTable5.Columns.Count; l++)
						{
							dataTable6.Columns.Add(value.ToString() + "_" + dataTable5.Columns[l].ColumnName);
						}
						for (int m = 0; m < dataTable.Rows.Count; m++)
						{
							for (int n = 0; n < dataTable5.Rows.Count; n++)
							{
								DataRow dataRow3 = dataTable6.NewRow();
								List<object> list2 = dataTable.Rows[m].ItemArray.ToList();
								list2.AddRange(dataTable5.Rows[n].ItemArray);
								dataRow3.ItemArray = list2.ToArray();
								dataTable6.Rows.Add(dataRow3);
							}
						}
						dataTable = dataTable6;
						continue;
					}
					object value2 = reader.Value;
					_ = reader.Path;
					if (!dataTable.Columns.Contains(value.ToString()))
					{
						dataTable.Columns.Add(value.ToString());
					}
					if (dataTable.Rows.Count > 0)
					{
						for (int num = 0; num < dataTable.Rows.Count; num++)
						{
							dataTable.Rows[num][value.ToString()] = value2;
						}
					}
					else
					{
						DataRow dataRow4 = dataTable.NewRow();
						dataRow4[value.ToString()] = value2;
						dataTable.Rows.Add(dataRow4);
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
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}

	public override bool CanConvert(Type objectType)
	{
		return true;
	}
}
