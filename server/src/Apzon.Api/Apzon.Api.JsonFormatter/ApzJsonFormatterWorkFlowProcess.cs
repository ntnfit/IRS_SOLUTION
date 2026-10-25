using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Newtonsoft.Json;

namespace Apzon.Api.JsonFormatter;

public class ApzJsonFormatterWorkFlowProcess : MediaTypeFormatter
{
	public ApzJsonFormatterWorkFlowProcess()
	{
		SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
	}

	public override bool CanReadType(Type type)
	{
		if (type.Name.ToLower().Contains("workflowprocess") || (type.Name.ToLower().Contains("list") && type.GetGenericArguments().Any((Type x) => x.Name.ToLower().Contains("workflowprocess"))))
		{
			return true;
		}
		return false;
	}

	public override bool CanWriteType(Type type)
	{
		if (type == typeof(JsonValue) || type == typeof(JsonObject) || type == typeof(JsonArray))
		{
			return false;
		}
		return true;
	}

	public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
	{
		return Task<object>.Factory.StartNew(() =>
		{
			if (type.Name.ToLower().Contains("workflowprocess") || (type.Name.ToLower().Contains("list") && type.GetGenericArguments().Any((Type x) => x.Name.ToLower().Contains("workflowprocess"))))
			{
				string value = (content.Headers.ContentEncoding.ToList().Exists((string t) => t.Equals("deflate")) ? CompressionHelper.UndeflateByte(content.ReadAsByteArrayAsync().Result).ConvertByteToString() : ((!content.Headers.ContentEncoding.ToList().Exists((string t) => t.Equals("deflate-Gzip"))) ? content.ReadAsStringAsync().Result : JsoncCompressionHelper.Decompress(content.ReadAsByteArrayAsync().Result).ConvertByteToString()));
				if (type.Name.ToLower().Contains("workflowprocess"))
				{
					return JsonConvert.DeserializeObject<IWorkflowProcess>(value, new JsonConverter[1]
					{
						new ApzJsonConverterIWorkFlowProcess()
					});
				}
				if (type.Name.ToLower().Contains("list") && type.GetGenericArguments().Any((Type x) => x.Name.ToLower().Contains("workflowprocess")))
				{
					return JsonConvert.DeserializeObject<List<IWorkflowProcess>>(value, new JsonConverter[1]
					{
						new ApzJsonConverterListWorkFlowProcess()
					});
				}
				return JsonConvert.DeserializeObject(value, type, new JsonSerializerSettings());
			}
			return new JsonMediaTypeFormatter().ReadFromStreamAsync(type, readStream, content, formatterLogger).Result;
		});
	}
}
