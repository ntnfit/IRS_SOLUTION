using Apzon.Services.Repository.PM;

namespace Apzon.Services.HanaRepository.PM;

internal class HanaGanttControlRepository : GanttControlRepository
{
	public HanaGanttControlRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
