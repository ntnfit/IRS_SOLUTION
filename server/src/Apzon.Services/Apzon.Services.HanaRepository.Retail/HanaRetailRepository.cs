using Apzon.Services.Repository.Retail;

namespace Apzon.Services.HanaRepository.Retail;

public class HanaRetailRepository : RetailRepository
{
	public HanaRetailRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
