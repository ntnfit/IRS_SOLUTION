using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.HanaRepository.ProdutionAddon;

public class HanaReportCompleteForProductionRepository : ReportCompleteForProductionRepository
{
	public HanaReportCompleteForProductionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
