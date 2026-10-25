using Apzon.Services.Repository.Production;

namespace Apzon.Services.HanaRepository.Production;

public class HanaProductionOrderImportToolRepository : ProductionOrderImportToolRepository
{
	public HanaProductionOrderImportToolRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetBinWarehouseQuery()
	{
		return "select \"WhsCode\" from owhs where ifnull(\"BinActivat\",'N') = 'Y';";
	}
}
