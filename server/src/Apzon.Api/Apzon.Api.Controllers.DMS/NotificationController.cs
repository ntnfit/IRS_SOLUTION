using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.DMS;

[ValidateLoginSapExceptionHandling]
public class NotificationController : BaseApiController
{
	[HttpPost]
	public string PushDeviceToken([FromBody] DataTable dt)
	{
		return UnitOfWork.Notification.PushDeviceToken(dt);
	}

	[HttpPost]
	public string RemoveDeviceToken([FromBody] DataTable dt)
	{
		return UnitOfWork.Notification.RemoveDeviceToken(dt);
	}

	[HttpGet]
	public DataTable GetNotifications(string userName, string appName, string database, int startIndex, int pageSize)
	{
		return UnitOfWork.Notification.GetNotifications(userName, appName, database, startIndex, pageSize);
	}

	[HttpPost]
	public string UpdateNotificationEvent([FromBody] DataTable dt)
	{
		return UnitOfWork.Notification.UpdateNotificationEvent(dt);
	}

	[HttpPost]
	public string AddNotificationEvent([FromBody] DataTable dt)
	{
		return UnitOfWork.Notification.AddNotificationEvent(dt);
	}

	[HttpGet]
	public DataTable GetListApp()
	{
		return UnitOfWork.Notification.GetListApp();
	}

	[HttpGet]
	public DataTable GetEventSetting(string appName)
	{
		return UnitOfWork.Notification.GetEventSetting(appName);
	}

	[HttpPost]
	public string UpdateNotification([FromBody] DataTable dt)
	{
		return UnitOfWork.Notification.UpdateNotification(dt);
	}

	[HttpGet]
	public DataTable GetUnreadNotifications(string userName, string appName, string database)
	{
		return UnitOfWork.Notification.GetUnreadNotifications(userName, appName, database);
	}

	[HttpPost]
	public string MarkAllAsRead([FromBody] DataTable dt)
	{
		return UnitOfWork.Notification.MarkAllAsRead(dt);
	}
}
