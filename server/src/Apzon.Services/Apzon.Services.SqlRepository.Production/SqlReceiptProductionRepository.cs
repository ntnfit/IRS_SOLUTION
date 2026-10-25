using Apzon.Services.Repository.Production;

namespace Apzon.Services.SqlRepository.Production;

public class SqlReceiptProductionRepository : ReceiptProductionRepository
{
	public SqlReceiptProductionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
