using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApi.OutputCache.V2;

namespace Apzon.Api.Attributes;

public class CachingAttribute : CacheOutputAttribute
{
	public CachingAttribute()
	{
		CacheKeyGenerator = typeof(ApzCacheKeyGenerator);
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

	protected override bool IsCachingAllowed(HttpActionContext actionContext, bool anonymousOnly)
	{
		if (actionContext.Request.Method == HttpMethod.Post)
		{
			return true;
		}
		return base.IsCachingAllowed(actionContext, anonymousOnly);
	}
}
