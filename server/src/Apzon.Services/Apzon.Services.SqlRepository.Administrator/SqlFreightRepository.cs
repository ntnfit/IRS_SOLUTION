using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.SqlRepository.Administrator;

public class SqlFreightRepository : FreightRepository
{
	public SqlFreightRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
