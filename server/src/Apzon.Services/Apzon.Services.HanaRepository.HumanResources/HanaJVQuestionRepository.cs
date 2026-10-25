using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.HanaRepository.HumanResources;

public class HanaJVQuestionRepository : JVQuestionRepository
{
	public HanaJVQuestionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string ProcessBenefitQuery(string tblName)
	{
		return $"declare Id int=0;\r\n\t                              create local temporary table #APZ_OBNFLocal as (select top 0 * from APZ_OBNF);\r\n\t                              exec 'drop table #APZ_OBNFLocal;';\r\n\t                              exec 'create local temporary table #APZ_OBNFLocal as (select * from #DTAPZ_OBNF);';\r\n\t                              select ifnull((select max(\"Id\") from APZ_OBNF),0) into Id from DUMMY;\r\n                                  delete from APZ_OBNF;\r\n                                  insert into APZ_OBNF(\"Id\", \"Name\", \"Remark\")\r\n                                  select :Id + row_number() over(order by \"Id\"), \"Name\", \"Remark\" from #APZ_OBNFLocal;";
	}
}
