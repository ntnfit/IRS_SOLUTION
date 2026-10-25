using Apzon.Services.Repository.PackingManagement;

namespace Apzon.Services.SqlRepository.PackingManagement;

internal class SqlCreateTypeCaseRepository : CreateTypeCaseRepository
{
	public SqlCreateTypeCaseRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
