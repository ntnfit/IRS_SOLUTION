using System.Data;

namespace Apzon.Services.Interfaces.Financials;

public interface ICosting
{
	DataTable GetMaxDocEntry();

	DataTable Costing(string period);

	DataTable Cogs(string period);

	DataTable KCSX(string period);

	DataTable CostingAllocate(string period);

	bool CheckRevaluation(string period);

	bool CheckCogs(string period);

	bool CheckProd(string period);
}
