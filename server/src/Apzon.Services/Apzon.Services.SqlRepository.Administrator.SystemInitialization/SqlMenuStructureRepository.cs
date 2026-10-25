using Apzon.Services.Repository.Administrator.SystemInitialization;

namespace Apzon.Services.SqlRepository.Administrator.SystemInitialization;

public class SqlMenuStructureRepository : MenuStructureRepository
{
	public SqlMenuStructureRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
