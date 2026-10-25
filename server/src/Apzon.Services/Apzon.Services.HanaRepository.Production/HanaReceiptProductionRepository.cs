using Apzon.Services.Repository.Production;

namespace Apzon.Services.HanaRepository.Production;

public class HanaReceiptProductionRepository : ReceiptProductionRepository
{
	public HanaReceiptProductionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
