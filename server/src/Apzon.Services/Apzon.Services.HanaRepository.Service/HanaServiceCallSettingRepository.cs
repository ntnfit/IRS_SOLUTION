using Apzon.Services.Repository.Service;

namespace Apzon.Services.HanaRepository.Service;

public class HanaServiceCallSettingRepository : ServiceCallSettingRepository
{
	public HanaServiceCallSettingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
