using Apzon.Services.Repository.HumanResources.TimeSheeting;

namespace Apzon.Services.SqlRepository.HumanResources.TimeSheeting;

public class SqlLeaveInformationRepository : LeaveInformationRepository
{
	public SqlLeaveInformationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
