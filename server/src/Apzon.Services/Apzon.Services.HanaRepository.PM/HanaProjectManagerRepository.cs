using Apzon.Services.Repository.PM;

namespace Apzon.Services.HanaRepository.PM;

public class HanaProjectManagerRepository : ProjectManagerRepository
{
	public HanaProjectManagerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string LoadSubProjectByMasterIdQuery()
	{
		return "select * from APZ_OPRG where \"MasterEntry\"=projectId";
	}
}
