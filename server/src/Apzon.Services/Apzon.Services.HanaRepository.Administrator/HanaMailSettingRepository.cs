using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.HanaRepository.Administrator;

public class HanaMailSettingRepository : MailSettingRepository
{
	public HanaMailSettingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
