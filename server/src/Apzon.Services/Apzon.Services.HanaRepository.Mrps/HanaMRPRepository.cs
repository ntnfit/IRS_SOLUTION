using Apzon.Services.Repository.Mrps;

namespace Apzon.Services.HanaRepository.Mrps;

public class HanaMRPRepository : MRPRepository
{
	public HanaMRPRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetNewIdMrpQuery()
	{
		return "select ifnull((Select max(\"MrpCode\") from APZ_OMRP), 0) from DUMMY;";
	}

	protected override string ValidateScheduleForMrpQuery()
	{
		return "select (select 1 from apz_OPRS where \"PSCode\" in (select trim(data) from split(@lstPsCode, ',')) and \"Status\" <> 'A') \"Approval\", \r\n                    (select 1 from APZ_OPRS where \"PSCode\" in (select trim(data)  from split(@lstPsCode, ',')) and \"Type\" <> 'T' and \"Type\" <> 'O') \"ScheduleType\" from dummy;\r\n                    Select a.*from APZ_OMRP a inner join APZ_OPRS b on a.\"MrpCode\" = b.\"MrpId\" where(select count(1)\r\n                        from split(@lstPsCode, ',')) = 1 and b.\"PSCode\" in (select trim(data) from Split(@lstPsCode, ','));";
	}
}
