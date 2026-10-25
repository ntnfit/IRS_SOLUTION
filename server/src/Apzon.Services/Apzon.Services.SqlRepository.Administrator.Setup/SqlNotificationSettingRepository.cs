using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

internal class SqlNotificationSettingRepository : NotificationSettingRepository
{
	public SqlNotificationSettingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
