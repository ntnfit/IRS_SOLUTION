using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.SqlRepository.Administrator;

internal class SqlFormWizardRepository : FormWizardRepository
{
	public SqlFormWizardRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
