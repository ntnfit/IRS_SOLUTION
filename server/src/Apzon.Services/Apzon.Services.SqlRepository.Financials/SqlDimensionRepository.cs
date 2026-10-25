using Apzon.Services.Repository.Financials;

namespace Apzon.Services.SqlRepository.Financials;

public class SqlDimensionRepository : DimensionRepository
{
	public SqlDimensionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
