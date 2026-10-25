using Apzon.Services.Repository.Commons;

namespace Apzon.Services.HanaRepository.Commons;

public class HanaCreateFormulaRepository : CreateFormulaRepository
{
	public HanaCreateFormulaRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
