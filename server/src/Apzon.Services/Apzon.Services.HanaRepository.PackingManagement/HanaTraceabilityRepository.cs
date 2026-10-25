using Apzon.Services.Repository.PackingManagement;

namespace Apzon.Services.HanaRepository.PackingManagement;

public class HanaTraceabilityRepository : TraceabilityRepository
{
	public HanaTraceabilityRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
