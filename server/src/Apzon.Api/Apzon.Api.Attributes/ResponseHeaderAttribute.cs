using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Apzon.Api.Attributes;

public class ResponseHeaderAttribute : ActionFilterAttribute
{
	public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
	{
		Collection<DeflateCompressionAttribute> customAttributes = actionExecutedContext.ActionContext.ActionDescriptor.GetCustomAttributes<DeflateCompressionAttribute>();
		if (customAttributes != null && customAttributes.Count > 0 && actionExecutedContext.ActionContext.Response != null && actionExecutedContext.ActionContext.Response.Content != null && actionExecutedContext.ActionContext.Response.Content.Headers != null)
		{
			actionExecutedContext.ActionContext.Response.Content.Headers.Remove("Content-Type");
			actionExecutedContext.ActionContext.Response.Content.Headers.Add("Content-encoding", "deflate");
			actionExecutedContext.ActionContext.Response.Content.Headers.Add("Content-Type", "application/json");
		}
		base.OnActionExecuted(actionExecutedContext);
	}

	public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
	{
		Collection<DeflateCompressionAttribute> customAttributes = actionExecutedContext.ActionContext.ActionDescriptor.GetCustomAttributes<DeflateCompressionAttribute>();
		if (customAttributes != null && customAttributes.Count > 0 && actionExecutedContext.ActionContext.Response != null && actionExecutedContext.ActionContext.Response.Content != null && actionExecutedContext.ActionContext.Response.Content.Headers != null)
		{
			actionExecutedContext.ActionContext.Response.Content.Headers.Remove("Content-Type");
			actionExecutedContext.ActionContext.Response.Content.Headers.Add("Content-encoding", "deflate");
			actionExecutedContext.ActionContext.Response.Content.Headers.Add("Content-Type", "application/json");
		}
		return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
	}
}
