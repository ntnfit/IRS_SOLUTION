using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface INotificationSettingClient
{
	Task<DataTable> GetData(string user_code);

	Task<string> SaveData(NotificationSettingImpl setting);
}
