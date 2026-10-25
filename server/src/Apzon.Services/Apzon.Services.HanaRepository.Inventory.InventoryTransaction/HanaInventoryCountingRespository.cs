using Apzon.Services.Repository.Inventory.InventoryTransaction;

namespace Apzon.Services.HanaRepository.Inventory.InventoryTransaction;

public class HanaInventoryCountingRespository : InventoryCountingRespository
{
	public HanaInventoryCountingRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetMaxDocEntryQuery()
	{
		return "SELECT MAX(\"DocEntry\") FROM APZ_OINC";
	}
}
