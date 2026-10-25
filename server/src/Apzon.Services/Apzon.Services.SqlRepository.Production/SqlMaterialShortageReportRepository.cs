using Apzon.Services.Repository.Production;

namespace Apzon.Services.SqlRepository.Production;

public class SqlMaterialShortageReportRepository : MaterialShortageReportRepository
{
	public SqlMaterialShortageReportRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
