using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

public class SqlBranchesRepository : BranchesRepository
{
	public SqlBranchesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
