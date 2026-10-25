using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.HanaRepository.HumanResources;

public class HanaJVInterviewRepository : JVInterviewRepository
{
	public HanaJVInterviewRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public override string ProcessInterviewStatusQuery(string tblName)
	{
		return $"declare Id int=0;\r\n\t            create local temporary table #APZ_OHISLocal as (select top 0 * from APZ_OHIS);\r\n\t            exec 'drop table #APZ_OHISLocal;';\r\n\t            exec 'create local temporary table #APZ_OHISLocal as (select * from \"{tblName}\");';\r\n\t            select ifnull((select max(\"Id\") from APZ_OHIS),0) into Id from DUMMY;\r\n                delete from APZ_OHIS;\r\n                insert into APZ_OHIS(\"Id\", \"Name\", \"Remark\")\r\n                select :Id + row_number() over(order by \"Id\" asc),\"Name\",\"Remark\" from #APZ_OHISLocal;";
	}

	public override string ProcessInterviewTypeQuery(string tblName)
	{
		return $"declare Id int=0;\r\n\t            create local temporary table #APZ_OHITLocal as (select top 0 * from APZ_OHIT);\r\n\t            exec 'drop table #APZ_OHITLocal;';\r\n\t            exec 'create local temporary table #APZ_OHITLocal as (select * from \"{tblName}\");';\r\n\t            select ifnull((select max(\"Id\") from APZ_OHIT),0) into Id from DUMMY;\r\n                delete from APZ_OHIT;\r\n                insert into APZ_OHIT(\"Id\", \"Name\", \"Remark\")\r\n                select :Id + row_number() over(order by \"Id\" asc) as \"Id\" ,\"Name\",\"Remark\" from #APZ_OHITLocal;";
	}
}
