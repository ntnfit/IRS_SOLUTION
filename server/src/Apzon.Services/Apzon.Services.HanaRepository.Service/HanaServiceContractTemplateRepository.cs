using Apzon.Services.Repository.Service;

namespace Apzon.Services.HanaRepository.Service;

public class HanaServiceContractTemplateRepository : ServiceContractTemplateRepository
{
	public HanaServiceContractTemplateRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public override string DeleteContractTemplateQuery()
	{
		return "create local temporary table #APZ_OCTT_DELETETMP as (select top 0 * from APZ_OCTT);\r\n\t                exec 'drop table #APZ_OCTT_DELETETMP';\r\n\t                exec 'create local temporary table #APZ_OCTT_DELETETMP as (select * from #APZ_OCTT_DELETE)';\r\n\t                delete from APZ_OCTT where \"ContractID\" in (select a.\"ContractID\" from APZ_OCTT a inner join #APZ_OCTT_DELETETMP b on a.\"ContractID\" = b.\"ContractID\");";
	}
}
