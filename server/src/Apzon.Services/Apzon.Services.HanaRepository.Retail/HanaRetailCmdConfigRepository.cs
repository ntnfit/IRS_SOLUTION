using Apzon.Services.Repository.Retail;

namespace Apzon.Services.HanaRepository.Retail;

public class HanaRetailCmdConfigRepository : RetailCmdConfigRepository
{
	public HanaRetailCmdConfigRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
