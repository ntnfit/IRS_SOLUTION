using Apzon.Services.Repository.Service;

namespace Apzon.Services.SqlRepository.Service;

public class SqlServiceCallSettingRepository : ServiceCallSettingRepository
{
	public SqlServiceCallSettingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
