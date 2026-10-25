using Apzon.Services.Repository.Inventory;

namespace Apzon.Services.HanaRepository.Inventory;

public class HanaMaterialRevaluationRepository : MaterialRevaluationRepository
{
	public HanaMaterialRevaluationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
