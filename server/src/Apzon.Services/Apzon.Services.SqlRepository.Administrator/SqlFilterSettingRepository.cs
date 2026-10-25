using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.SqlRepository.Administrator;

internal class SqlFilterSettingRepository : FilterSettingRepository
{
	public SqlFilterSettingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
