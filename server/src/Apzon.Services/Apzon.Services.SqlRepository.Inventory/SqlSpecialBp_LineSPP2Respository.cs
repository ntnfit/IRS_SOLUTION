using Apzon.Services.Repository.Inventory;

namespace Apzon.Services.SqlRepository.Inventory;

public class SqlSpecialBp_LineSPP2Respository : SpecialBp_LineSPP2Respository
{
	public SqlSpecialBp_LineSPP2Respository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}
}
