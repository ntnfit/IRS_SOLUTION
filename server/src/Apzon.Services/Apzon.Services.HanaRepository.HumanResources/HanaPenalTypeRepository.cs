using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.HanaRepository.HumanResources;

public class HanaPenalTypeRepository : PenalTypeRepository
{
	public HanaPenalTypeRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetDeleteQuery()
	{
		return "Delete from APZ_OHPT where  \"Code\" NOT IN(select \"PenalizationType\" from APZ_HPT1) and \"Code\" = @code;\r\n                    select ::ROWCOUNT from dummy;";
	}
}
