using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaLoginIRSRepository : LoginIRSRepository
{
	public HanaLoginIRSRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetCheckAdministratorQuery()
	{
		return "select 1 from OUSR where UPPER(USER_CODE) = UPPER($userAdministrator);\r\n                     Select 1 from OUSR where UPPER(USER_CODE) = UPPER(user) or UPPER($userAdministrator) = UPPER($user);";
	}

	protected override string GetCheckLicenseIrsGetLicenseQuery()
	{
		return "declare installNumber nvarchar(50);\r\n                    select ifnull((select top 1 \"InstallNumber\" from APZ_OALC where \"HardwareKey\" = @hardKey), '') into installNumber from DUMMY;\r\n                    Select top 1 \"InstallNumber\", \"License\", \"DateRequest\" from APZ_OALC where \"HardwareKey\" = @hardKey;\r\n\r\n                    Select \"LicenseType\", COUNT(1) \"LicenseUse\" from (select \"LicenseType\" from APZ_LOU where \"InstallNumber\" = $installNumber group by \"LicenseType\", \"User\") x group by \"LicenseType\";\r\n\r\n                    Select \"LicenseType\" from APZ_LOU where \"InstallNumber\" = $installNumber and \"User\" = @user and \"DataBase\" = @database;";
	}

	protected override string GetGetupdateDateRequestQuery()
	{
		return "update APZ_OALC set \"DateRequest\" = dateRespuest where \"InstallNumber\" = $insNum;";
	}

	protected override string GetServerDateQuery()
	{
		return "SELECT current_date from DUMMY;";
	}
}
