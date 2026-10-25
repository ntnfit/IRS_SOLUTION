using Apzon.Services.Repository.PM;

namespace Apzon.Services.SqlRepository.PM;

public class SqlProjectReportCompletionRepository : ProjectReportCompletionRepository
{
	public SqlProjectReportCompletionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
