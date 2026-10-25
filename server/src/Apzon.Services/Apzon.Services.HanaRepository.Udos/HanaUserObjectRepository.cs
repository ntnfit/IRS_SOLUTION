using Apzon.Services.Repository.Udos;

namespace Apzon.Services.HanaRepository.Udos;

public class HanaUserObjectRepository : UserObjectRepository
{
	public HanaUserObjectRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
