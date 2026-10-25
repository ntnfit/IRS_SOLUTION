using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaAuthRepository : AuthRepository
{
	public HanaAuthRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetCheckExistsMemuQuery()
	{
		return "select ifnull((SELECT top 1 'Y' FROM APZ_OMNI), 'N') from dummy;";
	}

	protected override string GetSaveDataProcessQueryDelete()
	{
		return "Delete from OPRM where USER_CODE = userCode;";
	}

	protected override string GetSaveDataProcessQueryUpdateData()
	{
		return " create local temporary table #tempAuth as (select top 0 * from OPRM);\r\n                    exec 'insert into #tempAuth select * from #APZ_AUTH;';\r\n                    INSERT INTO OPRM (USER_CODE,\"menucode\",\"authoriza\",\"isDefault\")\r\n                                SELECT \"USER_CODE\",\"menucode\",\"authoriza\",\"isDefault\"\r\n                                FROM #tempAuth;";
	}

	protected override string GetUserForCopyQuery()
	{
		return "select  USER_CODE,U_NAME ,ifnull(b.\"Name\",'') as \"Department\" from OUSR a\r\n                    left join OUDP b on b.\"Code\" = a.\"Department\" where GROUPS<>99 and user_code<>@user;";
	}
}
