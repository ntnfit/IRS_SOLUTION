using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.SqlRepository.ProdutionAddon;

public class SqlReportCompleteForProductionRepository : ReportCompleteForProductionRepository
{
	public SqlReportCompleteForProductionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
