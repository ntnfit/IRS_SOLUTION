using Apzon.Services.Repository.Inventory;

namespace Apzon.Services.HanaRepository.Inventory;

public class HanaPackingRepository : PackingRepository
{
	public HanaPackingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetNextIdQuery()
	{
		return "SELECT IFNULL((SELECT Max(\"DocEntry\") FROM APZ_OPAK),0)+1 ID from DUMMY;";
	}
}
