using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaBarConfigRepository : BarConfigRepository
{
	public HanaBarConfigRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetColumnsQuery()
	{
		return "create local temporary table #temp as (select top 0 * from APZ_OCPB);\r\n                                   insert into #temp (\"ObjType\") values (202);\r\n                                   insert into #temp (\"ObjType\") values (22);\r\n                                   select * from #temp;\r\n                                   drop table #temp;";
	}

	protected override string GetSaveQuery()
	{
		return "create local temporary table #tempSave as (select top 0 * from APZ_OCPB);\r\n                                            exec 'drop table #tempSave;';\r\n                                            exec 'create local temporary table #tempSave as (select * from \"#temp\");';\r\n                                            delete from APZ_OCPB a where exists(select 1 from #tempSave where \"ObjType\" = a.\"ObjType\");\r\n                                            insert into APZ_OCPB select * from #tempSave;";
	}
}
