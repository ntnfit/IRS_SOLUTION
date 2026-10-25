using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaNotificationSettingRepository : NotificationSettingRepository
{
	public HanaNotificationSettingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetDataQuery()
	{
		return "select * from APZ_NOSE where \"UserCode\"=usercode;";
	}
}
