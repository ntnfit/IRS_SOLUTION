using Apzon.Services.Repository.PackingManagement;

namespace Apzon.Services.SqlRepository.PackingManagement;

internal class SqlTraceabilityRepository : TraceabilityRepository
{
	public SqlTraceabilityRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
