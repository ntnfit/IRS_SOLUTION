using Apzon.Services.Repository.Commons;

namespace Apzon.Services.SqlRepository.Commons;

public class SqlFormulaRepository : FormulaRepository
{
	public SqlFormulaRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
