using Apzon.Services.Repository.HumanResources;

namespace Apzon.Services.HanaRepository.HumanResources;

public class HanaEmpContractRepository : EmpContractRepository
{
	public HanaEmpContractRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string SaveDataGetQuery()
	{
		return "\r\n                    declare i int;\r\n                    SELECT IFNULL(MAX(\"typeID\"),0) into i FROM APZ_OCTY;\r\n                    create local temporary table #tblData as (select top 0 b.* from (select 1 temp_col from dummy) a left join APZ_OCTY b on 1=2);\r\n                    exec 'insert into #tblData select * from \"#dtAPZ_OCTY\"';\r\n                    while exists(select 1 from #tblData where ifnull(\"typeID\",0) = 0)\r\n                    DO\r\n                        i = $i + 1;\r\n\t                    UPDATE top 1 #tblData SET \"typeID\"  = $i where ifnull(\"typeID\",0) = 0;\r\n                    END WHILE;\r\n\r\n                    DELETE from APZ_OCTY;\r\n                    INSERT INTO APZ_OCTY\r\n                        SELECT * FROM #tblData;";
	}
}
