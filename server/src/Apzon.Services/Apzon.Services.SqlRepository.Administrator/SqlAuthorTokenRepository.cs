using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.SqlRepository.Administrator;

internal class SqlAuthorTokenRepository(IDatabaseService databaseService) : AuthorTokenRepository(databaseService)
{
	public new string TempHet1 = "#APZ_HET1";
}
