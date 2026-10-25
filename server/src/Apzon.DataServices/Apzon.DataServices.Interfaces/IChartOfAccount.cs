using System.Collections.Generic;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;

namespace Apzon.DataServices.Interfaces;

public interface IChartOfAccount
{
	HttpResult Save(List<ChartOfAccountsImpl> coa, string sessionId);

	string Delete(int code, string sessionId);
}
