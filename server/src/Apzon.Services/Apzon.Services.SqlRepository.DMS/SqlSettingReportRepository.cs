using Apzon.Services.Repository.DMS;

namespace Apzon.Services.SqlRepository.DMS;

public class SqlSettingReportRepository : SettingReportRepository
{
	public SqlSettingReportRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
