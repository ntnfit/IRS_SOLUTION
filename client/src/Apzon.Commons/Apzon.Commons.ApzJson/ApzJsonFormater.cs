using System;
using System.Data;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Apzon.Commons.Helper;
using Newtonsoft.Json;

namespace Apzon.Commons.ApzJson;

public class ApzJsonFormater : MediaTypeFormatter
{
	public ApzJsonFormater()
	{
		SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
	}

	public override bool CanWriteType(Type type)
	{
		if (type == typeof(JsonValue) || type == typeof(JsonObject) || type == typeof(JsonArray))
		{
			return false;
		}
		return true;
	}

	public override bool CanReadType(Type type)
	{
		if (type.Name.ToLower().Contains("workflowprocess") || (type.Name.ToLower().Contains("list") && type.GetGenericArguments().Any((Type x) => x.Name.ToLower().Contains("workflowprocess"))))
		{
			return false;
		}
		return true;
	}

	public override Task<object> ReadFromStreamAsync(Type type, Stream stream, HttpContent contentHeaders, IFormatterLogger formatterLogger)
	{
		return Task<object>.Factory.StartNew(() =>
		{
			if (type == typeof(DataTable) || type == typeof(DataSet) || (type.Name.Length > 5 && type.Name.Substring(0, 5).ToLower().Equals("tuple")) || type == typeof(DataRow))
			{
				string text = (contentHeaders.Headers.ContentEncoding.ToList().Exists((string t) => t.Equals("deflate")) ? CompressionHelper.UndeflateByte(contentHeaders.ReadAsByteArrayAsync().Result).ConvertByteToString() : ((!contentHeaders.Headers.ContentEncoding.ToList().Exists((string t) => t.Equals("deflate-Gzip"))) ? contentHeaders.ReadAsStringAsync().Result : JsoncCompressionHelper.Decompress(contentHeaders.ReadAsByteArrayAsync().Result).ConvertByteToString()));
				if (type == typeof(DataTable))
				{
					if (!text.Contains("Apz-Columns"))
					{
						return JsonConvert.DeserializeObject<DataTable>(text);
					}
					return JsonConvert.DeserializeObject<DataTable>(text, new JsonConverter[2]
					{
						new ApzJsonConverterDataTable(),
						new TrimmingConverter()
					});
				}
				if (type == typeof(DataSet))
				{
					if (!text.Contains("Apz-Columns"))
					{
						return JsonConvert.DeserializeObject<DataSet>(text);
					}
					return JsonConvert.DeserializeObject<DataSet>(text, new JsonConverter[3]
					{
						new ApzJsonConverterDataTable(),
						new ApzJsonConverterDataSet(),
						new TrimmingConverter()
					});
				}
				if (type == typeof(DataRow))
				{
					if (!text.Contains("Apz-Columns"))
					{
						return JsonConvert.DeserializeObject<DataRow>(text);
					}
					return JsonConvert.DeserializeObject<DataRow>(text, new JsonConverter[2]
					{
						new ApzJsonConverterDataRow(),
						new TrimmingConverter()
					});
				}
				if (type.Name.Length > 5 && type.Name.Substring(0, 5).ToLower().Equals("tuple"))
				{
					return JsonConvert.DeserializeObject(text, type, new ApzJsonConverterTuple(), new TrimmingConverter());
				}
				return JsonConvert.DeserializeObject(text, type, new JsonSerializerSettings());
			}
			return new JsonMediaTypeFormatter
			{
				SerializerSettings = 
				{
					Converters = { (JsonConverter)new TrimmingConverter() }
				}
			}.ReadFromStreamAsync(type, stream, contentHeaders, formatterLogger).Result;
		});
	}

	public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContent contentHeaders, TransportContext transportContext)
	{
		if (value != null)
		{
			type = value.GetType();
		}
		return Task.Factory.StartNew(() =>
		{
			string s = ((type.Name.Length > 5 && type.Name.Substring(0, 5).ToLower().Equals("tuple")) ? JsonConvert.SerializeObject(value, new ApzJsonConverterTuple()) : ((type == typeof(DataRow)) ? JsonConvert.SerializeObject(value, new ApzJsonConverterDataRow()) : ((type == typeof(DataTable)) ? JsonConvert.SerializeObject(value, new ApzJsonConverterDataTable()) : ((!(type == typeof(DataSet))) ? JsonConvert.SerializeObject(value, type, Formatting.Indented, new JsonSerializerSettings()) : JsonConvert.SerializeObject(value, new ApzJsonConverterDataTable(), new ApzJsonConverterDataSet())))));
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			stream.Write(bytes, 0, bytes.Length);
			stream.Flush();
		});
	}
}
