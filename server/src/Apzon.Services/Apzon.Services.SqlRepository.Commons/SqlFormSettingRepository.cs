using Apzon.Services.Repository.Commons;

namespace Apzon.Services.SqlRepository.Commons;

internal class SqlFormSettingRepository : FormSettingRepository
{
	public SqlFormSettingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
