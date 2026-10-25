using Apzon.Services.Repository.Production;

namespace Apzon.Services.SqlRepository.Production;

public class SqlOutsourcingRepository : OutsourcingRepository
{
	public SqlOutsourcingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
