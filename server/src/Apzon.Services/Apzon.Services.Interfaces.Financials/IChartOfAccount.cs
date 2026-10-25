using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.MasterData;

namespace Apzon.Services.Interfaces.Financials;

public interface IChartOfAccount : IService<ChartOfAccountsImpl>
{
	List<ChartOfAccountsImpl> Cfl();

	DataTable GetStruct();
}
