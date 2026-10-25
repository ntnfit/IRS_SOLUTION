using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Hubs;

public class NotificationClient : BaseHub<NotificationClient>
{
	public void SendNotifiedToClient(NotifiedObj notified)
	{
		try
		{
			if (notified.AssignAll)
			{
				AllUserInGroup.UpdateNotification(notified);
				return;
			}
			string[] array = notified.NotifiedUser.Split(';').Distinct().ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				if (string.IsNullOrEmpty(array[i]))
				{
					continue;
				}
				List<object> socketByUser = GetSocketByUser(array[i]);
				if (socketByUser == null)
				{
					continue;
				}
				for (int j = 0; j < socketByUser.Count; j++)
				{
					if ((dynamic)socketByUser[j] != null)
					{
						((dynamic)socketByUser[j]).UpdateNotification(notified);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}
}
