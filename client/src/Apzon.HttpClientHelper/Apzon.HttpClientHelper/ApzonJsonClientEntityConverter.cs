using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Apzon.Commons;
using Apzon.Commons.ApzJson;
using Apzon.Entities.Models;
using Newtonsoft.Json;

namespace Apzon.HttpClientHelper;

public class ApzonJsonClientEntityConverter<T> : JsonConverter
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

	public dynamic CreateChildList()
	{
		try
		{
			return typeof(T).GetGenericArguments()[0].GetConstructor(new Type[0]).Invoke(new object[0]);
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
			if (typeof(T).Name == typeof(List<>).Name && typeof(T).IsGenericType)
			{
				return ReadJsonListEntity(reader, objectType, existingValue, serializer);
			}
			if (typeof(T) == typeof(DataTable))
			{
				return new ApzJsonConverterDataTable().ReadJson(reader, objectType, existingValue, serializer);
			}
			if (typeof(T) == typeof(DocumentTable) || typeof(T) == typeof(DocumentLineTable) || typeof(T) == typeof(DocPaymentTable))
			{
				return new ApzonJsonClientDataTableApzConverter<T>().ReadJson(reader, objectType, existingValue, serializer);
			}
			if (typeof(T) == typeof(DataSet))
			{
				return new ApzJsonConverterDataSet().ReadJson(reader, objectType, existingValue, serializer);
			}
			if (typeof(T).Name.Substring(0, 5).ToLower().Equals("tuple"))
			{
				return new ApzJsonConverterTuple().ReadJson(reader, objectType, existingValue, serializer);
			}
			dynamic val = CreateObj();
			reader.Read();
			List<string> list = (from p in typeof(T).GetProperties()
				select p.Name).ToList();
			List<string> list2 = new List<string>();
			if (reader.TokenType == JsonToken.PropertyName && reader.Value is string && (string)reader.Value == "Apz-Columns")
			{
				reader.Read();
				while (reader.TokenType == JsonToken.StartArray)
				{
					reader.Read();
					while (reader.TokenType != JsonToken.EndArray)
					{
						reader.Read();
						string text = "";
						if (reader.TokenType == JsonToken.PropertyName && reader.Value is string && (string)reader.Value == "Name")
						{
							reader.Read();
							text = reader.Value.ToString();
							reader.Read();
						}
						if (reader.TokenType == JsonToken.PropertyName && reader.Value is string && (string)reader.Value == "Type")
						{
							reader.Read();
							reader.Read();
						}
						if (reader.TokenType == JsonToken.PropertyName && reader.Value is string && (string)reader.Value == "dflt")
						{
							reader.Read();
							reader.Read();
						}
						list2.Add(list.Contains(text) ? text : "");
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
						int num = 0;
						while (reader.TokenType != JsonToken.EndArray)
						{
							if (!string.IsNullOrEmpty(list2[num]))
							{
								try
								{
									if (reader.Value != null)
									{
										PropertyInfo property = typeof(T).GetProperty(list2[num]);
										dynamic property2 = val.GetType().GetProperty(list2[num]);
										property.SetValue(val, (property.PropertyType.BaseType == typeof(Enum)) ? Enum.ToObject(property.PropertyType, reader.Value) : Convert.ChangeType(reader.Value, Nullable.GetUnderlyingType(property.PropertyType) ?? property2.PropertyType));
									}
								}
								catch (Exception ex)
								{
									Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
								}
							}
							num++;
							reader.Read();
						}
						reader.Read();
					}
					reader.Read();
				}
				reader.Read();
			}
			return val;
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
		}
		return null;
	}

	public object ReadJsonListEntity(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		try
		{
			dynamic val = CreateObj();
			reader.Read();
			List<string> list = (from p in objectType.GetGenericArguments()[0].GetProperties()
				select p.Name).ToList();
			List<string> list2 = new List<string>();
			if (reader.TokenType == JsonToken.PropertyName && reader.Value is string && (string)reader.Value == "Apz-Columns")
			{
				reader.Read();
				while (reader.TokenType == JsonToken.StartArray)
				{
					reader.Read();
					while (reader.TokenType != JsonToken.EndArray)
					{
						reader.Read();
						string text = "";
						if (reader.TokenType == JsonToken.PropertyName && reader.Value is string && (string)reader.Value == "Name")
						{
							reader.Read();
							text = reader.Value.ToString();
							reader.Read();
						}
						if (reader.TokenType == JsonToken.PropertyName && reader.Value is string && (string)reader.Value == "Type")
						{
							reader.Read();
							reader.Read();
						}
						if (reader.TokenType == JsonToken.PropertyName && reader.Value is string && (string)reader.Value == "dflt")
						{
							reader.Read();
							reader.Read();
						}
						list2.Add(list.Contains(text) ? text : "");
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
						dynamic val2 = CreateChildList();
						reader.Read();
						int num = 0;
						while (reader.TokenType != JsonToken.EndArray)
						{
							if (!string.IsNullOrEmpty(list2[num]))
							{
								try
								{
									if (reader.Value != null)
									{
										PropertyInfo property = objectType.GetGenericArguments()[0].GetProperty(list2[num]);
										dynamic property2 = val2.GetType().GetProperty(list2[num]);
										property.SetValue(val2, (property.PropertyType.BaseType == typeof(Enum)) ? Enum.ToObject(property.PropertyType, reader.Value) : Convert.ChangeType(reader.Value, Nullable.GetUnderlyingType(property.PropertyType) ?? property2.PropertyType));
									}
								}
								catch (Exception ex)
								{
									Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
								}
							}
							num++;
							reader.Read();
						}
						val.Add(val2);
						reader.Read();
					}
					reader.Read();
				}
				reader.Read();
			}
			return val;
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			return null;
		}
	}

	public override bool CanConvert(Type objectType)
	{
		return objectType == typeof(T);
	}
}
