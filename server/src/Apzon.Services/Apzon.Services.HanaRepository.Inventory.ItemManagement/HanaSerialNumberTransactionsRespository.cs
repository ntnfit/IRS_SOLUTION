using Apzon.Services.Repository.Inventory.ItemManagement;

namespace Apzon.Services.HanaRepository.Inventory.ItemManagement;

public class HanaSerialNumberTransactionsRespository : SerialNumberTransactionsRespository
{
	public HanaSerialNumberTransactionsRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
