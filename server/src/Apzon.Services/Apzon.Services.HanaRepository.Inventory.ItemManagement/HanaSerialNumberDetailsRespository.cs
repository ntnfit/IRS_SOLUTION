using Apzon.Services.Repository.Inventory.ItemManagement;

namespace Apzon.Services.HanaRepository.Inventory.ItemManagement;

public class HanaSerialNumberDetailsRespository : SerialNumberDetailsRespository
{
	public HanaSerialNumberDetailsRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
