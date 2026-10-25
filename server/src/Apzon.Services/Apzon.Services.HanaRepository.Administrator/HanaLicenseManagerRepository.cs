using Apzon.Services.Repository.Administrator;

namespace Apzon.Services.HanaRepository.Administrator;

public class HanaLicenseManagerRepository : LicenseManagerRepository
{
	public HanaLicenseManagerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetUpdateSessionUserQuery()
	{
		return "UPDATE APZ_LOGLOGIN SET \"LogoffTime\" = current_date , \"status\" = 'N' where \"User\" = $user and \"Session\"=$session;\r\n                     INSERT INTO APZ_LOGLOGIN (\"ID\", \"Session\", \"User\",  \"DataBase\", \"LoginTime\", \"UpdateTime\" , \"status\",\"ClientLocalIpAddress\",\"ClientMachineName\",\"ClientWindowsLogonName\") values \r\n\t\t\t\t\t\t\t\t(ifnull((select max(ID) from APZ_LOGLOGIN),0) + 1, $session, $user, $database, now(), now() , 'Y',$client_ip,$client_machine,$client_logon);";
	}

	protected override string GetServerDateQuery()
	{
		return "SELECT current_date as d from DUMMY;";
	}

	protected override string GetLicenseUserQuery()
	{
		return "select LOWER(\"User\") \"User\",\"DataBase\" from APZ_LOU where \"InstallNumber\" = $licenseNo and \"LicenseType\" = $licenseType;";
	}

	protected override string GetLicenseQuery()
	{
		return "select \"License\", \"InstallNumber\" From APZ_OALC a where a.\"HardwareKey\" = @hardwarekey;\r\n                     select x.\"LicenseType\", count(1) \"Used\" from (select a.\"LicenseType\" from APZ_LOU a inner join APZ_OALC b ON a.\"InstallNumber\" = b.\"InstallNumber\" where b.\"HardwareKey\" = @hardwarekey group by  a.\"LicenseType\", a.\"User\") x group by x.\"LicenseType\";";
	}

	protected override string GetOfflineQuery()
	{
		return "select \"Session\", \"User\", \"UpdateTime\", \"DataBase\" from APZ_LOGLOGIN where \"status\" = 'Y';";
	}

	protected override string GetUpdateSessionQuery()
	{
		return "UPDATE APZ_LOGLOGIN SET \"LogoffTime\" = logoffDate , \"status\" = status where \"Session\" = $session;";
	}

	protected override string GetDeleteAllLicenseUserQuery()
	{
		return "Delete a from APZ_LOU a INNER JOIN APZ_OALC b on a.\"InstallNumber\" = b.\"InstallNumber\" where b.\"HardwareKey\" = $hardwareKey;";
	}

	protected override string GetUpdateDataTrialMaxIdQuery()
	{
		return "Select \"Quantity\", \"LocCode\", \"ItemCode\", \"DocDate\", \"TransType\", \"DocEntry\", \"MessageID\" from OILM where \"MessageID\" = $id;";
	}

	protected override string GetUpdateDataTrialGetdataQuery()
	{
		return "Select \"Quantity\", \"LocCode\", \"ItemCode\", \"DocDate\", \"TransType\", \"DocEntry\", \"MessageID\" from OILM where \"MessageID\" = $id;";
	}

	protected override string GetUpdateDataTrialUpdateQuery()
	{
		return "UPDATE APZ_CINF SET \"TextCode\" = $code;";
	}
}
