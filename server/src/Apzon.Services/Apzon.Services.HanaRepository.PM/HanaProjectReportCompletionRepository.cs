using Apzon.Services.Repository.PM;

namespace Apzon.Services.HanaRepository.PM;

public class HanaProjectReportCompletionRepository : ProjectReportCompletionRepository
{
	public HanaProjectReportCompletionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
