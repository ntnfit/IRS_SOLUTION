using Apzon.Services.Repository.ProdutionAddon;

namespace Apzon.Services.HanaRepository.ProdutionAddon;

public class HanaOperationRepository : OperationRepository
{
	public HanaOperationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
