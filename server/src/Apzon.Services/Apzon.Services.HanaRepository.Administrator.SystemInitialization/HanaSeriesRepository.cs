using Apzon.Services.Repository.Administrator.SystemInitialization;

namespace Apzon.Services.HanaRepository.Administrator.SystemInitialization;

public class HanaSeriesRepository : SeriesRepository
{
	public HanaSeriesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string LoadDocQuery()
	{
		return "declare dfltAllUser int;\r\n                    declare dflUser int;\r\n                    select top 1 \"DfltSeries\" into dfltAllUser from ONNM where \"ObjectCode\" = @objCode and \"DocSubType\" = @docSubType;\r\n                    dflUser = ifnull((select top 1 \"Series\" from NNM2 where \"ObjectCode\" = @objCode and \"DocSubType\" = @docSubType and \"UserSign\" = @userId), @dfltAllUser);\r\n                    select *, CASE when \"Series\" = @dflUser then 'Y' else 'N' end \"IsDflt\" from NNM1 where \"ObjectCode\" = @objCode and \"DocSubType\" = @docSubType;";
	}
}
