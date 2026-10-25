using System.Data;

namespace Apzon.Services.Interfaces.DMS;

public interface INotification
{
	string UpdateNotificationEvent(DataTable dt);

	string AddNotificationEvent(DataTable dt);

	DataTable GetListApp();

	DataTable GetEventSetting(string appName);

	string PushDeviceToken(DataTable dt);

	string RemoveDeviceToken(DataTable dt);

	DataTable GetNotifications(string userName, string appName, string dbName, int startIndex, int pageSize);

	string UpdateNotification(DataTable dt);

	DataTable GetUnreadNotifications(string userName, string appName, string dbName);

	string MarkAllAsRead(DataTable dt);
}
