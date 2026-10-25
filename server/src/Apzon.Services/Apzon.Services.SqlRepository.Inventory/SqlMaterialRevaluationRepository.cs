using Apzon.Services.Repository.Inventory;

namespace Apzon.Services.SqlRepository.Inventory;

public class SqlMaterialRevaluationRepository : MaterialRevaluationRepository
{
	public SqlMaterialRevaluationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
