using Apzon.Services.Repository.Commons;

namespace Apzon.Services.HanaRepository.Commons;

public class HanaFormulaRepository : FormulaRepository
{
	public HanaFormulaRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
