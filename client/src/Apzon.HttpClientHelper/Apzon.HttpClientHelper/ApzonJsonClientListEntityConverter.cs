using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Apzon.Commons;
using Newtonsoft.Json;

namespace Apzon.HttpClientHelper;

public class ApzonJsonClientListEntityConverter<T> : JsonConverter
{
	public List<T> CreateListObj()
	{
		try
		{
			return (List<T>)typeof(List<T>).GetConstructor(new Type[0]).Invoke(new object[0]);
		}
		catch
		{
			return null;
		}
	}

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
			List<T> list = CreateListObj();
			reader.Read();
			List<string> list2 = (from p in typeof(T).GetProperties()
				select p.Name).ToList();
			List<string> list3 = new List<string>();
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
						list3.Add(list2.Contains(text) ? text : "");
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
						dynamic val = CreateObj();
						int num = 0;
						while (reader.TokenType != JsonToken.EndArray)
						{
							if (!string.IsNullOrEmpty(list3[num]))
							{
								try
								{
									if (reader.Value != null)
									{
										PropertyInfo property = typeof(T).GetProperty(list3[num]);
										dynamic property2 = val.GetType().GetProperty(list3[num]);
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
						list.Add(val);
						reader.Read();
					}
					reader.Read();
				}
				reader.Read();
			}
			return list;
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
		}
		return null;
	}

	public override bool CanConvert(Type objectType)
	{
		return true;
	}
}
