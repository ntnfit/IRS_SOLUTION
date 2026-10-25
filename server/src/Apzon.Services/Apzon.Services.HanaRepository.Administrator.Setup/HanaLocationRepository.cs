using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaLocationRepository : LocationRepository
{
	public HanaLocationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetMaxCodeQuery()
	{
		return "SELECT IFNULL( MAX(\"Code\" ),0) as \"Code\"\r\n                            FROM OLCT; ";
	}
}
