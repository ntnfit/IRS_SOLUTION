using Apzon.Services.Repository.DMS;

namespace Apzon.Services.SqlRepository.DMS;

public class SqlSaleTerritoryRepository : SaleTerritoryRepository
{
	public SqlSaleTerritoryRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
