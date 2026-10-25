using System.Data;
using System.Threading.Tasks;

namespace Apzon.HttpClientHelper;

public interface IWorkShiftClient
{
	Task<DataTable> GetLine(int shiftID);

	Task<DataTable> GetAll();

	Task<string> Remove(int methodId);

	Task<string> Save(DataSet ds);
}
