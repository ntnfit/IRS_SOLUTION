using Apzon.Services.Repository.BusinessPartner;

namespace Apzon.Services.HanaRepository.BusinessPartner;

public class HanaContactPersonRepository : ContactPersonRepository
{
	public HanaContactPersonRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string ShowContactPersonQuery()
	{
		return "SELECT *\r\n                              FROM OCPR where ((\"CardCode\"=Code and Code != '') OR Code = '');";
	}
}
