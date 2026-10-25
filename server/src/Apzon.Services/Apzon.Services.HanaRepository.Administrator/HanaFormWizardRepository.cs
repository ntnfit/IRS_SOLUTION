using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.HanaRepository.Administrator;

public class HanaFormWizardRepository : FormWizardRepository
{
	public HanaFormWizardRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetValidQuery()
	{
		return "SELECT * FROM APZ_OFWZ WHERE \"Valid\"='Y';";
	}
}
