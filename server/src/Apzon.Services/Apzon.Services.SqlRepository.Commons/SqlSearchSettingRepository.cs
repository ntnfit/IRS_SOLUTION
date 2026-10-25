using Apzon.Services.Repository.Commons;

namespace Apzon.Services.SqlRepository.Commons;

internal class SqlSearchSettingRepository : SearchSettingRepository
{
	public SqlSearchSettingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
