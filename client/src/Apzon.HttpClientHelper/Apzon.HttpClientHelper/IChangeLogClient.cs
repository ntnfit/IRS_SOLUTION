using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IChangeLogClient
{
	Task<DataTable> GetChangeLog(GetChangeLogParams pr);

	Task<DataTable> GetChangeLogDifferences(GetChangeLogParams pr);
}
