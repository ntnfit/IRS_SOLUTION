using Apzon.Services.Repository.Inventory;

namespace Apzon.Services.SqlRepository.Inventory;

public class SqlSpecialBp_LineSPP1Respository : SpecialBp_LineSPP1Respository
{
	public SqlSpecialBp_LineSPP1Respository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
