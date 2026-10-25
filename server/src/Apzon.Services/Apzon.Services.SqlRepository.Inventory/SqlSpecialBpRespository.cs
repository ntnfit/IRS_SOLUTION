using Apzon.Services.Repository.Inventory;

namespace Apzon.Services.SqlRepository.Inventory;

public class SqlSpecialBpRespository : SpecialBpRespository
{
	public SqlSpecialBpRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
