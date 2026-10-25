using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaLengthAndWidthRepository : LengthAndWidthRepository
{
	public HanaLengthAndWidthRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetMaxQuery()
	{
		return "SELECT MAX (IFNULL(\"UnitCode\",0)) as \"UnitCode\" from OLGT;";
	}
}
