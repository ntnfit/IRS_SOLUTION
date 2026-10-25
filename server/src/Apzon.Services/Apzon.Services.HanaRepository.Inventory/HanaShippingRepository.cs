using Apzon.Services.Repository.Inventory;

namespace Apzon.Services.HanaRepository.Inventory;

public class HanaShippingRepository : ShippingRepository
{
	public HanaShippingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetNextIdQuery()
	{
		return "SELECT IFNULL((SELECT Max(\"DocEntry\") FROM APZ_SHIP),0)+1 ID FROM DUMMY;";
	}
}
