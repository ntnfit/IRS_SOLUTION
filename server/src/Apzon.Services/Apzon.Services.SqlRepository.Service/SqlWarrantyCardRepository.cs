using Apzon.Services.Repository.Service;

namespace Apzon.Services.SqlRepository.Service;

public class SqlWarrantyCardRepository : WarrantyCardRepository
{
	public SqlWarrantyCardRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
