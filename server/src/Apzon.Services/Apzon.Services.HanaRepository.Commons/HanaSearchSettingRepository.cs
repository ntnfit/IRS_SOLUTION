using Apzon.Services.Repository.Commons;

namespace Apzon.Services.HanaRepository.Commons;

public class HanaSearchSettingRepository : SearchSettingRepository
{
	public HanaSearchSettingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
