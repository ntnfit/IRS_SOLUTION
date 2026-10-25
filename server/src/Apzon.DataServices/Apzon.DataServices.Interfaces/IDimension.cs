using System.Data;
using System.Threading.Tasks;

namespace Apzon.DataServices.Interfaces;

public interface IDimension : IRepository<DataTable>
{
	Task<string> Save(string sessionId, DataTable dt);
}
