using Apzon.Services.Repository.FacebookCRM;

namespace Apzon.Services.SqlRepository.FacebookCRM;

internal class SqlFacebookCRMRepository : FacebookCRMRepository
{
	public SqlFacebookCRMRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
