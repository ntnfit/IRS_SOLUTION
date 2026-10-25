using Apzon.Services.Repository.Inventory.ItemManagement;

namespace Apzon.Services.HanaRepository.Inventory.ItemManagement;

internal class HanaAlternativeItemsRepository : AlternativeItemsRepository
{
	public HanaAlternativeItemsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
