using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.SqlRepository.Administrator;

internal class SqlEmpTokenRepository(IDatabaseService databaseService) : EmpTokenRepository(databaseService)
{
	public new string TempOhet = "#APZ_OHET";
}
