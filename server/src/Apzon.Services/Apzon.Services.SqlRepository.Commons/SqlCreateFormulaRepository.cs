using Apzon.Services.Repository.Commons;

namespace Apzon.Services.SqlRepository.Commons;

public class SqlCreateFormulaRepository : CreateFormulaRepository
{
	public SqlCreateFormulaRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
