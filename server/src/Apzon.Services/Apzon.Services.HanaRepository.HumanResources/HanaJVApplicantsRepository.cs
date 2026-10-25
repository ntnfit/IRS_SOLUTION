using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.HanaRepository.HumanResources;

public class HanaJVApplicantsRepository : JVApplicantsRepository
{
	public HanaJVApplicantsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public override string GetMaxIdApplicantQuery()
	{
		return "select ifnull(max(\"ApplicantID\"), 0) as \"ApplicantID\" from APZ_OHAP;";
	}
}
