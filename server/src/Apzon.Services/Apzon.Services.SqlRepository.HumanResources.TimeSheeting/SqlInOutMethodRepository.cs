using Apzon.Services.Repository.HumanResources.TimeSheeting;

namespace Apzon.Services.SqlRepository.HumanResources.TimeSheeting;

public class SqlInOutMethodRepository : InOutMethodRepository
{
	public SqlInOutMethodRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
