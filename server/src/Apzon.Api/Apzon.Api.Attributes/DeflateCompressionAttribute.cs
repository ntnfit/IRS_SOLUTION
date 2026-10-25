using System.Net.Http;
using System.Web.Http.Filters;
using Apzon.Commons.Helper;

namespace Apzon.Api.Attributes;

public class DeflateCompressionAttribute : ActionFilterAttribute
{
	public override void OnActionExecuted(HttpActionExecutedContext actContext)
	{
		byte[] array = actContext.Response.Content?.ReadAsByteArrayAsync().Result;
		byte[] content = ((array == null) ? new byte[0] : CompressionHelper.DeflateByte(array));
		actContext.Response.Content = new ByteArrayContent(content);
		actContext.Response.Content.Headers.Remove("Content-Type");
		actContext.Response.Content.Headers.Add("Content-encoding", "deflate");
		actContext.Response.Content.Headers.Add("Content-Type", "application/json");
		base.OnActionExecuted(actContext);
	}
}
