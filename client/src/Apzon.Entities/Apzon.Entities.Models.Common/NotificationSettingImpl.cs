namespace Apzon.Entities.Models.Common;

public class NotificationSettingImpl
{
	public string user_code { get; set; }

	public string typeDay { get; set; }

	public int fromDay { get; set; }

	public int toDay { get; set; }

	public string pickDay { get; set; }

	public string typeTime { get; set; }

	public int fromTime { get; set; }

	public int toTime { get; set; }

	public int specifyNotification { get; set; }

	public string typeSpecify { get; set; }
}
