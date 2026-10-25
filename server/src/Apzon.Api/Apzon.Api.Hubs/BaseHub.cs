using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Apzon.Api.Broker;
using Apzon.Api.Models;
using Apzon.Commons;
using Apzon.Services;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Apzon.Api.Hubs;

[HubAuthorize]
public abstract class BaseHub<T> : Hub where T : IHub
{
	public IHubContext HubContext;

	protected IUnitOfWork UnitOfWork;

	protected string GroupName = "";

	protected dynamic AllUser;

	protected dynamic OtherUser;

	protected List<dynamic> CurrentUser = new List<object>();

	protected dynamic AllUserInGroup;

	protected dynamic OtherUserInGroup;

	protected List<string> HubConnectionId { get; private set; } = new List<string>();

	protected BaseHub()
	{
		try
		{
			if (HttpContext.Current != null)
			{
				string text = HttpContext.Current.Request.Headers["sessionId"];
				if (!string.IsNullOrEmpty(text))
				{
					if (Global.SessionUserInfo.ContainsKey(text))
					{
						GroupName = $"{Global.SessionUserInfo[text].DatabaseName}-{GetType().Name}";
					}
					IUnitOfWork apzUnitOfWork = BrokerService.GetApzUnitOfWork(text);
					if (apzUnitOfWork != null)
					{
						UnitOfWork = apzUnitOfWork;
					}
					HubConnectionId = GetConnectionIdBySessionId(text);
				}
			}
			HubContext = GlobalHost.ConnectionManager.GetHubContext<T>();
			AllUser = HubContext.Clients.All;
			if (HubConnectionId != null && HubConnectionId.Count > 0)
			{
				OtherUser = HubContext.Clients.AllExcept(HubConnectionId.ToArray());
			}
			else
			{
				OtherUser = AllUser;
			}
			if (!string.IsNullOrEmpty(GroupName))
			{
				AllUserInGroup = HubContext.Clients.Group(GroupName);
				if (HubConnectionId != null && HubConnectionId.Count > 0)
				{
					OtherUserInGroup = HubContext.Clients.Group(GroupName, HubConnectionId.ToArray());
				}
				else
				{
					OtherUserInGroup = AllUserInGroup;
				}
			}
			if (HubConnectionId == null || HubConnectionId.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < HubConnectionId.Count; i++)
			{
				if (!string.IsNullOrEmpty(HubConnectionId[i]))
				{
					((List<object>)CurrentUser).Add(HubContext.Clients.Client(HubConnectionId[i]));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private List<string> GetConnectionIdBySessionId(string sessionId)
	{
		try
		{
			List<string> list = new List<string>();
			if (Global.SessionUserInfo != null && Global.SessionUserInfo.ContainsKey(sessionId))
			{
				KeySocketModel[] array = Global.SessionUserInfo[sessionId].ListSocketClientConect.Where((KeySocketModel t) => t.SocketType.Equals(GetType().Name)).ToArray();
				for (int num = 0; num < array.Length; num++)
				{
					if (!string.IsNullOrEmpty(array[num].SocketId))
					{
						list.Add(array[num].SocketId);
					}
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}

	public dynamic UserByConnetionId(string connectionId)
	{
		return HubContext.Clients.Client(connectionId);
	}

	public List<dynamic> GetSocketByUser(string userName, string dbName = "")
	{
		try
		{
			KeyValuePair<string, SessionUserInfoImpl>[] array = Global.SessionUserInfo.Where((KeyValuePair<string, SessionUserInfoImpl> t) => t.Value.Equals(userName)).ToArray();
			if (array.Length == 0)
			{
				return null;
			}
			List<object> list = new List<object>();
			for (int num = 0; num < array.Length; num++)
			{
				string key = array[num].Key;
				SessionUserInfoImpl value = array[num].Value;
				if (!GroupName.Equals($"{value.DatabaseName}-{GetType().Name}"))
				{
					return null;
				}
				KeySocketModel[] array2 = Global.SessionUserInfo[key].ListSocketClientConect.Where((KeySocketModel t) => t.SocketType.Equals(GetType().Name)).ToArray();
				for (int num2 = 0; num2 < array2.Length; num2++)
				{
					if (array2[num2] != null && !string.IsNullOrEmpty(array2[num2].SocketId))
					{
						list.Add(HubContext.Clients.Client(Function.ToString(array2[num2].SocketId)));
					}
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}

	public override Task OnConnected()
	{
		try
		{
			lock (Global.SessionUserInfo)
			{
				string text = Context.Headers["sessionId"];
				if (!string.IsNullOrEmpty(text) && Global.SessionUserInfo.ContainsKey(text))
				{
					KeySocketModel objKey = new KeySocketModel
					{
						SocketId = Context.ConnectionId,
						SocketType = GetType().Name
					};
					if (Global.SessionUserInfo[text].ListSocketClientConect == null)
					{
						Global.SessionUserInfo[text].ListSocketClientConect = new List<KeySocketModel>();
					}
					if (!Global.SessionUserInfo[text].ListSocketClientConect.Exists((KeySocketModel t) => t.SocketId.Equals(objKey.SocketId) && t.SocketType.Equals(objKey.SocketType)))
					{
						Global.SessionUserInfo[text].ListSocketClientConect.Add(objKey);
					}
					GroupName = $"{Global.SessionUserInfo[text].DatabaseName}-{GetType().Name}";
					IUnitOfWork apzUnitOfWork = BrokerService.GetApzUnitOfWork(text);
					if (apzUnitOfWork != null)
					{
						UnitOfWork = apzUnitOfWork;
					}
					HubConnectionId = GetConnectionIdBySessionId(text);
				}
				HubContext = GlobalHost.ConnectionManager.GetHubContext<T>();
				AllUser = HubContext.Clients.All;
				if (HubConnectionId != null && HubConnectionId.Count > 0)
				{
					OtherUser = HubContext.Clients.AllExcept(HubConnectionId.ToArray());
				}
				else
				{
					OtherUser = AllUser;
				}
				if (!string.IsNullOrEmpty(GroupName))
				{
					AllUserInGroup = HubContext.Clients.Group(GroupName);
					if (HubConnectionId != null && HubConnectionId.Count > 0)
					{
						OtherUserInGroup = HubContext.Clients.Group(GroupName, HubConnectionId.ToArray());
					}
					else
					{
						OtherUserInGroup = AllUserInGroup;
					}
				}
				if (HubConnectionId != null && HubConnectionId.Count > 0)
				{
					for (int num = 0; num < HubConnectionId.Count; num++)
					{
						if (!string.IsNullOrEmpty(HubConnectionId[num]))
						{
							((List<object>)CurrentUser).Add(HubContext.Clients.Client(HubConnectionId[num]));
						}
					}
				}
				JoinGroup(GroupName);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return base.OnConnected();
	}

	public override Task OnDisconnected(bool stopCalled)
	{
		lock (Global.SessionUserInfo)
		{
			string key = Context.Headers["sessionId"];
			if (Global.SessionUserInfo != null && Global.SessionUserInfo.Count > 0 && Global.SessionUserInfo.ContainsKey(key))
			{
				KeySocketModel keySocketModel = Global.SessionUserInfo[key].ListSocketClientConect.FirstOrDefault((KeySocketModel t) => t.SocketId.Equals(Context.ConnectionId) && t.SocketType.Equals(GetType().Name));
				if (keySocketModel != null)
				{
					Global.SessionUserInfo[key].ListSocketClientConect.Remove(keySocketModel);
				}
			}
		}
		return base.OnDisconnected(stopCalled);
	}

	public void JoinGroup(string groupName)
	{
		Groups.Add(Context.ConnectionId, groupName);
	}

	public void LeaveGroup(string groupName)
	{
		Groups.Remove(Context.ConnectionId, groupName);
	}
}
