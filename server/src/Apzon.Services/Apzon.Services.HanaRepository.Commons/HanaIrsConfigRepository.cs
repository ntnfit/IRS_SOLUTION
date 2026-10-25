using Apzon.Services.Repository.Commons;

namespace Apzon.Services.HanaRepository.Commons;

public class HanaIrsConfigRepository : IrsConfigRepository
{
	public HanaIrsConfigRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
