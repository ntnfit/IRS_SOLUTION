using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.HanaRepository.HumanResources;

public class HanaHREducationRepository : HREducationRepository
{
	public HanaHREducationRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetManagerNameQuery()
	{
		return "SELECT a.\"manager\", CASE WHEN LTRIM(RTRIM(\"lastName\")) = '' OR \"firstName\" IS NULL THEN IFNULL(\"firstName\", '') ELSE CASE WHEN LTRIM(RTRIM(\"lastName\")) = '' OR \"lastName\" IS NULL THEN IFNULL(\"lastName\", '') ELSE IFNULL(\"firstName\", '') || ', ' || IFNULL(\"lastName\", '') END END AS \"managerName\" FROM hem3 a LEFT JOIN ohem b ON a.\"manager\" = b.\"empID\";";
	}

	protected override string GetMaxIDQuery()
	{
		return "select ifnull(max(\"empID\"), 0) from OHEM";
	}
}
