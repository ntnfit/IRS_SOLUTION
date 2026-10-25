using Apzon.Services.Repository.Service;

namespace Apzon.Services.HanaRepository.Service;

public class HanaWarrantyCardRepository : WarrantyCardRepository
{
	public HanaWarrantyCardRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
