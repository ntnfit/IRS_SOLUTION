using Apzon.Services.Repository.Administrator.SystemInitialization;

namespace Apzon.Services.HanaRepository.Administrator.SystemInitialization;

public class HanaCompanyDetailsRepository : CompanyDetailsRepository
{
	public HanaCompanyDetailsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string ExistsTableQuery()
	{
		return "declare active int=0;\r\n\t                    select (select cast(count(1) as int) from tables where table_name=@Table and schema_name = current_schema) INTO active from DUMMY;\r\n\t                    if active > 0 then\r\n\t\t                    select 'TRUE' as Exists from DUMMY;\r\n\t                    end if;";
	}

	protected override string GetOADMQuery()
	{
		return "select a.*,b.\"EnbSgmnAct\",b.\"RelStkNoPr\",b.\"DocNmMtd\" from OADM a,CINF b limit 1;";
	}
}
