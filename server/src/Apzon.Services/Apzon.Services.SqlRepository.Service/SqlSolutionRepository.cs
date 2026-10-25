using Apzon.Services.Repository.Service;

namespace Apzon.Services.SqlRepository.Service;

public class SqlSolutionRepository : SolutionRepository
{
	public SqlSolutionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
