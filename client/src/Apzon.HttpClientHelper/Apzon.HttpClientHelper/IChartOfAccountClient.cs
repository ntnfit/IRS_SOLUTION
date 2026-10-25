using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.MasterData;

namespace Apzon.HttpClientHelper;

public interface IChartOfAccountClient
{
	Task<string> Save(List<ChartOfAccountsImpl> pr);

	Task<DataTable> GetStruct();
}
