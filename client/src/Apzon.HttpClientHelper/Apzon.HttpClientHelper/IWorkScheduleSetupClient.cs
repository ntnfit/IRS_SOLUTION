using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IWorkScheduleSetupClient
{
	Task<DataSet> GetLine(int schId);

	Task<DataTable> GetAll();

	Task<string> Remove(int schId);

	Task<string> Save(DataSet ds);
}
