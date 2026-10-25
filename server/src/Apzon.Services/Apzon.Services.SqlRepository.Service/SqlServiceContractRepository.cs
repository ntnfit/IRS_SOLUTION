using Apzon.Services.Repository.Service;

namespace Apzon.Services.SqlRepository.Service;

public class SqlServiceContractRepository : ServiceContractRepository
{
	public SqlServiceContractRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
