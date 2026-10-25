using Apzon.Services.Repository.Mrps;

namespace Apzon.Services.HanaRepository.Mrps;

public class HanaLinesRepository : LinesRepository
{
	public HanaLinesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetChildLinesQuery()
	{
		return "declare maxId int = 0;\r\n                    declare i int = 0;\r\n                    declare tempLine int = 0;\r\n                    create local temporary table #ret as (SELECT top 0 *, cast(null as int) ID FROM APZ_OLIN);\r\n                    insert into #ret SELECT  *, row_number() over(order by \"LineID\") FROM    APZ_OLIN WHERE   \"LineID\" = @ID;\r\n    select ifnull((select max(ID) from #ret), 0) into maxId from DUMMY;\r\n    While :i <= :maxId\r\n    DO\r\n        select ifnull((select \"LineID\" from #ret where ID = :i), 0) into tempLine from DUMMY;\r\n        insert into #ret SELECT  *, :maxId + row_number() over(order by \"LineID\") FROM APZ_OLIN where \"FatherLine\" = :tempLine;\r\n        i = :i + 1;\r\n        select ifnull((select max(ID) from #ret), 0) into maxId from DUMMY;\r\n    END WHILE;\r\n\r\nSELECT  \"LineID\" FROM    #ret;";
	}
}
