using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Apzon.Api.Hubs;

public class HubAuthorizeAttribute : AuthorizeAttribute
{
	public override bool AuthorizeHubConnection(HubDescriptor hubDescriptor, IRequest request)
	{
		string text = request.Headers["sessionId"];
		if ((string.IsNullOrEmpty(text) || !Global.SessionUserInfo.ContainsKey(text)) && request.LocalPath.Length > 5 && !request.LocalPath.Substring(request.LocalPath.Length - 5).Equals("abort"))
		{
			return false;
		}
		return true;
	}
}
