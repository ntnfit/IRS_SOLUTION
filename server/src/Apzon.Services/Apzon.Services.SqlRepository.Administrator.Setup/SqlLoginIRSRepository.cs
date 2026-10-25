using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.SqlRepository.Administrator.Setup;

internal class SqlLoginIRSRepository : LoginIRSRepository
{
	public SqlLoginIRSRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetCheckAdministratorQuery()
	{
		return "select 1 from OUSR where UPPER(USER_CODE) = UPPER(@userAdministrator)\r\n                                     Select 1 from OUSR where UPPER(USER_CODE) = UPPER(@user) or UPPER(@userAdministrator) = UPPER(@user)";
	}
}
