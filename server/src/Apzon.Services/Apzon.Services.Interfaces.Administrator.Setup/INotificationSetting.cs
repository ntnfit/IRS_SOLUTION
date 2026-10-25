using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface INotificationSetting
{
	DataTable GetData(string user_code);

	string SaveData(NotificationSettingImpl setting);
}
