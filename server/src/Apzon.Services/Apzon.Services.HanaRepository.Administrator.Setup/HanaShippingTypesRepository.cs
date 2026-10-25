using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaShippingTypesRepository : ShippingTypesRepository
{
	public HanaShippingTypesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string ShowShippingQueryString()
	{
		return base.ShowShippingQueryString() + ";";
	}

	protected override string GetMaxCodeQuery()
	{
		return "SELECT IFNULL(MAX(\"TrnspCode\"), 0) AS \"TrnspCode\" FROM OSHP;";
	}
}
