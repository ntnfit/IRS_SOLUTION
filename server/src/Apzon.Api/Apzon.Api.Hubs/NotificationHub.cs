using System.Threading.Tasks;
using Apzon.Api.Broker;
using Microsoft.AspNet.SignalR.Hubs;

namespace Apzon.Api.Hubs;

[HubName("NotificationHub")]
public class NotificationHub : BaseHub<NotificationHub>
{
	[HubMethodName("SendAllUser")]
	public void SendAllUser(string message)
	{
		if (AllUser != null)
		{
			AllUser.UpdateMessage(message);
		}
	}

	[HubMethodName("SendOtherUser")]
	public void SendOtherUser(string message)
	{
		if (OtherUser != null)
		{
			OtherUser.UpdateMessage(message);
		}
	}

	[HubMethodName("SendCurrentUser")]
	public void SendCurrentUser(string message)
	{
		if (CurrentUser != null)
		{
			for (int i = 0; i < CurrentUser.Count; i++)
			{
				CurrentUser[i].UpdateMessage(message);
			}
		}
	}

	[HubMethodName("SendAllUserInGroup")]
	public void SendAllUserInGroup(string message)
	{
		if (AllUserInGroup != null)
		{
			AllUserInGroup.UpdateMessage(message);
		}
	}

	[HubMethodName("SendOtherUserInGroup")]
	public void SendOtherUserInGroup(string message)
	{
		if (OtherUserInGroup != null)
		{
			OtherUserInGroup.UpdateMessage(message);
		}
	}

	[HubMethodName("CheckOnline")]
	public void CheckOnline(bool isChangeLic, string socketClientId = "")
	{
		if (isChangeLic)
		{
			if (!string.IsNullOrEmpty(socketClientId))
			{
				dynamic val = UserByConnetionId(socketClientId);
				if (val != null)
				{
					val.UpdateSession(false, true);
				}
			}
			else if (Global.CloudSystem)
			{
				if (AllUserInGroup != null)
				{
					AllUserInGroup.UpdateSession(false, true);
				}
			}
			else if (AllUser != null)
			{
				AllUser.UpdateSession(false, true);
			}
		}
		else
		{
			dynamic val2 = UserByConnetionId(socketClientId);
			if (val2 != null)
			{
				val2.UpdateSession(true, false);
			}
		}
	}

	public override Task OnDisconnected(bool stopCalled)
	{
		BrokerService.LogOut(Context.Headers["sessionId"]);
		return base.OnDisconnected(stopCalled);
	}
}
