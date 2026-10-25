using Apzon.Services.Repository.Commons;

namespace Apzon.Services.SqlRepository.Commons;

public class SqlFormFilterSettingRepository : FormFilterSettingRepository
{
	public SqlFormFilterSettingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
