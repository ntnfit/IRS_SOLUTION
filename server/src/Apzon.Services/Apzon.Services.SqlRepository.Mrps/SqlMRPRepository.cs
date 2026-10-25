using Apzon.Services.Repository.Mrps;

namespace Apzon.Services.SqlRepository.Mrps;

public class SqlMRPRepository : MRPRepository
{
	public SqlMRPRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
