using Apzon.Services.Repository.HumanResources.TimeSheeting;

namespace Apzon.Services.HanaRepository.HumanResources.TimeSheeting;

public class HanaInOutMethodRepository : InOutMethodRepository
{
	public HanaInOutMethodRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
