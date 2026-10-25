using System;
using System.Web.Http.Filters;
using Apzon.Commons;

namespace Apzon.Api.Attributes;

public class MemoryAttribute : ActionFilterAttribute
{
	public override void OnActionExecuted(HttpActionExecutedContext actContext)
	{
		Logging.Write("Write memery!");
		byte[] array = actContext.Response.Content?.ReadAsByteArrayAsync().Result;
		if (array != null)
		{
			Logging.Write("Memory byte: " + (double)array.Length / Math.Pow(2.0, 20.0) + "M");
		}
		else
		{
			Logging.Write("Memory byte: " + 0 + "M");
		}
		base.OnActionExecuted(actContext);
	}
}
