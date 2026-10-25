using Apzon.Services.Repository.HumanResources.TimeSheeting;

namespace Apzon.Services.HanaRepository.HumanResources.TimeSheeting;

public class HanaLeaveInformationRepository : LeaveInformationRepository
{
	public HanaLeaveInformationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
