using Apzon.Services.Repository.Administrator.Setup.Purchasing;

namespace Apzon.Services.HanaRepository.Administrator.Setup.Purchasing;

public class HanaLandedCostsRepository : LandedCostsRepository
{
	public HanaLandedCostsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetMaxDocEntryQuery()
	{
		return "SELECT IFNULL(MAX(\"DocEntry\"), 0) as \"maxCode\" from OIPF;";
	}
}
