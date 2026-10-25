using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IChangeLog : IRepository<ChangeLogDifferancesParams>
{
	DataTable GetChangeLog(GetChangeLogParams pr, string sessionId);

	DataTable GetChangeLogDifferences(GetChangeLogParams pr, string sessionId);
}
